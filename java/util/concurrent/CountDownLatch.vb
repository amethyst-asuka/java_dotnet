'
' * ORACLE PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' 

'
' *
' *
' *
' *
' *
' * Written by Doug Lea with assistance from members of JCP JSR-166
' * Expert Group and released to the public domain, as explained at
' * http://creativecommons.org/publicdomain/zero/1.0/
' 

Namespace java.util.concurrent

	''' <summary>
	''' A synchronization aid that allows one or more threads to wait until
	''' a set of operations being performed in other threads completes.
	''' 
	''' <p>A {@code CountDownLatch} is initialized with a given <em>count</em>.
	''' The <seealso cref="#await await"/> methods block until the current count reaches
	''' zero due to invocations of the <seealso cref="#countDown"/> method, after which
	''' all waiting threads are released and any subsequent invocations of
	''' <seealso cref="#await await"/> return immediately.  This is a one-shot phenomenon
	''' -- the count cannot be reset.  If you need a version that resets the
	''' count, consider using a <seealso cref="CyclicBarrier"/>.
	''' 
	''' <p>A {@code CountDownLatch} is a versatile synchronization tool
	''' and can be used for a number of purposes.  A
	''' {@code CountDownLatch} initialized with a count of one serves as a
	''' simple on/off latch, or gate: all threads invoking <seealso cref="#await await"/>
	''' wait at the gate until it is opened by a thread invoking {@link
	''' #countDown}.  A {@code CountDownLatch} initialized to <em>N</em>
	''' can be used to make one thread wait until <em>N</em> threads have
	''' completed some action, or some action has been completed N times.
	''' 
	''' <p>A useful property of a {@code CountDownLatch} is that it
	''' doesn't require that threads calling {@code countDown} wait for
	''' the count to reach zero before proceeding, it simply prevents any
	''' thread from proceeding past an <seealso cref="#await await"/> until all
	''' threads could pass.
	''' 
	''' <p><b>Sample usage:</b> Here is a pair of classes in which a group
	''' of worker threads use two countdown latches:
	''' <ul>
	''' <li>The first is a start signal that prevents any worker from proceeding
	''' until the driver is ready for them to proceed;
	''' <li>The second is a completion signal that allows the driver to wait
	''' until all workers have completed.
	''' </ul>
	''' 
	'''  <pre> {@code
	''' class Driver { // ...
	'''   void main() throws InterruptedException {
	'''     CountDownLatch startSignal = new CountDownLatch(1);
	'''     CountDownLatch doneSignal = new CountDownLatch(N);
	''' 
	'''     for (int i = 0; i < N; ++i) // create and start threads
	'''       new Thread(new Worker(startSignal, doneSignal)).start();
	''' 
	'''     doSomethingElse();            // don't let run yet
	'''     startSignal.countDown();      // let all threads proceed
	'''     doSomethingElse();
	'''     doneSignal.await();           // wait for all to finish
	'''   }
	''' }
	''' 
	''' class Worker implements Runnable {
	'''   private final CountDownLatch startSignal;
	'''   private final CountDownLatch doneSignal;
	'''   Worker(CountDownLatch startSignal, CountDownLatch doneSignal) {
	'''     this.startSignal = startSignal;
	'''     this.doneSignal = doneSignal;
	'''   }
	'''   public void run() {
	'''     try {
	'''       startSignal.await();
	'''       doWork();
	'''       doneSignal.countDown();
	'''     } catch (InterruptedException ex) {} // return;
	'''   }
	''' 
	'''   void doWork() { ... }
	''' }}</pre>
	''' 
	''' <p>Another typical usage would be to divide a problem into N parts,
	''' describe each part with a Runnable that executes that portion and
	''' counts down on the latch, and queue all the Runnables to an
	''' Executor.  When all sub-parts are complete, the coordinating thread
	''' will be able to pass through await. (When threads must repeatedly
	''' count down in this way, instead use a <seealso cref="CyclicBarrier"/>.)
	''' 
	'''  <pre> {@code
	''' class Driver2 { // ...
	'''   void main() throws InterruptedException {
	'''     CountDownLatch doneSignal = new CountDownLatch(N);
	'''     Executor e = ...
	''' 
	'''     for (int i = 0; i < N; ++i) // create and start threads
	'''       e.execute(new WorkerRunnable(doneSignal, i));
	''' 
	'''     doneSignal.await();           // wait for all to finish
	'''   }
	''' }
	''' 
	''' class WorkerRunnable implements Runnable {
	'''   private final CountDownLatch doneSignal;
	'''   private final int i;
	'''   WorkerRunnable(CountDownLatch doneSignal, int i) {
	'''     this.doneSignal = doneSignal;
	'''     this.i = i;
	'''   }
	'''   public void run() {
	'''     try {
	'''       doWork(i);
	'''       doneSignal.countDown();
	'''     } catch (InterruptedException ex) {} // return;
	'''   }
	''' 
	'''   void doWork() { ... }
	''' }}</pre>
	''' 
	''' <p>Memory consistency effects: Until the count reaches
	''' zero, actions in a thread prior to calling
	''' {@code countDown()}
	''' <a href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	''' actions following a successful return from a corresponding
	''' {@code await()} in another thread.
	''' 
	''' @since 1.5
	''' @author Doug Lea
	''' </summary>
	Public Class CountDownLatch
		''' <summary>
		''' Synchronization control For CountDownLatch.
		''' Uses AQS state to represent count.
		''' </summary>
		Private NotInheritable Class Sync
			Inherits java.util.concurrent.locks.AbstractQueuedSynchronizer

			Private Const serialVersionUID As Long = 4982264981922014374L

			Friend Sub New(ByVal count As Integer)
				state = count
			End Sub

			Friend Property count As Integer
				Get
					Return state
				End Get
			End Property

			Protected Friend Overrides Function tryAcquireShared(ByVal acquires As Integer) As Integer
				Return If(state = 0, 1, -1)
			End Function

			Protected Friend Overrides Function tryReleaseShared(ByVal releases As Integer) As Boolean
				' Decrement count; signal when transition to zero
				Do
					Dim c As Integer = state
					If c = 0 Then Return False
					Dim nextc As Integer = c-1
					If compareAndSetState(c, nextc) Then Return nextc = 0
				Loop
			End Function
		End Class

		Private ReadOnly sync As Sync

		''' <summary>
		''' Constructs a {@code CountDownLatch} initialized with the given count.
		''' </summary>
		''' <param name="count"> the number of times <seealso cref="#countDown"/> must be invoked
		'''        before threads can pass through <seealso cref="#await"/> </param>
		''' <exception cref="IllegalArgumentException"> if {@code count} is negative </exception>
		Public Sub New(ByVal count As Integer)
			If count < 0 Then Throw New IllegalArgumentException("count < 0")
			Me.sync = New Sync(count)
		End Sub

		''' <summary>
		''' Causes the current thread to wait until the latch has counted down to
		''' zero, unless the thread is <seealso cref="Thread#interrupt interrupted"/>.
		''' 
		''' <p>If the current count is zero then this method returns immediately.
		''' 
		''' <p>If the current count is greater than zero then the current
		''' thread becomes disabled for thread scheduling purposes and lies
		''' dormant until one of two things happen:
		''' <ul>
		''' <li>The count reaches zero due to invocations of the
		''' <seealso cref="#countDown"/> method; or
		''' <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		''' the current thread.
		''' </ul>
		''' 
		''' <p>If the current thread:
		''' <ul>
		''' <li>has its interrupted status set on entry to this method; or
		''' <li>is <seealso cref="Thread#interrupt interrupted"/> while waiting,
		''' </ul>
		''' then <seealso cref="InterruptedException"/> is thrown and the current thread's
		''' interrupted status is cleared.
		''' </summary>
		''' <exception cref="InterruptedException"> if the current thread is interrupted
		'''         while waiting </exception>
		Public Overridable Sub [await]()
			sync.acquireSharedInterruptibly(1)
		End Sub

		''' <summary>
		''' Causes the current thread to wait until the latch has counted down to
		''' zero, unless the thread is <seealso cref="Thread#interrupt interrupted"/>,
		''' or the specified waiting time elapses.
		''' 
		''' <p>If the current count is zero then this method returns immediately
		''' with the value {@code true}.
		''' 
		''' <p>If the current count is greater than zero then the current
		''' thread becomes disabled for thread scheduling purposes and lies
		''' dormant until one of three things happen:
		''' <ul>
		''' <li>The count reaches zero due to invocations of the
		''' <seealso cref="#countDown"/> method; or
		''' <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		''' the current thread; or
		''' <li>The specified waiting time elapses.
		''' </ul>
		''' 
		''' <p>If the count reaches zero then the method returns with the
		''' value {@code true}.
		''' 
		''' <p>If the current thread:
		''' <ul>
		''' <li>has its interrupted status set on entry to this method; or
		''' <li>is <seealso cref="Thread#interrupt interrupted"/> while waiting,
		''' </ul>
		''' then <seealso cref="InterruptedException"/> is thrown and the current thread's
		''' interrupted status is cleared.
		''' 
		''' <p>If the specified waiting time elapses then the value {@code false}
		''' is returned.  If the time is less than or equal to zero, the method
		''' will not wait at all.
		''' </summary>
		''' <param name="timeout"> the maximum time to wait </param>
		''' <param name="unit"> the time unit of the {@code timeout} argument </param>
		''' <returns> {@code true} if the count reached zero and {@code false}
		'''         if the waiting time elapsed before the count reached zero </returns>
		''' <exception cref="InterruptedException"> if the current thread is interrupted
		'''         while waiting </exception>
		Public Overridable Function [await](ByVal timeout As Long, ByVal unit As TimeUnit) As Boolean
			Return sync.tryAcquireSharedNanos(1, unit.toNanos(timeout))
		End Function

		''' <summary>
		''' Decrements the count of the latch, releasing all waiting threads if
		''' the count reaches zero.
		''' 
		''' <p>If the current count is greater than zero then it is decremented.
		''' If the new count is zero then all waiting threads are re-enabled for
		''' thread scheduling purposes.
		''' 
		''' <p>If the current count equals zero then nothing happens.
		''' </summary>
		Public Overridable Sub countDown()
			sync.releaseShared(1)
		End Sub

		''' <summary>
		''' Returns the current count.
		''' 
		''' <p>This method is typically used for debugging and testing purposes.
		''' </summary>
		''' <returns> the current count </returns>
		Public Overridable Property count As Long
			Get
				Return sync.count
			End Get
		End Property

		''' <summary>
		''' Returns a string identifying this latch, as well as its state.
		''' The state, in brackets, includes the String {@code "Count ="}
		''' followed by the current count.
		''' </summary>
		''' <returns> a string identifying this latch, as well as its state </returns>
		Public Overrides Function ToString() As String
			Return MyBase.ToString() & "[Count = " & sync.count & "]"
		End Function
	End Class

End Namespace
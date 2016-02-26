Imports System
Imports System.Threading

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

Namespace java.util.concurrent.locks

	''' <summary>
	''' Basic thread blocking primitives for creating locks and other
	''' synchronization classes.
	''' 
	''' <p>This class associates, with each thread that uses it, a permit
	''' (in the sense of the {@link java.util.concurrent.Semaphore
	''' Semaphore} [Class]). A call to {@code park} will return immediately
	''' if the permit is available, consuming it in the process; otherwise
	''' it <em>may</em> block.  A call to {@code unpark} makes the permit
	''' available, if it was not already available. (Unlike with Semaphores
	''' though, permits do not accumulate. There is at most one.)
	''' 
	''' <p>Methods {@code park} and {@code unpark} provide efficient
	''' means of blocking and unblocking threads that do not encounter the
	''' problems that cause the deprecated methods {@code Thread.suspend}
	''' and {@code Thread.resume} to be unusable for such purposes: Races
	''' between one thread invoking {@code park} and another thread trying
	''' to {@code unpark} it will preserve liveness, due to the
	''' permit. Additionally, {@code park} will return if the caller's
	''' thread was interrupted, and timeout versions are supported. The
	''' {@code park} method may also return at any other time, for "no
	''' reason", so in general must be invoked within a loop that rechecks
	''' conditions upon return. In this sense {@code park} serves as an
	''' optimization of a "busy wait" that does not waste as much time
	''' spinning, but must be paired with an {@code unpark} to be
	''' effective.
	''' 
	''' <p>The three forms of {@code park} each also support a
	''' {@code blocker} object parameter. This object is recorded while
	''' the thread is blocked to permit monitoring and diagnostic tools to
	''' identify the reasons that threads are blocked. (Such tools may
	''' access blockers using method <seealso cref="#getBlocker(Thread)"/>.)
	''' The use of these forms rather than the original forms without this
	''' parameter is strongly encouraged. The normal argument to supply as
	''' a {@code blocker} within a lock implementation is {@code this}.
	''' 
	''' <p>These methods are designed to be used as tools for creating
	''' higher-level synchronization utilities, and are not in themselves
	''' useful for most concurrency control applications.  The {@code park}
	''' method is designed for use only in constructions of the form:
	''' 
	'''  <pre> {@code
	''' while (!canProceed()) { ... LockSupport.park(this); }}</pre>
	''' 
	''' where neither {@code canProceed} nor any other actions prior to the
	''' call to {@code park} entail locking or blocking.  Because only one
	''' permit is associated with each thread, any intermediary uses of
	''' {@code park} could interfere with its intended effects.
	''' 
	''' <p><b>Sample Usage.</b> Here is a sketch of a first-in-first-out
	''' non-reentrant lock class:
	'''  <pre> {@code
	''' class FIFOMutex {
	'''   private final AtomicBoolean locked = new AtomicBoolean(false);
	'''   private final Queue<Thread> waiters
	'''     = new ConcurrentLinkedQueue<Thread>();
	''' 
	'''   public void lock() {
	'''     boolean wasInterrupted = false;
	'''     Thread current = Thread.currentThread();
	'''     waiters.add(current);
	''' 
	'''     // Block while not first in queue or cannot acquire lock
	'''     while (waiters.peek() != current ||
	'''            !locked.compareAndSet(false, true)) {
	'''       LockSupport.park(this);
	'''       if (Thread.interrupted()) // ignore interrupts while waiting
	'''         wasInterrupted = true;
	'''     }
	''' 
	'''     waiters.remove();
	'''     if (wasInterrupted)          // reassert interrupt status on exit
	'''       current.interrupt();
	'''   }
	''' 
	'''   public void unlock() {
	'''     locked.set(false);
	'''     LockSupport.unpark(waiters.peek());
	'''   }
	''' }}</pre>
	''' </summary>
	Public Class LockSupport
		Private Sub New() ' Cannot be instantiated.
		End Sub

		Private Shared Sub setBlocker(ByVal t As Thread, ByVal arg As Object)
			' Even though volatile, hotspot doesn't need a write barrier here.
			UNSAFE.putObject(t, parkBlockerOffset, arg)
		End Sub

		''' <summary>
		''' Makes available the permit for the given thread, if it
		''' was not already available.  If the thread was blocked on
		''' {@code park} then it will unblock.  Otherwise, its next call
		''' to {@code park} is guaranteed not to block. This operation
		''' is not guaranteed to have any effect at all if the given
		''' thread has not been started.
		''' </summary>
		''' <param name="thread"> the thread to unpark, or {@code null}, in which case
		'''        this operation has no effect </param>
		Public Shared Sub unpark(ByVal thread_Renamed As Thread)
			If thread_Renamed IsNot Nothing Then UNSAFE.unpark(thread_Renamed)
		End Sub

		''' <summary>
		''' Disables the current thread for thread scheduling purposes unless the
		''' permit is available.
		''' 
		''' <p>If the permit is available then it is consumed and the call returns
		''' immediately; otherwise
		''' the current thread becomes disabled for thread scheduling
		''' purposes and lies dormant until one of three things happens:
		''' 
		''' <ul>
		''' <li>Some other thread invokes <seealso cref="#unpark unpark"/> with the
		''' current thread as the target; or
		''' 
		''' <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		''' the current thread; or
		''' 
		''' <li>The call spuriously (that is, for no reason) returns.
		''' </ul>
		''' 
		''' <p>This method does <em>not</em> report which of these caused the
		''' method to return. Callers should re-check the conditions which caused
		''' the thread to park in the first place. Callers may also determine,
		''' for example, the interrupt status of the thread upon return.
		''' </summary>
		''' <param name="blocker"> the synchronization object responsible for this
		'''        thread parking
		''' @since 1.6 </param>
		Public Shared Sub park(ByVal blocker As Object)
			Dim t As Thread = Thread.CurrentThread
			blockerker(t, blocker)
			UNSAFE.park(False, 0L)
			blockerker(t, Nothing)
		End Sub

		''' <summary>
		''' Disables the current thread for thread scheduling purposes, for up to
		''' the specified waiting time, unless the permit is available.
		''' 
		''' <p>If the permit is available then it is consumed and the call
		''' returns immediately; otherwise the current thread becomes disabled
		''' for thread scheduling purposes and lies dormant until one of four
		''' things happens:
		''' 
		''' <ul>
		''' <li>Some other thread invokes <seealso cref="#unpark unpark"/> with the
		''' current thread as the target; or
		''' 
		''' <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		''' the current thread; or
		''' 
		''' <li>The specified waiting time elapses; or
		''' 
		''' <li>The call spuriously (that is, for no reason) returns.
		''' </ul>
		''' 
		''' <p>This method does <em>not</em> report which of these caused the
		''' method to return. Callers should re-check the conditions which caused
		''' the thread to park in the first place. Callers may also determine,
		''' for example, the interrupt status of the thread, or the elapsed time
		''' upon return.
		''' </summary>
		''' <param name="blocker"> the synchronization object responsible for this
		'''        thread parking </param>
		''' <param name="nanos"> the maximum number of nanoseconds to wait
		''' @since 1.6 </param>
		Public Shared Sub parkNanos(ByVal blocker As Object, ByVal nanos As Long)
			If nanos > 0 Then
				Dim t As Thread = Thread.CurrentThread
				blockerker(t, blocker)
				UNSAFE.park(False, nanos)
				blockerker(t, Nothing)
			End If
		End Sub

		''' <summary>
		''' Disables the current thread for thread scheduling purposes, until
		''' the specified deadline, unless the permit is available.
		''' 
		''' <p>If the permit is available then it is consumed and the call
		''' returns immediately; otherwise the current thread becomes disabled
		''' for thread scheduling purposes and lies dormant until one of four
		''' things happens:
		''' 
		''' <ul>
		''' <li>Some other thread invokes <seealso cref="#unpark unpark"/> with the
		''' current thread as the target; or
		''' 
		''' <li>Some other thread <seealso cref="Thread#interrupt interrupts"/> the
		''' current thread; or
		''' 
		''' <li>The specified deadline passes; or
		''' 
		''' <li>The call spuriously (that is, for no reason) returns.
		''' </ul>
		''' 
		''' <p>This method does <em>not</em> report which of these caused the
		''' method to return. Callers should re-check the conditions which caused
		''' the thread to park in the first place. Callers may also determine,
		''' for example, the interrupt status of the thread, or the current time
		''' upon return.
		''' </summary>
		''' <param name="blocker"> the synchronization object responsible for this
		'''        thread parking </param>
		''' <param name="deadline"> the absolute time, in milliseconds from the Epoch,
		'''        to wait until
		''' @since 1.6 </param>
		Public Shared Sub parkUntil(ByVal blocker As Object, ByVal deadline As Long)
			Dim t As Thread = Thread.CurrentThread
			blockerker(t, blocker)
			UNSAFE.park(True, deadline)
			blockerker(t, Nothing)
		End Sub

		''' <summary>
		''' Returns the blocker object supplied to the most recent
		''' invocation of a park method that has not yet unblocked, or null
		''' if not blocked.  The value returned is just a momentary
		''' snapshot -- the thread may have since unblocked or blocked on a
		''' different blocker object.
		''' </summary>
		''' <param name="t"> the thread </param>
		''' <returns> the blocker </returns>
		''' <exception cref="NullPointerException"> if argument is null
		''' @since 1.6 </exception>
		Public Shared Function getBlocker(ByVal t As Thread) As Object
			If t Is Nothing Then Throw New NullPointerException
			Return UNSAFE.getObjectVolatile(t, parkBlockerOffset)
		End Function

		''' <summary>
		''' Disables the current thread for thread scheduling purposes unless the
		''' permit is available.
		''' 
		''' <p>If the permit is available then it is consumed and the call
		''' returns immediately; otherwise the current thread becomes disabled
		''' for thread scheduling purposes and lies dormant until one of three
		''' things happens:
		''' 
		''' <ul>
		''' 
		''' <li>Some other thread invokes <seealso cref="#unpark unpark"/> with the
		''' current thread as the target; or
		''' 
		''' <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		''' the current thread; or
		''' 
		''' <li>The call spuriously (that is, for no reason) returns.
		''' </ul>
		''' 
		''' <p>This method does <em>not</em> report which of these caused the
		''' method to return. Callers should re-check the conditions which caused
		''' the thread to park in the first place. Callers may also determine,
		''' for example, the interrupt status of the thread upon return.
		''' </summary>
		Public Shared Sub park()
			UNSAFE.park(False, 0L)
		End Sub

		''' <summary>
		''' Disables the current thread for thread scheduling purposes, for up to
		''' the specified waiting time, unless the permit is available.
		''' 
		''' <p>If the permit is available then it is consumed and the call
		''' returns immediately; otherwise the current thread becomes disabled
		''' for thread scheduling purposes and lies dormant until one of four
		''' things happens:
		''' 
		''' <ul>
		''' <li>Some other thread invokes <seealso cref="#unpark unpark"/> with the
		''' current thread as the target; or
		''' 
		''' <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		''' the current thread; or
		''' 
		''' <li>The specified waiting time elapses; or
		''' 
		''' <li>The call spuriously (that is, for no reason) returns.
		''' </ul>
		''' 
		''' <p>This method does <em>not</em> report which of these caused the
		''' method to return. Callers should re-check the conditions which caused
		''' the thread to park in the first place. Callers may also determine,
		''' for example, the interrupt status of the thread, or the elapsed time
		''' upon return.
		''' </summary>
		''' <param name="nanos"> the maximum number of nanoseconds to wait </param>
		Public Shared Sub parkNanos(ByVal nanos As Long)
			If nanos > 0 Then UNSAFE.park(False, nanos)
		End Sub

		''' <summary>
		''' Disables the current thread for thread scheduling purposes, until
		''' the specified deadline, unless the permit is available.
		''' 
		''' <p>If the permit is available then it is consumed and the call
		''' returns immediately; otherwise the current thread becomes disabled
		''' for thread scheduling purposes and lies dormant until one of four
		''' things happens:
		''' 
		''' <ul>
		''' <li>Some other thread invokes <seealso cref="#unpark unpark"/> with the
		''' current thread as the target; or
		''' 
		''' <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		''' the current thread; or
		''' 
		''' <li>The specified deadline passes; or
		''' 
		''' <li>The call spuriously (that is, for no reason) returns.
		''' </ul>
		''' 
		''' <p>This method does <em>not</em> report which of these caused the
		''' method to return. Callers should re-check the conditions which caused
		''' the thread to park in the first place. Callers may also determine,
		''' for example, the interrupt status of the thread, or the current time
		''' upon return.
		''' </summary>
		''' <param name="deadline"> the absolute time, in milliseconds from the Epoch,
		'''        to wait until </param>
		Public Shared Sub parkUntil(ByVal deadline As Long)
			UNSAFE.park(True, deadline)
		End Sub

		''' <summary>
		''' Returns the pseudo-randomly initialized or updated secondary seed.
		''' Copied from ThreadLocalRandom due to package access restrictions.
		''' </summary>
		Friend Shared Function nextSecondarySeed() As Integer
			Dim r As Integer
			Dim t As Thread = Thread.CurrentThread
			r = UNSAFE.getInt(t, SECONDARY)
			If r <> 0 Then
				r = r Xor r << 13 ' xorshift
				r = r Xor CInt(CUInt(r) >> 17)
				r = r Xor r << 5
			Else
				r = java.util.concurrent.ThreadLocalRandom.current().Next()
				If r = 0 Then r = 1 ' avoid zero
				End If
			UNSAFE.putInt(t, SECONDARY, r)
			Return r
		End Function

		' Hotspot implementation via intrinsics API
		Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
		Private Shared ReadOnly parkBlockerOffset As Long
		Private Shared ReadOnly SEED As Long
		Private Shared ReadOnly PROBE As Long
		Private Shared ReadOnly SECONDARY As Long
		Shared Sub New()
			Try
				UNSAFE = sun.misc.Unsafe.unsafe
				Dim tk As  [Class] = GetType(Thread)
				parkBlockerOffset = UNSAFE.objectFieldOffset(tk.getDeclaredField("parkBlocker"))
				SEED = UNSAFE.objectFieldOffset(tk.getDeclaredField("threadLocalRandomSeed"))
				PROBE = UNSAFE.objectFieldOffset(tk.getDeclaredField("threadLocalRandomProbe"))
				SECONDARY = UNSAFE.objectFieldOffset(tk.getDeclaredField("threadLocalRandomSecondarySeed"))
			Catch ex As Exception
				Throw New [Error](ex)
			End Try
		End Sub

	End Class

End Namespace
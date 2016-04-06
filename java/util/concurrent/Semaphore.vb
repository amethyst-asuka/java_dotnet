Imports System
Imports System.Collections.Generic

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
	''' A counting semaphore.  Conceptually, a semaphore maintains a set of
	''' permits.  Each <seealso cref="#acquire"/> blocks if necessary until a permit is
	''' available, and then takes it.  Each <seealso cref="#release"/> adds a permit,
	''' potentially releasing a blocking acquirer.
	''' However, no actual permit objects are used; the {@code Semaphore} just
	''' keeps a count of the number available and acts accordingly.
	''' 
	''' <p>Semaphores are often used to restrict the number of threads than can
	''' access some (physical or logical) resource. For example, here is
	''' a class that uses a semaphore to control access to a pool of items:
	'''  <pre> {@code
	''' class Pool {
	'''   private static final int MAX_AVAILABLE = 100;
	'''   private final Semaphore available = new Semaphore(MAX_AVAILABLE, true);
	''' 
	'''   public Object getItem() throws InterruptedException {
	'''     available.acquire();
	'''     return getNextAvailableItem();
	'''   }
	''' 
	'''   public  Sub  putItem(Object x) {
	'''     if (markAsUnused(x))
	'''       available.release();
	'''   }
	''' 
	'''   // Not a particularly efficient data structure; just for demo
	''' 
	'''   protected Object[] items = ... whatever kinds of items being managed
	'''   protected boolean[] used = new boolean[MAX_AVAILABLE];
	''' 
	'''   protected synchronized Object getNextAvailableItem() {
	'''     for (int i = 0; i < MAX_AVAILABLE; ++i) {
	'''       if (!used[i]) {
	'''          used[i] = true;
	'''          return items[i];
	'''       }
	'''     }
	'''     return null; // not reached
	'''   }
	''' 
	'''   protected synchronized boolean markAsUnused(Object item) {
	'''     for (int i = 0; i < MAX_AVAILABLE; ++i) {
	'''       if (item == items[i]) {
	'''          if (used[i]) {
	'''            used[i] = false;
	'''            return true;
	'''          } else
	'''            return false;
	'''       }
	'''     }
	'''     return false;
	'''   }
	''' }}</pre>
	''' 
	''' <p>Before obtaining an item each thread must acquire a permit from
	''' the semaphore, guaranteeing that an item is available for use. When
	''' the thread has finished with the item it is returned back to the
	''' pool and a permit is returned to the semaphore, allowing another
	''' thread to acquire that item.  Note that no synchronization lock is
	''' held when <seealso cref="#acquire"/> is called as that would prevent an item
	''' from being returned to the pool.  The semaphore encapsulates the
	''' synchronization needed to restrict access to the pool, separately
	''' from any synchronization needed to maintain the consistency of the
	''' pool itself.
	''' 
	''' <p>A semaphore initialized to one, and which is used such that it
	''' only has at most one permit available, can serve as a mutual
	''' exclusion lock.  This is more commonly known as a <em>binary
	''' semaphore</em>, because it only has two states: one permit
	''' available, or zero permits available.  When used in this way, the
	''' binary semaphore has the property (unlike many <seealso cref="java.util.concurrent.locks.Lock"/>
	''' implementations), that the &quot;lock&quot; can be released by a
	''' thread other than the owner (as semaphores have no notion of
	''' ownership).  This can be useful in some specialized contexts, such
	''' as deadlock recovery.
	''' 
	''' <p> The constructor for this class optionally accepts a
	''' <em>fairness</em> parameter. When set false, this class makes no
	''' guarantees about the order in which threads acquire permits. In
	''' particular, <em>barging</em> is permitted, that is, a thread
	''' invoking <seealso cref="#acquire"/> can be allocated a permit ahead of a
	''' thread that has been waiting - logically the new thread places itself at
	''' the head of the queue of waiting threads. When fairness is set true, the
	''' semaphore guarantees that threads invoking any of the {@link
	''' #acquire() acquire} methods are selected to obtain permits in the order in
	''' which their invocation of those methods was processed
	''' (first-in-first-out; FIFO). Note that FIFO ordering necessarily
	''' applies to specific internal points of execution within these
	''' methods.  So, it is possible for one thread to invoke
	''' {@code acquire} before another, but reach the ordering point after
	''' the other, and similarly upon return from the method.
	''' Also note that the untimed <seealso cref="#tryAcquire() tryAcquire"/> methods do not
	''' honor the fairness setting, but will take any permits that are
	''' available.
	''' 
	''' <p>Generally, semaphores used to control resource access should be
	''' initialized as fair, to ensure that no thread is starved out from
	''' accessing a resource. When using semaphores for other kinds of
	''' synchronization control, the throughput advantages of non-fair
	''' ordering often outweigh fairness considerations.
	''' 
	''' <p>This class also provides convenience methods to {@link
	''' #acquire(int) acquire} and <seealso cref="#release(int) release"/> multiple
	''' permits at a time.  Beware of the increased risk of indefinite
	''' postponement when these methods are used without fairness set true.
	''' 
	''' <p>Memory consistency effects: Actions in a thread prior to calling
	''' a "release" method such as {@code release()}
	''' <a href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	''' actions following a successful "acquire" method such as {@code acquire()}
	''' in another thread.
	''' 
	''' @since 1.5
	''' @author Doug Lea
	''' </summary>
	<Serializable> _
	Public Class Semaphore
		Private Const serialVersionUID As Long = -3222578661600680210L
		''' <summary>
		''' All mechanics via AbstractQueuedSynchronizer subclass </summary>
		Private ReadOnly sync As Sync

		''' <summary>
		''' Synchronization implementation for semaphore.  Uses AQS state
		''' to represent permits. Subclassed into fair and nonfair
		''' versions.
		''' </summary>
		Friend MustInherit Class Sync
			Inherits java.util.concurrent.locks.AbstractQueuedSynchronizer

			Private Const serialVersionUID As Long = 1192457210091910933L

			Friend Sub New(  permits As Integer)
				state = permits
			End Sub

			Friend Property permits As Integer
				Get
					Return state
				End Get
			End Property

			Friend Function nonfairTryAcquireShared(  acquires As Integer) As Integer
				Do
					Dim available As Integer = state
					Dim remaining As Integer = available - acquires
					If remaining < 0 OrElse compareAndSetState(available, remaining) Then Return remaining
				Loop
			End Function

			Protected Friend NotOverridable Overrides Function tryReleaseShared(  releases As Integer) As Boolean
				Do
					Dim current As Integer = state
					Dim [next] As Integer = current + releases
					If [next] < current Then ' overflow Throw New [Error]("Maximum permit count exceeded")
					If compareAndSetState(current, [next]) Then Return True
				Loop
			End Function

			Friend Sub reducePermits(  reductions As Integer)
				Do
					Dim current As Integer = state
					Dim [next] As Integer = current - reductions
					If [next] > current Then ' underflow Throw New [Error]("Permit count underflow")
					If compareAndSetState(current, [next]) Then Return
				Loop
			End Sub

			Friend Function drainPermits() As Integer
				Do
					Dim current As Integer = state
					If current = 0 OrElse compareAndSetState(current, 0) Then Return current
				Loop
			End Function
		End Class

		''' <summary>
		''' NonFair version
		''' </summary>
		Friend NotInheritable Class NonfairSync
			Inherits Sync

			Private Const serialVersionUID As Long = -2694183684443567898L

			Friend Sub New(  permits As Integer)
				MyBase.New(permits)
			End Sub

			Protected Friend Overrides Function tryAcquireShared(  acquires As Integer) As Integer
				Return nonfairTryAcquireShared(acquires)
			End Function
		End Class

		''' <summary>
		''' Fair version
		''' </summary>
		Friend NotInheritable Class FairSync
			Inherits Sync

			Private Const serialVersionUID As Long = 2014338818796000944L

			Friend Sub New(  permits As Integer)
				MyBase.New(permits)
			End Sub

			Protected Friend Overrides Function tryAcquireShared(  acquires As Integer) As Integer
				Do
					If hasQueuedPredecessors() Then Return -1
					Dim available As Integer = state
					Dim remaining As Integer = available - acquires
					If remaining < 0 OrElse compareAndSetState(available, remaining) Then Return remaining
				Loop
			End Function
		End Class

		''' <summary>
		''' Creates a {@code Semaphore} with the given number of
		''' permits and nonfair fairness setting.
		''' </summary>
		''' <param name="permits"> the initial number of permits available.
		'''        This value may be negative, in which case releases
		'''        must occur before any acquires will be granted. </param>
		Public Sub New(  permits As Integer)
			sync = New NonfairSync(permits)
		End Sub

		''' <summary>
		''' Creates a {@code Semaphore} with the given number of
		''' permits and the given fairness setting.
		''' </summary>
		''' <param name="permits"> the initial number of permits available.
		'''        This value may be negative, in which case releases
		'''        must occur before any acquires will be granted. </param>
		''' <param name="fair"> {@code true} if this semaphore will guarantee
		'''        first-in first-out granting of permits under contention,
		'''        else {@code false} </param>
		Public Sub New(  permits As Integer,   fair As Boolean)
			sync = If(fair, New FairSync(permits), New NonfairSync(permits))
		End Sub

		''' <summary>
		''' Acquires a permit from this semaphore, blocking until one is
		''' available, or the thread is <seealso cref="Thread#interrupt interrupted"/>.
		''' 
		''' <p>Acquires a permit, if one is available and returns immediately,
		''' reducing the number of available permits by one.
		''' 
		''' <p>If no permit is available then the current thread becomes
		''' disabled for thread scheduling purposes and lies dormant until
		''' one of two things happens:
		''' <ul>
		''' <li>Some other thread invokes the <seealso cref="#release"/> method for this
		''' semaphore and the current thread is next to be assigned a permit; or
		''' <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		''' the current thread.
		''' </ul>
		''' 
		''' <p>If the current thread:
		''' <ul>
		''' <li>has its interrupted status set on entry to this method; or
		''' <li>is <seealso cref="Thread#interrupt interrupted"/> while waiting
		''' for a permit,
		''' </ul>
		''' then <seealso cref="InterruptedException"/> is thrown and the current thread's
		''' interrupted status is cleared.
		''' </summary>
		''' <exception cref="InterruptedException"> if the current thread is interrupted </exception>
		Public Overridable Sub acquire()
			sync.acquireSharedInterruptibly(1)
		End Sub

		''' <summary>
		''' Acquires a permit from this semaphore, blocking until one is
		''' available.
		''' 
		''' <p>Acquires a permit, if one is available and returns immediately,
		''' reducing the number of available permits by one.
		''' 
		''' <p>If no permit is available then the current thread becomes
		''' disabled for thread scheduling purposes and lies dormant until
		''' some other thread invokes the <seealso cref="#release"/> method for this
		''' semaphore and the current thread is next to be assigned a permit.
		''' 
		''' <p>If the current thread is <seealso cref="Thread#interrupt interrupted"/>
		''' while waiting for a permit then it will continue to wait, but the
		''' time at which the thread is assigned a permit may change compared to
		''' the time it would have received the permit had no interruption
		''' occurred.  When the thread does return from this method its interrupt
		''' status will be set.
		''' </summary>
		Public Overridable Sub acquireUninterruptibly()
			sync.acquireShared(1)
		End Sub

		''' <summary>
		''' Acquires a permit from this semaphore, only if one is available at the
		''' time of invocation.
		''' 
		''' <p>Acquires a permit, if one is available and returns immediately,
		''' with the value {@code true},
		''' reducing the number of available permits by one.
		''' 
		''' <p>If no permit is available then this method will return
		''' immediately with the value {@code false}.
		''' 
		''' <p>Even when this semaphore has been set to use a
		''' fair ordering policy, a call to {@code tryAcquire()} <em>will</em>
		''' immediately acquire a permit if one is available, whether or not
		''' other threads are currently waiting.
		''' This &quot;barging&quot; behavior can be useful in certain
		''' circumstances, even though it breaks fairness. If you want to honor
		''' the fairness setting, then use
		''' <seealso cref="#tryAcquire(long, TimeUnit) tryAcquire(0, TimeUnit.SECONDS) "/>
		''' which is almost equivalent (it also detects interruption).
		''' </summary>
		''' <returns> {@code true} if a permit was acquired and {@code false}
		'''         otherwise </returns>
		Public Overridable Function tryAcquire() As Boolean
			Return sync.nonfairTryAcquireShared(1) >= 0
		End Function

		''' <summary>
		''' Acquires a permit from this semaphore, if one becomes available
		''' within the given waiting time and the current thread has not
		''' been <seealso cref="Thread#interrupt interrupted"/>.
		''' 
		''' <p>Acquires a permit, if one is available and returns immediately,
		''' with the value {@code true},
		''' reducing the number of available permits by one.
		''' 
		''' <p>If no permit is available then the current thread becomes
		''' disabled for thread scheduling purposes and lies dormant until
		''' one of three things happens:
		''' <ul>
		''' <li>Some other thread invokes the <seealso cref="#release"/> method for this
		''' semaphore and the current thread is next to be assigned a permit; or
		''' <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		''' the current thread; or
		''' <li>The specified waiting time elapses.
		''' </ul>
		''' 
		''' <p>If a permit is acquired then the value {@code true} is returned.
		''' 
		''' <p>If the current thread:
		''' <ul>
		''' <li>has its interrupted status set on entry to this method; or
		''' <li>is <seealso cref="Thread#interrupt interrupted"/> while waiting
		''' to acquire a permit,
		''' </ul>
		''' then <seealso cref="InterruptedException"/> is thrown and the current thread's
		''' interrupted status is cleared.
		''' 
		''' <p>If the specified waiting time elapses then the value {@code false}
		''' is returned.  If the time is less than or equal to zero, the method
		''' will not wait at all.
		''' </summary>
		''' <param name="timeout"> the maximum time to wait for a permit </param>
		''' <param name="unit"> the time unit of the {@code timeout} argument </param>
		''' <returns> {@code true} if a permit was acquired and {@code false}
		'''         if the waiting time elapsed before a permit was acquired </returns>
		''' <exception cref="InterruptedException"> if the current thread is interrupted </exception>
		Public Overridable Function tryAcquire(  timeout As Long,   unit As TimeUnit) As Boolean
			Return sync.tryAcquireSharedNanos(1, unit.toNanos(timeout))
		End Function

		''' <summary>
		''' Releases a permit, returning it to the semaphore.
		''' 
		''' <p>Releases a permit, increasing the number of available permits by
		''' one.  If any threads are trying to acquire a permit, then one is
		''' selected and given the permit that was just released.  That thread
		''' is (re)enabled for thread scheduling purposes.
		''' 
		''' <p>There is no requirement that a thread that releases a permit must
		''' have acquired that permit by calling <seealso cref="#acquire"/>.
		''' Correct usage of a semaphore is established by programming convention
		''' in the application.
		''' </summary>
		Public Overridable Sub release()
			sync.releaseShared(1)
		End Sub

		''' <summary>
		''' Acquires the given number of permits from this semaphore,
		''' blocking until all are available,
		''' or the thread is <seealso cref="Thread#interrupt interrupted"/>.
		''' 
		''' <p>Acquires the given number of permits, if they are available,
		''' and returns immediately, reducing the number of available permits
		''' by the given amount.
		''' 
		''' <p>If insufficient permits are available then the current thread becomes
		''' disabled for thread scheduling purposes and lies dormant until
		''' one of two things happens:
		''' <ul>
		''' <li>Some other thread invokes one of the <seealso cref="#release() release"/>
		''' methods for this semaphore, the current thread is next to be assigned
		''' permits and the number of available permits satisfies this request; or
		''' <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		''' the current thread.
		''' </ul>
		''' 
		''' <p>If the current thread:
		''' <ul>
		''' <li>has its interrupted status set on entry to this method; or
		''' <li>is <seealso cref="Thread#interrupt interrupted"/> while waiting
		''' for a permit,
		''' </ul>
		''' then <seealso cref="InterruptedException"/> is thrown and the current thread's
		''' interrupted status is cleared.
		''' Any permits that were to be assigned to this thread are instead
		''' assigned to other threads trying to acquire permits, as if
		''' permits had been made available by a call to <seealso cref="#release()"/>.
		''' </summary>
		''' <param name="permits"> the number of permits to acquire </param>
		''' <exception cref="InterruptedException"> if the current thread is interrupted </exception>
		''' <exception cref="IllegalArgumentException"> if {@code permits} is negative </exception>
		Public Overridable Sub acquire(  permits As Integer)
			If permits < 0 Then Throw New IllegalArgumentException
			sync.acquireSharedInterruptibly(permits)
		End Sub

		''' <summary>
		''' Acquires the given number of permits from this semaphore,
		''' blocking until all are available.
		''' 
		''' <p>Acquires the given number of permits, if they are available,
		''' and returns immediately, reducing the number of available permits
		''' by the given amount.
		''' 
		''' <p>If insufficient permits are available then the current thread becomes
		''' disabled for thread scheduling purposes and lies dormant until
		''' some other thread invokes one of the <seealso cref="#release() release"/>
		''' methods for this semaphore, the current thread is next to be assigned
		''' permits and the number of available permits satisfies this request.
		''' 
		''' <p>If the current thread is <seealso cref="Thread#interrupt interrupted"/>
		''' while waiting for permits then it will continue to wait and its
		''' position in the queue is not affected.  When the thread does return
		''' from this method its interrupt status will be set.
		''' </summary>
		''' <param name="permits"> the number of permits to acquire </param>
		''' <exception cref="IllegalArgumentException"> if {@code permits} is negative </exception>
		Public Overridable Sub acquireUninterruptibly(  permits As Integer)
			If permits < 0 Then Throw New IllegalArgumentException
			sync.acquireShared(permits)
		End Sub

		''' <summary>
		''' Acquires the given number of permits from this semaphore, only
		''' if all are available at the time of invocation.
		''' 
		''' <p>Acquires the given number of permits, if they are available, and
		''' returns immediately, with the value {@code true},
		''' reducing the number of available permits by the given amount.
		''' 
		''' <p>If insufficient permits are available then this method will return
		''' immediately with the value {@code false} and the number of available
		''' permits is unchanged.
		''' 
		''' <p>Even when this semaphore has been set to use a fair ordering
		''' policy, a call to {@code tryAcquire} <em>will</em>
		''' immediately acquire a permit if one is available, whether or
		''' not other threads are currently waiting.  This
		''' &quot;barging&quot; behavior can be useful in certain
		''' circumstances, even though it breaks fairness. If you want to
		''' honor the fairness setting, then use {@link #tryAcquire(int,
		''' long, TimeUnit) tryAcquire(permits, 0, TimeUnit.SECONDS) }
		''' which is almost equivalent (it also detects interruption).
		''' </summary>
		''' <param name="permits"> the number of permits to acquire </param>
		''' <returns> {@code true} if the permits were acquired and
		'''         {@code false} otherwise </returns>
		''' <exception cref="IllegalArgumentException"> if {@code permits} is negative </exception>
		Public Overridable Function tryAcquire(  permits As Integer) As Boolean
			If permits < 0 Then Throw New IllegalArgumentException
			Return sync.nonfairTryAcquireShared(permits) >= 0
		End Function

		''' <summary>
		''' Acquires the given number of permits from this semaphore, if all
		''' become available within the given waiting time and the current
		''' thread has not been <seealso cref="Thread#interrupt interrupted"/>.
		''' 
		''' <p>Acquires the given number of permits, if they are available and
		''' returns immediately, with the value {@code true},
		''' reducing the number of available permits by the given amount.
		''' 
		''' <p>If insufficient permits are available then
		''' the current thread becomes disabled for thread scheduling
		''' purposes and lies dormant until one of three things happens:
		''' <ul>
		''' <li>Some other thread invokes one of the <seealso cref="#release() release"/>
		''' methods for this semaphore, the current thread is next to be assigned
		''' permits and the number of available permits satisfies this request; or
		''' <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		''' the current thread; or
		''' <li>The specified waiting time elapses.
		''' </ul>
		''' 
		''' <p>If the permits are acquired then the value {@code true} is returned.
		''' 
		''' <p>If the current thread:
		''' <ul>
		''' <li>has its interrupted status set on entry to this method; or
		''' <li>is <seealso cref="Thread#interrupt interrupted"/> while waiting
		''' to acquire the permits,
		''' </ul>
		''' then <seealso cref="InterruptedException"/> is thrown and the current thread's
		''' interrupted status is cleared.
		''' Any permits that were to be assigned to this thread, are instead
		''' assigned to other threads trying to acquire permits, as if
		''' the permits had been made available by a call to <seealso cref="#release()"/>.
		''' 
		''' <p>If the specified waiting time elapses then the value {@code false}
		''' is returned.  If the time is less than or equal to zero, the method
		''' will not wait at all.  Any permits that were to be assigned to this
		''' thread, are instead assigned to other threads trying to acquire
		''' permits, as if the permits had been made available by a call to
		''' <seealso cref="#release()"/>.
		''' </summary>
		''' <param name="permits"> the number of permits to acquire </param>
		''' <param name="timeout"> the maximum time to wait for the permits </param>
		''' <param name="unit"> the time unit of the {@code timeout} argument </param>
		''' <returns> {@code true} if all permits were acquired and {@code false}
		'''         if the waiting time elapsed before all permits were acquired </returns>
		''' <exception cref="InterruptedException"> if the current thread is interrupted </exception>
		''' <exception cref="IllegalArgumentException"> if {@code permits} is negative </exception>
		Public Overridable Function tryAcquire(  permits As Integer,   timeout As Long,   unit As TimeUnit) As Boolean
			If permits < 0 Then Throw New IllegalArgumentException
			Return sync.tryAcquireSharedNanos(permits, unit.toNanos(timeout))
		End Function

		''' <summary>
		''' Releases the given number of permits, returning them to the semaphore.
		''' 
		''' <p>Releases the given number of permits, increasing the number of
		''' available permits by that amount.
		''' If any threads are trying to acquire permits, then one
		''' is selected and given the permits that were just released.
		''' If the number of available permits satisfies that thread's request
		''' then that thread is (re)enabled for thread scheduling purposes;
		''' otherwise the thread will wait until sufficient permits are available.
		''' If there are still permits available
		''' after this thread's request has been satisfied, then those permits
		''' are assigned in turn to other threads trying to acquire permits.
		''' 
		''' <p>There is no requirement that a thread that releases a permit must
		''' have acquired that permit by calling <seealso cref="Semaphore#acquire acquire"/>.
		''' Correct usage of a semaphore is established by programming convention
		''' in the application.
		''' </summary>
		''' <param name="permits"> the number of permits to release </param>
		''' <exception cref="IllegalArgumentException"> if {@code permits} is negative </exception>
		Public Overridable Sub release(  permits As Integer)
			If permits < 0 Then Throw New IllegalArgumentException
			sync.releaseShared(permits)
		End Sub

		''' <summary>
		''' Returns the current number of permits available in this semaphore.
		''' 
		''' <p>This method is typically used for debugging and testing purposes.
		''' </summary>
		''' <returns> the number of permits available in this semaphore </returns>
		Public Overridable Function availablePermits() As Integer
			Return sync.permits
		End Function

		''' <summary>
		''' Acquires and returns all permits that are immediately available.
		''' </summary>
		''' <returns> the number of permits acquired </returns>
		Public Overridable Function drainPermits() As Integer
			Return sync.drainPermits()
		End Function

		''' <summary>
		''' Shrinks the number of available permits by the indicated
		''' reduction. This method can be useful in subclasses that use
		''' semaphores to track resources that become unavailable. This
		''' method differs from {@code acquire} in that it does not block
		''' waiting for permits to become available.
		''' </summary>
		''' <param name="reduction"> the number of permits to remove </param>
		''' <exception cref="IllegalArgumentException"> if {@code reduction} is negative </exception>
		Protected Friend Overridable Sub reducePermits(  reduction As Integer)
			If reduction < 0 Then Throw New IllegalArgumentException
			sync.reducePermits(reduction)
		End Sub

		''' <summary>
		''' Returns {@code true} if this semaphore has fairness set true.
		''' </summary>
		''' <returns> {@code true} if this semaphore has fairness set true </returns>
		Public Overridable Property fair As Boolean
			Get
				Return TypeOf sync Is FairSync
			End Get
		End Property

		''' <summary>
		''' Queries whether any threads are waiting to acquire. Note that
		''' because cancellations may occur at any time, a {@code true}
		''' return does not guarantee that any other thread will ever
		''' acquire.  This method is designed primarily for use in
		''' monitoring of the system state.
		''' </summary>
		''' <returns> {@code true} if there may be other threads waiting to
		'''         acquire the lock </returns>
		Public Function hasQueuedThreads() As Boolean
			Return sync.hasQueuedThreads()
		End Function

		''' <summary>
		''' Returns an estimate of the number of threads waiting to acquire.
		''' The value is only an estimate because the number of threads may
		''' change dynamically while this method traverses internal data
		''' structures.  This method is designed for use in monitoring of the
		''' system state, not for synchronization control.
		''' </summary>
		''' <returns> the estimated number of threads waiting for this lock </returns>
		Public Property queueLength As Integer
			Get
				Return sync.queueLength
			End Get
		End Property

		''' <summary>
		''' Returns a collection containing threads that may be waiting to acquire.
		''' Because the actual set of threads may change dynamically while
		''' constructing this result, the returned collection is only a best-effort
		''' estimate.  The elements of the returned collection are in no particular
		''' order.  This method is designed to facilitate construction of
		''' subclasses that provide more extensive monitoring facilities.
		''' </summary>
		''' <returns> the collection of threads </returns>
		Protected Friend Overridable Property queuedThreads As ICollection(Of Thread)
			Get
				Return sync.queuedThreads
			End Get
		End Property

		''' <summary>
		''' Returns a string identifying this semaphore, as well as its state.
		''' The state, in brackets, includes the String {@code "Permits ="}
		''' followed by the number of permits.
		''' </summary>
		''' <returns> a string identifying this semaphore, as well as its state </returns>
		Public Overrides Function ToString() As String
			Return MyBase.ToString() & "[Permits = " & sync.permits & "]"
		End Function
	End Class

End Namespace
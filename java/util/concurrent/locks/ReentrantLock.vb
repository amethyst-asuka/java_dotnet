Imports System
Imports System.Collections.Generic
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
	''' A reentrant mutual exclusion <seealso cref="Lock"/> with the same basic
	''' behavior and semantics as the implicit monitor lock accessed using
	''' {@code synchronized} methods and statements, but with extended
	''' capabilities.
	''' 
	''' <p>A {@code ReentrantLock} is <em>owned</em> by the thread last
	''' successfully locking, but not yet unlocking it. A thread invoking
	''' {@code lock} will return, successfully acquiring the lock, when
	''' the lock is not owned by another thread. The method will return
	''' immediately if the current thread already owns the lock. This can
	''' be checked using methods <seealso cref="#isHeldByCurrentThread"/>, and {@link
	''' #getHoldCount}.
	''' 
	''' <p>The constructor for this class accepts an optional
	''' <em>fairness</em> parameter.  When set {@code true}, under
	''' contention, locks favor granting access to the longest-waiting
	''' thread.  Otherwise this lock does not guarantee any particular
	''' access order.  Programs using fair locks accessed by many threads
	''' may display lower overall throughput (i.e., are slower; often much
	''' slower) than those using the default setting, but have smaller
	''' variances in times to obtain locks and guarantee lack of
	''' starvation. Note however, that fairness of locks does not guarantee
	''' fairness of thread scheduling. Thus, one of many threads using a
	''' fair lock may obtain it multiple times in succession while other
	''' active threads are not progressing and not currently holding the
	''' lock.
	''' Also note that the untimed <seealso cref="#tryLock()"/> method does not
	''' honor the fairness setting. It will succeed if the lock
	''' is available even if other threads are waiting.
	''' 
	''' <p>It is recommended practice to <em>always</em> immediately
	''' follow a call to {@code lock} with a {@code try} block, most
	''' typically in a before/after construction such as:
	''' 
	'''  <pre> {@code
	''' class X {
	'''   private final ReentrantLock lock = new ReentrantLock();
	'''   // ...
	''' 
	'''   public void m() {
	'''     lock.lock();  // block until condition holds
	'''     try {
	'''       // ... method body
	'''     } finally {
	'''       lock.unlock()
	'''     }
	'''   }
	''' }}</pre>
	''' 
	''' <p>In addition to implementing the <seealso cref="Lock"/> interface, this
	''' class defines a number of {@code public} and {@code protected}
	''' methods for inspecting the state of the lock.  Some of these
	''' methods are only useful for instrumentation and monitoring.
	''' 
	''' <p>Serialization of this class behaves in the same way as built-in
	''' locks: a deserialized lock is in the unlocked state, regardless of
	''' its state when serialized.
	''' 
	''' <p>This lock supports a maximum of 2147483647 recursive locks by
	''' the same thread. Attempts to exceed this limit result in
	''' <seealso cref="Error"/> throws from locking methods.
	''' 
	''' @since 1.5
	''' @author Doug Lea
	''' </summary>
	<Serializable> _
	Public Class ReentrantLock
		Implements Lock

		Private Const serialVersionUID As Long = 7373984872572414699L
		''' <summary>
		''' Synchronizer providing all implementation mechanics </summary>
		Private ReadOnly sync As Sync

		''' <summary>
		''' Base of synchronization control for this lock. Subclassed
		''' into fair and nonfair versions below. Uses AQS state to
		''' represent the number of holds on the lock.
		''' </summary>
		Friend MustInherit Class Sync
			Inherits AbstractQueuedSynchronizer

			Private Const serialVersionUID As Long = -5179523762034025860L

			''' <summary>
			''' Performs <seealso cref="Lock#lock"/>. The main reason for subclassing
			''' is to allow fast path for nonfair version.
			''' </summary>
			Friend MustOverride Sub lock()

			''' <summary>
			''' Performs non-fair tryLock.  tryAcquire is implemented in
			''' subclasses, but both need nonfair try for trylock method.
			''' </summary>
			Friend Function nonfairTryAcquire(ByVal acquires As Integer) As Boolean
				Dim current As Thread = Thread.CurrentThread
				Dim c As Integer = state
				If c = 0 Then
					If compareAndSetState(0, acquires) Then
						exclusiveOwnerThread = current
						Return True
					End If
				ElseIf current Is exclusiveOwnerThread Then
					Dim nextc As Integer = c + acquires
					If nextc < 0 Then ' overflow Throw New [Error]("Maximum lock count exceeded")
					state = nextc
					Return True
				End If
				Return False
			End Function

			Protected Friend NotOverridable Overrides Function tryRelease(ByVal releases As Integer) As Boolean
				Dim c As Integer = state - releases
				If Thread.CurrentThread IsNot exclusiveOwnerThread Then Throw New IllegalMonitorStateException
				Dim free As Boolean = False
				If c = 0 Then
					free = True
					exclusiveOwnerThread = Nothing
				End If
				state = c
				Return free
			End Function

			Protected Friend Property NotOverridable Overrides heldExclusively As Boolean
				Get
					' While we must in general read state before owner,
					' we don't need to do so to check if current thread is owner
					Return exclusiveOwnerThread Is Thread.CurrentThread
				End Get
			End Property

			Friend Function newCondition() As ConditionObject
				Return New ConditionObject
			End Function

			' Methods relayed from outer class

			Friend Property owner As Thread
				Get
					Return If(state = 0, Nothing, exclusiveOwnerThread)
				End Get
			End Property

			Friend Property holdCount As Integer
				Get
					Return If(heldExclusively, state, 0)
				End Get
			End Property

			Friend Property locked As Boolean
				Get
					Return state <> 0
				End Get
			End Property

			''' <summary>
			''' Reconstitutes the instance from a stream (that is, deserializes it).
			''' </summary>
			Private Sub readObject(ByVal s As java.io.ObjectInputStream)
				s.defaultReadObject()
				state = 0 ' reset to unlocked state
			End Sub
		End Class

		''' <summary>
		''' Sync object for non-fair locks
		''' </summary>
		Friend NotInheritable Class NonfairSync
			Inherits Sync

			Private Const serialVersionUID As Long = 7316153563782823691L

			''' <summary>
			''' Performs lock.  Try immediate barge, backing up to normal
			''' acquire on failure.
			''' </summary>
			Friend NotOverridable Overrides Sub lock()
				If compareAndSetState(0, 1) Then
					exclusiveOwnerThread = Thread.CurrentThread
				Else
					acquire(1)
				End If
			End Sub

			Protected Friend NotOverridable Overrides Function tryAcquire(ByVal acquires As Integer) As Boolean
				Return nonfairTryAcquire(acquires)
			End Function
		End Class

		''' <summary>
		''' Sync object for fair locks
		''' </summary>
		Friend NotInheritable Class FairSync
			Inherits Sync

			Private Const serialVersionUID As Long = -3000897897090466540L

			Friend NotOverridable Overrides Sub lock()
				acquire(1)
			End Sub

			''' <summary>
			''' Fair version of tryAcquire.  Don't grant access unless
			''' recursive call or no waiters or is first.
			''' </summary>
			Protected Friend NotOverridable Overrides Function tryAcquire(ByVal acquires As Integer) As Boolean
				Dim current As Thread = Thread.CurrentThread
				Dim c As Integer = state
				If c = 0 Then
					If (Not hasQueuedPredecessors()) AndAlso compareAndSetState(0, acquires) Then
						exclusiveOwnerThread = current
						Return True
					End If
				ElseIf current Is exclusiveOwnerThread Then
					Dim nextc As Integer = c + acquires
					If nextc < 0 Then Throw New [Error]("Maximum lock count exceeded")
					state = nextc
					Return True
				End If
				Return False
			End Function
		End Class

		''' <summary>
		''' Creates an instance of {@code ReentrantLock}.
		''' This is equivalent to using {@code ReentrantLock(false)}.
		''' </summary>
		Public Sub New()
			sync = New NonfairSync
		End Sub

		''' <summary>
		''' Creates an instance of {@code ReentrantLock} with the
		''' given fairness policy.
		''' </summary>
		''' <param name="fair"> {@code true} if this lock should use a fair ordering policy </param>
		Public Sub New(ByVal fair As Boolean)
			sync = If(fair, New FairSync, New NonfairSync)
		End Sub

		''' <summary>
		''' Acquires the lock.
		''' 
		''' <p>Acquires the lock if it is not held by another thread and returns
		''' immediately, setting the lock hold count to one.
		''' 
		''' <p>If the current thread already holds the lock then the hold
		''' count is incremented by one and the method returns immediately.
		''' 
		''' <p>If the lock is held by another thread then the
		''' current thread becomes disabled for thread scheduling
		''' purposes and lies dormant until the lock has been acquired,
		''' at which time the lock hold count is set to one.
		''' </summary>
		Public Overridable Sub lock() Implements Lock.lock
			sync.lock()
		End Sub

		''' <summary>
		''' Acquires the lock unless the current thread is
		''' <seealso cref="Thread#interrupt interrupted"/>.
		''' 
		''' <p>Acquires the lock if it is not held by another thread and returns
		''' immediately, setting the lock hold count to one.
		''' 
		''' <p>If the current thread already holds this lock then the hold count
		''' is incremented by one and the method returns immediately.
		''' 
		''' <p>If the lock is held by another thread then the
		''' current thread becomes disabled for thread scheduling
		''' purposes and lies dormant until one of two things happens:
		''' 
		''' <ul>
		''' 
		''' <li>The lock is acquired by the current thread; or
		''' 
		''' <li>Some other thread <seealso cref="Thread#interrupt interrupts"/> the
		''' current thread.
		''' 
		''' </ul>
		''' 
		''' <p>If the lock is acquired by the current thread then the lock hold
		''' count is set to one.
		''' 
		''' <p>If the current thread:
		''' 
		''' <ul>
		''' 
		''' <li>has its interrupted status set on entry to this method; or
		''' 
		''' <li>is <seealso cref="Thread#interrupt interrupted"/> while acquiring
		''' the lock,
		''' 
		''' </ul>
		''' 
		''' then <seealso cref="InterruptedException"/> is thrown and the current thread's
		''' interrupted status is cleared.
		''' 
		''' <p>In this implementation, as this method is an explicit
		''' interruption point, preference is given to responding to the
		''' interrupt over normal or reentrant acquisition of the lock.
		''' </summary>
		''' <exception cref="InterruptedException"> if the current thread is interrupted </exception>
		Public Overridable Sub lockInterruptibly() Implements Lock.lockInterruptibly
			sync.acquireInterruptibly(1)
		End Sub

		''' <summary>
		''' Acquires the lock only if it is not held by another thread at the time
		''' of invocation.
		''' 
		''' <p>Acquires the lock if it is not held by another thread and
		''' returns immediately with the value {@code true}, setting the
		''' lock hold count to one. Even when this lock has been set to use a
		''' fair ordering policy, a call to {@code tryLock()} <em>will</em>
		''' immediately acquire the lock if it is available, whether or not
		''' other threads are currently waiting for the lock.
		''' This &quot;barging&quot; behavior can be useful in certain
		''' circumstances, even though it breaks fairness. If you want to honor
		''' the fairness setting for this lock, then use
		''' <seealso cref="#tryLock(long, TimeUnit) tryLock(0, TimeUnit.SECONDS) "/>
		''' which is almost equivalent (it also detects interruption).
		''' 
		''' <p>If the current thread already holds this lock then the hold
		''' count is incremented by one and the method returns {@code true}.
		''' 
		''' <p>If the lock is held by another thread then this method will return
		''' immediately with the value {@code false}.
		''' </summary>
		''' <returns> {@code true} if the lock was free and was acquired by the
		'''         current thread, or the lock was already held by the current
		'''         thread; and {@code false} otherwise </returns>
		Public Overridable Function tryLock() As Boolean Implements Lock.tryLock
			Return sync.nonfairTryAcquire(1)
		End Function

		''' <summary>
		''' Acquires the lock if it is not held by another thread within the given
		''' waiting time and the current thread has not been
		''' <seealso cref="Thread#interrupt interrupted"/>.
		''' 
		''' <p>Acquires the lock if it is not held by another thread and returns
		''' immediately with the value {@code true}, setting the lock hold count
		''' to one. If this lock has been set to use a fair ordering policy then
		''' an available lock <em>will not</em> be acquired if any other threads
		''' are waiting for the lock. This is in contrast to the <seealso cref="#tryLock()"/>
		''' method. If you want a timed {@code tryLock} that does permit barging on
		''' a fair lock then combine the timed and un-timed forms together:
		''' 
		'''  <pre> {@code
		''' if (lock.tryLock() ||
		'''     lock.tryLock(timeout, unit)) {
		'''   ...
		''' }}</pre>
		''' 
		''' <p>If the current thread
		''' already holds this lock then the hold count is incremented by one and
		''' the method returns {@code true}.
		''' 
		''' <p>If the lock is held by another thread then the
		''' current thread becomes disabled for thread scheduling
		''' purposes and lies dormant until one of three things happens:
		''' 
		''' <ul>
		''' 
		''' <li>The lock is acquired by the current thread; or
		''' 
		''' <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		''' the current thread; or
		''' 
		''' <li>The specified waiting time elapses
		''' 
		''' </ul>
		''' 
		''' <p>If the lock is acquired then the value {@code true} is returned and
		''' the lock hold count is set to one.
		''' 
		''' <p>If the current thread:
		''' 
		''' <ul>
		''' 
		''' <li>has its interrupted status set on entry to this method; or
		''' 
		''' <li>is <seealso cref="Thread#interrupt interrupted"/> while
		''' acquiring the lock,
		''' 
		''' </ul>
		''' then <seealso cref="InterruptedException"/> is thrown and the current thread's
		''' interrupted status is cleared.
		''' 
		''' <p>If the specified waiting time elapses then the value {@code false}
		''' is returned.  If the time is less than or equal to zero, the method
		''' will not wait at all.
		''' 
		''' <p>In this implementation, as this method is an explicit
		''' interruption point, preference is given to responding to the
		''' interrupt over normal or reentrant acquisition of the lock, and
		''' over reporting the elapse of the waiting time.
		''' </summary>
		''' <param name="timeout"> the time to wait for the lock </param>
		''' <param name="unit"> the time unit of the timeout argument </param>
		''' <returns> {@code true} if the lock was free and was acquired by the
		'''         current thread, or the lock was already held by the current
		'''         thread; and {@code false} if the waiting time elapsed before
		'''         the lock could be acquired </returns>
		''' <exception cref="InterruptedException"> if the current thread is interrupted </exception>
		''' <exception cref="NullPointerException"> if the time unit is null </exception>
		Public Overridable Function tryLock(ByVal timeout As Long, ByVal unit As java.util.concurrent.TimeUnit) As Boolean Implements Lock.tryLock
			Return sync.tryAcquireNanos(1, unit.toNanos(timeout))
		End Function

		''' <summary>
		''' Attempts to release this lock.
		''' 
		''' <p>If the current thread is the holder of this lock then the hold
		''' count is decremented.  If the hold count is now zero then the lock
		''' is released.  If the current thread is not the holder of this
		''' lock then <seealso cref="IllegalMonitorStateException"/> is thrown.
		''' </summary>
		''' <exception cref="IllegalMonitorStateException"> if the current thread does not
		'''         hold this lock </exception>
		Public Overridable Sub unlock() Implements Lock.unlock
			sync.release(1)
		End Sub

		''' <summary>
		''' Returns a <seealso cref="Condition"/> instance for use with this
		''' <seealso cref="Lock"/> instance.
		''' 
		''' <p>The returned <seealso cref="Condition"/> instance supports the same
		''' usages as do the <seealso cref="Object"/> monitor methods ({@link
		''' Object#wait() wait}, <seealso cref="Object#notify notify"/>, and {@link
		''' Object#notifyAll notifyAll}) when used with the built-in
		''' monitor lock.
		''' 
		''' <ul>
		''' 
		''' <li>If this lock is not held when any of the <seealso cref="Condition"/>
		''' <seealso cref="Condition#await() waiting"/> or {@linkplain
		''' Condition#signal signalling} methods are called, then an {@link
		''' IllegalMonitorStateException} is thrown.
		''' 
		''' <li>When the condition <seealso cref="Condition#await() waiting"/>
		''' methods are called the lock is released and, before they
		''' return, the lock is reacquired and the lock hold count restored
		''' to what it was when the method was called.
		''' 
		''' <li>If a thread is <seealso cref="Thread#interrupt interrupted"/>
		''' while waiting then the wait will terminate, an {@link
		''' InterruptedException} will be thrown, and the thread's
		''' interrupted status will be cleared.
		''' 
		''' <li> Waiting threads are signalled in FIFO order.
		''' 
		''' <li>The ordering of lock reacquisition for threads returning
		''' from waiting methods is the same as for threads initially
		''' acquiring the lock, which is in the default case not specified,
		''' but for <em>fair</em> locks favors those threads that have been
		''' waiting the longest.
		''' 
		''' </ul>
		''' </summary>
		''' <returns> the Condition object </returns>
		Public Overridable Function newCondition() As Condition Implements Lock.newCondition
			Return sync.newCondition()
		End Function

		''' <summary>
		''' Queries the number of holds on this lock by the current thread.
		''' 
		''' <p>A thread has a hold on a lock for each lock action that is not
		''' matched by an unlock action.
		''' 
		''' <p>The hold count information is typically only used for testing and
		''' debugging purposes. For example, if a certain section of code should
		''' not be entered with the lock already held then we can assert that
		''' fact:
		''' 
		'''  <pre> {@code
		''' class X {
		'''   ReentrantLock lock = new ReentrantLock();
		'''   // ...
		'''   public void m() {
		'''     assert lock.getHoldCount() == 0;
		'''     lock.lock();
		'''     try {
		'''       // ... method body
		'''     } finally {
		'''       lock.unlock();
		'''     }
		'''   }
		''' }}</pre>
		''' </summary>
		''' <returns> the number of holds on this lock by the current thread,
		'''         or zero if this lock is not held by the current thread </returns>
		Public Overridable Property holdCount As Integer
			Get
				Return sync.holdCount
			End Get
		End Property

		''' <summary>
		''' Queries if this lock is held by the current thread.
		''' 
		''' <p>Analogous to the <seealso cref="Thread#holdsLock(Object)"/> method for
		''' built-in monitor locks, this method is typically used for
		''' debugging and testing. For example, a method that should only be
		''' called while a lock is held can assert that this is the case:
		''' 
		'''  <pre> {@code
		''' class X {
		'''   ReentrantLock lock = new ReentrantLock();
		'''   // ...
		''' 
		'''   public void m() {
		'''       assert lock.isHeldByCurrentThread();
		'''       // ... method body
		'''   }
		''' }}</pre>
		''' 
		''' <p>It can also be used to ensure that a reentrant lock is used
		''' in a non-reentrant manner, for example:
		''' 
		'''  <pre> {@code
		''' class X {
		'''   ReentrantLock lock = new ReentrantLock();
		'''   // ...
		''' 
		'''   public void m() {
		'''       assert !lock.isHeldByCurrentThread();
		'''       lock.lock();
		'''       try {
		'''           // ... method body
		'''       } finally {
		'''           lock.unlock();
		'''       }
		'''   }
		''' }}</pre>
		''' </summary>
		''' <returns> {@code true} if current thread holds this lock and
		'''         {@code false} otherwise </returns>
		Public Overridable Property heldByCurrentThread As Boolean
			Get
				Return sync.heldExclusively
			End Get
		End Property

		''' <summary>
		''' Queries if this lock is held by any thread. This method is
		''' designed for use in monitoring of the system state,
		''' not for synchronization control.
		''' </summary>
		''' <returns> {@code true} if any thread holds this lock and
		'''         {@code false} otherwise </returns>
		Public Overridable Property locked As Boolean
			Get
				Return sync.locked
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if this lock has fairness set true.
		''' </summary>
		''' <returns> {@code true} if this lock has fairness set true </returns>
		Public Property fair As Boolean
			Get
				Return TypeOf sync Is FairSync
			End Get
		End Property

		''' <summary>
		''' Returns the thread that currently owns this lock, or
		''' {@code null} if not owned. When this method is called by a
		''' thread that is not the owner, the return value reflects a
		''' best-effort approximation of current lock status. For example,
		''' the owner may be momentarily {@code null} even if there are
		''' threads trying to acquire the lock but have not yet done so.
		''' This method is designed to facilitate construction of
		''' subclasses that provide more extensive lock monitoring
		''' facilities.
		''' </summary>
		''' <returns> the owner, or {@code null} if not owned </returns>
		Protected Friend Overridable Property owner As Thread
			Get
				Return sync.owner
			End Get
		End Property

		''' <summary>
		''' Queries whether any threads are waiting to acquire this lock. Note that
		''' because cancellations may occur at any time, a {@code true}
		''' return does not guarantee that any other thread will ever
		''' acquire this lock.  This method is designed primarily for use in
		''' monitoring of the system state.
		''' </summary>
		''' <returns> {@code true} if there may be other threads waiting to
		'''         acquire the lock </returns>
		Public Function hasQueuedThreads() As Boolean
			Return sync.hasQueuedThreads()
		End Function

		''' <summary>
		''' Queries whether the given thread is waiting to acquire this
		''' lock. Note that because cancellations may occur at any time, a
		''' {@code true} return does not guarantee that this thread
		''' will ever acquire this lock.  This method is designed primarily for use
		''' in monitoring of the system state.
		''' </summary>
		''' <param name="thread"> the thread </param>
		''' <returns> {@code true} if the given thread is queued waiting for this lock </returns>
		''' <exception cref="NullPointerException"> if the thread is null </exception>
		Public Function hasQueuedThread(ByVal thread_Renamed As Thread) As Boolean
			Return sync.isQueued(thread_Renamed)
		End Function

		''' <summary>
		''' Returns an estimate of the number of threads waiting to
		''' acquire this lock.  The value is only an estimate because the number of
		''' threads may change dynamically while this method traverses
		''' internal data structures.  This method is designed for use in
		''' monitoring of the system state, not for synchronization
		''' control.
		''' </summary>
		''' <returns> the estimated number of threads waiting for this lock </returns>
		Public Property queueLength As Integer
			Get
				Return sync.queueLength
			End Get
		End Property

		''' <summary>
		''' Returns a collection containing threads that may be waiting to
		''' acquire this lock.  Because the actual set of threads may change
		''' dynamically while constructing this result, the returned
		''' collection is only a best-effort estimate.  The elements of the
		''' returned collection are in no particular order.  This method is
		''' designed to facilitate construction of subclasses that provide
		''' more extensive monitoring facilities.
		''' </summary>
		''' <returns> the collection of threads </returns>
		Protected Friend Overridable Property queuedThreads As ICollection(Of Thread)
			Get
				Return sync.queuedThreads
			End Get
		End Property

		''' <summary>
		''' Queries whether any threads are waiting on the given condition
		''' associated with this lock. Note that because timeouts and
		''' interrupts may occur at any time, a {@code true} return does
		''' not guarantee that a future {@code signal} will awaken any
		''' threads.  This method is designed primarily for use in
		''' monitoring of the system state.
		''' </summary>
		''' <param name="condition"> the condition </param>
		''' <returns> {@code true} if there are any waiting threads </returns>
		''' <exception cref="IllegalMonitorStateException"> if this lock is not held </exception>
		''' <exception cref="IllegalArgumentException"> if the given condition is
		'''         not associated with this lock </exception>
		''' <exception cref="NullPointerException"> if the condition is null </exception>
		Public Overridable Function hasWaiters(ByVal condition As Condition) As Boolean
			If condition Is Nothing Then Throw New NullPointerException
			If Not(TypeOf condition Is AbstractQueuedSynchronizer.ConditionObject) Then Throw New IllegalArgumentException("not owner")
			Return sync.hasWaiters(CType(condition, AbstractQueuedSynchronizer.ConditionObject))
		End Function

		''' <summary>
		''' Returns an estimate of the number of threads waiting on the
		''' given condition associated with this lock. Note that because
		''' timeouts and interrupts may occur at any time, the estimate
		''' serves only as an upper bound on the actual number of waiters.
		''' This method is designed for use in monitoring of the system
		''' state, not for synchronization control.
		''' </summary>
		''' <param name="condition"> the condition </param>
		''' <returns> the estimated number of waiting threads </returns>
		''' <exception cref="IllegalMonitorStateException"> if this lock is not held </exception>
		''' <exception cref="IllegalArgumentException"> if the given condition is
		'''         not associated with this lock </exception>
		''' <exception cref="NullPointerException"> if the condition is null </exception>
		Public Overridable Function getWaitQueueLength(ByVal condition As Condition) As Integer
			If condition Is Nothing Then Throw New NullPointerException
			If Not(TypeOf condition Is AbstractQueuedSynchronizer.ConditionObject) Then Throw New IllegalArgumentException("not owner")
			Return sync.getWaitQueueLength(CType(condition, AbstractQueuedSynchronizer.ConditionObject))
		End Function

		''' <summary>
		''' Returns a collection containing those threads that may be
		''' waiting on the given condition associated with this lock.
		''' Because the actual set of threads may change dynamically while
		''' constructing this result, the returned collection is only a
		''' best-effort estimate. The elements of the returned collection
		''' are in no particular order.  This method is designed to
		''' facilitate construction of subclasses that provide more
		''' extensive condition monitoring facilities.
		''' </summary>
		''' <param name="condition"> the condition </param>
		''' <returns> the collection of threads </returns>
		''' <exception cref="IllegalMonitorStateException"> if this lock is not held </exception>
		''' <exception cref="IllegalArgumentException"> if the given condition is
		'''         not associated with this lock </exception>
		''' <exception cref="NullPointerException"> if the condition is null </exception>
		Protected Friend Overridable Function getWaitingThreads(ByVal condition As Condition) As ICollection(Of Thread)
			If condition Is Nothing Then Throw New NullPointerException
			If Not(TypeOf condition Is AbstractQueuedSynchronizer.ConditionObject) Then Throw New IllegalArgumentException("not owner")
			Return sync.getWaitingThreads(CType(condition, AbstractQueuedSynchronizer.ConditionObject))
		End Function

		''' <summary>
		''' Returns a string identifying this lock, as well as its lock state.
		''' The state, in brackets, includes either the String {@code "Unlocked"}
		''' or the String {@code "Locked by"} followed by the
		''' <seealso cref="Thread#getName name"/> of the owning thread.
		''' </summary>
		''' <returns> a string identifying this lock, as well as its lock state </returns>
		Public Overrides Function ToString() As String
			Dim o As Thread = sync.owner
			Return MyBase.ToString() & (If(o Is Nothing, "[Unlocked]", "[Locked by thread " & o.name & "]"))
		End Function
	End Class

End Namespace
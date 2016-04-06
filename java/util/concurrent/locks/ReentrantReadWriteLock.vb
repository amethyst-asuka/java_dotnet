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
	''' An implementation of <seealso cref="ReadWriteLock"/> supporting similar
	''' semantics to <seealso cref="ReentrantLock"/>.
	''' <p>This class has the following properties:
	''' 
	''' <ul>
	''' <li><b>Acquisition order</b>
	''' 
	''' <p>This class does not impose a reader or writer preference
	''' ordering for lock access.  However, it does support an optional
	''' <em>fairness</em> policy.
	''' 
	''' <dl>
	''' <dt><b><i>Non-fair mode (default)</i></b>
	''' <dd>When constructed as non-fair (the default), the order of entry
	''' to the read and write lock is unspecified, subject to reentrancy
	''' constraints.  A nonfair lock that is continuously contended may
	''' indefinitely postpone one or more reader or writer threads, but
	''' will normally have higher throughput than a fair lock.
	''' 
	''' <dt><b><i>Fair mode</i></b>
	''' <dd>When constructed as fair, threads contend for entry using an
	''' approximately arrival-order policy. When the currently held lock
	''' is released, either the longest-waiting single writer thread will
	''' be assigned the write lock, or if there is a group of reader threads
	''' waiting longer than all waiting writer threads, that group will be
	''' assigned the read lock.
	''' 
	''' <p>A thread that tries to acquire a fair read lock (non-reentrantly)
	''' will block if either the write lock is held, or there is a waiting
	''' writer thread. The thread will not acquire the read lock until
	''' after the oldest currently waiting writer thread has acquired and
	''' released the write lock. Of course, if a waiting writer abandons
	''' its wait, leaving one or more reader threads as the longest waiters
	''' in the queue with the write lock free, then those readers will be
	''' assigned the read lock.
	''' 
	''' <p>A thread that tries to acquire a fair write lock (non-reentrantly)
	''' will block unless both the read lock and write lock are free (which
	''' implies there are no waiting threads).  (Note that the non-blocking
	''' <seealso cref="ReadLock#tryLock()"/> and <seealso cref="WriteLock#tryLock()"/> methods
	''' do not honor this fair setting and will immediately acquire the lock
	''' if it is possible, regardless of waiting threads.)
	''' <p>
	''' </dl>
	''' 
	''' <li><b>Reentrancy</b>
	''' 
	''' <p>This lock allows both readers and writers to reacquire read or
	''' write locks in the style of a <seealso cref="ReentrantLock"/>. Non-reentrant
	''' readers are not allowed until all write locks held by the writing
	''' thread have been released.
	''' 
	''' <p>Additionally, a writer can acquire the read lock, but not
	''' vice-versa.  Among other applications, reentrancy can be useful
	''' when write locks are held during calls or callbacks to methods that
	''' perform reads under read locks.  If a reader tries to acquire the
	''' write lock it will never succeed.
	''' 
	''' <li><b>Lock downgrading</b>
	''' <p>Reentrancy also allows downgrading from the write lock to a read lock,
	''' by acquiring the write lock, then the read lock and then releasing the
	''' write lock. However, upgrading from a read lock to the write lock is
	''' <b>not</b> possible.
	''' 
	''' <li><b>Interruption of lock acquisition</b>
	''' <p>The read lock and write lock both support interruption during lock
	''' acquisition.
	''' 
	''' <li><b><seealso cref="Condition"/> support</b>
	''' <p>The write lock provides a <seealso cref="Condition"/> implementation that
	''' behaves in the same way, with respect to the write lock, as the
	''' <seealso cref="Condition"/> implementation provided by
	''' <seealso cref="ReentrantLock#newCondition"/> does for <seealso cref="ReentrantLock"/>.
	''' This <seealso cref="Condition"/> can, of course, only be used with the write lock.
	''' 
	''' <p>The read lock does not support a <seealso cref="Condition"/> and
	''' {@code readLock().newCondition()} throws
	''' {@code UnsupportedOperationException}.
	''' 
	''' <li><b>Instrumentation</b>
	''' <p>This class supports methods to determine whether locks
	''' are held or contended. These methods are designed for monitoring
	''' system state, not for synchronization control.
	''' </ul>
	''' 
	''' <p>Serialization of this class behaves in the same way as built-in
	''' locks: a deserialized lock is in the unlocked state, regardless of
	''' its state when serialized.
	''' 
	''' <p><b>Sample usages</b>. Here is a code sketch showing how to perform
	''' lock downgrading after updating a cache (exception handling is
	''' particularly tricky when handling multiple locks in a non-nested
	''' fashion):
	''' 
	''' <pre> {@code
	''' class CachedData {
	'''   Object data;
	'''   volatile boolean cacheValid;
	'''   final ReentrantReadWriteLock rwl = new ReentrantReadWriteLock();
	''' 
	'''    Sub  processCachedData() {
	'''     rwl.readLock().lock();
	'''     if (!cacheValid) {
	'''       // Must release read lock before acquiring write lock
	'''       rwl.readLock().unlock();
	'''       rwl.writeLock().lock();
	'''       try {
	'''         // Recheck state because another thread might have
	'''         // acquired write lock and changed state before we did.
	'''         if (!cacheValid) {
	'''           data = ...
	'''           cacheValid = true;
	'''         }
	'''         // Downgrade by acquiring read lock before releasing write lock
	'''         rwl.readLock().lock();
	'''       } finally {
	'''         rwl.writeLock().unlock(); // Unlock write, still hold read
	'''       }
	'''     }
	''' 
	'''     try {
	'''       use(data);
	'''     } finally {
	'''       rwl.readLock().unlock();
	'''     }
	'''   }
	''' }}</pre>
	''' 
	''' ReentrantReadWriteLocks can be used to improve concurrency in some
	''' uses of some kinds of Collections. This is typically worthwhile
	''' only when the collections are expected to be large, accessed by
	''' more reader threads than writer threads, and entail operations with
	''' overhead that outweighs synchronization overhead. For example, here
	''' is a class using a TreeMap that is expected to be large and
	''' concurrently accessed.
	''' 
	'''  <pre> {@code
	''' class RWDictionary {
	'''   private final Map<String, Data> m = new TreeMap<String, Data>();
	'''   private final ReentrantReadWriteLock rwl = new ReentrantReadWriteLock();
	'''   private final Lock r = rwl.readLock();
	'''   private final Lock w = rwl.writeLock();
	''' 
	'''   public Data get(String key) {
	'''     r.lock();
	'''     try { return m.get(key); }
	'''     finally { r.unlock(); }
	'''   }
	'''   public String[] allKeys() {
	'''     r.lock();
	'''     try { return m.keySet().toArray(); }
	'''     finally { r.unlock(); }
	'''   }
	'''   public Data put(String key, Data value) {
	'''     w.lock();
	'''     try { return m.put(key, value); }
	'''     finally { w.unlock(); }
	'''   }
	'''   public  Sub  clear() {
	'''     w.lock();
	'''     try { m.clear(); }
	'''     finally { w.unlock(); }
	'''   }
	''' }}</pre>
	''' 
	''' <h3>Implementation Notes</h3>
	''' 
	''' <p>This lock supports a maximum of 65535 recursive write locks
	''' and 65535 read locks. Attempts to exceed these limits result in
	''' <seealso cref="Error"/> throws from locking methods.
	''' 
	''' @since 1.5
	''' @author Doug Lea
	''' </summary>
	<Serializable> _
	Public Class ReentrantReadWriteLock
		Implements ReadWriteLock

		Private Const serialVersionUID As Long = -6992448646407690164L
		''' <summary>
		''' Inner class providing readlock </summary>
		Private ReadOnly readerLock As ReentrantReadWriteLock.ReadLock
		''' <summary>
		''' Inner class providing writelock </summary>
		Private ReadOnly writerLock As ReentrantReadWriteLock.WriteLock
		''' <summary>
		''' Performs all synchronization mechanics </summary>
		Friend ReadOnly sync As Sync

		''' <summary>
		''' Creates a new {@code ReentrantReadWriteLock} with
		''' default (nonfair) ordering properties.
		''' </summary>
		Public Sub New()
			Me.New(False)
		End Sub

		''' <summary>
		''' Creates a new {@code ReentrantReadWriteLock} with
		''' the given fairness policy.
		''' </summary>
		''' <param name="fair"> {@code true} if this lock should use a fair ordering policy </param>
		Public Sub New(  fair As Boolean)
			sync = If(fair, New FairSync, New NonfairSync)
			readerLock = New ReadLock(Me)
			writerLock = New WriteLock(Me)
		End Sub

		Public Overridable Function writeLock() As ReentrantReadWriteLock.WriteLock
			Return writerLock
		End Function
		Public Overridable Function readLock() As ReentrantReadWriteLock.ReadLock
			Return readerLock
		End Function

		''' <summary>
		''' Synchronization implementation for ReentrantReadWriteLock.
		''' Subclassed into fair and nonfair versions.
		''' </summary>
		Friend MustInherit Class Sync
			Inherits AbstractQueuedSynchronizer

			Private Const serialVersionUID As Long = 6317671515068378041L

	'        
	'         * Read vs write count extraction constants and functions.
	'         * Lock state is logically divided into two unsigned shorts:
	'         * The lower one representing the exclusive (writer) lock hold count,
	'         * and the upper the shared (reader) hold count.
	'         

			Friend Const SHARED_SHIFT As Integer = 16
			Friend Shared ReadOnly SHARED_UNIT As Integer = (1 << SHARED_SHIFT)
			Friend Shared ReadOnly MAX_COUNT As Integer = (1 << SHARED_SHIFT) - 1
			Friend Shared ReadOnly EXCLUSIVE_MASK As Integer = (1 << SHARED_SHIFT) - 1

			''' <summary>
			''' Returns the number of shared holds represented in count </summary>
			Friend Shared Function sharedCount(  c As Integer) As Integer
				Return CInt(CUInt(c) >> SHARED_SHIFT)
			End Function
			''' <summary>
			''' Returns the number of exclusive holds represented in count </summary>
			Friend Shared Function exclusiveCount(  c As Integer) As Integer
				Return c And EXCLUSIVE_MASK
			End Function

			''' <summary>
			''' A counter for per-thread read hold counts.
			''' Maintained as a ThreadLocal; cached in cachedHoldCounter
			''' </summary>
			Friend NotInheritable Class HoldCounter
				Friend count As Integer = 0
				' Use id, not reference, to avoid garbage retention
				Friend ReadOnly tid As Long = getThreadId(Thread.currentThread())
			End Class

			''' <summary>
			''' ThreadLocal subclass. Easiest to explicitly define for sake
			''' of deserialization mechanics.
			''' </summary>
			Friend NotInheritable Class ThreadLocalHoldCounter
				Inherits ThreadLocal(Of HoldCounter)

				Public Function initialValue() As HoldCounter
					Return New HoldCounter
				End Function
			End Class

			''' <summary>
			''' The number of reentrant read locks held by current thread.
			''' Initialized only in constructor and readObject.
			''' Removed whenever a thread's read hold count drops to 0.
			''' </summary>
			<NonSerialized> _
			Private readHolds As ThreadLocalHoldCounter

			''' <summary>
			''' The hold count of the last thread to successfully acquire
			''' readLock. This saves ThreadLocal lookup in the common case
			''' where the next thread to release is the last one to
			''' acquire. This is non-volatile since it is just used
			''' as a heuristic, and would be great for threads to cache.
			''' 
			''' <p>Can outlive the Thread for which it is caching the read
			''' hold count, but avoids garbage retention by not retaining a
			''' reference to the Thread.
			''' 
			''' <p>Accessed via a benign data race; relies on the memory
			''' model's final field and out-of-thin-air guarantees.
			''' </summary>
			<NonSerialized> _
			Private cachedHoldCounter As HoldCounter

			''' <summary>
			''' firstReader is the first thread to have acquired the read lock.
			''' firstReaderHoldCount is firstReader's hold count.
			''' 
			''' <p>More precisely, firstReader is the unique thread that last
			''' changed the shared count from 0 to 1, and has not released the
			''' read lock since then; null if there is no such thread.
			''' 
			''' <p>Cannot cause garbage retention unless the thread terminated
			''' without relinquishing its read locks, since tryReleaseShared
			''' sets it to null.
			''' 
			''' <p>Accessed via a benign data race; relies on the memory
			''' model's out-of-thin-air guarantees for references.
			''' 
			''' <p>This allows tracking of read holds for uncontended read
			''' locks to be very cheap.
			''' </summary>
			<NonSerialized> _
			Private firstReader As Thread = Nothing
			<NonSerialized> _
			Private firstReaderHoldCount As Integer

			Friend Sub New()
				readHolds = New ThreadLocalHoldCounter
				state = state ' ensures visibility of readHolds
			End Sub

	'        
	'         * Acquires and releases use the same code for fair and
	'         * nonfair locks, but differ in whether/how they allow barging
	'         * when queues are non-empty.
	'         

			''' <summary>
			''' Returns true if the current thread, when trying to acquire
			''' the read lock, and otherwise eligible to do so, should block
			''' because of policy for overtaking other waiting threads.
			''' </summary>
			Friend MustOverride Function readerShouldBlock() As Boolean

			''' <summary>
			''' Returns true if the current thread, when trying to acquire
			''' the write lock, and otherwise eligible to do so, should block
			''' because of policy for overtaking other waiting threads.
			''' </summary>
			Friend MustOverride Function writerShouldBlock() As Boolean

	'        
	'         * Note that tryRelease and tryAcquire can be called by
	'         * Conditions. So it is possible that their arguments contain
	'         * both read and write holds that are all released during a
	'         * condition wait and re-established in tryAcquire.
	'         

			Protected Friend NotOverridable Overrides Function tryRelease(  releases As Integer) As Boolean
				If Not heldExclusively Then Throw New IllegalMonitorStateException
				Dim nextc As Integer = state - releases
				Dim free As Boolean = exclusiveCount(nextc) = 0
				If free Then exclusiveOwnerThread = Nothing
				state = nextc
				Return free
			End Function

			Protected Friend NotOverridable Overrides Function tryAcquire(  acquires As Integer) As Boolean
	'            
	'             * Walkthrough:
	'             * 1. If read count nonzero or write count nonzero
	'             *    and owner is a different thread, fail.
	'             * 2. If count would saturate, fail. (This can only
	'             *    happen if count is already nonzero.)
	'             * 3. Otherwise, this thread is eligible for lock if
	'             *    it is either a reentrant acquire or
	'             *    queue policy allows it. If so, update state
	'             *    and set owner.
	'             
				Dim current As Thread = Thread.CurrentThread
				Dim c As Integer = state
				Dim w As Integer = exclusiveCount(c)
				If c <> 0 Then
					' (Note: if c != 0 and w == 0 then shared count != 0)
					If w = 0 OrElse current IsNot exclusiveOwnerThread Then Return False
					If w + exclusiveCount(acquires) > MAX_COUNT Then Throw New [Error]("Maximum lock count exceeded")
					' Reentrant acquire
					state = c + acquires
					Return True
				End If
				If writerShouldBlock() OrElse (Not compareAndSetState(c, c + acquires)) Then Return False
				exclusiveOwnerThread = current
				Return True
			End Function

			Protected Friend NotOverridable Overrides Function tryReleaseShared(  unused As Integer) As Boolean
				Dim current As Thread = Thread.CurrentThread
				If firstReader Is current Then
					' assert firstReaderHoldCount > 0;
					If firstReaderHoldCount = 1 Then
						firstReader = Nothing
					Else
						firstReaderHoldCount -= 1
					End If
				Else
					Dim rh As HoldCounter = cachedHoldCounter
					If rh Is Nothing OrElse rh.tid <> getThreadId(current) Then rh = readHolds.get()
					Dim count_Renamed As Integer = rh.count
					If count_Renamed <= 1 Then
						readHolds.remove()
						If count_Renamed <= 0 Then Throw unmatchedUnlockException()
					End If
					rh.count -= 1
				End If
				Do
					Dim c As Integer = state
					Dim nextc As Integer = c - SHARED_UNIT
					If compareAndSetState(c, nextc) Then Return nextc = 0
				Loop
			End Function

			Private Function unmatchedUnlockException() As IllegalMonitorStateException
				Return New IllegalMonitorStateException("attempt to unlock read lock, not locked by current thread")
			End Function

			Protected Friend NotOverridable Overrides Function tryAcquireShared(  unused As Integer) As Integer
	'            
	'             * Walkthrough:
	'             * 1. If write lock held by another thread, fail.
	'             * 2. Otherwise, this thread is eligible for
	'             *    lock wrt state, so ask if it should block
	'             *    because of queue policy. If not, try
	'             *    to grant by CASing state and updating count.
	'             *    Note that step does not check for reentrant
	'             *    acquires, which is postponed to full version
	'             *    to avoid having to check hold count in
	'             *    the more typical non-reentrant case.
	'             * 3. If step 2 fails either because thread
	'             *    apparently not eligible or CAS fails or count
	'             *    saturated, chain to version with full retry loop.
	'             
				Dim current As Thread = Thread.CurrentThread
				Dim c As Integer = state
				If exclusiveCount(c) <> 0 AndAlso exclusiveOwnerThread IsNot current Then Return -1
				Dim r As Integer = sharedCount(c)
				If (Not readerShouldBlock()) AndAlso r < MAX_COUNT AndAlso compareAndSetState(c, c + SHARED_UNIT) Then
					If r = 0 Then
						firstReader = current
						firstReaderHoldCount = 1
					ElseIf firstReader Is current Then
						firstReaderHoldCount += 1
					Else
						Dim rh As HoldCounter = cachedHoldCounter
						If rh Is Nothing OrElse rh.tid <> getThreadId(current) Then
								rh = readHolds.get()
								cachedHoldCounter = rh
						ElseIf rh.count = 0 Then
							readHolds.set(rh)
						End If
						rh.count += 1
					End If
					Return 1
				End If
				Return fullTryAcquireShared(current)
			End Function

			''' <summary>
			''' Full version of acquire for reads, that handles CAS misses
			''' and reentrant reads not dealt with in tryAcquireShared.
			''' </summary>
			Friend Function fullTryAcquireShared(  current As Thread) As Integer
	'            
	'             * This code is in part redundant with that in
	'             * tryAcquireShared but is simpler overall by not
	'             * complicating tryAcquireShared with interactions between
	'             * retries and lazily reading hold counts.
	'             
				Dim rh As HoldCounter = Nothing
				Do
					Dim c As Integer = state
					If exclusiveCount(c) <> 0 Then
						If exclusiveOwnerThread IsNot current Then Return -1
						' else we hold the exclusive lock; blocking here
						' would cause deadlock.
					ElseIf readerShouldBlock() Then
						' Make sure we're not acquiring read lock reentrantly
						If firstReader Is current Then
							' assert firstReaderHoldCount > 0;
						Else
							If rh Is Nothing Then
								rh = cachedHoldCounter
								If rh Is Nothing OrElse rh.tid <> getThreadId(current) Then
									rh = readHolds.get()
									If rh.count = 0 Then readHolds.remove()
								End If
							End If
							If rh.count = 0 Then Return -1
						End If
					End If
					If sharedCount(c) = MAX_COUNT Then Throw New [Error]("Maximum lock count exceeded")
					If compareAndSetState(c, c + SHARED_UNIT) Then
						If sharedCount(c) = 0 Then
							firstReader = current
							firstReaderHoldCount = 1
						ElseIf firstReader Is current Then
							firstReaderHoldCount += 1
						Else
							If rh Is Nothing Then rh = cachedHoldCounter
							If rh Is Nothing OrElse rh.tid <> getThreadId(current) Then
								rh = readHolds.get()
							ElseIf rh.count = 0 Then
								readHolds.set(rh)
							End If
							rh.count += 1
							cachedHoldCounter = rh ' cache for release
						End If
						Return 1
					End If
				Loop
			End Function

			''' <summary>
			''' Performs tryLock for write, enabling barging in both modes.
			''' This is identical in effect to tryAcquire except for lack
			''' of calls to writerShouldBlock.
			''' </summary>
			Friend Function tryWriteLock() As Boolean
				Dim current As Thread = Thread.CurrentThread
				Dim c As Integer = state
				If c <> 0 Then
					Dim w As Integer = exclusiveCount(c)
					If w = 0 OrElse current IsNot exclusiveOwnerThread Then Return False
					If w = MAX_COUNT Then Throw New [Error]("Maximum lock count exceeded")
				End If
				If Not compareAndSetState(c, c + 1) Then Return False
				exclusiveOwnerThread = current
				Return True
			End Function

			''' <summary>
			''' Performs tryLock for read, enabling barging in both modes.
			''' This is identical in effect to tryAcquireShared except for
			''' lack of calls to readerShouldBlock.
			''' </summary>
			Friend Function tryReadLock() As Boolean
				Dim current As Thread = Thread.CurrentThread
				Do
					Dim c As Integer = state
					If exclusiveCount(c) <> 0 AndAlso exclusiveOwnerThread IsNot current Then Return False
					Dim r As Integer = sharedCount(c)
					If r = MAX_COUNT Then Throw New [Error]("Maximum lock count exceeded")
					If compareAndSetState(c, c + SHARED_UNIT) Then
						If r = 0 Then
							firstReader = current
							firstReaderHoldCount = 1
						ElseIf firstReader Is current Then
							firstReaderHoldCount += 1
						Else
							Dim rh As HoldCounter = cachedHoldCounter
							If rh Is Nothing OrElse rh.tid <> getThreadId(current) Then
									rh = readHolds.get()
									cachedHoldCounter = rh
							ElseIf rh.count = 0 Then
								readHolds.set(rh)
							End If
							rh.count += 1
						End If
						Return True
					End If
				Loop
			End Function

			Protected Friend Property NotOverridable Overrides heldExclusively As Boolean
				Get
					' While we must in general read state before owner,
					' we don't need to do so to check if current thread is owner
					Return exclusiveOwnerThread Is Thread.CurrentThread
				End Get
			End Property

			' Methods relayed to outer class

			Friend Function newCondition() As ConditionObject
				Return New ConditionObject
			End Function

			Friend Property owner As Thread
				Get
					' Must read state before owner to ensure memory consistency
					Return (If(exclusiveCount(state) = 0, Nothing, exclusiveOwnerThread))
				End Get
			End Property

			Friend Property readLockCount As Integer
				Get
					Return sharedCount(state)
				End Get
			End Property

			Friend Property writeLocked As Boolean
				Get
					Return exclusiveCount(state) <> 0
				End Get
			End Property

			Friend Property writeHoldCount As Integer
				Get
					Return If(heldExclusively, exclusiveCount(state), 0)
				End Get
			End Property

			Friend Property readHoldCount As Integer
				Get
					If readLockCount = 0 Then Return 0
    
					Dim current As Thread = Thread.CurrentThread
					If firstReader Is current Then Return firstReaderHoldCount
    
					Dim rh As HoldCounter = cachedHoldCounter
					If rh IsNot Nothing AndAlso rh.tid = getThreadId(current) Then Return rh.count
    
					Dim count_Renamed As Integer = readHolds.get().count
					If count_Renamed = 0 Then readHolds.remove()
					Return count_Renamed
				End Get
			End Property

			''' <summary>
			''' Reconstitutes the instance from a stream (that is, deserializes it).
			''' </summary>
			Private Sub readObject(  s As java.io.ObjectInputStream)
				s.defaultReadObject()
				readHolds = New ThreadLocalHoldCounter
				state = 0 ' reset to unlocked state
			End Sub

			Friend Property count As Integer
				Get
					Return state
				End Get
			End Property
		End Class

		''' <summary>
		''' Nonfair version of Sync
		''' </summary>
		Friend NotInheritable Class NonfairSync
			Inherits Sync

			Private Const serialVersionUID As Long = -8159625535654395037L
			Friend NotOverridable Overrides Function writerShouldBlock() As Boolean
				Return False ' writers can always barge
			End Function
			Friend NotOverridable Overrides Function readerShouldBlock() As Boolean
	'             As a heuristic to avoid indefinite writer starvation,
	'             * block if the thread that momentarily appears to be head
	'             * of queue, if one exists, is a waiting writer.  This is
	'             * only a probabilistic effect since a new reader will not
	'             * block if there is a waiting writer behind other enabled
	'             * readers that have not yet drained from the queue.
	'             
				Return apparentlyFirstQueuedIsExclusive()
			End Function
		End Class

		''' <summary>
		''' Fair version of Sync
		''' </summary>
		Friend NotInheritable Class FairSync
			Inherits Sync

			Private Const serialVersionUID As Long = -2274990926593161451L
			Friend NotOverridable Overrides Function writerShouldBlock() As Boolean
				Return hasQueuedPredecessors()
			End Function
			Friend NotOverridable Overrides Function readerShouldBlock() As Boolean
				Return hasQueuedPredecessors()
			End Function
		End Class

		''' <summary>
		''' The lock returned by method <seealso cref="ReentrantReadWriteLock#readLock"/>.
		''' </summary>
		<Serializable> _
		Public Class ReadLock
			Implements Lock

			Private Const serialVersionUID As Long = -5992448646407690164L
			Private ReadOnly sync_Renamed As Sync

			''' <summary>
			''' Constructor for use by subclasses
			''' </summary>
			''' <param name="lock"> the outer lock object </param>
			''' <exception cref="NullPointerException"> if the lock is null </exception>
			Protected Friend Sub New(  lock As ReentrantReadWriteLock)
				sync_Renamed = lock.sync
			End Sub

			''' <summary>
			''' Acquires the read lock.
			''' 
			''' <p>Acquires the read lock if the write lock is not held by
			''' another thread and returns immediately.
			''' 
			''' <p>If the write lock is held by another thread then
			''' the current thread becomes disabled for thread scheduling
			''' purposes and lies dormant until the read lock has been acquired.
			''' </summary>
			Public Overridable Sub lock() Implements Lock.lock
				sync_Renamed.acquireShared(1)
			End Sub

			''' <summary>
			''' Acquires the read lock unless the current thread is
			''' <seealso cref="Thread#interrupt interrupted"/>.
			''' 
			''' <p>Acquires the read lock if the write lock is not held
			''' by another thread and returns immediately.
			''' 
			''' <p>If the write lock is held by another thread then the
			''' current thread becomes disabled for thread scheduling
			''' purposes and lies dormant until one of two things happens:
			''' 
			''' <ul>
			''' 
			''' <li>The read lock is acquired by the current thread; or
			''' 
			''' <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
			''' the current thread.
			''' 
			''' </ul>
			''' 
			''' <p>If the current thread:
			''' 
			''' <ul>
			''' 
			''' <li>has its interrupted status set on entry to this method; or
			''' 
			''' <li>is <seealso cref="Thread#interrupt interrupted"/> while
			''' acquiring the read lock,
			''' 
			''' </ul>
			''' 
			''' then <seealso cref="InterruptedException"/> is thrown and the current
			''' thread's interrupted status is cleared.
			''' 
			''' <p>In this implementation, as this method is an explicit
			''' interruption point, preference is given to responding to
			''' the interrupt over normal or reentrant acquisition of the
			''' lock.
			''' </summary>
			''' <exception cref="InterruptedException"> if the current thread is interrupted </exception>
			Public Overridable Sub lockInterruptibly() Implements Lock.lockInterruptibly
				sync_Renamed.acquireSharedInterruptibly(1)
			End Sub

			''' <summary>
			''' Acquires the read lock only if the write lock is not held by
			''' another thread at the time of invocation.
			''' 
			''' <p>Acquires the read lock if the write lock is not held by
			''' another thread and returns immediately with the value
			''' {@code true}. Even when this lock has been set to use a
			''' fair ordering policy, a call to {@code tryLock()}
			''' <em>will</em> immediately acquire the read lock if it is
			''' available, whether or not other threads are currently
			''' waiting for the read lock.  This &quot;barging&quot; behavior
			''' can be useful in certain circumstances, even though it
			''' breaks fairness. If you want to honor the fairness setting
			''' for this lock, then use {@link #tryLock(long, TimeUnit)
			''' tryLock(0, TimeUnit.SECONDS) } which is almost equivalent
			''' (it also detects interruption).
			''' 
			''' <p>If the write lock is held by another thread then
			''' this method will return immediately with the value
			''' {@code false}.
			''' </summary>
			''' <returns> {@code true} if the read lock was acquired </returns>
			Public Overridable Function tryLock() As Boolean Implements Lock.tryLock
				Return sync_Renamed.tryReadLock()
			End Function

			''' <summary>
			''' Acquires the read lock if the write lock is not held by
			''' another thread within the given waiting time and the
			''' current thread has not been {@link Thread#interrupt
			''' interrupted}.
			''' 
			''' <p>Acquires the read lock if the write lock is not held by
			''' another thread and returns immediately with the value
			''' {@code true}. If this lock has been set to use a fair
			''' ordering policy then an available lock <em>will not</em> be
			''' acquired if any other threads are waiting for the
			''' lock. This is in contrast to the <seealso cref="#tryLock()"/>
			''' method. If you want a timed {@code tryLock} that does
			''' permit barging on a fair lock then combine the timed and
			''' un-timed forms together:
			''' 
			'''  <pre> {@code
			''' if (lock.tryLock() ||
			'''     lock.tryLock(timeout, unit)) {
			'''   ...
			''' }}</pre>
			''' 
			''' <p>If the write lock is held by another thread then the
			''' current thread becomes disabled for thread scheduling
			''' purposes and lies dormant until one of three things happens:
			''' 
			''' <ul>
			''' 
			''' <li>The read lock is acquired by the current thread; or
			''' 
			''' <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
			''' the current thread; or
			''' 
			''' <li>The specified waiting time elapses.
			''' 
			''' </ul>
			''' 
			''' <p>If the read lock is acquired then the value {@code true} is
			''' returned.
			''' 
			''' <p>If the current thread:
			''' 
			''' <ul>
			''' 
			''' <li>has its interrupted status set on entry to this method; or
			''' 
			''' <li>is <seealso cref="Thread#interrupt interrupted"/> while
			''' acquiring the read lock,
			''' 
			''' </ul> then <seealso cref="InterruptedException"/> is thrown and the
			''' current thread's interrupted status is cleared.
			''' 
			''' <p>If the specified waiting time elapses then the value
			''' {@code false} is returned.  If the time is less than or
			''' equal to zero, the method will not wait at all.
			''' 
			''' <p>In this implementation, as this method is an explicit
			''' interruption point, preference is given to responding to
			''' the interrupt over normal or reentrant acquisition of the
			''' lock, and over reporting the elapse of the waiting time.
			''' </summary>
			''' <param name="timeout"> the time to wait for the read lock </param>
			''' <param name="unit"> the time unit of the timeout argument </param>
			''' <returns> {@code true} if the read lock was acquired </returns>
			''' <exception cref="InterruptedException"> if the current thread is interrupted </exception>
			''' <exception cref="NullPointerException"> if the time unit is null </exception>
			Public Overridable Function tryLock(  timeout As Long,   unit As java.util.concurrent.TimeUnit) As Boolean Implements Lock.tryLock
				Return sync_Renamed.tryAcquireSharedNanos(1, unit.toNanos(timeout))
			End Function

			''' <summary>
			''' Attempts to release this lock.
			''' 
			''' <p>If the number of readers is now zero then the lock
			''' is made available for write lock attempts.
			''' </summary>
			Public Overridable Sub unlock() Implements Lock.unlock
				sync_Renamed.releaseShared(1)
			End Sub

			''' <summary>
			''' Throws {@code UnsupportedOperationException} because
			''' {@code ReadLocks} do not support conditions.
			''' </summary>
			''' <exception cref="UnsupportedOperationException"> always </exception>
			Public Overridable Function newCondition() As Condition Implements Lock.newCondition
				Throw New UnsupportedOperationException
			End Function

			''' <summary>
			''' Returns a string identifying this lock, as well as its lock state.
			''' The state, in brackets, includes the String {@code "Read locks ="}
			''' followed by the number of held read locks.
			''' </summary>
			''' <returns> a string identifying this lock, as well as its lock state </returns>
			Public Overrides Function ToString() As String
				Dim r As Integer = sync_Renamed.readLockCount
				Return MyBase.ToString() & "[Read locks = " & r & "]"
			End Function
		End Class

		''' <summary>
		''' The lock returned by method <seealso cref="ReentrantReadWriteLock#writeLock"/>.
		''' </summary>
		<Serializable> _
		Public Class WriteLock
			Implements Lock

			Private Const serialVersionUID As Long = -4992448646407690164L
			Private ReadOnly sync_Renamed As Sync

			''' <summary>
			''' Constructor for use by subclasses
			''' </summary>
			''' <param name="lock"> the outer lock object </param>
			''' <exception cref="NullPointerException"> if the lock is null </exception>
			Protected Friend Sub New(  lock As ReentrantReadWriteLock)
				sync_Renamed = lock.sync
			End Sub

			''' <summary>
			''' Acquires the write lock.
			''' 
			''' <p>Acquires the write lock if neither the read nor write lock
			''' are held by another thread
			''' and returns immediately, setting the write lock hold count to
			''' one.
			''' 
			''' <p>If the current thread already holds the write lock then the
			''' hold count is incremented by one and the method returns
			''' immediately.
			''' 
			''' <p>If the lock is held by another thread then the current
			''' thread becomes disabled for thread scheduling purposes and
			''' lies dormant until the write lock has been acquired, at which
			''' time the write lock hold count is set to one.
			''' </summary>
			Public Overridable Sub lock() Implements Lock.lock
				sync_Renamed.acquire(1)
			End Sub

			''' <summary>
			''' Acquires the write lock unless the current thread is
			''' <seealso cref="Thread#interrupt interrupted"/>.
			''' 
			''' <p>Acquires the write lock if neither the read nor write lock
			''' are held by another thread
			''' and returns immediately, setting the write lock hold count to
			''' one.
			''' 
			''' <p>If the current thread already holds this lock then the
			''' hold count is incremented by one and the method returns
			''' immediately.
			''' 
			''' <p>If the lock is held by another thread then the current
			''' thread becomes disabled for thread scheduling purposes and
			''' lies dormant until one of two things happens:
			''' 
			''' <ul>
			''' 
			''' <li>The write lock is acquired by the current thread; or
			''' 
			''' <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
			''' the current thread.
			''' 
			''' </ul>
			''' 
			''' <p>If the write lock is acquired by the current thread then the
			''' lock hold count is set to one.
			''' 
			''' <p>If the current thread:
			''' 
			''' <ul>
			''' 
			''' <li>has its interrupted status set on entry to this method;
			''' or
			''' 
			''' <li>is <seealso cref="Thread#interrupt interrupted"/> while
			''' acquiring the write lock,
			''' 
			''' </ul>
			''' 
			''' then <seealso cref="InterruptedException"/> is thrown and the current
			''' thread's interrupted status is cleared.
			''' 
			''' <p>In this implementation, as this method is an explicit
			''' interruption point, preference is given to responding to
			''' the interrupt over normal or reentrant acquisition of the
			''' lock.
			''' </summary>
			''' <exception cref="InterruptedException"> if the current thread is interrupted </exception>
			Public Overridable Sub lockInterruptibly() Implements Lock.lockInterruptibly
				sync_Renamed.acquireInterruptibly(1)
			End Sub

			''' <summary>
			''' Acquires the write lock only if it is not held by another thread
			''' at the time of invocation.
			''' 
			''' <p>Acquires the write lock if neither the read nor write lock
			''' are held by another thread
			''' and returns immediately with the value {@code true},
			''' setting the write lock hold count to one. Even when this lock has
			''' been set to use a fair ordering policy, a call to
			''' {@code tryLock()} <em>will</em> immediately acquire the
			''' lock if it is available, whether or not other threads are
			''' currently waiting for the write lock.  This &quot;barging&quot;
			''' behavior can be useful in certain circumstances, even
			''' though it breaks fairness. If you want to honor the
			''' fairness setting for this lock, then use {@link
			''' #tryLock(long, TimeUnit) tryLock(0, TimeUnit.SECONDS) }
			''' which is almost equivalent (it also detects interruption).
			''' 
			''' <p>If the current thread already holds this lock then the
			''' hold count is incremented by one and the method returns
			''' {@code true}.
			''' 
			''' <p>If the lock is held by another thread then this method
			''' will return immediately with the value {@code false}.
			''' </summary>
			''' <returns> {@code true} if the lock was free and was acquired
			''' by the current thread, or the write lock was already held
			''' by the current thread; and {@code false} otherwise. </returns>
			Public Overridable Function tryLock() As Boolean Implements Lock.tryLock
				Return sync_Renamed.tryWriteLock()
			End Function

			''' <summary>
			''' Acquires the write lock if it is not held by another thread
			''' within the given waiting time and the current thread has
			''' not been <seealso cref="Thread#interrupt interrupted"/>.
			''' 
			''' <p>Acquires the write lock if neither the read nor write lock
			''' are held by another thread
			''' and returns immediately with the value {@code true},
			''' setting the write lock hold count to one. If this lock has been
			''' set to use a fair ordering policy then an available lock
			''' <em>will not</em> be acquired if any other threads are
			''' waiting for the write lock. This is in contrast to the {@link
			''' #tryLock()} method. If you want a timed {@code tryLock}
			''' that does permit barging on a fair lock then combine the
			''' timed and un-timed forms together:
			''' 
			'''  <pre> {@code
			''' if (lock.tryLock() ||
			'''     lock.tryLock(timeout, unit)) {
			'''   ...
			''' }}</pre>
			''' 
			''' <p>If the current thread already holds this lock then the
			''' hold count is incremented by one and the method returns
			''' {@code true}.
			''' 
			''' <p>If the lock is held by another thread then the current
			''' thread becomes disabled for thread scheduling purposes and
			''' lies dormant until one of three things happens:
			''' 
			''' <ul>
			''' 
			''' <li>The write lock is acquired by the current thread; or
			''' 
			''' <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
			''' the current thread; or
			''' 
			''' <li>The specified waiting time elapses
			''' 
			''' </ul>
			''' 
			''' <p>If the write lock is acquired then the value {@code true} is
			''' returned and the write lock hold count is set to one.
			''' 
			''' <p>If the current thread:
			''' 
			''' <ul>
			''' 
			''' <li>has its interrupted status set on entry to this method;
			''' or
			''' 
			''' <li>is <seealso cref="Thread#interrupt interrupted"/> while
			''' acquiring the write lock,
			''' 
			''' </ul>
			''' 
			''' then <seealso cref="InterruptedException"/> is thrown and the current
			''' thread's interrupted status is cleared.
			''' 
			''' <p>If the specified waiting time elapses then the value
			''' {@code false} is returned.  If the time is less than or
			''' equal to zero, the method will not wait at all.
			''' 
			''' <p>In this implementation, as this method is an explicit
			''' interruption point, preference is given to responding to
			''' the interrupt over normal or reentrant acquisition of the
			''' lock, and over reporting the elapse of the waiting time.
			''' </summary>
			''' <param name="timeout"> the time to wait for the write lock </param>
			''' <param name="unit"> the time unit of the timeout argument
			''' </param>
			''' <returns> {@code true} if the lock was free and was acquired
			''' by the current thread, or the write lock was already held by the
			''' current thread; and {@code false} if the waiting time
			''' elapsed before the lock could be acquired.
			''' </returns>
			''' <exception cref="InterruptedException"> if the current thread is interrupted </exception>
			''' <exception cref="NullPointerException"> if the time unit is null </exception>
			Public Overridable Function tryLock(  timeout As Long,   unit As java.util.concurrent.TimeUnit) As Boolean Implements Lock.tryLock
				Return sync_Renamed.tryAcquireNanos(1, unit.toNanos(timeout))
			End Function

			''' <summary>
			''' Attempts to release this lock.
			''' 
			''' <p>If the current thread is the holder of this lock then
			''' the hold count is decremented. If the hold count is now
			''' zero then the lock is released.  If the current thread is
			''' not the holder of this lock then {@link
			''' IllegalMonitorStateException} is thrown.
			''' </summary>
			''' <exception cref="IllegalMonitorStateException"> if the current thread does not
			''' hold this lock </exception>
			Public Overridable Sub unlock() Implements Lock.unlock
				sync_Renamed.release(1)
			End Sub

			''' <summary>
			''' Returns a <seealso cref="Condition"/> instance for use with this
			''' <seealso cref="Lock"/> instance.
			''' <p>The returned <seealso cref="Condition"/> instance supports the same
			''' usages as do the <seealso cref="Object"/> monitor methods ({@link
			''' Object#wait() wait}, <seealso cref="Object#notify notify"/>, and {@link
			''' Object#notifyAll notifyAll}) when used with the built-in
			''' monitor lock.
			''' 
			''' <ul>
			''' 
			''' <li>If this write lock is not held when any {@link
			''' Condition} method is called then an {@link
			''' IllegalMonitorStateException} is thrown.  (Read locks are
			''' held independently of write locks, so are not checked or
			''' affected. However it is essentially always an error to
			''' invoke a condition waiting method when the current thread
			''' has also acquired read locks, since other threads that
			''' could unblock it will not be able to acquire the write
			''' lock.)
			''' 
			''' <li>When the condition <seealso cref="Condition#await() waiting"/>
			''' methods are called the write lock is released and, before
			''' they return, the write lock is reacquired and the lock hold
			''' count restored to what it was when the method was called.
			''' 
			''' <li>If a thread is <seealso cref="Thread#interrupt interrupted"/> while
			''' waiting then the wait will terminate, an {@link
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
				Return sync_Renamed.newCondition()
			End Function

			''' <summary>
			''' Returns a string identifying this lock, as well as its lock
			''' state.  The state, in brackets includes either the String
			''' {@code "Unlocked"} or the String {@code "Locked by"}
			''' followed by the <seealso cref="Thread#getName name"/> of the owning thread.
			''' </summary>
			''' <returns> a string identifying this lock, as well as its lock state </returns>
			Public Overrides Function ToString() As String
				Dim o As Thread = sync_Renamed.owner
				Return MyBase.ToString() & (If(o Is Nothing, "[Unlocked]", "[Locked by thread " & o.name & "]"))
			End Function

			''' <summary>
			''' Queries if this write lock is held by the current thread.
			''' Identical in effect to {@link
			''' ReentrantReadWriteLock#isWriteLockedByCurrentThread}.
			''' </summary>
			''' <returns> {@code true} if the current thread holds this lock and
			'''         {@code false} otherwise
			''' @since 1.6 </returns>
			Public Overridable Property heldByCurrentThread As Boolean
				Get
					Return sync_Renamed.heldExclusively
				End Get
			End Property

			''' <summary>
			''' Queries the number of holds on this write lock by the current
			''' thread.  A thread has a hold on a lock for each lock action
			''' that is not matched by an unlock action.  Identical in effect
			''' to <seealso cref="ReentrantReadWriteLock#getWriteHoldCount"/>.
			''' </summary>
			''' <returns> the number of holds on this lock by the current thread,
			'''         or zero if this lock is not held by the current thread
			''' @since 1.6 </returns>
			Public Overridable Property holdCount As Integer
				Get
					Return sync_Renamed.writeHoldCount
				End Get
			End Property
		End Class

		' Instrumentation and status

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
		''' Returns the thread that currently owns the write lock, or
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
		''' Queries the number of read locks held for this lock. This
		''' method is designed for use in monitoring system state, not for
		''' synchronization control. </summary>
		''' <returns> the number of read locks held </returns>
		Public Overridable Property readLockCount As Integer
			Get
				Return sync.readLockCount
			End Get
		End Property

		''' <summary>
		''' Queries if the write lock is held by any thread. This method is
		''' designed for use in monitoring system state, not for
		''' synchronization control.
		''' </summary>
		''' <returns> {@code true} if any thread holds the write lock and
		'''         {@code false} otherwise </returns>
		Public Overridable Property writeLocked As Boolean
			Get
				Return sync.writeLocked
			End Get
		End Property

		''' <summary>
		''' Queries if the write lock is held by the current thread.
		''' </summary>
		''' <returns> {@code true} if the current thread holds the write lock and
		'''         {@code false} otherwise </returns>
		Public Overridable Property writeLockedByCurrentThread As Boolean
			Get
				Return sync.heldExclusively
			End Get
		End Property

		''' <summary>
		''' Queries the number of reentrant write holds on this lock by the
		''' current thread.  A writer thread has a hold on a lock for
		''' each lock action that is not matched by an unlock action.
		''' </summary>
		''' <returns> the number of holds on the write lock by the current thread,
		'''         or zero if the write lock is not held by the current thread </returns>
		Public Overridable Property writeHoldCount As Integer
			Get
				Return sync.writeHoldCount
			End Get
		End Property

		''' <summary>
		''' Queries the number of reentrant read holds on this lock by the
		''' current thread.  A reader thread has a hold on a lock for
		''' each lock action that is not matched by an unlock action.
		''' </summary>
		''' <returns> the number of holds on the read lock by the current thread,
		'''         or zero if the read lock is not held by the current thread
		''' @since 1.6 </returns>
		Public Overridable Property readHoldCount As Integer
			Get
				Return sync.readHoldCount
			End Get
		End Property

		''' <summary>
		''' Returns a collection containing threads that may be waiting to
		''' acquire the write lock.  Because the actual set of threads may
		''' change dynamically while constructing this result, the returned
		''' collection is only a best-effort estimate.  The elements of the
		''' returned collection are in no particular order.  This method is
		''' designed to facilitate construction of subclasses that provide
		''' more extensive lock monitoring facilities.
		''' </summary>
		''' <returns> the collection of threads </returns>
		Protected Friend Overridable Property queuedWriterThreads As ICollection(Of Thread)
			Get
				Return sync.exclusiveQueuedThreads
			End Get
		End Property

		''' <summary>
		''' Returns a collection containing threads that may be waiting to
		''' acquire the read lock.  Because the actual set of threads may
		''' change dynamically while constructing this result, the returned
		''' collection is only a best-effort estimate.  The elements of the
		''' returned collection are in no particular order.  This method is
		''' designed to facilitate construction of subclasses that provide
		''' more extensive lock monitoring facilities.
		''' </summary>
		''' <returns> the collection of threads </returns>
		Protected Friend Overridable Property queuedReaderThreads As ICollection(Of Thread)
			Get
				Return sync.sharedQueuedThreads
			End Get
		End Property

		''' <summary>
		''' Queries whether any threads are waiting to acquire the read or
		''' write lock. Note that because cancellations may occur at any
		''' time, a {@code true} return does not guarantee that any other
		''' thread will ever acquire a lock.  This method is designed
		''' primarily for use in monitoring of the system state.
		''' </summary>
		''' <returns> {@code true} if there may be other threads waiting to
		'''         acquire the lock </returns>
		Public Function hasQueuedThreads() As Boolean
			Return sync.hasQueuedThreads()
		End Function

		''' <summary>
		''' Queries whether the given thread is waiting to acquire either
		''' the read or write lock. Note that because cancellations may
		''' occur at any time, a {@code true} return does not guarantee
		''' that this thread will ever acquire a lock.  This method is
		''' designed primarily for use in monitoring of the system state.
		''' </summary>
		''' <param name="thread"> the thread </param>
		''' <returns> {@code true} if the given thread is queued waiting for this lock </returns>
		''' <exception cref="NullPointerException"> if the thread is null </exception>
		Public Function hasQueuedThread(  thread_Renamed As Thread) As Boolean
			Return sync.isQueued(thread_Renamed)
		End Function

		''' <summary>
		''' Returns an estimate of the number of threads waiting to acquire
		''' either the read or write lock.  The value is only an estimate
		''' because the number of threads may change dynamically while this
		''' method traverses internal data structures.  This method is
		''' designed for use in monitoring of the system state, not for
		''' synchronization control.
		''' </summary>
		''' <returns> the estimated number of threads waiting for this lock </returns>
		Public Property queueLength As Integer
			Get
				Return sync.queueLength
			End Get
		End Property

		''' <summary>
		''' Returns a collection containing threads that may be waiting to
		''' acquire either the read or write lock.  Because the actual set
		''' of threads may change dynamically while constructing this
		''' result, the returned collection is only a best-effort estimate.
		''' The elements of the returned collection are in no particular
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
		''' Queries whether any threads are waiting on the given condition
		''' associated with the write lock. Note that because timeouts and
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
		Public Overridable Function hasWaiters(  condition As Condition) As Boolean
			If condition Is Nothing Then Throw New NullPointerException
			If Not(TypeOf condition Is AbstractQueuedSynchronizer.ConditionObject) Then Throw New IllegalArgumentException("not owner")
			Return sync.hasWaiters(CType(condition, AbstractQueuedSynchronizer.ConditionObject))
		End Function

		''' <summary>
		''' Returns an estimate of the number of threads waiting on the
		''' given condition associated with the write lock. Note that because
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
		Public Overridable Function getWaitQueueLength(  condition As Condition) As Integer
			If condition Is Nothing Then Throw New NullPointerException
			If Not(TypeOf condition Is AbstractQueuedSynchronizer.ConditionObject) Then Throw New IllegalArgumentException("not owner")
			Return sync.getWaitQueueLength(CType(condition, AbstractQueuedSynchronizer.ConditionObject))
		End Function

		''' <summary>
		''' Returns a collection containing those threads that may be
		''' waiting on the given condition associated with the write lock.
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
		Protected Friend Overridable Function getWaitingThreads(  condition As Condition) As ICollection(Of Thread)
			If condition Is Nothing Then Throw New NullPointerException
			If Not(TypeOf condition Is AbstractQueuedSynchronizer.ConditionObject) Then Throw New IllegalArgumentException("not owner")
			Return sync.getWaitingThreads(CType(condition, AbstractQueuedSynchronizer.ConditionObject))
		End Function

		''' <summary>
		''' Returns a string identifying this lock, as well as its lock state.
		''' The state, in brackets, includes the String {@code "Write locks ="}
		''' followed by the number of reentrantly held write locks, and the
		''' String {@code "Read locks ="} followed by the number of held
		''' read locks.
		''' </summary>
		''' <returns> a string identifying this lock, as well as its lock state </returns>
		Public Overrides Function ToString() As String
			Dim c As Integer = sync.count
			Dim w As Integer = Sync.exclusiveCount(c)
			Dim r As Integer = Sync.sharedCount(c)

			Return MyBase.ToString() & "[Write locks = " & w & ", Read locks = " & r & "]"
		End Function

		''' <summary>
		''' Returns the thread id for the given thread.  We must access
		''' this directly rather than via method Thread.getId() because
		''' getId() is not final, and has been known to be overridden in
		''' ways that do not preserve unique mappings.
		''' </summary>
		Friend Shared Function getThreadId(  thread_Renamed As Thread) As Long
			Return UNSAFE.getLongVolatile(thread_Renamed, TID_OFFSET)
		End Function

		' Unsafe mechanics
		Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
		Private Shared ReadOnly TID_OFFSET As Long
		Shared Sub New()
			Try
				UNSAFE = sun.misc.Unsafe.unsafe
				Dim tk As  [Class] = GetType(Thread)
				TID_OFFSET = UNSAFE.objectFieldOffset(tk.getDeclaredField("tid"))
			Catch e As Exception
				Throw New [Error](e)
			End Try
		End Sub

	End Class

End Namespace
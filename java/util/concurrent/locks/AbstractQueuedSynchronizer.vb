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
	''' Provides a framework for implementing blocking locks and related
	''' synchronizers (semaphores, events, etc) that rely on
	''' first-in-first-out (FIFO) wait queues.  This class is designed to
	''' be a useful basis for most kinds of synchronizers that rely on a
	''' single atomic {@code int} value to represent state. Subclasses
	''' must define the protected methods that change this state, and which
	''' define what that state means in terms of this object being acquired
	''' or released.  Given these, the other methods in this class carry
	''' out all queuing and blocking mechanics. Subclasses can maintain
	''' other state fields, but only the atomically updated {@code int}
	''' value manipulated using methods <seealso cref="#getState"/>, {@link
	''' #setState} and <seealso cref="#compareAndSetState"/> is tracked with respect
	''' to synchronization.
	''' 
	''' <p>Subclasses should be defined as non-public internal helper
	''' classes that are used to implement the synchronization properties
	''' of their enclosing class.  Class
	''' {@code AbstractQueuedSynchronizer} does not implement any
	''' synchronization interface.  Instead it defines methods such as
	''' <seealso cref="#acquireInterruptibly"/> that can be invoked as
	''' appropriate by concrete locks and related synchronizers to
	''' implement their public methods.
	''' 
	''' <p>This class supports either or both a default <em>exclusive</em>
	''' mode and a <em>shared</em> mode. When acquired in exclusive mode,
	''' attempted acquires by other threads cannot succeed. Shared mode
	''' acquires by multiple threads may (but need not) succeed. This class
	''' does not &quot;understand&quot; these differences except in the
	''' mechanical sense that when a shared mode acquire succeeds, the next
	''' waiting thread (if one exists) must also determine whether it can
	''' acquire as well. Threads waiting in the different modes share the
	''' same FIFO queue. Usually, implementation subclasses support only
	''' one of these modes, but both can come into play for example in a
	''' <seealso cref="ReadWriteLock"/>. Subclasses that support only exclusive or
	''' only shared modes need not define the methods supporting the unused mode.
	''' 
	''' <p>This class defines a nested <seealso cref="ConditionObject"/> class that
	''' can be used as a <seealso cref="Condition"/> implementation by subclasses
	''' supporting exclusive mode for which method {@link
	''' #isHeldExclusively} reports whether synchronization is exclusively
	''' held with respect to the current thread, method <seealso cref="#release"/>
	''' invoked with the current <seealso cref="#getState"/> value fully releases
	''' this object, and <seealso cref="#acquire"/>, given this saved state value,
	''' eventually restores this object to its previous acquired state.  No
	''' {@code AbstractQueuedSynchronizer} method otherwise creates such a
	''' condition, so if this constraint cannot be met, do not use it.  The
	''' behavior of <seealso cref="ConditionObject"/> depends of course on the
	''' semantics of its synchronizer implementation.
	''' 
	''' <p>This class provides inspection, instrumentation, and monitoring
	''' methods for the internal queue, as well as similar methods for
	''' condition objects. These can be exported as desired into classes
	''' using an {@code AbstractQueuedSynchronizer} for their
	''' synchronization mechanics.
	''' 
	''' <p>Serialization of this class stores only the underlying atomic
	''' integer maintaining state, so deserialized objects have empty
	''' thread queues. Typical subclasses requiring serializability will
	''' define a {@code readObject} method that restores this to a known
	''' initial state upon deserialization.
	''' 
	''' <h3>Usage</h3>
	''' 
	''' <p>To use this class as the basis of a synchronizer, redefine the
	''' following methods, as applicable, by inspecting and/or modifying
	''' the synchronization state using <seealso cref="#getState"/>, {@link
	''' #setState} and/or <seealso cref="#compareAndSetState"/>:
	''' 
	''' <ul>
	''' <li> <seealso cref="#tryAcquire"/>
	''' <li> <seealso cref="#tryRelease"/>
	''' <li> <seealso cref="#tryAcquireShared"/>
	''' <li> <seealso cref="#tryReleaseShared"/>
	''' <li> <seealso cref="#isHeldExclusively"/>
	''' </ul>
	''' 
	''' Each of these methods by default throws {@link
	''' UnsupportedOperationException}.  Implementations of these methods
	''' must be internally thread-safe, and should in general be short and
	''' not block. Defining these methods is the <em>only</em> supported
	''' means of using this class. All other methods are declared
	''' {@code final} because they cannot be independently varied.
	''' 
	''' <p>You may also find the inherited methods from {@link
	''' AbstractOwnableSynchronizer} useful to keep track of the thread
	''' owning an exclusive synchronizer.  You are encouraged to use them
	''' -- this enables monitoring and diagnostic tools to assist users in
	''' determining which threads hold locks.
	''' 
	''' <p>Even though this class is based on an internal FIFO queue, it
	''' does not automatically enforce FIFO acquisition policies.  The core
	''' of exclusive synchronization takes the form:
	''' 
	''' <pre>
	''' Acquire:
	'''     while (!tryAcquire(arg)) {
	'''        <em>enqueue thread if it is not already queued</em>;
	'''        <em>possibly block current thread</em>;
	'''     }
	''' 
	''' Release:
	'''     if (tryRelease(arg))
	'''        <em>unblock the first queued thread</em>;
	''' </pre>
	''' 
	''' (Shared mode is similar but may involve cascading signals.)
	''' 
	''' <p id="barging">Because checks in acquire are invoked before
	''' enqueuing, a newly acquiring thread may <em>barge</em> ahead of
	''' others that are blocked and queued.  However, you can, if desired,
	''' define {@code tryAcquire} and/or {@code tryAcquireShared} to
	''' disable barging by internally invoking one or more of the inspection
	''' methods, thereby providing a <em>fair</em> FIFO acquisition order.
	''' In particular, most fair synchronizers can define {@code tryAcquire}
	''' to return {@code false} if <seealso cref="#hasQueuedPredecessors"/> (a method
	''' specifically designed to be used by fair synchronizers) returns
	''' {@code true}.  Other variations are possible.
	''' 
	''' <p>Throughput and scalability are generally highest for the
	''' default barging (also known as <em>greedy</em>,
	''' <em>renouncement</em>, and <em>convoy-avoidance</em>) strategy.
	''' While this is not guaranteed to be fair or starvation-free, earlier
	''' queued threads are allowed to recontend before later queued
	''' threads, and each recontention has an unbiased chance to succeed
	''' against incoming threads.  Also, while acquires do not
	''' &quot;spin&quot; in the usual sense, they may perform multiple
	''' invocations of {@code tryAcquire} interspersed with other
	''' computations before blocking.  This gives most of the benefits of
	''' spins when exclusive synchronization is only briefly held, without
	''' most of the liabilities when it isn't. If so desired, you can
	''' augment this by preceding calls to acquire methods with
	''' "fast-path" checks, possibly prechecking <seealso cref="#hasContended"/>
	''' and/or <seealso cref="#hasQueuedThreads"/> to only do so if the synchronizer
	''' is likely not to be contended.
	''' 
	''' <p>This class provides an efficient and scalable basis for
	''' synchronization in part by specializing its range of use to
	''' synchronizers that can rely on {@code int} state, acquire, and
	''' release parameters, and an internal FIFO wait queue. When this does
	''' not suffice, you can build synchronizers from a lower level using
	''' <seealso cref="java.util.concurrent.atomic atomic"/> classes, your own custom
	''' <seealso cref="java.util.Queue"/> classes, and <seealso cref="LockSupport"/> blocking
	''' support.
	''' 
	''' <h3>Usage Examples</h3>
	''' 
	''' <p>Here is a non-reentrant mutual exclusion lock class that uses
	''' the value zero to represent the unlocked state, and one to
	''' represent the locked state. While a non-reentrant lock
	''' does not strictly require recording of the current owner
	''' thread, this class does so anyway to make usage easier to monitor.
	''' It also supports conditions and exposes
	''' one of the instrumentation methods:
	''' 
	'''  <pre> {@code
	''' class Mutex implements Lock, java.io.Serializable {
	''' 
	'''   // Our internal helper class
	'''   private static class Sync extends AbstractQueuedSynchronizer {
	'''     // Reports whether in locked state
	'''     protected boolean isHeldExclusively() {
	'''       return getState() == 1;
	'''     }
	''' 
	'''     // Acquires the lock if state is zero
	'''     public boolean tryAcquire(int acquires) {
	'''       assert acquires == 1; // Otherwise unused
	'''       if (compareAndSetState(0, 1)) {
	'''         setExclusiveOwnerThread(Thread.currentThread());
	'''         return true;
	'''       }
	'''       return false;
	'''     }
	''' 
	'''     // Releases the lock by setting state to zero
	'''     protected boolean tryRelease(int releases) {
	'''       assert releases == 1; // Otherwise unused
	'''       if (getState() == 0) throw new IllegalMonitorStateException();
	'''       setExclusiveOwnerThread(null);
	'''       setState(0);
	'''       return true;
	'''     }
	''' 
	'''     // Provides a Condition
	'''     Condition newCondition() { return new ConditionObject(); }
	''' 
	'''     // Deserializes properly
	'''     private void readObject(ObjectInputStream s)
	'''         throws IOException, ClassNotFoundException {
	'''       s.defaultReadObject();
	'''       setState(0); // reset to unlocked state
	'''     }
	'''   }
	''' 
	'''   // The sync object does all the hard work. We just forward to it.
	'''   private final Sync sync = new Sync();
	''' 
	'''   public void lock()                { sync.acquire(1); }
	'''   public boolean tryLock()          { return sync.tryAcquire(1); }
	'''   public void unlock()              { sync.release(1); }
	'''   public Condition newCondition()   { return sync.newCondition(); }
	'''   public boolean isLocked()         { return sync.isHeldExclusively(); }
	'''   public boolean hasQueuedThreads() { return sync.hasQueuedThreads(); }
	'''   public void lockInterruptibly() throws InterruptedException {
	'''     sync.acquireInterruptibly(1);
	'''   }
	'''   public boolean tryLock(long timeout, TimeUnit unit)
	'''       throws InterruptedException {
	'''     return sync.tryAcquireNanos(1, unit.toNanos(timeout));
	'''   }
	''' }}</pre>
	''' 
	''' <p>Here is a latch class that is like a
	''' <seealso cref="java.util.concurrent.CountDownLatch CountDownLatch"/>
	''' except that it only requires a single {@code signal} to
	''' fire. Because a latch is non-exclusive, it uses the {@code shared}
	''' acquire and release methods.
	''' 
	'''  <pre> {@code
	''' class BooleanLatch {
	''' 
	'''   private static class Sync extends AbstractQueuedSynchronizer {
	'''     boolean isSignalled() { return getState() != 0; }
	''' 
	'''     protected int tryAcquireShared(int ignore) {
	'''       return isSignalled() ? 1 : -1;
	'''     }
	''' 
	'''     protected boolean tryReleaseShared(int ignore) {
	'''       setState(1);
	'''       return true;
	'''     }
	'''   }
	''' 
	'''   private final Sync sync = new Sync();
	'''   public boolean isSignalled() { return sync.isSignalled(); }
	'''   public void signal()         { sync.releaseShared(1); }
	'''   public void await() throws InterruptedException {
	'''     sync.acquireSharedInterruptibly(1);
	'''   }
	''' }}</pre>
	''' 
	''' @since 1.5
	''' @author Doug Lea
	''' </summary>
	<Serializable> _
	Public MustInherit Class AbstractQueuedSynchronizer
		Inherits AbstractOwnableSynchronizer

		Private Const serialVersionUID As Long = 7373984972572414691L

		''' <summary>
		''' Creates a new {@code AbstractQueuedSynchronizer} instance
		''' with initial synchronization state of zero.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Wait queue node class.
		''' 
		''' <p>The wait queue is a variant of a "CLH" (Craig, Landin, and
		''' Hagersten) lock queue. CLH locks are normally used for
		''' spinlocks.  We instead use them for blocking synchronizers, but
		''' use the same basic tactic of holding some of the control
		''' information about a thread in the predecessor of its node.  A
		''' "status" field in each node keeps track of whether a thread
		''' should block.  A node is signalled when its predecessor
		''' releases.  Each node of the queue otherwise serves as a
		''' specific-notification-style monitor holding a single waiting
		''' thread. The status field does NOT control whether threads are
		''' granted locks etc though.  A thread may try to acquire if it is
		''' first in the queue. But being first does not guarantee success;
		''' it only gives the right to contend.  So the currently released
		''' contender thread may need to rewait.
		''' 
		''' <p>To enqueue into a CLH lock, you atomically splice it in as new
		''' tail. To dequeue, you just set the head field.
		''' <pre>
		'''      +------+  prev +-----+       +-----+
		''' head |      | <---- |     | <---- |     |  tail
		'''      +------+       +-----+       +-----+
		''' </pre>
		''' 
		''' <p>Insertion into a CLH queue requires only a single atomic
		''' operation on "tail", so there is a simple atomic point of
		''' demarcation from unqueued to queued. Similarly, dequeuing
		''' involves only updating the "head". However, it takes a bit
		''' more work for nodes to determine who their successors are,
		''' in part to deal with possible cancellation due to timeouts
		''' and interrupts.
		''' 
		''' <p>The "prev" links (not used in original CLH locks), are mainly
		''' needed to handle cancellation. If a node is cancelled, its
		''' successor is (normally) relinked to a non-cancelled
		''' predecessor. For explanation of similar mechanics in the case
		''' of spin locks, see the papers by Scott and Scherer at
		''' http://www.cs.rochester.edu/u/scott/synchronization/
		''' 
		''' <p>We also use "next" links to implement blocking mechanics.
		''' The thread id for each node is kept in its own node, so a
		''' predecessor signals the next node to wake up by traversing
		''' next link to determine which thread it is.  Determination of
		''' successor must avoid races with newly queued nodes to set
		''' the "next" fields of their predecessors.  This is solved
		''' when necessary by checking backwards from the atomically
		''' updated "tail" when a node's successor appears to be null.
		''' (Or, said differently, the next-links are an optimization
		''' so that we don't usually need a backward scan.)
		''' 
		''' <p>Cancellation introduces some conservatism to the basic
		''' algorithms.  Since we must poll for cancellation of other
		''' nodes, we can miss noticing whether a cancelled node is
		''' ahead or behind us. This is dealt with by always unparking
		''' successors upon cancellation, allowing them to stabilize on
		''' a new predecessor, unless we can identify an uncancelled
		''' predecessor who will carry this responsibility.
		''' 
		''' <p>CLH queues need a dummy header node to get started. But
		''' we don't create them on construction, because it would be wasted
		''' effort if there is never contention. Instead, the node
		''' is constructed and head and tail pointers are set upon first
		''' contention.
		''' 
		''' <p>Threads waiting on Conditions use the same nodes, but
		''' use an additional link. Conditions only need to link nodes
		''' in simple (non-concurrent) linked queues because they are
		''' only accessed when exclusively held.  Upon await, a node is
		''' inserted into a condition queue.  Upon signal, the node is
		''' transferred to the main queue.  A special value of status
		''' field is used to mark which queue a node is on.
		''' 
		''' <p>Thanks go to Dave Dice, Mark Moir, Victor Luchangco, Bill
		''' Scherer and Michael Scott, along with members of JSR-166
		''' expert group, for helpful ideas, discussions, and critiques
		''' on the design of this class.
		''' </summary>
		Friend NotInheritable Class Node
			''' <summary>
			''' Marker to indicate a node is waiting in shared mode </summary>
			Friend Shared ReadOnly [SHARED] As New Node
			''' <summary>
			''' Marker to indicate a node is waiting in exclusive mode </summary>
			Friend Const EXCLUSIVE As Node = Nothing

			''' <summary>
			''' waitStatus value to indicate thread has cancelled </summary>
			Friend Const CANCELLED As Integer = 1
			''' <summary>
			''' waitStatus value to indicate successor's thread needs unparking </summary>
			Friend Const SIGNAL As Integer = -1
			''' <summary>
			''' waitStatus value to indicate thread is waiting on condition </summary>
			Friend Const CONDITION As Integer = -2
			''' <summary>
			''' waitStatus value to indicate the next acquireShared should
			''' unconditionally propagate
			''' </summary>
			Friend Const PROPAGATE As Integer = -3

			''' <summary>
			''' Status field, taking on only the values:
			'''   SIGNAL:     The successor of this node is (or will soon be)
			'''               blocked (via park), so the current node must
			'''               unpark its successor when it releases or
			'''               cancels. To avoid races, acquire methods must
			'''               first indicate they need a signal,
			'''               then retry the atomic acquire, and then,
			'''               on failure, block.
			'''   CANCELLED:  This node is cancelled due to timeout or interrupt.
			'''               Nodes never leave this state. In particular,
			'''               a thread with cancelled node never again blocks.
			'''   CONDITION:  This node is currently on a condition queue.
			'''               It will not be used as a sync queue node
			'''               until transferred, at which time the status
			'''               will be set to 0. (Use of this value here has
			'''               nothing to do with the other uses of the
			'''               field, but simplifies mechanics.)
			'''   PROPAGATE:  A releaseShared should be propagated to other
			'''               nodes. This is set (for head node only) in
			'''               doReleaseShared to ensure propagation
			'''               continues, even if other operations have
			'''               since intervened.
			'''   0:          None of the above
			''' 
			''' The values are arranged numerically to simplify use.
			''' Non-negative values mean that a node doesn't need to
			''' signal. So, most code doesn't need to check for particular
			''' values, just for sign.
			''' 
			''' The field is initialized to 0 for normal sync nodes, and
			''' CONDITION for condition nodes.  It is modified using CAS
			''' (or when possible, unconditional volatile writes).
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend waitStatus As Integer

			''' <summary>
			''' Link to predecessor node that current node/thread relies on
			''' for checking waitStatus. Assigned during enqueuing, and nulled
			''' out (for sake of GC) only upon dequeuing.  Also, upon
			''' cancellation of a predecessor, we short-circuit while
			''' finding a non-cancelled one, which will always exist
			''' because the head node is never cancelled: A node becomes
			''' head only as a result of successful acquire. A
			''' cancelled thread never succeeds in acquiring, and a thread only
			''' cancels itself, not any other node.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend prev As Node

			''' <summary>
			''' Link to the successor node that the current node/thread
			''' unparks upon release. Assigned during enqueuing, adjusted
			''' when bypassing cancelled predecessors, and nulled out (for
			''' sake of GC) when dequeued.  The enq operation does not
			''' assign next field of a predecessor until after attachment,
			''' so seeing a null next field does not necessarily mean that
			''' node is at end of queue. However, if a next field appears
			''' to be null, we can scan prev's from the tail to
			''' double-check.  The next field of cancelled nodes is set to
			''' point to the node itself instead of null, to make life
			''' easier for isOnSyncQueue.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend [next] As Node

			''' <summary>
			''' The thread that enqueued this node.  Initialized on
			''' construction and nulled out after use.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend thread_Renamed As Thread

			''' <summary>
			''' Link to next node waiting on condition, or the special
			''' value SHARED.  Because condition queues are accessed only
			''' when holding in exclusive mode, we just need a simple
			''' linked queue to hold nodes while they are waiting on
			''' conditions. They are then transferred to the queue to
			''' re-acquire. And because conditions can only be exclusive,
			''' we save a field by using special value to indicate shared
			''' mode.
			''' </summary>
			Friend nextWaiter As Node

			''' <summary>
			''' Returns true if node is waiting in shared mode.
			''' </summary>
			Friend Property [shared] As Boolean
				Get
					Return nextWaiter Is [SHARED]
				End Get
			End Property

			''' <summary>
			''' Returns previous node, or throws NullPointerException if null.
			''' Use when predecessor cannot be null.  The null check could
			''' be elided, but is present to help the VM.
			''' </summary>
			''' <returns> the predecessor of this node </returns>
			Friend Function predecessor() As Node
				Dim p As Node = prev
				If p Is Nothing Then
					Throw New NullPointerException
				Else
					Return p
				End If
			End Function

			Friend Sub New() ' Used to establish initial head or SHARED marker
			End Sub

			Friend Sub New(ByVal thread_Renamed As Thread, ByVal mode As Node) ' Used by addWaiter
				Me.nextWaiter = mode
				Me.thread_Renamed = thread_Renamed
			End Sub

			Friend Sub New(ByVal thread_Renamed As Thread, ByVal waitStatus As Integer) ' Used by Condition
				Me.waitStatus = waitStatus
				Me.thread_Renamed = thread_Renamed
			End Sub
		End Class

		''' <summary>
		''' Head of the wait queue, lazily initialized.  Except for
		''' initialization, it is modified only via method setHead.  Note:
		''' If head exists, its waitStatus is guaranteed not to be
		''' CANCELLED.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private head As Node

		''' <summary>
		''' Tail of the wait queue, lazily initialized.  Modified only via
		''' method enq to add new wait node.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private tail As Node

		''' <summary>
		''' The synchronization state.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private state As Integer

		''' <summary>
		''' Returns the current value of synchronization state.
		''' This operation has memory semantics of a {@code volatile} read. </summary>
		''' <returns> current state value </returns>
		Protected Friend Property state As Integer
			Get
				Return state
			End Get
			Set(ByVal newState As Integer)
				state = newState
			End Set
		End Property


		''' <summary>
		''' Atomically sets synchronization state to the given updated
		''' value if the current state value equals the expected value.
		''' This operation has memory semantics of a {@code volatile} read
		''' and write.
		''' </summary>
		''' <param name="expect"> the expected value </param>
		''' <param name="update"> the new value </param>
		''' <returns> {@code true} if successful. False return indicates that the actual
		'''         value was not equal to the expected value. </returns>
		Protected Friend Function compareAndSetState(ByVal expect As Integer, ByVal update As Integer) As Boolean
			' See below for intrinsics setup to support this
			Return unsafe.compareAndSwapInt(Me, stateOffset, expect, update)
		End Function

		' Queuing utilities

		''' <summary>
		''' The number of nanoseconds for which it is faster to spin
		''' rather than to use timed park. A rough estimate suffices
		''' to improve responsiveness with very short timeouts.
		''' </summary>
		Friend Const spinForTimeoutThreshold As Long = 1000L

		''' <summary>
		''' Inserts node into queue, initializing if necessary. See picture above. </summary>
		''' <param name="node"> the node to insert </param>
		''' <returns> node's predecessor </returns>
		Private Function enq(ByVal node_Renamed As Node) As Node
			Do
				Dim t As Node = tail
				If t Is Nothing Then ' Must initialize
					If compareAndSetHead(New Node) Then tail = head
				Else
					node_Renamed.prev = t
					If compareAndSetTail(t, node_Renamed) Then
						t.next = node_Renamed
						Return t
					End If
				End If
			Loop
		End Function

		''' <summary>
		''' Creates and enqueues node for current thread and given mode.
		''' </summary>
		''' <param name="mode"> Node.EXCLUSIVE for exclusive, Node.SHARED for shared </param>
		''' <returns> the new node </returns>
		Private Function addWaiter(ByVal mode As Node) As Node
			Dim node_Renamed As New Node(Thread.CurrentThread, mode)
			' Try the fast path of enq; backup to full enq on failure
			Dim pred As Node = tail
			If pred IsNot Nothing Then
				node_Renamed.prev = pred
				If compareAndSetTail(pred, node_Renamed) Then
					pred.next = node_Renamed
					Return node_Renamed
				End If
			End If
			enq(node_Renamed)
			Return node_Renamed
		End Function

		''' <summary>
		''' Sets head of queue to be node, thus dequeuing. Called only by
		''' acquire methods.  Also nulls out unused fields for sake of GC
		''' and to suppress unnecessary signals and traversals.
		''' </summary>
		''' <param name="node"> the node </param>
		Private Property head As Node
			Set(ByVal node_Renamed As Node)
				head = node_Renamed
				node_Renamed.thread_Renamed = Nothing
				node_Renamed.prev = Nothing
			End Set
		End Property

		''' <summary>
		''' Wakes up node's successor, if one exists.
		''' </summary>
		''' <param name="node"> the node </param>
		Private Sub unparkSuccessor(ByVal node_Renamed As Node)
	'        
	'         * If status is negative (i.e., possibly needing signal) try
	'         * to clear in anticipation of signalling.  It is OK if this
	'         * fails or if status is changed by waiting thread.
	'         
			Dim ws As Integer = node_Renamed.waitStatus
			If ws < 0 Then compareAndSetWaitStatus(node_Renamed, ws, 0)

	'        
	'         * Thread to unpark is held in successor, which is normally
	'         * just the next node.  But if cancelled or apparently null,
	'         * traverse backwards from tail to find the actual
	'         * non-cancelled successor.
	'         
			Dim s As Node = node_Renamed.next
			If s Is Nothing OrElse s.waitStatus > 0 Then
				s = Nothing
				Dim t As Node = tail
				Do While t IsNot Nothing AndAlso t IsNot node_Renamed
					If t.waitStatus <= 0 Then s = t
					t = t.prev
				Loop
			End If
			If s IsNot Nothing Then LockSupport.unpark(s.thread_Renamed)
		End Sub

		''' <summary>
		''' Release action for shared mode -- signals successor and ensures
		''' propagation. (Note: For exclusive mode, release just amounts
		''' to calling unparkSuccessor of head if it needs signal.)
		''' </summary>
		Private Sub doReleaseShared()
	'        
	'         * Ensure that a release propagates, even if there are other
	'         * in-progress acquires/releases.  This proceeds in the usual
	'         * way of trying to unparkSuccessor of head if it needs
	'         * signal. But if it does not, status is set to PROPAGATE to
	'         * ensure that upon release, propagation continues.
	'         * Additionally, we must loop in case a new node is added
	'         * while we are doing this. Also, unlike other uses of
	'         * unparkSuccessor, we need to know if CAS to reset status
	'         * fails, if so rechecking.
	'         
			Do
				Dim h As Node = head
				If h IsNot Nothing AndAlso h IsNot tail Then
					Dim ws As Integer = h.waitStatus
					If ws = Node.SIGNAL Then
						If Not compareAndSetWaitStatus(h, Node.SIGNAL, 0) Then Continue Do ' loop to recheck cases
						unparkSuccessor(h)
					ElseIf ws = 0 AndAlso (Not compareAndSetWaitStatus(h, 0, Node.PROPAGATE)) Then
						Continue Do ' loop on failed CAS
					End If
				End If
				If h Is head Then ' loop if head changed Exit Do
			Loop
		End Sub

		''' <summary>
		''' Sets head of queue, and checks if successor may be waiting
		''' in shared mode, if so propagating if either propagate > 0 or
		''' PROPAGATE status was set.
		''' </summary>
		''' <param name="node"> the node </param>
		''' <param name="propagate"> the return value from a tryAcquireShared </param>
		Private Sub setHeadAndPropagate(ByVal node_Renamed As Node, ByVal propagate As Integer)
			Dim h As Node = head ' Record old head for check below
			head = node_Renamed
	'        
	'         * Try to signal next queued node if:
	'         *   Propagation was indicated by caller,
	'         *     or was recorded (as h.waitStatus either before
	'         *     or after setHead) by a previous operation
	'         *     (note: this uses sign-check of waitStatus because
	'         *      PROPAGATE status may transition to SIGNAL.)
	'         * and
	'         *   The next node is waiting in shared mode,
	'         *     or we don't know, because it appears null
	'         *
	'         * The conservatism in both of these checks may cause
	'         * unnecessary wake-ups, but only when there are multiple
	'         * racing acquires/releases, so most need signals now or soon
	'         * anyway.
	'         
			h = head
			If propagate > 0 OrElse h Is Nothing OrElse h.waitStatus < 0 OrElse h Is Nothing OrElse h.waitStatus < 0 Then
				Dim s As Node = node_Renamed.next
				If s Is Nothing OrElse s.shared Then doReleaseShared()
			End If
		End Sub

		' Utilities for various versions of acquire

		''' <summary>
		''' Cancels an ongoing attempt to acquire.
		''' </summary>
		''' <param name="node"> the node </param>
		Private Sub cancelAcquire(ByVal node_Renamed As Node)
			' Ignore if node doesn't exist
			If node_Renamed Is Nothing Then Return

			node_Renamed.thread_Renamed = Nothing

			' Skip cancelled predecessors
			Dim pred As Node = node_Renamed.prev
			Do While pred.waitStatus > 0
					pred = pred.prev
					node_Renamed.prev = pred
			Loop

			' predNext is the apparent node to unsplice. CASes below will
			' fail if not, in which case, we lost race vs another cancel
			' or signal, so no further action is necessary.
			Dim predNext As Node = pred.next

			' Can use unconditional write instead of CAS here.
			' After this atomic step, other Nodes can skip past us.
			' Before, we are free of interference from other threads.
			node_Renamed.waitStatus = Node.CANCELLED

			' If we are the tail, remove ourselves.
			If node_Renamed Is tail AndAlso compareAndSetTail(node_Renamed, pred) Then
				compareAndSetNext(pred, predNext, Nothing)
			Else
				' If successor needs signal, try to set pred's next-link
				' so it will get one. Otherwise wake it up to propagate.
				Dim ws As Integer
				ws = pred.waitStatus
				If pred IsNot head AndAlso (ws = Node.SIGNAL OrElse (ws <= 0 AndAlso compareAndSetWaitStatus(pred, ws, Node.SIGNAL))) AndAlso pred.thread_Renamed IsNot Nothing Then
					Dim [next] As Node = node_Renamed.next
					If [next] IsNot Nothing AndAlso [next].waitStatus <= 0 Then compareAndSetNext(pred, predNext, [next])
				Else
					unparkSuccessor(node_Renamed)
				End If

				node_Renamed.next = node_Renamed ' help GC
			End If
		End Sub

		''' <summary>
		''' Checks and updates status for a node that failed to acquire.
		''' Returns true if thread should block. This is the main signal
		''' control in all acquire loops.  Requires that pred == node.prev.
		''' </summary>
		''' <param name="pred"> node's predecessor holding status </param>
		''' <param name="node"> the node </param>
		''' <returns> {@code true} if thread should block </returns>
		Private Shared Function shouldParkAfterFailedAcquire(ByVal pred As Node, ByVal node_Renamed As Node) As Boolean
			Dim ws As Integer = pred.waitStatus
			If ws = Node.SIGNAL Then Return True
			If ws > 0 Then
	'            
	'             * Predecessor was cancelled. Skip over predecessors and
	'             * indicate retry.
	'             
				Do
						pred = pred.prev
						node_Renamed.prev = pred
				Loop While pred.waitStatus > 0
				pred.next = node_Renamed
			Else
	'            
	'             * waitStatus must be 0 or PROPAGATE.  Indicate that we
	'             * need a signal, but don't park yet.  Caller will need to
	'             * retry to make sure it cannot acquire before parking.
	'             
				compareAndSetWaitStatus(pred, ws, Node.SIGNAL)
			End If
			Return False
		End Function

		''' <summary>
		''' Convenience method to interrupt current thread.
		''' </summary>
		Friend Shared Sub selfInterrupt()
			Thread.CurrentThread.Interrupt()
		End Sub

		''' <summary>
		''' Convenience method to park and then check if interrupted
		''' </summary>
		''' <returns> {@code true} if interrupted </returns>
		Private Function parkAndCheckInterrupt() As Boolean
			LockSupport.park(Me)
			Return Thread.interrupted()
		End Function

	'    
	'     * Various flavors of acquire, varying in exclusive/shared and
	'     * control modes.  Each is mostly the same, but annoyingly
	'     * different.  Only a little bit of factoring is possible due to
	'     * interactions of exception mechanics (including ensuring that we
	'     * cancel if tryAcquire throws exception) and other control, at
	'     * least not without hurting performance too much.
	'     

		''' <summary>
		''' Acquires in exclusive uninterruptible mode for thread already in
		''' queue. Used by condition wait methods as well as acquire.
		''' </summary>
		''' <param name="node"> the node </param>
		''' <param name="arg"> the acquire argument </param>
		''' <returns> {@code true} if interrupted while waiting </returns>
		Friend Function acquireQueued(ByVal node_Renamed As Node, ByVal arg As Integer) As Boolean
			Dim failed As Boolean = True
			Try
				Dim interrupted As Boolean = False
				Do
					Dim p As Node = node_Renamed.predecessor()
					If p Is head AndAlso tryAcquire(arg) Then
						head = node_Renamed
						p.next = Nothing ' help GC
						failed = False
						Return interrupted
					End If
					If shouldParkAfterFailedAcquire(p, node_Renamed) AndAlso parkAndCheckInterrupt() Then interrupted = True
				Loop
			Finally
				If failed Then cancelAcquire(node_Renamed)
			End Try
		End Function

		''' <summary>
		''' Acquires in exclusive interruptible mode. </summary>
		''' <param name="arg"> the acquire argument </param>
		Private Sub doAcquireInterruptibly(ByVal arg As Integer)
			Dim node_Renamed As Node = addWaiter(Node.EXCLUSIVE)
			Dim failed As Boolean = True
			Try
				Do
					Dim p As Node = node_Renamed.predecessor()
					If p Is head AndAlso tryAcquire(arg) Then
						head = node_Renamed
						p.next = Nothing ' help GC
						failed = False
						Return
					End If
					If shouldParkAfterFailedAcquire(p, node_Renamed) AndAlso parkAndCheckInterrupt() Then Throw New InterruptedException
				Loop
			Finally
				If failed Then cancelAcquire(node_Renamed)
			End Try
		End Sub

		''' <summary>
		''' Acquires in exclusive timed mode.
		''' </summary>
		''' <param name="arg"> the acquire argument </param>
		''' <param name="nanosTimeout"> max wait time </param>
		''' <returns> {@code true} if acquired </returns>
		Private Function doAcquireNanos(ByVal arg As Integer, ByVal nanosTimeout As Long) As Boolean
			If nanosTimeout <= 0L Then Return False
			Dim deadline As Long = System.nanoTime() + nanosTimeout
			Dim node_Renamed As Node = addWaiter(Node.EXCLUSIVE)
			Dim failed As Boolean = True
			Try
				Do
					Dim p As Node = node_Renamed.predecessor()
					If p Is head AndAlso tryAcquire(arg) Then
						head = node_Renamed
						p.next = Nothing ' help GC
						failed = False
						Return True
					End If
					nanosTimeout = deadline - System.nanoTime()
					If nanosTimeout <= 0L Then Return False
					If shouldParkAfterFailedAcquire(p, node_Renamed) AndAlso nanosTimeout > spinForTimeoutThreshold Then LockSupport.parkNanos(Me, nanosTimeout)
					If Thread.interrupted() Then Throw New InterruptedException
				Loop
			Finally
				If failed Then cancelAcquire(node_Renamed)
			End Try
		End Function

		''' <summary>
		''' Acquires in shared uninterruptible mode. </summary>
		''' <param name="arg"> the acquire argument </param>
		Private Sub doAcquireShared(ByVal arg As Integer)
			Dim node_Renamed As Node = addWaiter(Node.SHARED)
			Dim failed As Boolean = True
			Try
				Dim interrupted As Boolean = False
				Do
					Dim p As Node = node_Renamed.predecessor()
					If p Is head Then
						Dim r As Integer = tryAcquireShared(arg)
						If r >= 0 Then
							headAndPropagateate(node_Renamed, r)
							p.next = Nothing ' help GC
							If interrupted Then selfInterrupt()
							failed = False
							Return
						End If
					End If
					If shouldParkAfterFailedAcquire(p, node_Renamed) AndAlso parkAndCheckInterrupt() Then interrupted = True
				Loop
			Finally
				If failed Then cancelAcquire(node_Renamed)
			End Try
		End Sub

		''' <summary>
		''' Acquires in shared interruptible mode. </summary>
		''' <param name="arg"> the acquire argument </param>
		Private Sub doAcquireSharedInterruptibly(ByVal arg As Integer)
			Dim node_Renamed As Node = addWaiter(Node.SHARED)
			Dim failed As Boolean = True
			Try
				Do
					Dim p As Node = node_Renamed.predecessor()
					If p Is head Then
						Dim r As Integer = tryAcquireShared(arg)
						If r >= 0 Then
							headAndPropagateate(node_Renamed, r)
							p.next = Nothing ' help GC
							failed = False
							Return
						End If
					End If
					If shouldParkAfterFailedAcquire(p, node_Renamed) AndAlso parkAndCheckInterrupt() Then Throw New InterruptedException
				Loop
			Finally
				If failed Then cancelAcquire(node_Renamed)
			End Try
		End Sub

		''' <summary>
		''' Acquires in shared timed mode.
		''' </summary>
		''' <param name="arg"> the acquire argument </param>
		''' <param name="nanosTimeout"> max wait time </param>
		''' <returns> {@code true} if acquired </returns>
		Private Function doAcquireSharedNanos(ByVal arg As Integer, ByVal nanosTimeout As Long) As Boolean
			If nanosTimeout <= 0L Then Return False
			Dim deadline As Long = System.nanoTime() + nanosTimeout
			Dim node_Renamed As Node = addWaiter(Node.SHARED)
			Dim failed As Boolean = True
			Try
				Do
					Dim p As Node = node_Renamed.predecessor()
					If p Is head Then
						Dim r As Integer = tryAcquireShared(arg)
						If r >= 0 Then
							headAndPropagateate(node_Renamed, r)
							p.next = Nothing ' help GC
							failed = False
							Return True
						End If
					End If
					nanosTimeout = deadline - System.nanoTime()
					If nanosTimeout <= 0L Then Return False
					If shouldParkAfterFailedAcquire(p, node_Renamed) AndAlso nanosTimeout > spinForTimeoutThreshold Then LockSupport.parkNanos(Me, nanosTimeout)
					If Thread.interrupted() Then Throw New InterruptedException
				Loop
			Finally
				If failed Then cancelAcquire(node_Renamed)
			End Try
		End Function

		' Main exported methods

		''' <summary>
		''' Attempts to acquire in exclusive mode. This method should query
		''' if the state of the object permits it to be acquired in the
		''' exclusive mode, and if so to acquire it.
		''' 
		''' <p>This method is always invoked by the thread performing
		''' acquire.  If this method reports failure, the acquire method
		''' may queue the thread, if it is not already queued, until it is
		''' signalled by a release from some other thread. This can be used
		''' to implement method <seealso cref="Lock#tryLock()"/>.
		''' 
		''' <p>The default
		''' implementation throws <seealso cref="UnsupportedOperationException"/>.
		''' </summary>
		''' <param name="arg"> the acquire argument. This value is always the one
		'''        passed to an acquire method, or is the value saved on entry
		'''        to a condition wait.  The value is otherwise uninterpreted
		'''        and can represent anything you like. </param>
		''' <returns> {@code true} if successful. Upon success, this object has
		'''         been acquired. </returns>
		''' <exception cref="IllegalMonitorStateException"> if acquiring would place this
		'''         synchronizer in an illegal state. This exception must be
		'''         thrown in a consistent fashion for synchronization to work
		'''         correctly. </exception>
		''' <exception cref="UnsupportedOperationException"> if exclusive mode is not supported </exception>
		Protected Friend Overridable Function tryAcquire(ByVal arg As Integer) As Boolean
			Throw New UnsupportedOperationException
		End Function

		''' <summary>
		''' Attempts to set the state to reflect a release in exclusive
		''' mode.
		''' 
		''' <p>This method is always invoked by the thread performing release.
		''' 
		''' <p>The default implementation throws
		''' <seealso cref="UnsupportedOperationException"/>.
		''' </summary>
		''' <param name="arg"> the release argument. This value is always the one
		'''        passed to a release method, or the current state value upon
		'''        entry to a condition wait.  The value is otherwise
		'''        uninterpreted and can represent anything you like. </param>
		''' <returns> {@code true} if this object is now in a fully released
		'''         state, so that any waiting threads may attempt to acquire;
		'''         and {@code false} otherwise. </returns>
		''' <exception cref="IllegalMonitorStateException"> if releasing would place this
		'''         synchronizer in an illegal state. This exception must be
		'''         thrown in a consistent fashion for synchronization to work
		'''         correctly. </exception>
		''' <exception cref="UnsupportedOperationException"> if exclusive mode is not supported </exception>
		Protected Friend Overridable Function tryRelease(ByVal arg As Integer) As Boolean
			Throw New UnsupportedOperationException
		End Function

		''' <summary>
		''' Attempts to acquire in shared mode. This method should query if
		''' the state of the object permits it to be acquired in the shared
		''' mode, and if so to acquire it.
		''' 
		''' <p>This method is always invoked by the thread performing
		''' acquire.  If this method reports failure, the acquire method
		''' may queue the thread, if it is not already queued, until it is
		''' signalled by a release from some other thread.
		''' 
		''' <p>The default implementation throws {@link
		''' UnsupportedOperationException}.
		''' </summary>
		''' <param name="arg"> the acquire argument. This value is always the one
		'''        passed to an acquire method, or is the value saved on entry
		'''        to a condition wait.  The value is otherwise uninterpreted
		'''        and can represent anything you like. </param>
		''' <returns> a negative value on failure; zero if acquisition in shared
		'''         mode succeeded but no subsequent shared-mode acquire can
		'''         succeed; and a positive value if acquisition in shared
		'''         mode succeeded and subsequent shared-mode acquires might
		'''         also succeed, in which case a subsequent waiting thread
		'''         must check availability. (Support for three different
		'''         return values enables this method to be used in contexts
		'''         where acquires only sometimes act exclusively.)  Upon
		'''         success, this object has been acquired. </returns>
		''' <exception cref="IllegalMonitorStateException"> if acquiring would place this
		'''         synchronizer in an illegal state. This exception must be
		'''         thrown in a consistent fashion for synchronization to work
		'''         correctly. </exception>
		''' <exception cref="UnsupportedOperationException"> if shared mode is not supported </exception>
		Protected Friend Overridable Function tryAcquireShared(ByVal arg As Integer) As Integer
			Throw New UnsupportedOperationException
		End Function

		''' <summary>
		''' Attempts to set the state to reflect a release in shared mode.
		''' 
		''' <p>This method is always invoked by the thread performing release.
		''' 
		''' <p>The default implementation throws
		''' <seealso cref="UnsupportedOperationException"/>.
		''' </summary>
		''' <param name="arg"> the release argument. This value is always the one
		'''        passed to a release method, or the current state value upon
		'''        entry to a condition wait.  The value is otherwise
		'''        uninterpreted and can represent anything you like. </param>
		''' <returns> {@code true} if this release of shared mode may permit a
		'''         waiting acquire (shared or exclusive) to succeed; and
		'''         {@code false} otherwise </returns>
		''' <exception cref="IllegalMonitorStateException"> if releasing would place this
		'''         synchronizer in an illegal state. This exception must be
		'''         thrown in a consistent fashion for synchronization to work
		'''         correctly. </exception>
		''' <exception cref="UnsupportedOperationException"> if shared mode is not supported </exception>
		Protected Friend Overridable Function tryReleaseShared(ByVal arg As Integer) As Boolean
			Throw New UnsupportedOperationException
		End Function

		''' <summary>
		''' Returns {@code true} if synchronization is held exclusively with
		''' respect to the current (calling) thread.  This method is invoked
		''' upon each call to a non-waiting <seealso cref="ConditionObject"/> method.
		''' (Waiting methods instead invoke <seealso cref="#release"/>.)
		''' 
		''' <p>The default implementation throws {@link
		''' UnsupportedOperationException}. This method is invoked
		''' internally only within <seealso cref="ConditionObject"/> methods, so need
		''' not be defined if conditions are not used.
		''' </summary>
		''' <returns> {@code true} if synchronization is held exclusively;
		'''         {@code false} otherwise </returns>
		''' <exception cref="UnsupportedOperationException"> if conditions are not supported </exception>
		Protected Friend Overridable Property heldExclusively As Boolean
			Get
				Throw New UnsupportedOperationException
			End Get
		End Property

		''' <summary>
		''' Acquires in exclusive mode, ignoring interrupts.  Implemented
		''' by invoking at least once <seealso cref="#tryAcquire"/>,
		''' returning on success.  Otherwise the thread is queued, possibly
		''' repeatedly blocking and unblocking, invoking {@link
		''' #tryAcquire} until success.  This method can be used
		''' to implement method <seealso cref="Lock#lock"/>.
		''' </summary>
		''' <param name="arg"> the acquire argument.  This value is conveyed to
		'''        <seealso cref="#tryAcquire"/> but is otherwise uninterpreted and
		'''        can represent anything you like. </param>
		Public Sub acquire(ByVal arg As Integer)
			If (Not tryAcquire(arg)) AndAlso acquireQueued(addWaiter(Node.EXCLUSIVE), arg) Then selfInterrupt()
		End Sub

		''' <summary>
		''' Acquires in exclusive mode, aborting if interrupted.
		''' Implemented by first checking interrupt status, then invoking
		''' at least once <seealso cref="#tryAcquire"/>, returning on
		''' success.  Otherwise the thread is queued, possibly repeatedly
		''' blocking and unblocking, invoking <seealso cref="#tryAcquire"/>
		''' until success or the thread is interrupted.  This method can be
		''' used to implement method <seealso cref="Lock#lockInterruptibly"/>.
		''' </summary>
		''' <param name="arg"> the acquire argument.  This value is conveyed to
		'''        <seealso cref="#tryAcquire"/> but is otherwise uninterpreted and
		'''        can represent anything you like. </param>
		''' <exception cref="InterruptedException"> if the current thread is interrupted </exception>
		Public Sub acquireInterruptibly(ByVal arg As Integer)
			If Thread.interrupted() Then Throw New InterruptedException
			If Not tryAcquire(arg) Then doAcquireInterruptibly(arg)
		End Sub

		''' <summary>
		''' Attempts to acquire in exclusive mode, aborting if interrupted,
		''' and failing if the given timeout elapses.  Implemented by first
		''' checking interrupt status, then invoking at least once {@link
		''' #tryAcquire}, returning on success.  Otherwise, the thread is
		''' queued, possibly repeatedly blocking and unblocking, invoking
		''' <seealso cref="#tryAcquire"/> until success or the thread is interrupted
		''' or the timeout elapses.  This method can be used to implement
		''' method <seealso cref="Lock#tryLock(long, TimeUnit)"/>.
		''' </summary>
		''' <param name="arg"> the acquire argument.  This value is conveyed to
		'''        <seealso cref="#tryAcquire"/> but is otherwise uninterpreted and
		'''        can represent anything you like. </param>
		''' <param name="nanosTimeout"> the maximum number of nanoseconds to wait </param>
		''' <returns> {@code true} if acquired; {@code false} if timed out </returns>
		''' <exception cref="InterruptedException"> if the current thread is interrupted </exception>
		Public Function tryAcquireNanos(ByVal arg As Integer, ByVal nanosTimeout As Long) As Boolean
			If Thread.interrupted() Then Throw New InterruptedException
			Return tryAcquire(arg) OrElse doAcquireNanos(arg, nanosTimeout)
		End Function

		''' <summary>
		''' Releases in exclusive mode.  Implemented by unblocking one or
		''' more threads if <seealso cref="#tryRelease"/> returns true.
		''' This method can be used to implement method <seealso cref="Lock#unlock"/>.
		''' </summary>
		''' <param name="arg"> the release argument.  This value is conveyed to
		'''        <seealso cref="#tryRelease"/> but is otherwise uninterpreted and
		'''        can represent anything you like. </param>
		''' <returns> the value returned from <seealso cref="#tryRelease"/> </returns>
		Public Function release(ByVal arg As Integer) As Boolean
			If tryRelease(arg) Then
				Dim h As Node = head
				If h IsNot Nothing AndAlso h.waitStatus <> 0 Then unparkSuccessor(h)
				Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Acquires in shared mode, ignoring interrupts.  Implemented by
		''' first invoking at least once <seealso cref="#tryAcquireShared"/>,
		''' returning on success.  Otherwise the thread is queued, possibly
		''' repeatedly blocking and unblocking, invoking {@link
		''' #tryAcquireShared} until success.
		''' </summary>
		''' <param name="arg"> the acquire argument.  This value is conveyed to
		'''        <seealso cref="#tryAcquireShared"/> but is otherwise uninterpreted
		'''        and can represent anything you like. </param>
		Public Sub acquireShared(ByVal arg As Integer)
			If tryAcquireShared(arg) < 0 Then doAcquireShared(arg)
		End Sub

		''' <summary>
		''' Acquires in shared mode, aborting if interrupted.  Implemented
		''' by first checking interrupt status, then invoking at least once
		''' <seealso cref="#tryAcquireShared"/>, returning on success.  Otherwise the
		''' thread is queued, possibly repeatedly blocking and unblocking,
		''' invoking <seealso cref="#tryAcquireShared"/> until success or the thread
		''' is interrupted. </summary>
		''' <param name="arg"> the acquire argument.
		''' This value is conveyed to <seealso cref="#tryAcquireShared"/> but is
		''' otherwise uninterpreted and can represent anything
		''' you like. </param>
		''' <exception cref="InterruptedException"> if the current thread is interrupted </exception>
		Public Sub acquireSharedInterruptibly(ByVal arg As Integer)
			If Thread.interrupted() Then Throw New InterruptedException
			If tryAcquireShared(arg) < 0 Then doAcquireSharedInterruptibly(arg)
		End Sub

		''' <summary>
		''' Attempts to acquire in shared mode, aborting if interrupted, and
		''' failing if the given timeout elapses.  Implemented by first
		''' checking interrupt status, then invoking at least once {@link
		''' #tryAcquireShared}, returning on success.  Otherwise, the
		''' thread is queued, possibly repeatedly blocking and unblocking,
		''' invoking <seealso cref="#tryAcquireShared"/> until success or the thread
		''' is interrupted or the timeout elapses.
		''' </summary>
		''' <param name="arg"> the acquire argument.  This value is conveyed to
		'''        <seealso cref="#tryAcquireShared"/> but is otherwise uninterpreted
		'''        and can represent anything you like. </param>
		''' <param name="nanosTimeout"> the maximum number of nanoseconds to wait </param>
		''' <returns> {@code true} if acquired; {@code false} if timed out </returns>
		''' <exception cref="InterruptedException"> if the current thread is interrupted </exception>
		Public Function tryAcquireSharedNanos(ByVal arg As Integer, ByVal nanosTimeout As Long) As Boolean
			If Thread.interrupted() Then Throw New InterruptedException
			Return tryAcquireShared(arg) >= 0 OrElse doAcquireSharedNanos(arg, nanosTimeout)
		End Function

		''' <summary>
		''' Releases in shared mode.  Implemented by unblocking one or more
		''' threads if <seealso cref="#tryReleaseShared"/> returns true.
		''' </summary>
		''' <param name="arg"> the release argument.  This value is conveyed to
		'''        <seealso cref="#tryReleaseShared"/> but is otherwise uninterpreted
		'''        and can represent anything you like. </param>
		''' <returns> the value returned from <seealso cref="#tryReleaseShared"/> </returns>
		Public Function releaseShared(ByVal arg As Integer) As Boolean
			If tryReleaseShared(arg) Then
				doReleaseShared()
				Return True
			End If
			Return False
		End Function

		' Queue inspection methods

		''' <summary>
		''' Queries whether any threads are waiting to acquire. Note that
		''' because cancellations due to interrupts and timeouts may occur
		''' at any time, a {@code true} return does not guarantee that any
		''' other thread will ever acquire.
		''' 
		''' <p>In this implementation, this operation returns in
		''' constant time.
		''' </summary>
		''' <returns> {@code true} if there may be other threads waiting to acquire </returns>
		Public Function hasQueuedThreads() As Boolean
			Return head IsNot tail
		End Function

		''' <summary>
		''' Queries whether any threads have ever contended to acquire this
		''' synchronizer; that is if an acquire method has ever blocked.
		''' 
		''' <p>In this implementation, this operation returns in
		''' constant time.
		''' </summary>
		''' <returns> {@code true} if there has ever been contention </returns>
		Public Function hasContended() As Boolean
			Return head IsNot Nothing
		End Function

		''' <summary>
		''' Returns the first (longest-waiting) thread in the queue, or
		''' {@code null} if no threads are currently queued.
		''' 
		''' <p>In this implementation, this operation normally returns in
		''' constant time, but may iterate upon contention if other threads are
		''' concurrently modifying the queue.
		''' </summary>
		''' <returns> the first (longest-waiting) thread in the queue, or
		'''         {@code null} if no threads are currently queued </returns>
		Public Property firstQueuedThread As Thread
			Get
				' handle only fast path, else relay
				Return If(head Is tail, Nothing, fullGetFirstQueuedThread())
			End Get
		End Property

		''' <summary>
		''' Version of getFirstQueuedThread called when fastpath fails
		''' </summary>
		Private Function fullGetFirstQueuedThread() As Thread
	'        
	'         * The first node is normally head.next. Try to get its
	'         * thread field, ensuring consistent reads: If thread
	'         * field is nulled out or s.prev is no longer head, then
	'         * some other thread(s) concurrently performed setHead in
	'         * between some of our reads. We try this twice before
	'         * resorting to traversal.
	'         
			Dim h, s As Node
			Dim st As Thread
			h = head
			s = h.next
			st = s.thread_Renamed
			h = head
			s = h.next
			st = s.thread_Renamed
			If (h IsNot Nothing AndAlso s IsNot Nothing AndAlso s.prev Is head AndAlso st IsNot Nothing) OrElse (h IsNot Nothing AndAlso s IsNot Nothing AndAlso s.prev Is head AndAlso st IsNot Nothing) Then Return st

	'        
	'         * Head's next field might not have been set yet, or may have
	'         * been unset after setHead. So we must check to see if tail
	'         * is actually first node. If not, we continue on, safely
	'         * traversing from tail back to head to find first,
	'         * guaranteeing termination.
	'         

			Dim t As Node = tail
			Dim firstThread As Thread = Nothing
			Do While t IsNot Nothing AndAlso t IsNot head
				Dim tt As Thread = t.thread_Renamed
				If tt IsNot Nothing Then firstThread = tt
				t = t.prev
			Loop
			Return firstThread
		End Function

		''' <summary>
		''' Returns true if the given thread is currently queued.
		''' 
		''' <p>This implementation traverses the queue to determine
		''' presence of the given thread.
		''' </summary>
		''' <param name="thread"> the thread </param>
		''' <returns> {@code true} if the given thread is on the queue </returns>
		''' <exception cref="NullPointerException"> if the thread is null </exception>
		Public Function isQueued(ByVal thread_Renamed As Thread) As Boolean
			If thread_Renamed Is Nothing Then Throw New NullPointerException
			Dim p As Node = tail
			Do While p IsNot Nothing
				If p.thread_Renamed Is thread_Renamed Then Return True
				p = p.prev
			Loop
			Return False
		End Function

		''' <summary>
		''' Returns {@code true} if the apparent first queued thread, if one
		''' exists, is waiting in exclusive mode.  If this method returns
		''' {@code true}, and the current thread is attempting to acquire in
		''' shared mode (that is, this method is invoked from {@link
		''' #tryAcquireShared}) then it is guaranteed that the current thread
		''' is not the first queued thread.  Used only as a heuristic in
		''' ReentrantReadWriteLock.
		''' </summary>
		Friend Function apparentlyFirstQueuedIsExclusive() As Boolean
			Dim h, s As Node
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return (h = head) IsNot Nothing AndAlso (s = h.next) IsNot Nothing AndAlso (Not s.shared) AndAlso s.thread_Renamed IsNot Nothing
		End Function

		''' <summary>
		''' Queries whether any threads have been waiting to acquire longer
		''' than the current thread.
		''' 
		''' <p>An invocation of this method is equivalent to (but may be
		''' more efficient than):
		'''  <pre> {@code
		''' getFirstQueuedThread() != Thread.currentThread() &&
		''' hasQueuedThreads()}</pre>
		''' 
		''' <p>Note that because cancellations due to interrupts and
		''' timeouts may occur at any time, a {@code true} return does not
		''' guarantee that some other thread will acquire before the current
		''' thread.  Likewise, it is possible for another thread to win a
		''' race to enqueue after this method has returned {@code false},
		''' due to the queue being empty.
		''' 
		''' <p>This method is designed to be used by a fair synchronizer to
		''' avoid <a href="AbstractQueuedSynchronizer#barging">barging</a>.
		''' Such a synchronizer's <seealso cref="#tryAcquire"/> method should return
		''' {@code false}, and its <seealso cref="#tryAcquireShared"/> method should
		''' return a negative value, if this method returns {@code true}
		''' (unless this is a reentrant acquire).  For example, the {@code
		''' tryAcquire} method for a fair, reentrant, exclusive mode
		''' synchronizer might look like this:
		''' 
		'''  <pre> {@code
		''' protected boolean tryAcquire(int arg) {
		'''   if (isHeldExclusively()) {
		'''     // A reentrant acquire; increment hold count
		'''     return true;
		'''   } else if (hasQueuedPredecessors()) {
		'''     return false;
		'''   } else {
		'''     // try to acquire normally
		'''   }
		''' }}</pre>
		''' </summary>
		''' <returns> {@code true} if there is a queued thread preceding the
		'''         current thread, and {@code false} if the current thread
		'''         is at the head of the queue or the queue is empty
		''' @since 1.7 </returns>
		Public Function hasQueuedPredecessors() As Boolean
			' The correctness of this depends on head being initialized
			' before tail and on head.next being accurate if the current
			' thread is first in queue.
			Dim t As Node = tail ' Read fields in reverse initialization order
			Dim h As Node = head
			Dim s As Node
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return h IsNot t AndAlso ((s = h.next) Is Nothing OrElse s.thread_Renamed IsNot Thread.CurrentThread)
		End Function


		' Instrumentation and monitoring methods

		''' <summary>
		''' Returns an estimate of the number of threads waiting to
		''' acquire.  The value is only an estimate because the number of
		''' threads may change dynamically while this method traverses
		''' internal data structures.  This method is designed for use in
		''' monitoring system state, not for synchronization
		''' control.
		''' </summary>
		''' <returns> the estimated number of threads waiting to acquire </returns>
		Public Property queueLength As Integer
			Get
				Dim n As Integer = 0
				Dim p As Node = tail
				Do While p IsNot Nothing
					If p.thread_Renamed IsNot Nothing Then n += 1
					p = p.prev
				Loop
				Return n
			End Get
		End Property

		''' <summary>
		''' Returns a collection containing threads that may be waiting to
		''' acquire.  Because the actual set of threads may change
		''' dynamically while constructing this result, the returned
		''' collection is only a best-effort estimate.  The elements of the
		''' returned collection are in no particular order.  This method is
		''' designed to facilitate construction of subclasses that provide
		''' more extensive monitoring facilities.
		''' </summary>
		''' <returns> the collection of threads </returns>
		Public Property queuedThreads As ICollection(Of Thread)
			Get
				Dim list As New List(Of Thread)
				Dim p As Node = tail
				Do While p IsNot Nothing
					Dim t As Thread = p.thread_Renamed
					If t IsNot Nothing Then list.add(t)
					p = p.prev
				Loop
				Return list
			End Get
		End Property

		''' <summary>
		''' Returns a collection containing threads that may be waiting to
		''' acquire in exclusive mode. This has the same properties
		''' as <seealso cref="#getQueuedThreads"/> except that it only returns
		''' those threads waiting due to an exclusive acquire.
		''' </summary>
		''' <returns> the collection of threads </returns>
		Public Property exclusiveQueuedThreads As ICollection(Of Thread)
			Get
				Dim list As New List(Of Thread)
				Dim p As Node = tail
				Do While p IsNot Nothing
					If Not p.shared Then
						Dim t As Thread = p.thread_Renamed
						If t IsNot Nothing Then list.add(t)
					End If
					p = p.prev
				Loop
				Return list
			End Get
		End Property

		''' <summary>
		''' Returns a collection containing threads that may be waiting to
		''' acquire in shared mode. This has the same properties
		''' as <seealso cref="#getQueuedThreads"/> except that it only returns
		''' those threads waiting due to a shared acquire.
		''' </summary>
		''' <returns> the collection of threads </returns>
		Public Property sharedQueuedThreads As ICollection(Of Thread)
			Get
				Dim list As New List(Of Thread)
				Dim p As Node = tail
				Do While p IsNot Nothing
					If p.shared Then
						Dim t As Thread = p.thread_Renamed
						If t IsNot Nothing Then list.add(t)
					End If
					p = p.prev
				Loop
				Return list
			End Get
		End Property

		''' <summary>
		''' Returns a string identifying this synchronizer, as well as its state.
		''' The state, in brackets, includes the String {@code "State ="}
		''' followed by the current value of <seealso cref="#getState"/>, and either
		''' {@code "nonempty"} or {@code "empty"} depending on whether the
		''' queue is empty.
		''' </summary>
		''' <returns> a string identifying this synchronizer, as well as its state </returns>
		Public Overrides Function ToString() As String
			Dim s As Integer = state
			Dim q As String = If(hasQueuedThreads(), "non", "")
			Return MyBase.ToString() & "[State = " & s & ", " & q & "empty queue]"
		End Function


		' Internal support methods for Conditions

		''' <summary>
		''' Returns true if a node, always one that was initially placed on
		''' a condition queue, is now waiting to reacquire on sync queue. </summary>
		''' <param name="node"> the node </param>
		''' <returns> true if is reacquiring </returns>
		Friend Function isOnSyncQueue(ByVal node_Renamed As Node) As Boolean
			If node_Renamed.waitStatus = Node.CONDITION OrElse node_Renamed.prev Is Nothing Then Return False
			If node_Renamed.next IsNot Nothing Then ' If has successor, it must be on queue Return True
	'        
	'         * node.prev can be non-null, but not yet on queue because
	'         * the CAS to place it on queue can fail. So we have to
	'         * traverse from tail to make sure it actually made it.  It
	'         * will always be near the tail in calls to this method, and
	'         * unless the CAS failed (which is unlikely), it will be
	'         * there, so we hardly ever traverse much.
	'         
			Return findNodeFromTail(node_Renamed)
		End Function

		''' <summary>
		''' Returns true if node is on sync queue by searching backwards from tail.
		''' Called only when needed by isOnSyncQueue. </summary>
		''' <returns> true if present </returns>
		Private Function findNodeFromTail(ByVal node_Renamed As Node) As Boolean
			Dim t As Node = tail
			Do
				If t Is node_Renamed Then Return True
				If t Is Nothing Then Return False
				t = t.prev
			Loop
		End Function

		''' <summary>
		''' Transfers a node from a condition queue onto sync queue.
		''' Returns true if successful. </summary>
		''' <param name="node"> the node </param>
		''' <returns> true if successfully transferred (else the node was
		''' cancelled before signal) </returns>
		Friend Function transferForSignal(ByVal node_Renamed As Node) As Boolean
	'        
	'         * If cannot change waitStatus, the node has been cancelled.
	'         
			If Not compareAndSetWaitStatus(node_Renamed, Node.CONDITION, 0) Then Return False

	'        
	'         * Splice onto queue and try to set waitStatus of predecessor to
	'         * indicate that thread is (probably) waiting. If cancelled or
	'         * attempt to set waitStatus fails, wake up to resync (in which
	'         * case the waitStatus can be transiently and harmlessly wrong).
	'         
			Dim p As Node = enq(node_Renamed)
			Dim ws As Integer = p.waitStatus
			If ws > 0 OrElse (Not compareAndSetWaitStatus(p, ws, Node.SIGNAL)) Then LockSupport.unpark(node_Renamed.thread_Renamed)
			Return True
		End Function

		''' <summary>
		''' Transfers node, if necessary, to sync queue after a cancelled wait.
		''' Returns true if thread was cancelled before being signalled.
		''' </summary>
		''' <param name="node"> the node </param>
		''' <returns> true if cancelled before the node was signalled </returns>
		Friend Function transferAfterCancelledWait(ByVal node_Renamed As Node) As Boolean
			If compareAndSetWaitStatus(node_Renamed, Node.CONDITION, 0) Then
				enq(node_Renamed)
				Return True
			End If
	'        
	'         * If we lost out to a signal(), then we can't proceed
	'         * until it finishes its enq().  Cancelling during an
	'         * incomplete transfer is both rare and transient, so just
	'         * spin.
	'         
			Do While Not isOnSyncQueue(node_Renamed)
				Thread.yield()
			Loop
			Return False
		End Function

		''' <summary>
		''' Invokes release with current state value; returns saved state.
		''' Cancels node and throws exception on failure. </summary>
		''' <param name="node"> the condition node for this wait </param>
		''' <returns> previous sync state </returns>
		Friend Function fullyRelease(ByVal node_Renamed As Node) As Integer
			Dim failed As Boolean = True
			Try
				Dim savedState As Integer = state
				If release(savedState) Then
					failed = False
					Return savedState
				Else
					Throw New IllegalMonitorStateException
				End If
			Finally
				If failed Then node_Renamed.waitStatus = Node.CANCELLED
			End Try
		End Function

		' Instrumentation methods for conditions

		''' <summary>
		''' Queries whether the given ConditionObject
		''' uses this synchronizer as its lock.
		''' </summary>
		''' <param name="condition"> the condition </param>
		''' <returns> {@code true} if owned </returns>
		''' <exception cref="NullPointerException"> if the condition is null </exception>
		Public Function owns(ByVal condition As ConditionObject) As Boolean
			Return condition.isOwnedBy(Me)
		End Function

		''' <summary>
		''' Queries whether any threads are waiting on the given condition
		''' associated with this synchronizer. Note that because timeouts
		''' and interrupts may occur at any time, a {@code true} return
		''' does not guarantee that a future {@code signal} will awaken
		''' any threads.  This method is designed primarily for use in
		''' monitoring of the system state.
		''' </summary>
		''' <param name="condition"> the condition </param>
		''' <returns> {@code true} if there are any waiting threads </returns>
		''' <exception cref="IllegalMonitorStateException"> if exclusive synchronization
		'''         is not held </exception>
		''' <exception cref="IllegalArgumentException"> if the given condition is
		'''         not associated with this synchronizer </exception>
		''' <exception cref="NullPointerException"> if the condition is null </exception>
		Public Function hasWaiters(ByVal condition As ConditionObject) As Boolean
			If Not owns(condition) Then Throw New IllegalArgumentException("Not owner")
			Return condition.hasWaiters()
		End Function

		''' <summary>
		''' Returns an estimate of the number of threads waiting on the
		''' given condition associated with this synchronizer. Note that
		''' because timeouts and interrupts may occur at any time, the
		''' estimate serves only as an upper bound on the actual number of
		''' waiters.  This method is designed for use in monitoring of the
		''' system state, not for synchronization control.
		''' </summary>
		''' <param name="condition"> the condition </param>
		''' <returns> the estimated number of waiting threads </returns>
		''' <exception cref="IllegalMonitorStateException"> if exclusive synchronization
		'''         is not held </exception>
		''' <exception cref="IllegalArgumentException"> if the given condition is
		'''         not associated with this synchronizer </exception>
		''' <exception cref="NullPointerException"> if the condition is null </exception>
		Public Function getWaitQueueLength(ByVal condition As ConditionObject) As Integer
			If Not owns(condition) Then Throw New IllegalArgumentException("Not owner")
			Return condition.waitQueueLength
		End Function

		''' <summary>
		''' Returns a collection containing those threads that may be
		''' waiting on the given condition associated with this
		''' synchronizer.  Because the actual set of threads may change
		''' dynamically while constructing this result, the returned
		''' collection is only a best-effort estimate. The elements of the
		''' returned collection are in no particular order.
		''' </summary>
		''' <param name="condition"> the condition </param>
		''' <returns> the collection of threads </returns>
		''' <exception cref="IllegalMonitorStateException"> if exclusive synchronization
		'''         is not held </exception>
		''' <exception cref="IllegalArgumentException"> if the given condition is
		'''         not associated with this synchronizer </exception>
		''' <exception cref="NullPointerException"> if the condition is null </exception>
		Public Function getWaitingThreads(ByVal condition As ConditionObject) As ICollection(Of Thread)
			If Not owns(condition) Then Throw New IllegalArgumentException("Not owner")
			Return condition.waitingThreads
		End Function

		''' <summary>
		''' Condition implementation for a {@link
		''' AbstractQueuedSynchronizer} serving as the basis of a {@link
		''' Lock} implementation.
		''' 
		''' <p>Method documentation for this class describes mechanics,
		''' not behavioral specifications from the point of view of Lock
		''' and Condition users. Exported versions of this class will in
		''' general need to be accompanied by documentation describing
		''' condition semantics that rely on those of the associated
		''' {@code AbstractQueuedSynchronizer}.
		''' 
		''' <p>This class is Serializable, but all fields are transient,
		''' so deserialized conditions have no waiters.
		''' </summary>
		<Serializable> _
		Public Class ConditionObject
			Implements Condition

			Private ReadOnly outerInstance As AbstractQueuedSynchronizer

			Private Const serialVersionUID As Long = 1173984872572414699L
			''' <summary>
			''' First node of condition queue. </summary>
			<NonSerialized> _
			Private firstWaiter As Node
			''' <summary>
			''' Last node of condition queue. </summary>
			<NonSerialized> _
			Private lastWaiter As Node

			''' <summary>
			''' Creates a new {@code ConditionObject} instance.
			''' </summary>
			Public Sub New(ByVal outerInstance As AbstractQueuedSynchronizer)
					Me.outerInstance = outerInstance
			End Sub

			' Internal methods

			''' <summary>
			''' Adds a new waiter to wait queue. </summary>
			''' <returns> its new wait node </returns>
			Private Function addConditionWaiter() As Node
				Dim t As Node = lastWaiter
				' If lastWaiter is cancelled, clean out.
				If t IsNot Nothing AndAlso t.waitStatus <> Node.CONDITION Then
					unlinkCancelledWaiters()
					t = lastWaiter
				End If
				Dim node_Renamed As New Node(Thread.CurrentThread, Node.CONDITION)
				If t Is Nothing Then
					firstWaiter = node_Renamed
				Else
					t.nextWaiter = node_Renamed
				End If
				lastWaiter = node_Renamed
				Return node_Renamed
			End Function

			''' <summary>
			''' Removes and transfers nodes until hit non-cancelled one or
			''' null. Split out from signal in part to encourage compilers
			''' to inline the case of no waiters. </summary>
			''' <param name="first"> (non-null) the first node on condition queue </param>
			Private Sub doSignal(ByVal first As Node)
				Do
					firstWaiter = first.nextWaiter
					If firstWaiter Is Nothing Then lastWaiter = Nothing
					first.nextWaiter = Nothing
					first = firstWaiter
				Loop While (Not outerInstance.transferForSignal(first)) AndAlso first IsNot Nothing
			End Sub

			''' <summary>
			''' Removes and transfers all nodes. </summary>
			''' <param name="first"> (non-null) the first node on condition queue </param>
			Private Sub doSignalAll(ByVal first As Node)
					firstWaiter = Nothing
					lastWaiter = firstWaiter
				Do
					Dim [next] As Node = first.nextWaiter
					first.nextWaiter = Nothing
					outerInstance.transferForSignal(first)
					first = [next]
				Loop While first IsNot Nothing
			End Sub

			''' <summary>
			''' Unlinks cancelled waiter nodes from condition queue.
			''' Called only while holding lock. This is called when
			''' cancellation occurred during condition wait, and upon
			''' insertion of a new waiter when lastWaiter is seen to have
			''' been cancelled. This method is needed to avoid garbage
			''' retention in the absence of signals. So even though it may
			''' require a full traversal, it comes into play only when
			''' timeouts or cancellations occur in the absence of
			''' signals. It traverses all nodes rather than stopping at a
			''' particular target to unlink all pointers to garbage nodes
			''' without requiring many re-traversals during cancellation
			''' storms.
			''' </summary>
			Private Sub unlinkCancelledWaiters()
				Dim t As Node = firstWaiter
				Dim trail As Node = Nothing
				Do While t IsNot Nothing
					Dim [next] As Node = t.nextWaiter
					If t.waitStatus <> Node.CONDITION Then
						t.nextWaiter = Nothing
						If trail Is Nothing Then
							firstWaiter = [next]
						Else
							trail.nextWaiter = [next]
						End If
						If [next] Is Nothing Then lastWaiter = trail
					Else
						trail = t
					End If
					t = [next]
				Loop
			End Sub

			' public methods

			''' <summary>
			''' Moves the longest-waiting thread, if one exists, from the
			''' wait queue for this condition to the wait queue for the
			''' owning lock.
			''' </summary>
			''' <exception cref="IllegalMonitorStateException"> if <seealso cref="#isHeldExclusively"/>
			'''         returns {@code false} </exception>
			Public Sub signal() Implements Condition.signal
				If Not outerInstance.heldExclusively Then Throw New IllegalMonitorStateException
				Dim first As Node = firstWaiter
				If first IsNot Nothing Then doSignal(first)
			End Sub

			''' <summary>
			''' Moves all threads from the wait queue for this condition to
			''' the wait queue for the owning lock.
			''' </summary>
			''' <exception cref="IllegalMonitorStateException"> if <seealso cref="#isHeldExclusively"/>
			'''         returns {@code false} </exception>
			Public Sub signalAll() Implements Condition.signalAll
				If Not outerInstance.heldExclusively Then Throw New IllegalMonitorStateException
				Dim first As Node = firstWaiter
				If first IsNot Nothing Then doSignalAll(first)
			End Sub

			''' <summary>
			''' Implements uninterruptible condition wait.
			''' <ol>
			''' <li> Save lock state returned by <seealso cref="#getState"/>.
			''' <li> Invoke <seealso cref="#release"/> with saved state as argument,
			'''      throwing IllegalMonitorStateException if it fails.
			''' <li> Block until signalled.
			''' <li> Reacquire by invoking specialized version of
			'''      <seealso cref="#acquire"/> with saved state as argument.
			''' </ol>
			''' </summary>
			Public Sub awaitUninterruptibly() Implements Condition.awaitUninterruptibly
				Dim node_Renamed As Node = addConditionWaiter()
				Dim savedState As Integer = outerInstance.fullyRelease(node_Renamed)
				Dim interrupted As Boolean = False
				Do While Not outerInstance.isOnSyncQueue(node_Renamed)
					LockSupport.park(Me)
					If Thread.interrupted() Then interrupted = True
				Loop
				If outerInstance.acquireQueued(node_Renamed, savedState) OrElse interrupted Then selfInterrupt()
			End Sub

	'        
	'         * For interruptible waits, we need to track whether to throw
	'         * InterruptedException, if interrupted while blocked on
	'         * condition, versus reinterrupt current thread, if
	'         * interrupted while blocked waiting to re-acquire.
	'         

			''' <summary>
			''' Mode meaning to reinterrupt on exit from wait </summary>
			Private Const REINTERRUPT As Integer = 1
			''' <summary>
			''' Mode meaning to throw InterruptedException on exit from wait </summary>
			Private Const THROW_IE As Integer = -1

			''' <summary>
			''' Checks for interrupt, returning THROW_IE if interrupted
			''' before signalled, REINTERRUPT if after signalled, or
			''' 0 if not interrupted.
			''' </summary>
			Private Function checkInterruptWhileWaiting(ByVal node_Renamed As Node) As Integer
				Return If(Thread.interrupted(), (If(outerInstance.transferAfterCancelledWait(node_Renamed), THROW_IE, REINTERRUPT)), 0)
			End Function

			''' <summary>
			''' Throws InterruptedException, reinterrupts current thread, or
			''' does nothing, depending on mode.
			''' </summary>
			Private Sub reportInterruptAfterWait(ByVal interruptMode As Integer)
				If interruptMode = THROW_IE Then
					Throw New InterruptedException
				ElseIf interruptMode = REINTERRUPT Then
					selfInterrupt()
				End If
			End Sub

			''' <summary>
			''' Implements interruptible condition wait.
			''' <ol>
			''' <li> If current thread is interrupted, throw InterruptedException.
			''' <li> Save lock state returned by <seealso cref="#getState"/>.
			''' <li> Invoke <seealso cref="#release"/> with saved state as argument,
			'''      throwing IllegalMonitorStateException if it fails.
			''' <li> Block until signalled or interrupted.
			''' <li> Reacquire by invoking specialized version of
			'''      <seealso cref="#acquire"/> with saved state as argument.
			''' <li> If interrupted while blocked in step 4, throw InterruptedException.
			''' </ol>
			''' </summary>
			Public Sub [await]() Implements Condition.await
				If Thread.interrupted() Then Throw New InterruptedException
				Dim node_Renamed As Node = addConditionWaiter()
				Dim savedState As Integer = outerInstance.fullyRelease(node_Renamed)
				Dim interruptMode As Integer = 0
				Do While Not outerInstance.isOnSyncQueue(node_Renamed)
					LockSupport.park(Me)
					interruptMode = checkInterruptWhileWaiting(node_Renamed)
					If interruptMode <> 0 Then Exit Do
				Loop
				If outerInstance.acquireQueued(node_Renamed, savedState) AndAlso interruptMode <> THROW_IE Then interruptMode = REINTERRUPT
				If node_Renamed.nextWaiter IsNot Nothing Then ' clean up if cancelled unlinkCancelledWaiters()
				If interruptMode <> 0 Then reportInterruptAfterWait(interruptMode)
			End Sub

			''' <summary>
			''' Implements timed condition wait.
			''' <ol>
			''' <li> If current thread is interrupted, throw InterruptedException.
			''' <li> Save lock state returned by <seealso cref="#getState"/>.
			''' <li> Invoke <seealso cref="#release"/> with saved state as argument,
			'''      throwing IllegalMonitorStateException if it fails.
			''' <li> Block until signalled, interrupted, or timed out.
			''' <li> Reacquire by invoking specialized version of
			'''      <seealso cref="#acquire"/> with saved state as argument.
			''' <li> If interrupted while blocked in step 4, throw InterruptedException.
			''' </ol>
			''' </summary>
			Public Function awaitNanos(ByVal nanosTimeout As Long) As Long Implements Condition.awaitNanos
				If Thread.interrupted() Then Throw New InterruptedException
				Dim node_Renamed As Node = addConditionWaiter()
				Dim savedState As Integer = outerInstance.fullyRelease(node_Renamed)
				Dim deadline As Long = System.nanoTime() + nanosTimeout
				Dim interruptMode As Integer = 0
				Do While Not outerInstance.isOnSyncQueue(node_Renamed)
					If nanosTimeout <= 0L Then
						outerInstance.transferAfterCancelledWait(node_Renamed)
						Exit Do
					End If
					If nanosTimeout >= spinForTimeoutThreshold Then LockSupport.parkNanos(Me, nanosTimeout)
					interruptMode = checkInterruptWhileWaiting(node_Renamed)
					If interruptMode <> 0 Then Exit Do
					nanosTimeout = deadline - System.nanoTime()
				Loop
				If outerInstance.acquireQueued(node_Renamed, savedState) AndAlso interruptMode <> THROW_IE Then interruptMode = REINTERRUPT
				If node_Renamed.nextWaiter IsNot Nothing Then unlinkCancelledWaiters()
				If interruptMode <> 0 Then reportInterruptAfterWait(interruptMode)
				Return deadline - System.nanoTime()
			End Function

			''' <summary>
			''' Implements absolute timed condition wait.
			''' <ol>
			''' <li> If current thread is interrupted, throw InterruptedException.
			''' <li> Save lock state returned by <seealso cref="#getState"/>.
			''' <li> Invoke <seealso cref="#release"/> with saved state as argument,
			'''      throwing IllegalMonitorStateException if it fails.
			''' <li> Block until signalled, interrupted, or timed out.
			''' <li> Reacquire by invoking specialized version of
			'''      <seealso cref="#acquire"/> with saved state as argument.
			''' <li> If interrupted while blocked in step 4, throw InterruptedException.
			''' <li> If timed out while blocked in step 4, return false, else true.
			''' </ol>
			''' </summary>
			Public Function awaitUntil(ByVal deadline As DateTime?) As Boolean Implements Condition.awaitUntil
				Dim abstime As Long = deadline.Value.time
				If Thread.interrupted() Then Throw New InterruptedException
				Dim node_Renamed As Node = addConditionWaiter()
				Dim savedState As Integer = outerInstance.fullyRelease(node_Renamed)
				Dim timedout As Boolean = False
				Dim interruptMode As Integer = 0
				Do While Not outerInstance.isOnSyncQueue(node_Renamed)
					If System.currentTimeMillis() > abstime Then
						timedout = outerInstance.transferAfterCancelledWait(node_Renamed)
						Exit Do
					End If
					LockSupport.parkUntil(Me, abstime)
					interruptMode = checkInterruptWhileWaiting(node_Renamed)
					If interruptMode <> 0 Then Exit Do
				Loop
				If outerInstance.acquireQueued(node_Renamed, savedState) AndAlso interruptMode <> THROW_IE Then interruptMode = REINTERRUPT
				If node_Renamed.nextWaiter IsNot Nothing Then unlinkCancelledWaiters()
				If interruptMode <> 0 Then reportInterruptAfterWait(interruptMode)
				Return Not timedout
			End Function

			''' <summary>
			''' Implements timed condition wait.
			''' <ol>
			''' <li> If current thread is interrupted, throw InterruptedException.
			''' <li> Save lock state returned by <seealso cref="#getState"/>.
			''' <li> Invoke <seealso cref="#release"/> with saved state as argument,
			'''      throwing IllegalMonitorStateException if it fails.
			''' <li> Block until signalled, interrupted, or timed out.
			''' <li> Reacquire by invoking specialized version of
			'''      <seealso cref="#acquire"/> with saved state as argument.
			''' <li> If interrupted while blocked in step 4, throw InterruptedException.
			''' <li> If timed out while blocked in step 4, return false, else true.
			''' </ol>
			''' </summary>
			Public Function [await](ByVal time As Long, ByVal unit As java.util.concurrent.TimeUnit) As Boolean Implements Condition.await
				Dim nanosTimeout As Long = unit.toNanos(time)
				If Thread.interrupted() Then Throw New InterruptedException
				Dim node_Renamed As Node = addConditionWaiter()
				Dim savedState As Integer = outerInstance.fullyRelease(node_Renamed)
				Dim deadline As Long = System.nanoTime() + nanosTimeout
				Dim timedout As Boolean = False
				Dim interruptMode As Integer = 0
				Do While Not outerInstance.isOnSyncQueue(node_Renamed)
					If nanosTimeout <= 0L Then
						timedout = outerInstance.transferAfterCancelledWait(node_Renamed)
						Exit Do
					End If
					If nanosTimeout >= spinForTimeoutThreshold Then LockSupport.parkNanos(Me, nanosTimeout)
					interruptMode = checkInterruptWhileWaiting(node_Renamed)
					If interruptMode <> 0 Then Exit Do
					nanosTimeout = deadline - System.nanoTime()
				Loop
				If outerInstance.acquireQueued(node_Renamed, savedState) AndAlso interruptMode <> THROW_IE Then interruptMode = REINTERRUPT
				If node_Renamed.nextWaiter IsNot Nothing Then unlinkCancelledWaiters()
				If interruptMode <> 0 Then reportInterruptAfterWait(interruptMode)
				Return Not timedout
			End Function

			'  support for instrumentation

			''' <summary>
			''' Returns true if this condition was created by the given
			''' synchronization object.
			''' </summary>
			''' <returns> {@code true} if owned </returns>
			Friend Function isOwnedBy(ByVal sync As AbstractQueuedSynchronizer) As Boolean
				Return sync Is AbstractQueuedSynchronizer.this
			End Function

			''' <summary>
			''' Queries whether any threads are waiting on this condition.
			''' Implements <seealso cref="AbstractQueuedSynchronizer#hasWaiters(ConditionObject)"/>.
			''' </summary>
			''' <returns> {@code true} if there are any waiting threads </returns>
			''' <exception cref="IllegalMonitorStateException"> if <seealso cref="#isHeldExclusively"/>
			'''         returns {@code false} </exception>
			Protected Friend Function hasWaiters() As Boolean
				If Not outerInstance.heldExclusively Then Throw New IllegalMonitorStateException
				Dim w As Node = firstWaiter
				Do While w IsNot Nothing
					If w.waitStatus = Node.CONDITION Then Return True
					w = w.nextWaiter
				Loop
				Return False
			End Function

			''' <summary>
			''' Returns an estimate of the number of threads waiting on
			''' this condition.
			''' Implements <seealso cref="AbstractQueuedSynchronizer#getWaitQueueLength(ConditionObject)"/>.
			''' </summary>
			''' <returns> the estimated number of waiting threads </returns>
			''' <exception cref="IllegalMonitorStateException"> if <seealso cref="#isHeldExclusively"/>
			'''         returns {@code false} </exception>
			Protected Friend Property waitQueueLength As Integer
				Get
					If Not outerInstance.heldExclusively Then Throw New IllegalMonitorStateException
					Dim n As Integer = 0
					Dim w As Node = firstWaiter
					Do While w IsNot Nothing
						If w.waitStatus = Node.CONDITION Then n += 1
						w = w.nextWaiter
					Loop
					Return n
				End Get
			End Property

			''' <summary>
			''' Returns a collection containing those threads that may be
			''' waiting on this Condition.
			''' Implements <seealso cref="AbstractQueuedSynchronizer#getWaitingThreads(ConditionObject)"/>.
			''' </summary>
			''' <returns> the collection of threads </returns>
			''' <exception cref="IllegalMonitorStateException"> if <seealso cref="#isHeldExclusively"/>
			'''         returns {@code false} </exception>
			Protected Friend Property waitingThreads As ICollection(Of Thread)
				Get
					If Not outerInstance.heldExclusively Then Throw New IllegalMonitorStateException
					Dim list As New List(Of Thread)
					Dim w As Node = firstWaiter
					Do While w IsNot Nothing
						If w.waitStatus = Node.CONDITION Then
							Dim t As Thread = w.thread_Renamed
							If t IsNot Nothing Then list.add(t)
						End If
						w = w.nextWaiter
					Loop
					Return list
				End Get
			End Property
		End Class

		''' <summary>
		''' Setup to support compareAndSet. We need to natively implement
		''' this here: For the sake of permitting future enhancements, we
		''' cannot explicitly subclass AtomicInteger, which would be
		''' efficient and useful otherwise. So, as the lesser of evils, we
		''' natively implement using hotspot intrinsics API. And while we
		''' are at it, we do the same for other CASable fields (which could
		''' otherwise be done with atomic field updaters).
		''' </summary>
		Private Shared ReadOnly unsafe As sun.misc.Unsafe = sun.misc.Unsafe.unsafe
		Private Shared ReadOnly stateOffset As Long
		Private Shared ReadOnly headOffset As Long
		Private Shared ReadOnly tailOffset As Long
		Private Shared ReadOnly waitStatusOffset As Long
		Private Shared ReadOnly nextOffset As Long

		Shared Sub New()
			Try
				stateOffset = unsafe.objectFieldOffset(GetType(AbstractQueuedSynchronizer).getDeclaredField("state"))
				headOffset = unsafe.objectFieldOffset(GetType(AbstractQueuedSynchronizer).getDeclaredField("head"))
				tailOffset = unsafe.objectFieldOffset(GetType(AbstractQueuedSynchronizer).getDeclaredField("tail"))
				waitStatusOffset = unsafe.objectFieldOffset(GetType(Node).getDeclaredField("waitStatus"))
				nextOffset = unsafe.objectFieldOffset(GetType(Node).getDeclaredField("next"))

			Catch ex As Exception
				Throw New [Error](ex)
			End Try
		End Sub

		''' <summary>
		''' CAS head field. Used only by enq.
		''' </summary>
		Private Function compareAndSetHead(ByVal update As Node) As Boolean
			Return unsafe.compareAndSwapObject(Me, headOffset, Nothing, update)
		End Function

		''' <summary>
		''' CAS tail field. Used only by enq.
		''' </summary>
		Private Function compareAndSetTail(ByVal expect As Node, ByVal update As Node) As Boolean
			Return unsafe.compareAndSwapObject(Me, tailOffset, expect, update)
		End Function

		''' <summary>
		''' CAS waitStatus field of a node.
		''' </summary>
		Private Shared Function compareAndSetWaitStatus(ByVal node_Renamed As Node, ByVal expect As Integer, ByVal update As Integer) As Boolean
			Return unsafe.compareAndSwapInt(node_Renamed, waitStatusOffset, expect, update)
		End Function

		''' <summary>
		''' CAS next field of a node.
		''' </summary>
		Private Shared Function compareAndSetNext(ByVal node_Renamed As Node, ByVal expect As Node, ByVal update As Node) As Boolean
			Return unsafe.compareAndSwapObject(node_Renamed, nextOffset, expect, update)
		End Function
	End Class

End Namespace
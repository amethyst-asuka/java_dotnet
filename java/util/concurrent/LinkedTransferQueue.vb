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

Namespace java.util.concurrent


	''' <summary>
	''' An unbounded <seealso cref="TransferQueue"/> based on linked nodes.
	''' This queue orders elements FIFO (first-in-first-out) with respect
	''' to any given producer.  The <em>head</em> of the queue is that
	''' element that has been on the queue the longest time for some
	''' producer.  The <em>tail</em> of the queue is that element that has
	''' been on the queue the shortest time for some producer.
	''' 
	''' <p>Beware that, unlike in most collections, the {@code size} method
	''' is <em>NOT</em> a constant-time operation. Because of the
	''' asynchronous nature of these queues, determining the current number
	''' of elements requires a traversal of the elements, and so may report
	''' inaccurate results if this collection is modified during traversal.
	''' Additionally, the bulk operations {@code addAll},
	''' {@code removeAll}, {@code retainAll}, {@code containsAll},
	''' {@code equals}, and {@code toArray} are <em>not</em> guaranteed
	''' to be performed atomically. For example, an iterator operating
	''' concurrently with an {@code addAll} operation might view only some
	''' of the added elements.
	''' 
	''' <p>This class and its iterator implement all of the
	''' <em>optional</em> methods of the <seealso cref="Collection"/> and {@link
	''' Iterator} interfaces.
	''' 
	''' <p>Memory consistency effects: As with other concurrent
	''' collections, actions in a thread prior to placing an object into a
	''' {@code LinkedTransferQueue}
	''' <a href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	''' actions subsequent to the access or removal of that element from
	''' the {@code LinkedTransferQueue} in another thread.
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @since 1.7
	''' @author Doug Lea </summary>
	''' @param <E> the type of elements held in this collection </param>
	<Serializable> _
	Public Class LinkedTransferQueue(Of E)
		Inherits java.util.AbstractQueue(Of E)
		Implements TransferQueue(Of E)

		Private Const serialVersionUID As Long = -3223113410248163686L

	'    
	'     * *** Overview of Dual Queues with Slack ***
	'     *
	'     * Dual Queues, introduced by Scherer and Scott
	'     * (http://www.cs.rice.edu/~wns1/papers/2004-DISC-DDS.pdf) are
	'     * (linked) queues in which nodes may represent either data or
	'     * requests.  When a thread tries to enqueue a data node, but
	'     * encounters a request node, it instead "matches" and removes it;
	'     * and vice versa for enqueuing requests. Blocking Dual Queues
	'     * arrange that threads enqueuing unmatched requests block until
	'     * other threads provide the match. Dual Synchronous Queues (see
	'     * Scherer, Lea, & Scott
	'     * http://www.cs.rochester.edu/u/scott/papers/2009_Scherer_CACM_SSQ.pdf)
	'     * additionally arrange that threads enqueuing unmatched data also
	'     * block.  Dual Transfer Queues support all of these modes, as
	'     * dictated by callers.
	'     *
	'     * A FIFO dual queue may be implemented using a variation of the
	'     * Michael & Scott (M&S) lock-free queue algorithm
	'     * (http://www.cs.rochester.edu/u/scott/papers/1996_PODC_queues.pdf).
	'     * It maintains two pointer fields, "head", pointing to a
	'     * (matched) node that in turn points to the first actual
	'     * (unmatched) queue node (or null if empty); and "tail" that
	'     * points to the last node on the queue (or again null if
	'     * empty). For example, here is a possible queue with four data
	'     * elements:
	'     *
	'     *  head                tail
	'     *    |                   |
	'     *    v                   v
	'     *    M -> U -> U -> U -> U
	'     *
	'     * The M&S queue algorithm is known to be prone to scalability and
	'     * overhead limitations when maintaining (via CAS) these head and
	'     * tail pointers. This has led to the development of
	'     * contention-reducing variants such as elimination arrays (see
	'     * Moir et al http://portal.acm.org/citation.cfm?id=1074013) and
	'     * optimistic back pointers (see Ladan-Mozes & Shavit
	'     * http://people.csail.mit.edu/edya/publications/OptimisticFIFOQueue-journal.pdf).
	'     * However, the nature of dual queues enables a simpler tactic for
	'     * improving M&S-style implementations when dual-ness is needed.
	'     *
	'     * In a dual queue, each node must atomically maintain its match
	'     * status. While there are other possible variants, we implement
	'     * this here as: for a data-mode node, matching entails CASing an
	'     * "item" field from a non-null data value to null upon match, and
	'     * vice-versa for request nodes, CASing from null to a data
	'     * value. (Note that the linearization properties of this style of
	'     * queue are easy to verify -- elements are made available by
	'     * linking, and unavailable by matching.) Compared to plain M&S
	'     * queues, this property of dual queues requires one additional
	'     * successful atomic operation per enq/deq pair. But it also
	'     * enables lower cost variants of queue maintenance mechanics. (A
	'     * variation of this idea applies even for non-dual queues that
	'     * support deletion of interior elements, such as
	'     * j.u.c.ConcurrentLinkedQueue.)
	'     *
	'     * Once a node is matched, its match status can never again
	'     * change.  We may thus arrange that the linked list of them
	'     * contain a prefix of zero or more matched nodes, followed by a
	'     * suffix of zero or more unmatched nodes. (Note that we allow
	'     * both the prefix and suffix to be zero length, which in turn
	'     * means that we do not use a dummy header.)  If we were not
	'     * concerned with either time or space efficiency, we could
	'     * correctly perform enqueue and dequeue operations by traversing
	'     * from a pointer to the initial node; CASing the item of the
	'     * first unmatched node on match and CASing the next field of the
	'     * trailing node on appends. (Plus some special-casing when
	'     * initially empty).  While this would be a terrible idea in
	'     * itself, it does have the benefit of not requiring ANY atomic
	'     * updates on head/tail fields.
	'     *
	'     * We introduce here an approach that lies between the extremes of
	'     * never versus always updating queue (head and tail) pointers.
	'     * This offers a tradeoff between sometimes requiring extra
	'     * traversal steps to locate the first and/or last unmatched
	'     * nodes, versus the reduced overhead and contention of fewer
	'     * updates to queue pointers. For example, a possible snapshot of
	'     * a queue is:
	'     *
	'     *  head           tail
	'     *    |              |
	'     *    v              v
	'     *    M -> M -> U -> U -> U -> U
	'     *
	'     * The best value for this "slack" (the targeted maximum distance
	'     * between the value of "head" and the first unmatched node, and
	'     * similarly for "tail") is an empirical matter. We have found
	'     * that using very small constants in the range of 1-3 work best
	'     * over a range of platforms. Larger values introduce increasing
	'     * costs of cache misses and risks of long traversal chains, while
	'     * smaller values increase CAS contention and overhead.
	'     *
	'     * Dual queues with slack differ from plain M&S dual queues by
	'     * virtue of only sometimes updating head or tail pointers when
	'     * matching, appending, or even traversing nodes; in order to
	'     * maintain a targeted slack.  The idea of "sometimes" may be
	'     * operationalized in several ways. The simplest is to use a
	'     * per-operation counter incremented on each traversal step, and
	'     * to try (via CAS) to update the associated queue pointer
	'     * whenever the count exceeds a threshold. Another, that requires
	'     * more overhead, is to use random number generators to update
	'     * with a given probability per traversal step.
	'     *
	'     * In any strategy along these lines, because CASes updating
	'     * fields may fail, the actual slack may exceed targeted
	'     * slack. However, they may be retried at any time to maintain
	'     * targets.  Even when using very small slack values, this
	'     * approach works well for dual queues because it allows all
	'     * operations up to the point of matching or appending an item
	'     * (hence potentially allowing progress by another thread) to be
	'     * read-only, thus not introducing any further contention. As
	'     * described below, we implement this by performing slack
	'     * maintenance retries only after these points.
	'     *
	'     * As an accompaniment to such techniques, traversal overhead can
	'     * be further reduced without increasing contention of head
	'     * pointer updates: Threads may sometimes shortcut the "next" link
	'     * path from the current "head" node to be closer to the currently
	'     * known first unmatched node, and similarly for tail. Again, this
	'     * may be triggered with using thresholds or randomization.
	'     *
	'     * These ideas must be further extended to avoid unbounded amounts
	'     * of costly-to-reclaim garbage caused by the sequential "next"
	'     * links of nodes starting at old forgotten head nodes: As first
	'     * described in detail by Boehm
	'     * (http://portal.acm.org/citation.cfm?doid=503272.503282) if a GC
	'     * delays noticing that any arbitrarily old node has become
	'     * garbage, all newer dead nodes will also be unreclaimed.
	'     * (Similar issues arise in non-GC environments.)  To cope with
	'     * this in our implementation, upon CASing to advance the head
	'     * pointer, we set the "next" link of the previous head to point
	'     * only to itself; thus limiting the length of connected dead lists.
	'     * (We also take similar care to wipe out possibly garbage
	'     * retaining values held in other Node fields.)  However, doing so
	'     * adds some further complexity to traversal: If any "next"
	'     * pointer links to itself, it indicates that the current thread
	'     * has lagged behind a head-update, and so the traversal must
	'     * continue from the "head".  Traversals trying to find the
	'     * current tail starting from "tail" may also encounter
	'     * self-links, in which case they also continue at "head".
	'     *
	'     * It is tempting in slack-based scheme to not even use CAS for
	'     * updates (similarly to Ladan-Mozes & Shavit). However, this
	'     * cannot be done for head updates under the above link-forgetting
	'     * mechanics because an update may leave head at a detached node.
	'     * And while direct writes are possible for tail updates, they
	'     * increase the risk of long retraversals, and hence long garbage
	'     * chains, which can be much more costly than is worthwhile
	'     * considering that the cost difference of performing a CAS vs
	'     * write is smaller when they are not triggered on each operation
	'     * (especially considering that writes and CASes equally require
	'     * additional GC bookkeeping ("write barriers") that are sometimes
	'     * more costly than the writes themselves because of contention).
	'     *
	'     * *** Overview of implementation ***
	'     *
	'     * We use a threshold-based approach to updates, with a slack
	'     * threshold of two -- that is, we update head/tail when the
	'     * current pointer appears to be two or more steps away from the
	'     * first/last node. The slack value is hard-wired: a path greater
	'     * than one is naturally implemented by checking equality of
	'     * traversal pointers except when the list has only one element,
	'     * in which case we keep slack threshold at one. Avoiding tracking
	'     * explicit counts across method calls slightly simplifies an
	'     * already-messy implementation. Using randomization would
	'     * probably work better if there were a low-quality dirt-cheap
	'     * per-thread one available, but even ThreadLocalRandom is too
	'     * heavy for these purposes.
	'     *
	'     * With such a small slack threshold value, it is not worthwhile
	'     * to augment this with path short-circuiting (i.e., unsplicing
	'     * interior nodes) except in the case of cancellation/removal (see
	'     * below).
	'     *
	'     * We allow both the head and tail fields to be null before any
	'     * nodes are enqueued; initializing upon first append.  This
	'     * simplifies some other logic, as well as providing more
	'     * efficient explicit control paths instead of letting JVMs insert
	'     * implicit NullPointerExceptions when they are null.  While not
	'     * currently fully implemented, we also leave open the possibility
	'     * of re-nulling these fields when empty (which is complicated to
	'     * arrange, for little benefit.)
	'     *
	'     * All enqueue/dequeue operations are handled by the single method
	'     * "xfer" with parameters indicating whether to act as some form
	'     * of offer, put, poll, take, or transfer (each possibly with
	'     * timeout). The relative complexity of using one monolithic
	'     * method outweighs the code bulk and maintenance problems of
	'     * using separate methods for each case.
	'     *
	'     * Operation consists of up to three phases. The first is
	'     * implemented within method xfer, the second in tryAppend, and
	'     * the third in method awaitMatch.
	'     *
	'     * 1. Try to match an existing node
	'     *
	'     *    Starting at head, skip already-matched nodes until finding
	'     *    an unmatched node of opposite mode, if one exists, in which
	'     *    case matching it and returning, also if necessary updating
	'     *    head to one past the matched node (or the node itself if the
	'     *    list has no other unmatched nodes). If the CAS misses, then
	'     *    a loop retries advancing head by two steps until either
	'     *    success or the slack is at most two. By requiring that each
	'     *    attempt advances head by two (if applicable), we ensure that
	'     *    the slack does not grow without bound. Traversals also check
	'     *    if the initial head is now off-list, in which case they
	'     *    start at the new head.
	'     *
	'     *    If no candidates are found and the call was untimed
	'     *    poll/offer, (argument "how" is NOW) return.
	'     *
	'     * 2. Try to append a new node (method tryAppend)
	'     *
	'     *    Starting at current tail pointer, find the actual last node
	'     *    and try to append a new node (or if head was null, establish
	'     *    the first node). Nodes can be appended only if their
	'     *    predecessors are either already matched or are of the same
	'     *    mode. If we detect otherwise, then a new node with opposite
	'     *    mode must have been appended during traversal, so we must
	'     *    restart at phase 1. The traversal and update steps are
	'     *    otherwise similar to phase 1: Retrying upon CAS misses and
	'     *    checking for staleness.  In particular, if a self-link is
	'     *    encountered, then we can safely jump to a node on the list
	'     *    by continuing the traversal at current head.
	'     *
	'     *    On successful append, if the call was ASYNC, return.
	'     *
	'     * 3. Await match or cancellation (method awaitMatch)
	'     *
	'     *    Wait for another thread to match node; instead cancelling if
	'     *    the current thread was interrupted or the wait timed out. On
	'     *    multiprocessors, we use front-of-queue spinning: If a node
	'     *    appears to be the first unmatched node in the queue, it
	'     *    spins a bit before blocking. In either case, before blocking
	'     *    it tries to unsplice any nodes between the current "head"
	'     *    and the first unmatched node.
	'     *
	'     *    Front-of-queue spinning vastly improves performance of
	'     *    heavily contended queues. And so long as it is relatively
	'     *    brief and "quiet", spinning does not much impact performance
	'     *    of less-contended queues.  During spins threads check their
	'     *    interrupt status and generate a thread-local random number
	'     *    to decide to occasionally perform a Thread.yield. While
	'     *    yield has underdefined specs, we assume that it might help,
	'     *    and will not hurt, in limiting impact of spinning on busy
	'     *    systems.  We also use smaller (1/2) spins for nodes that are
	'     *    not known to be front but whose predecessors have not
	'     *    blocked -- these "chained" spins avoid artifacts of
	'     *    front-of-queue rules which otherwise lead to alternating
	'     *    nodes spinning vs blocking. Further, front threads that
	'     *    represent phase changes (from data to request node or vice
	'     *    versa) compared to their predecessors receive additional
	'     *    chained spins, reflecting longer paths typically required to
	'     *    unblock threads during phase changes.
	'     *
	'     *
	'     * ** Unlinking removed interior nodes **
	'     *
	'     * In addition to minimizing garbage retention via self-linking
	'     * described above, we also unlink removed interior nodes. These
	'     * may arise due to timed out or interrupted waits, or calls to
	'     * remove(x) or Iterator.remove.  Normally, given a node that was
	'     * at one time known to be the predecessor of some node s that is
	'     * to be removed, we can unsplice s by CASing the next field of
	'     * its predecessor if it still points to s (otherwise s must
	'     * already have been removed or is now offlist). But there are two
	'     * situations in which we cannot guarantee to make node s
	'     * unreachable in this way: (1) If s is the trailing node of list
	'     * (i.e., with null next), then it is pinned as the target node
	'     * for appends, so can only be removed later after other nodes are
	'     * appended. (2) We cannot necessarily unlink s given a
	'     * predecessor node that is matched (including the case of being
	'     * cancelled): the predecessor may already be unspliced, in which
	'     * case some previous reachable node may still point to s.
	'     * (For further explanation see Herlihy & Shavit "The Art of
	'     * Multiprocessor Programming" chapter 9).  Although, in both
	'     * cases, we can rule out the need for further action if either s
	'     * or its predecessor are (or can be made to be) at, or fall off
	'     * from, the head of list.
	'     *
	'     * Without taking these into account, it would be possible for an
	'     * unbounded number of supposedly removed nodes to remain
	'     * reachable.  Situations leading to such buildup are uncommon but
	'     * can occur in practice; for example when a series of short timed
	'     * calls to poll repeatedly time out but never otherwise fall off
	'     * the list because of an untimed call to take at the front of the
	'     * queue.
	'     *
	'     * When these cases arise, rather than always retraversing the
	'     * entire list to find an actual predecessor to unlink (which
	'     * won't help for case (1) anyway), we record a conservative
	'     * estimate of possible unsplice failures (in "sweepVotes").
	'     * We trigger a full sweep when the estimate exceeds a threshold
	'     * ("SWEEP_THRESHOLD") indicating the maximum number of estimated
	'     * removal failures to tolerate before sweeping through, unlinking
	'     * cancelled nodes that were not unlinked upon initial removal.
	'     * We perform sweeps by the thread hitting threshold (rather than
	'     * background threads or by spreading work to other threads)
	'     * because in the main contexts in which removal occurs, the
	'     * caller is already timed-out, cancelled, or performing a
	'     * potentially O(n) operation (e.g. remove(x)), none of which are
	'     * time-critical enough to warrant the overhead that alternatives
	'     * would impose on other threads.
	'     *
	'     * Because the sweepVotes estimate is conservative, and because
	'     * nodes become unlinked "naturally" as they fall off the head of
	'     * the queue, and because we allow votes to accumulate even while
	'     * sweeps are in progress, there are typically significantly fewer
	'     * such nodes than estimated.  Choice of a threshold value
	'     * balances the likelihood of wasted effort and contention, versus
	'     * providing a worst-case bound on retention of interior nodes in
	'     * quiescent queues. The value defined below was chosen
	'     * empirically to balance these under various timeout scenarios.
	'     *
	'     * Note that we cannot self-link unlinked interior nodes during
	'     * sweeps. However, the associated garbage chains terminate when
	'     * some successor ultimately falls off the head of the list and is
	'     * self-linked.
	'     

		''' <summary>
		''' True if on multiprocessor </summary>
		Private Shared ReadOnly MP As Boolean = Runtime.runtime.availableProcessors() > 1

		''' <summary>
		''' The number of times to spin (with randomly interspersed calls
		''' to Thread.yield) on multiprocessor before blocking when a node
		''' is apparently the first waiter in the queue.  See above for
		''' explanation. Must be a power of two. The value is empirically
		''' derived -- it works pretty well across a variety of processors,
		''' numbers of CPUs, and OSes.
		''' </summary>
		Private Shared ReadOnly FRONT_SPINS As Integer = 1 << 7

		''' <summary>
		''' The number of times to spin before blocking when a node is
		''' preceded by another node that is apparently spinning.  Also
		''' serves as an increment to FRONT_SPINS on phase changes, and as
		''' base average frequency for yielding during spins. Must be a
		''' power of two.
		''' </summary>
		Private Shared ReadOnly CHAINED_SPINS As Integer = FRONT_SPINS >>> 1

		''' <summary>
		''' The maximum number of estimated removal failures (sweepVotes)
		''' to tolerate before sweeping through the queue unlinking
		''' cancelled nodes that were not unlinked upon initial
		''' removal. See above for explanation. The value must be at least
		''' two to avoid useless sweeps when removing trailing nodes.
		''' </summary>
		Friend Const SWEEP_THRESHOLD As Integer = 32

		''' <summary>
		''' Queue nodes. Uses Object, not E, for items to allow forgetting
		''' them after use.  Relies heavily on Unsafe mechanics to minimize
		''' unnecessary ordering constraints: Writes that are intrinsically
		''' ordered wrt other accesses or CASes use simple relaxed forms.
		''' </summary>
		Friend NotInheritable Class Node
			Friend ReadOnly isData As Boolean ' false if this is a request node
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend item As Object ' initially non-null if isData; CASed to match
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend [next] As Node
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend waiter As Thread ' null until waiting

			' CAS methods for fields
			Friend Function casNext(ByVal cmp As Node, ByVal val As Node) As Boolean
				Return UNSAFE.compareAndSwapObject(Me, nextOffset, cmp, val)
			End Function

			Friend Function casItem(ByVal cmp As Object, ByVal val As Object) As Boolean
				' assert cmp == null || cmp.getClass() != Node.class;
				Return UNSAFE.compareAndSwapObject(Me, itemOffset, cmp, val)
			End Function

			''' <summary>
			''' Constructs a new node.  Uses relaxed write because item can
			''' only be seen after publication via casNext.
			''' </summary>
			Friend Sub New(ByVal item As Object, ByVal isData As Boolean)
				UNSAFE.putObject(Me, itemOffset, item) ' relaxed write
				Me.isData = isData
			End Sub

			''' <summary>
			''' Links node to itself to avoid garbage retention.  Called
			''' only after CASing head field, so uses relaxed write.
			''' </summary>
			Friend Sub forgetNext()
				UNSAFE.putObject(Me, nextOffset, Me)
			End Sub

			''' <summary>
			''' Sets item to self and waiter to null, to avoid garbage
			''' retention after matching or cancelling. Uses relaxed writes
			''' because order is already constrained in the only calling
			''' contexts: item is forgotten only after volatile/atomic
			''' mechanics that extract items.  Similarly, clearing waiter
			''' follows either CAS or return from park (if ever parked;
			''' else we don't care).
			''' </summary>
			Friend Sub forgetContents()
				UNSAFE.putObject(Me, itemOffset, Me)
				UNSAFE.putObject(Me, waiterOffset, Nothing)
			End Sub

			''' <summary>
			''' Returns true if this node has been matched, including the
			''' case of artificial matches due to cancellation.
			''' </summary>
			Friend Property matched As Boolean
				Get
					Dim x As Object = item
					Return (x Is Me) OrElse ((x Is Nothing) = isData)
				End Get
			End Property

			''' <summary>
			''' Returns true if this is an unmatched request node.
			''' </summary>
			Friend Property unmatchedRequest As Boolean
				Get
					Return (Not isData) AndAlso item Is Nothing
				End Get
			End Property

			''' <summary>
			''' Returns true if a node with the given mode cannot be
			''' appended to this node because this node is unmatched and
			''' has opposite data mode.
			''' </summary>
			Friend Function cannotPrecede(ByVal haveData As Boolean) As Boolean
				Dim d As Boolean = isData
				Dim x As Object
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return d <> haveData AndAlso (x = item) IsNot Me AndAlso (x IsNot Nothing) = d
			End Function

			''' <summary>
			''' Tries to artificially match a data node -- used by remove.
			''' </summary>
			Friend Function tryMatchData() As Boolean
				' assert isData;
				Dim x As Object = item
				If x IsNot Nothing AndAlso x IsNot Me AndAlso casItem(x, Nothing) Then
					java.util.concurrent.locks.LockSupport.unpark(waiter)
					Return True
				End If
				Return False
			End Function

			Private Const serialVersionUID As Long = -3375979862319811754L

			' Unsafe mechanics
			Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
			Private Shared ReadOnly itemOffset As Long
			Private Shared ReadOnly nextOffset As Long
			Private Shared ReadOnly waiterOffset As Long
			Shared Sub New()
				Try
					UNSAFE = sun.misc.Unsafe.unsafe
					Dim k As  [Class] = GetType(Node)
					itemOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("item"))
					nextOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("next"))
					waiterOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("waiter"))
				Catch e As Exception
					Throw New [Error](e)
				End Try
			End Sub
		End Class

		''' <summary>
		''' head of the queue; null until first enqueue </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Friend head As Node

		''' <summary>
		''' tail of the queue; null until first append </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private tail As Node

		''' <summary>
		''' The number of apparent failures to unsplice removed nodes </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private sweepVotes As Integer

		' CAS methods for fields
		Private Function casTail(ByVal cmp As Node, ByVal val As Node) As Boolean
			Return UNSAFE.compareAndSwapObject(Me, tailOffset, cmp, val)
		End Function

		Private Function casHead(ByVal cmp As Node, ByVal val As Node) As Boolean
			Return UNSAFE.compareAndSwapObject(Me, headOffset, cmp, val)
		End Function

		Private Function casSweepVotes(ByVal cmp As Integer, ByVal val As Integer) As Boolean
			Return UNSAFE.compareAndSwapInt(Me, sweepVotesOffset, cmp, val)
		End Function

	'    
	'     * Possible values for "how" argument in xfer method.
	'     
		Private Const NOW As Integer = 0 ' for untimed poll, tryTransfer
		Private Const [ASYNC] As Integer = 1 ' for offer, put, add
		Private Const SYNC As Integer = 2 ' for transfer, take
		Private Const TIMED As Integer = 3 ' for timed poll, tryTransfer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function cast(Of E)(ByVal item As Object) As E
			' assert item == null || item.getClass() != Node.class;
			Return CType(item, E)
		End Function

		''' <summary>
		''' Implements all queuing methods. See above for explanation.
		''' </summary>
		''' <param name="e"> the item or null for take </param>
		''' <param name="haveData"> true if this is a put, else a take </param>
		''' <param name="how"> NOW, ASYNC, SYNC, or TIMED </param>
		''' <param name="nanos"> timeout in nanosecs, used only if mode is TIMED </param>
		''' <returns> an item if matched, else e </returns>
		''' <exception cref="NullPointerException"> if haveData mode but e is null </exception>
		Private Function xfer(ByVal e As E, ByVal haveData As Boolean, ByVal how As Integer, ByVal nanos As Long) As E
			If haveData AndAlso (e Is Nothing) Then Throw New NullPointerException
			Dim s As Node = Nothing ' the node to append, if needed

			retry:
			Do ' restart on append race

				Dim h As Node = head
				Dim p As Node = h
				Do While p IsNot Nothing ' find & match first node
					Dim isData As Boolean = p.isData
					Dim item As Object = p.item
					If item IsNot p AndAlso (item IsNot Nothing) = isData Then ' unmatched
						If isData = haveData Then ' can't match Exit Do
						If p.casItem(item, e) Then ' match
							Dim q As Node = p
							Do While q IsNot h
								Dim n As Node = q.next ' update by 2 unless singleton
								If head Is h AndAlso casHead(h,If(n Is Nothing, q, n)) Then
									h.forgetNext()
									Exit Do
								End If ' advance and retry
								h = head
								q = h.next
								If h Is Nothing OrElse q Is Nothing OrElse (Not q.matched) Then Exit Do ' unless slack < 2
							Loop
							java.util.concurrent.locks.LockSupport.unpark(p.waiter)
							Return LinkedTransferQueue.cast(Of E)(item)
						End If
					End If
					Dim n As Node = p.next
						If p IsNot n Then
							p = n
						Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							p = (h = head)
						End If
				Loop

				If how <> NOW Then ' No matches available
					If s Is Nothing Then s = New Node(e, haveData)
					Dim pred As Node = tryAppend(s, haveData)
					If pred Is Nothing Then GoTo retry ' lost race vs opposite mode
					If how <> [ASYNC] Then Return awaitMatch(s, pred, e, (how = TIMED), nanos)
				End If
				Return e ' not waiting
			Loop
		End Function

		''' <summary>
		''' Tries to append node s as tail.
		''' </summary>
		''' <param name="s"> the node to append </param>
		''' <param name="haveData"> true if appending in data mode </param>
		''' <returns> null on failure due to losing race with append in
		''' different mode, else s's predecessor, or s itself if no
		''' predecessor </returns>
		Private Function tryAppend(ByVal s As Node, ByVal haveData As Boolean) As Node
			Dim t As Node = tail
			Dim p As Node = t
			Do ' move p to last node and append
				Dim n, u As Node ' temps for reads of next & tail
				p = head
				If p Is Nothing AndAlso p Is Nothing Then
					If casHead(Nothing, s) Then Return s ' initialize
				ElseIf p.cannotPrecede(haveData) Then
					Return Nothing ' lost race vs opposite mode
				Else
					n = p.next
					If n IsNot Nothing Then ' not last; keep traversing
							u = tail
							If p IsNot t AndAlso t IsNot u Then
									t = u
									p = t
							Else
								p = If(p IsNot n, n, Nothing)
							End If
					ElseIf Not p.casNext(Nothing, s) Then
						p = p.next ' re-read on CAS failure
					Else
						If p IsNot t Then ' update if slack now >= 2
					End If
						t = tail
						s = t.next
						s = s.next
						Do While (tail IsNot t OrElse (Not casTail(t, s))) AndAlso t IsNot Nothing AndAlso s IsNot Nothing AndAlso s IsNot Nothing AndAlso s IsNot t ' advance and retry

							t = tail
							s = t.next
							s = s.next
						Loop
						End If
					Return p
					End If
			Loop
		End Function

		''' <summary>
		''' Spins/yields/blocks until node s is matched or caller gives up.
		''' </summary>
		''' <param name="s"> the waiting node </param>
		''' <param name="pred"> the predecessor of s, or s itself if it has no
		''' predecessor, or null if unknown (the null case does not occur
		''' in any current calls but may in possible future extensions) </param>
		''' <param name="e"> the comparison value for checking match </param>
		''' <param name="timed"> if true, wait only until timeout elapses </param>
		''' <param name="nanos"> timeout in nanosecs, used only if timed is true </param>
		''' <returns> matched item, or e if unmatched on interrupt or timeout </returns>
		Private Function awaitMatch(ByVal s As Node, ByVal pred As Node, ByVal e As E, ByVal timed As Boolean, ByVal nanos As Long) As E
			Dim deadline As Long = If(timed, System.nanoTime() + nanos, 0L)
			Dim w As Thread = Thread.CurrentThread
			Dim spins As Integer = -1 ' initialized after first item and cancel checks
			Dim randomYields As ThreadLocalRandom = Nothing ' bound if needed

			Do
				Dim item As Object = s.item
				If item IsNot e Then ' matched
					' assert item != s;
					s.forgetContents() ' avoid garbage
					Return LinkedTransferQueue.cast(Of E)(item)
				End If
				If (w.interrupted OrElse (timed AndAlso nanos <= 0)) AndAlso s.casItem(e, s) Then ' cancel
					unsplice(pred, s)
					Return e
				End If

				If spins < 0 Then ' establish spins at/near front
					spins = spinsFor(pred, s.isData)
					If spins > 0 Then randomYields = ThreadLocalRandom.current()
				ElseIf spins > 0 Then ' spin
					spins -= 1
					If randomYields.Next(CHAINED_SPINS) = 0 Then Thread.yield() ' occasionally yield
				ElseIf s.waiter Is Nothing Then
					s.waiter = w ' request unpark then recheck
				ElseIf timed Then
					nanos = deadline - System.nanoTime()
					If nanos > 0L Then java.util.concurrent.locks.LockSupport.parkNanos(Me, nanos)
				Else
					java.util.concurrent.locks.LockSupport.park(Me)
				End If
			Loop
		End Function

		''' <summary>
		''' Returns spin/yield value for a node with given predecessor and
		''' data mode. See above for explanation.
		''' </summary>
		Private Shared Function spinsFor(ByVal pred As Node, ByVal haveData As Boolean) As Integer
			If MP AndAlso pred IsNot Nothing Then
				If pred.isData <> haveData Then ' phase change Return FRONT_SPINS + CHAINED_SPINS
				If pred.matched Then ' probably at front Return FRONT_SPINS
				If pred.waiter Is Nothing Then ' pred apparently spinning Return CHAINED_SPINS
			End If
			Return 0
		End Function

		' -------------- Traversal methods -------------- 

		''' <summary>
		''' Returns the successor of p, or the head node if p.next has been
		''' linked to self, which will only be true if traversing with a
		''' stale pointer that is now off the list.
		''' </summary>
		Friend Function succ(ByVal p As Node) As Node
			Dim [next] As Node = p.next
			Return If(p Is [next], head, [next])
		End Function

		''' <summary>
		''' Returns the first unmatched node of the given mode, or null if
		''' none.  Used by methods isEmpty, hasWaitingConsumer.
		''' </summary>
		Private Function firstOfMode(ByVal isData As Boolean) As Node
			Dim p As Node = head
			Do While p IsNot Nothing
				If Not p.matched Then Return If(p.isData = isData, p, Nothing)
				p = succ(p)
			Loop
			Return Nothing
		End Function

		''' <summary>
		''' Version of firstOfMode used by Spliterator. Callers must
		''' recheck if the returned node's item field is null or
		''' self-linked before using.
		''' </summary>
		Friend Function firstDataNode() As Node
			Dim p As Node = head
			Do While p IsNot Nothing
				Dim item As Object = p.item
				If p.isData Then
					If item IsNot Nothing AndAlso item IsNot p Then Return p
				ElseIf item Is Nothing Then
					Exit Do
				End If
				p = p.next
				If p Is p Then p = head
			Loop
			Return Nothing
		End Function

		''' <summary>
		''' Returns the item in the first unmatched node with isData; or
		''' null if none.  Used by peek.
		''' </summary>
		Private Function firstDataItem() As E
			Dim p As Node = head
			Do While p IsNot Nothing
				Dim item As Object = p.item
				If p.isData Then
					If item IsNot Nothing AndAlso item IsNot p Then Return LinkedTransferQueue.cast(Of E)(item)
				ElseIf item Is Nothing Then
					Return Nothing
				End If
				p = succ(p)
			Loop
			Return Nothing
		End Function

		''' <summary>
		''' Traverses and counts unmatched nodes of the given mode.
		''' Used by methods size and getWaitingConsumerCount.
		''' </summary>
		Private Function countOfMode(ByVal data As Boolean) As Integer
			Dim count As Integer = 0
			Dim p As Node = head
			Do While p IsNot Nothing
				If Not p.matched Then
					If p.isData <> data Then Return 0
					count += 1
					If count = Integer.MaxValue Then ' saturated Exit Do
				End If
				Dim n As Node = p.next
				If n IsNot p Then
					p = n
				Else
					count = 0
					p = head
				End If
			Loop
			Return count
		End Function

		Friend NotInheritable Class Itr
			Implements IEnumerator(Of E)

			Private ReadOnly outerInstance As LinkedTransferQueue

			Private nextNode As Node ' next node to return item for
			Private nextItem As E ' the corresponding item
			Private lastRet As Node ' last returned node, to support remove
			Private lastPred As Node ' predecessor to unlink lastRet

			''' <summary>
			''' Moves to next node after prev, or first node if prev null.
			''' </summary>
			Private Sub advance(ByVal prev As Node)
	'            
	'             * To track and avoid buildup of deleted nodes in the face
	'             * of calls to both Queue.remove and Itr.remove, we must
	'             * include variants of unsplice and sweep upon each
	'             * advance: Upon Itr.remove, we may need to catch up links
	'             * from lastPred, and upon other removes, we might need to
	'             * skip ahead from stale nodes and unsplice deleted ones
	'             * found while advancing.
	'             

				Dim r, b As Node ' reset lastPred upon possible deletion of lastRet
				r = lastRet
				If r IsNot Nothing AndAlso (Not r.matched) Then
					lastPred = r ' next lastPred is old lastRet
				Else
					b = lastPred
					If b Is Nothing OrElse b.matched Then
						lastPred = Nothing ' at start of list
					Else
						Dim s, n As Node ' help with removal of lastPred.next
					End If
					s = b.next
					n = s.next
					Do While s IsNot Nothing AndAlso s IsNot b AndAlso s.matched AndAlso n IsNot Nothing AndAlso n IsNot s
						b.casNext(s, n)
						s = b.next
						n = s.next
					Loop
					End If

				Me.lastRet = prev

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				for (Node p = prev, s, n;;)
					s = If(p Is Nothing, outerInstance.head, p.next)
					If s Is Nothing Then
						break
					ElseIf s Is p Then
						p = Nothing
						continue
					End If
					Dim item As Object = s.item
					If s.isData Then
						If item IsNot Nothing AndAlso item IsNot s Then
							nextItem = LinkedTransferQueue.cast(Of E)(item)
							nextNode = s
							Return
						End If
					ElseIf item Is Nothing Then
						break
					End If
					' assert s.isMatched();
					If p Is Nothing Then
						p = s
					Else
						n = s.next
						If n Is Nothing Then
							break
						ElseIf s = n Then
							p = Nothing
						Else
							p.casNext(s, n)
						End If
						End If
				nextNode = Nothing
				nextItem = Nothing
			End Sub

			Friend Sub New(ByVal outerInstance As LinkedTransferQueue)
					Me.outerInstance = outerInstance
				advance(Nothing)
			End Sub

			Public Function hasNext() As Boolean
				Return nextNode IsNot Nothing
			End Function

			Public Function [next]() As E
				Dim p As Node = nextNode
				If p Is Nothing Then Throw New java.util.NoSuchElementException
				Dim e As E = nextItem
				advance(p)
				Return e
			End Function

			Public Sub remove()
				Dim lastRet As Node = Me.lastRet
				If lastRet Is Nothing Then Throw New IllegalStateException
				Me.lastRet = Nothing
				If lastRet.tryMatchData() Then outerInstance.unsplice(lastPred, lastRet)
			End Sub
		End Class

		''' <summary>
		''' A customized variant of Spliterators.IteratorSpliterator </summary>
		Friend NotInheritable Class LTQSpliterator(Of E)
			Implements java.util.Spliterator(Of E)

			Friend Shared ReadOnly MAX_BATCH As Integer = 1 << 25 ' max batch array size;
			Friend ReadOnly queue As LinkedTransferQueue(Of E)
			Friend current As Node ' current node; null until initialized
			Friend batch As Integer ' batch size for splits
			Friend exhausted As Boolean ' true when no more nodes
			Friend Sub New(ByVal queue As LinkedTransferQueue(Of E))
				Me.queue = queue
			End Sub

			Public Function trySplit() As java.util.Spliterator(Of E)
				Dim p As Node
				Dim q As LinkedTransferQueue(Of E) = Me.queue
				Dim b As Integer = batch
				Dim n As Integer = If(b <= 0, 1, If(b >= MAX_BATCH, MAX_BATCH, b + 1))
				p = current
				p = q.firstDataNode()
				If (Not exhausted) AndAlso (p IsNot Nothing OrElse p IsNot Nothing) AndAlso p.next IsNot Nothing Then
					Dim a As Object() = New Object(n - 1){}
					Dim i As Integer = 0
					Do
						Dim e As Object = p.item
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						If e IsNot p AndAlso (a(i) = e) IsNot Nothing Then i += 1
						p = p.next
						If p Is p Then p = q.firstDataNode()
					Loop While p IsNot Nothing AndAlso i < n AndAlso p.isData
					current = p
					If current Is Nothing Then exhausted = True
					If i > 0 Then
						batch = i
						Return java.util.Spliterators.spliterator(a, 0, i, java.util.Spliterator.ORDERED Or java.util.Spliterator.NONNULL Or java.util.Spliterator.CONCURRENT)
					End If
				End If
				Return Nothing
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				Dim p As Node
				If action Is Nothing Then Throw New NullPointerException
				Dim q As LinkedTransferQueue(Of E) = Me.queue
				p = current
				p = q.firstDataNode()
				If (Not exhausted) AndAlso (p IsNot Nothing OrElse p IsNot Nothing) Then
					exhausted = True
					Do
						Dim e As Object = p.item
						If e IsNot Nothing AndAlso e IsNot p Then action.accept(CType(e, E))
						p = p.next
						If p Is p Then p = q.firstDataNode()
					Loop While p IsNot Nothing AndAlso p.isData
				End If
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean
				Dim p As Node
				If action Is Nothing Then Throw New NullPointerException
				Dim q As LinkedTransferQueue(Of E) = Me.queue
				p = current
				p = q.firstDataNode()
				If (Not exhausted) AndAlso (p IsNot Nothing OrElse p IsNot Nothing) Then
					Dim e As Object
					Do
						e = p.item
						If e Is p Then e = Nothing
						p = p.next
						If p Is p Then p = q.firstDataNode()
					Loop While e Is Nothing AndAlso p IsNot Nothing AndAlso p.isData
					current = p
					If current Is Nothing Then exhausted = True
					If e IsNot Nothing Then
						action.accept(CType(e, E))
						Return True
					End If
				End If
				Return False
			End Function

			Public Function estimateSize() As Long
				Return Long.MaxValue
			End Function

			Public Function characteristics() As Integer
				Return java.util.Spliterator.ORDERED Or java.util.Spliterator.NONNULL Or java.util.Spliterator.CONCURRENT
			End Function
		End Class

		''' <summary>
		''' Returns a <seealso cref="Spliterator"/> over the elements in this queue.
		''' 
		''' <p>The returned spliterator is
		''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		''' 
		''' <p>The {@code Spliterator} reports <seealso cref="Spliterator#CONCURRENT"/>,
		''' <seealso cref="Spliterator#ORDERED"/>, and <seealso cref="Spliterator#NONNULL"/>.
		''' 
		''' @implNote
		''' The {@code Spliterator} implements {@code trySplit} to permit limited
		''' parallelism.
		''' </summary>
		''' <returns> a {@code Spliterator} over the elements in this queue
		''' @since 1.8 </returns>
		Public Overridable Function spliterator() As java.util.Spliterator(Of E)
			Return New LTQSpliterator(Of E)(Me)
		End Function

		' -------------- Removal methods -------------- 

		''' <summary>
		''' Unsplices (now or later) the given deleted/cancelled node with
		''' the given predecessor.
		''' </summary>
		''' <param name="pred"> a node that was at one time known to be the
		''' predecessor of s, or null or s itself if s is/was at head </param>
		''' <param name="s"> the node to be unspliced </param>
		Friend Sub unsplice(ByVal pred As Node, ByVal s As Node)
			s.forgetContents() ' forget unneeded fields
	'        
	'         * See above for rationale. Briefly: if pred still points to
	'         * s, try to unlink s.  If s cannot be unlinked, because it is
	'         * trailing node or pred might be unlinked, and neither pred
	'         * nor s are head or offlist, add to sweepVotes, and if enough
	'         * votes have accumulated, sweep.
	'         
			If pred IsNot Nothing AndAlso pred IsNot s AndAlso pred.next Is s Then
				Dim n As Node = s.next
				If n Is Nothing OrElse (n IsNot s AndAlso pred.casNext(s, n) AndAlso pred.matched) Then
					Do ' check if at, or could be, head
						Dim h As Node = head
						If h Is pred OrElse h Is s OrElse h Is Nothing Then Return ' at head or list empty
						If Not h.matched Then Exit Do
						Dim hn As Node = h.next
						If hn Is Nothing Then Return ' now empty
						If hn IsNot h AndAlso casHead(h, hn) Then h.forgetNext() ' advance head
					Loop
					If pred.next IsNot pred AndAlso s.next IsNot s Then ' recheck if offlist
						Do ' sweep now if enough votes
							Dim v As Integer = sweepVotes
							If v < SWEEP_THRESHOLD Then
								If casSweepVotes(v, v + 1) Then Exit Do
							ElseIf casSweepVotes(v, 0) Then
								sweep()
								Exit Do
							End If
						Loop
					End If
				End If
			End If
		End Sub

		''' <summary>
		''' Unlinks matched (typically cancelled) nodes encountered in a
		''' traversal from head.
		''' </summary>
		Private Sub sweep()
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			for (Node p = head, s, n; p != Nothing && (s = p.next) != Nothing;)
				If Not s.matched Then
					' Unmatched nodes are never self-linked
					p = s
				Else
					n = s.next
					If n Is Nothing Then ' trailing node is pinned
						break
					ElseIf s = n Then ' stale
						' No need to also check for p == s, since that implies s == n
						p = head
					Else
						p.casNext(s, n)
					End If
					End If
		End Sub

		''' <summary>
		''' Main implementation of remove(Object)
		''' </summary>
		Private Function findAndRemove(ByVal e As Object) As Boolean
			If e IsNot Nothing Then
				Dim pred As Node = Nothing
				Dim p As Node = head
				Do While p IsNot Nothing
					Dim item As Object = p.item
					If p.isData Then
						If item IsNot Nothing AndAlso item IsNot p AndAlso e.Equals(item) AndAlso p.tryMatchData() Then
							unsplice(pred, p)
							Return True
						End If
					ElseIf item Is Nothing Then
						Exit Do
					End If
					pred = p
					p = p.next
					If p Is pred Then ' stale
						pred = Nothing
						p = head
					End If
				Loop
			End If
			Return False
		End Function

		''' <summary>
		''' Creates an initially empty {@code LinkedTransferQueue}.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Creates a {@code LinkedTransferQueue}
		''' initially containing the elements of the given collection,
		''' added in traversal order of the collection's iterator.
		''' </summary>
		''' <param name="c"> the collection of elements to initially contain </param>
		''' <exception cref="NullPointerException"> if the specified collection or any
		'''         of its elements are null </exception>
		Public Sub New(Of T1 As E)(ByVal c As ICollection(Of T1))
			Me.New()
			addAll(c)
		End Sub

		''' <summary>
		''' Inserts the specified element at the tail of this queue.
		''' As the queue is unbounded, this method will never block.
		''' </summary>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Sub put(ByVal e As E)
			xfer(e, True, [ASYNC], 0)
		End Sub

		''' <summary>
		''' Inserts the specified element at the tail of this queue.
		''' As the queue is unbounded, this method will never block or
		''' return {@code false}.
		''' </summary>
		''' <returns> {@code true} (as specified by
		'''  {@link java.util.concurrent.BlockingQueue#offer(Object,long,TimeUnit)
		'''  BlockingQueue.offer}) </returns>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function offer(ByVal e As E, ByVal timeout As Long, ByVal unit As java.util.concurrent.TimeUnit) As Boolean
			xfer(e, True, [ASYNC], 0)
			Return True
		End Function

		''' <summary>
		''' Inserts the specified element at the tail of this queue.
		''' As the queue is unbounded, this method will never return {@code false}.
		''' </summary>
		''' <returns> {@code true} (as specified by <seealso cref="Queue#offer"/>) </returns>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function offer(ByVal e As E) As Boolean
			xfer(e, True, [ASYNC], 0)
			Return True
		End Function

		''' <summary>
		''' Inserts the specified element at the tail of this queue.
		''' As the queue is unbounded, this method will never throw
		''' <seealso cref="IllegalStateException"/> or return {@code false}.
		''' </summary>
		''' <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function add(ByVal e As E) As Boolean
			xfer(e, True, [ASYNC], 0)
			Return True
		End Function

		''' <summary>
		''' Transfers the element to a waiting consumer immediately, if possible.
		''' 
		''' <p>More precisely, transfers the specified element immediately
		''' if there exists a consumer already waiting to receive it (in
		''' <seealso cref="#take"/> or timed <seealso cref="#poll(long,TimeUnit) poll"/>),
		''' otherwise returning {@code false} without enqueuing the element.
		''' </summary>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function tryTransfer(ByVal e As E) As Boolean
			Return xfer(e, True, NOW, 0) Is Nothing
		End Function

		''' <summary>
		''' Transfers the element to a consumer, waiting if necessary to do so.
		''' 
		''' <p>More precisely, transfers the specified element immediately
		''' if there exists a consumer already waiting to receive it (in
		''' <seealso cref="#take"/> or timed <seealso cref="#poll(long,TimeUnit) poll"/>),
		''' else inserts the specified element at the tail of this queue
		''' and waits until the element is received by a consumer.
		''' </summary>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Sub transfer(ByVal e As E)
			If xfer(e, True, SYNC, 0) IsNot Nothing Then
				Thread.interrupted() ' failure possible only due to interrupt
				Throw New InterruptedException
			End If
		End Sub

		''' <summary>
		''' Transfers the element to a consumer if it is possible to do so
		''' before the timeout elapses.
		''' 
		''' <p>More precisely, transfers the specified element immediately
		''' if there exists a consumer already waiting to receive it (in
		''' <seealso cref="#take"/> or timed <seealso cref="#poll(long,TimeUnit) poll"/>),
		''' else inserts the specified element at the tail of this queue
		''' and waits until the element is received by a consumer,
		''' returning {@code false} if the specified wait time elapses
		''' before the element can be transferred.
		''' </summary>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function tryTransfer(ByVal e As E, ByVal timeout As Long, ByVal unit As java.util.concurrent.TimeUnit) As Boolean
			If xfer(e, True, TIMED, unit.toNanos(timeout)) Is Nothing Then Return True
			If Not Thread.interrupted() Then Return False
			Throw New InterruptedException
		End Function

		Public Overridable Function take() As E
			Dim e As E = xfer(Nothing, False, SYNC, 0)
			If e IsNot Nothing Then Return e
			Thread.interrupted()
			Throw New InterruptedException
		End Function

		Public Overridable Function poll(ByVal timeout As Long, ByVal unit As java.util.concurrent.TimeUnit) As E
			Dim e As E = xfer(Nothing, False, TIMED, unit.toNanos(timeout))
			If e IsNot Nothing OrElse (Not Thread.interrupted()) Then Return e
			Throw New InterruptedException
		End Function

		Public Overridable Function poll() As E
			Return xfer(Nothing, False, NOW, 0)
		End Function

		''' <exception cref="NullPointerException">     {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function drainTo(Of T1)(ByVal c As ICollection(Of T1)) As Integer
			If c Is Nothing Then Throw New NullPointerException
			If c Is Me Then Throw New IllegalArgumentException
			Dim n As Integer = 0
			Dim e As E
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While (e = poll()) IsNot Nothing
				c.Add(e)
				n += 1
			Loop
			Return n
		End Function

		''' <exception cref="NullPointerException">     {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function drainTo(Of T1)(ByVal c As ICollection(Of T1), ByVal maxElements As Integer) As Integer
			If c Is Nothing Then Throw New NullPointerException
			If c Is Me Then Throw New IllegalArgumentException
			Dim n As Integer = 0
			Dim e As E
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While n < maxElements AndAlso (e = poll()) IsNot Nothing
				c.Add(e)
				n += 1
			Loop
			Return n
		End Function

		''' <summary>
		''' Returns an iterator over the elements in this queue in proper sequence.
		''' The elements will be returned in order from first (head) to last (tail).
		''' 
		''' <p>The returned iterator is
		''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		''' </summary>
		''' <returns> an iterator over the elements in this queue in proper sequence </returns>
		Public Overridable Function [iterator]() As IEnumerator(Of E)
			Return New Itr(Me)
		End Function

		Public Overridable Function peek() As E
			Return firstDataItem()
		End Function

		''' <summary>
		''' Returns {@code true} if this queue contains no elements.
		''' </summary>
		''' <returns> {@code true} if this queue contains no elements </returns>
		Public Overridable Property empty As Boolean
			Get
				Dim p As Node = head
				Do While p IsNot Nothing
					If Not p.matched Then Return Not p.isData
					p = succ(p)
				Loop
				Return True
			End Get
		End Property

		Public Overridable Function hasWaitingConsumer() As Boolean Implements TransferQueue(Of E).hasWaitingConsumer
			Return firstOfMode(False) IsNot Nothing
		End Function

		''' <summary>
		''' Returns the number of elements in this queue.  If this queue
		''' contains more than {@code Integer.MAX_VALUE} elements, returns
		''' {@code Integer.MAX_VALUE}.
		''' 
		''' <p>Beware that, unlike in most collections, this method is
		''' <em>NOT</em> a constant-time operation. Because of the
		''' asynchronous nature of these queues, determining the current
		''' number of elements requires an O(n) traversal.
		''' </summary>
		''' <returns> the number of elements in this queue </returns>
		Public Overridable Function size() As Integer
			Return countOfMode(True)
		End Function

		Public Overridable Property waitingConsumerCount As Integer Implements TransferQueue(Of E).getWaitingConsumerCount
			Get
				Return countOfMode(False)
			End Get
		End Property

		''' <summary>
		''' Removes a single instance of the specified element from this queue,
		''' if it is present.  More formally, removes an element {@code e} such
		''' that {@code o.equals(e)}, if this queue contains one or more such
		''' elements.
		''' Returns {@code true} if this queue contained the specified element
		''' (or equivalently, if this queue changed as a result of the call).
		''' </summary>
		''' <param name="o"> element to be removed from this queue, if present </param>
		''' <returns> {@code true} if this queue changed as a result of the call </returns>
		Public Overridable Function remove(ByVal o As Object) As Boolean
			Return findAndRemove(o)
		End Function

		''' <summary>
		''' Returns {@code true} if this queue contains the specified element.
		''' More formally, returns {@code true} if and only if this queue contains
		''' at least one element {@code e} such that {@code o.equals(e)}.
		''' </summary>
		''' <param name="o"> object to be checked for containment in this queue </param>
		''' <returns> {@code true} if this queue contains the specified element </returns>
		Public Overridable Function contains(ByVal o As Object) As Boolean
			If o Is Nothing Then Return False
			Dim p As Node = head
			Do While p IsNot Nothing
				Dim item As Object = p.item
				If p.isData Then
					If item IsNot Nothing AndAlso item IsNot p AndAlso o.Equals(item) Then Return True
				ElseIf item Is Nothing Then
					Exit Do
				End If
				p = succ(p)
			Loop
			Return False
		End Function

		''' <summary>
		''' Always returns {@code Integer.MAX_VALUE} because a
		''' {@code LinkedTransferQueue} is not capacity constrained.
		''' </summary>
		''' <returns> {@code Integer.MAX_VALUE} (as specified by
		'''         {@link java.util.concurrent.BlockingQueue#remainingCapacity()
		'''         BlockingQueue.remainingCapacity}) </returns>
		Public Overridable Function remainingCapacity() As Integer
			Return Integer.MaxValue
		End Function

		''' <summary>
		''' Saves this queue to a stream (that is, serializes it).
		''' </summary>
		''' <param name="s"> the stream </param>
		''' <exception cref="java.io.IOException"> if an I/O error occurs
		''' @serialData All of the elements (each an {@code E}) in
		''' the proper order, followed by a null </exception>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()
			For Each e As E In Me
				s.writeObject(e)
			Next e
			' Use trailing null as sentinel
			s.writeObject(Nothing)
		End Sub

		''' <summary>
		''' Reconstitutes this queue from a stream (that is, deserializes it). </summary>
		''' <param name="s"> the stream </param>
		''' <exception cref="ClassNotFoundException"> if the class of a serialized object
		'''         could not be found </exception>
		''' <exception cref="java.io.IOException"> if an I/O error occurs </exception>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			Do
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim item As E = CType(s.readObject(), E)
				If item Is Nothing Then
					Exit Do
				Else
					offer(item)
				End If
			Loop
		End Sub

		' Unsafe mechanics

		Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
		Private Shared ReadOnly headOffset As Long
		Private Shared ReadOnly tailOffset As Long
		Private Shared ReadOnly sweepVotesOffset As Long
		Shared Sub New()
			Try
				UNSAFE = sun.misc.Unsafe.unsafe
				Dim k As  [Class] = GetType(LinkedTransferQueue)
				headOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("head"))
				tailOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("tail"))
				sweepVotesOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("sweepVotes"))
			Catch e As Exception
				Throw New [Error](e)
			End Try
		End Sub
	End Class

End Namespace
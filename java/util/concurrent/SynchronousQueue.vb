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
' * Written by Doug Lea, Bill Scherer, and Michael Scott with
' * assistance from members of JCP JSR-166 Expert Group and released to
' * the public domain, as explained at
' * http://creativecommons.org/publicdomain/zero/1.0/
' 

Namespace java.util.concurrent

	''' <summary>
	''' A <seealso cref="BlockingQueue blocking queue"/> in which each insert
	''' operation must wait for a corresponding remove operation by another
	''' thread, and vice versa.  A synchronous queue does not have any
	''' internal capacity, not even a capacity of one.  You cannot
	''' {@code peek} at a synchronous queue because an element is only
	''' present when you try to remove it; you cannot insert an element
	''' (using any method) unless another thread is trying to remove it;
	''' you cannot iterate as there is nothing to iterate.  The
	''' <em>head</em> of the queue is the element that the first queued
	''' inserting thread is trying to add to the queue; if there is no such
	''' queued thread then no element is available for removal and
	''' {@code poll()} will return {@code null}.  For purposes of other
	''' {@code Collection} methods (for example {@code contains}), a
	''' {@code SynchronousQueue} acts as an empty collection.  This queue
	''' does not permit {@code null} elements.
	''' 
	''' <p>Synchronous queues are similar to rendezvous channels used in
	''' CSP and Ada. They are well suited for handoff designs, in which an
	''' object running in one thread must sync up with an object running
	''' in another thread in order to hand it some information, event, or
	''' task.
	''' 
	''' <p>This class supports an optional fairness policy for ordering
	''' waiting producer and consumer threads.  By default, this ordering
	''' is not guaranteed. However, a queue constructed with fairness set
	''' to {@code true} grants threads access in FIFO order.
	''' 
	''' <p>This class and its iterator implement all of the
	''' <em>optional</em> methods of the <seealso cref="Collection"/> and {@link
	''' Iterator} interfaces.
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @since 1.5
	''' @author Doug Lea and Bill Scherer and Michael Scott </summary>
	''' @param <E> the type of elements held in this collection </param>
	<Serializable> _
	Public Class SynchronousQueue(Of E)
		Inherits AbstractQueue(Of E)
		Implements BlockingQueue(Of E)

		Private Const serialVersionUID As Long = -3223113410248163686L

	'    
	'     * This class implements extensions of the dual stack and dual
	'     * queue algorithms described in "Nonblocking Concurrent Objects
	'     * with Condition Synchronization", by W. N. Scherer III and
	'     * M. L. Scott.  18th Annual Conf. on Distributed Computing,
	'     * Oct. 2004 (see also
	'     * http://www.cs.rochester.edu/u/scott/synchronization/pseudocode/duals.html).
	'     * The (Lifo) stack is used for non-fair mode, and the (Fifo)
	'     * queue for fair mode. The performance of the two is generally
	'     * similar. Fifo usually supports higher throughput under
	'     * contention but Lifo maintains higher thread locality in common
	'     * applications.
	'     *
	'     * A dual queue (and similarly stack) is one that at any given
	'     * time either holds "data" -- items provided by put operations,
	'     * or "requests" -- slots representing take operations, or is
	'     * empty. A call to "fulfill" (i.e., a call requesting an item
	'     * from a queue holding data or vice versa) dequeues a
	'     * complementary node.  The most interesting feature of these
	'     * queues is that any operation can figure out which mode the
	'     * queue is in, and act accordingly without needing locks.
	'     *
	'     * Both the queue and stack extend abstract class Transferer
	'     * defining the single method transfer that does a put or a
	'     * take. These are unified into a single method because in dual
	'     * data structures, the put and take operations are symmetrical,
	'     * so nearly all code can be combined. The resulting transfer
	'     * methods are on the long side, but are easier to follow than
	'     * they would be if broken up into nearly-duplicated parts.
	'     *
	'     * The queue and stack data structures share many conceptual
	'     * similarities but very few concrete details. For simplicity,
	'     * they are kept distinct so that they can later evolve
	'     * separately.
	'     *
	'     * The algorithms here differ from the versions in the above paper
	'     * in extending them for use in synchronous queues, as well as
	'     * dealing with cancellation. The main differences include:
	'     *
	'     *  1. The original algorithms used bit-marked pointers, but
	'     *     the ones here use mode bits in nodes, leading to a number
	'     *     of further adaptations.
	'     *  2. SynchronousQueues must block threads waiting to become
	'     *     fulfilled.
	'     *  3. Support for cancellation via timeout and interrupts,
	'     *     including cleaning out cancelled nodes/threads
	'     *     from lists to avoid garbage retention and memory depletion.
	'     *
	'     * Blocking is mainly accomplished using LockSupport park/unpark,
	'     * except that nodes that appear to be the next ones to become
	'     * fulfilled first spin a bit (on multiprocessors only). On very
	'     * busy synchronous queues, spinning can dramatically improve
	'     * throughput. And on less busy ones, the amount of spinning is
	'     * small enough not to be noticeable.
	'     *
	'     * Cleaning is done in different ways in queues vs stacks.  For
	'     * queues, we can almost always remove a node immediately in O(1)
	'     * time (modulo retries for consistency checks) when it is
	'     * cancelled. But if it may be pinned as the current tail, it must
	'     * wait until some subsequent cancellation. For stacks, we need a
	'     * potentially O(n) traversal to be sure that we can remove the
	'     * node, but this can run concurrently with other threads
	'     * accessing the stack.
	'     *
	'     * While garbage collection takes care of most node reclamation
	'     * issues that otherwise complicate nonblocking algorithms, care
	'     * is taken to "forget" references to data, other nodes, and
	'     * threads that might be held on to long-term by blocked
	'     * threads. In cases where setting to null would otherwise
	'     * conflict with main algorithms, this is done by changing a
	'     * node's link to now point to the node itself. This doesn't arise
	'     * much for Stack nodes (because blocked threads do not hang on to
	'     * old head pointers), but references in Queue nodes must be
	'     * aggressively forgotten to avoid reachability of everything any
	'     * node has ever referred to since arrival.
	'     

		''' <summary>
		''' Shared internal API for dual stacks and queues.
		''' </summary>
		Friend MustInherit Class Transferer(Of E)
			''' <summary>
			''' Performs a put or take.
			''' </summary>
			''' <param name="e"> if non-null, the item to be handed to a consumer;
			'''          if null, requests that transfer return an item
			'''          offered by producer. </param>
			''' <param name="timed"> if this operation should timeout </param>
			''' <param name="nanos"> the timeout, in nanoseconds </param>
			''' <returns> if non-null, the item provided or received; if null,
			'''         the operation failed due to timeout or interrupt --
			'''         the caller can distinguish which of these occurred
			'''         by checking Thread.interrupted. </returns>
			Friend MustOverride Function transfer(ByVal e As E, ByVal timed As Boolean, ByVal nanos As Long) As E
		End Class

		''' <summary>
		''' The number of CPUs, for spin control </summary>
		Friend Shared ReadOnly NCPUS As Integer = Runtime.runtime.availableProcessors()

		''' <summary>
		''' The number of times to spin before blocking in timed waits.
		''' The value is empirically derived -- it works well across a
		''' variety of processors and OSes. Empirically, the best value
		''' seems not to vary with number of CPUs (beyond 2) so is just
		''' a constant.
		''' </summary>
		Friend Shared ReadOnly maxTimedSpins As Integer = If(NCPUS < 2, 0, 32)

		''' <summary>
		''' The number of times to spin before blocking in untimed waits.
		''' This is greater than timed value because untimed waits spin
		''' faster since they don't need to check times on each spin.
		''' </summary>
		Friend Shared ReadOnly maxUntimedSpins As Integer = maxTimedSpins * 16

		''' <summary>
		''' The number of nanoseconds for which it is faster to spin
		''' rather than to use timed park. A rough estimate suffices.
		''' </summary>
		Friend Const spinForTimeoutThreshold As Long = 1000L

		''' <summary>
		''' Dual stack </summary>
		Friend NotInheritable Class TransferStack(Of E)
			Inherits Transferer(Of E)

	'        
	'         * This extends Scherer-Scott dual stack algorithm, differing,
	'         * among other ways, by using "covering" nodes rather than
	'         * bit-marked pointers: Fulfilling operations push on marker
	'         * nodes (with FULFILLING bit set in mode) to reserve a spot
	'         * to match a waiting node.
	'         

			' Modes for SNodes, ORed together in node fields 
			''' <summary>
			''' Node represents an unfulfilled consumer </summary>
			Friend Const REQUEST As Integer = 0
			''' <summary>
			''' Node represents an unfulfilled producer </summary>
			Friend Const DATA As Integer = 1
			''' <summary>
			''' Node is fulfilling another unfulfilled DATA or REQUEST </summary>
			Friend Const FULFILLING As Integer = 2

			''' <summary>
			''' Returns true if m has fulfilling bit set. </summary>
			Friend Shared Function isFulfilling(ByVal m As Integer) As Boolean
				Return (m And FULFILLING) <> 0
			End Function

			''' <summary>
			''' Node class for TransferStacks. </summary>
			Friend NotInheritable Class SNode
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
				Friend [next] As SNode ' next node in stack
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
				Friend match As SNode ' the node matched to this
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
				Friend waiter As Thread ' to control park/unpark
				Friend item As Object ' data; or null for REQUESTs
				Friend mode As Integer
				' Note: item and mode fields don't need to be volatile
				' since they are always written before, and read after,
				' other volatile/atomic operations.

				Friend Sub New(ByVal item As Object)
					Me.item = item
				End Sub

				Friend Function casNext(ByVal cmp As SNode, ByVal val As SNode) As Boolean
					Return cmp Is [next] AndAlso UNSAFE.compareAndSwapObject(Me, nextOffset, cmp, val)
				End Function

				''' <summary>
				''' Tries to match node s to this node, if so, waking up thread.
				''' Fulfillers call tryMatch to identify their waiters.
				''' Waiters block until they have been matched.
				''' </summary>
				''' <param name="s"> the node to match </param>
				''' <returns> true if successfully matched to s </returns>
				Friend Function tryMatch(ByVal s As SNode) As Boolean
					If match Is Nothing AndAlso UNSAFE.compareAndSwapObject(Me, matchOffset, Nothing, s) Then
						Dim w As Thread = waiter
						If w IsNot Nothing Then ' waiters need at most one unpark
							waiter = Nothing
							java.util.concurrent.locks.LockSupport.unpark(w)
						End If
						Return True
					End If
					Return match Is s
				End Function

				''' <summary>
				''' Tries to cancel a wait by matching node to itself.
				''' </summary>
				Friend Sub tryCancel()
					UNSAFE.compareAndSwapObject(Me, matchOffset, Nothing, Me)
				End Sub

				Friend Property cancelled As Boolean
					Get
						Return match Is Me
					End Get
				End Property

				' Unsafe mechanics
				Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
				Private Shared ReadOnly matchOffset As Long
				Private Shared ReadOnly nextOffset As Long

				Shared Sub New()
					Try
						UNSAFE = sun.misc.Unsafe.unsafe
						Dim k As  [Class] = GetType(SNode)
						matchOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("match"))
						nextOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("next"))
					Catch e As Exception
						Throw New [Error](e)
					End Try
				End Sub
			End Class

			''' <summary>
			''' The head (top) of the stack </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend head As SNode

			Friend Function casHead(ByVal h As SNode, ByVal nh As SNode) As Boolean
				Return h Is head AndAlso UNSAFE.compareAndSwapObject(Me, headOffset, h, nh)
			End Function

			''' <summary>
			''' Creates or resets fields of a node. Called only from transfer
			''' where the node to push on stack is lazily created and
			''' reused when possible to help reduce intervals between reads
			''' and CASes of head and to avoid surges of garbage when CASes
			''' to push nodes fail due to contention.
			''' </summary>
			Friend Shared Function snode(ByVal s As SNode, ByVal e As Object, ByVal [next] As SNode, ByVal mode As Integer) As SNode
				If s Is Nothing Then s = New SNode(e)
				s.mode = mode
				s.next = [next]
				Return s
			End Function

			''' <summary>
			''' Puts or takes an item.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend Function transfer(ByVal e As E, ByVal timed As Boolean, ByVal nanos As Long) As E
	'            
	'             * Basic algorithm is to loop trying one of three actions:
	'             *
	'             * 1. If apparently empty or already containing nodes of same
	'             *    mode, try to push node on stack and wait for a match,
	'             *    returning it, or null if cancelled.
	'             *
	'             * 2. If apparently containing node of complementary mode,
	'             *    try to push a fulfilling node on to stack, match
	'             *    with corresponding waiting node, pop both from
	'             *    stack, and return matched item. The matching or
	'             *    unlinking might not actually be necessary because of
	'             *    other threads performing action 3:
	'             *
	'             * 3. If top of stack already holds another fulfilling node,
	'             *    help it out by doing its match and/or pop
	'             *    operations, and then continue. The code for helping
	'             *    is essentially the same as for fulfilling, except
	'             *    that it doesn't return the item.
	'             

				Dim s As SNode = Nothing ' constructed/reused as needed
				Dim mode As Integer = If(e Is Nothing, REQUEST, DATA)

				Do
					Dim h As SNode = head
					If h Is Nothing OrElse h.mode = mode Then ' empty or same-mode
						If timed AndAlso nanos <= 0 Then ' can't wait
							If h IsNot Nothing AndAlso h.cancelled Then
								casHead(h, h.next) ' pop cancelled node
							Else
								Return Nothing
							End If
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						ElseIf casHead(h, s = snode(s, e, h, mode)) Then
							Dim m As SNode = awaitFulfill(s, timed, nanos)
							If m Is s Then ' wait was cancelled
								clean(s)
								Return Nothing
							End If
							h = head
							If h IsNot Nothing AndAlso h.next Is s Then casHead(h, s.next) ' help s's fulfiller
							Return CType(If(mode = REQUEST, m.item, s.item), E)
						End If ' try to fulfill
					ElseIf Not isFulfilling(h.mode) Then
						If h.cancelled Then ' already cancelled
							casHead(h, h.next) ' pop and retry
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						ElseIf casHead(h, s=snode(s, e, h, FULFILLING Or mode)) Then
							Do ' loop until matched or waiters disappear
								Dim m As SNode = s.next ' m is s's match
								If m Is Nothing Then ' all waiters are gone
									casHead(s, Nothing) ' pop fulfill node
									s = Nothing ' use new node next time
									Exit Do ' restart main loop
								End If
								Dim mn As SNode = m.next
								If m.tryMatch(s) Then
									casHead(s, mn) ' pop both s and m
									Return CType(If(mode = REQUEST, m.item, s.item), E) ' lost match
								Else
									s.casNext(m, mn) ' help unlink
								End If
							Loop
						End If ' help a fulfiller
					Else
						Dim m As SNode = h.next ' m is h's match
						If m Is Nothing Then ' waiter is gone
							casHead(h, Nothing) ' pop fulfilling node
						Else
							Dim mn As SNode = m.next
							If m.tryMatch(h) Then ' help match
								casHead(h, mn) ' pop both h and m
							Else ' lost match
								h.casNext(m, mn) ' help unlink
							End If
						End If
					End If
				Loop
			End Function

			''' <summary>
			''' Spins/blocks until node s is matched by a fulfill operation.
			''' </summary>
			''' <param name="s"> the waiting node </param>
			''' <param name="timed"> true if timed wait </param>
			''' <param name="nanos"> timeout value </param>
			''' <returns> matched node, or s if cancelled </returns>
			Friend Function awaitFulfill(ByVal s As SNode, ByVal timed As Boolean, ByVal nanos As Long) As SNode
	'            
	'             * When a node/thread is about to block, it sets its waiter
	'             * field and then rechecks state at least one more time
	'             * before actually parking, thus covering race vs
	'             * fulfiller noticing that waiter is non-null so should be
	'             * woken.
	'             *
	'             * When invoked by nodes that appear at the point of call
	'             * to be at the head of the stack, calls to park are
	'             * preceded by spins to avoid blocking when producers and
	'             * consumers are arriving very close in time.  This can
	'             * happen enough to bother only on multiprocessors.
	'             *
	'             * The order of checks for returning out of main loop
	'             * reflects fact that interrupts have precedence over
	'             * normal returns, which have precedence over
	'             * timeouts. (So, on timeout, one last check for match is
	'             * done before giving up.) Except that calls from untimed
	'             * SynchronousQueue.{poll/offer} don't check interrupts
	'             * and don't wait at all, so are trapped in transfer
	'             * method rather than calling awaitFulfill.
	'             
				Dim deadline As Long = If(timed, System.nanoTime() + nanos, 0L)
				Dim w As Thread = Thread.CurrentThread
				Dim spins As Integer = (If(shouldSpin(s), (If(timed, maxTimedSpins, maxUntimedSpins)), 0))
				Do
					If w.interrupted Then s.tryCancel()
					Dim m As SNode = s.match
					If m IsNot Nothing Then Return m
					If timed Then
						nanos = deadline - System.nanoTime()
						If nanos <= 0L Then
							s.tryCancel()
							Continue Do
						End If
					End If
					If spins > 0 Then
						spins = If(shouldSpin(s), (spins-1), 0)
					ElseIf s.waiter Is Nothing Then
						s.waiter = w ' establish waiter so can park next iter
					ElseIf Not timed Then
						java.util.concurrent.locks.LockSupport.park(Me)
					ElseIf nanos > spinForTimeoutThreshold Then
						java.util.concurrent.locks.LockSupport.parkNanos(Me, nanos)
					End If
				Loop
			End Function

			''' <summary>
			''' Returns true if node s is at head or there is an active
			''' fulfiller.
			''' </summary>
			Friend Function shouldSpin(ByVal s As SNode) As Boolean
				Dim h As SNode = head
				Return (h Is s OrElse h Is Nothing OrElse isFulfilling(h.mode))
			End Function

			''' <summary>
			''' Unlinks s from the stack.
			''' </summary>
			Friend Sub clean(ByVal s As SNode)
				s.item = Nothing ' forget item
				s.waiter = Nothing ' forget thread

	'            
	'             * At worst we may need to traverse entire stack to unlink
	'             * s. If there are multiple concurrent calls to clean, we
	'             * might not see s if another thread has already removed
	'             * it. But we can stop when we see any node known to
	'             * follow s. We use s.next unless it too is cancelled, in
	'             * which case we try the node one past. We don't check any
	'             * further because we don't want to doubly traverse just to
	'             * find sentinel.
	'             

				Dim past As SNode = s.next
				If past IsNot Nothing AndAlso past.cancelled Then past = past.next

				' Absorb cancelled nodes at head
				Dim p As SNode
				p = head
				Do While p IsNot Nothing AndAlso p IsNot past AndAlso p.cancelled
					casHead(p, p.next)
					p = head
				Loop

				' Unsplice embedded nodes
				Do While p IsNot Nothing AndAlso p IsNot past
					Dim n As SNode = p.next
					If n IsNot Nothing AndAlso n.cancelled Then
						p.casNext(n, n.next)
					Else
						p = n
					End If
				Loop
			End Sub

			' Unsafe mechanics
			Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
			Private Shared ReadOnly headOffset As Long
			Shared Sub New()
				Try
					UNSAFE = sun.misc.Unsafe.unsafe
					Dim k As  [Class] = GetType(TransferStack)
					headOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("head"))
				Catch e As Exception
					Throw New [Error](e)
				End Try
			End Sub
		End Class

		''' <summary>
		''' Dual Queue </summary>
		Friend NotInheritable Class TransferQueue(Of E)
			Inherits Transferer(Of E)

	'        
	'         * This extends Scherer-Scott dual queue algorithm, differing,
	'         * among other ways, by using modes within nodes rather than
	'         * marked pointers. The algorithm is a little simpler than
	'         * that for stacks because fulfillers do not need explicit
	'         * nodes, and matching is done by CAS'ing QNode.item field
	'         * from non-null to null (for put) or vice versa (for take).
	'         

			''' <summary>
			''' Node class for TransferQueue. </summary>
			Friend NotInheritable Class QNode
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
				Friend [next] As QNode ' next node in queue
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
				Friend item As Object ' CAS'ed to or from null
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
				Friend waiter As Thread ' to control park/unpark
				Friend ReadOnly isData As Boolean

				Friend Sub New(ByVal item As Object, ByVal isData As Boolean)
					Me.item = item
					Me.isData = isData
				End Sub

				Friend Function casNext(ByVal cmp As QNode, ByVal val As QNode) As Boolean
					Return [next] Is cmp AndAlso UNSAFE.compareAndSwapObject(Me, nextOffset, cmp, val)
				End Function

				Friend Function casItem(ByVal cmp As Object, ByVal val As Object) As Boolean
					Return item Is cmp AndAlso UNSAFE.compareAndSwapObject(Me, itemOffset, cmp, val)
				End Function

				''' <summary>
				''' Tries to cancel by CAS'ing ref to this as item.
				''' </summary>
				Friend Sub tryCancel(ByVal cmp As Object)
					UNSAFE.compareAndSwapObject(Me, itemOffset, cmp, Me)
				End Sub

				Friend Property cancelled As Boolean
					Get
						Return item Is Me
					End Get
				End Property

				''' <summary>
				''' Returns true if this node is known to be off the queue
				''' because its next pointer has been forgotten due to
				''' an advanceHead operation.
				''' </summary>
				Friend Property offList As Boolean
					Get
						Return [next] Is Me
					End Get
				End Property

				' Unsafe mechanics
				Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
				Private Shared ReadOnly itemOffset As Long
				Private Shared ReadOnly nextOffset As Long

				Shared Sub New()
					Try
						UNSAFE = sun.misc.Unsafe.unsafe
						Dim k As  [Class] = GetType(QNode)
						itemOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("item"))
						nextOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("next"))
					Catch e As Exception
						Throw New [Error](e)
					End Try
				End Sub
			End Class

			''' <summary>
			''' Head of queue </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			<NonSerialized> _
			Friend head As QNode
			''' <summary>
			''' Tail of queue </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			<NonSerialized> _
			Friend tail As QNode
			''' <summary>
			''' Reference to a cancelled node that might not yet have been
			''' unlinked from queue because it was the last inserted node
			''' when it was cancelled.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			<NonSerialized> _
			Friend cleanMe As QNode

			Friend Sub New()
				Dim h As New QNode(Nothing, False) ' initialize to dummy node.
				head = h
				tail = h
			End Sub

			''' <summary>
			''' Tries to cas nh as new head; if successful, unlink
			''' old head's next node to avoid garbage retention.
			''' </summary>
			Friend Sub advanceHead(ByVal h As QNode, ByVal nh As QNode)
				If h Is head AndAlso UNSAFE.compareAndSwapObject(Me, headOffset, h, nh) Then h.next = h ' forget old next
			End Sub

			''' <summary>
			''' Tries to cas nt as new tail.
			''' </summary>
			Friend Sub advanceTail(ByVal t As QNode, ByVal nt As QNode)
				If tail Is t Then UNSAFE.compareAndSwapObject(Me, tailOffset, t, nt)
			End Sub

			''' <summary>
			''' Tries to CAS cleanMe slot.
			''' </summary>
			Friend Function casCleanMe(ByVal cmp As QNode, ByVal val As QNode) As Boolean
				Return cleanMe Is cmp AndAlso UNSAFE.compareAndSwapObject(Me, cleanMeOffset, cmp, val)
			End Function

			''' <summary>
			''' Puts or takes an item.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend Function transfer(ByVal e As E, ByVal timed As Boolean, ByVal nanos As Long) As E
	'             Basic algorithm is to loop trying to take either of
	'             * two actions:
	'             *
	'             * 1. If queue apparently empty or holding same-mode nodes,
	'             *    try to add node to queue of waiters, wait to be
	'             *    fulfilled (or cancelled) and return matching item.
	'             *
	'             * 2. If queue apparently contains waiting items, and this
	'             *    call is of complementary mode, try to fulfill by CAS'ing
	'             *    item field of waiting node and dequeuing it, and then
	'             *    returning matching item.
	'             *
	'             * In each case, along the way, check for and try to help
	'             * advance head and tail on behalf of other stalled/slow
	'             * threads.
	'             *
	'             * The loop starts off with a null check guarding against
	'             * seeing uninitialized head or tail values. This never
	'             * happens in current SynchronousQueue, but could if
	'             * callers held non-volatile/final ref to the
	'             * transferer. The check is here anyway because it places
	'             * null checks at top of loop, which is usually faster
	'             * than having them implicitly interspersed.
	'             

				Dim s As QNode = Nothing ' constructed/reused as needed
				Dim isData As Boolean = (e IsNot Nothing)

				Do
					Dim t As QNode = tail
					Dim h As QNode = head
					If t Is Nothing OrElse h Is Nothing Then ' saw uninitialized value Continue Do ' spin

					If h Is t OrElse t.isData = isData Then ' empty or same-mode
						Dim tn As QNode = t.next
						If t IsNot tail Then ' inconsistent read Continue Do
						If tn IsNot Nothing Then ' lagging tail
							advanceTail(t, tn)
							Continue Do
						End If
						If timed AndAlso nanos <= 0 Then ' can't wait Return Nothing
						If s Is Nothing Then s = New QNode(e, isData)
						If Not t.casNext(Nothing, s) Then ' failed to link in Continue Do

						advanceTail(t, s) ' swing tail and wait
						Dim x As Object = awaitFulfill(s, e, timed, nanos)
						If x Is s Then ' wait was cancelled
							clean(t, s)
							Return Nothing
						End If

						If Not s.offList Then ' not already unlinked
							advanceHead(t, s) ' unlink if head
							If x IsNot Nothing Then ' and forget fields s.item = s
							s.waiter = Nothing
						End If
						Return If(x IsNot Nothing, CType(x, E), e)
 ' complementary-mode
					Else
						Dim m As QNode = h.next ' node to fulfill
						If t IsNot tail OrElse m Is Nothing OrElse h IsNot head Then Continue Do ' inconsistent read

						Dim x As Object = m.item
						If isData = (x IsNot Nothing) OrElse x Is m OrElse (Not m.casItem(x, e)) Then ' lost CAS -  m cancelled -  m already fulfilled
							advanceHead(h, m) ' dequeue and retry
							Continue Do
						End If

						advanceHead(h, m) ' successfully fulfilled
						java.util.concurrent.locks.LockSupport.unpark(m.waiter)
						Return If(x IsNot Nothing, CType(x, E), e)
					End If
				Loop
			End Function

			''' <summary>
			''' Spins/blocks until node s is fulfilled.
			''' </summary>
			''' <param name="s"> the waiting node </param>
			''' <param name="e"> the comparison value for checking match </param>
			''' <param name="timed"> true if timed wait </param>
			''' <param name="nanos"> timeout value </param>
			''' <returns> matched item, or s if cancelled </returns>
			Friend Function awaitFulfill(ByVal s As QNode, ByVal e As E, ByVal timed As Boolean, ByVal nanos As Long) As Object
				' Same idea as TransferStack.awaitFulfill 
				Dim deadline As Long = If(timed, System.nanoTime() + nanos, 0L)
				Dim w As Thread = Thread.CurrentThread
				Dim spins As Integer = (If(head.next Is s, (If(timed, maxTimedSpins, maxUntimedSpins)), 0))
				Do
					If w.interrupted Then s.tryCancel(e)
					Dim x As Object = s.item
					If x IsNot e Then Return x
					If timed Then
						nanos = deadline - System.nanoTime()
						If nanos <= 0L Then
							s.tryCancel(e)
							Continue Do
						End If
					End If
					If spins > 0 Then
						spins -= 1
					ElseIf s.waiter Is Nothing Then
						s.waiter = w
					ElseIf Not timed Then
						java.util.concurrent.locks.LockSupport.park(Me)
					ElseIf nanos > spinForTimeoutThreshold Then
						java.util.concurrent.locks.LockSupport.parkNanos(Me, nanos)
					End If
				Loop
			End Function

			''' <summary>
			''' Gets rid of cancelled node s with original predecessor pred.
			''' </summary>
			Friend Sub clean(ByVal pred As QNode, ByVal s As QNode)
				s.waiter = Nothing ' forget thread
	'            
	'             * At any given time, exactly one node on list cannot be
	'             * deleted -- the last inserted node. To accommodate this,
	'             * if we cannot delete s, we save its predecessor as
	'             * "cleanMe", deleting the previously saved version
	'             * first. At least one of node s or the node previously
	'             * saved can always be deleted, so this always terminates.
	'             
				Do While pred.next Is s ' Return early if already unlinked
					Dim h As QNode = head
					Dim hn As QNode = h.next ' Absorb cancelled first node as head
					If hn IsNot Nothing AndAlso hn.cancelled Then
						advanceHead(h, hn)
						Continue Do
					End If
					Dim t As QNode = tail ' Ensure consistent read for tail
					If t Is h Then Return
					Dim tn As QNode = t.next
					If t IsNot tail Then Continue Do
					If tn IsNot Nothing Then
						advanceTail(t, tn)
						Continue Do
					End If
					If s IsNot t Then ' If not tail, try to unsplice
						Dim sn As QNode = s.next
						If sn Is s OrElse pred.casNext(s, sn) Then Return
					End If
					Dim dp As QNode = cleanMe
					If dp IsNot Nothing Then ' Try unlinking previous cancelled node
						Dim d As QNode = dp.next
						Dim dn As QNode
						dn = d.next
						If d Is Nothing OrElse d Is dp OrElse (Not d.cancelled) OrElse (d IsNot t AndAlso dn IsNot Nothing AndAlso dn IsNot d AndAlso dp.casNext(d, dn)) Then ' d unspliced -    that is on list -    has successor -  d not tail and -  d not cancelled or -  d is off list or -  d is gone or casCleanMe(dp, Nothing)
						If dp Is pred Then Return ' s is already saved node
					ElseIf casCleanMe(Nothing, pred) Then
						Return ' Postpone cleaning s
					End If
				Loop
			End Sub

			Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
			Private Shared ReadOnly headOffset As Long
			Private Shared ReadOnly tailOffset As Long
			Private Shared ReadOnly cleanMeOffset As Long
			Shared Sub New()
				Try
					UNSAFE = sun.misc.Unsafe.unsafe
					Dim k As  [Class] = GetType(TransferQueue)
					headOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("head"))
					tailOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("tail"))
					cleanMeOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("cleanMe"))
				Catch e As Exception
					Throw New [Error](e)
				End Try
			End Sub
		End Class

		''' <summary>
		''' The transferer. Set only in constructor, but cannot be declared
		''' as final without further complicating serialization.  Since
		''' this is accessed only at most once per public method, there
		''' isn't a noticeable performance penalty for using volatile
		''' instead of final here.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private transferer As Transferer(Of E)

		''' <summary>
		''' Creates a {@code SynchronousQueue} with nonfair access policy.
		''' </summary>
		Public Sub New()
			Me.New(False)
		End Sub

		''' <summary>
		''' Creates a {@code SynchronousQueue} with the specified fairness policy.
		''' </summary>
		''' <param name="fair"> if true, waiting threads contend in FIFO order for
		'''        access; otherwise the order is unspecified. </param>
		Public Sub New(ByVal fair As Boolean)
			transferer = If(fair, New TransferQueue(Of E), New TransferStack(Of E))
		End Sub

		''' <summary>
		''' Adds the specified element to this queue, waiting if necessary for
		''' another thread to receive it.
		''' </summary>
		''' <exception cref="InterruptedException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Sub put(ByVal e As E)
			If e Is Nothing Then Throw New NullPointerException
			If transferer.transfer(e, False, 0) Is Nothing Then
				Thread.interrupted()
				Throw New InterruptedException
			End If
		End Sub

		''' <summary>
		''' Inserts the specified element into this queue, waiting if necessary
		''' up to the specified wait time for another thread to receive it.
		''' </summary>
		''' <returns> {@code true} if successful, or {@code false} if the
		'''         specified waiting time elapses before a consumer appears </returns>
		''' <exception cref="InterruptedException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Function offer(ByVal e As E, ByVal timeout As Long, ByVal unit As TimeUnit) As Boolean
			If e Is Nothing Then Throw New NullPointerException
			If transferer.transfer(e, True, unit.toNanos(timeout)) IsNot Nothing Then Return True
			If Not Thread.interrupted() Then Return False
			Throw New InterruptedException
		End Function

		''' <summary>
		''' Inserts the specified element into this queue, if another thread is
		''' waiting to receive it.
		''' </summary>
		''' <param name="e"> the element to add </param>
		''' <returns> {@code true} if the element was added to this queue, else
		'''         {@code false} </returns>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function offer(ByVal e As E) As Boolean
			If e Is Nothing Then Throw New NullPointerException
			Return transferer.transfer(e, True, 0) IsNot Nothing
		End Function

		''' <summary>
		''' Retrieves and removes the head of this queue, waiting if necessary
		''' for another thread to insert it.
		''' </summary>
		''' <returns> the head of this queue </returns>
		''' <exception cref="InterruptedException"> {@inheritDoc} </exception>
		Public Overridable Function take() As E Implements BlockingQueue(Of E).take
			Dim e As E = transferer.transfer(Nothing, False, 0)
			If e IsNot Nothing Then Return e
			Thread.interrupted()
			Throw New InterruptedException
		End Function

		''' <summary>
		''' Retrieves and removes the head of this queue, waiting
		''' if necessary up to the specified wait time, for another thread
		''' to insert it.
		''' </summary>
		''' <returns> the head of this queue, or {@code null} if the
		'''         specified waiting time elapses before an element is present </returns>
		''' <exception cref="InterruptedException"> {@inheritDoc} </exception>
		Public Overridable Function poll(ByVal timeout As Long, ByVal unit As TimeUnit) As E Implements BlockingQueue(Of E).poll
			Dim e As E = transferer.transfer(Nothing, True, unit.toNanos(timeout))
			If e IsNot Nothing OrElse (Not Thread.interrupted()) Then Return e
			Throw New InterruptedException
		End Function

		''' <summary>
		''' Retrieves and removes the head of this queue, if another thread
		''' is currently making an element available.
		''' </summary>
		''' <returns> the head of this queue, or {@code null} if no
		'''         element is available </returns>
		Public Overridable Function poll() As E
			Return transferer.transfer(Nothing, True, 0)
		End Function

		''' <summary>
		''' Always returns {@code true}.
		''' A {@code SynchronousQueue} has no internal capacity.
		''' </summary>
		''' <returns> {@code true} </returns>
		Public Overridable Property empty As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Always returns zero.
		''' A {@code SynchronousQueue} has no internal capacity.
		''' </summary>
		''' <returns> zero </returns>
		Public Overridable Function size() As Integer
			Return 0
		End Function

		''' <summary>
		''' Always returns zero.
		''' A {@code SynchronousQueue} has no internal capacity.
		''' </summary>
		''' <returns> zero </returns>
		Public Overridable Function remainingCapacity() As Integer Implements BlockingQueue(Of E).remainingCapacity
			Return 0
		End Function

		''' <summary>
		''' Does nothing.
		''' A {@code SynchronousQueue} has no internal capacity.
		''' </summary>
		Public Overridable Sub clear()
		End Sub

		''' <summary>
		''' Always returns {@code false}.
		''' A {@code SynchronousQueue} has no internal capacity.
		''' </summary>
		''' <param name="o"> the element </param>
		''' <returns> {@code false} </returns>
		Public Overridable Function contains(ByVal o As Object) As Boolean Implements BlockingQueue(Of E).contains
			Return False
		End Function

		''' <summary>
		''' Always returns {@code false}.
		''' A {@code SynchronousQueue} has no internal capacity.
		''' </summary>
		''' <param name="o"> the element to remove </param>
		''' <returns> {@code false} </returns>
		Public Overridable Function remove(ByVal o As Object) As Boolean Implements BlockingQueue(Of E).remove
			Return False
		End Function

		''' <summary>
		''' Returns {@code false} unless the given collection is empty.
		''' A {@code SynchronousQueue} has no internal capacity.
		''' </summary>
		''' <param name="c"> the collection </param>
		''' <returns> {@code false} unless given collection is empty </returns>
		Public Overridable Function containsAll(Of T1)(ByVal c As Collection(Of T1)) As Boolean
			Return c.empty
		End Function

		''' <summary>
		''' Always returns {@code false}.
		''' A {@code SynchronousQueue} has no internal capacity.
		''' </summary>
		''' <param name="c"> the collection </param>
		''' <returns> {@code false} </returns>
		Public Overridable Function removeAll(Of T1)(ByVal c As Collection(Of T1)) As Boolean
			Return False
		End Function

		''' <summary>
		''' Always returns {@code false}.
		''' A {@code SynchronousQueue} has no internal capacity.
		''' </summary>
		''' <param name="c"> the collection </param>
		''' <returns> {@code false} </returns>
		Public Overridable Function retainAll(Of T1)(ByVal c As Collection(Of T1)) As Boolean
			Return False
		End Function

		''' <summary>
		''' Always returns {@code null}.
		''' A {@code SynchronousQueue} does not return elements
		''' unless actively waited on.
		''' </summary>
		''' <returns> {@code null} </returns>
		Public Overridable Function peek() As E
			Return Nothing
		End Function

		''' <summary>
		''' Returns an empty iterator in which {@code hasNext} always returns
		''' {@code false}.
		''' </summary>
		''' <returns> an empty iterator </returns>
		Public Overridable Function [iterator]() As [Iterator](Of E)
			Return Collections.emptyIterator()
		End Function

		''' <summary>
		''' Returns an empty spliterator in which calls to
		''' <seealso cref="java.util.Spliterator#trySplit()"/> always return {@code null}.
		''' </summary>
		''' <returns> an empty spliterator
		''' @since 1.8 </returns>
		Public Overridable Function spliterator() As java.util.Spliterator(Of E)
			Return java.util.Spliterators.emptySpliterator()
		End Function

		''' <summary>
		''' Returns a zero-length array. </summary>
		''' <returns> a zero-length array </returns>
		Public Overridable Function toArray() As Object()
			Return New Object(){}
		End Function

		''' <summary>
		''' Sets the zeroeth element of the specified array to {@code null}
		''' (if the array has non-zero length) and returns it.
		''' </summary>
		''' <param name="a"> the array </param>
		''' <returns> the specified array </returns>
		''' <exception cref="NullPointerException"> if the specified array is null </exception>
		Public Overridable Function toArray(Of T)(ByVal a As T()) As T()
			If a.Length > 0 Then a(0) = Nothing
			Return a
		End Function

		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="ClassCastException">            {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">          {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function drainTo(Of T1)(ByVal c As Collection(Of T1)) As Integer
			If c Is Nothing Then Throw New NullPointerException
			If c Is Me Then Throw New IllegalArgumentException
			Dim n As Integer = 0
			Dim e As E
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While (e = poll()) IsNot Nothing
				c.add(e)
				n += 1
			Loop
			Return n
		End Function

		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="ClassCastException">            {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">          {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function drainTo(Of T1)(ByVal c As Collection(Of T1), ByVal maxElements As Integer) As Integer
			If c Is Nothing Then Throw New NullPointerException
			If c Is Me Then Throw New IllegalArgumentException
			Dim n As Integer = 0
			Dim e As E
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While n < maxElements AndAlso (e = poll()) IsNot Nothing
				c.add(e)
				n += 1
			Loop
			Return n
		End Function

	'    
	'     * To cope with serialization strategy in the 1.5 version of
	'     * SynchronousQueue, we declare some unused classes and fields
	'     * that exist solely to enable serializability across versions.
	'     * These fields are never used, so are initialized only if this
	'     * object is ever serialized or deserialized.
	'     

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		<Serializable> _
		Friend Class WaitQueue
		End Class
		Friend Class LifoWaitQueue
			Inherits WaitQueue

			Private Const serialVersionUID As Long = -3633113410248163686L
		End Class
		Friend Class FifoWaitQueue
			Inherits WaitQueue

			Private Const serialVersionUID As Long = -3623113410248163686L
		End Class
		Private qlock As java.util.concurrent.locks.ReentrantLock
		Private waitingProducers As WaitQueue
		Private waitingConsumers As WaitQueue

		''' <summary>
		''' Saves this queue to a stream (that is, serializes it). </summary>
		''' <param name="s"> the stream </param>
		''' <exception cref="java.io.IOException"> if an I/O error occurs </exception>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			Dim fair As Boolean = TypeOf transferer Is TransferQueue
			If fair Then
				qlock = New java.util.concurrent.locks.ReentrantLock(True)
				waitingProducers = New FifoWaitQueue
				waitingConsumers = New FifoWaitQueue
			Else
				qlock = New java.util.concurrent.locks.ReentrantLock
				waitingProducers = New LifoWaitQueue
				waitingConsumers = New LifoWaitQueue
			End If
			s.defaultWriteObject()
		End Sub

		''' <summary>
		''' Reconstitutes this queue from a stream (that is, deserializes it). </summary>
		''' <param name="s"> the stream </param>
		''' <exception cref="ClassNotFoundException"> if the class of a serialized object
		'''         could not be found </exception>
		''' <exception cref="java.io.IOException"> if an I/O error occurs </exception>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			If TypeOf waitingProducers Is FifoWaitQueue Then
				transferer = New TransferQueue(Of E)
			Else
				transferer = New TransferStack(Of E)
			End If
		End Sub

		' Unsafe mechanics
		Friend Shared Function objectFieldOffset(ByVal UNSAFE As sun.misc.Unsafe, ByVal field As String, ByVal klazz As [Class]) As Long
			Try
				Return UNSAFE.objectFieldOffset(klazz.getDeclaredField(field))
			Catch e As NoSuchFieldException
				' Convert Exception to corresponding Error
				Dim error_Renamed As New NoSuchFieldError(field)
				error_Renamed.initCause(e)
				Throw error_Renamed
			End Try
		End Function

	End Class

End Namespace
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
' * Written by Doug Lea and Martin Buchholz with assistance from members of
' * JCP JSR-166 Expert Group and released to the public domain, as explained
' * at http://creativecommons.org/publicdomain/zero/1.0/
' 

Namespace java.util.concurrent


	''' <summary>
	''' An unbounded concurrent <seealso cref="Deque deque"/> based on linked nodes.
	''' Concurrent insertion, removal, and access operations execute safely
	''' across multiple threads.
	''' A {@code ConcurrentLinkedDeque} is an appropriate choice when
	''' many threads will share access to a common collection.
	''' Like most other concurrent collection implementations, this class
	''' does not permit the use of {@code null} elements.
	''' 
	''' <p>Iterators and spliterators are
	''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
	''' 
	''' <p>Beware that, unlike in most collections, the {@code size} method
	''' is <em>NOT</em> a constant-time operation. Because of the
	''' asynchronous nature of these deques, determining the current number
	''' of elements requires a traversal of the elements, and so may report
	''' inaccurate results if this collection is modified during traversal.
	''' Additionally, the bulk operations {@code addAll},
	''' {@code removeAll}, {@code retainAll}, {@code containsAll},
	''' {@code equals}, and {@code toArray} are <em>not</em> guaranteed
	''' to be performed atomically. For example, an iterator operating
	''' concurrently with an {@code addAll} operation might view only some
	''' of the added elements.
	''' 
	''' <p>This class and its iterator implement all of the <em>optional</em>
	''' methods of the <seealso cref="Deque"/> and <seealso cref="Iterator"/> interfaces.
	''' 
	''' <p>Memory consistency effects: As with other concurrent collections,
	''' actions in a thread prior to placing an object into a
	''' {@code ConcurrentLinkedDeque}
	''' <a href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	''' actions subsequent to the access or removal of that element from
	''' the {@code ConcurrentLinkedDeque} in another thread.
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @since 1.7
	''' @author Doug Lea
	''' @author Martin Buchholz </summary>
	''' @param <E> the type of elements held in this collection </param>
	<Serializable> _
	Public Class ConcurrentLinkedDeque(Of E)
		Inherits java.util.AbstractCollection(Of E)
		Implements java.util.Deque(Of E)

	'    
	'     * This is an implementation of a concurrent lock-free deque
	'     * supporting interior removes but not interior insertions, as
	'     * required to support the entire Deque interface.
	'     *
	'     * We extend the techniques developed for ConcurrentLinkedQueue and
	'     * LinkedTransferQueue (see the internal docs for those classes).
	'     * Understanding the ConcurrentLinkedQueue implementation is a
	'     * prerequisite for understanding the implementation of this class.
	'     *
	'     * The data structure is a symmetrical doubly-linked "GC-robust"
	'     * linked list of nodes.  We minimize the number of volatile writes
	'     * using two techniques: advancing multiple hops with a single CAS
	'     * and mixing volatile and non-volatile writes of the same memory
	'     * locations.
	'     *
	'     * A node contains the expected E ("item") and links to predecessor
	'     * ("prev") and successor ("next") nodes:
	'     *
	'     * class Node<E> { volatile Node<E> prev, next; volatile E item; }
	'     *
	'     * A node p is considered "live" if it contains a non-null item
	'     * (p.item != null).  When an item is CASed to null, the item is
	'     * atomically logically deleted from the collection.
	'     *
	'     * At any time, there is precisely one "first" node with a null
	'     * prev reference that terminates any chain of prev references
	'     * starting at a live node.  Similarly there is precisely one
	'     * "last" node terminating any chain of next references starting at
	'     * a live node.  The "first" and "last" nodes may or may not be live.
	'     * The "first" and "last" nodes are always mutually reachable.
	'     *
	'     * A new element is added atomically by CASing the null prev or
	'     * next reference in the first or last node to a fresh node
	'     * containing the element.  The element's node atomically becomes
	'     * "live" at that point.
	'     *
	'     * A node is considered "active" if it is a live node, or the
	'     * first or last node.  Active nodes cannot be unlinked.
	'     *
	'     * A "self-link" is a next or prev reference that is the same node:
	'     *   p.prev == p  or  p.next == p
	'     * Self-links are used in the node unlinking process.  Active nodes
	'     * never have self-links.
	'     *
	'     * A node p is active if and only if:
	'     *
	'     * p.item != null ||
	'     * (p.prev == null && p.next != p) ||
	'     * (p.next == null && p.prev != p)
	'     *
	'     * The deque object has two node references, "head" and "tail".
	'     * The head and tail are only approximations to the first and last
	'     * nodes of the deque.  The first node can always be found by
	'     * following prev pointers from head; likewise for tail.  However,
	'     * it is permissible for head and tail to be referring to deleted
	'     * nodes that have been unlinked and so may not be reachable from
	'     * any live node.
	'     *
	'     * There are 3 stages of node deletion;
	'     * "logical deletion", "unlinking", and "gc-unlinking".
	'     *
	'     * 1. "logical deletion" by CASing item to null atomically removes
	'     * the element from the collection, and makes the containing node
	'     * eligible for unlinking.
	'     *
	'     * 2. "unlinking" makes a deleted node unreachable from active
	'     * nodes, and thus eventually reclaimable by GC.  Unlinked nodes
	'     * may remain reachable indefinitely from an iterator.
	'     *
	'     * Physical node unlinking is merely an optimization (albeit a
	'     * critical one), and so can be performed at our convenience.  At
	'     * any time, the set of live nodes maintained by prev and next
	'     * links are identical, that is, the live nodes found via next
	'     * links from the first node is equal to the elements found via
	'     * prev links from the last node.  However, this is not true for
	'     * nodes that have already been logically deleted - such nodes may
	'     * be reachable in one direction only.
	'     *
	'     * 3. "gc-unlinking" takes unlinking further by making active
	'     * nodes unreachable from deleted nodes, making it easier for the
	'     * GC to reclaim future deleted nodes.  This step makes the data
	'     * structure "gc-robust", as first described in detail by Boehm
	'     * (http://portal.acm.org/citation.cfm?doid=503272.503282).
	'     *
	'     * GC-unlinked nodes may remain reachable indefinitely from an
	'     * iterator, but unlike unlinked nodes, are never reachable from
	'     * head or tail.
	'     *
	'     * Making the data structure GC-robust will eliminate the risk of
	'     * unbounded memory retention with conservative GCs and is likely
	'     * to improve performance with generational GCs.
	'     *
	'     * When a node is dequeued at either end, e.g. via poll(), we would
	'     * like to break any references from the node to active nodes.  We
	'     * develop further the use of self-links that was very effective in
	'     * other concurrent collection classes.  The idea is to replace
	'     * prev and next pointers with special values that are interpreted
	'     * to mean off-the-list-at-one-end.  These are approximations, but
	'     * good enough to preserve the properties we want in our
	'     * traversals, e.g. we guarantee that a traversal will never visit
	'     * the same element twice, but we don't guarantee whether a
	'     * traversal that runs out of elements will be able to see more
	'     * elements later after enqueues at that end.  Doing gc-unlinking
	'     * safely is particularly tricky, since any node can be in use
	'     * indefinitely (for example by an iterator).  We must ensure that
	'     * the nodes pointed at by head/tail never get gc-unlinked, since
	'     * head/tail are needed to get "back on track" by other nodes that
	'     * are gc-unlinked.  gc-unlinking accounts for much of the
	'     * implementation complexity.
	'     *
	'     * Since neither unlinking nor gc-unlinking are necessary for
	'     * correctness, there are many implementation choices regarding
	'     * frequency (eagerness) of these operations.  Since volatile
	'     * reads are likely to be much cheaper than CASes, saving CASes by
	'     * unlinking multiple adjacent nodes at a time may be a win.
	'     * gc-unlinking can be performed rarely and still be effective,
	'     * since it is most important that long chains of deleted nodes
	'     * are occasionally broken.
	'     *
	'     * The actual representation we use is that p.next == p means to
	'     * goto the first node (which in turn is reached by following prev
	'     * pointers from head), and p.next == null && p.prev == p means
	'     * that the iteration is at an end and that p is a (static final)
	'     * dummy node, NEXT_TERMINATOR, and not the last active node.
	'     * Finishing the iteration when encountering such a TERMINATOR is
	'     * good enough for read-only traversals, so such traversals can use
	'     * p.next == null as the termination condition.  When we need to
	'     * find the last (active) node, for enqueueing a new node, we need
	'     * to check whether we have reached a TERMINATOR node; if so,
	'     * restart traversal from tail.
	'     *
	'     * The implementation is completely directionally symmetrical,
	'     * except that most public methods that iterate through the list
	'     * follow next pointers ("forward" direction).
	'     *
	'     * We believe (without full proof) that all single-element deque
	'     * operations (e.g., addFirst, peekLast, pollLast) are linearizable
	'     * (see Herlihy and Shavit's book).  However, some combinations of
	'     * operations are known not to be linearizable.  In particular,
	'     * when an addFirst(A) is racing with pollFirst() removing B, it is
	'     * possible for an observer iterating over the elements to observe
	'     * A B C and subsequently observe A C, even though no interior
	'     * removes are ever performed.  Nevertheless, iterators behave
	'     * reasonably, providing the "weakly consistent" guarantees.
	'     *
	'     * Empirically, microbenchmarks suggest that this class adds about
	'     * 40% overhead relative to ConcurrentLinkedQueue, which feels as
	'     * good as we can hope for.
	'     

		Private Const serialVersionUID As Long = 876323262645176354L

		''' <summary>
		''' A node from which the first node on list (that is, the unique node p
		''' with p.prev == null && p.next != p) can be reached in O(1) time.
		''' Invariants:
		''' - the first node is always O(1) reachable from head via prev links
		''' - all live nodes are reachable from the first node via succ()
		''' - head != null
		''' - (tmp = head).next != tmp || tmp != head
		''' - head is never gc-unlinked (but may be unlinked)
		''' Non-invariants:
		''' - head.item may or may not be null
		''' - head may not be reachable from the first or last node, or from tail
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private head As Node(Of E)

		''' <summary>
		''' A node from which the last node on list (that is, the unique node p
		''' with p.next == null && p.prev != p) can be reached in O(1) time.
		''' Invariants:
		''' - the last node is always O(1) reachable from tail via next links
		''' - all live nodes are reachable from the last node via pred()
		''' - tail != null
		''' - tail is never gc-unlinked (but may be unlinked)
		''' Non-invariants:
		''' - tail.item may or may not be null
		''' - tail may not be reachable from the first or last node, or from head
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private tail As Node(Of E)

		Private Shared ReadOnly PREV_TERMINATOR As Node(Of Object), NEXT_TERMINATOR As Node(Of Object)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Overridable Function prevTerminator() As Node(Of E)
			Return CType(PREV_TERMINATOR, Node(Of E))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Overridable Function nextTerminator() As Node(Of E)
			Return CType(NEXT_TERMINATOR, Node(Of E))
		End Function

		Friend NotInheritable Class Node(Of E)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend prev As Node(Of E)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend item As E
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend [next] As Node(Of E)

			Friend Sub New() ' default constructor for NEXT_TERMINATOR, PREV_TERMINATOR
			End Sub

			''' <summary>
			''' Constructs a new node.  Uses relaxed write because item can
			''' only be seen after publication via casNext or casPrev.
			''' </summary>
			Friend Sub New(ByVal item As E)
				UNSAFE.putObject(Me, itemOffset, item)
			End Sub

			Friend Function casItem(ByVal cmp As E, ByVal val As E) As Boolean
				Return UNSAFE.compareAndSwapObject(Me, itemOffset, cmp, val)
			End Function

			Friend Sub lazySetNext(ByVal val As Node(Of E))
				UNSAFE.putOrderedObject(Me, nextOffset, val)
			End Sub

			Friend Function casNext(ByVal cmp As Node(Of E), ByVal val As Node(Of E)) As Boolean
				Return UNSAFE.compareAndSwapObject(Me, nextOffset, cmp, val)
			End Function

			Friend Sub lazySetPrev(ByVal val As Node(Of E))
				UNSAFE.putOrderedObject(Me, prevOffset, val)
			End Sub

			Friend Function casPrev(ByVal cmp As Node(Of E), ByVal val As Node(Of E)) As Boolean
				Return UNSAFE.compareAndSwapObject(Me, prevOffset, cmp, val)
			End Function

			' Unsafe mechanics

			Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
			Private Shared ReadOnly prevOffset As Long
			Private Shared ReadOnly itemOffset As Long
			Private Shared ReadOnly nextOffset As Long

			Shared Sub New()
				Try
					UNSAFE = sun.misc.Unsafe.unsafe
					Dim k As Class = GetType(Node)
					prevOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("prev"))
					itemOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("item"))
					nextOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("next"))
				Catch e As Exception
					Throw New [Error](e)
				End Try
			End Sub
		End Class

		''' <summary>
		''' Links e as first element.
		''' </summary>
		Private Sub linkFirst(ByVal e As E)
			checkNotNull(e)
			Dim newNode As New Node(Of E)(e)

			restartFromHead:
			Do
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				for (Node<E> h = head, p = h, q;;)
					q = p.prev
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					q = (p = q).prev
					If q IsNot Nothing AndAlso q IsNot Nothing Then
						' Check for head updates every other hop.
						' If p == q, we are sure to follow head instead.
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						p = If(h IsNot (h = head), h, q)
					ElseIf p.next Is p Then ' PREV_TERMINATOR
						GoTo restartFromHead
					Else
						' p is first node
						newNode.lazySetNext(p) ' CAS piggyback
						If p.casPrev(Nothing, newNode) Then
							' Successful CAS is the linearization point
							' for e to become an element of this deque,
							' and for newNode to become "live".
							If p IsNot h Then ' hop two nodes at a time casHead(h, newNode) ' Failure is OK.
							Return
						End If
						' Lost CAS race to another thread; re-read prev
					End If
			Loop
		End Sub

		''' <summary>
		''' Links e as last element.
		''' </summary>
		Private Sub linkLast(ByVal e As E)
			checkNotNull(e)
			Dim newNode As New Node(Of E)(e)

			restartFromTail:
			Do
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				for (Node<E> t = tail, p = t, q;;)
					q = p.next
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					q = (p = q).next
					If q IsNot Nothing AndAlso q IsNot Nothing Then
						' Check for tail updates every other hop.
						' If p == q, we are sure to follow tail instead.
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						p = If(t IsNot (t = tail), t, q)
					ElseIf p.prev Is p Then ' NEXT_TERMINATOR
						GoTo restartFromTail
					Else
						' p is last node
						newNode.lazySetPrev(p) ' CAS piggyback
						If p.casNext(Nothing, newNode) Then
							' Successful CAS is the linearization point
							' for e to become an element of this deque,
							' and for newNode to become "live".
							If p IsNot t Then ' hop two nodes at a time casTail(t, newNode) ' Failure is OK.
							Return
						End If
						' Lost CAS race to another thread; re-read next
					End If
			Loop
		End Sub

		Private Const HOPS As Integer = 2

		''' <summary>
		''' Unlinks non-null node x.
		''' </summary>
		Friend Overridable Sub unlink(ByVal x As Node(Of E))
			' assert x != null;
			' assert x.item == null;
			' assert x != PREV_TERMINATOR;
			' assert x != NEXT_TERMINATOR;

			Dim prev As Node(Of E) = x.prev
			Dim [next] As Node(Of E) = x.next
			If prev Is Nothing Then
				unlinkFirst(x, [next])
			ElseIf [next] Is Nothing Then
				unlinkLast(x, prev)
			Else
				' Unlink interior node.
				'
				' This is the common case, since a series of polls at the
				' same end will be "interior" removes, except perhaps for
				' the first one, since end nodes cannot be unlinked.
				'
				' At any time, all active nodes are mutually reachable by
				' following a sequence of either next or prev pointers.
				'
				' Our strategy is to find the unique active predecessor
				' and successor of x.  Try to fix up their links so that
				' they point to each other, leaving x unreachable from
				' active nodes.  If successful, and if x has no live
				' predecessor/successor, we additionally try to gc-unlink,
				' leaving active nodes unreachable from x, by rechecking
				' that the status of predecessor and successor are
				' unchanged and ensuring that x is not reachable from
				' tail/head, before setting x's prev/next links to their
				' logical approximate replacements, self/TERMINATOR.
				Dim activePred As Node(Of E), activeSucc As Node(Of E)
				Dim isFirst, isLast As Boolean
				Dim hops_Renamed As Integer = 1

				' Find active predecessor
				Dim p As Node(Of E) = prev
				Do
					If p.item IsNot Nothing Then
						activePred = p
						isFirst = False
						Exit Do
					End If
					Dim q As Node(Of E) = p.prev
					If q Is Nothing Then
						If p.next Is p Then Return
						activePred = p
						isFirst = True
						Exit Do
					ElseIf p Is q Then
						Return
					Else
						p = q
					End If
					hops_Renamed += 1
				Loop

				' Find active successor
				p = [next]
				Do
					If p.item IsNot Nothing Then
						activeSucc = p
						isLast = False
						Exit Do
					End If
					Dim q As Node(Of E) = p.next
					If q Is Nothing Then
						If p.prev Is p Then Return
						activeSucc = p
						isLast = True
						Exit Do
					ElseIf p Is q Then
						Return
					Else
						p = q
					End If
					hops_Renamed += 1
				Loop

				' TODO: better HOP heuristics
				If hops_Renamed < HOPS AndAlso (isFirst Or isLast) Then Return

				' Squeeze out deleted nodes between activePred and
				' activeSucc, including x.
				skipDeletedSuccessors(activePred)
				skipDeletedPredecessors(activeSucc)

				' Try to gc-unlink, if possible
				If (isFirst Or isLast) AndAlso (activePred.next Is activeSucc) AndAlso (activeSucc.prev Is activePred) AndAlso (If(isFirst, activePred.prev Is Nothing, activePred.item IsNot Nothing)) AndAlso (If(isLast, activeSucc.next Is Nothing, activeSucc.item IsNot Nothing)) Then
					' Recheck expected state of predecessor and successor

					updateHead() ' Ensure x is not reachable from head
					updateTail() ' Ensure x is not reachable from tail

					' Finally, actually gc-unlink
					x.lazySetPrev(If(isFirst, prevTerminator(), x))
					x.lazySetNext(If(isLast, nextTerminator(), x))
				End If
			End If
		End Sub

		''' <summary>
		''' Unlinks non-null first node.
		''' </summary>
		Private Sub unlinkFirst(ByVal first As Node(Of E), ByVal [next] As Node(Of E))
			' assert first != null;
			' assert next != null;
			' assert first.item == null;
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			for (Node<E> o = Nothing, p = next, q;;)
				q = p.next
				If p.item IsNot Nothing OrElse q Is Nothing Then
					If o IsNot Nothing AndAlso p.prev IsNot p AndAlso first.casNext([next], p) Then
						skipDeletedPredecessors(p)
						If first.prev Is Nothing AndAlso (p.next Is Nothing OrElse p.item IsNot Nothing) AndAlso p.prev Is first Then

							updateHead() ' Ensure o is not reachable from head
							updateTail() ' Ensure o is not reachable from tail

							' Finally, actually gc-unlink
							o.lazySetNext(o)
							o.lazySetPrev(prevTerminator())
						End If
					End If
					Return
				ElseIf p Is q Then
					Return
				Else
					o = p
					p = q
				End If
		End Sub

		''' <summary>
		''' Unlinks non-null last node.
		''' </summary>
		Private Sub unlinkLast(ByVal last As Node(Of E), ByVal prev As Node(Of E))
			' assert last != null;
			' assert prev != null;
			' assert last.item == null;
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			for (Node<E> o = Nothing, p = prev, q;;)
				q = p.prev
				If p.item IsNot Nothing OrElse q Is Nothing Then
					If o IsNot Nothing AndAlso p.next IsNot p AndAlso last.casPrev(prev, p) Then
						skipDeletedSuccessors(p)
						If last.next Is Nothing AndAlso (p.prev Is Nothing OrElse p.item IsNot Nothing) AndAlso p.next Is last Then

							updateHead() ' Ensure o is not reachable from head
							updateTail() ' Ensure o is not reachable from tail

							' Finally, actually gc-unlink
							o.lazySetPrev(o)
							o.lazySetNext(nextTerminator())
						End If
					End If
					Return
				ElseIf p Is q Then
					Return
				Else
					o = p
					p = q
				End If
		End Sub

		''' <summary>
		''' Guarantees that any node which was unlinked before a call to
		''' this method will be unreachable from head after it returns.
		''' Does not guarantee to eliminate slack, only that head will
		''' point to a node that was active while this method was running.
		''' </summary>
		Private Sub updateHead()
			' Either head already points to an active node, or we keep
			' trying to cas it to the first node until it does.
			Dim h As Node(Of E), p As Node(Of E), q As Node(Of E)
			restartFromHead:
			h = head
			p = h.prev
			Do While h .item Is Nothing AndAlso p IsNot Nothing
				Do
					q = p.prev
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					q = (p = q).prev
					If q Is Nothing OrElse q Is Nothing Then
						' It is possible that p is PREV_TERMINATOR,
						' but if so, the CAS is guaranteed to fail.
						If casHead(h, p) Then
							Return
						Else
							GoTo restartFromHead
						End If
					ElseIf h IsNot head Then
						GoTo restartFromHead
					Else
						p = q
					End If
				Loop
				h = head
				p = h.prev
			Loop
		End Sub

		''' <summary>
		''' Guarantees that any node which was unlinked before a call to
		''' this method will be unreachable from tail after it returns.
		''' Does not guarantee to eliminate slack, only that tail will
		''' point to a node that was active while this method was running.
		''' </summary>
		Private Sub updateTail()
			' Either tail already points to an active node, or we keep
			' trying to cas it to the last node until it does.
			Dim t As Node(Of E), p As Node(Of E), q As Node(Of E)
			restartFromTail:
			t = tail
			p = t.next
			Do While t .item Is Nothing AndAlso p IsNot Nothing
				Do
					q = p.next
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					q = (p = q).next
					If q Is Nothing OrElse q Is Nothing Then
						' It is possible that p is NEXT_TERMINATOR,
						' but if so, the CAS is guaranteed to fail.
						If casTail(t, p) Then
							Return
						Else
							GoTo restartFromTail
						End If
					ElseIf t IsNot tail Then
						GoTo restartFromTail
					Else
						p = q
					End If
				Loop
				t = tail
				p = t.next
			Loop
		End Sub

		Private Sub skipDeletedPredecessors(ByVal x As Node(Of E))
			whileActive:
			Do
				Dim prev As Node(Of E) = x.prev
				' assert prev != null;
				' assert x != NEXT_TERMINATOR;
				' assert x != PREV_TERMINATOR;
				Dim p As Node(Of E) = prev
				findActive:
				Do
					If p.item IsNot Nothing Then GoTo findActive
					Dim q As Node(Of E) = p.prev
					If q Is Nothing Then
						If p.next Is p Then GoTo whileActive
						GoTo findActive
					ElseIf p Is q Then
						GoTo whileActive
					Else
						p = q
					End If
				Loop

				' found active CAS target
				If prev Is p OrElse x.casPrev(prev, p) Then Return

			Loop While x.item IsNot Nothing OrElse x.next Is Nothing
		End Sub

		Private Sub skipDeletedSuccessors(ByVal x As Node(Of E))
			whileActive:
			Do
				Dim [next] As Node(Of E) = x.next
				' assert next != null;
				' assert x != NEXT_TERMINATOR;
				' assert x != PREV_TERMINATOR;
				Dim p As Node(Of E) = [next]
				findActive:
				Do
					If p.item IsNot Nothing Then GoTo findActive
					Dim q As Node(Of E) = p.next
					If q Is Nothing Then
						If p.prev Is p Then GoTo whileActive
						GoTo findActive
					ElseIf p Is q Then
						GoTo whileActive
					Else
						p = q
					End If
				Loop

				' found active CAS target
				If [next] Is p OrElse x.casNext([next], p) Then Return

			Loop While x.item IsNot Nothing OrElse x.prev Is Nothing
		End Sub

		''' <summary>
		''' Returns the successor of p, or the first node if p.next has been
		''' linked to self, which will only be true if traversing with a
		''' stale pointer that is now off the list.
		''' </summary>
		Friend Function succ(ByVal p As Node(Of E)) As Node(Of E)
			' TODO: should we skip deleted nodes here?
			Dim q As Node(Of E) = p.next
			Return If(p Is q, first(), q)
		End Function

		''' <summary>
		''' Returns the predecessor of p, or the last node if p.prev has been
		''' linked to self, which will only be true if traversing with a
		''' stale pointer that is now off the list.
		''' </summary>
		Friend Function pred(ByVal p As Node(Of E)) As Node(Of E)
			Dim q As Node(Of E) = p.prev
			Return If(p Is q, last(), q)
		End Function

		''' <summary>
		''' Returns the first node, the unique node p for which:
		'''     p.prev == null && p.next != p
		''' The returned node may or may not be logically deleted.
		''' Guarantees that head is set to the returned node.
		''' </summary>
		Friend Overridable Function first() As Node(Of E)
			restartFromHead:
			Do
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				for (Node<E> h = head, p = h, q;;)
					q = p.prev
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					q = (p = q).prev
					If q IsNot Nothing AndAlso q IsNot Nothing Then
						' Check for head updates every other hop.
						' If p == q, we are sure to follow head instead.
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						p = If(h IsNot (h = head), h, q)
					ElseIf p Is h OrElse casHead(h, p) Then
							 ' It is possible that p is PREV_TERMINATOR,
							 ' but if so, the CAS is guaranteed to fail.
						Return p
					Else
						GoTo restartFromHead
					End If
			Loop
		End Function

		''' <summary>
		''' Returns the last node, the unique node p for which:
		'''     p.next == null && p.prev != p
		''' The returned node may or may not be logically deleted.
		''' Guarantees that tail is set to the returned node.
		''' </summary>
		Friend Overridable Function last() As Node(Of E)
			restartFromTail:
			Do
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				for (Node<E> t = tail, p = t, q;;)
					q = p.next
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					q = (p = q).next
					If q IsNot Nothing AndAlso q IsNot Nothing Then
						' Check for tail updates every other hop.
						' If p == q, we are sure to follow tail instead.
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						p = If(t IsNot (t = tail), t, q)
					ElseIf p Is t OrElse casTail(t, p) Then
							 ' It is possible that p is NEXT_TERMINATOR,
							 ' but if so, the CAS is guaranteed to fail.
						Return p
					Else
						GoTo restartFromTail
					End If
			Loop
		End Function

		' Minor convenience utilities

		''' <summary>
		''' Throws NullPointerException if argument is null.
		''' </summary>
		''' <param name="v"> the element </param>
		Private Shared Sub checkNotNull(ByVal v As Object)
			If v Is Nothing Then Throw New NullPointerException
		End Sub

		''' <summary>
		''' Returns element unless it is null, in which case throws
		''' NoSuchElementException.
		''' </summary>
		''' <param name="v"> the element </param>
		''' <returns> the element </returns>
		Private Function screenNullResult(ByVal v As E) As E
			If v Is Nothing Then Throw New java.util.NoSuchElementException
			Return v
		End Function

		''' <summary>
		''' Creates an array list and fills it with elements of this list.
		''' Used by toArray.
		''' </summary>
		''' <returns> the array list </returns>
		Private Function toArrayList() As List(Of E)
			Dim list As New List(Of E)
			Dim p As Node(Of E) = first()
			Do While p IsNot Nothing
				Dim item As E = p.item
				If item IsNot Nothing Then list.add(item)
				p = succ(p)
			Loop
			Return list
		End Function

		''' <summary>
		''' Constructs an empty deque.
		''' </summary>
		Public Sub New()
				tail = New Node(Of E)(Nothing)
				head = tail
		End Sub

		''' <summary>
		''' Constructs a deque initially containing the elements of
		''' the given collection, added in traversal order of the
		''' collection's iterator.
		''' </summary>
		''' <param name="c"> the collection of elements to initially contain </param>
		''' <exception cref="NullPointerException"> if the specified collection or any
		'''         of its elements are null </exception>
		Public Sub New(Of T1 As E)(ByVal c As ICollection(Of T1))
			' Copy c into a private chain of Nodes
			Dim h As Node(Of E) = Nothing, t As Node(Of E) = Nothing
			For Each e As E In c
				checkNotNull(e)
				Dim newNode As New Node(Of E)(e)
				If h Is Nothing Then
						t = newNode
						h = t
				Else
					t.lazySetNext(newNode)
					newNode.lazySetPrev(t)
					t = newNode
				End If
			Next e
			initHeadTail(h, t)
		End Sub

		''' <summary>
		''' Initializes head and tail, ensuring invariants hold.
		''' </summary>
		Private Sub initHeadTail(ByVal h As Node(Of E), ByVal t As Node(Of E))
			If h Is t Then
				If h Is Nothing Then
						t = New Node(Of E)(Nothing)
						h = t
				Else
					' Avoid edge case of a single Node with non-null item.
					Dim newNode As New Node(Of E)(Nothing)
					t.lazySetNext(newNode)
					newNode.lazySetPrev(t)
					t = newNode
				End If
			End If
			head = h
			tail = t
		End Sub

		''' <summary>
		''' Inserts the specified element at the front of this deque.
		''' As the deque is unbounded, this method will never throw
		''' <seealso cref="IllegalStateException"/>.
		''' </summary>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Sub addFirst(ByVal e As E)
			linkFirst(e)
		End Sub

		''' <summary>
		''' Inserts the specified element at the end of this deque.
		''' As the deque is unbounded, this method will never throw
		''' <seealso cref="IllegalStateException"/>.
		''' 
		''' <p>This method is equivalent to <seealso cref="#add"/>.
		''' </summary>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Sub addLast(ByVal e As E)
			linkLast(e)
		End Sub

		''' <summary>
		''' Inserts the specified element at the front of this deque.
		''' As the deque is unbounded, this method will never return {@code false}.
		''' </summary>
		''' <returns> {@code true} (as specified by <seealso cref="Deque#offerFirst"/>) </returns>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function offerFirst(ByVal e As E) As Boolean
			linkFirst(e)
			Return True
		End Function

		''' <summary>
		''' Inserts the specified element at the end of this deque.
		''' As the deque is unbounded, this method will never return {@code false}.
		''' 
		''' <p>This method is equivalent to <seealso cref="#add"/>.
		''' </summary>
		''' <returns> {@code true} (as specified by <seealso cref="Deque#offerLast"/>) </returns>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function offerLast(ByVal e As E) As Boolean
			linkLast(e)
			Return True
		End Function

		Public Overridable Function peekFirst() As E
			Dim p As Node(Of E) = first()
			Do While p IsNot Nothing
				Dim item As E = p.item
				If item IsNot Nothing Then Return item
				p = succ(p)
			Loop
			Return Nothing
		End Function

		Public Overridable Function peekLast() As E
			Dim p As Node(Of E) = last()
			Do While p IsNot Nothing
				Dim item As E = p.item
				If item IsNot Nothing Then Return item
				p = pred(p)
			Loop
			Return Nothing
		End Function

		''' <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		Public Overridable Property first As E
			Get
				Return screenNullResult(peekFirst())
			End Get
		End Property

		''' <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		Public Overridable Property last As E
			Get
				Return screenNullResult(peekLast())
			End Get
		End Property

		Public Overridable Function pollFirst() As E
			Dim p As Node(Of E) = first()
			Do While p IsNot Nothing
				Dim item As E = p.item
				If item IsNot Nothing AndAlso p.casItem(item, Nothing) Then
					unlink(p)
					Return item
				End If
				p = succ(p)
			Loop
			Return Nothing
		End Function

		Public Overridable Function pollLast() As E
			Dim p As Node(Of E) = last()
			Do While p IsNot Nothing
				Dim item As E = p.item
				If item IsNot Nothing AndAlso p.casItem(item, Nothing) Then
					unlink(p)
					Return item
				End If
				p = pred(p)
			Loop
			Return Nothing
		End Function

		''' <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		Public Overridable Function removeFirst() As E
			Return screenNullResult(pollFirst())
		End Function

		''' <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		Public Overridable Function removeLast() As E
			Return screenNullResult(pollLast())
		End Function

		' *** Queue and stack methods ***

		''' <summary>
		''' Inserts the specified element at the tail of this deque.
		''' As the deque is unbounded, this method will never return {@code false}.
		''' </summary>
		''' <returns> {@code true} (as specified by <seealso cref="Queue#offer"/>) </returns>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function offer(ByVal e As E) As Boolean
			Return offerLast(e)
		End Function

		''' <summary>
		''' Inserts the specified element at the tail of this deque.
		''' As the deque is unbounded, this method will never throw
		''' <seealso cref="IllegalStateException"/> or return {@code false}.
		''' </summary>
		''' <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function add(ByVal e As E) As Boolean
			Return offerLast(e)
		End Function

		Public Overridable Function poll() As E
			Return pollFirst()
		End Function
		Public Overridable Function peek() As E
			Return peekFirst()
		End Function

		''' <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		Public Overridable Function remove() As E
			Return removeFirst()
		End Function

		''' <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		Public Overridable Function pop() As E
			Return removeFirst()
		End Function

		''' <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		Public Overridable Function element() As E
			Return first
		End Function

		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Sub push(ByVal e As E)
			addFirst(e)
		End Sub

		''' <summary>
		''' Removes the first element {@code e} such that
		''' {@code o.equals(e)}, if such an element exists in this deque.
		''' If the deque does not contain the element, it is unchanged.
		''' </summary>
		''' <param name="o"> element to be removed from this deque, if present </param>
		''' <returns> {@code true} if the deque contained the specified element </returns>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function removeFirstOccurrence(ByVal o As Object) As Boolean
			checkNotNull(o)
			Dim p As Node(Of E) = first()
			Do While p IsNot Nothing
				Dim item As E = p.item
				If item IsNot Nothing AndAlso o.Equals(item) AndAlso p.casItem(item, Nothing) Then
					unlink(p)
					Return True
				End If
				p = succ(p)
			Loop
			Return False
		End Function

		''' <summary>
		''' Removes the last element {@code e} such that
		''' {@code o.equals(e)}, if such an element exists in this deque.
		''' If the deque does not contain the element, it is unchanged.
		''' </summary>
		''' <param name="o"> element to be removed from this deque, if present </param>
		''' <returns> {@code true} if the deque contained the specified element </returns>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function removeLastOccurrence(ByVal o As Object) As Boolean
			checkNotNull(o)
			Dim p As Node(Of E) = last()
			Do While p IsNot Nothing
				Dim item As E = p.item
				If item IsNot Nothing AndAlso o.Equals(item) AndAlso p.casItem(item, Nothing) Then
					unlink(p)
					Return True
				End If
				p = pred(p)
			Loop
			Return False
		End Function

		''' <summary>
		''' Returns {@code true} if this deque contains at least one
		''' element {@code e} such that {@code o.equals(e)}.
		''' </summary>
		''' <param name="o"> element whose presence in this deque is to be tested </param>
		''' <returns> {@code true} if this deque contains the specified element </returns>
		Public Overridable Function contains(ByVal o As Object) As Boolean
			If o Is Nothing Then Return False
			Dim p As Node(Of E) = first()
			Do While p IsNot Nothing
				Dim item As E = p.item
				If item IsNot Nothing AndAlso o.Equals(item) Then Return True
				p = succ(p)
			Loop
			Return False
		End Function

		''' <summary>
		''' Returns {@code true} if this collection contains no elements.
		''' </summary>
		''' <returns> {@code true} if this collection contains no elements </returns>
		Public Overridable Property empty As Boolean
			Get
				Return peekFirst() Is Nothing
			End Get
		End Property

		''' <summary>
		''' Returns the number of elements in this deque.  If this deque
		''' contains more than {@code Integer.MAX_VALUE} elements, it
		''' returns {@code Integer.MAX_VALUE}.
		''' 
		''' <p>Beware that, unlike in most collections, this method is
		''' <em>NOT</em> a constant-time operation. Because of the
		''' asynchronous nature of these deques, determining the current
		''' number of elements requires traversing them all to count them.
		''' Additionally, it is possible for the size to change during
		''' execution of this method, in which case the returned result
		''' will be inaccurate. Thus, this method is typically not very
		''' useful in concurrent applications.
		''' </summary>
		''' <returns> the number of elements in this deque </returns>
		Public Overridable Function size() As Integer
			Dim count As Integer = 0
			Dim p As Node(Of E) = first()
			Do While p IsNot Nothing
				If p.item IsNot Nothing Then
					' Collection.size() spec says to max out
					count += 1
					If count = Integer.MaxValue Then Exit Do
				End If
				p = succ(p)
			Loop
			Return count
		End Function

		''' <summary>
		''' Removes the first element {@code e} such that
		''' {@code o.equals(e)}, if such an element exists in this deque.
		''' If the deque does not contain the element, it is unchanged.
		''' </summary>
		''' <param name="o"> element to be removed from this deque, if present </param>
		''' <returns> {@code true} if the deque contained the specified element </returns>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function remove(ByVal o As Object) As Boolean
			Return removeFirstOccurrence(o)
		End Function

		''' <summary>
		''' Appends all of the elements in the specified collection to the end of
		''' this deque, in the order that they are returned by the specified
		''' collection's iterator.  Attempts to {@code addAll} of a deque to
		''' itself result in {@code IllegalArgumentException}.
		''' </summary>
		''' <param name="c"> the elements to be inserted into this deque </param>
		''' <returns> {@code true} if this deque changed as a result of the call </returns>
		''' <exception cref="NullPointerException"> if the specified collection or any
		'''         of its elements are null </exception>
		''' <exception cref="IllegalArgumentException"> if the collection is this deque </exception>
		Public Overridable Function addAll(Of T1 As E)(ByVal c As ICollection(Of T1)) As Boolean
			If c Is Me Then Throw New IllegalArgumentException

			' Copy c into a private chain of Nodes
			Dim beginningOfTheEnd As Node(Of E) = Nothing, last_Renamed As Node(Of E) = Nothing
			For Each e As E In c
				checkNotNull(e)
				Dim newNode As New Node(Of E)(e)
				If beginningOfTheEnd Is Nothing Then
						last_Renamed = newNode
						beginningOfTheEnd = last_Renamed
				Else
					last_Renamed.lazySetNext(newNode)
					newNode.lazySetPrev(last_Renamed)
					last_Renamed = newNode
				End If
			Next e
			If beginningOfTheEnd Is Nothing Then Return False

			' Atomically append the chain at the tail of this collection
			restartFromTail:
			Do
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				for (Node<E> t = tail, p = t, q;;)
					q = p.next
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					q = (p = q).next
					If q IsNot Nothing AndAlso q IsNot Nothing Then
						' Check for tail updates every other hop.
						' If p == q, we are sure to follow tail instead.
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						p = If(t IsNot (t = tail), t, q)
					ElseIf p.prev Is p Then ' NEXT_TERMINATOR
						GoTo restartFromTail
					Else
						' p is last node
						beginningOfTheEnd.lazySetPrev(p) ' CAS piggyback
						If p.casNext(Nothing, beginningOfTheEnd) Then
							' Successful CAS is the linearization point
							' for all elements to be added to this deque.
							If Not casTail(t, last_Renamed) Then
								' Try a little harder to update tail,
								' since we may be adding many elements.
								t = tail
								If last_Renamed.next Is Nothing Then casTail(t, last_Renamed)
							End If
							Return True
						End If
						' Lost CAS race to another thread; re-read next
					End If
			Loop
		End Function

		''' <summary>
		''' Removes all of the elements from this deque.
		''' </summary>
		Public Overridable Sub clear()
			Do While pollFirst() IsNot Nothing

			Loop
		End Sub

		''' <summary>
		''' Returns an array containing all of the elements in this deque, in
		''' proper sequence (from first to last element).
		''' 
		''' <p>The returned array will be "safe" in that no references to it are
		''' maintained by this deque.  (In other words, this method must allocate
		''' a new array).  The caller is thus free to modify the returned array.
		''' 
		''' <p>This method acts as bridge between array-based and collection-based
		''' APIs.
		''' </summary>
		''' <returns> an array containing all of the elements in this deque </returns>
		Public Overridable Function toArray() As Object()
			Return toArrayList().ToArray()
		End Function

		''' <summary>
		''' Returns an array containing all of the elements in this deque,
		''' in proper sequence (from first to last element); the runtime
		''' type of the returned array is that of the specified array.  If
		''' the deque fits in the specified array, it is returned therein.
		''' Otherwise, a new array is allocated with the runtime type of
		''' the specified array and the size of this deque.
		''' 
		''' <p>If this deque fits in the specified array with room to spare
		''' (i.e., the array has more elements than this deque), the element in
		''' the array immediately following the end of the deque is set to
		''' {@code null}.
		''' 
		''' <p>Like the <seealso cref="#toArray()"/> method, this method acts as
		''' bridge between array-based and collection-based APIs.  Further,
		''' this method allows precise control over the runtime type of the
		''' output array, and may, under certain circumstances, be used to
		''' save allocation costs.
		''' 
		''' <p>Suppose {@code x} is a deque known to contain only strings.
		''' The following code can be used to dump the deque into a newly
		''' allocated array of {@code String}:
		''' 
		'''  <pre> {@code String[] y = x.toArray(new String[0]);}</pre>
		''' 
		''' Note that {@code toArray(new Object[0])} is identical in function to
		''' {@code toArray()}.
		''' </summary>
		''' <param name="a"> the array into which the elements of the deque are to
		'''          be stored, if it is big enough; otherwise, a new array of the
		'''          same runtime type is allocated for this purpose </param>
		''' <returns> an array containing all of the elements in this deque </returns>
		''' <exception cref="ArrayStoreException"> if the runtime type of the specified array
		'''         is not a supertype of the runtime type of every element in
		'''         this deque </exception>
		''' <exception cref="NullPointerException"> if the specified array is null </exception>
		Public Overridable Function toArray(Of T)(ByVal a As T()) As T()
			Return toArrayList().ToArray(a)
		End Function

		''' <summary>
		''' Returns an iterator over the elements in this deque in proper sequence.
		''' The elements will be returned in order from first (head) to last (tail).
		''' 
		''' <p>The returned iterator is
		''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		''' </summary>
		''' <returns> an iterator over the elements in this deque in proper sequence </returns>
		Public Overridable Function [iterator]() As IEnumerator(Of E)
			Return New Itr(Me)
		End Function

		''' <summary>
		''' Returns an iterator over the elements in this deque in reverse
		''' sequential order.  The elements will be returned in order from
		''' last (tail) to first (head).
		''' 
		''' <p>The returned iterator is
		''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		''' </summary>
		''' <returns> an iterator over the elements in this deque in reverse order </returns>
		Public Overridable Function descendingIterator() As IEnumerator(Of E)
			Return New DescendingItr(Me)
		End Function

		Private MustInherit Class AbstractItr
			Implements IEnumerator(Of E)

			Private ReadOnly outerInstance As ConcurrentLinkedDeque

			''' <summary>
			''' Next node to return item for.
			''' </summary>
			Private nextNode_Renamed As Node(Of E)

			''' <summary>
			''' nextItem holds on to item fields because once we claim
			''' that an element exists in hasNext(), we must return it in
			''' the following next() call even if it was in the process of
			''' being removed when hasNext() was called.
			''' </summary>
			Private nextItem As E

			''' <summary>
			''' Node returned by most recent call to next. Needed by remove.
			''' Reset to null if this element is deleted by a call to remove.
			''' </summary>
			Private lastRet As Node(Of E)

			Friend MustOverride Function startNode() As Node(Of E)
			Friend MustOverride Function nextNode(ByVal p As Node(Of E)) As Node(Of E)

			Friend Sub New(ByVal outerInstance As ConcurrentLinkedDeque)
					Me.outerInstance = outerInstance
				advance()
			End Sub

			''' <summary>
			''' Sets nextNode and nextItem to next valid node, or to null
			''' if no such.
			''' </summary>
			Private Sub advance()
				lastRet = nextNode_Renamed

				Dim p As Node(Of E) = If(nextNode_Renamed Is Nothing, startNode(), nextNode(nextNode_Renamed))
				Do
					If p Is Nothing Then
						' p might be active end or TERMINATOR node; both are OK
						nextNode_Renamed = Nothing
						nextItem = Nothing
						Exit Do
					End If
					Dim item As E = p.item
					If item IsNot Nothing Then
						nextNode_Renamed = p
						nextItem = item
						Exit Do
					End If
					p = nextNode(p)
				Loop
			End Sub

			Public Overridable Function hasNext() As Boolean
				Return nextItem IsNot Nothing
			End Function

			Public Overridable Function [next]() As E
				Dim item As E = nextItem
				If item Is Nothing Then Throw New java.util.NoSuchElementException
				advance()
				Return item
			End Function

			Public Overridable Sub remove()
				Dim l As Node(Of E) = lastRet
				If l Is Nothing Then Throw New IllegalStateException
				l.item = Nothing
				outerInstance.unlink(l)
				lastRet = Nothing
			End Sub
		End Class

		''' <summary>
		''' Forward iterator </summary>
		Private Class Itr
			Inherits AbstractItr

			Private ReadOnly outerInstance As ConcurrentLinkedDeque

			Public Sub New(ByVal outerInstance As ConcurrentLinkedDeque)
				Me.outerInstance = outerInstance
			End Sub

			Friend Overrides Function startNode() As Node(Of E)
				Return outerInstance.first()
			End Function
			Friend Overrides Function nextNode(ByVal p As Node(Of E)) As Node(Of E)
				Return outerInstance.succ(p)
			End Function
		End Class

		''' <summary>
		''' Descending iterator </summary>
		Private Class DescendingItr
			Inherits AbstractItr

			Private ReadOnly outerInstance As ConcurrentLinkedDeque

			Public Sub New(ByVal outerInstance As ConcurrentLinkedDeque)
				Me.outerInstance = outerInstance
			End Sub

			Friend Overrides Function startNode() As Node(Of E)
				Return outerInstance.last()
			End Function
			Friend Overrides Function nextNode(ByVal p As Node(Of E)) As Node(Of E)
				Return outerInstance.pred(p)
			End Function
		End Class

		''' <summary>
		''' A customized variant of Spliterators.IteratorSpliterator </summary>
		Friend NotInheritable Class CLDSpliterator(Of E)
			Implements java.util.Spliterator(Of E)

			Friend Shared ReadOnly MAX_BATCH As Integer = 1 << 25 ' max batch array size;
			Friend ReadOnly queue As ConcurrentLinkedDeque(Of E)
			Friend current As Node(Of E) ' current node; null until initialized
			Friend batch As Integer ' batch size for splits
			Friend exhausted As Boolean ' true when no more nodes
			Friend Sub New(ByVal queue As ConcurrentLinkedDeque(Of E))
				Me.queue = queue
			End Sub

			Public Function trySplit() As java.util.Spliterator(Of E)
				Dim p As Node(Of E)
				Dim q As ConcurrentLinkedDeque(Of E) = Me.queue
				Dim b As Integer = batch
				Dim n As Integer = If(b <= 0, 1, If(b >= MAX_BATCH, MAX_BATCH, b + 1))
				p = current
				p = q.first()
				If (Not exhausted) AndAlso (p IsNot Nothing OrElse p IsNot Nothing) Then
					p = p.next
					If p.item Is Nothing AndAlso p Is p Then
							p = q.first()
							current = p
					End If
					If p IsNot Nothing AndAlso p.next IsNot Nothing Then
						Dim a As Object() = New Object(n - 1){}
						Dim i As Integer = 0
						Do
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							If (a(i) = p.item) IsNot Nothing Then i += 1
							p = p.next
							If p Is p Then p = q.first()
						Loop While p IsNot Nothing AndAlso i < n
						current = p
						If current Is Nothing Then exhausted = True
						If i > 0 Then
							batch = i
							Return java.util.Spliterators.spliterator(a, 0, i, java.util.Spliterator.ORDERED Or java.util.Spliterator.NONNULL Or java.util.Spliterator.CONCURRENT)
						End If
					End If
				End If
				Return Nothing
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				Dim p As Node(Of E)
				If action Is Nothing Then Throw New NullPointerException
				Dim q As ConcurrentLinkedDeque(Of E) = Me.queue
				p = current
				p = q.first()
				If (Not exhausted) AndAlso (p IsNot Nothing OrElse p IsNot Nothing) Then
					exhausted = True
					Do
						Dim e As E = p.item
						p = p.next
						If p Is p Then p = q.first()
						If e IsNot Nothing Then action.accept(e)
					Loop While p IsNot Nothing
				End If
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean
				Dim p As Node(Of E)
				If action Is Nothing Then Throw New NullPointerException
				Dim q As ConcurrentLinkedDeque(Of E) = Me.queue
				p = current
				p = q.first()
				If (Not exhausted) AndAlso (p IsNot Nothing OrElse p IsNot Nothing) Then
					Dim e As E
					Do
						e = p.item
						p = p.next
						If p Is p Then p = q.first()
					Loop While e Is Nothing AndAlso p IsNot Nothing
					current = p
					If current Is Nothing Then exhausted = True
					If e IsNot Nothing Then
						action.accept(e)
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
		''' Returns a <seealso cref="Spliterator"/> over the elements in this deque.
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
		''' <returns> a {@code Spliterator} over the elements in this deque
		''' @since 1.8 </returns>
		Public Overridable Function spliterator() As java.util.Spliterator(Of E)
			Return New CLDSpliterator(Of E)(Me)
		End Function

		''' <summary>
		''' Saves this deque to a stream (that is, serializes it).
		''' </summary>
		''' <param name="s"> the stream </param>
		''' <exception cref="java.io.IOException"> if an I/O error occurs
		''' @serialData All of the elements (each an {@code E}) in
		''' the proper order, followed by a null </exception>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)

			' Write out any hidden stuff
			s.defaultWriteObject()

			' Write out all elements in the proper order.
			Dim p As Node(Of E) = first()
			Do While p IsNot Nothing
				Dim item As E = p.item
				If item IsNot Nothing Then s.writeObject(item)
				p = succ(p)
			Loop

			' Use trailing null as sentinel
			s.writeObject(Nothing)
		End Sub

		''' <summary>
		''' Reconstitutes this deque from a stream (that is, deserializes it). </summary>
		''' <param name="s"> the stream </param>
		''' <exception cref="ClassNotFoundException"> if the class of a serialized object
		'''         could not be found </exception>
		''' <exception cref="java.io.IOException"> if an I/O error occurs </exception>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()

			' Read in elements until trailing null sentinel found
			Dim h As Node(Of E) = Nothing, t As Node(Of E) = Nothing
			Dim item As Object
			item = s.readObject()
			Do While item IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim newNode As New Node(Of E)(CType(item, E))
				If h Is Nothing Then
						t = newNode
						h = t
				Else
					t.lazySetNext(newNode)
					newNode.lazySetPrev(t)
					t = newNode
				End If
				item = s.readObject()
			Loop
			initHeadTail(h, t)
		End Sub

		Private Function casHead(ByVal cmp As Node(Of E), ByVal val As Node(Of E)) As Boolean
			Return UNSAFE.compareAndSwapObject(Me, headOffset, cmp, val)
		End Function

		Private Function casTail(ByVal cmp As Node(Of E), ByVal val As Node(Of E)) As Boolean
			Return UNSAFE.compareAndSwapObject(Me, tailOffset, cmp, val)
		End Function

		' Unsafe mechanics

		Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
		Private Shared ReadOnly headOffset As Long
		Private Shared ReadOnly tailOffset As Long
		Shared Sub New()
			PREV_TERMINATOR = New Node(Of Object)
			PREV_TERMINATOR.next = PREV_TERMINATOR
			NEXT_TERMINATOR = New Node(Of Object)
			NEXT_TERMINATOR.prev = NEXT_TERMINATOR
			Try
				UNSAFE = sun.misc.Unsafe.unsafe
				Dim k As Class = GetType(ConcurrentLinkedDeque)
				headOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("head"))
				tailOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("tail"))
			Catch e As Exception
				Throw New [Error](e)
			End Try
		End Sub
	End Class

End Namespace
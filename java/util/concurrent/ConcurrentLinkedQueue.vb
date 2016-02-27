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
	''' An unbounded thread-safe <seealso cref="Queue queue"/> based on linked nodes.
	''' This queue orders elements FIFO (first-in-first-out).
	''' The <em>head</em> of the queue is that element that has been on the
	''' queue the longest time.
	''' The <em>tail</em> of the queue is that element that has been on the
	''' queue the shortest time. New elements
	''' are inserted at the tail of the queue, and the queue retrieval
	''' operations obtain elements at the head of the queue.
	''' A {@code ConcurrentLinkedQueue} is an appropriate choice when
	''' many threads will share access to a common collection.
	''' Like most other concurrent collection implementations, this class
	''' does not permit the use of {@code null} elements.
	''' 
	''' <p>This implementation employs an efficient <em>non-blocking</em>
	''' algorithm based on one described in <a
	''' href="http://www.cs.rochester.edu/u/michael/PODC96.html"> Simple,
	''' Fast, and Practical Non-Blocking and Blocking Concurrent Queue
	''' Algorithms</a> by Maged M. Michael and Michael L. Scott.
	''' 
	''' <p>Iterators are <i>weakly consistent</i>, returning elements
	''' reflecting the state of the queue at some point at or since the
	''' creation of the iterator.  They do <em>not</em> throw {@link
	''' java.util.ConcurrentModificationException}, and may proceed concurrently
	''' with other operations.  Elements contained in the queue since the creation
	''' of the iterator will be returned exactly once.
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
	''' <p>This class and its iterator implement all of the <em>optional</em>
	''' methods of the <seealso cref="Queue"/> and <seealso cref="Iterator"/> interfaces.
	''' 
	''' <p>Memory consistency effects: As with other concurrent
	''' collections, actions in a thread prior to placing an object into a
	''' {@code ConcurrentLinkedQueue}
	''' <a href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	''' actions subsequent to the access or removal of that element from
	''' the {@code ConcurrentLinkedQueue} in another thread.
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @since 1.5
	''' @author Doug Lea </summary>
	''' @param <E> the type of elements held in this collection </param>
	<Serializable> _
	Public Class ConcurrentLinkedQueue(Of E)
		Inherits java.util.AbstractQueue(Of E)
		Implements LinkedList(Of E)

		Private Const serialVersionUID As Long = 196745693267521676L

	'    
	'     * This is a modification of the Michael & Scott algorithm,
	'     * adapted for a garbage-collected environment, with support for
	'     * interior node deletion (to support remove(Object)).  For
	'     * explanation, read the paper.
	'     *
	'     * Note that like most non-blocking algorithms in this package,
	'     * this implementation relies on the fact that in garbage
	'     * collected systems, there is no possibility of ABA problems due
	'     * to recycled nodes, so there is no need to use "counted
	'     * pointers" or related techniques seen in versions used in
	'     * non-GC'ed settings.
	'     *
	'     * The fundamental invariants are:
	'     * - There is exactly one (last) Node with a null next reference,
	'     *   which is CASed when enqueueing.  This last Node can be
	'     *   reached in O(1) time from tail, but tail is merely an
	'     *   optimization - it can always be reached in O(N) time from
	'     *   head as well.
	'     * - The elements contained in the queue are the non-null items in
	'     *   Nodes that are reachable from head.  CASing the item
	'     *   reference of a Node to null atomically removes it from the
	'     *   queue.  Reachability of all elements from head must remain
	'     *   true even in the case of concurrent modifications that cause
	'     *   head to advance.  A dequeued Node may remain in use
	'     *   indefinitely due to creation of an Iterator or simply a
	'     *   poll() that has lost its time slice.
	'     *
	'     * The above might appear to imply that all Nodes are GC-reachable
	'     * from a predecessor dequeued Node.  That would cause two problems:
	'     * - allow a rogue Iterator to cause unbounded memory retention
	'     * - cause cross-generational linking of old Nodes to new Nodes if
	'     *   a Node was tenured while live, which generational GCs have a
	'     *   hard time dealing with, causing repeated major collections.
	'     * However, only non-deleted Nodes need to be reachable from
	'     * dequeued Nodes, and reachability does not necessarily have to
	'     * be of the kind understood by the GC.  We use the trick of
	'     * linking a Node that has just been dequeued to itself.  Such a
	'     * self-link implicitly means to advance to head.
	'     *
	'     * Both head and tail are permitted to lag.  In fact, failing to
	'     * update them every time one could is a significant optimization
	'     * (fewer CASes). As with LinkedTransferQueue (see the internal
	'     * documentation for that [Class]), we use a slack threshold of two;
	'     * that is, we update head/tail when the current pointer appears
	'     * to be two or more steps away from the first/last node.
	'     *
	'     * Since head and tail are updated concurrently and independently,
	'     * it is possible for tail to lag behind head (why not)?
	'     *
	'     * CASing a Node's item reference to null atomically removes the
	'     * element from the queue.  Iterators skip over Nodes with null
	'     * items.  Prior implementations of this class had a race between
	'     * poll() and remove(Object) where the same element would appear
	'     * to be successfully removed by two concurrent operations.  The
	'     * method remove(Object) also lazily unlinks deleted Nodes, but
	'     * this is merely an optimization.
	'     *
	'     * When constructing a Node (before enqueuing it) we avoid paying
	'     * for a volatile write to item by using Unsafe.putObject instead
	'     * of a normal write.  This allows the cost of enqueue to be
	'     * "one-and-a-half" CASes.
	'     *
	'     * Both head and tail may or may not point to a Node with a
	'     * non-null item.  If the queue is empty, all items must of course
	'     * be null.  Upon creation, both head and tail refer to a dummy
	'     * Node with null item.  Both head and tail are only updated using
	'     * CAS, so they never regress, although again this is merely an
	'     * optimization.
	'     

		Private Class Node(Of E)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend item As E
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend [next] As Node(Of E)

			''' <summary>
			''' Constructs a new node.  Uses relaxed write because item can
			''' only be seen after publication via casNext.
			''' </summary>
			Friend Sub New(ByVal item As E)
				UNSAFE.putObject(Me, itemOffset, item)
			End Sub

			Friend Overridable Function casItem(ByVal cmp As E, ByVal val As E) As Boolean
				Return UNSAFE.compareAndSwapObject(Me, itemOffset, cmp, val)
			End Function

			Friend Overridable Sub lazySetNext(ByVal val As Node(Of E))
				UNSAFE.putOrderedObject(Me, nextOffset, val)
			End Sub

			Friend Overridable Function casNext(ByVal cmp As Node(Of E), ByVal val As Node(Of E)) As Boolean
				Return UNSAFE.compareAndSwapObject(Me, nextOffset, cmp, val)
			End Function

			' Unsafe mechanics

			Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
			Private Shared ReadOnly itemOffset As Long
			Private Shared ReadOnly nextOffset As Long

			Shared Sub New()
				Try
					UNSAFE = sun.misc.Unsafe.unsafe
					Dim k As  [Class] = GetType(Node)
					itemOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("item"))
					nextOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("next"))
				Catch e As Exception
					Throw New [Error](e)
				End Try
			End Sub
		End Class

		''' <summary>
		''' A node from which the first live (non-deleted) node (if any)
		''' can be reached in O(1) time.
		''' Invariants:
		''' - all live nodes are reachable from head via succ()
		''' - head != null
		''' - (tmp = head).next != tmp || tmp != head
		''' Non-invariants:
		''' - head.item may or may not be null.
		''' - it is permitted for tail to lag behind head, that is, for tail
		'''   to not be reachable from head!
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private head As Node(Of E)

		''' <summary>
		''' A node from which the last node on list (that is, the unique
		''' node with node.next == null) can be reached in O(1) time.
		''' Invariants:
		''' - the last node is always reachable from tail via succ()
		''' - tail != null
		''' Non-invariants:
		''' - tail.item may or may not be null.
		''' - it is permitted for tail to lag behind head, that is, for tail
		'''   to not be reachable from head!
		''' - tail.next may or may not be self-pointing to tail.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private tail As Node(Of E)

		''' <summary>
		''' Creates a {@code ConcurrentLinkedQueue} that is initially empty.
		''' </summary>
		Public Sub New()
				tail = New Node(Of E)(Nothing)
				head = tail
		End Sub

		''' <summary>
		''' Creates a {@code ConcurrentLinkedQueue}
		''' initially containing the elements of the given collection,
		''' added in traversal order of the collection's iterator.
		''' </summary>
		''' <param name="c"> the collection of elements to initially contain </param>
		''' <exception cref="NullPointerException"> if the specified collection or any
		'''         of its elements are null </exception>
		Public Sub New(Of T1 As E)(ByVal c As ICollection(Of T1))
			Dim h As Node(Of E) = Nothing, t As Node(Of E) = Nothing
			For Each e As E In c
				checkNotNull(e)
				Dim newNode As New Node(Of E)(e)
				If h Is Nothing Then
						t = newNode
						h = t
				Else
					t.lazySetNext(newNode)
					t = newNode
				End If
			Next e
			If h Is Nothing Then
					t = New Node(Of E)(Nothing)
					h = t
			End If
			head = h
			tail = t
		End Sub

		' Have to override just to update the javadoc

		''' <summary>
		''' Inserts the specified element at the tail of this queue.
		''' As the queue is unbounded, this method will never throw
		''' <seealso cref="IllegalStateException"/> or return {@code false}.
		''' </summary>
		''' <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function add(ByVal e As E) As Boolean
			Return offer(e)
		End Function

		''' <summary>
		''' Tries to CAS head to p. If successful, repoint old head to itself
		''' as sentinel for succ(), below.
		''' </summary>
		Friend Sub updateHead(ByVal h As Node(Of E), ByVal p As Node(Of E))
			If h IsNot p AndAlso casHead(h, p) Then h.lazySetNext(h)
		End Sub

		''' <summary>
		''' Returns the successor of p, or the head node if p.next has been
		''' linked to self, which will only be true if traversing with a
		''' stale pointer that is now off the list.
		''' </summary>
		Friend Function succ(ByVal p As Node(Of E)) As Node(Of E)
			Dim [next] As Node(Of E) = p.next
			Return If(p Is [next], head, [next])
		End Function

		''' <summary>
		''' Inserts the specified element at the tail of this queue.
		''' As the queue is unbounded, this method will never return {@code false}.
		''' </summary>
		''' <returns> {@code true} (as specified by <seealso cref="Queue#offer"/>) </returns>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function offer(ByVal e As E) As Boolean
			checkNotNull(e)
			Dim newNode As New Node(Of E)(e)

			Dim t As Node(Of E) = tail
			Dim p As Node(Of E) = t
			Do
				Dim q As Node(Of E) = p.next
				If q Is Nothing Then
					' p is last node
					If p.casNext(Nothing, newNode) Then
						' Successful CAS is the linearization point
						' for e to become an element of this queue,
						' and for newNode to become "live".
						If p IsNot t Then ' hop two nodes at a time casTail(t, newNode) ' Failure is OK.
						Return True
					End If
					' Lost CAS race to another thread; re-read next
				ElseIf p Is q Then
					' We have fallen off list.  If tail is unchanged, it
					' will also be off-list, in which case we need to
					' jump to head, from which all live nodes are always
					' reachable.  Else the new tail is a better bet.
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					p = If(t IsNot (t = tail), t, head)
				Else
					' Check for tail updates after two hops.
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					p = If(p IsNot t AndAlso t IsNot (t = tail), t, q)
				End If
			Loop
		End Function

		Public Overridable Function poll() As E
			restartFromHead:
			Do
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				for (Node<E> h = head, p = h, q;;)
					Dim item As E = p.item

					If item IsNot Nothing AndAlso p.casItem(item, Nothing) Then
						' Successful CAS is the linearization point
						' for item to be removed from this queue.
						If p IsNot h Then ' hop two nodes at a time updateHead(h,If((q = p.next) IsNot Nothing, q, p))
						Return item
					Else
						q = p.next
						If q Is Nothing Then
							updateHead(h, p)
							Return Nothing
						ElseIf p Is q Then
							GoTo restartFromHead
						Else
							p = q
						End If
						End If
			Loop
		End Function

		Public Overridable Function peek() As E
			restartFromHead:
			Do
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				for (Node<E> h = head, p = h, q;;)
					Dim item As E = p.item
					q = p.next
					If item IsNot Nothing OrElse q Is Nothing Then
						updateHead(h, p)
						Return item
					ElseIf p Is q Then
						GoTo restartFromHead
					Else
						p = q
					End If
			Loop
		End Function

		''' <summary>
		''' Returns the first live (non-deleted) node on list, or null if none.
		''' This is yet another variant of poll/peek; here returning the
		''' first node, not element.  We could make peek() a wrapper around
		''' first(), but that would cost an extra volatile read of item,
		''' and the need to add a retry loop to deal with the possibility
		''' of losing a race to a concurrent poll().
		''' </summary>
		Friend Overridable Function first() As Node(Of E)
			restartFromHead:
			Do
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				for (Node<E> h = head, p = h, q;;)
					Dim hasItem As Boolean = (p.item IsNot Nothing)
					q = p.next
					If hasItem OrElse q Is Nothing Then
						updateHead(h, p)
						Return If(hasItem, p, Nothing)
					ElseIf p Is q Then
						GoTo restartFromHead
					Else
						p = q
					End If
			Loop
		End Function

		''' <summary>
		''' Returns {@code true} if this queue contains no elements.
		''' </summary>
		''' <returns> {@code true} if this queue contains no elements </returns>
		Public Overridable Property empty As Boolean
			Get
				Return first() Is Nothing
			End Get
		End Property

		''' <summary>
		''' Returns the number of elements in this queue.  If this queue
		''' contains more than {@code  java.lang.[Integer].MAX_VALUE} elements, returns
		''' {@code  java.lang.[Integer].MAX_VALUE}.
		''' 
		''' <p>Beware that, unlike in most collections, this method is
		''' <em>NOT</em> a constant-time operation. Because of the
		''' asynchronous nature of these queues, determining the current
		''' number of elements requires an O(n) traversal.
		''' Additionally, if elements are added or removed during execution
		''' of this method, the returned result may be inaccurate.  Thus,
		''' this method is typically not very useful in concurrent
		''' applications.
		''' </summary>
		''' <returns> the number of elements in this queue </returns>
		Public Overridable Function size() As Integer
			Dim count As Integer = 0
			Dim p As Node(Of E) = first()
			Do While p IsNot Nothing
				If p.item IsNot Nothing Then
					' Collection.size() spec says to max out
					count += 1
					If count =  java.lang.[Integer].Max_Value Then Exit Do
				End If
				p = succ(p)
			Loop
			Return count
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
			Dim p As Node(Of E) = first()
			Do While p IsNot Nothing
				Dim item As E = p.item
				If item IsNot Nothing AndAlso o.Equals(item) Then Return True
				p = succ(p)
			Loop
			Return False
		End Function

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
			If o Is Nothing Then Return False
			Dim pred As Node(Of E) = Nothing
			Dim p As Node(Of E) = first()
			Do While p IsNot Nothing
				Dim item As E = p.item
				If item IsNot Nothing AndAlso o.Equals(item) AndAlso p.casItem(item, Nothing) Then
					Dim [next] As Node(Of E) = succ(p)
					If pred IsNot Nothing AndAlso [next] IsNot Nothing Then pred.casNext(p, [next])
					Return True
				End If
				pred = p
				p = succ(p)
			Loop
			Return False
		End Function

		''' <summary>
		''' Appends all of the elements in the specified collection to the end of
		''' this queue, in the order that they are returned by the specified
		''' collection's iterator.  Attempts to {@code addAll} of a queue to
		''' itself result in {@code IllegalArgumentException}.
		''' </summary>
		''' <param name="c"> the elements to be inserted into this queue </param>
		''' <returns> {@code true} if this queue changed as a result of the call </returns>
		''' <exception cref="NullPointerException"> if the specified collection or any
		'''         of its elements are null </exception>
		''' <exception cref="IllegalArgumentException"> if the collection is this queue </exception>
		Public Overridable Function addAll(Of T1 As E)(ByVal c As ICollection(Of T1)) As Boolean
			If c Is Me Then Throw New IllegalArgumentException

			' Copy c into a private chain of Nodes
			Dim beginningOfTheEnd As Node(Of E) = Nothing, last As Node(Of E) = Nothing
			For Each e As E In c
				checkNotNull(e)
				Dim newNode As New Node(Of E)(e)
				If beginningOfTheEnd Is Nothing Then
						last = newNode
						beginningOfTheEnd = last
				Else
					last.lazySetNext(newNode)
					last = newNode
				End If
			Next e
			If beginningOfTheEnd Is Nothing Then Return False

			' Atomically append the chain at the tail of this collection
			Dim t As Node(Of E) = tail
			Dim p As Node(Of E) = t
			Do
				Dim q As Node(Of E) = p.next
				If q Is Nothing Then
					' p is last node
					If p.casNext(Nothing, beginningOfTheEnd) Then
						' Successful CAS is the linearization point
						' for all elements to be added to this queue.
						If Not casTail(t, last) Then
							' Try a little harder to update tail,
							' since we may be adding many elements.
							t = tail
							If last.next Is Nothing Then casTail(t, last)
						End If
						Return True
					End If
					' Lost CAS race to another thread; re-read next
				ElseIf p Is q Then
					' We have fallen off list.  If tail is unchanged, it
					' will also be off-list, in which case we need to
					' jump to head, from which all live nodes are always
					' reachable.  Else the new tail is a better bet.
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					p = If(t IsNot (t = tail), t, head)
				Else
					' Check for tail updates after two hops.
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					p = If(p IsNot t AndAlso t IsNot (t = tail), t, q)
				End If
			Loop
		End Function

		''' <summary>
		''' Returns an array containing all of the elements in this queue, in
		''' proper sequence.
		''' 
		''' <p>The returned array will be "safe" in that no references to it are
		''' maintained by this queue.  (In other words, this method must allocate
		''' a new array).  The caller is thus free to modify the returned array.
		''' 
		''' <p>This method acts as bridge between array-based and collection-based
		''' APIs.
		''' </summary>
		''' <returns> an array containing all of the elements in this queue </returns>
		Public Overridable Function toArray() As Object()
			' Use ArrayList to deal with resizing.
			Dim al As New List(Of E)
			Dim p As Node(Of E) = first()
			Do While p IsNot Nothing
				Dim item As E = p.item
				If item IsNot Nothing Then al.add(item)
				p = succ(p)
			Loop
			Return al.ToArray()
		End Function

		''' <summary>
		''' Returns an array containing all of the elements in this queue, in
		''' proper sequence; the runtime type of the returned array is that of
		''' the specified array.  If the queue fits in the specified array, it
		''' is returned therein.  Otherwise, a new array is allocated with the
		''' runtime type of the specified array and the size of this queue.
		''' 
		''' <p>If this queue fits in the specified array with room to spare
		''' (i.e., the array has more elements than this queue), the element in
		''' the array immediately following the end of the queue is set to
		''' {@code null}.
		''' 
		''' <p>Like the <seealso cref="#toArray()"/> method, this method acts as bridge between
		''' array-based and collection-based APIs.  Further, this method allows
		''' precise control over the runtime type of the output array, and may,
		''' under certain circumstances, be used to save allocation costs.
		''' 
		''' <p>Suppose {@code x} is a queue known to contain only strings.
		''' The following code can be used to dump the queue into a newly
		''' allocated array of {@code String}:
		''' 
		'''  <pre> {@code String[] y = x.toArray(new String[0]);}</pre>
		''' 
		''' Note that {@code toArray(new Object[0])} is identical in function to
		''' {@code toArray()}.
		''' </summary>
		''' <param name="a"> the array into which the elements of the queue are to
		'''          be stored, if it is big enough; otherwise, a new array of the
		'''          same runtime type is allocated for this purpose </param>
		''' <returns> an array containing all of the elements in this queue </returns>
		''' <exception cref="ArrayStoreException"> if the runtime type of the specified array
		'''         is not a supertype of the runtime type of every element in
		'''         this queue </exception>
		''' <exception cref="NullPointerException"> if the specified array is null </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function toArray(Of T)(ByVal a As T()) As T()
			' try to use sent-in array
			Dim k As Integer = 0
			Dim p As Node(Of E)
			p = first()
			Do While p IsNot Nothing AndAlso k < a.Length
				Dim item As E = p.item
				If item IsNot Nothing Then
					a(k) = CType(item, T)
					k += 1
				End If
				p = succ(p)
			Loop
			If p Is Nothing Then
				If k < a.Length Then a(k) = Nothing
				Return a
			End If

			' If won't fit, use ArrayList version
			Dim al As New List(Of E)
			Dim q As Node(Of E) = first()
			Do While q IsNot Nothing
				Dim item As E = q.item
				If item IsNot Nothing Then al.add(item)
				q = succ(q)
			Loop
			Return al.ToArray(a)
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

		Private Class Itr
			Implements IEnumerator(Of E)

			Private ReadOnly outerInstance As ConcurrentLinkedQueue

			''' <summary>
			''' Next node to return item for.
			''' </summary>
			Private nextNode As Node(Of E)

			''' <summary>
			''' nextItem holds on to item fields because once we claim
			''' that an element exists in hasNext(), we must return it in
			''' the following next() call even if it was in the process of
			''' being removed when hasNext() was called.
			''' </summary>
			Private nextItem As E

			''' <summary>
			''' Node of the last returned item, to support remove.
			''' </summary>
			Private lastRet As Node(Of E)

			Friend Sub New(ByVal outerInstance As ConcurrentLinkedQueue)
					Me.outerInstance = outerInstance
				advance()
			End Sub

			''' <summary>
			''' Moves to next valid node and returns item to return for
			''' next(), or null if no such.
			''' </summary>
			Private Function advance() As E
				lastRet = nextNode
				Dim x As E = nextItem

				Dim pred As Node(Of E), p As Node(Of E)
				If nextNode Is Nothing Then
					p = outerInstance.first()
					pred = Nothing
				Else
					pred = nextNode
					p = outerInstance.succ(nextNode)
				End If

				Do
					If p Is Nothing Then
						nextNode = Nothing
						nextItem = Nothing
						Return x
					End If
					Dim item As E = p.item
					If item IsNot Nothing Then
						nextNode = p
						nextItem = item
						Return x
					Else
						' skip over nulls
						Dim [next] As Node(Of E) = outerInstance.succ(p)
						If pred IsNot Nothing AndAlso [next] IsNot Nothing Then pred.casNext(p, [next])
						p = [next]
					End If
				Loop
			End Function

			Public Overridable Function hasNext() As Boolean
				Return nextNode IsNot Nothing
			End Function

			Public Overridable Function [next]() As E
				If nextNode Is Nothing Then Throw New java.util.NoSuchElementException
				Return advance()
			End Function

			Public Overridable Sub remove()
				Dim l As Node(Of E) = lastRet
				If l Is Nothing Then Throw New IllegalStateException
				' rely on a future traversal to relink.
				l.item = Nothing
				lastRet = Nothing
			End Sub
		End Class

		''' <summary>
		''' Saves this queue to a stream (that is, serializes it).
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
				Dim item As Object = p.item
				If item IsNot Nothing Then s.writeObject(item)
				p = succ(p)
			Loop

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
					t = newNode
				End If
				item = s.readObject()
			Loop
			If h Is Nothing Then
					t = New Node(Of E)(Nothing)
					h = t
			End If
			head = h
			tail = t
		End Sub

		''' <summary>
		''' A customized variant of Spliterators.IteratorSpliterator </summary>
		Friend NotInheritable Class CLQSpliterator(Of E)
			Implements java.util.Spliterator(Of E)

			Friend Shared ReadOnly MAX_BATCH As Integer = 1 << 25 ' max batch array size;
			Friend ReadOnly queue As ConcurrentLinkedQueue(Of E)
			Friend current As Node(Of E) ' current node; null until initialized
			Friend batch As Integer ' batch size for splits
			Friend exhausted As Boolean ' true when no more nodes
			Friend Sub New(ByVal queue As ConcurrentLinkedQueue(Of E))
				Me.queue = queue
			End Sub

			Public Function trySplit() As java.util.Spliterator(Of E)
				Dim p As Node(Of E)
				Dim q As ConcurrentLinkedQueue(Of E) = Me.queue
				Dim b As Integer = batch
				Dim n As Integer = If(b <= 0, 1, If(b >= MAX_BATCH, MAX_BATCH, b + 1))
				p = current
				p = q.first()
				If (Not exhausted) AndAlso (p IsNot Nothing OrElse p IsNot Nothing) AndAlso p.next IsNot Nothing Then
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
				Return Nothing
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				Dim p As Node(Of E)
				If action Is Nothing Then Throw New NullPointerException
				Dim q As ConcurrentLinkedQueue(Of E) = Me.queue
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
				Dim q As ConcurrentLinkedQueue(Of E) = Me.queue
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
				Return java.lang.[Long].Max_Value
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
		Public Overrides Function spliterator() As java.util.Spliterator(Of E)
			Return New CLQSpliterator(Of E)(Me)
		End Function

		''' <summary>
		''' Throws NullPointerException if argument is null.
		''' </summary>
		''' <param name="v"> the element </param>
		Private Shared Sub checkNotNull(ByVal v As Object)
			If v Is Nothing Then Throw New NullPointerException
		End Sub

		Private Function casTail(ByVal cmp As Node(Of E), ByVal val As Node(Of E)) As Boolean
			Return UNSAFE.compareAndSwapObject(Me, tailOffset, cmp, val)
		End Function

		Private Function casHead(ByVal cmp As Node(Of E), ByVal val As Node(Of E)) As Boolean
			Return UNSAFE.compareAndSwapObject(Me, headOffset, cmp, val)
		End Function

		' Unsafe mechanics

		Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
		Private Shared ReadOnly headOffset As Long
		Private Shared ReadOnly tailOffset As Long
		Shared Sub New()
			Try
				UNSAFE = sun.misc.Unsafe.unsafe
				Dim k As  [Class] = GetType(ConcurrentLinkedQueue)
				headOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("head"))
				tailOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("tail"))
			Catch e As Exception
				Throw New [Error](e)
			End Try
		End Sub
	End Class

End Namespace
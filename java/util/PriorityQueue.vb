Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util


	''' <summary>
	''' An unbounded priority <seealso cref="Queue queue"/> based on a priority heap.
	''' The elements of the priority queue are ordered according to their
	''' <seealso cref="Comparable natural ordering"/>, or by a <seealso cref="Comparator"/>
	''' provided at queue construction time, depending on which constructor is
	''' used.  A priority queue does not permit {@code null} elements.
	''' A priority queue relying on natural ordering also does not permit
	''' insertion of non-comparable objects (doing so may result in
	''' {@code ClassCastException}).
	''' 
	''' <p>The <em>head</em> of this queue is the <em>least</em> element
	''' with respect to the specified ordering.  If multiple elements are
	''' tied for least value, the head is one of those elements -- ties are
	''' broken arbitrarily.  The queue retrieval operations {@code poll},
	''' {@code remove}, {@code peek}, and {@code element} access the
	''' element at the head of the queue.
	''' 
	''' <p>A priority queue is unbounded, but has an internal
	''' <i>capacity</i> governing the size of an array used to store the
	''' elements on the queue.  It is always at least as large as the queue
	''' size.  As elements are added to a priority queue, its capacity
	''' grows automatically.  The details of the growth policy are not
	''' specified.
	''' 
	''' <p>This class and its iterator implement all of the
	''' <em>optional</em> methods of the <seealso cref="Collection"/> and {@link
	''' Iterator} interfaces.  The Iterator provided in method {@link
	''' #iterator()} is <em>not</em> guaranteed to traverse the elements of
	''' the priority queue in any particular order. If you need ordered
	''' traversal, consider using {@code Arrays.sort(pq.toArray())}.
	''' 
	''' <p><strong>Note that this implementation is not synchronized.</strong>
	''' Multiple threads should not access a {@code PriorityQueue}
	''' instance concurrently if any of the threads modifies the queue.
	''' Instead, use the thread-safe {@link
	''' java.util.concurrent.PriorityBlockingQueue} class.
	''' 
	''' <p>Implementation note: this implementation provides
	''' O(log(n)) time for the enqueuing and dequeuing methods
	''' ({@code offer}, {@code poll}, {@code remove()} and {@code add});
	''' linear time for the {@code remove(Object)} and {@code contains(Object)}
	''' methods; and constant time for the retrieval methods
	''' ({@code peek}, {@code element}, and {@code size}).
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @since 1.5
	''' @author Josh Bloch, Doug Lea </summary>
	''' @param <E> the type of elements held in this collection </param>
	<Serializable> _
	Public Class PriorityQueue(Of E)
		Inherits AbstractQueue(Of E)

		Private Const serialVersionUID As Long = -7720805057305804111L

		Private Const DEFAULT_INITIAL_CAPACITY As Integer = 11

		''' <summary>
		''' Priority queue represented as a balanced binary heap: the two
		''' children of queue[n] are queue[2*n+1] and queue[2*(n+1)].  The
		''' priority queue is ordered by comparator, or by the elements'
		''' natural ordering, if comparator is null: For each node n in the
		''' heap and each descendant d of n, n <= d.  The element with the
		''' lowest value is in queue[0], assuming the queue is nonempty.
		''' </summary>
		<NonSerialized> _
		Friend queue As Object() ' non-private to simplify nested class access

		''' <summary>
		''' The number of elements in the priority queue.
		''' </summary>
		Private size_Renamed As Integer = 0

		''' <summary>
		''' The comparator, or null if priority queue uses elements'
		''' natural ordering.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private ReadOnly comparator_Renamed As Comparator(Of ?)

		''' <summary>
		''' The number of times this priority queue has been
		''' <i>structurally modified</i>.  See AbstractList for gory details.
		''' </summary>
		<NonSerialized> _
		Friend modCount As Integer = 0 ' non-private to simplify nested class access

		''' <summary>
		''' Creates a {@code PriorityQueue} with the default initial
		''' capacity (11) that orders its elements according to their
		''' <seealso cref="Comparable natural ordering"/>.
		''' </summary>
		Public Sub New()
			Me.New(DEFAULT_INITIAL_CAPACITY, Nothing)
		End Sub

		''' <summary>
		''' Creates a {@code PriorityQueue} with the specified initial
		''' capacity that orders its elements according to their
		''' <seealso cref="Comparable natural ordering"/>.
		''' </summary>
		''' <param name="initialCapacity"> the initial capacity for this priority queue </param>
		''' <exception cref="IllegalArgumentException"> if {@code initialCapacity} is less
		'''         than 1 </exception>
		Public Sub New(  initialCapacity As Integer)
			Me.New(initialCapacity, Nothing)
		End Sub

		''' <summary>
		''' Creates a {@code PriorityQueue} with the default initial capacity and
		''' whose elements are ordered according to the specified comparator.
		''' </summary>
		''' <param name="comparator"> the comparator that will be used to order this
		'''         priority queue.  If {@code null}, the {@link Comparable
		'''         natural ordering} of the elements will be used.
		''' @since 1.8 </param>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Sub New(Of T1)(  comparator As Comparator(Of T1))
			Me.New(DEFAULT_INITIAL_CAPACITY, comparator)
		End Sub

		''' <summary>
		''' Creates a {@code PriorityQueue} with the specified initial capacity
		''' that orders its elements according to the specified comparator.
		''' </summary>
		''' <param name="initialCapacity"> the initial capacity for this priority queue </param>
		''' <param name="comparator"> the comparator that will be used to order this
		'''         priority queue.  If {@code null}, the {@link Comparable
		'''         natural ordering} of the elements will be used. </param>
		''' <exception cref="IllegalArgumentException"> if {@code initialCapacity} is
		'''         less than 1 </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Sub New(Of T1)(  initialCapacity As Integer,   comparator As Comparator(Of T1))
			' Note: This restriction of at least one is not actually needed,
			' but continues for 1.5 compatibility
			If initialCapacity < 1 Then Throw New IllegalArgumentException
			Me.queue = New Object(initialCapacity - 1){}
			Me.comparator_Renamed = comparator
		End Sub

		''' <summary>
		''' Creates a {@code PriorityQueue} containing the elements in the
		''' specified collection.  If the specified collection is an instance of
		''' a <seealso cref="SortedSet"/> or is another {@code PriorityQueue}, this
		''' priority queue will be ordered according to the same ordering.
		''' Otherwise, this priority queue will be ordered according to the
		''' <seealso cref="Comparable natural ordering"/> of its elements.
		''' </summary>
		''' <param name="c"> the collection whose elements are to be placed
		'''         into this priority queue </param>
		''' <exception cref="ClassCastException"> if elements of the specified collection
		'''         cannot be compared to one another according to the priority
		'''         queue's ordering </exception>
		''' <exception cref="NullPointerException"> if the specified collection or any
		'''         of its elements are null </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(Of T1 As E)(  c As Collection(Of T1))
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			If TypeOf c Is SortedSet(Of ?) Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim ss As SortedSet(Of ? As E) = CType(c, SortedSet(Of ? As E))
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Me.comparator_Renamed = CType(ss.comparator(), Comparator(Of ?))
				initElementsFromCollection(ss)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			ElseIf TypeOf c Is PriorityQueue(Of ?) Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim pq As PriorityQueue(Of ? As E) = CType(c, PriorityQueue(Of ? As E))
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Me.comparator_Renamed = CType(pq.comparator(), Comparator(Of ?))
				initFromPriorityQueue(pq)
			Else
				Me.comparator_Renamed = Nothing
				initFromCollection(c)
			End If
		End Sub

		''' <summary>
		''' Creates a {@code PriorityQueue} containing the elements in the
		''' specified priority queue.  This priority queue will be
		''' ordered according to the same ordering as the given priority
		''' queue.
		''' </summary>
		''' <param name="c"> the priority queue whose elements are to be placed
		'''         into this priority queue </param>
		''' <exception cref="ClassCastException"> if elements of {@code c} cannot be
		'''         compared to one another according to {@code c}'s
		'''         ordering </exception>
		''' <exception cref="NullPointerException"> if the specified priority queue or any
		'''         of its elements are null </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(Of T1 As E)(  c As PriorityQueue(Of T1))
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Me.comparator_Renamed = CType(c.comparator(), Comparator(Of ?))
			initFromPriorityQueue(c)
		End Sub

		''' <summary>
		''' Creates a {@code PriorityQueue} containing the elements in the
		''' specified sorted set.   This priority queue will be ordered
		''' according to the same ordering as the given sorted set.
		''' </summary>
		''' <param name="c"> the sorted set whose elements are to be placed
		'''         into this priority queue </param>
		''' <exception cref="ClassCastException"> if elements of the specified sorted
		'''         set cannot be compared to one another according to the
		'''         sorted set's ordering </exception>
		''' <exception cref="NullPointerException"> if the specified sorted set or any
		'''         of its elements are null </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(Of T1 As E)(  c As SortedSet(Of T1))
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Me.comparator_Renamed = CType(c.comparator(), Comparator(Of ?))
			initElementsFromCollection(c)
		End Sub

		Private Sub initFromPriorityQueue(Of T1 As E)(  c As PriorityQueue(Of T1))
			If c.GetType() Is GetType(PriorityQueue) Then
				Me.queue = c.ToArray()
				Me.size_Renamed = c.size()
			Else
				initFromCollection(c)
			End If
		End Sub

		Private Sub initElementsFromCollection(Of T1 As E)(  c As Collection(Of T1))
			Dim a As Object() = c.ToArray()
			' If c.toArray incorrectly doesn't return Object[], copy it.
			If a.GetType() IsNot GetType(Object()) Then a = Arrays.copyOf(a, a.Length, GetType(Object()))
			Dim len As Integer = a.Length
			If len = 1 OrElse Me.comparator_Renamed IsNot Nothing Then
				For i As Integer = 0 To len - 1
					If a(i) Is Nothing Then Throw New NullPointerException
				Next i
			End If
			Me.queue = a
			Me.size_Renamed = a.Length
		End Sub

		''' <summary>
		''' Initializes queue array with elements from the given Collection.
		''' </summary>
		''' <param name="c"> the collection </param>
		Private Sub initFromCollection(Of T1 As E)(  c As Collection(Of T1))
			initElementsFromCollection(c)
			heapify()
		End Sub

		''' <summary>
		''' The maximum size of array to allocate.
		''' Some VMs reserve some header words in an array.
		''' Attempts to allocate larger arrays may result in
		''' OutOfMemoryError: Requested array size exceeds VM limit
		''' </summary>
		Private Shared ReadOnly MAX_ARRAY_SIZE As Integer =  java.lang.[Integer].MAX_VALUE - 8

		''' <summary>
		''' Increases the capacity of the array.
		''' </summary>
		''' <param name="minCapacity"> the desired minimum capacity </param>
		Private Sub grow(  minCapacity As Integer)
			Dim oldCapacity As Integer = queue.Length
			' Double size if small; else grow by 50%
			Dim newCapacity As Integer = oldCapacity + (If(oldCapacity < 64, (oldCapacity + 2), (oldCapacity >> 1)))
			' overflow-conscious code
			If newCapacity - MAX_ARRAY_SIZE > 0 Then newCapacity = hugeCapacity(minCapacity)
			queue = New java.lang.Object(newCapacity - 1){}
			Array.Copy(queue, queue, newCapacity)
		End Sub

		Private Shared Function hugeCapacity(  minCapacity As Integer) As Integer
			If minCapacity < 0 Then ' overflow Throw New OutOfMemoryError
			Return If(minCapacity > MAX_ARRAY_SIZE,  java.lang.[Integer].Max_Value, MAX_ARRAY_SIZE)
		End Function

		''' <summary>
		''' Inserts the specified element into this priority queue.
		''' </summary>
		''' <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		''' <exception cref="ClassCastException"> if the specified element cannot be
		'''         compared with elements currently in this priority queue
		'''         according to the priority queue's ordering </exception>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function add(  e As E) As Boolean
			Return offer(e)
		End Function

		''' <summary>
		''' Inserts the specified element into this priority queue.
		''' </summary>
		''' <returns> {@code true} (as specified by <seealso cref="Queue#offer"/>) </returns>
		''' <exception cref="ClassCastException"> if the specified element cannot be
		'''         compared with elements currently in this priority queue
		'''         according to the priority queue's ordering </exception>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function offer(  e As E) As Boolean
			If e Is Nothing Then Throw New NullPointerException
			modCount += 1
			Dim i As Integer = size_Renamed
			If i >= queue.Length Then grow(i + 1)
			size_Renamed = i + 1
			If i = 0 Then
				queue(0) = e
			Else
				siftUp(i, e)
			End If
			Return True
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function peek() As E
			Return If(size_Renamed = 0, Nothing, CType(queue(0), E))
		End Function

		Private Function indexOf(  o As Object) As Integer
			If o IsNot Nothing Then
				For i As Integer = 0 To size_Renamed - 1
					If o.Equals(queue(i)) Then Return i
				Next i
			End If
			Return -1
		End Function

		''' <summary>
		''' Removes a single instance of the specified element from this queue,
		''' if it is present.  More formally, removes an element {@code e} such
		''' that {@code o.equals(e)}, if this queue contains one or more such
		''' elements.  Returns {@code true} if and only if this queue contained
		''' the specified element (or equivalently, if this queue changed as a
		''' result of the call).
		''' </summary>
		''' <param name="o"> element to be removed from this queue, if present </param>
		''' <returns> {@code true} if this queue changed as a result of the call </returns>
		Public Overridable Function remove(  o As Object) As Boolean
			Dim i As Integer = IndexOf(o)
			If i = -1 Then
				Return False
			Else
				removeAt(i)
				Return True
			End If
		End Function

		''' <summary>
		''' Version of remove using reference equality, not equals.
		''' Needed by iterator.remove.
		''' </summary>
		''' <param name="o"> element to be removed from this queue, if present </param>
		''' <returns> {@code true} if removed </returns>
		Friend Overridable Function removeEq(  o As Object) As Boolean
			For i As Integer = 0 To size_Renamed - 1
				If o Is queue(i) Then
					removeAt(i)
					Return True
				End If
			Next i
			Return False
		End Function

		''' <summary>
		''' Returns {@code true} if this queue contains the specified element.
		''' More formally, returns {@code true} if and only if this queue contains
		''' at least one element {@code e} such that {@code o.equals(e)}.
		''' </summary>
		''' <param name="o"> object to be checked for containment in this queue </param>
		''' <returns> {@code true} if this queue contains the specified element </returns>
		Public Overridable Function contains(  o As Object) As Boolean
			Return IndexOf(o) <> -1
		End Function

		''' <summary>
		''' Returns an array containing all of the elements in this queue.
		''' The elements are in no particular order.
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
			Return Arrays.copyOf(queue, size_Renamed)
		End Function

		''' <summary>
		''' Returns an array containing all of the elements in this queue; the
		''' runtime type of the returned array is that of the specified array.
		''' The returned array elements are in no particular order.
		''' If the queue fits in the specified array, it is returned therein.
		''' Otherwise, a new array is allocated with the runtime type of the
		''' specified array and the size of this queue.
		''' 
		''' <p>If the queue fits in the specified array with room to spare
		''' (i.e., the array has more elements than the queue), the element in
		''' the array immediately following the end of the collection is set to
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
		'''          same runtime type is allocated for this purpose. </param>
		''' <returns> an array containing all of the elements in this queue </returns>
		''' <exception cref="ArrayStoreException"> if the runtime type of the specified array
		'''         is not a supertype of the runtime type of every element in
		'''         this queue </exception>
		''' <exception cref="NullPointerException"> if the specified array is null </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function toArray(Of T)(  a As T()) As T()
			Dim size As Integer = Me.size_Renamed
			If a.Length < size Then Return CType(Arrays.copyOf(queue, size, a.GetType()), T())
			Array.Copy(queue, 0, a, 0, size)
			If a.Length > size Then a(size) = Nothing
			Return a
		End Function

		''' <summary>
		''' Returns an iterator over the elements in this queue. The iterator
		''' does not return the elements in any particular order.
		''' </summary>
		''' <returns> an iterator over the elements in this queue </returns>
		Public Overridable Function [iterator]() As [Iterator](Of E)
			Return New Itr(Me)
		End Function

		Private NotInheritable Class Itr
			Implements Iterator(Of E)

			Private ReadOnly outerInstance As PriorityQueue

			Public Sub New(  outerInstance As PriorityQueue)
				Me.outerInstance = outerInstance
			End Sub

			''' <summary>
			''' Index (into queue array) of element to be returned by
			''' subsequent call to next.
			''' </summary>
			Private cursor As Integer = 0

			''' <summary>
			''' Index of element returned by most recent call to next,
			''' unless that element came from the forgetMeNot list.
			''' Set to -1 if element is deleted by a call to remove.
			''' </summary>
			Private lastRet As Integer = -1

			''' <summary>
			''' A queue of elements that were moved from the unvisited portion of
			''' the heap into the visited portion as a result of "unlucky" element
			''' removals during the iteration.  (Unlucky element removals are those
			''' that require a siftup instead of a siftdown.)  We must visit all of
			''' the elements in this list to complete the iteration.  We do this
			''' after we've completed the "normal" iteration.
			''' 
			''' We expect that most iterations, even those involving removals,
			''' will not need to store elements in this field.
			''' </summary>
			Private forgetMeNot As ArrayDeque(Of E) = Nothing

			''' <summary>
			''' Element returned by the most recent call to next iff that
			''' element was drawn from the forgetMeNot list.
			''' </summary>
			Private lastRetElt As E = Nothing

			''' <summary>
			''' The modCount value that the iterator believes that the backing
			''' Queue should have.  If this expectation is violated, the iterator
			''' has detected concurrent modification.
			''' </summary>
			Private expectedModCount As Integer = outerInstance.modCount

			Public Function hasNext() As Boolean Implements Iterator(Of E).hasNext
				Return cursor < outerInstance.size_Renamed OrElse (forgetMeNot IsNot Nothing AndAlso (Not forgetMeNot.empty))
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Function [next]() As E Implements Iterator(Of E).next
				If expectedModCount <> outerInstance.modCount Then Throw New ConcurrentModificationException
				If cursor < outerInstance.size_Renamed Then Dim tempVar As Integer = cursor
						cursor += 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						Return CType(outerInstance.queue(lastRet = tempVar), E)
				If forgetMeNot IsNot Nothing Then
					lastRet = -1
					lastRetElt = forgetMeNot.poll()
					If lastRetElt IsNot Nothing Then Return lastRetElt
				End If
				Throw New NoSuchElementException
			End Function

			Public Sub remove() Implements Iterator(Of E).remove
				If expectedModCount <> outerInstance.modCount Then Throw New ConcurrentModificationException
				If lastRet <> -1 Then
					Dim moved As E = outerInstance.removeAt(lastRet)
					lastRet = -1
					If moved Is Nothing Then
						cursor -= 1
					Else
						If forgetMeNot Is Nothing Then forgetMeNot = New ArrayDeque(Of )
						forgetMeNot.add(moved)
					End If
				ElseIf lastRetElt IsNot Nothing Then
					outerInstance.removeEq(lastRetElt)
					lastRetElt = Nothing
				Else
					Throw New IllegalStateException
				End If
				expectedModCount = outerInstance.modCount
			End Sub
		End Class

		Public Overridable Function size() As Integer
			Return size_Renamed
		End Function

		''' <summary>
		''' Removes all of the elements from this priority queue.
		''' The queue will be empty after this call returns.
		''' </summary>
		Public Overridable Sub clear()
			modCount += 1
			Dim i As Integer = 0
			Do While i < size_Renamed
				queue(i) = Nothing
				i += 1
			Loop
			size_Renamed = 0
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function poll() As E
			If size_Renamed = 0 Then Return Nothing
			size_Renamed -= 1
			Dim s As Integer = size_Renamed
			modCount += 1
			Dim result As E = CType(queue(0), E)
			Dim x As E = CType(queue(s), E)
			queue(s) = Nothing
			If s <> 0 Then siftDown(0, x)
			Return result
		End Function

		''' <summary>
		''' Removes the ith element from queue.
		''' 
		''' Normally this method leaves the elements at up to i-1,
		''' inclusive, untouched.  Under these circumstances, it returns
		''' null.  Occasionally, in order to maintain the heap invariant,
		''' it must swap a later element of the list with one earlier than
		''' i.  Under these circumstances, this method returns the element
		''' that was previously at the end of the list and is now at some
		''' position before i. This fact is used by iterator.remove so as to
		''' avoid missing traversing elements.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Function removeAt(  i As Integer) As E
			' assert i >= 0 && i < size;
			modCount += 1
			size_Renamed -= 1
			Dim s As Integer = size_Renamed
			If s = i Then ' removed last element
				queue(i) = Nothing
			Else
				Dim moved As E = CType(queue(s), E)
				queue(s) = Nothing
				siftDown(i, moved)
				If queue(i) Is moved Then
					siftUp(i, moved)
					If queue(i) IsNot moved Then Return moved
				End If
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Inserts item x at position k, maintaining heap invariant by
		''' promoting x up the tree until it is greater than or equal to
		''' its parent, or is the root.
		''' 
		''' To simplify and speed up coercions and comparisons. the
		''' Comparable and Comparator versions are separated into different
		''' methods that are otherwise identical. (Similarly for siftDown.)
		''' </summary>
		''' <param name="k"> the position to fill </param>
		''' <param name="x"> the item to insert </param>
		Private Sub siftUp(  k As Integer,   x As E)
			If comparator_Renamed IsNot Nothing Then
				siftUpUsingComparator(k, x)
			Else
				siftUpComparable(k, x)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Sub siftUpComparable(  k As Integer,   x As E)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim key As Comparable(Of ?) = CType(x, Comparable(Of ?))
			Do While k > 0
				Dim parent As Integer = CInt(CUInt((k - 1)) >> 1)
				Dim e As Object = queue(parent)
				If key.CompareTo(CType(e, E)) >= 0 Then Exit Do
				queue(k) = e
				k = parent
			Loop
			queue(k) = key
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Sub siftUpUsingComparator(  k As Integer,   x As E)
			Do While k > 0
				Dim parent As Integer = CInt(CUInt((k - 1)) >> 1)
				Dim e As Object = queue(parent)
				If comparator_Renamed.Compare(x, CType(e, E)) >= 0 Then Exit Do
				queue(k) = e
				k = parent
			Loop
			queue(k) = x
		End Sub

		''' <summary>
		''' Inserts item x at position k, maintaining heap invariant by
		''' demoting x down the tree repeatedly until it is less than or
		''' equal to its children or is a leaf.
		''' </summary>
		''' <param name="k"> the position to fill </param>
		''' <param name="x"> the item to insert </param>
		Private Sub siftDown(  k As Integer,   x As E)
			If comparator_Renamed IsNot Nothing Then
				siftDownUsingComparator(k, x)
			Else
				siftDownComparable(k, x)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Sub siftDownComparable(  k As Integer,   x As E)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim key As Comparable(Of ?) = CType(x, Comparable(Of ?))
			Dim half As Integer = CInt(CUInt(size_Renamed) >> 1) ' loop while a non-leaf
			Do While k < half
				Dim child As Integer = (k << 1) + 1 ' assume left child is least
				Dim c As Object = queue(child)
				Dim right As Integer = child + 1
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				If right < size_Renamed AndAlso CType(c, Comparable(Of ?)).CompareTo(CType(queue(right), E)) > 0 Then c = queue(child = right)
				If key.CompareTo(CType(c, E)) <= 0 Then Exit Do
				queue(k) = c
				k = child
			Loop
			queue(k) = key
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Sub siftDownUsingComparator(  k As Integer,   x As E)
			Dim half As Integer = CInt(CUInt(size_Renamed) >> 1)
			Do While k < half
				Dim child As Integer = (k << 1) + 1
				Dim c As Object = queue(child)
				Dim right As Integer = child + 1
				If right < size_Renamed AndAlso comparator_Renamed.Compare(CType(c, E), CType(queue(right), E)) > 0 Then c = queue(child = right)
				If comparator_Renamed.Compare(x, CType(c, E)) <= 0 Then Exit Do
				queue(k) = c
				k = child
			Loop
			queue(k) = x
		End Sub

		''' <summary>
		''' Establishes the heap invariant (described above) in the entire tree,
		''' assuming nothing about the order of the elements prior to the call.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Sub heapify()
			For i As Integer = (CInt(CUInt(size_Renamed) >> 1)) - 1 To 0 Step -1
				siftDown(i, CType(queue(i), E))
			Next i
		End Sub

		''' <summary>
		''' Returns the comparator used to order the elements in this
		''' queue, or {@code null} if this queue is sorted according to
		''' the <seealso cref="Comparable natural ordering"/> of its elements.
		''' </summary>
		''' <returns> the comparator used to order this queue, or
		'''         {@code null} if this queue is sorted according to the
		'''         natural ordering of its elements </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Function comparator() As Comparator(Of ?)
			Return comparator_Renamed
		End Function

		''' <summary>
		''' Saves this queue to a stream (that is, serializes it).
		''' 
		''' @serialData The length of the array backing the instance is
		'''             emitted (int), followed by all of its elements
		'''             (each an {@code Object}) in the proper order. </summary>
		''' <param name="s"> the stream </param>
		Private Sub writeObject(  s As java.io.ObjectOutputStream)
			' Write out element count, and any hidden stuff
			s.defaultWriteObject()

			' Write out array length, for compatibility with 1.5 version
			s.writeInt (System.Math.Max(2, size_Renamed + 1))

			' Write out all elements in the "proper order".
			For i As Integer = 0 To size_Renamed - 1
				s.writeObject(queue(i))
			Next i
		End Sub

		''' <summary>
		''' Reconstitutes the {@code PriorityQueue} instance from a stream
		''' (that is, deserializes it).
		''' </summary>
		''' <param name="s"> the stream </param>
		Private Sub readObject(  s As java.io.ObjectInputStream)
			' Read in size, and any hidden stuff
			s.defaultReadObject()

			' Read in (and discard) array length
			s.readInt()

			queue = New Object(size_Renamed - 1){}

			' Read in all elements.
			For i As Integer = 0 To size_Renamed - 1
				queue(i) = s.readObject()
			Next i

			' Elements are guaranteed to be in "proper order", but the
			' spec has never explained what that might be.
			heapify()
		End Sub

		''' <summary>
		''' Creates a <em><a href="Spliterator.html#binding">late-binding</a></em>
		''' and <em>fail-fast</em> <seealso cref="Spliterator"/> over the elements in this
		''' queue.
		''' 
		''' <p>The {@code Spliterator} reports <seealso cref="Spliterator#SIZED"/>,
		''' <seealso cref="Spliterator#SUBSIZED"/>, and <seealso cref="Spliterator#NONNULL"/>.
		''' Overriding implementations should document the reporting of additional
		''' characteristic values.
		''' </summary>
		''' <returns> a {@code Spliterator} over the elements in this queue
		''' @since 1.8 </returns>
		Public Function spliterator() As Spliterator(Of E)
			Return New PriorityQueueSpliterator(Of E)(Me, 0, -1, 0)
		End Function

		Friend NotInheritable Class PriorityQueueSpliterator(Of E)
			Implements Spliterator(Of E)

	'        
	'         * This is very similar to ArrayList Spliterator, except for
	'         * extra null checks.
	'         
			Private ReadOnly pq As PriorityQueue(Of E)
			Private index As Integer ' current index, modified on advance/split
			Private fence As Integer ' -1 until first use
			Private expectedModCount As Integer ' initialized when fence set

			''' <summary>
			''' Creates new spliterator covering the given range </summary>
			Friend Sub New(  pq As PriorityQueue(Of E),   origin As Integer,   fence As Integer,   expectedModCount As Integer)
				Me.pq = pq
				Me.index = origin
				Me.fence = fence
				Me.expectedModCount = expectedModCount
			End Sub

			Private Property fence As Integer
				Get
					Dim hi As Integer
					hi = fence
					If hi < 0 Then
						expectedModCount = pq.modCount
							fence = pq.size
							hi = fence
					End If
					Return hi
				End Get
			End Property

			Public Function trySplit() As PriorityQueueSpliterator(Of E)
				Dim hi As Integer = fence, lo As Integer = index, mid As Integer = CInt(CUInt((lo + hi)) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If(lo >= mid, Nothing, New PriorityQueueSpliterator(Of E)(pq, lo, index = mid, expectedModCount))
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(  action As java.util.function.Consumer(Of T1)) Implements Spliterator(Of E).forEachRemaining
				Dim i, hi, mc As Integer ' hoist accesses and checks from loop
				Dim q As PriorityQueue(Of E)
				Dim a As Object()
				If action Is Nothing Then Throw New NullPointerException
				q = pq
				a = q.queue
				If q IsNot Nothing AndAlso a IsNot Nothing Then
					hi = fence
					If hi < 0 Then
						mc = q.modCount
						hi = q.size
					Else
						mc = expectedModCount
					End If
					i = index
					index = hi
					If i >= 0 AndAlso index <= a.Length Then
						Dim e As E
						Do
							If i < hi Then
								e = CType(a(i), E)
								If e Is Nothing Then ' must be CME Exit Do
								action.accept(e)
							ElseIf q.modCount <> mc Then
								Exit Do
							Else
								Return
							End If
							i += 1
						Loop
					End If
				End If
				Throw New ConcurrentModificationException
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(  action As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of E).tryAdvance
				If action Is Nothing Then Throw New NullPointerException
				Dim hi As Integer = fence, lo As Integer = index
				If lo >= 0 AndAlso lo < hi Then
					index = lo + 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim e As E = CType(pq.queue(lo), E)
					If e Is Nothing Then Throw New ConcurrentModificationException
					action.accept(e)
					If pq.modCount <> expectedModCount Then Throw New ConcurrentModificationException
					Return True
				End If
				Return False
			End Function

			Public Function estimateSize() As Long Implements Spliterator(Of E).estimateSize
				Return CLng(fence - index)
			End Function

			Public Function characteristics() As Integer Implements Spliterator(Of E).characteristics
				Return Spliterator.SIZED Or Spliterator.SUBSIZED Or Spliterator.NONNULL
			End Function
		End Class
	End Class

End Namespace
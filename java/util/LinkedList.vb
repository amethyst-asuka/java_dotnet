Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Doubly-linked list implementation of the {@code List} and {@code Deque}
	''' interfaces.  Implements all optional list operations, and permits all
	''' elements (including {@code null}).
	''' 
	''' <p>All of the operations perform as could be expected for a doubly-linked
	''' list.  Operations that index into the list will traverse the list from
	''' the beginning or the end, whichever is closer to the specified index.
	''' 
	''' <p><strong>Note that this implementation is not synchronized.</strong>
	''' If multiple threads access a linked list concurrently, and at least
	''' one of the threads modifies the list structurally, it <i>must</i> be
	''' synchronized externally.  (A structural modification is any operation
	''' that adds or deletes one or more elements; merely setting the value of
	''' an element is not a structural modification.)  This is typically
	''' accomplished by synchronizing on some object that naturally
	''' encapsulates the list.
	''' 
	''' If no such object exists, the list should be "wrapped" using the
	''' <seealso cref="Collections#synchronizedList Collections.synchronizedList"/>
	''' method.  This is best done at creation time, to prevent accidental
	''' unsynchronized access to the list:<pre>
	'''   List list = Collections.synchronizedList(new LinkedList(...));</pre>
	''' 
	''' <p>The iterators returned by this class's {@code iterator} and
	''' {@code listIterator} methods are <i>fail-fast</i>: if the list is
	''' structurally modified at any time after the iterator is created, in
	''' any way except through the Iterator's own {@code remove} or
	''' {@code add} methods, the iterator will throw a {@link
	''' ConcurrentModificationException}.  Thus, in the face of concurrent
	''' modification, the iterator fails quickly and cleanly, rather than
	''' risking arbitrary, non-deterministic behavior at an undetermined
	''' time in the future.
	''' 
	''' <p>Note that the fail-fast behavior of an iterator cannot be guaranteed
	''' as it is, generally speaking, impossible to make any hard guarantees in the
	''' presence of unsynchronized concurrent modification.  Fail-fast iterators
	''' throw {@code ConcurrentModificationException} on a best-effort basis.
	''' Therefore, it would be wrong to write a program that depended on this
	''' exception for its correctness:   <i>the fail-fast behavior of iterators
	''' should be used only to detect bugs.</i>
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @author  Josh Bloch </summary>
	''' <seealso cref=     List </seealso>
	''' <seealso cref=     ArrayList
	''' @since 1.2 </seealso>
	''' @param <E> the type of elements held in this collection </param>

	<Serializable> _
	Public Class LinkedList(Of E)
		Inherits AbstractSequentialList(Of E)
		Implements List(Of E), Deque(Of E), Cloneable

		<NonSerialized> _
		Friend size_Renamed As Integer = 0

		''' <summary>
		''' Pointer to first node.
		''' Invariant: (first == null && last == null) ||
		'''            (first.prev == null && first.item != null)
		''' </summary>
		<NonSerialized> _
		Friend first As Node(Of E)

		''' <summary>
		''' Pointer to last node.
		''' Invariant: (first == null && last == null) ||
		'''            (last.next == null && last.item != null)
		''' </summary>
		<NonSerialized> _
		Friend last As Node(Of E)

		''' <summary>
		''' Constructs an empty list.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a list containing the elements of the specified
		''' collection, in the order they are returned by the collection's
		''' iterator.
		''' </summary>
		''' <param name="c"> the collection whose elements are to be placed into this list </param>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
		Public Sub New(Of T1 As E)(  c As Collection(Of T1))
			Me.New()
			addAll(c)
		End Sub

		''' <summary>
		''' Links e as first element.
		''' </summary>
		Private Sub linkFirst(  e As E)
			Dim f As Node(Of E) = first
			Dim newNode As New Node(Of E)(Nothing, e, f)
			first = newNode
			If f Is Nothing Then
				last = newNode
			Else
				f.prev = newNode
			End If
			size_Renamed += 1
			modCount += 1
		End Sub

		''' <summary>
		''' Links e as last element.
		''' </summary>
		Friend Overridable Sub linkLast(  e As E)
			Dim l As Node(Of E) = last
			Dim newNode As New Node(Of E)(l, e, Nothing)
			last = newNode
			If l Is Nothing Then
				first = newNode
			Else
				l.next = newNode
			End If
			size_Renamed += 1
			modCount += 1
		End Sub

		''' <summary>
		''' Inserts element e before non-null Node succ.
		''' </summary>
		Friend Overridable Sub linkBefore(  e As E,   succ As Node(Of E))
			' assert succ != null;
			Dim pred As Node(Of E) = succ.prev
			Dim newNode As New Node(Of E)(pred, e, succ)
			succ.prev = newNode
			If pred Is Nothing Then
				first = newNode
			Else
				pred.next = newNode
			End If
			size_Renamed += 1
			modCount += 1
		End Sub

		''' <summary>
		''' Unlinks non-null first node f.
		''' </summary>
		Private Function unlinkFirst(  f As Node(Of E)) As E
			' assert f == first && f != null;
			Dim element As E = f.item
			Dim [next] As Node(Of E) = f.next
			f.item = Nothing
			f.next = Nothing ' help GC
			first = [next]
			If [next] Is Nothing Then
				last = Nothing
			Else
				[next].prev = Nothing
			End If
			size_Renamed -= 1
			modCount += 1
			Return element
		End Function

		''' <summary>
		''' Unlinks non-null last node l.
		''' </summary>
		Private Function unlinkLast(  l As Node(Of E)) As E
			' assert l == last && l != null;
			Dim element As E = l.item
			Dim prev As Node(Of E) = l.prev
			l.item = Nothing
			l.prev = Nothing ' help GC
			last = prev
			If prev Is Nothing Then
				first = Nothing
			Else
				prev.next = Nothing
			End If
			size_Renamed -= 1
			modCount += 1
			Return element
		End Function

		''' <summary>
		''' Unlinks non-null node x.
		''' </summary>
		Friend Overridable Function unlink(  x As Node(Of E)) As E
			' assert x != null;
			Dim element As E = x.item
			Dim [next] As Node(Of E) = x.next
			Dim prev As Node(Of E) = x.prev

			If prev Is Nothing Then
				first = [next]
			Else
				prev.next = [next]
				x.prev = Nothing
			End If

			If [next] Is Nothing Then
				last = prev
			Else
				[next].prev = prev
				x.next = Nothing
			End If

			x.item = Nothing
			size_Renamed -= 1
			modCount += 1
			Return element
		End Function

		''' <summary>
		''' Returns the first element in this list.
		''' </summary>
		''' <returns> the first element in this list </returns>
		''' <exception cref="NoSuchElementException"> if this list is empty </exception>
		Public Overridable Property first As E Implements Deque(Of E).getFirst
			Get
				Dim f As Node(Of E) = first
				If f Is Nothing Then Throw New NoSuchElementException
				Return f.item
			End Get
		End Property

		''' <summary>
		''' Returns the last element in this list.
		''' </summary>
		''' <returns> the last element in this list </returns>
		''' <exception cref="NoSuchElementException"> if this list is empty </exception>
		Public Overridable Property last As E Implements Deque(Of E).getLast
			Get
				Dim l As Node(Of E) = last
				If l Is Nothing Then Throw New NoSuchElementException
				Return l.item
			End Get
		End Property

		''' <summary>
		''' Removes and returns the first element from this list.
		''' </summary>
		''' <returns> the first element from this list </returns>
		''' <exception cref="NoSuchElementException"> if this list is empty </exception>
		Public Overridable Function removeFirst() As E Implements Deque(Of E).removeFirst
			Dim f As Node(Of E) = first
			If f Is Nothing Then Throw New NoSuchElementException
			Return unlinkFirst(f)
		End Function

		''' <summary>
		''' Removes and returns the last element from this list.
		''' </summary>
		''' <returns> the last element from this list </returns>
		''' <exception cref="NoSuchElementException"> if this list is empty </exception>
		Public Overridable Function removeLast() As E Implements Deque(Of E).removeLast
			Dim l As Node(Of E) = last
			If l Is Nothing Then Throw New NoSuchElementException
			Return unlinkLast(l)
		End Function

		''' <summary>
		''' Inserts the specified element at the beginning of this list.
		''' </summary>
		''' <param name="e"> the element to add </param>
		Public Overridable Sub addFirst(  e As E)
			linkFirst(e)
		End Sub

		''' <summary>
		''' Appends the specified element to the end of this list.
		''' 
		''' <p>This method is equivalent to <seealso cref="#add"/>.
		''' </summary>
		''' <param name="e"> the element to add </param>
		Public Overridable Sub addLast(  e As E)
			linkLast(e)
		End Sub

		''' <summary>
		''' Returns {@code true} if this list contains the specified element.
		''' More formally, returns {@code true} if and only if this list contains
		''' at least one element {@code e} such that
		''' <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>.
		''' </summary>
		''' <param name="o"> element whose presence in this list is to be tested </param>
		''' <returns> {@code true} if this list contains the specified element </returns>
		Public Overridable Function contains(  o As Object) As Boolean Implements List(Of E).contains, Deque(Of E).contains
			Return IndexOf(o) <> -1
		End Function

		''' <summary>
		''' Returns the number of elements in this list.
		''' </summary>
		''' <returns> the number of elements in this list </returns>
		Public Overridable Function size() As Integer Implements List(Of E).size, Deque(Of E).size
			Return size_Renamed
		End Function

		''' <summary>
		''' Appends the specified element to the end of this list.
		''' 
		''' <p>This method is equivalent to <seealso cref="#addLast"/>.
		''' </summary>
		''' <param name="e"> element to be appended to this list </param>
		''' <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		Public Overridable Function add(  e As E) As Boolean
			linkLast(e)
			Return True
		End Function

		''' <summary>
		''' Removes the first occurrence of the specified element from this list,
		''' if it is present.  If this list does not contain the element, it is
		''' unchanged.  More formally, removes the element with the lowest index
		''' {@code i} such that
		''' <tt>(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i)))</tt>
		''' (if such an element exists).  Returns {@code true} if this list
		''' contained the specified element (or equivalently, if this list
		''' changed as a result of the call).
		''' </summary>
		''' <param name="o"> element to be removed from this list, if present </param>
		''' <returns> {@code true} if this list contained the specified element </returns>
		Public Overridable Function remove(  o As Object) As Boolean Implements List(Of E).remove, Deque(Of E).remove
			If o Is Nothing Then
				Dim x As Node(Of E) = first
				Do While x IsNot Nothing
					If x.item Is Nothing Then
						unlink(x)
						Return True
					End If
					x = x.next
				Loop
			Else
				Dim x As Node(Of E) = first
				Do While x IsNot Nothing
					If o.Equals(x.item) Then
						unlink(x)
						Return True
					End If
					x = x.next
				Loop
			End If
			Return False
		End Function

		''' <summary>
		''' Appends all of the elements in the specified collection to the end of
		''' this list, in the order that they are returned by the specified
		''' collection's iterator.  The behavior of this operation is undefined if
		''' the specified collection is modified while the operation is in
		''' progress.  (Note that this will occur if the specified collection is
		''' this list, and it's nonempty.)
		''' </summary>
		''' <param name="c"> collection containing elements to be added to this list </param>
		''' <returns> {@code true} if this list changed as a result of the call </returns>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
		Public Overridable Function addAll(Of T1 As E)(  c As Collection(Of T1)) As Boolean Implements List(Of E).addAll
			Return addAll(size_Renamed, c)
		End Function

		''' <summary>
		''' Inserts all of the elements in the specified collection into this
		''' list, starting at the specified position.  Shifts the element
		''' currently at that position (if any) and any subsequent elements to
		''' the right (increases their indices).  The new elements will appear
		''' in the list in the order that they are returned by the
		''' specified collection's iterator.
		''' </summary>
		''' <param name="index"> index at which to insert the first element
		'''              from the specified collection </param>
		''' <param name="c"> collection containing elements to be added to this list </param>
		''' <returns> {@code true} if this list changed as a result of the call </returns>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
		Public Overridable Function addAll(Of T1 As E)(  index As Integer,   c As Collection(Of T1)) As Boolean Implements List(Of E).addAll
			checkPositionIndex(index)

			Dim a As Object() = c.ToArray()
			Dim numNew As Integer = a.Length
			If numNew = 0 Then Return False

			Dim pred As Node(Of E), succ As Node(Of E)
			If index = size_Renamed Then
				succ = Nothing
				pred = last
			Else
				succ = node(index)
				pred = succ.prev
			End If

			For Each o As Object In a
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim e As E = CType(o, E)
				Dim newNode As New Node(Of E)(pred, e, Nothing)
				If pred Is Nothing Then
					first = newNode
				Else
					pred.next = newNode
				End If
				pred = newNode
			Next o

			If succ Is Nothing Then
				last = pred
			Else
				pred.next = succ
				succ.prev = pred
			End If

			size_Renamed += numNew
			modCount += 1
			Return True
		End Function

		''' <summary>
		''' Removes all of the elements from this list.
		''' The list will be empty after this call returns.
		''' </summary>
		Public Overridable Sub clear() Implements List(Of E).clear
			' Clearing all of the links between nodes is "unnecessary", but:
			' - helps a generational GC if the discarded nodes inhabit
			'   more than one generation
			' - is sure to free memory even if there is a reachable Iterator
			Dim x As Node(Of E) = first
			Do While x IsNot Nothing
				Dim [next] As Node(Of E) = x.next
				x.item = Nothing
				x.next = Nothing
				x.prev = Nothing
				x = [next]
			Loop
				last = Nothing
				first = last
			size_Renamed = 0
			modCount += 1
		End Sub


		' Positional Access Operations

		''' <summary>
		''' Returns the element at the specified position in this list.
		''' </summary>
		''' <param name="index"> index of the element to return </param>
		''' <returns> the element at the specified position in this list </returns>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overridable Function [get](  index As Integer) As E Implements List(Of E).get
			checkElementIndex(index)
			Return node(index).item
		End Function

		''' <summary>
		''' Replaces the element at the specified position in this list with the
		''' specified element.
		''' </summary>
		''' <param name="index"> index of the element to replace </param>
		''' <param name="element"> element to be stored at the specified position </param>
		''' <returns> the element previously at the specified position </returns>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overridable Function [set](  index As Integer,   element As E) As E
			checkElementIndex(index)
			Dim x As Node(Of E) = node(index)
			Dim oldVal As E = x.item
			x.item = element
			Return oldVal
		End Function

		''' <summary>
		''' Inserts the specified element at the specified position in this list.
		''' Shifts the element currently at that position (if any) and any
		''' subsequent elements to the right (adds one to their indices).
		''' </summary>
		''' <param name="index"> index at which the specified element is to be inserted </param>
		''' <param name="element"> element to be inserted </param>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overridable Sub add(  index As Integer,   element As E)
			checkPositionIndex(index)

			If index = size_Renamed Then
				linkLast(element)
			Else
				linkBefore(element, node(index))
			End If
		End Sub

		''' <summary>
		''' Removes the element at the specified position in this list.  Shifts any
		''' subsequent elements to the left (subtracts one from their indices).
		''' Returns the element that was removed from the list.
		''' </summary>
		''' <param name="index"> the index of the element to be removed </param>
		''' <returns> the element previously at the specified position </returns>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overridable Function remove(  index As Integer) As E Implements List(Of E).remove
			checkElementIndex(index)
			Return unlink(node(index))
		End Function

		''' <summary>
		''' Tells if the argument is the index of an existing element.
		''' </summary>
		Private Function isElementIndex(  index As Integer) As Boolean
			Return index >= 0 AndAlso index < size_Renamed
		End Function

		''' <summary>
		''' Tells if the argument is the index of a valid position for an
		''' iterator or an add operation.
		''' </summary>
		Private Function isPositionIndex(  index As Integer) As Boolean
			Return index >= 0 AndAlso index <= size_Renamed
		End Function

		''' <summary>
		''' Constructs an IndexOutOfBoundsException detail message.
		''' Of the many possible refactorings of the error handling code,
		''' this "outlining" performs best with both server and client VMs.
		''' </summary>
		Private Function outOfBoundsMsg(  index As Integer) As String
			Return "Index: " & index & ", Size: " & size_Renamed
		End Function

		Private Sub checkElementIndex(  index As Integer)
			If Not isElementIndex(index) Then Throw New IndexOutOfBoundsException(outOfBoundsMsg(index))
		End Sub

		Private Sub checkPositionIndex(  index As Integer)
			If Not isPositionIndex(index) Then Throw New IndexOutOfBoundsException(outOfBoundsMsg(index))
		End Sub

		''' <summary>
		''' Returns the (non-null) Node at the specified element index.
		''' </summary>
		Friend Overridable Function node(  index As Integer) As Node(Of E)
			' assert isElementIndex(index);

			If index < (size_Renamed >> 1) Then
				Dim x As Node(Of E) = first
				Dim i As Integer = 0
				Do While i < index
					x = x.next
					i += 1
				Loop
				Return x
			Else
				Dim x As Node(Of E) = last
				Dim i As Integer = size_Renamed - 1
				Do While i > index
					x = x.prev
					i -= 1
				Loop
				Return x
			End If
		End Function

		' Search Operations

		''' <summary>
		''' Returns the index of the first occurrence of the specified element
		''' in this list, or -1 if this list does not contain the element.
		''' More formally, returns the lowest index {@code i} such that
		''' <tt>(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i)))</tt>,
		''' or -1 if there is no such index.
		''' </summary>
		''' <param name="o"> element to search for </param>
		''' <returns> the index of the first occurrence of the specified element in
		'''         this list, or -1 if this list does not contain the element </returns>
		Public Overridable Function indexOf(  o As Object) As Integer Implements List(Of E).indexOf
			Dim index As Integer = 0
			If o Is Nothing Then
				Dim x As Node(Of E) = first
				Do While x IsNot Nothing
					If x.item Is Nothing Then Return index
					index += 1
					x = x.next
				Loop
			Else
				Dim x As Node(Of E) = first
				Do While x IsNot Nothing
					If o.Equals(x.item) Then Return index
					index += 1
					x = x.next
				Loop
			End If
			Return -1
		End Function

		''' <summary>
		''' Returns the index of the last occurrence of the specified element
		''' in this list, or -1 if this list does not contain the element.
		''' More formally, returns the highest index {@code i} such that
		''' <tt>(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i)))</tt>,
		''' or -1 if there is no such index.
		''' </summary>
		''' <param name="o"> element to search for </param>
		''' <returns> the index of the last occurrence of the specified element in
		'''         this list, or -1 if this list does not contain the element </returns>
		Public Overridable Function lastIndexOf(  o As Object) As Integer Implements List(Of E).lastIndexOf
			Dim index As Integer = size_Renamed
			If o Is Nothing Then
				Dim x As Node(Of E) = last
				Do While x IsNot Nothing
					index -= 1
					If x.item Is Nothing Then Return index
					x = x.prev
				Loop
			Else
				Dim x As Node(Of E) = last
				Do While x IsNot Nothing
					index -= 1
					If o.Equals(x.item) Then Return index
					x = x.prev
				Loop
			End If
			Return -1
		End Function

		' Queue operations.

		''' <summary>
		''' Retrieves, but does not remove, the head (first element) of this list.
		''' </summary>
		''' <returns> the head of this list, or {@code null} if this list is empty
		''' @since 1.5 </returns>
		Public Overridable Function peek() As E Implements Deque(Of E).peek
			Dim f As Node(Of E) = first
			Return If(f Is Nothing, Nothing, f.item)
		End Function

		''' <summary>
		''' Retrieves, but does not remove, the head (first element) of this list.
		''' </summary>
		''' <returns> the head of this list </returns>
		''' <exception cref="NoSuchElementException"> if this list is empty
		''' @since 1.5 </exception>
		Public Overridable Function element() As E Implements Deque(Of E).element
			Return first
		End Function

		''' <summary>
		''' Retrieves and removes the head (first element) of this list.
		''' </summary>
		''' <returns> the head of this list, or {@code null} if this list is empty
		''' @since 1.5 </returns>
		Public Overridable Function poll() As E Implements Deque(Of E).poll
			Dim f As Node(Of E) = first
			Return If(f Is Nothing, Nothing, unlinkFirst(f))
		End Function

		''' <summary>
		''' Retrieves and removes the head (first element) of this list.
		''' </summary>
		''' <returns> the head of this list </returns>
		''' <exception cref="NoSuchElementException"> if this list is empty
		''' @since 1.5 </exception>
		Public Overridable Function remove() As E Implements Deque(Of E).remove
			Return removeFirst()
		End Function

		''' <summary>
		''' Adds the specified element as the tail (last element) of this list.
		''' </summary>
		''' <param name="e"> the element to add </param>
		''' <returns> {@code true} (as specified by <seealso cref="Queue#offer"/>)
		''' @since 1.5 </returns>
		Public Overridable Function offer(  e As E) As Boolean
			Return add(e)
		End Function

		' Deque operations
		''' <summary>
		''' Inserts the specified element at the front of this list.
		''' </summary>
		''' <param name="e"> the element to insert </param>
		''' <returns> {@code true} (as specified by <seealso cref="Deque#offerFirst"/>)
		''' @since 1.6 </returns>
		Public Overridable Function offerFirst(  e As E) As Boolean
			addFirst(e)
			Return True
		End Function

		''' <summary>
		''' Inserts the specified element at the end of this list.
		''' </summary>
		''' <param name="e"> the element to insert </param>
		''' <returns> {@code true} (as specified by <seealso cref="Deque#offerLast"/>)
		''' @since 1.6 </returns>
		Public Overridable Function offerLast(  e As E) As Boolean
			addLast(e)
			Return True
		End Function

		''' <summary>
		''' Retrieves, but does not remove, the first element of this list,
		''' or returns {@code null} if this list is empty.
		''' </summary>
		''' <returns> the first element of this list, or {@code null}
		'''         if this list is empty
		''' @since 1.6 </returns>
		Public Overridable Function peekFirst() As E Implements Deque(Of E).peekFirst
			Dim f As Node(Of E) = first
			Return If(f Is Nothing, Nothing, f.item)
		End Function

		''' <summary>
		''' Retrieves, but does not remove, the last element of this list,
		''' or returns {@code null} if this list is empty.
		''' </summary>
		''' <returns> the last element of this list, or {@code null}
		'''         if this list is empty
		''' @since 1.6 </returns>
		Public Overridable Function peekLast() As E Implements Deque(Of E).peekLast
			Dim l As Node(Of E) = last
			Return If(l Is Nothing, Nothing, l.item)
		End Function

		''' <summary>
		''' Retrieves and removes the first element of this list,
		''' or returns {@code null} if this list is empty.
		''' </summary>
		''' <returns> the first element of this list, or {@code null} if
		'''     this list is empty
		''' @since 1.6 </returns>
		Public Overridable Function pollFirst() As E Implements Deque(Of E).pollFirst
			Dim f As Node(Of E) = first
			Return If(f Is Nothing, Nothing, unlinkFirst(f))
		End Function

		''' <summary>
		''' Retrieves and removes the last element of this list,
		''' or returns {@code null} if this list is empty.
		''' </summary>
		''' <returns> the last element of this list, or {@code null} if
		'''     this list is empty
		''' @since 1.6 </returns>
		Public Overridable Function pollLast() As E Implements Deque(Of E).pollLast
			Dim l As Node(Of E) = last
			Return If(l Is Nothing, Nothing, unlinkLast(l))
		End Function

		''' <summary>
		''' Pushes an element onto the stack represented by this list.  In other
		''' words, inserts the element at the front of this list.
		''' 
		''' <p>This method is equivalent to <seealso cref="#addFirst"/>.
		''' </summary>
		''' <param name="e"> the element to push
		''' @since 1.6 </param>
		Public Overridable Sub push(  e As E)
			addFirst(e)
		End Sub

		''' <summary>
		''' Pops an element from the stack represented by this list.  In other
		''' words, removes and returns the first element of this list.
		''' 
		''' <p>This method is equivalent to <seealso cref="#removeFirst()"/>.
		''' </summary>
		''' <returns> the element at the front of this list (which is the top
		'''         of the stack represented by this list) </returns>
		''' <exception cref="NoSuchElementException"> if this list is empty
		''' @since 1.6 </exception>
		Public Overridable Function pop() As E Implements Deque(Of E).pop
			Return removeFirst()
		End Function

		''' <summary>
		''' Removes the first occurrence of the specified element in this
		''' list (when traversing the list from head to tail).  If the list
		''' does not contain the element, it is unchanged.
		''' </summary>
		''' <param name="o"> element to be removed from this list, if present </param>
		''' <returns> {@code true} if the list contained the specified element
		''' @since 1.6 </returns>
		Public Overridable Function removeFirstOccurrence(  o As Object) As Boolean Implements Deque(Of E).removeFirstOccurrence
			Return remove(o)
		End Function

		''' <summary>
		''' Removes the last occurrence of the specified element in this
		''' list (when traversing the list from head to tail).  If the list
		''' does not contain the element, it is unchanged.
		''' </summary>
		''' <param name="o"> element to be removed from this list, if present </param>
		''' <returns> {@code true} if the list contained the specified element
		''' @since 1.6 </returns>
		Public Overridable Function removeLastOccurrence(  o As Object) As Boolean Implements Deque(Of E).removeLastOccurrence
			If o Is Nothing Then
				Dim x As Node(Of E) = last
				Do While x IsNot Nothing
					If x.item Is Nothing Then
						unlink(x)
						Return True
					End If
					x = x.prev
				Loop
			Else
				Dim x As Node(Of E) = last
				Do While x IsNot Nothing
					If o.Equals(x.item) Then
						unlink(x)
						Return True
					End If
					x = x.prev
				Loop
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a list-iterator of the elements in this list (in proper
		''' sequence), starting at the specified position in the list.
		''' Obeys the general contract of {@code List.listIterator(int)}.<p>
		''' 
		''' The list-iterator is <i>fail-fast</i>: if the list is structurally
		''' modified at any time after the Iterator is created, in any way except
		''' through the list-iterator's own {@code remove} or {@code add}
		''' methods, the list-iterator will throw a
		''' {@code ConcurrentModificationException}.  Thus, in the face of
		''' concurrent modification, the iterator fails quickly and cleanly, rather
		''' than risking arbitrary, non-deterministic behavior at an undetermined
		''' time in the future.
		''' </summary>
		''' <param name="index"> index of the first element to be returned from the
		'''              list-iterator (by a call to {@code next}) </param>
		''' <returns> a ListIterator of the elements in this list (in proper
		'''         sequence), starting at the specified position in the list </returns>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		''' <seealso cref= List#listIterator(int) </seealso>
		Public Overridable Function listIterator(  index As Integer) As ListIterator(Of E) Implements List(Of E).listIterator
			checkPositionIndex(index)
			Return New ListItr(Me, index)
		End Function

		Private Class ListItr
			Implements ListIterator(Of E)

			Private ReadOnly outerInstance As LinkedList

			Private lastReturned As Node(Of E)
			Private next_Renamed As Node(Of E)
			Private nextIndex_Renamed As Integer
			Private expectedModCount As Integer = outerInstance.modCount

			Friend Sub New(  outerInstance As LinkedList,   index As Integer)
					Me.outerInstance = outerInstance
				' assert isPositionIndex(index);
				next_Renamed = If(index = outerInstance.size_Renamed, Nothing, outerInstance.node(index))
				nextIndex_Renamed = index
			End Sub

			Public Overridable Function hasNext() As Boolean Implements ListIterator(Of E).hasNext
				Return nextIndex_Renamed < outerInstance.size_Renamed
			End Function

			Public Overridable Function [next]() As E Implements ListIterator(Of E).next
				checkForComodification()
				If Not hasNext() Then Throw New NoSuchElementException

				lastReturned = next_Renamed
				next_Renamed = next_Renamed.next
				nextIndex_Renamed += 1
				Return lastReturned.item
			End Function

			Public Overridable Function hasPrevious() As Boolean Implements ListIterator(Of E).hasPrevious
				Return nextIndex_Renamed > 0
			End Function

			Public Overridable Function previous() As E Implements ListIterator(Of E).previous
				checkForComodification()
				If Not hasPrevious() Then Throw New NoSuchElementException

					next_Renamed = If(next_Renamed Is Nothing, outerInstance.last, next_Renamed.prev)
					lastReturned = next_Renamed
				nextIndex_Renamed -= 1
				Return lastReturned.item
			End Function

			Public Overridable Function nextIndex() As Integer Implements ListIterator(Of E).nextIndex
				Return nextIndex_Renamed
			End Function

			Public Overridable Function previousIndex() As Integer Implements ListIterator(Of E).previousIndex
				Return nextIndex_Renamed - 1
			End Function

			Public Overridable Sub remove() Implements ListIterator(Of E).remove
				checkForComodification()
				If lastReturned Is Nothing Then Throw New IllegalStateException

				Dim lastNext As Node(Of E) = lastReturned.next
				outerInstance.unlink(lastReturned)
				If next_Renamed Is lastReturned Then
					next_Renamed = lastNext
				Else
					nextIndex_Renamed -= 1
				End If
				lastReturned = Nothing
				expectedModCount += 1
			End Sub

			Public Overridable Sub [set](  e As E)
				If lastReturned Is Nothing Then Throw New IllegalStateException
				checkForComodification()
				lastReturned.item = e
			End Sub

			Public Overridable Sub add(  e As E)
				checkForComodification()
				lastReturned = Nothing
				If next_Renamed Is Nothing Then
					outerInstance.linkLast(e)
				Else
					outerInstance.linkBefore(e, next_Renamed)
				End If
				nextIndex_Renamed += 1
				expectedModCount += 1
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overridable Sub forEachRemaining(Of T1)(  action As java.util.function.Consumer(Of T1))
				Objects.requireNonNull(action)
				Do While outerInstance.modCount = expectedModCount AndAlso nextIndex_Renamed < outerInstance.size_Renamed
					action.accept(next_Renamed.item)
					lastReturned = next_Renamed
					next_Renamed = next_Renamed.next
					nextIndex_Renamed += 1
				Loop
				checkForComodification()
			End Sub

			Friend Sub checkForComodification()
				If outerInstance.modCount <> expectedModCount Then Throw New ConcurrentModificationException
			End Sub
		End Class

		Private Class Node(Of E)
			Friend item As E
			Friend [next] As Node(Of E)
			Friend prev As Node(Of E)

			Friend Sub New(  prev As Node(Of E),   element As E,   [next] As Node(Of E))
				Me.item = element
				Me.next = [next]
				Me.prev = prev
			End Sub
		End Class

		''' <summary>
		''' @since 1.6
		''' </summary>
		Public Overridable Function descendingIterator() As [Iterator](Of E) Implements Deque(Of E).descendingIterator
			Return New DescendingIterator(Me)
		End Function

		''' <summary>
		''' Adapter to provide descending iterators via ListItr.previous
		''' </summary>
		Private Class DescendingIterator
			Implements Iterator(Of E)

			Private ReadOnly outerInstance As LinkedList

			Public Sub New(  outerInstance As LinkedList)
				Me.outerInstance = outerInstance
			End Sub

			Private ReadOnly itr As New ListItr(outerInstance.size())
			Public Overridable Function hasNext() As Boolean Implements Iterator(Of E).hasNext
				Return itr.hasPrevious()
			End Function
			Public Overridable Function [next]() As E Implements Iterator(Of E).next
				Return itr.previous()
			End Function
			Public Overridable Sub remove() Implements Iterator(Of E).remove
				itr.remove()
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Function superClone() As LinkedList(Of E)
			Try
				Return CType(MyBase.clone(), LinkedList(Of E))
			Catch e As CloneNotSupportedException
				Throw New InternalError(e)
			End Try
		End Function

		''' <summary>
		''' Returns a shallow copy of this {@code LinkedList}. (The elements
		''' themselves are not cloned.)
		''' </summary>
		''' <returns> a shallow copy of this {@code LinkedList} instance </returns>
		Public Overridable Function clone() As Object
			Dim clone_Renamed As LinkedList(Of E) = superClone()

			' Put clone into "virgin" state
				clone_Renamed.last = Nothing
				clone_Renamed.first = clone_Renamed.last
			clone_Renamed.size_Renamed = 0
			clone_Renamed.modCount = 0

			' Initialize clone with our elements
			Dim x As Node(Of E) = first
			Do While x IsNot Nothing
				clone_Renamed.add(x.item)
				x = x.next
			Loop

			Return clone_Renamed
		End Function

		''' <summary>
		''' Returns an array containing all of the elements in this list
		''' in proper sequence (from first to last element).
		''' 
		''' <p>The returned array will be "safe" in that no references to it are
		''' maintained by this list.  (In other words, this method must allocate
		''' a new array).  The caller is thus free to modify the returned array.
		''' 
		''' <p>This method acts as bridge between array-based and collection-based
		''' APIs.
		''' </summary>
		''' <returns> an array containing all of the elements in this list
		'''         in proper sequence </returns>
		Public Overridable Function toArray() As Object() Implements List(Of E).toArray
			Dim result As Object() = New Object(size_Renamed - 1){}
			Dim i As Integer = 0
			Dim x As Node(Of E) = first
			Do While x IsNot Nothing
				result(i) = x.item
				i += 1
				x = x.next
			Loop
			Return result
		End Function

		''' <summary>
		''' Returns an array containing all of the elements in this list in
		''' proper sequence (from first to last element); the runtime type of
		''' the returned array is that of the specified array.  If the list fits
		''' in the specified array, it is returned therein.  Otherwise, a new
		''' array is allocated with the runtime type of the specified array and
		''' the size of this list.
		''' 
		''' <p>If the list fits in the specified array with room to spare (i.e.,
		''' the array has more elements than the list), the element in the array
		''' immediately following the end of the list is set to {@code null}.
		''' (This is useful in determining the length of the list <i>only</i> if
		''' the caller knows that the list does not contain any null elements.)
		''' 
		''' <p>Like the <seealso cref="#toArray()"/> method, this method acts as bridge between
		''' array-based and collection-based APIs.  Further, this method allows
		''' precise control over the runtime type of the output array, and may,
		''' under certain circumstances, be used to save allocation costs.
		''' 
		''' <p>Suppose {@code x} is a list known to contain only strings.
		''' The following code can be used to dump the list into a newly
		''' allocated array of {@code String}:
		''' 
		''' <pre>
		'''     String[] y = x.toArray(new String[0]);</pre>
		''' 
		''' Note that {@code toArray(new Object[0])} is identical in function to
		''' {@code toArray()}.
		''' </summary>
		''' <param name="a"> the array into which the elements of the list are to
		'''          be stored, if it is big enough; otherwise, a new array of the
		'''          same runtime type is allocated for this purpose. </param>
		''' <returns> an array containing the elements of the list </returns>
		''' <exception cref="ArrayStoreException"> if the runtime type of the specified array
		'''         is not a supertype of the runtime type of every element in
		'''         this list </exception>
		''' <exception cref="NullPointerException"> if the specified array is null </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function toArray(Of T)(  a As T()) As T() Implements List(Of E).toArray
			If a.Length < size_Renamed Then a = CType(java.lang.reflect.Array.newInstance(a.GetType().GetElementType(), size_Renamed), T())
			Dim i As Integer = 0
			Dim result As Object() = a
			Dim x As Node(Of E) = first
			Do While x IsNot Nothing
				result(i) = x.item
				i += 1
				x = x.next
			Loop

			If a.Length > size_Renamed Then a(size_Renamed) = Nothing

			Return a
		End Function

		Private Const serialVersionUID As Long = 876323262645176354L

		''' <summary>
		''' Saves the state of this {@code LinkedList} instance to a stream
		''' (that is, serializes it).
		''' 
		''' @serialData The size of the list (the number of elements it
		'''             contains) is emitted (int), followed by all of its
		'''             elements (each an Object) in the proper order.
		''' </summary>
		Private Sub writeObject(  s As java.io.ObjectOutputStream)
			' Write out any hidden serialization magic
			s.defaultWriteObject()

			' Write out size
			s.writeInt(size_Renamed)

			' Write out all elements in the proper order.
			Dim x As Node(Of E) = first
			Do While x IsNot Nothing
				s.writeObject(x.item)
				x = x.next
			Loop
		End Sub

		''' <summary>
		''' Reconstitutes this {@code LinkedList} instance from a stream
		''' (that is, deserializes it).
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Sub readObject(  s As java.io.ObjectInputStream)
			' Read in any hidden serialization magic
			s.defaultReadObject()

			' Read in size
			Dim size As Integer = s.readInt()

			' Read in all elements in the proper order.
			For i As Integer = 0 To size - 1
				linkLast(CType(s.readObject(), E))
			Next i
		End Sub

		''' <summary>
		''' Creates a <em><a href="Spliterator.html#binding">late-binding</a></em>
		''' and <em>fail-fast</em> <seealso cref="Spliterator"/> over the elements in this
		''' list.
		''' 
		''' <p>The {@code Spliterator} reports <seealso cref="Spliterator#SIZED"/> and
		''' <seealso cref="Spliterator#ORDERED"/>.  Overriding implementations should document
		''' the reporting of additional characteristic values.
		''' 
		''' @implNote
		''' The {@code Spliterator} additionally reports <seealso cref="Spliterator#SUBSIZED"/>
		''' and implements {@code trySplit} to permit limited parallelism..
		''' </summary>
		''' <returns> a {@code Spliterator} over the elements in this list
		''' @since 1.8 </returns>
		Public Overrides Function spliterator() As Spliterator(Of E) Implements List(Of E).spliterator
			Return New LLSpliterator(Of E)(Me, -1, 0)
		End Function

		''' <summary>
		''' A customized variant of Spliterators.IteratorSpliterator </summary>
		Friend NotInheritable Class LLSpliterator(Of E)
			Implements Spliterator(Of E)

			Friend Shared ReadOnly BATCH_UNIT As Integer = 1 << 10 ' batch array size increment
			Friend Shared ReadOnly MAX_BATCH As Integer = 1 << 25 ' max batch array size;
			Friend ReadOnly list As LinkedList(Of E) ' null OK unless traversed
			Friend current As Node(Of E) ' current node; null until initialized
			Friend est As Integer ' size estimate; -1 until first needed
			Friend expectedModCount As Integer ' initialized when est set
			Friend batch As Integer ' batch size for splits

			Friend Sub New(  list As LinkedList(Of E),   est As Integer,   expectedModCount As Integer)
				Me.list = list
				Me.est = est
				Me.expectedModCount = expectedModCount
			End Sub

			Friend Property est As Integer
				Get
					Dim s As Integer ' force initialization
					Dim lst As LinkedList(Of E)
					s = est
					If s < 0 Then
						lst = list
						If lst Is Nothing Then
								est = 0
								s = est
						Else
							expectedModCount = lst.modCount
							current = lst.first
								est = lst.size_Renamed
								s = est
						End If
					End If
					Return s
				End Get
			End Property

			Public Function estimateSize() As Long Implements Spliterator(Of E).estimateSize
				Return CLng(est)
			End Function

			Public Function trySplit() As Spliterator(Of E) Implements Spliterator(Of E).trySplit
				Dim p As Node(Of E)
				Dim s As Integer = est
				p = current
				If s > 1 AndAlso p IsNot Nothing Then
					Dim n As Integer = batch + BATCH_UNIT
					If n > s Then n = s
					If n > MAX_BATCH Then n = MAX_BATCH
					Dim a As Object() = New Object(n - 1){}
					Dim j As Integer = 0
					Do
						a(j) = p.item
						j += 1
						p = p.next
					Loop While p IsNot Nothing AndAlso j < n
					current = p
					batch = j
					est = s - j
					Return Spliterators.spliterator(a, 0, j, Spliterator.ORDERED)
				End If
				Return Nothing
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(  action As java.util.function.Consumer(Of T1)) Implements Spliterator(Of E).forEachRemaining
				Dim p As Node(Of E)
				Dim n As Integer
				If action Is Nothing Then Throw New NullPointerException
				n = est
				p = current
				If n > 0 AndAlso p IsNot Nothing Then
					current = Nothing
					est = 0
					Do
						Dim e As E = p.item
						p = p.next
						action.accept(e)
						n -= 1
					Loop While p IsNot Nothing AndAlso n > 0
				End If
				If list.modCount <> expectedModCount Then Throw New ConcurrentModificationException
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(  action As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of E).tryAdvance
				Dim p As Node(Of E)
				If action Is Nothing Then Throw New NullPointerException
				p = current
				If est > 0 AndAlso p IsNot Nothing Then
					est -= 1
					Dim e As E = p.item
					current = p.next
					action.accept(e)
					If list.modCount <> expectedModCount Then Throw New ConcurrentModificationException
					Return True
				End If
				Return False
			End Function

			Public Function characteristics() As Integer Implements Spliterator(Of E).characteristics
				Return Spliterator.ORDERED Or Spliterator.SIZED Or Spliterator.SUBSIZED
			End Function
		End Class

	End Class

End Namespace
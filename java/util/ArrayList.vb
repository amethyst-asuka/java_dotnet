Imports System
Imports System.Collections
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
	''' Resizable-array implementation of the <tt>List</tt> interface.  Implements
	''' all optional list operations, and permits all elements, including
	''' <tt>null</tt>.  In addition to implementing the <tt>List</tt> interface,
	''' this class provides methods to manipulate the size of the array that is
	''' used internally to store the list.  (This class is roughly equivalent to
	''' <tt>Vector</tt>, except that it is unsynchronized.)
	''' 
	''' <p>The <tt>size</tt>, <tt>isEmpty</tt>, <tt>get</tt>, <tt>set</tt>,
	''' <tt>iterator</tt>, and <tt>listIterator</tt> operations run in constant
	''' time.  The <tt>add</tt> operation runs in <i>amortized constant time</i>,
	''' that is, adding n elements requires O(n) time.  All of the other operations
	''' run in linear time (roughly speaking).  The constant factor is low compared
	''' to that for the <tt>LinkedList</tt> implementation.
	''' 
	''' <p>Each <tt>ArrayList</tt> instance has a <i>capacity</i>.  The capacity is
	''' the size of the array used to store the elements in the list.  It is always
	''' at least as large as the list size.  As elements are added to an ArrayList,
	''' its capacity grows automatically.  The details of the growth policy are not
	''' specified beyond the fact that adding an element has constant amortized
	''' time cost.
	''' 
	''' <p>An application can increase the capacity of an <tt>ArrayList</tt> instance
	''' before adding a large number of elements using the <tt>ensureCapacity</tt>
	''' operation.  This may reduce the amount of incremental reallocation.
	''' 
	''' <p><strong>Note that this implementation is not synchronized.</strong>
	''' If multiple threads access an <tt>ArrayList</tt> instance concurrently,
	''' and at least one of the threads modifies the list structurally, it
	''' <i>must</i> be synchronized externally.  (A structural modification is
	''' any operation that adds or deletes one or more elements, or explicitly
	''' resizes the backing array; merely setting the value of an element is not
	''' a structural modification.)  This is typically accomplished by
	''' synchronizing on some object that naturally encapsulates the list.
	''' 
	''' If no such object exists, the list should be "wrapped" using the
	''' <seealso cref="Collections#synchronizedList Collections.synchronizedList"/>
	''' method.  This is best done at creation time, to prevent accidental
	''' unsynchronized access to the list:<pre>
	'''   List list = Collections.synchronizedList(new ArrayList(...));</pre>
	''' 
	''' <p><a name="fail-fast">
	''' The iterators returned by this class's <seealso cref="#iterator() iterator"/> and
	''' <seealso cref="#listIterator(int) listIterator"/> methods are <em>fail-fast</em>:</a>
	''' if the list is structurally modified at any time after the iterator is
	''' created, in any way except through the iterator's own
	''' <seealso cref="ListIterator#remove() remove"/> or
	''' <seealso cref="ListIterator#add(Object) add"/> methods, the iterator will throw a
	''' <seealso cref="ConcurrentModificationException"/>.  Thus, in the face of
	''' concurrent modification, the iterator fails quickly and cleanly, rather
	''' than risking arbitrary, non-deterministic behavior at an undetermined
	''' time in the future.
	''' 
	''' <p>Note that the fail-fast behavior of an iterator cannot be guaranteed
	''' as it is, generally speaking, impossible to make any hard guarantees in the
	''' presence of unsynchronized concurrent modification.  Fail-fast iterators
	''' throw {@code ConcurrentModificationException} on a best-effort basis.
	''' Therefore, it would be wrong to write a program that depended on this
	''' exception for its correctness:  <i>the fail-fast behavior of iterators
	''' should be used only to detect bugs.</i>
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @author  Josh Bloch
	''' @author  Neal Gafter </summary>
	''' <seealso cref=     Collection </seealso>
	''' <seealso cref=     List </seealso>
	''' <seealso cref=     LinkedList </seealso>
	''' <seealso cref=     Vector
	''' @since   1.2 </seealso>

	<Serializable> _
	Public Class List(Of E)
		Inherits AbstractList(Of E)
        Implements IList(Of E), RandomAccess, Cloneable

        Private Const serialVersionUID As Long = 8683452581122892189L

		''' <summary>
		''' Default initial capacity.
		''' </summary>
		Private Const DEFAULT_CAPACITY As Integer = 10

		''' <summary>
		''' Shared empty array instance used for empty instances.
		''' </summary>
		Private Shared ReadOnly EMPTY_ELEMENTDATA As Object() = {}

		''' <summary>
		''' Shared empty array instance used for default sized empty instances. We
		''' distinguish this from EMPTY_ELEMENTDATA to know how much to inflate when
		''' first element is added.
		''' </summary>
		Private Shared ReadOnly DEFAULTCAPACITY_EMPTY_ELEMENTDATA As Object() = {}

		''' <summary>
		''' The array buffer into which the elements of the ArrayList are stored.
		''' The capacity of the ArrayList is the length of this array buffer. Any
		''' empty ArrayList with elementData == DEFAULTCAPACITY_EMPTY_ELEMENTDATA
		''' will be expanded to DEFAULT_CAPACITY when the first element is added.
		''' </summary>
		<NonSerialized> _
		Friend elementData_Renamed As Object() ' non-private to simplify nested class access

		''' <summary>
		''' The size of the ArrayList (the number of elements it contains).
		''' 
		''' @serial
		''' </summary>
		Private size_Renamed As Integer

		''' <summary>
		''' Constructs an empty list with the specified initial capacity.
		''' </summary>
		''' <param name="initialCapacity">  the initial capacity of the list </param>
		''' <exception cref="IllegalArgumentException"> if the specified initial capacity
		'''         is negative </exception>
		Public Sub New(ByVal initialCapacity As Integer)
			If initialCapacity > 0 Then
				Me.elementData_Renamed = New Object(initialCapacity - 1){}
			ElseIf initialCapacity = 0 Then
				Me.elementData_Renamed = EMPTY_ELEMENTDATA
			Else
				Throw New IllegalArgumentException("Illegal Capacity: " & initialCapacity)
			End If
		End Sub

		''' <summary>
		''' Constructs an empty list with an initial capacity of ten.
		''' </summary>
		Public Sub New()
			Me.elementData_Renamed = DEFAULTCAPACITY_EMPTY_ELEMENTDATA
		End Sub

        ''' <summary>
        ''' Constructs a list containing the elements of the specified
        ''' collection, in the order they are returned by the collection's
        ''' iterator.
        ''' </summary>
        ''' <param name="c"> the collection whose elements are to be placed into this list </param>
        ''' <exception cref="NullPointerException"> if the specified collection is null </exception>
        Public Sub New(ByVal c As Collection(Of E))
            elementData_Renamed = c.toArray()
            size_Renamed = elementData_Renamed.Length
            If size_Renamed <> 0 Then
                ' c.toArray might (incorrectly) not return Object[] (see 6260652)
                If elementData_Renamed.GetType() IsNot GetType(Object()) Then elementData_Renamed = Arrays.copyOf(elementData_Renamed, size_Renamed, GetType(Object()))
            Else
                ' replace with empty array.
                Me.elementData_Renamed = EMPTY_ELEMENTDATA
            End If
        End Sub

        ''' <summary>
        ''' Trims the capacity of this <tt>ArrayList</tt> instance to be the
        ''' list's current size.  An application can use this operation to minimize
        ''' the storage of an <tt>ArrayList</tt> instance.
        ''' </summary>
        Public Overridable Sub trimToSize()
			modCount += 1
			If size_Renamed < elementData_Renamed.Length Then elementData_Renamed = If(size_Renamed = 0, EMPTY_ELEMENTDATA, Arrays.copyOf(elementData_Renamed, size_Renamed))
		End Sub

		''' <summary>
		''' Increases the capacity of this <tt>ArrayList</tt> instance, if
		''' necessary, to ensure that it can hold at least the number of elements
		''' specified by the minimum capacity argument.
		''' </summary>
		''' <param name="minCapacity">   the desired minimum capacity </param>
		Public Overridable Sub ensureCapacity(ByVal minCapacity As Integer)
			Dim minExpand As Integer = If(elementData_Renamed <> DEFAULTCAPACITY_EMPTY_ELEMENTDATA, 0, DEFAULT_CAPACITY)
				' any size if not default element table
				' larger than default for default empty table. It's already
				' supposed to be at default size.

			If minCapacity > minExpand Then ensureExplicitCapacity(minCapacity)
		End Sub

		Private Sub ensureCapacityInternal(ByVal minCapacity As Integer)
			If elementData_Renamed = DEFAULTCAPACITY_EMPTY_ELEMENTDATA Then minCapacity = System.Math.Max(DEFAULT_CAPACITY, minCapacity)

			ensureExplicitCapacity(minCapacity)
		End Sub

		Private Sub ensureExplicitCapacity(ByVal minCapacity As Integer)
			modCount += 1

			' overflow-conscious code
			If minCapacity - elementData_Renamed.Length > 0 Then grow(minCapacity)
		End Sub

		''' <summary>
		''' The maximum size of array to allocate.
		''' Some VMs reserve some header words in an array.
		''' Attempts to allocate larger arrays may result in
		''' OutOfMemoryError: Requested array size exceeds VM limit
		''' </summary>
		Private Shared ReadOnly MAX_ARRAY_SIZE As Integer =  java.lang.[Integer].MAX_VALUE - 8

		''' <summary>
		''' Increases the capacity to ensure that it can hold at least the
		''' number of elements specified by the minimum capacity argument.
		''' </summary>
		''' <param name="minCapacity"> the desired minimum capacity </param>
		Private Sub grow(ByVal minCapacity As Integer)
			' overflow-conscious code
			Dim oldCapacity As Integer = elementData_Renamed.Length
			Dim newCapacity As Integer = oldCapacity + (oldCapacity >> 1)
			If newCapacity - minCapacity < 0 Then newCapacity = minCapacity
			If newCapacity - MAX_ARRAY_SIZE > 0 Then newCapacity = hugeCapacity(minCapacity)
			' minCapacity is usually close to size, so this is a win:
			elementData = New java.lang.Object(newCapacity - 1){}
			Array.Copy(elementData_Renamed, elementData_Renamed, newCapacity)
		End Sub

		Private Shared Function hugeCapacity(ByVal minCapacity As Integer) As Integer
			If minCapacity < 0 Then ' overflow Throw New OutOfMemoryError
			Return If(minCapacity > MAX_ARRAY_SIZE,  java.lang.[Integer].Max_Value, MAX_ARRAY_SIZE)
		End Function

		''' <summary>
		''' Returns the number of elements in this list.
		''' </summary>
		''' <returns> the number of elements in this list </returns>
		Public Overridable Function size() As Integer Implements List(Of E).size
			Return size_Renamed
		End Function

        ''' <summary>
        ''' Returns <tt>true</tt> if this list contains no elements.
        ''' </summary>
        ''' <returns> <tt>true</tt> if this list contains no elements </returns>
        Public Overridable ReadOnly Property empty As Boolean Implements List(Of E).isEmpty
            Get
                Return size_Renamed = 0
            End Get
        End Property

        ''' <summary>
        ''' Returns <tt>true</tt> if this list contains the specified element.
        ''' More formally, returns <tt>true</tt> if and only if this list contains
        ''' at least one element <tt>e</tt> such that
        ''' <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>.
        ''' </summary>
        ''' <param name="o"> element whose presence in this list is to be tested </param>
        ''' <returns> <tt>true</tt> if this list contains the specified element </returns>
        Public Overridable Function contains(ByVal o As Object) As Boolean Implements List(Of E).contains
			Return IndexOf(o) >= 0
		End Function

		''' <summary>
		''' Returns the index of the first occurrence of the specified element
		''' in this list, or -1 if this list does not contain the element.
		''' More formally, returns the lowest index <tt>i</tt> such that
		''' <tt>(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i)))</tt>,
		''' or -1 if there is no such index.
		''' </summary>
		Public Overridable Function indexOf(ByVal o As Object) As Integer Implements List(Of E).indexOf
			If o Is Nothing Then
				For i As Integer = 0 To size_Renamed - 1
					If elementData_Renamed(i) Is Nothing Then Return i
				Next i
			Else
				For i As Integer = 0 To size_Renamed - 1
					If o.Equals(elementData_Renamed(i)) Then Return i
				Next i
			End If
			Return -1
		End Function

		''' <summary>
		''' Returns the index of the last occurrence of the specified element
		''' in this list, or -1 if this list does not contain the element.
		''' More formally, returns the highest index <tt>i</tt> such that
		''' <tt>(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i)))</tt>,
		''' or -1 if there is no such index.
		''' </summary>
		Public Overridable Function lastIndexOf(ByVal o As Object) As Integer Implements List(Of E).lastIndexOf
			If o Is Nothing Then
				For i As Integer = size_Renamed-1 To 0 Step -1
					If elementData_Renamed(i) Is Nothing Then Return i
				Next i
			Else
				For i As Integer = size_Renamed-1 To 0 Step -1
					If o.Equals(elementData_Renamed(i)) Then Return i
				Next i
			End If
			Return -1
		End Function

		''' <summary>
		''' Returns a shallow copy of this <tt>ArrayList</tt> instance.  (The
		''' elements themselves are not copied.)
		''' </summary>
		''' <returns> a clone of this <tt>ArrayList</tt> instance </returns>
		Public Overridable Function clone() As Object
			Try
                Dim v As List(Of E) = CType(MyBase.clone(), List(Of E))
                v.elementData = New java.lang.Object(size - 1){}
				Array.Copy(elementData_Renamed, v.elementData_Renamed, size_Renamed)
				v.modCount = 0
				Return v
			Catch e As CloneNotSupportedException
				' this shouldn't happen, since we are Cloneable
				Throw New InternalError(e)
			End Try
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
		''' <returns> an array containing all of the elements in this list in
		'''         proper sequence </returns>
		Public Overridable Function toArray() As Object() Implements List(Of E).toArray
			Return Arrays.copyOf(elementData_Renamed, size_Renamed)
		End Function

        ''' <summary>
        ''' Returns an array containing all of the elements in this list in proper
        ''' sequence (from first to last element); the runtime type of the returned
        ''' array is that of the specified array.  If the list fits in the
        ''' specified array, it is returned therein.  Otherwise, a new array is
        ''' allocated with the runtime type of the specified array and the size of
        ''' this list.
        ''' 
        ''' <p>If the list fits in the specified array with room to spare
        ''' (i.e., the array has more elements than the list), the element in
        ''' the array immediately following the end of the collection is set to
        ''' <tt>null</tt>.  (This is useful in determining the length of the
        ''' list <i>only</i> if the caller knows that the list does not contain
        ''' any null elements.)
        ''' </summary>
        ''' <param name="a"> the array into which the elements of the list are to
        '''          be stored, if it is big enough; otherwise, a new array of the
        '''          same runtime type is allocated for this purpose. </param>
        ''' <returns> an array containing the elements of the list </returns>
        ''' <exception cref="ArrayStoreException"> if the runtime type of the specified array
        '''         is not a supertype of the runtime type of every element in
        '''         this list </exception>
        ''' <exception cref="NullPointerException"> if the specified array is null </exception>
        Public Overridable Function toArray(Of T)(ByVal a As T()) As T() Implements List(Of E).toArray
            If a.Length < size_Renamed Then Return CType(Arrays.copyOf(elementData_Renamed, size_Renamed, a.GetType()), T())
            Array.Copy(elementData_Renamed, 0, a, 0, size_Renamed)
            If a.Length > size_Renamed Then a(size_Renamed) = Nothing
            Return a
        End Function

        ' Positional Access Operations
        Friend Overridable Function elementData(ByVal index As Integer) As E
            Return CType(elementData_Renamed(index), E)
        End Function

        ''' <summary>
        ''' Returns the element at the specified position in this list.
        ''' </summary>
        ''' <param name="index"> index of the element to return </param>
        ''' <returns> the element at the specified position in this list </returns>
        ''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
        Public Overridable Function [get](ByVal index As Integer) As E Implements List(Of E).get
			rangeCheck(index)

			Return elementData(index)
		End Function

		''' <summary>
		''' Replaces the element at the specified position in this list with
		''' the specified element.
		''' </summary>
		''' <param name="index"> index of the element to replace </param>
		''' <param name="element"> element to be stored at the specified position </param>
		''' <returns> the element previously at the specified position </returns>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overridable Function [set](ByVal index As Integer, ByVal element As E) As E
			rangeCheck(index)

			Dim oldValue As E = elementData(index)
			elementData_Renamed(index) = element
			Return oldValue
		End Function

		''' <summary>
		''' Appends the specified element to the end of this list.
		''' </summary>
		''' <param name="e"> element to be appended to this list </param>
		''' <returns> <tt>true</tt> (as specified by <seealso cref="Collection#add"/>) </returns>
		Public Overridable Function add(ByVal e As E) As Boolean
			ensureCapacityInternal(size_Renamed + 1) ' Increments modCount!!
			elementData_Renamed(size_Renamed) = e
			size_Renamed += 1
			Return True
		End Function

		''' <summary>
		''' Inserts the specified element at the specified position in this
		''' list. Shifts the element currently at that position (if any) and
		''' any subsequent elements to the right (adds one to their indices).
		''' </summary>
		''' <param name="index"> index at which the specified element is to be inserted </param>
		''' <param name="element"> element to be inserted </param>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overridable Sub add(ByVal index As Integer, ByVal element As E)
			rangeCheckForAdd(index)

			ensureCapacityInternal(size_Renamed + 1) ' Increments modCount!!
			Array.Copy(elementData_Renamed, index, elementData_Renamed, index + 1, size_Renamed - index)
			elementData_Renamed(index) = element
			size_Renamed += 1
		End Sub

		''' <summary>
		''' Removes the element at the specified position in this list.
		''' Shifts any subsequent elements to the left (subtracts one from their
		''' indices).
		''' </summary>
		''' <param name="index"> the index of the element to be removed </param>
		''' <returns> the element that was removed from the list </returns>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overridable Function remove(ByVal index As Integer) As E Implements List(Of E).remove
			rangeCheck(index)

			modCount += 1
			Dim oldValue As E = elementData(index)

			Dim numMoved As Integer = size_Renamed - index - 1
			If numMoved > 0 Then Array.Copy(elementData_Renamed, index+1, elementData_Renamed, index, numMoved)
			size_Renamed -= 1
			elementData_Renamed(size_Renamed) = Nothing

			Return oldValue
		End Function

		''' <summary>
		''' Removes the first occurrence of the specified element from this list,
		''' if it is present.  If the list does not contain the element, it is
		''' unchanged.  More formally, removes the element with the lowest index
		''' <tt>i</tt> such that
		''' <tt>(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i)))</tt>
		''' (if such an element exists).  Returns <tt>true</tt> if this list
		''' contained the specified element (or equivalently, if this list
		''' changed as a result of the call).
		''' </summary>
		''' <param name="o"> element to be removed from this list, if present </param>
		''' <returns> <tt>true</tt> if this list contained the specified element </returns>
		Public Overridable Function remove(ByVal o As Object) As Boolean Implements List(Of E).remove
			If o Is Nothing Then
				For index As Integer = 0 To size_Renamed - 1
					If elementData_Renamed(index) Is Nothing Then
						fastRemove(index)
						Return True
					End If
				Next index
			Else
				For index As Integer = 0 To size_Renamed - 1
					If o.Equals(elementData_Renamed(index)) Then
						fastRemove(index)
						Return True
					End If
				Next index
			End If
			Return False
		End Function

	'    
	'     * Private remove method that skips bounds checking and does not
	'     * return the value removed.
	'     
		Private Sub fastRemove(ByVal index As Integer)
			modCount += 1
			Dim numMoved As Integer = size_Renamed - index - 1
			If numMoved > 0 Then Array.Copy(elementData_Renamed, index+1, elementData_Renamed, index, numMoved)
			size_Renamed -= 1
			elementData_Renamed(size_Renamed) = Nothing
		End Sub

		''' <summary>
		''' Removes all of the elements from this list.  The list will
		''' be empty after this call returns.
		''' </summary>
		Public Overridable Sub clear() Implements List(Of E).clear
			modCount += 1

			' clear to let GC do its work
			Dim i As Integer = 0
			Do While i < size_Renamed
				elementData_Renamed(i) = Nothing
				i += 1
			Loop

			size_Renamed = 0
		End Sub

		''' <summary>
		''' Appends all of the elements in the specified collection to the end of
		''' this list, in the order that they are returned by the
		''' specified collection's Iterator.  The behavior of this operation is
		''' undefined if the specified collection is modified while the operation
		''' is in progress.  (This implies that the behavior of this call is
		''' undefined if the specified collection is this list, and this
		''' list is nonempty.)
		''' </summary>
		''' <param name="c"> collection containing elements to be added to this list </param>
		''' <returns> <tt>true</tt> if this list changed as a result of the call </returns>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
		Public Overridable Function addAll(Of T1 As E)(ByVal c As Collection(Of T1)) As Boolean Implements List(Of E).addAll
			Dim a As Object() = c.ToArray()
			Dim numNew As Integer = a.Length
			ensureCapacityInternal(size_Renamed + numNew) ' Increments modCount
			Array.Copy(a, 0, elementData_Renamed, size_Renamed, numNew)
			size_Renamed += numNew
			Return numNew <> 0
		End Function

		''' <summary>
		''' Inserts all of the elements in the specified collection into this
		''' list, starting at the specified position.  Shifts the element
		''' currently at that position (if any) and any subsequent elements to
		''' the right (increases their indices).  The new elements will appear
		''' in the list in the order that they are returned by the
		''' specified collection's iterator.
		''' </summary>
		''' <param name="index"> index at which to insert the first element from the
		'''              specified collection </param>
		''' <param name="c"> collection containing elements to be added to this list </param>
		''' <returns> <tt>true</tt> if this list changed as a result of the call </returns>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
		Public Overridable Function addAll(Of T1 As E)(ByVal index As Integer, ByVal c As Collection(Of T1)) As Boolean Implements List(Of E).addAll
			rangeCheckForAdd(index)

			Dim a As Object() = c.ToArray()
			Dim numNew As Integer = a.Length
			ensureCapacityInternal(size_Renamed + numNew) ' Increments modCount

			Dim numMoved As Integer = size_Renamed - index
			If numMoved > 0 Then Array.Copy(elementData_Renamed, index, elementData_Renamed, index + numNew, numMoved)

			Array.Copy(a, 0, elementData_Renamed, index, numNew)
			size_Renamed += numNew
			Return numNew <> 0
		End Function

		''' <summary>
		''' Removes from this list all of the elements whose index is between
		''' {@code fromIndex}, inclusive, and {@code toIndex}, exclusive.
		''' Shifts any succeeding elements to the left (reduces their index).
		''' This call shortens the list by {@code (toIndex - fromIndex)} elements.
		''' (If {@code toIndex==fromIndex}, this operation has no effect.)
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> if {@code fromIndex} or
		'''         {@code toIndex} is out of range
		'''         ({@code fromIndex < 0 ||
		'''          fromIndex >= size() ||
		'''          toIndex > size() ||
		'''          toIndex < fromIndex}) </exception>
		Protected Friend Overridable Sub removeRange(ByVal fromIndex As Integer, ByVal toIndex As Integer)
			modCount += 1
			Dim numMoved As Integer = size_Renamed - toIndex
			Array.Copy(elementData_Renamed, toIndex, elementData_Renamed, fromIndex, numMoved)

			' clear to let GC do its work
			Dim newSize As Integer = size_Renamed - (toIndex-fromIndex)
			For i As Integer = newSize To size_Renamed - 1
				elementData_Renamed(i) = Nothing
			Next i
			size_Renamed = newSize
		End Sub

		''' <summary>
		''' Checks if the given index is in range.  If not, throws an appropriate
		''' runtime exception.  This method does *not* check if the index is
		''' negative: It is always used immediately prior to an array access,
		''' which throws an ArrayIndexOutOfBoundsException if index is negative.
		''' </summary>
		Private Sub rangeCheck(ByVal index As Integer)
			If index >= size_Renamed Then Throw New IndexOutOfBoundsException(outOfBoundsMsg(index))
		End Sub

		''' <summary>
		''' A version of rangeCheck used by add and addAll.
		''' </summary>
		Private Sub rangeCheckForAdd(ByVal index As Integer)
			If index > size_Renamed OrElse index < 0 Then Throw New IndexOutOfBoundsException(outOfBoundsMsg(index))
		End Sub

		''' <summary>
		''' Constructs an IndexOutOfBoundsException detail message.
		''' Of the many possible refactorings of the error handling code,
		''' this "outlining" performs best with both server and client VMs.
		''' </summary>
		Private Function outOfBoundsMsg(ByVal index As Integer) As String
			Return "Index: " & index & ", Size: " & size_Renamed
		End Function

		''' <summary>
		''' Removes from this list all of its elements that are contained in the
		''' specified collection.
		''' </summary>
		''' <param name="c"> collection containing elements to be removed from this list </param>
		''' <returns> {@code true} if this list changed as a result of the call </returns>
		''' <exception cref="ClassCastException"> if the class of an element of this list
		'''         is incompatible with the specified collection
		''' (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		''' <exception cref="NullPointerException"> if this list contains a null element and the
		'''         specified collection does not permit null elements
		''' (<a href="Collection.html#optional-restrictions">optional</a>),
		'''         or if the specified collection is null </exception>
		''' <seealso cref= Collection#contains(Object) </seealso>
		Public Overridable Function removeAll(Of T1)(ByVal c As Collection(Of T1)) As Boolean Implements List(Of E).removeAll
			Objects.requireNonNull(c)
			Return batchRemove(c, False)
		End Function

		''' <summary>
		''' Retains only the elements in this list that are contained in the
		''' specified collection.  In other words, removes from this list all
		''' of its elements that are not contained in the specified collection.
		''' </summary>
		''' <param name="c"> collection containing elements to be retained in this list </param>
		''' <returns> {@code true} if this list changed as a result of the call </returns>
		''' <exception cref="ClassCastException"> if the class of an element of this list
		'''         is incompatible with the specified collection
		''' (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		''' <exception cref="NullPointerException"> if this list contains a null element and the
		'''         specified collection does not permit null elements
		''' (<a href="Collection.html#optional-restrictions">optional</a>),
		'''         or if the specified collection is null </exception>
		''' <seealso cref= Collection#contains(Object) </seealso>
		Public Overridable Function retainAll(Of T1)(ByVal c As Collection(Of T1)) As Boolean Implements List(Of E).retainAll
			Objects.requireNonNull(c)
			Return batchRemove(c, True)
		End Function

		Private Function batchRemove(Of T1)(ByVal c As Collection(Of T1), ByVal complement As Boolean) As Boolean
			Dim elementData As Object() = Me.elementData_Renamed
			Dim r As Integer = 0, w As Integer = 0
			Dim modified As Boolean = False
			Try
				Do While r < size_Renamed
					If c.contains(elementData(r)) = complement Then
						elementData(w) = elementData(r)
						w += 1
					End If
					r += 1
				Loop
			Finally
				' Preserve behavioral compatibility with AbstractCollection,
				' even if c.contains() throws.
				If r <> size_Renamed Then
					Array.Copy(elementData, r, elementData, w, size_Renamed - r)
					w += size_Renamed - r
				End If
				If w <> size_Renamed Then
					' clear to let GC do its work
					Dim i As Integer = w
					Do While i < size_Renamed
						elementData(i) = Nothing
						i += 1
					Loop
					modCount += size_Renamed - w
					size_Renamed = w
					modified = True
				End If
			End Try
			Return modified
		End Function

		''' <summary>
		''' Save the state of the <tt>ArrayList</tt> instance to a stream (that
		''' is, serialize it).
		''' 
		''' @serialData The length of the array backing the <tt>ArrayList</tt>
		'''             instance is emitted (int), followed by all of its elements
		'''             (each an <tt>Object</tt>) in the proper order.
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			' Write out element count, and any hidden stuff
			Dim expectedModCount As Integer = modCount
			s.defaultWriteObject()

			' Write out size as capacity for behavioural compatibility with clone()
			s.writeInt(size_Renamed)

			' Write out all elements in the proper order.
			For i As Integer = 0 To size_Renamed - 1
				s.writeObject(elementData_Renamed(i))
			Next i

			If modCount <> expectedModCount Then Throw New ConcurrentModificationException
		End Sub

		''' <summary>
		''' Reconstitute the <tt>ArrayList</tt> instance from a stream (that is,
		''' deserialize it).
		''' </summary>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			elementData_Renamed = EMPTY_ELEMENTDATA

			' Read in size, and any hidden stuff
			s.defaultReadObject()

			' Read in capacity
			s.readInt() ' ignored

			If size_Renamed > 0 Then
				' be like clone(), allocate array based upon size not capacity
				ensureCapacityInternal(size_Renamed)

				Dim a As Object() = elementData_Renamed
				' Read in all elements in the proper order.
				For i As Integer = 0 To size_Renamed - 1
					a(i) = s.readObject()
				Next i
			End If
		End Sub

		''' <summary>
		''' Returns a list iterator over the elements in this list (in proper
		''' sequence), starting at the specified position in the list.
		''' The specified index indicates the first element that would be
		''' returned by an initial call to <seealso cref="ListIterator#next next"/>.
		''' An initial call to <seealso cref="ListIterator#previous previous"/> would
		''' return the element with the specified index minus one.
		''' 
		''' <p>The returned list iterator is <a href="#fail-fast"><i>fail-fast</i></a>.
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overridable Function listIterator(ByVal index As Integer) As ListIterator(Of E) Implements List(Of E).listIterator
			If index < 0 OrElse index > size_Renamed Then Throw New IndexOutOfBoundsException("Index: " & index)
			Return New ListItr(Me, index)
		End Function

		''' <summary>
		''' Returns a list iterator over the elements in this list (in proper
		''' sequence).
		''' 
		''' <p>The returned list iterator is <a href="#fail-fast"><i>fail-fast</i></a>.
		''' </summary>
		''' <seealso cref= #listIterator(int) </seealso>
		Public Overridable Function listIterator() As ListIterator(Of E) Implements List(Of E).listIterator
			Return New ListItr(Me, 0)
		End Function

		''' <summary>
		''' Returns an iterator over the elements in this list in proper sequence.
		''' 
		''' <p>The returned iterator is <a href="#fail-fast"><i>fail-fast</i></a>.
		''' </summary>
		''' <returns> an iterator over the elements in this list in proper sequence </returns>
		Public Overridable Function [iterator]() As [Iterator](Of E) Implements List(Of E).iterator
			Return New Itr(Me)
		End Function

		''' <summary>
		''' An optimized version of AbstractList.Itr
		''' </summary>
		Private Class Itr
			Implements Iterator(Of E)

			Private ReadOnly outerInstance As ArrayList

			Public Sub New(ByVal outerInstance As ArrayList)
				Me.outerInstance = outerInstance
			End Sub

			Friend cursor As Integer ' index of next element to return
			Friend lastRet As Integer = -1 ' index of last element returned; -1 if no such
			Friend expectedModCount As Integer = outerInstance.modCount

			Public Overridable Function hasNext() As Boolean Implements Iterator(Of E).hasNext
				Return cursor <> outerInstance.size_Renamed
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Function [next]() As E Implements Iterator(Of E).next
				checkForComodification()
				Dim i As Integer = cursor
				If i >= outerInstance.size_Renamed Then Throw New NoSuchElementException
				Dim elementData As Object() = outerInstance.elementData
				If i >= elementData.Length Then Throw New ConcurrentModificationException
				cursor = i + 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return CType(elementData(lastRet = i), E)
			End Function

			Public Overridable Sub remove() Implements Iterator(Of E).remove
				If lastRet < 0 Then Throw New IllegalStateException
				checkForComodification()

				Try
					outerInstance.remove(lastRet)
					cursor = lastRet
					lastRet = -1
					expectedModCount = outerInstance.modCount
				Catch ex As IndexOutOfBoundsException
					Throw New ConcurrentModificationException
				End Try
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEachRemaining(Of T1)(ByVal consumer As java.util.function.Consumer(Of T1)) Implements Iterator(Of E).forEachRemaining
				Objects.requireNonNull(consumer)
				Dim size As Integer = outerInstance.size
				Dim i As Integer = cursor
				If i >= size Then Return
				Dim elementData As Object() = outerInstance.elementData
				If i >= elementData.Length Then Throw New ConcurrentModificationException
				Do While i <> size AndAlso outerInstance.modCount = expectedModCount
					consumer.accept(CType(elementData(i), E))
					i += 1
				Loop
				' update once at end of iteration to reduce heap write traffic
				cursor = i
				lastRet = i - 1
				checkForComodification()
			End Sub

			Friend Sub checkForComodification()
				If outerInstance.modCount <> expectedModCount Then Throw New ConcurrentModificationException
			End Sub
		End Class

		''' <summary>
		''' An optimized version of AbstractList.ListItr
		''' </summary>
		Private Class ListItr
			Inherits Itr
			Implements ListIterator(Of E)

			Private ReadOnly outerInstance As ArrayList

			Friend Sub New(ByVal outerInstance As ArrayList, ByVal index As Integer)
                MyBase.New(outerInstance)
                Me.outerInstance = outerInstance
                Me.cursor = index
            End Sub

			Public Overridable Function hasPrevious() As Boolean Implements ListIterator(Of E).hasPrevious
				Return cursor <> 0
			End Function

			Public Overridable Function nextIndex() As Integer Implements ListIterator(Of E).nextIndex
				Return cursor
			End Function

			Public Overridable Function previousIndex() As Integer Implements ListIterator(Of E).previousIndex
				Return cursor - 1
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Function previous() As E Implements ListIterator(Of E).previous
				checkForComodification()
				Dim i As Integer = cursor - 1
				If i < 0 Then Throw New NoSuchElementException
				Dim elementData As Object() = outerInstance.elementData
				If i >= elementData.Length Then Throw New ConcurrentModificationException
				cursor = i
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return CType(elementData(lastRet = i), E)
			End Function

			Public Overridable Sub [set](ByVal e As E)
				If lastRet < 0 Then Throw New IllegalStateException
				checkForComodification()

				Try
					outerInstance.set(lastRet, e)
				Catch ex As IndexOutOfBoundsException
					Throw New ConcurrentModificationException
				End Try
			End Sub

			Public Overridable Sub add(ByVal e As E)
				checkForComodification()

				Try
					Dim i As Integer = cursor
					outerInstance.add(i, e)
					cursor = i + 1
					lastRet = -1
					expectedModCount = outerInstance.modCount
				Catch ex As IndexOutOfBoundsException
					Throw New ConcurrentModificationException
				End Try
			End Sub
		End Class

		''' <summary>
		''' Returns a view of the portion of this list between the specified
		''' {@code fromIndex}, inclusive, and {@code toIndex}, exclusive.  (If
		''' {@code fromIndex} and {@code toIndex} are equal, the returned list is
		''' empty.)  The returned list is backed by this list, so non-structural
		''' changes in the returned list are reflected in this list, and vice-versa.
		''' The returned list supports all of the optional list operations.
		''' 
		''' <p>This method eliminates the need for explicit range operations (of
		''' the sort that commonly exist for arrays).  Any operation that expects
		''' a list can be used as a range operation by passing a subList view
		''' instead of a whole list.  For example, the following idiom
		''' removes a range of elements from a list:
		''' <pre>
		'''      list.subList(from, to).clear();
		''' </pre>
		''' Similar idioms may be constructed for <seealso cref="#indexOf(Object)"/> and
		''' <seealso cref="#lastIndexOf(Object)"/>, and all of the algorithms in the
		''' <seealso cref="Collections"/> class can be applied to a subList.
		''' 
		''' <p>The semantics of the list returned by this method become undefined if
		''' the backing list (i.e., this list) is <i>structurally modified</i> in
		''' any way other than via the returned list.  (Structural modifications are
		''' those that change the size of this list, or otherwise perturb it in such
		''' a fashion that iterations in progress may yield incorrect results.)
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Public Overridable Function subList(ByVal fromIndex As Integer, ByVal toIndex As Integer) As List(Of E) Implements List(Of E).subList
			subListRangeCheck(fromIndex, toIndex, size_Renamed)
			Return New SubList(Me, Me, 0, fromIndex, toIndex)
		End Function

		Friend Shared Sub subListRangeCheck(ByVal fromIndex As Integer, ByVal toIndex As Integer, ByVal size As Integer)
			If fromIndex < 0 Then Throw New IndexOutOfBoundsException("fromIndex = " & fromIndex)
			If toIndex > size Then Throw New IndexOutOfBoundsException("toIndex = " & toIndex)
			If fromIndex > toIndex Then Throw New IllegalArgumentException("fromIndex(" & fromIndex & ") > toIndex(" & toIndex & ")")
		End Sub

		Private Class SubList
			Inherits AbstractList(Of E)
			Implements RandomAccess

			Private ReadOnly outerInstance As ArrayList

			Private ReadOnly parent As AbstractList(Of E)
			Private ReadOnly parentOffset As Integer
			Private ReadOnly offset As Integer
			Friend size_Renamed As Integer

			Friend Sub New(ByVal outerInstance As ArrayList, ByVal parent As AbstractList(Of E), ByVal offset As Integer, ByVal fromIndex As Integer, ByVal toIndex As Integer)
					Me.outerInstance = outerInstance
				Me.parent = parent
				Me.parentOffset = fromIndex
				Me.offset = offset + fromIndex
				Me.size_Renamed = toIndex - fromIndex
				Me.modCount = outerInstance.modCount
			End Sub

			Public Overridable Function [set](ByVal index As Integer, ByVal e As E) As E
				rangeCheck(index)
				checkForComodification()
				Dim oldValue As E = outerInstance.elementData(offset + index)
				outerInstance.elementData(offset + index) = e
				Return oldValue
			End Function

			Public Overridable Function [get](ByVal index As Integer) As E
				rangeCheck(index)
				checkForComodification()
				Return outerInstance.elementData(offset + index)
			End Function

			Public Overridable Function size() As Integer
				checkForComodification()
				Return Me.size_Renamed
			End Function

			Public Overridable Sub add(ByVal index As Integer, ByVal e As E)
				rangeCheckForAdd(index)
				checkForComodification()
				parent.add(parentOffset + index, e)
				Me.modCount = parent.modCount
				Me.size_Renamed += 1
			End Sub

			Public Overridable Function remove(ByVal index As Integer) As E
				rangeCheck(index)
				checkForComodification()
				Dim result As E = parent.remove(parentOffset + index)
				Me.modCount = parent.modCount
				Me.size_Renamed -= 1
				Return result
			End Function

			Protected Friend Overridable Sub removeRange(ByVal fromIndex As Integer, ByVal toIndex As Integer)
				checkForComodification()
				parent.removeRange(parentOffset + fromIndex, parentOffset + toIndex)
				Me.modCount = parent.modCount
				Me.size_Renamed -= toIndex - fromIndex
			End Sub

			Public Overridable Function addAll(Of T1 As E)(ByVal c As Collection(Of T1)) As Boolean
				Return addAll(Me.size_Renamed, c)
			End Function

			Public Overridable Function addAll(Of T1 As E)(ByVal index As Integer, ByVal c As Collection(Of T1)) As Boolean
				rangeCheckForAdd(index)
				Dim cSize As Integer = c.size()
				If cSize=0 Then Return False

				checkForComodification()
				parent.addAll(parentOffset + index, c)
				Me.modCount = parent.modCount
				Me.size_Renamed += cSize
				Return True
			End Function

			Public Overridable Function [iterator]() As [Iterator](Of E)
				Return listIterator()
			End Function

			Public Overridable Function listIterator(ByVal index As Integer) As ListIterator(Of E)
				checkForComodification()
				rangeCheckForAdd(index)
				Dim offset As Integer = Me.offset

				Return New ListIteratorAnonymousInnerClassHelper(Of E)
			End Function

			Private Class ListIteratorAnonymousInnerClassHelper(Of E)
				Implements ListIterator(Of E)

				Friend cursor As Integer = index
				Friend lastRet As Integer = -1
				Friend expectedModCount As Integer = outerInstance.modCount

				Public Overridable Function hasNext() As Boolean Implements ListIterator(Of E).hasNext
					Return cursor <> outerInstance.size
				End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Public Overridable Function [next]() As E Implements ListIterator(Of E).next
					outerInstance.checkForComodification()
					Dim i As Integer = cursor
					If i >= outerInstance.size Then Throw New NoSuchElementException
					Dim elementData As Object() = outerInstance.elementData
					If outerInstance.offset + i >= elementData.Length Then Throw New ConcurrentModificationException
					cursor = i + 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return CType(elementData(outerInstance.offset + (lastRet = i)), E)
				End Function

				Public Overridable Function hasPrevious() As Boolean Implements ListIterator(Of E).hasPrevious
					Return cursor <> 0
				End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Public Overridable Function previous() As E Implements ListIterator(Of E).previous
					outerInstance.checkForComodification()
					Dim i As Integer = cursor - 1
					If i < 0 Then Throw New NoSuchElementException
					Dim elementData As Object() = outerInstance.elementData
					If outerInstance.offset + i >= elementData.Length Then Throw New ConcurrentModificationException
					cursor = i
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return CType(elementData(outerInstance.offset + (lastRet = i)), E)
				End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Public Overridable Sub forEachRemaining(Of T1)(ByVal consumer As java.util.function.Consumer(Of T1))
					Objects.requireNonNull(consumer)
					Dim size As Integer = outerInstance.size
					Dim i As Integer = cursor
					If i >= size Then Return
					Dim elementData As Object() = outerInstance.elementData
					If outerInstance.offset + i >= elementData.Length Then Throw New ConcurrentModificationException
					Do While i <> size AndAlso outerInstance.modCount = expectedModCount
						consumer.accept(CType(elementData(outerInstance.offset + (i)), E))
						i += 1
					Loop
					' update once at end of iteration to reduce heap write traffic
						cursor = i
						lastRet = cursor
					outerInstance.checkForComodification()
				End Sub

				Public Overridable Function nextIndex() As Integer Implements ListIterator(Of E).nextIndex
					Return cursor
				End Function

				Public Overridable Function previousIndex() As Integer Implements ListIterator(Of E).previousIndex
					Return cursor - 1
				End Function

				Public Overridable Sub remove() Implements ListIterator(Of E).remove
					If lastRet < 0 Then Throw New IllegalStateException
					outerInstance.checkForComodification()

					Try
						outerInstance.remove(lastRet)
						cursor = lastRet
						lastRet = -1
						expectedModCount = outerInstance.modCount
					Catch ex As IndexOutOfBoundsException
						Throw New ConcurrentModificationException
					End Try
				End Sub

				Public Overridable Sub [set](ByVal e As E)
					If lastRet < 0 Then Throw New IllegalStateException
					outerInstance.checkForComodification()

					Try
						outerInstance.set(outerInstance.offset + lastRet, e)
					Catch ex As IndexOutOfBoundsException
						Throw New ConcurrentModificationException
					End Try
				End Sub

				Public Overridable Sub add(ByVal e As E)
					outerInstance.checkForComodification()

					Try
						Dim i As Integer = cursor
						outerInstance.add(i, e)
						cursor = i + 1
						lastRet = -1
						expectedModCount = outerInstance.modCount
					Catch ex As IndexOutOfBoundsException
						Throw New ConcurrentModificationException
					End Try
				End Sub

				Friend Sub checkForComodification()
					If expectedModCount <> outerInstance.modCount Then Throw New ConcurrentModificationException
				End Sub
			End Class

			Public Overridable Function subList(ByVal fromIndex As Integer, ByVal toIndex As Integer) As List(Of E)
				subListRangeCheck(fromIndex, toIndex, size_Renamed)
				Return New SubList(Me, offset, fromIndex, toIndex)
			End Function

			Private Sub rangeCheck(ByVal index As Integer)
				If index < 0 OrElse index >= Me.size_Renamed Then Throw New IndexOutOfBoundsException(outOfBoundsMsg(index))
			End Sub

			Private Sub rangeCheckForAdd(ByVal index As Integer)
				If index < 0 OrElse index > Me.size_Renamed Then Throw New IndexOutOfBoundsException(outOfBoundsMsg(index))
			End Sub

			Private Function outOfBoundsMsg(ByVal index As Integer) As String
				Return "Index: " & index & ", Size: " & Me.size_Renamed
			End Function

			Private Sub checkForComodification()
				If outerInstance.modCount <> Me.modCount Then Throw New ConcurrentModificationException
			End Sub

			Public Overridable Function spliterator() As Spliterator(Of E)
				checkForComodification()
				Return New ArrayListSpliterator(Of E)(ArrayList.this, offset, offset + Me.size_Renamed, Me.modCount)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
			Objects.requireNonNull(action)
			Dim expectedModCount As Integer = modCount
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim elementData As E() = CType(Me.elementData_Renamed, E())
			Dim size As Integer = Me.size_Renamed
			Dim i As Integer=0
			Do While modCount = expectedModCount AndAlso i < size
				action.accept(elementData(i))
				i += 1
			Loop
			If modCount <> expectedModCount Then Throw New ConcurrentModificationException
		End Sub

		''' <summary>
		''' Creates a <em><a href="Spliterator.html#binding">late-binding</a></em>
		''' and <em>fail-fast</em> <seealso cref="Spliterator"/> over the elements in this
		''' list.
		''' 
		''' <p>The {@code Spliterator} reports <seealso cref="Spliterator#SIZED"/>,
		''' <seealso cref="Spliterator#SUBSIZED"/>, and <seealso cref="Spliterator#ORDERED"/>.
		''' Overriding implementations should document the reporting of additional
		''' characteristic values.
		''' </summary>
		''' <returns> a {@code Spliterator} over the elements in this list
		''' @since 1.8 </returns>
		Public Overrides Function spliterator() As Spliterator(Of E) Implements List(Of E).spliterator
			Return New ArrayListSpliterator(Of )(Me, 0, -1, 0)
		End Function

		''' <summary>
		''' Index-based split-by-two, lazily initialized Spliterator </summary>
		Friend NotInheritable Class ArrayListSpliterator(Of E)
			Implements Spliterator(Of E)

	'        
	'         * If ArrayLists were immutable, or structurally immutable (no
	'         * adds, removes, etc), we could implement their spliterators
	'         * with Arrays.spliterator. Instead we detect as much
	'         * interference during traversal as practical without
	'         * sacrificing much performance. We rely primarily on
	'         * modCounts. These are not guaranteed to detect concurrency
	'         * violations, and are sometimes overly conservative about
	'         * within-thread interference, but detect enough problems to
	'         * be worthwhile in practice. To carry this out, we (1) lazily
	'         * initialize fence and expectedModCount until the latest
	'         * point that we need to commit to the state we are checking
	'         * against; thus improving precision.  (This doesn't apply to
	'         * SubLists, that create spliterators with current non-lazy
	'         * values).  (2) We perform only a single
	'         * ConcurrentModificationException check at the end of forEach
	'         * (the most performance-sensitive method). When using forEach
	'         * (as opposed to iterators), we can normally only detect
	'         * interference after actions, not before. Further
	'         * CME-triggering checks apply to all other possible
	'         * violations of assumptions for example null or too-small
	'         * elementData array given its size(), that could only have
	'         * occurred due to interference.  This allows the inner loop
	'         * of forEach to run without any further checks, and
	'         * simplifies lambda-resolution. While this does entail a
	'         * number of checks, note that in the common case of
	'         * list.stream().forEach(a), no checks or other computation
	'         * occur anywhere other than inside forEach itself.  The other
	'         * less-often-used methods cannot take advantage of most of
	'         * these streamlinings.
	'         

			Private ReadOnly list As List(Of E)
			Private index As Integer ' current index, modified on advance/split
			Private fence As Integer ' -1 until used; then one past last index
			Private expectedModCount As Integer ' initialized when fence set

			''' <summary>
			''' Create new spliterator covering the given  range </summary>
			Friend Sub New(ByVal list As List(Of E), ByVal origin As Integer, ByVal fence As Integer, ByVal expectedModCount As Integer)
				Me.list = list ' OK if null unless traversed
				Me.index = origin
				Me.fence = fence
				Me.expectedModCount = expectedModCount
			End Sub

			Private Property fence As Integer
				Get
					Dim hi As Integer ' (a specialized variant appears in method forEach)
					Dim lst As List(Of E)
					hi = fence
					If hi < 0 Then
						lst = list
						If lst Is Nothing Then
								fence = 0
								hi = fence
						Else
							expectedModCount = lst.modCount
								fence = lst.size
								hi = fence
						End If
					End If
					Return hi
				End Get
			End Property

			Public Function trySplit() As ArrayListSpliterator(Of E)
				Dim hi As Integer = fence, lo As Integer = index, mid As Integer = CInt(CUInt((lo + hi)) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If(lo >= mid, Nothing, New ArrayListSpliterator(Of E)(list, lo, index = mid, expectedModCount)) ' divide range in half unless too small
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of E).tryAdvance
				If action Is Nothing Then Throw New NullPointerException
				Dim hi As Integer = fence, i As Integer = index
				If i < hi Then
					index = i + 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim e As E = CType(list.elementData_Renamed(i), E)
					action.accept(e)
					If list.modCount <> expectedModCount Then Throw New ConcurrentModificationException
					Return True
				End If
				Return False
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Spliterator(Of E).forEachRemaining
				Dim i, hi, mc As Integer ' hoist accesses and checks from loop
				Dim lst As List(Of E)
				Dim a As Object()
				If action Is Nothing Then Throw New NullPointerException
				lst = list
				a = lst.elementData_Renamed
				If lst IsNot Nothing AndAlso a IsNot Nothing Then
					hi = fence
					If hi < 0 Then
						mc = lst.modCount
						hi = lst.size
					Else
						mc = expectedModCount
					End If
					i = index
					index = hi
					If i >= 0 AndAlso index <= a.Length Then
						Do While i < hi
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
							Dim e As E = CType(a(i), E)
							action.accept(e)
							i += 1
						Loop
						If lst.modCount = mc Then Return
					End If
				End If
				Throw New ConcurrentModificationException
			End Sub

			Public Function estimateSize() As Long Implements Spliterator(Of E).estimateSize
				Return CLng(fence - index)
			End Function

			Public Function characteristics() As Integer Implements Spliterator(Of E).characteristics
				Return Spliterator.ORDERED Or Spliterator.SIZED Or Spliterator.SUBSIZED
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Function removeIf(Of T1)(ByVal filter As java.util.function.Predicate(Of T1)) As Boolean
			Objects.requireNonNull(filter)
			' figure out which elements are to be removed
			' any exception thrown from the filter predicate at this stage
			' will leave the collection unmodified
			Dim removeCount As Integer = 0
			Dim removeSet As New BitSet(size_Renamed)
			Dim expectedModCount As Integer = modCount
			Dim size As Integer = Me.size_Renamed
			Dim i As Integer=0
			Do While modCount = expectedModCount AndAlso i < size
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim element As E = CType(elementData_Renamed(i), E)
				If filter.test(element) Then
					removeSet.set(i)
					removeCount += 1
				End If
				i += 1
			Loop
			If modCount <> expectedModCount Then Throw New ConcurrentModificationException

			' shift surviving elements left over the spaces left by removed elements
			Dim anyToRemove As Boolean = removeCount > 0
			If anyToRemove Then
				Dim newSize As Integer = size - removeCount
				i = 0
				Dim j As Integer=0
				Do While (i < size) AndAlso (j < newSize)
					i = removeSet.nextClearBit(i)
					elementData_Renamed(j) = elementData_Renamed(i)
					i += 1
					j += 1
				Loop
				For k As Integer = newSize To size - 1
					elementData_Renamed(k) = Nothing ' Let gc do its work
				Next k
				Me.size_Renamed = newSize
				If modCount <> expectedModCount Then Throw New ConcurrentModificationException
				modCount += 1
			End If

			Return anyToRemove
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Sub replaceAll(ByVal [operator] As java.util.function.UnaryOperator(Of E)) Implements List(Of E).replaceAll
			Objects.requireNonNull([operator])
			Dim expectedModCount As Integer = modCount
			Dim size As Integer = Me.size_Renamed
			Dim i As Integer=0
			Do While modCount = expectedModCount AndAlso i < size
				elementData_Renamed(i) = [operator].apply(CType(elementData_Renamed(i), E))
				i += 1
			Loop
			If modCount <> expectedModCount Then Throw New ConcurrentModificationException
			modCount += 1
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Sub sort(Of T1)(ByVal c As Comparator(Of T1)) Implements List(Of E).sort
			Dim expectedModCount As Integer = modCount
			Array.Sort(CType(elementData_Renamed, E()), 0, size_Renamed, c)
			If modCount <> expectedModCount Then Throw New ConcurrentModificationException
			modCount += 1
		End Sub
	End Class

End Namespace
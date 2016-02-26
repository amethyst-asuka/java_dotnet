Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

'
' * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The {@code Vector} class implements a growable array of
	''' objects. Like an array, it contains components that can be
	''' accessed using an integer index. However, the size of a
	''' {@code Vector} can grow or shrink as needed to accommodate
	''' adding and removing items after the {@code Vector} has been created.
	''' 
	''' <p>Each vector tries to optimize storage management by maintaining a
	''' {@code capacity} and a {@code capacityIncrement}. The
	''' {@code capacity} is always at least as large as the vector
	''' size; it is usually larger because as components are added to the
	''' vector, the vector's storage increases in chunks the size of
	''' {@code capacityIncrement}. An application can increase the
	''' capacity of a vector before inserting a large number of
	''' components; this reduces the amount of incremental reallocation.
	''' 
	''' <p><a name="fail-fast">
	''' The iterators returned by this class's <seealso cref="#iterator() iterator"/> and
	''' <seealso cref="#listIterator(int) listIterator"/> methods are <em>fail-fast</em></a>:
	''' if the vector is structurally modified at any time after the iterator is
	''' created, in any way except through the iterator's own
	''' <seealso cref="ListIterator#remove() remove"/> or
	''' <seealso cref="ListIterator#add(Object) add"/> methods, the iterator will throw a
	''' <seealso cref="ConcurrentModificationException"/>.  Thus, in the face of
	''' concurrent modification, the iterator fails quickly and cleanly, rather
	''' than risking arbitrary, non-deterministic behavior at an undetermined
	''' time in the future.  The <seealso cref="Enumeration Enumerations"/> returned by
	''' the <seealso cref="#elements() elements"/> method are <em>not</em> fail-fast.
	''' 
	''' <p>Note that the fail-fast behavior of an iterator cannot be guaranteed
	''' as it is, generally speaking, impossible to make any hard guarantees in the
	''' presence of unsynchronized concurrent modification.  Fail-fast iterators
	''' throw {@code ConcurrentModificationException} on a best-effort basis.
	''' Therefore, it would be wrong to write a program that depended on this
	''' exception for its correctness:  <i>the fail-fast behavior of iterators
	''' should be used only to detect bugs.</i>
	''' 
	''' <p>As of the Java 2 platform v1.2, this class was retrofitted to
	''' implement the <seealso cref="List"/> interface, making it a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.  Unlike the new collection
	''' implementations, {@code Vector} is synchronized.  If a thread-safe
	''' implementation is not needed, it is recommended to use {@link
	''' ArrayList} in place of {@code Vector}.
	''' 
	''' @author  Lee Boynton
	''' @author  Jonathan Payne </summary>
	''' <seealso cref= Collection </seealso>
	''' <seealso cref= LinkedList
	''' @since   JDK1.0 </seealso>
	<Serializable> _
	Public Class Vector(Of E)
		Inherits AbstractList(Of E)
		Implements List(Of E), RandomAccess, Cloneable

		''' <summary>
		''' The array buffer into which the components of the vector are
		''' stored. The capacity of the vector is the length of this array buffer,
		''' and is at least large enough to contain all the vector's elements.
		''' 
		''' <p>Any array elements following the last element in the Vector are null.
		''' 
		''' @serial
		''' </summary>
		Protected Friend elementData_Renamed As Object()

		''' <summary>
		''' The number of valid components in this {@code Vector} object.
		''' Components {@code elementData[0]} through
		''' {@code elementData[elementCount-1]} are the actual items.
		''' 
		''' @serial
		''' </summary>
		Protected Friend elementCount As Integer

		''' <summary>
		''' The amount by which the capacity of the vector is automatically
		''' incremented when its size becomes greater than its capacity.  If
		''' the capacity increment is less than or equal to zero, the capacity
		''' of the vector is doubled each time it needs to grow.
		''' 
		''' @serial
		''' </summary>
		Protected Friend capacityIncrement As Integer

		''' <summary>
		''' use serialVersionUID from JDK 1.0.2 for interoperability </summary>
		Private Const serialVersionUID As Long = -2767605614048989439L

		''' <summary>
		''' Constructs an empty vector with the specified initial capacity and
		''' capacity increment.
		''' </summary>
		''' <param name="initialCapacity">     the initial capacity of the vector </param>
		''' <param name="capacityIncrement">   the amount by which the capacity is
		'''                              increased when the vector overflows </param>
		''' <exception cref="IllegalArgumentException"> if the specified initial capacity
		'''         is negative </exception>
		Public Sub New(ByVal initialCapacity As Integer, ByVal capacityIncrement As Integer)
			MyBase.New()
			If initialCapacity < 0 Then Throw New IllegalArgumentException("Illegal Capacity: " & initialCapacity)
			Me.elementData_Renamed = New Object(initialCapacity - 1){}
			Me.capacityIncrement = capacityIncrement
		End Sub

		''' <summary>
		''' Constructs an empty vector with the specified initial capacity and
		''' with its capacity increment equal to zero.
		''' </summary>
		''' <param name="initialCapacity">   the initial capacity of the vector </param>
		''' <exception cref="IllegalArgumentException"> if the specified initial capacity
		'''         is negative </exception>
		Public Sub New(ByVal initialCapacity As Integer)
			Me.New(initialCapacity, 0)
		End Sub

		''' <summary>
		''' Constructs an empty vector so that its internal data array
		''' has size {@code 10} and its standard capacity increment is
		''' zero.
		''' </summary>
		Public Sub New()
			Me.New(10)
		End Sub

		''' <summary>
		''' Constructs a vector containing the elements of the specified
		''' collection, in the order they are returned by the collection's
		''' iterator.
		''' </summary>
		''' <param name="c"> the collection whose elements are to be placed into this
		'''       vector </param>
		''' <exception cref="NullPointerException"> if the specified collection is null
		''' @since   1.2 </exception>
		Public Sub New(Of T1 As E)(ByVal c As Collection(Of T1))
			elementData_Renamed = c.ToArray()
			elementCount = elementData_Renamed.Length
			' c.toArray might (incorrectly) not return Object[] (see 6260652)
			If elementData_Renamed.GetType() IsNot GetType(Object()) Then elementData_Renamed = Arrays.copyOf(elementData_Renamed, elementCount, GetType(Object()))
		End Sub

		''' <summary>
		''' Copies the components of this vector into the specified array.
		''' The item at index {@code k} in this vector is copied into
		''' component {@code k} of {@code anArray}.
		''' </summary>
		''' <param name="anArray"> the array into which the components get copied </param>
		''' <exception cref="NullPointerException"> if the given array is null </exception>
		''' <exception cref="IndexOutOfBoundsException"> if the specified array is not
		'''         large enough to hold all the components of this vector </exception>
		''' <exception cref="ArrayStoreException"> if a component of this vector is not of
		'''         a runtime type that can be stored in the specified array </exception>
		''' <seealso cref= #toArray(Object[]) </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub copyInto(ByVal anArray As Object())
			Array.Copy(elementData_Renamed, 0, anArray, 0, elementCount)
		End Sub

		''' <summary>
		''' Trims the capacity of this vector to be the vector's current
		''' size. If the capacity of this vector is larger than its current
		''' size, then the capacity is changed to equal the size by replacing
		''' its internal data array, kept in the field {@code elementData},
		''' with a smaller one. An application can use this operation to
		''' minimize the storage of a vector.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub trimToSize()
			modCount += 1
			Dim oldCapacity As Integer = elementData_Renamed.Length
			If elementCount < oldCapacity Then
				elementData = New java.lang.Object(elementCount - 1){}
				Array.Copy(elementData_Renamed, elementData_Renamed, elementCount)
			End If
		End Sub

		''' <summary>
		''' Increases the capacity of this vector, if necessary, to ensure
		''' that it can hold at least the number of components specified by
		''' the minimum capacity argument.
		''' 
		''' <p>If the current capacity of this vector is less than
		''' {@code minCapacity}, then its capacity is increased by replacing its
		''' internal data array, kept in the field {@code elementData}, with a
		''' larger one.  The size of the new data array will be the old size plus
		''' {@code capacityIncrement}, unless the value of
		''' {@code capacityIncrement} is less than or equal to zero, in which case
		''' the new capacity will be twice the old capacity; but if this new size
		''' is still smaller than {@code minCapacity}, then the new capacity will
		''' be {@code minCapacity}.
		''' </summary>
		''' <param name="minCapacity"> the desired minimum capacity </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub ensureCapacity(ByVal minCapacity As Integer)
			If minCapacity > 0 Then
				modCount += 1
				ensureCapacityHelper(minCapacity)
			End If
		End Sub

		''' <summary>
		''' This implements the unsynchronized semantics of ensureCapacity.
		''' Synchronized methods in this class can internally call this
		''' method for ensuring capacity without incurring the cost of an
		''' extra synchronization.
		''' </summary>
		''' <seealso cref= #ensureCapacity(int) </seealso>
		Private Sub ensureCapacityHelper(ByVal minCapacity As Integer)
			' overflow-conscious code
			If minCapacity - elementData_Renamed.Length > 0 Then grow(minCapacity)
		End Sub

		''' <summary>
		''' The maximum size of array to allocate.
		''' Some VMs reserve some header words in an array.
		''' Attempts to allocate larger arrays may result in
		''' OutOfMemoryError: Requested array size exceeds VM limit
		''' </summary>
		Private Shared ReadOnly MAX_ARRAY_SIZE As Integer =  [Integer].MAX_VALUE - 8

		Private Sub grow(ByVal minCapacity As Integer)
			' overflow-conscious code
			Dim oldCapacity As Integer = elementData_Renamed.Length
			Dim newCapacity As Integer = oldCapacity + (If(capacityIncrement > 0, capacityIncrement, oldCapacity))
			If newCapacity - minCapacity < 0 Then newCapacity = minCapacity
			If newCapacity - MAX_ARRAY_SIZE > 0 Then newCapacity = hugeCapacity(minCapacity)
			elementData = New java.lang.Object(newCapacity - 1){}
			Array.Copy(elementData_Renamed, elementData_Renamed, newCapacity)
		End Sub

		Private Shared Function hugeCapacity(ByVal minCapacity As Integer) As Integer
			If minCapacity < 0 Then ' overflow Throw New OutOfMemoryError
			Return If(minCapacity > MAX_ARRAY_SIZE, Integer.MaxValue, MAX_ARRAY_SIZE)
		End Function

		''' <summary>
		''' Sets the size of this vector. If the new size is greater than the
		''' current size, new {@code null} items are added to the end of
		''' the vector. If the new size is less than the current size, all
		''' components at index {@code newSize} and greater are discarded.
		''' </summary>
		''' <param name="newSize">   the new size of this vector </param>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the new size is negative </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property size As Integer
			Set(ByVal newSize As Integer)
				modCount += 1
				If newSize > elementCount Then
					ensureCapacityHelper(newSize)
				Else
					For i As Integer = newSize To elementCount - 1
						elementData_Renamed(i) = Nothing
					Next i
				End If
				elementCount = newSize
			End Set
		End Property

		''' <summary>
		''' Returns the current capacity of this vector.
		''' </summary>
		''' <returns>  the current capacity (the length of its internal
		'''          data array, kept in the field {@code elementData}
		'''          of this vector) </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function capacity() As Integer
			Return elementData_Renamed.Length
		End Function

		''' <summary>
		''' Returns the number of components in this vector.
		''' </summary>
		''' <returns>  the number of components in this vector </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function size() As Integer Implements List(Of E).size
			Return elementCount
		End Function

		''' <summary>
		''' Tests if this vector has no components.
		''' </summary>
		''' <returns>  {@code true} if and only if this vector has
		'''          no components, that is, its size is zero;
		'''          {@code false} otherwise. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property empty As Boolean Implements List(Of E).isEmpty
			Get
				Return elementCount = 0
			End Get
		End Property

		''' <summary>
		''' Returns an enumeration of the components of this vector. The
		''' returned {@code Enumeration} object will generate all items in
		''' this vector. The first item generated is the item at index {@code 0},
		''' then the item at index {@code 1}, and so on.
		''' </summary>
		''' <returns>  an enumeration of the components of this vector </returns>
		''' <seealso cref=     Iterator </seealso>
		Public Overridable Function elements() As Enumeration(Of E)
			Return New EnumerationAnonymousInnerClassHelper(Of E)
		End Function

		Private Class EnumerationAnonymousInnerClassHelper(Of E)
			Implements Enumeration(Of E)

			Friend count As Integer = 0

			Public Overridable Function hasMoreElements() As Boolean Implements Enumeration(Of E).hasMoreElements
				Return count < outerInstance.elementCount
			End Function

			Public Overridable Function nextElement() As E Implements Enumeration(Of E).nextElement
				SyncLock Vector.this
					If count < outerInstance.elementCount Then Return outerInstance.elementData(count++)
				End SyncLock
				Throw New NoSuchElementException("Vector Enumeration")
			End Function
		End Class

		''' <summary>
		''' Returns {@code true} if this vector contains the specified element.
		''' More formally, returns {@code true} if and only if this vector
		''' contains at least one element {@code e} such that
		''' <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>.
		''' </summary>
		''' <param name="o"> element whose presence in this vector is to be tested </param>
		''' <returns> {@code true} if this vector contains the specified element </returns>
		Public Overridable Function contains(ByVal o As Object) As Boolean Implements List(Of E).contains
			Return IndexOf(o, 0) >= 0
		End Function

		''' <summary>
		''' Returns the index of the first occurrence of the specified element
		''' in this vector, or -1 if this vector does not contain the element.
		''' More formally, returns the lowest index {@code i} such that
		''' <tt>(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i)))</tt>,
		''' or -1 if there is no such index.
		''' </summary>
		''' <param name="o"> element to search for </param>
		''' <returns> the index of the first occurrence of the specified element in
		'''         this vector, or -1 if this vector does not contain the element </returns>
		Public Overridable Function indexOf(ByVal o As Object) As Integer Implements List(Of E).indexOf
			Return IndexOf(o, 0)
		End Function

		''' <summary>
		''' Returns the index of the first occurrence of the specified element in
		''' this vector, searching forwards from {@code index}, or returns -1 if
		''' the element is not found.
		''' More formally, returns the lowest index {@code i} such that
		''' <tt>(i&nbsp;&gt;=&nbsp;index&nbsp;&amp;&amp;&nbsp;(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i))))</tt>,
		''' or -1 if there is no such index.
		''' </summary>
		''' <param name="o"> element to search for </param>
		''' <param name="index"> index to start searching from </param>
		''' <returns> the index of the first occurrence of the element in
		'''         this vector at position {@code index} or later in the vector;
		'''         {@code -1} if the element is not found. </returns>
		''' <exception cref="IndexOutOfBoundsException"> if the specified index is negative </exception>
		''' <seealso cref=     Object#equals(Object) </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function indexOf(ByVal o As Object, ByVal index As Integer) As Integer
			If o Is Nothing Then
				For i As Integer = index To elementCount - 1
					If elementData_Renamed(i) Is Nothing Then Return i
				Next i
			Else
				For i As Integer = index To elementCount - 1
					If o.Equals(elementData_Renamed(i)) Then Return i
				Next i
			End If
			Return -1
		End Function

		''' <summary>
		''' Returns the index of the last occurrence of the specified element
		''' in this vector, or -1 if this vector does not contain the element.
		''' More formally, returns the highest index {@code i} such that
		''' <tt>(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i)))</tt>,
		''' or -1 if there is no such index.
		''' </summary>
		''' <param name="o"> element to search for </param>
		''' <returns> the index of the last occurrence of the specified element in
		'''         this vector, or -1 if this vector does not contain the element </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function lastIndexOf(ByVal o As Object) As Integer Implements List(Of E).lastIndexOf
			Return LastIndexOf(o, elementCount-1)
		End Function

		''' <summary>
		''' Returns the index of the last occurrence of the specified element in
		''' this vector, searching backwards from {@code index}, or returns -1 if
		''' the element is not found.
		''' More formally, returns the highest index {@code i} such that
		''' <tt>(i&nbsp;&lt;=&nbsp;index&nbsp;&amp;&amp;&nbsp;(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i))))</tt>,
		''' or -1 if there is no such index.
		''' </summary>
		''' <param name="o"> element to search for </param>
		''' <param name="index"> index to start searching backwards from </param>
		''' <returns> the index of the last occurrence of the element at position
		'''         less than or equal to {@code index} in this vector;
		'''         -1 if the element is not found. </returns>
		''' <exception cref="IndexOutOfBoundsException"> if the specified index is greater
		'''         than or equal to the current size of this vector </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function lastIndexOf(ByVal o As Object, ByVal index As Integer) As Integer
			If index >= elementCount Then Throw New IndexOutOfBoundsException(index & " >= " & elementCount)

			If o Is Nothing Then
				For i As Integer = index To 0 Step -1
					If elementData_Renamed(i) Is Nothing Then Return i
				Next i
			Else
				For i As Integer = index To 0 Step -1
					If o.Equals(elementData_Renamed(i)) Then Return i
				Next i
			End If
			Return -1
		End Function

		''' <summary>
		''' Returns the component at the specified index.
		''' 
		''' <p>This method is identical in functionality to the <seealso cref="#get(int)"/>
		''' method (which is part of the <seealso cref="List"/> interface).
		''' </summary>
		''' <param name="index">   an index into this vector </param>
		''' <returns>     the component at the specified index </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the index is out of range
		'''         ({@code index < 0 || index >= size()}) </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function elementAt(ByVal index As Integer) As E
			If index >= elementCount Then Throw New ArrayIndexOutOfBoundsException(index & " >= " & elementCount)

			Return elementData(index)
		End Function

		''' <summary>
		''' Returns the first component (the item at index {@code 0}) of
		''' this vector.
		''' </summary>
		''' <returns>     the first component of this vector </returns>
		''' <exception cref="NoSuchElementException"> if this vector has no components </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function firstElement() As E
			If elementCount = 0 Then Throw New NoSuchElementException
			Return elementData(0)
		End Function

		''' <summary>
		''' Returns the last component of the vector.
		''' </summary>
		''' <returns>  the last component of the vector, i.e., the component at index
		'''          <code>size()&nbsp;-&nbsp;1</code>. </returns>
		''' <exception cref="NoSuchElementException"> if this vector is empty </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function lastElement() As E
			If elementCount = 0 Then Throw New NoSuchElementException
			Return elementData(elementCount - 1)
		End Function

		''' <summary>
		''' Sets the component at the specified {@code index} of this
		''' vector to be the specified object. The previous component at that
		''' position is discarded.
		''' 
		''' <p>The index must be a value greater than or equal to {@code 0}
		''' and less than the current size of the vector.
		''' 
		''' <p>This method is identical in functionality to the
		''' <seealso cref="#set(int, Object) set(int, E)"/>
		''' method (which is part of the <seealso cref="List"/> interface). Note that the
		''' {@code set} method reverses the order of the parameters, to more closely
		''' match array usage.  Note also that the {@code set} method returns the
		''' old value that was stored at the specified position.
		''' </summary>
		''' <param name="obj">     what the component is to be set to </param>
		''' <param name="index">   the specified index </param>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the index is out of range
		'''         ({@code index < 0 || index >= size()}) </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub setElementAt(ByVal obj As E, ByVal index As Integer)
			If index >= elementCount Then Throw New ArrayIndexOutOfBoundsException(index & " >= " & elementCount)
			elementData_Renamed(index) = obj
		End Sub

		''' <summary>
		''' Deletes the component at the specified index. Each component in
		''' this vector with an index greater or equal to the specified
		''' {@code index} is shifted downward to have an index one
		''' smaller than the value it had previously. The size of this vector
		''' is decreased by {@code 1}.
		''' 
		''' <p>The index must be a value greater than or equal to {@code 0}
		''' and less than the current size of the vector.
		''' 
		''' <p>This method is identical in functionality to the <seealso cref="#remove(int)"/>
		''' method (which is part of the <seealso cref="List"/> interface).  Note that the
		''' {@code remove} method returns the old value that was stored at the
		''' specified position.
		''' </summary>
		''' <param name="index">   the index of the object to remove </param>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the index is out of range
		'''         ({@code index < 0 || index >= size()}) </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeElementAt(ByVal index As Integer)
			modCount += 1
			If index >= elementCount Then
				Throw New ArrayIndexOutOfBoundsException(index & " >= " & elementCount)
			ElseIf index < 0 Then
				Throw New ArrayIndexOutOfBoundsException(index)
			End If
			Dim j As Integer = elementCount - index - 1
			If j > 0 Then Array.Copy(elementData_Renamed, index + 1, elementData_Renamed, index, j)
			elementCount -= 1
			elementData_Renamed(elementCount) = Nothing ' to let gc do its work
		End Sub

		''' <summary>
		''' Inserts the specified object as a component in this vector at the
		''' specified {@code index}. Each component in this vector with
		''' an index greater or equal to the specified {@code index} is
		''' shifted upward to have an index one greater than the value it had
		''' previously.
		''' 
		''' <p>The index must be a value greater than or equal to {@code 0}
		''' and less than or equal to the current size of the vector. (If the
		''' index is equal to the current size of the vector, the new element
		''' is appended to the Vector.)
		''' 
		''' <p>This method is identical in functionality to the
		''' <seealso cref="#add(int, Object) add(int, E)"/>
		''' method (which is part of the <seealso cref="List"/> interface).  Note that the
		''' {@code add} method reverses the order of the parameters, to more closely
		''' match array usage.
		''' </summary>
		''' <param name="obj">     the component to insert </param>
		''' <param name="index">   where to insert the new component </param>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the index is out of range
		'''         ({@code index < 0 || index > size()}) </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub insertElementAt(ByVal obj As E, ByVal index As Integer)
			modCount += 1
			If index > elementCount Then Throw New ArrayIndexOutOfBoundsException(index & " > " & elementCount)
			ensureCapacityHelper(elementCount + 1)
			Array.Copy(elementData_Renamed, index, elementData_Renamed, index + 1, elementCount - index)
			elementData_Renamed(index) = obj
			elementCount += 1
		End Sub

		''' <summary>
		''' Adds the specified component to the end of this vector,
		''' increasing its size by one. The capacity of this vector is
		''' increased if its size becomes greater than its capacity.
		''' 
		''' <p>This method is identical in functionality to the
		''' <seealso cref="#add(Object) add(E)"/>
		''' method (which is part of the <seealso cref="List"/> interface).
		''' </summary>
		''' <param name="obj">   the component to be added </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addElement(ByVal obj As E)
			modCount += 1
			ensureCapacityHelper(elementCount + 1)
			elementData_Renamed(elementCount) = obj
			elementCount += 1
		End Sub

		''' <summary>
		''' Removes the first (lowest-indexed) occurrence of the argument
		''' from this vector. If the object is found in this vector, each
		''' component in the vector with an index greater or equal to the
		''' object's index is shifted downward to have an index one smaller
		''' than the value it had previously.
		''' 
		''' <p>This method is identical in functionality to the
		''' <seealso cref="#remove(Object)"/> method (which is part of the
		''' <seealso cref="List"/> interface).
		''' </summary>
		''' <param name="obj">   the component to be removed </param>
		''' <returns>  {@code true} if the argument was a component of this
		'''          vector; {@code false} otherwise. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function removeElement(ByVal obj As Object) As Boolean
			modCount += 1
			Dim i As Integer = IndexOf(obj)
			If i >= 0 Then
				removeElementAt(i)
				Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Removes all components from this vector and sets its size to zero.
		''' 
		''' <p>This method is identical in functionality to the <seealso cref="#clear"/>
		''' method (which is part of the <seealso cref="List"/> interface).
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeAllElements()
			modCount += 1
			' Let gc do its work
			Dim i As Integer = 0
			Do While i < elementCount
				elementData_Renamed(i) = Nothing
				i += 1
			Loop

			elementCount = 0
		End Sub

		''' <summary>
		''' Returns a clone of this vector. The copy will contain a
		''' reference to a clone of the internal data array, not a reference
		''' to the original internal data array of this {@code Vector} object.
		''' </summary>
		''' <returns>  a clone of this vector </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function clone() As Object
			Try
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim v As Vector(Of E) = CType(MyBase.clone(), Vector(Of E))
				v.elementData = New java.lang.Object(elementCount - 1){}
				Array.Copy(elementData_Renamed, v.elementData_Renamed, elementCount)
				v.modCount = 0
				Return v
			Catch e As CloneNotSupportedException
				' this shouldn't happen, since we are Cloneable
				Throw New InternalError(e)
			End Try
		End Function

		''' <summary>
		''' Returns an array containing all of the elements in this Vector
		''' in the correct order.
		''' 
		''' @since 1.2
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function toArray() As Object() Implements List(Of E).toArray
			Return Arrays.copyOf(elementData_Renamed, elementCount)
		End Function

		''' <summary>
		''' Returns an array containing all of the elements in this Vector in the
		''' correct order; the runtime type of the returned array is that of the
		''' specified array.  If the Vector fits in the specified array, it is
		''' returned therein.  Otherwise, a new array is allocated with the runtime
		''' type of the specified array and the size of this Vector.
		''' 
		''' <p>If the Vector fits in the specified array with room to spare
		''' (i.e., the array has more elements than the Vector),
		''' the element in the array immediately following the end of the
		''' Vector is set to null.  (This is useful in determining the length
		''' of the Vector <em>only</em> if the caller knows that the Vector
		''' does not contain any null elements.)
		''' </summary>
		''' <param name="a"> the array into which the elements of the Vector are to
		'''          be stored, if it is big enough; otherwise, a new array of the
		'''          same runtime type is allocated for this purpose. </param>
		''' <returns> an array containing the elements of the Vector </returns>
		''' <exception cref="ArrayStoreException"> if the runtime type of a is not a supertype
		''' of the runtime type of every element in this Vector </exception>
		''' <exception cref="NullPointerException"> if the given array is null
		''' @since 1.2 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function toArray(Of T)(ByVal a As T()) As T() Implements List(Of E).toArray
			If a.Length < elementCount Then Return CType(Arrays.copyOf(elementData_Renamed, elementCount, a.GetType()), T())

			Array.Copy(elementData_Renamed, 0, a, 0, elementCount)

			If a.Length > elementCount Then a(elementCount) = Nothing

			Return a
		End Function

		' Positional Access Operations

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Overridable Function elementData(ByVal index As Integer) As E
			Return CType(elementData_Renamed(index), E)
		End Function

		''' <summary>
		''' Returns the element at the specified position in this Vector.
		''' </summary>
		''' <param name="index"> index of the element to return </param>
		''' <returns> object at the specified index </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the index is out of range
		'''            ({@code index < 0 || index >= size()})
		''' @since 1.2 </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function [get](ByVal index As Integer) As E Implements List(Of E).get
			If index >= elementCount Then Throw New ArrayIndexOutOfBoundsException(index)

			Return elementData(index)
		End Function

		''' <summary>
		''' Replaces the element at the specified position in this Vector with the
		''' specified element.
		''' </summary>
		''' <param name="index"> index of the element to replace </param>
		''' <param name="element"> element to be stored at the specified position </param>
		''' <returns> the element previously at the specified position </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the index is out of range
		'''         ({@code index < 0 || index >= size()})
		''' @since 1.2 </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function [set](ByVal index As Integer, ByVal element As E) As E
			If index >= elementCount Then Throw New ArrayIndexOutOfBoundsException(index)

			Dim oldValue As E = elementData(index)
			elementData_Renamed(index) = element
			Return oldValue
		End Function

		''' <summary>
		''' Appends the specified element to the end of this Vector.
		''' </summary>
		''' <param name="e"> element to be appended to this Vector </param>
		''' <returns> {@code true} (as specified by <seealso cref="Collection#add"/>)
		''' @since 1.2 </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function add(ByVal e As E) As Boolean
			modCount += 1
			ensureCapacityHelper(elementCount + 1)
			elementData_Renamed(elementCount) = e
			elementCount += 1
			Return True
		End Function

		''' <summary>
		''' Removes the first occurrence of the specified element in this Vector
		''' If the Vector does not contain the element, it is unchanged.  More
		''' formally, removes the element with the lowest index i such that
		''' {@code (o==null ? get(i)==null : o.equals(get(i)))} (if such
		''' an element exists).
		''' </summary>
		''' <param name="o"> element to be removed from this Vector, if present </param>
		''' <returns> true if the Vector contained the specified element
		''' @since 1.2 </returns>
		Public Overridable Function remove(ByVal o As Object) As Boolean Implements List(Of E).remove
			Return removeElement(o)
		End Function

		''' <summary>
		''' Inserts the specified element at the specified position in this Vector.
		''' Shifts the element currently at that position (if any) and any
		''' subsequent elements to the right (adds one to their indices).
		''' </summary>
		''' <param name="index"> index at which the specified element is to be inserted </param>
		''' <param name="element"> element to be inserted </param>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the index is out of range
		'''         ({@code index < 0 || index > size()})
		''' @since 1.2 </exception>
		Public Overridable Sub add(ByVal index As Integer, ByVal element As E)
			insertElementAt(element, index)
		End Sub

		''' <summary>
		''' Removes the element at the specified position in this Vector.
		''' Shifts any subsequent elements to the left (subtracts one from their
		''' indices).  Returns the element that was removed from the Vector.
		''' </summary>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the index is out of range
		'''         ({@code index < 0 || index >= size()}) </exception>
		''' <param name="index"> the index of the element to be removed </param>
		''' <returns> element that was removed
		''' @since 1.2 </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function remove(ByVal index As Integer) As E Implements List(Of E).remove
			modCount += 1
			If index >= elementCount Then Throw New ArrayIndexOutOfBoundsException(index)
			Dim oldValue As E = elementData(index)

			Dim numMoved As Integer = elementCount - index - 1
			If numMoved > 0 Then Array.Copy(elementData_Renamed, index+1, elementData_Renamed, index, numMoved)
			elementCount -= 1
			elementData_Renamed(elementCount) = Nothing

			Return oldValue
		End Function

		''' <summary>
		''' Removes all of the elements from this Vector.  The Vector will
		''' be empty after this call returns (unless it throws an exception).
		''' 
		''' @since 1.2
		''' </summary>
		Public Overridable Sub clear() Implements List(Of E).clear
			removeAllElements()
		End Sub

		' Bulk Operations

		''' <summary>
		''' Returns true if this Vector contains all of the elements in the
		''' specified Collection.
		''' </summary>
		''' <param name="c"> a collection whose elements will be tested for containment
		'''          in this Vector </param>
		''' <returns> true if this Vector contains all of the elements in the
		'''         specified collection </returns>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function containsAll(Of T1)(ByVal c As Collection(Of T1)) As Boolean Implements List(Of E).containsAll
			Return MyBase.containsAll(c)
		End Function

		''' <summary>
		''' Appends all of the elements in the specified Collection to the end of
		''' this Vector, in the order that they are returned by the specified
		''' Collection's Iterator.  The behavior of this operation is undefined if
		''' the specified Collection is modified while the operation is in progress.
		''' (This implies that the behavior of this call is undefined if the
		''' specified Collection is this Vector, and this Vector is nonempty.)
		''' </summary>
		''' <param name="c"> elements to be inserted into this Vector </param>
		''' <returns> {@code true} if this Vector changed as a result of the call </returns>
		''' <exception cref="NullPointerException"> if the specified collection is null
		''' @since 1.2 </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function addAll(Of T1 As E)(ByVal c As Collection(Of T1)) As Boolean Implements List(Of E).addAll
			modCount += 1
			Dim a As Object() = c.ToArray()
			Dim numNew As Integer = a.Length
			ensureCapacityHelper(elementCount + numNew)
			Array.Copy(a, 0, elementData_Renamed, elementCount, numNew)
			elementCount += numNew
			Return numNew <> 0
		End Function

		''' <summary>
		''' Removes from this Vector all of its elements that are contained in the
		''' specified Collection.
		''' </summary>
		''' <param name="c"> a collection of elements to be removed from the Vector </param>
		''' <returns> true if this Vector changed as a result of the call </returns>
		''' <exception cref="ClassCastException"> if the types of one or more elements
		'''         in this vector are incompatible with the specified
		'''         collection
		''' (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		''' <exception cref="NullPointerException"> if this vector contains one or more null
		'''         elements and the specified collection does not support null
		'''         elements
		''' (<a href="Collection.html#optional-restrictions">optional</a>),
		'''         or if the specified collection is null
		''' @since 1.2 </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function removeAll(Of T1)(ByVal c As Collection(Of T1)) As Boolean Implements List(Of E).removeAll
			Return MyBase.removeAll(c)
		End Function

		''' <summary>
		''' Retains only the elements in this Vector that are contained in the
		''' specified Collection.  In other words, removes from this Vector all
		''' of its elements that are not contained in the specified Collection.
		''' </summary>
		''' <param name="c"> a collection of elements to be retained in this Vector
		'''          (all other elements are removed) </param>
		''' <returns> true if this Vector changed as a result of the call </returns>
		''' <exception cref="ClassCastException"> if the types of one or more elements
		'''         in this vector are incompatible with the specified
		'''         collection
		''' (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		''' <exception cref="NullPointerException"> if this vector contains one or more null
		'''         elements and the specified collection does not support null
		'''         elements
		'''         (<a href="Collection.html#optional-restrictions">optional</a>),
		'''         or if the specified collection is null
		''' @since 1.2 </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function retainAll(Of T1)(ByVal c As Collection(Of T1)) As Boolean Implements List(Of E).retainAll
			Return MyBase.retainAll(c)
		End Function

		''' <summary>
		''' Inserts all of the elements in the specified Collection into this
		''' Vector at the specified position.  Shifts the element currently at
		''' that position (if any) and any subsequent elements to the right
		''' (increases their indices).  The new elements will appear in the Vector
		''' in the order that they are returned by the specified Collection's
		''' iterator.
		''' </summary>
		''' <param name="index"> index at which to insert the first element from the
		'''              specified collection </param>
		''' <param name="c"> elements to be inserted into this Vector </param>
		''' <returns> {@code true} if this Vector changed as a result of the call </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the index is out of range
		'''         ({@code index < 0 || index > size()}) </exception>
		''' <exception cref="NullPointerException"> if the specified collection is null
		''' @since 1.2 </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function addAll(Of T1 As E)(ByVal index As Integer, ByVal c As Collection(Of T1)) As Boolean Implements List(Of E).addAll
			modCount += 1
			If index < 0 OrElse index > elementCount Then Throw New ArrayIndexOutOfBoundsException(index)

			Dim a As Object() = c.ToArray()
			Dim numNew As Integer = a.Length
			ensureCapacityHelper(elementCount + numNew)

			Dim numMoved As Integer = elementCount - index
			If numMoved > 0 Then Array.Copy(elementData_Renamed, index, elementData_Renamed, index + numNew, numMoved)

			Array.Copy(a, 0, elementData_Renamed, index, numNew)
			elementCount += numNew
			Return numNew <> 0
		End Function

		''' <summary>
		''' Compares the specified Object with this Vector for equality.  Returns
		''' true if and only if the specified Object is also a List, both Lists
		''' have the same size, and all corresponding pairs of elements in the two
		''' Lists are <em>equal</em>.  (Two elements {@code e1} and
		''' {@code e2} are <em>equal</em> if {@code (e1==null ? e2==null :
		''' e1.equals(e2))}.)  In other words, two Lists are defined to be
		''' equal if they contain the same elements in the same order.
		''' </summary>
		''' <param name="o"> the Object to be compared for equality with this Vector </param>
		''' <returns> true if the specified Object is equal to this Vector </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			Return MyBase.Equals(o)
		End Function

		''' <summary>
		''' Returns the hash code value for this Vector.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function GetHashCode() As Integer
			Return MyBase.GetHashCode()
		End Function

		''' <summary>
		''' Returns a string representation of this Vector, containing
		''' the String representation of each element.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function ToString() As String
			Return MyBase.ToString()
		End Function

		''' <summary>
		''' Returns a view of the portion of this List between fromIndex,
		''' inclusive, and toIndex, exclusive.  (If fromIndex and toIndex are
		''' equal, the returned List is empty.)  The returned List is backed by this
		''' List, so changes in the returned List are reflected in this List, and
		''' vice-versa.  The returned List supports all of the optional List
		''' operations supported by this List.
		''' 
		''' <p>This method eliminates the need for explicit range operations (of
		''' the sort that commonly exist for arrays).  Any operation that expects
		''' a List can be used as a range operation by operating on a subList view
		''' instead of a whole List.  For example, the following idiom
		''' removes a range of elements from a List:
		''' <pre>
		'''      list.subList(from, to).clear();
		''' </pre>
		''' Similar idioms may be constructed for indexOf and lastIndexOf,
		''' and all of the algorithms in the Collections class can be applied to
		''' a subList.
		''' 
		''' <p>The semantics of the List returned by this method become undefined if
		''' the backing list (i.e., this List) is <i>structurally modified</i> in
		''' any way other than via the returned List.  (Structural modifications are
		''' those that change the size of the List, or otherwise perturb it in such
		''' a fashion that iterations in progress may yield incorrect results.)
		''' </summary>
		''' <param name="fromIndex"> low endpoint (inclusive) of the subList </param>
		''' <param name="toIndex"> high endpoint (exclusive) of the subList </param>
		''' <returns> a view of the specified range within this List </returns>
		''' <exception cref="IndexOutOfBoundsException"> if an endpoint index value is out of range
		'''         {@code (fromIndex < 0 || toIndex > size)} </exception>
		''' <exception cref="IllegalArgumentException"> if the endpoint indices are out of order
		'''         {@code (fromIndex > toIndex)} </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function subList(ByVal fromIndex As Integer, ByVal toIndex As Integer) As List(Of E) Implements List(Of E).subList
			Return Collections.synchronizedList(MyBase.subList(fromIndex, toIndex), Me)
		End Function

		''' <summary>
		''' Removes from this list all of the elements whose index is between
		''' {@code fromIndex}, inclusive, and {@code toIndex}, exclusive.
		''' Shifts any succeeding elements to the left (reduces their index).
		''' This call shortens the list by {@code (toIndex - fromIndex)} elements.
		''' (If {@code toIndex==fromIndex}, this operation has no effect.)
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overridable Sub removeRange(ByVal fromIndex As Integer, ByVal toIndex As Integer)
			modCount += 1
			Dim numMoved As Integer = elementCount - toIndex
			Array.Copy(elementData_Renamed, toIndex, elementData_Renamed, fromIndex, numMoved)

			' Let gc do its work
			Dim newElementCount As Integer = elementCount - (toIndex-fromIndex)
			Do While elementCount <> newElementCount
				elementCount -= 1
				elementData_Renamed(elementCount) = Nothing
			Loop
		End Sub

		''' <summary>
		''' Save the state of the {@code Vector} instance to a stream (that
		''' is, serialize it).
		''' This method performs synchronization to ensure the consistency
		''' of the serialized data.
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			Dim fields As java.io.ObjectOutputStream.PutField = s.putFields()
			Dim data As Object()
			SyncLock Me
				fields.put("capacityIncrement", capacityIncrement)
				fields.put("elementCount", elementCount)
				data = elementData_Renamed.clone()
			End SyncLock
			fields.put("elementData", data)
			s.writeFields()
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
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function listIterator(ByVal index As Integer) As ListIterator(Of E) Implements List(Of E).listIterator
			If index < 0 OrElse index > elementCount Then Throw New IndexOutOfBoundsException("Index: " & index)
			Return New ListItr(Me, index)
		End Function

		''' <summary>
		''' Returns a list iterator over the elements in this list (in proper
		''' sequence).
		''' 
		''' <p>The returned list iterator is <a href="#fail-fast"><i>fail-fast</i></a>.
		''' </summary>
		''' <seealso cref= #listIterator(int) </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function listIterator() As ListIterator(Of E) Implements List(Of E).listIterator
			Return New ListItr(Me, 0)
		End Function

		''' <summary>
		''' Returns an iterator over the elements in this list in proper sequence.
		''' 
		''' <p>The returned iterator is <a href="#fail-fast"><i>fail-fast</i></a>.
		''' </summary>
		''' <returns> an iterator over the elements in this list in proper sequence </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function [iterator]() As [Iterator](Of E) Implements List(Of E).iterator
			Return New Itr(Me)
		End Function

		''' <summary>
		''' An optimized version of AbstractList.Itr
		''' </summary>
		Private Class Itr
			Implements Iterator(Of E)

			Private ReadOnly outerInstance As Vector

			Public Sub New(ByVal outerInstance As Vector)
				Me.outerInstance = outerInstance
			End Sub

			Friend cursor As Integer ' index of next element to return
			Friend lastRet As Integer = -1 ' index of last element returned; -1 if no such
			Friend expectedModCount As Integer = outerInstance.modCount

			Public Overridable Function hasNext() As Boolean Implements Iterator(Of E).hasNext
				' Racy but within spec, since modifications are checked
				' within or after synchronization in next/previous
				Return cursor <> outerInstance.elementCount
			End Function

			Public Overridable Function [next]() As E Implements Iterator(Of E).next
				SyncLock Vector.this
					checkForComodification()
					Dim i As Integer = cursor
					If i >= outerInstance.elementCount Then Throw New NoSuchElementException
					cursor = i + 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return outerInstance.elementData(lastRet = i)
				End SyncLock
			End Function

			Public Overridable Sub remove() Implements Iterator(Of E).remove
				If lastRet = -1 Then Throw New IllegalStateException
				SyncLock Vector.this
					checkForComodification()
					outerInstance.remove(lastRet)
					expectedModCount = outerInstance.modCount
				End SyncLock
				cursor = lastRet
				lastRet = -1
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Iterator(Of E).forEachRemaining
				Objects.requireNonNull(action)
				SyncLock Vector.this
					Dim size As Integer = outerInstance.elementCount
					Dim i As Integer = cursor
					If i >= size Then Return
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim elementData As E() = CType(Vector.this.elementData, E())
					If i >= elementData.Length Then Throw New ConcurrentModificationException
					Do While i <> size AndAlso outerInstance.modCount = expectedModCount
						action.accept(elementData(i))
						i += 1
					Loop
					' update once at end of iteration to reduce heap write traffic
					cursor = i
					lastRet = i - 1
					checkForComodification()
				End SyncLock
			End Sub

			Friend Sub checkForComodification()
				If outerInstance.modCount <> expectedModCount Then Throw New ConcurrentModificationException
			End Sub
		End Class

		''' <summary>
		''' An optimized version of AbstractList.ListItr
		''' </summary>
		Friend NotInheritable Class ListItr
			Inherits Itr
			Implements ListIterator(Of E)

			Private ReadOnly outerInstance As Vector

			Friend Sub New(ByVal outerInstance As Vector, ByVal index As Integer)
					Me.outerInstance = outerInstance
				MyBase.New()
				cursor = index
			End Sub

			Public Function hasPrevious() As Boolean Implements ListIterator(Of E).hasPrevious
				Return cursor <> 0
			End Function

			Public Function nextIndex() As Integer Implements ListIterator(Of E).nextIndex
				Return cursor
			End Function

			Public Function previousIndex() As Integer Implements ListIterator(Of E).previousIndex
				Return cursor - 1
			End Function

			Public Function previous() As E Implements ListIterator(Of E).previous
				SyncLock Vector.this
					checkForComodification()
					Dim i As Integer = cursor - 1
					If i < 0 Then Throw New NoSuchElementException
					cursor = i
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return outerInstance.elementData(lastRet = i)
				End SyncLock
			End Function

			Public Sub [set](ByVal e As E)
				If lastRet = -1 Then Throw New IllegalStateException
				SyncLock Vector.this
					checkForComodification()
					outerInstance.set(lastRet, e)
				End SyncLock
			End Sub

			Public Sub add(ByVal e As E)
				Dim i As Integer = cursor
				SyncLock Vector.this
					checkForComodification()
					outerInstance.add(i, e)
					expectedModCount = outerInstance.modCount
				End SyncLock
				cursor = i + 1
				lastRet = -1
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
			Objects.requireNonNull(action)
			Dim expectedModCount As Integer = modCount
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim elementData As E() = CType(Me.elementData_Renamed, E())
			Dim elementCount As Integer = Me.elementCount
			Dim i As Integer=0
			Do While modCount = expectedModCount AndAlso i < elementCount
				action.accept(elementData(i))
				i += 1
			Loop
			If modCount <> expectedModCount Then Throw New ConcurrentModificationException
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function removeIf(Of T1)(ByVal filter As java.util.function.Predicate(Of T1)) As Boolean
			Objects.requireNonNull(filter)
			' figure out which elements are to be removed
			' any exception thrown from the filter predicate at this stage
			' will leave the collection unmodified
			Dim removeCount As Integer = 0
			Dim size_Renamed As Integer = elementCount
			Dim removeSet As New BitSet(size_Renamed)
			Dim expectedModCount As Integer = modCount
			Dim i As Integer=0
			Do While modCount = expectedModCount AndAlso i < size_Renamed
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
				Dim newSize As Integer = size_Renamed - removeCount
				i = 0
				Dim j As Integer=0
				Do While (i < size_Renamed) AndAlso (j < newSize)
					i = removeSet.nextClearBit(i)
					elementData_Renamed(j) = elementData_Renamed(i)
					i += 1
					j += 1
				Loop
				For k As Integer = newSize To size_Renamed - 1
					elementData_Renamed(k) = Nothing ' Let gc do its work
				Next k
				elementCount = newSize
				If modCount <> expectedModCount Then Throw New ConcurrentModificationException
				modCount += 1
			End If

			Return anyToRemove
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub replaceAll(ByVal [operator] As java.util.function.UnaryOperator(Of E)) Implements List(Of E).replaceAll
			Objects.requireNonNull([operator])
			Dim expectedModCount As Integer = modCount
			Dim size_Renamed As Integer = elementCount
			Dim i As Integer=0
			Do While modCount = expectedModCount AndAlso i < size_Renamed
				elementData_Renamed(i) = [operator].apply(CType(elementData_Renamed(i), E))
				i += 1
			Loop
			If modCount <> expectedModCount Then Throw New ConcurrentModificationException
			modCount += 1
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub sort(Of T1)(ByVal c As Comparator(Of T1)) Implements List(Of E).sort
			Dim expectedModCount As Integer = modCount
			Array.Sort(CType(elementData_Renamed, E()), 0, elementCount, c)
			If modCount <> expectedModCount Then Throw New ConcurrentModificationException
			modCount += 1
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
			Return New VectorSpliterator(Of )(Me, Nothing, 0, -1, 0)
		End Function

		''' <summary>
		''' Similar to ArrayList Spliterator </summary>
		Friend NotInheritable Class VectorSpliterator(Of E)
			Implements Spliterator(Of E)

			Private ReadOnly list As Vector(Of E)
			Private array As Object()
			Private index As Integer ' current index, modified on advance/split
			Private fence As Integer ' -1 until used; then one past last index
			Private expectedModCount As Integer ' initialized when fence set

			''' <summary>
			''' Create new spliterator covering the given  range </summary>
			Friend Sub New(ByVal list As Vector(Of E), ByVal array As Object(), ByVal origin As Integer, ByVal fence As Integer, ByVal expectedModCount As Integer)
				Me.list = list
				Me.array = array
				Me.index = origin
				Me.fence = fence
				Me.expectedModCount = expectedModCount
			End Sub

			Private Property fence As Integer
				Get
					Dim hi As Integer
					hi = fence
					If hi < 0 Then
						SyncLock list
							array = list.elementData_Renamed
							expectedModCount = list.modCount
								fence = list.elementCount
								hi = fence
						End SyncLock
					End If
					Return hi
				End Get
			End Property

			Public Function trySplit() As Spliterator(Of E) Implements Spliterator(Of E).trySplit
				Dim hi As Integer = fence, lo As Integer = index, mid As Integer = CInt(CUInt((lo + hi)) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If(lo >= mid, Nothing, New VectorSpliterator(Of E)(list, array, lo, index = mid, expectedModCount))
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of E).tryAdvance
				Dim i As Integer
				If action Is Nothing Then Throw New NullPointerException
				i = index
				If fence > i Then
					index = i + 1
					action.accept(CType(array(i), E))
					If list.modCount <> expectedModCount Then Throw New ConcurrentModificationException
					Return True
				End If
				Return False
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Spliterator(Of E).forEachRemaining
				Dim i, hi As Integer ' hoist accesses and checks from loop
				Dim lst As Vector(Of E)
				Dim a As Object()
				If action Is Nothing Then Throw New NullPointerException
				lst = list
				If lst IsNot Nothing Then
					hi = fence
					If hi < 0 Then
						SyncLock lst
							expectedModCount = lst.modCount
								array = lst.elementData_Renamed
								a = array
								fence = lst.elementCount
								hi = fence
						End SyncLock
					Else
						a = array
					End If
					i = index
					index = hi
					If a IsNot Nothing AndAlso i >= 0 AndAlso index <= a.Length Then
						Do While i < hi
							action.accept(CType(a(i), E))
							i += 1
						Loop
						If lst.modCount = expectedModCount Then Return
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
	End Class

End Namespace
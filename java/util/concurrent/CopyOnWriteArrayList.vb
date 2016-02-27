Imports System
Imports System.Collections
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
' * Written by Doug Lea with assistance from members of JCP JSR-166
' * Expert Group.  Adapted and released, under explicit permission,
' * from JDK ArrayList.java which carries the following copyright:
' *
' * Copyright 1997 by Sun Microsystems, Inc.,
' * 901 San Antonio Road, Palo Alto, California, 94303, U.S.A.
' * All rights reserved.
' 

Namespace java.util.concurrent

	''' <summary>
	''' A thread-safe variant of <seealso cref="java.util.ArrayList"/> in which all mutative
	''' operations ({@code add}, {@code set}, and so on) are implemented by
	''' making a fresh copy of the underlying array.
	''' 
	''' <p>This is ordinarily too costly, but may be <em>more</em> efficient
	''' than alternatives when traversal operations vastly outnumber
	''' mutations, and is useful when you cannot or don't want to
	''' synchronize traversals, yet need to preclude interference among
	''' concurrent threads.  The "snapshot" style iterator method uses a
	''' reference to the state of the array at the point that the iterator
	''' was created. This array never changes during the lifetime of the
	''' iterator, so interference is impossible and the iterator is
	''' guaranteed not to throw {@code ConcurrentModificationException}.
	''' The iterator will not reflect additions, removals, or changes to
	''' the list since the iterator was created.  Element-changing
	''' operations on iterators themselves ({@code remove}, {@code set}, and
	''' {@code add}) are not supported. These methods throw
	''' {@code UnsupportedOperationException}.
	''' 
	''' <p>All elements are permitted, including {@code null}.
	''' 
	''' <p>Memory consistency effects: As with other concurrent
	''' collections, actions in a thread prior to placing an object into a
	''' {@code CopyOnWriteArrayList}
	''' <a href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	''' actions subsequent to the access or removal of that element from
	''' the {@code CopyOnWriteArrayList} in another thread.
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @since 1.5
	''' @author Doug Lea </summary>
	''' @param <E> the type of elements held in this collection </param>
	<Serializable> _
	Public Class CopyOnWriteArrayList(Of E)
		Implements IList(Of E), java.util.RandomAccess, Cloneable

		Private Const serialVersionUID As Long = 8673264195747942595L

		''' <summary>
		''' The lock protecting all mutators </summary>
		<NonSerialized> _
		Friend ReadOnly lock As New java.util.concurrent.locks.ReentrantLock

		''' <summary>
		''' The array, accessed only via getArray/setArray. </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private array As Object()

		''' <summary>
		''' Gets the array.  Non-private so as to also be accessible
		''' from CopyOnWriteArraySet class.
		''' </summary>
		Friend Property array As Object()
			Get
				Return array
			End Get
			Set(ByVal a As Object())
				array = a
			End Set
		End Property


		''' <summary>
		''' Creates an empty list.
		''' </summary>
		Public Sub New()
			array = New Object(){}
		End Sub

		''' <summary>
		''' Creates a list containing the elements of the specified
		''' collection, in the order they are returned by the collection's
		''' iterator.
		''' </summary>
		''' <param name="c"> the collection of initially held elements </param>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
		Public Sub New(Of T1 As E)(ByVal c As ICollection(Of T1))
			Dim elements As Object()
			If c.GetType() Is GetType(CopyOnWriteArrayList) Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				elements = CType(c, CopyOnWriteArrayList(Of ?)).array
			Else
				elements = c.ToArray()
				' c.toArray might (incorrectly) not return Object[] (see 6260652)
				If elements.GetType() IsNot GetType(Object()) Then elements = java.util.Arrays.copyOf(elements, elements.Length, GetType(Object()))
			End If
			array = elements
		End Sub

		''' <summary>
		''' Creates a list holding a copy of the given array.
		''' </summary>
		''' <param name="toCopyIn"> the array (a copy of this array is used as the
		'''        internal array) </param>
		''' <exception cref="NullPointerException"> if the specified array is null </exception>
		Public Sub New(ByVal toCopyIn As E())
			array = java.util.Arrays.copyOf(toCopyIn, toCopyIn.Length, GetType(Object()))
		End Sub

		''' <summary>
		''' Returns the number of elements in this list.
		''' </summary>
		''' <returns> the number of elements in this list </returns>
		Public Overridable Function size() As Integer
			Return array.Length
		End Function

		''' <summary>
		''' Returns {@code true} if this list contains no elements.
		''' </summary>
		''' <returns> {@code true} if this list contains no elements </returns>
		Public Overridable Property empty As Boolean
			Get
				Return size() = 0
			End Get
		End Property

		''' <summary>
		''' Tests for equality, coping with nulls.
		''' </summary>
		Private Shared Function eq(ByVal o1 As Object, ByVal o2 As Object) As Boolean
			Return If(o1 Is Nothing, o2 Is Nothing, o1.Equals(o2))
		End Function

		''' <summary>
		''' static version of indexOf, to allow repeated calls without
		''' needing to re-acquire array each time. </summary>
		''' <param name="o"> element to search for </param>
		''' <param name="elements"> the array </param>
		''' <param name="index"> first index to search </param>
		''' <param name="fence"> one past last index to search </param>
		''' <returns> index of element, or -1 if absent </returns>
		Private Shared Function indexOf(ByVal o As Object, ByVal elements As Object(), ByVal index As Integer, ByVal fence As Integer) As Integer
			If o Is Nothing Then
				For i As Integer = index To fence - 1
					If elements(i) Is Nothing Then Return i
				Next i
			Else
				For i As Integer = index To fence - 1
					If o.Equals(elements(i)) Then Return i
				Next i
			End If
			Return -1
		End Function

		''' <summary>
		''' static version of lastIndexOf. </summary>
		''' <param name="o"> element to search for </param>
		''' <param name="elements"> the array </param>
		''' <param name="index"> first index to search </param>
		''' <returns> index of element, or -1 if absent </returns>
		Private Shared Function lastIndexOf(ByVal o As Object, ByVal elements As Object(), ByVal index As Integer) As Integer
			If o Is Nothing Then
				For i As Integer = index To 0 Step -1
					If elements(i) Is Nothing Then Return i
				Next i
			Else
				For i As Integer = index To 0 Step -1
					If o.Equals(elements(i)) Then Return i
				Next i
			End If
			Return -1
		End Function

		''' <summary>
		''' Returns {@code true} if this list contains the specified element.
		''' More formally, returns {@code true} if and only if this list contains
		''' at least one element {@code e} such that
		''' <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>.
		''' </summary>
		''' <param name="o"> element whose presence in this list is to be tested </param>
		''' <returns> {@code true} if this list contains the specified element </returns>
		Public Overridable Function contains(ByVal o As Object) As Boolean
			Dim elements As Object() = array
			Return IndexOf(o, elements, 0, elements.Length) >= 0
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Function indexOf(ByVal o As Object) As Integer
			Dim elements As Object() = array
			Return IndexOf(o, elements, 0, elements.Length)
		End Function

		''' <summary>
		''' Returns the index of the first occurrence of the specified element in
		''' this list, searching forwards from {@code index}, or returns -1 if
		''' the element is not found.
		''' More formally, returns the lowest index {@code i} such that
		''' <tt>(i&nbsp;&gt;=&nbsp;index&nbsp;&amp;&amp;&nbsp;(e==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;e.equals(get(i))))</tt>,
		''' or -1 if there is no such index.
		''' </summary>
		''' <param name="e"> element to search for </param>
		''' <param name="index"> index to start searching from </param>
		''' <returns> the index of the first occurrence of the element in
		'''         this list at position {@code index} or later in the list;
		'''         {@code -1} if the element is not found. </returns>
		''' <exception cref="IndexOutOfBoundsException"> if the specified index is negative </exception>
		Public Overridable Function indexOf(ByVal e As E, ByVal index As Integer) As Integer
			Dim elements As Object() = array
			Return IndexOf(e, elements, index, elements.Length)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Function lastIndexOf(ByVal o As Object) As Integer
			Dim elements As Object() = array
			Return LastIndexOf(o, elements, elements.Length - 1)
		End Function

		''' <summary>
		''' Returns the index of the last occurrence of the specified element in
		''' this list, searching backwards from {@code index}, or returns -1 if
		''' the element is not found.
		''' More formally, returns the highest index {@code i} such that
		''' <tt>(i&nbsp;&lt;=&nbsp;index&nbsp;&amp;&amp;&nbsp;(e==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;e.equals(get(i))))</tt>,
		''' or -1 if there is no such index.
		''' </summary>
		''' <param name="e"> element to search for </param>
		''' <param name="index"> index to start searching backwards from </param>
		''' <returns> the index of the last occurrence of the element at position
		'''         less than or equal to {@code index} in this list;
		'''         -1 if the element is not found. </returns>
		''' <exception cref="IndexOutOfBoundsException"> if the specified index is greater
		'''         than or equal to the current size of this list </exception>
		Public Overridable Function lastIndexOf(ByVal e As E, ByVal index As Integer) As Integer
			Dim elements As Object() = array
			Return LastIndexOf(e, elements, index)
		End Function

		''' <summary>
		''' Returns a shallow copy of this list.  (The elements themselves
		''' are not copied.)
		''' </summary>
		''' <returns> a clone of this list </returns>
		Public Overridable Function clone() As Object
			Try
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim clone_Renamed As CopyOnWriteArrayList(Of E) = CType(MyBase.clone(), CopyOnWriteArrayList(Of E))
				clone_Renamed.resetLock()
				Return clone_Renamed
			Catch e As CloneNotSupportedException
				' this shouldn't happen, since we are Cloneable
				Throw New InternalError
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
		''' <returns> an array containing all the elements in this list </returns>
		Public Overridable Function toArray() As Object()
			Dim elements As Object() = array
			Return java.util.Arrays.copyOf(elements, elements.Length)
		End Function

		''' <summary>
		''' Returns an array containing all of the elements in this list in
		''' proper sequence (from first to last element); the runtime type of
		''' the returned array is that of the specified array.  If the list fits
		''' in the specified array, it is returned therein.  Otherwise, a new
		''' array is allocated with the runtime type of the specified array and
		''' the size of this list.
		''' 
		''' <p>If this list fits in the specified array with room to spare
		''' (i.e., the array has more elements than this list), the element in
		''' the array immediately following the end of the list is set to
		''' {@code null}.  (This is useful in determining the length of this
		''' list <i>only</i> if the caller knows that this list does not contain
		''' any null elements.)
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
		'''  <pre> {@code String[] y = x.toArray(new String[0]);}</pre>
		''' 
		''' Note that {@code toArray(new Object[0])} is identical in function to
		''' {@code toArray()}.
		''' </summary>
		''' <param name="a"> the array into which the elements of the list are to
		'''          be stored, if it is big enough; otherwise, a new array of the
		'''          same runtime type is allocated for this purpose. </param>
		''' <returns> an array containing all the elements in this list </returns>
		''' <exception cref="ArrayStoreException"> if the runtime type of the specified array
		'''         is not a supertype of the runtime type of every element in
		'''         this list </exception>
		''' <exception cref="NullPointerException"> if the specified array is null </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function toArray(Of T)(ByVal a As T()) As T()
			Dim elements As Object() = array
			Dim len As Integer = elements.Length
			If a.Length < len Then
				Return CType(java.util.Arrays.copyOf(elements, len, a.GetType()), T())
			Else
				Array.Copy(elements, 0, a, 0, len)
				If a.Length > len Then a(len) = Nothing
				Return a
			End If
		End Function

		' Positional Access Operations

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Function [get](ByVal a As Object(), ByVal index As Integer) As E
			Return CType(a(index), E)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overridable Function [get](ByVal index As Integer) As E
			Return [get](array, index)
		End Function

		''' <summary>
		''' Replaces the element at the specified position in this list with the
		''' specified element.
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overridable Function [set](ByVal index As Integer, ByVal element As E) As E
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim elements As Object() = array
				Dim oldValue As E = [get](elements, index)

				If oldValue IsNot element Then
					Dim len As Integer = elements.Length
					Dim newElements As Object() = java.util.Arrays.copyOf(elements, len)
					newElements(index) = element
					array = newElements
				Else
					' Not quite a no-op; ensures volatile write semantics
					array = elements
				End If
				Return oldValue
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Appends the specified element to the end of this list.
		''' </summary>
		''' <param name="e"> element to be appended to this list </param>
		''' <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		Public Overridable Function add(ByVal e As E) As Boolean
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim elements As Object() = array
				Dim len As Integer = elements.Length
				Dim newElements As Object() = java.util.Arrays.copyOf(elements, len + 1)
				newElements(len) = e
				array = newElements
				Return True
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Inserts the specified element at the specified position in this
		''' list. Shifts the element currently at that position (if any) and
		''' any subsequent elements to the right (adds one to their indices).
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overridable Sub add(ByVal index As Integer, ByVal element As E)
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim elements As Object() = array
				Dim len As Integer = elements.Length
				If index > len OrElse index < 0 Then Throw New IndexOutOfBoundsException("Index: " & index & ", Size: " & len)
				Dim newElements As Object()
				Dim numMoved As Integer = len - index
				If numMoved = 0 Then
					newElements = java.util.Arrays.copyOf(elements, len + 1)
				Else
					newElements = New Object(len){}
					Array.Copy(elements, 0, newElements, 0, index)
					Array.Copy(elements, index, newElements, index + 1, numMoved)
				End If
				newElements(index) = element
				array = newElements
			Finally
				lock.unlock()
			End Try
		End Sub

		''' <summary>
		''' Removes the element at the specified position in this list.
		''' Shifts any subsequent elements to the left (subtracts one from their
		''' indices).  Returns the element that was removed from the list.
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overridable Function remove(ByVal index As Integer) As E
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim elements As Object() = array
				Dim len As Integer = elements.Length
				Dim oldValue As E = [get](elements, index)
				Dim numMoved As Integer = len - index - 1
				If numMoved = 0 Then
					array = java.util.Arrays.copyOf(elements, len - 1)
				Else
					Dim newElements As Object() = New Object(len - 2){}
					Array.Copy(elements, 0, newElements, 0, index)
					Array.Copy(elements, index + 1, newElements, index, numMoved)
					array = newElements
				End If
				Return oldValue
			Finally
				lock.unlock()
			End Try
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
		Public Overridable Function remove(ByVal o As Object) As Boolean
			Dim snapshot As Object() = array
			Dim index As Integer = IndexOf(o, snapshot, 0, snapshot.Length)
			Return If(index < 0, False, remove(o, snapshot, index))
		End Function

		''' <summary>
		''' A version of remove(Object) using the strong hint that given
		''' recent snapshot contains o at the given index.
		''' </summary>
		Private Function remove(ByVal o As Object, ByVal snapshot As Object(), ByVal index As Integer) As Boolean
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim current As Object() = array
				Dim len As Integer = current.Length
				If snapshot <> current Then
					findIndex:
					Dim prefix As Integer = System.Math.Min(index, len)
					For i As Integer = 0 To prefix - 1
						If current(i) IsNot snapshot(i) AndAlso eq(o, current(i)) Then
							index = i
							GoTo findIndex
						End If
					Next i
					If index >= len Then Return False
					If current(index) Is o Then GoTo findIndex
					index = IndexOf(o, current, index, len)
					If index < 0 Then Return False
				End If
				Dim newElements As Object() = New Object(len - 2){}
				Array.Copy(current, 0, newElements, 0, index)
				Array.Copy(current, index + 1, newElements, index, len - index - 1)
				array = newElements
				Return True
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Removes from this list all of the elements whose index is between
		''' {@code fromIndex}, inclusive, and {@code toIndex}, exclusive.
		''' Shifts any succeeding elements to the left (reduces their index).
		''' This call shortens the list by {@code (toIndex - fromIndex)} elements.
		''' (If {@code toIndex==fromIndex}, this operation has no effect.)
		''' </summary>
		''' <param name="fromIndex"> index of first element to be removed </param>
		''' <param name="toIndex"> index after last element to be removed </param>
		''' <exception cref="IndexOutOfBoundsException"> if fromIndex or toIndex out of range
		'''         ({@code fromIndex < 0 || toIndex > size() || toIndex < fromIndex}) </exception>
		Friend Overridable Sub removeRange(ByVal fromIndex As Integer, ByVal toIndex As Integer)
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim elements As Object() = array
				Dim len As Integer = elements.Length

				If fromIndex < 0 OrElse toIndex > len OrElse toIndex < fromIndex Then Throw New IndexOutOfBoundsException
				Dim newlen As Integer = len - (toIndex - fromIndex)
				Dim numMoved As Integer = len - toIndex
				If numMoved = 0 Then
					array = java.util.Arrays.copyOf(elements, newlen)
				Else
					Dim newElements As Object() = New Object(newlen - 1){}
					Array.Copy(elements, 0, newElements, 0, fromIndex)
					Array.Copy(elements, toIndex, newElements, fromIndex, numMoved)
					array = newElements
				End If
			Finally
				lock.unlock()
			End Try
		End Sub

		''' <summary>
		''' Appends the element, if not present.
		''' </summary>
		''' <param name="e"> element to be added to this list, if absent </param>
		''' <returns> {@code true} if the element was added </returns>
		Public Overridable Function addIfAbsent(ByVal e As E) As Boolean
			Dim snapshot As Object() = array
			Return If(IndexOf(e, snapshot, 0, snapshot.Length) >= 0, False, addIfAbsent(e, snapshot))
		End Function

		''' <summary>
		''' A version of addIfAbsent using the strong hint that given
		''' recent snapshot does not contain e.
		''' </summary>
		Private Function addIfAbsent(ByVal e As E, ByVal snapshot As Object()) As Boolean
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim current As Object() = array
				Dim len As Integer = current.Length
				If snapshot <> current Then
					' Optimize for lost race to another addXXX operation
					Dim common As Integer = System.Math.Min(snapshot.Length, len)
					For i As Integer = 0 To common - 1
						If current(i) IsNot snapshot(i) AndAlso eq(e, current(i)) Then Return False
					Next i
					If IndexOf(e, current, common, len) >= 0 Then Return False
				End If
				Dim newElements As Object() = java.util.Arrays.copyOf(current, len + 1)
				newElements(len) = e
				array = newElements
				Return True
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Returns {@code true} if this list contains all of the elements of the
		''' specified collection.
		''' </summary>
		''' <param name="c"> collection to be checked for containment in this list </param>
		''' <returns> {@code true} if this list contains all of the elements of the
		'''         specified collection </returns>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
		''' <seealso cref= #contains(Object) </seealso>
		Public Overridable Function containsAll(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
			Dim elements As Object() = array
			Dim len As Integer = elements.Length
			For Each e As Object In c
				If IndexOf(e, elements, 0, len) < 0 Then Return False
			Next e
			Return True
		End Function

		''' <summary>
		''' Removes from this list all of its elements that are contained in
		''' the specified collection. This is a particularly expensive operation
		''' in this class because of the need for an internal temporary array.
		''' </summary>
		''' <param name="c"> collection containing elements to be removed from this list </param>
		''' <returns> {@code true} if this list changed as a result of the call </returns>
		''' <exception cref="ClassCastException"> if the class of an element of this list
		'''         is incompatible with the specified collection
		'''         (<a href="../Collection.html#optional-restrictions">optional</a>) </exception>
		''' <exception cref="NullPointerException"> if this list contains a null element and the
		'''         specified collection does not permit null elements
		'''         (<a href="../Collection.html#optional-restrictions">optional</a>),
		'''         or if the specified collection is null </exception>
		''' <seealso cref= #remove(Object) </seealso>
		Public Overridable Function removeAll(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
			If c Is Nothing Then Throw New NullPointerException
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim elements As Object() = array
				Dim len As Integer = elements.Length
				If len <> 0 Then
					' temp array holds those elements we know we want to keep
					Dim newlen As Integer = 0
					Dim temp As Object() = New Object(len - 1){}
					For i As Integer = 0 To len - 1
						Dim element As Object = elements(i)
						If Not c.Contains(element) Then
							temp(newlen) = element
							newlen += 1
						End If
					Next i
					If newlen <> len Then
						array = java.util.Arrays.copyOf(temp, newlen)
						Return True
					End If
				End If
				Return False
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Retains only the elements in this list that are contained in the
		''' specified collection.  In other words, removes from this list all of
		''' its elements that are not contained in the specified collection.
		''' </summary>
		''' <param name="c"> collection containing elements to be retained in this list </param>
		''' <returns> {@code true} if this list changed as a result of the call </returns>
		''' <exception cref="ClassCastException"> if the class of an element of this list
		'''         is incompatible with the specified collection
		'''         (<a href="../Collection.html#optional-restrictions">optional</a>) </exception>
		''' <exception cref="NullPointerException"> if this list contains a null element and the
		'''         specified collection does not permit null elements
		'''         (<a href="../Collection.html#optional-restrictions">optional</a>),
		'''         or if the specified collection is null </exception>
		''' <seealso cref= #remove(Object) </seealso>
		Public Overridable Function retainAll(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
			If c Is Nothing Then Throw New NullPointerException
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim elements As Object() = array
				Dim len As Integer = elements.Length
				If len <> 0 Then
					' temp array holds those elements we know we want to keep
					Dim newlen As Integer = 0
					Dim temp As Object() = New Object(len - 1){}
					For i As Integer = 0 To len - 1
						Dim element As Object = elements(i)
						If c.Contains(element) Then
							temp(newlen) = element
							newlen += 1
						End If
					Next i
					If newlen <> len Then
						array = java.util.Arrays.copyOf(temp, newlen)
						Return True
					End If
				End If
				Return False
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Appends all of the elements in the specified collection that
		''' are not already contained in this list, to the end of
		''' this list, in the order that they are returned by the
		''' specified collection's iterator.
		''' </summary>
		''' <param name="c"> collection containing elements to be added to this list </param>
		''' <returns> the number of elements added </returns>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
		''' <seealso cref= #addIfAbsent(Object) </seealso>
		Public Overridable Function addAllAbsent(Of T1 As E)(ByVal c As ICollection(Of T1)) As Integer
			Dim cs As Object() = c.ToArray()
			If cs.Length = 0 Then Return 0
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim elements As Object() = array
				Dim len As Integer = elements.Length
				Dim added As Integer = 0
				' uniquify and compact elements in cs
				For i As Integer = 0 To cs.Length - 1
					Dim e As Object = cs(i)
					If IndexOf(e, elements, 0, len) < 0 AndAlso IndexOf(e, cs, 0, added) < 0 Then cs(added) = e
						added += 1
				Next i
				If added > 0 Then
					Dim newElements As Object() = java.util.Arrays.copyOf(elements, len + added)
					Array.Copy(cs, 0, newElements, len, added)
					array = newElements
				End If
				Return added
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Removes all of the elements from this list.
		''' The list will be empty after this call returns.
		''' </summary>
		Public Overridable Sub clear()
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				array = New Object(){}
			Finally
				lock.unlock()
			End Try
		End Sub

		''' <summary>
		''' Appends all of the elements in the specified collection to the end
		''' of this list, in the order that they are returned by the specified
		''' collection's iterator.
		''' </summary>
		''' <param name="c"> collection containing elements to be added to this list </param>
		''' <returns> {@code true} if this list changed as a result of the call </returns>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
		''' <seealso cref= #add(Object) </seealso>
		Public Overridable Function addAll(Of T1 As E)(ByVal c As ICollection(Of T1)) As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim cs As Object() = If(c.GetType() Is GetType(CopyOnWriteArrayList), CType(c, CopyOnWriteArrayList(Of ?)).array, c.ToArray())
			If cs.Length = 0 Then Return False
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim elements As Object() = array
				Dim len As Integer = elements.Length
				If len = 0 AndAlso cs.GetType() Is GetType(Object()) Then
					array = cs
				Else
					Dim newElements As Object() = java.util.Arrays.copyOf(elements, len + cs.Length)
					Array.Copy(cs, 0, newElements, len, cs.Length)
					array = newElements
				End If
				Return True
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Inserts all of the elements in the specified collection into this
		''' list, starting at the specified position.  Shifts the element
		''' currently at that position (if any) and any subsequent elements to
		''' the right (increases their indices).  The new elements will appear
		''' in this list in the order that they are returned by the
		''' specified collection's iterator.
		''' </summary>
		''' <param name="index"> index at which to insert the first element
		'''        from the specified collection </param>
		''' <param name="c"> collection containing elements to be added to this list </param>
		''' <returns> {@code true} if this list changed as a result of the call </returns>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
		''' <seealso cref= #add(int,Object) </seealso>
		Public Overridable Function addAll(Of T1 As E)(ByVal index As Integer, ByVal c As ICollection(Of T1)) As Boolean
			Dim cs As Object() = c.ToArray()
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim elements As Object() = array
				Dim len As Integer = elements.Length
				If index > len OrElse index < 0 Then Throw New IndexOutOfBoundsException("Index: " & index & ", Size: " & len)
				If cs.Length = 0 Then Return False
				Dim numMoved As Integer = len - index
				Dim newElements As Object()
				If numMoved = 0 Then
					newElements = java.util.Arrays.copyOf(elements, len + cs.Length)
				Else
					newElements = New Object(len + cs.Length - 1){}
					Array.Copy(elements, 0, newElements, 0, index)
					Array.Copy(elements, index, newElements, index + cs.Length, numMoved)
				End If
				Array.Copy(cs, 0, newElements, index, cs.Length)
				array = newElements
				Return True
			Finally
				lock.unlock()
			End Try
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
			If action Is Nothing Then Throw New NullPointerException
			Dim elements As Object() = array
			Dim len As Integer = elements.Length
			For i As Integer = 0 To len - 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim e As E = CType(elements(i), E)
				action.accept(e)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function removeIf(Of T1)(ByVal filter As java.util.function.Predicate(Of T1)) As Boolean
			If filter Is Nothing Then Throw New NullPointerException
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim elements As Object() = array
				Dim len As Integer = elements.Length
				If len <> 0 Then
					Dim newlen As Integer = 0
					Dim temp As Object() = New Object(len - 1){}
					For i As Integer = 0 To len - 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim e As E = CType(elements(i), E)
						If Not filter.test(e) Then
							temp(newlen) = e
							newlen += 1
						End If
					Next i
					If newlen <> len Then
						array = java.util.Arrays.copyOf(temp, newlen)
						Return True
					End If
				End If
				Return False
			Finally
				lock.unlock()
			End Try
		End Function

		Public Overridable Sub replaceAll(ByVal [operator] As java.util.function.UnaryOperator(Of E))
			If [operator] Is Nothing Then Throw New NullPointerException
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim elements As Object() = array
				Dim len As Integer = elements.Length
				Dim newElements As Object() = java.util.Arrays.copyOf(elements, len)
				For i As Integer = 0 To len - 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim e As E = CType(elements(i), E)
					newElements(i) = [operator].apply(e)
				Next i
				array = newElements
			Finally
				lock.unlock()
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Sub sort(Of T1)(ByVal c As IComparer(Of T1))
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim elements As Object() = array
				Dim newElements As Object() = java.util.Arrays.copyOf(elements, elements.Length)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim es As E() = CType(newElements, E())
				java.util.Arrays.sort(es, c)
				array = newElements
			Finally
				lock.unlock()
			End Try
		End Sub

		''' <summary>
		''' Saves this list to a stream (that is, serializes it).
		''' </summary>
		''' <param name="s"> the stream </param>
		''' <exception cref="java.io.IOException"> if an I/O error occurs
		''' @serialData The length of the array backing the list is emitted
		'''               (int), followed by all of its elements (each an Object)
		'''               in the proper order. </exception>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)

			s.defaultWriteObject()

			Dim elements As Object() = array
			' Write out array length
			s.writeInt(elements.Length)

			' Write out all elements in the proper order.
			For Each element As Object In elements
				s.writeObject(element)
			Next element
		End Sub

		''' <summary>
		''' Reconstitutes this list from a stream (that is, deserializes it). </summary>
		''' <param name="s"> the stream </param>
		''' <exception cref="ClassNotFoundException"> if the class of a serialized object
		'''         could not be found </exception>
		''' <exception cref="java.io.IOException"> if an I/O error occurs </exception>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)

			s.defaultReadObject()

			' bind to new lock
			resetLock()

			' Read in array length and allocate array
			Dim len As Integer = s.readInt()
			Dim elements As Object() = New Object(len - 1){}

			' Read in all elements in the proper order.
			For i As Integer = 0 To len - 1
				elements(i) = s.readObject()
			Next i
			array = elements
		End Sub

		''' <summary>
		''' Returns a string representation of this list.  The string
		''' representation consists of the string representations of the list's
		''' elements in the order they are returned by its iterator, enclosed in
		''' square brackets ({@code "[]"}).  Adjacent elements are separated by
		''' the characters {@code ", "} (comma and space).  Elements are
		''' converted to strings as by <seealso cref="String#valueOf(Object)"/>.
		''' </summary>
		''' <returns> a string representation of this list </returns>
		Public Overrides Function ToString() As String
			Return java.util.Arrays.ToString(array)
		End Function

		''' <summary>
		''' Compares the specified object with this list for equality.
		''' Returns {@code true} if the specified object is the same object
		''' as this object, or if it is also a <seealso cref="List"/> and the sequence
		''' of elements returned by an <seealso cref="List#iterator() iterator"/>
		''' over the specified list is the same as the sequence returned by
		''' an iterator over this list.  The two sequences are considered to
		''' be the same if they have the same length and corresponding
		''' elements at the same position in the sequence are <em>equal</em>.
		''' Two elements {@code e1} and {@code e2} are considered
		''' <em>equal</em> if {@code (e1==null ? e2==null : e1.equals(e2))}.
		''' </summary>
		''' <param name="o"> the object to be compared for equality with this list </param>
		''' <returns> {@code true} if the specified object is equal to this list </returns>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o Is Me Then Return True
			If Not(TypeOf o Is IList) Then Return False

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim list As IList(Of ?) = CType(o, IList(Of ?))
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim it As IEnumerator(Of ?) = list.GetEnumerator()
			Dim elements As Object() = array
			Dim len As Integer = elements.Length
			For i As Integer = 0 To len - 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				If (Not it.hasNext()) OrElse (Not eq(elements(i), it.next())) Then Return False
			Next i
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If it.hasNext() Then Return False
			Return True
		End Function

		''' <summary>
		''' Returns the hash code value for this list.
		''' 
		''' <p>This implementation uses the definition in <seealso cref="List#hashCode"/>.
		''' </summary>
		''' <returns> the hash code value for this list </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim hashCode As Integer = 1
			Dim elements As Object() = array
			Dim len As Integer = elements.Length
			For i As Integer = 0 To len - 1
				Dim obj As Object = elements(i)
				hashCode = 31*hashCode + (If(obj Is Nothing, 0, obj.GetHashCode()))
			Next i
			Return hashCode
		End Function

		''' <summary>
		''' Returns an iterator over the elements in this list in proper sequence.
		''' 
		''' <p>The returned iterator provides a snapshot of the state of the list
		''' when the iterator was constructed. No synchronization is needed while
		''' traversing the iterator. The iterator does <em>NOT</em> support the
		''' {@code remove} method.
		''' </summary>
		''' <returns> an iterator over the elements in this list in proper sequence </returns>
		Public Overridable Function [iterator]() As IEnumerator(Of E)
			Return New COWIterator(Of E)(array, 0)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' <p>The returned iterator provides a snapshot of the state of the list
		''' when the iterator was constructed. No synchronization is needed while
		''' traversing the iterator. The iterator does <em>NOT</em> support the
		''' {@code remove}, {@code set} or {@code add} methods.
		''' </summary>
		Public Overridable Function listIterator() As IEnumerator(Of E)
			Return New COWIterator(Of E)(array, 0)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' <p>The returned iterator provides a snapshot of the state of the list
		''' when the iterator was constructed. No synchronization is needed while
		''' traversing the iterator. The iterator does <em>NOT</em> support the
		''' {@code remove}, {@code set} or {@code add} methods.
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overridable Function listIterator(ByVal index As Integer) As IEnumerator(Of E)
			Dim elements As Object() = array
			Dim len As Integer = elements.Length
			If index < 0 OrElse index > len Then Throw New IndexOutOfBoundsException("Index: " & index)

			Return New COWIterator(Of E)(elements, index)
		End Function

		''' <summary>
		''' Returns a <seealso cref="Spliterator"/> over the elements in this list.
		''' 
		''' <p>The {@code Spliterator} reports <seealso cref="Spliterator#IMMUTABLE"/>,
		''' <seealso cref="Spliterator#ORDERED"/>, <seealso cref="Spliterator#SIZED"/>, and
		''' <seealso cref="Spliterator#SUBSIZED"/>.
		''' 
		''' <p>The spliterator provides a snapshot of the state of the list
		''' when the spliterator was constructed. No synchronization is needed while
		''' operating on the spliterator.
		''' </summary>
		''' <returns> a {@code Spliterator} over the elements in this list
		''' @since 1.8 </returns>
		Public Overridable Function spliterator() As java.util.Spliterator(Of E)
			Return java.util.Spliterators.spliterator(array, java.util.Spliterator.IMMUTABLE Or java.util.Spliterator.ORDERED)
		End Function

		Friend NotInheritable Class COWIterator(Of E)
			Implements IEnumerator(Of E)

			''' <summary>
			''' Snapshot of the array </summary>
			Private ReadOnly snapshot As Object()
			''' <summary>
			''' Index of element to be returned by subsequent call to next. </summary>
			Private cursor As Integer

			Private Sub New(ByVal elements As Object(), ByVal initialCursor As Integer)
				cursor = initialCursor
				snapshot = elements
			End Sub

			Public Function hasNext() As Boolean
				Return cursor < snapshot.Length
			End Function

			Public Function hasPrevious() As Boolean
				Return cursor > 0
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Function [next]() As E
				If Not hasNext() Then Throw New java.util.NoSuchElementException
					Dim tempVar As Integer = cursor
					cursor += 1
					Return CType(snapshot(tempVar), E)
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Function previous() As E
				If Not hasPrevious() Then Throw New java.util.NoSuchElementException
				cursor -= 1
				Return CType(snapshot(cursor), E)
			End Function

			Public Function nextIndex() As Integer
				Return cursor
			End Function

			Public Function previousIndex() As Integer
				Return cursor-1
			End Function

			''' <summary>
			''' Not supported. Always throws UnsupportedOperationException. </summary>
			''' <exception cref="UnsupportedOperationException"> always; {@code remove}
			'''         is not supported by this iterator. </exception>
			Public Sub remove()
				Throw New UnsupportedOperationException
			End Sub

			''' <summary>
			''' Not supported. Always throws UnsupportedOperationException. </summary>
			''' <exception cref="UnsupportedOperationException"> always; {@code set}
			'''         is not supported by this iterator. </exception>
			Public Sub [set](ByVal e As E)
				Throw New UnsupportedOperationException
			End Sub

			''' <summary>
			''' Not supported. Always throws UnsupportedOperationException. </summary>
			''' <exception cref="UnsupportedOperationException"> always; {@code add}
			'''         is not supported by this iterator. </exception>
			Public Sub add(ByVal e As E)
				Throw New UnsupportedOperationException
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				java.util.Objects.requireNonNull(action)
				Dim elements As Object() = snapshot
				Dim size As Integer = elements.Length
				For i As Integer = cursor To size - 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim e As E = CType(elements(i), E)
					action.accept(e)
				Next i
				cursor = size
			End Sub
		End Class

		''' <summary>
		''' Returns a view of the portion of this list between
		''' {@code fromIndex}, inclusive, and {@code toIndex}, exclusive.
		''' The returned list is backed by this list, so changes in the
		''' returned list are reflected in this list.
		''' 
		''' <p>The semantics of the list returned by this method become
		''' undefined if the backing list (i.e., this list) is modified in
		''' any way other than via the returned list.
		''' </summary>
		''' <param name="fromIndex"> low endpoint (inclusive) of the subList </param>
		''' <param name="toIndex"> high endpoint (exclusive) of the subList </param>
		''' <returns> a view of the specified range within this list </returns>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overridable Function subList(ByVal fromIndex As Integer, ByVal toIndex As Integer) As IList(Of E)
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim elements As Object() = array
				Dim len As Integer = elements.Length
				If fromIndex < 0 OrElse toIndex > len OrElse fromIndex > toIndex Then Throw New IndexOutOfBoundsException
				Return New COWSubList(Of E)(Me, fromIndex, toIndex)
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Sublist for CopyOnWriteArrayList.
		''' This class extends AbstractList merely for convenience, to
		''' avoid having to define addAll, etc. This doesn't hurt, but
		''' is wasteful.  This class does not need or use modCount
		''' mechanics in AbstractList, but does need to check for
		''' concurrent modification using similar mechanics.  On each
		''' operation, the array that we expect the backing list to use
		''' is checked and updated.  Since we do this for all of the
		''' base operations invoked by those defined in AbstractList,
		''' all is well.  While inefficient, this is not worth
		''' improving.  The kinds of list operations inherited from
		''' AbstractList are already so slow on COW sublists that
		''' adding a bit more space/time doesn't seem even noticeable.
		''' </summary>
		Private Class COWSubList(Of E)
			Inherits java.util.AbstractList(Of E)
			Implements java.util.RandomAccess

			Private ReadOnly l As CopyOnWriteArrayList(Of E)
			Private ReadOnly offset As Integer
			Private size_Renamed As Integer
			Private expectedArray As Object()

			' only call this holding l's lock
			Friend Sub New(ByVal list As CopyOnWriteArrayList(Of E), ByVal fromIndex As Integer, ByVal toIndex As Integer)
				l = list
				expectedArray = l.array
				offset = fromIndex
				size_Renamed = toIndex - fromIndex
			End Sub

			' only call this holding l's lock
			Private Sub checkForComodification()
				If l.array <> expectedArray Then Throw New java.util.ConcurrentModificationException
			End Sub

			' only call this holding l's lock
			Private Sub rangeCheck(ByVal index As Integer)
				If index < 0 OrElse index >= size_Renamed Then Throw New IndexOutOfBoundsException("Index: " & index & ",Size: " & size_Renamed)
			End Sub

			Public Overridable Function [set](ByVal index As Integer, ByVal element As E) As E
				Dim lock As java.util.concurrent.locks.ReentrantLock = l.lock
				lock.lock()
				Try
					rangeCheck(index)
					checkForComodification()
					Dim x As E = l.set(index+offset, element)
					expectedArray = l.array
					Return x
				Finally
					lock.unlock()
				End Try
			End Function

			Public Overridable Function [get](ByVal index As Integer) As E
				Dim lock As java.util.concurrent.locks.ReentrantLock = l.lock
				lock.lock()
				Try
					rangeCheck(index)
					checkForComodification()
					Return l.get(index+offset)
				Finally
					lock.unlock()
				End Try
			End Function

			Public Overridable Function size() As Integer
				Dim lock As java.util.concurrent.locks.ReentrantLock = l.lock
				lock.lock()
				Try
					checkForComodification()
					Return size_Renamed
				Finally
					lock.unlock()
				End Try
			End Function

			Public Overridable Sub add(ByVal index As Integer, ByVal element As E)
				Dim lock As java.util.concurrent.locks.ReentrantLock = l.lock
				lock.lock()
				Try
					checkForComodification()
					If index < 0 OrElse index > size_Renamed Then Throw New IndexOutOfBoundsException
					l.add(index+offset, element)
					expectedArray = l.array
					size_Renamed += 1
				Finally
					lock.unlock()
				End Try
			End Sub

			Public Overridable Sub clear()
				Dim lock As java.util.concurrent.locks.ReentrantLock = l.lock
				lock.lock()
				Try
					checkForComodification()
					l.removeRange(offset, offset+size_Renamed)
					expectedArray = l.array
					size_Renamed = 0
				Finally
					lock.unlock()
				End Try
			End Sub

			Public Overridable Function remove(ByVal index As Integer) As E
				Dim lock As java.util.concurrent.locks.ReentrantLock = l.lock
				lock.lock()
				Try
					rangeCheck(index)
					checkForComodification()
					Dim result As E = l.remove(index+offset)
					expectedArray = l.array
					size_Renamed -= 1
					Return result
				Finally
					lock.unlock()
				End Try
			End Function

			Public Overridable Function remove(ByVal o As Object) As Boolean
				Dim index As Integer = IndexOf(o)
				If index = -1 Then Return False
				remove(index)
				Return True
			End Function

			Public Overridable Function [iterator]() As IEnumerator(Of E)
				Dim lock As java.util.concurrent.locks.ReentrantLock = l.lock
				lock.lock()
				Try
					checkForComodification()
					Return New COWSubListIterator(Of E)(l, 0, offset, size_Renamed)
				Finally
					lock.unlock()
				End Try
			End Function

			Public Overridable Function listIterator(ByVal index As Integer) As IEnumerator(Of E)
				Dim lock As java.util.concurrent.locks.ReentrantLock = l.lock
				lock.lock()
				Try
					checkForComodification()
					If index < 0 OrElse index > size_Renamed Then Throw New IndexOutOfBoundsException("Index: " & index & ", Size: " & size_Renamed)
					Return New COWSubListIterator(Of E)(l, index, offset, size_Renamed)
				Finally
					lock.unlock()
				End Try
			End Function

			Public Overridable Function subList(ByVal fromIndex As Integer, ByVal toIndex As Integer) As IList(Of E)
				Dim lock As java.util.concurrent.locks.ReentrantLock = l.lock
				lock.lock()
				Try
					checkForComodification()
					If fromIndex < 0 OrElse toIndex > size_Renamed OrElse fromIndex > toIndex Then Throw New IndexOutOfBoundsException
					Return New COWSubList(Of E)(l, fromIndex + offset, toIndex + offset)
				Finally
					lock.unlock()
				End Try
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overridable Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				If action Is Nothing Then Throw New NullPointerException
				Dim lo As Integer = offset
				Dim hi As Integer = offset + size_Renamed
				Dim a As Object() = expectedArray
				If l.array <> a Then Throw New java.util.ConcurrentModificationException
				If lo < 0 OrElse hi > a.Length Then Throw New IndexOutOfBoundsException
				For i As Integer = lo To hi - 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim e As E = CType(a(i), E)
					action.accept(e)
				Next i
			End Sub

			Public Overridable Sub replaceAll(ByVal [operator] As java.util.function.UnaryOperator(Of E))
				If [operator] Is Nothing Then Throw New NullPointerException
				Dim lock As java.util.concurrent.locks.ReentrantLock = l.lock
				lock.lock()
				Try
					Dim lo As Integer = offset
					Dim hi As Integer = offset + size_Renamed
					Dim elements As Object() = expectedArray
					If l.array <> elements Then Throw New java.util.ConcurrentModificationException
					Dim len As Integer = elements.Length
					If lo < 0 OrElse hi > len Then Throw New IndexOutOfBoundsException
					Dim newElements As Object() = java.util.Arrays.copyOf(elements, len)
					For i As Integer = lo To hi - 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim e As E = CType(elements(i), E)
						newElements(i) = [operator].apply(e)
					Next i
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					l.array = expectedArray = newElements
				Finally
					lock.unlock()
				End Try
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overridable Sub sort(Of T1)(ByVal c As IComparer(Of T1))
				Dim lock As java.util.concurrent.locks.ReentrantLock = l.lock
				lock.lock()
				Try
					Dim lo As Integer = offset
					Dim hi As Integer = offset + size_Renamed
					Dim elements As Object() = expectedArray
					If l.array <> elements Then Throw New java.util.ConcurrentModificationException
					Dim len As Integer = elements.Length
					If lo < 0 OrElse hi > len Then Throw New IndexOutOfBoundsException
					Dim newElements As Object() = java.util.Arrays.copyOf(elements, len)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim es As E() = CType(newElements, E())
					java.util.Arrays.sort(es, lo, hi, c)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					l.array = expectedArray = newElements
				Finally
					lock.unlock()
				End Try
			End Sub

			Public Overridable Function removeAll(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
				If c Is Nothing Then Throw New NullPointerException
				Dim removed As Boolean = False
				Dim lock As java.util.concurrent.locks.ReentrantLock = l.lock
				lock.lock()
				Try
					Dim n As Integer = size_Renamed
					If n > 0 Then
						Dim lo As Integer = offset
						Dim hi As Integer = offset + n
						Dim elements As Object() = expectedArray
						If l.array <> elements Then Throw New java.util.ConcurrentModificationException
						Dim len As Integer = elements.Length
						If lo < 0 OrElse hi > len Then Throw New IndexOutOfBoundsException
						Dim newSize As Integer = 0
						Dim temp As Object() = New Object(n - 1){}
						For i As Integer = lo To hi - 1
							Dim element As Object = elements(i)
							If Not c.Contains(element) Then
								temp(newSize) = element
								newSize += 1
							End If
						Next i
						If newSize <> n Then
							Dim newElements As Object() = New Object(len - n + newSize - 1){}
							Array.Copy(elements, 0, newElements, 0, lo)
							Array.Copy(temp, 0, newElements, lo, newSize)
							Array.Copy(elements, hi, newElements, lo + newSize, len - hi)
							size_Renamed = newSize
							removed = True
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							l.array = expectedArray = newElements
						End If
					End If
				Finally
					lock.unlock()
				End Try
				Return removed
			End Function

			Public Overridable Function retainAll(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
				If c Is Nothing Then Throw New NullPointerException
				Dim removed As Boolean = False
				Dim lock As java.util.concurrent.locks.ReentrantLock = l.lock
				lock.lock()
				Try
					Dim n As Integer = size_Renamed
					If n > 0 Then
						Dim lo As Integer = offset
						Dim hi As Integer = offset + n
						Dim elements As Object() = expectedArray
						If l.array <> elements Then Throw New java.util.ConcurrentModificationException
						Dim len As Integer = elements.Length
						If lo < 0 OrElse hi > len Then Throw New IndexOutOfBoundsException
						Dim newSize As Integer = 0
						Dim temp As Object() = New Object(n - 1){}
						For i As Integer = lo To hi - 1
							Dim element As Object = elements(i)
							If c.Contains(element) Then
								temp(newSize) = element
								newSize += 1
							End If
						Next i
						If newSize <> n Then
							Dim newElements As Object() = New Object(len - n + newSize - 1){}
							Array.Copy(elements, 0, newElements, 0, lo)
							Array.Copy(temp, 0, newElements, lo, newSize)
							Array.Copy(elements, hi, newElements, lo + newSize, len - hi)
							size_Renamed = newSize
							removed = True
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							l.array = expectedArray = newElements
						End If
					End If
				Finally
					lock.unlock()
				End Try
				Return removed
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overridable Function removeIf(Of T1)(ByVal filter As java.util.function.Predicate(Of T1)) As Boolean
				If filter Is Nothing Then Throw New NullPointerException
				Dim removed As Boolean = False
				Dim lock As java.util.concurrent.locks.ReentrantLock = l.lock
				lock.lock()
				Try
					Dim n As Integer = size_Renamed
					If n > 0 Then
						Dim lo As Integer = offset
						Dim hi As Integer = offset + n
						Dim elements As Object() = expectedArray
						If l.array <> elements Then Throw New java.util.ConcurrentModificationException
						Dim len As Integer = elements.Length
						If lo < 0 OrElse hi > len Then Throw New IndexOutOfBoundsException
						Dim newSize As Integer = 0
						Dim temp As Object() = New Object(n - 1){}
						For i As Integer = lo To hi - 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
							Dim e As E = CType(elements(i), E)
							If Not filter.test(e) Then
								temp(newSize) = e
								newSize += 1
							End If
						Next i
						If newSize <> n Then
							Dim newElements As Object() = New Object(len - n + newSize - 1){}
							Array.Copy(elements, 0, newElements, 0, lo)
							Array.Copy(temp, 0, newElements, lo, newSize)
							Array.Copy(elements, hi, newElements, lo + newSize, len - hi)
							size_Renamed = newSize
							removed = True
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							l.array = expectedArray = newElements
						End If
					End If
				Finally
					lock.unlock()
				End Try
				Return removed
			End Function

			Public Overridable Function spliterator() As java.util.Spliterator(Of E)
				Dim lo As Integer = offset
				Dim hi As Integer = offset + size_Renamed
				Dim a As Object() = expectedArray
				If l.array <> a Then Throw New java.util.ConcurrentModificationException
				If lo < 0 OrElse hi > a.Length Then Throw New IndexOutOfBoundsException
				Return java.util.Spliterators.spliterator(a, lo, hi, java.util.Spliterator.IMMUTABLE Or java.util.Spliterator.ORDERED)
			End Function

		End Class

		Private Class COWSubListIterator(Of E)
			Implements IEnumerator(Of E)

			Private ReadOnly it As IEnumerator(Of E)
			Private ReadOnly offset As Integer
			Private ReadOnly size As Integer

			Friend Sub New(ByVal l As IList(Of E), ByVal index As Integer, ByVal offset As Integer, ByVal size As Integer)
				Me.offset = offset
				Me.size = size
				it = l.listIterator(index+offset)
			End Sub

			Public Overridable Function hasNext() As Boolean
				Return nextIndex() < size
			End Function

			Public Overridable Function [next]() As E
				If hasNext() Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Return it.next()
				Else
					Throw New java.util.NoSuchElementException
				End If
			End Function

			Public Overridable Function hasPrevious() As Boolean
				Return previousIndex() >= 0
			End Function

			Public Overridable Function previous() As E
				If hasPrevious() Then
					Return it.previous()
				Else
					Throw New java.util.NoSuchElementException
				End If
			End Function

			Public Overridable Function nextIndex() As Integer
				Return it.nextIndex() - offset
			End Function

			Public Overridable Function previousIndex() As Integer
				Return it.previousIndex() - offset
			End Function

			Public Overridable Sub remove()
				Throw New UnsupportedOperationException
			End Sub

			Public Overridable Sub [set](ByVal e As E)
				Throw New UnsupportedOperationException
			End Sub

			Public Overridable Sub add(ByVal e As E)
				Throw New UnsupportedOperationException
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				java.util.Objects.requireNonNull(action)
				Dim s As Integer = size
				Dim i As IEnumerator(Of E) = it
				Do While nextIndex() < s
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					action.accept(i.next())
				Loop
			End Sub
		End Class

		' Support for resetting lock while deserializing
		Private Sub resetLock()
			UNSAFE.putObjectVolatile(Me, lockOffset, New java.util.concurrent.locks.ReentrantLock)
		End Sub
		Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
		Private Shared ReadOnly lockOffset As Long
		Shared Sub New()
			Try
				UNSAFE = sun.misc.Unsafe.unsafe
				Dim k As  [Class] = GetType(CopyOnWriteArrayList)
				lockOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("lock"))
			Catch e As Exception
				Throw New [Error](e)
			End Try
		End Sub
	End Class

End Namespace
Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' This class provides a skeletal implementation of the <seealso cref="List"/>
	''' interface to minimize the effort required to implement this interface
	''' backed by a "random access" data store (such as an array).  For sequential
	''' access data (such as a linked list), <seealso cref="AbstractSequentialList"/> should
	''' be used in preference to this class.
	''' 
	''' <p>To implement an unmodifiable list, the programmer needs only to extend
	''' this class and provide implementations for the <seealso cref="#get(int)"/> and
	''' <seealso cref="List#size() size()"/> methods.
	''' 
	''' <p>To implement a modifiable list, the programmer must additionally
	''' override the <seealso cref="#set(int, Object) set(int, E)"/> method (which otherwise
	''' throws an {@code UnsupportedOperationException}).  If the list is
	''' variable-size the programmer must additionally override the
	''' <seealso cref="#add(int, Object) add(int, E)"/> and <seealso cref="#remove(int)"/> methods.
	''' 
	''' <p>The programmer should generally provide a  Sub  (no argument) and collection
	''' constructor, as per the recommendation in the <seealso cref="Collection"/> interface
	''' specification.
	''' 
	''' <p>Unlike the other abstract collection implementations, the programmer does
	''' <i>not</i> have to provide an iterator implementation; the iterator and
	''' list iterator are implemented by this [Class], on top of the "random access"
	''' methods:
	''' <seealso cref="#get(int)"/>,
	''' <seealso cref="#set(int, Object) set(int, E)"/>,
	''' <seealso cref="#add(int, Object) add(int, E)"/> and
	''' <seealso cref="#remove(int)"/>.
	''' 
	''' <p>The documentation for each non-abstract method in this class describes its
	''' implementation in detail.  Each of these methods may be overridden if the
	''' collection being implemented admits a more efficient implementation.
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @author  Josh Bloch
	''' @author  Neal Gafter
	''' @since 1.2
	''' </summary>

	Public MustInherit Class AbstractList(Of E)
		Inherits AbstractCollection(Of E)
		Implements List(Of E)

		''' <summary>
		''' Sole constructor.  (For invocation by subclass constructors, typically
		''' implicit.)
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Appends the specified element to the end of this list (optional
		''' operation).
		''' 
		''' <p>Lists that support this operation may place limitations on what
		''' elements may be added to this list.  In particular, some
		''' lists will refuse to add null elements, and others will impose
		''' restrictions on the type of elements that may be added.  List
		''' classes should clearly specify in their documentation any restrictions
		''' on what elements may be added.
		''' 
		''' <p>This implementation calls {@code add(size(), e)}.
		''' 
		''' <p>Note that this implementation throws an
		''' {@code UnsupportedOperationException} unless
		''' <seealso cref="#add(int, Object) add(int, E)"/> is overridden.
		''' </summary>
		''' <param name="e"> element to be appended to this list </param>
		''' <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		''' <exception cref="UnsupportedOperationException"> if the {@code add} operation
		'''         is not supported by this list </exception>
		''' <exception cref="ClassCastException"> if the class of the specified element
		'''         prevents it from being added to this list </exception>
		''' <exception cref="NullPointerException"> if the specified element is null and this
		'''         list does not permit null elements </exception>
		''' <exception cref="IllegalArgumentException"> if some property of this element
		'''         prevents it from being added to this list </exception>
		Public Overridable Function add(  e As E) As Boolean
			add(size(), e)
			Return True
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public MustOverride Function [get](  index As Integer) As E Implements List(Of E).get

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' <p>This implementation always throws an
		''' {@code UnsupportedOperationException}.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="ClassCastException">            {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">          {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
		''' <exception cref="IndexOutOfBoundsException">     {@inheritDoc} </exception>
		Public Overridable Function [set](  index As Integer,   element As E) As E
			Throw New UnsupportedOperationException
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' <p>This implementation always throws an
		''' {@code UnsupportedOperationException}.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="ClassCastException">            {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">          {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
		''' <exception cref="IndexOutOfBoundsException">     {@inheritDoc} </exception>
		Public Overridable Sub add(  index As Integer,   element As E)
			Throw New UnsupportedOperationException
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' <p>This implementation always throws an
		''' {@code UnsupportedOperationException}.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="IndexOutOfBoundsException">     {@inheritDoc} </exception>
		Public Overridable Function remove(  index As Integer) As E Implements List(Of E).remove
			Throw New UnsupportedOperationException
		End Function


		' Search Operations

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' <p>This implementation first gets a list iterator (with
		''' {@code listIterator()}).  Then, it iterates over the list until the
		''' specified element is found or the end of the list is reached.
		''' </summary>
		''' <exception cref="ClassCastException">   {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Function indexOf(  o As Object) As Integer Implements List(Of E).indexOf
			Dim it As ListIterator(Of E) = listIterator()
			If o Is Nothing Then
				Do While it.hasNext()
					If it.next() Is Nothing Then Return it.previousIndex()
				Loop
			Else
				Do While it.hasNext()
					If o.Equals(it.next()) Then Return it.previousIndex()
				Loop
			End If
			Return -1
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' <p>This implementation first gets a list iterator that points to the end
		''' of the list (with {@code listIterator(size())}).  Then, it iterates
		''' backwards over the list until the specified element is found, or the
		''' beginning of the list is reached.
		''' </summary>
		''' <exception cref="ClassCastException">   {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Function lastIndexOf(  o As Object) As Integer Implements List(Of E).lastIndexOf
			Dim it As ListIterator(Of E) = listIterator(size())
			If o Is Nothing Then
				Do While it.hasPrevious()
					If it.previous() Is Nothing Then Return it.nextIndex()
				Loop
			Else
				Do While it.hasPrevious()
					If o.Equals(it.previous()) Then Return it.nextIndex()
				Loop
			End If
			Return -1
		End Function


		' Bulk Operations

		''' <summary>
		''' Removes all of the elements from this list (optional operation).
		''' The list will be empty after this call returns.
		''' 
		''' <p>This implementation calls {@code removeRange(0, size())}.
		''' 
		''' <p>Note that this implementation throws an
		''' {@code UnsupportedOperationException} unless {@code remove(int
		''' index)} or {@code removeRange(int fromIndex, int toIndex)} is
		''' overridden.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> if the {@code clear} operation
		'''         is not supported by this list </exception>
		Public Overridable Sub clear() Implements List(Of E).clear
			removeRange(0, size())
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' <p>This implementation gets an iterator over the specified collection
		''' and iterates over it, inserting the elements obtained from the
		''' iterator into this list at the appropriate position, one at a time,
		''' using {@code add(int, E)}.
		''' Many implementations will override this method for efficiency.
		''' 
		''' <p>Note that this implementation throws an
		''' {@code UnsupportedOperationException} unless
		''' <seealso cref="#add(int, Object) add(int, E)"/> is overridden.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="ClassCastException">            {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">          {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
		''' <exception cref="IndexOutOfBoundsException">     {@inheritDoc} </exception>
		Public Overridable Function addAll(Of T1 As E)(  index As Integer,   c As Collection(Of T1)) As Boolean Implements List(Of E).addAll
			rangeCheckForAdd(index)
			Dim modified As Boolean = False
			For Each e As E In c
				add(index, e)
				index += 1
				modified = True
			Next e
			Return modified
		End Function


		' Iterators

		''' <summary>
		''' Returns an iterator over the elements in this list in proper sequence.
		''' 
		''' <p>This implementation returns a straightforward implementation of the
		''' iterator interface, relying on the backing list's {@code size()},
		''' {@code get(int)}, and {@code remove(int)} methods.
		''' 
		''' <p>Note that the iterator returned by this method will throw an
		''' <seealso cref="UnsupportedOperationException"/> in response to its
		''' {@code remove} method unless the list's {@code remove(int)} method is
		''' overridden.
		''' 
		''' <p>This implementation can be made to throw runtime exceptions in the
		''' face of concurrent modification, as described in the specification
		''' for the (protected) <seealso cref="#modCount"/> field.
		''' </summary>
		''' <returns> an iterator over the elements in this list in proper sequence </returns>
		Public Overridable Function [iterator]() As [Iterator](Of E) Implements List(Of E).iterator
			Return New Itr(Me)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' <p>This implementation returns {@code listIterator(0)}.
		''' </summary>
		''' <seealso cref= #listIterator(int) </seealso>
		Public Overridable Function listIterator() As ListIterator(Of E) Implements List(Of E).listIterator
			Return listIterator(0)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' <p>This implementation returns a straightforward implementation of the
		''' {@code ListIterator} interface that extends the implementation of the
		''' {@code Iterator} interface returned by the {@code iterator()} method.
		''' The {@code ListIterator} implementation relies on the backing list's
		''' {@code get(int)}, {@code set(int, E)}, {@code add(int, E)}
		''' and {@code remove(int)} methods.
		''' 
		''' <p>Note that the list iterator returned by this implementation will
		''' throw an <seealso cref="UnsupportedOperationException"/> in response to its
		''' {@code remove}, {@code set} and {@code add} methods unless the
		''' list's {@code remove(int)}, {@code set(int, E)}, and
		''' {@code add(int, E)} methods are overridden.
		''' 
		''' <p>This implementation can be made to throw runtime exceptions in the
		''' face of concurrent modification, as described in the specification for
		''' the (protected) <seealso cref="#modCount"/> field.
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overridable Function listIterator(  index As Integer) As ListIterator(Of E) Implements List(Of E).listIterator
			rangeCheckForAdd(index)

			Return New ListItr(Me, index)
		End Function

		Private Class Itr
			Implements Iterator(Of E)

			Private ReadOnly outerInstance As AbstractList

			Public Sub New(  outerInstance As AbstractList)
				Me.outerInstance = outerInstance
			End Sub

			''' <summary>
			''' Index of element to be returned by subsequent call to next.
			''' </summary>
			Friend cursor As Integer = 0

			''' <summary>
			''' Index of element returned by most recent call to next or
			''' previous.  Reset to -1 if this element is deleted by a call
			''' to remove.
			''' </summary>
			Friend lastRet As Integer = -1

			''' <summary>
			''' The modCount value that the iterator believes that the backing
			''' List should have.  If this expectation is violated, the iterator
			''' has detected concurrent modification.
			''' </summary>
			Friend expectedModCount As Integer = outerInstance.modCount

			Public Overridable Function hasNext() As Boolean Implements Iterator(Of E).hasNext
				Return cursor <> outerInstance.size()
			End Function

			Public Overridable Function [next]() As E Implements Iterator(Of E).next
				checkForComodification()
				Try
					Dim i As Integer = cursor
					Dim next_Renamed As E = outerInstance.get(i)
					lastRet = i
					cursor = i + 1
					Return next_Renamed
				Catch e As IndexOutOfBoundsException
					checkForComodification()
					Throw New NoSuchElementException
				End Try
			End Function

			Public Overridable Sub remove() Implements Iterator(Of E).remove
				If lastRet < 0 Then Throw New IllegalStateException
				checkForComodification()

				Try
					outerInstance.remove(lastRet)
					If lastRet < cursor Then cursor -= 1
					lastRet = -1
					expectedModCount = outerInstance.modCount
				Catch e As IndexOutOfBoundsException
					Throw New ConcurrentModificationException
				End Try
			End Sub

			Friend Sub checkForComodification()
				If outerInstance.modCount <> expectedModCount Then Throw New ConcurrentModificationException
			End Sub
		End Class

		Private Class ListItr
			Inherits Itr
			Implements ListIterator(Of E)

			Private ReadOnly outerInstance As AbstractList

			Friend Sub New(  outerInstance As AbstractList,   index As Integer)
					Me.outerInstance = outerInstance
				cursor = index
			End Sub

			Public Overridable Function hasPrevious() As Boolean Implements ListIterator(Of E).hasPrevious
				Return cursor <> 0
			End Function

			Public Overridable Function previous() As E Implements ListIterator(Of E).previous
				checkForComodification()
				Try
					Dim i As Integer = cursor - 1
					Dim previous_Renamed As E = outerInstance.get(i)
						cursor = i
						lastRet = cursor
					Return previous_Renamed
				Catch e As IndexOutOfBoundsException
					checkForComodification()
					Throw New NoSuchElementException
				End Try
			End Function

			Public Overridable Function nextIndex() As Integer Implements ListIterator(Of E).nextIndex
				Return cursor
			End Function

			Public Overridable Function previousIndex() As Integer Implements ListIterator(Of E).previousIndex
				Return cursor-1
			End Function

			Public Overridable Sub [set](  e As E)
				If lastRet < 0 Then Throw New IllegalStateException
				checkForComodification()

				Try
					outerInstance.set(lastRet, e)
					expectedModCount = outerInstance.modCount
				Catch ex As IndexOutOfBoundsException
					Throw New ConcurrentModificationException
				End Try
			End Sub

			Public Overridable Sub add(  e As E)
				checkForComodification()

				Try
					Dim i As Integer = cursor
					outerInstance.add(i, e)
					lastRet = -1
					cursor = i + 1
					expectedModCount = outerInstance.modCount
				Catch ex As IndexOutOfBoundsException
					Throw New ConcurrentModificationException
				End Try
			End Sub
		End Class

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' <p>This implementation returns a list that subclasses
		''' {@code AbstractList}.  The subclass stores, in private fields, the
		''' offset of the subList within the backing list, the size of the subList
		''' (which can change over its lifetime), and the expected
		''' {@code modCount} value of the backing list.  There are two variants
		''' of the subclass, one of which implements {@code RandomAccess}.
		''' If this list implements {@code RandomAccess} the returned list will
		''' be an instance of the subclass that implements {@code RandomAccess}.
		''' 
		''' <p>The subclass's {@code set(int, E)}, {@code get(int)},
		''' {@code add(int, E)}, {@code remove(int)}, {@code addAll(int,
		''' Collection)} and {@code removeRange(int, int)} methods all
		''' delegate to the corresponding methods on the backing abstract list,
		''' after bounds-checking the index and adjusting for the offset.  The
		''' {@code addAll(Collection c)} method merely returns {@code addAll(size,
		''' c)}.
		''' 
		''' <p>The {@code listIterator(int)} method returns a "wrapper object"
		''' over a list iterator on the backing list, which is created with the
		''' corresponding method on the backing list.  The {@code iterator} method
		''' merely returns {@code listIterator()}, and the {@code size} method
		''' merely returns the subclass's {@code size} field.
		''' 
		''' <p>All methods first check to see if the actual {@code modCount} of
		''' the backing list is equal to its expected value, and throw a
		''' {@code ConcurrentModificationException} if it is not.
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> if an endpoint index value is out of range
		'''         {@code (fromIndex < 0 || toIndex > size)} </exception>
		''' <exception cref="IllegalArgumentException"> if the endpoint indices are out of order
		'''         {@code (fromIndex > toIndex)} </exception>
		Public Overridable Function subList(  fromIndex As Integer,   toIndex As Integer) As List(Of E) Implements List(Of E).subList
			Return (If(TypeOf Me Is RandomAccess, New RandomAccessSubList(Of )(Me, fromIndex, toIndex), New SubList(Of )(Me, fromIndex, toIndex)))
		End Function

		' Comparison and hashing

		''' <summary>
		''' Compares the specified object with this list for equality.  Returns
		''' {@code true} if and only if the specified object is also a list, both
		''' lists have the same size, and all corresponding pairs of elements in
		''' the two lists are <i>equal</i>.  (Two elements {@code e1} and
		''' {@code e2} are <i>equal</i> if {@code (e1==null ? e2==null :
		''' e1.equals(e2))}.)  In other words, two lists are defined to be
		''' equal if they contain the same elements in the same order.<p>
		''' 
		''' This implementation first checks if the specified object is this
		''' list. If so, it returns {@code true}; if not, it checks if the
		''' specified object is a list. If not, it returns {@code false}; if so,
		''' it iterates over both lists, comparing corresponding pairs of elements.
		''' If any comparison returns {@code false}, this method returns
		''' {@code false}.  If either iterator runs out of elements before the
		''' other it returns {@code false} (as the lists are of unequal length);
		''' otherwise it returns {@code true} when the iterations complete.
		''' </summary>
		''' <param name="o"> the object to be compared for equality with this list </param>
		''' <returns> {@code true} if the specified object is equal to this list </returns>
		Public Overrides Function Equals(  o As Object) As Boolean
			If o Is Me Then Return True
			If Not(TypeOf o Is List) Then Return False

			Dim e1 As ListIterator(Of E) = listIterator()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim e2 As ListIterator(Of ?) = CType(o, List(Of ?)).GetEnumerator()
			Do While e1.MoveNext() AndAlso e2.MoveNext()
				Dim o1 As E = e1.next()
				Dim o2 As Object = e2.Current
				If Not(If(o1 Is Nothing, o2 Is Nothing, o1.Equals(o2))) Then Return False
			Loop
			Return Not(e1.hasNext() OrElse e2.hasNext())
		End Function

		''' <summary>
		''' Returns the hash code value for this list.
		''' 
		''' <p>This implementation uses exactly the code that is used to define the
		''' list hash function in the documentation for the <seealso cref="List#hashCode"/>
		''' method.
		''' </summary>
		''' <returns> the hash code value for this list </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim hashCode As Integer = 1
			For Each e As E In Me
				hashCode = 31*hashCode + (If(e Is Nothing, 0, e.GetHashCode()))
			Next e
			Return hashCode
		End Function

		''' <summary>
		''' Removes from this list all of the elements whose index is between
		''' {@code fromIndex}, inclusive, and {@code toIndex}, exclusive.
		''' Shifts any succeeding elements to the left (reduces their index).
		''' This call shortens the list by {@code (toIndex - fromIndex)} elements.
		''' (If {@code toIndex==fromIndex}, this operation has no effect.)
		''' 
		''' <p>This method is called by the {@code clear} operation on this list
		''' and its subLists.  Overriding this method to take advantage of
		''' the internals of the list implementation can <i>substantially</i>
		''' improve the performance of the {@code clear} operation on this list
		''' and its subLists.
		''' 
		''' <p>This implementation gets a list iterator positioned before
		''' {@code fromIndex}, and repeatedly calls {@code ListIterator.next}
		''' followed by {@code ListIterator.remove} until the entire range has
		''' been removed.  <b>Note: if {@code ListIterator.remove} requires linear
		''' time, this implementation requires quadratic time.</b>
		''' </summary>
		''' <param name="fromIndex"> index of first element to be removed </param>
		''' <param name="toIndex"> index after last element to be removed </param>
		Protected Friend Overridable Sub removeRange(  fromIndex As Integer,   toIndex As Integer)
			Dim it As ListIterator(Of E) = listIterator(fromIndex)
			Dim i As Integer=0
			Dim n As Integer=toIndex-fromIndex
			Do While i<n
				it.next()
				it.remove()
				i += 1
			Loop
		End Sub

		''' <summary>
		''' The number of times this list has been <i>structurally modified</i>.
		''' Structural modifications are those that change the size of the
		''' list, or otherwise perturb it in such a fashion that iterations in
		''' progress may yield incorrect results.
		''' 
		''' <p>This field is used by the iterator and list iterator implementation
		''' returned by the {@code iterator} and {@code listIterator} methods.
		''' If the value of this field changes unexpectedly, the iterator (or list
		''' iterator) will throw a {@code ConcurrentModificationException} in
		''' response to the {@code next}, {@code remove}, {@code previous},
		''' {@code set} or {@code add} operations.  This provides
		''' <i>fail-fast</i> behavior, rather than non-deterministic behavior in
		''' the face of concurrent modification during iteration.
		''' 
		''' <p><b>Use of this field by subclasses is optional.</b> If a subclass
		''' wishes to provide fail-fast iterators (and list iterators), then it
		''' merely has to increment this field in its {@code add(int, E)} and
		''' {@code remove(int)} methods (and any other methods that it overrides
		''' that result in structural modifications to the list).  A single call to
		''' {@code add(int, E)} or {@code remove(int)} must add no more than
		''' one to this field, or the iterators (and list iterators) will throw
		''' bogus {@code ConcurrentModificationExceptions}.  If an implementation
		''' does not wish to provide fail-fast iterators, this field may be
		''' ignored.
		''' </summary>
		<NonSerialized> _
		Protected Friend modCount As Integer = 0

		Private Sub rangeCheckForAdd(  index As Integer)
			If index < 0 OrElse index > size() Then Throw New IndexOutOfBoundsException(outOfBoundsMsg(index))
		End Sub

		Private Function outOfBoundsMsg(  index As Integer) As String
			Return "Index: " & index & ", Size: " & size()
		End Function
	End Class

	Friend Class SubList(Of E)
		Inherits AbstractList(Of E)

		Private ReadOnly l As AbstractList(Of E)
		Private ReadOnly offset As Integer
		Private size_Renamed As Integer

		Friend Sub New(  list As AbstractList(Of E),   fromIndex As Integer,   toIndex As Integer)
			If fromIndex < 0 Then Throw New IndexOutOfBoundsException("fromIndex = " & fromIndex)
			If toIndex > list.size() Then Throw New IndexOutOfBoundsException("toIndex = " & toIndex)
			If fromIndex > toIndex Then Throw New IllegalArgumentException("fromIndex(" & fromIndex & ") > toIndex(" & toIndex & ")")
			l = list
			offset = fromIndex
			size_Renamed = toIndex - fromIndex
			Me.modCount = l.modCount
		End Sub

		Public Overridable Function [set](  index As Integer,   element As E) As E
			rangeCheck(index)
			checkForComodification()
			Return l.set(index+offset, element)
		End Function

		Public Overridable Function [get](  index As Integer) As E
			rangeCheck(index)
			checkForComodification()
			Return l.get(index+offset)
		End Function

		Public Overridable Function size() As Integer
			checkForComodification()
			Return size_Renamed
		End Function

		Public Overridable Sub add(  index As Integer,   element As E)
			rangeCheckForAdd(index)
			checkForComodification()
			l.add(index+offset, element)
			Me.modCount = l.modCount
			size_Renamed += 1
		End Sub

		Public Overridable Function remove(  index As Integer) As E
			rangeCheck(index)
			checkForComodification()
			Dim result As E = l.remove(index+offset)
			Me.modCount = l.modCount
			size_Renamed -= 1
			Return result
		End Function

		Protected Friend Overridable Sub removeRange(  fromIndex As Integer,   toIndex As Integer)
			checkForComodification()
			l.removeRange(fromIndex+offset, toIndex+offset)
			Me.modCount = l.modCount
			size_Renamed -= (toIndex-fromIndex)
		End Sub

		Public Overridable Function addAll(Of T1 As E)(  c As Collection(Of T1)) As Boolean
			Return addAll(size_Renamed, c)
		End Function

		Public Overridable Function addAll(Of T1 As E)(  index As Integer,   c As Collection(Of T1)) As Boolean
			rangeCheckForAdd(index)
			Dim cSize As Integer = c.size()
			If cSize=0 Then Return False

			checkForComodification()
			l.addAll(offset+index, c)
			Me.modCount = l.modCount
			size_Renamed += cSize
			Return True
		End Function

		Public Overridable Function [iterator]() As [Iterator](Of E)
			Return listIterator()
		End Function

		Public Overridable Function listIterator(  index As Integer) As ListIterator(Of E)
			checkForComodification()
			rangeCheckForAdd(index)

			Return New ListIteratorAnonymousInnerClassHelper(Of E)
		End Function

		Private Class ListIteratorAnonymousInnerClassHelper(Of E)
			Implements ListIterator(Of E)

			Private ReadOnly i As ListIterator(Of E) = outerInstance.l.listIterator(index+outerInstance.offset)

			Public Overridable Function hasNext() As Boolean Implements ListIterator(Of E).hasNext
				Return nextIndex() < outerInstance.size_Renamed
			End Function

			Public Overridable Function [next]() As E Implements ListIterator(Of E).next
				If hasNext() Then
					Return i.next()
				Else
					Throw New NoSuchElementException
				End If
			End Function

			Public Overridable Function hasPrevious() As Boolean Implements ListIterator(Of E).hasPrevious
				Return previousIndex() >= 0
			End Function

			Public Overridable Function previous() As E Implements ListIterator(Of E).previous
				If hasPrevious() Then
					Return i.previous()
				Else
					Throw New NoSuchElementException
				End If
			End Function

			Public Overridable Function nextIndex() As Integer Implements ListIterator(Of E).nextIndex
				Return i.nextIndex() - outerInstance.offset
			End Function

			Public Overridable Function previousIndex() As Integer Implements ListIterator(Of E).previousIndex
				Return i.previousIndex() - outerInstance.offset
			End Function

			Public Overridable Sub remove() Implements ListIterator(Of E).remove
				i.remove()
				outerInstance.modCount = outerInstance.l.modCount
				outerInstance.size_Renamed -= 1
			End Sub

			Public Overridable Sub [set](  e As E)
				i.set(e)
			End Sub

			Public Overridable Sub add(  e As E)
				i.add(e)
				outerInstance.modCount = outerInstance.l.modCount
				outerInstance.size_Renamed += 1
			End Sub
		End Class

		Public Overridable Function subList(  fromIndex As Integer,   toIndex As Integer) As List(Of E)
			Return New SubList(Of )(Me, fromIndex, toIndex)
		End Function

		Private Sub rangeCheck(  index As Integer)
			If index < 0 OrElse index >= size_Renamed Then Throw New IndexOutOfBoundsException(outOfBoundsMsg(index))
		End Sub

		Private Sub rangeCheckForAdd(  index As Integer)
			If index < 0 OrElse index > size_Renamed Then Throw New IndexOutOfBoundsException(outOfBoundsMsg(index))
		End Sub

		Private Function outOfBoundsMsg(  index As Integer) As String
			Return "Index: " & index & ", Size: " & size_Renamed
		End Function

		Private Sub checkForComodification()
			If Me.modCount <> l.modCount Then Throw New ConcurrentModificationException
		End Sub
	End Class

	Friend Class RandomAccessSubList(Of E)
		Inherits SubList(Of E)
		Implements RandomAccess

		Friend Sub New(  list As AbstractList(Of E),   fromIndex As Integer,   toIndex As Integer)
			MyBase.New(list, fromIndex, toIndex)
		End Sub

		Public Overridable Function subList(  fromIndex As Integer,   toIndex As Integer) As List(Of E)
			Return New RandomAccessSubList(Of )(Me, fromIndex, toIndex)
		End Function
	End Class

End Namespace
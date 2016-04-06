Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A <seealso cref="NavigableSet"/> implementation based on a <seealso cref="TreeMap"/>.
	''' The elements are ordered using their {@link Comparable natural
	''' ordering}, or by a <seealso cref="Comparator"/> provided at set creation
	''' time, depending on which constructor is used.
	''' 
	''' <p>This implementation provides guaranteed log(n) time cost for the basic
	''' operations ({@code add}, {@code remove} and {@code contains}).
	''' 
	''' <p>Note that the ordering maintained by a set (whether or not an explicit
	''' comparator is provided) must be <i>consistent with equals</i> if it is to
	''' correctly implement the {@code Set} interface.  (See {@code Comparable}
	''' or {@code Comparator} for a precise definition of <i>consistent with
	''' equals</i>.)  This is so because the {@code Set} interface is defined in
	''' terms of the {@code equals} operation, but a {@code TreeSet} instance
	''' performs all element comparisons using its {@code compareTo} (or
	''' {@code compare}) method, so two elements that are deemed equal by this method
	''' are, from the standpoint of the set, equal.  The behavior of a set
	''' <i>is</i> well-defined even if its ordering is inconsistent with equals; it
	''' just fails to obey the general contract of the {@code Set} interface.
	''' 
	''' <p><strong>Note that this implementation is not synchronized.</strong>
	''' If multiple threads access a tree set concurrently, and at least one
	''' of the threads modifies the set, it <i>must</i> be synchronized
	''' externally.  This is typically accomplished by synchronizing on some
	''' object that naturally encapsulates the set.
	''' If no such object exists, the set should be "wrapped" using the
	''' <seealso cref="Collections#synchronizedSortedSet Collections.synchronizedSortedSet"/>
	''' method.  This is best done at creation time, to prevent accidental
	''' unsynchronized access to the set: <pre>
	'''   SortedSet s = Collections.synchronizedSortedSet(new TreeSet(...));</pre>
	''' 
	''' <p>The iterators returned by this class's {@code iterator} method are
	''' <i>fail-fast</i>: if the set is modified at any time after the iterator is
	''' created, in any way except through the iterator's own {@code remove}
	''' method, the iterator will throw a <seealso cref="ConcurrentModificationException"/>.
	''' Thus, in the face of concurrent modification, the iterator fails quickly
	''' and cleanly, rather than risking arbitrary, non-deterministic behavior at
	''' an undetermined time in the future.
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
	''' </summary>
	''' @param <E> the type of elements maintained by this set
	''' 
	''' @author  Josh Bloch </param>
	''' <seealso cref=     Collection </seealso>
	''' <seealso cref=     Set </seealso>
	''' <seealso cref=     HashSet </seealso>
	''' <seealso cref=     Comparable </seealso>
	''' <seealso cref=     Comparator </seealso>
	''' <seealso cref=     TreeMap
	''' @since   1.2 </seealso>

	<Serializable> _
	Public Class TreeSet(Of E)
		Inherits AbstractSet(Of E)
		Implements NavigableSet(Of E), Cloneable

		''' <summary>
		''' The backing map.
		''' </summary>
		<NonSerialized> _
		Private m As NavigableMap(Of E, Object)

		' Dummy value to associate with an Object in the backing Map
		Private Shared ReadOnly PRESENT As New Object

		''' <summary>
		''' Constructs a set backed by the specified navigable map.
		''' </summary>
		Friend Sub New(  m As NavigableMap(Of E, Object))
			Me.m = m
		End Sub

		''' <summary>
		''' Constructs a new, empty tree set, sorted according to the
		''' natural ordering of its elements.  All elements inserted into
		''' the set must implement the <seealso cref="Comparable"/> interface.
		''' Furthermore, all such elements must be <i>mutually
		''' comparable</i>: {@code e1.compareTo(e2)} must not throw a
		''' {@code ClassCastException} for any elements {@code e1} and
		''' {@code e2} in the set.  If the user attempts to add an element
		''' to the set that violates this constraint (for example, the user
		''' attempts to add a string element to a set whose elements are
		''' integers), the {@code add} call will throw a
		''' {@code ClassCastException}.
		''' </summary>
		Public Sub New()
			Me.New(New TreeMap(Of E, Object))
		End Sub

		''' <summary>
		''' Constructs a new, empty tree set, sorted according to the specified
		''' comparator.  All elements inserted into the set must be <i>mutually
		''' comparable</i> by the specified comparator: {@code comparator.compare(e1,
		''' e2)} must not throw a {@code ClassCastException} for any elements
		''' {@code e1} and {@code e2} in the set.  If the user attempts to add
		''' an element to the set that violates this constraint, the
		''' {@code add} call will throw a {@code ClassCastException}.
		''' </summary>
		''' <param name="comparator"> the comparator that will be used to order this set.
		'''        If {@code null}, the {@link Comparable natural
		'''        ordering} of the elements will be used. </param>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Sub New(Of T1)(  comparator As Comparator(Of T1))
			Me.New(New TreeMap(Of )(comparator))
		End Sub

		''' <summary>
		''' Constructs a new tree set containing the elements in the specified
		''' collection, sorted according to the <i>natural ordering</i> of its
		''' elements.  All elements inserted into the set must implement the
		''' <seealso cref="Comparable"/> interface.  Furthermore, all such elements must be
		''' <i>mutually comparable</i>: {@code e1.compareTo(e2)} must not throw a
		''' {@code ClassCastException} for any elements {@code e1} and
		''' {@code e2} in the set.
		''' </summary>
		''' <param name="c"> collection whose elements will comprise the new set </param>
		''' <exception cref="ClassCastException"> if the elements in {@code c} are
		'''         not <seealso cref="Comparable"/>, or are not mutually comparable </exception>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
		Public Sub New(Of T1 As E)(  c As Collection(Of T1))
			Me.New()
			addAll(c)
		End Sub

		''' <summary>
		''' Constructs a new tree set containing the same elements and
		''' using the same ordering as the specified sorted set.
		''' </summary>
		''' <param name="s"> sorted set whose elements will comprise the new set </param>
		''' <exception cref="NullPointerException"> if the specified sorted set is null </exception>
		Public Sub New(  s As SortedSet(Of E))
			Me.New(s.comparator())
			addAll(s)
		End Sub

		''' <summary>
		''' Returns an iterator over the elements in this set in ascending order.
		''' </summary>
		''' <returns> an iterator over the elements in this set in ascending order </returns>
		Public Overridable Function [iterator]() As [Iterator](Of E) Implements NavigableSet(Of E).iterator
			Return m.navigableKeySet().GetEnumerator()
		End Function

		''' <summary>
		''' Returns an iterator over the elements in this set in descending order.
		''' </summary>
		''' <returns> an iterator over the elements in this set in descending order
		''' @since 1.6 </returns>
		Public Overridable Function descendingIterator() As [Iterator](Of E) Implements NavigableSet(Of E).descendingIterator
			Return m.descendingKeySet().GetEnumerator()
		End Function

		''' <summary>
		''' @since 1.6
		''' </summary>
		Public Overridable Function descendingSet() As NavigableSet(Of E) Implements NavigableSet(Of E).descendingSet
			Return New TreeSet(Of )(m.descendingMap())
		End Function

		''' <summary>
		''' Returns the number of elements in this set (its cardinality).
		''' </summary>
		''' <returns> the number of elements in this set (its cardinality) </returns>
		Public Overridable Function size() As Integer
			Return m.size()
		End Function

		''' <summary>
		''' Returns {@code true} if this set contains no elements.
		''' </summary>
		''' <returns> {@code true} if this set contains no elements </returns>
		Public Overridable Property empty As Boolean
			Get
				Return m.empty
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if this set contains the specified element.
		''' More formally, returns {@code true} if and only if this set
		''' contains an element {@code e} such that
		''' <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>.
		''' </summary>
		''' <param name="o"> object to be checked for containment in this set </param>
		''' <returns> {@code true} if this set contains the specified element </returns>
		''' <exception cref="ClassCastException"> if the specified object cannot be compared
		'''         with the elements currently in the set </exception>
		''' <exception cref="NullPointerException"> if the specified element is null
		'''         and this set uses natural ordering, or its comparator
		'''         does not permit null elements </exception>
		Public Overridable Function contains(  o As Object) As Boolean
			Return m.containsKey(o)
		End Function

		''' <summary>
		''' Adds the specified element to this set if it is not already present.
		''' More formally, adds the specified element {@code e} to this set if
		''' the set contains no element {@code e2} such that
		''' <tt>(e==null&nbsp;?&nbsp;e2==null&nbsp;:&nbsp;e.equals(e2))</tt>.
		''' If this set already contains the element, the call leaves the set
		''' unchanged and returns {@code false}.
		''' </summary>
		''' <param name="e"> element to be added to this set </param>
		''' <returns> {@code true} if this set did not already contain the specified
		'''         element </returns>
		''' <exception cref="ClassCastException"> if the specified object cannot be compared
		'''         with the elements currently in this set </exception>
		''' <exception cref="NullPointerException"> if the specified element is null
		'''         and this set uses natural ordering, or its comparator
		'''         does not permit null elements </exception>
		Public Overridable Function add(  e As E) As Boolean
			Return m.put(e, PRESENT) Is Nothing
		End Function

		''' <summary>
		''' Removes the specified element from this set if it is present.
		''' More formally, removes an element {@code e} such that
		''' <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>,
		''' if this set contains such an element.  Returns {@code true} if
		''' this set contained the element (or equivalently, if this set
		''' changed as a result of the call).  (This set will not contain the
		''' element once the call returns.)
		''' </summary>
		''' <param name="o"> object to be removed from this set, if present </param>
		''' <returns> {@code true} if this set contained the specified element </returns>
		''' <exception cref="ClassCastException"> if the specified object cannot be compared
		'''         with the elements currently in this set </exception>
		''' <exception cref="NullPointerException"> if the specified element is null
		'''         and this set uses natural ordering, or its comparator
		'''         does not permit null elements </exception>
		Public Overridable Function remove(  o As Object) As Boolean
			Return m.remove(o) Is PRESENT
		End Function

		''' <summary>
		''' Removes all of the elements from this set.
		''' The set will be empty after this call returns.
		''' </summary>
		Public Overridable Sub clear()
			m.clear()
		End Sub

		''' <summary>
		''' Adds all of the elements in the specified collection to this set.
		''' </summary>
		''' <param name="c"> collection containing elements to be added to this set </param>
		''' <returns> {@code true} if this set changed as a result of the call </returns>
		''' <exception cref="ClassCastException"> if the elements provided cannot be compared
		'''         with the elements currently in the set </exception>
		''' <exception cref="NullPointerException"> if the specified collection is null or
		'''         if any element is null and this set uses natural ordering, or
		'''         its comparator does not permit null elements </exception>
		Public Overridable Function addAll(Of T1 As E)(  c As Collection(Of T1)) As Boolean
			' Use linear-time version if applicable
			If m.size()=0 AndAlso c.size() > 0 AndAlso TypeOf c Is SortedSet AndAlso TypeOf m Is TreeMap Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim [set] As SortedSet(Of ? As E) = CType(c, SortedSet(Of ? As E))
				Dim map As TreeMap(Of E, Object) = CType(m, TreeMap(Of E, Object))
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cc As Comparator(Of ?) = [set].comparator()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim mc As Comparator(Of ?) = map.comparator()
				If cc Is mc OrElse (cc IsNot Nothing AndAlso cc.Equals(mc)) Then
					map.addAllForTreeSet([set], PRESENT)
					Return True
				End If
			End If
			Return MyBase.addAll(c)
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code fromElement} or {@code toElement}
		'''         is null and this set uses natural ordering, or its comparator
		'''         does not permit null elements </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc}
		''' @since 1.6 </exception>
		Public Overridable Function subSet(  fromElement As E,   fromInclusive As Boolean,   toElement As E,   toInclusive As Boolean) As NavigableSet(Of E)
			Return New TreeSet(Of )(m.subMap(fromElement, fromInclusive, toElement, toInclusive))
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code toElement} is null and
		'''         this set uses natural ordering, or its comparator does
		'''         not permit null elements </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc}
		''' @since 1.6 </exception>
		Public Overridable Function headSet(  toElement As E,   inclusive As Boolean) As NavigableSet(Of E)
			Return New TreeSet(Of )(m.headMap(toElement, inclusive))
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code fromElement} is null and
		'''         this set uses natural ordering, or its comparator does
		'''         not permit null elements </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc}
		''' @since 1.6 </exception>
		Public Overridable Function tailSet(  fromElement As E,   inclusive As Boolean) As NavigableSet(Of E)
			Return New TreeSet(Of )(m.tailMap(fromElement, inclusive))
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code fromElement} or
		'''         {@code toElement} is null and this set uses natural ordering,
		'''         or its comparator does not permit null elements </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Public Overridable Function subSet(  fromElement As E,   toElement As E) As SortedSet(Of E)
			Return subSet(fromElement, True, toElement, False)
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code toElement} is null
		'''         and this set uses natural ordering, or its comparator does
		'''         not permit null elements </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Public Overridable Function headSet(  toElement As E) As SortedSet(Of E)
			Return headSet(toElement, False)
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code fromElement} is null
		'''         and this set uses natural ordering, or its comparator does
		'''         not permit null elements </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Public Overridable Function tailSet(  fromElement As E) As SortedSet(Of E)
			Return tailSet(fromElement, True)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Function comparator() As Comparator(Of ?)
			Return m.comparator()
		End Function

		''' <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		Public Overridable Function first() As E
			Return m.firstKey()
		End Function

		''' <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		Public Overridable Function last() As E
			Return m.lastKey()
		End Function

		' NavigableSet API methods

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified element is null
		'''         and this set uses natural ordering, or its comparator
		'''         does not permit null elements
		''' @since 1.6 </exception>
		Public Overridable Function lower(  e As E) As E
			Return m.lowerKey(e)
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified element is null
		'''         and this set uses natural ordering, or its comparator
		'''         does not permit null elements
		''' @since 1.6 </exception>
		Public Overridable Function floor(  e As E) As E
			Return m.floorKey(e)
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified element is null
		'''         and this set uses natural ordering, or its comparator
		'''         does not permit null elements
		''' @since 1.6 </exception>
		Public Overridable Function ceiling(  e As E) As E
			Return m.ceilingKey(e)
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified element is null
		'''         and this set uses natural ordering, or its comparator
		'''         does not permit null elements
		''' @since 1.6 </exception>
		Public Overridable Function higher(  e As E) As E
			Return m.higherKey(e)
		End Function

		''' <summary>
		''' @since 1.6
		''' </summary>
		Public Overridable Function pollFirst() As E Implements NavigableSet(Of E).pollFirst
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim e As KeyValuePair(Of E, ?) = m.pollFirstEntry()
			Return If(e Is Nothing, Nothing, e.Key)
		End Function

		''' <summary>
		''' @since 1.6
		''' </summary>
		Public Overridable Function pollLast() As E Implements NavigableSet(Of E).pollLast
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim e As KeyValuePair(Of E, ?) = m.pollLastEntry()
			Return If(e Is Nothing, Nothing, e.Key)
		End Function

		''' <summary>
		''' Returns a shallow copy of this {@code TreeSet} instance. (The elements
		''' themselves are not cloned.)
		''' </summary>
		''' <returns> a shallow copy of this set </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function clone() As Object
			Dim clone_Renamed As TreeSet(Of E)
			Try
				clone_Renamed = CType(MyBase.clone(), TreeSet(Of E))
			Catch e As CloneNotSupportedException
				Throw New InternalError(e)
			End Try

			clone_Renamed.m = New TreeMap(Of )(m)
			Return clone_Renamed
		End Function

		''' <summary>
		''' Save the state of the {@code TreeSet} instance to a stream (that is,
		''' serialize it).
		''' 
		''' @serialData Emits the comparator used to order this set, or
		'''             {@code null} if it obeys its elements' natural ordering
		'''             (Object), followed by the size of the set (the number of
		'''             elements it contains) (int), followed by all of its
		'''             elements (each an Object) in order (as determined by the
		'''             set's Comparator, or by the elements' natural ordering if
		'''             the set has no Comparator).
		''' </summary>
		Private Sub writeObject(  s As java.io.ObjectOutputStream)
			' Write out any hidden stuff
			s.defaultWriteObject()

			' Write out Comparator
			s.writeObject(m.comparator())

			' Write out size
			s.writeInt(m.size())

			' Write out all elements in the proper order.
			For Each e As E In m.Keys
				s.writeObject(e)
			Next e
		End Sub

		''' <summary>
		''' Reconstitute the {@code TreeSet} instance from a stream (that is,
		''' deserialize it).
		''' </summary>
		Private Sub readObject(  s As java.io.ObjectInputStream)
			' Read in any hidden stuff
			s.defaultReadObject()

			' Read in Comparator
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim c As Comparator(Of ?) = CType(s.readObject(), Comparator(Of ?))

			' Create backing TreeMap
			Dim tm As New TreeMap(Of E, Object)(c)
			m = tm

			' Read in size
			Dim size As Integer = s.readInt()

			tm.readTreeSet(size, s, PRESENT)
		End Sub

		''' <summary>
		''' Creates a <em><a href="Spliterator.html#binding">late-binding</a></em>
		''' and <em>fail-fast</em> <seealso cref="Spliterator"/> over the elements in this
		''' set.
		''' 
		''' <p>The {@code Spliterator} reports <seealso cref="Spliterator#SIZED"/>,
		''' <seealso cref="Spliterator#DISTINCT"/>, <seealso cref="Spliterator#SORTED"/>, and
		''' <seealso cref="Spliterator#ORDERED"/>.  Overriding implementations should document
		''' the reporting of additional characteristic values.
		''' 
		''' <p>The spliterator's comparator (see
		''' <seealso cref="java.util.Spliterator#getComparator()"/>) is {@code null} if
		''' the tree set's comparator (see <seealso cref="#comparator()"/>) is {@code null}.
		''' Otherwise, the spliterator's comparator is the same as or imposes the
		''' same total ordering as the tree set's comparator.
		''' </summary>
		''' <returns> a {@code Spliterator} over the elements in this set
		''' @since 1.8 </returns>
		Public Overridable Function spliterator() As Spliterator(Of E)
			Return TreeMap.keySpliteratorFor(m)
		End Function

		Private Const serialVersionUID As Long = -2479143000061671589L
	End Class

End Namespace
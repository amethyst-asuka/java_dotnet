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
' * Written by Doug Lea with assistance from members of JCP JSR-166
' * Expert Group and released to the public domain, as explained at
' * http://creativecommons.org/publicdomain/zero/1.0/
' 

Namespace java.util.concurrent

	''' <summary>
	''' A scalable concurrent <seealso cref="NavigableSet"/> implementation based on
	''' a <seealso cref="ConcurrentSkipListMap"/>.  The elements of the set are kept
	''' sorted according to their <seealso cref="Comparable natural ordering"/>,
	''' or by a <seealso cref="Comparator"/> provided at set creation time, depending
	''' on which constructor is used.
	''' 
	''' <p>This implementation provides expected average <i>log(n)</i> time
	''' cost for the {@code contains}, {@code add}, and {@code remove}
	''' operations and their variants.  Insertion, removal, and access
	''' operations safely execute concurrently by multiple threads.
	''' 
	''' <p>Iterators and spliterators are
	''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
	''' 
	''' <p>Ascending ordered views and their iterators are faster than
	''' descending ones.
	''' 
	''' <p>Beware that, unlike in most collections, the {@code size}
	''' method is <em>not</em> a constant-time operation. Because of the
	''' asynchronous nature of these sets, determining the current number
	''' of elements requires a traversal of the elements, and so may report
	''' inaccurate results if this collection is modified during traversal.
	''' Additionally, the bulk operations {@code addAll},
	''' {@code removeAll}, {@code retainAll}, {@code containsAll},
	''' {@code equals}, and {@code toArray} are <em>not</em> guaranteed
	''' to be performed atomically. For example, an iterator operating
	''' concurrently with an {@code addAll} operation might view only some
	''' of the added elements.
	''' 
	''' <p>This class and its iterators implement all of the
	''' <em>optional</em> methods of the <seealso cref="Set"/> and <seealso cref="Iterator"/>
	''' interfaces. Like most other concurrent collection implementations,
	''' this class does not permit the use of {@code null} elements,
	''' because {@code null} arguments and return values cannot be reliably
	''' distinguished from the absence of elements.
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @author Doug Lea </summary>
	''' @param <E> the type of elements maintained by this set
	''' @since 1.6 </param>
	<Serializable> _
	Public Class ConcurrentSkipListSet(Of E)
		Inherits java.util.AbstractSet(Of E)
		Implements java.util.NavigableSet(Of E), Cloneable

		Private Const serialVersionUID As Long = -2479143111061671589L

		''' <summary>
		''' The underlying map. Uses Boolean.TRUE as value for each
		''' element.  This field is declared final for the sake of thread
		''' safety, which entails some ugliness in clone().
		''' </summary>
		Private ReadOnly m As ConcurrentNavigableMap(Of E, Object)

		''' <summary>
		''' Constructs a new, empty set that orders its elements according to
		''' their <seealso cref="Comparable natural ordering"/>.
		''' </summary>
		Public Sub New()
			m = New ConcurrentSkipListMap(Of E, Object)
		End Sub

		''' <summary>
		''' Constructs a new, empty set that orders its elements according to
		''' the specified comparator.
		''' </summary>
		''' <param name="comparator"> the comparator that will be used to order this set.
		'''        If {@code null}, the {@link Comparable natural
		'''        ordering} of the elements will be used. </param>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Sub New(Of T1)(ByVal comparator As IComparer(Of T1))
			m = New ConcurrentSkipListMap(Of E, Object)(comparator)
		End Sub

		''' <summary>
		''' Constructs a new set containing the elements in the specified
		''' collection, that orders its elements according to their
		''' <seealso cref="Comparable natural ordering"/>.
		''' </summary>
		''' <param name="c"> The elements that will comprise the new set </param>
		''' <exception cref="ClassCastException"> if the elements in {@code c} are
		'''         not <seealso cref="Comparable"/>, or are not mutually comparable </exception>
		''' <exception cref="NullPointerException"> if the specified collection or any
		'''         of its elements are null </exception>
		Public Sub New(Of T1 As E)(ByVal c As ICollection(Of T1))
			m = New ConcurrentSkipListMap(Of E, Object)
			addAll(c)
		End Sub

		''' <summary>
		''' Constructs a new set containing the same elements and using the
		''' same ordering as the specified sorted set.
		''' </summary>
		''' <param name="s"> sorted set whose elements will comprise the new set </param>
		''' <exception cref="NullPointerException"> if the specified sorted set or any
		'''         of its elements are null </exception>
		Public Sub New(ByVal s As java.util.SortedSet(Of E))
			m = New ConcurrentSkipListMap(Of E, Object)(s.comparator())
			addAll(s)
		End Sub

		''' <summary>
		''' For use by submaps
		''' </summary>
		Friend Sub New(ByVal m As ConcurrentNavigableMap(Of E, Object))
			Me.m = m
		End Sub

		''' <summary>
		''' Returns a shallow copy of this {@code ConcurrentSkipListSet}
		''' instance. (The elements themselves are not cloned.)
		''' </summary>
		''' <returns> a shallow copy of this set </returns>
		Public Overridable Function clone() As ConcurrentSkipListSet(Of E)
			Try
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim clone_Renamed As ConcurrentSkipListSet(Of E) = CType(MyBase.clone(), ConcurrentSkipListSet(Of E))
				clone_Renamed.map = New ConcurrentSkipListMap(Of E, Object)(m)
				Return clone_Renamed
			Catch e As CloneNotSupportedException
				Throw New InternalError
			End Try
		End Function

		' ---------------- Set operations -------------- 

		''' <summary>
		''' Returns the number of elements in this set.  If this set
		''' contains more than {@code Integer.MAX_VALUE} elements, it
		''' returns {@code Integer.MAX_VALUE}.
		''' 
		''' <p>Beware that, unlike in most collections, this method is
		''' <em>NOT</em> a constant-time operation. Because of the
		''' asynchronous nature of these sets, determining the current
		''' number of elements requires traversing them all to count them.
		''' Additionally, it is possible for the size to change during
		''' execution of this method, in which case the returned result
		''' will be inaccurate. Thus, this method is typically not very
		''' useful in concurrent applications.
		''' </summary>
		''' <returns> the number of elements in this set </returns>
		Public Overridable Function size() As Integer
			Return m.size()
		End Function

		''' <summary>
		''' Returns {@code true} if this set contains no elements. </summary>
		''' <returns> {@code true} if this set contains no elements </returns>
		Public Overridable Property empty As Boolean
			Get
				Return m.empty
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if this set contains the specified element.
		''' More formally, returns {@code true} if and only if this set
		''' contains an element {@code e} such that {@code o.equals(e)}.
		''' </summary>
		''' <param name="o"> object to be checked for containment in this set </param>
		''' <returns> {@code true} if this set contains the specified element </returns>
		''' <exception cref="ClassCastException"> if the specified element cannot be
		'''         compared with the elements currently in this set </exception>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function contains(ByVal o As Object) As Boolean
			Return m.containsKey(o)
		End Function

		''' <summary>
		''' Adds the specified element to this set if it is not already present.
		''' More formally, adds the specified element {@code e} to this set if
		''' the set contains no element {@code e2} such that {@code e.equals(e2)}.
		''' If this set already contains the element, the call leaves the set
		''' unchanged and returns {@code false}.
		''' </summary>
		''' <param name="e"> element to be added to this set </param>
		''' <returns> {@code true} if this set did not already contain the
		'''         specified element </returns>
		''' <exception cref="ClassCastException"> if {@code e} cannot be compared
		'''         with the elements currently in this set </exception>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function add(ByVal e As E) As Boolean
			Return m.putIfAbsent(e, Boolean.TRUE) Is Nothing
		End Function

		''' <summary>
		''' Removes the specified element from this set if it is present.
		''' More formally, removes an element {@code e} such that
		''' {@code o.equals(e)}, if this set contains such an element.
		''' Returns {@code true} if this set contained the element (or
		''' equivalently, if this set changed as a result of the call).
		''' (This set will not contain the element once the call returns.)
		''' </summary>
		''' <param name="o"> object to be removed from this set, if present </param>
		''' <returns> {@code true} if this set contained the specified element </returns>
		''' <exception cref="ClassCastException"> if {@code o} cannot be compared
		'''         with the elements currently in this set </exception>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function remove(ByVal o As Object) As Boolean
			Return m.remove(o, Boolean.TRUE)
		End Function

		''' <summary>
		''' Removes all of the elements from this set.
		''' </summary>
		Public Overridable Sub clear()
			m.clear()
		End Sub

		''' <summary>
		''' Returns an iterator over the elements in this set in ascending order.
		''' </summary>
		''' <returns> an iterator over the elements in this set in ascending order </returns>
		Public Overridable Function [iterator]() As IEnumerator(Of E)
			Return m.navigableKeySet().GetEnumerator()
		End Function

		''' <summary>
		''' Returns an iterator over the elements in this set in descending order.
		''' </summary>
		''' <returns> an iterator over the elements in this set in descending order </returns>
		Public Overridable Function descendingIterator() As IEnumerator(Of E)
			Return m.descendingKeySet().GetEnumerator()
		End Function


		' ---------------- AbstractSet Overrides -------------- 

		''' <summary>
		''' Compares the specified object with this set for equality.  Returns
		''' {@code true} if the specified object is also a set, the two sets
		''' have the same size, and every member of the specified set is
		''' contained in this set (or equivalently, every member of this set is
		''' contained in the specified set).  This definition ensures that the
		''' equals method works properly across different implementations of the
		''' set interface.
		''' </summary>
		''' <param name="o"> the object to be compared for equality with this set </param>
		''' <returns> {@code true} if the specified object is equal to this set </returns>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			' Override AbstractSet version to avoid calling size()
			If o Is Me Then Return True
			If Not(TypeOf o Is java.util.Set) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim c As ICollection(Of ?) = CType(o, ICollection(Of ?))
			Try
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
				Return containsAll(c) AndAlso c.containsAll(Me)
			Catch unused As ClassCastException
				Return False
			Catch unused As NullPointerException
				Return False
			End Try
		End Function

		''' <summary>
		''' Removes from this set all of its elements that are contained in
		''' the specified collection.  If the specified collection is also
		''' a set, this operation effectively modifies this set so that its
		''' value is the <i>asymmetric set difference</i> of the two sets.
		''' </summary>
		''' <param name="c"> collection containing elements to be removed from this set </param>
		''' <returns> {@code true} if this set changed as a result of the call </returns>
		''' <exception cref="ClassCastException"> if the types of one or more elements in this
		'''         set are incompatible with the specified collection </exception>
		''' <exception cref="NullPointerException"> if the specified collection or any
		'''         of its elements are null </exception>
		Public Overridable Function removeAll(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
			' Override AbstractSet version to avoid unnecessary call to size()
			Dim modified As Boolean = False
			For Each e As Object In c
				If remove(e) Then modified = True
			Next e
			Return modified
		End Function

		' ---------------- Relational operations -------------- 

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function lower(ByVal e As E) As E
			Return m.lowerKey(e)
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function floor(ByVal e As E) As E
			Return m.floorKey(e)
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function ceiling(ByVal e As E) As E
			Return m.ceilingKey(e)
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function higher(ByVal e As E) As E
			Return m.higherKey(e)
		End Function

		Public Overridable Function pollFirst() As E
			Dim e As KeyValuePair(Of E, Object) = m.pollFirstEntry()
			Return If(e Is Nothing, Nothing, e.Key)
		End Function

		Public Overridable Function pollLast() As E
			Dim e As KeyValuePair(Of E, Object) = m.pollLastEntry()
			Return If(e Is Nothing, Nothing, e.Key)
		End Function


		' ---------------- SortedSet operations -------------- 


'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Function comparator() As IComparer(Of ?)
			Return m.comparator()
		End Function

		''' <exception cref="java.util.NoSuchElementException"> {@inheritDoc} </exception>
		Public Overridable Function first() As E
			Return m.firstKey()
		End Function

		''' <exception cref="java.util.NoSuchElementException"> {@inheritDoc} </exception>
		Public Overridable Function last() As E
			Return m.lastKey()
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code fromElement} or
		'''         {@code toElement} is null </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Public Overridable Function subSet(ByVal fromElement As E, ByVal fromInclusive As Boolean, ByVal toElement As E, ByVal toInclusive As Boolean) As java.util.NavigableSet(Of E)
			Return New ConcurrentSkipListSet(Of E) (m.subMap(fromElement, fromInclusive, toElement, toInclusive))
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code toElement} is null </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Public Overridable Function headSet(ByVal toElement As E, ByVal inclusive As Boolean) As java.util.NavigableSet(Of E)
			Return New ConcurrentSkipListSet(Of E)(m.headMap(toElement, inclusive))
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code fromElement} is null </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Public Overridable Function tailSet(ByVal fromElement As E, ByVal inclusive As Boolean) As java.util.NavigableSet(Of E)
			Return New ConcurrentSkipListSet(Of E)(m.tailMap(fromElement, inclusive))
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code fromElement} or
		'''         {@code toElement} is null </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Public Overridable Function subSet(ByVal fromElement As E, ByVal toElement As E) As java.util.NavigableSet(Of E)
			Return subSet(fromElement, True, toElement, False)
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code toElement} is null </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Public Overridable Function headSet(ByVal toElement As E) As java.util.NavigableSet(Of E)
			Return headSet(toElement, False)
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code fromElement} is null </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Public Overridable Function tailSet(ByVal fromElement As E) As java.util.NavigableSet(Of E)
			Return tailSet(fromElement, True)
		End Function

		''' <summary>
		''' Returns a reverse order view of the elements contained in this set.
		''' The descending set is backed by this set, so changes to the set are
		''' reflected in the descending set, and vice-versa.
		''' 
		''' <p>The returned set has an ordering equivalent to
		''' <seealso cref="Collections#reverseOrder(Comparator) Collections.reverseOrder"/>{@code (comparator())}.
		''' The expression {@code s.descendingSet().descendingSet()} returns a
		''' view of {@code s} essentially equivalent to {@code s}.
		''' </summary>
		''' <returns> a reverse order view of this set </returns>
		Public Overridable Function descendingSet() As java.util.NavigableSet(Of E)
			Return New ConcurrentSkipListSet(Of E)(m.descendingMap())
		End Function

		''' <summary>
		''' Returns a <seealso cref="Spliterator"/> over the elements in this set.
		''' 
		''' <p>The {@code Spliterator} reports <seealso cref="Spliterator#CONCURRENT"/>,
		''' <seealso cref="Spliterator#NONNULL"/>, <seealso cref="Spliterator#DISTINCT"/>,
		''' <seealso cref="Spliterator#SORTED"/> and <seealso cref="Spliterator#ORDERED"/>, with an
		''' encounter order that is ascending order.  Overriding implementations
		''' should document the reporting of additional characteristic values.
		''' 
		''' <p>The spliterator's comparator (see
		''' <seealso cref="java.util.Spliterator#getComparator()"/>) is {@code null} if
		''' the set's comparator (see <seealso cref="#comparator()"/>) is {@code null}.
		''' Otherwise, the spliterator's comparator is the same as or imposes the
		''' same total ordering as the set's comparator.
		''' </summary>
		''' <returns> a {@code Spliterator} over the elements in this set
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function spliterator() As java.util.Spliterator(Of E)
			If TypeOf m Is ConcurrentSkipListMap Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Return CType(m, ConcurrentSkipListMap(Of E, ?)).keySpliterator()
			Else
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Return CType(CType(m, ConcurrentSkipListMap.SubMap(Of E, ?)).keyIterator(), java.util.Spliterator(Of E))
			End If
		End Function

		' Support for resetting map in clone
		Private Property map As ConcurrentNavigableMap(Of E, Object)
			Set(ByVal map As ConcurrentNavigableMap(Of E, Object))
				UNSAFE.putObjectVolatile(Me, mapOffset, map)
			End Set
		End Property

		Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
		Private Shared ReadOnly mapOffset As Long
		Shared Sub New()
			Try
				UNSAFE = sun.misc.Unsafe.unsafe
				Dim k As Class = GetType(ConcurrentSkipListSet)
				mapOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("m"))
			Catch e As Exception
				Throw New [Error](e)
			End Try
		End Sub
	End Class

End Namespace
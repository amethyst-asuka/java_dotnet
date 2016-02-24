Imports System
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' A Red-Black tree based <seealso cref="NavigableMap"/> implementation.
	''' The map is sorted according to the {@link Comparable natural
	''' ordering} of its keys, or by a <seealso cref="Comparator"/> provided at map
	''' creation time, depending on which constructor is used.
	''' 
	''' <p>This implementation provides guaranteed log(n) time cost for the
	''' {@code containsKey}, {@code get}, {@code put} and {@code remove}
	''' operations.  Algorithms are adaptations of those in Cormen, Leiserson, and
	''' Rivest's <em>Introduction to Algorithms</em>.
	''' 
	''' <p>Note that the ordering maintained by a tree map, like any sorted map, and
	''' whether or not an explicit comparator is provided, must be <em>consistent
	''' with {@code equals}</em> if this sorted map is to correctly implement the
	''' {@code Map} interface.  (See {@code Comparable} or {@code Comparator} for a
	''' precise definition of <em>consistent with equals</em>.)  This is so because
	''' the {@code Map} interface is defined in terms of the {@code equals}
	''' operation, but a sorted map performs all key comparisons using its {@code
	''' compareTo} (or {@code compare}) method, so two keys that are deemed equal by
	''' this method are, from the standpoint of the sorted map, equal.  The behavior
	''' of a sorted map <em>is</em> well-defined even if its ordering is
	''' inconsistent with {@code equals}; it just fails to obey the general contract
	''' of the {@code Map} interface.
	''' 
	''' <p><strong>Note that this implementation is not synchronized.</strong>
	''' If multiple threads access a map concurrently, and at least one of the
	''' threads modifies the map structurally, it <em>must</em> be synchronized
	''' externally.  (A structural modification is any operation that adds or
	''' deletes one or more mappings; merely changing the value associated
	''' with an existing key is not a structural modification.)  This is
	''' typically accomplished by synchronizing on some object that naturally
	''' encapsulates the map.
	''' If no such object exists, the map should be "wrapped" using the
	''' <seealso cref="Collections#synchronizedSortedMap Collections.synchronizedSortedMap"/>
	''' method.  This is best done at creation time, to prevent accidental
	''' unsynchronized access to the map: <pre>
	'''   SortedMap m = Collections.synchronizedSortedMap(new TreeMap(...));</pre>
	''' 
	''' <p>The iterators returned by the {@code iterator} method of the collections
	''' returned by all of this class's "collection view methods" are
	''' <em>fail-fast</em>: if the map is structurally modified at any time after
	''' the iterator is created, in any way except through the iterator's own
	''' {@code remove} method, the iterator will throw a {@link
	''' ConcurrentModificationException}.  Thus, in the face of concurrent
	''' modification, the iterator fails quickly and cleanly, rather than risking
	''' arbitrary, non-deterministic behavior at an undetermined time in the future.
	''' 
	''' <p>Note that the fail-fast behavior of an iterator cannot be guaranteed
	''' as it is, generally speaking, impossible to make any hard guarantees in the
	''' presence of unsynchronized concurrent modification.  Fail-fast iterators
	''' throw {@code ConcurrentModificationException} on a best-effort basis.
	''' Therefore, it would be wrong to write a program that depended on this
	''' exception for its correctness:   <em>the fail-fast behavior of iterators
	''' should be used only to detect bugs.</em>
	''' 
	''' <p>All {@code Map.Entry} pairs returned by methods in this class
	''' and its views represent snapshots of mappings at the time they were
	''' produced. They do <strong>not</strong> support the {@code Entry.setValue}
	''' method. (Note however that it is possible to change mappings in the
	''' associated map using {@code put}.)
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' </summary>
	''' @param <K> the type of keys maintained by this map </param>
	''' @param <V> the type of mapped values
	''' 
	''' @author  Josh Bloch and Doug Lea </param>
	''' <seealso cref= Map </seealso>
	''' <seealso cref= HashMap </seealso>
	''' <seealso cref= Hashtable </seealso>
	''' <seealso cref= Comparable </seealso>
	''' <seealso cref= Comparator </seealso>
	''' <seealso cref= Collection
	''' @since 1.2 </seealso>

	<Serializable> _
	Public Class TreeMap(Of K, V)
		Inherits AbstractMap(Of K, V)
		Implements NavigableMap(Of K, V), Cloneable

		''' <summary>
		''' The comparator used to maintain order in this tree map, or
		''' null if it uses the natural ordering of its keys.
		''' 
		''' @serial
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private ReadOnly comparator_Renamed As Comparator(Of ?)

		<NonSerialized> _
		Private root As Entry(Of K, V)

		''' <summary>
		''' The number of entries in the tree
		''' </summary>
		<NonSerialized> _
		Private size_Renamed As Integer = 0

		''' <summary>
		''' The number of structural modifications to the tree.
		''' </summary>
		<NonSerialized> _
		Private modCount As Integer = 0

		''' <summary>
		''' Constructs a new, empty tree map, using the natural ordering of its
		''' keys.  All keys inserted into the map must implement the {@link
		''' Comparable} interface.  Furthermore, all such keys must be
		''' <em>mutually comparable</em>: {@code k1.compareTo(k2)} must not throw
		''' a {@code ClassCastException} for any keys {@code k1} and
		''' {@code k2} in the map.  If the user attempts to put a key into the
		''' map that violates this constraint (for example, the user attempts to
		''' put a string key into a map whose keys are integers), the
		''' {@code put(Object key, Object value)} call will throw a
		''' {@code ClassCastException}.
		''' </summary>
		Public Sub New()
			comparator_Renamed = Nothing
		End Sub

		''' <summary>
		''' Constructs a new, empty tree map, ordered according to the given
		''' comparator.  All keys inserted into the map must be <em>mutually
		''' comparable</em> by the given comparator: {@code comparator.compare(k1,
		''' k2)} must not throw a {@code ClassCastException} for any keys
		''' {@code k1} and {@code k2} in the map.  If the user attempts to put
		''' a key into the map that violates this constraint, the {@code put(Object
		''' key, Object value)} call will throw a
		''' {@code ClassCastException}.
		''' </summary>
		''' <param name="comparator"> the comparator that will be used to order this map.
		'''        If {@code null}, the {@link Comparable natural
		'''        ordering} of the keys will be used. </param>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Sub New(Of T1)(ByVal comparator As Comparator(Of T1))
			Me.comparator_Renamed = comparator
		End Sub

		''' <summary>
		''' Constructs a new tree map containing the same mappings as the given
		''' map, ordered according to the <em>natural ordering</em> of its keys.
		''' All keys inserted into the new map must implement the {@link
		''' Comparable} interface.  Furthermore, all such keys must be
		''' <em>mutually comparable</em>: {@code k1.compareTo(k2)} must not throw
		''' a {@code ClassCastException} for any keys {@code k1} and
		''' {@code k2} in the map.  This method runs in n*log(n) time.
		''' </summary>
		''' <param name="m"> the map whose mappings are to be placed in this map </param>
		''' <exception cref="ClassCastException"> if the keys in m are not <seealso cref="Comparable"/>,
		'''         or are not mutually comparable </exception>
		''' <exception cref="NullPointerException"> if the specified map is null </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Sub New(Of T1 As K, ? As V)(ByVal m As Map(Of T1))
			comparator_Renamed = Nothing
			putAll(m)
		End Sub

		''' <summary>
		''' Constructs a new tree map containing the same mappings and
		''' using the same ordering as the specified sorted map.  This
		''' method runs in linear time.
		''' </summary>
		''' <param name="m"> the sorted map whose mappings are to be placed in this map,
		'''         and whose comparator is to be used to sort this map </param>
		''' <exception cref="NullPointerException"> if the specified map is null </exception>
		Public Sub New(Of T1 As V)(ByVal m As SortedMap(Of T1))
			comparator_Renamed = m.comparator()
			Try
				buildFromSorted(m.size(), m.entrySet().GetEnumerator(), Nothing, Nothing)
			Catch cannotHappen As java.io.IOException
			Catch cannotHappen As ClassNotFoundException
			End Try
		End Sub


		' Query Operations

		''' <summary>
		''' Returns the number of key-value mappings in this map.
		''' </summary>
		''' <returns> the number of key-value mappings in this map </returns>
		Public Overridable Function size() As Integer
			Return size_Renamed
		End Function

		''' <summary>
		''' Returns {@code true} if this map contains a mapping for the specified
		''' key.
		''' </summary>
		''' <param name="key"> key whose presence in this map is to be tested </param>
		''' <returns> {@code true} if this map contains a mapping for the
		'''         specified key </returns>
		''' <exception cref="ClassCastException"> if the specified key cannot be compared
		'''         with the keys currently in the map </exception>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         and this map uses natural ordering, or its comparator
		'''         does not permit null keys </exception>
		Public Overridable Function containsKey(ByVal key As Object) As Boolean
			Return getEntry(key) IsNot Nothing
		End Function

		''' <summary>
		''' Returns {@code true} if this map maps one or more keys to the
		''' specified value.  More formally, returns {@code true} if and only if
		''' this map contains at least one mapping to a value {@code v} such
		''' that {@code (value==null ? v==null : value.equals(v))}.  This
		''' operation will probably require time linear in the map size for
		''' most implementations.
		''' </summary>
		''' <param name="value"> value whose presence in this map is to be tested </param>
		''' <returns> {@code true} if a mapping to {@code value} exists;
		'''         {@code false} otherwise
		''' @since 1.2 </returns>
		Public Overridable Function containsValue(ByVal value As Object) As Boolean
			Dim e As Entry(Of K, V) = firstEntry
			Do While e IsNot Nothing
				If valEquals(value, e.value) Then Return True
				e = successor(e)
			Loop
			Return False
		End Function

		''' <summary>
		''' Returns the value to which the specified key is mapped,
		''' or {@code null} if this map contains no mapping for the key.
		''' 
		''' <p>More formally, if this map contains a mapping from a key
		''' {@code k} to a value {@code v} such that {@code key} compares
		''' equal to {@code k} according to the map's ordering, then this
		''' method returns {@code v}; otherwise it returns {@code null}.
		''' (There can be at most one such mapping.)
		''' 
		''' <p>A return value of {@code null} does not <em>necessarily</em>
		''' indicate that the map contains no mapping for the key; it's also
		''' possible that the map explicitly maps the key to {@code null}.
		''' The <seealso cref="#containsKey containsKey"/> operation may be used to
		''' distinguish these two cases.
		''' </summary>
		''' <exception cref="ClassCastException"> if the specified key cannot be compared
		'''         with the keys currently in the map </exception>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         and this map uses natural ordering, or its comparator
		'''         does not permit null keys </exception>
		Public Overridable Function [get](ByVal key As Object) As V
			Dim p As Entry(Of K, V) = getEntry(key)
			Return (If(p Is Nothing, Nothing, p.value))
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Function comparator() As Comparator(Of ?)
			Return comparator_Renamed
		End Function

		''' <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		Public Overridable Function firstKey() As K
			Return key(firstEntry)
		End Function

		''' <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		Public Overridable Function lastKey() As K
			Return key(lastEntry)
		End Function

		''' <summary>
		''' Copies all of the mappings from the specified map to this map.
		''' These mappings replace any mappings that this map had for any
		''' of the keys currently in the specified map.
		''' </summary>
		''' <param name="map"> mappings to be stored in this map </param>
		''' <exception cref="ClassCastException"> if the class of a key or value in
		'''         the specified map prevents it from being stored in this map </exception>
		''' <exception cref="NullPointerException"> if the specified map is null or
		'''         the specified map contains a null key and this map does not
		'''         permit null keys </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Sub putAll(Of T1 As K, ? As V)(ByVal map As Map(Of T1))
			Dim mapSize As Integer = map.size()
			If size_Renamed=0 AndAlso mapSize<>0 AndAlso TypeOf map Is SortedMap Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim c As Comparator(Of ?) = CType(map, SortedMap(Of ?, ?)).comparator()
				If c Is comparator_Renamed OrElse (c IsNot Nothing AndAlso c.Equals(comparator_Renamed)) Then
					modCount += 1
					Try
						buildFromSorted(mapSize, map.entrySet().GetEnumerator(), Nothing, Nothing)
					Catch cannotHappen As java.io.IOException
					Catch cannotHappen As ClassNotFoundException
					End Try
					Return
				End If
			End If
			MyBase.putAll(map)
		End Sub

		''' <summary>
		''' Returns this map's entry for the given key, or {@code null} if the map
		''' does not contain an entry for the key.
		''' </summary>
		''' <returns> this map's entry for the given key, or {@code null} if the map
		'''         does not contain an entry for the key </returns>
		''' <exception cref="ClassCastException"> if the specified key cannot be compared
		'''         with the keys currently in the map </exception>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         and this map uses natural ordering, or its comparator
		'''         does not permit null keys </exception>
		Friend Function getEntry(ByVal key As Object) As Entry(Of K, V)
			' Offload comparator-based version for sake of performance
			If comparator_Renamed IsNot Nothing Then Return getEntryUsingComparator(key)
			If key Is Nothing Then Throw New NullPointerException
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim k As Comparable(Of ?) = CType(key, Comparable(Of ?))
			Dim p As Entry(Of K, V) = root
			Do While p IsNot Nothing
				Dim cmp As Integer = k.CompareTo(p.key)
				If cmp < 0 Then
					p = p.left
				ElseIf cmp > 0 Then
					p = p.right
				Else
					Return p
				End If
			Loop
			Return Nothing
		End Function

		''' <summary>
		''' Version of getEntry using comparator. Split off from getEntry
		''' for performance. (This is not worth doing for most methods,
		''' that are less dependent on comparator performance, but is
		''' worthwhile here.)
		''' </summary>
		Friend Function getEntryUsingComparator(ByVal key As Object) As Entry(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim k As K = CType(key, K)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim cpr As Comparator(Of ?) = comparator_Renamed
			If cpr IsNot Nothing Then
				Dim p As Entry(Of K, V) = root
				Do While p IsNot Nothing
					Dim cmp As Integer = cpr.Compare(k, p.key)
					If cmp < 0 Then
						p = p.left
					ElseIf cmp > 0 Then
						p = p.right
					Else
						Return p
					End If
				Loop
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Gets the entry corresponding to the specified key; if no such entry
		''' exists, returns the entry for the least key greater than the specified
		''' key; if no such entry exists (i.e., the greatest key in the Tree is less
		''' than the specified key), returns {@code null}.
		''' </summary>
		Friend Function getCeilingEntry(ByVal key As K) As Entry(Of K, V)
			Dim p As Entry(Of K, V) = root
			Do While p IsNot Nothing
				Dim cmp As Integer = compare(key, p.key)
				If cmp < 0 Then
					If p.left IsNot Nothing Then
						p = p.left
					Else
						Return p
					End If
				ElseIf cmp > 0 Then
					If p.right IsNot Nothing Then
						p = p.right
					Else
						Dim parent As Entry(Of K, V) = p.parent
						Dim ch As Entry(Of K, V) = p
						Do While parent IsNot Nothing AndAlso ch Is parent.right
							ch = parent
							parent = parent.parent
						Loop
						Return parent
					End If
				Else
					Return p
				End If
			Loop
			Return Nothing
		End Function

		''' <summary>
		''' Gets the entry corresponding to the specified key; if no such entry
		''' exists, returns the entry for the greatest key less than the specified
		''' key; if no such entry exists, returns {@code null}.
		''' </summary>
		Friend Function getFloorEntry(ByVal key As K) As Entry(Of K, V)
			Dim p As Entry(Of K, V) = root
			Do While p IsNot Nothing
				Dim cmp As Integer = compare(key, p.key)
				If cmp > 0 Then
					If p.right IsNot Nothing Then
						p = p.right
					Else
						Return p
					End If
				ElseIf cmp < 0 Then
					If p.left IsNot Nothing Then
						p = p.left
					Else
						Dim parent As Entry(Of K, V) = p.parent
						Dim ch As Entry(Of K, V) = p
						Do While parent IsNot Nothing AndAlso ch Is parent.left
							ch = parent
							parent = parent.parent
						Loop
						Return parent
					End If
				Else
					Return p
				End If

			Loop
			Return Nothing
		End Function

		''' <summary>
		''' Gets the entry for the least key greater than the specified
		''' key; if no such entry exists, returns the entry for the least
		''' key greater than the specified key; if no such entry exists
		''' returns {@code null}.
		''' </summary>
		Friend Function getHigherEntry(ByVal key As K) As Entry(Of K, V)
			Dim p As Entry(Of K, V) = root
			Do While p IsNot Nothing
				Dim cmp As Integer = compare(key, p.key)
				If cmp < 0 Then
					If p.left IsNot Nothing Then
						p = p.left
					Else
						Return p
					End If
				Else
					If p.right IsNot Nothing Then
						p = p.right
					Else
						Dim parent As Entry(Of K, V) = p.parent
						Dim ch As Entry(Of K, V) = p
						Do While parent IsNot Nothing AndAlso ch Is parent.right
							ch = parent
							parent = parent.parent
						Loop
						Return parent
					End If
				End If
			Loop
			Return Nothing
		End Function

		''' <summary>
		''' Returns the entry for the greatest key less than the specified key; if
		''' no such entry exists (i.e., the least key in the Tree is greater than
		''' the specified key), returns {@code null}.
		''' </summary>
		Friend Function getLowerEntry(ByVal key As K) As Entry(Of K, V)
			Dim p As Entry(Of K, V) = root
			Do While p IsNot Nothing
				Dim cmp As Integer = compare(key, p.key)
				If cmp > 0 Then
					If p.right IsNot Nothing Then
						p = p.right
					Else
						Return p
					End If
				Else
					If p.left IsNot Nothing Then
						p = p.left
					Else
						Dim parent As Entry(Of K, V) = p.parent
						Dim ch As Entry(Of K, V) = p
						Do While parent IsNot Nothing AndAlso ch Is parent.left
							ch = parent
							parent = parent.parent
						Loop
						Return parent
					End If
				End If
			Loop
			Return Nothing
		End Function

		''' <summary>
		''' Associates the specified value with the specified key in this map.
		''' If the map previously contained a mapping for the key, the old
		''' value is replaced.
		''' </summary>
		''' <param name="key"> key with which the specified value is to be associated </param>
		''' <param name="value"> value to be associated with the specified key
		''' </param>
		''' <returns> the previous value associated with {@code key}, or
		'''         {@code null} if there was no mapping for {@code key}.
		'''         (A {@code null} return can also indicate that the map
		'''         previously associated {@code null} with {@code key}.) </returns>
		''' <exception cref="ClassCastException"> if the specified key cannot be compared
		'''         with the keys currently in the map </exception>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         and this map uses natural ordering, or its comparator
		'''         does not permit null keys </exception>
		Public Overridable Function put(ByVal key As K, ByVal value As V) As V
			Dim t As Entry(Of K, V) = root
			If t Is Nothing Then
				compare(key, key) ' type (and possibly null) check

				root = New Entry(Of )(key, value, Nothing)
				size_Renamed = 1
				modCount += 1
				Return Nothing
			End If
			Dim cmp As Integer
			Dim parent As Entry(Of K, V)
			' split comparator and comparable paths
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim cpr As Comparator(Of ?) = comparator_Renamed
			If cpr IsNot Nothing Then
				Do
					parent = t
					cmp = cpr.Compare(key, t.key)
					If cmp < 0 Then
						t = t.left
					ElseIf cmp > 0 Then
						t = t.right
					Else
						Return t.valuelue(value)
					End If
				Loop While t IsNot Nothing
			Else
				If key Is Nothing Then Throw New NullPointerException
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim k As Comparable(Of ?) = CType(key, Comparable(Of ?))
				Do
					parent = t
					cmp = k.CompareTo(t.key)
					If cmp < 0 Then
						t = t.left
					ElseIf cmp > 0 Then
						t = t.right
					Else
						Return t.valuelue(value)
					End If
				Loop While t IsNot Nothing
			End If
			Dim e As New Entry(Of K, V)(key, value, parent)
			If cmp < 0 Then
				parent.left = e
			Else
				parent.right = e
			End If
			fixAfterInsertion(e)
			size_Renamed += 1
			modCount += 1
			Return Nothing
		End Function

		''' <summary>
		''' Removes the mapping for this key from this TreeMap if present.
		''' </summary>
		''' <param name="key"> key for which mapping should be removed </param>
		''' <returns> the previous value associated with {@code key}, or
		'''         {@code null} if there was no mapping for {@code key}.
		'''         (A {@code null} return can also indicate that the map
		'''         previously associated {@code null} with {@code key}.) </returns>
		''' <exception cref="ClassCastException"> if the specified key cannot be compared
		'''         with the keys currently in the map </exception>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         and this map uses natural ordering, or its comparator
		'''         does not permit null keys </exception>
		Public Overridable Function remove(ByVal key As Object) As V
			Dim p As Entry(Of K, V) = getEntry(key)
			If p Is Nothing Then Return Nothing

			Dim oldValue As V = p.value
			deleteEntry(p)
			Return oldValue
		End Function

		''' <summary>
		''' Removes all of the mappings from this map.
		''' The map will be empty after this call returns.
		''' </summary>
		Public Overridable Sub clear()
			modCount += 1
			size_Renamed = 0
			root = Nothing
		End Sub

		''' <summary>
		''' Returns a shallow copy of this {@code TreeMap} instance. (The keys and
		''' values themselves are not cloned.)
		''' </summary>
		''' <returns> a shallow copy of this map </returns>
		Public Overridable Function clone() As Object
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim clone_Renamed As TreeMap(Of ?, ?)
			Try
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				clone_Renamed = CType(MyBase.clone(), TreeMap(Of ?, ?))
			Catch e As CloneNotSupportedException
				Throw New InternalError(e)
			End Try

			' Put clone into "virgin" state (except for comparator)
			clone_Renamed.root = Nothing
			clone_Renamed.size_Renamed = 0
			clone_Renamed.modCount = 0
			clone_Renamed.entrySet_Renamed = Nothing
			clone_Renamed.navigableKeySet_Renamed = Nothing
			clone_Renamed.descendingMap_Renamed = Nothing

			' Initialize clone with our mappings
			Try
				clone_Renamed.buildFromSorted(size_Renamed, entrySet().GetEnumerator(), Nothing, Nothing)
			Catch cannotHappen As java.io.IOException
			Catch cannotHappen As ClassNotFoundException
			End Try

			Return clone_Renamed
		End Function

		' NavigableMap API methods

		''' <summary>
		''' @since 1.6
		''' </summary>
		Public Overridable Function firstEntry() As KeyValuePair(Of K, V) Implements NavigableMap(Of K, V).firstEntry
			Return exportEntry(firstEntry)
		End Function

		''' <summary>
		''' @since 1.6
		''' </summary>
		Public Overridable Function lastEntry() As KeyValuePair(Of K, V) Implements NavigableMap(Of K, V).lastEntry
			Return exportEntry(lastEntry)
		End Function

		''' <summary>
		''' @since 1.6
		''' </summary>
		Public Overridable Function pollFirstEntry() As KeyValuePair(Of K, V) Implements NavigableMap(Of K, V).pollFirstEntry
			Dim p As Entry(Of K, V) = firstEntry
			Dim result As KeyValuePair(Of K, V) = exportEntry(p)
			If p IsNot Nothing Then deleteEntry(p)
			Return result
		End Function

		''' <summary>
		''' @since 1.6
		''' </summary>
		Public Overridable Function pollLastEntry() As KeyValuePair(Of K, V) Implements NavigableMap(Of K, V).pollLastEntry
			Dim p As Entry(Of K, V) = lastEntry
			Dim result As KeyValuePair(Of K, V) = exportEntry(p)
			If p IsNot Nothing Then deleteEntry(p)
			Return result
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         and this map uses natural ordering, or its comparator
		'''         does not permit null keys
		''' @since 1.6 </exception>
		Public Overridable Function lowerEntry(ByVal key As K) As KeyValuePair(Of K, V) Implements NavigableMap(Of K, V).lowerEntry
			Return exportEntry(getLowerEntry(key))
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         and this map uses natural ordering, or its comparator
		'''         does not permit null keys
		''' @since 1.6 </exception>
		Public Overridable Function lowerKey(ByVal key As K) As K Implements NavigableMap(Of K, V).lowerKey
			Return keyOrNull(getLowerEntry(key))
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         and this map uses natural ordering, or its comparator
		'''         does not permit null keys
		''' @since 1.6 </exception>
		Public Overridable Function floorEntry(ByVal key As K) As KeyValuePair(Of K, V) Implements NavigableMap(Of K, V).floorEntry
			Return exportEntry(getFloorEntry(key))
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         and this map uses natural ordering, or its comparator
		'''         does not permit null keys
		''' @since 1.6 </exception>
		Public Overridable Function floorKey(ByVal key As K) As K Implements NavigableMap(Of K, V).floorKey
			Return keyOrNull(getFloorEntry(key))
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         and this map uses natural ordering, or its comparator
		'''         does not permit null keys
		''' @since 1.6 </exception>
		Public Overridable Function ceilingEntry(ByVal key As K) As KeyValuePair(Of K, V) Implements NavigableMap(Of K, V).ceilingEntry
			Return exportEntry(getCeilingEntry(key))
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         and this map uses natural ordering, or its comparator
		'''         does not permit null keys
		''' @since 1.6 </exception>
		Public Overridable Function ceilingKey(ByVal key As K) As K Implements NavigableMap(Of K, V).ceilingKey
			Return keyOrNull(getCeilingEntry(key))
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         and this map uses natural ordering, or its comparator
		'''         does not permit null keys
		''' @since 1.6 </exception>
		Public Overridable Function higherEntry(ByVal key As K) As KeyValuePair(Of K, V) Implements NavigableMap(Of K, V).higherEntry
			Return exportEntry(getHigherEntry(key))
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         and this map uses natural ordering, or its comparator
		'''         does not permit null keys
		''' @since 1.6 </exception>
		Public Overridable Function higherKey(ByVal key As K) As K Implements NavigableMap(Of K, V).higherKey
			Return keyOrNull(getHigherEntry(key))
		End Function

		' Views

		''' <summary>
		''' Fields initialized to contain an instance of the entry set view
		''' the first time this view is requested.  Views are stateless, so
		''' there's no reason to create more than one.
		''' </summary>
		<NonSerialized> _
		Private entrySet_Renamed As EntrySet
		<NonSerialized> _
		Private navigableKeySet_Renamed As KeySet(Of K)
		<NonSerialized> _
		Private descendingMap_Renamed As NavigableMap(Of K, V)

		''' <summary>
		''' Returns a <seealso cref="Set"/> view of the keys contained in this map.
		''' 
		''' <p>The set's iterator returns the keys in ascending order.
		''' The set's spliterator is
		''' <em><a href="Spliterator.html#binding">late-binding</a></em>,
		''' <em>fail-fast</em>, and additionally reports <seealso cref="Spliterator#SORTED"/>
		''' and <seealso cref="Spliterator#ORDERED"/> with an encounter order that is ascending
		''' key order.  The spliterator's comparator (see
		''' <seealso cref="java.util.Spliterator#getComparator()"/>) is {@code null} if
		''' the tree map's comparator (see <seealso cref="#comparator()"/>) is {@code null}.
		''' Otherwise, the spliterator's comparator is the same as or imposes the
		''' same total ordering as the tree map's comparator.
		''' 
		''' <p>The set is backed by the map, so changes to the map are
		''' reflected in the set, and vice-versa.  If the map is modified
		''' while an iteration over the set is in progress (except through
		''' the iterator's own {@code remove} operation), the results of
		''' the iteration are undefined.  The set supports element removal,
		''' which removes the corresponding mapping from the map, via the
		''' {@code Iterator.remove}, {@code Set.remove},
		''' {@code removeAll}, {@code retainAll}, and {@code clear}
		''' operations.  It does not support the {@code add} or {@code addAll}
		''' operations.
		''' </summary>
		Public Overridable Function keySet() As [Set](Of K)
			Return navigableKeySet()
		End Function

		''' <summary>
		''' @since 1.6
		''' </summary>
		Public Overridable Function navigableKeySet() As NavigableSet(Of K) Implements NavigableMap(Of K, V).navigableKeySet
			Dim nks As KeySet(Of K) = navigableKeySet_Renamed
				If nks IsNot Nothing Then
					Return nks
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (navigableKeySet_Renamed = New KeySet(Of )(Me))
				End If
		End Function

		''' <summary>
		''' @since 1.6
		''' </summary>
		Public Overridable Function descendingKeySet() As NavigableSet(Of K) Implements NavigableMap(Of K, V).descendingKeySet
			Return descendingMap().navigableKeySet()
		End Function

		''' <summary>
		''' Returns a <seealso cref="Collection"/> view of the values contained in this map.
		''' 
		''' <p>The collection's iterator returns the values in ascending order
		''' of the corresponding keys. The collection's spliterator is
		''' <em><a href="Spliterator.html#binding">late-binding</a></em>,
		''' <em>fail-fast</em>, and additionally reports <seealso cref="Spliterator#ORDERED"/>
		''' with an encounter order that is ascending order of the corresponding
		''' keys.
		''' 
		''' <p>The collection is backed by the map, so changes to the map are
		''' reflected in the collection, and vice-versa.  If the map is
		''' modified while an iteration over the collection is in progress
		''' (except through the iterator's own {@code remove} operation),
		''' the results of the iteration are undefined.  The collection
		''' supports element removal, which removes the corresponding
		''' mapping from the map, via the {@code Iterator.remove},
		''' {@code Collection.remove}, {@code removeAll},
		''' {@code retainAll} and {@code clear} operations.  It does not
		''' support the {@code add} or {@code addAll} operations.
		''' </summary>
		Public Overridable Function values() As Collection(Of V)
			Dim vs As Collection(Of V) = values_Renamed
				If vs IsNot Nothing Then
					Return vs
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (values_Renamed = New Values(Me))
				End If
		End Function

		''' <summary>
		''' Returns a <seealso cref="Set"/> view of the mappings contained in this map.
		''' 
		''' <p>The set's iterator returns the entries in ascending key order. The
		''' sets's spliterator is
		''' <em><a href="Spliterator.html#binding">late-binding</a></em>,
		''' <em>fail-fast</em>, and additionally reports <seealso cref="Spliterator#SORTED"/> and
		''' <seealso cref="Spliterator#ORDERED"/> with an encounter order that is ascending key
		''' order.
		''' 
		''' <p>The set is backed by the map, so changes to the map are
		''' reflected in the set, and vice-versa.  If the map is modified
		''' while an iteration over the set is in progress (except through
		''' the iterator's own {@code remove} operation, or through the
		''' {@code setValue} operation on a map entry returned by the
		''' iterator) the results of the iteration are undefined.  The set
		''' supports element removal, which removes the corresponding
		''' mapping from the map, via the {@code Iterator.remove},
		''' {@code Set.remove}, {@code removeAll}, {@code retainAll} and
		''' {@code clear} operations.  It does not support the
		''' {@code add} or {@code addAll} operations.
		''' </summary>
		Public Overridable Function entrySet() As [Set](Of KeyValuePair(Of K, V))
			Dim es As EntrySet = entrySet_Renamed
				If es IsNot Nothing Then
					Return es
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (entrySet_Renamed = New EntrySet(Me))
				End If
		End Function

		''' <summary>
		''' @since 1.6
		''' </summary>
		Public Overridable Function descendingMap() As NavigableMap(Of K, V) Implements NavigableMap(Of K, V).descendingMap
			Dim km As NavigableMap(Of K, V) = descendingMap_Renamed
				If km IsNot Nothing Then
					Return km
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (descendingMap_Renamed = New DescendingSubMap(Of )(Me, True, Nothing, True, True, Nothing, True))
				End If
		End Function

		''' <exception cref="ClassCastException">       {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code fromKey} or {@code toKey} is
		'''         null and this map uses natural ordering, or its comparator
		'''         does not permit null keys </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc}
		''' @since 1.6 </exception>
		Public Overridable Function subMap(ByVal fromKey As K, ByVal fromInclusive As Boolean, ByVal toKey As K, ByVal toInclusive As Boolean) As NavigableMap(Of K, V) Implements NavigableMap(Of K, V).subMap
			Return New AscendingSubMap(Of )(Me, False, fromKey, fromInclusive, False, toKey, toInclusive)
		End Function

		''' <exception cref="ClassCastException">       {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code toKey} is null
		'''         and this map uses natural ordering, or its comparator
		'''         does not permit null keys </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc}
		''' @since 1.6 </exception>
		Public Overridable Function headMap(ByVal toKey As K, ByVal inclusive As Boolean) As NavigableMap(Of K, V) Implements NavigableMap(Of K, V).headMap
			Return New AscendingSubMap(Of )(Me, True, Nothing, True, False, toKey, inclusive)
		End Function

		''' <exception cref="ClassCastException">       {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code fromKey} is null
		'''         and this map uses natural ordering, or its comparator
		'''         does not permit null keys </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc}
		''' @since 1.6 </exception>
		Public Overridable Function tailMap(ByVal fromKey As K, ByVal inclusive As Boolean) As NavigableMap(Of K, V) Implements NavigableMap(Of K, V).tailMap
			Return New AscendingSubMap(Of )(Me, False, fromKey, inclusive, True, Nothing, True)
		End Function

		''' <exception cref="ClassCastException">       {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code fromKey} or {@code toKey} is
		'''         null and this map uses natural ordering, or its comparator
		'''         does not permit null keys </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Public Overridable Function subMap(ByVal fromKey As K, ByVal toKey As K) As SortedMap(Of K, V) Implements NavigableMap(Of K, V).subMap
			Return subMap(fromKey, True, toKey, False)
		End Function

		''' <exception cref="ClassCastException">       {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code toKey} is null
		'''         and this map uses natural ordering, or its comparator
		'''         does not permit null keys </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Public Overridable Function headMap(ByVal toKey As K) As SortedMap(Of K, V) Implements NavigableMap(Of K, V).headMap
			Return headMap(toKey, False)
		End Function

		''' <exception cref="ClassCastException">       {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code fromKey} is null
		'''         and this map uses natural ordering, or its comparator
		'''         does not permit null keys </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Public Overridable Function tailMap(ByVal fromKey As K) As SortedMap(Of K, V) Implements NavigableMap(Of K, V).tailMap
			Return tailMap(fromKey, True)
		End Function

		Public Overrides Function replace(ByVal key As K, ByVal oldValue As V, ByVal newValue As V) As Boolean
			Dim p As Entry(Of K, V) = getEntry(key)
			If p IsNot Nothing AndAlso Objects.Equals(oldValue, p.value) Then
				p.value = newValue
				Return True
			End If
			Return False
		End Function

		Public Overrides Function replace(ByVal key As K, ByVal value As V) As V
			Dim p As Entry(Of K, V) = getEntry(key)
			If p IsNot Nothing Then
				Dim oldValue As V = p.value
				p.value = value
				Return oldValue
			End If
			Return Nothing
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Sub forEach(Of T1)(ByVal action As java.util.function.BiConsumer(Of T1))
			Objects.requireNonNull(action)
			Dim expectedModCount As Integer = modCount
			Dim e As Entry(Of K, V) = firstEntry
			Do While e IsNot Nothing
				action.accept(e.key, e.value)

				If expectedModCount <> modCount Then Throw New ConcurrentModificationException
				e = successor(e)
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Sub replaceAll(Of T1 As V)(ByVal [function] As java.util.function.BiFunction(Of T1))
			Objects.requireNonNull([function])
			Dim expectedModCount As Integer = modCount

			Dim e As Entry(Of K, V) = firstEntry
			Do While e IsNot Nothing
				e.value = [function].apply(e.key, e.value)

				If expectedModCount <> modCount Then Throw New ConcurrentModificationException
				e = successor(e)
			Loop
		End Sub

		' View class support

		Friend Class Values
			Inherits AbstractCollection(Of V)

			Private ReadOnly outerInstance As TreeMap

			Public Sub New(ByVal outerInstance As TreeMap)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Function [iterator]() As [Iterator](Of V)
				Return New ValueIterator(outerInstance.firstEntry)
			End Function

			Public Overridable Function size() As Integer
				Return outerInstance.size()
			End Function

			Public Overridable Function contains(ByVal o As Object) As Boolean
				Return outerInstance.containsValue(o)
			End Function

			Public Overridable Function remove(ByVal o As Object) As Boolean
				Dim e As Entry(Of K, V) = outerInstance.firstEntry
				Do While e IsNot Nothing
					If valEquals(e.value, o) Then
						outerInstance.deleteEntry(e)
						Return True
					End If
					e = successor(e)
				Loop
				Return False
			End Function

			Public Overridable Sub clear()
				outerInstance.clear()
			End Sub

			Public Overridable Function spliterator() As Spliterator(Of V)
				Return New ValueSpliterator(Of K, V)(TreeMap.this, Nothing, Nothing, 0, -1, 0)
			End Function
		End Class

		Friend Class EntrySet
			Inherits AbstractSet(Of KeyValuePair(Of K, V))

			Private ReadOnly outerInstance As TreeMap

			Public Sub New(ByVal outerInstance As TreeMap)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Function [iterator]() As [Iterator](Of KeyValuePair(Of K, V))
				Return New EntryIterator(outerInstance.firstEntry)
			End Function

			Public Overridable Function contains(ByVal o As Object) As Boolean
				If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim entry As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
				Dim value As Object = entry.Value
				Dim p As Entry(Of K, V) = outerInstance.getEntry(entry.Key)
				Return p IsNot Nothing AndAlso valEquals(p.value, value)
			End Function

			Public Overridable Function remove(ByVal o As Object) As Boolean
				If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim entry As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
				Dim value As Object = entry.Value
				Dim p As Entry(Of K, V) = outerInstance.getEntry(entry.Key)
				If p IsNot Nothing AndAlso valEquals(p.value, value) Then
					outerInstance.deleteEntry(p)
					Return True
				End If
				Return False
			End Function

			Public Overridable Function size() As Integer
				Return outerInstance.size()
			End Function

			Public Overridable Sub clear()
				outerInstance.clear()
			End Sub

			Public Overridable Function spliterator() As Spliterator(Of KeyValuePair(Of K, V))
				Return New EntrySpliterator(Of K, V)(TreeMap.this, Nothing, Nothing, 0, -1, 0)
			End Function
		End Class

	'    
	'     * Unlike Values and EntrySet, the KeySet class is static,
	'     * delegating to a NavigableMap to allow use by SubMaps, which
	'     * outweighs the ugliness of needing type-tests for the following
	'     * Iterator methods that are defined appropriately in main versus
	'     * submap classes.
	'     

		Friend Overridable Function keyIterator() As [Iterator](Of K)
			Return New KeyIterator(Me, firstEntry)
		End Function

		Friend Overridable Function descendingKeyIterator() As [Iterator](Of K)
			Return New DescendingKeyIterator(Me, lastEntry)
		End Function

		Friend NotInheritable Class KeySet(Of E)
			Inherits AbstractSet(Of E)
			Implements NavigableSet(Of E)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private ReadOnly m As NavigableMap(Of E, ?)
			Friend Sub New(Of T1)(ByVal map As NavigableMap(Of T1))
				m = map
			End Sub

			Public Function [iterator]() As [Iterator](Of E) Implements NavigableSet(Of E).iterator
				If TypeOf m Is TreeMap Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Return CType(m, TreeMap(Of E, ?)).keyIterator()
				Else
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Return CType(m, TreeMap.NavigableSubMap(Of E, ?)).keyIterator()
				End If
			End Function

			Public Function descendingIterator() As [Iterator](Of E) Implements NavigableSet(Of E).descendingIterator
				If TypeOf m Is TreeMap Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Return CType(m, TreeMap(Of E, ?)).descendingKeyIterator()
				Else
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Return CType(m, TreeMap.NavigableSubMap(Of E, ?)).descendingKeyIterator()
				End If
			End Function

			Public Function size() As Integer
				Return m.size()
			End Function
			Public Property empty As Boolean
				Get
					Return m.empty
				End Get
			End Property
			Public Function contains(ByVal o As Object) As Boolean
				Return m.containsKey(o)
			End Function
			Public Sub clear()
				m.clear()
			End Sub
			Public Function lower(ByVal e As E) As E
				Return m.lowerKey(e)
			End Function
			Public Function floor(ByVal e As E) As E
				Return m.floorKey(e)
			End Function
			Public Function ceiling(ByVal e As E) As E
				Return m.ceilingKey(e)
			End Function
			Public Function higher(ByVal e As E) As E
				Return m.higherKey(e)
			End Function
			Public Function first() As E
				Return m.firstKey()
			End Function
			Public Function last() As E
				Return m.lastKey()
			End Function
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Function comparator() As Comparator(Of ?)
				Return m.comparator()
			End Function
			Public Function pollFirst() As E Implements NavigableSet(Of E).pollFirst
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e As KeyValuePair(Of E, ?) = m.pollFirstEntry()
				Return If(e Is Nothing, Nothing, e.Key)
			End Function
			Public Function pollLast() As E Implements NavigableSet(Of E).pollLast
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e As KeyValuePair(Of E, ?) = m.pollLastEntry()
				Return If(e Is Nothing, Nothing, e.Key)
			End Function
			Public Function remove(ByVal o As Object) As Boolean
				Dim oldSize As Integer = size()
				m.remove(o)
				Return size() <> oldSize
			End Function
			Public Function subSet(ByVal fromElement As E, ByVal fromInclusive As Boolean, ByVal toElement As E, ByVal toInclusive As Boolean) As NavigableSet(Of E)
				Return New KeySet(Of )(m.subMap(fromElement, fromInclusive, toElement, toInclusive))
			End Function
			Public Function headSet(ByVal toElement As E, ByVal inclusive As Boolean) As NavigableSet(Of E)
				Return New KeySet(Of )(m.headMap(toElement, inclusive))
			End Function
			Public Function tailSet(ByVal fromElement As E, ByVal inclusive As Boolean) As NavigableSet(Of E)
				Return New KeySet(Of )(m.tailMap(fromElement, inclusive))
			End Function
			Public Function subSet(ByVal fromElement As E, ByVal toElement As E) As SortedSet(Of E)
				Return subSet(fromElement, True, toElement, False)
			End Function
			Public Function headSet(ByVal toElement As E) As SortedSet(Of E)
				Return headSet(toElement, False)
			End Function
			Public Function tailSet(ByVal fromElement As E) As SortedSet(Of E)
				Return tailSet(fromElement, True)
			End Function
			Public Function descendingSet() As NavigableSet(Of E) Implements NavigableSet(Of E).descendingSet
				Return New KeySet(Of )(m.descendingMap())
			End Function

			Public Function spliterator() As Spliterator(Of E)
				Return keySpliteratorFor(m)
			End Function
		End Class

		''' <summary>
		''' Base class for TreeMap Iterators
		''' </summary>
		Friend MustInherit Class PrivateEntryIterator(Of T)
			Implements Iterator(Of T)

			Private ReadOnly outerInstance As TreeMap

			Friend [next] As Entry(Of K, V)
			Friend lastReturned As Entry(Of K, V)
			Friend expectedModCount As Integer

			Friend Sub New(ByVal outerInstance As TreeMap, ByVal first As Entry(Of K, V))
					Me.outerInstance = outerInstance
				expectedModCount = outerInstance.modCount
				lastReturned = Nothing
				[next] = first
			End Sub

			Public Function hasNext() As Boolean Implements Iterator(Of T).hasNext
				Return [next] IsNot Nothing
			End Function

			Friend Function nextEntry() As Entry(Of K, V)
				Dim e As Entry(Of K, V) = [next]
				If e Is Nothing Then Throw New NoSuchElementException
				If outerInstance.modCount <> expectedModCount Then Throw New ConcurrentModificationException
				[next] = successor(e)
				lastReturned = e
				Return e
			End Function

			Friend Function prevEntry() As Entry(Of K, V)
				Dim e As Entry(Of K, V) = [next]
				If e Is Nothing Then Throw New NoSuchElementException
				If outerInstance.modCount <> expectedModCount Then Throw New ConcurrentModificationException
				[next] = predecessor(e)
				lastReturned = e
				Return e
			End Function

			Public Overridable Sub remove() Implements Iterator(Of T).remove
				If lastReturned Is Nothing Then Throw New IllegalStateException
				If outerInstance.modCount <> expectedModCount Then Throw New ConcurrentModificationException
				' deleted entries are replaced by their successors
				If lastReturned.left IsNot Nothing AndAlso lastReturned.right IsNot Nothing Then [next] = lastReturned
				outerInstance.deleteEntry(lastReturned)
				expectedModCount = outerInstance.modCount
				lastReturned = Nothing
			End Sub
		End Class

		Friend NotInheritable Class EntryIterator
			Inherits PrivateEntryIterator(Of KeyValuePair(Of K, V))

			Private ReadOnly outerInstance As TreeMap

			Friend Sub New(ByVal outerInstance As TreeMap, ByVal first As Entry(Of K, V))
					Me.outerInstance = outerInstance
				MyBase.New(first)
			End Sub
			Public Function [next]() As KeyValuePair(Of K, V)
				Return nextEntry()
			End Function
		End Class

		Friend NotInheritable Class ValueIterator
			Inherits PrivateEntryIterator(Of V)

			Private ReadOnly outerInstance As TreeMap

			Friend Sub New(ByVal outerInstance As TreeMap, ByVal first As Entry(Of K, V))
					Me.outerInstance = outerInstance
				MyBase.New(first)
			End Sub
			Public Function [next]() As V
				Return nextEntry().value
			End Function
		End Class

		Friend NotInheritable Class KeyIterator
			Inherits PrivateEntryIterator(Of K)

			Private ReadOnly outerInstance As TreeMap

			Friend Sub New(ByVal outerInstance As TreeMap, ByVal first As Entry(Of K, V))
					Me.outerInstance = outerInstance
				MyBase.New(first)
			End Sub
			Public Function [next]() As K
				Return nextEntry().key
			End Function
		End Class

		Friend NotInheritable Class DescendingKeyIterator
			Inherits PrivateEntryIterator(Of K)

			Private ReadOnly outerInstance As TreeMap

			Friend Sub New(ByVal outerInstance As TreeMap, ByVal first As Entry(Of K, V))
					Me.outerInstance = outerInstance
				MyBase.New(first)
			End Sub
			Public Function [next]() As K
				Return prevEntry().key
			End Function
			Public Sub remove()
				If lastReturned Is Nothing Then Throw New IllegalStateException
				If outerInstance.modCount <> expectedModCount Then Throw New ConcurrentModificationException
				outerInstance.deleteEntry(lastReturned)
				lastReturned = Nothing
				expectedModCount = outerInstance.modCount
			End Sub
		End Class

		' Little utilities

		''' <summary>
		''' Compares two keys using the correct comparison method for this TreeMap.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Function compare(ByVal k1 As Object, ByVal k2 As Object) As Integer
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Return If(comparator_Renamed Is Nothing, CType(k1, Comparable(Of ?)).CompareTo(CType(k2, K)), comparator_Renamed.Compare(CType(k1, K), CType(k2, K)))
		End Function

		''' <summary>
		''' Test two values for equality.  Differs from o1.equals(o2) only in
		''' that it copes with {@code null} o1 properly.
		''' </summary>
		Friend Shared Function valEquals(ByVal o1 As Object, ByVal o2 As Object) As Boolean
			Return (If(o1 Is Nothing, o2 Is Nothing, o1.Equals(o2)))
		End Function

		''' <summary>
		''' Return SimpleImmutableEntry for entry, or null if null
		''' </summary>
		Shared Function exportEntry(Of K, V)(ByVal e As TreeMap.Entry(Of K, V)) As KeyValuePair(Of K, V)
			Return If(e Is Nothing, Nothing, New AbstractMap.SimpleImmutableEntry(Of )(e))
		End Function

		''' <summary>
		''' Return key for entry, or null if null
		''' </summary>
		Shared Function keyOrNull(Of K, V)(ByVal e As TreeMap.Entry(Of K, V)) As K
			Return If(e Is Nothing, Nothing, e.key)
		End Function

		''' <summary>
		''' Returns the key corresponding to the specified Entry. </summary>
		''' <exception cref="NoSuchElementException"> if the Entry is null </exception>
		Friend Shared Function key(Of K, T1)(ByVal e As Entry(Of T1)) As K
			If e Is Nothing Then Throw New NoSuchElementException
			Return e.key
		End Function


		' SubMaps

		''' <summary>
		''' Dummy value serving as unmatchable fence key for unbounded
		''' SubMapIterators
		''' </summary>
		Private Shared ReadOnly UNBOUNDED As New Object

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Friend MustInherit Class NavigableSubMap(Of K, V)
			Inherits AbstractMap(Of K, V)
			Implements NavigableMap(Of K, V)

			Private Const serialVersionUID As Long = -2102997345730753016L
			''' <summary>
			''' The backing map.
			''' </summary>
			Friend ReadOnly m As TreeMap(Of K, V)

			''' <summary>
			''' Endpoints are represented as triples (fromStart, lo,
			''' loInclusive) and (toEnd, hi, hiInclusive). If fromStart is
			''' true, then the low (absolute) bound is the start of the
			''' backing map, and the other values are ignored. Otherwise,
			''' if loInclusive is true, lo is the inclusive bound, else lo
			''' is the exclusive bound. Similarly for the upper bound.
			''' </summary>
			Friend ReadOnly lo, hi As K
			Friend ReadOnly fromStart, toEnd As Boolean
			Friend ReadOnly loInclusive, hiInclusive As Boolean

			Friend Sub New(ByVal m As TreeMap(Of K, V), ByVal fromStart As Boolean, ByVal lo As K, ByVal loInclusive As Boolean, ByVal toEnd As Boolean, ByVal hi As K, ByVal hiInclusive As Boolean)
				If (Not fromStart) AndAlso (Not toEnd) Then
					If m.Compare(lo, hi) > 0 Then Throw New IllegalArgumentException("fromKey > toKey")
				Else
					If Not fromStart Then ' type check m.Compare(lo, lo)
					If Not toEnd Then m.Compare(hi, hi)
				End If

				Me.m = m
				Me.fromStart = fromStart
				Me.lo = lo
				Me.loInclusive = loInclusive
				Me.toEnd = toEnd
				Me.hi = hi
				Me.hiInclusive = hiInclusive
			End Sub

			' internal utilities

			Friend Function tooLow(ByVal key As Object) As Boolean
				If Not fromStart Then
					Dim c As Integer = m.Compare(key, lo)
					If c < 0 OrElse (c = 0 AndAlso (Not loInclusive)) Then Return True
				End If
				Return False
			End Function

			Friend Function tooHigh(ByVal key As Object) As Boolean
				If Not toEnd Then
					Dim c As Integer = m.Compare(key, hi)
					If c > 0 OrElse (c = 0 AndAlso (Not hiInclusive)) Then Return True
				End If
				Return False
			End Function

			Friend Function inRange(ByVal key As Object) As Boolean
				Return (Not tooLow(key)) AndAlso Not tooHigh(key)
			End Function

			Friend Function inClosedRange(ByVal key As Object) As Boolean
				Return (fromStart OrElse m.Compare(key, lo) >= 0) AndAlso (toEnd OrElse m.Compare(hi, key) >= 0)
			End Function

			Friend Function inRange(ByVal key As Object, ByVal inclusive As Boolean) As Boolean
				Return If(inclusive, inRange(key), inClosedRange(key))
			End Function

	'        
	'         * Absolute versions of relation operations.
	'         * Subclasses map to these using like-named "sub"
	'         * versions that invert senses for descending maps
	'         

			Friend Function absLowest() As TreeMap.Entry(Of K, V)
				Dim e As TreeMap.Entry(Of K, V) = (If(fromStart, m.firstEntry, (If(loInclusive, m.getCeilingEntry(lo), m.getHigherEntry(lo)))))
				Return If(e Is Nothing OrElse tooHigh(e.key), Nothing, e)
			End Function

			Friend Function absHighest() As TreeMap.Entry(Of K, V)
				Dim e As TreeMap.Entry(Of K, V) = (If(toEnd, m.lastEntry, (If(hiInclusive, m.getFloorEntry(hi), m.getLowerEntry(hi)))))
				Return If(e Is Nothing OrElse tooLow(e.key), Nothing, e)
			End Function

			Friend Function absCeiling(ByVal key As K) As TreeMap.Entry(Of K, V)
				If tooLow(key) Then Return absLowest()
				Dim e As TreeMap.Entry(Of K, V) = m.getCeilingEntry(key)
				Return If(e Is Nothing OrElse tooHigh(e.key), Nothing, e)
			End Function

			Friend Function absHigher(ByVal key As K) As TreeMap.Entry(Of K, V)
				If tooLow(key) Then Return absLowest()
				Dim e As TreeMap.Entry(Of K, V) = m.getHigherEntry(key)
				Return If(e Is Nothing OrElse tooHigh(e.key), Nothing, e)
			End Function

			Friend Function absFloor(ByVal key As K) As TreeMap.Entry(Of K, V)
				If tooHigh(key) Then Return absHighest()
				Dim e As TreeMap.Entry(Of K, V) = m.getFloorEntry(key)
				Return If(e Is Nothing OrElse tooLow(e.key), Nothing, e)
			End Function

			Friend Function absLower(ByVal key As K) As TreeMap.Entry(Of K, V)
				If tooHigh(key) Then Return absHighest()
				Dim e As TreeMap.Entry(Of K, V) = m.getLowerEntry(key)
				Return If(e Is Nothing OrElse tooLow(e.key), Nothing, e)
			End Function

			''' <summary>
			''' Returns the absolute high fence for ascending traversal </summary>
			Friend Function absHighFence() As TreeMap.Entry(Of K, V)
				Return (If(toEnd, Nothing, (If(hiInclusive, m.getHigherEntry(hi), m.getCeilingEntry(hi)))))
			End Function

			''' <summary>
			''' Return the absolute low fence for descending traversal </summary>
			Friend Function absLowFence() As TreeMap.Entry(Of K, V)
				Return (If(fromStart, Nothing, (If(loInclusive, m.getLowerEntry(lo), m.getFloorEntry(lo)))))
			End Function

			' Abstract methods defined in ascending vs descending classes
			' These relay to the appropriate absolute versions

			Friend MustOverride Function subLowest() As TreeMap.Entry(Of K, V)
			Friend MustOverride Function subHighest() As TreeMap.Entry(Of K, V)
			Friend MustOverride Function subCeiling(ByVal key As K) As TreeMap.Entry(Of K, V)
			Friend MustOverride Function subHigher(ByVal key As K) As TreeMap.Entry(Of K, V)
			Friend MustOverride Function subFloor(ByVal key As K) As TreeMap.Entry(Of K, V)
			Friend MustOverride Function subLower(ByVal key As K) As TreeMap.Entry(Of K, V)

			''' <summary>
			''' Returns ascending iterator from the perspective of this submap </summary>
			Friend MustOverride Function keyIterator() As [Iterator](Of K)

			Friend MustOverride Function keySpliterator() As Spliterator(Of K)

			''' <summary>
			''' Returns descending iterator from the perspective of this submap </summary>
			Friend MustOverride Function descendingKeyIterator() As [Iterator](Of K)

			' public methods

			Public Overridable Property empty As Boolean
				Get
					Return If(fromStart AndAlso toEnd, m.empty, entrySet().empty)
				End Get
			End Property

			Public Overridable Function size() As Integer
				Return If(fromStart AndAlso toEnd, m.size(), entrySet().size())
			End Function

			Public Function containsKey(ByVal key As Object) As Boolean
				Return inRange(key) AndAlso m.containsKey(key)
			End Function

			Public Function put(ByVal key As K, ByVal value As V) As V
				If Not inRange(key) Then Throw New IllegalArgumentException("key out of range")
				Return m.put(key, value)
			End Function

			Public Function [get](ByVal key As Object) As V
				Return If((Not inRange(key)), Nothing, m.get(key))
			End Function

			Public Function remove(ByVal key As Object) As V
				Return If((Not inRange(key)), Nothing, m.remove(key))
			End Function

			Public Function ceilingEntry(ByVal key As K) As KeyValuePair(Of K, V) Implements NavigableMap(Of K, V).ceilingEntry
				Return exportEntry(subCeiling(key))
			End Function

			Public Function ceilingKey(ByVal key As K) As K Implements NavigableMap(Of K, V).ceilingKey
				Return keyOrNull(subCeiling(key))
			End Function

			Public Function higherEntry(ByVal key As K) As KeyValuePair(Of K, V) Implements NavigableMap(Of K, V).higherEntry
				Return exportEntry(subHigher(key))
			End Function

			Public Function higherKey(ByVal key As K) As K Implements NavigableMap(Of K, V).higherKey
				Return keyOrNull(subHigher(key))
			End Function

			Public Function floorEntry(ByVal key As K) As KeyValuePair(Of K, V) Implements NavigableMap(Of K, V).floorEntry
				Return exportEntry(subFloor(key))
			End Function

			Public Function floorKey(ByVal key As K) As K Implements NavigableMap(Of K, V).floorKey
				Return keyOrNull(subFloor(key))
			End Function

			Public Function lowerEntry(ByVal key As K) As KeyValuePair(Of K, V) Implements NavigableMap(Of K, V).lowerEntry
				Return exportEntry(subLower(key))
			End Function

			Public Function lowerKey(ByVal key As K) As K Implements NavigableMap(Of K, V).lowerKey
				Return keyOrNull(subLower(key))
			End Function

			Public Function firstKey() As K
				Return key(subLowest())
			End Function

			Public Function lastKey() As K
				Return key(subHighest())
			End Function

			Public Function firstEntry() As KeyValuePair(Of K, V) Implements NavigableMap(Of K, V).firstEntry
				Return exportEntry(subLowest())
			End Function

			Public Function lastEntry() As KeyValuePair(Of K, V) Implements NavigableMap(Of K, V).lastEntry
				Return exportEntry(subHighest())
			End Function

			Public Function pollFirstEntry() As KeyValuePair(Of K, V) Implements NavigableMap(Of K, V).pollFirstEntry
				Dim e As TreeMap.Entry(Of K, V) = subLowest()
				Dim result As KeyValuePair(Of K, V) = exportEntry(e)
				If e IsNot Nothing Then m.deleteEntry(e)
				Return result
			End Function

			Public Function pollLastEntry() As KeyValuePair(Of K, V) Implements NavigableMap(Of K, V).pollLastEntry
				Dim e As TreeMap.Entry(Of K, V) = subHighest()
				Dim result As KeyValuePair(Of K, V) = exportEntry(e)
				If e IsNot Nothing Then m.deleteEntry(e)
				Return result
			End Function

			' Views
			<NonSerialized> _
			Friend descendingMapView As NavigableMap(Of K, V)
			<NonSerialized> _
			Friend entrySetView As EntrySetView
			<NonSerialized> _
			Friend navigableKeySetView As KeySet(Of K)

			Public Function navigableKeySet() As NavigableSet(Of K) Implements NavigableMap(Of K, V).navigableKeySet
				Dim nksv As KeySet(Of K) = navigableKeySetView
					If nksv IsNot Nothing Then
						Return nksv
					Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						Return (navigableKeySetView = New TreeMap.KeySet(Of )(Me))
					End If
			End Function

			Public Function keySet() As [Set](Of K)
				Return navigableKeySet()
			End Function

			Public Overridable Function descendingKeySet() As NavigableSet(Of K) Implements NavigableMap(Of K, V).descendingKeySet
				Return outerInstance.descendingMap().navigableKeySet()
			End Function

			Public Function subMap(ByVal fromKey As K, ByVal toKey As K) As SortedMap(Of K, V) Implements NavigableMap(Of K, V).subMap
				Return subMap(fromKey, True, toKey, False)
			End Function

			Public Function headMap(ByVal toKey As K) As SortedMap(Of K, V) Implements NavigableMap(Of K, V).headMap
				Return headMap(toKey, False)
			End Function

			Public Function tailMap(ByVal fromKey As K) As SortedMap(Of K, V) Implements NavigableMap(Of K, V).tailMap
				Return tailMap(fromKey, True)
			End Function

			' View classes

			Friend MustInherit Class EntrySetView
				Inherits AbstractSet(Of KeyValuePair(Of K, V))

				Private ReadOnly outerInstance As TreeMap.NavigableSubMap

				Public Sub New(ByVal outerInstance As TreeMap.NavigableSubMap)
					Me.outerInstance = outerInstance
				End Sub

				<NonSerialized> _
				Private size_Renamed As Integer = -1, sizeModCount As Integer

				Public Overridable Function size() As Integer
					If outerInstance.fromStart AndAlso outerInstance.toEnd Then Return outerInstance.m.size()
					If size_Renamed = -1 OrElse sizeModCount <> outerInstance.m.modCount Then
						sizeModCount = outerInstance.m.modCount
						size_Renamed = 0
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Dim i As [Iterator](Of ?) = [iterator]()
						Do While i.MoveNext()
							size_Renamed += 1
							i.Current
						Loop
					End If
					Return size_Renamed
				End Function

				Public Overridable Property empty As Boolean
					Get
						Dim n As TreeMap.Entry(Of K, V) = outerInstance.absLowest()
						Return n Is Nothing OrElse outerInstance.tooHigh(n.key)
					End Get
				End Property

				Public Overridable Function contains(ByVal o As Object) As Boolean
					If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim entry As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
					Dim key As Object = entry.Key
					If Not outerInstance.inRange(key) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim node As TreeMap.Entry(Of ?, ?) = outerInstance.m.getEntry(key)
					Return node IsNot Nothing AndAlso valEquals(node.value, entry.Value)
				End Function

				Public Overridable Function remove(ByVal o As Object) As Boolean
					If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim entry As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
					Dim key As Object = entry.Key
					If Not outerInstance.inRange(key) Then Return False
					Dim node As TreeMap.Entry(Of K, V) = outerInstance.m.getEntry(key)
					If node IsNot Nothing AndAlso valEquals(node.value, entry.Value) Then
						outerInstance.m.deleteEntry(node)
						Return True
					End If
					Return False
				End Function
			End Class

			''' <summary>
			''' Iterators for SubMaps
			''' </summary>
			Friend MustInherit Class SubMapIterator(Of T)
				Implements Iterator(Of T)

				Private ReadOnly outerInstance As TreeMap.NavigableSubMap

				Friend lastReturned As TreeMap.Entry(Of K, V)
				Friend [next] As TreeMap.Entry(Of K, V)
				Friend ReadOnly fenceKey As Object
				Friend expectedModCount As Integer

				Friend Sub New(ByVal outerInstance As TreeMap.NavigableSubMap, ByVal first As TreeMap.Entry(Of K, V), ByVal fence As TreeMap.Entry(Of K, V))
						Me.outerInstance = outerInstance
					expectedModCount = outerInstance.m.modCount
					lastReturned = Nothing
					[next] = first
					fenceKey = If(fence Is Nothing, UNBOUNDED, fence.key)
				End Sub

				Public Function hasNext() As Boolean Implements Iterator(Of T).hasNext
					Return [next] IsNot Nothing AndAlso [next].key IsNot fenceKey
				End Function

				Friend Function nextEntry() As TreeMap.Entry(Of K, V)
					Dim e As TreeMap.Entry(Of K, V) = [next]
					If e Is Nothing OrElse e.key Is fenceKey Then Throw New NoSuchElementException
					If outerInstance.m.modCount <> expectedModCount Then Throw New ConcurrentModificationException
					[next] = successor(e)
					lastReturned = e
					Return e
				End Function

				Friend Function prevEntry() As TreeMap.Entry(Of K, V)
					Dim e As TreeMap.Entry(Of K, V) = [next]
					If e Is Nothing OrElse e.key Is fenceKey Then Throw New NoSuchElementException
					If outerInstance.m.modCount <> expectedModCount Then Throw New ConcurrentModificationException
					[next] = predecessor(e)
					lastReturned = e
					Return e
				End Function

				Friend Sub removeAscending()
					If lastReturned Is Nothing Then Throw New IllegalStateException
					If outerInstance.m.modCount <> expectedModCount Then Throw New ConcurrentModificationException
					' deleted entries are replaced by their successors
					If lastReturned.left IsNot Nothing AndAlso lastReturned.right IsNot Nothing Then [next] = lastReturned
					outerInstance.m.deleteEntry(lastReturned)
					lastReturned = Nothing
					expectedModCount = outerInstance.m.modCount
				End Sub

				Friend Sub removeDescending()
					If lastReturned Is Nothing Then Throw New IllegalStateException
					If outerInstance.m.modCount <> expectedModCount Then Throw New ConcurrentModificationException
					outerInstance.m.deleteEntry(lastReturned)
					lastReturned = Nothing
					expectedModCount = outerInstance.m.modCount
				End Sub

			End Class

			Friend NotInheritable Class SubMapEntryIterator
				Inherits SubMapIterator(Of KeyValuePair(Of K, V))

				Private ReadOnly outerInstance As TreeMap.NavigableSubMap

				Friend Sub New(ByVal outerInstance As TreeMap.NavigableSubMap, ByVal first As TreeMap.Entry(Of K, V), ByVal fence As TreeMap.Entry(Of K, V))
						Me.outerInstance = outerInstance
					MyBase.New(first, fence)
				End Sub
				Public Function [next]() As KeyValuePair(Of K, V)
					Return nextEntry()
				End Function
				Public Sub remove()
					removeAscending()
				End Sub
			End Class

			Friend NotInheritable Class DescendingSubMapEntryIterator
				Inherits SubMapIterator(Of KeyValuePair(Of K, V))

				Private ReadOnly outerInstance As TreeMap.NavigableSubMap

				Friend Sub New(ByVal outerInstance As TreeMap.NavigableSubMap, ByVal last As TreeMap.Entry(Of K, V), ByVal fence As TreeMap.Entry(Of K, V))
						Me.outerInstance = outerInstance
					MyBase.New(last, fence)
				End Sub

				Public Function [next]() As KeyValuePair(Of K, V)
					Return prevEntry()
				End Function
				Public Sub remove()
					removeDescending()
				End Sub
			End Class

			' Implement minimal Spliterator as KeySpliterator backup
			Friend NotInheritable Class SubMapKeyIterator
				Inherits SubMapIterator(Of K)
				Implements Spliterator(Of K)

				Private ReadOnly outerInstance As TreeMap.NavigableSubMap

				Friend Sub New(ByVal outerInstance As TreeMap.NavigableSubMap, ByVal first As TreeMap.Entry(Of K, V), ByVal fence As TreeMap.Entry(Of K, V))
						Me.outerInstance = outerInstance
					MyBase.New(first, fence)
				End Sub
				Public Function [next]() As K
					Return nextEntry().key
				End Function
				Public Sub remove()
					removeAscending()
				End Sub
				Public Function trySplit() As Spliterator(Of K) Implements Spliterator(Of K).trySplit
					Return Nothing
				End Function
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Spliterator(Of K).forEachRemaining
					Do While hasNext()
						action.accept([next]())
					Loop
				End Sub
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of K).tryAdvance
					If hasNext() Then
						action.accept([next]())
						Return True
					End If
					Return False
				End Function
				Public Function estimateSize() As Long Implements Spliterator(Of K).estimateSize
					Return Long.MaxValue
				End Function
				Public Function characteristics() As Integer Implements Spliterator(Of K).characteristics
					Return Spliterator.DISTINCT Or Spliterator.ORDERED Or Spliterator.SORTED
				End Function
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Public Function getComparator() As Comparator(Of ?) Implements Spliterator(Of K).getComparator
					Return outerInstance.comparator()
				End Function
			End Class

			Friend NotInheritable Class DescendingSubMapKeyIterator
				Inherits SubMapIterator(Of K)
				Implements Spliterator(Of K)

				Private ReadOnly outerInstance As TreeMap.NavigableSubMap

				Friend Sub New(ByVal outerInstance As TreeMap.NavigableSubMap, ByVal last As TreeMap.Entry(Of K, V), ByVal fence As TreeMap.Entry(Of K, V))
						Me.outerInstance = outerInstance
					MyBase.New(last, fence)
				End Sub
				Public Function [next]() As K
					Return prevEntry().key
				End Function
				Public Sub remove()
					removeDescending()
				End Sub
				Public Function trySplit() As Spliterator(Of K) Implements Spliterator(Of K).trySplit
					Return Nothing
				End Function
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Spliterator(Of K).forEachRemaining
					Do While hasNext()
						action.accept([next]())
					Loop
				End Sub
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of K).tryAdvance
					If hasNext() Then
						action.accept([next]())
						Return True
					End If
					Return False
				End Function
				Public Function estimateSize() As Long Implements Spliterator(Of K).estimateSize
					Return Long.MaxValue
				End Function
				Public Function characteristics() As Integer Implements Spliterator(Of K).characteristics
					Return Spliterator.DISTINCT Or Spliterator.ORDERED
				End Function
			End Class
		End Class

		''' <summary>
		''' @serial include
		''' </summary>
		Friend NotInheritable Class AscendingSubMap(Of K, V)
			Inherits NavigableSubMap(Of K, V)

			Private Const serialVersionUID As Long = 912986545866124060L

			Friend Sub New(ByVal m As TreeMap(Of K, V), ByVal fromStart As Boolean, ByVal lo As K, ByVal loInclusive As Boolean, ByVal toEnd As Boolean, ByVal hi As K, ByVal hiInclusive As Boolean)
				MyBase.New(m, fromStart, lo, loInclusive, toEnd, hi, hiInclusive)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Function comparator() As Comparator(Of ?)
				Return m.comparator()
			End Function

			Public Function subMap(ByVal fromKey As K, ByVal fromInclusive As Boolean, ByVal toKey As K, ByVal toInclusive As Boolean) As NavigableMap(Of K, V)
				If Not inRange(fromKey, fromInclusive) Then Throw New IllegalArgumentException("fromKey out of range")
				If Not inRange(toKey, toInclusive) Then Throw New IllegalArgumentException("toKey out of range")
				Return New AscendingSubMap(Of )(m, False, fromKey, fromInclusive, False, toKey, toInclusive)
			End Function

			Public Function headMap(ByVal toKey As K, ByVal inclusive As Boolean) As NavigableMap(Of K, V)
				If Not inRange(toKey, inclusive) Then Throw New IllegalArgumentException("toKey out of range")
				Return New AscendingSubMap(Of )(m, fromStart, lo, loInclusive, False, toKey, inclusive)
			End Function

			Public Function tailMap(ByVal fromKey As K, ByVal inclusive As Boolean) As NavigableMap(Of K, V)
				If Not inRange(fromKey, inclusive) Then Throw New IllegalArgumentException("fromKey out of range")
				Return New AscendingSubMap(Of )(m, False, fromKey, inclusive, toEnd, hi, hiInclusive)
			End Function

			Public Function descendingMap() As NavigableMap(Of K, V)
				Dim mv As NavigableMap(Of K, V) = descendingMapView
					If mv IsNot Nothing Then
						Return mv
					Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						Return (descendingMapView = New DescendingSubMap(Of )(m, fromStart, lo, loInclusive, toEnd, hi, hiInclusive))
					End If
			End Function

			Friend Function keyIterator() As [Iterator](Of K)
				Return New SubMapKeyIterator(absLowest(), absHighFence())
			End Function

			Friend Function keySpliterator() As Spliterator(Of K)
				Return New SubMapKeyIterator(absLowest(), absHighFence())
			End Function

			Friend Function descendingKeyIterator() As [Iterator](Of K)
				Return New DescendingSubMapKeyIterator(absHighest(), absLowFence())
			End Function

			Friend NotInheritable Class AscendingEntrySetView
				Inherits EntrySetView

				Private ReadOnly outerInstance As TreeMap.AscendingSubMap

				Public Sub New(ByVal outerInstance As TreeMap.AscendingSubMap)
					Me.outerInstance = outerInstance
				End Sub

				Public Function [iterator]() As [Iterator](Of KeyValuePair(Of K, V))
					Return New SubMapEntryIterator(outerInstance.absLowest(), outerInstance.absHighFence())
				End Function
			End Class

			Public Function entrySet() As [Set](Of KeyValuePair(Of K, V))
				Dim es As EntrySetView = entrySetView
					If es IsNot Nothing Then
						Return es
					Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						Return (entrySetView = New AscendingEntrySetView(Me))
					End If
			End Function

			Friend Function subLowest() As TreeMap.Entry(Of K, V)
				Return absLowest()
			End Function
			Friend Function subHighest() As TreeMap.Entry(Of K, V)
				Return absHighest()
			End Function
			Friend Function subCeiling(ByVal key As K) As TreeMap.Entry(Of K, V)
				Return absCeiling(key)
			End Function
			Friend Function subHigher(ByVal key As K) As TreeMap.Entry(Of K, V)
				Return absHigher(key)
			End Function
			Friend Function subFloor(ByVal key As K) As TreeMap.Entry(Of K, V)
				Return absFloor(key)
			End Function
			Friend Function subLower(ByVal key As K) As TreeMap.Entry(Of K, V)
				Return absLower(key)
			End Function
		End Class

		''' <summary>
		''' @serial include
		''' </summary>
		Friend NotInheritable Class DescendingSubMap(Of K, V)
			Inherits NavigableSubMap(Of K, V)

			Private Const serialVersionUID As Long = 912986545866120460L
			Friend Sub New(ByVal m As TreeMap(Of K, V), ByVal fromStart As Boolean, ByVal lo As K, ByVal loInclusive As Boolean, ByVal toEnd As Boolean, ByVal hi As K, ByVal hiInclusive As Boolean)
				MyBase.New(m, fromStart, lo, loInclusive, toEnd, hi, hiInclusive)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private ReadOnly reverseComparator As Comparator(Of ?) = Collections.reverseOrder(m.comparator)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Function comparator() As Comparator(Of ?)
				Return reverseComparator
			End Function

			Public Function subMap(ByVal fromKey As K, ByVal fromInclusive As Boolean, ByVal toKey As K, ByVal toInclusive As Boolean) As NavigableMap(Of K, V)
				If Not inRange(fromKey, fromInclusive) Then Throw New IllegalArgumentException("fromKey out of range")
				If Not inRange(toKey, toInclusive) Then Throw New IllegalArgumentException("toKey out of range")
				Return New DescendingSubMap(Of )(m, False, toKey, toInclusive, False, fromKey, fromInclusive)
			End Function

			Public Function headMap(ByVal toKey As K, ByVal inclusive As Boolean) As NavigableMap(Of K, V)
				If Not inRange(toKey, inclusive) Then Throw New IllegalArgumentException("toKey out of range")
				Return New DescendingSubMap(Of )(m, False, toKey, inclusive, toEnd, hi, hiInclusive)
			End Function

			Public Function tailMap(ByVal fromKey As K, ByVal inclusive As Boolean) As NavigableMap(Of K, V)
				If Not inRange(fromKey, inclusive) Then Throw New IllegalArgumentException("fromKey out of range")
				Return New DescendingSubMap(Of )(m, fromStart, lo, loInclusive, False, fromKey, inclusive)
			End Function

			Public Function descendingMap() As NavigableMap(Of K, V)
				Dim mv As NavigableMap(Of K, V) = descendingMapView
					If mv IsNot Nothing Then
						Return mv
					Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						Return (descendingMapView = New AscendingSubMap(Of )(m, fromStart, lo, loInclusive, toEnd, hi, hiInclusive))
					End If
			End Function

			Friend Function keyIterator() As [Iterator](Of K)
				Return New DescendingSubMapKeyIterator(absHighest(), absLowFence())
			End Function

			Friend Function keySpliterator() As Spliterator(Of K)
				Return New DescendingSubMapKeyIterator(absHighest(), absLowFence())
			End Function

			Friend Function descendingKeyIterator() As [Iterator](Of K)
				Return New SubMapKeyIterator(absLowest(), absHighFence())
			End Function

			Friend NotInheritable Class DescendingEntrySetView
				Inherits EntrySetView

				Private ReadOnly outerInstance As TreeMap.DescendingSubMap

				Public Sub New(ByVal outerInstance As TreeMap.DescendingSubMap)
					Me.outerInstance = outerInstance
				End Sub

				Public Function [iterator]() As [Iterator](Of KeyValuePair(Of K, V))
					Return New DescendingSubMapEntryIterator(outerInstance.absHighest(), outerInstance.absLowFence())
				End Function
			End Class

			Public Function entrySet() As [Set](Of KeyValuePair(Of K, V))
				Dim es As EntrySetView = entrySetView
					If es IsNot Nothing Then
						Return es
					Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						Return (entrySetView = New DescendingEntrySetView(Me))
					End If
			End Function

			Friend Function subLowest() As TreeMap.Entry(Of K, V)
				Return absHighest()
			End Function
			Friend Function subHighest() As TreeMap.Entry(Of K, V)
				Return absLowest()
			End Function
			Friend Function subCeiling(ByVal key As K) As TreeMap.Entry(Of K, V)
				Return absFloor(key)
			End Function
			Friend Function subHigher(ByVal key As K) As TreeMap.Entry(Of K, V)
				Return absLower(key)
			End Function
			Friend Function subFloor(ByVal key As K) As TreeMap.Entry(Of K, V)
				Return absCeiling(key)
			End Function
			Friend Function subLower(ByVal key As K) As TreeMap.Entry(Of K, V)
				Return absHigher(key)
			End Function
		End Class

		''' <summary>
		''' This class exists solely for the sake of serialization
		''' compatibility with previous releases of TreeMap that did not
		''' support NavigableMap.  It translates an old-version SubMap into
		''' a new-version AscendingSubMap. This class is never otherwise
		''' used.
		''' 
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class SubMap
			Inherits AbstractMap(Of K, V)
			Implements SortedMap(Of K, V)

			Private ReadOnly outerInstance As TreeMap

			Public Sub New(ByVal outerInstance As TreeMap)
				Me.outerInstance = outerInstance
			End Sub

			Private Const serialVersionUID As Long = -6520786458950516097L
			Private fromStart As Boolean = False, toEnd As Boolean = False
			Private fromKey, toKey As K
			Private Function readResolve() As Object
				Return New AscendingSubMap(Of )(TreeMap.this, fromStart, fromKey, True, toEnd, toKey, False)
			End Function
			Public Overridable Function entrySet() As [Set](Of KeyValuePair(Of K, V)) Implements SortedMap(Of K, V).entrySet
				Throw New InternalError
			End Function
			Public Overridable Function lastKey() As K Implements SortedMap(Of K, V).lastKey
				Throw New InternalError
			End Function
			Public Overridable Function firstKey() As K Implements SortedMap(Of K, V).firstKey
				Throw New InternalError
			End Function
			Public Overridable Function subMap(ByVal fromKey As K, ByVal toKey As K) As SortedMap(Of K, V) Implements SortedMap(Of K, V).subMap
				Throw New InternalError
			End Function
			Public Overridable Function headMap(ByVal toKey As K) As SortedMap(Of K, V) Implements SortedMap(Of K, V).headMap
				Throw New InternalError
			End Function
			Public Overridable Function tailMap(ByVal fromKey As K) As SortedMap(Of K, V) Implements SortedMap(Of K, V).tailMap
				Throw New InternalError
			End Function
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Function comparator() As Comparator(Of ?) Implements SortedMap(Of K, V).comparator
				Throw New InternalError
			End Function
		End Class


		' Red-black mechanics

		Private Const RED As Boolean = False
		Private Const BLACK As Boolean = True

		''' <summary>
		''' Node in the Tree.  Doubles as a means to pass key-value pairs back to
		''' user (see Map.Entry).
		''' </summary>

		Friend NotInheritable Class Entry(Of K, V)
			Implements KeyValuePair(Of K, V)

			Friend key As K
			Friend value As V
			Friend left As Entry(Of K, V)
			Friend right As Entry(Of K, V)
			Friend parent As Entry(Of K, V)
			Friend color As Boolean = BLACK

			''' <summary>
			''' Make a new cell with given key, value, and parent, and with
			''' {@code null} child links, and BLACK color.
			''' </summary>
			Friend Sub New(ByVal key As K, ByVal value As V, ByVal parent As Entry(Of K, V))
				Me.key = key
				Me.value = value
				Me.parent = parent
			End Sub

			''' <summary>
			''' Returns the key.
			''' </summary>
			''' <returns> the key </returns>
			Public Property key As K
				Get
					Return key
				End Get
			End Property

			''' <summary>
			''' Returns the value associated with the key.
			''' </summary>
			''' <returns> the value associated with the key </returns>
			Public Property value As V
				Get
					Return value
				End Get
			End Property

			''' <summary>
			''' Replaces the value currently associated with the key with the given
			''' value.
			''' </summary>
			''' <returns> the value associated with the key before this method was
			'''         called </returns>
			Public Function setValue(ByVal value As V) As V
				Dim oldValue As V = Me.value
				Me.value = value
				Return oldValue
			End Function

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))

				Return valEquals(key,e.Key) AndAlso valEquals(value,e.Value)
			End Function

			Public Overrides Function GetHashCode() As Integer
				Dim keyHash As Integer = (If(key Is Nothing, 0, key.GetHashCode()))
				Dim valueHash As Integer = (If(value Is Nothing, 0, value.GetHashCode()))
				Return keyHash Xor valueHash
			End Function

			Public Overrides Function ToString() As String
				Return key & "=" & value
			End Function
		End Class

		''' <summary>
		''' Returns the first Entry in the TreeMap (according to the TreeMap's
		''' key-sort function).  Returns null if the TreeMap is empty.
		''' </summary>
		Friend Property firstEntry As Entry(Of K, V)
			Get
				Dim p As Entry(Of K, V) = root
				If p IsNot Nothing Then
					Do While p.left IsNot Nothing
						p = p.left
					Loop
				End If
				Return p
			End Get
		End Property

		''' <summary>
		''' Returns the last Entry in the TreeMap (according to the TreeMap's
		''' key-sort function).  Returns null if the TreeMap is empty.
		''' </summary>
		Friend Property lastEntry As Entry(Of K, V)
			Get
				Dim p As Entry(Of K, V) = root
				If p IsNot Nothing Then
					Do While p.right IsNot Nothing
						p = p.right
					Loop
				End If
				Return p
			End Get
		End Property

		''' <summary>
		''' Returns the successor of the specified Entry, or null if no such.
		''' </summary>
		Shared Function successor(Of K, V)(ByVal t As Entry(Of K, V)) As TreeMap.Entry(Of K, V)
			If t Is Nothing Then
				Return Nothing
			ElseIf t.right IsNot Nothing Then
				Dim p As Entry(Of K, V) = t.right
				Do While p.left IsNot Nothing
					p = p.left
				Loop
				Return p
			Else
				Dim p As Entry(Of K, V) = t.parent
				Dim ch As Entry(Of K, V) = t
				Do While p IsNot Nothing AndAlso ch Is p.right
					ch = p
					p = p.parent
				Loop
				Return p
			End If
		End Function

		''' <summary>
		''' Returns the predecessor of the specified Entry, or null if no such.
		''' </summary>
		Friend Shared Function predecessor(Of K, V)(ByVal t As Entry(Of K, V)) As Entry(Of K, V)
			If t Is Nothing Then
				Return Nothing
			ElseIf t.left IsNot Nothing Then
				Dim p As Entry(Of K, V) = t.left
				Do While p.right IsNot Nothing
					p = p.right
				Loop
				Return p
			Else
				Dim p As Entry(Of K, V) = t.parent
				Dim ch As Entry(Of K, V) = t
				Do While p IsNot Nothing AndAlso ch Is p.left
					ch = p
					p = p.parent
				Loop
				Return p
			End If
		End Function

		''' <summary>
		''' Balancing operations.
		''' 
		''' Implementations of rebalancings during insertion and deletion are
		''' slightly different than the CLR version.  Rather than using dummy
		''' nilnodes, we use a set of accessors that deal properly with null.  They
		''' are used to avoid messiness surrounding nullness checks in the main
		''' algorithms.
		''' </summary>

		Private Shared Function colorOf(Of K, V)(ByVal p As Entry(Of K, V)) As Boolean
			Return (If(p Is Nothing, BLACK, p.color))
		End Function

		Private Shared Function parentOf(Of K, V)(ByVal p As Entry(Of K, V)) As Entry(Of K, V)
			Return (If(p Is Nothing, Nothing, p.parent))
		End Function

		Private Shared Sub setColor(Of K, V)(ByVal p As Entry(Of K, V), ByVal c As Boolean)
			If p IsNot Nothing Then p.color = c
		End Sub

		Private Shared Function leftOf(Of K, V)(ByVal p As Entry(Of K, V)) As Entry(Of K, V)
			Return If(p Is Nothing, Nothing, p.left)
		End Function

		Private Shared Function rightOf(Of K, V)(ByVal p As Entry(Of K, V)) As Entry(Of K, V)
			Return If(p Is Nothing, Nothing, p.right)
		End Function

		''' <summary>
		''' From CLR </summary>
		Private Sub rotateLeft(ByVal p As Entry(Of K, V))
			If p IsNot Nothing Then
				Dim r As Entry(Of K, V) = p.right
				p.right = r.left
				If r.left IsNot Nothing Then r.left.parent = p
				r.parent = p.parent
				If p.parent Is Nothing Then
					root = r
				ElseIf p.parent.left Is p Then
					p.parent.left = r
				Else
					p.parent.right = r
				End If
				r.left = p
				p.parent = r
			End If
		End Sub

		''' <summary>
		''' From CLR </summary>
		Private Sub rotateRight(ByVal p As Entry(Of K, V))
			If p IsNot Nothing Then
				Dim l As Entry(Of K, V) = p.left
				p.left = l.right
				If l.right IsNot Nothing Then l.right.parent = p
				l.parent = p.parent
				If p.parent Is Nothing Then
					root = l
				ElseIf p.parent.right Is p Then
					p.parent.right = l
				Else
					p.parent.left = l
				End If
				l.right = p
				p.parent = l
			End If
		End Sub

		''' <summary>
		''' From CLR </summary>
		Private Sub fixAfterInsertion(ByVal x As Entry(Of K, V))
			x.color = RED

			Do While x IsNot Nothing AndAlso x IsNot root AndAlso x.parent.color = RED
				If parentOf(x) Is leftOf(parentOf(parentOf(x))) Then
					Dim y As Entry(Of K, V) = rightOf(parentOf(parentOf(x)))
					If colorOf(y) = RED Then
						colorlor(parentOf(x), BLACK)
						colorlor(y, BLACK)
						colorlor(parentOf(parentOf(x)), RED)
						x = parentOf(parentOf(x))
					Else
						If x Is rightOf(parentOf(x)) Then
							x = parentOf(x)
							rotateLeft(x)
						End If
						colorlor(parentOf(x), BLACK)
						colorlor(parentOf(parentOf(x)), RED)
						rotateRight(parentOf(parentOf(x)))
					End If
				Else
					Dim y As Entry(Of K, V) = leftOf(parentOf(parentOf(x)))
					If colorOf(y) = RED Then
						colorlor(parentOf(x), BLACK)
						colorlor(y, BLACK)
						colorlor(parentOf(parentOf(x)), RED)
						x = parentOf(parentOf(x))
					Else
						If x Is leftOf(parentOf(x)) Then
							x = parentOf(x)
							rotateRight(x)
						End If
						colorlor(parentOf(x), BLACK)
						colorlor(parentOf(parentOf(x)), RED)
						rotateLeft(parentOf(parentOf(x)))
					End If
				End If
			Loop
			root.color = BLACK
		End Sub

		''' <summary>
		''' Delete node p, and then rebalance the tree.
		''' </summary>
		Private Sub deleteEntry(ByVal p As Entry(Of K, V))
			modCount += 1
			size_Renamed -= 1

			' If strictly internal, copy successor's element to p and then make p
			' point to successor.
			If p.left IsNot Nothing AndAlso p.right IsNot Nothing Then
				Dim s As Entry(Of K, V) = successor(p)
				p.key = s.key
				p.value = s.value
				p = s
			End If ' p has 2 children

			' Start fixup at replacement node, if it exists.
			Dim replacement As Entry(Of K, V) = (If(p.left IsNot Nothing, p.left, p.right))

			If replacement IsNot Nothing Then
				' Link replacement to parent
				replacement.parent = p.parent
				If p.parent Is Nothing Then
					root = replacement
				ElseIf p Is p.parent.left Then
					p.parent.left = replacement
				Else
					p.parent.right = replacement
				End If

				' Null out links so they are OK to use by fixAfterDeletion.
					p.parent = Nothing
						p.right = p.parent
						p.left = p.right

				' Fix replacement
				If p.color = BLACK Then
					fixAfterDeletion(replacement)
				End If ' return if we are the only node.
			ElseIf p.parent Is Nothing Then
				root = Nothing '  No children. Use self as phantom replacement and unlink.
			Else
				If p.color = BLACK Then fixAfterDeletion(p)

				If p.parent IsNot Nothing Then
					If p Is p.parent.left Then
						p.parent.left = Nothing
					ElseIf p Is p.parent.right Then
						p.parent.right = Nothing
					End If
					p.parent = Nothing
				End If
			End If
		End Sub

		''' <summary>
		''' From CLR </summary>
		Private Sub fixAfterDeletion(ByVal x As Entry(Of K, V))
			Do While x IsNot root AndAlso colorOf(x) = BLACK
				If x Is leftOf(parentOf(x)) Then
					Dim sib As Entry(Of K, V) = rightOf(parentOf(x))

					If colorOf(sib) = RED Then
						colorlor(sib, BLACK)
						colorlor(parentOf(x), RED)
						rotateLeft(parentOf(x))
						sib = rightOf(parentOf(x))
					End If

					If colorOf(leftOf(sib)) = BLACK AndAlso colorOf(rightOf(sib)) = BLACK Then
						colorlor(sib, RED)
						x = parentOf(x)
					Else
						If colorOf(rightOf(sib)) = BLACK Then
							colorlor(leftOf(sib), BLACK)
							colorlor(sib, RED)
							rotateRight(sib)
							sib = rightOf(parentOf(x))
						End If
						colorlor(sib, colorOf(parentOf(x)))
						colorlor(parentOf(x), BLACK)
						colorlor(rightOf(sib), BLACK)
						rotateLeft(parentOf(x))
						x = root
					End If ' symmetric
				Else
					Dim sib As Entry(Of K, V) = leftOf(parentOf(x))

					If colorOf(sib) = RED Then
						colorlor(sib, BLACK)
						colorlor(parentOf(x), RED)
						rotateRight(parentOf(x))
						sib = leftOf(parentOf(x))
					End If

					If colorOf(rightOf(sib)) = BLACK AndAlso colorOf(leftOf(sib)) = BLACK Then
						colorlor(sib, RED)
						x = parentOf(x)
					Else
						If colorOf(leftOf(sib)) = BLACK Then
							colorlor(rightOf(sib), BLACK)
							colorlor(sib, RED)
							rotateLeft(sib)
							sib = leftOf(parentOf(x))
						End If
						colorlor(sib, colorOf(parentOf(x)))
						colorlor(parentOf(x), BLACK)
						colorlor(leftOf(sib), BLACK)
						rotateRight(parentOf(x))
						x = root
					End If
				End If
			Loop

			colorlor(x, BLACK)
		End Sub

		Private Const serialVersionUID As Long = 919286545866124006L

		''' <summary>
		''' Save the state of the {@code TreeMap} instance to a stream (i.e.,
		''' serialize it).
		''' 
		''' @serialData The <em>size</em> of the TreeMap (the number of key-value
		'''             mappings) is emitted (int), followed by the key (Object)
		'''             and value (Object) for each key-value mapping represented
		'''             by the TreeMap. The key-value mappings are emitted in
		'''             key-order (as determined by the TreeMap's Comparator,
		'''             or by the keys' natural ordering if the TreeMap has no
		'''             Comparator).
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			' Write out the Comparator and any hidden stuff
			s.defaultWriteObject()

			' Write out size (number of Mappings)
			s.writeInt(size_Renamed)

			' Write out keys and values (alternating)
			Dim i As [Iterator](Of KeyValuePair(Of K, V)) = entrySet().GetEnumerator()
			Do While i.MoveNext()
				Dim e As KeyValuePair(Of K, V) = i.Current
				s.writeObject(e.Key)
				s.writeObject(e.Value)
			Loop
		End Sub

		''' <summary>
		''' Reconstitute the {@code TreeMap} instance from a stream (i.e.,
		''' deserialize it).
		''' </summary>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			' Read in the Comparator and any hidden stuff
			s.defaultReadObject()

			' Read in size
			Dim size As Integer = s.readInt()

			buildFromSorted(size, Nothing, s, Nothing)
		End Sub

		''' <summary>
		''' Intended to be called only from TreeSet.readObject </summary>
		Friend Overridable Sub readTreeSet(ByVal size As Integer, ByVal s As java.io.ObjectInputStream, ByVal defaultVal As V)
			buildFromSorted(size, Nothing, s, defaultVal)
		End Sub

		''' <summary>
		''' Intended to be called only from TreeSet.addAll </summary>
		Friend Overridable Sub addAllForTreeSet(Of T1 As K)(ByVal [set] As SortedSet(Of T1), ByVal defaultVal As V)
			Try
				buildFromSorted([set].size(), [set].GetEnumerator(), Nothing, defaultVal)
			Catch cannotHappen As java.io.IOException
			Catch cannotHappen As ClassNotFoundException
			End Try
		End Sub


		''' <summary>
		''' Linear time tree building algorithm from sorted data.  Can accept keys
		''' and/or values from iterator or stream. This leads to too many
		''' parameters, but seems better than alternatives.  The four formats
		''' that this method accepts are:
		''' 
		'''    1) An iterator of Map.Entries.  (it != null, defaultVal == null).
		'''    2) An iterator of keys.         (it != null, defaultVal != null).
		'''    3) A stream of alternating serialized keys and values.
		'''                                   (it == null, defaultVal == null).
		'''    4) A stream of serialized keys. (it == null, defaultVal != null).
		''' 
		''' It is assumed that the comparator of the TreeMap is already set prior
		''' to calling this method.
		''' </summary>
		''' <param name="size"> the number of keys (or key-value pairs) to be read from
		'''        the iterator or stream </param>
		''' <param name="it"> If non-null, new entries are created from entries
		'''        or keys read from this iterator. </param>
		''' <param name="str"> If non-null, new entries are created from keys and
		'''        possibly values read from this stream in serialized form.
		'''        Exactly one of it and str should be non-null. </param>
		''' <param name="defaultVal"> if non-null, this default value is used for
		'''        each value in the map.  If null, each value is read from
		'''        iterator or stream, as described above. </param>
		''' <exception cref="java.io.IOException"> propagated from stream reads. This cannot
		'''         occur if str is null. </exception>
		''' <exception cref="ClassNotFoundException"> propagated from readObject.
		'''         This cannot occur if str is null. </exception>
		Private Sub buildFromSorted(Of T1)(ByVal size As Integer, ByVal it As [Iterator](Of T1), ByVal str As java.io.ObjectInputStream, ByVal defaultVal As V)
			Me.size_Renamed = size
			root = buildFromSorted(0, 0, size-1, computeRedLevel(size), it, str, defaultVal)
		End Sub

		''' <summary>
		''' Recursive "helper method" that does the real work of the
		''' previous method.  Identically named parameters have
		''' identical definitions.  Additional parameters are documented below.
		''' It is assumed that the comparator and size fields of the TreeMap are
		''' already set prior to calling this method.  (It ignores both fields.)
		''' </summary>
		''' <param name="level"> the current level of tree. Initial call should be 0. </param>
		''' <param name="lo"> the first element index of this subtree. Initial should be 0. </param>
		''' <param name="hi"> the last element index of this subtree.  Initial should be
		'''        size-1. </param>
		''' <param name="redLevel"> the level at which nodes should be red.
		'''        Must be equal to computeRedLevel for tree of this size. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Function buildFromSorted(Of T1)(ByVal level As Integer, ByVal lo As Integer, ByVal hi As Integer, ByVal redLevel As Integer, ByVal it As [Iterator](Of T1), ByVal str As java.io.ObjectInputStream, ByVal defaultVal As V) As Entry(Of K, V)
	'        
	'         * Strategy: The root is the middlemost element. To get to it, we
	'         * have to first recursively construct the entire left subtree,
	'         * so as to grab all of its elements. We can then proceed with right
	'         * subtree.
	'         *
	'         * The lo and hi arguments are the minimum and maximum
	'         * indices to pull out of the iterator or stream for current subtree.
	'         * They are not actually indexed, we just proceed sequentially,
	'         * ensuring that items are extracted in corresponding order.
	'         

			If hi < lo Then Return Nothing

			Dim mid As Integer = CInt(CUInt((lo + hi)) >> 1)

			Dim left As Entry(Of K, V) = Nothing
			If lo < mid Then left = buildFromSorted(level+1, lo, mid - 1, redLevel, it, str, defaultVal)

			' extract key and/or value from iterator or stream
			Dim key As K
			Dim value As V
			If it IsNot Nothing Then
				If defaultVal Is Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim entry_Renamed As KeyValuePair(Of ?, ?) = CType(it.next(), KeyValuePair(Of ?, ?))
					key = CType(entry_Renamed.Key, K)
					value = CType(entry_Renamed.Value, V)
				Else
					key = CType(it.next(), K)
					value = defaultVal
				End If ' use stream
			Else
				key = CType(str.readObject(), K)
				value = (If(defaultVal IsNot Nothing, defaultVal, CType(str.readObject(), V)))
			End If

			Dim middle As New Entry(Of K, V)(key, value, Nothing)

			' color nodes in non-full bottommost level red
			If level = redLevel Then middle.color = RED

			If left IsNot Nothing Then
				middle.left = left
				left.parent = middle
			End If

			If mid < hi Then
				Dim right As Entry(Of K, V) = buildFromSorted(level+1, mid+1, hi, redLevel, it, str, defaultVal)
				middle.right = right
				right.parent = middle
			End If

			Return middle
		End Function

		''' <summary>
		''' Find the level down to which to assign all nodes BLACK.  This is the
		''' last `full' level of the complete binary tree produced by
		''' buildTree. The remaining nodes are colored RED. (This makes a `nice'
		''' set of color assignments wrt future insertions.) This level number is
		''' computed by finding the number of splits needed to reach the zeroeth
		''' node.  (The answer is ~lg(N), but in any case must be computed by same
		''' quick O(lg(N)) loop.)
		''' </summary>
		Private Shared Function computeRedLevel(ByVal sz As Integer) As Integer
			Dim level As Integer = 0
			Dim m As Integer = sz - 1
			Do While m >= 0
				level += 1
				m = m \ 2 - 1
			Loop
			Return level
		End Function

		''' <summary>
		''' Currently, we support Spliterator-based versions only for the
		''' full map, in either plain of descending form, otherwise relying
		''' on defaults because size estimation for submaps would dominate
		''' costs. The type tests needed to check these for key views are
		''' not very nice but avoid disrupting existing class
		''' structures. Callers must use plain default spliterators if this
		''' returns null.
		''' </summary>
		Friend Shared Function keySpliteratorFor(Of K, T1)(ByVal m As NavigableMap(Of T1)) As Spliterator(Of K)
			If TypeOf m Is TreeMap Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim t As TreeMap(Of K, Object) = CType(m, TreeMap(Of K, Object))
				Return t.keySpliterator()
			End If
			If TypeOf m Is DescendingSubMap Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim dm As DescendingSubMap(Of K, ?) = CType(m, DescendingSubMap(Of K, ?))
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim tm As TreeMap(Of K, ?) = dm.m
				If dm Is tm.descendingMap_Renamed Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim t As TreeMap(Of K, Object) = CType(tm, TreeMap(Of K, Object))
					Return t.descendingKeySpliterator()
				End If
			End If
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim sm As NavigableSubMap(Of K, ?) = CType(m, NavigableSubMap(Of K, ?))
			Return sm.keySpliterator()
		End Function

		Friend Function keySpliterator() As Spliterator(Of K)
			Return New KeySpliterator(Of K, V)(Me, Nothing, Nothing, 0, -1, 0)
		End Function

		Friend Function descendingKeySpliterator() As Spliterator(Of K)
			Return New DescendingKeySpliterator(Of K, V)(Me, Nothing, Nothing, 0, -2, 0)
		End Function

		''' <summary>
		''' Base class for spliterators.  Iteration starts at a given
		''' origin and continues up to but not including a given fence (or
		''' null for end).  At top-level, for ascending cases, the first
		''' split uses the root as left-fence/right-origin. From there,
		''' right-hand splits replace the current fence with its left
		''' child, also serving as origin for the split-off spliterator.
		''' Left-hands are symmetric. Descending versions place the origin
		''' at the end and invert ascending split rules.  This base class
		''' is non-commital about directionality, or whether the top-level
		''' spliterator covers the whole tree. This means that the actual
		''' split mechanics are located in subclasses. Some of the subclass
		''' trySplit methods are identical (except for return types), but
		''' not nicely factorable.
		''' 
		''' Currently, subclass versions exist only for the full map
		''' (including descending keys via its descendingMap).  Others are
		''' possible but currently not worthwhile because submaps require
		''' O(n) computations to determine size, which substantially limits
		''' potential speed-ups of using custom Spliterators versus default
		''' mechanics.
		''' 
		''' To boostrap initialization, external constructors use
		''' negative size estimates: -1 for ascend, -2 for descend.
		''' </summary>
		Friend Class TreeMapSpliterator(Of K, V)
			Friend ReadOnly tree As TreeMap(Of K, V)
			Friend current As TreeMap.Entry(Of K, V) ' traverser; initially first node in range
			Friend fence As TreeMap.Entry(Of K, V) ' one past last, or null
			Friend side As Integer ' 0: top, -1: is a left split, +1: right
			Friend est As Integer ' size estimate (exact only for top-level)
			Friend expectedModCount As Integer ' for CME checks

			Friend Sub New(ByVal tree As TreeMap(Of K, V), ByVal origin As TreeMap.Entry(Of K, V), ByVal fence As TreeMap.Entry(Of K, V), ByVal side As Integer, ByVal est As Integer, ByVal expectedModCount As Integer)
				Me.tree = tree
				Me.current = origin
				Me.fence = fence
				Me.side = side
				Me.est = est
				Me.expectedModCount = expectedModCount
			End Sub

			Friend Property estimate As Integer
				Get
					Dim s As Integer
					Dim t As TreeMap(Of K, V)
					s = est
					If s < 0 Then
						t = tree
						If t IsNot Nothing Then
							current = If(s = -1, t.firstEntry, t.lastEntry)
								est = t.size
								s = est
							expectedModCount = t.modCount
						Else
								est = 0
								s = est
						End If
					End If
					Return s
				End Get
			End Property

			Public Function estimateSize() As Long
				Return CLng(estimate)
			End Function
		End Class

		Friend NotInheritable Class KeySpliterator(Of K, V)
			Inherits TreeMapSpliterator(Of K, V)
			Implements Spliterator(Of K)

			Friend Sub New(ByVal tree As TreeMap(Of K, V), ByVal origin As TreeMap.Entry(Of K, V), ByVal fence As TreeMap.Entry(Of K, V), ByVal side As Integer, ByVal est As Integer, ByVal expectedModCount As Integer)
				MyBase.New(tree, origin, fence, side, est, expectedModCount)
			End Sub

			Public Function trySplit() As KeySpliterator(Of K, V)
				If est < 0 Then estimate ' force initialization
				Dim d As Integer = side
				Dim e As TreeMap.Entry(Of K, V) = current, f As TreeMap.Entry(Of K, V) = fence, s As TreeMap.Entry(Of K, V) = (If(e Is Nothing OrElse e Is f, Nothing, If(d = 0, tree.root, If(d > 0, e.right, If(d < 0 AndAlso f IsNot Nothing, f.left, Nothing))))) ' was left -  was right -  was top -  empty
				If s IsNot Nothing AndAlso s IsNot e AndAlso s IsNot f AndAlso tree.Compare(e.key, s.key) < 0 Then ' e not already past s
					side = 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return New KeySpliterator(Of ) (tree, e, current = s, -1, est >>>= 1, expectedModCount)
				End If
				Return Nothing
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Spliterator(Of K).forEachRemaining
				If action Is Nothing Then Throw New NullPointerException
				If est < 0 Then estimate ' force initialization
				Dim f As TreeMap.Entry(Of K, V) = fence, e As TreeMap.Entry(Of K, V), p As TreeMap.Entry(Of K, V), pl As TreeMap.Entry(Of K, V)
				e = current
				If e IsNot Nothing AndAlso e IsNot f Then
					current = f ' exhaust
					Do
						action.accept(e.key)
						p = e.right
						If p IsNot Nothing Then
							pl = p.left
							Do While pl IsNot Nothing
								p = pl
								pl = p.left
							Loop
						Else
							p = e.parent
							Do While p IsNot Nothing AndAlso e Is p.right
								e = p
								p = e.parent
							Loop
						End If
						e = p
					Loop While e IsNot Nothing AndAlso e IsNot f
					If tree.modCount <> expectedModCount Then Throw New ConcurrentModificationException
				End If
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of K).tryAdvance
				Dim e As TreeMap.Entry(Of K, V)
				If action Is Nothing Then Throw New NullPointerException
				If est < 0 Then estimate ' force initialization
				e = current
				If e Is Nothing OrElse e Is fence Then Return False
				current = successor(e)
				action.accept(e.key)
				If tree.modCount <> expectedModCount Then Throw New ConcurrentModificationException
				Return True
			End Function

			Public Function characteristics() As Integer Implements Spliterator(Of K).characteristics
				Return (If(side = 0, Spliterator.SIZED, 0)) Or Spliterator.DISTINCT Or Spliterator.SORTED Or Spliterator.ORDERED
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Function getComparator() As Comparator(Of ?) Implements Spliterator(Of K).getComparator
				Return tree.comparator
			End Function

		End Class

		Friend NotInheritable Class DescendingKeySpliterator(Of K, V)
			Inherits TreeMapSpliterator(Of K, V)
			Implements Spliterator(Of K)

			Friend Sub New(ByVal tree As TreeMap(Of K, V), ByVal origin As TreeMap.Entry(Of K, V), ByVal fence As TreeMap.Entry(Of K, V), ByVal side As Integer, ByVal est As Integer, ByVal expectedModCount As Integer)
				MyBase.New(tree, origin, fence, side, est, expectedModCount)
			End Sub

			Public Function trySplit() As DescendingKeySpliterator(Of K, V)
				If est < 0 Then estimate ' force initialization
				Dim d As Integer = side
				Dim e As TreeMap.Entry(Of K, V) = current, f As TreeMap.Entry(Of K, V) = fence, s As TreeMap.Entry(Of K, V) = (If(e Is Nothing OrElse e Is f, Nothing, If(d = 0, tree.root, If(d < 0, e.left, If(d > 0 AndAlso f IsNot Nothing, f.right, Nothing))))) ' was right -  was left -  was top -  empty
				If s IsNot Nothing AndAlso s IsNot e AndAlso s IsNot f AndAlso tree.Compare(e.key, s.key) > 0 Then ' e not already past s
					side = 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return New DescendingKeySpliterator(Of ) (tree, e, current = s, -1, est >>>= 1, expectedModCount)
				End If
				Return Nothing
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Spliterator(Of K).forEachRemaining
				If action Is Nothing Then Throw New NullPointerException
				If est < 0 Then estimate ' force initialization
				Dim f As TreeMap.Entry(Of K, V) = fence, e As TreeMap.Entry(Of K, V), p As TreeMap.Entry(Of K, V), pr As TreeMap.Entry(Of K, V)
				e = current
				If e IsNot Nothing AndAlso e IsNot f Then
					current = f ' exhaust
					Do
						action.accept(e.key)
						p = e.left
						If p IsNot Nothing Then
							pr = p.right
							Do While pr IsNot Nothing
								p = pr
								pr = p.right
							Loop
						Else
							p = e.parent
							Do While p IsNot Nothing AndAlso e Is p.left
								e = p
								p = e.parent
							Loop
						End If
						e = p
					Loop While e IsNot Nothing AndAlso e IsNot f
					If tree.modCount <> expectedModCount Then Throw New ConcurrentModificationException
				End If
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of K).tryAdvance
				Dim e As TreeMap.Entry(Of K, V)
				If action Is Nothing Then Throw New NullPointerException
				If est < 0 Then estimate ' force initialization
				e = current
				If e Is Nothing OrElse e Is fence Then Return False
				current = predecessor(e)
				action.accept(e.key)
				If tree.modCount <> expectedModCount Then Throw New ConcurrentModificationException
				Return True
			End Function

			Public Function characteristics() As Integer Implements Spliterator(Of K).characteristics
				Return (If(side = 0, Spliterator.SIZED, 0)) Or Spliterator.DISTINCT Or Spliterator.ORDERED
			End Function
		End Class

		Friend NotInheritable Class ValueSpliterator(Of K, V)
			Inherits TreeMapSpliterator(Of K, V)
			Implements Spliterator(Of V)

			Friend Sub New(ByVal tree As TreeMap(Of K, V), ByVal origin As TreeMap.Entry(Of K, V), ByVal fence As TreeMap.Entry(Of K, V), ByVal side As Integer, ByVal est As Integer, ByVal expectedModCount As Integer)
				MyBase.New(tree, origin, fence, side, est, expectedModCount)
			End Sub

			Public Function trySplit() As ValueSpliterator(Of K, V)
				If est < 0 Then estimate ' force initialization
				Dim d As Integer = side
				Dim e As TreeMap.Entry(Of K, V) = current, f As TreeMap.Entry(Of K, V) = fence, s As TreeMap.Entry(Of K, V) = (If(e Is Nothing OrElse e Is f, Nothing, If(d = 0, tree.root, If(d > 0, e.right, If(d < 0 AndAlso f IsNot Nothing, f.left, Nothing))))) ' was left -  was right -  was top -  empty
				If s IsNot Nothing AndAlso s IsNot e AndAlso s IsNot f AndAlso tree.Compare(e.key, s.key) < 0 Then ' e not already past s
					side = 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return New ValueSpliterator(Of ) (tree, e, current = s, -1, est >>>= 1, expectedModCount)
				End If
				Return Nothing
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Spliterator(Of V).forEachRemaining
				If action Is Nothing Then Throw New NullPointerException
				If est < 0 Then estimate ' force initialization
				Dim f As TreeMap.Entry(Of K, V) = fence, e As TreeMap.Entry(Of K, V), p As TreeMap.Entry(Of K, V), pl As TreeMap.Entry(Of K, V)
				e = current
				If e IsNot Nothing AndAlso e IsNot f Then
					current = f ' exhaust
					Do
						action.accept(e.value)
						p = e.right
						If p IsNot Nothing Then
							pl = p.left
							Do While pl IsNot Nothing
								p = pl
								pl = p.left
							Loop
						Else
							p = e.parent
							Do While p IsNot Nothing AndAlso e Is p.right
								e = p
								p = e.parent
							Loop
						End If
						e = p
					Loop While e IsNot Nothing AndAlso e IsNot f
					If tree.modCount <> expectedModCount Then Throw New ConcurrentModificationException
				End If
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of V).tryAdvance
				Dim e As TreeMap.Entry(Of K, V)
				If action Is Nothing Then Throw New NullPointerException
				If est < 0 Then estimate ' force initialization
				e = current
				If e Is Nothing OrElse e Is fence Then Return False
				current = successor(e)
				action.accept(e.value)
				If tree.modCount <> expectedModCount Then Throw New ConcurrentModificationException
				Return True
			End Function

			Public Function characteristics() As Integer Implements Spliterator(Of V).characteristics
				Return (If(side = 0, Spliterator.SIZED, 0)) Or Spliterator.ORDERED
			End Function
		End Class

		Friend NotInheritable Class EntrySpliterator(Of K, V)
			Inherits TreeMapSpliterator(Of K, V)
			Implements Spliterator(Of KeyValuePair(Of K, V))

			Friend Sub New(ByVal tree As TreeMap(Of K, V), ByVal origin As TreeMap.Entry(Of K, V), ByVal fence As TreeMap.Entry(Of K, V), ByVal side As Integer, ByVal est As Integer, ByVal expectedModCount As Integer)
				MyBase.New(tree, origin, fence, side, est, expectedModCount)
			End Sub

			Public Function trySplit() As EntrySpliterator(Of K, V)
				If est < 0 Then estimate ' force initialization
				Dim d As Integer = side
				Dim e As TreeMap.Entry(Of K, V) = current, f As TreeMap.Entry(Of K, V) = fence, s As TreeMap.Entry(Of K, V) = (If(e Is Nothing OrElse e Is f, Nothing, If(d = 0, tree.root, If(d > 0, e.right, If(d < 0 AndAlso f IsNot Nothing, f.left, Nothing))))) ' was left -  was right -  was top -  empty
				If s IsNot Nothing AndAlso s IsNot e AndAlso s IsNot f AndAlso tree.Compare(e.key, s.key) < 0 Then ' e not already past s
					side = 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return New EntrySpliterator(Of ) (tree, e, current = s, -1, est >>>= 1, expectedModCount)
				End If
				Return Nothing
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Spliterator(Of KeyValuePair(Of K, V)).forEachRemaining
				If action Is Nothing Then Throw New NullPointerException
				If est < 0 Then estimate ' force initialization
				Dim f As TreeMap.Entry(Of K, V) = fence, e As TreeMap.Entry(Of K, V), p As TreeMap.Entry(Of K, V), pl As TreeMap.Entry(Of K, V)
				e = current
				If e IsNot Nothing AndAlso e IsNot f Then
					current = f ' exhaust
					Do
						action.accept(e)
						p = e.right
						If p IsNot Nothing Then
							pl = p.left
							Do While pl IsNot Nothing
								p = pl
								pl = p.left
							Loop
						Else
							p = e.parent
							Do While p IsNot Nothing AndAlso e Is p.right
								e = p
								p = e.parent
							Loop
						End If
						e = p
					Loop While e IsNot Nothing AndAlso e IsNot f
					If tree.modCount <> expectedModCount Then Throw New ConcurrentModificationException
				End If
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of KeyValuePair(Of K, V)).tryAdvance
				Dim e As TreeMap.Entry(Of K, V)
				If action Is Nothing Then Throw New NullPointerException
				If est < 0 Then estimate ' force initialization
				e = current
				If e Is Nothing OrElse e Is fence Then Return False
				current = successor(e)
				action.accept(e)
				If tree.modCount <> expectedModCount Then Throw New ConcurrentModificationException
				Return True
			End Function

			Public Function characteristics() As Integer Implements Spliterator(Of KeyValuePair(Of K, V)).characteristics
				Return (If(side = 0, Spliterator.SIZED, 0)) Or Spliterator.DISTINCT Or Spliterator.SORTED Or Spliterator.ORDERED
			End Function

			Public Overrides Function getComparator() As Comparator(Of KeyValuePair(Of K, V)) Implements Spliterator(Of KeyValuePair(Of K, V)).getComparator
				' Adapt or create a key-based comparator
				If tree.comparator IsNot Nothing Then
					Return DictionaryEntry.comparingByKey(tree.comparator)
				Else
					Return (Comparator(Of KeyValuePair(Of K, V)) And java.io.Serializable)(e1, e2) ->
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Dim k1 As Comparable(Of ?) = CType(e1.key, Comparable(Of ?))
						Return k1.CompareTo(e2.key)
				End If
			End Function
		End Class
	End Class

End Namespace
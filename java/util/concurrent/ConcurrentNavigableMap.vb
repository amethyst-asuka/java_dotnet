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
	''' A <seealso cref="ConcurrentMap"/> supporting <seealso cref="NavigableMap"/> operations,
	''' and recursively so for its navigable sub-maps.
	''' 
	''' <p>This interface is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @author Doug Lea </summary>
	''' @param <K> the type of keys maintained by this map </param>
	''' @param <V> the type of mapped values
	''' @since 1.6 </param>
	Public Interface ConcurrentNavigableMap(Of K, V)
		Inherits ConcurrentMap(Of K, V), NavigableMap(Of K, V)

		''' <exception cref="ClassCastException">       {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">     {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Function subMap(ByVal fromKey As K, ByVal fromInclusive As Boolean, ByVal toKey As K, ByVal toInclusive As Boolean) As ConcurrentNavigableMap(Of K, V)

		''' <exception cref="ClassCastException">       {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">     {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Function headMap(ByVal toKey As K, ByVal inclusive As Boolean) As ConcurrentNavigableMap(Of K, V)

		''' <exception cref="ClassCastException">       {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">     {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Function tailMap(ByVal fromKey As K, ByVal inclusive As Boolean) As ConcurrentNavigableMap(Of K, V)

		''' <exception cref="ClassCastException">       {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">     {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Function subMap(ByVal fromKey As K, ByVal toKey As K) As ConcurrentNavigableMap(Of K, V)

		''' <exception cref="ClassCastException">       {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">     {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Function headMap(ByVal toKey As K) As ConcurrentNavigableMap(Of K, V)

		''' <exception cref="ClassCastException">       {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">     {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Function tailMap(ByVal fromKey As K) As ConcurrentNavigableMap(Of K, V)

		''' <summary>
		''' Returns a reverse order view of the mappings contained in this map.
		''' The descending map is backed by this map, so changes to the map are
		''' reflected in the descending map, and vice-versa.
		''' 
		''' <p>The returned map has an ordering equivalent to
		''' <seealso cref="Collections#reverseOrder(Comparator) Collections.reverseOrder"/>{@code (comparator())}.
		''' The expression {@code m.descendingMap().descendingMap()} returns a
		''' view of {@code m} essentially equivalent to {@code m}.
		''' </summary>
		''' <returns> a reverse order view of this map </returns>
		Function descendingMap() As ConcurrentNavigableMap(Of K, V)

		''' <summary>
		''' Returns a <seealso cref="NavigableSet"/> view of the keys contained in this map.
		''' The set's iterator returns the keys in ascending order.
		''' The set is backed by the map, so changes to the map are
		''' reflected in the set, and vice-versa.  The set supports element
		''' removal, which removes the corresponding mapping from the map,
		''' via the {@code Iterator.remove}, {@code Set.remove},
		''' {@code removeAll}, {@code retainAll}, and {@code clear}
		''' operations.  It does not support the {@code add} or {@code addAll}
		''' operations.
		''' 
		''' <p>The view's iterators and spliterators are
		''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		''' </summary>
		''' <returns> a navigable set view of the keys in this map </returns>
		Function navigableKeySet() As NavigableSet(Of K)

		''' <summary>
		''' Returns a <seealso cref="NavigableSet"/> view of the keys contained in this map.
		''' The set's iterator returns the keys in ascending order.
		''' The set is backed by the map, so changes to the map are
		''' reflected in the set, and vice-versa.  The set supports element
		''' removal, which removes the corresponding mapping from the map,
		''' via the {@code Iterator.remove}, {@code Set.remove},
		''' {@code removeAll}, {@code retainAll}, and {@code clear}
		''' operations.  It does not support the {@code add} or {@code addAll}
		''' operations.
		''' 
		''' <p>The view's iterators and spliterators are
		''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		''' 
		''' <p>This method is equivalent to method {@code navigableKeySet}.
		''' </summary>
		''' <returns> a navigable set view of the keys in this map </returns>
		Function keySet() As NavigableSet(Of K)

		''' <summary>
		''' Returns a reverse order <seealso cref="NavigableSet"/> view of the keys contained in this map.
		''' The set's iterator returns the keys in descending order.
		''' The set is backed by the map, so changes to the map are
		''' reflected in the set, and vice-versa.  The set supports element
		''' removal, which removes the corresponding mapping from the map,
		''' via the {@code Iterator.remove}, {@code Set.remove},
		''' {@code removeAll}, {@code retainAll}, and {@code clear}
		''' operations.  It does not support the {@code add} or {@code addAll}
		''' operations.
		''' 
		''' <p>The view's iterators and spliterators are
		''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		''' </summary>
		''' <returns> a reverse order navigable set view of the keys in this map </returns>
		Function descendingKeySet() As NavigableSet(Of K)
	End Interface

End Namespace
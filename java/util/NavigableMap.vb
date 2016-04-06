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
' * Written by Doug Lea and Josh Bloch with assistance from members of JCP
' * JSR-166 Expert Group and released to the public domain, as explained at
' * http://creativecommons.org/publicdomain/zero/1.0/
' 

Namespace java.util

	''' <summary>
	''' A <seealso cref="SortedMap"/> extended with navigation methods returning the
	''' closest matches for given search targets. Methods
	''' {@code lowerEntry}, {@code floorEntry}, {@code ceilingEntry},
	''' and {@code higherEntry} return {@code Map.Entry} objects
	''' associated with keys respectively less than, less than or equal,
	''' greater than or equal, and greater than a given key, returning
	''' {@code null} if there is no such key.  Similarly, methods
	''' {@code lowerKey}, {@code floorKey}, {@code ceilingKey}, and
	''' {@code higherKey} return only the associated keys. All of these
	''' methods are designed for locating, not traversing entries.
	''' 
	''' <p>A {@code NavigableMap} may be accessed and traversed in either
	''' ascending or descending key order.  The {@code descendingMap}
	''' method returns a view of the map with the senses of all relational
	''' and directional methods inverted. The performance of ascending
	''' operations and views is likely to be faster than that of descending
	''' ones.  Methods {@code subMap}, {@code headMap},
	''' and {@code tailMap} differ from the like-named {@code
	''' SortedMap} methods in accepting additional arguments describing
	''' whether lower and upper bounds are inclusive versus exclusive.
	''' Submaps of any {@code NavigableMap} must implement the {@code
	''' NavigableMap} interface.
	''' 
	''' <p>This interface additionally defines methods {@code firstEntry},
	''' {@code pollFirstEntry}, {@code lastEntry}, and
	''' {@code pollLastEntry} that return and/or remove the least and
	''' greatest mappings, if any exist, else returning {@code null}.
	''' 
	''' <p>Implementations of entry-returning methods are expected to
	''' return {@code Map.Entry} pairs representing snapshots of mappings
	''' at the time they were produced, and thus generally do <em>not</em>
	''' support the optional {@code Entry.setValue} method. Note however
	''' that it is possible to change mappings in the associated map using
	''' method {@code put}.
	''' 
	''' <p>Methods
	''' <seealso cref="#subMap(Object, Object) subMap(K, K)"/>,
	''' <seealso cref="#headMap(Object) headMap(K)"/>, and
	''' <seealso cref="#tailMap(Object) tailMap(K)"/>
	''' are specified to return {@code SortedMap} to allow existing
	''' implementations of {@code SortedMap} to be compatibly retrofitted to
	''' implement {@code NavigableMap}, but extensions and implementations
	''' of this interface are encouraged to override these methods to return
	''' {@code NavigableMap}.  Similarly,
	''' <seealso cref="#keySet()"/> can be overriden to return {@code NavigableSet}.
	''' 
	''' <p>This interface is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @author Doug Lea
	''' @author Josh Bloch </summary>
	''' @param <K> the type of keys maintained by this map </param>
	''' @param <V> the type of mapped values
	''' @since 1.6 </param>
	Public Interface NavigableMap(Of K, V)
		Inherits SortedMap(Of K, V)

		''' <summary>
		''' Returns a key-value mapping associated with the greatest key
		''' strictly less than the given key, or {@code null} if there is
		''' no such key.
		''' </summary>
		''' <param name="key"> the key </param>
		''' <returns> an entry with the greatest key less than {@code key},
		'''         or {@code null} if there is no such key </returns>
		''' <exception cref="ClassCastException"> if the specified key cannot be compared
		'''         with the keys currently in the map </exception>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         and this map does not permit null keys </exception>
		Function lowerEntry(  key As K) As KeyValuePair(Of K, V)

		''' <summary>
		''' Returns the greatest key strictly less than the given key, or
		''' {@code null} if there is no such key.
		''' </summary>
		''' <param name="key"> the key </param>
		''' <returns> the greatest key less than {@code key},
		'''         or {@code null} if there is no such key </returns>
		''' <exception cref="ClassCastException"> if the specified key cannot be compared
		'''         with the keys currently in the map </exception>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         and this map does not permit null keys </exception>
		Function lowerKey(  key As K) As K

		''' <summary>
		''' Returns a key-value mapping associated with the greatest key
		''' less than or equal to the given key, or {@code null} if there
		''' is no such key.
		''' </summary>
		''' <param name="key"> the key </param>
		''' <returns> an entry with the greatest key less than or equal to
		'''         {@code key}, or {@code null} if there is no such key </returns>
		''' <exception cref="ClassCastException"> if the specified key cannot be compared
		'''         with the keys currently in the map </exception>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         and this map does not permit null keys </exception>
		Function floorEntry(  key As K) As KeyValuePair(Of K, V)

		''' <summary>
		''' Returns the greatest key less than or equal to the given key,
		''' or {@code null} if there is no such key.
		''' </summary>
		''' <param name="key"> the key </param>
		''' <returns> the greatest key less than or equal to {@code key},
		'''         or {@code null} if there is no such key </returns>
		''' <exception cref="ClassCastException"> if the specified key cannot be compared
		'''         with the keys currently in the map </exception>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         and this map does not permit null keys </exception>
		Function floorKey(  key As K) As K

		''' <summary>
		''' Returns a key-value mapping associated with the least key
		''' greater than or equal to the given key, or {@code null} if
		''' there is no such key.
		''' </summary>
		''' <param name="key"> the key </param>
		''' <returns> an entry with the least key greater than or equal to
		'''         {@code key}, or {@code null} if there is no such key </returns>
		''' <exception cref="ClassCastException"> if the specified key cannot be compared
		'''         with the keys currently in the map </exception>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         and this map does not permit null keys </exception>
		Function ceilingEntry(  key As K) As KeyValuePair(Of K, V)

		''' <summary>
		''' Returns the least key greater than or equal to the given key,
		''' or {@code null} if there is no such key.
		''' </summary>
		''' <param name="key"> the key </param>
		''' <returns> the least key greater than or equal to {@code key},
		'''         or {@code null} if there is no such key </returns>
		''' <exception cref="ClassCastException"> if the specified key cannot be compared
		'''         with the keys currently in the map </exception>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         and this map does not permit null keys </exception>
		Function ceilingKey(  key As K) As K

		''' <summary>
		''' Returns a key-value mapping associated with the least key
		''' strictly greater than the given key, or {@code null} if there
		''' is no such key.
		''' </summary>
		''' <param name="key"> the key </param>
		''' <returns> an entry with the least key greater than {@code key},
		'''         or {@code null} if there is no such key </returns>
		''' <exception cref="ClassCastException"> if the specified key cannot be compared
		'''         with the keys currently in the map </exception>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         and this map does not permit null keys </exception>
		Function higherEntry(  key As K) As KeyValuePair(Of K, V)

		''' <summary>
		''' Returns the least key strictly greater than the given key, or
		''' {@code null} if there is no such key.
		''' </summary>
		''' <param name="key"> the key </param>
		''' <returns> the least key greater than {@code key},
		'''         or {@code null} if there is no such key </returns>
		''' <exception cref="ClassCastException"> if the specified key cannot be compared
		'''         with the keys currently in the map </exception>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         and this map does not permit null keys </exception>
		Function higherKey(  key As K) As K

		''' <summary>
		''' Returns a key-value mapping associated with the least
		''' key in this map, or {@code null} if the map is empty.
		''' </summary>
		''' <returns> an entry with the least key,
		'''         or {@code null} if this map is empty </returns>
		Function firstEntry() As KeyValuePair(Of K, V)

		''' <summary>
		''' Returns a key-value mapping associated with the greatest
		''' key in this map, or {@code null} if the map is empty.
		''' </summary>
		''' <returns> an entry with the greatest key,
		'''         or {@code null} if this map is empty </returns>
		Function lastEntry() As KeyValuePair(Of K, V)

		''' <summary>
		''' Removes and returns a key-value mapping associated with
		''' the least key in this map, or {@code null} if the map is empty.
		''' </summary>
		''' <returns> the removed first entry of this map,
		'''         or {@code null} if this map is empty </returns>
		Function pollFirstEntry() As KeyValuePair(Of K, V)

		''' <summary>
		''' Removes and returns a key-value mapping associated with
		''' the greatest key in this map, or {@code null} if the map is empty.
		''' </summary>
		''' <returns> the removed last entry of this map,
		'''         or {@code null} if this map is empty </returns>
		Function pollLastEntry() As KeyValuePair(Of K, V)

		''' <summary>
		''' Returns a reverse order view of the mappings contained in this map.
		''' The descending map is backed by this map, so changes to the map are
		''' reflected in the descending map, and vice-versa.  If either map is
		''' modified while an iteration over a collection view of either map
		''' is in progress (except through the iterator's own {@code remove}
		''' operation), the results of the iteration are undefined.
		''' 
		''' <p>The returned map has an ordering equivalent to
		''' <tt><seealso cref="Collections#reverseOrder(Comparator) Collections.reverseOrder"/>(comparator())</tt>.
		''' The expression {@code m.descendingMap().descendingMap()} returns a
		''' view of {@code m} essentially equivalent to {@code m}.
		''' </summary>
		''' <returns> a reverse order view of this map </returns>
		Function descendingMap() As NavigableMap(Of K, V)

		''' <summary>
		''' Returns a <seealso cref="NavigableSet"/> view of the keys contained in this map.
		''' The set's iterator returns the keys in ascending order.
		''' The set is backed by the map, so changes to the map are reflected in
		''' the set, and vice-versa.  If the map is modified while an iteration
		''' over the set is in progress (except through the iterator's own {@code
		''' remove} operation), the results of the iteration are undefined.  The
		''' set supports element removal, which removes the corresponding mapping
		''' from the map, via the {@code Iterator.remove}, {@code Set.remove},
		''' {@code removeAll}, {@code retainAll}, and {@code clear} operations.
		''' It does not support the {@code add} or {@code addAll} operations.
		''' </summary>
		''' <returns> a navigable set view of the keys in this map </returns>
		Function navigableKeySet() As NavigableSet(Of K)

		''' <summary>
		''' Returns a reverse order <seealso cref="NavigableSet"/> view of the keys contained in this map.
		''' The set's iterator returns the keys in descending order.
		''' The set is backed by the map, so changes to the map are reflected in
		''' the set, and vice-versa.  If the map is modified while an iteration
		''' over the set is in progress (except through the iterator's own {@code
		''' remove} operation), the results of the iteration are undefined.  The
		''' set supports element removal, which removes the corresponding mapping
		''' from the map, via the {@code Iterator.remove}, {@code Set.remove},
		''' {@code removeAll}, {@code retainAll}, and {@code clear} operations.
		''' It does not support the {@code add} or {@code addAll} operations.
		''' </summary>
		''' <returns> a reverse order navigable set view of the keys in this map </returns>
		Function descendingKeySet() As NavigableSet(Of K)

		''' <summary>
		''' Returns a view of the portion of this map whose keys range from
		''' {@code fromKey} to {@code toKey}.  If {@code fromKey} and
		''' {@code toKey} are equal, the returned map is empty unless
		''' {@code fromInclusive} and {@code toInclusive} are both true.  The
		''' returned map is backed by this map, so changes in the returned map are
		''' reflected in this map, and vice-versa.  The returned map supports all
		''' optional map operations that this map supports.
		''' 
		''' <p>The returned map will throw an {@code IllegalArgumentException}
		''' on an attempt to insert a key outside of its range, or to construct a
		''' submap either of whose endpoints lie outside its range.
		''' </summary>
		''' <param name="fromKey"> low endpoint of the keys in the returned map </param>
		''' <param name="fromInclusive"> {@code true} if the low endpoint
		'''        is to be included in the returned view </param>
		''' <param name="toKey"> high endpoint of the keys in the returned map </param>
		''' <param name="toInclusive"> {@code true} if the high endpoint
		'''        is to be included in the returned view </param>
		''' <returns> a view of the portion of this map whose keys range from
		'''         {@code fromKey} to {@code toKey} </returns>
		''' <exception cref="ClassCastException"> if {@code fromKey} and {@code toKey}
		'''         cannot be compared to one another using this map's comparator
		'''         (or, if the map has no comparator, using natural ordering).
		'''         Implementations may, but are not required to, throw this
		'''         exception if {@code fromKey} or {@code toKey}
		'''         cannot be compared to keys currently in the map. </exception>
		''' <exception cref="NullPointerException"> if {@code fromKey} or {@code toKey}
		'''         is null and this map does not permit null keys </exception>
		''' <exception cref="IllegalArgumentException"> if {@code fromKey} is greater than
		'''         {@code toKey}; or if this map itself has a restricted
		'''         range, and {@code fromKey} or {@code toKey} lies
		'''         outside the bounds of the range </exception>
		Function subMap(  fromKey As K,   fromInclusive As Boolean,   toKey As K,   toInclusive As Boolean) As NavigableMap(Of K, V)

		''' <summary>
		''' Returns a view of the portion of this map whose keys are less than (or
		''' equal to, if {@code inclusive} is true) {@code toKey}.  The returned
		''' map is backed by this map, so changes in the returned map are reflected
		''' in this map, and vice-versa.  The returned map supports all optional
		''' map operations that this map supports.
		''' 
		''' <p>The returned map will throw an {@code IllegalArgumentException}
		''' on an attempt to insert a key outside its range.
		''' </summary>
		''' <param name="toKey"> high endpoint of the keys in the returned map </param>
		''' <param name="inclusive"> {@code true} if the high endpoint
		'''        is to be included in the returned view </param>
		''' <returns> a view of the portion of this map whose keys are less than
		'''         (or equal to, if {@code inclusive} is true) {@code toKey} </returns>
		''' <exception cref="ClassCastException"> if {@code toKey} is not compatible
		'''         with this map's comparator (or, if the map has no comparator,
		'''         if {@code toKey} does not implement <seealso cref="Comparable"/>).
		'''         Implementations may, but are not required to, throw this
		'''         exception if {@code toKey} cannot be compared to keys
		'''         currently in the map. </exception>
		''' <exception cref="NullPointerException"> if {@code toKey} is null
		'''         and this map does not permit null keys </exception>
		''' <exception cref="IllegalArgumentException"> if this map itself has a
		'''         restricted range, and {@code toKey} lies outside the
		'''         bounds of the range </exception>
		Function headMap(  toKey As K,   inclusive As Boolean) As NavigableMap(Of K, V)

		''' <summary>
		''' Returns a view of the portion of this map whose keys are greater than (or
		''' equal to, if {@code inclusive} is true) {@code fromKey}.  The returned
		''' map is backed by this map, so changes in the returned map are reflected
		''' in this map, and vice-versa.  The returned map supports all optional
		''' map operations that this map supports.
		''' 
		''' <p>The returned map will throw an {@code IllegalArgumentException}
		''' on an attempt to insert a key outside its range.
		''' </summary>
		''' <param name="fromKey"> low endpoint of the keys in the returned map </param>
		''' <param name="inclusive"> {@code true} if the low endpoint
		'''        is to be included in the returned view </param>
		''' <returns> a view of the portion of this map whose keys are greater than
		'''         (or equal to, if {@code inclusive} is true) {@code fromKey} </returns>
		''' <exception cref="ClassCastException"> if {@code fromKey} is not compatible
		'''         with this map's comparator (or, if the map has no comparator,
		'''         if {@code fromKey} does not implement <seealso cref="Comparable"/>).
		'''         Implementations may, but are not required to, throw this
		'''         exception if {@code fromKey} cannot be compared to keys
		'''         currently in the map. </exception>
		''' <exception cref="NullPointerException"> if {@code fromKey} is null
		'''         and this map does not permit null keys </exception>
		''' <exception cref="IllegalArgumentException"> if this map itself has a
		'''         restricted range, and {@code fromKey} lies outside the
		'''         bounds of the range </exception>
		Function tailMap(  fromKey As K,   inclusive As Boolean) As NavigableMap(Of K, V)

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' <p>Equivalent to {@code subMap(fromKey, true, toKey, false)}.
		''' </summary>
		''' <exception cref="ClassCastException">       {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">     {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Function subMap(  fromKey As K,   toKey As K) As SortedMap(Of K, V)

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' <p>Equivalent to {@code headMap(toKey, false)}.
		''' </summary>
		''' <exception cref="ClassCastException">       {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">     {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Function headMap(  toKey As K) As SortedMap(Of K, V)

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' <p>Equivalent to {@code tailMap(fromKey, true)}.
		''' </summary>
		''' <exception cref="ClassCastException">       {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">     {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Function tailMap(  fromKey As K) As SortedMap(Of K, V)
	End Interface

End Namespace
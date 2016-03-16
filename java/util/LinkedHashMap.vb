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
	''' <p>Hash table and linked list implementation of the <tt>Map</tt> interface,
	''' with predictable iteration order.  This implementation differs from
	''' <tt>HashMap</tt> in that it maintains a doubly-linked list running through
	''' all of its entries.  This linked list defines the iteration ordering,
	''' which is normally the order in which keys were inserted into the map
	''' (<i>insertion-order</i>).  Note that insertion order is not affected
	''' if a key is <i>re-inserted</i> into the map.  (A key <tt>k</tt> is
	''' reinserted into a map <tt>m</tt> if <tt>m.put(k, v)</tt> is invoked when
	''' <tt>m.containsKey(k)</tt> would return <tt>true</tt> immediately prior to
	''' the invocation.)
	''' 
	''' <p>This implementation spares its clients from the unspecified, generally
	''' chaotic ordering provided by <seealso cref="HashMap"/> (and <seealso cref="Hashtable"/>),
	''' without incurring the increased cost associated with <seealso cref="TreeMap"/>.  It
	''' can be used to produce a copy of a map that has the same order as the
	''' original, regardless of the original map's implementation:
	''' <pre>
	'''      Sub  foo(Map m) {
	'''         Map copy = new LinkedHashMap(m);
	'''         ...
	'''     }
	''' </pre>
	''' This technique is particularly useful if a module takes a map on input,
	''' copies it, and later returns results whose order is determined by that of
	''' the copy.  (Clients generally appreciate having things returned in the same
	''' order they were presented.)
	''' 
	''' <p>A special <seealso cref="#LinkedHashMap(int,float,boolean) constructor"/> is
	''' provided to create a linked hash map whose order of iteration is the order
	''' in which its entries were last accessed, from least-recently accessed to
	''' most-recently (<i>access-order</i>).  This kind of map is well-suited to
	''' building LRU caches.  Invoking the {@code put}, {@code putIfAbsent},
	''' {@code get}, {@code getOrDefault}, {@code compute}, {@code computeIfAbsent},
	''' {@code computeIfPresent}, or {@code merge} methods results
	''' in an access to the corresponding entry (assuming it exists after the
	''' invocation completes). The {@code replace} methods only result in an access
	''' of the entry if the value is replaced.  The {@code putAll} method generates one
	''' entry access for each mapping in the specified map, in the order that
	''' key-value mappings are provided by the specified map's entry set iterator.
	''' <i>No other methods generate entry accesses.</i>  In particular, operations
	''' on collection-views do <i>not</i> affect the order of iteration of the
	''' backing map.
	''' 
	''' <p>The <seealso cref="#removeEldestEntry(Map.Entry)"/> method may be overridden to
	''' impose a policy for removing stale mappings automatically when new mappings
	''' are added to the map.
	''' 
	''' <p>This class provides all of the optional <tt>Map</tt> operations, and
	''' permits null elements.  Like <tt>HashMap</tt>, it provides constant-time
	''' performance for the basic operations (<tt>add</tt>, <tt>contains</tt> and
	''' <tt>remove</tt>), assuming the hash function disperses elements
	''' properly among the buckets.  Performance is likely to be just slightly
	''' below that of <tt>HashMap</tt>, due to the added expense of maintaining the
	''' linked list, with one exception: Iteration over the collection-views
	''' of a <tt>LinkedHashMap</tt> requires time proportional to the <i>size</i>
	''' of the map, regardless of its capacity.  Iteration over a <tt>HashMap</tt>
	''' is likely to be more expensive, requiring time proportional to its
	''' <i>capacity</i>.
	''' 
	''' <p>A linked hash map has two parameters that affect its performance:
	''' <i>initial capacity</i> and <i>load factor</i>.  They are defined precisely
	''' as for <tt>HashMap</tt>.  Note, however, that the penalty for choosing an
	''' excessively high value for initial capacity is less severe for this class
	''' than for <tt>HashMap</tt>, as iteration times for this class are unaffected
	''' by capacity.
	''' 
	''' <p><strong>Note that this implementation is not synchronized.</strong>
	''' If multiple threads access a linked hash map concurrently, and at least
	''' one of the threads modifies the map structurally, it <em>must</em> be
	''' synchronized externally.  This is typically accomplished by
	''' synchronizing on some object that naturally encapsulates the map.
	''' 
	''' If no such object exists, the map should be "wrapped" using the
	''' <seealso cref="Collections#synchronizedMap Collections.synchronizedMap"/>
	''' method.  This is best done at creation time, to prevent accidental
	''' unsynchronized access to the map:<pre>
	'''   Map m = Collections.synchronizedMap(new LinkedHashMap(...));</pre>
	''' 
	''' A structural modification is any operation that adds or deletes one or more
	''' mappings or, in the case of access-ordered linked hash maps, affects
	''' iteration order.  In insertion-ordered linked hash maps, merely changing
	''' the value associated with a key that is already contained in the map is not
	''' a structural modification.  <strong>In access-ordered linked hash maps,
	''' merely querying the map with <tt>get</tt> is a structural modification.
	''' </strong>)
	''' 
	''' <p>The iterators returned by the <tt>iterator</tt> method of the collections
	''' returned by all of this class's collection view methods are
	''' <em>fail-fast</em>: if the map is structurally modified at any time after
	''' the iterator is created, in any way except through the iterator's own
	''' <tt>remove</tt> method, the iterator will throw a {@link
	''' ConcurrentModificationException}.  Thus, in the face of concurrent
	''' modification, the iterator fails quickly and cleanly, rather than risking
	''' arbitrary, non-deterministic behavior at an undetermined time in the future.
	''' 
	''' <p>Note that the fail-fast behavior of an iterator cannot be guaranteed
	''' as it is, generally speaking, impossible to make any hard guarantees in the
	''' presence of unsynchronized concurrent modification.  Fail-fast iterators
	''' throw <tt>ConcurrentModificationException</tt> on a best-effort basis.
	''' Therefore, it would be wrong to write a program that depended on this
	''' exception for its correctness:   <i>the fail-fast behavior of iterators
	''' should be used only to detect bugs.</i>
	''' 
	''' <p>The spliterators returned by the spliterator method of the collections
	''' returned by all of this class's collection view methods are
	''' <em><a href="Spliterator.html#binding">late-binding</a></em>,
	''' <em>fail-fast</em>, and additionally report <seealso cref="Spliterator#ORDERED"/>.
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @implNote
	''' The spliterators returned by the spliterator method of the collections
	''' returned by all of this class's collection view methods are created from
	''' the iterators of the corresponding collections.
	''' </summary>
	''' @param <K> the type of keys maintained by this map </param>
	''' @param <V> the type of mapped values
	''' 
	''' @author  Josh Bloch </param>
	''' <seealso cref=     Object#hashCode() </seealso>
	''' <seealso cref=     Collection </seealso>
	''' <seealso cref=     Map </seealso>
	''' <seealso cref=     HashMap </seealso>
	''' <seealso cref=     TreeMap </seealso>
	''' <seealso cref=     Hashtable
	''' @since   1.4 </seealso>
	Public Class LinkedHashMap(Of K, V)
		Inherits HashMap(Of K, V)
		Implements Map(Of K, V)

	'    
	'     * Implementation note.  A previous version of this class was
	'     * internally structured a little differently. Because superclass
	'     * HashMap now uses trees for some of its nodes, class
	'     * LinkedHashMap.Entry is now treated as intermediary node class
	'     * that can also be converted to tree form. The name of this
	'     * [Class], LinkedHashMap.Entry, is confusing in several ways in its
	'     * current context, but cannot be changed.  Otherwise, even though
	'     * it is not exported outside this package, some existing source
	'     * code is known to have relied on a symbol resolution corner case
	'     * rule in calls to removeEldestEntry that suppressed compilation
	'     * errors due to ambiguous usages. So, we keep the name to
	'     * preserve unmodified compilability.
	'     *
	'     * The changes in node classes also require using two fields
	'     * (head, tail) rather than a pointer to a header node to maintain
	'     * the doubly-linked before/after list. This class also
	'     * previously used a different style of callback methods upon
	'     * access, insertion, and removal.
	'     

		''' <summary>
		''' HashMap.Node subclass for normal LinkedHashMap entries.
		''' </summary>
		Friend Class Entry(Of K, V)
			Inherits HashMap.Node(Of K, V)

			Friend before As Entry(Of K, V), after As Entry(Of K, V)
			Friend Sub New(ByVal hash As Integer, ByVal key As K, ByVal value As V, ByVal [next] As Node(Of K, V))
				MyBase.New(hash, key, value, [next])
			End Sub
		End Class

		Private Const serialVersionUID As Long = 3801124242820219131L

		''' <summary>
		''' The head (eldest) of the doubly linked list.
		''' </summary>
		<NonSerialized> _
		Friend head As LinkedHashMap.Entry(Of K, V)

		''' <summary>
		''' The tail (youngest) of the doubly linked list.
		''' </summary>
		<NonSerialized> _
		Friend tail As LinkedHashMap.Entry(Of K, V)

		''' <summary>
		''' The iteration ordering method for this linked hash map: <tt>true</tt>
		''' for access-order, <tt>false</tt> for insertion-order.
		''' 
		''' @serial
		''' </summary>
		Friend ReadOnly accessOrder As Boolean

		' internal utilities

		' link at the end of list
		Private Sub linkNodeLast(ByVal p As LinkedHashMap.Entry(Of K, V))
			Dim last As LinkedHashMap.Entry(Of K, V) = tail
			tail = p
			If last Is Nothing Then
				head = p
			Else
				p.before = last
				last.after = p
			End If
		End Sub

		' apply src's links to dst
		Private Sub transferLinks(ByVal src As LinkedHashMap.Entry(Of K, V), ByVal dst As LinkedHashMap.Entry(Of K, V))
				dst.before = src.before
				Dim b As LinkedHashMap.Entry(Of K, V) = dst.before
				dst.after = src.after
				Dim a As LinkedHashMap.Entry(Of K, V) = dst.after
			If b Is Nothing Then
				head = dst
			Else
				b.after = dst
			End If
			If a Is Nothing Then
				tail = dst
			Else
				a.before = dst
			End If
		End Sub

		' overrides of HashMap hook methods

		Friend Overridable Sub reinitialize()
			MyBase.reinitialize()
				tail = Nothing
				head = tail
		End Sub

		Friend Overridable Function newNode(ByVal hash As Integer, ByVal key As K, ByVal value As V, ByVal e As Node(Of K, V)) As Node(Of K, V)
			Dim p As New LinkedHashMap.Entry(Of K, V)(hash, key, value, e)
			linkNodeLast(p)
			Return p
		End Function

		Friend Overridable Function replacementNode(ByVal p As Node(Of K, V), ByVal [next] As Node(Of K, V)) As Node(Of K, V)
			Dim q As LinkedHashMap.Entry(Of K, V) = CType(p, LinkedHashMap.Entry(Of K, V))
			Dim t As New LinkedHashMap.Entry(Of K, V)(q.hash, q.key, q.value, [next])
			transferLinks(q, t)
			Return t
		End Function

		Friend Overridable Function newTreeNode(ByVal hash As Integer, ByVal key As K, ByVal value As V, ByVal [next] As Node(Of K, V)) As TreeNode(Of K, V)
			Dim p As New TreeNode(Of K, V)(hash, key, value, [next])
			linkNodeLast(p)
			Return p
		End Function

		Friend Overridable Function replacementTreeNode(ByVal p As Node(Of K, V), ByVal [next] As Node(Of K, V)) As TreeNode(Of K, V)
			Dim q As LinkedHashMap.Entry(Of K, V) = CType(p, LinkedHashMap.Entry(Of K, V))
			Dim t As New TreeNode(Of K, V)(q.hash, q.key, q.value, [next])
			transferLinks(q, t)
			Return t
		End Function

		Friend Overridable Sub afterNodeRemoval(ByVal e As Node(Of K, V)) ' unlink
			Dim p As LinkedHashMap.Entry(Of K, V) = CType(e, LinkedHashMap.Entry(Of K, V)), b As LinkedHashMap.Entry(Of K, V) = p.before, a As LinkedHashMap.Entry(Of K, V) = p.after
				p.after = Nothing
				p.before = p.after
			If b Is Nothing Then
				head = a
			Else
				b.after = a
			End If
			If a Is Nothing Then
				tail = b
			Else
				a.before = b
			End If
		End Sub

		Friend Overridable Sub afterNodeInsertion(ByVal evict As Boolean) ' possibly remove eldest
			Dim first As LinkedHashMap.Entry(Of K, V)
			first = head
			If evict AndAlso first IsNot Nothing AndAlso removeEldestEntry(first) Then
				Dim key As K = first.key
				removeNode(hash(key), key, Nothing, False, True)
			End If
		End Sub

		Friend Overridable Sub afterNodeAccess(ByVal e As Node(Of K, V)) ' move node to last
			Dim last As LinkedHashMap.Entry(Of K, V)
			last = tail
			If accessOrder AndAlso last IsNot e Then
				Dim p As LinkedHashMap.Entry(Of K, V) = CType(e, LinkedHashMap.Entry(Of K, V)), b As LinkedHashMap.Entry(Of K, V) = p.before, a As LinkedHashMap.Entry(Of K, V) = p.after
				p.after = Nothing
				If b Is Nothing Then
					head = a
				Else
					b.after = a
				End If
				If a IsNot Nothing Then
					a.before = b
				Else
					last = b
				End If
				If last Is Nothing Then
					head = p
				Else
					p.before = last
					last.after = p
				End If
				tail = p
				modCount += 1
			End If
		End Sub

		Friend Overridable Sub internalWriteEntries(ByVal s As java.io.ObjectOutputStream)
			Dim e As LinkedHashMap.Entry(Of K, V) = head
			Do While e IsNot Nothing
				s.writeObject(e.key)
				s.writeObject(e.value)
				e = e.after
			Loop
		End Sub

		''' <summary>
		''' Constructs an empty insertion-ordered <tt>LinkedHashMap</tt> instance
		''' with the specified initial capacity and load factor.
		''' </summary>
		''' <param name="initialCapacity"> the initial capacity </param>
		''' <param name="loadFactor">      the load factor </param>
		''' <exception cref="IllegalArgumentException"> if the initial capacity is negative
		'''         or the load factor is nonpositive </exception>
		Public Sub New(ByVal initialCapacity As Integer, ByVal loadFactor As Single)
			MyBase.New(initialCapacity, loadFactor)
			accessOrder = False
		End Sub

		''' <summary>
		''' Constructs an empty insertion-ordered <tt>LinkedHashMap</tt> instance
		''' with the specified initial capacity and a default load factor (0.75).
		''' </summary>
		''' <param name="initialCapacity"> the initial capacity </param>
		''' <exception cref="IllegalArgumentException"> if the initial capacity is negative </exception>
		Public Sub New(ByVal initialCapacity As Integer)
			MyBase.New(initialCapacity)
			accessOrder = False
		End Sub

		''' <summary>
		''' Constructs an empty insertion-ordered <tt>LinkedHashMap</tt> instance
		''' with the default initial capacity (16) and load factor (0.75).
		''' </summary>
		Public Sub New()
			MyBase.New()
			accessOrder = False
		End Sub

		''' <summary>
		''' Constructs an insertion-ordered <tt>LinkedHashMap</tt> instance with
		''' the same mappings as the specified map.  The <tt>LinkedHashMap</tt>
		''' instance is created with a default load factor (0.75) and an initial
		''' capacity sufficient to hold the mappings in the specified map.
		''' </summary>
		''' <param name="m"> the map whose mappings are to be placed in this map </param>
		''' <exception cref="NullPointerException"> if the specified map is null </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Sub New(Of T1 As K, ? As V)(ByVal m As Map(Of T1))
			MyBase.New()
			accessOrder = False
			putMapEntries(m, False)
		End Sub

		''' <summary>
		''' Constructs an empty <tt>LinkedHashMap</tt> instance with the
		''' specified initial capacity, load factor and ordering mode.
		''' </summary>
		''' <param name="initialCapacity"> the initial capacity </param>
		''' <param name="loadFactor">      the load factor </param>
		''' <param name="accessOrder">     the ordering mode - <tt>true</tt> for
		'''         access-order, <tt>false</tt> for insertion-order </param>
		''' <exception cref="IllegalArgumentException"> if the initial capacity is negative
		'''         or the load factor is nonpositive </exception>
		Public Sub New(ByVal initialCapacity As Integer, ByVal loadFactor As Single, ByVal accessOrder As Boolean)
			MyBase.New(initialCapacity, loadFactor)
			Me.accessOrder = accessOrder
		End Sub


		''' <summary>
		''' Returns <tt>true</tt> if this map maps one or more keys to the
		''' specified value.
		''' </summary>
		''' <param name="value"> value whose presence in this map is to be tested </param>
		''' <returns> <tt>true</tt> if this map maps one or more keys to the
		'''         specified value </returns>
		Public Overridable Function containsValue(ByVal value As Object) As Boolean Implements Map(Of K, V).containsValue
			Dim e As LinkedHashMap.Entry(Of K, V) = head
			Do While e IsNot Nothing
				Dim v As V = e.value
				If v Is value OrElse (value IsNot Nothing AndAlso value.Equals(v)) Then Return True
				e = e.after
			Loop
			Return False
		End Function

		''' <summary>
		''' Returns the value to which the specified key is mapped,
		''' or {@code null} if this map contains no mapping for the key.
		''' 
		''' <p>More formally, if this map contains a mapping from a key
		''' {@code k} to a value {@code v} such that {@code (key==null ? k==null :
		''' key.equals(k))}, then this method returns {@code v}; otherwise
		''' it returns {@code null}.  (There can be at most one such mapping.)
		''' 
		''' <p>A return value of {@code null} does not <i>necessarily</i>
		''' indicate that the map contains no mapping for the key; it's also
		''' possible that the map explicitly maps the key to {@code null}.
		''' The <seealso cref="#containsKey containsKey"/> operation may be used to
		''' distinguish these two cases.
		''' </summary>
		Public Overridable Function [get](ByVal key As Object) As V Implements Map(Of K, V).get
			Dim e As Node(Of K, V)
			e = getNode(hash(key), key)
			If e Is Nothing Then Return Nothing
			If accessOrder Then afterNodeAccess(e)
			Return e.value
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Function getOrDefault(ByVal key As Object, ByVal defaultValue As V) As V Implements Map(Of K, V).getOrDefault
		   Dim e As Node(Of K, V)
		   e = getNode(hash(key), key)
		   If e Is Nothing Then Return defaultValue
		   If accessOrder Then afterNodeAccess(e)
		   Return e.value
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Sub clear() Implements Map(Of K, V).clear
			MyBase.clear()
				tail = Nothing
				head = tail
		End Sub

		''' <summary>
		''' Returns <tt>true</tt> if this map should remove its eldest entry.
		''' This method is invoked by <tt>put</tt> and <tt>putAll</tt> after
		''' inserting a new entry into the map.  It provides the implementor
		''' with the opportunity to remove the eldest entry each time a new one
		''' is added.  This is useful if the map represents a cache: it allows
		''' the map to reduce memory consumption by deleting stale entries.
		''' 
		''' <p>Sample use: this override will allow the map to grow up to 100
		''' entries and then delete the eldest entry each time a new entry is
		''' added, maintaining a steady state of 100 entries.
		''' <pre>
		'''     private static final int MAX_ENTRIES = 100;
		''' 
		'''     protected boolean removeEldestEntry(Map.Entry eldest) {
		'''        return size() &gt; MAX_ENTRIES;
		'''     }
		''' </pre>
		''' 
		''' <p>This method typically does not modify the map in any way,
		''' instead allowing the map to modify itself as directed by its
		''' return value.  It <i>is</i> permitted for this method to modify
		''' the map directly, but if it does so, it <i>must</i> return
		''' <tt>false</tt> (indicating that the map should not attempt any
		''' further modification).  The effects of returning <tt>true</tt>
		''' after modifying the map from within this method are unspecified.
		''' 
		''' <p>This implementation merely returns <tt>false</tt> (so that this
		''' map acts like a normal map - the eldest element is never removed).
		''' </summary>
		''' <param name="eldest"> The least recently inserted entry in the map, or if
		'''           this is an access-ordered map, the least recently accessed
		'''           entry.  This is the entry that will be removed it this
		'''           method returns <tt>true</tt>.  If the map was empty prior
		'''           to the <tt>put</tt> or <tt>putAll</tt> invocation resulting
		'''           in this invocation, this will be the entry that was just
		'''           inserted; in other words, if the map contains a single
		'''           entry, the eldest entry is also the newest. </param>
		''' <returns>   <tt>true</tt> if the eldest entry should be removed
		'''           from the map; <tt>false</tt> if it should be retained. </returns>
		Protected Friend Overridable Function removeEldestEntry(ByVal eldest As KeyValuePair(Of K, V)) As Boolean
			Return False
		End Function

		''' <summary>
		''' Returns a <seealso cref="Set"/> view of the keys contained in this map.
		''' The set is backed by the map, so changes to the map are
		''' reflected in the set, and vice-versa.  If the map is modified
		''' while an iteration over the set is in progress (except through
		''' the iterator's own <tt>remove</tt> operation), the results of
		''' the iteration are undefined.  The set supports element removal,
		''' which removes the corresponding mapping from the map, via the
		''' <tt>Iterator.remove</tt>, <tt>Set.remove</tt>,
		''' <tt>removeAll</tt>, <tt>retainAll</tt>, and <tt>clear</tt>
		''' operations.  It does not support the <tt>add</tt> or <tt>addAll</tt>
		''' operations.
		''' Its <seealso cref="Spliterator"/> typically provides faster sequential
		''' performance but much poorer parallel performance than that of
		''' {@code HashMap}.
		''' </summary>
		''' <returns> a set view of the keys contained in this map </returns>
		Public Overridable Function keySet() As [Set](Of K) Implements Map(Of K, V).keySet
			Dim ks As [Set](Of K)
				ks = keySet_Renamed
				If ks Is Nothing Then
						keySet_Renamed = New LinkedKeySet(Me, Me)
						Return keySet_Renamed
				Else
					Return ks
				End If
		End Function

		Friend NotInheritable Class LinkedKeySet
			Inherits AbstractSet(Of K)

			Private ReadOnly outerInstance As LinkedHashMap

			Public Sub New(ByVal outerInstance As LinkedHashMap)
				Me.outerInstance = outerInstance
			End Sub

			Public Function size() As Integer
				Return outerInstance.size_Renamed
			End Function
			Public Sub clear()
				outerInstance.clear()
			End Sub
			Public Function [iterator]() As [Iterator](Of K)
				Return New LinkedKeyIterator
			End Function
			Public Function contains(ByVal o As Object) As Boolean
				Return outerInstance.containsKey(o)
			End Function
			Public Function remove(ByVal key As Object) As Boolean
				Return outerInstance.removeNode(hash(key), key, Nothing, False, True) IsNot Nothing
			End Function
			Public Function spliterator() As Spliterator(Of K)
				Return Spliterators.spliterator(Me, Spliterator.SIZED Or Spliterator.ORDERED Or Spliterator.DISTINCT)
			End Function
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				If action Is Nothing Then Throw New NullPointerException
				Dim mc As Integer = outerInstance.modCount
				Dim e As LinkedHashMap.Entry(Of K, V) = outerInstance.head
				Do While e IsNot Nothing
					action.accept(e.key)
					e = e.after
				Loop
				If outerInstance.modCount <> mc Then Throw New ConcurrentModificationException
			End Sub
		End Class

		''' <summary>
		''' Returns a <seealso cref="Collection"/> view of the values contained in this map.
		''' The collection is backed by the map, so changes to the map are
		''' reflected in the collection, and vice-versa.  If the map is
		''' modified while an iteration over the collection is in progress
		''' (except through the iterator's own <tt>remove</tt> operation),
		''' the results of the iteration are undefined.  The collection
		''' supports element removal, which removes the corresponding
		''' mapping from the map, via the <tt>Iterator.remove</tt>,
		''' <tt>Collection.remove</tt>, <tt>removeAll</tt>,
		''' <tt>retainAll</tt> and <tt>clear</tt> operations.  It does not
		''' support the <tt>add</tt> or <tt>addAll</tt> operations.
		''' Its <seealso cref="Spliterator"/> typically provides faster sequential
		''' performance but much poorer parallel performance than that of
		''' {@code HashMap}.
		''' </summary>
		''' <returns> a view of the values contained in this map </returns>
		Public Overridable Function values() As Collection(Of V) Implements Map(Of K, V).values
			Dim vs As Collection(Of V)
				vs = values_Renamed
				If vs Is Nothing Then
						values_Renamed = New LinkedValues(Me, Me)
						Return values_Renamed
				Else
					Return vs
				End If
		End Function

		Friend NotInheritable Class LinkedValues
			Inherits AbstractCollection(Of V)

			Private ReadOnly outerInstance As LinkedHashMap

			Public Sub New(ByVal outerInstance As LinkedHashMap)
				Me.outerInstance = outerInstance
			End Sub

			Public Function size() As Integer
				Return outerInstance.size_Renamed
			End Function
			Public Sub clear()
				outerInstance.clear()
			End Sub
			Public Function [iterator]() As [Iterator](Of V)
				Return New LinkedValueIterator
			End Function
			Public Function contains(ByVal o As Object) As Boolean
				Return outerInstance.containsValue(o)
			End Function
			Public Function spliterator() As Spliterator(Of V)
				Return Spliterators.spliterator(Me, Spliterator.SIZED Or Spliterator.ORDERED)
			End Function
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				If action Is Nothing Then Throw New NullPointerException
				Dim mc As Integer = outerInstance.modCount
				Dim e As LinkedHashMap.Entry(Of K, V) = outerInstance.head
				Do While e IsNot Nothing
					action.accept(e.value)
					e = e.after
				Loop
				If outerInstance.modCount <> mc Then Throw New ConcurrentModificationException
			End Sub
		End Class

		''' <summary>
		''' Returns a <seealso cref="Set"/> view of the mappings contained in this map.
		''' The set is backed by the map, so changes to the map are
		''' reflected in the set, and vice-versa.  If the map is modified
		''' while an iteration over the set is in progress (except through
		''' the iterator's own <tt>remove</tt> operation, or through the
		''' <tt>setValue</tt> operation on a map entry returned by the
		''' iterator) the results of the iteration are undefined.  The set
		''' supports element removal, which removes the corresponding
		''' mapping from the map, via the <tt>Iterator.remove</tt>,
		''' <tt>Set.remove</tt>, <tt>removeAll</tt>, <tt>retainAll</tt> and
		''' <tt>clear</tt> operations.  It does not support the
		''' <tt>add</tt> or <tt>addAll</tt> operations.
		''' Its <seealso cref="Spliterator"/> typically provides faster sequential
		''' performance but much poorer parallel performance than that of
		''' {@code HashMap}.
		''' </summary>
		''' <returns> a set view of the mappings contained in this map </returns>
		Public Overridable Function entrySet() As [Set](Of KeyValuePair(Of K, V)) Implements Map(Of K, V).entrySet
			Dim es As [Set](Of KeyValuePair(Of K, V))
				es = entrySet_Renamed
				If es Is Nothing Then
						entrySet_Renamed = New LinkedEntrySet(Me, Me)
						Return entrySet_Renamed
				Else
					Return es
				End If
		End Function

		Friend NotInheritable Class LinkedEntrySet
			Inherits AbstractSet(Of KeyValuePair(Of K, V))

			Private ReadOnly outerInstance As LinkedHashMap

			Public Sub New(ByVal outerInstance As LinkedHashMap)
				Me.outerInstance = outerInstance
			End Sub

			Public Function size() As Integer
				Return outerInstance.size_Renamed
			End Function
			Public Sub clear()
				outerInstance.clear()
			End Sub
			Public Function [iterator]() As [Iterator](Of KeyValuePair(Of K, V))
				Return New LinkedEntryIterator
			End Function
			Public Function contains(ByVal o As Object) As Boolean
				If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
				Dim key As Object = e.Key
				Dim candidate As Node(Of K, V) = outerInstance.getNode(hash(key), key)
				Return candidate IsNot Nothing AndAlso candidate.Equals(e)
			End Function
			Public Function remove(ByVal o As Object) As Boolean
				If TypeOf o Is DictionaryEntry Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim e As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
					Dim key As Object = e.Key
					Dim value As Object = e.Value
					Return outerInstance.removeNode(hash(key), key, value, True, True) IsNot Nothing
				End If
				Return False
			End Function
			Public Function spliterator() As Spliterator(Of KeyValuePair(Of K, V))
				Return Spliterators.spliterator(Me, Spliterator.SIZED Or Spliterator.ORDERED Or Spliterator.DISTINCT)
			End Function
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				If action Is Nothing Then Throw New NullPointerException
				Dim mc As Integer = outerInstance.modCount
				Dim e As LinkedHashMap.Entry(Of K, V) = outerInstance.head
				Do While e IsNot Nothing
					action.accept(e)
					e = e.after
				Loop
				If outerInstance.modCount <> mc Then Throw New ConcurrentModificationException
			End Sub
		End Class

		' Map overrides

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Sub forEach(Of T1)(ByVal action As java.util.function.BiConsumer(Of T1)) Implements Map(Of K, V).forEach
			If action Is Nothing Then Throw New NullPointerException
			Dim mc As Integer = modCount
			Dim e As LinkedHashMap.Entry(Of K, V) = head
			Do While e IsNot Nothing
				action.accept(e.key, e.value)
				e = e.after
			Loop
			If modCount <> mc Then Throw New ConcurrentModificationException
		End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Sub replaceAll(Of T1 As V)(ByVal [function] As java.util.function.BiFunction(Of T1)) Implements Map(Of K, V).replaceAll
			If [function] Is Nothing Then Throw New NullPointerException
			Dim mc As Integer = modCount
			Dim e As LinkedHashMap.Entry(Of K, V) = head
			Do While e IsNot Nothing
				e.value = [function].apply(e.key, e.value)
				e = e.after
			Loop
			If modCount <> mc Then Throw New ConcurrentModificationException
		End Sub

		' Iterators

		Friend MustInherit Class LinkedHashIterator
			Private ReadOnly outerInstance As LinkedHashMap

			Friend [next] As LinkedHashMap.Entry(Of K, V)
			Friend current As LinkedHashMap.Entry(Of K, V)
			Friend expectedModCount As Integer

			Friend Sub New(ByVal outerInstance As LinkedHashMap)
					Me.outerInstance = outerInstance
				[next] = outerInstance.head
				expectedModCount = outerInstance.modCount
				current = Nothing
			End Sub

			Public Function hasNext() As Boolean
				Return [next] IsNot Nothing
			End Function

			Friend Function nextNode() As LinkedHashMap.Entry(Of K, V)
				Dim e As LinkedHashMap.Entry(Of K, V) = [next]
				If outerInstance.modCount <> expectedModCount Then Throw New ConcurrentModificationException
				If e Is Nothing Then Throw New NoSuchElementException
				current = e
				[next] = e.after
				Return e
			End Function

			Public Sub remove()
				Dim p As Node(Of K, V) = current
				If p Is Nothing Then Throw New IllegalStateException
				If outerInstance.modCount <> expectedModCount Then Throw New ConcurrentModificationException
				current = Nothing
				Dim key As K = p.key
				outerInstance.removeNode(hash(key), key, Nothing, False, False)
				expectedModCount = outerInstance.modCount
			End Sub
		End Class

		Friend NotInheritable Class LinkedKeyIterator
			Inherits LinkedHashIterator
			Implements Iterator(Of K)

			Private ReadOnly outerInstance As LinkedHashMap

			Public Sub New(ByVal outerInstance As LinkedHashMap)
				Me.outerInstance = outerInstance
			End Sub

			Public Function [next]() As K
				Return nextNode().key
			End Function
		End Class

		Friend NotInheritable Class LinkedValueIterator
			Inherits LinkedHashIterator
			Implements Iterator(Of V)

			Private ReadOnly outerInstance As LinkedHashMap

			Public Sub New(ByVal outerInstance As LinkedHashMap)
				Me.outerInstance = outerInstance
			End Sub

			Public Function [next]() As V
				Return nextNode().value
			End Function
		End Class

		Friend NotInheritable Class LinkedEntryIterator
			Inherits LinkedHashIterator
			Implements Iterator(Of KeyValuePair(Of K, V))

			Private ReadOnly outerInstance As LinkedHashMap

			Public Sub New(ByVal outerInstance As LinkedHashMap)
				Me.outerInstance = outerInstance
			End Sub

			Public Function [next]() As KeyValuePair(Of K, V)
				Return nextNode()
			End Function
		End Class


	End Class

End Namespace
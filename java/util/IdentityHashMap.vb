Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' This class implements the <tt>Map</tt> interface with a hash table, using
	''' reference-equality in place of object-equality when comparing keys (and
	''' values).  In other words, in an <tt>IdentityHashMap</tt>, two keys
	''' <tt>k1</tt> and <tt>k2</tt> are considered equal if and only if
	''' <tt>(k1==k2)</tt>.  (In normal <tt>Map</tt> implementations (like
	''' <tt>HashMap</tt>) two keys <tt>k1</tt> and <tt>k2</tt> are considered equal
	''' if and only if <tt>(k1==null ? k2==null : k1.equals(k2))</tt>.)
	''' 
	''' <p><b>This class is <i>not</i> a general-purpose <tt>Map</tt>
	''' implementation!  While this class implements the <tt>Map</tt> interface, it
	''' intentionally violates <tt>Map's</tt> general contract, which mandates the
	''' use of the <tt>equals</tt> method when comparing objects.  This class is
	''' designed for use only in the rare cases wherein reference-equality
	''' semantics are required.</b>
	''' 
	''' <p>A typical use of this class is <i>topology-preserving object graph
	''' transformations</i>, such as serialization or deep-copying.  To perform such
	''' a transformation, a program must maintain a "node table" that keeps track
	''' of all the object references that have already been processed.  The node
	''' table must not equate distinct objects even if they happen to be equal.
	''' Another typical use of this class is to maintain <i>proxy objects</i>.  For
	''' example, a debugging facility might wish to maintain a proxy object for
	''' each object in the program being debugged.
	''' 
	''' <p>This class provides all of the optional map operations, and permits
	''' <tt>null</tt> values and the <tt>null</tt> key.  This class makes no
	''' guarantees as to the order of the map; in particular, it does not guarantee
	''' that the order will remain constant over time.
	''' 
	''' <p>This class provides constant-time performance for the basic
	''' operations (<tt>get</tt> and <tt>put</tt>), assuming the system
	''' identity hash function (<seealso cref="System#identityHashCode(Object)"/>)
	''' disperses elements properly among the buckets.
	''' 
	''' <p>This class has one tuning parameter (which affects performance but not
	''' semantics): <i>expected maximum size</i>.  This parameter is the maximum
	''' number of key-value mappings that the map is expected to hold.  Internally,
	''' this parameter is used to determine the number of buckets initially
	''' comprising the hash table.  The precise relationship between the expected
	''' maximum size and the number of buckets is unspecified.
	''' 
	''' <p>If the size of the map (the number of key-value mappings) sufficiently
	''' exceeds the expected maximum size, the number of buckets is increased.
	''' Increasing the number of buckets ("rehashing") may be fairly expensive, so
	''' it pays to create identity hash maps with a sufficiently large expected
	''' maximum size.  On the other hand, iteration over collection views requires
	''' time proportional to the number of buckets in the hash table, so it
	''' pays not to set the expected maximum size too high if you are especially
	''' concerned with iteration performance or memory usage.
	''' 
	''' <p><strong>Note that this implementation is not synchronized.</strong>
	''' If multiple threads access an identity hash map concurrently, and at
	''' least one of the threads modifies the map structurally, it <i>must</i>
	''' be synchronized externally.  (A structural modification is any operation
	''' that adds or deletes one or more mappings; merely changing the value
	''' associated with a key that an instance already contains is not a
	''' structural modification.)  This is typically accomplished by
	''' synchronizing on some object that naturally encapsulates the map.
	''' 
	''' If no such object exists, the map should be "wrapped" using the
	''' <seealso cref="Collections#synchronizedMap Collections.synchronizedMap"/>
	''' method.  This is best done at creation time, to prevent accidental
	''' unsynchronized access to the map:<pre>
	'''   Map m = Collections.synchronizedMap(new IdentityHashMap(...));</pre>
	''' 
	''' <p>The iterators returned by the <tt>iterator</tt> method of the
	''' collections returned by all of this class's "collection view
	''' methods" are <i>fail-fast</i>: if the map is structurally modified
	''' at any time after the iterator is created, in any way except
	''' through the iterator's own <tt>remove</tt> method, the iterator
	''' will throw a <seealso cref="ConcurrentModificationException"/>.  Thus, in the
	''' face of concurrent modification, the iterator fails quickly and
	''' cleanly, rather than risking arbitrary, non-deterministic behavior
	''' at an undetermined time in the future.
	''' 
	''' <p>Note that the fail-fast behavior of an iterator cannot be guaranteed
	''' as it is, generally speaking, impossible to make any hard guarantees in the
	''' presence of unsynchronized concurrent modification.  Fail-fast iterators
	''' throw <tt>ConcurrentModificationException</tt> on a best-effort basis.
	''' Therefore, it would be wrong to write a program that depended on this
	''' exception for its correctness: <i>fail-fast iterators should be used only
	''' to detect bugs.</i>
	''' 
	''' <p>Implementation note: This is a simple <i>linear-probe</i> hash table,
	''' as described for example in texts by Sedgewick and Knuth.  The array
	''' alternates holding keys and values.  (This has better locality for large
	''' tables than does using separate arrays.)  For many JRE implementations
	''' and operation mixes, this class will yield better performance than
	''' <seealso cref="HashMap"/> (which uses <i>chaining</i> rather than linear-probing).
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' </summary>
	''' <seealso cref=     System#identityHashCode(Object) </seealso>
	''' <seealso cref=     Object#hashCode() </seealso>
	''' <seealso cref=     Collection </seealso>
	''' <seealso cref=     Map </seealso>
	''' <seealso cref=     HashMap </seealso>
	''' <seealso cref=     TreeMap
	''' @author  Doug Lea and Josh Bloch
	''' @since   1.4 </seealso>

	<Serializable> _
	Public Class IdentityHashMap(Of K, V)
		Inherits AbstractMap(Of K, V)
		Implements Map(Of K, V), Cloneable

		''' <summary>
		''' The initial capacity used by the no-args constructor.
		''' MUST be a power of two.  The value 32 corresponds to the
		''' (specified) expected maximum size of 21, given a load factor
		''' of 2/3.
		''' </summary>
		Private Const DEFAULT_CAPACITY As Integer = 32

		''' <summary>
		''' The minimum capacity, used if a lower value is implicitly specified
		''' by either of the constructors with arguments.  The value 4 corresponds
		''' to an expected maximum size of 2, given a load factor of 2/3.
		''' MUST be a power of two.
		''' </summary>
		Private Const MINIMUM_CAPACITY As Integer = 4

		''' <summary>
		''' The maximum capacity, used if a higher value is implicitly specified
		''' by either of the constructors with arguments.
		''' MUST be a power of two <= 1<<29.
		''' 
		''' In fact, the map can hold no more than MAXIMUM_CAPACITY-1 items
		''' because it has to have at least one slot with the key == null
		''' in order to avoid infinite loops in get(), put(), remove()
		''' </summary>
		Private Shared ReadOnly MAXIMUM_CAPACITY As Integer = 1 << 29

		''' <summary>
		''' The table, resized as necessary. Length MUST always be a power of two.
		''' </summary>
		<NonSerialized> _
		Friend table As Object() ' non-private to simplify nested class access

		''' <summary>
		''' The number of key-value mappings contained in this identity hash map.
		''' 
		''' @serial
		''' </summary>
		Friend size_Renamed As Integer

		''' <summary>
		''' The number of modifications, to support fast-fail iterators
		''' </summary>
		<NonSerialized> _
		Friend modCount As Integer

		''' <summary>
		''' Value representing null keys inside tables.
		''' </summary>
		Friend Shared ReadOnly NULL_KEY As New Object

		''' <summary>
		''' Use NULL_KEY for key if it is null.
		''' </summary>
		Private Shared Function maskNull(ByVal key As Object) As Object
			Return (If(key Is Nothing, NULL_KEY, key))
		End Function

		''' <summary>
		''' Returns internal representation of null key back to caller as null.
		''' </summary>
		Friend Shared Function unmaskNull(ByVal key As Object) As Object
			Return (If(key Is NULL_KEY, Nothing, key))
		End Function

		''' <summary>
		''' Constructs a new, empty identity hash map with a default expected
		''' maximum size (21).
		''' </summary>
		Public Sub New()
			init(DEFAULT_CAPACITY)
		End Sub

		''' <summary>
		''' Constructs a new, empty map with the specified expected maximum size.
		''' Putting more than the expected number of key-value mappings into
		''' the map may cause the internal data structure to grow, which may be
		''' somewhat time-consuming.
		''' </summary>
		''' <param name="expectedMaxSize"> the expected maximum size of the map </param>
		''' <exception cref="IllegalArgumentException"> if <tt>expectedMaxSize</tt> is negative </exception>
		Public Sub New(ByVal expectedMaxSize As Integer)
			If expectedMaxSize < 0 Then Throw New IllegalArgumentException("expectedMaxSize is negative: " & expectedMaxSize)
			init(capacity(expectedMaxSize))
		End Sub

		''' <summary>
		''' Returns the appropriate capacity for the given expected maximum size.
		''' Returns the smallest power of two between MINIMUM_CAPACITY and
		''' MAXIMUM_CAPACITY, inclusive, that is greater than (3 *
		''' expectedMaxSize)/2, if such a number exists.  Otherwise returns
		''' MAXIMUM_CAPACITY.
		''' </summary>
		Private Shared Function capacity(ByVal expectedMaxSize As Integer) As Integer
			' assert expectedMaxSize >= 0;
			Return If(expectedMaxSize > MAXIMUM_CAPACITY \ 3, MAXIMUM_CAPACITY, If(expectedMaxSize <= 2 * MINIMUM_CAPACITY \ 3, MINIMUM_CAPACITY,  java.lang.[Integer].highestOneBit(expectedMaxSize + (expectedMaxSize << 1))))
		End Function

		''' <summary>
		''' Initializes object to be an empty map with the specified initial
		''' capacity, which is assumed to be a power of two between
		''' MINIMUM_CAPACITY and MAXIMUM_CAPACITY inclusive.
		''' </summary>
		Private Sub init(ByVal initCapacity As Integer)
			' assert (initCapacity & -initCapacity) == initCapacity; // power of 2
			' assert initCapacity >= MINIMUM_CAPACITY;
			' assert initCapacity <= MAXIMUM_CAPACITY;

			table = New Object(2 * initCapacity - 1){}
		End Sub

		''' <summary>
		''' Constructs a new identity hash map containing the keys-value mappings
		''' in the specified map.
		''' </summary>
		''' <param name="m"> the map whose mappings are to be placed into this map </param>
		''' <exception cref="NullPointerException"> if the specified map is null </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Sub New(Of T1 As K, ? As V)(ByVal m As Map(Of T1))
			' Allow for a bit of growth
			Me.New(CInt(Fix((1 + m.size()) * 1.1)))
			putAll(m)
		End Sub

		''' <summary>
		''' Returns the number of key-value mappings in this identity hash map.
		''' </summary>
		''' <returns> the number of key-value mappings in this map </returns>
		Public Overridable Function size() As Integer Implements Map(Of K, V).size
			Return size_Renamed
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if this identity hash map contains no key-value
		''' mappings.
		''' </summary>
		''' <returns> <tt>true</tt> if this identity hash map contains no key-value
		'''         mappings </returns>
		Public Overridable Property empty As Boolean Implements Map(Of K, V).isEmpty
			Get
				Return size_Renamed = 0
			End Get
		End Property

		''' <summary>
		''' Returns index for Object x.
		''' </summary>
		Private Shared Function hash(ByVal x As Object, ByVal length As Integer) As Integer
			Dim h As Integer = System.identityHashCode(x)
			' Multiply by -127, and left-shift to use least bit as part of hash
			Return ((h << 1) - (h << 8)) And (length - 1)
		End Function

		''' <summary>
		''' Circularly traverses table of size len.
		''' </summary>
		Private Shared Function nextKeyIndex(ByVal i As Integer, ByVal len As Integer) As Integer
			Return (If(i + 2 < len, i + 2, 0))
		End Function

		''' <summary>
		''' Returns the value to which the specified key is mapped,
		''' or {@code null} if this map contains no mapping for the key.
		''' 
		''' <p>More formally, if this map contains a mapping from a key
		''' {@code k} to a value {@code v} such that {@code (key == k)},
		''' then this method returns {@code v}; otherwise it returns
		''' {@code null}.  (There can be at most one such mapping.)
		''' 
		''' <p>A return value of {@code null} does not <i>necessarily</i>
		''' indicate that the map contains no mapping for the key; it's also
		''' possible that the map explicitly maps the key to {@code null}.
		''' The <seealso cref="#containsKey containsKey"/> operation may be used to
		''' distinguish these two cases.
		''' </summary>
		''' <seealso cref= #put(Object, Object) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function [get](ByVal key As Object) As V Implements Map(Of K, V).get
			Dim k As Object = maskNull(key)
			Dim tab As Object() = table
			Dim len As Integer = tab.Length
			Dim i As Integer = hash(k, len)
			Do
				Dim item As Object = tab(i)
				If item Is k Then Return CType(tab(i + 1), V)
				If item Is Nothing Then Return Nothing
				i = nextKeyIndex(i, len)
			Loop
		End Function

		''' <summary>
		''' Tests whether the specified object reference is a key in this identity
		''' hash map.
		''' </summary>
		''' <param name="key">   possible key </param>
		''' <returns>  <code>true</code> if the specified object reference is a key
		'''          in this map </returns>
		''' <seealso cref=     #containsValue(Object) </seealso>
		Public Overridable Function containsKey(ByVal key As Object) As Boolean Implements Map(Of K, V).containsKey
			Dim k As Object = maskNull(key)
			Dim tab As Object() = table
			Dim len As Integer = tab.Length
			Dim i As Integer = hash(k, len)
			Do
				Dim item As Object = tab(i)
				If item Is k Then Return True
				If item Is Nothing Then Return False
				i = nextKeyIndex(i, len)
			Loop
		End Function

		''' <summary>
		''' Tests whether the specified object reference is a value in this identity
		''' hash map.
		''' </summary>
		''' <param name="value"> value whose presence in this map is to be tested </param>
		''' <returns> <tt>true</tt> if this map maps one or more keys to the
		'''         specified object reference </returns>
		''' <seealso cref=     #containsKey(Object) </seealso>
		Public Overridable Function containsValue(ByVal value As Object) As Boolean Implements Map(Of K, V).containsValue
			Dim tab As Object() = table
			For i As Integer = 1 To tab.Length - 1 Step 2
				If tab(i) Is value AndAlso tab(i - 1) IsNot Nothing Then Return True
			Next i

			Return False
		End Function

		''' <summary>
		''' Tests if the specified key-value mapping is in the map.
		''' </summary>
		''' <param name="key">   possible key </param>
		''' <param name="value"> possible value </param>
		''' <returns>  <code>true</code> if and only if the specified key-value
		'''          mapping is in the map </returns>
		Private Function containsMapping(ByVal key As Object, ByVal value As Object) As Boolean
			Dim k As Object = maskNull(key)
			Dim tab As Object() = table
			Dim len As Integer = tab.Length
			Dim i As Integer = hash(k, len)
			Do
				Dim item As Object = tab(i)
				If item Is k Then Return tab(i + 1) Is value
				If item Is Nothing Then Return False
				i = nextKeyIndex(i, len)
			Loop
		End Function

		''' <summary>
		''' Associates the specified value with the specified key in this identity
		''' hash map.  If the map previously contained a mapping for the key, the
		''' old value is replaced.
		''' </summary>
		''' <param name="key"> the key with which the specified value is to be associated </param>
		''' <param name="value"> the value to be associated with the specified key </param>
		''' <returns> the previous value associated with <tt>key</tt>, or
		'''         <tt>null</tt> if there was no mapping for <tt>key</tt>.
		'''         (A <tt>null</tt> return can also indicate that the map
		'''         previously associated <tt>null</tt> with <tt>key</tt>.) </returns>
		''' <seealso cref=     Object#equals(Object) </seealso>
		''' <seealso cref=     #get(Object) </seealso>
		''' <seealso cref=     #containsKey(Object) </seealso>
		Public Overridable Function put(ByVal key As K, ByVal value As V) As V Implements Map(Of K, V).put
			Dim k As Object = maskNull(key)

			retryAfterResize:
			Do
				Dim tab As Object() = table
				Dim len As Integer = tab.Length
				Dim i As Integer = hash(k, len)

				Dim item As Object
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While (item = tab(i)) IsNot Nothing
					If item Is k Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim oldValue As V = CType(tab(i + 1), V)
						tab(i + 1) = value
						Return oldValue
					End If
					i = nextKeyIndex(i, len)
				Loop

				Dim s As Integer = size_Renamed + 1
				' Use optimized form of 3 * s.
				' Next capacity is len, 2 * current capacity.
				If s + (s << 1) > len AndAlso resize(len) Then GoTo retryAfterResize

				modCount += 1
				tab(i) = k
				tab(i + 1) = value
				size_Renamed = s
				Return Nothing
			Loop
		End Function

		''' <summary>
		''' Resizes the table if necessary to hold given capacity.
		''' </summary>
		''' <param name="newCapacity"> the new capacity, must be a power of two. </param>
		''' <returns> whether a resize did in fact take place </returns>
		Private Function resize(ByVal newCapacity As Integer) As Boolean
			' assert (newCapacity & -newCapacity) == newCapacity; // power of 2
			Dim newLength As Integer = newCapacity * 2

			Dim oldTable As Object() = table
			Dim oldLength As Integer = oldTable.Length
			If oldLength = 2 * MAXIMUM_CAPACITY Then ' can't expand any further
				If size_Renamed = MAXIMUM_CAPACITY - 1 Then Throw New IllegalStateException("Capacity exhausted.")
				Return False
			End If
			If oldLength >= newLength Then Return False

			Dim newTable As Object() = New Object(newLength - 1){}

			For j As Integer = 0 To oldLength - 1 Step 2
				Dim key As Object = oldTable(j)
				If key IsNot Nothing Then
					Dim value As Object = oldTable(j+1)
					oldTable(j) = Nothing
					oldTable(j+1) = Nothing
					Dim i As Integer = hash(key, newLength)
					Do While newTable(i) IsNot Nothing
						i = nextKeyIndex(i, newLength)
					Loop
					newTable(i) = key
					newTable(i + 1) = value
				End If
			Next j
			table = newTable
			Return True
		End Function

		''' <summary>
		''' Copies all of the mappings from the specified map to this map.
		''' These mappings will replace any mappings that this map had for
		''' any of the keys currently in the specified map.
		''' </summary>
		''' <param name="m"> mappings to be stored in this map </param>
		''' <exception cref="NullPointerException"> if the specified map is null </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Sub putAll(Of T1 As K, ? As V)(ByVal m As Map(Of T1)) Implements Map(Of K, V).putAll
			Dim n As Integer = m.size()
			If n = 0 Then Return
			If n > size_Renamed Then resize(capacity(n)) ' conservatively pre-expand

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			For Each e As Entry(Of ? As K, ? As V) In m.entrySet()
				put(e.key, e.value)
			Next e
		End Sub

		''' <summary>
		''' Removes the mapping for this key from this map if present.
		''' </summary>
		''' <param name="key"> key whose mapping is to be removed from the map </param>
		''' <returns> the previous value associated with <tt>key</tt>, or
		'''         <tt>null</tt> if there was no mapping for <tt>key</tt>.
		'''         (A <tt>null</tt> return can also indicate that the map
		'''         previously associated <tt>null</tt> with <tt>key</tt>.) </returns>
		Public Overridable Function remove(ByVal key As Object) As V Implements Map(Of K, V).remove
			Dim k As Object = maskNull(key)
			Dim tab As Object() = table
			Dim len As Integer = tab.Length
			Dim i As Integer = hash(k, len)

			Do
				Dim item As Object = tab(i)
				If item Is k Then
					modCount += 1
					size_Renamed -= 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim oldValue As V = CType(tab(i + 1), V)
					tab(i + 1) = Nothing
					tab(i) = Nothing
					closeDeletion(i)
					Return oldValue
				End If
				If item Is Nothing Then Return Nothing
				i = nextKeyIndex(i, len)
			Loop
		End Function

		''' <summary>
		''' Removes the specified key-value mapping from the map if it is present.
		''' </summary>
		''' <param name="key">   possible key </param>
		''' <param name="value"> possible value </param>
		''' <returns>  <code>true</code> if and only if the specified key-value
		'''          mapping was in the map </returns>
		Private Function removeMapping(ByVal key As Object, ByVal value As Object) As Boolean
			Dim k As Object = maskNull(key)
			Dim tab As Object() = table
			Dim len As Integer = tab.Length
			Dim i As Integer = hash(k, len)

			Do
				Dim item As Object = tab(i)
				If item Is k Then
					If tab(i + 1) IsNot value Then Return False
					modCount += 1
					size_Renamed -= 1
					tab(i) = Nothing
					tab(i + 1) = Nothing
					closeDeletion(i)
					Return True
				End If
				If item Is Nothing Then Return False
				i = nextKeyIndex(i, len)
			Loop
		End Function

		''' <summary>
		''' Rehash all possibly-colliding entries following a
		''' deletion. This preserves the linear-probe
		''' collision properties required by get, put, etc.
		''' </summary>
		''' <param name="d"> the index of a newly empty deleted slot </param>
		Private Sub closeDeletion(ByVal d As Integer)
			' Adapted from Knuth Section 6.4 Algorithm R
			Dim tab As Object() = table
			Dim len As Integer = tab.Length

			' Look for items to swap into newly vacated slot
			' starting at index immediately following deletion,
			' and continuing until a null slot is seen, indicating
			' the end of a run of possibly-colliding keys.
			Dim item As Object
			Dim i As Integer = nextKeyIndex(d, len)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While (item = tab(i)) IsNot Nothing
				' The following test triggers if the item at slot i (which
				' hashes to be at slot r) should take the spot vacated by d.
				' If so, we swap it in, and then continue with d now at the
				' newly vacated i.  This process will terminate when we hit
				' the null slot at the end of this run.
				' The test is messy because we are using a circular table.
				Dim r As Integer = hash(item, len)
				If (i < r AndAlso (r <= d OrElse d <= i)) OrElse (r <= d AndAlso d <= i) Then
					tab(d) = item
					tab(d + 1) = tab(i + 1)
					tab(i) = Nothing
					tab(i + 1) = Nothing
					d = i
				End If
				i = nextKeyIndex(i, len)
			Loop
		End Sub

		''' <summary>
		''' Removes all of the mappings from this map.
		''' The map will be empty after this call returns.
		''' </summary>
		Public Overridable Sub clear() Implements Map(Of K, V).clear
			modCount += 1
			Dim tab As Object() = table
			For i As Integer = 0 To tab.Length - 1
				tab(i) = Nothing
			Next i
			size_Renamed = 0
		End Sub

		''' <summary>
		''' Compares the specified object with this map for equality.  Returns
		''' <tt>true</tt> if the given object is also a map and the two maps
		''' represent identical object-reference mappings.  More formally, this
		''' map is equal to another map <tt>m</tt> if and only if
		''' <tt>this.entrySet().equals(m.entrySet())</tt>.
		''' 
		''' <p><b>Owing to the reference-equality-based semantics of this map it is
		''' possible that the symmetry and transitivity requirements of the
		''' <tt>Object.equals</tt> contract may be violated if this map is compared
		''' to a normal map.  However, the <tt>Object.equals</tt> contract is
		''' guaranteed to hold among <tt>IdentityHashMap</tt> instances.</b>
		''' </summary>
		''' <param name="o"> object to be compared for equality with this map </param>
		''' <returns> <tt>true</tt> if the specified object is equal to this map </returns>
		''' <seealso cref= Object#equals(Object) </seealso>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o Is Me Then
				Return True
			ElseIf TypeOf o Is IdentityHashMap Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim m As IdentityHashMap(Of ?, ?) = CType(o, IdentityHashMap(Of ?, ?))
				If m.size() <> size_Renamed Then Return False

				Dim tab As Object() = m.table
				For i As Integer = 0 To tab.Length - 1 Step 2
					Dim k As Object = tab(i)
					If k IsNot Nothing AndAlso (Not containsMapping(k, tab(i + 1))) Then Return False
				Next i
				Return True
			ElseIf TypeOf o Is Map Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim m As Map(Of ?, ?) = CType(o, Map(Of ?, ?))
				Return entrySet().Equals(m.entrySet())
			Else
				Return False ' o is not a Map
			End If
		End Function

		''' <summary>
		''' Returns the hash code value for this map.  The hash code of a map is
		''' defined to be the sum of the hash codes of each entry in the map's
		''' <tt>entrySet()</tt> view.  This ensures that <tt>m1.equals(m2)</tt>
		''' implies that <tt>m1.hashCode()==m2.hashCode()</tt> for any two
		''' <tt>IdentityHashMap</tt> instances <tt>m1</tt> and <tt>m2</tt>, as
		''' required by the general contract of <seealso cref="Object#hashCode"/>.
		''' 
		''' <p><b>Owing to the reference-equality-based semantics of the
		''' <tt>Map.Entry</tt> instances in the set returned by this map's
		''' <tt>entrySet</tt> method, it is possible that the contractual
		''' requirement of <tt>Object.hashCode</tt> mentioned in the previous
		''' paragraph will be violated if one of the two objects being compared is
		''' an <tt>IdentityHashMap</tt> instance and the other is a normal map.</b>
		''' </summary>
		''' <returns> the hash code value for this map </returns>
		''' <seealso cref= Object#equals(Object) </seealso>
		''' <seealso cref= #equals(Object) </seealso>
		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = 0
			Dim tab As Object() = table
			For i As Integer = 0 To tab.Length - 1 Step 2
				Dim key As Object = tab(i)
				If key IsNot Nothing Then
					Dim k As Object = unmaskNull(key)
					result += System.identityHashCode(k) Xor System.identityHashCode(tab(i + 1))
				End If
			Next i
			Return result
		End Function

		''' <summary>
		''' Returns a shallow copy of this identity hash map: the keys and values
		''' themselves are not cloned.
		''' </summary>
		''' <returns> a shallow copy of this map </returns>
		Public Overridable Function clone() As Object
			Try
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim m As IdentityHashMap(Of ?, ?) = CType(MyBase.clone(), IdentityHashMap(Of ?, ?))
				m.entrySet_Renamed = Nothing
				m.table = table.clone()
				Return m
			Catch e As CloneNotSupportedException
				Throw New InternalError(e)
			End Try
		End Function

		Private MustInherit Class IdentityHashMapIterator(Of T)
			Implements Iterator(Of T)

			Private ReadOnly outerInstance As IdentityHashMap

			Public Sub New(ByVal outerInstance As IdentityHashMap)
				Me.outerInstance = outerInstance
			End Sub

			Friend index As Integer = (If(outerInstance.size_Renamed <> 0, 0, outerInstance.table.Length)) ' current slot.
			Friend expectedModCount As Integer = outerInstance.modCount ' to support fast-fail
			Friend lastReturnedIndex As Integer = -1 ' to allow remove()
			Friend indexValid As Boolean ' To avoid unnecessary next computation
			Friend traversalTable As Object() = outerInstance.table ' reference to main table or copy

			Public Overridable Function hasNext() As Boolean Implements Iterator(Of T).hasNext
				Dim tab As Object() = traversalTable
				For i As Integer = index To tab.Length - 1 Step 2
					Dim key As Object = tab(i)
					If key IsNot Nothing Then
						index = i
							indexValid = True
							Return indexValid
					End If
				Next i
				index = tab.Length
				Return False
			End Function

			Protected Friend Overridable Function nextIndex() As Integer
				If outerInstance.modCount <> expectedModCount Then Throw New ConcurrentModificationException
				If (Not indexValid) AndAlso (Not hasNext()) Then Throw New NoSuchElementException

				indexValid = False
				lastReturnedIndex = index
				index += 2
				Return lastReturnedIndex
			End Function

			Public Overridable Sub remove() Implements Iterator(Of T).remove
				If lastReturnedIndex = -1 Then Throw New IllegalStateException
				If outerInstance.modCount <> expectedModCount Then Throw New ConcurrentModificationException

				outerInstance.modCount += 1
				expectedModCount = outerInstance.modCount
				Dim deletedSlot As Integer = lastReturnedIndex
				lastReturnedIndex = -1
				' back up index to revisit new contents after deletion
				index = deletedSlot
				indexValid = False

				' Removal code proceeds as in closeDeletion except that
				' it must catch the rare case where an element already
				' seen is swapped into a vacant slot that will be later
				' traversed by this iterator. We cannot allow future
				' next() calls to return it again.  The likelihood of
				' this occurring under 2/3 load factor is very slim, but
				' when it does happen, we must make a copy of the rest of
				' the table to use for the rest of the traversal. Since
				' this can only happen when we are near the end of the table,
				' even in these rare cases, this is not very expensive in
				' time or space.

				Dim tab As Object() = traversalTable
				Dim len As Integer = tab.Length

				Dim d As Integer = deletedSlot
				Dim key As Object = tab(d)
				tab(d) = Nothing ' vacate the slot
				tab(d + 1) = Nothing

				' If traversing a copy, remove in real table.
				' We can skip gap-closure on copy.
				If tab <> outerInstance.table Then
					outerInstance.remove(key)
					expectedModCount = outerInstance.modCount
					Return
				End If

				outerInstance.size_Renamed -= 1

				Dim item As Object
				Dim i As Integer = nextKeyIndex(d, len)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While (item = tab(i)) IsNot Nothing
					Dim r As Integer = hash(item, len)
					' See closeDeletion for explanation of this conditional
					If (i < r AndAlso (r <= d OrElse d <= i)) OrElse (r <= d AndAlso d <= i) Then

						' If we are about to swap an already-seen element
						' into a slot that may later be returned by next(),
						' then clone the rest of table for use in future
						' next() calls. It is OK that our copy will have
						' a gap in the "wrong" place, since it will never
						' be used for searching anyway.

						If i < deletedSlot AndAlso d >= deletedSlot AndAlso traversalTable = outerInstance.table Then
							Dim remaining As Integer = len - deletedSlot
							Dim newTable As Object() = New Object(remaining - 1){}
							Array.Copy(tab, deletedSlot, newTable, 0, remaining)
							traversalTable = newTable
							index = 0
						End If

						tab(d) = item
						tab(d + 1) = tab(i + 1)
						tab(i) = Nothing
						tab(i + 1) = Nothing
						d = i
					End If
					i = nextKeyIndex(i, len)
				Loop
			End Sub
		End Class

		Private Class KeyIterator
			Inherits IdentityHashMapIterator(Of K)

			Private ReadOnly outerInstance As IdentityHashMap

			Public Sub New(ByVal outerInstance As IdentityHashMap)
				Me.outerInstance = outerInstance
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Function [next]() As K
				Return CType(unmaskNull(traversalTable(nextIndex())), K)
			End Function
		End Class

		Private Class ValueIterator
			Inherits IdentityHashMapIterator(Of V)

			Private ReadOnly outerInstance As IdentityHashMap

			Public Sub New(ByVal outerInstance As IdentityHashMap)
				Me.outerInstance = outerInstance
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Function [next]() As V
				Return CType(traversalTable(nextIndex() + 1), V)
			End Function
		End Class

		Private Class EntryIterator
			Inherits IdentityHashMapIterator(Of KeyValuePair(Of K, V))

			Private ReadOnly outerInstance As IdentityHashMap

			Public Sub New(ByVal outerInstance As IdentityHashMap)
				Me.outerInstance = outerInstance
			End Sub

			Private lastReturnedEntry As Entry

			Public Overridable Function [next]() As KeyValuePair(Of K, V)
				lastReturnedEntry = New Entry(Me, nextIndex())
				Return lastReturnedEntry
			End Function

			Public Overridable Sub remove()
				lastReturnedIndex = (If(Nothing Is lastReturnedEntry, -1, lastReturnedEntry.index))
				MyBase.remove()
				lastReturnedEntry.index = lastReturnedIndex
				lastReturnedEntry = Nothing
			End Sub

			Private Class Entry
				Implements KeyValuePair(Of K, V)

				Private ReadOnly outerInstance As IdentityHashMap.EntryIterator

				Private index As Integer

				Private Sub New(ByVal outerInstance As IdentityHashMap.EntryIterator, ByVal index As Integer)
						Me.outerInstance = outerInstance
					Me.index = index
				End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Public Overridable Property key As K
					Get
						checkIndexForEntryUse()
						Return CType(unmaskNull(outerInstance.traversalTable(index)), K)
					End Get
				End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Public Overridable Property value As V
					Get
						checkIndexForEntryUse()
						Return CType(outerInstance.traversalTable(index+1), V)
					End Get
				End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Public Overridable Function setValue(ByVal value As V) As V
					checkIndexForEntryUse()
					Dim oldValue As V = CType(outerInstance.traversalTable(index+1), V)
					outerInstance.traversalTable(index+1) = value
					' if shadowing, force into main table
					If outerInstance.traversalTable <> outerInstance.table Then put(CType(outerInstance.traversalTable(index), K), value)
					Return oldValue
				End Function

				Public Overrides Function Equals(ByVal o As Object) As Boolean
					If index < 0 Then Return MyBase.Equals(o)

					If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim e As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
					Return (e.Key = unmaskNull(outerInstance.traversalTable(index)) AndAlso e.Value Is outerInstance.traversalTable(index+1))
				End Function

				Public Overrides Function GetHashCode() As Integer
					If outerInstance.lastReturnedIndex < 0 Then Return MyBase.GetHashCode()

					Return (System.identityHashCode(unmaskNull(outerInstance.traversalTable(index))) Xor System.identityHashCode(outerInstance.traversalTable(index+1)))
				End Function

				Public Overrides Function ToString() As String
					If index < 0 Then Return MyBase.ToString()

					Return (unmaskNull(outerInstance.traversalTable(index)) & "=" & outerInstance.traversalTable(index+1))
				End Function

				Private Sub checkIndexForEntryUse()
					If index < 0 Then Throw New IllegalStateException("Entry was removed")
				End Sub
			End Class
		End Class

		' Views

		''' <summary>
		''' This field is initialized to contain an instance of the entry set
		''' view the first time this view is requested.  The view is stateless,
		''' so there's no reason to create more than one.
		''' </summary>
		<NonSerialized> _
		Private entrySet_Renamed As [Set](Of KeyValuePair(Of K, V))

		''' <summary>
		''' Returns an identity-based set view of the keys contained in this map.
		''' The set is backed by the map, so changes to the map are reflected in
		''' the set, and vice-versa.  If the map is modified while an iteration
		''' over the set is in progress, the results of the iteration are
		''' undefined.  The set supports element removal, which removes the
		''' corresponding mapping from the map, via the <tt>Iterator.remove</tt>,
		''' <tt>Set.remove</tt>, <tt>removeAll</tt>, <tt>retainAll</tt>, and
		''' <tt>clear</tt> methods.  It does not support the <tt>add</tt> or
		''' <tt>addAll</tt> methods.
		''' 
		''' <p><b>While the object returned by this method implements the
		''' <tt>Set</tt> interface, it does <i>not</i> obey <tt>Set's</tt> general
		''' contract.  Like its backing map, the set returned by this method
		''' defines element equality as reference-equality rather than
		''' object-equality.  This affects the behavior of its <tt>contains</tt>,
		''' <tt>remove</tt>, <tt>containsAll</tt>, <tt>equals</tt>, and
		''' <tt>hashCode</tt> methods.</b>
		''' 
		''' <p><b>The <tt>equals</tt> method of the returned set returns <tt>true</tt>
		''' only if the specified object is a set containing exactly the same
		''' object references as the returned set.  The symmetry and transitivity
		''' requirements of the <tt>Object.equals</tt> contract may be violated if
		''' the set returned by this method is compared to a normal set.  However,
		''' the <tt>Object.equals</tt> contract is guaranteed to hold among sets
		''' returned by this method.</b>
		''' 
		''' <p>The <tt>hashCode</tt> method of the returned set returns the sum of
		''' the <i>identity hashcodes</i> of the elements in the set, rather than
		''' the sum of their hashcodes.  This is mandated by the change in the
		''' semantics of the <tt>equals</tt> method, in order to enforce the
		''' general contract of the <tt>Object.hashCode</tt> method among sets
		''' returned by this method.
		''' </summary>
		''' <returns> an identity-based set view of the keys contained in this map </returns>
		''' <seealso cref= Object#equals(Object) </seealso>
		''' <seealso cref= System#identityHashCode(Object) </seealso>
		Public Overridable Function keySet() As [Set](Of K) Implements Map(Of K, V).keySet
			Dim ks As [Set](Of K) = keySet_Renamed
			If ks IsNot Nothing Then
				Return ks
			Else
					keySet_Renamed = New KeySet(Me, Me)
					Return keySet_Renamed
			End If
		End Function

		Private Class KeySet
			Inherits AbstractSet(Of K)

			Private ReadOnly outerInstance As IdentityHashMap

			Public Sub New(ByVal outerInstance As IdentityHashMap)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Function [iterator]() As [Iterator](Of K)
				Return New KeyIterator
			End Function
			Public Overridable Function size() As Integer
				Return outerInstance.size_Renamed
			End Function
			Public Overridable Function contains(ByVal o As Object) As Boolean
				Return outerInstance.containsKey(o)
			End Function
			Public Overridable Function remove(ByVal o As Object) As Boolean
				Dim oldSize As Integer = outerInstance.size_Renamed
				outerInstance.remove(o)
				Return outerInstance.size_Renamed <> oldSize
			End Function
	'        
	'         * Must revert from AbstractSet's impl to AbstractCollection's, as
	'         * the former contains an optimization that results in incorrect
	'         * behavior when c is a smaller "normal" (non-identity-based) Set.
	'         
			Public Overridable Function removeAll(Of T1)(ByVal c As Collection(Of T1)) As Boolean
				Objects.requireNonNull(c)
				Dim modified As Boolean = False
				Dim i As [Iterator](Of K) = [iterator]()
				Do While i.MoveNext()
					If c.contains(i.Current) Then
						i.remove()
						modified = True
					End If
				Loop
				Return modified
			End Function
			Public Overridable Sub clear()
				outerInstance.clear()
			End Sub
			Public Overrides Function GetHashCode() As Integer
				Dim result As Integer = 0
				For Each key As K In Me
					result += System.identityHashCode(key)
				Next key
				Return result
			End Function
			Public Overridable Function toArray() As Object()
				Return ToArray(New Object(){})
			End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Function toArray(Of T)(ByVal a As T()) As T()
				Dim expectedModCount As Integer = outerInstance.modCount
				Dim size As Integer = size()
				If a.Length < size Then a = CType(Array.newInstance(a.GetType().GetElementType(), size), T())
				Dim tab As Object() = outerInstance.table
				Dim ti As Integer = 0
				For si As Integer = 0 To tab.Length - 1 Step 2
					Dim key As Object
					key = tab(si)
					If key IsNot Nothing Then ' key present ?
						' more elements than expected -> concurrent modification from other thread
						If ti >= size Then Throw New ConcurrentModificationException
						a(ti) = CType(unmaskNull(key), T)
						ti += 1
					End If
				Next si
				' fewer elements than expected or concurrent modification from other thread detected
				If ti < size OrElse expectedModCount <> outerInstance.modCount Then Throw New ConcurrentModificationException
				' final null marker as per spec
				If ti < a.Length Then a(ti) = Nothing
				Return a
			End Function

			Public Overridable Function spliterator() As Spliterator(Of K)
				Return New KeySpliterator(Of )(IdentityHashMap.this, 0, -1, 0, 0)
			End Function
		End Class

		''' <summary>
		''' Returns a <seealso cref="Collection"/> view of the values contained in this map.
		''' The collection is backed by the map, so changes to the map are
		''' reflected in the collection, and vice-versa.  If the map is
		''' modified while an iteration over the collection is in progress,
		''' the results of the iteration are undefined.  The collection
		''' supports element removal, which removes the corresponding
		''' mapping from the map, via the <tt>Iterator.remove</tt>,
		''' <tt>Collection.remove</tt>, <tt>removeAll</tt>,
		''' <tt>retainAll</tt> and <tt>clear</tt> methods.  It does not
		''' support the <tt>add</tt> or <tt>addAll</tt> methods.
		''' 
		''' <p><b>While the object returned by this method implements the
		''' <tt>Collection</tt> interface, it does <i>not</i> obey
		''' <tt>Collection's</tt> general contract.  Like its backing map,
		''' the collection returned by this method defines element equality as
		''' reference-equality rather than object-equality.  This affects the
		''' behavior of its <tt>contains</tt>, <tt>remove</tt> and
		''' <tt>containsAll</tt> methods.</b>
		''' </summary>
		Public Overridable Function values() As Collection(Of V) Implements Map(Of K, V).values
			Dim vs As Collection(Of V) = values_Renamed
			If vs IsNot Nothing Then
				Return vs
			Else
					values_Renamed = New Values(Me, Me)
					Return values_Renamed
			End If
		End Function

		Private Class Values
			Inherits AbstractCollection(Of V)

			Private ReadOnly outerInstance As IdentityHashMap

			Public Sub New(ByVal outerInstance As IdentityHashMap)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Function [iterator]() As [Iterator](Of V)
				Return New ValueIterator
			End Function
			Public Overridable Function size() As Integer
				Return outerInstance.size_Renamed
			End Function
			Public Overridable Function contains(ByVal o As Object) As Boolean
				Return outerInstance.containsValue(o)
			End Function
			Public Overridable Function remove(ByVal o As Object) As Boolean
				Dim i As [Iterator](Of V) = [iterator]()
				Do While i.MoveNext()
					If i.Current Is o Then
						i.remove()
						Return True
					End If
				Loop
				Return False
			End Function
			Public Overridable Sub clear()
				outerInstance.clear()
			End Sub
			Public Overridable Function toArray() As Object()
				Return ToArray(New Object(){})
			End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Function toArray(Of T)(ByVal a As T()) As T()
				Dim expectedModCount As Integer = outerInstance.modCount
				Dim size As Integer = size()
				If a.Length < size Then a = CType(Array.newInstance(a.GetType().GetElementType(), size), T())
				Dim tab As Object() = outerInstance.table
				Dim ti As Integer = 0
				For si As Integer = 0 To tab.Length - 1 Step 2
					If tab(si) IsNot Nothing Then ' key present ?
						' more elements than expected -> concurrent modification from other thread
						If ti >= size Then Throw New ConcurrentModificationException
						a(ti) = CType(tab(si+1), T)
						ti += 1
					End If
				Next si
				' fewer elements than expected or concurrent modification from other thread detected
				If ti < size OrElse expectedModCount <> outerInstance.modCount Then Throw New ConcurrentModificationException
				' final null marker as per spec
				If ti < a.Length Then a(ti) = Nothing
				Return a
			End Function

			Public Overridable Function spliterator() As Spliterator(Of V)
				Return New ValueSpliterator(Of )(IdentityHashMap.this, 0, -1, 0, 0)
			End Function
		End Class

		''' <summary>
		''' Returns a <seealso cref="Set"/> view of the mappings contained in this map.
		''' Each element in the returned set is a reference-equality-based
		''' <tt>Map.Entry</tt>.  The set is backed by the map, so changes
		''' to the map are reflected in the set, and vice-versa.  If the
		''' map is modified while an iteration over the set is in progress,
		''' the results of the iteration are undefined.  The set supports
		''' element removal, which removes the corresponding mapping from
		''' the map, via the <tt>Iterator.remove</tt>, <tt>Set.remove</tt>,
		''' <tt>removeAll</tt>, <tt>retainAll</tt> and <tt>clear</tt>
		''' methods.  It does not support the <tt>add</tt> or
		''' <tt>addAll</tt> methods.
		''' 
		''' <p>Like the backing map, the <tt>Map.Entry</tt> objects in the set
		''' returned by this method define key and value equality as
		''' reference-equality rather than object-equality.  This affects the
		''' behavior of the <tt>equals</tt> and <tt>hashCode</tt> methods of these
		''' <tt>Map.Entry</tt> objects.  A reference-equality based <tt>Map.Entry
		''' e</tt> is equal to an object <tt>o</tt> if and only if <tt>o</tt> is a
		''' <tt>Map.Entry</tt> and <tt>e.getKey()==o.getKey() &amp;&amp;
		''' e.getValue()==o.getValue()</tt>.  To accommodate these equals
		''' semantics, the <tt>hashCode</tt> method returns
		''' <tt>System.identityHashCode(e.getKey()) ^
		''' System.identityHashCode(e.getValue())</tt>.
		''' 
		''' <p><b>Owing to the reference-equality-based semantics of the
		''' <tt>Map.Entry</tt> instances in the set returned by this method,
		''' it is possible that the symmetry and transitivity requirements of
		''' the <seealso cref="Object#equals(Object)"/> contract may be violated if any of
		''' the entries in the set is compared to a normal map entry, or if
		''' the set returned by this method is compared to a set of normal map
		''' entries (such as would be returned by a call to this method on a normal
		''' map).  However, the <tt>Object.equals</tt> contract is guaranteed to
		''' hold among identity-based map entries, and among sets of such entries.
		''' </b>
		''' </summary>
		''' <returns> a set view of the identity-mappings contained in this map </returns>
		Public Overridable Function entrySet() As [Set](Of KeyValuePair(Of K, V)) Implements Map(Of K, V).entrySet
			Dim es As [Set](Of KeyValuePair(Of K, V)) = entrySet_Renamed
			If es IsNot Nothing Then
				Return es
			Else
					entrySet_Renamed = New EntrySet(Me, Me)
					Return entrySet_Renamed
			End If
		End Function

		Private Class EntrySet
			Inherits AbstractSet(Of KeyValuePair(Of K, V))

			Private ReadOnly outerInstance As IdentityHashMap

			Public Sub New(ByVal outerInstance As IdentityHashMap)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Function [iterator]() As [Iterator](Of KeyValuePair(Of K, V))
				Return New EntryIterator
			End Function
			Public Overridable Function contains(ByVal o As Object) As Boolean
				If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim entry As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
				Return outerInstance.containsMapping(entry.Key, entry.Value)
			End Function
			Public Overridable Function remove(ByVal o As Object) As Boolean
				If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim entry As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
				Return outerInstance.removeMapping(entry.Key, entry.Value)
			End Function
			Public Overridable Function size() As Integer
				Return outerInstance.size_Renamed
			End Function
			Public Overridable Sub clear()
				outerInstance.clear()
			End Sub
	'        
	'         * Must revert from AbstractSet's impl to AbstractCollection's, as
	'         * the former contains an optimization that results in incorrect
	'         * behavior when c is a smaller "normal" (non-identity-based) Set.
	'         
			Public Overridable Function removeAll(Of T1)(ByVal c As Collection(Of T1)) As Boolean
				Objects.requireNonNull(c)
				Dim modified As Boolean = False
				Dim i As [Iterator](Of KeyValuePair(Of K, V)) = [iterator]()
				Do While i.MoveNext()
					If c.contains(i.Current) Then
						i.remove()
						modified = True
					End If
				Loop
				Return modified
			End Function

			Public Overridable Function toArray() As Object()
				Return ToArray(New Object(){})
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Function toArray(Of T)(ByVal a As T()) As T()
				Dim expectedModCount As Integer = outerInstance.modCount
				Dim size As Integer = size()
				If a.Length < size Then a = CType(Array.newInstance(a.GetType().GetElementType(), size), T())
				Dim tab As Object() = outerInstance.table
				Dim ti As Integer = 0
				For si As Integer = 0 To tab.Length - 1 Step 2
					Dim key As Object
					key = tab(si)
					If key IsNot Nothing Then ' key present ?
						' more elements than expected -> concurrent modification from other thread
						If ti >= size Then Throw New ConcurrentModificationException
						a(ti) = CType(New AbstractMap.SimpleEntry(Of )(unmaskNull(key), tab(si + 1)), T)
						ti += 1
					End If
				Next si
				' fewer elements than expected or concurrent modification from other thread detected
				If ti < size OrElse expectedModCount <> outerInstance.modCount Then Throw New ConcurrentModificationException
				' final null marker as per spec
				If ti < a.Length Then a(ti) = Nothing
				Return a
			End Function

			Public Overridable Function spliterator() As Spliterator(Of KeyValuePair(Of K, V))
				Return New EntrySpliterator(Of )(IdentityHashMap.this, 0, -1, 0, 0)
			End Function
		End Class


		Private Const serialVersionUID As Long = 8188218128353913216L

		''' <summary>
		''' Saves the state of the <tt>IdentityHashMap</tt> instance to a stream
		''' (i.e., serializes it).
		''' 
		''' @serialData The <i>size</i> of the HashMap (the number of key-value
		'''          mappings) (<tt>int</tt>), followed by the key (Object) and
		'''          value (Object) for each key-value mapping represented by the
		'''          IdentityHashMap.  The key-value mappings are emitted in no
		'''          particular order.
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			' Write out and any hidden stuff
			s.defaultWriteObject()

			' Write out size (number of Mappings)
			s.writeInt(size_Renamed)

			' Write out keys and values (alternating)
			Dim tab As Object() = table
			For i As Integer = 0 To tab.Length - 1 Step 2
				Dim key As Object = tab(i)
				If key IsNot Nothing Then
					s.writeObject(unmaskNull(key))
					s.writeObject(tab(i + 1))
				End If
			Next i
		End Sub

		''' <summary>
		''' Reconstitutes the <tt>IdentityHashMap</tt> instance from a stream (i.e.,
		''' deserializes it).
		''' </summary>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			' Read in any hidden stuff
			s.defaultReadObject()

			' Read in size (number of Mappings)
			Dim size As Integer = s.readInt()
			If size < 0 Then Throw New java.io.StreamCorruptedException("Illegal mappings count: " & size)
			init(capacity(size))

			' Read the keys and values, and put the mappings in the table
			For i As Integer = 0 To size - 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim key As K = CType(s.readObject(), K)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim value As V = CType(s.readObject(), V)
				putForCreate(key, value)
			Next i
		End Sub

		''' <summary>
		''' The put method for readObject.  It does not resize the table,
		''' update modCount, etc.
		''' </summary>
		Private Sub putForCreate(ByVal key As K, ByVal value As V)
			Dim k As Object = maskNull(key)
			Dim tab As Object() = table
			Dim len As Integer = tab.Length
			Dim i As Integer = hash(k, len)

			Dim item As Object
			item = tab(i)
			Do While item IsNot Nothing
				If item Is k Then Throw New java.io.StreamCorruptedException
				i = nextKeyIndex(i, len)
				item = tab(i)
			Loop
			tab(i) = k
			tab(i + 1) = value
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Sub forEach(Of T1)(ByVal action As java.util.function.BiConsumer(Of T1)) Implements Map(Of K, V).forEach
			Objects.requireNonNull(action)
			Dim expectedModCount As Integer = modCount

			Dim t As Object() = table
			For index As Integer = 0 To t.Length - 1 Step 2
				Dim k As Object = t(index)
				If k IsNot Nothing Then action.accept(CType(unmaskNull(k), K), CType(t(index + 1), V))

				If modCount <> expectedModCount Then Throw New ConcurrentModificationException
			Next index
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Sub replaceAll(Of T1 As V)(ByVal [function] As java.util.function.BiFunction(Of T1)) Implements Map(Of K, V).replaceAll
			Objects.requireNonNull([function])
			Dim expectedModCount As Integer = modCount

			Dim t As Object() = table
			For index As Integer = 0 To t.Length - 1 Step 2
				Dim k As Object = t(index)
				If k IsNot Nothing Then t(index + 1) = [function].apply(CType(unmaskNull(k), K), CType(t(index + 1), V))

				If modCount <> expectedModCount Then Throw New ConcurrentModificationException
			Next index
		End Sub

		''' <summary>
		''' Similar form as array-based Spliterators, but skips blank elements,
		''' and guestimates size as decreasing by half per split.
		''' </summary>
		Friend Class IdentityHashMapSpliterator(Of K, V)
			Friend ReadOnly map As IdentityHashMap(Of K, V)
			Friend index As Integer ' current index, modified on advance/split
			Friend fence As Integer ' -1 until first use; then one past last index
			Friend est As Integer ' size estimate
			Friend expectedModCount As Integer ' initialized when fence set

			Friend Sub New(ByVal map As IdentityHashMap(Of K, V), ByVal origin As Integer, ByVal fence As Integer, ByVal est As Integer, ByVal expectedModCount As Integer)
				Me.map = map
				Me.index = origin
				Me.fence = fence
				Me.est = est
				Me.expectedModCount = expectedModCount
			End Sub

			Friend Property fence As Integer
				Get
					Dim hi As Integer
					hi = fence
					If hi < 0 Then
						est = map.size_Renamed
						expectedModCount = map.modCount
							fence = map.table.Length
							hi = fence
					End If
					Return hi
				End Get
			End Property

			Public Function estimateSize() As Long
				fence ' force init
				Return CLng(est)
			End Function
		End Class

		Friend NotInheritable Class KeySpliterator(Of K, V)
			Inherits IdentityHashMapSpliterator(Of K, V)
			Implements Spliterator(Of K)

			Friend Sub New(ByVal map As IdentityHashMap(Of K, V), ByVal origin As Integer, ByVal fence As Integer, ByVal est As Integer, ByVal expectedModCount As Integer)
				MyBase.New(map, origin, fence, est, expectedModCount)
			End Sub

			Public Function trySplit() As KeySpliterator(Of K, V)
				Dim hi As Integer = fence, lo As Integer = index, mid As Integer = (CInt(CUInt((lo + hi)) >> 1)) And Not 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If(lo >= mid, Nothing, New KeySpliterator(Of K, V)(map, lo, index = mid, est >>>= 1, expectedModCount))
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Spliterator(Of K).forEachRemaining
				If action Is Nothing Then Throw New NullPointerException
				Dim i, hi, mc As Integer
				Dim key As Object
				Dim m As IdentityHashMap(Of K, V)
				Dim a As Object()
				m = map
				a = m.table
				i = index
				hi = fence
				index = hi
				If m IsNot Nothing AndAlso a IsNot Nothing AndAlso i >= 0 AndAlso index <= a.Length Then
					Do While i < hi
						key = a(i)
						If key IsNot Nothing Then action.accept(CType(unmaskNull(key), K))
						i += 2
					Loop
					If m.modCount = expectedModCount Then Return
				End If
				Throw New ConcurrentModificationException
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of K).tryAdvance
				If action Is Nothing Then Throw New NullPointerException
				Dim a As Object() = map.table
				Dim hi As Integer = fence
				Do While index < hi
					Dim key As Object = a(index)
					index += 2
					If key IsNot Nothing Then
						action.accept(CType(unmaskNull(key), K))
						If map.modCount <> expectedModCount Then Throw New ConcurrentModificationException
						Return True
					End If
				Loop
				Return False
			End Function

			Public Function characteristics() As Integer Implements Spliterator(Of K).characteristics
				Return (If(fence < 0 OrElse est = map.size_Renamed, SIZED, 0)) Or Spliterator.DISTINCT
			End Function
		End Class

		Friend NotInheritable Class ValueSpliterator(Of K, V)
			Inherits IdentityHashMapSpliterator(Of K, V)
			Implements Spliterator(Of V)

			Friend Sub New(ByVal m As IdentityHashMap(Of K, V), ByVal origin As Integer, ByVal fence As Integer, ByVal est As Integer, ByVal expectedModCount As Integer)
				MyBase.New(m, origin, fence, est, expectedModCount)
			End Sub

			Public Function trySplit() As ValueSpliterator(Of K, V)
				Dim hi As Integer = fence, lo As Integer = index, mid As Integer = (CInt(CUInt((lo + hi)) >> 1)) And Not 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If(lo >= mid, Nothing, New ValueSpliterator(Of K, V)(map, lo, index = mid, est >>>= 1, expectedModCount))
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Spliterator(Of V).forEachRemaining
				If action Is Nothing Then Throw New NullPointerException
				Dim i, hi, mc As Integer
				Dim m As IdentityHashMap(Of K, V)
				Dim a As Object()
				m = map
				a = m.table
				i = index
				hi = fence
				index = hi
				If m IsNot Nothing AndAlso a IsNot Nothing AndAlso i >= 0 AndAlso index <= a.Length Then
					Do While i < hi
						If a(i) IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
							Dim v As V = CType(a(i+1), V)
							action.accept(v)
						End If
						i += 2
					Loop
					If m.modCount = expectedModCount Then Return
				End If
				Throw New ConcurrentModificationException
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of V).tryAdvance
				If action Is Nothing Then Throw New NullPointerException
				Dim a As Object() = map.table
				Dim hi As Integer = fence
				Do While index < hi
					Dim key As Object = a(index)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim v As V = CType(a(index+1), V)
					index += 2
					If key IsNot Nothing Then
						action.accept(v)
						If map.modCount <> expectedModCount Then Throw New ConcurrentModificationException
						Return True
					End If
				Loop
				Return False
			End Function

			Public Function characteristics() As Integer Implements Spliterator(Of V).characteristics
				Return (If(fence < 0 OrElse est = map.size_Renamed, SIZED, 0))
			End Function

		End Class

		Friend NotInheritable Class EntrySpliterator(Of K, V)
			Inherits IdentityHashMapSpliterator(Of K, V)
			Implements Spliterator(Of KeyValuePair(Of K, V))

			Friend Sub New(ByVal m As IdentityHashMap(Of K, V), ByVal origin As Integer, ByVal fence As Integer, ByVal est As Integer, ByVal expectedModCount As Integer)
				MyBase.New(m, origin, fence, est, expectedModCount)
			End Sub

			Public Function trySplit() As EntrySpliterator(Of K, V)
				Dim hi As Integer = fence, lo As Integer = index, mid As Integer = (CInt(CUInt((lo + hi)) >> 1)) And Not 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If(lo >= mid, Nothing, New EntrySpliterator(Of K, V)(map, lo, index = mid, est >>>= 1, expectedModCount))
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Spliterator(Of KeyValuePair(Of K, V)).forEachRemaining
				If action Is Nothing Then Throw New NullPointerException
				Dim i, hi, mc As Integer
				Dim m As IdentityHashMap(Of K, V)
				Dim a As Object()
				m = map
				a = m.table
				i = index
				hi = fence
				index = hi
				If m IsNot Nothing AndAlso a IsNot Nothing AndAlso i >= 0 AndAlso index <= a.Length Then
					Do While i < hi
						Dim key As Object = a(i)
						If key IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
							Dim k As K = CType(unmaskNull(key), K)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
							Dim v As V = CType(a(i+1), V)
							action.accept(New AbstractMap.SimpleImmutableEntry(Of K, V)(k, v))

						End If
						i += 2
					Loop
					If m.modCount = expectedModCount Then Return
				End If
				Throw New ConcurrentModificationException
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of KeyValuePair(Of K, V)).tryAdvance
				If action Is Nothing Then Throw New NullPointerException
				Dim a As Object() = map.table
				Dim hi As Integer = fence
				Do While index < hi
					Dim key As Object = a(index)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim v As V = CType(a(index+1), V)
					index += 2
					If key IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim k As K = CType(unmaskNull(key), K)
						action.accept(New AbstractMap.SimpleImmutableEntry(Of K, V)(k, v))
						If map.modCount <> expectedModCount Then Throw New ConcurrentModificationException
						Return True
					End If
				Loop
				Return False
			End Function

			Public Function characteristics() As Integer Implements Spliterator(Of KeyValuePair(Of K, V)).characteristics
				Return (If(fence < 0 OrElse est = map.size_Renamed, SIZED, 0)) Or Spliterator.DISTINCT
			End Function
		End Class

	End Class

End Namespace
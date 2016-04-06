Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
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
	''' Hash table based implementation of the <tt>Map</tt> interface, with
	''' <em>weak keys</em>.
	''' An entry in a <tt>WeakHashMap</tt> will automatically be removed when
	''' its key is no longer in ordinary use.  More precisely, the presence of a
	''' mapping for a given key will not prevent the key from being discarded by the
	''' garbage collector, that is, made finalizable, finalized, and then reclaimed.
	''' When a key has been discarded its entry is effectively removed from the map,
	''' so this class behaves somewhat differently from other <tt>Map</tt>
	''' implementations.
	''' 
	''' <p> Both null values and the null key are supported. This class has
	''' performance characteristics similar to those of the <tt>HashMap</tt>
	''' [Class], and has the same efficiency parameters of <em>initial capacity</em>
	''' and <em>load factor</em>.
	''' 
	''' <p> Like most collection classes, this class is not synchronized.
	''' A synchronized <tt>WeakHashMap</tt> may be constructed using the
	''' <seealso cref="Collections#synchronizedMap Collections.synchronizedMap"/>
	''' method.
	''' 
	''' <p> This class is intended primarily for use with key objects whose
	''' <tt>equals</tt> methods test for object identity using the
	''' <tt>==</tt> operator.  Once such a key is discarded it can never be
	''' recreated, so it is impossible to do a lookup of that key in a
	''' <tt>WeakHashMap</tt> at some later time and be surprised that its entry
	''' has been removed.  This class will work perfectly well with key objects
	''' whose <tt>equals</tt> methods are not based upon object identity, such
	''' as <tt>String</tt> instances.  With such recreatable key objects,
	''' however, the automatic removal of <tt>WeakHashMap</tt> entries whose
	''' keys have been discarded may prove to be confusing.
	''' 
	''' <p> The behavior of the <tt>WeakHashMap</tt> class depends in part upon
	''' the actions of the garbage collector, so several familiar (though not
	''' required) <tt>Map</tt> invariants do not hold for this class.  Because
	''' the garbage collector may discard keys at any time, a
	''' <tt>WeakHashMap</tt> may behave as though an unknown thread is silently
	''' removing entries.  In particular, even if you synchronize on a
	''' <tt>WeakHashMap</tt> instance and invoke none of its mutator methods, it
	''' is possible for the <tt>size</tt> method to return smaller values over
	''' time, for the <tt>isEmpty</tt> method to return <tt>false</tt> and
	''' then <tt>true</tt>, for the <tt>containsKey</tt> method to return
	''' <tt>true</tt> and later <tt>false</tt> for a given key, for the
	''' <tt>get</tt> method to return a value for a given key but later return
	''' <tt>null</tt>, for the <tt>put</tt> method to return
	''' <tt>null</tt> and the <tt>remove</tt> method to return
	''' <tt>false</tt> for a key that previously appeared to be in the map, and
	''' for successive examinations of the key set, the value collection, and
	''' the entry set to yield successively smaller numbers of elements.
	''' 
	''' <p> Each key object in a <tt>WeakHashMap</tt> is stored indirectly as
	''' the referent of a weak reference.  Therefore a key will automatically be
	''' removed only after the weak references to it, both inside and outside of the
	''' map, have been cleared by the garbage collector.
	''' 
	''' <p> <strong>Implementation note:</strong> The value objects in a
	''' <tt>WeakHashMap</tt> are held by ordinary strong references.  Thus care
	''' should be taken to ensure that value objects do not strongly refer to their
	''' own keys, either directly or indirectly, since that will prevent the keys
	''' from being discarded.  Note that a value object may refer indirectly to its
	''' key via the <tt>WeakHashMap</tt> itself; that is, a value object may
	''' strongly refer to some other key object whose associated value object, in
	''' turn, strongly refers to the key of the first value object.  If the values
	''' in the map do not rely on the map holding strong references to them, one way
	''' to deal with this is to wrap values themselves within
	''' <tt>WeakReferences</tt> before
	''' inserting, as in: <tt>m.put(key, new WeakReference(value))</tt>,
	''' and then unwrapping upon each <tt>get</tt>.
	''' 
	''' <p>The iterators returned by the <tt>iterator</tt> method of the collections
	''' returned by all of this class's "collection view methods" are
	''' <i>fail-fast</i>: if the map is structurally modified at any time after the
	''' iterator is created, in any way except through the iterator's own
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
	''' exception for its correctness:  <i>the fail-fast behavior of iterators
	''' should be used only to detect bugs.</i>
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' </summary>
	''' @param <K> the type of keys maintained by this map </param>
	''' @param <V> the type of mapped values
	''' 
	''' @author      Doug Lea
	''' @author      Josh Bloch
	''' @author      Mark Reinhold
	''' @since       1.2 </param>
	''' <seealso cref=         java.util.HashMap </seealso>
	''' <seealso cref=         java.lang.ref.WeakReference </seealso>
	Public Class WeakHashMap(Of K, V)
		Inherits AbstractMap(Of K, V)
		Implements Map(Of K, V)

		''' <summary>
		''' The default initial capacity -- MUST be a power of two.
		''' </summary>
		Private Const DEFAULT_INITIAL_CAPACITY As Integer = 16

		''' <summary>
		''' The maximum capacity, used if a higher value is implicitly specified
		''' by either of the constructors with arguments.
		''' MUST be a power of two <= 1<<30.
		''' </summary>
		Private Shared ReadOnly MAXIMUM_CAPACITY As Integer = 1 << 30

		''' <summary>
		''' The load factor used when none specified in constructor.
		''' </summary>
		Private Const DEFAULT_LOAD_FACTOR As Single = 0.75f

		''' <summary>
		''' The table, resized as necessary. Length MUST Always be a power of two.
		''' </summary>
		Friend table As Entry(Of K, V)()

		''' <summary>
		''' The number of key-value mappings contained in this weak hash map.
		''' </summary>
		Private size_Renamed As Integer

		''' <summary>
		''' The next size value at which to resize (capacity * load factor).
		''' </summary>
		Private threshold As Integer

		''' <summary>
		''' The load factor for the hash table.
		''' </summary>
		Private ReadOnly loadFactor As Single

		''' <summary>
		''' Reference queue for cleared WeakEntries
		''' </summary>
		Private ReadOnly queue As New ReferenceQueue(Of Object)

		''' <summary>
		''' The number of times this WeakHashMap has been structurally modified.
		''' Structural modifications are those that change the number of
		''' mappings in the map or otherwise modify its internal structure
		''' (e.g., rehash).  This field is used to make iterators on
		''' Collection-views of the map fail-fast.
		''' </summary>
		''' <seealso cref= ConcurrentModificationException </seealso>
		Friend modCount As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Function newTable(  n As Integer) As Entry(Of K, V)()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Return CType(New Entry(Of ?, ?)(n - 1){}, Entry(Of K, V)())
		End Function

		''' <summary>
		''' Constructs a new, empty <tt>WeakHashMap</tt> with the given initial
		''' capacity and the given load factor.
		''' </summary>
		''' <param name="initialCapacity"> The initial capacity of the <tt>WeakHashMap</tt> </param>
		''' <param name="loadFactor">      The load factor of the <tt>WeakHashMap</tt> </param>
		''' <exception cref="IllegalArgumentException"> if the initial capacity is negative,
		'''         or if the load factor is nonpositive. </exception>
		Public Sub New(  initialCapacity As Integer,   loadFactor As Single)
			If initialCapacity < 0 Then Throw New IllegalArgumentException("Illegal Initial Capacity: " & initialCapacity)
			If initialCapacity > MAXIMUM_CAPACITY Then initialCapacity = MAXIMUM_CAPACITY

			If loadFactor <= 0 OrElse Float.IsNaN(loadFactor) Then Throw New IllegalArgumentException("Illegal Load factor: " & loadFactor)
			Dim capacity As Integer = 1
			Do While capacity < initialCapacity
				capacity <<= 1
			Loop
			table = newTable(capacity)
			Me.loadFactor = loadFactor
			threshold = CInt(Fix(capacity * loadFactor))
		End Sub

		''' <summary>
		''' Constructs a new, empty <tt>WeakHashMap</tt> with the given initial
		''' capacity and the default load factor (0.75).
		''' </summary>
		''' <param name="initialCapacity"> The initial capacity of the <tt>WeakHashMap</tt> </param>
		''' <exception cref="IllegalArgumentException"> if the initial capacity is negative </exception>
		Public Sub New(  initialCapacity As Integer)
			Me.New(initialCapacity, DEFAULT_LOAD_FACTOR)
		End Sub

		''' <summary>
		''' Constructs a new, empty <tt>WeakHashMap</tt> with the default initial
		''' capacity (16) and load factor (0.75).
		''' </summary>
		Public Sub New()
			Me.New(DEFAULT_INITIAL_CAPACITY, DEFAULT_LOAD_FACTOR)
		End Sub

		''' <summary>
		''' Constructs a new <tt>WeakHashMap</tt> with the same mappings as the
		''' specified map.  The <tt>WeakHashMap</tt> is created with the default
		''' load factor (0.75) and an initial capacity sufficient to hold the
		''' mappings in the specified map.
		''' </summary>
		''' <param name="m"> the map whose mappings are to be placed in this map </param>
		''' <exception cref="NullPointerException"> if the specified map is null
		''' @since   1.3 </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Sub New(Of T1 As K, ? As V)(  m As Map(Of T1))
			Me.New (System.Math.Max(CInt(Fix(m.size() / DEFAULT_LOAD_FACTOR)) + 1, DEFAULT_INITIAL_CAPACITY), DEFAULT_LOAD_FACTOR)
			putAll(m)
		End Sub

		' internal utilities

		''' <summary>
		''' Value representing null keys inside tables.
		''' </summary>
		Private Shared ReadOnly NULL_KEY As New Object

		''' <summary>
		''' Use NULL_KEY for key if it is null.
		''' </summary>
		Private Shared Function maskNull(  key As Object) As Object
			Return If(key Is Nothing, NULL_KEY, key)
		End Function

		''' <summary>
		''' Returns internal representation of null key back to caller as null.
		''' </summary>
		Friend Shared Function unmaskNull(  key As Object) As Object
			Return If(key Is NULL_KEY, Nothing, key)
		End Function

		''' <summary>
		''' Checks for equality of non-null reference x and possibly-null y.  By
		''' default uses Object.equals.
		''' </summary>
		Private Shared Function eq(  x As Object,   y As Object) As Boolean
			Return x Is y OrElse x.Equals(y)
		End Function

		''' <summary>
		''' Retrieve object hash code and applies a supplemental hash function to the
		''' result hash, which defends against poor quality hash functions.  This is
		''' critical because HashMap uses power-of-two length hash tables, that
		''' otherwise encounter collisions for hashCodes that do not differ
		''' in lower bits.
		''' </summary>
		Friend Function hash(  k As Object) As Integer
			Dim h As Integer = k.GetHashCode()

			' This function ensures that hashCodes that differ only by
			' constant multiples at each bit position have a bounded
			' number of collisions (approximately 8 at default load factor).
			h = h Xor (CInt(CUInt(h) >> 20)) Xor (CInt(CUInt(h) >> 12))
			Return h Xor (CInt(CUInt(h) >> 7)) Xor (CInt(CUInt(h) >> 4))
		End Function

		''' <summary>
		''' Returns index for hash code h.
		''' </summary>
		Private Shared Function indexFor(  h As Integer,   length As Integer) As Integer
			Return h And (length-1)
		End Function

		''' <summary>
		''' Expunges stale entries from the table.
		''' </summary>
		Private Sub expungeStaleEntries()
			Dim x As Object
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While (x = queue.poll()) IsNot Nothing
				SyncLock queue
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim e As Entry(Of K, V) = CType(x, Entry(Of K, V))
					Dim i As Integer = indexFor(e.hash, table.Length)

					Dim prev As Entry(Of K, V) = table(i)
					Dim p As Entry(Of K, V) = prev
					Do While p IsNot Nothing
						Dim [next] As Entry(Of K, V) = p.next
						If p Is e Then
							If prev Is e Then
								table(i) = [next]
							Else
								prev.next = [next]
							End If
							' Must not null out e.next;
							' stale entries may be in use by a HashIterator
							e.value = Nothing ' Help GC
							size_Renamed -= 1
							Exit Do
						End If
						prev = p
						p = [next]
					Loop
				End SyncLock
			Loop
		End Sub

		''' <summary>
		''' Returns the table after first expunging stale entries.
		''' </summary>
		Private Property table As Entry(Of K, V)()
			Get
				expungeStaleEntries()
				Return table
			End Get
		End Property

		''' <summary>
		''' Returns the number of key-value mappings in this map.
		''' This result is a snapshot, and may not reflect unprocessed
		''' entries that will be removed before next attempted access
		''' because they are no longer referenced.
		''' </summary>
		Public Overridable Function size() As Integer Implements Map(Of K, V).size
			If size_Renamed = 0 Then Return 0
			expungeStaleEntries()
			Return size_Renamed
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if this map contains no key-value mappings.
		''' This result is a snapshot, and may not reflect unprocessed
		''' entries that will be removed before next attempted access
		''' because they are no longer referenced.
		''' </summary>
		Public Overridable Property empty As Boolean Implements Map(Of K, V).isEmpty
			Get
				Return size() = 0
			End Get
		End Property

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
		''' <seealso cref= #put(Object, Object) </seealso>
		Public Overridable Function [get](  key As Object) As V Implements Map(Of K, V).get
			Dim k As Object = maskNull(key)
			Dim h As Integer = hash(k)
			Dim tab As Entry(Of K, V)() = table
			Dim index As Integer = indexFor(h, tab.Length)
			Dim e As Entry(Of K, V) = tab(index)
			Do While e IsNot Nothing
				If e.hash = h AndAlso eq(k, e.get()) Then Return e.value
				e = e.next
			Loop
			Return Nothing
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if this map contains a mapping for the
		''' specified key.
		''' </summary>
		''' <param name="key">   The key whose presence in this map is to be tested </param>
		''' <returns> <tt>true</tt> if there is a mapping for <tt>key</tt>;
		'''         <tt>false</tt> otherwise </returns>
		Public Overridable Function containsKey(  key As Object) As Boolean Implements Map(Of K, V).containsKey
			Return getEntry(key) IsNot Nothing
		End Function

		''' <summary>
		''' Returns the entry associated with the specified key in this map.
		''' Returns null if the map contains no mapping for this key.
		''' </summary>
		Friend Overridable Function getEntry(  key As Object) As Entry(Of K, V)
			Dim k As Object = maskNull(key)
			Dim h As Integer = hash(k)
			Dim tab As Entry(Of K, V)() = table
			Dim index As Integer = indexFor(h, tab.Length)
			Dim e As Entry(Of K, V) = tab(index)
			Do While e IsNot Nothing AndAlso Not(e.hash = h AndAlso eq(k, e.get()))
				e = e.next
			Loop
			Return e
		End Function

		''' <summary>
		''' Associates the specified value with the specified key in this map.
		''' If the map previously contained a mapping for this key, the old
		''' value is replaced.
		''' </summary>
		''' <param name="key"> key with which the specified value is to be associated. </param>
		''' <param name="value"> value to be associated with the specified key. </param>
		''' <returns> the previous value associated with <tt>key</tt>, or
		'''         <tt>null</tt> if there was no mapping for <tt>key</tt>.
		'''         (A <tt>null</tt> return can also indicate that the map
		'''         previously associated <tt>null</tt> with <tt>key</tt>.) </returns>
		Public Overridable Function put(  key As K,   value As V) As V Implements Map(Of K, V).put
			Dim k As Object = maskNull(key)
			Dim h As Integer = hash(k)
			Dim tab As Entry(Of K, V)() = table
			Dim i As Integer = indexFor(h, tab.Length)

			Dim e As Entry(Of K, V) = tab(i)
			Do While e IsNot Nothing
				If h = e.hash AndAlso eq(k, e.get()) Then
					Dim oldValue As V = e.value
					If value IsNot oldValue Then e.value = value
					Return oldValue
				End If
				e = e.next
			Loop

			modCount += 1
			Dim e As Entry(Of K, V) = tab(i)
			tab(i) = New Entry(Of )(k, value, queue, h, e)
			size_Renamed += 1
			If size_Renamed >= threshold Then resize(tab.Length * 2)
			Return Nothing
		End Function

		''' <summary>
		''' Rehashes the contents of this map into a new array with a
		''' larger capacity.  This method is called automatically when the
		''' number of keys in this map reaches its threshold.
		''' 
		''' If current capacity is MAXIMUM_CAPACITY, this method does not
		''' resize the map, but sets threshold to  java.lang.[Integer].MAX_VALUE.
		''' This has the effect of preventing future calls.
		''' </summary>
		''' <param name="newCapacity"> the new capacity, MUST be a power of two;
		'''        must be greater than current capacity unless current
		'''        capacity is MAXIMUM_CAPACITY (in which case value
		'''        is irrelevant). </param>
		Friend Overridable Sub resize(  newCapacity As Integer)
			Dim oldTable As Entry(Of K, V)() = table
			Dim oldCapacity As Integer = oldTable.Length
			If oldCapacity = MAXIMUM_CAPACITY Then
				threshold =  java.lang.[Integer].Max_Value
				Return
			End If

			Dim newTable As Entry(Of K, V)() = newTable(newCapacity)
			transfer(oldTable, newTable)
			table = newTable

	'        
	'         * If ignoring null elements and processing ref queue caused massive
	'         * shrinkage, then restore old table.  This should be rare, but avoids
	'         * unbounded expansion of garbage-filled tables.
	'         
			If size_Renamed >= threshold \ 2 Then
				threshold = CInt(Fix(newCapacity * loadFactor))
			Else
				expungeStaleEntries()
				transfer(newTable, oldTable)
				table = oldTable
			End If
		End Sub

		''' <summary>
		''' Transfers all entries from src to dest tables </summary>
		Private Sub transfer(  src As Entry(Of K, V)(),   dest As Entry(Of K, V)())
			For j As Integer = 0 To src.Length - 1
				Dim e As Entry(Of K, V) = src(j)
				src(j) = Nothing
				Do While e IsNot Nothing
					Dim [next] As Entry(Of K, V) = e.next
					Dim key As Object = e.get()
					If key Is Nothing Then
						e.next = Nothing ' Help GC
						e.value = Nothing '  "   "
						size_Renamed -= 1
					Else
						Dim i As Integer = indexFor(e.hash, dest.Length)
						e.next = dest(i)
						dest(i) = e
					End If
					e = [next]
				Loop
			Next j
		End Sub

		''' <summary>
		''' Copies all of the mappings from the specified map to this map.
		''' These mappings will replace any mappings that this map had for any
		''' of the keys currently in the specified map.
		''' </summary>
		''' <param name="m"> mappings to be stored in this map. </param>
		''' <exception cref="NullPointerException"> if the specified map is null. </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Sub putAll(Of T1 As K, ? As V)(  m As Map(Of T1)) Implements Map(Of K, V).putAll
			Dim numKeysToBeAdded As Integer = m.size()
			If numKeysToBeAdded = 0 Then Return

	'        
	'         * Expand the map if the map if the number of mappings to be added
	'         * is greater than or equal to threshold.  This is conservative; the
	'         * obvious condition is (m.size() + size) >= threshold, but this
	'         * condition could result in a map with twice the appropriate capacity,
	'         * if the keys to be added overlap with the keys already in this map.
	'         * By using the conservative calculation, we subject ourself
	'         * to at most one extra resize.
	'         
			If numKeysToBeAdded > threshold Then
				Dim targetCapacity As Integer = CInt(Fix(numKeysToBeAdded / loadFactor + 1))
				If targetCapacity > MAXIMUM_CAPACITY Then targetCapacity = MAXIMUM_CAPACITY
				Dim newCapacity As Integer = table.Length
				Do While newCapacity < targetCapacity
					newCapacity <<= 1
				Loop
				If newCapacity > table.Length Then resize(newCapacity)
			End If

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			For Each e As KeyValuePair(Of ? As K, ? As V) In m.entrySet()
				put(e.Key, e.Value)
			Next e
		End Sub

		''' <summary>
		''' Removes the mapping for a key from this weak hash map if it is present.
		''' More formally, if this map contains a mapping from key <tt>k</tt> to
		''' value <tt>v</tt> such that <code>(key==null ?  k==null :
		''' key.equals(k))</code>, that mapping is removed.  (The map can contain
		''' at most one such mapping.)
		''' 
		''' <p>Returns the value to which this map previously associated the key,
		''' or <tt>null</tt> if the map contained no mapping for the key.  A
		''' return value of <tt>null</tt> does not <i>necessarily</i> indicate
		''' that the map contained no mapping for the key; it's also possible
		''' that the map explicitly mapped the key to <tt>null</tt>.
		''' 
		''' <p>The map will not contain a mapping for the specified key once the
		''' call returns.
		''' </summary>
		''' <param name="key"> key whose mapping is to be removed from the map </param>
		''' <returns> the previous value associated with <tt>key</tt>, or
		'''         <tt>null</tt> if there was no mapping for <tt>key</tt> </returns>
		Public Overridable Function remove(  key As Object) As V Implements Map(Of K, V).remove
			Dim k As Object = maskNull(key)
			Dim h As Integer = hash(k)
			Dim tab As Entry(Of K, V)() = table
			Dim i As Integer = indexFor(h, tab.Length)
			Dim prev As Entry(Of K, V) = tab(i)
			Dim e As Entry(Of K, V) = prev

			Do While e IsNot Nothing
				Dim [next] As Entry(Of K, V) = e.next
				If h = e.hash AndAlso eq(k, e.get()) Then
					modCount += 1
					size_Renamed -= 1
					If prev Is e Then
						tab(i) = [next]
					Else
						prev.next = [next]
					End If
					Return e.value
				End If
				prev = e
				e = [next]
			Loop

			Return Nothing
		End Function

		''' <summary>
		''' Special version of remove needed by Entry set </summary>
		Friend Overridable Function removeMapping(  o As Object) As Boolean
			If Not(TypeOf o Is DictionaryEntry) Then Return False
			Dim tab As Entry(Of K, V)() = table
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim entry_Renamed As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
			Dim k As Object = maskNull(entry_Renamed.Key)
			Dim h As Integer = hash(k)
			Dim i As Integer = indexFor(h, tab.Length)
			Dim prev As Entry(Of K, V) = tab(i)
			Dim e As Entry(Of K, V) = prev

			Do While e IsNot Nothing
				Dim [next] As Entry(Of K, V) = e.next
				If h = e.hash AndAlso e.Equals(entry_Renamed) Then
					modCount += 1
					size_Renamed -= 1
					If prev Is e Then
						tab(i) = [next]
					Else
						prev.next = [next]
					End If
					Return True
				End If
				prev = e
				e = [next]
			Loop

			Return False
		End Function

		''' <summary>
		''' Removes all of the mappings from this map.
		''' The map will be empty after this call returns.
		''' </summary>
		Public Overridable Sub clear() Implements Map(Of K, V).clear
			' clear out ref queue. We don't need to expunge entries
			' since table is getting cleared.
			Do While queue.poll() IsNot Nothing

			Loop

			modCount += 1
			Arrays.fill(table, Nothing)
			size_Renamed = 0

			' Allocation of array may have caused GC, which may have caused
			' additional entries to go stale.  Removing these entries from the
			' reference queue will make them eligible for reclamation.
			Do While queue.poll() IsNot Nothing

			Loop
		End Sub

		''' <summary>
		''' Returns <tt>true</tt> if this map maps one or more keys to the
		''' specified value.
		''' </summary>
		''' <param name="value"> value whose presence in this map is to be tested </param>
		''' <returns> <tt>true</tt> if this map maps one or more keys to the
		'''         specified value </returns>
		Public Overridable Function containsValue(  value As Object) As Boolean Implements Map(Of K, V).containsValue
			If value Is Nothing Then Return containsNullValue()

			Dim tab As Entry(Of K, V)() = table
			Dim i As Integer = tab.Length
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While i -= 1 > 0
				Dim e As Entry(Of K, V) = tab(i)
				Do While e IsNot Nothing
					If value.Equals(e.value) Then Return True
					e = e.next
				Loop
			Loop
			Return False
		End Function

		''' <summary>
		''' Special-case code for containsValue with null argument
		''' </summary>
		Private Function containsNullValue() As Boolean
			Dim tab As Entry(Of K, V)() = table
			Dim i As Integer = tab.Length
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While i -= 1 > 0
				Dim e As Entry(Of K, V) = tab(i)
				Do While e IsNot Nothing
					If e.value Is Nothing Then Return True
					e = e.next
				Loop
			Loop
			Return False
		End Function

		''' <summary>
		''' The entries in this hash table extend WeakReference, using its main ref
		''' field as the key.
		''' </summary>
		Private Class Entry(Of K, V)
			Inherits WeakReference(Of Object)
			Implements KeyValuePair(Of K, V)

			Friend value As V
			Friend ReadOnly hash As Integer
			Friend [next] As Entry(Of K, V)

			''' <summary>
			''' Creates new entry.
			''' </summary>
			Friend Sub New(  key As Object,   value As V,   queue As ReferenceQueue(Of Object),   hash As Integer,   [next] As Entry(Of K, V))
				MyBase.New(key, queue)
				Me.value = value
				Me.hash = hash
				Me.next = [next]
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Property key As K
				Get
					Return CType(WeakHashMap.unmaskNull(get()), K)
				End Get
			End Property

			Public Overridable Property value As V
				Get
					Return value
				End Get
			End Property

			Public Overridable Function setValue(  newValue As V) As V
				Dim oldValue As V = value
				value = newValue
				Return oldValue
			End Function

			Public Overrides Function Equals(  o As Object) As Boolean
				If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
				Dim k1 As K = key
				Dim k2 As Object = e.Key
				If k1 Is k2 OrElse (k1 IsNot Nothing AndAlso k1.Equals(k2)) Then
					Dim v1 As V = value
					Dim v2 As Object = e.Value
					If v1 Is v2 OrElse (v1 IsNot Nothing AndAlso v1.Equals(v2)) Then Return True
				End If
				Return False
			End Function

			Public Overrides Function GetHashCode() As Integer
				Dim k As K = key
				Dim v As V = value
				Return Objects.hashCode(k) Xor Objects.hashCode(v)
			End Function

			Public Overrides Function ToString() As String
				Return key & "=" & value
			End Function
		End Class

		Private MustInherit Class HashIterator(Of T)
			Implements Iterator(Of T)

			Private ReadOnly outerInstance As WeakHashMap

			Private index As Integer
			Private entry As Entry(Of K, V)
			Private lastReturned As Entry(Of K, V)
			Private expectedModCount As Integer = outerInstance.modCount

			''' <summary>
			''' Strong reference needed to avoid disappearance of key
			''' between hasNext and next
			''' </summary>
			Private nextKey As Object

			''' <summary>
			''' Strong reference needed to avoid disappearance of key
			''' between nextEntry() and any use of the entry
			''' </summary>
			Private currentKey As Object

			Friend Sub New(  outerInstance As WeakHashMap)
					Me.outerInstance = outerInstance
				index = If(outerInstance.empty, 0, outerInstance.table.Length)
			End Sub

			Public Overridable Function hasNext() As Boolean Implements Iterator(Of T).hasNext
				Dim t As Entry(Of K, V)() = outerInstance.table

				Do While nextKey Is Nothing
					Dim e As Entry(Of K, V) = entry
					Dim i As Integer = index
					Do While e Is Nothing AndAlso i > 0
						i -= 1
						e = t(i)
					Loop
					entry = e
					index = i
					If e Is Nothing Then
						currentKey = Nothing
						Return False
					End If
					nextKey = e.get() ' hold on to key in strong ref
					If nextKey Is Nothing Then entry = entry.next
				Loop
				Return True
			End Function

			''' <summary>
			''' The common parts of next() across different types of iterators </summary>
			Protected Friend Overridable Function nextEntry() As Entry(Of K, V)
				If outerInstance.modCount <> expectedModCount Then Throw New ConcurrentModificationException
				If nextKey Is Nothing AndAlso (Not hasNext()) Then Throw New NoSuchElementException

				lastReturned = entry
				entry = entry.next
				currentKey = nextKey
				nextKey = Nothing
				Return lastReturned
			End Function

			Public Overridable Sub remove() Implements Iterator(Of T).remove
				If lastReturned Is Nothing Then Throw New IllegalStateException
				If outerInstance.modCount <> expectedModCount Then Throw New ConcurrentModificationException

				outerInstance.remove(currentKey)
				expectedModCount = outerInstance.modCount
				lastReturned = Nothing
				currentKey = Nothing
			End Sub

		End Class

		Private Class ValueIterator
			Inherits HashIterator(Of V)

			Private ReadOnly outerInstance As WeakHashMap

			Public Sub New(  outerInstance As WeakHashMap)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Function [next]() As V
				Return nextEntry().value
			End Function
		End Class

		Private Class KeyIterator
			Inherits HashIterator(Of K)

			Private ReadOnly outerInstance As WeakHashMap

			Public Sub New(  outerInstance As WeakHashMap)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Function [next]() As K
				Return nextEntry().key
			End Function
		End Class

		Private Class EntryIterator
			Inherits HashIterator(Of KeyValuePair(Of K, V))

			Private ReadOnly outerInstance As WeakHashMap

			Public Sub New(  outerInstance As WeakHashMap)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Function [next]() As KeyValuePair(Of K, V)
				Return nextEntry()
			End Function
		End Class

		' Views

		<NonSerialized> _
		Private entrySet_Renamed As [Set](Of KeyValuePair(Of K, V))

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
		''' </summary>
		Public Overridable Function keySet() As [Set](Of K) Implements Map(Of K, V).keySet
			Dim ks As [Set](Of K) = keySet_Renamed
				If ks IsNot Nothing Then
					Return (ks)
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (keySet_Renamed = New KeySet(Me))
				End If
		End Function

		Private Class KeySet
			Inherits AbstractSet(Of K)

			Private ReadOnly outerInstance As WeakHashMap

			Public Sub New(  outerInstance As WeakHashMap)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Function [iterator]() As [Iterator](Of K)
				Return New KeyIterator
			End Function

			Public Overridable Function size() As Integer
				Return outerInstance.size()
			End Function

			Public Overridable Function contains(  o As Object) As Boolean
				Return outerInstance.containsKey(o)
			End Function

			Public Overridable Function remove(  o As Object) As Boolean
				If outerInstance.containsKey(o) Then
					outerInstance.remove(o)
					Return True
				Else
					Return False
				End If
			End Function

			Public Overridable Sub clear()
				outerInstance.clear()
			End Sub

			Public Overridable Function spliterator() As Spliterator(Of K)
				Return New KeySpliterator(Of )(WeakHashMap.this, 0, -1, 0, 0)
			End Function
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
		''' </summary>
		Public Overridable Function values() As Collection(Of V) Implements Map(Of K, V).values
			Dim vs As Collection(Of V) = values_Renamed
				If vs IsNot Nothing Then
					Return vs
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (values_Renamed = New Values(Me))
				End If
		End Function

		Private Class Values
			Inherits AbstractCollection(Of V)

			Private ReadOnly outerInstance As WeakHashMap

			Public Sub New(  outerInstance As WeakHashMap)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Function [iterator]() As [Iterator](Of V)
				Return New ValueIterator
			End Function

			Public Overridable Function size() As Integer
				Return outerInstance.size()
			End Function

			Public Overridable Function contains(  o As Object) As Boolean
				Return outerInstance.containsValue(o)
			End Function

			Public Overridable Sub clear()
				outerInstance.clear()
			End Sub

			Public Overridable Function spliterator() As Spliterator(Of V)
				Return New ValueSpliterator(Of )(WeakHashMap.this, 0, -1, 0, 0)
			End Function
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
		''' </summary>
		Public Overridable Function entrySet() As [Set](Of KeyValuePair(Of K, V)) Implements Map(Of K, V).entrySet
			Dim es As [Set](Of KeyValuePair(Of K, V)) = entrySet_Renamed
				If es IsNot Nothing Then
					Return es
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (entrySet_Renamed = New EntrySet(Me))
				End If
		End Function

		Private Class EntrySet
			Inherits AbstractSet(Of KeyValuePair(Of K, V))

			Private ReadOnly outerInstance As WeakHashMap

			Public Sub New(  outerInstance As WeakHashMap)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Function [iterator]() As [Iterator](Of KeyValuePair(Of K, V))
				Return New EntryIterator
			End Function

			Public Overridable Function contains(  o As Object) As Boolean
				If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
				Dim candidate As Entry(Of K, V) = outerInstance.getEntry(e.Key)
				Return candidate IsNot Nothing AndAlso candidate.Equals(e)
			End Function

			Public Overridable Function remove(  o As Object) As Boolean
				Return outerInstance.removeMapping(o)
			End Function

			Public Overridable Function size() As Integer
				Return outerInstance.size()
			End Function

			Public Overridable Sub clear()
				outerInstance.clear()
			End Sub

			Private Function deepCopy() As List(Of KeyValuePair(Of K, V))
				Dim list As List(Of KeyValuePair(Of K, V)) = New List(Of KeyValuePair(Of K, V))(size())
				For Each e As KeyValuePair(Of K, V) In Me
					list.add(New AbstractMap.SimpleEntry(Of )(e))
				Next e
				Return list
			End Function

			Public Overridable Function toArray() As Object()
				Return deepCopy().ToArray()
			End Function

			Public Overridable Function toArray(Of T)(  a As T()) As T()
				Return deepCopy().ToArray(a)
			End Function

			Public Overridable Function spliterator() As Spliterator(Of KeyValuePair(Of K, V))
				Return New EntrySpliterator(Of )(WeakHashMap.this, 0, -1, 0, 0)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Sub forEach(Of T1)(  action As java.util.function.BiConsumer(Of T1)) Implements Map(Of K, V).forEach
			Objects.requireNonNull(action)
			Dim expectedModCount As Integer = modCount

			Dim tab As Entry(Of K, V)() = table
			For Each entry_Renamed As Entry(Of K, V) In tab
				Do While entry_Renamed IsNot Nothing
					Dim key As Object = entry_Renamed.get()
					If key IsNot Nothing Then action.accept(CType(WeakHashMap.unmaskNull(key), K), entry_Renamed.value)
					entry_Renamed = entry_Renamed.next

					If expectedModCount <> modCount Then Throw New ConcurrentModificationException
				Loop
			Next entry_Renamed
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Sub replaceAll(Of T1 As V)(  [function] As java.util.function.BiFunction(Of T1)) Implements Map(Of K, V).replaceAll
			Objects.requireNonNull([function])
			Dim expectedModCount As Integer = modCount

			Dim tab As Entry(Of K, V)() = table
			For Each entry_Renamed As Entry(Of K, V) In tab
				Do While entry_Renamed IsNot Nothing
					Dim key As Object = entry_Renamed.get()
					If key IsNot Nothing Then entry_Renamed.value = [function].apply(CType(WeakHashMap.unmaskNull(key), K), entry_Renamed.value)
					entry_Renamed = entry_Renamed.next

					If expectedModCount <> modCount Then Throw New ConcurrentModificationException
				Loop
			Next entry_Renamed
		End Sub

		''' <summary>
		''' Similar form as other hash Spliterators, but skips dead
		''' elements.
		''' </summary>
		Friend Class WeakHashMapSpliterator(Of K, V)
			Friend ReadOnly map As WeakHashMap(Of K, V)
			Friend current As WeakHashMap.Entry(Of K, V) ' current node
			Friend index As Integer ' current index, modified on advance/split
			Friend fence As Integer ' -1 until first use; then one past last index
			Friend est As Integer ' size estimate
			Friend expectedModCount As Integer ' for comodification checks

			Friend Sub New(  m As WeakHashMap(Of K, V),   origin As Integer,   fence As Integer,   est As Integer,   expectedModCount As Integer)
				Me.map = m
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
						Dim m As WeakHashMap(Of K, V) = map
						est = m.size()
						expectedModCount = m.modCount
							fence = m.table.Length
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
			Inherits WeakHashMapSpliterator(Of K, V)
			Implements Spliterator(Of K)

			Friend Sub New(  m As WeakHashMap(Of K, V),   origin As Integer,   fence As Integer,   est As Integer,   expectedModCount As Integer)
				MyBase.New(m, origin, fence, est, expectedModCount)
			End Sub

			Public Function trySplit() As KeySpliterator(Of K, V)
				Dim hi As Integer = fence, lo As Integer = index, mid As Integer = CInt(CUInt((lo + hi)) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If(lo >= mid, Nothing, New KeySpliterator(Of K, V)(map, lo, index = mid, est >>>= 1, expectedModCount))
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(  action As java.util.function.Consumer(Of T1)) Implements Spliterator(Of K).forEachRemaining
				Dim i, hi, mc As Integer
				If action Is Nothing Then Throw New NullPointerException
				Dim m As WeakHashMap(Of K, V) = map
				Dim tab As WeakHashMap.Entry(Of K, V)() = m.table
				hi = fence
				If hi < 0 Then
						expectedModCount = m.modCount
						mc = expectedModCount
						fence = tab.Length
						hi = fence
				Else
					mc = expectedModCount
				End If
				i = index
				index = hi
				If tab.Length >= hi AndAlso i >= 0 AndAlso (i < index OrElse current IsNot Nothing) Then
					Dim p As WeakHashMap.Entry(Of K, V) = current
					current = Nothing ' exhaust
					Do
						If p Is Nothing Then
							p = tab(i)
							i += 1
						Else
							Dim x As Object = p.get()
							p = p.next
							If x IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
								Dim k As K = CType(WeakHashMap.unmaskNull(x), K)
								action.accept(k)
							End If
						End If
					Loop While p IsNot Nothing OrElse i < hi
				End If
				If m.modCount <> mc Then Throw New ConcurrentModificationException
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(  action As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of K).tryAdvance
				Dim hi As Integer
				If action Is Nothing Then Throw New NullPointerException
				Dim tab As WeakHashMap.Entry(Of K, V)() = map.table
				hi = fence
				If tab.Length >= hi AndAlso index >= 0 Then
					Do While current IsNot Nothing OrElse index < hi
						If current Is Nothing Then
							current = tab(index)
							index += 1
						Else
							Dim x As Object = current.get()
							current = current.next
							If x IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
								Dim k As K = CType(WeakHashMap.unmaskNull(x), K)
								action.accept(k)
								If map.modCount <> expectedModCount Then Throw New ConcurrentModificationException
								Return True
							End If
						End If
					Loop
				End If
				Return False
			End Function

			Public Function characteristics() As Integer Implements Spliterator(Of K).characteristics
				Return Spliterator.DISTINCT
			End Function
		End Class

		Friend NotInheritable Class ValueSpliterator(Of K, V)
			Inherits WeakHashMapSpliterator(Of K, V)
			Implements Spliterator(Of V)

			Friend Sub New(  m As WeakHashMap(Of K, V),   origin As Integer,   fence As Integer,   est As Integer,   expectedModCount As Integer)
				MyBase.New(m, origin, fence, est, expectedModCount)
			End Sub

			Public Function trySplit() As ValueSpliterator(Of K, V)
				Dim hi As Integer = fence, lo As Integer = index, mid As Integer = CInt(CUInt((lo + hi)) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If(lo >= mid, Nothing, New ValueSpliterator(Of K, V)(map, lo, index = mid, est >>>= 1, expectedModCount))
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(  action As java.util.function.Consumer(Of T1)) Implements Spliterator(Of V).forEachRemaining
				Dim i, hi, mc As Integer
				If action Is Nothing Then Throw New NullPointerException
				Dim m As WeakHashMap(Of K, V) = map
				Dim tab As WeakHashMap.Entry(Of K, V)() = m.table
				hi = fence
				If hi < 0 Then
						expectedModCount = m.modCount
						mc = expectedModCount
						fence = tab.Length
						hi = fence
				Else
					mc = expectedModCount
				End If
				i = index
				index = hi
				If tab.Length >= hi AndAlso i >= 0 AndAlso (i < index OrElse current IsNot Nothing) Then
					Dim p As WeakHashMap.Entry(Of K, V) = current
					current = Nothing ' exhaust
					Do
						If p Is Nothing Then
							p = tab(i)
							i += 1
						Else
							Dim x As Object = p.get()
							Dim v As V = p.value
							p = p.next
							If x IsNot Nothing Then action.accept(v)
						End If
					Loop While p IsNot Nothing OrElse i < hi
				End If
				If m.modCount <> mc Then Throw New ConcurrentModificationException
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(  action As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of V).tryAdvance
				Dim hi As Integer
				If action Is Nothing Then Throw New NullPointerException
				Dim tab As WeakHashMap.Entry(Of K, V)() = map.table
				hi = fence
				If tab.Length >= hi AndAlso index >= 0 Then
					Do While current IsNot Nothing OrElse index < hi
						If current Is Nothing Then
							current = tab(index)
							index += 1
						Else
							Dim x As Object = current.get()
							Dim v As V = current.value
							current = current.next
							If x IsNot Nothing Then
								action.accept(v)
								If map.modCount <> expectedModCount Then Throw New ConcurrentModificationException
								Return True
							End If
						End If
					Loop
				End If
				Return False
			End Function

			Public Function characteristics() As Integer Implements Spliterator(Of V).characteristics
				Return 0
			End Function
		End Class

		Friend NotInheritable Class EntrySpliterator(Of K, V)
			Inherits WeakHashMapSpliterator(Of K, V)
			Implements Spliterator(Of KeyValuePair(Of K, V))

			Friend Sub New(  m As WeakHashMap(Of K, V),   origin As Integer,   fence As Integer,   est As Integer,   expectedModCount As Integer)
				MyBase.New(m, origin, fence, est, expectedModCount)
			End Sub

			Public Function trySplit() As EntrySpliterator(Of K, V)
				Dim hi As Integer = fence, lo As Integer = index, mid As Integer = CInt(CUInt((lo + hi)) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If(lo >= mid, Nothing, New EntrySpliterator(Of K, V)(map, lo, index = mid, est >>>= 1, expectedModCount))
			End Function


'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(  action As java.util.function.Consumer(Of T1)) Implements Spliterator(Of KeyValuePair(Of K, V)).forEachRemaining
				Dim i, hi, mc As Integer
				If action Is Nothing Then Throw New NullPointerException
				Dim m As WeakHashMap(Of K, V) = map
				Dim tab As WeakHashMap.Entry(Of K, V)() = m.table
				hi = fence
				If hi < 0 Then
						expectedModCount = m.modCount
						mc = expectedModCount
						fence = tab.Length
						hi = fence
				Else
					mc = expectedModCount
				End If
				i = index
				index = hi
				If tab.Length >= hi AndAlso i >= 0 AndAlso (i < index OrElse current IsNot Nothing) Then
					Dim p As WeakHashMap.Entry(Of K, V) = current
					current = Nothing ' exhaust
					Do
						If p Is Nothing Then
							p = tab(i)
							i += 1
						Else
							Dim x As Object = p.get()
							Dim v As V = p.value
							p = p.next
							If x IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
								Dim k As K = CType(WeakHashMap.unmaskNull(x), K)
								action.accept(New AbstractMap.SimpleImmutableEntry(Of K, V)(k, v))
							End If
						End If
					Loop While p IsNot Nothing OrElse i < hi
				End If
				If m.modCount <> mc Then Throw New ConcurrentModificationException
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(  action As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of KeyValuePair(Of K, V)).tryAdvance
				Dim hi As Integer
				If action Is Nothing Then Throw New NullPointerException
				Dim tab As WeakHashMap.Entry(Of K, V)() = map.table
				hi = fence
				If tab.Length >= hi AndAlso index >= 0 Then
					Do While current IsNot Nothing OrElse index < hi
						If current Is Nothing Then
							current = tab(index)
							index += 1
						Else
							Dim x As Object = current.get()
							Dim v As V = current.value
							current = current.next
							If x IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
								Dim k As K = CType(WeakHashMap.unmaskNull(x), K)
								action.accept(New AbstractMap.SimpleImmutableEntry(Of K, V)(k, v))
								If map.modCount <> expectedModCount Then Throw New ConcurrentModificationException
								Return True
							End If
						End If
					Loop
				End If
				Return False
			End Function

			Public Function characteristics() As Integer Implements Spliterator(Of KeyValuePair(Of K, V)).characteristics
				Return Spliterator.DISTINCT
			End Function
		End Class

	End Class

End Namespace
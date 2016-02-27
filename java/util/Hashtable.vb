Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This class implements a hash table, which maps keys to values. Any
	''' non-<code>null</code> object can be used as a key or as a value. <p>
	''' 
	''' To successfully store and retrieve objects from a hashtable, the
	''' objects used as keys must implement the <code>hashCode</code>
	''' method and the <code>equals</code> method. <p>
	''' 
	''' An instance of <code>Hashtable</code> has two parameters that affect its
	''' performance: <i>initial capacity</i> and <i>load factor</i>.  The
	''' <i>capacity</i> is the number of <i>buckets</i> in the hash table, and the
	''' <i>initial capacity</i> is simply the capacity at the time the hash table
	''' is created.  Note that the hash table is <i>open</i>: in the case of a "hash
	''' collision", a single bucket stores multiple entries, which must be searched
	''' sequentially.  The <i>load factor</i> is a measure of how full the hash
	''' table is allowed to get before its capacity is automatically increased.
	''' The initial capacity and load factor parameters are merely hints to
	''' the implementation.  The exact details as to when and whether the rehash
	''' method is invoked are implementation-dependent.<p>
	''' 
	''' Generally, the default load factor (.75) offers a good tradeoff between
	''' time and space costs.  Higher values decrease the space overhead but
	''' increase the time cost to look up an entry (which is reflected in most
	''' <tt>Hashtable</tt> operations, including <tt>get</tt> and <tt>put</tt>).<p>
	''' 
	''' The initial capacity controls a tradeoff between wasted space and the
	''' need for <code>rehash</code> operations, which are time-consuming.
	''' No <code>rehash</code> operations will <i>ever</i> occur if the initial
	''' capacity is greater than the maximum number of entries the
	''' <tt>Hashtable</tt> will contain divided by its load factor.  However,
	''' setting the initial capacity too high can waste space.<p>
	''' 
	''' If many entries are to be made into a <code>Hashtable</code>,
	''' creating it with a sufficiently large capacity may allow the
	''' entries to be inserted more efficiently than letting it perform
	''' automatic rehashing as needed to grow the table. <p>
	''' 
	''' This example creates a hashtable of numbers. It uses the names of
	''' the numbers as keys:
	''' <pre>   {@code
	'''   Hashtable<String, Integer> numbers
	'''     = new Hashtable<String, Integer>();
	'''   numbers.put("one", 1);
	'''   numbers.put("two", 2);
	'''   numbers.put("three", 3);}</pre>
	''' 
	''' <p>To retrieve a number, use the following code:
	''' <pre>   {@code
	'''   Integer n = numbers.get("two");
	'''   if (n != null) {
	'''     System.out.println("two = " + n);
	'''   }}</pre>
	''' 
	''' <p>The iterators returned by the <tt>iterator</tt> method of the collections
	''' returned by all of this class's "collection view methods" are
	''' <em>fail-fast</em>: if the Hashtable is structurally modified at any time
	''' after the iterator is created, in any way except through the iterator's own
	''' <tt>remove</tt> method, the iterator will throw a {@link
	''' ConcurrentModificationException}.  Thus, in the face of concurrent
	''' modification, the iterator fails quickly and cleanly, rather than risking
	''' arbitrary, non-deterministic behavior at an undetermined time in the future.
	''' The Enumerations returned by Hashtable's keys and elements methods are
	''' <em>not</em> fail-fast.
	''' 
	''' <p>Note that the fail-fast behavior of an iterator cannot be guaranteed
	''' as it is, generally speaking, impossible to make any hard guarantees in the
	''' presence of unsynchronized concurrent modification.  Fail-fast iterators
	''' throw <tt>ConcurrentModificationException</tt> on a best-effort basis.
	''' Therefore, it would be wrong to write a program that depended on this
	''' exception for its correctness: <i>the fail-fast behavior of iterators
	''' should be used only to detect bugs.</i>
	''' 
	''' <p>As of the Java 2 platform v1.2, this class was retrofitted to
	''' implement the <seealso cref="Map"/> interface, making it a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' 
	''' Java Collections Framework</a>.  Unlike the new collection
	''' implementations, {@code Hashtable} is synchronized.  If a
	''' thread-safe implementation is not needed, it is recommended to use
	''' <seealso cref="HashMap"/> in place of {@code Hashtable}.  If a thread-safe
	''' highly-concurrent implementation is desired, then it is recommended
	''' to use <seealso cref="java.util.concurrent.ConcurrentHashMap"/> in place of
	''' {@code Hashtable}.
	''' 
	''' @author  Arthur van Hoff
	''' @author  Josh Bloch
	''' @author  Neal Gafter </summary>
	''' <seealso cref=     Object#equals(java.lang.Object) </seealso>
	''' <seealso cref=     Object#hashCode() </seealso>
	''' <seealso cref=     Hashtable#rehash() </seealso>
	''' <seealso cref=     Collection </seealso>
	''' <seealso cref=     Map </seealso>
	''' <seealso cref=     HashMap </seealso>
	''' <seealso cref=     TreeMap
	''' @since JDK1.0 </seealso>
	<Serializable> _
	Public Class Dictionary(Of K, V)
		Inherits Dictionary(Of K, V)
		Implements Map(Of K, V), Cloneable

		''' <summary>
		''' The hash table data.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		<NonSerialized> _
		Private table As Entry(Of ?, ?)()

		''' <summary>
		''' The total number of entries in the hash table.
		''' </summary>
		<NonSerialized> _
		Private count As Integer

		''' <summary>
		''' The table is rehashed when its size exceeds this threshold.  (The
		''' value of this field is (int)(capacity * loadFactor).)
		''' 
		''' @serial
		''' </summary>
		Private threshold As Integer

		''' <summary>
		''' The load factor for the hashtable.
		''' 
		''' @serial
		''' </summary>
		Private loadFactor As Single

		''' <summary>
		''' The number of times this Hashtable has been structurally modified
		''' Structural modifications are those that change the number of entries in
		''' the Hashtable or otherwise modify its internal structure (e.g.,
		''' rehash).  This field is used to make iterators on Collection-views of
		''' the Hashtable fail-fast.  (See ConcurrentModificationException).
		''' </summary>
		<NonSerialized> _
		Private modCount As Integer = 0

		''' <summary>
		''' use serialVersionUID from JDK 1.0.2 for interoperability </summary>
		Private Const serialVersionUID As Long = 1421746759512286392L

		''' <summary>
		''' Constructs a new, empty hashtable with the specified initial
		''' capacity and the specified load factor.
		''' </summary>
		''' <param name="initialCapacity">   the initial capacity of the hashtable. </param>
		''' <param name="loadFactor">        the load factor of the hashtable. </param>
		''' <exception cref="IllegalArgumentException">  if the initial capacity is less
		'''             than zero, or if the load factor is nonpositive. </exception>
		Public Sub New(ByVal initialCapacity As Integer, ByVal loadFactor As Single)
			If initialCapacity < 0 Then Throw New IllegalArgumentException("Illegal Capacity: " & initialCapacity)
			If loadFactor <= 0 OrElse Float.IsNaN(loadFactor) Then Throw New IllegalArgumentException("Illegal Load: " & loadFactor)

			If initialCapacity=0 Then initialCapacity = 1
			Me.loadFactor = loadFactor
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			table = New Entry(Of ?, ?)(initialCapacity - 1){}
			threshold = CInt(Fix (System.Math.Min(initialCapacity * loadFactor, MAX_ARRAY_SIZE + 1)))
		End Sub

		''' <summary>
		''' Constructs a new, empty hashtable with the specified initial capacity
		''' and default load factor (0.75).
		''' </summary>
		''' <param name="initialCapacity">   the initial capacity of the hashtable. </param>
		''' <exception cref="IllegalArgumentException"> if the initial capacity is less
		'''              than zero. </exception>
		Public Sub New(ByVal initialCapacity As Integer)
			Me.New(initialCapacity, 0.75f)
		End Sub

		''' <summary>
		''' Constructs a new, empty hashtable with a default initial capacity (11)
		''' and load factor (0.75).
		''' </summary>
		Public Sub New()
			Me.New(11, 0.75f)
		End Sub

		''' <summary>
		''' Constructs a new hashtable with the same mappings as the given
		''' Map.  The hashtable is created with an initial capacity sufficient to
		''' hold the mappings in the given Map and a default load factor (0.75).
		''' </summary>
		''' <param name="t"> the map whose mappings are to be placed in this map. </param>
		''' <exception cref="NullPointerException"> if the specified map is null.
		''' @since   1.2 </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Sub New(Of T1 As K, ? As V)(ByVal t As Map(Of T1))
			Me.New (System.Math.Max(2*t.size(), 11), 0.75f)
			putAll(t)
		End Sub

		''' <summary>
		''' Returns the number of keys in this hashtable.
		''' </summary>
		''' <returns>  the number of keys in this hashtable. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function size() As Integer Implements Map(Of K, V).size
			Return count
		End Function

		''' <summary>
		''' Tests if this hashtable maps no keys to values.
		''' </summary>
		''' <returns>  <code>true</code> if this hashtable maps no keys to values;
		'''          <code>false</code> otherwise. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property empty As Boolean Implements Map(Of K, V).isEmpty
			Get
				Return count = 0
			End Get
		End Property

		''' <summary>
		''' Returns an enumeration of the keys in this hashtable.
		''' </summary>
		''' <returns>  an enumeration of the keys in this hashtable. </returns>
		''' <seealso cref=     Enumeration </seealso>
		''' <seealso cref=     #elements() </seealso>
		''' <seealso cref=     #keySet() </seealso>
		''' <seealso cref=     Map </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function keys() As Enumeration(Of K)
			Return Me.getEnumeration(Of K)(KEYS_Renamed)
		End Function

		''' <summary>
		''' Returns an enumeration of the values in this hashtable.
		''' Use the Enumeration methods on the returned object to fetch the elements
		''' sequentially.
		''' </summary>
		''' <returns>  an enumeration of the values in this hashtable. </returns>
		''' <seealso cref=     java.util.Enumeration </seealso>
		''' <seealso cref=     #keys() </seealso>
		''' <seealso cref=     #values() </seealso>
		''' <seealso cref=     Map </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function elements() As Enumeration(Of V)
			Return Me.getEnumeration(Of V)(VALUES_Renamed)
		End Function

		''' <summary>
		''' Tests if some key maps into the specified value in this hashtable.
		''' This operation is more expensive than the {@link #containsKey
		''' containsKey} method.
		''' 
		''' <p>Note that this method is identical in functionality to
		''' <seealso cref="#containsValue containsValue"/>, (which is part of the
		''' <seealso cref="Map"/> interface in the collections framework).
		''' </summary>
		''' <param name="value">   a value to search for </param>
		''' <returns>     <code>true</code> if and only if some key maps to the
		'''             <code>value</code> argument in this hashtable as
		'''             determined by the <tt>equals</tt> method;
		'''             <code>false</code> otherwise. </returns>
		''' <exception cref="NullPointerException">  if the value is <code>null</code> </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function contains(ByVal value As Object) As Boolean
			If value Is Nothing Then Throw New NullPointerException

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim tab As Entry(Of ?, ?)() = table
			Dim i As Integer = tab.Length
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While i -= 1 > 0
				Dim e As Entry(Of ?, ?) = tab(i)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Do While e IsNot Nothing
					If e.value.Equals(value) Then Return True
					e = e.next
				Loop
			Loop
			Return False
		End Function

		''' <summary>
		''' Returns true if this hashtable maps one or more keys to this value.
		''' 
		''' <p>Note that this method is identical in functionality to {@link
		''' #contains contains} (which predates the <seealso cref="Map"/> interface).
		''' </summary>
		''' <param name="value"> value whose presence in this hashtable is to be tested </param>
		''' <returns> <tt>true</tt> if this map maps one or more keys to the
		'''         specified value </returns>
		''' <exception cref="NullPointerException">  if the value is <code>null</code>
		''' @since 1.2 </exception>
		Public Overridable Function containsValue(ByVal value As Object) As Boolean Implements Map(Of K, V).containsValue
			Return contains(value)
		End Function

		''' <summary>
		''' Tests if the specified object is a key in this hashtable.
		''' </summary>
		''' <param name="key">   possible key </param>
		''' <returns>  <code>true</code> if and only if the specified object
		'''          is a key in this hashtable, as determined by the
		'''          <tt>equals</tt> method; <code>false</code> otherwise. </returns>
		''' <exception cref="NullPointerException">  if the key is <code>null</code> </exception>
		''' <seealso cref=     #contains(Object) </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function containsKey(ByVal key As Object) As Boolean Implements Map(Of K, V).containsKey
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim tab As Entry(Of ?, ?)() = table
			Dim hash As Integer = key.GetHashCode()
			Dim index As Integer = (hash And &H7FFFFFFF) Mod tab.Length
			Dim e As Entry(Of ?, ?) = tab(index)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Do While e IsNot Nothing
				If (e.hash = hash) AndAlso e.key.Equals(key) Then Return True
				e = e.next
			Loop
			Return False
		End Function

		''' <summary>
		''' Returns the value to which the specified key is mapped,
		''' or {@code null} if this map contains no mapping for the key.
		''' 
		''' <p>More formally, if this map contains a mapping from a key
		''' {@code k} to a value {@code v} such that {@code (key.equals(k))},
		''' then this method returns {@code v}; otherwise it returns
		''' {@code null}.  (There can be at most one such mapping.)
		''' </summary>
		''' <param name="key"> the key whose associated value is to be returned </param>
		''' <returns> the value to which the specified key is mapped, or
		'''         {@code null} if this map contains no mapping for the key </returns>
		''' <exception cref="NullPointerException"> if the specified key is null </exception>
		''' <seealso cref=     #put(Object, Object) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function [get](ByVal key As Object) As V Implements Map(Of K, V).get
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim tab As Entry(Of ?, ?)() = table
			Dim hash As Integer = key.GetHashCode()
			Dim index As Integer = (hash And &H7FFFFFFF) Mod tab.Length
			Dim e As Entry(Of ?, ?) = tab(index)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Do While e IsNot Nothing
				If (e.hash = hash) AndAlso e.key.Equals(key) Then Return CType(e.value, V)
				e = e.next
			Loop
			Return Nothing
		End Function

		''' <summary>
		''' The maximum size of array to allocate.
		''' Some VMs reserve some header words in an array.
		''' Attempts to allocate larger arrays may result in
		''' OutOfMemoryError: Requested array size exceeds VM limit
		''' </summary>
		Private Shared ReadOnly MAX_ARRAY_SIZE As Integer =  java.lang.[Integer].MAX_VALUE - 8

		''' <summary>
		''' Increases the capacity of and internally reorganizes this
		''' hashtable, in order to accommodate and access its entries more
		''' efficiently.  This method is called automatically when the
		''' number of keys in the hashtable exceeds this hashtable's capacity
		''' and load factor.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend Overridable Sub rehash()
			Dim oldCapacity As Integer = table.Length
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim oldMap As Entry(Of ?, ?)() = table

			' overflow-conscious code
			Dim newCapacity As Integer = (oldCapacity << 1) + 1
			If newCapacity - MAX_ARRAY_SIZE > 0 Then
				If oldCapacity = MAX_ARRAY_SIZE Then Return
				newCapacity = MAX_ARRAY_SIZE
			End If
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim newMap As Entry(Of ?, ?)() = New Entry(Of ?, ?)(newCapacity - 1){}

			modCount += 1
			threshold = CInt(Fix (System.Math.Min(newCapacity * loadFactor, MAX_ARRAY_SIZE + 1)))
			table = newMap

			Dim i As Integer = oldCapacity
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While i -= 1 > 0
				Dim old As Entry(Of K, V) = CType(oldMap(i), Entry(Of K, V))
				Do While old IsNot Nothing
					Dim e As Entry(Of K, V) = old
					old = old.next

					Dim index As Integer = (e.hash And &H7FFFFFFF) Mod newCapacity
					e.next = CType(newMap(index), Entry(Of K, V))
					newMap(index) = e
				Loop
			Loop
		End Sub

		Private Sub addEntry(ByVal hash As Integer, ByVal key As K, ByVal value As V, ByVal index As Integer)
			modCount += 1

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim tab As Entry(Of ?, ?)() = table
			If count >= threshold Then
				' Rehash the table if the threshold is exceeded
				rehash()

				tab = table
				hash = key.GetHashCode()
				index = (hash And &H7FFFFFFF) Mod tab.Length
			End If

			' Creates the new entry.
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim e As Entry(Of K, V) = CType(tab(index), Entry(Of K, V))
			tab(index) = New Entry(Of )(hash, key, value, e)
			count += 1
		End Sub

		''' <summary>
		''' Maps the specified <code>key</code> to the specified
		''' <code>value</code> in this hashtable. Neither the key nor the
		''' value can be <code>null</code>. <p>
		''' 
		''' The value can be retrieved by calling the <code>get</code> method
		''' with a key that is equal to the original key.
		''' </summary>
		''' <param name="key">     the hashtable key </param>
		''' <param name="value">   the value </param>
		''' <returns>     the previous value of the specified key in this hashtable,
		'''             or <code>null</code> if it did not have one </returns>
		''' <exception cref="NullPointerException">  if the key or value is
		'''               <code>null</code> </exception>
		''' <seealso cref=     Object#equals(Object) </seealso>
		''' <seealso cref=     #get(Object) </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function put(ByVal key As K, ByVal value As V) As V Implements Map(Of K, V).put
			' Make sure the value is not null
			If value Is Nothing Then Throw New NullPointerException

			' Makes sure the key is not already in the hashtable.
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim tab As Entry(Of ?, ?)() = table
			Dim hash As Integer = key.GetHashCode()
			Dim index As Integer = (hash And &H7FFFFFFF) Mod tab.Length
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim entry As Entry(Of K, V) = CType(tab(index), Entry(Of K, V))
			Do While entry IsNot Nothing
				If (entry.hash = hash) AndAlso entry.key.Equals(key) Then
					Dim old As V = entry.value
					entry.value = value
					Return old
				End If
				entry = entry.next
			Loop

			addEntry(hash, key, value, index)
			Return Nothing
		End Function

		''' <summary>
		''' Removes the key (and its corresponding value) from this
		''' hashtable. This method does nothing if the key is not in the hashtable.
		''' </summary>
		''' <param name="key">   the key that needs to be removed </param>
		''' <returns>  the value to which the key had been mapped in this hashtable,
		'''          or <code>null</code> if the key did not have a mapping </returns>
		''' <exception cref="NullPointerException">  if the key is <code>null</code> </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function remove(ByVal key As Object) As V Implements Map(Of K, V).remove
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim tab As Entry(Of ?, ?)() = table
			Dim hash As Integer = key.GetHashCode()
			Dim index As Integer = (hash And &H7FFFFFFF) Mod tab.Length
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim e As Entry(Of K, V) = CType(tab(index), Entry(Of K, V))
			Dim prev As Entry(Of K, V) = Nothing
			Do While e IsNot Nothing
				If (e.hash = hash) AndAlso e.key.Equals(key) Then
					modCount += 1
					If prev IsNot Nothing Then
						prev.next = e.next
					Else
						tab(index) = e.next
					End If
					count -= 1
					Dim oldValue As V = e.value
					e.value = Nothing
					Return oldValue
				End If
				prev = e
				e = e.next
			Loop
			Return Nothing
		End Function

		''' <summary>
		''' Copies all of the mappings from the specified map to this hashtable.
		''' These mappings will replace any mappings that this hashtable had for any
		''' of the keys currently in the specified map.
		''' </summary>
		''' <param name="t"> mappings to be stored in this map </param>
		''' <exception cref="NullPointerException"> if the specified map is null
		''' @since 1.2 </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub putAll(Of T1 As K, ? As V)(ByVal t As Map(Of T1)) Implements Map(Of K, V).putAll
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			For Each e As KeyValuePair(Of ? As K, ? As V) In t.entrySet()
				put(e.Key, e.Value)
			Next e
		End Sub

		''' <summary>
		''' Clears this hashtable so that it contains no keys.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub clear() Implements Map(Of K, V).clear
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim tab As Entry(Of ?, ?)() = table
			modCount += 1
			Dim index As Integer = tab.Length
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While index -= 1 >= 0
				tab(index) = Nothing
			Loop
			count = 0
		End Sub

		''' <summary>
		''' Creates a shallow copy of this hashtable. All the structure of the
		''' hashtable itself is copied, but the keys and values are not cloned.
		''' This is a relatively expensive operation.
		''' </summary>
		''' <returns>  a clone of the hashtable </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function clone() As Object
			Try
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim t As Dictionary(Of ?, ?) = CType(MyBase.clone(), Dictionary(Of ?, ?))
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				t.table = New Entry(Of ?, ?)(table.Length - 1){}
				Dim i As Integer = table.Length
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While i -= 1 > 0
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					t.table(i) = If(table(i) IsNot Nothing, CType(table(i).clone(), Entry(Of ?, ?)), Nothing)
				Loop
				t.keySet_Renamed = Nothing
				t.entrySet_Renamed = Nothing
				t.values_Renamed = Nothing
				t.modCount = 0
				Return t
			Catch e As CloneNotSupportedException
				' this shouldn't happen, since we are Cloneable
				Throw New InternalError(e)
			End Try
		End Function

		''' <summary>
		''' Returns a string representation of this <tt>Hashtable</tt> object
		''' in the form of a set of entries, enclosed in braces and separated
		''' by the ASCII characters "<tt>,&nbsp;</tt>" (comma and space). Each
		''' entry is rendered as the key, an equals sign <tt>=</tt>, and the
		''' associated element, where the <tt>toString</tt> method is used to
		''' convert the key and element to strings.
		''' </summary>
		''' <returns>  a string representation of this hashtable </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function ToString() As String
			Dim max As Integer = size() - 1
			If max = -1 Then Return "{}"

			Dim sb As New StringBuilder
			Dim it As [Iterator](Of KeyValuePair(Of K, V)) = entrySet().GetEnumerator()

			sb.append("{"c)
			Dim i As Integer = 0
			Do
				Dim e As KeyValuePair(Of K, V) = it.next()
				Dim key As K = e.Key
				Dim value As V = e.Value
				sb.append(If(key Is Me, "(this Map)", key.ToString()))
				sb.append("="c)
				sb.append(If(value Is Me, "(this Map)", value.ToString()))

				If i = max Then Return sb.append("}"c).ToString()
				sb.append(", ")
				i += 1
			Loop
		End Function


		Private Function getEnumeration(Of T)(ByVal type As Integer) As Enumeration(Of T)
			If count = 0 Then
				Return Collections.emptyEnumeration()
			Else
				Return New Enumerator(Me, Of )(type, False)
			End If
		End Function

		Private Function getIterator(Of T)(ByVal type As Integer) As [Iterator](Of T)
			If count = 0 Then
				Return Collections.emptyIterator()
			Else
				Return New Enumerator(Me, Of )(type, True)
			End If
		End Function

		' Views

		''' <summary>
		''' Each of these fields are initialized to contain an instance of the
		''' appropriate view the first time this view is requested.  The views are
		''' stateless, so there's no reason to create more than one of each.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private keySet_Renamed As [Set](Of K)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private entrySet_Renamed As [Set](Of KeyValuePair(Of K, V))
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private values_Renamed As Collection(Of V)

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
		''' 
		''' @since 1.2
		''' </summary>
		Public Overridable Function keySet() As [Set](Of K) Implements Map(Of K, V).keySet
			If keySet_Renamed Is Nothing Then keySet_Renamed = Collections.synchronizedSet(New KeySet(Me), Me)
			Return keySet_Renamed
		End Function

		Private Class KeySet
			Inherits AbstractSet(Of K)

			Private ReadOnly outerInstance As Hashtable

			Public Sub New(ByVal outerInstance As Hashtable)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Function [iterator]() As [Iterator](Of K)
				Return outerInstance.getIterator(KEYS_Renamed)
			End Function
			Public Overridable Function size() As Integer
				Return outerInstance.count
			End Function
			Public Overridable Function contains(ByVal o As Object) As Boolean
				Return outerInstance.containsKey(o)
			End Function
			Public Overridable Function remove(ByVal o As Object) As Boolean
				Return outerInstance.remove(o) IsNot Nothing
			End Function
			Public Overridable Sub clear()
				outerInstance.clear()
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
		''' 
		''' @since 1.2
		''' </summary>
		Public Overridable Function entrySet() As [Set](Of KeyValuePair(Of K, V)) Implements Map(Of K, V).entrySet
			If entrySet_Renamed Is Nothing Then entrySet_Renamed = Collections.synchronizedSet(New EntrySet(Me), Me)
			Return entrySet_Renamed
		End Function

		Private Class EntrySet
			Inherits AbstractSet(Of KeyValuePair(Of K, V))

			Private ReadOnly outerInstance As Hashtable

			Public Sub New(ByVal outerInstance As Hashtable)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Function [iterator]() As [Iterator](Of KeyValuePair(Of K, V))
				Return outerInstance.getIterator(ENTRIES)
			End Function

			Public Overridable Function add(ByVal o As KeyValuePair(Of K, V)) As Boolean
				Return MyBase.add(o)
			End Function

			Public Overridable Function contains(ByVal o As Object) As Boolean
				If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim entry As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
				Dim key As Object = entry.Key
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim tab As Entry(Of ?, ?)() = outerInstance.table
				Dim hash As Integer = key.GetHashCode()
				Dim index As Integer = (hash And &H7FFFFFFF) Mod tab.Length

				Dim e As Entry(Of ?, ?) = tab(index)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Do While e IsNot Nothing
					If e.hash=hash AndAlso e.Equals(entry) Then Return True
					e = e.next
				Loop
				Return False
			End Function

			Public Overridable Function remove(ByVal o As Object) As Boolean
				If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim entry As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
				Dim key As Object = entry.Key
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim tab As Entry(Of ?, ?)() = outerInstance.table
				Dim hash As Integer = key.GetHashCode()
				Dim index As Integer = (hash And &H7FFFFFFF) Mod tab.Length

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim e As Entry(Of K, V) = CType(tab(index), Entry(Of K, V))
				Dim prev As Entry(Of K, V) = Nothing
				Do While e IsNot Nothing
					If e.hash=hash AndAlso e.Equals(entry) Then
						outerInstance.modCount += 1
						If prev IsNot Nothing Then
							prev.next = e.next
						Else
							tab(index) = e.next
						End If

						outerInstance.count -= 1
						e.value = Nothing
						Return True
					End If
					prev = e
					e = e.next
				Loop
				Return False
			End Function

			Public Overridable Function size() As Integer
				Return outerInstance.count
			End Function

			Public Overridable Sub clear()
				outerInstance.clear()
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
		''' 
		''' @since 1.2
		''' </summary>
		Public Overridable Function values() As Collection(Of V) Implements Map(Of K, V).values
			If values_Renamed Is Nothing Then values_Renamed = Collections.synchronizedCollection(New ValueCollection(Me), Me)
			Return values_Renamed
		End Function

		Private Class ValueCollection
			Inherits AbstractCollection(Of V)

			Private ReadOnly outerInstance As Hashtable

			Public Sub New(ByVal outerInstance As Hashtable)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Function [iterator]() As [Iterator](Of V)
				Return outerInstance.getIterator(VALUES_Renamed)
			End Function
			Public Overridable Function size() As Integer
				Return outerInstance.count
			End Function
			Public Overridable Function contains(ByVal o As Object) As Boolean
				Return outerInstance.containsValue(o)
			End Function
			Public Overridable Sub clear()
				outerInstance.clear()
			End Sub
		End Class

		' Comparison and hashing

		''' <summary>
		''' Compares the specified Object with this Map for equality,
		''' as per the definition in the Map interface.
		''' </summary>
		''' <param name="o"> object to be compared for equality with this hashtable </param>
		''' <returns> true if the specified Object is equal to this Map </returns>
		''' <seealso cref= Map#equals(Object)
		''' @since 1.2 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o Is Me Then Return True

			If Not(TypeOf o Is Map) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim t As Map(Of ?, ?) = CType(o, Map(Of ?, ?))
			If t.size() IsNot size() Then Return False

			Try
				Dim i As [Iterator](Of KeyValuePair(Of K, V)) = entrySet().GetEnumerator()
				Do While i.MoveNext()
					Dim e As KeyValuePair(Of K, V) = i.Current
					Dim key As K = e.Key
					Dim value As V = e.Value
					If value Is Nothing Then
						If Not(t.get(key) Is Nothing AndAlso t.containsKey(key)) Then Return False
					Else
						If Not value.Equals(t.get(key)) Then Return False
					End If
				Loop
			Catch unused As  [Class]CastException
				Return False
			Catch unused As NullPointerException
				Return False
			End Try

			Return True
		End Function

		''' <summary>
		''' Returns the hash code value for this Map as per the definition in the
		''' Map interface.
		''' </summary>
		''' <seealso cref= Map#hashCode()
		''' @since 1.2 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function GetHashCode() As Integer
	'        
	'         * This code detects the recursion caused by computing the hash code
	'         * of a self-referential hash table and prevents the stack overflow
	'         * that would otherwise result.  This allows certain 1.1-era
	'         * applets with self-referential hash tables to work.  This code
	'         * abuses the loadFactor field to do double-duty as a hashCode
	'         * in progress flag, so as not to worsen the space performance.
	'         * A negative load factor indicates that hash code computation is
	'         * in progress.
	'         
			Dim h As Integer = 0
			If count = 0 OrElse loadFactor < 0 Then Return h ' Returns zero

			loadFactor = -loadFactor ' Mark hashCode computation in progress
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim tab As Entry(Of ?, ?)() = table
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			For Each entry As Entry(Of ?, ?) In tab
				Do While entry IsNot Nothing
					h += entry.GetHashCode()
					entry = entry.next
				Loop
			Next entry

			loadFactor = -loadFactor ' Mark hashCode computation complete

			Return h
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function getOrDefault(ByVal key As Object, ByVal defaultValue As V) As V Implements Map(Of K, V).getOrDefault
			Dim result As V = [get](key)
			Return If(Nothing Is result, defaultValue, result)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub forEach(Of T1)(ByVal action As java.util.function.BiConsumer(Of T1)) Implements Map(Of K, V).forEach
			Objects.requireNonNull(action) ' explicit check required in case
												' table is empty.
			Dim expectedModCount As Integer = modCount

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim tab As Entry(Of ?, ?)() = table
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			For Each entry As Entry(Of ?, ?) In tab
				Do While entry IsNot Nothing
					action.accept(CType(entry.key, K), CType(entry.value, V))
					entry = entry.next

					If expectedModCount <> modCount Then Throw New ConcurrentModificationException
				Loop
			Next entry
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub replaceAll(Of T1 As V)(ByVal [function] As java.util.function.BiFunction(Of T1)) Implements Map(Of K, V).replaceAll
			Objects.requireNonNull([function]) ' explicit check required in case
												  ' table is empty.
			Dim expectedModCount As Integer = modCount

			Dim tab As Entry(Of K, V)() = CType(table, Entry(Of K, V)())
			For Each entry As Entry(Of K, V) In tab
				Do While entry IsNot Nothing
					entry.value = Objects.requireNonNull([function].apply(entry.key, entry.value))
					entry = entry.next

					If expectedModCount <> modCount Then Throw New ConcurrentModificationException
				Loop
			Next entry
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function putIfAbsent(ByVal key As K, ByVal value As V) As V Implements Map(Of K, V).putIfAbsent
			Objects.requireNonNull(value)

			' Makes sure the key is not already in the hashtable.
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim tab As Entry(Of ?, ?)() = table
			Dim hash As Integer = key.GetHashCode()
			Dim index As Integer = (hash And &H7FFFFFFF) Mod tab.Length
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim entry As Entry(Of K, V) = CType(tab(index), Entry(Of K, V))
			Do While entry IsNot Nothing
				If (entry.hash = hash) AndAlso entry.key.Equals(key) Then
					Dim old As V = entry.value
					If old Is Nothing Then entry.value = value
					Return old
				End If
				entry = entry.next
			Loop

			addEntry(hash, key, value, index)
			Return Nothing
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function remove(ByVal key As Object, ByVal value As Object) As Boolean Implements Map(Of K, V).remove
			Objects.requireNonNull(value)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim tab As Entry(Of ?, ?)() = table
			Dim hash As Integer = key.GetHashCode()
			Dim index As Integer = (hash And &H7FFFFFFF) Mod tab.Length
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim e As Entry(Of K, V) = CType(tab(index), Entry(Of K, V))
			Dim prev As Entry(Of K, V) = Nothing
			Do While e IsNot Nothing
				If (e.hash = hash) AndAlso e.key.Equals(key) AndAlso e.value.Equals(value) Then
					modCount += 1
					If prev IsNot Nothing Then
						prev.next = e.next
					Else
						tab(index) = e.next
					End If
					count -= 1
					e.value = Nothing
					Return True
				End If
				prev = e
				e = e.next
			Loop
			Return False
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function replace(ByVal key As K, ByVal oldValue As V, ByVal newValue As V) As Boolean Implements Map(Of K, V).replace
			Objects.requireNonNull(oldValue)
			Objects.requireNonNull(newValue)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim tab As Entry(Of ?, ?)() = table
			Dim hash As Integer = key.GetHashCode()
			Dim index As Integer = (hash And &H7FFFFFFF) Mod tab.Length
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim e As Entry(Of K, V) = CType(tab(index), Entry(Of K, V))
			Do While e IsNot Nothing
				If (e.hash = hash) AndAlso e.key.Equals(key) Then
					If e.value.Equals(oldValue) Then
						e.value = newValue
						Return True
					Else
						Return False
					End If
				End If
				e = e.next
			Loop
			Return False
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function replace(ByVal key As K, ByVal value As V) As V Implements Map(Of K, V).replace
			Objects.requireNonNull(value)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim tab As Entry(Of ?, ?)() = table
			Dim hash As Integer = key.GetHashCode()
			Dim index As Integer = (hash And &H7FFFFFFF) Mod tab.Length
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim e As Entry(Of K, V) = CType(tab(index), Entry(Of K, V))
			Do While e IsNot Nothing
				If (e.hash = hash) AndAlso e.key.Equals(key) Then
					Dim oldValue As V = e.value
					e.value = value
					Return oldValue
				End If
				e = e.next
			Loop
			Return Nothing
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function computeIfAbsent(Of T1 As V)(ByVal key As K, ByVal mappingFunction As java.util.function.Function(Of T1)) As V Implements Map(Of K, V).computeIfAbsent
			Objects.requireNonNull(mappingFunction)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim tab As Entry(Of ?, ?)() = table
			Dim hash As Integer = key.GetHashCode()
			Dim index As Integer = (hash And &H7FFFFFFF) Mod tab.Length
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim e As Entry(Of K, V) = CType(tab(index), Entry(Of K, V))
			Do While e IsNot Nothing
				If e.hash = hash AndAlso e.key.Equals(key) Then Return e.value
				e = e.next
			Loop

			Dim newValue As V = mappingFunction.apply(key)
			If newValue IsNot Nothing Then addEntry(hash, key, newValue, index)

			Return newValue
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function computeIfPresent(Of T1 As V)(ByVal key As K, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V Implements Map(Of K, V).computeIfPresent
			Objects.requireNonNull(remappingFunction)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim tab As Entry(Of ?, ?)() = table
			Dim hash As Integer = key.GetHashCode()
			Dim index As Integer = (hash And &H7FFFFFFF) Mod tab.Length
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim e As Entry(Of K, V) = CType(tab(index), Entry(Of K, V))
			Dim prev As Entry(Of K, V) = Nothing
			Do While e IsNot Nothing
				If e.hash = hash AndAlso e.key.Equals(key) Then
					Dim newValue As V = remappingFunction.apply(key, e.value)
					If newValue Is Nothing Then
						modCount += 1
						If prev IsNot Nothing Then
							prev.next = e.next
						Else
							tab(index) = e.next
						End If
						count -= 1
					Else
						e.value = newValue
					End If
					Return newValue
				End If
				prev = e
				e = e.next
			Loop
			Return Nothing
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function compute(Of T1 As V)(ByVal key As K, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V Implements Map(Of K, V).compute
			Objects.requireNonNull(remappingFunction)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim tab As Entry(Of ?, ?)() = table
			Dim hash As Integer = key.GetHashCode()
			Dim index As Integer = (hash And &H7FFFFFFF) Mod tab.Length
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim e As Entry(Of K, V) = CType(tab(index), Entry(Of K, V))
			Dim prev As Entry(Of K, V) = Nothing
			Do While e IsNot Nothing
				If e.hash = hash AndAlso Objects.Equals(e.key, key) Then
					Dim newValue As V = remappingFunction.apply(key, e.value)
					If newValue Is Nothing Then
						modCount += 1
						If prev IsNot Nothing Then
							prev.next = e.next
						Else
							tab(index) = e.next
						End If
						count -= 1
					Else
						e.value = newValue
					End If
					Return newValue
				End If
				prev = e
				e = e.next
			Loop

			Dim newValue As V = remappingFunction.apply(key, Nothing)
			If newValue IsNot Nothing Then addEntry(hash, key, newValue, index)

			Return newValue
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function merge(Of T1 As V)(ByVal key As K, ByVal value As V, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V Implements Map(Of K, V).merge
			Objects.requireNonNull(remappingFunction)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim tab As Entry(Of ?, ?)() = table
			Dim hash As Integer = key.GetHashCode()
			Dim index As Integer = (hash And &H7FFFFFFF) Mod tab.Length
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim e As Entry(Of K, V) = CType(tab(index), Entry(Of K, V))
			Dim prev As Entry(Of K, V) = Nothing
			Do While e IsNot Nothing
				If e.hash = hash AndAlso e.key.Equals(key) Then
					Dim newValue As V = remappingFunction.apply(e.value, value)
					If newValue Is Nothing Then
						modCount += 1
						If prev IsNot Nothing Then
							prev.next = e.next
						Else
							tab(index) = e.next
						End If
						count -= 1
					Else
						e.value = newValue
					End If
					Return newValue
				End If
				prev = e
				e = e.next
			Loop

			If value IsNot Nothing Then addEntry(hash, key, value, index)

			Return value
		End Function

		''' <summary>
		''' Save the state of the Hashtable to a stream (i.e., serialize it).
		''' 
		''' @serialData The <i>capacity</i> of the Hashtable (the length of the
		'''             bucket array) is emitted (int), followed by the
		'''             <i>size</i> of the Hashtable (the number of key-value
		'''             mappings), followed by the key (Object) and value (Object)
		'''             for each key-value mapping represented by the Hashtable
		'''             The key-value mappings are emitted in no particular order.
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			Dim entryStack As Entry(Of Object, Object) = Nothing

			SyncLock Me
				' Write out the length, threshold, loadfactor
				s.defaultWriteObject()

				' Write out length, count of elements
				s.writeInt(table.Length)
				s.writeInt(count)

				' Stack copies of the entries in the table
				For index As Integer = 0 To table.Length - 1
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim entry As Entry(Of ?, ?) = table(index)

					Do While entry IsNot Nothing
						entryStack = New Entry(Of )(0, entry.key, entry.value, entryStack)
						entry = entry.next
					Loop
				Next index
			End SyncLock

			' Write out the key/value objects from the stacked entries
			Do While entryStack IsNot Nothing
				s.writeObject(entryStack.key)
				s.writeObject(entryStack.value)
				entryStack = entryStack.next
			Loop
		End Sub

		''' <summary>
		''' Reconstitute the Hashtable from a stream (i.e., deserialize it).
		''' </summary>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			' Read in the length, threshold, and loadfactor
			s.defaultReadObject()

			' Read the original length of the array and number of elements
			Dim origlength As Integer = s.readInt()
			Dim elements As Integer = s.readInt()

			' Compute new size with a bit of room 5% to grow but
			' no larger than the original size.  Make the length
			' odd if it's large enough, this helps distribute the entries.
			' Guard against the length ending up zero, that's not valid.
			Dim length As Integer = CInt(Fix(elements * loadFactor)) + (elements \ 20) + 3
			If length > elements AndAlso (length And 1) = 0 Then length -= 1
			If origlength > 0 AndAlso length > origlength Then length = origlength
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			table = New Entry(Of ?, ?)(length - 1){}
			threshold = CInt(Fix (System.Math.Min(length * loadFactor, MAX_ARRAY_SIZE + 1)))
			count = 0

			' Read the number of elements and then all the key/value objects
			Do While elements > 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim key As K = CType(s.readObject(), K)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim value As V = CType(s.readObject(), V)
				' synch could be eliminated for performance
				reconstitutionPut(table, key, value)
				elements -= 1
			Loop
		End Sub

		''' <summary>
		''' The put method used by readObject. This is provided because put
		''' is overridable and should not be called in readObject since the
		''' subclass will not yet be initialized.
		''' 
		''' <p>This differs from the regular put method in several ways. No
		''' checking for rehashing is necessary since the number of elements
		''' initially in the table is known. The modCount is not incremented
		''' because we are creating a new instance. Also, no return value
		''' is needed.
		''' </summary>
		Private Sub reconstitutionPut(Of T1)(ByVal tab As Entry(Of T1)(), ByVal key As K, ByVal value As V)
			If value Is Nothing Then Throw New java.io.StreamCorruptedException
			' Makes sure the key is not already in the hashtable.
			' This should not happen in deserialized version.
			Dim hash As Integer = key.GetHashCode()
			Dim index As Integer = (hash And &H7FFFFFFF) Mod tab.Length
			Dim e As Entry(Of ?, ?) = tab(index)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Do While e IsNot Nothing
				If (e.hash = hash) AndAlso e.key.Equals(key) Then Throw New java.io.StreamCorruptedException
				e = e.next
			Loop
			' Creates the new entry.
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim e As Entry(Of K, V) = CType(tab(index), Entry(Of K, V))
			tab(index) = New Entry(Of )(hash, key, value, e)
			count += 1
		End Sub

		''' <summary>
		''' Hashtable bucket collision list entry
		''' </summary>
		Private Class Entry(Of K, V)
			Implements KeyValuePair(Of K, V)

			Friend ReadOnly hash As Integer
			Friend ReadOnly key As K
			Friend value As V
			Friend [next] As Entry(Of K, V)

			Protected Friend Sub New(ByVal hash As Integer, ByVal key As K, ByVal value As V, ByVal [next] As Entry(Of K, V))
				Me.hash = hash
				Me.key = key
				Me.value = value
				Me.next = [next]
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Protected Friend Overridable Function clone() As Object
				Return New Entry(Of )(hash, key, value, (If([next] Is Nothing, Nothing, CType([next].clone(), Entry(Of K, V)))))
			End Function

			' Map.Entry Ops

			Public Overridable Property key As K
				Get
					Return key
				End Get
			End Property

			Public Overridable Property value As V
				Get
					Return value
				End Get
			End Property

			Public Overridable Function setValue(ByVal value As V) As V
				If value Is Nothing Then Throw New NullPointerException

				Dim oldValue As V = Me.value
				Me.value = value
				Return oldValue
			End Function

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))

				Return (If(key Is Nothing, e.Key Is Nothing, key.Equals(e.Key))) AndAlso (If(value Is Nothing, e.Value Is Nothing, value.Equals(e.Value)))
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return hash Xor Objects.hashCode(value)
			End Function

			Public Overrides Function ToString() As String
				Return key.ToString() & "=" & value.ToString()
			End Function
		End Class

		' Types of Enumerations/Iterations
		Private Const KEYS_Renamed As Integer = 0
		Private Const VALUES_Renamed As Integer = 1
		Private Const ENTRIES As Integer = 2

		''' <summary>
		''' A hashtable enumerator class.  This class implements both the
		''' Enumeration and Iterator interfaces, but individual instances
		''' can be created with the Iterator methods disabled.  This is necessary
		''' to avoid unintentionally increasing the capabilities granted a user
		''' by passing an Enumeration.
		''' </summary>
		Private Class Enumerator(Of T)
			Implements Enumeration(Of T), Iterator(Of T)

			Private ReadOnly outerInstance As Hashtable

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend table As Entry(Of ?, ?)() = outerInstance.table
			Friend index As Integer = table.Length
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend entry As Entry(Of ?, ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend lastReturned As Entry(Of ?, ?)
			Friend type As Integer

			''' <summary>
			''' Indicates whether this Enumerator is serving as an Iterator
			''' or an Enumeration.  (true -> Iterator).
			''' </summary>
			Friend [iterator] As Boolean

			''' <summary>
			''' The modCount value that the iterator believes that the backing
			''' Hashtable should have.  If this expectation is violated, the iterator
			''' has detected concurrent modification.
			''' </summary>
			Protected Friend expectedModCount As Integer = outerInstance.modCount

			Friend Sub New(ByVal outerInstance As Hashtable, ByVal type As Integer, ByVal [iterator] As Boolean)
					Me.outerInstance = outerInstance
				Me.type = type
				Me.iterator = [iterator]
			End Sub

			Public Overridable Function hasMoreElements() As Boolean Implements Enumeration(Of T).hasMoreElements
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e As Entry(Of ?, ?) = entry
				Dim i As Integer = index
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim t As Entry(Of ?, ?)() = table
				' Use locals for faster loop iteration 
				Do While e Is Nothing AndAlso i > 0
					i -= 1
					e = t(i)
				Loop
				entry = e
				index = i
				Return e IsNot Nothing
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Function nextElement() As T
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim et As Entry(Of ?, ?) = entry
				Dim i As Integer = index
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim t As Entry(Of ?, ?)() = table
				' Use locals for faster loop iteration 
				Do While et Is Nothing AndAlso i > 0
					i -= 1
					et = t(i)
				Loop
				entry = et
				index = i
				If et IsNot Nothing Then
						lastReturned = entry
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Dim e As Entry(Of ?, ?) = lastReturned
					entry = e.next
					Return If(type = KEYS_Renamed, CType(e.key, T), (If(type = VALUES_Renamed, CType(e.value, T), CType(e, T))))
				End If
				Throw New NoSuchElementException("Hashtable Enumerator")
			End Function

			' Iterator methods
			Public Overridable Function hasNext() As Boolean Implements Iterator(Of T).hasNext
				Return hasMoreElements()
			End Function

			Public Overridable Function [next]() As T
				If outerInstance.modCount <> expectedModCount Then Throw New ConcurrentModificationException
				Return nextElement()
			End Function

			Public Overridable Sub remove() Implements Iterator(Of T).remove
				If Not [iterator] Then Throw New UnsupportedOperationException
				If lastReturned Is Nothing Then Throw New IllegalStateException("Hashtable Enumerator")
				If outerInstance.modCount <> expectedModCount Then Throw New ConcurrentModificationException

				SyncLock Hashtable.this
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim tab As Entry(Of ?, ?)() = outerInstance.table
					Dim index As Integer = (lastReturned.hash And &H7FFFFFFF) Mod tab.Length

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim e As Entry(Of K, V) = CType(tab(index), Entry(Of K, V))
					Dim prev As Entry(Of K, V) = Nothing
					Do While e IsNot Nothing
						If e Is lastReturned Then
							outerInstance.modCount += 1
							expectedModCount += 1
							If prev Is Nothing Then
								tab(index) = e.next
							Else
								prev.next = e.next
							End If
							outerInstance.count -= 1
							lastReturned = Nothing
							Return
						End If
						prev = e
						e = e.next
					Loop
					Throw New ConcurrentModificationException
				End SyncLock
			End Sub
		End Class
	End Class

End Namespace
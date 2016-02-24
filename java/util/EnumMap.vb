Imports System
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 2003, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' A specialized <seealso cref="Map"/> implementation for use with enum type keys.  All
	''' of the keys in an enum map must come from a single enum type that is
	''' specified, explicitly or implicitly, when the map is created.  Enum maps
	''' are represented internally as arrays.  This representation is extremely
	''' compact and efficient.
	''' 
	''' <p>Enum maps are maintained in the <i>natural order</i> of their keys
	''' (the order in which the enum constants are declared).  This is reflected
	''' in the iterators returned by the collections views (<seealso cref="#keySet()"/>,
	''' <seealso cref="#entrySet()"/>, and <seealso cref="#values()"/>).
	''' 
	''' <p>Iterators returned by the collection views are <i>weakly consistent</i>:
	''' they will never throw <seealso cref="ConcurrentModificationException"/> and they may
	''' or may not show the effects of any modifications to the map that occur while
	''' the iteration is in progress.
	''' 
	''' <p>Null keys are not permitted.  Attempts to insert a null key will
	''' throw <seealso cref="NullPointerException"/>.  Attempts to test for the
	''' presence of a null key or to remove one will, however, function properly.
	''' Null values are permitted.
	''' 
	''' <P>Like most collection implementations <tt>EnumMap</tt> is not
	''' synchronized. If multiple threads access an enum map concurrently, and at
	''' least one of the threads modifies the map, it should be synchronized
	''' externally.  This is typically accomplished by synchronizing on some
	''' object that naturally encapsulates the enum map.  If no such object exists,
	''' the map should be "wrapped" using the <seealso cref="Collections#synchronizedMap"/>
	''' method.  This is best done at creation time, to prevent accidental
	''' unsynchronized access:
	''' 
	''' <pre>
	'''     Map&lt;EnumKey, V&gt; m
	'''         = Collections.synchronizedMap(new EnumMap&lt;EnumKey, V&gt;(...));
	''' </pre>
	''' 
	''' <p>Implementation note: All basic operations execute in constant time.
	''' They are likely (though not guaranteed) to be faster than their
	''' <seealso cref="HashMap"/> counterparts.
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @author Josh Bloch </summary>
	''' <seealso cref= EnumSet
	''' @since 1.5 </seealso>
	<Serializable> _
	Public Class EnumMap(Of K As System.Enum(Of K), V)
		Inherits AbstractMap(Of K, V)
		Implements Cloneable

		''' <summary>
		''' The <tt>Class</tt> object for the enum type of all the keys of this map.
		''' 
		''' @serial
		''' </summary>
		Private ReadOnly keyType As Class

		''' <summary>
		''' All of the values comprising K.  (Cached for performance.)
		''' </summary>
		<NonSerialized> _
		Private keyUniverse As K()

		''' <summary>
		''' Array representation of this map.  The ith element is the value
		''' to which universe[i] is currently mapped, or null if it isn't
		''' mapped to anything, or NULL if it's mapped to null.
		''' </summary>
		<NonSerialized> _
		Private vals As Object()

		''' <summary>
		''' The number of mappings in this map.
		''' </summary>
		<NonSerialized> _
		Private size_Renamed As Integer = 0

		''' <summary>
		''' Distinguished non-null value for representing null values.
		''' </summary>
		Private Shared ReadOnly NULL As Object = New ObjectAnonymousInnerClassHelper

		Private Class ObjectAnonymousInnerClassHelper
			Inherits Object

			Public Overrides Function GetHashCode() As Integer
				Return 0
			End Function

			Public Overrides Function ToString() As String
				Return "java.util.EnumMap.NULL"
			End Function
		End Class

		Private Function maskNull(ByVal value As Object) As Object
			Return (If(value Is Nothing, NULL, value))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Function unmaskNull(ByVal value As Object) As V
			Return CType(If(value Is NULL, Nothing, value), V)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Shared ReadOnly ZERO_LENGTH_ENUM_ARRAY As System.Enum(Of ?)() = New [Enum](Of ?)(){}

		''' <summary>
		''' Creates an empty enum map with the specified key type.
		''' </summary>
		''' <param name="keyType"> the class object of the key type for this enum map </param>
		''' <exception cref="NullPointerException"> if <tt>keyType</tt> is null </exception>
		Public Sub New(ByVal keyType As Class)
			Me.keyType = keyType
			keyUniverse = getKeyUniverse(keyType)
			vals = New Object(keyUniverse.Length - 1){}
		End Sub

		''' <summary>
		''' Creates an enum map with the same key type as the specified enum
		''' map, initially containing the same mappings (if any).
		''' </summary>
		''' <param name="m"> the enum map from which to initialize this enum map </param>
		''' <exception cref="NullPointerException"> if <tt>m</tt> is null </exception>
		Public Sub New(Of T1 As V)(ByVal m As EnumMap(Of T1))
			keyType = m.keyType
			keyUniverse = m.keyUniverse
			vals = m.vals.clone()
			size_Renamed = m.size_Renamed
		End Sub

		''' <summary>
		''' Creates an enum map initialized from the specified map.  If the
		''' specified map is an <tt>EnumMap</tt> instance, this constructor behaves
		''' identically to <seealso cref="#EnumMap(EnumMap)"/>.  Otherwise, the specified map
		''' must contain at least one mapping (in order to determine the new
		''' enum map's key type).
		''' </summary>
		''' <param name="m"> the map from which to initialize this enum map </param>
		''' <exception cref="IllegalArgumentException"> if <tt>m</tt> is not an
		'''     <tt>EnumMap</tt> instance and contains no mappings </exception>
		''' <exception cref="NullPointerException"> if <tt>m</tt> is null </exception>
		Public Sub New(Of T1 As V)(ByVal m As Map(Of T1))
			If TypeOf m Is EnumMap Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim em As EnumMap(Of K, ? As V) = CType(m, EnumMap(Of K, ? As V))
				keyType = em.keyType
				keyUniverse = em.keyUniverse
				vals = em.vals.clone()
				size_Renamed = em.size_Renamed
			Else
				If m.empty Then Throw New IllegalArgumentException("Specified map is empty")
				keyType = m.Keys.GetEnumerator().next().declaringClass
				keyUniverse = getKeyUniverse(keyType)
				vals = New Object(keyUniverse.Length - 1){}
				putAll(m)
			End If
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
		''' Returns <tt>true</tt> if this map maps one or more keys to the
		''' specified value.
		''' </summary>
		''' <param name="value"> the value whose presence in this map is to be tested </param>
		''' <returns> <tt>true</tt> if this map maps one or more keys to this value </returns>
		Public Overridable Function containsValue(ByVal value As Object) As Boolean
			value = maskNull(value)

			For Each val As Object In vals
				If value.Equals(val) Then Return True
			Next val

			Return False
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if this map contains a mapping for the specified
		''' key.
		''' </summary>
		''' <param name="key"> the key whose presence in this map is to be tested </param>
		''' <returns> <tt>true</tt> if this map contains a mapping for the specified
		'''            key </returns>
		Public Overridable Function containsKey(ByVal key As Object) As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Return isValidKey(key) AndAlso vals(CType(key, Enum(Of ?)).ordinal()) IsNot Nothing
		End Function

		Private Function containsMapping(ByVal key As Object, ByVal value As Object) As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Return isValidKey(key) AndAlso maskNull(value).Equals(vals(CType(key, Enum(Of ?)).ordinal()))
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
		Public Overridable Function [get](ByVal key As Object) As V
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Return (If(isValidKey(key), unmaskNull(vals(CType(key, Enum(Of ?)).ordinal())), Nothing))
		End Function

		' Modification Operations

		''' <summary>
		''' Associates the specified value with the specified key in this map.
		''' If the map previously contained a mapping for this key, the old
		''' value is replaced.
		''' </summary>
		''' <param name="key"> the key with which the specified value is to be associated </param>
		''' <param name="value"> the value to be associated with the specified key
		''' </param>
		''' <returns> the previous value associated with specified key, or
		'''     <tt>null</tt> if there was no mapping for key.  (A <tt>null</tt>
		'''     return can also indicate that the map previously associated
		'''     <tt>null</tt> with the specified key.) </returns>
		''' <exception cref="NullPointerException"> if the specified key is null </exception>
		Public Overridable Function put(ByVal key As K, ByVal value As V) As V
			typeCheck(key)

			Dim index As Integer = key.ordinal()
			Dim oldValue As Object = vals(index)
			vals(index) = maskNull(value)
			If oldValue Is Nothing Then size_Renamed += 1
			Return unmaskNull(oldValue)
		End Function

		''' <summary>
		''' Removes the mapping for this key from this map if present.
		''' </summary>
		''' <param name="key"> the key whose mapping is to be removed from the map </param>
		''' <returns> the previous value associated with specified key, or
		'''     <tt>null</tt> if there was no entry for key.  (A <tt>null</tt>
		'''     return can also indicate that the map previously associated
		'''     <tt>null</tt> with the specified key.) </returns>
		Public Overridable Function remove(ByVal key As Object) As V
			If Not isValidKey(key) Then Return Nothing
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim index As Integer = CType(key, Enum(Of ?)).ordinal()
			Dim oldValue As Object = vals(index)
			vals(index) = Nothing
			If oldValue IsNot Nothing Then size_Renamed -= 1
			Return unmaskNull(oldValue)
		End Function

		Private Function removeMapping(ByVal key As Object, ByVal value As Object) As Boolean
			If Not isValidKey(key) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim index As Integer = CType(key, Enum(Of ?)).ordinal()
			If maskNull(value).Equals(vals(index)) Then
				vals(index) = Nothing
				size_Renamed -= 1
				Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Returns true if key is of the proper type to be a key in this
		''' enum map.
		''' </summary>
		Private Function isValidKey(ByVal key As Object) As Boolean
			If key Is Nothing Then Return False

			' Cheaper than instanceof Enum followed by getDeclaringClass
			Dim keyClass As Class = key.GetType()
			Return keyClass Is keyType OrElse keyClass.BaseType Is keyType
		End Function

		' Bulk Operations

		''' <summary>
		''' Copies all of the mappings from the specified map to this map.
		''' These mappings will replace any mappings that this map had for
		''' any of the keys currently in the specified map.
		''' </summary>
		''' <param name="m"> the mappings to be stored in this map </param>
		''' <exception cref="NullPointerException"> the specified map is null, or if
		'''     one or more keys in the specified map are null </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Sub putAll(Of T1 As K, ? As V)(ByVal m As Map(Of T1))
			If TypeOf m Is EnumMap Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim em As EnumMap(Of ?, ?) = CType(m, EnumMap(Of ?, ?))
				If em.keyType IsNot keyType Then
					If em.empty Then Return
					Throw New ClassCastException(em.keyType & " != " & keyType)
				End If

				For i As Integer = 0 To keyUniverse.Length - 1
					Dim emValue As Object = em.vals(i)
					If emValue IsNot Nothing Then
						If vals(i) Is Nothing Then size_Renamed += 1
						vals(i) = emValue
					End If
				Next i
			Else
				MyBase.putAll(m)
			End If
		End Sub

		''' <summary>
		''' Removes all mappings from this map.
		''' </summary>
		Public Overridable Sub clear()
			Arrays.fill(vals, Nothing)
			size_Renamed = 0
		End Sub

		' Views

		''' <summary>
		''' This field is initialized to contain an instance of the entry set
		''' view the first time this view is requested.  The view is stateless,
		''' so there's no reason to create more than one.
		''' </summary>
		<NonSerialized> _
		Private entrySet_Renamed As [Set](Of KeyValuePair(Of K, V))

		''' <summary>
		''' Returns a <seealso cref="Set"/> view of the keys contained in this map.
		''' The returned set obeys the general contract outlined in
		''' <seealso cref="Map#keySet()"/>.  The set's iterator will return the keys
		''' in their natural order (the order in which the enum constants
		''' are declared).
		''' </summary>
		''' <returns> a set view of the keys contained in this enum map </returns>
		Public Overridable Function keySet() As [Set](Of K)
			Dim ks As [Set](Of K) = keySet
			If ks IsNot Nothing Then
				Return ks
			Else
					keySet = New KeySet(Me, Me)
					Return keySet
			End If
		End Function

		Private Class KeySet
			Inherits AbstractSet(Of K)

			Private ReadOnly outerInstance As EnumMap

			Public Sub New(ByVal outerInstance As EnumMap)
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
			Public Overridable Sub clear()
				outerInstance.clear()
			End Sub
		End Class

		''' <summary>
		''' Returns a <seealso cref="Collection"/> view of the values contained in this map.
		''' The returned collection obeys the general contract outlined in
		''' <seealso cref="Map#values()"/>.  The collection's iterator will return the
		''' values in the order their corresponding keys appear in map,
		''' which is their natural order (the order in which the enum constants
		''' are declared).
		''' </summary>
		''' <returns> a collection view of the values contained in this map </returns>
		Public Overridable Function values() As Collection(Of V)
			Dim vs As Collection(Of V) = values
			If vs IsNot Nothing Then
				Return vs
			Else
					values = New Values(Me, Me)
					Return values
			End If
		End Function

		Private Class Values
			Inherits AbstractCollection(Of V)

			Private ReadOnly outerInstance As EnumMap

			Public Sub New(ByVal outerInstance As EnumMap)
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
				o = outerInstance.maskNull(o)

				For i As Integer = 0 To outerInstance.vals.Length - 1
					If o.Equals(outerInstance.vals(i)) Then
						outerInstance.vals(i) = Nothing
						outerInstance.size_Renamed -= 1
						Return True
					End If
				Next i
				Return False
			End Function
			Public Overridable Sub clear()
				outerInstance.clear()
			End Sub
		End Class

		''' <summary>
		''' Returns a <seealso cref="Set"/> view of the mappings contained in this map.
		''' The returned set obeys the general contract outlined in
		''' <seealso cref="Map#keySet()"/>.  The set's iterator will return the
		''' mappings in the order their keys appear in map, which is their
		''' natural order (the order in which the enum constants are declared).
		''' </summary>
		''' <returns> a set view of the mappings contained in this enum map </returns>
		Public Overridable Function entrySet() As [Set](Of KeyValuePair(Of K, V))
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

			Private ReadOnly outerInstance As EnumMap

			Public Sub New(ByVal outerInstance As EnumMap)
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
			Public Overridable Function toArray() As Object()
				Return fillEntryArray(New Object(outerInstance.size_Renamed - 1){})
			End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Function toArray(Of T)(ByVal a As T()) As T()
				Dim size As Integer = size()
				If a.Length < size Then a = CType(java.lang.reflect.Array.newInstance(a.GetType().GetElementType(), size), T())
				If a.Length > size Then a(size) = Nothing
				Return CType(fillEntryArray(a), T())
			End Function
			Private Function fillEntryArray(ByVal a As Object()) As Object()
				Dim j As Integer = 0
				For i As Integer = 0 To outerInstance.vals.Length - 1
					If outerInstance.vals(i) IsNot Nothing Then a(j) = New AbstractMap.SimpleEntry(Of )(outerInstance.keyUniverse(i), outerInstance.unmaskNull(outerInstance.vals(i)))
				Next i
						j += 1
				Return a
			End Function
		End Class

		Private MustInherit Class EnumMapIterator(Of T)
			Implements Iterator(Of T)

			Private ReadOnly outerInstance As EnumMap

			Public Sub New(ByVal outerInstance As EnumMap)
				Me.outerInstance = outerInstance
			End Sub

			' Lower bound on index of next element to return
			Friend index As Integer = 0

			' Index of last returned element, or -1 if none
			Friend lastReturnedIndex As Integer = -1

			Public Overridable Function hasNext() As Boolean Implements Iterator(Of T).hasNext
				Do While index < outerInstance.vals.Length AndAlso outerInstance.vals(index) Is Nothing
					index += 1
				Loop
				Return index <> outerInstance.vals.Length
			End Function

			Public Overridable Sub remove() Implements Iterator(Of T).remove
				checkLastReturnedIndex()

				If outerInstance.vals(lastReturnedIndex) IsNot Nothing Then
					outerInstance.vals(lastReturnedIndex) = Nothing
					outerInstance.size_Renamed -= 1
				End If
				lastReturnedIndex = -1
			End Sub

			Private Sub checkLastReturnedIndex()
				If lastReturnedIndex < 0 Then Throw New IllegalStateException
			End Sub
		End Class

		Private Class KeyIterator
			Inherits EnumMapIterator(Of K)

			Private ReadOnly outerInstance As EnumMap

			Public Sub New(ByVal outerInstance As EnumMap)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Function [next]() As K
				If Not hasNext() Then Throw New NoSuchElementException
				lastReturnedIndex = index
				index += 1
				Return outerInstance.keyUniverse(lastReturnedIndex)
			End Function
		End Class

		Private Class ValueIterator
			Inherits EnumMapIterator(Of V)

			Private ReadOnly outerInstance As EnumMap

			Public Sub New(ByVal outerInstance As EnumMap)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Function [next]() As V
				If Not hasNext() Then Throw New NoSuchElementException
				lastReturnedIndex = index
				index += 1
				Return outerInstance.unmaskNull(outerInstance.vals(lastReturnedIndex))
			End Function
		End Class

		Private Class EntryIterator
			Inherits EnumMapIterator(Of KeyValuePair(Of K, V))

			Private ReadOnly outerInstance As EnumMap

			Public Sub New(ByVal outerInstance As EnumMap)
				Me.outerInstance = outerInstance
			End Sub

			Private lastReturnedEntry As DictionaryEntry

			Public Overridable Function [next]() As KeyValuePair(Of K, V)
				If Not hasNext() Then Throw New NoSuchElementException
				lastReturnedEntry = New DictionaryEntry(index)
				index += 1
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

				Private ReadOnly outerInstance As EnumMap.EntryIterator

				Private index As Integer

				Function IDictionary.Entry(ByVal index As Integer) As [Private]
					Me.index = index
				End Function

				Public Overridable Property key As K
					Get
						checkIndexForEntryUse()
						Return keyUniverse(index)
					End Get
				End Property

				Public Overridable Property value As V
					Get
						checkIndexForEntryUse()
						Return unmaskNull(vals(index))
					End Get
				End Property

				Public Overridable Function setValue(ByVal value As V) As V
					checkIndexForEntryUse()
					Dim oldValue As V = unmaskNull(vals(index))
					vals(index) = maskNull(value)
					Return oldValue
				End Function

				Public Overrides Function Equals(ByVal o As Object) As Boolean
					If index < 0 Then Return o Is Me

					If Not(TypeOf o Is DictionaryEntry) Then Return False

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim e As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
					Dim ourValue As V = unmaskNull(vals(index))
					Dim hisValue As Object = e.Value
					Return (e.Key = keyUniverse(index) AndAlso (ourValue Is hisValue OrElse (ourValue IsNot Nothing AndAlso ourValue.Equals(hisValue))))
				End Function

				Public Overrides Function GetHashCode() As Integer
					If index < 0 Then Return MyBase.GetHashCode()

					Return entryHashCode(index)
				End Function

				Public Overrides Function ToString() As String
					If index < 0 Then Return MyBase.ToString()

					Return keyUniverse(index) & "=" & unmaskNull(vals(index))
				End Function

				Private Sub checkIndexForEntryUse()
					If index < 0 Then Throw New IllegalStateException("Entry was removed")
				End Sub
			End Class
		End Class

		' Comparison and hashing

		''' <summary>
		''' Compares the specified object with this map for equality.  Returns
		''' <tt>true</tt> if the given object is also a map and the two maps
		''' represent the same mappings, as specified in the {@link
		''' Map#equals(Object)} contract.
		''' </summary>
		''' <param name="o"> the object to be compared for equality with this map </param>
		''' <returns> <tt>true</tt> if the specified object is equal to this map </returns>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then Return True
			If TypeOf o Is EnumMap Then Return Equals(CType(o, EnumMap(Of ?, ?)))
			If Not(TypeOf o Is Map) Then Return False

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim m As Map(Of ?, ?) = CType(o, Map(Of ?, ?))
			If size_Renamed <> m.size() Then Return False

			For i As Integer = 0 To keyUniverse.Length - 1
				If Nothing IsNot vals(i) Then
					Dim key As K = keyUniverse(i)
					Dim value As V = unmaskNull(vals(i))
					If Nothing Is value Then
						If Not((Nothing Is m.get(key)) AndAlso m.containsKey(key)) Then Return False
					Else
					   If Not value.Equals(m.get(key)) Then Return False
					End If
				End If
			Next i

			Return True
		End Function

		Private Overrides Function Equals(Of T1)(ByVal em As EnumMap(Of T1)) As Boolean
			If em.keyType IsNot keyType Then Return size_Renamed = 0 AndAlso em.size_Renamed = 0

			' Key types match, compare each value
			For i As Integer = 0 To keyUniverse.Length - 1
				Dim ourValue As Object = vals(i)
				Dim hisValue As Object = em.vals(i)
				If hisValue IsNot ourValue AndAlso (hisValue Is Nothing OrElse (Not hisValue.Equals(ourValue))) Then Return False
			Next i
			Return True
		End Function

		''' <summary>
		''' Returns the hash code value for this map.  The hash code of a map is
		''' defined to be the sum of the hash codes of each entry in the map.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Dim h As Integer = 0

			For i As Integer = 0 To keyUniverse.Length - 1
				If Nothing IsNot vals(i) Then h += entryHashCode(i)
			Next i

			Return h
		End Function

		Private Function entryHashCode(ByVal index As Integer) As Integer
			Return (keyUniverse(index).GetHashCode() Xor vals(index).GetHashCode())
		End Function

		''' <summary>
		''' Returns a shallow copy of this enum map.  (The values themselves
		''' are not cloned.
		''' </summary>
		''' <returns> a shallow copy of this enum map </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function clone() As EnumMap(Of K, V)
			Dim result As EnumMap(Of K, V) = Nothing
			Try
				result = CType(MyBase.clone(), EnumMap(Of K, V))
			Catch e As CloneNotSupportedException
				Throw New AssertionError
			End Try
			result.vals = result.vals.clone()
			result.entrySet_Renamed = Nothing
			Return result
		End Function

		''' <summary>
		''' Throws an exception if e is not of the correct type for this enum set.
		''' </summary>
		Private Sub typeCheck(ByVal key As K)
			Dim keyClass As Class = key.GetType()
			If keyClass IsNot keyType AndAlso keyClass.BaseType IsNot keyType Then Throw New ClassCastException(keyClass & " != " & keyType)
		End Sub

		''' <summary>
		''' Returns all of the values comprising K.
		''' The result is uncloned, cached, and shared by all callers.
		''' </summary>
		Private Shared Function getKeyUniverse(Of K As System.Enum(Of K))(ByVal keyType As Class) As K()
			Return sun.misc.SharedSecrets.javaLangAccess.getEnumConstantsShared(keyType)
		End Function

		Private Const serialVersionUID As Long = 458661240069192865L

		''' <summary>
		''' Save the state of the <tt>EnumMap</tt> instance to a stream (i.e.,
		''' serialize it).
		''' 
		''' @serialData The <i>size</i> of the enum map (the number of key-value
		'''             mappings) is emitted (int), followed by the key (Object)
		'''             and value (Object) for each key-value mapping represented
		'''             by the enum map.
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			' Write out the key type and any hidden stuff
			s.defaultWriteObject()

			' Write out size (number of Mappings)
			s.writeInt(size_Renamed)

			' Write out keys and values (alternating)
			Dim entriesToBeWritten As Integer = size_Renamed
			Dim i As Integer = 0
			Do While entriesToBeWritten > 0
				If Nothing IsNot vals(i) Then
					s.writeObject(keyUniverse(i))
					s.writeObject(unmaskNull(vals(i)))
					entriesToBeWritten -= 1
				End If
				i += 1
			Loop
		End Sub

		''' <summary>
		''' Reconstitute the <tt>EnumMap</tt> instance from a stream (i.e.,
		''' deserialize it).
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			' Read in the key type and any hidden stuff
			s.defaultReadObject()

			keyUniverse = getKeyUniverse(keyType)
			vals = New Object(keyUniverse.Length - 1){}

			' Read in size (number of Mappings)
			Dim size As Integer = s.readInt()

			' Read the keys and values, and put the mappings in the HashMap
			For i As Integer = 0 To size - 1
				Dim key As K = CType(s.readObject(), K)
				Dim value As V = CType(s.readObject(), V)
				put(key, value)
			Next i
		End Sub
	End Class

End Namespace
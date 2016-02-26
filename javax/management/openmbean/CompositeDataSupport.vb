Imports System
Imports System.Collections.Generic
Imports System.Text

'
' * Copyright (c) 2000, 2008, Oracle and/or its affiliates. All rights reserved.
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


Namespace javax.management.openmbean


	' java import
	'

	' jmx import
	'


	''' <summary>
	''' The <tt>CompositeDataSupport</tt> class is the <i>open data</i> class which
	''' implements the <tt>CompositeData</tt> interface.
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	<Serializable> _
	Public Class CompositeDataSupport
		Implements CompositeData

		' Serial version 
		Friend Const serialVersionUID As Long = 8003518976613702244L

		''' <summary>
		''' @serial Internal representation of the mapping of item names to their
		''' respective values.
		'''         A <seealso cref="SortedMap"/> is used for faster retrieval of elements.
		''' </summary>
		Private ReadOnly contents As java.util.SortedMap(Of String, Object)

		''' <summary>
		''' @serial The <i>composite type </i> of this <i>composite data</i> instance.
		''' </summary>
		Private ReadOnly compositeType As CompositeType

		''' <summary>
		''' <p>Constructs a <tt>CompositeDataSupport</tt> instance with the specified
		''' <tt>compositeType</tt>, whose item values
		''' are specified by <tt>itemValues[]</tt>, in the same order as in
		''' <tt>itemNames[]</tt>.
		''' As a <tt>CompositeType</tt> does not specify any order on its items,
		''' the <tt>itemNames[]</tt> parameter is used
		''' to specify the order in which the values are given in <tt>itemValues[]</tt>.
		''' The items contained in this <tt>CompositeDataSupport</tt> instance are
		''' internally stored in a <tt>TreeMap</tt>,
		''' thus sorted in ascending lexicographic order of their names, for faster
		''' retrieval of individual item values.</p>
		''' 
		''' <p>The constructor checks that all the constraints listed below for each
		''' parameter are satisfied,
		''' and throws the appropriate exception if they are not.</p>
		''' </summary>
		''' <param name="compositeType"> the <i>composite type </i> of this <i>composite
		''' data</i> instance; must not be null.
		''' </param>
		''' <param name="itemNames"> <tt>itemNames</tt> must list, in any order, all the
		''' item names defined in <tt>compositeType</tt>; the order in which the
		''' names are listed, is used to match values in <tt>itemValues[]</tt>; must
		''' not be null or empty.
		''' </param>
		''' <param name="itemValues"> the values of the items, listed in the same order as
		''' their respective names in <tt>itemNames</tt>; each item value can be
		''' null, but if it is non-null it must be a valid value for the open type
		''' defined in <tt>compositeType</tt> for the corresponding item; must be of
		''' the same size as <tt>itemNames</tt>; must not be null or empty.
		''' </param>
		''' <exception cref="IllegalArgumentException"> <tt>compositeType</tt> is null, or
		''' <tt>itemNames[]</tt> or <tt>itemValues[]</tt> is null or empty, or one
		''' of the elements in <tt>itemNames[]</tt> is a null or empty string, or
		''' <tt>itemNames[]</tt> and <tt>itemValues[]</tt> are not of the same size.
		''' </exception>
		''' <exception cref="OpenDataException"> <tt>itemNames[]</tt> or
		''' <tt>itemValues[]</tt>'s size differs from the number of items defined in
		''' <tt>compositeType</tt>, or one of the elements in <tt>itemNames[]</tt>
		''' does not exist as an item name defined in <tt>compositeType</tt>, or one
		''' of the elements in <tt>itemValues[]</tt> is not a valid value for the
		''' corresponding item as defined in <tt>compositeType</tt>. </exception>
		Public Sub New(ByVal ___compositeType As CompositeType, ByVal itemNames As String(), ByVal itemValues As Object())
			Me.New(makeMap(itemNames, itemValues), ___compositeType)
		End Sub

		Private Shared Function makeMap(ByVal itemNames As String(), ByVal itemValues As Object()) As java.util.SortedMap(Of String, Object)

			If itemNames Is Nothing OrElse itemValues Is Nothing Then Throw New System.ArgumentException("Null itemNames or itemValues")
			If itemNames.Length = 0 OrElse itemValues.Length = 0 Then Throw New System.ArgumentException("Empty itemNames or itemValues")
			If itemNames.Length <> itemValues.Length Then Throw New System.ArgumentException("Different lengths: itemNames[" & itemNames.Length & "], itemValues[" & itemValues.Length & "]")

			Dim map As java.util.SortedMap(Of String, Object) = New SortedDictionary(Of String, Object)
			For i As Integer = 0 To itemNames.Length - 1
				Dim name As String = itemNames(i)
				If name Is Nothing OrElse name.Equals("") Then Throw New System.ArgumentException("Null or empty item name")
				If map.containsKey(name) Then Throw New OpenDataException("Duplicate item name " & name)
				map.put(itemNames(i), itemValues(i))
			Next i

			Return map
		End Function

		''' <summary>
		''' <p>
		''' Constructs a <tt>CompositeDataSupport</tt> instance with the specified <tt>compositeType</tt>, whose item names and corresponding values
		''' are given by the mappings in the map <tt>items</tt>.
		''' This constructor converts the keys to a string array and the values to an object array and calls
		''' <tt>CompositeDataSupport(javax.management.openmbean.CompositeType, java.lang.String[], java.lang.Object[])</tt>.
		''' </summary>
		''' <param name="compositeType">  the <i>composite type </i> of this <i>composite data</i> instance;
		'''                        must not be null. </param>
		''' <param name="items">  the mappings of all the item names to their values;
		'''                <tt>items</tt> must contain all the item names defined in <tt>compositeType</tt>;
		'''                must not be null or empty.
		''' </param>
		''' <exception cref="IllegalArgumentException"> <tt>compositeType</tt> is null, or
		''' <tt>items</tt> is null or empty, or one of the keys in <tt>items</tt> is a null
		''' or empty string. </exception>
		''' <exception cref="OpenDataException"> <tt>items</tt>' size differs from the
		''' number of items defined in <tt>compositeType</tt>, or one of the
		''' keys in <tt>items</tt> does not exist as an item name defined in
		''' <tt>compositeType</tt>, or one of the values in <tt>items</tt>
		''' is not a valid value for the corresponding item as defined in
		''' <tt>compositeType</tt>. </exception>
		''' <exception cref="ArrayStoreException"> one or more keys in <tt>items</tt> is not of
		''' the class <tt>java.lang.String</tt>. </exception>
		Public Sub New(Of T1)(ByVal ___compositeType As CompositeType, ByVal items As IDictionary(Of T1))
			Me.New(makeMap(items), ___compositeType)
		End Sub

		Private Shared Function makeMap(Of T1)(ByVal items As IDictionary(Of T1)) As java.util.SortedMap(Of String, Object)
			If items Is Nothing OrElse items.Count = 0 Then Throw New System.ArgumentException("Null or empty items map")

			Dim map As java.util.SortedMap(Of String, Object) = New SortedDictionary(Of String, Object)
			For Each key As Object In items.Keys
				If key Is Nothing OrElse key.Equals("") Then Throw New System.ArgumentException("Null or empty item name")
				If Not(TypeOf key Is String) Then Throw New ArrayStoreException("Item name is not string: " & key)
				map.put(CStr(key), items(key))
			Next key
			Return map
		End Function

		Private Sub New(ByVal items As java.util.SortedMap(Of String, Object), ByVal ___compositeType As CompositeType)

			' Check compositeType is not null
			'
			If ___compositeType Is Nothing Then Throw New System.ArgumentException("Argument compositeType cannot be null.")

			' item names defined in compositeType:
			Dim namesFromType As javax.management.openmbean.CompositeType.KeyCollection = ___compositeType.Keys
			Dim namesFromItems As java.util.SortedMap(Of String, Object).KeyCollection = items.Keys

			' This is just a comparison, but we do it this way for a better
			' exception message.
			If Not namesFromType.Equals(namesFromItems) Then
				Dim extraFromType As java.util.Set(Of String) = New SortedSet(Of String)(namesFromType)
				extraFromType.removeAll(namesFromItems)
				Dim extraFromItems As java.util.Set(Of String) = New SortedSet(Of String)(namesFromItems)
				extraFromItems.removeAll(namesFromType)
				If (Not extraFromType.empty) OrElse (Not extraFromItems.empty) Then Throw New OpenDataException("Item names do not match CompositeType: " & "names in items but not in CompositeType: " & extraFromItems & "; names in CompositeType but not in items: " & extraFromType)
			End If

			' Check each value, if not null, is of the open type defined for the
			' corresponding item
			For Each name As String In namesFromType
				Dim value As Object = items.get(name)
				If value IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim itemType As OpenType(Of ?) = ___compositeType.getType(name)
					If Not itemType.isValue(value) Then Throw New OpenDataException("Argument value of wrong type for item " & name & ": value " & value & ", type " & itemType)
				End If
			Next name

			' Initialize internal fields: compositeType and contents
			'
			Me.compositeType = ___compositeType
			Me.contents = items
		End Sub

		''' <summary>
		''' Returns the <i>composite type </i> of this <i>composite data</i> instance.
		''' </summary>
		Public Overridable Property compositeType As CompositeType Implements CompositeData.getCompositeType
			Get
    
				Return compositeType
			End Get
		End Property

		''' <summary>
		''' Returns the value of the item whose name is <tt>key</tt>.
		''' </summary>
		''' <exception cref="IllegalArgumentException">  if <tt>key</tt> is a null or empty String.
		''' </exception>
		''' <exception cref="InvalidKeyException">  if <tt>key</tt> is not an existing item name for
		''' this <tt>CompositeData</tt> instance. </exception>
		Public Overridable Function [get](ByVal key As String) As Object Implements CompositeData.get

			If (key Is Nothing) OrElse (key.Trim().Equals("")) Then Throw New System.ArgumentException("Argument key cannot be a null or empty String.")
			If Not contents.containsKey(key.Trim()) Then Throw New InvalidKeyException("Argument key=""" & key.Trim() & """ is not an existing item name for this CompositeData instance.")
			Return contents.get(key.Trim())
		End Function

		''' <summary>
		''' Returns an array of the values of the items whose names are specified by
		''' <tt>keys</tt>, in the same order as <tt>keys</tt>.
		''' </summary>
		''' <exception cref="IllegalArgumentException">  if an element in <tt>keys</tt> is a null
		''' or empty String.
		''' </exception>
		''' <exception cref="InvalidKeyException">  if an element in <tt>keys</tt> is not an existing
		''' item name for this <tt>CompositeData</tt> instance. </exception>
		Public Overridable Function getAll(ByVal keys As String()) As Object() Implements CompositeData.getAll

			If (keys Is Nothing) OrElse (keys.Length = 0) Then Return New Object(){}
			Dim results As Object() = New Object(keys.Length - 1){}
			For i As Integer = 0 To keys.Length - 1
				results(i) = Me.get(keys(i))
			Next i
			Return results
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if and only if this <tt>CompositeData</tt> instance contains
		''' an item whose name is <tt>key</tt>.
		''' If <tt>key</tt> is a null or empty String, this method simply returns false.
		''' </summary>
		Public Overridable Function containsKey(ByVal key As String) As Boolean Implements CompositeData.containsKey

			If (key Is Nothing) OrElse (key.Trim().Equals("")) Then Return False
			Return contents.containsKey(key)
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if and only if this <tt>CompositeData</tt> instance
		''' contains an item
		''' whose value is <tt>value</tt>.
		''' </summary>
		Public Overridable Function containsValue(ByVal value As Object) As Boolean Implements CompositeData.containsValue

			Return contents.containsValue(value)
		End Function

		''' <summary>
		''' Returns an unmodifiable Collection view of the item values contained in this
		''' <tt>CompositeData</tt> instance.
		''' The returned collection's iterator will return the values in the ascending
		''' lexicographic order of the corresponding
		''' item names.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Function values() As ICollection(Of ?) Implements CompositeData.values

			Return java.util.Collections.unmodifiableCollection(contents.values())
		End Function

		''' <summary>
		''' Compares the specified <var>obj</var> parameter with this
		''' <code>CompositeDataSupport</code> instance for equality.
		''' <p>
		''' Returns <tt>true</tt> if and only if all of the following statements are true:
		''' <ul>
		''' <li><var>obj</var> is non null,</li>
		''' <li><var>obj</var> also implements the <code>CompositeData</code> interface,</li>
		''' <li>their composite types are equal</li>
		''' <li>their contents, i.e. (name, value) pairs are equal. If a value contained in
		''' the content is an array, the value comparison is done as if by calling
		''' the <seealso cref="java.util.Arrays#deepEquals(Object[], Object[]) deepEquals"/> method
		''' for arrays of object reference types or the appropriate overloading of
		''' {@code Arrays.equals(e1,e2)} for arrays of primitive types</li>
		''' </ul>
		''' <p>
		''' This ensures that this <tt>equals</tt> method works properly for
		''' <var>obj</var> parameters which are different implementations of the
		''' <code>CompositeData</code> interface, with the restrictions mentioned in the
		''' <seealso cref="java.util.Collection#equals(Object) equals"/>
		''' method of the <tt>java.util.Collection</tt> interface.
		''' </summary>
		''' <param name="obj">  the object to be compared for equality with this
		''' <code>CompositeDataSupport</code> instance. </param>
		''' <returns>  <code>true</code> if the specified object is equal to this
		''' <code>CompositeDataSupport</code> instance. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True

			' if obj is not a CompositeData, return false
			If Not(TypeOf obj Is CompositeData) Then Return False

			Dim other As CompositeData = CType(obj, CompositeData)

			' their compositeType should be equal
			If Not Me.compositeType.Equals(other.compositeType) Then Return False

			If contents.size() <> other.values().Count Then Return False

			For Each entry As KeyValuePair(Of String, Object) In contents.entrySet()
				Dim e1 As Object = entry.Value
				Dim e2 As Object = other.get(entry.Key)

				If e1 Is e2 Then Continue For
				If e1 Is Nothing Then Return False

				Dim eq As Boolean = If(e1.GetType().IsArray, java.util.Arrays.deepEquals(New Object() {e1}, New Object() {e2}), e1.Equals(e2))

				If Not eq Then Return False
			Next entry

			' All tests for equality were successful
			'
			Return True
		End Function

		''' <summary>
		''' Returns the hash code value for this <code>CompositeDataSupport</code> instance.
		''' <p>
		''' The hash code of a <code>CompositeDataSupport</code> instance is the sum of the hash codes
		''' of all elements of information used in <code>equals</code> comparisons
		''' (ie: its <i>composite type</i> and all the item values).
		''' <p>
		''' This ensures that <code> t1.equals(t2) </code> implies that <code> t1.hashCode()==t2.hashCode() </code>
		''' for any two <code>CompositeDataSupport</code> instances <code>t1</code> and <code>t2</code>,
		''' as required by the general contract of the method
		''' <seealso cref="Object#hashCode() Object.hashCode()"/>.
		''' <p>
		''' Each item value's hash code is added to the returned hash code.
		''' If an item value is an array,
		''' its hash code is obtained as if by calling the
		''' <seealso cref="java.util.Arrays#deepHashCode(Object[]) deepHashCode"/> method
		''' for arrays of object reference types or the appropriate overloading
		''' of {@code Arrays.hashCode(e)} for arrays of primitive types.
		''' </summary>
		''' <returns> the hash code value for this <code>CompositeDataSupport</code> instance </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim ___hashcode As Integer = compositeType.GetHashCode()

			For Each o As Object In contents.values()
				If TypeOf o Is Object() Then
					___hashcode += java.util.Arrays.deepHashCode(CType(o, Object()))
				ElseIf TypeOf o Is SByte() Then
					___hashcode += java.util.Arrays.hashCode(CType(o, SByte()))
				ElseIf TypeOf o Is Short() Then
					___hashcode += java.util.Arrays.hashCode(CType(o, Short()))
				ElseIf TypeOf o Is Integer() Then
					___hashcode += java.util.Arrays.hashCode(CType(o, Integer()))
				ElseIf TypeOf o Is Long() Then
					___hashcode += java.util.Arrays.hashCode(CType(o, Long()))
				ElseIf TypeOf o Is Char() Then
					___hashcode += java.util.Arrays.hashCode(CType(o, Char()))
				ElseIf TypeOf o Is Single() Then
					___hashcode += java.util.Arrays.hashCode(CType(o, Single()))
				ElseIf TypeOf o Is Double() Then
					___hashcode += java.util.Arrays.hashCode(CType(o, Double()))
				ElseIf TypeOf o Is Boolean() Then
					___hashcode += java.util.Arrays.hashCode(CType(o, Boolean()))
				ElseIf o IsNot Nothing Then
					___hashcode += o.GetHashCode()
				End If
			Next o

			Return ___hashcode
		End Function

		''' <summary>
		''' Returns a string representation of this <code>CompositeDataSupport</code> instance.
		''' <p>
		''' The string representation consists of the name of this class (ie <code>javax.management.openmbean.CompositeDataSupport</code>),
		''' the string representation of the composite type of this instance, and the string representation of the contents
		''' (ie list the itemName=itemValue mappings).
		''' </summary>
		''' <returns>  a string representation of this <code>CompositeDataSupport</code> instance </returns>
		Public Overrides Function ToString() As String
			Return (New StringBuilder).Append(Me.GetType().name).append("(compositeType=").append(compositeType.ToString()).append(",contents=").append(contentString()).append(")").ToString()
		End Function

		Private Function contentString() As String
			Dim sb As New StringBuilder("{")
			Dim sep As String = ""
			For Each entry As KeyValuePair(Of String, Object) In contents.entrySet()
				sb.Append(sep).append(entry.Key).append("=")
				Dim s As String = java.util.Arrays.deepToString(New Object() {entry.Value})
				sb.Append(s.Substring(1, s.Length - 1 - 1))
				sep = ", "
			Next entry
			sb.Append("}")
			Return sb.ToString()
		End Function
	End Class

End Namespace
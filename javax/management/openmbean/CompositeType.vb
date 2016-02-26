Imports System
Imports System.Collections.Generic
Imports System.Text

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>CompositeType</code> class is the <i>open type</i> class
	''' whose instances describe the types of <seealso cref="CompositeData CompositeData"/> values.
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Class CompositeType
		Inherits OpenType(Of CompositeData)

		' Serial version 
		Friend Const serialVersionUID As Long = -5366242454346948798L

		''' <summary>
		''' @serial Sorted mapping of the item names to their descriptions
		''' </summary>
		Private nameToDescription As SortedDictionary(Of String, String)

		''' <summary>
		''' @serial Sorted mapping of the item names to their open types
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private nameToType As SortedDictionary(Of String, OpenType(Of ?))

	'     As this instance is immutable, following three values need only
	'     * be calculated once.  
		<NonSerialized> _
		Private myHashCode As Integer? = Nothing
		<NonSerialized> _
		Private myToString As String = Nothing
		<NonSerialized> _
		Private myNamesSet As java.util.Set(Of String) = Nothing


		' *** Constructor *** 

		''' <summary>
		''' Constructs a <code>CompositeType</code> instance, checking for the validity of the given parameters.
		''' The validity constraints are described below for each parameter.
		''' <p>
		''' Note that the contents of the three array parameters
		''' <var>itemNames</var>, <var>itemDescriptions</var> and <var>itemTypes</var>
		''' are internally copied so that any subsequent modification of these arrays by the caller of this constructor
		''' has no impact on the constructed <code>CompositeType</code> instance.
		''' <p>
		''' The Java class name of composite data values this composite type represents
		''' (ie the class name returned by the <seealso cref="OpenType#getClassName() getClassName"/> method)
		''' is set to the string value returned by <code>CompositeData.class.getName()</code>.
		''' <p> </summary>
		''' <param name="typeName">  The name given to the composite type this instance represents; cannot be a null or empty string.
		''' <br>&nbsp; </param>
		''' <param name="description">  The human readable description of the composite type this instance represents;
		'''                      cannot be a null or empty string.
		''' <br>&nbsp; </param>
		''' <param name="itemNames">  The names of the items contained in the
		'''                    composite data values described by this <code>CompositeType</code> instance;
		'''                    cannot be null and should contain at least one element; no element can be a null or empty string.
		'''                    Note that the order in which the item names are given is not important to differentiate a
		'''                    <code>CompositeType</code> instance from another;
		'''                    the item names are internally stored sorted in ascending alphanumeric order.
		''' <br>&nbsp; </param>
		''' <param name="itemDescriptions">  The descriptions, in the same order as <var>itemNames</var>, of the items contained in the
		'''                           composite data values described by this <code>CompositeType</code> instance;
		'''                           should be of the same size as <var>itemNames</var>;
		'''                           no element can be null or an empty string.
		''' <br>&nbsp; </param>
		''' <param name="itemTypes">  The open type instances, in the same order as <var>itemNames</var>, describing the items contained
		'''                    in the composite data values described by this <code>CompositeType</code> instance;
		'''                    should be of the same size as <var>itemNames</var>;
		'''                    no element can be null.
		''' <br>&nbsp; </param>
		''' <exception cref="IllegalArgumentException">  If <var>typeName</var> or <var>description</var> is a null or empty string,
		'''                                   or <var>itemNames</var> or <var>itemDescriptions</var> or <var>itemTypes</var> is null,
		'''                                   or any element of <var>itemNames</var> or <var>itemDescriptions</var>
		'''                                   is a null or empty string,
		'''                                   or any element of <var>itemTypes</var> is null,
		'''                                   or <var>itemNames</var> or <var>itemDescriptions</var> or <var>itemTypes</var>
		'''                                   are not of the same size.
		''' <br>&nbsp; </exception>
		''' <exception cref="OpenDataException">  If <var>itemNames</var> contains duplicate item names
		'''                            (case sensitive, but leading and trailing whitespaces removed). </exception>
		Public Sub New(Of T1)(ByVal typeName As String, ByVal description As String, ByVal itemNames As String(), ByVal itemDescriptions As String(), ByVal itemTypes As OpenType(Of T1)())

			' Check and construct state defined by parent
			'
			MyBase.New(GetType(CompositeData).name, typeName, description, False)

			' Check the 3 arrays are not null or empty (ie length==0) and that there is no null element or empty string in them
			'
			checkForNullElement(itemNames, "itemNames")
			checkForNullElement(itemDescriptions, "itemDescriptions")
			checkForNullElement(itemTypes, "itemTypes")
			checkForEmptyString(itemNames, "itemNames")
			checkForEmptyString(itemDescriptions, "itemDescriptions")

			' Check the sizes of the 3 arrays are the same
			'
			If (itemNames.Length <> itemDescriptions.Length) OrElse (itemNames.Length <> itemTypes.Length) Then Throw New System.ArgumentException("Array arguments itemNames[], itemDescriptions[] and itemTypes[] " & "should be of same length (got " & itemNames.Length & ", " & itemDescriptions.Length & " and " & itemTypes.Length & ").")

			' Initialize internal "names to descriptions" and "names to types" sorted maps,
			' and, by doing so, check there are no duplicate item names
			'
			nameToDescription = New SortedDictionary(Of String, String)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			nameToType = New SortedDictionary(Of String, OpenType(Of ?))
			Dim key As String
			For i As Integer = 0 To itemNames.Length - 1
				key = itemNames(i).Trim()
				If nameToDescription.ContainsKey(key) Then Throw New OpenDataException("Argument's element itemNames[" & i & "]=""" & itemNames(i) & """ duplicates a previous item names.")
				nameToDescription(key) = itemDescriptions(i).Trim()
				nameToType(key) = itemTypes(i)
			Next i
		End Sub

		Private Shared Sub checkForNullElement(ByVal arg As Object(), ByVal argName As String)
			If (arg Is Nothing) OrElse (arg.Length = 0) Then Throw New System.ArgumentException("Argument " & argName & "[] cannot be null or empty.")
			For i As Integer = 0 To arg.Length - 1
				If arg(i) Is Nothing Then Throw New System.ArgumentException("Argument's element " & argName & "[" & i & "] cannot be null.")
			Next i
		End Sub

		Private Shared Sub checkForEmptyString(ByVal arg As String(), ByVal argName As String)
			For i As Integer = 0 To arg.Length - 1
				If arg(i).Trim().Equals("") Then Throw New System.ArgumentException("Argument's element " & argName & "[" & i & "] cannot be an empty string.")
			Next i
		End Sub

		' *** Composite type specific information methods *** 

		''' <summary>
		''' Returns <code>true</code> if this <code>CompositeType</code> instance defines an item
		''' whose name is <var>itemName</var>.
		''' </summary>
		''' <param name="itemName"> the name of the item.
		''' </param>
		''' <returns> true if an item of this name is present. </returns>
		Public Overridable Function containsKey(ByVal itemName As String) As Boolean

			If itemName Is Nothing Then Return False
			Return nameToDescription.ContainsKey(itemName)
		End Function

		''' <summary>
		''' Returns the description of the item whose name is <var>itemName</var>,
		''' or <code>null</code> if this <code>CompositeType</code> instance does not define any item
		''' whose name is <var>itemName</var>.
		''' </summary>
		''' <param name="itemName"> the name of the item.
		''' </param>
		''' <returns> the description. </returns>
		Public Overridable Function getDescription(ByVal itemName As String) As String

			If itemName Is Nothing Then Return Nothing
			Return nameToDescription(itemName)
		End Function

		''' <summary>
		''' Returns the <i>open type</i> of the item whose name is <var>itemName</var>,
		''' or <code>null</code> if this <code>CompositeType</code> instance does not define any item
		''' whose name is <var>itemName</var>.
		''' </summary>
		''' <param name="itemName"> the name of the time.
		''' </param>
		''' <returns> the type. </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Function [getType](ByVal itemName As String) As OpenType(Of ?)

			If itemName Is Nothing Then Return Nothing
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Return CType(nameToType(itemName), OpenType(Of ?))
		End Function

		''' <summary>
		''' Returns an unmodifiable Set view of all the item names defined by this <code>CompositeType</code> instance.
		''' The set's iterator will return the item names in ascending order.
		''' </summary>
		''' <returns> a <seealso cref="Set"/> of <seealso cref="String"/>. </returns>
		Public Overridable Function keySet() As java.util.Set(Of String)

			' Initializes myNamesSet on first call
			If myNamesSet Is Nothing Then myNamesSet = java.util.Collections.unmodifiableSet(nameToDescription.Keys)

			Return myNamesSet ' always return the same value
		End Function


		''' <summary>
		''' Tests whether <var>obj</var> is a value which could be
		''' described by this <code>CompositeType</code> instance.
		''' 
		''' <p>If <var>obj</var> is null or is not an instance of
		''' <code>javax.management.openmbean.CompositeData</code>,
		''' <code>isValue</code> returns <code>false</code>.</p>
		''' 
		''' <p>If <var>obj</var> is an instance of
		''' <code>javax.management.openmbean.CompositeData</code>, then let
		''' {@code ct} be its {@code CompositeType} as returned by {@link
		''' CompositeData#getCompositeType()}.  The result is true if
		''' {@code this} is <em>assignable from</em> {@code ct}.  This
		''' means that:</p>
		''' 
		''' <ul>
		''' <li><seealso cref="#getTypeName() this.getTypeName()"/> equals
		''' {@code ct.getTypeName()}, and
		''' <li>there are no item names present in {@code this} that are
		''' not also present in {@code ct}, and
		''' <li>for every item in {@code this}, its type is assignable from
		''' the type of the corresponding item in {@code ct}.
		''' </ul>
		''' 
		''' <p>A {@code TabularType} is assignable from another {@code
		''' TabularType} if they have the same {@linkplain
		''' TabularType#getTypeName() typeName} and {@linkplain
		''' TabularType#getIndexNames() index name list}, and the
		''' <seealso cref="TabularType#getRowType() row type"/> of the first is
		''' assignable from the row type of the second.
		''' 
		''' <p>An {@code ArrayType} is assignable from another {@code
		''' ArrayType} if they have the same {@linkplain
		''' ArrayType#getDimension() dimension}; and both are {@linkplain
		''' ArrayType#isPrimitiveArray() primitive arrays} or neither is;
		''' and the {@link ArrayType#getElementOpenType() element
		''' type} of the first is assignable from the element type of the
		''' second.
		''' 
		''' <p>In every other case, an {@code OpenType} is assignable from
		''' another {@code OpenType} only if they are equal.</p>
		''' 
		''' <p>These rules mean that extra items can be added to a {@code
		''' CompositeData} without making it invalid for a {@code CompositeType}
		''' that does not have those items.</p>
		''' </summary>
		''' <param name="obj">  the value whose open type is to be tested for compatibility
		''' with this <code>CompositeType</code> instance.
		''' </param>
		''' <returns> <code>true</code> if <var>obj</var> is a value for this
		''' composite type, <code>false</code> otherwise. </returns>
		Public Overridable Function isValue(ByVal obj As Object) As Boolean

			' if obj is null or not CompositeData, return false
			'
			If Not(TypeOf obj Is CompositeData) Then Return False

			' if obj is not a CompositeData, return false
			'
			Dim ___value As CompositeData = CType(obj, CompositeData)

			' test value's CompositeType is assignable to this CompositeType instance
			'
			Dim valueType As CompositeType = ___value.compositeType
			Return valueType.IsSubclassOf(Me)
		End Function

		''' <summary>
		''' Tests whether values of the given type can be assigned to this
		''' open type.  The result is true if the given type is also a
		''' CompositeType with the same name (<seealso cref="#getTypeName()"/>), and
		''' every item in this type is also present in the given type with
		''' the same name and assignable type.  There can be additional
		''' items in the given type, which are ignored.
		''' </summary>
		''' <param name="ot"> the type to be tested.
		''' </param>
		''' <returns> true if {@code ot} is assignable to this open type. </returns>
		Friend Overrides Function isAssignableFrom(Of T1)(ByVal ot As OpenType(Of T1)) As Boolean
			If Not(TypeOf ot Is CompositeType) Then Return False
			Dim ct As CompositeType = CType(ot, CompositeType)
			If Not ct.typeName.Equals(typeName) Then Return False
			For Each key As String In keySet()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim otItemType As OpenType(Of ?) = ct.getType(key)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim thisItemType As OpenType(Of ?) = [getType](key)
				If otItemType Is Nothing OrElse (Not otItemType.IsSubclassOf(thisItemType)) Then Return False
			Next key
			Return True
		End Function


		' *** Methods overriden from class Object *** 

		''' <summary>
		''' Compares the specified <code>obj</code> parameter with this <code>CompositeType</code> instance for equality.
		''' <p>
		''' Two <code>CompositeType</code> instances are equal if and only if all of the following statements are true:
		''' <ul>
		''' <li>their type names are equal</li>
		''' <li>their items' names and types are equal</li>
		''' </ul>
		''' <br>&nbsp; </summary>
		''' <param name="obj">  the object to be compared for equality with this <code>CompositeType</code> instance;
		'''              if <var>obj</var> is <code>null</code>, <code>equals</code> returns <code>false</code>.
		''' </param>
		''' <returns>  <code>true</code> if the specified object is equal to this <code>CompositeType</code> instance. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean

			' if obj is null, return false
			'
			If obj Is Nothing Then Return False

			' if obj is not a CompositeType, return false
			'
			Dim other As CompositeType
			Try
				other = CType(obj, CompositeType)
			Catch e As ClassCastException
				Return False
			End Try

			' Now, really test for equality between this CompositeType instance and the other
			'

			' their names should be equal
			If Not Me.typeName.Equals(other.typeName) Then Return False

			' their items names and types should be equal
			If Not Me.nameToType.Equals(other.nameToType) Then Return False

			' All tests for equality were successfull
			'
			Return True
		End Function

		''' <summary>
		''' Returns the hash code value for this <code>CompositeType</code> instance.
		''' <p>
		''' The hash code of a <code>CompositeType</code> instance is the sum of the hash codes
		''' of all elements of information used in <code>equals</code> comparisons
		''' (ie: name, items names, items types).
		''' This ensures that <code> t1.equals(t2) </code> implies that <code> t1.hashCode()==t2.hashCode() </code>
		''' for any two <code>CompositeType</code> instances <code>t1</code> and <code>t2</code>,
		''' as required by the general contract of the method
		''' <seealso cref="Object#hashCode() Object.hashCode()"/>.
		''' <p>
		''' As <code>CompositeType</code> instances are immutable, the hash code for this instance is calculated once,
		''' on the first call to <code>hashCode</code>, and then the same value is returned for subsequent calls.
		''' </summary>
		''' <returns>  the hash code value for this <code>CompositeType</code> instance </returns>
		Public Overrides Function GetHashCode() As Integer

			' Calculate the hash code value if it has not yet been done (ie 1st call to hashCode())
			'
			If myHashCode Is Nothing Then
				Dim ___value As Integer = 0
				___value += Me.typeName.GetHashCode()
				For Each key As String In nameToDescription.Keys
					___value += key.GetHashCode()
					___value += Me.nameToType(key).GetHashCode()
				Next key
				myHashCode = Convert.ToInt32(___value)
			End If

			' return always the same hash code for this instance (immutable)
			'
			Return myHashCode
		End Function

		''' <summary>
		''' Returns a string representation of this <code>CompositeType</code> instance.
		''' <p>
		''' The string representation consists of
		''' the name of this class (ie <code>javax.management.openmbean.CompositeType</code>), the type name for this instance,
		''' and the list of the items names and types string representation of this instance.
		''' <p>
		''' As <code>CompositeType</code> instances are immutable, the string representation for this instance is calculated once,
		''' on the first call to <code>toString</code>, and then the same value is returned for subsequent calls.
		''' </summary>
		''' <returns>  a string representation of this <code>CompositeType</code> instance </returns>
		Public Overrides Function ToString() As String

			' Calculate the string representation if it has not yet been done (ie 1st call to toString())
			'
			If myToString Is Nothing Then
				Dim result As New StringBuilder
				result.Append(Me.GetType().name)
				result.Append("(name=")
				result.Append(typeName)
				result.Append(",items=(")
				Dim i As Integer=0
				Dim k As IEnumerator(Of String)=nameToType.Keys.GetEnumerator()
				Dim key As String
				Do While k.MoveNext()
					key = k.Current
					If i > 0 Then result.Append(",")
					result.Append("(itemName=")
					result.Append(key)
					result.Append(",itemType=")
					result.Append(nameToType(key).ToString() & ")")
					i += 1
				Loop
				result.Append("))")
				myToString = result.ToString()
			End If

			' return always the same string representation for this instance (immutable)
			'
			Return myToString
		End Function

	End Class

End Namespace
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
	''' The <code>TabularType</code> class is the <i> open type</i> class
	''' whose instances describe the types of <seealso cref="TabularData TabularData"/> values.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class TabularType
		Inherits OpenType(Of TabularData)

		' Serial version 
		Friend Const serialVersionUID As Long = 6554071860220659261L


		''' <summary>
		''' @serial The composite type of rows
		''' </summary>
		Private rowType As CompositeType

		''' <summary>
		''' @serial The items used to index each row element, kept in the order the user gave
		'''         This is an unmodifiable <seealso cref="ArrayList"/>
		''' </summary>
		Private indexNames As IList(Of String)


		<NonSerialized> _
		Private myHashCode As Integer? = Nothing ' As this instance is immutable, these two values
		<NonSerialized> _
		Private myToString As String = Nothing ' need only be calculated once.


		' *** Constructor *** 

		''' <summary>
		''' Constructs a <code>TabularType</code> instance, checking for the validity of the given parameters.
		''' The validity constraints are described below for each parameter.
		''' <p>
		''' The Java class name of tabular data values this tabular type represents
		''' (ie the class name returned by the <seealso cref="OpenType#getClassName() getClassName"/> method)
		''' is set to the string value returned by <code>TabularData.class.getName()</code>.
		''' <p> </summary>
		''' <param name="typeName">  The name given to the tabular type this instance represents; cannot be a null or empty string.
		''' <br>&nbsp; </param>
		''' <param name="description">  The human readable description of the tabular type this instance represents;
		'''                      cannot be a null or empty string.
		''' <br>&nbsp; </param>
		''' <param name="rowType">  The type of the row elements of tabular data values described by this tabular type instance;
		'''                  cannot be null.
		''' <br>&nbsp; </param>
		''' <param name="indexNames">  The names of the items the values of which are used to uniquely index each row element in the
		'''                     tabular data values described by this tabular type instance;
		'''                     cannot be null or empty. Each element should be an item name defined in <var>rowType</var>
		'''                     (no null or empty string allowed).
		'''                     It is important to note that the <b>order</b> of the item names in <var>indexNames</var>
		'''                     is used by the methods <seealso cref="TabularData#get(java.lang.Object[]) get"/> and
		'''                     <seealso cref="TabularData#remove(java.lang.Object[]) remove"/> of class
		'''                     <code>TabularData</code> to match their array of values parameter to items.
		''' <br>&nbsp; </param>
		''' <exception cref="IllegalArgumentException">  if <var>rowType</var> is null,
		'''                                   or <var>indexNames</var> is a null or empty array,
		'''                                   or an element in <var>indexNames</var> is a null or empty string,
		'''                                   or <var>typeName</var> or <var>description</var> is a null or empty string.
		''' <br>&nbsp; </exception>
		''' <exception cref="OpenDataException">  if an element's value of <var>indexNames</var>
		'''                            is not an item name defined in <var>rowType</var>. </exception>
		Public Sub New(ByVal typeName As String, ByVal description As String, ByVal rowType As CompositeType, ByVal indexNames As String())

			' Check and initialize state defined by parent.
			'
			MyBase.New(GetType(TabularData).name, typeName, description, False)

			' Check rowType is not null
			'
			If rowType Is Nothing Then Throw New System.ArgumentException("Argument rowType cannot be null.")

			' Check indexNames is neither null nor empty and does not contain any null element or empty string
			'
			checkForNullElement(indexNames, "indexNames")
			checkForEmptyString(indexNames, "indexNames")

			' Check all indexNames values are valid item names for rowType
			'
			For i As Integer = 0 To indexNames.Length - 1
				If Not rowType.containsKey(indexNames(i)) Then Throw New OpenDataException("Argument's element value indexNames[" & i & "]=""" & indexNames(i) & """ is not a valid item name for rowType.")
			Next i

			' initialize rowType
			'
			Me.rowType = rowType

			' initialize indexNames (copy content so that subsequent
			' modifs to the array referenced by the indexNames parameter
			' have no impact)
			'
			Dim tmpList As IList(Of String) = New List(Of String)(indexNames.Length + 1)
			For i As Integer = 0 To indexNames.Length - 1
				tmpList.Add(indexNames(i))
			Next i
			Me.indexNames = java.util.Collections.unmodifiableList(tmpList)
		End Sub

		''' <summary>
		''' Checks that Object[] arg is neither null nor empty (ie length==0)
		''' and that it does not contain any null element.
		''' </summary>
		Private Shared Sub checkForNullElement(ByVal arg As Object(), ByVal argName As String)
			If (arg Is Nothing) OrElse (arg.Length = 0) Then Throw New System.ArgumentException("Argument " & argName & "[] cannot be null or empty.")
			For i As Integer = 0 To arg.Length - 1
				If arg(i) Is Nothing Then Throw New System.ArgumentException("Argument's element " & argName & "[" & i & "] cannot be null.")
			Next i
		End Sub

		''' <summary>
		''' Checks that String[] does not contain any empty (or blank characters only) string.
		''' </summary>
		Private Shared Sub checkForEmptyString(ByVal arg As String(), ByVal argName As String)
			For i As Integer = 0 To arg.Length - 1
				If arg(i).Trim().Equals("") Then Throw New System.ArgumentException("Argument's element " & argName & "[" & i & "] cannot be an empty string.")
			Next i
		End Sub


		' *** Tabular type specific information methods *** 

		''' <summary>
		''' Returns the type of the row elements of tabular data values
		''' described by this <code>TabularType</code> instance.
		''' </summary>
		''' <returns> the type of each row. </returns>
		Public Overridable Property rowType As CompositeType
			Get
    
				Return rowType
			End Get
		End Property

		''' <summary>
		''' <p>Returns, in the same order as was given to this instance's
		''' constructor, an unmodifiable List of the names of the items the
		''' values of which are used to uniquely index each row element of
		''' tabular data values described by this <code>TabularType</code>
		''' instance.</p>
		''' </summary>
		''' <returns> a List of String representing the names of the index
		''' items.
		'''  </returns>
		Public Overridable Property indexNames As IList(Of String)
			Get
    
				Return indexNames
			End Get
		End Property

		''' <summary>
		''' Tests whether <var>obj</var> is a value which could be
		''' described by this <code>TabularType</code> instance.
		''' 
		''' <p>If <var>obj</var> is null or is not an instance of
		''' <code>javax.management.openmbean.TabularData</code>,
		''' <code>isValue</code> returns <code>false</code>.</p>
		''' 
		''' <p>If <var>obj</var> is an instance of
		''' <code>javax.management.openmbean.TabularData</code>, say {@code
		''' td}, the result is true if this {@code TabularType} is
		''' <em>assignable from</em> {@link TabularData#getTabularType()
		''' td.getTabularType()}, as defined in {@link
		''' CompositeType#isValue CompositeType.isValue}.</p>
		''' </summary>
		''' <param name="obj"> the value whose open type is to be tested for
		''' compatibility with this <code>TabularType</code> instance.
		''' </param>
		''' <returns> <code>true</code> if <var>obj</var> is a value for this
		''' tabular type, <code>false</code> otherwise. </returns>
		Public Overridable Function isValue(ByVal obj As Object) As Boolean

			' if obj is null or not a TabularData, return false
			'
			If Not(TypeOf obj Is TabularData) Then Return False

			' if obj is not a TabularData, return false
			'
			Dim ___value As TabularData = CType(obj, TabularData)
			Dim valueType As TabularType = ___value.tabularType
			Return isAssignableFrom(valueType)
		End Function

		Friend Overrides Function isAssignableFrom(Of T1)(ByVal ot As OpenType(Of T1)) As Boolean
			If Not(TypeOf ot Is TabularType) Then Return False
			Dim tt As TabularType = CType(ot, TabularType)
			If (Not typeName.Equals(tt.typeName)) OrElse (Not indexNames.Equals(tt.indexNames)) Then Return False
			Return tt.rowType.IsSubclassOf(rowType)
		End Function


		' *** Methods overriden from class Object *** 

		''' <summary>
		''' Compares the specified <code>obj</code> parameter with this <code>TabularType</code> instance for equality.
		''' <p>
		''' Two <code>TabularType</code> instances are equal if and only if all of the following statements are true:
		''' <ul>
		''' <li>their type names are equal</li>
		''' <li>their row types are equal</li>
		''' <li>they use the same index names, in the same order</li>
		''' </ul>
		''' <br>&nbsp; </summary>
		''' <param name="obj">  the object to be compared for equality with this <code>TabularType</code> instance;
		'''              if <var>obj</var> is <code>null</code>, <code>equals</code> returns <code>false</code>.
		''' </param>
		''' <returns>  <code>true</code> if the specified object is equal to this <code>TabularType</code> instance. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean

			' if obj is null, return false
			'
			If obj Is Nothing Then Return False

			' if obj is not a TabularType, return false
			'
			Dim other As TabularType
			Try
				other = CType(obj, TabularType)
			Catch e As ClassCastException
				Return False
			End Try

			' Now, really test for equality between this TabularType instance and the other:
			'

			' their names should be equal
			If Not Me.typeName.Equals(other.typeName) Then Return False

			' their row types should be equal
			If Not Me.rowType.Equals(other.rowType) Then Return False

			' their index names should be equal and in the same order (ensured by List.equals())
			If Not Me.indexNames.Equals(other.indexNames) Then Return False

			' All tests for equality were successfull
			'
			Return True
		End Function

		''' <summary>
		''' Returns the hash code value for this <code>TabularType</code> instance.
		''' <p>
		''' The hash code of a <code>TabularType</code> instance is the sum of the hash codes
		''' of all elements of information used in <code>equals</code> comparisons
		''' (ie: name, row type, index names).
		''' This ensures that <code> t1.equals(t2) </code> implies that <code> t1.hashCode()==t2.hashCode() </code>
		''' for any two <code>TabularType</code> instances <code>t1</code> and <code>t2</code>,
		''' as required by the general contract of the method
		''' <seealso cref="Object#hashCode() Object.hashCode()"/>.
		''' <p>
		''' As <code>TabularType</code> instances are immutable, the hash code for this instance is calculated once,
		''' on the first call to <code>hashCode</code>, and then the same value is returned for subsequent calls.
		''' </summary>
		''' <returns>  the hash code value for this <code>TabularType</code> instance </returns>
		Public Overrides Function GetHashCode() As Integer

			' Calculate the hash code value if it has not yet been done (ie 1st call to hashCode())
			'
			If myHashCode Is Nothing Then
				Dim ___value As Integer = 0
				___value += Me.typeName.GetHashCode()
				___value += Me.rowType.GetHashCode()
				For Each index As String In indexNames
					___value += index.GetHashCode()
				Next index
				myHashCode = Convert.ToInt32(___value)
			End If

			' return always the same hash code for this instance (immutable)
			'
			Return myHashCode
		End Function

		''' <summary>
		''' Returns a string representation of this <code>TabularType</code> instance.
		''' <p>
		''' The string representation consists of the name of this class (ie <code>javax.management.openmbean.TabularType</code>),
		''' the type name for this instance, the row type string representation of this instance,
		''' and the index names of this instance.
		''' <p>
		''' As <code>TabularType</code> instances are immutable, the string representation for this instance is calculated once,
		''' on the first call to <code>toString</code>, and then the same value is returned for subsequent calls.
		''' </summary>
		''' <returns>  a string representation of this <code>TabularType</code> instance </returns>
		Public Overrides Function ToString() As String

			' Calculate the string representation if it has not yet been done (ie 1st call to toString())
			'
			If myToString Is Nothing Then
				Dim result As (New StringBuilder).Append(Me.GetType().name).append("(name=").append(typeName).append(",rowType=").append(rowType.ToString()).append(",indexNames=(")
				Dim sep As String = ""
				For Each index As String In indexNames
					result.Append(sep).append(index)
					sep = ","
				Next index
				result.Append("))")
				myToString = result.ToString()
			End If

			' return always the same string representation for this instance (immutable)
			'
			Return myToString
		End Function

	End Class

End Namespace
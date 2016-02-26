Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1998, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.text.html.parser


	''' <summary>
	''' This class defines the attributes of an SGML element
	''' as described in a DTD using the ATTLIST construct.
	''' An AttributeList can be obtained from the Element
	''' class using the getAttributes() method.
	''' <p>
	''' It is actually an element in a linked list. Use the
	''' getNext() method repeatedly to enumerate all the attributes
	''' of an element.
	''' </summary>
	''' <seealso cref=         Element
	''' @author      Arthur Van Hoff
	'''  </seealso>
	<Serializable> _
	Public NotInheritable Class AttributeList
		Implements DTDConstants

		Public name As String
		Public type As Integer
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public values As List(Of ?)
		Public modifier As Integer
		Public value As String
		Public [next] As AttributeList

		Friend Sub New()
		End Sub

		''' <summary>
		''' Create an attribute list element.
		''' </summary>
		Public Sub New(ByVal name As String)
			Me.name = name
		End Sub

		''' <summary>
		''' Create an attribute list element.
		''' </summary>
		Public Sub New(Of T1)(ByVal name As String, ByVal type As Integer, ByVal modifier As Integer, ByVal value As String, ByVal values As List(Of T1), ByVal [next] As AttributeList)
			Me.name = name
			Me.type = type
			Me.modifier = modifier
			Me.value = value
			Me.values = values
			Me.next = [next]
		End Sub

		''' <returns> attribute name </returns>
		Public Property name As String
			Get
				Return name
			End Get
		End Property

		''' <returns> attribute type </returns>
		''' <seealso cref= DTDConstants </seealso>
		Public Property type As Integer
			Get
				Return type
			End Get
		End Property

		''' <returns> attribute modifier </returns>
		''' <seealso cref= DTDConstants </seealso>
		Public Property modifier As Integer
			Get
				Return modifier
			End Get
		End Property

		''' <returns> possible attribute values </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Property values As System.Collections.IEnumerator(Of ?)
			Get
				Return If(values IsNot Nothing, values.elements(), Nothing)
			End Get
		End Property

		''' <returns> default attribute value </returns>
		Public Property value As String
			Get
				Return value
			End Get
		End Property

		''' <returns> the next attribute in the list </returns>
		Public Property [next] As AttributeList
			Get
				Return [next]
			End Get
		End Property

		''' <returns> string representation </returns>
		Public Overrides Function ToString() As String
			Return name
		End Function

		''' <summary>
		''' Create a hashtable of attribute types.
		''' </summary>
		Friend Shared attributeTypes As New Dictionary(Of Object, Object)

		Friend Shared Sub defineAttributeType(ByVal nm As String, ByVal val As Integer)
			Dim num As Integer? = Convert.ToInt32(val)
			attributeTypes(nm) = num
			attributeTypes(num) = nm
		End Sub

		Shared Sub New()
			defineAttributeType("CDATA", CDATA)
			defineAttributeType("ENTITY", ENTITY)
			defineAttributeType("ENTITIES", ENTITIES)
			defineAttributeType("ID", ID)
			defineAttributeType("IDREF", IDREF)
			defineAttributeType("IDREFS", IDREFS)
			defineAttributeType("NAME", NAME)
			defineAttributeType("NAMES", NAMES)
			defineAttributeType("NMTOKEN", NMTOKEN)
			defineAttributeType("NMTOKENS", NMTOKENS)
			defineAttributeType("NOTATION", NOTATION)
			defineAttributeType("NUMBER", NUMBER)
			defineAttributeType("NUMBERS", NUMBERS)
			defineAttributeType("NUTOKEN", NUTOKEN)
			defineAttributeType("NUTOKENS", NUTOKENS)

			attributeTypes("fixed") = Convert.ToInt32(FIXED)
			attributeTypes("required") = Convert.ToInt32(REQUIRED)
			attributeTypes("current") = Convert.ToInt32(CURRENT)
			attributeTypes("conref") = Convert.ToInt32(CONREF)
			attributeTypes("implied") = Convert.ToInt32(IMPLIED)
		End Sub

		Public Shared Function name2type(ByVal nm As String) As Integer
			Dim i As Integer? = CInt(Fix(attributeTypes(nm)))
			Return If(i Is Nothing, CDATA, i)
		End Function

		Public Shared Function type2name(ByVal tp As Integer) As String
			Return CStr(attributeTypes(Convert.ToInt32(tp)))
		End Function
	End Class

End Namespace
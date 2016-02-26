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

Namespace javax.swing.text.html.parser


	''' <summary>
	''' An element as described in a DTD using the ELEMENT construct.
	''' This is essential the description of a tag. It describes the
	''' type, content model, attributes, attribute types etc. It is used
	''' to correctly parse a document by the Parser.
	''' </summary>
	''' <seealso cref= DTD </seealso>
	''' <seealso cref= AttributeList
	''' @author Arthur van Hoff </seealso>
	<Serializable> _
	Public NotInheritable Class Element
		Implements DTDConstants

		Public index As Integer
		Public name As String
		Public oStart As Boolean
		Public oEnd As Boolean
		Public inclusions As BitArray
		Public exclusions As BitArray
		Public type As Integer = ANY
		Public content As ContentModel
		Public atts As AttributeList

		''' <summary>
		''' A field to store user data. Mostly used to store
		''' style sheets.
		''' </summary>
		Public data As Object

		Friend Sub New()
		End Sub

		''' <summary>
		''' Create a new element.
		''' </summary>
		Friend Sub New(ByVal name As String, ByVal index As Integer)
			Me.name = name
			Me.index = index
			If index > maxIndex Then sun.awt.AppContext.appContext.put(MAX_INDEX_KEY, index)
		End Sub

		Private Shared ReadOnly MAX_INDEX_KEY As New Object

		Friend Property Shared maxIndex As Integer
			Get
				Dim value As Integer? = CInt(Fix(sun.awt.AppContext.appContext.get(MAX_INDEX_KEY)))
				Return If(value IsNot Nothing, value, 0)
			End Get
		End Property

		''' <summary>
		''' Get the name of the element.
		''' </summary>
		Public Property name As String
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' Return true if the start tag can be omitted.
		''' </summary>
		Public Function omitStart() As Boolean
			Return oStart
		End Function

		''' <summary>
		''' Return true if the end tag can be omitted.
		''' </summary>
		Public Function omitEnd() As Boolean
			Return oEnd
		End Function

		''' <summary>
		''' Get type.
		''' </summary>
		Public Property type As Integer
			Get
				Return type
			End Get
		End Property

		''' <summary>
		''' Get content model
		''' </summary>
		Public Property content As ContentModel
			Get
				Return content
			End Get
		End Property

		''' <summary>
		''' Get the attributes.
		''' </summary>
		Public Property attributes As AttributeList
			Get
				Return atts
			End Get
		End Property

		''' <summary>
		''' Get index.
		''' </summary>
		Public Property index As Integer
			Get
				Return index
			End Get
		End Property

		''' <summary>
		''' Check if empty
		''' </summary>
		Public Property empty As Boolean
			Get
				Return type = EMPTY
			End Get
		End Property

		''' <summary>
		''' Convert to a string.
		''' </summary>
		Public Overrides Function ToString() As String
			Return name
		End Function

		''' <summary>
		''' Get an attribute by name.
		''' </summary>
		Public Function getAttribute(ByVal name As String) As AttributeList
			Dim a As AttributeList = atts
			Do While a IsNot Nothing
				If a.name.Equals(name) Then Return a
				a = a.next
			Loop
			Return Nothing
		End Function

		''' <summary>
		''' Get an attribute by value.
		''' </summary>
		Public Function getAttributeByValue(ByVal name As String) As AttributeList
			Dim a As AttributeList = atts
			Do While a IsNot Nothing
				If (a.values IsNot Nothing) AndAlso a.values.Contains(name) Then Return a
				a = a.next
			Loop
			Return Nothing
		End Function


		Friend Shared contentTypes As New Dictionary(Of String, Integer?)

		Shared Sub New()
			contentTypes("CDATA") = Convert.ToInt32(CDATA)
			contentTypes("RCDATA") = Convert.ToInt32(RCDATA)
			contentTypes("EMPTY") = Convert.ToInt32(EMPTY)
			contentTypes("ANY") = Convert.ToInt32(ANY)
		End Sub

		Public Shared Function name2type(ByVal nm As String) As Integer
			Dim val As Integer? = contentTypes(nm)
			Return If(val IsNot Nothing, val, 0)
		End Function
	End Class

End Namespace
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
	''' An entity is described in a DTD using the ENTITY construct.
	''' It defines the type and value of the the entity.
	''' </summary>
	''' <seealso cref= DTD
	''' @author Arthur van Hoff </seealso>
	Public NotInheritable Class Entity
		Implements DTDConstants

		Public name As String
		Public type As Integer
		Public data As Char()

		''' <summary>
		''' Creates an entity. </summary>
		''' <param name="name"> the name of the entity </param>
		''' <param name="type"> the type of the entity </param>
		''' <param name="data"> the char array of data </param>
		Public Sub New(ByVal name As String, ByVal type As Integer, ByVal data As Char())
			Me.name = name
			Me.type = type
			Me.data = data
		End Sub

		''' <summary>
		''' Gets the name of the entity. </summary>
		''' <returns> the name of the entity, as a <code>String</code> </returns>
		Public Property name As String
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' Gets the type of the entity. </summary>
		''' <returns> the type of the entity </returns>
		Public Property type As Integer
			Get
				Return type And &HFFFF
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if it is a parameter entity. </summary>
		''' <returns> <code>true</code> if it is a parameter entity </returns>
		Public Property parameter As Boolean
			Get
				Return (type And PARAMETER) <> 0
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if it is a general entity. </summary>
		''' <returns> <code>true</code> if it is a general entity </returns>
		Public Property general As Boolean
			Get
				Return (type And GENERAL) <> 0
			End Get
		End Property

		''' <summary>
		''' Returns the <code>data</code>. </summary>
		''' <returns> the <code>data</code> </returns>
		Public Property data As Char
			Get
			''' <summary>
			''' Returns the data as a <code>String</code>. </summary>
			''' <returns> the data as a <code>String</code> </returns>
				Return New String(data, 0, data.Length)
			End Get
		End Property


		Friend Shared entityTypes As New Dictionary(Of String, Integer?)

		Shared Sub New()
			entityTypes("PUBLIC") = Convert.ToInt32(PUBLIC)
			entityTypes("CDATA") = Convert.ToInt32(CDATA)
			entityTypes("SDATA") = Convert.ToInt32(SDATA)
			entityTypes("PI") = Convert.ToInt32(PI)
			entityTypes("STARTTAG") = Convert.ToInt32(STARTTAG)
			entityTypes("ENDTAG") = Convert.ToInt32(ENDTAG)
			entityTypes("MS") = Convert.ToInt32(MS)
			entityTypes("MD") = Convert.ToInt32(MD)
			entityTypes("SYSTEM") = Convert.ToInt32(SYSTEM)
		End Sub

		''' <summary>
		''' Converts <code>nm</code> string to the corresponding
		''' entity type.  If the string does not have a corresponding
		''' entity type, returns the type corresponding to "CDATA".
		''' Valid entity types are: "PUBLIC", "CDATA", "SDATA", "PI",
		''' "STARTTAG", "ENDTAG", "MS", "MD", "SYSTEM".
		''' </summary>
		''' <param name="nm"> the string to be converted </param>
		''' <returns> the corresponding entity type, or the type corresponding
		'''   to "CDATA", if none exists </returns>
		Public Shared Function name2type(ByVal nm As String) As Integer
			Dim i As Integer? = entityTypes(nm)
			Return If(i Is Nothing, CDATA, i)
		End Function
	End Class

End Namespace
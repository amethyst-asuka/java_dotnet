Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 1998, 2010, Oracle and/or its affiliates. All rights reserved.
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
	''' The representation of an SGML DTD.  DTD describes a document
	''' syntax and is used in parsing of HTML documents.  It contains
	''' a list of elements and their attributes as well as a list of
	''' entities defined in the DTD.
	''' </summary>
	''' <seealso cref= Element </seealso>
	''' <seealso cref= AttributeList </seealso>
	''' <seealso cref= ContentModel </seealso>
	''' <seealso cref= Parser
	''' @author Arthur van Hoff </seealso>
	Public Class DTD
		Implements DTDConstants

		Public name As String
		Public elements As New List(Of Element)
		Public elementHash As New Dictionary(Of String, Element)
		Public entityHash As New Dictionary(Of Object, Entity)
		Public ReadOnly pcdata As Element = getElement("#pcdata")
		Public ReadOnly html As Element = getElement("html")
		Public ReadOnly meta As Element = getElement("meta")
		Public ReadOnly base As Element = getElement("base")
		Public ReadOnly isindex As Element = getElement("isindex")
		Public ReadOnly head As Element = getElement("head")
		Public ReadOnly body As Element = getElement("body")
		Public ReadOnly applet As Element = getElement("applet")
		Public ReadOnly param As Element = getElement("param")
		Public ReadOnly p As Element = getElement("p")
		Public ReadOnly title As Element = getElement("title")
		Friend ReadOnly style As Element = getElement("style")
		Friend ReadOnly link As Element = getElement("link")
		Friend ReadOnly script As Element = getElement("script")

		Public Const FILE_VERSION As Integer = 1

		''' <summary>
		''' Creates a new DTD with the specified name. </summary>
		''' <param name="name"> the name, as a <code>String</code> of the new DTD </param>
		Protected Friend Sub New(ByVal name As String)
			Me.name = name
			defEntity("#RE", GENERAL, ControlChars.Cr)
			defEntity("#RS", GENERAL, ControlChars.Lf)
			defEntity("#SPACE", GENERAL, " "c)
			defineElement("unknown", EMPTY, False, True, Nothing, Nothing, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Gets the name of the DTD. </summary>
		''' <returns> the name of the DTD </returns>
		Public Overridable Property name As String
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' Gets an entity by name. </summary>
		''' <returns> the <code>Entity</code> corresponding to the
		'''   <code>name</code> <code>String</code> </returns>
		Public Overridable Function getEntity(ByVal name As String) As Entity
			Return entityHash(name)
		End Function

		''' <summary>
		''' Gets a character entity. </summary>
		''' <returns> the <code>Entity</code> corresponding to the
		'''    <code>ch</code> character </returns>
		Public Overridable Function getEntity(ByVal ch As Integer) As Entity
			Return entityHash(Convert.ToInt32(ch))
		End Function

		''' <summary>
		''' Returns <code>true</code> if the element is part of the DTD,
		''' otherwise returns <code>false</code>.
		''' </summary>
		''' <param name="name"> the requested <code>String</code> </param>
		''' <returns> <code>true</code> if <code>name</code> exists as
		'''   part of the DTD, otherwise returns <code>false</code> </returns>
		Friend Overridable Function elementExists(ByVal name As String) As Boolean
			Return (Not "unknown".Equals(name)) AndAlso (elementHash(name) IsNot Nothing)
		End Function

		''' <summary>
		''' Gets an element by name. A new element is
		''' created if the element doesn't exist.
		''' </summary>
		''' <param name="name"> the requested <code>String</code> </param>
		''' <returns> the <code>Element</code> corresponding to
		'''   <code>name</code>, which may be newly created </returns>
		Public Overridable Function getElement(ByVal name As String) As Element
			Dim e As Element = elementHash(name)
			If e Is Nothing Then
				e = New Element(name, elements.Count)
				elements.Add(e)
				elementHash(name) = e
			End If
			Return e
		End Function

		''' <summary>
		''' Gets an element by index.
		''' </summary>
		''' <param name="index"> the requested index </param>
		''' <returns> the <code>Element</code> corresponding to
		'''   <code>index</code> </returns>
		Public Overridable Function getElement(ByVal index As Integer) As Element
			Return elements(index)
		End Function

		''' <summary>
		''' Defines an entity.  If the <code>Entity</code> specified
		''' by <code>name</code>, <code>type</code>, and <code>data</code>
		''' exists, it is returned; otherwise a new <code>Entity</code>
		''' is created and is returned.
		''' </summary>
		''' <param name="name"> the name of the <code>Entity</code> as a <code>String</code> </param>
		''' <param name="type"> the type of the <code>Entity</code> </param>
		''' <param name="data"> the <code>Entity</code>'s data </param>
		''' <returns> the <code>Entity</code> requested or a new <code>Entity</code>
		'''   if not found </returns>
		Public Overridable Function defineEntity(ByVal name As String, ByVal type As Integer, ByVal data As Char()) As Entity
			Dim ent As Entity = entityHash(name)
			If ent Is Nothing Then
				ent = New Entity(name, type, data)
				entityHash(name) = ent
				If ((type And GENERAL) <> 0) AndAlso (data.Length = 1) Then
					Select Case type And Not GENERAL
					  Case CDATA, SDATA
						  entityHash(Convert.ToInt32(data(0))) = ent
					End Select
				End If
			End If
			Return ent
		End Function

		''' <summary>
		''' Returns the <code>Element</code> which matches the
		''' specified parameters.  If one doesn't exist, a new
		''' one is created and returned.
		''' </summary>
		''' <param name="name"> the name of the <code>Element</code> </param>
		''' <param name="type"> the type of the <code>Element</code> </param>
		''' <param name="omitStart"> <code>true</code> if start should be omitted </param>
		''' <param name="omitEnd">  <code>true</code> if end should be omitted </param>
		''' <param name="content">  the <code>ContentModel</code> </param>
		''' <param name="atts"> the <code>AttributeList</code> specifying the
		'''    <code>Element</code> </param>
		''' <returns> the <code>Element</code> specified </returns>
		Public Overridable Function defineElement(ByVal name As String, ByVal type As Integer, ByVal omitStart As Boolean, ByVal omitEnd As Boolean, ByVal content As ContentModel, ByVal exclusions As BitArray, ByVal inclusions As BitArray, ByVal atts As AttributeList) As Element
			Dim e As Element = getElement(name)
			e.type = type
			e.oStart = omitStart
			e.oEnd = omitEnd
			e.content = content
			e.exclusions = exclusions
			e.inclusions = inclusions
			e.atts = atts
			Return e
		End Function

		''' <summary>
		''' Defines attributes for an {@code Element}.
		''' </summary>
		''' <param name="name"> the name of the <code>Element</code> </param>
		''' <param name="atts"> the <code>AttributeList</code> specifying the
		'''    <code>Element</code> </param>
		Public Overridable Sub defineAttributes(ByVal name As String, ByVal atts As AttributeList)
			Dim e As Element = getElement(name)
			e.atts = atts
		End Sub

		''' <summary>
		''' Creates and returns a character <code>Entity</code>. </summary>
		''' <param name="name"> the entity's name </param>
		''' <returns> the new character <code>Entity</code> </returns>
		Public Overridable Function defEntity(ByVal name As String, ByVal type As Integer, ByVal ch As Integer) As Entity
			Dim data As Char() = {ChrW(ch)}
			Return defineEntity(name, type, data)
		End Function

		''' <summary>
		''' Creates and returns an <code>Entity</code>. </summary>
		''' <param name="name"> the entity's name </param>
		''' <returns> the new <code>Entity</code> </returns>
		Protected Friend Overridable Function defEntity(ByVal name As String, ByVal type As Integer, ByVal str As String) As Entity
			Dim len As Integer = str.Length
			Dim data As Char() = New Char(len - 1){}
			str.getChars(0, len, data, 0)
			Return defineEntity(name, type, data)
		End Function

		''' <summary>
		''' Creates and returns an <code>Element</code>. </summary>
		''' <param name="name"> the element's name </param>
		''' <returns> the new <code>Element</code> </returns>
		Protected Friend Overridable Function defElement(ByVal name As String, ByVal type As Integer, ByVal omitStart As Boolean, ByVal omitEnd As Boolean, ByVal content As ContentModel, ByVal exclusions As String(), ByVal inclusions As String(), ByVal atts As AttributeList) As Element
			Dim excl As BitArray = Nothing
			If exclusions IsNot Nothing AndAlso exclusions.Length > 0 Then
				excl = New BitArray
				For Each str As String In exclusions
					If str.Length > 0 Then excl.Set(getElement(str).index, True)
				Next str
			End If
			Dim incl As BitArray = Nothing
			If inclusions IsNot Nothing AndAlso inclusions.Length > 0 Then
				incl = New BitArray
				For Each str As String In inclusions
					If str.Length > 0 Then incl.Set(getElement(str).index, True)
				Next str
			End If
			Return defineElement(name, type, omitStart, omitEnd, content, excl, incl, atts)
		End Function

		''' <summary>
		''' Creates and returns an <code>AttributeList</code>. </summary>
		''' <param name="name"> the attribute list's name </param>
		''' <returns> the new <code>AttributeList</code> </returns>
		Protected Friend Overridable Function defAttributeList(ByVal name As String, ByVal type As Integer, ByVal modifier As Integer, ByVal value As String, ByVal values As String, ByVal atts As AttributeList) As AttributeList
			Dim vals As List(Of String) = Nothing
			If values IsNot Nothing Then
				vals = New List(Of String)
				Dim s As New java.util.StringTokenizer(values, "|")
				Do While s.hasMoreTokens()
					Dim str As String = s.nextToken()
					If str.Length > 0 Then vals.Add(str)
				Loop
			End If
			Return New AttributeList(name, type, modifier, value, vals, atts)
		End Function

		''' <summary>
		''' Creates and returns a new content model. </summary>
		''' <param name="type"> the type of the new content model </param>
		''' <returns> the new <code>ContentModel</code> </returns>
		Protected Friend Overridable Function defContentModel(ByVal type As Integer, ByVal obj As Object, ByVal [next] As ContentModel) As ContentModel
			Return New ContentModel(type, obj, [next])
		End Function

		''' <summary>
		''' Returns a string representation of this DTD. </summary>
		''' <returns> the string representation of this DTD </returns>
		Public Overrides Function ToString() As String
			Return name
		End Function

		''' <summary>
		''' The hashtable key of DTDs in AppContext.
		''' </summary>
		Private Shared ReadOnly DTD_HASH_KEY As New Object

		Public Shared Sub putDTDHash(ByVal name As String, ByVal dtd As DTD)
			dtdHash(name) = dtd
		End Sub

		''' <summary>
		''' Returns a DTD with the specified <code>name</code>.  If
		''' a DTD with that name doesn't exist, one is created
		''' and returned.  Any uppercase characters in the name
		''' are converted to lowercase.
		''' </summary>
		''' <param name="name"> the name of the DTD </param>
		''' <returns> the DTD which corresponds to <code>name</code> </returns>
		Public Shared Function getDTD(ByVal name As String) As DTD
			name = name.ToLower()
			Dim ___dtd As DTD = dtdHash(name)
			If ___dtd Is Nothing Then ___dtd = New DTD(name)

			Return ___dtd
		End Function

		Private Property Shared dtdHash As Dictionary(Of String, DTD)
			Get
				Dim appContext As sun.awt.AppContext = sun.awt.AppContext.appContext
    
				Dim result As Dictionary(Of String, DTD) = CType(appContext.get(DTD_HASH_KEY), Dictionary(Of String, DTD))
    
				If result Is Nothing Then
					result = New Dictionary(Of String, DTD)
    
					appContext.put(DTD_HASH_KEY, result)
				End If
    
				Return result
			End Get
		End Property

		''' <summary>
		''' Recreates a DTD from an archived format. </summary>
		''' <param name="in">  the <code>DataInputStream</code> to read from </param>
		Public Overridable Sub read(ByVal [in] As java.io.DataInputStream)
			If [in].readInt() <> FILE_VERSION Then
			End If

			'
			' Read the list of names
			'
			Dim names As String() = New String([in].readShort() - 1){}
			For i As Integer = 0 To names.Length - 1
				names(i) = [in].readUTF()
			Next i


			'
			' Read the entities
			'
			Dim num As Integer = [in].readShort()
			For i As Integer = 0 To num - 1
				Dim nameId As Short = [in].readShort()
				Dim type As Integer = [in].readByte()
				Dim ___name As String = [in].readUTF()
				defEntity(names(nameId), type Or GENERAL, ___name)
			Next i

			' Read the elements
			'
			num = [in].readShort()
			For i As Integer = 0 To num - 1
				Dim nameId As Short = [in].readShort()
				Dim type As Integer = [in].readByte()
				Dim flags As SByte = [in].readByte()
				Dim m As ContentModel = readContentModel([in], names)
				Dim exclusions As String() = readNameArray([in], names)
				Dim inclusions As String() = readNameArray([in], names)
				Dim atts As AttributeList = readAttributeList([in], names)
				defElement(names(nameId), type, ((flags And &H1) <> 0), ((flags And &H2) <> 0), m, exclusions, inclusions, atts)
			Next i
		End Sub

		Private Function readContentModel(ByVal [in] As java.io.DataInputStream, ByVal names As String()) As ContentModel
			Dim flag As SByte = [in].readByte()
			Select Case flag
				Case 0 ' null
					Return Nothing
				Case 1 ' content_c
					Dim type As Integer = [in].readByte()
					Dim m As ContentModel = readContentModel([in], names)
					Dim [next] As ContentModel = readContentModel([in], names)
					Return defContentModel(type, m, [next])
				Case 2 ' content_e
					Dim type As Integer = [in].readByte()
					Dim el As Element = getElement(names([in].readShort()))
					Dim [next] As ContentModel = readContentModel([in], names)
					Return defContentModel(type, el, [next])
			Case Else
					Throw New java.io.IOException("bad bdtd")
			End Select
		End Function

		Private Function readNameArray(ByVal [in] As java.io.DataInputStream, ByVal names As String()) As String()
			Dim num As Integer = [in].readShort()
			If num = 0 Then Return Nothing
			Dim result As String() = New String(num - 1){}
			For i As Integer = 0 To num - 1
				result(i) = names([in].readShort())
			Next i
			Return result
		End Function


		Private Function readAttributeList(ByVal [in] As java.io.DataInputStream, ByVal names As String()) As AttributeList
			Dim result As AttributeList = Nothing
			For num As Integer = [in].readByte() To 1 Step -1
				Dim nameId As Short = [in].readShort()
				Dim type As Integer = [in].readByte()
				Dim modifier As Integer = [in].readByte()
				Dim valueId As Short = [in].readShort()
				Dim value As String = If(valueId = -1, Nothing, names(valueId))
				Dim values As List(Of String) = Nothing
				Dim numValues As Short = [in].readShort()
				If numValues > 0 Then
					values = New List(Of String)(numValues)
					For i As Integer = 0 To numValues - 1
						values.Add(names([in].readShort()))
					Next i
				End If
	result = New AttributeList(names(nameId), type, modifier, value, values, result)
				' We reverse the order of the linked list by doing this, but
				' that order isn't important.
			Next num
			Return result
		End Function

	End Class

End Namespace
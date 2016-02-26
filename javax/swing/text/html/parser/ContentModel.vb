Imports System
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
	''' A representation of a content model. A content model is
	''' basically a restricted BNF expression. It is restricted in
	''' the sense that it must be deterministic. This means that you
	''' don't have to represent it as a finite state automaton.<p>
	''' See Annex H on page 556 of the SGML handbook for more information.
	''' 
	''' @author   Arthur van Hoff
	''' 
	''' </summary>
	<Serializable> _
	Public NotInheritable Class ContentModel
		''' <summary>
		''' Type. Either '*', '?', '+', ',', '|', '&amp;'.
		''' </summary>
		Public type As Integer

		''' <summary>
		''' The content. Either an Element or a ContentModel.
		''' </summary>
		Public content As Object

		''' <summary>
		''' The next content model (in a ',', '|' or '&amp;' expression).
		''' </summary>
		Public [next] As ContentModel

		Public Sub New()
		End Sub

		''' <summary>
		''' Create a content model for an element.
		''' </summary>
		Public Sub New(ByVal content As Element)
			Me.New(0, content, Nothing)
		End Sub

		''' <summary>
		''' Create a content model of a particular type.
		''' </summary>
		Public Sub New(ByVal type As Integer, ByVal content As ContentModel)
			Me.New(type, content, Nothing)
		End Sub

		''' <summary>
		''' Create a content model of a particular type.
		''' </summary>
		Public Sub New(ByVal type As Integer, ByVal content As Object, ByVal [next] As ContentModel)
			Me.type = type
			Me.content = content
			Me.next = [next]
		End Sub

		''' <summary>
		''' Return true if the content model could
		''' match an empty input stream.
		''' </summary>
		Public Function empty() As Boolean
			Select Case type
			  Case "*"c, "?"c
				Return True

			  Case "+"c, "|"c
				Dim m As ContentModel = CType(content, ContentModel)
				Do While m IsNot Nothing
					If m.empty() Then Return True
					m = m.next
				Loop
				Return False

			  Case ","c, "&"c
				Dim m As ContentModel = CType(content, ContentModel)
				Do While m IsNot Nothing
					If Not m.empty() Then Return False
					m = m.next
				Loop
				Return True

			  Case Else
				Return False
			End Select
		End Function

		''' <summary>
		''' Update elemVec with the list of elements that are
		''' part of the this contentModel.
		''' </summary>
		 Public Sub getElements(ByVal elemVec As List(Of Element))
			 Select Case type
			 Case "*"c, "?"c, "+"c
				 CType(content, ContentModel).getElements(elemVec)
			 Case ","c, "|"c, "&"c
				 Dim m As ContentModel=CType(content, ContentModel)
				 Do While m IsNot Nothing
					 m.getElements(elemVec)
					 m=m.next
				 Loop
			 Case Else
				 elemVec.Add(CType(content, Element))
			 End Select
		 End Sub

		 Private valSet As Boolean()
		 Private val As Boolean()
		 ' A cache used by first().  This cache was found to speed parsing
		 ' by about 10% (based on measurements of the 4-12 code base after
		 ' buffering was fixed).

		''' <summary>
		''' Return true if the token could potentially be the
		''' first token in the input stream.
		''' </summary>
		Public Function first(ByVal token As Object) As Boolean
			Select Case type
			  Case "*"c, "?"c, "+"c
				Return CType(content, ContentModel).first(token)

			  Case ","c
				Dim m As ContentModel = CType(content, ContentModel)
				Do While m IsNot Nothing
					If m.first(token) Then Return True
					If Not m.empty() Then Return False
					m = m.next
				Loop
				Return False

			  Case "|"c, "&"c
				Dim e As Element = CType(token, Element)
				If valSet Is Nothing OrElse valSet.Length <= Element.maxIndex Then
					valSet = New Boolean(Element.maxIndex){}
					val = New Boolean(valSet.Length - 1){}
				End If
				If valSet(e.index) Then Return val(e.index)
				Dim m As ContentModel = CType(content, ContentModel)
				Do While m IsNot Nothing
					If m.first(token) Then
						val(e.index) = True
						Exit Do
					End If
					m = m.next
				Loop
				valSet(e.index) = True
				Return val(e.index)

			  Case Else
				Return (content Is token)
				' PENDING: refer to comment in ContentModelState
	'
	'              if (content == token) {
	'                  return true;
	'              }
	'              Element e = (Element)content;
	'              if (e.omitStart() && e.content != null) {
	'                  return e.content.first(token);
	'              }
	'              return false;
	'
			End Select
		End Function

		''' <summary>
		''' Return the element that must be next.
		''' </summary>
		Public Function first() As Element
			Select Case type
			  Case "&"c, "|"c, "*"c, "?"c
				Return Nothing

			  Case "+"c, ","c
				Return CType(content, ContentModel).first()

			  Case Else
				Return CType(content, Element)
			End Select
		End Function

		''' <summary>
		''' Convert to a string.
		''' </summary>
		Public Overrides Function ToString() As String
			Select Case type
			  Case "*"c
				Return content & "*"
			  Case "?"c
				Return content & "?"
			  Case "+"c
				Return content & "+"

			  Case ","c, "|"c, "&"c
				Dim data As Char() = {" "c, ChrW(type), " "c}
				Dim str As String = ""
				Dim m As ContentModel = CType(content, ContentModel)
				Do While m IsNot Nothing
					str = str + m
					If m.next IsNot Nothing Then str += New String(data)
					m = m.next
				Loop
				Return "(" & str & ")"

			  Case Else
				Return content.ToString()
			End Select
		End Function
	End Class

End Namespace
'
' * Copyright (c) 1998, Oracle and/or its affiliates. All rights reserved.
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
	''' A generic HTML TagElement class. The methods define how white
	''' space is interpreted around the tag.
	''' 
	''' @author      Sunita Mani
	''' </summary>

	Public Class TagElement

		Friend elem As Element
		Friend htmlTag As javax.swing.text.html.HTML.Tag
		Friend insertedByErrorRecovery As Boolean

		Public Sub New(ByVal elem As Element)
			Me.New(elem, False)
		End Sub

		Public Sub New(ByVal elem As Element, ByVal fictional As Boolean)
			Me.elem = elem
			htmlTag = javax.swing.text.html.HTML.getTag(elem.name)
			If htmlTag Is Nothing Then htmlTag = New javax.swing.text.html.HTML.UnknownTag(elem.name)
			insertedByErrorRecovery = fictional
		End Sub

		Public Overridable Function breaksFlow() As Boolean
			Return htmlTag.breaksFlow()
		End Function

		Public Overridable Property preformatted As Boolean
			Get
				Return htmlTag.preformatted
			End Get
		End Property

		Public Overridable Property element As Element
			Get
				Return elem
			End Get
		End Property

		Public Overridable Property hTMLTag As javax.swing.text.html.HTML.Tag
			Get
				Return htmlTag
			End Get
		End Property

		Public Overridable Function fictional() As Boolean
			Return insertedByErrorRecovery
		End Function
	End Class

End Namespace
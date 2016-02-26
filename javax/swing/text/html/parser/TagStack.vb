Imports Microsoft.VisualBasic
Imports System
Imports System.Collections

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
	''' A stack of tags. Used while parsing an HTML document.
	''' It, together with the ContentModelStates, defines the
	''' complete state of the parser while reading a document.
	''' When a start tag is encountered an element is pushed onto
	''' the stack, when an end tag is enountered an element is popped
	''' of the stack.
	''' </summary>
	''' <seealso cref= Parser </seealso>
	''' <seealso cref= DTD </seealso>
	''' <seealso cref= ContentModelState
	''' @author      Arthur van Hoff </seealso>
	Friend NotInheritable Class TagStack
		Implements DTDConstants

		Friend tag As TagElement
		Friend elem As Element
		Friend state As ContentModelState
		Friend [next] As TagStack
		Friend inclusions As BitArray
		Friend exclusions As BitArray
		Friend net As Boolean
		Friend pre As Boolean

		''' <summary>
		''' Construct a stack element.
		''' </summary>
		Friend Sub New(ByVal tag As TagElement, ByVal [next] As TagStack)
			Me.tag = tag
			Me.elem = tag.element
			Me.next = [next]

			Dim elem As Element = tag.element
			If elem.content IsNot Nothing Then Me.state = New ContentModelState(elem.content)

			If [next] IsNot Nothing Then
				inclusions = [next].inclusions
				exclusions = [next].exclusions
				pre = [next].pre
			End If
			If tag.preformatted Then pre = True

			If elem.inclusions IsNot Nothing Then
				If inclusions IsNot Nothing Then
					inclusions = CType(inclusions.clone(), BitArray)
					inclusions = inclusions.Or(elem.inclusions)
				Else
					inclusions = elem.inclusions
				End If
			End If
			If elem.exclusions IsNot Nothing Then
				If exclusions IsNot Nothing Then
					exclusions = CType(exclusions.clone(), BitArray)
					exclusions = exclusions.Or(elem.exclusions)
				Else
					exclusions = elem.exclusions
				End If
			End If
		End Sub

		''' <summary>
		''' Return the element that must come next in the
		''' input stream.
		''' </summary>
		Public Function first() As Element
			Return If(state IsNot Nothing, state.first(), Nothing)
		End Function

		''' <summary>
		''' Return the ContentModel that must be satisfied by
		''' what comes next in the input stream.
		''' </summary>
		Public Function contentModel() As ContentModel
			If state Is Nothing Then
				Return Nothing
			Else
				Return state.model
			End If
		End Function

		''' <summary>
		''' Return true if the element that is contained at
		''' the index specified by the parameter is part of
		''' the exclusions specified in the DTD for the element
		''' currently on the TagStack.
		''' </summary>
		Friend Function excluded(ByVal elemIndex As Integer) As Boolean
			Return (exclusions IsNot Nothing) AndAlso exclusions.Get(elem.index)
		End Function


		''' <summary>
		''' Advance the state by reducing the given element.
		''' Returns false if the element is not legal and the
		''' state is not advanced.
		''' </summary>
		Friend Function advance(ByVal elem As Element) As Boolean
			If (exclusions IsNot Nothing) AndAlso exclusions.Get(elem.index) Then Return False
			If state IsNot Nothing Then
				Dim newState As ContentModelState = state.advance(elem)
				If newState IsNot Nothing Then
					state = newState
					Return True
				End If
			ElseIf Me.elem.type = ANY Then
				Return True
			End If
			Return (inclusions IsNot Nothing) AndAlso inclusions.Get(elem.index)
		End Function

		''' <summary>
		''' Return true if the current state can be terminated.
		''' </summary>
		Friend Function terminate() As Boolean
			Return (state Is Nothing) OrElse state.terminate()
		End Function

		''' <summary>
		''' Convert to a string.
		''' </summary>
		Public Overrides Function ToString() As String
			Return If([next] Is Nothing, "<" & tag.element.name & ">", [next] & " <" & tag.element.name & ">")
		End Function
	End Class

	Friend Class NPrintWriter
		Inherits PrintWriter

		Private numLines As Integer = 5
		Private numPrinted As Integer = 0

		Public Sub New(ByVal numberOfLines As Integer)
			MyBase.New(System.out)
			numLines = numberOfLines
		End Sub

		Public Overridable Sub println(ByVal array As Char())
			If numPrinted >= numLines Then Return

			Dim partialArray As Char() = Nothing

			For i As Integer = 0 To array.Length - 1
				If array(i) = ControlChars.Lf Then numPrinted += 1

				If numPrinted = numLines Then Array.Copy(array, 0, partialArray, 0, i)
			Next i

			If partialArray IsNot Nothing Then MyBase.print(partialArray)

			If numPrinted = numLines Then Return

			MyBase.println(array)
			numPrinted += 1
		End Sub
	End Class

End Namespace
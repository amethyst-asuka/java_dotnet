'
' * Copyright (c) 1998, 2000, Oracle and/or its affiliates. All rights reserved.
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
	''' A content model state. This is basically a list of pointers to
	''' the BNF expression representing the model (the ContentModel).
	''' Each element in a DTD has a content model which describes the
	''' elements that may occur inside, and the order in which they can
	''' occur.
	''' <p>
	''' Each time a token is reduced a new state is created.
	''' <p>
	''' See Annex H on page 556 of the SGML handbook for more information.
	''' </summary>
	''' <seealso cref= Parser </seealso>
	''' <seealso cref= DTD </seealso>
	''' <seealso cref= Element </seealso>
	''' <seealso cref= ContentModel
	''' @author Arthur van Hoff </seealso>
	Friend Class ContentModelState
		Friend model As ContentModel
		Friend value As Long
		Friend [next] As ContentModelState

		''' <summary>
		''' Create a content model state for a content model.
		''' </summary>
		Public Sub New(ByVal model As ContentModel)
			Me.New(model, Nothing, 0)
		End Sub

		''' <summary>
		''' Create a content model state for a content model given the
		''' remaining state that needs to be reduce.
		''' </summary>
		Friend Sub New(ByVal content As Object, ByVal [next] As ContentModelState)
			Me.New(content, [next], 0)
		End Sub

		''' <summary>
		''' Create a content model state for a content model given the
		''' remaining state that needs to be reduce.
		''' </summary>
		Friend Sub New(ByVal content As Object, ByVal [next] As ContentModelState, ByVal value As Long)
			Me.model = CType(content, ContentModel)
			Me.next = [next]
			Me.value = value
		End Sub

		''' <summary>
		''' Return the content model that is relevant to the current state.
		''' </summary>
		Public Overridable Property model As ContentModel
			Get
				Dim m As ContentModel = model
				For i As Integer = 0 To value - 1
					If m.next IsNot Nothing Then
						m = m.next
					Else
						Return Nothing
					End If
				Next i
				Return m
			End Get
		End Property

		''' <summary>
		''' Check if the state can be terminated. That is there are no more
		''' tokens required in the input stream. </summary>
		''' <returns> true if the model can terminate without further input </returns>
		Public Overridable Function terminate() As Boolean
			Select Case model.type
			  Case "+"c
				If (value = 0) AndAlso Not(model).empty() Then Return False
			  Case "*"c, "?"c
				Return ([next] Is Nothing) OrElse [next].terminate()

			  Case "|"c
				Dim m As ContentModel = CType(model.content, ContentModel)
				Do While m IsNot Nothing
					If m.empty() Then Return ([next] Is Nothing) OrElse [next].terminate()
					m = m.next
				Loop
				Return False

			  Case "&"c
				Dim m As ContentModel = CType(model.content, ContentModel)

				Dim i As Integer = 0
				Do While m IsNot Nothing
					If (value And (1L << i)) = 0 Then
						If Not m.empty() Then Return False
					End If
					i += 1
					m = m.next
				Loop
				Return ([next] Is Nothing) OrElse [next].terminate()

			  Case ","c
				Dim m As ContentModel = CType(model.content, ContentModel)
				Dim i As Integer = 0
				Do While i < value

					i += 1
					m = m.next
				Loop

				Do While (m IsNot Nothing) AndAlso m.empty()

					m = m.next
				Loop
				If m IsNot Nothing Then Return False
				Return ([next] Is Nothing) OrElse [next].terminate()

			Case Else
			  Return False
			End Select
		End Function

		''' <summary>
		''' Check if the state can be terminated. That is there are no more
		''' tokens required in the input stream. </summary>
		''' <returns> the only possible element that can occur next </returns>
		Public Overridable Function first() As Element
			Select Case model.type
			  Case "*"c, "?"c, "|"c, "&"c
				Return Nothing

			  Case "+"c
				Return model.first()

			  Case ","c
				  Dim m As ContentModel = CType(model.content, ContentModel)
				  Dim i As Integer = 0
				  Do While i < value

					  i += 1
					  m = m.next
				  Loop
				  Return m.first()

			  Case Else
				Return model.first()
			End Select
		End Function

		''' <summary>
		''' Advance this state to a new state. An exception is thrown if the
		''' token is illegal at this point in the content model. </summary>
		''' <returns> next state after reducing a token </returns>
		Public Overridable Function advance(ByVal token As Object) As ContentModelState
			Select Case model.type
			  Case "+"c
				If model.first(token) Then Return (New ContentModelState(model.content, New ContentModelState(model, [next], value + 1))).advance(token)
				If value <> 0 Then
					If [next] IsNot Nothing Then
						Return [next].advance(token)
					Else
						Return Nothing
					End If
				End If

			  Case "*"c
				If model.first(token) Then Return (New ContentModelState(model.content, Me)).advance(token)
				If [next] IsNot Nothing Then
					Return [next].advance(token)
				Else
					Return Nothing
				End If

			  Case "?"c
				If model.first(token) Then Return (New ContentModelState(model.content, [next])).advance(token)
				If [next] IsNot Nothing Then
					Return [next].advance(token)
				Else
					Return Nothing
				End If

			  Case "|"c
				Dim m As ContentModel = CType(model.content, ContentModel)
				Do While m IsNot Nothing
					If m.first(token) Then Return (New ContentModelState(m, [next])).advance(token)
					m = m.next
				Loop

			  Case ","c
				Dim m As ContentModel = CType(model.content, ContentModel)
				Dim i As Integer = 0
				Do While i < value

					i += 1
					m = m.next
				Loop

				If m.first(token) OrElse m.empty() Then
					If m.next Is Nothing Then
						Return (New ContentModelState(m, [next])).advance(token)
					Else
						Return (New ContentModelState(m, New ContentModelState(model, [next], value + 1))).advance(token)
					End If
				End If
				Exit Select

			  Case "&"c
				Dim m As ContentModel = CType(model.content, ContentModel)
				Dim complete As Boolean = True

				Dim i As Integer = 0
				Do While m IsNot Nothing
					If (value And (1L << i)) = 0 Then
						If m.first(token) Then Return (New ContentModelState(m, New ContentModelState(model, [next], value Or (1L << i)))).advance(token)
						If Not m.empty() Then complete = False
					End If
					i += 1
					m = m.next
				Loop
				If complete Then
					If [next] IsNot Nothing Then
						Return [next].advance(token)
					Else
						Return Nothing
					End If
				End If
				Exit Select

			  Case Else
				If model.content Is token Then
					If [next] Is Nothing AndAlso (TypeOf token Is Element) AndAlso CType(token, Element).content IsNot Nothing Then Return New ContentModelState(CType(token, Element).content)
					Return [next]
				End If
				' PENDING: Currently we don't correctly deal with optional start
				' tags. This can most notably be seen with the 4.01 spec where
				' TBODY's start and end tags are optional.
				' Uncommenting this and the PENDING in ContentModel will
				' correctly skip the omit tags, but the delegate is not notified.
				' Some additional API needs to be added to track skipped tags,
				' and this can then be added back.
	'
	'            if ((model.content instanceof Element)) {
	'                Element e = (Element)model.content;
	'
	'                if (e.omitStart() && e.content != null) {
	'                    return new ContentModelState(e.content, next).advance(
	'                                           token);
	'                }
	'            }
	'
			End Select

			' We used to throw this exception at this point.  However, it
			' was determined that throwing this exception was more expensive
			' than returning null, and we could not justify to ourselves why
			' it was necessary to throw an exception, rather than simply
			' returning null.  I'm leaving it in a commented out state so
			' that it can be easily restored if the situation ever arises.
			'
			' throw new IllegalArgumentException("invalid token: " + token);
			Return Nothing
		End Function
	End Class

End Namespace
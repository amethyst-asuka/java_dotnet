Imports Microsoft.VisualBasic
Imports System
Imports javax.swing.text

'
' * Copyright (c) 1997, 2008, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text.html


	''' <summary>
	''' A view implementation to display an html horizontal
	''' rule.
	''' 
	''' @author  Timothy Prinzing
	''' @author  Sara Swanson
	''' </summary>
	Friend Class HRuleView
		Inherits View

		''' <summary>
		''' Creates a new view that represents an &lt;hr&gt; element.
		''' </summary>
		''' <param name="elem"> the element to create a view for </param>
		Public Sub New(ByVal elem As Element)
			MyBase.New(elem)
			propertiesFromAttributestes()
		End Sub

		''' <summary>
		''' Update any cached values that come from attributes.
		''' </summary>
		Protected Friend Overridable Sub setPropertiesFromAttributes()
			Dim sheet As StyleSheet = CType(document, HTMLDocument).styleSheet
			Dim eAttr As AttributeSet = element.attributes
			attr = sheet.getViewAttributes(Me)

			alignment = StyleConstants.ALIGN_CENTER
			size = 0
			noshade = Nothing
			widthValue = Nothing

			If attr IsNot Nothing Then
				' getAlignment() returns ALIGN_LEFT by default, and HR should
				' use ALIGN_CENTER by default, so we check if the alignment
				' attribute is actually defined
				If attr.getAttribute(StyleConstants.Alignment) IsNot Nothing Then alignment = StyleConstants.getAlignment(attr)

				noshade = CStr(eAttr.getAttribute(HTML.Attribute.NOSHADE))
				Dim value As Object = eAttr.getAttribute(HTML.Attribute.SIZE)
				If value IsNot Nothing AndAlso (TypeOf value Is String) Then
					Try
						size = Convert.ToInt32(CStr(value))
					Catch e As NumberFormatException
						size = 1
					End Try
				End If
				value = attr.getAttribute(CSS.Attribute.WIDTH)
				If value IsNot Nothing AndAlso (TypeOf value Is CSS.LengthValue) Then widthValue = CType(value, CSS.LengthValue)
				topMargin = getLength(CSS.Attribute.MARGIN_TOP, attr)
				bottomMargin = getLength(CSS.Attribute.MARGIN_BOTTOM, attr)
				leftMargin = getLength(CSS.Attribute.MARGIN_LEFT, attr)
				rightMargin = getLength(CSS.Attribute.MARGIN_RIGHT, attr)
			Else
					rightMargin = 0
						leftMargin = rightMargin
							bottomMargin = leftMargin
							topMargin = bottomMargin
			End If
			size = Math.Max(2, size)
		End Sub

		' This will be removed and centralized at some point, need to unify this
		' and avoid private classes.
		Private Function getLength(ByVal key As CSS.Attribute, ByVal a As AttributeSet) As Single
			Dim lv As CSS.LengthValue = CType(a.getAttribute(key), CSS.LengthValue)
			Dim len As Single = If(lv IsNot Nothing, lv.value, 0)
			Return len
		End Function

		' --- View methods ---------------------------------------------

		''' <summary>
		''' Paints the view.
		''' </summary>
		''' <param name="g"> the graphics context </param>
		''' <param name="a"> the allocation region for the view </param>
		''' <seealso cref= View#paint </seealso>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal a As Shape)
			Dim alloc As Rectangle = If(TypeOf a Is Rectangle, CType(a, Rectangle), a.bounds)
			Dim x As Integer = 0
			Dim y As Integer = alloc.y + SPACE_ABOVE + CInt(Fix(topMargin))
			Dim width As Integer = alloc.width - CInt(Fix(leftMargin + rightMargin))
			If widthValue IsNot Nothing Then width = CInt(Fix(widthValue.getValue(CSng(width))))
			Dim height As Integer = alloc.height - (SPACE_ABOVE + SPACE_BELOW + CInt(Fix(topMargin)) + CInt(Fix(bottomMargin)))
			If size > 0 Then height = size

			' Align the rule horizontally.
			Select Case alignment
			Case StyleConstants.ALIGN_CENTER
				x = alloc.x + (alloc.width / 2) - (width \ 2)
			Case StyleConstants.ALIGN_RIGHT
				x = alloc.x + alloc.width - width - CInt(Fix(rightMargin))
			Case Else
				x = alloc.x + CInt(Fix(leftMargin))
			End Select

			' Paint either a shaded rule or a solid line.
			If noshade IsNot Nothing Then
				g.color = Color.black
				g.fillRect(x, y, width, height)
			Else
				Dim bg As Color = container.background
				Dim bottom, top As Color
				If bg Is Nothing OrElse bg.Equals(Color.white) Then
					top = Color.darkGray
					bottom = Color.lightGray
				Else
					top = Color.darkGray
					bottom = Color.white
				End If
				g.color = bottom
				g.drawLine(x + width - 1, y, x + width - 1, y + height - 1)
				g.drawLine(x, y + height - 1, x + width - 1, y + height - 1)
				g.color = top
				g.drawLine(x, y, x + width - 1, y)
				g.drawLine(x, y, x, y + height - 1)
			End If

		End Sub


		''' <summary>
		''' Calculates the desired shape of the rule... this is
		''' basically the preferred size of the border.
		''' </summary>
		''' <param name="axis"> may be either X_AXIS or Y_AXIS </param>
		''' <returns> the desired span </returns>
		''' <seealso cref= View#getPreferredSpan </seealso>
		Public Overrides Function getPreferredSpan(ByVal axis As Integer) As Single
			Select Case axis
			Case View.X_AXIS
				Return 1
			Case View.Y_AXIS
				If size > 0 Then
					Return size + SPACE_ABOVE + SPACE_BELOW + topMargin + bottomMargin
				Else
					If noshade IsNot Nothing Then
						Return 2 + SPACE_ABOVE + SPACE_BELOW + topMargin + bottomMargin
					Else
						Return SPACE_ABOVE + SPACE_BELOW + topMargin +bottomMargin
					End If
				End If
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case Else
				Throw New System.ArgumentException("Invalid axis: " & axis)
			End Select
		End Function

		''' <summary>
		''' Gets the resize weight for the axis.
		''' The rule is: rigid vertically and flexible horizontally.
		''' </summary>
		''' <param name="axis"> may be either X_AXIS or Y_AXIS </param>
		''' <returns> the weight </returns>
		Public Overrides Function getResizeWeight(ByVal axis As Integer) As Integer
			If axis = View.X_AXIS Then
					Return 1
			ElseIf axis = View.Y_AXIS Then
					Return 0
			Else
				Return 0
			End If
		End Function

		''' <summary>
		''' Determines how attractive a break opportunity in
		''' this view is.  This is implemented to request a forced break.
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <param name="pos"> the potential location of the start of the
		'''   broken view (greater than or equal to zero).
		'''   This may be useful for calculating tab
		'''   positions. </param>
		''' <param name="len"> specifies the relative length from <em>pos</em>
		'''   where a potential break is desired. The value must be greater
		'''   than or equal to zero. </param>
		''' <returns> the weight, which should be a value between
		'''   ForcedBreakWeight and BadBreakWeight. </returns>
		Public Overrides Function getBreakWeight(ByVal axis As Integer, ByVal pos As Single, ByVal len As Single) As Integer
			If axis = X_AXIS Then Return ForcedBreakWeight
			Return BadBreakWeight
		End Function

		Public Overrides Function breakView(ByVal axis As Integer, ByVal offset As Integer, ByVal pos As Single, ByVal len As Single) As View
			Return Nothing
		End Function

		''' <summary>
		''' Provides a mapping from the document model coordinate space
		''' to the coordinate space of the view mapped to it.
		''' </summary>
		''' <param name="pos"> the position to convert </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <returns> the bounding box of the given position </returns>
		''' <exception cref="BadLocationException">  if the given position does not
		''' represent a valid location in the associated document </exception>
		''' <seealso cref= View#modelToView </seealso>
		Public Overrides Function modelToView(ByVal pos As Integer, ByVal a As Shape, ByVal b As Position.Bias) As Shape
			Dim p0 As Integer = startOffset
			Dim p1 As Integer = endOffset
			If (pos >= p0) AndAlso (pos <= p1) Then
				Dim r As Rectangle = a.bounds
				If pos = p1 Then r.x += r.width
				r.width = 0
				Return r
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Provides a mapping from the view coordinate space to the logical
		''' coordinate space of the model.
		''' </summary>
		''' <param name="x"> the X coordinate </param>
		''' <param name="y"> the Y coordinate </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <returns> the location within the model that best represents the
		'''  given point of view </returns>
		''' <seealso cref= View#viewToModel </seealso>
		Public Overrides Function viewToModel(ByVal x As Single, ByVal y As Single, ByVal a As Shape, ByVal bias As Position.Bias()) As Integer
			Dim alloc As Rectangle = CType(a, Rectangle)
			If x < alloc.x + (alloc.width / 2) Then
				bias(0) = Position.Bias.Forward
				Return startOffset
			End If
			bias(0) = Position.Bias.Backward
			Return endOffset
		End Function

		''' <summary>
		''' Fetches the attributes to use when rendering.  This is
		''' implemented to multiplex the attributes specified in the
		''' model with a StyleSheet.
		''' </summary>
		Public Property Overrides attributes As AttributeSet
			Get
				Return attr
			End Get
		End Property

		Public Overridable Sub changedUpdate(ByVal changes As javax.swing.event.DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			MyBase.changedUpdate(changes, a, f)
			Dim pos As Integer = changes.offset
			If pos <= startOffset AndAlso (pos + changes.length) >= endOffset Then propertiesFromAttributestes()
		End Sub

		' --- variables ------------------------------------------------

		Private topMargin As Single
		Private bottomMargin As Single
		Private leftMargin As Single
		Private rightMargin As Single
		Private alignment As Integer = StyleConstants.ALIGN_CENTER
		Private noshade As String = Nothing
		Private size As Integer = 0
		Private widthValue As CSS.LengthValue

		Private Const SPACE_ABOVE As Integer = 3
		Private Const SPACE_BELOW As Integer = 3

		''' <summary>
		''' View Attributes. </summary>
		Private attr As AttributeSet
	End Class

End Namespace
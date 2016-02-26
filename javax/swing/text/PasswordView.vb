Imports Microsoft.VisualBasic

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text


	''' <summary>
	''' Implements a View suitable for use in JPasswordField
	''' UI implementations.  This is basically a field ui that
	''' renders its contents as the echo character specified
	''' in the associated component (if it can narrow the
	''' component to a JPasswordField).
	''' 
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref=     View </seealso>
	Public Class PasswordView
		Inherits FieldView

		''' <summary>
		''' Constructs a new view wrapped on an element.
		''' </summary>
		''' <param name="elem"> the element </param>
		Public Sub New(ByVal elem As Element)
			MyBase.New(elem)
		End Sub

		''' <summary>
		''' Renders the given range in the model as normal unselected
		''' text.  This sets the foreground color and echos the characters
		''' using the value returned by getEchoChar().
		''' </summary>
		''' <param name="g"> the graphics context </param>
		''' <param name="x"> the starting X coordinate &gt;= 0 </param>
		''' <param name="y"> the starting Y coordinate &gt;= 0 </param>
		''' <param name="p0"> the starting offset in the model &gt;= 0 </param>
		''' <param name="p1"> the ending offset in the model &gt;= p0 </param>
		''' <returns> the X location of the end of the range &gt;= 0 </returns>
		''' <exception cref="BadLocationException"> if p0 or p1 are out of range </exception>
		Protected Friend Overrides Function drawUnselectedText(ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal p0 As Integer, ByVal p1 As Integer) As Integer

			Dim c As Container = container
			If TypeOf c Is javax.swing.JPasswordField Then
				Dim f As javax.swing.JPasswordField = CType(c, javax.swing.JPasswordField)
				If Not f.echoCharIsSet() Then Return MyBase.drawUnselectedText(g, x, y, p0, p1)
				If f.enabled Then
					g.color = f.foreground
				Else
					g.color = f.disabledTextColor
				End If
				Dim echoChar As Char = f.echoChar
				Dim n As Integer = p1 - p0
				For i As Integer = 0 To n - 1
					x = drawEchoCharacter(g, x, y, echoChar)
				Next i
			End If
			Return x
		End Function

		''' <summary>
		''' Renders the given range in the model as selected text.  This
		''' is implemented to render the text in the color specified in
		''' the hosting component.  It assumes the highlighter will render
		''' the selected background.  Uses the result of getEchoChar() to
		''' display the characters.
		''' </summary>
		''' <param name="g"> the graphics context </param>
		''' <param name="x"> the starting X coordinate &gt;= 0 </param>
		''' <param name="y"> the starting Y coordinate &gt;= 0 </param>
		''' <param name="p0"> the starting offset in the model &gt;= 0 </param>
		''' <param name="p1"> the ending offset in the model &gt;= p0 </param>
		''' <returns> the X location of the end of the range &gt;= 0 </returns>
		''' <exception cref="BadLocationException"> if p0 or p1 are out of range </exception>
		Protected Friend Overrides Function drawSelectedText(ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal p0 As Integer, ByVal p1 As Integer) As Integer
			g.color = selected
			Dim c As Container = container
			If TypeOf c Is javax.swing.JPasswordField Then
				Dim f As javax.swing.JPasswordField = CType(c, javax.swing.JPasswordField)
				If Not f.echoCharIsSet() Then Return MyBase.drawSelectedText(g, x, y, p0, p1)
				Dim echoChar As Char = f.echoChar
				Dim n As Integer = p1 - p0
				For i As Integer = 0 To n - 1
					x = drawEchoCharacter(g, x, y, echoChar)
				Next i
			End If
			Return x
		End Function

		''' <summary>
		''' Renders the echo character, or whatever graphic should be used
		''' to display the password characters.  The color in the Graphics
		''' object is set to the appropriate foreground color for selected
		''' or unselected text.
		''' </summary>
		''' <param name="g"> the graphics context </param>
		''' <param name="x"> the starting X coordinate &gt;= 0 </param>
		''' <param name="y"> the starting Y coordinate &gt;= 0 </param>
		''' <param name="c"> the echo character </param>
		''' <returns> the updated X position &gt;= 0 </returns>
		Protected Friend Overridable Function drawEchoCharacter(ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal c As Char) As Integer
			ONE(0) = c
			sun.swing.SwingUtilities2.drawChars(Utilities.getJComponent(Me), g, ONE, 0, 1, x, y)
			Return x + g.fontMetrics.charWidth(c)
		End Function

		''' <summary>
		''' Provides a mapping from the document model coordinate space
		''' to the coordinate space of the view mapped to it.
		''' </summary>
		''' <param name="pos"> the position to convert &gt;= 0 </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <returns> the bounding box of the given position </returns>
		''' <exception cref="BadLocationException">  if the given position does not
		'''   represent a valid location in the associated document </exception>
		''' <seealso cref= View#modelToView </seealso>
		Public Overrides Function modelToView(ByVal pos As Integer, ByVal a As Shape, ByVal b As Position.Bias) As Shape
			Dim c As Container = container
			If TypeOf c Is javax.swing.JPasswordField Then
				Dim f As javax.swing.JPasswordField = CType(c, javax.swing.JPasswordField)
				If Not f.echoCharIsSet() Then Return MyBase.modelToView(pos, a, b)
				Dim echoChar As Char = f.echoChar
				Dim m As FontMetrics = f.getFontMetrics(f.font)

				Dim alloc As Rectangle = adjustAllocation(a).bounds
				Dim dx As Integer = (pos - startOffset) * m.charWidth(echoChar)
				alloc.x += dx
				alloc.width = 1
				Return alloc
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Provides a mapping from the view coordinate space to the logical
		''' coordinate space of the model.
		''' </summary>
		''' <param name="fx"> the X coordinate &gt;= 0.0f </param>
		''' <param name="fy"> the Y coordinate &gt;= 0.0f </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <returns> the location within the model that best represents the
		'''  given point in the view </returns>
		''' <seealso cref= View#viewToModel </seealso>
		Public Overrides Function viewToModel(ByVal fx As Single, ByVal fy As Single, ByVal a As Shape, ByVal bias As Position.Bias()) As Integer
			bias(0) = Position.Bias.Forward
			Dim n As Integer = 0
			Dim c As Container = container
			If TypeOf c Is javax.swing.JPasswordField Then
				Dim f As javax.swing.JPasswordField = CType(c, javax.swing.JPasswordField)
				If Not f.echoCharIsSet() Then Return MyBase.viewToModel(fx, fy, a, bias)
				Dim echoChar As Char = f.echoChar
				Dim charWidth As Integer = f.getFontMetrics(f.font).charWidth(echoChar)
				a = adjustAllocation(a)
				Dim alloc As Rectangle = If(TypeOf a Is Rectangle, CType(a, Rectangle), a.bounds)
				n = (If(charWidth > 0, (CInt(Fix(fx)) - alloc.x) / charWidth, Integer.MaxValue))
				If n < 0 Then
					n = 0
				ElseIf n > (startOffset + document.length) Then
					n = document.length - startOffset
				End If
			End If
			Return startOffset + n
		End Function

		''' <summary>
		''' Determines the preferred span for this view along an
		''' axis.
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <returns>   the span the view would like to be rendered into &gt;= 0.
		'''           Typically the view is told to render into the span
		'''           that is returned, although there is no guarantee.
		'''           The parent may choose to resize or break the view. </returns>
		Public Overrides Function getPreferredSpan(ByVal axis As Integer) As Single
			Select Case axis
			Case View.X_AXIS
				Dim c As Container = container
				If TypeOf c Is javax.swing.JPasswordField Then
					Dim f As javax.swing.JPasswordField = CType(c, javax.swing.JPasswordField)
					If f.echoCharIsSet() Then
						Dim echoChar As Char = f.echoChar
						Dim m As FontMetrics = f.getFontMetrics(f.font)
						Dim doc As Document = document
						Return m.charWidth(echoChar) * document.length
					End If
				End If
			End Select
			Return MyBase.getPreferredSpan(axis)
		End Function

		Friend Shared ONE As Char() = New Char(0){}
	End Class

End Namespace
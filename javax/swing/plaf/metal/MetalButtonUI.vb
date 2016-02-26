Imports javax.swing
Imports javax.swing.border
Imports javax.swing.plaf.basic
Imports javax.swing.plaf

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

Namespace javax.swing.plaf.metal


	''' <summary>
	''' MetalButtonUI implementation
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author Tom Santos
	''' </summary>
	Public Class MetalButtonUI
		Inherits BasicButtonUI

		' NOTE: These are not really needed, but at this point we can't pull
		' them. Their values are updated purely for historical reasons.
		Protected Friend focusColor As Color
		Protected Friend selectColor As Color
		Protected Friend disabledTextColor As Color

		Private Shared ReadOnly METAL_BUTTON_UI_KEY As New Object

		' ********************************
		'          Create PLAF
		' ********************************
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Dim appContext As sun.awt.AppContext = sun.awt.AppContext.appContext
			Dim ___metalButtonUI As MetalButtonUI = CType(appContext.get(METAL_BUTTON_UI_KEY), MetalButtonUI)
			If ___metalButtonUI Is Nothing Then
				___metalButtonUI = New MetalButtonUI
				appContext.put(METAL_BUTTON_UI_KEY, ___metalButtonUI)
			End If
			Return ___metalButtonUI
		End Function

		' ********************************
		'          Install
		' ********************************
		Public Overrides Sub installDefaults(ByVal b As AbstractButton)
			MyBase.installDefaults(b)
		End Sub

		Public Overrides Sub uninstallDefaults(ByVal b As AbstractButton)
			MyBase.uninstallDefaults(b)
		End Sub

		' ********************************
		'         Create Listeners
		' ********************************
		Protected Friend Overrides Function createButtonListener(ByVal b As AbstractButton) As BasicButtonListener
			Return MyBase.createButtonListener(b)
		End Function


		' ********************************
		'         Default Accessors
		' ********************************
		Protected Friend Overridable Property selectColor As Color
			Get
				selectColor = UIManager.getColor(propertyPrefix & "select")
				Return selectColor
			End Get
		End Property

		Protected Friend Overridable Property disabledTextColor As Color
			Get
				disabledTextColor = UIManager.getColor(propertyPrefix & "disabledText")
				Return disabledTextColor
			End Get
		End Property

		Protected Friend Overridable Property focusColor As Color
			Get
				focusColor = UIManager.getColor(propertyPrefix & "focus")
				Return focusColor
			End Get
		End Property

		' ********************************
		'          Paint
		' ********************************
		''' <summary>
		''' If necessary paints the background of the component, then
		''' invokes <code>paint</code>.
		''' </summary>
		''' <param name="g"> Graphics to paint to </param>
		''' <param name="c"> JComponent painting on </param>
		''' <exception cref="NullPointerException"> if <code>g</code> or <code>c</code> is
		'''         null </exception>
		''' <seealso cref= javax.swing.plaf.ComponentUI#update </seealso>
		''' <seealso cref= javax.swing.plaf.ComponentUI#paint
		''' @since 1.5 </seealso>
		Public Overridable Sub update(ByVal g As Graphics, ByVal c As JComponent)
			Dim button As AbstractButton = CType(c, AbstractButton)
			If (TypeOf c.background Is UIResource) AndAlso button.contentAreaFilled AndAlso c.enabled Then
				Dim model As ButtonModel = button.model
				If Not MetalUtils.isToolBarButton(c) Then
					If (Not model.armed) AndAlso (Not model.pressed) AndAlso MetalUtils.drawGradient(c, g, "Button.gradient", 0, 0, c.width, c.height, True) Then
						paint(g, c)
						Return
					End If
				ElseIf model.rollover AndAlso MetalUtils.drawGradient(c, g, "Button.gradient", 0, 0, c.width, c.height, True) Then
					paint(g, c)
					Return
				End If
			End If
			MyBase.update(g, c)
		End Sub

		Protected Friend Overrides Sub paintButtonPressed(ByVal g As Graphics, ByVal b As AbstractButton)
			If b.contentAreaFilled Then
				Dim size As Dimension = b.size
				g.color = selectColor
				g.fillRect(0, 0, size.width, size.height)
			End If
		End Sub

		Protected Friend Overrides Sub paintFocus(ByVal g As Graphics, ByVal b As AbstractButton, ByVal viewRect As Rectangle, ByVal textRect As Rectangle, ByVal iconRect As Rectangle)

			Dim focusRect As New Rectangle
			Dim text As String = b.text
			Dim isIcon As Boolean = b.icon IsNot Nothing

			' If there is text
			If text IsNot Nothing AndAlso (Not text.Equals("")) Then
				If Not isIcon Then
					focusRect.bounds = textRect
				Else
					focusRect.bounds = iconRect.union(textRect)
				End If
			' If there is an icon and no text
			ElseIf isIcon Then
				focusRect.bounds = iconRect
			End If

			g.color = focusColor
			g.drawRect((focusRect.x-1), (focusRect.y-1), focusRect.width+1, focusRect.height+1)

		End Sub


		Protected Friend Overrides Sub paintText(ByVal g As Graphics, ByVal c As JComponent, ByVal textRect As Rectangle, ByVal text As String)
			Dim b As AbstractButton = CType(c, AbstractButton)
			Dim model As ButtonModel = b.model
			Dim fm As FontMetrics = sun.swing.SwingUtilities2.getFontMetrics(c, g)
			Dim mnemIndex As Integer = b.displayedMnemonicIndex

			' Draw the Text 
			If model.enabled Then
				''' <summary>
				'''* paint the text normally </summary>
				g.color = b.foreground
			Else
				''' <summary>
				'''* paint the text disabled ** </summary>
				g.color = disabledTextColor
			End If
			sun.swing.SwingUtilities2.drawStringUnderlineCharAt(c, g,text,mnemIndex, textRect.x, textRect.y + fm.ascent)
		End Sub
	End Class

End Namespace
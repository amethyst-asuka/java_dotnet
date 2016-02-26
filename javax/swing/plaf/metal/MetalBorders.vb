Imports Microsoft.VisualBasic
Imports javax.swing
Imports javax.swing.border
Imports javax.swing.plaf

'
' * Copyright (c) 1998, 2015, Oracle and/or its affiliates. All rights reserved.
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
	''' Factory object that can vend Borders appropriate for the metal L &amp; F.
	''' @author Steve Wilson
	''' </summary>

	Public Class MetalBorders

		''' <summary>
		''' Client property indicating the button shouldn't provide a rollover
		''' indicator. Only used with the Ocean theme.
		''' </summary>
		Friend Shared NO_BUTTON_ROLLOVER As Object = New sun.swing.StringUIClientPropertyKey("NoButtonRollover")


		Public Class Flush3DBorder
			Inherits AbstractBorder
			Implements UIResource

			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				If c.enabled Then
					MetalUtils.drawFlush3DBorder(g, x, y, w, h)
				Else
					MetalUtils.drawDisabledBorder(g, x, y, w, h)
				End If
			End Sub

			Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal newInsets As java.awt.Insets) As java.awt.Insets
				newInsets.set(2, 2, 2, 2)
				Return newInsets
			End Function
		End Class

		Public Class ButtonBorder
			Inherits AbstractBorder
			Implements UIResource

			Protected Friend Shared borderInsets As New java.awt.Insets(3, 3, 3, 3)

			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				If Not(TypeOf c Is AbstractButton) Then Return
				If MetalLookAndFeel.usingOcean() Then
					paintOceanBorder(c, g, x, y, w, h)
					Return
				End If
				Dim button As AbstractButton = CType(c, AbstractButton)
				Dim model As ButtonModel = button.model

				If model.enabled Then
					Dim isPressed As Boolean = model.pressed AndAlso model.armed
					Dim isDefault As Boolean = (TypeOf button Is JButton AndAlso CType(button, JButton).defaultButton)

					If isPressed AndAlso isDefault Then
						MetalUtils.drawDefaultButtonPressedBorder(g, x, y, w, h)
					ElseIf isPressed Then
						MetalUtils.drawPressed3DBorder(g, x, y, w, h)
					ElseIf isDefault Then
						MetalUtils.drawDefaultButtonBorder(g, x, y, w, h, False)
					Else
						MetalUtils.drawButtonBorder(g, x, y, w, h, False)
					End If ' disabled state
				Else
					MetalUtils.drawDisabledBorder(g, x, y, w-1, h-1)
				End If
			End Sub

			Private Sub paintOceanBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				Dim button As AbstractButton = CType(c, AbstractButton)
				Dim model As ButtonModel = CType(c, AbstractButton).model

				g.translate(x, y)
				If MetalUtils.isToolBarButton(button) Then
					If model.enabled Then
						If model.pressed Then
							g.color = MetalLookAndFeel.white
							g.fillRect(1, h - 1, w - 1, 1)
							g.fillRect(w - 1, 1, 1, h - 1)
							g.color = MetalLookAndFeel.controlDarkShadow
							g.drawRect(0, 0, w - 2, h - 2)
							g.fillRect(1, 1, w - 3, 1)
						ElseIf model.selected OrElse model.rollover Then
							g.color = MetalLookAndFeel.white
							g.fillRect(1, h - 1, w - 1, 1)
							g.fillRect(w - 1, 1, 1, h - 1)
							g.color = MetalLookAndFeel.controlDarkShadow
							g.drawRect(0, 0, w - 2, h - 2)
						Else
							g.color = MetalLookAndFeel.white
							g.drawRect(1, 1, w - 2, h - 2)
							g.color = UIManager.getColor("Button.toolBarBorderBackground")
							g.drawRect(0, 0, w - 2, h - 2)
						End If
					Else
					   g.color = UIManager.getColor("Button.disabledToolBarBorderBackground")
					   g.drawRect(0, 0, w - 2, h - 2)
					End If
				ElseIf model.enabled Then
					Dim pressed As Boolean = model.pressed
					Dim armed As Boolean = model.armed

					If (TypeOf c Is JButton) AndAlso CType(c, JButton).defaultButton Then
						g.color = MetalLookAndFeel.controlDarkShadow
						g.drawRect(0, 0, w - 1, h - 1)
						g.drawRect(1, 1, w - 3, h - 3)
					ElseIf pressed Then
						g.color = MetalLookAndFeel.controlDarkShadow
						g.fillRect(0, 0, w, 2)
						g.fillRect(0, 2, 2, h - 2)
						g.fillRect(w - 1, 1, 1, h - 1)
						g.fillRect(1, h - 1, w - 2, 1)
					ElseIf model.rollover AndAlso button.getClientProperty(NO_BUTTON_ROLLOVER) Is Nothing Then
						g.color = MetalLookAndFeel.primaryControl
						g.drawRect(0, 0, w - 1, h - 1)
						g.drawRect(2, 2, w - 5, h - 5)
						g.color = MetalLookAndFeel.controlDarkShadow
						g.drawRect(1, 1, w - 3, h - 3)
					Else
						g.color = MetalLookAndFeel.controlDarkShadow
						g.drawRect(0, 0, w - 1, h - 1)
					End If
				Else
					g.color = MetalLookAndFeel.inactiveControlTextColor
					g.drawRect(0, 0, w - 1, h - 1)
					If (TypeOf c Is JButton) AndAlso CType(c, JButton).defaultButton Then g.drawRect(1, 1, w - 3, h - 3)
				End If
			End Sub

			Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal newInsets As java.awt.Insets) As java.awt.Insets
				newInsets.set(3, 3, 3, 3)
				Return newInsets
			End Function
		End Class

		Public Class InternalFrameBorder
			Inherits AbstractBorder
			Implements UIResource

			Private Const corner As Integer = 14

			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)

				Dim background As java.awt.Color
				Dim highlight As java.awt.Color
				Dim shadow As java.awt.Color

				If TypeOf c Is JInternalFrame AndAlso CType(c, JInternalFrame).selected Then
					background = MetalLookAndFeel.primaryControlDarkShadow
					highlight = MetalLookAndFeel.primaryControlShadow
					shadow = MetalLookAndFeel.primaryControlInfo
				Else
					background = MetalLookAndFeel.controlDarkShadow
					highlight = MetalLookAndFeel.controlShadow
					shadow = MetalLookAndFeel.controlInfo
				End If

				  g.color = background
				  ' Draw outermost lines
				  g.drawLine(1, 0, w-2, 0)
				  g.drawLine(0, 1, 0, h-2)
				  g.drawLine(w-1, 1, w-1, h-2)
				  g.drawLine(1, h-1, w-2, h-1)

				  ' Draw the bulk of the border
				  For i As Integer = 1 To 4
					  g.drawRect(x+i,y+i,w-(i*2)-1, h-(i*2)-1)
				  Next i

				  If TypeOf c Is JInternalFrame AndAlso CType(c, JInternalFrame).resizable Then
					  g.color = highlight
					  ' Draw the Long highlight lines
					  g.drawLine(corner+1, 3, w-corner, 3)
					  g.drawLine(3, corner+1, 3, h-corner)
					  g.drawLine(w-2, corner+1, w-2, h-corner)
					  g.drawLine(corner+1, h-2, w-corner, h-2)

					  g.color = shadow
					  ' Draw the Long shadow lines
					  g.drawLine(corner, 2, w-corner-1, 2)
					  g.drawLine(2, corner, 2, h-corner-1)
					  g.drawLine(w-3, corner, w-3, h-corner-1)
					  g.drawLine(corner, h-3, w-corner-1, h-3)
				  End If

			End Sub

			  Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal newInsets As java.awt.Insets) As java.awt.Insets
				  newInsets.set(5, 5, 5, 5)
				  Return newInsets
			  End Function
		End Class

		''' <summary>
		''' Border for a Frame.
		''' @since 1.4
		''' </summary>
		Friend Class FrameBorder
			Inherits AbstractBorder
			Implements UIResource

			Private Const corner As Integer = 14

			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)

				Dim background As java.awt.Color
				Dim highlight As java.awt.Color
				Dim shadow As java.awt.Color

				Dim window As java.awt.Window = SwingUtilities.getWindowAncestor(c)
				If window IsNot Nothing AndAlso window.active Then
					background = MetalLookAndFeel.primaryControlDarkShadow
					highlight = MetalLookAndFeel.primaryControlShadow
					shadow = MetalLookAndFeel.primaryControlInfo
				Else
					background = MetalLookAndFeel.controlDarkShadow
					highlight = MetalLookAndFeel.controlShadow
					shadow = MetalLookAndFeel.controlInfo
				End If

				g.color = background
				' Draw outermost lines
				g.drawLine(x+1, y+0, x+w-2, y+0)
				g.drawLine(x+0, y+1, x+0, y +h-2)
				g.drawLine(x+w-1, y+1, x+w-1, y+h-2)
				g.drawLine(x+1, y+h-1, x+w-2, y+h-1)

				' Draw the bulk of the border
				For i As Integer = 1 To 4
					g.drawRect(x+i,y+i,w-(i*2)-1, h-(i*2)-1)
				Next i

				If (TypeOf window Is java.awt.Frame) AndAlso CType(window, java.awt.Frame).resizable Then
					g.color = highlight
					' Draw the Long highlight lines
					g.drawLine(corner+1, 3, w-corner, 3)
					g.drawLine(3, corner+1, 3, h-corner)
					g.drawLine(w-2, corner+1, w-2, h-corner)
					g.drawLine(corner+1, h-2, w-corner, h-2)

					g.color = shadow
					' Draw the Long shadow lines
					g.drawLine(corner, 2, w-corner-1, 2)
					g.drawLine(2, corner, 2, h-corner-1)
					g.drawLine(w-3, corner, w-3, h-corner-1)
					g.drawLine(corner, h-3, w-corner-1, h-3)
				End If

			End Sub

			Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal newInsets As java.awt.Insets) As java.awt.Insets
				newInsets.set(5, 5, 5, 5)
				Return newInsets
			End Function
		End Class

		''' <summary>
		''' Border for a Frame.
		''' @since 1.4
		''' </summary>
		Friend Class DialogBorder
			Inherits AbstractBorder
			Implements UIResource

			Private Const corner As Integer = 14

			Protected Friend Overridable Property activeBackground As java.awt.Color
				Get
					Return MetalLookAndFeel.primaryControlDarkShadow
				End Get
			End Property

			Protected Friend Overridable Property activeHighlight As java.awt.Color
				Get
					Return MetalLookAndFeel.primaryControlShadow
				End Get
			End Property

			Protected Friend Overridable Property activeShadow As java.awt.Color
				Get
					Return MetalLookAndFeel.primaryControlInfo
				End Get
			End Property

			Protected Friend Overridable Property inactiveBackground As java.awt.Color
				Get
					Return MetalLookAndFeel.controlDarkShadow
				End Get
			End Property

			Protected Friend Overridable Property inactiveHighlight As java.awt.Color
				Get
					Return MetalLookAndFeel.controlShadow
				End Get
			End Property

			Protected Friend Overridable Property inactiveShadow As java.awt.Color
				Get
					Return MetalLookAndFeel.controlInfo
				End Get
			End Property

			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				Dim background As java.awt.Color
				Dim highlight As java.awt.Color
				Dim shadow As java.awt.Color

				Dim window As java.awt.Window = SwingUtilities.getWindowAncestor(c)
				If window IsNot Nothing AndAlso window.active Then
					background = activeBackground
					highlight = activeHighlight
					shadow = activeShadow
				Else
					background = inactiveBackground
					highlight = inactiveHighlight
					shadow = inactiveShadow
				End If

				g.color = background
				' Draw outermost lines
				g.drawLine(x + 1, y + 0, x + w-2, y + 0)
				g.drawLine(x + 0, y + 1, x + 0, y + h - 2)
				g.drawLine(x + w - 1, y + 1, x + w - 1, y + h - 2)
				g.drawLine(x + 1, y + h - 1, x + w - 2, y + h - 1)

				' Draw the bulk of the border
				For i As Integer = 1 To 4
					g.drawRect(x+i,y+i,w-(i*2)-1, h-(i*2)-1)
				Next i


				If (TypeOf window Is java.awt.Dialog) AndAlso CType(window, java.awt.Dialog).resizable Then
					g.color = highlight
					' Draw the Long highlight lines
					g.drawLine(corner+1, 3, w-corner, 3)
					g.drawLine(3, corner+1, 3, h-corner)
					g.drawLine(w-2, corner+1, w-2, h-corner)
					g.drawLine(corner+1, h-2, w-corner, h-2)

					g.color = shadow
					' Draw the Long shadow lines
					g.drawLine(corner, 2, w-corner-1, 2)
					g.drawLine(2, corner, 2, h-corner-1)
					g.drawLine(w-3, corner, w-3, h-corner-1)
					g.drawLine(corner, h-3, w-corner-1, h-3)
				End If

			End Sub

			Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal newInsets As java.awt.Insets) As java.awt.Insets
				newInsets.set(5, 5, 5, 5)
				Return newInsets
			End Function
		End Class

		''' <summary>
		''' Border for an Error Dialog.
		''' @since 1.4
		''' </summary>
		Friend Class ErrorDialogBorder
			Inherits DialogBorder
			Implements UIResource

			Protected Friend Property Overrides activeBackground As java.awt.Color
				Get
					Return UIManager.getColor("OptionPane.errorDialog.border.background")
				End Get
			End Property
		End Class


		''' <summary>
		''' Border for a QuestionDialog.  Also used for a JFileChooser and a
		''' JColorChooser..
		''' @since 1.4
		''' </summary>
		Friend Class QuestionDialogBorder
			Inherits DialogBorder
			Implements UIResource

			Protected Friend Property Overrides activeBackground As java.awt.Color
				Get
					Return UIManager.getColor("OptionPane.questionDialog.border.background")
				End Get
			End Property
		End Class


		''' <summary>
		''' Border for a Warning Dialog.
		''' @since 1.4
		''' </summary>
		Friend Class WarningDialogBorder
			Inherits DialogBorder
			Implements UIResource

			Protected Friend Property Overrides activeBackground As java.awt.Color
				Get
					Return UIManager.getColor("OptionPane.warningDialog.border.background")
				End Get
			End Property
		End Class


		''' <summary>
		''' Border for a Palette.
		''' @since 1.3
		''' </summary>
		Public Class PaletteBorder
			Inherits AbstractBorder
			Implements UIResource

			Friend titleHeight As Integer = 0

			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)

				g.translate(x,y)
				g.color = MetalLookAndFeel.primaryControlDarkShadow
				g.drawLine(0, 1, 0, h-2)
				g.drawLine(1, h-1, w-2, h-1)
				g.drawLine(w-1, 1, w-1, h-2)
				g.drawLine(1, 0, w-2, 0)
				g.drawRect(1,1, w-3, h-3)
				g.translate(-x,-y)

			End Sub

			Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal newInsets As java.awt.Insets) As java.awt.Insets
				newInsets.set(1, 1, 1, 1)
				Return newInsets
			End Function
		End Class

		Public Class OptionDialogBorder
			Inherits AbstractBorder
			Implements UIResource

			Friend titleHeight As Integer = 0

			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)

				g.translate(x,y)

				Dim messageType As Integer = JOptionPane.PLAIN_MESSAGE
				If TypeOf c Is JInternalFrame Then
					Dim obj As Object = CType(c, JInternalFrame).getClientProperty("JInternalFrame.messageType")
					If TypeOf obj Is Integer? Then messageType = CInt(Fix(obj))
				End If

				Dim borderColor As java.awt.Color

				Select Case messageType
				Case(JOptionPane.ERROR_MESSAGE)
					borderColor = UIManager.getColor("OptionPane.errorDialog.border.background")
				Case(JOptionPane.QUESTION_MESSAGE)
					borderColor = UIManager.getColor("OptionPane.questionDialog.border.background")
				Case(JOptionPane.WARNING_MESSAGE)
					borderColor = UIManager.getColor("OptionPane.warningDialog.border.background")
				Case Else
					borderColor = MetalLookAndFeel.primaryControlDarkShadow
				End Select

				g.color = borderColor

				  ' Draw outermost lines
				  g.drawLine(1, 0, w-2, 0)
				  g.drawLine(0, 1, 0, h-2)
				  g.drawLine(w-1, 1, w-1, h-2)
				  g.drawLine(1, h-1, w-2, h-1)

				  ' Draw the bulk of the border
				  For i As Integer = 1 To 2
					  g.drawRect(i, i, w-(i*2)-1, h-(i*2)-1)
				  Next i

				g.translate(-x,-y)

			End Sub

			Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal newInsets As java.awt.Insets) As java.awt.Insets
				newInsets.set(3, 3, 3, 3)
				Return newInsets
			End Function
		End Class


		Public Class MenuBarBorder
			Inherits AbstractBorder
			Implements UIResource

			Protected Friend Shared borderInsets As New java.awt.Insets(1, 0, 1, 0)

			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				g.translate(x, y)

				If MetalLookAndFeel.usingOcean() Then
					' Only paint a border if we're not next to a horizontal toolbar
					If TypeOf c Is JMenuBar AndAlso (Not MetalToolBarUI.doesMenuBarBorderToolBar(CType(c, JMenuBar))) Then
						g.color = MetalLookAndFeel.control
						sun.swing.SwingUtilities2.drawHLine(g, 0, w - 1, h - 2)
						g.color = UIManager.getColor("MenuBar.borderColor")
						sun.swing.SwingUtilities2.drawHLine(g, 0, w - 1, h - 1)
					End If
				Else
					g.color = MetalLookAndFeel.controlShadow
					sun.swing.SwingUtilities2.drawHLine(g, 0, w - 1, h - 1)
				End If
				g.translate(-x, -y)
			End Sub

			Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal newInsets As java.awt.Insets) As java.awt.Insets
				If MetalLookAndFeel.usingOcean() Then
					newInsets.set(0, 0, 2, 0)
				Else
					newInsets.set(1, 0, 1, 0)
				End If
				Return newInsets
			End Function
		End Class

		Public Class MenuItemBorder
			Inherits AbstractBorder
			Implements UIResource

			Protected Friend Shared borderInsets As New java.awt.Insets(2, 2, 2, 2)

			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				If Not(TypeOf c Is JMenuItem) Then Return
				Dim b As JMenuItem = CType(c, JMenuItem)
				Dim model As ButtonModel = b.model

				g.translate(x, y)

				If TypeOf c.parent Is JMenuBar Then
					If model.armed OrElse model.selected Then
						g.color = MetalLookAndFeel.controlDarkShadow
						g.drawLine(0, 0, w - 2, 0)
						g.drawLine(0, 0, 0, h - 1)
						g.drawLine(w - 2, 2, w - 2, h - 1)

						g.color = MetalLookAndFeel.primaryControlHighlight
						g.drawLine(w - 1, 1, w - 1, h - 1)

						g.color = MetalLookAndFeel.menuBackground
						g.drawLine(w - 1, 0, w - 1, 0)
					End If
				Else
					If model.armed OrElse (TypeOf c Is JMenu AndAlso model.selected) Then
						g.color = MetalLookAndFeel.primaryControlDarkShadow
						g.drawLine(0, 0, w - 1, 0)

						g.color = MetalLookAndFeel.primaryControlHighlight
						g.drawLine(0, h - 1, w - 1, h - 1)
					Else
						g.color = MetalLookAndFeel.primaryControlHighlight
						g.drawLine(0, 0, 0, h - 1)
					End If
				End If

				g.translate(-x, -y)
			End Sub

			Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal newInsets As java.awt.Insets) As java.awt.Insets
				newInsets.set(2, 2, 2, 2)
				Return newInsets
			End Function
		End Class

		Public Class PopupMenuBorder
			Inherits AbstractBorder
			Implements UIResource

			Protected Friend Shared borderInsets As New java.awt.Insets(3, 1, 2, 1)

			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				g.translate(x, y)

				g.color = MetalLookAndFeel.primaryControlDarkShadow
				g.drawRect(0, 0, w - 1, h - 1)

				g.color = MetalLookAndFeel.primaryControlHighlight
				g.drawLine(1, 1, w - 2, 1)
				g.drawLine(1, 2, 1, 2)
				g.drawLine(1, h - 2, 1, h - 2)

				g.translate(-x, -y)

			End Sub

			Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal newInsets As java.awt.Insets) As java.awt.Insets
				newInsets.set(3, 1, 2, 1)
				Return newInsets
			End Function
		End Class


		Public Class RolloverButtonBorder
			Inherits ButtonBorder

			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				Dim b As AbstractButton = CType(c, AbstractButton)
				Dim model As ButtonModel = b.model

				If model.rollover AndAlso Not(model.pressed AndAlso (Not model.armed)) Then MyBase.paintBorder(c, g, x, y, w, h)
			End Sub

		End Class

		''' <summary>
		''' A border which is like a Margin border but it will only honor the margin
		''' if the margin has been explicitly set by the developer.
		''' 
		''' Note: This is identical to the package private class
		''' BasicBorders.RolloverMarginBorder and should probably be consolidated.
		''' </summary>
		Friend Class RolloverMarginBorder
			Inherits EmptyBorder

			Public Sub New()
				MyBase.New(3,3,3,3) ' hardcoded margin for JLF requirements.
			End Sub

			Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal insets As java.awt.Insets) As java.awt.Insets
				Dim margin As java.awt.Insets = Nothing

				If TypeOf c Is AbstractButton Then margin = CType(c, AbstractButton).margin
				If margin Is Nothing OrElse TypeOf margin Is UIResource Then
					' default margin so replace
					insets.left = left
					insets.top = top
					insets.right = right
					insets.bottom = bottom
				Else
					' Margin which has been explicitly set by the user.
					insets.left = margin.left
					insets.top = margin.top
					insets.right = margin.right
					insets.bottom = margin.bottom
				End If
				Return insets
			End Function
		End Class

		Public Class ToolBarBorder
			Inherits AbstractBorder
			Implements UIResource, SwingConstants

			Protected Friend bumps As New MetalBumps(10, 10, MetalLookAndFeel.controlHighlight, MetalLookAndFeel.controlDarkShadow, UIManager.getColor("ToolBar.background"))

			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				If Not(TypeOf c Is JToolBar) Then Return
				g.translate(x, y)

				If CType(c, JToolBar).floatable Then
					If CType(c, JToolBar).orientation = HORIZONTAL Then
						Dim shift As Integer = If(MetalLookAndFeel.usingOcean(), -1, 0)
						bumps.bumpArearea(10, h - 4)
						If MetalUtils.isLeftToRight(c) Then
							bumps.paintIcon(c, g, 2, 2 + shift)
						Else
							bumps.paintIcon(c, g, w-12, 2 + shift)
						End If
					Else ' vertical
						bumps.bumpArearea(w - 4, 10)
						bumps.paintIcon(c, g, 2, 2)
					End If

				End If

				If CType(c, JToolBar).orientation = HORIZONTAL AndAlso MetalLookAndFeel.usingOcean() Then
					g.color = MetalLookAndFeel.control
					g.drawLine(0, h - 2, w, h - 2)
					g.color = UIManager.getColor("ToolBar.borderColor")
					g.drawLine(0, h - 1, w, h - 1)
				End If

				g.translate(-x, -y)
			End Sub

			Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal newInsets As java.awt.Insets) As java.awt.Insets
				If MetalLookAndFeel.usingOcean() Then
					newInsets.set(1, 2, 3, 2)
				Else
						newInsets.right = 2
							newInsets.bottom = newInsets.right
								newInsets.left = newInsets.bottom
								newInsets.top = newInsets.left
				End If

				If Not(TypeOf c Is JToolBar) Then Return newInsets
				If CType(c, JToolBar).floatable Then
					If CType(c, JToolBar).orientation = HORIZONTAL Then
						If c.componentOrientation.leftToRight Then
							newInsets.left = 16
						Else
							newInsets.right = 16
						End If ' vertical
					Else
						newInsets.top = 16
					End If
				End If

				Dim margin As java.awt.Insets = CType(c, JToolBar).margin

				If margin IsNot Nothing Then
					newInsets.left += margin.left
					newInsets.top += margin.top
					newInsets.right += margin.right
					newInsets.bottom += margin.bottom
				End If

				Return newInsets
			End Function
		End Class

		Private Shared ___buttonBorder As Border

		''' <summary>
		''' Returns a border instance for a JButton
		''' @since 1.3
		''' </summary>
		Public Property Shared buttonBorder As Border
			Get
				If ___buttonBorder Is Nothing Then ___buttonBorder = New BorderUIResource.CompoundBorderUIResource(New MetalBorders.ButtonBorder, New javax.swing.plaf.basic.BasicBorders.MarginBorder)
				Return ___buttonBorder
			End Get
		End Property

		Private Shared textBorder As Border

		''' <summary>
		''' Returns a border instance for a text component
		''' @since 1.3
		''' </summary>
		Public Property Shared textBorder As Border
			Get
				If textBorder Is Nothing Then textBorder = New BorderUIResource.CompoundBorderUIResource(New MetalBorders.Flush3DBorder, New javax.swing.plaf.basic.BasicBorders.MarginBorder)
				Return textBorder
			End Get
		End Property

		Private Shared textFieldBorder As Border

		''' <summary>
		''' Returns a border instance for a JTextField
		''' @since 1.3
		''' </summary>
		Public Property Shared textFieldBorder As Border
			Get
				If textFieldBorder Is Nothing Then textFieldBorder = New BorderUIResource.CompoundBorderUIResource(New MetalBorders.TextFieldBorder, New javax.swing.plaf.basic.BasicBorders.MarginBorder)
				Return textFieldBorder
			End Get
		End Property

		Public Class TextFieldBorder
			Inherits Flush3DBorder

			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)

			  If Not(TypeOf c Is javax.swing.text.JTextComponent) Then
					' special case for non-text components (bug ID 4144840)
					If c.enabled Then
						MetalUtils.drawFlush3DBorder(g, x, y, w, h)
					Else
						MetalUtils.drawDisabledBorder(g, x, y, w, h)
					End If
					Return
			  End If

				If c.enabled AndAlso CType(c, javax.swing.text.JTextComponent).editable Then
					MetalUtils.drawFlush3DBorder(g, x, y, w, h)
				Else
					MetalUtils.drawDisabledBorder(g, x, y, w, h)
				End If

			End Sub
		End Class

		Public Class ScrollPaneBorder
			Inherits AbstractBorder
			Implements UIResource

			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)

				If Not(TypeOf c Is JScrollPane) Then Return
				Dim scroll As JScrollPane = CType(c, JScrollPane)
				Dim colHeader As JComponent = scroll.columnHeader
				Dim colHeaderHeight As Integer = 0
				If colHeader IsNot Nothing Then colHeaderHeight = colHeader.height

				Dim rowHeader As JComponent = scroll.rowHeader
				Dim rowHeaderWidth As Integer = 0
				If rowHeader IsNot Nothing Then rowHeaderWidth = rowHeader.width


				g.translate(x, y)

				g.color = MetalLookAndFeel.controlDarkShadow
				g.drawRect(0, 0, w-2, h-2)
				g.color = MetalLookAndFeel.controlHighlight

				g.drawLine(w-1, 1, w-1, h-1)
				g.drawLine(1, h-1, w-1, h-1)

				g.color = MetalLookAndFeel.control
				g.drawLine(w-2, 2+colHeaderHeight, w-2, 2+colHeaderHeight)
				g.drawLine(1+rowHeaderWidth, h-2, 1+rowHeaderWidth, h-2)

				g.translate(-x, -y)

			End Sub

			Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal insets As java.awt.Insets) As java.awt.Insets
				insets.set(1, 1, 2, 2)
				Return insets
			End Function
		End Class

		Private Shared toggleButtonBorder As Border

		''' <summary>
		''' Returns a border instance for a JToggleButton
		''' @since 1.3
		''' </summary>
		Public Property Shared toggleButtonBorder As Border
			Get
				If toggleButtonBorder Is Nothing Then toggleButtonBorder = New BorderUIResource.CompoundBorderUIResource(New MetalBorders.ToggleButtonBorder, New javax.swing.plaf.basic.BasicBorders.MarginBorder)
				Return toggleButtonBorder
			End Get
		End Property

		''' <summary>
		''' @since 1.3
		''' </summary>
		Public Class ToggleButtonBorder
			Inherits ButtonBorder

			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				Dim button As AbstractButton = CType(c, AbstractButton)
				Dim model As ButtonModel = button.model
				If MetalLookAndFeel.usingOcean() Then
					If model.armed OrElse (Not button.enabled) Then
						MyBase.paintBorder(c, g, x, y, w, h)
					Else
					 g.color = MetalLookAndFeel.controlDarkShadow
					 g.drawRect(0, 0, w - 1, h - 1)
					End If
				Return
				End If
				If Not c.enabled Then
					MetalUtils.drawDisabledBorder(g, x, y, w-1, h-1)
				Else
					If model.pressed AndAlso model.armed Then
					   MetalUtils.drawPressed3DBorder(g, x, y, w, h)
					ElseIf model.selected Then
						MetalUtils.drawDark3DBorder(g, x, y, w, h)
					Else
						MetalUtils.drawFlush3DBorder(g, x, y, w, h)
					End If
				End If
			End Sub
		End Class

		''' <summary>
		''' Border for a Table Header
		''' @since 1.3
		''' </summary>
		Public Class TableHeaderBorder
			Inherits javax.swing.border.AbstractBorder

			Protected Friend editorBorderInsets As New java.awt.Insets(2, 2, 2, 0)

			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				g.translate(x, y)

				g.color = MetalLookAndFeel.controlDarkShadow
				g.drawLine(w-1, 0, w-1, h-1)
				g.drawLine(1, h-1, w-1, h-1)
				g.color = MetalLookAndFeel.controlHighlight
				g.drawLine(0, 0, w-2, 0)
				g.drawLine(0, 0, 0, h-2)

				g.translate(-x, -y)
			End Sub

			Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal insets As java.awt.Insets) As java.awt.Insets
				insets.set(2, 2, 2, 0)
				Return insets
			End Function
		End Class

		''' <summary>
		''' Returns a border instance for a Desktop Icon
		''' @since 1.3
		''' </summary>
		Public Property Shared desktopIconBorder As Border
			Get
				Return New BorderUIResource.CompoundBorderUIResource(New LineBorder(MetalLookAndFeel.controlDarkShadow, 1), New MatteBorder(2,2,1,2, MetalLookAndFeel.control))
			End Get
		End Property

		Friend Property Shared toolBarRolloverBorder As Border
			Get
				If MetalLookAndFeel.usingOcean() Then Return New CompoundBorder(New MetalBorders.ButtonBorder, New MetalBorders.RolloverMarginBorder)
				Return New CompoundBorder(New MetalBorders.RolloverButtonBorder, New MetalBorders.RolloverMarginBorder)
			End Get
		End Property

		Friend Property Shared toolBarNonrolloverBorder As Border
			Get
				If MetalLookAndFeel.usingOcean() Then Dim TempCompoundBorder As CompoundBorder = New CompoundBorder(New MetalBorders.ButtonBorder, New MetalBorders.RolloverMarginBorder)
				Return New CompoundBorder(New MetalBorders.ButtonBorder, New MetalBorders.RolloverMarginBorder)
			End Get
		End Property
	End Class

End Namespace
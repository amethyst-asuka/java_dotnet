Imports javax.swing
Imports javax.swing.border
Imports javax.swing.plaf

'
' * Copyright (c) 1997, 2015, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf.basic




	''' <summary>
	''' Factory object that can vend Borders appropriate for the basic L &amp; F.
	''' @author Georges Saab
	''' @author Amy Fowler
	''' </summary>

	Public Class BasicBorders

		Public Property Shared buttonBorder As Border
			Get
				Dim table As UIDefaults = UIManager.lookAndFeelDefaults
				Dim ___buttonBorder As Border = New BorderUIResource.CompoundBorderUIResource(New BasicBorders.ButtonBorder(table.getColor("Button.shadow"), table.getColor("Button.darkShadow"), table.getColor("Button.light"), table.getColor("Button.highlight")), New MarginBorder)
				Return ___buttonBorder
			End Get
		End Property

		Public Property Shared radioButtonBorder As Border
			Get
				Dim table As UIDefaults = UIManager.lookAndFeelDefaults
				Dim ___radioButtonBorder As Border = New BorderUIResource.CompoundBorderUIResource(New BasicBorders.RadioButtonBorder(table.getColor("RadioButton.shadow"), table.getColor("RadioButton.darkShadow"), table.getColor("RadioButton.light"), table.getColor("RadioButton.highlight")), New MarginBorder)
				Return ___radioButtonBorder
			End Get
		End Property

		Public Property Shared toggleButtonBorder As Border
			Get
				Dim table As UIDefaults = UIManager.lookAndFeelDefaults
				Dim ___toggleButtonBorder As Border = New BorderUIResource.CompoundBorderUIResource(New BasicBorders.ToggleButtonBorder(table.getColor("ToggleButton.shadow"), table.getColor("ToggleButton.darkShadow"), table.getColor("ToggleButton.light"), table.getColor("ToggleButton.highlight")), New MarginBorder)
				Return ___toggleButtonBorder
			End Get
		End Property

		Public Property Shared menuBarBorder As Border
			Get
				Dim table As UIDefaults = UIManager.lookAndFeelDefaults
				Dim ___menuBarBorder As Border = New BasicBorders.MenuBarBorder(table.getColor("MenuBar.shadow"), table.getColor("MenuBar.highlight"))
				Return ___menuBarBorder
			End Get
		End Property

		Public Property Shared splitPaneBorder As Border
			Get
				Dim table As UIDefaults = UIManager.lookAndFeelDefaults
				Dim ___splitPaneBorder As Border = New BasicBorders.SplitPaneBorder(table.getColor("SplitPane.highlight"), table.getColor("SplitPane.darkShadow"))
				Return ___splitPaneBorder
			End Get
		End Property

		''' <summary>
		''' Returns a border instance for a JSplitPane divider
		''' @since 1.3
		''' </summary>
		Public Property Shared splitPaneDividerBorder As Border
			Get
				Dim table As UIDefaults = UIManager.lookAndFeelDefaults
				Dim ___splitPaneBorder As Border = New BasicBorders.SplitPaneDividerBorder(table.getColor("SplitPane.highlight"), table.getColor("SplitPane.darkShadow"))
				Return ___splitPaneBorder
			End Get
		End Property

		Public Property Shared textFieldBorder As Border
			Get
				Dim table As UIDefaults = UIManager.lookAndFeelDefaults
				Dim ___textFieldBorder As Border = New BasicBorders.FieldBorder(table.getColor("TextField.shadow"), table.getColor("TextField.darkShadow"), table.getColor("TextField.light"), table.getColor("TextField.highlight"))
				Return ___textFieldBorder
			End Get
		End Property

		Public Property Shared progressBarBorder As Border
			Get
				Dim table As UIDefaults = UIManager.lookAndFeelDefaults
				Dim ___progressBarBorder As Border = New BorderUIResource.LineBorderUIResource(java.awt.Color.green, 2)
				Return ___progressBarBorder
			End Get
		End Property

		Public Property Shared internalFrameBorder As Border
			Get
				Dim table As UIDefaults = UIManager.lookAndFeelDefaults
				Dim ___internalFrameBorder As Border = New BorderUIResource.CompoundBorderUIResource(New BevelBorder(BevelBorder.RAISED, table.getColor("InternalFrame.borderLight"), table.getColor("InternalFrame.borderHighlight"), table.getColor("InternalFrame.borderDarkShadow"), table.getColor("InternalFrame.borderShadow")), BorderFactory.createLineBorder(table.getColor("InternalFrame.borderColor"), 1))
    
				Return ___internalFrameBorder
			End Get
		End Property

		''' <summary>
		''' Special thin border for rollover toolbar buttons.
		''' @since 1.4
		''' </summary>
		Public Class RolloverButtonBorder
			Inherits ButtonBorder

			Public Sub New(ByVal shadow As java.awt.Color, ByVal darkShadow As java.awt.Color, ByVal highlight As java.awt.Color, ByVal lightHighlight As java.awt.Color)
				MyBase.New(shadow, darkShadow, highlight, lightHighlight)
			End Sub

			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				Dim b As AbstractButton = CType(c, AbstractButton)
				Dim model As ButtonModel = b.model

				Dim shade As java.awt.Color = shadow
				Dim p As java.awt.Component = b.parent
				If p IsNot Nothing AndAlso p.background.Equals(shadow) Then shade = darkShadow

				If (model.rollover AndAlso Not(model.pressed AndAlso (Not model.armed))) OrElse model.selected Then

					Dim oldColor As java.awt.Color = g.color
					g.translate(x, y)

					If model.pressed AndAlso model.armed OrElse model.selected Then
						' Draw the pressd button
						g.color = shade
						g.drawRect(0, 0, w-1, h-1)
						g.color = lightHighlight
						g.drawLine(w-1, 0, w-1, h-1)
						g.drawLine(0, h-1, w-1, h-1)
					Else
						' Draw a rollover button
						g.color = lightHighlight
						g.drawRect(0, 0, w-1, h-1)
						g.color = shade
						g.drawLine(w-1, 0, w-1, h-1)
						g.drawLine(0, h-1, w-1, h-1)
					End If
					g.translate(-x, -y)
					g.color = oldColor
				End If
			End Sub
		End Class


		''' <summary>
		''' A border which is like a Margin border but it will only honor the margin
		''' if the margin has been explicitly set by the developer.
		''' 
		''' Note: This is identical to the package private class
		''' MetalBorders.RolloverMarginBorder and should probably be consolidated.
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

	   Public Class ButtonBorder
		   Inherits AbstractBorder
		   Implements UIResource

			Protected Friend shadow As java.awt.Color
			Protected Friend darkShadow As java.awt.Color
			Protected Friend highlight As java.awt.Color
			Protected Friend lightHighlight As java.awt.Color

			Public Sub New(ByVal shadow As java.awt.Color, ByVal darkShadow As java.awt.Color, ByVal highlight As java.awt.Color, ByVal lightHighlight As java.awt.Color)
				Me.shadow = shadow
				Me.darkShadow = darkShadow
				Me.highlight = highlight
				Me.lightHighlight = lightHighlight
			End Sub

			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
				Dim isPressed As Boolean = False
				Dim isDefault As Boolean = False

				If TypeOf c Is AbstractButton Then
					Dim b As AbstractButton = CType(c, AbstractButton)
					Dim model As ButtonModel = b.model

					isPressed = model.pressed AndAlso model.armed

					If TypeOf c Is JButton Then isDefault = CType(c, JButton).defaultButton
				End If
				BasicGraphicsUtils.drawBezel(g, x, y, width, height, isPressed, isDefault, shadow, darkShadow, highlight, lightHighlight)
			End Sub

			Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal insets As java.awt.Insets) As java.awt.Insets
				' leave room for default visual
				insets.set(2, 3, 3, 3)
				Return insets
			End Function

	   End Class

		Public Class ToggleButtonBorder
			Inherits ButtonBorder

			Public Sub New(ByVal shadow As java.awt.Color, ByVal darkShadow As java.awt.Color, ByVal highlight As java.awt.Color, ByVal lightHighlight As java.awt.Color)
				MyBase.New(shadow, darkShadow, highlight, lightHighlight)
			End Sub

			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
					BasicGraphicsUtils.drawBezel(g, x, y, width, height, False, False, shadow, darkShadow, highlight, lightHighlight)
			End Sub

			Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal insets As java.awt.Insets) As java.awt.Insets
				insets.set(2, 2, 2, 2)
				Return insets
			End Function
		End Class

		Public Class RadioButtonBorder
			Inherits ButtonBorder

			Public Sub New(ByVal shadow As java.awt.Color, ByVal darkShadow As java.awt.Color, ByVal highlight As java.awt.Color, ByVal lightHighlight As java.awt.Color)
				MyBase.New(shadow, darkShadow, highlight, lightHighlight)
			End Sub


			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)

				If TypeOf c Is AbstractButton Then
					Dim b As AbstractButton = CType(c, AbstractButton)
					Dim model As ButtonModel = b.model

					If model.armed AndAlso model.pressed OrElse model.selected Then
						BasicGraphicsUtils.drawLoweredBezel(g, x, y, width, height, shadow, darkShadow, highlight, lightHighlight)
					Else
						BasicGraphicsUtils.drawBezel(g, x, y, width, height, False, b.focusPainted AndAlso b.hasFocus(), shadow, darkShadow, highlight, lightHighlight)
					End If
				Else
					BasicGraphicsUtils.drawBezel(g, x, y, width, height, False, False, shadow, darkShadow, highlight, lightHighlight)
				End If
			End Sub

			Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal insets As java.awt.Insets) As java.awt.Insets
				insets.set(2, 2, 2, 2)
				Return insets
			End Function
		End Class

		Public Class MenuBarBorder
			Inherits AbstractBorder
			Implements UIResource

			Private shadow As java.awt.Color
			Private highlight As java.awt.Color

			Public Sub New(ByVal shadow As java.awt.Color, ByVal highlight As java.awt.Color)
				Me.shadow = shadow
				Me.highlight = highlight
			End Sub

			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
				Dim oldColor As java.awt.Color = g.color
				g.translate(x, y)
				g.color = shadow
				sun.swing.SwingUtilities2.drawHLine(g, 0, width - 1, height - 2)
				g.color = highlight
				sun.swing.SwingUtilities2.drawHLine(g, 0, width - 1, height - 1)
				g.translate(-x, -y)
				g.color = oldColor
			End Sub

			Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal insets As java.awt.Insets) As java.awt.Insets
				insets.set(0, 0, 2, 0)
				Return insets
			End Function
		End Class

		Public Class MarginBorder
			Inherits AbstractBorder
			Implements UIResource

			Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal insets As java.awt.Insets) As java.awt.Insets
				Dim margin As java.awt.Insets = Nothing
				'
				' Ideally we'd have an interface defined for classes which
				' support margins (to avoid this hackery), but we've
				' decided against it for simplicity
				'
			   If TypeOf c Is AbstractButton Then
				   Dim b As AbstractButton = CType(c, AbstractButton)
				   margin = b.margin
			   ElseIf TypeOf c Is JToolBar Then
				   Dim t As JToolBar = CType(c, JToolBar)
				   margin = t.margin
			   ElseIf TypeOf c Is javax.swing.text.JTextComponent Then
				   Dim t As javax.swing.text.JTextComponent = CType(c, javax.swing.text.JTextComponent)
				   margin = t.margin
			   End If
			   insets.top = If(margin IsNot Nothing, margin.top, 0)
			   insets.left = If(margin IsNot Nothing, margin.left, 0)
			   insets.bottom = If(margin IsNot Nothing, margin.bottom, 0)
			   insets.right = If(margin IsNot Nothing, margin.right, 0)

			   Return insets
			End Function
		End Class

		Public Class FieldBorder
			Inherits AbstractBorder
			Implements UIResource

			Protected Friend shadow As java.awt.Color
			Protected Friend darkShadow As java.awt.Color
			Protected Friend highlight As java.awt.Color
			Protected Friend lightHighlight As java.awt.Color

			Public Sub New(ByVal shadow As java.awt.Color, ByVal darkShadow As java.awt.Color, ByVal highlight As java.awt.Color, ByVal lightHighlight As java.awt.Color)
				Me.shadow = shadow
				Me.highlight = highlight
				Me.darkShadow = darkShadow
				Me.lightHighlight = lightHighlight
			End Sub

			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
				BasicGraphicsUtils.drawEtchedRect(g, x, y, width, height, shadow, darkShadow, highlight, lightHighlight)
			End Sub

			Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal insets As java.awt.Insets) As java.awt.Insets
				Dim margin As java.awt.Insets = Nothing
				If TypeOf c Is javax.swing.text.JTextComponent Then margin = CType(c, javax.swing.text.JTextComponent).margin
				insets.top = If(margin IsNot Nothing, 2+margin.top, 2)
				insets.left = If(margin IsNot Nothing, 2+margin.left, 2)
				insets.bottom = If(margin IsNot Nothing, 2+margin.bottom, 2)
				insets.right = If(margin IsNot Nothing, 2+margin.right, 2)

				Return insets
			End Function
		End Class


		''' <summary>
		''' Draws the border around the divider in a splitpane
		''' (when BasicSplitPaneUI is used). To get the appropriate effect, this
		''' needs to be used with a SplitPaneBorder.
		''' </summary>
		Friend Class SplitPaneDividerBorder
			Implements Border, UIResource

			Friend highlight As java.awt.Color
			Friend shadow As java.awt.Color

			Friend Sub New(ByVal highlight As java.awt.Color, ByVal shadow As java.awt.Color)
				Me.highlight = highlight
				Me.shadow = shadow
			End Sub

			Public Overridable Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer) Implements Border.paintBorder
				If Not(TypeOf c Is BasicSplitPaneDivider) Then Return
				Dim child As java.awt.Component
				Dim cBounds As java.awt.Rectangle
				Dim splitPane As JSplitPane = CType(c, BasicSplitPaneDivider).basicSplitPaneUI.splitPane
				Dim size As java.awt.Dimension = c.size

				child = splitPane.leftComponent
				' This is needed for the space between the divider and end of
				' splitpane.
				g.color = c.background
				g.drawRect(x, y, width - 1, height - 1)
				If splitPane.orientation = JSplitPane.HORIZONTAL_SPLIT Then
					If child IsNot Nothing Then
						g.color = highlight
						g.drawLine(0, 0, 0, size.height)
					End If
					child = splitPane.rightComponent
					If child IsNot Nothing Then
						g.color = shadow
						g.drawLine(size.width - 1, 0, size.width - 1, size.height)
					End If
				Else
					If child IsNot Nothing Then
						g.color = highlight
						g.drawLine(0, 0, size.width, 0)
					End If
					child = splitPane.rightComponent
					If child IsNot Nothing Then
						g.color = shadow
						g.drawLine(0, size.height - 1, size.width, size.height - 1)
					End If
				End If
			End Sub
			Public Overridable Function getBorderInsets(ByVal c As java.awt.Component) As java.awt.Insets Implements Border.getBorderInsets
				Dim insets As New java.awt.Insets(0,0,0,0)
				If TypeOf c Is BasicSplitPaneDivider Then
					Dim bspui As BasicSplitPaneUI = CType(c, BasicSplitPaneDivider).basicSplitPaneUI

					If bspui IsNot Nothing Then
						Dim splitPane As JSplitPane = bspui.splitPane

						If splitPane IsNot Nothing Then
							If splitPane.orientation = JSplitPane.HORIZONTAL_SPLIT Then
									insets.bottom = 0
									insets.top = insets.bottom
									insets.right = 1
									insets.left = insets.right
								Return insets
							End If
							' VERTICAL_SPLIT
								insets.bottom = 1
								insets.top = insets.bottom
								insets.right = 0
								insets.left = insets.right
							Return insets
						End If
					End If
				End If
					insets.right = 1
						insets.left = insets.right
							insets.bottom = insets.left
							insets.top = insets.bottom
				Return insets
			End Function
			Public Overridable Property borderOpaque As Boolean Implements Border.isBorderOpaque
				Get
					Return True
				End Get
			End Property
		End Class


		''' <summary>
		''' Draws the border around the splitpane. To work correctly you should
		''' also install a border on the divider (property SplitPaneDivider.border).
		''' </summary>
		Public Class SplitPaneBorder
			Implements Border, UIResource

			Protected Friend highlight As java.awt.Color
			Protected Friend shadow As java.awt.Color

			Public Sub New(ByVal highlight As java.awt.Color, ByVal shadow As java.awt.Color)
				Me.highlight = highlight
				Me.shadow = shadow
			End Sub

			Public Overridable Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer) Implements Border.paintBorder
				If Not(TypeOf c Is JSplitPane) Then Return
				' The only tricky part with this border is that the divider is
				' not positioned at the top (for horizontal) or left (for vert),
				' so this border draws to where the divider is:
				' -----------------
				' |xxxxxxx xxxxxxx|
				' |x     ---     x|
				' |x     | |     x|
				' |x     |D|     x|
				' |x     | |     x|
				' |x     ---     x|
				' |xxxxxxx xxxxxxx|
				' -----------------
				' The above shows (rather excessively) what this looks like for
				' a horizontal orientation. This border then draws the x's, with
				' the SplitPaneDividerBorder drawing its own border.

				Dim child As java.awt.Component
				Dim cBounds As java.awt.Rectangle

				Dim splitPane As JSplitPane = CType(c, JSplitPane)

				child = splitPane.leftComponent
				' This is needed for the space between the divider and end of
				' splitpane.
				g.color = c.background
				g.drawRect(x, y, width - 1, height - 1)
				If splitPane.orientation = JSplitPane.HORIZONTAL_SPLIT Then
					If child IsNot Nothing Then
						cBounds = child.bounds
						g.color = shadow
						g.drawLine(0, 0, cBounds.width + 1, 0)
						g.drawLine(0, 1, 0, cBounds.height + 1)

						g.color = highlight
						g.drawLine(0, cBounds.height + 1, cBounds.width + 1, cBounds.height + 1)
					End If
					child = splitPane.rightComponent
					If child IsNot Nothing Then
						cBounds = child.bounds

						Dim maxX As Integer = cBounds.x + cBounds.width
						Dim maxY As Integer = cBounds.y + cBounds.height

						g.color = shadow
						g.drawLine(cBounds.x - 1, 0, maxX, 0)
						g.color = highlight
						g.drawLine(cBounds.x - 1, maxY, maxX, maxY)
						g.drawLine(maxX, 0, maxX, maxY + 1)
					End If
				Else
					If child IsNot Nothing Then
						cBounds = child.bounds
						g.color = shadow
						g.drawLine(0, 0, cBounds.width + 1, 0)
						g.drawLine(0, 1, 0, cBounds.height)
						g.color = highlight
						g.drawLine(1 + cBounds.width, 0, 1 + cBounds.width, cBounds.height + 1)
						g.drawLine(0, cBounds.height + 1, 0, cBounds.height + 1)
					End If
					child = splitPane.rightComponent
					If child IsNot Nothing Then
						cBounds = child.bounds

						Dim maxX As Integer = cBounds.x + cBounds.width
						Dim maxY As Integer = cBounds.y + cBounds.height

						g.color = shadow
						g.drawLine(0, cBounds.y - 1, 0, maxY)
						g.drawLine(maxX, cBounds.y - 1, maxX, cBounds.y - 1)
						g.color = highlight
						g.drawLine(0, maxY, cBounds.width + 1, maxY)
						g.drawLine(maxX, cBounds.y, maxX, maxY)
					End If
				End If
			End Sub
			Public Overridable Function getBorderInsets(ByVal c As java.awt.Component) As java.awt.Insets Implements Border.getBorderInsets
				Return New java.awt.Insets(1, 1, 1, 1)
			End Function
			Public Overridable Property borderOpaque As Boolean Implements Border.isBorderOpaque
				Get
					Return True
				End Get
			End Property
		End Class

	End Class

End Namespace
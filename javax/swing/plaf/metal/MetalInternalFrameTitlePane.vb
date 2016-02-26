Imports Microsoft.VisualBasic
Imports System
Imports javax.swing
Imports javax.swing.border

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

Namespace javax.swing.plaf.metal



	''' <summary>
	''' Class that manages a JLF title bar
	''' @author Steve Wilson
	''' @author Brian Beck
	''' @since 1.3
	''' </summary>

	Public Class MetalInternalFrameTitlePane
		Inherits javax.swing.plaf.basic.BasicInternalFrameTitlePane

		Protected Friend isPalette As Boolean = False
		Protected Friend paletteCloseIcon As Icon
		Protected Friend paletteTitleHeight As Integer

		Private Shared ReadOnly handyEmptyBorder As Border = New EmptyBorder(0,0,0,0)

		''' <summary>
		''' Key used to lookup Color from UIManager. If this is null,
		''' <code>getWindowTitleBackground</code> is used.
		''' </summary>
		Private selectedBackgroundKey As String
		''' <summary>
		''' Key used to lookup Color from UIManager. If this is null,
		''' <code>getWindowTitleForeground</code> is used.
		''' </summary>
		Private selectedForegroundKey As String
		''' <summary>
		''' Key used to lookup shadow color from UIManager. If this is null,
		''' <code>getPrimaryControlDarkShadow</code> is used.
		''' </summary>
		Private selectedShadowKey As String
		''' <summary>
		''' Boolean indicating the state of the <code>JInternalFrame</code>s
		''' closable property at <code>updateUI</code> time.
		''' </summary>
		Private wasClosable As Boolean

		Friend buttonsWidth As Integer = 0

		Friend activeBumps As New MetalBumps(0, 0, MetalLookAndFeel.primaryControlHighlight, MetalLookAndFeel.primaryControlDarkShadow,If(UIManager.get("InternalFrame.activeTitleGradient") IsNot Nothing, Nothing, MetalLookAndFeel.primaryControl))
		Friend inactiveBumps As New MetalBumps(0, 0, MetalLookAndFeel.controlHighlight, MetalLookAndFeel.controlDarkShadow,If(UIManager.get("InternalFrame.inactiveTitleGradient") IsNot Nothing, Nothing, MetalLookAndFeel.control))
		Friend paletteBumps As MetalBumps

		Private activeBumpsHighlight As Color = MetalLookAndFeel.primaryControlHighlight
		Private activeBumpsShadow As Color = MetalLookAndFeel.primaryControlDarkShadow

		Public Sub New(ByVal f As JInternalFrame)
			MyBase.New(f)
		End Sub

		Public Overrides Sub addNotify()
			MyBase.addNotify()
			' This is done here instead of in installDefaults as I was worried
			' that the BasicInternalFrameUI might not be fully initialized, and
			' that if this resets the closable state the BasicInternalFrameUI
			' Listeners that get notified might be in an odd/uninitialized state.
			updateOptionPaneState()
		End Sub

		Protected Friend Overrides Sub installDefaults()
			MyBase.installDefaults()
			font = UIManager.getFont("InternalFrame.titleFont")
			paletteTitleHeight = UIManager.getInt("InternalFrame.paletteTitleHeight")
			paletteCloseIcon = UIManager.getIcon("InternalFrame.paletteCloseIcon")
			wasClosable = frame.closable
				selectedBackgroundKey = Nothing
				selectedForegroundKey = selectedBackgroundKey
			If MetalLookAndFeel.usingOcean() Then opaque = True
		End Sub

		Protected Friend Overrides Sub uninstallDefaults()
			MyBase.uninstallDefaults()
			If wasClosable <> frame.closable Then frame.closable = wasClosable
		End Sub

		Protected Friend Overrides Sub createButtons()
			MyBase.createButtons()

			Dim paintActive As Boolean? = If(frame.selected, Boolean.TRUE, Boolean.FALSE)
			iconButton.putClientProperty("paintActive", paintActive)
			iconButton.border = handyEmptyBorder

			maxButton.putClientProperty("paintActive", paintActive)
			maxButton.border = handyEmptyBorder

			closeButton.putClientProperty("paintActive", paintActive)
			closeButton.border = handyEmptyBorder

			' The palette close icon isn't opaque while the regular close icon is.
			' This makes sure palette close buttons have the right background.
			closeButton.background = MetalLookAndFeel.primaryControlShadow

			If MetalLookAndFeel.usingOcean() Then
				iconButton.contentAreaFilled = False
				maxButton.contentAreaFilled = False
				closeButton.contentAreaFilled = False
			End If
		End Sub

		''' <summary>
		''' Override the parent's method to do nothing. Metal frames do not
		''' have system menus.
		''' </summary>
		Protected Friend Overrides Sub assembleSystemMenu()
		End Sub

		''' <summary>
		''' Override the parent's method to do nothing. Metal frames do not
		''' have system menus.
		''' </summary>
		Protected Friend Overrides Sub addSystemMenuItems(ByVal systemMenu As JMenu)
		End Sub

		''' <summary>
		''' Override the parent's method to do nothing. Metal frames do not
		''' have system menus.
		''' </summary>
		Protected Friend Overrides Sub showSystemMenu()
		End Sub

		''' <summary>
		''' Override the parent's method avoid creating a menu bar. Metal frames
		''' do not have system menus.
		''' </summary>
		Protected Friend Overrides Sub addSubComponents()
			add(iconButton)
			add(maxButton)
			add(closeButton)
		End Sub

		Protected Friend Overrides Function createPropertyChangeListener() As java.beans.PropertyChangeListener
			Return New MetalPropertyChangeHandler(Me)
		End Function

		Protected Friend Overrides Function createLayout() As LayoutManager
			Return New MetalTitlePaneLayout(Me)
		End Function

		Friend Class MetalPropertyChangeHandler
			Inherits javax.swing.plaf.basic.BasicInternalFrameTitlePane.PropertyChangeHandler

			Private ReadOnly outerInstance As MetalInternalFrameTitlePane

			Public Sub New(ByVal outerInstance As MetalInternalFrameTitlePane)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Sub propertyChange(ByVal evt As java.beans.PropertyChangeEvent)
				Dim prop As String = evt.propertyName
				If prop.Equals(JInternalFrame.IS_SELECTED_PROPERTY) Then
					Dim b As Boolean? = CBool(evt.newValue)
					outerInstance.iconButton.putClientProperty("paintActive", b)
					outerInstance.closeButton.putClientProperty("paintActive", b)
					outerInstance.maxButton.putClientProperty("paintActive", b)
				ElseIf "JInternalFrame.messageType".Equals(prop) Then
					outerInstance.updateOptionPaneState()
					outerInstance.frame.repaint()
				End If
				MyBase.propertyChange(evt)
			End Sub
		End Class

		Friend Class MetalTitlePaneLayout
			Inherits TitlePaneLayout

			Private ReadOnly outerInstance As MetalInternalFrameTitlePane

			Public Sub New(ByVal outerInstance As MetalInternalFrameTitlePane)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal c As Component)
			End Sub
			Public Overridable Sub removeLayoutComponent(ByVal c As Component)
			End Sub
			Public Overridable Function preferredLayoutSize(ByVal c As Container) As Dimension
				Return minimumLayoutSize(c)
			End Function

			Public Overridable Function minimumLayoutSize(ByVal c As Container) As Dimension
				' Compute width.
				Dim width As Integer = 30
				If outerInstance.frame.closable Then width += 21
				If outerInstance.frame.maximizable Then width += 16 + (If(outerInstance.frame.closable, 10, 4))
				If outerInstance.frame.iconifiable Then width += 16 + (If(outerInstance.frame.maximizable, 2, (If(outerInstance.frame.closable, 10, 4))))
				Dim fm As FontMetrics = outerInstance.frame.getFontMetrics(font)
				Dim frameTitle As String = outerInstance.frame.title
				Dim title_w As Integer = If(frameTitle IsNot Nothing, sun.swing.SwingUtilities2.stringWidth(outerInstance.frame, fm, frameTitle), 0)
				Dim title_length As Integer = If(frameTitle IsNot Nothing, frameTitle.Length, 0)

				If title_length > 2 Then
					Dim subtitle_w As Integer = sun.swing.SwingUtilities2.stringWidth(outerInstance.frame, fm, outerInstance.frame.title.Substring(0, 2) & "...")
					width += If(title_w < subtitle_w, title_w, subtitle_w)
				Else
					width += title_w
				End If

				' Compute height.
				Dim height As Integer
				If outerInstance.isPalette Then
					height = outerInstance.paletteTitleHeight
				Else
					Dim fontHeight As Integer = fm.height
					fontHeight += 7
					Dim icon As Icon = outerInstance.frame.frameIcon
					Dim iconHeight As Integer = 0
					If icon IsNot Nothing Then iconHeight = Math.Min(icon.iconHeight, 16)
					iconHeight += 5
					height = Math.Max(fontHeight, iconHeight)
				End If

				Return New Dimension(width, height)
			End Function

			Public Overridable Sub layoutContainer(ByVal c As Container)
				Dim leftToRight As Boolean = MetalUtils.isLeftToRight(outerInstance.frame)

				Dim w As Integer = outerInstance.width
				Dim x As Integer = If(leftToRight, w, 0)
				Dim y As Integer = 2
				Dim spacing As Integer

				' assumes all buttons have the same dimensions
				' these dimensions include the borders
				Dim buttonHeight As Integer = outerInstance.closeButton.icon.iconHeight
				Dim buttonWidth As Integer = outerInstance.closeButton.icon.iconWidth

				If outerInstance.frame.closable Then
					If outerInstance.isPalette Then
						spacing = 3
						x += If(leftToRight, -spacing -(buttonWidth+2), spacing)
						outerInstance.closeButton.boundsnds(x, y, buttonWidth+2, outerInstance.height-4)
						If Not leftToRight Then x += (buttonWidth+2)
					Else
						spacing = 4
						x += If(leftToRight, -spacing -buttonWidth, spacing)
						outerInstance.closeButton.boundsnds(x, y, buttonWidth, buttonHeight)
						If Not leftToRight Then x += buttonWidth
					End If
				End If

				If outerInstance.frame.maximizable AndAlso (Not outerInstance.isPalette) Then
					spacing = If(outerInstance.frame.closable, 10, 4)
					x += If(leftToRight, -spacing -buttonWidth, spacing)
					outerInstance.maxButton.boundsnds(x, y, buttonWidth, buttonHeight)
					If Not leftToRight Then x += buttonWidth
				End If

				If outerInstance.frame.iconifiable AndAlso (Not outerInstance.isPalette) Then
					spacing = If(outerInstance.frame.maximizable, 2, (If(outerInstance.frame.closable, 10, 4)))
					x += If(leftToRight, -spacing -buttonWidth, spacing)
					outerInstance.iconButton.boundsnds(x, y, buttonWidth, buttonHeight)
					If Not leftToRight Then x += buttonWidth
				End If

				outerInstance.buttonsWidth = If(leftToRight, w - x, x)
			End Sub
		End Class

		Public Overridable Sub paintPalette(ByVal g As Graphics)
			Dim leftToRight As Boolean = MetalUtils.isLeftToRight(frame)

			Dim ___width As Integer = width
			Dim ___height As Integer = height

			If paletteBumps Is Nothing Then paletteBumps = New MetalBumps(0, 0, MetalLookAndFeel.primaryControlHighlight, MetalLookAndFeel.primaryControlInfo, MetalLookAndFeel.primaryControlShadow)

			Dim ___background As Color = MetalLookAndFeel.primaryControlShadow
			Dim darkShadow As Color = MetalLookAndFeel.primaryControlDarkShadow

			g.color = ___background
			g.fillRect(0, 0, ___width, ___height)

			g.color = darkShadow
			g.drawLine(0, ___height - 1, ___width, ___height -1)

			Dim xOffset As Integer = If(leftToRight, 4, buttonsWidth + 4)
			Dim bumpLength As Integer = ___width - buttonsWidth -2*4
			Dim bumpHeight As Integer = height - 4
			paletteBumps.bumpArearea(bumpLength, bumpHeight)
			paletteBumps.paintIcon(Me, g, xOffset, 2)
		End Sub

		Public Overrides Sub paintComponent(ByVal g As Graphics)
			If isPalette Then
				paintPalette(g)
				Return
			End If

			Dim leftToRight As Boolean = MetalUtils.isLeftToRight(frame)
			Dim isSelected As Boolean = frame.selected

			Dim ___width As Integer = width
			Dim ___height As Integer = height

			Dim ___background As Color = Nothing
			Dim ___foreground As Color = Nothing
			Dim shadow As Color = Nothing

			Dim bumps As MetalBumps
			Dim gradientKey As String

			If isSelected Then
				If Not MetalLookAndFeel.usingOcean() Then
					closeButton.contentAreaFilled = True
					maxButton.contentAreaFilled = True
					iconButton.contentAreaFilled = True
				End If
				If selectedBackgroundKey IsNot Nothing Then ___background = UIManager.getColor(selectedBackgroundKey)
				If ___background Is Nothing Then ___background = MetalLookAndFeel.windowTitleBackground
				If selectedForegroundKey IsNot Nothing Then ___foreground = UIManager.getColor(selectedForegroundKey)
				If selectedShadowKey IsNot Nothing Then shadow = UIManager.getColor(selectedShadowKey)
				If shadow Is Nothing Then shadow = MetalLookAndFeel.primaryControlDarkShadow
				If ___foreground Is Nothing Then ___foreground = MetalLookAndFeel.windowTitleForeground
				activeBumps.bumpColorsors(activeBumpsHighlight, activeBumpsShadow,If(UIManager.get("InternalFrame.activeTitleGradient") IsNot Nothing, Nothing, ___background))
				bumps = activeBumps
				gradientKey = "InternalFrame.activeTitleGradient"
			Else
				If Not MetalLookAndFeel.usingOcean() Then
					closeButton.contentAreaFilled = False
					maxButton.contentAreaFilled = False
					iconButton.contentAreaFilled = False
				End If
				___background = MetalLookAndFeel.windowTitleInactiveBackground
				___foreground = MetalLookAndFeel.windowTitleInactiveForeground
				shadow = MetalLookAndFeel.controlDarkShadow
				bumps = inactiveBumps
				gradientKey = "InternalFrame.inactiveTitleGradient"
			End If

			If Not MetalUtils.drawGradient(Me, g, gradientKey, 0, 0, ___width, ___height, True) Then
				g.color = ___background
				g.fillRect(0, 0, ___width, ___height)
			End If

			g.color = shadow
			g.drawLine(0, ___height - 1, ___width, ___height -1)
			g.drawLine(0, 0, 0,0)
			g.drawLine(___width - 1, 0, ___width -1, 0)


			Dim titleLength As Integer
			Dim xOffset As Integer = If(leftToRight, 5, ___width - 5)
			Dim frameTitle As String = frame.title

			Dim icon As Icon = frame.frameIcon
			If icon IsNot Nothing Then
				If Not leftToRight Then xOffset -= icon.iconWidth
				Dim iconY As Integer = ((___height \ 2) - (icon.iconHeight \2))
				icon.paintIcon(frame, g, xOffset, iconY)
				xOffset += If(leftToRight, icon.iconWidth + 5, -5)
			End If

			If frameTitle IsNot Nothing Then
				Dim f As Font = font
				g.font = f
				Dim fm As FontMetrics = sun.swing.SwingUtilities2.getFontMetrics(frame, g, f)
				Dim fHeight As Integer = fm.height

				g.color = ___foreground

				Dim yOffset As Integer = ((___height - fm.height) / 2) + fm.ascent

				Dim rect As New Rectangle(0, 0, 0, 0)
				If frame.iconifiable Then
					rect = iconButton.bounds
				ElseIf frame.maximizable Then
					rect = maxButton.bounds
				ElseIf frame.closable Then
					rect = closeButton.bounds
				End If
				Dim titleW As Integer

				If leftToRight Then
				  If rect.x = 0 Then rect.x = frame.width-frame.insets.right-2
				  titleW = rect.x - xOffset - 4
				  frameTitle = getTitle(frameTitle, fm, titleW)
				Else
				  titleW = xOffset - rect.x - rect.width - 4
				  frameTitle = getTitle(frameTitle, fm, titleW)
				  xOffset -= sun.swing.SwingUtilities2.stringWidth(frame, fm, frameTitle)
				End If

				titleLength = sun.swing.SwingUtilities2.stringWidth(frame, fm, frameTitle)
				sun.swing.SwingUtilities2.drawString(frame, g, frameTitle, xOffset, yOffset)
				xOffset += If(leftToRight, titleLength + 5, -5)
			End If

			Dim bumpXOffset As Integer
			Dim bumpLength As Integer
			If leftToRight Then
				bumpLength = ___width - buttonsWidth - xOffset - 5
				bumpXOffset = xOffset
			Else
				bumpLength = xOffset - buttonsWidth - 5
				bumpXOffset = buttonsWidth + 5
			End If
			Dim bumpYOffset As Integer = 3
			Dim bumpHeight As Integer = height - (2 * bumpYOffset)
			bumps.bumpArearea(bumpLength, bumpHeight)
			bumps.paintIcon(Me, g, bumpXOffset, bumpYOffset)
		End Sub

		Public Overridable Property palette As Boolean
			Set(ByVal b As Boolean)
				isPalette = b
    
				If isPalette Then
					closeButton.icon = paletteCloseIcon
				 If frame.maximizable Then remove(maxButton)
					If frame.iconifiable Then remove(iconButton)
				Else
					closeButton.icon = closeIcon
					If frame.maximizable Then add(maxButton)
					If frame.iconifiable Then add(iconButton)
				End If
				revalidate()
				repaint()
			End Set
		End Property

		''' <summary>
		''' Updates any state dependant upon the JInternalFrame being shown in
		''' a <code>JOptionPane</code>.
		''' </summary>
		Private Sub updateOptionPaneState()
			Dim type As Integer = -2
			Dim closable As Boolean = wasClosable
			Dim obj As Object = frame.getClientProperty("JInternalFrame.messageType")

			If obj Is Nothing Then Return
			If TypeOf obj Is Integer? Then type = CInt(Fix(obj))
			Select Case type
			Case JOptionPane.ERROR_MESSAGE
				selectedBackgroundKey = "OptionPane.errorDialog.titlePane.background"
				selectedForegroundKey = "OptionPane.errorDialog.titlePane.foreground"
				selectedShadowKey = "OptionPane.errorDialog.titlePane.shadow"
				closable = False
			Case JOptionPane.QUESTION_MESSAGE
				selectedBackgroundKey = "OptionPane.questionDialog.titlePane.background"
				selectedForegroundKey = "OptionPane.questionDialog.titlePane.foreground"
				selectedShadowKey = "OptionPane.questionDialog.titlePane.shadow"
				closable = False
			Case JOptionPane.WARNING_MESSAGE
				selectedBackgroundKey = "OptionPane.warningDialog.titlePane.background"
				selectedForegroundKey = "OptionPane.warningDialog.titlePane.foreground"
				selectedShadowKey = "OptionPane.warningDialog.titlePane.shadow"
				closable = False
			Case JOptionPane.INFORMATION_MESSAGE, JOptionPane.PLAIN_MESSAGE
					selectedShadowKey = Nothing
						selectedForegroundKey = selectedShadowKey
						selectedBackgroundKey = selectedForegroundKey
				closable = False
			Case Else
					selectedShadowKey = Nothing
						selectedForegroundKey = selectedShadowKey
						selectedBackgroundKey = selectedForegroundKey
			End Select
			If closable <> frame.closable Then frame.closable = closable
		End Sub
	End Class

End Namespace
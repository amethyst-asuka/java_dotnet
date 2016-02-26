Imports System
Imports javax.swing
Imports javax.swing.plaf

'
' * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf.synth


	''' <summary>
	''' The class that manages a synth title bar
	''' 
	''' @author David Kloba
	''' @author Joshua Outwater
	''' @author Steve Wilson
	''' </summary>
	Friend Class SynthInternalFrameTitlePane
		Inherits javax.swing.plaf.basic.BasicInternalFrameTitlePane
		Implements SynthUI, java.beans.PropertyChangeListener

		Protected Friend systemPopupMenu As JPopupMenu
		Protected Friend menuButton As JButton

		Private style As SynthStyle
		Private titleSpacing As Integer
		Private buttonSpacing As Integer
		' Alignment for the title, one of SwingConstants.(LEADING|TRAILING|CENTER)
		Private titleAlignment As Integer

		Public Sub New(ByVal f As JInternalFrame)
			MyBase.New(f)
		End Sub

		Public Property Overrides uIClassID As String
			Get
				Return "InternalFrameTitlePaneUI"
			End Get
		End Property

		Public Overridable Function getContext(ByVal c As JComponent) As SynthContext
			Return getContext(c, getComponentState(c))
		End Function

		Public Overridable Function getContext(ByVal c As JComponent, ByVal state As Integer) As SynthContext
			Return SynthContext.getContext(c, style, state)
		End Function

		Private Function getRegion(ByVal c As JComponent) As Region
			Return SynthLookAndFeel.getRegion(c)
		End Function

		Private Function getComponentState(ByVal c As JComponent) As Integer
			If frame IsNot Nothing Then
				If frame.selected Then Return SELECTED
			End If
			Return SynthLookAndFeel.getComponentState(c)
		End Function

		Protected Friend Overrides Sub addSubComponents()
			menuButton.name = "InternalFrameTitlePane.menuButton"
			iconButton.name = "InternalFrameTitlePane.iconifyButton"
			maxButton.name = "InternalFrameTitlePane.maximizeButton"
			closeButton.name = "InternalFrameTitlePane.closeButton"

			add(menuButton)
			add(iconButton)
			add(maxButton)
			add(closeButton)
		End Sub

		Protected Friend Overrides Sub installListeners()
			MyBase.installListeners()
			frame.addPropertyChangeListener(Me)
			addPropertyChangeListener(Me)
		End Sub

		Protected Friend Overrides Sub uninstallListeners()
			frame.removePropertyChangeListener(Me)
			removePropertyChangeListener(Me)
			MyBase.uninstallListeners()
		End Sub

		Private Sub updateStyle(ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(Me, ENABLED)
			Dim oldStyle As SynthStyle = style
			style = SynthLookAndFeel.updateStyle(___context, Me)
			If style IsNot oldStyle Then
				maxIcon = style.getIcon(___context,"InternalFrameTitlePane.maximizeIcon")
				minIcon = style.getIcon(___context,"InternalFrameTitlePane.minimizeIcon")
				iconIcon = style.getIcon(___context,"InternalFrameTitlePane.iconifyIcon")
				closeIcon = style.getIcon(___context,"InternalFrameTitlePane.closeIcon")
				titleSpacing = style.getInt(___context, "InternalFrameTitlePane.titleSpacing", 2)
				buttonSpacing = style.getInt(___context, "InternalFrameTitlePane.buttonSpacing", 2)
				Dim alignString As String = CStr(style.get(___context, "InternalFrameTitlePane.titleAlignment"))
				titleAlignment = SwingConstants.LEADING
				If alignString IsNot Nothing Then
					alignString = alignString.ToUpper()
					If alignString.Equals("TRAILING") Then
						titleAlignment = SwingConstants.TRAILING
					ElseIf alignString.Equals("CENTER") Then
						titleAlignment = SwingConstants.CENTER
					End If
				End If
			End If
			___context.Dispose()
		End Sub

		Protected Friend Overrides Sub installDefaults()
			MyBase.installDefaults()
			updateStyle(Me)
		End Sub

		Protected Friend Overrides Sub uninstallDefaults()
			Dim ___context As SynthContext = getContext(Me, ENABLED)
			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing
			Dim di As JInternalFrame.JDesktopIcon = frame.desktopIcon
			If di IsNot Nothing AndAlso di.componentPopupMenu Is systemPopupMenu Then di.componentPopupMenu = Nothing
			MyBase.uninstallDefaults()
		End Sub

		Private Class JPopupMenuUIResource
			Inherits JPopupMenu
			Implements UIResource

		End Class

		Protected Friend Overrides Sub assembleSystemMenu()
			systemPopupMenu = New JPopupMenuUIResource
			addSystemMenuItems(systemPopupMenu)
			enableActions()
			menuButton = createNoFocusButton()
			updateMenuIcon()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			menuButton.addMouseListener(New MouseAdapter()
	'		{
	'			public void mousePressed(MouseEvent e)
	'			{
	'				try
	'				{
	'					frame.setSelected(True);
	'				}
	'				catch(PropertyVetoException pve)
	'				{
	'				}
	'				showSystemMenu();
	'			}
	'		});
			Dim p As JPopupMenu = frame.componentPopupMenu
			If p Is Nothing OrElse TypeOf p Is UIResource Then frame.componentPopupMenu = systemPopupMenu
			If frame.desktopIcon IsNot Nothing Then
				p = frame.desktopIcon.componentPopupMenu
				If p Is Nothing OrElse TypeOf p Is UIResource Then frame.desktopIcon.componentPopupMenu = systemPopupMenu
			End If
			inheritsPopupMenu = True
		End Sub

		Protected Friend Overridable Sub addSystemMenuItems(ByVal menu As JPopupMenu)
			Dim mi As JMenuItem = menu.add(restoreAction)
			mi.mnemonic = getButtonMnemonic("restore")
			mi = menu.add(moveAction)
			mi.mnemonic = getButtonMnemonic("move")
			mi = menu.add(sizeAction)
			mi.mnemonic = getButtonMnemonic("size")
			mi = menu.add(iconifyAction)
			mi.mnemonic = getButtonMnemonic("minimize")
			mi = menu.add(maximizeAction)
			mi.mnemonic = getButtonMnemonic("maximize")
			menu.add(New JSeparator)
			mi = menu.add(closeAction)
			mi.mnemonic = getButtonMnemonic("close")
		End Sub

		Private Shared Function getButtonMnemonic(ByVal button As String) As Integer
			Try
				Return Convert.ToInt32(UIManager.getString("InternalFrameTitlePane." & button & "Button.mnemonic"))
			Catch e As NumberFormatException
				Return -1
			End Try
		End Function

		Protected Friend Overrides Sub showSystemMenu()
			Dim ___insets As Insets = frame.insets
			If Not frame.icon Then
				systemPopupMenu.show(frame, menuButton.x, y + height)
			Else
				systemPopupMenu.show(menuButton, x - ___insets.left - ___insets.right, y - systemPopupMenu.preferredSize.height - ___insets.bottom - ___insets.top)
			End If
		End Sub

		' SynthInternalFrameTitlePane has no UI, we'll invoke paint on it.
		Public Overrides Sub paintComponent(ByVal g As Graphics)
			Dim ___context As SynthContext = getContext(Me)
			SynthLookAndFeel.update(___context, g)
			___context.painter.paintInternalFrameTitlePaneBackground(___context, g, 0, 0, width, height)
			paint(___context, g)
			___context.Dispose()
		End Sub

		Protected Friend Overridable Sub paint(ByVal context As SynthContext, ByVal g As Graphics)
			Dim ___title As String = frame.title

			If ___title IsNot Nothing Then
				Dim style As SynthStyle = context.style

				g.color = style.getColor(context, ColorType.TEXT_FOREGROUND)
				g.font = style.getFont(context)

				' Center text vertically.
				Dim fm As FontMetrics = sun.swing.SwingUtilities2.getFontMetrics(frame, g)
				Dim ___baseline As Integer = (height + fm.ascent - fm.leading - fm.descent) / 2
				Dim lastButton As JButton = Nothing
				If frame.iconifiable Then
					lastButton = iconButton
				ElseIf frame.maximizable Then
					lastButton = maxButton
				ElseIf frame.closable Then
					lastButton = closeButton
				End If
				Dim maxX As Integer
				Dim minX As Integer
				Dim ltr As Boolean = SynthLookAndFeel.isLeftToRight(frame)
				Dim titleAlignment As Integer = Me.titleAlignment
				If ltr Then
					If lastButton IsNot Nothing Then
						maxX = lastButton.x - titleSpacing
					Else
						maxX = frame.width - frame.insets.right - titleSpacing
					End If
					minX = menuButton.x + menuButton.width + titleSpacing
				Else
					If lastButton IsNot Nothing Then
						minX = lastButton.x + lastButton.width + titleSpacing
					Else
						minX = frame.insets.left + titleSpacing
					End If
					maxX = menuButton.x - titleSpacing
					If titleAlignment = SwingConstants.LEADING Then
						titleAlignment = SwingConstants.TRAILING
					ElseIf titleAlignment = SwingConstants.TRAILING Then
						titleAlignment = SwingConstants.LEADING
					End If
				End If
				Dim clippedTitle As String = getTitle(___title, fm, maxX - minX)
				If clippedTitle = ___title Then
					' String fit, align as necessary.
					If titleAlignment = SwingConstants.TRAILING Then
						minX = maxX - style.getGraphicsUtils(context).computeStringWidth(context, g.font, fm, ___title)
					ElseIf titleAlignment = SwingConstants.CENTER Then
						Dim ___width As Integer = style.getGraphicsUtils(context).computeStringWidth(context, g.font, fm, ___title)
						minX = Math.Max(minX, (width - ___width) \ 2)
						minX = Math.Min(maxX - ___width, minX)
					End If
				End If
				style.getGraphicsUtils(context).paintText(context, g, clippedTitle, minX, ___baseline - fm.ascent, -1)
			End If
		End Sub

		Public Overridable Sub paintBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintInternalFrameTitlePaneBorder(context, g, x, y, w, h)
		End Sub

		Protected Friend Overrides Function createLayout() As LayoutManager
			Dim ___context As SynthContext = getContext(Me)
			Dim lm As LayoutManager = CType(style.get(___context, "InternalFrameTitlePane.titlePaneLayout"), LayoutManager)
			___context.Dispose()
			Return If(lm IsNot Nothing, lm, New SynthTitlePaneLayout(Me))
		End Function

		Public Overridable Sub propertyChange(ByVal evt As java.beans.PropertyChangeEvent)
			If evt.source Is Me Then
				If SynthLookAndFeel.shouldUpdateStyle(evt) Then updateStyle(Me)
			Else
				' Changes for the internal frame
				If evt.propertyName = JInternalFrame.FRAME_ICON_PROPERTY Then updateMenuIcon()
			End If
		End Sub

		''' <summary>
		''' Resets the menuButton icon to match that of the frame.
		''' </summary>
		Private Sub updateMenuIcon()
			Dim frameIcon As Icon = frame.frameIcon
			Dim ___context As SynthContext = getContext(Me)
			If frameIcon IsNot Nothing Then
				Dim maxSize As Dimension = CType(___context.style.get(___context, "InternalFrameTitlePane.maxFrameIconSize"), Dimension)
				Dim maxWidth As Integer = 16
				Dim maxHeight As Integer = 16
				If maxSize IsNot Nothing Then
					maxWidth = maxSize.width
					maxHeight = maxSize.height
				End If
				If (frameIcon.iconWidth > maxWidth OrElse frameIcon.iconHeight > maxHeight) AndAlso (TypeOf frameIcon Is ImageIcon) Then frameIcon = New ImageIcon(CType(frameIcon, ImageIcon).image.getScaledInstance(maxWidth, maxHeight, Image.SCALE_SMOOTH))
			End If
			___context.Dispose()
			menuButton.icon = frameIcon
		End Sub


		Friend Class SynthTitlePaneLayout
			Implements LayoutManager

			Private ReadOnly outerInstance As SynthInternalFrameTitlePane

			Public Sub New(ByVal outerInstance As SynthInternalFrameTitlePane)
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
				Dim context As SynthContext = outerInstance.getContext(SynthInternalFrameTitlePane.this)
				Dim width As Integer = 0
				Dim height As Integer = 0

				Dim buttonCount As Integer = 0
				Dim pref As Dimension

				If outerInstance.frame.closable Then
					pref = outerInstance.closeButton.preferredSize
					width += pref.width
					height = Math.Max(pref.height, height)
					buttonCount += 1
				End If
				If outerInstance.frame.maximizable Then
					pref = outerInstance.maxButton.preferredSize
					width += pref.width
					height = Math.Max(pref.height, height)
					buttonCount += 1
				End If
				If outerInstance.frame.iconifiable Then
					pref = outerInstance.iconButton.preferredSize
					width += pref.width
					height = Math.Max(pref.height, height)
					buttonCount += 1
				End If
				pref = outerInstance.menuButton.preferredSize
				width += pref.width
				height = Math.Max(pref.height, height)

				width += Math.Max(0, (buttonCount - 1) * outerInstance.buttonSpacing)

				Dim fm As FontMetrics = outerInstance.getFontMetrics(font)
				Dim graphicsUtils As SynthGraphicsUtils = context.style.getGraphicsUtils(context)
				Dim frameTitle As String = outerInstance.frame.title
				Dim title_w As Integer = If(frameTitle IsNot Nothing, graphicsUtils.computeStringWidth(context, fm.font, fm, frameTitle), 0)
				Dim title_length As Integer = If(frameTitle IsNot Nothing, frameTitle.Length, 0)

				' Leave room for three characters in the title.
				If title_length > 3 Then
					Dim subtitle_w As Integer = graphicsUtils.computeStringWidth(context, fm.font, fm, frameTitle.Substring(0, 3) & "...")
					width += If(title_w < subtitle_w, title_w, subtitle_w)
				Else
					width += title_w
				End If

				height = Math.Max(fm.height + 2, height)

				width += outerInstance.titleSpacing + outerInstance.titleSpacing

				Dim insets As Insets = outerInstance.insets
				height += insets.top + insets.bottom
				width += insets.left + insets.right
				context.Dispose()
				Return New Dimension(width, height)
			End Function

			Private Function center(ByVal c As Component, ByVal insets As Insets, ByVal x As Integer, ByVal trailing As Boolean) As Integer
				Dim pref As Dimension = c.preferredSize
				If trailing Then x -= pref.width
				c.boundsnds(x, insets.top + (outerInstance.height - insets.top - insets.bottom - pref.height) / 2, pref.width, pref.height)
				If pref.width > 0 Then
					If trailing Then Return x - outerInstance.buttonSpacing
					Return x + pref.width + outerInstance.buttonSpacing
				End If
				Return x
			End Function

			Public Overridable Sub layoutContainer(ByVal c As Container)
				Dim insets As Insets = c.insets
				Dim pref As Dimension

				If SynthLookAndFeel.isLeftToRight(outerInstance.frame) Then
					center(outerInstance.menuButton, insets, insets.left, False)
					Dim x As Integer = outerInstance.width - insets.right
					If outerInstance.frame.closable Then x = center(outerInstance.closeButton, insets, x, True)
					If outerInstance.frame.maximizable Then x = center(outerInstance.maxButton, insets, x, True)
					If outerInstance.frame.iconifiable Then x = center(outerInstance.iconButton, insets, x, True)
				Else
					center(outerInstance.menuButton, insets, outerInstance.width - insets.right, True)
					Dim x As Integer = insets.left
					If outerInstance.frame.closable Then x = center(outerInstance.closeButton, insets, x, False)
					If outerInstance.frame.maximizable Then x = center(outerInstance.maxButton, insets, x, False)
					If outerInstance.frame.iconifiable Then x = center(outerInstance.iconButton, insets, x, False)
				End If
			End Sub
		End Class

		Private Function createNoFocusButton() As JButton
			Dim button As New JButton
			button.focusable = False
			button.margin = New Insets(0,0,0,0)
			Return button
		End Function
	End Class

End Namespace
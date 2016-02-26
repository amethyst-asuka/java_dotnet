Imports System
Imports javax.swing
Imports javax.swing.plaf
import static sun.swing.SwingUtilities2.AA_TEXT_PROPERTY_KEY

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

Namespace javax.swing.plaf.basic



	''' <summary>
	''' The class that manages a basic title bar
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
	''' @author David Kloba
	''' @author Steve Wilson
	''' </summary>
	Public Class BasicInternalFrameTitlePane
		Inherits JComponent

		Protected Friend menuBar As JMenuBar
		Protected Friend iconButton As JButton
		Protected Friend maxButton As JButton
		Protected Friend closeButton As JButton

		Protected Friend windowMenu As JMenu
		Protected Friend frame As JInternalFrame

		Protected Friend selectedTitleColor As Color
		Protected Friend selectedTextColor As Color
		Protected Friend notSelectedTitleColor As Color
		Protected Friend notSelectedTextColor As Color

		Protected Friend maxIcon As Icon
		Protected Friend minIcon As Icon
		Protected Friend iconIcon As Icon
		Protected Friend closeIcon As Icon

		Protected Friend propertyChangeListener As java.beans.PropertyChangeListener

		Protected Friend closeAction As Action
		Protected Friend maximizeAction As Action
		Protected Friend iconifyAction As Action
		Protected Friend restoreAction As Action
		Protected Friend moveAction As Action
		Protected Friend sizeAction As Action

		' These constants are not used in JDK code
		Protected Friend Shared ReadOnly CLOSE_CMD As String = UIManager.getString("InternalFrameTitlePane.closeButtonText")
		Protected Friend Shared ReadOnly ICONIFY_CMD As String = UIManager.getString("InternalFrameTitlePane.minimizeButtonText")
		Protected Friend Shared ReadOnly RESTORE_CMD As String = UIManager.getString("InternalFrameTitlePane.restoreButtonText")
		Protected Friend Shared ReadOnly MAXIMIZE_CMD As String = UIManager.getString("InternalFrameTitlePane.maximizeButtonText")
		Protected Friend Shared ReadOnly MOVE_CMD As String = UIManager.getString("InternalFrameTitlePane.moveButtonText")
		Protected Friend Shared ReadOnly SIZE_CMD As String = UIManager.getString("InternalFrameTitlePane.sizeButtonText")

		Private closeButtonToolTip As String
		Private iconButtonToolTip As String
		Private restoreButtonToolTip As String
		Private maxButtonToolTip As String
		Private handler As Handler

		Public Sub New(ByVal f As JInternalFrame)
			frame = f
			installTitlePane()
		End Sub

		Protected Friend Overridable Sub installTitlePane()
			installDefaults()
			installListeners()

			createActions()
			enableActions()
			createActionMap()

			layout = createLayout()

			assembleSystemMenu()
			createButtons()
			addSubComponents()

			updateProperties()
		End Sub

		Private Sub updateProperties()
			Dim aaTextInfo As Object = frame.getClientProperty(AA_TEXT_PROPERTY_KEY)
			putClientProperty(AA_TEXT_PROPERTY_KEY, aaTextInfo)
		End Sub

		Protected Friend Overridable Sub addSubComponents()
			add(menuBar)
			add(iconButton)
			add(maxButton)
			add(closeButton)
		End Sub

		Protected Friend Overridable Sub createActions()
			maximizeAction = New MaximizeAction(Me)
			iconifyAction = New IconifyAction(Me)
			closeAction = New CloseAction(Me)
			restoreAction = New RestoreAction(Me)
			moveAction = New MoveAction(Me)
			sizeAction = New SizeAction(Me)
		End Sub

		Friend Overridable Function createActionMap() As ActionMap
			Dim map As ActionMap = New ActionMapUIResource
			map.put("showSystemMenu", New ShowSystemMenuAction(Me, True))
			map.put("hideSystemMenu", New ShowSystemMenuAction(Me, False))
			Return map
		End Function

		Protected Friend Overridable Sub installListeners()
			If propertyChangeListener Is Nothing Then propertyChangeListener = createPropertyChangeListener()
			frame.addPropertyChangeListener(propertyChangeListener)
		End Sub

		Protected Friend Overridable Sub uninstallListeners()
			frame.removePropertyChangeListener(propertyChangeListener)
			handler = Nothing
		End Sub

		Protected Friend Overridable Sub installDefaults()
			maxIcon = UIManager.getIcon("InternalFrame.maximizeIcon")
			minIcon = UIManager.getIcon("InternalFrame.minimizeIcon")
			iconIcon = UIManager.getIcon("InternalFrame.iconifyIcon")
			closeIcon = UIManager.getIcon("InternalFrame.closeIcon")

			selectedTitleColor = UIManager.getColor("InternalFrame.activeTitleBackground")
			selectedTextColor = UIManager.getColor("InternalFrame.activeTitleForeground")
			notSelectedTitleColor = UIManager.getColor("InternalFrame.inactiveTitleBackground")
			notSelectedTextColor = UIManager.getColor("InternalFrame.inactiveTitleForeground")
			font = UIManager.getFont("InternalFrame.titleFont")
			closeButtonToolTip = UIManager.getString("InternalFrame.closeButtonToolTip")
			iconButtonToolTip = UIManager.getString("InternalFrame.iconButtonToolTip")
			restoreButtonToolTip = UIManager.getString("InternalFrame.restoreButtonToolTip")
			maxButtonToolTip = UIManager.getString("InternalFrame.maxButtonToolTip")
		End Sub


		Protected Friend Overridable Sub uninstallDefaults()
		End Sub

		Protected Friend Overridable Sub createButtons()
			iconButton = New NoFocusButton(Me, "InternalFrameTitlePane.iconifyButtonAccessibleName", "InternalFrameTitlePane.iconifyButtonOpacity")
			iconButton.addActionListener(iconifyAction)
			If iconButtonToolTip IsNot Nothing AndAlso iconButtonToolTip.Length <> 0 Then iconButton.toolTipText = iconButtonToolTip

			maxButton = New NoFocusButton(Me, "InternalFrameTitlePane.maximizeButtonAccessibleName", "InternalFrameTitlePane.maximizeButtonOpacity")
			maxButton.addActionListener(maximizeAction)

			closeButton = New NoFocusButton(Me, "InternalFrameTitlePane.closeButtonAccessibleName", "InternalFrameTitlePane.closeButtonOpacity")
			closeButton.addActionListener(closeAction)
			If closeButtonToolTip IsNot Nothing AndAlso closeButtonToolTip.Length <> 0 Then closeButton.toolTipText = closeButtonToolTip

			buttonIconsons()
		End Sub

		Protected Friend Overridable Sub setButtonIcons()
			If frame.icon Then
				If minIcon IsNot Nothing Then iconButton.icon = minIcon
				If restoreButtonToolTip IsNot Nothing AndAlso restoreButtonToolTip.Length <> 0 Then iconButton.toolTipText = restoreButtonToolTip
				If maxIcon IsNot Nothing Then maxButton.icon = maxIcon
				If maxButtonToolTip IsNot Nothing AndAlso maxButtonToolTip.Length <> 0 Then maxButton.toolTipText = maxButtonToolTip
			ElseIf frame.maximum Then
				If iconIcon IsNot Nothing Then iconButton.icon = iconIcon
				If iconButtonToolTip IsNot Nothing AndAlso iconButtonToolTip.Length <> 0 Then iconButton.toolTipText = iconButtonToolTip
				If minIcon IsNot Nothing Then maxButton.icon = minIcon
				If restoreButtonToolTip IsNot Nothing AndAlso restoreButtonToolTip.Length <> 0 Then maxButton.toolTipText = restoreButtonToolTip
			Else
				If iconIcon IsNot Nothing Then iconButton.icon = iconIcon
				If iconButtonToolTip IsNot Nothing AndAlso iconButtonToolTip.Length <> 0 Then iconButton.toolTipText = iconButtonToolTip
				If maxIcon IsNot Nothing Then maxButton.icon = maxIcon
				If maxButtonToolTip IsNot Nothing AndAlso maxButtonToolTip.Length <> 0 Then maxButton.toolTipText = maxButtonToolTip
			End If
			If closeIcon IsNot Nothing Then closeButton.icon = closeIcon
		End Sub

		Protected Friend Overridable Sub assembleSystemMenu()
			menuBar = createSystemMenuBar()
			windowMenu = createSystemMenu()
			menuBar.add(windowMenu)
			addSystemMenuItems(windowMenu)
			enableActions()
		End Sub

		Protected Friend Overridable Sub addSystemMenuItems(ByVal systemMenu As JMenu)
			Dim mi As JMenuItem = systemMenu.add(restoreAction)
			mi.mnemonic = getButtonMnemonic("restore")
			mi = systemMenu.add(moveAction)
			mi.mnemonic = getButtonMnemonic("move")
			mi = systemMenu.add(sizeAction)
			mi.mnemonic = getButtonMnemonic("size")
			mi = systemMenu.add(iconifyAction)
			mi.mnemonic = getButtonMnemonic("minimize")
			mi = systemMenu.add(maximizeAction)
			mi.mnemonic = getButtonMnemonic("maximize")
			systemMenu.add(New JSeparator)
			mi = systemMenu.add(closeAction)
			mi.mnemonic = getButtonMnemonic("close")
		End Sub

		Private Shared Function getButtonMnemonic(ByVal button As String) As Integer
			Try
				Return Convert.ToInt32(UIManager.getString("InternalFrameTitlePane." & button & "Button.mnemonic"))
			Catch e As NumberFormatException
				Return -1
			End Try
		End Function

		Protected Friend Overridable Function createSystemMenu() As JMenu
			Return New JMenu("    ")
		End Function

		Protected Friend Overridable Function createSystemMenuBar() As JMenuBar
			menuBar = New SystemMenuBar(Me)
			menuBar.borderPainted = False
			Return menuBar
		End Function

		Protected Friend Overridable Sub showSystemMenu()
			'      windowMenu.setPopupMenuVisible(true);
		  '      windowMenu.setVisible(true);
		  windowMenu.doClick()
		End Sub

		Public Overrides Sub paintComponent(ByVal g As Graphics)
			paintTitleBackground(g)

			If frame.title IsNot Nothing Then
				Dim isSelected As Boolean = frame.selected
				Dim f As Font = g.font
				g.font = font
				If isSelected Then
					g.color = selectedTextColor
				Else
					g.color = notSelectedTextColor
				End If

				' Center text vertically.
				Dim fm As FontMetrics = sun.swing.SwingUtilities2.getFontMetrics(frame, g)
				Dim ___baseline As Integer = (height + fm.ascent - fm.leading - fm.descent) / 2

				Dim titleX As Integer
				Dim r As New Rectangle(0, 0, 0, 0)
				If frame.iconifiable Then
					r = iconButton.bounds
				ElseIf frame.maximizable Then
					r = maxButton.bounds
				ElseIf frame.closable Then
					r = closeButton.bounds
				End If
				Dim titleW As Integer

				Dim ___title As String = frame.title
				If BasicGraphicsUtils.isLeftToRight(frame) Then
				  If r.x = 0 Then r.x = frame.width-frame.insets.right
				  titleX = menuBar.x + menuBar.width + 2
				  titleW = r.x - titleX - 3
				  ___title = getTitle(frame.title, fm, titleW)
				Else
					titleX = menuBar.x - 2 - sun.swing.SwingUtilities2.stringWidth(frame,fm,___title)
				End If

				sun.swing.SwingUtilities2.drawString(frame, g, ___title, titleX, ___baseline)
				g.font = f
			End If
		End Sub

	   ''' <summary>
	   ''' Invoked from paintComponent.
	   ''' Paints the background of the titlepane.  All text and icons will
	   ''' then be rendered on top of this background. </summary>
	   ''' <param name="g"> the graphics to use to render the background
	   ''' @since 1.4 </param>
		Protected Friend Overridable Sub paintTitleBackground(ByVal g As Graphics)
			Dim isSelected As Boolean = frame.selected

			If isSelected Then
				g.color = selectedTitleColor
			Else
				g.color = notSelectedTitleColor
			End If
			g.fillRect(0, 0, width, height)
		End Sub

		Protected Friend Overridable Function getTitle(ByVal text As String, ByVal fm As FontMetrics, ByVal availTextWidth As Integer) As String
			Return sun.swing.SwingUtilities2.clipStringIfNecessary(frame, fm, text, availTextWidth)
		End Function

		''' <summary>
		''' Post a WINDOW_CLOSING-like event to the frame, so that it can
		''' be treated like a regular Frame.
		''' </summary>
		Protected Friend Overridable Sub postClosingEvent(ByVal frame As JInternalFrame)
			Dim e As New javax.swing.event.InternalFrameEvent(frame, javax.swing.event.InternalFrameEvent.INTERNAL_FRAME_CLOSING)
			' Try posting event, unless there's a SecurityManager.
			Try
				Toolkit.defaultToolkit.systemEventQueue.postEvent(e)
			Catch se As SecurityException
				frame.dispatchEvent(e)
			End Try
		End Sub


		Protected Friend Overridable Sub enableActions()
			restoreAction.enabled = frame.maximum OrElse frame.icon
			maximizeAction.enabled = (frame.maximizable AndAlso (Not frame.maximum) AndAlso (Not frame.icon)) OrElse (frame.maximizable AndAlso frame.icon)
			iconifyAction.enabled = frame.iconifiable AndAlso (Not frame.icon)
			closeAction.enabled = frame.closable
			sizeAction.enabled = False
			moveAction.enabled = False
		End Sub

		Private Property handler As Handler
			Get
				If handler Is Nothing Then handler = New Handler(Me)
				Return handler
			End Get
		End Property

		Protected Friend Overridable Function createPropertyChangeListener() As java.beans.PropertyChangeListener
			Return handler
		End Function

		Protected Friend Overridable Function createLayout() As LayoutManager
			Return handler
		End Function


		Private Class Handler
			Implements LayoutManager, java.beans.PropertyChangeListener

			Private ReadOnly outerInstance As BasicInternalFrameTitlePane

			Public Sub New(ByVal outerInstance As BasicInternalFrameTitlePane)
				Me.outerInstance = outerInstance
			End Sub

			'
			' PropertyChangeListener
			'
			Public Overridable Sub propertyChange(ByVal evt As java.beans.PropertyChangeEvent)
				Dim prop As String = evt.propertyName

				If prop = JInternalFrame.IS_SELECTED_PROPERTY Then
					repaint()
					Return
				End If

				If prop = JInternalFrame.IS_ICON_PROPERTY OrElse prop = JInternalFrame.IS_MAXIMUM_PROPERTY Then
					outerInstance.buttonIconsons()
					outerInstance.enableActions()
					Return
				End If

				If "closable" = prop Then
					If evt.newValue = Boolean.TRUE Then
						add(outerInstance.closeButton)
					Else
						remove(outerInstance.closeButton)
					End If
				ElseIf "maximizable" = prop Then
					If evt.newValue = Boolean.TRUE Then
						add(outerInstance.maxButton)
					Else
						remove(outerInstance.maxButton)
					End If
				ElseIf "iconable" = prop Then
					If evt.newValue = Boolean.TRUE Then
						add(outerInstance.iconButton)
					Else
						remove(outerInstance.iconButton)
					End If
				End If
				outerInstance.enableActions()

				outerInstance.revalidate()
				repaint()
			End Sub


			'
			' LayoutManager
			'
			Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal c As Component)
			End Sub
			Public Overridable Sub removeLayoutComponent(ByVal c As Component)
			End Sub
			Public Overridable Function preferredLayoutSize(ByVal c As Container) As Dimension
				Return minimumLayoutSize(c)
			End Function

			Public Overridable Function minimumLayoutSize(ByVal c As Container) As Dimension
				' Calculate width.
				Dim width As Integer = 22

				If outerInstance.frame.closable Then width += 19
				If outerInstance.frame.maximizable Then width += 19
				If outerInstance.frame.iconifiable Then width += 19

				Dim fm As FontMetrics = outerInstance.frame.getFontMetrics(font)
				Dim frameTitle As String = outerInstance.frame.title
				Dim title_w As Integer = If(frameTitle IsNot Nothing, sun.swing.SwingUtilities2.stringWidth(outerInstance.frame, fm, frameTitle), 0)
				Dim title_length As Integer = If(frameTitle IsNot Nothing, frameTitle.Length, 0)

				' Leave room for three characters in the title.
				If title_length > 3 Then
					Dim subtitle_w As Integer = sun.swing.SwingUtilities2.stringWidth(outerInstance.frame, fm, frameTitle.Substring(0, 3) & "...")
					width += If(title_w < subtitle_w, title_w, subtitle_w)
				Else
					width += title_w
				End If

				' Calculate height.
				Dim icon As Icon = outerInstance.frame.frameIcon
				Dim fontHeight As Integer = fm.height
				fontHeight += 2
				Dim iconHeight As Integer = 0
				If icon IsNot Nothing Then iconHeight = Math.Min(icon.iconHeight, 16)
				iconHeight += 2

				Dim height As Integer = Math.Max(fontHeight, iconHeight)

				Dim [dim] As New Dimension(width, height)

				' Take into account the border insets if any.
				If outerInstance.border IsNot Nothing Then
					Dim insets As Insets = outerInstance.border.getBorderInsets(c)
					[dim].height += insets.top + insets.bottom
					[dim].width += insets.left + insets.right
				End If
				Return [dim]
			End Function

			Public Overridable Sub layoutContainer(ByVal c As Container)
				Dim leftToRight As Boolean = BasicGraphicsUtils.isLeftToRight(outerInstance.frame)

				Dim w As Integer = outerInstance.width
				Dim h As Integer = outerInstance.height
				Dim x As Integer

				Dim buttonHeight As Integer = outerInstance.closeButton.icon.iconHeight

				Dim icon As Icon = outerInstance.frame.frameIcon
				Dim iconHeight As Integer = 0
				If icon IsNot Nothing Then iconHeight = icon.iconHeight
				x = If(leftToRight, 2, w - 16 - 2)
				outerInstance.menuBar.boundsnds(x, (h - iconHeight) \ 2, 16, 16)

				x = If(leftToRight, w - 16 - 2, 2)

				If outerInstance.frame.closable Then
					outerInstance.closeButton.boundsnds(x, (h - buttonHeight) \ 2, 16, 14)
					x += If(leftToRight, -(16 + 2), 16 + 2)
				End If

				If outerInstance.frame.maximizable Then
					outerInstance.maxButton.boundsnds(x, (h - buttonHeight) \ 2, 16, 14)
					x += If(leftToRight, -(16 + 2), 16 + 2)
				End If

				If outerInstance.frame.iconifiable Then outerInstance.iconButton.boundsnds(x, (h - buttonHeight) \ 2, 16, 14)
			End Sub
		End Class

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of <code>Foo</code>.
		''' </summary>
		Public Class PropertyChangeHandler
			Implements java.beans.PropertyChangeListener

			Private ReadOnly outerInstance As BasicInternalFrameTitlePane

			Public Sub New(ByVal outerInstance As BasicInternalFrameTitlePane)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.
			Public Overridable Sub propertyChange(ByVal evt As java.beans.PropertyChangeEvent)
				outerInstance.handler.propertyChange(evt)
			End Sub
		End Class

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of <code>Foo</code>.
		''' </summary>
		Public Class TitlePaneLayout
			Implements LayoutManager

			Private ReadOnly outerInstance As BasicInternalFrameTitlePane

			Public Sub New(ByVal outerInstance As BasicInternalFrameTitlePane)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.
			Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal c As Component)
				outerInstance.handler.addLayoutComponent(name, c)
			End Sub

			Public Overridable Sub removeLayoutComponent(ByVal c As Component)
				outerInstance.handler.removeLayoutComponent(c)
			End Sub

			Public Overridable Function preferredLayoutSize(ByVal c As Container) As Dimension
				Return outerInstance.handler.preferredLayoutSize(c)
			End Function

			Public Overridable Function minimumLayoutSize(ByVal c As Container) As Dimension
				Return outerInstance.handler.minimumLayoutSize(c)
			End Function

			Public Overridable Sub layoutContainer(ByVal c As Container)
				outerInstance.handler.layoutContainer(c)
			End Sub
		End Class

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of <code>Foo</code>.
		''' </summary>
		Public Class CloseAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicInternalFrameTitlePane

			Public Sub New(ByVal outerInstance As BasicInternalFrameTitlePane)
					Me.outerInstance = outerInstance
				MyBase.New(UIManager.getString("InternalFrameTitlePane.closeButtonText"))
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				If outerInstance.frame.closable Then outerInstance.frame.doDefaultCloseAction()
			End Sub
		End Class ' end CloseAction

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of <code>Foo</code>.
		''' </summary>
		Public Class MaximizeAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicInternalFrameTitlePane

			Public Sub New(ByVal outerInstance As BasicInternalFrameTitlePane)
					Me.outerInstance = outerInstance
				MyBase.New(UIManager.getString("InternalFrameTitlePane.maximizeButtonText"))
			End Sub

			Public Overridable Sub actionPerformed(ByVal evt As ActionEvent)
				If outerInstance.frame.maximizable Then
					If outerInstance.frame.maximum AndAlso outerInstance.frame.icon Then
						Try
							outerInstance.frame.icon = False
						Catch e As java.beans.PropertyVetoException
						End Try
					ElseIf Not outerInstance.frame.maximum Then
						Try
							outerInstance.frame.maximum = True
						Catch e As java.beans.PropertyVetoException
						End Try
					Else
						Try
							outerInstance.frame.maximum = False
						Catch e As java.beans.PropertyVetoException
						End Try
					End If
				End If
			End Sub
		End Class

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of <code>Foo</code>.
		''' </summary>
		Public Class IconifyAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicInternalFrameTitlePane

			Public Sub New(ByVal outerInstance As BasicInternalFrameTitlePane)
					Me.outerInstance = outerInstance
				MyBase.New(UIManager.getString("InternalFrameTitlePane.minimizeButtonText"))
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				If outerInstance.frame.iconifiable Then
				  If Not outerInstance.frame.icon Then
					Try
						outerInstance.frame.icon = True
					Catch e1 As java.beans.PropertyVetoException
					End Try
				  Else
					Try
						outerInstance.frame.icon = False
					Catch e1 As java.beans.PropertyVetoException
					End Try
				  End If
				End If
			End Sub
		End Class ' end IconifyAction

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of <code>Foo</code>.
		''' </summary>
		Public Class RestoreAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicInternalFrameTitlePane

			Public Sub New(ByVal outerInstance As BasicInternalFrameTitlePane)
					Me.outerInstance = outerInstance
				MyBase.New(UIManager.getString("InternalFrameTitlePane.restoreButtonText"))
			End Sub

			Public Overridable Sub actionPerformed(ByVal evt As ActionEvent)
				If outerInstance.frame.maximizable AndAlso outerInstance.frame.maximum AndAlso outerInstance.frame.icon Then
					Try
						outerInstance.frame.icon = False
					Catch e As java.beans.PropertyVetoException
					End Try
				ElseIf outerInstance.frame.maximizable AndAlso outerInstance.frame.maximum Then
					Try
						outerInstance.frame.maximum = False
					Catch e As java.beans.PropertyVetoException
					End Try
				ElseIf outerInstance.frame.iconifiable AndAlso outerInstance.frame.icon Then
					Try
						outerInstance.frame.icon = False
					Catch e As java.beans.PropertyVetoException
					End Try
				End If
			End Sub
		End Class

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of <code>Foo</code>.
		''' </summary>
		Public Class MoveAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicInternalFrameTitlePane

			Public Sub New(ByVal outerInstance As BasicInternalFrameTitlePane)
					Me.outerInstance = outerInstance
				MyBase.New(UIManager.getString("InternalFrameTitlePane.moveButtonText"))
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				' This action is currently undefined
			End Sub
		End Class ' end MoveAction

	'    
	'     * Handles showing and hiding the system menu.
	'     
		Private Class ShowSystemMenuAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicInternalFrameTitlePane

			Private show As Boolean ' whether to show the menu

			Public Sub New(ByVal outerInstance As BasicInternalFrameTitlePane, ByVal show As Boolean)
					Me.outerInstance = outerInstance
				Me.show = show
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				If show Then
					outerInstance.windowMenu.doClick()
				Else
					outerInstance.windowMenu.visible = False
				End If
			End Sub
		End Class

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of <code>Foo</code>.
		''' </summary>
		Public Class SizeAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicInternalFrameTitlePane

			Public Sub New(ByVal outerInstance As BasicInternalFrameTitlePane)
					Me.outerInstance = outerInstance
				MyBase.New(UIManager.getString("InternalFrameTitlePane.sizeButtonText"))
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				' This action is currently undefined
			End Sub
		End Class ' end SizeAction


		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of <code>Foo</code>.
		''' </summary>
		Public Class SystemMenuBar
			Inherits JMenuBar

			Private ReadOnly outerInstance As BasicInternalFrameTitlePane

			Public Sub New(ByVal outerInstance As BasicInternalFrameTitlePane)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Property focusTraversable As Boolean
				Get
					Return False
				End Get
			End Property
			Public Overrides Sub requestFocus()
			End Sub
			Public Overrides Sub paint(ByVal g As Graphics)
				Dim icon As Icon = outerInstance.frame.frameIcon
				If icon Is Nothing Then icon = CType(sun.swing.DefaultLookup.get(outerInstance.frame, outerInstance.frame.uI, "InternalFrame.icon"), Icon)
				If icon IsNot Nothing Then
					' Resize to 16x16 if necessary.
					If TypeOf icon Is ImageIcon AndAlso (icon.iconWidth > 16 OrElse icon.iconHeight > 16) Then
						Dim img As Image = CType(icon, ImageIcon).image
						CType(icon, ImageIcon).image = img.getScaledInstance(16, 16, Image.SCALE_SMOOTH)
					End If
					icon.paintIcon(Me, g, 0, 0)
				End If
			End Sub

			Public Property Overrides opaque As Boolean
				Get
					Return True
				End Get
			End Property
		End Class ' end SystemMenuBar


		Private Class NoFocusButton
			Inherits JButton

			Private ReadOnly outerInstance As BasicInternalFrameTitlePane

			Private uiKey As String
			Public Sub New(ByVal outerInstance As BasicInternalFrameTitlePane, ByVal uiKey As String, ByVal opacityKey As String)
					Me.outerInstance = outerInstance
				focusPainted = False
				margin = New Insets(0,0,0,0)
				Me.uiKey = uiKey

				Dim opacity As Object = UIManager.get(opacityKey)
				If TypeOf opacity Is Boolean? Then opaque = CBool(opacity)
			End Sub
			Public Overridable Property focusTraversable As Boolean
				Get
					Return False
				End Get
			End Property
			Public Overrides Sub requestFocus()
			End Sub
			Public Property Overrides accessibleContext As javax.accessibility.AccessibleContext
				Get
					Dim ac As javax.accessibility.AccessibleContext = MyBase.accessibleContext
					If uiKey IsNot Nothing Then
						ac.accessibleName = UIManager.getString(uiKey)
						uiKey = Nothing
					End If
					Return ac
				End Get
			End Property
		End Class ' end NoFocusButton

	End Class ' End Title Pane Class

End Namespace
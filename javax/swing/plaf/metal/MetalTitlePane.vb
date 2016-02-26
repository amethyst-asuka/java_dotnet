Imports System
Imports System.Diagnostics
Imports System.Collections.Generic
Imports javax.swing
Imports javax.swing.border
Imports javax.swing.plaf
Imports javax.swing.plaf.basic
Imports javax.accessibility

'
' * Copyright (c) 2000, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' Class that manages a JLF awt.Window-descendant class's title bar.
	''' <p>
	''' This class assumes it will be created with a particular window
	''' decoration style, and that if the style changes, a new one will
	''' be created.
	''' 
	''' @author Terry Kellerman
	''' @since 1.4
	''' </summary>
	Friend Class MetalTitlePane
		Inherits JComponent

		Private Shared ReadOnly handyEmptyBorder As Border = New EmptyBorder(0,0,0,0)
		Private Const IMAGE_HEIGHT As Integer = 16
		Private Const IMAGE_WIDTH As Integer = 16

		''' <summary>
		''' PropertyChangeListener added to the JRootPane.
		''' </summary>
		Private propertyChangeListener As PropertyChangeListener

		''' <summary>
		''' JMenuBar, typically renders the system menu items.
		''' </summary>
		Private menuBar As JMenuBar
		''' <summary>
		''' Action used to close the Window.
		''' </summary>
		Private closeAction As Action

		''' <summary>
		''' Action used to iconify the Frame.
		''' </summary>
		Private iconifyAction As Action

		''' <summary>
		''' Action to restore the Frame size.
		''' </summary>
		Private restoreAction As Action

		''' <summary>
		''' Action to restore the Frame size.
		''' </summary>
		Private maximizeAction As Action

		''' <summary>
		''' Button used to maximize or restore the Frame.
		''' </summary>
		Private toggleButton As JButton

		''' <summary>
		''' Button used to maximize or restore the Frame.
		''' </summary>
		Private iconifyButton As JButton

		''' <summary>
		''' Button used to maximize or restore the Frame.
		''' </summary>
		Private closeButton As JButton

		''' <summary>
		''' Icon used for toggleButton when window is normal size.
		''' </summary>
		Private maximizeIcon As Icon

		''' <summary>
		''' Icon used for toggleButton when window is maximized.
		''' </summary>
		Private minimizeIcon As Icon

		''' <summary>
		''' Image used for the system menu icon
		''' </summary>
		Private systemIcon As Image

		''' <summary>
		''' Listens for changes in the state of the Window listener to update
		''' the state of the widgets.
		''' </summary>
		Private windowListener As WindowListener

		''' <summary>
		''' Window we're currently in.
		''' </summary>
		Private window As Window

		''' <summary>
		''' JRootPane rendering for.
		''' </summary>
		Private rootPane As JRootPane

		''' <summary>
		''' Room remaining in title for bumps.
		''' </summary>
		Private buttonsWidth As Integer

		''' <summary>
		''' Buffered Frame.state property. As state isn't bound, this is kept
		''' to determine when to avoid updating widgets.
		''' </summary>
		Private state As Integer

		''' <summary>
		''' MetalRootPaneUI that created us.
		''' </summary>
		Private rootPaneUI As MetalRootPaneUI


		' Colors
		Private inactiveBackground As Color = UIManager.getColor("inactiveCaption")
		Private inactiveForeground As Color = UIManager.getColor("inactiveCaptionText")
		Private inactiveShadow As Color = UIManager.getColor("inactiveCaptionBorder")
		Private activeBumpsHighlight As Color = MetalLookAndFeel.primaryControlHighlight
		Private activeBumpsShadow As Color = MetalLookAndFeel.primaryControlDarkShadow
		Private activeBackground As Color = Nothing
		Private activeForeground As Color = Nothing
		Private activeShadow As Color = Nothing

		' Bumps
		Private activeBumps As New MetalBumps(0, 0, activeBumpsHighlight, activeBumpsShadow, MetalLookAndFeel.primaryControl)
		Private inactiveBumps As New MetalBumps(0, 0, MetalLookAndFeel.controlHighlight, MetalLookAndFeel.controlDarkShadow, MetalLookAndFeel.control)


		Public Sub New(ByVal root As JRootPane, ByVal ui As MetalRootPaneUI)
			Me.rootPane = root
			rootPaneUI = ui

			state = -1

			installSubcomponents()
			determineColors()
			installDefaults()

			layout = createLayout()
		End Sub

		''' <summary>
		''' Uninstalls the necessary state.
		''' </summary>
		Private Sub uninstall()
			uninstallListeners()
			window = Nothing
			removeAll()
		End Sub

		''' <summary>
		''' Installs the necessary listeners.
		''' </summary>
		Private Sub installListeners()
			If window IsNot Nothing Then
				windowListener = createWindowListener()
				window.addWindowListener(windowListener)
				propertyChangeListener = createWindowPropertyChangeListener()
				window.addPropertyChangeListener(propertyChangeListener)
			End If
		End Sub

		''' <summary>
		''' Uninstalls the necessary listeners.
		''' </summary>
		Private Sub uninstallListeners()
			If window IsNot Nothing Then
				window.removeWindowListener(windowListener)
				window.removePropertyChangeListener(propertyChangeListener)
			End If
		End Sub

		''' <summary>
		''' Returns the <code>WindowListener</code> to add to the
		''' <code>Window</code>.
		''' </summary>
		Private Function createWindowListener() As WindowListener
			Return New WindowHandler(Me)
		End Function

		''' <summary>
		''' Returns the <code>PropertyChangeListener</code> to install on
		''' the <code>Window</code>.
		''' </summary>
		Private Function createWindowPropertyChangeListener() As PropertyChangeListener
			Return New PropertyChangeHandler(Me)
		End Function

		''' <summary>
		''' Returns the <code>JRootPane</code> this was created for.
		''' </summary>
		Public Property Overrides rootPane As JRootPane
			Get
				Return rootPane
			End Get
		End Property

		''' <summary>
		''' Returns the decoration style of the <code>JRootPane</code>.
		''' </summary>
		Private Property windowDecorationStyle As Integer
			Get
				Return rootPane.windowDecorationStyle
			End Get
		End Property

		Public Overrides Sub addNotify()
			MyBase.addNotify()

			uninstallListeners()

			window = SwingUtilities.getWindowAncestor(Me)
			If window IsNot Nothing Then
				If TypeOf window Is Frame Then
					state = CType(window, Frame).extendedState
				Else
					state = 0
				End If
				active = window.active
				installListeners()
				updateSystemIcon()
			End If
		End Sub

		Public Overrides Sub removeNotify()
			MyBase.removeNotify()

			uninstallListeners()
			window = Nothing
		End Sub

		''' <summary>
		''' Adds any sub-Components contained in the <code>MetalTitlePane</code>.
		''' </summary>
		Private Sub installSubcomponents()
			Dim decorationStyle As Integer = windowDecorationStyle
			If decorationStyle = JRootPane.FRAME Then
				createActions()
				menuBar = createMenuBar()
				add(menuBar)
				createButtons()
				add(iconifyButton)
				add(toggleButton)
				add(closeButton)
			ElseIf decorationStyle = JRootPane.PLAIN_DIALOG OrElse decorationStyle = JRootPane.INFORMATION_DIALOG OrElse decorationStyle = JRootPane.ERROR_DIALOG OrElse decorationStyle = JRootPane.COLOR_CHOOSER_DIALOG OrElse decorationStyle = JRootPane.FILE_CHOOSER_DIALOG OrElse decorationStyle = JRootPane.QUESTION_DIALOG OrElse decorationStyle = JRootPane.WARNING_DIALOG Then
				createActions()
				createButtons()
				add(closeButton)
			End If
		End Sub

		''' <summary>
		''' Determines the Colors to draw with.
		''' </summary>
		Private Sub determineColors()
			Select Case windowDecorationStyle
			Case JRootPane.FRAME
				activeBackground = UIManager.getColor("activeCaption")
				activeForeground = UIManager.getColor("activeCaptionText")
				activeShadow = UIManager.getColor("activeCaptionBorder")
			Case JRootPane.ERROR_DIALOG
				activeBackground = UIManager.getColor("OptionPane.errorDialog.titlePane.background")
				activeForeground = UIManager.getColor("OptionPane.errorDialog.titlePane.foreground")
				activeShadow = UIManager.getColor("OptionPane.errorDialog.titlePane.shadow")
			Case JRootPane.QUESTION_DIALOG, JRootPane.COLOR_CHOOSER_DIALOG, JRootPane.FILE_CHOOSER_DIALOG
				activeBackground = UIManager.getColor("OptionPane.questionDialog.titlePane.background")
				activeForeground = UIManager.getColor("OptionPane.questionDialog.titlePane.foreground")
				activeShadow = UIManager.getColor("OptionPane.questionDialog.titlePane.shadow")
			Case JRootPane.WARNING_DIALOG
				activeBackground = UIManager.getColor("OptionPane.warningDialog.titlePane.background")
				activeForeground = UIManager.getColor("OptionPane.warningDialog.titlePane.foreground")
				activeShadow = UIManager.getColor("OptionPane.warningDialog.titlePane.shadow")
			Case Else
				activeBackground = UIManager.getColor("activeCaption")
				activeForeground = UIManager.getColor("activeCaptionText")
				activeShadow = UIManager.getColor("activeCaptionBorder")
			End Select
			activeBumps.bumpColorsors(activeBumpsHighlight, activeBumpsShadow, activeBackground)
		End Sub

		''' <summary>
		''' Installs the fonts and necessary properties on the MetalTitlePane.
		''' </summary>
		Private Sub installDefaults()
			font = UIManager.getFont("InternalFrame.titleFont", locale)
		End Sub

		''' <summary>
		''' Uninstalls any previously installed UI values.
		''' </summary>
		Private Sub uninstallDefaults()
		End Sub

		''' <summary>
		''' Returns the <code>JMenuBar</code> displaying the appropriate
		''' system menu items.
		''' </summary>
		Protected Friend Overridable Function createMenuBar() As JMenuBar
			menuBar = New SystemMenuBar(Me)
			menuBar.focusable = False
			menuBar.borderPainted = True
			menuBar.add(createMenu())
			Return menuBar
		End Function

		''' <summary>
		''' Closes the Window.
		''' </summary>
		Private Sub close()
			Dim ___window As Window = window

			If ___window IsNot Nothing Then ___window.dispatchEvent(New WindowEvent(___window, WindowEvent.WINDOW_CLOSING))
		End Sub

		''' <summary>
		''' Iconifies the Frame.
		''' </summary>
		Private Sub iconify()
			Dim ___frame As Frame = frame
			If ___frame IsNot Nothing Then ___frame.extendedState = state Or Frame.ICONIFIED
		End Sub

		''' <summary>
		''' Maximizes the Frame.
		''' </summary>
		Private Sub maximize()
			Dim ___frame As Frame = frame
			If ___frame IsNot Nothing Then ___frame.extendedState = state Or Frame.MAXIMIZED_BOTH
		End Sub

		''' <summary>
		''' Restores the Frame size.
		''' </summary>
		Private Sub restore()
			Dim ___frame As Frame = frame

			If ___frame Is Nothing Then Return

			If (state And Frame.ICONIFIED) <> 0 Then
				___frame.extendedState = state And (Not Frame.ICONIFIED)
			Else
				___frame.extendedState = state And (Not Frame.MAXIMIZED_BOTH)
			End If
		End Sub

		''' <summary>
		''' Create the <code>Action</code>s that get associated with the
		''' buttons and menu items.
		''' </summary>
		Private Sub createActions()
			closeAction = New CloseAction(Me)
			If windowDecorationStyle = JRootPane.FRAME Then
				iconifyAction = New IconifyAction(Me)
				restoreAction = New RestoreAction(Me)
				maximizeAction = New MaximizeAction(Me)
			End If
		End Sub

		''' <summary>
		''' Returns the <code>JMenu</code> displaying the appropriate menu items
		''' for manipulating the Frame.
		''' </summary>
		Private Function createMenu() As JMenu
			Dim menu As New JMenu("")
			If windowDecorationStyle = JRootPane.FRAME Then addMenuItems(menu)
			Return menu
		End Function

		''' <summary>
		''' Adds the necessary <code>JMenuItem</code>s to the passed in menu.
		''' </summary>
		Private Sub addMenuItems(ByVal menu As JMenu)
			Dim locale As java.util.Locale = rootPane.locale
			Dim mi As JMenuItem = menu.add(restoreAction)
			Dim mnemonic As Integer = MetalUtils.getInt("MetalTitlePane.restoreMnemonic", -1)

			If mnemonic <> -1 Then mi.mnemonic = mnemonic

			mi = menu.add(iconifyAction)
			mnemonic = MetalUtils.getInt("MetalTitlePane.iconifyMnemonic", -1)
			If mnemonic <> -1 Then mi.mnemonic = mnemonic

			If Toolkit.defaultToolkit.isFrameStateSupported(Frame.MAXIMIZED_BOTH) Then
				mi = menu.add(maximizeAction)
				mnemonic = MetalUtils.getInt("MetalTitlePane.maximizeMnemonic", -1)
				If mnemonic <> -1 Then mi.mnemonic = mnemonic
			End If

			menu.add(New JSeparator)

			mi = menu.add(closeAction)
			mnemonic = MetalUtils.getInt("MetalTitlePane.closeMnemonic", -1)
			If mnemonic <> -1 Then mi.mnemonic = mnemonic
		End Sub

		''' <summary>
		''' Returns a <code>JButton</code> appropriate for placement on the
		''' TitlePane.
		''' </summary>
		Private Function createTitleButton() As JButton
			Dim button As New JButton

			button.focusPainted = False
			button.focusable = False
			button.opaque = True
			Return button
		End Function

		''' <summary>
		''' Creates the Buttons that will be placed on the TitlePane.
		''' </summary>
		Private Sub createButtons()
			closeButton = createTitleButton()
			closeButton.action = closeAction
			closeButton.text = Nothing
			closeButton.putClientProperty("paintActive", Boolean.TRUE)
			closeButton.border = handyEmptyBorder
			closeButton.putClientProperty(AccessibleContext.ACCESSIBLE_NAME_PROPERTY, "Close")
			closeButton.icon = UIManager.getIcon("InternalFrame.closeIcon")

			If windowDecorationStyle = JRootPane.FRAME Then
				maximizeIcon = UIManager.getIcon("InternalFrame.maximizeIcon")
				minimizeIcon = UIManager.getIcon("InternalFrame.minimizeIcon")

				iconifyButton = createTitleButton()
				iconifyButton.action = iconifyAction
				iconifyButton.text = Nothing
				iconifyButton.putClientProperty("paintActive", Boolean.TRUE)
				iconifyButton.border = handyEmptyBorder
				iconifyButton.putClientProperty(AccessibleContext.ACCESSIBLE_NAME_PROPERTY, "Iconify")
				iconifyButton.icon = UIManager.getIcon("InternalFrame.iconifyIcon")

				toggleButton = createTitleButton()
				toggleButton.action = restoreAction
				toggleButton.putClientProperty("paintActive", Boolean.TRUE)
				toggleButton.border = handyEmptyBorder
				toggleButton.putClientProperty(AccessibleContext.ACCESSIBLE_NAME_PROPERTY, "Maximize")
				toggleButton.icon = maximizeIcon
			End If
		End Sub

		''' <summary>
		''' Returns the <code>LayoutManager</code> that should be installed on
		''' the <code>MetalTitlePane</code>.
		''' </summary>
		Private Function createLayout() As LayoutManager
			Return New TitlePaneLayout(Me)
		End Function

		''' <summary>
		''' Updates state dependant upon the Window's active state.
		''' </summary>
		Private Property active As Boolean
			Set(ByVal isActive As Boolean)
				Dim activeB As Boolean? = If(isActive, Boolean.TRUE, Boolean.FALSE)
    
				closeButton.putClientProperty("paintActive", activeB)
				If windowDecorationStyle = JRootPane.FRAME Then
					iconifyButton.putClientProperty("paintActive", activeB)
					toggleButton.putClientProperty("paintActive", activeB)
				End If
				' Repaint the whole thing as the Borders that are used have
				' different colors for active vs inactive
				rootPane.repaint()
			End Set
		End Property

		''' <summary>
		''' Sets the state of the Window.
		''' </summary>
		Private Property state As Integer
			Set(ByVal state As Integer)
				stateate(state, False)
			End Set
		End Property

		''' <summary>
		''' Sets the state of the window. If <code>updateRegardless</code> is
		''' true and the state has not changed, this will update anyway.
		''' </summary>
		Private Sub setState(ByVal state As Integer, ByVal updateRegardless As Boolean)
			Dim w As Window = window

			If w IsNot Nothing AndAlso windowDecorationStyle = JRootPane.FRAME Then
				If Me.state = state AndAlso (Not updateRegardless) Then Return
				Dim ___frame As Frame = frame

				If ___frame IsNot Nothing Then
					Dim ___rootPane As JRootPane = rootPane

					If ((state And Frame.MAXIMIZED_BOTH) <> 0) AndAlso (___rootPane.border Is Nothing OrElse (TypeOf ___rootPane.border Is UIResource)) AndAlso ___frame.showing Then
						___rootPane.border = Nothing
					ElseIf (state And Frame.MAXIMIZED_BOTH) = 0 Then
						' This is a croak, if state becomes bound, this can
						' be nuked.
						rootPaneUI.installBorder(___rootPane)
					End If
					If ___frame.resizable Then
						If (state And Frame.MAXIMIZED_BOTH) <> 0 Then
							updateToggleButton(restoreAction, minimizeIcon)
							maximizeAction.enabled = False
							restoreAction.enabled = True
						Else
							updateToggleButton(maximizeAction, maximizeIcon)
							maximizeAction.enabled = True
							restoreAction.enabled = False
						End If
						If toggleButton.parent Is Nothing OrElse iconifyButton.parent Is Nothing Then
							add(toggleButton)
							add(iconifyButton)
							revalidate()
							repaint()
						End If
						toggleButton.text = Nothing
					Else
						maximizeAction.enabled = False
						restoreAction.enabled = False
						If toggleButton.parent IsNot Nothing Then
							remove(toggleButton)
							revalidate()
							repaint()
						End If
					End If
				Else
					' Not contained in a Frame
					maximizeAction.enabled = False
					restoreAction.enabled = False
					iconifyAction.enabled = False
					remove(toggleButton)
					remove(iconifyButton)
					revalidate()
					repaint()
				End If
				closeAction.enabled = True
				Me.state = state
			End If
		End Sub

		''' <summary>
		''' Updates the toggle button to contain the Icon <code>icon</code>, and
		''' Action <code>action</code>.
		''' </summary>
		Private Sub updateToggleButton(ByVal action As Action, ByVal icon As Icon)
			toggleButton.action = action
			toggleButton.icon = icon
			toggleButton.text = Nothing
		End Sub

		''' <summary>
		''' Returns the Frame rendering in. This will return null if the
		''' <code>JRootPane</code> is not contained in a <code>Frame</code>.
		''' </summary>
		Private Property frame As Frame
			Get
				Dim ___window As Window = window
    
				If TypeOf ___window Is Frame Then Return CType(___window, Frame)
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns the <code>Window</code> the <code>JRootPane</code> is
		''' contained in. This will return null if there is no parent ancestor
		''' of the <code>JRootPane</code>.
		''' </summary>
		Private Property window As Window
			Get
				Return window
			End Get
		End Property

		''' <summary>
		''' Returns the String to display as the title.
		''' </summary>
		Private Property title As String
			Get
				Dim w As Window = window
    
				If TypeOf w Is Frame Then
					Return CType(w, Frame).title
				ElseIf TypeOf w Is Dialog Then
					Return CType(w, Dialog).title
				End If
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Renders the TitlePane.
		''' </summary>
		Public Overrides Sub paintComponent(ByVal g As Graphics)
			' As state isn't bound, we need a convenience place to check
			' if it has changed. Changing the state typically changes the
			If frame IsNot Nothing Then state = frame.extendedState
			Dim ___rootPane As JRootPane = rootPane
			Dim ___window As Window = window
			Dim leftToRight As Boolean = If(___window Is Nothing, ___rootPane.componentOrientation.leftToRight, ___window.componentOrientation.leftToRight)
			Dim isSelected As Boolean = If(___window Is Nothing, True, ___window.active)
			Dim ___width As Integer = width
			Dim ___height As Integer = height

			Dim ___background As Color
			Dim ___foreground As Color
			Dim darkShadow As Color

			Dim bumps As MetalBumps

			If isSelected Then
				___background = activeBackground
				___foreground = activeForeground
				darkShadow = activeShadow
				bumps = activeBumps
			Else
				___background = inactiveBackground
				___foreground = inactiveForeground
				darkShadow = inactiveShadow
				bumps = inactiveBumps
			End If

			g.color = ___background
			g.fillRect(0, 0, ___width, ___height)

			g.color = darkShadow
			g.drawLine(0, ___height - 1, ___width, ___height -1)
			g.drawLine(0, 0, 0,0)
			g.drawLine(___width - 1, 0, ___width -1, 0)

			Dim xOffset As Integer = If(leftToRight, 5, ___width - 5)

			If windowDecorationStyle = JRootPane.FRAME Then xOffset += If(leftToRight, IMAGE_WIDTH + 5, - IMAGE_WIDTH - 5)

			Dim theTitle As String = title
			If theTitle IsNot Nothing Then
				Dim fm As FontMetrics = sun.swing.SwingUtilities2.getFontMetrics(___rootPane, g)

				g.color = ___foreground

				Dim yOffset As Integer = ((___height - fm.height) / 2) + fm.ascent

				Dim rect As New Rectangle(0, 0, 0, 0)
				If iconifyButton IsNot Nothing AndAlso iconifyButton.parent IsNot Nothing Then rect = iconifyButton.bounds
				Dim titleW As Integer

				If leftToRight Then
					If rect.x = 0 Then rect.x = ___window.width - ___window.insets.right-2
					titleW = rect.x - xOffset - 4
					theTitle = sun.swing.SwingUtilities2.clipStringIfNecessary(___rootPane, fm, theTitle, titleW)
				Else
					titleW = xOffset - rect.x - rect.width - 4
					theTitle = sun.swing.SwingUtilities2.clipStringIfNecessary(___rootPane, fm, theTitle, titleW)
					xOffset -= sun.swing.SwingUtilities2.stringWidth(___rootPane, fm, theTitle)
				End If
				Dim titleLength As Integer = sun.swing.SwingUtilities2.stringWidth(___rootPane, fm, theTitle)
				sun.swing.SwingUtilities2.drawString(___rootPane, g, theTitle, xOffset, yOffset)
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

		''' <summary>
		''' Actions used to <code>close</code> the <code>Window</code>.
		''' </summary>
		Private Class CloseAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As MetalTitlePane

			Public Sub New(ByVal outerInstance As MetalTitlePane)
					Me.outerInstance = outerInstance
				MyBase.New(UIManager.getString("MetalTitlePane.closeTitle", locale))
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				outerInstance.close()
			End Sub
		End Class


		''' <summary>
		''' Actions used to <code>iconfiy</code> the <code>Frame</code>.
		''' </summary>
		Private Class IconifyAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As MetalTitlePane

			Public Sub New(ByVal outerInstance As MetalTitlePane)
					Me.outerInstance = outerInstance
				MyBase.New(UIManager.getString("MetalTitlePane.iconifyTitle", locale))
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				outerInstance.iconify()
			End Sub
		End Class


		''' <summary>
		''' Actions used to <code>restore</code> the <code>Frame</code>.
		''' </summary>
		Private Class RestoreAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As MetalTitlePane

			Public Sub New(ByVal outerInstance As MetalTitlePane)
					Me.outerInstance = outerInstance
				MyBase.New(UIManager.getString("MetalTitlePane.restoreTitle", locale))
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				outerInstance.restore()
			End Sub
		End Class


		''' <summary>
		''' Actions used to <code>restore</code> the <code>Frame</code>.
		''' </summary>
		Private Class MaximizeAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As MetalTitlePane

			Public Sub New(ByVal outerInstance As MetalTitlePane)
					Me.outerInstance = outerInstance
				MyBase.New(UIManager.getString("MetalTitlePane.maximizeTitle", locale))
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				outerInstance.maximize()
			End Sub
		End Class


		''' <summary>
		''' Class responsible for drawing the system menu. Looks up the
		''' image to draw from the Frame associated with the
		''' <code>JRootPane</code>.
		''' </summary>
		Private Class SystemMenuBar
			Inherits JMenuBar

			Private ReadOnly outerInstance As MetalTitlePane

			Public Sub New(ByVal outerInstance As MetalTitlePane)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Sub paint(ByVal g As Graphics)
				If opaque Then
					g.color = background
					g.fillRect(0, 0, width, height)
				End If

				If outerInstance.systemIcon IsNot Nothing Then
					g.drawImage(outerInstance.systemIcon, 0, 0, IMAGE_WIDTH, IMAGE_HEIGHT, Nothing)
				Else
					Dim icon As Icon = UIManager.getIcon("InternalFrame.icon")

					If icon IsNot Nothing Then icon.paintIcon(Me, g, 0, 0)
				End If
			End Sub
			Public Property Overrides minimumSize As Dimension
				Get
					Return preferredSize
				End Get
			End Property
			Public Property Overrides preferredSize As Dimension
				Get
					Dim ___size As Dimension = MyBase.preferredSize
    
					Return New Dimension(Math.Max(IMAGE_WIDTH, ___size.width), Math.Max(___size.height, IMAGE_HEIGHT))
				End Get
			End Property
		End Class

		Private Class TitlePaneLayout
			Implements LayoutManager

			Private ReadOnly outerInstance As MetalTitlePane

			Public Sub New(ByVal outerInstance As MetalTitlePane)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal c As Component)
			End Sub
			Public Overridable Sub removeLayoutComponent(ByVal c As Component)
			End Sub
			Public Overridable Function preferredLayoutSize(ByVal c As Container) As Dimension
				Dim height As Integer = computeHeight()
				Return New Dimension(height, height)
			End Function

			Public Overridable Function minimumLayoutSize(ByVal c As Container) As Dimension
				Return preferredLayoutSize(c)
			End Function

			Private Function computeHeight() As Integer
				Dim fm As FontMetrics = outerInstance.rootPane.getFontMetrics(font)
				Dim fontHeight As Integer = fm.height
				fontHeight += 7
				Dim iconHeight As Integer = 0
				If outerInstance.windowDecorationStyle = JRootPane.FRAME Then iconHeight = IMAGE_HEIGHT

				Dim finalHeight As Integer = Math.Max(fontHeight, iconHeight)
				Return finalHeight
			End Function

			Public Overridable Sub layoutContainer(ByVal c As Container)
				Dim leftToRight As Boolean = If(outerInstance.window Is Nothing, outerInstance.rootPane.componentOrientation.leftToRight, outerInstance.window.componentOrientation.leftToRight)

				Dim w As Integer = outerInstance.width
				Dim x As Integer
				Dim y As Integer = 3
				Dim spacing As Integer
				Dim buttonHeight As Integer
				Dim buttonWidth As Integer

				If outerInstance.closeButton IsNot Nothing AndAlso outerInstance.closeButton.icon IsNot Nothing Then
					buttonHeight = outerInstance.closeButton.icon.iconHeight
					buttonWidth = outerInstance.closeButton.icon.iconWidth
				Else
					buttonHeight = IMAGE_HEIGHT
					buttonWidth = IMAGE_WIDTH
				End If

				' assumes all buttons have the same dimensions
				' these dimensions include the borders

				x = If(leftToRight, w, 0)

				spacing = 5
				x = If(leftToRight, spacing, w - buttonWidth - spacing)
				If outerInstance.menuBar IsNot Nothing Then outerInstance.menuBar.boundsnds(x, y, buttonWidth, buttonHeight)

				x = If(leftToRight, w, 0)
				spacing = 4
				x += If(leftToRight, -spacing -buttonWidth, spacing)
				If outerInstance.closeButton IsNot Nothing Then outerInstance.closeButton.boundsnds(x, y, buttonWidth, buttonHeight)

				If Not leftToRight Then x += buttonWidth

				If outerInstance.windowDecorationStyle = JRootPane.FRAME Then
					If Toolkit.defaultToolkit.isFrameStateSupported(Frame.MAXIMIZED_BOTH) Then
						If outerInstance.toggleButton.parent IsNot Nothing Then
							spacing = 10
							x += If(leftToRight, -spacing -buttonWidth, spacing)
							outerInstance.toggleButton.boundsnds(x, y, buttonWidth, buttonHeight)
							If Not leftToRight Then x += buttonWidth
						End If
					End If

					If outerInstance.iconifyButton IsNot Nothing AndAlso outerInstance.iconifyButton.parent IsNot Nothing Then
						spacing = 2
						x += If(leftToRight, -spacing -buttonWidth, spacing)
						outerInstance.iconifyButton.boundsnds(x, y, buttonWidth, buttonHeight)
						If Not leftToRight Then x += buttonWidth
					End If
				End If
				outerInstance.buttonsWidth = If(leftToRight, w - x, x)
			End Sub
		End Class



		''' <summary>
		''' PropertyChangeListener installed on the Window. Updates the necessary
		''' state as the state of the Window changes.
		''' </summary>
		Private Class PropertyChangeHandler
			Implements PropertyChangeListener

			Private ReadOnly outerInstance As MetalTitlePane

			Public Sub New(ByVal outerInstance As MetalTitlePane)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub propertyChange(ByVal pce As PropertyChangeEvent)
				Dim name As String = pce.propertyName

				' Frame.state isn't currently bound.
				If "resizable".Equals(name) OrElse "state".Equals(name) Then
					Dim frame As Frame = outerInstance.frame

					If frame IsNot Nothing Then outerInstance.stateate(frame.extendedState, True)
					If "resizable".Equals(name) Then outerInstance.rootPane.repaint()
				ElseIf "title".Equals(name) Then
					repaint()
				ElseIf "componentOrientation" = name Then
					outerInstance.revalidate()
					repaint()
				ElseIf "iconImage" = name Then
					outerInstance.updateSystemIcon()
					outerInstance.revalidate()
					repaint()
				End If
			End Sub
		End Class

		''' <summary>
		''' Update the image used for the system icon
		''' </summary>
		Private Sub updateSystemIcon()
			Dim ___window As Window = window
			If ___window Is Nothing Then
				systemIcon = Nothing
				Return
			End If
			Dim icons As IList(Of Image) = ___window.iconImages
			Debug.Assert(icons IsNot Nothing)

			If icons.Count = 0 Then
				systemIcon = Nothing
			ElseIf icons.Count = 1 Then
				systemIcon = icons(0)
			Else
				systemIcon = sun.awt.SunToolkit.getScaledIconImage(icons, IMAGE_WIDTH, IMAGE_HEIGHT)
			End If
		End Sub


		''' <summary>
		''' WindowListener installed on the Window, updates the state as necessary.
		''' </summary>
		Private Class WindowHandler
			Inherits WindowAdapter

			Private ReadOnly outerInstance As MetalTitlePane

			Public Sub New(ByVal outerInstance As MetalTitlePane)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub windowActivated(ByVal ev As WindowEvent)
				outerInstance.active = True
			End Sub

			Public Overridable Sub windowDeactivated(ByVal ev As WindowEvent)
				outerInstance.active = False
			End Sub
		End Class
	End Class

End Namespace
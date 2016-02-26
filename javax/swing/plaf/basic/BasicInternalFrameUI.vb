Imports System
Imports javax.swing
Imports javax.swing.plaf
Imports javax.swing.event

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
	''' A basic L&amp;F implementation of JInternalFrame.
	''' 
	''' @author David Kloba
	''' @author Rich Schiavi
	''' </summary>
	Public Class BasicInternalFrameUI
		Inherits InternalFrameUI

		Protected Friend frame As JInternalFrame

		Private handler As Handler
		Protected Friend borderListener As MouseInputAdapter
		Protected Friend propertyChangeListener As PropertyChangeListener
		Protected Friend internalFrameLayout As LayoutManager
		Protected Friend componentListener As ComponentListener
		Protected Friend glassPaneDispatcher As MouseInputListener
		Private internalFrameListener As InternalFrameListener

		Protected Friend northPane As JComponent
		Protected Friend southPane As JComponent
		Protected Friend westPane As JComponent
		Protected Friend eastPane As JComponent

		Protected Friend titlePane As BasicInternalFrameTitlePane ' access needs this

		Private Shared sharedDesktopManager As DesktopManager
		Private componentListenerAdded As Boolean = False

		Private parentBounds As Rectangle

		Private dragging As Boolean = False
		Private resizing As Boolean = False

		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend openMenuKey As KeyStroke

		Private keyBindingRegistered As Boolean = False
		Private keyBindingActive As Boolean = False

	'///////////////////////////////////////////////////////////////////////////
	' ComponentUI Interface Implementation methods
	'///////////////////////////////////////////////////////////////////////////
		Public Shared Function createUI(ByVal b As JComponent) As ComponentUI
			Return New BasicInternalFrameUI(CType(b, JInternalFrame))
		End Function

		Public Sub New(ByVal b As JInternalFrame)
			Dim laf As LookAndFeel = UIManager.lookAndFeel
			If TypeOf laf Is BasicLookAndFeel Then CType(laf, BasicLookAndFeel).installAWTEventListener()
		End Sub

		Public Overridable Sub installUI(ByVal c As JComponent)

			frame = CType(c, JInternalFrame)

			installDefaults()
			installListeners()
			installComponents()
			installKeyboardActions()

			LookAndFeel.installProperty(frame, "opaque", Boolean.TRUE)
		End Sub

		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			If c IsNot frame Then Throw New IllegalComponentStateException(Me & " was asked to deinstall() " & c & " when it only knows about " & frame & ".")

			uninstallKeyboardActions()
			uninstallComponents()
			uninstallListeners()
			uninstallDefaults()
			updateFrameCursor()
			handler = Nothing
			frame = Nothing
		End Sub

		Protected Friend Overridable Sub installDefaults()
			Dim frameIcon As Icon = frame.frameIcon
			If frameIcon Is Nothing OrElse TypeOf frameIcon Is UIResource Then frame.frameIcon = UIManager.getIcon("InternalFrame.icon")

			' Enable the content pane to inherit background color from its
			' parent by setting its background color to null.
			Dim contentPane As Container = frame.contentPane
			If contentPane IsNot Nothing Then
			  Dim bg As Color = contentPane.background
			  If TypeOf bg Is UIResource Then contentPane.background = Nothing
			End If
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			frame.layout = internalFrameLayout = createLayoutManager()
			frame.background = UIManager.lookAndFeelDefaults.getColor("control")

			LookAndFeel.installBorder(frame, "InternalFrame.border")

		End Sub
		Protected Friend Overridable Sub installKeyboardActions()
			createInternalFrameListener()
			If internalFrameListener IsNot Nothing Then frame.addInternalFrameListener(internalFrameListener)

			LazyActionMap.installLazyActionMap(frame, GetType(BasicInternalFrameUI), "InternalFrame.actionMap")
		End Sub

		Friend Shared Sub loadActionMap(ByVal map As LazyActionMap)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			map.put(New sun.swing.UIAction("showSystemMenu")
	'		{
	'			public void actionPerformed(ActionEvent evt)
	'			{
	'				JInternalFrame iFrame = (JInternalFrame)evt.getSource();
	'				if (iFrame.getUI() instanceof BasicInternalFrameUI)
	'				{
	'					JComponent comp = ((BasicInternalFrameUI) iFrame.getUI()).getNorthPane();
	'					if (comp instanceof BasicInternalFrameTitlePane)
	'					{
	'						((BasicInternalFrameTitlePane)comp).showSystemMenu();
	'					}
	'				}
	'			}
	'
	'			public boolean isEnabled(Object sender)
	'			{
	'				if (sender instanceof JInternalFrame)
	'				{
	'					JInternalFrame iFrame = (JInternalFrame)sender;
	'					if (iFrame.getUI() instanceof BasicInternalFrameUI)
	'					{
	'						Return ((BasicInternalFrameUI)iFrame.getUI()).isKeyBindingActive();
	'					}
	'				}
	'				Return False;
	'			}
	'		});

			' Set the ActionMap's parent to the Auditory Feedback Action Map
			BasicLookAndFeel.installAudioActionMap(map)
		End Sub

		Protected Friend Overridable Sub installComponents()
			northPane = createNorthPane(frame)
			southPane = createSouthPane(frame)
			eastPane = createEastPane(frame)
			westPane = createWestPane(frame)
		End Sub

		''' <summary>
		''' @since 1.3
		''' </summary>
		Protected Friend Overridable Sub installListeners()
			borderListener = createBorderListener(frame)
			propertyChangeListener = createPropertyChangeListener()
			frame.addPropertyChangeListener(propertyChangeListener)
			installMouseHandlers(frame)
			glassPaneDispatcher = createGlassPaneDispatcher()
			If glassPaneDispatcher IsNot Nothing Then
				frame.glassPane.addMouseListener(glassPaneDispatcher)
				frame.glassPane.addMouseMotionListener(glassPaneDispatcher)
			End If
			componentListener = createComponentListener()
			If frame.parent IsNot Nothing Then parentBounds = frame.parent.bounds
			If (frame.parent IsNot Nothing) AndAlso (Not componentListenerAdded) Then
				frame.parent.addComponentListener(componentListener)
				componentListenerAdded = True
			End If
		End Sub

		' Provide a FocusListener to listen for a WINDOW_LOST_FOCUS event,
		' so that a resize can be cancelled if the focus is lost while resizing
		' when an Alt-Tab, modal dialog popup, iconify, dispose, or remove
		' of the internal frame occurs.
		Private Property windowFocusListener As WindowFocusListener
			Get
				Return handler
			End Get
		End Property

		' Cancel a resize in progress by calling finishMouseReleased().
		Private Sub cancelResize()
			If resizing Then
				If TypeOf borderListener Is BorderListener Then CType(borderListener, BorderListener).finishMouseReleased()
			End If
		End Sub

		Private Property handler As Handler
			Get
				If handler Is Nothing Then handler = New Handler(Me)
				Return handler
			End Get
		End Property

		Friend Overridable Function getInputMap(ByVal condition As Integer) As InputMap
			If condition = JComponent.WHEN_IN_FOCUSED_WINDOW Then Return createInputMap(condition)
			Return Nothing
		End Function

		Friend Overridable Function createInputMap(ByVal condition As Integer) As InputMap
			If condition = JComponent.WHEN_IN_FOCUSED_WINDOW Then
				Dim bindings As Object() = CType(sun.swing.DefaultLookup.get(frame, Me, "InternalFrame.windowBindings"), Object())

				If bindings IsNot Nothing Then Return LookAndFeel.makeComponentInputMap(frame, bindings)
			End If
			Return Nothing
		End Function

		Protected Friend Overridable Sub uninstallDefaults()
			Dim frameIcon As Icon = frame.frameIcon
			If TypeOf frameIcon Is UIResource Then frame.frameIcon = Nothing
			internalFrameLayout = Nothing
			frame.layout = Nothing
			LookAndFeel.uninstallBorder(frame)
		End Sub

		Protected Friend Overridable Sub uninstallComponents()
			northPane = Nothing
			southPane = Nothing
			eastPane = Nothing
			westPane = Nothing
			If titlePane IsNot Nothing Then titlePane.uninstallDefaults()
			titlePane = Nothing
		End Sub

		''' <summary>
		''' @since 1.3
		''' </summary>
		Protected Friend Overridable Sub uninstallListeners()
			If (frame.parent IsNot Nothing) AndAlso componentListenerAdded Then
				frame.parent.removeComponentListener(componentListener)
				componentListenerAdded = False
			End If
			componentListener = Nothing
		  If glassPaneDispatcher IsNot Nothing Then
			  frame.glassPane.removeMouseListener(glassPaneDispatcher)
			  frame.glassPane.removeMouseMotionListener(glassPaneDispatcher)
			  glassPaneDispatcher = Nothing
		  End If
		  deinstallMouseHandlers(frame)
		  frame.removePropertyChangeListener(propertyChangeListener)
		  propertyChangeListener = Nothing
		  borderListener = Nothing
		End Sub

		Protected Friend Overridable Sub uninstallKeyboardActions()
			If internalFrameListener IsNot Nothing Then frame.removeInternalFrameListener(internalFrameListener)
			internalFrameListener = Nothing

			SwingUtilities.replaceUIInputMap(frame, JComponent.WHEN_IN_FOCUSED_WINDOW, Nothing)
			SwingUtilities.replaceUIActionMap(frame, Nothing)

		End Sub

		Friend Overridable Sub updateFrameCursor()
			If resizing Then Return
			Dim s As Cursor = frame.lastCursor
			If s Is Nothing Then s = Cursor.getPredefinedCursor(Cursor.DEFAULT_CURSOR)
			frame.cursor = s
		End Sub

		Protected Friend Overridable Function createLayoutManager() As LayoutManager
			Return handler
		End Function

		Protected Friend Overridable Function createPropertyChangeListener() As PropertyChangeListener
			Return handler
		End Function



		Public Overridable Function getPreferredSize(ByVal x As JComponent) As Dimension
			If frame Is x Then Return frame.layout.preferredLayoutSize(x)
			Return New Dimension(100, 100)
		End Function

		Public Overridable Function getMinimumSize(ByVal x As JComponent) As Dimension
			If frame Is x Then Return frame.layout.minimumLayoutSize(x)
			Return New Dimension(0, 0)
		End Function

		Public Overridable Function getMaximumSize(ByVal x As JComponent) As Dimension
			Return New Dimension(Integer.MaxValue, Integer.MaxValue)
		End Function



		''' <summary>
		''' Installs necessary mouse handlers on <code>newPane</code>
		''' and adds it to the frame.
		''' Reverse process for the <code>currentPane</code>.
		''' </summary>
		Protected Friend Overridable Sub replacePane(ByVal currentPane As JComponent, ByVal newPane As JComponent)
			If currentPane IsNot Nothing Then
				deinstallMouseHandlers(currentPane)
				frame.remove(currentPane)
			End If
			If newPane IsNot Nothing Then
			   frame.add(newPane)
			   installMouseHandlers(newPane)
			End If
		End Sub

		Protected Friend Overridable Sub deinstallMouseHandlers(ByVal c As JComponent)
		  c.removeMouseListener(borderListener)
		  c.removeMouseMotionListener(borderListener)
		End Sub

		Protected Friend Overridable Sub installMouseHandlers(ByVal c As JComponent)
		  c.addMouseListener(borderListener)
		  c.addMouseMotionListener(borderListener)
		End Sub

		Protected Friend Overridable Function createNorthPane(ByVal w As JInternalFrame) As JComponent
		  titlePane = New BasicInternalFrameTitlePane(w)
		  Return titlePane
		End Function


		Protected Friend Overridable Function createSouthPane(ByVal w As JInternalFrame) As JComponent
			Return Nothing
		End Function

		Protected Friend Overridable Function createWestPane(ByVal w As JInternalFrame) As JComponent
			Return Nothing
		End Function

		Protected Friend Overridable Function createEastPane(ByVal w As JInternalFrame) As JComponent
			Return Nothing
		End Function


		Protected Friend Overridable Function createBorderListener(ByVal w As JInternalFrame) As MouseInputAdapter
			Return New BorderListener(Me)
		End Function

		Protected Friend Overridable Sub createInternalFrameListener()
			internalFrameListener = handler
		End Sub

		Protected Friend Property keyBindingRegistered As Boolean
			Get
			  Return keyBindingRegistered
			End Get
			Set(ByVal b As Boolean)
			  keyBindingRegistered = b
			End Set
		End Property


		Public Property keyBindingActive As Boolean
			Get
			  Return keyBindingActive
			End Get
			Set(ByVal b As Boolean)
			  keyBindingActive = b
			End Set
		End Property



		Protected Friend Overridable Sub setupMenuOpenKey()
			' PENDING(hania): Why are these WHEN_IN_FOCUSED_WINDOWs? Shouldn't
			' they be WHEN_ANCESTOR_OF_FOCUSED_COMPONENT?
			' Also, no longer registering on the desktopicon, the previous
			' action did nothing.
			Dim map As InputMap = getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW)
			SwingUtilities.replaceUIInputMap(frame, JComponent.WHEN_IN_FOCUSED_WINDOW, map)
			'ActionMap actionMap = getActionMap();
			'SwingUtilities.replaceUIActionMap(frame, actionMap);
		End Sub

		Protected Friend Overridable Sub setupMenuCloseKey()
		End Sub

		Public Overridable Property northPane As JComponent
			Get
				Return northPane
			End Get
			Set(ByVal c As JComponent)
				If northPane IsNot Nothing AndAlso TypeOf northPane Is BasicInternalFrameTitlePane Then CType(northPane, BasicInternalFrameTitlePane).uninstallListeners()
				replacePane(northPane, c)
				northPane = c
				If TypeOf c Is BasicInternalFrameTitlePane Then titlePane = CType(c, BasicInternalFrameTitlePane)
			End Set
		End Property


		Public Overridable Property southPane As JComponent
			Get
				Return southPane
			End Get
			Set(ByVal c As JComponent)
				southPane = c
			End Set
		End Property


		Public Overridable Property westPane As JComponent
			Get
				Return westPane
			End Get
			Set(ByVal c As JComponent)
				westPane = c
			End Set
		End Property


		Public Overridable Property eastPane As JComponent
			Get
				Return eastPane
			End Get
			Set(ByVal c As JComponent)
				eastPane = c
			End Set
		End Property


		Public Class InternalFramePropertyChangeListener
			Implements PropertyChangeListener

			Private ReadOnly outerInstance As BasicInternalFrameUI

			Public Sub New(ByVal outerInstance As BasicInternalFrameUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.
			''' <summary>
			''' Detects changes in state from the JInternalFrame and handles
			''' actions.
			''' </summary>
			Public Overridable Sub propertyChange(ByVal evt As PropertyChangeEvent)
				outerInstance.handler.propertyChange(evt)
			End Sub
		End Class

	  Public Class InternalFrameLayout
		  Implements LayoutManager

		  Private ReadOnly outerInstance As BasicInternalFrameUI

		  Public Sub New(ByVal outerInstance As BasicInternalFrameUI)
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

	'/ DesktopManager methods
		''' <summary>
		''' Returns the proper DesktopManager. Calls getDesktopPane() to
		''' find the JDesktop component and returns the desktopManager from
		''' it. If this fails, it will return a default DesktopManager that
		''' should work in arbitrary parents.
		''' </summary>
		Protected Friend Overridable Property desktopManager As DesktopManager
			Get
				If frame.desktopPane IsNot Nothing AndAlso frame.desktopPane.desktopManager IsNot Nothing Then Return frame.desktopPane.desktopManager
				If sharedDesktopManager Is Nothing Then sharedDesktopManager = createDesktopManager()
				Return sharedDesktopManager
			End Get
		End Property

		Protected Friend Overridable Function createDesktopManager() As DesktopManager
		  Return New DefaultDesktopManager
		End Function

		''' <summary>
		''' This method is called when the user wants to close the frame.
		''' The <code>playCloseSound</code> Action is fired.
		''' This action is delegated to the desktopManager.
		''' </summary>
		Protected Friend Overridable Sub closeFrame(ByVal f As JInternalFrame)
			' Internal Frame Auditory Cue Activation
			BasicLookAndFeel.playSound(frame,"InternalFrame.closeSound")
			' delegate to desktop manager
			desktopManager.closeFrame(f)
		End Sub

		''' <summary>
		''' This method is called when the user wants to maximize the frame.
		''' The <code>playMaximizeSound</code> Action is fired.
		''' This action is delegated to the desktopManager.
		''' </summary>
		Protected Friend Overridable Sub maximizeFrame(ByVal f As JInternalFrame)
			' Internal Frame Auditory Cue Activation
			BasicLookAndFeel.playSound(frame,"InternalFrame.maximizeSound")
			' delegate to desktop manager
			desktopManager.maximizeFrame(f)
		End Sub

		''' <summary>
		''' This method is called when the user wants to minimize the frame.
		''' The <code>playRestoreDownSound</code> Action is fired.
		''' This action is delegated to the desktopManager.
		''' </summary>
		Protected Friend Overridable Sub minimizeFrame(ByVal f As JInternalFrame)
			' Internal Frame Auditory Cue Activation
			If Not f.icon Then BasicLookAndFeel.playSound(frame,"InternalFrame.restoreDownSound")
			' delegate to desktop manager
			desktopManager.minimizeFrame(f)
		End Sub

		''' <summary>
		''' This method is called when the user wants to iconify the frame.
		''' The <code>playMinimizeSound</code> Action is fired.
		''' This action is delegated to the desktopManager.
		''' </summary>
		Protected Friend Overridable Sub iconifyFrame(ByVal f As JInternalFrame)
			' Internal Frame Auditory Cue Activation
			BasicLookAndFeel.playSound(frame, "InternalFrame.minimizeSound")
			' delegate to desktop manager
			desktopManager.iconifyFrame(f)
		End Sub

		''' <summary>
		''' This method is called when the user wants to deiconify the frame.
		''' The <code>playRestoreUpSound</code> Action is fired.
		''' This action is delegated to the desktopManager.
		''' </summary>
		Protected Friend Overridable Sub deiconifyFrame(ByVal f As JInternalFrame)
			' Internal Frame Auditory Cue Activation
			If Not f.maximum Then BasicLookAndFeel.playSound(frame, "InternalFrame.restoreUpSound")
			' delegate to desktop manager
			desktopManager.deiconifyFrame(f)
		End Sub

		''' <summary>
		''' This method is called when the frame becomes selected.
		''' This action is delegated to the desktopManager.
		''' </summary>
		Protected Friend Overridable Sub activateFrame(ByVal f As JInternalFrame)
			desktopManager.activateFrame(f)
		End Sub
		''' <summary>
		''' This method is called when the frame is no longer selected.
		''' This action is delegated to the desktopManager.
		''' </summary>
		Protected Friend Overridable Sub deactivateFrame(ByVal f As JInternalFrame)
			desktopManager.deactivateFrame(f)
		End Sub

		'///////////////////////////////////////////////////////////////////////
		'/ Border Listener Class
		'///////////////////////////////////////////////////////////////////////
		''' <summary>
		''' Listens for border adjustments.
		''' </summary>
		Protected Friend Class BorderListener
			Inherits MouseInputAdapter
			Implements SwingConstants

			Private ReadOnly outerInstance As BasicInternalFrameUI

			Public Sub New(ByVal outerInstance As BasicInternalFrameUI)
				Me.outerInstance = outerInstance
			End Sub

			' _x & _y are the mousePressed location in absolute coordinate system
			Friend _x, _y As Integer
			' __x & __y are the mousePressed location in source view's coordinate system
			Friend __x, __y As Integer
			Friend startingBounds As Rectangle
			Friend resizeDir As Integer


			Protected Friend ReadOnly RESIZE_NONE As Integer = 0
			Private discardRelease As Boolean = False

			Friend resizeCornerSize As Integer = 16

			Public Overridable Sub mouseClicked(ByVal e As MouseEvent)
				If e.clickCount > 1 AndAlso e.source Is outerInstance.northPane Then
					If outerInstance.frame.iconifiable AndAlso outerInstance.frame.icon Then
						Try
							outerInstance.frame.icon = False
						Catch e2 As PropertyVetoException
						End Try
					ElseIf outerInstance.frame.maximizable Then
						If Not outerInstance.frame.maximum Then
							Try
								outerInstance.frame.maximum = True
							Catch e2 As PropertyVetoException
							End Try
						Else
							Try
								outerInstance.frame.maximum = False
							Catch e3 As PropertyVetoException
							End Try
						End If
					End If
				End If
			End Sub

			' Factor out finishMouseReleased() from mouseReleased(), so that
			' it can be called by cancelResize() without passing it a null
			' MouseEvent.
			Friend Overridable Sub finishMouseReleased()
			   If discardRelease Then
				 discardRelease = False
				 Return
			   End If
				If resizeDir = RESIZE_NONE Then
					outerInstance.desktopManager.endDraggingFrame(outerInstance.frame)
					outerInstance.dragging = False
				Else
					' Remove the WindowFocusListener for handling a
					' WINDOW_LOST_FOCUS event with a cancelResize().
					Dim windowAncestor As Window = SwingUtilities.getWindowAncestor(outerInstance.frame)
					If windowAncestor IsNot Nothing Then windowAncestor.removeWindowFocusListener(outerInstance.windowFocusListener)
					Dim c As Container = outerInstance.frame.topLevelAncestor
					If TypeOf c Is RootPaneContainer Then
						Dim glassPane As Component = CType(c, RootPaneContainer).glassPane
						glassPane.cursor = Cursor.getPredefinedCursor(Cursor.DEFAULT_CURSOR)
						glassPane.visible = False
					End If
					outerInstance.desktopManager.endResizingFrame(outerInstance.frame)
					outerInstance.resizing = False
					outerInstance.updateFrameCursor()
				End If
				_x = 0
				_y = 0
				__x = 0
				__y = 0
				startingBounds = Nothing
				resizeDir = RESIZE_NONE
				' Set discardRelease to true, so that only a mousePressed()
				' which sets it to false, will allow entry to the above code
				' for finishing a resize.
				discardRelease = True
			End Sub

			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
				finishMouseReleased()
			End Sub

			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				Dim p As Point = SwingUtilities.convertPoint(CType(e.source, Component), e.x, e.y, Nothing)
				__x = e.x
				__y = e.y
				_x = p.x
				_y = p.y
				startingBounds = outerInstance.frame.bounds
				resizeDir = RESIZE_NONE
				discardRelease = False

				Try
					outerInstance.frame.selected = True
				Catch e1 As PropertyVetoException
				End Try

				Dim i As Insets = outerInstance.frame.insets

				Dim ep As New Point(__x, __y)
				If e.source Is outerInstance.northPane Then
					Dim np As Point = outerInstance.northPane.location
					ep.x += np.x
					ep.y += np.y
				End If

				If e.source Is outerInstance.northPane Then
					If ep.x > i.left AndAlso ep.y > i.top AndAlso ep.x < outerInstance.frame.width - i.right Then
						outerInstance.desktopManager.beginDraggingFrame(outerInstance.frame)
						outerInstance.dragging = True
						Return
					End If
				End If
				If Not outerInstance.frame.resizable Then Return

				If e.source Is outerInstance.frame OrElse e.source Is outerInstance.northPane Then
					If ep.x <= i.left Then
						If ep.y < resizeCornerSize + i.top Then
							resizeDir = NORTH_WEST
						ElseIf ep.y > outerInstance.frame.height - resizeCornerSize - i.bottom Then
							resizeDir = SOUTH_WEST
						Else
							resizeDir = WEST
						End If
					ElseIf ep.x >= outerInstance.frame.width - i.right Then
						If ep.y < resizeCornerSize + i.top Then
							resizeDir = NORTH_EAST
						ElseIf ep.y > outerInstance.frame.height - resizeCornerSize - i.bottom Then
							resizeDir = SOUTH_EAST
						Else
							resizeDir = EAST
						End If
					ElseIf ep.y <= i.top Then
						If ep.x < resizeCornerSize + i.left Then
							resizeDir = NORTH_WEST
						ElseIf ep.x > outerInstance.frame.width - resizeCornerSize - i.right Then
							resizeDir = NORTH_EAST
						Else
							resizeDir = NORTH
						End If
					ElseIf ep.y >= outerInstance.frame.height - i.bottom Then
						If ep.x < resizeCornerSize + i.left Then
							resizeDir = SOUTH_WEST
						ElseIf ep.x > outerInstance.frame.width - resizeCornerSize - i.right Then
							resizeDir = SOUTH_EAST
						Else
						  resizeDir = SOUTH
						End If
					Else
	'                   the mouse press happened inside the frame, not in the
	'                     border 
					  discardRelease = True
					  Return
					End If
					Dim s As Cursor = Cursor.getPredefinedCursor(Cursor.DEFAULT_CURSOR)
					Select Case resizeDir
					Case SOUTH
					  s = Cursor.getPredefinedCursor(Cursor.S_RESIZE_CURSOR)
					Case NORTH
					  s = Cursor.getPredefinedCursor(Cursor.N_RESIZE_CURSOR)
					Case WEST
					  s = Cursor.getPredefinedCursor(Cursor.W_RESIZE_CURSOR)
					Case EAST
					  s = Cursor.getPredefinedCursor(Cursor.E_RESIZE_CURSOR)
					Case SOUTH_EAST
					  s = Cursor.getPredefinedCursor(Cursor.SE_RESIZE_CURSOR)
					Case SOUTH_WEST
					  s = Cursor.getPredefinedCursor(Cursor.SW_RESIZE_CURSOR)
					Case NORTH_WEST
					  s = Cursor.getPredefinedCursor(Cursor.NW_RESIZE_CURSOR)
					Case NORTH_EAST
					  s = Cursor.getPredefinedCursor(Cursor.NE_RESIZE_CURSOR)
					End Select
					Dim c As Container = outerInstance.frame.topLevelAncestor
					If TypeOf c Is RootPaneContainer Then
						Dim glassPane As Component = CType(c, RootPaneContainer).glassPane
						glassPane.visible = True
						glassPane.cursor = s
					End If
					outerInstance.desktopManager.beginResizingFrame(outerInstance.frame, resizeDir)
					outerInstance.resizing = True
					' Add the WindowFocusListener for handling a
					' WINDOW_LOST_FOCUS event with a cancelResize().
					Dim windowAncestor As Window = SwingUtilities.getWindowAncestor(outerInstance.frame)
					If windowAncestor IsNot Nothing Then windowAncestor.addWindowFocusListener(outerInstance.windowFocusListener)
					Return
				End If
			End Sub

			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)

				If startingBounds Is Nothing Then Return

				Dim p As Point = SwingUtilities.convertPoint(CType(e.source, Component), e.x, e.y, Nothing)
				Dim deltaX As Integer = _x - p.x
				Dim deltaY As Integer = _y - p.y
				Dim min As Dimension = outerInstance.frame.minimumSize
				Dim max As Dimension = outerInstance.frame.maximumSize
				Dim newX, newY, newW, newH As Integer
				Dim i As Insets = outerInstance.frame.insets

				' Handle a MOVE
				If outerInstance.dragging Then
					If outerInstance.frame.maximum OrElse ((e.modifiers And InputEvent.BUTTON1_MASK) <> InputEvent.BUTTON1_MASK) Then Return
					Dim pWidth, pHeight As Integer
					Dim s As Dimension = outerInstance.frame.parent.size
					pWidth = s.width
					pHeight = s.height


					newX = startingBounds.x - deltaX
					newY = startingBounds.y - deltaY

					' Make sure we stay in-bounds
					If newX + i.left <= -__x Then newX = -__x - i.left + 1
					If newY + i.top <= -__y Then newY = -__y - i.top + 1
					If newX + __x + i.right >= pWidth Then newX = pWidth - __x - i.right - 1
					If newY + __y + i.bottom >= pHeight Then newY = pHeight - __y - i.bottom - 1

					outerInstance.desktopManager.dragFrame(outerInstance.frame, newX, newY)
					Return
				End If

				If Not outerInstance.frame.resizable Then Return

				newX = outerInstance.frame.x
				newY = outerInstance.frame.y
				newW = outerInstance.frame.width
				newH = outerInstance.frame.height

				outerInstance.parentBounds = outerInstance.frame.parent.bounds

				Select Case resizeDir
				Case RESIZE_NONE
					Return
				Case NORTH
					If startingBounds.height + deltaY < min.height Then
						deltaY = -(startingBounds.height - min.height)
					ElseIf startingBounds.height + deltaY > max.height Then
						deltaY = max.height - startingBounds.height
					End If
					If startingBounds.y - deltaY < 0 Then deltaY = startingBounds.y

					newX = startingBounds.x
					newY = startingBounds.y - deltaY
					newW = startingBounds.width
					newH = startingBounds.height + deltaY
				Case NORTH_EAST
					If startingBounds.height + deltaY < min.height Then
						deltaY = -(startingBounds.height - min.height)
					ElseIf startingBounds.height + deltaY > max.height Then
						deltaY = max.height - startingBounds.height
					End If
					If startingBounds.y - deltaY < 0 Then deltaY = startingBounds.y

					If startingBounds.width - deltaX < min.width Then
						deltaX = startingBounds.width - min.width
					ElseIf startingBounds.width - deltaX > max.width Then
						deltaX = -(max.width - startingBounds.width)
					End If
					If startingBounds.x + startingBounds.width - deltaX > outerInstance.parentBounds.width Then deltaX = startingBounds.x + startingBounds.width - outerInstance.parentBounds.width

					newX = startingBounds.x
					newY = startingBounds.y - deltaY
					newW = startingBounds.width - deltaX
					newH = startingBounds.height + deltaY
				Case EAST
					If startingBounds.width - deltaX < min.width Then
						deltaX = startingBounds.width - min.width
					ElseIf startingBounds.width - deltaX > max.width Then
						deltaX = -(max.width - startingBounds.width)
					End If
					If startingBounds.x + startingBounds.width - deltaX > outerInstance.parentBounds.width Then deltaX = startingBounds.x + startingBounds.width - outerInstance.parentBounds.width

					newW = startingBounds.width - deltaX
					newH = startingBounds.height
				Case SOUTH_EAST
					If startingBounds.width - deltaX < min.width Then
						deltaX = startingBounds.width - min.width
					ElseIf startingBounds.width - deltaX > max.width Then
						deltaX = -(max.width - startingBounds.width)
					End If
					If startingBounds.x + startingBounds.width - deltaX > outerInstance.parentBounds.width Then deltaX = startingBounds.x + startingBounds.width - outerInstance.parentBounds.width

					If startingBounds.height - deltaY < min.height Then
						deltaY = startingBounds.height - min.height
					ElseIf startingBounds.height - deltaY > max.height Then
						deltaY = -(max.height - startingBounds.height)
					End If
					If startingBounds.y + startingBounds.height - deltaY > outerInstance.parentBounds.height Then deltaY = startingBounds.y + startingBounds.height - outerInstance.parentBounds.height

					newW = startingBounds.width - deltaX
					newH = startingBounds.height - deltaY
				Case SOUTH
					If startingBounds.height - deltaY < min.height Then
						deltaY = startingBounds.height - min.height
					ElseIf startingBounds.height - deltaY > max.height Then
						deltaY = -(max.height - startingBounds.height)
					End If
					If startingBounds.y + startingBounds.height - deltaY > outerInstance.parentBounds.height Then deltaY = startingBounds.y + startingBounds.height - outerInstance.parentBounds.height

					newW = startingBounds.width
					newH = startingBounds.height - deltaY
				Case SOUTH_WEST
					If startingBounds.height - deltaY < min.height Then
						deltaY = startingBounds.height - min.height
					ElseIf startingBounds.height - deltaY > max.height Then
						deltaY = -(max.height - startingBounds.height)
					End If
					If startingBounds.y + startingBounds.height - deltaY > outerInstance.parentBounds.height Then deltaY = startingBounds.y + startingBounds.height - outerInstance.parentBounds.height

					If startingBounds.width + deltaX < min.width Then
						deltaX = -(startingBounds.width - min.width)
					ElseIf startingBounds.width + deltaX > max.width Then
						deltaX = max.width - startingBounds.width
					End If
					If startingBounds.x - deltaX < 0 Then deltaX = startingBounds.x

					newX = startingBounds.x - deltaX
					newY = startingBounds.y
					newW = startingBounds.width + deltaX
					newH = startingBounds.height - deltaY
				Case WEST
					If startingBounds.width + deltaX < min.width Then
						deltaX = -(startingBounds.width - min.width)
					ElseIf startingBounds.width + deltaX > max.width Then
						deltaX = max.width - startingBounds.width
					End If
					If startingBounds.x - deltaX < 0 Then deltaX = startingBounds.x

					newX = startingBounds.x - deltaX
					newY = startingBounds.y
					newW = startingBounds.width + deltaX
					newH = startingBounds.height
				Case NORTH_WEST
					If startingBounds.width + deltaX < min.width Then
						deltaX = -(startingBounds.width - min.width)
					ElseIf startingBounds.width + deltaX > max.width Then
						deltaX = max.width - startingBounds.width
					End If
					If startingBounds.x - deltaX < 0 Then deltaX = startingBounds.x

					If startingBounds.height + deltaY < min.height Then
						deltaY = -(startingBounds.height - min.height)
					ElseIf startingBounds.height + deltaY > max.height Then
						deltaY = max.height - startingBounds.height
					End If
					If startingBounds.y - deltaY < 0 Then deltaY = startingBounds.y

					newX = startingBounds.x - deltaX
					newY = startingBounds.y - deltaY
					newW = startingBounds.width + deltaX
					newH = startingBounds.height + deltaY
				Case Else
					Return
				End Select
				outerInstance.desktopManager.resizeFrame(outerInstance.frame, newX, newY, newW, newH)
			End Sub

			Public Overridable Sub mouseMoved(ByVal e As MouseEvent)

				If Not outerInstance.frame.resizable Then Return

				If e.source Is outerInstance.frame OrElse e.source Is outerInstance.northPane Then
					Dim i As Insets = outerInstance.frame.insets
					Dim ep As New Point(e.x, e.y)
					If e.source Is outerInstance.northPane Then
						Dim np As Point = outerInstance.northPane.location
						ep.x += np.x
						ep.y += np.y
					End If
					If ep.x <= i.left Then
						If ep.y < resizeCornerSize + i.top Then
							outerInstance.frame.cursor = Cursor.getPredefinedCursor(Cursor.NW_RESIZE_CURSOR)
						ElseIf ep.y > outerInstance.frame.height - resizeCornerSize - i.bottom Then
							outerInstance.frame.cursor = Cursor.getPredefinedCursor(Cursor.SW_RESIZE_CURSOR)
						Else
							outerInstance.frame.cursor = Cursor.getPredefinedCursor(Cursor.W_RESIZE_CURSOR)
						End If
					ElseIf ep.x >= outerInstance.frame.width - i.right Then
						If e.y < resizeCornerSize + i.top Then
							outerInstance.frame.cursor = Cursor.getPredefinedCursor(Cursor.NE_RESIZE_CURSOR)
						ElseIf ep.y > outerInstance.frame.height - resizeCornerSize - i.bottom Then
							outerInstance.frame.cursor = Cursor.getPredefinedCursor(Cursor.SE_RESIZE_CURSOR)
						Else
							outerInstance.frame.cursor = Cursor.getPredefinedCursor(Cursor.E_RESIZE_CURSOR)
						End If
					ElseIf ep.y <= i.top Then
						If ep.x < resizeCornerSize + i.left Then
							outerInstance.frame.cursor = Cursor.getPredefinedCursor(Cursor.NW_RESIZE_CURSOR)
						ElseIf ep.x > outerInstance.frame.width - resizeCornerSize - i.right Then
							outerInstance.frame.cursor = Cursor.getPredefinedCursor(Cursor.NE_RESIZE_CURSOR)
						Else
							outerInstance.frame.cursor = Cursor.getPredefinedCursor(Cursor.N_RESIZE_CURSOR)
						End If
					ElseIf ep.y >= outerInstance.frame.height - i.bottom Then
						If ep.x < resizeCornerSize + i.left Then
							outerInstance.frame.cursor = Cursor.getPredefinedCursor(Cursor.SW_RESIZE_CURSOR)
						ElseIf ep.x > outerInstance.frame.width - resizeCornerSize - i.right Then
							outerInstance.frame.cursor = Cursor.getPredefinedCursor(Cursor.SE_RESIZE_CURSOR)
						Else
							outerInstance.frame.cursor = Cursor.getPredefinedCursor(Cursor.S_RESIZE_CURSOR)
						End If
					Else
						outerInstance.updateFrameCursor()
					End If
					Return
				End If

				outerInstance.updateFrameCursor()
			End Sub

			Public Overridable Sub mouseEntered(ByVal e As MouseEvent)
				outerInstance.updateFrameCursor()
			End Sub

			Public Overridable Sub mouseExited(ByVal e As MouseEvent)
				outerInstance.updateFrameCursor()
			End Sub

		End Class '/ End BorderListener Class

		Protected Friend Class ComponentHandler
			Implements ComponentListener

			Private ReadOnly outerInstance As BasicInternalFrameUI

			Public Sub New(ByVal outerInstance As BasicInternalFrameUI)
				Me.outerInstance = outerInstance
			End Sub

		  ' NOTE: This class exists only for backward compatibility. All
		  ' its functionality has been moved into Handler. If you need to add
		  ' new functionality add it to the Handler, but make sure this
		  ' class calls into the Handler.
		  ''' <summary>
		  ''' Invoked when a JInternalFrame's parent's size changes. </summary>
		  Public Overridable Sub componentResized(ByVal e As ComponentEvent)
			  outerInstance.handler.componentResized(e)
		  End Sub

		  Public Overridable Sub componentMoved(ByVal e As ComponentEvent)
			  outerInstance.handler.componentMoved(e)
		  End Sub
		  Public Overridable Sub componentShown(ByVal e As ComponentEvent)
			  outerInstance.handler.componentShown(e)
		  End Sub
		  Public Overridable Sub componentHidden(ByVal e As ComponentEvent)
			  outerInstance.handler.componentHidden(e)
		  End Sub
		End Class

		Protected Friend Overridable Function createComponentListener() As ComponentListener
		  Return handler
		End Function


		Protected Friend Class GlassPaneDispatcher
			Implements MouseInputListener

			Private ReadOnly outerInstance As BasicInternalFrameUI

			Public Sub New(ByVal outerInstance As BasicInternalFrameUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.
			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				outerInstance.handler.mousePressed(e)
			End Sub

			Public Overridable Sub mouseEntered(ByVal e As MouseEvent)
				outerInstance.handler.mouseEntered(e)
			End Sub

			Public Overridable Sub mouseMoved(ByVal e As MouseEvent)
				outerInstance.handler.mouseMoved(e)
			End Sub

			Public Overridable Sub mouseExited(ByVal e As MouseEvent)
				outerInstance.handler.mouseExited(e)
			End Sub

			Public Overridable Sub mouseClicked(ByVal e As MouseEvent)
				outerInstance.handler.mouseClicked(e)
			End Sub

			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
				outerInstance.handler.mouseReleased(e)
			End Sub

			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
				outerInstance.handler.mouseDragged(e)
			End Sub
		End Class

		Protected Friend Overridable Function createGlassPaneDispatcher() As MouseInputListener
			Return Nothing
		End Function


		Protected Friend Class BasicInternalFrameListener
			Implements InternalFrameListener

			Private ReadOnly outerInstance As BasicInternalFrameUI

			Public Sub New(ByVal outerInstance As BasicInternalFrameUI)
				Me.outerInstance = outerInstance
			End Sub

		  ' NOTE: This class exists only for backward compatibility. All
		  ' its functionality has been moved into Handler. If you need to add
		  ' new functionality add it to the Handler, but make sure this
		  ' class calls into the Handler.
		  Public Overridable Sub internalFrameClosing(ByVal e As InternalFrameEvent) Implements InternalFrameListener.internalFrameClosing
			  outerInstance.handler.internalFrameClosing(e)
		  End Sub

		  Public Overridable Sub internalFrameClosed(ByVal e As InternalFrameEvent) Implements InternalFrameListener.internalFrameClosed
			  outerInstance.handler.internalFrameClosed(e)
		  End Sub

		  Public Overridable Sub internalFrameOpened(ByVal e As InternalFrameEvent) Implements InternalFrameListener.internalFrameOpened
			  outerInstance.handler.internalFrameOpened(e)
		  End Sub

		  Public Overridable Sub internalFrameIconified(ByVal e As InternalFrameEvent) Implements InternalFrameListener.internalFrameIconified
			  outerInstance.handler.internalFrameIconified(e)
		  End Sub

		  Public Overridable Sub internalFrameDeiconified(ByVal e As InternalFrameEvent) Implements InternalFrameListener.internalFrameDeiconified
			  outerInstance.handler.internalFrameDeiconified(e)
		  End Sub

		  Public Overridable Sub internalFrameActivated(ByVal e As InternalFrameEvent) Implements InternalFrameListener.internalFrameActivated
			  outerInstance.handler.internalFrameActivated(e)
		  End Sub


		  Public Overridable Sub internalFrameDeactivated(ByVal e As InternalFrameEvent) Implements InternalFrameListener.internalFrameDeactivated
			  outerInstance.handler.internalFrameDeactivated(e)
		  End Sub
		End Class

		Private Class Handler
			Implements ComponentListener, InternalFrameListener, LayoutManager, MouseInputListener, PropertyChangeListener, WindowFocusListener, SwingConstants

			Private ReadOnly outerInstance As BasicInternalFrameUI

			Public Sub New(ByVal outerInstance As BasicInternalFrameUI)
				Me.outerInstance = outerInstance
			End Sub


			Public Overridable Sub windowGainedFocus(ByVal e As WindowEvent)
			End Sub

			Public Overridable Sub windowLostFocus(ByVal e As WindowEvent)
				' Cancel a resize which may be in progress, when a
				' WINDOW_LOST_FOCUS event occurs, which may be
				' caused by an Alt-Tab or a modal dialog popup.
				outerInstance.cancelResize()
			End Sub

			' ComponentHandler methods
			''' <summary>
			''' Invoked when a JInternalFrame's parent's size changes. </summary>
			Public Overridable Sub componentResized(ByVal e As ComponentEvent)
				' Get the JInternalFrame's parent container size
				Dim parentNewBounds As Rectangle = CType(e.source, Component).bounds
				Dim icon As JInternalFrame.JDesktopIcon = Nothing

				If outerInstance.frame IsNot Nothing Then
					icon = outerInstance.frame.desktopIcon
					' Resize the internal frame if it is maximized and relocate
					' the associated icon as well.
					If outerInstance.frame.maximum Then outerInstance.frame.boundsnds(0, 0, parentNewBounds.width, parentNewBounds.height)
				End If

				' Relocate the icon base on the new parent bounds.
				If icon IsNot Nothing Then
					Dim iconBounds As Rectangle = icon.bounds
					Dim y As Integer = iconBounds.y + (parentNewBounds.height - outerInstance.parentBounds.height)
					icon.boundsnds(iconBounds.x, y, iconBounds.width, iconBounds.height)
				End If

				' Update the new parent bounds for next resize.
				If Not outerInstance.parentBounds.Equals(parentNewBounds) Then outerInstance.parentBounds = parentNewBounds

				' Validate the component tree for this container.
				If outerInstance.frame IsNot Nothing Then outerInstance.frame.validate()
			End Sub

			Public Overridable Sub componentMoved(ByVal e As ComponentEvent)
			End Sub
			Public Overridable Sub componentShown(ByVal e As ComponentEvent)
			End Sub
			Public Overridable Sub componentHidden(ByVal e As ComponentEvent)
			End Sub


			' InternalFrameListener
			Public Overridable Sub internalFrameClosed(ByVal e As InternalFrameEvent) Implements InternalFrameListener.internalFrameClosed
				outerInstance.frame.removeInternalFrameListener(outerInstance.handler)
			End Sub

			Public Overridable Sub internalFrameActivated(ByVal e As InternalFrameEvent) Implements InternalFrameListener.internalFrameActivated
				If Not outerInstance.keyBindingRegistered Then
					outerInstance.keyBindingRegistered = True
					outerInstance.setupMenuOpenKey()
					outerInstance.setupMenuCloseKey()
				End If
				If outerInstance.keyBindingRegistered Then outerInstance.keyBindingActive = True
			End Sub

			Public Overridable Sub internalFrameDeactivated(ByVal e As InternalFrameEvent) Implements InternalFrameListener.internalFrameDeactivated
				outerInstance.keyBindingActive = False
			End Sub

			Public Overridable Sub internalFrameClosing(ByVal e As InternalFrameEvent) Implements InternalFrameListener.internalFrameClosing
			End Sub
			Public Overridable Sub internalFrameOpened(ByVal e As InternalFrameEvent) Implements InternalFrameListener.internalFrameOpened
			End Sub
			Public Overridable Sub internalFrameIconified(ByVal e As InternalFrameEvent) Implements InternalFrameListener.internalFrameIconified
			End Sub
			Public Overridable Sub internalFrameDeiconified(ByVal e As InternalFrameEvent) Implements InternalFrameListener.internalFrameDeiconified
			End Sub


			' LayoutManager
			Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal c As Component)
			End Sub
			Public Overridable Sub removeLayoutComponent(ByVal c As Component)
			End Sub
			Public Overridable Function preferredLayoutSize(ByVal c As Container) As Dimension
				Dim result As Dimension
				Dim i As Insets = outerInstance.frame.insets

				result = New Dimension(outerInstance.frame.rootPane.preferredSize)
				result.width += i.left + i.right
				result.height += i.top + i.bottom

				If outerInstance.northPane IsNot Nothing Then
					Dim d As Dimension = outerInstance.northPane.preferredSize
					result.width = Math.Max(d.width, result.width)
					result.height += d.height
				End If

				If outerInstance.southPane IsNot Nothing Then
					Dim d As Dimension = outerInstance.southPane.preferredSize
					result.width = Math.Max(d.width, result.width)
					result.height += d.height
				End If

				If outerInstance.eastPane IsNot Nothing Then
					Dim d As Dimension = outerInstance.eastPane.preferredSize
					result.width += d.width
					result.height = Math.Max(d.height, result.height)
				End If

				If outerInstance.westPane IsNot Nothing Then
					Dim d As Dimension = outerInstance.westPane.preferredSize
					result.width += d.width
					result.height = Math.Max(d.height, result.height)
				End If
				Return result
			End Function

			Public Overridable Function minimumLayoutSize(ByVal c As Container) As Dimension
				' The minimum size of the internal frame only takes into
				' account the title pane since you are allowed to resize
				' the frames to the point where just the title pane is visible.
				Dim result As New Dimension
				If outerInstance.northPane IsNot Nothing AndAlso TypeOf outerInstance.northPane Is BasicInternalFrameTitlePane Then result = New Dimension(outerInstance.northPane.minimumSize)
				Dim i As Insets = outerInstance.frame.insets
				result.width += i.left + i.right
				result.height += i.top + i.bottom

				Return result
			End Function

			Public Overridable Sub layoutContainer(ByVal c As Container)
				Dim i As Insets = outerInstance.frame.insets
				Dim cx, cy, cw, ch As Integer

				cx = i.left
				cy = i.top
				cw = outerInstance.frame.width - i.left - i.right
				ch = outerInstance.frame.height - i.top - i.bottom

				If outerInstance.northPane IsNot Nothing Then
					Dim size As Dimension = outerInstance.northPane.preferredSize
					If sun.swing.DefaultLookup.getBoolean(outerInstance.frame, BasicInternalFrameUI.this, "InternalFrame.layoutTitlePaneAtOrigin", False) Then
						cy = 0
						ch += i.top
						outerInstance.northPane.boundsnds(0, 0, outerInstance.frame.width, size.height)
					Else
						outerInstance.northPane.boundsnds(cx, cy, cw, size.height)
					End If
					cy += size.height
					ch -= size.height
				End If

				If outerInstance.southPane IsNot Nothing Then
					Dim size As Dimension = outerInstance.southPane.preferredSize
					outerInstance.southPane.boundsnds(cx, outerInstance.frame.height - i.bottom - size.height, cw, size.height)
					ch -= size.height
				End If

				If outerInstance.westPane IsNot Nothing Then
					Dim size As Dimension = outerInstance.westPane.preferredSize
					outerInstance.westPane.boundsnds(cx, cy, size.width, ch)
					cw -= size.width
					cx += size.width
				End If

				If outerInstance.eastPane IsNot Nothing Then
					Dim size As Dimension = outerInstance.eastPane.preferredSize
					outerInstance.eastPane.boundsnds(cw - size.width, cy, size.width, ch)
					cw -= size.width
				End If

				If outerInstance.frame.rootPane IsNot Nothing Then outerInstance.frame.rootPane.boundsnds(cx, cy, cw, ch)
			End Sub


			' MouseInputListener
			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseEntered(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseMoved(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseExited(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseClicked(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
			End Sub

			' PropertyChangeListener
			Public Overridable Sub propertyChange(ByVal evt As PropertyChangeEvent)
				Dim prop As String = evt.propertyName
				Dim f As JInternalFrame = CType(evt.source, JInternalFrame)
				Dim newValue As Object = evt.newValue
				Dim oldValue As Object = evt.oldValue

				If JInternalFrame.IS_CLOSED_PROPERTY = prop Then
					If newValue Is Boolean.TRUE Then
						' Cancel a resize in progress if the internal frame
						' gets a setClosed(true) or dispose().
						outerInstance.cancelResize()
						If (outerInstance.frame.parent IsNot Nothing) AndAlso outerInstance.componentListenerAdded Then outerInstance.frame.parent.removeComponentListener(outerInstance.componentListener)
						outerInstance.closeFrame(f)
					End If
				ElseIf JInternalFrame.IS_MAXIMUM_PROPERTY = prop Then
					If newValue Is Boolean.TRUE Then
						outerInstance.maximizeFrame(f)
					Else
						outerInstance.minimizeFrame(f)
					End If
				ElseIf JInternalFrame.IS_ICON_PROPERTY = prop Then
					If newValue Is Boolean.TRUE Then
						outerInstance.iconifyFrame(f)
					Else
						outerInstance.deiconifyFrame(f)
					End If
				ElseIf JInternalFrame.IS_SELECTED_PROPERTY = prop Then
					If newValue Is Boolean.TRUE AndAlso oldValue Is Boolean.FALSE Then
						outerInstance.activateFrame(f)
					ElseIf newValue Is Boolean.FALSE AndAlso oldValue Is Boolean.TRUE Then
						outerInstance.deactivateFrame(f)
					End If
				ElseIf prop = "ancestor" Then
					If newValue Is Nothing Then outerInstance.cancelResize()
					If outerInstance.frame.parent IsNot Nothing Then
						outerInstance.parentBounds = f.parent.bounds
					Else
						outerInstance.parentBounds = Nothing
					End If
					If (outerInstance.frame.parent IsNot Nothing) AndAlso (Not outerInstance.componentListenerAdded) Then
						f.parent.addComponentListener(outerInstance.componentListener)
						outerInstance.componentListenerAdded = True
					End If
				ElseIf JInternalFrame.TITLE_PROPERTY = prop OrElse prop = "closable" OrElse prop = "iconable" OrElse prop = "maximizable" Then
					Dim [dim] As Dimension = outerInstance.frame.minimumSize
					Dim frame_dim As Dimension = outerInstance.frame.size
					If [dim].width > frame_dim.width Then outerInstance.frame.sizeize([dim].width, frame_dim.height)
				End If
			End Sub
		End Class
	End Class

End Namespace
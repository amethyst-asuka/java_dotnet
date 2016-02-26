Imports System

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


Namespace javax.swing


	''' <summary>
	''' Manages all the <code>ToolTips</code> in the system.
	''' <p>
	''' ToolTipManager contains numerous properties for configuring how long it
	''' will take for the tooltips to become visible, and how long till they
	''' hide. Consider a component that has a different tooltip based on where
	''' the mouse is, such as JTree. When the mouse moves into the JTree and
	''' over a region that has a valid tooltip, the tooltip will become
	''' visible after <code>initialDelay</code> milliseconds. After
	''' <code>dismissDelay</code> milliseconds the tooltip will be hidden. If
	''' the mouse is over a region that has a valid tooltip, and the tooltip
	''' is currently visible, when the mouse moves to a region that doesn't have
	''' a valid tooltip the tooltip will be hidden. If the mouse then moves back
	''' into a region that has a valid tooltip within <code>reshowDelay</code>
	''' milliseconds, the tooltip will immediately be shown, otherwise the
	''' tooltip will be shown again after <code>initialDelay</code> milliseconds.
	''' </summary>
	''' <seealso cref= JComponent#createToolTip
	''' @author Dave Moore
	''' @author Rich Schiavi </seealso>
	Public Class ToolTipManager
		Inherits MouseAdapter
		Implements MouseMotionListener

		Friend enterTimer, exitTimer, insideTimer As Timer
		Friend toolTipText As String
		Friend preferredLocation As Point
		Friend insideComponent As JComponent
		Friend mouseEvent As MouseEvent
		Friend showImmediately As Boolean
		Private Shared ReadOnly TOOL_TIP_MANAGER_KEY As New Object
		<NonSerialized> _
		Friend tipWindow As Popup
		''' <summary>
		''' The Window tip is being displayed in. This will be non-null if
		''' the Window tip is in differs from that of insideComponent's Window.
		''' </summary>
		Private window As Window
		Friend tip As JToolTip

		Private popupRect As Rectangle = Nothing
		Private popupFrameRect As Rectangle = Nothing

		Friend enabled As Boolean = True
		Private tipShowing As Boolean = False

		Private focusChangeListener As FocusListener = Nothing
		Private moveBeforeEnterListener As MouseMotionListener = Nothing
		Private accessibilityKeyListener As KeyListener = Nothing

		Private postTip As KeyStroke
		Private hideTip As KeyStroke

		' PENDING(ges)
		Protected Friend lightWeightPopupEnabled As Boolean = True
		Protected Friend heavyWeightPopupEnabled As Boolean = False

		Friend Sub New()
			enterTimer = New Timer(750, New insideTimerAction(Me))
			enterTimer.repeats = False
			exitTimer = New Timer(500, New outsideTimerAction(Me))
			exitTimer.repeats = False
			insideTimer = New Timer(4000, New stillInsideTimerAction(Me))
			insideTimer.repeats = False

			moveBeforeEnterListener = New MoveBeforeEnterListener(Me)
			accessibilityKeyListener = New AccessibilityKeyListener(Me)

			postTip = KeyStroke.getKeyStroke(KeyEvent.VK_F1, InputEvent.CTRL_MASK)
			hideTip = KeyStroke.getKeyStroke(KeyEvent.VK_ESCAPE, 0)
		End Sub

		''' <summary>
		''' Enables or disables the tooltip.
		''' </summary>
		''' <param name="flag">  true to enable the tip, false otherwise </param>
		Public Overridable Property enabled As Boolean
			Set(ByVal flag As Boolean)
				enabled = flag
				If Not flag Then hideTipWindow()
			End Set
			Get
				Return enabled
			End Get
		End Property


		''' <summary>
		''' When displaying the <code>JToolTip</code>, the
		''' <code>ToolTipManager</code> chooses to use a lightweight
		''' <code>JPanel</code> if it fits. This method allows you to
		''' disable this feature. You have to do disable it if your
		''' application mixes light weight and heavy weights components.
		''' </summary>
		''' <param name="aFlag"> true if a lightweight panel is desired, false otherwise
		'''  </param>
		Public Overridable Property lightWeightPopupEnabled As Boolean
			Set(ByVal aFlag As Boolean)
				lightWeightPopupEnabled = aFlag
			End Set
			Get
				Return lightWeightPopupEnabled
			End Get
		End Property



		''' <summary>
		''' Specifies the initial delay value.
		''' </summary>
		''' <param name="milliseconds">  the number of milliseconds to delay
		'''        (after the cursor has paused) before displaying the
		'''        tooltip </param>
		''' <seealso cref= #getInitialDelay </seealso>
		Public Overridable Property initialDelay As Integer
			Set(ByVal milliseconds As Integer)
				enterTimer.initialDelay = milliseconds
			End Set
			Get
				Return enterTimer.initialDelay
			End Get
		End Property


		''' <summary>
		''' Specifies the dismissal delay value.
		''' </summary>
		''' <param name="milliseconds">  the number of milliseconds to delay
		'''        before taking away the tooltip </param>
		''' <seealso cref= #getDismissDelay </seealso>
		Public Overridable Property dismissDelay As Integer
			Set(ByVal milliseconds As Integer)
				insideTimer.initialDelay = milliseconds
			End Set
			Get
				Return insideTimer.initialDelay
			End Get
		End Property


		''' <summary>
		''' Used to specify the amount of time before the user has to wait
		''' <code>initialDelay</code> milliseconds before a tooltip will be
		''' shown. That is, if the tooltip is hidden, and the user moves into
		''' a region of the same Component that has a valid tooltip within
		''' <code>milliseconds</code> milliseconds the tooltip will immediately
		''' be shown. Otherwise, if the user moves into a region with a valid
		''' tooltip after <code>milliseconds</code> milliseconds, the user
		''' will have to wait an additional <code>initialDelay</code>
		''' milliseconds before the tooltip is shown again.
		''' </summary>
		''' <param name="milliseconds"> time in milliseconds </param>
		''' <seealso cref= #getReshowDelay </seealso>
		Public Overridable Property reshowDelay As Integer
			Set(ByVal milliseconds As Integer)
				exitTimer.initialDelay = milliseconds
			End Set
			Get
				Return exitTimer.initialDelay
			End Get
		End Property


		' Returns GraphicsConfiguration instance that toFind belongs to or null
		' if drawing point is set to a point beyond visible screen area (e.g.
		' Point(20000, 20000))
		Private Function getDrawingGC(ByVal toFind As Point) As GraphicsConfiguration
			Dim env As GraphicsEnvironment = GraphicsEnvironment.localGraphicsEnvironment
			Dim devices As GraphicsDevice() = env.screenDevices
			For Each device As GraphicsDevice In devices
				Dim configs As GraphicsConfiguration() = device.configurations
				For Each config As GraphicsConfiguration In configs
					Dim rect As Rectangle = config.bounds
					If rect.contains(toFind) Then Return config
				Next config
			Next device

			Return Nothing
		End Function

		Friend Overridable Sub showTipWindow()
			If insideComponent Is Nothing OrElse (Not insideComponent.showing) Then Return
			Dim mode As String = UIManager.getString("ToolTipManager.enableToolTipMode")
			If "activeApplication".Equals(mode) Then
				Dim kfm As KeyboardFocusManager = KeyboardFocusManager.currentKeyboardFocusManager
				If kfm.focusedWindow Is Nothing Then Return
			End If
			If enabled Then
				Dim size As Dimension
				Dim screenLocation As Point = insideComponent.locationOnScreen
				Dim location As Point

				Dim toFind As Point
				If preferredLocation IsNot Nothing Then
					toFind = New Point(screenLocation.x + preferredLocation.x, screenLocation.y + preferredLocation.y)
				Else
					toFind = mouseEvent.locationOnScreen
				End If

				Dim gc As GraphicsConfiguration = getDrawingGC(toFind)
				If gc Is Nothing Then
					toFind = mouseEvent.locationOnScreen
					gc = getDrawingGC(toFind)
					If gc Is Nothing Then gc = insideComponent.graphicsConfiguration
				End If

				Dim sBounds As Rectangle = gc.bounds
				Dim screenInsets As Insets = Toolkit.defaultToolkit.getScreenInsets(gc)
				' Take into account screen insets, decrease viewport
				sBounds.x += screenInsets.left
				sBounds.y += screenInsets.top
				sBounds.width -= (screenInsets.left + screenInsets.right)
				sBounds.height -= (screenInsets.top + screenInsets.bottom)
			Dim leftToRight As Boolean = SwingUtilities.isLeftToRight(insideComponent)

				' Just to be paranoid
				hideTipWindow()

				tip = insideComponent.createToolTip()
				tip.tipText = toolTipText
				size = tip.preferredSize

				If preferredLocation IsNot Nothing Then
					location = toFind
			If Not leftToRight Then location.x -= size.width
				Else
					location = New Point(screenLocation.x + mouseEvent.x, screenLocation.y + mouseEvent.y + 20)
			If Not leftToRight Then
				If location.x - size.width>=0 Then location.x -= size.width
			End If

				End If

			' we do not adjust x/y when using awt.Window tips
			If popupRect Is Nothing Then popupRect = New Rectangle
			popupRect.boundsnds(location.x,location.y, size.width,size.height)

			' Fit as much of the tooltip on screen as possible
				If location.x < sBounds.x Then
					location.x = sBounds.x
				ElseIf location.x - sBounds.x + size.width > sBounds.width Then
					location.x = sBounds.x + Math.Max(0, sBounds.width - size.width)
				End If
				If location.y < sBounds.y Then
					location.y = sBounds.y
				ElseIf location.y - sBounds.y + size.height > sBounds.height Then
					location.y = sBounds.y + Math.Max(0, sBounds.height - size.height)
				End If

				Dim ___popupFactory As PopupFactory = PopupFactory.sharedInstance

				If lightWeightPopupEnabled Then
			Dim y As Integer = getPopupFitHeight(popupRect, insideComponent)
			Dim x As Integer = getPopupFitWidth(popupRect,insideComponent)
			If x>0 OrElse y>0 Then
				___popupFactory.popupType = PopupFactory.MEDIUM_WEIGHT_POPUP
			Else
				___popupFactory.popupType = PopupFactory.LIGHT_WEIGHT_POPUP
			End If
				Else
					___popupFactory.popupType = PopupFactory.MEDIUM_WEIGHT_POPUP
				End If
			tipWindow = ___popupFactory.getPopup(insideComponent, tip, location.x, location.y)
				___popupFactory.popupType = PopupFactory.LIGHT_WEIGHT_POPUP

			tipWindow.show()

				Dim componentWindow As Window = SwingUtilities.windowForComponent(insideComponent)

				window = SwingUtilities.windowForComponent(tip)
				If window IsNot Nothing AndAlso window IsNot componentWindow Then
					window.addMouseListener(Me)
				Else
					window = Nothing
				End If

				insideTimer.start()
			tipShowing = True
			End If
		End Sub

		Friend Overridable Sub hideTipWindow()
			If tipWindow IsNot Nothing Then
				If window IsNot Nothing Then
					window.removeMouseListener(Me)
					window = Nothing
				End If
				tipWindow.hide()
				tipWindow = Nothing
				tipShowing = False
				tip = Nothing
				insideTimer.stop()
			End If
		End Sub

		''' <summary>
		''' Returns a shared <code>ToolTipManager</code> instance.
		''' </summary>
		''' <returns> a shared <code>ToolTipManager</code> object </returns>
		Public Shared Function sharedInstance() As ToolTipManager
			Dim value As Object = SwingUtilities.appContextGet(TOOL_TIP_MANAGER_KEY)
			If TypeOf value Is ToolTipManager Then Return CType(value, ToolTipManager)
			Dim manager As New ToolTipManager
			SwingUtilities.appContextPut(TOOL_TIP_MANAGER_KEY, manager)
			Return manager
		End Function

		' add keylistener here to trigger tip for access
		''' <summary>
		''' Registers a component for tooltip management.
		''' <p>
		''' This will register key bindings to show and hide the tooltip text
		''' only if <code>component</code> has focus bindings. This is done
		''' so that components that are not normally focus traversable, such
		''' as <code>JLabel</code>, are not made focus traversable as a result
		''' of invoking this method.
		''' </summary>
		''' <param name="component">  a <code>JComponent</code> object to add </param>
		''' <seealso cref= JComponent#isFocusTraversable </seealso>
		Public Overridable Sub registerComponent(ByVal component As JComponent)
			component.removeMouseListener(Me)
			component.addMouseListener(Me)
			component.removeMouseMotionListener(moveBeforeEnterListener)
			component.addMouseMotionListener(moveBeforeEnterListener)
			component.removeKeyListener(accessibilityKeyListener)
			component.addKeyListener(accessibilityKeyListener)
		End Sub

		''' <summary>
		''' Removes a component from tooltip control.
		''' </summary>
		''' <param name="component">  a <code>JComponent</code> object to remove </param>
		Public Overridable Sub unregisterComponent(ByVal component As JComponent)
			component.removeMouseListener(Me)
			component.removeMouseMotionListener(moveBeforeEnterListener)
			component.removeKeyListener(accessibilityKeyListener)
		End Sub

		' implements java.awt.event.MouseListener
		''' <summary>
		'''  Called when the mouse enters the region of a component.
		'''  This determines whether the tool tip should be shown.
		''' </summary>
		'''  <param name="event">  the event in question </param>
		Public Overridable Sub mouseEntered(ByVal [event] As MouseEvent)
			initiateToolTip([event])
		End Sub

		Private Sub initiateToolTip(ByVal [event] As MouseEvent)
			If [event].source Is window Then Return
			Dim component As JComponent = CType([event].source, JComponent)
			component.removeMouseMotionListener(moveBeforeEnterListener)

			exitTimer.stop()

			Dim location As Point = [event].point
			' ensure tooltip shows only in proper place
			If location.x < 0 OrElse location.x >=component.width OrElse location.y < 0 OrElse location.y >= component.height Then Return

			If insideComponent IsNot Nothing Then enterTimer.stop()
			' A component in an unactive internal frame is sent two
			' mouseEntered events, make sure we don't end up adding
			' ourselves an extra time.
			component.removeMouseMotionListener(Me)
			component.addMouseMotionListener(Me)

			Dim sameComponent As Boolean = (insideComponent Is component)

			insideComponent = component
		If tipWindow IsNot Nothing Then
				mouseEvent = [event]
				If showImmediately Then
					Dim newToolTipText As String = component.getToolTipText([event])
					Dim newPreferredLocation As Point = component.getToolTipLocation([event])
					Dim sameLoc As Boolean = If(preferredLocation IsNot Nothing, preferredLocation.Equals(newPreferredLocation), (newPreferredLocation Is Nothing))

					If (Not sameComponent) OrElse (Not toolTipText.Equals(newToolTipText)) OrElse (Not sameLoc) Then
						toolTipText = newToolTipText
						preferredLocation = newPreferredLocation
						showTipWindow()
					End If
				Else
					enterTimer.start()
				End If
		End If
		End Sub

		' implements java.awt.event.MouseListener
		''' <summary>
		'''  Called when the mouse exits the region of a component.
		'''  Any tool tip showing should be hidden.
		''' </summary>
		'''  <param name="event">  the event in question </param>
		Public Overridable Sub mouseExited(ByVal [event] As MouseEvent)
			Dim shouldHide As Boolean = True
			If insideComponent Is Nothing Then
				' Drag exit
			End If
			If window IsNot Nothing AndAlso [event].source Is window AndAlso insideComponent IsNot Nothing Then
			  ' if we get an exit and have a heavy window
			  ' we need to check if it if overlapping the inside component
				Dim insideComponentWindow As Container = insideComponent.topLevelAncestor
				' insideComponent may be removed after tooltip is made visible
				If insideComponentWindow IsNot Nothing Then
					Dim location As Point = [event].point
					SwingUtilities.convertPointToScreen(location, window)

					location.x -= insideComponentWindow.x
					location.y -= insideComponentWindow.y

					location = SwingUtilities.convertPoint(Nothing, location, insideComponent)
					If location.x >= 0 AndAlso location.x < insideComponent.width AndAlso location.y >= 0 AndAlso location.y < insideComponent.height Then
						shouldHide = False
					Else
						shouldHide = True
					End If
				End If
			ElseIf [event].source Is insideComponent AndAlso tipWindow IsNot Nothing Then
				Dim win As Window = SwingUtilities.getWindowAncestor(insideComponent)
				If win IsNot Nothing Then ' insideComponent may have been hidden (e.g. in a menu)
					Dim location As Point = SwingUtilities.convertPoint(insideComponent, [event].point, win)
					Dim bounds As Rectangle = insideComponent.topLevelAncestor.bounds
					location.x += bounds.x
					location.y += bounds.y

					Dim loc As New Point(0, 0)
					SwingUtilities.convertPointToScreen(loc, tip)
					bounds.x = loc.x
					bounds.y = loc.y
					bounds.width = tip.width
					bounds.height = tip.height

					If location.x >= bounds.x AndAlso location.x < (bounds.x + bounds.width) AndAlso location.y >= bounds.y AndAlso location.y < (bounds.y + bounds.height) Then
						shouldHide = False
					Else
						shouldHide = True
					End If
				End If
			End If

			If shouldHide Then
				enterTimer.stop()
			If insideComponent IsNot Nothing Then insideComponent.removeMouseMotionListener(Me)
				insideComponent = Nothing
				toolTipText = Nothing
				mouseEvent = Nothing
				hideTipWindow()
				exitTimer.restart()
			End If
		End Sub

		' implements java.awt.event.MouseListener
		''' <summary>
		'''  Called when the mouse is pressed.
		'''  Any tool tip showing should be hidden.
		''' </summary>
		'''  <param name="event">  the event in question </param>
		Public Overridable Sub mousePressed(ByVal [event] As MouseEvent)
			hideTipWindow()
			enterTimer.stop()
			showImmediately = False
			insideComponent = Nothing
			mouseEvent = Nothing
		End Sub

		' implements java.awt.event.MouseMotionListener
		''' <summary>
		'''  Called when the mouse is pressed and dragged.
		'''  Does nothing.
		''' </summary>
		'''  <param name="event">  the event in question </param>
		Public Overridable Sub mouseDragged(ByVal [event] As MouseEvent)
		End Sub

		' implements java.awt.event.MouseMotionListener
		''' <summary>
		'''  Called when the mouse is moved.
		'''  Determines whether the tool tip should be displayed.
		''' </summary>
		'''  <param name="event">  the event in question </param>
		Public Overridable Sub mouseMoved(ByVal [event] As MouseEvent)
			If tipShowing Then
				checkForTipChange([event])
			ElseIf showImmediately Then
				Dim component As JComponent = CType([event].source, JComponent)
				toolTipText = component.getToolTipText([event])
				If toolTipText IsNot Nothing Then
					preferredLocation = component.getToolTipLocation([event])
					mouseEvent = [event]
					insideComponent = component
					exitTimer.stop()
					showTipWindow()
				End If
			Else
				' Lazily lookup the values from within insideTimerAction
				insideComponent = CType([event].source, JComponent)
				mouseEvent = [event]
				toolTipText = Nothing
				enterTimer.restart()
			End If
		End Sub

		''' <summary>
		''' Checks to see if the tooltip needs to be changed in response to
		''' the MouseMoved event <code>event</code>.
		''' </summary>
		Private Sub checkForTipChange(ByVal [event] As MouseEvent)
			Dim component As JComponent = CType([event].source, JComponent)
			Dim newText As String = component.getToolTipText([event])
			Dim newPreferredLocation As Point = component.getToolTipLocation([event])

			If newText IsNot Nothing OrElse newPreferredLocation IsNot Nothing Then
				mouseEvent = [event]
				If ((newText IsNot Nothing AndAlso newText.Equals(toolTipText)) OrElse newText Is Nothing) AndAlso ((newPreferredLocation IsNot Nothing AndAlso newPreferredLocation.Equals(preferredLocation)) OrElse newPreferredLocation Is Nothing) Then
					If tipWindow IsNot Nothing Then
						insideTimer.restart()
					Else
						enterTimer.restart()
					End If
				Else
					toolTipText = newText
					preferredLocation = newPreferredLocation
					If showImmediately Then
						hideTipWindow()
						showTipWindow()
						exitTimer.stop()
					Else
						enterTimer.restart()
					End If
				End If
			Else
				toolTipText = Nothing
				preferredLocation = Nothing
				mouseEvent = Nothing
				insideComponent = Nothing
				hideTipWindow()
				enterTimer.stop()
				exitTimer.restart()
			End If
		End Sub

		Protected Friend Class insideTimerAction
			Implements ActionListener

			Private ReadOnly outerInstance As ToolTipManager

			Public Sub New(ByVal outerInstance As ToolTipManager)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				If outerInstance.insideComponent IsNot Nothing AndAlso outerInstance.insideComponent.showing Then
					' Lazy lookup
					If outerInstance.toolTipText Is Nothing AndAlso outerInstance.mouseEvent IsNot Nothing Then
						outerInstance.toolTipText = outerInstance.insideComponent.getToolTipText(outerInstance.mouseEvent)
						outerInstance.preferredLocation = outerInstance.insideComponent.getToolTipLocation(outerInstance.mouseEvent)
					End If
					If outerInstance.toolTipText IsNot Nothing Then
						outerInstance.showImmediately = True
						outerInstance.showTipWindow()
					Else
						outerInstance.insideComponent = Nothing
						outerInstance.toolTipText = Nothing
						outerInstance.preferredLocation = Nothing
						outerInstance.mouseEvent = Nothing
						outerInstance.hideTipWindow()
					End If
				End If
			End Sub
		End Class

		Protected Friend Class outsideTimerAction
			Implements ActionListener

			Private ReadOnly outerInstance As ToolTipManager

			Public Sub New(ByVal outerInstance As ToolTipManager)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				outerInstance.showImmediately = False
			End Sub
		End Class

		Protected Friend Class stillInsideTimerAction
			Implements ActionListener

			Private ReadOnly outerInstance As ToolTipManager

			Public Sub New(ByVal outerInstance As ToolTipManager)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				outerInstance.hideTipWindow()
				outerInstance.enterTimer.stop()
				outerInstance.showImmediately = False
				outerInstance.insideComponent = Nothing
				outerInstance.mouseEvent = Nothing
			End Sub
		End Class

	'   This listener is registered when the tooltip is first registered
	'   * on a component in order to catch the situation where the tooltip
	'   * was turned on while the mouse was already within the bounds of
	'   * the component.  This way, the tooltip will be initiated on a
	'   * mouse-entered or mouse-moved, whichever occurs first.  Once the
	'   * tooltip has been initiated, we can remove this listener and rely
	'   * solely on mouse-entered to initiate the tooltip.
	'   
		Private Class MoveBeforeEnterListener
			Inherits MouseMotionAdapter

			Private ReadOnly outerInstance As ToolTipManager

			Public Sub New(ByVal outerInstance As ToolTipManager)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub mouseMoved(ByVal e As MouseEvent)
				outerInstance.initiateToolTip(e)
			End Sub
		End Class

		Friend Shared Function frameForComponent(ByVal component As Component) As Frame
			Do While Not(TypeOf component Is Frame)
				component = component.parent
			Loop
			Return CType(component, Frame)
		End Function

	  Private Function createFocusChangeListener() As FocusListener
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'		Return New FocusAdapter()
	'	{
	'	  public void focusLost(FocusEvent evt)
	'	  {
	'		hideTipWindow();
	'		insideComponent = Nothing;
	'		JComponent c = (JComponent)evt.getSource();
	'		c.removeFocusListener(focusChangeListener);
	'	  }
	'	};
	  End Function

	  ' Returns: 0 no adjust
	  '         -1 can't fit
	  '         >0 adjust value by amount returned
	  Private Function getPopupFitWidth(ByVal popupRectInScreen As Rectangle, ByVal invoker As Component) As Integer
		If invoker IsNot Nothing Then
		  Dim parent As Container
		  parent = invoker.parent
		  Do While parent IsNot Nothing
			' fix internal frame size bug: 4139087 - 4159012
			If TypeOf parent Is JFrame OrElse TypeOf parent Is JDialog OrElse TypeOf parent Is JWindow Then ' no check for awt.Frame since we use Heavy tips
			  Return getWidthAdjust(parent.bounds,popupRectInScreen)
			ElseIf TypeOf parent Is JApplet OrElse TypeOf parent Is JInternalFrame Then
			  If popupFrameRect Is Nothing Then popupFrameRect = New Rectangle
			  Dim p As Point = parent.locationOnScreen
			  popupFrameRect.boundsnds(p.x,p.y, parent.bounds.width, parent.bounds.height)
			  Return getWidthAdjust(popupFrameRect,popupRectInScreen)
			End If
			  parent = parent.parent
		  Loop
		End If
		Return 0
	  End Function

	  ' Returns:  0 no adjust
	  '          >0 adjust by value return
	  Private Function getPopupFitHeight(ByVal popupRectInScreen As Rectangle, ByVal invoker As Component) As Integer
		If invoker IsNot Nothing Then
		  Dim parent As Container
		  parent = invoker.parent
		  Do While parent IsNot Nothing
			If TypeOf parent Is JFrame OrElse TypeOf parent Is JDialog OrElse TypeOf parent Is JWindow Then
			  Return getHeightAdjust(parent.bounds,popupRectInScreen)
			ElseIf TypeOf parent Is JApplet OrElse TypeOf parent Is JInternalFrame Then
			  If popupFrameRect Is Nothing Then popupFrameRect = New Rectangle
			  Dim p As Point = parent.locationOnScreen
			  popupFrameRect.boundsnds(p.x,p.y, parent.bounds.width, parent.bounds.height)
			  Return getHeightAdjust(popupFrameRect,popupRectInScreen)
			End If
			  parent = parent.parent
		  Loop
		End If
		Return 0
	  End Function

	  Private Function getHeightAdjust(ByVal a As Rectangle, ByVal b As Rectangle) As Integer
		If b.y >= a.y AndAlso (b.y + b.height) <= (a.y + a.height) Then
		  Return 0
		Else
		  Return (((b.y + b.height) - (a.y + a.height)) + 5)
		End If
	  End Function

	  ' Return the number of pixels over the edge we are extending.
	  ' If we are over the edge the ToolTipManager can adjust.
	  ' REMIND: what if the Tooltip is just too big to fit at all - we currently will just clip
	  Private Function getWidthAdjust(ByVal a As Rectangle, ByVal b As Rectangle) As Integer
		'    System.out.println("width b.x/b.width: " + b.x + "/" + b.width +
		'                 "a.x/a.width: " + a.x + "/" + a.width);
		If b.x >= a.x AndAlso (b.x + b.width) <= (a.x + a.width) Then
		  Return 0
		Else
		  Return (((b.x + b.width) - (a.x +a.width)) + 5)
		End If
	  End Function


		'
		' Actions
		'
		Private Sub show(ByVal source As JComponent)
			If tipWindow IsNot Nothing Then ' showing we unshow
				hideTipWindow()
				insideComponent = Nothing
			Else
				hideTipWindow() ' be safe
				enterTimer.stop()
				exitTimer.stop()
				insideTimer.stop()
				insideComponent = source
				If insideComponent IsNot Nothing Then
					toolTipText = insideComponent.toolTipText
					preferredLocation = New Point(10,insideComponent.height+ 10) ' manual set
					showTipWindow()
					' put a focuschange listener on to bring the tip down
					If focusChangeListener Is Nothing Then focusChangeListener = createFocusChangeListener()
					insideComponent.addFocusListener(focusChangeListener)
				End If
			End If
		End Sub

		Private Sub hide(ByVal source As JComponent)
			hideTipWindow()
			source.removeFocusListener(focusChangeListener)
			preferredLocation = Nothing
			insideComponent = Nothing
		End Sub

	'     This listener is registered when the tooltip is first registered
	'     * on a component in order to process accessibility keybindings.
	'     * This will apply globally across L&F
	'     *
	'     * Post Tip: Ctrl+F1
	'     * Unpost Tip: Esc and Ctrl+F1
	'     
		Private Class AccessibilityKeyListener
			Inherits KeyAdapter

			Private ReadOnly outerInstance As ToolTipManager

			Public Sub New(ByVal outerInstance As ToolTipManager)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub keyPressed(ByVal e As KeyEvent)
				If Not e.consumed Then
					Dim source As JComponent = CType(e.component, JComponent)
					Dim keyStrokeForEvent As KeyStroke = KeyStroke.getKeyStrokeForEvent(e)
					If outerInstance.hideTip.Equals(keyStrokeForEvent) Then
						If outerInstance.tipWindow IsNot Nothing Then
							outerInstance.hide(source)
							e.consume()
						End If
					ElseIf outerInstance.postTip.Equals(keyStrokeForEvent) Then
						' Shown tooltip will be hidden
						outerInstance.show(source)
						e.consume()
					End If
				End If
			End Sub
		End Class
	End Class

End Namespace
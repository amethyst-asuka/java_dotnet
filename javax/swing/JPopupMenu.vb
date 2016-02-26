Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports javax.accessibility
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

Namespace javax.swing




	''' <summary>
	''' An implementation of a popup menu -- a small window that pops up
	''' and displays a series of choices. A <code>JPopupMenu</code> is used for the
	''' menu that appears when the user selects an item on the menu bar.
	''' It is also used for "pull-right" menu that appears when the
	''' selects a menu item that activates it. Finally, a <code>JPopupMenu</code>
	''' can also be used anywhere else you want a menu to appear.  For
	''' example, when the user right-clicks in a specified area.
	''' <p>
	''' For information and examples of using popup menus, see
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/menu.html">How to Use Menus</a>
	''' in <em>The Java Tutorial.</em>
	''' <p>
	''' <strong>Warning:</strong> Swing is not thread safe. For more
	''' information see <a
	''' href="package-summary.html#threading">Swing's Threading
	''' Policy</a>.
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
	''' @beaninfo
	'''   attribute: isContainer false
	''' description: A small window that pops up and displays a series of choices.
	''' 
	''' @author Georges Saab
	''' @author David Karlton
	''' @author Arnaud Weber
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class JPopupMenu
		Inherits JComponent
		Implements Accessible, MenuElement

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "PopupMenuUI"

		''' <summary>
		''' Key used in AppContext to determine if light way popups are the default.
		''' </summary>
		Private Shared ReadOnly defaultLWPopupEnabledKey As Object = New StringBuilder("JPopupMenu.defaultLWPopupEnabledKey")

		''' <summary>
		''' Bug#4425878-Property javax.swing.adjustPopupLocationToFit introduced </summary>
		Friend Shared popupPostionFixDisabled As Boolean = False

		Shared Sub New()
			popupPostionFixDisabled = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("javax.swing.adjustPopupLocationToFit","")).Equals("false")

		End Sub

		<NonSerialized> _
		Friend invoker As Component
		<NonSerialized> _
		Friend popup As Popup
		<NonSerialized> _
		Friend frame As Frame
		Private desiredLocationX, desiredLocationY As Integer

		Private label As String = Nothing
		Private ___paintBorder As Boolean = True
		Private margin As Insets = Nothing

		''' <summary>
		''' Used to indicate if lightweight popups should be used.
		''' </summary>
		Private lightWeightPopup As Boolean = True

	'    
	'     * Model for the selected subcontrol.
	'     
		Private selectionModel As SingleSelectionModel

	'     Lock object used in place of class object for synchronization.
	'     * (4187686)
	'     
		Private Shared ReadOnly classLock As New Object

		' diagnostic aids -- should be false for production builds. 
		Private Const TRACE As Boolean = False ' trace creates and disposes
		Private Const VERBOSE As Boolean = False ' show reuse hits/misses
		Private Const DEBUG As Boolean = False ' show bad params, misc.

		''' <summary>
		'''  Sets the default value of the <code>lightWeightPopupEnabled</code>
		'''  property.
		''' </summary>
		'''  <param name="aFlag"> <code>true</code> if popups can be lightweight,
		'''               otherwise <code>false</code> </param>
		'''  <seealso cref= #getDefaultLightWeightPopupEnabled </seealso>
		'''  <seealso cref= #setLightWeightPopupEnabled </seealso>
		Public Shared Property defaultLightWeightPopupEnabled As Boolean
			Set(ByVal aFlag As Boolean)
				SwingUtilities.appContextPut(defaultLWPopupEnabledKey, Convert.ToBoolean(aFlag))
			End Set
			Get
				Dim b As Boolean? = CBool(SwingUtilities.appContextGet(defaultLWPopupEnabledKey))
				If b Is Nothing Then
					SwingUtilities.appContextPut(defaultLWPopupEnabledKey, Boolean.TRUE)
					Return True
				End If
				Return b
			End Get
		End Property


		''' <summary>
		''' Constructs a <code>JPopupMenu</code> without an "invoker".
		''' </summary>
		Public Sub New()
			Me.New(Nothing)
		End Sub

		''' <summary>
		''' Constructs a <code>JPopupMenu</code> with the specified title.
		''' </summary>
		''' <param name="label">  the string that a UI may use to display as a title
		''' for the popup menu. </param>
		Public Sub New(ByVal label As String)
			Me.label = label
			lightWeightPopup = defaultLightWeightPopupEnabled
			selectionModel = New DefaultSingleSelectionModel
			enableEvents(AWTEvent.MOUSE_EVENT_MASK)
			focusTraversalKeysEnabled = False
			updateUI()
		End Sub



		''' <summary>
		''' Returns the look and feel (L&amp;F) object that renders this component.
		''' </summary>
		''' <returns> the <code>PopupMenuUI</code> object that renders this component </returns>
		Public Overridable Property uI As javax.swing.plaf.PopupMenuUI
			Get
				Return CType(ui, javax.swing.plaf.PopupMenuUI)
			End Get
			Set(ByVal ui As javax.swing.plaf.PopupMenuUI)
				MyBase.uI = ui
			End Set
		End Property


		''' <summary>
		''' Resets the UI property to a value from the current look and feel.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), javax.swing.plaf.PopupMenuUI)
		End Sub


		''' <summary>
		''' Returns the name of the L&amp;F class that renders this component.
		''' </summary>
		''' <returns> the string "PopupMenuUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property

		Protected Friend Overridable Sub processFocusEvent(ByVal evt As FocusEvent)
			MyBase.processFocusEvent(evt)
		End Sub

		''' <summary>
		''' Processes key stroke events such as mnemonics and accelerators.
		''' </summary>
		''' <param name="evt">  the key event to be processed </param>
		Protected Friend Overrides Sub processKeyEvent(ByVal evt As KeyEvent)
			MenuSelectionManager.defaultManager().processKeyEvent(evt)
			If evt.consumed Then Return
			MyBase.processKeyEvent(evt)
		End Sub


		''' <summary>
		''' Returns the model object that handles single selections.
		''' </summary>
		''' <returns> the <code>selectionModel</code> property </returns>
		''' <seealso cref= SingleSelectionModel </seealso>
		Public Overridable Property selectionModel As SingleSelectionModel
			Get
				Return selectionModel
			End Get
			Set(ByVal model As SingleSelectionModel)
				selectionModel = model
			End Set
		End Property


		''' <summary>
		''' Appends the specified menu item to the end of this menu.
		''' </summary>
		''' <param name="menuItem"> the <code>JMenuItem</code> to add </param>
		''' <returns> the <code>JMenuItem</code> added </returns>
		Public Overridable Function add(ByVal menuItem As JMenuItem) As JMenuItem
			MyBase.add(menuItem)
			Return menuItem
		End Function

		''' <summary>
		''' Creates a new menu item with the specified text and appends
		''' it to the end of this menu.
		''' </summary>
		''' <param name="s"> the string for the menu item to be added </param>
		Public Overridable Function add(ByVal s As String) As JMenuItem
			Return add(New JMenuItem(s))
		End Function

		''' <summary>
		''' Appends a new menu item to the end of the menu which
		''' dispatches the specified <code>Action</code> object.
		''' </summary>
		''' <param name="a"> the <code>Action</code> to add to the menu </param>
		''' <returns> the new menu item </returns>
		''' <seealso cref= Action </seealso>
		Public Overridable Function add(ByVal a As Action) As JMenuItem
			Dim mi As JMenuItem = createActionComponent(a)
			mi.action = a
			add(mi)
			Return mi
		End Function

		''' <summary>
		''' Returns an point which has been adjusted to take into account of the
		''' desktop bounds, taskbar and multi-monitor configuration.
		''' <p>
		''' This adustment may be cancelled by invoking the application with
		''' -Djavax.swing.adjustPopupLocationToFit=false
		''' </summary>
		Friend Overridable Function adjustPopupLocationToFitScreen(ByVal xPosition As Integer, ByVal yPosition As Integer) As Point
			Dim ___popupLocation As New Point(xPosition, yPosition)

			If popupPostionFixDisabled = True OrElse GraphicsEnvironment.headless Then Return ___popupLocation

			' Get screen bounds
			Dim scrBounds As Rectangle
			Dim gc As GraphicsConfiguration = getCurrentGraphicsConfiguration(___popupLocation)
			Dim toolkit As Toolkit = Toolkit.defaultToolkit
			If gc IsNot Nothing Then
				' If we have GraphicsConfiguration use it to get screen bounds
				scrBounds = gc.bounds
			Else
				' If we don't have GraphicsConfiguration use primary screen
				scrBounds = New Rectangle(toolkit.screenSize)
			End If

			' Calculate the screen size that popup should fit
			Dim ___popupSize As Dimension = outerInstance.preferredSize
			Dim popupRightX As Long = CLng(Fix(___popupLocation.x)) + CLng(Fix(___popupSize.width))
			Dim popupBottomY As Long = CLng(Fix(___popupLocation.y)) + CLng(Fix(___popupSize.height))
			Dim scrWidth As Integer = scrBounds.width
			Dim scrHeight As Integer = scrBounds.height

			If Not canPopupOverlapTaskBar() Then
				' Insets include the task bar. Take them into account.
				Dim scrInsets As Insets = toolkit.getScreenInsets(gc)
				scrBounds.x += scrInsets.left
				scrBounds.y += scrInsets.top
				scrWidth -= scrInsets.left + scrInsets.right
				scrHeight -= scrInsets.top + scrInsets.bottom
			End If
			Dim scrRightX As Integer = scrBounds.x + scrWidth
			Dim scrBottomY As Integer = scrBounds.y + scrHeight

			' Ensure that popup menu fits the screen
			If popupRightX > (Long) scrRightX Then ___popupLocation.x = scrRightX - ___popupSize.width

			If popupBottomY > (Long) scrBottomY Then ___popupLocation.y = scrBottomY - ___popupSize.height

			If ___popupLocation.x < scrBounds.x Then ___popupLocation.x = scrBounds.x

			If ___popupLocation.y < scrBounds.y Then ___popupLocation.y = scrBounds.y

			Return ___popupLocation
		End Function

		''' <summary>
		''' Tries to find GraphicsConfiguration
		''' that contains the mouse cursor position.
		''' Can return null.
		''' </summary>
		Private Function getCurrentGraphicsConfiguration(ByVal popupLocation As Point) As GraphicsConfiguration
			Dim gc As GraphicsConfiguration = Nothing
			Dim ge As GraphicsEnvironment = GraphicsEnvironment.localGraphicsEnvironment
			Dim gd As GraphicsDevice() = ge.screenDevices
			For i As Integer = 0 To gd.Length - 1
				If gd(i).type = GraphicsDevice.TYPE_RASTER_SCREEN Then
					Dim dgc As GraphicsConfiguration = gd(i).defaultConfiguration
					If dgc.bounds.contains(popupLocation) Then
						gc = dgc
						Exit For
					End If
				End If
			Next i
			' If not found and we have invoker, ask invoker about his gc
			If gc Is Nothing AndAlso invoker IsNot Nothing Then gc = invoker.graphicsConfiguration
			Return gc
		End Function

		''' <summary>
		''' Returns whether popup is allowed to be shown above the task bar.
		''' </summary>
		Friend Shared Function canPopupOverlapTaskBar() As Boolean
			Dim result As Boolean = True

			Dim tk As Toolkit = Toolkit.defaultToolkit
			If TypeOf tk Is sun.awt.SunToolkit Then result = CType(tk, sun.awt.SunToolkit).canPopupOverlapTaskBar()

			Return result
		End Function

		''' <summary>
		''' Factory method which creates the <code>JMenuItem</code> for
		''' <code>Actions</code> added to the <code>JPopupMenu</code>.
		''' </summary>
		''' <param name="a"> the <code>Action</code> for the menu item to be added </param>
		''' <returns> the new menu item </returns>
		''' <seealso cref= Action
		''' 
		''' @since 1.3 </seealso>
		Protected Friend Overridable Function createActionComponent(ByVal a As Action) As JMenuItem
			Dim mi As JMenuItem = New JMenuItemAnonymousInnerClassHelper
			mi.horizontalTextPosition = JButton.TRAILING
			mi.verticalTextPosition = JButton.CENTER
			Return mi
		End Function

		Private Class JMenuItemAnonymousInnerClassHelper
			Inherits JMenuItem

			Protected Friend Overrides Function createActionPropertyChangeListener(ByVal a As Action) As PropertyChangeListener
				Dim pcl As PropertyChangeListener = outerInstance.createActionChangeListener(Me)
				If pcl Is Nothing Then pcl = MyBase.createActionPropertyChangeListener(a)
				Return pcl
			End Function
		End Class

		''' <summary>
		''' Returns a properly configured <code>PropertyChangeListener</code>
		''' which updates the control as changes to the <code>Action</code> occur.
		''' </summary>
		Protected Friend Overridable Function createActionChangeListener(ByVal b As JMenuItem) As PropertyChangeListener
			Return b.createActionPropertyChangeListener0(b.action)
		End Function

		''' <summary>
		''' Removes the component at the specified index from this popup menu.
		''' </summary>
		''' <param name="pos"> the position of the item to be removed </param>
		''' <exception cref="IllegalArgumentException"> if the value of
		'''                          <code>pos</code> &lt; 0, or if the value of
		'''                          <code>pos</code> is greater than the
		'''                          number of items </exception>
		Public Overridable Sub remove(ByVal pos As Integer)
			If pos < 0 Then Throw New System.ArgumentException("index less than zero.")
			If pos > componentCount -1 Then Throw New System.ArgumentException("index greater than the number of items.")
			MyBase.remove(pos)
		End Sub

		''' <summary>
		''' Sets the value of the <code>lightWeightPopupEnabled</code> property,
		''' which by default is <code>true</code>.
		''' By default, when a look and feel displays a popup,
		''' it can choose to
		''' use a lightweight (all-Java) popup.
		''' Lightweight popup windows are more efficient than heavyweight
		''' (native peer) windows,
		''' but lightweight and heavyweight components do not mix well in a GUI.
		''' If your application mixes lightweight and heavyweight components,
		''' you should disable lightweight popups.
		''' Some look and feels might always use heavyweight popups,
		''' no matter what the value of this property.
		''' </summary>
		''' <param name="aFlag">  <code>false</code> to disable lightweight popups
		''' @beaninfo
		''' description: Determines whether lightweight popups are used when possible
		'''      expert: true
		''' </param>
		''' <seealso cref= #isLightWeightPopupEnabled </seealso>
		Public Overridable Property lightWeightPopupEnabled As Boolean
			Set(ByVal aFlag As Boolean)
				' NOTE: this use to set the flag on a shared JPopupMenu, which meant
				' this effected ALL JPopupMenus.
				lightWeightPopup = aFlag
			End Set
			Get
				Return lightWeightPopup
			End Get
		End Property


		''' <summary>
		''' Returns the popup menu's label
		''' </summary>
		''' <returns> a string containing the popup menu's label </returns>
		''' <seealso cref= #setLabel </seealso>
		Public Overridable Property label As String
			Get
				Return label
			End Get
			Set(ByVal label As String)
				Dim oldValue As String = Me.label
				Me.label = label
				firePropertyChange("label", oldValue, label)
				If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, oldValue, label)
				invalidate()
				repaint()
			End Set
		End Property


		''' <summary>
		''' Appends a new separator at the end of the menu.
		''' </summary>
		Public Overridable Sub addSeparator()
			add(New JPopupMenu.Separator)
		End Sub

		''' <summary>
		''' Inserts a menu item for the specified <code>Action</code> object at
		''' a given position.
		''' </summary>
		''' <param name="a">  the <code>Action</code> object to insert </param>
		''' <param name="index">      specifies the position at which to insert the
		'''                   <code>Action</code>, where 0 is the first </param>
		''' <exception cref="IllegalArgumentException"> if <code>index</code> &lt; 0 </exception>
		''' <seealso cref= Action </seealso>
		Public Overridable Sub insert(ByVal a As Action, ByVal index As Integer)
			Dim mi As JMenuItem = createActionComponent(a)
			mi.action = a
			insert(mi, index)
		End Sub

		''' <summary>
		''' Inserts the specified component into the menu at a given
		''' position.
		''' </summary>
		''' <param name="component">  the <code>Component</code> to insert </param>
		''' <param name="index">      specifies the position at which
		'''                   to insert the component, where 0 is the first </param>
		''' <exception cref="IllegalArgumentException"> if <code>index</code> &lt; 0 </exception>
		Public Overridable Sub insert(ByVal component As Component, ByVal index As Integer)
			If index < 0 Then Throw New System.ArgumentException("index less than zero.")

			Dim nitems As Integer = componentCount
			' PENDING(ges): Why not use an array?
			Dim tempItems As New List(Of Component)

	'         Remove the item at index, nitems-index times
	'           storing them in a temporary vector in the
	'           order they appear on the menu.
	'           
			For i As Integer = index To nitems - 1
				tempItems.Add(getComponent(index))
				remove(index)
			Next i

			add(component)

	'         Add the removed items back to the menu, they are
	'           already in the correct order in the temp vector.
	'           
			For Each tempItem As Component In tempItems
				add(tempItem)
			Next tempItem
		End Sub

		''' <summary>
		'''  Adds a <code>PopupMenu</code> listener.
		''' </summary>
		'''  <param name="l">  the <code>PopupMenuListener</code> to add </param>
		Public Overridable Sub addPopupMenuListener(ByVal l As PopupMenuListener)
			listenerList.add(GetType(PopupMenuListener),l)
		End Sub

		''' <summary>
		''' Removes a <code>PopupMenu</code> listener.
		''' </summary>
		''' <param name="l">  the <code>PopupMenuListener</code> to remove </param>
		Public Overridable Sub removePopupMenuListener(ByVal l As PopupMenuListener)
			listenerList.remove(GetType(PopupMenuListener),l)
		End Sub

		''' <summary>
		''' Returns an array of all the <code>PopupMenuListener</code>s added
		''' to this JMenuItem with addPopupMenuListener().
		''' </summary>
		''' <returns> all of the <code>PopupMenuListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		Public Overridable Property popupMenuListeners As PopupMenuListener()
			Get
				Return listenerList.getListeners(GetType(PopupMenuListener))
			End Get
		End Property

		''' <summary>
		''' Adds a <code>MenuKeyListener</code> to the popup menu.
		''' </summary>
		''' <param name="l"> the <code>MenuKeyListener</code> to be added
		''' @since 1.5 </param>
		Public Overridable Sub addMenuKeyListener(ByVal l As MenuKeyListener)
			listenerList.add(GetType(MenuKeyListener), l)
		End Sub

		''' <summary>
		''' Removes a <code>MenuKeyListener</code> from the popup menu.
		''' </summary>
		''' <param name="l"> the <code>MenuKeyListener</code> to be removed
		''' @since 1.5 </param>
		Public Overridable Sub removeMenuKeyListener(ByVal l As MenuKeyListener)
			listenerList.remove(GetType(MenuKeyListener), l)
		End Sub

		''' <summary>
		''' Returns an array of all the <code>MenuKeyListener</code>s added
		''' to this JPopupMenu with addMenuKeyListener().
		''' </summary>
		''' <returns> all of the <code>MenuKeyListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.5 </returns>
		Public Overridable Property menuKeyListeners As MenuKeyListener()
			Get
				Return listenerList.getListeners(GetType(MenuKeyListener))
			End Get
		End Property

		''' <summary>
		''' Notifies <code>PopupMenuListener</code>s that this popup menu will
		''' become visible.
		''' </summary>
		Protected Friend Overridable Sub firePopupMenuWillBecomeVisible()
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As PopupMenuEvent=Nothing
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(PopupMenuListener) Then
					If e Is Nothing Then e = New PopupMenuEvent(Me)
					CType(___listeners(i+1), PopupMenuListener).popupMenuWillBecomeVisible(e)
				End If
			Next i
		End Sub

		''' <summary>
		''' Notifies <code>PopupMenuListener</code>s that this popup menu will
		''' become invisible.
		''' </summary>
		Protected Friend Overridable Sub firePopupMenuWillBecomeInvisible()
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As PopupMenuEvent=Nothing
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(PopupMenuListener) Then
					If e Is Nothing Then e = New PopupMenuEvent(Me)
					CType(___listeners(i+1), PopupMenuListener).popupMenuWillBecomeInvisible(e)
				End If
			Next i
		End Sub

		''' <summary>
		''' Notifies <code>PopupMenuListeners</code> that this popup menu is
		''' cancelled.
		''' </summary>
		Protected Friend Overridable Sub firePopupMenuCanceled()
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As PopupMenuEvent=Nothing
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(PopupMenuListener) Then
					If e Is Nothing Then e = New PopupMenuEvent(Me)
					CType(___listeners(i+1), PopupMenuListener).popupMenuCanceled(e)
				End If
			Next i
		End Sub

		''' <summary>
		''' Always returns true since popups, by definition, should always
		''' be on top of all other windows. </summary>
		''' <returns> true </returns>
		' package private
		Friend Overrides Function alwaysOnTop() As Boolean
			Return True
		End Function

		''' <summary>
		''' Lays out the container so that it uses the minimum space
		''' needed to display its contents.
		''' </summary>
		Public Overridable Sub pack()
			If popup IsNot Nothing Then
				Dim pref As Dimension = preferredSize

				If pref Is Nothing OrElse pref.width <> width OrElse pref.height <> height Then
					showPopup()
				Else
					validate()
				End If
			End If
		End Sub

		''' <summary>
		''' Sets the visibility of the popup menu.
		''' </summary>
		''' <param name="b"> true to make the popup visible, or false to
		'''          hide it
		''' @beaninfo
		'''           bound: true
		'''     description: Makes the popup visible </param>
		Public Overrides Property visible As Boolean
			Set(ByVal b As Boolean)
				If DEBUG Then Console.WriteLine("JPopupMenu.setVisible " & b)
    
				' Is it a no-op?
				If b = visible Then Return
    
				' if closing, first close all Submenus
				If b = False Then
    
					' 4234793: This is a workaround because JPopupMenu.firePopupMenuCanceled is
					' a protected method and cannot be called from BasicPopupMenuUI directly
					' The real solution could be to make
					' firePopupMenuCanceled public and call it directly.
					Dim doCanceled As Boolean? = CBool(getClientProperty("JPopupMenu.firePopupMenuCanceled"))
					If doCanceled IsNot Nothing AndAlso doCanceled Is Boolean.TRUE Then
						putClientProperty("JPopupMenu.firePopupMenuCanceled", Boolean.FALSE)
						firePopupMenuCanceled()
					End If
					selectionModel.clearSelection()
    
				Else
					' This is a popup menu with MenuElement children,
					' set selection path before popping up!
					If popupMenu Then
						Dim [me] As MenuElement() = New MenuElement(0){}
						[me](0) = Me
						MenuSelectionManager.defaultManager().selectedPath = [me]
					End If
				End If
    
				If b Then
					firePopupMenuWillBecomeVisible()
					showPopup()
					firePropertyChange("visible", Boolean.FALSE, Boolean.TRUE)
    
    
				ElseIf popup IsNot Nothing Then
					firePopupMenuWillBecomeInvisible()
					popup.hide()
					popup = Nothing
					firePropertyChange("visible", Boolean.TRUE, Boolean.FALSE)
					' 4694797: When popup menu is made invisible, selected path
					' should be cleared
					If popupMenu Then MenuSelectionManager.defaultManager().clearSelectedPath()
				End If
			End Set
			Get
				Return popup IsNot Nothing
			End Get
		End Property

		''' <summary>
		''' Retrieves <code>Popup</code> instance from the
		''' <code>PopupMenuUI</code> that has had <code>show</code> invoked on
		''' it. If the current <code>popup</code> is non-null,
		''' this will invoke <code>dispose</code> of it, and then
		''' <code>show</code> the new one.
		''' <p>
		''' This does NOT fire any events, it is up the caller to dispatch
		''' the necessary events.
		''' </summary>
		Private Sub showPopup()
			Dim oldPopup As Popup = popup

			If oldPopup IsNot Nothing Then oldPopup.hide()
			Dim ___popupFactory As PopupFactory = PopupFactory.sharedInstance

			If lightWeightPopupEnabled Then
				___popupFactory.popupType = PopupFactory.LIGHT_WEIGHT_POPUP
			Else
				___popupFactory.popupType = PopupFactory.HEAVY_WEIGHT_POPUP
			End If

			' adjust the location of the popup
			Dim p As Point = adjustPopupLocationToFitScreen(desiredLocationX,desiredLocationY)
			desiredLocationX = p.x
			desiredLocationY = p.y

			Dim newPopup As Popup = uI.getPopup(Me, desiredLocationX, desiredLocationY)

			___popupFactory.popupType = PopupFactory.LIGHT_WEIGHT_POPUP
			popup = newPopup
			newPopup.show()
		End Sub


		''' <summary>
		''' Sets the location of the upper left corner of the
		''' popup menu using x, y coordinates.
		''' <p>
		''' The method changes the geometry-related data. Therefore,
		''' the native windowing system may ignore such requests, or it may modify
		''' the requested data, so that the {@code JPopupMenu} object is placed and sized
		''' in a way that corresponds closely to the desktop settings.
		''' </summary>
		''' <param name="x"> the x coordinate of the popup's new position
		'''          in the screen's coordinate space </param>
		''' <param name="y"> the y coordinate of the popup's new position
		'''          in the screen's coordinate space
		''' @beaninfo
		''' description: The location of the popup menu. </param>
		Public Overridable Sub setLocation(ByVal x As Integer, ByVal y As Integer)
			Dim oldX As Integer = desiredLocationX
			Dim oldY As Integer = desiredLocationY

			desiredLocationX = x
			desiredLocationY = y
			If popup IsNot Nothing AndAlso (x <> oldX OrElse y <> oldY) Then showPopup()
		End Sub

		''' <summary>
		''' Returns true if the popup menu is a standalone popup menu
		''' rather than the submenu of a <code>JMenu</code>.
		''' </summary>
		''' <returns> true if this menu is a standalone popup menu, otherwise false </returns>
		Private Property popupMenu As Boolean
			Get
				Return ((invoker IsNot Nothing) AndAlso Not(TypeOf invoker Is JMenu))
			End Get
		End Property

		''' <summary>
		''' Returns the component which is the 'invoker' of this
		''' popup menu.
		''' </summary>
		''' <returns> the <code>Component</code> in which the popup menu is displayed </returns>
		Public Overridable Property invoker As Component
			Get
				Return Me.invoker
			End Get
			Set(ByVal invoker As Component)
				Dim oldInvoker As Component = Me.invoker
				Me.invoker = invoker
				If (oldInvoker IsNot Me.invoker) AndAlso (ui IsNot Nothing) Then
					ui.uninstallUI(Me)
					ui.installUI(Me)
				End If
				invalidate()
			End Set
		End Property


		''' <summary>
		''' Displays the popup menu at the position x,y in the coordinate
		''' space of the component invoker.
		''' </summary>
		''' <param name="invoker"> the component in whose space the popup menu is to appear </param>
		''' <param name="x"> the x coordinate in invoker's coordinate space at which
		''' the popup menu is to be displayed </param>
		''' <param name="y"> the y coordinate in invoker's coordinate space at which
		''' the popup menu is to be displayed </param>
		Public Overridable Sub show(ByVal invoker As Component, ByVal x As Integer, ByVal y As Integer)
			If DEBUG Then Console.WriteLine("in JPopupMenu.show ")
			invoker = invoker
			Dim newFrame As Frame = getFrame(invoker)
			If newFrame IsNot frame Then
				' Use the invoker's frame so that events
				' are propagated properly
				If newFrame IsNot Nothing Then
					Me.frame = newFrame
					If popup IsNot Nothing Then visible = False
				End If
			End If
			Dim invokerOrigin As Point
			If invoker IsNot Nothing Then
				invokerOrigin = invoker.locationOnScreen

				' To avoid integer overflow
				Dim lx, ly As Long
				lx = (CLng(Fix(invokerOrigin.x))) + (CLng(x))
				ly = (CLng(Fix(invokerOrigin.y))) + (CLng(y))
				If lx > Integer.MaxValue Then lx = Integer.MaxValue
				If lx < Integer.MinValue Then lx = Integer.MinValue
				If ly > Integer.MaxValue Then ly = Integer.MaxValue
				If ly < Integer.MinValue Then ly = Integer.MinValue

				locationion(CInt(lx), CInt(ly))
			Else
				locationion(x, y)
			End If
			visible = True
		End Sub

		''' <summary>
		''' Returns the popup menu which is at the root of the menu system
		''' for this popup menu.
		''' </summary>
		''' <returns> the topmost grandparent <code>JPopupMenu</code> </returns>
		Friend Overridable Property rootPopupMenu As JPopupMenu
			Get
				Dim mp As JPopupMenu = Me
				Do While (mp IsNot Nothing) AndAlso (mp.popupMenu<>True) AndAlso (mp.invoker IsNot Nothing) AndAlso (mp.invoker.parent IsNot Nothing) AndAlso (TypeOf mp.invoker.parent Is JPopupMenu)
					mp = CType(mp.invoker.parent, JPopupMenu)
				Loop
				Return mp
			End Get
		End Property

		''' <summary>
		''' Returns the component at the specified index.
		''' </summary>
		''' <param name="i">  the index of the component, where 0 is the first </param>
		''' <returns> the <code>Component</code> at that index </returns>
		''' @deprecated replaced by <seealso cref="java.awt.Container#getComponent(int)"/> 
		<Obsolete("replaced by <seealso cref="java.awt.Container#getComponent(int)"/>")> _
		Public Overridable Function getComponentAtIndex(ByVal i As Integer) As Component
			Return getComponent(i)
		End Function

		''' <summary>
		''' Returns the index of the specified component.
		''' </summary>
		''' <param name="c"> the <code>Component</code> to find </param>
		''' <returns> the index of the component, where 0 is the first;
		'''         or -1 if the component is not found </returns>
		Public Overridable Function getComponentIndex(ByVal c As Component) As Integer
			Dim ncomponents As Integer = Me.componentCount
			Dim ___component As Component() = Me.components
			For i As Integer = 0 To ncomponents - 1
				Dim comp As Component = ___component(i)
				If comp Is c Then Return i
			Next i
			Return -1
		End Function

		''' <summary>
		''' Sets the size of the Popup window using a <code>Dimension</code> object.
		''' This is equivalent to <code>setPreferredSize(d)</code>.
		''' </summary>
		''' <param name="d">   the <code>Dimension</code> specifying the new size
		''' of this component.
		''' @beaninfo
		''' description: The size of the popup menu </param>
		Public Overridable Property popupSize As Dimension
			Set(ByVal d As Dimension)
				Dim oldSize As Dimension = preferredSize
    
				preferredSize = d
				If popup IsNot Nothing Then
					Dim newSize As Dimension = preferredSize
    
					If Not oldSize.Equals(newSize) Then showPopup()
				End If
			End Set
		End Property

		''' <summary>
		''' Sets the size of the Popup window to the specified width and
		''' height. This is equivalent to
		'''  <code>setPreferredSize(new Dimension(width, height))</code>.
		''' </summary>
		''' <param name="width"> the new width of the Popup in pixels </param>
		''' <param name="height"> the new height of the Popup in pixels
		''' @beaninfo
		''' description: The size of the popup menu </param>
		Public Overridable Sub setPopupSize(ByVal width As Integer, ByVal height As Integer)
			popupSize = New Dimension(width, height)
		End Sub

		''' <summary>
		''' Sets the currently selected component,  This will result
		''' in a change to the selection model.
		''' </summary>
		''' <param name="sel"> the <code>Component</code> to select
		''' @beaninfo
		''' description: The selected component on the popup menu
		'''      expert: true
		'''      hidden: true </param>
		Public Overridable Property selected As Component
			Set(ByVal sel As Component)
				Dim model As SingleSelectionModel = selectionModel
				Dim index As Integer = getComponentIndex(sel)
				model.selectedIndex = index
			End Set
		End Property

		''' <summary>
		''' Checks whether the border should be painted.
		''' </summary>
		''' <returns> true if the border is painted, false otherwise </returns>
		''' <seealso cref= #setBorderPainted </seealso>
		Public Overridable Property borderPainted As Boolean
			Get
				Return ___paintBorder
			End Get
			Set(ByVal b As Boolean)
				___paintBorder = b
				repaint()
			End Set
		End Property


		''' <summary>
		''' Paints the popup menu's border if the <code>borderPainted</code>
		''' property is <code>true</code>. </summary>
		''' <param name="g">  the <code>Graphics</code> object
		''' </param>
		''' <seealso cref= JComponent#paint </seealso>
		''' <seealso cref= JComponent#setBorder </seealso>
		Protected Friend Overrides Sub paintBorder(ByVal g As Graphics)
			If borderPainted Then MyBase.paintBorder(g)
		End Sub

		''' <summary>
		''' Returns the margin, in pixels, between the popup menu's border and
		''' its containers.
		''' </summary>
		''' <returns> an <code>Insets</code> object containing the margin values. </returns>
		Public Overridable Property margin As Insets
			Get
				If margin Is Nothing Then
					Return New Insets(0,0,0,0)
				Else
					Return margin
				End If
			End Get
		End Property


		''' <summary>
		''' Examines the list of menu items to determine whether
		''' <code>popup</code> is a popup menu.
		''' </summary>
		''' <param name="popup">  a <code>JPopupMenu</code> </param>
		''' <returns> true if <code>popup</code> </returns>
		Friend Overridable Function isSubPopupMenu(ByVal popup As JPopupMenu) As Boolean
			Dim ncomponents As Integer = Me.componentCount
			Dim ___component As Component() = Me.components
			For i As Integer = 0 To ncomponents - 1
				Dim comp As Component = ___component(i)
				If TypeOf comp Is JMenu Then
					Dim menu As JMenu = CType(comp, JMenu)
					Dim subPopup As JPopupMenu = menu.popupMenu
					If subPopup Is popup Then Return True
					If subPopup.isSubPopupMenu(popup) Then Return True
				End If
			Next i
			Return False
		End Function


		Private Shared Function getFrame(ByVal c As Component) As Frame
			Dim w As Component = c

			Do While Not(TypeOf w Is Frame) AndAlso (w IsNot Nothing)
				w = w.parent
			Loop
			Return CType(w, Frame)
		End Function


		''' <summary>
		''' Returns a string representation of this <code>JPopupMenu</code>.
		''' This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JPopupMenu</code>. </returns>
		Protected Friend Overrides Function paramString() As String
			Dim labelString As String = (If(label IsNot Nothing, label, ""))
			Dim paintBorderString As String = (If(___paintBorder, "true", "false"))
			Dim marginString As String = (If(margin IsNot Nothing, margin.ToString(), ""))
			Dim lightWeightPopupEnabledString As String = (If(lightWeightPopupEnabled, "true", "false"))
			Return MyBase.paramString() & ",desiredLocationX=" & desiredLocationX & ",desiredLocationY=" & desiredLocationY & ",label=" & labelString & ",lightWeightPopupEnabled=" & lightWeightPopupEnabledString & ",margin=" & marginString & ",paintBorder=" & paintBorderString
		End Function

	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this JPopupMenu.
		''' For JPopupMenus, the AccessibleContext takes the form of an
		''' AccessibleJPopupMenu.
		''' A new AccessibleJPopupMenu instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJPopupMenu that serves as the
		'''         AccessibleContext of this JPopupMenu </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJPopupMenu(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JPopupMenu</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to popup menu user-interface
		''' elements.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend Class AccessibleJPopupMenu
			Inherits AccessibleJComponent
			Implements PropertyChangeListener

			Private ReadOnly outerInstance As JPopupMenu


			''' <summary>
			''' AccessibleJPopupMenu constructor
			''' 
			''' @since 1.5
			''' </summary>
			Protected Friend Sub New(ByVal outerInstance As JPopupMenu)
					Me.outerInstance = outerInstance
				outerInstance.addPropertyChangeListener(Me)
			End Sub

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of
			''' the object </returns>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.POPUP_MENU
				End Get
			End Property

			''' <summary>
			''' This method gets called when a bound property is changed. </summary>
			''' <param name="e"> A <code>PropertyChangeEvent</code> object describing
			''' the event source and the property that has changed. Must not be null.
			''' </param>
			''' <exception cref="NullPointerException"> if the parameter is null.
			''' @since 1.5 </exception>
			Public Overridable Sub propertyChange(ByVal e As PropertyChangeEvent)
				Dim propertyName As String = e.propertyName
				If propertyName = "visible" Then
					If e.oldValue = Boolean.FALSE AndAlso e.newValue = Boolean.TRUE Then
						handlePopupIsVisibleEvent(True)

					ElseIf e.oldValue = Boolean.TRUE AndAlso e.newValue = Boolean.FALSE Then
						handlePopupIsVisibleEvent(False)
					End If
				End If
			End Sub

	'        
	'         * Handles popup "visible" PropertyChangeEvent
	'         
			Private Sub handlePopupIsVisibleEvent(ByVal visible As Boolean)
				If visible Then
					' notify listeners that the popup became visible
					outerInstance.firePropertyChange(ACCESSIBLE_STATE_PROPERTY, Nothing, AccessibleState.VISIBLE)
					' notify listeners that a popup list item is selected
					fireActiveDescendant()
				Else
					' notify listeners that the popup became hidden
					outerInstance.firePropertyChange(ACCESSIBLE_STATE_PROPERTY, AccessibleState.VISIBLE, Nothing)
				End If
			End Sub

	'        
	'         * Fires AccessibleActiveDescendant PropertyChangeEvent to notify listeners
	'         * on the popup menu invoker that a popup list item has been selected
	'         
			Private Sub fireActiveDescendant()
				If TypeOf JPopupMenu.this Is javax.swing.plaf.basic.BasicComboPopup Then
					' get the popup list
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim popupList As JList(Of ?) = CType(JPopupMenu.this, javax.swing.plaf.basic.BasicComboPopup).list
					If popupList Is Nothing Then Return

					' get the first selected item
					Dim ac As AccessibleContext = popupList.accessibleContext
					Dim selection As AccessibleSelection = ac.accessibleSelection
					If selection Is Nothing Then Return
					Dim a As Accessible = selection.getAccessibleSelection(0)
					If a Is Nothing Then Return
					Dim selectedItem As AccessibleContext = a.accessibleContext

					' fire the event with the popup invoker as the source.
					If selectedItem IsNot Nothing AndAlso outerInstance.invoker IsNot Nothing Then
						Dim invokerContext As AccessibleContext = outerInstance.invoker.accessibleContext
						If invokerContext IsNot Nothing Then invokerContext.firePropertyChange(ACCESSIBLE_ACTIVE_DESCENDANT_PROPERTY, Nothing, selectedItem)
					End If
				End If
			End Sub
		End Class ' inner class AccessibleJPopupMenu


	'//////////
	' Serialization support.
	'//////////
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			Dim values As New List(Of Object)

			s.defaultWriteObject()
			' Save the invoker, if its Serializable.
			If invoker IsNot Nothing AndAlso TypeOf invoker Is java.io.Serializable Then
				values.Add("invoker")
				values.Add(invoker)
			End If
			' Save the popup, if its Serializable.
			If popup IsNot Nothing AndAlso TypeOf popup Is java.io.Serializable Then
				values.Add("popup")
				values.Add(popup)
			End If
			s.writeObject(values)

			If uIClassID.Equals(uiClassID) Then
				Dim count As SByte = JComponent.getWriteObjCounter(Me)
				count -= 1
				JComponent.writeObjCounterter(Me, count)
				If count = 0 AndAlso ui IsNot Nothing Then ui.installUI(Me)
			End If
		End Sub

		' implements javax.swing.MenuElement
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim values As List(Of ?) = CType(s.readObject(), ArrayList)
			Dim indexCounter As Integer = 0
			Dim maxCounter As Integer = values.Count

			If indexCounter < maxCounter AndAlso values(indexCounter).Equals("invoker") Then
				indexCounter += 1
				invoker = CType(values(indexCounter), Component)
				indexCounter += 1
			End If
			If indexCounter < maxCounter AndAlso values(indexCounter).Equals("popup") Then
				indexCounter += 1
				popup = CType(values(indexCounter), Popup)
				indexCounter += 1
			End If
		End Sub


		''' <summary>
		''' This method is required to conform to the
		''' <code>MenuElement</code> interface, but it not implemented. </summary>
		''' <seealso cref= MenuElement#processMouseEvent(MouseEvent, MenuElement[], MenuSelectionManager) </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public void processMouseEvent(MouseEvent event,MenuElement path() ,MenuSelectionManager manager)

		''' <summary>
		''' Processes a key event forwarded from the
		''' <code>MenuSelectionManager</code> and changes the menu selection,
		''' if necessary, by using <code>MenuSelectionManager</code>'s API.
		''' <p>
		''' Note: you do not have to forward the event to sub-components.
		''' This is done automatically by the <code>MenuSelectionManager</code>.
		''' </summary>
		''' <param name="e">  a <code>KeyEvent</code> </param>
		''' <param name="path"> the <code>MenuElement</code> path array </param>
		''' <param name="manager">   the <code>MenuSelectionManager</code> </param>
		public void processKeyEvent(KeyEvent e, MenuElement path() , MenuSelectionManager manager)
			Dim mke As New MenuKeyEvent(e.component, e.iD, e.when, e.modifiers, e.keyCode, e.keyChar, path, manager)
			processMenuKeyEvent(mke)

			If mke.consumed Then e.consume()

		''' <summary>
		''' Handles a keystroke in a menu.
		''' </summary>
		''' <param name="e">  a <code>MenuKeyEvent</code> object
		''' @since 1.5 </param>
		private void processMenuKeyEvent(MenuKeyEvent e)
			Select Case e.iD
			Case KeyEvent.KEY_PRESSED
				fireMenuKeyPressed(e)
			Case KeyEvent.KEY_RELEASED
				fireMenuKeyReleased(e)
			Case KeyEvent.KEY_TYPED
				fireMenuKeyTyped(e)
			Case Else
			End Select

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.
		''' </summary>
		''' <param name="event"> a <code>MenuKeyEvent</code> </param>
		''' <seealso cref= EventListenerList </seealso>
		private void fireMenuKeyPressed(MenuKeyEvent [event])
			Dim ___listeners As Object() = listenerList.listenerList
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(MenuKeyListener) Then CType(___listeners(i+1), MenuKeyListener).menuKeyPressed([event])
			Next i

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.
		''' </summary>
		''' <param name="event"> a <code>MenuKeyEvent</code> </param>
		''' <seealso cref= EventListenerList </seealso>
		private void fireMenuKeyReleased(MenuKeyEvent [event])
			Dim ___listeners As Object() = listenerList.listenerList
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(MenuKeyListener) Then CType(___listeners(i+1), MenuKeyListener).menuKeyReleased([event])
			Next i

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.
		''' </summary>
		''' <param name="event"> a <code>MenuKeyEvent</code> </param>
		''' <seealso cref= EventListenerList </seealso>
		private void fireMenuKeyTyped(MenuKeyEvent [event])
			Dim ___listeners As Object() = listenerList.listenerList
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(MenuKeyListener) Then CType(___listeners(i+1), MenuKeyListener).menuKeyTyped([event])
			Next i

		''' <summary>
		''' Messaged when the menubar selection changes to activate or
		''' deactivate this menu. This implements the
		''' <code>javax.swing.MenuElement</code> interface.
		''' Overrides <code>MenuElement.menuSelectionChanged</code>.
		''' </summary>
		''' <param name="isIncluded">  true if this menu is active, false if
		'''        it is not </param>
		''' <seealso cref= MenuElement#menuSelectionChanged(boolean) </seealso>
		public void menuSelectionChanged(Boolean isIncluded)
			If DEBUG Then Console.WriteLine("In JPopupMenu.menuSelectionChanged " & isIncluded)
			If TypeOf invoker Is JMenu Then
				Dim m As JMenu = CType(invoker, JMenu)
				If isIncluded Then
					m.popupMenuVisible = True
				Else
					m.popupMenuVisible = False
				End If
			End If
			If popupMenu AndAlso (Not isIncluded) Then visible = False

		''' <summary>
		''' Returns an array of <code>MenuElement</code>s containing the submenu
		''' for this menu component.  It will only return items conforming to
		''' the <code>JMenuElement</code> interface.
		''' If popup menu is <code>null</code> returns
		''' an empty array.  This method is required to conform to the
		''' <code>MenuElement</code> interface.
		''' </summary>
		''' <returns> an array of <code>MenuElement</code> objects </returns>
		''' <seealso cref= MenuElement#getSubElements </seealso>
		public MenuElement() subElements
			Dim result As MenuElement()
			Dim tmp As New List(Of MenuElement)
			Dim c As Integer = componentCount
			Dim i As Integer
			Dim m As Component

			For i = 0 To c - 1
				m = getComponent(i)
				If TypeOf m Is MenuElement Then tmp.Add(CType(m, MenuElement))
			Next i

			result = New MenuElement(tmp.Count - 1){}
			i=0
			c=tmp.Count
			Do While i < c
				result(i) = tmp(i)
				i += 1
			Loop
			Return result

		''' <summary>
		''' Returns this <code>JPopupMenu</code> component. </summary>
		''' <returns> this <code>JPopupMenu</code> object </returns>
		''' <seealso cref= MenuElement#getComponent </seealso>
		public Component component
			Return Me


		''' <summary>
		''' A popup menu-specific separator.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		public static class Separator extends JSeparator
			public Separator()
				MyBase(JSeparator.HORIZONTAL)

			''' <summary>
			''' Returns the name of the L&amp;F class that renders this component.
			''' </summary>
			''' <returns> the string "PopupMenuSeparatorUI" </returns>
			''' <seealso cref= JComponent#getUIClassID </seealso>
			''' <seealso cref= UIDefaults#getUI </seealso>
			public String uIClassID
				Return "PopupMenuSeparatorUI"


		''' <summary>
		''' Returns true if the <code>MouseEvent</code> is considered a popup trigger
		''' by the <code>JPopupMenu</code>'s currently installed UI.
		''' </summary>
		''' <returns> true if the mouse event is a popup trigger
		''' @since 1.3 </returns>
		public Boolean isPopupTrigger(MouseEvent e)
			Return uI.isPopupTrigger(e)
	End Class

End Namespace
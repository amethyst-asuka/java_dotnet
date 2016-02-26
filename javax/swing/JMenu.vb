Imports System
Imports System.Collections.Generic
Imports javax.swing.event
Imports javax.swing.plaf
Imports javax.swing.plaf.basic
Imports javax.accessibility

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
	''' An implementation of a menu -- a popup window containing
	''' <code>JMenuItem</code>s that
	''' is displayed when the user selects an item on the <code>JMenuBar</code>.
	''' In addition to <code>JMenuItem</code>s, a <code>JMenu</code> can
	''' also contain <code>JSeparator</code>s.
	''' <p>
	''' In essence, a menu is a button with an associated <code>JPopupMenu</code>.
	''' When the "button" is pressed, the <code>JPopupMenu</code> appears. If the
	''' "button" is on the <code>JMenuBar</code>, the menu is a top-level window.
	''' If the "button" is another menu item, then the <code>JPopupMenu</code> is
	''' "pull-right" menu.
	''' <p>
	''' Menus can be configured, and to some degree controlled, by
	''' <code><a href="Action.html">Action</a></code>s.  Using an
	''' <code>Action</code> with a menu has many benefits beyond directly
	''' configuring a menu.  Refer to <a href="Action.html#buttonActions">
	''' Swing Components Supporting <code>Action</code></a> for more
	''' details, and you can find more information in <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/misc/action.html">How
	''' to Use Actions</a>, a section in <em>The Java Tutorial</em>.
	''' <p>
	''' For information and examples of using menus see
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/menu.html">How to Use Menus</a>,
	''' a section in <em>The Java Tutorial.</em>
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
	'''   attribute: isContainer true
	''' description: A popup window containing menu items displayed in a menu bar.
	''' 
	''' @author Georges Saab
	''' @author David Karlton
	''' @author Arnaud Weber </summary>
	''' <seealso cref= JMenuItem </seealso>
	''' <seealso cref= JSeparator </seealso>
	''' <seealso cref= JMenuBar </seealso>
	''' <seealso cref= JPopupMenu </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class JMenu
		Inherits JMenuItem
		Implements Accessible, MenuElement

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "MenuUI"

	'    
	'     * The popup menu portion of the menu.
	'     
		Private popupMenu As JPopupMenu

	'    
	'     * The button's model listeners.  Default is <code>null</code>.
	'     
		Private menuChangeListener As ChangeListener = Nothing

	'    
	'     * Only one <code>MenuEvent</code> is needed for each menu since the
	'     * event's only state is the source property.  The source of events
	'     * generated is always "this".  Default is <code>null</code>.
	'     
		Private menuEvent As MenuEvent = Nothing

	'    
	'     * Used by the look and feel (L&F) code to handle
	'     * implementation specific menu behaviors.
	'     
		Private delay As Integer

	'     
	'      * Location of the popup component. Location is <code>null</code>
	'      * if it was not customized by <code>setMenuLocation</code>
	'      
		 Private customMenuLocation As java.awt.Point = Nothing

		' Diagnostic aids -- should be false for production builds. 
		Private Const TRACE As Boolean = False ' trace creates and disposes
		Private Const VERBOSE As Boolean = False ' show reuse hits/misses
		Private Const DEBUG As Boolean = False ' show bad params, misc.

		''' <summary>
		''' Constructs a new <code>JMenu</code> with no text.
		''' </summary>
		Public Sub New()
			Me.New("")
		End Sub

		''' <summary>
		''' Constructs a new <code>JMenu</code> with the supplied string
		''' as its text.
		''' </summary>
		''' <param name="s">  the text for the menu label </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub

		''' <summary>
		''' Constructs a menu whose properties are taken from the
		''' <code>Action</code> supplied. </summary>
		''' <param name="a"> an <code>Action</code>
		''' 
		''' @since 1.3 </param>
		Public Sub New(ByVal a As Action)
			Me.New()
			action = a
		End Sub

		''' <summary>
		''' Constructs a new <code>JMenu</code> with the supplied string as
		''' its text and specified as a tear-off menu or not.
		''' </summary>
		''' <param name="s"> the text for the menu label </param>
		''' <param name="b"> can the menu be torn off (not yet implemented) </param>
		Public Sub New(ByVal s As String, ByVal b As Boolean)
			Me.New(s)
		End Sub


		''' <summary>
		''' Overriden to do nothing. We want JMenu to be focusable, but
		''' <code>JMenuItem</code> doesn't want to be, thus we override this
		''' do nothing. We don't invoke <code>setFocusable(true)</code> after
		''' super's constructor has completed as this has the side effect that
		''' <code>JMenu</code> will be considered traversable via the
		''' keyboard, which we don't want. Making a Component traversable by
		''' the keyboard after invoking <code>setFocusable(true)</code> is OK,
		''' as <code>setFocusable</code> is new API
		''' and is speced as such, but internally we don't want to use it like
		''' this else we change the keyboard traversability.
		''' </summary>
		Friend Overrides Sub initFocusability()
		End Sub

		''' <summary>
		''' Resets the UI property with a value from the current look and feel.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), MenuItemUI)

			If popupMenu IsNot Nothing Then popupMenu.uI = CType(UIManager.getUI(popupMenu), PopupMenuUI)

		End Sub


		''' <summary>
		''' Returns the name of the L&amp;F class that renders this component.
		''' </summary>
		''' <returns> the string "MenuUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property

		'    public void repaint(long tm, int x, int y, int width, int height) {
		'        Thread.currentThread().dumpStack();
		'        super.repaint(tm,x,y,width,height);
		'    }

		''' <summary>
		''' Sets the data model for the "menu button" -- the label
		''' that the user clicks to open or close the menu.
		''' </summary>
		''' <param name="newModel"> the <code>ButtonModel</code> </param>
		''' <seealso cref= #getModel
		''' @beaninfo
		''' description: The menu's model
		'''       bound: true
		'''      expert: true
		'''      hidden: true </seealso>
		Public Overrides Property model As ButtonModel
			Set(ByVal newModel As ButtonModel)
				Dim oldModel As ButtonModel = model
    
				MyBase.model = newModel
    
				If oldModel IsNot Nothing AndAlso menuChangeListener IsNot Nothing Then
					oldModel.removeChangeListener(menuChangeListener)
					menuChangeListener = Nothing
				End If
    
				model = newModel
    
				If newModel IsNot Nothing Then
					menuChangeListener = createMenuChangeListener()
					newModel.addChangeListener(menuChangeListener)
				End If
			End Set
		End Property

		''' <summary>
		''' Returns true if the menu is currently selected (highlighted).
		''' </summary>
		''' <returns> true if the menu is selected, else false </returns>
		Public Property Overrides selected As Boolean
			Get
				Return model.selected
			End Get
			Set(ByVal b As Boolean)
				Dim ___model As ButtonModel = model
				Dim oldValue As Boolean = ___model.selected
    
				' TIGER - 4840653
				' Removed code which fired an AccessibleState.SELECTED
				' PropertyChangeEvent since this resulted in two
				' identical events being fired since
				' AbstractButton.fireItemStateChanged also fires the
				' same event. This caused screen readers to speak the
				' name of the item twice.
    
				If b <> ___model.selected Then model.selected = b
			End Set
		End Property


		''' <summary>
		''' Returns true if the menu's popup window is visible.
		''' </summary>
		''' <returns> true if the menu is visible, else false </returns>
		Public Overridable Property popupMenuVisible As Boolean
			Get
				ensurePopupMenuCreated()
				Return popupMenu.visible
			End Get
			Set(ByVal b As Boolean)
				If DEBUG Then Console.WriteLine("in JMenu.setPopupMenuVisible " & b)
    
				Dim isVisible As Boolean = popupMenuVisible
				If b <> isVisible AndAlso (enabled OrElse (Not b)) Then
					ensurePopupMenuCreated()
					If (b=True) AndAlso showing Then
						' Set location of popupMenu (pulldown or pullright)
						Dim p As java.awt.Point = customMenuLocation
						If p Is Nothing Then p = popupMenuOrigin
						popupMenu.show(Me, p.x, p.y)
					Else
						popupMenu.visible = False
					End If
				End If
			End Set
		End Property


		''' <summary>
		''' Computes the origin for the <code>JMenu</code>'s popup menu.
		''' This method uses Look and Feel properties named
		''' <code>Menu.menuPopupOffsetX</code>,
		''' <code>Menu.menuPopupOffsetY</code>,
		''' <code>Menu.submenuPopupOffsetX</code>, and
		''' <code>Menu.submenuPopupOffsetY</code>
		''' to adjust the exact location of popup.
		''' </summary>
		''' <returns> a <code>Point</code> in the coordinate space of the
		'''          menu which should be used as the origin
		'''          of the <code>JMenu</code>'s popup menu
		''' 
		''' @since 1.3 </returns>
		Protected Friend Overridable Property popupMenuOrigin As java.awt.Point
			Get
				Dim ___x As Integer
				Dim ___y As Integer
				Dim pm As JPopupMenu = popupMenu
				' Figure out the sizes needed to caclulate the menu position
				Dim s As java.awt.Dimension = size
				Dim pmSize As java.awt.Dimension = pm.size
				' For the first time the menu is popped up,
				' the size has not yet been initiated
				If pmSize.width=0 Then pmSize = pm.preferredSize
				Dim position As java.awt.Point = locationOnScreen
				Dim toolkit As java.awt.Toolkit = java.awt.Toolkit.defaultToolkit
				Dim gc As java.awt.GraphicsConfiguration = graphicsConfiguration
				Dim screenBounds As New java.awt.Rectangle(toolkit.screenSize)
				Dim ge As java.awt.GraphicsEnvironment = java.awt.GraphicsEnvironment.localGraphicsEnvironment
				Dim gd As java.awt.GraphicsDevice() = ge.screenDevices
				For i As Integer = 0 To gd.Length - 1
					If gd(i).type = java.awt.GraphicsDevice.TYPE_RASTER_SCREEN Then
						Dim dgc As java.awt.GraphicsConfiguration = gd(i).defaultConfiguration
						If dgc.bounds.contains(position) Then
							gc = dgc
							Exit For
						End If
					End If
				Next i
    
    
				If gc IsNot Nothing Then
					screenBounds = gc.bounds
					' take screen insets (e.g. taskbar) into account
					Dim screenInsets As java.awt.Insets = toolkit.getScreenInsets(gc)
    
					screenBounds.width -= Math.Abs(screenInsets.left + screenInsets.right)
					screenBounds.height -= Math.Abs(screenInsets.top + screenInsets.bottom)
					position.x -= Math.Abs(screenInsets.left)
					position.y -= Math.Abs(screenInsets.top)
				End If
    
				Dim parent As java.awt.Container = parent
				If TypeOf parent Is JPopupMenu Then
					' We are a submenu (pull-right)
					Dim xOffset As Integer = UIManager.getInt("Menu.submenuPopupOffsetX")
					Dim yOffset As Integer = UIManager.getInt("Menu.submenuPopupOffsetY")
    
					If SwingUtilities.isLeftToRight(Me) Then
						' First determine x:
						___x = s.width + xOffset ' Prefer placement to the right
						If position.x + ___x + pmSize.width >= screenBounds.width + screenBounds.x AndAlso screenBounds.width - s.width < 2*(position.x - screenBounds.x) Then ___x = 0 - xOffset - pmSize.width
					Else
						' First determine x:
						___x = 0 - xOffset - pmSize.width ' Prefer placement to the left
						If position.x + ___x < screenBounds.x AndAlso screenBounds.width - s.width > 2*(position.x - screenBounds.x) Then ___x = s.width + xOffset
					End If
					' Then the y:
					___y = yOffset ' Prefer dropping down
					If position.y + ___y + pmSize.height >= screenBounds.height + screenBounds.y AndAlso screenBounds.height - s.height < 2*(position.y - screenBounds.y) Then ___y = s.height - yOffset - pmSize.height
				Else
					' We are a toplevel menu (pull-down)
					Dim xOffset As Integer = UIManager.getInt("Menu.menuPopupOffsetX")
					Dim yOffset As Integer = UIManager.getInt("Menu.menuPopupOffsetY")
    
					If SwingUtilities.isLeftToRight(Me) Then
						' First determine the x:
						___x = xOffset ' Extend to the right
						If position.x + ___x + pmSize.width >= screenBounds.width + screenBounds.x AndAlso screenBounds.width - s.width < 2*(position.x - screenBounds.x) Then ___x = s.width - xOffset - pmSize.width
					Else
						' First determine the x:
						___x = s.width - xOffset - pmSize.width ' Extend to the left
						If position.x + ___x < screenBounds.x AndAlso screenBounds.width - s.width > 2*(position.x - screenBounds.x) Then ___x = xOffset
					End If
					' Then the y:
					___y = s.height + yOffset ' Prefer dropping down
					If position.y + ___y + pmSize.height >= screenBounds.height + screenBounds.y AndAlso screenBounds.height - s.height < 2*(position.y - screenBounds.y) Then ___y = 0 - yOffset - pmSize.height ' Otherwise drop 'up'
				End If
				Return New java.awt.Point(___x,___y)
			End Get
		End Property


		''' <summary>
		''' Returns the suggested delay, in milliseconds, before submenus
		''' are popped up or down.
		''' Each look and feel (L&amp;F) may determine its own policy for
		''' observing the <code>delay</code> property.
		''' In most cases, the delay is not observed for top level menus
		''' or while dragging.  The default for <code>delay</code> is 0.
		''' This method is a property of the look and feel code and is used
		''' to manage the idiosyncrasies of the various UI implementations.
		''' 
		''' </summary>
		''' <returns> the <code>delay</code> property </returns>
		Public Overridable Property delay As Integer
			Get
				Return delay
			End Get
			Set(ByVal d As Integer)
				If d < 0 Then Throw New System.ArgumentException("Delay must be a positive integer")
    
				delay = d
			End Set
		End Property


		''' <summary>
		''' The window-closing listener for the popup.
		''' </summary>
		''' <seealso cref= WinListener </seealso>
		Protected Friend popupListener As WinListener

		Private Sub ensurePopupMenuCreated()
			If popupMenu Is Nothing Then
				Dim thisMenu As JMenu = Me
				Me.popupMenu = New JPopupMenu
				popupMenu.invoker = Me
				popupListener = createWinListener(popupMenu)
			End If
		End Sub

	'    
	'     * Return the customized location of the popup component.
	'     
		Private Property customMenuLocation As java.awt.Point
			Get
				Return customMenuLocation
			End Get
		End Property

		''' <summary>
		''' Sets the location of the popup component.
		''' </summary>
		''' <param name="x"> the x coordinate of the popup's new position </param>
		''' <param name="y"> the y coordinate of the popup's new position </param>
		Public Overridable Sub setMenuLocation(ByVal x As Integer, ByVal y As Integer)
			customMenuLocation = New java.awt.Point(x, y)
			If popupMenu IsNot Nothing Then popupMenu.locationion(x, y)
		End Sub

		''' <summary>
		''' Appends a menu item to the end of this menu.
		''' Returns the menu item added.
		''' </summary>
		''' <param name="menuItem"> the <code>JMenuitem</code> to be added </param>
		''' <returns> the <code>JMenuItem</code> added </returns>
		Public Overridable Function add(ByVal menuItem As JMenuItem) As JMenuItem
			ensurePopupMenuCreated()
			Return popupMenu.add(menuItem)
		End Function

		''' <summary>
		''' Appends a component to the end of this menu.
		''' Returns the component added.
		''' </summary>
		''' <param name="c"> the <code>Component</code> to add </param>
		''' <returns> the <code>Component</code> added </returns>
		Public Overridable Function add(ByVal c As java.awt.Component) As java.awt.Component
			ensurePopupMenuCreated()
			popupMenu.add(c)
			Return c
		End Function

		''' <summary>
		''' Adds the specified component to this container at the given
		''' position. If <code>index</code> equals -1, the component will
		''' be appended to the end. </summary>
		''' <param name="c">   the <code>Component</code> to add </param>
		''' <param name="index">    the position at which to insert the component </param>
		''' <returns>    the <code>Component</code> added </returns>
		''' <seealso cref=       #remove </seealso>
		''' <seealso cref= java.awt.Container#add(Component, int) </seealso>
		Public Overridable Function add(ByVal c As java.awt.Component, ByVal index As Integer) As java.awt.Component
			ensurePopupMenuCreated()
			popupMenu.add(c, index)
			Return c
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
		''' Creates a new menu item attached to the specified
		''' <code>Action</code> object and appends it to the end of this menu.
		''' </summary>
		''' <param name="a"> the <code>Action</code> for the menu item to be added </param>
		''' <seealso cref= Action </seealso>
		Public Overridable Function add(ByVal a As Action) As JMenuItem
			Dim mi As JMenuItem = createActionComponent(a)
			mi.action = a
			add(mi)
			Return mi
		End Function

		''' <summary>
		''' Factory method which creates the <code>JMenuItem</code> for
		''' <code>Action</code>s added to the <code>JMenu</code>.
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
		''' Appends a new separator to the end of the menu.
		''' </summary>
		Public Overridable Sub addSeparator()
			ensurePopupMenuCreated()
			popupMenu.addSeparator()
		End Sub

		''' <summary>
		''' Inserts a new menu item with the specified text at a
		''' given position.
		''' </summary>
		''' <param name="s"> the text for the menu item to add </param>
		''' <param name="pos"> an integer specifying the position at which to add the
		'''               new menu item </param>
		''' <exception cref="IllegalArgumentException"> when the value of
		'''                  <code>pos</code> &lt; 0 </exception>
		Public Overridable Sub insert(ByVal s As String, ByVal pos As Integer)
			If pos < 0 Then Throw New System.ArgumentException("index less than zero.")

			ensurePopupMenuCreated()
			popupMenu.insert(New JMenuItem(s), pos)
		End Sub

		''' <summary>
		''' Inserts the specified <code>JMenuitem</code> at a given position.
		''' </summary>
		''' <param name="mi"> the <code>JMenuitem</code> to add </param>
		''' <param name="pos"> an integer specifying the position at which to add the
		'''               new <code>JMenuitem</code> </param>
		''' <returns> the new menu item </returns>
		''' <exception cref="IllegalArgumentException"> if the value of
		'''                  <code>pos</code> &lt; 0 </exception>
		Public Overridable Function insert(ByVal mi As JMenuItem, ByVal pos As Integer) As JMenuItem
			If pos < 0 Then Throw New System.ArgumentException("index less than zero.")
			ensurePopupMenuCreated()
			popupMenu.insert(mi, pos)
			Return mi
		End Function

		''' <summary>
		''' Inserts a new menu item attached to the specified <code>Action</code>
		''' object at a given position.
		''' </summary>
		''' <param name="a"> the <code>Action</code> object for the menu item to add </param>
		''' <param name="pos"> an integer specifying the position at which to add the
		'''               new menu item </param>
		''' <exception cref="IllegalArgumentException"> if the value of
		'''                  <code>pos</code> &lt; 0 </exception>
		Public Overridable Function insert(ByVal a As Action, ByVal pos As Integer) As JMenuItem
			If pos < 0 Then Throw New System.ArgumentException("index less than zero.")

			ensurePopupMenuCreated()
			Dim mi As New JMenuItem(a)
			mi.horizontalTextPosition = JButton.TRAILING
			mi.verticalTextPosition = JButton.CENTER
			popupMenu.insert(mi, pos)
			Return mi
		End Function

		''' <summary>
		''' Inserts a separator at the specified position.
		''' </summary>
		''' <param name="index"> an integer specifying the position at which to
		'''                    insert the menu separator </param>
		''' <exception cref="IllegalArgumentException"> if the value of
		'''                       <code>index</code> &lt; 0 </exception>
		Public Overridable Sub insertSeparator(ByVal index As Integer)
			If index < 0 Then Throw New System.ArgumentException("index less than zero.")

			ensurePopupMenuCreated()
			popupMenu.insert(New JPopupMenu.Separator, index)
		End Sub

		''' <summary>
		''' Returns the <code>JMenuItem</code> at the specified position.
		''' If the component at <code>pos</code> is not a menu item,
		''' <code>null</code> is returned.
		''' This method is included for AWT compatibility.
		''' </summary>
		''' <param name="pos">    an integer specifying the position </param>
		''' <exception cref="IllegalArgumentException"> if the value of
		'''                       <code>pos</code> &lt; 0 </exception>
		''' <returns>  the menu item at the specified position; or <code>null</code>
		'''          if the item as the specified position is not a menu item </returns>
		Public Overridable Function getItem(ByVal pos As Integer) As JMenuItem
			If pos < 0 Then Throw New System.ArgumentException("index less than zero.")

			Dim c As java.awt.Component = getMenuComponent(pos)
			If TypeOf c Is JMenuItem Then
				Dim mi As JMenuItem = CType(c, JMenuItem)
				Return mi
			End If

			' 4173633
			Return Nothing
		End Function

		''' <summary>
		''' Returns the number of items on the menu, including separators.
		''' This method is included for AWT compatibility.
		''' </summary>
		''' <returns> an integer equal to the number of items on the menu </returns>
		''' <seealso cref= #getMenuComponentCount </seealso>
		Public Overridable Property itemCount As Integer
			Get
				Return menuComponentCount
			End Get
		End Property

		''' <summary>
		''' Returns true if the menu can be torn off.  This method is not
		''' yet implemented.
		''' </summary>
		''' <returns> true if the menu can be torn off, else false </returns>
		''' <exception cref="Error">  if invoked -- this method is not yet implemented </exception>
		Public Overridable Property tearOff As Boolean
			Get
				Throw New Exception("boolean isTearOff() {} not yet implemented")
			End Get
		End Property

		''' <summary>
		''' Removes the specified menu item from this menu.  If there is no
		''' popup menu, this method will have no effect.
		''' </summary>
		''' <param name="item"> the <code>JMenuItem</code> to be removed from the menu </param>
		Public Overridable Sub remove(ByVal item As JMenuItem)
			If popupMenu IsNot Nothing Then popupMenu.remove(item)
		End Sub

		''' <summary>
		''' Removes the menu item at the specified index from this menu.
		''' </summary>
		''' <param name="pos"> the position of the item to be removed </param>
		''' <exception cref="IllegalArgumentException"> if the value of
		'''                       <code>pos</code> &lt; 0, or if <code>pos</code>
		'''                       is greater than the number of menu items </exception>
		Public Overridable Sub remove(ByVal pos As Integer)
			If pos < 0 Then Throw New System.ArgumentException("index less than zero.")
			If pos > itemCount Then Throw New System.ArgumentException("index greater than the number of items.")
			If popupMenu IsNot Nothing Then popupMenu.remove(pos)
		End Sub

		''' <summary>
		''' Removes the component <code>c</code> from this menu.
		''' </summary>
		''' <param name="c"> the component to be removed </param>
		Public Overridable Sub remove(ByVal c As java.awt.Component)
			If popupMenu IsNot Nothing Then popupMenu.remove(c)
		End Sub

		''' <summary>
		''' Removes all menu items from this menu.
		''' </summary>
		Public Overridable Sub removeAll()
			If popupMenu IsNot Nothing Then popupMenu.removeAll()
		End Sub

		''' <summary>
		''' Returns the number of components on the menu.
		''' </summary>
		''' <returns> an integer containing the number of components on the menu </returns>
		Public Overridable Property menuComponentCount As Integer
			Get
				Dim componentCount As Integer = 0
				If popupMenu IsNot Nothing Then componentCount = popupMenu.componentCount
				Return componentCount
			End Get
		End Property

		''' <summary>
		''' Returns the component at position <code>n</code>.
		''' </summary>
		''' <param name="n"> the position of the component to be returned </param>
		''' <returns> the component requested, or <code>null</code>
		'''                  if there is no popup menu
		'''  </returns>
		Public Overridable Function getMenuComponent(ByVal n As Integer) As java.awt.Component
			If popupMenu IsNot Nothing Then Return popupMenu.getComponent(n)

			Return Nothing
		End Function

		''' <summary>
		''' Returns an array of <code>Component</code>s of the menu's
		''' subcomponents.  Note that this returns all <code>Component</code>s
		''' in the popup menu, including separators.
		''' </summary>
		''' <returns> an array of <code>Component</code>s or an empty array
		'''          if there is no popup menu </returns>
		Public Overridable Property menuComponents As java.awt.Component()
			Get
				If popupMenu IsNot Nothing Then Return popupMenu.components
    
				Return New java.awt.Component(){}
			End Get
		End Property

		''' <summary>
		''' Returns true if the menu is a 'top-level menu', that is, if it is
		''' the direct child of a menubar.
		''' </summary>
		''' <returns> true if the menu is activated from the menu bar;
		'''         false if the menu is activated from a menu item
		'''         on another menu </returns>
		Public Overridable Property topLevelMenu As Boolean
			Get
				Return TypeOf parent Is JMenuBar
    
			End Get
		End Property

		''' <summary>
		''' Returns true if the specified component exists in the
		''' submenu hierarchy.
		''' </summary>
		''' <param name="c"> the <code>Component</code> to be tested </param>
		''' <returns> true if the <code>Component</code> exists, false otherwise </returns>
		Public Overridable Function isMenuComponent(ByVal c As java.awt.Component) As Boolean
			' Are we in the MenuItem part of the menu
			If c Is Me Then Return True
			' Are we in the PopupMenu?
			If TypeOf c Is JPopupMenu Then
				Dim comp As JPopupMenu = CType(c, JPopupMenu)
				If comp Is Me.popupMenu Then Return True
			End If
			' Are we in a Component on the PopupMenu
			Dim ncomponents As Integer = Me.menuComponentCount
			Dim ___component As java.awt.Component() = Me.menuComponents
			For i As Integer = 0 To ncomponents - 1
				Dim comp As java.awt.Component = ___component(i)
				' Are we in the current component?
				If comp Is c Then Return True
				' Hmmm, what about Non-menu containers?

				' Recursive call for the Menu case
				If TypeOf comp Is JMenu Then
					Dim subMenu As JMenu = CType(comp, JMenu)
					If subMenu.isMenuComponent(c) Then Return True
				End If
			Next i
			Return False
		End Function


	'    
	'     * Returns a point in the coordinate space of this menu's popupmenu
	'     * which corresponds to the point <code>p</code> in the menu's
	'     * coordinate space.
	'     *
	'     * @param p the point to be translated
	'     * @return the point in the coordinate space of this menu's popupmenu
	'     
		Private Function translateToPopupMenu(ByVal p As java.awt.Point) As java.awt.Point
			Return translateToPopupMenu(p.x, p.y)
		End Function

	'    
	'     * Returns a point in the coordinate space of this menu's popupmenu
	'     * which corresponds to the point (x,y) in the menu's coordinate space.
	'     *
	'     * @param x the x coordinate of the point to be translated
	'     * @param y the y coordinate of the point to be translated
	'     * @return the point in the coordinate space of this menu's popupmenu
	'     
		Private Function translateToPopupMenu(ByVal x As Integer, ByVal y As Integer) As java.awt.Point
				Dim newX As Integer
				Dim newY As Integer

				If TypeOf parent Is JPopupMenu Then
					newX = x - size.width
					newY = y
				Else
					newX = x
					newY = y - size.height
				End If

				Return New java.awt.Point(newX, newY)
		End Function

		''' <summary>
		''' Returns the popupmenu associated with this menu.  If there is
		''' no popupmenu, it will create one.
		''' </summary>
		Public Overridable Property popupMenu As JPopupMenu
			Get
				ensurePopupMenuCreated()
				Return popupMenu
			End Get
		End Property

		''' <summary>
		''' Adds a listener for menu events.
		''' </summary>
		''' <param name="l"> the listener to be added </param>
		Public Overridable Sub addMenuListener(ByVal l As MenuListener)
			listenerList.add(GetType(MenuListener), l)
		End Sub

		''' <summary>
		''' Removes a listener for menu events.
		''' </summary>
		''' <param name="l"> the listener to be removed </param>
		Public Overridable Sub removeMenuListener(ByVal l As MenuListener)
			listenerList.remove(GetType(MenuListener), l)
		End Sub

		''' <summary>
		''' Returns an array of all the <code>MenuListener</code>s added
		''' to this JMenu with addMenuListener().
		''' </summary>
		''' <returns> all of the <code>MenuListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		Public Overridable Property menuListeners As MenuListener()
			Get
				Return listenerList.getListeners(GetType(MenuListener))
			End Get
		End Property

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is created lazily.
		''' </summary>
		''' <exception cref="Error">  if there is a <code>null</code> listener </exception>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireMenuSelected()
			If DEBUG Then Console.WriteLine("In JMenu.fireMenuSelected")
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(MenuListener) Then
					If ___listeners(i+1) Is Nothing Then
						Throw New Exception(text & " has a NULL Listener!! " & i)
					Else
						' Lazily create the event:
						If menuEvent Is Nothing Then menuEvent = New MenuEvent(Me)
						CType(___listeners(i+1), MenuListener).menuSelected(menuEvent)
					End If
				End If
			Next i
		End Sub

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is created lazily.
		''' </summary>
		''' <exception cref="Error"> if there is a <code>null</code> listener </exception>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireMenuDeselected()
			If DEBUG Then Console.WriteLine("In JMenu.fireMenuDeselected")
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(MenuListener) Then
					If ___listeners(i+1) Is Nothing Then
						Throw New Exception(text & " has a NULL Listener!! " & i)
					Else
						' Lazily create the event:
						If menuEvent Is Nothing Then menuEvent = New MenuEvent(Me)
						CType(___listeners(i+1), MenuListener).menuDeselected(menuEvent)
					End If
				End If
			Next i
		End Sub

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is created lazily.
		''' </summary>
		''' <exception cref="Error"> if there is a <code>null</code> listener </exception>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireMenuCanceled()
			If DEBUG Then Console.WriteLine("In JMenu.fireMenuCanceled")
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(MenuListener) Then
					If ___listeners(i+1) Is Nothing Then
						Throw New Exception(text & " has a NULL Listener!! " & i)
					Else
						' Lazily create the event:
						If menuEvent Is Nothing Then menuEvent = New MenuEvent(Me)
						CType(___listeners(i+1), MenuListener).menuCanceled(menuEvent)
					End If
				End If
			Next i
		End Sub

		' Overriden to do nothing, JMenu doesn't support an accelerator
		Friend Overrides Sub configureAcceleratorFromAction(ByVal a As Action)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		<Serializable> _
		Friend Class MenuChangeListener
			Implements ChangeListener

			Private ReadOnly outerInstance As JMenu

			Public Sub New(ByVal outerInstance As JMenu)
				Me.outerInstance = outerInstance
			End Sub

			Friend isSelected As Boolean = False
			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				Dim model As ButtonModel = CType(e.source, ButtonModel)
				Dim modelSelected As Boolean = model.selected

				If modelSelected <> isSelected Then
					If modelSelected = True Then
						outerInstance.fireMenuSelected()
					Else
						outerInstance.fireMenuDeselected()
					End If
					isSelected = modelSelected
				End If
			End Sub
		End Class

		Private Function createMenuChangeListener() As ChangeListener
			Return New MenuChangeListener(Me)
		End Function


		''' <summary>
		''' Creates a window-closing listener for the popup.
		''' </summary>
		''' <param name="p"> the <code>JPopupMenu</code> </param>
		''' <returns> the new window-closing listener
		''' </returns>
		''' <seealso cref= WinListener </seealso>
		Protected Friend Overridable Function createWinListener(ByVal p As JPopupMenu) As WinListener
			Return New WinListener(Me, p)
		End Function

		''' <summary>
		''' A listener class that watches for a popup window closing.
		''' When the popup is closing, the listener deselects the menu.
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		<Serializable> _
		Protected Friend Class WinListener
			Inherits WindowAdapter

			Private ReadOnly outerInstance As JMenu

			Friend popupMenu As JPopupMenu
			''' <summary>
			'''  Create the window listener for the specified popup.
			''' @since 1.4
			''' </summary>
			Public Sub New(ByVal outerInstance As JMenu, ByVal p As JPopupMenu)
					Me.outerInstance = outerInstance
				Me.popupMenu = p
			End Sub
			''' <summary>
			''' Deselect the menu when the popup is closed from outside.
			''' </summary>
			Public Overridable Sub windowClosing(ByVal e As WindowEvent)
				outerInstance.selected = False
			End Sub
		End Class

		''' <summary>
		''' Messaged when the menubar selection changes to activate or
		''' deactivate this menu.
		''' Overrides <code>JMenuItem.menuSelectionChanged</code>.
		''' </summary>
		''' <param name="isIncluded">  true if this menu is active, false if
		'''        it is not </param>
		Public Overrides Sub menuSelectionChanged(ByVal isIncluded As Boolean) Implements MenuElement.menuSelectionChanged
			If DEBUG Then Console.WriteLine("In JMenu.menuSelectionChanged to " & isIncluded)
			selected = isIncluded
		End Sub

		''' <summary>
		''' Returns an array of <code>MenuElement</code>s containing the submenu
		''' for this menu component.  If popup menu is <code>null</code> returns
		''' an empty array.  This method is required to conform to the
		''' <code>MenuElement</code> interface.  Note that since
		''' <code>JSeparator</code>s do not conform to the <code>MenuElement</code>
		''' interface, this array will only contain <code>JMenuItem</code>s.
		''' </summary>
		''' <returns> an array of <code>MenuElement</code> objects </returns>
		Public Property Overrides subElements As MenuElement() Implements MenuElement.getSubElements
			Get
				If popupMenu Is Nothing Then
					Return New MenuElement(){}
				Else
					Dim result As MenuElement() = New MenuElement(0){}
					result(0) = popupMenu
					Return result
				End If
			End Get
		End Property


		' implements javax.swing.MenuElement
		''' <summary>
		''' Returns the <code>java.awt.Component</code> used to
		''' paint this <code>MenuElement</code>.
		''' The returned component is used to convert events and detect if
		''' an event is inside a menu component.
		''' </summary>
		Public Property Overrides component As java.awt.Component
			Get
				Return Me
			End Get
		End Property


		''' <summary>
		''' Sets the <code>ComponentOrientation</code> property of this menu
		''' and all components contained within it. This includes all
		''' components returned by <seealso cref="#getMenuComponents getMenuComponents"/>.
		''' </summary>
		''' <param name="o"> the new component orientation of this menu and
		'''        the components contained within it. </param>
		''' <exception cref="NullPointerException"> if <code>orientation</code> is null. </exception>
		''' <seealso cref= java.awt.Component#setComponentOrientation </seealso>
		''' <seealso cref= java.awt.Component#getComponentOrientation
		''' @since 1.4 </seealso>
		Public Overridable Sub applyComponentOrientation(ByVal o As java.awt.ComponentOrientation)
			MyBase.applyComponentOrientation(o)

			If popupMenu IsNot Nothing Then
				Dim ncomponents As Integer = menuComponentCount
				For i As Integer = 0 To ncomponents - 1
					getMenuComponent(i).applyComponentOrientation(o)
				Next i
				popupMenu.componentOrientation = o
			End If
		End Sub

		Public Overridable Property componentOrientation As java.awt.ComponentOrientation
			Set(ByVal o As java.awt.ComponentOrientation)
				MyBase.componentOrientation = o
				If popupMenu IsNot Nothing Then popupMenu.componentOrientation = o
			End Set
		End Property

		''' <summary>
		''' <code>setAccelerator</code> is not defined for <code>JMenu</code>.
		''' Use <code>setMnemonic</code> instead. </summary>
		''' <param name="keyStroke">  the keystroke combination which will invoke
		'''                  the <code>JMenuItem</code>'s actionlisteners
		'''                  without navigating the menu hierarchy </param>
		''' <exception cref="Error">  if invoked -- this method is not defined for JMenu.
		'''                  Use <code>setMnemonic</code> instead
		''' 
		''' @beaninfo
		'''     description: The keystroke combination which will invoke the JMenuItem's
		'''                  actionlisteners without navigating the menu hierarchy
		'''          hidden: true </exception>
		Public Overrides Property accelerator As KeyStroke
			Set(ByVal ___keyStroke As KeyStroke)
				Throw New Exception("setAccelerator() is not defined for JMenu.  Use setMnemonic() instead.")
			End Set
		End Property

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
		''' Programmatically performs a "click".  This overrides the method
		''' <code>AbstractButton.doClick</code> in order to make the menu pop up. </summary>
		''' <param name="pressTime">  indicates the number of milliseconds the
		'''          button was pressed for </param>
		Public Overrides Sub doClick(ByVal pressTime As Integer)
			Dim [me] As MenuElement() = buildMenuElementArray(Me)
			MenuSelectionManager.defaultManager().selectedPath = [me]
		End Sub

	'    
	'     * Build an array of menu elements - from <code>PopupMenu</code> to
	'     * the root <code>JMenuBar</code>.
	'     * @param  leaf  the leaf node from which to start building up the array
	'     * @return the array of menu items
	'     
		Private Function buildMenuElementArray(ByVal leaf As JMenu) As MenuElement()
			Dim elements As New List(Of MenuElement)
			Dim current As java.awt.Component = leaf.popupMenu
			Dim pop As JPopupMenu
			Dim menu As JMenu
			Dim bar As JMenuBar

			Do
				If TypeOf current Is JPopupMenu Then
					pop = CType(current, JPopupMenu)
					elements.Insert(0, pop)
					current = pop.invoker
				ElseIf TypeOf current Is JMenu Then
					menu = CType(current, JMenu)
					elements.Insert(0, menu)
					current = menu.parent
				ElseIf TypeOf current Is JMenuBar Then
					bar = CType(current, JMenuBar)
					elements.Insert(0, bar)
					Dim [me] As MenuElement() = New MenuElement(elements.Count - 1){}
					elements.CopyTo([me])
					Return [me]
				End If
			Loop
		End Function


		''' <summary>
		''' See <code>readObject</code> and <code>writeObject</code> in
		''' <code>JComponent</code> for more
		''' information about serialization in Swing.
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()
			If uIClassID.Equals(uiClassID) Then
				Dim count As SByte = JComponent.getWriteObjCounter(Me)
				count -= 1
				JComponent.writeObjCounterter(Me, count)
				If count = 0 AndAlso ui IsNot Nothing Then ui.installUI(Me)
			End If
		End Sub


		''' <summary>
		''' Returns a string representation of this <code>JMenu</code>. This
		''' method is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this JMenu. </returns>
		Protected Friend Overrides Function paramString() As String
			Return MyBase.paramString()
		End Function


	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this JMenu.
		''' For JMenus, the AccessibleContext takes the form of an
		''' AccessibleJMenu.
		''' A new AccessibleJMenu instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJMenu that serves as the
		'''         AccessibleContext of this JMenu </returns>
		Public Property Overrides accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJMenu(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JMenu</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to menu user-interface elements.
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend Class AccessibleJMenu
			Inherits AccessibleJMenuItem
			Implements AccessibleSelection

			Private ReadOnly outerInstance As JMenu

			Public Sub New(ByVal outerInstance As JMenu)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Returns the number of accessible children in the object.  If all
			''' of the children of this object implement Accessible, than this
			''' method should return the number of children of this object.
			''' </summary>
			''' <returns> the number of accessible children in the object. </returns>
			Public Overridable Property accessibleChildrenCount As Integer
				Get
					Dim children As java.awt.Component() = outerInstance.menuComponents
					Dim count As Integer = 0
					For Each child As java.awt.Component In children
						If TypeOf child Is Accessible Then count += 1
					Next child
					Return count
				End Get
			End Property

			''' <summary>
			''' Returns the nth Accessible child of the object.
			''' </summary>
			''' <param name="i"> zero-based index of child </param>
			''' <returns> the nth Accessible child of the object </returns>
			Public Overridable Function getAccessibleChild(ByVal i As Integer) As Accessible
				Dim children As java.awt.Component() = outerInstance.menuComponents
				Dim count As Integer = 0
				For Each child As java.awt.Component In children
					If TypeOf child Is Accessible Then
						If count = i Then
							If TypeOf child Is JComponent Then
								' FIXME:  [[[WDW - probably should set this when
								' the component is added to the menu.  I tried
								' to do this in most cases, but the separators
								' added by addSeparator are hard to get to.]]]
								Dim ac As AccessibleContext = child.accessibleContext
								ac.accessibleParent = JMenu.this
							End If
							Return CType(child, Accessible)
						Else
							count += 1
						End If
					End If
				Next child
				Return Nothing
			End Function

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.MENU
				End Get
			End Property

			''' <summary>
			''' Get the AccessibleSelection associated with this object.  In the
			''' implementation of the Java Accessibility API for this class,
			''' return this object, which is responsible for implementing the
			''' AccessibleSelection interface on behalf of itself.
			''' </summary>
			''' <returns> this object </returns>
			Public Overridable Property accessibleSelection As AccessibleSelection
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Returns 1 if a sub-menu is currently selected in this menu.
			''' </summary>
			''' <returns> 1 if a menu is currently selected, else 0 </returns>
			Public Overridable Property accessibleSelectionCount As Integer Implements AccessibleSelection.getAccessibleSelectionCount
				Get
					Dim [me] As MenuElement() = MenuSelectionManager.defaultManager().selectedPath
					If [me] IsNot Nothing Then
						For i As Integer = 0 To [me].Length - 1
							If [me](i) Is JMenu.this Then ' this menu is selected
								If i+1 < [me].Length Then Return 1
							End If
						Next i
					End If
					Return 0
				End Get
			End Property

			''' <summary>
			''' Returns the currently selected sub-menu if one is selected,
			''' otherwise null (there can only be one selection, and it can
			''' only be a sub-menu, as otherwise menu items don't remain
			''' selected).
			''' </summary>
			Public Overridable Function getAccessibleSelection(ByVal i As Integer) As Accessible Implements AccessibleSelection.getAccessibleSelection
				' if i is a sub-menu & popped, return it
				If i < 0 OrElse i >= outerInstance.itemCount Then Return Nothing
				Dim [me] As MenuElement() = MenuSelectionManager.defaultManager().selectedPath
				If [me] IsNot Nothing Then
					For j As Integer = 0 To [me].Length - 1
						If [me](j) Is JMenu.this Then ' this menu is selected
							' so find the next JMenuItem in the MenuElement
							' array, and return it!
							j += 1
							Do While j < [me].Length
								If TypeOf [me](j) Is JMenuItem Then Return CType([me](j), Accessible)
								j += 1
							Loop
						End If
					Next j
				End If
				Return Nothing
			End Function

			''' <summary>
			''' Returns true if the current child of this object is selected
			''' (that is, if this child is a popped-up submenu).
			''' </summary>
			''' <param name="i"> the zero-based index of the child in this Accessible
			''' object. </param>
			''' <seealso cref= AccessibleContext#getAccessibleChild </seealso>
			Public Overridable Function isAccessibleChildSelected(ByVal i As Integer) As Boolean Implements AccessibleSelection.isAccessibleChildSelected
				' if i is a sub-menu and is pop-ed up, return true, else false
				Dim [me] As MenuElement() = MenuSelectionManager.defaultManager().selectedPath
				If [me] IsNot Nothing Then
					Dim mi As JMenuItem = outerInstance.getItem(i)
					For j As Integer = 0 To [me].Length - 1
						If [me](j) Is mi Then Return True
					Next j
				End If
				Return False
			End Function


			''' <summary>
			''' Selects the <code>i</code>th menu in the menu.
			''' If that item is a submenu,
			''' it will pop up in response.  If a different item is already
			''' popped up, this will force it to close.  If this is a sub-menu
			''' that is already popped up (selected), this method has no
			''' effect.
			''' </summary>
			''' <param name="i"> the index of the item to be selected </param>
			''' <seealso cref= #getAccessibleStateSet </seealso>
			Public Overridable Sub addAccessibleSelection(ByVal i As Integer) Implements AccessibleSelection.addAccessibleSelection
				If i < 0 OrElse i >= outerInstance.itemCount Then Return
				Dim mi As JMenuItem = outerInstance.getItem(i)
				If mi IsNot Nothing Then
					If TypeOf mi Is JMenu Then
						Dim [me] As MenuElement() = outerInstance.buildMenuElementArray(CType(mi, JMenu))
						MenuSelectionManager.defaultManager().selectedPath = [me]
					Else
						MenuSelectionManager.defaultManager().selectedPath = Nothing
					End If
				End If
			End Sub

			''' <summary>
			''' Removes the nth item from the selection.  In general, menus
			''' can only have one item within them selected at a time
			''' (e.g. one sub-menu popped open).
			''' </summary>
			''' <param name="i"> the zero-based index of the selected item </param>
			Public Overridable Sub removeAccessibleSelection(ByVal i As Integer) Implements AccessibleSelection.removeAccessibleSelection
				If i < 0 OrElse i >= outerInstance.itemCount Then Return
				Dim mi As JMenuItem = outerInstance.getItem(i)
				If mi IsNot Nothing AndAlso TypeOf mi Is JMenu Then
					If mi.selected Then
						Dim old As MenuElement() = MenuSelectionManager.defaultManager().selectedPath
						Dim [me] As MenuElement() = New MenuElement(old.Length-2 - 1){}
						For j As Integer = 0 To old.Length -2 - 1
							[me](j) = old(j)
						Next j
						MenuSelectionManager.defaultManager().selectedPath = [me]
					End If
				End If
			End Sub

			''' <summary>
			''' Clears the selection in the object, so that nothing in the
			''' object is selected.  This will close any open sub-menu.
			''' </summary>
			Public Overridable Sub clearAccessibleSelection() Implements AccessibleSelection.clearAccessibleSelection
				' if this menu is selected, reset selection to only go
				' to this menu; else do nothing
				Dim old As MenuElement() = MenuSelectionManager.defaultManager().selectedPath
				If old IsNot Nothing Then
					For j As Integer = 0 To old.Length - 1
						If old(j) Is JMenu.this Then ' menu is in the selection!
							Dim [me] As MenuElement() = New MenuElement(j){}
							Array.Copy(old, 0, [me], 0, j)
							[me](j) = outerInstance.popupMenu
							MenuSelectionManager.defaultManager().selectedPath = [me]
						End If
					Next j
				End If
			End Sub

			''' <summary>
			''' Normally causes every selected item in the object to be selected
			''' if the object supports multiple selections.  This method
			''' makes no sense in a menu bar, and so does nothing.
			''' </summary>
			Public Overridable Sub selectAllAccessibleSelection() Implements AccessibleSelection.selectAllAccessibleSelection
			End Sub
		End Class ' inner class AccessibleJMenu

	End Class

End Namespace
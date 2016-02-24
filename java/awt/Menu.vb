Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports javax.accessibility

'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.awt


	''' <summary>
	''' A <code>Menu</code> object is a pull-down menu component
	''' that is deployed from a menu bar.
	''' <p>
	''' A menu can optionally be a <i>tear-off</i> menu. A tear-off menu
	''' can be opened and dragged away from its parent menu bar or menu.
	''' It remains on the screen after the mouse button has been released.
	''' The mechanism for tearing off a menu is platform dependent, since
	''' the look and feel of the tear-off menu is determined by its peer.
	''' On platforms that do not support tear-off menus, the tear-off
	''' property is ignored.
	''' <p>
	''' Each item in a menu must belong to the <code>MenuItem</code>
	''' class. It can be an instance of <code>MenuItem</code>, a submenu
	''' (an instance of <code>Menu</code>), or a check box (an instance of
	''' <code>CheckboxMenuItem</code>).
	''' 
	''' @author Sami Shaio </summary>
	''' <seealso cref=     java.awt.MenuItem </seealso>
	''' <seealso cref=     java.awt.CheckboxMenuItem
	''' @since   JDK1.0 </seealso>
	Public Class Menu
		Inherits MenuItem
		Implements MenuContainer, Accessible

		Shared Sub New()
			' ensure that the necessary native libraries are loaded 
			Toolkit.loadLibraries()
			If Not GraphicsEnvironment.headless Then initIDs()

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.awt.AWTAccessor.setMenuAccessor(New sun.awt.AWTAccessor.MenuAccessor()
	'		{
	'				public Vector<MenuComponent> getItems(Menu menu)
	'				{
	'					Return menu.items;
	'				}
	'			});
		End Sub

		''' <summary>
		''' A vector of the items that will be part of the Menu.
		''' 
		''' @serial </summary>
		''' <seealso cref= #countItems() </seealso>
		Friend items As New List(Of MenuComponent)

		''' <summary>
		''' This field indicates whether the menu has the
		''' tear of property or not.  It will be set to
		''' <code>true</code> if the menu has the tear off
		''' property and it will be set to <code>false</code>
		''' if it does not.
		''' A torn off menu can be deleted by a user when
		''' it is no longer needed.
		''' 
		''' @serial </summary>
		''' <seealso cref= #isTearOff() </seealso>
		Friend tearOff As Boolean

		''' <summary>
		''' This field will be set to <code>true</code>
		''' if the Menu in question is actually a help
		''' menu.  Otherwise it will be set to <code>
		''' false</code>.
		''' 
		''' @serial
		''' </summary>
		Friend isHelpMenu As Boolean

		Private Const base As String = "menu"
		Private Shared nameCounter As Integer = 0

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		 Private Const serialVersionUID As Long = -8809584163345499784L

		''' <summary>
		''' Constructs a new menu with an empty label. This menu is not
		''' a tear-off menu. </summary>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' @since      JDK1.1 </seealso>
		Public Sub New()
			Me.New("", False)
		End Sub

		''' <summary>
		''' Constructs a new menu with the specified label. This menu is not
		''' a tear-off menu. </summary>
		''' <param name="label"> the menu's label in the menu bar, or in
		'''                   another menu of which this menu is a submenu. </param>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New(ByVal label_Renamed As String)
			Me.New(label_Renamed, False)
		End Sub

		''' <summary>
		''' Constructs a new menu with the specified label,
		''' indicating whether the menu can be torn off.
		''' <p>
		''' Tear-off functionality may not be supported by all
		''' implementations of AWT.  If a particular implementation doesn't
		''' support tear-off menus, this value is silently ignored. </summary>
		''' <param name="label"> the menu's label in the menu bar, or in
		'''                   another menu of which this menu is a submenu. </param>
		''' <param name="tearOff">   if <code>true</code>, the menu
		'''                   is a tear-off menu. </param>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' @since       JDK1.0. </seealso>
		Public Sub New(ByVal label_Renamed As String, ByVal tearOff As Boolean)
			MyBase.New(label_Renamed)
			Me.tearOff = tearOff
		End Sub

		''' <summary>
		''' Construct a name for this MenuComponent.  Called by getName() when
		''' the name is null.
		''' </summary>
		Friend Overrides Function constructComponentName() As String
			SyncLock GetType(Menu)
					Dim tempVar As Integer = nameCounter
					nameCounter += 1
					Return base + tempVar
			End SyncLock
		End Function

		''' <summary>
		''' Creates the menu's peer.  The peer allows us to modify the
		''' appearance of the menu without changing its functionality.
		''' </summary>
		Public Overrides Sub addNotify()
			SyncLock treeLock
				If peer Is Nothing Then peer = Toolkit.defaultToolkit.createMenu(Me)
				Dim nitems As Integer = itemCount
				For i As Integer = 0 To nitems - 1
					Dim mi As MenuItem = getItem(i)
					mi.parent = Me
					mi.addNotify()
				Next i
			End SyncLock
		End Sub

		''' <summary>
		''' Removes the menu's peer.  The peer allows us to modify the appearance
		''' of the menu without changing its functionality.
		''' </summary>
		Public Overrides Sub removeNotify()
			SyncLock treeLock
				Dim nitems As Integer = itemCount
				For i As Integer = 0 To nitems - 1
					getItem(i).removeNotify()
				Next i
				MyBase.removeNotify()
			End SyncLock
		End Sub

		''' <summary>
		''' Indicates whether this menu is a tear-off menu.
		''' <p>
		''' Tear-off functionality may not be supported by all
		''' implementations of AWT.  If a particular implementation doesn't
		''' support tear-off menus, this value is silently ignored. </summary>
		''' <returns>      <code>true</code> if this is a tear-off menu;
		'''                         <code>false</code> otherwise. </returns>
		Public Overridable Property tearOff As Boolean
			Get
				Return tearOff
			End Get
		End Property

		''' <summary>
		''' Get the number of items in this menu. </summary>
		''' <returns>     the number of items in this menu.
		''' @since      JDK1.1 </returns>
		Public Overridable Property itemCount As Integer
			Get
				Return countItems()
			End Get
		End Property

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>getItemCount()</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Function countItems() As Integer
			Return countItemsImpl()
		End Function

	'    
	'     * This is called by the native code, so client code can't
	'     * be called on the toolkit thread.
	'     
		Friend Function countItemsImpl() As Integer
			Return items.Count
		End Function

		''' <summary>
		''' Gets the item located at the specified index of this menu. </summary>
		''' <param name="index"> the position of the item to be returned. </param>
		''' <returns>    the item located at the specified index. </returns>
		Public Overridable Function getItem(ByVal index As Integer) As MenuItem
			Return getItemImpl(index)
		End Function

	'    
	'     * This is called by the native code, so client code can't
	'     * be called on the toolkit thread.
	'     
		Friend Function getItemImpl(ByVal index As Integer) As MenuItem
			Return CType(items(index), MenuItem)
		End Function

		''' <summary>
		''' Adds the specified menu item to this menu. If the
		''' menu item has been part of another menu, removes it
		''' from that menu.
		''' </summary>
		''' <param name="mi">   the menu item to be added </param>
		''' <returns>      the menu item added </returns>
		''' <seealso cref=         java.awt.Menu#insert(java.lang.String, int) </seealso>
		''' <seealso cref=         java.awt.Menu#insert(java.awt.MenuItem, int) </seealso>
		Public Overridable Function add(ByVal mi As MenuItem) As MenuItem
			SyncLock treeLock
				If mi.parent IsNot Nothing Then mi.parent.remove(mi)
				items.Add(mi)
				mi.parent = Me
				Dim peer_Renamed As java.awt.peer.MenuPeer = CType(Me.peer, java.awt.peer.MenuPeer)
				If peer_Renamed IsNot Nothing Then
					mi.addNotify()
					peer_Renamed.addItem(mi)
				End If
				Return mi
			End SyncLock
		End Function

		''' <summary>
		''' Adds an item with the specified label to this menu.
		''' </summary>
		''' <param name="label">   the text on the item </param>
		''' <seealso cref=         java.awt.Menu#insert(java.lang.String, int) </seealso>
		''' <seealso cref=         java.awt.Menu#insert(java.awt.MenuItem, int) </seealso>
		Public Overridable Sub add(ByVal label_Renamed As String)
			add(New MenuItem(label_Renamed))
		End Sub

		''' <summary>
		''' Inserts a menu item into this menu
		''' at the specified position.
		''' </summary>
		''' <param name="menuitem">  the menu item to be inserted. </param>
		''' <param name="index">     the position at which the menu
		'''                          item should be inserted. </param>
		''' <seealso cref=           java.awt.Menu#add(java.lang.String) </seealso>
		''' <seealso cref=           java.awt.Menu#add(java.awt.MenuItem) </seealso>
		''' <exception cref="IllegalArgumentException"> if the value of
		'''                    <code>index</code> is less than zero
		''' @since         JDK1.1 </exception>

		Public Overridable Sub insert(ByVal menuitem As MenuItem, ByVal index As Integer)
			SyncLock treeLock
				If index < 0 Then Throw New IllegalArgumentException("index less than zero.")

				Dim nitems As Integer = itemCount
				Dim tempItems As New List(Of MenuItem)

	'             Remove the item at index, nitems-index times
	'               storing them in a temporary vector in the
	'               order they appear on the menu.
	'            
				For i As Integer = index To nitems - 1
					tempItems.Add(getItem(index))
					remove(index)
				Next i

				add(menuitem)

	'             Add the removed items back to the menu, they are
	'               already in the correct order in the temp vector.
	'            
				For i As Integer = 0 To tempItems.Count - 1
					add(tempItems(i))
				Next i
			End SyncLock
		End Sub

		''' <summary>
		''' Inserts a menu item with the specified label into this menu
		''' at the specified position.  This is a convenience method for
		''' <code>insert(menuItem, index)</code>.
		''' </summary>
		''' <param name="label"> the text on the item </param>
		''' <param name="index"> the position at which the menu item
		'''                      should be inserted </param>
		''' <seealso cref=         java.awt.Menu#add(java.lang.String) </seealso>
		''' <seealso cref=         java.awt.Menu#add(java.awt.MenuItem) </seealso>
		''' <exception cref="IllegalArgumentException"> if the value of
		'''                    <code>index</code> is less than zero
		''' @since       JDK1.1 </exception>

		Public Overridable Sub insert(ByVal label_Renamed As String, ByVal index As Integer)
			insert(New MenuItem(label_Renamed), index)
		End Sub

		''' <summary>
		''' Adds a separator line, or a hypen, to the menu at the current position. </summary>
		''' <seealso cref=         java.awt.Menu#insertSeparator(int) </seealso>
		Public Overridable Sub addSeparator()
			add("-")
		End Sub

		''' <summary>
		''' Inserts a separator at the specified position. </summary>
		''' <param name="index"> the position at which the
		'''                       menu separator should be inserted. </param>
		''' <exception cref="IllegalArgumentException"> if the value of
		'''                       <code>index</code> is less than 0. </exception>
		''' <seealso cref=         java.awt.Menu#addSeparator
		''' @since       JDK1.1 </seealso>

		Public Overridable Sub insertSeparator(ByVal index As Integer)
			SyncLock treeLock
				If index < 0 Then Throw New IllegalArgumentException("index less than zero.")

				Dim nitems As Integer = itemCount
				Dim tempItems As New List(Of MenuItem)

	'             Remove the item at index, nitems-index times
	'               storing them in a temporary vector in the
	'               order they appear on the menu.
	'            
				For i As Integer = index To nitems - 1
					tempItems.Add(getItem(index))
					remove(index)
				Next i

				addSeparator()

	'             Add the removed items back to the menu, they are
	'               already in the correct order in the temp vector.
	'            
				For i As Integer = 0 To tempItems.Count - 1
					add(tempItems(i))
				Next i
			End SyncLock
		End Sub

		''' <summary>
		''' Removes the menu item at the specified index from this menu. </summary>
		''' <param name="index"> the position of the item to be removed. </param>
		Public Overridable Sub remove(ByVal index As Integer)
			SyncLock treeLock
				Dim mi As MenuItem = getItem(index)
				items.RemoveAt(index)
				Dim peer_Renamed As java.awt.peer.MenuPeer = CType(Me.peer, java.awt.peer.MenuPeer)
				If peer_Renamed IsNot Nothing Then
					mi.removeNotify()
					mi.parent = Nothing
					peer_Renamed.delItem(index)
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' Removes the specified menu item from this menu. </summary>
		''' <param name="item"> the item to be removed from the menu.
		'''         If <code>item</code> is <code>null</code>
		'''         or is not in this menu, this method does
		'''         nothing. </param>
		Public Overridable Sub remove(ByVal item As MenuComponent) Implements MenuContainer.remove
			SyncLock treeLock
				Dim index As Integer = items.IndexOf(item)
				If index >= 0 Then remove(index)
			End SyncLock
		End Sub

		''' <summary>
		''' Removes all items from this menu.
		''' @since       JDK1.0.
		''' </summary>
		Public Overridable Sub removeAll()
			SyncLock treeLock
				Dim nitems As Integer = itemCount
				For i As Integer = nitems-1 To 0 Step -1
					remove(i)
				Next i
			End SyncLock
		End Sub

	'    
	'     * Post an ActionEvent to the target of the MenuPeer
	'     * associated with the specified keyboard event (on
	'     * keydown).  Returns true if there is an associated
	'     * keyboard event.
	'     
		Friend Overridable Function handleShortcut(ByVal e As java.awt.event.KeyEvent) As Boolean
			Dim nitems As Integer = itemCount
			For i As Integer = 0 To nitems - 1
				Dim mi As MenuItem = getItem(i)
				If mi.handleShortcut(e) Then Return True
			Next i
			Return False
		End Function

		Friend Overrides Function getShortcutMenuItem(ByVal s As MenuShortcut) As MenuItem
			Dim nitems As Integer = itemCount
			For i As Integer = 0 To nitems - 1
				Dim mi As MenuItem = getItem(i).getShortcutMenuItem(s)
				If mi IsNot Nothing Then Return mi
			Next i
			Return Nothing
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Function shortcuts() As System.Collections.IEnumerator(Of MenuShortcut)
			Dim shortcuts_Renamed As New List(Of MenuShortcut)
			Dim nitems As Integer = itemCount
			For i As Integer = 0 To nitems - 1
				Dim mi As MenuItem = getItem(i)
				If TypeOf mi Is Menu Then
					Dim e As System.Collections.IEnumerator(Of MenuShortcut) = CType(mi, Menu).shortcuts()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Do While e.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						shortcuts_Renamed.Add(e.nextElement())
					Loop
				Else
					Dim ms As MenuShortcut = mi.shortcut
					If ms IsNot Nothing Then shortcuts_Renamed.Add(ms)
				End If
			Next i
			Return shortcuts_Renamed.elements()
		End Function

		Friend Overrides Sub deleteShortcut(ByVal s As MenuShortcut)
			Dim nitems As Integer = itemCount
			For i As Integer = 0 To nitems - 1
				getItem(i).deleteShortcut(s)
			Next i
		End Sub


	'     Serialization support.  A MenuContainer is responsible for
	'     * restoring the parent fields of its children.
	'     

		''' <summary>
		''' The menu serialized Data Version.
		''' 
		''' @serial
		''' </summary>
		Private menuSerializedDataVersion As Integer = 1

		''' <summary>
		''' Writes default serializable fields to stream.
		''' </summary>
		''' <param name="s"> the <code>ObjectOutputStream</code> to write </param>
		''' <seealso cref= AWTEventMulticaster#save(ObjectOutputStream, String, EventListener) </seealso>
		''' <seealso cref= #readObject(ObjectInputStream) </seealso>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
		  s.defaultWriteObject()
		End Sub

		''' <summary>
		''' Reads the <code>ObjectInputStream</code>.
		''' Unrecognized keys or values will be ignored.
		''' </summary>
		''' <param name="s"> the <code>ObjectInputStream</code> to read </param>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= #writeObject(ObjectOutputStream) </seealso>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
		  ' HeadlessException will be thrown from MenuComponent's readObject
		  s.defaultReadObject()
		  For i As Integer = 0 To items.Count - 1
			Dim item_Renamed As MenuItem = CType(items(i), MenuItem)
			item_Renamed.parent = Me
		  Next i
		End Sub

		''' <summary>
		''' Returns a string representing the state of this <code>Menu</code>.
		''' This method is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not be
		''' <code>null</code>.
		''' </summary>
		''' <returns> the parameter string of this menu </returns>
		Public Overrides Function paramString() As String
			Dim str As String = ",tearOff=" & tearOff & ",isHelpMenu=" & isHelpMenu
			Return MyBase.paramString() + str
		End Function

		''' <summary>
		''' Initialize JNI field and method IDs
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub


	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this Menu.
		''' For menus, the AccessibleContext takes the form of an
		''' AccessibleAWTMenu.
		''' A new AccessibleAWTMenu instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleAWTMenu that serves as the
		'''         AccessibleContext of this Menu
		''' @since 1.3 </returns>
		Public Property Overrides accessibleContext As AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleAWTMenu(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' Defined in MenuComponent. Overridden here.
		''' </summary>
		Friend Overrides Function getAccessibleChildIndex(ByVal child As MenuComponent) As Integer
			Return items.IndexOf(child)
		End Function

		''' <summary>
		''' Inner class of Menu used to provide default support for
		''' accessibility.  This class is not meant to be used directly by
		''' application developers, but is instead meant only to be
		''' subclassed by menu component developers.
		''' <p>
		''' This class implements accessibility support for the
		''' <code>Menu</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to menu user-interface elements.
		''' @since 1.3
		''' </summary>
		Protected Friend Class AccessibleAWTMenu
			Inherits AccessibleAWTMenuItem

			Private ReadOnly outerInstance As Menu

			Public Sub New(ByVal outerInstance As Menu)
				Me.outerInstance = outerInstance
			End Sub

	'        
	'         * JDK 1.3 serialVersionUID
	'         
			Private Const serialVersionUID As Long = 5228160894980069094L

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.MENU
				End Get
			End Property

		End Class ' class AccessibleAWTMenu

	End Class

End Namespace
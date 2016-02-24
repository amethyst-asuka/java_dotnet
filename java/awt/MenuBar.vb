Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports javax.accessibility

'
' * Copyright (c) 1995, 2015, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>MenuBar</code> class encapsulates the platform's
	''' concept of a menu bar bound to a frame. In order to associate
	''' the menu bar with a <code>Frame</code> object, call the
	''' frame's <code>setMenuBar</code> method.
	''' <p>
	''' <A NAME="mbexample"></A><!-- target for cross references -->
	''' This is what a menu bar might look like:
	''' <p>
	''' <img src="doc-files/MenuBar-1.gif"
	''' alt="Diagram of MenuBar containing 2 menus: Examples and Options.
	''' Examples menu is expanded showing items: Basic, Simple, Check, and More Examples."
	''' style="float:center; margin: 7px 10px;">
	''' <p>
	''' A menu bar handles keyboard shortcuts for menu items, passing them
	''' along to its child menus.
	''' (Keyboard shortcuts, which are optional, provide the user with
	''' an alternative to the mouse for invoking a menu item and the
	''' action that is associated with it.)
	''' Each menu item can maintain an instance of <code>MenuShortcut</code>.
	''' The <code>MenuBar</code> class defines several methods,
	''' <seealso cref="MenuBar#shortcuts"/> and
	''' <seealso cref="MenuBar#getShortcutMenuItem"/>
	''' that retrieve information about the shortcuts a given
	''' menu bar is managing.
	''' 
	''' @author Sami Shaio </summary>
	''' <seealso cref=        java.awt.Frame </seealso>
	''' <seealso cref=        java.awt.Frame#setMenuBar(java.awt.MenuBar) </seealso>
	''' <seealso cref=        java.awt.Menu </seealso>
	''' <seealso cref=        java.awt.MenuItem </seealso>
	''' <seealso cref=        java.awt.MenuShortcut
	''' @since      JDK1.0 </seealso>
	Public Class MenuBar
		Inherits MenuComponent
		Implements MenuContainer, Accessible

		Shared Sub New()
			' ensure that the necessary native libraries are loaded 
			Toolkit.loadLibraries()
			If Not GraphicsEnvironment.headless Then initIDs()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.awt.AWTAccessor.setMenuBarAccessor(New sun.awt.AWTAccessor.MenuBarAccessor()
	'		{
	'				public Menu getHelpMenu(MenuBar menuBar)
	'				{
	'					Return menuBar.helpMenu;
	'				}
	'
	'				public Vector<Menu> getMenus(MenuBar menuBar)
	'				{
	'					Return menuBar.menus;
	'				}
	'			});
		End Sub

		''' <summary>
		''' This field represents a vector of the
		''' actual menus that will be part of the MenuBar.
		''' 
		''' @serial </summary>
		''' <seealso cref= #countMenus() </seealso>
		Friend menus As New List(Of Menu)

		''' <summary>
		''' This menu is a special menu dedicated to
		''' help.  The one thing to note about this menu
		''' is that on some platforms it appears at the
		''' right edge of the menubar.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getHelpMenu() </seealso>
		''' <seealso cref= #setHelpMenu(Menu) </seealso>
		Friend helpMenu As Menu

		Private Const base As String = "menubar"
		Private Shared nameCounter As Integer = 0

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		 Private Const serialVersionUID As Long = -4930327919388951260L

		''' <summary>
		''' Creates a new menu bar. </summary>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New()
		End Sub

		''' <summary>
		''' Construct a name for this MenuComponent.  Called by getName() when
		''' the name is null.
		''' </summary>
		Friend Overrides Function constructComponentName() As String
			SyncLock GetType(MenuBar)
					Dim tempVar As Integer = nameCounter
					nameCounter += 1
					Return base + tempVar
			End SyncLock
		End Function

		''' <summary>
		''' Creates the menu bar's peer.  The peer allows us to change the
		''' appearance of the menu bar without changing any of the menu bar's
		''' functionality.
		''' </summary>
		Public Overridable Sub addNotify()
			SyncLock treeLock
				If peer Is Nothing Then peer = Toolkit.defaultToolkit.createMenuBar(Me)

				Dim nmenus As Integer = menuCount
				For i As Integer = 0 To nmenus - 1
					getMenu(i).addNotify()
				Next i
			End SyncLock
		End Sub

		''' <summary>
		''' Removes the menu bar's peer.  The peer allows us to change the
		''' appearance of the menu bar without changing any of the menu bar's
		''' functionality.
		''' </summary>
		Public Overrides Sub removeNotify()
			SyncLock treeLock
				Dim nmenus As Integer = menuCount
				For i As Integer = 0 To nmenus - 1
					getMenu(i).removeNotify()
				Next i
				MyBase.removeNotify()
			End SyncLock
		End Sub

		''' <summary>
		''' Gets the help menu on the menu bar. </summary>
		''' <returns>    the help menu on this menu bar. </returns>
		Public Overridable Property helpMenu As Menu
			Get
				Return helpMenu
			End Get
			Set(ByVal m As Menu)
				SyncLock treeLock
					If helpMenu Is m Then Return
					If helpMenu IsNot Nothing Then remove(helpMenu)
					helpMenu = m
					If m IsNot Nothing Then
						If m.parent IsNot Me Then add(m)
						m.isHelpMenu = True
						m.parent = Me
						Dim peer_Renamed As java.awt.peer.MenuBarPeer = CType(Me.peer, java.awt.peer.MenuBarPeer)
						If peer_Renamed IsNot Nothing Then
							If m.peer Is Nothing Then m.addNotify()
							peer_Renamed.addHelpMenu(m)
						End If
					End If
				End SyncLock
			End Set
		End Property


		''' <summary>
		''' Adds the specified menu to the menu bar.
		''' If the menu has been part of another menu bar,
		''' removes it from that menu bar.
		''' </summary>
		''' <param name="m">   the menu to be added </param>
		''' <returns>       the menu added </returns>
		''' <seealso cref=          java.awt.MenuBar#remove(int) </seealso>
		''' <seealso cref=          java.awt.MenuBar#remove(java.awt.MenuComponent) </seealso>
		Public Overridable Function add(ByVal m As Menu) As Menu
			SyncLock treeLock
				If m.parent IsNot Nothing Then m.parent.remove(m)
				menus.Add(m)
				m.parent = Me

				Dim peer_Renamed As java.awt.peer.MenuBarPeer = CType(Me.peer, java.awt.peer.MenuBarPeer)
				If peer_Renamed IsNot Nothing Then
					If m.peer Is Nothing Then m.addNotify()
					peer_Renamed.addMenu(m)
				End If
				Return m
			End SyncLock
		End Function

		''' <summary>
		''' Removes the menu located at the specified
		''' index from this menu bar. </summary>
		''' <param name="index">   the position of the menu to be removed. </param>
		''' <seealso cref=          java.awt.MenuBar#add(java.awt.Menu) </seealso>
		Public Overridable Sub remove(ByVal index As Integer)
			SyncLock treeLock
				Dim m As Menu = getMenu(index)
				menus.RemoveAt(index)
				Dim peer_Renamed As java.awt.peer.MenuBarPeer = CType(Me.peer, java.awt.peer.MenuBarPeer)
				If peer_Renamed IsNot Nothing Then
					m.removeNotify()
					m.parent = Nothing
					peer_Renamed.delMenu(index)
				End If
				If helpMenu Is m Then
					helpMenu = Nothing
					m.isHelpMenu = False
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' Removes the specified menu component from this menu bar. </summary>
		''' <param name="m"> the menu component to be removed. </param>
		''' <seealso cref=          java.awt.MenuBar#add(java.awt.Menu) </seealso>
		Public Overridable Sub remove(ByVal m As MenuComponent) Implements MenuContainer.remove
			SyncLock treeLock
				Dim index As Integer = menus.IndexOf(m)
				If index >= 0 Then remove(index)
			End SyncLock
		End Sub

		''' <summary>
		''' Gets the number of menus on the menu bar. </summary>
		''' <returns>     the number of menus on the menu bar.
		''' @since      JDK1.1 </returns>
		Public Overridable Property menuCount As Integer
			Get
				Return countMenus()
			End Get
		End Property

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>getMenuCount()</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Function countMenus() As Integer
			Return menuCountImpl
		End Function

	'    
	'     * This is called by the native code, so client code can't
	'     * be called on the toolkit thread.
	'     
		Friend Property menuCountImpl As Integer
			Get
				Return menus.Count
			End Get
		End Property

		''' <summary>
		''' Gets the specified menu. </summary>
		''' <param name="i"> the index position of the menu to be returned. </param>
		''' <returns>     the menu at the specified index of this menu bar. </returns>
		Public Overridable Function getMenu(ByVal i As Integer) As Menu
			Return getMenuImpl(i)
		End Function

	'    
	'     * This is called by the native code, so client code can't
	'     * be called on the toolkit thread.
	'     
		Friend Function getMenuImpl(ByVal i As Integer) As Menu
			Return menus(i)
		End Function

		''' <summary>
		''' Gets an enumeration of all menu shortcuts this menu bar
		''' is managing. </summary>
		''' <returns>      an enumeration of menu shortcuts that this
		'''                      menu bar is managing. </returns>
		''' <seealso cref=         java.awt.MenuShortcut
		''' @since       JDK1.1 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function shortcuts() As System.Collections.IEnumerator(Of MenuShortcut)
			Dim shortcuts_Renamed As New List(Of MenuShortcut)
			Dim nmenus As Integer = menuCount
			For i As Integer = 0 To nmenus - 1
				Dim e As System.Collections.IEnumerator(Of MenuShortcut) = getMenu(i).shortcuts()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Do While e.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					shortcuts_Renamed.Add(e.nextElement())
				Loop
			Next i
			Return shortcuts_Renamed.elements()
		End Function

		''' <summary>
		''' Gets the instance of <code>MenuItem</code> associated
		''' with the specified <code>MenuShortcut</code> object,
		''' or <code>null</code> if none of the menu items being managed
		''' by this menu bar is associated with the specified menu
		''' shortcut. </summary>
		''' <param name="s"> the specified menu shortcut. </param>
		''' <seealso cref=          java.awt.MenuItem </seealso>
		''' <seealso cref=          java.awt.MenuShortcut
		''' @since        JDK1.1 </seealso>
		 Public Overridable Function getShortcutMenuItem(ByVal s As MenuShortcut) As MenuItem
			Dim nmenus As Integer = menuCount
			For i As Integer = 0 To nmenus - 1
				Dim mi As MenuItem = getMenu(i).getShortcutMenuItem(s)
				If mi IsNot Nothing Then Return mi
			Next i
			Return Nothing ' MenuShortcut wasn't found
		 End Function

	'    
	'     * Post an ACTION_EVENT to the target of the MenuPeer
	'     * associated with the specified keyboard event (on
	'     * keydown).  Returns true if there is an associated
	'     * keyboard event.
	'     
		Friend Overridable Function handleShortcut(ByVal e As java.awt.event.KeyEvent) As Boolean
			' Is it a key event?
			Dim id As Integer = e.iD
			If id <> java.awt.event.KeyEvent.KEY_PRESSED AndAlso id <> java.awt.event.KeyEvent.KEY_RELEASED Then Return False

			' Is the accelerator modifier key pressed?
			Dim accelKey As Integer = Toolkit.defaultToolkit.menuShortcutKeyMask
			If (e.modifiers And accelKey) = 0 Then Return False

			' Pass MenuShortcut on to child menus.
			Dim nmenus As Integer = menuCount
			For i As Integer = 0 To nmenus - 1
				Dim m As Menu = getMenu(i)
				If m.handleShortcut(e) Then Return True
			Next i
			Return False
		End Function

		''' <summary>
		''' Deletes the specified menu shortcut. </summary>
		''' <param name="s"> the menu shortcut to delete.
		''' @since     JDK1.1 </param>
		Public Overridable Sub deleteShortcut(ByVal s As MenuShortcut)
			Dim nmenus As Integer = menuCount
			For i As Integer = 0 To nmenus - 1
				getMenu(i).deleteShortcut(s)
			Next i
		End Sub

	'     Serialization support.  Restore the (transient) parent
	'     * fields of Menubar menus here.
	'     

		''' <summary>
		''' The MenuBar's serialized data version.
		''' 
		''' @serial
		''' </summary>
		Private menuBarSerializedDataVersion As Integer = 1

		''' <summary>
		''' Writes default serializable fields to stream.
		''' </summary>
		''' <param name="s"> the <code>ObjectOutputStream</code> to write </param>
		''' <seealso cref= AWTEventMulticaster#save(ObjectOutputStream, String, EventListener) </seealso>
		''' <seealso cref= #readObject(java.io.ObjectInputStream) </seealso>
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
		''' <seealso cref= #writeObject(java.io.ObjectOutputStream) </seealso>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
		  ' HeadlessException will be thrown from MenuComponent's readObject
		  s.defaultReadObject()
		  For i As Integer = 0 To menus.Count - 1
			Dim m As Menu = menus(i)
			m.parent = Me
		  Next i
		End Sub

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
		''' Gets the AccessibleContext associated with this MenuBar.
		''' For menu bars, the AccessibleContext takes the form of an
		''' AccessibleAWTMenuBar.
		''' A new AccessibleAWTMenuBar instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleAWTMenuBar that serves as the
		'''         AccessibleContext of this MenuBar
		''' @since 1.3 </returns>
		Public Property Overrides accessibleContext As AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleAWTMenuBar(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' Defined in MenuComponent. Overridden here.
		''' </summary>
		Friend Overrides Function getAccessibleChildIndex(ByVal child As MenuComponent) As Integer
			Return menus.IndexOf(child)
		End Function

		''' <summary>
		''' Inner class of MenuBar used to provide default support for
		''' accessibility.  This class is not meant to be used directly by
		''' application developers, but is instead meant only to be
		''' subclassed by menu component developers.
		''' <p>
		''' This class implements accessibility support for the
		''' <code>MenuBar</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to menu bar user-interface elements.
		''' @since 1.3
		''' </summary>
		Protected Friend Class AccessibleAWTMenuBar
			Inherits AccessibleAWTMenuComponent

			Private ReadOnly outerInstance As MenuBar

			Public Sub New(ByVal outerInstance As MenuBar)
				Me.outerInstance = outerInstance
			End Sub

	'        
	'         * JDK 1.3 serialVersionUID
	'         
			Private Const serialVersionUID As Long = -8577604491830083815L

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object
			''' @since 1.4 </returns>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.MENU_BAR
				End Get
			End Property

		End Class ' class AccessibleAWTMenuBar

	End Class

End Namespace
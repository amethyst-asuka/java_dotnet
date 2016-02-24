Imports System
Imports javax.accessibility

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A class that implements a menu which can be dynamically popped up
	''' at a specified position within a component.
	''' <p>
	''' As the inheritance hierarchy implies, a <code>PopupMenu</code>
	'''  can be used anywhere a <code>Menu</code> can be used.
	''' However, if you use a <code>PopupMenu</code> like a <code>Menu</code>
	''' (e.g., you add it to a <code>MenuBar</code>), then you <b>cannot</b>
	''' call <code>show</code> on that <code>PopupMenu</code>.
	''' 
	''' @author      Amy Fowler
	''' </summary>
	Public Class PopupMenu
		Inherits Menu

		Private Const base As String = "popup"
		Friend Shared nameCounter As Integer = 0

		<NonSerialized> _
		Friend isTrayIconPopup As Boolean = False

		Shared Sub New()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.awt.AWTAccessor.setPopupMenuAccessor(New sun.awt.AWTAccessor.PopupMenuAccessor()
	'		{
	'				public boolean isTrayIconPopup(PopupMenu popupMenu)
	'				{
	'					Return popupMenu.isTrayIconPopup;
	'				}
	'			});
		End Sub

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = -4620452533522760060L

		''' <summary>
		''' Creates a new popup menu with an empty name. </summary>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New()
			Me.New("")
		End Sub

		''' <summary>
		''' Creates a new popup menu with the specified name.
		''' </summary>
		''' <param name="label"> a non-<code>null</code> string specifying
		'''                the popup menu's label </param>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New(ByVal label_Renamed As String)
			MyBase.New(label_Renamed)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Property Overrides parent As MenuContainer
			Get
				If isTrayIconPopup Then Return Nothing
				Return MyBase.parent
			End Get
		End Property

		''' <summary>
		''' Constructs a name for this <code>MenuComponent</code>.
		''' Called by <code>getName</code> when the name is <code>null</code>.
		''' </summary>
		Friend Overrides Function constructComponentName() As String
			SyncLock GetType(PopupMenu)
					Dim tempVar As Integer = nameCounter
					nameCounter += 1
					Return base + tempVar
			End SyncLock
		End Function

		''' <summary>
		''' Creates the popup menu's peer.
		''' The peer allows us to change the appearance of the popup menu without
		''' changing any of the popup menu's functionality.
		''' </summary>
		Public Overrides Sub addNotify()
			SyncLock treeLock
				' If our parent is not a Component, then this PopupMenu is
				' really just a plain, old Menu.
				If parent IsNot Nothing AndAlso Not(TypeOf parent Is Component) Then
					MyBase.addNotify()
				Else
					If peer Is Nothing Then peer = Toolkit.defaultToolkit.createPopupMenu(Me)
					Dim nitems As Integer = itemCount
					For i As Integer = 0 To nitems - 1
						Dim mi As MenuItem = getItem(i)
						mi.parent = Me
						mi.addNotify()
					Next i
				End If
			End SyncLock
		End Sub

	   ''' <summary>
	   ''' Shows the popup menu at the x, y position relative to an origin
	   ''' component.
	   ''' The origin component must be contained within the component
	   ''' hierarchy of the popup menu's parent.  Both the origin and the parent
	   ''' must be showing on the screen for this method to be valid.
	   ''' <p>
	   ''' If this <code>PopupMenu</code> is being used as a <code>Menu</code>
	   ''' (i.e., it has a non-<code>Component</code> parent),
	   ''' then you cannot call this method on the <code>PopupMenu</code>.
	   ''' </summary>
	   ''' <param name="origin"> the component which defines the coordinate space </param>
	   ''' <param name="x"> the x coordinate position to popup the menu </param>
	   ''' <param name="y"> the y coordinate position to popup the menu </param>
	   ''' <exception cref="NullPointerException">  if the parent is <code>null</code> </exception>
	   ''' <exception cref="IllegalArgumentException">  if this <code>PopupMenu</code>
	   '''                has a non-<code>Component</code> parent </exception>
	   ''' <exception cref="IllegalArgumentException"> if the origin is not in the
	   '''                parent's hierarchy </exception>
	   ''' <exception cref="RuntimeException"> if the parent is not showing on screen </exception>
		Public Overridable Sub show(ByVal origin As Component, ByVal x As Integer, ByVal y As Integer)
			' Use localParent for thread safety.
			Dim localParent As MenuContainer = parent
			If localParent Is Nothing Then Throw New NullPointerException("parent is null")
			If Not(TypeOf localParent Is Component) Then Throw New IllegalArgumentException("PopupMenus with non-Component parents cannot be shown")
			Dim compParent As Component = CType(localParent, Component)
			'Fixed 6278745: Incorrect exception throwing in PopupMenu.show() method
			'Exception was not thrown if compParent was not equal to origin and
			'was not Container
			If compParent IsNot origin Then
				If TypeOf compParent Is Container Then
					If Not CType(compParent, Container).isAncestorOf(origin) Then Throw New IllegalArgumentException("origin not in parent's hierarchy")
				Else
					Throw New IllegalArgumentException("origin not in parent's hierarchy")
				End If
			End If
			If compParent.peer Is Nothing OrElse (Not compParent.showing) Then Throw New RuntimeException("parent not showing on screen")
			If peer Is Nothing Then addNotify()
			SyncLock treeLock
				If peer IsNot Nothing Then CType(peer, java.awt.peer.PopupMenuPeer).show(New [Event](origin, 0, Event.MOUSE_DOWN, x, y, 0, 0))
			End SyncLock
		End Sub


	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the <code>AccessibleContext</code> associated with this
		''' <code>PopupMenu</code>.
		''' </summary>
		''' <returns> the <code>AccessibleContext</code> of this
		'''                <code>PopupMenu</code>
		''' @since 1.3 </returns>
		Public Property Overrides accessibleContext As AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleAWTPopupMenu(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' Inner class of PopupMenu used to provide default support for
		''' accessibility.  This class is not meant to be used directly by
		''' application developers, but is instead meant only to be
		''' subclassed by menu component developers.
		''' <p>
		''' The class used to obtain the accessible role for this object.
		''' @since 1.3
		''' </summary>
		Protected Friend Class AccessibleAWTPopupMenu
			Inherits AccessibleAWTMenu

			Private ReadOnly outerInstance As PopupMenu

			Public Sub New(ByVal outerInstance As PopupMenu)
				Me.outerInstance = outerInstance
			End Sub

	'        
	'         * JDK 1.3 serialVersionUID
	'         
			Private Const serialVersionUID As Long = -4282044795947239955L

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.POPUP_MENU
				End Get
			End Property

		End Class ' class AccessibleAWTPopupMenu

	End Class

End Namespace
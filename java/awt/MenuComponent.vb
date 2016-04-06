Imports System
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
	''' The abstract class <code>MenuComponent</code> is the superclass
	''' of all menu-related components. In this respect, the class
	''' <code>MenuComponent</code> is analogous to the abstract superclass
	''' <code>Component</code> for AWT components.
	''' <p>
	''' Menu components receive and process AWT events, just as components do,
	''' through the method <code>processEvent</code>.
	''' 
	''' @author      Arthur van Hoff
	''' @since       JDK1.0
	''' </summary>
	<Serializable> _
	Public MustInherit Class MenuComponent

		Shared Sub New()
			' ensure that the necessary native libraries are loaded 
			Toolkit.loadLibraries()
			If Not GraphicsEnvironment.headless Then initIDs()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.awt.AWTAccessor.setMenuComponentAccessor(New sun.awt.AWTAccessor.MenuComponentAccessor()
	'		{
	'				public AppContext getAppContext(MenuComponent menuComp)
	'				{
	'					Return menuComp.appContext;
	'				}
	'				public  Sub  setAppContext(MenuComponent menuComp, AppContext appContext)
	'				{
	'					menuComp.appContext = appContext;
	'				}
	'				public MenuContainer getParent(MenuComponent menuComp)
	'				{
	'					Return menuComp.parent;
	'				}
	'				public Font getFont_NoClientCode(MenuComponent menuComp)
	'				{
	'					Return menuComp.getFont_NoClientCode();
	'				}
	'			});
		End Sub

		<NonSerialized> _
		Friend peer As java.awt.peer.MenuComponentPeer
		<NonSerialized> _
		Friend parent As MenuContainer

		''' <summary>
		''' The <code>AppContext</code> of the <code>MenuComponent</code>.
		''' This is set in the constructor and never changes.
		''' </summary>
		<NonSerialized> _
		Friend appContext As sun.awt.AppContext

		''' <summary>
		''' The menu component's font. This value can be
		''' <code>null</code> at which point a default will be used.
		''' This defaults to <code>null</code>.
		''' 
		''' @serial </summary>
		''' <seealso cref= #setFont(Font) </seealso>
		''' <seealso cref= #getFont() </seealso>
		Friend font_Renamed As Font

		''' <summary>
		''' The menu component's name, which defaults to <code>null</code>.
		''' @serial </summary>
		''' <seealso cref= #getName() </seealso>
		''' <seealso cref= #setName(String) </seealso>
		Private name As String

		''' <summary>
		''' A variable to indicate whether a name is explicitly set.
		''' If <code>true</code> the name will be set explicitly.
		''' This defaults to <code>false</code>.
		''' @serial </summary>
		''' <seealso cref= #setName(String) </seealso>
		Private nameExplicitlySet As Boolean = False

		''' <summary>
		''' Defaults to <code>false</code>.
		''' @serial </summary>
		''' <seealso cref= #dispatchEvent(AWTEvent) </seealso>
		Friend newEventsOnly As Boolean = False

	'    
	'     * The menu's AccessControlContext.
	'     
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private acc As java.security.AccessControlContext = java.security.AccessController.context

	'    
	'     * Returns the acc this menu component was constructed with.
	'     
		Friend Property accessControlContext As java.security.AccessControlContext
			Get
				If acc Is Nothing Then Throw New SecurityException("MenuComponent is missing AccessControlContext")
				Return acc
			End Get
		End Property

	'    
	'     * Internal constants for serialization.
	'     
		Friend Const actionListenerK As String = Component.actionListenerK
		Friend Const itemListenerK As String = Component.itemListenerK

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = -4536902356223894379L


		''' <summary>
		''' Creates a <code>MenuComponent</code>. </summary>
		''' <exception cref="HeadlessException"> if
		'''    <code>GraphicsEnvironment.isHeadless</code>
		'''    returns <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New()
			GraphicsEnvironment.checkHeadless()
			appContext = sun.awt.AppContext.appContext
		End Sub

		''' <summary>
		''' Constructs a name for this <code>MenuComponent</code>.
		''' Called by <code>getName</code> when the name is <code>null</code>. </summary>
		''' <returns> a name for this <code>MenuComponent</code> </returns>
		Friend Overridable Function constructComponentName() As String
			Return Nothing ' For strict compliance with prior platform versions, a MenuComponent
						 ' that doesn't set its name should return null from
						 ' getName()
		End Function

		''' <summary>
		''' Gets the name of the menu component. </summary>
		''' <returns>        the name of the menu component </returns>
		''' <seealso cref=           java.awt.MenuComponent#setName(java.lang.String)
		''' @since         JDK1.1 </seealso>
		Public Overridable Property name As String
			Get
				If name Is Nothing AndAlso (Not nameExplicitlySet) Then
					SyncLock Me
						If name Is Nothing AndAlso (Not nameExplicitlySet) Then name = constructComponentName()
					End SyncLock
				End If
				Return name
			End Get
			Set(  name As String)
				SyncLock Me
					Me.name = name
					nameExplicitlySet = True
				End SyncLock
			End Set
		End Property


		''' <summary>
		''' Returns the parent container for this menu component. </summary>
		''' <returns>    the menu component containing this menu component,
		'''                 or <code>null</code> if this menu component
		'''                 is the outermost component, the menu bar itself </returns>
		Public Overridable Property parent As MenuContainer
			Get
				Return parent_NoClientCode
			End Get
		End Property
		' NOTE: This method may be called by privileged threads.
		'       This functionality is implemented in a package-private method
		'       to insure that it cannot be overridden by client subclasses.
		'       DO NOT INVOKE CLIENT CODE ON THIS THREAD!
		Friend Property parent_NoClientCode As MenuContainer
			Get
				Return parent
			End Get
		End Property

		''' @deprecated As of JDK version 1.1,
		''' programs should not directly manipulate peers. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Property peer As java.awt.peer.MenuComponentPeer
			Get
				Return peer
			End Get
		End Property

		''' <summary>
		''' Gets the font used for this menu component. </summary>
		''' <returns>   the font used in this menu component, if there is one;
		'''                  <code>null</code> otherwise </returns>
		''' <seealso cref=     java.awt.MenuComponent#setFont </seealso>
		Public Overridable Property font As Font
			Get
				Dim font_Renamed As Font = Me.font_Renamed
				If font_Renamed IsNot Nothing Then Return font_Renamed
				Dim parent_Renamed As MenuContainer = Me.parent
				If parent_Renamed IsNot Nothing Then Return parent_Renamed.font
				Return Nothing
			End Get
			Set(  f As Font)
				font_Renamed = f
				'Fixed 6312943: NullPointerException in method MenuComponent.setFont(Font)
				Dim peer_Renamed As java.awt.peer.MenuComponentPeer = Me.peer
				If peer_Renamed IsNot Nothing Then peer_Renamed.font = f
			End Set
		End Property

		' NOTE: This method may be called by privileged threads.
		'       This functionality is implemented in a package-private method
		'       to insure that it cannot be overridden by client subclasses.
		'       DO NOT INVOKE CLIENT CODE ON THIS THREAD!
		Friend Property font_NoClientCode As Font
			Get
				Dim font_Renamed As Font = Me.font_Renamed
				If font_Renamed IsNot Nothing Then Return font_Renamed
    
				' The MenuContainer interface does not have getFont_NoClientCode()
				' and it cannot, because it must be package-private. Because of
				' this, we must manually cast classes that implement
				' MenuContainer.
				Dim parent_Renamed As Object = Me.parent
				If parent_Renamed IsNot Nothing Then
					If TypeOf parent_Renamed Is Component Then
						font_Renamed = CType(parent_Renamed, Component).font_NoClientCode
					ElseIf TypeOf parent_Renamed Is MenuComponent Then
						font_Renamed = CType(parent_Renamed, MenuComponent).font_NoClientCode
					End If
				End If
				Return font_Renamed
			End Get
		End Property



		''' <summary>
		''' Removes the menu component's peer.  The peer allows us to modify the
		''' appearance of the menu component without changing the functionality of
		''' the menu component.
		''' </summary>
		Public Overridable Sub removeNotify()
			SyncLock treeLock
				Dim p As java.awt.peer.MenuComponentPeer = Me.peer
				If p IsNot Nothing Then
					Toolkit.eventQueue.removeSourceEvents(Me, True)
					Me.peer = Nothing
					p.Dispose()
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' Posts the specified event to the menu.
		''' This method is part of the Java&nbsp;1.0 event system
		''' and it is maintained only for backwards compatibility.
		''' Its use is discouraged, and it may not be supported
		''' in the future. </summary>
		''' <param name="evt"> the event which is to take place </param>
		''' @deprecated As of JDK version 1.1, replaced by {@link
		''' #dispatchEvent(AWTEvent) dispatchEvent}. 
		<Obsolete("As of JDK version 1.1, replaced by {@link")> _
		Public Overridable Function postEvent(  evt As [Event]) As Boolean
			Dim parent_Renamed As MenuContainer = Me.parent
			If parent_Renamed IsNot Nothing Then parent_Renamed.postEvent(evt)
			Return False
		End Function

		''' <summary>
		''' Delivers an event to this component or one of its sub components. </summary>
		''' <param name="e"> the event </param>
		Public Sub dispatchEvent(  e As AWTEvent)
			dispatchEventImpl(e)
		End Sub

		Friend Overridable Sub dispatchEventImpl(  e As AWTEvent)
			EventQueue.currentEventAndMostRecentTime = e

			Toolkit.defaultToolkit.notifyAWTEventListeners(e)

			If newEventsOnly OrElse (parent IsNot Nothing AndAlso TypeOf parent Is MenuComponent AndAlso CType(parent, MenuComponent).newEventsOnly) Then
				If eventEnabled(e) Then
					processEvent(e)
				ElseIf TypeOf e Is java.awt.event.ActionEvent AndAlso parent IsNot Nothing Then
					e.source = parent
					CType(parent, MenuComponent).dispatchEvent(e)
				End If
 ' backward compatibility
			Else
				Dim olde As [Event] = e.convertToOld()
				If olde IsNot Nothing Then postEvent(olde)
			End If
		End Sub

		' REMIND: remove when filtering is done at lower level
		Friend Overridable Function eventEnabled(  e As AWTEvent) As Boolean
			Return False
		End Function
		''' <summary>
		''' Processes events occurring on this menu component.
		''' <p>Note that if the event parameter is <code>null</code>
		''' the behavior is unspecified and may result in an
		''' exception.
		''' </summary>
		''' <param name="e"> the event
		''' @since JDK1.1 </param>
		Protected Friend Overridable Sub processEvent(  e As AWTEvent)
		End Sub

		''' <summary>
		''' Returns a string representing the state of this
		''' <code>MenuComponent</code>. This method is intended to be used
		''' only for debugging purposes, and the content and format of the
		''' returned string may vary between implementations. The returned
		''' string may be empty but may not be <code>null</code>.
		''' </summary>
		''' <returns>     the parameter string of this menu component </returns>
		Protected Friend Overridable Function paramString() As String
			Dim thisName As String = name
			Return (If(thisName IsNot Nothing, thisName, ""))
		End Function

		''' <summary>
		''' Returns a representation of this menu component as a string. </summary>
		''' <returns>  a string representation of this menu component </returns>
		Public Overrides Function ToString() As String
			Return Me.GetType().name & "[" & paramString() & "]"
		End Function

		''' <summary>
		''' Gets this component's locking object (the object that owns the thread
		''' synchronization monitor) for AWT component-tree and layout
		''' operations. </summary>
		''' <returns> this component's locking object </returns>
		Protected Friend Property treeLock As Object
			Get
				Return Component.LOCK
			End Get
		End Property

		''' <summary>
		''' Reads the menu component from an object input stream.
		''' </summary>
		''' <param name="s"> the <code>ObjectInputStream</code> to read </param>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code>
		''' @serial </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Private Sub readObject(  s As java.io.ObjectInputStream)
			GraphicsEnvironment.checkHeadless()

			acc = java.security.AccessController.context

			s.defaultReadObject()

			appContext = sun.awt.AppContext.appContext
		End Sub

		''' <summary>
		''' Initialize JNI field and method IDs.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub


	'    
	'     * --- Accessibility Support ---
	'     *
	'     *  MenuComponent will contain all of the methods in interface Accessible,
	'     *  though it won't actually implement the interface - that will be up
	'     *  to the individual objects which extend MenuComponent.
	'     

		Friend accessibleContext As AccessibleContext = Nothing

		''' <summary>
		''' Gets the <code>AccessibleContext</code> associated with
		''' this <code>MenuComponent</code>.
		''' 
		''' The method implemented by this base class returns <code>null</code>.
		''' Classes that extend <code>MenuComponent</code>
		''' should implement this method to return the
		''' <code>AccessibleContext</code> associated with the subclass.
		''' </summary>
		''' <returns> the <code>AccessibleContext</code> of this
		'''     <code>MenuComponent</code>
		''' @since 1.3 </returns>
		Public Overridable Property accessibleContext As AccessibleContext
			Get
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' Inner class of <code>MenuComponent</code> used to provide
		''' default support for accessibility.  This class is not meant
		''' to be used directly by application developers, but is instead
		''' meant only to be subclassed by menu component developers.
		''' <p>
		''' The class used to obtain the accessible role for this object.
		''' @since 1.3
		''' </summary>
		<Serializable> _
		Protected Friend MustInherit Class AccessibleAWTMenuComponent
			Inherits AccessibleContext
			Implements AccessibleComponent, AccessibleSelection

			Private ReadOnly outerInstance As MenuComponent

	'        
	'         * JDK 1.3 serialVersionUID
	'         
			Private Const serialVersionUID As Long = -4269533416223798698L

			''' <summary>
			''' Although the class is abstract, this should be called by
			''' all sub-classes.
			''' </summary>
			Protected Friend Sub New(  outerInstance As MenuComponent)
					Me.outerInstance = outerInstance
			End Sub

			' AccessibleContext methods
			'

			''' <summary>
			''' Gets the <code>AccessibleSelection</code> associated with this
			''' object which allows its <code>Accessible</code> children to be selected.
			''' </summary>
			''' <returns> <code>AccessibleSelection</code> if supported by object;
			'''      else return <code>null</code> </returns>
			''' <seealso cref= AccessibleSelection </seealso>
			Public Overridable Property accessibleSelection As AccessibleSelection
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Gets the accessible name of this object.  This should almost never
			''' return <code>java.awt.MenuComponent.getName</code>, as that
			''' generally isn't a localized name, and doesn't have meaning for the
			''' user.  If the object is fundamentally a text object (e.g. a menu item), the
			''' accessible name should be the text of the object (e.g. "save").
			''' If the object has a tooltip, the tooltip text may also be an
			''' appropriate String to return.
			''' </summary>
			''' <returns> the localized name of the object -- can be <code>null</code>
			'''         if this object does not have a name </returns>
			''' <seealso cref= AccessibleContext#setAccessibleName </seealso>
			Public Overridable Property accessibleName As String
				Get
					Return accessibleName
				End Get
			End Property

			''' <summary>
			''' Gets the accessible description of this object.  This should be
			''' a concise, localized description of what this object is - what
			''' is its meaning to the user.  If the object has a tooltip, the
			''' tooltip text may be an appropriate string to return, assuming
			''' it contains a concise description of the object (instead of just
			''' the name of the object - e.g. a "Save" icon on a toolbar that
			''' had "save" as the tooltip text shouldn't return the tooltip
			''' text as the description, but something like "Saves the current
			''' text document" instead).
			''' </summary>
			''' <returns> the localized description of the object -- can be
			'''     <code>null</code> if this object does not have a description </returns>
			''' <seealso cref= AccessibleContext#setAccessibleDescription </seealso>
			Public Overridable Property accessibleDescription As String
				Get
					Return accessibleDescription
				End Get
			End Property

			''' <summary>
			''' Gets the role of this object.
			''' </summary>
			''' <returns> an instance of <code>AccessibleRole</code>
			'''     describing the role of the object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.AWT_COMPONENT ' Non-specific -- overridden in subclasses
				End Get
			End Property

			''' <summary>
			''' Gets the state of this object.
			''' </summary>
			''' <returns> an instance of <code>AccessibleStateSet</code>
			'''     containing the current state set of the object </returns>
			''' <seealso cref= AccessibleState </seealso>
			Public Overridable Property accessibleStateSet As AccessibleStateSet
				Get
					Return outerInstance.accessibleStateSet
				End Get
			End Property

			''' <summary>
			''' Gets the <code>Accessible</code> parent of this object.
			''' If the parent of this object implements <code>Accessible</code>,
			''' this method should simply return <code>getParent</code>.
			''' </summary>
			''' <returns> the <code>Accessible</code> parent of this object -- can
			'''    be <code>null</code> if this object does not have an
			'''    <code>Accessible</code> parent </returns>
			Public Overridable Property accessibleParent As Accessible
				Get
					If accessibleParent IsNot Nothing Then
						Return accessibleParent
					Else
						Dim parent As MenuContainer = outerInstance.parent
						If TypeOf parent Is Accessible Then Return CType(parent, Accessible)
					End If
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Gets the index of this object in its accessible parent.
			''' </summary>
			''' <returns> the index of this object in its parent; -1 if this
			'''     object does not have an accessible parent </returns>
			''' <seealso cref= #getAccessibleParent </seealso>
			Public Overridable Property accessibleIndexInParent As Integer
				Get
					Return outerInstance.accessibleIndexInParent
				End Get
			End Property

			''' <summary>
			''' Returns the number of accessible children in the object.  If all
			''' of the children of this object implement <code>Accessible</code>,
			''' then this method should return the number of children of this object.
			''' </summary>
			''' <returns> the number of accessible children in the object </returns>
			Public Overridable Property accessibleChildrenCount As Integer
				Get
					Return 0 ' MenuComponents don't have children
				End Get
			End Property

			''' <summary>
			''' Returns the nth <code>Accessible</code> child of the object.
			''' </summary>
			''' <param name="i"> zero-based index of child </param>
			''' <returns> the nth Accessible child of the object </returns>
			Public Overridable Function getAccessibleChild(  i As Integer) As Accessible
				Return Nothing ' MenuComponents don't have children
			End Function

			''' <summary>
			''' Returns the locale of this object.
			''' </summary>
			''' <returns> the locale of this object </returns>
			Public Overridable Property locale As java.util.Locale
				Get
					Dim parent As MenuContainer = outerInstance.parent
					If TypeOf parent Is Component Then
						Return CType(parent, Component).locale
					Else
						Return java.util.Locale.default
					End If
				End Get
			End Property

			''' <summary>
			''' Gets the <code>AccessibleComponent</code> associated with
			''' this object if one exists.  Otherwise return <code>null</code>.
			''' </summary>
			''' <returns> the component </returns>
			Public Overridable Property accessibleComponent As AccessibleComponent
				Get
					Return Me
				End Get
			End Property


			' AccessibleComponent methods
			'
			''' <summary>
			''' Gets the background color of this object.
			''' </summary>
			''' <returns> the background color, if supported, of the object;
			'''     otherwise, <code>null</code> </returns>
			Public Overridable Property background As Color
				Get
					Return Nothing ' Not supported for MenuComponents
				End Get
				Set(  c As Color)
					' Not supported for MenuComponents
				End Set
			End Property


			''' <summary>
			''' Gets the foreground color of this object.
			''' </summary>
			''' <returns> the foreground color, if supported, of the object;
			'''     otherwise, <code>null</code> </returns>
			Public Overridable Property foreground As Color
				Get
					Return Nothing ' Not supported for MenuComponents
				End Get
				Set(  c As Color)
					' Not supported for MenuComponents
				End Set
			End Property


			''' <summary>
			''' Gets the <code>Cursor</code> of this object.
			''' </summary>
			''' <returns> the <code>Cursor</code>, if supported, of the object;
			'''     otherwise, <code>null</code> </returns>
			Public Overridable Property cursor As Cursor
				Get
					Return Nothing ' Not supported for MenuComponents
				End Get
				Set(  cursor_Renamed As Cursor)
					' Not supported for MenuComponents
				End Set
			End Property


			''' <summary>
			''' Gets the <code>Font</code> of this object.
			''' </summary>
			''' <returns> the <code>Font</code>,if supported, for the object;
			'''     otherwise, <code>null</code> </returns>
			Public Overridable Property font As Font
				Get
					Return outerInstance.font
				End Get
				Set(  f As Font)
					outerInstance.font = f
				End Set
			End Property


			''' <summary>
			''' Gets the <code>FontMetrics</code> of this object.
			''' </summary>
			''' <param name="f"> the <code>Font</code> </param>
			''' <returns> the FontMetrics, if supported, the object;
			'''              otherwise, <code>null</code> </returns>
			''' <seealso cref= #getFont </seealso>
			Public Overridable Function getFontMetrics(  f As Font) As FontMetrics
				Return Nothing ' Not supported for MenuComponents
			End Function

			''' <summary>
			''' Determines if the object is enabled.
			''' </summary>
			''' <returns> true if object is enabled; otherwise, false </returns>
			Public Overridable Property enabled As Boolean
				Get
					Return True ' Not supported for MenuComponents
				End Get
				Set(  b As Boolean)
					' Not supported for MenuComponents
				End Set
			End Property


			''' <summary>
			''' Determines if the object is visible.  Note: this means that the
			''' object intends to be visible; however, it may not in fact be
			''' showing on the screen because one of the objects that this object
			''' is contained by is not visible.  To determine if an object is
			''' showing on the screen, use <code>isShowing</code>.
			''' </summary>
			''' <returns> true if object is visible; otherwise, false </returns>
			Public Overridable Property visible As Boolean
				Get
					Return True ' Not supported for MenuComponents
				End Get
				Set(  b As Boolean)
					' Not supported for MenuComponents
				End Set
			End Property


			''' <summary>
			''' Determines if the object is showing.  This is determined by checking
			''' the visibility of the object and ancestors of the object.  Note:
			''' this will return true even if the object is obscured by another
			''' (for example, it happens to be underneath a menu that was pulled
			''' down).
			''' </summary>
			''' <returns> true if object is showing; otherwise, false </returns>
			Public Overridable Property showing As Boolean
				Get
					Return True ' Not supported for MenuComponents
				End Get
			End Property

			''' <summary>
			''' Checks whether the specified point is within this object's bounds,
			''' where the point's x and y coordinates are defined to be relative to
			''' the coordinate system of the object.
			''' </summary>
			''' <param name="p"> the <code>Point</code> relative to the coordinate
			'''     system of the object </param>
			''' <returns> true if object contains <code>Point</code>; otherwise false </returns>
			Public Overridable Function contains(  p As Point) As Boolean
				Return False ' Not supported for MenuComponents
			End Function

			''' <summary>
			''' Returns the location of the object on the screen.
			''' </summary>
			''' <returns> location of object on screen -- can be <code>null</code>
			'''     if this object is not on the screen </returns>
			Public Overridable Property locationOnScreen As Point
				Get
					Return Nothing ' Not supported for MenuComponents
				End Get
			End Property

			''' <summary>
			''' Gets the location of the object relative to the parent in the form
			''' of a point specifying the object's top-left corner in the screen's
			''' coordinate space.
			''' </summary>
			''' <returns> an instance of <code>Point</code> representing the
			'''    top-left corner of the object's bounds in the coordinate
			'''    space of the screen; <code>null</code> if
			'''    this object or its parent are not on the screen </returns>
			Public Overridable Property location As Point
				Get
					Return Nothing ' Not supported for MenuComponents
				End Get
				Set(  p As Point)
					' Not supported for MenuComponents
				End Set
			End Property


			''' <summary>
			''' Gets the bounds of this object in the form of a
			''' <code>Rectangle</code> object.
			''' The bounds specify this object's width, height, and location
			''' relative to its parent.
			''' </summary>
			''' <returns> a rectangle indicating this component's bounds;
			'''     <code>null</code> if this object is not on the screen </returns>
			Public Overridable Property bounds As Rectangle
				Get
					Return Nothing ' Not supported for MenuComponents
				End Get
				Set(  r As Rectangle)
					' Not supported for MenuComponents
				End Set
			End Property


			''' <summary>
			''' Returns the size of this object in the form of a
			''' <code>Dimension</code> object. The height field of
			''' the <code>Dimension</code> object contains this object's
			''' height, and the width field of the <code>Dimension</code>
			''' object contains this object's width.
			''' </summary>
			''' <returns> a <code>Dimension</code> object that indicates the
			'''         size of this component; <code>null</code>
			'''         if this object is not on the screen </returns>
			Public Overridable Property size As Dimension
				Get
					Return Nothing ' Not supported for MenuComponents
				End Get
				Set(  d As Dimension)
					' Not supported for MenuComponents
				End Set
			End Property


			''' <summary>
			''' Returns the <code>Accessible</code> child, if one exists,
			''' contained at the local coordinate <code>Point</code>.
			''' If there is no <code>Accessible</code> child, <code>null</code>
			''' is returned.
			''' </summary>
			''' <param name="p"> the point defining the top-left corner of the
			'''    <code>Accessible</code>, given in the coordinate space
			'''    of the object's parent </param>
			''' <returns> the <code>Accessible</code>, if it exists,
			'''    at the specified location; else <code>null</code> </returns>
			Public Overridable Function getAccessibleAt(  p As Point) As Accessible
				Return Nothing ' MenuComponents don't have children
			End Function

			''' <summary>
			''' Returns whether this object can accept focus or not.
			''' </summary>
			''' <returns> true if object can accept focus; otherwise false </returns>
			Public Overridable Property focusTraversable As Boolean
				Get
					Return True ' Not supported for MenuComponents
				End Get
			End Property

			''' <summary>
			''' Requests focus for this object.
			''' </summary>
			Public Overridable Sub requestFocus()
				' Not supported for MenuComponents
			End Sub

			''' <summary>
			''' Adds the specified focus listener to receive focus events from this
			''' component.
			''' </summary>
			''' <param name="l"> the focus listener </param>
			Public Overridable Sub addFocusListener(  l As java.awt.event.FocusListener)
				' Not supported for MenuComponents
			End Sub

			''' <summary>
			''' Removes the specified focus listener so it no longer receives focus
			''' events from this component.
			''' </summary>
			''' <param name="l"> the focus listener </param>
			Public Overridable Sub removeFocusListener(  l As java.awt.event.FocusListener)
				' Not supported for MenuComponents
			End Sub

			' AccessibleSelection methods
			'

			''' <summary>
			''' Returns the number of <code>Accessible</code> children currently selected.
			''' If no children are selected, the return value will be 0.
			''' </summary>
			''' <returns> the number of items currently selected </returns>
			 Public Overridable Property accessibleSelectionCount As Integer
				 Get
					 Return 0 '  To be fully implemented in a future release
				 End Get
			 End Property

			''' <summary>
			''' Returns an <code>Accessible</code> representing the specified
			''' selected child in the object.  If there isn't a selection, or there are
			''' fewer children selected than the integer passed in, the return
			''' value will be <code>null</code>.
			''' <p>Note that the index represents the i-th selected child, which
			''' is different from the i-th child.
			''' </summary>
			''' <param name="i"> the zero-based index of selected children </param>
			''' <returns> the i-th selected child </returns>
			''' <seealso cref= #getAccessibleSelectionCount </seealso>
			 Public Overridable Function getAccessibleSelection(  i As Integer) As Accessible
				 Return Nothing '  To be fully implemented in a future release
			 End Function

			''' <summary>
			''' Determines if the current child of this object is selected.
			''' </summary>
			''' <returns> true if the current child of this object is selected;
			'''    else false </returns>
			''' <param name="i"> the zero-based index of the child in this
			'''      <code>Accessible</code> object </param>
			''' <seealso cref= AccessibleContext#getAccessibleChild </seealso>
			 Public Overridable Function isAccessibleChildSelected(  i As Integer) As Boolean
				 Return False '  To be fully implemented in a future release
			 End Function

			''' <summary>
			''' Adds the specified <code>Accessible</code> child of the object
			''' to the object's selection.  If the object supports multiple selections,
			''' the specified child is added to any existing selection, otherwise
			''' it replaces any existing selection in the object.  If the
			''' specified child is already selected, this method has no effect.
			''' </summary>
			''' <param name="i"> the zero-based index of the child </param>
			''' <seealso cref= AccessibleContext#getAccessibleChild </seealso>
			 Public Overridable Sub addAccessibleSelection(  i As Integer)
				   '  To be fully implemented in a future release
			 End Sub

			''' <summary>
			''' Removes the specified child of the object from the object's
			''' selection.  If the specified item isn't currently selected, this
			''' method has no effect.
			''' </summary>
			''' <param name="i"> the zero-based index of the child </param>
			''' <seealso cref= AccessibleContext#getAccessibleChild </seealso>
			 Public Overridable Sub removeAccessibleSelection(  i As Integer)
				   '  To be fully implemented in a future release
			 End Sub

			''' <summary>
			''' Clears the selection in the object, so that no children in the
			''' object are selected.
			''' </summary>
			 Public Overridable Sub clearAccessibleSelection()
				   '  To be fully implemented in a future release
			 End Sub

			''' <summary>
			''' Causes every child of the object to be selected
			''' if the object supports multiple selections.
			''' </summary>
			 Public Overridable Sub selectAllAccessibleSelection()
				   '  To be fully implemented in a future release
			 End Sub

		End Class ' inner class AccessibleAWTComponent

		''' <summary>
		''' Gets the index of this object in its accessible parent.
		''' </summary>
		''' <returns> -1 if this object does not have an accessible parent;
		'''      otherwise, the index of the child in its accessible parent. </returns>
		Friend Overridable Property accessibleIndexInParent As Integer
			Get
				Dim localParent As MenuContainer = parent
				If Not(TypeOf localParent Is MenuComponent) Then Return -1
				Dim localParentMenu As MenuComponent = CType(localParent, MenuComponent)
				Return localParentMenu.getAccessibleChildIndex(Me)
			End Get
		End Property

		''' <summary>
		''' Gets the index of the child within this MenuComponent.
		''' </summary>
		''' <param name="child"> MenuComponent whose index we are interested in. </param>
		''' <returns> -1 if this object doesn't contain the child,
		'''      otherwise, index of the child. </returns>
		Friend Overridable Function getAccessibleChildIndex(  child As MenuComponent) As Integer
			Return -1 ' Overridden in subclasses.
		End Function

		''' <summary>
		''' Gets the state of this object.
		''' </summary>
		''' <returns> an instance of <code>AccessibleStateSet</code>
		'''     containing the current state set of the object </returns>
		''' <seealso cref= AccessibleState </seealso>
		Friend Overridable Property accessibleStateSet As AccessibleStateSet
			Get
				Dim states As New AccessibleStateSet
				Return states
			End Get
		End Property

	End Class

End Namespace
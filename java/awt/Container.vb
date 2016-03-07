Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Collections.Generic
Imports System.Threading
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
	''' A generic Abstract Window Toolkit(AWT) container object is a component
	''' that can contain other AWT components.
	''' <p>
	''' Components added to a container are tracked in a list.  The order
	''' of the list will define the components' front-to-back stacking order
	''' within the container.  If no index is specified when adding a
	''' component to a container, it will be added to the end of the list
	''' (and hence to the bottom of the stacking order).
	''' <p>
	''' <b>Note</b>: For details on the focus subsystem, see
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/misc/focus.html">
	''' How to Use the Focus Subsystem</a>,
	''' a section in <em>The Java Tutorial</em>, and the
	''' <a href="../../java/awt/doc-files/FocusSpec.html">Focus Specification</a>
	''' for more information.
	''' 
	''' @author      Arthur van Hoff
	''' @author      Sami Shaio </summary>
	''' <seealso cref=       #add(java.awt.Component, int) </seealso>
	''' <seealso cref=       #getComponent(int) </seealso>
	''' <seealso cref=       LayoutManager
	''' @since     JDK1.0 </seealso>
	Public Class Container
		Inherits Component

		Private Shared ReadOnly log As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.awt.Container")
		Private Shared ReadOnly eventLog As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.awt.event.Container")

		Private Shared ReadOnly EMPTY_ARRAY As Component() = New Component(){}

		''' <summary>
		''' The components in this container. </summary>
		''' <seealso cref= #add </seealso>
		''' <seealso cref= #getComponents </seealso>
		Private component_Renamed As IList(Of Component) = New List(Of Component)

		''' <summary>
		''' Layout manager for this container. </summary>
		''' <seealso cref= #doLayout </seealso>
		''' <seealso cref= #setLayout </seealso>
		''' <seealso cref= #getLayout </seealso>
		Friend layoutMgr As LayoutManager

		''' <summary>
		''' Event router for lightweight components.  If this container
		''' is native, this dispatcher takes care of forwarding and
		''' retargeting the events to lightweight components contained
		''' (if any).
		''' </summary>
		Private dispatcher As LightweightDispatcher

		''' <summary>
		''' The focus traversal policy that will manage keyboard traversal of this
		''' Container's children, if this Container is a focus cycle root. If the
		''' value is null, this Container inherits its policy from its focus-cycle-
		''' root ancestor. If all such ancestors of this Container have null
		''' policies, then the current KeyboardFocusManager's default policy is
		''' used. If the value is non-null, this policy will be inherited by all
		''' focus-cycle-root children that have no keyboard-traversal policy of
		''' their own (as will, recursively, their focus-cycle-root children).
		''' <p>
		''' If this Container is not a focus cycle root, the value will be
		''' remembered, but will not be used or inherited by this or any other
		''' Containers until this Container is made a focus cycle root.
		''' </summary>
		''' <seealso cref= #setFocusTraversalPolicy </seealso>
		''' <seealso cref= #getFocusTraversalPolicy
		''' @since 1.4 </seealso>
		<NonSerialized> _
		Private focusTraversalPolicy As FocusTraversalPolicy

		''' <summary>
		''' Indicates whether this Component is the root of a focus traversal cycle.
		''' Once focus enters a traversal cycle, typically it cannot leave it via
		''' focus traversal unless one of the up- or down-cycle keys is pressed.
		''' Normal traversal is limited to this Container, and all of this
		''' Container's descendants that are not descendants of inferior focus cycle
		''' roots.
		''' </summary>
		''' <seealso cref= #setFocusCycleRoot </seealso>
		''' <seealso cref= #isFocusCycleRoot
		''' @since 1.4 </seealso>
		Private focusCycleRoot As Boolean = False


		''' <summary>
		''' Stores the value of focusTraversalPolicyProvider property.
		''' @since 1.5 </summary>
		''' <seealso cref= #setFocusTraversalPolicyProvider </seealso>
		Private focusTraversalPolicyProvider As Boolean

		' keeps track of the threads that are printing this component
		<NonSerialized> _
		Private printingThreads As java.util.Set(Of Thread)
		' True if there is at least one thread that's printing this component
		<NonSerialized> _
		Private printing As Boolean = False

		<NonSerialized> _
		Friend containerListener As ContainerListener

		' HierarchyListener and HierarchyBoundsListener support 
		<NonSerialized> _
		Friend listeningChildren As Integer
		<NonSerialized> _
		Friend listeningBoundsChildren As Integer
		<NonSerialized> _
		Friend descendantsCount As Integer

		' Non-opaque window support -- see Window.setLayersOpaque 
		<NonSerialized> _
		Friend preserveBackgroundColor As Color = Nothing

		''' <summary>
		''' JDK 1.1 serialVersionUID
		''' </summary>
		Private Const serialVersionUID As Long = 4613797578919906343L

		''' <summary>
		''' A constant which toggles one of the controllable behaviors
		''' of <code>getMouseEventTarget</code>. It is used to specify whether
		''' the method can return the Container on which it is originally called
		''' in case if none of its children are the current mouse event targets.
		''' </summary>
		''' <seealso cref= #getMouseEventTarget(int, int, boolean) </seealso>
		Friend Const INCLUDE_SELF As Boolean = True

		''' <summary>
		''' A constant which toggles one of the controllable behaviors
		''' of <code>getMouseEventTarget</code>. It is used to specify whether
		''' the method should search only lightweight components.
		''' </summary>
		''' <seealso cref= #getMouseEventTarget(int, int, boolean) </seealso>
		Friend Const SEARCH_HEAVYWEIGHTS As Boolean = True

	'    
	'     * Number of HW or LW components in this container (including
	'     * all descendant containers).
	'     
		<NonSerialized> _
		Private numOfHWComponents As Integer = 0
		<NonSerialized> _
		Private numOfLWComponents As Integer = 0

		Private Shared ReadOnly mixingLog As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.awt.mixing.Container")

		''' <summary>
		''' @serialField ncomponents                     int
		'''       The number of components in this container.
		'''       This value can be null.
		''' @serialField component                       Component[]
		'''       The components in this container.
		''' @serialField layoutMgr                       LayoutManager
		'''       Layout manager for this container.
		''' @serialField dispatcher                      LightweightDispatcher
		'''       Event router for lightweight components.  If this container
		'''       is native, this dispatcher takes care of forwarding and
		'''       retargeting the events to lightweight components contained
		'''       (if any).
		''' @serialField maxSize                         Dimension
		'''       Maximum size of this Container.
		''' @serialField focusCycleRoot                  boolean
		'''       Indicates whether this Component is the root of a focus traversal cycle.
		'''       Once focus enters a traversal cycle, typically it cannot leave it via
		'''       focus traversal unless one of the up- or down-cycle keys is pressed.
		'''       Normal traversal is limited to this Container, and all of this
		'''       Container's descendants that are not descendants of inferior focus cycle
		'''       roots.
		''' @serialField containerSerializedDataVersion  int
		'''       Container Serial Data Version.
		''' @serialField focusTraversalPolicyProvider    boolean
		'''       Stores the value of focusTraversalPolicyProvider property.
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("ncomponents",  java.lang.[Integer].TYPE), New java.io.ObjectStreamField("component", GetType(Component())), New java.io.ObjectStreamField("layoutMgr", GetType(LayoutManager)), New java.io.ObjectStreamField("dispatcher", GetType(LightweightDispatcher)), New java.io.ObjectStreamField("maxSize", GetType(Dimension)), New java.io.ObjectStreamField("focusCycleRoot",  java.lang.[Boolean].TYPE), New java.io.ObjectStreamField("containerSerializedDataVersion",  java.lang.[Integer].TYPE), New java.io.ObjectStreamField("focusTraversalPolicyProvider",  java.lang.[Boolean].TYPE) }

		Shared Sub New()
			' ensure that the necessary native libraries are loaded 
			Toolkit.loadLibraries()
			If Not GraphicsEnvironment.headless Then initIDs()

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.awt.AWTAccessor.setContainerAccessor(New sun.awt.AWTAccessor.ContainerAccessor()
	'		{
	'			@Override public void validateUnconditionally(Container cont)
	'			{
	'				cont.validateUnconditionally();
	'			}
	'
	'			@Override public Component findComponentAt(Container cont, int x, int y, boolean ignoreEnabled)
	'			{
	'				Return cont.findComponentAt(x, y, ignoreEnabled);
	'			}
	'		});
			' Don't lazy-read because every app uses invalidate()
			isJavaAwtSmartInvalidate = java.security.AccessController.doPrivileged(New sun.security.action.GetBooleanAction("java.awt.smartInvalidate"))
		End Sub

		''' <summary>
		''' Initialize JNI field and method IDs for fields that may be
		'''   called from C.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub

		''' <summary>
		''' Constructs a new Container. Containers can be extended directly,
		''' but are lightweight in this case and must be contained by a parent
		''' somewhere higher up in the component tree that is native.
		''' (such as Frame for example).
		''' </summary>
		Public Sub New()
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Overrides Sub initializeFocusTraversalKeys()
			focusTraversalKeys = New java.util.Set(3){}
		End Sub

		''' <summary>
		''' Gets the number of components in this panel.
		''' <p>
		''' Note: This method should be called under AWT tree lock.
		''' </summary>
		''' <returns>    the number of components in this panel. </returns>
		''' <seealso cref=       #getComponent
		''' @since     JDK1.1 </seealso>
		''' <seealso cref= Component#getTreeLock() </seealso>
		Public Overridable Property componentCount As Integer
			Get
				Return countComponents()
			End Get
		End Property

		''' @deprecated As of JDK version 1.1,
		''' replaced by getComponentCount(). 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Function countComponents() As Integer
			' This method is not synchronized under AWT tree lock.
			' Instead, the calling code is responsible for the
			' synchronization. See 6784816 for details.
			Return component_Renamed.Count
		End Function

		''' <summary>
		''' Gets the nth component in this container.
		''' <p>
		''' Note: This method should be called under AWT tree lock.
		''' </summary>
		''' <param name="n">   the index of the component to get. </param>
		''' <returns>     the n<sup>th</sup> component in this container. </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''                 if the n<sup>th</sup> value does not exist. </exception>
		''' <seealso cref= Component#getTreeLock() </seealso>
		Public Overridable Function getComponent(ByVal n As Integer) As Component
			' This method is not synchronized under AWT tree lock.
			' Instead, the calling code is responsible for the
			' synchronization. See 6784816 for details.
			Try
				Return component_Renamed(n)
			Catch z As IndexOutOfBoundsException
				Throw New ArrayIndexOutOfBoundsException("No such child: " & n)
			End Try
		End Function

		''' <summary>
		''' Gets all the components in this container.
		''' <p>
		''' Note: This method should be called under AWT tree lock.
		''' </summary>
		''' <returns>    an array of all the components in this container. </returns>
		''' <seealso cref= Component#getTreeLock() </seealso>
		Public Overridable Property components As Component()
			Get
				' This method is not synchronized under AWT tree lock.
				' Instead, the calling code is responsible for the
				' synchronization. See 6784816 for details.
				Return components_NoClientCode
			End Get
		End Property

		' NOTE: This method may be called by privileged threads.
		'       This functionality is implemented in a package-private method
		'       to insure that it cannot be overridden by client subclasses.
		'       DO NOT INVOKE CLIENT CODE ON THIS THREAD!
		Friend Property components_NoClientCode As Component()
			Get
				Return component_Renamed.ToArray(EMPTY_ARRAY)
			End Get
		End Property

	'    
	'     * Wrapper for getComponents() method with a proper synchronization.
	'     
		Friend Overridable Property componentsSync As Component()
			Get
				SyncLock treeLock
					Return components
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Determines the insets of this container, which indicate the size
		''' of the container's border.
		''' <p>
		''' A <code>Frame</code> object, for example, has a top inset that
		''' corresponds to the height of the frame's title bar. </summary>
		''' <returns>    the insets of this container. </returns>
		''' <seealso cref=       Insets </seealso>
		''' <seealso cref=       LayoutManager
		''' @since     JDK1.1 </seealso>
		Public Overridable Property insets As Insets
			Get
				Return insets()
			End Get
		End Property

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>getInsets()</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Function insets() As Insets
			Dim peer_Renamed As java.awt.peer.ComponentPeer = Me.peer
			If TypeOf peer_Renamed Is java.awt.peer.ContainerPeer Then
				Dim cpeer As java.awt.peer.ContainerPeer = CType(peer_Renamed, java.awt.peer.ContainerPeer)
				Return CType(cpeer.insets.clone(), Insets)
			End If
			Return New Insets(0, 0, 0, 0)
		End Function

		''' <summary>
		''' Appends the specified component to the end of this container.
		''' This is a convenience method for <seealso cref="#addImpl"/>.
		''' <p>
		''' This method changes layout-related information, and therefore,
		''' invalidates the component hierarchy. If the container has already been
		''' displayed, the hierarchy must be validated thereafter in order to
		''' display the added component.
		''' </summary>
		''' <param name="comp">   the component to be added </param>
		''' <exception cref="NullPointerException"> if {@code comp} is {@code null} </exception>
		''' <seealso cref= #addImpl </seealso>
		''' <seealso cref= #invalidate </seealso>
		''' <seealso cref= #validate </seealso>
		''' <seealso cref= javax.swing.JComponent#revalidate() </seealso>
		''' <returns>    the component argument </returns>
		Public Overridable Function add(ByVal comp As Component) As Component
			addImpl(comp, Nothing, -1)
			Return comp
		End Function

		''' <summary>
		''' Adds the specified component to this container.
		''' This is a convenience method for <seealso cref="#addImpl"/>.
		''' <p>
		''' This method is obsolete as of 1.1.  Please use the
		''' method <code>add(Component, Object)</code> instead.
		''' <p>
		''' This method changes layout-related information, and therefore,
		''' invalidates the component hierarchy. If the container has already been
		''' displayed, the hierarchy must be validated thereafter in order to
		''' display the added component.
		''' </summary>
		''' <exception cref="NullPointerException"> if {@code comp} is {@code null} </exception>
		''' <seealso cref= #add(Component, Object) </seealso>
		''' <seealso cref= #invalidate </seealso>
		Public Overridable Function add(ByVal name As String, ByVal comp As Component) As Component
			addImpl(comp, name, -1)
			Return comp
		End Function

		''' <summary>
		''' Adds the specified component to this container at the given
		''' position.
		''' This is a convenience method for <seealso cref="#addImpl"/>.
		''' <p>
		''' This method changes layout-related information, and therefore,
		''' invalidates the component hierarchy. If the container has already been
		''' displayed, the hierarchy must be validated thereafter in order to
		''' display the added component.
		''' 
		''' </summary>
		''' <param name="comp">   the component to be added </param>
		''' <param name="index">    the position at which to insert the component,
		'''                   or <code>-1</code> to append the component to the end </param>
		''' <exception cref="NullPointerException"> if {@code comp} is {@code null} </exception>
		''' <exception cref="IllegalArgumentException"> if {@code index} is invalid (see
		'''            <seealso cref="#addImpl"/> for details) </exception>
		''' <returns>    the component <code>comp</code> </returns>
		''' <seealso cref= #addImpl </seealso>
		''' <seealso cref= #remove </seealso>
		''' <seealso cref= #invalidate </seealso>
		''' <seealso cref= #validate </seealso>
		''' <seealso cref= javax.swing.JComponent#revalidate() </seealso>
		Public Overridable Function add(ByVal comp As Component, ByVal index As Integer) As Component
			addImpl(comp, Nothing, index)
			Return comp
		End Function

		''' <summary>
		''' Checks that the component
		''' isn't supposed to be added into itself.
		''' </summary>
		Private Sub checkAddToSelf(ByVal comp As Component)
			If TypeOf comp Is Container Then
				Dim cn As Container = Me
				Do While cn IsNot Nothing
					If cn Is comp Then Throw New IllegalArgumentException("adding container's parent to itself")
					cn=cn.parent
				Loop
			End If
		End Sub

		''' <summary>
		''' Checks that the component is not a Window instance.
		''' </summary>
		Private Sub checkNotAWindow(ByVal comp As Component)
			If TypeOf comp Is Window Then Throw New IllegalArgumentException("adding a window to a container")
		End Sub

		''' <summary>
		''' Checks that the component comp can be added to this container
		''' Checks :  index in bounds of container's size,
		''' comp is not one of this container's parents,
		''' and comp is not a window.
		''' Comp and container must be on the same GraphicsDevice.
		''' if comp is container, all sub-components must be on
		''' same GraphicsDevice.
		''' 
		''' @since 1.5
		''' </summary>
		Private Sub checkAdding(ByVal comp As Component, ByVal index As Integer)
			checkTreeLock()

			Dim thisGC As GraphicsConfiguration = graphicsConfiguration

			If index > component_Renamed.Count OrElse index < 0 Then Throw New IllegalArgumentException("illegal component position")
			If comp.parent Is Me Then
				If index = component_Renamed.Count Then Throw New IllegalArgumentException("illegal component position " & index & " should be less then " & component_Renamed.Count)
			End If
			checkAddToSelf(comp)
			checkNotAWindow(comp)

			Dim thisTopLevel As Window = containingWindow
			Dim compTopLevel As Window = comp.containingWindow
			If thisTopLevel IsNot compTopLevel Then Throw New IllegalArgumentException("component and container should be in the same top-level window")
			If thisGC IsNot Nothing Then comp.checkGD(thisGC.device.iDstring)
		End Sub

		''' <summary>
		''' Removes component comp from this container without making unneccessary changes
		''' and generating unneccessary events. This function intended to perform optimized
		''' remove, for example, if newParent and current parent are the same it just changes
		''' index without calling removeNotify.
		''' Note: Should be called while holding treeLock
		''' Returns whether removeNotify was invoked
		''' @since: 1.5
		''' </summary>
		Private Function removeDelicately(ByVal comp As Component, ByVal newParent As Container, ByVal newIndex As Integer) As Boolean
			checkTreeLock()

			Dim index As Integer = getComponentZOrder(comp)
			Dim needRemoveNotify As Boolean = isRemoveNotifyNeeded(comp, Me, newParent)
			If needRemoveNotify Then comp.removeNotify()
			If newParent IsNot Me Then
				If layoutMgr IsNot Nothing Then layoutMgr.removeLayoutComponent(comp)
				adjustListeningChildren(AWTEvent.HIERARCHY_EVENT_MASK, -comp.numListening(AWTEvent.HIERARCHY_EVENT_MASK))
				adjustListeningChildren(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK, -comp.numListening(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK))
				adjustDescendants(-(comp.countHierarchyMembers()))

				comp.parent = Nothing
				If needRemoveNotify Then comp.graphicsConfiguration = Nothing
				component_Renamed.RemoveAt(index)

				invalidateIfValid()
			Else
				' We should remove component and then
				' add it by the newIndex without newIndex decrement if even we shift components to the left
				' after remove. Consult the rules below:
				' 2->4: 012345 -> 013425, 2->5: 012345 -> 013452
				' 4->2: 012345 -> 014235
				component_Renamed.RemoveAt(index)
				component_Renamed.Insert(newIndex, comp)
			End If
			If comp.parent Is Nothing Then ' was actually removed
				If containerListener IsNot Nothing OrElse (eventMask And AWTEvent.CONTAINER_EVENT_MASK) <> 0 OrElse Toolkit.enabledOnToolkit(AWTEvent.CONTAINER_EVENT_MASK) Then
					Dim e As New ContainerEvent(Me, ContainerEvent.COMPONENT_REMOVED, comp)
					dispatchEvent(e)

				End If
				comp.createHierarchyEvents(HierarchyEvent.HIERARCHY_CHANGED, comp, Me, HierarchyEvent.PARENT_CHANGED, Toolkit.enabledOnToolkit(AWTEvent.HIERARCHY_EVENT_MASK))
				If peer IsNot Nothing AndAlso layoutMgr Is Nothing AndAlso visible Then updateCursorImmediately()
			End If
			Return needRemoveNotify
		End Function

		''' <summary>
		''' Checks whether this container can contain component which is focus owner.
		''' Verifies that container is enable and showing, and if it is focus cycle root
		''' its FTP allows component to be focus owner
		''' @since 1.5
		''' </summary>
		Friend Overridable Function canContainFocusOwner(ByVal focusOwnerCandidate As Component) As Boolean
			If Not(enabled AndAlso displayable AndAlso visible AndAlso focusable) Then Return False
			If focusCycleRoot Then
				Dim policy As FocusTraversalPolicy = focusTraversalPolicy
				If TypeOf policy Is DefaultFocusTraversalPolicy Then
					If Not CType(policy, DefaultFocusTraversalPolicy).accept(focusOwnerCandidate) Then Return False
				End If
			End If
			SyncLock treeLock
				If parent IsNot Nothing Then Return parent.canContainFocusOwner(focusOwnerCandidate)
			End SyncLock
			Return True
		End Function

		''' <summary>
		''' Checks whether or not this container has heavyweight children.
		''' Note: Should be called while holding tree lock </summary>
		''' <returns> true if there is at least one heavyweight children in a container, false otherwise
		''' @since 1.5 </returns>
		Friend Function hasHeavyweightDescendants() As Boolean
			checkTreeLock()
			Return numOfHWComponents > 0
		End Function

		''' <summary>
		''' Checks whether or not this container has lightweight children.
		''' Note: Should be called while holding tree lock </summary>
		''' <returns> true if there is at least one lightweight children in a container, false otherwise
		''' @since 1.7 </returns>
		Friend Function hasLightweightDescendants() As Boolean
			checkTreeLock()
			Return numOfLWComponents > 0
		End Function

		''' <summary>
		''' Returns closest heavyweight component to this container. If this container is heavyweight
		''' returns this.
		''' @since 1.5
		''' </summary>
		Friend Overridable Property heavyweightContainer As Container
			Get
				checkTreeLock()
				If peer IsNot Nothing AndAlso Not(TypeOf peer Is java.awt.peer.LightweightPeer) Then
					Return Me
				Else
					Return nativeContainer
				End If
			End Get
		End Property

		''' <summary>
		''' Detects whether or not remove from current parent and adding to new parent requires call of
		''' removeNotify on the component. Since removeNotify destroys native window this might (not)
		''' be required. For example, if new container and old containers are the same we don't need to
		''' destroy native window.
		''' @since: 1.5
		''' </summary>
		Private Shared Function isRemoveNotifyNeeded(ByVal comp As Component, ByVal oldContainer As Container, ByVal newContainer As Container) As Boolean
            If oldContainer Is Nothing Then Return False  ' Component didn't have parent - no removeNotify Return False
            If comp.peer Is Nothing Then Return False ' Component didn't have peer - no removeNotify Return False
            If newContainer.peer Is Nothing Then Return True

			' If component is lightweight non-Container or lightweight Container with all but heavyweight
			' children there is no need to call remove notify
			If comp.lightweight Then
				Dim isContainer As Boolean = TypeOf comp Is Container

				If (Not isContainer) OrElse (isContainer AndAlso (Not CType(comp, Container).hasHeavyweightDescendants())) Then Return False
			End If

			' If this point is reached, then the comp is either a HW or a LW container with HW descendants.

			' All three components have peers, check for peer change
			Dim newNativeContainer As Container = oldContainer.heavyweightContainer
			Dim oldNativeContainer As Container = newContainer.heavyweightContainer
			If newNativeContainer IsNot oldNativeContainer Then
				' Native containers change - check whether or not current platform supports
				' changing of widget hierarchy on native level without recreation.
				' The current implementation forbids reparenting of LW containers with HW descendants
				' into another native container w/o destroying the peers. Actually such an operation
				' is quite rare. If we ever need to save the peers, we'll have to slightly change the
				' addDelicately() method in order to handle such LW containers recursively, reparenting
				' each HW descendant independently.
				Return Not comp.peer.reparentSupported
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Moves the specified component to the specified z-order index in
		''' the container. The z-order determines the order that components
		''' are painted; the component with the highest z-order paints first
		''' and the component with the lowest z-order paints last.
		''' Where components overlap, the component with the lower
		''' z-order paints over the component with the higher z-order.
		''' <p>
		''' If the component is a child of some other container, it is
		''' removed from that container before being added to this container.
		''' The important difference between this method and
		''' <code>java.awt.Container.add(Component, int)</code> is that this method
		''' doesn't call <code>removeNotify</code> on the component while
		''' removing it from its previous container unless necessary and when
		''' allowed by the underlying native windowing system. This way, if the
		''' component has the keyboard focus, it maintains the focus when
		''' moved to the new position.
		''' <p>
		''' This property is guaranteed to apply only to lightweight
		''' non-<code>Container</code> components.
		''' <p>
		''' This method changes layout-related information, and therefore,
		''' invalidates the component hierarchy.
		''' <p>
		''' <b>Note</b>: Not all platforms support changing the z-order of
		''' heavyweight components from one container into another without
		''' the call to <code>removeNotify</code>. There is no way to detect
		''' whether a platform supports this, so developers shouldn't make
		''' any assumptions.
		''' </summary>
		''' <param name="comp"> the component to be moved </param>
		''' <param name="index"> the position in the container's list to
		'''            insert the component, where <code>getComponentCount()</code>
		'''            appends to the end </param>
		''' <exception cref="NullPointerException"> if <code>comp</code> is
		'''            <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>comp</code> is one of the
		'''            container's parents </exception>
		''' <exception cref="IllegalArgumentException"> if <code>index</code> is not in
		'''            the range <code>[0, getComponentCount()]</code> for moving
		'''            between containers, or not in the range
		'''            <code>[0, getComponentCount()-1]</code> for moving inside
		'''            a container </exception>
		''' <exception cref="IllegalArgumentException"> if adding a container to itself </exception>
		''' <exception cref="IllegalArgumentException"> if adding a <code>Window</code>
		'''            to a container </exception>
		''' <seealso cref= #getComponentZOrder(java.awt.Component) </seealso>
		''' <seealso cref= #invalidate
		''' @since 1.5 </seealso>
		Public Overridable Sub setComponentZOrder(ByVal comp As Component, ByVal index As Integer)
			 SyncLock treeLock
				 ' Store parent because remove will clear it
				 Dim curParent As Container = comp.parent
				 Dim oldZindex As Integer = getComponentZOrder(comp)

				 If curParent Is Me AndAlso index = oldZindex Then Return
				 checkAdding(comp, index)

				 Dim peerRecreated As Boolean = If(curParent IsNot Nothing, curParent.removeDelicately(comp, Me, index), False)

				 addDelicately(comp, curParent, index)

				 ' If the oldZindex == -1, the component gets inserted,
				 ' rather than it changes its z-order.
				 If (Not peerRecreated) AndAlso oldZindex <> -1 Then comp.mixOnZOrderChanging(oldZindex, index)
			 End SyncLock
		End Sub

		''' <summary>
		''' Traverses the tree of components and reparents children heavyweight component
		''' to new heavyweight parent.
		''' @since 1.5
		''' </summary>
		Private Sub reparentTraverse(ByVal parentPeer As java.awt.peer.ContainerPeer, ByVal child As Container)
			checkTreeLock()

			For i As Integer = 0 To child.componentCount - 1
				Dim comp As Component = child.getComponent(i)
				If comp.lightweight Then
					' If components is lightweight check if it is container
					' If it is container it might contain heavyweight children we need to reparent
					If TypeOf comp Is Container Then reparentTraverse(parentPeer, CType(comp, Container))
				Else
					' Q: Need to update NativeInLightFixer?
					comp.peer.reparent(parentPeer)
				End If
			Next i
		End Sub

		''' <summary>
		''' Reparents child component peer to this container peer.
		''' Container must be heavyweight.
		''' @since 1.5
		''' </summary>
		Private Sub reparentChild(ByVal comp As Component)
			checkTreeLock()
			If comp Is Nothing Then Return
			If comp.lightweight Then
				' If component is lightweight container we need to reparent all its explicit  heavyweight children
				If TypeOf comp Is Container Then reparentTraverse(CType(peer, java.awt.peer.ContainerPeer), CType(comp, Container))
			Else
				comp.peer.reparent(CType(peer, java.awt.peer.ContainerPeer))
			End If
		End Sub

		''' <summary>
		''' Adds component to this container. Tries to minimize side effects of this adding -
		''' doesn't call remove notify if it is not required.
		''' @since 1.5
		''' </summary>
		Private Sub addDelicately(ByVal comp As Component, ByVal curParent As Container, ByVal index As Integer)
			checkTreeLock()

			' Check if moving between containers
			If curParent IsNot Me Then
				'index == -1 means add to the end.
				If index = -1 Then
					component_Renamed.Add(comp)
				Else
					component_Renamed.Insert(index, comp)
				End If
				comp.parent = Me
				comp.graphicsConfiguration = graphicsConfiguration

				adjustListeningChildren(AWTEvent.HIERARCHY_EVENT_MASK, comp.numListening(AWTEvent.HIERARCHY_EVENT_MASK))
				adjustListeningChildren(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK, comp.numListening(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK))
				adjustDescendants(comp.countHierarchyMembers())
			Else
				If index < component_Renamed.Count Then component_Renamed(index) = comp
			End If

			invalidateIfValid()
			If peer IsNot Nothing Then
				If comp.peer Is Nothing Then ' Remove notify was called or it didn't have peer - create new one
					comp.addNotify() ' Both container and child have peers, it means child peer should be reparented.
				Else
					' In both cases we need to reparent native widgets.
					Dim newNativeContainer As Container = heavyweightContainer
					Dim oldNativeContainer As Container = curParent.heavyweightContainer
					If oldNativeContainer IsNot newNativeContainer Then newNativeContainer.reparentChild(comp)
					comp.updateZOrder()

					If (Not comp.lightweight) AndAlso lightweight Then comp.relocateComponent()
				End If
			End If
			If curParent IsNot Me Then
				' Notify the layout manager of the added component. 
				If layoutMgr IsNot Nothing Then
					If TypeOf layoutMgr Is LayoutManager2 Then
						CType(layoutMgr, LayoutManager2).addLayoutComponent(comp, Nothing)
					Else
						layoutMgr.addLayoutComponent(Nothing, comp)
					End If
				End If
				If containerListener IsNot Nothing OrElse (eventMask And AWTEvent.CONTAINER_EVENT_MASK) <> 0 OrElse Toolkit.enabledOnToolkit(AWTEvent.CONTAINER_EVENT_MASK) Then
					Dim e As New ContainerEvent(Me, ContainerEvent.COMPONENT_ADDED, comp)
					dispatchEvent(e)
				End If
				comp.createHierarchyEvents(HierarchyEvent.HIERARCHY_CHANGED, comp, Me, HierarchyEvent.PARENT_CHANGED, Toolkit.enabledOnToolkit(AWTEvent.HIERARCHY_EVENT_MASK))

				' If component is focus owner or parent container of focus owner check that after reparenting
				' focus owner moved out if new container prohibit this kind of focus owner.
				If comp.focusOwner AndAlso (Not comp.canBeFocusOwnerRecursively()) Then
					comp.transferFocus()
				ElseIf TypeOf comp Is Container Then
					Dim focusOwner_Renamed As Component = KeyboardFocusManager.currentKeyboardFocusManager.focusOwner
					If focusOwner_Renamed IsNot Nothing AndAlso isParentOf(focusOwner_Renamed) AndAlso (Not focusOwner_Renamed.canBeFocusOwnerRecursively()) Then focusOwner_Renamed.transferFocus()
				End If
			Else
				comp.createHierarchyEvents(HierarchyEvent.HIERARCHY_CHANGED, comp, Me, HierarchyEvent.HIERARCHY_CHANGED, Toolkit.enabledOnToolkit(AWTEvent.HIERARCHY_EVENT_MASK))
			End If

			If peer IsNot Nothing AndAlso layoutMgr Is Nothing AndAlso visible Then updateCursorImmediately()
		End Sub

		''' <summary>
		''' Returns the z-order index of the component inside the container.
		''' The higher a component is in the z-order hierarchy, the lower
		''' its index.  The component with the lowest z-order index is
		''' painted last, above all other child components.
		''' </summary>
		''' <param name="comp"> the component being queried </param>
		''' <returns>  the z-order index of the component; otherwise
		'''          returns -1 if the component is <code>null</code>
		'''          or doesn't belong to the container </returns>
		''' <seealso cref= #setComponentZOrder(java.awt.Component, int)
		''' @since 1.5 </seealso>
		Public Overridable Function getComponentZOrder(ByVal comp As Component) As Integer
			If comp Is Nothing Then Return -1
			SyncLock treeLock
				' Quick check - container should be immediate parent of the component
				If comp.parent IsNot Me Then Return -1
				Return component_Renamed.IndexOf(comp)
			End SyncLock
		End Function

		''' <summary>
		''' Adds the specified component to the end of this container.
		''' Also notifies the layout manager to add the component to
		''' this container's layout using the specified constraints object.
		''' This is a convenience method for <seealso cref="#addImpl"/>.
		''' <p>
		''' This method changes layout-related information, and therefore,
		''' invalidates the component hierarchy. If the container has already been
		''' displayed, the hierarchy must be validated thereafter in order to
		''' display the added component.
		''' 
		''' </summary>
		''' <param name="comp"> the component to be added </param>
		''' <param name="constraints"> an object expressing
		'''                  layout constraints for this component </param>
		''' <exception cref="NullPointerException"> if {@code comp} is {@code null} </exception>
		''' <seealso cref= #addImpl </seealso>
		''' <seealso cref= #invalidate </seealso>
		''' <seealso cref= #validate </seealso>
		''' <seealso cref= javax.swing.JComponent#revalidate() </seealso>
		''' <seealso cref=       LayoutManager
		''' @since     JDK1.1 </seealso>
		Public Overridable Sub add(ByVal comp As Component, ByVal constraints As Object)
			addImpl(comp, constraints, -1)
		End Sub

		''' <summary>
		''' Adds the specified component to this container with the specified
		''' constraints at the specified index.  Also notifies the layout
		''' manager to add the component to the this container's layout using
		''' the specified constraints object.
		''' This is a convenience method for <seealso cref="#addImpl"/>.
		''' <p>
		''' This method changes layout-related information, and therefore,
		''' invalidates the component hierarchy. If the container has already been
		''' displayed, the hierarchy must be validated thereafter in order to
		''' display the added component.
		''' 
		''' </summary>
		''' <param name="comp"> the component to be added </param>
		''' <param name="constraints"> an object expressing layout constraints for this </param>
		''' <param name="index"> the position in the container's list at which to insert
		''' the component; <code>-1</code> means insert at the end
		''' component </param>
		''' <exception cref="NullPointerException"> if {@code comp} is {@code null} </exception>
		''' <exception cref="IllegalArgumentException"> if {@code index} is invalid (see
		'''            <seealso cref="#addImpl"/> for details) </exception>
		''' <seealso cref= #addImpl </seealso>
		''' <seealso cref= #invalidate </seealso>
		''' <seealso cref= #validate </seealso>
		''' <seealso cref= javax.swing.JComponent#revalidate() </seealso>
		''' <seealso cref= #remove </seealso>
		''' <seealso cref= LayoutManager </seealso>
		Public Overridable Sub add(ByVal comp As Component, ByVal constraints As Object, ByVal index As Integer)
		   addImpl(comp, constraints, index)
		End Sub

		''' <summary>
		''' Adds the specified component to this container at the specified
		''' index. This method also notifies the layout manager to add
		''' the component to this container's layout using the specified
		''' constraints object via the <code>addLayoutComponent</code>
		''' method.
		''' <p>
		''' The constraints are
		''' defined by the particular layout manager being used.  For
		''' example, the <code>BorderLayout</code> class defines five
		''' constraints: <code>BorderLayout.NORTH</code>,
		''' <code>BorderLayout.SOUTH</code>, <code>BorderLayout.EAST</code>,
		''' <code>BorderLayout.WEST</code>, and <code>BorderLayout.CENTER</code>.
		''' <p>
		''' The <code>GridBagLayout</code> class requires a
		''' <code>GridBagConstraints</code> object.  Failure to pass
		''' the correct type of constraints object results in an
		''' <code>IllegalArgumentException</code>.
		''' <p>
		''' If the current layout manager implements {@code LayoutManager2}, then
		''' <seealso cref="LayoutManager2#addLayoutComponent(Component,Object)"/> is invoked on
		''' it. If the current layout manager does not implement
		''' {@code LayoutManager2}, and constraints is a {@code String}, then
		''' <seealso cref="LayoutManager#addLayoutComponent(String,Component)"/> is invoked on it.
		''' <p>
		''' If the component is not an ancestor of this container and has a non-null
		''' parent, it is removed from its current parent before it is added to this
		''' container.
		''' <p>
		''' This is the method to override if a program needs to track
		''' every add request to a container as all other add methods defer
		''' to this one. An overriding method should
		''' usually include a call to the superclass's version of the method:
		''' 
		''' <blockquote>
		''' <code>super.addImpl(comp, constraints, index)</code>
		''' </blockquote>
		''' <p>
		''' This method changes layout-related information, and therefore,
		''' invalidates the component hierarchy. If the container has already been
		''' displayed, the hierarchy must be validated thereafter in order to
		''' display the added component.
		''' </summary>
		''' <param name="comp">       the component to be added </param>
		''' <param name="constraints"> an object expressing layout constraints
		'''                 for this component </param>
		''' <param name="index"> the position in the container's list at which to
		'''                 insert the component, where <code>-1</code>
		'''                 means append to the end </param>
		''' <exception cref="IllegalArgumentException"> if {@code index} is invalid;
		'''            if {@code comp} is a child of this container, the valid
		'''            range is {@code [-1, getComponentCount()-1]}; if component is
		'''            not a child of this container, the valid range is
		'''            {@code [-1, getComponentCount()]}
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if {@code comp} is an ancestor of
		'''                                     this container </exception>
		''' <exception cref="IllegalArgumentException"> if adding a window to a container </exception>
		''' <exception cref="NullPointerException"> if {@code comp} is {@code null} </exception>
		''' <seealso cref=       #add(Component) </seealso>
		''' <seealso cref=       #add(Component, int) </seealso>
		''' <seealso cref=       #add(Component, java.lang.Object) </seealso>
		''' <seealso cref= #invalidate </seealso>
		''' <seealso cref=       LayoutManager </seealso>
		''' <seealso cref=       LayoutManager2
		''' @since     JDK1.1 </seealso>
		Protected Friend Overridable Sub addImpl(ByVal comp As Component, ByVal constraints As Object, ByVal index As Integer)
			SyncLock treeLock
	'             Check for correct arguments:  index in bounds,
	'             * comp cannot be one of this container's parents,
	'             * and comp cannot be a window.
	'             * comp and container must be on the same GraphicsDevice.
	'             * if comp is container, all sub-components must be on
	'             * same GraphicsDevice.
	'             
				Dim thisGC As GraphicsConfiguration = Me.graphicsConfiguration

				If index > component_Renamed.Count OrElse (index < 0 AndAlso index <> -1) Then Throw New IllegalArgumentException("illegal component position")
				checkAddToSelf(comp)
				checkNotAWindow(comp)
				If thisGC IsNot Nothing Then comp.checkGD(thisGC.device.iDstring)

				' Reparent the component and tidy up the tree's state. 
				If comp.parent IsNot Nothing Then
					comp.parent.remove(comp)
						If index > component_Renamed.Count Then Throw New IllegalArgumentException("illegal component position")
				End If

				'index == -1 means add to the end.
				If index = -1 Then
					component_Renamed.Add(comp)
				Else
					component_Renamed.Insert(index, comp)
				End If
				comp.parent = Me
				comp.graphicsConfiguration = thisGC

				adjustListeningChildren(AWTEvent.HIERARCHY_EVENT_MASK, comp.numListening(AWTEvent.HIERARCHY_EVENT_MASK))
				adjustListeningChildren(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK, comp.numListening(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK))
				adjustDescendants(comp.countHierarchyMembers())

				invalidateIfValid()
				If peer IsNot Nothing Then comp.addNotify()

				' Notify the layout manager of the added component. 
				If layoutMgr IsNot Nothing Then
					If TypeOf layoutMgr Is LayoutManager2 Then
						CType(layoutMgr, LayoutManager2).addLayoutComponent(comp, constraints)
					ElseIf TypeOf constraints Is String Then
						layoutMgr.addLayoutComponent(CStr(constraints), comp)
					End If
				End If
				If containerListener IsNot Nothing OrElse (eventMask And AWTEvent.CONTAINER_EVENT_MASK) <> 0 OrElse Toolkit.enabledOnToolkit(AWTEvent.CONTAINER_EVENT_MASK) Then
					Dim e As New ContainerEvent(Me, ContainerEvent.COMPONENT_ADDED, comp)
					dispatchEvent(e)
				End If

				comp.createHierarchyEvents(HierarchyEvent.HIERARCHY_CHANGED, comp, Me, HierarchyEvent.PARENT_CHANGED, Toolkit.enabledOnToolkit(AWTEvent.HIERARCHY_EVENT_MASK))
				If peer IsNot Nothing AndAlso layoutMgr Is Nothing AndAlso visible Then updateCursorImmediately()
			End SyncLock
		End Sub

		Friend Overrides Function updateGraphicsData(ByVal gc As GraphicsConfiguration) As Boolean
			checkTreeLock()

			Dim ret As Boolean = MyBase.updateGraphicsData(gc)

			For Each comp As Component In component_Renamed
				If comp IsNot Nothing Then ret = ret Or comp.updateGraphicsData(gc)
			Next comp
			Return ret
		End Function

		''' <summary>
		''' Checks that all Components that this Container contains are on
		''' the same GraphicsDevice as this Container.  If not, throws an
		''' IllegalArgumentException.
		''' </summary>
		Friend Overrides Sub checkGD(ByVal stringID As String)
			For Each comp As Component In component_Renamed
				If comp IsNot Nothing Then comp.checkGD(stringID)
			Next comp
		End Sub

		''' <summary>
		''' Removes the component, specified by <code>index</code>,
		''' from this container.
		''' This method also notifies the layout manager to remove the
		''' component from this container's layout via the
		''' <code>removeLayoutComponent</code> method.
		''' <p>
		''' This method changes layout-related information, and therefore,
		''' invalidates the component hierarchy. If the container has already been
		''' displayed, the hierarchy must be validated thereafter in order to
		''' reflect the changes.
		''' 
		''' </summary>
		''' <param name="index">   the index of the component to be removed </param>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code index} is not in
		'''         range {@code [0, getComponentCount()-1]} </exception>
		''' <seealso cref= #add </seealso>
		''' <seealso cref= #invalidate </seealso>
		''' <seealso cref= #validate </seealso>
		''' <seealso cref= #getComponentCount
		''' @since JDK1.1 </seealso>
		Public Overridable Sub remove(ByVal index As Integer)
			SyncLock treeLock
				If index < 0 OrElse index >= component_Renamed.Count Then Throw New ArrayIndexOutOfBoundsException(index)
				Dim comp As Component = component_Renamed(index)
				If peer IsNot Nothing Then comp.removeNotify()
				If layoutMgr IsNot Nothing Then layoutMgr.removeLayoutComponent(comp)

				adjustListeningChildren(AWTEvent.HIERARCHY_EVENT_MASK, -comp.numListening(AWTEvent.HIERARCHY_EVENT_MASK))
				adjustListeningChildren(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK, -comp.numListening(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK))
				adjustDescendants(-(comp.countHierarchyMembers()))

				comp.parent = Nothing
				component_Renamed.RemoveAt(index)
				comp.graphicsConfiguration = Nothing

				invalidateIfValid()
				If containerListener IsNot Nothing OrElse (eventMask And AWTEvent.CONTAINER_EVENT_MASK) <> 0 OrElse Toolkit.enabledOnToolkit(AWTEvent.CONTAINER_EVENT_MASK) Then
					Dim e As New ContainerEvent(Me, ContainerEvent.COMPONENT_REMOVED, comp)
					dispatchEvent(e)
				End If

				comp.createHierarchyEvents(HierarchyEvent.HIERARCHY_CHANGED, comp, Me, HierarchyEvent.PARENT_CHANGED, Toolkit.enabledOnToolkit(AWTEvent.HIERARCHY_EVENT_MASK))
				If peer IsNot Nothing AndAlso layoutMgr Is Nothing AndAlso visible Then updateCursorImmediately()
			End SyncLock
		End Sub

		''' <summary>
		''' Removes the specified component from this container.
		''' This method also notifies the layout manager to remove the
		''' component from this container's layout via the
		''' <code>removeLayoutComponent</code> method.
		''' <p>
		''' This method changes layout-related information, and therefore,
		''' invalidates the component hierarchy. If the container has already been
		''' displayed, the hierarchy must be validated thereafter in order to
		''' reflect the changes.
		''' </summary>
		''' <param name="comp"> the component to be removed </param>
		''' <exception cref="NullPointerException"> if {@code comp} is {@code null} </exception>
		''' <seealso cref= #add </seealso>
		''' <seealso cref= #invalidate </seealso>
		''' <seealso cref= #validate </seealso>
		''' <seealso cref= #remove(int) </seealso>
		Public Overridable Sub remove(ByVal comp As Component)
			SyncLock treeLock
				If comp.parent Is Me Then
					Dim index As Integer = component_Renamed.IndexOf(comp)
					If index >= 0 Then remove(index)
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' Removes all the components from this container.
		''' This method also notifies the layout manager to remove the
		''' components from this container's layout via the
		''' <code>removeLayoutComponent</code> method.
		''' <p>
		''' This method changes layout-related information, and therefore,
		''' invalidates the component hierarchy. If the container has already been
		''' displayed, the hierarchy must be validated thereafter in order to
		''' reflect the changes.
		''' </summary>
		''' <seealso cref= #add </seealso>
		''' <seealso cref= #remove </seealso>
		''' <seealso cref= #invalidate </seealso>
		Public Overridable Sub removeAll()
			SyncLock treeLock
				adjustListeningChildren(AWTEvent.HIERARCHY_EVENT_MASK, -listeningChildren)
				adjustListeningChildren(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK, -listeningBoundsChildren)
				adjustDescendants(-descendantsCount)

				Do While component_Renamed.Count > 0
					Dim comp As Component = component_Renamed.Remove(component_Renamed.Count-1)

					If peer IsNot Nothing Then comp.removeNotify()
					If layoutMgr IsNot Nothing Then layoutMgr.removeLayoutComponent(comp)
					comp.parent = Nothing
					comp.graphicsConfiguration = Nothing
					If containerListener IsNot Nothing OrElse (eventMask And AWTEvent.CONTAINER_EVENT_MASK) <> 0 OrElse Toolkit.enabledOnToolkit(AWTEvent.CONTAINER_EVENT_MASK) Then
						Dim e As New ContainerEvent(Me, ContainerEvent.COMPONENT_REMOVED, comp)
						dispatchEvent(e)
					End If

					comp.createHierarchyEvents(HierarchyEvent.HIERARCHY_CHANGED, comp, Me, HierarchyEvent.PARENT_CHANGED, Toolkit.enabledOnToolkit(AWTEvent.HIERARCHY_EVENT_MASK))
				Loop
				If peer IsNot Nothing AndAlso layoutMgr Is Nothing AndAlso visible Then updateCursorImmediately()
				invalidateIfValid()
			End SyncLock
		End Sub

		' Should only be called while holding tree lock
		Friend Overrides Function numListening(ByVal mask As Long) As Integer
			Dim superListening As Integer = MyBase.numListening(mask)

			If mask = AWTEvent.HIERARCHY_EVENT_MASK Then
				If eventLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then
					' Verify listeningChildren is correct
					Dim sum As Integer = 0
					For Each comp As Component In component_Renamed
						sum += comp.numListening(mask)
					Next comp
					If listeningChildren <> sum Then eventLog.fine("Assertion (listeningChildren == sum) failed")
				End If
				Return listeningChildren + superListening
			ElseIf mask = AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK Then
				If eventLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then
					' Verify listeningBoundsChildren is correct
					Dim sum As Integer = 0
					For Each comp As Component In component_Renamed
						sum += comp.numListening(mask)
					Next comp
					If listeningBoundsChildren <> sum Then eventLog.fine("Assertion (listeningBoundsChildren == sum) failed")
				End If
				Return listeningBoundsChildren + superListening
			Else
				' assert false;
				If eventLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then eventLog.fine("This code must never be reached")
				Return superListening
			End If
		End Function

		' Should only be called while holding tree lock
		Friend Overridable Sub adjustListeningChildren(ByVal mask As Long, ByVal num As Integer)
			If eventLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then
				Dim toAssert As Boolean = (mask = AWTEvent.HIERARCHY_EVENT_MASK OrElse mask = AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK OrElse mask = (AWTEvent.HIERARCHY_EVENT_MASK Or AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK))
				If Not toAssert Then eventLog.fine("Assertion failed")
			End If

			If num = 0 Then Return

			If (mask And AWTEvent.HIERARCHY_EVENT_MASK) <> 0 Then listeningChildren += num
			If (mask And AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) <> 0 Then listeningBoundsChildren += num

			adjustListeningChildrenOnParent(mask, num)
		End Sub

		' Should only be called while holding tree lock
		Friend Overridable Sub adjustDescendants(ByVal num As Integer)
			If num = 0 Then Return

			descendantsCount += num
			adjustDecendantsOnParent(num)
		End Sub

		' Should only be called while holding tree lock
		Friend Overridable Sub adjustDecendantsOnParent(ByVal num As Integer)
			If parent IsNot Nothing Then parent.adjustDescendants(num)
		End Sub

		' Should only be called while holding tree lock
		Friend Overrides Function countHierarchyMembers() As Integer
			If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then
				' Verify descendantsCount is correct
				Dim sum As Integer = 0
				For Each comp As Component In component_Renamed
					sum += comp.countHierarchyMembers()
				Next comp
				If descendantsCount <> sum Then log.fine("Assertion (descendantsCount == sum) failed")
			End If
			Return descendantsCount + 1
		End Function

		Private Function getListenersCount(ByVal id As Integer, ByVal enabledOnToolkit As Boolean) As Integer
			checkTreeLock()
			If enabledOnToolkit Then Return descendantsCount
			Select Case id
			  Case HierarchyEvent.HIERARCHY_CHANGED
				Return listeningChildren
			  Case HierarchyEvent.ANCESTOR_MOVED, HierarchyEvent.ANCESTOR_RESIZED
				Return listeningBoundsChildren
			  Case Else
				Return 0
			End Select
		End Function

		Friend NotOverridable Overrides Function createHierarchyEvents(ByVal id As Integer, ByVal changed As Component, ByVal changedParent As Container, ByVal changeFlags As Long, ByVal enabledOnToolkit As Boolean) As Integer
			checkTreeLock()
			Dim listeners_Renamed As Integer = getListenersCount(id, enabledOnToolkit)

			Dim count As Integer = listeners_Renamed
			Dim i As Integer = 0
			Do While count > 0
				count -= component_Renamed(i).createHierarchyEvents(id, changed, changedParent, changeFlags, enabledOnToolkit)
				i += 1
			Loop
			Return listeners_Renamed + MyBase.createHierarchyEvents(id, changed, changedParent, changeFlags, enabledOnToolkit)
		End Function

		Friend Sub createChildHierarchyEvents(ByVal id As Integer, ByVal changeFlags As Long, ByVal enabledOnToolkit As Boolean)
			checkTreeLock()
			If component_Renamed.Count = 0 Then Return
			Dim listeners_Renamed As Integer = getListenersCount(id, enabledOnToolkit)

			Dim count As Integer = listeners_Renamed
			Dim i As Integer = 0
			Do While count > 0
				count -= component_Renamed(i).createHierarchyEvents(id, Me, parent, changeFlags, enabledOnToolkit)
				i += 1
			Loop
		End Sub

		''' <summary>
		''' Gets the layout manager for this container. </summary>
		''' <seealso cref= #doLayout </seealso>
		''' <seealso cref= #setLayout </seealso>
		Public Overridable Property layout As LayoutManager
			Get
				Return layoutMgr
			End Get
			Set(ByVal mgr As LayoutManager)
				layoutMgr = mgr
				invalidateIfValid()
			End Set
		End Property


		''' <summary>
		''' Causes this container to lay out its components.  Most programs
		''' should not call this method directly, but should invoke
		''' the <code>validate</code> method instead. </summary>
		''' <seealso cref= LayoutManager#layoutContainer </seealso>
		''' <seealso cref= #setLayout </seealso>
		''' <seealso cref= #validate
		''' @since JDK1.1 </seealso>
		Public Overrides Sub doLayout()
			layout()
		End Sub

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>doLayout()</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overrides Sub layout()
			Dim layoutMgr As LayoutManager = Me.layoutMgr
			If layoutMgr IsNot Nothing Then layoutMgr.layoutContainer(Me)
		End Sub

		''' <summary>
		''' Indicates if this container is a <i>validate root</i>.
		''' <p>
		''' Layout-related changes, such as bounds of the validate root descendants,
		''' do not affect the layout of the validate root parent. This peculiarity
		''' enables the {@code invalidate()} method to stop invalidating the
		''' component hierarchy when the method encounters a validate root. However,
		''' to preserve backward compatibility this new optimized behavior is
		''' enabled only when the {@code java.awt.smartInvalidate} system property
		''' value is set to {@code true}.
		''' <p>
		''' If a component hierarchy contains validate roots and the new optimized
		''' {@code invalidate()} behavior is enabled, the {@code validate()} method
		''' must be invoked on the validate root of a previously invalidated
		''' component to restore the validity of the hierarchy later. Otherwise,
		''' calling the {@code validate()} method on the top-level container (such
		''' as a {@code Frame} object) should be used to restore the validity of the
		''' component hierarchy.
		''' <p>
		''' The {@code Window} class and the {@code Applet} class are the validate
		''' roots in AWT.  Swing introduces more validate roots.
		''' </summary>
		''' <returns> whether this container is a validate root </returns>
		''' <seealso cref= #invalidate </seealso>
		''' <seealso cref= java.awt.Component#invalidate </seealso>
		''' <seealso cref= javax.swing.JComponent#isValidateRoot </seealso>
		''' <seealso cref= javax.swing.JComponent#revalidate
		''' @since 1.7 </seealso>
		Public Overridable Property validateRoot As Boolean
			Get
				Return False
			End Get
		End Property

		Private Shared ReadOnly isJavaAwtSmartInvalidate As Boolean

		''' <summary>
		''' Invalidates the parent of the container unless the container
		''' is a validate root.
		''' </summary>
		Friend Overrides Sub invalidateParent()
			If (Not isJavaAwtSmartInvalidate) OrElse (Not validateRoot) Then MyBase.invalidateParent()
		End Sub

		''' <summary>
		''' Invalidates the container.
		''' <p>
		''' If the {@code LayoutManager} installed on this container is an instance
		''' of the {@code LayoutManager2} interface, then
		''' the <seealso cref="LayoutManager2#invalidateLayout(Container)"/> method is invoked
		''' on it supplying this {@code Container} as the argument.
		''' <p>
		''' Afterwards this method marks this container invalid, and invalidates its
		''' ancestors. See the <seealso cref="Component#invalidate"/> method for more details.
		''' </summary>
		''' <seealso cref= #validate </seealso>
		''' <seealso cref= #layout </seealso>
		''' <seealso cref= LayoutManager2 </seealso>
		Public Overrides Sub invalidate()
			Dim layoutMgr As LayoutManager = Me.layoutMgr
			If TypeOf layoutMgr Is LayoutManager2 Then
				Dim lm As LayoutManager2 = CType(layoutMgr, LayoutManager2)
				lm.invalidateLayout(Me)
			End If
			MyBase.invalidate()
		End Sub

		''' <summary>
		''' Validates this container and all of its subcomponents.
		''' <p>
		''' Validating a container means laying out its subcomponents.
		''' Layout-related changes, such as setting the bounds of a component, or
		''' adding a component to the container, invalidate the container
		''' automatically.  Note that the ancestors of the container may be
		''' invalidated also (see <seealso cref="Component#invalidate"/> for details.)
		''' Therefore, to restore the validity of the hierarchy, the {@code
		''' validate()} method should be invoked on the top-most invalid
		''' container of the hierarchy.
		''' <p>
		''' Validating the container may be a quite time-consuming operation. For
		''' performance reasons a developer may postpone the validation of the
		''' hierarchy till a set of layout-related operations completes, e.g. after
		''' adding all the children to the container.
		''' <p>
		''' If this {@code Container} is not valid, this method invokes
		''' the {@code validateTree} method and marks this {@code Container}
		''' as valid. Otherwise, no action is performed.
		''' </summary>
		''' <seealso cref= #add(java.awt.Component) </seealso>
		''' <seealso cref= #invalidate </seealso>
		''' <seealso cref= Container#isValidateRoot </seealso>
		''' <seealso cref= javax.swing.JComponent#revalidate() </seealso>
		''' <seealso cref= #validateTree </seealso>
		Public Overrides Sub validate()
			Dim updateCur As Boolean = False
			SyncLock treeLock
				If ((Not valid) OrElse descendUnconditionallyWhenValidating) AndAlso peer IsNot Nothing Then
					Dim p As java.awt.peer.ContainerPeer = Nothing
					If TypeOf peer Is java.awt.peer.ContainerPeer Then p = CType(peer, java.awt.peer.ContainerPeer)
					If p IsNot Nothing Then p.beginValidate()
					validateTree()
					If p IsNot Nothing Then
						p.endValidate()
						' Avoid updating cursor if this is an internal call.
						' See validateUnconditionally() for details.
						If Not descendUnconditionallyWhenValidating Then updateCur = visible
					End If
				End If
			End SyncLock
			If updateCur Then updateCursorImmediately()
		End Sub

		''' <summary>
		''' Indicates whether valid containers should also traverse their
		''' children and call the validateTree() method on them.
		''' 
		''' Synchronization: TreeLock.
		''' 
		''' The field is allowed to be static as long as the TreeLock itself is
		''' static.
		''' </summary>
		''' <seealso cref= #validateUnconditionally() </seealso>
		Private Shared descendUnconditionallyWhenValidating As Boolean = False

		''' <summary>
		''' Unconditionally validate the component hierarchy.
		''' </summary>
		Friend Sub validateUnconditionally()
			Dim updateCur As Boolean = False
			SyncLock treeLock
				descendUnconditionallyWhenValidating = True

				validate()
				If TypeOf peer Is java.awt.peer.ContainerPeer Then updateCur = visible

				descendUnconditionallyWhenValidating = False
			End SyncLock
			If updateCur Then updateCursorImmediately()
		End Sub

		''' <summary>
		''' Recursively descends the container tree and recomputes the
		''' layout for any subtrees marked as needing it (those marked as
		''' invalid).  Synchronization should be provided by the method
		''' that calls this one:  <code>validate</code>.
		''' </summary>
		''' <seealso cref= #doLayout </seealso>
		''' <seealso cref= #validate </seealso>
		Protected Friend Overridable Sub validateTree()
			checkTreeLock()
			If (Not valid) OrElse descendUnconditionallyWhenValidating Then
				If TypeOf peer Is java.awt.peer.ContainerPeer Then CType(peer, java.awt.peer.ContainerPeer).beginLayout()
				If Not valid Then doLayout()
				For i As Integer = 0 To component_Renamed.Count - 1
					Dim comp As Component = component_Renamed(i)
					If (TypeOf comp Is Container) AndAlso Not(TypeOf comp Is Window) AndAlso ((Not comp.valid) OrElse descendUnconditionallyWhenValidating) Then
						CType(comp, Container).validateTree()
					Else
						comp.validate()
					End If
				Next i
				If TypeOf peer Is java.awt.peer.ContainerPeer Then CType(peer, java.awt.peer.ContainerPeer).endLayout()
			End If
			MyBase.validate()
		End Sub

		''' <summary>
		''' Recursively descends the container tree and invalidates all
		''' contained components.
		''' </summary>
		Friend Overridable Sub invalidateTree()
			SyncLock treeLock
				For i As Integer = 0 To component_Renamed.Count - 1
					Dim comp As Component = component_Renamed(i)
					If TypeOf comp Is Container Then
						CType(comp, Container).invalidateTree()
					Else
						comp.invalidateIfValid()
					End If
				Next i
				invalidateIfValid()
			End SyncLock
		End Sub

		''' <summary>
		''' Sets the font of this container.
		''' <p>
		''' This method changes layout-related information, and therefore,
		''' invalidates the component hierarchy.
		''' </summary>
		''' <param name="f"> The font to become this container's font. </param>
		''' <seealso cref= Component#getFont </seealso>
		''' <seealso cref= #invalidate
		''' @since JDK1.0 </seealso>
		Public Overrides Property font As Font
			Set(ByVal f As Font)
				Dim shouldinvalidate As Boolean = False
    
				Dim oldfont As Font = font
				MyBase.font = f
				Dim newfont As Font = font
				If newfont IsNot oldfont AndAlso (oldfont Is Nothing OrElse (Not oldfont.Equals(newfont))) Then invalidateTree()
			End Set
		End Property

		''' <summary>
		''' Returns the preferred size of this container.  If the preferred size has
		''' not been set explicitly by <seealso cref="Component#setPreferredSize(Dimension)"/>
		''' and this {@code Container} has a {@code non-null} <seealso cref="LayoutManager"/>,
		''' then <seealso cref="LayoutManager#preferredLayoutSize(Container)"/>
		''' is used to calculate the preferred size.
		''' 
		''' <p>Note: some implementations may cache the value returned from the
		''' {@code LayoutManager}.  Implementations that cache need not invoke
		''' {@code preferredLayoutSize} on the {@code LayoutManager} every time
		''' this method is invoked, rather the {@code LayoutManager} will only
		''' be queried after the {@code Container} becomes invalid.
		''' </summary>
		''' <returns>    an instance of <code>Dimension</code> that represents
		'''                the preferred size of this container. </returns>
		''' <seealso cref=       #getMinimumSize </seealso>
		''' <seealso cref=       #getMaximumSize </seealso>
		''' <seealso cref=       #getLayout </seealso>
		''' <seealso cref=       LayoutManager#preferredLayoutSize(Container) </seealso>
		''' <seealso cref=       Component#getPreferredSize </seealso>
		Public Property Overrides preferredSize As Dimension
			Get
				Return preferredSize()
			End Get
		End Property

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>getPreferredSize()</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overrides Function preferredSize() As Dimension
	'         Avoid grabbing the lock if a reasonable cached size value
	'         * is available.
	'         
			Dim [dim] As Dimension = prefSize
			If [dim] Is Nothing OrElse Not(preferredSizeSet OrElse valid) Then
				SyncLock treeLock
					prefSize = If(layoutMgr IsNot Nothing, layoutMgr.preferredLayoutSize(Me), MyBase.preferredSize())
					[dim] = prefSize
				End SyncLock
			End If
			If [dim] IsNot Nothing Then
				Return New Dimension([dim])
			Else
				Return [dim]
			End If
		End Function

		''' <summary>
		''' Returns the minimum size of this container.  If the minimum size has
		''' not been set explicitly by <seealso cref="Component#setMinimumSize(Dimension)"/>
		''' and this {@code Container} has a {@code non-null} <seealso cref="LayoutManager"/>,
		''' then <seealso cref="LayoutManager#minimumLayoutSize(Container)"/>
		''' is used to calculate the minimum size.
		''' 
		''' <p>Note: some implementations may cache the value returned from the
		''' {@code LayoutManager}.  Implementations that cache need not invoke
		''' {@code minimumLayoutSize} on the {@code LayoutManager} every time
		''' this method is invoked, rather the {@code LayoutManager} will only
		''' be queried after the {@code Container} becomes invalid.
		''' </summary>
		''' <returns>    an instance of <code>Dimension</code> that represents
		'''                the minimum size of this container. </returns>
		''' <seealso cref=       #getPreferredSize </seealso>
		''' <seealso cref=       #getMaximumSize </seealso>
		''' <seealso cref=       #getLayout </seealso>
		''' <seealso cref=       LayoutManager#minimumLayoutSize(Container) </seealso>
		''' <seealso cref=       Component#getMinimumSize
		''' @since     JDK1.1 </seealso>
		Public Property Overrides minimumSize As Dimension
			Get
				Return minimumSize()
			End Get
		End Property

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>getMinimumSize()</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overrides Function minimumSize() As Dimension
	'         Avoid grabbing the lock if a reasonable cached size value
	'         * is available.
	'         
			Dim [dim] As Dimension = minSize
			If [dim] Is Nothing OrElse Not(minimumSizeSet OrElse valid) Then
				SyncLock treeLock
					minSize = If(layoutMgr IsNot Nothing, layoutMgr.minimumLayoutSize(Me), MyBase.minimumSize())
					[dim] = minSize
				End SyncLock
			End If
			If [dim] IsNot Nothing Then
				Return New Dimension([dim])
			Else
				Return [dim]
			End If
		End Function

		''' <summary>
		''' Returns the maximum size of this container.  If the maximum size has
		''' not been set explicitly by <seealso cref="Component#setMaximumSize(Dimension)"/>
		''' and the <seealso cref="LayoutManager"/> installed on this {@code Container}
		''' is an instance of <seealso cref="LayoutManager2"/>, then
		''' <seealso cref="LayoutManager2#maximumLayoutSize(Container)"/>
		''' is used to calculate the maximum size.
		''' 
		''' <p>Note: some implementations may cache the value returned from the
		''' {@code LayoutManager2}.  Implementations that cache need not invoke
		''' {@code maximumLayoutSize} on the {@code LayoutManager2} every time
		''' this method is invoked, rather the {@code LayoutManager2} will only
		''' be queried after the {@code Container} becomes invalid.
		''' </summary>
		''' <returns>    an instance of <code>Dimension</code> that represents
		'''                the maximum size of this container. </returns>
		''' <seealso cref=       #getPreferredSize </seealso>
		''' <seealso cref=       #getMinimumSize </seealso>
		''' <seealso cref=       #getLayout </seealso>
		''' <seealso cref=       LayoutManager2#maximumLayoutSize(Container) </seealso>
		''' <seealso cref=       Component#getMaximumSize </seealso>
		Public Property Overrides maximumSize As Dimension
			Get
		'         Avoid grabbing the lock if a reasonable cached size value
		'         * is available.
		'         
				Dim [dim] As Dimension = maxSize
				If [dim] Is Nothing OrElse Not(maximumSizeSet OrElse valid) Then
					SyncLock treeLock
					   If TypeOf layoutMgr Is LayoutManager2 Then
							Dim lm As LayoutManager2 = CType(layoutMgr, LayoutManager2)
							maxSize = lm.maximumLayoutSize(Me)
					   Else
							maxSize = MyBase.maximumSize
					   End If
					   [dim] = maxSize
					End SyncLock
				End If
				If [dim] IsNot Nothing Then
					Return New Dimension([dim])
				Else
					Return [dim]
				End If
			End Get
		End Property

		''' <summary>
		''' Returns the alignment along the x axis.  This specifies how
		''' the component would like to be aligned relative to other
		''' components.  The value should be a number between 0 and 1
		''' where 0 represents alignment along the origin, 1 is aligned
		''' the furthest away from the origin, 0.5 is centered, etc.
		''' </summary>
		Public Property Overrides alignmentX As Single
			Get
				Dim xAlign As Single
				If TypeOf layoutMgr Is LayoutManager2 Then
					SyncLock treeLock
						Dim lm As LayoutManager2 = CType(layoutMgr, LayoutManager2)
						xAlign = lm.getLayoutAlignmentX(Me)
					End SyncLock
				Else
					xAlign = MyBase.alignmentX
				End If
				Return xAlign
			End Get
		End Property

		''' <summary>
		''' Returns the alignment along the y axis.  This specifies how
		''' the component would like to be aligned relative to other
		''' components.  The value should be a number between 0 and 1
		''' where 0 represents alignment along the origin, 1 is aligned
		''' the furthest away from the origin, 0.5 is centered, etc.
		''' </summary>
		Public Property Overrides alignmentY As Single
			Get
				Dim yAlign As Single
				If TypeOf layoutMgr Is LayoutManager2 Then
					SyncLock treeLock
						Dim lm As LayoutManager2 = CType(layoutMgr, LayoutManager2)
						yAlign = lm.getLayoutAlignmentY(Me)
					End SyncLock
				Else
					yAlign = MyBase.alignmentY
				End If
				Return yAlign
			End Get
		End Property

		''' <summary>
		''' Paints the container. This forwards the paint to any lightweight
		''' components that are children of this container. If this method is
		''' reimplemented, super.paint(g) should be called so that lightweight
		''' components are properly rendered. If a child component is entirely
		''' clipped by the current clipping setting in g, paint() will not be
		''' forwarded to that child.
		''' </summary>
		''' <param name="g"> the specified Graphics window </param>
		''' <seealso cref=   Component#update(Graphics) </seealso>
		Public Overrides Sub paint(ByVal g As Graphics)
			If showing Then
				SyncLock objectLock
					If printing Then
						If printingThreads.contains(Thread.CurrentThread) Then Return
					End If
				End SyncLock

				' The container is showing on screen and
				' this paint() is not called from print().
				' Paint self and forward the paint to lightweight subcomponents.

				' super.paint(); -- Don't bother, since it's a NOP.

				GraphicsCallback.PaintCallback.instance.runComponents(componentsSync, g, GraphicsCallback.LIGHTWEIGHTS)
			End If
		End Sub

		''' <summary>
		''' Updates the container.  This forwards the update to any lightweight
		''' components that are children of this container.  If this method is
		''' reimplemented, super.update(g) should be called so that lightweight
		''' components are properly rendered.  If a child component is entirely
		''' clipped by the current clipping setting in g, update() will not be
		''' forwarded to that child.
		''' </summary>
		''' <param name="g"> the specified Graphics window </param>
		''' <seealso cref=   Component#update(Graphics) </seealso>
		Public Overrides Sub update(ByVal g As Graphics)
			If showing Then
				If Not(TypeOf peer Is java.awt.peer.LightweightPeer) Then g.clearRect(0, 0, width, height)
				paint(g)
			End If
		End Sub

		''' <summary>
		''' Prints the container. This forwards the print to any lightweight
		''' components that are children of this container. If this method is
		''' reimplemented, super.print(g) should be called so that lightweight
		''' components are properly rendered. If a child component is entirely
		''' clipped by the current clipping setting in g, print() will not be
		''' forwarded to that child.
		''' </summary>
		''' <param name="g"> the specified Graphics window </param>
		''' <seealso cref=   Component#update(Graphics) </seealso>
		Public Overrides Sub print(ByVal g As Graphics)
			If showing Then
				Dim t As Thread = Thread.CurrentThread
				Try
					SyncLock objectLock
						If printingThreads Is Nothing Then printingThreads = New HashSet(Of )
						printingThreads.add(t)
						printing = True
					End SyncLock
					MyBase.print(g) ' By default, Component.print() calls paint()
				Finally
					SyncLock objectLock
						printingThreads.remove(t)
						printing = Not printingThreads.empty
					End SyncLock
				End Try

				GraphicsCallback.PrintCallback.instance.runComponents(componentsSync, g, GraphicsCallback.LIGHTWEIGHTS)
			End If
		End Sub

		''' <summary>
		''' Paints each of the components in this container. </summary>
		''' <param name="g">   the graphics context. </param>
		''' <seealso cref=       Component#paint </seealso>
		''' <seealso cref=       Component#paintAll </seealso>
		Public Overridable Sub paintComponents(ByVal g As Graphics)
			If showing Then GraphicsCallback.PaintAllCallback.instance.runComponents(componentsSync, g, GraphicsCallback.TWO_PASSES)
		End Sub

		''' <summary>
		''' Simulates the peer callbacks into java.awt for printing of
		''' lightweight Containers. </summary>
		''' <param name="g">   the graphics context to use for printing. </param>
		''' <seealso cref=       Component#printAll </seealso>
		''' <seealso cref=       #printComponents </seealso>
		Friend Overrides Sub lightweightPaint(ByVal g As Graphics)
			MyBase.lightweightPaint(g)
			paintHeavyweightComponents(g)
		End Sub

		''' <summary>
		''' Prints all the heavyweight subcomponents.
		''' </summary>
		Friend Overrides Sub paintHeavyweightComponents(ByVal g As Graphics)
			If showing Then GraphicsCallback.PaintHeavyweightComponentsCallback.instance.runComponents(componentsSync, g, GraphicsCallback.LIGHTWEIGHTS Or GraphicsCallback.HEAVYWEIGHTS)
		End Sub

		''' <summary>
		''' Prints each of the components in this container. </summary>
		''' <param name="g">   the graphics context. </param>
		''' <seealso cref=       Component#print </seealso>
		''' <seealso cref=       Component#printAll </seealso>
		Public Overridable Sub printComponents(ByVal g As Graphics)
			If showing Then GraphicsCallback.PrintAllCallback.instance.runComponents(componentsSync, g, GraphicsCallback.TWO_PASSES)
		End Sub

		''' <summary>
		''' Simulates the peer callbacks into java.awt for printing of
		''' lightweight Containers. </summary>
		''' <param name="g">   the graphics context to use for printing. </param>
		''' <seealso cref=       Component#printAll </seealso>
		''' <seealso cref=       #printComponents </seealso>
		Friend Overrides Sub lightweightPrint(ByVal g As Graphics)
			MyBase.lightweightPrint(g)
			printHeavyweightComponents(g)
		End Sub

		''' <summary>
		''' Prints all the heavyweight subcomponents.
		''' </summary>
		Friend Overrides Sub printHeavyweightComponents(ByVal g As Graphics)
			If showing Then GraphicsCallback.PrintHeavyweightComponentsCallback.instance.runComponents(componentsSync, g, GraphicsCallback.LIGHTWEIGHTS Or GraphicsCallback.HEAVYWEIGHTS)
		End Sub

		''' <summary>
		''' Adds the specified container listener to receive container events
		''' from this container.
		''' If l is null, no exception is thrown and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l"> the container listener
		''' </param>
		''' <seealso cref= #removeContainerListener </seealso>
		''' <seealso cref= #getContainerListeners </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addContainerListener(ByVal l As ContainerListener)
			If l Is Nothing Then Return
			containerListener = AWTEventMulticaster.add(containerListener, l)
			newEventsOnly = True
		End Sub

		''' <summary>
		''' Removes the specified container listener so it no longer receives
		''' container events from this container.
		''' If l is null, no exception is thrown and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l"> the container listener
		''' </param>
		''' <seealso cref= #addContainerListener </seealso>
		''' <seealso cref= #getContainerListeners </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeContainerListener(ByVal l As ContainerListener)
			If l Is Nothing Then Return
			containerListener = AWTEventMulticaster.remove(containerListener, l)
		End Sub

		''' <summary>
		''' Returns an array of all the container listeners
		''' registered on this container.
		''' </summary>
		''' <returns> all of this container's <code>ContainerListener</code>s
		'''         or an empty array if no container
		'''         listeners are currently registered
		''' </returns>
		''' <seealso cref= #addContainerListener </seealso>
		''' <seealso cref= #removeContainerListener
		''' @since 1.4 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property containerListeners As ContainerListener()
			Get
				Return getListeners(GetType(ContainerListener))
			End Get
		End Property

		''' <summary>
		''' Returns an array of all the objects currently registered
		''' as <code><em>Foo</em>Listener</code>s
		''' upon this <code>Container</code>.
		''' <code><em>Foo</em>Listener</code>s are registered using the
		''' <code>add<em>Foo</em>Listener</code> method.
		''' 
		''' <p>
		''' You can specify the <code>listenerType</code> argument
		''' with a class literal, such as
		''' <code><em>Foo</em>Listener.class</code>.
		''' For example, you can query a
		''' <code>Container</code> <code>c</code>
		''' for its container listeners with the following code:
		''' 
		''' <pre>ContainerListener[] cls = (ContainerListener[])(c.getListeners(ContainerListener.class));</pre>
		''' 
		''' If no such listeners exist, this method returns an empty array.
		''' </summary>
		''' <param name="listenerType"> the type of listeners requested; this parameter
		'''          should specify an interface that descends from
		'''          <code>java.util.EventListener</code> </param>
		''' <returns> an array of all objects registered as
		'''          <code><em>Foo</em>Listener</code>s on this container,
		'''          or an empty array if no such listeners have been added </returns>
		''' <exception cref="ClassCastException"> if <code>listenerType</code>
		'''          doesn't specify a class or interface that implements
		'''          <code>java.util.EventListener</code> </exception>
		''' <exception cref="NullPointerException"> if {@code listenerType} is {@code null}
		''' </exception>
		''' <seealso cref= #getContainerListeners
		''' 
		''' @since 1.3 </seealso>
		Public Overrides Function getListeners(Of T As java.util.EventListener)(ByVal listenerType As [Class]) As T()
			Dim l As java.util.EventListener = Nothing
			If listenerType Is GetType(ContainerListener) Then
				l = containerListener
			Else
				Return MyBase.getListeners(listenerType)
			End If
			Return AWTEventMulticaster.getListeners(l, listenerType)
		End Function

		' REMIND: remove when filtering is done at lower level
		Friend Overrides Function eventEnabled(ByVal e As AWTEvent) As Boolean
			Dim id As Integer = e.iD

			If id = ContainerEvent.COMPONENT_ADDED OrElse id = ContainerEvent.COMPONENT_REMOVED Then
				If (eventMask And AWTEvent.CONTAINER_EVENT_MASK) <> 0 OrElse containerListener IsNot Nothing Then Return True
				Return False
			End If
			Return MyBase.eventEnabled(e)
		End Function

		''' <summary>
		''' Processes events on this container. If the event is a
		''' <code>ContainerEvent</code>, it invokes the
		''' <code>processContainerEvent</code> method, else it invokes
		''' its superclass's <code>processEvent</code>.
		''' <p>Note that if the event parameter is <code>null</code>
		''' the behavior is unspecified and may result in an
		''' exception.
		''' </summary>
		''' <param name="e"> the event </param>
		Protected Friend Overrides Sub processEvent(ByVal e As AWTEvent)
			If TypeOf e Is ContainerEvent Then
				processContainerEvent(CType(e, ContainerEvent))
				Return
			End If
			MyBase.processEvent(e)
		End Sub

		''' <summary>
		''' Processes container events occurring on this container by
		''' dispatching them to any registered ContainerListener objects.
		''' NOTE: This method will not be called unless container events
		''' are enabled for this component; this happens when one of the
		''' following occurs:
		''' <ul>
		''' <li>A ContainerListener object is registered via
		'''     <code>addContainerListener</code>
		''' <li>Container events are enabled via <code>enableEvents</code>
		''' </ul>
		''' <p>Note that if the event parameter is <code>null</code>
		''' the behavior is unspecified and may result in an
		''' exception.
		''' </summary>
		''' <param name="e"> the container event </param>
		''' <seealso cref= Component#enableEvents </seealso>
		Protected Friend Overridable Sub processContainerEvent(ByVal e As ContainerEvent)
			Dim listener As ContainerListener = containerListener
			If listener IsNot Nothing Then
				Select Case e.iD
				  Case ContainerEvent.COMPONENT_ADDED
					listener.componentAdded(e)
				  Case ContainerEvent.COMPONENT_REMOVED
					listener.componentRemoved(e)
				End Select
			End If
		End Sub

	'    
	'     * Dispatches an event to this component or one of its sub components.
	'     * Create ANCESTOR_RESIZED and ANCESTOR_MOVED events in response to
	'     * COMPONENT_RESIZED and COMPONENT_MOVED events. We have to do this
	'     * here instead of in processComponentEvent because ComponentEvents
	'     * may not be enabled for this Container.
	'     * @param e the event
	'     
		Friend Overrides Sub dispatchEventImpl(ByVal e As AWTEvent)
			If (dispatcher IsNot Nothing) AndAlso dispatcher.dispatchEvent(e) Then
				' event was sent to a lightweight component.  The
				' native-produced event sent to the native container
				' must be properly disposed of by the peer, so it
				' gets forwarded.  If the native host has been removed
				' as a result of the sending the lightweight event,
				' the peer reference will be null.
				e.consume()
				If peer IsNot Nothing Then peer.handleEvent(e)
				Return
			End If

			MyBase.dispatchEventImpl(e)

			SyncLock treeLock
				Select Case e.iD
				  Case ComponentEvent.COMPONENT_RESIZED
					createChildHierarchyEvents(HierarchyEvent.ANCESTOR_RESIZED, 0, Toolkit.enabledOnToolkit(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK))
				  Case ComponentEvent.COMPONENT_MOVED
					createChildHierarchyEvents(HierarchyEvent.ANCESTOR_MOVED, 0, Toolkit.enabledOnToolkit(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK))
				  Case Else
				End Select
			End SyncLock
		End Sub

	'    
	'     * Dispatches an event to this component, without trying to forward
	'     * it to any subcomponents
	'     * @param e the event
	'     
		Friend Overridable Sub dispatchEventToSelf(ByVal e As AWTEvent)
			MyBase.dispatchEventImpl(e)
		End Sub

		''' <summary>
		''' Fetchs the top-most (deepest) lightweight component that is interested
		''' in receiving mouse events.
		''' </summary>
		Friend Overridable Function getMouseEventTarget(ByVal x As Integer, ByVal y As Integer, ByVal includeSelf As Boolean) As Component
			Return getMouseEventTarget(x, y, includeSelf, MouseEventTargetFilter.FILTER, (Not SEARCH_HEAVYWEIGHTS))
		End Function

		''' <summary>
		''' Fetches the top-most (deepest) component to receive SunDropTargetEvents.
		''' </summary>
		Friend Overridable Function getDropTargetEventTarget(ByVal x As Integer, ByVal y As Integer, ByVal includeSelf As Boolean) As Component
			Return getMouseEventTarget(x, y, includeSelf, DropTargetEventTargetFilter.FILTER, SEARCH_HEAVYWEIGHTS)
		End Function

		''' <summary>
		''' A private version of getMouseEventTarget which has two additional
		''' controllable behaviors. This method searches for the top-most
		''' descendant of this container that contains the given coordinates
		''' and is accepted by the given filter. The search will be constrained to
		''' lightweight descendants if the last argument is <code>false</code>.
		''' </summary>
		''' <param name="filter"> EventTargetFilter instance to determine whether the
		'''        given component is a valid target for this event. </param>
		''' <param name="searchHeavyweights"> if <code>false</code>, the method
		'''        will bypass heavyweight components during the search. </param>
		Private Function getMouseEventTarget(ByVal x As Integer, ByVal y As Integer, ByVal includeSelf As Boolean, ByVal filter As EventTargetFilter, ByVal searchHeavyweights As Boolean) As Component
			Dim comp As Component = Nothing
			If searchHeavyweights Then comp = getMouseEventTargetImpl(x, y, includeSelf, filter, SEARCH_HEAVYWEIGHTS, searchHeavyweights)

			If comp Is Nothing OrElse comp Is Me Then comp = getMouseEventTargetImpl(x, y, includeSelf, filter, (Not SEARCH_HEAVYWEIGHTS), searchHeavyweights)

			Return comp
		End Function

		''' <summary>
		''' A private version of getMouseEventTarget which has three additional
		''' controllable behaviors. This method searches for the top-most
		''' descendant of this container that contains the given coordinates
		''' and is accepted by the given filter. The search will be constrained to
		''' descendants of only lightweight children or only heavyweight children
		''' of this container depending on searchHeavyweightChildren. The search will
		''' be constrained to only lightweight descendants of the searched children
		''' of this container if searchHeavyweightDescendants is <code>false</code>.
		''' </summary>
		''' <param name="filter"> EventTargetFilter instance to determine whether the
		'''        selected component is a valid target for this event. </param>
		''' <param name="searchHeavyweightChildren"> if <code>true</code>, the method
		'''        will bypass immediate lightweight children during the search.
		'''        If <code>false</code>, the methods will bypass immediate
		'''        heavyweight children during the search. </param>
		''' <param name="searchHeavyweightDescendants"> if <code>false</code>, the method
		'''        will bypass heavyweight descendants which are not immediate
		'''        children during the search. If <code>true</code>, the method
		'''        will traverse both lightweight and heavyweight descendants during
		'''        the search. </param>
		Private Function getMouseEventTargetImpl(ByVal x As Integer, ByVal y As Integer, ByVal includeSelf As Boolean, ByVal filter As EventTargetFilter, ByVal searchHeavyweightChildren As Boolean, ByVal searchHeavyweightDescendants As Boolean) As Component
			SyncLock treeLock

				For i As Integer = 0 To component_Renamed.Count - 1
					Dim comp As Component = component_Renamed(i)
					If comp IsNot Nothing AndAlso comp.visible AndAlso (((Not searchHeavyweightChildren) AndAlso TypeOf comp.peer Is java.awt.peer.LightweightPeer) OrElse (searchHeavyweightChildren AndAlso Not(TypeOf comp.peer Is java.awt.peer.LightweightPeer))) AndAlso comp.contains(x - comp.x, y - comp.y) Then

						' found a component that intersects the point, see if there
						' is a deeper possibility.
						If TypeOf comp Is Container Then
							Dim child As Container = CType(comp, Container)
							Dim deeper As Component = child.getMouseEventTarget(x - child.x, y - child.y, includeSelf, filter, searchHeavyweightDescendants)
							If deeper IsNot Nothing Then Return deeper
						Else
							If filter.accept(comp) Then Return comp
						End If
					End If
				Next i

				Dim isPeerOK As Boolean
				Dim isMouseOverMe As Boolean

				isPeerOK = (TypeOf peer Is java.awt.peer.LightweightPeer) OrElse includeSelf
				isMouseOverMe = contains(x,y)

				' didn't find a child target, return this component if it's
				' a possible target
				If isMouseOverMe AndAlso isPeerOK AndAlso filter.accept(Me) Then Return Me
				' no possible target
				Return Nothing
			End SyncLock
		End Function

		Friend Interface EventTargetFilter
			Function accept(ByVal comp As Component) As Boolean
		End Interface

		Friend Class MouseEventTargetFilter
			Implements EventTargetFilter

			Friend Shared ReadOnly FILTER As EventTargetFilter = New MouseEventTargetFilter

			Private Sub New()
			End Sub

			Public Overridable Function accept(ByVal comp As Component) As Boolean
				Return (comp.eventMask And AWTEvent.MOUSE_MOTION_EVENT_MASK) <> 0 OrElse (comp.eventMask And AWTEvent.MOUSE_EVENT_MASK) <> 0 OrElse (comp.eventMask And AWTEvent.MOUSE_WHEEL_EVENT_MASK) <> 0 OrElse comp.mouseListener IsNot Nothing OrElse comp.mouseMotionListener IsNot Nothing OrElse comp.mouseWheelListener IsNot Nothing
			End Function
		End Class

		Friend Class DropTargetEventTargetFilter
			Implements EventTargetFilter

			Friend Shared ReadOnly FILTER As EventTargetFilter = New DropTargetEventTargetFilter

			Private Sub New()
			End Sub

			Public Overridable Function accept(ByVal comp As Component) As Boolean
				Dim dt As java.awt.dnd.DropTarget = comp.dropTarget
				Return dt IsNot Nothing AndAlso dt.active
			End Function
		End Class

		''' <summary>
		''' This is called by lightweight components that want the containing
		''' windowed parent to enable some kind of events on their behalf.
		''' This is needed for events that are normally only dispatched to
		''' windows to be accepted so that they can be forwarded downward to
		''' the lightweight component that has enabled them.
		''' </summary>
		Friend Overridable Sub proxyEnableEvents(ByVal events As Long)
			If TypeOf peer Is java.awt.peer.LightweightPeer Then
				' this container is lightweight.... continue sending it
				' upward.
				If parent IsNot Nothing Then parent.proxyEnableEvents(events)
			Else
				' This is a native container, so it needs to host
				' one of it's children.  If this function is called before
				' a peer has been created we don't yet have a dispatcher
				' because it has not yet been determined if this instance
				' is lightweight.
				If dispatcher IsNot Nothing Then dispatcher.enableEvents(events)
			End If
		End Sub

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>dispatchEvent(AWTEvent e)</code> 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overrides Sub deliverEvent(ByVal e As [Event])
			Dim comp As Component = getComponentAt(e.x, e.y)
			If (comp IsNot Nothing) AndAlso (comp IsNot Me) Then
				e.translate(-comp.x, -comp.y)
				comp.deliverEvent(e)
			Else
				postEvent(e)
			End If
		End Sub

		''' <summary>
		''' Locates the component that contains the x,y position.  The
		''' top-most child component is returned in the case where there
		''' is overlap in the components.  This is determined by finding
		''' the component closest to the index 0 that claims to contain
		''' the given point via Component.contains(), except that Components
		''' which have native peers take precedence over those which do not
		''' (i.e., lightweight Components).
		''' </summary>
		''' <param name="x"> the <i>x</i> coordinate </param>
		''' <param name="y"> the <i>y</i> coordinate </param>
		''' <returns> null if the component does not contain the position.
		''' If there is no child component at the requested point and the
		''' point is within the bounds of the container the container itself
		''' is returned; otherwise the top-most child is returned. </returns>
		''' <seealso cref= Component#contains
		''' @since JDK1.1 </seealso>
		Public Overrides Function getComponentAt(ByVal x As Integer, ByVal y As Integer) As Component
			Return locate(x, y)
		End Function

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>getComponentAt(int, int)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overrides Function locate(ByVal x As Integer, ByVal y As Integer) As Component
			If Not contains(x, y) Then Return Nothing
			Dim lightweight_Renamed As Component = Nothing
			SyncLock treeLock
				' Optimized version of two passes:
				' see comment in sun.awt.SunGraphicsCallback
				For Each comp As Component In component_Renamed
					If comp.contains(x - comp.x, y - comp.y) Then
						If Not comp.lightweight Then Return comp
						If lightweight_Renamed Is Nothing Then lightweight_Renamed = comp
					End If
				Next comp
			End SyncLock
			Return If(lightweight_Renamed IsNot Nothing, lightweight_Renamed, Me)
		End Function

		''' <summary>
		''' Gets the component that contains the specified point. </summary>
		''' <param name="p">   the point. </param>
		''' <returns>     returns the component that contains the point,
		'''                 or <code>null</code> if the component does
		'''                 not contain the point. </returns>
		''' <seealso cref=        Component#contains
		''' @since      JDK1.1 </seealso>
		Public Overrides Function getComponentAt(ByVal p As Point) As Component
			Return getComponentAt(p.x, p.y)
		End Function

		''' <summary>
		''' Returns the position of the mouse pointer in this <code>Container</code>'s
		''' coordinate space if the <code>Container</code> is under the mouse pointer,
		''' otherwise returns <code>null</code>.
		''' This method is similar to <seealso cref="Component#getMousePosition()"/> with the exception
		''' that it can take the <code>Container</code>'s children into account.
		''' If <code>allowChildren</code> is <code>false</code>, this method will return
		''' a non-null value only if the mouse pointer is above the <code>Container</code>
		''' directly, not above the part obscured by children.
		''' If <code>allowChildren</code> is <code>true</code>, this method returns
		''' a non-null value if the mouse pointer is above <code>Container</code> or any
		''' of its descendants.
		''' </summary>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless() returns true </exception>
		''' <param name="allowChildren"> true if children should be taken into account </param>
		''' <seealso cref=       Component#getMousePosition </seealso>
		''' <returns>    mouse coordinates relative to this <code>Component</code>, or null
		''' @since     1.5 </returns>
		Public Overridable Function getMousePosition(ByVal allowChildren As Boolean) As Point
			If GraphicsEnvironment.headless Then Throw New HeadlessException
			Dim pi As PointerInfo = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
		   )
			SyncLock treeLock
				Dim inTheSameWindow As Component = findUnderMouseInWindow(pi)
				If isSameOrAncestorOf(inTheSameWindow, allowChildren) Then Return pointRelativeToComponent(pi.location)
				Return Nothing
			End SyncLock
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedAction(Of T)

			Public Overridable Function run() As PointerInfo
				Return MouseInfo.pointerInfo
			End Function
		End Class

		Friend Overrides Function isSameOrAncestorOf(ByVal comp As Component, ByVal allowChildren As Boolean) As Boolean
			Return Me Is comp OrElse (allowChildren AndAlso isParentOf(comp))
		End Function

		''' <summary>
		''' Locates the visible child component that contains the specified
		''' position.  The top-most child component is returned in the case
		''' where there is overlap in the components.  If the containing child
		''' component is a Container, this method will continue searching for
		''' the deepest nested child component.  Components which are not
		''' visible are ignored during the search.<p>
		''' 
		''' The findComponentAt method is different from getComponentAt in
		''' that getComponentAt only searches the Container's immediate
		''' children; if the containing component is a Container,
		''' findComponentAt will search that child to find a nested component.
		''' </summary>
		''' <param name="x"> the <i>x</i> coordinate </param>
		''' <param name="y"> the <i>y</i> coordinate </param>
		''' <returns> null if the component does not contain the position.
		''' If there is no child component at the requested point and the
		''' point is within the bounds of the container the container itself
		''' is returned. </returns>
		''' <seealso cref= Component#contains </seealso>
		''' <seealso cref= #getComponentAt
		''' @since 1.2 </seealso>
		Public Overridable Function findComponentAt(ByVal x As Integer, ByVal y As Integer) As Component
			Return findComponentAt(x, y, True)
		End Function

		''' <summary>
		''' Private version of findComponentAt which has a controllable
		''' behavior. Setting 'ignoreEnabled' to 'false' bypasses disabled
		''' Components during the search. This behavior is used by the
		''' lightweight cursor support in sun.awt.GlobalCursorManager.
		''' 
		''' The addition of this feature is temporary, pending the
		''' adoption of new, public API which exports this feature.
		''' </summary>
		Friend Function findComponentAt(ByVal x As Integer, ByVal y As Integer, ByVal ignoreEnabled As Boolean) As Component
			SyncLock treeLock
				If recursivelyVisible Then Return findComponentAtImpl(x, y, ignoreEnabled)
			End SyncLock
			Return Nothing
		End Function

		Friend Function findComponentAtImpl(ByVal x As Integer, ByVal y As Integer, ByVal ignoreEnabled As Boolean) As Component
			' checkTreeLock(); commented for a performance reason

			If Not(contains(x, y) AndAlso visible AndAlso (ignoreEnabled OrElse enabled)) Then Return Nothing
			Dim lightweight_Renamed As Component = Nothing
			' Optimized version of two passes:
			' see comment in sun.awt.SunGraphicsCallback
			For Each comp As Component In component_Renamed
				Dim x1 As Integer = x - comp.x
				Dim y1 As Integer = y - comp.y
				If Not comp.contains(x1, y1) Then Continue For ' fast path
				If Not comp.lightweight Then
					Dim child As Component = getChildAt(comp, x1, y1, ignoreEnabled)
					If child IsNot Nothing Then Return child
				Else
					If lightweight_Renamed Is Nothing Then lightweight_Renamed = getChildAt(comp, x1, y1, ignoreEnabled)
				End If
			Next comp
			Return If(lightweight_Renamed IsNot Nothing, lightweight_Renamed, Me)
		End Function

		''' <summary>
		''' Helper method for findComponentAtImpl. Finds a child component using
		''' findComponentAtImpl for Container and getComponentAt for Component.
		''' </summary>
		Private Shared Function getChildAt(ByVal comp As Component, ByVal x As Integer, ByVal y As Integer, ByVal ignoreEnabled As Boolean) As Component
			If TypeOf comp Is Container Then
				comp = CType(comp, Container).findComponentAtImpl(x, y, ignoreEnabled)
			Else
				comp = comp.getComponentAt(x, y)
			End If
			If comp IsNot Nothing AndAlso comp.visible AndAlso (ignoreEnabled OrElse comp.enabled) Then Return comp
			Return Nothing
		End Function

		''' <summary>
		''' Locates the visible child component that contains the specified
		''' point.  The top-most child component is returned in the case
		''' where there is overlap in the components.  If the containing child
		''' component is a Container, this method will continue searching for
		''' the deepest nested child component.  Components which are not
		''' visible are ignored during the search.<p>
		''' 
		''' The findComponentAt method is different from getComponentAt in
		''' that getComponentAt only searches the Container's immediate
		''' children; if the containing component is a Container,
		''' findComponentAt will search that child to find a nested component.
		''' </summary>
		''' <param name="p">   the point. </param>
		''' <returns> null if the component does not contain the position.
		''' If there is no child component at the requested point and the
		''' point is within the bounds of the container the container itself
		''' is returned. </returns>
		''' <exception cref="NullPointerException"> if {@code p} is {@code null} </exception>
		''' <seealso cref= Component#contains </seealso>
		''' <seealso cref= #getComponentAt
		''' @since 1.2 </seealso>
		Public Overridable Function findComponentAt(ByVal p As Point) As Component
			Return findComponentAt(p.x, p.y)
		End Function

		''' <summary>
		''' Makes this Container displayable by connecting it to
		''' a native screen resource.  Making a container displayable will
		''' cause all of its children to be made displayable.
		''' This method is called internally by the toolkit and should
		''' not be called directly by programs. </summary>
		''' <seealso cref= Component#isDisplayable </seealso>
		''' <seealso cref= #removeNotify </seealso>
		Public Overrides Sub addNotify()
			SyncLock treeLock
				' addNotify() on the children may cause proxy event enabling
				' on this instance, so we first call super.addNotify() and
				' possibly create an lightweight event dispatcher before calling
				' addNotify() on the children which may be lightweight.
				MyBase.addNotify()
				If Not(TypeOf peer Is java.awt.peer.LightweightPeer) Then dispatcher = New LightweightDispatcher(Me)

				' We shouldn't use iterator because of the Swing menu
				' implementation specifics:
				' the menu is being assigned as a child to JLayeredPane
				' instead of particular component so always affect
				' collection of component if menu is becoming shown or hidden.
				For i As Integer = 0 To component_Renamed.Count - 1
					component_Renamed(i).addNotify()
				Next i
			End SyncLock
		End Sub

		''' <summary>
		''' Makes this Container undisplayable by removing its connection
		''' to its native screen resource.  Making a container undisplayable
		''' will cause all of its children to be made undisplayable.
		''' This method is called by the toolkit internally and should
		''' not be called directly by programs. </summary>
		''' <seealso cref= Component#isDisplayable </seealso>
		''' <seealso cref= #addNotify </seealso>
		Public Overrides Sub removeNotify()
			SyncLock treeLock
				' We shouldn't use iterator because of the Swing menu
				' implementation specifics:
				' the menu is being assigned as a child to JLayeredPane
				' instead of particular component so always affect
				' collection of component if menu is becoming shown or hidden.
				For i As Integer = component_Renamed.Count-1 To 0 Step -1
					Dim comp As Component = component_Renamed(i)
					If comp IsNot Nothing Then
						' Fix for 6607170.
						' We want to suppress focus change on disposal
						' of the focused component. But because of focus
						' is asynchronous, we should suppress focus change
						' on every component in case it receives native focus
						' in the process of disposal.
						comp.autoFocusTransferOnDisposal = False
						comp.removeNotify()
						comp.autoFocusTransferOnDisposal = True
					End If
				Next i
				' If some of the children had focus before disposal then it still has.
				' Auto-transfer focus to the next (or previous) component if auto-transfer
				' is enabled.
				If containsFocus() AndAlso KeyboardFocusManager.isAutoFocusTransferEnabledFor(Me) Then
					If Not transferFocus(False) Then transferFocusBackward(True)
				End If
				If dispatcher IsNot Nothing Then
					dispatcher.Dispose()
					dispatcher = Nothing
				End If
				MyBase.removeNotify()
			End SyncLock
		End Sub

		''' <summary>
		''' Checks if the component is contained in the component hierarchy of
		''' this container. </summary>
		''' <param name="c"> the component </param>
		''' <returns>     <code>true</code> if it is an ancestor;
		'''             <code>false</code> otherwise.
		''' @since      JDK1.1 </returns>
		Public Overridable Function isAncestorOf(ByVal c As Component) As Boolean
			Dim p As Container
			p = c.parent
			If c Is Nothing OrElse (p Is Nothing) Then Return False
			Do While p IsNot Nothing
				If p Is Me Then Return True
				p = p.parent
			Loop
			Return False
		End Function

	'    
	'     * The following code was added to support modal JInternalFrames
	'     * Unfortunately this code has to be added here so that we can get access to
	'     * some private AWT classes like SequencedEvent.
	'     *
	'     * The native container of the LW component has this field set
	'     * to tell it that it should block Mouse events for all LW
	'     * children except for the modal component.
	'     *
	'     * In the case of nested Modal components, we store the previous
	'     * modal component in the new modal components value of modalComp;
	'     

		<NonSerialized> _
		Friend modalComp As Component
		<NonSerialized> _
		Friend modalAppContext As sun.awt.AppContext

		Private Sub startLWModal()
			' Store the app context on which this component is being shown.
			' Event dispatch thread of this app context will be sleeping until
			' we wake it by any event from hideAndDisposeHandler().
			modalAppContext = sun.awt.AppContext.appContext

			' keep the KeyEvents from being dispatched
			' until the focus has been transfered
			Dim time As Long = Toolkit.eventQueue.mostRecentKeyEventTime
			Dim predictedFocusOwner As Component = If(Component.isInstanceOf(Me, "javax.swing.JInternalFrame"), CType(Me, javax.swing.JInternalFrame).mostRecentFocusOwner, Nothing)
			If predictedFocusOwner IsNot Nothing Then KeyboardFocusManager.currentKeyboardFocusManager.enqueueKeyEvents(time, predictedFocusOwner)
			' We have two mechanisms for blocking: 1. If we're on the
			' EventDispatchThread, start a new event pump. 2. If we're
			' on any other thread, call wait() on the treelock.
			Dim nativeContainer_Renamed As Container
			SyncLock treeLock
				nativeContainer_Renamed = heavyweightContainer
				If nativeContainer_Renamed.modalComp IsNot Nothing Then
					Me.modalComp = nativeContainer_Renamed.modalComp
					nativeContainer_Renamed.modalComp = Me
					Return
				Else
					nativeContainer_Renamed.modalComp = Me
				End If
			End SyncLock

			Dim pumpEventsForHierarchy As Runnable = New RunnableAnonymousInnerClassHelper

			If EventQueue.dispatchThread Then
				Dim currentSequencedEvent As SequencedEvent = KeyboardFocusManager.currentKeyboardFocusManager.currentSequencedEvent
				If currentSequencedEvent IsNot Nothing Then currentSequencedEvent.Dispose()

				pumpEventsForHierarchy.run()
			Else
				SyncLock treeLock
					Toolkit.eventQueue.postEvent(New sun.awt.PeerEvent(Me, pumpEventsForHierarchy, sun.awt.PeerEvent.PRIORITY_EVENT))
					Do While (windowClosingException Is Nothing) AndAlso (nativeContainer_Renamed.modalComp IsNot Nothing)
						Try
							treeLock.wait()
						Catch e As InterruptedException
							Exit Do
						End Try
					Loop
				End SyncLock
			End If
			If windowClosingException IsNot Nothing Then
				windowClosingException.fillInStackTrace()
				Throw windowClosingException
			End If
			If predictedFocusOwner IsNot Nothing Then KeyboardFocusManager.currentKeyboardFocusManager.dequeueKeyEvents(time, predictedFocusOwner)
		End Sub

		Private Class RunnableAnonymousInnerClassHelper
			Implements Runnable

			Public Overridable Sub run() Implements Runnable.run
				Dim dispatchThread As EventDispatchThread = CType(Thread.CurrentThread, EventDispatchThread)
                dispatchThread.pumpEventsForHierarchy(New ConditionalAnonymousInnerClassHelper)
            End Sub

			Private Class ConditionalAnonymousInnerClassHelper
				Implements Conditional

				Public Overridable Function evaluate() As Boolean Implements Conditional.evaluate
				Return ((windowClosingException Is Nothing) AndAlso (nativeContainer.modalComp IsNot Nothing))
				End Function
			End Class
		End Class

		Private Sub stopLWModal()
			SyncLock treeLock
				If modalAppContext IsNot Nothing Then
					Dim nativeContainer_Renamed As Container = heavyweightContainer
					If nativeContainer_Renamed IsNot Nothing Then
						If Me.modalComp IsNot Nothing Then
							nativeContainer_Renamed.modalComp = Me.modalComp
							Me.modalComp = Nothing
							Return
						Else
							nativeContainer_Renamed.modalComp = Nothing
						End If
					End If
					' Wake up event dispatch thread on which the dialog was
					' initially shown
					sun.awt.SunToolkit.postEvent(modalAppContext, New sun.awt.PeerEvent(Me, New WakingRunnable, sun.awt.PeerEvent.PRIORITY_EVENT))
				End If
				EventQueue.invokeLater(New WakingRunnable)
				treeLock.notifyAll()
			End SyncLock
		End Sub

		Friend NotInheritable Class WakingRunnable
			Implements Runnable

			Public Sub run() Implements Runnable.run
			End Sub
		End Class

		' End of JOptionPane support code 

		''' <summary>
		''' Returns a string representing the state of this <code>Container</code>.
		''' This method is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not be
		''' <code>null</code>.
		''' </summary>
		''' <returns>    the parameter string of this container </returns>
		Protected Friend Overrides Function paramString() As String
			Dim str As String = MyBase.paramString()
			Dim layoutMgr As LayoutManager = Me.layoutMgr
			If layoutMgr IsNot Nothing Then str &= ",layout=" & layoutMgr.GetType().name
			Return str
		End Function

		''' <summary>
		''' Prints a listing of this container to the specified output
		''' stream. The listing starts at the specified indentation.
		''' <p>
		''' The immediate children of the container are printed with
		''' an indentation of <code>indent+1</code>.  The children
		''' of those children are printed at <code>indent+2</code>
		''' and so on.
		''' </summary>
		''' <param name="out">      a print stream </param>
		''' <param name="indent">   the number of spaces to indent </param>
		''' <exception cref="NullPointerException"> if {@code out} is {@code null} </exception>
		''' <seealso cref=      Component#list(java.io.PrintStream, int)
		''' @since    JDK1.0 </seealso>
		Public Overrides Sub list(ByVal out As java.io.PrintStream, ByVal indent As Integer)
			MyBase.list(out, indent)
			SyncLock treeLock
				For i As Integer = 0 To component_Renamed.Count - 1
					Dim comp As Component = component_Renamed(i)
					If comp IsNot Nothing Then comp.list(out, indent+1)
				Next i
			End SyncLock
		End Sub

		''' <summary>
		''' Prints out a list, starting at the specified indentation,
		''' to the specified print writer.
		''' <p>
		''' The immediate children of the container are printed with
		''' an indentation of <code>indent+1</code>.  The children
		''' of those children are printed at <code>indent+2</code>
		''' and so on.
		''' </summary>
		''' <param name="out">      a print writer </param>
		''' <param name="indent">   the number of spaces to indent </param>
		''' <exception cref="NullPointerException"> if {@code out} is {@code null} </exception>
		''' <seealso cref=      Component#list(java.io.PrintWriter, int)
		''' @since    JDK1.1 </seealso>
		Public Overrides Sub list(ByVal out As java.io.PrintWriter, ByVal indent As Integer)
			MyBase.list(out, indent)
			SyncLock treeLock
				For i As Integer = 0 To component_Renamed.Count - 1
					Dim comp As Component = component_Renamed(i)
					If comp IsNot Nothing Then comp.list(out, indent+1)
				Next i
			End SyncLock
		End Sub

		''' <summary>
		''' Sets the focus traversal keys for a given traversal operation for this
		''' Container.
		''' <p>
		''' The default values for a Container's focus traversal keys are
		''' implementation-dependent. Sun recommends that all implementations for a
		''' particular native platform use the same default values. The
		''' recommendations for Windows and Unix are listed below. These
		''' recommendations are used in the Sun AWT implementations.
		''' 
		''' <table border=1 summary="Recommended default values for a Container's focus traversal keys">
		''' <tr>
		'''    <th>Identifier</th>
		'''    <th>Meaning</th>
		'''    <th>Default</th>
		''' </tr>
		''' <tr>
		'''    <td>KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS</td>
		'''    <td>Normal forward keyboard traversal</td>
		'''    <td>TAB on KEY_PRESSED, CTRL-TAB on KEY_PRESSED</td>
		''' </tr>
		''' <tr>
		'''    <td>KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS</td>
		'''    <td>Normal reverse keyboard traversal</td>
		'''    <td>SHIFT-TAB on KEY_PRESSED, CTRL-SHIFT-TAB on KEY_PRESSED</td>
		''' </tr>
		''' <tr>
		'''    <td>KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS</td>
		'''    <td>Go up one focus traversal cycle</td>
		'''    <td>none</td>
		''' </tr>
		''' <tr>
		'''    <td>KeyboardFocusManager.DOWN_CYCLE_TRAVERSAL_KEYS<td>
		'''    <td>Go down one focus traversal cycle</td>
		'''    <td>none</td>
		''' </tr>
		''' </table>
		''' 
		''' To disable a traversal key, use an empty Set; Collections.EMPTY_SET is
		''' recommended.
		''' <p>
		''' Using the AWTKeyStroke API, client code can specify on which of two
		''' specific KeyEvents, KEY_PRESSED or KEY_RELEASED, the focus traversal
		''' operation will occur. Regardless of which KeyEvent is specified,
		''' however, all KeyEvents related to the focus traversal key, including the
		''' associated KEY_TYPED event, will be consumed, and will not be dispatched
		''' to any Container. It is a runtime error to specify a KEY_TYPED event as
		''' mapping to a focus traversal operation, or to map the same event to
		''' multiple default focus traversal operations.
		''' <p>
		''' If a value of null is specified for the Set, this Container inherits the
		''' Set from its parent. If all ancestors of this Container have null
		''' specified for the Set, then the current KeyboardFocusManager's default
		''' Set is used.
		''' <p>
		''' This method may throw a {@code ClassCastException} if any {@code Object}
		''' in {@code keystrokes} is not an {@code AWTKeyStroke}.
		''' </summary>
		''' <param name="id"> one of KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
		'''        KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS,
		'''        KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS, or
		'''        KeyboardFocusManager.DOWN_CYCLE_TRAVERSAL_KEYS </param>
		''' <param name="keystrokes"> the Set of AWTKeyStroke for the specified operation </param>
		''' <seealso cref= #getFocusTraversalKeys </seealso>
		''' <seealso cref= KeyboardFocusManager#FORWARD_TRAVERSAL_KEYS </seealso>
		''' <seealso cref= KeyboardFocusManager#BACKWARD_TRAVERSAL_KEYS </seealso>
		''' <seealso cref= KeyboardFocusManager#UP_CYCLE_TRAVERSAL_KEYS </seealso>
		''' <seealso cref= KeyboardFocusManager#DOWN_CYCLE_TRAVERSAL_KEYS </seealso>
		''' <exception cref="IllegalArgumentException"> if id is not one of
		'''         KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
		'''         KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS,
		'''         KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS, or
		'''         KeyboardFocusManager.DOWN_CYCLE_TRAVERSAL_KEYS, or if keystrokes
		'''         contains null, or if any keystroke represents a KEY_TYPED event,
		'''         or if any keystroke already maps to another focus traversal
		'''         operation for this Container
		''' @since 1.4
		''' @beaninfo
		'''       bound: true </exception>
		Public Overrides Sub setFocusTraversalKeys(Of T1 As AWTKeyStroke)(ByVal id As Integer, ByVal keystrokes As java.util.Set(Of T1))
			If id < 0 OrElse id >= KeyboardFocusManager.TRAVERSAL_KEY_LENGTH Then Throw New IllegalArgumentException("invalid focus traversal key identifier")

			' Don't call super.setFocusTraversalKey. The Component parameter check
			' does not allow DOWN_CYCLE_TRAVERSAL_KEYS, but we do.
			focusTraversalKeys_NoIDCheckeck(id, keystrokes)
		End Sub

		''' <summary>
		''' Returns the Set of focus traversal keys for a given traversal operation
		''' for this Container. (See
		''' <code>setFocusTraversalKeys</code> for a full description of each key.)
		''' <p>
		''' If a Set of traversal keys has not been explicitly defined for this
		''' Container, then this Container's parent's Set is returned. If no Set
		''' has been explicitly defined for any of this Container's ancestors, then
		''' the current KeyboardFocusManager's default Set is returned.
		''' </summary>
		''' <param name="id"> one of KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
		'''        KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS,
		'''        KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS, or
		'''        KeyboardFocusManager.DOWN_CYCLE_TRAVERSAL_KEYS </param>
		''' <returns> the Set of AWTKeyStrokes for the specified operation. The Set
		'''         will be unmodifiable, and may be empty. null will never be
		'''         returned. </returns>
		''' <seealso cref= #setFocusTraversalKeys </seealso>
		''' <seealso cref= KeyboardFocusManager#FORWARD_TRAVERSAL_KEYS </seealso>
		''' <seealso cref= KeyboardFocusManager#BACKWARD_TRAVERSAL_KEYS </seealso>
		''' <seealso cref= KeyboardFocusManager#UP_CYCLE_TRAVERSAL_KEYS </seealso>
		''' <seealso cref= KeyboardFocusManager#DOWN_CYCLE_TRAVERSAL_KEYS </seealso>
		''' <exception cref="IllegalArgumentException"> if id is not one of
		'''         KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
		'''         KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS,
		'''         KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS, or
		'''         KeyboardFocusManager.DOWN_CYCLE_TRAVERSAL_KEYS
		''' @since 1.4 </exception>
		Public Overrides Function getFocusTraversalKeys(ByVal id As Integer) As java.util.Set(Of AWTKeyStroke)
			If id < 0 OrElse id >= KeyboardFocusManager.TRAVERSAL_KEY_LENGTH Then Throw New IllegalArgumentException("invalid focus traversal key identifier")

			' Don't call super.getFocusTraversalKey. The Component parameter check
			' does not allow DOWN_CYCLE_TRAVERSAL_KEY, but we do.
			Return getFocusTraversalKeys_NoIDCheck(id)
		End Function

		''' <summary>
		''' Returns whether the Set of focus traversal keys for the given focus
		''' traversal operation has been explicitly defined for this Container. If
		''' this method returns <code>false</code>, this Container is inheriting the
		''' Set from an ancestor, or from the current KeyboardFocusManager.
		''' </summary>
		''' <param name="id"> one of KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
		'''        KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS,
		'''        KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS, or
		'''        KeyboardFocusManager.DOWN_CYCLE_TRAVERSAL_KEYS </param>
		''' <returns> <code>true</code> if the the Set of focus traversal keys for the
		'''         given focus traversal operation has been explicitly defined for
		'''         this Component; <code>false</code> otherwise. </returns>
		''' <exception cref="IllegalArgumentException"> if id is not one of
		'''         KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
		'''        KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS,
		'''        KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS, or
		'''        KeyboardFocusManager.DOWN_CYCLE_TRAVERSAL_KEYS
		''' @since 1.4 </exception>
		Public Overrides Function areFocusTraversalKeysSet(ByVal id As Integer) As Boolean
			If id < 0 OrElse id >= KeyboardFocusManager.TRAVERSAL_KEY_LENGTH Then Throw New IllegalArgumentException("invalid focus traversal key identifier")

			Return (focusTraversalKeys IsNot Nothing AndAlso focusTraversalKeys(id) IsNot Nothing)
		End Function

		''' <summary>
		''' Returns whether the specified Container is the focus cycle root of this
		''' Container's focus traversal cycle. Each focus traversal cycle has only
		''' a single focus cycle root and each Container which is not a focus cycle
		''' root belongs to only a single focus traversal cycle. Containers which
		''' are focus cycle roots belong to two cycles: one rooted at the Container
		''' itself, and one rooted at the Container's nearest focus-cycle-root
		''' ancestor. This method will return <code>true</code> for both such
		''' Containers in this case.
		''' </summary>
		''' <param name="container"> the Container to be tested </param>
		''' <returns> <code>true</code> if the specified Container is a focus-cycle-
		'''         root of this Container; <code>false</code> otherwise </returns>
		''' <seealso cref= #isFocusCycleRoot()
		''' @since 1.4 </seealso>
		Public Overrides Function isFocusCycleRoot(ByVal container_Renamed As Container) As Boolean
			If focusCycleRoot AndAlso container_Renamed Is Me Then
				Return True
			Else
				Return MyBase.isFocusCycleRoot(container_Renamed)
			End If
		End Function

		Private Function findTraversalRoot() As Container
			' I potentially have two roots, myself and my root parent
			' If I am the current root, then use me
			' If none of my parents are roots, then use me
			' If my root parent is the current root, then use my root parent
			' If neither I nor my root parent is the current root, then
			' use my root parent (a guess)

			Dim currentFocusCycleRoot As Container = KeyboardFocusManager.currentKeyboardFocusManager.currentFocusCycleRoot
			Dim root As Container

			If currentFocusCycleRoot Is Me Then
				root = Me
			Else
				root = focusCycleRootAncestor
				If root Is Nothing Then root = Me
			End If

			If root IsNot currentFocusCycleRoot Then KeyboardFocusManager.currentKeyboardFocusManager.globalCurrentFocusCycleRootPriv = root
			Return root
		End Function

		Friend NotOverridable Overrides Function containsFocus() As Boolean
			Dim focusOwner_Renamed As Component = KeyboardFocusManager.currentKeyboardFocusManager.focusOwner
			Return isParentOf(focusOwner_Renamed)
		End Function

		''' <summary>
		''' Check if this component is the child of this container or its children.
		''' Note: this function acquires treeLock
		''' Note: this function traverses children tree only in one Window. </summary>
		''' <param name="comp"> a component in test, must not be null </param>
		Private Function isParentOf(ByVal comp As Component) As Boolean
			SyncLock treeLock
				Do While comp IsNot Nothing AndAlso comp IsNot Me AndAlso Not(TypeOf comp Is Window)
					comp = comp.parent
				Loop
				Return (comp Is Me)
			End SyncLock
		End Function

		Friend Overrides Sub clearMostRecentFocusOwnerOnHide()
			Dim reset As Boolean = False
			Dim window_Renamed As Window = Nothing

			SyncLock treeLock
				window_Renamed = containingWindow
				If window_Renamed IsNot Nothing Then
					Dim comp As Component = KeyboardFocusManager.getMostRecentFocusOwner(window_Renamed)
					reset = ((comp Is Me) OrElse isParentOf(comp))
					' This synchronized should always be the second in a pair
					' (tree lock, KeyboardFocusManager.class)
					SyncLock GetType(KeyboardFocusManager)
						Dim storedComp As Component = window_Renamed.temporaryLostComponent
						If isParentOf(storedComp) OrElse storedComp Is Me Then window_Renamed.temporaryLostComponent = Nothing
					End SyncLock
				End If
			End SyncLock

			If reset Then KeyboardFocusManager.mostRecentFocusOwnerner(window_Renamed, Nothing)
		End Sub

		Friend Overrides Sub clearCurrentFocusCycleRootOnHide()
			Dim kfm As KeyboardFocusManager = KeyboardFocusManager.currentKeyboardFocusManager
			Dim cont As Container = kfm.currentFocusCycleRoot

			If cont Is Me OrElse isParentOf(cont) Then kfm.globalCurrentFocusCycleRootPriv = Nothing
		End Sub

		Friend Property NotOverridable Overrides traversalRoot As Container
			Get
				If focusCycleRoot Then Return findTraversalRoot()
    
				Return MyBase.traversalRoot
			End Get
		End Property

		''' <summary>
		''' Sets the focus traversal policy that will manage keyboard traversal of
		''' this Container's children, if this Container is a focus cycle root. If
		''' the argument is null, this Container inherits its policy from its focus-
		''' cycle-root ancestor. If the argument is non-null, this policy will be
		''' inherited by all focus-cycle-root children that have no keyboard-
		''' traversal policy of their own (as will, recursively, their focus-cycle-
		''' root children).
		''' <p>
		''' If this Container is not a focus cycle root, the policy will be
		''' remembered, but will not be used or inherited by this or any other
		''' Containers until this Container is made a focus cycle root.
		''' </summary>
		''' <param name="policy"> the new focus traversal policy for this Container </param>
		''' <seealso cref= #getFocusTraversalPolicy </seealso>
		''' <seealso cref= #setFocusCycleRoot </seealso>
		''' <seealso cref= #isFocusCycleRoot
		''' @since 1.4
		''' @beaninfo
		'''       bound: true </seealso>
		Public Overridable Property focusTraversalPolicy As FocusTraversalPolicy
			Set(ByVal policy As FocusTraversalPolicy)
				Dim oldPolicy As FocusTraversalPolicy
				SyncLock Me
					oldPolicy = Me.focusTraversalPolicy
					Me.focusTraversalPolicy = policy
				End SyncLock
				firePropertyChange("focusTraversalPolicy", oldPolicy, policy)
			End Set
			Get
				If (Not focusTraversalPolicyProvider) AndAlso (Not focusCycleRoot) Then Return Nothing
    
				Dim policy As FocusTraversalPolicy = Me.focusTraversalPolicy
				If policy IsNot Nothing Then Return policy
    
				Dim rootAncestor As Container = focusCycleRootAncestor
				If rootAncestor IsNot Nothing Then
					Return rootAncestor.focusTraversalPolicy
				Else
					Return KeyboardFocusManager.currentKeyboardFocusManager.defaultFocusTraversalPolicy
				End If
			End Get
		End Property


		''' <summary>
		''' Returns whether the focus traversal policy has been explicitly set for
		''' this Container. If this method returns <code>false</code>, this
		''' Container will inherit its focus traversal policy from an ancestor.
		''' </summary>
		''' <returns> <code>true</code> if the focus traversal policy has been
		'''         explicitly set for this Container; <code>false</code> otherwise.
		''' @since 1.4 </returns>
		Public Overridable Property focusTraversalPolicySet As Boolean
			Get
				Return (focusTraversalPolicy IsNot Nothing)
			End Get
		End Property

		''' <summary>
		''' Sets whether this Container is the root of a focus traversal cycle. Once
		''' focus enters a traversal cycle, typically it cannot leave it via focus
		''' traversal unless one of the up- or down-cycle keys is pressed. Normal
		''' traversal is limited to this Container, and all of this Container's
		''' descendants that are not descendants of inferior focus cycle roots. Note
		''' that a FocusTraversalPolicy may bend these restrictions, however. For
		''' example, ContainerOrderFocusTraversalPolicy supports implicit down-cycle
		''' traversal.
		''' <p>
		''' The alternative way to specify the traversal order of this Container's
		''' children is to make this Container a
		''' <a href="doc-files/FocusSpec.html#FocusTraversalPolicyProviders">focus traversal policy provider</a>.
		''' </summary>
		''' <param name="focusCycleRoot"> indicates whether this Container is the root of a
		'''        focus traversal cycle </param>
		''' <seealso cref= #isFocusCycleRoot() </seealso>
		''' <seealso cref= #setFocusTraversalPolicy </seealso>
		''' <seealso cref= #getFocusTraversalPolicy </seealso>
		''' <seealso cref= ContainerOrderFocusTraversalPolicy </seealso>
		''' <seealso cref= #setFocusTraversalPolicyProvider
		''' @since 1.4
		''' @beaninfo
		'''       bound: true </seealso>
		Public Overridable Property focusCycleRoot As Boolean
			Set(ByVal focusCycleRoot As Boolean)
				Dim oldFocusCycleRoot As Boolean
				SyncLock Me
					oldFocusCycleRoot = Me.focusCycleRoot
					Me.focusCycleRoot = focusCycleRoot
				End SyncLock
				firePropertyChange("focusCycleRoot", oldFocusCycleRoot, focusCycleRoot)
			End Set
			Get
				Return focusCycleRoot
			End Get
		End Property


		''' <summary>
		''' Sets whether this container will be used to provide focus
		''' traversal policy. Container with this property as
		''' <code>true</code> will be used to acquire focus traversal policy
		''' instead of closest focus cycle root ancestor. </summary>
		''' <param name="provider"> indicates whether this container will be used to
		'''                provide focus traversal policy </param>
		''' <seealso cref= #setFocusTraversalPolicy </seealso>
		''' <seealso cref= #getFocusTraversalPolicy </seealso>
		''' <seealso cref= #isFocusTraversalPolicyProvider
		''' @since 1.5
		''' @beaninfo
		'''        bound: true </seealso>
		Public Property focusTraversalPolicyProvider As Boolean
			Set(ByVal provider As Boolean)
				Dim oldProvider As Boolean
				SyncLock Me
					oldProvider = focusTraversalPolicyProvider
					focusTraversalPolicyProvider = provider
				End SyncLock
				firePropertyChange("focusTraversalPolicyProvider", oldProvider, provider)
			End Set
			Get
				Return focusTraversalPolicyProvider
			End Get
		End Property


		''' <summary>
		''' Transfers the focus down one focus traversal cycle. If this Container is
		''' a focus cycle root, then the focus owner is set to this Container's
		''' default Component to focus, and the current focus cycle root is set to
		''' this Container. If this Container is not a focus cycle root, then no
		''' focus traversal operation occurs.
		''' </summary>
		''' <seealso cref=       Component#requestFocus() </seealso>
		''' <seealso cref=       #isFocusCycleRoot </seealso>
		''' <seealso cref=       #setFocusCycleRoot
		''' @since     1.4 </seealso>
		Public Overridable Sub transferFocusDownCycle()
			If focusCycleRoot Then
				KeyboardFocusManager.currentKeyboardFocusManager.globalCurrentFocusCycleRootPriv = Me
				Dim toFocus As Component = focusTraversalPolicy.getDefaultComponent(Me)
				If toFocus IsNot Nothing Then toFocus.requestFocus(sun.awt.CausedFocusEvent.Cause.TRAVERSAL_DOWN)
			End If
		End Sub

		Friend Overridable Sub preProcessKeyEvent(ByVal e As KeyEvent)
			Dim parent_Renamed As Container = Me.parent
			If parent_Renamed IsNot Nothing Then parent_Renamed.preProcessKeyEvent(e)
		End Sub

		Friend Overridable Sub postProcessKeyEvent(ByVal e As KeyEvent)
			Dim parent_Renamed As Container = Me.parent
			If parent_Renamed IsNot Nothing Then parent_Renamed.postProcessKeyEvent(e)
		End Sub

		Friend Overrides Function postsOldMouseEvents() As Boolean
			Return True
		End Function

		''' <summary>
		''' Sets the <code>ComponentOrientation</code> property of this container
		''' and all components contained within it.
		''' <p>
		''' This method changes layout-related information, and therefore,
		''' invalidates the component hierarchy.
		''' </summary>
		''' <param name="o"> the new component orientation of this container and
		'''        the components contained within it. </param>
		''' <exception cref="NullPointerException"> if <code>orientation</code> is null. </exception>
		''' <seealso cref= Component#setComponentOrientation </seealso>
		''' <seealso cref= Component#getComponentOrientation </seealso>
		''' <seealso cref= #invalidate
		''' @since 1.4 </seealso>
		Public Overrides Sub applyComponentOrientation(ByVal o As ComponentOrientation)
			MyBase.applyComponentOrientation(o)
			SyncLock treeLock
				For i As Integer = 0 To component_Renamed.Count - 1
					Dim comp As Component = component_Renamed(i)
					comp.applyComponentOrientation(o)
				Next i
			End SyncLock
		End Sub

		''' <summary>
		''' Adds a PropertyChangeListener to the listener list. The listener is
		''' registered for all bound properties of this [Class], including the
		''' following:
		''' <ul>
		'''    <li>this Container's font ("font")</li>
		'''    <li>this Container's background color ("background")</li>
		'''    <li>this Container's foreground color ("foreground")</li>
		'''    <li>this Container's focusability ("focusable")</li>
		'''    <li>this Container's focus traversal keys enabled state
		'''        ("focusTraversalKeysEnabled")</li>
		'''    <li>this Container's Set of FORWARD_TRAVERSAL_KEYS
		'''        ("forwardFocusTraversalKeys")</li>
		'''    <li>this Container's Set of BACKWARD_TRAVERSAL_KEYS
		'''        ("backwardFocusTraversalKeys")</li>
		'''    <li>this Container's Set of UP_CYCLE_TRAVERSAL_KEYS
		'''        ("upCycleFocusTraversalKeys")</li>
		'''    <li>this Container's Set of DOWN_CYCLE_TRAVERSAL_KEYS
		'''        ("downCycleFocusTraversalKeys")</li>
		'''    <li>this Container's focus traversal policy ("focusTraversalPolicy")
		'''        </li>
		'''    <li>this Container's focus-cycle-root state ("focusCycleRoot")</li>
		''' </ul>
		''' Note that if this Container is inheriting a bound property, then no
		''' event will be fired in response to a change in the inherited property.
		''' <p>
		''' If listener is null, no exception is thrown and no action is performed.
		''' </summary>
		''' <param name="listener">  the PropertyChangeListener to be added
		''' </param>
		''' <seealso cref= Component#removePropertyChangeListener </seealso>
		''' <seealso cref= #addPropertyChangeListener(java.lang.String,java.beans.PropertyChangeListener) </seealso>
		Public Overrides Sub addPropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
			MyBase.addPropertyChangeListener(listener)
		End Sub

		''' <summary>
		''' Adds a PropertyChangeListener to the listener list for a specific
		''' property. The specified property may be user-defined, or one of the
		''' following defaults:
		''' <ul>
		'''    <li>this Container's font ("font")</li>
		'''    <li>this Container's background color ("background")</li>
		'''    <li>this Container's foreground color ("foreground")</li>
		'''    <li>this Container's focusability ("focusable")</li>
		'''    <li>this Container's focus traversal keys enabled state
		'''        ("focusTraversalKeysEnabled")</li>
		'''    <li>this Container's Set of FORWARD_TRAVERSAL_KEYS
		'''        ("forwardFocusTraversalKeys")</li>
		'''    <li>this Container's Set of BACKWARD_TRAVERSAL_KEYS
		'''        ("backwardFocusTraversalKeys")</li>
		'''    <li>this Container's Set of UP_CYCLE_TRAVERSAL_KEYS
		'''        ("upCycleFocusTraversalKeys")</li>
		'''    <li>this Container's Set of DOWN_CYCLE_TRAVERSAL_KEYS
		'''        ("downCycleFocusTraversalKeys")</li>
		'''    <li>this Container's focus traversal policy ("focusTraversalPolicy")
		'''        </li>
		'''    <li>this Container's focus-cycle-root state ("focusCycleRoot")</li>
		'''    <li>this Container's focus-traversal-policy-provider state("focusTraversalPolicyProvider")</li>
		'''    <li>this Container's focus-traversal-policy-provider state("focusTraversalPolicyProvider")</li>
		''' </ul>
		''' Note that if this Container is inheriting a bound property, then no
		''' event will be fired in response to a change in the inherited property.
		''' <p>
		''' If listener is null, no exception is thrown and no action is performed.
		''' </summary>
		''' <param name="propertyName"> one of the property names listed above </param>
		''' <param name="listener"> the PropertyChangeListener to be added
		''' </param>
		''' <seealso cref= #addPropertyChangeListener(java.beans.PropertyChangeListener) </seealso>
		''' <seealso cref= Component#removePropertyChangeListener </seealso>
		Public Overrides Sub addPropertyChangeListener(ByVal propertyName As String, ByVal listener As java.beans.PropertyChangeListener)
			MyBase.addPropertyChangeListener(propertyName, listener)
		End Sub

		' Serialization support. A Container is responsible for restoring the
		' parent fields of its component children.

		''' <summary>
		''' Container Serial Data Version.
		''' </summary>
		Private containerSerializedDataVersion As Integer = 1

		''' <summary>
		''' Serializes this <code>Container</code> to the specified
		''' <code>ObjectOutputStream</code>.
		''' <ul>
		'''    <li>Writes default serializable fields to the stream.</li>
		'''    <li>Writes a list of serializable ContainerListener(s) as optional
		'''        data. The non-serializable ContainerListner(s) are detected and
		'''        no attempt is made to serialize them.</li>
		'''    <li>Write this Container's FocusTraversalPolicy if and only if it
		'''        is Serializable; otherwise, <code>null</code> is written.</li>
		''' </ul>
		''' </summary>
		''' <param name="s"> the <code>ObjectOutputStream</code> to write
		''' @serialData <code>null</code> terminated sequence of 0 or more pairs;
		'''   the pair consists of a <code>String</code> and <code>Object</code>;
		'''   the <code>String</code> indicates the type of object and
		'''   is one of the following:
		'''   <code>containerListenerK</code> indicating an
		'''     <code>ContainerListener</code> object;
		'''   the <code>Container</code>'s <code>FocusTraversalPolicy</code>,
		'''     or <code>null</code>
		''' </param>
		''' <seealso cref= AWTEventMulticaster#save(java.io.ObjectOutputStream, java.lang.String, java.util.EventListener) </seealso>
		''' <seealso cref= Container#containerListenerK </seealso>
		''' <seealso cref= #readObject(ObjectInputStream) </seealso>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			Dim f As java.io.ObjectOutputStream.PutField = s.putFields()
			f.put("ncomponents", component_Renamed.Count)
			f.put("component", component_Renamed.ToArray(EMPTY_ARRAY))
			f.put("layoutMgr", layoutMgr)
			f.put("dispatcher", dispatcher)
			f.put("maxSize", maxSize)
			f.put("focusCycleRoot", focusCycleRoot)
			f.put("containerSerializedDataVersion", containerSerializedDataVersion)
			f.put("focusTraversalPolicyProvider", focusTraversalPolicyProvider)
			s.writeFields()

			AWTEventMulticaster.save(s, containerListenerK, containerListener)
			s.writeObject(Nothing)

			If TypeOf focusTraversalPolicy Is java.io.Serializable Then
				s.writeObject(focusTraversalPolicy)
			Else
				s.writeObject(Nothing)
			End If
		End Sub

		''' <summary>
		''' Deserializes this <code>Container</code> from the specified
		''' <code>ObjectInputStream</code>.
		''' <ul>
		'''    <li>Reads default serializable fields from the stream.</li>
		'''    <li>Reads a list of serializable ContainerListener(s) as optional
		'''        data. If the list is null, no Listeners are installed.</li>
		'''    <li>Reads this Container's FocusTraversalPolicy, which may be null,
		'''        as optional data.</li>
		''' </ul>
		''' </summary>
		''' <param name="s"> the <code>ObjectInputStream</code> to read
		''' @serial </param>
		''' <seealso cref= #addContainerListener </seealso>
		''' <seealso cref= #writeObject(ObjectOutputStream) </seealso>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			Dim f As java.io.ObjectInputStream.GetField = s.readFields()
			Dim tmpComponent As Component() = CType(f.get("component", EMPTY_ARRAY), Component())
			Dim ncomponents As Integer = CInt(Fix(f.get("ncomponents", 0)))
			component_Renamed = New List(Of Component)(ncomponents)
			For i As Integer = 0 To ncomponents - 1
				component_Renamed.Add(tmpComponent(i))
			Next i
			layoutMgr = CType(f.get("layoutMgr", Nothing), LayoutManager)
			dispatcher = CType(f.get("dispatcher", Nothing), LightweightDispatcher)
			' Old stream. Doesn't contain maxSize among Component's fields.
			If maxSize Is Nothing Then maxSize = CType(f.get("maxSize", Nothing), Dimension)
			focusCycleRoot = f.get("focusCycleRoot", False)
			containerSerializedDataVersion = f.get("containerSerializedDataVersion", 1)
			focusTraversalPolicyProvider = f.get("focusTraversalPolicyProvider", False)
			Dim component_Renamed As IList(Of Component) = Me.component_Renamed
			For Each comp As Component In component_Renamed
				comp.parent = Me
				adjustListeningChildren(AWTEvent.HIERARCHY_EVENT_MASK, comp.numListening(AWTEvent.HIERARCHY_EVENT_MASK))
				adjustListeningChildren(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK, comp.numListening(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK))
				adjustDescendants(comp.countHierarchyMembers())
			Next comp

			Dim keyOrNull As Object
			keyOrNull = s.readObject()
			Do While Nothing IsNot keyOrNull
				Dim key As String = CStr(keyOrNull).intern()

				If containerListenerK = key Then
					addContainerListener(CType(s.readObject(), ContainerListener))
				Else
					' skip value for unrecognized key
					s.readObject()
				End If
				keyOrNull = s.readObject()
			Loop

			Try
				Dim policy As Object = s.readObject()
				If TypeOf policy Is FocusTraversalPolicy Then focusTraversalPolicy = CType(policy, FocusTraversalPolicy)
			Catch e As java.io.OptionalDataException
				' JDK 1.1/1.2/1.3 instances will not have this optional data.
				' e.eof will be true to indicate that there is no more data
				' available for this object. If e.eof is not true, throw the
				' exception as it might have been caused by reasons unrelated to
				' focusTraversalPolicy.

				If Not e.eof Then Throw e
			End Try
		End Sub

	'    
	'     * --- Accessibility Support ---
	'     

		''' <summary>
		''' Inner class of Container used to provide default support for
		''' accessibility.  This class is not meant to be used directly by
		''' application developers, but is instead meant only to be
		''' subclassed by container developers.
		''' <p>
		''' The class used to obtain the accessible role for this object,
		''' as well as implementing many of the methods in the
		''' AccessibleContainer interface.
		''' @since 1.3
		''' </summary>
		Protected Friend Class AccessibleAWTContainer
			Inherits AccessibleAWTComponent

			Private ReadOnly outerInstance As Container

			Public Sub New(ByVal outerInstance As Container)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' JDK1.3 serialVersionUID
			''' </summary>
			Private Const serialVersionUID As Long = 5081320404842566097L

			''' <summary>
			''' Returns the number of accessible children in the object.  If all
			''' of the children of this object implement <code>Accessible</code>,
			''' then this method should return the number of children of this object.
			''' </summary>
			''' <returns> the number of accessible children in the object </returns>
			Public Overridable Property accessibleChildrenCount As Integer
				Get
					Return outerInstance.accessibleChildrenCount
				End Get
			End Property

			''' <summary>
			''' Returns the nth <code>Accessible</code> child of the object.
			''' </summary>
			''' <param name="i"> zero-based index of child </param>
			''' <returns> the nth <code>Accessible</code> child of the object </returns>
			Public Overridable Function getAccessibleChild(ByVal i As Integer) As Accessible
				Return outerInstance.getAccessibleChild(i)
			End Function

			''' <summary>
			''' Returns the <code>Accessible</code> child, if one exists,
			''' contained at the local coordinate <code>Point</code>.
			''' </summary>
			''' <param name="p"> the point defining the top-left corner of the
			'''    <code>Accessible</code>, given in the coordinate space
			'''    of the object's parent </param>
			''' <returns> the <code>Accessible</code>, if it exists,
			'''    at the specified location; else <code>null</code> </returns>
			Public Overridable Function getAccessibleAt(ByVal p As Point) As Accessible
				Return outerInstance.getAccessibleAt(p)
			End Function

			''' <summary>
			''' Number of PropertyChangeListener objects registered. It's used
			''' to add/remove ContainerListener to track target Container's state.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			<NonSerialized> _
			Private propertyListenersCount As Integer = 0

			Protected Friend accessibleContainerHandler As ContainerListener = Nothing

			''' <summary>
			''' Fire <code>PropertyChange</code> listener, if one is registered,
			''' when children are added or removed.
			''' @since 1.3
			''' </summary>
			Protected Friend Class AccessibleContainerHandler
				Implements ContainerListener

				Private ReadOnly outerInstance As Container.AccessibleAWTContainer

				Public Sub New(ByVal outerInstance As Container.AccessibleAWTContainer)
					Me.outerInstance = outerInstance
				End Sub

				Public Overridable Sub componentAdded(ByVal e As ContainerEvent) Implements ContainerListener.componentAdded
					Dim c As Component = e.child
					If c IsNot Nothing AndAlso TypeOf c Is Accessible Then outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_CHILD_PROPERTY, Nothing, CType(c, Accessible).accessibleContext)
				End Sub
				Public Overridable Sub componentRemoved(ByVal e As ContainerEvent) Implements ContainerListener.componentRemoved
					Dim c As Component = e.child
					If c IsNot Nothing AndAlso TypeOf c Is Accessible Then outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_CHILD_PROPERTY, CType(c, Accessible).accessibleContext, Nothing)
				End Sub
			End Class

			''' <summary>
			''' Adds a PropertyChangeListener to the listener list.
			''' </summary>
			''' <param name="listener">  the PropertyChangeListener to be added </param>
			Public Overridable Sub addPropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
				If accessibleContainerHandler Is Nothing Then accessibleContainerHandler = New AccessibleContainerHandler(Me)
				Dim tempVar As Boolean = propertyListenersCount = 0
				propertyListenersCount += 1
				If tempVar Then outerInstance.addContainerListener(accessibleContainerHandler)
				MyBase.addPropertyChangeListener(listener)
			End Sub

			''' <summary>
			''' Remove a PropertyChangeListener from the listener list.
			''' This removes a PropertyChangeListener that was registered
			''' for all properties.
			''' </summary>
			''' <param name="listener"> the PropertyChangeListener to be removed </param>
			Public Overridable Sub removePropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
				propertyListenersCount -= 1
				If propertyListenersCount = 0 Then outerInstance.removeContainerListener(accessibleContainerHandler)
				MyBase.removePropertyChangeListener(listener)
			End Sub

		End Class ' inner class AccessibleAWTContainer

		''' <summary>
		''' Returns the <code>Accessible</code> child contained at the local
		''' coordinate <code>Point</code>, if one exists.  Otherwise
		''' returns <code>null</code>.
		''' </summary>
		''' <param name="p"> the point defining the top-left corner of the
		'''    <code>Accessible</code>, given in the coordinate space
		'''    of the object's parent </param>
		''' <returns> the <code>Accessible</code> at the specified location,
		'''    if it exists; otherwise <code>null</code> </returns>
		Friend Overridable Function getAccessibleAt(ByVal p As Point) As Accessible
			SyncLock treeLock
				If TypeOf Me Is Accessible Then
					Dim a As Accessible = CType(Me, Accessible)
					Dim ac As AccessibleContext = a.accessibleContext
					If ac IsNot Nothing Then
						Dim acmp As AccessibleComponent
						Dim location_Renamed As Point
						Dim nchildren As Integer = ac.accessibleChildrenCount
						For i As Integer = 0 To nchildren - 1
							a = ac.getAccessibleChild(i)
							If (a IsNot Nothing) Then
								ac = a.accessibleContext
								If ac IsNot Nothing Then
									acmp = ac.accessibleComponent
									If (acmp IsNot Nothing) AndAlso (acmp.showing) Then
										location_Renamed = acmp.location
										Dim np As New Point(p.x-location_Renamed.x, p.y-location_Renamed.y)
										If acmp.contains(np) Then Return a
									End If
								End If
							End If
						Next i
					End If
					Return CType(Me, Accessible)
				Else
					Dim ret As Component = Me
					If Not Me.contains(p.x,p.y) Then
						ret = Nothing
					Else
						Dim ncomponents As Integer = Me.componentCount
						For i As Integer = 0 To ncomponents - 1
							Dim comp As Component = Me.getComponent(i)
							If (comp IsNot Nothing) AndAlso comp.showing Then
								Dim location_Renamed As Point = comp.location
								If comp.contains(p.x-location_Renamed.x,p.y-location_Renamed.y) Then ret = comp
							End If
						Next i
					End If
					If TypeOf ret Is Accessible Then Return CType(ret, Accessible)
				End If
				Return Nothing
			End SyncLock
		End Function

		''' <summary>
		''' Returns the number of accessible children in the object.  If all
		''' of the children of this object implement <code>Accessible</code>,
		''' then this method should return the number of children of this object.
		''' </summary>
		''' <returns> the number of accessible children in the object </returns>
		Friend Overridable Property accessibleChildrenCount As Integer
			Get
				SyncLock treeLock
					Dim count As Integer = 0
					Dim children As Component() = Me.components
					For i As Integer = 0 To children.Length - 1
						If TypeOf children(i) Is Accessible Then count += 1
					Next i
					Return count
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Returns the nth <code>Accessible</code> child of the object.
		''' </summary>
		''' <param name="i"> zero-based index of child </param>
		''' <returns> the nth <code>Accessible</code> child of the object </returns>
		Friend Overridable Function getAccessibleChild(ByVal i As Integer) As Accessible
			SyncLock treeLock
				Dim children As Component() = Me.components
				Dim count As Integer = 0
				For j As Integer = 0 To children.Length - 1
					If TypeOf children(j) Is Accessible Then
						If count = i Then
							Return CType(children(j), Accessible)
						Else
							count += 1
						End If
					End If
				Next j
				Return Nothing
			End SyncLock
		End Function

		' ************************** MIXING CODE *******************************

		Friend Sub increaseComponentCount(ByVal c As Component)
			SyncLock treeLock
				If Not c.displayable Then Throw New IllegalStateException("Peer does not exist while invoking the increaseComponentCount() method")

				Dim addHW As Integer = 0
				Dim addLW As Integer = 0

				If TypeOf c Is Container Then
					addLW = CType(c, Container).numOfLWComponents
					addHW = CType(c, Container).numOfHWComponents
				End If
				If c.lightweight Then
					addLW += 1
				Else
					addHW += 1
				End If

				Dim cont As Container = Me
				Do While cont IsNot Nothing
					cont.numOfLWComponents += addLW
					cont.numOfHWComponents += addHW
					cont = cont.container
				Loop
			End SyncLock
		End Sub

		Friend Sub decreaseComponentCount(ByVal c As Component)
			SyncLock treeLock
				If Not c.displayable Then Throw New IllegalStateException("Peer does not exist while invoking the decreaseComponentCount() method")

				Dim subHW As Integer = 0
				Dim subLW As Integer = 0

				If TypeOf c Is Container Then
					subLW = CType(c, Container).numOfLWComponents
					subHW = CType(c, Container).numOfHWComponents
				End If
				If c.lightweight Then
					subLW += 1
				Else
					subHW += 1
				End If

				Dim cont As Container = Me
				Do While cont IsNot Nothing
					cont.numOfLWComponents -= subLW
					cont.numOfHWComponents -= subHW
					cont = cont.container
				Loop
			End SyncLock
		End Sub

		Private Property topmostComponentIndex As Integer
			Get
				checkTreeLock()
				If componentCount > 0 Then Return 0
				Return -1
			End Get
		End Property

		Private Property bottommostComponentIndex As Integer
			Get
				checkTreeLock()
				If componentCount > 0 Then Return componentCount - 1
				Return -1
			End Get
		End Property

	'    
	'     * This method is overriden to handle opaque children in non-opaque
	'     * containers.
	'     
		Friend Property NotOverridable Overrides opaqueShape As sun.java2d.pipe.Region
			Get
				checkTreeLock()
				If lightweight AndAlso nonOpaqueForMixing AndAlso hasLightweightDescendants() Then
					Dim s As sun.java2d.pipe.Region = sun.java2d.pipe.Region.EMPTY_REGION
					For index As Integer = 0 To componentCount - 1
						Dim c As Component = getComponent(index)
						If c.lightweight AndAlso c.showing Then s = s.getUnion(c.opaqueShape)
					Next index
					Return s.getIntersection(normalShape)
				End If
				Return MyBase.opaqueShape
			End Get
		End Property

		Friend Sub recursiveSubtractAndApplyShape(ByVal shape As sun.java2d.pipe.Region)
			recursiveSubtractAndApplyShape(shape, topmostComponentIndex, bottommostComponentIndex)
		End Sub

		Friend Sub recursiveSubtractAndApplyShape(ByVal shape As sun.java2d.pipe.Region, ByVal fromZorder As Integer)
			recursiveSubtractAndApplyShape(shape, fromZorder, bottommostComponentIndex)
		End Sub

		Friend Sub recursiveSubtractAndApplyShape(ByVal shape As sun.java2d.pipe.Region, ByVal fromZorder As Integer, ByVal toZorder As Integer)
			checkTreeLock()
			If mixingLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then mixingLog.fine("this = " & Me & "; shape=" & shape & "; fromZ=" & fromZorder & "; toZ=" & toZorder)
			If fromZorder = -1 Then Return
			If shape.empty Then Return
			' An invalid container with not-null layout should be ignored
			' by the mixing code, the container will be validated later
			' and the mixing code will be executed later.
			If layout IsNot Nothing AndAlso (Not valid) Then Return
			For index As Integer = fromZorder To toZorder
				Dim comp As Component = getComponent(index)
				If Not comp.lightweight Then
					comp.subtractAndApplyShape(shape)
				ElseIf TypeOf comp Is Container AndAlso CType(comp, Container).hasHeavyweightDescendants() AndAlso comp.showing Then
					CType(comp, Container).recursiveSubtractAndApplyShape(shape)
				End If
			Next index
		End Sub

		Friend Sub recursiveApplyCurrentShape()
			recursiveApplyCurrentShape(topmostComponentIndex, bottommostComponentIndex)
		End Sub

		Friend Sub recursiveApplyCurrentShape(ByVal fromZorder As Integer)
			recursiveApplyCurrentShape(fromZorder, bottommostComponentIndex)
		End Sub

		Friend Sub recursiveApplyCurrentShape(ByVal fromZorder As Integer, ByVal toZorder As Integer)
			checkTreeLock()
			If mixingLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then mixingLog.fine("this = " & Me & "; fromZ=" & fromZorder & "; toZ=" & toZorder)
			If fromZorder = -1 Then Return
			' An invalid container with not-null layout should be ignored
			' by the mixing code, the container will be validated later
			' and the mixing code will be executed later.
			If layout IsNot Nothing AndAlso (Not valid) Then Return
			For index As Integer = fromZorder To toZorder
				Dim comp As Component = getComponent(index)
				If Not comp.lightweight Then comp.applyCurrentShape()
				If TypeOf comp Is Container AndAlso CType(comp, Container).hasHeavyweightDescendants() Then CType(comp, Container).recursiveApplyCurrentShape()
			Next index
		End Sub

		Private Sub recursiveShowHeavyweightChildren()
			If (Not hasHeavyweightDescendants()) OrElse (Not visible) Then Return
			For index As Integer = 0 To componentCount - 1
				Dim comp As Component = getComponent(index)
				If comp.lightweight Then
					If TypeOf comp Is Container Then CType(comp, Container).recursiveShowHeavyweightChildren()
				Else
					If comp.visible Then
						Dim peer_Renamed As java.awt.peer.ComponentPeer = comp.peer
						If peer_Renamed IsNot Nothing Then peer_Renamed.visible = True
					End If
				End If
			Next index
		End Sub

		Private Sub recursiveHideHeavyweightChildren()
			If Not hasHeavyweightDescendants() Then Return
			For index As Integer = 0 To componentCount - 1
				Dim comp As Component = getComponent(index)
				If comp.lightweight Then
					If TypeOf comp Is Container Then CType(comp, Container).recursiveHideHeavyweightChildren()
				Else
					If comp.visible Then
						Dim peer_Renamed As java.awt.peer.ComponentPeer = comp.peer
						If peer_Renamed IsNot Nothing Then peer_Renamed.visible = False
					End If
				End If
			Next index
		End Sub

		Private Sub recursiveRelocateHeavyweightChildren(ByVal origin As Point)
			For index As Integer = 0 To componentCount - 1
				Dim comp As Component = getComponent(index)
				If comp.lightweight Then
					If TypeOf comp Is Container AndAlso CType(comp, Container).hasHeavyweightDescendants() Then
						Dim newOrigin As New Point(origin)
						newOrigin.translate(comp.x, comp.y)
						CType(comp, Container).recursiveRelocateHeavyweightChildren(newOrigin)
					End If
				Else
					Dim peer_Renamed As java.awt.peer.ComponentPeer = comp.peer
					If peer_Renamed IsNot Nothing Then peer_Renamed.boundsnds(origin.x + comp.x, origin.y + comp.y, comp.width, comp.height, java.awt.peer.ComponentPeer.SET_LOCATION)
				End If
			Next index
		End Sub

		''' <summary>
		''' Checks if the container and its direct lightweight containers are
		''' visible.
		''' 
		''' Consider the heavyweight container hides or shows the HW descendants
		''' automatically. Therefore we care of LW containers' visibility only.
		''' 
		''' This method MUST be invoked under the TreeLock.
		''' </summary>
		Friend Property recursivelyVisibleUpToHeavyweightContainer As Boolean
			Get
				If Not lightweight Then Return True
    
				Dim cont As Container = Me
				Do While cont IsNot Nothing AndAlso cont.lightweight
					If Not cont.visible Then Return False
					cont = cont.container
				Loop
				Return True
			End Get
		End Property

		Friend Overrides Sub mixOnShowing()
			SyncLock treeLock
				If mixingLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then mixingLog.fine("this = " & Me)

				Dim isLightweight As Boolean = lightweight

				If isLightweight AndAlso recursivelyVisibleUpToHeavyweightContainer Then recursiveShowHeavyweightChildren()

				If Not mixingNeeded Then Return

				If (Not isLightweight) OrElse (isLightweight AndAlso hasHeavyweightDescendants()) Then recursiveApplyCurrentShape()

				MyBase.mixOnShowing()
			End SyncLock
		End Sub

		Friend Overrides Sub mixOnHiding(ByVal isLightweight As Boolean)
			SyncLock treeLock
				If mixingLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then mixingLog.fine("this = " & Me & "; isLightweight=" & isLightweight)
				If isLightweight Then recursiveHideHeavyweightChildren()
				MyBase.mixOnHiding(isLightweight)
			End SyncLock
		End Sub

		Friend Overrides Sub mixOnReshaping()
			SyncLock treeLock
				If mixingLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then mixingLog.fine("this = " & Me)

				Dim isMixingNeeded As Boolean = mixingNeeded

				If lightweight AndAlso hasHeavyweightDescendants() Then
					Dim origin As New Point(x, y)
					Dim cont As Container = container
					Do While cont IsNot Nothing AndAlso cont.lightweight
						origin.translate(cont.x, cont.y)
						cont = cont.container
					Loop

					recursiveRelocateHeavyweightChildren(origin)

					If Not isMixingNeeded Then Return

					recursiveApplyCurrentShape()
				End If

				If Not isMixingNeeded Then Return

				MyBase.mixOnReshaping()
			End SyncLock
		End Sub

		Friend Overrides Sub mixOnZOrderChanging(ByVal oldZorder As Integer, ByVal newZorder As Integer)
			SyncLock treeLock
				If mixingLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then mixingLog.fine("this = " & Me & "; oldZ=" & oldZorder & "; newZ=" & newZorder)

				If Not mixingNeeded Then Return

				Dim becameHigher As Boolean = newZorder < oldZorder

				If becameHigher AndAlso lightweight AndAlso hasHeavyweightDescendants() Then recursiveApplyCurrentShape()
				MyBase.mixOnZOrderChanging(oldZorder, newZorder)
			End SyncLock
		End Sub

		Friend Overrides Sub mixOnValidating()
			SyncLock treeLock
				If mixingLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then mixingLog.fine("this = " & Me)

				If Not mixingNeeded Then Return

				If hasHeavyweightDescendants() Then recursiveApplyCurrentShape()

				If lightweight AndAlso nonOpaqueForMixing Then subtractAndApplyShapeBelowMe()

				MyBase.mixOnValidating()
			End SyncLock
		End Sub

		' ****************** END OF MIXING CODE ********************************
	End Class


	''' <summary>
	''' Class to manage the dispatching of MouseEvents to the lightweight descendants
	''' and SunDropTargetEvents to both lightweight and heavyweight descendants
	''' contained by a native container.
	''' 
	''' NOTE: the class name is not appropriate anymore, but we cannot change it
	''' because we must keep serialization compatibility.
	''' 
	''' @author Timothy Prinzing
	''' </summary>
	<Serializable> _
	Friend Class LightweightDispatcher
		Implements AWTEventListener

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = 5184291520170872969L
	'    
	'     * Our own mouse event for when we're dragged over from another hw
	'     * container
	'     
		Private Const LWD_MOUSE_DRAGGED_OVER As Integer = 1500

		Private Shared ReadOnly eventLog As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.awt.event.LightweightDispatcher")

		Private Shared ReadOnly BUTTONS_DOWN_MASK As Integer

		Shared Sub New()
			Dim buttonsDownMask As Integer() = sun.awt.AWTAccessor.inputEventAccessor.buttonDownMasks
			Dim mask As Integer = 0
			For Each buttonDownMask As Integer In buttonsDownMask
				mask = mask Or buttonDownMask
			Next buttonDownMask
			BUTTONS_DOWN_MASK = mask
		End Sub

		Friend Sub New(ByVal nativeContainer As Container)
			Me.nativeContainer = nativeContainer
			mouseEventTarget = New WeakReference(Of )(Nothing)
			targetLastEntered = New WeakReference(Of )(Nothing)
			targetLastEnteredDT = New WeakReference(Of )(Nothing)
			eventMask = 0
		End Sub

	'    
	'     * Clean up any resources allocated when dispatcher was created;
	'     * should be called from Container.removeNotify
	'     
		Friend Overridable Sub dispose()
			'System.out.println("Disposing lw dispatcher");
			stopListeningForOtherDrags()
			mouseEventTarget.clear()
			targetLastEntered.clear()
			targetLastEnteredDT.clear()
		End Sub

		''' <summary>
		''' Enables events to subcomponents.
		''' </summary>
		Friend Overridable Sub enableEvents(ByVal events As Long)
			eventMask = eventMask Or events
		End Sub

		''' <summary>
		''' Dispatches an event to a sub-component if necessary, and
		''' returns whether or not the event was forwarded to a
		''' sub-component.
		''' </summary>
		''' <param name="e"> the event </param>
		Friend Overridable Function dispatchEvent(ByVal e As AWTEvent) As Boolean
			Dim ret As Boolean = False

	'        
	'         * Fix for BugTraq Id 4389284.
	'         * Dispatch SunDropTargetEvents regardless of eventMask value.
	'         * Do not update cursor on dispatching SunDropTargetEvents.
	'         
			If TypeOf e Is sun.awt.dnd.SunDropTargetEvent Then

				Dim sdde As sun.awt.dnd.SunDropTargetEvent = CType(e, sun.awt.dnd.SunDropTargetEvent)
				ret = processDropTargetEvent(sdde)

			Else
				If TypeOf e Is MouseEvent AndAlso (eventMask And MOUSE_MASK) <> 0 Then
					Dim [me] As MouseEvent = CType(e, MouseEvent)
					ret = processMouseEvent([me])
				End If

				If e.iD = MouseEvent.MOUSE_MOVED Then nativeContainer.updateCursorImmediately()
			End If

			Return ret
		End Function

	'     This method effectively returns whether or not a mouse button was down
	'     * just BEFORE the event happened.  A better method name might be
	'     * wasAMouseButtonDownBeforeThisEvent().
	'     
		Private Function isMouseGrab(ByVal e As MouseEvent) As Boolean
			Dim modifiers As Integer = e.modifiersEx

			If e.iD = MouseEvent.MOUSE_PRESSED OrElse e.iD = MouseEvent.MOUSE_RELEASED Then modifiers = modifiers Xor InputEvent.getMaskForButton(e.button)
			' modifiers now as just before event 
			Return ((modifiers And BUTTONS_DOWN_MASK) <> 0)
		End Function

		''' <summary>
		''' This method attempts to distribute a mouse event to a lightweight
		''' component.  It tries to avoid doing any unnecessary probes down
		''' into the component tree to minimize the overhead of determining
		''' where to route the event, since mouse movement events tend to
		''' come in large and frequent amounts.
		''' </summary>
		Private Function processMouseEvent(ByVal e As MouseEvent) As Boolean
			Dim id As Integer = e.iD
			Dim mouseOver As Component = nativeContainer.getMouseEventTarget(e.x, e.y, Container.INCLUDE_SELF) ' sensitive to mouse events

			trackMouseEnterExit(mouseOver, e)

			Dim met As Component = mouseEventTarget.get()
			' 4508327 : MOUSE_CLICKED should only go to the recipient of
			' the accompanying MOUSE_PRESSED, so don't reset mouseEventTarget on a
			' MOUSE_CLICKED.
			If (Not isMouseGrab(e)) AndAlso id <> MouseEvent.MOUSE_CLICKED Then
				met = If(mouseOver IsNot nativeContainer, mouseOver, Nothing)
				mouseEventTarget = New WeakReference(Of )(met)
			End If

			If met IsNot Nothing Then
				Select Case id
					Case MouseEvent.MOUSE_ENTERED, MouseEvent.MOUSE_EXITED
					Case MouseEvent.MOUSE_PRESSED
						retargetMouseEvent(met, id, e)
					Case MouseEvent.MOUSE_RELEASED
						retargetMouseEvent(met, id, e)
					Case MouseEvent.MOUSE_CLICKED
						' 4508327: MOUSE_CLICKED should never be dispatched to a Component
						' other than that which received the MOUSE_PRESSED event.  If the
						' mouse is now over a different Component, don't dispatch the event.
						' The previous fix for a similar problem was associated with bug
						' 4155217.
						If mouseOver Is met Then retargetMouseEvent(mouseOver, id, e)
					Case MouseEvent.MOUSE_MOVED
						retargetMouseEvent(met, id, e)
					Case MouseEvent.MOUSE_DRAGGED
						If isMouseGrab(e) Then retargetMouseEvent(met, id, e)
					Case MouseEvent.MOUSE_WHEEL
						' This may send it somewhere that doesn't have MouseWheelEvents
						' enabled.  In this case, Component.dispatchEventImpl() will
						' retarget the event to a parent that DOES have the events enabled.
						If eventLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) AndAlso (mouseOver IsNot Nothing) Then eventLog.finest("retargeting mouse wheel to " & mouseOver.name & ", " & mouseOver.GetType())
						retargetMouseEvent(mouseOver, id, e)
				End Select
				'Consuming of wheel events is implemented in "retargetMouseEvent".
				If id <> MouseEvent.MOUSE_WHEEL Then e.consume()
			End If
			Return e.consumed
		End Function

		Private Function processDropTargetEvent(ByVal e As sun.awt.dnd.SunDropTargetEvent) As Boolean
			Dim id As Integer = e.iD
			Dim x As Integer = e.x
			Dim y As Integer = e.y

	'        
	'         * Fix for BugTraq ID 4395290.
	'         * It is possible that SunDropTargetEvent's Point is outside of the
	'         * native container bounds. In this case we truncate coordinates.
	'         
			If Not nativeContainer.contains(x, y) Then
				Dim d As Dimension = nativeContainer.size
				If d.width <= x Then
					x = d.width - 1
				ElseIf x < 0 Then
					x = 0
				End If
				If d.height <= y Then
					y = d.height - 1
				ElseIf y < 0 Then
					y = 0
				End If
			End If
			Dim mouseOver As Component = nativeContainer.getDropTargetEventTarget(x, y, Container.INCLUDE_SELF) ' not necessarily sensitive to mouse events
			trackMouseEnterExit(mouseOver, e)

			If mouseOver IsNot nativeContainer AndAlso mouseOver IsNot Nothing Then
				Select Case id
				Case sun.awt.dnd.SunDropTargetEvent.MOUSE_ENTERED, SunDropTargetEvent.MOUSE_EXITED
				Case Else
					retargetMouseEvent(mouseOver, id, e)
					e.consume()
				End Select
			End If
			Return e.consumed
		End Function

	'    
	'     * Generates dnd enter/exit events as mouse moves over lw components
	'     * @param targetOver       Target mouse is over (including native container)
	'     * @param e                SunDropTarget mouse event in native container
	'     
		Private Sub trackDropTargetEnterExit(ByVal targetOver As Component, ByVal e As MouseEvent)
			Dim id As Integer = e.iD
			If id = MouseEvent.MOUSE_ENTERED AndAlso isMouseDTInNativeContainer Then
				' This can happen if a lightweight component which initiated the
				' drag has an associated drop target. MOUSE_ENTERED comes when the
				' mouse is in the native container already. To propagate this event
				' properly we should null out targetLastEntered.
				targetLastEnteredDT.clear()
			ElseIf id = MouseEvent.MOUSE_ENTERED Then
				isMouseDTInNativeContainer = True
			ElseIf id = MouseEvent.MOUSE_EXITED Then
				isMouseDTInNativeContainer = False
			End If
			Dim tle As Component = retargetMouseEnterExit(targetOver, e, targetLastEnteredDT.get(), isMouseDTInNativeContainer)
			targetLastEnteredDT = New WeakReference(Of )(tle)
		End Sub

	'    
	'     * Generates enter/exit events as mouse moves over lw components
	'     * @param targetOver        Target mouse is over (including native container)
	'     * @param e                 Mouse event in native container
	'     
		Private Sub trackMouseEnterExit(ByVal targetOver As Component, ByVal e As MouseEvent)
			If TypeOf e Is sun.awt.dnd.SunDropTargetEvent Then
				trackDropTargetEnterExit(targetOver, e)
				Return
			End If
			Dim id As Integer = e.iD

			If id <> MouseEvent.MOUSE_EXITED AndAlso id <> MouseEvent.MOUSE_DRAGGED AndAlso id <> LWD_MOUSE_DRAGGED_OVER AndAlso (Not isMouseInNativeContainer) Then
				' any event but an exit or drag means we're in the native container
				isMouseInNativeContainer = True
				startListeningForOtherDrags()
			ElseIf id = MouseEvent.MOUSE_EXITED Then
				isMouseInNativeContainer = False
				stopListeningForOtherDrags()
			End If
			Dim tle As Component = retargetMouseEnterExit(targetOver, e, targetLastEntered.get(), isMouseInNativeContainer)
			targetLastEntered = New WeakReference(Of )(tle)
		End Sub

		Private Function retargetMouseEnterExit(ByVal targetOver As Component, ByVal e As MouseEvent, ByVal lastEntered As Component, ByVal inNativeContainer As Boolean) As Component
			Dim id As Integer = e.iD
			Dim targetEnter As Component = If(inNativeContainer, targetOver, Nothing)

			If lastEntered IsNot targetEnter Then
				If lastEntered IsNot Nothing Then retargetMouseEvent(lastEntered, MouseEvent.MOUSE_EXITED, e)
				If id = MouseEvent.MOUSE_EXITED Then e.consume()

				If targetEnter IsNot Nothing Then retargetMouseEvent(targetEnter, MouseEvent.MOUSE_ENTERED, e)
				If id = MouseEvent.MOUSE_ENTERED Then e.consume()
			End If
			Return targetEnter
		End Function

	'    
	'     * Listens to global mouse drag events so even drags originating
	'     * from other heavyweight containers will generate enter/exit
	'     * events in this container
	'     
		Private Sub startListeningForOtherDrags()
			'System.out.println("Adding AWTEventListener");
			java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
		   )
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedAction(Of T)

			Public Overridable Function run() As Object
				outerInstance.nativeContainer.toolkit.addAWTEventListener(LightweightDispatcher.this, AWTEvent.MOUSE_EVENT_MASK Or AWTEvent.MOUSE_MOTION_EVENT_MASK)
				Return Nothing
			End Function
		End Class

		Private Sub stopListeningForOtherDrags()
			'System.out.println("Removing AWTEventListener");
			java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper2(Of T)
		   )
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
			Implements PrivilegedAction(Of T)

			Public Overridable Function run() As Object
				outerInstance.nativeContainer.toolkit.removeAWTEventListener(LightweightDispatcher.this)
				Return Nothing
			End Function
		End Class

	'    
	'     * (Implementation of AWTEventListener)
	'     * Listen for drag events posted in other hw components so we can
	'     * track enter/exit regardless of where a drag originated
	'     
		Public Overridable Sub eventDispatched(ByVal e As AWTEvent)
			Dim isForeignDrag As Boolean = (TypeOf e Is MouseEvent) AndAlso Not(TypeOf e Is sun.awt.dnd.SunDropTargetEvent) AndAlso (e.id = MouseEvent.MOUSE_DRAGGED) AndAlso (e.source IsNot nativeContainer)

			If Not isForeignDrag Then Return

			Dim srcEvent As MouseEvent = CType(e, MouseEvent)
			Dim [me] As MouseEvent

			SyncLock nativeContainer.treeLock
				Dim srcComponent As Component = srcEvent.component

				' component may have disappeared since drag event posted
				' (i.e. Swing hierarchical menus)
				If Not srcComponent.showing Then Return

				' see 5083555
				' check if srcComponent is in any modal blocked window
				Dim c As Component = nativeContainer
				Do While (c IsNot Nothing) AndAlso Not(TypeOf c Is Window)
					c = c.parent_NoClientCode
				Loop
				If (c Is Nothing) OrElse CType(c, Window).modalBlocked Then Return

				'
				' create an internal 'dragged-over' event indicating
				' we are being dragged over from another hw component
				'
				[me] = New MouseEvent(nativeContainer, LWD_MOUSE_DRAGGED_OVER, srcEvent.when, srcEvent.modifiersEx Or srcEvent.modifiers, srcEvent.x, srcEvent.y, srcEvent.xOnScreen, srcEvent.yOnScreen, srcEvent.clickCount, srcEvent.popupTrigger, srcEvent.button)
				CType(srcEvent, AWTEvent).copyPrivateDataInto([me])
				' translate coordinates to this native container
				Dim ptSrcOrigin As Point = srcComponent.locationOnScreen

				If sun.awt.AppContext.appContext IsNot nativeContainer.appContext Then
					Dim mouseEvent_Renamed As MouseEvent = [me]
					Dim r As Runnable = New RunnableAnonymousInnerClassHelper
					sun.awt.SunToolkit.executeOnEventHandlerThread(nativeContainer, r)
					Return
				Else
					If Not nativeContainer.showing Then Return

					Dim ptDstOrigin As Point = nativeContainer.locationOnScreen
					[me].translatePoint(ptSrcOrigin.x - ptDstOrigin.x, ptSrcOrigin.y - ptDstOrigin.y)
				End If
			End SyncLock
			'System.out.println("Track event: " + me);
			' feed the 'dragged-over' event directly to the enter/exit
			' code (not a real event so don't pass it to dispatchEvent)
			Dim targetOver As Component = nativeContainer.getMouseEventTarget([me].x, [me].y, Container.INCLUDE_SELF)
			trackMouseEnterExit(targetOver, [me])
		End Sub

		Private Class RunnableAnonymousInnerClassHelper
			Implements Runnable

			Public Overridable Sub run() Implements Runnable.run
				If Not outerInstance.nativeContainer.showing Then Return

				Dim ptDstOrigin As Point = outerInstance.nativeContainer.locationOnScreen
				mouseEvent.translatePoint(ptSrcOrigin.x - ptDstOrigin.x, ptSrcOrigin.y - ptDstOrigin.y)
				Dim targetOver As Component = outerInstance.nativeContainer.getMouseEventTarget(mouseEvent.x, mouseEvent.y, Container.INCLUDE_SELF)
				outerInstance.trackMouseEnterExit(targetOver, mouseEvent)
			End Sub
		End Class

		''' <summary>
		''' Sends a mouse event to the current mouse event recipient using
		''' the given event (sent to the windowed host) as a srcEvent.  If
		''' the mouse event target is still in the component tree, the
		''' coordinates of the event are translated to those of the target.
		''' If the target has been removed, we don't bother to send the
		''' message.
		''' </summary>
		Friend Overridable Sub retargetMouseEvent(ByVal target As Component, ByVal id As Integer, ByVal e As MouseEvent)
			If target Is Nothing Then Return ' mouse is over another hw component or target is disabled

			Dim x As Integer = e.x, y As Integer = e.y
			Dim component_Renamed As Component

			component_Renamed = target
			Do While component_Renamed IsNot Nothing AndAlso component_Renamed IsNot nativeContainer
				x -= component_Renamed.x
				y -= component_Renamed.y
				component_Renamed = component_Renamed.parent
			Loop
			Dim retargeted As MouseEvent
			If component_Renamed IsNot Nothing Then
				If TypeOf e Is sun.awt.dnd.SunDropTargetEvent Then
					retargeted = New sun.awt.dnd.SunDropTargetEvent(target, id, x, y, CType(e, sun.awt.dnd.SunDropTargetEvent).dispatcher)
				ElseIf id = MouseEvent.MOUSE_WHEEL Then
					retargeted = New MouseWheelEvent(target, id, e.when, e.modifiersEx Or e.modifiers, x, y, e.xOnScreen, e.yOnScreen, e.clickCount, e.popupTrigger, CType(e, MouseWheelEvent).scrollType, CType(e, MouseWheelEvent).scrollAmount, CType(e, MouseWheelEvent).wheelRotation, CType(e, MouseWheelEvent).preciseWheelRotation)
				Else
					retargeted = New MouseEvent(target, id, e.when, e.modifiersEx Or e.modifiers, x, y, e.xOnScreen, e.yOnScreen, e.clickCount, e.popupTrigger, e.button)
				End If

				CType(e, AWTEvent).copyPrivateDataInto(retargeted)

				If target Is nativeContainer Then
					' avoid recursively calling LightweightDispatcher...
					CType(target, Container).dispatchEventToSelf(retargeted)
				Else
					Debug.Assert(sun.awt.AppContext.appContext Is target.appContext)

					If nativeContainer.modalComp IsNot Nothing Then
						If CType(nativeContainer.modalComp, Container).isAncestorOf(target) Then
							target.dispatchEvent(retargeted)
						Else
							e.consume()
						End If
					Else
						target.dispatchEvent(retargeted)
					End If
				End If
				If id = MouseEvent.MOUSE_WHEEL AndAlso retargeted.consumed Then e.consume()
			End If
		End Sub

		' --- member variables -------------------------------

		''' <summary>
		''' The windowed container that might be hosting events for
		''' subcomponents.
		''' </summary>
		Private nativeContainer As Container

		''' <summary>
		''' This variable is not used, but kept for serialization compatibility
		''' </summary>
		Private focus As Component

		''' <summary>
		''' The current subcomponent being hosted by this windowed
		''' component that has events being forwarded to it.  If this
		''' is null, there are currently no events being forwarded to
		''' a subcomponent.
		''' </summary>
		<NonSerialized> _
		Private mouseEventTarget As WeakReference(Of Component)

		''' <summary>
		''' The last component entered by the {@code MouseEvent}.
		''' </summary>
		<NonSerialized> _
		Private targetLastEntered As WeakReference(Of Component)

		''' <summary>
		''' The last component entered by the {@code SunDropTargetEvent}.
		''' </summary>
		<NonSerialized> _
		Private targetLastEnteredDT As WeakReference(Of Component)

		''' <summary>
		''' Is the mouse over the native container.
		''' </summary>
		<NonSerialized> _
		Private isMouseInNativeContainer As Boolean = False

		''' <summary>
		''' Is DnD over the native container.
		''' </summary>
		<NonSerialized> _
		Private isMouseDTInNativeContainer As Boolean = False

		''' <summary>
		''' This variable is not used, but kept for serialization compatibility
		''' </summary>
		Private nativeCursor As Cursor

		''' <summary>
		''' The event mask for contained lightweight components.  Lightweight
		''' components need a windowed container to host window-related
		''' events.  This separate mask indicates events that have been
		''' requested by contained lightweight components without effecting
		''' the mask of the windowed component itself.
		''' </summary>
		Private eventMask As Long

		''' <summary>
		''' The kind of events routed to lightweight components from windowed
		''' hosts.
		''' </summary>
		Private Shared ReadOnly PROXY_EVENT_MASK As Long = AWTEvent.FOCUS_EVENT_MASK Or AWTEvent.KEY_EVENT_MASK Or AWTEvent.MOUSE_EVENT_MASK Or AWTEvent.MOUSE_MOTION_EVENT_MASK Or AWTEvent.MOUSE_WHEEL_EVENT_MASK

		Private Shared ReadOnly MOUSE_MASK As Long = AWTEvent.MOUSE_EVENT_MASK Or AWTEvent.MOUSE_MOTION_EVENT_MASK Or AWTEvent.MOUSE_WHEEL_EVENT_MASK
	End Class

End Namespace
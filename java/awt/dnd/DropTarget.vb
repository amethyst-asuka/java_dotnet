Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices

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

Namespace java.awt.dnd





	''' <summary>
	''' The <code>DropTarget</code> is associated
	''' with a <code>Component</code> when that <code>Component</code>
	''' wishes
	''' to accept drops during Drag and Drop operations.
	''' <P>
	'''  Each
	''' <code>DropTarget</code> is associated with a <code>FlavorMap</code>.
	''' The default <code>FlavorMap</code> hereafter designates the
	''' <code>FlavorMap</code> returned by <code>SystemFlavorMap.getDefaultFlavorMap()</code>.
	''' 
	''' @since 1.2
	''' </summary>

	<Serializable> _
	Public Class DropTarget
		Implements DropTargetListener

		Private Const serialVersionUID As Long = -6283860791671019047L

		''' <summary>
		''' Creates a new DropTarget given the <code>Component</code>
		''' to associate itself with, an <code>int</code> representing
		''' the default acceptable action(s) to
		''' support, a <code>DropTargetListener</code>
		''' to handle event processing, a <code>boolean</code> indicating
		''' if the <code>DropTarget</code> is currently accepting drops, and
		''' a <code>FlavorMap</code> to use (or null for the default <CODE>FlavorMap</CODE>).
		''' <P>
		''' The Component will receive drops only if it is enabled. </summary>
		''' <param name="c">         The <code>Component</code> with which this <code>DropTarget</code> is associated </param>
		''' <param name="ops">       The default acceptable actions for this <code>DropTarget</code> </param>
		''' <param name="dtl">       The <code>DropTargetListener</code> for this <code>DropTarget</code> </param>
		''' <param name="act">       Is the <code>DropTarget</code> accepting drops. </param>
		''' <param name="fm">        The <code>FlavorMap</code> to use, or null for the default <CODE>FlavorMap</CODE> </param>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		'''            returns true </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New(ByVal c As java.awt.Component, ByVal ops As Integer, ByVal dtl As DropTargetListener, ByVal act As Boolean, ByVal fm As java.awt.datatransfer.FlavorMap)
			If java.awt.GraphicsEnvironment.headless Then Throw New java.awt.HeadlessException

			component_Renamed = c

			defaultActions = ops

			If dtl IsNot Nothing Then
				Try
				addDropTargetListener(dtl)
			Catch tmle As java.util.TooManyListenersException
				' do nothing!
			End Try
			End If

			If c IsNot Nothing Then
				c.dropTarget = Me
				active = act
			End If

			If fm IsNot Nothing Then
				flavorMap = fm
			Else
				flavorMap = java.awt.datatransfer.SystemFlavorMap.defaultFlavorMap
			End If
		End Sub

		''' <summary>
		''' Creates a <code>DropTarget</code> given the <code>Component</code>
		''' to associate itself with, an <code>int</code> representing
		''' the default acceptable action(s)
		''' to support, a <code>DropTargetListener</code>
		''' to handle event processing, and a <code>boolean</code> indicating
		''' if the <code>DropTarget</code> is currently accepting drops.
		''' <P>
		''' The Component will receive drops only if it is enabled. </summary>
		''' <param name="c">         The <code>Component</code> with which this <code>DropTarget</code> is associated </param>
		''' <param name="ops">       The default acceptable actions for this <code>DropTarget</code> </param>
		''' <param name="dtl">       The <code>DropTargetListener</code> for this <code>DropTarget</code> </param>
		''' <param name="act">       Is the <code>DropTarget</code> accepting drops. </param>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		'''            returns true </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New(ByVal c As java.awt.Component, ByVal ops As Integer, ByVal dtl As DropTargetListener, ByVal act As Boolean)
			Me.New(c, ops, dtl, act, Nothing)
		End Sub

		''' <summary>
		''' Creates a <code>DropTarget</code>. </summary>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		'''            returns true </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New()
			Me.New(Nothing, DnDConstants.ACTION_COPY_OR_MOVE, Nothing, True, Nothing)
		End Sub

		''' <summary>
		''' Creates a <code>DropTarget</code> given the <code>Component</code>
		''' to associate itself with, and the <code>DropTargetListener</code>
		''' to handle event processing.
		''' <P>
		''' The Component will receive drops only if it is enabled. </summary>
		''' <param name="c">         The <code>Component</code> with which this <code>DropTarget</code> is associated </param>
		''' <param name="dtl">       The <code>DropTargetListener</code> for this <code>DropTarget</code> </param>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		'''            returns true </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New(ByVal c As java.awt.Component, ByVal dtl As DropTargetListener)
			Me.New(c, DnDConstants.ACTION_COPY_OR_MOVE, dtl, True, Nothing)
		End Sub

		''' <summary>
		''' Creates a <code>DropTarget</code> given the <code>Component</code>
		''' to associate itself with, an <code>int</code> representing
		''' the default acceptable action(s) to support, and a
		''' <code>DropTargetListener</code> to handle event processing.
		''' <P>
		''' The Component will receive drops only if it is enabled. </summary>
		''' <param name="c">         The <code>Component</code> with which this <code>DropTarget</code> is associated </param>
		''' <param name="ops">       The default acceptable actions for this <code>DropTarget</code> </param>
		''' <param name="dtl">       The <code>DropTargetListener</code> for this <code>DropTarget</code> </param>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		'''            returns true </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New(ByVal c As java.awt.Component, ByVal ops As Integer, ByVal dtl As DropTargetListener)
			Me.New(c, ops, dtl, True)
		End Sub

		''' <summary>
		''' Note: this interface is required to permit the safe association
		''' of a DropTarget with a Component in one of two ways, either:
		''' <code> component.setDropTarget(droptarget); </code>
		''' or <code> droptarget.setComponent(component); </code>
		''' <P>
		''' The Component will receive drops only if it is enabled. </summary>
		''' <param name="c"> The new <code>Component</code> this <code>DropTarget</code>
		''' is to be associated with. </param>

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property component As java.awt.Component
			Set(ByVal c As java.awt.Component)
				If component_Renamed Is c OrElse component_Renamed IsNot Nothing AndAlso component_Renamed.Equals(c) Then Return
    
				Dim old As java.awt.Component
				Dim oldPeer As java.awt.peer.ComponentPeer = Nothing
    
				old = component_Renamed
				If old IsNot Nothing Then
					clearAutoscroll()
    
					component_Renamed = Nothing
    
					If componentPeer IsNot Nothing Then
						oldPeer = componentPeer
						removeNotify(componentPeer)
					End If
    
					old.dropTarget = Nothing
    
				End If
    
				component_Renamed = c
				If component_Renamed IsNot Nothing Then
					Try
					c.dropTarget = Me ' undo the change
				Catch e As Exception
					If old IsNot Nothing Then
						old.dropTarget = Me
						addNotify(oldPeer)
					End If
				End Try
				End If
			End Set
			Get
				Return component_Renamed
			End Get
		End Property



		''' <summary>
		''' Sets the default acceptable actions for this <code>DropTarget</code>
		''' <P> </summary>
		''' <param name="ops"> the default actions </param>
		''' <seealso cref= java.awt.dnd.DnDConstants </seealso>

		Public Overridable Property defaultActions As Integer
			Set(ByVal ops As Integer)
				dropTargetContext.targetActions = ops And (DnDConstants.ACTION_COPY_OR_MOVE Or DnDConstants.ACTION_REFERENCE)
			End Set
			Get
				Return actions
			End Get
		End Property

	'    
	'     * Called by DropTargetContext.setTargetActions()
	'     * with appropriate synchronization.
	'     
		Friend Overridable Sub doSetDefaultActions(ByVal ops As Integer)
			actions = ops
		End Sub



		''' <summary>
		''' Sets the DropTarget active if <code>true</code>,
		''' inactive if <code>false</code>.
		''' <P> </summary>
		''' <param name="isActive"> sets the <code>DropTarget</code> (in)active. </param>

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property active As Boolean
			Set(ByVal isActive As Boolean)
				If isActive <> active Then active = isActive
    
				If Not active Then clearAutoscroll()
			End Set
			Get
				Return active
			End Get
		End Property



		''' <summary>
		''' Adds a new <code>DropTargetListener</code> (UNICAST SOURCE).
		''' <P> </summary>
		''' <param name="dtl"> The new <code>DropTargetListener</code>
		''' <P> </param>
		''' <exception cref="TooManyListenersException"> if a
		''' <code>DropTargetListener</code> is already added to this
		''' <code>DropTarget</code>. </exception>

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addDropTargetListener(ByVal dtl As DropTargetListener)
			If dtl Is Nothing Then Return

			If Equals(dtl) Then Throw New IllegalArgumentException("DropTarget may not be its own Listener")

			If dtListener Is Nothing Then
				dtListener = dtl
			Else
				Throw New java.util.TooManyListenersException
			End If
		End Sub

		''' <summary>
		''' Removes the current <code>DropTargetListener</code> (UNICAST SOURCE).
		''' <P> </summary>
		''' <param name="dtl"> the DropTargetListener to deregister. </param>

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeDropTargetListener(ByVal dtl As DropTargetListener)
			If dtl IsNot Nothing AndAlso dtListener IsNot Nothing Then
				If dtListener.Equals(dtl) Then
					dtListener = Nothing
				Else
					Throw New IllegalArgumentException("listener mismatch")
				End If
			End If
		End Sub

		''' <summary>
		''' Calls <code>dragEnter</code> on the registered
		''' <code>DropTargetListener</code> and passes it
		''' the specified <code>DropTargetDragEvent</code>.
		''' Has no effect if this <code>DropTarget</code>
		''' is not active.
		''' </summary>
		''' <param name="dtde"> the <code>DropTargetDragEvent</code>
		''' </param>
		''' <exception cref="NullPointerException"> if this <code>DropTarget</code>
		'''         is active and <code>dtde</code> is <code>null</code>
		''' </exception>
		''' <seealso cref= #isActive </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub dragEnter(ByVal dtde As DropTargetDragEvent)
			isDraggingInside = True

			If Not active Then Return

			If dtListener IsNot Nothing Then
				dtListener.dragEnter(dtde)
			Else
				dtde.dropTargetContext.targetActions = DnDConstants.ACTION_NONE
			End If

			initializeAutoscrolling(dtde.location)
		End Sub

		''' <summary>
		''' Calls <code>dragOver</code> on the registered
		''' <code>DropTargetListener</code> and passes it
		''' the specified <code>DropTargetDragEvent</code>.
		''' Has no effect if this <code>DropTarget</code>
		''' is not active.
		''' </summary>
		''' <param name="dtde"> the <code>DropTargetDragEvent</code>
		''' </param>
		''' <exception cref="NullPointerException"> if this <code>DropTarget</code>
		'''         is active and <code>dtde</code> is <code>null</code>
		''' </exception>
		''' <seealso cref= #isActive </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub dragOver(ByVal dtde As DropTargetDragEvent)
			If Not active Then Return

			If dtListener IsNot Nothing AndAlso active Then dtListener.dragOver(dtde)

			updateAutoscroll(dtde.location)
		End Sub

		''' <summary>
		''' Calls <code>dropActionChanged</code> on the registered
		''' <code>DropTargetListener</code> and passes it
		''' the specified <code>DropTargetDragEvent</code>.
		''' Has no effect if this <code>DropTarget</code>
		''' is not active.
		''' </summary>
		''' <param name="dtde"> the <code>DropTargetDragEvent</code>
		''' </param>
		''' <exception cref="NullPointerException"> if this <code>DropTarget</code>
		'''         is active and <code>dtde</code> is <code>null</code>
		''' </exception>
		''' <seealso cref= #isActive </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub dropActionChanged(ByVal dtde As DropTargetDragEvent)
			If Not active Then Return

			If dtListener IsNot Nothing Then dtListener.dropActionChanged(dtde)

			updateAutoscroll(dtde.location)
		End Sub

		''' <summary>
		''' Calls <code>dragExit</code> on the registered
		''' <code>DropTargetListener</code> and passes it
		''' the specified <code>DropTargetEvent</code>.
		''' Has no effect if this <code>DropTarget</code>
		''' is not active.
		''' <p>
		''' This method itself does not throw any exception
		''' for null parameter but for exceptions thrown by
		''' the respective method of the listener.
		''' </summary>
		''' <param name="dte"> the <code>DropTargetEvent</code>
		''' </param>
		''' <seealso cref= #isActive </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub dragExit(ByVal dte As DropTargetEvent) Implements DropTargetListener.dragExit
			isDraggingInside = False

			If Not active Then Return

			If dtListener IsNot Nothing AndAlso active Then dtListener.dragExit(dte)

			clearAutoscroll()
		End Sub

		''' <summary>
		''' Calls <code>drop</code> on the registered
		''' <code>DropTargetListener</code> and passes it
		''' the specified <code>DropTargetDropEvent</code>
		''' if this <code>DropTarget</code> is active.
		''' </summary>
		''' <param name="dtde"> the <code>DropTargetDropEvent</code>
		''' </param>
		''' <exception cref="NullPointerException"> if <code>dtde</code> is null
		'''         and at least one of the following is true: this
		'''         <code>DropTarget</code> is not active, or there is
		'''         no a <code>DropTargetListener</code> registered.
		''' </exception>
		''' <seealso cref= #isActive </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub drop(ByVal dtde As DropTargetDropEvent)
			isDraggingInside = False

			clearAutoscroll()

			If dtListener IsNot Nothing AndAlso active Then
				dtListener.drop(dtde)
			Else ' we should'nt get here ...
				dtde.rejectDrop()
			End If
		End Sub

		''' <summary>
		''' Gets the <code>FlavorMap</code>
		''' associated with this <code>DropTarget</code>.
		''' If no <code>FlavorMap</code> has been set for this
		''' <code>DropTarget</code>, it is associated with the default
		''' <code>FlavorMap</code>.
		''' <P> </summary>
		''' <returns> the FlavorMap for this DropTarget </returns>

		Public Overridable Property flavorMap As java.awt.datatransfer.FlavorMap
			Get
				Return flavorMap
			End Get
			Set(ByVal fm As java.awt.datatransfer.FlavorMap)
				flavorMap = If(fm Is Nothing, java.awt.datatransfer.SystemFlavorMap.defaultFlavorMap, fm)
			End Set
		End Property



		''' <summary>
		''' Notify the DropTarget that it has been associated with a Component
		''' 
		''' **********************************************************************
		''' This method is usually called from java.awt.Component.addNotify() of
		''' the Component associated with this DropTarget to notify the DropTarget
		''' that a ComponentPeer has been associated with that Component.
		''' 
		''' Calling this method, other than to notify this DropTarget of the
		''' association of the ComponentPeer with the Component may result in
		''' a malfunction of the DnD system.
		''' **********************************************************************
		''' <P> </summary>
		''' <param name="peer"> The Peer of the Component we are associated with!
		'''  </param>

		Public Overridable Sub addNotify(ByVal peer As java.awt.peer.ComponentPeer)
			If peer Is componentPeer Then Return

			componentPeer = peer

			Dim c As java.awt.Component = component_Renamed
			Do While c IsNot Nothing AndAlso TypeOf peer Is java.awt.peer.LightweightPeer
				peer = c.peer
				c = c.parent
			Loop

			If TypeOf peer Is java.awt.dnd.peer.DropTargetPeer Then
				nativePeer = peer
				CType(peer, java.awt.dnd.peer.DropTargetPeer).addDropTarget(Me)
			Else
				nativePeer = Nothing
			End If
		End Sub

		''' <summary>
		''' Notify the DropTarget that it has been disassociated from a Component
		''' 
		''' **********************************************************************
		''' This method is usually called from java.awt.Component.removeNotify() of
		''' the Component associated with this DropTarget to notify the DropTarget
		''' that a ComponentPeer has been disassociated with that Component.
		''' 
		''' Calling this method, other than to notify this DropTarget of the
		''' disassociation of the ComponentPeer from the Component may result in
		''' a malfunction of the DnD system.
		''' **********************************************************************
		''' <P> </summary>
		''' <param name="peer"> The Peer of the Component we are being disassociated from! </param>

		Public Overridable Sub removeNotify(ByVal peer As java.awt.peer.ComponentPeer)
			If nativePeer IsNot Nothing Then CType(nativePeer, java.awt.dnd.peer.DropTargetPeer).removeDropTarget(Me)

				nativePeer = Nothing
				componentPeer = nativePeer

			SyncLock Me
				If isDraggingInside Then dragExit(New DropTargetEvent(dropTargetContext))
			End SyncLock
		End Sub

		''' <summary>
		''' Gets the <code>DropTargetContext</code> associated
		''' with this <code>DropTarget</code>.
		''' <P> </summary>
		''' <returns> the <code>DropTargetContext</code> associated with this <code>DropTarget</code>. </returns>

		Public Overridable Property dropTargetContext As DropTargetContext
			Get
				Return dropTargetContext
			End Get
		End Property

		''' <summary>
		''' Creates the DropTargetContext associated with this DropTarget.
		''' Subclasses may override this method to instantiate their own
		''' DropTargetContext subclass.
		''' 
		''' This call is typically *only* called by the platform's
		''' DropTargetContextPeer as a drag operation encounters this
		''' DropTarget. Accessing the Context while no Drag is current
		''' has undefined results.
		''' </summary>

		Protected Friend Overridable Function createDropTargetContext() As DropTargetContext
			Return New DropTargetContext(Me)
		End Function

		''' <summary>
		''' Serializes this <code>DropTarget</code>. Performs default serialization,
		''' and then writes out this object's <code>DropTargetListener</code> if and
		''' only if it can be serialized. If not, <code>null</code> is written
		''' instead.
		''' 
		''' @serialData The default serializable fields, in alphabetical order,
		'''             followed by either a <code>DropTargetListener</code>
		'''             instance, or <code>null</code>.
		''' @since 1.4
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()

			s.writeObject(If(SerializationTester.test(dtListener), dtListener, Nothing))
		End Sub

		''' <summary>
		''' Deserializes this <code>DropTarget</code>. This method first performs
		''' default deserialization for all non-<code>transient</code> fields. An
		''' attempt is then made to deserialize this object's
		''' <code>DropTargetListener</code> as well. This is first attempted by
		''' deserializing the field <code>dtListener</code>, because, in releases
		''' prior to 1.4, a non-<code>transient</code> field of this name stored the
		''' <code>DropTargetListener</code>. If this fails, the next object in the
		''' stream is used instead.
		''' 
		''' @since 1.4
		''' </summary>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			Dim f As java.io.ObjectInputStream.GetField = s.readFields()

			Try
				dropTargetContext = CType(f.get("dropTargetContext", Nothing), DropTargetContext)
			Catch e As IllegalArgumentException
				' Pre-1.4 support. 'dropTargetContext' was previously transient
			End Try
			If dropTargetContext Is Nothing Then dropTargetContext = createDropTargetContext()

			component_Renamed = CType(f.get("component", Nothing), java.awt.Component)
			actions = f.get("actions", DnDConstants.ACTION_COPY_OR_MOVE)
			active = f.get("active", True)

			' Pre-1.4 support. 'dtListener' was previously non-transient
			Try
				dtListener = CType(f.get("dtListener", Nothing), DropTargetListener)
			Catch e As IllegalArgumentException
				' 1.4-compatible byte stream. 'dtListener' was written explicitly
				dtListener = CType(s.readObject(), DropTargetListener)
			End Try
		End Sub

		''' <summary>
		'''****************************************************************** </summary>

		''' <summary>
		''' this protected nested class implements autoscrolling
		''' </summary>

		Protected Friend Class DropTargetAutoScroller
			Implements java.awt.event.ActionListener

			''' <summary>
			''' construct a DropTargetAutoScroller
			''' <P> </summary>
			''' <param name="c"> the <code>Component</code> </param>
			''' <param name="p"> the <code>Point</code> </param>

			Protected Friend Sub New(ByVal c As java.awt.Component, ByVal p As java.awt.Point)
				MyBase.New()

				component_Renamed = c
				autoScroll = CType(component_Renamed, Autoscroll)

				Dim t As java.awt.Toolkit = java.awt.Toolkit.defaultToolkit

				Dim initial As Integer? = Convert.ToInt32(100)
				Dim interval As Integer? = Convert.ToInt32(100)

				Try
					initial = CInt(Fix(t.getDesktopProperty("DnD.Autoscroll.initialDelay")))
				Catch e As Exception
					' ignore
				End Try

				Try
					interval = CInt(Fix(t.getDesktopProperty("DnD.Autoscroll.interval")))
				Catch e As Exception
					' ignore
				End Try

				timer = New javax.swing.Timer(interval, Me)

				timer.coalesce = True
				timer.initialDelay = initial

				locn = p
				prev = p

				Try
					hysteresis = CInt(Fix(t.getDesktopProperty("DnD.Autoscroll.cursorHysteresis")))
				Catch e As Exception
					' ignore
				End Try

				timer.start()
			End Sub

			''' <summary>
			''' update the geometry of the autoscroll region
			''' </summary>

			Private Sub updateRegion()
			   Dim i As java.awt.Insets = autoScroll.autoscrollInsets
			   Dim size As java.awt.Dimension = component_Renamed.size

			   If size.width <> outer.width OrElse size.height <> outer.height Then outer.reshape(0, 0, size.width, size.height)

			   If inner.x <> i.left OrElse inner.y <> i.top Then inner.locationion(i.left, i.top)

			   Dim newWidth As Integer = size.width - (i.left + i.right)
			   Dim newHeight As Integer = size.height - (i.top + i.bottom)

			   If newWidth <> inner.width OrElse newHeight <> inner.height Then inner.sizeize(newWidth, newHeight)

			End Sub

			''' <summary>
			''' cause autoscroll to occur
			''' <P> </summary>
			''' <param name="newLocn"> the <code>Point</code> </param>

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Protected Friend Overridable Sub updateLocation(ByVal newLocn As java.awt.Point)
				prev = locn
				locn = newLocn

				If System.Math.Abs(locn.x - prev.x) > hysteresis OrElse System.Math.Abs(locn.y - prev.y) > hysteresis Then
					If timer.running Then timer.stop()
				Else
					If Not timer.running Then timer.start()
				End If
			End Sub

			''' <summary>
			''' cause autoscrolling to stop
			''' </summary>

			Protected Friend Overridable Sub [stop]()
				timer.stop()
			End Sub

			''' <summary>
			''' cause autoscroll to occur
			''' <P> </summary>
			''' <param name="e"> the <code>ActionEvent</code> </param>

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				updateRegion()

				If outer.contains(locn) AndAlso (Not inner.contains(locn)) Then autoScroll.autoscroll(locn)
			End Sub

	'        
	'         * fields
	'         

			Private component_Renamed As java.awt.Component
			Private autoScroll As Autoscroll

			Private timer As javax.swing.Timer

			Private locn As java.awt.Point
			Private prev As java.awt.Point

			Private outer As New java.awt.Rectangle
			Private inner As New java.awt.Rectangle

			Private hysteresis As Integer = 10
		End Class

		''' <summary>
		'''****************************************************************** </summary>

		''' <summary>
		''' create an embedded autoscroller
		''' <P> </summary>
		''' <param name="c"> the <code>Component</code> </param>
		''' <param name="p"> the <code>Point</code> </param>

		Protected Friend Overridable Function createDropTargetAutoScroller(ByVal c As java.awt.Component, ByVal p As java.awt.Point) As DropTargetAutoScroller
			Return New DropTargetAutoScroller(c, p)
		End Function

		''' <summary>
		''' initialize autoscrolling
		''' <P> </summary>
		''' <param name="p"> the <code>Point</code> </param>

		Protected Friend Overridable Sub initializeAutoscrolling(ByVal p As java.awt.Point)
			If component_Renamed Is Nothing OrElse Not(TypeOf component_Renamed Is Autoscroll) Then Return

			autoScroller = createDropTargetAutoScroller(component_Renamed, p)
		End Sub

		''' <summary>
		''' update autoscrolling with current cursor location
		''' <P> </summary>
		''' <param name="dragCursorLocn"> the <code>Point</code> </param>

		Protected Friend Overridable Sub updateAutoscroll(ByVal dragCursorLocn As java.awt.Point)
			If autoScroller IsNot Nothing Then autoScroller.updateLocation(dragCursorLocn)
		End Sub

		''' <summary>
		''' clear autoscrolling
		''' </summary>

		Protected Friend Overridable Sub clearAutoscroll()
			If autoScroller IsNot Nothing Then
				autoScroller.stop()
				autoScroller = Nothing
			End If
		End Sub

		''' <summary>
		''' The DropTargetContext associated with this DropTarget.
		''' 
		''' @serial
		''' </summary>
		Private dropTargetContext As DropTargetContext = createDropTargetContext()

		''' <summary>
		''' The Component associated with this DropTarget.
		''' 
		''' @serial
		''' </summary>
		Private component_Renamed As java.awt.Component

	'    
	'     * That Component's  Peer
	'     
		<NonSerialized> _
		Private componentPeer As java.awt.peer.ComponentPeer

	'    
	'     * That Component's "native" Peer
	'     
		<NonSerialized> _
		Private nativePeer As java.awt.peer.ComponentPeer


		''' <summary>
		''' Default permissible actions supported by this DropTarget.
		''' </summary>
		''' <seealso cref= #setDefaultActions </seealso>
		''' <seealso cref= #getDefaultActions
		''' @serial </seealso>
		Friend actions As Integer = DnDConstants.ACTION_COPY_OR_MOVE

		''' <summary>
		''' <code>true</code> if the DropTarget is accepting Drag &amp; Drop operations.
		''' 
		''' @serial
		''' </summary>
		Friend active As Boolean = True

	'    
	'     * the auto scrolling object
	'     

		<NonSerialized> _
		Private autoScroller As DropTargetAutoScroller

	'    
	'     * The delegate
	'     

		<NonSerialized> _
		Private dtListener As DropTargetListener

	'    
	'     * The FlavorMap
	'     

		<NonSerialized> _
		Private flavorMap As java.awt.datatransfer.FlavorMap

	'    
	'     * If the dragging is currently inside this drop target
	'     
		<NonSerialized> _
		Private isDraggingInside As Boolean
	End Class

End Namespace
Imports Microsoft.VisualBasic
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

Namespace java.awt.dnd



	''' <summary>
	''' The <code>DragSource</code> is the entity responsible
	''' for the initiation of the Drag
	''' and Drop operation, and may be used in a number of scenarios:
	''' <UL>
	''' <LI>1 default instance per JVM for the lifetime of that JVM.
	''' <LI>1 instance per class of potential Drag Initiator object (e.g
	''' TextField). [implementation dependent]
	''' <LI>1 per instance of a particular
	''' <code>Component</code>, or application specific
	''' object associated with a <code>Component</code>
	''' instance in the GUI. [implementation dependent]
	''' <LI>Some other arbitrary association. [implementation dependent]
	''' </UL>
	''' 
	''' Once the <code>DragSource</code> is
	''' obtained, a <code>DragGestureRecognizer</code> should
	''' also be obtained to associate the <code>DragSource</code>
	''' with a particular
	''' <code>Component</code>.
	''' <P>
	''' The initial interpretation of the user's gesture,
	''' and the subsequent starting of the drag operation
	''' are the responsibility of the implementing
	''' <code>Component</code>, which is usually
	''' implemented by a <code>DragGestureRecognizer</code>.
	''' <P>
	''' When a drag gesture occurs, the
	''' <code>DragSource</code>'s
	''' startDrag() method shall be
	''' invoked in order to cause processing
	''' of the user's navigational
	''' gestures and delivery of Drag and Drop
	''' protocol notifications. A
	''' <code>DragSource</code> shall only
	''' permit a single Drag and Drop operation to be
	''' current at any one time, and shall
	''' reject any further startDrag() requests
	''' by throwing an <code>IllegalDnDOperationException</code>
	''' until such time as the extant operation is complete.
	''' <P>
	''' The startDrag() method invokes the
	''' createDragSourceContext() method to
	''' instantiate an appropriate
	''' <code>DragSourceContext</code>
	''' and associate the <code>DragSourceContextPeer</code>
	''' with that.
	''' <P>
	''' If the Drag and Drop System is
	''' unable to initiate a drag operation for
	''' some reason, the startDrag() method throws
	''' a <code>java.awt.dnd.InvalidDnDOperationException</code>
	''' to signal such a condition. Typically this
	''' exception is thrown when the underlying platform
	''' system is either not in a state to
	''' initiate a drag, or the parameters specified are invalid.
	''' <P>
	''' Note that during the drag, the
	''' set of operations exposed by the source
	''' at the start of the drag operation may not change
	''' until the operation is complete.
	''' The operation(s) are constant for the
	''' duration of the operation with respect to the
	''' <code>DragSource</code>.
	''' 
	''' @since 1.2
	''' </summary>

	<Serializable> _
	Public Class DragSource

		Private Const serialVersionUID As Long = 6236096958971414066L

	'    
	'     * load a system default cursor
	'     

		Private Shared Function load(  name As String) As java.awt.Cursor
			If java.awt.GraphicsEnvironment.headless Then Return Nothing

			Try
				Return CType(java.awt.Toolkit.defaultToolkit.getDesktopProperty(name), java.awt.Cursor)
			Catch e As Exception
				e.printStackTrace()

				Throw New RuntimeException("failed to load system cursor: " & name & " : " & e.message)
			End Try
		End Function


		''' <summary>
		''' The default <code>Cursor</code> to use with a copy operation indicating
		''' that a drop is currently allowed. <code>null</code> if
		''' <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>.
		''' </summary>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Shared ReadOnly DefaultCopyDrop As java.awt.Cursor = load("DnD.Cursor.CopyDrop")

		''' <summary>
		''' The default <code>Cursor</code> to use with a move operation indicating
		''' that a drop is currently allowed. <code>null</code> if
		''' <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>.
		''' </summary>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Shared ReadOnly DefaultMoveDrop As java.awt.Cursor = load("DnD.Cursor.MoveDrop")

		''' <summary>
		''' The default <code>Cursor</code> to use with a link operation indicating
		''' that a drop is currently allowed. <code>null</code> if
		''' <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>.
		''' </summary>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Shared ReadOnly DefaultLinkDrop As java.awt.Cursor = load("DnD.Cursor.LinkDrop")

		''' <summary>
		''' The default <code>Cursor</code> to use with a copy operation indicating
		''' that a drop is currently not allowed. <code>null</code> if
		''' <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>.
		''' </summary>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Shared ReadOnly DefaultCopyNoDrop As java.awt.Cursor = load("DnD.Cursor.CopyNoDrop")

		''' <summary>
		''' The default <code>Cursor</code> to use with a move operation indicating
		''' that a drop is currently not allowed. <code>null</code> if
		''' <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>.
		''' </summary>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Shared ReadOnly DefaultMoveNoDrop As java.awt.Cursor = load("DnD.Cursor.MoveNoDrop")

		''' <summary>
		''' The default <code>Cursor</code> to use with a link operation indicating
		''' that a drop is currently not allowed. <code>null</code> if
		''' <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>.
		''' </summary>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Shared ReadOnly DefaultLinkNoDrop As java.awt.Cursor = load("DnD.Cursor.LinkNoDrop")

		Private Shared ReadOnly dflt As DragSource = If(java.awt.GraphicsEnvironment.headless, Nothing, New DragSource)

		''' <summary>
		''' Internal constants for serialization.
		''' </summary>
		Friend Const dragSourceListenerK As String = "dragSourceL"
		Friend Const dragSourceMotionListenerK As String = "dragSourceMotionL"

        ''' <summary>
        ''' Gets the <code>DragSource</code> object associated with
        ''' the underlying platform.
        ''' </summary>
        ''' <returns> the platform DragSource </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        '''            returns true </exception>
        ''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
        Public Shared ReadOnly Property defaultDragSource As DragSource
            Get
                If java.awt.GraphicsEnvironment.headless Then
                    Throw New java.awt.HeadlessException
                Else
                    Return dflt
                End If
            End Get
        End Property

        ''' <summary>
        ''' Reports
        ''' whether or not drag
        ''' <code>Image</code> support
        ''' is available on the underlying platform.
        ''' <P> </summary>
        ''' <returns> if the Drag Image support is available on this platform </returns>

        Public Shared ReadOnly Property dragImageSupported As Boolean
            Get
                Dim t As java.awt.Toolkit = java.awt.Toolkit.defaultToolkit

                Dim supported As Boolean?

                Try
                    supported = CBool(java.awt.Toolkit.defaultToolkit.getDesktopProperty("DnD.isDragImageSupported"))

                    Return supported
                Catch e As Exception
                    Return False
                End Try
            End Get
        End Property

        ''' <summary>
        ''' Creates a new <code>DragSource</code>.
        ''' </summary>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        '''            returns true </exception>
        ''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
        Public Sub New()
			If java.awt.GraphicsEnvironment.headless Then Throw New java.awt.HeadlessException
		End Sub

		''' <summary>
		''' Start a drag, given the <code>DragGestureEvent</code>
		''' that initiated the drag, the initial
		''' <code>Cursor</code> to use,
		''' the <code>Image</code> to drag,
		''' the offset of the <code>Image</code> origin
		''' from the hotspot of the <code>Cursor</code> at
		''' the instant of the trigger,
		''' the <code>Transferable</code> subject data
		''' of the drag, the <code>DragSourceListener</code>,
		''' and the <code>FlavorMap</code>.
		''' <P> </summary>
		''' <param name="trigger">        the <code>DragGestureEvent</code> that initiated the drag </param>
		''' <param name="dragCursor">     the initial {@code Cursor} for this drag operation
		'''                       or {@code null} for the default cursor handling;
		'''                       see <a href="DragSourceContext.html#defaultCursor">DragSourceContext</a>
		'''                       for more details on the cursor handling mechanism during drag and drop </param>
		''' <param name="dragImage">      the image to drag or {@code null} </param>
		''' <param name="imageOffset">    the offset of the <code>Image</code> origin from the hotspot
		'''                       of the <code>Cursor</code> at the instant of the trigger </param>
		''' <param name="transferable">   the subject data of the drag </param>
		''' <param name="dsl">            the <code>DragSourceListener</code> </param>
		''' <param name="flavorMap">      the <code>FlavorMap</code> to use, or <code>null</code>
		''' <P> </param>
		''' <exception cref="java.awt.dnd.InvalidDnDOperationException">
		'''    if the Drag and Drop
		'''    system is unable to initiate a drag operation, or if the user
		'''    attempts to start a drag while an existing drag operation
		'''    is still executing </exception>

		Public Overridable Sub startDrag(  trigger As DragGestureEvent,   dragCursor As java.awt.Cursor,   dragImage As java.awt.Image,   imageOffset As java.awt.Point,   transferable As java.awt.datatransfer.Transferable,   dsl As DragSourceListener,   flavorMap As java.awt.datatransfer.FlavorMap)

			sun.awt.dnd.SunDragSourceContextPeer.dragDropInProgress = True

			Try
				If flavorMap IsNot Nothing Then Me.flavorMap = flavorMap

				Dim dscp As java.awt.dnd.peer.DragSourceContextPeer = java.awt.Toolkit.defaultToolkit.createDragSourceContextPeer(trigger)

				Dim dsc As DragSourceContext = createDragSourceContext(dscp, trigger, dragCursor, dragImage, imageOffset, transferable, dsl)

				If dsc Is Nothing Then Throw New InvalidDnDOperationException

				dscp.startDrag(dsc, dsc.cursor, dragImage, imageOffset) ' may throw
			Catch e As RuntimeException
				sun.awt.dnd.SunDragSourceContextPeer.dragDropInProgress = False
				Throw e
			End Try
		End Sub

		''' <summary>
		''' Start a drag, given the <code>DragGestureEvent</code>
		''' that initiated the drag, the initial
		''' <code>Cursor</code> to use,
		''' the <code>Transferable</code> subject data
		''' of the drag, the <code>DragSourceListener</code>,
		''' and the <code>FlavorMap</code>.
		''' <P> </summary>
		''' <param name="trigger">        the <code>DragGestureEvent</code> that
		''' initiated the drag </param>
		''' <param name="dragCursor">     the initial {@code Cursor} for this drag operation
		'''                       or {@code null} for the default cursor handling;
		'''                       see <a href="DragSourceContext.html#defaultCursor">DragSourceContext</a>
		'''                       for more details on the cursor handling mechanism during drag and drop </param>
		''' <param name="transferable">   the subject data of the drag </param>
		''' <param name="dsl">            the <code>DragSourceListener</code> </param>
		''' <param name="flavorMap">      the <code>FlavorMap</code> to use or <code>null</code>
		''' <P> </param>
		''' <exception cref="java.awt.dnd.InvalidDnDOperationException">
		'''    if the Drag and Drop
		'''    system is unable to initiate a drag operation, or if the user
		'''    attempts to start a drag while an existing drag operation
		'''    is still executing </exception>

		Public Overridable Sub startDrag(  trigger As DragGestureEvent,   dragCursor As java.awt.Cursor,   transferable As java.awt.datatransfer.Transferable,   dsl As DragSourceListener,   flavorMap As java.awt.datatransfer.FlavorMap)
			startDrag(trigger, dragCursor, Nothing, Nothing, transferable, dsl, flavorMap)
		End Sub

		''' <summary>
		''' Start a drag, given the <code>DragGestureEvent</code>
		''' that initiated the drag, the initial <code>Cursor</code>
		''' to use,
		''' the <code>Image</code> to drag,
		''' the offset of the <code>Image</code> origin
		''' from the hotspot of the <code>Cursor</code>
		''' at the instant of the trigger,
		''' the subject data of the drag, and
		''' the <code>DragSourceListener</code>.
		''' <P> </summary>
		''' <param name="trigger">           the <code>DragGestureEvent</code> that initiated the drag </param>
		''' <param name="dragCursor">     the initial {@code Cursor} for this drag operation
		'''                       or {@code null} for the default cursor handling;
		'''                       see <a href="DragSourceContext.html#defaultCursor">DragSourceContext</a>
		'''                       for more details on the cursor handling mechanism during drag and drop </param>
		''' <param name="dragImage">         the <code>Image</code> to drag or <code>null</code> </param>
		''' <param name="dragOffset">        the offset of the <code>Image</code> origin from the hotspot
		'''                          of the <code>Cursor</code> at the instant of the trigger </param>
		''' <param name="transferable">      the subject data of the drag </param>
		''' <param name="dsl">               the <code>DragSourceListener</code>
		''' <P> </param>
		''' <exception cref="java.awt.dnd.InvalidDnDOperationException">
		'''    if the Drag and Drop
		'''    system is unable to initiate a drag operation, or if the user
		'''    attempts to start a drag while an existing drag operation
		'''    is still executing </exception>

		Public Overridable Sub startDrag(  trigger As DragGestureEvent,   dragCursor As java.awt.Cursor,   dragImage As java.awt.Image,   dragOffset As java.awt.Point,   transferable As java.awt.datatransfer.Transferable,   dsl As DragSourceListener)
			startDrag(trigger, dragCursor, dragImage, dragOffset, transferable, dsl, Nothing)
		End Sub

		''' <summary>
		''' Start a drag, given the <code>DragGestureEvent</code>
		''' that initiated the drag, the initial
		''' <code>Cursor</code> to
		''' use,
		''' the <code>Transferable</code> subject data
		''' of the drag, and the <code>DragSourceListener</code>.
		''' <P> </summary>
		''' <param name="trigger">           the <code>DragGestureEvent</code> that initiated the drag </param>
		''' <param name="dragCursor">     the initial {@code Cursor} for this drag operation
		'''                       or {@code null} for the default cursor handling;
		'''                       see <a href="DragSourceContext.html#defaultCursor">DragSourceContext</a> class
		'''                       for more details on the cursor handling mechanism during drag and drop </param>
		''' <param name="transferable">      the subject data of the drag </param>
		''' <param name="dsl">               the <code>DragSourceListener</code>
		''' <P> </param>
		''' <exception cref="java.awt.dnd.InvalidDnDOperationException">
		'''    if the Drag and Drop
		'''    system is unable to initiate a drag operation, or if the user
		'''    attempts to start a drag while an existing drag operation
		'''    is still executing </exception>

		Public Overridable Sub startDrag(  trigger As DragGestureEvent,   dragCursor As java.awt.Cursor,   transferable As java.awt.datatransfer.Transferable,   dsl As DragSourceListener)
			startDrag(trigger, dragCursor, Nothing, Nothing, transferable, dsl, Nothing)
		End Sub

		''' <summary>
		''' Creates the {@code DragSourceContext} to handle the current drag
		''' operation.
		''' <p>
		''' To incorporate a new <code>DragSourceContext</code>
		''' subclass, subclass <code>DragSource</code> and
		''' override this method.
		''' <p>
		''' If <code>dragImage</code> is <code>null</code>, no image is used
		''' to represent the drag over feedback for this drag operation, but
		''' <code>NullPointerException</code> is not thrown.
		''' <p>
		''' If <code>dsl</code> is <code>null</code>, no drag source listener
		''' is registered with the created <code>DragSourceContext</code>,
		''' but <code>NullPointerException</code> is not thrown.
		''' </summary>
		''' <param name="dscp">          The <code>DragSourceContextPeer</code> for this drag </param>
		''' <param name="dgl">           The <code>DragGestureEvent</code> that triggered the
		'''                      drag </param>
		''' <param name="dragCursor">     The initial {@code Cursor} for this drag operation
		'''                       or {@code null} for the default cursor handling;
		'''                       see <a href="DragSourceContext.html#defaultCursor">DragSourceContext</a> class
		'''                       for more details on the cursor handling mechanism during drag and drop </param>
		''' <param name="dragImage">     The <code>Image</code> to drag or <code>null</code> </param>
		''' <param name="imageOffset">   The offset of the <code>Image</code> origin from the
		'''                      hotspot of the cursor at the instant of the trigger </param>
		''' <param name="t">             The subject data of the drag </param>
		''' <param name="dsl">           The <code>DragSourceListener</code>
		''' </param>
		''' <returns> the <code>DragSourceContext</code>
		''' </returns>
		''' <exception cref="NullPointerException"> if <code>dscp</code> is <code>null</code> </exception>
		''' <exception cref="NullPointerException"> if <code>dgl</code> is <code>null</code> </exception>
		''' <exception cref="NullPointerException"> if <code>dragImage</code> is not
		'''    <code>null</code> and <code>imageOffset</code> is <code>null</code> </exception>
		''' <exception cref="NullPointerException"> if <code>t</code> is <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException"> if the <code>Component</code>
		'''         associated with the trigger event is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if the <code>DragSource</code> for the
		'''         trigger event is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if the drag action for the
		'''         trigger event is <code>DnDConstants.ACTION_NONE</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if the source actions for the
		'''         <code>DragGestureRecognizer</code> associated with the trigger
		'''         event are equal to <code>DnDConstants.ACTION_NONE</code>. </exception>

		Protected Friend Overridable Function createDragSourceContext(  dscp As java.awt.dnd.peer.DragSourceContextPeer,   dgl As DragGestureEvent,   dragCursor As java.awt.Cursor,   dragImage As java.awt.Image,   imageOffset As java.awt.Point,   t As java.awt.datatransfer.Transferable,   dsl As DragSourceListener) As DragSourceContext
			Return New DragSourceContext(dscp, dgl, dragCursor, dragImage, imageOffset, t, dsl)
		End Function

		''' <summary>
		''' This method returns the
		''' <code>FlavorMap</code> for this <code>DragSource</code>.
		''' <P> </summary>
		''' <returns> the <code>FlavorMap</code> for this <code>DragSource</code> </returns>

		Public Overridable Property flavorMap As java.awt.datatransfer.FlavorMap

        ''' <summary>
        ''' Creates a new <code>DragGestureRecognizer</code>
        ''' that implements the specified
        ''' abstract subclass of
        ''' <code>DragGestureRecognizer</code>, and
        ''' sets the specified <code>Component</code>
        ''' and <code>DragGestureListener</code> on
        ''' the newly created object.
        ''' <P> </summary>
        ''' <param name="recognizerAbstractClass"> the requested abstract type </param>
        ''' <param name="actions">                 the permitted source drag actions </param>
        ''' <param name="c">                       the <code>Component</code> target </param>
        ''' <param name="dgl">        the <code>DragGestureListener</code> to notify
        ''' <P> </param>
        ''' <returns> the new <code>DragGestureRecognizer</code> or <code>null</code>
        '''    if the <code>Toolkit.createDragGestureRecognizer</code> method
        '''    has no implementation available for
        '''    the requested <code>DragGestureRecognizer</code>
        '''    subclass and returns <code>null</code> </returns>

        Public Overridable Function createDragGestureRecognizer(Of T As DragGestureRecognizer)(  recognizerAbstractClass As [Class],   c As java.awt.Component,   actions As Integer,   dgl As DragGestureListener) As T
			Return java.awt.Toolkit.defaultToolkit.createDragGestureRecognizer(recognizerAbstractClass, Me, c, actions, dgl)
		End Function


		''' <summary>
		''' Creates a new <code>DragGestureRecognizer</code>
		''' that implements the default
		''' abstract subclass of <code>DragGestureRecognizer</code>
		''' for this <code>DragSource</code>,
		''' and sets the specified <code>Component</code>
		''' and <code>DragGestureListener</code> on the
		''' newly created object.
		''' 
		''' For this <code>DragSource</code>
		''' the default is <code>MouseDragGestureRecognizer</code>.
		''' <P> </summary>
		''' <param name="c">       the <code>Component</code> target for the recognizer </param>
		''' <param name="actions"> the permitted source actions </param>
		''' <param name="dgl">     the <code>DragGestureListener</code> to notify
		''' <P> </param>
		''' <returns> the new <code>DragGestureRecognizer</code> or <code>null</code>
		'''    if the <code>Toolkit.createDragGestureRecognizer</code> method
		'''    has no implementation available for
		'''    the requested <code>DragGestureRecognizer</code>
		'''    subclass and returns <code>null</code> </returns>

		Public Overridable Function createDefaultDragGestureRecognizer(  c As java.awt.Component,   actions As Integer,   dgl As DragGestureListener) As DragGestureRecognizer
			Return java.awt.Toolkit.defaultToolkit.createDragGestureRecognizer(GetType(MouseDragGestureRecognizer), Me, c, actions, dgl)
		End Function

		''' <summary>
		''' Adds the specified <code>DragSourceListener</code> to this
		''' <code>DragSource</code> to receive drag source events during drag
		''' operations intiated with this <code>DragSource</code>.
		''' If a <code>null</code> listener is specified, no action is taken and no
		''' exception is thrown.
		''' </summary>
		''' <param name="dsl"> the <code>DragSourceListener</code> to add
		''' </param>
		''' <seealso cref=      #removeDragSourceListener </seealso>
		''' <seealso cref=      #getDragSourceListeners
		''' @since 1.4 </seealso>
		Public Overridable Sub addDragSourceListener(  dsl As DragSourceListener)
			If dsl IsNot Nothing Then
				SyncLock Me
					listener = DnDEventMulticaster.add(listener, dsl)
				End SyncLock
			End If
		End Sub

		''' <summary>
		''' Removes the specified <code>DragSourceListener</code> from this
		''' <code>DragSource</code>.
		''' If a <code>null</code> listener is specified, no action is taken and no
		''' exception is thrown.
		''' If the listener specified by the argument was not previously added to
		''' this <code>DragSource</code>, no action is taken and no exception
		''' is thrown.
		''' </summary>
		''' <param name="dsl"> the <code>DragSourceListener</code> to remove
		''' </param>
		''' <seealso cref=      #addDragSourceListener </seealso>
		''' <seealso cref=      #getDragSourceListeners
		''' @since 1.4 </seealso>
		Public Overridable Sub removeDragSourceListener(  dsl As DragSourceListener)
			If dsl IsNot Nothing Then
				SyncLock Me
					listener = DnDEventMulticaster.remove(listener, dsl)
				End SyncLock
			End If
		End Sub

		''' <summary>
		''' Gets all the <code>DragSourceListener</code>s
		''' registered with this <code>DragSource</code>.
		''' </summary>
		''' <returns> all of this <code>DragSource</code>'s
		'''         <code>DragSourceListener</code>s or an empty array if no
		'''         such listeners are currently registered
		''' </returns>
		''' <seealso cref=      #addDragSourceListener </seealso>
		''' <seealso cref=      #removeDragSourceListener
		''' @since    1.4 </seealso>
		Public Overridable Property dragSourceListeners As DragSourceListener()
			Get
				Return getListeners(GetType(DragSourceListener))
			End Get
		End Property

		''' <summary>
		''' Adds the specified <code>DragSourceMotionListener</code> to this
		''' <code>DragSource</code> to receive drag motion events during drag
		''' operations intiated with this <code>DragSource</code>.
		''' If a <code>null</code> listener is specified, no action is taken and no
		''' exception is thrown.
		''' </summary>
		''' <param name="dsml"> the <code>DragSourceMotionListener</code> to add
		''' </param>
		''' <seealso cref=      #removeDragSourceMotionListener </seealso>
		''' <seealso cref=      #getDragSourceMotionListeners
		''' @since 1.4 </seealso>
		Public Overridable Sub addDragSourceMotionListener(  dsml As DragSourceMotionListener)
			If dsml IsNot Nothing Then
				SyncLock Me
					motionListener = DnDEventMulticaster.add(motionListener, dsml)
				End SyncLock
			End If
		End Sub

		''' <summary>
		''' Removes the specified <code>DragSourceMotionListener</code> from this
		''' <code>DragSource</code>.
		''' If a <code>null</code> listener is specified, no action is taken and no
		''' exception is thrown.
		''' If the listener specified by the argument was not previously added to
		''' this <code>DragSource</code>, no action is taken and no exception
		''' is thrown.
		''' </summary>
		''' <param name="dsml"> the <code>DragSourceMotionListener</code> to remove
		''' </param>
		''' <seealso cref=      #addDragSourceMotionListener </seealso>
		''' <seealso cref=      #getDragSourceMotionListeners
		''' @since 1.4 </seealso>
		Public Overridable Sub removeDragSourceMotionListener(  dsml As DragSourceMotionListener)
			If dsml IsNot Nothing Then
				SyncLock Me
					motionListener = DnDEventMulticaster.remove(motionListener, dsml)
				End SyncLock
			End If
		End Sub

		''' <summary>
		''' Gets all of the  <code>DragSourceMotionListener</code>s
		''' registered with this <code>DragSource</code>.
		''' </summary>
		''' <returns> all of this <code>DragSource</code>'s
		'''         <code>DragSourceMotionListener</code>s or an empty array if no
		'''         such listeners are currently registered
		''' </returns>
		''' <seealso cref=      #addDragSourceMotionListener </seealso>
		''' <seealso cref=      #removeDragSourceMotionListener
		''' @since    1.4 </seealso>
		Public Overridable Property dragSourceMotionListeners As DragSourceMotionListener()
			Get
				Return getListeners(GetType(DragSourceMotionListener))
			End Get
		End Property

		''' <summary>
		''' Gets all the objects currently registered as
		''' <code><em>Foo</em>Listener</code>s upon this <code>DragSource</code>.
		''' <code><em>Foo</em>Listener</code>s are registered using the
		''' <code>add<em>Foo</em>Listener</code> method.
		''' </summary>
		''' <param name="listenerType"> the type of listeners requested; this parameter
		'''          should specify an interface that descends from
		'''          <code>java.util.EventListener</code> </param>
		''' <returns> an array of all objects registered as
		'''          <code><em>Foo</em>Listener</code>s on this
		'''          <code>DragSource</code>, or an empty array if no such listeners
		'''          have been added </returns>
		''' <exception cref="ClassCastException"> if <code>listenerType</code>
		'''          doesn't specify a class or interface that implements
		'''          <code>java.util.EventListener</code>
		''' </exception>
		''' <seealso cref= #getDragSourceListeners </seealso>
		''' <seealso cref= #getDragSourceMotionListeners
		''' @since 1.4 </seealso>
		Public Overridable Function getListeners(Of T As java.util.EventListener)(  listenerType As [Class]) As T()
			Dim l As java.util.EventListener = Nothing
			If listenerType Is GetType(DragSourceListener) Then
				l = listener
			ElseIf listenerType Is GetType(DragSourceMotionListener) Then
				l = motionListener
			End If
			Return DnDEventMulticaster.getListeners(l, listenerType)
		End Function

		''' <summary>
		''' This method calls <code>dragEnter</code> on the
		''' <code>DragSourceListener</code>s registered with this
		''' <code>DragSource</code>, and passes them the specified
		''' <code>DragSourceDragEvent</code>.
		''' </summary>
		''' <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		Friend Overridable Sub processDragEnter(  dsde As DragSourceDragEvent)
			Dim dsl As DragSourceListener = listener
			If dsl IsNot Nothing Then dsl.dragEnter(dsde)
		End Sub

		''' <summary>
		''' This method calls <code>dragOver</code> on the
		''' <code>DragSourceListener</code>s registered with this
		''' <code>DragSource</code>, and passes them the specified
		''' <code>DragSourceDragEvent</code>.
		''' </summary>
		''' <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		Friend Overridable Sub processDragOver(  dsde As DragSourceDragEvent)
			Dim dsl As DragSourceListener = listener
			If dsl IsNot Nothing Then dsl.dragOver(dsde)
		End Sub

		''' <summary>
		''' This method calls <code>dropActionChanged</code> on the
		''' <code>DragSourceListener</code>s registered with this
		''' <code>DragSource</code>, and passes them the specified
		''' <code>DragSourceDragEvent</code>.
		''' </summary>
		''' <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		Friend Overridable Sub processDropActionChanged(  dsde As DragSourceDragEvent)
			Dim dsl As DragSourceListener = listener
			If dsl IsNot Nothing Then dsl.dropActionChanged(dsde)
		End Sub

		''' <summary>
		''' This method calls <code>dragExit</code> on the
		''' <code>DragSourceListener</code>s registered with this
		''' <code>DragSource</code>, and passes them the specified
		''' <code>DragSourceEvent</code>.
		''' </summary>
		''' <param name="dse"> the <code>DragSourceEvent</code> </param>
		Friend Overridable Sub processDragExit(  dse As DragSourceEvent)
			Dim dsl As DragSourceListener = listener
			If dsl IsNot Nothing Then dsl.dragExit(dse)
		End Sub

		''' <summary>
		''' This method calls <code>dragDropEnd</code> on the
		''' <code>DragSourceListener</code>s registered with this
		''' <code>DragSource</code>, and passes them the specified
		''' <code>DragSourceDropEvent</code>.
		''' </summary>
		''' <param name="dsde"> the <code>DragSourceEvent</code> </param>
		Friend Overridable Sub processDragDropEnd(  dsde As DragSourceDropEvent)
			Dim dsl As DragSourceListener = listener
			If dsl IsNot Nothing Then dsl.dragDropEnd(dsde)
		End Sub

		''' <summary>
		''' This method calls <code>dragMouseMoved</code> on the
		''' <code>DragSourceMotionListener</code>s registered with this
		''' <code>DragSource</code>, and passes them the specified
		''' <code>DragSourceDragEvent</code>.
		''' </summary>
		''' <param name="dsde"> the <code>DragSourceEvent</code> </param>
		Friend Overridable Sub processDragMouseMoved(  dsde As DragSourceDragEvent)
			Dim dsml As DragSourceMotionListener = motionListener
			If dsml IsNot Nothing Then dsml.dragMouseMoved(dsde)
		End Sub

		''' <summary>
		''' Serializes this <code>DragSource</code>. This method first performs
		''' default serialization. Next, it writes out this object's
		''' <code>FlavorMap</code> if and only if it can be serialized. If not,
		''' <code>null</code> is written instead. Next, it writes out
		''' <code>Serializable</code> listeners registered with this
		''' object. Listeners are written in a <code>null</code>-terminated sequence
		''' of 0 or more pairs. The pair consists of a <code>String</code> and an
		''' <code>Object</code>; the <code>String</code> indicates the type of the
		''' <code>Object</code> and is one of the following:
		''' <ul>
		''' <li><code>dragSourceListenerK</code> indicating a
		'''     <code>DragSourceListener</code> object;
		''' <li><code>dragSourceMotionListenerK</code> indicating a
		'''     <code>DragSourceMotionListener</code> object.
		''' </ul>
		''' 
		''' @serialData Either a <code>FlavorMap</code> instance, or
		'''      <code>null</code>, followed by a <code>null</code>-terminated
		'''      sequence of 0 or more pairs; the pair consists of a
		'''      <code>String</code> and an <code>Object</code>; the
		'''      <code>String</code> indicates the type of the <code>Object</code>
		'''      and is one of the following:
		'''      <ul>
		'''      <li><code>dragSourceListenerK</code> indicating a
		'''          <code>DragSourceListener</code> object;
		'''      <li><code>dragSourceMotionListenerK</code> indicating a
		'''          <code>DragSourceMotionListener</code> object.
		'''      </ul>.
		''' @since 1.4
		''' </summary>
		Private Sub writeObject(  s As java.io.ObjectOutputStream)
			s.defaultWriteObject()

			s.writeObject(If(SerializationTester.test(flavorMap), flavorMap, Nothing))

			DnDEventMulticaster.save(s, dragSourceListenerK, listener)
			DnDEventMulticaster.save(s, dragSourceMotionListenerK, motionListener)
			s.writeObject(Nothing)
		End Sub

		''' <summary>
		''' Deserializes this <code>DragSource</code>. This method first performs
		''' default deserialization. Next, this object's <code>FlavorMap</code> is
		''' deserialized by using the next object in the stream.
		''' If the resulting <code>FlavorMap</code> is <code>null</code>, this
		''' object's <code>FlavorMap</code> is set to the default FlavorMap for
		''' this thread's <code>ClassLoader</code>.
		''' Next, this object's listeners are deserialized by reading a
		''' <code>null</code>-terminated sequence of 0 or more key/value pairs
		''' from the stream:
		''' <ul>
		''' <li>If a key object is a <code>String</code> equal to
		''' <code>dragSourceListenerK</code>, a <code>DragSourceListener</code> is
		''' deserialized using the corresponding value object and added to this
		''' <code>DragSource</code>.
		''' <li>If a key object is a <code>String</code> equal to
		''' <code>dragSourceMotionListenerK</code>, a
		''' <code>DragSourceMotionListener</code> is deserialized using the
		''' corresponding value object and added to this <code>DragSource</code>.
		''' <li>Otherwise, the key/value pair is skipped.
		''' </ul>
		''' </summary>
		''' <seealso cref= java.awt.datatransfer.SystemFlavorMap#getDefaultFlavorMap
		''' @since 1.4 </seealso>
		Private Sub readObject(  s As java.io.ObjectInputStream)
			s.defaultReadObject()

			' 'flavorMap' was written explicitly
			flavorMap = CType(s.readObject(), java.awt.datatransfer.FlavorMap)

			' Implementation assumes 'flavorMap' is never null.
			If flavorMap Is Nothing Then flavorMap = java.awt.datatransfer.SystemFlavorMap.defaultFlavorMap

			Dim keyOrNull As Object
			keyOrNull = s.readObject()
			Do While Nothing IsNot keyOrNull
				Dim key As String = CStr(keyOrNull).intern()

				If dragSourceListenerK = key Then
					addDragSourceListener(CType(s.readObject(), DragSourceListener))
				ElseIf dragSourceMotionListenerK = key Then
					addDragSourceMotionListener(CType(s.readObject(), DragSourceMotionListener))
				Else
					' skip value for unrecognized key
					s.readObject()
				End If
				keyOrNull = s.readObject()
			Loop
		End Sub

        ''' <summary>
        ''' Returns the drag gesture motion threshold. The drag gesture motion threshold
        ''' defines the recommended behavior for <seealso cref="MouseDragGestureRecognizer"/>s.
        ''' <p>
        ''' If the system property <code>awt.dnd.drag.threshold</code> is set to
        ''' a positive integer, this method returns the value of the system property;
        ''' otherwise if a pertinent desktop property is available and supported by
        ''' the implementation of the Java platform, this method returns the value of
        ''' that property; otherwise this method returns some default value.
        ''' The pertinent desktop property can be queried using
        ''' <code>java.awt.Toolkit.getDesktopProperty("DnD.gestureMotionThreshold")</code>.
        ''' </summary>
        ''' <returns> the drag gesture motion threshold </returns>
        ''' <seealso cref= MouseDragGestureRecognizer
        ''' @since 1.5 </seealso>
        Public Shared ReadOnly Property dragThreshold As Integer
            Get
                Dim ts As Integer = java.security.AccessController.doPrivileged(New sun.security.action.GetIntegerAction("awt.dnd.drag.threshold", 0))
                If ts > 0 Then
                    Return ts
                Else
                    Dim td As Integer? = CInt(Fix(java.awt.Toolkit.defaultToolkit.getDesktopProperty("DnD.gestureMotionThreshold")))
                    If td IsNot Nothing Then Return td
                End If
                Return 5
            End Get
        End Property

        '    
        '     * fields
        '     

        <NonSerialized> _
		Private flavorMap As java.awt.datatransfer.FlavorMap = java.awt.datatransfer.SystemFlavorMap.defaultFlavorMap

		<NonSerialized> _
		Private listener As DragSourceListener

		<NonSerialized> _
		Private motionListener As DragSourceMotionListener
	End Class

End Namespace
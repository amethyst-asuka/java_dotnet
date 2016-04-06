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
	''' The <code>DragSourceContext</code> class is responsible for managing the
	''' initiator side of the Drag and Drop protocol. In particular, it is responsible
	''' for managing drag event notifications to the
	''' <seealso cref="DragSourceListener DragSourceListeners"/>
	''' and <seealso cref="DragSourceMotionListener DragSourceMotionListeners"/>, and providing the
	''' <seealso cref="Transferable"/> representing the source data for the drag operation.
	''' <p>
	''' Note that the <code>DragSourceContext</code> itself
	''' implements the <code>DragSourceListener</code> and
	''' <code>DragSourceMotionListener</code> interfaces.
	''' This is to allow the platform peer
	''' (the <seealso cref="DragSourceContextPeer"/> instance)
	''' created by the <seealso cref="DragSource"/> to notify
	''' the <code>DragSourceContext</code> of
	''' state changes in the ongoing operation. This allows the
	''' <code>DragSourceContext</code> object to interpose
	''' itself between the platform and the
	''' listeners provided by the initiator of the drag operation.
	''' <p>
	''' <a name="defaultCursor"></a>
	''' By default, {@code DragSourceContext} sets the cursor as appropriate
	''' for the current state of the drag and drop operation. For example, if
	''' the user has chosen <seealso cref="DnDConstants#ACTION_MOVE the move action"/>,
	''' and the pointer is over a target that accepts
	''' the move action, the default move cursor is shown. When
	''' the pointer is over an area that does not accept the transfer,
	''' the default "no drop" cursor is shown.
	''' <p>
	''' This default handling mechanism is disabled when a custom cursor is set
	''' by the <seealso cref="#setCursor"/> method. When the default handling is disabled,
	''' it becomes the responsibility
	''' of the developer to keep the cursor up to date, by listening
	''' to the {@code DragSource} events and calling the {@code setCursor()} method.
	''' Alternatively, you can provide custom cursor behavior by providing
	''' custom implementations of the {@code DragSource}
	''' and the {@code DragSourceContext} classes.
	''' </summary>
	''' <seealso cref= DragSourceListener </seealso>
	''' <seealso cref= DragSourceMotionListener </seealso>
	''' <seealso cref= DnDConstants
	''' @since 1.2 </seealso>

	<Serializable> _
	Public Class DragSourceContext
		Implements DragSourceListener, DragSourceMotionListener

		Private Const serialVersionUID As Long = -115407898692194719L

		' used by updateCurrentCursor

		''' <summary>
		''' An <code>int</code> used by updateCurrentCursor()
		''' indicating that the <code>Cursor</code> should change
		''' to the default (no drop) <code>Cursor</code>.
		''' </summary>
		Protected Friend Const [DEFAULT] As Integer = 0

		''' <summary>
		''' An <code>int</code> used by updateCurrentCursor()
		''' indicating that the <code>Cursor</code>
		''' has entered a <code>DropTarget</code>.
		''' </summary>
		Protected Friend Const ENTER As Integer = 1

		''' <summary>
		''' An <code>int</code> used by updateCurrentCursor()
		''' indicating that the <code>Cursor</code> is
		''' over a <code>DropTarget</code>.
		''' </summary>
		Protected Friend Const OVER As Integer = 2

		''' <summary>
		''' An <code>int</code> used by updateCurrentCursor()
		''' indicating that the user operation has changed.
		''' </summary>

		Protected Friend Const CHANGED As Integer = 3

		''' <summary>
		''' Called from <code>DragSource</code>, this constructor creates a new
		''' <code>DragSourceContext</code> given the
		''' <code>DragSourceContextPeer</code> for this Drag, the
		''' <code>DragGestureEvent</code> that triggered the Drag, the initial
		''' <code>Cursor</code> to use for the Drag, an (optional)
		''' <code>Image</code> to display while the Drag is taking place, the offset
		''' of the <code>Image</code> origin from the hotspot at the instant of the
		''' triggering event, the <code>Transferable</code> subject data, and the
		''' <code>DragSourceListener</code> to use during the Drag and Drop
		''' operation.
		''' <br>
		''' If <code>DragSourceContextPeer</code> is <code>null</code>
		''' <code>NullPointerException</code> is thrown.
		''' <br>
		''' If <code>DragGestureEvent</code> is <code>null</code>
		''' <code>NullPointerException</code> is thrown.
		''' <br>
		''' If <code>Cursor</code> is <code>null</code> no exception is thrown and
		''' the default drag cursor behavior is activated for this drag operation.
		''' <br>
		''' If <code>Image</code> is <code>null</code> no exception is thrown.
		''' <br>
		''' If <code>Image</code> is not <code>null</code> and the offset is
		''' <code>null</code> <code>NullPointerException</code> is thrown.
		''' <br>
		''' If <code>Transferable</code> is <code>null</code>
		''' <code>NullPointerException</code> is thrown.
		''' <br>
		''' If <code>DragSourceListener</code> is <code>null</code> no exception
		''' is thrown.
		''' </summary>
		''' <param name="dscp">       the <code>DragSourceContextPeer</code> for this drag </param>
		''' <param name="trigger">    the triggering event </param>
		''' <param name="dragCursor">     the initial {@code Cursor} for this drag operation
		'''                       or {@code null} for the default cursor handling;
		'''                       see <a href="DragSourceContext.html#defaultCursor">class level documentation</a>
		'''                       for more details on the cursor handling mechanism during drag and drop </param>
		''' <param name="dragImage">  the <code>Image</code> to drag (or <code>null</code>) </param>
		''' <param name="offset">     the offset of the image origin from the hotspot at the
		'''                   instant of the triggering event </param>
		''' <param name="t">          the <code>Transferable</code> </param>
		''' <param name="dsl">        the <code>DragSourceListener</code>
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the <code>Component</code> associated
		'''         with the trigger event is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if the <code>DragSource</code> for the
		'''         trigger event is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if the drag action for the
		'''         trigger event is <code>DnDConstants.ACTION_NONE</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if the source actions for the
		'''         <code>DragGestureRecognizer</code> associated with the trigger
		'''         event are equal to <code>DnDConstants.ACTION_NONE</code>. </exception>
		''' <exception cref="NullPointerException"> if dscp, trigger, or t are null, or
		'''         if dragImage is non-null and offset is null </exception>
		Public Sub New(  dscp As java.awt.dnd.peer.DragSourceContextPeer,   trigger As DragGestureEvent,   dragCursor As java.awt.Cursor,   dragImage As java.awt.Image,   offset As java.awt.Point,   t As java.awt.datatransfer.Transferable,   dsl As DragSourceListener)
			If dscp Is Nothing Then Throw New NullPointerException("DragSourceContextPeer")

			If trigger Is Nothing Then Throw New NullPointerException("Trigger")

			If trigger.dragSource Is Nothing Then Throw New IllegalArgumentException("DragSource")

			If trigger.component Is Nothing Then Throw New IllegalArgumentException("Component")

			If trigger.sourceAsDragGestureRecognizer.sourceActions Is DnDConstants.ACTION_NONE Then Throw New IllegalArgumentException("source actions")

			If trigger.dragAction = DnDConstants.ACTION_NONE Then Throw New IllegalArgumentException("no drag action")

			If t Is Nothing Then Throw New NullPointerException("Transferable")

			If dragImage IsNot Nothing AndAlso offset Is Nothing Then Throw New NullPointerException("offset")

			peer = dscp
			Me.trigger = trigger
			cursor_Renamed = dragCursor
			transferable = t
			listener = dsl
			sourceActions = trigger.sourceAsDragGestureRecognizer.sourceActions

			useCustomCursor = (dragCursor IsNot Nothing)

			updateCurrentCursor(trigger.dragAction, sourceActions, [DEFAULT])
		End Sub

		''' <summary>
		''' Returns the <code>DragSource</code>
		''' that instantiated this <code>DragSourceContext</code>.
		''' </summary>
		''' <returns> the <code>DragSource</code> that
		'''   instantiated this <code>DragSourceContext</code> </returns>

		Public Overridable Property dragSource As DragSource
			Get
				Return trigger.dragSource
			End Get
		End Property

		''' <summary>
		''' Returns the <code>Component</code> associated with this
		''' <code>DragSourceContext</code>.
		''' </summary>
		''' <returns> the <code>Component</code> that started the drag </returns>

		Public Overridable Property component As java.awt.Component
			Get
				Return trigger.component
			End Get
		End Property

		''' <summary>
		''' Returns the <code>DragGestureEvent</code>
		''' that initially triggered the drag.
		''' </summary>
		''' <returns> the Event that triggered the drag </returns>

		Public Overridable Property trigger As DragGestureEvent
			Get
				Return trigger
			End Get
		End Property

		''' <summary>
		''' Returns a bitwise mask of <code>DnDConstants</code> that
		''' represent the set of drop actions supported by the drag source for the
		''' drag operation associated with this <code>DragSourceContext</code>.
		''' </summary>
		''' <returns> the drop actions supported by the drag source </returns>
		Public Overridable Property sourceActions As Integer
			Get
				Return sourceActions
			End Get
		End Property

		''' <summary>
		''' Sets the cursor for this drag operation to the specified
		''' <code>Cursor</code>.  If the specified <code>Cursor</code>
		''' is <code>null</code>, the default drag cursor behavior is
		''' activated for this drag operation, otherwise it is deactivated.
		''' </summary>
		''' <param name="c">     the initial {@code Cursor} for this drag operation,
		'''                       or {@code null} for the default cursor handling;
		'''                       see {@link Cursor class
		'''                       level documentation} for more details
		'''                       on the cursor handling during drag and drop
		'''  </param>

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property cursor As java.awt.Cursor
			Set(  c As java.awt.Cursor)
				useCustomCursor = (c IsNot Nothing)
				cursorImpl = c
			End Set
			Get
				Return cursor_Renamed
			End Get
		End Property



		''' <summary>
		''' Add a <code>DragSourceListener</code> to this
		''' <code>DragSourceContext</code> if one has not already been added.
		''' If a <code>DragSourceListener</code> already exists,
		''' this method throws a <code>TooManyListenersException</code>.
		''' <P> </summary>
		''' <param name="dsl"> the <code>DragSourceListener</code> to add.
		''' Note that while <code>null</code> is not prohibited,
		''' it is not acceptable as a parameter.
		''' <P> </param>
		''' <exception cref="TooManyListenersException"> if
		''' a <code>DragSourceListener</code> has already been added </exception>

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addDragSourceListener(  dsl As DragSourceListener)
			If dsl Is Nothing Then Return

			If Equals(dsl) Then Throw New IllegalArgumentException("DragSourceContext may not be its own listener")

			If listener IsNot Nothing Then
				Throw New java.util.TooManyListenersException
			Else
				listener = dsl
			End If
		End Sub

		''' <summary>
		''' Removes the specified <code>DragSourceListener</code>
		''' from  this <code>DragSourceContext</code>.
		''' </summary>
		''' <param name="dsl"> the <code>DragSourceListener</code> to remove;
		'''     note that while <code>null</code> is not prohibited,
		'''     it is not acceptable as a parameter </param>

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeDragSourceListener(  dsl As DragSourceListener)
			If listener IsNot Nothing AndAlso listener.Equals(dsl) Then
				listener = Nothing
			Else
				Throw New IllegalArgumentException
			End If
		End Sub

		''' <summary>
		''' Notifies the peer that the <code>Transferable</code>'s
		''' <code>DataFlavor</code>s have changed.
		''' </summary>

		Public Overridable Sub transferablesFlavorsChanged()
			If peer IsNot Nothing Then peer.transferablesFlavorsChanged()
		End Sub

		''' <summary>
		''' Calls <code>dragEnter</code> on the
		''' <code>DragSourceListener</code>s registered with this
		''' <code>DragSourceContext</code> and with the associated
		''' <code>DragSource</code>, and passes them the specified
		''' <code>DragSourceDragEvent</code>.
		''' </summary>
		''' <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		Public Overridable Sub dragEnter(  dsde As DragSourceDragEvent) Implements DragSourceListener.dragEnter
			Dim dsl As DragSourceListener = listener
			If dsl IsNot Nothing Then dsl.dragEnter(dsde)
			dragSource.processDragEnter(dsde)

			updateCurrentCursor(sourceActions, dsde.targetActions, ENTER)
		End Sub

		''' <summary>
		''' Calls <code>dragOver</code> on the
		''' <code>DragSourceListener</code>s registered with this
		''' <code>DragSourceContext</code> and with the associated
		''' <code>DragSource</code>, and passes them the specified
		''' <code>DragSourceDragEvent</code>.
		''' </summary>
		''' <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		Public Overridable Sub dragOver(  dsde As DragSourceDragEvent) Implements DragSourceListener.dragOver
			Dim dsl As DragSourceListener = listener
			If dsl IsNot Nothing Then dsl.dragOver(dsde)
			dragSource.processDragOver(dsde)

			updateCurrentCursor(sourceActions, dsde.targetActions, OVER)
		End Sub

		''' <summary>
		''' Calls <code>dragExit</code> on the
		''' <code>DragSourceListener</code>s registered with this
		''' <code>DragSourceContext</code> and with the associated
		''' <code>DragSource</code>, and passes them the specified
		''' <code>DragSourceEvent</code>.
		''' </summary>
		''' <param name="dse"> the <code>DragSourceEvent</code> </param>
		Public Overridable Sub dragExit(  dse As DragSourceEvent) Implements DragSourceListener.dragExit
			Dim dsl As DragSourceListener = listener
			If dsl IsNot Nothing Then dsl.dragExit(dse)
			dragSource.processDragExit(dse)

			updateCurrentCursor(DnDConstants.ACTION_NONE, DnDConstants.ACTION_NONE, [DEFAULT])
		End Sub

		''' <summary>
		''' Calls <code>dropActionChanged</code> on the
		''' <code>DragSourceListener</code>s registered with this
		''' <code>DragSourceContext</code> and with the associated
		''' <code>DragSource</code>, and passes them the specified
		''' <code>DragSourceDragEvent</code>.
		''' </summary>
		''' <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		Public Overridable Sub dropActionChanged(  dsde As DragSourceDragEvent) Implements DragSourceListener.dropActionChanged
			Dim dsl As DragSourceListener = listener
			If dsl IsNot Nothing Then dsl.dropActionChanged(dsde)
			dragSource.processDropActionChanged(dsde)

			updateCurrentCursor(sourceActions, dsde.targetActions, CHANGED)
		End Sub

		''' <summary>
		''' Calls <code>dragDropEnd</code> on the
		''' <code>DragSourceListener</code>s registered with this
		''' <code>DragSourceContext</code> and with the associated
		''' <code>DragSource</code>, and passes them the specified
		''' <code>DragSourceDropEvent</code>.
		''' </summary>
		''' <param name="dsde"> the <code>DragSourceDropEvent</code> </param>
		Public Overridable Sub dragDropEnd(  dsde As DragSourceDropEvent) Implements DragSourceListener.dragDropEnd
			Dim dsl As DragSourceListener = listener
			If dsl IsNot Nothing Then dsl.dragDropEnd(dsde)
			dragSource.processDragDropEnd(dsde)
		End Sub

		''' <summary>
		''' Calls <code>dragMouseMoved</code> on the
		''' <code>DragSourceMotionListener</code>s registered with the
		''' <code>DragSource</code> associated with this
		''' <code>DragSourceContext</code>, and them passes the specified
		''' <code>DragSourceDragEvent</code>.
		''' </summary>
		''' <param name="dsde"> the <code>DragSourceDragEvent</code>
		''' @since 1.4 </param>
		Public Overridable Sub dragMouseMoved(  dsde As DragSourceDragEvent) Implements DragSourceMotionListener.dragMouseMoved
			dragSource.processDragMouseMoved(dsde)
		End Sub

		''' <summary>
		''' Returns the <code>Transferable</code> associated with
		''' this <code>DragSourceContext</code>.
		''' </summary>
		''' <returns> the <code>Transferable</code> </returns>
		Public Overridable Property transferable As java.awt.datatransfer.Transferable
			Get
				Return transferable
			End Get
		End Property

		''' <summary>
		''' If the default drag cursor behavior is active, this method
		''' sets the default drag cursor for the specified actions
		''' supported by the drag source, the drop target action,
		''' and status, otherwise this method does nothing.
		''' </summary>
		''' <param name="sourceAct"> the actions supported by the drag source </param>
		''' <param name="targetAct"> the drop target action </param>
		''' <param name="status"> one of the fields <code>DEFAULT</code>,
		'''               <code>ENTER</code>, <code>OVER</code>,
		'''               <code>CHANGED</code> </param>

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overridable Sub updateCurrentCursor(  sourceAct As Integer,   targetAct As Integer,   status As Integer)

			' if the cursor has been previously set then don't do any defaults
			' processing.

			If useCustomCursor Then Return

			' do defaults processing

			Dim c As java.awt.Cursor = Nothing

			Select Case status
				Case Else
					targetAct = DnDConstants.ACTION_NONE
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case ENTER, OVER, CHANGED
					Dim ra As Integer = sourceAct And targetAct

					If ra = DnDConstants.ACTION_NONE Then ' no drop possible
						If (sourceAct And DnDConstants.ACTION_LINK) = DnDConstants.ACTION_LINK Then
							c = DragSource.DefaultLinkNoDrop
						ElseIf (sourceAct And DnDConstants.ACTION_MOVE) = DnDConstants.ACTION_MOVE Then
							c = DragSource.DefaultMoveNoDrop
						Else
							c = DragSource.DefaultCopyNoDrop
						End If ' drop possible
					Else
						If (ra And DnDConstants.ACTION_LINK) = DnDConstants.ACTION_LINK Then
							c = DragSource.DefaultLinkDrop
						ElseIf (ra And DnDConstants.ACTION_MOVE) = DnDConstants.ACTION_MOVE Then
							c = DragSource.DefaultMoveDrop
						Else
							c = DragSource.DefaultCopyDrop
						End If
					End If
			End Select

			cursorImpl = c
		End Sub

		Private Property cursorImpl As java.awt.Cursor
			Set(  c As java.awt.Cursor)
				If cursor_Renamed Is Nothing OrElse (Not cursor_Renamed.Equals(c)) Then
					cursor_Renamed = c
					If peer IsNot Nothing Then peer.cursor = cursor_Renamed
				End If
			End Set
		End Property

		''' <summary>
		''' Serializes this <code>DragSourceContext</code>. This method first
		''' performs default serialization. Next, this object's
		''' <code>Transferable</code> is written out if and only if it can be
		''' serialized. If not, <code>null</code> is written instead. In this case,
		''' a <code>DragSourceContext</code> created from the resulting deserialized
		''' stream will contain a dummy <code>Transferable</code> which supports no
		''' <code>DataFlavor</code>s. Finally, this object's
		''' <code>DragSourceListener</code> is written out if and only if it can be
		''' serialized. If not, <code>null</code> is written instead.
		''' 
		''' @serialData The default serializable fields, in alphabetical order,
		'''             followed by either a <code>Transferable</code> instance, or
		'''             <code>null</code>, followed by either a
		'''             <code>DragSourceListener</code> instance, or
		'''             <code>null</code>.
		''' @since 1.4
		''' </summary>
		Private Sub writeObject(  s As java.io.ObjectOutputStream)
			s.defaultWriteObject()

			s.writeObject(If(SerializationTester.test(transferable), transferable, Nothing))
			s.writeObject(If(SerializationTester.test(listener), listener, Nothing))
		End Sub

		''' <summary>
		''' Deserializes this <code>DragSourceContext</code>. This method first
		''' performs default deserialization for all non-<code>transient</code>
		''' fields. This object's <code>Transferable</code> and
		''' <code>DragSourceListener</code> are then deserialized as well by using
		''' the next two objects in the stream. If the resulting
		''' <code>Transferable</code> is <code>null</code>, this object's
		''' <code>Transferable</code> is set to a dummy <code>Transferable</code>
		''' which supports no <code>DataFlavor</code>s.
		''' 
		''' @since 1.4
		''' </summary>
		Private Sub readObject(  s As java.io.ObjectInputStream)
			Dim f As java.io.ObjectInputStream.GetField = s.readFields()

			Dim newTrigger As DragGestureEvent = CType(f.get("trigger", Nothing), DragGestureEvent)
			If newTrigger Is Nothing Then Throw New java.io.InvalidObjectException("Null trigger")
			If newTrigger.dragSource Is Nothing Then Throw New java.io.InvalidObjectException("Null DragSource")
			If newTrigger.component Is Nothing Then Throw New java.io.InvalidObjectException("Null trigger component")

			Dim newSourceActions As Integer = AscW(f.get("sourceActions", 0)) And (DnDConstants.ACTION_COPY_OR_MOVE Or DnDConstants.ACTION_LINK)
			If newSourceActions = DnDConstants.ACTION_NONE Then Throw New java.io.InvalidObjectException("Invalid source actions")
			Dim triggerActions As Integer = newTrigger.dragAction
			If triggerActions <> DnDConstants.ACTION_COPY AndAlso triggerActions <> DnDConstants.ACTION_MOVE AndAlso triggerActions <> DnDConstants.ACTION_LINK Then Throw New java.io.InvalidObjectException("No drag action")
			trigger = newTrigger

			cursor_Renamed = CType(f.get("cursor", Nothing), java.awt.Cursor)
			useCustomCursor = f.get("useCustomCursor", False)
			sourceActions = newSourceActions

			transferable = CType(s.readObject(), java.awt.datatransfer.Transferable)
			listener = CType(s.readObject(), DragSourceListener)

			' Implementation assumes 'transferable' is never null.
			If transferable Is Nothing Then
				If emptyTransferable Is Nothing Then emptyTransferable = New TransferableAnonymousInnerClassHelper
				transferable = emptyTransferable
			End If
		End Sub

		Private Class TransferableAnonymousInnerClassHelper
			Implements java.awt.datatransfer.Transferable

			Public Overridable Property transferDataFlavors As java.awt.datatransfer.DataFlavor()
				Get
					Return New java.awt.datatransfer.DataFlavor(){}
				End Get
			End Property
			Public Overridable Function isDataFlavorSupported(  flavor As java.awt.datatransfer.DataFlavor) As Boolean
				Return False
			End Function
			Public Overridable Function getTransferData(  flavor As java.awt.datatransfer.DataFlavor) As Object
				Throw New java.awt.datatransfer.UnsupportedFlavorException(flavor)
			End Function
		End Class

		Private Shared emptyTransferable As java.awt.datatransfer.Transferable

	'    
	'     * fields
	'     

		<NonSerialized> _
		Private peer As java.awt.dnd.peer.DragSourceContextPeer

		''' <summary>
		''' The event which triggered the start of the drag.
		''' 
		''' @serial
		''' </summary>
		Private trigger As DragGestureEvent

		''' <summary>
		''' The current drag cursor.
		''' 
		''' @serial
		''' </summary>
		Private cursor_Renamed As java.awt.Cursor

		<NonSerialized> _
		Private transferable As java.awt.datatransfer.Transferable

		<NonSerialized> _
		Private listener As DragSourceListener

		''' <summary>
		''' <code>true</code> if the custom drag cursor is used instead of the
		''' default one.
		''' 
		''' @serial
		''' </summary>
		Private useCustomCursor As Boolean

		''' <summary>
		''' A bitwise mask of <code>DnDConstants</code> that represents the set of
		''' drop actions supported by the drag source for the drag operation associated
		''' with this <code>DragSourceContext.</code>
		''' 
		''' @serial
		''' </summary>
		Private sourceActions As Integer
	End Class

End Namespace
Imports System
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A <code>DragGestureEvent</code> is passed
	''' to <code>DragGestureListener</code>'s
	''' dragGestureRecognized() method
	''' when a particular <code>DragGestureRecognizer</code> detects that a
	''' platform dependent drag initiating gesture has occurred
	''' on the <code>Component</code> that it is tracking.
	''' 
	''' The {@code action} field of any {@code DragGestureEvent} instance should take one of the following
	''' values:
	''' <ul>
	''' <li> {@code DnDConstants.ACTION_COPY}
	''' <li> {@code DnDConstants.ACTION_MOVE}
	''' <li> {@code DnDConstants.ACTION_LINK}
	''' </ul>
	''' Assigning the value different from listed above will cause an unspecified behavior.
	''' </summary>
	''' <seealso cref= java.awt.dnd.DragGestureRecognizer </seealso>
	''' <seealso cref= java.awt.dnd.DragGestureListener </seealso>
	''' <seealso cref= java.awt.dnd.DragSource </seealso>
	''' <seealso cref= java.awt.dnd.DnDConstants </seealso>

	Public Class DragGestureEvent
		Inherits java.util.EventObject

		Private Const serialVersionUID As Long = 9080172649166731306L

		''' <summary>
		''' Constructs a <code>DragGestureEvent</code> object given by the
		''' <code>DragGestureRecognizer</code> instance firing this event,
		''' an {@code act} parameter representing
		''' the user's preferred action, an {@code ori} parameter
		''' indicating the origin of the drag, and a {@code List} of
		''' events that comprise the gesture({@code evs} parameter).
		''' <P> </summary>
		''' <param name="dgr"> The <code>DragGestureRecognizer</code> firing this event </param>
		''' <param name="act"> The user's preferred action.
		'''            For information on allowable values, see
		'''            the class description for <seealso cref="DragGestureEvent"/> </param>
		''' <param name="ori"> The origin of the drag </param>
		''' <param name="evs"> The <code>List</code> of events that comprise the gesture
		''' <P> </param>
		''' <exception cref="IllegalArgumentException"> if any parameter equals {@code null} </exception>
		''' <exception cref="IllegalArgumentException"> if the act parameter does not comply with
		'''                                  the values given in the class
		'''                                  description for <seealso cref="DragGestureEvent"/> </exception>
		''' <seealso cref= java.awt.dnd.DnDConstants </seealso>

		Public Sub New(Of T1 As java.awt.event.InputEvent)(ByVal dgr As DragGestureRecognizer, ByVal act As Integer, ByVal ori As java.awt.Point, ByVal evs As IList(Of T1))
			MyBase.New(dgr)

			component_Renamed = dgr.component
			If component_Renamed Is Nothing Then Throw New IllegalArgumentException("null component")
			dragSource = dgr.dragSource
			If dragSource Is Nothing Then Throw New IllegalArgumentException("null DragSource")

			If evs Is Nothing OrElse evs.Count = 0 Then Throw New IllegalArgumentException("null or empty list of events")

			If act <> DnDConstants.ACTION_COPY AndAlso act <> DnDConstants.ACTION_MOVE AndAlso act <> DnDConstants.ACTION_LINK Then Throw New IllegalArgumentException("bad action")

			If ori Is Nothing Then Throw New IllegalArgumentException("null origin")

			events = evs
			action = act
			origin = ori
		End Sub

		''' <summary>
		''' Returns the source as a <code>DragGestureRecognizer</code>.
		''' <P> </summary>
		''' <returns> the source as a <code>DragGestureRecognizer</code> </returns>

		Public Overridable Property sourceAsDragGestureRecognizer As DragGestureRecognizer
			Get
				Return CType(source, DragGestureRecognizer)
			End Get
		End Property

		''' <summary>
		''' Returns the <code>Component</code> associated
		''' with this <code>DragGestureEvent</code>.
		''' <P> </summary>
		''' <returns> the Component </returns>

		Public Overridable Property component As java.awt.Component
			Get
				Return component_Renamed
			End Get
		End Property

		''' <summary>
		''' Returns the <code>DragSource</code>.
		''' <P> </summary>
		''' <returns> the <code>DragSource</code> </returns>

		Public Overridable Property dragSource As DragSource
			Get
				Return dragSource
			End Get
		End Property

		''' <summary>
		''' Returns a <code>Point</code> in the coordinates
		''' of the <code>Component</code> over which the drag originated.
		''' <P> </summary>
		''' <returns> the Point where the drag originated in Component coords. </returns>

		Public Overridable Property dragOrigin As java.awt.Point
			Get
				Return origin
			End Get
		End Property

		''' <summary>
		''' Returns an <code>Iterator</code> for the events
		''' comprising the gesture.
		''' <P> </summary>
		''' <returns> an Iterator for the events comprising the gesture </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function [iterator]() As IEnumerator(Of java.awt.event.InputEvent)
			Return events.GetEnumerator()
		End Function

		''' <summary>
		''' Returns an <code>Object</code> array of the
		''' events comprising the drag gesture.
		''' <P> </summary>
		''' <returns> an array of the events comprising the gesture </returns>

		Public Overridable Function toArray() As Object()
			Return events.ToArray()
		End Function

		''' <summary>
		''' Returns an array of the events comprising the drag gesture.
		''' <P> </summary>
		''' <param name="array"> the array of <code>EventObject</code> sub(types)
		''' <P> </param>
		''' <returns> an array of the events comprising the gesture </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function toArray(ByVal array As Object()) As Object()
			Return events.ToArray(array)
		End Function

		''' <summary>
		''' Returns an <code>int</code> representing the
		''' action selected by the user.
		''' <P> </summary>
		''' <returns> the action selected by the user </returns>

		Public Overridable Property dragAction As Integer
			Get
				Return action
			End Get
		End Property

		''' <summary>
		''' Returns the initial event that triggered the gesture.
		''' <P> </summary>
		''' <returns> the first "triggering" event in the sequence of the gesture </returns>

		Public Overridable Property triggerEvent As java.awt.event.InputEvent
			Get
				Return sourceAsDragGestureRecognizer.triggerEvent
			End Get
		End Property

		''' <summary>
		''' Starts the drag operation given the <code>Cursor</code> for this drag
		''' operation and the <code>Transferable</code> representing the source data
		''' for this drag operation.
		''' <br>
		''' If a <code>null</code> <code>Cursor</code> is specified no exception will
		''' be thrown and default drag cursors will be used instead.
		''' <br>
		''' If a <code>null</code> <code>Transferable</code> is specified
		''' <code>NullPointerException</code> will be thrown. </summary>
		''' <param name="dragCursor">     The initial {@code Cursor} for this drag operation
		'''                       or {@code null} for the default cursor handling;
		'''                       see
		'''                       <a href="DragSourceContext.html#defaultCursor">DragSourceContext</a>
		'''                       for more details on the cursor handling mechanism
		'''                       during drag and drop </param>
		''' <param name="transferable"> The <code>Transferable</code> representing the source
		'''                     data for this drag operation.
		''' </param>
		''' <exception cref="InvalidDnDOperationException"> if the Drag and Drop
		'''         system is unable to initiate a drag operation, or if the user
		'''         attempts to start a drag while an existing drag operation is
		'''         still executing. </exception>
		''' <exception cref="NullPointerException"> if the {@code Transferable} is {@code null}
		''' @since 1.4 </exception>
		Public Overridable Sub startDrag(ByVal dragCursor As java.awt.Cursor, ByVal transferable As java.awt.datatransfer.Transferable)
			dragSource.startDrag(Me, dragCursor, transferable, Nothing)
		End Sub

		''' <summary>
		''' Starts the drag given the initial <code>Cursor</code> to display,
		''' the <code>Transferable</code> object,
		''' and the <code>DragSourceListener</code> to use.
		''' <P> </summary>
		''' <param name="dragCursor">     The initial {@code Cursor} for this drag operation
		'''                       or {@code null} for the default cursor handling;
		'''                       see
		'''                       <a href="DragSourceContext.html#defaultCursor">DragSourceContext</a>
		'''                       for more details on the cursor handling mechanism
		'''                       during drag and drop </param>
		''' <param name="transferable"> The source's Transferable </param>
		''' <param name="dsl">          The source's DragSourceListener
		''' <P> </param>
		''' <exception cref="InvalidDnDOperationException"> if
		''' the Drag and Drop system is unable to
		''' initiate a drag operation, or if the user
		''' attempts to start a drag while an existing
		''' drag operation is still executing. </exception>

		Public Overridable Sub startDrag(ByVal dragCursor As java.awt.Cursor, ByVal transferable As java.awt.datatransfer.Transferable, ByVal dsl As DragSourceListener)
			dragSource.startDrag(Me, dragCursor, transferable, dsl)
		End Sub

		''' <summary>
		''' Start the drag given the initial <code>Cursor</code> to display,
		''' a drag <code>Image</code>, the offset of
		''' the <code>Image</code>,
		''' the <code>Transferable</code> object, and
		''' the <code>DragSourceListener</code> to use.
		''' <P> </summary>
		''' <param name="dragCursor">     The initial {@code Cursor} for this drag operation
		'''                       or {@code null} for the default cursor handling;
		'''                       see
		'''                       <a href="DragSourceContext.html#defaultCursor">DragSourceContext</a>
		'''                       for more details on the cursor handling mechanism
		'''                       during drag and drop </param>
		''' <param name="dragImage">    The source's dragImage </param>
		''' <param name="imageOffset">  The dragImage's offset </param>
		''' <param name="transferable"> The source's Transferable </param>
		''' <param name="dsl">          The source's DragSourceListener
		''' <P> </param>
		''' <exception cref="InvalidDnDOperationException"> if
		''' the Drag and Drop system is unable to
		''' initiate a drag operation, or if the user
		''' attempts to start a drag while an existing
		''' drag operation is still executing. </exception>

		Public Overridable Sub startDrag(ByVal dragCursor As java.awt.Cursor, ByVal dragImage As java.awt.Image, ByVal imageOffset As java.awt.Point, ByVal transferable As java.awt.datatransfer.Transferable, ByVal dsl As DragSourceListener)
			dragSource.startDrag(Me, dragCursor, dragImage, imageOffset, transferable, dsl)
		End Sub

		''' <summary>
		''' Serializes this <code>DragGestureEvent</code>. Performs default
		''' serialization and then writes out this object's <code>List</code> of
		''' gesture events if and only if the <code>List</code> can be serialized.
		''' If not, <code>null</code> is written instead. In this case, a
		''' <code>DragGestureEvent</code> created from the resulting deserialized
		''' stream will contain an empty <code>List</code> of gesture events.
		''' 
		''' @serialData The default serializable fields, in alphabetical order,
		'''             followed by either a <code>List</code> instance, or
		'''             <code>null</code>.
		''' @since 1.4
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()

			s.writeObject(If(SerializationTester.test(events), events, Nothing))
		End Sub

		''' <summary>
		''' Deserializes this <code>DragGestureEvent</code>. This method first
		''' performs default deserialization for all non-<code>transient</code>
		''' fields. An attempt is then made to deserialize this object's
		''' <code>List</code> of gesture events as well. This is first attempted
		''' by deserializing the field <code>events</code>, because, in releases
		''' prior to 1.4, a non-<code>transient</code> field of this name stored the
		''' <code>List</code> of gesture events. If this fails, the next object in
		''' the stream is used instead. If the resulting <code>List</code> is
		''' <code>null</code>, this object's <code>List</code> of gesture events
		''' is set to an empty <code>List</code>.
		''' 
		''' @since 1.4
		''' </summary>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			Dim f As java.io.ObjectInputStream.GetField = s.readFields()

			Dim newDragSource As DragSource = CType(f.get("dragSource", Nothing), DragSource)
			If newDragSource Is Nothing Then Throw New java.io.InvalidObjectException("null DragSource")
			dragSource = newDragSource

			Dim newComponent As java.awt.Component = CType(f.get("component", Nothing), java.awt.Component)
			If newComponent Is Nothing Then Throw New java.io.InvalidObjectException("null component")
			component_Renamed = newComponent

			Dim newOrigin As java.awt.Point = CType(f.get("origin", Nothing), java.awt.Point)
			If newOrigin Is Nothing Then Throw New java.io.InvalidObjectException("null origin")
			origin = newOrigin

			Dim newAction As Integer = f.get("action", 0)
			If newAction <> DnDConstants.ACTION_COPY AndAlso newAction <> DnDConstants.ACTION_MOVE AndAlso newAction <> DnDConstants.ACTION_LINK Then Throw New java.io.InvalidObjectException("bad action")
			action = newAction

			' Pre-1.4 support. 'events' was previously non-transient
			Dim newEvents As IList
			Try
				newEvents = CType(f.get("events", Nothing), IList)
			Catch e As IllegalArgumentException
				' 1.4-compatible byte stream. 'events' was written explicitly
				newEvents = CType(s.readObject(), IList)
			End Try

			' Implementation assumes 'events' is never null.
			If newEvents IsNot Nothing AndAlso newEvents.Count = 0 Then
				' Constructor treats empty events list as invalid value
				' Throw exception if serialized list is empty
				Throw New java.io.InvalidObjectException("empty list of events")
			ElseIf newEvents Is Nothing Then
				newEvents = java.util.Collections.emptyList()
			End If
			events = newEvents
		End Sub

	'    
	'     * fields
	'     
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		<NonSerialized> _
		Private events As IList

		''' <summary>
		''' The DragSource associated with this DragGestureEvent.
		''' 
		''' @serial
		''' </summary>
		Private dragSource As DragSource

		''' <summary>
		''' The Component associated with this DragGestureEvent.
		''' 
		''' @serial
		''' </summary>
		Private component_Renamed As java.awt.Component

		''' <summary>
		''' The origin of the drag.
		''' 
		''' @serial
		''' </summary>
		Private origin As java.awt.Point

		''' <summary>
		''' The user's preferred action.
		''' 
		''' @serial
		''' </summary>
		Private action As Integer
	End Class

End Namespace
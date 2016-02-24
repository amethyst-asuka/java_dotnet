Imports System
Imports System.Runtime.CompilerServices
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
	''' The <code>DragGestureRecognizer</code> is an
	''' abstract base class for the specification
	''' of a platform-dependent listener that can be associated with a particular
	''' <code>Component</code> in order to
	''' identify platform-dependent drag initiating gestures.
	''' <p>
	''' The appropriate <code>DragGestureRecognizer</code>
	''' subclass instance is obtained from the
	''' <seealso cref="DragSource"/> associated with
	''' a particular <code>Component</code>, or from the <code>Toolkit</code> object via its
	''' <seealso cref="java.awt.Toolkit#createDragGestureRecognizer createDragGestureRecognizer()"/>
	''' method.
	''' <p>
	''' Once the <code>DragGestureRecognizer</code>
	''' is associated with a particular <code>Component</code>
	''' it will register the appropriate listener interfaces on that
	''' <code>Component</code>
	''' in order to track the input events delivered to the <code>Component</code>.
	''' <p>
	''' Once the <code>DragGestureRecognizer</code> identifies a sequence of events
	''' on the <code>Component</code> as a drag initiating gesture, it will notify
	''' its unicast <code>DragGestureListener</code> by
	''' invoking its
	''' <seealso cref="java.awt.dnd.DragGestureListener#dragGestureRecognized gestureRecognized()"/>
	''' method.
	''' <P>
	''' When a concrete <code>DragGestureRecognizer</code>
	''' instance detects a drag initiating
	''' gesture on the <code>Component</code> it is associated with,
	''' it fires a <seealso cref="DragGestureEvent"/> to
	''' the <code>DragGestureListener</code> registered on
	''' its unicast event source for <code>DragGestureListener</code>
	''' events. This <code>DragGestureListener</code> is responsible
	''' for causing the associated
	''' <code>DragSource</code> to start the Drag and Drop operation (if
	''' appropriate).
	''' <P>
	''' @author Laurence P. G. Cable </summary>
	''' <seealso cref= java.awt.dnd.DragGestureListener </seealso>
	''' <seealso cref= java.awt.dnd.DragGestureEvent </seealso>
	''' <seealso cref= java.awt.dnd.DragSource </seealso>

	<Serializable> _
	Public MustInherit Class DragGestureRecognizer

		Private Const serialVersionUID As Long = 8996673345831063337L

		''' <summary>
		''' Construct a new <code>DragGestureRecognizer</code>
		''' given the <code>DragSource</code> to be used
		''' in this Drag and Drop operation, the <code>Component</code>
		''' this <code>DragGestureRecognizer</code> should "observe"
		''' for drag initiating gestures, the action(s) supported
		''' for this Drag and Drop operation, and the
		''' <code>DragGestureListener</code> to notify
		''' once a drag initiating gesture has been detected.
		''' <P> </summary>
		''' <param name="ds">  the <code>DragSource</code> this
		''' <code>DragGestureRecognizer</code>
		''' will use to process the Drag and Drop operation
		''' </param>
		''' <param name="c"> the <code>Component</code>
		''' this <code>DragGestureRecognizer</code>
		''' should "observe" the event stream to,
		''' in order to detect a drag initiating gesture.
		''' If this value is <code>null</code>, the
		''' <code>DragGestureRecognizer</code>
		''' is not associated with any <code>Component</code>.
		''' </param>
		''' <param name="sa">  the set (logical OR) of the
		''' <code>DnDConstants</code>
		''' that this Drag and Drop operation will support
		''' </param>
		''' <param name="dgl"> the <code>DragGestureRecognizer</code>
		''' to notify when a drag gesture is detected
		''' <P> </param>
		''' <exception cref="IllegalArgumentException">
		''' if ds is <code>null</code>. </exception>

		Protected Friend Sub New(ByVal ds As DragSource, ByVal c As java.awt.Component, ByVal sa As Integer, ByVal dgl As DragGestureListener)
			MyBase.New()

			If ds Is Nothing Then Throw New IllegalArgumentException("null DragSource")

			dragSource = ds
			component_Renamed = c
			sourceActions = sa And (DnDConstants.ACTION_COPY_OR_MOVE Or DnDConstants.ACTION_LINK)

			Try
				If dgl IsNot Nothing Then addDragGestureListener(dgl)
			Catch tmle As java.util.TooManyListenersException
				' cant happen ...
			End Try
		End Sub

		''' <summary>
		''' Construct a new <code>DragGestureRecognizer</code>
		''' given the <code>DragSource</code> to be used in this
		''' Drag and Drop
		''' operation, the <code>Component</code> this
		''' <code>DragGestureRecognizer</code> should "observe"
		''' for drag initiating gestures, and the action(s)
		''' supported for this Drag and Drop operation.
		''' <P> </summary>
		''' <param name="ds">  the <code>DragSource</code> this
		''' <code>DragGestureRecognizer</code> will use to
		''' process the Drag and Drop operation
		''' </param>
		''' <param name="c">   the <code>Component</code> this
		''' <code>DragGestureRecognizer</code> should "observe" the event
		''' stream to, in order to detect a drag initiating gesture.
		''' If this value is <code>null</code>, the
		''' <code>DragGestureRecognizer</code>
		''' is not associated with any <code>Component</code>.
		''' </param>
		''' <param name="sa"> the set (logical OR) of the <code>DnDConstants</code>
		''' that this Drag and Drop operation will support
		''' <P> </param>
		''' <exception cref="IllegalArgumentException">
		''' if ds is <code>null</code>. </exception>

		Protected Friend Sub New(ByVal ds As DragSource, ByVal c As java.awt.Component, ByVal sa As Integer)
			Me.New(ds, c, sa, Nothing)
		End Sub

		''' <summary>
		''' Construct a new <code>DragGestureRecognizer</code>
		''' given the <code>DragSource</code> to be used
		''' in this Drag and Drop operation, and
		''' the <code>Component</code> this
		''' <code>DragGestureRecognizer</code>
		''' should "observe" for drag initiating gestures.
		''' <P> </summary>
		''' <param name="ds"> the <code>DragSource</code> this
		''' <code>DragGestureRecognizer</code>
		''' will use to process the Drag and Drop operation
		''' </param>
		''' <param name="c"> the <code>Component</code>
		''' this <code>DragGestureRecognizer</code>
		''' should "observe" the event stream to,
		''' in order to detect a drag initiating gesture.
		''' If this value is <code>null</code>,
		''' the <code>DragGestureRecognizer</code>
		''' is not associated with any <code>Component</code>.
		''' <P> </param>
		''' <exception cref="IllegalArgumentException">
		''' if ds is <code>null</code>. </exception>

		Protected Friend Sub New(ByVal ds As DragSource, ByVal c As java.awt.Component)
			Me.New(ds, c, DnDConstants.ACTION_NONE)
		End Sub

		''' <summary>
		''' Construct a new <code>DragGestureRecognizer</code>
		''' given the <code>DragSource</code> to be used in this
		''' Drag and Drop operation.
		''' <P> </summary>
		''' <param name="ds"> the <code>DragSource</code> this
		''' <code>DragGestureRecognizer</code> will
		''' use to process the Drag and Drop operation
		''' <P> </param>
		''' <exception cref="IllegalArgumentException">
		''' if ds is <code>null</code>. </exception>

		Protected Friend Sub New(ByVal ds As DragSource)
			Me.New(ds, Nothing)
		End Sub

		''' <summary>
		''' register this DragGestureRecognizer's Listeners with the Component
		''' 
		''' subclasses must override this method
		''' </summary>

		Protected Friend MustOverride Sub registerListeners()

		''' <summary>
		''' unregister this DragGestureRecognizer's Listeners with the Component
		''' 
		''' subclasses must override this method
		''' </summary>

		Protected Friend MustOverride Sub unregisterListeners()

		''' <summary>
		''' This method returns the <code>DragSource</code>
		''' this <code>DragGestureRecognizer</code>
		''' will use in order to process the Drag and Drop
		''' operation.
		''' <P> </summary>
		''' <returns> the DragSource </returns>

		Public Overridable Property dragSource As DragSource
			Get
				Return dragSource
			End Get
		End Property

		''' <summary>
		''' This method returns the <code>Component</code>
		''' that is to be "observed" by the
		''' <code>DragGestureRecognizer</code>
		''' for drag initiating gestures.
		''' <P> </summary>
		''' <returns> The Component this DragGestureRecognizer
		''' is associated with </returns>

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property component As java.awt.Component
			Get
				Return component_Renamed
			End Get
			Set(ByVal c As java.awt.Component)
				If component_Renamed IsNot Nothing AndAlso dragGestureListener IsNot Nothing Then unregisterListeners()
    
				component_Renamed = c
    
				If component_Renamed IsNot Nothing AndAlso dragGestureListener IsNot Nothing Then registerListeners()
			End Set
		End Property



		''' <summary>
		''' This method returns an int representing the
		''' type of action(s) this Drag and Drop
		''' operation will support.
		''' <P> </summary>
		''' <returns> the currently permitted source action(s) </returns>

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property sourceActions As Integer
			Get
				Return sourceActions
			End Get
			Set(ByVal actions As Integer)
				sourceActions = actions And (DnDConstants.ACTION_COPY_OR_MOVE Or DnDConstants.ACTION_LINK)
			End Set
		End Property



		''' <summary>
		''' This method returns the first event in the
		''' series of events that initiated
		''' the Drag and Drop operation.
		''' <P> </summary>
		''' <returns> the initial event that triggered the drag gesture </returns>

		Public Overridable Property triggerEvent As java.awt.event.InputEvent
			Get
				Return If(events.Count = 0, Nothing, events(0))
			End Get
		End Property

		''' <summary>
		''' Reset the Recognizer, if its currently recognizing a gesture, ignore
		''' it.
		''' </summary>

		Public Overridable Sub resetRecognizer()
			events.Clear()
		End Sub

		''' <summary>
		''' Register a new <code>DragGestureListener</code>.
		''' <P> </summary>
		''' <param name="dgl"> the <code>DragGestureListener</code> to register
		''' with this <code>DragGestureRecognizer</code>.
		''' <P> </param>
		''' <exception cref="java.util.TooManyListenersException"> if a
		''' <code>DragGestureListener</code> has already been added. </exception>

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addDragGestureListener(ByVal dgl As DragGestureListener)
			If dragGestureListener IsNot Nothing Then
				Throw New java.util.TooManyListenersException
			Else
				dragGestureListener = dgl

				If component_Renamed IsNot Nothing Then registerListeners()
			End If
		End Sub

		''' <summary>
		''' unregister the current DragGestureListener
		''' <P> </summary>
		''' <param name="dgl"> the <code>DragGestureListener</code> to unregister
		''' from this <code>DragGestureRecognizer</code>
		''' <P> </param>
		''' <exception cref="IllegalArgumentException"> if
		''' dgl is not (equal to) the currently registered <code>DragGestureListener</code>. </exception>

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeDragGestureListener(ByVal dgl As DragGestureListener)
			If dragGestureListener Is Nothing OrElse (Not dragGestureListener.Equals(dgl)) Then
				Throw New IllegalArgumentException
			Else
				dragGestureListener = Nothing

				If component_Renamed IsNot Nothing Then unregisterListeners()
			End If
		End Sub

		''' <summary>
		''' Notify the DragGestureListener that a Drag and Drop initiating
		''' gesture has occurred. Then reset the state of the Recognizer.
		''' <P> </summary>
		''' <param name="dragAction"> The action initially selected by the users gesture </param>
		''' <param name="p">          The point (in Component coords) where the gesture originated </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overridable Sub fireDragGestureRecognized(ByVal dragAction As Integer, ByVal p As java.awt.Point)
			Try
				If dragGestureListener IsNot Nothing Then dragGestureListener.dragGestureRecognized(New DragGestureEvent(Me, dragAction, p, events))
			Finally
				events.Clear()
			End Try
		End Sub

		''' <summary>
		''' Listeners registered on the Component by this Recognizer shall record
		''' all Events that are recognized as part of the series of Events that go
		''' to comprise a Drag and Drop initiating gesture via this API.
		''' <P>
		''' This method is used by a <code>DragGestureRecognizer</code>
		''' implementation to add an <code>InputEvent</code>
		''' subclass (that it believes is one in a series
		''' of events that comprise a Drag and Drop operation)
		''' to the array of events that this
		''' <code>DragGestureRecognizer</code> maintains internally.
		''' <P> </summary>
		''' <param name="awtie"> the <code>InputEvent</code>
		''' to add to this <code>DragGestureRecognizer</code>'s
		''' internal array of events. Note that <code>null</code>
		''' is not a valid value, and will be ignored. </param>

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overridable Sub appendEvent(ByVal awtie As java.awt.event.InputEvent)
			events.Add(awtie)
		End Sub

		''' <summary>
		''' Serializes this <code>DragGestureRecognizer</code>. This method first
		''' performs default serialization. Then, this object's
		''' <code>DragGestureListener</code> is written out if and only if it can be
		''' serialized. If not, <code>null</code> is written instead.
		''' 
		''' @serialData The default serializable fields, in alphabetical order,
		'''             followed by either a <code>DragGestureListener</code>, or
		'''             <code>null</code>.
		''' @since 1.4
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()

			s.writeObject(If(SerializationTester.test(dragGestureListener), dragGestureListener, Nothing))
		End Sub

		''' <summary>
		''' Deserializes this <code>DragGestureRecognizer</code>. This method first
		''' performs default deserialization for all non-<code>transient</code>
		''' fields. This object's <code>DragGestureListener</code> is then
		''' deserialized as well by using the next object in the stream.
		''' 
		''' @since 1.4
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			Dim f As java.io.ObjectInputStream.GetField = s.readFields()

			Dim newDragSource As DragSource = CType(f.get("dragSource", Nothing), DragSource)
			If newDragSource Is Nothing Then Throw New java.io.InvalidObjectException("null DragSource")
			dragSource = newDragSource

			component_Renamed = CType(f.get("component", Nothing), java.awt.Component)
			sourceActions = AscW(f.get("sourceActions", 0)) And (DnDConstants.ACTION_COPY_OR_MOVE Or DnDConstants.ACTION_LINK)
			events = CType(f.get("events", New List(Of )(1)), List(Of java.awt.event.InputEvent))

			dragGestureListener = CType(s.readObject(), DragGestureListener)
		End Sub

	'    
	'     * fields
	'     

		''' <summary>
		''' The <code>DragSource</code>
		''' associated with this
		''' <code>DragGestureRecognizer</code>.
		''' 
		''' @serial
		''' </summary>
		Protected Friend dragSource As DragSource

		''' <summary>
		''' The <code>Component</code>
		''' associated with this <code>DragGestureRecognizer</code>.
		''' 
		''' @serial
		''' </summary>
		Protected Friend component_Renamed As java.awt.Component

		''' <summary>
		''' The <code>DragGestureListener</code>
		''' associated with this <code>DragGestureRecognizer</code>.
		''' </summary>
		<NonSerialized> _
		Protected Friend dragGestureListener As DragGestureListener

	  ''' <summary>
	  ''' An <code>int</code> representing
	  ''' the type(s) of action(s) used
	  ''' in this Drag and Drop operation.
	  ''' 
	  ''' @serial
	  ''' </summary>
	  Protected Friend sourceActions As Integer

	   ''' <summary>
	   ''' The list of events (in order) that
	   ''' the <code>DragGestureRecognizer</code>
	   ''' "recognized" as a "gesture" that triggers a drag.
	   ''' 
	   ''' @serial
	   ''' </summary>
	   Protected Friend events As New List(Of java.awt.event.InputEvent)(1)
	End Class

End Namespace
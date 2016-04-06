'
' * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A class extends <code>AWTEventMulticaster</code> to implement efficient and
	''' thread-safe multi-cast event dispatching for the drag-and-drop events defined
	''' in the java.awt.dnd package.
	''' 
	''' @since       1.4 </summary>
	''' <seealso cref= AWTEventMulticaster </seealso>

	Friend Class DnDEventMulticaster
		Inherits java.awt.AWTEventMulticaster
		Implements DragSourceListener, DragSourceMotionListener

		''' <summary>
		''' Creates an event multicaster instance which chains listener-a
		''' with listener-b. Input parameters <code>a</code> and <code>b</code>
		''' should not be <code>null</code>, though implementations may vary in
		''' choosing whether or not to throw <code>NullPointerException</code>
		''' in that case.
		''' </summary>
		''' <param name="a"> listener-a </param>
		''' <param name="b"> listener-b </param>
		Protected Friend Sub New(  a As java.util.EventListener,   b As java.util.EventListener)
			MyBase.New(a,b)
		End Sub

		''' <summary>
		''' Handles the <code>DragSourceDragEvent</code> by invoking
		''' <code>dragEnter</code> on listener-a and listener-b.
		''' </summary>
		''' <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		Public Overridable Sub dragEnter(  dsde As DragSourceDragEvent) Implements DragSourceListener.dragEnter
			CType(a, DragSourceListener).dragEnter(dsde)
			CType(b, DragSourceListener).dragEnter(dsde)
		End Sub

		''' <summary>
		''' Handles the <code>DragSourceDragEvent</code> by invoking
		''' <code>dragOver</code> on listener-a and listener-b.
		''' </summary>
		''' <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		Public Overridable Sub dragOver(  dsde As DragSourceDragEvent) Implements DragSourceListener.dragOver
			CType(a, DragSourceListener).dragOver(dsde)
			CType(b, DragSourceListener).dragOver(dsde)
		End Sub

		''' <summary>
		''' Handles the <code>DragSourceDragEvent</code> by invoking
		''' <code>dropActionChanged</code> on listener-a and listener-b.
		''' </summary>
		''' <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		Public Overridable Sub dropActionChanged(  dsde As DragSourceDragEvent) Implements DragSourceListener.dropActionChanged
			CType(a, DragSourceListener).dropActionChanged(dsde)
			CType(b, DragSourceListener).dropActionChanged(dsde)
		End Sub

		''' <summary>
		''' Handles the <code>DragSourceEvent</code> by invoking
		''' <code>dragExit</code> on listener-a and listener-b.
		''' </summary>
		''' <param name="dse"> the <code>DragSourceEvent</code> </param>
		Public Overridable Sub dragExit(  dse As DragSourceEvent) Implements DragSourceListener.dragExit
			CType(a, DragSourceListener).dragExit(dse)
			CType(b, DragSourceListener).dragExit(dse)
		End Sub

		''' <summary>
		''' Handles the <code>DragSourceDropEvent</code> by invoking
		''' <code>dragDropEnd</code> on listener-a and listener-b.
		''' </summary>
		''' <param name="dsde"> the <code>DragSourceDropEvent</code> </param>
		Public Overridable Sub dragDropEnd(  dsde As DragSourceDropEvent) Implements DragSourceListener.dragDropEnd
			CType(a, DragSourceListener).dragDropEnd(dsde)
			CType(b, DragSourceListener).dragDropEnd(dsde)
		End Sub

		''' <summary>
		''' Handles the <code>DragSourceDragEvent</code> by invoking
		''' <code>dragMouseMoved</code> on listener-a and listener-b.
		''' </summary>
		''' <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		Public Overridable Sub dragMouseMoved(  dsde As DragSourceDragEvent) Implements DragSourceMotionListener.dragMouseMoved
			CType(a, DragSourceMotionListener).dragMouseMoved(dsde)
			CType(b, DragSourceMotionListener).dragMouseMoved(dsde)
		End Sub

		''' <summary>
		''' Adds drag-source-listener-a with drag-source-listener-b and
		''' returns the resulting multicast listener.
		''' </summary>
		''' <param name="a"> drag-source-listener-a </param>
		''' <param name="b"> drag-source-listener-b </param>
		Public Shared Function add(  a As DragSourceListener,   b As DragSourceListener) As DragSourceListener
			Return CType(addInternal(a, b), DragSourceListener)
		End Function

		''' <summary>
		''' Adds drag-source-motion-listener-a with drag-source-motion-listener-b and
		''' returns the resulting multicast listener.
		''' </summary>
		''' <param name="a"> drag-source-motion-listener-a </param>
		''' <param name="b"> drag-source-motion-listener-b </param>
		Public Shared Function add(  a As DragSourceMotionListener,   b As DragSourceMotionListener) As DragSourceMotionListener
			Return CType(addInternal(a, b), DragSourceMotionListener)
		End Function

		''' <summary>
		''' Removes the old drag-source-listener from drag-source-listener-l
		''' and returns the resulting multicast listener.
		''' </summary>
		''' <param name="l"> drag-source-listener-l </param>
		''' <param name="oldl"> the drag-source-listener being removed </param>
		Public Shared Function remove(  l As DragSourceListener,   oldl As DragSourceListener) As DragSourceListener
			Return CType(removeInternal(l, oldl), DragSourceListener)
		End Function

		''' <summary>
		''' Removes the old drag-source-motion-listener from
		''' drag-source-motion-listener-l and returns the resulting multicast
		''' listener.
		''' </summary>
		''' <param name="l"> drag-source-motion-listener-l </param>
		''' <param name="ol"> the drag-source-motion-listener being removed </param>
		Public Shared Function remove(  l As DragSourceMotionListener,   ol As DragSourceMotionListener) As DragSourceMotionListener
			Return CType(removeInternal(l, ol), DragSourceMotionListener)
		End Function

		''' <summary>
		''' Returns the resulting multicast listener from adding listener-a
		''' and listener-b together.
		''' If listener-a is null, it returns listener-b;
		''' If listener-b is null, it returns listener-a
		''' If neither are null, then it creates and returns
		''' a new AWTEventMulticaster instance which chains a with b. </summary>
		''' <param name="a"> event listener-a </param>
		''' <param name="b"> event listener-b </param>
		Protected Friend Shared Function addInternal(  a As java.util.EventListener,   b As java.util.EventListener) As java.util.EventListener
			If a Is Nothing Then Return b
			If b Is Nothing Then Return a
			Return New DnDEventMulticaster(a, b)
		End Function

		''' <summary>
		''' Removes a listener from this multicaster and returns the
		''' resulting multicast listener. </summary>
		''' <param name="oldl"> the listener to be removed </param>
		Protected Friend Overrides Function remove(  oldl As java.util.EventListener) As java.util.EventListener
			If oldl Is a Then Return b
			If oldl Is b Then Return a
			Dim a2 As java.util.EventListener = removeInternal(a, oldl)
			Dim b2 As java.util.EventListener = removeInternal(b, oldl)
			If a2 Is a AndAlso b2 Is b Then Return Me ' it's not here
			Return addInternal(a2, b2)
		End Function

		''' <summary>
		''' Returns the resulting multicast listener after removing the
		''' old listener from listener-l.
		''' If listener-l equals the old listener OR listener-l is null,
		''' returns null.
		''' Else if listener-l is an instance of AWTEventMulticaster,
		''' then it removes the old listener from it.
		''' Else, returns listener l. </summary>
		''' <param name="l"> the listener being removed from </param>
		''' <param name="oldl"> the listener being removed </param>
		Protected Friend Shared Function removeInternal(  l As java.util.EventListener,   oldl As java.util.EventListener) As java.util.EventListener
			If l Is oldl OrElse l Is Nothing Then
				Return Nothing
			ElseIf TypeOf l Is DnDEventMulticaster Then
				Return CType(l, DnDEventMulticaster).remove(oldl)
			Else
				Return l ' it's not here
			End If
		End Function

		Protected Friend Shared Sub save(  s As java.io.ObjectOutputStream,   k As String,   l As java.util.EventListener)
			java.awt.AWTEventMulticaster.save(s, k, l)
		End Sub
	End Class

End Namespace
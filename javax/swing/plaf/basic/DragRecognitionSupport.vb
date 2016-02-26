Imports System
Imports javax.swing

'
' * Copyright (c) 2005, 2008, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.plaf.basic


	''' <summary>
	''' Drag gesture recognition support for classes that have a
	''' <code>TransferHandler</code>. The gesture for a drag in this class is a mouse
	''' press followed by movement by <code>DragSource.getDragThreshold()</code>
	''' pixels. An instance of this class is maintained per AppContext, and the
	''' public static methods call into the appropriate instance.
	''' 
	''' @author Shannon Hickey
	''' </summary>
	Friend Class DragRecognitionSupport
		Private motionThreshold As Integer
		Private dndArmedEvent As MouseEvent
		Private component As JComponent

		''' <summary>
		''' This interface allows us to pass in a handler to mouseDragged,
		''' so that we can be notified immediately before a drag begins.
		''' </summary>
		Public Interface BeforeDrag
			Sub dragStarting(ByVal [me] As MouseEvent)
		End Interface

		''' <summary>
		''' Returns the DragRecognitionSupport for the caller's AppContext.
		''' </summary>
		Private Property Shared dragRecognitionSupport As DragRecognitionSupport
			Get
				Dim support As DragRecognitionSupport = CType(sun.awt.AppContext.appContext.get(GetType(DragRecognitionSupport)), DragRecognitionSupport)
    
				If support Is Nothing Then
					support = New DragRecognitionSupport
					sun.awt.AppContext.appContext.put(GetType(DragRecognitionSupport), support)
				End If
    
				Return support
			End Get
		End Property

		''' <summary>
		''' Returns whether or not the event is potentially part of a drag sequence.
		''' </summary>
		Public Shared Function mousePressed(ByVal [me] As MouseEvent) As Boolean
			Return dragRecognitionSupport.mousePressedImpl([me])
		End Function

		''' <summary>
		''' If a dnd recognition has been going on, return the MouseEvent
		''' that started the recognition. Otherwise, return null.
		''' </summary>
		Public Shared Function mouseReleased(ByVal [me] As MouseEvent) As MouseEvent
			Return dragRecognitionSupport.mouseReleasedImpl([me])
		End Function

		''' <summary>
		''' Returns whether or not a drag gesture recognition is ongoing.
		''' </summary>
		Public Shared Function mouseDragged(ByVal [me] As MouseEvent, ByVal bd As BeforeDrag) As Boolean
			Return dragRecognitionSupport.mouseDraggedImpl([me], bd)
		End Function

		Private Sub clearState()
			dndArmedEvent = Nothing
			component = Nothing
		End Sub

		Private Function mapDragOperationFromModifiers(ByVal [me] As MouseEvent, ByVal th As TransferHandler) As Integer

			If th Is Nothing OrElse (Not SwingUtilities.isLeftMouseButton([me])) Then Return TransferHandler.NONE

			Return sun.awt.dnd.SunDragSourceContextPeer.convertModifiersToDropAction([me].modifiersEx, th.getSourceActions(component))
		End Function

		''' <summary>
		''' Returns whether or not the event is potentially part of a drag sequence.
		''' </summary>
		Private Function mousePressedImpl(ByVal [me] As MouseEvent) As Boolean
			component = CType([me].source, JComponent)

			If mapDragOperationFromModifiers([me], component.transferHandler) <> TransferHandler.NONE Then

				motionThreshold = java.awt.dnd.DragSource.dragThreshold
				dndArmedEvent = [me]
				Return True
			End If

			clearState()
			Return False
		End Function

		''' <summary>
		''' If a dnd recognition has been going on, return the MouseEvent
		''' that started the recognition. Otherwise, return null.
		''' </summary>
		Private Function mouseReleasedImpl(ByVal [me] As MouseEvent) As MouseEvent
			' no recognition has been going on 
			If dndArmedEvent Is Nothing Then Return Nothing

			Dim retEvent As MouseEvent = Nothing

			If [me].source Is component Then
				retEvent = dndArmedEvent
			End If ' else component has changed unexpectedly, so return null

			clearState()
			Return retEvent
		End Function

		''' <summary>
		''' Returns whether or not a drag gesture recognition is ongoing.
		''' </summary>
		Private Function mouseDraggedImpl(ByVal [me] As MouseEvent, ByVal bd As BeforeDrag) As Boolean
			' no recognition is in progress 
			If dndArmedEvent Is Nothing Then Return False

			' component has changed unexpectedly, so bail 
			If [me].source IsNot component Then
				clearState()
				Return False
			End If

			Dim dx As Integer = Math.Abs([me].x - dndArmedEvent.x)
			Dim dy As Integer = Math.Abs([me].y - dndArmedEvent.y)
			If (dx > motionThreshold) OrElse (dy > motionThreshold) Then
				Dim th As TransferHandler = component.transferHandler
				Dim action As Integer = mapDragOperationFromModifiers([me], th)
				If action <> TransferHandler.NONE Then
					' notify the BeforeDrag instance 
					If bd IsNot Nothing Then bd.dragStarting(dndArmedEvent)
					th.exportAsDrag(component, dndArmedEvent, action)
					clearState()
				End If
			End If

			Return True
		End Function
	End Class

End Namespace
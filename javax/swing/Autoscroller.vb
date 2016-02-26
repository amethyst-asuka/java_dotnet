'
' * Copyright (c) 1997, 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing


	''' <summary>
	''' Autoscroller is responsible for generating synthetic mouse dragged
	''' events. It is the responsibility of the Component (or its MouseListeners)
	''' that receive the events to do the actual scrolling in response to the
	''' mouse dragged events.
	''' 
	''' @author Dave Moore
	''' @author Scott Violet
	''' </summary>
	Friend Class Autoscroller
		Implements ActionListener

		''' <summary>
		''' Global Autoscroller.
		''' </summary>
		Private Shared sharedInstance As New Autoscroller

		' As there can only ever be one autoscroller active these fields are
		' static. The Timer is recreated as necessary to target the appropriate
		' Autoscroller instance.
		Private Shared [event] As MouseEvent
		Private Shared timer As Timer
		Private Shared component As JComponent

		'
		' The public API, all methods are cover methods for an instance method
		'
		''' <summary>
		''' Stops autoscroll events from happening on the specified component.
		''' </summary>
		Public Shared Sub [stop](ByVal c As JComponent)
			sharedInstance._stop(c)
		End Sub

		''' <summary>
		''' Stops autoscroll events from happening on the specified component.
		''' </summary>
		Public Shared Function isRunning(ByVal c As JComponent) As Boolean
			Return sharedInstance._isRunning(c)
		End Function

		''' <summary>
		''' Invoked when a mouse dragged event occurs, will start the autoscroller
		''' if necessary.
		''' </summary>
		Public Shared Sub processMouseDragged(ByVal e As MouseEvent)
			sharedInstance._processMouseDragged(e)
		End Sub


		Friend Sub New()
		End Sub

		''' <summary>
		''' Starts the timer targeting the passed in component.
		''' </summary>
		Private Sub start(ByVal c As JComponent, ByVal e As MouseEvent)
			Dim screenLocation As Point = c.locationOnScreen

			If component IsNot c Then _stop(component)
			component = c
			[event] = New MouseEvent(component, e.iD, e.when, e.modifiers, e.x + screenLocation.x, e.y + screenLocation.y, e.xOnScreen, e.yOnScreen, e.clickCount, e.popupTrigger, MouseEvent.NOBUTTON)

			If timer Is Nothing Then timer = New Timer(100, Me)

			If Not timer.running Then timer.start()
		End Sub

		'
		' Methods mirror the public static API
		'

		''' <summary>
		''' Stops scrolling for the passed in widget.
		''' </summary>
		Private Sub _stop(ByVal c As JComponent)
			If component Is c Then
				If timer IsNot Nothing Then timer.stop()
				timer = Nothing
				[event] = Nothing
				component = Nothing
			End If
		End Sub

		''' <summary>
		''' Returns true if autoscrolling is currently running for the specified
		''' widget.
		''' </summary>
		Private Function _isRunning(ByVal c As JComponent) As Boolean
			Return (c Is component AndAlso timer IsNot Nothing AndAlso timer.running)
		End Function

		''' <summary>
		''' MouseListener method, invokes start/stop as necessary.
		''' </summary>
		Private Sub _processMouseDragged(ByVal e As MouseEvent)
			Dim component As JComponent = CType(e.component, JComponent)
			Dim [stop] As Boolean = True
			If component.showing Then
				Dim visibleRect As Rectangle = component.visibleRect
				[stop] = visibleRect.contains(e.x, e.y)
			End If
			If [stop] Then
				_stop(component)
			Else
				start(component, e)
			End If
		End Sub

		'
		' ActionListener
		'
		''' <summary>
		''' ActionListener method. Invoked when the Timer fires. This will scroll
		''' if necessary.
		''' </summary>
		Public Overridable Sub actionPerformed(ByVal x As ActionEvent)
			Dim component As JComponent = Autoscroller.component

			If component Is Nothing OrElse (Not component.showing) OrElse ([event] Is Nothing) Then
				_stop(component)
				Return
			End If
			Dim screenLocation As Point = component.locationOnScreen
			Dim e As New MouseEvent(component, [event].iD, [event].when, [event].modifiers, [event].x - screenLocation.x, [event].y - screenLocation.y, [event].xOnScreen, [event].yOnScreen, [event].clickCount, [event].popupTrigger, MouseEvent.NOBUTTON)
			component.superProcessMouseMotionEvent(e)
		End Sub

	End Class

End Namespace
'
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

'
' *
' *
' *
' *
' *
' * Copyright (c) 2000 World Wide Web Consortium,
' * (Massachusetts Institute of Technology, Institut National de
' * Recherche en Informatique et en Automatique, Keio University). All
' * Rights Reserved. This program is distributed under the W3C's Software
' * Intellectual Property License. This program is distributed in the
' * hope that it will be useful, but WITHOUT ANY WARRANTY; without even
' * the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
' * PURPOSE.
' * See W3C License http://www.w3.org/Consortium/Legal/ for more details.
' 

Namespace org.w3c.dom.events


	''' <summary>
	''' The <code>MouseEvent</code> interface provides specific contextual
	''' information associated with Mouse events.
	''' <p>The <code>detail</code> attribute inherited from <code>UIEvent</code>
	''' indicates the number of times a mouse button has been pressed and
	''' released over the same screen location during a user action. The
	''' attribute value is 1 when the user begins this action and increments by 1
	''' for each full sequence of pressing and releasing. If the user moves the
	''' mouse between the mousedown and mouseup the value will be set to 0,
	''' indicating that no click is occurring.
	''' <p>In the case of nested elements mouse events are always targeted at the
	''' most deeply nested element. Ancestors of the targeted element may use
	''' bubbling to obtain notification of mouse events which occur within its
	''' descendent elements.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/REC-DOM-Level-2-Events-20001113'>Document Object Model (DOM) Level 2 Events Specification</a>.
	''' @since DOM Level 2
	''' </summary>
	Public Interface MouseEvent
		Inherits UIEvent

		''' <summary>
		''' The horizontal coordinate at which the event occurred relative to the
		''' origin of the screen coordinate system.
		''' </summary>
		ReadOnly Property screenX As Integer

		''' <summary>
		''' The vertical coordinate at which the event occurred relative to the
		''' origin of the screen coordinate system.
		''' </summary>
		ReadOnly Property screenY As Integer

		''' <summary>
		''' The horizontal coordinate at which the event occurred relative to the
		''' DOM implementation's client area.
		''' </summary>
		ReadOnly Property clientX As Integer

		''' <summary>
		''' The vertical coordinate at which the event occurred relative to the DOM
		''' implementation's client area.
		''' </summary>
		ReadOnly Property clientY As Integer

		''' <summary>
		''' Used to indicate whether the 'ctrl' key was depressed during the firing
		''' of the event.
		''' </summary>
		ReadOnly Property ctrlKey As Boolean

		''' <summary>
		''' Used to indicate whether the 'shift' key was depressed during the
		''' firing of the event.
		''' </summary>
		ReadOnly Property shiftKey As Boolean

		''' <summary>
		''' Used to indicate whether the 'alt' key was depressed during the firing
		''' of the event. On some platforms this key may map to an alternative
		''' key name.
		''' </summary>
		ReadOnly Property altKey As Boolean

		''' <summary>
		''' Used to indicate whether the 'meta' key was depressed during the firing
		''' of the event. On some platforms this key may map to an alternative
		''' key name.
		''' </summary>
		ReadOnly Property metaKey As Boolean

		''' <summary>
		''' During mouse events caused by the depression or release of a mouse
		''' button, <code>button</code> is used to indicate which mouse button
		''' changed state. The values for <code>button</code> range from zero to
		''' indicate the left button of the mouse, one to indicate the middle
		''' button if present, and two to indicate the right button. For mice
		''' configured for left handed use in which the button actions are
		''' reversed the values are instead read from right to left.
		''' </summary>
		ReadOnly Property button As Short

		''' <summary>
		''' Used to identify a secondary <code>EventTarget</code> related to a UI
		''' event. Currently this attribute is used with the mouseover event to
		''' indicate the <code>EventTarget</code> which the pointing device
		''' exited and with the mouseout event to indicate the
		''' <code>EventTarget</code> which the pointing device entered.
		''' </summary>
		ReadOnly Property relatedTarget As EventTarget

		''' <summary>
		''' The <code>initMouseEvent</code> method is used to initialize the value
		''' of a <code>MouseEvent</code> created through the
		''' <code>DocumentEvent</code> interface. This method may only be called
		''' before the <code>MouseEvent</code> has been dispatched via the
		''' <code>dispatchEvent</code> method, though it may be called multiple
		''' times during that phase if necessary. If called multiple times, the
		''' final invocation takes precedence. </summary>
		''' <param name="typeArg"> Specifies the event type. </param>
		''' <param name="canBubbleArg"> Specifies whether or not the event can bubble. </param>
		''' <param name="cancelableArg"> Specifies whether or not the event's default
		'''   action can be prevented. </param>
		''' <param name="viewArg"> Specifies the <code>Event</code>'s
		'''   <code>AbstractView</code>. </param>
		''' <param name="detailArg"> Specifies the <code>Event</code>'s mouse click count. </param>
		''' <param name="screenXArg"> Specifies the <code>Event</code>'s screen x
		'''   coordinate </param>
		''' <param name="screenYArg"> Specifies the <code>Event</code>'s screen y
		'''   coordinate </param>
		''' <param name="clientXArg"> Specifies the <code>Event</code>'s client x
		'''   coordinate </param>
		''' <param name="clientYArg"> Specifies the <code>Event</code>'s client y
		'''   coordinate </param>
		''' <param name="ctrlKeyArg"> Specifies whether or not control key was depressed
		'''   during the <code>Event</code>. </param>
		''' <param name="altKeyArg"> Specifies whether or not alt key was depressed during
		'''   the <code>Event</code>. </param>
		''' <param name="shiftKeyArg"> Specifies whether or not shift key was depressed
		'''   during the <code>Event</code>. </param>
		''' <param name="metaKeyArg"> Specifies whether or not meta key was depressed
		'''   during the <code>Event</code>. </param>
		''' <param name="buttonArg"> Specifies the <code>Event</code>'s mouse button. </param>
		''' <param name="relatedTargetArg"> Specifies the <code>Event</code>'s related
		'''   <code>EventTarget</code>. </param>
		Sub initMouseEvent(ByVal typeArg As String, ByVal canBubbleArg As Boolean, ByVal cancelableArg As Boolean, ByVal viewArg As org.w3c.dom.views.AbstractView, ByVal detailArg As Integer, ByVal screenXArg As Integer, ByVal screenYArg As Integer, ByVal clientXArg As Integer, ByVal clientYArg As Integer, ByVal ctrlKeyArg As Boolean, ByVal altKeyArg As Boolean, ByVal shiftKeyArg As Boolean, ByVal metaKeyArg As Boolean, ByVal buttonArg As Short, ByVal relatedTargetArg As EventTarget)

	End Interface

End Namespace
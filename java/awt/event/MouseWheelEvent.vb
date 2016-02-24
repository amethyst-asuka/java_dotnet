'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.event



	''' <summary>
	''' An event which indicates that the mouse wheel was rotated in a component.
	''' <P>
	''' A wheel mouse is a mouse which has a wheel in place of the middle button.
	''' This wheel can be rotated towards or away from the user.  Mouse wheels are
	''' most often used for scrolling, though other uses are possible.
	''' <P>
	''' A MouseWheelEvent object is passed to every <code>MouseWheelListener</code>
	''' object which registered to receive the "interesting" mouse events using the
	''' component's <code>addMouseWheelListener</code> method.  Each such listener
	''' object gets a <code>MouseEvent</code> containing the mouse event.
	''' <P>
	''' Due to the mouse wheel's special relationship to scrolling Components,
	''' MouseWheelEvents are delivered somewhat differently than other MouseEvents.
	''' This is because while other MouseEvents usually affect a change on
	''' the Component directly under the mouse
	''' cursor (for instance, when clicking a button), MouseWheelEvents often have
	''' an effect away from the mouse cursor (moving the wheel while
	''' over a Component inside a ScrollPane should scroll one of the
	''' Scrollbars on the ScrollPane).
	''' <P>
	''' MouseWheelEvents start delivery from the Component underneath the
	''' mouse cursor.  If MouseWheelEvents are not enabled on the
	''' Component, the event is delivered to the first ancestor
	''' Container with MouseWheelEvents enabled.  This will usually be
	''' a ScrollPane with wheel scrolling enabled.  The source
	''' Component and x,y coordinates will be relative to the event's
	''' final destination (the ScrollPane).  This allows a complex
	''' GUI to be installed without modification into a ScrollPane, and
	''' for all MouseWheelEvents to be delivered to the ScrollPane for
	''' scrolling.
	''' <P>
	''' Some AWT Components are implemented using native widgets which
	''' display their own scrollbars and handle their own scrolling.
	''' The particular Components for which this is true will vary from
	''' platform to platform.  When the mouse wheel is
	''' moved over one of these Components, the event is delivered straight to
	''' the native widget, and not propagated to ancestors.
	''' <P>
	''' Platforms offer customization of the amount of scrolling that
	''' should take place when the mouse wheel is moved.  The two most
	''' common settings are to scroll a certain number of "units"
	''' (commonly lines of text in a text-based component) or an entire "block"
	''' (similar to page-up/page-down).  The MouseWheelEvent offers
	''' methods for conforming to the underlying platform settings.  These
	''' platform settings can be changed at any time by the user.  MouseWheelEvents
	''' reflect the most recent settings.
	''' <P>
	''' The <code>MouseWheelEvent</code> class includes methods for
	''' getting the number of "clicks" by which the mouse wheel is rotated.
	''' The <seealso cref="#getWheelRotation"/> method returns the integer number
	''' of "clicks" corresponding to the number of notches by which the wheel was
	''' rotated. In addition to this method, the <code>MouseWheelEvent</code>
	''' class provides the <seealso cref="#getPreciseWheelRotation"/> method which returns
	''' a double number of "clicks" in case a partial rotation occurred.
	''' The <seealso cref="#getPreciseWheelRotation"/> method is useful if a mouse supports
	''' a high-resolution wheel, such as a freely rotating wheel with no
	''' notches. Applications can benefit by using this method to process
	''' mouse wheel events more precisely, and thus, making visual perception
	''' smoother.
	''' 
	''' @author Brent Christian </summary>
	''' <seealso cref= MouseWheelListener </seealso>
	''' <seealso cref= java.awt.ScrollPane </seealso>
	''' <seealso cref= java.awt.ScrollPane#setWheelScrollingEnabled(boolean) </seealso>
	''' <seealso cref= javax.swing.JScrollPane </seealso>
	''' <seealso cref= javax.swing.JScrollPane#setWheelScrollingEnabled(boolean)
	''' @since 1.4 </seealso>

	Public Class MouseWheelEvent
		Inherits MouseEvent

		''' <summary>
		''' Constant representing scrolling by "units" (like scrolling with the
		''' arrow keys)
		''' </summary>
		''' <seealso cref= #getScrollType </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const WHEEL_UNIT_SCROLL As Integer = 0

		''' <summary>
		''' Constant representing scrolling by a "block" (like scrolling
		''' with page-up, page-down keys)
		''' </summary>
		''' <seealso cref= #getScrollType </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const WHEEL_BLOCK_SCROLL As Integer = 1

		''' <summary>
		''' Indicates what sort of scrolling should take place in response to this
		''' event, based on platform settings.  Legal values are:
		''' <ul>
		''' <li> WHEEL_UNIT_SCROLL
		''' <li> WHEEL_BLOCK_SCROLL
		''' </ul>
		''' </summary>
		''' <seealso cref= #getScrollType </seealso>
		Friend scrollType As Integer

		''' <summary>
		''' Only valid for scrollType WHEEL_UNIT_SCROLL.
		''' Indicates number of units that should be scrolled per
		''' click of mouse wheel rotation, based on platform settings.
		''' </summary>
		''' <seealso cref= #getScrollAmount </seealso>
		''' <seealso cref= #getScrollType </seealso>
		Friend scrollAmount As Integer

		''' <summary>
		''' Indicates how far the mouse wheel was rotated.
		''' </summary>
		''' <seealso cref= #getWheelRotation </seealso>
		Friend wheelRotation As Integer

		''' <summary>
		''' Indicates how far the mouse wheel was rotated.
		''' </summary>
		''' <seealso cref= #getPreciseWheelRotation </seealso>
		Friend preciseWheelRotation As Double

	'    
	'     * serialVersionUID
	'     

		Private Shadows Const serialVersionUID As Long = 6459879390515399677L

		''' <summary>
		''' Constructs a <code>MouseWheelEvent</code> object with the
		''' specified source component, type, modifiers, coordinates,
		''' scroll type, scroll amount, and wheel rotation.
		''' <p>Absolute coordinates xAbs and yAbs are set to source's location on screen plus
		''' relative coordinates x and y. xAbs and yAbs are set to zero if the source is not showing.
		''' <p>Note that passing in an invalid <code>id</code> results in
		''' unspecified behavior. This method throws an
		''' <code>IllegalArgumentException</code> if <code>source</code>
		''' is <code>null</code>.
		''' </summary>
		''' <param name="source">         the <code>Component</code> that originated
		'''                       the event </param>
		''' <param name="id">             the integer that identifies the event </param>
		''' <param name="when">           a long that gives the time the event occurred </param>
		''' <param name="modifiers">      the modifier keys down during event
		'''                       (shift, ctrl, alt, meta) </param>
		''' <param name="x">              the horizontal x coordinate for the mouse location </param>
		''' <param name="y">              the vertical y coordinate for the mouse location </param>
		''' <param name="clickCount">     the number of mouse clicks associated with event </param>
		''' <param name="popupTrigger">   a boolean, true if this event is a trigger for a
		'''                       popup-menu </param>
		''' <param name="scrollType">     the type of scrolling which should take place in
		'''                       response to this event;  valid values are
		'''                       <code>WHEEL_UNIT_SCROLL</code> and
		'''                       <code>WHEEL_BLOCK_SCROLL</code> </param>
		''' <param name="scrollAmount">  for scrollType <code>WHEEL_UNIT_SCROLL</code>,
		'''                       the number of units to be scrolled </param>
		''' <param name="wheelRotation">  the integer number of "clicks" by which the mouse
		'''                       wheel was rotated
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		''' <seealso cref= MouseEvent#MouseEvent(java.awt.Component, int, long, int, int, int, int, boolean) </seealso>
		''' <seealso cref= MouseEvent#MouseEvent(java.awt.Component, int, long, int, int, int, int, int, int, boolean, int) </seealso>
		Public Sub New(ByVal source As java.awt.Component, ByVal id As Integer, ByVal [when] As Long, ByVal modifiers As Integer, ByVal x As Integer, ByVal y As Integer, ByVal clickCount As Integer, ByVal popupTrigger As Boolean, ByVal scrollType As Integer, ByVal scrollAmount As Integer, ByVal wheelRotation As Integer)

			Me.New(source, id, [when], modifiers, x, y, 0, 0, clickCount, popupTrigger, scrollType, scrollAmount, wheelRotation)
		End Sub

		''' <summary>
		''' Constructs a <code>MouseWheelEvent</code> object with the
		''' specified source component, type, modifiers, coordinates,
		''' absolute coordinates, scroll type, scroll amount, and wheel rotation.
		''' <p>Note that passing in an invalid <code>id</code> results in
		''' unspecified behavior. This method throws an
		''' <code>IllegalArgumentException</code> if <code>source</code>
		''' is <code>null</code>.<p>
		''' Even if inconsistent values for relative and absolute coordinates are
		''' passed to the constructor, the MouseWheelEvent instance is still
		''' created and no exception is thrown.
		''' </summary>
		''' <param name="source">         the <code>Component</code> that originated
		'''                       the event </param>
		''' <param name="id">             the integer that identifies the event </param>
		''' <param name="when">           a long that gives the time the event occurred </param>
		''' <param name="modifiers">      the modifier keys down during event
		'''                       (shift, ctrl, alt, meta) </param>
		''' <param name="x">              the horizontal x coordinate for the mouse location </param>
		''' <param name="y">              the vertical y coordinate for the mouse location </param>
		''' <param name="xAbs">           the absolute horizontal x coordinate for the mouse location </param>
		''' <param name="yAbs">           the absolute vertical y coordinate for the mouse location </param>
		''' <param name="clickCount">     the number of mouse clicks associated with event </param>
		''' <param name="popupTrigger">   a boolean, true if this event is a trigger for a
		'''                       popup-menu </param>
		''' <param name="scrollType">     the type of scrolling which should take place in
		'''                       response to this event;  valid values are
		'''                       <code>WHEEL_UNIT_SCROLL</code> and
		'''                       <code>WHEEL_BLOCK_SCROLL</code> </param>
		''' <param name="scrollAmount">  for scrollType <code>WHEEL_UNIT_SCROLL</code>,
		'''                       the number of units to be scrolled </param>
		''' <param name="wheelRotation">  the integer number of "clicks" by which the mouse
		'''                       wheel was rotated
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		''' <seealso cref= MouseEvent#MouseEvent(java.awt.Component, int, long, int, int, int, int, boolean) </seealso>
		''' <seealso cref= MouseEvent#MouseEvent(java.awt.Component, int, long, int, int, int, int, int, int, boolean, int)
		''' @since 1.6 </seealso>
		Public Sub New(ByVal source As java.awt.Component, ByVal id As Integer, ByVal [when] As Long, ByVal modifiers As Integer, ByVal x As Integer, ByVal y As Integer, ByVal xAbs As Integer, ByVal yAbs As Integer, ByVal clickCount As Integer, ByVal popupTrigger As Boolean, ByVal scrollType As Integer, ByVal scrollAmount As Integer, ByVal wheelRotation As Integer)

			Me.New(source, id, [when], modifiers, x, y, xAbs, yAbs, clickCount, popupTrigger, scrollType, scrollAmount, wheelRotation, wheelRotation)

		End Sub


		''' <summary>
		''' Constructs a <code>MouseWheelEvent</code> object with the specified
		''' source component, type, modifiers, coordinates, absolute coordinates,
		''' scroll type, scroll amount, and wheel rotation.
		''' <p>Note that passing in an invalid <code>id</code> parameter results
		''' in unspecified behavior. This method throws an
		''' <code>IllegalArgumentException</code> if <code>source</code> equals
		''' <code>null</code>.
		''' <p>Even if inconsistent values for relative and absolute coordinates
		''' are passed to the constructor, a <code>MouseWheelEvent</code> instance
		''' is still created and no exception is thrown.
		''' </summary>
		''' <param name="source">         the <code>Component</code> that originated the event </param>
		''' <param name="id">             the integer value that identifies the event </param>
		''' <param name="when">           a long value that gives the time when the event occurred </param>
		''' <param name="modifiers">      the modifier keys down during event
		'''                       (shift, ctrl, alt, meta) </param>
		''' <param name="x">              the horizontal <code>x</code> coordinate for the
		'''                       mouse location </param>
		''' <param name="y">              the vertical <code>y</code> coordinate for the
		'''                       mouse location </param>
		''' <param name="xAbs">           the absolute horizontal <code>x</code> coordinate for
		'''                       the mouse location </param>
		''' <param name="yAbs">           the absolute vertical <code>y</code> coordinate for
		'''                       the mouse location </param>
		''' <param name="clickCount">     the number of mouse clicks associated with the event </param>
		''' <param name="popupTrigger">   a boolean value, <code>true</code> if this event is a trigger
		'''                       for a popup-menu </param>
		''' <param name="scrollType">     the type of scrolling which should take place in
		'''                       response to this event;  valid values are
		'''                       <code>WHEEL_UNIT_SCROLL</code> and
		'''                       <code>WHEEL_BLOCK_SCROLL</code> </param>
		''' <param name="scrollAmount">  for scrollType <code>WHEEL_UNIT_SCROLL</code>,
		'''                       the number of units to be scrolled </param>
		''' <param name="wheelRotation">  the integer number of "clicks" by which the mouse wheel
		'''                       was rotated </param>
		''' <param name="preciseWheelRotation"> the double number of "clicks" by which the mouse wheel
		'''                       was rotated
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		''' <seealso cref= MouseEvent#MouseEvent(java.awt.Component, int, long, int, int, int, int, boolean) </seealso>
		''' <seealso cref= MouseEvent#MouseEvent(java.awt.Component, int, long, int, int, int, int, int, int, boolean, int)
		''' @since 1.7 </seealso>
		Public Sub New(ByVal source As java.awt.Component, ByVal id As Integer, ByVal [when] As Long, ByVal modifiers As Integer, ByVal x As Integer, ByVal y As Integer, ByVal xAbs As Integer, ByVal yAbs As Integer, ByVal clickCount As Integer, ByVal popupTrigger As Boolean, ByVal scrollType As Integer, ByVal scrollAmount As Integer, ByVal wheelRotation As Integer, ByVal preciseWheelRotation As Double)

			MyBase.New(source, id, [when], modifiers, x, y, xAbs, yAbs, clickCount, popupTrigger, MouseEvent.NOBUTTON)

			Me.scrollType = scrollType
			Me.scrollAmount = scrollAmount
			Me.wheelRotation = wheelRotation
			Me.preciseWheelRotation = preciseWheelRotation

		End Sub

		''' <summary>
		''' Returns the type of scrolling that should take place in response to this
		''' event.  This is determined by the native platform.  Legal values are:
		''' <ul>
		''' <li> MouseWheelEvent.WHEEL_UNIT_SCROLL
		''' <li> MouseWheelEvent.WHEEL_BLOCK_SCROLL
		''' </ul>
		''' </summary>
		''' <returns> either MouseWheelEvent.WHEEL_UNIT_SCROLL or
		'''  MouseWheelEvent.WHEEL_BLOCK_SCROLL, depending on the configuration of
		'''  the native platform. </returns>
		''' <seealso cref= java.awt.Adjustable#getUnitIncrement </seealso>
		''' <seealso cref= java.awt.Adjustable#getBlockIncrement </seealso>
		''' <seealso cref= javax.swing.Scrollable#getScrollableUnitIncrement </seealso>
		''' <seealso cref= javax.swing.Scrollable#getScrollableBlockIncrement </seealso>
		Public Overridable Property scrollType As Integer
			Get
				Return scrollType
			End Get
		End Property

		''' <summary>
		''' Returns the number of units that should be scrolled per
		''' click of mouse wheel rotation.
		''' Only valid if <code>getScrollType</code> returns
		''' <code>MouseWheelEvent.WHEEL_UNIT_SCROLL</code>
		''' </summary>
		''' <returns> number of units to scroll, or an undefined value if
		'''  <code>getScrollType</code> returns
		'''  <code>MouseWheelEvent.WHEEL_BLOCK_SCROLL</code> </returns>
		''' <seealso cref= #getScrollType </seealso>
		Public Overridable Property scrollAmount As Integer
			Get
				Return scrollAmount
			End Get
		End Property

		''' <summary>
		''' Returns the number of "clicks" the mouse wheel was rotated, as an integer.
		''' A partial rotation may occur if the mouse supports a high-resolution wheel.
		''' In this case, the method returns zero until a full "click" has been accumulated.
		''' </summary>
		''' <returns> negative values if the mouse wheel was rotated up/away from
		''' the user, and positive values if the mouse wheel was rotated down/
		''' towards the user </returns>
		''' <seealso cref= #getPreciseWheelRotation </seealso>
		Public Overridable Property wheelRotation As Integer
			Get
				Return wheelRotation
			End Get
		End Property

		''' <summary>
		''' Returns the number of "clicks" the mouse wheel was rotated, as a double.
		''' A partial rotation may occur if the mouse supports a high-resolution wheel.
		''' In this case, the return value will include a fractional "click".
		''' </summary>
		''' <returns> negative values if the mouse wheel was rotated up or away from
		''' the user, and positive values if the mouse wheel was rotated down or
		''' towards the user </returns>
		''' <seealso cref= #getWheelRotation
		''' @since 1.7 </seealso>
		Public Overridable Property preciseWheelRotation As Double
			Get
				Return preciseWheelRotation
			End Get
		End Property

		''' <summary>
		''' This is a convenience method to aid in the implementation of
		''' the common-case MouseWheelListener - to scroll a ScrollPane or
		''' JScrollPane by an amount which conforms to the platform settings.
		''' (Note, however, that <code>ScrollPane</code> and
		''' <code>JScrollPane</code> already have this functionality built in.)
		''' <P>
		''' This method returns the number of units to scroll when scroll type is
		''' MouseWheelEvent.WHEEL_UNIT_SCROLL, and should only be called if
		''' <code>getScrollType</code> returns MouseWheelEvent.WHEEL_UNIT_SCROLL.
		''' <P>
		''' Direction of scroll, amount of wheel movement,
		''' and platform settings for wheel scrolling are all accounted for.
		''' This method does not and cannot take into account value of the
		''' Adjustable/Scrollable unit increment, as this will vary among
		''' scrolling components.
		''' <P>
		''' A simplified example of how this method might be used in a
		''' listener:
		''' <pre>
		'''  mouseWheelMoved(MouseWheelEvent event) {
		'''      ScrollPane sp = getScrollPaneFromSomewhere();
		'''      Adjustable adj = sp.getVAdjustable()
		'''      if (MouseWheelEvent.getScrollType() == WHEEL_UNIT_SCROLL) {
		'''          int totalScrollAmount =
		'''              event.getUnitsToScroll() *
		'''              adj.getUnitIncrement();
		'''          adj.setValue(adj.getValue() + totalScrollAmount);
		'''      }
		'''  }
		''' </pre>
		''' </summary>
		''' <returns> the number of units to scroll based on the direction and amount
		'''  of mouse wheel rotation, and on the wheel scrolling settings of the
		'''  native platform </returns>
		''' <seealso cref= #getScrollType </seealso>
		''' <seealso cref= #getScrollAmount </seealso>
		''' <seealso cref= MouseWheelListener </seealso>
		''' <seealso cref= java.awt.Adjustable </seealso>
		''' <seealso cref= java.awt.Adjustable#getUnitIncrement </seealso>
		''' <seealso cref= javax.swing.Scrollable </seealso>
		''' <seealso cref= javax.swing.Scrollable#getScrollableUnitIncrement </seealso>
		''' <seealso cref= java.awt.ScrollPane </seealso>
		''' <seealso cref= java.awt.ScrollPane#setWheelScrollingEnabled </seealso>
		''' <seealso cref= javax.swing.JScrollPane </seealso>
		''' <seealso cref= javax.swing.JScrollPane#setWheelScrollingEnabled </seealso>
		Public Overridable Property unitsToScroll As Integer
			Get
				Return scrollAmount * wheelRotation
			End Get
		End Property

		''' <summary>
		''' Returns a parameter string identifying this event.
		''' This method is useful for event-logging and for debugging.
		''' </summary>
		''' <returns> a string identifying the event and its attributes </returns>
		Public Overrides Function paramString() As String
			Dim scrollTypeStr As String = Nothing

			If scrollType = WHEEL_UNIT_SCROLL Then
				scrollTypeStr = "WHEEL_UNIT_SCROLL"
			ElseIf scrollType = WHEEL_BLOCK_SCROLL Then
				scrollTypeStr = "WHEEL_BLOCK_SCROLL"
			Else
				scrollTypeStr = "unknown scroll type"
			End If
			Return MyBase.paramString() & ",scrollType=" & scrollTypeStr & ",scrollAmount=" & scrollAmount & ",wheelRotation=" & wheelRotation & ",preciseWheelRotation=" & preciseWheelRotation
		End Function
	End Class

End Namespace
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
	''' The <code>UIEvent</code> interface provides specific contextual information
	''' associated with User Interface events.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/REC-DOM-Level-2-Events-20001113'>Document Object Model (DOM) Level 2 Events Specification</a>.
	''' @since DOM Level 2
	''' </summary>
	Public Interface UIEvent
		Inherits [Event]

		''' <summary>
		''' The <code>view</code> attribute identifies the <code>AbstractView</code>
		'''  from which the event was generated.
		''' </summary>
		ReadOnly Property view As org.w3c.dom.views.AbstractView

		''' <summary>
		''' Specifies some detail information about the <code>Event</code>,
		''' depending on the type of event.
		''' </summary>
		ReadOnly Property detail As Integer

		''' <summary>
		''' The <code>initUIEvent</code> method is used to initialize the value of
		''' a <code>UIEvent</code> created through the <code>DocumentEvent</code>
		''' interface. This method may only be called before the
		''' <code>UIEvent</code> has been dispatched via the
		''' <code>dispatchEvent</code> method, though it may be called multiple
		''' times during that phase if necessary. If called multiple times, the
		''' final invocation takes precedence. </summary>
		''' <param name="typeArg"> Specifies the event type. </param>
		''' <param name="canBubbleArg"> Specifies whether or not the event can bubble. </param>
		''' <param name="cancelableArg"> Specifies whether or not the event's default
		'''   action can be prevented. </param>
		''' <param name="viewArg"> Specifies the <code>Event</code>'s
		'''   <code>AbstractView</code>. </param>
		''' <param name="detailArg"> Specifies the <code>Event</code>'s detail. </param>
		Sub initUIEvent(ByVal typeArg As String, ByVal canBubbleArg As Boolean, ByVal cancelableArg As Boolean, ByVal viewArg As org.w3c.dom.views.AbstractView, ByVal detailArg As Integer)

	End Interface

End Namespace
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
	''' The <code>MutationEvent</code> interface provides specific contextual
	''' information associated with Mutation events.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/REC-DOM-Level-2-Events-20001113'>Document Object Model (DOM) Level 2 Events Specification</a>.
	''' @since DOM Level 2
	''' </summary>
	Public Interface MutationEvent
		Inherits [Event]

		' attrChangeType
		''' <summary>
		''' The <code>Attr</code> was modified in place.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short MODIFICATION = 1;
		''' <summary>
		''' The <code>Attr</code> was just added.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short ADDITION = 2;
		''' <summary>
		''' The <code>Attr</code> was just removed.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short REMOVAL = 3;

		''' <summary>
		'''  <code>relatedNode</code> is used to identify a secondary node related
		''' to a mutation event. For example, if a mutation event is dispatched
		''' to a node indicating that its parent has changed, the
		''' <code>relatedNode</code> is the changed parent. If an event is
		''' instead dispatched to a subtree indicating a node was changed within
		''' it, the <code>relatedNode</code> is the changed node. In the case of
		''' the DOMAttrModified event it indicates the <code>Attr</code> node
		''' which was modified, added, or removed.
		''' </summary>
		ReadOnly Property relatedNode As org.w3c.dom.Node

		''' <summary>
		'''  <code>prevValue</code> indicates the previous value of the
		''' <code>Attr</code> node in DOMAttrModified events, and of the
		''' <code>CharacterData</code> node in DOMCharacterDataModified events.
		''' </summary>
		ReadOnly Property prevValue As String

		''' <summary>
		'''  <code>newValue</code> indicates the new value of the <code>Attr</code>
		''' node in DOMAttrModified events, and of the <code>CharacterData</code>
		''' node in DOMCharacterDataModified events.
		''' </summary>
		ReadOnly Property newValue As String

		''' <summary>
		'''  <code>attrName</code> indicates the name of the changed
		''' <code>Attr</code> node in a DOMAttrModified event.
		''' </summary>
		ReadOnly Property attrName As String

		''' <summary>
		'''  <code>attrChange</code> indicates the type of change which triggered
		''' the DOMAttrModified event. The values can be <code>MODIFICATION</code>
		''' , <code>ADDITION</code>, or <code>REMOVAL</code>.
		''' </summary>
		ReadOnly Property attrChange As Short

		''' <summary>
		''' The <code>initMutationEvent</code> method is used to initialize the
		''' value of a <code>MutationEvent</code> created through the
		''' <code>DocumentEvent</code> interface. This method may only be called
		''' before the <code>MutationEvent</code> has been dispatched via the
		''' <code>dispatchEvent</code> method, though it may be called multiple
		''' times during that phase if necessary. If called multiple times, the
		''' final invocation takes precedence. </summary>
		''' <param name="typeArg"> Specifies the event type. </param>
		''' <param name="canBubbleArg"> Specifies whether or not the event can bubble. </param>
		''' <param name="cancelableArg"> Specifies whether or not the event's default
		'''   action can be prevented. </param>
		''' <param name="relatedNodeArg"> Specifies the <code>Event</code>'s related Node. </param>
		''' <param name="prevValueArg"> Specifies the <code>Event</code>'s
		'''   <code>prevValue</code> attribute. This value may be null. </param>
		''' <param name="newValueArg"> Specifies the <code>Event</code>'s
		'''   <code>newValue</code> attribute. This value may be null. </param>
		''' <param name="attrNameArg"> Specifies the <code>Event</code>'s
		'''   <code>attrName</code> attribute. This value may be null. </param>
		''' <param name="attrChangeArg"> Specifies the <code>Event</code>'s
		'''   <code>attrChange</code> attribute </param>
		Sub initMutationEvent(ByVal typeArg As String, ByVal canBubbleArg As Boolean, ByVal cancelableArg As Boolean, ByVal relatedNodeArg As org.w3c.dom.Node, ByVal prevValueArg As String, ByVal newValueArg As String, ByVal attrNameArg As String, ByVal attrChangeArg As Short)

	End Interface

End Namespace
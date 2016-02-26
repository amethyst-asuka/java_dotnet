Imports javax.swing.undo
Imports javax.swing.text

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
Namespace javax.swing.event

	''' <summary>
	''' Interface for document change notifications.  This provides
	''' detailed information to Document observers about how the
	''' Document changed.  It provides high level information such
	''' as type of change and where it occurred, as well as the more
	''' detailed structural changes (What Elements were inserted and
	''' removed).
	''' 
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref= javax.swing.text.Document </seealso>
	''' <seealso cref= DocumentListener </seealso>
	Public Interface DocumentEvent

		''' <summary>
		''' Returns the offset within the document of the start
		''' of the change.
		''' </summary>
		''' <returns> the offset &gt;= 0 </returns>
		ReadOnly Property offset As Integer

		''' <summary>
		''' Returns the length of the change.
		''' </summary>
		''' <returns> the length &gt;= 0 </returns>
		ReadOnly Property length As Integer

		''' <summary>
		''' Gets the document that sourced the change event.
		''' </summary>
		''' <returns> the document </returns>
		ReadOnly Property document As Document

		''' <summary>
		''' Gets the type of event.
		''' </summary>
		''' <returns> the type </returns>
		Function [getType]() As EventType

		''' <summary>
		''' Gets the change information for the given element.
		''' The change information describes what elements were
		''' added and removed and the location.  If there were
		''' no changes, null is returned.
		''' <p>
		''' This method is for observers to discover the structural
		''' changes that were made.  This means that only elements
		''' that existed prior to the mutation (and still exist after
		''' the mutation) need to have ElementChange records.
		''' The changes made available need not be recursive.
		''' <p>
		''' For example, if the an element is removed from it's
		''' parent, this method should report that the parent
		''' changed and provide an ElementChange implementation
		''' that describes the change to the parent.  If the
		''' child element removed had children, these elements
		''' do not need to be reported as removed.
		''' <p>
		''' If an child element is insert into a parent element,
		''' the parent element should report a change.  If the
		''' child element also had elements inserted into it
		''' (grandchildren to the parent) these elements need
		''' not report change.
		''' </summary>
		''' <param name="elem"> the element </param>
		''' <returns> the change information, or null if the
		'''   element was not modified </returns>
		Function getChange(ByVal elem As Element) As ElementChange

		''' <summary>
		''' Enumeration for document event types
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public static final class EventType
	'	{
	'
	'		private EventType(String s)
	'		{
	'			typeString = s;
	'		}
	'
	'		''' <summary>
	'		''' Insert type.
	'		''' </summary>
	'		public static final EventType INSERT = New EventType("INSERT");
	'
	'		''' <summary>
	'		''' Remove type.
	'		''' </summary>
	'		public static final EventType REMOVE = New EventType("REMOVE");
	'
	'		''' <summary>
	'		''' Change type.
	'		''' </summary>
	'		public static final EventType CHANGE = New EventType("CHANGE");
	'
	'		''' <summary>
	'		''' Converts the type to a string.
	'		''' </summary>
	'		''' <returns> the string </returns>
	'		public String toString()
	'		{
	'			Return typeString;
	'		}
	'
	'		private String typeString;
	'	}

		''' <summary>
		''' Describes changes made to a specific element.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public interface ElementChange
	'	{
	'
	'		''' <summary>
	'		''' Returns the element represented.  This is the element
	'		''' that was changed.
	'		''' </summary>
	'		''' <returns> the element </returns>
	'		public Element getElement();
	'
	'		''' <summary>
	'		''' Fetches the index within the element represented.
	'		''' This is the location that children were added
	'		''' and/or removed.
	'		''' </summary>
	'		''' <returns> the index &gt;= 0 </returns>
	'		public int getIndex();
	'
	'		''' <summary>
	'		''' Gets the child elements that were removed from the
	'		''' given parent element.  The element array returned is
	'		''' sorted in the order that the elements used to lie in
	'		''' the document, and must be contiguous.
	'		''' </summary>
	'		''' <returns> the child elements </returns>
	'		public Element[] getChildrenRemoved();
	'
	'		''' <summary>
	'		''' Gets the child elements that were added to the given
	'		''' parent element.  The element array returned is in the
	'		''' order that the elements lie in the document, and must
	'		''' be contiguous.
	'		''' </summary>
	'		''' <returns> the child elements </returns>
	'		public Element[] getChildrenAdded();
	'
	'	}
	End Interface

End Namespace
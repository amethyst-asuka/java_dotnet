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
' * Copyright (c) 2009 by Oracle Corporation. All Rights Reserved.
' 

Namespace javax.xml.stream.util


	''' <summary>
	''' This interface defines a class that allows a user to register
	''' a way to allocate events given an XMLStreamReader.  An implementation
	''' is not required to use the XMLEventFactory implementation but this
	''' is recommended.  The XMLEventAllocator can be set on an XMLInputFactory
	''' using the property "javax.xml.stream.allocator"
	''' 
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved. </summary>
	''' <seealso cref= javax.xml.stream.XMLInputFactory </seealso>
	''' <seealso cref= javax.xml.stream.XMLEventFactory
	''' @since 1.6 </seealso>
	Public Interface XMLEventAllocator

	  ''' <summary>
	  ''' This method creates an instance of the XMLEventAllocator. This
	  ''' allows the XMLInputFactory to allocate a new instance per reader.
	  ''' </summary>
	  Function newInstance() As XMLEventAllocator

	  ''' <summary>
	  ''' This method allocates an event given the current
	  ''' state of the XMLStreamReader.  If this XMLEventAllocator
	  ''' does not have a one-to-one mapping between reader states
	  ''' and events this method will return null.  This method
	  ''' must not modify the state of the XMLStreamReader. </summary>
	  ''' <param name="reader"> The XMLStreamReader to allocate from </param>
	  ''' <returns> the event corresponding to the current reader state </returns>
	  Function allocate(ByVal reader As javax.xml.stream.XMLStreamReader) As javax.xml.stream.events.XMLEvent

	  ''' <summary>
	  ''' This method allocates an event or set of events
	  ''' given the current
	  ''' state of the XMLStreamReader and adds the event
	  ''' or set of events to the
	  ''' consumer that was passed in.  This method can be used
	  ''' to expand or contract reader states into event states.
	  ''' This method may modify the state of the XMLStreamReader. </summary>
	  ''' <param name="reader"> The XMLStreamReader to allocate from </param>
	  ''' <param name="consumer"> The XMLEventConsumer to add to. </param>
	  Sub allocate(ByVal reader As javax.xml.stream.XMLStreamReader, ByVal consumer As XMLEventConsumer)

	End Interface

End Namespace
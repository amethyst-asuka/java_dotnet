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

Namespace javax.xml.stream



	''' 
	''' <summary>
	''' This is the top level interface for parsing XML Events.  It provides
	''' the ability to peek at the next event and returns configuration
	''' information through the property interface.
	''' 
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved. </summary>
	''' <seealso cref= XMLInputFactory </seealso>
	''' <seealso cref= XMLEventWriter
	''' @since 1.6 </seealso>
	Public Interface XMLEventReader
		Inherits IEnumerator

	  ''' <summary>
	  ''' Get the next XMLEvent </summary>
	  ''' <seealso cref= XMLEvent </seealso>
	  ''' <exception cref="XMLStreamException"> if there is an error with the underlying XML. </exception>
	  ''' <exception cref="NoSuchElementException"> iteration has no more elements. </exception>
	  Function nextEvent() As javax.xml.stream.events.XMLEvent

	  ''' <summary>
	  ''' Check if there are more events.
	  ''' Returns true if there are more events and false otherwise. </summary>
	  ''' <returns> true if the event reader has more events, false otherwise </returns>
	  Function hasNext() As Boolean

	  ''' <summary>
	  ''' Check the next XMLEvent without reading it from the stream.
	  ''' Returns null if the stream is at EOF or has no more XMLEvents.
	  ''' A call to peek() will be equal to the next return of next(). </summary>
	  ''' <seealso cref= XMLEvent </seealso>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Function peek() As javax.xml.stream.events.XMLEvent

	  ''' <summary>
	  ''' Reads the content of a text-only element. Precondition:
	  ''' the current event is START_ELEMENT. Postcondition:
	  ''' The current event is the corresponding END_ELEMENT. </summary>
	  ''' <exception cref="XMLStreamException"> if the current event is not a START_ELEMENT
	  ''' or if a non text element is encountered </exception>
	  ReadOnly Property elementText As String

	  ''' <summary>
	  ''' Skips any insignificant space events until a START_ELEMENT or
	  ''' END_ELEMENT is reached. If anything other than space characters are
	  ''' encountered, an exception is thrown. This method should
	  ''' be used when processing element-only content because
	  ''' the parser is not able to recognize ignorable whitespace if
	  ''' the DTD is missing or not interpreted. </summary>
	  ''' <exception cref="XMLStreamException"> if anything other than space characters are encountered </exception>
	  Function nextTag() As javax.xml.stream.events.XMLEvent

	  ''' <summary>
	  ''' Get the value of a feature/property from the underlying implementation </summary>
	  ''' <param name="name"> The name of the property </param>
	  ''' <returns> The value of the property </returns>
	  ''' <exception cref="IllegalArgumentException"> if the property is not supported </exception>
	  Function getProperty(ByVal name As String) As Object

	  ''' <summary>
	  ''' Frees any resources associated with this Reader.  This method does not close the
	  ''' underlying input source. </summary>
	  ''' <exception cref="XMLStreamException"> if there are errors freeing associated resources </exception>
	  Sub close()
	End Interface

End Namespace
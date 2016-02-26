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

Namespace javax.xml.stream.events

	''' <summary>
	''' This is the base event interface for handling markup events.
	''' Events are value objects that are used to communicate the
	''' XML 1.0 InfoSet to the Application.  Events may be cached
	''' and referenced after the parse has completed.
	''' 
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved. </summary>
	''' <seealso cref= javax.xml.stream.XMLEventReader </seealso>
	''' <seealso cref= Characters </seealso>
	''' <seealso cref= ProcessingInstruction </seealso>
	''' <seealso cref= StartElement </seealso>
	''' <seealso cref= EndElement </seealso>
	''' <seealso cref= StartDocument </seealso>
	''' <seealso cref= EndDocument </seealso>
	''' <seealso cref= EntityReference </seealso>
	''' <seealso cref= EntityDeclaration </seealso>
	''' <seealso cref= NotationDeclaration
	''' @since 1.6 </seealso>
	Public Interface XMLEvent
		Inherits javax.xml.stream.XMLStreamConstants

	  ''' <summary>
	  ''' Returns an integer code for this event. </summary>
	  ''' <seealso cref= #START_ELEMENT </seealso>
	  ''' <seealso cref= #END_ELEMENT </seealso>
	  ''' <seealso cref= #CHARACTERS </seealso>
	  ''' <seealso cref= #ATTRIBUTE </seealso>
	  ''' <seealso cref= #NAMESPACE </seealso>
	  ''' <seealso cref= #PROCESSING_INSTRUCTION </seealso>
	  ''' <seealso cref= #COMMENT </seealso>
	  ''' <seealso cref= #START_DOCUMENT </seealso>
	  ''' <seealso cref= #END_DOCUMENT </seealso>
	  ''' <seealso cref= #DTD </seealso>
	  ReadOnly Property eventType As Integer

	  ''' <summary>
	  ''' Return the location of this event.  The Location
	  ''' returned from this method is non-volatile and
	  ''' will retain its information. </summary>
	  ''' <seealso cref= javax.xml.stream.Location </seealso>
	  ReadOnly Property location As javax.xml.stream.Location

	  ''' <summary>
	  ''' A utility function to check if this event is a StartElement. </summary>
	  ''' <seealso cref= StartElement </seealso>
	  ReadOnly Property startElement As Boolean

	  ''' <summary>
	  ''' A utility function to check if this event is an Attribute. </summary>
	  ''' <seealso cref= Attribute </seealso>
	  ReadOnly Property attribute As Boolean

	  ''' <summary>
	  ''' A utility function to check if this event is a Namespace. </summary>
	  ''' <seealso cref= Namespace </seealso>
	  ReadOnly Property [namespace] As Boolean


	  ''' <summary>
	  ''' A utility function to check if this event is a EndElement. </summary>
	  ''' <seealso cref= EndElement </seealso>
	  ReadOnly Property endElement As Boolean

	  ''' <summary>
	  ''' A utility function to check if this event is an EntityReference. </summary>
	  ''' <seealso cref= EntityReference </seealso>
	  ReadOnly Property entityReference As Boolean

	  ''' <summary>
	  ''' A utility function to check if this event is a ProcessingInstruction. </summary>
	  ''' <seealso cref= ProcessingInstruction </seealso>
	  ReadOnly Property processingInstruction As Boolean

	  ''' <summary>
	  ''' A utility function to check if this event is Characters. </summary>
	  ''' <seealso cref= Characters </seealso>
	  ReadOnly Property characters As Boolean

	  ''' <summary>
	  ''' A utility function to check if this event is a StartDocument. </summary>
	  ''' <seealso cref= StartDocument </seealso>
	  ReadOnly Property startDocument As Boolean

	  ''' <summary>
	  ''' A utility function to check if this event is an EndDocument. </summary>
	  ''' <seealso cref= EndDocument </seealso>
	  ReadOnly Property endDocument As Boolean

	  ''' <summary>
	  ''' Returns this event as a start element event, may result in
	  ''' a class cast exception if this event is not a start element.
	  ''' </summary>
	  Function asStartElement() As StartElement

	  ''' <summary>
	  ''' Returns this event as an end  element event, may result in
	  ''' a class cast exception if this event is not a end element.
	  ''' </summary>
	  Function asEndElement() As EndElement

	  ''' <summary>
	  ''' Returns this event as Characters, may result in
	  ''' a class cast exception if this event is not Characters.
	  ''' </summary>
	  Function asCharacters() As Characters

	  ''' <summary>
	  ''' This method is provided for implementations to provide
	  ''' optional type information about the associated event.
	  ''' It is optional and will return null if no information
	  ''' is available.
	  ''' </summary>
	  ReadOnly Property schemaType As javax.xml.namespace.QName

	  ''' <summary>
	  ''' This method will write the XMLEvent as per the XML 1.0 specification as Unicode characters.
	  ''' No indentation or whitespace should be outputted.
	  ''' 
	  ''' Any user defined event type SHALL have this method
	  ''' called when being written to on an output stream.
	  ''' Built in Event types MUST implement this method,
	  ''' but implementations MAY choose not call these methods
	  ''' for optimizations reasons when writing out built in
	  ''' Events to an output stream.
	  ''' The output generated MUST be equivalent in terms of the
	  ''' infoset expressed.
	  ''' </summary>
	  ''' <param name="writer"> The writer that will output the data </param>
	  ''' <exception cref="XMLStreamException"> if there is a fatal error writing the event </exception>
	  Sub writeAsEncodedUnicode(ByVal writer As java.io.Writer)

	End Interface

End Namespace
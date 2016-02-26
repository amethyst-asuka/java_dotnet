Imports javax.xml.stream.events

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
	''' This is the top level interface for writing XML documents.
	''' 
	''' Instances of this interface are not required to validate the
	''' form of the XML.
	''' 
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved. </summary>
	''' <seealso cref= XMLEventReader </seealso>
	''' <seealso cref= javax.xml.stream.events.XMLEvent </seealso>
	''' <seealso cref= javax.xml.stream.events.Characters </seealso>
	''' <seealso cref= javax.xml.stream.events.ProcessingInstruction </seealso>
	''' <seealso cref= javax.xml.stream.events.StartElement </seealso>
	''' <seealso cref= javax.xml.stream.events.EndElement
	''' @since 1.6 </seealso>
	Public Interface XMLEventWriter
		Inherits javax.xml.stream.util.XMLEventConsumer

	  ''' <summary>
	  ''' Writes any cached events to the underlying output mechanism </summary>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub flush()

	  ''' <summary>
	  ''' Frees any resources associated with this stream </summary>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub close()

	  ''' <summary>
	  ''' Add an event to the output stream
	  ''' Adding a START_ELEMENT will open a new namespace scope that
	  ''' will be closed when the corresponding END_ELEMENT is written.
	  ''' <table border="2" rules="all" cellpadding="4">
	  '''   <thead>
	  '''     <tr>
	  '''       <th align="center" colspan="2">
	  '''         Required and optional fields for events added to the writer
	  '''       </th>
	  '''     </tr>
	  '''   </thead>
	  '''   <tbody>
	  '''     <tr>
	  '''       <th>Event Type</th>
	  '''       <th>Required Fields</th>
	  '''       <th>Optional Fields</th>
	  '''       <th>Required Behavior</th>
	  '''     </tr>
	  '''     <tr>
	  '''       <td> START_ELEMENT  </td>
	  '''       <td> QName name </td>
	  '''       <td> namespaces , attributes </td>
	  '''       <td> A START_ELEMENT will be written by writing the name,
	  '''       namespaces, and attributes of the event in XML 1.0 valid
	  '''       syntax for START_ELEMENTs.
	  '''       The name is written by looking up the prefix for
	  '''       the namespace uri.  The writer can be configured to
	  '''       respect prefixes of QNames.  If the writer is respecting
	  '''       prefixes it must use the prefix set on the QName.  The
	  '''       default behavior is to lookup the value for the prefix
	  '''       on the EventWriter's internal namespace context.
	  '''       Each attribute (if any)
	  '''       is written using the behavior specified in the attribute
	  '''       section of this table.  Each namespace (if any) is written
	  '''       using the behavior specified in the namespace section of this
	  '''       table.
	  '''       </td>
	  '''     </tr>
	  '''     <tr>
	  '''       <td> END_ELEMENT  </td>
	  '''       <td> Qname name  </td>
	  '''       <td> None </td>
	  '''       <td> A well formed END_ELEMENT tag is written.
	  '''       The name is written by looking up the prefix for
	  '''       the namespace uri.  The writer can be configured to
	  '''       respect prefixes of QNames.  If the writer is respecting
	  '''       prefixes it must use the prefix set on the QName.  The
	  '''       default behavior is to lookup the value for the prefix
	  '''       on the EventWriter's internal namespace context.
	  '''       If the END_ELEMENT name does not match the START_ELEMENT
	  '''       name an XMLStreamException is thrown.
	  '''       </td>
	  '''     </tr>
	  '''     <tr>
	  '''       <td> ATTRIBUTE  </td>
	  '''       <td> QName name , String value </td>
	  '''       <td> QName type </td>
	  '''       <td> An attribute is written using the same algorithm
	  '''            to find the lexical form as used in START_ELEMENT.
	  '''            The default is to use double quotes to wrap attribute
	  '''            values and to escape any double quotes found in the
	  '''            value.  The type value is ignored.
	  '''       </td>
	  '''     </tr>
	  '''     <tr>
	  '''       <td> NAMESPACE  </td>
	  '''       <td> String prefix, String namespaceURI,
	  '''            boolean isDefaultNamespaceDeclaration
	  '''      </td>
	  '''       <td> None  </td>
	  '''       <td> A namespace declaration is written.  If the
	  '''            namespace is a default namespace declaration
	  '''            (isDefault is true) then xmlns="$namespaceURI"
	  '''            is written and the prefix is optional.  If
	  '''            isDefault is false, the prefix must be declared
	  '''            and the writer must prepend xmlns to the prefix
	  '''            and write out a standard prefix declaration.
	  '''      </td>
	  '''     </tr>
	  '''     <tr>
	  '''       <td> PROCESSING_INSTRUCTION  </td>
	  '''       <td>   None</td>
	  '''       <td>   String target, String data</td>
	  '''       <td>   The data does not need to be present and may be
	  '''              null.  Target is required and many not be null.
	  '''              The writer
	  '''              will write data section
	  '''              directly after the target,
	  '''              enclosed in appropriate XML 1.0 syntax
	  '''      </td>
	  '''     </tr>
	  '''     <tr>
	  '''       <td> COMMENT  </td>
	  '''       <td> None  </td>
	  '''       <td> String comment  </td>
	  '''       <td> If the comment is present (not null) it is written, otherwise an
	  '''            an empty comment is written
	  '''      </td>
	  '''     </tr>
	  '''     <tr>
	  '''       <td> START_DOCUMENT  </td>
	  '''       <td> None  </td>
	  '''       <td> String encoding , boolean standalone, String version  </td>
	  '''       <td> A START_DOCUMENT event is not required to be written to the
	  '''             stream.  If present the attributes are written inside
	  '''             the appropriate XML declaration syntax
	  '''      </td>
	  '''     </tr>
	  '''     <tr>
	  '''       <td> END_DOCUMENT  </td>
	  '''       <td> None </td>
	  '''       <td> None  </td>
	  '''       <td> Nothing is written to the output  </td>
	  '''     </tr>
	  '''     <tr>
	  '''       <td> DTD  </td>
	  '''       <td> String DocumentTypeDefinition  </td>
	  '''       <td> None  </td>
	  '''       <td> The DocumentTypeDefinition is written to the output  </td>
	  '''     </tr>
	  '''   </tbody>
	  ''' </table> </summary>
	  ''' <param name="event"> the event to be added </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub add(ByVal [event] As XMLEvent)

	  ''' <summary>
	  ''' Adds an entire stream to an output stream,
	  ''' calls next() on the inputStream argument until hasNext() returns false
	  ''' This should be treated as a convenience method that will
	  ''' perform the following loop over all the events in an
	  ''' event reader and call add on each event.
	  ''' </summary>
	  ''' <param name="reader"> the event stream to add to the output </param>
	  ''' <exception cref="XMLStreamException"> </exception>

	  Sub add(ByVal reader As XMLEventReader)

	  ''' <summary>
	  ''' Gets the prefix the uri is bound to </summary>
	  ''' <param name="uri"> the uri to look up </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Function getPrefix(ByVal uri As String) As String

	  ''' <summary>
	  ''' Sets the prefix the uri is bound to.  This prefix is bound
	  ''' in the scope of the current START_ELEMENT / END_ELEMENT pair.
	  ''' If this method is called before a START_ELEMENT has been written
	  ''' the prefix is bound in the root scope. </summary>
	  ''' <param name="prefix"> the prefix to bind to the uri </param>
	  ''' <param name="uri"> the uri to bind to the prefix </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub setPrefix(ByVal prefix As String, ByVal uri As String)

	  ''' <summary>
	  ''' Binds a URI to the default namespace
	  ''' This URI is bound
	  ''' in the scope of the current START_ELEMENT / END_ELEMENT pair.
	  ''' If this method is called before a START_ELEMENT has been written
	  ''' the uri is bound in the root scope. </summary>
	  ''' <param name="uri"> the uri to bind to the default namespace </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  WriteOnly Property defaultNamespace As String

	  ''' <summary>
	  ''' Sets the current namespace context for prefix and uri bindings.
	  ''' This context becomes the root namespace context for writing and
	  ''' will replace the current root namespace context.  Subsequent calls
	  ''' to setPrefix and setDefaultNamespace will bind namespaces using
	  ''' the context passed to the method as the root context for resolving
	  ''' namespaces. </summary>
	  ''' <param name="context"> the namespace context to use for this writer </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Property namespaceContext As javax.xml.namespace.NamespaceContext



	End Interface

End Namespace
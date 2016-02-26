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


	''' <summary>
	''' The XMLStreamWriter interface specifies how to write XML.  The XMLStreamWriter  does
	''' not perform well formedness checking on its input.  However
	''' the writeCharacters method is required to escape &amp; , &lt; and &gt;
	''' For attribute values the writeAttribute method will escape the
	''' above characters plus &quot; to ensure that all character content
	''' and attribute values are well formed.
	''' 
	''' Each NAMESPACE
	''' and ATTRIBUTE must be individually written.
	''' 
	''' <table border="1" cellpadding="2" cellspacing="0">
	'''     <thead>
	'''         <tr>
	'''             <th colspan="5">XML Namespaces, <code>javax.xml.stream.isRepairingNamespaces</code> and write method behaviour</th>
	'''         </tr>
	'''         <tr>
	'''             <th>Method</th> <!-- method -->
	'''             <th colspan="2"><code>isRepairingNamespaces</code> == true</th>
	'''             <th colspan="2"><code>isRepairingNamespaces</code> == false</th>
	'''         </tr>
	'''         <tr>
	'''             <th></th> <!-- method -->
	'''             <th>namespaceURI bound</th>
	'''             <th>namespaceURI unbound</th>
	'''             <th>namespaceURI bound</th>
	'''             <th>namespaceURI unbound</th>
	'''         </tr>
	'''     </thead>
	''' 
	'''     <tbody>
	'''         <tr>
	'''             <th><code>writeAttribute(namespaceURI, localName, value)</code></th>
	'''             <!-- isRepairingNamespaces == true -->
	'''             <td>
	'''                 <!-- namespaceURI bound -->
	'''                 prefix:localName="value"&nbsp;<sup>[1]</sup>
	'''             </td>
	'''             <td>
	'''                 <!-- namespaceURI unbound -->
	'''                 xmlns:{generated}="namespaceURI" {generated}:localName="value"
	'''             </td>
	'''             <!-- isRepairingNamespaces == false -->
	'''             <td>
	'''                 <!-- namespaceURI bound -->
	'''                 prefix:localName="value"&nbsp;<sup>[1]</sup>
	'''             </td>
	'''             <td>
	'''                 <!-- namespaceURI unbound -->
	'''                 <code>XMLStreamException</code>
	'''             </td>
	'''         </tr>
	''' 
	'''         <tr>
	'''             <th><code>writeAttribute(prefix, namespaceURI, localName, value)</code></th>
	'''             <!-- isRepairingNamespaces == true -->
	'''             <td>
	'''                 <!-- namespaceURI bound -->
	'''                 bound to same prefix:<br />
	'''                 prefix:localName="value"&nbsp;<sup>[1]</sup><br />
	'''                 <br />
	'''                 bound to different prefix:<br />
	'''                 xmlns:{generated}="namespaceURI" {generated}:localName="value"
	'''             </td>
	'''             <td>
	'''                 <!-- namespaceURI unbound -->
	'''                 xmlns:prefix="namespaceURI" prefix:localName="value"&nbsp;<sup>[3]</sup>
	'''             </td>
	'''             <!-- isRepairingNamespaces == false -->
	'''             <td>
	'''                 <!-- namespaceURI bound -->
	'''                 bound to same prefix:<br />
	'''                 prefix:localName="value"&nbsp;<sup>[1][2]</sup><br />
	'''                 <br />
	'''                 bound to different prefix:<br />
	'''                 <code>XMLStreamException</code><sup>[2]</sup>
	'''             </td>
	'''             <td>
	'''                 <!-- namespaceURI unbound -->
	'''                 xmlns:prefix="namespaceURI" prefix:localName="value"&nbsp;<sup>[2][5]</sup>
	'''             </td>
	'''         </tr>
	''' 
	'''         <tr>
	'''             <th><code>writeStartElement(namespaceURI, localName)</code><br />
	'''                 <br />
	'''                 <code>writeEmptyElement(namespaceURI, localName)</code></th>
	'''             <!-- isRepairingNamespaces == true -->
	'''             <td >
	'''                 <!-- namespaceURI bound -->
	'''                 &lt;prefix:localName&gt;&nbsp;<sup>[1]</sup>
	'''             </td>
	'''             <td>
	'''                 <!-- namespaceURI unbound -->
	'''                 &lt;{generated}:localName xmlns:{generated}="namespaceURI"&gt;
	'''             </td>
	'''             <!-- isRepairingNamespaces == false -->
	'''             <td>
	'''                 <!-- namespaceURI bound -->
	'''                 &lt;prefix:localName&gt;&nbsp;<sup>[1]</sup>
	'''             </td>
	'''             <td>
	'''                 <!-- namespaceURI unbound -->
	'''                 <code>XMLStreamException</code>
	'''             </td>
	'''         </tr>
	''' 
	'''         <tr>
	'''             <th><code>writeStartElement(prefix, localName, namespaceURI)</code><br />
	'''                 <br />
	'''                 <code>writeEmptyElement(prefix, localName, namespaceURI)</code></th>
	'''             <!-- isRepairingNamespaces == true -->
	'''             <td>
	'''                 <!-- namespaceURI bound -->
	'''                 bound to same prefix:<br />
	'''                 &lt;prefix:localName&gt;&nbsp;<sup>[1]</sup><br />
	'''                 <br />
	'''                 bound to different prefix:<br />
	'''                 &lt;{generated}:localName xmlns:{generated}="namespaceURI"&gt;
	'''             </td>
	'''             <td>
	'''                 <!-- namespaceURI unbound -->
	'''                 &lt;prefix:localName xmlns:prefix="namespaceURI"&gt;&nbsp;<sup>[4]</sup>
	'''             </td>
	'''             <!-- isRepairingNamespaces == false -->
	'''             <td>
	'''                 <!-- namespaceURI bound -->
	'''                 bound to same prefix:<br />
	'''                 &lt;prefix:localName&gt;&nbsp;<sup>[1]</sup><br />
	'''                 <br />
	'''                 bound to different prefix:<br />
	'''                 <code>XMLStreamException</code>
	'''             </td>
	'''             <td>
	'''                 <!-- namespaceURI unbound -->
	'''                 &lt;prefix:localName&gt;&nbsp;
	'''             </td>
	'''         </tr>
	'''     </tbody>
	'''     <tfoot>
	'''         <tr>
	'''             <td colspan="5">
	'''                 Notes:
	'''                 <ul>
	'''                     <li>[1] if namespaceURI == default Namespace URI, then no prefix is written</li>
	'''                     <li>[2] if prefix == "" || null && namespaceURI == "", then no prefix or Namespace declaration is generated or written</li>
	'''                     <li>[3] if prefix == "" || null, then a prefix is randomly generated</li>
	'''                     <li>[4] if prefix == "" || null, then it is treated as the default Namespace and no prefix is generated or written, an xmlns declaration is generated and written if the namespaceURI is unbound</li>
	'''                     <li>[5] if prefix == "" || null, then it is treated as an invalid attempt to define the default Namespace and an XMLStreamException is thrown</li>
	'''                 </ul>
	'''             </td>
	'''         </tr>
	'''     </tfoot>
	''' </table>
	''' 
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved. </summary>
	''' <seealso cref= XMLOutputFactory </seealso>
	''' <seealso cref= XMLStreamReader
	''' @since 1.6 </seealso>
	Public Interface XMLStreamWriter

	  ''' <summary>
	  ''' Writes a start tag to the output.  All writeStartElement methods
	  ''' open a new scope in the internal namespace context.  Writing the
	  ''' corresponding EndElement causes the scope to be closed. </summary>
	  ''' <param name="localName"> local name of the tag, may not be null </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub writeStartElement(ByVal localName As String)

	  ''' <summary>
	  ''' Writes a start tag to the output </summary>
	  ''' <param name="namespaceURI"> the namespaceURI of the prefix to use, may not be null </param>
	  ''' <param name="localName"> local name of the tag, may not be null </param>
	  ''' <exception cref="XMLStreamException"> if the namespace URI has not been bound to a prefix and
	  ''' javax.xml.stream.isRepairingNamespaces has not been set to true </exception>
	  Sub writeStartElement(ByVal namespaceURI As String, ByVal localName As String)

	  ''' <summary>
	  ''' Writes a start tag to the output </summary>
	  ''' <param name="localName"> local name of the tag, may not be null </param>
	  ''' <param name="prefix"> the prefix of the tag, may not be null </param>
	  ''' <param name="namespaceURI"> the uri to bind the prefix to, may not be null </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub writeStartElement(ByVal prefix As String, ByVal localName As String, ByVal namespaceURI As String)

	  ''' <summary>
	  ''' Writes an empty element tag to the output </summary>
	  ''' <param name="namespaceURI"> the uri to bind the tag to, may not be null </param>
	  ''' <param name="localName"> local name of the tag, may not be null </param>
	  ''' <exception cref="XMLStreamException"> if the namespace URI has not been bound to a prefix and
	  ''' javax.xml.stream.isRepairingNamespaces has not been set to true </exception>
	  Sub writeEmptyElement(ByVal namespaceURI As String, ByVal localName As String)

	  ''' <summary>
	  ''' Writes an empty element tag to the output </summary>
	  ''' <param name="prefix"> the prefix of the tag, may not be null </param>
	  ''' <param name="localName"> local name of the tag, may not be null </param>
	  ''' <param name="namespaceURI"> the uri to bind the tag to, may not be null </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub writeEmptyElement(ByVal prefix As String, ByVal localName As String, ByVal namespaceURI As String)

	  ''' <summary>
	  ''' Writes an empty element tag to the output </summary>
	  ''' <param name="localName"> local name of the tag, may not be null </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub writeEmptyElement(ByVal localName As String)

	  ''' <summary>
	  ''' Writes string data to the output without checking for well formedness.
	  ''' The data is opaque to the XMLStreamWriter, i.e. the characters are written
	  ''' blindly to the underlying output.  If the method cannot be supported
	  ''' in the currrent writing context the implementation may throw a
	  ''' UnsupportedOperationException.  For example note that any
	  ''' namespace declarations, end tags, etc. will be ignored and could
	  ''' interfere with proper maintanence of the writers internal state.
	  ''' </summary>
	  ''' <param name="data"> the data to write </param>
	  '  public void writeRaw(String data) throws XMLStreamException;

	  ''' <summary>
	  ''' Writes an end tag to the output relying on the internal
	  ''' state of the writer to determine the prefix and local name
	  ''' of the event. </summary>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub writeEndElement()

	  ''' <summary>
	  ''' Closes any start tags and writes corresponding end tags. </summary>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub writeEndDocument()

	  ''' <summary>
	  ''' Close this writer and free any resources associated with the
	  ''' writer.  This must not close the underlying output stream. </summary>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub close()

	  ''' <summary>
	  ''' Write any cached data to the underlying output mechanism. </summary>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub flush()

	  ''' <summary>
	  ''' Writes an attribute to the output stream without
	  ''' a prefix. </summary>
	  ''' <param name="localName"> the local name of the attribute </param>
	  ''' <param name="value"> the value of the attribute </param>
	  ''' <exception cref="IllegalStateException"> if the current state does not allow Attribute writing </exception>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub writeAttribute(ByVal localName As String, ByVal value As String)

	  ''' <summary>
	  ''' Writes an attribute to the output stream </summary>
	  ''' <param name="prefix"> the prefix for this attribute </param>
	  ''' <param name="namespaceURI"> the uri of the prefix for this attribute </param>
	  ''' <param name="localName"> the local name of the attribute </param>
	  ''' <param name="value"> the value of the attribute </param>
	  ''' <exception cref="IllegalStateException"> if the current state does not allow Attribute writing </exception>
	  ''' <exception cref="XMLStreamException"> if the namespace URI has not been bound to a prefix and
	  ''' javax.xml.stream.isRepairingNamespaces has not been set to true </exception>

	  Sub writeAttribute(ByVal prefix As String, ByVal namespaceURI As String, ByVal localName As String, ByVal value As String)

	  ''' <summary>
	  ''' Writes an attribute to the output stream </summary>
	  ''' <param name="namespaceURI"> the uri of the prefix for this attribute </param>
	  ''' <param name="localName"> the local name of the attribute </param>
	  ''' <param name="value"> the value of the attribute </param>
	  ''' <exception cref="IllegalStateException"> if the current state does not allow Attribute writing </exception>
	  ''' <exception cref="XMLStreamException"> if the namespace URI has not been bound to a prefix and
	  ''' javax.xml.stream.isRepairingNamespaces has not been set to true </exception>
	  Sub writeAttribute(ByVal namespaceURI As String, ByVal localName As String, ByVal value As String)

	  ''' <summary>
	  ''' Writes a namespace to the output stream
	  ''' If the prefix argument to this method is the empty string,
	  ''' "xmlns", or null this method will delegate to writeDefaultNamespace
	  ''' </summary>
	  ''' <param name="prefix"> the prefix to bind this namespace to </param>
	  ''' <param name="namespaceURI"> the uri to bind the prefix to </param>
	  ''' <exception cref="IllegalStateException"> if the current state does not allow Namespace writing </exception>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub writeNamespace(ByVal prefix As String, ByVal namespaceURI As String)

	  ''' <summary>
	  ''' Writes the default namespace to the stream </summary>
	  ''' <param name="namespaceURI"> the uri to bind the default namespace to </param>
	  ''' <exception cref="IllegalStateException"> if the current state does not allow Namespace writing </exception>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub writeDefaultNamespace(ByVal namespaceURI As String)

	  ''' <summary>
	  ''' Writes an xml comment with the data enclosed </summary>
	  ''' <param name="data"> the data contained in the comment, may be null </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub writeComment(ByVal data As String)

	  ''' <summary>
	  ''' Writes a processing instruction </summary>
	  ''' <param name="target"> the target of the processing instruction, may not be null </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub writeProcessingInstruction(ByVal target As String)

	  ''' <summary>
	  ''' Writes a processing instruction </summary>
	  ''' <param name="target"> the target of the processing instruction, may not be null </param>
	  ''' <param name="data"> the data contained in the processing instruction, may not be null </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub writeProcessingInstruction(ByVal target As String, ByVal data As String)

	  ''' <summary>
	  ''' Writes a CData section </summary>
	  ''' <param name="data"> the data contained in the CData Section, may not be null </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub writeCData(ByVal data As String)

	  ''' <summary>
	  ''' Write a DTD section.  This string represents the entire doctypedecl production
	  ''' from the XML 1.0 specification.
	  ''' </summary>
	  ''' <param name="dtd"> the DTD to be written </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub writeDTD(ByVal dtd As String)

	  ''' <summary>
	  ''' Writes an entity reference </summary>
	  ''' <param name="name"> the name of the entity </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub writeEntityRef(ByVal name As String)

	  ''' <summary>
	  ''' Write the XML Declaration. Defaults the XML version to 1.0, and the encoding to utf-8 </summary>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub writeStartDocument()

	  ''' <summary>
	  ''' Write the XML Declaration. Defaults the XML version to 1.0 </summary>
	  ''' <param name="version"> version of the xml document </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub writeStartDocument(ByVal version As String)

	  ''' <summary>
	  ''' Write the XML Declaration.  Note that the encoding parameter does
	  ''' not set the actual encoding of the underlying output.  That must
	  ''' be set when the instance of the XMLStreamWriter is created using the
	  ''' XMLOutputFactory </summary>
	  ''' <param name="encoding"> encoding of the xml declaration </param>
	  ''' <param name="version"> version of the xml document </param>
	  ''' <exception cref="XMLStreamException"> If given encoding does not match encoding
	  ''' of the underlying stream </exception>
	  Sub writeStartDocument(ByVal encoding As String, ByVal version As String)

	  ''' <summary>
	  ''' Write text to the output </summary>
	  ''' <param name="text"> the value to write </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub writeCharacters(ByVal text As String)

	  ''' <summary>
	  ''' Write text to the output </summary>
	  ''' <param name="text"> the value to write </param>
	  ''' <param name="start"> the starting position in the array </param>
	  ''' <param name="len"> the number of characters to write </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub writeCharacters(ByVal text As Char(), ByVal start As Integer, ByVal len As Integer)

	  ''' <summary>
	  ''' Gets the prefix the uri is bound to </summary>
	  ''' <returns> the prefix or null </returns>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Function getPrefix(ByVal uri As String) As String

	  ''' <summary>
	  ''' Sets the prefix the uri is bound to.  This prefix is bound
	  ''' in the scope of the current START_ELEMENT / END_ELEMENT pair.
	  ''' If this method is called before a START_ELEMENT has been written
	  ''' the prefix is bound in the root scope. </summary>
	  ''' <param name="prefix"> the prefix to bind to the uri, may not be null </param>
	  ''' <param name="uri"> the uri to bind to the prefix, may be null </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Sub setPrefix(ByVal prefix As String, ByVal uri As String)


	  ''' <summary>
	  ''' Binds a URI to the default namespace
	  ''' This URI is bound
	  ''' in the scope of the current START_ELEMENT / END_ELEMENT pair.
	  ''' If this method is called before a START_ELEMENT has been written
	  ''' the uri is bound in the root scope. </summary>
	  ''' <param name="uri"> the uri to bind to the default namespace, may be null </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  WriteOnly Property defaultNamespace As String

	  ''' <summary>
	  ''' Sets the current namespace context for prefix and uri bindings.
	  ''' This context becomes the root namespace context for writing and
	  ''' will replace the current root namespace context.  Subsequent calls
	  ''' to setPrefix and setDefaultNamespace will bind namespaces using
	  ''' the context passed to the method as the root context for resolving
	  ''' namespaces.  This method may only be called once at the start of
	  ''' the document.  It does not cause the namespaces to be declared.
	  ''' If a namespace URI to prefix mapping is found in the namespace
	  ''' context it is treated as declared and the prefix may be used
	  ''' by the StreamWriter. </summary>
	  ''' <param name="context"> the namespace context to use for this writer, may not be null </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Property namespaceContext As javax.xml.namespace.NamespaceContext


	  ''' <summary>
	  ''' Get the value of a feature/property from the underlying implementation </summary>
	  ''' <param name="name"> The name of the property, may not be null </param>
	  ''' <returns> The value of the property </returns>
	  ''' <exception cref="IllegalArgumentException"> if the property is not supported </exception>
	  ''' <exception cref="NullPointerException"> if the name is null </exception>
	  Function getProperty(ByVal name As String) As Object

	End Interface

End Namespace
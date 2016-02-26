Imports System

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.bind


	''' <summary>
	''' <p>
	''' The <tt>Marshaller</tt> class is responsible for governing the process
	''' of serializing Java content trees back into XML data.  It provides the basic
	''' marshalling methods:
	''' 
	''' <p>
	''' <i>Assume the following setup code for all following code fragments:</i>
	''' <blockquote>
	'''    <pre>
	'''       JAXBContext jc = JAXBContext.newInstance( "com.acme.foo" );
	'''       Unmarshaller u = jc.createUnmarshaller();
	'''       Object element = u.unmarshal( new File( "foo.xml" ) );
	'''       Marshaller m = jc.createMarshaller();
	'''    </pre>
	''' </blockquote>
	''' 
	''' <p>
	''' Marshalling to a File:
	''' <blockquote>
	'''    <pre>
	'''       OutputStream os = new FileOutputStream( "nosferatu.xml" );
	'''       m.marshal( element, os );
	'''    </pre>
	''' </blockquote>
	''' 
	''' <p>
	''' Marshalling to a SAX ContentHandler:
	''' <blockquote>
	'''    <pre>
	'''       // assume MyContentHandler instanceof ContentHandler
	'''       m.marshal( element, new MyContentHandler() );
	'''    </pre>
	''' </blockquote>
	''' 
	''' <p>
	''' Marshalling to a DOM Node:
	''' <blockquote>
	'''    <pre>
	'''       DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
	'''       dbf.setNamespaceAware(true);
	'''       DocumentBuilder db = dbf.newDocumentBuilder();
	'''       Document doc = db.newDocument();
	''' 
	'''       m.marshal( element, doc );
	'''    </pre>
	''' </blockquote>
	''' 
	''' <p>
	''' Marshalling to a java.io.OutputStream:
	''' <blockquote>
	'''    <pre>
	'''       m.marshal( element, System.out );
	'''    </pre>
	''' </blockquote>
	''' 
	''' <p>
	''' Marshalling to a java.io.Writer:
	''' <blockquote>
	'''    <pre>
	'''       m.marshal( element, new PrintWriter( System.out ) );
	'''    </pre>
	''' </blockquote>
	''' 
	''' <p>
	''' Marshalling to a javax.xml.transform.SAXResult:
	''' <blockquote>
	'''    <pre>
	'''       // assume MyContentHandler instanceof ContentHandler
	'''       SAXResult result = new SAXResult( new MyContentHandler() );
	''' 
	'''       m.marshal( element, result );
	'''    </pre>
	''' </blockquote>
	''' 
	''' <p>
	''' Marshalling to a javax.xml.transform.DOMResult:
	''' <blockquote>
	'''    <pre>
	'''       DOMResult result = new DOMResult();
	''' 
	'''       m.marshal( element, result );
	'''    </pre>
	''' </blockquote>
	''' 
	''' <p>
	''' Marshalling to a javax.xml.transform.StreamResult:
	''' <blockquote>
	'''    <pre>
	'''       StreamResult result = new StreamResult( System.out );
	''' 
	'''       m.marshal( element, result );
	'''    </pre>
	''' </blockquote>
	''' 
	''' <p>
	''' Marshalling to a javax.xml.stream.XMLStreamWriter:
	''' <blockquote>
	'''    <pre>
	'''       XMLStreamWriter xmlStreamWriter =
	'''           XMLOutputFactory.newInstance().createXMLStreamWriter( ... );
	''' 
	'''       m.marshal( element, xmlStreamWriter );
	'''    </pre>
	''' </blockquote>
	''' 
	''' <p>
	''' Marshalling to a javax.xml.stream.XMLEventWriter:
	''' <blockquote>
	'''    <pre>
	'''       XMLEventWriter xmlEventWriter =
	'''           XMLOutputFactory.newInstance().createXMLEventWriter( ... );
	''' 
	'''       m.marshal( element, xmlEventWriter );
	'''    </pre>
	''' </blockquote>
	''' 
	''' <p>
	''' <a name="elementMarshalling"></a>
	''' <b>Marshalling content tree rooted by a JAXB element</b><br>
	''' <blockquote>
	''' The first parameter of the overloaded
	''' <tt>Marshaller.marshal(java.lang.Object, ...)</tt> methods must be a
	''' JAXB element as computed by
	''' <seealso cref="JAXBIntrospector#isElement(java.lang.Object)"/>;
	''' otherwise, a <tt>Marshaller.marshal</tt> method must throw a
	''' <seealso cref="MarshalException"/>. There exist two mechanisms
	''' to enable marshalling an instance that is not a JAXB element.
	''' One method is to wrap the instance as a value of a <seealso cref="JAXBElement"/>,
	''' and pass the wrapper element as the first parameter to
	''' a <tt>Marshaller.marshal</tt> method. For java to schema binding, it
	''' is also possible to simply annotate the instance's class with
	''' &#64;<seealso cref="XmlRootElement"/>.
	''' </blockquote>
	''' 
	''' <p>
	''' <b>Encoding</b><br>
	''' <blockquote>
	''' By default, the Marshaller will use UTF-8 encoding when generating XML data
	''' to a <tt>java.io.OutputStream</tt>, or a <tt>java.io.Writer</tt>.  Use the
	''' <seealso cref="#setProperty(String,Object) setProperty"/> API to change the output
	''' encoding used during these marshal operations.  Client applications are
	''' expected to supply a valid character encoding name as defined in the
	''' <a href="http://www.w3.org/TR/2000/REC-xml-20001006#charencoding">W3C XML 1.0
	''' Recommendation</a> and supported by your Java Platform</a>.
	''' </blockquote>
	''' 
	''' <p>
	''' <b>Validation and Well-Formedness</b><br>
	''' <blockquote>
	''' <p>
	''' Client applications are not required to validate the Java content tree prior
	''' to calling any of the marshal API's.  Furthermore, there is no requirement
	''' that the Java content tree be valid with respect to its original schema in
	''' order to marshal it back into XML data.  Different JAXB Providers will
	''' support marshalling invalid Java content trees at varying levels, however
	''' all JAXB Providers must be able to marshal a valid content tree back to
	''' XML data.  A JAXB Provider must throw a <tt>MarshalException</tt> when it
	''' is unable to complete the marshal operation due to invalid content.  Some
	''' JAXB Providers will fully allow marshalling invalid content, others will fail
	''' on the first validation error.
	''' <p>
	''' Even when schema validation is not explictly enabled for the marshal operation,
	''' it is possible that certain types of validation events will be detected
	''' during the operation.  Validation events will be reported to the registered
	''' event handler.  If the client application has not registered an event handler
	''' prior to invoking one of the marshal API's, then events will be delivered to
	''' a default event handler which will terminate the marshal operation after
	''' encountering the first error or fatal error. Note that for JAXB 2.0 and
	''' later versions, <seealso cref="javax.xml.bind.helpers.DefaultValidationEventHandler"/> is
	''' no longer used.
	''' 
	''' </blockquote>
	''' 
	''' <p>
	''' <a name="supportedProps"></a>
	''' <b>Supported Properties</b><br>
	''' <blockquote>
	''' <p>
	''' All JAXB Providers are required to support the following set of properties.
	''' Some providers may support additional properties.
	''' <dl>
	'''   <dt><tt>jaxb.encoding</tt> - value must be a java.lang.String</dt>
	'''   <dd>The output encoding to use when marshalling the XML data.  The
	'''               Marshaller will use "UTF-8" by default if this property is not
	'''       specified.</dd>
	'''   <dt><tt>jaxb.formatted.output</tt> - value must be a java.lang.Boolean</dt>
	'''   <dd>This property controls whether or not the Marshaller will format
	'''       the resulting XML data with line breaks and indentation.  A
	'''       true value for this property indicates human readable indented
	'''       xml data, while a false value indicates unformatted xml data.
	'''       The Marshaller will default to false (unformatted) if this
	'''       property is not specified.</dd>
	'''   <dt><tt>jaxb.schemaLocation</tt> - value must be a java.lang.String</dt>
	'''   <dd>This property allows the client application to specify an
	'''       xsi:schemaLocation attribute in the generated XML data.  The format of
	'''       the schemaLocation attribute value is discussed in an easy to
	'''       understand, non-normative form in
	'''       <a href="http://www.w3.org/TR/xmlschema-0/#schemaLocation">Section 5.6
	'''       of the W3C XML Schema Part 0: Primer</a> and specified in
	'''       <a href="http://www.w3.org/TR/xmlschema-1/#Instance_Document_Constructions">
	'''       Section 2.6 of the W3C XML Schema Part 1: Structures</a>.</dd>
	'''   <dt><tt>jaxb.noNamespaceSchemaLocation</tt> - value must be a java.lang.String</dt>
	'''   <dd>This property allows the client application to specify an
	'''       xsi:noNamespaceSchemaLocation attribute in the generated XML
	'''       data.  The format of the schemaLocation attribute value is discussed in
	'''       an easy to understand, non-normative form in
	'''       <a href="http://www.w3.org/TR/xmlschema-0/#schemaLocation">Section 5.6
	'''       of the W3C XML Schema Part 0: Primer</a> and specified in
	'''       <a href="http://www.w3.org/TR/xmlschema-1/#Instance_Document_Constructions">
	'''       Section 2.6 of the W3C XML Schema Part 1: Structures</a>.</dd>
	'''   <dt><tt>jaxb.fragment</tt> - value must be a java.lang.Boolean</dt>
	'''   <dd>This property determines whether or not document level events will be
	'''       generated by the Marshaller.  If the property is not specified, the
	'''       default is <tt>false</tt>. This property has different implications depending
	'''       on which marshal api you are using - when this property is set to true:<br>
	'''       <ul>
	'''         <li><seealso cref="#marshal(Object,org.xml.sax.ContentHandler) marshal(Object,ContentHandler)"/> - the Marshaller won't
	'''             invoke <seealso cref="org.xml.sax.ContentHandler#startDocument()"/> and
	'''             <seealso cref="org.xml.sax.ContentHandler#endDocument()"/>.</li>
	'''         <li><seealso cref="#marshal(Object,org.w3c.dom.Node) marshal(Object,Node)"/> - the property has no effect on this
	'''             API.</li>
	'''         <li><seealso cref="#marshal(Object,java.io.OutputStream) marshal(Object,OutputStream)"/> - the Marshaller won't
	'''             generate an xml declaration.</li>
	'''         <li><seealso cref="#marshal(Object,java.io.Writer) marshal(Object,Writer)"/> - the Marshaller won't
	'''             generate an xml declaration.</li>
	'''         <li><seealso cref="#marshal(Object,javax.xml.transform.Result) marshal(Object,Result)"/> - depends on the kind of
	'''             Result object, see semantics for Node, ContentHandler, and Stream APIs</li>
	'''         <li><seealso cref="#marshal(Object,javax.xml.stream.XMLEventWriter) marshal(Object,XMLEventWriter)"/> - the
	'''             Marshaller will not generate <seealso cref="javax.xml.stream.events.XMLEvent#START_DOCUMENT"/> and
	'''             <seealso cref="javax.xml.stream.events.XMLEvent#END_DOCUMENT"/> events.</li>
	'''         <li><seealso cref="#marshal(Object,javax.xml.stream.XMLStreamWriter) marshal(Object,XMLStreamWriter)"/> - the
	'''             Marshaller will not generate <seealso cref="javax.xml.stream.events.XMLEvent#START_DOCUMENT"/> and
	'''             <seealso cref="javax.xml.stream.events.XMLEvent#END_DOCUMENT"/> events.</li>
	'''       </ul>
	'''   </dd>
	''' </dl>
	''' </blockquote>
	''' 
	''' <p>
	''' <a name="marshalEventCallback"></a>
	''' <b>Marshal Event Callbacks</b><br>
	''' <blockquote>
	''' "The {@link Marshaller} provides two styles of callback mechanisms
	''' that allow application specific processing during key points in the
	''' unmarshalling process.  In 'class defined' event callbacks, application
	''' specific code placed in JAXB mapped classes is triggered during
	''' marshalling.  'External listeners' allow for centralized processing
	''' of marshal events in one callback method rather than by type event callbacks.
	''' 
	''' <p>
	''' Class defined event callback methods allow any JAXB mapped class to specify
	''' its own specific callback methods by defining methods with the following method signatures:
	''' <blockquote>
	''' <pre>
	'''   // Invoked by Marshaller after it has created an instance of this object.
	'''   boolean beforeMarshal(Marshaller);
	''' 
	'''   // Invoked by Marshaller after it has marshalled all properties of this object.
	'''   void afterMarshal(Marshaller);
	''' </pre>
	''' </blockquote>
	''' The class defined event callback methods should be used when the callback method requires
	''' access to non-public methods and/or fields of the class.
	''' <p>
	''' The external listener callback mechanism enables the registration of a <seealso cref="Listener"/>
	''' instance with a <seealso cref="Marshaller#setListener(Listener)"/>. The external listener receives all callback events,
	''' allowing for more centralized processing than per class defined callback methods.
	''' <p>
	''' The 'class defined' and external listener event callback methods are independent of each other,
	''' both can be called for one event. The invocation ordering when both listener callback methods exist is
	''' defined in <seealso cref="Listener#beforeMarshal(Object)"/> and <seealso cref="Listener#afterMarshal(Object)"/>.
	''' <p>
	''' An event callback method throwing an exception terminates the current marshal process.
	''' </blockquote>
	''' 
	''' @author <ul><li>Kohsuke Kawaguchi, Sun Microsystems, Inc.</li><li>Ryan Shoemaker, Sun Microsystems, Inc.</li><li>Joe Fialli, Sun Microsystems, Inc.</li></ul> </summary>
	''' <seealso cref= JAXBContext </seealso>
	''' <seealso cref= Validator </seealso>
	''' <seealso cref= Unmarshaller
	''' @since JAXB1.0 </seealso>
	Public Interface Marshaller

		''' <summary>
		''' The name of the property used to specify the output encoding in
		''' the marshalled XML data.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String JAXB_ENCODING = "jaxb.encoding";

		''' <summary>
		''' The name of the property used to specify whether or not the marshalled
		''' XML data is formatted with linefeeds and indentation.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String JAXB_FORMATTED_OUTPUT = "jaxb.formatted.output";

		''' <summary>
		''' The name of the property used to specify the xsi:schemaLocation
		''' attribute value to place in the marshalled XML output.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String JAXB_SCHEMA_LOCATION = "jaxb.schemaLocation";

		''' <summary>
		''' The name of the property used to specify the
		''' xsi:noNamespaceSchemaLocation attribute value to place in the marshalled
		''' XML output.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String JAXB_NO_NAMESPACE_SCHEMA_LOCATION = "jaxb.noNamespaceSchemaLocation";

		''' <summary>
		''' The name of the property used to specify whether or not the marshaller
		''' will generate document level events (ie calling startDocument or endDocument).
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String JAXB_FRAGMENT = "jaxb.fragment";

		''' <summary>
		''' Marshal the content tree rooted at <tt>jaxbElement</tt> into the specified
		''' <tt>javax.xml.transform.Result</tt>.
		''' 
		''' <p>
		''' All JAXB Providers must at least support
		''' <seealso cref="javax.xml.transform.dom.DOMResult"/>,
		''' <seealso cref="javax.xml.transform.sax.SAXResult"/>, and
		''' <seealso cref="javax.xml.transform.stream.StreamResult"/>. It can
		''' support other derived classes of <tt>Result</tt> as well.
		''' </summary>
		''' <param name="jaxbElement">
		'''      The root of content tree to be marshalled. </param>
		''' <param name="result">
		'''      XML will be sent to this Result
		''' </param>
		''' <exception cref="JAXBException">
		'''      If any unexpected problem occurs during the marshalling. </exception>
		''' <exception cref="MarshalException">
		'''      If the <seealso cref="ValidationEventHandler ValidationEventHandler"/>
		'''      returns false from its <tt>handleEvent</tt> method or the
		'''      <tt>Marshaller</tt> is unable to marshal <tt>obj</tt> (or any
		'''      object reachable from <tt>obj</tt>).  See <a href="#elementMarshalling">
		'''      Marshalling a JAXB element</a>. </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If any of the method parameters are null </exception>
		Sub marshal(ByVal jaxbElement As Object, ByVal result As javax.xml.transform.Result)

		''' <summary>
		''' Marshal the content tree rooted at <tt>jaxbElement</tt> into an output stream.
		''' </summary>
		''' <param name="jaxbElement">
		'''      The root of content tree to be marshalled. </param>
		''' <param name="os">
		'''      XML will be added to this stream.
		''' </param>
		''' <exception cref="JAXBException">
		'''      If any unexpected problem occurs during the marshalling. </exception>
		''' <exception cref="MarshalException">
		'''      If the <seealso cref="ValidationEventHandler ValidationEventHandler"/>
		'''      returns false from its <tt>handleEvent</tt> method or the
		'''      <tt>Marshaller</tt> is unable to marshal <tt>obj</tt> (or any
		'''      object reachable from <tt>obj</tt>).  See <a href="#elementMarshalling">
		'''      Marshalling a JAXB element</a>. </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If any of the method parameters are null </exception>
		Sub marshal(ByVal jaxbElement As Object, ByVal os As java.io.OutputStream)

		''' <summary>
		''' Marshal the content tree rooted at <tt>jaxbElement</tt> into a file.
		''' </summary>
		''' <param name="jaxbElement">
		'''      The root of content tree to be marshalled. </param>
		''' <param name="output">
		'''      File to be written. If this file already exists, it will be overwritten.
		''' </param>
		''' <exception cref="JAXBException">
		'''      If any unexpected problem occurs during the marshalling. </exception>
		''' <exception cref="MarshalException">
		'''      If the <seealso cref="ValidationEventHandler ValidationEventHandler"/>
		'''      returns false from its <tt>handleEvent</tt> method or the
		'''      <tt>Marshaller</tt> is unable to marshal <tt>obj</tt> (or any
		'''      object reachable from <tt>obj</tt>).  See <a href="#elementMarshalling">
		'''      Marshalling a JAXB element</a>. </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If any of the method parameters are null
		''' @since JAXB2.1 </exception>
		Sub marshal(ByVal jaxbElement As Object, ByVal output As java.io.File)

		''' <summary>
		''' Marshal the content tree rooted at <tt>jaxbElement</tt> into a Writer.
		''' </summary>
		''' <param name="jaxbElement">
		'''      The root of content tree to be marshalled. </param>
		''' <param name="writer">
		'''      XML will be sent to this writer.
		''' </param>
		''' <exception cref="JAXBException">
		'''      If any unexpected problem occurs during the marshalling. </exception>
		''' <exception cref="MarshalException">
		'''      If the <seealso cref="ValidationEventHandler ValidationEventHandler"/>
		'''      returns false from its <tt>handleEvent</tt> method or the
		'''      <tt>Marshaller</tt> is unable to marshal <tt>obj</tt> (or any
		'''      object reachable from <tt>obj</tt>).  See <a href="#elementMarshalling">
		'''      Marshalling a JAXB element</a>. </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If any of the method parameters are null </exception>
		Sub marshal(ByVal jaxbElement As Object, ByVal writer As java.io.Writer)

		''' <summary>
		''' Marshal the content tree rooted at <tt>jaxbElement</tt> into SAX2 events.
		''' </summary>
		''' <param name="jaxbElement">
		'''      The root of content tree to be marshalled. </param>
		''' <param name="handler">
		'''      XML will be sent to this handler as SAX2 events.
		''' </param>
		''' <exception cref="JAXBException">
		'''      If any unexpected problem occurs during the marshalling. </exception>
		''' <exception cref="MarshalException">
		'''      If the <seealso cref="ValidationEventHandler ValidationEventHandler"/>
		'''      returns false from its <tt>handleEvent</tt> method or the
		'''      <tt>Marshaller</tt> is unable to marshal <tt>obj</tt> (or any
		'''      object reachable from <tt>obj</tt>).  See <a href="#elementMarshalling">
		'''      Marshalling a JAXB element</a>. </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If any of the method parameters are null </exception>
		Sub marshal(ByVal jaxbElement As Object, ByVal handler As org.xml.sax.ContentHandler)

		''' <summary>
		''' Marshal the content tree rooted at <tt>jaxbElement</tt> into a DOM tree.
		''' </summary>
		''' <param name="jaxbElement">
		'''      The content tree to be marshalled. </param>
		''' <param name="node">
		'''      DOM nodes will be added as children of this node.
		'''      This parameter must be a Node that accepts children
		'''      (<seealso cref="org.w3c.dom.Document"/>,
		'''      <seealso cref=" org.w3c.dom.DocumentFragment"/>, or
		'''      <seealso cref=" org.w3c.dom.Element"/>)
		''' </param>
		''' <exception cref="JAXBException">
		'''      If any unexpected problem occurs during the marshalling. </exception>
		''' <exception cref="MarshalException">
		'''      If the <seealso cref="ValidationEventHandler ValidationEventHandler"/>
		'''      returns false from its <tt>handleEvent</tt> method or the
		'''      <tt>Marshaller</tt> is unable to marshal <tt>jaxbElement</tt> (or any
		'''      object reachable from <tt>jaxbElement</tt>).  See <a href="#elementMarshalling">
		'''      Marshalling a JAXB element</a>. </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If any of the method parameters are null </exception>
		Sub marshal(ByVal jaxbElement As Object, ByVal node As org.w3c.dom.Node)

		''' <summary>
		''' Marshal the content tree rooted at <tt>jaxbElement</tt> into a
		''' <seealso cref="javax.xml.stream.XMLStreamWriter"/>.
		''' </summary>
		''' <param name="jaxbElement">
		'''      The content tree to be marshalled. </param>
		''' <param name="writer">
		'''      XML will be sent to this writer.
		''' </param>
		''' <exception cref="JAXBException">
		'''      If any unexpected problem occurs during the marshalling. </exception>
		''' <exception cref="MarshalException">
		'''      If the <seealso cref="ValidationEventHandler ValidationEventHandler"/>
		'''      returns false from its <tt>handleEvent</tt> method or the
		'''      <tt>Marshaller</tt> is unable to marshal <tt>obj</tt> (or any
		'''      object reachable from <tt>obj</tt>).  See <a href="#elementMarshalling">
		'''      Marshalling a JAXB element</a>. </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If any of the method parameters are null
		''' @since JAXB 2.0 </exception>
		Sub marshal(ByVal jaxbElement As Object, ByVal writer As javax.xml.stream.XMLStreamWriter)

		''' <summary>
		''' Marshal the content tree rooted at <tt>jaxbElement</tt> into a
		''' <seealso cref="javax.xml.stream.XMLEventWriter"/>.
		''' </summary>
		''' <param name="jaxbElement">
		'''      The content tree rooted at jaxbElement to be marshalled. </param>
		''' <param name="writer">
		'''      XML will be sent to this writer.
		''' </param>
		''' <exception cref="JAXBException">
		'''      If any unexpected problem occurs during the marshalling. </exception>
		''' <exception cref="MarshalException">
		'''      If the <seealso cref="ValidationEventHandler ValidationEventHandler"/>
		'''      returns false from its <tt>handleEvent</tt> method or the
		'''      <tt>Marshaller</tt> is unable to marshal <tt>obj</tt> (or any
		'''      object reachable from <tt>obj</tt>).  See <a href="#elementMarshalling">
		'''      Marshalling a JAXB element</a>. </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If any of the method parameters are null
		''' @since JAXB 2.0 </exception>
		Sub marshal(ByVal jaxbElement As Object, ByVal writer As javax.xml.stream.XMLEventWriter)

		''' <summary>
		''' Get a DOM tree view of the content tree(Optional).
		''' 
		''' If the returned DOM tree is updated, these changes are also
		''' visible in the content tree.
		''' Use <seealso cref="#marshal(Object, org.w3c.dom.Node)"/> to force
		''' a deep copy of the content tree to a DOM representation.
		''' </summary>
		''' <param name="contentTree"> - JAXB Java representation of XML content
		''' </param>
		''' <returns> the DOM tree view of the contentTree
		''' </returns>
		''' <exception cref="UnsupportedOperationException">
		'''      If the JAXB provider implementation does not support a
		'''      DOM view of the content tree
		''' </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If any of the method parameters are null
		''' </exception>
		''' <exception cref="JAXBException">
		'''      If any unexpected problem occurs
		'''  </exception>
		Function getNode(ByVal contentTree As Object) As org.w3c.dom.Node

		''' <summary>
		''' Set the particular property in the underlying implementation of
		''' <tt>Marshaller</tt>.  This method can only be used to set one of
		''' the standard JAXB defined properties above or a provider specific
		''' property.  Attempting to set an undefined property will result in
		''' a PropertyException being thrown.  See <a href="#supportedProps">
		''' Supported Properties</a>.
		''' </summary>
		''' <param name="name"> the name of the property to be set. This value can either
		'''              be specified using one of the constant fields or a user
		'''              supplied string. </param>
		''' <param name="value"> the value of the property to be set
		''' </param>
		''' <exception cref="PropertyException"> when there is an error processing the given
		'''                            property or value </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If the name parameter is null </exception>
		Sub setProperty(ByVal name As String, ByVal value As Object)

		''' <summary>
		''' Get the particular property in the underlying implementation of
		''' <tt>Marshaller</tt>.  This method can only be used to get one of
		''' the standard JAXB defined properties above or a provider specific
		''' property.  Attempting to get an undefined property will result in
		''' a PropertyException being thrown.  See <a href="#supportedProps">
		''' Supported Properties</a>.
		''' </summary>
		''' <param name="name"> the name of the property to retrieve </param>
		''' <returns> the value of the requested property
		''' </returns>
		''' <exception cref="PropertyException">
		'''      when there is an error retrieving the given property or value
		'''      property name </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If the name parameter is null </exception>
		Function getProperty(ByVal name As String) As Object

		''' <summary>
		''' Allow an application to register a validation event handler.
		''' <p>
		''' The validation event handler will be called by the JAXB Provider if any
		''' validation errors are encountered during calls to any of the marshal
		''' API's.  If the client application does not register a validation event
		''' handler before invoking one of the marshal methods, then validation
		''' events will be handled by the default event handler which will terminate
		''' the marshal operation after the first error or fatal error is encountered.
		''' <p>
		''' Calling this method with a null parameter will cause the Marshaller
		''' to revert back to the default default event handler.
		''' </summary>
		''' <param name="handler"> the validation event handler </param>
		''' <exception cref="JAXBException"> if an error was encountered while setting the
		'''         event handler </exception>
		Property eventHandler As ValidationEventHandler




		''' <summary>
		''' Associates a configured instance of <seealso cref="XmlAdapter"/> with this marshaller.
		''' 
		''' <p>
		''' This is a convenience method that invokes <code>setAdapter(adapter.getClass(),adapter);</code>.
		''' </summary>
		''' <seealso cref= #setAdapter(Class,XmlAdapter) </seealso>
		''' <exception cref="IllegalArgumentException">
		'''      if the adapter parameter is null. </exception>
		''' <exception cref="UnsupportedOperationException">
		'''      if invoked agains a JAXB 1.0 implementation.
		''' @since JAXB 2.0 </exception>
		WriteOnly Property adapter As javax.xml.bind.annotation.adapters.XmlAdapter

		''' <summary>
		''' Associates a configured instance of <seealso cref="XmlAdapter"/> with this marshaller.
		''' 
		''' <p>
		''' Every marshaller internally maintains a
		''' <seealso cref="java.util.Map"/>&lt;<seealso cref="Class"/>,<seealso cref="XmlAdapter"/>>,
		''' which it uses for marshalling classes whose fields/methods are annotated
		''' with <seealso cref="javax.xml.bind.annotation.adapters.XmlJavaTypeAdapter"/>.
		''' 
		''' <p>
		''' This method allows applications to use a configured instance of <seealso cref="XmlAdapter"/>.
		''' When an instance of an adapter is not given, a marshaller will create
		''' one by invoking its default constructor.
		''' </summary>
		''' <param name="type">
		'''      The type of the adapter. The specified instance will be used when
		'''      <seealso cref="javax.xml.bind.annotation.adapters.XmlJavaTypeAdapter#value()"/>
		'''      refers to this type. </param>
		''' <param name="adapter">
		'''      The instance of the adapter to be used. If null, it will un-register
		'''      the current adapter set for this type. </param>
		''' <exception cref="IllegalArgumentException">
		'''      if the type parameter is null. </exception>
		''' <exception cref="UnsupportedOperationException">
		'''      if invoked agains a JAXB 1.0 implementation.
		''' @since JAXB 2.0 </exception>
		 Sub setAdapter(Of A As javax.xml.bind.annotation.adapters.XmlAdapter)(ByVal type As Type, ByVal adapter As A)

		''' <summary>
		''' Gets the adapter associated with the specified type.
		''' 
		''' This is the reverse operation of the <seealso cref="#setAdapter"/> method.
		''' </summary>
		''' <exception cref="IllegalArgumentException">
		'''      if the type parameter is null. </exception>
		''' <exception cref="UnsupportedOperationException">
		'''      if invoked agains a JAXB 1.0 implementation.
		''' @since JAXB 2.0 </exception>
		 Function getAdapter(Of A As javax.xml.bind.annotation.adapters.XmlAdapter)(ByVal type As Type) As A


		''' <summary>
		''' <p>Associate a context that enables binary data within an XML document
		''' to be transmitted as XML-binary optimized attachment.
		''' The attachment is referenced from the XML document content model
		''' by content-id URIs(cid) references stored within the xml document.
		''' </summary>
		''' <exception cref="IllegalStateException"> if attempt to concurrently call this
		'''                               method during a marshal operation. </exception>
		Property attachmentMarshaller As javax.xml.bind.attachment.AttachmentMarshaller


		''' <summary>
		''' Specify the JAXP 1.3 <seealso cref="javax.xml.validation.Schema Schema"/>
		''' object that should be used to validate subsequent marshal operations
		''' against.  Passing null into this method will disable validation.
		''' 
		''' <p>
		''' This method allows the caller to validate the marshalled XML as it's marshalled.
		''' 
		''' <p>
		''' Initially this property is set to <tt>null</tt>.
		''' </summary>
		''' <param name="schema"> Schema object to validate marshal operations against or null to disable validation </param>
		''' <exception cref="UnsupportedOperationException"> could be thrown if this method is
		'''         invoked on an Marshaller created from a JAXBContext referencing
		'''         JAXB 1.0 mapped classes
		''' @since JAXB2.0 </exception>
		Property schema As javax.xml.validation.Schema


		''' <summary>
		''' <p/>
		''' Register an instance of an implementation of this class with a <seealso cref="Marshaller"/> to externally listen
		''' for marshal events.
		''' <p/>
		''' <p/>
		''' This class enables pre and post processing of each marshalled object.
		''' The event callbacks are called when marshalling from an instance that maps to an xml element or
		''' complex type definition. The event callbacks are not called when marshalling from an instance of a
		''' Java datatype that represents a simple type definition.
		''' <p/>
		''' <p/>
		''' External listener is one of two different mechanisms for defining marshal event callbacks.
		''' See <a href="Marshaller.html#marshalEventCallback">Marshal Event Callbacks</a> for an overview.
		''' </summary>
		''' <seealso cref= Marshaller#setListener(Listener) </seealso>
		''' <seealso cref= Marshaller#getListener()
		''' @since JAXB2.0 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public static abstract class Listener
	'	{
	'		''' <summary>
	'		''' <p/>
	'		''' Callback method invoked before marshalling from <tt>source</tt> to XML.
	'		''' <p/>
	'		''' <p/>
	'		''' This method is invoked just before marshalling process starts to marshal <tt>source</tt>.
	'		''' Note that if the class of <tt>source</tt> defines its own <tt>beforeMarshal</tt> method,
	'		''' the class specific callback method is invoked just before this method is invoked.
	'		''' </summary>
	'		''' <param name="source"> instance of JAXB mapped class prior to marshalling from it. </param>
	'		public void beforeMarshal(Object source)
	'		{
	'		}
	'
	'		''' <summary>
	'		''' <p/>
	'		''' Callback method invoked after marshalling <tt>source</tt> to XML.
	'		''' <p/>
	'		''' <p/>
	'		''' This method is invoked after <tt>source</tt> and all its descendants have been marshalled.
	'		''' Note that if the class of <tt>source</tt> defines its own <tt>afterMarshal</tt> method,
	'		''' the class specific callback method is invoked just before this method is invoked.
	'		''' </summary>
	'		''' <param name="source"> instance of JAXB mapped class after marshalling it. </param>
	'		public void afterMarshal(Object source)
	'		{
	'		}
	'	}

		''' <summary>
		''' <p>
		''' Register marshal event callback <seealso cref="Listener"/> with this <seealso cref="Marshaller"/>.
		''' 
		''' <p>
		''' There is only one Listener per Marshaller. Setting a Listener replaces the previous set Listener.
		''' One can unregister current Listener by setting listener to <tt>null</tt>.
		''' </summary>
		''' <param name="listener"> an instance of a class that implements <seealso cref="Listener"/>
		''' @since JAXB2.0 </param>
		Property listener As Listener

	End Interface

End Namespace
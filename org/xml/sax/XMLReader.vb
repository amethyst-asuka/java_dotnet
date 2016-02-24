'
' * Copyright (c) 2000, 2005, Oracle and/or its affiliates. All rights reserved.
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

' XMLReader.java - read an XML document.
' http://www.saxproject.org
' Written by David Megginson
' NO WARRANTY!  This class is in the Public Domain.
' $Id: XMLReader.java,v 1.3 2004/11/03 22:55:32 jsuttor Exp $

Namespace org.xml.sax



	''' <summary>
	''' Interface for reading an XML document using callbacks.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' See <a href='http://www.saxproject.org'>http://www.saxproject.org</a>
	''' for further information.
	''' </blockquote>
	''' 
	''' <p><strong>Note:</strong> despite its name, this interface does
	''' <em>not</em> extend the standard Java <seealso cref="java.io.Reader Reader"/>
	''' interface, because reading XML is a fundamentally different activity
	''' than reading character data.</p>
	''' 
	''' <p>XMLReader is the interface that an XML parser's SAX2 driver must
	''' implement.  This interface allows an application to set and
	''' query features and properties in the parser, to register
	''' event handlers for document processing, and to initiate
	''' a document parse.</p>
	''' 
	''' <p>All SAX interfaces are assumed to be synchronous: the
	''' <seealso cref="#parse parse"/> methods must not return until parsing
	''' is complete, and readers must wait for an event-handler callback
	''' to return before reporting the next event.</p>
	''' 
	''' <p>This interface replaces the (now deprecated) SAX 1.0 {@link
	''' org.xml.sax.Parser Parser} interface.  The XMLReader interface
	''' contains two important enhancements over the old Parser
	''' interface (as well as some minor ones):</p>
	''' 
	''' <ol>
	''' <li>it adds a standard way to query and set features and
	'''  properties; and</li>
	''' <li>it adds Namespace support, which is required for many
	'''  higher-level XML standards.</li>
	''' </ol>
	''' 
	''' <p>There are adapters available to convert a SAX1 Parser to
	''' a SAX2 XMLReader and vice-versa.</p>
	''' 
	''' @since SAX 2.0
	''' @author David Megginson </summary>
	''' <seealso cref= org.xml.sax.XMLFilter </seealso>
	''' <seealso cref= org.xml.sax.helpers.ParserAdapter </seealso>
	''' <seealso cref= org.xml.sax.helpers.XMLReaderAdapter </seealso>
	Public Interface XMLReader


		'//////////////////////////////////////////////////////////////////
		' Configuration.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Look up the value of a feature flag.
		''' 
		''' <p>The feature name is any fully-qualified URI.  It is
		''' possible for an XMLReader to recognize a feature name but
		''' temporarily be unable to return its value.
		''' Some feature values may be available only in specific
		''' contexts, such as before, during, or after a parse.
		''' Also, some feature values may not be programmatically accessible.
		''' (In the case of an adapter for SAX1 <seealso cref="Parser"/>, there is no
		''' implementation-independent way to expose whether the underlying
		''' parser is performing validation, expanding external entities,
		''' and so forth.) </p>
		''' 
		''' <p>All XMLReaders are required to recognize the
		''' http://xml.org/sax/features/namespaces and the
		''' http://xml.org/sax/features/namespace-prefixes feature names.</p>
		''' 
		''' <p>Typical usage is something like this:</p>
		''' 
		''' <pre>
		''' XMLReader r = new MySAXDriver();
		''' 
		'''                         // try to activate validation
		''' try {
		'''   r.setFeature("http://xml.org/sax/features/validation", true);
		''' } catch (SAXException e) {
		'''   System.err.println("Cannot activate validation.");
		''' }
		''' 
		'''                         // register event handlers
		''' r.setContentHandler(new MyContentHandler());
		''' r.setErrorHandler(new MyErrorHandler());
		''' 
		'''                         // parse the first document
		''' try {
		'''   r.parse("http://www.foo.com/mydoc.xml");
		''' } catch (IOException e) {
		'''   System.err.println("I/O exception reading XML document");
		''' } catch (SAXException e) {
		'''   System.err.println("XML exception reading document.");
		''' }
		''' </pre>
		''' 
		''' <p>Implementors are free (and encouraged) to invent their own features,
		''' using names built on their own URIs.</p>
		''' </summary>
		''' <param name="name"> The feature name, which is a fully-qualified URI. </param>
		''' <returns> The current value of the feature (true or false). </returns>
		''' <exception cref="org.xml.sax.SAXNotRecognizedException"> If the feature
		'''            value can't be assigned or retrieved. </exception>
		''' <exception cref="org.xml.sax.SAXNotSupportedException"> When the
		'''            XMLReader recognizes the feature name but
		'''            cannot determine its value at this time. </exception>
		''' <seealso cref= #setFeature </seealso>
		Function getFeature(ByVal name As String) As Boolean


		''' <summary>
		''' Set the value of a feature flag.
		''' 
		''' <p>The feature name is any fully-qualified URI.  It is
		''' possible for an XMLReader to expose a feature value but
		''' to be unable to change the current value.
		''' Some feature values may be immutable or mutable only
		''' in specific contexts, such as before, during, or after
		''' a parse.</p>
		''' 
		''' <p>All XMLReaders are required to support setting
		''' http://xml.org/sax/features/namespaces to true and
		''' http://xml.org/sax/features/namespace-prefixes to false.</p>
		''' </summary>
		''' <param name="name"> The feature name, which is a fully-qualified URI. </param>
		''' <param name="value"> The requested value of the feature (true or false). </param>
		''' <exception cref="org.xml.sax.SAXNotRecognizedException"> If the feature
		'''            value can't be assigned or retrieved. </exception>
		''' <exception cref="org.xml.sax.SAXNotSupportedException"> When the
		'''            XMLReader recognizes the feature name but
		'''            cannot set the requested value. </exception>
		''' <seealso cref= #getFeature </seealso>
		Sub setFeature(ByVal name As String, ByVal value As Boolean)


		''' <summary>
		''' Look up the value of a property.
		''' 
		''' <p>The property name is any fully-qualified URI.  It is
		''' possible for an XMLReader to recognize a property name but
		''' temporarily be unable to return its value.
		''' Some property values may be available only in specific
		''' contexts, such as before, during, or after a parse.</p>
		''' 
		''' <p>XMLReaders are not required to recognize any specific
		''' property names, though an initial core set is documented for
		''' SAX2.</p>
		''' 
		''' <p>Implementors are free (and encouraged) to invent their own properties,
		''' using names built on their own URIs.</p>
		''' </summary>
		''' <param name="name"> The property name, which is a fully-qualified URI. </param>
		''' <returns> The current value of the property. </returns>
		''' <exception cref="org.xml.sax.SAXNotRecognizedException"> If the property
		'''            value can't be assigned or retrieved. </exception>
		''' <exception cref="org.xml.sax.SAXNotSupportedException"> When the
		'''            XMLReader recognizes the property name but
		'''            cannot determine its value at this time. </exception>
		''' <seealso cref= #setProperty </seealso>
		Function getProperty(ByVal name As String) As Object


		''' <summary>
		''' Set the value of a property.
		''' 
		''' <p>The property name is any fully-qualified URI.  It is
		''' possible for an XMLReader to recognize a property name but
		''' to be unable to change the current value.
		''' Some property values may be immutable or mutable only
		''' in specific contexts, such as before, during, or after
		''' a parse.</p>
		''' 
		''' <p>XMLReaders are not required to recognize setting
		''' any specific property names, though a core set is defined by
		''' SAX2.</p>
		''' 
		''' <p>This method is also the standard mechanism for setting
		''' extended handlers.</p>
		''' </summary>
		''' <param name="name"> The property name, which is a fully-qualified URI. </param>
		''' <param name="value"> The requested value for the property. </param>
		''' <exception cref="org.xml.sax.SAXNotRecognizedException"> If the property
		'''            value can't be assigned or retrieved. </exception>
		''' <exception cref="org.xml.sax.SAXNotSupportedException"> When the
		'''            XMLReader recognizes the property name but
		'''            cannot set the requested value. </exception>
		Sub setProperty(ByVal name As String, ByVal value As Object)



		'//////////////////////////////////////////////////////////////////
		' Event handlers.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Allow an application to register an entity resolver.
		''' 
		''' <p>If the application does not register an entity resolver,
		''' the XMLReader will perform its own default resolution.</p>
		''' 
		''' <p>Applications may register a new or different resolver in the
		''' middle of a parse, and the SAX parser must begin using the new
		''' resolver immediately.</p>
		''' </summary>
		''' <param name="resolver"> The entity resolver. </param>
		''' <seealso cref= #getEntityResolver </seealso>
		Property entityResolver As EntityResolver




		''' <summary>
		''' Allow an application to register a DTD event handler.
		''' 
		''' <p>If the application does not register a DTD handler, all DTD
		''' events reported by the SAX parser will be silently ignored.</p>
		''' 
		''' <p>Applications may register a new or different handler in the
		''' middle of a parse, and the SAX parser must begin using the new
		''' handler immediately.</p>
		''' </summary>
		''' <param name="handler"> The DTD handler. </param>
		''' <seealso cref= #getDTDHandler </seealso>
		Property dTDHandler As DTDHandler




		''' <summary>
		''' Allow an application to register a content event handler.
		''' 
		''' <p>If the application does not register a content handler, all
		''' content events reported by the SAX parser will be silently
		''' ignored.</p>
		''' 
		''' <p>Applications may register a new or different handler in the
		''' middle of a parse, and the SAX parser must begin using the new
		''' handler immediately.</p>
		''' </summary>
		''' <param name="handler"> The content handler. </param>
		''' <seealso cref= #getContentHandler </seealso>
		Property contentHandler As ContentHandler




		''' <summary>
		''' Allow an application to register an error event handler.
		''' 
		''' <p>If the application does not register an error handler, all
		''' error events reported by the SAX parser will be silently
		''' ignored; however, normal processing may not continue.  It is
		''' highly recommended that all SAX applications implement an
		''' error handler to avoid unexpected bugs.</p>
		''' 
		''' <p>Applications may register a new or different handler in the
		''' middle of a parse, and the SAX parser must begin using the new
		''' handler immediately.</p>
		''' </summary>
		''' <param name="handler"> The error handler. </param>
		''' <seealso cref= #getErrorHandler </seealso>
		Property errorHandler As ErrorHandler





		'//////////////////////////////////////////////////////////////////
		' Parsing.
		'//////////////////////////////////////////////////////////////////

		''' <summary>
		''' Parse an XML document.
		''' 
		''' <p>The application can use this method to instruct the XML
		''' reader to begin parsing an XML document from any valid input
		''' source (a character stream, a byte stream, or a URI).</p>
		''' 
		''' <p>Applications may not invoke this method while a parse is in
		''' progress (they should create a new XMLReader instead for each
		''' nested XML document).  Once a parse is complete, an
		''' application may reuse the same XMLReader object, possibly with a
		''' different input source.
		''' Configuration of the XMLReader object (such as handler bindings and
		''' values established for feature flags and properties) is unchanged
		''' by completion of a parse, unless the definition of that aspect of
		''' the configuration explicitly specifies other behavior.
		''' (For example, feature flags or properties exposing
		''' characteristics of the document being parsed.)
		''' </p>
		''' 
		''' <p>During the parse, the XMLReader will provide information
		''' about the XML document through the registered event
		''' handlers.</p>
		''' 
		''' <p>This method is synchronous: it will not return until parsing
		''' has ended.  If a client application wants to terminate
		''' parsing early, it should throw an exception.</p>
		''' </summary>
		''' <param name="input"> The input source for the top-level of the
		'''        XML document. </param>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <exception cref="java.io.IOException"> An IO exception from the parser,
		'''            possibly from a byte stream or character stream
		'''            supplied by the application. </exception>
		''' <seealso cref= org.xml.sax.InputSource </seealso>
		''' <seealso cref= #parse(java.lang.String) </seealso>
		''' <seealso cref= #setEntityResolver </seealso>
		''' <seealso cref= #setDTDHandler </seealso>
		''' <seealso cref= #setContentHandler </seealso>
		''' <seealso cref= #setErrorHandler </seealso>
		Sub parse(ByVal input As InputSource)


		''' <summary>
		''' Parse an XML document from a system identifier (URI).
		''' 
		''' <p>This method is a shortcut for the common case of reading a
		''' document from a system identifier.  It is the exact
		''' equivalent of the following:</p>
		''' 
		''' <pre>
		''' parse(new InputSource(systemId));
		''' </pre>
		''' 
		''' <p>If the system identifier is a URL, it must be fully resolved
		''' by the application before it is passed to the parser.</p>
		''' </summary>
		''' <param name="systemId"> The system identifier (URI). </param>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <exception cref="java.io.IOException"> An IO exception from the parser,
		'''            possibly from a byte stream or character stream
		'''            supplied by the application. </exception>
		''' <seealso cref= #parse(org.xml.sax.InputSource) </seealso>
		Sub parse(ByVal systemId As String)

	End Interface

End Namespace
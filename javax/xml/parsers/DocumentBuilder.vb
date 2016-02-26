'
' * Copyright (c) 2000, 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.parsers





	''' <summary>
	''' Defines the API to obtain DOM Document instances from an XML
	''' document. Using this class, an application programmer can obtain a
	''' <seealso cref="Document"/> from XML.<p>
	''' 
	''' An instance of this class can be obtained from the
	''' <seealso cref="DocumentBuilderFactory#newDocumentBuilder()"/> method. Once
	''' an instance of this class is obtained, XML can be parsed from a
	''' variety of input sources. These input sources are InputStreams,
	''' Files, URLs, and SAX InputSources.<p>
	''' 
	''' Note that this class reuses several classes from the SAX API. This
	''' does not require that the implementor of the underlying DOM
	''' implementation use a SAX parser to parse XML document into a
	''' <code>Document</code>. It merely requires that the implementation
	''' communicate with the application using these existing APIs.
	''' 
	''' @author <a href="mailto:Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' </summary>

	Public MustInherit Class DocumentBuilder


		''' <summary>
		''' Protected constructor </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' <p>Reset this <code>DocumentBuilder</code> to its original configuration.</p>
		''' 
		''' <p><code>DocumentBuilder</code> is reset to the same state as when it was created with
		''' <seealso cref="DocumentBuilderFactory#newDocumentBuilder()"/>.
		''' <code>reset()</code> is designed to allow the reuse of existing <code>DocumentBuilder</code>s
		''' thus saving resources associated with the creation of new <code>DocumentBuilder</code>s.</p>
		''' 
		''' <p>The reset <code>DocumentBuilder</code> is not guaranteed to have the same <seealso cref="EntityResolver"/> or <seealso cref="ErrorHandler"/>
		''' <code>Object</code>s, e.g. <seealso cref="Object#equals(Object obj)"/>.  It is guaranteed to have a functionally equal
		''' <code>EntityResolver</code> and <code>ErrorHandler</code>.</p>
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> When implementation does not
		'''   override this method.
		''' 
		''' @since 1.5 </exception>
		Public Overridable Sub reset()

			' implementors should override this method
			Throw New System.NotSupportedException("This DocumentBuilder, """ & Me.GetType().name & """, does not support the reset functionality." & "  Specification """ & Me.GetType().Assembly.specificationTitle & """" & " version """ & Me.GetType().Assembly.specificationVersion & """")
		End Sub

		''' <summary>
		''' Parse the content of the given <code>InputStream</code> as an XML
		''' document and return a new DOM <seealso cref="Document"/> object.
		''' An <code>IllegalArgumentException</code> is thrown if the
		''' <code>InputStream</code> is null.
		''' </summary>
		''' <param name="is"> InputStream containing the content to be parsed.
		''' </param>
		''' <returns> <code>Document</code> result of parsing the
		'''  <code>InputStream</code>
		''' </returns>
		''' <exception cref="IOException"> If any IO errors occur. </exception>
		''' <exception cref="SAXException"> If any parse errors occur. </exception>
		''' <exception cref="IllegalArgumentException"> When <code>is</code> is <code>null</code>
		''' </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler </seealso>

		Public Overridable Function parse(ByVal [is] As java.io.InputStream) As org.w3c.dom.Document
			If [is] Is Nothing Then Throw New System.ArgumentException("InputStream cannot be null")

			Dim [in] As New org.xml.sax.InputSource([is])
			Return parse([in])
		End Function

		''' <summary>
		''' Parse the content of the given <code>InputStream</code> as an
		''' XML document and return a new DOM <seealso cref="Document"/> object.
		''' An <code>IllegalArgumentException</code> is thrown if the
		''' <code>InputStream</code> is null.
		''' </summary>
		''' <param name="is"> InputStream containing the content to be parsed. </param>
		''' <param name="systemId"> Provide a base for resolving relative URIs.
		''' </param>
		''' <returns> A new DOM Document object.
		''' </returns>
		''' <exception cref="IOException"> If any IO errors occur. </exception>
		''' <exception cref="SAXException"> If any parse errors occur. </exception>
		''' <exception cref="IllegalArgumentException"> When <code>is</code> is <code>null</code>
		''' </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler </seealso>

		Public Overridable Function parse(ByVal [is] As java.io.InputStream, ByVal systemId As String) As org.w3c.dom.Document
			If [is] Is Nothing Then Throw New System.ArgumentException("InputStream cannot be null")

			Dim [in] As New org.xml.sax.InputSource([is])
			[in].systemId = systemId
			Return parse([in])
		End Function

		''' <summary>
		''' Parse the content of the given URI as an XML document
		''' and return a new DOM <seealso cref="Document"/> object.
		''' An <code>IllegalArgumentException</code> is thrown if the
		''' URI is <code>null</code> null.
		''' </summary>
		''' <param name="uri"> The location of the content to be parsed.
		''' </param>
		''' <returns> A new DOM Document object.
		''' </returns>
		''' <exception cref="IOException"> If any IO errors occur. </exception>
		''' <exception cref="SAXException"> If any parse errors occur. </exception>
		''' <exception cref="IllegalArgumentException"> When <code>uri</code> is <code>null</code>
		''' </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler </seealso>

		Public Overridable Function parse(ByVal uri As String) As org.w3c.dom.Document
			If uri Is Nothing Then Throw New System.ArgumentException("URI cannot be null")

			Dim [in] As New org.xml.sax.InputSource(uri)
			Return parse([in])
		End Function

		''' <summary>
		''' Parse the content of the given file as an XML document
		''' and return a new DOM <seealso cref="Document"/> object.
		''' An <code>IllegalArgumentException</code> is thrown if the
		''' <code>File</code> is <code>null</code> null.
		''' </summary>
		''' <param name="f"> The file containing the XML to parse.
		''' </param>
		''' <exception cref="IOException"> If any IO errors occur. </exception>
		''' <exception cref="SAXException"> If any parse errors occur. </exception>
		''' <exception cref="IllegalArgumentException"> When <code>f</code> is <code>null</code>
		''' </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler </seealso>
		''' <returns> A new DOM Document object. </returns>

		Public Overridable Function parse(ByVal f As java.io.File) As org.w3c.dom.Document
			If f Is Nothing Then Throw New System.ArgumentException("File cannot be null")

			'convert file to appropriate URI, f.toURI().toASCIIString()
			'converts the URI to string as per rule specified in
			'RFC 2396,
			Dim [in] As New org.xml.sax.InputSource(f.toURI().toASCIIString())
			Return parse([in])
		End Function

		''' <summary>
		''' Parse the content of the given input source as an XML document
		''' and return a new DOM <seealso cref="Document"/> object.
		''' An <code>IllegalArgumentException</code> is thrown if the
		''' <code>InputSource</code> is <code>null</code> null.
		''' </summary>
		''' <param name="is"> InputSource containing the content to be parsed.
		''' </param>
		''' <returns> A new DOM Document object.
		''' </returns>
		''' <exception cref="IOException"> If any IO errors occur. </exception>
		''' <exception cref="SAXException"> If any parse errors occur. </exception>
		''' <exception cref="IllegalArgumentException"> When <code>is</code> is <code>null</code>
		''' </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler </seealso>

		Public MustOverride Function parse(ByVal [is] As org.xml.sax.InputSource) As org.w3c.dom.Document


		''' <summary>
		''' Indicates whether or not this parser is configured to
		''' understand namespaces.
		''' </summary>
		''' <returns> true if this parser is configured to understand
		'''         namespaces; false otherwise. </returns>

		Public MustOverride ReadOnly Property namespaceAware As Boolean

		''' <summary>
		''' Indicates whether or not this parser is configured to
		''' validate XML documents.
		''' </summary>
		''' <returns> true if this parser is configured to validate
		'''         XML documents; false otherwise. </returns>

		Public MustOverride ReadOnly Property validating As Boolean

		''' <summary>
		''' Specify the <seealso cref="EntityResolver"/> to be used to resolve
		''' entities present in the XML document to be parsed. Setting
		''' this to <code>null</code> will result in the underlying
		''' implementation using it's own default implementation and
		''' behavior.
		''' </summary>
		''' <param name="er"> The <code>EntityResolver</code> to be used to resolve entities
		'''           present in the XML document to be parsed. </param>

		Public MustOverride WriteOnly Property entityResolver As org.xml.sax.EntityResolver

		''' <summary>
		''' Specify the <seealso cref="ErrorHandler"/> to be used by the parser.
		''' Setting this to <code>null</code> will result in the underlying
		''' implementation using it's own default implementation and
		''' behavior.
		''' </summary>
		''' <param name="eh"> The <code>ErrorHandler</code> to be used by the parser. </param>

		Public MustOverride WriteOnly Property errorHandler As org.xml.sax.ErrorHandler

		''' <summary>
		''' Obtain a new instance of a DOM <seealso cref="Document"/> object
		''' to build a DOM tree with.
		''' </summary>
		''' <returns> A new instance of a DOM Document object. </returns>

		Public MustOverride Function newDocument() As org.w3c.dom.Document

		''' <summary>
		''' Obtain an instance of a <seealso cref="DOMImplementation"/> object.
		''' </summary>
		''' <returns> A new instance of a <code>DOMImplementation</code>. </returns>

		Public MustOverride ReadOnly Property dOMImplementation As org.w3c.dom.DOMImplementation

		''' <summary>
		''' <p>Get current state of canonicalization.</p>
		''' </summary>
		''' <returns> current state canonicalization control </returns>
	'    
	'    public boolean getCanonicalization() {
	'        return canonicalState;
	'    }
	'    

		''' <summary>
		''' <p>Get a reference to the the <seealso cref="Schema"/> being used by
		''' the XML processor.</p>
		''' 
		''' <p>If no schema is being used, <code>null</code> is returned.</p>
		''' </summary>
		''' <returns> <seealso cref="Schema"/> being used or <code>null</code>
		'''  if none in use
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> When implementation does not
		'''   override this method
		''' 
		''' @since 1.5 </exception>
		Public Overridable Property schema As javax.xml.validation.Schema
			Get
				Throw New System.NotSupportedException("This parser does not support specification """ & Me.GetType().Assembly.specificationTitle & """ version """ & Me.GetType().Assembly.specificationVersion & """")
			End Get
		End Property


		''' <summary>
		''' <p>Get the XInclude processing mode for this parser.</p>
		''' 
		''' @return
		'''      the return value of
		'''      the <seealso cref="DocumentBuilderFactory#isXIncludeAware()"/>
		'''      when this parser was created from factory.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> When implementation does not
		'''   override this method
		''' 
		''' @since 1.5
		''' </exception>
		''' <seealso cref= DocumentBuilderFactory#setXIncludeAware(boolean) </seealso>
		Public Overridable Property xIncludeAware As Boolean
			Get
				Throw New System.NotSupportedException("This parser does not support specification """ & Me.GetType().Assembly.specificationTitle & """ version """ & Me.GetType().Assembly.specificationVersion & """")
			End Get
		End Property
	End Class

End Namespace
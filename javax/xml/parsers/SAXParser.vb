'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Defines the API that wraps an <seealso cref="org.xml.sax.XMLReader"/>
	''' implementation class. In JAXP 1.0, this class wrapped the
	''' <seealso cref="org.xml.sax.Parser"/> interface, however this interface was
	''' replaced by the <seealso cref="org.xml.sax.XMLReader"/>. For ease
	''' of transition, this class continues to support the same name
	''' and interface as well as supporting new methods.
	''' 
	''' An instance of this class can be obtained from the
	''' <seealso cref="javax.xml.parsers.SAXParserFactory#newSAXParser()"/> method.
	''' Once an instance of this class is obtained, XML can be parsed from
	''' a variety of input sources. These input sources are InputStreams,
	''' Files, URLs, and SAX InputSources.<p>
	''' 
	''' This static method creates a new factory instance based
	''' on a system property setting or uses the platform default
	''' if no property has been defined.<p>
	''' 
	''' The system property that controls which Factory implementation
	''' to create is named <code>&quot;javax.xml.parsers.SAXParserFactory&quot;</code>.
	''' This property names a class that is a concrete subclass of this
	''' abstract class. If no property is defined, a platform default
	''' will be used.</p>
	''' 
	''' As the content is parsed by the underlying parser, methods of the
	''' given <seealso cref="org.xml.sax.HandlerBase"/> or the
	''' <seealso cref="org.xml.sax.helpers.DefaultHandler"/> are called.<p>
	''' 
	''' Implementors of this class which wrap an underlaying implementation
	''' can consider using the <seealso cref="org.xml.sax.helpers.ParserAdapter"/>
	''' class to initially adapt their SAX1 implementation to work under
	''' this revised class.
	''' 
	''' @author <a href="mailto:Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' </summary>
	Public MustInherit Class SAXParser

		''' <summary>
		''' <p>Protected constructor to prevent instaniation.
		''' Use <seealso cref="javax.xml.parsers.SAXParserFactory#newSAXParser()"/>.</p>
		''' </summary>
		Protected Friend Sub New()

		End Sub

			''' <summary>
			''' <p>Reset this <code>SAXParser</code> to its original configuration.</p>
			''' 
			''' <p><code>SAXParser</code> is reset to the same state as when it was created with
			''' <seealso cref="SAXParserFactory#newSAXParser()"/>.
			''' <code>reset()</code> is designed to allow the reuse of existing <code>SAXParser</code>s
			''' thus saving resources associated with the creation of new <code>SAXParser</code>s.</p>
			''' 
			''' <p>The reset <code>SAXParser</code> is not guaranteed to have the same <seealso cref="Schema"/>
			''' <code>Object</code>, e.g. <seealso cref="Object#equals(Object obj)"/>.  It is guaranteed to have a functionally equal
			''' <code>Schema</code>.</p>
			''' </summary>
			''' <exception cref="UnsupportedOperationException"> When Implementations do not
			'''   override this method
			''' 
			''' @since 1.5 </exception>
			Public Overridable Sub reset()

					' implementors should override this method
					Throw New System.NotSupportedException("This SAXParser, """ & Me.GetType().name & """, does not support the reset functionality." & "  Specification """ & Me.GetType().Assembly.specificationTitle & """" & " version """ & Me.GetType().Assembly.specificationVersion & """")
			End Sub

		''' <summary>
		''' <p>Parse the content of the given <seealso cref="java.io.InputStream"/>
		''' instance as XML using the specified <seealso cref="org.xml.sax.HandlerBase"/>.
		''' <i> Use of the DefaultHandler version of this method is recommended as
		''' the HandlerBase class has been deprecated in SAX 2.0</i>.</p>
		''' </summary>
		''' <param name="is"> InputStream containing the content to be parsed. </param>
		''' <param name="hb"> The SAX HandlerBase to use.
		''' </param>
		''' <exception cref="IllegalArgumentException"> If the given InputStream is null. </exception>
		''' <exception cref="SAXException"> If parse produces a SAX error. </exception>
		''' <exception cref="IOException"> If an IO error occurs interacting with the
		'''   <code>InputStream</code>.
		''' </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler </seealso>
		Public Overridable Sub parse(ByVal [is] As java.io.InputStream, ByVal hb As org.xml.sax.HandlerBase)
			If [is] Is Nothing Then Throw New System.ArgumentException("InputStream cannot be null")

			Dim input As New org.xml.sax.InputSource([is])
			Me.parse(input, hb)
		End Sub

		''' <summary>
		''' <p>Parse the content of the given <seealso cref="java.io.InputStream"/>
		''' instance as XML using the specified <seealso cref="org.xml.sax.HandlerBase"/>.
		''' <i> Use of the DefaultHandler version of this method is recommended as
		''' the HandlerBase class has been deprecated in SAX 2.0</i>.</p>
		''' </summary>
		''' <param name="is"> InputStream containing the content to be parsed. </param>
		''' <param name="hb"> The SAX HandlerBase to use. </param>
		''' <param name="systemId"> The systemId which is needed for resolving relative URIs.
		''' </param>
		''' <exception cref="IllegalArgumentException"> If the given <code>InputStream</code> is
		'''   <code>null</code>. </exception>
		''' <exception cref="IOException"> If any IO error occurs interacting with the
		'''   <code>InputStream</code>. </exception>
		''' <exception cref="SAXException"> If any SAX errors occur during processing.
		''' </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler version of this method instead. </seealso>
		Public Overridable Sub parse(ByVal [is] As java.io.InputStream, ByVal hb As org.xml.sax.HandlerBase, ByVal systemId As String)
			If [is] Is Nothing Then Throw New System.ArgumentException("InputStream cannot be null")

			Dim input As New org.xml.sax.InputSource([is])
			input.systemId = systemId
			Me.parse(input, hb)
		End Sub

		''' <summary>
		''' Parse the content of the given <seealso cref="java.io.InputStream"/>
		''' instance as XML using the specified
		''' <seealso cref="org.xml.sax.helpers.DefaultHandler"/>.
		''' </summary>
		''' <param name="is"> InputStream containing the content to be parsed. </param>
		''' <param name="dh"> The SAX DefaultHandler to use.
		''' </param>
		''' <exception cref="IllegalArgumentException"> If the given InputStream is null. </exception>
		''' <exception cref="IOException"> If any IO errors occur. </exception>
		''' <exception cref="SAXException"> If any SAX errors occur during processing.
		''' </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler </seealso>
		Public Overridable Sub parse(ByVal [is] As java.io.InputStream, ByVal dh As org.xml.sax.helpers.DefaultHandler)
			If [is] Is Nothing Then Throw New System.ArgumentException("InputStream cannot be null")

			Dim input As New org.xml.sax.InputSource([is])
			Me.parse(input, dh)
		End Sub

		''' <summary>
		''' Parse the content of the given <seealso cref="java.io.InputStream"/>
		''' instance as XML using the specified
		''' <seealso cref="org.xml.sax.helpers.DefaultHandler"/>.
		''' </summary>
		''' <param name="is"> InputStream containing the content to be parsed. </param>
		''' <param name="dh"> The SAX DefaultHandler to use. </param>
		''' <param name="systemId"> The systemId which is needed for resolving relative URIs.
		''' </param>
		''' <exception cref="IllegalArgumentException"> If the given InputStream is null. </exception>
		''' <exception cref="IOException"> If any IO errors occur. </exception>
		''' <exception cref="SAXException"> If any SAX errors occur during processing.
		''' </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler version of this method instead. </seealso>
		Public Overridable Sub parse(ByVal [is] As java.io.InputStream, ByVal dh As org.xml.sax.helpers.DefaultHandler, ByVal systemId As String)
			If [is] Is Nothing Then Throw New System.ArgumentException("InputStream cannot be null")

			Dim input As New org.xml.sax.InputSource([is])
			input.systemId = systemId
			Me.parse(input, dh)
		End Sub

		''' <summary>
		''' Parse the content described by the giving Uniform Resource
		''' Identifier (URI) as XML using the specified
		''' <seealso cref="org.xml.sax.HandlerBase"/>.
		''' <i> Use of the DefaultHandler version of this method is recommended as
		''' the <code>HandlerBase</code> class has been deprecated in SAX 2.0</i>
		''' </summary>
		''' <param name="uri"> The location of the content to be parsed. </param>
		''' <param name="hb"> The SAX HandlerBase to use.
		''' </param>
		''' <exception cref="IllegalArgumentException"> If the uri is null. </exception>
		''' <exception cref="IOException"> If any IO errors occur. </exception>
		''' <exception cref="SAXException"> If any SAX errors occur during processing.
		''' </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler </seealso>
		Public Overridable Sub parse(ByVal uri As String, ByVal hb As org.xml.sax.HandlerBase)
			If uri Is Nothing Then Throw New System.ArgumentException("uri cannot be null")

			Dim input As New org.xml.sax.InputSource(uri)
			Me.parse(input, hb)
		End Sub

		''' <summary>
		''' Parse the content described by the giving Uniform Resource
		''' Identifier (URI) as XML using the specified
		''' <seealso cref="org.xml.sax.helpers.DefaultHandler"/>.
		''' </summary>
		''' <param name="uri"> The location of the content to be parsed. </param>
		''' <param name="dh"> The SAX DefaultHandler to use.
		''' </param>
		''' <exception cref="IllegalArgumentException"> If the uri is null. </exception>
		''' <exception cref="IOException"> If any IO errors occur. </exception>
		''' <exception cref="SAXException"> If any SAX errors occur during processing.
		''' </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler </seealso>
		Public Overridable Sub parse(ByVal uri As String, ByVal dh As org.xml.sax.helpers.DefaultHandler)
			If uri Is Nothing Then Throw New System.ArgumentException("uri cannot be null")

			Dim input As New org.xml.sax.InputSource(uri)
			Me.parse(input, dh)
		End Sub

		''' <summary>
		''' Parse the content of the file specified as XML using the
		''' specified <seealso cref="org.xml.sax.HandlerBase"/>.
		''' <i> Use of the DefaultHandler version of this method is recommended as
		''' the HandlerBase class has been deprecated in SAX 2.0</i>
		''' </summary>
		''' <param name="f"> The file containing the XML to parse </param>
		''' <param name="hb"> The SAX HandlerBase to use.
		''' </param>
		''' <exception cref="IllegalArgumentException"> If the File object is null. </exception>
		''' <exception cref="IOException"> If any IO errors occur. </exception>
		''' <exception cref="SAXException"> If any SAX errors occur during processing.
		''' </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler </seealso>
		Public Overridable Sub parse(ByVal f As java.io.File, ByVal hb As org.xml.sax.HandlerBase)
			If f Is Nothing Then Throw New System.ArgumentException("File cannot be null")

			'convert file to appropriate URI, f.toURI().toASCIIString()
			'converts the URI to string as per rule specified in
			'RFC 2396,
			Dim input As New org.xml.sax.InputSource(f.toURI().toASCIIString())
			Me.parse(input, hb)
		End Sub

		''' <summary>
		''' Parse the content of the file specified as XML using the
		''' specified <seealso cref="org.xml.sax.helpers.DefaultHandler"/>.
		''' </summary>
		''' <param name="f"> The file containing the XML to parse </param>
		''' <param name="dh"> The SAX DefaultHandler to use.
		''' </param>
		''' <exception cref="IllegalArgumentException"> If the File object is null. </exception>
		''' <exception cref="IOException"> If any IO errors occur. </exception>
		''' <exception cref="SAXException"> If any SAX errors occur during processing.
		''' </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler </seealso>
		Public Overridable Sub parse(ByVal f As java.io.File, ByVal dh As org.xml.sax.helpers.DefaultHandler)
			If f Is Nothing Then Throw New System.ArgumentException("File cannot be null")

			'convert file to appropriate URI, f.toURI().toASCIIString()
			'converts the URI to string as per rule specified in
			'RFC 2396,
			Dim input As New org.xml.sax.InputSource(f.toURI().toASCIIString())
			Me.parse(input, dh)
		End Sub

		''' <summary>
		''' Parse the content given <seealso cref="org.xml.sax.InputSource"/>
		''' as XML using the specified
		''' <seealso cref="org.xml.sax.HandlerBase"/>.
		''' <i> Use of the DefaultHandler version of this method is recommended as
		''' the HandlerBase class has been deprecated in SAX 2.0</i>
		''' </summary>
		''' <param name="is"> The InputSource containing the content to be parsed. </param>
		''' <param name="hb"> The SAX HandlerBase to use.
		''' </param>
		''' <exception cref="IllegalArgumentException"> If the <code>InputSource</code> object
		'''   is <code>null</code>. </exception>
		''' <exception cref="IOException"> If any IO errors occur. </exception>
		''' <exception cref="SAXException"> If any SAX errors occur during processing.
		''' </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler </seealso>
		Public Overridable Sub parse(ByVal [is] As org.xml.sax.InputSource, ByVal hb As org.xml.sax.HandlerBase)
			If [is] Is Nothing Then Throw New System.ArgumentException("InputSource cannot be null")

			Dim ___parser As org.xml.sax.Parser = Me.parser
			If hb IsNot Nothing Then
				___parser.documentHandler = hb
				___parser.entityResolver = hb
				___parser.errorHandler = hb
				___parser.dTDHandler = hb
			End If
			___parser.parse([is])
		End Sub

		''' <summary>
		''' Parse the content given <seealso cref="org.xml.sax.InputSource"/>
		''' as XML using the specified
		''' <seealso cref="org.xml.sax.helpers.DefaultHandler"/>.
		''' </summary>
		''' <param name="is"> The InputSource containing the content to be parsed. </param>
		''' <param name="dh"> The SAX DefaultHandler to use.
		''' </param>
		''' <exception cref="IllegalArgumentException"> If the <code>InputSource</code> object
		'''   is <code>null</code>. </exception>
		''' <exception cref="IOException"> If any IO errors occur. </exception>
		''' <exception cref="SAXException"> If any SAX errors occur during processing.
		''' </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler </seealso>
		Public Overridable Sub parse(ByVal [is] As org.xml.sax.InputSource, ByVal dh As org.xml.sax.helpers.DefaultHandler)
			If [is] Is Nothing Then Throw New System.ArgumentException("InputSource cannot be null")

			Dim reader As org.xml.sax.XMLReader = Me.xMLReader
			If dh IsNot Nothing Then
				reader.contentHandler = dh
				reader.entityResolver = dh
				reader.errorHandler = dh
				reader.dTDHandler = dh
			End If
			reader.parse([is])
		End Sub

		''' <summary>
		''' Returns the SAX parser that is encapsultated by the
		''' implementation of this class.
		''' </summary>
		''' <returns> The SAX parser that is encapsultated by the
		'''         implementation of this class.
		''' </returns>
		''' <exception cref="SAXException"> If any SAX errors occur during processing. </exception>
		Public MustOverride ReadOnly Property parser As org.xml.sax.Parser

		''' <summary>
		''' Returns the <seealso cref="org.xml.sax.XMLReader"/> that is encapsulated by the
		''' implementation of this class.
		''' </summary>
		''' <returns> The XMLReader that is encapsulated by the
		'''         implementation of this class.
		''' </returns>
		''' <exception cref="SAXException"> If any SAX errors occur during processing. </exception>

		Public MustOverride ReadOnly Property xMLReader As org.xml.sax.XMLReader

		''' <summary>
		''' Indicates whether or not this parser is configured to
		''' understand namespaces.
		''' </summary>
		''' <returns> true if this parser is configured to
		'''         understand namespaces; false otherwise. </returns>

		Public MustOverride ReadOnly Property namespaceAware As Boolean

		''' <summary>
		''' Indicates whether or not this parser is configured to
		''' validate XML documents.
		''' </summary>
		''' <returns> true if this parser is configured to
		'''         validate XML documents; false otherwise. </returns>

		Public MustOverride ReadOnly Property validating As Boolean

		''' <summary>
		''' <p>Sets the particular property in the underlying implementation of
		''' <seealso cref="org.xml.sax.XMLReader"/>.
		''' A list of the core features and properties can be found at
		''' <a href="http://sax.sourceforge.net/?selected=get-set">
		''' http://sax.sourceforge.net/?selected=get-set</a>.</p>
		''' <p>
		''' All implementations that implement JAXP 1.5 or newer are required to
		''' support the <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_DTD"/> and
		''' <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_SCHEMA"/> properties.
		''' </p>
		''' <ul>
		'''   <li>
		'''      <p>
		'''      Setting the <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_DTD"/> property
		'''      restricts the access to external DTDs, external Entity References to
		'''      the protocols specified by the property.  If access is denied during parsing
		'''      due to the restriction of this property, <seealso cref="org.xml.sax.SAXException"/>
		'''      will be thrown by the parse methods defined by <seealso cref="javax.xml.parsers.SAXParser"/>.
		'''      </p>
		'''      <p>
		'''      Setting the <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_SCHEMA"/> property
		'''      restricts the access to external Schema set by the schemaLocation attribute to
		'''      the protocols specified by the property.  If access is denied during parsing
		'''      due to the restriction of this property, <seealso cref="org.xml.sax.SAXException"/>
		'''      will be thrown by the parse methods defined by the <seealso cref="javax.xml.parsers.SAXParser"/>.
		'''      </p>
		'''   </li>
		''' </ul>
		''' </summary>
		''' <param name="name"> The name of the property to be set. </param>
		''' <param name="value"> The value of the property to be set.
		''' </param>
		''' <exception cref="SAXNotRecognizedException"> When the underlying XMLReader does
		'''   not recognize the property name. </exception>
		''' <exception cref="SAXNotSupportedException"> When the underlying XMLReader
		'''  recognizes the property name but doesn't support the property.
		''' </exception>
		''' <seealso cref= org.xml.sax.XMLReader#setProperty </seealso>
		Public MustOverride Sub setProperty(ByVal name As String, ByVal value As Object)

		''' <summary>
		''' <p>Returns the particular property requested for in the underlying
		''' implementation of <seealso cref="org.xml.sax.XMLReader"/>.</p>
		''' </summary>
		''' <param name="name"> The name of the property to be retrieved. </param>
		''' <returns> Value of the requested property.
		''' </returns>
		''' <exception cref="SAXNotRecognizedException"> When the underlying XMLReader does
		'''    not recognize the property name. </exception>
		''' <exception cref="SAXNotSupportedException"> When the underlying XMLReader
		'''  recognizes the property name but doesn't support the property.
		''' </exception>
		''' <seealso cref= org.xml.sax.XMLReader#getProperty </seealso>
		Public MustOverride Function getProperty(ByVal name As String) As Object

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
		'''      the <seealso cref="SAXParserFactory#isXIncludeAware()"/>
		'''      when this parser was created from factory.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> When implementation does not
		'''   override this method
		''' 
		''' @since 1.5
		''' </exception>
		''' <seealso cref= SAXParserFactory#setXIncludeAware(boolean) </seealso>
		Public Overridable Property xIncludeAware As Boolean
			Get
				Throw New System.NotSupportedException("This parser does not support specification """ & Me.GetType().Assembly.specificationTitle & """ version """ & Me.GetType().Assembly.specificationVersion & """")
			End Get
		End Property
	End Class

End Namespace
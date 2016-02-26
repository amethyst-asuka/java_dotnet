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

Namespace javax.xml.transform.stream



	''' <summary>
	''' <p>Acts as an holder for a transformation Source in the form
	''' of a stream of XML markup.</p>
	''' 
	''' <p><em>Note:</em> Due to their internal use of either a <seealso cref="Reader"/> or <seealso cref="InputStream"/> instance,
	''' <code>StreamSource</code> instances may only be used once.</p>
	''' 
	''' @author <a href="Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' </summary>
	Public Class StreamSource
		Implements javax.xml.transform.Source

		''' <summary>
		''' If <seealso cref="javax.xml.transform.TransformerFactory#getFeature"/>
		''' returns true when passed this value as an argument,
		''' the Transformer supports Source input of this type.
		''' </summary>
		Public Const FEATURE As String = "http://javax.xml.transform.stream.StreamSource/feature"

		''' <summary>
		''' <p>Zero-argument default constructor.  If this constructor is used, and
		''' no Stream source is set using
		''' <seealso cref="#setInputStream(java.io.InputStream inputStream)"/> or
		''' <seealso cref="#setReader(java.io.Reader reader)"/>, then the
		''' <code>Transformer</code> will
		''' create an empty source <seealso cref="java.io.InputStream"/> using
		''' <seealso cref="java.io.InputStream#InputStream() new InputStream()"/>.</p>
		''' </summary>
		''' <seealso cref= javax.xml.transform.Transformer#transform(Source xmlSource, Result outputTarget) </seealso>
		Public Sub New()
		End Sub

		''' <summary>
		''' Construct a StreamSource from a byte stream.  Normally,
		''' a stream should be used rather than a reader, so
		''' the XML parser can resolve character encoding specified
		''' by the XML declaration.
		''' 
		''' <p>If this constructor is used to process a stylesheet, normally
		''' setSystemId should also be called, so that relative URI references
		''' can be resolved.</p>
		''' </summary>
		''' <param name="inputStream"> A valid InputStream reference to an XML stream. </param>
		Public Sub New(ByVal inputStream As java.io.InputStream)
			inputStream = inputStream
		End Sub

		''' <summary>
		''' Construct a StreamSource from a byte stream.  Normally,
		''' a stream should be used rather than a reader, so that
		''' the XML parser can resolve character encoding specified
		''' by the XML declaration.
		''' 
		''' <p>This constructor allows the systemID to be set in addition
		''' to the input stream, which allows relative URIs
		''' to be processed.</p>
		''' </summary>
		''' <param name="inputStream"> A valid InputStream reference to an XML stream. </param>
		''' <param name="systemId"> Must be a String that conforms to the URI syntax. </param>
		Public Sub New(ByVal inputStream As java.io.InputStream, ByVal systemId As String)
			inputStream = inputStream
			systemId = systemId
		End Sub

		''' <summary>
		''' Construct a StreamSource from a character reader.  Normally,
		''' a stream should be used rather than a reader, so that
		''' the XML parser can resolve character encoding specified
		''' by the XML declaration.  However, in many cases the encoding
		''' of the input stream is already resolved, as in the case of
		''' reading XML from a StringReader.
		''' </summary>
		''' <param name="reader"> A valid Reader reference to an XML character stream. </param>
		Public Sub New(ByVal reader As java.io.Reader)
			reader = reader
		End Sub

		''' <summary>
		''' Construct a StreamSource from a character reader.  Normally,
		''' a stream should be used rather than a reader, so that
		''' the XML parser may resolve character encoding specified
		''' by the XML declaration.  However, in many cases the encoding
		''' of the input stream is already resolved, as in the case of
		''' reading XML from a StringReader.
		''' </summary>
		''' <param name="reader"> A valid Reader reference to an XML character stream. </param>
		''' <param name="systemId"> Must be a String that conforms to the URI syntax. </param>
		Public Sub New(ByVal reader As java.io.Reader, ByVal systemId As String)
			reader = reader
			systemId = systemId
		End Sub

		''' <summary>
		''' Construct a StreamSource from a URL.
		''' </summary>
		''' <param name="systemId"> Must be a String that conforms to the URI syntax. </param>
		Public Sub New(ByVal systemId As String)
			Me.systemId = systemId
		End Sub

		''' <summary>
		''' Construct a StreamSource from a File.
		''' </summary>
		''' <param name="f"> Must a non-null File reference. </param>
		Public Sub New(ByVal f As java.io.File)
			'convert file to appropriate URI, f.toURI().toASCIIString()
			'converts the URI to string as per rule specified in
			'RFC 2396,
			systemId = f.toURI().toASCIIString()
		End Sub

		''' <summary>
		''' Set the byte stream to be used as input.  Normally,
		''' a stream should be used rather than a reader, so that
		''' the XML parser can resolve character encoding specified
		''' by the XML declaration.
		''' 
		''' <p>If this Source object is used to process a stylesheet, normally
		''' setSystemId should also be called, so that relative URL references
		''' can be resolved.</p>
		''' </summary>
		''' <param name="inputStream"> A valid InputStream reference to an XML stream. </param>
		Public Overridable Property inputStream As java.io.InputStream
			Set(ByVal inputStream As java.io.InputStream)
				Me.inputStream = inputStream
			End Set
			Get
				Return inputStream
			End Get
		End Property


		''' <summary>
		''' Set the input to be a character reader.  Normally,
		''' a stream should be used rather than a reader, so that
		''' the XML parser can resolve character encoding specified
		''' by the XML declaration.  However, in many cases the encoding
		''' of the input stream is already resolved, as in the case of
		''' reading XML from a StringReader.
		''' </summary>
		''' <param name="reader"> A valid Reader reference to an XML CharacterStream. </param>
		Public Overridable Property reader As java.io.Reader
			Set(ByVal reader As java.io.Reader)
				Me.reader = reader
			End Set
			Get
				Return reader
			End Get
		End Property


		''' <summary>
		''' Set the public identifier for this Source.
		''' 
		''' <p>The public identifier is always optional: if the application
		''' writer includes one, it will be provided as part of the
		''' location information.</p>
		''' </summary>
		''' <param name="publicId"> The public identifier as a string. </param>
		Public Overridable Property publicId As String
			Set(ByVal publicId As String)
				Me.publicId = publicId
			End Set
			Get
				Return publicId
			End Get
		End Property


		''' <summary>
		''' Set the system identifier for this Source.
		''' 
		''' <p>The system identifier is optional if there is a byte stream
		''' or a character stream, but it is still useful to provide one,
		''' since the application can use it to resolve relative URIs
		''' and can include it in error messages and warnings (the parser
		''' will attempt to open a connection to the URI only if
		''' there is no byte stream or character stream specified).</p>
		''' </summary>
		''' <param name="systemId"> The system identifier as a URL string. </param>
		Public Overridable Property systemId As String
			Set(ByVal systemId As String)
				Me.systemId = systemId
			End Set
			Get
				Return systemId
			End Get
		End Property


		''' <summary>
		''' Set the system ID from a File reference.
		''' </summary>
		''' <param name="f"> Must a non-null File reference. </param>
		Public Overridable Property systemId As java.io.File
			Set(ByVal f As java.io.File)
				'convert file to appropriate URI, f.toURI().toASCIIString()
				'converts the URI to string as per rule specified in
				'RFC 2396,
				Me.systemId = f.toURI().toASCIIString()
			End Set
		End Property

		'////////////////////////////////////////////////////////////////////
		' Internal state.
		'////////////////////////////////////////////////////////////////////

		''' <summary>
		''' The public identifier for this input source, or null.
		''' </summary>
		Private publicId As String

		''' <summary>
		''' The system identifier as a URL string, or null.
		''' </summary>
		Private systemId As String

		''' <summary>
		''' The byte stream for this Source, or null.
		''' </summary>
		Private inputStream As java.io.InputStream

		''' <summary>
		''' The character stream for this Source, or null.
		''' </summary>
		Private reader As java.io.Reader
	End Class

End Namespace
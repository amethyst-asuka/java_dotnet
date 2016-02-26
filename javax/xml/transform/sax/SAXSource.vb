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

Namespace javax.xml.transform.sax



	''' <summary>
	''' <p>Acts as an holder for SAX-style Source.</p>
	''' 
	''' <p>Note that XSLT requires namespace support. Attempting to transform an
	''' input source that is not
	''' generated with a namespace-aware parser may result in errors.
	''' Parsers can be made namespace aware by calling the
	''' <seealso cref="javax.xml.parsers.SAXParserFactory#setNamespaceAware(boolean awareness)"/> method.</p>
	''' 
	''' @author <a href="mailto:Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' </summary>
	Public Class SAXSource
		Implements javax.xml.transform.Source

		''' <summary>
		''' If <seealso cref="javax.xml.transform.TransformerFactory#getFeature"/>
		''' returns true when passed this value as an argument,
		''' the Transformer supports Source input of this type.
		''' </summary>
		Public Const FEATURE As String = "http://javax.xml.transform.sax.SAXSource/feature"

		''' <summary>
		''' <p>Zero-argument default constructor.  If this constructor is used, and
		''' no SAX source is set using
		''' <seealso cref="#setInputSource(InputSource inputSource)"/> , then the
		''' <code>Transformer</code> will
		''' create an empty source <seealso cref="org.xml.sax.InputSource"/> using
		''' <seealso cref="org.xml.sax.InputSource#InputSource() new InputSource()"/>.</p>
		''' </summary>
		''' <seealso cref= javax.xml.transform.Transformer#transform(Source xmlSource, Result outputTarget) </seealso>
		Public Sub New()
		End Sub

		''' <summary>
		''' Create a <code>SAXSource</code>, using an <seealso cref="org.xml.sax.XMLReader"/>
		''' and a SAX InputSource. The <seealso cref="javax.xml.transform.Transformer"/>
		''' or <seealso cref="javax.xml.transform.sax.SAXTransformerFactory"/> will set itself
		''' to be the reader's <seealso cref="org.xml.sax.ContentHandler"/>, and then will call
		''' reader.parse(inputSource).
		''' </summary>
		''' <param name="reader"> An XMLReader to be used for the parse. </param>
		''' <param name="inputSource"> A SAX input source reference that must be non-null
		''' and that will be passed to the reader parse method. </param>
		Public Sub New(ByVal reader As org.xml.sax.XMLReader, ByVal inputSource As org.xml.sax.InputSource)
			Me.reader = reader
			Me.inputSource = inputSource
		End Sub

		''' <summary>
		''' Create a <code>SAXSource</code>, using a SAX <code>InputSource</code>.
		''' The <seealso cref="javax.xml.transform.Transformer"/> or
		''' <seealso cref="javax.xml.transform.sax.SAXTransformerFactory"/> creates a
		''' reader via <seealso cref="org.xml.sax.helpers.XMLReaderFactory"/>
		''' (if setXMLReader is not used), sets itself as
		''' the reader's <seealso cref="org.xml.sax.ContentHandler"/>, and calls
		''' reader.parse(inputSource).
		''' </summary>
		''' <param name="inputSource"> An input source reference that must be non-null
		''' and that will be passed to the parse method of the reader. </param>
		Public Sub New(ByVal inputSource As org.xml.sax.InputSource)
			Me.inputSource = inputSource
		End Sub

		''' <summary>
		''' Set the XMLReader to be used for the Source.
		''' </summary>
		''' <param name="reader"> A valid XMLReader or XMLFilter reference. </param>
		Public Overridable Property xMLReader As org.xml.sax.XMLReader
			Set(ByVal reader As org.xml.sax.XMLReader)
				Me.reader = reader
			End Set
			Get
				Return reader
			End Get
		End Property


		''' <summary>
		''' Set the SAX InputSource to be used for the Source.
		''' </summary>
		''' <param name="inputSource"> A valid InputSource reference. </param>
		Public Overridable Property inputSource As org.xml.sax.InputSource
			Set(ByVal inputSource As org.xml.sax.InputSource)
				Me.inputSource = inputSource
			End Set
			Get
				Return inputSource
			End Get
		End Property


		''' <summary>
		''' Set the system identifier for this Source.  If an input source
		''' has already been set, it will set the system ID or that
		''' input source, otherwise it will create a new input source.
		''' 
		''' <p>The system identifier is optional if there is a byte stream
		''' or a character stream, but it is still useful to provide one,
		''' since the application can use it to resolve relative URIs
		''' and can include it in error messages and warnings (the parser
		''' will attempt to open a connection to the URI only if
		''' no byte stream or character stream is specified).</p>
		''' </summary>
		''' <param name="systemId"> The system identifier as a URI string. </param>
		Public Overridable Property systemId As String
			Set(ByVal systemId As String)
    
				If Nothing Is inputSource Then
					inputSource = New org.xml.sax.InputSource(systemId)
				Else
					inputSource.systemId = systemId
				End If
			End Set
			Get
    
				If inputSource Is Nothing Then
					Return Nothing
				Else
					Return inputSource.systemId
				End If
			End Get
		End Property


		''' <summary>
		''' The XMLReader to be used for the source tree input. May be null.
		''' </summary>
		Private reader As org.xml.sax.XMLReader

		''' <summary>
		''' <p>The SAX InputSource to be used for the source tree input.
		''' Should not be <code>null</code>.</p>
		''' </summary>
		Private inputSource As org.xml.sax.InputSource

		''' <summary>
		''' Attempt to obtain a SAX InputSource object from a Source
		''' object.
		''' </summary>
		''' <param name="source"> Must be a non-null Source reference.
		''' </param>
		''' <returns> An InputSource, or null if Source can not be converted. </returns>
		Public Shared Function sourceToInputSource(ByVal source As javax.xml.transform.Source) As org.xml.sax.InputSource

			If TypeOf source Is SAXSource Then
				Return CType(source, SAXSource).inputSource
			ElseIf TypeOf source Is javax.xml.transform.stream.StreamSource Then
				Dim ss As javax.xml.transform.stream.StreamSource = CType(source, javax.xml.transform.stream.StreamSource)
				Dim isource As New org.xml.sax.InputSource(ss.systemId)

				isource.byteStream = ss.inputStream
				isource.characterStream = ss.reader
				isource.publicId = ss.publicId

				Return isource
			Else
				Return Nothing
			End If
		End Function
	End Class

End Namespace
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

' SAX input source.
' http://www.saxproject.org
' No warranty; no copyright -- use this as you will.
' $Id: InputSource.java,v 1.2 2004/11/03 22:55:32 jsuttor Exp $

Namespace org.xml.sax


	''' <summary>
	''' A single input source for an XML entity.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' See <a href='http://www.saxproject.org'>http://www.saxproject.org</a>
	''' for further information.
	''' </blockquote>
	''' 
	''' <p>This class allows a SAX application to encapsulate information
	''' about an input source in a single object, which may include
	''' a public identifier, a system identifier, a byte stream (possibly
	''' with a specified encoding), and/or a character stream.</p>
	''' 
	''' <p>There are two places that the application can deliver an
	''' input source to the parser: as the argument to the Parser.parse
	''' method, or as the return value of the EntityResolver.resolveEntity
	''' method.</p>
	''' 
	''' <p>The SAX parser will use the InputSource object to determine how
	''' to read XML input.  If there is a character stream available, the
	''' parser will read that stream directly, disregarding any text
	''' encoding declaration found in that stream.
	''' If there is no character stream, but there is
	''' a byte stream, the parser will use that byte stream, using the
	''' encoding specified in the InputSource or else (if no encoding is
	''' specified) autodetecting the character encoding using an algorithm
	''' such as the one in the XML specification.  If neither a character
	''' stream nor a
	''' byte stream is available, the parser will attempt to open a URI
	''' connection to the resource identified by the system
	''' identifier.</p>
	''' 
	''' <p>An InputSource object belongs to the application: the SAX parser
	''' shall never modify it in any way (it may modify a copy if
	''' necessary).  However, standard processing of both byte and
	''' character streams is to close them on as part of end-of-parse cleanup,
	''' so applications should not attempt to re-use such streams after they
	''' have been handed to a parser.  </p>
	''' 
	''' @since SAX 1.0
	''' @author David Megginson </summary>
	''' <seealso cref= org.xml.sax.XMLReader#parse(org.xml.sax.InputSource) </seealso>
	''' <seealso cref= org.xml.sax.EntityResolver#resolveEntity </seealso>
	''' <seealso cref= java.io.InputStream </seealso>
	''' <seealso cref= java.io.Reader </seealso>
	Public Class InputSource

		''' <summary>
		''' Zero-argument default constructor.
		''' </summary>
		''' <seealso cref= #setPublicId </seealso>
		''' <seealso cref= #setSystemId </seealso>
		''' <seealso cref= #setByteStream </seealso>
		''' <seealso cref= #setCharacterStream </seealso>
		''' <seealso cref= #setEncoding </seealso>
		Public Sub New()
		End Sub


		''' <summary>
		''' Create a new input source with a system identifier.
		''' 
		''' <p>Applications may use setPublicId to include a
		''' public identifier as well, or setEncoding to specify
		''' the character encoding, if known.</p>
		''' 
		''' <p>If the system identifier is a URL, it must be fully
		''' resolved (it may not be a relative URL).</p>
		''' </summary>
		''' <param name="systemId"> The system identifier (URI). </param>
		''' <seealso cref= #setPublicId </seealso>
		''' <seealso cref= #setSystemId </seealso>
		''' <seealso cref= #setByteStream </seealso>
		''' <seealso cref= #setEncoding </seealso>
		''' <seealso cref= #setCharacterStream </seealso>
		Public Sub New(ByVal systemId As String)
			systemId = systemId
		End Sub


		''' <summary>
		''' Create a new input source with a byte stream.
		''' 
		''' <p>Application writers should use setSystemId() to provide a base
		''' for resolving relative URIs, may use setPublicId to include a
		''' public identifier, and may use setEncoding to specify the object's
		''' character encoding.</p>
		''' </summary>
		''' <param name="byteStream"> The raw byte stream containing the document. </param>
		''' <seealso cref= #setPublicId </seealso>
		''' <seealso cref= #setSystemId </seealso>
		''' <seealso cref= #setEncoding </seealso>
		''' <seealso cref= #setByteStream </seealso>
		''' <seealso cref= #setCharacterStream </seealso>
		Public Sub New(ByVal byteStream As java.io.InputStream)
			byteStream = byteStream
		End Sub


		''' <summary>
		''' Create a new input source with a character stream.
		''' 
		''' <p>Application writers should use setSystemId() to provide a base
		''' for resolving relative URIs, and may use setPublicId to include a
		''' public identifier.</p>
		''' 
		''' <p>The character stream shall not include a byte order mark.</p>
		''' </summary>
		''' <seealso cref= #setPublicId </seealso>
		''' <seealso cref= #setSystemId </seealso>
		''' <seealso cref= #setByteStream </seealso>
		''' <seealso cref= #setCharacterStream </seealso>
		Public Sub New(ByVal characterStream As java.io.Reader)
			characterStream = characterStream
		End Sub


		''' <summary>
		''' Set the public identifier for this input source.
		''' 
		''' <p>The public identifier is always optional: if the application
		''' writer includes one, it will be provided as part of the
		''' location information.</p>
		''' </summary>
		''' <param name="publicId"> The public identifier as a string. </param>
		''' <seealso cref= #getPublicId </seealso>
		''' <seealso cref= org.xml.sax.Locator#getPublicId </seealso>
		''' <seealso cref= org.xml.sax.SAXParseException#getPublicId </seealso>
		Public Overridable Property publicId As String
			Set(ByVal publicId As String)
				Me.publicId = publicId
			End Set
			Get
				Return publicId
			End Get
		End Property




		''' <summary>
		''' Set the system identifier for this input source.
		''' 
		''' <p>The system identifier is optional if there is a byte stream
		''' or a character stream, but it is still useful to provide one,
		''' since the application can use it to resolve relative URIs
		''' and can include it in error messages and warnings (the parser
		''' will attempt to open a connection to the URI only if
		''' there is no byte stream or character stream specified).</p>
		''' 
		''' <p>If the application knows the character encoding of the
		''' object pointed to by the system identifier, it can register
		''' the encoding using the setEncoding method.</p>
		''' 
		''' <p>If the system identifier is a URL, it must be fully
		''' resolved (it may not be a relative URL).</p>
		''' </summary>
		''' <param name="systemId"> The system identifier as a string. </param>
		''' <seealso cref= #setEncoding </seealso>
		''' <seealso cref= #getSystemId </seealso>
		''' <seealso cref= org.xml.sax.Locator#getSystemId </seealso>
		''' <seealso cref= org.xml.sax.SAXParseException#getSystemId </seealso>
		Public Overridable Property systemId As String
			Set(ByVal systemId As String)
				Me.systemId = systemId
			End Set
			Get
				Return systemId
			End Get
		End Property




		''' <summary>
		''' Set the byte stream for this input source.
		''' 
		''' <p>The SAX parser will ignore this if there is also a character
		''' stream specified, but it will use a byte stream in preference
		''' to opening a URI connection itself.</p>
		''' 
		''' <p>If the application knows the character encoding of the
		''' byte stream, it should set it with the setEncoding method.</p>
		''' </summary>
		''' <param name="byteStream"> A byte stream containing an XML document or
		'''        other entity. </param>
		''' <seealso cref= #setEncoding </seealso>
		''' <seealso cref= #getByteStream </seealso>
		''' <seealso cref= #getEncoding </seealso>
		''' <seealso cref= java.io.InputStream </seealso>
		Public Overridable Property byteStream As java.io.InputStream
			Set(ByVal byteStream As java.io.InputStream)
				Me.byteStream = byteStream
			End Set
			Get
				Return byteStream
			End Get
		End Property




		''' <summary>
		''' Set the character encoding, if known.
		''' 
		''' <p>The encoding must be a string acceptable for an
		''' XML encoding declaration (see section 4.3.3 of the XML 1.0
		''' recommendation).</p>
		''' 
		''' <p>This method has no effect when the application provides a
		''' character stream.</p>
		''' </summary>
		''' <param name="encoding"> A string describing the character encoding. </param>
		''' <seealso cref= #setSystemId </seealso>
		''' <seealso cref= #setByteStream </seealso>
		''' <seealso cref= #getEncoding </seealso>
		Public Overridable Property encoding As String
			Set(ByVal encoding As String)
				Me.encoding = encoding
			End Set
			Get
				Return encoding
			End Get
		End Property




		''' <summary>
		''' Set the character stream for this input source.
		''' 
		''' <p>If there is a character stream specified, the SAX parser
		''' will ignore any byte stream and will not attempt to open
		''' a URI connection to the system identifier.</p>
		''' </summary>
		''' <param name="characterStream"> The character stream containing the
		'''        XML document or other entity. </param>
		''' <seealso cref= #getCharacterStream </seealso>
		''' <seealso cref= java.io.Reader </seealso>
		Public Overridable Property characterStream As java.io.Reader
			Set(ByVal characterStream As java.io.Reader)
				Me.characterStream = characterStream
			End Set
			Get
				Return characterStream
			End Get
		End Property





		'//////////////////////////////////////////////////////////////////
		' Internal state.
		'//////////////////////////////////////////////////////////////////

		Private publicId As String
		Private systemId As String
		Private byteStream As java.io.InputStream
		Private encoding As String
		Private characterStream As java.io.Reader

	End Class

	' end of InputSource.java

End Namespace
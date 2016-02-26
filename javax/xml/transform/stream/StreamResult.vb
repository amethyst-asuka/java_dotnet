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
	''' <p>Acts as an holder for a transformation result,
	''' which may be XML, plain Text, HTML, or some other form of markup.</p>
	''' 
	''' @author <a href="Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' </summary>
	Public Class StreamResult
		Implements javax.xml.transform.Result

		''' <summary>
		''' If <seealso cref="javax.xml.transform.TransformerFactory#getFeature"/>
		''' returns true when passed this value as an argument,
		''' the Transformer supports Result output of this type.
		''' </summary>
		Public Const FEATURE As String = "http://javax.xml.transform.stream.StreamResult/feature"

		''' <summary>
		''' Zero-argument default constructor.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Construct a StreamResult from a byte stream.  Normally,
		''' a stream should be used rather than a reader, so that
		''' the transformer may use instructions contained in the
		''' transformation instructions to control the encoding.
		''' </summary>
		''' <param name="outputStream"> A valid OutputStream reference. </param>
		Public Sub New(ByVal outputStream As java.io.OutputStream)
			outputStream = outputStream
		End Sub

		''' <summary>
		''' Construct a StreamResult from a character stream.  Normally,
		''' a stream should be used rather than a reader, so that
		''' the transformer may use instructions contained in the
		''' transformation instructions to control the encoding.  However,
		''' there are times when it is useful to write to a character
		''' stream, such as when using a StringWriter.
		''' </summary>
		''' <param name="writer">  A valid Writer reference. </param>
		Public Sub New(ByVal writer As java.io.Writer)
			writer = writer
		End Sub

		''' <summary>
		''' Construct a StreamResult from a URL.
		''' </summary>
		''' <param name="systemId"> Must be a String that conforms to the URI syntax. </param>
		Public Sub New(ByVal systemId As String)
			Me.systemId = systemId
		End Sub

		''' <summary>
		''' Construct a StreamResult from a File.
		''' </summary>
		''' <param name="f"> Must a non-null File reference. </param>
		Public Sub New(ByVal f As java.io.File)
			'convert file to appropriate URI, f.toURI().toASCIIString()
			'converts the URI to string as per rule specified in
			'RFC 2396,
			systemId = f.toURI().toASCIIString()
		End Sub

		''' <summary>
		''' Set the ByteStream that is to be written to.  Normally,
		''' a stream should be used rather than a reader, so that
		''' the transformer may use instructions contained in the
		''' transformation instructions to control the encoding.
		''' </summary>
		''' <param name="outputStream"> A valid OutputStream reference. </param>
		Public Overridable Property outputStream As java.io.OutputStream
			Set(ByVal outputStream As java.io.OutputStream)
				Me.outputStream = outputStream
			End Set
			Get
				Return outputStream
			End Get
		End Property


		''' <summary>
		''' Set the writer that is to receive the result.  Normally,
		''' a stream should be used rather than a writer, so that
		''' the transformer may use instructions contained in the
		''' transformation instructions to control the encoding.  However,
		''' there are times when it is useful to write to a writer,
		''' such as when using a StringWriter.
		''' </summary>
		''' <param name="writer">  A valid Writer reference. </param>
		Public Overridable Property writer As java.io.Writer
			Set(ByVal writer As java.io.Writer)
				Me.writer = writer
			End Set
			Get
				Return writer
			End Get
		End Property


		''' <summary>
		''' Set the systemID that may be used in association
		''' with the byte or character stream, or, if neither is set, use
		''' this value as a writeable URI (probably a file name).
		''' </summary>
		''' <param name="systemId"> The system identifier as a URI string. </param>
		Public Overridable Property systemId As String
			Set(ByVal systemId As String)
				Me.systemId = systemId
			End Set
			Get
				Return systemId
			End Get
		End Property

		''' <summary>
		''' <p>Set the system ID from a <code>File</code> reference.</p>
		''' 
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
		''' The systemID that may be used in association
		''' with the byte or character stream, or, if neither is set, use
		''' this value as a writeable URI (probably a file name).
		''' </summary>
		Private systemId As String

		''' <summary>
		''' The byte stream that is to be written to.
		''' </summary>
		Private outputStream As java.io.OutputStream

		''' <summary>
		''' The character stream that is to be written to.
		''' </summary>
		Private writer As java.io.Writer
	End Class

End Namespace
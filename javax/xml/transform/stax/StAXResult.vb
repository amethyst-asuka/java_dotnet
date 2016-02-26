'
' * Copyright (c) 2005, 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.transform.stax


	''' <summary>
	''' <p>Acts as a holder for an XML <seealso cref="Result"/> in the
	''' form of a StAX writer,i.e.
	''' <seealso cref="XMLStreamWriter"/> or <seealso cref="XMLEventWriter"/>.
	''' <code>StAXResult</code> can be used in all cases that accept
	''' a <code>Result</code>, e.g. <seealso cref="javax.xml.transform.Transformer"/>,
	''' <seealso cref="javax.xml.validation.Validator"/> which accept
	''' <code>Result</code> as input.
	''' 
	''' @author <a href="mailto:Neeraj.Bajaj@Sun.com">Neeraj Bajaj</a>
	''' @author <a href="mailto:Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' </summary>
	''' <seealso cref= <a href="http://jcp.org/en/jsr/detail?id=173">
	'''  JSR 173: Streaming API for XML</a> </seealso>
	''' <seealso cref= XMLStreamWriter </seealso>
	''' <seealso cref= XMLEventWriter
	''' 
	''' @since 1.6 </seealso>
	Public Class StAXResult
		Implements javax.xml.transform.Result

		''' <summary>
		''' If <seealso cref="javax.xml.transform.TransformerFactory#getFeature(String name)"/>
		''' returns true when passed this value as an argument,
		''' the Transformer supports Result output of this type.
		''' </summary>
		Public Const FEATURE As String = "http://javax.xml.transform.stax.StAXResult/feature"

		''' <summary>
		''' <p><code>XMLEventWriter</code> to be used for
		''' <code>Result</code> output.</p>
		''' </summary>
		Private xmlEventWriter As javax.xml.stream.XMLEventWriter = Nothing

		''' <summary>
		''' <p><code>XMLStreamWriter</code> to be used for
		''' <code>Result</code> output.</p>
		''' </summary>
		Private xmlStreamWriter As javax.xml.stream.XMLStreamWriter = Nothing

		''' <summary>
		''' <p>System identifier for this <code>StAXResult</code>.<p> </summary>
		Private systemId As String = Nothing

		''' <summary>
		''' <p>Creates a new instance of a <code>StAXResult</code>
		''' by supplying an <seealso cref="XMLEventWriter"/>.</p>
		''' 
		''' <p><code>XMLEventWriter</code> must be a
		''' non-<code>null</code> reference.</p>
		''' </summary>
		''' <param name="xmlEventWriter"> <code>XMLEventWriter</code> used to create
		'''   this <code>StAXResult</code>.
		''' </param>
		''' <exception cref="IllegalArgumentException"> If <code>xmlEventWriter</code> ==
		'''   <code>null</code>. </exception>
		Public Sub New(ByVal xmlEventWriter As javax.xml.stream.XMLEventWriter)

			If xmlEventWriter Is Nothing Then Throw New System.ArgumentException("StAXResult(XMLEventWriter) with XMLEventWriter == null")

			Me.xmlEventWriter = xmlEventWriter
		End Sub

		''' <summary>
		''' <p>Creates a new instance of a <code>StAXResult</code>
		''' by supplying an <seealso cref="XMLStreamWriter"/>.</p>
		''' 
		''' <p><code>XMLStreamWriter</code> must be a
		''' non-<code>null</code> reference.</p>
		''' </summary>
		''' <param name="xmlStreamWriter"> <code>XMLStreamWriter</code> used to create
		'''   this <code>StAXResult</code>.
		''' </param>
		''' <exception cref="IllegalArgumentException"> If <code>xmlStreamWriter</code> ==
		'''   <code>null</code>. </exception>
		Public Sub New(ByVal xmlStreamWriter As javax.xml.stream.XMLStreamWriter)

			If xmlStreamWriter Is Nothing Then Throw New System.ArgumentException("StAXResult(XMLStreamWriter) with XMLStreamWriter == null")

			Me.xmlStreamWriter = xmlStreamWriter
		End Sub

		''' <summary>
		''' <p>Get the <code>XMLEventWriter</code> used by this
		''' <code>StAXResult</code>.</p>
		''' 
		''' <p><code>XMLEventWriter</code> will be <code>null</code>
		''' if this <code>StAXResult</code> was created with a
		''' <code>XMLStreamWriter</code>.</p>
		''' </summary>
		''' <returns> <code>XMLEventWriter</code> used by this
		'''   <code>StAXResult</code>. </returns>
		Public Overridable Property xMLEventWriter As javax.xml.stream.XMLEventWriter
			Get
    
				Return xmlEventWriter
			End Get
		End Property

		''' <summary>
		''' <p>Get the <code>XMLStreamWriter</code> used by this
		''' <code>StAXResult</code>.</p>
		''' 
		''' <p><code>XMLStreamWriter</code> will be <code>null</code>
		''' if this <code>StAXResult</code> was created with a
		''' <code>XMLEventWriter</code>.</p>
		''' </summary>
		''' <returns> <code>XMLStreamWriter</code> used by this
		'''   <code>StAXResult</code>. </returns>
		Public Overridable Property xMLStreamWriter As javax.xml.stream.XMLStreamWriter
			Get
    
				Return xmlStreamWriter
			End Get
		End Property

		''' <summary>
		''' <p>In the context of a <code>StAXResult</code>, it is not appropriate
		''' to explicitly set the system identifier.
		''' The <code>XMLEventWriter</code> or <code>XMLStreamWriter</code>
		''' used to construct this <code>StAXResult</code> determines the
		''' system identifier of the XML result.</p>
		''' 
		''' <p>An <seealso cref="UnsupportedOperationException"/> is <strong>always</strong>
		''' thrown by this method.</p>
		''' </summary>
		''' <param name="systemId"> Ignored.
		''' </param>
		''' <exception cref="UnsupportedOperationException"> Is <strong>always</strong>
		'''   thrown by this method. </exception>
		Public Overridable Property systemId As String
			Set(ByVal systemId As String)
    
				Throw New System.NotSupportedException("StAXResult#setSystemId(systemId) cannot set the " & "system identifier for a StAXResult")
			End Set
			Get
    
				Return Nothing
			End Get
		End Property

	End Class

End Namespace
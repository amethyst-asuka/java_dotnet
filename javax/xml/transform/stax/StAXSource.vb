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
	''' <p>Acts as a holder for an XML <seealso cref="Source"/> in the
	''' form of a StAX reader,i.e.
	''' <seealso cref="XMLStreamReader"/> or <seealso cref="XMLEventReader"/>.
	''' <code>StAXSource</code> can be used in all cases that accept
	''' a <code>Source</code>, e.g. <seealso cref="javax.xml.transform.Transformer"/>,
	''' <seealso cref="javax.xml.validation.Validator"/> which accept
	''' <code>Source</code> as input.
	''' 
	''' <p><code>StAXSource</code>s are consumed during processing
	''' and are not reusable.</p>
	''' 
	''' @author <a href="mailto:Neeraj.Bajaj@Sun.com">Neeraj Bajaj</a>
	''' @author <a href="mailto:Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' </summary>
	''' <seealso cref= <a href="http://jcp.org/en/jsr/detail?id=173">
	'''  JSR 173: Streaming API for XML</a> </seealso>
	''' <seealso cref= XMLStreamReader </seealso>
	''' <seealso cref= XMLEventReader
	''' 
	''' @since 1.6 </seealso>
	Public Class StAXSource
		Implements javax.xml.transform.Source

		''' <summary>
		''' If <seealso cref="javax.xml.transform.TransformerFactory#getFeature(String name)"/>
		''' returns true when passed this value as an argument,
		''' the Transformer supports Source input of this type.
		''' </summary>
		Public Const FEATURE As String = "http://javax.xml.transform.stax.StAXSource/feature"

		''' <summary>
		''' <p><code>XMLEventReader</code> to be used for source input.</p> </summary>
		Private xmlEventReader As javax.xml.stream.XMLEventReader = Nothing

		''' <summary>
		''' <p><code>XMLStreamReader</code> to be used for source input.</p> </summary>
		Private xmlStreamReader As javax.xml.stream.XMLStreamReader = Nothing

		''' <summary>
		''' <p>System identifier of source input.</p> </summary>
		Private systemId As String = Nothing

		''' <summary>
		''' <p>Creates a new instance of a <code>StAXSource</code>
		''' by supplying an <seealso cref="XMLEventReader"/>.</p>
		''' 
		''' <p><code>XMLEventReader</code> must be a
		''' non-<code>null</code> reference.</p>
		''' 
		''' <p><code>XMLEventReader</code> must be in
		''' <seealso cref="XMLStreamConstants#START_DOCUMENT"/> or
		''' <seealso cref="XMLStreamConstants#START_ELEMENT"/> state.</p>
		''' </summary>
		''' <param name="xmlEventReader"> <code>XMLEventReader</code> used to create
		'''   this <code>StAXSource</code>.
		''' </param>
		''' <exception cref="XMLStreamException"> If <code>xmlEventReader</code> access
		'''   throws an <code>Exception</code>. </exception>
		''' <exception cref="IllegalArgumentException"> If <code>xmlEventReader</code> ==
		'''   <code>null</code>. </exception>
		''' <exception cref="IllegalStateException"> If <code>xmlEventReader</code>
		'''   is not in <code>XMLStreamConstants.START_DOCUMENT</code> or
		'''   <code>XMLStreamConstants.START_ELEMENT</code> state. </exception>
		Public Sub New(ByVal xmlEventReader As javax.xml.stream.XMLEventReader)

			If xmlEventReader Is Nothing Then Throw New System.ArgumentException("StAXSource(XMLEventReader) with XMLEventReader == null")

			' TODO: This is ugly ...
			' there is no way to know the current position(event) of
			' XMLEventReader.  peek() is the only way to know the next event.
			' The next event on the input stream should be
			' XMLStreamConstants.START_DOCUMENT or
			' XMLStreamConstants.START_ELEMENT.
			Dim [event] As javax.xml.stream.events.XMLEvent = xmlEventReader.peek()
			Dim eventType As Integer = [event].eventType
			If eventType <> javax.xml.stream.XMLStreamConstants.START_DOCUMENT AndAlso eventType <> javax.xml.stream.XMLStreamConstants.START_ELEMENT Then Throw New IllegalStateException("StAXSource(XMLEventReader) with XMLEventReader " & "not in XMLStreamConstants.START_DOCUMENT or " & "XMLStreamConstants.START_ELEMENT state")

			Me.xmlEventReader = xmlEventReader
			systemId = [event].location.systemId
		End Sub

		''' <summary>
		''' <p>Creates a new instance of a <code>StAXSource</code>
		''' by supplying an <seealso cref="XMLStreamReader"/>.</p>
		''' 
		''' <p><code>XMLStreamReader</code> must be a
		''' non-<code>null</code> reference.</p>
		''' 
		''' <p><code>XMLStreamReader</code> must be in
		''' <seealso cref="XMLStreamConstants#START_DOCUMENT"/> or
		''' <seealso cref="XMLStreamConstants#START_ELEMENT"/> state.</p>
		''' </summary>
		''' <param name="xmlStreamReader"> <code>XMLStreamReader</code> used to create
		'''   this <code>StAXSource</code>.
		''' </param>
		''' <exception cref="IllegalArgumentException"> If <code>xmlStreamReader</code> ==
		'''   <code>null</code>. </exception>
		''' <exception cref="IllegalStateException"> If <code>xmlStreamReader</code>
		'''   is not in <code>XMLStreamConstants.START_DOCUMENT</code> or
		'''   <code>XMLStreamConstants.START_ELEMENT</code> state. </exception>
		Public Sub New(ByVal xmlStreamReader As javax.xml.stream.XMLStreamReader)

			If xmlStreamReader Is Nothing Then Throw New System.ArgumentException("StAXSource(XMLStreamReader) with XMLStreamReader == null")

			Dim eventType As Integer = xmlStreamReader.eventType
			If eventType <> javax.xml.stream.XMLStreamConstants.START_DOCUMENT AndAlso eventType <> javax.xml.stream.XMLStreamConstants.START_ELEMENT Then Throw New IllegalStateException("StAXSource(XMLStreamReader) with XMLStreamReader" & "not in XMLStreamConstants.START_DOCUMENT or " & "XMLStreamConstants.START_ELEMENT state")

			Me.xmlStreamReader = xmlStreamReader
			systemId = xmlStreamReader.location.systemId
		End Sub

		''' <summary>
		''' <p>Get the <code>XMLEventReader</code> used by this
		''' <code>StAXSource</code>.</p>
		''' 
		''' <p><code>XMLEventReader</code> will be <code>null</code>.
		''' if this <code>StAXSource</code> was created with a
		''' <code>XMLStreamReader</code>.</p>
		''' </summary>
		''' <returns> <code>XMLEventReader</code> used by this
		'''   <code>StAXSource</code>. </returns>
		Public Overridable Property xMLEventReader As javax.xml.stream.XMLEventReader
			Get
    
				Return xmlEventReader
			End Get
		End Property

		''' <summary>
		''' <p>Get the <code>XMLStreamReader</code> used by this
		''' <code>StAXSource</code>.</p>
		''' 
		''' <p><code>XMLStreamReader</code> will be <code>null</code>
		''' if this <code>StAXSource</code> was created with a
		''' <code>XMLEventReader</code>.</p>
		''' </summary>
		''' <returns> <code>XMLStreamReader</code> used by this
		'''   <code>StAXSource</code>. </returns>
		Public Overridable Property xMLStreamReader As javax.xml.stream.XMLStreamReader
			Get
    
				Return xmlStreamReader
			End Get
		End Property

		''' <summary>
		''' <p>In the context of a <code>StAXSource</code>, it is not appropriate
		''' to explicitly set the system identifier.
		''' The <code>XMLStreamReader</code> or <code>XMLEventReader</code>
		''' used to construct this <code>StAXSource</code> determines the
		''' system identifier of the XML source.</p>
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
    
				Throw New System.NotSupportedException("StAXSource#setSystemId(systemId) cannot set the " & "system identifier for a StAXSource")
			End Set
			Get
    
				Return systemId
			End Get
		End Property

	End Class

End Namespace
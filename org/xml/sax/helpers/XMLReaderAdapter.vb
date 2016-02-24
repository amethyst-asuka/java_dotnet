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

' XMLReaderAdapter.java - adapt an SAX2 XMLReader to a SAX1 Parser
' http://www.saxproject.org
' Written by David Megginson
' NO WARRANTY!  This class is in the public domain.
' $Id: XMLReaderAdapter.java,v 1.3 2004/11/03 22:53:09 jsuttor Exp $

Namespace org.xml.sax.helpers





	''' <summary>
	''' Adapt a SAX2 XMLReader as a SAX1 Parser.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' See <a href='http://www.saxproject.org'>http://www.saxproject.org</a>
	''' for further information.
	''' </blockquote>
	''' 
	''' <p>This class wraps a SAX2 <seealso cref="org.xml.sax.XMLReader XMLReader"/>
	''' and makes it act as a SAX1 <seealso cref="org.xml.sax.Parser Parser"/>.  The XMLReader
	''' must support a true value for the
	''' http://xml.org/sax/features/namespace-prefixes property or parsing will fail
	''' with a <seealso cref="org.xml.sax.SAXException SAXException"/>; if the XMLReader
	''' supports a false value for the http://xml.org/sax/features/namespaces
	''' property, that will also be used to improve efficiency.</p>
	''' 
	''' @since SAX 2.0
	''' @author David Megginson </summary>
	''' <seealso cref= org.xml.sax.Parser </seealso>
	''' <seealso cref= org.xml.sax.XMLReader </seealso>
	Public Class XMLReaderAdapter
		Implements org.xml.sax.Parser, org.xml.sax.ContentHandler


		'//////////////////////////////////////////////////////////////////
		' Constructor.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Create a new adapter.
		''' 
		''' <p>Use the "org.xml.sax.driver" property to locate the SAX2
		''' driver to embed.</p>
		''' </summary>
		''' <exception cref="org.xml.sax.SAXException"> If the embedded driver
		'''            cannot be instantiated or if the
		'''            org.xml.sax.driver property is not specified. </exception>
		Public Sub New()
			setup(XMLReaderFactory.createXMLReader())
		End Sub


		''' <summary>
		''' Create a new adapter.
		''' 
		''' <p>Create a new adapter, wrapped around a SAX2 XMLReader.
		''' The adapter will make the XMLReader act like a SAX1
		''' Parser.</p>
		''' </summary>
		''' <param name="xmlReader"> The SAX2 XMLReader to wrap. </param>
		''' <exception cref="java.lang.NullPointerException"> If the argument is null. </exception>
		Public Sub New(ByVal xmlReader As org.xml.sax.XMLReader)
			setup(xmlReader)
		End Sub



		''' <summary>
		''' Internal setup.
		''' </summary>
		''' <param name="xmlReader"> The embedded XMLReader. </param>
		Private Sub setup(ByVal xmlReader As org.xml.sax.XMLReader)
			If xmlReader Is Nothing Then Throw New NullPointerException("XMLReader must not be null")
			Me.xmlReader = xmlReader
			qAtts = New AttributesAdapter(Me)
		End Sub



		'//////////////////////////////////////////////////////////////////
		' Implementation of org.xml.sax.Parser.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Set the locale for error reporting.
		''' 
		''' <p>This is not supported in SAX2, and will always throw
		''' an exception.</p>
		''' </summary>
		''' <param name="locale"> the locale for error reporting. </param>
		''' <seealso cref= org.xml.sax.Parser#setLocale </seealso>
		''' <exception cref="org.xml.sax.SAXException"> Thrown unless overridden. </exception>
		Public Overridable Property locale As java.util.Locale
			Set(ByVal locale As java.util.Locale)
				Throw New org.xml.sax.SAXNotSupportedException("setLocale not supported")
			End Set
		End Property


		''' <summary>
		''' Register the entity resolver.
		''' </summary>
		''' <param name="resolver"> The new resolver. </param>
		''' <seealso cref= org.xml.sax.Parser#setEntityResolver </seealso>
		Public Overridable Property entityResolver As org.xml.sax.EntityResolver
			Set(ByVal resolver As org.xml.sax.EntityResolver)
				xmlReader.entityResolver = resolver
			End Set
		End Property


		''' <summary>
		''' Register the DTD event handler.
		''' </summary>
		''' <param name="handler"> The new DTD event handler. </param>
		''' <seealso cref= org.xml.sax.Parser#setDTDHandler </seealso>
		Public Overridable Property dTDHandler As org.xml.sax.DTDHandler
			Set(ByVal handler As org.xml.sax.DTDHandler)
				xmlReader.dTDHandler = handler
			End Set
		End Property


		''' <summary>
		''' Register the SAX1 document event handler.
		''' 
		''' <p>Note that the SAX1 document handler has no Namespace
		''' support.</p>
		''' </summary>
		''' <param name="handler"> The new SAX1 document event handler. </param>
		''' <seealso cref= org.xml.sax.Parser#setDocumentHandler </seealso>
		Public Overridable Property documentHandler As org.xml.sax.DocumentHandler
			Set(ByVal handler As org.xml.sax.DocumentHandler)
				documentHandler = handler
			End Set
		End Property


		''' <summary>
		''' Register the error event handler.
		''' </summary>
		''' <param name="handler"> The new error event handler. </param>
		''' <seealso cref= org.xml.sax.Parser#setErrorHandler </seealso>
		Public Overridable Property errorHandler As org.xml.sax.ErrorHandler
			Set(ByVal handler As org.xml.sax.ErrorHandler)
				xmlReader.errorHandler = handler
			End Set
		End Property


		''' <summary>
		''' Parse the document.
		''' 
		''' <p>This method will throw an exception if the embedded
		''' XMLReader does not support the
		''' http://xml.org/sax/features/namespace-prefixes property.</p>
		''' </summary>
		''' <param name="systemId"> The absolute URL of the document. </param>
		''' <exception cref="java.io.IOException"> If there is a problem reading
		'''            the raw content of the document. </exception>
		''' <exception cref="org.xml.sax.SAXException"> If there is a problem
		'''            processing the document. </exception>
		''' <seealso cref= #parse(org.xml.sax.InputSource) </seealso>
		''' <seealso cref= org.xml.sax.Parser#parse(java.lang.String) </seealso>
		Public Overridable Sub parse(ByVal systemId As String)
			parse(New org.xml.sax.InputSource(systemId))
		End Sub


		''' <summary>
		''' Parse the document.
		''' 
		''' <p>This method will throw an exception if the embedded
		''' XMLReader does not support the
		''' http://xml.org/sax/features/namespace-prefixes property.</p>
		''' </summary>
		''' <param name="input"> An input source for the document. </param>
		''' <exception cref="java.io.IOException"> If there is a problem reading
		'''            the raw content of the document. </exception>
		''' <exception cref="org.xml.sax.SAXException"> If there is a problem
		'''            processing the document. </exception>
		''' <seealso cref= #parse(java.lang.String) </seealso>
		''' <seealso cref= org.xml.sax.Parser#parse(org.xml.sax.InputSource) </seealso>
		Public Overridable Sub parse(ByVal input As org.xml.sax.InputSource)
			setupXMLReader()
			xmlReader.parse(input)
		End Sub


		''' <summary>
		''' Set up the XML reader.
		''' </summary>
		Private Sub setupXMLReader()
			xmlReader.featureure("http://xml.org/sax/features/namespace-prefixes", True)
			Try
				xmlReader.featureure("http://xml.org/sax/features/namespaces", False)
			Catch e As org.xml.sax.SAXException
				' NO OP: it's just extra information, and we can ignore it
			End Try
			xmlReader.contentHandler = Me
		End Sub



		'//////////////////////////////////////////////////////////////////
		' Implementation of org.xml.sax.ContentHandler.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Set a document locator.
		''' </summary>
		''' <param name="locator"> The document locator. </param>
		''' <seealso cref= org.xml.sax.ContentHandler#setDocumentLocator </seealso>
		Public Overridable Property documentLocator As org.xml.sax.Locator
			Set(ByVal locator As org.xml.sax.Locator)
				If documentHandler IsNot Nothing Then documentHandler.documentLocator = locator
			End Set
		End Property


		''' <summary>
		''' Start document event.
		''' </summary>
		''' <exception cref="org.xml.sax.SAXException"> The client may raise a
		'''            processing exception. </exception>
		''' <seealso cref= org.xml.sax.ContentHandler#startDocument </seealso>
		Public Overridable Sub startDocument()
			If documentHandler IsNot Nothing Then documentHandler.startDocument()
		End Sub


		''' <summary>
		''' End document event.
		''' </summary>
		''' <exception cref="org.xml.sax.SAXException"> The client may raise a
		'''            processing exception. </exception>
		''' <seealso cref= org.xml.sax.ContentHandler#endDocument </seealso>
		Public Overridable Sub endDocument()
			If documentHandler IsNot Nothing Then documentHandler.endDocument()
		End Sub


		''' <summary>
		''' Adapt a SAX2 start prefix mapping event.
		''' </summary>
		''' <param name="prefix"> The prefix being mapped. </param>
		''' <param name="uri"> The Namespace URI being mapped to. </param>
		''' <seealso cref= org.xml.sax.ContentHandler#startPrefixMapping </seealso>
		Public Overridable Sub startPrefixMapping(ByVal prefix As String, ByVal uri As String)
		End Sub


		''' <summary>
		''' Adapt a SAX2 end prefix mapping event.
		''' </summary>
		''' <param name="prefix"> The prefix being mapped. </param>
		''' <seealso cref= org.xml.sax.ContentHandler#endPrefixMapping </seealso>
		Public Overridable Sub endPrefixMapping(ByVal prefix As String)
		End Sub


		''' <summary>
		''' Adapt a SAX2 start element event.
		''' </summary>
		''' <param name="uri"> The Namespace URI. </param>
		''' <param name="localName"> The Namespace local name. </param>
		''' <param name="qName"> The qualified (prefixed) name. </param>
		''' <param name="atts"> The SAX2 attributes. </param>
		''' <exception cref="org.xml.sax.SAXException"> The client may raise a
		'''            processing exception. </exception>
		''' <seealso cref= org.xml.sax.ContentHandler#endDocument </seealso>
		Public Overridable Sub startElement(ByVal uri As String, ByVal localName As String, ByVal qName As String, ByVal atts As org.xml.sax.Attributes)
			If documentHandler IsNot Nothing Then
				qAtts.attributes = atts
				documentHandler.startElement(qName, qAtts)
			End If
		End Sub


		''' <summary>
		''' Adapt a SAX2 end element event.
		''' </summary>
		''' <param name="uri"> The Namespace URI. </param>
		''' <param name="localName"> The Namespace local name. </param>
		''' <param name="qName"> The qualified (prefixed) name. </param>
		''' <exception cref="org.xml.sax.SAXException"> The client may raise a
		'''            processing exception. </exception>
		''' <seealso cref= org.xml.sax.ContentHandler#endElement </seealso>
		Public Overridable Sub endElement(ByVal uri As String, ByVal localName As String, ByVal qName As String)
			If documentHandler IsNot Nothing Then documentHandler.endElement(qName)
		End Sub


		''' <summary>
		''' Adapt a SAX2 characters event.
		''' </summary>
		''' <param name="ch"> An array of characters. </param>
		''' <param name="start"> The starting position in the array. </param>
		''' <param name="length"> The number of characters to use. </param>
		''' <exception cref="org.xml.sax.SAXException"> The client may raise a
		'''            processing exception. </exception>
		''' <seealso cref= org.xml.sax.ContentHandler#characters </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public void characters(char ch() , int start, int length) throws org.xml.sax.SAXException
			If documentHandler IsNot Nothing Then documentHandler.characters(ch, start, length)


		''' <summary>
		''' Adapt a SAX2 ignorable whitespace event.
		''' </summary>
		''' <param name="ch"> An array of characters. </param>
		''' <param name="start"> The starting position in the array. </param>
		''' <param name="length"> The number of characters to use. </param>
		''' <exception cref="org.xml.sax.SAXException"> The client may raise a
		'''            processing exception. </exception>
		''' <seealso cref= org.xml.sax.ContentHandler#ignorableWhitespace </seealso>
		public void ignorableWhitespace(Char ch() , Integer start, Integer length) throws org.xml.sax.SAXException
			If documentHandler IsNot Nothing Then documentHandler.ignorableWhitespace(ch, start, length)


		''' <summary>
		''' Adapt a SAX2 processing instruction event.
		''' </summary>
		''' <param name="target"> The processing instruction target. </param>
		''' <param name="data"> The remainder of the processing instruction </param>
		''' <exception cref="org.xml.sax.SAXException"> The client may raise a
		'''            processing exception. </exception>
		''' <seealso cref= org.xml.sax.ContentHandler#processingInstruction </seealso>
		public void processingInstruction(String target, String data) throws org.xml.sax.SAXException
			If documentHandler IsNot Nothing Then documentHandler.processingInstruction(target, data)


		''' <summary>
		''' Adapt a SAX2 skipped entity event.
		''' </summary>
		''' <param name="name"> The name of the skipped entity. </param>
		''' <seealso cref= org.xml.sax.ContentHandler#skippedEntity </seealso>
		''' <exception cref="org.xml.sax.SAXException"> Throwable by subclasses. </exception>
		public void skippedEntity(String name) throws org.xml.sax.SAXException



		'//////////////////////////////////////////////////////////////////
		' Internal state.
		'//////////////////////////////////////////////////////////////////

		Dim xmlReader As org.xml.sax.XMLReader
		Dim documentHandler_Renamed As org.xml.sax.DocumentHandler
		Dim qAtts As AttributesAdapter



		'//////////////////////////////////////////////////////////////////
		' Internal class.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Internal class to wrap a SAX2 Attributes object for SAX1.
		''' </summary>
		final class AttributesAdapter implements org.xml.sax.AttributeList
			AttributesAdapter()


			''' <summary>
			''' Set the embedded Attributes object.
			''' </summary>
			''' <param name="The"> embedded SAX2 Attributes. </param>
			void attributestes(org.xml.sax.Attributes attributes)
				Me.attributes = attributes


			''' <summary>
			''' Return the number of attributes.
			''' </summary>
			''' <returns> The length of the attribute list. </returns>
			''' <seealso cref= org.xml.sax.AttributeList#getLength </seealso>
			public Integer length
				Return attributes.length


			''' <summary>
			''' Return the qualified (prefixed) name of an attribute by position.
			''' </summary>
			''' <returns> The qualified name. </returns>
			''' <seealso cref= org.xml.sax.AttributeList#getName </seealso>
			public String getName(Integer i)
				Return attributes.getQName(i)


			''' <summary>
			''' Return the type of an attribute by position.
			''' </summary>
			''' <returns> The type. </returns>
			''' <seealso cref= org.xml.sax.AttributeList#getType(int) </seealso>
			public String getType(Integer i)
				Return attributes.getType(i)


			''' <summary>
			''' Return the value of an attribute by position.
			''' </summary>
			''' <returns> The value. </returns>
			''' <seealso cref= org.xml.sax.AttributeList#getValue(int) </seealso>
			public String getValue(Integer i)
				Return attributes.getValue(i)


			''' <summary>
			''' Return the type of an attribute by qualified (prefixed) name.
			''' </summary>
			''' <returns> The type. </returns>
			''' <seealso cref= org.xml.sax.AttributeList#getType(java.lang.String) </seealso>
			public String getType(String qName)
				Return attributes.getType(qName)


			''' <summary>
			''' Return the value of an attribute by qualified (prefixed) name.
			''' </summary>
			''' <returns> The value. </returns>
			''' <seealso cref= org.xml.sax.AttributeList#getValue(java.lang.String) </seealso>
			public String getValue(String qName)
				Return attributes.getValue(qName)

			private org.xml.sax.Attributes attributes

	End Class

	' end of XMLReaderAdapter.java

End Namespace
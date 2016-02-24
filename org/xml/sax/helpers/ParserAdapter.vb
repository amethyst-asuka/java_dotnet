Imports System.Collections

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

' ParserAdapter.java - adapt a SAX1 Parser to a SAX2 XMLReader.
' http://www.saxproject.org
' Written by David Megginson
' NO WARRANTY!  This class is in the public domain.
' $Id: ParserAdapter.java,v 1.3 2004/11/03 22:53:09 jsuttor Exp $

Namespace org.xml.sax.helpers





	''' <summary>
	''' Adapt a SAX1 Parser as a SAX2 XMLReader.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' See <a href='http://www.saxproject.org'>http://www.saxproject.org</a>
	''' for further information.
	''' </blockquote>
	''' 
	''' <p>This class wraps a SAX1 <seealso cref="org.xml.sax.Parser Parser"/>
	''' and makes it act as a SAX2 <seealso cref="org.xml.sax.XMLReader XMLReader"/>,
	''' with feature, property, and Namespace support.  Note
	''' that it is not possible to report {@link org.xml.sax.ContentHandler#skippedEntity
	''' skippedEntity} events, since SAX1 does not make that information available.</p>
	''' 
	''' <p>This adapter does not test for duplicate Namespace-qualified
	''' attribute names.</p>
	''' 
	''' @since SAX 2.0
	''' @author David Megginson
	''' @version 2.0.1 (sax2r2) </summary>
	''' <seealso cref= org.xml.sax.helpers.XMLReaderAdapter </seealso>
	''' <seealso cref= org.xml.sax.XMLReader </seealso>
	''' <seealso cref= org.xml.sax.Parser </seealso>
	Public Class ParserAdapter
		Implements org.xml.sax.XMLReader, org.xml.sax.DocumentHandler

		Private Shared ss As New SecuritySupport

		'//////////////////////////////////////////////////////////////////
		' Constructors.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Construct a new parser adapter.
		''' 
		''' <p>Use the "org.xml.sax.parser" property to locate the
		''' embedded SAX1 driver.</p>
		''' </summary>
		''' <exception cref="SAXException"> If the embedded driver
		'''            cannot be instantiated or if the
		'''            org.xml.sax.parser property is not specified. </exception>
		Public Sub New()
			MyBase.New()

			Dim driver As String = ss.getSystemProperty("org.xml.sax.parser")

			Try
				setup(ParserFactory.makeParser())
			Catch e1 As ClassNotFoundException
				Throw New org.xml.sax.SAXException("Cannot find SAX1 driver class " & driver, e1)
			Catch e2 As IllegalAccessException
				Throw New org.xml.sax.SAXException("SAX1 driver class " & driver & " found but cannot be loaded", e2)
			Catch e3 As InstantiationException
				Throw New org.xml.sax.SAXException("SAX1 driver class " & driver & " loaded but cannot be instantiated", e3)
			Catch e4 As ClassCastException
				Throw New org.xml.sax.SAXException("SAX1 driver class " & driver & " does not implement org.xml.sax.Parser")
			Catch e5 As NullPointerException
				Throw New org.xml.sax.SAXException("System property org.xml.sax.parser not specified")
			End Try
		End Sub


		''' <summary>
		''' Construct a new parser adapter.
		''' 
		''' <p>Note that the embedded parser cannot be changed once the
		''' adapter is created; to embed a different parser, allocate
		''' a new ParserAdapter.</p>
		''' </summary>
		''' <param name="parser"> The SAX1 parser to embed. </param>
		''' <exception cref="java.lang.NullPointerException"> If the parser parameter
		'''            is null. </exception>
		Public Sub New(ByVal parser As org.xml.sax.Parser)
			MyBase.New()
			setup(parser)
		End Sub


		''' <summary>
		''' Internal setup method.
		''' </summary>
		''' <param name="parser"> The embedded parser. </param>
		''' <exception cref="java.lang.NullPointerException"> If the parser parameter
		'''            is null. </exception>
		Private Sub setup(ByVal parser As org.xml.sax.Parser)
			If parser Is Nothing Then Throw New NullPointerException("Parser argument must not be null")
			Me.parser = parser
			atts = New AttributesImpl
			nsSupport = New NamespaceSupport
			attAdapter = New AttributeListAdapter(Me)
		End Sub



		'//////////////////////////////////////////////////////////////////
		' Implementation of org.xml.sax.XMLReader.
		'//////////////////////////////////////////////////////////////////


		'
		' Internal constants for the sake of convenience.
		'
		Private Const FEATURES As String = "http://xml.org/sax/features/"
		Private Shared ReadOnly NAMESPACES As String = FEATURES & "namespaces"
		Private Shared ReadOnly NAMESPACE_PREFIXES As String = FEATURES & "namespace-prefixes"
		Private Shared ReadOnly XMLNS_URIs As String = FEATURES & "xmlns-uris"


		''' <summary>
		''' Set a feature flag for the parser.
		''' 
		''' <p>The only features recognized are namespaces and
		''' namespace-prefixes.</p>
		''' </summary>
		''' <param name="name"> The feature name, as a complete URI. </param>
		''' <param name="value"> The requested feature value. </param>
		''' <exception cref="SAXNotRecognizedException"> If the feature
		'''            can't be assigned or retrieved. </exception>
		''' <exception cref="SAXNotSupportedException"> If the feature
		'''            can't be assigned that value. </exception>
		''' <seealso cref= org.xml.sax.XMLReader#setFeature </seealso>
		Public Overridable Sub setFeature(ByVal name As String, ByVal value As Boolean)
			If name.Equals(NAMESPACES) Then
				checkNotParsing("feature", name)
				namespaces = value
				If (Not namespaces) AndAlso (Not prefixes) Then prefixes = True
			ElseIf name.Equals(NAMESPACE_PREFIXES) Then
				checkNotParsing("feature", name)
				prefixes = value
				If (Not prefixes) AndAlso (Not namespaces) Then namespaces = True
			ElseIf name.Equals(XMLNS_URIs) Then
				checkNotParsing("feature", name)
				uris = value
			Else
				Throw New org.xml.sax.SAXNotRecognizedException("Feature: " & name)
			End If
		End Sub


		''' <summary>
		''' Check a parser feature flag.
		''' 
		''' <p>The only features recognized are namespaces and
		''' namespace-prefixes.</p>
		''' </summary>
		''' <param name="name"> The feature name, as a complete URI. </param>
		''' <returns> The current feature value. </returns>
		''' <exception cref="SAXNotRecognizedException"> If the feature
		'''            value can't be assigned or retrieved. </exception>
		''' <exception cref="SAXNotSupportedException"> If the
		'''            feature is not currently readable. </exception>
		''' <seealso cref= org.xml.sax.XMLReader#setFeature </seealso>
		Public Overridable Function getFeature(ByVal name As String) As Boolean
			If name.Equals(NAMESPACES) Then
				Return namespaces
			ElseIf name.Equals(NAMESPACE_PREFIXES) Then
				Return prefixes
			ElseIf name.Equals(XMLNS_URIs) Then
				Return uris
			Else
				Throw New org.xml.sax.SAXNotRecognizedException("Feature: " & name)
			End If
		End Function


		''' <summary>
		''' Set a parser property.
		''' 
		''' <p>No properties are currently recognized.</p>
		''' </summary>
		''' <param name="name"> The property name. </param>
		''' <param name="value"> The property value. </param>
		''' <exception cref="SAXNotRecognizedException"> If the property
		'''            value can't be assigned or retrieved. </exception>
		''' <exception cref="SAXNotSupportedException"> If the property
		'''            can't be assigned that value. </exception>
		''' <seealso cref= org.xml.sax.XMLReader#setProperty </seealso>
		Public Overridable Sub setProperty(ByVal name As String, ByVal value As Object)
			Throw New org.xml.sax.SAXNotRecognizedException("Property: " & name)
		End Sub


		''' <summary>
		''' Get a parser property.
		''' 
		''' <p>No properties are currently recognized.</p>
		''' </summary>
		''' <param name="name"> The property name. </param>
		''' <returns> The property value. </returns>
		''' <exception cref="SAXNotRecognizedException"> If the property
		'''            value can't be assigned or retrieved. </exception>
		''' <exception cref="SAXNotSupportedException"> If the property
		'''            value is not currently readable. </exception>
		''' <seealso cref= org.xml.sax.XMLReader#getProperty </seealso>
		Public Overridable Function getProperty(ByVal name As String) As Object
			Throw New org.xml.sax.SAXNotRecognizedException("Property: " & name)
		End Function


		''' <summary>
		''' Set the entity resolver.
		''' </summary>
		''' <param name="resolver"> The new entity resolver. </param>
		''' <seealso cref= org.xml.sax.XMLReader#setEntityResolver </seealso>
		Public Overridable Property entityResolver As org.xml.sax.EntityResolver
			Set(ByVal resolver As org.xml.sax.EntityResolver)
				entityResolver = resolver
			End Set
			Get
				Return entityResolver
			End Get
		End Property




		''' <summary>
		''' Set the DTD handler.
		''' </summary>
		''' <param name="handler"> the new DTD handler </param>
		''' <seealso cref= org.xml.sax.XMLReader#setEntityResolver </seealso>
		Public Overridable Property dTDHandler As org.xml.sax.DTDHandler
			Set(ByVal handler As org.xml.sax.DTDHandler)
				dtdHandler = handler
			End Set
			Get
				Return dtdHandler
			End Get
		End Property




		''' <summary>
		''' Set the content handler.
		''' </summary>
		''' <param name="handler"> the new content handler </param>
		''' <seealso cref= org.xml.sax.XMLReader#setEntityResolver </seealso>
		Public Overridable Property contentHandler As org.xml.sax.ContentHandler
			Set(ByVal handler As org.xml.sax.ContentHandler)
				contentHandler = handler
			End Set
			Get
				Return contentHandler
			End Get
		End Property




		''' <summary>
		''' Set the error handler.
		''' </summary>
		''' <param name="handler"> The new error handler. </param>
		''' <seealso cref= org.xml.sax.XMLReader#setEntityResolver </seealso>
		Public Overridable Property errorHandler As org.xml.sax.ErrorHandler
			Set(ByVal handler As org.xml.sax.ErrorHandler)
				errorHandler = handler
			End Set
			Get
				Return errorHandler
			End Get
		End Property




		''' <summary>
		''' Parse an XML document.
		''' </summary>
		''' <param name="systemId"> The absolute URL of the document. </param>
		''' <exception cref="java.io.IOException"> If there is a problem reading
		'''            the raw content of the document. </exception>
		''' <exception cref="SAXException"> If there is a problem
		'''            processing the document. </exception>
		''' <seealso cref= #parse(org.xml.sax.InputSource) </seealso>
		''' <seealso cref= org.xml.sax.Parser#parse(java.lang.String) </seealso>
		Public Overridable Sub parse(ByVal systemId As String)
			parse(New org.xml.sax.InputSource(systemId))
		End Sub


		''' <summary>
		''' Parse an XML document.
		''' </summary>
		''' <param name="input"> An input source for the document. </param>
		''' <exception cref="java.io.IOException"> If there is a problem reading
		'''            the raw content of the document. </exception>
		''' <exception cref="SAXException"> If there is a problem
		'''            processing the document. </exception>
		''' <seealso cref= #parse(java.lang.String) </seealso>
		''' <seealso cref= org.xml.sax.Parser#parse(org.xml.sax.InputSource) </seealso>
		Public Overridable Sub parse(ByVal input As org.xml.sax.InputSource)
			If parsing Then Throw New org.xml.sax.SAXException("Parser is already in use")
			setupParser()
			parsing = True
			Try
				parser.parse(input)
			Finally
				parsing = False
			End Try
			parsing = False
		End Sub



		'//////////////////////////////////////////////////////////////////
		' Implementation of org.xml.sax.DocumentHandler.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Adapter implementation method; do not call.
		''' Adapt a SAX1 document locator event.
		''' </summary>
		''' <param name="locator"> A document locator. </param>
		''' <seealso cref= org.xml.sax.ContentHandler#setDocumentLocator </seealso>
		Public Overridable Property documentLocator As org.xml.sax.Locator
			Set(ByVal locator As org.xml.sax.Locator)
				Me.locator = locator
				If contentHandler IsNot Nothing Then contentHandler.documentLocator = locator
			End Set
		End Property


		''' <summary>
		''' Adapter implementation method; do not call.
		''' Adapt a SAX1 start document event.
		''' </summary>
		''' <exception cref="SAXException"> The client may raise a
		'''            processing exception. </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler#startDocument </seealso>
		Public Overridable Sub startDocument()
			If contentHandler IsNot Nothing Then contentHandler.startDocument()
		End Sub


		''' <summary>
		''' Adapter implementation method; do not call.
		''' Adapt a SAX1 end document event.
		''' </summary>
		''' <exception cref="SAXException"> The client may raise a
		'''            processing exception. </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler#endDocument </seealso>
		Public Overridable Sub endDocument()
			If contentHandler IsNot Nothing Then contentHandler.endDocument()
		End Sub


		''' <summary>
		''' Adapter implementation method; do not call.
		''' Adapt a SAX1 startElement event.
		''' 
		''' <p>If necessary, perform Namespace processing.</p>
		''' </summary>
		''' <param name="qName"> The qualified (prefixed) name. </param>
		''' <param name="qAtts"> The XML attribute list (with qnames). </param>
		''' <exception cref="SAXException"> The client may raise a
		'''            processing exception. </exception>
		Public Overridable Sub startElement(ByVal qName As String, ByVal qAtts As org.xml.sax.AttributeList)
									' These are exceptions from the
									' first pass; they should be
									' ignored if there's a second pass,
									' but reported otherwise.
			Dim exceptions As ArrayList = Nothing

									' If we're not doing Namespace
									' processing, dispatch this quickly.
			If Not namespaces Then
				If contentHandler IsNot Nothing Then
					attAdapter.attributeList = qAtts
					contentHandler.startElement("", "", qName.intern(), attAdapter)
				End If
				Return
			End If


									' OK, we're doing Namespace processing.
			nsSupport.pushContext()
			Dim length As Integer = qAtts.length

									' First pass:  handle NS decls
			For i As Integer = 0 To length - 1
				Dim attQName As String = qAtts.getName(i)

				If Not attQName.StartsWith("xmlns") Then Continue For
									' Could be a declaration...
				Dim prefix As String
				Dim n As Integer = attQName.IndexOf(":"c)

									' xmlns=...
				If n = -1 AndAlso attQName.Length = 5 Then
					prefix = ""
				ElseIf n <> 5 Then
					' XML namespaces spec doesn't discuss "xmlnsf:oo"
					' (and similarly named) attributes ... at most, warn
					Continue For ' xmlns:foo=...
				Else
					prefix = attQName.Substring(n+1)
				End If

				Dim value As String = qAtts.getValue(i)
				If Not nsSupport.declarePrefix(prefix, value) Then
					reportError("Illegal Namespace prefix: " & prefix)
					Continue For
				End If
				If contentHandler IsNot Nothing Then contentHandler.startPrefixMapping(prefix, value)
			Next i

									' Second pass: copy all relevant
									' attributes into the SAX2 AttributeList
									' using updated prefix bindings
			atts.clear()
			For i As Integer = 0 To length - 1
				Dim attQName As String = qAtts.getName(i)
				Dim type As String = qAtts.getType(i)
				Dim value As String = qAtts.getValue(i)

									' Declaration?
				If attQName.StartsWith("xmlns") Then
					Dim prefix As String
					Dim n As Integer = attQName.IndexOf(":"c)

					If n = -1 AndAlso attQName.Length = 5 Then
						prefix = ""
					ElseIf n <> 5 Then
						' XML namespaces spec doesn't discuss "xmlnsf:oo"
						' (and similarly named) attributes ... ignore
						prefix = Nothing
					Else
						prefix = attQName.Substring(6)
					End If
									' Yes, decl:  report or prune
					If prefix IsNot Nothing Then
						If prefixes Then
							If uris Then
								' note funky case:  localname can be null
								' when declaring the default prefix, and
								' yet the uri isn't null.
								atts.addAttribute(nsSupport.XMLNS, prefix, attQName.intern(), type, value)
							Else
								atts.addAttribute("", "", attQName.intern(), type, value)
							End If
						End If
						Continue For
					End If
				End If

									' Not a declaration -- report
				Try
					Dim attName As String() = processName(attQName, True, True)
					atts.addAttribute(attName(0), attName(1), attName(2), type, value)
				Catch e As org.xml.sax.SAXException
					If exceptions Is Nothing Then exceptions = New ArrayList
					exceptions.Add(e)
					atts.addAttribute("", attQName, attQName, type, value)
				End Try
			Next i

			' now handle the deferred exception reports
			If exceptions IsNot Nothing AndAlso errorHandler IsNot Nothing Then
				For i As Integer = 0 To exceptions.Count - 1
					errorHandler.error(CType(exceptions(i), org.xml.sax.SAXParseException))
				Next i
			End If

									' OK, finally report the event.
			If contentHandler IsNot Nothing Then
				Dim name As String() = processName(qName, False, False)
				contentHandler.startElement(name(0), name(1), name(2), atts)
			End If
		End Sub


		''' <summary>
		''' Adapter implementation method; do not call.
		''' Adapt a SAX1 end element event.
		''' </summary>
		''' <param name="qName"> The qualified (prefixed) name. </param>
		''' <exception cref="SAXException"> The client may raise a
		'''            processing exception. </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler#endElement </seealso>
		Public Overridable Sub endElement(ByVal qName As String)
									' If we're not doing Namespace
									' processing, dispatch this quickly.
			If Not namespaces Then
				If contentHandler IsNot Nothing Then contentHandler.endElement("", "", qName.intern())
				Return
			End If

									' Split the name.
			Dim names As String() = processName(qName, False, False)
			If contentHandler IsNot Nothing Then
				contentHandler.endElement(names(0), names(1), names(2))
				Dim prefixes As System.Collections.IEnumerator = nsSupport.declaredPrefixes
				Do While prefixes.hasMoreElements()
					Dim prefix As String = CStr(prefixes.nextElement())
					contentHandler.endPrefixMapping(prefix)
				Loop
			End If
			nsSupport.popContext()
		End Sub


		''' <summary>
		''' Adapter implementation method; do not call.
		''' Adapt a SAX1 characters event.
		''' </summary>
		''' <param name="ch"> An array of characters. </param>
		''' <param name="start"> The starting position in the array. </param>
		''' <param name="length"> The number of characters to use. </param>
		''' <exception cref="SAXException"> The client may raise a
		'''            processing exception. </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler#characters </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public void characters(char ch() , int start, int length) throws org.xml.sax.SAXException
			If contentHandler IsNot Nothing Then contentHandler.characters(ch, start, length)


		''' <summary>
		''' Adapter implementation method; do not call.
		''' Adapt a SAX1 ignorable whitespace event.
		''' </summary>
		''' <param name="ch"> An array of characters. </param>
		''' <param name="start"> The starting position in the array. </param>
		''' <param name="length"> The number of characters to use. </param>
		''' <exception cref="SAXException"> The client may raise a
		'''            processing exception. </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler#ignorableWhitespace </seealso>
		public void ignorableWhitespace(Char ch() , Integer start, Integer length) throws org.xml.sax.SAXException
			If contentHandler IsNot Nothing Then contentHandler.ignorableWhitespace(ch, start, length)


		''' <summary>
		''' Adapter implementation method; do not call.
		''' Adapt a SAX1 processing instruction event.
		''' </summary>
		''' <param name="target"> The processing instruction target. </param>
		''' <param name="data"> The remainder of the processing instruction </param>
		''' <exception cref="SAXException"> The client may raise a
		'''            processing exception. </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler#processingInstruction </seealso>
		public void processingInstruction(String target, String data) throws org.xml.sax.SAXException
			If contentHandler IsNot Nothing Then contentHandler.processingInstruction(target, data)



		'//////////////////////////////////////////////////////////////////
		' Internal utility methods.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Initialize the parser before each run.
		''' </summary>
		private void setupParser()
			' catch an illegal "nonsense" state.
			If (Not prefixes) AndAlso (Not namespaces) Then Throw New IllegalStateException

			nsSupport.reset()
			If uris Then nsSupport.namespaceDeclUris = True

			If entityResolver IsNot Nothing Then parser.entityResolver = entityResolver
			If dtdHandler IsNot Nothing Then parser.dTDHandler = dtdHandler
			If errorHandler IsNot Nothing Then parser.errorHandler = errorHandler
			parser.documentHandler = Me
			locator = Nothing


		''' <summary>
		''' Process a qualified (prefixed) name.
		''' 
		''' <p>If the name has an undeclared prefix, use only the qname
		''' and make an ErrorHandler.error callback in case the app is
		''' interested.</p>
		''' </summary>
		''' <param name="qName"> The qualified (prefixed) name. </param>
		''' <param name="isAttribute"> true if this is an attribute name. </param>
		''' <returns> The name split into three parts. </returns>
		''' <exception cref="SAXException"> The client may throw
		'''            an exception if there is an error callback. </exception>
		private String () processName(String qName, Boolean isAttribute, Boolean useException) throws org.xml.sax.SAXException
			Dim parts As String() = nsSupport.processName(qName, nameParts, isAttribute)
			If parts Is Nothing Then
				If useException Then Throw makeException("Undeclared prefix: " & qName)
				reportError("Undeclared prefix: " & qName)
				parts = New String(2){}
					parts(1) = ""
					parts(0) = parts(1)
				parts(2) = qName.intern()
			End If
			Return parts


		''' <summary>
		''' Report a non-fatal error.
		''' </summary>
		''' <param name="message"> The error message. </param>
		''' <exception cref="SAXException"> The client may throw
		'''            an exception. </exception>
		void reportError(String message) throws org.xml.sax.SAXException
			If errorHandler IsNot Nothing Then errorHandler.error(makeException(message))


		''' <summary>
		''' Construct an exception for the current context.
		''' </summary>
		''' <param name="message"> The error message. </param>
		private org.xml.sax.SAXParseException makeException(String message)
			If locator IsNot Nothing Then
				Return New org.xml.sax.SAXParseException(message, locator)
			Else
				Return New org.xml.sax.SAXParseException(message, Nothing, Nothing, -1, -1)
			End If


		''' <summary>
		''' Throw an exception if we are parsing.
		''' 
		''' <p>Use this method to detect illegal feature or
		''' property changes.</p>
		''' </summary>
		''' <param name="type"> The type of thing (feature or property). </param>
		''' <param name="name"> The feature or property name. </param>
		''' <exception cref="SAXNotSupportedException"> If a
		'''            document is currently being parsed. </exception>
		private void checkNotParsing(String type, String name) throws org.xml.sax.SAXNotSupportedException
			If parsing Then Throw New org.xml.sax.SAXNotSupportedException("Cannot change " & type + AscW(" "c) + name & " while parsing")



		'//////////////////////////////////////////////////////////////////
		' Internal state.
		'//////////////////////////////////////////////////////////////////

		private NamespaceSupport nsSupport
		private AttributeListAdapter attAdapter

		private Boolean parsing = False
		private String nameParts() = New String(2){}

		private org.xml.sax.Parser parser = Nothing

		private AttributesImpl atts = Nothing

									' Features
		private Boolean namespaces = True
		private Boolean prefixes = False
		private Boolean uris = False

									' Properties

									' Handlers
		Dim locator As org.xml.sax.Locator

		Dim entityResolver_Renamed As org.xml.sax.EntityResolver = Nothing
		Dim dtdHandler_Renamed As org.xml.sax.DTDHandler = Nothing
		Dim contentHandler_Renamed As org.xml.sax.ContentHandler = Nothing
		Dim errorHandler_Renamed As org.xml.sax.ErrorHandler = Nothing



		'//////////////////////////////////////////////////////////////////
		' Inner class to wrap an AttributeList when not doing NS proc.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Adapt a SAX1 AttributeList as a SAX2 Attributes object.
		''' 
		''' <p>This class is in the Public Domain, and comes with NO
		''' WARRANTY of any kind.</p>
		''' 
		''' <p>This wrapper class is used only when Namespace support
		''' is disabled -- it provides pretty much a direct mapping
		''' from SAX1 to SAX2, except that names and types are
		''' interned whenever requested.</p>
		''' </summary>
		final class AttributeListAdapter implements org.xml.sax.Attributes

			''' <summary>
			''' Construct a new adapter.
			''' </summary>
			AttributeListAdapter()


			''' <summary>
			''' Set the embedded AttributeList.
			''' 
			''' <p>This method must be invoked before any of the others
			''' can be used.</p>
			''' </summary>
			''' <param name="The"> SAX1 attribute list (with qnames). </param>
			void attributeListist(org.xml.sax.AttributeList qAtts)
				Me.qAtts = qAtts


			''' <summary>
			''' Return the length of the attribute list.
			''' </summary>
			''' <returns> The number of attributes in the list. </returns>
			''' <seealso cref= org.xml.sax.Attributes#getLength </seealso>
			public Integer length
				Return qAtts.length


			''' <summary>
			''' Return the Namespace URI of the specified attribute.
			''' </summary>
			''' <param name="The"> attribute's index. </param>
			''' <returns> Always the empty string. </returns>
			''' <seealso cref= org.xml.sax.Attributes#getURI </seealso>
			public String getURI(Integer i)
				Return ""


			''' <summary>
			''' Return the local name of the specified attribute.
			''' </summary>
			''' <param name="The"> attribute's index. </param>
			''' <returns> Always the empty string. </returns>
			''' <seealso cref= org.xml.sax.Attributes#getLocalName </seealso>
			public String getLocalName(Integer i)
				Return ""


			''' <summary>
			''' Return the qualified (prefixed) name of the specified attribute.
			''' </summary>
			''' <param name="The"> attribute's index. </param>
			''' <returns> The attribute's qualified name, internalized. </returns>
			public String getQName(Integer i)
				Return qAtts.getName(i).intern()


			''' <summary>
			''' Return the type of the specified attribute.
			''' </summary>
			''' <param name="The"> attribute's index. </param>
			''' <returns> The attribute's type as an internalized string. </returns>
			public String getType(Integer i)
				Return qAtts.getType(i).intern()


			''' <summary>
			''' Return the value of the specified attribute.
			''' </summary>
			''' <param name="The"> attribute's index. </param>
			''' <returns> The attribute's value. </returns>
			public String getValue(Integer i)
				Return qAtts.getValue(i)


			''' <summary>
			''' Look up an attribute index by Namespace name.
			''' </summary>
			''' <param name="uri"> The Namespace URI or the empty string. </param>
			''' <param name="localName"> The local name. </param>
			''' <returns> The attributes index, or -1 if none was found. </returns>
			''' <seealso cref= org.xml.sax.Attributes#getIndex(java.lang.String,java.lang.String) </seealso>
			public Integer getIndex(String uri, String localName)
				Return -1


			''' <summary>
			''' Look up an attribute index by qualified (prefixed) name.
			''' </summary>
			''' <param name="qName"> The qualified name. </param>
			''' <returns> The attributes index, or -1 if none was found. </returns>
			''' <seealso cref= org.xml.sax.Attributes#getIndex(java.lang.String) </seealso>
			public Integer getIndex(String qName)
				Dim max As Integer = atts.length
				For i As Integer = 0 To max - 1
					If qAtts.getName(i).Equals(qName) Then Return i
				Next i
				Return -1


			''' <summary>
			''' Look up the type of an attribute by Namespace name.
			''' </summary>
			''' <param name="uri"> The Namespace URI </param>
			''' <param name="localName"> The local name. </param>
			''' <returns> The attribute's type as an internalized string. </returns>
			public String getType(String uri, String localName)
				Return Nothing


			''' <summary>
			''' Look up the type of an attribute by qualified (prefixed) name.
			''' </summary>
			''' <param name="qName"> The qualified name. </param>
			''' <returns> The attribute's type as an internalized string. </returns>
			public String getType(String qName)
				Return qAtts.getType(qName).intern()


			''' <summary>
			''' Look up the value of an attribute by Namespace name.
			''' </summary>
			''' <param name="uri"> The Namespace URI </param>
			''' <param name="localName"> The local name. </param>
			''' <returns> The attribute's value. </returns>
			public String getValue(String uri, String localName)
				Return Nothing


			''' <summary>
			''' Look up the value of an attribute by qualified (prefixed) name.
			''' </summary>
			''' <param name="qName"> The qualified name. </param>
			''' <returns> The attribute's value. </returns>
			public String getValue(String qName)
				Return qAtts.getValue(qName)

			private org.xml.sax.AttributeList qAtts
	End Class

	' end of ParserAdapter.java

End Namespace
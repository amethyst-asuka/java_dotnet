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

' XMLFilterImpl.java - base SAX2 filter implementation.
' http://www.saxproject.org
' Written by David Megginson
' NO WARRANTY!  This class is in the Public Domain.
' $Id: XMLFilterImpl.java,v 1.3 2004/11/03 22:53:09 jsuttor Exp $

Namespace org.xml.sax.helpers




	''' <summary>
	''' Base class for deriving an XML filter.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' See <a href='http://www.saxproject.org'>http://www.saxproject.org</a>
	''' for further information.
	''' </blockquote>
	''' 
	''' <p>This class is designed to sit between an {@link org.xml.sax.XMLReader
	''' XMLReader} and the client application's event handlers.  By default, it
	''' does nothing but pass requests up to the reader and events
	''' on to the handlers unmodified, but subclasses can override
	''' specific methods to modify the event stream or the configuration
	''' requests as they pass through.</p>
	''' 
	''' @since SAX 2.0
	''' @author David Megginson </summary>
	''' <seealso cref= org.xml.sax.XMLFilter </seealso>
	''' <seealso cref= org.xml.sax.XMLReader </seealso>
	''' <seealso cref= org.xml.sax.EntityResolver </seealso>
	''' <seealso cref= org.xml.sax.DTDHandler </seealso>
	''' <seealso cref= org.xml.sax.ContentHandler </seealso>
	''' <seealso cref= org.xml.sax.ErrorHandler </seealso>
	Public Class XMLFilterImpl
		Implements org.xml.sax.XMLFilter, org.xml.sax.EntityResolver, org.xml.sax.DTDHandler, org.xml.sax.ContentHandler, org.xml.sax.ErrorHandler


		'//////////////////////////////////////////////////////////////////
		' Constructors.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Construct an empty XML filter, with no parent.
		''' 
		''' <p>This filter will have no parent: you must assign a parent
		''' before you start a parse or do any configuration with
		''' setFeature or setProperty, unless you use this as a pure event
		''' consumer rather than as an <seealso cref="XMLReader"/>.</p>
		''' </summary>
		''' <seealso cref= org.xml.sax.XMLReader#setFeature </seealso>
		''' <seealso cref= org.xml.sax.XMLReader#setProperty </seealso>
		''' <seealso cref= #setParent </seealso>
		Public Sub New()
			MyBase.New()
		End Sub


		''' <summary>
		''' Construct an XML filter with the specified parent.
		''' </summary>
		''' <seealso cref= #setParent </seealso>
		''' <seealso cref= #getParent </seealso>
		Public Sub New(ByVal parent As org.xml.sax.XMLReader)
			MyBase.New()
			parent = parent
		End Sub



		'//////////////////////////////////////////////////////////////////
		' Implementation of org.xml.sax.XMLFilter.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Set the parent reader.
		''' 
		''' <p>This is the <seealso cref="org.xml.sax.XMLReader XMLReader"/> from which
		''' this filter will obtain its events and to which it will pass its
		''' configuration requests.  The parent may itself be another filter.</p>
		''' 
		''' <p>If there is no parent reader set, any attempt to parse
		''' or to set or get a feature or property will fail.</p>
		''' </summary>
		''' <param name="parent"> The parent XML reader. </param>
		''' <seealso cref= #getParent </seealso>
		Public Overridable Property parent As org.xml.sax.XMLReader
			Set(ByVal parent As org.xml.sax.XMLReader)
				Me.parent = parent
			End Set
			Get
				Return parent
			End Get
		End Property





		'//////////////////////////////////////////////////////////////////
		' Implementation of org.xml.sax.XMLReader.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Set the value of a feature.
		''' 
		''' <p>This will always fail if the parent is null.</p>
		''' </summary>
		''' <param name="name"> The feature name. </param>
		''' <param name="value"> The requested feature value. </param>
		''' <exception cref="org.xml.sax.SAXNotRecognizedException"> If the feature
		'''            value can't be assigned or retrieved from the parent. </exception>
		''' <exception cref="org.xml.sax.SAXNotSupportedException"> When the
		'''            parent recognizes the feature name but
		'''            cannot set the requested value. </exception>
		Public Overridable Sub setFeature(ByVal name As String, ByVal value As Boolean)
			If parent IsNot Nothing Then
				parent.featureure(name, value)
			Else
				Throw New org.xml.sax.SAXNotRecognizedException("Feature: " & name)
			End If
		End Sub


		''' <summary>
		''' Look up the value of a feature.
		''' 
		''' <p>This will always fail if the parent is null.</p>
		''' </summary>
		''' <param name="name"> The feature name. </param>
		''' <returns> The current value of the feature. </returns>
		''' <exception cref="org.xml.sax.SAXNotRecognizedException"> If the feature
		'''            value can't be assigned or retrieved from the parent. </exception>
		''' <exception cref="org.xml.sax.SAXNotSupportedException"> When the
		'''            parent recognizes the feature name but
		'''            cannot determine its value at this time. </exception>
		Public Overridable Function getFeature(ByVal name As String) As Boolean
			If parent IsNot Nothing Then
				Return parent.getFeature(name)
			Else
				Throw New org.xml.sax.SAXNotRecognizedException("Feature: " & name)
			End If
		End Function


		''' <summary>
		''' Set the value of a property.
		''' 
		''' <p>This will always fail if the parent is null.</p>
		''' </summary>
		''' <param name="name"> The property name. </param>
		''' <param name="value"> The requested property value. </param>
		''' <exception cref="org.xml.sax.SAXNotRecognizedException"> If the property
		'''            value can't be assigned or retrieved from the parent. </exception>
		''' <exception cref="org.xml.sax.SAXNotSupportedException"> When the
		'''            parent recognizes the property name but
		'''            cannot set the requested value. </exception>
		Public Overridable Sub setProperty(ByVal name As String, ByVal value As Object)
			If parent IsNot Nothing Then
				parent.propertyrty(name, value)
			Else
				Throw New org.xml.sax.SAXNotRecognizedException("Property: " & name)
			End If
		End Sub


		''' <summary>
		''' Look up the value of a property.
		''' </summary>
		''' <param name="name"> The property name. </param>
		''' <returns> The current value of the property. </returns>
		''' <exception cref="org.xml.sax.SAXNotRecognizedException"> If the property
		'''            value can't be assigned or retrieved from the parent. </exception>
		''' <exception cref="org.xml.sax.SAXNotSupportedException"> When the
		'''            parent recognizes the property name but
		'''            cannot determine its value at this time. </exception>
		Public Overridable Function getProperty(ByVal name As String) As Object
			If parent IsNot Nothing Then
				Return parent.getProperty(name)
			Else
				Throw New org.xml.sax.SAXNotRecognizedException("Property: " & name)
			End If
		End Function


		''' <summary>
		''' Set the entity resolver.
		''' </summary>
		''' <param name="resolver"> The new entity resolver. </param>
		Public Overridable Property entityResolver As org.xml.sax.EntityResolver
			Set(ByVal resolver As org.xml.sax.EntityResolver)
				entityResolver = resolver
			End Set
			Get
				Return entityResolver
			End Get
		End Property




		''' <summary>
		''' Set the DTD event handler.
		''' </summary>
		''' <param name="handler"> the new DTD handler </param>
		Public Overridable Property dTDHandler As org.xml.sax.DTDHandler
			Set(ByVal handler As org.xml.sax.DTDHandler)
				dtdHandler = handler
			End Set
			Get
				Return dtdHandler
			End Get
		End Property




		''' <summary>
		''' Set the content event handler.
		''' </summary>
		''' <param name="handler"> the new content handler </param>
		Public Overridable Property contentHandler As org.xml.sax.ContentHandler
			Set(ByVal handler As org.xml.sax.ContentHandler)
				contentHandler = handler
			End Set
			Get
				Return contentHandler
			End Get
		End Property




		''' <summary>
		''' Set the error event handler.
		''' </summary>
		''' <param name="handler"> the new error handler </param>
		Public Overridable Property errorHandler As org.xml.sax.ErrorHandler
			Set(ByVal handler As org.xml.sax.ErrorHandler)
				errorHandler = handler
			End Set
			Get
				Return errorHandler
			End Get
		End Property




		''' <summary>
		''' Parse a document.
		''' </summary>
		''' <param name="input"> The input source for the document entity. </param>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <exception cref="java.io.IOException"> An IO exception from the parser,
		'''            possibly from a byte stream or character stream
		'''            supplied by the application. </exception>
		Public Overridable Sub parse(ByVal input As org.xml.sax.InputSource)
			setupParse()
			parent.parse(input)
		End Sub


		''' <summary>
		''' Parse a document.
		''' </summary>
		''' <param name="systemId"> The system identifier as a fully-qualified URI. </param>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <exception cref="java.io.IOException"> An IO exception from the parser,
		'''            possibly from a byte stream or character stream
		'''            supplied by the application. </exception>
		Public Overridable Sub parse(ByVal systemId As String)
			parse(New org.xml.sax.InputSource(systemId))
		End Sub



		'//////////////////////////////////////////////////////////////////
		' Implementation of org.xml.sax.EntityResolver.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Filter an external entity resolution.
		''' </summary>
		''' <param name="publicId"> The entity's public identifier, or null. </param>
		''' <param name="systemId"> The entity's system identifier. </param>
		''' <returns> A new InputSource or null for the default. </returns>
		''' <exception cref="org.xml.sax.SAXException"> The client may throw
		'''            an exception during processing. </exception>
		''' <exception cref="java.io.IOException"> The client may throw an
		'''            I/O-related exception while obtaining the
		'''            new InputSource. </exception>
		Public Overridable Function resolveEntity(ByVal publicId As String, ByVal systemId As String) As org.xml.sax.InputSource
			If entityResolver IsNot Nothing Then
				Return entityResolver.resolveEntity(publicId, systemId)
			Else
				Return Nothing
			End If
		End Function



		'//////////////////////////////////////////////////////////////////
		' Implementation of org.xml.sax.DTDHandler.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Filter a notation declaration event.
		''' </summary>
		''' <param name="name"> The notation name. </param>
		''' <param name="publicId"> The notation's public identifier, or null. </param>
		''' <param name="systemId"> The notation's system identifier, or null. </param>
		''' <exception cref="org.xml.sax.SAXException"> The client may throw
		'''            an exception during processing. </exception>
		Public Overridable Sub notationDecl(ByVal name As String, ByVal publicId As String, ByVal systemId As String)
			If dtdHandler IsNot Nothing Then dtdHandler.notationDecl(name, publicId, systemId)
		End Sub


		''' <summary>
		''' Filter an unparsed entity declaration event.
		''' </summary>
		''' <param name="name"> The entity name. </param>
		''' <param name="publicId"> The entity's public identifier, or null. </param>
		''' <param name="systemId"> The entity's system identifier, or null. </param>
		''' <param name="notationName"> The name of the associated notation. </param>
		''' <exception cref="org.xml.sax.SAXException"> The client may throw
		'''            an exception during processing. </exception>
		Public Overridable Sub unparsedEntityDecl(ByVal name As String, ByVal publicId As String, ByVal systemId As String, ByVal notationName As String)
			If dtdHandler IsNot Nothing Then dtdHandler.unparsedEntityDecl(name, publicId, systemId, notationName)
		End Sub



		'//////////////////////////////////////////////////////////////////
		' Implementation of org.xml.sax.ContentHandler.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Filter a new document locator event.
		''' </summary>
		''' <param name="locator"> The document locator. </param>
		Public Overridable Property documentLocator As org.xml.sax.Locator
			Set(ByVal locator As org.xml.sax.Locator)
				Me.locator = locator
				If contentHandler IsNot Nothing Then contentHandler.documentLocator = locator
			End Set
		End Property


		''' <summary>
		''' Filter a start document event.
		''' </summary>
		''' <exception cref="org.xml.sax.SAXException"> The client may throw
		'''            an exception during processing. </exception>
		Public Overridable Sub startDocument()
			If contentHandler IsNot Nothing Then contentHandler.startDocument()
		End Sub


		''' <summary>
		''' Filter an end document event.
		''' </summary>
		''' <exception cref="org.xml.sax.SAXException"> The client may throw
		'''            an exception during processing. </exception>
		Public Overridable Sub endDocument()
			If contentHandler IsNot Nothing Then contentHandler.endDocument()
		End Sub


		''' <summary>
		''' Filter a start Namespace prefix mapping event.
		''' </summary>
		''' <param name="prefix"> The Namespace prefix. </param>
		''' <param name="uri"> The Namespace URI. </param>
		''' <exception cref="org.xml.sax.SAXException"> The client may throw
		'''            an exception during processing. </exception>
		Public Overridable Sub startPrefixMapping(ByVal prefix As String, ByVal uri As String)
			If contentHandler IsNot Nothing Then contentHandler.startPrefixMapping(prefix, uri)
		End Sub


		''' <summary>
		''' Filter an end Namespace prefix mapping event.
		''' </summary>
		''' <param name="prefix"> The Namespace prefix. </param>
		''' <exception cref="org.xml.sax.SAXException"> The client may throw
		'''            an exception during processing. </exception>
		Public Overridable Sub endPrefixMapping(ByVal prefix As String)
			If contentHandler IsNot Nothing Then contentHandler.endPrefixMapping(prefix)
		End Sub


		''' <summary>
		''' Filter a start element event.
		''' </summary>
		''' <param name="uri"> The element's Namespace URI, or the empty string. </param>
		''' <param name="localName"> The element's local name, or the empty string. </param>
		''' <param name="qName"> The element's qualified (prefixed) name, or the empty
		'''        string. </param>
		''' <param name="atts"> The element's attributes. </param>
		''' <exception cref="org.xml.sax.SAXException"> The client may throw
		'''            an exception during processing. </exception>
		Public Overridable Sub startElement(ByVal uri As String, ByVal localName As String, ByVal qName As String, ByVal atts As org.xml.sax.Attributes)
			If contentHandler IsNot Nothing Then contentHandler.startElement(uri, localName, qName, atts)
		End Sub


		''' <summary>
		''' Filter an end element event.
		''' </summary>
		''' <param name="uri"> The element's Namespace URI, or the empty string. </param>
		''' <param name="localName"> The element's local name, or the empty string. </param>
		''' <param name="qName"> The element's qualified (prefixed) name, or the empty
		'''        string. </param>
		''' <exception cref="org.xml.sax.SAXException"> The client may throw
		'''            an exception during processing. </exception>
		Public Overridable Sub endElement(ByVal uri As String, ByVal localName As String, ByVal qName As String)
			If contentHandler IsNot Nothing Then contentHandler.endElement(uri, localName, qName)
		End Sub


		''' <summary>
		''' Filter a character data event.
		''' </summary>
		''' <param name="ch"> An array of characters. </param>
		''' <param name="start"> The starting position in the array. </param>
		''' <param name="length"> The number of characters to use from the array. </param>
		''' <exception cref="org.xml.sax.SAXException"> The client may throw
		'''            an exception during processing. </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public void characters(char ch() , int start, int length) throws org.xml.sax.SAXException
			If contentHandler IsNot Nothing Then contentHandler.characters(ch, start, length)


		''' <summary>
		''' Filter an ignorable whitespace event.
		''' </summary>
		''' <param name="ch"> An array of characters. </param>
		''' <param name="start"> The starting position in the array. </param>
		''' <param name="length"> The number of characters to use from the array. </param>
		''' <exception cref="org.xml.sax.SAXException"> The client may throw
		'''            an exception during processing. </exception>
		public void ignorableWhitespace(Char ch() , Integer start, Integer length) throws org.xml.sax.SAXException
			If contentHandler IsNot Nothing Then contentHandler.ignorableWhitespace(ch, start, length)


		''' <summary>
		''' Filter a processing instruction event.
		''' </summary>
		''' <param name="target"> The processing instruction target. </param>
		''' <param name="data"> The text following the target. </param>
		''' <exception cref="org.xml.sax.SAXException"> The client may throw
		'''            an exception during processing. </exception>
		public void processingInstruction(String target, String data) throws org.xml.sax.SAXException
			If contentHandler IsNot Nothing Then contentHandler.processingInstruction(target, data)


		''' <summary>
		''' Filter a skipped entity event.
		''' </summary>
		''' <param name="name"> The name of the skipped entity. </param>
		''' <exception cref="org.xml.sax.SAXException"> The client may throw
		'''            an exception during processing. </exception>
		public void skippedEntity(String name) throws org.xml.sax.SAXException
			If contentHandler IsNot Nothing Then contentHandler.skippedEntity(name)



		'//////////////////////////////////////////////////////////////////
		' Implementation of org.xml.sax.ErrorHandler.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Filter a warning event.
		''' </summary>
		''' <param name="e"> The warning as an exception. </param>
		''' <exception cref="org.xml.sax.SAXException"> The client may throw
		'''            an exception during processing. </exception>
		public void warning(org.xml.sax.SAXParseException e) throws org.xml.sax.SAXException
			If errorHandler IsNot Nothing Then errorHandler.warning(e)


		''' <summary>
		''' Filter an error event.
		''' </summary>
		''' <param name="e"> The error as an exception. </param>
		''' <exception cref="org.xml.sax.SAXException"> The client may throw
		'''            an exception during processing. </exception>
		public void error(org.xml.sax.SAXParseException e) throws org.xml.sax.SAXException
			If errorHandler IsNot Nothing Then errorHandler.error(e)


		''' <summary>
		''' Filter a fatal error event.
		''' </summary>
		''' <param name="e"> The error as an exception. </param>
		''' <exception cref="org.xml.sax.SAXException"> The client may throw
		'''            an exception during processing. </exception>
		public void fatalError(org.xml.sax.SAXParseException e) throws org.xml.sax.SAXException
			If errorHandler IsNot Nothing Then errorHandler.fatalError(e)



		'//////////////////////////////////////////////////////////////////
		' Internal methods.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Set up before a parse.
		''' 
		''' <p>Before every parse, check whether the parent is
		''' non-null, and re-register the filter for all of the
		''' events.</p>
		''' </summary>
		private void setupParse()
			If parent Is Nothing Then Throw New NullPointerException("No parent for filter")
			parent.entityResolver = Me
			parent.dTDHandler = Me
			parent.contentHandler = Me
			parent.errorHandler = Me



		'//////////////////////////////////////////////////////////////////
		' Internal state.
		'//////////////////////////////////////////////////////////////////

		private org.xml.sax.XMLReader parent = Nothing
		private org.xml.sax.Locator locator = Nothing
		private org.xml.sax.EntityResolver entityResolver = Nothing
		private org.xml.sax.DTDHandler dtdHandler = Nothing
		private org.xml.sax.ContentHandler contentHandler = Nothing
		private org.xml.sax.ErrorHandler errorHandler = Nothing

	End Class

	' end of XMLFilterImpl.java

End Namespace
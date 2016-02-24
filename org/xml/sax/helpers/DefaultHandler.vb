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

' DefaultHandler.java - default implementation of the core handlers.
' http://www.saxproject.org
' Written by David Megginson
' NO WARRANTY!  This class is in the public domain.
' $Id: DefaultHandler.java,v 1.3 2006/04/13 02:06:32 jeffsuttor Exp $

Namespace org.xml.sax.helpers




	''' <summary>
	''' Default base class for SAX2 event handlers.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' See <a href='http://www.saxproject.org'>http://www.saxproject.org</a>
	''' for further information.
	''' </blockquote>
	''' 
	''' <p>This class is available as a convenience base class for SAX2
	''' applications: it provides default implementations for all of the
	''' callbacks in the four core SAX2 handler classes:</p>
	''' 
	''' <ul>
	''' <li><seealso cref="org.xml.sax.EntityResolver EntityResolver"/></li>
	''' <li><seealso cref="org.xml.sax.DTDHandler DTDHandler"/></li>
	''' <li><seealso cref="org.xml.sax.ContentHandler ContentHandler"/></li>
	''' <li><seealso cref="org.xml.sax.ErrorHandler ErrorHandler"/></li>
	''' </ul>
	''' 
	''' <p>Application writers can extend this class when they need to
	''' implement only part of an interface; parser writers can
	''' instantiate this class to provide default handlers when the
	''' application has not supplied its own.</p>
	''' 
	''' <p>This class replaces the deprecated SAX1
	''' <seealso cref="org.xml.sax.HandlerBase HandlerBase"/> class.</p>
	''' 
	''' @since SAX 2.0
	''' @author David Megginson, </summary>
	''' <seealso cref= org.xml.sax.EntityResolver </seealso>
	''' <seealso cref= org.xml.sax.DTDHandler </seealso>
	''' <seealso cref= org.xml.sax.ContentHandler </seealso>
	''' <seealso cref= org.xml.sax.ErrorHandler </seealso>
	Public Class DefaultHandler
		Implements org.xml.sax.EntityResolver, org.xml.sax.DTDHandler, org.xml.sax.ContentHandler, org.xml.sax.ErrorHandler


		'//////////////////////////////////////////////////////////////////
		' Default implementation of the EntityResolver interface.
		'//////////////////////////////////////////////////////////////////

		''' <summary>
		''' Resolve an external entity.
		''' 
		''' <p>Always return null, so that the parser will use the system
		''' identifier provided in the XML document.  This method implements
		''' the SAX default behaviour: application writers can override it
		''' in a subclass to do special translations such as catalog lookups
		''' or URI redirection.</p>
		''' </summary>
		''' <param name="publicId"> The public identifier, or null if none is
		'''                 available. </param>
		''' <param name="systemId"> The system identifier provided in the XML
		'''                 document. </param>
		''' <returns> The new input source, or null to require the
		'''         default behaviour. </returns>
		''' <exception cref="java.io.IOException"> If there is an error setting
		'''            up the new input source. </exception>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <seealso cref= org.xml.sax.EntityResolver#resolveEntity </seealso>
		Public Overridable Function resolveEntity(ByVal publicId As String, ByVal systemId As String) As org.xml.sax.InputSource
			Return Nothing
		End Function



		'//////////////////////////////////////////////////////////////////
		' Default implementation of DTDHandler interface.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Receive notification of a notation declaration.
		''' 
		''' <p>By default, do nothing.  Application writers may override this
		''' method in a subclass if they wish to keep track of the notations
		''' declared in a document.</p>
		''' </summary>
		''' <param name="name"> The notation name. </param>
		''' <param name="publicId"> The notation public identifier, or null if not
		'''                 available. </param>
		''' <param name="systemId"> The notation system identifier. </param>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <seealso cref= org.xml.sax.DTDHandler#notationDecl </seealso>
		Public Overridable Sub notationDecl(ByVal name As String, ByVal publicId As String, ByVal systemId As String)
			' no op
		End Sub


		''' <summary>
		''' Receive notification of an unparsed entity declaration.
		''' 
		''' <p>By default, do nothing.  Application writers may override this
		''' method in a subclass to keep track of the unparsed entities
		''' declared in a document.</p>
		''' </summary>
		''' <param name="name"> The entity name. </param>
		''' <param name="publicId"> The entity public identifier, or null if not
		'''                 available. </param>
		''' <param name="systemId"> The entity system identifier. </param>
		''' <param name="notationName"> The name of the associated notation. </param>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <seealso cref= org.xml.sax.DTDHandler#unparsedEntityDecl </seealso>
		Public Overridable Sub unparsedEntityDecl(ByVal name As String, ByVal publicId As String, ByVal systemId As String, ByVal notationName As String)
			' no op
		End Sub



		'//////////////////////////////////////////////////////////////////
		' Default implementation of ContentHandler interface.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Receive a Locator object for document events.
		''' 
		''' <p>By default, do nothing.  Application writers may override this
		''' method in a subclass if they wish to store the locator for use
		''' with other document events.</p>
		''' </summary>
		''' <param name="locator"> A locator for all SAX document events. </param>
		''' <seealso cref= org.xml.sax.ContentHandler#setDocumentLocator </seealso>
		''' <seealso cref= org.xml.sax.Locator </seealso>
		Public Overridable Property documentLocator As org.xml.sax.Locator
			Set(ByVal locator As org.xml.sax.Locator)
				' no op
			End Set
		End Property


		''' <summary>
		''' Receive notification of the beginning of the document.
		''' 
		''' <p>By default, do nothing.  Application writers may override this
		''' method in a subclass to take specific actions at the beginning
		''' of a document (such as allocating the root node of a tree or
		''' creating an output file).</p>
		''' </summary>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <seealso cref= org.xml.sax.ContentHandler#startDocument </seealso>
		Public Overridable Sub startDocument()
			' no op
		End Sub


		''' <summary>
		''' Receive notification of the end of the document.
		''' 
		''' <p>By default, do nothing.  Application writers may override this
		''' method in a subclass to take specific actions at the end
		''' of a document (such as finalising a tree or closing an output
		''' file).</p>
		''' </summary>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <seealso cref= org.xml.sax.ContentHandler#endDocument </seealso>
		Public Overridable Sub endDocument()
			' no op
		End Sub


		''' <summary>
		''' Receive notification of the start of a Namespace mapping.
		''' 
		''' <p>By default, do nothing.  Application writers may override this
		''' method in a subclass to take specific actions at the start of
		''' each Namespace prefix scope (such as storing the prefix mapping).</p>
		''' </summary>
		''' <param name="prefix"> The Namespace prefix being declared. </param>
		''' <param name="uri"> The Namespace URI mapped to the prefix. </param>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <seealso cref= org.xml.sax.ContentHandler#startPrefixMapping </seealso>
		Public Overridable Sub startPrefixMapping(ByVal prefix As String, ByVal uri As String)
			' no op
		End Sub


		''' <summary>
		''' Receive notification of the end of a Namespace mapping.
		''' 
		''' <p>By default, do nothing.  Application writers may override this
		''' method in a subclass to take specific actions at the end of
		''' each prefix mapping.</p>
		''' </summary>
		''' <param name="prefix"> The Namespace prefix being declared. </param>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <seealso cref= org.xml.sax.ContentHandler#endPrefixMapping </seealso>
		Public Overridable Sub endPrefixMapping(ByVal prefix As String)
			' no op
		End Sub


		''' <summary>
		''' Receive notification of the start of an element.
		''' 
		''' <p>By default, do nothing.  Application writers may override this
		''' method in a subclass to take specific actions at the start of
		''' each element (such as allocating a new tree node or writing
		''' output to a file).</p>
		''' </summary>
		''' <param name="uri"> The Namespace URI, or the empty string if the
		'''        element has no Namespace URI or if Namespace
		'''        processing is not being performed. </param>
		''' <param name="localName"> The local name (without prefix), or the
		'''        empty string if Namespace processing is not being
		'''        performed. </param>
		''' <param name="qName"> The qualified name (with prefix), or the
		'''        empty string if qualified names are not available. </param>
		''' <param name="attributes"> The attributes attached to the element.  If
		'''        there are no attributes, it shall be an empty
		'''        Attributes object. </param>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <seealso cref= org.xml.sax.ContentHandler#startElement </seealso>
		Public Overridable Sub startElement(ByVal uri As String, ByVal localName As String, ByVal qName As String, ByVal attributes As org.xml.sax.Attributes)
			' no op
		End Sub


		''' <summary>
		''' Receive notification of the end of an element.
		''' 
		''' <p>By default, do nothing.  Application writers may override this
		''' method in a subclass to take specific actions at the end of
		''' each element (such as finalising a tree node or writing
		''' output to a file).</p>
		''' </summary>
		''' <param name="uri"> The Namespace URI, or the empty string if the
		'''        element has no Namespace URI or if Namespace
		'''        processing is not being performed. </param>
		''' <param name="localName"> The local name (without prefix), or the
		'''        empty string if Namespace processing is not being
		'''        performed. </param>
		''' <param name="qName"> The qualified name (with prefix), or the
		'''        empty string if qualified names are not available. </param>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <seealso cref= org.xml.sax.ContentHandler#endElement </seealso>
		Public Overridable Sub endElement(ByVal uri As String, ByVal localName As String, ByVal qName As String)
			' no op
		End Sub


		''' <summary>
		''' Receive notification of character data inside an element.
		''' 
		''' <p>By default, do nothing.  Application writers may override this
		''' method to take specific actions for each chunk of character data
		''' (such as adding the data to a node or buffer, or printing it to
		''' a file).</p>
		''' </summary>
		''' <param name="ch"> The characters. </param>
		''' <param name="start"> The start position in the character array. </param>
		''' <param name="length"> The number of characters to use from the
		'''               character array. </param>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <seealso cref= org.xml.sax.ContentHandler#characters </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public void characters(char ch() , int start, int length) throws org.xml.sax.SAXException
			' no op


		''' <summary>
		''' Receive notification of ignorable whitespace in element content.
		''' 
		''' <p>By default, do nothing.  Application writers may override this
		''' method to take specific actions for each chunk of ignorable
		''' whitespace (such as adding data to a node or buffer, or printing
		''' it to a file).</p>
		''' </summary>
		''' <param name="ch"> The whitespace characters. </param>
		''' <param name="start"> The start position in the character array. </param>
		''' <param name="length"> The number of characters to use from the
		'''               character array. </param>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <seealso cref= org.xml.sax.ContentHandler#ignorableWhitespace </seealso>
		public void ignorableWhitespace(Char ch() , Integer start, Integer length) throws org.xml.sax.SAXException
			' no op


		''' <summary>
		''' Receive notification of a processing instruction.
		''' 
		''' <p>By default, do nothing.  Application writers may override this
		''' method in a subclass to take specific actions for each
		''' processing instruction, such as setting status variables or
		''' invoking other methods.</p>
		''' </summary>
		''' <param name="target"> The processing instruction target. </param>
		''' <param name="data"> The processing instruction data, or null if
		'''             none is supplied. </param>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <seealso cref= org.xml.sax.ContentHandler#processingInstruction </seealso>
		public void processingInstruction(String target, String data) throws org.xml.sax.SAXException
			' no op


		''' <summary>
		''' Receive notification of a skipped entity.
		''' 
		''' <p>By default, do nothing.  Application writers may override this
		''' method in a subclass to take specific actions for each
		''' processing instruction, such as setting status variables or
		''' invoking other methods.</p>
		''' </summary>
		''' <param name="name"> The name of the skipped entity. </param>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <seealso cref= org.xml.sax.ContentHandler#processingInstruction </seealso>
		public void skippedEntity(String name) throws org.xml.sax.SAXException
			' no op



		'//////////////////////////////////////////////////////////////////
		' Default implementation of the ErrorHandler interface.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Receive notification of a parser warning.
		''' 
		''' <p>The default implementation does nothing.  Application writers
		''' may override this method in a subclass to take specific actions
		''' for each warning, such as inserting the message in a log file or
		''' printing it to the console.</p>
		''' </summary>
		''' <param name="e"> The warning information encoded as an exception. </param>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <seealso cref= org.xml.sax.ErrorHandler#warning </seealso>
		''' <seealso cref= org.xml.sax.SAXParseException </seealso>
		public void warning(org.xml.sax.SAXParseException e) throws org.xml.sax.SAXException
			' no op


		''' <summary>
		''' Receive notification of a recoverable parser error.
		''' 
		''' <p>The default implementation does nothing.  Application writers
		''' may override this method in a subclass to take specific actions
		''' for each error, such as inserting the message in a log file or
		''' printing it to the console.</p>
		''' </summary>
		''' <param name="e"> The error information encoded as an exception. </param>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <seealso cref= org.xml.sax.ErrorHandler#warning </seealso>
		''' <seealso cref= org.xml.sax.SAXParseException </seealso>
		public void error(org.xml.sax.SAXParseException e) throws org.xml.sax.SAXException
			' no op


		''' <summary>
		''' Report a fatal XML parsing error.
		''' 
		''' <p>The default implementation throws a SAXParseException.
		''' Application writers may override this method in a subclass if
		''' they need to take specific actions for each fatal error (such as
		''' collecting all of the errors into a single report): in any case,
		''' the application must stop all regular processing when this
		''' method is invoked, since the document is no longer reliable, and
		''' the parser may no longer report parsing events.</p>
		''' </summary>
		''' <param name="e"> The error information encoded as an exception. </param>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <seealso cref= org.xml.sax.ErrorHandler#fatalError </seealso>
		''' <seealso cref= org.xml.sax.SAXParseException </seealso>
		public void fatalError(org.xml.sax.SAXParseException e) throws org.xml.sax.SAXException
			Throw e

	End Class

	' end of DefaultHandler.java

End Namespace
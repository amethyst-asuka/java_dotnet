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

' SAX default handler base class.
' http://www.saxproject.org
' No warranty; no copyright -- use this as you will.
' $Id: HandlerBase.java,v 1.2 2005/06/10 03:50:47 jeffsuttor Exp $

Namespace org.xml.sax

	''' <summary>
	''' Default base class for handlers.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' See <a href='http://www.saxproject.org'>http://www.saxproject.org</a>
	''' for further information.
	''' </blockquote>
	''' 
	''' <p>This class implements the default behaviour for four SAX1
	''' interfaces: EntityResolver, DTDHandler, DocumentHandler,
	''' and ErrorHandler.  It is now obsolete, but is included in SAX2 to
	''' support legacy SAX1 applications.  SAX2 applications should use
	''' the <seealso cref="org.xml.sax.helpers.DefaultHandler DefaultHandler"/>
	''' class instead.</p>
	''' 
	''' <p>Application writers can extend this class when they need to
	''' implement only part of an interface; parser writers can
	''' instantiate this class to provide default handlers when the
	''' application has not supplied its own.</p>
	''' 
	''' <p>Note that the use of this class is optional.</p>
	''' </summary>
	''' @deprecated This class works with the deprecated
	'''             <seealso cref="org.xml.sax.DocumentHandler DocumentHandler"/>
	'''             interface.  It has been replaced by the SAX2
	'''             <seealso cref="org.xml.sax.helpers.DefaultHandler DefaultHandler"/>
	'''             class.
	''' @since SAX 1.0
	''' @author David Megginson 
	''' <seealso cref= org.xml.sax.EntityResolver </seealso>
	''' <seealso cref= org.xml.sax.DTDHandler </seealso>
	''' <seealso cref= org.xml.sax.DocumentHandler </seealso>
	''' <seealso cref= org.xml.sax.ErrorHandler </seealso>
	Public Class HandlerBase
		Implements EntityResolver, DTDHandler, DocumentHandler, ErrorHandler


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
		''' <param name="publicId"> The public identifer, or null if none is
		'''                 available. </param>
		''' <param name="systemId"> The system identifier provided in the XML
		'''                 document. </param>
		''' <returns> The new input source, or null to require the
		'''         default behaviour. </returns>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <seealso cref= org.xml.sax.EntityResolver#resolveEntity </seealso>
		Public Overridable Function resolveEntity(ByVal publicId As String, ByVal systemId As String) As InputSource Implements EntityResolver.resolveEntity
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
		''' <seealso cref= org.xml.sax.DTDHandler#notationDecl </seealso>
		Public Overridable Sub notationDecl(ByVal name As String, ByVal publicId As String, ByVal systemId As String) Implements DTDHandler.notationDecl
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
		''' <seealso cref= org.xml.sax.DTDHandler#unparsedEntityDecl </seealso>
		Public Overridable Sub unparsedEntityDecl(ByVal name As String, ByVal publicId As String, ByVal systemId As String, ByVal notationName As String) Implements DTDHandler.unparsedEntityDecl
			' no op
		End Sub



		'//////////////////////////////////////////////////////////////////
		' Default implementation of DocumentHandler interface.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Receive a Locator object for document events.
		''' 
		''' <p>By default, do nothing.  Application writers may override this
		''' method in a subclass if they wish to store the locator for use
		''' with other document events.</p>
		''' </summary>
		''' <param name="locator"> A locator for all SAX document events. </param>
		''' <seealso cref= org.xml.sax.DocumentHandler#setDocumentLocator </seealso>
		''' <seealso cref= org.xml.sax.Locator </seealso>
		Public Overridable Property documentLocator Implements DocumentHandler.setDocumentLocator As Locator
			Set(ByVal locator As Locator)
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
		''' <seealso cref= org.xml.sax.DocumentHandler#startDocument </seealso>
		Public Overridable Sub startDocument() Implements DocumentHandler.startDocument
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
		''' <seealso cref= org.xml.sax.DocumentHandler#endDocument </seealso>
		Public Overridable Sub endDocument() Implements DocumentHandler.endDocument
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
		''' <param name="name"> The element type name. </param>
		''' <param name="attributes"> The specified or defaulted attributes. </param>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler#startElement </seealso>
		Public Overridable Sub startElement(ByVal name As String, ByVal attributes As AttributeList) Implements DocumentHandler.startElement
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
		''' <param name="name"> the element name </param>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <seealso cref= org.xml.sax.DocumentHandler#endElement </seealso>
		Public Overridable Sub endElement(ByVal name As String) Implements DocumentHandler.endElement
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
		''' <seealso cref= org.xml.sax.DocumentHandler#characters </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public void characters(char ch() , int start, int length) throws SAXException
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
		''' <seealso cref= org.xml.sax.DocumentHandler#ignorableWhitespace </seealso>
		public void ignorableWhitespace(Char ch() , Integer start, Integer length) throws SAXException
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
		''' <seealso cref= org.xml.sax.DocumentHandler#processingInstruction </seealso>
		public void processingInstruction(String target, String data) throws SAXException
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
		public void warning(SAXParseException e) throws SAXException
			' no op


		''' <summary>
		''' Receive notification of a recoverable parser error.
		''' 
		''' <p>The default implementation does nothing.  Application writers
		''' may override this method in a subclass to take specific actions
		''' for each error, such as inserting the message in a log file or
		''' printing it to the console.</p>
		''' </summary>
		''' <param name="e"> The warning information encoded as an exception. </param>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <seealso cref= org.xml.sax.ErrorHandler#warning </seealso>
		''' <seealso cref= org.xml.sax.SAXParseException </seealso>
		public void error(SAXParseException e) throws SAXException
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
		public void fatalError(SAXParseException e) throws SAXException
			Throw e

	End Class

	' end of HandlerBase.java

End Namespace
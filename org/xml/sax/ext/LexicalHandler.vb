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

' LexicalHandler.java - optional handler for lexical parse events.
' http://www.saxproject.org
' Public Domain: no warranty.
' $Id: LexicalHandler.java,v 1.2 2004/11/03 22:49:08 jsuttor Exp $

Namespace org.xml.sax.ext


	''' <summary>
	''' SAX2 extension handler for lexical events.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' See <a href='http://www.saxproject.org'>http://www.saxproject.org</a>
	''' for further information.
	''' </blockquote>
	''' 
	''' <p>This is an optional extension handler for SAX2 to provide
	''' lexical information about an XML document, such as comments
	''' and CDATA section boundaries.
	''' XML readers are not required to recognize this handler, and it
	''' is not part of core-only SAX2 distributions.</p>
	''' 
	''' <p>The events in the lexical handler apply to the entire document,
	''' not just to the document element, and all lexical handler events
	''' must appear between the content handler's startDocument and
	''' endDocument events.</p>
	''' 
	''' <p>To set the LexicalHandler for an XML reader, use the
	''' <seealso cref="org.xml.sax.XMLReader#setProperty setProperty"/> method
	''' with the property name
	''' <code>http://xml.org/sax/properties/lexical-handler</code>
	''' and an object implementing this interface (or null) as the value.
	''' If the reader does not report lexical events, it will throw a
	''' <seealso cref="org.xml.sax.SAXNotRecognizedException SAXNotRecognizedException"/>
	''' when you attempt to register the handler.</p>
	''' 
	''' @since SAX 2.0 (extensions 1.0)
	''' @author David Megginson
	''' </summary>
	Public Interface LexicalHandler

		''' <summary>
		''' Report the start of DTD declarations, if any.
		''' 
		''' <p>This method is intended to report the beginning of the
		''' DOCTYPE declaration; if the document has no DOCTYPE declaration,
		''' this method will not be invoked.</p>
		''' 
		''' <p>All declarations reported through
		''' <seealso cref="org.xml.sax.DTDHandler DTDHandler"/> or
		''' <seealso cref="org.xml.sax.ext.DeclHandler DeclHandler"/> events must appear
		''' between the startDTD and <seealso cref="#endDTD endDTD"/> events.
		''' Declarations are assumed to belong to the internal DTD subset
		''' unless they appear between <seealso cref="#startEntity startEntity"/>
		''' and <seealso cref="#endEntity endEntity"/> events.  Comments and
		''' processing instructions from the DTD should also be reported
		''' between the startDTD and endDTD events, in their original
		''' order of (logical) occurrence; they are not required to
		''' appear in their correct locations relative to DTDHandler
		''' or DeclHandler events, however.</p>
		''' 
		''' <p>Note that the start/endDTD events will appear within
		''' the start/endDocument events from ContentHandler and
		''' before the first
		''' <seealso cref="org.xml.sax.ContentHandler#startElement startElement"/>
		''' event.</p>
		''' </summary>
		''' <param name="name"> The document type name. </param>
		''' <param name="publicId"> The declared public identifier for the
		'''        external DTD subset, or null if none was declared. </param>
		''' <param name="systemId"> The declared system identifier for the
		'''        external DTD subset, or null if none was declared.
		'''        (Note that this is not resolved against the document
		'''        base URI.) </param>
		''' <exception cref="SAXException"> The application may raise an
		'''            exception. </exception>
		''' <seealso cref= #endDTD </seealso>
		''' <seealso cref= #startEntity </seealso>
		Sub startDTD(ByVal name As String, ByVal publicId As String, ByVal systemId As String)


		''' <summary>
		''' Report the end of DTD declarations.
		''' 
		''' <p>This method is intended to report the end of the
		''' DOCTYPE declaration; if the document has no DOCTYPE declaration,
		''' this method will not be invoked.</p>
		''' </summary>
		''' <exception cref="SAXException"> The application may raise an exception. </exception>
		''' <seealso cref= #startDTD </seealso>
		Sub endDTD()


		''' <summary>
		''' Report the beginning of some internal and external XML entities.
		''' 
		''' <p>The reporting of parameter entities (including
		''' the external DTD subset) is optional, and SAX2 drivers that
		''' report LexicalHandler events may not implement it; you can use the
		''' <code
		''' >http://xml.org/sax/features/lexical-handler/parameter-entities</code>
		''' feature to query or control the reporting of parameter entities.</p>
		''' 
		''' <p>General entities are reported with their regular names,
		''' parameter entities have '%' prepended to their names, and
		''' the external DTD subset has the pseudo-entity name "[dtd]".</p>
		''' 
		''' <p>When a SAX2 driver is providing these events, all other
		''' events must be properly nested within start/end entity
		''' events.  There is no additional requirement that events from
		''' <seealso cref="org.xml.sax.ext.DeclHandler DeclHandler"/> or
		''' <seealso cref="org.xml.sax.DTDHandler DTDHandler"/> be properly ordered.</p>
		''' 
		''' <p>Note that skipped entities will be reported through the
		''' <seealso cref="org.xml.sax.ContentHandler#skippedEntity skippedEntity"/>
		''' event, which is part of the ContentHandler interface.</p>
		''' 
		''' <p>Because of the streaming event model that SAX uses, some
		''' entity boundaries cannot be reported under any
		''' circumstances:</p>
		''' 
		''' <ul>
		''' <li>general entities within attribute values</li>
		''' <li>parameter entities within declarations</li>
		''' </ul>
		''' 
		''' <p>These will be silently expanded, with no indication of where
		''' the original entity boundaries were.</p>
		''' 
		''' <p>Note also that the boundaries of character references (which
		''' are not really entities anyway) are not reported.</p>
		''' 
		''' <p>All start/endEntity events must be properly nested.
		''' </summary>
		''' <param name="name"> The name of the entity.  If it is a parameter
		'''        entity, the name will begin with '%', and if it is the
		'''        external DTD subset, it will be "[dtd]". </param>
		''' <exception cref="SAXException"> The application may raise an exception. </exception>
		''' <seealso cref= #endEntity </seealso>
		''' <seealso cref= org.xml.sax.ext.DeclHandler#internalEntityDecl </seealso>
		''' <seealso cref= org.xml.sax.ext.DeclHandler#externalEntityDecl </seealso>
		Sub startEntity(ByVal name As String)


		''' <summary>
		''' Report the end of an entity.
		''' </summary>
		''' <param name="name"> The name of the entity that is ending. </param>
		''' <exception cref="SAXException"> The application may raise an exception. </exception>
		''' <seealso cref= #startEntity </seealso>
		Sub endEntity(ByVal name As String)


		''' <summary>
		''' Report the start of a CDATA section.
		''' 
		''' <p>The contents of the CDATA section will be reported through
		''' the regular {@link org.xml.sax.ContentHandler#characters
		''' characters} event; this event is intended only to report
		''' the boundary.</p>
		''' </summary>
		''' <exception cref="SAXException"> The application may raise an exception. </exception>
		''' <seealso cref= #endCDATA </seealso>
		Sub startCDATA()


		''' <summary>
		''' Report the end of a CDATA section.
		''' </summary>
		''' <exception cref="SAXException"> The application may raise an exception. </exception>
		''' <seealso cref= #startCDATA </seealso>
		Sub endCDATA()


		''' <summary>
		''' Report an XML comment anywhere in the document.
		''' 
		''' <p>This callback will be used for comments inside or outside the
		''' document element, including comments in the external DTD
		''' subset (if read).  Comments in the DTD must be properly
		''' nested inside start/endDTD and start/endEntity events (if
		''' used).</p>
		''' </summary>
		''' <param name="ch"> An array holding the characters in the comment. </param>
		''' <param name="start"> The starting position in the array. </param>
		''' <param name="length"> The number of characters to use from the array. </param>
		''' <exception cref="SAXException"> The application may raise an exception. </exception>
		Sub comment(Char ByVal  As ch(), ByVal start As Integer, ByVal length As Integer)

	End Interface

	' end of LexicalHandler.java

End Namespace
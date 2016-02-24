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

' SAX DTD handler.
' http://www.saxproject.org
' No warranty; no copyright -- use this as you will.
' $Id: DTDHandler.java,v 1.2 2004/11/03 22:44:51 jsuttor Exp $

Namespace org.xml.sax

	''' <summary>
	''' Receive notification of basic DTD-related events.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' See <a href='http://www.saxproject.org'>http://www.saxproject.org</a>
	''' for further information.
	''' </blockquote>
	''' 
	''' <p>If a SAX application needs information about notations and
	''' unparsed entities, then the application implements this
	''' interface and registers an instance with the SAX parser using
	''' the parser's setDTDHandler method.  The parser uses the
	''' instance to report notation and unparsed entity declarations to
	''' the application.</p>
	''' 
	''' <p>Note that this interface includes only those DTD events that
	''' the XML recommendation <em>requires</em> processors to report:
	''' notation and unparsed entity declarations.</p>
	''' 
	''' <p>The SAX parser may report these events in any order, regardless
	''' of the order in which the notations and unparsed entities were
	''' declared; however, all DTD events must be reported after the
	''' document handler's startDocument event, and before the first
	''' startElement event.
	''' (If the <seealso cref="org.xml.sax.ext.LexicalHandler LexicalHandler"/> is
	''' used, these events must also be reported before the endDTD event.)
	''' </p>
	''' 
	''' <p>It is up to the application to store the information for
	''' future use (perhaps in a hash table or object tree).
	''' If the application encounters attributes of type "NOTATION",
	''' "ENTITY", or "ENTITIES", it can use the information that it
	''' obtained through this interface to find the entity and/or
	''' notation corresponding with the attribute value.</p>
	''' 
	''' @since SAX 1.0
	''' @author David Megginson </summary>
	''' <seealso cref= org.xml.sax.XMLReader#setDTDHandler </seealso>
	Public Interface DTDHandler


		''' <summary>
		''' Receive notification of a notation declaration event.
		''' 
		''' <p>It is up to the application to record the notation for later
		''' reference, if necessary;
		''' notations may appear as attribute values and in unparsed entity
		''' declarations, and are sometime used with processing instruction
		''' target names.</p>
		''' 
		''' <p>At least one of publicId and systemId must be non-null.
		''' If a system identifier is present, and it is a URL, the SAX
		''' parser must resolve it fully before passing it to the
		''' application through this event.</p>
		''' 
		''' <p>There is no guarantee that the notation declaration will be
		''' reported before any unparsed entities that use it.</p>
		''' </summary>
		''' <param name="name"> The notation name. </param>
		''' <param name="publicId"> The notation's public identifier, or null if
		'''        none was given. </param>
		''' <param name="systemId"> The notation's system identifier, or null if
		'''        none was given. </param>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <seealso cref= #unparsedEntityDecl </seealso>
		''' <seealso cref= org.xml.sax.Attributes </seealso>
		Sub notationDecl(ByVal name As String, ByVal publicId As String, ByVal systemId As String)


		''' <summary>
		''' Receive notification of an unparsed entity declaration event.
		''' 
		''' <p>Note that the notation name corresponds to a notation
		''' reported by the <seealso cref="#notationDecl notationDecl"/> event.
		''' It is up to the application to record the entity for later
		''' reference, if necessary;
		''' unparsed entities may appear as attribute values.
		''' </p>
		''' 
		''' <p>If the system identifier is a URL, the parser must resolve it
		''' fully before passing it to the application.</p>
		''' </summary>
		''' <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		'''            wrapping another exception. </exception>
		''' <param name="name"> The unparsed entity's name. </param>
		''' <param name="publicId"> The entity's public identifier, or null if none
		'''        was given. </param>
		''' <param name="systemId"> The entity's system identifier. </param>
		''' <param name="notationName"> The name of the associated notation. </param>
		''' <seealso cref= #notationDecl </seealso>
		''' <seealso cref= org.xml.sax.Attributes </seealso>
		Sub unparsedEntityDecl(ByVal name As String, ByVal publicId As String, ByVal systemId As String, ByVal notationName As String)

	End Interface

	' end of DTDHandler.java

End Namespace
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

' DeclHandler.java - Optional handler for DTD declaration events.
' http://www.saxproject.org
' Public Domain: no warranty.
' $Id: DeclHandler.java,v 1.2 2004/11/03 22:49:08 jsuttor Exp $

Namespace org.xml.sax.ext



	''' <summary>
	''' SAX2 extension handler for DTD declaration events.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' See <a href='http://www.saxproject.org'>http://www.saxproject.org</a>
	''' for further information.
	''' </blockquote>
	''' 
	''' <p>This is an optional extension handler for SAX2 to provide more
	''' complete information about DTD declarations in an XML document.
	''' XML readers are not required to recognize this handler, and it
	''' is not part of core-only SAX2 distributions.</p>
	''' 
	''' <p>Note that data-related DTD declarations (unparsed entities and
	''' notations) are already reported through the {@link
	''' org.xml.sax.DTDHandler DTDHandler} interface.</p>
	''' 
	''' <p>If you are using the declaration handler together with a lexical
	''' handler, all of the events will occur between the
	''' <seealso cref="org.xml.sax.ext.LexicalHandler#startDTD startDTD"/> and the
	''' <seealso cref="org.xml.sax.ext.LexicalHandler#endDTD endDTD"/> events.</p>
	''' 
	''' <p>To set the DeclHandler for an XML reader, use the
	''' <seealso cref="org.xml.sax.XMLReader#setProperty setProperty"/> method
	''' with the property name
	''' <code>http://xml.org/sax/properties/declaration-handler</code>
	''' and an object implementing this interface (or null) as the value.
	''' If the reader does not report declaration events, it will throw a
	''' <seealso cref="org.xml.sax.SAXNotRecognizedException SAXNotRecognizedException"/>
	''' when you attempt to register the handler.</p>
	''' 
	''' @since SAX 2.0 (extensions 1.0)
	''' @author David Megginson
	''' </summary>
	Public Interface DeclHandler

		''' <summary>
		''' Report an element type declaration.
		''' 
		''' <p>The content model will consist of the string "EMPTY", the
		''' string "ANY", or a parenthesised group, optionally followed
		''' by an occurrence indicator.  The model will be normalized so
		''' that all parameter entities are fully resolved and all whitespace
		''' is removed,and will include the enclosing parentheses.  Other
		''' normalization (such as removing redundant parentheses or
		''' simplifying occurrence indicators) is at the discretion of the
		''' parser.</p>
		''' </summary>
		''' <param name="name"> The element type name. </param>
		''' <param name="model"> The content model as a normalized string. </param>
		''' <exception cref="SAXException"> The application may raise an exception. </exception>
		Sub elementDecl(ByVal name As String, ByVal model As String)


		''' <summary>
		''' Report an attribute type declaration.
		''' 
		''' <p>Only the effective (first) declaration for an attribute will
		''' be reported.  The type will be one of the strings "CDATA",
		''' "ID", "IDREF", "IDREFS", "NMTOKEN", "NMTOKENS", "ENTITY",
		''' "ENTITIES", a parenthesized token group with
		''' the separator "|" and all whitespace removed, or the word
		''' "NOTATION" followed by a space followed by a parenthesized
		''' token group with all whitespace removed.</p>
		''' 
		''' <p>The value will be the value as reported to applications,
		''' appropriately normalized and with entity and character
		''' references expanded.  </p>
		''' </summary>
		''' <param name="eName"> The name of the associated element. </param>
		''' <param name="aName"> The name of the attribute. </param>
		''' <param name="type"> A string representing the attribute type. </param>
		''' <param name="mode"> A string representing the attribute defaulting mode
		'''        ("#IMPLIED", "#REQUIRED", or "#FIXED") or null if
		'''        none of these applies. </param>
		''' <param name="value"> A string representing the attribute's default value,
		'''        or null if there is none. </param>
		''' <exception cref="SAXException"> The application may raise an exception. </exception>
		Sub attributeDecl(ByVal eName As String, ByVal aName As String, ByVal type As String, ByVal mode As String, ByVal value As String)


		''' <summary>
		''' Report an internal entity declaration.
		''' 
		''' <p>Only the effective (first) declaration for each entity
		''' will be reported.  All parameter entities in the value
		''' will be expanded, but general entities will not.</p>
		''' </summary>
		''' <param name="name"> The name of the entity.  If it is a parameter
		'''        entity, the name will begin with '%'. </param>
		''' <param name="value"> The replacement text of the entity. </param>
		''' <exception cref="SAXException"> The application may raise an exception. </exception>
		''' <seealso cref= #externalEntityDecl </seealso>
		''' <seealso cref= org.xml.sax.DTDHandler#unparsedEntityDecl </seealso>
		Sub internalEntityDecl(ByVal name As String, ByVal value As String)


		''' <summary>
		''' Report a parsed external entity declaration.
		''' 
		''' <p>Only the effective (first) declaration for each entity
		''' will be reported.</p>
		''' 
		''' <p>If the system identifier is a URL, the parser must resolve it
		''' fully before passing it to the application.</p>
		''' </summary>
		''' <param name="name"> The name of the entity.  If it is a parameter
		'''        entity, the name will begin with '%'. </param>
		''' <param name="publicId"> The entity's public identifier, or null if none
		'''        was given. </param>
		''' <param name="systemId"> The entity's system identifier. </param>
		''' <exception cref="SAXException"> The application may raise an exception. </exception>
		''' <seealso cref= #internalEntityDecl </seealso>
		''' <seealso cref= org.xml.sax.DTDHandler#unparsedEntityDecl </seealso>
		Sub externalEntityDecl(ByVal name As String, ByVal publicId As String, ByVal systemId As String)

	End Interface

	' end of DeclHandler.java

End Namespace
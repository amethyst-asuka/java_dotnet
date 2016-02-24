'
' * Copyright (c) 2004, 2005, Oracle and/or its affiliates. All rights reserved.
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

' DefaultHandler2.java - extended DefaultHandler
' http://www.saxproject.org
' Public Domain: no warranty.
' $Id: DefaultHandler2.java,v 1.2 2004/11/03 22:49:08 jsuttor Exp $

Namespace org.xml.sax.ext



	''' <summary>
	''' This class extends the SAX2 base handler class to support the
	''' SAX2 <seealso cref="LexicalHandler"/>, <seealso cref="DeclHandler"/>, and
	''' <seealso cref="EntityResolver2"/> extensions.  Except for overriding the
	''' original SAX1 <seealso cref="DefaultHandler#resolveEntity resolveEntity()"/>
	''' method the added handler methods just return.  Subclassers may
	''' override everything on a method-by-method basis.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' </blockquote>
	''' 
	''' <p> <em>Note:</em> this class might yet learn that the
	''' <em>ContentHandler.setDocumentLocator()</em> call might be passed a
	''' <seealso cref="Locator2"/> object, and that the
	''' <em>ContentHandler.startElement()</em> call might be passed a
	''' <seealso cref="Attributes2"/> object.
	''' 
	''' @since SAX 2.0 (extensions 1.1 alpha)
	''' @author David Brownell
	''' </summary>
	Public Class DefaultHandler2
		Inherits org.xml.sax.helpers.DefaultHandler
		Implements LexicalHandler, DeclHandler, EntityResolver2

		''' <summary>
		''' Constructs a handler which ignores all parsing events. </summary>
		Public Sub New()
		End Sub


		' SAX2 ext-1.0 LexicalHandler

		Public Overridable Sub startCDATA() Implements LexicalHandler.startCDATA
		End Sub

		Public Overridable Sub endCDATA() Implements LexicalHandler.endCDATA
		End Sub

		Public Overridable Sub startDTD(ByVal name As String, ByVal publicId As String, ByVal systemId As String) Implements LexicalHandler.startDTD
		End Sub

		Public Overridable Sub endDTD() Implements LexicalHandler.endDTD
		End Sub

		Public Overridable Sub startEntity(ByVal name As String) Implements LexicalHandler.startEntity
		End Sub

		Public Overridable Sub endEntity(ByVal name As String) Implements LexicalHandler.endEntity
		End Sub

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public void comment(char ch () , int start, int length) throws org.xml.sax.SAXException


		' SAX2 ext-1.0 DeclHandler

		public void attributeDecl(String eName, String aName, String type, String mode, String value) throws org.xml.sax.SAXException

		public void elementDecl(String name, String model) throws org.xml.sax.SAXException

		public void externalEntityDecl(String name, String publicId, String systemId) throws org.xml.sax.SAXException

		public void internalEntityDecl(String name, String value) throws org.xml.sax.SAXException

		' SAX2 ext-1.1 EntityResolver2

		''' <summary>
		''' Tells the parser that if no external subset has been declared
		''' in the document text, none should be used.
		''' </summary>
		public org.xml.sax.InputSource getExternalSubset(String name, String baseURI) throws org.xml.sax.SAXException, java.io.IOException
				Return Nothing

		''' <summary>
		''' Tells the parser to resolve the systemId against the baseURI
		''' and read the entity text from that resulting absolute URI.
		''' Note that because the older
		''' <seealso cref="DefaultHandler#resolveEntity DefaultHandler.resolveEntity()"/>,
		''' method is overridden to call this one, this method may sometimes
		''' be invoked with null <em>name</em> and <em>baseURI</em>, and
		''' with the <em>systemId</em> already absolutized.
		''' </summary>
		public org.xml.sax.InputSource resolveEntity(String name, String publicId, String baseURI, String systemId) throws org.xml.sax.SAXException, java.io.IOException
				Return Nothing

		' SAX1 EntityResolver

		''' <summary>
		''' Invokes
		''' <seealso cref="EntityResolver2#resolveEntity EntityResolver2.resolveEntity()"/>
		''' with null entity name and base URI.
		''' You only need to override that method to use this class.
		''' </summary>
		public org.xml.sax.InputSource resolveEntity(String publicId, String systemId) throws org.xml.sax.SAXException, java.io.IOException
				Return resolveEntity(Nothing, publicId, Nothing, systemId)
	End Class

End Namespace
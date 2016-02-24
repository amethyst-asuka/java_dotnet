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

' Attributes.java - attribute list with Namespace support
' http://www.saxproject.org
' Written by David Megginson
' NO WARRANTY!  This class is in the public domain.
' $Id: Attributes.java,v 1.2 2004/11/03 22:44:51 jsuttor Exp $

Namespace org.xml.sax


	''' <summary>
	''' Interface for a list of XML attributes.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' See <a href='http://www.saxproject.org'>http://www.saxproject.org</a>
	''' for further information.
	''' </blockquote>
	''' 
	''' <p>This interface allows access to a list of attributes in
	''' three different ways:</p>
	''' 
	''' <ol>
	''' <li>by attribute index;</li>
	''' <li>by Namespace-qualified name; or</li>
	''' <li>by qualified (prefixed) name.</li>
	''' </ol>
	''' 
	''' <p>The list will not contain attributes that were declared
	''' #IMPLIED but not specified in the start tag.  It will also not
	''' contain attributes used as Namespace declarations (xmlns*) unless
	''' the <code>http://xml.org/sax/features/namespace-prefixes</code>
	''' feature is set to <var>true</var> (it is <var>false</var> by
	''' default).
	''' Because SAX2 conforms to the original "Namespaces in XML"
	''' recommendation, it normally does not
	''' give namespace declaration attributes a namespace URI.
	''' </p>
	''' 
	''' <p>Some SAX2 parsers may support using an optional feature flag
	''' (<code>http://xml.org/sax/features/xmlns-uris</code>) to request
	''' that those attributes be given URIs, conforming to a later
	''' backwards-incompatible revision of that recommendation.  (The
	''' attribute's "local name" will be the prefix, or "xmlns" when
	''' defining a default element namespace.)  For portability, handler
	''' code should always resolve that conflict, rather than requiring
	''' parsers that can change the setting of that feature flag.  </p>
	''' 
	''' <p>If the namespace-prefixes feature (see above) is
	''' <var>false</var>, access by qualified name may not be available; if
	''' the <code>http://xml.org/sax/features/namespaces</code> feature is
	''' <var>false</var>, access by Namespace-qualified names may not be
	''' available.</p>
	''' 
	''' <p>This interface replaces the now-deprecated SAX1 {@link
	''' org.xml.sax.AttributeList AttributeList} interface, which does not
	''' contain Namespace support.  In addition to Namespace support, it
	''' adds the <var>getIndex</var> methods (below).</p>
	''' 
	''' <p>The order of attributes in the list is unspecified, and will
	''' vary from implementation to implementation.</p>
	''' 
	''' @since SAX 2.0
	''' @author David Megginson </summary>
	''' <seealso cref= org.xml.sax.helpers.AttributesImpl </seealso>
	''' <seealso cref= org.xml.sax.ext.DeclHandler#attributeDecl </seealso>
	Public Interface Attributes


		'//////////////////////////////////////////////////////////////////
		' Indexed access.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Return the number of attributes in the list.
		''' 
		''' <p>Once you know the number of attributes, you can iterate
		''' through the list.</p>
		''' </summary>
		''' <returns> The number of attributes in the list. </returns>
		''' <seealso cref= #getURI(int) </seealso>
		''' <seealso cref= #getLocalName(int) </seealso>
		''' <seealso cref= #getQName(int) </seealso>
		''' <seealso cref= #getType(int) </seealso>
		''' <seealso cref= #getValue(int) </seealso>
		ReadOnly Property length As Integer


		''' <summary>
		''' Look up an attribute's Namespace URI by index.
		''' </summary>
		''' <param name="index"> The attribute index (zero-based). </param>
		''' <returns> The Namespace URI, or the empty string if none
		'''         is available, or null if the index is out of
		'''         range. </returns>
		''' <seealso cref= #getLength </seealso>
		Function getURI(ByVal index As Integer) As String


		''' <summary>
		''' Look up an attribute's local name by index.
		''' </summary>
		''' <param name="index"> The attribute index (zero-based). </param>
		''' <returns> The local name, or the empty string if Namespace
		'''         processing is not being performed, or null
		'''         if the index is out of range. </returns>
		''' <seealso cref= #getLength </seealso>
		Function getLocalName(ByVal index As Integer) As String


		''' <summary>
		''' Look up an attribute's XML qualified (prefixed) name by index.
		''' </summary>
		''' <param name="index"> The attribute index (zero-based). </param>
		''' <returns> The XML qualified name, or the empty string
		'''         if none is available, or null if the index
		'''         is out of range. </returns>
		''' <seealso cref= #getLength </seealso>
		Function getQName(ByVal index As Integer) As String


		''' <summary>
		''' Look up an attribute's type by index.
		''' 
		''' <p>The attribute type is one of the strings "CDATA", "ID",
		''' "IDREF", "IDREFS", "NMTOKEN", "NMTOKENS", "ENTITY", "ENTITIES",
		''' or "NOTATION" (always in upper case).</p>
		''' 
		''' <p>If the parser has not read a declaration for the attribute,
		''' or if the parser does not report attribute types, then it must
		''' return the value "CDATA" as stated in the XML 1.0 Recommendation
		''' (clause 3.3.3, "Attribute-Value Normalization").</p>
		''' 
		''' <p>For an enumerated attribute that is not a notation, the
		''' parser will report the type as "NMTOKEN".</p>
		''' </summary>
		''' <param name="index"> The attribute index (zero-based). </param>
		''' <returns> The attribute's type as a string, or null if the
		'''         index is out of range. </returns>
		''' <seealso cref= #getLength </seealso>
		Function [getType](ByVal index As Integer) As String


		''' <summary>
		''' Look up an attribute's value by index.
		''' 
		''' <p>If the attribute value is a list of tokens (IDREFS,
		''' ENTITIES, or NMTOKENS), the tokens will be concatenated
		''' into a single string with each token separated by a
		''' single space.</p>
		''' </summary>
		''' <param name="index"> The attribute index (zero-based). </param>
		''' <returns> The attribute's value as a string, or null if the
		'''         index is out of range. </returns>
		''' <seealso cref= #getLength </seealso>
		Function getValue(ByVal index As Integer) As String



		'//////////////////////////////////////////////////////////////////
		' Name-based query.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Look up the index of an attribute by Namespace name.
		''' </summary>
		''' <param name="uri"> The Namespace URI, or the empty string if
		'''        the name has no Namespace URI. </param>
		''' <param name="localName"> The attribute's local name. </param>
		''' <returns> The index of the attribute, or -1 if it does not
		'''         appear in the list. </returns>
		Function getIndex(ByVal uri As String, ByVal localName As String) As Integer


		''' <summary>
		''' Look up the index of an attribute by XML qualified (prefixed) name.
		''' </summary>
		''' <param name="qName"> The qualified (prefixed) name. </param>
		''' <returns> The index of the attribute, or -1 if it does not
		'''         appear in the list. </returns>
		Function getIndex(ByVal qName As String) As Integer


		''' <summary>
		''' Look up an attribute's type by Namespace name.
		''' 
		''' <p>See <seealso cref="#getType(int) getType(int)"/> for a description
		''' of the possible types.</p>
		''' </summary>
		''' <param name="uri"> The Namespace URI, or the empty String if the
		'''        name has no Namespace URI. </param>
		''' <param name="localName"> The local name of the attribute. </param>
		''' <returns> The attribute type as a string, or null if the
		'''         attribute is not in the list or if Namespace
		'''         processing is not being performed. </returns>
		Function [getType](ByVal uri As String, ByVal localName As String) As String


		''' <summary>
		''' Look up an attribute's type by XML qualified (prefixed) name.
		''' 
		''' <p>See <seealso cref="#getType(int) getType(int)"/> for a description
		''' of the possible types.</p>
		''' </summary>
		''' <param name="qName"> The XML qualified name. </param>
		''' <returns> The attribute type as a string, or null if the
		'''         attribute is not in the list or if qualified names
		'''         are not available. </returns>
		Function [getType](ByVal qName As String) As String


		''' <summary>
		''' Look up an attribute's value by Namespace name.
		''' 
		''' <p>See <seealso cref="#getValue(int) getValue(int)"/> for a description
		''' of the possible values.</p>
		''' </summary>
		''' <param name="uri"> The Namespace URI, or the empty String if the
		'''        name has no Namespace URI. </param>
		''' <param name="localName"> The local name of the attribute. </param>
		''' <returns> The attribute value as a string, or null if the
		'''         attribute is not in the list. </returns>
		Function getValue(ByVal uri As String, ByVal localName As String) As String


		''' <summary>
		''' Look up an attribute's value by XML qualified (prefixed) name.
		''' 
		''' <p>See <seealso cref="#getValue(int) getValue(int)"/> for a description
		''' of the possible values.</p>
		''' </summary>
		''' <param name="qName"> The XML qualified name. </param>
		''' <returns> The attribute value as a string, or null if the
		'''         attribute is not in the list or if qualified names
		'''         are not available. </returns>
		Function getValue(ByVal qName As String) As String

	End Interface

	' end of Attributes.java

End Namespace
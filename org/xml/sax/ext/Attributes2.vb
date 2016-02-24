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

' Attributes2.java - extended Attributes
' http://www.saxproject.org
' Public Domain: no warranty.
' $Id: Attributes2.java,v 1.2 2004/11/03 22:49:07 jsuttor Exp $

Namespace org.xml.sax.ext



	''' <summary>
	''' SAX2 extension to augment the per-attribute information
	''' provided though <seealso cref="Attributes"/>.
	''' If an implementation supports this extension, the attributes
	''' provided in {@link org.xml.sax.ContentHandler#startElement
	''' ContentHandler.startElement() } will implement this interface,
	''' and the <em>http://xml.org/sax/features/use-attributes2</em>
	''' feature flag will have the value <em>true</em>.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' </blockquote>
	''' 
	''' <p> XMLReader implementations are not required to support this
	''' information, and it is not part of core-only SAX2 distributions.</p>
	''' 
	''' <p>Note that if an attribute was defaulted (<em>!isSpecified()</em>)
	''' it will of necessity also have been declared (<em>isDeclared()</em>)
	''' in the DTD.
	''' Similarly if an attribute's type is anything except CDATA, then it
	''' must have been declared.
	''' </p>
	''' 
	''' @since SAX 2.0 (extensions 1.1 alpha)
	''' @author David Brownell
	''' </summary>
	Public Interface Attributes2
		Inherits org.xml.sax.Attributes

		''' <summary>
		''' Returns false unless the attribute was declared in the DTD.
		''' This helps distinguish two kinds of attributes that SAX reports
		''' as CDATA:  ones that were declared (and hence are usually valid),
		''' and those that were not (and which are never valid).
		''' </summary>
		''' <param name="index"> The attribute index (zero-based). </param>
		''' <returns> true if the attribute was declared in the DTD,
		'''          false otherwise. </returns>
		''' <exception cref="java.lang.ArrayIndexOutOfBoundsException"> When the
		'''            supplied index does not identify an attribute. </exception>
		Function isDeclared(ByVal index As Integer) As Boolean

		''' <summary>
		''' Returns false unless the attribute was declared in the DTD.
		''' This helps distinguish two kinds of attributes that SAX reports
		''' as CDATA:  ones that were declared (and hence are usually valid),
		''' and those that were not (and which are never valid).
		''' </summary>
		''' <param name="qName"> The XML qualified (prefixed) name. </param>
		''' <returns> true if the attribute was declared in the DTD,
		'''          false otherwise. </returns>
		''' <exception cref="java.lang.IllegalArgumentException"> When the
		'''            supplied name does not identify an attribute. </exception>
		Function isDeclared(ByVal qName As String) As Boolean

		''' <summary>
		''' Returns false unless the attribute was declared in the DTD.
		''' This helps distinguish two kinds of attributes that SAX reports
		''' as CDATA:  ones that were declared (and hence are usually valid),
		''' and those that were not (and which are never valid).
		''' 
		''' <p>Remember that since DTDs do not "understand" namespaces, the
		''' namespace URI associated with an attribute may not have come from
		''' the DTD.  The declaration will have applied to the attribute's
		''' <em>qName</em>.
		''' </summary>
		''' <param name="uri"> The Namespace URI, or the empty string if
		'''        the name has no Namespace URI. </param>
		''' <param name="localName"> The attribute's local name. </param>
		''' <returns> true if the attribute was declared in the DTD,
		'''          false otherwise. </returns>
		''' <exception cref="java.lang.IllegalArgumentException"> When the
		'''            supplied names do not identify an attribute. </exception>
		Function isDeclared(ByVal uri As String, ByVal localName As String) As Boolean

		''' <summary>
		''' Returns true unless the attribute value was provided
		''' by DTD defaulting.
		''' </summary>
		''' <param name="index"> The attribute index (zero-based). </param>
		''' <returns> true if the value was found in the XML text,
		'''          false if the value was provided by DTD defaulting. </returns>
		''' <exception cref="java.lang.ArrayIndexOutOfBoundsException"> When the
		'''            supplied index does not identify an attribute. </exception>
		Function isSpecified(ByVal index As Integer) As Boolean

		''' <summary>
		''' Returns true unless the attribute value was provided
		''' by DTD defaulting.
		''' 
		''' <p>Remember that since DTDs do not "understand" namespaces, the
		''' namespace URI associated with an attribute may not have come from
		''' the DTD.  The declaration will have applied to the attribute's
		''' <em>qName</em>.
		''' </summary>
		''' <param name="uri"> The Namespace URI, or the empty string if
		'''        the name has no Namespace URI. </param>
		''' <param name="localName"> The attribute's local name. </param>
		''' <returns> true if the value was found in the XML text,
		'''          false if the value was provided by DTD defaulting. </returns>
		''' <exception cref="java.lang.IllegalArgumentException"> When the
		'''            supplied names do not identify an attribute. </exception>
		Function isSpecified(ByVal uri As String, ByVal localName As String) As Boolean

		''' <summary>
		''' Returns true unless the attribute value was provided
		''' by DTD defaulting.
		''' </summary>
		''' <param name="qName"> The XML qualified (prefixed) name. </param>
		''' <returns> true if the value was found in the XML text,
		'''          false if the value was provided by DTD defaulting. </returns>
		''' <exception cref="java.lang.IllegalArgumentException"> When the
		'''            supplied name does not identify an attribute. </exception>
		Function isSpecified(ByVal qName As String) As Boolean
	End Interface

End Namespace
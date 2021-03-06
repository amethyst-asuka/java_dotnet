Imports System.Collections

'
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

'
' * Copyright (c) 2009 by Oracle Corporation. All Rights Reserved.
' 

Namespace javax.xml.stream.events



	''' <summary>
	''' The StartElement interface provides access to information about
	''' start elements.  A StartElement is reported for each Start Tag
	''' in the document.
	''' 
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved.
	''' @since 1.6
	''' </summary>
	Public Interface StartElement
		Inherits XMLEvent

	  ''' <summary>
	  ''' Get the name of this event </summary>
	  ''' <returns> the qualified name of this event </returns>
	  ReadOnly Property name As javax.xml.namespace.QName

	  ''' <summary>
	  ''' Returns an Iterator of non-namespace declared attributes declared on
	  ''' this START_ELEMENT,
	  ''' returns an empty iterator if there are no attributes.  The
	  ''' iterator must contain only implementations of the javax.xml.stream.Attribute
	  ''' interface.   Attributes are fundamentally unordered and may not be reported
	  ''' in any order.
	  ''' </summary>
	  ''' <returns> a readonly Iterator over Attribute interfaces, or an
	  ''' empty iterator </returns>
	  ReadOnly Property attributes As IEnumerator

	  ''' <summary>
	  ''' Returns an Iterator of namespaces declared on this element.
	  ''' This Iterator does not contain previously declared namespaces
	  ''' unless they appear on the current START_ELEMENT.
	  ''' Therefore this list may contain redeclared namespaces and duplicate namespace
	  ''' declarations. Use the getNamespaceContext() method to get the
	  ''' current context of namespace declarations.
	  ''' 
	  ''' <p>The iterator must contain only implementations of the
	  ''' javax.xml.stream.Namespace interface.
	  ''' 
	  ''' <p>A Namespace isA Attribute.  One
	  ''' can iterate over a list of namespaces as a list of attributes.
	  ''' However this method returns only the list of namespaces
	  ''' declared on this START_ELEMENT and does not
	  ''' include the attributes declared on this START_ELEMENT.
	  ''' 
	  ''' Returns an empty iterator if there are no namespaces.
	  ''' </summary>
	  ''' <returns> a readonly Iterator over Namespace interfaces, or an
	  ''' empty iterator
	  '''  </returns>
	  ReadOnly Property namespaces As IEnumerator

	  ''' <summary>
	  ''' Returns the attribute referred to by this name </summary>
	  ''' <param name="name"> the qname of the desired name </param>
	  ''' <returns> the attribute corresponding to the name value or null </returns>
	  Function getAttributeByName(ByVal name As javax.xml.namespace.QName) As Attribute

	  ''' <summary>
	  ''' Gets a read-only namespace context. If no context is
	  ''' available this method will return an empty namespace context.
	  ''' The NamespaceContext contains information about all namespaces
	  ''' in scope for this StartElement.
	  ''' </summary>
	  ''' <returns> the current namespace context </returns>
	  ReadOnly Property namespaceContext As javax.xml.namespace.NamespaceContext

	  ''' <summary>
	  ''' Gets the value that the prefix is bound to in the
	  ''' context of this element.  Returns null if
	  ''' the prefix is not bound in this context </summary>
	  ''' <param name="prefix"> the prefix to lookup </param>
	  ''' <returns> the uri bound to the prefix or null </returns>
	  Function getNamespaceURI(ByVal prefix As String) As String
	End Interface

End Namespace
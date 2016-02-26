'
' * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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
' * $Id: XMLCryptoContext.java,v 1.6 2005/05/10 15:47:42 mullan Exp $
' 
Namespace javax.xml.crypto

	''' <summary>
	''' Contains common context information for XML cryptographic operations.
	''' 
	''' <p>This interface contains methods for setting and retrieving properties
	''' that affect the processing of XML signatures or XML encrypted structures.
	''' 
	''' <p>Note that <code>XMLCryptoContext</code> instances can contain information
	''' and state specific to the XML cryptographic structure it is used with.
	''' The results are unpredictable if an <code>XMLCryptoContext</code> is
	''' used with multiple structures (for example, you should not use the same
	''' <seealso cref="javax.xml.crypto.dsig.XMLValidateContext"/> instance to validate two
	''' different <seealso cref="javax.xml.crypto.dsig.XMLSignature"/> objects).
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6
	''' </summary>
	Public Interface XMLCryptoContext

		''' <summary>
		''' Returns the base URI.
		''' </summary>
		''' <returns> the base URI, or <code>null</code> if not specified </returns>
		''' <seealso cref= #setBaseURI(String) </seealso>
		Property baseURI As String


		''' <summary>
		''' Returns the key selector for finding a key.
		''' </summary>
		''' <returns> the key selector, or <code>null</code> if not specified </returns>
		''' <seealso cref= #setKeySelector(KeySelector) </seealso>
		Property keySelector As KeySelector


		''' <summary>
		''' Returns a <code>URIDereferencer</code> that is used to dereference
		''' <seealso cref="URIReference"/>s.
		''' </summary>
		''' <returns> the <code>URIDereferencer</code>, or <code>null</code> if not
		'''    specified </returns>
		''' <seealso cref= #setURIDereferencer(URIDereferencer) </seealso>
		Property uRIDereferencer As URIDereferencer


		''' <summary>
		''' Returns the namespace prefix that the specified namespace URI is
		''' associated with. Returns the specified default prefix if the specified
		''' namespace URI has not been bound to a prefix. To bind a namespace URI
		''' to a prefix, call the <seealso cref="#putNamespacePrefix putNamespacePrefix"/>
		''' method.
		''' </summary>
		''' <param name="namespaceURI"> a namespace URI </param>
		''' <param name="defaultPrefix"> the prefix to be returned in the event that the
		'''    the specified namespace URI has not been bound to a prefix. </param>
		''' <returns> the prefix that is associated with the specified namespace URI,
		'''    or <code>defaultPrefix</code> if the URI is not registered. If
		'''    the namespace URI is registered but has no prefix, an empty string
		'''    (<code>""</code>) is returned. </returns>
		''' <exception cref="NullPointerException"> if <code>namespaceURI</code> is
		'''    <code>null</code> </exception>
		''' <seealso cref= #putNamespacePrefix(String, String) </seealso>
		Function getNamespacePrefix(ByVal namespaceURI As String, ByVal defaultPrefix As String) As String

		''' <summary>
		''' Maps the specified namespace URI to the specified prefix. If there is
		''' already a prefix associated with the specified namespace URI, the old
		''' prefix is replaced by the specified prefix.
		''' </summary>
		''' <param name="namespaceURI"> a namespace URI </param>
		''' <param name="prefix"> a namespace prefix (or <code>null</code> to remove any
		'''    existing mapping). Specifying the empty string (<code>""</code>)
		'''    binds no prefix to the namespace URI. </param>
		''' <returns> the previous prefix associated with the specified namespace
		'''    URI, or <code>null</code> if there was none </returns>
		''' <exception cref="NullPointerException"> if <code>namespaceURI</code> is
		'''    <code>null</code> </exception>
		''' <seealso cref= #getNamespacePrefix(String, String) </seealso>
		Function putNamespacePrefix(ByVal namespaceURI As String, ByVal prefix As String) As String

		''' <summary>
		''' Returns the default namespace prefix. The default namespace prefix
		''' is the prefix for all namespace URIs not explicitly set by the
		''' <seealso cref="#putNamespacePrefix putNamespacePrefix"/> method.
		''' </summary>
		''' <returns> the default namespace prefix, or <code>null</code> if none has
		'''    been set. </returns>
		''' <seealso cref= #setDefaultNamespacePrefix(String) </seealso>
		Property defaultNamespacePrefix As String


		''' <summary>
		''' Sets the specified property.
		''' </summary>
		''' <param name="name"> the name of the property </param>
		''' <param name="value"> the value of the property to be set </param>
		''' <returns> the previous value of the specified property, or
		'''    <code>null</code> if it did not have a value </returns>
		''' <exception cref="NullPointerException"> if <code>name</code> is <code>null</code> </exception>
		''' <seealso cref= #getProperty(String) </seealso>
		Function setProperty(ByVal name As String, ByVal value As Object) As Object

		''' <summary>
		''' Returns the value of the specified property.
		''' </summary>
		''' <param name="name"> the name of the property </param>
		''' <returns> the current value of the specified property, or
		'''    <code>null</code> if it does not have a value </returns>
		''' <exception cref="NullPointerException"> if <code>name</code> is <code>null</code> </exception>
		''' <seealso cref= #setProperty(String, Object) </seealso>
		Function getProperty(ByVal name As String) As Object

		''' <summary>
		''' Returns the value to which this context maps the specified key.
		''' 
		''' <p>More formally, if this context contains a mapping from a key
		''' <code>k</code> to a value <code>v</code> such that
		''' <code>(key==null ? k==null : key.equals(k))</code>, then this method
		''' returns <code>v</code>; otherwise it returns <code>null</code>. (There
		''' can be at most one such mapping.)
		''' 
		''' <p>This method is useful for retrieving arbitrary information that is
		''' specific to the cryptographic operation that this context is used for.
		''' </summary>
		''' <param name="key"> the key whose associated value is to be returned </param>
		''' <returns> the value to which this context maps the specified key, or
		'''    <code>null</code> if there is no mapping for the key </returns>
		''' <seealso cref= #put(Object, Object) </seealso>
		Function [get](ByVal key As Object) As Object

		''' <summary>
		''' Associates the specified value with the specified key in this context.
		''' If the context previously contained a mapping for this key, the old
		''' value is replaced by the specified value.
		''' 
		''' <p>This method is useful for storing arbitrary information that is
		''' specific to the cryptographic operation that this context is used for.
		''' </summary>
		''' <param name="key"> key with which the specified value is to be associated with </param>
		''' <param name="value"> value to be associated with the specified key </param>
		''' <returns> the previous value associated with the key, or <code>null</code>
		'''    if there was no mapping for the key </returns>
		''' <exception cref="IllegalArgumentException"> if some aspect of this key or value
		'''    prevents it from being stored in this context </exception>
		''' <seealso cref= #get(Object) </seealso>
		Function put(ByVal key As Object, ByVal value As Object) As Object
	End Interface

End Namespace
'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
' * $Id: DOMValidateContext.java,v 1.8 2005/05/10 16:31:14 mullan Exp $
' 
Namespace javax.xml.crypto.dsig.dom


	''' <summary>
	''' A DOM-specific <seealso cref="XMLValidateContext"/>. This class contains additional
	''' methods to specify the location in a DOM tree where an <seealso cref="XMLSignature"/>
	''' is to be unmarshalled and validated from.
	''' 
	''' <p>Note that the behavior of an unmarshalled <code>XMLSignature</code>
	''' is undefined if the contents of the underlying DOM tree are modified by the
	''' caller after the <code>XMLSignature</code> is created.
	''' 
	''' <p>Also, note that <code>DOMValidateContext</code> instances can contain
	''' information and state specific to the XML signature structure it is
	''' used with. The results are unpredictable if a
	''' <code>DOMValidateContext</code> is used with different signature structures
	''' (for example, you should not use the same <code>DOMValidateContext</code>
	''' instance to validate two different <seealso cref="XMLSignature"/> objects).
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6 </summary>
	''' <seealso cref= XMLSignatureFactory#unmarshalXMLSignature(XMLValidateContext) </seealso>
	Public Class DOMValidateContext
		Inherits javax.xml.crypto.dom.DOMCryptoContext
		Implements javax.xml.crypto.dsig.XMLValidateContext

		Private node As org.w3c.dom.Node

		''' <summary>
		''' Creates a <code>DOMValidateContext</code> containing the specified key
		''' selector and node.
		''' </summary>
		''' <param name="ks"> a key selector for finding a validation key </param>
		''' <param name="node"> the node </param>
		''' <exception cref="NullPointerException"> if <code>ks</code> or <code>node</code> is
		'''    <code>null</code> </exception>
		Public Sub New(ByVal ks As javax.xml.crypto.KeySelector, ByVal node As org.w3c.dom.Node)
			If ks Is Nothing Then Throw New NullPointerException("key selector is null")
			init(node, ks)
		End Sub

		''' <summary>
		''' Creates a <code>DOMValidateContext</code> containing the specified key
		''' and node. The validating key will be stored in a
		''' <seealso cref="KeySelector#singletonKeySelector singleton KeySelector"/> that
		''' is returned when the <seealso cref="#getKeySelector getKeySelector"/>
		''' method is called.
		''' </summary>
		''' <param name="validatingKey"> the validating key </param>
		''' <param name="node"> the node </param>
		''' <exception cref="NullPointerException"> if <code>validatingKey</code> or
		'''    <code>node</code> is <code>null</code> </exception>
		Public Sub New(ByVal validatingKey As java.security.Key, ByVal node As org.w3c.dom.Node)
			If validatingKey Is Nothing Then Throw New NullPointerException("validatingKey is null")
			init(node, javax.xml.crypto.KeySelector.singletonKeySelector(validatingKey))
		End Sub

		Private Sub init(ByVal node As org.w3c.dom.Node, ByVal ks As javax.xml.crypto.KeySelector)
			If node Is Nothing Then Throw New NullPointerException("node is null")

			Me.node = node
			MyBase.keySelector = ks
			If System.securityManager IsNot Nothing Then MyBase.propertyrty("org.jcp.xml.dsig.secureValidation", Boolean.TRUE)
		End Sub

		''' <summary>
		''' Sets the node.
		''' </summary>
		''' <param name="node"> the node </param>
		''' <exception cref="NullPointerException"> if <code>node</code> is <code>null</code> </exception>
		''' <seealso cref= #getNode </seealso>
		Public Overridable Property node As org.w3c.dom.Node
			Set(ByVal node As org.w3c.dom.Node)
				If node Is Nothing Then Throw New NullPointerException
				Me.node = node
			End Set
			Get
				Return node
			End Get
		End Property

	End Class

End Namespace
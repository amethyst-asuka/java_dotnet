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
' * $Id: DOMSignContext.java,v 1.9 2005/05/10 16:31:14 mullan Exp $
' 
Namespace javax.xml.crypto.dsig.dom


	''' <summary>
	''' A DOM-specific <seealso cref="XMLSignContext"/>. This class contains additional methods
	''' to specify the location in a DOM tree where an <seealso cref="XMLSignature"/>
	''' object is to be marshalled when generating the signature.
	''' 
	''' <p>Note that <code>DOMSignContext</code> instances can contain
	''' information and state specific to the XML signature structure it is
	''' used with. The results are unpredictable if a
	''' <code>DOMSignContext</code> is used with different signature structures
	''' (for example, you should not use the same <code>DOMSignContext</code>
	''' instance to sign two different <seealso cref="XMLSignature"/> objects).
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6
	''' </summary>
	Public Class DOMSignContext
		Inherits javax.xml.crypto.dom.DOMCryptoContext
		Implements javax.xml.crypto.dsig.XMLSignContext

		Private parent As org.w3c.dom.Node
		Private nextSibling As org.w3c.dom.Node

		''' <summary>
		''' Creates a <code>DOMSignContext</code> with the specified signing key
		''' and parent node. The signing key is stored in a
		''' <seealso cref="KeySelector#singletonKeySelector singleton KeySelector"/> that is
		''' returned by the <seealso cref="#getKeySelector getKeySelector"/> method.
		''' The marshalled <code>XMLSignature</code> will be added as the last
		''' child element of the specified parent node unless a next sibling node is
		''' specified by invoking the <seealso cref="#setNextSibling setNextSibling"/> method.
		''' </summary>
		''' <param name="signingKey"> the signing key </param>
		''' <param name="parent"> the parent node </param>
		''' <exception cref="NullPointerException"> if <code>signingKey</code> or
		'''    <code>parent</code> is <code>null</code> </exception>
		Public Sub New(ByVal signingKey As java.security.Key, ByVal parent As org.w3c.dom.Node)
			If signingKey Is Nothing Then Throw New NullPointerException("signingKey cannot be null")
			If parent Is Nothing Then Throw New NullPointerException("parent cannot be null")
			keySelector = javax.xml.crypto.KeySelector.singletonKeySelector(signingKey)
			Me.parent = parent
		End Sub

		''' <summary>
		''' Creates a <code>DOMSignContext</code> with the specified signing key,
		''' parent and next sibling nodes. The signing key is stored in a
		''' <seealso cref="KeySelector#singletonKeySelector singleton KeySelector"/> that is
		''' returned by the <seealso cref="#getKeySelector getKeySelector"/> method.
		''' The marshalled <code>XMLSignature</code> will be inserted as a child
		''' element of the specified parent node and immediately before the
		''' specified next sibling node.
		''' </summary>
		''' <param name="signingKey"> the signing key </param>
		''' <param name="parent"> the parent node </param>
		''' <param name="nextSibling"> the next sibling node </param>
		''' <exception cref="NullPointerException"> if <code>signingKey</code>,
		'''    <code>parent</code> or <code>nextSibling</code> is <code>null</code> </exception>
		Public Sub New(ByVal signingKey As java.security.Key, ByVal parent As org.w3c.dom.Node, ByVal nextSibling As org.w3c.dom.Node)
			If signingKey Is Nothing Then Throw New NullPointerException("signingKey cannot be null")
			If parent Is Nothing Then Throw New NullPointerException("parent cannot be null")
			If nextSibling Is Nothing Then Throw New NullPointerException("nextSibling cannot be null")
			keySelector = javax.xml.crypto.KeySelector.singletonKeySelector(signingKey)
			Me.parent = parent
			Me.nextSibling = nextSibling
		End Sub

		''' <summary>
		''' Creates a <code>DOMSignContext</code> with the specified key selector
		''' and parent node. The marshalled <code>XMLSignature</code> will be added
		''' as the last child element of the specified parent node unless a next
		''' sibling node is specified by invoking the
		''' <seealso cref="#setNextSibling setNextSibling"/> method.
		''' </summary>
		''' <param name="ks"> the key selector </param>
		''' <param name="parent"> the parent node </param>
		''' <exception cref="NullPointerException"> if <code>ks</code> or <code>parent</code>
		'''    is <code>null</code> </exception>
		Public Sub New(ByVal ks As javax.xml.crypto.KeySelector, ByVal parent As org.w3c.dom.Node)
			If ks Is Nothing Then Throw New NullPointerException("key selector cannot be null")
			If parent Is Nothing Then Throw New NullPointerException("parent cannot be null")
			keySelector = ks
			Me.parent = parent
		End Sub

		''' <summary>
		''' Creates a <code>DOMSignContext</code> with the specified key selector,
		''' parent and next sibling nodes. The marshalled <code>XMLSignature</code>
		''' will be inserted as a child element of the specified parent node and
		''' immediately before the specified next sibling node.
		''' </summary>
		''' <param name="ks"> the key selector </param>
		''' <param name="parent"> the parent node </param>
		''' <param name="nextSibling"> the next sibling node </param>
		''' <exception cref="NullPointerException"> if <code>ks</code>, <code>parent</code> or
		'''    <code>nextSibling</code> is <code>null</code> </exception>
		Public Sub New(ByVal ks As javax.xml.crypto.KeySelector, ByVal parent As org.w3c.dom.Node, ByVal nextSibling As org.w3c.dom.Node)
			If ks Is Nothing Then Throw New NullPointerException("key selector cannot be null")
			If parent Is Nothing Then Throw New NullPointerException("parent cannot be null")
			If nextSibling Is Nothing Then Throw New NullPointerException("nextSibling cannot be null")
			keySelector = ks
			Me.parent = parent
			Me.nextSibling = nextSibling
		End Sub

		''' <summary>
		''' Sets the parent node.
		''' </summary>
		''' <param name="parent"> the parent node. The marshalled <code>XMLSignature</code>
		'''    will be added as a child element of this node. </param>
		''' <exception cref="NullPointerException"> if <code>parent</code> is <code>null</code> </exception>
		''' <seealso cref= #getParent </seealso>
		Public Overridable Property parent As org.w3c.dom.Node
			Set(ByVal parent As org.w3c.dom.Node)
				If parent Is Nothing Then Throw New NullPointerException("parent is null")
				Me.parent = parent
			End Set
			Get
				Return parent
			End Get
		End Property

		''' <summary>
		''' Sets the next sibling node.
		''' </summary>
		''' <param name="nextSibling"> the next sibling node. The marshalled
		'''    <code>XMLSignature</code> will be inserted immediately before this
		'''    node. Specify <code>null</code> to remove the current setting. </param>
		''' <seealso cref= #getNextSibling </seealso>
		Public Overridable Property nextSibling As org.w3c.dom.Node
			Set(ByVal nextSibling As org.w3c.dom.Node)
				Me.nextSibling = nextSibling
			End Set
			Get
				Return nextSibling
			End Get
		End Property


	End Class

End Namespace
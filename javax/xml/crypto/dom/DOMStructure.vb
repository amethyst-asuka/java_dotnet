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
' * $Id: DOMStructure.java,v 1.6 2005/05/09 18:33:26 mullan Exp $
' 
Namespace javax.xml.crypto.dom


	''' <summary>
	''' A DOM-specific <seealso cref="XMLStructure"/>. The purpose of this class is to
	''' allow a DOM node to be used to represent extensible content (any elements
	''' or mixed content) in XML Signature structures.
	''' 
	''' <p>If a sequence of nodes is needed, the node contained in the
	''' <code>DOMStructure</code> is the first node of the sequence and successive
	''' nodes can be accessed by invoking <seealso cref="Node#getNextSibling"/>.
	''' 
	''' <p>If the owner document of the <code>DOMStructure</code> is different than
	''' the target document of an <code>XMLSignature</code>, the
	''' <seealso cref="XMLSignature#sign(XMLSignContext)"/> method imports the node into the
	''' target document before generating the signature.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6
	''' </summary>
	Public Class DOMStructure
		Implements javax.xml.crypto.XMLStructure

		Private ReadOnly node As org.w3c.dom.Node

		''' <summary>
		''' Creates a <code>DOMStructure</code> containing the specified node.
		''' </summary>
		''' <param name="node"> the node </param>
		''' <exception cref="NullPointerException"> if <code>node</code> is <code>null</code> </exception>
		Public Sub New(ByVal node As org.w3c.dom.Node)
			If node Is Nothing Then Throw New NullPointerException("node cannot be null")
			Me.node = node
		End Sub

		''' <summary>
		''' Returns the node contained in this <code>DOMStructure</code>.
		''' </summary>
		''' <returns> the node </returns>
		Public Overridable Property node As org.w3c.dom.Node
			Get
				Return node
			End Get
		End Property

		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Function isFeatureSupported(ByVal feature As String) As Boolean
			If feature Is Nothing Then
				Throw New NullPointerException
			Else
				Return False
			End If
		End Function
	End Class

End Namespace
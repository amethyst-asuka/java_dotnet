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

Namespace javax.xml.transform.dom



	''' <summary>
	''' <p>Acts as a holder for a transformation Source tree in the
	''' form of a Document Object Model (DOM) tree.</p>
	''' 
	''' <p>Note that XSLT requires namespace support. Attempting to transform a DOM
	''' that was not contructed with a namespace-aware parser may result in errors.
	''' Parsers can be made namespace aware by calling
	''' <seealso cref="javax.xml.parsers.DocumentBuilderFactory#setNamespaceAware(boolean awareness)"/>.</p>
	''' 
	''' @author <a href="Jeff.Suttor@Sun.com">Jeff Suttor</a> </summary>
	''' <seealso cref= <a href="http://www.w3.org/TR/DOM-Level-2">Document Object Model (DOM) Level 2 Specification</a> </seealso>
	Public Class DOMSource
		Implements javax.xml.transform.Source

		''' <summary>
		''' <p><code>Node</code> to serve as DOM source.</p>
		''' </summary>
		Private node As org.w3c.dom.Node

		''' <summary>
		''' <p>The base ID (URL or system ID) from where URLs
		''' will be resolved.</p>
		''' </summary>
		Private systemID As String

		''' <summary>
		''' If <seealso cref="javax.xml.transform.TransformerFactory#getFeature"/>
		''' returns true when passed this value as an argument,
		''' the Transformer supports Source input of this type.
		''' </summary>
		Public Const FEATURE As String = "http://javax.xml.transform.dom.DOMSource/feature"

		''' <summary>
		''' <p>Zero-argument default constructor.  If this constructor is used, and
		''' no DOM source is set using <seealso cref="#setNode(Node node)"/> , then the
		''' <code>Transformer</code> will
		''' create an empty source <seealso cref="org.w3c.dom.Document"/> using
		''' <seealso cref="javax.xml.parsers.DocumentBuilder#newDocument()"/>.</p>
		''' </summary>
		''' <seealso cref= javax.xml.transform.Transformer#transform(Source xmlSource, Result outputTarget) </seealso>
		Public Sub New()
		End Sub

		''' <summary>
		''' Create a new input source with a DOM node.  The operation
		''' will be applied to the subtree rooted at this node.  In XSLT,
		''' a "/" pattern still means the root of the tree (not the subtree),
		''' and the evaluation of global variables and parameters is done
		''' from the root node also.
		''' </summary>
		''' <param name="n"> The DOM node that will contain the Source tree. </param>
		Public Sub New(ByVal n As org.w3c.dom.Node)
			node = n
		End Sub

		''' <summary>
		''' Create a new input source with a DOM node, and with the
		''' system ID also passed in as the base URI.
		''' </summary>
		''' <param name="node"> The DOM node that will contain the Source tree. </param>
		''' <param name="systemID"> Specifies the base URI associated with node. </param>
		Public Sub New(ByVal node As org.w3c.dom.Node, ByVal systemID As String)
			node = node
			systemId = systemID
		End Sub

		''' <summary>
		''' Set the node that will represents a Source DOM tree.
		''' </summary>
		''' <param name="node"> The node that is to be transformed. </param>
		Public Overridable Property node As org.w3c.dom.Node
			Set(ByVal node As org.w3c.dom.Node)
				Me.node = node
			End Set
			Get
				Return node
			End Get
		End Property


		''' <summary>
		''' Set the base ID (URL or system ID) from where URLs
		''' will be resolved.
		''' </summary>
		''' <param name="systemID"> Base URL for this DOM tree. </param>
		Public Overridable Property systemId As String
			Set(ByVal systemID As String)
				Me.systemID = systemID
			End Set
			Get
				Return Me.systemID
			End Get
		End Property

	End Class

End Namespace
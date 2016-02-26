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
	''' <p>Acts as a holder for a transformation result tree in the form of a Document Object Model (DOM) tree.</p>
	''' 
	''' <p>If no output DOM source is set, the transformation will create a Document node as the holder for the result of the transformation,
	''' which may be retrieved with <seealso cref="#getNode()"/>.</p>
	''' 
	''' @author <a href="Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' </summary>
	Public Class DOMResult
		Implements javax.xml.transform.Result

		''' <summary>
		''' <p>If <seealso cref="javax.xml.transform.TransformerFactory#getFeature"/>
		''' returns <code>true</code> when passed this value as an argument,
		''' the <code>Transformer</code> supports <code>Result</code> output of this type.</p>
		''' </summary>
		Public Const FEATURE As String = "http://javax.xml.transform.dom.DOMResult/feature"

		''' <summary>
		''' <p>Zero-argument default constructor.</p>
		''' 
		''' <p><code>node</code>,
		''' <code>siblingNode</code> and
		''' <code>systemId</code>
		''' will be set to <code>null</code>.</p>
		''' </summary>
		Public Sub New()
			node = Nothing
			nextSibling = Nothing
			systemId = Nothing
		End Sub

		''' <summary>
		''' <p>Use a DOM node to create a new output target.</p>
		''' 
		''' <p>In practice, the node should be
		''' a <seealso cref="org.w3c.dom.Document"/> node,
		''' a <seealso cref="org.w3c.dom.DocumentFragment"/> node, or
		''' a <seealso cref="org.w3c.dom.Element"/> node.
		''' In other words, a node that accepts children.</p>
		''' 
		''' <p><code>siblingNode</code> and
		''' <code>systemId</code>
		''' will be set to <code>null</code>.</p>
		''' </summary>
		''' <param name="node"> The DOM node that will contain the result tree. </param>
		Public Sub New(ByVal node As org.w3c.dom.Node)
			node = node
			nextSibling = Nothing
			systemId = Nothing
		End Sub

		''' <summary>
		''' <p>Use a DOM node to create a new output target with the specified System ID.<p>
		''' 
		''' <p>In practice, the node should be
		''' a <seealso cref="org.w3c.dom.Document"/> node,
		''' a <seealso cref="org.w3c.dom.DocumentFragment"/> node, or
		''' a <seealso cref="org.w3c.dom.Element"/> node.
		''' In other words, a node that accepts children.</p>
		''' 
		''' <p><code>siblingNode</code> will be set to <code>null</code>.</p>
		''' </summary>
		''' <param name="node"> The DOM node that will contain the result tree. </param>
		''' <param name="systemId"> The system identifier which may be used in association with this node. </param>
		Public Sub New(ByVal node As org.w3c.dom.Node, ByVal systemId As String)
			node = node
			nextSibling = Nothing
			systemId = systemId
		End Sub

		''' <summary>
		''' <p>Use a DOM node to create a new output target specifying the child node where the result nodes should be inserted before.</p>
		''' 
		''' <p>In practice, <code>node</code> and <code>nextSibling</code> should be
		''' a <seealso cref="org.w3c.dom.Document"/> node,
		''' a <seealso cref="org.w3c.dom.DocumentFragment"/> node, or
		''' a <seealso cref="org.w3c.dom.Element"/> node.
		''' In other words, a node that accepts children.</p>
		''' 
		''' <p>Use <code>nextSibling</code> to specify the child node
		''' where the result nodes should be inserted before.
		''' If <code>nextSibling</code> is not a sibling of <code>node</code>,
		''' then an <code>IllegalArgumentException</code> is thrown.
		''' If <code>node</code> is <code>null</code> and <code>nextSibling</code> is not <code>null</code>,
		''' then an <code>IllegalArgumentException</code> is thrown.
		''' If <code>nextSibling</code> is <code>null</code>,
		''' then the behavior is the same as calling <seealso cref="#DOMResult(Node node)"/>,
		''' i.e. append the result nodes as the last child of the specified <code>node</code>.</p>
		''' 
		''' <p><code>systemId</code> will be set to <code>null</code>.</p>
		''' </summary>
		''' <param name="node"> The DOM node that will contain the result tree. </param>
		''' <param name="nextSibling"> The child node where the result nodes should be inserted before.
		''' </param>
		''' <exception cref="IllegalArgumentException"> If <code>nextSibling</code> is not a sibling of <code>node</code> or
		'''   <code>node</code> is <code>null</code> and <code>nextSibling</code>
		'''   is not <code>null</code>.
		''' 
		''' @since 1.5 </exception>
		Public Sub New(ByVal node As org.w3c.dom.Node, ByVal nextSibling As org.w3c.dom.Node)

			' does the corrent parent/child relationship exist?
			If nextSibling IsNot Nothing Then
				' cannot be a sibling of a null node
				If node Is Nothing Then Throw New System.ArgumentException("Cannot create a DOMResult when the nextSibling is contained by the ""null"" node.")

				' nextSibling contained by node?
				If (node.compareDocumentPosition(nextSibling) And org.w3c.dom.Node.DOCUMENT_POSITION_CONTAINED_BY)=0 Then Throw New System.ArgumentException("Cannot create a DOMResult when the nextSibling is not contained by the node.")
			End If

			node = node
			nextSibling = nextSibling
			systemId = Nothing
		End Sub

		''' <summary>
		''' <p>Use a DOM node to create a new output target specifying the child node where the result nodes should be inserted before and
		''' the specified System ID.</p>
		''' 
		''' <p>In practice, <code>node</code> and <code>nextSibling</code> should be
		''' a <seealso cref="org.w3c.dom.Document"/> node,
		''' a <seealso cref="org.w3c.dom.DocumentFragment"/> node, or a
		''' <seealso cref="org.w3c.dom.Element"/> node.
		''' In other words, a node that accepts children.</p>
		''' 
		''' <p>Use <code>nextSibling</code> to specify the child node
		''' where the result nodes should be inserted before.
		''' If <code>nextSibling</code> is not a sibling of <code>node</code>,
		''' then an <code>IllegalArgumentException</code> is thrown.
		''' If <code>node</code> is <code>null</code> and <code>nextSibling</code> is not <code>null</code>,
		''' then an <code>IllegalArgumentException</code> is thrown.
		''' If <code>nextSibling</code> is <code>null</code>,
		''' then the behavior is the same as calling <seealso cref="#DOMResult(Node node, String systemId)"/>,
		''' i.e. append the result nodes as the last child of the specified node and use the specified System ID.</p>
		''' </summary>
		''' <param name="node"> The DOM node that will contain the result tree. </param>
		''' <param name="nextSibling"> The child node where the result nodes should be inserted before. </param>
		''' <param name="systemId"> The system identifier which may be used in association with this node.
		''' </param>
		''' <exception cref="IllegalArgumentException"> If <code>nextSibling</code> is not a
		'''   sibling of <code>node</code> or
		'''   <code>node</code> is <code>null</code> and <code>nextSibling</code>
		'''   is not <code>null</code>.
		''' 
		''' @since 1.5 </exception>
		Public Sub New(ByVal node As org.w3c.dom.Node, ByVal nextSibling As org.w3c.dom.Node, ByVal systemId As String)

			' does the corrent parent/child relationship exist?
			If nextSibling IsNot Nothing Then
				' cannot be a sibling of a null node
				If node Is Nothing Then Throw New System.ArgumentException("Cannot create a DOMResult when the nextSibling is contained by the ""null"" node.")

				' nextSibling contained by node?
				If (node.compareDocumentPosition(nextSibling) And org.w3c.dom.Node.DOCUMENT_POSITION_CONTAINED_BY)=0 Then Throw New System.ArgumentException("Cannot create a DOMResult when the nextSibling is not contained by the node.")
			End If

			node = node
			nextSibling = nextSibling
			systemId = systemId
		End Sub

		''' <summary>
		''' <p>Set the node that will contain the result DOM tree.<p>
		''' 
		''' <p>In practice, the node should be
		''' a <seealso cref="org.w3c.dom.Document"/> node,
		''' a <seealso cref="org.w3c.dom.DocumentFragment"/> node, or
		''' a <seealso cref="org.w3c.dom.Element"/> node.
		''' In other words, a node that accepts children.</p>
		''' 
		''' <p>An <code>IllegalStateException</code> is thrown if
		''' <code>nextSibling</code> is not <code>null</code> and
		''' <code>node</code> is not a parent of <code>nextSibling</code>.
		''' An <code>IllegalStateException</code> is thrown if <code>node</code> is <code>null</code> and
		''' <code>nextSibling</code> is not <code>null</code>.</p>
		''' </summary>
		''' <param name="node"> The node to which the transformation will be appended.
		''' </param>
		''' <exception cref="IllegalStateException"> If <code>nextSibling</code> is not
		'''   <code>null</code> and
		'''   <code>nextSibling</code> is not a child of <code>node</code> or
		'''   <code>node</code> is <code>null</code> and
		'''   <code>nextSibling</code> is not <code>null</code>. </exception>
		Public Overridable Property node As org.w3c.dom.Node
			Set(ByVal node As org.w3c.dom.Node)
				' does the corrent parent/child relationship exist?
				If nextSibling IsNot Nothing Then
					' cannot be a sibling of a null node
					If node Is Nothing Then Throw New IllegalStateException("Cannot create a DOMResult when the nextSibling is contained by the ""null"" node.")
    
					' nextSibling contained by node?
					If (node.compareDocumentPosition(nextSibling) And org.w3c.dom.Node.DOCUMENT_POSITION_CONTAINED_BY)=0 Then Throw New System.ArgumentException("Cannot create a DOMResult when the nextSibling is not contained by the node.")
				End If
    
				Me.node = node
			End Set
			Get
				Return node
			End Get
		End Property


		''' <summary>
		''' <p>Set the child node before which the result nodes will be inserted.</p>
		''' 
		''' <p>Use <code>nextSibling</code> to specify the child node
		''' before which the result nodes should be inserted.
		''' If <code>nextSibling</code> is not a descendant of <code>node</code>,
		''' then an <code>IllegalArgumentException</code> is thrown.
		''' If <code>node</code> is <code>null</code> and <code>nextSibling</code> is not <code>null</code>,
		''' then an <code>IllegalStateException</code> is thrown.
		''' If <code>nextSibling</code> is <code>null</code>,
		''' then the behavior is the same as calling <seealso cref="#DOMResult(Node node)"/>,
		''' i.e. append the result nodes as the last child of the specified <code>node</code>.</p>
		''' </summary>
		''' <param name="nextSibling"> The child node before which the result nodes will be inserted.
		''' </param>
		''' <exception cref="IllegalArgumentException"> If <code>nextSibling</code> is not a
		'''   descendant of <code>node</code>. </exception>
		''' <exception cref="IllegalStateException"> If <code>node</code> is <code>null</code>
		'''   and <code>nextSibling</code> is not <code>null</code>.
		''' 
		''' @since 1.5 </exception>
		Public Overridable Property nextSibling As org.w3c.dom.Node
			Set(ByVal nextSibling As org.w3c.dom.Node)
    
				' does the corrent parent/child relationship exist?
				If nextSibling IsNot Nothing Then
					' cannot be a sibling of a null node
					If node Is Nothing Then Throw New IllegalStateException("Cannot create a DOMResult when the nextSibling is contained by the ""null"" node.")
    
					' nextSibling contained by node?
					If (node.compareDocumentPosition(nextSibling) And org.w3c.dom.Node.DOCUMENT_POSITION_CONTAINED_BY)=0 Then Throw New System.ArgumentException("Cannot create a DOMResult when the nextSibling is not contained by the node.")
				End If
    
				Me.nextSibling = nextSibling
			End Set
			Get
				Return nextSibling
			End Get
		End Property


		''' <summary>
		''' <p>Set the systemId that may be used in association with the node.</p>
		''' </summary>
		''' <param name="systemId"> The system identifier as a URI string. </param>
		Public Overridable Property systemId As String
			Set(ByVal systemId As String)
				Me.systemId = systemId
			End Set
			Get
				Return systemId
			End Get
		End Property


		'////////////////////////////////////////////////////////////////////
		' Internal state.
		'////////////////////////////////////////////////////////////////////

		''' <summary>
		''' <p>The node to which the transformation will be appended.</p>
		''' </summary>
		Private node As org.w3c.dom.Node = Nothing

		''' <summary>
		''' <p>The child node before which the result nodes will be inserted.</p>
		''' 
		''' @since 1.5
		''' </summary>
		Private nextSibling As org.w3c.dom.Node = Nothing

		''' <summary>
		''' <p>The System ID that may be used in association with the node.</p>
		''' </summary>
		Private systemId As String = Nothing
	End Class

End Namespace
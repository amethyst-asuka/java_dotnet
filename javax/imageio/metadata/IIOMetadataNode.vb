Imports System.Collections

'
' * Copyright (c) 2000, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.imageio.metadata




	Friend Class IIODOMException
		Inherits org.w3c.dom.DOMException

		Public Sub New(ByVal code As Short, ByVal message As String)
			MyBase.New(code, message)
		End Sub
	End Class

	Friend Class IIONamedNodeMap
		Implements org.w3c.dom.NamedNodeMap

		Friend nodes As IList

		Public Sub New(ByVal nodes As IList)
			Me.nodes = nodes
		End Sub

		Public Overridable Property length As Integer
			Get
				Return nodes.Count
			End Get
		End Property

		Public Overridable Function getNamedItem(ByVal name As String) As org.w3c.dom.Node
			Dim iter As IEnumerator = nodes.GetEnumerator()
			Do While iter.hasNext()
				Dim node As org.w3c.dom.Node = CType(iter.next(), org.w3c.dom.Node)
				If name.Equals(node.nodeName) Then Return node
			Loop

			Return Nothing
		End Function

		Public Overridable Function item(ByVal index As Integer) As org.w3c.dom.Node
			Dim node As org.w3c.dom.Node = CType(nodes(index), org.w3c.dom.Node)
			Return node
		End Function

		Public Overridable Function removeNamedItem(ByVal name As String) As org.w3c.dom.Node
			Throw New org.w3c.dom.DOMException(org.w3c.dom.DOMException.NO_MODIFICATION_ALLOWED_ERR, "This NamedNodeMap is read-only!")
		End Function

		Public Overridable Function setNamedItem(ByVal arg As org.w3c.dom.Node) As org.w3c.dom.Node
			Throw New org.w3c.dom.DOMException(org.w3c.dom.DOMException.NO_MODIFICATION_ALLOWED_ERR, "This NamedNodeMap is read-only!")
		End Function

		''' <summary>
		''' Equivalent to <code>getNamedItem(localName)</code>.
		''' </summary>
		Public Overridable Function getNamedItemNS(ByVal namespaceURI As String, ByVal localName As String) As org.w3c.dom.Node
			Return getNamedItem(localName)
		End Function

		''' <summary>
		''' Equivalent to <code>setNamedItem(arg)</code>.
		''' </summary>
		Public Overridable Function setNamedItemNS(ByVal arg As org.w3c.dom.Node) As org.w3c.dom.Node
			Return namedItemtem(arg)
		End Function

		''' <summary>
		''' Equivalent to <code>removeNamedItem(localName)</code>.
		''' </summary>
		Public Overridable Function removeNamedItemNS(ByVal namespaceURI As String, ByVal localName As String) As org.w3c.dom.Node
			Return removeNamedItem(localName)
		End Function
	End Class

	Friend Class IIONodeList
		Implements org.w3c.dom.NodeList

		Friend nodes As IList

		Public Sub New(ByVal nodes As IList)
			Me.nodes = nodes
		End Sub

		Public Overridable Property length As Integer
			Get
				Return nodes.Count
			End Get
		End Property

		Public Overridable Function item(ByVal index As Integer) As org.w3c.dom.Node
			If index < 0 OrElse index > nodes.Count Then Return Nothing
			Return CType(nodes(index), org.w3c.dom.Node)
		End Function
	End Class

	Friend Class IIOAttr
		Inherits IIOMetadataNode
		Implements org.w3c.dom.Attr

		Friend owner As org.w3c.dom.Element
		Friend name As String
		Friend value As String

		Public Sub New(ByVal owner As org.w3c.dom.Element, ByVal name As String, ByVal value As String)
			Me.owner = owner
			Me.name = name
			Me.value = value
		End Sub

		Public Overridable Property name As String
			Get
				Return name
			End Get
		End Property

		Public Property Overrides nodeName As String
			Get
				Return name
			End Get
		End Property

		Public Property Overrides nodeType As Short
			Get
				Return ATTRIBUTE_NODE
			End Get
		End Property

		Public Overridable Property specified As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overridable Property value As String
			Get
				Return value
			End Get
			Set(ByVal value As String)
				Me.value = value
			End Set
		End Property

		Public Property Overrides nodeValue As String
			Get
				Return value
			End Get
			Set(ByVal value As String)
				Me.value = value
			End Set
		End Property



		Public Overridable Property ownerElement As org.w3c.dom.Element
			Get
				Return owner
			End Get
			Set(ByVal owner As org.w3c.dom.Element)
				Me.owner = owner
			End Set
		End Property


		''' <summary>
		''' This method is new in the DOM L3 for Attr interface.
		''' Could throw DOMException here, but its probably OK
		''' to always return false. One reason for this, is we have no good
		''' way to document this exception, since this class, IIOAttr,
		''' is not a public class. The rest of the methods that throw
		''' DOMException are publically documented as such on IIOMetadataNode. </summary>
		''' <returns> false </returns>
		Public Overridable Property id As Boolean
			Get
				Return False
			End Get
		End Property


	End Class

	''' <summary>
	''' A class representing a node in a meta-data tree, which implements
	''' the <a
	''' href="../../../../api/org/w3c/dom/Element.html">
	''' <code>org.w3c.dom.Element</code></a> interface and additionally allows
	''' for the storage of non-textual objects via the
	''' <code>getUserObject</code> and <code>setUserObject</code> methods.
	''' 
	''' <p> This class is not intended to be used for general XML
	''' processing. In particular, <code>Element</code> nodes created
	''' within the Image I/O API are not compatible with those created by
	''' Sun's standard implementation of the <code>org.w3.dom</code> API.
	''' In particular, the implementation is tuned for simple uses and may
	''' not perform well for intensive processing.
	''' 
	''' <p> Namespaces are ignored in this implementation.  The terms "tag
	''' name" and "node name" are always considered to be synonymous.
	''' 
	''' <em>Note:</em>
	''' The DOM Level 3 specification added a number of new methods to the
	''' {@code Node}, {@code Element} and {@code Attr} interfaces that are not
	''' of value to the {@code IIOMetadataNode} implementation or specification.
	''' 
	''' Calling such methods on an {@code IIOMetadataNode}, or an {@code Attr}
	''' instance returned from an {@code IIOMetadataNode} will result in a
	''' {@code DOMException} being thrown.
	''' </summary>
	''' <seealso cref= IIOMetadata#getAsTree </seealso>
	''' <seealso cref= IIOMetadata#setFromTree </seealso>
	''' <seealso cref= IIOMetadata#mergeTree
	'''  </seealso>
	Public Class IIOMetadataNode
		Implements org.w3c.dom.Element, org.w3c.dom.NodeList

		''' <summary>
		''' The name of the node as a <code>String</code>.
		''' </summary>
		Private nodeName As String = Nothing

		''' <summary>
		''' The value of the node as a <code>String</code>.  The Image I/O
		''' API typically does not make use of the node value.
		''' </summary>
		Private nodeValue As String = Nothing

		''' <summary>
		''' The <code>Object</code> value associated with this node.
		''' </summary>
		Private userObject As Object = Nothing

		''' <summary>
		''' The parent node of this node, or <code>null</code> if this node
		''' forms the root of its own tree.
		''' </summary>
		Private parent As IIOMetadataNode = Nothing

		''' <summary>
		''' The number of child nodes.
		''' </summary>
		Private numChildren As Integer = 0

		''' <summary>
		''' The first (leftmost) child node of this node, or
		''' <code>null</code> if this node is a leaf node.
		''' </summary>
		Private firstChild As IIOMetadataNode = Nothing

		''' <summary>
		''' The last (rightmost) child node of this node, or
		''' <code>null</code> if this node is a leaf node.
		''' </summary>
		Private lastChild As IIOMetadataNode = Nothing

		''' <summary>
		''' The next (right) sibling node of this node, or
		''' <code>null</code> if this node is its parent's last child node.
		''' </summary>
		Private nextSibling As IIOMetadataNode = Nothing

		''' <summary>
		''' The previous (left) sibling node of this node, or
		''' <code>null</code> if this node is its parent's first child node.
		''' </summary>
		Private previousSibling As IIOMetadataNode = Nothing

		''' <summary>
		''' A <code>List</code> of <code>IIOAttr</code> nodes representing
		''' attributes.
		''' </summary>
		Private attributes As IList = New ArrayList

		''' <summary>
		''' Constructs an empty <code>IIOMetadataNode</code>.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs an <code>IIOMetadataNode</code> with a given node
		''' name.
		''' </summary>
		''' <param name="nodeName"> the name of the node, as a <code>String</code>. </param>
		Public Sub New(ByVal nodeName As String)
			Me.nodeName = nodeName
		End Sub

		''' <summary>
		''' Check that the node is either <code>null</code> or an
		''' <code>IIOMetadataNode</code>.
		''' </summary>
		Private Sub checkNode(ByVal node As org.w3c.dom.Node)
			If node Is Nothing Then Return
			If Not(TypeOf node Is IIOMetadataNode) Then Throw New IIODOMException(org.w3c.dom.DOMException.WRONG_DOCUMENT_ERR, "Node not an IIOMetadataNode!")
		End Sub

		' Methods from Node

		''' <summary>
		''' Returns the node name associated with this node.
		''' </summary>
		''' <returns> the node name, as a <code>String</code>. </returns>
		Public Overridable Property nodeName As String
			Get
				Return nodeName
			End Get
		End Property

		''' <summary>
		''' Returns the value associated with this node.
		''' </summary>
		''' <returns> the node value, as a <code>String</code>. </returns>
		Public Overridable Property nodeValue As String
			Get
				Return nodeValue
			End Get
			Set(ByVal nodeValue As String)
				Me.nodeValue = nodeValue
			End Set
		End Property


		''' <summary>
		''' Returns the node type, which is always
		''' <code>ELEMENT_NODE</code>.
		''' </summary>
		''' <returns> the <code>short</code> value <code>ELEMENT_NODE</code>. </returns>
		Public Overridable Property nodeType As Short
			Get
				Return ELEMENT_NODE
			End Get
		End Property

		''' <summary>
		''' Returns the parent of this node.  A <code>null</code> value
		''' indicates that the node is the root of its own tree.  To add a
		''' node to an existing tree, use one of the
		''' <code>insertBefore</code>, <code>replaceChild</code>, or
		''' <code>appendChild</code> methods.
		''' </summary>
		''' <returns> the parent, as a <code>Node</code>.
		''' </returns>
		''' <seealso cref= #insertBefore </seealso>
		''' <seealso cref= #replaceChild </seealso>
		''' <seealso cref= #appendChild </seealso>
		Public Overridable Property parentNode As org.w3c.dom.Node
			Get
				Return parent
			End Get
		End Property

		''' <summary>
		''' Returns a <code>NodeList</code> that contains all children of this node.
		''' If there are no children, this is a <code>NodeList</code> containing
		''' no nodes.
		''' </summary>
		''' <returns> the children as a <code>NodeList</code> </returns>
		Public Overridable Property childNodes As org.w3c.dom.NodeList
			Get
				Return Me
			End Get
		End Property

		''' <summary>
		''' Returns the first child of this node, or <code>null</code> if
		''' the node has no children.
		''' </summary>
		''' <returns> the first child, as a <code>Node</code>, or
		''' <code>null</code> </returns>
		Public Overridable Property firstChild As org.w3c.dom.Node
			Get
				Return firstChild
			End Get
		End Property

		''' <summary>
		''' Returns the last child of this node, or <code>null</code> if
		''' the node has no children.
		''' </summary>
		''' <returns> the last child, as a <code>Node</code>, or
		''' <code>null</code>. </returns>
		Public Overridable Property lastChild As org.w3c.dom.Node
			Get
				Return lastChild
			End Get
		End Property

		''' <summary>
		''' Returns the previous sibling of this node, or <code>null</code>
		''' if this node has no previous sibling.
		''' </summary>
		''' <returns> the previous sibling, as a <code>Node</code>, or
		''' <code>null</code>. </returns>
		Public Overridable Property previousSibling As org.w3c.dom.Node
			Get
				Return previousSibling
			End Get
		End Property

		''' <summary>
		''' Returns the next sibling of this node, or <code>null</code> if
		''' the node has no next sibling.
		''' </summary>
		''' <returns> the next sibling, as a <code>Node</code>, or
		''' <code>null</code>. </returns>
		Public Overridable Property nextSibling As org.w3c.dom.Node
			Get
				Return nextSibling
			End Get
		End Property

		''' <summary>
		''' Returns a <code>NamedNodeMap</code> containing the attributes of
		''' this node.
		''' </summary>
		''' <returns> a <code>NamedNodeMap</code> containing the attributes of
		''' this node. </returns>
		Public Overridable Property attributes As org.w3c.dom.NamedNodeMap
			Get
				Return New IIONamedNodeMap(attributes)
			End Get
		End Property

		''' <summary>
		''' Returns <code>null</code>, since <code>IIOMetadataNode</code>s
		''' do not belong to any <code>Document</code>.
		''' </summary>
		''' <returns> <code>null</code>. </returns>
		Public Overridable Property ownerDocument As org.w3c.dom.Document
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Inserts the node <code>newChild</code> before the existing
		''' child node <code>refChild</code>. If <code>refChild</code> is
		''' <code>null</code>, insert <code>newChild</code> at the end of
		''' the list of children.
		''' </summary>
		''' <param name="newChild"> the <code>Node</code> to insert. </param>
		''' <param name="refChild"> the reference <code>Node</code>.
		''' </param>
		''' <returns> the node being inserted.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>newChild</code> is
		''' <code>null</code>. </exception>
		Public Overridable Function insertBefore(ByVal newChild As org.w3c.dom.Node, ByVal refChild As org.w3c.dom.Node) As org.w3c.dom.Node
			If newChild Is Nothing Then Throw New System.ArgumentException("newChild == null!")

			checkNode(newChild)
			checkNode(refChild)

			Dim newChildNode As IIOMetadataNode = CType(newChild, IIOMetadataNode)
			Dim refChildNode As IIOMetadataNode = CType(refChild, IIOMetadataNode)

			' Siblings, can be null.
			Dim previous As IIOMetadataNode = Nothing
			Dim [next] As IIOMetadataNode = Nothing

			If refChild Is Nothing Then
				previous = Me.lastChild
				[next] = Nothing
				Me.lastChild = newChildNode
			Else
				previous = refChildNode.previousSibling
				[next] = refChildNode
			End If

			If previous IsNot Nothing Then previous.nextSibling = newChildNode
			If [next] IsNot Nothing Then [next].previousSibling = newChildNode

			newChildNode.parent = Me
			newChildNode.previousSibling = previous
			newChildNode.nextSibling = [next]

			' N.B.: O.K. if refChild == null
			If Me.firstChild Is refChildNode Then Me.firstChild = newChildNode

			numChildren += 1
			Return newChildNode
		End Function

		''' <summary>
		''' Replaces the child node <code>oldChild</code> with
		''' <code>newChild</code> in the list of children, and returns the
		''' <code>oldChild</code> node.
		''' </summary>
		''' <param name="newChild"> the <code>Node</code> to insert. </param>
		''' <param name="oldChild"> the <code>Node</code> to be replaced.
		''' </param>
		''' <returns> the node replaced.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>newChild</code> is
		''' <code>null</code>. </exception>
		Public Overridable Function replaceChild(ByVal newChild As org.w3c.dom.Node, ByVal oldChild As org.w3c.dom.Node) As org.w3c.dom.Node
			If newChild Is Nothing Then Throw New System.ArgumentException("newChild == null!")

			checkNode(newChild)
			checkNode(oldChild)

			Dim newChildNode As IIOMetadataNode = CType(newChild, IIOMetadataNode)
			Dim oldChildNode As IIOMetadataNode = CType(oldChild, IIOMetadataNode)

			Dim previous As IIOMetadataNode = oldChildNode.previousSibling
			Dim [next] As IIOMetadataNode = oldChildNode.nextSibling

			If previous IsNot Nothing Then previous.nextSibling = newChildNode
			If [next] IsNot Nothing Then [next].previousSibling = newChildNode

			newChildNode.parent = Me
			newChildNode.previousSibling = previous
			newChildNode.nextSibling = [next]

			If firstChild Is oldChildNode Then firstChild = newChildNode
			If lastChild Is oldChildNode Then lastChild = newChildNode

			oldChildNode.parent = Nothing
			oldChildNode.previousSibling = Nothing
			oldChildNode.nextSibling = Nothing

			Return oldChildNode
		End Function

		''' <summary>
		''' Removes the child node indicated by <code>oldChild</code> from
		''' the list of children, and returns it.
		''' </summary>
		''' <param name="oldChild"> the <code>Node</code> to be removed.
		''' </param>
		''' <returns> the node removed.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>oldChild</code> is
		''' <code>null</code>. </exception>
		Public Overridable Function removeChild(ByVal oldChild As org.w3c.dom.Node) As org.w3c.dom.Node
			If oldChild Is Nothing Then Throw New System.ArgumentException("oldChild == null!")
			checkNode(oldChild)

			Dim oldChildNode As IIOMetadataNode = CType(oldChild, IIOMetadataNode)

			Dim previous As IIOMetadataNode = oldChildNode.previousSibling
			Dim [next] As IIOMetadataNode = oldChildNode.nextSibling

			If previous IsNot Nothing Then previous.nextSibling = [next]
			If [next] IsNot Nothing Then [next].previousSibling = previous

			If Me.firstChild Is oldChildNode Then Me.firstChild = [next]
			If Me.lastChild Is oldChildNode Then Me.lastChild = previous

			oldChildNode.parent = Nothing
			oldChildNode.previousSibling = Nothing
			oldChildNode.nextSibling = Nothing

			numChildren -= 1
			Return oldChildNode
		End Function

		''' <summary>
		''' Adds the node <code>newChild</code> to the end of the list of
		''' children of this node.
		''' </summary>
		''' <param name="newChild"> the <code>Node</code> to insert.
		''' </param>
		''' <returns> the node added.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>newChild</code> is
		''' <code>null</code>. </exception>
		Public Overridable Function appendChild(ByVal newChild As org.w3c.dom.Node) As org.w3c.dom.Node
			If newChild Is Nothing Then Throw New System.ArgumentException("newChild == null!")
			checkNode(newChild)

			' insertBefore will increment numChildren
			Return insertBefore(newChild, Nothing)
		End Function

		''' <summary>
		''' Returns <code>true</code> if this node has child nodes.
		''' </summary>
		''' <returns> <code>true</code> if this node has children. </returns>
		Public Overridable Function hasChildNodes() As Boolean
			Return numChildren > 0
		End Function

		''' <summary>
		''' Returns a duplicate of this node.  The duplicate node has no
		''' parent (<code>getParentNode</code> returns <code>null</code>).
		''' If a shallow clone is being performed (<code>deep</code> is
		''' <code>false</code>), the new node will not have any children or
		''' siblings.  If a deep clone is being performed, the new node
		''' will form the root of a complete cloned subtree.
		''' </summary>
		''' <param name="deep"> if <code>true</code>, recursively clone the subtree
		''' under the specified node; if <code>false</code>, clone only the
		''' node itself.
		''' </param>
		''' <returns> the duplicate node. </returns>
		Public Overridable Function cloneNode(ByVal deep As Boolean) As org.w3c.dom.Node
			Dim newNode As New IIOMetadataNode(Me.nodeName)
			newNode.userObject = userObject
			' Attributes

			If deep Then
				Dim child As IIOMetadataNode = firstChild
				Do While child IsNot Nothing
					newNode.appendChild(child.cloneNode(True))
					child = child.nextSibling
				Loop
			End If

			Return newNode
		End Function

		''' <summary>
		''' Does nothing, since <code>IIOMetadataNode</code>s do not
		''' contain <code>Text</code> children.
		''' </summary>
		Public Overridable Sub normalize()
		End Sub

		''' <summary>
		''' Returns <code>false</code> since DOM features are not
		''' supported.
		''' </summary>
		''' <returns> <code>false</code>.
		''' </returns>
		''' <param name="feature"> a <code>String</code>, which is ignored. </param>
		''' <param name="version"> a <code>String</code>, which is ignored. </param>
		Public Overridable Function isSupported(ByVal feature As String, ByVal version As String) As Boolean
			Return False
		End Function

		''' <summary>
		''' Returns <code>null</code>, since namespaces are not supported.
		''' </summary>
		Public Overridable Property namespaceURI As String
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns <code>null</code>, since namespaces are not supported.
		''' </summary>
		''' <returns> <code>null</code>.
		''' </returns>
		''' <seealso cref= #setPrefix </seealso>
		Public Overridable Property prefix As String
			Get
				Return Nothing
			End Get
			Set(ByVal prefix As String)
			End Set
		End Property


		''' <summary>
		''' Equivalent to <code>getNodeName</code>.
		''' </summary>
		''' <returns> the node name, as a <code>String</code>. </returns>
		Public Overridable Property localName As String
			Get
				Return nodeName
			End Get
		End Property

		' Methods from Element


		''' <summary>
		''' Equivalent to <code>getNodeName</code>.
		''' </summary>
		''' <returns> the node name, as a <code>String</code> </returns>
		Public Overridable Property tagName As String
			Get
				Return nodeName
			End Get
		End Property

		''' <summary>
		''' Retrieves an attribute value by name. </summary>
		''' <param name="name"> The name of the attribute to retrieve. </param>
		''' <returns> The <code>Attr</code> value as a string, or the empty string
		''' if that attribute does not have a specified or default value. </returns>
		Public Overridable Function getAttribute(ByVal name As String) As String
			Dim attr As org.w3c.dom.Attr = getAttributeNode(name)
			If attr Is Nothing Then Return ""
			Return attr.value
		End Function

		''' <summary>
		''' Equivalent to <code>getAttribute(localName)</code>.
		''' </summary>
		''' <seealso cref= #setAttributeNS </seealso>
		Public Overridable Function getAttributeNS(ByVal namespaceURI As String, ByVal localName As String) As String
			Return getAttribute(localName)
		End Function

		Public Overridable Sub setAttribute(ByVal name As String, ByVal value As String)
			' Name must be valid unicode chars
			Dim valid As Boolean = True
			Dim chs As Char() = name.ToCharArray()
			For i As Integer = 0 To chs.Length - 1
				If chs(i) >= &Hfffe Then
					valid = False
					Exit For
				End If
			Next i
			If Not valid Then Throw New IIODOMException(org.w3c.dom.DOMException.INVALID_CHARACTER_ERR, "Attribute name is illegal!")
			removeAttribute(name, False)
			attributes.Add(New IIOAttr(Me, name, value))
		End Sub

		''' <summary>
		''' Equivalent to <code>setAttribute(qualifiedName, value)</code>.
		''' </summary>
		''' <seealso cref= #getAttributeNS </seealso>
		Public Overridable Sub setAttributeNS(ByVal namespaceURI As String, ByVal qualifiedName As String, ByVal value As String)
			attributeute(qualifiedName, value)
		End Sub

		Public Overridable Sub removeAttribute(ByVal name As String)
			removeAttribute(name, True)
		End Sub

		Private Sub removeAttribute(ByVal name As String, ByVal checkPresent As Boolean)
			Dim numAttributes As Integer = attributes.Count
			For i As Integer = 0 To numAttributes - 1
				Dim attr As IIOAttr = CType(attributes(i), IIOAttr)
				If name.Equals(attr.name) Then
					attr.ownerElement = Nothing
					attributes.RemoveAt(i)
					Return
				End If
			Next i

			' If we get here, the attribute doesn't exist
			If checkPresent Then Throw New IIODOMException(org.w3c.dom.DOMException.NOT_FOUND_ERR, "No such attribute!")
		End Sub

		''' <summary>
		''' Equivalent to <code>removeAttribute(localName)</code>.
		''' </summary>
		Public Overridable Sub removeAttributeNS(ByVal namespaceURI As String, ByVal localName As String)
			removeAttribute(localName)
		End Sub

		Public Overridable Function getAttributeNode(ByVal name As String) As org.w3c.dom.Attr
			Dim node As org.w3c.dom.Node = attributes.getNamedItem(name)
			Return CType(node, org.w3c.dom.Attr)
		End Function

		''' <summary>
		''' Equivalent to <code>getAttributeNode(localName)</code>.
		''' </summary>
		''' <seealso cref= #setAttributeNodeNS </seealso>
	   Public Overridable Function getAttributeNodeNS(ByVal namespaceURI As String, ByVal localName As String) As org.w3c.dom.Attr
			Return getAttributeNode(localName)
	   End Function

		Public Overridable Function setAttributeNode(ByVal newAttr As org.w3c.dom.Attr) As org.w3c.dom.Attr
			Dim owner As org.w3c.dom.Element = newAttr.ownerElement
			If owner IsNot Nothing Then
				If owner Is Me Then
					Return Nothing
				Else
					Throw New org.w3c.dom.DOMException(org.w3c.dom.DOMException.INUSE_ATTRIBUTE_ERR, "Attribute is already in use")
				End If
			End If

			Dim attr As IIOAttr
			If TypeOf newAttr Is IIOAttr Then
				attr = CType(newAttr, IIOAttr)
				attr.ownerElement = Me
			Else
				attr = New IIOAttr(Me, newAttr.name, newAttr.value)
			End If

			Dim oldAttr As org.w3c.dom.Attr = getAttributeNode(attr.name)
			If oldAttr IsNot Nothing Then removeAttributeNode(oldAttr)

			attributes.Add(attr)

			Return oldAttr
		End Function

		''' <summary>
		''' Equivalent to <code>setAttributeNode(newAttr)</code>.
		''' </summary>
		''' <seealso cref= #getAttributeNodeNS </seealso>
		Public Overridable Function setAttributeNodeNS(ByVal newAttr As org.w3c.dom.Attr) As org.w3c.dom.Attr
			Return attributeNodeode(newAttr)
		End Function

		Public Overridable Function removeAttributeNode(ByVal oldAttr As org.w3c.dom.Attr) As org.w3c.dom.Attr
			removeAttribute(oldAttr.name)
			Return oldAttr
		End Function

		Public Overridable Function getElementsByTagName(ByVal name As String) As org.w3c.dom.NodeList
			Dim l As IList = New ArrayList
			getElementsByTagName(name, l)
			Return New IIONodeList(l)
		End Function

		Private Sub getElementsByTagName(ByVal name As String, ByVal l As IList)
			If nodeName.Equals(name) Then l.Add(Me)

			Dim child As org.w3c.dom.Node = firstChild
			Do While child IsNot Nothing
				CType(child, IIOMetadataNode).getElementsByTagName(name, l)
				child = child.nextSibling
			Loop
		End Sub

		''' <summary>
		''' Equivalent to <code>getElementsByTagName(localName)</code>.
		''' </summary>
		Public Overridable Function getElementsByTagNameNS(ByVal namespaceURI As String, ByVal localName As String) As org.w3c.dom.NodeList
			Return getElementsByTagName(localName)
		End Function

		Public Overridable Function hasAttributes() As Boolean
			Return attributes.Count > 0
		End Function

		Public Overridable Function hasAttribute(ByVal name As String) As Boolean
			Return getAttributeNode(name) IsNot Nothing
		End Function

		''' <summary>
		''' Equivalent to <code>hasAttribute(localName)</code>.
		''' </summary>
		Public Overridable Function hasAttributeNS(ByVal namespaceURI As String, ByVal localName As String) As Boolean
			Return hasAttribute(localName)
		End Function

		' Methods from NodeList

		Public Overridable Property length As Integer
			Get
				Return numChildren
			End Get
		End Property

		Public Overridable Function item(ByVal index As Integer) As org.w3c.dom.Node
			If index < 0 Then Return Nothing

			Dim child As org.w3c.dom.Node = firstChild
			Dim tempVar As Boolean = child IsNot Nothing AndAlso index > 0
			index -= 1
			Do While tempVar
				child = child.nextSibling
				tempVar = child IsNot Nothing AndAlso index > 0
				index -= 1
			Loop
			Return child
		End Function

		''' <summary>
		''' Returns the <code>Object</code> value associated with this node.
		''' </summary>
		''' <returns> the user <code>Object</code>.
		''' </returns>
		''' <seealso cref= #setUserObject </seealso>
		Public Overridable Property userObject As Object
			Get
				Return userObject
			End Get
			Set(ByVal userObject As Object)
				Me.userObject = userObject
			End Set
		End Property


		' Start of dummy methods for DOM L3.

		''' <summary>
		''' This DOM Level 3 method is not supported for {@code IIOMetadataNode}
		''' and will throw a {@code DOMException}. </summary>
		''' <exception cref="DOMException"> - always. </exception>
		Public Overridable Sub setIdAttribute(ByVal name As String, ByVal isId As Boolean)
			Throw New org.w3c.dom.DOMException(org.w3c.dom.DOMException.NOT_SUPPORTED_ERR, "Method not supported")
		End Sub

		''' <summary>
		''' This DOM Level 3 method is not supported for {@code IIOMetadataNode}
		''' and will throw a {@code DOMException}. </summary>
		''' <exception cref="DOMException"> - always. </exception>
		Public Overridable Sub setIdAttributeNS(ByVal namespaceURI As String, ByVal localName As String, ByVal isId As Boolean)
			Throw New org.w3c.dom.DOMException(org.w3c.dom.DOMException.NOT_SUPPORTED_ERR, "Method not supported")
		End Sub

		''' <summary>
		''' This DOM Level 3 method is not supported for {@code IIOMetadataNode}
		''' and will throw a {@code DOMException}. </summary>
		''' <exception cref="DOMException"> - always. </exception>
		Public Overridable Sub setIdAttributeNode(ByVal idAttr As org.w3c.dom.Attr, ByVal isId As Boolean)
			Throw New org.w3c.dom.DOMException(org.w3c.dom.DOMException.NOT_SUPPORTED_ERR, "Method not supported")
		End Sub

		''' <summary>
		''' This DOM Level 3 method is not supported for {@code IIOMetadataNode}
		''' and will throw a {@code DOMException}. </summary>
		''' <exception cref="DOMException"> - always. </exception>
		Public Overridable Property schemaTypeInfo As org.w3c.dom.TypeInfo
			Get
				Throw New org.w3c.dom.DOMException(org.w3c.dom.DOMException.NOT_SUPPORTED_ERR, "Method not supported")
			End Get
		End Property

		''' <summary>
		''' This DOM Level 3 method is not supported for {@code IIOMetadataNode}
		''' and will throw a {@code DOMException}. </summary>
		''' <exception cref="DOMException"> - always. </exception>
		Public Overridable Function setUserData(ByVal key As String, ByVal data As Object, ByVal handler As org.w3c.dom.UserDataHandler) As Object
			Throw New org.w3c.dom.DOMException(org.w3c.dom.DOMException.NOT_SUPPORTED_ERR, "Method not supported")
		End Function

		''' <summary>
		''' This DOM Level 3 method is not supported for {@code IIOMetadataNode}
		''' and will throw a {@code DOMException}. </summary>
		''' <exception cref="DOMException"> - always. </exception>
		Public Overridable Function getUserData(ByVal key As String) As Object
			Throw New org.w3c.dom.DOMException(org.w3c.dom.DOMException.NOT_SUPPORTED_ERR, "Method not supported")
		End Function

		''' <summary>
		''' This DOM Level 3 method is not supported for {@code IIOMetadataNode}
		''' and will throw a {@code DOMException}. </summary>
		''' <exception cref="DOMException"> - always. </exception>
		Public Overridable Function getFeature(ByVal feature As String, ByVal version As String) As Object
			Throw New org.w3c.dom.DOMException(org.w3c.dom.DOMException.NOT_SUPPORTED_ERR, "Method not supported")
		End Function

		''' <summary>
		''' This DOM Level 3 method is not supported for {@code IIOMetadataNode}
		''' and will throw a {@code DOMException}. </summary>
		''' <exception cref="DOMException"> - always. </exception>
		Public Overridable Function isSameNode(ByVal node As org.w3c.dom.Node) As Boolean
			Throw New org.w3c.dom.DOMException(org.w3c.dom.DOMException.NOT_SUPPORTED_ERR, "Method not supported")
		End Function

		''' <summary>
		''' This DOM Level 3 method is not supported for {@code IIOMetadataNode}
		''' and will throw a {@code DOMException}. </summary>
		''' <exception cref="DOMException"> - always. </exception>
		Public Overridable Function isEqualNode(ByVal node As org.w3c.dom.Node) As Boolean
			Throw New org.w3c.dom.DOMException(org.w3c.dom.DOMException.NOT_SUPPORTED_ERR, "Method not supported")
		End Function

		''' <summary>
		''' This DOM Level 3 method is not supported for {@code IIOMetadataNode}
		''' and will throw a {@code DOMException}. </summary>
		''' <exception cref="DOMException"> - always. </exception>
		Public Overridable Function lookupNamespaceURI(ByVal prefix As String) As String
			Throw New org.w3c.dom.DOMException(org.w3c.dom.DOMException.NOT_SUPPORTED_ERR, "Method not supported")
		End Function

		''' <summary>
		''' This DOM Level 3 method is not supported for {@code IIOMetadataNode}
		''' and will throw a {@code DOMException}. </summary>
		''' <exception cref="DOMException"> - always. </exception>
		Public Overridable Function isDefaultNamespace(ByVal namespaceURI As String) As Boolean
			Throw New org.w3c.dom.DOMException(org.w3c.dom.DOMException.NOT_SUPPORTED_ERR, "Method not supported")
		End Function

		''' <summary>
		''' This DOM Level 3 method is not supported for {@code IIOMetadataNode}
		''' and will throw a {@code DOMException}. </summary>
		''' <exception cref="DOMException"> - always. </exception>
		Public Overridable Function lookupPrefix(ByVal namespaceURI As String) As String
			Throw New org.w3c.dom.DOMException(org.w3c.dom.DOMException.NOT_SUPPORTED_ERR, "Method not supported")
		End Function

		''' <summary>
		''' This DOM Level 3 method is not supported for {@code IIOMetadataNode}
		''' and will throw a {@code DOMException}. </summary>
		''' <exception cref="DOMException"> - always. </exception>
		Public Overridable Property textContent As String
			Get
				Throw New org.w3c.dom.DOMException(org.w3c.dom.DOMException.NOT_SUPPORTED_ERR, "Method not supported")
			End Get
			Set(ByVal textContent As String)
				Throw New org.w3c.dom.DOMException(org.w3c.dom.DOMException.NOT_SUPPORTED_ERR, "Method not supported")
			End Set
		End Property


		''' <summary>
		''' This DOM Level 3 method is not supported for {@code IIOMetadataNode}
		''' and will throw a {@code DOMException}. </summary>
		''' <exception cref="DOMException"> - always. </exception>
		Public Overridable Function compareDocumentPosition(ByVal other As org.w3c.dom.Node) As Short
			Throw New org.w3c.dom.DOMException(org.w3c.dom.DOMException.NOT_SUPPORTED_ERR, "Method not supported")
		End Function

		''' <summary>
		''' This DOM Level 3 method is not supported for {@code IIOMetadataNode}
		''' and will throw a {@code DOMException}. </summary>
		''' <exception cref="DOMException"> - always. </exception>
		Public Overridable Property baseURI As String
			Get
				Throw New org.w3c.dom.DOMException(org.w3c.dom.DOMException.NOT_SUPPORTED_ERR, "Method not supported")
			End Get
		End Property
		'End of dummy methods for DOM L3.


	End Class

End Namespace
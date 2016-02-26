Imports System
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.tree
	   ' ISSUE: this class depends on nothing in AWT -- move to java.util?



	''' <summary>
	''' A <code>DefaultMutableTreeNode</code> is a general-purpose node in a tree data
	''' structure.
	''' For examples of using default mutable tree nodes, see
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/tree.html">How to Use Trees</a>
	''' in <em>The Java Tutorial.</em>
	''' 
	''' <p>
	''' 
	''' A tree node may have at most one parent and 0 or more children.
	''' <code>DefaultMutableTreeNode</code> provides operations for examining and modifying a
	''' node's parent and children and also operations for examining the tree that
	''' the node is a part of.  A node's tree is the set of all nodes that can be
	''' reached by starting at the node and following all the possible links to
	''' parents and children.  A node with no parent is the root of its tree; a
	''' node with no children is a leaf.  A tree may consist of many subtrees,
	''' each node acting as the root for its own subtree.
	''' <p>
	''' This class provides enumerations for efficiently traversing a tree or
	''' subtree in various orders or for following the path between two nodes.
	''' A <code>DefaultMutableTreeNode</code> may also hold a reference to a user object, the
	''' use of which is left to the user.  Asking a <code>DefaultMutableTreeNode</code> for its
	''' string representation with <code>toString()</code> returns the string
	''' representation of its user object.
	''' <p>
	''' <b>This is not a thread safe class.</b>If you intend to use
	''' a DefaultMutableTreeNode (or a tree of TreeNodes) in more than one thread, you
	''' need to do your own synchronizing. A good convention to adopt is
	''' synchronizing on the root node of a tree.
	''' <p>
	''' While DefaultMutableTreeNode implements the MutableTreeNode interface and
	''' will allow you to add in any implementation of MutableTreeNode not all
	''' of the methods in DefaultMutableTreeNode will be applicable to all
	''' MutableTreeNodes implementations. Especially with some of the enumerations
	''' that are provided, using some of these methods assumes the
	''' DefaultMutableTreeNode contains only DefaultMutableNode instances. All
	''' of the TreeNode/MutableTreeNode methods will behave as defined no
	''' matter what implementations are added.
	''' 
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' </summary>
	''' <seealso cref= MutableTreeNode
	''' 
	''' @author Rob Davis </seealso>
	<Serializable> _
	Public Class DefaultMutableTreeNode
		Implements ICloneable, MutableTreeNode

		Private Const serialVersionUID As Long = -4298474751201349152L

		''' <summary>
		''' An enumeration that is always empty. This is used when an enumeration
		''' of a leaf node's children is requested.
		''' </summary>
		Public Shared ReadOnly EMPTY_ENUMERATION As System.Collections.IEnumerator(Of TreeNode) = Collections.emptyEnumeration()

		''' <summary>
		''' this node's parent, or null if this node has no parent </summary>
		Protected Friend parent As MutableTreeNode

		''' <summary>
		''' array of children, may be null if this node has no children </summary>
		Protected Friend ___children As ArrayList

		''' <summary>
		''' optional user object </summary>
		<NonSerialized> _
		Protected Friend userObject As Object

		''' <summary>
		''' true if the node is able to have children </summary>
		Protected Friend allowsChildren As Boolean


		''' <summary>
		''' Creates a tree node that has no parent and no children, but which
		''' allows children.
		''' </summary>
		Public Sub New()
			Me.New(Nothing)
		End Sub

		''' <summary>
		''' Creates a tree node with no parent, no children, but which allows
		''' children, and initializes it with the specified user object.
		''' </summary>
		''' <param name="userObject"> an Object provided by the user that constitutes
		'''                   the node's data </param>
		Public Sub New(ByVal userObject As Object)
			Me.New(userObject, True)
		End Sub

		''' <summary>
		''' Creates a tree node with no parent, no children, initialized with
		''' the specified user object, and that allows children only if
		''' specified.
		''' </summary>
		''' <param name="userObject"> an Object provided by the user that constitutes
		'''        the node's data </param>
		''' <param name="allowsChildren"> if true, the node is allowed to have child
		'''        nodes -- otherwise, it is always a leaf node </param>
		Public Sub New(ByVal userObject As Object, ByVal allowsChildren As Boolean)
			MyBase.New()
			parent = Nothing
			Me.allowsChildren = allowsChildren
			Me.userObject = userObject
		End Sub


		'
		'  Primitives
		'

		''' <summary>
		''' Removes <code>newChild</code> from its present parent (if it has a
		''' parent), sets the child's parent to this node, and then adds the child
		''' to this node's child array at index <code>childIndex</code>.
		''' <code>newChild</code> must not be null and must not be an ancestor of
		''' this node.
		''' </summary>
		''' <param name="newChild">        the MutableTreeNode to insert under this node </param>
		''' <param name="childIndex">      the index in this node's child array
		'''                          where this node is to be inserted </param>
		''' <exception cref="ArrayIndexOutOfBoundsException">  if
		'''                          <code>childIndex</code> is out of bounds </exception>
		''' <exception cref="IllegalArgumentException">        if
		'''                          <code>newChild</code> is null or is an
		'''                          ancestor of this node </exception>
		''' <exception cref="IllegalStateException">   if this node does not allow
		'''                                          children </exception>
		''' <seealso cref=     #isNodeDescendant </seealso>
		Public Overridable Sub insert(ByVal newChild As MutableTreeNode, ByVal childIndex As Integer) Implements MutableTreeNode.insert
			If Not allowsChildren Then
				Throw New IllegalStateException("node does not allow children")
			ElseIf newChild Is Nothing Then
				Throw New System.ArgumentException("new child is null")
			ElseIf isNodeAncestor(newChild) Then
				Throw New System.ArgumentException("new child is an ancestor")
			End If

				Dim oldParent As MutableTreeNode = CType(newChild.parent, MutableTreeNode)

				If oldParent IsNot Nothing Then oldParent.remove(newChild)
				newChild.parent = Me
				If ___children Is Nothing Then ___children = New ArrayList
				___children.Insert(childIndex, newChild)
		End Sub

		''' <summary>
		''' Removes the child at the specified index from this node's children
		''' and sets that node's parent to null. The child node to remove
		''' must be a <code>MutableTreeNode</code>.
		''' </summary>
		''' <param name="childIndex">      the index in this node's child array
		'''                          of the child to remove </param>
		''' <exception cref="ArrayIndexOutOfBoundsException">  if
		'''                          <code>childIndex</code> is out of bounds </exception>
		Public Overridable Sub remove(ByVal childIndex As Integer) Implements MutableTreeNode.remove
			Dim child As MutableTreeNode = CType(getChildAt(childIndex), MutableTreeNode)
			___children.RemoveAt(childIndex)
			child.parent = Nothing
		End Sub

		''' <summary>
		''' Sets this node's parent to <code>newParent</code> but does not
		''' change the parent's child array.  This method is called from
		''' <code>insert()</code> and <code>remove()</code> to
		''' reassign a child's parent, it should not be messaged from anywhere
		''' else.
		''' </summary>
		''' <param name="newParent">       this node's new parent </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property parent Implements MutableTreeNode.setParent As MutableTreeNode
			Set(ByVal newParent As MutableTreeNode)
				parent = newParent
			End Set
			Get
				Return parent
			End Get
		End Property


		''' <summary>
		''' Returns the child at the specified index in this node's child array.
		''' </summary>
		''' <param name="index">   an index into this node's child array </param>
		''' <exception cref="ArrayIndexOutOfBoundsException">  if <code>index</code>
		'''                                          is out of bounds </exception>
		''' <returns>  the TreeNode in this node's child array at  the specified index </returns>
		Public Overridable Function getChildAt(ByVal index As Integer) As TreeNode Implements TreeNode.getChildAt
			If ___children Is Nothing Then Throw New System.IndexOutOfRangeException("node has no children")
			Return CType(___children(index), TreeNode)
		End Function

		''' <summary>
		''' Returns the number of children of this node.
		''' </summary>
		''' <returns>  an int giving the number of children of this node </returns>
		Public Overridable Property childCount As Integer Implements TreeNode.getChildCount
			Get
				If ___children Is Nothing Then
					Return 0
				Else
					Return ___children.Count
				End If
			End Get
		End Property

		''' <summary>
		''' Returns the index of the specified child in this node's child array.
		''' If the specified node is not a child of this node, returns
		''' <code>-1</code>.  This method performs a linear search and is O(n)
		''' where n is the number of children.
		''' </summary>
		''' <param name="aChild">  the TreeNode to search for among this node's children </param>
		''' <exception cref="IllegalArgumentException">        if <code>aChild</code>
		'''                                                  is null </exception>
		''' <returns>  an int giving the index of the node in this node's child
		'''          array, or <code>-1</code> if the specified node is a not
		'''          a child of this node </returns>
		Public Overridable Function getIndex(ByVal aChild As TreeNode) As Integer Implements TreeNode.getIndex
			If aChild Is Nothing Then Throw New System.ArgumentException("argument is null")

			If Not isNodeChild(aChild) Then Return -1
			Return ___children.IndexOf(aChild) ' linear search
		End Function

		''' <summary>
		''' Creates and returns a forward-order enumeration of this node's
		''' children.  Modifying this node's child array invalidates any child
		''' enumerations created before the modification.
		''' </summary>
		''' <returns>  an Enumeration of this node's children </returns>
		Public Overridable Function children() As System.Collections.IEnumerator
			If ___children Is Nothing Then
				Return EMPTY_ENUMERATION
			Else
				Return ___children.elements()
			End If
		End Function

		''' <summary>
		''' Determines whether or not this node is allowed to have children.
		''' If <code>allows</code> is false, all of this node's children are
		''' removed.
		''' <p>
		''' Note: By default, a node allows children.
		''' </summary>
		''' <param name="allows">  true if this node is allowed to have children </param>
		Public Overridable Property allowsChildren As Boolean
			Set(ByVal allows As Boolean)
				If allows <> allowsChildren Then
					allowsChildren = allows
					If Not allowsChildren Then removeAllChildren()
				End If
			End Set
			Get
				Return allowsChildren
			End Get
		End Property


		''' <summary>
		''' Sets the user object for this node to <code>userObject</code>.
		''' </summary>
		''' <param name="userObject">      the Object that constitutes this node's
		'''                          user-specified data </param>
		''' <seealso cref=     #getUserObject </seealso>
		''' <seealso cref=     #toString </seealso>
		Public Overridable Property userObject Implements MutableTreeNode.setUserObject As Object
			Set(ByVal userObject As Object)
				Me.userObject = userObject
			End Set
			Get
				Return userObject
			End Get
		End Property



		'
		'  Derived methods
		'

		''' <summary>
		''' Removes the subtree rooted at this node from the tree, giving this
		''' node a null parent.  Does nothing if this node is the root of its
		''' tree.
		''' </summary>
		Public Overridable Sub removeFromParent() Implements MutableTreeNode.removeFromParent
			Dim ___parent As MutableTreeNode = CType(parent, MutableTreeNode)
			If ___parent IsNot Nothing Then ___parent.remove(Me)
		End Sub

		''' <summary>
		''' Removes <code>aChild</code> from this node's child array, giving it a
		''' null parent.
		''' </summary>
		''' <param name="aChild">  a child of this node to remove </param>
		''' <exception cref="IllegalArgumentException">        if <code>aChild</code>
		'''                                  is null or is not a child of this node </exception>
		Public Overridable Sub remove(ByVal aChild As MutableTreeNode) Implements MutableTreeNode.remove
			If aChild Is Nothing Then Throw New System.ArgumentException("argument is null")

			If Not isNodeChild(aChild) Then Throw New System.ArgumentException("argument is not a child")
			remove(getIndex(aChild)) ' linear search
		End Sub

		''' <summary>
		''' Removes all of this node's children, setting their parents to null.
		''' If this node has no children, this method does nothing.
		''' </summary>
		Public Overridable Sub removeAllChildren()
			For i As Integer = childCount-1 To 0 Step -1
				remove(i)
			Next i
		End Sub

		''' <summary>
		''' Removes <code>newChild</code> from its parent and makes it a child of
		''' this node by adding it to the end of this node's child array.
		''' </summary>
		''' <seealso cref=             #insert </seealso>
		''' <param name="newChild">        node to add as a child of this node </param>
		''' <exception cref="IllegalArgumentException">    if <code>newChild</code>
		'''                                          is null </exception>
		''' <exception cref="IllegalStateException">   if this node does not allow
		'''                                          children </exception>
		Public Overridable Sub add(ByVal newChild As MutableTreeNode)
			If newChild IsNot Nothing AndAlso newChild.parent Is Me Then
				insert(newChild, childCount - 1)
			Else
				insert(newChild, childCount)
			End If
		End Sub



		'
		'  Tree Queries
		'

		''' <summary>
		''' Returns true if <code>anotherNode</code> is an ancestor of this node
		''' -- if it is this node, this node's parent, or an ancestor of this
		''' node's parent.  (Note that a node is considered an ancestor of itself.)
		''' If <code>anotherNode</code> is null, this method returns false.  This
		''' operation is at worst O(h) where h is the distance from the root to
		''' this node.
		''' </summary>
		''' <seealso cref=             #isNodeDescendant </seealso>
		''' <seealso cref=             #getSharedAncestor </seealso>
		''' <param name="anotherNode">     node to test as an ancestor of this node </param>
		''' <returns>  true if this node is a descendant of <code>anotherNode</code> </returns>
		Public Overridable Function isNodeAncestor(ByVal anotherNode As TreeNode) As Boolean
			If anotherNode Is Nothing Then Return False

			Dim ancestor As TreeNode = Me

			Do
				If ancestor Is anotherNode Then Return True
				ancestor = ancestor.parent
			Loop While ancestor IsNot Nothing

			Return False
		End Function

		''' <summary>
		''' Returns true if <code>anotherNode</code> is a descendant of this node
		''' -- if it is this node, one of this node's children, or a descendant of
		''' one of this node's children.  Note that a node is considered a
		''' descendant of itself.  If <code>anotherNode</code> is null, returns
		''' false.  This operation is at worst O(h) where h is the distance from the
		''' root to <code>anotherNode</code>.
		''' </summary>
		''' <seealso cref=     #isNodeAncestor </seealso>
		''' <seealso cref=     #getSharedAncestor </seealso>
		''' <param name="anotherNode">     node to test as descendant of this node </param>
		''' <returns>  true if this node is an ancestor of <code>anotherNode</code> </returns>
		Public Overridable Function isNodeDescendant(ByVal anotherNode As DefaultMutableTreeNode) As Boolean
			If anotherNode Is Nothing Then Return False

			Return anotherNode.isNodeAncestor(Me)
		End Function

		''' <summary>
		''' Returns the nearest common ancestor to this node and <code>aNode</code>.
		''' Returns null, if no such ancestor exists -- if this node and
		''' <code>aNode</code> are in different trees or if <code>aNode</code> is
		''' null.  A node is considered an ancestor of itself.
		''' </summary>
		''' <seealso cref=     #isNodeAncestor </seealso>
		''' <seealso cref=     #isNodeDescendant </seealso>
		''' <param name="aNode">   node to find common ancestor with </param>
		''' <returns>  nearest ancestor common to this node and <code>aNode</code>,
		'''          or null if none </returns>
		Public Overridable Function getSharedAncestor(ByVal aNode As DefaultMutableTreeNode) As TreeNode
			If aNode Is Me Then
				Return Me
			ElseIf aNode Is Nothing Then
				Return Nothing
			End If

			Dim level1, level2, diff As Integer
			Dim node1, node2 As TreeNode

			level1 = level
			level2 = aNode.level

			If level2 > level1 Then
				diff = level2 - level1
				node1 = aNode
				node2 = Me
			Else
				diff = level1 - level2
				node1 = Me
				node2 = aNode
			End If

			' Go up the tree until the nodes are at the same level
			Do While diff > 0
				node1 = node1.parent
				diff -= 1
			Loop

			' Move up the tree until we find a common ancestor.  Since we know
			' that both nodes are at the same level, we won't cross paths
			' unknowingly (if there is a common ancestor, both nodes hit it in
			' the same iteration).

			Do
				If node1 Is node2 Then Return node1
				node1 = node1.parent
				node2 = node2.parent
			Loop While node1 IsNot Nothing ' only need to check one -- they're at the
			' same level so if one is null, the other is

			If node1 IsNot Nothing OrElse node2 IsNot Nothing Then Throw New Exception("nodes should be null")

			Return Nothing
		End Function


		''' <summary>
		''' Returns true if and only if <code>aNode</code> is in the same tree
		''' as this node.  Returns false if <code>aNode</code> is null.
		''' </summary>
		''' <seealso cref=     #getSharedAncestor </seealso>
		''' <seealso cref=     #getRoot </seealso>
		''' <returns>  true if <code>aNode</code> is in the same tree as this node;
		'''          false if <code>aNode</code> is null </returns>
		Public Overridable Function isNodeRelated(ByVal aNode As DefaultMutableTreeNode) As Boolean
			Return (aNode IsNot Nothing) AndAlso (root Is aNode.root)
		End Function


		''' <summary>
		''' Returns the depth of the tree rooted at this node -- the longest
		''' distance from this node to a leaf.  If this node has no children,
		''' returns 0.  This operation is much more expensive than
		''' <code>getLevel()</code> because it must effectively traverse the entire
		''' tree rooted at this node.
		''' </summary>
		''' <seealso cref=     #getLevel </seealso>
		''' <returns>  the depth of the tree whose root is this node </returns>
		Public Overridable Property depth As Integer
			Get
				Dim last As Object = Nothing
				Dim enum_ As System.Collections.IEnumerator = breadthFirstEnumeration()
    
				Do While enum_.hasMoreElements()
					last = enum_.nextElement()
				Loop
    
				If last Is Nothing Then Throw New Exception("nodes should be null")
    
				Return CType(last, DefaultMutableTreeNode).level - level
			End Get
		End Property



		''' <summary>
		''' Returns the number of levels above this node -- the distance from
		''' the root to this node.  If this node is the root, returns 0.
		''' </summary>
		''' <seealso cref=     #getDepth </seealso>
		''' <returns>  the number of levels above this node </returns>
		Public Overridable Property level As Integer
			Get
				Dim ancestor As TreeNode
				Dim levels As Integer = 0
    
				ancestor = Me
				ancestor = ancestor.parent
				Do While ancestor IsNot Nothing
					levels += 1
					ancestor = ancestor.parent
				Loop
    
				Return levels
			End Get
		End Property


		''' <summary>
		''' Returns the path from the root, to get to this node.  The last
		''' element in the path is this node.
		''' </summary>
		''' <returns> an array of TreeNode objects giving the path, where the
		'''         first element in the path is the root and the last
		'''         element is this node. </returns>
		Public Overridable Property path As TreeNode()
			Get
				Return getPathToRoot(Me, 0)
			End Get
		End Property

		''' <summary>
		''' Builds the parents of node up to and including the root node,
		''' where the original node is the last element in the returned array.
		''' The length of the returned array gives the node's depth in the
		''' tree.
		''' </summary>
		''' <param name="aNode">  the TreeNode to get the path for </param>
		''' <param name="depth">  an int giving the number of steps already taken towards
		'''        the root (on recursive calls), used to size the returned array </param>
		''' <returns> an array of TreeNodes giving the path from the root to the
		'''         specified node </returns>
		Protected Friend Overridable Function getPathToRoot(ByVal aNode As TreeNode, ByVal depth As Integer) As TreeNode()
			Dim retNodes As TreeNode()

	'         Check for null, in case someone passed in a null node, or
	'           they passed in an element that isn't rooted at root. 
			If aNode Is Nothing Then
				If depth = 0 Then
					Return Nothing
				Else
					retNodes = New TreeNode(depth - 1){}
				End If
			Else
				depth += 1
				retNodes = getPathToRoot(aNode.parent, depth)
				retNodes(retNodes.Length - depth) = aNode
			End If
			Return retNodes
		End Function

		''' <summary>
		''' Returns the user object path, from the root, to get to this node.
		''' If some of the TreeNodes in the path have null user objects, the
		''' returned path will contain nulls.
		''' </summary>
		Public Overridable Property userObjectPath As Object()
			Get
				Dim realPath As TreeNode() = path
				Dim retPath As Object() = New Object(realPath.Length - 1){}
    
				For counter As Integer = 0 To realPath.Length - 1
					retPath(counter) = CType(realPath(counter), DefaultMutableTreeNode).userObject
				Next counter
				Return retPath
			End Get
		End Property

		''' <summary>
		''' Returns the root of the tree that contains this node.  The root is
		''' the ancestor with a null parent.
		''' </summary>
		''' <seealso cref=     #isNodeAncestor </seealso>
		''' <returns>  the root of the tree that contains this node </returns>
		Public Overridable Property root As TreeNode
			Get
				Dim ancestor As TreeNode = Me
				Dim previous As TreeNode
    
				Do
					previous = ancestor
					ancestor = ancestor.parent
				Loop While ancestor IsNot Nothing
    
				Return previous
			End Get
		End Property


		''' <summary>
		''' Returns true if this node is the root of the tree.  The root is
		''' the only node in the tree with a null parent; every tree has exactly
		''' one root.
		''' </summary>
		''' <returns>  true if this node is the root of its tree </returns>
		Public Overridable Property root As Boolean
			Get
				Return parent Is Nothing
			End Get
		End Property


		''' <summary>
		''' Returns the node that follows this node in a preorder traversal of this
		''' node's tree.  Returns null if this node is the last node of the
		''' traversal.  This is an inefficient way to traverse the entire tree; use
		''' an enumeration, instead.
		''' </summary>
		''' <seealso cref=     #preorderEnumeration </seealso>
		''' <returns>  the node that follows this node in a preorder traversal, or
		'''          null if this node is last </returns>
		Public Overridable Property nextNode As DefaultMutableTreeNode
			Get
				If childCount = 0 Then
					' No children, so look for nextSibling
					Dim ___nextSibling As DefaultMutableTreeNode = nextSibling
    
					If ___nextSibling Is Nothing Then
						Dim aNode As DefaultMutableTreeNode = CType(parent, DefaultMutableTreeNode)
    
						Do
							If aNode Is Nothing Then Return Nothing
    
							___nextSibling = aNode.nextSibling
							If ___nextSibling IsNot Nothing Then Return ___nextSibling
    
							aNode = CType(aNode.parent, DefaultMutableTreeNode)
						Loop While True
					Else
						Return ___nextSibling
					End If
				Else
					Return CType(getChildAt(0), DefaultMutableTreeNode)
				End If
			End Get
		End Property


		''' <summary>
		''' Returns the node that precedes this node in a preorder traversal of
		''' this node's tree.  Returns <code>null</code> if this node is the
		''' first node of the traversal -- the root of the tree.
		''' This is an inefficient way to
		''' traverse the entire tree; use an enumeration, instead.
		''' </summary>
		''' <seealso cref=     #preorderEnumeration </seealso>
		''' <returns>  the node that precedes this node in a preorder traversal, or
		'''          null if this node is the first </returns>
		Public Overridable Property previousNode As DefaultMutableTreeNode
			Get
				Dim ___previousSibling As DefaultMutableTreeNode
				Dim myParent As DefaultMutableTreeNode = CType(parent, DefaultMutableTreeNode)
    
				If myParent Is Nothing Then Return Nothing
    
				___previousSibling = previousSibling
    
				If ___previousSibling IsNot Nothing Then
					If ___previousSibling.childCount = 0 Then
						Return ___previousSibling
					Else
						Return ___previousSibling.lastLeaf
					End If
				Else
					Return myParent
				End If
			End Get
		End Property

		''' <summary>
		''' Creates and returns an enumeration that traverses the subtree rooted at
		''' this node in preorder.  The first node returned by the enumeration's
		''' <code>nextElement()</code> method is this node.<P>
		''' 
		''' Modifying the tree by inserting, removing, or moving a node invalidates
		''' any enumerations created before the modification.
		''' </summary>
		''' <seealso cref=     #postorderEnumeration </seealso>
		''' <returns>  an enumeration for traversing the tree in preorder </returns>
		Public Overridable Function preorderEnumeration() As System.Collections.IEnumerator
			Return New PreorderEnumeration(Me, Me)
		End Function

		''' <summary>
		''' Creates and returns an enumeration that traverses the subtree rooted at
		''' this node in postorder.  The first node returned by the enumeration's
		''' <code>nextElement()</code> method is the leftmost leaf.  This is the
		''' same as a depth-first traversal.<P>
		''' 
		''' Modifying the tree by inserting, removing, or moving a node invalidates
		''' any enumerations created before the modification.
		''' </summary>
		''' <seealso cref=     #depthFirstEnumeration </seealso>
		''' <seealso cref=     #preorderEnumeration </seealso>
		''' <returns>  an enumeration for traversing the tree in postorder </returns>
		Public Overridable Function postorderEnumeration() As System.Collections.IEnumerator
			Return New PostorderEnumeration(Me, Me)
		End Function

		''' <summary>
		''' Creates and returns an enumeration that traverses the subtree rooted at
		''' this node in breadth-first order.  The first node returned by the
		''' enumeration's <code>nextElement()</code> method is this node.<P>
		''' 
		''' Modifying the tree by inserting, removing, or moving a node invalidates
		''' any enumerations created before the modification.
		''' </summary>
		''' <seealso cref=     #depthFirstEnumeration </seealso>
		''' <returns>  an enumeration for traversing the tree in breadth-first order </returns>
		Public Overridable Function breadthFirstEnumeration() As System.Collections.IEnumerator
			Return New BreadthFirstEnumeration(Me, Me)
		End Function

		''' <summary>
		''' Creates and returns an enumeration that traverses the subtree rooted at
		''' this node in depth-first order.  The first node returned by the
		''' enumeration's <code>nextElement()</code> method is the leftmost leaf.
		''' This is the same as a postorder traversal.<P>
		''' 
		''' Modifying the tree by inserting, removing, or moving a node invalidates
		''' any enumerations created before the modification.
		''' </summary>
		''' <seealso cref=     #breadthFirstEnumeration </seealso>
		''' <seealso cref=     #postorderEnumeration </seealso>
		''' <returns>  an enumeration for traversing the tree in depth-first order </returns>
		Public Overridable Function depthFirstEnumeration() As System.Collections.IEnumerator
			Return postorderEnumeration()
		End Function

		''' <summary>
		''' Creates and returns an enumeration that follows the path from
		''' <code>ancestor</code> to this node.  The enumeration's
		''' <code>nextElement()</code> method first returns <code>ancestor</code>,
		''' then the child of <code>ancestor</code> that is an ancestor of this
		''' node, and so on, and finally returns this node.  Creation of the
		''' enumeration is O(m) where m is the number of nodes between this node
		''' and <code>ancestor</code>, inclusive.  Each <code>nextElement()</code>
		''' message is O(1).<P>
		''' 
		''' Modifying the tree by inserting, removing, or moving a node invalidates
		''' any enumerations created before the modification.
		''' </summary>
		''' <seealso cref=             #isNodeAncestor </seealso>
		''' <seealso cref=             #isNodeDescendant </seealso>
		''' <exception cref="IllegalArgumentException"> if <code>ancestor</code> is
		'''                                          not an ancestor of this node </exception>
		''' <returns>  an enumeration for following the path from an ancestor of
		'''          this node to this one </returns>
		Public Overridable Function pathFromAncestorEnumeration(ByVal ancestor As TreeNode) As System.Collections.IEnumerator
			Return New PathBetweenNodesEnumeration(Me, ancestor, Me)
		End Function


		'
		'  Child Queries
		'

		''' <summary>
		''' Returns true if <code>aNode</code> is a child of this node.  If
		''' <code>aNode</code> is null, this method returns false.
		''' </summary>
		''' <returns>  true if <code>aNode</code> is a child of this node; false if
		'''                  <code>aNode</code> is null </returns>
		Public Overridable Function isNodeChild(ByVal aNode As TreeNode) As Boolean
			Dim retval As Boolean

			If aNode Is Nothing Then
				retval = False
			Else
				If childCount = 0 Then
					retval = False
				Else
					retval = (aNode.parent Is Me)
				End If
			End If

			Return retval
		End Function


		''' <summary>
		''' Returns this node's first child.  If this node has no children,
		''' throws NoSuchElementException.
		''' </summary>
		''' <returns>  the first child of this node </returns>
		''' <exception cref="NoSuchElementException">  if this node has no children </exception>
		Public Overridable Property firstChild As TreeNode
			Get
				If childCount = 0 Then Throw New NoSuchElementException("node has no children")
				Return getChildAt(0)
			End Get
		End Property


		''' <summary>
		''' Returns this node's last child.  If this node has no children,
		''' throws NoSuchElementException.
		''' </summary>
		''' <returns>  the last child of this node </returns>
		''' <exception cref="NoSuchElementException">  if this node has no children </exception>
		Public Overridable Property lastChild As TreeNode
			Get
				If childCount = 0 Then Throw New NoSuchElementException("node has no children")
				Return getChildAt(childCount-1)
			End Get
		End Property


		''' <summary>
		''' Returns the child in this node's child array that immediately
		''' follows <code>aChild</code>, which must be a child of this node.  If
		''' <code>aChild</code> is the last child, returns null.  This method
		''' performs a linear search of this node's children for
		''' <code>aChild</code> and is O(n) where n is the number of children; to
		''' traverse the entire array of children, use an enumeration instead.
		''' </summary>
		''' <seealso cref=             #children </seealso>
		''' <exception cref="IllegalArgumentException"> if <code>aChild</code> is
		'''                                  null or is not a child of this node </exception>
		''' <returns>  the child of this node that immediately follows
		'''          <code>aChild</code> </returns>
		Public Overridable Function getChildAfter(ByVal aChild As TreeNode) As TreeNode
			If aChild Is Nothing Then Throw New System.ArgumentException("argument is null")

			Dim ___index As Integer = getIndex(aChild) ' linear search

			If ___index = -1 Then Throw New System.ArgumentException("node is not a child")

			If ___index < childCount - 1 Then
				Return getChildAt(___index + 1)
			Else
				Return Nothing
			End If
		End Function


		''' <summary>
		''' Returns the child in this node's child array that immediately
		''' precedes <code>aChild</code>, which must be a child of this node.  If
		''' <code>aChild</code> is the first child, returns null.  This method
		''' performs a linear search of this node's children for <code>aChild</code>
		''' and is O(n) where n is the number of children.
		''' </summary>
		''' <exception cref="IllegalArgumentException"> if <code>aChild</code> is null
		'''                                          or is not a child of this node </exception>
		''' <returns>  the child of this node that immediately precedes
		'''          <code>aChild</code> </returns>
		Public Overridable Function getChildBefore(ByVal aChild As TreeNode) As TreeNode
			If aChild Is Nothing Then Throw New System.ArgumentException("argument is null")

			Dim ___index As Integer = getIndex(aChild) ' linear search

			If ___index = -1 Then Throw New System.ArgumentException("argument is not a child")

			If ___index > 0 Then
				Return getChildAt(___index - 1)
			Else
				Return Nothing
			End If
		End Function


		'
		'  Sibling Queries
		'


		''' <summary>
		''' Returns true if <code>anotherNode</code> is a sibling of (has the
		''' same parent as) this node.  A node is its own sibling.  If
		''' <code>anotherNode</code> is null, returns false.
		''' </summary>
		''' <param name="anotherNode">     node to test as sibling of this node </param>
		''' <returns>  true if <code>anotherNode</code> is a sibling of this node </returns>
		Public Overridable Function isNodeSibling(ByVal anotherNode As TreeNode) As Boolean
			Dim retval As Boolean

			If anotherNode Is Nothing Then
				retval = False
			ElseIf anotherNode Is Me Then
				retval = True
			Else
				Dim myParent As TreeNode = parent
				retval = (myParent IsNot Nothing AndAlso myParent Is anotherNode.parent)

				If retval AndAlso (Not CType(parent, DefaultMutableTreeNode).isNodeChild(anotherNode)) Then Throw New Exception("sibling has different parent")
			End If

			Return retval
		End Function


		''' <summary>
		''' Returns the number of siblings of this node.  A node is its own sibling
		''' (if it has no parent or no siblings, this method returns
		''' <code>1</code>).
		''' </summary>
		''' <returns>  the number of siblings of this node </returns>
		Public Overridable Property siblingCount As Integer
			Get
				Dim myParent As TreeNode = parent
    
				If myParent Is Nothing Then
					Return 1
				Else
					Return myParent.childCount
				End If
			End Get
		End Property


		''' <summary>
		''' Returns the next sibling of this node in the parent's children array.
		''' Returns null if this node has no parent or is the parent's last child.
		''' This method performs a linear search that is O(n) where n is the number
		''' of children; to traverse the entire array, use the parent's child
		''' enumeration instead.
		''' </summary>
		''' <seealso cref=     #children </seealso>
		''' <returns>  the sibling of this node that immediately follows this node </returns>
		Public Overridable Property nextSibling As DefaultMutableTreeNode
			Get
				Dim retval As DefaultMutableTreeNode
    
				Dim myParent As DefaultMutableTreeNode = CType(parent, DefaultMutableTreeNode)
    
				If myParent Is Nothing Then
					retval = Nothing
				Else
					retval = CType(myParent.getChildAfter(Me), DefaultMutableTreeNode) ' linear search
				End If
    
				If retval IsNot Nothing AndAlso (Not isNodeSibling(retval)) Then Throw New Exception("child of parent is not a sibling")
    
				Return retval
			End Get
		End Property


		''' <summary>
		''' Returns the previous sibling of this node in the parent's children
		''' array.  Returns null if this node has no parent or is the parent's
		''' first child.  This method performs a linear search that is O(n) where n
		''' is the number of children.
		''' </summary>
		''' <returns>  the sibling of this node that immediately precedes this node </returns>
		Public Overridable Property previousSibling As DefaultMutableTreeNode
			Get
				Dim retval As DefaultMutableTreeNode
    
				Dim myParent As DefaultMutableTreeNode = CType(parent, DefaultMutableTreeNode)
    
				If myParent Is Nothing Then
					retval = Nothing
				Else
					retval = CType(myParent.getChildBefore(Me), DefaultMutableTreeNode) ' linear search
				End If
    
				If retval IsNot Nothing AndAlso (Not isNodeSibling(retval)) Then Throw New Exception("child of parent is not a sibling")
    
				Return retval
			End Get
		End Property



		'
		'  Leaf Queries
		'

		''' <summary>
		''' Returns true if this node has no children.  To distinguish between
		''' nodes that have no children and nodes that <i>cannot</i> have
		''' children (e.g. to distinguish files from empty directories), use this
		''' method in conjunction with <code>getAllowsChildren</code>
		''' </summary>
		''' <seealso cref=     #getAllowsChildren </seealso>
		''' <returns>  true if this node has no children </returns>
		Public Overridable Property leaf As Boolean Implements TreeNode.isLeaf
			Get
				Return (childCount = 0)
			End Get
		End Property


		''' <summary>
		''' Finds and returns the first leaf that is a descendant of this node --
		''' either this node or its first child's first leaf.
		''' Returns this node if it is a leaf.
		''' </summary>
		''' <seealso cref=     #isLeaf </seealso>
		''' <seealso cref=     #isNodeDescendant </seealso>
		''' <returns>  the first leaf in the subtree rooted at this node </returns>
		Public Overridable Property firstLeaf As DefaultMutableTreeNode
			Get
				Dim node As DefaultMutableTreeNode = Me
    
				Do While Not node.leaf
					node = CType(node.firstChild, DefaultMutableTreeNode)
				Loop
    
				Return node
			End Get
		End Property


		''' <summary>
		''' Finds and returns the last leaf that is a descendant of this node --
		''' either this node or its last child's last leaf.
		''' Returns this node if it is a leaf.
		''' </summary>
		''' <seealso cref=     #isLeaf </seealso>
		''' <seealso cref=     #isNodeDescendant </seealso>
		''' <returns>  the last leaf in the subtree rooted at this node </returns>
		Public Overridable Property lastLeaf As DefaultMutableTreeNode
			Get
				Dim node As DefaultMutableTreeNode = Me
    
				Do While Not node.leaf
					node = CType(node.lastChild, DefaultMutableTreeNode)
				Loop
    
				Return node
			End Get
		End Property


		''' <summary>
		''' Returns the leaf after this node or null if this node is the
		''' last leaf in the tree.
		''' <p>
		''' In this implementation of the <code>MutableNode</code> interface,
		''' this operation is very inefficient. In order to determine the
		''' next node, this method first performs a linear search in the
		''' parent's child-list in order to find the current node.
		''' <p>
		''' That implementation makes the operation suitable for short
		''' traversals from a known position. But to traverse all of the
		''' leaves in the tree, you should use <code>depthFirstEnumeration</code>
		''' to enumerate the nodes in the tree and use <code>isLeaf</code>
		''' on each node to determine which are leaves.
		''' </summary>
		''' <seealso cref=     #depthFirstEnumeration </seealso>
		''' <seealso cref=     #isLeaf </seealso>
		''' <returns>  returns the next leaf past this node </returns>
		Public Overridable Property nextLeaf As DefaultMutableTreeNode
			Get
				Dim ___nextSibling As DefaultMutableTreeNode
				Dim myParent As DefaultMutableTreeNode = CType(parent, DefaultMutableTreeNode)
    
				If myParent Is Nothing Then Return Nothing
    
				___nextSibling = nextSibling ' linear search
    
				If ___nextSibling IsNot Nothing Then Return ___nextSibling.firstLeaf
    
				Return myParent.nextLeaf ' tail recursion
			End Get
		End Property


		''' <summary>
		''' Returns the leaf before this node or null if this node is the
		''' first leaf in the tree.
		''' <p>
		''' In this implementation of the <code>MutableNode</code> interface,
		''' this operation is very inefficient. In order to determine the
		''' previous node, this method first performs a linear search in the
		''' parent's child-list in order to find the current node.
		''' <p>
		''' That implementation makes the operation suitable for short
		''' traversals from a known position. But to traverse all of the
		''' leaves in the tree, you should use <code>depthFirstEnumeration</code>
		''' to enumerate the nodes in the tree and use <code>isLeaf</code>
		''' on each node to determine which are leaves.
		''' </summary>
		''' <seealso cref=             #depthFirstEnumeration </seealso>
		''' <seealso cref=             #isLeaf </seealso>
		''' <returns>  returns the leaf before this node </returns>
		Public Overridable Property previousLeaf As DefaultMutableTreeNode
			Get
				Dim ___previousSibling As DefaultMutableTreeNode
				Dim myParent As DefaultMutableTreeNode = CType(parent, DefaultMutableTreeNode)
    
				If myParent Is Nothing Then Return Nothing
    
				___previousSibling = previousSibling ' linear search
    
				If ___previousSibling IsNot Nothing Then Return ___previousSibling.lastLeaf
    
				Return myParent.previousLeaf ' tail recursion
			End Get
		End Property


		''' <summary>
		''' Returns the total number of leaves that are descendants of this node.
		''' If this node is a leaf, returns <code>1</code>.  This method is O(n)
		''' where n is the number of descendants of this node.
		''' </summary>
		''' <seealso cref=     #isNodeAncestor </seealso>
		''' <returns>  the number of leaves beneath this node </returns>
		Public Overridable Property leafCount As Integer
			Get
				Dim count As Integer = 0
    
				Dim node As TreeNode
				Dim enum_ As System.Collections.IEnumerator = breadthFirstEnumeration() ' order matters not
    
				Do While enum_.hasMoreElements()
					node = CType(enum_.nextElement(), TreeNode)
					If node.leaf Then count += 1
				Loop
    
				If count < 1 Then Throw New Exception("tree has zero leaves")
    
				Return count
			End Get
		End Property


		'
		'  Overrides
		'

		''' <summary>
		''' Returns the result of sending <code>toString()</code> to this node's
		''' user object, or the empty string if the node has no user object.
		''' </summary>
		''' <seealso cref=     #getUserObject </seealso>
		Public Overrides Function ToString() As String
			If userObject Is Nothing Then
				Return ""
			Else
				Return userObject.ToString()
			End If
		End Function

		''' <summary>
		''' Overridden to make clone public.  Returns a shallow copy of this node;
		''' the new node has no parent or children and has a reference to the same
		''' user object, if any.
		''' </summary>
		''' <returns>  a copy of this node </returns>
		Public Overridable Function clone() As Object
			Dim newNode As DefaultMutableTreeNode

			Try
				newNode = CType(MyBase.clone(), DefaultMutableTreeNode)

				' shallow copy -- the new node has no parent or children
				newNode.___children = Nothing
				newNode.parent = Nothing

			Catch e As CloneNotSupportedException
				' Won't happen because we implement Cloneable
				Throw New Exception(e.ToString())
			End Try

			Return newNode
		End Function


		' Serialization support.
		Private Sub writeObject(ByVal s As ObjectOutputStream)
			Dim tValues As Object()

			s.defaultWriteObject()
			' Save the userObject, if its Serializable.
			If userObject IsNot Nothing AndAlso TypeOf userObject Is Serializable Then
				tValues = New Object(1){}
				tValues(0) = "userObject"
				tValues(1) = userObject
			Else
				tValues = New Object(){}
			End If
			s.writeObject(tValues)
		End Sub

		Private Sub readObject(ByVal s As ObjectInputStream)
			Dim tValues As Object()

			s.defaultReadObject()

			tValues = CType(s.readObject(), Object())

			If tValues.Length > 0 AndAlso tValues(0).Equals("userObject") Then userObject = tValues(1)
		End Sub

		Private NotInheritable Class PreorderEnumeration
			Implements System.Collections.IEnumerator(Of TreeNode)

			Private ReadOnly outerInstance As DefaultMutableTreeNode

			Private ReadOnly stack As New Stack(Of System.Collections.IEnumerator)

			Public Sub New(ByVal outerInstance As DefaultMutableTreeNode, ByVal rootNode As TreeNode)
					Me.outerInstance = outerInstance
				MyBase.New()
				Dim v As New List(Of TreeNode)(1)
				v.Add(rootNode) ' PENDING: don't really need a vector
				stack.Push(v.elements())
			End Sub

			Public Function hasMoreElements() As Boolean
				Return (stack.Count > 0 AndAlso stack.Peek().hasMoreElements())
			End Function

			Public Function nextElement() As TreeNode
				Dim enumer As System.Collections.IEnumerator = stack.Peek()
				Dim node As TreeNode = CType(enumer.nextElement(), TreeNode)
				Dim children As System.Collections.IEnumerator = node.children()

				If Not enumer.hasMoreElements() Then stack.Pop()
				If children.hasMoreElements() Then stack.Push(children)
				Return node
			End Function

		End Class ' End of class PreorderEnumeration



		Friend NotInheritable Class PostorderEnumeration
			Implements System.Collections.IEnumerator(Of TreeNode)

			Private ReadOnly outerInstance As DefaultMutableTreeNode

			Protected Friend root As TreeNode
			Protected Friend children As System.Collections.IEnumerator(Of TreeNode)
			Protected Friend subtree As System.Collections.IEnumerator(Of TreeNode)

			Public Sub New(ByVal outerInstance As DefaultMutableTreeNode, ByVal rootNode As TreeNode)
					Me.outerInstance = outerInstance
				MyBase.New()
				root = rootNode
				children = root.children()
				subtree = EMPTY_ENUMERATION
			End Sub

			Public Function hasMoreElements() As Boolean
				Return root IsNot Nothing
			End Function

			Public Function nextElement() As TreeNode
				Dim retval As TreeNode

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				If subtree.hasMoreElements() Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					retval = subtree.nextElement()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				ElseIf children.hasMoreElements() Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					subtree = New PostorderEnumeration(children.nextElement())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					retval = subtree.nextElement()
				Else
					retval = root
					root = Nothing
				End If

				Return retval
			End Function

		End Class ' End of class PostorderEnumeration



		Friend NotInheritable Class BreadthFirstEnumeration
			Implements System.Collections.IEnumerator(Of TreeNode)

			Private ReadOnly outerInstance As DefaultMutableTreeNode

			Protected Friend queue As Queue

			Public Sub New(ByVal outerInstance As DefaultMutableTreeNode, ByVal rootNode As TreeNode)
					Me.outerInstance = outerInstance
				MyBase.New()
				Dim v As New List(Of TreeNode)(1)
				v.Add(rootNode) ' PENDING: don't really need a vector
				queue = New Queue(Me)
				queue.enqueue(v.elements())
			End Sub

			Public Function hasMoreElements() As Boolean
				Return ((Not queue.empty) AndAlso CType(queue.firstObject(), System.Collections.IEnumerator).hasMoreElements())
			End Function

			Public Function nextElement() As TreeNode
				Dim enumer As System.Collections.IEnumerator = CType(queue.firstObject(), System.Collections.IEnumerator)
				Dim node As TreeNode = CType(enumer.nextElement(), TreeNode)
				Dim children As System.Collections.IEnumerator = node.children()

				If Not enumer.hasMoreElements() Then queue.dequeue()
				If children.hasMoreElements() Then queue.enqueue(children)
				Return node
			End Function


			' A simple queue with a linked list data structure.
			Friend NotInheritable Class Queue
				Private ReadOnly outerInstance As DefaultMutableTreeNode.BreadthFirstEnumeration

				Public Sub New(ByVal outerInstance As DefaultMutableTreeNode.BreadthFirstEnumeration)
					Me.outerInstance = outerInstance
				End Sub

				Friend head As QNode ' null if empty
				Friend tail As QNode

				Friend NotInheritable Class QNode
					Private ReadOnly outerInstance As DefaultMutableTreeNode.BreadthFirstEnumeration.Queue

					Public [object] As Object
					Public [next] As QNode ' null if end
					Public Sub New(ByVal outerInstance As DefaultMutableTreeNode.BreadthFirstEnumeration.Queue, ByVal [object] As Object, ByVal [next] As QNode)
							Me.outerInstance = outerInstance
						Me.object = [object]
						Me.next = [next]
					End Sub
				End Class

				Public Sub enqueue(ByVal anObject As Object)
					If head Is Nothing Then
							tail = New QNode(Me, Me, anObject, Nothing)
							head = tail
					Else
						tail.next = New QNode(Me, anObject, Nothing)
						tail = tail.next
					End If
				End Sub

				Public Function dequeue() As Object
					If head Is Nothing Then Throw New NoSuchElementException("No more elements")

					Dim retval As Object = head.object
					Dim oldHead As QNode = head
					head = head.next
					If head Is Nothing Then
						tail = Nothing
					Else
						oldHead.next = Nothing
					End If
					Return retval
				End Function

				Public Function firstObject() As Object
					If head Is Nothing Then Throw New NoSuchElementException("No more elements")

					Return head.object
				End Function

				Public Property empty As Boolean
					Get
						Return head Is Nothing
					End Get
				End Property

			End Class ' End of class Queue

		End Class ' End of class BreadthFirstEnumeration



		Friend NotInheritable Class PathBetweenNodesEnumeration
			Implements System.Collections.IEnumerator(Of TreeNode)

			Private ReadOnly outerInstance As DefaultMutableTreeNode

			Protected Friend stack As Stack(Of TreeNode)

			Public Sub New(ByVal outerInstance As DefaultMutableTreeNode, ByVal ancestor As TreeNode, ByVal descendant As TreeNode)
					Me.outerInstance = outerInstance
				MyBase.New()

				If ancestor Is Nothing OrElse descendant Is Nothing Then Throw New System.ArgumentException("argument is null")

				Dim current As TreeNode

				stack = New Stack(Of TreeNode)
				stack.Push(descendant)

				current = descendant
				Do While current IsNot ancestor
					current = current.parent
					If current Is Nothing AndAlso descendant IsNot ancestor Then Throw New System.ArgumentException("node " & ancestor & " is not an ancestor of " & descendant)
					stack.Push(current)
				Loop
			End Sub

			Public Function hasMoreElements() As Boolean
				Return stack.Count > 0
			End Function

			Public Function nextElement() As TreeNode
				Try
					Return stack.Pop()
				Catch e As EmptyStackException
					Throw New NoSuchElementException("No more elements")
				End Try
			End Function

		End Class ' End of class PathBetweenNodesEnumeration



	End Class ' End of class DefaultMutableTreeNode

End Namespace
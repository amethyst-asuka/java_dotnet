Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports javax.swing.event

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

	''' <summary>
	''' A simple tree data model that uses TreeNodes.
	''' For further information and examples that use DefaultTreeModel,
	''' see <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/tree.html">How to Use Trees</a>
	''' in <em>The Java Tutorial.</em>
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author Rob Davis
	''' @author Ray Ryan
	''' @author Scott Violet
	''' </summary>
	<Serializable> _
	Public Class DefaultTreeModel
		Implements TreeModel

		''' <summary>
		''' Root of the tree. </summary>
		Protected Friend root As TreeNode
		''' <summary>
		''' Listeners. </summary>
		Protected Friend listenerList As New EventListenerList
		''' <summary>
		''' Determines how the <code>isLeaf</code> method figures
		''' out if a node is a leaf node. If true, a node is a leaf
		''' node if it does not allow children. (If it allows
		''' children, it is not a leaf node, even if no children
		''' are present.) That lets you distinguish between <i>folder</i>
		''' nodes and <i>file</i> nodes in a file system, for example.
		''' <p>
		''' If this value is false, then any node which has no
		''' children is a leaf node, and any node may acquire
		''' children.
		''' </summary>
		''' <seealso cref= TreeNode#getAllowsChildren </seealso>
		''' <seealso cref= TreeModel#isLeaf </seealso>
		''' <seealso cref= #setAsksAllowsChildren </seealso>
		Protected Friend ___asksAllowsChildren As Boolean


		''' <summary>
		''' Creates a tree in which any node can have children.
		''' </summary>
		''' <param name="root"> a TreeNode object that is the root of the tree </param>
		''' <seealso cref= #DefaultTreeModel(TreeNode, boolean) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		 Public Sub New(ByVal root As TreeNode)
			Me.New(root, False)
		 End Sub

		''' <summary>
		''' Creates a tree specifying whether any node can have children,
		''' or whether only certain nodes can have children.
		''' </summary>
		''' <param name="root"> a TreeNode object that is the root of the tree </param>
		''' <param name="asksAllowsChildren"> a boolean, false if any node can
		'''        have children, true if each node is asked to see if
		'''        it can have children </param>
		''' <seealso cref= #asksAllowsChildren </seealso>
		Public Sub New(ByVal root As TreeNode, ByVal asksAllowsChildren As Boolean)
			MyBase.New()
			Me.root = root
			Me.___asksAllowsChildren = asksAllowsChildren
		End Sub

		''' <summary>
		''' Sets whether or not to test leafness by asking getAllowsChildren()
		''' or isLeaf() to the TreeNodes.  If newvalue is true, getAllowsChildren()
		''' is messaged, otherwise isLeaf() is messaged.
		''' </summary>
		Public Overridable Property asksAllowsChildren As Boolean
			Set(ByVal newValue As Boolean)
				___asksAllowsChildren = newValue
			End Set
		End Property

		''' <summary>
		''' Tells how leaf nodes are determined.
		''' </summary>
		''' <returns> true if only nodes which do not allow children are
		'''         leaf nodes, false if nodes which have no children
		'''         (even if allowed) are leaf nodes </returns>
		''' <seealso cref= #asksAllowsChildren </seealso>
		Public Overridable Function asksAllowsChildren() As Boolean
			Return ___asksAllowsChildren
		End Function

		''' <summary>
		''' Sets the root to <code>root</code>. A null <code>root</code> implies
		''' the tree is to display nothing, and is legal.
		''' </summary>
		Public Overridable Property root As TreeNode
			Set(ByVal root As TreeNode)
				Dim oldRoot As Object = Me.root
				Me.root = root
				If root Is Nothing AndAlso oldRoot IsNot Nothing Then
					fireTreeStructureChanged(Me, Nothing)
				Else
					nodeStructureChanged(root)
				End If
			End Set
			Get
				Return root
			End Get
		End Property


		''' <summary>
		''' Returns the index of child in parent.
		''' If either the parent or child is <code>null</code>, returns -1. </summary>
		''' <param name="parent"> a note in the tree, obtained from this data source </param>
		''' <param name="child"> the node we are interested in </param>
		''' <returns> the index of the child in the parent, or -1
		'''    if either the parent or the child is <code>null</code> </returns>
		Public Overridable Function getIndexOfChild(ByVal parent As Object, ByVal child As Object) As Integer Implements TreeModel.getIndexOfChild
			If parent Is Nothing OrElse child Is Nothing Then Return -1
			Return CType(parent, TreeNode).getIndex(CType(child, TreeNode))
		End Function

		''' <summary>
		''' Returns the child of <I>parent</I> at index <I>index</I> in the parent's
		''' child array.  <I>parent</I> must be a node previously obtained from
		''' this data source. This should not return null if <i>index</i>
		''' is a valid index for <i>parent</i> (that is <i>index</i> &gt;= 0 &amp;&amp;
		''' <i>index</i> &lt; getChildCount(<i>parent</i>)).
		''' </summary>
		''' <param name="parent">  a node in the tree, obtained from this data source </param>
		''' <returns>  the child of <I>parent</I> at index <I>index</I> </returns>
		Public Overridable Function getChild(ByVal parent As Object, ByVal index As Integer) As Object Implements TreeModel.getChild
			Return CType(parent, TreeNode).getChildAt(index)
		End Function

		''' <summary>
		''' Returns the number of children of <I>parent</I>.  Returns 0 if the node
		''' is a leaf or if it has no children.  <I>parent</I> must be a node
		''' previously obtained from this data source.
		''' </summary>
		''' <param name="parent">  a node in the tree, obtained from this data source </param>
		''' <returns>  the number of children of the node <I>parent</I> </returns>
		Public Overridable Function getChildCount(ByVal parent As Object) As Integer Implements TreeModel.getChildCount
			Return CType(parent, TreeNode).childCount
		End Function

		''' <summary>
		''' Returns whether the specified node is a leaf node.
		''' The way the test is performed depends on the
		''' <code>askAllowsChildren</code> setting.
		''' </summary>
		''' <param name="node"> the node to check </param>
		''' <returns> true if the node is a leaf node
		''' </returns>
		''' <seealso cref= #asksAllowsChildren </seealso>
		''' <seealso cref= TreeModel#isLeaf </seealso>
		Public Overridable Function isLeaf(ByVal node As Object) As Boolean Implements TreeModel.isLeaf
			If ___asksAllowsChildren Then Return Not CType(node, TreeNode).allowsChildren
			Return CType(node, TreeNode).leaf
		End Function

		''' <summary>
		''' Invoke this method if you've modified the {@code TreeNode}s upon which
		''' this model depends. The model will notify all of its listeners that the
		''' model has changed.
		''' </summary>
		Public Overridable Sub reload()
			reload(root)
		End Sub

		''' <summary>
		''' This sets the user object of the TreeNode identified by path
		''' and posts a node changed.  If you use custom user objects in
		''' the TreeModel you're going to need to subclass this and
		''' set the user object of the changed node to something meaningful.
		''' </summary>
		Public Overridable Sub valueForPathChanged(ByVal path As TreePath, ByVal newValue As Object) Implements TreeModel.valueForPathChanged
			Dim aNode As MutableTreeNode = CType(path.lastPathComponent, MutableTreeNode)

			aNode.userObject = newValue
			nodeChanged(aNode)
		End Sub

		''' <summary>
		''' Invoked this to insert newChild at location index in parents children.
		''' This will then message nodesWereInserted to create the appropriate
		''' event. This is the preferred way to add children as it will create
		''' the appropriate event.
		''' </summary>
		Public Overridable Sub insertNodeInto(ByVal newChild As MutableTreeNode, ByVal parent As MutableTreeNode, ByVal index As Integer)
			parent.insert(newChild, index)

			Dim newIndexs As Integer() = New Integer(0){}

			newIndexs(0) = index
			nodesWereInserted(parent, newIndexs)
		End Sub

		''' <summary>
		''' Message this to remove node from its parent. This will message
		''' nodesWereRemoved to create the appropriate event. This is the
		''' preferred way to remove a node as it handles the event creation
		''' for you.
		''' </summary>
		Public Overridable Sub removeNodeFromParent(ByVal node As MutableTreeNode)
			Dim parent As MutableTreeNode = CType(node.parent, MutableTreeNode)

			If parent Is Nothing Then Throw New System.ArgumentException("node does not have a parent.")

			Dim childIndex As Integer() = New Integer(0){}
			Dim removedArray As Object() = New Object(0){}

			childIndex(0) = parent.getIndex(node)
			parent.remove(childIndex(0))
			removedArray(0) = node
			nodesWereRemoved(parent, childIndex, removedArray)
		End Sub

		''' <summary>
		''' Invoke this method after you've changed how node is to be
		''' represented in the tree.
		''' </summary>
		Public Overridable Sub nodeChanged(ByVal node As TreeNode)
			If listenerList IsNot Nothing AndAlso node IsNot Nothing Then
				Dim parent As TreeNode = node.parent

				If parent IsNot Nothing Then
					Dim anIndex As Integer = parent.getIndex(node)
					If anIndex <> -1 Then
						Dim cIndexs As Integer() = New Integer(0){}

						cIndexs(0) = anIndex
						nodesChanged(parent, cIndexs)
					End If
				ElseIf node Is root Then
					nodesChanged(node, Nothing)
				End If
			End If
		End Sub

		''' <summary>
		''' Invoke this method if you've modified the {@code TreeNode}s upon which
		''' this model depends. The model will notify all of its listeners that the
		''' model has changed below the given node.
		''' </summary>
		''' <param name="node"> the node below which the model has changed </param>
		Public Overridable Sub reload(ByVal node As TreeNode)
			If node IsNot Nothing Then fireTreeStructureChanged(Me, getPathToRoot(node), Nothing, Nothing)
		End Sub

		''' <summary>
		''' Invoke this method after you've inserted some TreeNodes into
		''' node.  childIndices should be the index of the new elements and
		''' must be sorted in ascending order.
		''' </summary>
		Public Overridable Sub nodesWereInserted(ByVal node As TreeNode, ByVal childIndices As Integer())
			If listenerList IsNot Nothing AndAlso node IsNot Nothing AndAlso childIndices IsNot Nothing AndAlso childIndices.Length > 0 Then
				Dim cCount As Integer = childIndices.Length
				Dim newChildren As Object() = New Object(cCount - 1){}

				For counter As Integer = 0 To cCount - 1
					newChildren(counter) = node.getChildAt(childIndices(counter))
				Next counter
				fireTreeNodesInserted(Me, getPathToRoot(node), childIndices, newChildren)
			End If
		End Sub

		''' <summary>
		''' Invoke this method after you've removed some TreeNodes from
		''' node.  childIndices should be the index of the removed elements and
		''' must be sorted in ascending order. And removedChildren should be
		''' the array of the children objects that were removed.
		''' </summary>
		Public Overridable Sub nodesWereRemoved(ByVal node As TreeNode, ByVal childIndices As Integer(), ByVal removedChildren As Object())
			If node IsNot Nothing AndAlso childIndices IsNot Nothing Then fireTreeNodesRemoved(Me, getPathToRoot(node), childIndices, removedChildren)
		End Sub

		''' <summary>
		''' Invoke this method after you've changed how the children identified by
		''' childIndicies are to be represented in the tree.
		''' </summary>
		Public Overridable Sub nodesChanged(ByVal node As TreeNode, ByVal childIndices As Integer())
			If node IsNot Nothing Then
				If childIndices IsNot Nothing Then
					Dim cCount As Integer = childIndices.Length

					If cCount > 0 Then
						Dim cChildren As Object() = New Object(cCount - 1){}

						For counter As Integer = 0 To cCount - 1
							cChildren(counter) = node.getChildAt(childIndices(counter))
						Next counter
						fireTreeNodesChanged(Me, getPathToRoot(node), childIndices, cChildren)
					End If
				ElseIf node Is root Then
					fireTreeNodesChanged(Me, getPathToRoot(node), Nothing, Nothing)
				End If
			End If
		End Sub

		''' <summary>
		''' Invoke this method if you've totally changed the children of
		''' node and its children's children...  This will post a
		''' treeStructureChanged event.
		''' </summary>
		Public Overridable Sub nodeStructureChanged(ByVal node As TreeNode)
			If node IsNot Nothing Then fireTreeStructureChanged(Me, getPathToRoot(node), Nothing, Nothing)
		End Sub

		''' <summary>
		''' Builds the parents of node up to and including the root node,
		''' where the original node is the last element in the returned array.
		''' The length of the returned array gives the node's depth in the
		''' tree.
		''' </summary>
		''' <param name="aNode"> the TreeNode to get the path for </param>
		Public Overridable Function getPathToRoot(ByVal aNode As TreeNode) As TreeNode()
			Return getPathToRoot(aNode, 0)
		End Function

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
			' This method recurses, traversing towards the root in order
			' size the array. On the way back, it fills in the nodes,
			' starting from the root and working back to the original node.

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
				If aNode Is root Then
					retNodes = New TreeNode(depth - 1){}
				Else
					retNodes = getPathToRoot(aNode.parent, depth)
				End If
				retNodes(retNodes.Length - depth) = aNode
			End If
			Return retNodes
		End Function

		'
		'  Events
		'

		''' <summary>
		''' Adds a listener for the TreeModelEvent posted after the tree changes.
		''' </summary>
		''' <seealso cref=     #removeTreeModelListener </seealso>
		''' <param name="l">       the listener to add </param>
		Public Overridable Sub addTreeModelListener(ByVal l As TreeModelListener) Implements TreeModel.addTreeModelListener
			listenerList.add(GetType(TreeModelListener), l)
		End Sub

		''' <summary>
		''' Removes a listener previously added with <B>addTreeModelListener()</B>.
		''' </summary>
		''' <seealso cref=     #addTreeModelListener </seealso>
		''' <param name="l">       the listener to remove </param>
		Public Overridable Sub removeTreeModelListener(ByVal l As TreeModelListener) Implements TreeModel.removeTreeModelListener
			listenerList.remove(GetType(TreeModelListener), l)
		End Sub

		''' <summary>
		''' Returns an array of all the tree model listeners
		''' registered on this model.
		''' </summary>
		''' <returns> all of this model's <code>TreeModelListener</code>s
		'''         or an empty
		'''         array if no tree model listeners are currently registered
		''' </returns>
		''' <seealso cref= #addTreeModelListener </seealso>
		''' <seealso cref= #removeTreeModelListener
		''' 
		''' @since 1.4 </seealso>
		Public Overridable Property treeModelListeners As TreeModelListener()
			Get
				Return listenerList.getListeners(GetType(TreeModelListener))
			End Get
		End Property

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the parameters passed into
		''' the fire method.
		''' </summary>
		''' <param name="source"> the source of the {@code TreeModelEvent};
		'''               typically {@code this} </param>
		''' <param name="path"> the path to the parent of the nodes that changed; use
		'''             {@code null} to identify the root has changed </param>
		''' <param name="childIndices"> the indices of the changed elements </param>
		''' <param name="children"> the changed elements </param>
		Protected Friend Overridable Sub fireTreeNodesChanged(ByVal source As Object, ByVal path As Object(), ByVal childIndices As Integer(), ByVal children As Object())
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As TreeModelEvent = Nothing
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(TreeModelListener) Then
					' Lazily create the event:
					If e Is Nothing Then e = New TreeModelEvent(source, path, childIndices, children)
					CType(___listeners(i+1), TreeModelListener).treeNodesChanged(e)
				End If
			Next i
		End Sub

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the parameters passed into
		''' the fire method.
		''' </summary>
		''' <param name="source"> the source of the {@code TreeModelEvent};
		'''               typically {@code this} </param>
		''' <param name="path"> the path to the parent the nodes were added to </param>
		''' <param name="childIndices"> the indices of the new elements </param>
		''' <param name="children"> the new elements </param>
		Protected Friend Overridable Sub fireTreeNodesInserted(ByVal source As Object, ByVal path As Object(), ByVal childIndices As Integer(), ByVal children As Object())
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As TreeModelEvent = Nothing
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(TreeModelListener) Then
					' Lazily create the event:
					If e Is Nothing Then e = New TreeModelEvent(source, path, childIndices, children)
					CType(___listeners(i+1), TreeModelListener).treeNodesInserted(e)
				End If
			Next i
		End Sub

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the parameters passed into
		''' the fire method.
		''' </summary>
		''' <param name="source"> the source of the {@code TreeModelEvent};
		'''               typically {@code this} </param>
		''' <param name="path"> the path to the parent the nodes were removed from </param>
		''' <param name="childIndices"> the indices of the removed elements </param>
		''' <param name="children"> the removed elements </param>
		Protected Friend Overridable Sub fireTreeNodesRemoved(ByVal source As Object, ByVal path As Object(), ByVal childIndices As Integer(), ByVal children As Object())
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As TreeModelEvent = Nothing
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(TreeModelListener) Then
					' Lazily create the event:
					If e Is Nothing Then e = New TreeModelEvent(source, path, childIndices, children)
					CType(___listeners(i+1), TreeModelListener).treeNodesRemoved(e)
				End If
			Next i
		End Sub

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the parameters passed into
		''' the fire method.
		''' </summary>
		''' <param name="source"> the source of the {@code TreeModelEvent};
		'''               typically {@code this} </param>
		''' <param name="path"> the path to the parent of the structure that has changed;
		'''             use {@code null} to identify the root has changed </param>
		''' <param name="childIndices"> the indices of the affected elements </param>
		''' <param name="children"> the affected elements </param>
		Protected Friend Overridable Sub fireTreeStructureChanged(ByVal source As Object, ByVal path As Object(), ByVal childIndices As Integer(), ByVal children As Object())
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As TreeModelEvent = Nothing
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(TreeModelListener) Then
					' Lazily create the event:
					If e Is Nothing Then e = New TreeModelEvent(source, path, childIndices, children)
					CType(___listeners(i+1), TreeModelListener).treeStructureChanged(e)
				End If
			Next i
		End Sub

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the parameters passed into
		''' the fire method.
		''' </summary>
		''' <param name="source"> the source of the {@code TreeModelEvent};
		'''               typically {@code this} </param>
		''' <param name="path"> the path to the parent of the structure that has changed;
		'''             use {@code null} to identify the root has changed </param>
		Private Sub fireTreeStructureChanged(ByVal source As Object, ByVal path As TreePath)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As TreeModelEvent = Nothing
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(TreeModelListener) Then
					' Lazily create the event:
					If e Is Nothing Then e = New TreeModelEvent(source, path)
					CType(___listeners(i+1), TreeModelListener).treeStructureChanged(e)
				End If
			Next i
		End Sub

		''' <summary>
		''' Returns an array of all the objects currently registered
		''' as <code><em>Foo</em>Listener</code>s
		''' upon this model.
		''' <code><em>Foo</em>Listener</code>s are registered using the
		''' <code>add<em>Foo</em>Listener</code> method.
		''' 
		''' <p>
		''' 
		''' You can specify the <code>listenerType</code> argument
		''' with a class literal,
		''' such as
		''' <code><em>Foo</em>Listener.class</code>.
		''' For example, you can query a
		''' <code>DefaultTreeModel</code> <code>m</code>
		''' for its tree model listeners with the following code:
		''' 
		''' <pre>TreeModelListener[] tmls = (TreeModelListener[])(m.getListeners(TreeModelListener.class));</pre>
		''' 
		''' If no such listeners exist, this method returns an empty array.
		''' </summary>
		''' <param name="listenerType"> the type of listeners requested; this parameter
		'''          should specify an interface that descends from
		'''          <code>java.util.EventListener</code> </param>
		''' <returns> an array of all objects registered as
		'''          <code><em>Foo</em>Listener</code>s on this component,
		'''          or an empty array if no such
		'''          listeners have been added </returns>
		''' <exception cref="ClassCastException"> if <code>listenerType</code>
		'''          doesn't specify a class or interface that implements
		'''          <code>java.util.EventListener</code>
		''' </exception>
		''' <seealso cref= #getTreeModelListeners
		''' 
		''' @since 1.3 </seealso>
		Public Overridable Function getListeners(Of T As EventListener)(ByVal listenerType As Type) As T()
			Return listenerList.getListeners(listenerType)
		End Function

		' Serialization support.
		Private Sub writeObject(ByVal s As ObjectOutputStream)
			Dim values As New List(Of Object)

			s.defaultWriteObject()
			' Save the root, if its Serializable.
			If root IsNot Nothing AndAlso TypeOf root Is Serializable Then
				values.Add("root")
				values.Add(root)
			End If
			s.writeObject(values)
		End Sub

		Private Sub readObject(ByVal s As ObjectInputStream)
			s.defaultReadObject()

			Dim values As ArrayList = CType(s.readObject(), ArrayList)
			Dim indexCounter As Integer = 0
			Dim maxCounter As Integer = values.Count

			If indexCounter < maxCounter AndAlso values(indexCounter).Equals("root") Then
				indexCounter += 1
				root = CType(values(indexCounter), TreeNode)
				indexCounter += 1
			End If
		End Sub


	End Class ' End of class DefaultTreeModel

End Namespace
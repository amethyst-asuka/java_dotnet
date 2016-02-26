Imports System
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' NOTE: This will become more open in a future release.
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

	Public Class VariableHeightLayoutCache
		Inherits AbstractLayoutCache

		''' <summary>
		''' The array of nodes that are currently visible, in the order they
		''' are displayed.
		''' </summary>
		Private visibleNodes As List(Of Object)

		''' <summary>
		''' This is set to true if one of the entries has an invalid size.
		''' </summary>
		Private ___updateNodeSizes As Boolean

		''' <summary>
		''' The root node of the internal cache of nodes that have been shown.
		''' If the treeModel is vending a network rather than a true tree,
		''' there may be one cached node for each path to a modeled node.
		''' </summary>
		Private root As TreeStateNode

		''' <summary>
		''' Used in getting sizes for nodes to avoid creating a new Rectangle
		''' every time a size is needed.
		''' </summary>
		Private boundsBuffer As java.awt.Rectangle

		''' <summary>
		''' Maps from <code>TreePath</code> to a <code>TreeStateNode</code>.
		''' </summary>
		Private treePathMapping As Dictionary(Of TreePath, TreeStateNode)

		''' <summary>
		''' A stack of stacks.
		''' </summary>
		Private tempStacks As Stack(Of Stack(Of TreePath))


		Public Sub New()
			MyBase.New()
			tempStacks = New Stack(Of Stack(Of TreePath))
			visibleNodes = New List(Of Object)
			boundsBuffer = New java.awt.Rectangle
			treePathMapping = New Dictionary(Of TreePath, TreeStateNode)
		End Sub

		''' <summary>
		''' Sets the <code>TreeModel</code> that will provide the data.
		''' </summary>
		''' <param name="newModel"> the <code>TreeModel</code> that is to provide the data
		''' @beaninfo
		'''        bound: true
		'''  description: The TreeModel that will provide the data. </param>
		Public Overrides Property model As TreeModel
			Set(ByVal newModel As TreeModel)
				MyBase.model = newModel
				rebuild(False)
			End Set
		End Property

		''' <summary>
		''' Determines whether or not the root node from
		''' the <code>TreeModel</code> is visible.
		''' </summary>
		''' <param name="rootVisible"> true if the root node of the tree is to be displayed </param>
		''' <seealso cref= #rootVisible
		''' @beaninfo
		'''        bound: true
		'''  description: Whether or not the root node
		'''               from the TreeModel is visible. </seealso>
		Public Overrides Property rootVisible As Boolean
			Set(ByVal rootVisible As Boolean)
				If rootVisible <> rootVisible AndAlso root IsNot Nothing Then
					If rootVisible Then
						root.updatePreferredSize(0)
						visibleNodes.Insert(0, root)
					ElseIf visibleNodes.Count > 0 Then
						visibleNodes.RemoveAt(0)
						If treeSelectionModel IsNot Nothing Then treeSelectionModel.removeSelectionPath(root.treePath)
					End If
					If treeSelectionModel IsNot Nothing Then treeSelectionModel.resetRowSelection()
					If rowCount > 0 Then getNode(0).yOrigin = 0
					updateYLocationsFrom(0)
					visibleNodesChanged()
				End If
				MyBase.rootVisible = rootVisible
			End Set
		End Property

		''' <summary>
		''' Sets the height of each cell.  If the specified value
		''' is less than or equal to zero the current cell renderer is
		''' queried for each row's height.
		''' </summary>
		''' <param name="rowHeight"> the height of each cell, in pixels
		''' @beaninfo
		'''        bound: true
		'''  description: The height of each cell. </param>
		Public Overrides Property rowHeight As Integer
			Set(ByVal rowHeight As Integer)
				If rowHeight <> rowHeight Then
					MyBase.rowHeight = rowHeight
					invalidateSizes()
					Me.visibleNodesChanged()
				End If
			End Set
		End Property

		''' <summary>
		''' Sets the renderer that is responsible for drawing nodes in the tree. </summary>
		''' <param name="nd"> the renderer </param>
		Public Overrides Property nodeDimensions As NodeDimensions
			Set(ByVal nd As NodeDimensions)
				MyBase.nodeDimensions = nd
				invalidateSizes()
				visibleNodesChanged()
			End Set
		End Property

		''' <summary>
		''' Marks the path <code>path</code> expanded state to
		''' <code>isExpanded</code>. </summary>
		''' <param name="path"> the <code>TreePath</code> of interest </param>
		''' <param name="isExpanded"> true if the path should be expanded, otherwise false </param>
		Public Overrides Sub setExpandedState(ByVal path As TreePath, ByVal isExpanded As Boolean)
			If path IsNot Nothing Then
				If isExpanded Then
					ensurePathIsExpanded(path, True)
				Else
					Dim ___node As TreeStateNode = getNodeForPath(path, False, True)

					If ___node IsNot Nothing Then
						___node.makeVisible()
						___node.collapse()
					End If
				End If
			End If
		End Sub

		''' <summary>
		''' Returns true if the path is expanded, and visible. </summary>
		''' <returns> true if the path is expanded and visible, otherwise false </returns>
		Public Overrides Function getExpandedState(ByVal path As TreePath) As Boolean
			Dim ___node As TreeStateNode = getNodeForPath(path, True, False)

			Return If(___node IsNot Nothing, (___node.visible AndAlso ___node.expanded), False)
		End Function

		''' <summary>
		''' Returns the <code>Rectangle</code> enclosing the label portion
		''' into which the item identified by <code>path</code> will be drawn.
		''' </summary>
		''' <param name="path">  the path to be drawn </param>
		''' <param name="placeIn"> the bounds of the enclosing rectangle </param>
		''' <returns> the bounds of the enclosing rectangle or <code>null</code>
		'''    if the node could not be ascertained </returns>
		Public Overrides Function getBounds(ByVal path As TreePath, ByVal placeIn As java.awt.Rectangle) As java.awt.Rectangle
			Dim ___node As TreeStateNode = getNodeForPath(path, True, False)

			If ___node IsNot Nothing Then
				If ___updateNodeSizes Then updateNodeSizes(False)
				Return ___node.getNodeBounds(placeIn)
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Returns the path for <code>row</code>.  If <code>row</code>
		''' is not visible, <code>null</code> is returned.
		''' </summary>
		''' <param name="row"> the location of interest </param>
		''' <returns> the path for <code>row</code>, or <code>null</code>
		''' if <code>row</code> is not visible </returns>
		Public Overrides Function getPathForRow(ByVal row As Integer) As TreePath
			If row >= 0 AndAlso row < rowCount Then Return getNode(row).treePath
			Return Nothing
		End Function

		''' <summary>
		''' Returns the row where the last item identified in path is visible.
		''' Will return -1 if any of the elements in path are not
		''' currently visible.
		''' </summary>
		''' <param name="path"> the <code>TreePath</code> of interest </param>
		''' <returns> the row where the last item in path is visible </returns>
		Public Overrides Function getRowForPath(ByVal path As TreePath) As Integer
			If path Is Nothing Then Return -1

			Dim visNode As TreeStateNode = getNodeForPath(path, True, False)

			If visNode IsNot Nothing Then Return visNode.row
			Return -1
		End Function

		''' <summary>
		''' Returns the number of visible rows. </summary>
		''' <returns> the number of visible rows </returns>
		Public Property Overrides rowCount As Integer
			Get
				Return visibleNodes.Count
			End Get
		End Property

		''' <summary>
		''' Instructs the <code>LayoutCache</code> that the bounds for
		''' <code>path</code> are invalid, and need to be updated.
		''' </summary>
		''' <param name="path"> the <code>TreePath</code> which is now invalid </param>
		Public Overrides Sub invalidatePathBounds(ByVal path As TreePath)
			Dim ___node As TreeStateNode = getNodeForPath(path, True, False)

			If ___node IsNot Nothing Then
				___node.markSizeInvalid()
				If ___node.visible Then updateYLocationsFrom(___node.row)
			End If
		End Sub

		''' <summary>
		''' Returns the preferred height. </summary>
		''' <returns> the preferred height </returns>
		Public Property Overrides preferredHeight As Integer
			Get
				' Get the height
				Dim ___rowCount As Integer = rowCount
    
				If ___rowCount > 0 Then
					Dim ___node As TreeStateNode = getNode(___rowCount - 1)
    
					Return ___node.yOrigin + ___node.preferredHeight
				End If
				Return 0
			End Get
		End Property

		''' <summary>
		''' Returns the preferred width and height for the region in
		''' <code>visibleRegion</code>.
		''' </summary>
		''' <param name="bounds">  the region being queried </param>
		Public Overrides Function getPreferredWidth(ByVal bounds As java.awt.Rectangle) As Integer
			If ___updateNodeSizes Then updateNodeSizes(False)

			Return maxNodeWidth
		End Function

		''' <summary>
		''' Returns the path to the node that is closest to x,y.  If
		''' there is nothing currently visible this will return <code>null</code>,
		''' otherwise it will always return a valid path.
		''' If you need to test if the
		''' returned object is exactly at x, y you should get the bounds for
		''' the returned path and test x, y against that.
		''' </summary>
		''' <param name="x">  the x-coordinate </param>
		''' <param name="y">  the y-coordinate </param>
		''' <returns> the path to the node that is closest to x, y </returns>
		Public Overrides Function getPathClosestTo(ByVal x As Integer, ByVal y As Integer) As TreePath
			If rowCount = 0 Then Return Nothing

			If ___updateNodeSizes Then updateNodeSizes(False)

			Dim row As Integer = getRowContainingYLocation(y)

			Return getNode(row).treePath
		End Function

		''' <summary>
		''' Returns an <code>Enumerator</code> that increments over the visible paths
		''' starting at the passed in location. The ordering of the enumeration
		''' is based on how the paths are displayed.
		''' </summary>
		''' <param name="path"> the location in the <code>TreePath</code> to start </param>
		''' <returns> an <code>Enumerator</code> that increments over the visible
		'''     paths </returns>
		Public Overrides Function getVisiblePathsFrom(ByVal path As TreePath) As System.Collections.IEnumerator(Of TreePath)
			Dim ___node As TreeStateNode = getNodeForPath(path, True, False)

			If ___node IsNot Nothing Then Return New VisibleTreeStateNodeEnumeration(Me, ___node)
			Return Nothing
		End Function

		''' <summary>
		''' Returns the number of visible children for <code>path</code>. </summary>
		''' <returns> the number of visible children for <code>path</code> </returns>
		Public Overrides Function getVisibleChildCount(ByVal path As TreePath) As Integer
			Dim ___node As TreeStateNode = getNodeForPath(path, True, False)

			Return If(___node IsNot Nothing, ___node.visibleChildCount, 0)
		End Function

		''' <summary>
		''' Informs the <code>TreeState</code> that it needs to recalculate
		''' all the sizes it is referencing.
		''' </summary>
		Public Overrides Sub invalidateSizes()
			If root IsNot Nothing Then root.deepMarkSizeInvalid()
			If (Not fixedRowHeight) AndAlso visibleNodes.Count > 0 Then updateNodeSizes(True)
		End Sub

		''' <summary>
		''' Returns true if the value identified by <code>path</code> is
		''' currently expanded. </summary>
		''' <returns> true if the value identified by <code>path</code> is
		'''    currently expanded </returns>
		Public Overrides Function isExpanded(ByVal path As TreePath) As Boolean
			If path IsNot Nothing Then
				Dim lastNode As TreeStateNode = getNodeForPath(path, True, False)

				Return (lastNode IsNot Nothing AndAlso lastNode.expanded)
			End If
			Return False
		End Function

		'
		' TreeModelListener methods
		'

		''' <summary>
		''' Invoked after a node (or a set of siblings) has changed in some
		''' way. The node(s) have not changed locations in the tree or
		''' altered their children arrays, but other attributes have
		''' changed and may affect presentation. Example: the name of a
		''' file has changed, but it is in the same location in the file
		''' system.
		''' 
		''' <p><code>e.path</code> returns the path the parent of the
		''' changed node(s).
		''' 
		''' <p><code>e.childIndices</code> returns the index(es) of the
		''' changed node(s).
		''' </summary>
		''' <param name="e"> the <code>TreeModelEvent</code> of interest </param>
		Public Overrides Sub treeNodesChanged(ByVal e As javax.swing.event.TreeModelEvent)
			If e IsNot Nothing Then
				Dim changedIndexs As Integer()
				Dim changedNode As TreeStateNode

				changedIndexs = e.childIndices
				changedNode = getNodeForPath(sun.swing.SwingUtilities2.getTreePath(e, model), False, False)
				If changedNode IsNot Nothing Then
					Dim changedValue As Object = changedNode.value

	'                 Update the size of the changed node, as well as all the
	'                   child indexs that are passed in. 
					changedNode.updatePreferredSize()
					If changedNode.hasBeenExpanded() AndAlso changedIndexs IsNot Nothing Then
						Dim counter As Integer
						Dim changedChildNode As TreeStateNode

						For counter = 0 To changedIndexs.Length - 1
							changedChildNode = CType(changedNode.getChildAt(changedIndexs(counter)), TreeStateNode)
							' Reset the user object. 
							changedChildNode.userObject = treeModel.getChild(changedValue, changedIndexs(counter))
							changedChildNode.updatePreferredSize()
						Next counter
					ElseIf changedNode Is root Then
						' Null indicies for root indicates it changed.
						changedNode.updatePreferredSize()
					End If
					If Not fixedRowHeight Then
						Dim aRow As Integer = changedNode.row

						If aRow <> -1 Then Me.updateYLocationsFrom(aRow)
					End If
					Me.visibleNodesChanged()
				End If
			End If
		End Sub


		''' <summary>
		''' Invoked after nodes have been inserted into the tree.
		''' 
		''' <p><code>e.path</code> returns the parent of the new nodes.
		''' <p><code>e.childIndices</code> returns the indices of the new nodes in
		''' ascending order.
		''' </summary>
		''' <param name="e"> the <code>TreeModelEvent</code> of interest </param>
		Public Overrides Sub treeNodesInserted(ByVal e As javax.swing.event.TreeModelEvent)
			If e IsNot Nothing Then
				Dim changedIndexs As Integer()
				Dim changedParentNode As TreeStateNode

				changedIndexs = e.childIndices
				changedParentNode = getNodeForPath(sun.swing.SwingUtilities2.getTreePath(e, model), False, False)
	'             Only need to update the children if the node has been
	'               expanded once. 
				' PENDING(scott): make sure childIndexs is sorted!
				If changedParentNode IsNot Nothing AndAlso changedIndexs IsNot Nothing AndAlso changedIndexs.Length > 0 Then
					If changedParentNode.hasBeenExpanded() Then
						Dim makeVisible As Boolean
						Dim counter As Integer
						Dim changedParent As Object
						Dim newNode As TreeStateNode
						Dim oldChildCount As Integer = changedParentNode.childCount

						changedParent = changedParentNode.value
						makeVisible = ((changedParentNode Is root AndAlso (Not rootVisible)) OrElse (changedParentNode.row <> -1 AndAlso changedParentNode.expanded))
						For counter = 0 To changedIndexs.Length - 1
							newNode = Me.createNodeAt(changedParentNode, changedIndexs(counter))
						Next counter
						If oldChildCount = 0 Then changedParentNode.updatePreferredSize()
						If treeSelectionModel IsNot Nothing Then treeSelectionModel.resetRowSelection()
	'                     Update the y origins from the index of the parent
	'                       to the end of the visible rows. 
						If (Not fixedRowHeight) AndAlso (makeVisible OrElse (oldChildCount = 0 AndAlso changedParentNode.visible)) Then
							If changedParentNode Is root Then
								Me.updateYLocationsFrom(0)
							Else
								Me.updateYLocationsFrom(changedParentNode.row)
							End If
							Me.visibleNodesChanged()
						ElseIf makeVisible Then
							Me.visibleNodesChanged()
						End If
					ElseIf treeModel.getChildCount(changedParentNode.value) - changedIndexs.Length = 0 Then
						changedParentNode.updatePreferredSize()
						If (Not fixedRowHeight) AndAlso changedParentNode.visible Then updateYLocationsFrom(changedParentNode.row)
					End If
				End If
			End If
		End Sub

		''' <summary>
		''' Invoked after nodes have been removed from the tree.  Note that
		''' if a subtree is removed from the tree, this method may only be
		''' invoked once for the root of the removed subtree, not once for
		''' each individual set of siblings removed.
		''' 
		''' <p><code>e.path</code> returns the former parent of the deleted nodes.
		''' 
		''' <p><code>e.childIndices</code> returns the indices the nodes had
		''' before they were deleted in ascending order.
		''' </summary>
		''' <param name="e"> the <code>TreeModelEvent</code> of interest </param>
		Public Overrides Sub treeNodesRemoved(ByVal e As javax.swing.event.TreeModelEvent)
			If e IsNot Nothing Then
				Dim changedIndexs As Integer()
				Dim changedParentNode As TreeStateNode

				changedIndexs = e.childIndices
				changedParentNode = getNodeForPath(sun.swing.SwingUtilities2.getTreePath(e, model), False, False)
				' PENDING(scott): make sure that changedIndexs are sorted in
				' ascending order.
				If changedParentNode IsNot Nothing AndAlso changedIndexs IsNot Nothing AndAlso changedIndexs.Length > 0 Then
					If changedParentNode.hasBeenExpanded() Then
						Dim makeInvisible As Boolean
						Dim counter As Integer
						Dim removedRow As Integer
						Dim removedNode As TreeStateNode

						makeInvisible = ((changedParentNode Is root AndAlso (Not rootVisible)) OrElse (changedParentNode.row <> -1 AndAlso changedParentNode.expanded))
						For counter = changedIndexs.Length - 1 To 0 Step -1
							removedNode = CType(changedParentNode.getChildAt(changedIndexs(counter)), TreeStateNode)
							If removedNode.expanded Then removedNode.collapse(False)

							' Let the selection model now. 
							If makeInvisible Then
								removedRow = removedNode.row
								If removedRow <> -1 Then visibleNodes.RemoveAt(removedRow)
							End If
							changedParentNode.remove(changedIndexs(counter))
						Next counter
						If changedParentNode.childCount = 0 Then
							' Update the size of the parent.
							changedParentNode.updatePreferredSize()
							If changedParentNode.expanded AndAlso changedParentNode.leaf Then changedParentNode.collapse(False)
						End If
						If treeSelectionModel IsNot Nothing Then treeSelectionModel.resetRowSelection()
	'                     Update the y origins from the index of the parent
	'                       to the end of the visible rows. 
						If (Not fixedRowHeight) AndAlso (makeInvisible OrElse (changedParentNode.childCount = 0 AndAlso changedParentNode.visible)) Then
							If changedParentNode Is root Then
	'                             It is possible for first row to have been
	'                               removed if the root isn't visible, in which
	'                               case ylocations will be off! 
								If rowCount > 0 Then getNode(0).yOrigin = 0
								updateYLocationsFrom(0)
							Else
								updateYLocationsFrom(changedParentNode.row)
							End If
							Me.visibleNodesChanged()
						ElseIf makeInvisible Then
							Me.visibleNodesChanged()
						End If
					ElseIf treeModel.getChildCount(changedParentNode.value) = 0 Then
						changedParentNode.updatePreferredSize()
						If (Not fixedRowHeight) AndAlso changedParentNode.visible Then Me.updateYLocationsFrom(changedParentNode.row)
					End If
				End If
			End If
		End Sub

		''' <summary>
		''' Invoked after the tree has drastically changed structure from a
		''' given node down.  If the path returned by <code>e.getPath</code>
		''' is of length one and the first element does not identify the
		''' current root node the first element should become the new root
		''' of the tree.
		''' 
		''' <p><code>e.path</code> holds the path to the node.
		''' <p><code>e.childIndices</code> returns <code>null</code>.
		''' </summary>
		''' <param name="e"> the <code>TreeModelEvent</code> of interest </param>
		Public Overrides Sub treeStructureChanged(ByVal e As javax.swing.event.TreeModelEvent)
			If e IsNot Nothing Then
				Dim changedPath As TreePath = sun.swing.SwingUtilities2.getTreePath(e, model)
				Dim changedNode As TreeStateNode

				changedNode = getNodeForPath(changedPath, False, False)

				' Check if root has changed, either to a null root, or
				' to an entirely new root.
				If changedNode Is root OrElse (changedNode Is Nothing AndAlso ((changedPath Is Nothing AndAlso treeModel IsNot Nothing AndAlso treeModel.root Is Nothing) OrElse (changedPath IsNot Nothing AndAlso changedPath.pathCount = 1))) Then
					rebuild(True)
				ElseIf changedNode IsNot Nothing Then
					Dim nodeIndex, oldRow As Integer
					Dim newNode, parent As TreeStateNode
					Dim wasExpanded, wasVisible As Boolean
					Dim newIndex As Integer

					wasExpanded = changedNode.expanded
					wasVisible = (changedNode.row <> -1)
					' Remove the current node and recreate a new one. 
					parent = CType(changedNode.parent, TreeStateNode)
					nodeIndex = parent.getIndex(changedNode)
					If wasVisible AndAlso wasExpanded Then changedNode.collapse(False)
					If wasVisible Then visibleNodes.Remove(changedNode)
					changedNode.removeFromParent()
					createNodeAt(parent, nodeIndex)
					newNode = CType(parent.getChildAt(nodeIndex), TreeStateNode)
					If wasVisible AndAlso wasExpanded Then newNode.expand(False)
					newIndex = newNode.row
					If (Not fixedRowHeight) AndAlso wasVisible Then
						If newIndex = 0 Then
							updateYLocationsFrom(newIndex)
						Else
							updateYLocationsFrom(newIndex - 1)
						End If
						Me.visibleNodesChanged()
					ElseIf wasVisible Then
						Me.visibleNodesChanged()
					End If
				End If
			End If
		End Sub


		'
		' Local methods
		'

		Private Sub visibleNodesChanged()
		End Sub

		''' <summary>
		''' Adds a mapping for node.
		''' </summary>
		Private Sub addMapping(ByVal node As TreeStateNode)
			treePathMapping(node.treePath) = node
		End Sub

		''' <summary>
		''' Removes the mapping for a previously added node.
		''' </summary>
		Private Sub removeMapping(ByVal node As TreeStateNode)
			treePathMapping.Remove(node.treePath)
		End Sub

		''' <summary>
		''' Returns the node previously added for <code>path</code>. This may
		''' return null, if you to create a node use getNodeForPath.
		''' </summary>
		Private Function getMapping(ByVal path As TreePath) As TreeStateNode
			Return treePathMapping(path)
		End Function

		''' <summary>
		''' Retursn the bounds for row, <code>row</code> by reference in
		''' <code>placeIn</code>. If <code>placeIn</code> is null a new
		''' Rectangle will be created and returned.
		''' </summary>
		Private Function getBounds(ByVal row As Integer, ByVal placeIn As java.awt.Rectangle) As java.awt.Rectangle
			If ___updateNodeSizes Then updateNodeSizes(False)

			If row >= 0 AndAlso row < rowCount Then Return getNode(row).getNodeBounds(placeIn)
			Return Nothing
		End Function

		''' <summary>
		''' Completely rebuild the tree, all expanded state, and node caches are
		''' removed. All nodes are collapsed, except the root.
		''' </summary>
		Private Sub rebuild(ByVal clearSelection As Boolean)
			Dim rootObject As Object

			treePathMapping.Clear()
			rootObject = treeModel.root
			If treeModel IsNot Nothing AndAlso rootObject IsNot Nothing Then
				root = createNodeForValue(rootObject)
				root.path = New TreePath(rootObject)
				addMapping(root)
				root.updatePreferredSize(0)
				visibleNodes.Clear()
				If rootVisible Then visibleNodes.Add(root)
				If Not root.expanded Then
					root.expand()
				Else
					Dim cursor As System.Collections.IEnumerator = root.children()
					Do While cursor.hasMoreElements()
						visibleNodes.Add(cursor.nextElement())
					Loop
					If Not fixedRowHeight Then updateYLocationsFrom(0)
				End If
			Else
				visibleNodes.Clear()
				root = Nothing
			End If
			If clearSelection AndAlso treeSelectionModel IsNot Nothing Then treeSelectionModel.clearSelection()
			Me.visibleNodesChanged()
		End Sub

		''' <summary>
		''' Creates a new node to represent the node at <I>childIndex</I> in
		''' <I>parent</I>s children.  This should be called if the node doesn't
		''' already exist and <I>parent</I> has been expanded at least once.
		''' The newly created node will be made visible if <I>parent</I> is
		''' currently expanded.  This does not update the position of any
		''' cells, nor update the selection if it needs to be.  If succesful
		''' in creating the new TreeStateNode, it is returned, otherwise
		''' null is returned.
		''' </summary>
		Private Function createNodeAt(ByVal parent As TreeStateNode, ByVal childIndex As Integer) As TreeStateNode
			Dim isParentRoot As Boolean
			Dim newValue As Object
			Dim newChildNode As TreeStateNode

			newValue = treeModel.getChild(parent.value, childIndex)
			newChildNode = createNodeForValue(newValue)
			parent.insert(newChildNode, childIndex)
			newChildNode.updatePreferredSize(-1)
			isParentRoot = (parent Is root)
			If newChildNode IsNot Nothing AndAlso parent.expanded AndAlso (parent.row <> -1 OrElse isParentRoot) Then
				Dim newRow As Integer

				' Find the new row to insert this newly visible node at. 
				If childIndex = 0 Then
					If isParentRoot AndAlso (Not rootVisible) Then
						newRow = 0
					Else
						newRow = parent.row + 1
					End If
				ElseIf childIndex = parent.childCount Then
					newRow = parent.lastVisibleNode.row + 1
				Else
					Dim previousNode As TreeStateNode

					previousNode = CType(parent.getChildAt(childIndex - 1), TreeStateNode)
					newRow = previousNode.lastVisibleNode.row + 1
				End If
				visibleNodes.Insert(newRow, newChildNode)
			End If
			Return newChildNode
		End Function

		''' <summary>
		''' Returns the TreeStateNode identified by path.  This mirrors
		''' the behavior of getNodeForPath, but tries to take advantage of
		''' path if it is an instance of AbstractTreePath.
		''' </summary>
		Private Function getNodeForPath(ByVal path As TreePath, ByVal onlyIfVisible As Boolean, ByVal shouldCreate As Boolean) As TreeStateNode
			If path IsNot Nothing Then
				Dim ___node As TreeStateNode

				___node = getMapping(path)
				If ___node IsNot Nothing Then
					If onlyIfVisible AndAlso (Not ___node.visible) Then Return Nothing
					Return ___node
				End If

				' Check all the parent paths, until a match is found.
				Dim paths As Stack(Of TreePath)

				If tempStacks.Count = 0 Then
					paths = New Stack(Of TreePath)
				Else
					paths = tempStacks.Pop()
				End If

				Try
					paths.Push(path)
					path = path.parentPath
					___node = Nothing
					Do While path IsNot Nothing
						___node = getMapping(path)
						If ___node IsNot Nothing Then
							' Found a match, create entries for all paths in
							' paths.
							Do While ___node IsNot Nothing AndAlso paths.Count > 0
								path = paths.Pop()
								___node.getLoadedChildren(shouldCreate)

								Dim childIndex As Integer = treeModel.getIndexOfChild(___node.userObject, path.lastPathComponent)

								If childIndex = -1 OrElse childIndex >= ___node.childCount OrElse (onlyIfVisible AndAlso (Not ___node.visible)) Then
									___node = Nothing
								Else
									___node = CType(___node.getChildAt(childIndex), TreeStateNode)
								End If
							Loop
							Return ___node
						End If
						paths.Push(path)
						path = path.parentPath
					Loop
				Finally
					paths.removeAllElements()
					tempStacks.Push(paths)
				End Try
				' If we get here it means they share a different root!
				' We could throw an exception...
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Updates the y locations of all of the visible nodes after
		''' location.
		''' </summary>
		Private Sub updateYLocationsFrom(ByVal location As Integer)
			If location >= 0 AndAlso location < rowCount Then
				Dim counter, maxCounter, newYOrigin As Integer
				Dim aNode As TreeStateNode

				aNode = getNode(location)
				newYOrigin = aNode.yOrigin + aNode.preferredHeight
				counter = location + 1
				maxCounter = visibleNodes.Count
				Do While counter < maxCounter
					aNode = CType(visibleNodes(counter), TreeStateNode)
					aNode.yOrigin = newYOrigin
					newYOrigin += aNode.preferredHeight
					counter += 1
				Loop
			End If
		End Sub

		''' <summary>
		''' Resets the y origin of all the visible nodes as well as messaging
		''' all the visible nodes to updatePreferredSize().  You should not
		''' normally have to call this.  Expanding and contracting the nodes
		''' automaticly adjusts the locations.
		''' updateAll determines if updatePreferredSize() is call on all nodes
		''' or just those that don't have a valid size.
		''' </summary>
		Private Sub updateNodeSizes(ByVal updateAll As Boolean)
			Dim aY, counter, maxCounter As Integer
			Dim ___node As TreeStateNode

			___updateNodeSizes = False
			counter = 0
			aY = counter
			maxCounter = visibleNodes.Count
			Do While counter < maxCounter
				___node = CType(visibleNodes(counter), TreeStateNode)
				___node.yOrigin = aY
				If updateAll OrElse (Not ___node.hasValidSize()) Then ___node.updatePreferredSize(counter)
				aY += ___node.preferredHeight
				counter += 1
			Loop
		End Sub

		''' <summary>
		''' Returns the index of the row containing location.  If there
		''' are no rows, -1 is returned.  If location is beyond the last
		''' row index, the last row index is returned.
		''' </summary>
		Private Function getRowContainingYLocation(ByVal location As Integer) As Integer
			If fixedRowHeight Then
				If rowCount = 0 Then Return -1
				Return Math.Max(0, Math.Min(rowCount - 1, location \ rowHeight))
			End If

			Dim max, maxY, mid, min, minY As Integer
			Dim ___node As TreeStateNode

			max = rowCount
			If max <= 0 Then Return -1
				min = 0
				mid = min
			Do While min < max
				mid = (max - min) \ 2 + min
				___node = CType(visibleNodes(mid), TreeStateNode)
				minY = ___node.yOrigin
				maxY = minY + ___node.preferredHeight
				If location < minY Then
					max = mid - 1
				ElseIf location >= maxY Then
					min = mid + 1
				Else
					Exit Do
				End If
			Loop
			If min = max Then
				mid = min
				If mid >= rowCount Then mid = rowCount - 1
			End If
			Return mid
		End Function

		''' <summary>
		''' Ensures that all the path components in path are expanded, accept
		''' for the last component which will only be expanded if expandLast
		''' is true.
		''' Returns true if succesful in finding the path.
		''' </summary>
		Private Sub ensurePathIsExpanded(ByVal aPath As TreePath, ByVal expandLast As Boolean)
			If aPath IsNot Nothing Then
				' Make sure the last entry isn't a leaf.
				If treeModel.isLeaf(aPath.lastPathComponent) Then
					aPath = aPath.parentPath
					expandLast = True
				End If
				If aPath IsNot Nothing Then
					Dim lastNode As TreeStateNode = getNodeForPath(aPath, False, True)

					If lastNode IsNot Nothing Then
						lastNode.makeVisible()
						If expandLast Then lastNode.expand()
					End If
				End If
			End If
		End Sub

		''' <summary>
		''' Returns the AbstractTreeUI.VisibleNode displayed at the given row
		''' </summary>
		Private Function getNode(ByVal row As Integer) As TreeStateNode
			Return CType(visibleNodes(row), TreeStateNode)
		End Function

		''' <summary>
		''' Returns the maximum node width.
		''' </summary>
		Private Property maxNodeWidth As Integer
			Get
				Dim maxWidth As Integer = 0
				Dim nodeWidth As Integer
				Dim counter As Integer
				Dim ___node As TreeStateNode
    
				For counter = rowCount - 1 To 0 Step -1
					___node = Me.getNode(counter)
					nodeWidth = ___node.preferredWidth + ___node.xOrigin
					If nodeWidth > maxWidth Then maxWidth = nodeWidth
				Next counter
				Return maxWidth
			End Get
		End Property

		''' <summary>
		''' Responsible for creating a TreeStateNode that will be used
		''' to track display information about value.
		''' </summary>
		Private Function createNodeForValue(ByVal value As Object) As TreeStateNode
			Return New TreeStateNode(Me, value)
		End Function


		''' <summary>
		''' TreeStateNode is used to keep track of each of
		''' the nodes that have been expanded. This will also cache the preferred
		''' size of the value it represents.
		''' </summary>
		Private Class TreeStateNode
			Inherits DefaultMutableTreeNode

			Private ReadOnly outerInstance As VariableHeightLayoutCache

			''' <summary>
			''' Preferred size needed to draw the user object. </summary>
			Protected Friend preferredWidth As Integer
			Protected Friend preferredHeight As Integer

			''' <summary>
			''' X location that the user object will be drawn at. </summary>
			Protected Friend xOrigin As Integer

			''' <summary>
			''' Y location that the user object will be drawn at. </summary>
			Protected Friend yOrigin As Integer

			''' <summary>
			''' Is this node currently expanded? </summary>
			Protected Friend expanded As Boolean

			''' <summary>
			''' Has this node been expanded at least once? </summary>
			Protected Friend ___hasBeenExpanded As Boolean

			''' <summary>
			''' Path of this node. </summary>
			Protected Friend path As TreePath


			Public Sub New(ByVal outerInstance As VariableHeightLayoutCache, ByVal value As Object)
					Me.outerInstance = outerInstance
				MyBase.New(value)
			End Sub

			'
			' Overriden DefaultMutableTreeNode methods
			'

			''' <summary>
			''' Messaged when this node is added somewhere, resets the path
			''' and adds a mapping from path to this node.
			''' </summary>
			Public Overrides Property parent As MutableTreeNode
				Set(ByVal parent As MutableTreeNode)
					MyBase.parent = parent
					If parent IsNot Nothing Then
						path = CType(parent, TreeStateNode).treePath.pathByAddingChild(userObject)
						outerInstance.addMapping(Me)
					End If
				End Set
			End Property

			''' <summary>
			''' Messaged when this node is removed from its parent, this messages
			''' <code>removedFromMapping</code> to remove all the children.
			''' </summary>
			Public Overrides Sub remove(ByVal childIndex As Integer)
				Dim node As TreeStateNode = CType(getChildAt(childIndex), TreeStateNode)

				node.removeFromMapping()
				MyBase.remove(childIndex)
			End Sub

			''' <summary>
			''' Messaged to set the user object. This resets the path.
			''' </summary>
			Public Overrides Property userObject As Object
				Set(ByVal o As Object)
					MyBase.userObject = o
					If path IsNot Nothing Then
						Dim ___parent As TreeStateNode = CType(parent, TreeStateNode)
    
						If ___parent IsNot Nothing Then
							resetChildrenPaths(___parent.treePath)
						Else
							resetChildrenPaths(Nothing)
						End If
					End If
				End Set
			End Property

			''' <summary>
			''' Returns the children of the receiver.
			''' If the receiver is not currently expanded, this will return an
			''' empty enumeration.
			''' </summary>
			Public Overrides Function children() As System.Collections.IEnumerator
				If Not Me.expanded Then
					Return DefaultMutableTreeNode.EMPTY_ENUMERATION
				Else
					Return MyBase.children()
				End If
			End Function

			''' <summary>
			''' Returns true if the receiver is a leaf.
			''' </summary>
			Public Property Overrides leaf As Boolean
				Get
					Return outerInstance.model.isLeaf(Me.value)
				End Get
			End Property

			'
			' VariableHeightLayoutCache
			'

			''' <summary>
			''' Returns the location and size of this node.
			''' </summary>
			Public Overridable Function getNodeBounds(ByVal placeIn As java.awt.Rectangle) As java.awt.Rectangle
				If placeIn Is Nothing Then
					placeIn = New java.awt.Rectangle(xOrigin, yOrigin, preferredWidth, preferredHeight)
				Else
					placeIn.x = xOrigin
					placeIn.y = yOrigin
					placeIn.width = preferredWidth
					placeIn.height = preferredHeight
				End If
				Return placeIn
			End Function

			''' <returns> x location to draw node at. </returns>
			Public Overridable Property xOrigin As Integer
				Get
					If Not hasValidSize() Then updatePreferredSize(row)
					Return xOrigin
				End Get
			End Property

			''' <summary>
			''' Returns the y origin the user object will be drawn at.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
            Public Overridable Function getYOrigin() As Integer 'JavaToDotNetTempPropertyGetyOrigin
			Public Overridable Property yOrigin As Integer
				Get
					If outerInstance.fixedRowHeight Then
						Dim aRow As Integer = row
    
						If aRow = -1 Then Return -1
						Return outerInstance.rowHeight * aRow
					End If
					Return yOrigin
				End Get
				Set(ByVal newYOrigin As Integer)
			End Property

			''' <summary>
			''' Returns the preferred height of the receiver.
			''' </summary>
			Public Overridable Property preferredHeight As Integer
				Get
					If outerInstance.fixedRowHeight Then
						Return outerInstance.rowHeight
					ElseIf Not hasValidSize() Then
						updatePreferredSize(row)
					End If
					Return preferredHeight
				End Get
			End Property

			''' <summary>
			''' Returns the preferred width of the receiver.
			''' </summary>
			Public Overridable Property preferredWidth As Integer
				Get
					If Not hasValidSize() Then updatePreferredSize(row)
					Return preferredWidth
				End Get
			End Property

			''' <summary>
			''' Returns true if this node has a valid size.
			''' </summary>
			Public Overridable Function hasValidSize() As Boolean
				Return (preferredHeight <> 0)
			End Function

			''' <summary>
			''' Returns the row of the receiver.
			''' </summary>
			Public Overridable Property row As Integer
				Get
					Return outerInstance.visibleNodes.IndexOf(Me)
				End Get
			End Property

			''' <summary>
			''' Returns true if this node has been expanded at least once.
			''' </summary>
			Public Overridable Function hasBeenExpanded() As Boolean
				Return ___hasBeenExpanded
			End Function

			''' <summary>
			''' Returns true if the receiver has been expanded.
			''' </summary>
			Public Overridable Property expanded As Boolean
				Get
					Return expanded
				End Get
			End Property

			''' <summary>
			''' Returns the last visible node that is a child of this
			''' instance.
			''' </summary>
			Public Overridable Property lastVisibleNode As TreeStateNode
				Get
					Dim node As TreeStateNode = Me
    
					Do While node.expanded AndAlso node.childCount > 0
						node = CType(node.lastChild, TreeStateNode)
					Loop
					Return node
				End Get
			End Property

			''' <summary>
			''' Returns true if the receiver is currently visible.
			''' </summary>
			Public Overridable Property visible As Boolean
				Get
					If Me Is outerInstance.root Then Return True
    
					Dim ___parent As TreeStateNode = CType(parent, TreeStateNode)
    
					Return (___parent IsNot Nothing AndAlso ___parent.expanded AndAlso ___parent.visible)
				End Get
			End Property

			''' <summary>
			''' Returns the number of children this will have. If the children
			''' have not yet been loaded, this messages the model.
			''' </summary>
			Public Overridable Property modelChildCount As Integer
				Get
					If ___hasBeenExpanded Then Return MyBase.childCount
					Return outerInstance.model.getChildCount(value)
				End Get
			End Property

			''' <summary>
			''' Returns the number of visible children, that is the number of
			''' children that are expanded, or leafs.
			''' </summary>
			Public Overridable Property visibleChildCount As Integer
				Get
					Dim ___childCount As Integer = 0
    
					If expanded Then
						Dim maxCounter As Integer = childCount
    
						___childCount += maxCounter
						For counter As Integer = 0 To maxCounter - 1
							___childCount += CType(getChildAt(counter), TreeStateNode).visibleChildCount
						Next counter
					End If
					Return ___childCount
				End Get
			End Property

			''' <summary>
			''' Toggles the receiver between expanded and collapsed.
			''' </summary>
			Public Overridable Sub toggleExpanded()
				If expanded Then
					collapse()
				Else
					expand()
				End If
			End Sub

			''' <summary>
			''' Makes the receiver visible, but invoking
			''' <code>expandParentAndReceiver</code> on the superclass.
			''' </summary>
			Public Overridable Sub makeVisible()
				Dim ___parent As TreeStateNode = CType(parent, TreeStateNode)

				If ___parent IsNot Nothing Then ___parent.expandParentAndReceiver()
			End Sub

			''' <summary>
			''' Expands the receiver.
			''' </summary>
			Public Overridable Sub expand()
				expand(True)
			End Sub

			''' <summary>
			''' Collapses the receiver.
			''' </summary>
			Public Overridable Sub collapse()
				collapse(True)
			End Sub

			''' <summary>
			''' Returns the value the receiver is representing. This is a cover
			''' for getUserObject.
			''' </summary>
			Public Overridable Property value As Object
				Get
					Return userObject
				End Get
			End Property

			''' <summary>
			''' Returns a TreePath instance for this node.
			''' </summary>
			Public Overridable Property treePath As TreePath
				Get
					Return path
				End Get
			End Property

			'
			' Local methods
			'

			''' <summary>
			''' Recreates the receivers path, and all its children's paths.
			''' </summary>
			Protected Friend Overridable Sub resetChildrenPaths(ByVal parentPath As TreePath)
				outerInstance.removeMapping(Me)
				If parentPath Is Nothing Then
					path = New TreePath(userObject)
				Else
					path = parentPath.pathByAddingChild(userObject)
				End If
				outerInstance.addMapping(Me)
				For counter As Integer = childCount - 1 To 0 Step -1
					CType(getChildAt(counter), TreeStateNode).resetChildrenPaths(path)
				Next counter
			End Sub

				yOrigin = newYOrigin
			End Sub

			''' <summary>
			''' Shifts the y origin by <code>offset</code>.
			''' </summary>
			Protected Friend Overridable Sub shiftYOriginBy(ByVal offset As Integer)
				yOrigin += offset
			End Sub

			''' <summary>
			''' Updates the receivers preferredSize by invoking
			''' <code>updatePreferredSize</code> with an argument of -1.
			''' </summary>
			Protected Friend Overridable Sub updatePreferredSize()
				updatePreferredSize(row)
			End Sub

			''' <summary>
			''' Updates the preferred size by asking the current renderer
			''' for the Dimension needed to draw the user object this
			''' instance represents.
			''' </summary>
			Protected Friend Overridable Sub updatePreferredSize(ByVal index As Integer)
				Dim bounds As java.awt.Rectangle = outerInstance.getNodeDimensions(Me.userObject, index, level, expanded, outerInstance.boundsBuffer)

				If bounds Is Nothing Then
					xOrigin = 0
						preferredHeight = 0
						preferredWidth = preferredHeight
					outerInstance.___updateNodeSizes = True
				ElseIf bounds.height = 0 Then
					xOrigin = 0
						preferredHeight = 0
						preferredWidth = preferredHeight
					outerInstance.___updateNodeSizes = True
				Else
					xOrigin = bounds.x
					preferredWidth = bounds.width
					If outerInstance.fixedRowHeight Then
						preferredHeight = outerInstance.rowHeight
					Else
						preferredHeight = bounds.height
					End If
				End If
			End Sub

			''' <summary>
			''' Marks the receivers size as invalid. Next time the size, location
			''' is asked for it will be obtained.
			''' </summary>
			Protected Friend Overridable Sub markSizeInvalid()
				preferredHeight = 0
			End Sub

			''' <summary>
			''' Marks the receivers size, and all its descendants sizes, as invalid.
			''' </summary>
			Protected Friend Overridable Sub deepMarkSizeInvalid()
				markSizeInvalid()
				For counter As Integer = childCount - 1 To 0 Step -1
					CType(getChildAt(counter), TreeStateNode).deepMarkSizeInvalid()
				Next counter
			End Sub

			''' <summary>
			''' Returns the children of the receiver. If the children haven't
			''' been loaded from the model and
			''' <code>createIfNeeded</code> is true, the children are first
			''' loaded.
			''' </summary>
			Protected Friend Overridable Function getLoadedChildren(ByVal createIfNeeded As Boolean) As System.Collections.IEnumerator
				If (Not createIfNeeded) OrElse ___hasBeenExpanded Then Return MyBase.children()

				Dim newNode As TreeStateNode
				Dim realNode As Object = value
				Dim treeModel As TreeModel = outerInstance.model
				Dim count As Integer = treeModel.getChildCount(realNode)

				___hasBeenExpanded = True

				Dim childRow As Integer = row

				If childRow = -1 Then
					For i As Integer = 0 To count - 1
						newNode = outerInstance.createNodeForValue(treeModel.getChild(realNode, i))
						Me.add(newNode)
						newNode.updatePreferredSize(-1)
					Next i
				Else
					childRow += 1
					For i As Integer = 0 To count - 1
						newNode = outerInstance.createNodeForValue(treeModel.getChild(realNode, i))
						Me.add(newNode)
						newNode.updatePreferredSize(childRow)
						childRow += 1
					Next i
				End If
				Return MyBase.children()
			End Function

			''' <summary>
			''' Messaged from expand and collapse. This is meant for subclassers
			''' that may wish to do something interesting with this.
			''' </summary>
			Protected Friend Overridable Sub didAdjustTree()
			End Sub

			''' <summary>
			''' Invokes <code>expandParentAndReceiver</code> on the parent,
			''' and expands the receiver.
			''' </summary>
			Protected Friend Overridable Sub expandParentAndReceiver()
				Dim ___parent As TreeStateNode = CType(parent, TreeStateNode)

				If ___parent IsNot Nothing Then ___parent.expandParentAndReceiver()
				expand()
			End Sub

			''' <summary>
			''' Expands this node in the tree.  This will load the children
			''' from the treeModel if this node has not previously been
			''' expanded.  If <I>adjustTree</I> is true the tree and selection
			''' are updated accordingly.
			''' </summary>
			Protected Friend Overridable Sub expand(ByVal adjustTree As Boolean)
				If (Not expanded) AndAlso (Not leaf) Then
					Dim isFixed As Boolean = outerInstance.fixedRowHeight
					Dim startHeight As Integer = preferredHeight
					Dim originalRow As Integer = row

					expanded = True
					updatePreferredSize(originalRow)

					If Not ___hasBeenExpanded Then
						Dim newNode As TreeStateNode
						Dim realNode As Object = value
						Dim treeModel As TreeModel = outerInstance.model
						Dim count As Integer = treeModel.getChildCount(realNode)

						___hasBeenExpanded = True
						If originalRow = -1 Then
							For i As Integer = 0 To count - 1
								newNode = outerInstance.createNodeForValue(treeModel.getChild(realNode, i))
								Me.add(newNode)
								newNode.updatePreferredSize(-1)
							Next i
						Else
							Dim offset As Integer = originalRow + 1
							For i As Integer = 0 To count - 1
								newNode = outerInstance.createNodeForValue(treeModel.getChild(realNode, i))
								Me.add(newNode)
								newNode.updatePreferredSize(offset)
							Next i
						End If
					End If

					Dim i As Integer = originalRow
					Dim cursor As System.Collections.IEnumerator = preorderEnumeration()
					cursor.nextElement() ' don't add me, I'm already in

					Dim newYOrigin As Integer

					If isFixed Then
						newYOrigin = 0
					ElseIf Me Is outerInstance.root AndAlso (Not outerInstance.rootVisible) Then
						newYOrigin = 0
					Else
						newYOrigin = yOrigin + Me.preferredHeight
					End If
					Dim aNode As TreeStateNode
					If Not isFixed Then
						Do While cursor.hasMoreElements()
							aNode = CType(cursor.nextElement(), TreeStateNode)
							If (Not outerInstance.___updateNodeSizes) AndAlso (Not aNode.hasValidSize()) Then aNode.updatePreferredSize(i + 1)
							aNode.yOrigin = newYOrigin
							newYOrigin += aNode.preferredHeight
							i += 1
							outerInstance.visibleNodes.Insert(i, aNode)
						Loop
					Else
						Do While cursor.hasMoreElements()
							aNode = CType(cursor.nextElement(), TreeStateNode)
							i += 1
							outerInstance.visibleNodes.Insert(i, aNode)
						Loop
					End If

					If adjustTree AndAlso (originalRow <> i OrElse preferredHeight <> startHeight) Then
						' Adjust the Y origin of any nodes following this row.
						i += 1
						If (Not isFixed) AndAlso i < outerInstance.rowCount Then
							Dim counter As Integer
							Dim heightDiff As Integer = newYOrigin - (yOrigin + preferredHeight) + (preferredHeight - startHeight)

							For counter = outerInstance.visibleNodes.Count - 1 To i Step -1
								CType(outerInstance.visibleNodes(counter), TreeStateNode).shiftYOriginBy(heightDiff)
							Next counter
						End If
						didAdjustTree()
						outerInstance.visibleNodesChanged()
					End If

					' Update the rows in the selection
					If outerInstance.treeSelectionModel IsNot Nothing Then outerInstance.treeSelectionModel.resetRowSelection()
				End If
			End Sub

			''' <summary>
			''' Collapses this node in the tree.  If <I>adjustTree</I> is
			''' true the tree and selection are updated accordingly.
			''' </summary>
			Protected Friend Overridable Sub collapse(ByVal adjustTree As Boolean)
				If expanded Then
					Dim cursor As System.Collections.IEnumerator = preorderEnumeration()
					cursor.nextElement() ' don't remove me, I'm still visible
					Dim rowsDeleted As Integer = 0
					Dim isFixed As Boolean = outerInstance.fixedRowHeight
					Dim lastYEnd As Integer
					If isFixed Then
						lastYEnd = 0
					Else
						lastYEnd = preferredHeight + yOrigin
					End If
					Dim startHeight As Integer = preferredHeight
					Dim startYEnd As Integer = lastYEnd
					Dim myRow As Integer = row

					If Not isFixed Then
						Do While cursor.hasMoreElements()
							Dim node As TreeStateNode = CType(cursor.nextElement(), TreeStateNode)
							If node.visible Then
								rowsDeleted += 1
								'visibleNodes.removeElement(node);
								lastYEnd = node.yOrigin + node.preferredHeight
							End If
						Loop
					Else
						Do While cursor.hasMoreElements()
							Dim node As TreeStateNode = CType(cursor.nextElement(), TreeStateNode)
							If node.visible Then rowsDeleted += 1
						Loop
					End If

					' Clean up the visible nodes.
					For counter As Integer = rowsDeleted + myRow To myRow + 1 Step -1
						outerInstance.visibleNodes.RemoveAt(counter)
					Next counter

					expanded = False

					If myRow = -1 Then
						markSizeInvalid()
					ElseIf adjustTree Then
						updatePreferredSize(myRow)
					End If

					If myRow <> -1 AndAlso adjustTree AndAlso (rowsDeleted > 0 OrElse startHeight <> preferredHeight) Then
						' Adjust the Y origin of any rows following this one.
						startYEnd += (preferredHeight - startHeight)
						If (Not isFixed) AndAlso (myRow + 1) < outerInstance.rowCount AndAlso startYEnd <> lastYEnd Then
							Dim counter, maxCounter, shiftAmount As Integer

							shiftAmount = startYEnd - lastYEnd
							counter = myRow + 1
							maxCounter = outerInstance.visibleNodes.Count
							Do While counter < maxCounter
								CType(outerInstance.visibleNodes(counter), TreeStateNode).shiftYOriginBy(shiftAmount)
								counter += 1
							Loop
						End If
						didAdjustTree()
						outerInstance.visibleNodesChanged()
					End If
					If outerInstance.treeSelectionModel IsNot Nothing AndAlso rowsDeleted > 0 AndAlso myRow <> -1 Then outerInstance.treeSelectionModel.resetRowSelection()
				End If
			End Sub

			''' <summary>
			''' Removes the receiver, and all its children, from the mapping
			''' table.
			''' </summary>
			Protected Friend Overridable Sub removeFromMapping()
				If path IsNot Nothing Then
					outerInstance.removeMapping(Me)
					For counter As Integer = childCount - 1 To 0 Step -1
						CType(getChildAt(counter), TreeStateNode).removeFromMapping()
					Next counter
				End If
			End Sub
		End Class ' End of VariableHeightLayoutCache.TreeStateNode


		''' <summary>
		''' An enumerator to iterate through visible nodes.
		''' </summary>
		Private Class VisibleTreeStateNodeEnumeration
			Implements System.Collections.IEnumerator(Of TreePath)

			Private ReadOnly outerInstance As VariableHeightLayoutCache

			''' <summary>
			''' Parent thats children are being enumerated. </summary>
			Protected Friend parent As TreeStateNode
			''' <summary>
			''' Index of next child. An index of -1 signifies parent should be
			''' visibled next. 
			''' </summary>
			Protected Friend nextIndex As Integer
			''' <summary>
			''' Number of children in parent. </summary>
			Protected Friend childCount As Integer

			Protected Friend Sub New(ByVal outerInstance As VariableHeightLayoutCache, ByVal node As TreeStateNode)
					Me.outerInstance = outerInstance
				Me.New(node, -1)
			End Sub

			Protected Friend Sub New(ByVal outerInstance As VariableHeightLayoutCache, ByVal parent As TreeStateNode, ByVal startIndex As Integer)
					Me.outerInstance = outerInstance
				Me.parent = parent
				Me.nextIndex = startIndex
				Me.childCount = Me.parent.childCount
			End Sub

			''' <returns> true if more visible nodes. </returns>
			Public Overridable Function hasMoreElements() As Boolean
				Return (parent IsNot Nothing)
			End Function

			''' <returns> next visible TreePath. </returns>
			Public Overridable Function nextElement() As TreePath
				If Not hasMoreElements() Then Throw New java.util.NoSuchElementException("No more visible paths")

				Dim retObject As TreePath

				If nextIndex = -1 Then
					retObject = parent.treePath
				Else
					Dim node As TreeStateNode = CType(parent.getChildAt(nextIndex), TreeStateNode)

					retObject = node.treePath
				End If
				updateNextObject()
				Return retObject
			End Function

			''' <summary>
			''' Determines the next object by invoking <code>updateNextIndex</code>
			''' and if not succesful <code>findNextValidParent</code>.
			''' </summary>
			Protected Friend Overridable Sub updateNextObject()
				If Not updateNextIndex() Then findNextValidParent()
			End Sub

			''' <summary>
			''' Finds the next valid parent, this should be called when nextIndex
			''' is beyond the number of children of the current parent.
			''' </summary>
			Protected Friend Overridable Function findNextValidParent() As Boolean
				If parent Is outerInstance.root Then
					' mark as invalid!
					parent = Nothing
					Return False
				End If
				Do While parent IsNot Nothing
					Dim newParent As TreeStateNode = CType(parent.parent, TreeStateNode)

					If newParent IsNot Nothing Then
						nextIndex = newParent.getIndex(parent)
						parent = newParent
						childCount = parent.childCount
						If updateNextIndex() Then Return True
					Else
						parent = Nothing
					End If
				Loop
				Return False
			End Function

			''' <summary>
			''' Updates <code>nextIndex</code> returning false if it is beyond
			''' the number of children of parent.
			''' </summary>
			Protected Friend Overridable Function updateNextIndex() As Boolean
				' nextIndex == -1 identifies receiver, make sure is expanded
				' before descend.
				If nextIndex = -1 AndAlso (Not parent.expanded) Then Return False

				' Check that it can have kids
				If childCount = 0 Then
					Return False
				' Make sure next index not beyond child count.
				Else
					nextIndex += 1
					If nextIndex >= childCount Then Return False
					End If

				Dim child As TreeStateNode = CType(parent.getChildAt(nextIndex), TreeStateNode)

				If child IsNot Nothing AndAlso child.expanded Then
					parent = child
					nextIndex = -1
					childCount = child.childCount
				End If
				Return True
			End Function
		End Class ' VariableHeightLayoutCache.VisibleTreeStateNodeEnumeration
	End Class

End Namespace
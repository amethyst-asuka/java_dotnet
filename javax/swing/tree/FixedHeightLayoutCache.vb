Imports System
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
	''' @author Scott Violet
	''' </summary>

	Public Class FixedHeightLayoutCache
		Inherits AbstractLayoutCache

		''' <summary>
		''' Root node. </summary>
		Private root As FHTreeStateNode

		''' <summary>
		''' Number of rows currently visible. </summary>
		Private rowCount As Integer

		''' <summary>
		''' Used in getting sizes for nodes to avoid creating a new Rectangle
		''' every time a size is needed.
		''' </summary>
		Private boundsBuffer As java.awt.Rectangle

		''' <summary>
		''' Maps from TreePath to a FHTreeStateNode.
		''' </summary>
		Private treePathMapping As Dictionary(Of TreePath, FHTreeStateNode)

		''' <summary>
		''' Used for getting path/row information.
		''' </summary>
		Private info As SearchInfo

		Private tempStacks As Stack(Of Stack(Of TreePath))


		Public Sub New()
			MyBase.New()
			tempStacks = New Stack(Of Stack(Of TreePath))
			boundsBuffer = New java.awt.Rectangle
			treePathMapping = New Dictionary(Of TreePath, FHTreeStateNode)
			info = New SearchInfo(Me)
			rowHeight = 1
		End Sub

		''' <summary>
		''' Sets the TreeModel that will provide the data.
		''' </summary>
		''' <param name="newModel"> the TreeModel that is to provide the data </param>
		Public Overrides Property model As TreeModel
			Set(ByVal newModel As TreeModel)
				MyBase.model = newModel
				rebuild(False)
			End Set
		End Property

		''' <summary>
		''' Determines whether or not the root node from
		''' the TreeModel is visible.
		''' </summary>
		''' <param name="rootVisible"> true if the root node of the tree is to be displayed </param>
		''' <seealso cref= #rootVisible </seealso>
		Public Overrides Property rootVisible As Boolean
			Set(ByVal rootVisible As Boolean)
				If rootVisible <> rootVisible Then
					MyBase.rootVisible = rootVisible
					If root IsNot Nothing Then
						If rootVisible Then
							rowCount += 1
							root.adjustRowBy(1)
						Else
							rowCount -= 1
							root.adjustRowBy(-1)
						End If
						visibleNodesChanged()
					End If
				End If
			End Set
		End Property

		''' <summary>
		''' Sets the height of each cell. If rowHeight is less than or equal to
		''' 0 this will throw an IllegalArgumentException.
		''' </summary>
		''' <param name="rowHeight"> the height of each cell, in pixels </param>
		Public Overrides Property rowHeight As Integer
			Set(ByVal rowHeight As Integer)
				If rowHeight <= 0 Then Throw New System.ArgumentException("FixedHeightLayoutCache only supports row heights greater than 0")
				If rowHeight <> rowHeight Then
					MyBase.rowHeight = rowHeight
					visibleNodesChanged()
				End If
			End Set
		End Property

		''' <summary>
		''' Returns the number of visible rows.
		''' </summary>
		Public Property Overrides rowCount As Integer
			Get
				Return rowCount
			End Get
		End Property

		''' <summary>
		''' Does nothing, FixedHeightLayoutCache doesn't cache width, and that
		''' is all that could change.
		''' </summary>
		Public Overrides Sub invalidatePathBounds(ByVal path As TreePath)
		End Sub


		''' <summary>
		''' Informs the TreeState that it needs to recalculate all the sizes
		''' it is referencing.
		''' </summary>
		Public Overrides Sub invalidateSizes()
			' Nothing to do here, rowHeight still same, which is all
			' this is interested in, visible region may have changed though.
			visibleNodesChanged()
		End Sub

		''' <summary>
		''' Returns true if the value identified by row is currently expanded.
		''' </summary>
		Public Overrides Function isExpanded(ByVal path As TreePath) As Boolean
			If path IsNot Nothing Then
				Dim lastNode As FHTreeStateNode = getNodeForPath(path, True, False)

				Return (lastNode IsNot Nothing AndAlso lastNode.expanded)
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a rectangle giving the bounds needed to draw path.
		''' </summary>
		''' <param name="path">     a TreePath specifying a node </param>
		''' <param name="placeIn">  a Rectangle object giving the available space </param>
		''' <returns> a Rectangle object specifying the space to be used </returns>
		Public Overrides Function getBounds(ByVal path As TreePath, ByVal placeIn As java.awt.Rectangle) As java.awt.Rectangle
			If path Is Nothing Then Return Nothing

			Dim node As FHTreeStateNode = getNodeForPath(path, True, False)

			If node IsNot Nothing Then Return getBounds(node, -1, placeIn)

			' node hasn't been created yet.
			Dim parentPath As TreePath = path.parentPath

			node = getNodeForPath(parentPath, True, False)
			If node IsNot Nothing AndAlso node.expanded Then
				Dim childIndex As Integer = treeModel.getIndexOfChild(parentPath.lastPathComponent, path.lastPathComponent)

				If childIndex <> -1 Then Return getBounds(node, childIndex, placeIn)
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Returns the path for passed in row.  If row is not visible
		''' null is returned.
		''' </summary>
		Public Overrides Function getPathForRow(ByVal row As Integer) As TreePath
			If row >= 0 AndAlso row < rowCount Then
				If root.getPathForRow(row, rowCount, info) Then Return info.path
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Returns the row that the last item identified in path is visible
		''' at.  Will return -1 if any of the elements in path are not
		''' currently visible.
		''' </summary>
		Public Overrides Function getRowForPath(ByVal path As TreePath) As Integer
			If path Is Nothing OrElse root Is Nothing Then Return -1

			Dim node As FHTreeStateNode = getNodeForPath(path, True, False)

			If node IsNot Nothing Then Return node.row

			Dim parentPath As TreePath = path.parentPath

			node = getNodeForPath(parentPath, True, False)
			If node IsNot Nothing AndAlso node.expanded Then Return node.getRowToModelIndex(treeModel.getIndexOfChild(parentPath.lastPathComponent, path.lastPathComponent))
			Return -1
		End Function

		''' <summary>
		''' Returns the path to the node that is closest to x,y.  If
		''' there is nothing currently visible this will return null, otherwise
		''' it'll always return a valid path.  If you need to test if the
		''' returned object is exactly at x, y you should get the bounds for
		''' the returned path and test x, y against that.
		''' </summary>
		Public Overrides Function getPathClosestTo(ByVal x As Integer, ByVal y As Integer) As TreePath
			If rowCount = 0 Then Return Nothing

			Dim row As Integer = getRowContainingYLocation(y)

			Return getPathForRow(row)
		End Function

		''' <summary>
		''' Returns the number of visible children for row.
		''' </summary>
		Public Overrides Function getVisibleChildCount(ByVal path As TreePath) As Integer
			Dim node As FHTreeStateNode = getNodeForPath(path, True, False)

			If node Is Nothing Then Return 0
			Return node.totalChildCount
		End Function

		''' <summary>
		''' Returns an Enumerator that increments over the visible paths
		''' starting at the passed in location. The ordering of the enumeration
		''' is based on how the paths are displayed.
		''' </summary>
		Public Overrides Function getVisiblePathsFrom(ByVal path As TreePath) As System.Collections.IEnumerator(Of TreePath)
			If path Is Nothing Then Return Nothing

			Dim node As FHTreeStateNode = getNodeForPath(path, True, False)

			If node IsNot Nothing Then Return New VisibleFHTreeStateNodeEnumeration(Me, node)
			Dim parentPath As TreePath = path.parentPath

			node = getNodeForPath(parentPath, True, False)
			If node IsNot Nothing AndAlso node.expanded Then Return New VisibleFHTreeStateNodeEnumeration(Me, node, treeModel.getIndexOfChild(parentPath.lastPathComponent, path.lastPathComponent))
			Return Nothing
		End Function

		''' <summary>
		''' Marks the path <code>path</code> expanded state to
		''' <code>isExpanded</code>.
		''' </summary>
		Public Overrides Sub setExpandedState(ByVal path As TreePath, ByVal isExpanded As Boolean)
			If isExpanded Then
				ensurePathIsExpanded(path, True)
			ElseIf path IsNot Nothing Then
				Dim parentPath As TreePath = path.parentPath

				' YECK! Make the parent expanded.
				If parentPath IsNot Nothing Then
					Dim parentNode As FHTreeStateNode = getNodeForPath(parentPath, False, True)
					If parentNode IsNot Nothing Then parentNode.makeVisible()
				End If
				' And collapse the child.
				Dim childNode As FHTreeStateNode = getNodeForPath(path, True, False)

				If childNode IsNot Nothing Then childNode.collapse(True)
			End If
		End Sub

		''' <summary>
		''' Returns true if the path is expanded, and visible.
		''' </summary>
		Public Overrides Function getExpandedState(ByVal path As TreePath) As Boolean
			Dim node As FHTreeStateNode = getNodeForPath(path, True, False)

			Return If(node IsNot Nothing, (node.visible AndAlso node.expanded), False)
		End Function

		'
		' TreeModelListener methods
		'

		''' <summary>
		''' <p>Invoked after a node (or a set of siblings) has changed in some
		''' way. The node(s) have not changed locations in the tree or
		''' altered their children arrays, but other attributes have
		''' changed and may affect presentation. Example: the name of a
		''' file has changed, but it is in the same location in the file
		''' system.</p>
		''' 
		''' <p>e.path() returns the path the parent of the changed node(s).</p>
		''' 
		''' <p>e.childIndices() returns the index(es) of the changed node(s).</p>
		''' </summary>
		Public Overrides Sub treeNodesChanged(ByVal e As javax.swing.event.TreeModelEvent)
			If e IsNot Nothing Then
				Dim changedIndexs As Integer()
				Dim changedParent As FHTreeStateNode = getNodeForPath(sun.swing.SwingUtilities2.getTreePath(e, model), False, False)
				Dim maxCounter As Integer

				changedIndexs = e.childIndices
	'             Only need to update the children if the node has been
	'               expanded once. 
				' PENDING(scott): make sure childIndexs is sorted!
				If changedParent IsNot Nothing Then
					maxCounter = changedIndexs.Length
					If changedIndexs IsNot Nothing AndAlso maxCounter > 0 Then
						Dim parentValue As Object = changedParent.userObject

						For counter As Integer = 0 To maxCounter - 1
							Dim child As FHTreeStateNode = changedParent.getChildAtModelIndex(changedIndexs(counter))

							If child IsNot Nothing Then child.userObject = treeModel.getChild(parentValue, changedIndexs(counter))
						Next counter
						If changedParent.visible AndAlso changedParent.expanded Then visibleNodesChanged()
					' Null for root indicates it changed.
					ElseIf changedParent Is root AndAlso changedParent.visible AndAlso changedParent.expanded Then
						visibleNodesChanged()
					End If
				End If
			End If
		End Sub

		''' <summary>
		''' <p>Invoked after nodes have been inserted into the tree.</p>
		''' 
		''' <p>e.path() returns the parent of the new nodes
		''' <p>e.childIndices() returns the indices of the new nodes in
		''' ascending order.
		''' </summary>
		Public Overrides Sub treeNodesInserted(ByVal e As javax.swing.event.TreeModelEvent)
			If e IsNot Nothing Then
				Dim changedIndexs As Integer()
				Dim changedParent As FHTreeStateNode = getNodeForPath(sun.swing.SwingUtilities2.getTreePath(e, model), False, False)
				Dim maxCounter As Integer

				changedIndexs = e.childIndices
	'             Only need to update the children if the node has been
	'               expanded once. 
				' PENDING(scott): make sure childIndexs is sorted!
				maxCounter = changedIndexs.Length
				If changedParent IsNot Nothing AndAlso changedIndexs IsNot Nothing AndAlso maxCounter > 0 Then
					Dim isVisible As Boolean = (changedParent.visible AndAlso changedParent.expanded)

					For counter As Integer = 0 To maxCounter - 1
						changedParent.childInsertedAtModelIndex(changedIndexs(counter), isVisible)
					Next counter
					If isVisible AndAlso treeSelectionModel IsNot Nothing Then treeSelectionModel.resetRowSelection()
					If changedParent.visible Then Me.visibleNodesChanged()
				End If
			End If
		End Sub

		''' <summary>
		''' <p>Invoked after nodes have been removed from the tree.  Note that
		''' if a subtree is removed from the tree, this method may only be
		''' invoked once for the root of the removed subtree, not once for
		''' each individual set of siblings removed.</p>
		''' 
		''' <p>e.path() returns the former parent of the deleted nodes.</p>
		''' 
		''' <p>e.childIndices() returns the indices the nodes had before they were deleted in ascending order.</p>
		''' </summary>
		Public Overrides Sub treeNodesRemoved(ByVal e As javax.swing.event.TreeModelEvent)
			If e IsNot Nothing Then
				Dim changedIndexs As Integer()
				Dim maxCounter As Integer
				Dim parentPath As TreePath = sun.swing.SwingUtilities2.getTreePath(e, model)
				Dim changedParentNode As FHTreeStateNode = getNodeForPath(parentPath, False, False)

				changedIndexs = e.childIndices
				' PENDING(scott): make sure that changedIndexs are sorted in
				' ascending order.
				maxCounter = changedIndexs.Length
				If changedParentNode IsNot Nothing AndAlso changedIndexs IsNot Nothing AndAlso maxCounter > 0 Then
					Dim children As Object() = e.children
					Dim isVisible As Boolean = (changedParentNode.visible AndAlso changedParentNode.expanded)

					For counter As Integer = maxCounter - 1 To 0 Step -1
						changedParentNode.removeChildAtModelIndex(changedIndexs(counter), isVisible)
					Next counter
					If isVisible Then
						If treeSelectionModel IsNot Nothing Then treeSelectionModel.resetRowSelection()
						If treeModel.getChildCount(changedParentNode.userObject) = 0 AndAlso changedParentNode.leaf Then changedParentNode.collapse(False)
						visibleNodesChanged()
					ElseIf changedParentNode.visible Then
						visibleNodesChanged()
					End If
				End If
			End If
		End Sub

		''' <summary>
		''' <p>Invoked after the tree has drastically changed structure from a
		''' given node down.  If the path returned by e.getPath() is of length
		''' one and the first element does not identify the current root node
		''' the first element should become the new root of the tree.
		''' 
		''' <p>e.path() holds the path to the node.</p>
		''' <p>e.childIndices() returns null.</p>
		''' </summary>
		Public Overrides Sub treeStructureChanged(ByVal e As javax.swing.event.TreeModelEvent)
			If e IsNot Nothing Then
				Dim changedPath As TreePath = sun.swing.SwingUtilities2.getTreePath(e, model)
				Dim changedNode As FHTreeStateNode = getNodeForPath(changedPath, False, False)

				' Check if root has changed, either to a null root, or
				' to an entirely new root.
				If changedNode Is root OrElse (changedNode Is Nothing AndAlso ((changedPath Is Nothing AndAlso treeModel IsNot Nothing AndAlso treeModel.root Is Nothing) OrElse (changedPath IsNot Nothing AndAlso changedPath.pathCount <= 1))) Then
					rebuild(True)
				ElseIf changedNode IsNot Nothing Then
					Dim wasExpanded, wasVisible As Boolean
					Dim parent As FHTreeStateNode = CType(changedNode.parent, FHTreeStateNode)

					wasExpanded = changedNode.expanded
					wasVisible = changedNode.visible

					Dim index As Integer = parent.getIndex(changedNode)
					changedNode.collapse(False)
					parent.remove(index)

					If wasVisible AndAlso wasExpanded Then
						Dim row As Integer = changedNode.row
						parent.resetChildrenRowsFrom(row, index, changedNode.childIndex)
						changedNode = getNodeForPath(changedPath, False, True)
						changedNode.expand()
					End If
					If treeSelectionModel IsNot Nothing AndAlso wasVisible AndAlso wasExpanded Then treeSelectionModel.resetRowSelection()
					If wasVisible Then Me.visibleNodesChanged()
				End If
			End If
		End Sub


		'
		' Local methods
		'

		Private Sub visibleNodesChanged()
		End Sub

		''' <summary>
		''' Returns the bounds for the given node. If <code>childIndex</code>
		''' is -1, the bounds of <code>parent</code> are returned, otherwise
		''' the bounds of the node at <code>childIndex</code> are returned.
		''' </summary>
		Private Function getBounds(ByVal parent As FHTreeStateNode, ByVal childIndex As Integer, ByVal placeIn As java.awt.Rectangle) As java.awt.Rectangle
			Dim ___expanded As Boolean
			Dim level As Integer
			Dim row As Integer
			Dim value As Object

			If childIndex = -1 Then
				' Getting bounds for parent
				row = parent.row
				value = parent.userObject
				___expanded = parent.expanded
				level = parent.level
			Else
				row = parent.getRowToModelIndex(childIndex)
				value = treeModel.getChild(parent.userObject, childIndex)
				___expanded = False
				level = parent.level + 1
			End If

			Dim ___bounds As java.awt.Rectangle = getNodeDimensions(value, row, level, ___expanded, boundsBuffer)
			' No node dimensions, bail.
			If ___bounds Is Nothing Then Return Nothing

			If placeIn Is Nothing Then placeIn = New java.awt.Rectangle

			placeIn.x = ___bounds.x
			placeIn.height = rowHeight
			placeIn.y = row * placeIn.height
			placeIn.width = ___bounds.width
			Return placeIn
		End Function

		''' <summary>
		''' Adjust the large row count of the AbstractTreeUI the receiver was
		''' created with.
		''' </summary>
		Private Sub adjustRowCountBy(ByVal changeAmount As Integer)
			rowCount += changeAmount
		End Sub

		''' <summary>
		''' Adds a mapping for node.
		''' </summary>
		Private Sub addMapping(ByVal node As FHTreeStateNode)
			treePathMapping(node.treePath) = node
		End Sub

		''' <summary>
		''' Removes the mapping for a previously added node.
		''' </summary>
		Private Sub removeMapping(ByVal node As FHTreeStateNode)
			treePathMapping.Remove(node.treePath)
		End Sub

		''' <summary>
		''' Returns the node previously added for <code>path</code>. This may
		''' return null, if you to create a node use getNodeForPath.
		''' </summary>
		Private Function getMapping(ByVal path As TreePath) As FHTreeStateNode
			Return treePathMapping(path)
		End Function

		''' <summary>
		''' Sent to completely rebuild the visible tree. All nodes are collapsed.
		''' </summary>
		Private Sub rebuild(ByVal clearSelection As Boolean)
			Dim rootUO As Object

			treePathMapping.Clear()
			rootUO = treeModel.root
			If treeModel IsNot Nothing AndAlso rootUO IsNot Nothing Then
				root = createNodeForValue(rootUO, 0)
				root.path = New TreePath(rootUO)
				addMapping(root)
				If rootVisible Then
					rowCount = 1
					root.row = 0
				Else
					rowCount = 0
					root.row = -1
				End If
				root.expand()
			Else
				root = Nothing
				rowCount = 0
			End If
			If clearSelection AndAlso treeSelectionModel IsNot Nothing Then treeSelectionModel.clearSelection()
			Me.visibleNodesChanged()
		End Sub

		''' <summary>
		''' Returns the index of the row containing location.  If there
		''' are no rows, -1 is returned.  If location is beyond the last
		''' row index, the last row index is returned.
		''' </summary>
		Private Function getRowContainingYLocation(ByVal location As Integer) As Integer
			If rowCount = 0 Then Return -1
			Return Math.Max(0, Math.Min(rowCount - 1, location \ rowHeight))
		End Function

		''' <summary>
		''' Ensures that all the path components in path are expanded, accept
		''' for the last component which will only be expanded if expandLast
		''' is true.
		''' Returns true if succesful in finding the path.
		''' </summary>
		Private Function ensurePathIsExpanded(ByVal aPath As TreePath, ByVal expandLast As Boolean) As Boolean
			If aPath IsNot Nothing Then
				' Make sure the last entry isn't a leaf.
				If treeModel.isLeaf(aPath.lastPathComponent) Then
					aPath = aPath.parentPath
					expandLast = True
				End If
				If aPath IsNot Nothing Then
					Dim lastNode As FHTreeStateNode = getNodeForPath(aPath, False, True)

					If lastNode IsNot Nothing Then
						lastNode.makeVisible()
						If expandLast Then lastNode.expand()
						Return True
					End If
				End If
			End If
			Return False
		End Function

		''' <summary>
		''' Creates and returns an instance of FHTreeStateNode.
		''' </summary>
		Private Function createNodeForValue(ByVal value As Object, ByVal childIndex As Integer) As FHTreeStateNode
			Return New FHTreeStateNode(Me, value, childIndex, -1)
		End Function

		''' <summary>
		''' Messages getTreeNodeForPage(path, onlyIfVisible, shouldCreate,
		''' path.length) as long as path is non-null and the length is {@literal >} 0.
		''' Otherwise returns null.
		''' </summary>
		Private Function getNodeForPath(ByVal path As TreePath, ByVal onlyIfVisible As Boolean, ByVal shouldCreate As Boolean) As FHTreeStateNode
			If path IsNot Nothing Then
				Dim node As FHTreeStateNode

				node = getMapping(path)
				If node IsNot Nothing Then
					If onlyIfVisible AndAlso (Not node.visible) Then Return Nothing
					Return node
				End If
				If onlyIfVisible Then Return Nothing

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
					node = Nothing
					Do While path IsNot Nothing
						node = getMapping(path)
						If node IsNot Nothing Then
							' Found a match, create entries for all paths in
							' paths.
							Do While node IsNot Nothing AndAlso paths.Count > 0
								path = paths.Pop()
								node = node.createChildFor(path.lastPathComponent)
							Loop
							Return node
						End If
						paths.Push(path)
						path = path.parentPath
					Loop
				Finally
					paths.removeAllElements()
					tempStacks.Push(paths)
				End Try
				' If we get here it means they share a different root!
				Return Nothing
			End If
			Return Nothing
		End Function

		''' <summary>
		''' FHTreeStateNode is used to track what has been expanded.
		''' FHTreeStateNode differs from VariableHeightTreeState.TreeStateNode
		''' in that it is highly model intensive. That is almost all queries to a
		''' FHTreeStateNode result in the TreeModel being queried. And it
		''' obviously does not support variable sized row heights.
		''' </summary>
		Private Class FHTreeStateNode
			Inherits DefaultMutableTreeNode

			Private ReadOnly outerInstance As FixedHeightLayoutCache

			''' <summary>
			''' Is this node expanded? </summary>
			Protected Friend ___isExpanded As Boolean

			''' <summary>
			''' Index of this node from the model. </summary>
			Protected Friend childIndex As Integer

			''' <summary>
			''' Child count of the receiver. </summary>
			Protected Friend childCount As Integer

			''' <summary>
			''' Row of the receiver. This is only valid if the row is expanded.
			''' </summary>
			Protected Friend row As Integer

			''' <summary>
			''' Path of this node. </summary>
			Protected Friend path As TreePath


			Public Sub New(ByVal outerInstance As FixedHeightLayoutCache, ByVal userObject As Object, ByVal childIndex As Integer, ByVal row As Integer)
					Me.outerInstance = outerInstance
				MyBase.New(userObject)
				Me.childIndex = childIndex
				Me.row = row
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
						path = CType(parent, FHTreeStateNode).treePath.pathByAddingChild(userObject)
						outerInstance.addMapping(Me)
					End If
				End Set
			End Property

			''' <summary>
			''' Messaged when this node is removed from its parent, this messages
			''' <code>removedFromMapping</code> to remove all the children.
			''' </summary>
			Public Overrides Sub remove(ByVal childIndex As Integer)
				Dim node As FHTreeStateNode = CType(getChildAt(childIndex), FHTreeStateNode)

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
						Dim ___parent As FHTreeStateNode = CType(parent, FHTreeStateNode)
    
						If ___parent IsNot Nothing Then
							resetChildrenPaths(___parent.treePath)
						Else
							resetChildrenPaths(Nothing)
						End If
					End If
				End Set
			End Property

			'
			'

			''' <summary>
			''' Returns the index of the receiver in the model.
			''' </summary>
			Public Overridable Property childIndex As Integer
				Get
					Return childIndex
				End Get
			End Property

			''' <summary>
			''' Returns the <code>TreePath</code> of the receiver.
			''' </summary>
			Public Overridable Property treePath As TreePath
				Get
					Return path
				End Get
			End Property

			''' <summary>
			''' Returns the child for the passed in model index, this will
			''' return <code>null</code> if the child for <code>index</code>
			''' has not yet been created (expanded).
			''' </summary>
			Public Overridable Function getChildAtModelIndex(ByVal index As Integer) As FHTreeStateNode
				' PENDING: Make this a binary search!
				For counter As Integer = childCount - 1 To 0 Step -1
					If CType(getChildAt(counter), FHTreeStateNode).childIndex = index Then Return CType(getChildAt(counter), FHTreeStateNode)
				Next counter
				Return Nothing
			End Function

			''' <summary>
			''' Returns true if this node is visible. This is determined by
			''' asking all the parents if they are expanded.
			''' </summary>
			Public Overridable Property visible As Boolean
				Get
					Dim ___parent As FHTreeStateNode = CType(parent, FHTreeStateNode)
    
					If ___parent Is Nothing Then Return True
					Return (___parent.expanded AndAlso ___parent.visible)
				End Get
			End Property

			''' <summary>
			''' Returns the row of the receiver.
			''' </summary>
			Public Overridable Property row As Integer
				Get
					Return row
				End Get
			End Property

			''' <summary>
			''' Returns the row of the child with a model index of
			''' <code>index</code>.
			''' </summary>
			Public Overridable Function getRowToModelIndex(ByVal index As Integer) As Integer
				Dim child As FHTreeStateNode
				Dim lastRow As Integer = row + 1
				Dim retValue As Integer = lastRow

				' This too could be a binary search!
				Dim counter As Integer = 0
				Dim maxCounter As Integer = childCount
				Do While counter < maxCounter
					child = CType(getChildAt(counter), FHTreeStateNode)
					If child.childIndex >= index Then
						If child.childIndex = index Then Return child.row
						If counter = 0 Then Return row + 1 + index
						Return child.row - (child.childIndex - index)
					End If
					counter += 1
				Loop
				' YECK!
				Return row + 1 + totalChildCount - (childCount - index)
			End Function

			''' <summary>
			''' Returns the number of children in the receiver by descending all
			''' expanded nodes and messaging them with getTotalChildCount.
			''' </summary>
			Public Overridable Property totalChildCount As Integer
				Get
					If expanded Then
						Dim ___parent As FHTreeStateNode = CType(parent, FHTreeStateNode)
						Dim pIndex As Integer
    
						pIndex = ___parent.getIndex(Me)
						If ___parent IsNot Nothing AndAlso pIndex + 1 < ___parent.childCount Then
							' This node has a created sibling, to calc total
							' child count directly from that!
							Dim ___nextSibling As FHTreeStateNode = CType(___parent.getChildAt(pIndex + 1), FHTreeStateNode)
    
							Return ___nextSibling.row - row - (___nextSibling.childIndex - childIndex)
						Else
							Dim retCount As Integer = childCount
    
							For counter As Integer = childCount - 1 To 0 Step -1
								retCount += CType(getChildAt(counter), FHTreeStateNode).totalChildCount
							Next counter
							Return retCount
						End If
					End If
					Return 0
				End Get
			End Property

			''' <summary>
			''' Returns true if this node is expanded.
			''' </summary>
			Public Overridable Property expanded As Boolean
				Get
					Return ___isExpanded
				End Get
			End Property

			''' <summary>
			''' The highest visible nodes have a depth of 0.
			''' </summary>
			Public Overridable Property visibleLevel As Integer
				Get
					If outerInstance.rootVisible Then
						Return level
					Else
						Return level - 1
					End If
				End Get
			End Property

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
					CType(getChildAt(counter), FHTreeStateNode).resetChildrenPaths(path)
				Next counter
			End Sub

			''' <summary>
			''' Removes the receiver, and all its children, from the mapping
			''' table.
			''' </summary>
			Protected Friend Overridable Sub removeFromMapping()
				If path IsNot Nothing Then
					outerInstance.removeMapping(Me)
					For counter As Integer = childCount - 1 To 0 Step -1
						CType(getChildAt(counter), FHTreeStateNode).removeFromMapping()
					Next counter
				End If
			End Sub

			''' <summary>
			''' Creates a new node to represent <code>userObject</code>.
			''' This does NOT check to ensure there isn't already a child node
			''' to manage <code>userObject</code>.
			''' </summary>
			Protected Friend Overridable Function createChildFor(ByVal userObject As Object) As FHTreeStateNode
				Dim newChildIndex As Integer = outerInstance.treeModel.getIndexOfChild(userObject, userObject)

				If newChildIndex < 0 Then Return Nothing

				Dim aNode As FHTreeStateNode
				Dim child As FHTreeStateNode = outerInstance.createNodeForValue(userObject, newChildIndex)
				Dim childRow As Integer

				If visible Then
					childRow = getRowToModelIndex(newChildIndex)
				Else
					childRow = -1
				End If
				child.row = childRow
				Dim counter As Integer = 0
				Dim maxCounter As Integer = childCount
				Do While counter < maxCounter
					aNode = CType(getChildAt(counter), FHTreeStateNode)
					If aNode.childIndex > newChildIndex Then
						insert(child, counter)
						Return child
					End If
					counter += 1
				Loop
				add(child)
				Return child
			End Function

			''' <summary>
			''' Adjusts the receiver, and all its children rows by
			''' <code>amount</code>.
			''' </summary>
			Protected Friend Overridable Sub adjustRowBy(ByVal amount As Integer)
				row += amount
				If ___isExpanded Then
					For counter As Integer = childCount - 1 To 0 Step -1
						CType(getChildAt(counter), FHTreeStateNode).adjustRowBy(amount)
					Next counter
				End If
			End Sub

			''' <summary>
			''' Adjusts this node, its child, and its parent starting at
			''' an index of <code>index</code> index is the index of the child
			''' to start adjusting from, which is not necessarily the model
			''' index.
			''' </summary>
			Protected Friend Overridable Sub adjustRowBy(ByVal amount As Integer, ByVal startIndex As Integer)
				' Could check isVisible, but probably isn't worth it.
				If ___isExpanded Then
					' children following startIndex.
					For counter As Integer = childCount - 1 To startIndex Step -1
						CType(getChildAt(counter), FHTreeStateNode).adjustRowBy(amount)
					Next counter
				End If
				' Parent
				Dim ___parent As FHTreeStateNode = CType(parent, FHTreeStateNode)

				If ___parent IsNot Nothing Then ___parent.adjustRowBy(amount, ___parent.getIndex(Me) + 1)
			End Sub

			''' <summary>
			''' Messaged when the node has expanded. This updates all of
			''' the receivers children rows, as well as the total row count.
			''' </summary>
			Protected Friend Overridable Sub didExpand()
				Dim nextRow As Integer = rowAndChildrenren(row)
				Dim ___parent As FHTreeStateNode = CType(parent, FHTreeStateNode)
				Dim childRowCount As Integer = nextRow - row - 1

				If ___parent IsNot Nothing Then ___parent.adjustRowBy(childRowCount, ___parent.getIndex(Me) + 1)
				outerInstance.adjustRowCountBy(childRowCount)
			End Sub

			''' <summary>
			''' Sets the receivers row to <code>nextRow</code> and recursively
			''' updates all the children of the receivers rows. The index the
			''' next row is to be placed as is returned.
			''' </summary>
			Protected Friend Overridable Function setRowAndChildren(ByVal nextRow As Integer) As Integer
				row = nextRow

				If Not expanded Then Return row + 1

				Dim lastRow As Integer = row + 1
				Dim lastModelIndex As Integer = 0
				Dim child As FHTreeStateNode
				Dim maxCounter As Integer = childCount

				For counter As Integer = 0 To maxCounter - 1
					child = CType(getChildAt(counter), FHTreeStateNode)
					lastRow += (child.childIndex - lastModelIndex)
					lastModelIndex = child.childIndex + 1
					If child.___isExpanded Then
						lastRow = child.rowAndChildrenren(lastRow)
					Else
						child.row = lastRow
						lastRow += 1
					End If
				Next counter
				Return lastRow + childCount - lastModelIndex
			End Function

			''' <summary>
			''' Resets the receivers children's rows. Starting with the child
			''' at <code>childIndex</code> (and <code>modelIndex</code>) to
			''' <code>newRow</code>. This uses <code>setRowAndChildren</code>
			''' to recursively descend children, and uses
			''' <code>resetRowSelection</code> to ascend parents.
			''' </summary>
			' This can be rather expensive, but is needed for the collapse
			' case this is resulting from a remove (although I could fix
			' that by having instances of FHTreeStateNode hold a ref to
			' the number of children). I prefer this though, making determing
			' the row of a particular node fast is very nice!
			Protected Friend Overridable Sub resetChildrenRowsFrom(ByVal newRow As Integer, ByVal childIndex As Integer, ByVal modelIndex As Integer)
				Dim lastRow As Integer = newRow
				Dim lastModelIndex As Integer = modelIndex
				Dim node As FHTreeStateNode
				Dim maxCounter As Integer = childCount

				For counter As Integer = childIndex To maxCounter - 1
					node = CType(getChildAt(counter), FHTreeStateNode)
					lastRow += (node.childIndex - lastModelIndex)
					lastModelIndex = node.childIndex + 1
					If node.___isExpanded Then
						lastRow = node.rowAndChildrenren(lastRow)
					Else
						node.row = lastRow
						lastRow += 1
					End If
				Next counter
				lastRow += childCount - lastModelIndex
				node = CType(parent, FHTreeStateNode)
				If node IsNot Nothing Then
					node.resetChildrenRowsFrom(lastRow, node.getIndex(Me) + 1, Me.childIndex + 1)
				Else ' This is the root, reset total ROWCOUNT!
					outerInstance.rowCount = lastRow
				End If
			End Sub

			''' <summary>
			''' Makes the receiver visible, but invoking
			''' <code>expandParentAndReceiver</code> on the superclass.
			''' </summary>
			Protected Friend Overridable Sub makeVisible()
				Dim ___parent As FHTreeStateNode = CType(parent, FHTreeStateNode)

				If ___parent IsNot Nothing Then ___parent.expandParentAndReceiver()
			End Sub

			''' <summary>
			''' Invokes <code>expandParentAndReceiver</code> on the parent,
			''' and expands the receiver.
			''' </summary>
			Protected Friend Overridable Sub expandParentAndReceiver()
				Dim ___parent As FHTreeStateNode = CType(parent, FHTreeStateNode)

				If ___parent IsNot Nothing Then ___parent.expandParentAndReceiver()
				expand()
			End Sub

			''' <summary>
			''' Expands the receiver.
			''' </summary>
			Protected Friend Overridable Sub expand()
				If (Not ___isExpanded) AndAlso (Not leaf) Then
					Dim ___visible As Boolean = visible

					___isExpanded = True
					childCount = outerInstance.treeModel.getChildCount(userObject)

					If ___visible Then didExpand()

					' Update the selection model.
					If ___visible AndAlso outerInstance.treeSelectionModel IsNot Nothing Then outerInstance.treeSelectionModel.resetRowSelection()
				End If
			End Sub

			''' <summary>
			''' Collapses the receiver. If <code>adjustRows</code> is true,
			''' the rows of nodes after the receiver are adjusted.
			''' </summary>
			Protected Friend Overridable Sub collapse(ByVal adjustRows As Boolean)
				If ___isExpanded Then
					If visible AndAlso adjustRows Then
						Dim ___childCount As Integer = totalChildCount

						___isExpanded = False
						outerInstance.adjustRowCountBy(-___childCount)
						' We can do this because adjustRowBy won't descend
						' the children.
						adjustRowBy(-___childCount, 0)
					Else
						___isExpanded = False
					End If

					If adjustRows AndAlso visible AndAlso outerInstance.treeSelectionModel IsNot Nothing Then outerInstance.treeSelectionModel.resetRowSelection()
				End If
			End Sub

			''' <summary>
			''' Returns true if the receiver is a leaf.
			''' </summary>
			Public Property Overrides leaf As Boolean
				Get
					Dim model As TreeModel = outerInstance.model
    
					Return If(model IsNot Nothing, model.isLeaf(Me.userObject), True)
				End Get
			End Property

			''' <summary>
			''' Adds newChild to this nodes children at the appropriate location.
			''' The location is determined from the childIndex of newChild.
			''' </summary>
			Protected Friend Overridable Sub addNode(ByVal newChild As FHTreeStateNode)
				Dim added As Boolean = False
				Dim ___childIndex As Integer = newChild.childIndex

				Dim counter As Integer = 0
				Dim maxCounter As Integer = childCount
				Do While counter < maxCounter
					If CType(getChildAt(counter), FHTreeStateNode).childIndex > ___childIndex Then
						added = True
						insert(newChild, counter)
						counter = maxCounter
					End If
					counter += 1
				Loop
				If Not added Then add(newChild)
			End Sub

			''' <summary>
			''' Removes the child at <code>modelIndex</code>.
			''' <code>isChildVisible</code> should be true if the receiver
			''' is visible and expanded.
			''' </summary>
			Protected Friend Overridable Sub removeChildAtModelIndex(ByVal modelIndex As Integer, ByVal isChildVisible As Boolean)
				Dim childNode As FHTreeStateNode = getChildAtModelIndex(modelIndex)

				If childNode IsNot Nothing Then
					Dim ___row As Integer = childNode.row
					Dim ___index As Integer = getIndex(childNode)

					childNode.collapse(False)
					remove(___index)
					adjustChildIndexs(___index, -1)
					childCount -= 1
					If isChildVisible Then resetChildrenRowsFrom(___row, ___index, modelIndex)
				Else
					Dim maxCounter As Integer = childCount
					Dim aChild As FHTreeStateNode

					For counter As Integer = 0 To maxCounter - 1
						aChild = CType(getChildAt(counter), FHTreeStateNode)
						If aChild.childIndex >= modelIndex Then
							If isChildVisible Then
								adjustRowBy(-1, counter)
								outerInstance.adjustRowCountBy(-1)
							End If
							' Since matched and children are always sorted by
							' index, no need to continue testing with the
							' above.
							Do While counter < maxCounter
								CType(getChildAt(counter), FHTreeStateNode).childIndex -= 1
								counter += 1
							Loop
							childCount -= 1
							Return
						End If
					Next counter
					' No children to adjust, but it was a child, so we still need
					' to adjust nodes after this one.
					If isChildVisible Then
						adjustRowBy(-1, maxCounter)
						outerInstance.adjustRowCountBy(-1)
					End If
					childCount -= 1
				End If
			End Sub

			''' <summary>
			''' Adjusts the child indexs of the receivers children by
			''' <code>amount</code>, starting at <code>index</code>.
			''' </summary>
			Protected Friend Overridable Sub adjustChildIndexs(ByVal index As Integer, ByVal amount As Integer)
				Dim counter As Integer = index
				Dim maxCounter As Integer = childCount
				Do While counter < maxCounter
					CType(getChildAt(counter), FHTreeStateNode).childIndex += amount
					counter += 1
				Loop
			End Sub

			''' <summary>
			''' Messaged when a child has been inserted at index. For all the
			''' children that have a childIndex &ge; index their index is incremented
			''' by one.
			''' </summary>
			Protected Friend Overridable Sub childInsertedAtModelIndex(ByVal index As Integer, ByVal isExpandedAndVisible As Boolean)
				Dim aChild As FHTreeStateNode
				Dim maxCounter As Integer = childCount

				For counter As Integer = 0 To maxCounter - 1
					aChild = CType(getChildAt(counter), FHTreeStateNode)
					If aChild.childIndex >= index Then
						If isExpandedAndVisible Then
							adjustRowBy(1, counter)
							outerInstance.adjustRowCountBy(1)
						End If
	'                     Since matched and children are always sorted by
	'                       index, no need to continue testing with the above. 
						Do While counter < maxCounter
							CType(getChildAt(counter), FHTreeStateNode).childIndex += 1
							counter += 1
						Loop
						childCount += 1
						Return
					End If
				Next counter
				' No children to adjust, but it was a child, so we still need
				' to adjust nodes after this one.
				If isExpandedAndVisible Then
					adjustRowBy(1, maxCounter)
					outerInstance.adjustRowCountBy(1)
				End If
				childCount += 1
			End Sub

			''' <summary>
			''' Returns true if there is a row for <code>row</code>.
			''' <code>nextRow</code> gives the bounds of the receiver.
			''' Information about the found row is returned in <code>info</code>.
			''' This should be invoked on root with <code>nextRow</code> set
			''' to <code>getRowCount</code>().
			''' </summary>
			Protected Friend Overridable Function getPathForRow(ByVal row As Integer, ByVal nextRow As Integer, ByVal info As SearchInfo) As Boolean
				If Me.row = row Then
					info.node = Me
					info.isNodeParentNode = False
					info.childIndex = childIndex
					Return True
				End If

				Dim child As FHTreeStateNode
				Dim ___lastChild As FHTreeStateNode = Nothing

				Dim counter As Integer = 0
				Dim maxCounter As Integer = childCount
				Do While counter < maxCounter
					child = CType(getChildAt(counter), FHTreeStateNode)
					If child.row > row Then
						If counter = 0 Then
							' No node exists for it, and is first.
							info.node = Me
							info.isNodeParentNode = True
							info.childIndex = row - Me.row - 1
							Return True
						Else
							' May have been in last child's bounds.
							Dim lastChildEndRow As Integer = 1 + child.row - (child.childIndex - ___lastChild.childIndex)

							If row < lastChildEndRow Then Return ___lastChild.getPathForRow(row, lastChildEndRow, info)
							' Between last child and child, but not in last child
							info.node = Me
							info.isNodeParentNode = True
							info.childIndex = row - lastChildEndRow + ___lastChild.childIndex + 1
							Return True
						End If
					End If
					___lastChild = child
					counter += 1
				Loop

				' Not in children, but we should have it, offset from
				' nextRow.
				If ___lastChild IsNot Nothing Then
					Dim lastChildEndRow As Integer = nextRow - (childCount - ___lastChild.childIndex) + 1

					If row < lastChildEndRow Then Return ___lastChild.getPathForRow(row, lastChildEndRow, info)
					' Between last child and child, but not in last child
					info.node = Me
					info.isNodeParentNode = True
					info.childIndex = row - lastChildEndRow + ___lastChild.childIndex + 1
					Return True
				Else
					' No children.
					Dim retChildIndex As Integer = row - Me.row - 1

					If retChildIndex >= childCount Then Return False
					info.node = Me
					info.isNodeParentNode = True
					info.childIndex = retChildIndex
					Return True
				End If
			End Function

			''' <summary>
			''' Asks all the children of the receiver for their totalChildCount
			''' and returns this value (plus stopIndex).
			''' </summary>
			Protected Friend Overridable Function getCountTo(ByVal stopIndex As Integer) As Integer
				Dim aChild As FHTreeStateNode
				Dim retCount As Integer = stopIndex + 1

				Dim counter As Integer = 0
				Dim maxCounter As Integer = childCount
				Do While counter < maxCounter
					aChild = CType(getChildAt(counter), FHTreeStateNode)
					If aChild.childIndex >= stopIndex Then
						counter = maxCounter
					Else
						retCount += aChild.totalChildCount
					End If
					counter += 1
				Loop
				If parent IsNot Nothing Then Return retCount + CType(parent, FHTreeStateNode).getCountTo(childIndex)
				If Not outerInstance.rootVisible Then Return (retCount - 1)
				Return retCount
			End Function

			''' <summary>
			''' Returns the number of children that are expanded to
			''' <code>stopIndex</code>. This does not include the number
			''' of children that the child at <code>stopIndex</code> might
			''' have.
			''' </summary>
			Protected Friend Overridable Function getNumExpandedChildrenTo(ByVal stopIndex As Integer) As Integer
				Dim aChild As FHTreeStateNode
				Dim retCount As Integer = stopIndex

				Dim counter As Integer = 0
				Dim maxCounter As Integer = childCount
				Do While counter < maxCounter
					aChild = CType(getChildAt(counter), FHTreeStateNode)
					If aChild.childIndex >= stopIndex Then
						Return retCount
					Else
						retCount += aChild.totalChildCount
					End If
					counter += 1
				Loop
				Return retCount
			End Function

			''' <summary>
			''' Messaged when this node either expands or collapses.
			''' </summary>
			Protected Friend Overridable Sub didAdjustTree()
			End Sub

		End Class ' FixedHeightLayoutCache.FHTreeStateNode


		''' <summary>
		''' Used as a placeholder when getting the path in FHTreeStateNodes.
		''' </summary>
		Private Class SearchInfo
			Private ReadOnly outerInstance As FixedHeightLayoutCache

			Public Sub New(ByVal outerInstance As FixedHeightLayoutCache)
				Me.outerInstance = outerInstance
			End Sub

			Protected Friend node As FHTreeStateNode
			Protected Friend isNodeParentNode As Boolean
			Protected Friend childIndex As Integer

			Protected Friend Overridable Property path As TreePath
				Get
					If node Is Nothing Then Return Nothing
    
					If isNodeParentNode Then Return node.treePath.pathByAddingChild(outerInstance.treeModel.getChild(node.userObject, childIndex))
					Return node.path
				End Get
			End Property
		End Class ' FixedHeightLayoutCache.SearchInfo


		''' <summary>
		''' An enumerator to iterate through visible nodes.
		''' </summary>
		' This is very similar to
		' VariableHeightTreeState.VisibleTreeStateNodeEnumeration
		Private Class VisibleFHTreeStateNodeEnumeration
			Implements System.Collections.IEnumerator(Of TreePath)

			Private ReadOnly outerInstance As FixedHeightLayoutCache

			''' <summary>
			''' Parent thats children are being enumerated. </summary>
			Protected Friend parent As FHTreeStateNode
			''' <summary>
			''' Index of next child. An index of -1 signifies parent should be
			''' visibled next. 
			''' </summary>
			Protected Friend nextIndex As Integer
			''' <summary>
			''' Number of children in parent. </summary>
			Protected Friend childCount As Integer

			Protected Friend Sub New(ByVal outerInstance As FixedHeightLayoutCache, ByVal node As FHTreeStateNode)
					Me.outerInstance = outerInstance
				Me.New(node, -1)
			End Sub

			Protected Friend Sub New(ByVal outerInstance As FixedHeightLayoutCache, ByVal parent As FHTreeStateNode, ByVal startIndex As Integer)
					Me.outerInstance = outerInstance
				Me.parent = parent
				Me.nextIndex = startIndex
				Me.childCount = outerInstance.treeModel.getChildCount(Me.parent.userObject)
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
					Dim node As FHTreeStateNode = parent.getChildAtModelIndex(nextIndex)

					If node Is Nothing Then
						retObject = parent.treePath.pathByAddingChild(outerInstance.treeModel.getChild(parent.userObject, nextIndex))
					Else
						retObject = node.treePath
					End If
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
					Dim newParent As FHTreeStateNode = CType(parent.parent, FHTreeStateNode)

					If newParent IsNot Nothing Then
						nextIndex = parent.childIndex
						parent = newParent
						childCount = outerInstance.treeModel.getChildCount(parent.userObject)
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

				Dim child As FHTreeStateNode = parent.getChildAtModelIndex(nextIndex)

				If child IsNot Nothing AndAlso child.expanded Then
					parent = child
					nextIndex = -1
					childCount = outerInstance.treeModel.getChildCount(child.userObject)
				End If
				Return True
			End Function
		End Class ' FixedHeightLayoutCache.VisibleFHTreeStateNodeEnumeration
	End Class

End Namespace
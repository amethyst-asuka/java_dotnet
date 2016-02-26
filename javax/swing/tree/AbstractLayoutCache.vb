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

	Public MustInherit Class AbstractLayoutCache
		Implements RowMapper

		''' <summary>
		''' Object responsible for getting the size of a node. </summary>
		Protected Friend nodeDimensions As NodeDimensions

		''' <summary>
		''' Model providing information. </summary>
		Protected Friend treeModel As TreeModel

		''' <summary>
		''' Selection model. </summary>
		Protected Friend treeSelectionModel As TreeSelectionModel

		''' <summary>
		''' True if the root node is displayed, false if its children are
		''' the highest visible nodes.
		''' </summary>
		Protected Friend rootVisible As Boolean

		''' <summary>
		''' Height to use for each row.  If this is &lt;= 0 the renderer will be
		''' used to determine the height for each row.
		''' </summary>
		Protected Friend rowHeight As Integer


		''' <summary>
		''' Sets the renderer that is responsible for drawing nodes in the tree
		''' and which is therefore responsible for calculating the dimensions of
		''' individual nodes.
		''' </summary>
		''' <param name="nd"> a <code>NodeDimensions</code> object </param>
		Public Overridable Property nodeDimensions As NodeDimensions
			Set(ByVal nd As NodeDimensions)
				Me.nodeDimensions = nd
			End Set
			Get
				Return nodeDimensions
			End Get
		End Property


		''' <summary>
		''' Sets the <code>TreeModel</code> that will provide the data.
		''' </summary>
		''' <param name="newModel"> the <code>TreeModel</code> that is to
		'''          provide the data </param>
		Public Overridable Property model As TreeModel
			Set(ByVal newModel As TreeModel)
				treeModel = newModel
			End Set
			Get
				Return treeModel
			End Get
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
		Public Overridable Property rootVisible As Boolean
			Set(ByVal rootVisible As Boolean)
				Me.rootVisible = rootVisible
			End Set
			Get
				Return rootVisible
			End Get
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
		Public Overridable Property rowHeight As Integer
			Set(ByVal rowHeight As Integer)
				Me.rowHeight = rowHeight
			End Set
			Get
				Return rowHeight
			End Get
		End Property


		''' <summary>
		''' Sets the <code>TreeSelectionModel</code> used to manage the
		''' selection to new LSM.
		''' </summary>
		''' <param name="newLSM">  the new <code>TreeSelectionModel</code> </param>
		Public Overridable Property selectionModel As TreeSelectionModel
			Set(ByVal newLSM As TreeSelectionModel)
				If treeSelectionModel IsNot Nothing Then treeSelectionModel.rowMapper = Nothing
				treeSelectionModel = newLSM
				If treeSelectionModel IsNot Nothing Then treeSelectionModel.rowMapper = Me
			End Set
			Get
				Return treeSelectionModel
			End Get
		End Property


		''' <summary>
		''' Returns the preferred height.
		''' </summary>
		''' <returns> the preferred height </returns>
		Public Overridable Property preferredHeight As Integer
			Get
				' Get the height
				Dim ___rowCount As Integer = rowCount
    
				If ___rowCount > 0 Then
					Dim ___bounds As java.awt.Rectangle = getBounds(getPathForRow(___rowCount - 1), Nothing)
    
					If ___bounds IsNot Nothing Then Return ___bounds.y + ___bounds.height
				End If
				Return 0
			End Get
		End Property

		''' <summary>
		''' Returns the preferred width for the passed in region.
		''' The region is defined by the path closest to
		''' <code>(bounds.x, bounds.y)</code> and
		''' ends at <code>bounds.height + bounds.y</code>.
		''' If <code>bounds</code> is <code>null</code>,
		''' the preferred width for all the nodes
		''' will be returned (and this may be a VERY expensive
		''' computation).
		''' </summary>
		''' <param name="bounds"> the region being queried </param>
		''' <returns> the preferred width for the passed in region </returns>
		Public Overridable Function getPreferredWidth(ByVal bounds As java.awt.Rectangle) As Integer
			Dim ___rowCount As Integer = rowCount

			If ___rowCount > 0 Then
				' Get the width
				Dim firstPath As TreePath
				Dim endY As Integer

				If bounds Is Nothing Then
					firstPath = getPathForRow(0)
					endY = Integer.MaxValue
				Else
					firstPath = getPathClosestTo(bounds.x, bounds.y)
					endY = bounds.height + bounds.y
				End If

				Dim paths As System.Collections.IEnumerator = getVisiblePathsFrom(firstPath)

				If paths IsNot Nothing AndAlso paths.hasMoreElements() Then
					Dim pBounds As java.awt.Rectangle = getBounds(CType(paths.nextElement(), TreePath), Nothing)
					Dim width As Integer

					If pBounds IsNot Nothing Then
						width = pBounds.x + pBounds.width
						If pBounds.y >= endY Then Return width
					Else
						width = 0
					End If
					Do While pBounds IsNot Nothing AndAlso paths.hasMoreElements()
						pBounds = getBounds(CType(paths.nextElement(), TreePath), pBounds)
						If pBounds IsNot Nothing AndAlso pBounds.y < endY Then
							width = Math.Max(width, pBounds.x + pBounds.width)
						Else
							pBounds = Nothing
						End If
					Loop
					Return width
				End If
			End If
			Return 0
		End Function

		'
		' Abstract methods that must be implemented to be concrete.
		'

		''' <summary>
		''' Returns true if the value identified by row is currently expanded.
		''' </summary>
		Public MustOverride Function isExpanded(ByVal path As TreePath) As Boolean

		''' <summary>
		''' Returns a rectangle giving the bounds needed to draw path.
		''' </summary>
		''' <param name="path">     a <code>TreePath</code> specifying a node </param>
		''' <param name="placeIn">  a <code>Rectangle</code> object giving the
		'''          available space </param>
		''' <returns> a <code>Rectangle</code> object specifying the space to be used </returns>
		Public MustOverride Function getBounds(ByVal path As TreePath, ByVal placeIn As java.awt.Rectangle) As java.awt.Rectangle

		''' <summary>
		''' Returns the path for passed in row.  If row is not visible
		''' <code>null</code> is returned.
		''' </summary>
		''' <param name="row">  the row being queried </param>
		''' <returns> the <code>TreePath</code> for the given row </returns>
		Public MustOverride Function getPathForRow(ByVal row As Integer) As TreePath

		''' <summary>
		''' Returns the row that the last item identified in path is visible
		''' at.  Will return -1 if any of the elements in path are not
		''' currently visible.
		''' </summary>
		''' <param name="path"> the <code>TreePath</code> being queried </param>
		''' <returns> the row where the last item in path is visible or -1
		'''         if any elements in path aren't currently visible </returns>
		Public MustOverride Function getRowForPath(ByVal path As TreePath) As Integer

		''' <summary>
		''' Returns the path to the node that is closest to x,y.  If
		''' there is nothing currently visible this will return <code>null</code>,
		''' otherwise it'll always return a valid path.
		''' If you need to test if the
		''' returned object is exactly at x, y you should get the bounds for
		''' the returned path and test x, y against that.
		''' </summary>
		''' <param name="x"> the horizontal component of the desired location </param>
		''' <param name="y"> the vertical component of the desired location </param>
		''' <returns> the <code>TreePath</code> closest to the specified point </returns>
		Public MustOverride Function getPathClosestTo(ByVal x As Integer, ByVal y As Integer) As TreePath

		''' <summary>
		''' Returns an <code>Enumerator</code> that increments over the visible
		''' paths starting at the passed in location. The ordering of the
		''' enumeration is based on how the paths are displayed.
		''' The first element of the returned enumeration will be path,
		''' unless it isn't visible,
		''' in which case <code>null</code> will be returned.
		''' </summary>
		''' <param name="path"> the starting location for the enumeration </param>
		''' <returns> the <code>Enumerator</code> starting at the desired location </returns>
		Public MustOverride Function getVisiblePathsFrom(ByVal path As TreePath) As System.Collections.IEnumerator(Of TreePath)

		''' <summary>
		''' Returns the number of visible children for row.
		''' </summary>
		''' <param name="path">  the path being queried </param>
		''' <returns> the number of visible children for the specified path </returns>
		Public MustOverride Function getVisibleChildCount(ByVal path As TreePath) As Integer

		''' <summary>
		''' Marks the path <code>path</code> expanded state to
		''' <code>isExpanded</code>.
		''' </summary>
		''' <param name="path">  the path being expanded or collapsed </param>
		''' <param name="isExpanded"> true if the path should be expanded, false otherwise </param>
		Public MustOverride Sub setExpandedState(ByVal path As TreePath, ByVal isExpanded As Boolean)

		''' <summary>
		''' Returns true if the path is expanded, and visible.
		''' </summary>
		''' <param name="path">  the path being queried </param>
		''' <returns> true if the path is expanded and visible, false otherwise </returns>
		Public MustOverride Function getExpandedState(ByVal path As TreePath) As Boolean

		''' <summary>
		''' Number of rows being displayed.
		''' </summary>
		''' <returns> the number of rows being displayed </returns>
		Public MustOverride ReadOnly Property rowCount As Integer

		''' <summary>
		''' Informs the <code>TreeState</code> that it needs to recalculate
		''' all the sizes it is referencing.
		''' </summary>
		Public MustOverride Sub invalidateSizes()

		''' <summary>
		''' Instructs the <code>LayoutCache</code> that the bounds for
		''' <code>path</code> are invalid, and need to be updated.
		''' </summary>
		''' <param name="path"> the path being updated </param>
		Public MustOverride Sub invalidatePathBounds(ByVal path As TreePath)

		'
		' TreeModelListener methods
		' AbstractTreeState does not directly become a TreeModelListener on
		' the model, it is up to some other object to forward these methods.
		'

		''' <summary>
		''' <p>
		''' Invoked after a node (or a set of siblings) has changed in some
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
		''' <param name="e">  the <code>TreeModelEvent</code> </param>
		Public MustOverride Sub treeNodesChanged(ByVal e As javax.swing.event.TreeModelEvent)

		''' <summary>
		''' <p>Invoked after nodes have been inserted into the tree.</p>
		''' 
		''' <p>e.path() returns the parent of the new nodes</p>
		''' <p>e.childIndices() returns the indices of the new nodes in
		''' ascending order.</p>
		''' </summary>
		''' <param name="e"> the <code>TreeModelEvent</code> </param>
		Public MustOverride Sub treeNodesInserted(ByVal e As javax.swing.event.TreeModelEvent)

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
		''' <param name="e"> the <code>TreeModelEvent</code> </param>
		Public MustOverride Sub treeNodesRemoved(ByVal e As javax.swing.event.TreeModelEvent)

		''' <summary>
		''' <p>Invoked after the tree has drastically changed structure from a
		''' given node down.  If the path returned by <code>e.getPath()</code>
		''' is of length one and the first element does not identify the
		''' current root node the first element should become the new root
		''' of the tree.</p>
		''' 
		''' <p>e.path() holds the path to the node.</p>
		''' <p>e.childIndices() returns null.</p>
		''' </summary>
		''' <param name="e"> the <code>TreeModelEvent</code> </param>
		Public MustOverride Sub treeStructureChanged(ByVal e As javax.swing.event.TreeModelEvent)

		'
		' RowMapper
		'

		''' <summary>
		''' Returns the rows that the <code>TreePath</code> instances in
		''' <code>path</code> are being displayed at.
		''' This method should return an array of the same length as that passed
		''' in, and if one of the <code>TreePaths</code>
		''' in <code>path</code> is not valid its entry in the array should
		''' be set to -1.
		''' </summary>
		''' <param name="paths"> the array of <code>TreePath</code>s being queried </param>
		''' <returns> an array of the same length that is passed in containing
		'''          the rows that each corresponding where each
		'''          <code>TreePath</code> is displayed; if <code>paths</code>
		'''          is <code>null</code>, <code>null</code> is returned </returns>
		Public Overridable Function getRowsForPaths(ByVal paths As TreePath()) As Integer()
			If paths Is Nothing Then Return Nothing

			Dim numPaths As Integer = paths.Length
			Dim rows As Integer() = New Integer(numPaths - 1){}

			For counter As Integer = 0 To numPaths - 1
				rows(counter) = getRowForPath(paths(counter))
			Next counter
			Return rows
		End Function

		'
		' Local methods that subclassers may wish to use that are primarly
		' convenience methods.
		'

		''' <summary>
		''' Returns, by reference in <code>placeIn</code>,
		''' the size needed to represent <code>value</code>.
		''' If <code>inPlace</code> is <code>null</code>, a newly created
		''' <code>Rectangle</code> should be returned, otherwise the value
		''' should be placed in <code>inPlace</code> and returned. This will
		''' return <code>null</code> if there is no renderer.
		''' </summary>
		''' <param name="value"> the <code>value</code> to be represented </param>
		''' <param name="row">  row being queried </param>
		''' <param name="depth"> the depth of the row </param>
		''' <param name="expanded"> true if row is expanded, false otherwise </param>
		''' <param name="placeIn">  a <code>Rectangle</code> containing the size needed
		'''          to represent <code>value</code> </param>
		''' <returns> a <code>Rectangle</code> containing the node dimensions,
		'''          or <code>null</code> if node has no dimension </returns>
		Protected Friend Overridable Function getNodeDimensions(ByVal value As Object, ByVal row As Integer, ByVal depth As Integer, ByVal expanded As Boolean, ByVal placeIn As java.awt.Rectangle) As java.awt.Rectangle
			Dim nd As NodeDimensions = nodeDimensions

			If nd IsNot Nothing Then Return nd.getNodeDimensions(value, row, depth, expanded, placeIn)
			Return Nothing
		End Function

		''' <summary>
		''' Returns true if the height of each row is a fixed size.
		''' </summary>
		Protected Friend Overridable Property fixedRowHeight As Boolean
			Get
				Return (rowHeight > 0)
			End Get
		End Property


		''' <summary>
		''' Used by <code>AbstractLayoutCache</code> to determine the size
		''' and x origin of a particular node.
		''' </summary>
		Public MustInherit Class NodeDimensions
			''' <summary>
			''' Returns, by reference in bounds, the size and x origin to
			''' place value at. The calling method is responsible for determining
			''' the Y location. If bounds is <code>null</code>, a newly created
			''' <code>Rectangle</code> should be returned,
			''' otherwise the value should be placed in bounds and returned.
			''' </summary>
			''' <param name="value"> the <code>value</code> to be represented </param>
			''' <param name="row"> row being queried </param>
			''' <param name="depth"> the depth of the row </param>
			''' <param name="expanded"> true if row is expanded, false otherwise </param>
			''' <param name="bounds">  a <code>Rectangle</code> containing the size needed
			'''              to represent <code>value</code> </param>
			''' <returns> a <code>Rectangle</code> containing the node dimensions,
			'''              or <code>null</code> if node has no dimension </returns>
			Public MustOverride Function getNodeDimensions(ByVal value As Object, ByVal row As Integer, ByVal depth As Integer, ByVal expanded As Boolean, ByVal bounds As java.awt.Rectangle) As java.awt.Rectangle
		End Class
	End Class

End Namespace
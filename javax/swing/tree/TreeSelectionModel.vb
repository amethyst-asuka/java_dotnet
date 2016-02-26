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
	''' This interface represents the current state of the selection for
	''' the tree component.
	''' For information and examples of using tree selection models,
	''' see <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/tree.html">How to Use Trees</a>
	''' in <em>The Java Tutorial.</em>
	'''  
	''' <p>
	''' The state of the tree selection is characterized by
	''' a set of TreePaths, and optionally a set of integers. The mapping
	''' from TreePath to integer is done by way of an instance of RowMapper.
	''' It is not necessary for a TreeSelectionModel to have a RowMapper to
	''' correctly operate, but without a RowMapper <code>getSelectionRows</code>
	''' will return null.
	'''  
	''' <p>
	'''  
	''' A TreeSelectionModel can be configured to allow only one
	''' path (<code>SINGLE_TREE_SELECTION</code>) a number of
	''' contiguous paths (<code>CONTIGUOUS_TREE_SELECTION</code>) or a number of
	''' discontiguous paths (<code>DISCONTIGUOUS_TREE_SELECTION</code>).
	''' A <code>RowMapper</code> is used to determine if TreePaths are
	''' contiguous.
	''' In the absence of a RowMapper <code>CONTIGUOUS_TREE_SELECTION</code> and
	''' <code>DISCONTIGUOUS_TREE_SELECTION</code> behave the same, that is they
	''' allow any number of paths to be contained in the TreeSelectionModel.
	'''  
	''' <p>
	'''  
	''' For a selection model of <code>CONTIGUOUS_TREE_SELECTION</code> any
	''' time the paths are changed (<code>setSelectionPath</code>,
	''' <code>addSelectionPath</code> ...) the TreePaths are again checked to
	''' make they are contiguous. A check of the TreePaths can also be forced
	''' by invoking <code>resetRowSelection</code>. How a set of discontiguous
	''' TreePaths is mapped to a contiguous set is left to implementors of
	''' this interface to enforce a particular policy.
	'''  
	''' <p>
	'''  
	''' Implementations should combine duplicate TreePaths that are
	''' added to the selection. For example, the following code
	''' <pre>
	'''   TreePath[] paths = new TreePath[] { treePath, treePath };
	'''   treeSelectionModel.setSelectionPaths(paths);
	''' </pre>
	''' should result in only one path being selected:
	''' <code>treePath</code>, and
	''' not two copies of <code>treePath</code>.
	'''  
	''' <p>
	'''  
	''' The lead TreePath is the last path that was added (or set). The lead
	''' row is then the row that corresponds to the TreePath as determined
	''' from the RowMapper.
	'''  
	''' @author Scott Violet
	''' </summary>

	Public Interface TreeSelectionModel
		''' <summary>
		''' Selection can only contain one path at a time. </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int SINGLE_TREE_SELECTION = 1;

		''' <summary>
		''' Selection can only be contiguous. This will only be enforced if
		''' a RowMapper instance is provided. That is, if no RowMapper is set
		''' this behaves the same as DISCONTIGUOUS_TREE_SELECTION. 
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int CONTIGUOUS_TREE_SELECTION = 2;

		''' <summary>
		''' Selection can contain any number of items that are not necessarily
		''' contiguous. 
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int DISCONTIGUOUS_TREE_SELECTION = 4;

		''' <summary>
		''' Sets the selection model, which must be one of SINGLE_TREE_SELECTION,
		''' CONTIGUOUS_TREE_SELECTION or DISCONTIGUOUS_TREE_SELECTION.
		''' <p>
		''' This may change the selection if the current selection is not valid
		''' for the new mode. For example, if three TreePaths are
		''' selected when the mode is changed to <code>SINGLE_TREE_SELECTION</code>,
		''' only one TreePath will remain selected. It is up to the particular
		''' implementation to decide what TreePath remains selected.
		''' </summary>
		Property selectionMode As Integer


		''' <summary>
		''' Sets the selection to path. If this represents a change, then
		''' the TreeSelectionListeners are notified. If <code>path</code> is
		''' null, this has the same effect as invoking <code>clearSelection</code>.
		''' </summary>
		''' <param name="path"> new path to select </param>
		Property selectionPath As TreePath

		''' <summary>
		''' Sets the selection to path. If this represents a change, then
		''' the TreeSelectionListeners are notified. If <code>paths</code> is
		''' null, this has the same effect as invoking <code>clearSelection</code>.
		''' </summary>
		''' <param name="paths"> new selection </param>
		Property selectionPaths As TreePath()

		''' <summary>
		''' Adds path to the current selection. If path is not currently
		''' in the selection the TreeSelectionListeners are notified. This has
		''' no effect if <code>path</code> is null.
		''' </summary>
		''' <param name="path"> the new path to add to the current selection </param>
		Sub addSelectionPath(ByVal path As TreePath)

		''' <summary>
		''' Adds paths to the current selection.  If any of the paths in
		''' paths are not currently in the selection the TreeSelectionListeners
		''' are notified. This has
		''' no effect if <code>paths</code> is null.
		''' </summary>
		''' <param name="paths"> the new paths to add to the current selection </param>
		Sub addSelectionPaths(ByVal paths As TreePath())

		''' <summary>
		''' Removes path from the selection. If path is in the selection
		''' The TreeSelectionListeners are notified. This has no effect if
		''' <code>path</code> is null.
		''' </summary>
		''' <param name="path"> the path to remove from the selection </param>
		Sub removeSelectionPath(ByVal path As TreePath)

		''' <summary>
		''' Removes paths from the selection.  If any of the paths in
		''' <code>paths</code>
		''' are in the selection, the TreeSelectionListeners are notified.
		''' This method has no effect if <code>paths</code> is null.
		''' </summary>
		''' <param name="paths"> the path to remove from the selection </param>
		Sub removeSelectionPaths(ByVal paths As TreePath())



		''' <summary>
		''' Returns the number of paths that are selected.
		''' </summary>
		ReadOnly Property selectionCount As Integer

		''' <summary>
		''' Returns true if the path, <code>path</code>, is in the current
		''' selection.
		''' </summary>
		Function isPathSelected(ByVal path As TreePath) As Boolean

		''' <summary>
		''' Returns true if the selection is currently empty.
		''' </summary>
		ReadOnly Property selectionEmpty As Boolean

		''' <summary>
		''' Empties the current selection.  If this represents a change in the
		''' current selection, the selection listeners are notified.
		''' </summary>
		Sub clearSelection()

		''' <summary>
		''' Sets the RowMapper instance. This instance is used to determine
		''' the row for a particular TreePath.
		''' </summary>
		Property rowMapper As RowMapper


		''' <summary>
		''' Returns all of the currently selected rows. This will return
		''' null (or an empty array) if there are no selected TreePaths or
		''' a RowMapper has not been set.
		''' </summary>
		ReadOnly Property selectionRows As Integer()

		''' <summary>
		''' Returns the smallest value obtained from the RowMapper for the
		''' current set of selected TreePaths. If nothing is selected,
		''' or there is no RowMapper, this will return -1.
		''' </summary>
		ReadOnly Property minSelectionRow As Integer

		''' <summary>
		''' Returns the largest value obtained from the RowMapper for the
		''' current set of selected TreePaths. If nothing is selected,
		''' or there is no RowMapper, this will return -1.
		''' </summary>
		ReadOnly Property maxSelectionRow As Integer

		''' <summary>
		''' Returns true if the row identified by <code>row</code> is selected.
		''' </summary>
		Function isRowSelected(ByVal row As Integer) As Boolean

		''' <summary>
		''' Updates this object's mapping from TreePaths to rows. This should
		''' be invoked when the mapping from TreePaths to integers has changed
		''' (for example, a node has been expanded).
		''' <p>
		''' You do not normally have to call this; JTree and its associated
		''' listeners will invoke this for you. If you are implementing your own
		''' view class, then you will have to invoke this.
		''' </summary>
		Sub resetRowSelection()

		''' <summary>
		''' Returns the lead selection index. That is the last index that was
		''' added.
		''' </summary>
		ReadOnly Property leadSelectionRow As Integer

		''' <summary>
		''' Returns the last path that was added. This may differ from the
		''' leadSelectionPath property maintained by the JTree.
		''' </summary>
		ReadOnly Property leadSelectionPath As TreePath

		''' <summary>
		''' Adds a PropertyChangeListener to the listener list.
		''' The listener is registered for all properties.
		''' <p>
		''' A PropertyChangeEvent will get fired when the selection mode
		''' changes.
		''' </summary>
		''' <param name="listener">  the PropertyChangeListener to be added </param>
		Sub addPropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)

		''' <summary>
		''' Removes a PropertyChangeListener from the listener list.
		''' This removes a PropertyChangeListener that was registered
		''' for all properties.
		''' </summary>
		''' <param name="listener">  the PropertyChangeListener to be removed </param>
		Sub removePropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)

		''' <summary>
		''' Adds x to the list of listeners that are notified each time the
		''' set of selected TreePaths changes.
		''' </summary>
		''' <param name="x"> the new listener to be added </param>
		Sub addTreeSelectionListener(ByVal x As TreeSelectionListener)

		''' <summary>
		''' Removes x from the list of listeners that are notified each time
		''' the set of selected TreePaths changes.
		''' </summary>
		''' <param name="x"> the listener to remove </param>
		Sub removeTreeSelectionListener(ByVal x As TreeSelectionListener)
	End Interface

End Namespace
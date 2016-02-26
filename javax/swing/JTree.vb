Imports System
Imports System.Diagnostics
Imports System.Collections
Imports System.Collections.Generic
Imports javax.swing.event
Imports javax.swing.plaf
Imports javax.swing.tree
Imports javax.accessibility
Imports sun.swing.SwingUtilities2.Section

'
' * Copyright (c) 1997, 2015, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing


	''' <summary>
	''' <a name="jtree_description"></a>
	''' A control that displays a set of hierarchical data as an outline.
	''' You can find task-oriented documentation and examples of using trees in
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/tree.html">How to Use Trees</a>,
	''' a section in <em>The Java Tutorial.</em>
	''' <p>
	''' A specific node in a tree can be identified either by a
	''' <code>TreePath</code> (an object
	''' that encapsulates a node and all of its ancestors), or by its
	''' display row, where each row in the display area displays one node.
	''' An <i>expanded</i> node is a non-leaf node (as identified by
	''' <code>TreeModel.isLeaf(node)</code> returning false) that will displays
	''' its children when all its ancestors are <i>expanded</i>.
	''' A <i>collapsed</i>
	''' node is one which hides them. A <i>hidden</i> node is one which is
	''' under a collapsed ancestor. All of a <i>viewable</i> nodes parents
	''' are expanded, but may or may not be displayed. A <i>displayed</i> node
	''' is both viewable and in the display area, where it can be seen.
	''' </p>
	''' The following <code>JTree</code> methods use "visible" to mean "displayed":
	''' <ul>
	''' <li><code>isRootVisible()</code>
	''' <li><code>setRootVisible()</code>
	''' <li><code>scrollPathToVisible()</code>
	''' <li><code>scrollRowToVisible()</code>
	''' <li><code>getVisibleRowCount()</code>
	''' <li><code>setVisibleRowCount()</code>
	''' </ul>
	''' The next group of <code>JTree</code> methods use "visible" to mean
	''' "viewable" (under an expanded parent):
	''' <ul>
	''' <li><code>isVisible()</code>
	''' <li><code>makeVisible()</code>
	''' </ul>
	''' If you are interested in knowing when the selection changes implement
	''' the <code>TreeSelectionListener</code> interface and add the instance
	''' using the method <code>addTreeSelectionListener</code>.
	''' <code>valueChanged</code> will be invoked when the
	''' selection changes, that is if the user clicks twice on the same
	''' node <code>valueChanged</code> will only be invoked once.
	''' <p>
	''' If you are interested in detecting either double-click events or when
	''' a user clicks on a node, regardless of whether or not it was selected,
	''' we recommend you do the following:
	''' </p>
	''' <pre>
	''' final JTree tree = ...;
	''' 
	''' MouseListener ml = new MouseAdapter() {
	'''     public void <b>mousePressed</b>(MouseEvent e) {
	'''         int selRow = tree.getRowForLocation(e.getX(), e.getY());
	'''         TreePath selPath = tree.getPathForLocation(e.getX(), e.getY());
	'''         if(selRow != -1) {
	'''             if(e.getClickCount() == 1) {
	'''                 mySingleClick(selRow, selPath);
	'''             }
	'''             else if(e.getClickCount() == 2) {
	'''                 myDoubleClick(selRow, selPath);
	'''             }
	'''         }
	'''     }
	''' };
	''' tree.addMouseListener(ml);
	''' </pre>
	''' NOTE: This example obtains both the path and row, but you only need to
	''' get the one you're interested in.
	''' <p>
	''' To use <code>JTree</code> to display compound nodes
	''' (for example, nodes containing both
	''' a graphic icon and text), subclass <seealso cref="TreeCellRenderer"/> and use
	''' <seealso cref="#setCellRenderer"/> to tell the tree to use it. To edit such nodes,
	''' subclass <seealso cref="TreeCellEditor"/> and use <seealso cref="#setCellEditor"/>.
	''' </p>
	''' <p>
	''' Like all <code>JComponent</code> classes, you can use <seealso cref="InputMap"/> and
	''' <seealso cref="ActionMap"/>
	''' to associate an <seealso cref="Action"/> object with a <seealso cref="KeyStroke"/>
	''' and execute the action under specified conditions.
	''' </p>
	''' <strong>Warning:</strong> Swing is not thread safe. For more
	''' information see <a
	''' href="package-summary.html#threading">Swing's Threading
	''' Policy</a>.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' </p>
	''' @beaninfo
	'''   attribute: isContainer false
	''' description: A component that displays a set of hierarchical data as an outline.
	''' 
	''' @author Rob Davis
	''' @author Ray Ryan
	''' @author Scott Violet
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class JTree
		Inherits JComponent
		Implements Scrollable, Accessible

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "TreeUI"

		''' <summary>
		''' The model that defines the tree displayed by this object.
		''' </summary>
		<NonSerialized> _
		Protected Friend treeModel As TreeModel

		''' <summary>
		''' Models the set of selected nodes in this tree.
		''' </summary>
		<NonSerialized> _
		Protected Friend selectionModel As TreeSelectionModel

		''' <summary>
		''' True if the root node is displayed, false if its children are
		''' the highest visible nodes.
		''' </summary>
		Protected Friend rootVisible As Boolean

		''' <summary>
		''' The cell used to draw nodes. If <code>null</code>, the UI uses a default
		''' <code>cellRenderer</code>.
		''' </summary>
		<NonSerialized> _
		Protected Friend cellRenderer As TreeCellRenderer

		''' <summary>
		''' Height to use for each display row. If this is &lt;= 0 the renderer
		''' determines the height for each row.
		''' </summary>
		Protected Friend rowHeight As Integer
		Private rowHeightSet As Boolean = False

		''' <summary>
		''' Maps from <code>TreePath</code> to <code>Boolean</code>
		''' indicating whether or not the
		''' particular path is expanded. This ONLY indicates whether a
		''' given path is expanded, and NOT if it is visible or not. That
		''' information must be determined by visiting all the parent
		''' paths and seeing if they are visible.
		''' </summary>
		<NonSerialized> _
		Private expandedState As Dictionary(Of TreePath, Boolean?)


		''' <summary>
		''' True if handles are displayed at the topmost level of the tree.
		''' <p>
		''' A handle is a small icon that displays adjacent to the node which
		''' allows the user to click once to expand or collapse the node. A
		''' common interface shows a plus sign (+) for a node which can be
		''' expanded and a minus sign (-) for a node which can be collapsed.
		''' Handles are always shown for nodes below the topmost level.
		''' <p>
		''' If the <code>rootVisible</code> setting specifies that the root
		''' node is to be displayed, then that is the only node at the topmost
		''' level. If the root node is not displayed, then all of its
		''' children are at the topmost level of the tree. Handles are
		''' always displayed for nodes other than the topmost.
		''' <p>
		''' If the root node isn't visible, it is generally a good to make
		''' this value true. Otherwise, the tree looks exactly like a list,
		''' and users may not know that the "list entries" are actually
		''' tree nodes.
		''' </summary>
		''' <seealso cref= #rootVisible </seealso>
		Protected Friend showsRootHandles As Boolean
		Private showsRootHandlesSet As Boolean = False

		''' <summary>
		''' Creates a new event and passed it off the
		''' <code>selectionListeners</code>.
		''' </summary>
		<NonSerialized> _
		Protected Friend selectionRedirector As TreeSelectionRedirector

		''' <summary>
		''' Editor for the entries.  Default is <code>null</code>
		''' (tree is not editable).
		''' </summary>
		<NonSerialized> _
		Protected Friend cellEditor As TreeCellEditor

		''' <summary>
		''' Is the tree editable? Default is false.
		''' </summary>
		Protected Friend editable As Boolean

		''' <summary>
		''' Is this tree a large model? This is a code-optimization setting.
		''' A large model can be used when the cell height is the same for all
		''' nodes. The UI will then cache very little information and instead
		''' continually message the model. Without a large model the UI caches
		''' most of the information, resulting in fewer method calls to the model.
		''' <p>
		''' This value is only a suggestion to the UI. Not all UIs will
		''' take advantage of it. Default value is false.
		''' </summary>
		Protected Friend largeModel As Boolean

		''' <summary>
		''' Number of rows to make visible at one time. This value is used for
		''' the <code>Scrollable</code> interface. It determines the preferred
		''' size of the display area.
		''' </summary>
		Protected Friend visibleRowCount As Integer

		''' <summary>
		''' If true, when editing is to be stopped by way of selection changing,
		''' data in tree changing or other means <code>stopCellEditing</code>
		''' is invoked, and changes are saved. If false,
		''' <code>cancelCellEditing</code> is invoked, and changes
		''' are discarded. Default is false.
		''' </summary>
		Protected Friend invokesStopCellEditing As Boolean

		''' <summary>
		''' If true, when a node is expanded, as many of the descendants are
		''' scrolled to be visible.
		''' </summary>
		Protected Friend scrollsOnExpand As Boolean
		Private scrollsOnExpandSet As Boolean = False

		''' <summary>
		''' Number of mouse clicks before a node is expanded.
		''' </summary>
		Protected Friend toggleClickCount As Integer

		''' <summary>
		''' Updates the <code>expandedState</code>.
		''' </summary>
		<NonSerialized> _
		Protected Friend treeModelListener As TreeModelListener

		''' <summary>
		''' Used when <code>setExpandedState</code> is invoked,
		''' will be a <code>Stack</code> of <code>Stack</code>s.
		''' </summary>
		<NonSerialized> _
		Private expandedStack As Stack(Of Stack(Of TreePath))

		''' <summary>
		''' Lead selection path, may not be <code>null</code>.
		''' </summary>
		Private leadPath As TreePath

		''' <summary>
		''' Anchor path.
		''' </summary>
		Private anchorPath As TreePath

		''' <summary>
		''' True if paths in the selection should be expanded.
		''' </summary>
		Private expandsSelectedPaths As Boolean

		''' <summary>
		''' This is set to true for the life of the <code>setUI</code> call.
		''' </summary>
		Private settingUI As Boolean

		''' <summary>
		''' If true, mouse presses on selections initiate a drag operation. </summary>
		Private dragEnabled As Boolean

		''' <summary>
		''' The drop mode for this component.
		''' </summary>
		Private dropMode As DropMode = DropMode.USE_SELECTION

		''' <summary>
		''' The drop location.
		''' </summary>
		<NonSerialized> _
		Private dropLocation As DropLocation

		''' <summary>
		''' A subclass of <code>TransferHandler.DropLocation</code> representing
		''' a drop location for a <code>JTree</code>.
		''' </summary>
		''' <seealso cref= #getDropLocation
		''' @since 1.6 </seealso>
		Public NotInheritable Class DropLocation
			Inherits TransferHandler.DropLocation

			Private ReadOnly path As TreePath
			Private ReadOnly index As Integer

			Private Sub New(ByVal p As Point, ByVal path As TreePath, ByVal index As Integer)
				MyBase.New(p)
				Me.path = path
				Me.index = index
			End Sub

			''' <summary>
			''' Returns the index where the dropped data should be inserted
			''' with respect to the path returned by <code>getPath()</code>.
			''' <p>
			''' For drop modes <code>DropMode.USE_SELECTION</code> and
			''' <code>DropMode.ON</code>, this index is unimportant (and it will
			''' always be <code>-1</code>) as the only interesting data is the
			''' path over which the drop operation occurred.
			''' <p>
			''' For drop mode <code>DropMode.INSERT</code>, this index
			''' indicates the index at which the data should be inserted into
			''' the parent path represented by <code>getPath()</code>.
			''' <code>-1</code> indicates that the drop occurred over the
			''' parent itself, and in most cases should be treated as inserting
			''' into either the beginning or the end of the parent's list of
			''' children.
			''' <p>
			''' For <code>DropMode.ON_OR_INSERT</code>, this value will be
			''' an insert index, as described above, or <code>-1</code> if
			''' the drop occurred over the path itself.
			''' </summary>
			''' <returns> the child index </returns>
			''' <seealso cref= #getPath </seealso>
			Public Property childIndex As Integer
				Get
					Return index
				End Get
			End Property

			''' <summary>
			''' Returns the path where dropped data should be placed in the
			''' tree.
			''' <p>
			''' Interpretation of this value depends on the drop mode set on the
			''' component. If the drop mode is <code>DropMode.USE_SELECTION</code>
			''' or <code>DropMode.ON</code>, the return value is the path in the
			''' tree over which the data has been (or will be) dropped.
			''' <code>null</code> indicates that the drop is over empty space,
			''' not associated with a particular path.
			''' <p>
			''' If the drop mode is <code>DropMode.INSERT</code>, the return value
			''' refers to the path that should become the parent of the new data,
			''' in which case <code>getChildIndex()</code> indicates where the
			''' new item should be inserted into this parent path. A
			''' <code>null</code> path indicates that no parent path has been
			''' determined, which can happen for multiple reasons:
			''' <ul>
			'''    <li>The tree has no model
			'''    <li>There is no root in the tree
			'''    <li>The root is collapsed
			'''    <li>The root is a leaf node
			''' </ul>
			''' It is up to the developer to decide if and how they wish to handle
			''' the <code>null</code> case.
			''' <p>
			''' If the drop mode is <code>DropMode.ON_OR_INSERT</code>,
			''' <code>getChildIndex</code> can be used to determine whether the
			''' drop is on top of the path itself (<code>-1</code>) or the index
			''' at which it should be inserted into the path (values other than
			''' <code>-1</code>).
			''' </summary>
			''' <returns> the drop path </returns>
			''' <seealso cref= #getChildIndex </seealso>
			Public Property path As TreePath
				Get
					Return path
				End Get
			End Property

			''' <summary>
			''' Returns a string representation of this drop location.
			''' This method is intended to be used for debugging purposes,
			''' and the content and format of the returned string may vary
			''' between implementations.
			''' </summary>
			''' <returns> a string representation of this drop location </returns>
			Public Overrides Function ToString() As String
				Return Me.GetType().name & "[dropPoint=" & dropPoint & "," & "path=" & path & "," & "childIndex=" & index & "]"
			End Function
		End Class

		''' <summary>
		''' The row to expand during DnD.
		''' </summary>
		Private ___expandRow As Integer = -1

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Class TreeTimer
			Inherits Timer

			Private ReadOnly outerInstance As JTree

			Public Sub New(ByVal outerInstance As JTree)
					Me.outerInstance = outerInstance
				MyBase.New(2000, Nothing)
				repeats = False
			End Sub

			Public Overrides Sub fireActionPerformed(ByVal ae As ActionEvent)
				outerInstance.expandRow(outerInstance.___expandRow)
			End Sub
		End Class

		''' <summary>
		''' A timer to expand nodes during drop.
		''' </summary>
		Private dropTimer As TreeTimer

		''' <summary>
		''' When <code>addTreeExpansionListener</code> is invoked,
		''' and <code>settingUI</code> is true, this ivar gets set to the passed in
		''' <code>Listener</code>. This listener is then notified first in
		''' <code>fireTreeCollapsed</code> and <code>fireTreeExpanded</code>.
		''' <p>This is an ugly workaround for a way to have the UI listener
		''' get notified before other listeners.
		''' </summary>
		<NonSerialized> _
		Private uiTreeExpansionListener As TreeExpansionListener

		''' <summary>
		''' Max number of stacks to keep around.
		''' </summary>
		Private Shared TEMP_STACK_SIZE As Integer = 11

		'
		' Bound property names
		'
		''' <summary>
		''' Bound property name for <code>cellRenderer</code>. </summary>
		Public Const CELL_RENDERER_PROPERTY As String = "cellRenderer"
		''' <summary>
		''' Bound property name for <code>treeModel</code>. </summary>
		Public Const TREE_MODEL_PROPERTY As String = "model"
		''' <summary>
		''' Bound property name for <code>rootVisible</code>. </summary>
		Public Const ROOT_VISIBLE_PROPERTY As String = "rootVisible"
		''' <summary>
		''' Bound property name for <code>showsRootHandles</code>. </summary>
		Public Const SHOWS_ROOT_HANDLES_PROPERTY As String = "showsRootHandles"
		''' <summary>
		''' Bound property name for <code>rowHeight</code>. </summary>
		Public Const ROW_HEIGHT_PROPERTY As String = "rowHeight"
		''' <summary>
		''' Bound property name for <code>cellEditor</code>. </summary>
		Public Const CELL_EDITOR_PROPERTY As String = "cellEditor"
		''' <summary>
		''' Bound property name for <code>editable</code>. </summary>
		Public Const EDITABLE_PROPERTY As String = "editable"
		''' <summary>
		''' Bound property name for <code>largeModel</code>. </summary>
		Public Const LARGE_MODEL_PROPERTY As String = "largeModel"
		''' <summary>
		''' Bound property name for selectionModel. </summary>
		Public Const SELECTION_MODEL_PROPERTY As String = "selectionModel"
		''' <summary>
		''' Bound property name for <code>visibleRowCount</code>. </summary>
		Public Const VISIBLE_ROW_COUNT_PROPERTY As String = "visibleRowCount"
		''' <summary>
		''' Bound property name for <code>messagesStopCellEditing</code>. </summary>
		Public Const INVOKES_STOP_CELL_EDITING_PROPERTY As String = "invokesStopCellEditing"
		''' <summary>
		''' Bound property name for <code>scrollsOnExpand</code>. </summary>
		Public Const SCROLLS_ON_EXPAND_PROPERTY As String = "scrollsOnExpand"
		''' <summary>
		''' Bound property name for <code>toggleClickCount</code>. </summary>
		Public Const TOGGLE_CLICK_COUNT_PROPERTY As String = "toggleClickCount"
		''' <summary>
		''' Bound property name for <code>leadSelectionPath</code>.
		''' @since 1.3 
		''' </summary>
		Public Const LEAD_SELECTION_PATH_PROPERTY As String = "leadSelectionPath"
		''' <summary>
		''' Bound property name for anchor selection path.
		''' @since 1.3 
		''' </summary>
		Public Const ANCHOR_SELECTION_PATH_PROPERTY As String = "anchorSelectionPath"
		''' <summary>
		''' Bound property name for expands selected paths property
		''' @since 1.3 
		''' </summary>
		Public Const EXPANDS_SELECTED_PATHS_PROPERTY As String = "expandsSelectedPaths"


		''' <summary>
		''' Creates and returns a sample <code>TreeModel</code>.
		''' Used primarily for beanbuilders to show something interesting.
		''' </summary>
		''' <returns> the default <code>TreeModel</code> </returns>
		Protected Friend Property Shared defaultTreeModel As TreeModel
			Get
				Dim root As New DefaultMutableTreeNode("JTree")
				Dim parent As DefaultMutableTreeNode
    
				parent = New DefaultMutableTreeNode("colors")
				root.add(parent)
				parent.add(New DefaultMutableTreeNode("blue"))
				parent.add(New DefaultMutableTreeNode("violet"))
				parent.add(New DefaultMutableTreeNode("red"))
				parent.add(New DefaultMutableTreeNode("yellow"))
    
				parent = New DefaultMutableTreeNode("sports")
				root.add(parent)
				parent.add(New DefaultMutableTreeNode("basketball"))
				parent.add(New DefaultMutableTreeNode("soccer"))
				parent.add(New DefaultMutableTreeNode("football"))
				parent.add(New DefaultMutableTreeNode("hockey"))
    
				parent = New DefaultMutableTreeNode("food")
				root.add(parent)
				parent.add(New DefaultMutableTreeNode("hot dogs"))
				parent.add(New DefaultMutableTreeNode("pizza"))
				parent.add(New DefaultMutableTreeNode("ravioli"))
				parent.add(New DefaultMutableTreeNode("bananas"))
				Return New DefaultTreeModel(root)
			End Get
		End Property

		''' <summary>
		''' Returns a <code>TreeModel</code> wrapping the specified object.
		''' If the object is:<ul>
		''' <li>an array of <code>Object</code>s,
		''' <li>a <code>Hashtable</code>, or
		''' <li>a <code>Vector</code>
		''' </ul>then a new root node is created with each of the incoming
		''' objects as children. Otherwise, a new root is created with
		''' a value of {@code "root"}.
		''' </summary>
		''' <param name="value">  the <code>Object</code> used as the foundation for
		'''          the <code>TreeModel</code> </param>
		''' <returns> a <code>TreeModel</code> wrapping the specified object </returns>
		Protected Friend Shared Function createTreeModel(ByVal value As Object) As TreeModel
			Dim root As DefaultMutableTreeNode

			If (TypeOf value Is Object()) OrElse (TypeOf value Is Hashtable) OrElse (TypeOf value Is ArrayList) Then
				root = New DefaultMutableTreeNode("root")
				DynamicUtilTreeNode.createChildren(root, value)
			Else
				root = New DynamicUtilTreeNode("root", value)
			End If
			Return New DefaultTreeModel(root, False)
		End Function

		''' <summary>
		''' Returns a <code>JTree</code> with a sample model.
		''' The default model used by the tree defines a leaf node as any node
		''' without children.
		''' </summary>
		''' <seealso cref= DefaultTreeModel#asksAllowsChildren </seealso>
		Public Sub New()
			Me.New(defaultTreeModel)
		End Sub

		''' <summary>
		''' Returns a <code>JTree</code> with each element of the
		''' specified array as the
		''' child of a new root node which is not displayed.
		''' By default, the tree defines a leaf node as any node without
		''' children.
		''' </summary>
		''' <param name="value">  an array of <code>Object</code>s </param>
		''' <seealso cref= DefaultTreeModel#asksAllowsChildren </seealso>
		Public Sub New(ByVal value As Object())
			Me.New(createTreeModel(value))
			Me.rootVisible = False
			Me.showsRootHandles = True
			expandRoot()
		End Sub

		''' <summary>
		''' Returns a <code>JTree</code> with each element of the specified
		''' <code>Vector</code> as the
		''' child of a new root node which is not displayed. By default, the
		''' tree defines a leaf node as any node without children.
		''' </summary>
		''' <param name="value">  a <code>Vector</code> </param>
		''' <seealso cref= DefaultTreeModel#asksAllowsChildren </seealso>
		Public Sub New(Of T1)(ByVal value As List(Of T1))
			Me.New(createTreeModel(value))
			Me.rootVisible = False
			Me.showsRootHandles = True
			expandRoot()
		End Sub

		''' <summary>
		''' Returns a <code>JTree</code> created from a <code>Hashtable</code>
		''' which does not display with root.
		''' Each value-half of the key/value pairs in the <code>HashTable</code>
		''' becomes a child of the new root node. By default, the tree defines
		''' a leaf node as any node without children.
		''' </summary>
		''' <param name="value">  a <code>Hashtable</code> </param>
		''' <seealso cref= DefaultTreeModel#asksAllowsChildren </seealso>
		Public Sub New(Of T1)(ByVal value As Dictionary(Of T1))
			Me.New(createTreeModel(value))
			Me.rootVisible = False
			Me.showsRootHandles = True
			expandRoot()
		End Sub

		''' <summary>
		''' Returns a <code>JTree</code> with the specified
		''' <code>TreeNode</code> as its root,
		''' which displays the root node.
		''' By default, the tree defines a leaf node as any node without children.
		''' </summary>
		''' <param name="root">  a <code>TreeNode</code> object </param>
		''' <seealso cref= DefaultTreeModel#asksAllowsChildren </seealso>
		Public Sub New(ByVal root As TreeNode)
			Me.New(root, False)
		End Sub

		''' <summary>
		''' Returns a <code>JTree</code> with the specified <code>TreeNode</code>
		''' as its root, which
		''' displays the root node and which decides whether a node is a
		''' leaf node in the specified manner.
		''' </summary>
		''' <param name="root">  a <code>TreeNode</code> object </param>
		''' <param name="asksAllowsChildren">  if false, any node without children is a
		'''              leaf node; if true, only nodes that do not allow
		'''              children are leaf nodes </param>
		''' <seealso cref= DefaultTreeModel#asksAllowsChildren </seealso>
		Public Sub New(ByVal root As TreeNode, ByVal asksAllowsChildren As Boolean)
			Me.New(New DefaultTreeModel(root, asksAllowsChildren))
		End Sub

		''' <summary>
		''' Returns an instance of <code>JTree</code> which displays the root node
		''' -- the tree is created using the specified data model.
		''' </summary>
		''' <param name="newModel">  the <code>TreeModel</code> to use as the data model </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal newModel As TreeModel)
			MyBase.New()
			expandedStack = New Stack(Of Stack(Of TreePath))
			toggleClickCount = 2
			expandedState = New Dictionary(Of TreePath, Boolean?)
			layout = Nothing
			rowHeight = 16
			visibleRowCount = 20
			rootVisible = True
			selectionModel = New DefaultTreeSelectionModel
			cellRenderer = Nothing
			scrollsOnExpand = True
			opaque = True
			expandsSelectedPaths = True
			updateUI()
			model = newModel
		End Sub

		''' <summary>
		''' Returns the L&amp;F object that renders this component.
		''' </summary>
		''' <returns> the <code>TreeUI</code> object that renders this component </returns>
		Public Overridable Property uI As TreeUI
			Get
				Return CType(ui, TreeUI)
			End Get
			Set(ByVal ui As TreeUI)
				If Me.ui IsNot ui Then
					settingUI = True
					uiTreeExpansionListener = Nothing
					Try
						MyBase.uI = ui
					Finally
						settingUI = False
					End Try
				End If
			End Set
		End Property


		''' <summary>
		''' Notification from the <code>UIManager</code> that the L&amp;F has changed.
		''' Replaces the current UI object with the latest version from the
		''' <code>UIManager</code>.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), TreeUI)

			SwingUtilities.updateRendererOrEditorUI(cellRenderer)
			SwingUtilities.updateRendererOrEditorUI(cellEditor)
		End Sub


		''' <summary>
		''' Returns the name of the L&amp;F class that renders this component.
		''' </summary>
		''' <returns> the string "TreeUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property


		''' <summary>
		''' Returns the current <code>TreeCellRenderer</code>
		'''  that is rendering each cell.
		''' </summary>
		''' <returns> the <code>TreeCellRenderer</code> that is rendering each cell </returns>
		Public Overridable Property cellRenderer As TreeCellRenderer
			Get
				Return cellRenderer
			End Get
			Set(ByVal x As TreeCellRenderer)
				Dim oldValue As TreeCellRenderer = cellRenderer
    
				cellRenderer = x
				firePropertyChange(CELL_RENDERER_PROPERTY, oldValue, cellRenderer)
				invalidate()
			End Set
		End Property


		''' <summary>
		''' Determines whether the tree is editable. Fires a property
		''' change event if the new setting is different from the existing
		''' setting.
		''' <p>
		''' This is a bound property.
		''' </summary>
		''' <param name="flag">  a boolean value, true if the tree is editable
		''' @beaninfo
		'''        bound: true
		'''  description: Whether the tree is editable. </param>
		Public Overridable Property editable As Boolean
			Set(ByVal flag As Boolean)
				Dim oldValue As Boolean = Me.editable
    
				Me.editable = flag
				firePropertyChange(EDITABLE_PROPERTY, oldValue, flag)
				If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, (If(oldValue, AccessibleState.EDITABLE, Nothing)), (If(flag, AccessibleState.EDITABLE, Nothing)))
			End Set
			Get
				Return editable
			End Get
		End Property


		''' <summary>
		''' Sets the cell editor.  A <code>null</code> value implies that the
		''' tree cannot be edited.  If this represents a change in the
		''' <code>cellEditor</code>, the <code>propertyChange</code>
		''' method is invoked on all listeners.
		''' <p>
		''' This is a bound property.
		''' </summary>
		''' <param name="cellEditor"> the <code>TreeCellEditor</code> to use
		''' @beaninfo
		'''        bound: true
		'''  description: The cell editor. A null value implies the tree
		'''               cannot be edited. </param>
		Public Overridable Property cellEditor As TreeCellEditor
			Set(ByVal cellEditor As TreeCellEditor)
				Dim oldEditor As TreeCellEditor = Me.cellEditor
    
				Me.cellEditor = cellEditor
				firePropertyChange(CELL_EDITOR_PROPERTY, oldEditor, cellEditor)
				invalidate()
			End Set
			Get
				Return cellEditor
			End Get
		End Property


		''' <summary>
		''' Returns the <code>TreeModel</code> that is providing the data.
		''' </summary>
		''' <returns> the <code>TreeModel</code> that is providing the data </returns>
		Public Overridable Property model As TreeModel
			Get
				Return treeModel
			End Get
			Set(ByVal newModel As TreeModel)
				clearSelection()
    
				Dim oldModel As TreeModel = treeModel
    
				If treeModel IsNot Nothing AndAlso treeModelListener IsNot Nothing Then treeModel.removeTreeModelListener(treeModelListener)
    
				If accessibleContext IsNot Nothing Then
					If treeModel IsNot Nothing Then treeModel.removeTreeModelListener(CType(accessibleContext, TreeModelListener))
					If newModel IsNot Nothing Then newModel.addTreeModelListener(CType(accessibleContext, TreeModelListener))
				End If
    
				treeModel = newModel
				clearToggledPaths()
				If treeModel IsNot Nothing Then
					If treeModelListener Is Nothing Then treeModelListener = createTreeModelListener()
					If treeModelListener IsNot Nothing Then treeModel.addTreeModelListener(treeModelListener)
					' Mark the root as expanded, if it isn't a leaf.
					Dim treeRoot As Object = treeModel.root
					If treeRoot IsNot Nothing AndAlso (Not treeModel.isLeaf(treeRoot)) Then expandedState(New TreePath(treeRoot)) = Boolean.TRUE
				End If
				firePropertyChange(TREE_MODEL_PROPERTY, oldModel, treeModel)
				invalidate()
			End Set
		End Property


		''' <summary>
		''' Returns true if the root node of the tree is displayed.
		''' </summary>
		''' <returns> true if the root node of the tree is displayed </returns>
		''' <seealso cref= #rootVisible </seealso>
		Public Overridable Property rootVisible As Boolean
			Get
				Return rootVisible
			End Get
			Set(ByVal rootVisible As Boolean)
				Dim oldValue As Boolean = Me.rootVisible
    
				Me.rootVisible = rootVisible
				firePropertyChange(ROOT_VISIBLE_PROPERTY, oldValue, Me.rootVisible)
				If accessibleContext IsNot Nothing Then CType(accessibleContext, AccessibleJTree).fireVisibleDataPropertyChange()
			End Set
		End Property


		''' <summary>
		''' Sets the value of the <code>showsRootHandles</code> property,
		''' which specifies whether the node handles should be displayed.
		''' The default value of this property depends on the constructor
		''' used to create the <code>JTree</code>.
		''' Some look and feels might not support handles;
		''' they will ignore this property.
		''' <p>
		''' This is a bound property.
		''' </summary>
		''' <param name="newValue"> <code>true</code> if root handles should be displayed;
		'''                 otherwise, <code>false</code> </param>
		''' <seealso cref= #showsRootHandles </seealso>
		''' <seealso cref= #getShowsRootHandles
		''' @beaninfo
		'''        bound: true
		'''  description: Whether the node handles are to be
		'''               displayed. </seealso>
		Public Overridable Property showsRootHandles As Boolean
			Set(ByVal newValue As Boolean)
				Dim oldValue As Boolean = showsRootHandles
				Dim ___model As TreeModel = model
    
				showsRootHandles = newValue
				showsRootHandlesSet = True
				firePropertyChange(SHOWS_ROOT_HANDLES_PROPERTY, oldValue, showsRootHandles)
				If accessibleContext IsNot Nothing Then CType(accessibleContext, AccessibleJTree).fireVisibleDataPropertyChange()
				invalidate()
			End Set
			Get
				Return showsRootHandles
			End Get
		End Property


		''' <summary>
		''' Sets the height of each cell, in pixels.  If the specified value
		''' is less than or equal to zero the current cell renderer is
		''' queried for each row's height.
		''' <p>
		''' This is a bound property.
		''' </summary>
		''' <param name="rowHeight"> the height of each cell, in pixels
		''' @beaninfo
		'''        bound: true
		'''  description: The height of each cell. </param>
		Public Overridable Property rowHeight As Integer
			Set(ByVal rowHeight As Integer)
				Dim oldValue As Integer = Me.rowHeight
    
				Me.rowHeight = rowHeight
				rowHeightSet = True
				firePropertyChange(ROW_HEIGHT_PROPERTY, oldValue, Me.rowHeight)
				invalidate()
			End Set
			Get
				Return rowHeight
			End Get
		End Property


		''' <summary>
		''' Returns true if the height of each display row is a fixed size.
		''' </summary>
		''' <returns> true if the height of each row is a fixed size </returns>
		Public Overridable Property fixedRowHeight As Boolean
			Get
				Return (rowHeight > 0)
			End Get
		End Property

		''' <summary>
		''' Specifies whether the UI should use a large model.
		''' (Not all UIs will implement this.) Fires a property change
		''' for the LARGE_MODEL_PROPERTY.
		''' <p>
		''' This is a bound property.
		''' </summary>
		''' <param name="newValue"> true to suggest a large model to the UI </param>
		''' <seealso cref= #largeModel
		''' @beaninfo
		'''        bound: true
		'''  description: Whether the UI should use a
		'''               large model. </seealso>
		Public Overridable Property largeModel As Boolean
			Set(ByVal newValue As Boolean)
				Dim oldValue As Boolean = largeModel
    
				largeModel = newValue
				firePropertyChange(LARGE_MODEL_PROPERTY, oldValue, newValue)
			End Set
			Get
				Return largeModel
			End Get
		End Property


		''' <summary>
		''' Determines what happens when editing is interrupted by selecting
		''' another node in the tree, a change in the tree's data, or by some
		''' other means. Setting this property to <code>true</code> causes the
		''' changes to be automatically saved when editing is interrupted.
		''' <p>
		''' Fires a property change for the INVOKES_STOP_CELL_EDITING_PROPERTY.
		''' </summary>
		''' <param name="newValue"> true means that <code>stopCellEditing</code> is invoked
		'''        when editing is interrupted, and data is saved; false means that
		'''        <code>cancelCellEditing</code> is invoked, and changes are lost
		''' @beaninfo
		'''        bound: true
		'''  description: Determines what happens when editing is interrupted,
		'''               selecting another node in the tree, a change in the
		'''               tree's data, or some other means. </param>
		Public Overridable Property invokesStopCellEditing As Boolean
			Set(ByVal newValue As Boolean)
				Dim oldValue As Boolean = invokesStopCellEditing
    
				invokesStopCellEditing = newValue
				firePropertyChange(INVOKES_STOP_CELL_EDITING_PROPERTY, oldValue, newValue)
			End Set
			Get
				Return invokesStopCellEditing
			End Get
		End Property


		''' <summary>
		''' Sets the <code>scrollsOnExpand</code> property,
		''' which determines whether the
		''' tree might scroll to show previously hidden children.
		''' If this property is <code>true</code> (the default),
		''' when a node expands
		''' the tree can use scrolling to make
		''' the maximum possible number of the node's descendants visible.
		''' In some look and feels, trees might not need to scroll when expanded;
		''' those look and feels will ignore this property.
		''' <p>
		''' This is a bound property.
		''' </summary>
		''' <param name="newValue"> <code>false</code> to disable scrolling on expansion;
		'''                 <code>true</code> to enable it </param>
		''' <seealso cref= #getScrollsOnExpand
		''' 
		''' @beaninfo
		'''        bound: true
		'''  description: Indicates if a node descendant should be scrolled when expanded. </seealso>
		Public Overridable Property scrollsOnExpand As Boolean
			Set(ByVal newValue As Boolean)
				Dim oldValue As Boolean = scrollsOnExpand
    
				scrollsOnExpand = newValue
				scrollsOnExpandSet = True
				firePropertyChange(SCROLLS_ON_EXPAND_PROPERTY, oldValue, newValue)
			End Set
			Get
				Return scrollsOnExpand
			End Get
		End Property


		''' <summary>
		''' Sets the number of mouse clicks before a node will expand or close.
		''' The default is two.
		''' <p>
		''' This is a bound property.
		''' 
		''' @since 1.3
		''' @beaninfo
		'''        bound: true
		'''  description: Number of clicks before a node will expand/collapse.
		''' </summary>
		Public Overridable Property toggleClickCount As Integer
			Set(ByVal clickCount As Integer)
				Dim oldCount As Integer = toggleClickCount
    
				toggleClickCount = clickCount
				firePropertyChange(TOGGLE_CLICK_COUNT_PROPERTY, oldCount, clickCount)
			End Set
			Get
				Return toggleClickCount
			End Get
		End Property


		''' <summary>
		''' Configures the <code>expandsSelectedPaths</code> property. If
		''' true, any time the selection is changed, either via the
		''' <code>TreeSelectionModel</code>, or the cover methods provided by
		''' <code>JTree</code>, the <code>TreePath</code>s parents will be
		''' expanded to make them visible (visible meaning the parent path is
		''' expanded, not necessarily in the visible rectangle of the
		''' <code>JTree</code>). If false, when the selection
		''' changes the nodes parent is not made visible (all its parents expanded).
		''' This is useful if you wish to have your selection model maintain paths
		''' that are not always visible (all parents expanded).
		''' <p>
		''' This is a bound property.
		''' </summary>
		''' <param name="newValue"> the new value for <code>expandsSelectedPaths</code>
		''' 
		''' @since 1.3
		''' @beaninfo
		'''        bound: true
		'''  description: Indicates whether changes to the selection should make
		'''               the parent of the path visible. </param>
		Public Overridable Property expandsSelectedPaths As Boolean
			Set(ByVal newValue As Boolean)
				Dim oldValue As Boolean = expandsSelectedPaths
    
				expandsSelectedPaths = newValue
				firePropertyChange(EXPANDS_SELECTED_PATHS_PROPERTY, oldValue, newValue)
			End Set
			Get
				Return expandsSelectedPaths
			End Get
		End Property


		''' <summary>
		''' Turns on or off automatic drag handling. In order to enable automatic
		''' drag handling, this property should be set to {@code true}, and the
		''' tree's {@code TransferHandler} needs to be {@code non-null}.
		''' The default value of the {@code dragEnabled} property is {@code false}.
		''' <p>
		''' The job of honoring this property, and recognizing a user drag gesture,
		''' lies with the look and feel implementation, and in particular, the tree's
		''' {@code TreeUI}. When automatic drag handling is enabled, most look and
		''' feels (including those that subclass {@code BasicLookAndFeel}) begin a
		''' drag and drop operation whenever the user presses the mouse button over
		''' an item and then moves the mouse a few pixels. Setting this property to
		''' {@code true} can therefore have a subtle effect on how selections behave.
		''' <p>
		''' If a look and feel is used that ignores this property, you can still
		''' begin a drag and drop operation by calling {@code exportAsDrag} on the
		''' tree's {@code TransferHandler}.
		''' </summary>
		''' <param name="b"> whether or not to enable automatic drag handling </param>
		''' <exception cref="HeadlessException"> if
		'''            <code>b</code> is <code>true</code> and
		'''            <code>GraphicsEnvironment.isHeadless()</code>
		'''            returns <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= #getDragEnabled </seealso>
		''' <seealso cref= #setTransferHandler </seealso>
		''' <seealso cref= TransferHandler
		''' @since 1.4
		''' 
		''' @beaninfo
		'''  description: determines whether automatic drag handling is enabled
		'''        bound: false </seealso>
		Public Overridable Property dragEnabled As Boolean
			Set(ByVal b As Boolean)
				If b AndAlso GraphicsEnvironment.headless Then Throw New HeadlessException
				dragEnabled = b
			End Set
			Get
				Return dragEnabled
			End Get
		End Property


		''' <summary>
		''' Sets the drop mode for this component. For backward compatibility,
		''' the default for this property is <code>DropMode.USE_SELECTION</code>.
		''' Usage of one of the other modes is recommended, however, for an
		''' improved user experience. <code>DropMode.ON</code>, for instance,
		''' offers similar behavior of showing items as selected, but does so without
		''' affecting the actual selection in the tree.
		''' <p>
		''' <code>JTree</code> supports the following drop modes:
		''' <ul>
		'''    <li><code>DropMode.USE_SELECTION</code></li>
		'''    <li><code>DropMode.ON</code></li>
		'''    <li><code>DropMode.INSERT</code></li>
		'''    <li><code>DropMode.ON_OR_INSERT</code></li>
		''' </ul>
		''' <p>
		''' The drop mode is only meaningful if this component has a
		''' <code>TransferHandler</code> that accepts drops.
		''' </summary>
		''' <param name="dropMode"> the drop mode to use </param>
		''' <exception cref="IllegalArgumentException"> if the drop mode is unsupported
		'''         or <code>null</code> </exception>
		''' <seealso cref= #getDropMode </seealso>
		''' <seealso cref= #getDropLocation </seealso>
		''' <seealso cref= #setTransferHandler </seealso>
		''' <seealso cref= TransferHandler
		''' @since 1.6 </seealso>
		Public Property dropMode As DropMode
			Set(ByVal dropMode As DropMode)
				If dropMode IsNot Nothing Then
					Select Case dropMode
						Case DropMode.USE_SELECTION, ON, INSERT, ON_OR_INSERT
							Me.dropMode = dropMode
							Return
					End Select
				End If
    
				Throw New System.ArgumentException(dropMode & ": Unsupported drop mode for tree")
			End Set
			Get
				Return dropMode
			End Get
		End Property


		''' <summary>
		''' Calculates a drop location in this component, representing where a
		''' drop at the given point should insert data.
		''' </summary>
		''' <param name="p"> the point to calculate a drop location for </param>
		''' <returns> the drop location, or <code>null</code> </returns>
		Friend Overrides Function dropLocationForPoint(ByVal p As Point) As DropLocation
			Dim ___location As DropLocation = Nothing

			Dim row As Integer = getClosestRowForLocation(p.x, p.y)
			Dim ___bounds As Rectangle = getRowBounds(row)
			Dim ___model As TreeModel = model
			Dim root As Object = If(___model Is Nothing, Nothing, ___model.root)
			Dim rootPath As TreePath = If(root Is Nothing, Nothing, New TreePath(root))

			Dim child As TreePath
			Dim parent As TreePath
			Dim outside As Boolean = row = -1 OrElse p.y < ___bounds.y OrElse p.y >= ___bounds.y + ___bounds.height

			Select Case dropMode
				Case DropMode.USE_SELECTION, ON
					If outside Then
						___location = New DropLocation(p, Nothing, -1)
					Else
						___location = New DropLocation(p, getPathForRow(row), -1)
					End If

				Case DropMode.INSERT, ON_OR_INSERT
					If row = -1 Then
						If root IsNot Nothing AndAlso (Not ___model.isLeaf(root)) AndAlso isExpanded(rootPath) Then
							___location = New DropLocation(p, rootPath, 0)
						Else
							___location = New DropLocation(p, Nothing, -1)
						End If

						Exit Select
					End If

					Dim checkOn As Boolean = dropMode = DropMode.ON_OR_INSERT OrElse Not ___model.isLeaf(getPathForRow(row).lastPathComponent)

					Dim section As sun.swing.SwingUtilities2.Section = sun.swing.SwingUtilities2.liesInVertical(___bounds, p, checkOn)
					If section Is LEADING Then
						child = getPathForRow(row)
						parent = child.parentPath
					ElseIf section Is TRAILING Then
						Dim index As Integer = row + 1
						If index >= rowCount Then
							If ___model.isLeaf(root) OrElse (Not isExpanded(rootPath)) Then
								___location = New DropLocation(p, Nothing, -1)
							Else
								parent = rootPath
								index = ___model.getChildCount(root)
								___location = New DropLocation(p, parent, index)
							End If

							Exit Select
						End If

						child = getPathForRow(index)
						parent = child.parentPath
					Else
						Debug.Assert(checkOn)
						___location = New DropLocation(p, getPathForRow(row), -1)
						Exit Select
					End If

					If parent IsNot Nothing Then
						___location = New DropLocation(p, parent, ___model.getIndexOfChild(parent.lastPathComponent, child.lastPathComponent))
					ElseIf checkOn OrElse (Not ___model.isLeaf(root)) Then
						___location = New DropLocation(p, rootPath, -1)
					Else
						___location = New DropLocation(p, Nothing, -1)
					End If

				Case Else
					Debug.Assert(False, "Unexpected drop mode")
			End Select

			If outside OrElse row <> ___expandRow Then cancelDropTimer()

			If (Not outside) AndAlso row <> ___expandRow Then
				If isCollapsed(row) Then
					___expandRow = row
					startDropTimer()
				End If
			End If

			Return ___location
		End Function

		''' <summary>
		''' Called to set or clear the drop location during a DnD operation.
		''' In some cases, the component may need to use it's internal selection
		''' temporarily to indicate the drop location. To help facilitate this,
		''' this method returns and accepts as a parameter a state object.
		''' This state object can be used to store, and later restore, the selection
		''' state. Whatever this method returns will be passed back to it in
		''' future calls, as the state parameter. If it wants the DnD system to
		''' continue storing the same state, it must pass it back every time.
		''' Here's how this is used:
		''' <p>
		''' Let's say that on the first call to this method the component decides
		''' to save some state (because it is about to use the selection to show
		''' a drop index). It can return a state object to the caller encapsulating
		''' any saved selection state. On a second call, let's say the drop location
		''' is being changed to something else. The component doesn't need to
		''' restore anything yet, so it simply passes back the same state object
		''' to have the DnD system continue storing it. Finally, let's say this
		''' method is messaged with <code>null</code>. This means DnD
		''' is finished with this component for now, meaning it should restore
		''' state. At this point, it can use the state parameter to restore
		''' said state, and of course return <code>null</code> since there's
		''' no longer anything to store.
		''' </summary>
		''' <param name="location"> the drop location (as calculated by
		'''        <code>dropLocationForPoint</code>) or <code>null</code>
		'''        if there's no longer a valid drop location </param>
		''' <param name="state"> the state object saved earlier for this component,
		'''        or <code>null</code> </param>
		''' <param name="forDrop"> whether or not the method is being called because an
		'''        actual drop occurred </param>
		''' <returns> any saved state for this component, or <code>null</code> if none </returns>
		Friend Overrides Function setDropLocation(ByVal location As TransferHandler.DropLocation, ByVal state As Object, ByVal forDrop As Boolean) As Object

			Dim retVal As Object = Nothing
			Dim treeLocation As DropLocation = CType(location, DropLocation)

			If dropMode = DropMode.USE_SELECTION Then
				If treeLocation Is Nothing Then
					If (Not forDrop) AndAlso state IsNot Nothing Then
						selectionPaths = CType(state, TreePath()())(0)
						anchorSelectionPath = CType(state, TreePath()())(1)(0)
						leadSelectionPath = CType(state, TreePath()())(1)(1)
					End If
				Else
					If dropLocation Is Nothing Then
						Dim paths As TreePath() = selectionPaths
						If paths Is Nothing Then paths = New TreePath(){}

						retVal = New TreePath() {paths, {anchorSelectionPath, leadSelectionPath}}
					Else
						retVal = state
					End If

					selectionPath = treeLocation.path
				End If
			End If

			Dim old As DropLocation = dropLocation
			dropLocation = treeLocation
			firePropertyChange("dropLocation", old, dropLocation)

			Return retVal
		End Function

		''' <summary>
		''' Called to indicate to this component that DnD is done.
		''' Allows for us to cancel the expand timer.
		''' </summary>
		Friend Overrides Sub dndDone()
			cancelDropTimer()
			dropTimer = Nothing
		End Sub

		''' <summary>
		''' Returns the location that this component should visually indicate
		''' as the drop location during a DnD operation over the component,
		''' or {@code null} if no location is to currently be shown.
		''' <p>
		''' This method is not meant for querying the drop location
		''' from a {@code TransferHandler}, as the drop location is only
		''' set after the {@code TransferHandler}'s <code>canImport</code>
		''' has returned and has allowed for the location to be shown.
		''' <p>
		''' When this property changes, a property change event with
		''' name "dropLocation" is fired by the component.
		''' </summary>
		''' <returns> the drop location </returns>
		''' <seealso cref= #setDropMode </seealso>
		''' <seealso cref= TransferHandler#canImport(TransferHandler.TransferSupport)
		''' @since 1.6 </seealso>
		Public Property dropLocation As DropLocation
			Get
				Return dropLocation
			End Get
		End Property

		Private Sub startDropTimer()
			If dropTimer Is Nothing Then dropTimer = New TreeTimer(Me)
			dropTimer.start()
		End Sub

		Private Sub cancelDropTimer()
			If dropTimer IsNot Nothing AndAlso dropTimer.running Then
				___expandRow = -1
				dropTimer.stop()
			End If
		End Sub

		''' <summary>
		''' Returns <code>isEditable</code>. This is invoked from the UI before
		''' editing begins to insure that the given path can be edited. This
		''' is provided as an entry point for subclassers to add filtered
		''' editing without having to resort to creating a new editor.
		''' </summary>
		''' <returns> true if every parent node and the node itself is editable </returns>
		''' <seealso cref= #isEditable </seealso>
		Public Overridable Function isPathEditable(ByVal path As TreePath) As Boolean
			Return editable
		End Function

		''' <summary>
		''' Overrides <code>JComponent</code>'s <code>getToolTipText</code>
		''' method in order to allow
		''' renderer's tips to be used if it has text set.
		''' <p>
		''' NOTE: For <code>JTree</code> to properly display tooltips of its
		''' renderers, <code>JTree</code> must be a registered component with the
		''' <code>ToolTipManager</code>.  This can be done by invoking
		''' <code>ToolTipManager.sharedInstance().registerComponent(tree)</code>.
		''' This is not done automatically!
		''' </summary>
		''' <param name="event"> the <code>MouseEvent</code> that initiated the
		'''          <code>ToolTip</code> display </param>
		''' <returns> a string containing the  tooltip or <code>null</code>
		'''          if <code>event</code> is null </returns>
		Public Overrides Function getToolTipText(ByVal [event] As MouseEvent) As String
			Dim tip As String = Nothing

			If [event] IsNot Nothing Then
				Dim p As Point = [event].point
				Dim selRow As Integer = getRowForLocation(p.x, p.y)
				Dim r As TreeCellRenderer = cellRenderer

				If selRow <> -1 AndAlso r IsNot Nothing Then
					Dim path As TreePath = getPathForRow(selRow)
					Dim lastPath As Object = path.lastPathComponent
					Dim rComponent As Component = r.getTreeCellRendererComponent(Me, lastPath, isRowSelected(selRow), isExpanded(selRow), model.isLeaf(lastPath), selRow, True)

					If TypeOf rComponent Is JComponent Then
						Dim newEvent As MouseEvent
						Dim ___pathBounds As Rectangle = getPathBounds(path)

						p.translate(-___pathBounds.x, -___pathBounds.y)
						newEvent = New MouseEvent(rComponent, [event].iD, [event].when, [event].modifiers, p.x, p.y, [event].xOnScreen, [event].yOnScreen, [event].clickCount, [event].popupTrigger, MouseEvent.NOBUTTON)

						tip = CType(rComponent, JComponent).getToolTipText(newEvent)
					End If
				End If
			End If
			' No tip from the renderer get our own tip
			If tip Is Nothing Then tip = toolTipText
			Return tip
		End Function

		''' <summary>
		''' Called by the renderers to convert the specified value to
		''' text. This implementation returns <code>value.toString</code>, ignoring
		''' all other arguments. To control the conversion, subclass this
		''' method and use any of the arguments you need.
		''' </summary>
		''' <param name="value"> the <code>Object</code> to convert to text </param>
		''' <param name="selected"> true if the node is selected </param>
		''' <param name="expanded"> true if the node is expanded </param>
		''' <param name="leaf">  true if the node is a leaf node </param>
		''' <param name="row">  an integer specifying the node's display row, where 0 is
		'''             the first row in the display </param>
		''' <param name="hasFocus"> true if the node has the focus </param>
		''' <returns> the <code>String</code> representation of the node's value </returns>
		Public Overridable Function convertValueToText(ByVal value As Object, ByVal selected As Boolean, ByVal expanded As Boolean, ByVal leaf As Boolean, ByVal row As Integer, ByVal hasFocus As Boolean) As String
			If value IsNot Nothing Then
				Dim sValue As String = value.ToString()
				If sValue IsNot Nothing Then Return sValue
			End If
			Return ""
		End Function

		'
		' The following are convenience methods that get forwarded to the
		' current TreeUI.
		'

		''' <summary>
		''' Returns the number of viewable nodes. A node is viewable if all of its
		''' parents are expanded. The root is only included in this count if
		''' {@code isRootVisible()} is {@code true}. This returns {@code 0} if
		''' the UI has not been set.
		''' </summary>
		''' <returns> the number of viewable nodes </returns>
		Public Overridable Property rowCount As Integer
			Get
				Dim tree As TreeUI = uI
    
				If tree IsNot Nothing Then Return tree.getRowCount(Me)
				Return 0
			End Get
		End Property

		''' <summary>
		''' Selects the node identified by the specified path. If any
		''' component of the path is hidden (under a collapsed node), and
		''' <code>getExpandsSelectedPaths</code> is true it is
		''' exposed (made viewable).
		''' </summary>
		''' <param name="path"> the <code>TreePath</code> specifying the node to select </param>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setSelectionPath(ByVal path As TreePath) 'JavaToDotNetTempPropertySetselectionPath
		Public Overridable Property selectionPath As TreePath
			Set(ByVal path As TreePath)
				selectionModel.selectionPath = path
			End Set
			Get
		End Property

		''' <summary>
		''' Selects the nodes identified by the specified array of paths.
		''' If any component in any of the paths is hidden (under a collapsed
		''' node), and <code>getExpandsSelectedPaths</code> is true
		''' it is exposed (made viewable).
		''' </summary>
		''' <param name="paths"> an array of <code>TreePath</code> objects that specifies
		'''          the nodes to select </param>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setSelectionPaths(ByVal paths As TreePath[]) 'JavaToDotNetTempPropertySetselectionPaths
		Public Overridable Property selectionPaths As TreePath()
			Set(ByVal paths As TreePath())
				selectionModel.selectionPaths = paths
			End Set
			Get
		End Property

		''' <summary>
		''' Sets the path identifies as the lead. The lead may not be selected.
		''' The lead is not maintained by <code>JTree</code>,
		''' rather the UI will update it.
		''' <p>
		''' This is a bound property.
		''' </summary>
		''' <param name="newPath">  the new lead path
		''' @since 1.3
		''' @beaninfo
		'''        bound: true
		'''  description: Lead selection path </param>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setLeadSelectionPath(ByVal newPath As TreePath) 'JavaToDotNetTempPropertySetleadSelectionPath
		Public Overridable Property leadSelectionPath As TreePath
			Set(ByVal newPath As TreePath)
				Dim oldValue As TreePath = leadPath
    
				leadPath = newPath
				firePropertyChange(LEAD_SELECTION_PATH_PROPERTY, oldValue, newPath)
    
				If accessibleContext IsNot Nothing Then CType(accessibleContext, AccessibleJTree).fireActiveDescendantPropertyChange(oldValue, newPath)
			End Set
			Get
		End Property

		''' <summary>
		''' Sets the path identified as the anchor.
		''' The anchor is not maintained by <code>JTree</code>, rather the UI will
		''' update it.
		''' <p>
		''' This is a bound property.
		''' </summary>
		''' <param name="newPath">  the new anchor path
		''' @since 1.3
		''' @beaninfo
		'''        bound: true
		'''  description: Anchor selection path </param>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setAnchorSelectionPath(ByVal newPath As TreePath) 'JavaToDotNetTempPropertySetanchorSelectionPath
		Public Overridable Property anchorSelectionPath As TreePath
			Set(ByVal newPath As TreePath)
				Dim oldValue As TreePath = anchorPath
    
				anchorPath = newPath
				firePropertyChange(ANCHOR_SELECTION_PATH_PROPERTY, oldValue, newPath)
			End Set
			Get
		End Property

		''' <summary>
		''' Selects the node at the specified row in the display.
		''' </summary>
		''' <param name="row">  the row to select, where 0 is the first row in
		'''             the display </param>
		Public Overridable Property selectionRow As Integer
			Set(ByVal row As Integer)
				Dim rows As Integer() = { row }
    
				selectionRows = rows
			End Set
		End Property

		''' <summary>
		''' Selects the nodes corresponding to each of the specified rows
		''' in the display. If a particular element of <code>rows</code> is
		''' &lt; 0 or &gt;= <code>getRowCount</code>, it will be ignored.
		''' If none of the elements
		''' in <code>rows</code> are valid rows, the selection will
		''' be cleared. That is it will be as if <code>clearSelection</code>
		''' was invoked.
		''' </summary>
		''' <param name="rows">  an array of ints specifying the rows to select,
		'''              where 0 indicates the first row in the display </param>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setSelectionRows(ByVal rows As Integer[]) 'JavaToDotNetTempPropertySetselectionRows
		Public Overridable Property selectionRows As Integer()
			Set(ByVal rows As Integer())
				Dim ___ui As TreeUI = uI
    
				If ___ui IsNot Nothing AndAlso rows IsNot Nothing Then
					Dim numRows As Integer = rows.Length
					Dim paths As TreePath() = New TreePath(numRows - 1){}
    
					For counter As Integer = 0 To numRows - 1
						paths(counter) = ___ui.getPathForRow(Me, rows(counter))
					Next counter
					selectionPaths = paths
				End If
			End Set
			Get
		End Property

		''' <summary>
		''' Adds the node identified by the specified <code>TreePath</code>
		''' to the current selection. If any component of the path isn't
		''' viewable, and <code>getExpandsSelectedPaths</code> is true it is
		''' made viewable.
		''' <p>
		''' Note that <code>JTree</code> does not allow duplicate nodes to
		''' exist as children under the same parent -- each sibling must be
		''' a unique object.
		''' </summary>
		''' <param name="path"> the <code>TreePath</code> to add </param>
		Public Overridable Sub addSelectionPath(ByVal path As TreePath)
			selectionModel.addSelectionPath(path)
		End Sub

		''' <summary>
		''' Adds each path in the array of paths to the current selection. If
		''' any component of any of the paths isn't viewable and
		''' <code>getExpandsSelectedPaths</code> is true, it is
		''' made viewable.
		''' <p>
		''' Note that <code>JTree</code> does not allow duplicate nodes to
		''' exist as children under the same parent -- each sibling must be
		''' a unique object.
		''' </summary>
		''' <param name="paths"> an array of <code>TreePath</code> objects that specifies
		'''          the nodes to add </param>
		Public Overridable Sub addSelectionPaths(ByVal paths As TreePath())
			selectionModel.addSelectionPaths(paths)
		End Sub

		''' <summary>
		''' Adds the path at the specified row to the current selection.
		''' </summary>
		''' <param name="row">  an integer specifying the row of the node to add,
		'''             where 0 is the first row in the display </param>
		Public Overridable Sub addSelectionRow(ByVal row As Integer)
			Dim rows As Integer() = { row }

			addSelectionRows(rows)
		End Sub

		''' <summary>
		''' Adds the paths at each of the specified rows to the current selection.
		''' </summary>
		''' <param name="rows">  an array of ints specifying the rows to add,
		'''              where 0 indicates the first row in the display </param>
		Public Overridable Sub addSelectionRows(ByVal rows As Integer())
			Dim ___ui As TreeUI = uI

			If ___ui IsNot Nothing AndAlso rows IsNot Nothing Then
				Dim numRows As Integer = rows.Length
				Dim paths As TreePath() = New TreePath(numRows - 1){}

				For counter As Integer = 0 To numRows - 1
					paths(counter) = ___ui.getPathForRow(Me, rows(counter))
				Next counter
				addSelectionPaths(paths)
			End If
		End Sub

		''' <summary>
		''' Returns the last path component of the selected path. This is
		''' a convenience method for
		''' {@code getSelectionModel().getSelectionPath().getLastPathComponent()}.
		''' This is typically only useful if the selection has one path.
		''' </summary>
		''' <returns> the last path component of the selected path, or
		'''         <code>null</code> if nothing is selected </returns>
		''' <seealso cref= TreePath#getLastPathComponent </seealso>
		Public Overridable Property lastSelectedPathComponent As Object
			Get
				Dim selPath As TreePath = selectionModel.selectionPath
    
				If selPath IsNot Nothing Then Return selPath.lastPathComponent
				Return Nothing
			End Get
		End Property

			Return leadPath
		End Function

			Return anchorPath
		End Function

			Return selectionModel.selectionPath
		End Function

			Dim ___selectionPaths As TreePath() = selectionModel.selectionPaths

			Return If(___selectionPaths IsNot Nothing AndAlso ___selectionPaths.Length > 0, ___selectionPaths, Nothing)
		End Function

			Return selectionModel.selectionRows
		End Function

		''' <summary>
		''' Returns the number of nodes selected.
		''' </summary>
		''' <returns> the number of nodes selected </returns>
		Public Overridable Property selectionCount As Integer
			Get
				Return selectionModel.selectionCount
			End Get
		End Property

		''' <summary>
		''' Returns the smallest selected row. If the selection is empty, or
		''' none of the selected paths are viewable, {@code -1} is returned.
		''' </summary>
		''' <returns> the smallest selected row </returns>
		Public Overridable Property minSelectionRow As Integer
			Get
				Return selectionModel.minSelectionRow
			End Get
		End Property

		''' <summary>
		''' Returns the largest selected row. If the selection is empty, or
		''' none of the selected paths are viewable, {@code -1} is returned.
		''' </summary>
		''' <returns> the largest selected row </returns>
		Public Overridable Property maxSelectionRow As Integer
			Get
				Return selectionModel.maxSelectionRow
			End Get
		End Property

		''' <summary>
		''' Returns the row index corresponding to the lead path.
		''' </summary>
		''' <returns> an integer giving the row index of the lead path,
		'''          where 0 is the first row in the display; or -1
		'''          if <code>leadPath</code> is <code>null</code> </returns>
		Public Overridable Property leadSelectionRow As Integer
			Get
				Dim leadPath As TreePath = leadSelectionPath
    
				If leadPath IsNot Nothing Then Return getRowForPath(leadPath)
				Return -1
			End Get
		End Property

		''' <summary>
		''' Returns true if the item identified by the path is currently selected.
		''' </summary>
		''' <param name="path"> a <code>TreePath</code> identifying a node </param>
		''' <returns> true if the node is selected </returns>
		Public Overridable Function isPathSelected(ByVal path As TreePath) As Boolean
			Return selectionModel.isPathSelected(path)
		End Function

		''' <summary>
		''' Returns true if the node identified by row is selected.
		''' </summary>
		''' <param name="row">  an integer specifying a display row, where 0 is the first
		'''             row in the display </param>
		''' <returns> true if the node is selected </returns>
		Public Overridable Function isRowSelected(ByVal row As Integer) As Boolean
			Return selectionModel.isRowSelected(row)
		End Function

		''' <summary>
		''' Returns an <code>Enumeration</code> of the descendants of the
		''' path <code>parent</code> that
		''' are currently expanded. If <code>parent</code> is not currently
		''' expanded, this will return <code>null</code>.
		''' If you expand/collapse nodes while
		''' iterating over the returned <code>Enumeration</code>
		''' this may not return all
		''' the expanded paths, or may return paths that are no longer expanded.
		''' </summary>
		''' <param name="parent">  the path which is to be examined </param>
		''' <returns> an <code>Enumeration</code> of the descendents of
		'''          <code>parent</code>, or <code>null</code> if
		'''          <code>parent</code> is not currently expanded </returns>
		Public Overridable Function getExpandedDescendants(ByVal parent As TreePath) As System.Collections.IEnumerator(Of TreePath)
			If Not isExpanded(parent) Then Return Nothing

			Dim toggledPaths As System.Collections.IEnumerator(Of TreePath) = expandedState.Keys.GetEnumerator()
			Dim elements As List(Of TreePath) = Nothing
			Dim path As TreePath
			Dim value As Object

			If toggledPaths IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Do While toggledPaths.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					path = toggledPaths.nextElement()
					value = expandedState(path)
					' Add the path if it is expanded, a descendant of parent,
					' and it is visible (all parents expanded). This is rather
					' expensive!
					If path IsNot parent AndAlso value IsNot Nothing AndAlso CBool(value) AndAlso parent.isDescendant(path) AndAlso isVisible(path) Then
						If elements Is Nothing Then elements = New List(Of TreePath)
						elements.Add(path)
					End If
				Loop
			End If
			If elements Is Nothing Then
				Dim empty As [Set](Of TreePath) = Collections.emptySet()
				Return Collections.enumeration(empty)
			End If
			Return elements.elements()
		End Function

		''' <summary>
		''' Returns true if the node identified by the path has ever been
		''' expanded. </summary>
		''' <returns> true if the <code>path</code> has ever been expanded </returns>
		Public Overridable Function hasBeenExpanded(ByVal path As TreePath) As Boolean
			Return (path IsNot Nothing AndAlso expandedState(path) IsNot Nothing)
		End Function

		''' <summary>
		''' Returns true if the node identified by the path is currently expanded,
		''' </summary>
		''' <param name="path">  the <code>TreePath</code> specifying the node to check </param>
		''' <returns> false if any of the nodes in the node's path are collapsed,
		'''               true if all nodes in the path are expanded </returns>
		Public Overridable Function isExpanded(ByVal path As TreePath) As Boolean

			If path Is Nothing Then Return False
			Dim value As Object

			Do
				value = expandedState(path)
				If value Is Nothing OrElse (Not CBool(value)) Then Return False
				path=path.parentPath
			Loop While path IsNot Nothing

			Return True
		End Function

		''' <summary>
		''' Returns true if the node at the specified display row is currently
		''' expanded.
		''' </summary>
		''' <param name="row">  the row to check, where 0 is the first row in the
		'''             display </param>
		''' <returns> true if the node is currently expanded, otherwise false </returns>
		Public Overridable Function isExpanded(ByVal row As Integer) As Boolean
			Dim tree As TreeUI = uI

			If tree IsNot Nothing Then
				Dim path As TreePath = tree.getPathForRow(Me, row)

				If path IsNot Nothing Then
					Dim value As Boolean? = expandedState(path)

					Return (value IsNot Nothing AndAlso value)
				End If
			End If
			Return False
		End Function

		''' <summary>
		''' Returns true if the value identified by path is currently collapsed,
		''' this will return false if any of the values in path are currently
		''' not being displayed.
		''' </summary>
		''' <param name="path">  the <code>TreePath</code> to check </param>
		''' <returns> true if any of the nodes in the node's path are collapsed,
		'''               false if all nodes in the path are expanded </returns>
		Public Overridable Function isCollapsed(ByVal path As TreePath) As Boolean
			Return Not isExpanded(path)
		End Function

		''' <summary>
		''' Returns true if the node at the specified display row is collapsed.
		''' </summary>
		''' <param name="row">  the row to check, where 0 is the first row in the
		'''             display </param>
		''' <returns> true if the node is currently collapsed, otherwise false </returns>
		Public Overridable Function isCollapsed(ByVal row As Integer) As Boolean
			Return Not isExpanded(row)
		End Function

		''' <summary>
		''' Ensures that the node identified by path is currently viewable.
		''' </summary>
		''' <param name="path">  the <code>TreePath</code> to make visible </param>
		Public Overridable Sub makeVisible(ByVal path As TreePath)
			If path IsNot Nothing Then
				Dim parentPath As TreePath = path.parentPath

				If parentPath IsNot Nothing Then expandPath(parentPath)
			End If
		End Sub

		''' <summary>
		''' Returns true if the value identified by path is currently viewable,
		''' which means it is either the root or all of its parents are expanded.
		''' Otherwise, this method returns false.
		''' </summary>
		''' <returns> true if the node is viewable, otherwise false </returns>
		Public Overridable Function isVisible(ByVal path As TreePath) As Boolean
			If path IsNot Nothing Then
				Dim parentPath As TreePath = path.parentPath

				If parentPath IsNot Nothing Then Return isExpanded(parentPath)
				' Root.
				Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Returns the <code>Rectangle</code> that the specified node will be drawn
		''' into. Returns <code>null</code> if any component in the path is hidden
		''' (under a collapsed parent).
		''' <p>
		''' Note:<br>
		''' This method returns a valid rectangle, even if the specified
		''' node is not currently displayed.
		''' </summary>
		''' <param name="path"> the <code>TreePath</code> identifying the node </param>
		''' <returns> the <code>Rectangle</code> the node is drawn in,
		'''          or <code>null</code> </returns>
		Public Overridable Function getPathBounds(ByVal path As TreePath) As Rectangle
			Dim tree As TreeUI = uI

			If tree IsNot Nothing Then Return tree.getPathBounds(Me, path)
			Return Nothing
		End Function

		''' <summary>
		''' Returns the <code>Rectangle</code> that the node at the specified row is
		''' drawn in.
		''' </summary>
		''' <param name="row">  the row to be drawn, where 0 is the first row in the
		'''             display </param>
		''' <returns> the <code>Rectangle</code> the node is drawn in </returns>
		Public Overridable Function getRowBounds(ByVal row As Integer) As Rectangle
			Return getPathBounds(getPathForRow(row))
		End Function

		''' <summary>
		''' Makes sure all the path components in path are expanded (except
		''' for the last path component) and scrolls so that the
		''' node identified by the path is displayed. Only works when this
		''' <code>JTree</code> is contained in a <code>JScrollPane</code>.
		''' </summary>
		''' <param name="path">  the <code>TreePath</code> identifying the node to
		'''          bring into view </param>
		Public Overridable Sub scrollPathToVisible(ByVal path As TreePath)
			If path IsNot Nothing Then
				makeVisible(path)

				Dim ___bounds As Rectangle = getPathBounds(path)

				If ___bounds IsNot Nothing Then
					scrollRectToVisible(___bounds)
					If accessibleContext IsNot Nothing Then CType(accessibleContext, AccessibleJTree).fireVisibleDataPropertyChange()
				End If
			End If
		End Sub

		''' <summary>
		''' Scrolls the item identified by row until it is displayed. The minimum
		''' of amount of scrolling necessary to bring the row into view
		''' is performed. Only works when this <code>JTree</code> is contained in a
		''' <code>JScrollPane</code>.
		''' </summary>
		''' <param name="row">  an integer specifying the row to scroll, where 0 is the
		'''             first row in the display </param>
		Public Overridable Sub scrollRowToVisible(ByVal row As Integer)
			scrollPathToVisible(getPathForRow(row))
		End Sub

		''' <summary>
		''' Returns the path for the specified row.  If <code>row</code> is
		''' not visible, or a {@code TreeUI} has not been set, <code>null</code>
		''' is returned.
		''' </summary>
		''' <param name="row">  an integer specifying a row </param>
		''' <returns> the <code>TreePath</code> to the specified node,
		'''          <code>null</code> if <code>row &lt; 0</code>
		'''          or <code>row &gt;= getRowCount()</code> </returns>
		Public Overridable Function getPathForRow(ByVal row As Integer) As TreePath
			Dim tree As TreeUI = uI

			If tree IsNot Nothing Then Return tree.getPathForRow(Me, row)
			Return Nothing
		End Function

		''' <summary>
		''' Returns the row that displays the node identified by the specified
		''' path.
		''' </summary>
		''' <param name="path">  the <code>TreePath</code> identifying a node </param>
		''' <returns> an integer specifying the display row, where 0 is the first
		'''         row in the display, or -1 if any of the elements in path
		'''         are hidden under a collapsed parent. </returns>
		Public Overridable Function getRowForPath(ByVal path As TreePath) As Integer
			Dim tree As TreeUI = uI

			If tree IsNot Nothing Then Return tree.getRowForPath(Me, path)
			Return -1
		End Function

		''' <summary>
		''' Ensures that the node identified by the specified path is
		''' expanded and viewable. If the last item in the path is a
		''' leaf, this will have no effect.
		''' </summary>
		''' <param name="path">  the <code>TreePath</code> identifying a node </param>
		Public Overridable Sub expandPath(ByVal path As TreePath)
			' Only expand if not leaf!
			Dim ___model As TreeModel = model

			If path IsNot Nothing AndAlso ___model IsNot Nothing AndAlso (Not ___model.isLeaf(path.lastPathComponent)) Then expandedStateate(path, True)
		End Sub

		''' <summary>
		''' Ensures that the node in the specified row is expanded and
		''' viewable.
		''' <p>
		''' If <code>row</code> is &lt; 0 or &gt;= <code>getRowCount</code> this
		''' will have no effect.
		''' </summary>
		''' <param name="row">  an integer specifying a display row, where 0 is the
		'''             first row in the display </param>
		Public Overridable Sub expandRow(ByVal row As Integer)
			expandPath(getPathForRow(row))
		End Sub

		''' <summary>
		''' Ensures that the node identified by the specified path is
		''' collapsed and viewable.
		''' </summary>
		''' <param name="path">  the <code>TreePath</code> identifying a node </param>
		Public Overridable Sub collapsePath(ByVal path As TreePath)
			expandedStateate(path, False)
		End Sub

		''' <summary>
		''' Ensures that the node in the specified row is collapsed.
		''' <p>
		''' If <code>row</code> is &lt; 0 or &gt;= <code>getRowCount</code> this
		''' will have no effect.
		''' </summary>
		''' <param name="row">  an integer specifying a display row, where 0 is the
		'''             first row in the display </param>
		Public Overridable Sub collapseRow(ByVal row As Integer)
			collapsePath(getPathForRow(row))
		End Sub

		''' <summary>
		''' Returns the path for the node at the specified location.
		''' </summary>
		''' <param name="x"> an integer giving the number of pixels horizontally from
		'''          the left edge of the display area, minus any left margin </param>
		''' <param name="y"> an integer giving the number of pixels vertically from
		'''          the top of the display area, minus any top margin </param>
		''' <returns>  the <code>TreePath</code> for the node at that location </returns>
		Public Overridable Function getPathForLocation(ByVal x As Integer, ByVal y As Integer) As TreePath
			Dim closestPath As TreePath = getClosestPathForLocation(x, y)

			If closestPath IsNot Nothing Then
				Dim ___pathBounds As Rectangle = getPathBounds(closestPath)

				If ___pathBounds IsNot Nothing AndAlso x >= ___pathBounds.x AndAlso x < (___pathBounds.x + ___pathBounds.width) AndAlso y >= ___pathBounds.y AndAlso y < (___pathBounds.y + ___pathBounds.height) Then Return closestPath
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Returns the row for the specified location.
		''' </summary>
		''' <param name="x"> an integer giving the number of pixels horizontally from
		'''          the left edge of the display area, minus any left margin </param>
		''' <param name="y"> an integer giving the number of pixels vertically from
		'''          the top of the display area, minus any top margin </param>
		''' <returns> the row corresponding to the location, or -1 if the
		'''         location is not within the bounds of a displayed cell </returns>
		''' <seealso cref= #getClosestRowForLocation </seealso>
		Public Overridable Function getRowForLocation(ByVal x As Integer, ByVal y As Integer) As Integer
			Return getRowForPath(getPathForLocation(x, y))
		End Function

		''' <summary>
		''' Returns the path to the node that is closest to x,y.  If
		''' no nodes are currently viewable, or there is no model, returns
		''' <code>null</code>, otherwise it always returns a valid path.  To test if
		''' the node is exactly at x, y, get the node's bounds and
		''' test x, y against that.
		''' </summary>
		''' <param name="x"> an integer giving the number of pixels horizontally from
		'''          the left edge of the display area, minus any left margin </param>
		''' <param name="y"> an integer giving the number of pixels vertically from
		'''          the top of the display area, minus any top margin </param>
		''' <returns>  the <code>TreePath</code> for the node closest to that location,
		'''          <code>null</code> if nothing is viewable or there is no model
		''' </returns>
		''' <seealso cref= #getPathForLocation </seealso>
		''' <seealso cref= #getPathBounds </seealso>
		Public Overridable Function getClosestPathForLocation(ByVal x As Integer, ByVal y As Integer) As TreePath
			Dim tree As TreeUI = uI

			If tree IsNot Nothing Then Return tree.getClosestPathForLocation(Me, x, y)
			Return Nothing
		End Function

		''' <summary>
		''' Returns the row to the node that is closest to x,y.  If no nodes
		''' are viewable or there is no model, returns -1. Otherwise,
		''' it always returns a valid row.  To test if the returned object is
		''' exactly at x, y, get the bounds for the node at the returned
		''' row and test x, y against that.
		''' </summary>
		''' <param name="x"> an integer giving the number of pixels horizontally from
		'''          the left edge of the display area, minus any left margin </param>
		''' <param name="y"> an integer giving the number of pixels vertically from
		'''          the top of the display area, minus any top margin </param>
		''' <returns> the row closest to the location, -1 if nothing is
		'''         viewable or there is no model
		''' </returns>
		''' <seealso cref= #getRowForLocation </seealso>
		''' <seealso cref= #getRowBounds </seealso>
		Public Overridable Function getClosestRowForLocation(ByVal x As Integer, ByVal y As Integer) As Integer
			Return getRowForPath(getClosestPathForLocation(x, y))
		End Function

		''' <summary>
		''' Returns true if the tree is being edited. The item that is being
		''' edited can be obtained using <code>getSelectionPath</code>.
		''' </summary>
		''' <returns> true if the user is currently editing a node </returns>
		''' <seealso cref= #getSelectionPath </seealso>
		Public Overridable Property editing As Boolean
			Get
				Dim tree As TreeUI = uI
    
				If tree IsNot Nothing Then Return tree.isEditing(Me)
				Return False
			End Get
		End Property

		''' <summary>
		''' Ends the current editing session.
		''' (The <code>DefaultTreeCellEditor</code>
		''' object saves any edits that are currently in progress on a cell.
		''' Other implementations may operate differently.)
		''' Has no effect if the tree isn't being edited.
		''' <blockquote>
		''' <b>Note:</b><br>
		''' To make edit-saves automatic whenever the user changes
		''' their position in the tree, use <seealso cref="#setInvokesStopCellEditing"/>.
		''' </blockquote>
		''' </summary>
		''' <returns> true if editing was in progress and is now stopped,
		'''              false if editing was not in progress </returns>
		Public Overridable Function stopEditing() As Boolean
			Dim tree As TreeUI = uI

			If tree IsNot Nothing Then Return tree.stopEditing(Me)
			Return False
		End Function

		''' <summary>
		''' Cancels the current editing session. Has no effect if the
		''' tree isn't being edited.
		''' </summary>
		Public Overridable Sub cancelEditing()
			Dim tree As TreeUI = uI

			If tree IsNot Nothing Then tree.cancelEditing(Me)
		End Sub

		''' <summary>
		''' Selects the node identified by the specified path and initiates
		''' editing.  The edit-attempt fails if the <code>CellEditor</code>
		''' does not allow
		''' editing for the specified item.
		''' </summary>
		''' <param name="path">  the <code>TreePath</code> identifying a node </param>
		Public Overridable Sub startEditingAtPath(ByVal path As TreePath)
			Dim tree As TreeUI = uI

			If tree IsNot Nothing Then tree.startEditingAtPath(Me, path)
		End Sub

		''' <summary>
		''' Returns the path to the element that is currently being edited.
		''' </summary>
		''' <returns>  the <code>TreePath</code> for the node being edited </returns>
		Public Overridable Property editingPath As TreePath
			Get
				Dim tree As TreeUI = uI
    
				If tree IsNot Nothing Then Return tree.getEditingPath(Me)
				Return Nothing
			End Get
		End Property

		'
		' Following are primarily convenience methods for mapping from
		' row based selections to path selections.  Sometimes it is
		' easier to deal with these than paths (mouse downs, key downs
		' usually just deal with index based selections).
		' Since row based selections require a UI many of these won't work
		' without one.
		'

		''' <summary>
		''' Sets the tree's selection model. When a <code>null</code> value is
		''' specified an empty
		''' <code>selectionModel</code> is used, which does not allow selections.
		''' <p>
		''' This is a bound property.
		''' </summary>
		''' <param name="selectionModel"> the <code>TreeSelectionModel</code> to use,
		'''          or <code>null</code> to disable selections </param>
		''' <seealso cref= TreeSelectionModel
		''' @beaninfo
		'''        bound: true
		'''  description: The tree's selection model. </seealso>
		Public Overridable Property selectionModel As TreeSelectionModel
			Set(ByVal selectionModel As TreeSelectionModel)
				If selectionModel Is Nothing Then selectionModel = EmptySelectionModel.sharedInstance()
    
				Dim oldValue As TreeSelectionModel = Me.selectionModel
    
				If Me.selectionModel IsNot Nothing AndAlso selectionRedirector IsNot Nothing Then Me.selectionModel.removeTreeSelectionListener(selectionRedirector)
				If accessibleContext IsNot Nothing Then
				   Me.selectionModel.removeTreeSelectionListener(CType(accessibleContext, TreeSelectionListener))
				   selectionModel.addTreeSelectionListener(CType(accessibleContext, TreeSelectionListener))
				End If
    
				Me.selectionModel = selectionModel
				If selectionRedirector IsNot Nothing Then Me.selectionModel.addTreeSelectionListener(selectionRedirector)
				firePropertyChange(SELECTION_MODEL_PROPERTY, oldValue, Me.selectionModel)
    
				If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_SELECTION_PROPERTY, Convert.ToBoolean(False), Convert.ToBoolean(True))
			End Set
			Get
				Return selectionModel
			End Get
		End Property


		''' <summary>
		''' Returns the paths (inclusive) between the specified rows. If
		''' the specified indices are within the viewable set of rows, or
		''' bound the viewable set of rows, then the indices are
		''' constrained by the viewable set of rows. If the specified
		''' indices are not within the viewable set of rows, or do not
		''' bound the viewable set of rows, then an empty array is
		''' returned. For example, if the row count is {@code 10}, and this
		''' method is invoked with {@code -1, 20}, then the specified
		''' indices are constrained to the viewable set of rows, and this is
		''' treated as if invoked with {@code 0, 9}. On the other hand, if
		''' this were invoked with {@code -10, -1}, then the specified
		''' indices do not bound the viewable set of rows, and an empty
		''' array is returned.
		''' <p>
		''' The parameters are not order dependent. That is, {@code
		''' getPathBetweenRows(x, y)} is equivalent to
		''' {@code getPathBetweenRows(y, x)}.
		''' <p>
		''' An empty array is returned if the row count is {@code 0}, or
		''' the specified indices do not bound the viewable set of rows.
		''' </summary>
		''' <param name="index0"> the first index in the range </param>
		''' <param name="index1"> the last index in the range </param>
		''' <returns> the paths (inclusive) between the specified row indices </returns>
		Protected Friend Overridable Function getPathBetweenRows(ByVal index0 As Integer, ByVal index1 As Integer) As TreePath()
			Dim tree As TreeUI = uI
			If tree IsNot Nothing Then
				Dim ___rowCount As Integer = rowCount
				If ___rowCount > 0 AndAlso Not((index0 < 0 AndAlso index1 < 0) OrElse (index0 >= ___rowCount AndAlso index1 >= ___rowCount)) Then
					index0 = Math.Min(___rowCount - 1, Math.Max(index0, 0))
					index1 = Math.Min(___rowCount - 1, Math.Max(index1, 0))
					Dim minIndex As Integer = Math.Min(index0, index1)
					Dim maxIndex As Integer = Math.Max(index0, index1)
					Dim selection As TreePath() = New TreePath(maxIndex - minIndex){}
					For counter As Integer = minIndex To maxIndex
						selection(counter - minIndex) = tree.getPathForRow(Me, counter)
					Next counter
					Return selection
				End If
			End If
			Return New TreePath(){}
		End Function

		''' <summary>
		''' Selects the rows in the specified interval (inclusive). If
		''' the specified indices are within the viewable set of rows, or bound
		''' the viewable set of rows, then the specified rows are constrained by
		''' the viewable set of rows. If the specified indices are not within the
		''' viewable set of rows, or do not bound the viewable set of rows, then
		''' the selection is cleared. For example, if the row count is {@code
		''' 10}, and this method is invoked with {@code -1, 20}, then the
		''' specified indices bounds the viewable range, and this is treated as
		''' if invoked with {@code 0, 9}. On the other hand, if this were
		''' invoked with {@code -10, -1}, then the specified indices do not
		''' bound the viewable set of rows, and the selection is cleared.
		''' <p>
		''' The parameters are not order dependent. That is, {@code
		''' setSelectionInterval(x, y)} is equivalent to
		''' {@code setSelectionInterval(y, x)}.
		''' </summary>
		''' <param name="index0"> the first index in the range to select </param>
		''' <param name="index1"> the last index in the range to select </param>
		Public Overridable Sub setSelectionInterval(ByVal index0 As Integer, ByVal index1 As Integer)
			Dim paths As TreePath() = getPathBetweenRows(index0, index1)

			Me.selectionModel.selectionPaths = paths
		End Sub

		''' <summary>
		''' Adds the specified rows (inclusive) to the selection. If the
		''' specified indices are within the viewable set of rows, or bound
		''' the viewable set of rows, then the specified indices are
		''' constrained by the viewable set of rows. If the indices are not
		''' within the viewable set of rows, or do not bound the viewable
		''' set of rows, then the selection is unchanged. For example, if
		''' the row count is {@code 10}, and this method is invoked with
		''' {@code -1, 20}, then the specified indices bounds the viewable
		''' range, and this is treated as if invoked with {@code 0, 9}. On
		''' the other hand, if this were invoked with {@code -10, -1}, then
		''' the specified indices do not bound the viewable set of rows,
		''' and the selection is unchanged.
		''' <p>
		''' The parameters are not order dependent. That is, {@code
		''' addSelectionInterval(x, y)} is equivalent to
		''' {@code addSelectionInterval(y, x)}.
		''' </summary>
		''' <param name="index0"> the first index in the range to add to the selection </param>
		''' <param name="index1"> the last index in the range to add to the selection </param>
		Public Overridable Sub addSelectionInterval(ByVal index0 As Integer, ByVal index1 As Integer)
			Dim paths As TreePath() = getPathBetweenRows(index0, index1)

			If paths IsNot Nothing AndAlso paths.Length > 0 Then Me.selectionModel.addSelectionPaths(paths)
		End Sub

		''' <summary>
		''' Removes the specified rows (inclusive) from the selection. If
		''' the specified indices are within the viewable set of rows, or bound
		''' the viewable set of rows, then the specified indices are constrained by
		''' the viewable set of rows. If the specified indices are not within the
		''' viewable set of rows, or do not bound the viewable set of rows, then
		''' the selection is unchanged. For example, if the row count is {@code
		''' 10}, and this method is invoked with {@code -1, 20}, then the
		''' specified range bounds the viewable range, and this is treated as
		''' if invoked with {@code 0, 9}. On the other hand, if this were
		''' invoked with {@code -10, -1}, then the specified range does not
		''' bound the viewable set of rows, and the selection is unchanged.
		''' <p>
		''' The parameters are not order dependent. That is, {@code
		''' removeSelectionInterval(x, y)} is equivalent to
		''' {@code removeSelectionInterval(y, x)}.
		''' </summary>
		''' <param name="index0"> the first row to remove from the selection </param>
		''' <param name="index1"> the last row to remove from the selection </param>
		Public Overridable Sub removeSelectionInterval(ByVal index0 As Integer, ByVal index1 As Integer)
			Dim paths As TreePath() = getPathBetweenRows(index0, index1)

			If paths IsNot Nothing AndAlso paths.Length > 0 Then Me.selectionModel.removeSelectionPaths(paths)
		End Sub

		''' <summary>
		''' Removes the node identified by the specified path from the current
		''' selection.
		''' </summary>
		''' <param name="path">  the <code>TreePath</code> identifying a node </param>
		Public Overridable Sub removeSelectionPath(ByVal path As TreePath)
			Me.selectionModel.removeSelectionPath(path)
		End Sub

		''' <summary>
		''' Removes the nodes identified by the specified paths from the
		''' current selection.
		''' </summary>
		''' <param name="paths"> an array of <code>TreePath</code> objects that
		'''              specifies the nodes to remove </param>
		Public Overridable Sub removeSelectionPaths(ByVal paths As TreePath())
			Me.selectionModel.removeSelectionPaths(paths)
		End Sub

		''' <summary>
		''' Removes the row at the index <code>row</code> from the current
		''' selection.
		''' </summary>
		''' <param name="row">  the row to remove </param>
		Public Overridable Sub removeSelectionRow(ByVal row As Integer)
			Dim rows As Integer() = { row }

			removeSelectionRows(rows)
		End Sub

		''' <summary>
		''' Removes the rows that are selected at each of the specified
		''' rows.
		''' </summary>
		''' <param name="rows">  an array of ints specifying display rows, where 0 is
		'''             the first row in the display </param>
		Public Overridable Sub removeSelectionRows(ByVal rows As Integer())
			Dim ___ui As TreeUI = uI

			If ___ui IsNot Nothing AndAlso rows IsNot Nothing Then
				Dim numRows As Integer = rows.Length
				Dim paths As TreePath() = New TreePath(numRows - 1){}

				For counter As Integer = 0 To numRows - 1
					paths(counter) = ___ui.getPathForRow(Me, rows(counter))
				Next counter
				removeSelectionPaths(paths)
			End If
		End Sub

		''' <summary>
		''' Clears the selection.
		''' </summary>
		Public Overridable Sub clearSelection()
			selectionModel.clearSelection()
		End Sub

		''' <summary>
		''' Returns true if the selection is currently empty.
		''' </summary>
		''' <returns> true if the selection is currently empty </returns>
		Public Overridable Property selectionEmpty As Boolean
			Get
				Return selectionModel.selectionEmpty
			End Get
		End Property

		''' <summary>
		''' Adds a listener for <code>TreeExpansion</code> events.
		''' </summary>
		''' <param name="tel"> a TreeExpansionListener that will be notified when
		'''            a tree node is expanded or collapsed (a "negative
		'''            expansion") </param>
		Public Overridable Sub addTreeExpansionListener(ByVal tel As TreeExpansionListener)
			If settingUI Then uiTreeExpansionListener = tel
			listenerList.add(GetType(TreeExpansionListener), tel)
		End Sub

		''' <summary>
		''' Removes a listener for <code>TreeExpansion</code> events.
		''' </summary>
		''' <param name="tel"> the <code>TreeExpansionListener</code> to remove </param>
		Public Overridable Sub removeTreeExpansionListener(ByVal tel As TreeExpansionListener)
			listenerList.remove(GetType(TreeExpansionListener), tel)
			If uiTreeExpansionListener Is tel Then uiTreeExpansionListener = Nothing
		End Sub

		''' <summary>
		''' Returns an array of all the <code>TreeExpansionListener</code>s added
		''' to this JTree with addTreeExpansionListener().
		''' </summary>
		''' <returns> all of the <code>TreeExpansionListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		Public Overridable Property treeExpansionListeners As TreeExpansionListener()
			Get
				Return listenerList.getListeners(GetType(TreeExpansionListener))
			End Get
		End Property

		''' <summary>
		''' Adds a listener for <code>TreeWillExpand</code> events.
		''' </summary>
		''' <param name="tel"> a <code>TreeWillExpandListener</code> that will be notified
		'''            when a tree node will be expanded or collapsed (a "negative
		'''            expansion") </param>
		Public Overridable Sub addTreeWillExpandListener(ByVal tel As TreeWillExpandListener)
			listenerList.add(GetType(TreeWillExpandListener), tel)
		End Sub

		''' <summary>
		''' Removes a listener for <code>TreeWillExpand</code> events.
		''' </summary>
		''' <param name="tel"> the <code>TreeWillExpandListener</code> to remove </param>
		Public Overridable Sub removeTreeWillExpandListener(ByVal tel As TreeWillExpandListener)
			listenerList.remove(GetType(TreeWillExpandListener), tel)
		End Sub

		''' <summary>
		''' Returns an array of all the <code>TreeWillExpandListener</code>s added
		''' to this JTree with addTreeWillExpandListener().
		''' </summary>
		''' <returns> all of the <code>TreeWillExpandListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		Public Overridable Property treeWillExpandListeners As TreeWillExpandListener()
			Get
				Return listenerList.getListeners(GetType(TreeWillExpandListener))
			End Get
		End Property

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the <code>path</code> parameter.
		''' </summary>
		''' <param name="path"> the <code>TreePath</code> indicating the node that was
		'''          expanded </param>
		''' <seealso cref= EventListenerList </seealso>
		 Public Overridable Sub fireTreeExpanded(ByVal path As TreePath)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As TreeExpansionEvent = Nothing
			If uiTreeExpansionListener IsNot Nothing Then
				e = New TreeExpansionEvent(Me, path)
				uiTreeExpansionListener.treeExpanded(e)
			End If
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(TreeExpansionListener) AndAlso ___listeners(i + 1) IsNot uiTreeExpansionListener Then
					' Lazily create the event:
					If e Is Nothing Then e = New TreeExpansionEvent(Me, path)
					CType(___listeners(i+1), TreeExpansionListener).treeExpanded(e)
				End If
			Next i
		 End Sub

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the <code>path</code> parameter.
		''' </summary>
		''' <param name="path"> the <code>TreePath</code> indicating the node that was
		'''          collapsed </param>
		''' <seealso cref= EventListenerList </seealso>
		Public Overridable Sub fireTreeCollapsed(ByVal path As TreePath)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As TreeExpansionEvent = Nothing
			If uiTreeExpansionListener IsNot Nothing Then
				e = New TreeExpansionEvent(Me, path)
				uiTreeExpansionListener.treeCollapsed(e)
			End If
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(TreeExpansionListener) AndAlso ___listeners(i + 1) IsNot uiTreeExpansionListener Then
					' Lazily create the event:
					If e Is Nothing Then e = New TreeExpansionEvent(Me, path)
					CType(___listeners(i+1), TreeExpansionListener).treeCollapsed(e)
				End If
			Next i
		End Sub

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the <code>path</code> parameter.
		''' </summary>
		''' <param name="path"> the <code>TreePath</code> indicating the node that was
		'''          expanded </param>
		''' <seealso cref= EventListenerList </seealso>
		 Public Overridable Sub fireTreeWillExpand(ByVal path As TreePath)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As TreeExpansionEvent = Nothing
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(TreeWillExpandListener) Then
					' Lazily create the event:
					If e Is Nothing Then e = New TreeExpansionEvent(Me, path)
					CType(___listeners(i+1), TreeWillExpandListener).treeWillExpand(e)
				End If
			Next i
		 End Sub

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the <code>path</code> parameter.
		''' </summary>
		''' <param name="path"> the <code>TreePath</code> indicating the node that was
		'''          expanded </param>
		''' <seealso cref= EventListenerList </seealso>
		 Public Overridable Sub fireTreeWillCollapse(ByVal path As TreePath)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As TreeExpansionEvent = Nothing
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(TreeWillExpandListener) Then
					' Lazily create the event:
					If e Is Nothing Then e = New TreeExpansionEvent(Me, path)
					CType(___listeners(i+1), TreeWillExpandListener).treeWillCollapse(e)
				End If
			Next i
		 End Sub

		''' <summary>
		''' Adds a listener for <code>TreeSelection</code> events.
		''' </summary>
		''' <param name="tsl"> the <code>TreeSelectionListener</code> that will be notified
		'''            when a node is selected or deselected (a "negative
		'''            selection") </param>
		Public Overridable Sub addTreeSelectionListener(ByVal tsl As TreeSelectionListener)
			listenerList.add(GetType(TreeSelectionListener),tsl)
			If listenerList.getListenerCount(GetType(TreeSelectionListener)) <> 0 AndAlso selectionRedirector Is Nothing Then
				selectionRedirector = New TreeSelectionRedirector(Me)
				selectionModel.addTreeSelectionListener(selectionRedirector)
			End If
		End Sub

		''' <summary>
		''' Removes a <code>TreeSelection</code> listener.
		''' </summary>
		''' <param name="tsl"> the <code>TreeSelectionListener</code> to remove </param>
		Public Overridable Sub removeTreeSelectionListener(ByVal tsl As TreeSelectionListener)
			listenerList.remove(GetType(TreeSelectionListener),tsl)
			If listenerList.getListenerCount(GetType(TreeSelectionListener)) = 0 AndAlso selectionRedirector IsNot Nothing Then
				selectionModel.removeTreeSelectionListener(selectionRedirector)
				selectionRedirector = Nothing
			End If
		End Sub

		''' <summary>
		''' Returns an array of all the <code>TreeSelectionListener</code>s added
		''' to this JTree with addTreeSelectionListener().
		''' </summary>
		''' <returns> all of the <code>TreeSelectionListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		Public Overridable Property treeSelectionListeners As TreeSelectionListener()
			Get
				Return listenerList.getListeners(GetType(TreeSelectionListener))
			End Get
		End Property

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.
		''' </summary>
		''' <param name="e"> the <code>TreeSelectionEvent</code> to be fired;
		'''          generated by the
		'''          <code>TreeSelectionModel</code>
		'''          when a node is selected or deselected </param>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireValueChanged(ByVal e As TreeSelectionEvent)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				' TreeSelectionEvent e = null;
				If ___listeners(i) Is GetType(TreeSelectionListener) Then CType(___listeners(i+1), TreeSelectionListener).valueChanged(e)
			Next i
		End Sub

		''' <summary>
		''' Sent when the tree has changed enough that we need to resize
		''' the bounds, but not enough that we need to remove the
		''' expanded node set (e.g nodes were expanded or collapsed, or
		''' nodes were inserted into the tree). You should never have to
		''' invoke this, the UI will invoke this as it needs to.
		''' </summary>
		Public Overridable Sub treeDidChange()
			revalidate()
			repaint()
		End Sub

		''' <summary>
		''' Sets the number of rows that are to be displayed.
		''' This will only work if the tree is contained in a
		''' <code>JScrollPane</code>,
		''' and will adjust the preferred size and size of that scrollpane.
		''' <p>
		''' This is a bound property.
		''' </summary>
		''' <param name="newCount"> the number of rows to display
		''' @beaninfo
		'''        bound: true
		'''  description: The number of rows that are to be displayed. </param>
		Public Overridable Property visibleRowCount As Integer
			Set(ByVal newCount As Integer)
				Dim oldCount As Integer = visibleRowCount
    
				visibleRowCount = newCount
				firePropertyChange(VISIBLE_ROW_COUNT_PROPERTY, oldCount, visibleRowCount)
				invalidate()
				If accessibleContext IsNot Nothing Then CType(accessibleContext, AccessibleJTree).fireVisibleDataPropertyChange()
			End Set
			Get
				Return visibleRowCount
			End Get
		End Property


		''' <summary>
		''' Expands the root path, assuming the current TreeModel has been set.
		''' </summary>
		Private Sub expandRoot()
			Dim ___model As TreeModel = model

			If ___model IsNot Nothing AndAlso ___model.root IsNot Nothing Then expandPath(New TreePath(___model.root))
		End Sub

		''' <summary>
		''' Returns the TreePath to the next tree element that
		''' begins with a prefix. To handle the conversion of a
		''' <code>TreePath</code> into a String, <code>convertValueToText</code>
		''' is used.
		''' </summary>
		''' <param name="prefix"> the string to test for a match </param>
		''' <param name="startingRow"> the row for starting the search </param>
		''' <param name="bias"> the search direction, either
		''' Position.Bias.Forward or Position.Bias.Backward. </param>
		''' <returns> the TreePath of the next tree element that
		''' starts with the prefix; otherwise null </returns>
		''' <exception cref="IllegalArgumentException"> if prefix is null
		''' or startingRow is out of bounds
		''' @since 1.4 </exception>
		Public Overridable Function getNextMatch(ByVal prefix As String, ByVal startingRow As Integer, ByVal bias As javax.swing.text.Position.Bias) As TreePath

			Dim max As Integer = rowCount
			If prefix Is Nothing Then Throw New System.ArgumentException
			If startingRow < 0 OrElse startingRow >= max Then Throw New System.ArgumentException
			prefix = prefix.ToUpper()

			' start search from the next/previous element froom the
			' selected element
			Dim increment As Integer = If(bias Is javax.swing.text.Position.Bias.Forward, 1, -1)
			Dim row As Integer = startingRow
			Do
				Dim path As TreePath = getPathForRow(row)
				Dim text As String = convertValueToText(path.lastPathComponent, isRowSelected(row), isExpanded(row), True, row, False)

				If text.ToUpper().StartsWith(prefix) Then Return path
				row = (row + increment + max) Mod max
			Loop While row <> startingRow
			Return Nothing
		End Function

		' Serialization support.
		Private Sub writeObject(ByVal s As ObjectOutputStream)
			Dim values As New List(Of Object)

			s.defaultWriteObject()
			' Save the cellRenderer, if its Serializable.
			If cellRenderer IsNot Nothing AndAlso TypeOf cellRenderer Is Serializable Then
				values.Add("cellRenderer")
				values.Add(cellRenderer)
			End If
			' Save the cellEditor, if its Serializable.
			If cellEditor IsNot Nothing AndAlso TypeOf cellEditor Is Serializable Then
				values.Add("cellEditor")
				values.Add(cellEditor)
			End If
			' Save the treeModel, if its Serializable.
			If treeModel IsNot Nothing AndAlso TypeOf treeModel Is Serializable Then
				values.Add("treeModel")
				values.Add(treeModel)
			End If
			' Save the selectionModel, if its Serializable.
			If selectionModel IsNot Nothing AndAlso TypeOf selectionModel Is Serializable Then
				values.Add("selectionModel")
				values.Add(selectionModel)
			End If

			Dim expandedData As Object = archivableExpandedState

			If expandedData IsNot Nothing Then
				values.Add("expandedState")
				values.Add(expandedData)
			End If

			s.writeObject(values)
			If uIClassID.Equals(uiClassID) Then
				Dim count As SByte = JComponent.getWriteObjCounter(Me)
				count -= 1
				JComponent.writeObjCounterter(Me, count)
				If count = 0 AndAlso ui IsNot Nothing Then ui.installUI(Me)
			End If
		End Sub

		Private Sub readObject(ByVal s As ObjectInputStream)
			s.defaultReadObject()

			' Create an instance of expanded state.

			expandedState = New Dictionary(Of TreePath, Boolean?)

			expandedStack = New Stack(Of Stack(Of TreePath))

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim values As List(Of ?) = CType(s.readObject(), ArrayList)
			Dim indexCounter As Integer = 0
			Dim maxCounter As Integer = values.Count

			If indexCounter < maxCounter AndAlso values(indexCounter).Equals("cellRenderer") Then
				indexCounter += 1
				cellRenderer = CType(values(indexCounter), TreeCellRenderer)
				indexCounter += 1
			End If
			If indexCounter < maxCounter AndAlso values(indexCounter).Equals("cellEditor") Then
				indexCounter += 1
				cellEditor = CType(values(indexCounter), TreeCellEditor)
				indexCounter += 1
			End If
			If indexCounter < maxCounter AndAlso values(indexCounter).Equals("treeModel") Then
				indexCounter += 1
				treeModel = CType(values(indexCounter), TreeModel)
				indexCounter += 1
			End If
			If indexCounter < maxCounter AndAlso values(indexCounter).Equals("selectionModel") Then
				indexCounter += 1
				selectionModel = CType(values(indexCounter), TreeSelectionModel)
				indexCounter += 1
			End If
			If indexCounter < maxCounter AndAlso values(indexCounter).Equals("expandedState") Then
				indexCounter += 1
				unarchiveExpandedState(values(indexCounter))
				indexCounter += 1
			End If
			' Reinstall the redirector.
			If listenerList.getListenerCount(GetType(TreeSelectionListener)) <> 0 Then
				selectionRedirector = New TreeSelectionRedirector(Me)
				selectionModel.addTreeSelectionListener(selectionRedirector)
			End If
			' Listener to TreeModel.
			If treeModel IsNot Nothing Then
				treeModelListener = createTreeModelListener()
				If treeModelListener IsNot Nothing Then treeModel.addTreeModelListener(treeModelListener)
			End If
		End Sub

		''' <summary>
		''' Returns an object that can be archived indicating what nodes are
		''' expanded and what aren't. The objects from the model are NOT
		''' written out.
		''' </summary>
		Private Property archivableExpandedState As Object
			Get
				Dim ___model As TreeModel = model
    
				If ___model IsNot Nothing Then
					Dim paths As System.Collections.IEnumerator(Of TreePath) = expandedState.Keys.GetEnumerator()
    
					If paths IsNot Nothing Then
						Dim state As New List(Of Object)
    
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						Do While paths.hasMoreElements()
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
							Dim path As TreePath = paths.nextElement()
							Dim archivePath As Object
    
							Try
								archivePath = getModelIndexsForPath(path)
							Catch [error] As Exception
								archivePath = Nothing
							End Try
							If archivePath IsNot Nothing Then
								state.Add(archivePath)
								state.Add(expandedState(path))
							End If
						Loop
						Return state
					End If
				End If
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Updates the expanded state of nodes in the tree based on the
		''' previously archived state <code>state</code>.
		''' </summary>
		Private Sub unarchiveExpandedState(ByVal state As Object)
			If TypeOf state Is ArrayList Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim paths As List(Of ?) = CType(state, ArrayList)

				For counter As Integer = paths.Count - 1 To 0 Step -1
					Dim eState As Boolean? = CBool(paths(counter))
					counter -= 1
					Dim path As TreePath

					Try
						path = getPathForIndexs(CType(paths(counter), Integer()))
						If path IsNot Nothing Then expandedState(path) = eState
					Catch [error] As Exception
					End Try
				Next counter
			End If
		End Sub

		''' <summary>
		''' Returns an array of integers specifying the indexs of the
		''' components in the <code>path</code>. If <code>path</code> is
		''' the root, this will return an empty array.  If <code>path</code>
		''' is <code>null</code>, <code>null</code> will be returned.
		''' </summary>
		Private Function getModelIndexsForPath(ByVal path As TreePath) As Integer()
			If path IsNot Nothing Then
				Dim ___model As TreeModel = model
				Dim count As Integer = path.pathCount
				Dim indexs As Integer() = New Integer(count - 2){}
				Dim parent As Object = ___model.root

				For counter As Integer = 1 To count - 1
					indexs(counter - 1) = ___model.getIndexOfChild(parent, path.getPathComponent(counter))
					parent = path.getPathComponent(counter)
					If indexs(counter - 1) < 0 Then Return Nothing
				Next counter
				Return indexs
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Returns a <code>TreePath</code> created by obtaining the children
		''' for each of the indices in <code>indexs</code>. If <code>indexs</code>
		''' or the <code>TreeModel</code> is <code>null</code>, it will return
		''' <code>null</code>.
		''' </summary>
		Private Function getPathForIndexs(ByVal indexs As Integer()) As TreePath
			If indexs Is Nothing Then Return Nothing

			Dim ___model As TreeModel = model

			If ___model Is Nothing Then Return Nothing

			Dim count As Integer = indexs.Length
			Dim parent As Object = ___model.root
			If parent Is Nothing Then Return Nothing

			Dim parentPath As New TreePath(parent)

			For counter As Integer = 0 To count - 1
				parent = ___model.getChild(parent, indexs(counter))
				If parent Is Nothing Then Return Nothing
				parentPath = parentPath.pathByAddingChild(parent)
			Next counter
			Return parentPath
		End Function

		''' <summary>
		''' <code>EmptySelectionModel</code> is a <code>TreeSelectionModel</code>
		''' that does not allow anything to be selected.
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
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend Class EmptySelectionModel
			Inherits DefaultTreeSelectionModel

			''' <summary>
			''' The single instance of {@code EmptySelectionModel}.
			''' </summary>
			Protected Friend Shared ReadOnly ___sharedInstance As New EmptySelectionModel

			''' <summary>
			''' Returns the single instance of {@code EmptySelectionModel}.
			''' </summary>
			''' <returns> single instance of {@code EmptySelectionModel} </returns>
			Public Shared Function sharedInstance() As EmptySelectionModel
				Return ___sharedInstance
			End Function

			''' <summary>
			''' This is overriden to do nothing; {@code EmptySelectionModel}
			''' does not allow a selection.
			''' </summary>
			''' <param name="paths"> the paths to select; this is ignored </param>
			Public Overrides Property selectionPaths As TreePath()
				Set(ByVal paths As TreePath())
				End Set
			End Property

			''' <summary>
			''' This is overriden to do nothing; {@code EmptySelectionModel}
			''' does not allow a selection.
			''' </summary>
			''' <param name="paths"> the paths to add to the selection; this is ignored </param>
			Public Overrides Sub addSelectionPaths(ByVal paths As TreePath())
			End Sub

			''' <summary>
			''' This is overriden to do nothing; {@code EmptySelectionModel}
			''' does not allow a selection.
			''' </summary>
			''' <param name="paths"> the paths to remove; this is ignored </param>
			Public Overrides Sub removeSelectionPaths(ByVal paths As TreePath())
			End Sub

			''' <summary>
			''' This is overriden to do nothing; {@code EmptySelectionModel}
			''' does not allow a selection.
			''' </summary>
			''' <param name="mode"> the selection mode; this is ignored
			''' @since 1.7 </param>
			Public Overrides Property selectionMode As Integer
				Set(ByVal mode As Integer)
				End Set
			End Property

			''' <summary>
			''' This is overriden to do nothing; {@code EmptySelectionModel}
			''' does not allow a selection.
			''' </summary>
			''' <param name="mapper"> the {@code RowMapper} instance; this is ignored
			''' @since 1.7 </param>
			Public Overrides Property rowMapper As RowMapper
				Set(ByVal mapper As RowMapper)
				End Set
			End Property

			''' <summary>
			''' This is overriden to do nothing; {@code EmptySelectionModel}
			''' does not allow a selection.
			''' </summary>
			''' <param name="listener"> the listener to add; this is ignored
			''' @since 1.7 </param>
			Public Overrides Sub addTreeSelectionListener(ByVal listener As TreeSelectionListener)
			End Sub

			''' <summary>
			''' This is overriden to do nothing; {@code EmptySelectionModel}
			''' does not allow a selection.
			''' </summary>
			''' <param name="listener"> the listener to remove; this is ignored
			''' @since 1.7 </param>
			Public Overrides Sub removeTreeSelectionListener(ByVal listener As TreeSelectionListener)
			End Sub

			''' <summary>
			''' This is overriden to do nothing; {@code EmptySelectionModel}
			''' does not allow a selection.
			''' </summary>
			''' <param name="listener"> the listener to add; this is ignored
			''' @since 1.7 </param>
			Public Overridable Sub addPropertyChangeListener(ByVal listener As PropertyChangeListener)
			End Sub

			''' <summary>
			''' This is overriden to do nothing; {@code EmptySelectionModel}
			''' does not allow a selection.
			''' </summary>
			''' <param name="listener"> the listener to remove; this is ignored
			''' @since 1.7 </param>
			Public Overridable Sub removePropertyChangeListener(ByVal listener As PropertyChangeListener)
			End Sub
		End Class


		''' <summary>
		''' Handles creating a new <code>TreeSelectionEvent</code> with the
		''' <code>JTree</code> as the
		''' source and passing it off to all the listeners.
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
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		<Serializable> _
		Protected Friend Class TreeSelectionRedirector
			Implements TreeSelectionListener

			Private ReadOnly outerInstance As JTree

			Public Sub New(ByVal outerInstance As JTree)
				Me.outerInstance = outerInstance
			End Sub

			''' <summary>
			''' Invoked by the <code>TreeSelectionModel</code> when the
			''' selection changes.
			''' </summary>
			''' <param name="e"> the <code>TreeSelectionEvent</code> generated by the
			'''              <code>TreeSelectionModel</code> </param>
			Public Overridable Sub valueChanged(ByVal e As TreeSelectionEvent) Implements TreeSelectionListener.valueChanged
				Dim newE As TreeSelectionEvent

				newE = CType(e.cloneWithSource(JTree.this), TreeSelectionEvent)
				outerInstance.fireValueChanged(newE)
			End Sub
		End Class ' End of class JTree.TreeSelectionRedirector

		'
		' Scrollable interface
		'

		''' <summary>
		''' Returns the preferred display size of a <code>JTree</code>. The height is
		''' determined from <code>getVisibleRowCount</code> and the width
		''' is the current preferred width.
		''' </summary>
		''' <returns> a <code>Dimension</code> object containing the preferred size </returns>
		Public Overridable Property preferredScrollableViewportSize As Dimension
			Get
				Dim ___width As Integer = preferredSize.width
				Dim visRows As Integer = visibleRowCount
				Dim ___height As Integer = -1
    
				If fixedRowHeight Then
					___height = visRows * rowHeight
				Else
					Dim ___ui As TreeUI = uI
    
					If ___ui IsNot Nothing AndAlso visRows > 0 Then
						Dim rc As Integer = ___ui.getRowCount(Me)
    
						If rc >= visRows Then
							Dim ___bounds As Rectangle = getRowBounds(visRows - 1)
							If ___bounds IsNot Nothing Then ___height = ___bounds.y + ___bounds.height
						ElseIf rc > 0 Then
							Dim ___bounds As Rectangle = getRowBounds(0)
							If ___bounds IsNot Nothing Then ___height = ___bounds.height * visRows
						End If
					End If
					If ___height = -1 Then ___height = 16 * visRows
				End If
				Return New Dimension(___width, ___height)
			End Get
		End Property

		''' <summary>
		''' Returns the amount to increment when scrolling. The amount is
		''' the height of the first displayed row that isn't completely in view
		''' or, if it is totally displayed, the height of the next row in the
		''' scrolling direction.
		''' </summary>
		''' <param name="visibleRect"> the view area visible within the viewport </param>
		''' <param name="orientation"> either <code>SwingConstants.VERTICAL</code>
		'''          or <code>SwingConstants.HORIZONTAL</code> </param>
		''' <param name="direction"> less than zero to scroll up/left,
		'''          greater than zero for down/right </param>
		''' <returns> the "unit" increment for scrolling in the specified direction </returns>
		''' <seealso cref= JScrollBar#setUnitIncrement(int) </seealso>
		Public Overridable Function getScrollableUnitIncrement(ByVal visibleRect As Rectangle, ByVal orientation As Integer, ByVal direction As Integer) As Integer
			If orientation = SwingConstants.VERTICAL Then
				Dim ___rowBounds As Rectangle
				Dim firstIndex As Integer = getClosestRowForLocation(0, visibleRect.y)

				If firstIndex <> -1 Then
					___rowBounds = getRowBounds(firstIndex)
					If ___rowBounds.y <> visibleRect.y Then
						If direction < 0 Then Return Math.Max(0, (visibleRect.y - ___rowBounds.y))
						Return (___rowBounds.y + ___rowBounds.height - visibleRect.y)
					End If
					If direction < 0 Then ' UP
						If firstIndex <> 0 Then
							___rowBounds = getRowBounds(firstIndex - 1)
							Return ___rowBounds.height
						End If
					Else
						Return ___rowBounds.height
					End If
				End If
				Return 0
			End If
			Return 4
		End Function


		''' <summary>
		''' Returns the amount for a block increment, which is the height or
		''' width of <code>visibleRect</code>, based on <code>orientation</code>.
		''' </summary>
		''' <param name="visibleRect"> the view area visible within the viewport </param>
		''' <param name="orientation"> either <code>SwingConstants.VERTICAL</code>
		'''          or <code>SwingConstants.HORIZONTAL</code> </param>
		''' <param name="direction"> less than zero to scroll up/left,
		'''          greater than zero for down/right. </param>
		''' <returns> the "block" increment for scrolling in the specified direction </returns>
		''' <seealso cref= JScrollBar#setBlockIncrement(int) </seealso>
		Public Overridable Function getScrollableBlockIncrement(ByVal visibleRect As Rectangle, ByVal orientation As Integer, ByVal direction As Integer) As Integer
			Return If(orientation = SwingConstants.VERTICAL, visibleRect.height, visibleRect.width)
		End Function

		''' <summary>
		''' Returns false to indicate that the width of the viewport does not
		''' determine the width of the table, unless the preferred width of
		''' the tree is smaller than the viewports width.  In other words:
		''' ensure that the tree is never smaller than its viewport.
		''' </summary>
		''' <returns> whether the tree should track the width of the viewport </returns>
		''' <seealso cref= Scrollable#getScrollableTracksViewportWidth </seealso>
		Public Overridable Property scrollableTracksViewportWidth As Boolean Implements Scrollable.getScrollableTracksViewportWidth
			Get
				Dim parent As Container = SwingUtilities.getUnwrappedParent(Me)
				If TypeOf parent Is JViewport Then Return parent.width > preferredSize.width
				Return False
			End Get
		End Property

		''' <summary>
		''' Returns false to indicate that the height of the viewport does not
		''' determine the height of the table, unless the preferred height
		''' of the tree is smaller than the viewports height.  In other words:
		''' ensure that the tree is never smaller than its viewport.
		''' </summary>
		''' <returns> whether the tree should track the height of the viewport </returns>
		''' <seealso cref= Scrollable#getScrollableTracksViewportHeight </seealso>
		Public Overridable Property scrollableTracksViewportHeight As Boolean Implements Scrollable.getScrollableTracksViewportHeight
			Get
				Dim parent As Container = SwingUtilities.getUnwrappedParent(Me)
				If TypeOf parent Is JViewport Then Return parent.height > preferredSize.height
				Return False
			End Get
		End Property

		''' <summary>
		''' Sets the expanded state of this <code>JTree</code>.
		''' If <code>state</code> is
		''' true, all parents of <code>path</code> and path are marked as
		''' expanded. If <code>state</code> is false, all parents of
		''' <code>path</code> are marked EXPANDED, but <code>path</code> itself
		''' is marked collapsed.<p>
		''' This will fail if a <code>TreeWillExpandListener</code> vetos it.
		''' </summary>
		Protected Friend Overridable Sub setExpandedState(ByVal path As TreePath, ByVal state As Boolean)
			If path IsNot Nothing Then
				' Make sure all parents of path are expanded.
				Dim stack As Stack(Of TreePath)
				Dim parentPath As TreePath = path.parentPath

				If expandedStack.Count = 0 Then
					stack = New Stack(Of TreePath)
				Else
					stack = expandedStack.Pop()
				End If

				Try
					Do While parentPath IsNot Nothing
						If isExpanded(parentPath) Then
							parentPath = Nothing
						Else
							stack.Push(parentPath)
							parentPath = parentPath.parentPath
						End If
					Loop
					For counter As Integer = stack.Count - 1 To 0 Step -1
						parentPath = stack.Pop()
						If Not isExpanded(parentPath) Then
							Try
								fireTreeWillExpand(parentPath)
							Catch eve As ExpandVetoException
								' Expand vetoed!
								Return
							End Try
							expandedState(parentPath) = Boolean.TRUE
							fireTreeExpanded(parentPath)
							If accessibleContext IsNot Nothing Then CType(accessibleContext, AccessibleJTree).fireVisibleDataPropertyChange()
						End If
					Next counter
				Finally
					If expandedStack.Count < TEMP_STACK_SIZE Then
						stack.removeAllElements()
						expandedStack.Push(stack)
					End If
				End Try
				If Not state Then
					' collapse last path.
					Dim cValue As Object = expandedState(path)

					If cValue IsNot Nothing AndAlso CBool(cValue) Then
						Try
							fireTreeWillCollapse(path)
						Catch eve As ExpandVetoException
							Return
						End Try
						expandedState(path) = Boolean.FALSE
						fireTreeCollapsed(path)
						If removeDescendantSelectedPaths(path, False) AndAlso (Not isPathSelected(path)) Then addSelectionPath(path)
						If accessibleContext IsNot Nothing Then CType(accessibleContext, AccessibleJTree).fireVisibleDataPropertyChange()
					End If
				Else
					' Expand last path.
					Dim cValue As Object = expandedState(path)

					If cValue Is Nothing OrElse (Not CBool(cValue)) Then
						Try
							fireTreeWillExpand(path)
						Catch eve As ExpandVetoException
							Return
						End Try
						expandedState(path) = Boolean.TRUE
						fireTreeExpanded(path)
						If accessibleContext IsNot Nothing Then CType(accessibleContext, AccessibleJTree).fireVisibleDataPropertyChange()
					End If
				End If
			End If
		End Sub

		''' <summary>
		''' Returns an <code>Enumeration</code> of <code>TreePaths</code>
		''' that have been expanded that
		''' are descendants of <code>parent</code>.
		''' </summary>
		Protected Friend Overridable Function getDescendantToggledPaths(ByVal parent As TreePath) As System.Collections.IEnumerator(Of TreePath)
			If parent Is Nothing Then Return Nothing

			Dim descendants As New List(Of TreePath)
			Dim nodes As System.Collections.IEnumerator(Of TreePath) = expandedState.Keys.GetEnumerator()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While nodes.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim path As TreePath = nodes.nextElement()
				If parent.isDescendant(path) Then descendants.Add(path)
			Loop
			Return descendants.elements()
		End Function

		''' <summary>
		''' Removes any descendants of the <code>TreePaths</code> in
		''' <code>toRemove</code>
		''' that have been expanded.
		''' </summary>
		''' <param name="toRemove"> an enumeration of the paths to remove; a value of
		'''        {@code null} is ignored </param>
		''' <exception cref="ClassCastException"> if {@code toRemove} contains an
		'''         element that is not a {@code TreePath}; {@code null}
		'''         values are ignored </exception>
		 Protected Friend Overridable Sub removeDescendantToggledPaths(ByVal toRemove As System.Collections.IEnumerator(Of TreePath))
			 If toRemove IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				 Do While toRemove.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					 Dim descendants As System.Collections.IEnumerator(Of ?) = getDescendantToggledPaths(toRemove.nextElement())

					 If descendants IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						 Do While descendants.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
							 expandedState.Remove(descendants.nextElement())
						 Loop
					 End If
				 Loop
			 End If
		 End Sub

		 ''' <summary>
		 ''' Clears the cache of toggled tree paths. This does NOT send out
		 ''' any <code>TreeExpansionListener</code> events.
		 ''' </summary>
		 Protected Friend Overridable Sub clearToggledPaths()
			 expandedState.Clear()
		 End Sub

		 ''' <summary>
		 ''' Creates and returns an instance of <code>TreeModelHandler</code>.
		 ''' The returned
		 ''' object is responsible for updating the expanded state when the
		 ''' <code>TreeModel</code> changes.
		 ''' <p>
		 ''' For more information on what expanded state means, see the
		 ''' <a href=#jtree_description>JTree description</a> above.
		 ''' </summary>
		 Protected Friend Overridable Function createTreeModelListener() As TreeModelListener
			 Return New TreeModelHandler(Me)
		 End Function

		''' <summary>
		''' Removes any paths in the selection that are descendants of
		''' <code>path</code>. If <code>includePath</code> is true and
		''' <code>path</code> is selected, it will be removed from the selection.
		''' </summary>
		''' <returns> true if a descendant was selected
		''' @since 1.3 </returns>
		Protected Friend Overridable Function removeDescendantSelectedPaths(ByVal path As TreePath, ByVal includePath As Boolean) As Boolean
			Dim toRemove As TreePath() = getDescendantSelectedPaths(path, includePath)

			If toRemove IsNot Nothing Then
				selectionModel.removeSelectionPaths(toRemove)
				Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Returns an array of paths in the selection that are descendants of
		''' <code>path</code>. The returned array may contain <code>null</code>s.
		''' </summary>
		Private Function getDescendantSelectedPaths(ByVal path As TreePath, ByVal includePath As Boolean) As TreePath()
			Dim sm As TreeSelectionModel = selectionModel
			Dim selPaths As TreePath() = If(sm IsNot Nothing, sm.selectionPaths, Nothing)

			If selPaths IsNot Nothing Then
				Dim shouldRemove As Boolean = False

				For counter As Integer = selPaths.Length - 1 To 0 Step -1
					If selPaths(counter) IsNot Nothing AndAlso path.isDescendant(selPaths(counter)) AndAlso ((Not path.Equals(selPaths(counter))) OrElse includePath) Then
						shouldRemove = True
					Else
						selPaths(counter) = Nothing
					End If
				Next counter
				If Not shouldRemove Then selPaths = Nothing
				Return selPaths
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Removes any paths from the selection model that are descendants of
		''' the nodes identified by in <code>e</code>.
		''' </summary>
		Friend Overridable Sub removeDescendantSelectedPaths(ByVal e As TreeModelEvent)
			Dim pPath As TreePath = sun.swing.SwingUtilities2.getTreePath(e, model)
			Dim oldChildren As Object() = e.children
			Dim sm As TreeSelectionModel = selectionModel

			If sm IsNot Nothing AndAlso pPath IsNot Nothing AndAlso oldChildren IsNot Nothing AndAlso oldChildren.Length > 0 Then
				For counter As Integer = oldChildren.Length - 1 To 0 Step -1
					' Might be better to call getDescendantSelectedPaths
					' numerous times, then push to the model.
					removeDescendantSelectedPaths(pPath.pathByAddingChild(oldChildren(counter)), True)
				Next counter
			End If
		End Sub


		 ''' <summary>
		 ''' Listens to the model and updates the <code>expandedState</code>
		 ''' accordingly when nodes are removed, or changed.
		 ''' </summary>
		Protected Friend Class TreeModelHandler
			Implements TreeModelListener

			Private ReadOnly outerInstance As JTree

			Public Sub New(ByVal outerInstance As JTree)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub treeNodesChanged(ByVal e As TreeModelEvent) Implements TreeModelListener.treeNodesChanged
			End Sub

			Public Overridable Sub treeNodesInserted(ByVal e As TreeModelEvent) Implements TreeModelListener.treeNodesInserted
			End Sub

			Public Overridable Sub treeStructureChanged(ByVal e As TreeModelEvent) Implements TreeModelListener.treeStructureChanged
				If e Is Nothing Then Return

				' NOTE: If I change this to NOT remove the descendants
				' and update BasicTreeUIs treeStructureChanged method
				' to update descendants in response to a treeStructureChanged
				' event, all the children of the event won't collapse!
				Dim parent As TreePath = sun.swing.SwingUtilities2.getTreePath(e, outerInstance.model)

				If parent Is Nothing Then Return

				If parent.pathCount = 1 Then
					' New root, remove everything!
					outerInstance.clearToggledPaths()
					Dim treeRoot As Object = outerInstance.treeModel.root
					If treeRoot IsNot Nothing AndAlso (Not outerInstance.treeModel.isLeaf(treeRoot)) Then outerInstance.expandedState(parent) = Boolean.TRUE
				ElseIf outerInstance.expandedState(parent) IsNot Nothing Then
					Dim toRemove As New List(Of TreePath)(1)
					Dim isExpanded As Boolean = outerInstance.isExpanded(parent)

					toRemove.Add(parent)
					outerInstance.removeDescendantToggledPaths(toRemove.elements())
					If isExpanded Then
						Dim model As TreeModel = outerInstance.model

						If model Is Nothing OrElse model.isLeaf(parent.lastPathComponent) Then
							outerInstance.collapsePath(parent)
						Else
							outerInstance.expandedState(parent) = Boolean.TRUE
						End If
					End If
				End If
				outerInstance.removeDescendantSelectedPaths(parent, False)
			End Sub

			Public Overridable Sub treeNodesRemoved(ByVal e As TreeModelEvent) Implements TreeModelListener.treeNodesRemoved
				If e Is Nothing Then Return

				Dim parent As TreePath = sun.swing.SwingUtilities2.getTreePath(e, outerInstance.model)
				Dim children As Object() = e.children

				If children Is Nothing Then Return

				Dim rPath As TreePath
				Dim toRemove As New List(Of TreePath)(Math.Max(1, children.Length))

				For counter As Integer = children.Length - 1 To 0 Step -1
					rPath = parent.pathByAddingChild(children(counter))
					If outerInstance.expandedState(rPath) IsNot Nothing Then toRemove.Add(rPath)
				Next counter
				If toRemove.Count > 0 Then outerInstance.removeDescendantToggledPaths(toRemove.elements())

				Dim model As TreeModel = outerInstance.model

				If model Is Nothing OrElse model.isLeaf(parent.lastPathComponent) Then outerInstance.expandedState.Remove(parent)

				outerInstance.removeDescendantSelectedPaths(e)
			End Sub
		End Class


		''' <summary>
		''' <code>DynamicUtilTreeNode</code> can wrap
		''' vectors/hashtables/arrays/strings and
		''' create the appropriate children tree nodes as necessary. It is
		''' dynamic in that it will only create the children as necessary.
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
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Class DynamicUtilTreeNode
			Inherits DefaultMutableTreeNode

			''' <summary>
			''' Does the this <code>JTree</code> have children?
			''' This property is currently not implemented.
			''' </summary>
			Protected Friend hasChildren As Boolean
			''' <summary>
			''' Value to create children with. </summary>
			Protected Friend childValue As Object
			''' <summary>
			''' Have the children been loaded yet? </summary>
			Protected Friend loadedChildren As Boolean

			''' <summary>
			''' Adds to parent all the children in <code>children</code>.
			''' If <code>children</code> is an array or vector all of its
			''' elements are added is children, otherwise if <code>children</code>
			''' is a hashtable all the key/value pairs are added in the order
			''' <code>Enumeration</code> returns them.
			''' </summary>
			Public Shared Sub createChildren(ByVal parent As DefaultMutableTreeNode, ByVal children As Object)
				If TypeOf children Is ArrayList Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim childVector As List(Of ?) = CType(children, ArrayList)

					Dim counter As Integer = 0
					Dim maxCounter As Integer = childVector.Count
					Do While counter < maxCounter
						parent.add(New DynamicUtilTreeNode(childVector(counter), childVector(counter)))
						counter += 1
					Loop
				ElseIf TypeOf children Is Hashtable Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim childHT As Dictionary(Of ?, ?) = CType(children, Hashtable)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim keys As System.Collections.IEnumerator(Of ?) = childHT.Keys.GetEnumerator()
					Dim aKey As Object

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Do While keys.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						aKey = keys.nextElement()
						parent.add(New DynamicUtilTreeNode(aKey, childHT(aKey)))
					Loop
				ElseIf TypeOf children Is Object() Then
					Dim childArray As Object() = CType(children, Object())

					Dim counter As Integer = 0
					Dim maxCounter As Integer = childArray.Length
					Do While counter < maxCounter
						parent.add(New DynamicUtilTreeNode(childArray(counter), childArray(counter)))
						counter += 1
					Loop
				End If
			End Sub

			''' <summary>
			''' Creates a node with the specified object as its value and
			''' with the specified children. For the node to allow children,
			''' the children-object must be an array of objects, a
			''' <code>Vector</code>, or a <code>Hashtable</code> -- even
			''' if empty. Otherwise, the node is not
			''' allowed to have children.
			''' </summary>
			''' <param name="value">  the <code>Object</code> that is the value for the
			'''              new node </param>
			''' <param name="children"> an array of <code>Object</code>s, a
			'''              <code>Vector</code>, or a <code>Hashtable</code>
			'''              used to create the child nodes; if any other
			'''              object is specified, or if the value is
			'''              <code>null</code>,
			'''              then the node is not allowed to have children </param>
			Public Sub New(ByVal value As Object, ByVal children As Object)
				MyBase.New(value)
				loadedChildren = False
				childValue = children
				If children IsNot Nothing Then
					If TypeOf children Is ArrayList Then
						allowsChildren = True
					ElseIf TypeOf children Is Hashtable Then
						allowsChildren = True
					ElseIf TypeOf children Is Object() Then
						allowsChildren = True
					Else
						allowsChildren = False
					End If
				Else
					allowsChildren = False
				End If
			End Sub

			''' <summary>
			''' Returns true if this node allows children. Whether the node
			''' allows children depends on how it was created.
			''' </summary>
			''' <returns> true if this node allows children, false otherwise </returns>
			''' <seealso cref= JTree.DynamicUtilTreeNode </seealso>
			Public Property Overrides leaf As Boolean
				Get
					Return Not allowsChildren
				End Get
			End Property

			''' <summary>
			''' Returns the number of child nodes.
			''' </summary>
			''' <returns> the number of child nodes </returns>
			Public Property Overrides childCount As Integer
				Get
					If Not loadedChildren Then loadChildren()
					Return MyBase.childCount
				End Get
			End Property

			''' <summary>
			''' Loads the children based on <code>childValue</code>.
			''' If <code>childValue</code> is a <code>Vector</code>
			''' or array each element is added as a child,
			''' if <code>childValue</code> is a <code>Hashtable</code>
			''' each key/value pair is added in the order that
			''' <code>Enumeration</code> returns the keys.
			''' </summary>
			Protected Friend Overridable Sub loadChildren()
				loadedChildren = True
				createChildren(Me, childValue)
			End Sub

			''' <summary>
			''' Subclassed to load the children, if necessary.
			''' </summary>
			Public Overrides Function getChildAt(ByVal index As Integer) As TreeNode
				If Not loadedChildren Then loadChildren()
				Return MyBase.getChildAt(index)
			End Function

			''' <summary>
			''' Subclassed to load the children, if necessary.
			''' </summary>
			Public Overrides Function children() As System.Collections.IEnumerator
				If Not loadedChildren Then loadChildren()
				Return MyBase.children()
			End Function
		End Class

		Friend Overrides Sub setUIProperty(ByVal propertyName As String, ByVal value As Object)
			If propertyName = "rowHeight" Then
				If Not rowHeightSet Then
					rowHeight = CType(value, Number)
					rowHeightSet = False
				End If
			ElseIf propertyName = "scrollsOnExpand" Then
				If Not scrollsOnExpandSet Then
					scrollsOnExpand = CBool(value)
					scrollsOnExpandSet = False
				End If
			ElseIf propertyName = "showsRootHandles" Then
				If Not showsRootHandlesSet Then
					showsRootHandles = CBool(value)
					showsRootHandlesSet = False
				End If
			Else
				MyBase.uIPropertyrty(propertyName, value)
			End If
		End Sub


		''' <summary>
		''' Returns a string representation of this <code>JTree</code>.
		''' This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JTree</code>. </returns>
		Protected Friend Overrides Function paramString() As String
			Dim rootVisibleString As String = (If(rootVisible, "true", "false"))
			Dim showsRootHandlesString As String = (If(showsRootHandles, "true", "false"))
			Dim editableString As String = (If(editable, "true", "false"))
			Dim largeModelString As String = (If(largeModel, "true", "false"))
			Dim invokesStopCellEditingString As String = (If(invokesStopCellEditing, "true", "false"))
			Dim scrollsOnExpandString As String = (If(scrollsOnExpand, "true", "false"))

			Return MyBase.paramString() & ",editable=" & editableString & ",invokesStopCellEditing=" & invokesStopCellEditingString & ",largeModel=" & largeModelString & ",rootVisible=" & rootVisibleString & ",rowHeight=" & rowHeight & ",scrollsOnExpand=" & scrollsOnExpandString & ",showsRootHandles=" & showsRootHandlesString & ",toggleClickCount=" & toggleClickCount & ",visibleRowCount=" & visibleRowCount
		End Function

	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this JTree.
		''' For JTrees, the AccessibleContext takes the form of an
		''' AccessibleJTree.
		''' A new AccessibleJTree instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJTree that serves as the
		'''         AccessibleContext of this JTree </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJTree(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JTree</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to tree user-interface elements.
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
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend Class AccessibleJTree
			Inherits AccessibleJComponent
			Implements AccessibleSelection, TreeSelectionListener, TreeModelListener, TreeExpansionListener

			Private ReadOnly outerInstance As JTree


			Friend leadSelectionPath As TreePath
			Friend leadSelectionAccessible As Accessible

			Public Sub New(ByVal outerInstance As JTree)
					Me.outerInstance = outerInstance
				' Add a tree model listener for JTree
				Dim model As TreeModel = outerInstance.model
				If model IsNot Nothing Then model.addTreeModelListener(Me)
				outerInstance.addTreeExpansionListener(Me)
				outerInstance.addTreeSelectionListener(Me)
				leadSelectionPath = outerInstance.leadSelectionPath
				leadSelectionAccessible = If(leadSelectionPath IsNot Nothing, New AccessibleJTreeNode(Me, JTree.this, leadSelectionPath, JTree.this), Nothing)
			End Sub

			''' <summary>
			''' Tree Selection Listener value change method. Used to fire the
			''' property change
			''' </summary>
			''' <param name="e"> ListSelectionEvent
			'''  </param>
			Public Overridable Sub valueChanged(ByVal e As TreeSelectionEvent) Implements TreeSelectionListener.valueChanged
				outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_SELECTION_PROPERTY, Convert.ToBoolean(False), Convert.ToBoolean(True))
			End Sub

			''' <summary>
			''' Fire a visible data property change notification.
			''' A 'visible' data property is one that represents
			''' something about the way the component appears on the
			''' display, where that appearance isn't bound to any other
			''' property. It notifies screen readers  that the visual
			''' appearance of the component has changed, so they can
			''' notify the user.
			''' </summary>
			Public Overridable Sub fireVisibleDataPropertyChange()
			   outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, Convert.ToBoolean(False), Convert.ToBoolean(True))
			End Sub

			' Fire the visible data changes for the model changes.

			''' <summary>
			''' Tree Model Node change notification.
			''' </summary>
			''' <param name="e">  a Tree Model event </param>
			Public Overridable Sub treeNodesChanged(ByVal e As TreeModelEvent) Implements TreeModelListener.treeNodesChanged
			   fireVisibleDataPropertyChange()
			End Sub

			''' <summary>
			''' Tree Model Node change notification.
			''' </summary>
			''' <param name="e">  a Tree node insertion event </param>
			Public Overridable Sub treeNodesInserted(ByVal e As TreeModelEvent) Implements TreeModelListener.treeNodesInserted
			   fireVisibleDataPropertyChange()
			End Sub

			''' <summary>
			''' Tree Model Node change notification.
			''' </summary>
			''' <param name="e">  a Tree node(s) removal event </param>
			Public Overridable Sub treeNodesRemoved(ByVal e As TreeModelEvent) Implements TreeModelListener.treeNodesRemoved
			   fireVisibleDataPropertyChange()
			End Sub

			''' <summary>
			''' Tree Model structure change change notification.
			''' </summary>
			''' <param name="e">  a Tree Model event </param>
			Public Overridable Sub treeStructureChanged(ByVal e As TreeModelEvent) Implements TreeModelListener.treeStructureChanged
			   fireVisibleDataPropertyChange()
			End Sub

			''' <summary>
			''' Tree Collapsed notification.
			''' </summary>
			''' <param name="e">  a TreeExpansionEvent </param>
			Public Overridable Sub treeCollapsed(ByVal e As TreeExpansionEvent) Implements TreeExpansionListener.treeCollapsed
				fireVisibleDataPropertyChange()
				Dim path As TreePath = e.path
				If path IsNot Nothing Then
					' Set parent to null so AccessibleJTreeNode computes
					' its parent.
					Dim node As New AccessibleJTreeNode(Me, JTree.this, path, Nothing)
					Dim pce As New PropertyChangeEvent(node, AccessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.EXPANDED, AccessibleState.COLLAPSED)
					outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, pce)
				End If
			End Sub

			''' <summary>
			''' Tree Model Expansion notification.
			''' </summary>
			''' <param name="e">  a Tree node insertion event </param>
			Public Overridable Sub treeExpanded(ByVal e As TreeExpansionEvent) Implements TreeExpansionListener.treeExpanded
				fireVisibleDataPropertyChange()
				Dim path As TreePath = e.path
				If path IsNot Nothing Then
					' TIGER - 4839971
					' Set parent to null so AccessibleJTreeNode computes
					' its parent.
					Dim node As New AccessibleJTreeNode(Me, JTree.this, path, Nothing)
					Dim pce As New PropertyChangeEvent(node, AccessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.COLLAPSED, AccessibleState.EXPANDED)
					outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, pce)
				End If
			End Sub

			''' <summary>
			'''  Fire an active descendant property change notification.
			'''  The active descendant is used for objects such as list,
			'''  tree, and table, which may have transient children.
			'''  It notifies screen readers the active child of the component
			'''  has been changed so user can be notified from there.
			''' </summary>
			''' <param name="oldPath"> - lead path of previous active child </param>
			''' <param name="newPath"> - lead path of current active child
			'''  </param>
			Friend Overridable Sub fireActiveDescendantPropertyChange(ByVal oldPath As TreePath, ByVal newPath As TreePath)
				If oldPath IsNot newPath Then
					Dim oldLSA As Accessible = If(oldPath IsNot Nothing, New AccessibleJTreeNode(Me, JTree.this, oldPath, Nothing), Nothing)

					Dim newLSA As Accessible = If(newPath IsNot Nothing, New AccessibleJTreeNode(Me, JTree.this, newPath, Nothing), Nothing)
					outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_ACTIVE_DESCENDANT_PROPERTY, oldLSA, newLSA)
				End If
			End Sub

			Private Property currentAccessibleContext As AccessibleContext
				Get
					Dim c As Component = currentComponent
					If TypeOf c Is Accessible Then
						Return c.accessibleContext
					Else
						Return Nothing
					End If
				End Get
			End Property

			Private Property currentComponent As Component
				Get
					' is the object visible?
					' if so, get row, selected, focus & leaf state,
					' and then get the renderer component and return it
					Dim model As TreeModel = outerInstance.model
					If model Is Nothing Then Return Nothing
					Dim treeRoot As Object = model.root
					If treeRoot Is Nothing Then Return Nothing
    
					Dim path As New TreePath(treeRoot)
					If outerInstance.isVisible(path) Then
						Dim r As TreeCellRenderer = outerInstance.cellRenderer
						Dim ui As TreeUI = outerInstance.uI
						If ui IsNot Nothing Then
							Dim row As Integer = ui.getRowForPath(JTree.this, path)
							Dim lsr As Integer = outerInstance.leadSelectionRow
							Dim hasFocus As Boolean = outerInstance.focusOwner AndAlso (lsr = row)
							Dim selected As Boolean = outerInstance.isPathSelected(path)
							Dim expanded As Boolean = outerInstance.isExpanded(path)
    
							Return r.getTreeCellRendererComponent(JTree.this, treeRoot, selected, expanded, model.isLeaf(treeRoot), row, hasFocus)
						End If
					End If
					Return Nothing
				End Get
			End Property

			' Overridden methods from AccessibleJComponent

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.TREE
				End Get
			End Property

			''' <summary>
			''' Returns the <code>Accessible</code> child, if one exists,
			''' contained at the local coordinate <code>Point</code>.
			''' Otherwise returns <code>null</code>.
			''' </summary>
			''' <param name="p"> point in local coordinates of this <code>Accessible</code> </param>
			''' <returns> the <code>Accessible</code>, if it exists,
			'''    at the specified location; else <code>null</code> </returns>
			Public Overridable Function getAccessibleAt(ByVal p As Point) As Accessible
				Dim path As TreePath = outerInstance.getClosestPathForLocation(p.x, p.y)
				If path IsNot Nothing Then
					' JTree.this is NOT the parent; parent will get computed later
					Return New AccessibleJTreeNode(Me, JTree.this, path, Nothing)
				Else
					Return Nothing
				End If
			End Function

			''' <summary>
			''' Returns the number of top-level children nodes of this
			''' JTree.  Each of these nodes may in turn have children nodes.
			''' </summary>
			''' <returns> the number of accessible children nodes in the tree. </returns>
			Public Overridable Property accessibleChildrenCount As Integer
				Get
					Dim model As TreeModel = outerInstance.model
					If model Is Nothing Then Return 0
					If outerInstance.rootVisible Then Return 1 ' the root node
    
					Dim treeRoot As Object = model.root
					If treeRoot Is Nothing Then Return 0
					' return the root's first set of children count
					Return model.getChildCount(treeRoot)
				End Get
			End Property

			''' <summary>
			''' Return the nth Accessible child of the object.
			''' </summary>
			''' <param name="i"> zero-based index of child </param>
			''' <returns> the nth Accessible child of the object </returns>
			Public Overridable Function getAccessibleChild(ByVal i As Integer) As Accessible
				Dim model As TreeModel = outerInstance.model
				If model Is Nothing Then Return Nothing

				Dim treeRoot As Object = model.root
				If treeRoot Is Nothing Then Return Nothing

				If outerInstance.rootVisible Then
					If i = 0 Then ' return the root node Accessible
						Dim objPath As Object() = { treeRoot }
						If objPath(0) Is Nothing Then Return Nothing
						Dim path As New TreePath(objPath)
						Return New AccessibleJTreeNode(Me, JTree.this, path, JTree.this)
					Else
						Return Nothing
					End If
				End If

				' return Accessible for one of root's child nodes
				Dim count As Integer = model.getChildCount(treeRoot)
				If i < 0 OrElse i >= count Then Return Nothing
				Dim obj As Object = model.getChild(treeRoot, i)
				If obj Is Nothing Then Return Nothing
				Dim objPath As Object() = { treeRoot, obj }
				Dim path As New TreePath(objPath)
				Return New AccessibleJTreeNode(Me, JTree.this, path, JTree.this)
			End Function

			''' <summary>
			''' Get the index of this object in its accessible parent.
			''' </summary>
			''' <returns> the index of this object in its parent.  Since a JTree
			''' top-level object does not have an accessible parent. </returns>
			''' <seealso cref= #getAccessibleParent </seealso>
			Public Overridable Property accessibleIndexInParent As Integer
				Get
					' didn't ever need to override this...
					Return MyBase.accessibleIndexInParent
				End Get
			End Property

			' AccessibleSelection methods
			''' <summary>
			''' Get the AccessibleSelection associated with this object.  In the
			''' implementation of the Java Accessibility API for this class,
			''' return this object, which is responsible for implementing the
			''' AccessibleSelection interface on behalf of itself.
			''' </summary>
			''' <returns> this object </returns>
			Public Overridable Property accessibleSelection As AccessibleSelection
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Returns the number of items currently selected.
			''' If no items are selected, the return value will be 0.
			''' </summary>
			''' <returns> the number of items currently selected. </returns>
			Public Overridable Property accessibleSelectionCount As Integer Implements AccessibleSelection.getAccessibleSelectionCount
				Get
					Dim rootPath As Object() = New Object(0){}
					rootPath(0) = outerInstance.treeModel.root
					If rootPath(0) Is Nothing Then Return 0
    
					Dim childPath As New TreePath(rootPath)
					If outerInstance.isPathSelected(childPath) Then
						Return 1
					Else
						Return 0
					End If
				End Get
			End Property

			''' <summary>
			''' Returns an Accessible representing the specified selected item
			''' in the object.  If there isn't a selection, or there are
			''' fewer items selected than the integer passed in, the return
			''' value will be null.
			''' </summary>
			''' <param name="i"> the zero-based index of selected items </param>
			''' <returns> an Accessible containing the selected item </returns>
			Public Overridable Function getAccessibleSelection(ByVal i As Integer) As Accessible Implements AccessibleSelection.getAccessibleSelection
				' The JTree can have only one accessible child, the root.
				If i = 0 Then
					Dim rootPath As Object() = New Object(0){}
					rootPath(0) = outerInstance.treeModel.root
					If rootPath(0) Is Nothing Then Return Nothing
					Dim childPath As New TreePath(rootPath)
					If outerInstance.isPathSelected(childPath) Then Return New AccessibleJTreeNode(Me, JTree.this, childPath, JTree.this)
				End If
				Return Nothing
			End Function

			''' <summary>
			''' Returns true if the current child of this object is selected.
			''' </summary>
			''' <param name="i"> the zero-based index of the child in this Accessible object. </param>
			''' <seealso cref= AccessibleContext#getAccessibleChild </seealso>
			Public Overridable Function isAccessibleChildSelected(ByVal i As Integer) As Boolean Implements AccessibleSelection.isAccessibleChildSelected
				' The JTree can have only one accessible child, the root.
				If i = 0 Then
					Dim rootPath As Object() = New Object(0){}
					rootPath(0) = outerInstance.treeModel.root
					If rootPath(0) Is Nothing Then Return False
					Dim childPath As New TreePath(rootPath)
					Return outerInstance.isPathSelected(childPath)
				Else
					Return False
				End If
			End Function

			''' <summary>
			''' Adds the specified selected item in the object to the object's
			''' selection.  If the object supports multiple selections,
			''' the specified item is added to any existing selection, otherwise
			''' it replaces any existing selection in the object.  If the
			''' specified item is already selected, this method has no effect.
			''' </summary>
			''' <param name="i"> the zero-based index of selectable items </param>
			Public Overridable Sub addAccessibleSelection(ByVal i As Integer) Implements AccessibleSelection.addAccessibleSelection
			   Dim model As TreeModel = outerInstance.model
			   If model IsNot Nothing Then
				   If i = 0 Then
					   Dim objPath As Object() = {model.root}
					   If objPath(0) Is Nothing Then Return
					   Dim path As New TreePath(objPath)
					   outerInstance.addSelectionPath(path)
				   End If
			   End If
			End Sub

			''' <summary>
			''' Removes the specified selected item in the object from the object's
			''' selection.  If the specified item isn't currently selected, this
			''' method has no effect.
			''' </summary>
			''' <param name="i"> the zero-based index of selectable items </param>
			Public Overridable Sub removeAccessibleSelection(ByVal i As Integer) Implements AccessibleSelection.removeAccessibleSelection
				Dim model As TreeModel = outerInstance.model
				If model IsNot Nothing Then
					If i = 0 Then
						Dim objPath As Object() = {model.root}
						If objPath(0) Is Nothing Then Return
						Dim path As New TreePath(objPath)
						outerInstance.removeSelectionPath(path)
					End If
				End If
			End Sub

			''' <summary>
			''' Clears the selection in the object, so that nothing in the
			''' object is selected.
			''' </summary>
			Public Overridable Sub clearAccessibleSelection() Implements AccessibleSelection.clearAccessibleSelection
				Dim childCount As Integer = accessibleChildrenCount
				For i As Integer = 0 To childCount - 1
					removeAccessibleSelection(i)
				Next i
			End Sub

			''' <summary>
			''' Causes every selected item in the object to be selected
			''' if the object supports multiple selections.
			''' </summary>
			Public Overridable Sub selectAllAccessibleSelection() Implements AccessibleSelection.selectAllAccessibleSelection
				Dim model As TreeModel = outerInstance.model
				If model IsNot Nothing Then
					Dim objPath As Object() = {model.root}
					If objPath(0) Is Nothing Then Return
					Dim path As New TreePath(objPath)
					outerInstance.addSelectionPath(path)
				End If
			End Sub

			''' <summary>
			''' This class implements accessibility support for the
			''' <code>JTree</code> child.  It provides an implementation of the
			''' Java Accessibility API appropriate to tree nodes.
			''' </summary>
			Protected Friend Class AccessibleJTreeNode
				Inherits AccessibleContext
				Implements Accessible, AccessibleComponent, AccessibleSelection, AccessibleAction

				Private ReadOnly outerInstance As JTree.AccessibleJTree


				Private tree As JTree = Nothing
				Private treeModel As TreeModel = Nothing
				Private obj As Object = Nothing
				Private path As TreePath = Nothing
				Private Shadows accessibleParent As Accessible = Nothing
				Private index As Integer = 0
				Private isLeaf As Boolean = False

				''' <summary>
				'''  Constructs an AccessibleJTreeNode
				''' @since 1.4
				''' </summary>
				Public Sub New(ByVal outerInstance As JTree.AccessibleJTree, ByVal t As JTree, ByVal p As TreePath, ByVal ap As Accessible)
						Me.outerInstance = outerInstance
					tree = t
					path = p
					accessibleParent = ap
					treeModel = t.model
					obj = p.lastPathComponent
					If treeModel IsNot Nothing Then isLeaf = treeModel.isLeaf(obj)
				End Sub

				Private Function getChildTreePath(ByVal i As Integer) As TreePath
					' Tree nodes can't be so complex that they have
					' two sets of children -> we're ignoring that case
					If i < 0 OrElse i >= accessibleChildrenCount Then
						Return Nothing
					Else
						Dim childObj As Object = treeModel.getChild(obj, i)
						Dim objPath As Object() = path.path
						Dim objChildPath As Object() = New Object(objPath.Length){}
						Array.Copy(objPath, 0, objChildPath, 0, objPath.Length)
						objChildPath(objChildPath.Length-1) = childObj
						Return New TreePath(objChildPath)
					End If
				End Function

				''' <summary>
				''' Get the AccessibleContext associated with this tree node.
				''' In the implementation of the Java Accessibility API for
				''' this class, return this object, which is its own
				''' AccessibleContext.
				''' </summary>
				''' <returns> this object </returns>
				Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
					Get
						Return Me
					End Get
				End Property

				Private Property currentAccessibleContext As AccessibleContext
					Get
						Dim c As Component = currentComponent
						If TypeOf c Is Accessible Then
							Return c.accessibleContext
						Else
							Return Nothing
						End If
					End Get
				End Property

				Private Property currentComponent As Component
					Get
						' is the object visible?
						' if so, get row, selected, focus & leaf state,
						' and then get the renderer component and return it
						If tree.isVisible(path) Then
							Dim r As TreeCellRenderer = tree.cellRenderer
							If r Is Nothing Then Return Nothing
							Dim ui As TreeUI = tree.uI
							If ui IsNot Nothing Then
								Dim row As Integer = ui.getRowForPath(JTree.this, path)
								Dim selected As Boolean = tree.isPathSelected(path)
								Dim expanded As Boolean = tree.isExpanded(path)
								Dim hasFocus As Boolean = False ' how to tell?? -PK
								Return r.getTreeCellRendererComponent(tree, obj, selected, expanded, isLeaf, row, hasFocus)
							End If
						End If
						Return Nothing
					End Get
				End Property

			' AccessibleContext methods

				 ''' <summary>
				 ''' Get the accessible name of this object.
				 ''' </summary>
				 ''' <returns> the localized name of the object; null if this
				 ''' object does not have a name </returns>
				 Public Property Overrides accessibleName As String
					 Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Dim name As String = ac.accessibleName
							If (name IsNot Nothing) AndAlso (name <> "") Then
								Return ac.accessibleName
							Else
								Return Nothing
							End If
						End If
						If (accessibleName IsNot Nothing) AndAlso (accessibleName <> "") Then
							Return accessibleName
						Else
							' fall back to the client property
							Return CStr(getClientProperty(AccessibleContext.ACCESSIBLE_NAME_PROPERTY))
						End If
					 End Get
					 Set(ByVal s As String)
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							ac.accessibleName = s
						Else
							MyBase.accessibleName = s
						End If
					End Set
				 End Property


				'
				' *** should check tooltip text for desc. (needs MouseEvent)
				'
				''' <summary>
				''' Get the accessible description of this object.
				''' </summary>
				''' <returns> the localized description of the object; null if
				''' this object does not have a description </returns>
				Public Property Overrides accessibleDescription As String
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Return ac.accessibleDescription
						Else
							Return MyBase.accessibleDescription
						End If
					End Get
					Set(ByVal s As String)
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							ac.accessibleDescription = s
						Else
							MyBase.accessibleDescription = s
						End If
					End Set
				End Property


				''' <summary>
				''' Get the role of this object.
				''' </summary>
				''' <returns> an instance of AccessibleRole describing the role of the object </returns>
				''' <seealso cref= AccessibleRole </seealso>
				Public Property Overrides accessibleRole As AccessibleRole
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Return ac.accessibleRole
						Else
							Return AccessibleRole.UNKNOWN
						End If
					End Get
				End Property

				''' <summary>
				''' Get the state set of this object.
				''' </summary>
				''' <returns> an instance of AccessibleStateSet containing the
				''' current state set of the object </returns>
				''' <seealso cref= AccessibleState </seealso>
				Public Property Overrides accessibleStateSet As AccessibleStateSet
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						Dim states As AccessibleStateSet
						If ac IsNot Nothing Then
							states = ac.accessibleStateSet
						Else
							states = New AccessibleStateSet
						End If
						' need to test here, 'cause the underlying component
						' is a cellRenderer, which is never showing...
						If showing Then
							states.add(AccessibleState.SHOWING)
						ElseIf states.contains(AccessibleState.SHOWING) Then
							states.remove(AccessibleState.SHOWING)
						End If
						If visible Then
							states.add(AccessibleState.VISIBLE)
						ElseIf states.contains(AccessibleState.VISIBLE) Then
							states.remove(AccessibleState.VISIBLE)
						End If
						If tree.isPathSelected(path) Then states.add(AccessibleState.SELECTED)
						If path Is leadSelectionPath Then states.add(AccessibleState.ACTIVE)
						If Not isLeaf Then states.add(AccessibleState.EXPANDABLE)
						If tree.isExpanded(path) Then
							states.add(AccessibleState.EXPANDED)
						Else
							states.add(AccessibleState.COLLAPSED)
						End If
						If tree.editable Then states.add(AccessibleState.EDITABLE)
						Return states
					End Get
				End Property

				''' <summary>
				''' Get the Accessible parent of this object.
				''' </summary>
				''' <returns> the Accessible parent of this object; null if this
				''' object does not have an Accessible parent </returns>
				Public Property Overrides accessibleParent As Accessible
					Get
						' someone wants to know, so we need to create our parent
						' if we don't have one (hey, we're a talented kid!)
						If accessibleParent Is Nothing Then
							Dim objPath As Object() = path.path
							If objPath.Length > 1 Then
								Dim objParent As Object = objPath(objPath.Length-2)
								If treeModel IsNot Nothing Then index = treeModel.getIndexOfChild(objParent, obj)
								Dim objParentPath As Object() = New Object(objPath.Length-2){}
								Array.Copy(objPath, 0, objParentPath, 0, objPath.Length-1)
								Dim parentPath As New TreePath(objParentPath)
								accessibleParent = New AccessibleJTreeNode(tree, parentPath, Nothing)
								Me.accessibleParent = accessibleParent
							ElseIf treeModel IsNot Nothing Then
								accessibleParent = tree ' we're the top!
								index = 0 ' we're an only child!
								Me.accessibleParent = accessibleParent
							End If
						End If
						Return accessibleParent
					End Get
				End Property

				''' <summary>
				''' Get the index of this object in its accessible parent.
				''' </summary>
				''' <returns> the index of this object in its parent; -1 if this
				''' object does not have an accessible parent. </returns>
				''' <seealso cref= #getAccessibleParent </seealso>
				Public Property Overrides accessibleIndexInParent As Integer
					Get
						' index is invalid 'till we have an accessibleParent...
						If accessibleParent Is Nothing Then accessibleParent
						Dim objPath As Object() = path.path
						If objPath.Length > 1 Then
							Dim objParent As Object = objPath(objPath.Length-2)
							If treeModel IsNot Nothing Then index = treeModel.getIndexOfChild(objParent, obj)
						End If
						Return index
					End Get
				End Property

				''' <summary>
				''' Returns the number of accessible children in the object.
				''' </summary>
				''' <returns> the number of accessible children in the object. </returns>
				Public Property Overrides accessibleChildrenCount As Integer
					Get
						' Tree nodes can't be so complex that they have
						' two sets of children -> we're ignoring that case
						Return treeModel.getChildCount(obj)
					End Get
				End Property

				''' <summary>
				''' Return the specified Accessible child of the object.
				''' </summary>
				''' <param name="i"> zero-based index of child </param>
				''' <returns> the Accessible child of the object </returns>
				Public Overrides Function getAccessibleChild(ByVal i As Integer) As Accessible
					' Tree nodes can't be so complex that they have
					' two sets of children -> we're ignoring that case
					If i < 0 OrElse i >= accessibleChildrenCount Then
						Return Nothing
					Else
						Dim childObj As Object = treeModel.getChild(obj, i)
						Dim objPath As Object() = path.path
						Dim objChildPath As Object() = New Object(objPath.Length){}
						Array.Copy(objPath, 0, objChildPath, 0, objPath.Length)
						objChildPath(objChildPath.Length-1) = childObj
						Dim childPath As New TreePath(objChildPath)
						Return New AccessibleJTreeNode(JTree.this, childPath, Me)
					End If
				End Function

				''' <summary>
				''' Gets the locale of the component. If the component does not have
				''' a locale, then the locale of its parent is returned.
				''' </summary>
				''' <returns> This component's locale. If this component does not have
				''' a locale, the locale of its parent is returned. </returns>
				''' <exception cref="IllegalComponentStateException">
				''' If the Component does not have its own locale and has not yet
				''' been added to a containment hierarchy such that the locale can be
				''' determined from the containing parent. </exception>
				''' <seealso cref= #setLocale </seealso>
				Public Property Overrides locale As Locale
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Return ac.locale
						Else
							Return tree.locale
						End If
					End Get
				End Property

				''' <summary>
				''' Add a PropertyChangeListener to the listener list.
				''' The listener is registered for all properties.
				''' </summary>
				''' <param name="l">  The PropertyChangeListener to be added </param>
				Public Overridable Sub addPropertyChangeListener(ByVal l As PropertyChangeListener)
					Dim ac As AccessibleContext = currentAccessibleContext
					If ac IsNot Nothing Then
						ac.addPropertyChangeListener(l)
					Else
						MyBase.addPropertyChangeListener(l)
					End If
				End Sub

				''' <summary>
				''' Remove a PropertyChangeListener from the listener list.
				''' This removes a PropertyChangeListener that was registered
				''' for all properties.
				''' </summary>
				''' <param name="l">  The PropertyChangeListener to be removed </param>
				Public Overridable Sub removePropertyChangeListener(ByVal l As PropertyChangeListener)
					Dim ac As AccessibleContext = currentAccessibleContext
					If ac IsNot Nothing Then
						ac.removePropertyChangeListener(l)
					Else
						MyBase.removePropertyChangeListener(l)
					End If
				End Sub

				''' <summary>
				''' Get the AccessibleAction associated with this object.  In the
				''' implementation of the Java Accessibility API for this class,
				''' return this object, which is responsible for implementing the
				''' AccessibleAction interface on behalf of itself.
				''' </summary>
				''' <returns> this object </returns>
				Public Property Overrides accessibleAction As AccessibleAction
					Get
						Return Me
					End Get
				End Property

				''' <summary>
				''' Get the AccessibleComponent associated with this object.  In the
				''' implementation of the Java Accessibility API for this class,
				''' return this object, which is responsible for implementing the
				''' AccessibleComponent interface on behalf of itself.
				''' </summary>
				''' <returns> this object </returns>
				Public Property Overrides accessibleComponent As AccessibleComponent
					Get
						Return Me ' to override getBounds()
					End Get
				End Property

				''' <summary>
				''' Get the AccessibleSelection associated with this object if one
				''' exists.  Otherwise return null.
				''' </summary>
				''' <returns> the AccessibleSelection, or null </returns>
				Public Property Overrides accessibleSelection As AccessibleSelection
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing AndAlso isLeaf Then
							Return currentAccessibleContext.accessibleSelection
						Else
							Return Me
						End If
					End Get
				End Property

				''' <summary>
				''' Get the AccessibleText associated with this object if one
				''' exists.  Otherwise return null.
				''' </summary>
				''' <returns> the AccessibleText, or null </returns>
				Public Property Overrides accessibleText As AccessibleText
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Return currentAccessibleContext.accessibleText
						Else
							Return Nothing
						End If
					End Get
				End Property

				''' <summary>
				''' Get the AccessibleValue associated with this object if one
				''' exists.  Otherwise return null.
				''' </summary>
				''' <returns> the AccessibleValue, or null </returns>
				Public Property Overrides accessibleValue As AccessibleValue
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Return currentAccessibleContext.accessibleValue
						Else
							Return Nothing
						End If
					End Get
				End Property


			' AccessibleComponent methods

				''' <summary>
				''' Get the background color of this object.
				''' </summary>
				''' <returns> the background color, if supported, of the object;
				''' otherwise, null </returns>
				Public Overridable Property background As Color Implements AccessibleComponent.getBackground
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).background
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.background
							Else
								Return Nothing
							End If
						End If
					End Get
					Set(ByVal c As Color)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).background = c
						Else
							Dim cp As Component = currentComponent
							If cp IsNot Nothing Then cp.background = c
						End If
					End Set
				End Property



				''' <summary>
				''' Get the foreground color of this object.
				''' </summary>
				''' <returns> the foreground color, if supported, of the object;
				''' otherwise, null </returns>
				Public Overridable Property foreground As Color Implements AccessibleComponent.getForeground
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).foreground
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.foreground
							Else
								Return Nothing
							End If
						End If
					End Get
					Set(ByVal c As Color)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).foreground = c
						Else
							Dim cp As Component = currentComponent
							If cp IsNot Nothing Then cp.foreground = c
						End If
					End Set
				End Property


				Public Overridable Property cursor As Cursor Implements AccessibleComponent.getCursor
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).cursor
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.cursor
							Else
								Dim ap As Accessible = accessibleParent
								If TypeOf ap Is AccessibleComponent Then
									Return CType(ap, AccessibleComponent).cursor
								Else
									Return Nothing
								End If
							End If
						End If
					End Get
					Set(ByVal c As Cursor)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).cursor = c
						Else
							Dim cp As Component = currentComponent
							If cp IsNot Nothing Then cp.cursor = c
						End If
					End Set
				End Property


				Public Overridable Property font As Font Implements AccessibleComponent.getFont
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).font
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.font
							Else
								Return Nothing
							End If
						End If
					End Get
					Set(ByVal f As Font)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).font = f
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then c.font = f
						End If
					End Set
				End Property


				Public Overridable Function getFontMetrics(ByVal f As Font) As FontMetrics Implements AccessibleComponent.getFontMetrics
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						Return CType(ac, AccessibleComponent).getFontMetrics(f)
					Else
						Dim c As Component = currentComponent
						If c IsNot Nothing Then
							Return c.getFontMetrics(f)
						Else
							Return Nothing
						End If
					End If
				End Function

				Public Overridable Property enabled As Boolean Implements AccessibleComponent.isEnabled
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).enabled
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.enabled
							Else
								Return False
							End If
						End If
					End Get
					Set(ByVal b As Boolean)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).enabled = b
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then c.enabled = b
						End If
					End Set
				End Property


				Public Overridable Property visible As Boolean Implements AccessibleComponent.isVisible
					Get
						Dim pathBounds As Rectangle = tree.getPathBounds(path)
						Dim parentBounds As Rectangle = tree.visibleRect
						Return pathBounds IsNot Nothing AndAlso parentBounds IsNot Nothing AndAlso parentBounds.intersects(pathBounds)
					End Get
					Set(ByVal b As Boolean)
					End Set
				End Property


				Public Overridable Property showing As Boolean Implements AccessibleComponent.isShowing
					Get
						Return (tree.showing AndAlso visible)
					End Get
				End Property

				Public Overridable Function contains(ByVal p As Point) As Boolean Implements AccessibleComponent.contains
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						Dim r As Rectangle = CType(ac, AccessibleComponent).bounds
						Return r.contains(p)
					Else
						Dim c As Component = currentComponent
						If c IsNot Nothing Then
							Dim r As Rectangle = c.bounds
							Return r.contains(p)
						Else
							Return bounds.contains(p)
						End If
					End If
				End Function

				Public Overridable Property locationOnScreen As Point Implements AccessibleComponent.getLocationOnScreen
					Get
						If tree IsNot Nothing Then
							Dim treeLocation As Point = tree.locationOnScreen
							Dim pathBounds As Rectangle = tree.getPathBounds(path)
							If treeLocation IsNot Nothing AndAlso pathBounds IsNot Nothing Then
								Dim nodeLocation As New Point(pathBounds.x, pathBounds.y)
								nodeLocation.translate(treeLocation.x, treeLocation.y)
								Return nodeLocation
							Else
								Return Nothing
							End If
						Else
							Return Nothing
						End If
					End Get
				End Property

				Protected Friend Overridable Property locationInJTree As Point
					Get
						Dim r As Rectangle = tree.getPathBounds(path)
						If r IsNot Nothing Then
							Return r.location
						Else
							Return Nothing
						End If
					End Get
				End Property

				Public Overridable Property location As Point Implements AccessibleComponent.getLocation
					Get
						Dim r As Rectangle = bounds
						If r IsNot Nothing Then
							Return r.location
						Else
							Return Nothing
						End If
					End Get
					Set(ByVal p As Point)
					End Set
				End Property


				Public Overridable Property bounds As Rectangle Implements AccessibleComponent.getBounds
					Get
						Dim r As Rectangle = tree.getPathBounds(path)
						Dim parent As Accessible = accessibleParent
						If parent IsNot Nothing Then
							If TypeOf parent Is AccessibleJTreeNode Then
								Dim parentLoc As Point = CType(parent, AccessibleJTreeNode).locationInJTree
								If parentLoc IsNot Nothing AndAlso r IsNot Nothing Then
									r.translate(-parentLoc.x, -parentLoc.y)
								Else
									Return Nothing ' not visible!
								End If
							End If
						End If
						Return r
					End Get
					Set(ByVal r As Rectangle)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).bounds = r
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then c.bounds = r
						End If
					End Set
				End Property


				Public Overridable Property size As Dimension Implements AccessibleComponent.getSize
					Get
						Return bounds.size
					End Get
					Set(ByVal d As Dimension)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).size = d
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then c.size = d
						End If
					End Set
				End Property


				''' <summary>
				''' Returns the <code>Accessible</code> child, if one exists,
				''' contained at the local coordinate <code>Point</code>.
				''' Otherwise returns <code>null</code>.
				''' </summary>
				''' <param name="p"> point in local coordinates of this
				'''    <code>Accessible</code> </param>
				''' <returns> the <code>Accessible</code>, if it exists,
				'''    at the specified location; else <code>null</code> </returns>
				Public Overridable Function getAccessibleAt(ByVal p As Point) As Accessible Implements AccessibleComponent.getAccessibleAt
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						Return CType(ac, AccessibleComponent).getAccessibleAt(p)
					Else
						Return Nothing
					End If
				End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Public Overridable Property focusTraversable As Boolean Implements AccessibleComponent.isFocusTraversable
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).focusTraversable
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.focusTraversable
							Else
								Return False
							End If
						End If
					End Get
				End Property

				Public Overridable Sub requestFocus() Implements AccessibleComponent.requestFocus
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						CType(ac, AccessibleComponent).requestFocus()
					Else
						Dim c As Component = currentComponent
						If c IsNot Nothing Then c.requestFocus()
					End If
				End Sub

				Public Overridable Sub addFocusListener(ByVal l As FocusListener) Implements AccessibleComponent.addFocusListener
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						CType(ac, AccessibleComponent).addFocusListener(l)
					Else
						Dim c As Component = currentComponent
						If c IsNot Nothing Then c.addFocusListener(l)
					End If
				End Sub

				Public Overridable Sub removeFocusListener(ByVal l As FocusListener) Implements AccessibleComponent.removeFocusListener
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						CType(ac, AccessibleComponent).removeFocusListener(l)
					Else
						Dim c As Component = currentComponent
						If c IsNot Nothing Then c.removeFocusListener(l)
					End If
				End Sub

			' AccessibleSelection methods

				''' <summary>
				''' Returns the number of items currently selected.
				''' If no items are selected, the return value will be 0.
				''' </summary>
				''' <returns> the number of items currently selected. </returns>
				Public Overridable Property accessibleSelectionCount As Integer Implements AccessibleSelection.getAccessibleSelectionCount
					Get
						Dim count As Integer = 0
						Dim childCount As Integer = accessibleChildrenCount
						For i As Integer = 0 To childCount - 1
							Dim childPath As TreePath = getChildTreePath(i)
							If tree.isPathSelected(childPath) Then count += 1
						Next i
						Return count
					End Get
				End Property

				''' <summary>
				''' Returns an Accessible representing the specified selected item
				''' in the object.  If there isn't a selection, or there are
				''' fewer items selected than the integer passed in, the return
				''' value will be null.
				''' </summary>
				''' <param name="i"> the zero-based index of selected items </param>
				''' <returns> an Accessible containing the selected item </returns>
				Public Overridable Function getAccessibleSelection(ByVal i As Integer) As Accessible Implements AccessibleSelection.getAccessibleSelection
					Dim childCount As Integer = accessibleChildrenCount
					If i < 0 OrElse i >= childCount Then Return Nothing ' out of range
					Dim count As Integer = 0
					Dim j As Integer = 0
					Do While j < childCount AndAlso i >= count
						Dim childPath As TreePath = getChildTreePath(j)
						If tree.isPathSelected(childPath) Then
							If count = i Then
								Return New AccessibleJTreeNode(tree, childPath, Me)
							Else
								count += 1
							End If
						End If
						j += 1
					Loop
					Return Nothing
				End Function

				''' <summary>
				''' Returns true if the current child of this object is selected.
				''' </summary>
				''' <param name="i"> the zero-based index of the child in this Accessible
				''' object. </param>
				''' <seealso cref= AccessibleContext#getAccessibleChild </seealso>
				Public Overridable Function isAccessibleChildSelected(ByVal i As Integer) As Boolean Implements AccessibleSelection.isAccessibleChildSelected
					Dim childCount As Integer = accessibleChildrenCount
					If i < 0 OrElse i >= childCount Then
						Return False ' out of range
					Else
						Dim childPath As TreePath = getChildTreePath(i)
						Return tree.isPathSelected(childPath)
					End If
				End Function

				''' <summary>
				''' Adds the specified selected item in the object to the object's
				''' selection.  If the object supports multiple selections,
				''' the specified item is added to any existing selection, otherwise
				''' it replaces any existing selection in the object.  If the
				''' specified item is already selected, this method has no effect.
				''' </summary>
				''' <param name="i"> the zero-based index of selectable items </param>
				Public Overridable Sub addAccessibleSelection(ByVal i As Integer) Implements AccessibleSelection.addAccessibleSelection
				   Dim model As TreeModel = outerInstance.model
				   If model IsNot Nothing Then
					   If i >= 0 AndAlso i < accessibleChildrenCount Then
						   Dim path As TreePath = getChildTreePath(i)
						   outerInstance.addSelectionPath(path)
					   End If
				   End If
				End Sub

				''' <summary>
				''' Removes the specified selected item in the object from the
				''' object's
				''' selection.  If the specified item isn't currently selected, this
				''' method has no effect.
				''' </summary>
				''' <param name="i"> the zero-based index of selectable items </param>
				Public Overridable Sub removeAccessibleSelection(ByVal i As Integer) Implements AccessibleSelection.removeAccessibleSelection
				   Dim model As TreeModel = outerInstance.model
				   If model IsNot Nothing Then
					   If i >= 0 AndAlso i < accessibleChildrenCount Then
						   Dim path As TreePath = getChildTreePath(i)
						   outerInstance.removeSelectionPath(path)
					   End If
				   End If
				End Sub

				''' <summary>
				''' Clears the selection in the object, so that nothing in the
				''' object is selected.
				''' </summary>
				Public Overridable Sub clearAccessibleSelection() Implements AccessibleSelection.clearAccessibleSelection
					Dim childCount As Integer = accessibleChildrenCount
					For i As Integer = 0 To childCount - 1
						removeAccessibleSelection(i)
					Next i
				End Sub

				''' <summary>
				''' Causes every selected item in the object to be selected
				''' if the object supports multiple selections.
				''' </summary>
				Public Overridable Sub selectAllAccessibleSelection() Implements AccessibleSelection.selectAllAccessibleSelection
				   Dim model As TreeModel = outerInstance.model
				   If model IsNot Nothing Then
					   Dim childCount As Integer = accessibleChildrenCount
					   Dim path As TreePath
					   For i As Integer = 0 To childCount - 1
						   path = getChildTreePath(i)
						   outerInstance.addSelectionPath(path)
					   Next i
				   End If
				End Sub

			' AccessibleAction methods

				''' <summary>
				''' Returns the number of accessible actions available in this
				''' tree node.  If this node is not a leaf, there is at least
				''' one action (toggle expand), in addition to any available
				''' on the object behind the TreeCellRenderer.
				''' </summary>
				''' <returns> the number of Actions in this object </returns>
				Public Overridable Property accessibleActionCount As Integer Implements AccessibleAction.getAccessibleActionCount
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Dim aa As AccessibleAction = ac.accessibleAction
							If aa IsNot Nothing Then Return (aa.accessibleActionCount + (If(isLeaf, 0, 1)))
						End If
						Return If(isLeaf, 0, 1)
					End Get
				End Property

				''' <summary>
				''' Return a description of the specified action of the tree node.
				''' If this node is not a leaf, there is at least one action
				''' description (toggle expand), in addition to any available
				''' on the object behind the TreeCellRenderer.
				''' </summary>
				''' <param name="i"> zero-based index of the actions </param>
				''' <returns> a description of the action </returns>
				Public Overridable Function getAccessibleActionDescription(ByVal i As Integer) As String Implements AccessibleAction.getAccessibleActionDescription
					If i < 0 OrElse i >= accessibleActionCount Then Return Nothing
					Dim ac As AccessibleContext = currentAccessibleContext
					If i = 0 Then
						' TIGER - 4766636
						Return AccessibleAction.TOGGLE_EXPAND
					ElseIf ac IsNot Nothing Then
						Dim aa As AccessibleAction = ac.accessibleAction
						If aa IsNot Nothing Then Return aa.getAccessibleActionDescription(i - 1)
					End If
					Return Nothing
				End Function

				''' <summary>
				''' Perform the specified Action on the tree node.  If this node
				''' is not a leaf, there is at least one action which can be
				''' done (toggle expand), in addition to any available on the
				''' object behind the TreeCellRenderer.
				''' </summary>
				''' <param name="i"> zero-based index of actions </param>
				''' <returns> true if the the action was performed; else false. </returns>
				Public Overridable Function doAccessibleAction(ByVal i As Integer) As Boolean Implements AccessibleAction.doAccessibleAction
					If i < 0 OrElse i >= accessibleActionCount Then Return False
					Dim ac As AccessibleContext = currentAccessibleContext
					If i = 0 Then
						If outerInstance.isExpanded(path) Then
							outerInstance.collapsePath(path)
						Else
							outerInstance.expandPath(path)
						End If
						Return True
					ElseIf ac IsNot Nothing Then
						Dim aa As AccessibleAction = ac.accessibleAction
						If aa IsNot Nothing Then Return aa.doAccessibleAction(i - 1)
					End If
					Return False
				End Function

			End Class ' inner class AccessibleJTreeNode

		End Class ' inner class AccessibleJTree

	End Class ' End of class JTree

End Namespace
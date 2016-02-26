Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Text
Imports javax.swing
Imports javax.swing.event
Imports javax.swing.tree

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

Namespace javax.swing.plaf.basic



	''' <summary>
	''' The basic L&amp;F for a hierarchical data structure.
	''' <p>
	''' 
	''' @author Scott Violet
	''' @author Shannon Hickey (drag and drop)
	''' </summary>

	Public Class BasicTreeUI
		Inherits javax.swing.plaf.TreeUI

		Private Shared ReadOnly BASELINE_COMPONENT_KEY As New StringBuilder("Tree.baselineComponent")

		' Old actions forward to an instance of this.
		Private Shared ReadOnly SHARED_ACTION As New Actions

		<NonSerialized> _
		Protected Friend collapsedIcon As Icon
		<NonSerialized> _
		Protected Friend expandedIcon As Icon

		''' <summary>
		''' Color used to draw hash marks.  If <code>null</code> no hash marks
		''' will be drawn.
		''' </summary>
		Private hashColor As Color

		''' <summary>
		''' Distance between left margin and where vertical dashes will be
		''' drawn. 
		''' </summary>
		Protected Friend leftChildIndent As Integer
		''' <summary>
		''' Distance to add to leftChildIndent to determine where cell
		''' contents will be drawn. 
		''' </summary>
		Protected Friend rightChildIndent As Integer
		''' <summary>
		''' Total distance that will be indented.  The sum of leftChildIndent
		''' and rightChildIndent. 
		''' </summary>
		Protected Friend totalChildIndent As Integer

		''' <summary>
		''' Minimum preferred size. </summary>
		Protected Friend preferredMinSize As Dimension

		''' <summary>
		''' Index of the row that was last selected. </summary>
		Protected Friend lastSelectedRow As Integer

		''' <summary>
		''' Component that we're going to be drawing into. </summary>
		Protected Friend tree As JTree

		''' <summary>
		''' Renderer that is being used to do the actual cell drawing. </summary>
		<NonSerialized> _
		Protected Friend currentCellRenderer As TreeCellRenderer

		''' <summary>
		''' Set to true if the renderer that is currently in the tree was
		''' created by this instance. 
		''' </summary>
		Protected Friend createdRenderer As Boolean

		''' <summary>
		''' Editor for the tree. </summary>
		<NonSerialized> _
		Protected Friend cellEditor As TreeCellEditor

		''' <summary>
		''' Set to true if editor that is currently in the tree was
		''' created by this instance. 
		''' </summary>
		Protected Friend createdCellEditor As Boolean

		''' <summary>
		''' Set to false when editing and shouldSelectCell() returns true meaning
		''' the node should be selected before editing, used in completeEditing. 
		''' </summary>
		Protected Friend stopEditingInCompleteEditing As Boolean

		''' <summary>
		''' Used to paint the TreeCellRenderer. </summary>
		Protected Friend rendererPane As CellRendererPane

		''' <summary>
		''' Size needed to completely display all the nodes. </summary>
		Protected Friend preferredSize As Dimension

		''' <summary>
		''' Is the preferredSize valid? </summary>
		Protected Friend validCachedPreferredSize As Boolean

		''' <summary>
		''' Object responsible for handling sizing and expanded issues. </summary>
		' WARNING: Be careful with the bounds held by treeState. They are
		' always in terms of left-to-right. They get mapped to right-to-left
		' by the various methods of this class.
		Protected Friend treeState As AbstractLayoutCache


		''' <summary>
		''' Used for minimizing the drawing of vertical lines. </summary>
		Protected Friend drawingCache As Dictionary(Of TreePath, Boolean?)

		''' <summary>
		''' True if doing optimizations for a largeModel. Subclasses that
		''' don't support this may wish to override createLayoutCache to not
		''' return a FixedHeightLayoutCache instance. 
		''' </summary>
		Protected Friend largeModel As Boolean

		''' <summary>
		''' Reponsible for telling the TreeState the size needed for a node. </summary>
		Protected Friend nodeDimensions As AbstractLayoutCache.NodeDimensions

		''' <summary>
		''' Used to determine what to display. </summary>
		Protected Friend treeModel As TreeModel

		''' <summary>
		''' Model maintaining the selection. </summary>
		Protected Friend treeSelectionModel As TreeSelectionModel

		''' <summary>
		''' How much the depth should be offset to properly calculate
		''' x locations. This is based on whether or not the root is visible,
		''' and if the root handles are visible. 
		''' </summary>
		Protected Friend depthOffset As Integer

		' Following 4 ivars are only valid when editing.

		''' <summary>
		''' When editing, this will be the Component that is doing the actual
		''' editing. 
		''' </summary>
		Protected Friend editingComponent As Component

		''' <summary>
		''' Path that is being edited. </summary>
		Protected Friend editingPath As TreePath

		''' <summary>
		''' Row that is being edited. Should only be referenced if
		''' editingComponent is not null. 
		''' </summary>
		Protected Friend editingRow As Integer

		''' <summary>
		''' Set to true if the editor has a different size than the renderer. </summary>
		Protected Friend editorHasDifferentSize As Boolean

		''' <summary>
		''' Row correspondin to lead path. </summary>
		Private leadRow As Integer
		''' <summary>
		''' If true, the property change event for LEAD_SELECTION_PATH_PROPERTY,
		''' or ANCHOR_SELECTION_PATH_PROPERTY will not generate a repaint. 
		''' </summary>
		Private ignoreLAChange As Boolean

		''' <summary>
		''' Indicates the orientation. </summary>
		Private leftToRight As Boolean

		' Cached listeners
		Private propertyChangeListener As PropertyChangeListener
		Private selectionModelPropertyChangeListener As PropertyChangeListener
		Private mouseListener As MouseListener
		Private focusListener As FocusListener
		Private keyListener As KeyListener
		''' <summary>
		''' Used for large models, listens for moved/resized events and
		''' updates the validCachedPreferredSize bit accordingly. 
		''' </summary>
		Private componentListener As ComponentListener
		''' <summary>
		''' Listens for CellEditor events. </summary>
		Private cellEditorListener As CellEditorListener
		''' <summary>
		''' Updates the display when the selection changes. </summary>
		Private treeSelectionListener As TreeSelectionListener
		''' <summary>
		''' Is responsible for updating the display based on model events. </summary>
		Private treeModelListener As TreeModelListener
		''' <summary>
		''' Updates the treestate as the nodes expand. </summary>
		Private treeExpansionListener As TreeExpansionListener

		''' <summary>
		''' UI property indicating whether to paint lines </summary>
		Private paintLines As Boolean = True

		''' <summary>
		''' UI property for painting dashed lines </summary>
		Private lineTypeDashed As Boolean

		''' <summary>
		''' The time factor to treate the series of typed alphanumeric key
		''' as prefix for first letter navigation.
		''' </summary>
		Private timeFactor As Long = 1000L

		Private handler As Handler

		''' <summary>
		''' A temporary variable for communication between startEditingOnRelease
		''' and startEditing.
		''' </summary>
		Private releaseEvent As MouseEvent

		Public Shared Function createUI(ByVal x As JComponent) As javax.swing.plaf.ComponentUI
			Return New BasicTreeUI
		End Function


		Friend Shared Sub loadActionMap(ByVal map As LazyActionMap)
			map.put(New Actions(Actions.SELECT_PREVIOUS))
			map.put(New Actions(Actions.SELECT_PREVIOUS_CHANGE_LEAD))
			map.put(New Actions(Actions.SELECT_PREVIOUS_EXTEND_SELECTION))

			map.put(New Actions(Actions.SELECT_NEXT))
			map.put(New Actions(Actions.SELECT_NEXT_CHANGE_LEAD))
			map.put(New Actions(Actions.SELECT_NEXT_EXTEND_SELECTION))

			map.put(New Actions(Actions.SELECT_CHILD))
			map.put(New Actions(Actions.SELECT_CHILD_CHANGE_LEAD))

			map.put(New Actions(Actions.SELECT_PARENT))
			map.put(New Actions(Actions.SELECT_PARENT_CHANGE_LEAD))

			map.put(New Actions(Actions.SCROLL_UP_CHANGE_SELECTION))
			map.put(New Actions(Actions.SCROLL_UP_CHANGE_LEAD))
			map.put(New Actions(Actions.SCROLL_UP_EXTEND_SELECTION))

			map.put(New Actions(Actions.SCROLL_DOWN_CHANGE_SELECTION))
			map.put(New Actions(Actions.SCROLL_DOWN_EXTEND_SELECTION))
			map.put(New Actions(Actions.SCROLL_DOWN_CHANGE_LEAD))

			map.put(New Actions(Actions.SELECT_FIRST))
			map.put(New Actions(Actions.SELECT_FIRST_CHANGE_LEAD))
			map.put(New Actions(Actions.SELECT_FIRST_EXTEND_SELECTION))

			map.put(New Actions(Actions.SELECT_LAST))
			map.put(New Actions(Actions.SELECT_LAST_CHANGE_LEAD))
			map.put(New Actions(Actions.SELECT_LAST_EXTEND_SELECTION))

			map.put(New Actions(Actions.TOGGLE))

			map.put(New Actions(Actions.CANCEL_EDITING))

			map.put(New Actions(Actions.START_EDITING))

			map.put(New Actions(Actions.SELECT_ALL))

			map.put(New Actions(Actions.CLEAR_SELECTION))

			map.put(New Actions(Actions.SCROLL_LEFT))
			map.put(New Actions(Actions.SCROLL_RIGHT))

			map.put(New Actions(Actions.SCROLL_LEFT_EXTEND_SELECTION))
			map.put(New Actions(Actions.SCROLL_RIGHT_EXTEND_SELECTION))

			map.put(New Actions(Actions.SCROLL_RIGHT_CHANGE_LEAD))
			map.put(New Actions(Actions.SCROLL_LEFT_CHANGE_LEAD))

			map.put(New Actions(Actions.EXPAND))
			map.put(New Actions(Actions.COLLAPSE))
			map.put(New Actions(Actions.MOVE_SELECTION_TO_PARENT))

			map.put(New Actions(Actions.ADD_TO_SELECTION))
			map.put(New Actions(Actions.TOGGLE_AND_ANCHOR))
			map.put(New Actions(Actions.EXTEND_TO))
			map.put(New Actions(Actions.MOVE_SELECTION_TO))

			map.put(TransferHandler.cutAction)
			map.put(TransferHandler.copyAction)
			map.put(TransferHandler.pasteAction)
		End Sub


		Public Sub New()
			MyBase.New()
		End Sub

		Protected Friend Overridable Property hashColor As Color
			Get
				Return hashColor
			End Get
			Set(ByVal color As Color)
				hashColor = color
			End Set
		End Property


		Public Overridable Property leftChildIndent As Integer
			Set(ByVal newAmount As Integer)
				leftChildIndent = newAmount
				totalChildIndent = leftChildIndent + rightChildIndent
				If treeState IsNot Nothing Then treeState.invalidateSizes()
				updateSize()
			End Set
			Get
				Return leftChildIndent
			End Get
		End Property


		Public Overridable Property rightChildIndent As Integer
			Set(ByVal newAmount As Integer)
				rightChildIndent = newAmount
				totalChildIndent = leftChildIndent + rightChildIndent
				If treeState IsNot Nothing Then treeState.invalidateSizes()
				updateSize()
			End Set
			Get
				Return rightChildIndent
			End Get
		End Property


		Public Overridable Property expandedIcon As Icon
			Set(ByVal newG As Icon)
				expandedIcon = newG
			End Set
			Get
				Return expandedIcon
			End Get
		End Property


		Public Overridable Property collapsedIcon As Icon
			Set(ByVal newG As Icon)
				collapsedIcon = newG
			End Set
			Get
				Return collapsedIcon
			End Get
		End Property


		'
		' Methods for configuring the behavior of the tree. None of them
		' push the value to the JTree instance. You should really only
		' call these methods on the JTree.
		'

		''' <summary>
		''' Updates the componentListener, if necessary.
		''' </summary>
		Protected Friend Overridable Property largeModel As Boolean
			Set(ByVal largeModel As Boolean)
				If rowHeight < 1 Then largeModel = False
				If Me.largeModel <> largeModel Then
					completeEditing()
					Me.largeModel = largeModel
					treeState = createLayoutCache()
					configureLayoutCache()
					updateLayoutCacheExpandedNodesIfNecessary()
					updateSize()
				End If
			End Set
			Get
				Return largeModel
			End Get
		End Property


		''' <summary>
		''' Sets the row height, this is forwarded to the treeState.
		''' </summary>
		Protected Friend Overridable Property rowHeight As Integer
			Set(ByVal rowHeight As Integer)
				completeEditing()
				If treeState IsNot Nothing Then
					largeModel = tree.largeModel
					treeState.rowHeight = rowHeight
					updateSize()
				End If
			End Set
			Get
				Return If(tree Is Nothing, -1, tree.rowHeight)
			End Get
		End Property


		''' <summary>
		''' Sets the TreeCellRenderer to <code>tcr</code>. This invokes
		''' <code>updateRenderer</code>.
		''' </summary>
		Protected Friend Overridable Property cellRenderer As TreeCellRenderer
			Set(ByVal tcr As TreeCellRenderer)
				completeEditing()
				updateRenderer()
				If treeState IsNot Nothing Then
					treeState.invalidateSizes()
					updateSize()
				End If
			End Set
			Get
				Return currentCellRenderer
			End Get
		End Property


		''' <summary>
		''' Sets the TreeModel.
		''' </summary>
		Protected Friend Overridable Property model As TreeModel
			Set(ByVal model As TreeModel)
				completeEditing()
				If treeModel IsNot Nothing AndAlso treeModelListener IsNot Nothing Then treeModel.removeTreeModelListener(treeModelListener)
				treeModel = model
				If treeModel IsNot Nothing Then
					If treeModelListener IsNot Nothing Then treeModel.addTreeModelListener(treeModelListener)
				End If
				If treeState IsNot Nothing Then
					treeState.model = model
					updateLayoutCacheExpandedNodesIfNecessary()
					updateSize()
				End If
			End Set
			Get
				Return treeModel
			End Get
		End Property


		''' <summary>
		''' Sets the root to being visible.
		''' </summary>
		Protected Friend Overridable Property rootVisible As Boolean
			Set(ByVal newValue As Boolean)
				completeEditing()
				updateDepthOffset()
				If treeState IsNot Nothing Then
					treeState.rootVisible = newValue
					treeState.invalidateSizes()
					updateSize()
				End If
			End Set
			Get
				Return If(tree IsNot Nothing, tree.rootVisible, False)
			End Get
		End Property


		''' <summary>
		''' Determines whether the node handles are to be displayed.
		''' </summary>
		Protected Friend Overridable Property showsRootHandles As Boolean
			Set(ByVal newValue As Boolean)
				completeEditing()
				updateDepthOffset()
				If treeState IsNot Nothing Then
					treeState.invalidateSizes()
					updateSize()
				End If
			End Set
			Get
				Return If(tree IsNot Nothing, tree.showsRootHandles, False)
			End Get
		End Property


		''' <summary>
		''' Sets the cell editor.
		''' </summary>
		Protected Friend Overridable Property cellEditor As TreeCellEditor
			Set(ByVal editor As TreeCellEditor)
				updateCellEditor()
			End Set
			Get
				Return If(tree IsNot Nothing, tree.cellEditor, Nothing)
			End Get
		End Property


		''' <summary>
		''' Configures the receiver to allow, or not allow, editing.
		''' </summary>
		Protected Friend Overridable Property editable As Boolean
			Set(ByVal newValue As Boolean)
				updateCellEditor()
			End Set
			Get
				Return If(tree IsNot Nothing, tree.editable, False)
			End Get
		End Property


		''' <summary>
		''' Resets the selection model. The appropriate listener are installed
		''' on the model.
		''' </summary>
		Protected Friend Overridable Property selectionModel As TreeSelectionModel
			Set(ByVal newLSM As TreeSelectionModel)
				completeEditing()
				If selectionModelPropertyChangeListener IsNot Nothing AndAlso treeSelectionModel IsNot Nothing Then treeSelectionModel.removePropertyChangeListener(selectionModelPropertyChangeListener)
				If treeSelectionListener IsNot Nothing AndAlso treeSelectionModel IsNot Nothing Then treeSelectionModel.removeTreeSelectionListener(treeSelectionListener)
				treeSelectionModel = newLSM
				If treeSelectionModel IsNot Nothing Then
					If selectionModelPropertyChangeListener IsNot Nothing Then treeSelectionModel.addPropertyChangeListener(selectionModelPropertyChangeListener)
					If treeSelectionListener IsNot Nothing Then treeSelectionModel.addTreeSelectionListener(treeSelectionListener)
					If treeState IsNot Nothing Then treeState.selectionModel = treeSelectionModel
				ElseIf treeState IsNot Nothing Then
					treeState.selectionModel = Nothing
				End If
				If tree IsNot Nothing Then tree.repaint()
			End Set
			Get
				Return treeSelectionModel
			End Get
		End Property


		'
		' TreeUI methods
		'

		''' <summary>
		''' Returns the Rectangle enclosing the label portion that the
		''' last item in path will be drawn into.  Will return null if
		''' any component in path is currently valid.
		''' </summary>
		Public Overridable Function getPathBounds(ByVal tree As JTree, ByVal path As TreePath) As Rectangle
			If tree IsNot Nothing AndAlso treeState IsNot Nothing Then Return getPathBounds(path, tree.insets, New Rectangle)
			Return Nothing
		End Function

		Private Function getPathBounds(ByVal path As TreePath, ByVal insets As Insets, ByVal bounds As Rectangle) As Rectangle
			bounds = treeState.getBounds(path, bounds)
			If bounds IsNot Nothing Then
				If leftToRight Then
					bounds.x += insets.left
				Else
					bounds.x = tree.width - (bounds.x + bounds.width) - insets.right
				End If
				bounds.y += insets.top
			End If
			Return bounds
		End Function

		''' <summary>
		''' Returns the path for passed in row.  If row is not visible
		''' null is returned.
		''' </summary>
		Public Overridable Function getPathForRow(ByVal tree As JTree, ByVal row As Integer) As TreePath
			Return If(treeState IsNot Nothing, treeState.getPathForRow(row), Nothing)
		End Function

		''' <summary>
		''' Returns the row that the last item identified in path is visible
		''' at.  Will return -1 if any of the elements in path are not
		''' currently visible.
		''' </summary>
		Public Overridable Function getRowForPath(ByVal tree As JTree, ByVal path As TreePath) As Integer
			Return If(treeState IsNot Nothing, treeState.getRowForPath(path), -1)
		End Function

		''' <summary>
		''' Returns the number of rows that are being displayed.
		''' </summary>
		Public Overridable Function getRowCount(ByVal tree As JTree) As Integer
			Return If(treeState IsNot Nothing, treeState.rowCount, 0)
		End Function

		''' <summary>
		''' Returns the path to the node that is closest to x,y.  If
		''' there is nothing currently visible this will return null, otherwise
		''' it'll always return a valid path.  If you need to test if the
		''' returned object is exactly at x, y you should get the bounds for
		''' the returned path and test x, y against that.
		''' </summary>
		Public Overridable Function getClosestPathForLocation(ByVal tree As JTree, ByVal x As Integer, ByVal y As Integer) As TreePath
			If tree IsNot Nothing AndAlso treeState IsNot Nothing Then
				' TreeState doesn't care about the x location, hence it isn't
				' adjusted
				y -= tree.insets.top
				Return treeState.getPathClosestTo(x, y)
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Returns true if the tree is being edited.  The item that is being
		''' edited can be returned by getEditingPath().
		''' </summary>
		Public Overridable Function isEditing(ByVal tree As JTree) As Boolean
			Return (editingComponent IsNot Nothing)
		End Function

		''' <summary>
		''' Stops the current editing session.  This has no effect if the
		''' tree isn't being edited.  Returns true if the editor allows the
		''' editing session to stop.
		''' </summary>
		Public Overridable Function stopEditing(ByVal tree As JTree) As Boolean
			If editingComponent IsNot Nothing AndAlso cellEditor.stopCellEditing() Then
				completeEditing(False, False, True)
				Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Cancels the current editing session.
		''' </summary>
		Public Overridable Sub cancelEditing(ByVal tree As JTree)
			If editingComponent IsNot Nothing Then completeEditing(False, True, False)
		End Sub

		''' <summary>
		''' Selects the last item in path and tries to edit it.  Editing will
		''' fail if the CellEditor won't allow it for the selected item.
		''' </summary>
		Public Overridable Sub startEditingAtPath(ByVal tree As JTree, ByVal path As TreePath)
			tree.scrollPathToVisible(path)
			If path IsNot Nothing AndAlso tree.isVisible(path) Then startEditing(path, Nothing)
		End Sub

		''' <summary>
		''' Returns the path to the element that is being edited.
		''' </summary>
		Public Overridable Function getEditingPath(ByVal tree As JTree) As TreePath
			Return editingPath
		End Function

		'
		' Install methods
		'

		Public Overridable Sub installUI(ByVal c As JComponent)
			If c Is Nothing Then Throw New NullPointerException("null component passed to BasicTreeUI.installUI()")

			tree = CType(c, JTree)

			prepareForUIInstall()

			' Boilerplate install block
			installDefaults()
			installKeyboardActions()
			installComponents()
			installListeners()

			completeUIInstall()
		End Sub

		''' <summary>
		''' Invoked after the <code>tree</code> instance variable has been
		''' set, but before any defaults/listeners have been installed.
		''' </summary>
		Protected Friend Overridable Sub prepareForUIInstall()
			drawingCache = New Dictionary(Of TreePath, Boolean?)(7)

			' Data member initializations
			leftToRight = BasicGraphicsUtils.isLeftToRight(tree)
			stopEditingInCompleteEditing = True
			lastSelectedRow = -1
			leadRow = -1
			preferredSize = New Dimension

			largeModel = tree.largeModel
			If rowHeight <= 0 Then largeModel = False
			model = tree.model
		End Sub

		''' <summary>
		''' Invoked from installUI after all the defaults/listeners have been
		''' installed.
		''' </summary>
		Protected Friend Overridable Sub completeUIInstall()
			' Custom install code

			Me.showsRootHandles = tree.showsRootHandles

			updateRenderer()

			updateDepthOffset()

			selectionModel = tree.selectionModel

			' Create, if necessary, the TreeState instance.
			treeState = createLayoutCache()
			configureLayoutCache()

			updateSize()
		End Sub

		Protected Friend Overridable Sub installDefaults()
			If tree.background Is Nothing OrElse TypeOf tree.background Is javax.swing.plaf.UIResource Then tree.background = UIManager.getColor("Tree.background")
			If hashColor Is Nothing OrElse TypeOf hashColor Is javax.swing.plaf.UIResource Then hashColor = UIManager.getColor("Tree.hash")
			If tree.font Is Nothing OrElse TypeOf tree.font Is javax.swing.plaf.UIResource Then tree.font = UIManager.getFont("Tree.font")
			' JTree's original row height is 16.  To correctly display the
			' contents on Linux we should have set it to 18, Windows 19 and
			' Solaris 20.  As these values vary so much it's too hard to
			' be backward compatable and try to update the row height, we're
			' therefor NOT going to adjust the row height based on font.  If the
			' developer changes the font, it's there responsibility to update
			' the row height.

			expandedIcon = CType(UIManager.get("Tree.expandedIcon"), Icon)
			collapsedIcon = CType(UIManager.get("Tree.collapsedIcon"), Icon)

			leftChildIndent = CInt(Fix(UIManager.get("Tree.leftChildIndent")))
			rightChildIndent = CInt(Fix(UIManager.get("Tree.rightChildIndent")))

			LookAndFeel.installProperty(tree, "rowHeight", UIManager.get("Tree.rowHeight"))

			largeModel = (tree.largeModel AndAlso tree.rowHeight > 0)

			Dim scrollsOnExpand As Object = UIManager.get("Tree.scrollsOnExpand")
			If scrollsOnExpand IsNot Nothing Then LookAndFeel.installProperty(tree, "scrollsOnExpand", scrollsOnExpand)

			paintLines = UIManager.getBoolean("Tree.paintLines")
			lineTypeDashed = UIManager.getBoolean("Tree.lineTypeDashed")

			Dim l As Long? = CLng(Fix(UIManager.get("Tree.timeFactor")))
			timeFactor = If(l IsNot Nothing, l, 1000L)

			Dim ___showsRootHandles As Object = UIManager.get("Tree.showsRootHandles")
			If ___showsRootHandles IsNot Nothing Then LookAndFeel.installProperty(tree, JTree.SHOWS_ROOT_HANDLES_PROPERTY, ___showsRootHandles)
		End Sub

		Protected Friend Overridable Sub installListeners()
			propertyChangeListener = createPropertyChangeListener()
			If propertyChangeListener IsNot Nothing Then tree.addPropertyChangeListener(propertyChangeListener)
			mouseListener = createMouseListener()
			If mouseListener IsNot Nothing Then
				tree.addMouseListener(mouseListener)
				If TypeOf mouseListener Is MouseMotionListener Then tree.addMouseMotionListener(CType(mouseListener, MouseMotionListener))
			End If
			focusListener = createFocusListener()
			If focusListener IsNot Nothing Then tree.addFocusListener(focusListener)
			keyListener = createKeyListener()
			If keyListener IsNot Nothing Then tree.addKeyListener(keyListener)
			treeExpansionListener = createTreeExpansionListener()
			If treeExpansionListener IsNot Nothing Then tree.addTreeExpansionListener(treeExpansionListener)
			treeModelListener = createTreeModelListener()
			If treeModelListener IsNot Nothing AndAlso treeModel IsNot Nothing Then treeModel.addTreeModelListener(treeModelListener)
			selectionModelPropertyChangeListener = createSelectionModelPropertyChangeListener()
			If selectionModelPropertyChangeListener IsNot Nothing AndAlso treeSelectionModel IsNot Nothing Then treeSelectionModel.addPropertyChangeListener(selectionModelPropertyChangeListener)
			treeSelectionListener = createTreeSelectionListener()
			If treeSelectionListener IsNot Nothing AndAlso treeSelectionModel IsNot Nothing Then treeSelectionModel.addTreeSelectionListener(treeSelectionListener)

			Dim th As TransferHandler = tree.transferHandler
			If th Is Nothing OrElse TypeOf th Is javax.swing.plaf.UIResource Then
				tree.transferHandler = defaultTransferHandler
				' default TransferHandler doesn't support drop
				' so we don't want drop handling
				If TypeOf tree.dropTarget Is javax.swing.plaf.UIResource Then tree.dropTarget = Nothing
			End If

			LookAndFeel.installProperty(tree, "opaque", Boolean.TRUE)
		End Sub

		Protected Friend Overridable Sub installKeyboardActions()
			Dim km As InputMap = getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT)

			SwingUtilities.replaceUIInputMap(tree, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, km)
			km = getInputMap(JComponent.WHEN_FOCUSED)
			SwingUtilities.replaceUIInputMap(tree, JComponent.WHEN_FOCUSED, km)

			LazyActionMap.installLazyActionMap(tree, GetType(BasicTreeUI), "Tree.actionMap")
		End Sub

		Friend Overridable Function getInputMap(ByVal condition As Integer) As InputMap
			If condition = JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT Then
				Return CType(sun.swing.DefaultLookup.get(tree, Me, "Tree.ancestorInputMap"), InputMap)
			ElseIf condition = JComponent.WHEN_FOCUSED Then
				Dim keyMap As InputMap = CType(sun.swing.DefaultLookup.get(tree, Me, "Tree.focusInputMap"), InputMap)
				Dim rtlKeyMap As InputMap

				rtlKeyMap = CType(sun.swing.DefaultLookup.get(tree, Me, "Tree.focusInputMap.RightToLeft"), InputMap)
				If tree.componentOrientation.leftToRight OrElse (rtlKeyMap Is Nothing) Then
					Return keyMap
				Else
					rtlKeyMap.parent = keyMap
					Return rtlKeyMap
				End If
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Intalls the subcomponents of the tree, which is the renderer pane.
		''' </summary>
		Protected Friend Overridable Sub installComponents()
			rendererPane = createCellRendererPane()
			If rendererPane IsNot Nothing Then tree.add(rendererPane)
		End Sub

		'
		' Create methods.
		'

		''' <summary>
		''' Creates an instance of NodeDimensions that is able to determine
		''' the size of a given node in the tree.
		''' </summary>
		Protected Friend Overridable Function createNodeDimensions() As AbstractLayoutCache.NodeDimensions
			Return New NodeDimensionsHandler(Me)
		End Function

		''' <summary>
		''' Creates a listener that is responsible that updates the UI based on
		''' how the tree changes.
		''' </summary>
		Protected Friend Overridable Function createPropertyChangeListener() As PropertyChangeListener
			Return handler
		End Function

		Private Property handler As Handler
			Get
				If handler Is Nothing Then handler = New Handler(Me)
				Return handler
			End Get
		End Property

		''' <summary>
		''' Creates the listener responsible for updating the selection based on
		''' mouse events.
		''' </summary>
		Protected Friend Overridable Function createMouseListener() As MouseListener
			Return handler
		End Function

		''' <summary>
		''' Creates a listener that is responsible for updating the display
		''' when focus is lost/gained.
		''' </summary>
		Protected Friend Overridable Function createFocusListener() As FocusListener
			Return handler
		End Function

		''' <summary>
		''' Creates the listener reponsible for getting key events from
		''' the tree.
		''' </summary>
		Protected Friend Overridable Function createKeyListener() As KeyListener
			Return handler
		End Function

		''' <summary>
		''' Creates the listener responsible for getting property change
		''' events from the selection model.
		''' </summary>
		Protected Friend Overridable Function createSelectionModelPropertyChangeListener() As PropertyChangeListener
			Return handler
		End Function

		''' <summary>
		''' Creates the listener that updates the display based on selection change
		''' methods.
		''' </summary>
		Protected Friend Overridable Function createTreeSelectionListener() As TreeSelectionListener
			Return handler
		End Function

		''' <summary>
		''' Creates a listener to handle events from the current editor.
		''' </summary>
		Protected Friend Overridable Function createCellEditorListener() As CellEditorListener
			Return handler
		End Function

		''' <summary>
		''' Creates and returns a new ComponentHandler. This is used for
		''' the large model to mark the validCachedPreferredSize as invalid
		''' when the component moves.
		''' </summary>
		Protected Friend Overridable Function createComponentListener() As ComponentListener
			Return New ComponentHandler(Me)
		End Function

		''' <summary>
		''' Creates and returns the object responsible for updating the treestate
		''' when nodes expanded state changes.
		''' </summary>
		Protected Friend Overridable Function createTreeExpansionListener() As TreeExpansionListener
			Return handler
		End Function

		''' <summary>
		''' Creates the object responsible for managing what is expanded, as
		''' well as the size of nodes.
		''' </summary>
		Protected Friend Overridable Function createLayoutCache() As AbstractLayoutCache
			If largeModel AndAlso rowHeight > 0 Then Return New FixedHeightLayoutCache
			Return New VariableHeightLayoutCache
		End Function

		''' <summary>
		''' Returns the renderer pane that renderer components are placed in.
		''' </summary>
		Protected Friend Overridable Function createCellRendererPane() As CellRendererPane
			Return New CellRendererPane
		End Function

		''' <summary>
		''' Creates a default cell editor.
		''' </summary>
		Protected Friend Overridable Function createDefaultCellEditor() As TreeCellEditor
			If currentCellRenderer IsNot Nothing AndAlso (TypeOf currentCellRenderer Is DefaultTreeCellRenderer) Then
				Dim editor As New DefaultTreeCellEditor(tree, CType(currentCellRenderer, DefaultTreeCellRenderer))

				Return editor
			End If
			Return New DefaultTreeCellEditor(tree, Nothing)
		End Function

		''' <summary>
		''' Returns the default cell renderer that is used to do the
		''' stamping of each node.
		''' </summary>
		Protected Friend Overridable Function createDefaultCellRenderer() As TreeCellRenderer
			Return New DefaultTreeCellRenderer
		End Function

		''' <summary>
		''' Returns a listener that can update the tree when the model changes.
		''' </summary>
		Protected Friend Overridable Function createTreeModelListener() As TreeModelListener
			Return handler
		End Function

		'
		' Uninstall methods
		'

		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			completeEditing()

			prepareForUIUninstall()

			uninstallDefaults()
			uninstallListeners()
			uninstallKeyboardActions()
			uninstallComponents()

			completeUIUninstall()
		End Sub

		Protected Friend Overridable Sub prepareForUIUninstall()
		End Sub

		Protected Friend Overridable Sub completeUIUninstall()
			If createdRenderer Then tree.cellRenderer = Nothing
			If createdCellEditor Then tree.cellEditor = Nothing
			cellEditor = Nothing
			currentCellRenderer = Nothing
			rendererPane = Nothing
			componentListener = Nothing
			propertyChangeListener = Nothing
			mouseListener = Nothing
			focusListener = Nothing
			keyListener = Nothing
			selectionModel = Nothing
			treeState = Nothing
			drawingCache = Nothing
			selectionModelPropertyChangeListener = Nothing
			tree = Nothing
			treeModel = Nothing
			treeSelectionModel = Nothing
			treeSelectionListener = Nothing
			treeExpansionListener = Nothing
		End Sub

		Protected Friend Overridable Sub uninstallDefaults()
			If TypeOf tree.transferHandler Is javax.swing.plaf.UIResource Then tree.transferHandler = Nothing
		End Sub

		Protected Friend Overridable Sub uninstallListeners()
			If componentListener IsNot Nothing Then tree.removeComponentListener(componentListener)
			If propertyChangeListener IsNot Nothing Then tree.removePropertyChangeListener(propertyChangeListener)
			If mouseListener IsNot Nothing Then
				tree.removeMouseListener(mouseListener)
				If TypeOf mouseListener Is MouseMotionListener Then tree.removeMouseMotionListener(CType(mouseListener, MouseMotionListener))
			End If
			If focusListener IsNot Nothing Then tree.removeFocusListener(focusListener)
			If keyListener IsNot Nothing Then tree.removeKeyListener(keyListener)
			If treeExpansionListener IsNot Nothing Then tree.removeTreeExpansionListener(treeExpansionListener)
			If treeModel IsNot Nothing AndAlso treeModelListener IsNot Nothing Then treeModel.removeTreeModelListener(treeModelListener)
			If selectionModelPropertyChangeListener IsNot Nothing AndAlso treeSelectionModel IsNot Nothing Then treeSelectionModel.removePropertyChangeListener(selectionModelPropertyChangeListener)
			If treeSelectionListener IsNot Nothing AndAlso treeSelectionModel IsNot Nothing Then treeSelectionModel.removeTreeSelectionListener(treeSelectionListener)
			handler = Nothing
		End Sub

		Protected Friend Overridable Sub uninstallKeyboardActions()
			SwingUtilities.replaceUIActionMap(tree, Nothing)
			SwingUtilities.replaceUIInputMap(tree, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, Nothing)
			SwingUtilities.replaceUIInputMap(tree, JComponent.WHEN_FOCUSED, Nothing)
		End Sub

		''' <summary>
		''' Uninstalls the renderer pane.
		''' </summary>
		Protected Friend Overridable Sub uninstallComponents()
			If rendererPane IsNot Nothing Then tree.remove(rendererPane)
		End Sub

		''' <summary>
		''' Recomputes the right margin, and invalidates any tree states
		''' </summary>
		Private Sub redoTheLayout()
			If treeState IsNot Nothing Then treeState.invalidateSizes()
		End Sub

		''' <summary>
		''' Returns the baseline.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaseline(ByVal c As JComponent, ByVal width As Integer, ByVal height As Integer) As Integer
			MyBase.getBaseline(c, width, height)
			Dim lafDefaults As UIDefaults = UIManager.lookAndFeelDefaults
			Dim renderer As Component = CType(lafDefaults(BASELINE_COMPONENT_KEY), Component)
			If renderer Is Nothing Then
				Dim tcr As TreeCellRenderer = createDefaultCellRenderer()
				renderer = tcr.getTreeCellRendererComponent(tree, "a", False, False, False, -1, False)
				lafDefaults(BASELINE_COMPONENT_KEY) = renderer
			End If
			Dim ___rowHeight As Integer = tree.rowHeight
			Dim ___baseline As Integer
			If ___rowHeight > 0 Then
				___baseline = renderer.getBaseline(Integer.MaxValue, ___rowHeight)
			Else
				Dim pref As Dimension = renderer.preferredSize
				___baseline = renderer.getBaseline(pref.width, pref.height)
			End If
			Return ___baseline + tree.insets.top
		End Function

		''' <summary>
		''' Returns an enum indicating how the baseline of the component
		''' changes as the size changes.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaselineResizeBehavior(ByVal c As JComponent) As Component.BaselineResizeBehavior
			MyBase.getBaselineResizeBehavior(c)
			Return Component.BaselineResizeBehavior.CONSTANT_ASCENT
		End Function

		'
		' Painting routines.
		'

		Public Overridable Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			If tree IsNot c Then Throw New InternalError("incorrect component")

			' Should never happen if installed for a UI
			If treeState Is Nothing Then Return

			Dim paintBounds As Rectangle = g.clipBounds
			Dim insets As Insets = tree.insets
			Dim initialPath As TreePath = getClosestPathForLocation(tree, 0, paintBounds.y)
			Dim paintingEnumerator As System.Collections.IEnumerator = treeState.getVisiblePathsFrom(initialPath)
			Dim row As Integer = treeState.getRowForPath(initialPath)
			Dim endY As Integer = paintBounds.y + paintBounds.height

			drawingCache.Clear()

			If initialPath IsNot Nothing AndAlso paintingEnumerator IsNot Nothing Then
				Dim parentPath As TreePath = initialPath

				' Draw the lines, knobs, and rows

				' Find each parent and have them draw a line to their last child
				parentPath = parentPath.parentPath
				Do While parentPath IsNot Nothing
					paintVerticalPartOfLeg(g, paintBounds, insets, parentPath)
					drawingCache(parentPath) = Boolean.TRUE
					parentPath = parentPath.parentPath
				Loop

				Dim done As Boolean = False
				' Information for the node being rendered.
				Dim isExpanded As Boolean
				Dim hasBeenExpanded As Boolean
				Dim isLeaf As Boolean
				Dim boundsBuffer As New Rectangle
				Dim bounds As Rectangle
				Dim path As TreePath
				Dim ___rootVisible As Boolean = rootVisible

				Do While (Not done) AndAlso paintingEnumerator.hasMoreElements()
					path = CType(paintingEnumerator.nextElement(), TreePath)
					If path IsNot Nothing Then
						isLeaf = treeModel.isLeaf(path.lastPathComponent)
						If isLeaf Then
								hasBeenExpanded = False
								isExpanded = hasBeenExpanded
						Else
							isExpanded = treeState.getExpandedState(path)
							hasBeenExpanded = tree.hasBeenExpanded(path)
						End If
						bounds = getPathBounds(path, insets, boundsBuffer)
						If bounds Is Nothing Then Return
						' See if the vertical line to the parent has been drawn.
						parentPath = path.parentPath
						If parentPath IsNot Nothing Then
							If drawingCache(parentPath) Is Nothing Then
								paintVerticalPartOfLeg(g, paintBounds, insets, parentPath)
								drawingCache(parentPath) = Boolean.TRUE
							End If
							paintHorizontalPartOfLeg(g, paintBounds, insets, bounds, path, row, isExpanded, hasBeenExpanded, isLeaf)
						ElseIf ___rootVisible AndAlso row = 0 Then
							paintHorizontalPartOfLeg(g, paintBounds, insets, bounds, path, row, isExpanded, hasBeenExpanded, isLeaf)
						End If
						If shouldPaintExpandControl(path, row, isExpanded, hasBeenExpanded, isLeaf) Then paintExpandControl(g, paintBounds, insets, bounds, path, row, isExpanded, hasBeenExpanded, isLeaf)
						paintRow(g, paintBounds, insets, bounds, path, row, isExpanded, hasBeenExpanded, isLeaf)
						If (bounds.y + bounds.height) >= endY Then done = True
					Else
						done = True
					End If
					row += 1
				Loop
			End If

			paintDropLine(g)

			' Empty out the renderer pane, allowing renderers to be gc'ed.
			rendererPane.removeAll()

			drawingCache.Clear()
		End Sub

		''' <summary>
		''' Tells if a {@code DropLocation} should be indicated by a line between
		''' nodes. This is meant for {@code javax.swing.DropMode.INSERT} and
		''' {@code javax.swing.DropMode.ON_OR_INSERT} drop modes.
		''' </summary>
		''' <param name="loc"> a {@code DropLocation} </param>
		''' <returns> {@code true} if the drop location should be shown as a line
		''' @since 1.7 </returns>
		Protected Friend Overridable Function isDropLine(ByVal loc As JTree.DropLocation) As Boolean
			Return loc IsNot Nothing AndAlso loc.path IsNot Nothing AndAlso loc.childIndex <> -1
		End Function

		''' <summary>
		''' Paints the drop line.
		''' </summary>
		''' <param name="g"> {@code Graphics} object to draw on
		''' @since 1.7 </param>
		Protected Friend Overridable Sub paintDropLine(ByVal g As Graphics)
			Dim loc As JTree.DropLocation = tree.dropLocation
			If Not isDropLine(loc) Then Return

			Dim c As Color = UIManager.getColor("Tree.dropLineColor")
			If c IsNot Nothing Then
				g.color = c
				Dim rect As Rectangle = getDropLineRect(loc)
				g.fillRect(rect.x, rect.y, rect.width, rect.height)
			End If
		End Sub

		''' <summary>
		''' Returns a unbounding box for the drop line.
		''' </summary>
		''' <param name="loc"> a {@code DropLocation} </param>
		''' <returns> bounding box for the drop line
		''' @since 1.7 </returns>
		Protected Friend Overridable Function getDropLineRect(ByVal loc As JTree.DropLocation) As Rectangle
			Dim rect As Rectangle
			Dim path As TreePath = loc.path
			Dim index As Integer = loc.childIndex
			Dim ltr As Boolean = leftToRight

			Dim insets As Insets = tree.insets

			If tree.rowCount = 0 Then
				rect = New Rectangle(insets.left, insets.top, tree.width - insets.left - insets.right, 0)
			Else
				Dim ___model As TreeModel = model
				Dim root As Object = ___model.root

				If path.lastPathComponent Is root AndAlso index >= ___model.getChildCount(root) Then

					rect = tree.getRowBounds(tree.rowCount - 1)
					rect.y = rect.y + rect.height
					Dim xRect As Rectangle

					If Not tree.rootVisible Then
						xRect = tree.getRowBounds(0)
					ElseIf ___model.getChildCount(root) = 0 Then
						xRect = tree.getRowBounds(0)
						xRect.x += totalChildIndent
						xRect.width -= totalChildIndent + totalChildIndent
					Else
						Dim ___lastChildPath As TreePath = path.pathByAddingChild(___model.getChild(root, ___model.getChildCount(root) - 1))
						xRect = tree.getPathBounds(___lastChildPath)
					End If

					rect.x = xRect.x
					rect.width = xRect.width
				Else
					rect = tree.getPathBounds(path.pathByAddingChild(___model.getChild(path.lastPathComponent, index)))
				End If
			End If

			If rect.y <> 0 Then rect.y -= 1

			If Not ltr Then rect.x = rect.x + rect.width - 100

			rect.width = 100
			rect.height = 2

			Return rect
		End Function

		''' <summary>
		''' Paints the horizontal part of the leg. The receiver should
		''' NOT modify <code>clipBounds</code>, or <code>insets</code>.<p>
		''' NOTE: <code>parentRow</code> can be -1 if the root is not visible.
		''' </summary>
		Protected Friend Overridable Sub paintHorizontalPartOfLeg(ByVal g As Graphics, ByVal clipBounds As Rectangle, ByVal insets As Insets, ByVal bounds As Rectangle, ByVal path As TreePath, ByVal row As Integer, ByVal isExpanded As Boolean, ByVal hasBeenExpanded As Boolean, ByVal isLeaf As Boolean)
			If Not paintLines Then Return

			' Don't paint the legs for the root'ish node if the
			Dim depth As Integer = path.pathCount - 1
			If (depth = 0 OrElse (depth = 1 AndAlso (Not rootVisible))) AndAlso (Not showsRootHandles) Then Return

			Dim clipLeft As Integer = clipBounds.x
			Dim clipRight As Integer = clipBounds.x + clipBounds.width
			Dim clipTop As Integer = clipBounds.y
			Dim clipBottom As Integer = clipBounds.y + clipBounds.height
			Dim lineY As Integer = bounds.y + bounds.height / 2

			If leftToRight Then
				Dim leftX As Integer = bounds.x - rightChildIndent
				Dim nodeX As Integer = bounds.x - horizontalLegBuffer

				If lineY >= clipTop AndAlso lineY < clipBottom AndAlso nodeX >= clipLeft AndAlso leftX < clipRight AndAlso leftX < nodeX Then

					g.color = hashColor
					paintHorizontalLine(g, tree, lineY, leftX, nodeX - 1)
				End If
			Else
				Dim nodeX As Integer = bounds.x + bounds.width + horizontalLegBuffer
				Dim rightX As Integer = bounds.x + bounds.width + rightChildIndent

				If lineY >= clipTop AndAlso lineY < clipBottom AndAlso rightX >= clipLeft AndAlso nodeX < clipRight AndAlso nodeX < rightX Then

					g.color = hashColor
					paintHorizontalLine(g, tree, lineY, nodeX, rightX - 1)
				End If
			End If
		End Sub

		''' <summary>
		''' Paints the vertical part of the leg. The receiver should
		''' NOT modify <code>clipBounds</code>, <code>insets</code>.
		''' </summary>
		Protected Friend Overridable Sub paintVerticalPartOfLeg(ByVal g As Graphics, ByVal clipBounds As Rectangle, ByVal insets As Insets, ByVal path As TreePath)
			If Not paintLines Then Return

			Dim depth As Integer = path.pathCount - 1
			If depth = 0 AndAlso (Not showsRootHandles) AndAlso (Not rootVisible) Then Return
			Dim lineX As Integer = getRowX(-1, depth + 1)
			If leftToRight Then
				lineX = lineX - rightChildIndent + insets.left
			Else
				lineX = tree.width - lineX - insets.right + rightChildIndent - 1
			End If
			Dim clipLeft As Integer = clipBounds.x
			Dim clipRight As Integer = clipBounds.x + (clipBounds.width - 1)

			If lineX >= clipLeft AndAlso lineX <= clipRight Then
				Dim clipTop As Integer = clipBounds.y
				Dim clipBottom As Integer = clipBounds.y + clipBounds.height
				Dim parentBounds As Rectangle = getPathBounds(tree, path)
				Dim lastChildBounds As Rectangle = getPathBounds(tree, getLastChildPath(path))

				If lastChildBounds Is Nothing Then Return

				Dim top As Integer

				If parentBounds Is Nothing Then
					top = Math.Max(insets.top + verticalLegBuffer, clipTop)
				Else
					top = Math.Max(parentBounds.y + parentBounds.height + verticalLegBuffer, clipTop)
				End If
				If depth = 0 AndAlso (Not rootVisible) Then
					Dim ___model As TreeModel = model

					If ___model IsNot Nothing Then
						Dim root As Object = ___model.root

						If ___model.getChildCount(root) > 0 Then
							parentBounds = getPathBounds(tree, path.pathByAddingChild(___model.getChild(root, 0)))
							If parentBounds IsNot Nothing Then top = Math.Max(insets.top + verticalLegBuffer, parentBounds.y + parentBounds.height / 2)
						End If
					End If
				End If

				Dim bottom As Integer = Math.Min(lastChildBounds.y + (lastChildBounds.height / 2), clipBottom)

				If top <= bottom Then
					g.color = hashColor
					paintVerticalLine(g, tree, lineX, top, bottom)
				End If
			End If
		End Sub

		''' <summary>
		''' Paints the expand (toggle) part of a row. The receiver should
		''' NOT modify <code>clipBounds</code>, or <code>insets</code>.
		''' </summary>
		Protected Friend Overridable Sub paintExpandControl(ByVal g As Graphics, ByVal clipBounds As Rectangle, ByVal insets As Insets, ByVal bounds As Rectangle, ByVal path As TreePath, ByVal row As Integer, ByVal isExpanded As Boolean, ByVal hasBeenExpanded As Boolean, ByVal isLeaf As Boolean)
			Dim value As Object = path.lastPathComponent

			' Draw icons if not a leaf and either hasn't been loaded,
			' or the model child count is > 0.
			If (Not isLeaf) AndAlso ((Not hasBeenExpanded) OrElse treeModel.getChildCount(value) > 0) Then
				Dim middleXOfKnob As Integer
				If leftToRight Then
					middleXOfKnob = bounds.x - rightChildIndent + 1
				Else
					middleXOfKnob = bounds.x + bounds.width + rightChildIndent - 1
				End If
				Dim middleYOfKnob As Integer = bounds.y + (bounds.height / 2)

				If isExpanded Then
					Dim ___expandedIcon As Icon = expandedIcon
					If ___expandedIcon IsNot Nothing Then drawCentered(tree, g, ___expandedIcon, middleXOfKnob, middleYOfKnob)
				Else
					Dim ___collapsedIcon As Icon = collapsedIcon
					If ___collapsedIcon IsNot Nothing Then drawCentered(tree, g, ___collapsedIcon, middleXOfKnob, middleYOfKnob)
				End If
			End If
		End Sub

		''' <summary>
		''' Paints the renderer part of a row. The receiver should
		''' NOT modify <code>clipBounds</code>, or <code>insets</code>.
		''' </summary>
		Protected Friend Overridable Sub paintRow(ByVal g As Graphics, ByVal clipBounds As Rectangle, ByVal insets As Insets, ByVal bounds As Rectangle, ByVal path As TreePath, ByVal row As Integer, ByVal isExpanded As Boolean, ByVal hasBeenExpanded As Boolean, ByVal isLeaf As Boolean)
			' Don't paint the renderer if editing this row.
			If editingComponent IsNot Nothing AndAlso editingRow = row Then Return

			Dim leadIndex As Integer

			If tree.hasFocus() Then
				leadIndex = leadSelectionRow
			Else
				leadIndex = -1
			End If

			Dim component As Component

			component = currentCellRenderer.getTreeCellRendererComponent(tree, path.lastPathComponent, tree.isRowSelected(row), isExpanded, isLeaf, row, (leadIndex = row))

			rendererPane.paintComponent(g, component, tree, bounds.x, bounds.y, bounds.width, bounds.height, True)
		End Sub

		''' <summary>
		''' Returns true if the expand (toggle) control should be drawn for
		''' the specified row.
		''' </summary>
		Protected Friend Overridable Function shouldPaintExpandControl(ByVal path As TreePath, ByVal row As Integer, ByVal isExpanded As Boolean, ByVal hasBeenExpanded As Boolean, ByVal isLeaf As Boolean) As Boolean
			If isLeaf Then Return False

			Dim depth As Integer = path.pathCount - 1

			If (depth = 0 OrElse (depth = 1 AndAlso (Not rootVisible))) AndAlso (Not showsRootHandles) Then Return False
			Return True
		End Function

		''' <summary>
		''' Paints a vertical line.
		''' </summary>
		Protected Friend Overridable Sub paintVerticalLine(ByVal g As Graphics, ByVal c As JComponent, ByVal x As Integer, ByVal top As Integer, ByVal bottom As Integer)
			If lineTypeDashed Then
				drawDashedVerticalLine(g, x, top, bottom)
			Else
				g.drawLine(x, top, x, bottom)
			End If
		End Sub

		''' <summary>
		''' Paints a horizontal line.
		''' </summary>
		Protected Friend Overridable Sub paintHorizontalLine(ByVal g As Graphics, ByVal c As JComponent, ByVal y As Integer, ByVal left As Integer, ByVal right As Integer)
			If lineTypeDashed Then
				drawDashedHorizontalLine(g, y, left, right)
			Else
				g.drawLine(left, y, right, y)
			End If
		End Sub

		''' <summary>
		''' The vertical element of legs between nodes starts at the bottom of the
		''' parent node by default.  This method makes the leg start below that.
		''' </summary>
		Protected Friend Overridable Property verticalLegBuffer As Integer
			Get
				Return 0
			End Get
		End Property

		''' <summary>
		''' The horizontal element of legs between nodes starts at the
		''' right of the left-hand side of the child node by default.  This
		''' method makes the leg end before that.
		''' </summary>
		Protected Friend Overridable Property horizontalLegBuffer As Integer
			Get
				Return 0
			End Get
		End Property

		Private Function findCenteredX(ByVal x As Integer, ByVal iconWidth As Integer) As Integer
			Return If(leftToRight, x - CInt(Fix(Math.Ceiling(iconWidth / 2.0))), x - CInt(Fix(Math.Floor(iconWidth / 2.0))))
		End Function

		'
		' Generic painting methods
		'

		' Draws the icon centered at (x,y)
		Protected Friend Overridable Sub drawCentered(ByVal c As Component, ByVal graphics As Graphics, ByVal icon As Icon, ByVal x As Integer, ByVal y As Integer)
			icon.paintIcon(c, graphics, findCenteredX(x, icon.iconWidth), y - icon.iconHeight \ 2)
		End Sub

		' This method is slow -- revisit when Java2D is ready.
		' assumes x1 <= x2
		Protected Friend Overridable Sub drawDashedHorizontalLine(ByVal g As Graphics, ByVal y As Integer, ByVal x1 As Integer, ByVal x2 As Integer)
			' Drawing only even coordinates helps join line segments so they
			' appear as one line.  This can be defeated by translating the
			' Graphics by an odd amount.
			x1 += (x1 Mod 2)

			For x As Integer = x1 To x2 Step 2
				g.drawLine(x, y, x, y)
			Next x
		End Sub

		' This method is slow -- revisit when Java2D is ready.
		' assumes y1 <= y2
		Protected Friend Overridable Sub drawDashedVerticalLine(ByVal g As Graphics, ByVal x As Integer, ByVal y1 As Integer, ByVal y2 As Integer)
			' Drawing only even coordinates helps join line segments so they
			' appear as one line.  This can be defeated by translating the
			' Graphics by an odd amount.
			y1 += (y1 Mod 2)

			For y As Integer = y1 To y2 Step 2
				g.drawLine(x, y, x, y)
			Next y
		End Sub

		'
		' Various local methods
		'

		''' <summary>
		''' Returns the location, along the x-axis, to render a particular row
		''' at. The return value does not include any Insets specified on the JTree.
		''' This does not check for the validity of the row or depth, it is assumed
		''' to be correct and will not throw an Exception if the row or depth
		''' doesn't match that of the tree.
		''' </summary>
		''' <param name="row"> Row to return x location for </param>
		''' <param name="depth"> Depth of the row </param>
		''' <returns> amount to indent the given row.
		''' @since 1.5 </returns>
		Protected Friend Overridable Function getRowX(ByVal row As Integer, ByVal depth As Integer) As Integer
			Return totalChildIndent * (depth + depthOffset)
		End Function

		''' <summary>
		''' Makes all the nodes that are expanded in JTree expanded in LayoutCache.
		''' This invokes updateExpandedDescendants with the root path.
		''' </summary>
		Protected Friend Overridable Sub updateLayoutCacheExpandedNodes()
			If treeModel IsNot Nothing AndAlso treeModel.root IsNot Nothing Then updateExpandedDescendants(New TreePath(treeModel.root))
		End Sub

		Private Sub updateLayoutCacheExpandedNodesIfNecessary()
			If treeModel IsNot Nothing AndAlso treeModel.root IsNot Nothing Then
				Dim rootPath As New TreePath(treeModel.root)
				If tree.isExpanded(rootPath) Then
					updateLayoutCacheExpandedNodes()
				Else
					treeState.expandedStateate(rootPath, False)
				End If
			End If
		End Sub

		''' <summary>
		''' Updates the expanded state of all the descendants of <code>path</code>
		''' by getting the expanded descendants from the tree and forwarding
		''' to the tree state.
		''' </summary>
		Protected Friend Overridable Sub updateExpandedDescendants(ByVal path As TreePath)
			completeEditing()
			If treeState IsNot Nothing Then
				treeState.expandedStateate(path, True)

				Dim descendants As System.Collections.IEnumerator = tree.getExpandedDescendants(path)

				If descendants IsNot Nothing Then
					Do While descendants.hasMoreElements()
						path = CType(descendants.nextElement(), TreePath)
						treeState.expandedStateate(path, True)
					Loop
				End If
				updateLeadSelectionRow()
				updateSize()
			End If
		End Sub

		''' <summary>
		''' Returns a path to the last child of <code>parent</code>.
		''' </summary>
		Protected Friend Overridable Function getLastChildPath(ByVal parent As TreePath) As TreePath
			If treeModel IsNot Nothing Then
				Dim childCount As Integer = treeModel.getChildCount(parent.lastPathComponent)

				If childCount > 0 Then Return parent.pathByAddingChild(treeModel.getChild(parent.lastPathComponent, childCount - 1))
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Updates how much each depth should be offset by.
		''' </summary>
		Protected Friend Overridable Sub updateDepthOffset()
			If rootVisible Then
				If showsRootHandles Then
					depthOffset = 1
				Else
					depthOffset = 0
				End If
			ElseIf Not showsRootHandles Then
				depthOffset = -1
			Else
				depthOffset = 0
			End If
		End Sub

		''' <summary>
		''' Updates the cellEditor based on the editability of the JTree that
		''' we're contained in.  If the tree is editable but doesn't have a
		''' cellEditor, a basic one will be used.
		''' </summary>
		Protected Friend Overridable Sub updateCellEditor()
			Dim newEditor As TreeCellEditor

			completeEditing()
			If tree Is Nothing Then
				newEditor = Nothing
			Else
				If tree.editable Then
					newEditor = tree.cellEditor
					If newEditor Is Nothing Then
						newEditor = createDefaultCellEditor()
						If newEditor IsNot Nothing Then
							tree.cellEditor = newEditor
							createdCellEditor = True
						End If
					End If
				Else
					newEditor = Nothing
				End If
			End If
			If newEditor IsNot cellEditor Then
				If cellEditor IsNot Nothing AndAlso cellEditorListener IsNot Nothing Then cellEditor.removeCellEditorListener(cellEditorListener)
				cellEditor = newEditor
				If cellEditorListener Is Nothing Then cellEditorListener = createCellEditorListener()
				If newEditor IsNot Nothing AndAlso cellEditorListener IsNot Nothing Then newEditor.addCellEditorListener(cellEditorListener)
				createdCellEditor = False
			End If
		End Sub

		''' <summary>
		''' Messaged from the tree we're in when the renderer has changed.
		''' </summary>
		Protected Friend Overridable Sub updateRenderer()
			If tree IsNot Nothing Then
				Dim newCellRenderer As TreeCellRenderer

				newCellRenderer = tree.cellRenderer
				If newCellRenderer Is Nothing Then
					tree.cellRenderer = createDefaultCellRenderer()
					createdRenderer = True
				Else
					createdRenderer = False
					currentCellRenderer = newCellRenderer
					If createdCellEditor Then tree.cellEditor = Nothing
				End If
			Else
				createdRenderer = False
				currentCellRenderer = Nothing
			End If
			updateCellEditor()
		End Sub

		''' <summary>
		''' Resets the TreeState instance based on the tree we're providing the
		''' look and feel for.
		''' </summary>
		Protected Friend Overridable Sub configureLayoutCache()
			If treeState IsNot Nothing AndAlso tree IsNot Nothing Then
				If nodeDimensions Is Nothing Then nodeDimensions = createNodeDimensions()
				treeState.nodeDimensions = nodeDimensions
				treeState.rootVisible = tree.rootVisible
				treeState.rowHeight = tree.rowHeight
				treeState.selectionModel = selectionModel
				' Only do this if necessary, may loss state if call with
				' same model as it currently has.
				If treeState.model IsNot tree.model Then treeState.model = tree.model
				updateLayoutCacheExpandedNodesIfNecessary()
				' Create a listener to update preferred size when bounds
				' changes, if necessary.
				If largeModel Then
					If componentListener Is Nothing Then
						componentListener = createComponentListener()
						If componentListener IsNot Nothing Then tree.addComponentListener(componentListener)
					End If
				ElseIf componentListener IsNot Nothing Then
					tree.removeComponentListener(componentListener)
					componentListener = Nothing
				End If
			ElseIf componentListener IsNot Nothing Then
				tree.removeComponentListener(componentListener)
				componentListener = Nothing
			End If
		End Sub

		''' <summary>
		''' Marks the cached size as being invalid, and messages the
		''' tree with <code>treeDidChange</code>.
		''' </summary>
		Protected Friend Overridable Sub updateSize()
			validCachedPreferredSize = False
			tree.treeDidChange()
		End Sub

		Private Sub updateSize0()
			validCachedPreferredSize = False
			tree.revalidate()
		End Sub

		''' <summary>
		''' Updates the <code>preferredSize</code> instance variable,
		''' which is returned from <code>getPreferredSize()</code>.<p>
		''' For left to right orientations, the size is determined from the
		''' current AbstractLayoutCache. For RTL orientations, the preferred size
		''' becomes the width minus the minimum x position.
		''' </summary>
		Protected Friend Overridable Sub updateCachedPreferredSize()
			If treeState IsNot Nothing Then
				Dim i As Insets = tree.insets

				If largeModel Then
					Dim visRect As Rectangle = tree.visibleRect

					If visRect.x = 0 AndAlso visRect.y = 0 AndAlso visRect.width = 0 AndAlso visRect.height = 0 AndAlso tree.visibleRowCount > 0 Then
						' The tree doesn't have a valid bounds yet. Calculate
						' based on visible row count.
						visRect.width = 1
						visRect.height = tree.rowHeight * tree.visibleRowCount
					Else
						visRect.x -= i.left
						visRect.y -= i.top
					End If
					' we should consider a non-visible area above
					Dim component As Component = SwingUtilities.getUnwrappedParent(tree)
					If TypeOf component Is JViewport Then
						component = component.parent
						If TypeOf component Is JScrollPane Then
							Dim pane As JScrollPane = CType(component, JScrollPane)
							Dim bar As JScrollBar = pane.horizontalScrollBar
							If (bar IsNot Nothing) AndAlso bar.visible Then
								Dim height As Integer = bar.height
								visRect.y -= height
								visRect.height += height
							End If
						End If
					End If
					preferredSize.width = treeState.getPreferredWidth(visRect)
				Else
					preferredSize.width = treeState.getPreferredWidth(Nothing)
				End If
				preferredSize.height = treeState.preferredHeight
				preferredSize.width += i.left + i.right
				preferredSize.height += i.top + i.bottom
			End If
			validCachedPreferredSize = True
		End Sub

		''' <summary>
		''' Messaged from the VisibleTreeNode after it has been expanded.
		''' </summary>
		Protected Friend Overridable Sub pathWasExpanded(ByVal path As TreePath)
			If tree IsNot Nothing Then tree.fireTreeExpanded(path)
		End Sub

		''' <summary>
		''' Messaged from the VisibleTreeNode after it has collapsed.
		''' </summary>
		Protected Friend Overridable Sub pathWasCollapsed(ByVal path As TreePath)
			If tree IsNot Nothing Then tree.fireTreeCollapsed(path)
		End Sub

		''' <summary>
		''' Ensures that the rows identified by beginRow through endRow are
		''' visible.
		''' </summary>
		Protected Friend Overridable Sub ensureRowsAreVisible(ByVal beginRow As Integer, ByVal endRow As Integer)
			If tree IsNot Nothing AndAlso beginRow >= 0 AndAlso endRow < getRowCount(tree) Then
				Dim scrollVert As Boolean = sun.swing.DefaultLookup.getBoolean(tree, Me, "Tree.scrollsHorizontallyAndVertically", False)
				If beginRow = endRow Then
					Dim scrollBounds As Rectangle = getPathBounds(tree, getPathForRow(tree, beginRow))

					If scrollBounds IsNot Nothing Then
						If Not scrollVert Then
							scrollBounds.x = tree.visibleRect.x
							scrollBounds.width = 1
						End If
						tree.scrollRectToVisible(scrollBounds)
					End If
				Else
					Dim beginRect As Rectangle = getPathBounds(tree, getPathForRow(tree, beginRow))
					If beginRect IsNot Nothing Then
						Dim visRect As Rectangle = tree.visibleRect
						Dim testRect As Rectangle = beginRect
						Dim beginY As Integer = beginRect.y
						Dim maxY As Integer = beginY + visRect.height

						For counter As Integer = beginRow + 1 To endRow
								testRect = getPathBounds(tree, getPathForRow(tree, counter))
							If testRect Is Nothing Then Return
							If (testRect.y + testRect.height) > maxY Then counter = endRow
						Next counter
							tree.scrollRectToVisible(New Rectangle(visRect.x, beginY, 1, testRect.y + testRect.height- beginY))
					End If
				End If
			End If
		End Sub

		''' <summary>
		''' Sets the preferred minimum size.
		''' </summary>
		Public Overridable Property preferredMinSize As Dimension
			Set(ByVal newSize As Dimension)
				preferredMinSize = newSize
			End Set
			Get
				If preferredMinSize Is Nothing Then Return Nothing
				Return New Dimension(preferredMinSize)
			End Get
		End Property


		''' <summary>
		''' Returns the preferred size to properly display the tree,
		''' this is a cover method for getPreferredSize(c, true).
		''' </summary>
		Public Overridable Function getPreferredSize(ByVal c As JComponent) As Dimension
			Return getPreferredSize(c, True)
		End Function

		''' <summary>
		''' Returns the preferred size to represent the tree in
		''' <I>c</I>.  If <I>checkConsistency</I> is true
		''' <b>checkConsistency</b> is messaged first.
		''' </summary>
		Public Overridable Function getPreferredSize(ByVal c As JComponent, ByVal checkConsistency As Boolean) As Dimension
			Dim pSize As Dimension = Me.preferredMinSize

			If Not validCachedPreferredSize Then updateCachedPreferredSize()
			If tree IsNot Nothing Then
				If pSize IsNot Nothing Then Return New Dimension(Math.Max(pSize.width, preferredSize.width), Math.Max(pSize.height, preferredSize.height))
				Return New Dimension(preferredSize.width, preferredSize.height)
			ElseIf pSize IsNot Nothing Then
				Return pSize
			Else
				Return New Dimension(0, 0)
			End If
		End Function

		''' <summary>
		''' Returns the minimum size for this component.  Which will be
		''' the min preferred size or 0, 0.
		''' </summary>
		Public Overridable Function getMinimumSize(ByVal c As JComponent) As Dimension
			If Me.preferredMinSize IsNot Nothing Then Return Me.preferredMinSize
			Return New Dimension(0, 0)
		End Function

		''' <summary>
		''' Returns the maximum size for this component, which will be the
		''' preferred size if the instance is currently in a JTree, or 0, 0.
		''' </summary>
		Public Overridable Function getMaximumSize(ByVal c As JComponent) As Dimension
			If tree IsNot Nothing Then Return getPreferredSize(tree)
			If Me.preferredMinSize IsNot Nothing Then Return Me.preferredMinSize
			Return New Dimension(0, 0)
		End Function


		''' <summary>
		''' Messages to stop the editing session. If the UI the receiver
		''' is providing the look and feel for returns true from
		''' <code>getInvokesStopCellEditing</code>, stopCellEditing will
		''' invoked on the current editor. Then completeEditing will
		''' be messaged with false, true, false to cancel any lingering
		''' editing.
		''' </summary>
		Protected Friend Overridable Sub completeEditing()
			' If should invoke stopCellEditing, try that 
			If tree.invokesStopCellEditing AndAlso stopEditingInCompleteEditing AndAlso editingComponent IsNot Nothing Then cellEditor.stopCellEditing()
	'         Invoke cancelCellEditing, this will do nothing if stopCellEditing
	'           was successful. 
			completeEditing(False, True, False)
		End Sub

		''' <summary>
		''' Stops the editing session.  If messageStop is true the editor
		''' is messaged with stopEditing, if messageCancel is true the
		''' editor is messaged with cancelEditing. If messageTree is true
		''' the treeModel is messaged with valueForPathChanged.
		''' </summary>
		Protected Friend Overridable Sub completeEditing(ByVal messageStop As Boolean, ByVal messageCancel As Boolean, ByVal messageTree As Boolean)
			If stopEditingInCompleteEditing AndAlso editingComponent IsNot Nothing Then
				Dim oldComponent As Component = editingComponent
				Dim oldPath As TreePath = editingPath
				Dim oldEditor As TreeCellEditor = cellEditor
				Dim newValue As Object = oldEditor.cellEditorValue
				Dim editingBounds As Rectangle = getPathBounds(tree, editingPath)
				Dim requestFocus As Boolean = (tree IsNot Nothing AndAlso (tree.hasFocus() OrElse SwingUtilities.findFocusOwner(editingComponent) IsNot Nothing))

				editingComponent = Nothing
				editingPath = Nothing
				If messageStop Then
					oldEditor.stopCellEditing()
				ElseIf messageCancel Then
					oldEditor.cancelCellEditing()
				End If
				tree.remove(oldComponent)
				If editorHasDifferentSize Then
					treeState.invalidatePathBounds(oldPath)
					updateSize()
				ElseIf editingBounds IsNot Nothing Then
					editingBounds.x = 0
					editingBounds.width = tree.size.width
					tree.repaint(editingBounds)
				End If
				If requestFocus Then tree.requestFocus()
				If messageTree Then treeModel.valueForPathChanged(oldPath, newValue)
			End If
		End Sub

		' cover method for startEditing that allows us to pass extra
		' information into that method via a class variable
		Private Function startEditingOnRelease(ByVal path As TreePath, ByVal [event] As MouseEvent, ByVal releaseEvent As MouseEvent) As Boolean
			Me.releaseEvent = releaseEvent
			Try
				Return startEditing(path, [event])
			Finally
				Me.releaseEvent = Nothing
			End Try
		End Function

		''' <summary>
		''' Will start editing for node if there is a cellEditor and
		''' shouldSelectCell returns true.<p>
		''' This assumes that path is valid and visible.
		''' </summary>
		Protected Friend Overridable Function startEditing(ByVal path As TreePath, ByVal [event] As MouseEvent) As Boolean
			If isEditing(tree) AndAlso tree.invokesStopCellEditing AndAlso (Not stopEditing(tree)) Then Return False
			completeEditing()
			If cellEditor IsNot Nothing AndAlso tree.isPathEditable(path) Then
				Dim row As Integer = getRowForPath(tree, path)

				If cellEditor.isCellEditable([event]) Then
					editingComponent = cellEditor.getTreeCellEditorComponent(tree, path.lastPathComponent, tree.isPathSelected(path), tree.isExpanded(path), treeModel.isLeaf(path.lastPathComponent), row)
					Dim nodeBounds As Rectangle = getPathBounds(tree, path)
					If nodeBounds Is Nothing Then Return False

					editingRow = row

					Dim editorSize As Dimension = editingComponent.preferredSize

					' Only allow odd heights if explicitly set.
					If editorSize.height <> nodeBounds.height AndAlso rowHeight > 0 Then editorSize.height = rowHeight

					If editorSize.width <> nodeBounds.width OrElse editorSize.height <> nodeBounds.height Then
						' Editor wants different width or height, invalidate
						' treeState and relayout.
						editorHasDifferentSize = True
						treeState.invalidatePathBounds(path)
						updateSize()
						' To make sure x/y are updated correctly, fetch
						' the bounds again.
						nodeBounds = getPathBounds(tree, path)
						If nodeBounds Is Nothing Then Return False
					Else
						editorHasDifferentSize = False
					End If
					tree.add(editingComponent)
					editingComponent.boundsnds(nodeBounds.x, nodeBounds.y, nodeBounds.width, nodeBounds.height)
					editingPath = path
					sun.awt.AWTAccessor.componentAccessor.revalidateSynchronously(editingComponent)
					editingComponent.repaint()
					If cellEditor.shouldSelectCell([event]) Then
						stopEditingInCompleteEditing = False
						tree.selectionRow = row
						stopEditingInCompleteEditing = True
					End If

					Dim focusedComponent As Component = sun.swing.SwingUtilities2.0 compositeRequestFocus(editingComponent)
					Dim selectAll As Boolean = True

					If [event] IsNot Nothing Then
	'                     Find the component that will get forwarded all the
	'                       mouse events until mouseReleased. 
						Dim componentPoint As Point = SwingUtilities.convertPoint(tree, New Point([event].x, [event].y), editingComponent)

	'                     Create an instance of BasicTreeMouseListener to handle
	'                       passing the mouse/motion events to the necessary
	'                       component. 
						' We really want similar behavior to getMouseEventTarget,
						' but it is package private.
						Dim activeComponent As Component = SwingUtilities.getDeepestComponentAt(editingComponent, componentPoint.x, componentPoint.y)
						If activeComponent IsNot Nothing Then
							Dim ___handler As New MouseInputHandler(Me, tree, activeComponent, [event], focusedComponent)

							If releaseEvent IsNot Nothing Then ___handler.mouseReleased(releaseEvent)

							selectAll = False
						End If
					End If
					If selectAll AndAlso TypeOf focusedComponent Is JTextField Then CType(focusedComponent, JTextField).selectAll()
					Return True
				Else
					editingComponent = Nothing
				End If
			End If
			Return False
		End Function

		'
		' Following are primarily for handling mouse events.
		'

		''' <summary>
		''' If the <code>mouseX</code> and <code>mouseY</code> are in the
		''' expand/collapse region of the <code>row</code>, this will toggle
		''' the row.
		''' </summary>
		Protected Friend Overridable Sub checkForClickInExpandControl(ByVal path As TreePath, ByVal mouseX As Integer, ByVal mouseY As Integer)
		  If isLocationInExpandControl(path, mouseX, mouseY) Then handleExpandControlClick(path, mouseX, mouseY)
		End Sub

		''' <summary>
		''' Returns true if <code>mouseX</code> and <code>mouseY</code> fall
		''' in the area of row that is used to expand/collapse the node and
		''' the node at <code>row</code> does not represent a leaf.
		''' </summary>
		Protected Friend Overridable Function isLocationInExpandControl(ByVal path As TreePath, ByVal mouseX As Integer, ByVal mouseY As Integer) As Boolean
			If path IsNot Nothing AndAlso (Not treeModel.isLeaf(path.lastPathComponent)) Then
				Dim boxWidth As Integer
				Dim i As Insets = tree.insets

				If expandedIcon IsNot Nothing Then
					boxWidth = expandedIcon.iconWidth
				Else
					boxWidth = 8
				End If

				Dim boxLeftX As Integer = getRowX(tree.getRowForPath(path), path.pathCount - 1)

				If leftToRight Then
					boxLeftX = boxLeftX + i.left - rightChildIndent + 1
				Else
					boxLeftX = tree.width - boxLeftX - i.right + rightChildIndent - 1
				End If

				boxLeftX = findCenteredX(boxLeftX, boxWidth)

				Return (mouseX >= boxLeftX AndAlso mouseX < (boxLeftX + boxWidth))
			End If
			Return False
		End Function

		''' <summary>
		''' Messaged when the user clicks the particular row, this invokes
		''' toggleExpandState.
		''' </summary>
		Protected Friend Overridable Sub handleExpandControlClick(ByVal path As TreePath, ByVal mouseX As Integer, ByVal mouseY As Integer)
			toggleExpandState(path)
		End Sub

		''' <summary>
		''' Expands path if it is not expanded, or collapses row if it is expanded.
		''' If expanding a path and JTree scrolls on expand, ensureRowsAreVisible
		''' is invoked to scroll as many of the children to visible as possible
		''' (tries to scroll to last visible descendant of path).
		''' </summary>
		Protected Friend Overridable Sub toggleExpandState(ByVal path As TreePath)
			If Not tree.isExpanded(path) Then
				Dim row As Integer = getRowForPath(tree, path)

				tree.expandPath(path)
				updateSize()
				If row <> -1 Then
					If tree.scrollsOnExpand Then
						ensureRowsAreVisible(row, row + treeState.getVisibleChildCount(path))
					Else
						ensureRowsAreVisible(row, row)
					End If
				End If
			Else
				tree.collapsePath(path)
				updateSize()
			End If
		End Sub

		''' <summary>
		''' Returning true signifies a mouse event on the node should toggle
		''' the selection of only the row under mouse.
		''' </summary>
		Protected Friend Overridable Function isToggleSelectionEvent(ByVal [event] As MouseEvent) As Boolean
			Return (SwingUtilities.isLeftMouseButton([event]) AndAlso BasicGraphicsUtils.isMenuShortcutKeyDown([event]))
		End Function

		''' <summary>
		''' Returning true signifies a mouse event on the node should select
		''' from the anchor point.
		''' </summary>
		Protected Friend Overridable Function isMultiSelectEvent(ByVal [event] As MouseEvent) As Boolean
			Return (SwingUtilities.isLeftMouseButton([event]) AndAlso [event].shiftDown)
		End Function

		''' <summary>
		''' Returning true indicates the row under the mouse should be toggled
		''' based on the event. This is invoked after checkForClickInExpandControl,
		''' implying the location is not in the expand (toggle) control
		''' </summary>
		Protected Friend Overridable Function isToggleEvent(ByVal [event] As MouseEvent) As Boolean
			If Not SwingUtilities.isLeftMouseButton([event]) Then Return False
			Dim clickCount As Integer = tree.toggleClickCount

			If clickCount <= 0 Then Return False
			Return (([event].clickCount Mod clickCount) = 0)
		End Function

		''' <summary>
		''' Messaged to update the selection based on a MouseEvent over a
		''' particular row. If the event is a toggle selection event, the
		''' row is either selected, or deselected. If the event identifies
		''' a multi selection event, the selection is updated from the
		''' anchor point. Otherwise the row is selected, and if the event
		''' specified a toggle event the row is expanded/collapsed.
		''' </summary>
		Protected Friend Overridable Sub selectPathForEvent(ByVal path As TreePath, ByVal [event] As MouseEvent)
			' Adjust from the anchor point. 
			If isMultiSelectEvent([event]) Then
				Dim anchor As TreePath = anchorSelectionPath
				Dim anchorRow As Integer = If(anchor Is Nothing, -1, getRowForPath(tree, anchor))

				If anchorRow = -1 OrElse tree.selectionModel.selectionMode = TreeSelectionModel.SINGLE_TREE_SELECTION Then
					tree.selectionPath = path
				Else
					Dim row As Integer = getRowForPath(tree, path)
					Dim lastAnchorPath As TreePath = anchor

					If isToggleSelectionEvent([event]) Then
						If tree.isRowSelected(anchorRow) Then
							tree.addSelectionInterval(anchorRow, row)
						Else
							tree.removeSelectionInterval(anchorRow, row)
							tree.addSelectionInterval(row, row)
						End If
					ElseIf row < anchorRow Then
						tree.selectionIntervalval(row, anchorRow)
					Else
						tree.selectionIntervalval(anchorRow, row)
					End If
					lastSelectedRow = row
					anchorSelectionPath = lastAnchorPath
					leadSelectionPath = path
				End If

			' Should this event toggle the selection of this row?
			' Control toggles just this node. 
			ElseIf isToggleSelectionEvent([event]) Then
				If tree.isPathSelected(path) Then
					tree.removeSelectionPath(path)
				Else
					tree.addSelectionPath(path)
				End If
				lastSelectedRow = getRowForPath(tree, path)
				anchorSelectionPath = path
				leadSelectionPath = path

			' Otherwise set the selection to just this interval. 
			ElseIf SwingUtilities.isLeftMouseButton([event]) Then
				tree.selectionPath = path
				If isToggleEvent([event]) Then toggleExpandState(path)
			End If
		End Sub

		''' <returns> true if the node at <code>row</code> is a leaf. </returns>
		Protected Friend Overridable Function isLeaf(ByVal row As Integer) As Boolean
			Dim path As TreePath = getPathForRow(tree, row)

			If path IsNot Nothing Then Return treeModel.isLeaf(path.lastPathComponent)
			' Have to return something here...
			Return True
		End Function

		'
		' The following selection methods (lead/anchor) are covers for the
		' methods in JTree.
		'
		Private Property anchorSelectionPath As TreePath
			Set(ByVal newPath As TreePath)
				ignoreLAChange = True
				Try
					tree.anchorSelectionPath = newPath
				Finally
					ignoreLAChange = False
				End Try
			End Set
			Get
				Return tree.anchorSelectionPath
			End Get
		End Property


'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Private Sub setLeadSelectionPath(ByVal newPath As TreePath) 'JavaToDotNetTempPropertySetleadSelectionPath
		Private Property leadSelectionPath As TreePath
			Set(ByVal newPath As TreePath)
				leadSelectionPathath(newPath, False)
			End Set
			Get
		End Property

		Private Sub setLeadSelectionPath(ByVal newPath As TreePath, ByVal repaint As Boolean)
			Dim bounds As Rectangle = If(repaint, getPathBounds(tree, leadSelectionPath), Nothing)

			ignoreLAChange = True
			Try
				tree.leadSelectionPath = newPath
			Finally
				ignoreLAChange = False
			End Try
			leadRow = getRowForPath(tree, newPath)

			If repaint Then
				If bounds IsNot Nothing Then tree.repaint(getRepaintPathBounds(bounds))
				bounds = getPathBounds(tree, newPath)
				If bounds IsNot Nothing Then tree.repaint(getRepaintPathBounds(bounds))
			End If
		End Sub

		Private Function getRepaintPathBounds(ByVal bounds As Rectangle) As Rectangle
			If UIManager.getBoolean("Tree.repaintWholeRow") Then
			   bounds.x = 0
			   bounds.width = tree.width
			End If
			Return bounds
		End Function

			Return tree.leadSelectionPath
		End Function

		''' <summary>
		''' Updates the lead row of the selection.
		''' @since 1.7
		''' </summary>
		Protected Friend Overridable Sub updateLeadSelectionRow()
			leadRow = getRowForPath(tree, leadSelectionPath)
		End Sub

		''' <summary>
		''' Returns the lead row of the selection.
		''' </summary>
		''' <returns> selection lead row
		''' @since 1.7 </returns>
		Protected Friend Overridable Property leadSelectionRow As Integer
			Get
				Return leadRow
			End Get
		End Property

		''' <summary>
		''' Extends the selection from the anchor to make <code>newLead</code>
		''' the lead of the selection. This does not scroll.
		''' </summary>
		Private Sub extendSelection(ByVal newLead As TreePath)
			Dim aPath As TreePath = anchorSelectionPath
			Dim aRow As Integer = If(aPath Is Nothing, -1, getRowForPath(tree, aPath))
			Dim newIndex As Integer = getRowForPath(tree, newLead)

			If aRow = -1 Then
				tree.selectionRow = newIndex
			Else
				If aRow < newIndex Then
					tree.selectionIntervalval(aRow, newIndex)
				Else
					tree.selectionIntervalval(newIndex, aRow)
				End If
				anchorSelectionPath = aPath
				leadSelectionPath = newLead
			End If
		End Sub

		''' <summary>
		''' Invokes <code>repaint</code> on the JTree for the passed in TreePath,
		''' <code>path</code>.
		''' </summary>
		Private Sub repaintPath(ByVal path As TreePath)
			If path IsNot Nothing Then
				Dim bounds As Rectangle = getPathBounds(tree, path)
				If bounds IsNot Nothing Then tree.repaint(bounds.x, bounds.y, bounds.width, bounds.height)
			End If
		End Sub

		''' <summary>
		''' Updates the TreeState in response to nodes expanding/collapsing.
		''' </summary>
		Public Class TreeExpansionHandler
			Implements TreeExpansionListener

			Private ReadOnly outerInstance As BasicTreeUI

			Public Sub New(ByVal outerInstance As BasicTreeUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.

			''' <summary>
			''' Called whenever an item in the tree has been expanded.
			''' </summary>
			Public Overridable Sub treeExpanded(ByVal [event] As TreeExpansionEvent) Implements TreeExpansionListener.treeExpanded
				outerInstance.handler.treeExpanded([event])
			End Sub

			''' <summary>
			''' Called whenever an item in the tree has been collapsed.
			''' </summary>
			Public Overridable Sub treeCollapsed(ByVal [event] As TreeExpansionEvent) Implements TreeExpansionListener.treeCollapsed
				outerInstance.handler.treeCollapsed([event])
			End Sub
		End Class ' BasicTreeUI.TreeExpansionHandler


		''' <summary>
		''' Updates the preferred size when scrolling (if necessary).
		''' </summary>
		Public Class ComponentHandler
			Inherits ComponentAdapter
			Implements ActionListener

			Private ReadOnly outerInstance As BasicTreeUI

			Public Sub New(ByVal outerInstance As BasicTreeUI)
				Me.outerInstance = outerInstance
			End Sub

			''' <summary>
			''' Timer used when inside a scrollpane and the scrollbar is
			''' adjusting. 
			''' </summary>
			Protected Friend ___timer As Timer
			''' <summary>
			''' ScrollBar that is being adjusted. </summary>
			Protected Friend scrollBar As JScrollBar

			Public Overridable Sub componentMoved(ByVal e As ComponentEvent)
				If ___timer Is Nothing Then
					Dim ___scrollPane As JScrollPane = scrollPane

					If ___scrollPane Is Nothing Then
						outerInstance.updateSize()
					Else
						scrollBar = ___scrollPane.verticalScrollBar
						If scrollBar Is Nothing OrElse (Not scrollBar.valueIsAdjusting) Then
							' Try the horizontal scrollbar.
							scrollBar = ___scrollPane.horizontalScrollBar
							If scrollBar IsNot Nothing AndAlso scrollBar.valueIsAdjusting Then
								startTimer()
							Else
								outerInstance.updateSize()
							End If
						Else
							startTimer()
						End If
					End If
				End If
			End Sub

			''' <summary>
			''' Creates, if necessary, and starts a Timer to check if need to
			''' resize the bounds.
			''' </summary>
			Protected Friend Overridable Sub startTimer()
				If ___timer Is Nothing Then
					___timer = New Timer(200, Me)
					___timer.repeats = True
				End If
				___timer.start()
			End Sub

			''' <summary>
			''' Returns the JScrollPane housing the JTree, or null if one isn't
			''' found.
			''' </summary>
			Protected Friend Overridable Property scrollPane As JScrollPane
				Get
					Dim c As Component = outerInstance.tree.parent
    
					Do While c IsNot Nothing AndAlso Not(TypeOf c Is JScrollPane)
						c = c.parent
					Loop
					If TypeOf c Is JScrollPane Then Return CType(c, JScrollPane)
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Public as a result of Timer. If the scrollBar is null, or
			''' not adjusting, this stops the timer and updates the sizing.
			''' </summary>
			Public Overridable Sub actionPerformed(ByVal ae As ActionEvent)
				If scrollBar Is Nothing OrElse (Not scrollBar.valueIsAdjusting) Then
					If ___timer IsNot Nothing Then ___timer.stop()
					outerInstance.updateSize()
					___timer = Nothing
					scrollBar = Nothing
				End If
			End Sub
		End Class ' End of BasicTreeUI.ComponentHandler


		''' <summary>
		''' Forwards all TreeModel events to the TreeState.
		''' </summary>
		Public Class TreeModelHandler
			Implements TreeModelListener

			Private ReadOnly outerInstance As BasicTreeUI

			Public Sub New(ByVal outerInstance As BasicTreeUI)
				Me.outerInstance = outerInstance
			End Sub


			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.

			Public Overridable Sub treeNodesChanged(ByVal e As TreeModelEvent) Implements TreeModelListener.treeNodesChanged
				outerInstance.handler.treeNodesChanged(e)
			End Sub

			Public Overridable Sub treeNodesInserted(ByVal e As TreeModelEvent) Implements TreeModelListener.treeNodesInserted
				outerInstance.handler.treeNodesInserted(e)
			End Sub

			Public Overridable Sub treeNodesRemoved(ByVal e As TreeModelEvent) Implements TreeModelListener.treeNodesRemoved
				outerInstance.handler.treeNodesRemoved(e)
			End Sub

			Public Overridable Sub treeStructureChanged(ByVal e As TreeModelEvent) Implements TreeModelListener.treeStructureChanged
				outerInstance.handler.treeStructureChanged(e)
			End Sub
		End Class ' End of BasicTreeUI.TreeModelHandler


		''' <summary>
		''' Listens for changes in the selection model and updates the display
		''' accordingly.
		''' </summary>
		Public Class TreeSelectionHandler
			Implements TreeSelectionListener

			Private ReadOnly outerInstance As BasicTreeUI

			Public Sub New(ByVal outerInstance As BasicTreeUI)
				Me.outerInstance = outerInstance
			End Sub


			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.

			''' <summary>
			''' Messaged when the selection changes in the tree we're displaying
			''' for.  Stops editing, messages super and displays the changed paths.
			''' </summary>
			Public Overridable Sub valueChanged(ByVal [event] As TreeSelectionEvent) Implements TreeSelectionListener.valueChanged
				outerInstance.handler.valueChanged([event])
			End Sub
		End Class ' End of BasicTreeUI.TreeSelectionHandler


		''' <summary>
		''' Listener responsible for getting cell editing events and updating
		''' the tree accordingly.
		''' </summary>
		Public Class CellEditorHandler
			Implements CellEditorListener

			Private ReadOnly outerInstance As BasicTreeUI

			Public Sub New(ByVal outerInstance As BasicTreeUI)
				Me.outerInstance = outerInstance
			End Sub


			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.

			''' <summary>
			''' Messaged when editing has stopped in the tree. </summary>
			Public Overridable Sub editingStopped(ByVal e As ChangeEvent)
				outerInstance.handler.editingStopped(e)
			End Sub

			''' <summary>
			''' Messaged when editing has been canceled in the tree. </summary>
			Public Overridable Sub editingCanceled(ByVal e As ChangeEvent)
				outerInstance.handler.editingCanceled(e)
			End Sub
		End Class ' BasicTreeUI.CellEditorHandler


		''' <summary>
		''' This is used to get multiple key down events to appropriately generate
		''' events.
		''' </summary>
		Public Class KeyHandler
			Inherits KeyAdapter

			Private ReadOnly outerInstance As BasicTreeUI

			Public Sub New(ByVal outerInstance As BasicTreeUI)
				Me.outerInstance = outerInstance
			End Sub


			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.

			' Also note these fields aren't use anymore, nor does Handler have
			' the old functionality. This behavior worked around an old bug
			' in JComponent that has long since been fixed.

			''' <summary>
			''' Key code that is being generated for. </summary>
			Protected Friend repeatKeyAction As Action

			''' <summary>
			''' Set to true while keyPressed is active. </summary>
			Protected Friend isKeyDown As Boolean

			''' <summary>
			''' Invoked when a key has been typed.
			''' 
			''' Moves the keyboard focus to the first element
			''' whose first letter matches the alphanumeric key
			''' pressed by the user. Subsequent same key presses
			''' move the keyboard focus to the next object that
			''' starts with the same letter.
			''' </summary>
			Public Overridable Sub keyTyped(ByVal e As KeyEvent)
				outerInstance.handler.keyTyped(e)
			End Sub

			Public Overridable Sub keyPressed(ByVal e As KeyEvent)
				outerInstance.handler.keyPressed(e)
			End Sub

			Public Overridable Sub keyReleased(ByVal e As KeyEvent)
				outerInstance.handler.keyReleased(e)
			End Sub
		End Class ' End of BasicTreeUI.KeyHandler


		''' <summary>
		''' Repaints the lead selection row when focus is lost/gained.
		''' </summary>
		Public Class FocusHandler
			Implements FocusListener

			Private ReadOnly outerInstance As BasicTreeUI

			Public Sub New(ByVal outerInstance As BasicTreeUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.

			''' <summary>
			''' Invoked when focus is activated on the tree we're in, redraws the
			''' lead row.
			''' </summary>
			Public Overridable Sub focusGained(ByVal e As FocusEvent)
				outerInstance.handler.focusGained(e)
			End Sub

			''' <summary>
			''' Invoked when focus is activated on the tree we're in, redraws the
			''' lead row.
			''' </summary>
			Public Overridable Sub focusLost(ByVal e As FocusEvent)
				outerInstance.handler.focusLost(e)
			End Sub
		End Class ' End of class BasicTreeUI.FocusHandler


		''' <summary>
		''' Class responsible for getting size of node, method is forwarded
		''' to BasicTreeUI method. X location does not include insets, that is
		''' handled in getPathBounds.
		''' </summary>
		' This returns locations that don't include any Insets.
		Public Class NodeDimensionsHandler
			Inherits AbstractLayoutCache.NodeDimensions

			Private ReadOnly outerInstance As BasicTreeUI

			Public Sub New(ByVal outerInstance As BasicTreeUI)
				Me.outerInstance = outerInstance
			End Sub

			''' <summary>
			''' Responsible for getting the size of a particular node.
			''' </summary>
			Public Overridable Function getNodeDimensions(ByVal value As Object, ByVal row As Integer, ByVal depth As Integer, ByVal expanded As Boolean, ByVal size As Rectangle) As Rectangle
				' Return size of editing component, if editing and asking
				' for editing row.
				If outerInstance.editingComponent IsNot Nothing AndAlso outerInstance.editingRow = row Then
					Dim prefSize As Dimension = outerInstance.editingComponent.preferredSize
					Dim rh As Integer = outerInstance.rowHeight

					If rh > 0 AndAlso rh <> prefSize.height Then prefSize.height = rh
					If size IsNot Nothing Then
						size.x = getRowX(row, depth)
						size.width = prefSize.width
						size.height = prefSize.height
					Else
						size = New Rectangle(getRowX(row, depth), 0, prefSize.width, prefSize.height)
					End If
					Return size
				End If
				' Not editing, use renderer.
				If outerInstance.currentCellRenderer IsNot Nothing Then
					Dim aComponent As Component

					aComponent = outerInstance.currentCellRenderer.getTreeCellRendererComponent(outerInstance.tree, value, outerInstance.tree.isRowSelected(row), expanded, outerInstance.treeModel.isLeaf(value), row, False)
					If outerInstance.tree IsNot Nothing Then
						' Only ever removed when UI changes, this is OK!
						outerInstance.rendererPane.add(aComponent)
						aComponent.validate()
					End If
					Dim prefSize As Dimension = aComponent.preferredSize

					If size IsNot Nothing Then
						size.x = getRowX(row, depth)
						size.width = prefSize.width
						size.height = prefSize.height
					Else
						size = New Rectangle(getRowX(row, depth), 0, prefSize.width, prefSize.height)
					End If
					Return size
				End If
				Return Nothing
			End Function

			''' <returns> amount to indent the given row. </returns>
			Protected Friend Overridable Function getRowX(ByVal row As Integer, ByVal depth As Integer) As Integer
				Return outerInstance.getRowX(row, depth)
			End Function

		End Class ' End of class BasicTreeUI.NodeDimensionsHandler


		''' <summary>
		''' TreeMouseListener is responsible for updating the selection
		''' based on mouse events.
		''' </summary>
		Public Class MouseHandler
			Inherits MouseAdapter
			Implements MouseMotionListener

			Private ReadOnly outerInstance As BasicTreeUI

			Public Sub New(ByVal outerInstance As BasicTreeUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.

			''' <summary>
			''' Invoked when a mouse button has been pressed on a component.
			''' </summary>
			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				outerInstance.handler.mousePressed(e)
			End Sub

			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
				outerInstance.handler.mouseDragged(e)
			End Sub

			''' <summary>
			''' Invoked when the mouse button has been moved on a component
			''' (with no buttons no down).
			''' @since 1.4
			''' </summary>
			Public Overridable Sub mouseMoved(ByVal e As MouseEvent)
				outerInstance.handler.mouseMoved(e)
			End Sub

			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
				outerInstance.handler.mouseReleased(e)
			End Sub
		End Class ' End of BasicTreeUI.MouseHandler


		''' <summary>
		''' PropertyChangeListener for the tree. Updates the appropriate
		''' variable, or TreeState, based on what changes.
		''' </summary>
		Public Class PropertyChangeHandler
			Implements PropertyChangeListener

			Private ReadOnly outerInstance As BasicTreeUI

			Public Sub New(ByVal outerInstance As BasicTreeUI)
				Me.outerInstance = outerInstance
			End Sub


			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.

			Public Overridable Sub propertyChange(ByVal [event] As PropertyChangeEvent)
				outerInstance.handler.propertyChange([event])
			End Sub
		End Class ' End of BasicTreeUI.PropertyChangeHandler


		''' <summary>
		''' Listener on the TreeSelectionModel, resets the row selection if
		''' any of the properties of the model change.
		''' </summary>
		Public Class SelectionModelPropertyChangeHandler
			Implements PropertyChangeListener

			Private ReadOnly outerInstance As BasicTreeUI

			Public Sub New(ByVal outerInstance As BasicTreeUI)
				Me.outerInstance = outerInstance
			End Sub


			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.

			Public Overridable Sub propertyChange(ByVal [event] As PropertyChangeEvent)
				outerInstance.handler.propertyChange([event])
			End Sub
		End Class ' End of BasicTreeUI.SelectionModelPropertyChangeHandler


		''' <summary>
		''' <code>TreeTraverseAction</code> is the action used for left/right keys.
		''' Will toggle the expandedness of a node, as well as potentially
		''' incrementing the selection.
		''' </summary>
		Public Class TreeTraverseAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicTreeUI

			''' <summary>
			''' Determines direction to traverse, 1 means expand, -1 means
			''' collapse. 
			''' </summary>
			Protected Friend direction As Integer
			''' <summary>
			''' True if the selection is reset, false means only the lead path
			''' changes. 
			''' </summary>
			Private changeSelection As Boolean

			Public Sub New(ByVal outerInstance As BasicTreeUI, ByVal direction As Integer, ByVal name As String)
					Me.outerInstance = outerInstance
				Me.New(direction, name, True)
			End Sub

			Private Sub New(ByVal outerInstance As BasicTreeUI, ByVal direction As Integer, ByVal name As String, ByVal changeSelection As Boolean)
					Me.outerInstance = outerInstance
				Me.direction = direction
				Me.changeSelection = changeSelection
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				If outerInstance.tree IsNot Nothing Then SHARED_ACTION.traverse(outerInstance.tree, BasicTreeUI.this, direction, changeSelection)
			End Sub

			Public Property Overrides enabled As Boolean
				Get
					Return (outerInstance.tree IsNot Nothing AndAlso outerInstance.tree.enabled)
				End Get
			End Property
		End Class ' BasicTreeUI.TreeTraverseAction


		''' <summary>
		''' TreePageAction handles page up and page down events.
		''' </summary>
		Public Class TreePageAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicTreeUI

			''' <summary>
			''' Specifies the direction to adjust the selection by. </summary>
			Protected Friend direction As Integer
			''' <summary>
			''' True indicates should set selection from anchor path. </summary>
			Private addToSelection As Boolean
			Private changeSelection As Boolean

			Public Sub New(ByVal outerInstance As BasicTreeUI, ByVal direction As Integer, ByVal name As String)
					Me.outerInstance = outerInstance
				Me.New(direction, name, False, True)
			End Sub

			Private Sub New(ByVal outerInstance As BasicTreeUI, ByVal direction As Integer, ByVal name As String, ByVal addToSelection As Boolean, ByVal changeSelection As Boolean)
					Me.outerInstance = outerInstance
				Me.direction = direction
				Me.addToSelection = addToSelection
				Me.changeSelection = changeSelection
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				If outerInstance.tree IsNot Nothing Then SHARED_ACTION.page(outerInstance.tree, BasicTreeUI.this, direction, addToSelection, changeSelection)
			End Sub

			Public Property Overrides enabled As Boolean
				Get
					Return (outerInstance.tree IsNot Nothing AndAlso outerInstance.tree.enabled)
				End Get
			End Property

		End Class ' BasicTreeUI.TreePageAction


		''' <summary>
		''' TreeIncrementAction is used to handle up/down actions.  Selection
		''' is moved up or down based on direction.
		''' </summary>
		Public Class TreeIncrementAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicTreeUI

			''' <summary>
			''' Specifies the direction to adjust the selection by. </summary>
			Protected Friend direction As Integer
			''' <summary>
			''' If true the new item is added to the selection, if false the
			''' selection is reset. 
			''' </summary>
			Private addToSelection As Boolean
			Private changeSelection As Boolean

			Public Sub New(ByVal outerInstance As BasicTreeUI, ByVal direction As Integer, ByVal name As String)
					Me.outerInstance = outerInstance
				Me.New(direction, name, False, True)
			End Sub

			Private Sub New(ByVal outerInstance As BasicTreeUI, ByVal direction As Integer, ByVal name As String, ByVal addToSelection As Boolean, ByVal changeSelection As Boolean)
					Me.outerInstance = outerInstance
				Me.direction = direction
				Me.addToSelection = addToSelection
				Me.changeSelection = changeSelection
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				If outerInstance.tree IsNot Nothing Then SHARED_ACTION.increment(outerInstance.tree, BasicTreeUI.this, direction, addToSelection, changeSelection)
			End Sub

			Public Property Overrides enabled As Boolean
				Get
					Return (outerInstance.tree IsNot Nothing AndAlso outerInstance.tree.enabled)
				End Get
			End Property

		End Class ' End of class BasicTreeUI.TreeIncrementAction

		''' <summary>
		''' TreeHomeAction is used to handle end/home actions.
		''' Scrolls either the first or last cell to be visible based on
		''' direction.
		''' </summary>
		Public Class TreeHomeAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicTreeUI

			Protected Friend direction As Integer
			''' <summary>
			''' Set to true if append to selection. </summary>
			Private addToSelection As Boolean
			Private changeSelection As Boolean

			Public Sub New(ByVal outerInstance As BasicTreeUI, ByVal direction As Integer, ByVal name As String)
					Me.outerInstance = outerInstance
				Me.New(direction, name, False, True)
			End Sub

			Private Sub New(ByVal outerInstance As BasicTreeUI, ByVal direction As Integer, ByVal name As String, ByVal addToSelection As Boolean, ByVal changeSelection As Boolean)
					Me.outerInstance = outerInstance
				Me.direction = direction
				Me.changeSelection = changeSelection
				Me.addToSelection = addToSelection
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				If outerInstance.tree IsNot Nothing Then SHARED_ACTION.home(outerInstance.tree, BasicTreeUI.this, direction, addToSelection, changeSelection)
			End Sub

			Public Property Overrides enabled As Boolean
				Get
					Return (outerInstance.tree IsNot Nothing AndAlso outerInstance.tree.enabled)
				End Get
			End Property

		End Class ' End of class BasicTreeUI.TreeHomeAction


		''' <summary>
		''' For the first selected row expandedness will be toggled.
		''' </summary>
		Public Class TreeToggleAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicTreeUI

			Public Sub New(ByVal outerInstance As BasicTreeUI, ByVal name As String)
					Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				If outerInstance.tree IsNot Nothing Then SHARED_ACTION.toggle(outerInstance.tree, BasicTreeUI.this)
			End Sub

			Public Property Overrides enabled As Boolean
				Get
					Return (outerInstance.tree IsNot Nothing AndAlso outerInstance.tree.enabled)
				End Get
			End Property

		End Class ' End of class BasicTreeUI.TreeToggleAction


		''' <summary>
		''' ActionListener that invokes cancelEditing when action performed.
		''' </summary>
		Public Class TreeCancelEditingAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicTreeUI

			Public Sub New(ByVal outerInstance As BasicTreeUI, ByVal name As String)
					Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				If outerInstance.tree IsNot Nothing Then SHARED_ACTION.cancelEditing(outerInstance.tree, BasicTreeUI.this)
			End Sub

			Public Property Overrides enabled As Boolean
				Get
					Return (outerInstance.tree IsNot Nothing AndAlso outerInstance.tree.enabled AndAlso outerInstance.isEditing(outerInstance.tree))
				End Get
			End Property
		End Class ' End of class BasicTreeUI.TreeCancelEditingAction


		''' <summary>
		''' MouseInputHandler handles passing all mouse events,
		''' including mouse motion events, until the mouse is released to
		''' the destination it is constructed with. It is assumed all the
		''' events are currently target at source.
		''' </summary>
		Public Class MouseInputHandler
			Inherits Object
			Implements MouseInputListener

			Private ReadOnly outerInstance As BasicTreeUI

			''' <summary>
			''' Source that events are coming from. </summary>
			Protected Friend source As Component
			''' <summary>
			''' Destination that receives all events. </summary>
			Protected Friend destination As Component
			Private focusComponent As Component
			Private dispatchedEvent As Boolean

			Public Sub New(ByVal outerInstance As BasicTreeUI, ByVal source As Component, ByVal destination As Component, ByVal [event] As MouseEvent)
					Me.outerInstance = outerInstance
				Me.New(source, destination, [event], Nothing)
			End Sub

			Friend Sub New(ByVal outerInstance As BasicTreeUI, ByVal source As Component, ByVal destination As Component, ByVal [event] As MouseEvent, ByVal focusComponent As Component)
					Me.outerInstance = outerInstance
				Me.source = source
				Me.destination = destination
				Me.source.addMouseListener(Me)
				Me.source.addMouseMotionListener(Me)

				sun.swing.SwingUtilities2.skipClickCountunt(destination, [event].clickCount - 1)

				' Dispatch the editing event! 
				destination.dispatchEvent(SwingUtilities.convertMouseEvent(source, [event], destination))
				Me.focusComponent = focusComponent
			End Sub

			Public Overridable Sub mouseClicked(ByVal e As MouseEvent)
				If destination IsNot Nothing Then
					dispatchedEvent = True
					destination.dispatchEvent(SwingUtilities.convertMouseEvent(source, e, destination))
				End If
			End Sub

			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
				If destination IsNot Nothing Then destination.dispatchEvent(SwingUtilities.convertMouseEvent(source, e, destination))
				removeFromSource()
			End Sub

			Public Overridable Sub mouseEntered(ByVal e As MouseEvent)
				If Not SwingUtilities.isLeftMouseButton(e) Then removeFromSource()
			End Sub

			Public Overridable Sub mouseExited(ByVal e As MouseEvent)
				If Not SwingUtilities.isLeftMouseButton(e) Then removeFromSource()
			End Sub

			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
				If destination IsNot Nothing Then
					dispatchedEvent = True
					destination.dispatchEvent(SwingUtilities.convertMouseEvent(source, e, destination))
				End If
			End Sub

			Public Overridable Sub mouseMoved(ByVal e As MouseEvent)
				removeFromSource()
			End Sub

			Protected Friend Overridable Sub removeFromSource()
				If source IsNot Nothing Then
					source.removeMouseListener(Me)
					source.removeMouseMotionListener(Me)
					If focusComponent IsNot Nothing AndAlso focusComponent Is destination AndAlso (Not dispatchedEvent) AndAlso (TypeOf focusComponent Is JTextField) Then CType(focusComponent, JTextField).selectAll()
				End If
					destination = Nothing
					source = destination
			End Sub

		End Class ' End of class BasicTreeUI.MouseInputHandler

		Private Shared ReadOnly defaultTransferHandler As TransferHandler = New TreeTransferHandler

		Friend Class TreeTransferHandler
			Inherits TransferHandler
			Implements javax.swing.plaf.UIResource, IComparer(Of TreePath)

			Private tree As JTree

			''' <summary>
			''' Create a Transferable to use as the source for a data transfer.
			''' </summary>
			''' <param name="c">  The component holding the data to be transfered.  This
			'''  argument is provided to enable sharing of TransferHandlers by
			'''  multiple components. </param>
			''' <returns>  The representation of the data to be transfered.
			'''  </returns>
			Protected Friend Overrides Function createTransferable(ByVal c As JComponent) As Transferable
				If TypeOf c Is JTree Then
					tree = CType(c, JTree)
					Dim paths As TreePath() = tree.selectionPaths

					If paths Is Nothing OrElse paths.Length = 0 Then Return Nothing

					Dim plainBuf As New StringBuilder
					Dim htmlBuf As New StringBuilder

					htmlBuf.Append("<html>" & vbLf & "<body>" & vbLf & "<ul>" & vbLf)

					Dim model As TreeModel = tree.model
					Dim lastPath As TreePath = Nothing
					Dim displayPaths As TreePath() = getDisplayOrderPaths(paths)

					For Each path As TreePath In displayPaths
						Dim node As Object = path.lastPathComponent
						Dim leaf As Boolean = model.isLeaf(node)
						Dim label As String = getDisplayString(path, True, leaf)

						plainBuf.Append(label & vbLf)
						htmlBuf.Append("  <li>" & label & vbLf)
					Next path

					' remove the last newline
					plainBuf.Remove(plainBuf.Length - 1, 1)
					htmlBuf.Append("</ul>" & vbLf & "</body>" & vbLf & "</html>")

					tree = Nothing

					Return New BasicTransferable(plainBuf.ToString(), htmlBuf.ToString())
				End If

				Return Nothing
			End Function

			Public Overridable Function compare(ByVal o1 As TreePath, ByVal o2 As TreePath) As Integer
				Dim row1 As Integer = tree.getRowForPath(o1)
				Dim row2 As Integer = tree.getRowForPath(o2)
				Return row1 - row2
			End Function

			Friend Overridable Function getDisplayString(ByVal path As TreePath, ByVal selected As Boolean, ByVal leaf As Boolean) As String
				Dim row As Integer = tree.getRowForPath(path)
				Dim hasFocus As Boolean = tree.leadSelectionRow = row
				Dim node As Object = path.lastPathComponent
				Return tree.convertValueToText(node, selected, tree.isExpanded(row), leaf, row, hasFocus)
			End Function

			''' <summary>
			''' Selection paths are in selection order.  The conversion to
			''' HTML requires display order.  This method resorts the paths
			''' to be in the display order.
			''' </summary>
			Friend Overridable Function getDisplayOrderPaths(ByVal paths As TreePath()) As TreePath()
				' sort the paths to display order rather than selection order
				Dim selOrder As New List(Of TreePath)
				For Each path As TreePath In paths
					selOrder.Add(path)
				Next path
				java.util.Collections.sort(selOrder, Me)
				Dim n As Integer = selOrder.Count
				Dim displayPaths As TreePath() = New TreePath(n - 1){}
				For i As Integer = 0 To n - 1
					displayPaths(i) = selOrder(i)
				Next i
				Return displayPaths
			End Function

			Public Overrides Function getSourceActions(ByVal c As JComponent) As Integer
				Return COPY
			End Function

		End Class


		Private Class Handler
			Implements CellEditorListener, FocusListener, KeyListener, MouseListener, MouseMotionListener, PropertyChangeListener, TreeExpansionListener, TreeModelListener, TreeSelectionListener, javax.swing.plaf.basic.DragRecognitionSupport.BeforeDrag

			Private ReadOnly outerInstance As BasicTreeUI

			Public Sub New(ByVal outerInstance As BasicTreeUI)
				Me.outerInstance = outerInstance
			End Sub

			'
			' KeyListener
			'
			Private prefix As String = ""
			Private typedString As String = ""
			Private lastTime As Long = 0L

			''' <summary>
			''' Invoked when a key has been typed.
			''' 
			''' Moves the keyboard focus to the first element whose prefix matches the
			''' sequence of alphanumeric keys pressed by the user with delay less
			''' than value of <code>timeFactor</code> property (or 1000 milliseconds
			''' if it is not defined). Subsequent same key presses move the keyboard
			''' focus to the next object that starts with the same letter until another
			''' key is pressed, then it is treated as the prefix with appropriate number
			''' of the same letters followed by first typed another letter.
			''' </summary>
			Public Overridable Sub keyTyped(ByVal e As KeyEvent)
				' handle first letter navigation
				If outerInstance.tree IsNot Nothing AndAlso outerInstance.tree.rowCount>0 AndAlso outerInstance.tree.hasFocus() AndAlso outerInstance.tree.enabled Then
					If e.altDown OrElse BasicGraphicsUtils.isMenuShortcutKeyDown(e) OrElse isNavigationKey(e) Then Return
					Dim startingFromSelection As Boolean = True

					Dim c As Char = e.keyChar

					Dim time As Long = e.when
					Dim startingRow As Integer = outerInstance.tree.leadSelectionRow
					If time - lastTime < outerInstance.timeFactor Then
						typedString += c
						If (prefix.Length = 1) AndAlso (c = prefix.Chars(0)) Then
							' Subsequent same key presses move the keyboard focus to the next
							' object that starts with the same letter.
							startingRow += 1
						Else
							prefix = typedString
						End If
					Else
						startingRow += 1
						typedString = "" & AscW(c)
						prefix = typedString
					End If
					lastTime = time

					If startingRow < 0 OrElse startingRow >= outerInstance.tree.rowCount Then
						startingFromSelection = False
						startingRow = 0
					End If
					Dim path As TreePath = outerInstance.tree.getNextMatch(prefix, startingRow, javax.swing.text.Position.Bias.Forward)
					If path IsNot Nothing Then
						outerInstance.tree.selectionPath = path
						Dim row As Integer = outerInstance.getRowForPath(outerInstance.tree, path)
						outerInstance.ensureRowsAreVisible(row, row)
					ElseIf startingFromSelection Then
						path = outerInstance.tree.getNextMatch(prefix, 0, javax.swing.text.Position.Bias.Forward)
						If path IsNot Nothing Then
							outerInstance.tree.selectionPath = path
							Dim row As Integer = outerInstance.getRowForPath(outerInstance.tree, path)
							outerInstance.ensureRowsAreVisible(row, row)
						End If
					End If
				End If
			End Sub

			''' <summary>
			''' Invoked when a key has been pressed.
			''' 
			''' Checks to see if the key event is a navigation key to prevent
			''' dispatching these keys for the first letter navigation.
			''' </summary>
			Public Overridable Sub keyPressed(ByVal e As KeyEvent)
				If outerInstance.tree IsNot Nothing AndAlso isNavigationKey(e) Then
					prefix = ""
					typedString = ""
					lastTime = 0L
				End If
			End Sub

			Public Overridable Sub keyReleased(ByVal e As KeyEvent)
			End Sub

			''' <summary>
			''' Returns whether or not the supplied key event maps to a key that is used for
			''' navigation.  This is used for optimizing key input by only passing non-
			''' navigation keys to the first letter navigation mechanism.
			''' </summary>
			Private Function isNavigationKey(ByVal [event] As KeyEvent) As Boolean
				Dim inputMap As InputMap = outerInstance.tree.getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT)
				Dim key As KeyStroke = KeyStroke.getKeyStrokeForEvent([event])

				Return inputMap IsNot Nothing AndAlso inputMap.get(key) IsNot Nothing
			End Function


			'
			' PropertyChangeListener
			'
			Public Overridable Sub propertyChange(ByVal [event] As PropertyChangeEvent)
				If [event].source Is outerInstance.treeSelectionModel Then
					outerInstance.treeSelectionModel.resetRowSelection()
				ElseIf [event].source Is outerInstance.tree Then
					Dim changeName As String = [event].propertyName

					If changeName = JTree.LEAD_SELECTION_PATH_PROPERTY Then
						If Not outerInstance.ignoreLAChange Then
							outerInstance.updateLeadSelectionRow()
							outerInstance.repaintPath(CType([event].oldValue, TreePath))
							outerInstance.repaintPath(CType([event].newValue, TreePath))
						End If
					ElseIf changeName = JTree.ANCHOR_SELECTION_PATH_PROPERTY Then
						If Not outerInstance.ignoreLAChange Then
							outerInstance.repaintPath(CType([event].oldValue, TreePath))
							outerInstance.repaintPath(CType([event].newValue, TreePath))
						End If
					End If
					If changeName = JTree.CELL_RENDERER_PROPERTY Then
						outerInstance.cellRenderer = CType([event].newValue, TreeCellRenderer)
						outerInstance.redoTheLayout()
					ElseIf changeName = JTree.TREE_MODEL_PROPERTY Then
						outerInstance.model = CType([event].newValue, TreeModel)
					ElseIf changeName = JTree.ROOT_VISIBLE_PROPERTY Then
						outerInstance.rootVisible = CBool([event].newValue)
					ElseIf changeName = JTree.SHOWS_ROOT_HANDLES_PROPERTY Then
						outerInstance.showsRootHandles = CBool([event].newValue)
					ElseIf changeName = JTree.ROW_HEIGHT_PROPERTY Then
						outerInstance.rowHeight = CInt(Fix([event].newValue))
					ElseIf changeName = JTree.CELL_EDITOR_PROPERTY Then
						outerInstance.cellEditor = CType([event].newValue, TreeCellEditor)
					ElseIf changeName = JTree.EDITABLE_PROPERTY Then
						outerInstance.editable = CBool([event].newValue)
					ElseIf changeName = JTree.LARGE_MODEL_PROPERTY Then
						outerInstance.largeModel = outerInstance.tree.largeModel
					ElseIf changeName = JTree.SELECTION_MODEL_PROPERTY Then
						outerInstance.selectionModel = outerInstance.tree.selectionModel
					ElseIf changeName = "font" Then
						outerInstance.completeEditing()
						If outerInstance.treeState IsNot Nothing Then outerInstance.treeState.invalidateSizes()
						outerInstance.updateSize()
					ElseIf changeName = "componentOrientation" Then
						If outerInstance.tree IsNot Nothing Then
							outerInstance.leftToRight = BasicGraphicsUtils.isLeftToRight(outerInstance.tree)
							outerInstance.redoTheLayout()
							outerInstance.tree.treeDidChange()

							Dim km As InputMap = outerInstance.getInputMap(JComponent.WHEN_FOCUSED)
							SwingUtilities.replaceUIInputMap(outerInstance.tree, JComponent.WHEN_FOCUSED, km)
						End If
					ElseIf "dropLocation" = changeName Then
						Dim oldValue As JTree.DropLocation = CType([event].oldValue, JTree.DropLocation)
						repaintDropLocation(oldValue)
						repaintDropLocation(outerInstance.tree.dropLocation)
					End If
				End If
			End Sub

			Private Sub repaintDropLocation(ByVal loc As JTree.DropLocation)
				If loc Is Nothing Then Return

				Dim r As Rectangle

				If outerInstance.isDropLine(loc) Then
					r = outerInstance.getDropLineRect(loc)
				Else
					r = outerInstance.tree.getPathBounds(loc.path)
				End If

				If r IsNot Nothing Then outerInstance.tree.repaint(r)
			End Sub

			'
			' MouseListener
			'

			' Whether or not the mouse press (which is being considered as part
			' of a drag sequence) also caused the selection change to be fully
			' processed.
			Private dragPressDidSelection As Boolean

			' Set to true when a drag gesture has been fully recognized and DnD
			' begins. Use this to ignore further mouse events which could be
			' delivered if DnD is cancelled (via ESCAPE for example)
			Private dragStarted As Boolean

			' The path over which the press occurred and the press event itself
			Private pressedPath As TreePath
			Private pressedEvent As MouseEvent

			' Used to detect whether the press event causes a selection change.
			' If it does, we won't try to start editing on the release.
			Private valueChangedOnPress As Boolean

			Private Function isActualPath(ByVal path As TreePath, ByVal x As Integer, ByVal y As Integer) As Boolean
				If path Is Nothing Then Return False

				Dim bounds As Rectangle = outerInstance.getPathBounds(outerInstance.tree, path)
				If bounds Is Nothing OrElse y > (bounds.y + bounds.height) Then Return False

				Return (x >= bounds.x) AndAlso (x <= (bounds.x + bounds.width))
			End Function

			Public Overridable Sub mouseClicked(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseEntered(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseExited(ByVal e As MouseEvent)
			End Sub

			''' <summary>
			''' Invoked when a mouse button has been pressed on a component.
			''' </summary>
			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				If sun.swing.SwingUtilities2.shouldIgnore(e, outerInstance.tree) Then Return

				' if we can't stop any ongoing editing, do nothing
				If outerInstance.isEditing(outerInstance.tree) AndAlso outerInstance.tree.invokesStopCellEditing AndAlso (Not outerInstance.stopEditing(outerInstance.tree)) Then Return

				outerInstance.completeEditing()

				pressedPath = outerInstance.getClosestPathForLocation(outerInstance.tree, e.x, e.y)

				If outerInstance.tree.dragEnabled Then
					mousePressedDND(e)
				Else
					sun.swing.SwingUtilities2.adjustFocus(outerInstance.tree)
					handleSelection(e)
				End If
			End Sub

			Private Sub mousePressedDND(ByVal e As MouseEvent)
				pressedEvent = e
				Dim grabFocus As Boolean = True
				dragStarted = False
				valueChangedOnPress = False

				' if we have a valid path and this is a drag initiating event
				If isActualPath(pressedPath, e.x, e.y) AndAlso DragRecognitionSupport.mousePressed(e) Then

					dragPressDidSelection = False

					If BasicGraphicsUtils.isMenuShortcutKeyDown(e) Then
						' do nothing for control - will be handled on release
						' or when drag starts
						Return
					ElseIf (Not e.shiftDown) AndAlso outerInstance.tree.isPathSelected(pressedPath) Then
						' clicking on something that's already selected
						' and need to make it the lead now
						outerInstance.anchorSelectionPath = pressedPath
						outerInstance.leadSelectionPathath(pressedPath, True)
						Return
					End If

					dragPressDidSelection = True

					' could be a drag initiating event - don't grab focus
					grabFocus = False
				End If

				If grabFocus Then sun.swing.SwingUtilities2.adjustFocus(outerInstance.tree)

				handleSelection(e)
			End Sub

			Friend Overridable Sub handleSelection(ByVal e As MouseEvent)
				If pressedPath IsNot Nothing Then
					Dim bounds As Rectangle = outerInstance.getPathBounds(outerInstance.tree, pressedPath)

					If bounds Is Nothing OrElse e.y >= (bounds.y + bounds.height) Then Return

					' Preferably checkForClickInExpandControl could take
					' the Event to do this it self!
					If SwingUtilities.isLeftMouseButton(e) Then outerInstance.checkForClickInExpandControl(pressedPath, e.x, e.y)

					Dim x As Integer = e.x

					' Perhaps they clicked the cell itself. If so,
					' select it.
					If x >= bounds.x AndAlso x < (bounds.x + bounds.width) Then
						If outerInstance.tree.dragEnabled OrElse (Not outerInstance.startEditing(pressedPath, e)) Then outerInstance.selectPathForEvent(pressedPath, e)
					End If
				End If
			End Sub

			Public Overridable Sub dragStarting(ByVal [me] As MouseEvent)
				dragStarted = True

				If BasicGraphicsUtils.isMenuShortcutKeyDown([me]) Then
					outerInstance.tree.addSelectionPath(pressedPath)
					outerInstance.anchorSelectionPath = pressedPath
					outerInstance.leadSelectionPathath(pressedPath, True)
				End If

				pressedEvent = Nothing
				pressedPath = Nothing
			End Sub

			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
				If sun.swing.SwingUtilities2.shouldIgnore(e, outerInstance.tree) Then Return

				If outerInstance.tree.dragEnabled Then DragRecognitionSupport.mouseDragged(e, Me)
			End Sub

			''' <summary>
			''' Invoked when the mouse button has been moved on a component
			''' (with no buttons no down).
			''' </summary>
			Public Overridable Sub mouseMoved(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
				If sun.swing.SwingUtilities2.shouldIgnore(e, outerInstance.tree) Then Return

				If outerInstance.tree.dragEnabled Then mouseReleasedDND(e)

				pressedEvent = Nothing
				pressedPath = Nothing
			End Sub

			Private Sub mouseReleasedDND(ByVal e As MouseEvent)
				Dim [me] As MouseEvent = DragRecognitionSupport.mouseReleased(e)
				If [me] IsNot Nothing Then
					sun.swing.SwingUtilities2.adjustFocus(outerInstance.tree)
					If Not dragPressDidSelection Then handleSelection([me])
				End If

				If Not dragStarted Then

					' Note: We don't give the tree a chance to start editing if the
					' mouse press caused a selection change. Otherwise the default
					' tree cell editor will start editing on EVERY press and
					' release. If it turns out that this affects some editors, we
					' can always parameterize this with a client property. ex:
					'
					' if (pressedPath != null &&
					'         (Boolean.TRUE == tree.getClientProperty("Tree.DnD.canEditOnValueChange") ||
					'          !valueChangedOnPress) && ...
					If pressedPath IsNot Nothing AndAlso (Not valueChangedOnPress) AndAlso isActualPath(pressedPath, pressedEvent.x, pressedEvent.y) Then outerInstance.startEditingOnRelease(pressedPath, pressedEvent, e)
				End If
			End Sub

			'
			' FocusListener
			'
			Public Overridable Sub focusGained(ByVal e As FocusEvent)
				If outerInstance.tree IsNot Nothing Then
					Dim pBounds As Rectangle

					pBounds = outerInstance.getPathBounds(outerInstance.tree, outerInstance.tree.leadSelectionPath)
					If pBounds IsNot Nothing Then outerInstance.tree.repaint(outerInstance.getRepaintPathBounds(pBounds))
					pBounds = outerInstance.getPathBounds(outerInstance.tree, outerInstance.leadSelectionPath)
					If pBounds IsNot Nothing Then outerInstance.tree.repaint(outerInstance.getRepaintPathBounds(pBounds))
				End If
			End Sub

			Public Overridable Sub focusLost(ByVal e As FocusEvent)
				focusGained(e)
			End Sub

			'
			' CellEditorListener
			'
			Public Overridable Sub editingStopped(ByVal e As ChangeEvent)
				outerInstance.completeEditing(False, False, True)
			End Sub

			''' <summary>
			''' Messaged when editing has been canceled in the tree. </summary>
			Public Overridable Sub editingCanceled(ByVal e As ChangeEvent)
				outerInstance.completeEditing(False, False, False)
			End Sub


			'
			' TreeSelectionListener
			'
			Public Overridable Sub valueChanged(ByVal [event] As TreeSelectionEvent) Implements TreeSelectionListener.valueChanged
				valueChangedOnPress = True

				' Stop editing
				outerInstance.completeEditing()
				' Make sure all the paths are visible, if necessary.
				' PENDING: This should be tweaked when isAdjusting is added
				If outerInstance.tree.expandsSelectedPaths AndAlso outerInstance.treeSelectionModel IsNot Nothing Then
					Dim paths As TreePath() = outerInstance.treeSelectionModel.selectionPaths

					If paths IsNot Nothing Then
						For counter As Integer = paths.Length - 1 To 0 Step -1
							Dim path As TreePath = paths(counter).parentPath
							Dim expand As Boolean = True

							Do While path IsNot Nothing
								' Indicates this path isn't valid anymore,
								' we shouldn't attempt to expand it then.
								If outerInstance.treeModel.isLeaf(path.lastPathComponent) Then
									expand = False
									path = Nothing
								Else
									path = path.parentPath
								End If
							Loop
							If expand Then outerInstance.tree.makeVisible(paths(counter))
						Next counter
					End If
				End If

				Dim oldLead As TreePath = outerInstance.leadSelectionPath
				outerInstance.lastSelectedRow = outerInstance.tree.minSelectionRow
				Dim lead As TreePath = outerInstance.tree.selectionModel.leadSelectionPath
				outerInstance.anchorSelectionPath = lead
				outerInstance.leadSelectionPath = lead

				Dim changedPaths As TreePath() = [event].paths
				Dim nodeBounds As Rectangle
				Dim visRect As Rectangle = outerInstance.tree.visibleRect
				Dim paintPaths As Boolean = True
				Dim nWidth As Integer = outerInstance.tree.width

				If changedPaths IsNot Nothing Then
					Dim counter As Integer, maxCounter As Integer = changedPaths.Length

					If maxCounter > 4 Then
						outerInstance.tree.repaint()
						paintPaths = False
					Else
						For counter = 0 To maxCounter - 1
							nodeBounds = outerInstance.getPathBounds(outerInstance.tree, changedPaths(counter))
							If nodeBounds IsNot Nothing AndAlso visRect.intersects(nodeBounds) Then outerInstance.tree.repaint(0, nodeBounds.y, nWidth, nodeBounds.height)
						Next counter
					End If
				End If
				If paintPaths Then
					nodeBounds = outerInstance.getPathBounds(outerInstance.tree, oldLead)
					If nodeBounds IsNot Nothing AndAlso visRect.intersects(nodeBounds) Then outerInstance.tree.repaint(0, nodeBounds.y, nWidth, nodeBounds.height)
					nodeBounds = outerInstance.getPathBounds(outerInstance.tree, lead)
					If nodeBounds IsNot Nothing AndAlso visRect.intersects(nodeBounds) Then outerInstance.tree.repaint(0, nodeBounds.y, nWidth, nodeBounds.height)
				End If
			End Sub


			'
			' TreeExpansionListener
			'
			Public Overridable Sub treeExpanded(ByVal [event] As TreeExpansionEvent) Implements TreeExpansionListener.treeExpanded
				If [event] IsNot Nothing AndAlso outerInstance.tree IsNot Nothing Then
					Dim path As TreePath = [event].path

					outerInstance.updateExpandedDescendants(path)
				End If
			End Sub

			Public Overridable Sub treeCollapsed(ByVal [event] As TreeExpansionEvent) Implements TreeExpansionListener.treeCollapsed
				If [event] IsNot Nothing AndAlso outerInstance.tree IsNot Nothing Then
					Dim path As TreePath = [event].path

					outerInstance.completeEditing()
					If path IsNot Nothing AndAlso outerInstance.tree.isVisible(path) Then
						outerInstance.treeState.expandedStateate(path, False)
						outerInstance.updateLeadSelectionRow()
						outerInstance.updateSize()
					End If
				End If
			End Sub

			'
			' TreeModelListener
			'
			Public Overridable Sub treeNodesChanged(ByVal e As TreeModelEvent) Implements TreeModelListener.treeNodesChanged
				If outerInstance.treeState IsNot Nothing AndAlso e IsNot Nothing Then
					Dim parentPath As TreePath = sun.swing.SwingUtilities2.getTreePath(e, outerInstance.model)
					Dim indices As Integer() = e.childIndices
					If indices Is Nothing OrElse indices.Length = 0 Then
						' The root has changed
						outerInstance.treeState.treeNodesChanged(e)
						outerInstance.updateSize()
					ElseIf outerInstance.treeState.isExpanded(parentPath) Then
						' Changed nodes are visible
						' Find the minimum index, we only need paint from there
						' down.
						Dim minIndex As Integer = indices(0)
						For i As Integer = indices.Length - 1 To 1 Step -1
							minIndex = Math.Min(indices(i), minIndex)
						Next i
						Dim minChild As Object = outerInstance.treeModel.getChild(parentPath.lastPathComponent, minIndex)
						Dim minPath As TreePath = parentPath.pathByAddingChild(minChild)
						Dim minBounds As Rectangle = outerInstance.getPathBounds(outerInstance.tree, minPath)

						' Forward to the treestate
						outerInstance.treeState.treeNodesChanged(e)

						' Mark preferred size as bogus.
						outerInstance.updateSize0()

						' And repaint
						Dim newMinBounds As Rectangle = outerInstance.getPathBounds(outerInstance.tree, minPath)
						If minBounds Is Nothing OrElse newMinBounds Is Nothing Then Return

						If indices.Length = 1 AndAlso newMinBounds.height = minBounds.height Then
							outerInstance.tree.repaint(0, minBounds.y, outerInstance.tree.width, minBounds.height)
						Else
							outerInstance.tree.repaint(0, minBounds.y, outerInstance.tree.width, outerInstance.tree.height - minBounds.y)
						End If
					Else
						' Nodes that changed aren't visible.  No need to paint
						outerInstance.treeState.treeNodesChanged(e)
					End If
				End If
			End Sub

			Public Overridable Sub treeNodesInserted(ByVal e As TreeModelEvent) Implements TreeModelListener.treeNodesInserted
				If outerInstance.treeState IsNot Nothing AndAlso e IsNot Nothing Then
					outerInstance.treeState.treeNodesInserted(e)

					outerInstance.updateLeadSelectionRow()

					Dim path As TreePath = sun.swing.SwingUtilities2.getTreePath(e, outerInstance.model)

					If outerInstance.treeState.isExpanded(path) Then
						outerInstance.updateSize()
					Else
						' PENDING(sky): Need a method in TreeModelEvent
						' that can return the count, getChildIndices allocs
						' a new array!
						Dim indices As Integer() = e.childIndices
						Dim childCount As Integer = outerInstance.treeModel.getChildCount(path.lastPathComponent)

						If indices IsNot Nothing AndAlso (childCount - indices.Length) = 0 Then outerInstance.updateSize()
					End If
				End If
			End Sub

			Public Overridable Sub treeNodesRemoved(ByVal e As TreeModelEvent) Implements TreeModelListener.treeNodesRemoved
				If outerInstance.treeState IsNot Nothing AndAlso e IsNot Nothing Then
					outerInstance.treeState.treeNodesRemoved(e)

					outerInstance.updateLeadSelectionRow()

					Dim path As TreePath = sun.swing.SwingUtilities2.getTreePath(e, outerInstance.model)

					If outerInstance.treeState.isExpanded(path) OrElse outerInstance.treeModel.getChildCount(path.lastPathComponent) = 0 Then outerInstance.updateSize()
				End If
			End Sub

			Public Overridable Sub treeStructureChanged(ByVal e As TreeModelEvent) Implements TreeModelListener.treeStructureChanged
				If outerInstance.treeState IsNot Nothing AndAlso e IsNot Nothing Then
					outerInstance.treeState.treeStructureChanged(e)

					outerInstance.updateLeadSelectionRow()

					Dim pPath As TreePath = sun.swing.SwingUtilities2.getTreePath(e, outerInstance.model)

					If pPath IsNot Nothing Then pPath = pPath.parentPath
					If pPath Is Nothing OrElse outerInstance.treeState.isExpanded(pPath) Then outerInstance.updateSize()
				End If
			End Sub
		End Class



		Private Class Actions
			Inherits sun.swing.UIAction

			Private Const SELECT_PREVIOUS As String = "selectPrevious"
			Private Const SELECT_PREVIOUS_CHANGE_LEAD As String = "selectPreviousChangeLead"
			Private Const SELECT_PREVIOUS_EXTEND_SELECTION As String = "selectPreviousExtendSelection"
			Private Const SELECT_NEXT As String = "selectNext"
			Private Const SELECT_NEXT_CHANGE_LEAD As String = "selectNextChangeLead"
			Private Const SELECT_NEXT_EXTEND_SELECTION As String = "selectNextExtendSelection"
			Private Const SELECT_CHILD As String = "selectChild"
			Private Const SELECT_CHILD_CHANGE_LEAD As String = "selectChildChangeLead"
			Private Const SELECT_PARENT As String = "selectParent"
			Private Const SELECT_PARENT_CHANGE_LEAD As String = "selectParentChangeLead"
			Private Const SCROLL_UP_CHANGE_SELECTION As String = "scrollUpChangeSelection"
			Private Const SCROLL_UP_CHANGE_LEAD As String = "scrollUpChangeLead"
			Private Const SCROLL_UP_EXTEND_SELECTION As String = "scrollUpExtendSelection"
			Private Const SCROLL_DOWN_CHANGE_SELECTION As String = "scrollDownChangeSelection"
			Private Const SCROLL_DOWN_EXTEND_SELECTION As String = "scrollDownExtendSelection"
			Private Const SCROLL_DOWN_CHANGE_LEAD As String = "scrollDownChangeLead"
			Private Const SELECT_FIRST As String = "selectFirst"
			Private Const SELECT_FIRST_CHANGE_LEAD As String = "selectFirstChangeLead"
			Private Const SELECT_FIRST_EXTEND_SELECTION As String = "selectFirstExtendSelection"
			Private Const SELECT_LAST As String = "selectLast"
			Private Const SELECT_LAST_CHANGE_LEAD As String = "selectLastChangeLead"
			Private Const SELECT_LAST_EXTEND_SELECTION As String = "selectLastExtendSelection"
			Private Const ___TOGGLE As String = "toggle"
			Private Const CANCEL_EDITING As String = "cancel"
			Private Const START_EDITING As String = "startEditing"
			Private Const SELECT_ALL As String = "selectAll"
			Private Const CLEAR_SELECTION As String = "clearSelection"
			Private Const SCROLL_LEFT As String = "scrollLeft"
			Private Const SCROLL_RIGHT As String = "scrollRight"
			Private Const SCROLL_LEFT_EXTEND_SELECTION As String = "scrollLeftExtendSelection"
			Private Const SCROLL_RIGHT_EXTEND_SELECTION As String = "scrollRightExtendSelection"
			Private Const SCROLL_RIGHT_CHANGE_LEAD As String = "scrollRightChangeLead"
			Private Const SCROLL_LEFT_CHANGE_LEAD As String = "scrollLeftChangeLead"
			Private Const ___EXPAND As String = "expand"
			Private Const ___COLLAPSE As String = "collapse"
			Private Const MOVE_SELECTION_TO_PARENT As String = "moveSelectionToParent"

			' add the lead item to the selection without changing lead or anchor
			Private Const ADD_TO_SELECTION As String = "addToSelection"

			' toggle the selected state of the lead item and move the anchor to it
			Private Const TOGGLE_AND_ANCHOR As String = "toggleAndAnchor"

			' extend the selection to the lead item
			Private Const EXTEND_TO As String = "extendTo"

			' move the anchor to the lead and ensure only that item is selected
			Private Const MOVE_SELECTION_TO As String = "moveSelectionTo"

			Friend Sub New()
				MyBase.New(Nothing)
			End Sub

			Friend Sub New(ByVal key As String)
				MyBase.New(key)
			End Sub

			Public Overridable Function isEnabled(ByVal o As Object) As Boolean
				If TypeOf o Is JTree Then
					If name = CANCEL_EDITING Then Return CType(o, JTree).editing
				End If
				Return True
			End Function

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				Dim tree As JTree = CType(e.source, JTree)
				Dim ui As BasicTreeUI = CType(BasicLookAndFeel.getUIOfType(tree.uI, GetType(BasicTreeUI)), BasicTreeUI)
				If ui Is Nothing Then Return
				Dim key As String = name
				If key = SELECT_PREVIOUS Then
					increment(tree, ui, -1, False, True)
				ElseIf key = SELECT_PREVIOUS_CHANGE_LEAD Then
					increment(tree, ui, -1, False, False)
				ElseIf key = SELECT_PREVIOUS_EXTEND_SELECTION Then
					increment(tree, ui, -1, True, True)
				ElseIf key = SELECT_NEXT Then
					increment(tree, ui, 1, False, True)
				ElseIf key = SELECT_NEXT_CHANGE_LEAD Then
					increment(tree, ui, 1, False, False)
				ElseIf key = SELECT_NEXT_EXTEND_SELECTION Then
					increment(tree, ui, 1, True, True)
				ElseIf key = SELECT_CHILD Then
					traverse(tree, ui, 1, True)
				ElseIf key = SELECT_CHILD_CHANGE_LEAD Then
					traverse(tree, ui, 1, False)
				ElseIf key = SELECT_PARENT Then
					traverse(tree, ui, -1, True)
				ElseIf key = SELECT_PARENT_CHANGE_LEAD Then
					traverse(tree, ui, -1, False)
				ElseIf key = SCROLL_UP_CHANGE_SELECTION Then
					page(tree, ui, -1, False, True)
				ElseIf key = SCROLL_UP_CHANGE_LEAD Then
					page(tree, ui, -1, False, False)
				ElseIf key = SCROLL_UP_EXTEND_SELECTION Then
					page(tree, ui, -1, True, True)
				ElseIf key = SCROLL_DOWN_CHANGE_SELECTION Then
					page(tree, ui, 1, False, True)
				ElseIf key = SCROLL_DOWN_EXTEND_SELECTION Then
					page(tree, ui, 1, True, True)
				ElseIf key = SCROLL_DOWN_CHANGE_LEAD Then
					page(tree, ui, 1, False, False)
				ElseIf key = SELECT_FIRST Then
					home(tree, ui, -1, False, True)
				ElseIf key = SELECT_FIRST_CHANGE_LEAD Then
					home(tree, ui, -1, False, False)
				ElseIf key = SELECT_FIRST_EXTEND_SELECTION Then
					home(tree, ui, -1, True, True)
				ElseIf key = SELECT_LAST Then
					home(tree, ui, 1, False, True)
				ElseIf key = SELECT_LAST_CHANGE_LEAD Then
					home(tree, ui, 1, False, False)
				ElseIf key = SELECT_LAST_EXTEND_SELECTION Then
					home(tree, ui, 1, True, True)
				ElseIf key = ___TOGGLE Then
					toggle(tree, ui)
				ElseIf key = CANCEL_EDITING Then
					cancelEditing(tree, ui)
				ElseIf key = START_EDITING Then
					startEditing(tree, ui)
				ElseIf key = SELECT_ALL Then
					selectAll(tree, ui, True)
				ElseIf key = CLEAR_SELECTION Then
					selectAll(tree, ui, False)
				ElseIf key = ADD_TO_SELECTION Then
					If ui.getRowCount(tree) > 0 Then
						Dim lead As Integer = ui.leadSelectionRow
						If Not tree.isRowSelected(lead) Then
							Dim aPath As TreePath = ui.anchorSelectionPath
							tree.addSelectionRow(lead)
							ui.anchorSelectionPath = aPath
						End If
					End If
				ElseIf key = TOGGLE_AND_ANCHOR Then
					If ui.getRowCount(tree) > 0 Then
						Dim lead As Integer = ui.leadSelectionRow
						Dim lPath As TreePath = ui.leadSelectionPath
						If Not tree.isRowSelected(lead) Then
							tree.addSelectionRow(lead)
						Else
							tree.removeSelectionRow(lead)
							ui.leadSelectionPath = lPath
						End If
						ui.anchorSelectionPath = lPath
					End If
				ElseIf key = EXTEND_TO Then
					extendSelection(tree, ui)
				ElseIf key = MOVE_SELECTION_TO Then
					If ui.getRowCount(tree) > 0 Then
						Dim lead As Integer = ui.leadSelectionRow
						tree.selectionIntervalval(lead, lead)
					End If
				ElseIf key = SCROLL_LEFT Then
					scroll(tree, ui, SwingConstants.HORIZONTAL, -10)
				ElseIf key = SCROLL_RIGHT Then
					scroll(tree, ui, SwingConstants.HORIZONTAL, 10)
				ElseIf key = SCROLL_LEFT_EXTEND_SELECTION Then
					scrollChangeSelection(tree, ui, -1, True, True)
				ElseIf key = SCROLL_RIGHT_EXTEND_SELECTION Then
					scrollChangeSelection(tree, ui, 1, True, True)
				ElseIf key = SCROLL_RIGHT_CHANGE_LEAD Then
					scrollChangeSelection(tree, ui, 1, False, False)
				ElseIf key = SCROLL_LEFT_CHANGE_LEAD Then
					scrollChangeSelection(tree, ui, -1, False, False)
				ElseIf key = ___EXPAND Then
					expand(tree, ui)
				ElseIf key = ___COLLAPSE Then
					collapse(tree, ui)
				ElseIf key = MOVE_SELECTION_TO_PARENT Then
					moveSelectionToParent(tree, ui)
				End If
			End Sub

			Private Sub scrollChangeSelection(ByVal tree As JTree, ByVal ui As BasicTreeUI, ByVal direction As Integer, ByVal addToSelection As Boolean, ByVal changeSelection As Boolean)
				Dim rowCount As Integer

				rowCount = ui.getRowCount(tree)
				If rowCount > 0 AndAlso ui.treeSelectionModel IsNot Nothing Then
					Dim newPath As TreePath
					Dim visRect As Rectangle = tree.visibleRect

					If direction = -1 Then
						newPath = ui.getClosestPathForLocation(tree, visRect.x, visRect.y)
						visRect.x = Math.Max(0, visRect.x - visRect.width)
					Else
						visRect.x = Math.Min(Math.Max(0, tree.width - visRect.width), visRect.x + visRect.width)
						newPath = ui.getClosestPathForLocation(tree, visRect.x, visRect.y + visRect.height)
					End If
					' Scroll
					tree.scrollRectToVisible(visRect)
					' select
					If addToSelection Then
						ui.extendSelection(newPath)
					ElseIf changeSelection Then
						tree.selectionPath = newPath
					Else
						ui.leadSelectionPathath(newPath, True)
					End If
				End If
			End Sub

			Private Sub scroll(ByVal component As JTree, ByVal ui As BasicTreeUI, ByVal direction As Integer, ByVal amount As Integer)
				Dim visRect As Rectangle = component.visibleRect
				Dim size As Dimension = component.size
				If direction = SwingConstants.HORIZONTAL Then
					visRect.x += amount
					visRect.x = Math.Max(0, visRect.x)
					visRect.x = Math.Min(Math.Max(0, size.width - visRect.width), visRect.x)
				Else
					visRect.y += amount
					visRect.y = Math.Max(0, visRect.y)
					visRect.y = Math.Min(Math.Max(0, size.width - visRect.height), visRect.y)
				End If
				component.scrollRectToVisible(visRect)
			End Sub

			Private Sub extendSelection(ByVal tree As JTree, ByVal ui As BasicTreeUI)
				If ui.getRowCount(tree) > 0 Then
					Dim lead As Integer = ui.leadSelectionRow

					If lead <> -1 Then
						Dim leadP As TreePath = ui.leadSelectionPath
						Dim aPath As TreePath = ui.anchorSelectionPath
						Dim aRow As Integer = ui.getRowForPath(tree, aPath)

						If aRow = -1 Then aRow = 0
						tree.selectionIntervalval(aRow, lead)
						ui.leadSelectionPath = leadP
						ui.anchorSelectionPath = aPath
					End If
				End If
			End Sub

			Private Sub selectAll(ByVal tree As JTree, ByVal ui As BasicTreeUI, ByVal selectAll As Boolean)
				Dim rowCount As Integer = ui.getRowCount(tree)

				If rowCount > 0 Then
					If selectAll Then
						If tree.selectionModel.selectionMode = TreeSelectionModel.SINGLE_TREE_SELECTION Then

							Dim lead As Integer = ui.leadSelectionRow
							If lead <> -1 Then
								tree.selectionRow = lead
							ElseIf tree.minSelectionRow = -1 Then
								tree.selectionRow = 0
								ui.ensureRowsAreVisible(0, 0)
							End If
							Return
						End If

						Dim lastPath As TreePath = ui.leadSelectionPath
						Dim aPath As TreePath = ui.anchorSelectionPath

						If lastPath IsNot Nothing AndAlso (Not tree.isVisible(lastPath)) Then lastPath = Nothing
						tree.selectionIntervalval(0, rowCount - 1)
						If lastPath IsNot Nothing Then ui.leadSelectionPath = lastPath
						If aPath IsNot Nothing AndAlso tree.isVisible(aPath) Then ui.anchorSelectionPath = aPath
					Else
						Dim lastPath As TreePath = ui.leadSelectionPath
						Dim aPath As TreePath = ui.anchorSelectionPath

						tree.clearSelection()
						ui.anchorSelectionPath = aPath
						ui.leadSelectionPath = lastPath
					End If
				End If
			End Sub

			Private Sub startEditing(ByVal tree As JTree, ByVal ui As BasicTreeUI)
				Dim lead As TreePath = ui.leadSelectionPath
				Dim editRow As Integer = If(lead IsNot Nothing, ui.getRowForPath(tree, lead), -1)

				If editRow <> -1 Then tree.startEditingAtPath(lead)
			End Sub

			Private Sub cancelEditing(ByVal tree As JTree, ByVal ui As BasicTreeUI)
				tree.cancelEditing()
			End Sub

			Private Sub toggle(ByVal tree As JTree, ByVal ui As BasicTreeUI)
				Dim selRow As Integer = ui.leadSelectionRow

				If selRow <> -1 AndAlso (Not ui.isLeaf(selRow)) Then
					Dim aPath As TreePath = ui.anchorSelectionPath
					Dim lPath As TreePath = ui.leadSelectionPath

					ui.toggleExpandState(ui.getPathForRow(tree, selRow))
					ui.anchorSelectionPath = aPath
					ui.leadSelectionPath = lPath
				End If
			End Sub

			Private Sub expand(ByVal tree As JTree, ByVal ui As BasicTreeUI)
				Dim selRow As Integer = ui.leadSelectionRow
				tree.expandRow(selRow)
			End Sub

			Private Sub collapse(ByVal tree As JTree, ByVal ui As BasicTreeUI)
				Dim selRow As Integer = ui.leadSelectionRow
				tree.collapseRow(selRow)
			End Sub

			Private Sub increment(ByVal tree As JTree, ByVal ui As BasicTreeUI, ByVal direction As Integer, ByVal addToSelection As Boolean, ByVal changeSelection As Boolean)

				' disable moving of lead unless in discontiguous mode
				If (Not addToSelection) AndAlso (Not changeSelection) AndAlso tree.selectionModel.selectionMode <> TreeSelectionModel.DISCONTIGUOUS_TREE_SELECTION Then changeSelection = True

				Dim rowCount As Integer

				rowCount = tree.rowCount
				If ui.treeSelectionModel IsNot Nothing AndAlso rowCount > 0 Then
					Dim selIndex As Integer = ui.leadSelectionRow
					Dim newIndex As Integer

					If selIndex = -1 Then
						If direction = 1 Then
							newIndex = 0
						Else
							newIndex = rowCount - 1
						End If
					Else
						' Aparently people don't like wrapping;( 
						newIndex = Math.Min(rowCount - 1, Math.Max(0, (selIndex + direction)))
					End If
					If addToSelection AndAlso ui.treeSelectionModel.selectionMode <> TreeSelectionModel.SINGLE_TREE_SELECTION Then
						ui.extendSelection(tree.getPathForRow(newIndex))
					ElseIf changeSelection Then
						tree.selectionIntervalval(newIndex, newIndex)
					Else
						ui.leadSelectionPathath(tree.getPathForRow(newIndex),True)
					End If
					ui.ensureRowsAreVisible(newIndex, newIndex)
					ui.lastSelectedRow = newIndex
				End If
			End Sub

			Private Sub traverse(ByVal tree As JTree, ByVal ui As BasicTreeUI, ByVal direction As Integer, ByVal changeSelection As Boolean)

				' disable moving of lead unless in discontiguous mode
				If (Not changeSelection) AndAlso tree.selectionModel.selectionMode <> TreeSelectionModel.DISCONTIGUOUS_TREE_SELECTION Then changeSelection = True

				Dim rowCount As Integer

				rowCount = tree.rowCount
				If rowCount > 0 Then
					Dim minSelIndex As Integer = ui.leadSelectionRow
					Dim newIndex As Integer

					If minSelIndex = -1 Then
						newIndex = 0
					Else
	'                     Try and expand the node, otherwise go to next
	'                       node. 
						If direction = 1 Then
							Dim minSelPath As TreePath = ui.getPathForRow(tree, minSelIndex)
							Dim childCount As Integer = tree.model.getChildCount(minSelPath.lastPathComponent)
							newIndex = -1
							If Not ui.isLeaf(minSelIndex) Then
								If Not tree.isExpanded(minSelIndex) Then
									ui.toggleExpandState(minSelPath)
								ElseIf childCount > 0 Then
									newIndex = Math.Min(minSelIndex + 1, rowCount - 1)
								End If
							End If
						' Try to collapse node. 
						Else
							If (Not ui.isLeaf(minSelIndex)) AndAlso tree.isExpanded(minSelIndex) Then
								ui.toggleExpandState(ui.getPathForRow(tree, minSelIndex))
								newIndex = -1
							Else
								Dim path As TreePath = ui.getPathForRow(tree, minSelIndex)

								If path IsNot Nothing AndAlso path.pathCount > 1 Then
									newIndex = ui.getRowForPath(tree, path.parentPath)
								Else
									newIndex = -1
								End If
							End If
						End If
					End If
					If newIndex <> -1 Then
						If changeSelection Then
							tree.selectionIntervalval(newIndex, newIndex)
						Else
							ui.leadSelectionPathath(ui.getPathForRow(tree, newIndex), True)
						End If
						ui.ensureRowsAreVisible(newIndex, newIndex)
					End If
				End If
			End Sub

			Private Sub moveSelectionToParent(ByVal tree As JTree, ByVal ui As BasicTreeUI)
				Dim selRow As Integer = ui.leadSelectionRow
				Dim path As TreePath = ui.getPathForRow(tree, selRow)
				If path IsNot Nothing AndAlso path.pathCount > 1 Then
					Dim newIndex As Integer = ui.getRowForPath(tree, path.parentPath)
					If newIndex <> -1 Then
						tree.selectionIntervalval(newIndex, newIndex)
						ui.ensureRowsAreVisible(newIndex, newIndex)
					End If
				End If
			End Sub

			Private Sub page(ByVal tree As JTree, ByVal ui As BasicTreeUI, ByVal direction As Integer, ByVal addToSelection As Boolean, ByVal changeSelection As Boolean)

				' disable moving of lead unless in discontiguous mode
				If (Not addToSelection) AndAlso (Not changeSelection) AndAlso tree.selectionModel.selectionMode <> TreeSelectionModel.DISCONTIGUOUS_TREE_SELECTION Then changeSelection = True

				Dim rowCount As Integer

				rowCount = ui.getRowCount(tree)
				If rowCount > 0 AndAlso ui.treeSelectionModel IsNot Nothing Then
					Dim maxSize As Dimension = tree.size
					Dim lead As TreePath = ui.leadSelectionPath
					Dim newPath As TreePath
					Dim visRect As Rectangle = tree.visibleRect

					If direction = -1 Then
						' up.
						newPath = ui.getClosestPathForLocation(tree, visRect.x, visRect.y)
						If newPath.Equals(lead) Then
							visRect.y = Math.Max(0, visRect.y - visRect.height)
							newPath = tree.getClosestPathForLocation(visRect.x, visRect.y)
						End If
					Else
						' down
						visRect.y = Math.Min(maxSize.height, visRect.y + visRect.height - 1)
						newPath = tree.getClosestPathForLocation(visRect.x, visRect.y)
						If newPath.Equals(lead) Then
							visRect.y = Math.Min(maxSize.height, visRect.y + visRect.height - 1)
							newPath = tree.getClosestPathForLocation(visRect.x, visRect.y)
						End If
					End If
					Dim newRect As Rectangle = ui.getPathBounds(tree, newPath)
					If newRect IsNot Nothing Then
						newRect.x = visRect.x
						newRect.width = visRect.width
						If direction = -1 Then
							newRect.height = visRect.height
						Else
							newRect.y -= (visRect.height - newRect.height)
							newRect.height = visRect.height
						End If

						If addToSelection Then
							ui.extendSelection(newPath)
						ElseIf changeSelection Then
							tree.selectionPath = newPath
						Else
							ui.leadSelectionPathath(newPath, True)
						End If
						tree.scrollRectToVisible(newRect)
					End If
				End If
			End Sub

			Private Sub home(ByVal tree As JTree, ByVal ui As BasicTreeUI, ByVal direction As Integer, ByVal addToSelection As Boolean, ByVal changeSelection As Boolean)

				' disable moving of lead unless in discontiguous mode
				If (Not addToSelection) AndAlso (Not changeSelection) AndAlso tree.selectionModel.selectionMode <> TreeSelectionModel.DISCONTIGUOUS_TREE_SELECTION Then changeSelection = True

				Dim rowCount As Integer = ui.getRowCount(tree)

				If rowCount > 0 Then
					If direction = -1 Then
						ui.ensureRowsAreVisible(0, 0)
						If addToSelection Then
							Dim aPath As TreePath = ui.anchorSelectionPath
							Dim aRow As Integer = If(aPath Is Nothing, -1, ui.getRowForPath(tree, aPath))

							If aRow = -1 Then
								tree.selectionIntervalval(0, 0)
							Else
								tree.selectionIntervalval(0, aRow)
								ui.anchorSelectionPath = aPath
								ui.leadSelectionPath = ui.getPathForRow(tree, 0)
							End If
						ElseIf changeSelection Then
							tree.selectionIntervalval(0, 0)
						Else
							ui.leadSelectionPathath(ui.getPathForRow(tree, 0), True)
						End If
					Else
						ui.ensureRowsAreVisible(rowCount - 1, rowCount - 1)
						If addToSelection Then
							Dim aPath As TreePath = ui.anchorSelectionPath
							Dim aRow As Integer = If(aPath Is Nothing, -1, ui.getRowForPath(tree, aPath))

							If aRow = -1 Then
								tree.selectionIntervalval(rowCount - 1, rowCount -1)
							Else
								tree.selectionIntervalval(aRow, rowCount - 1)
								ui.anchorSelectionPath = aPath
								ui.leadSelectionPath = ui.getPathForRow(tree, rowCount -1)
							End If
						ElseIf changeSelection Then
							tree.selectionIntervalval(rowCount - 1, rowCount - 1)
						Else
							ui.leadSelectionPathath(ui.getPathForRow(tree, rowCount - 1), True)
						End If
						If ui.largeModel Then
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'							SwingUtilities.invokeLater(New Runnable()
	'						{
	'							public void run()
	'							{
	'								ui.ensureRowsAreVisible(rowCount - 1, rowCount - 1);
	'							}
	'						});
						End If
					End If
				End If
			End Sub
		End Class
	End Class ' End of class BasicTreeUI

End Namespace
Imports Microsoft.VisualBasic
Imports System
Imports System.Text
Imports javax.swing
Imports javax.swing.event
Imports javax.swing.plaf

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
	''' An extensible implementation of {@code ListUI}.
	''' <p>
	''' {@code BasicListUI} instances cannot be shared between multiple
	''' lists.
	''' 
	''' @author Hans Muller
	''' @author Philip Milne
	''' @author Shannon Hickey (drag and drop)
	''' </summary>
	Public Class BasicListUI
		Inherits ListUI

		Private Shared ReadOnly BASELINE_COMPONENT_KEY As New StringBuilder("List.baselineComponent")

		Protected Friend list As JList = Nothing
		Protected Friend rendererPane As CellRendererPane

		' Listeners that this UI attaches to the JList
		Protected Friend focusListener As FocusListener
		Protected Friend mouseInputListener As MouseInputListener
		Protected Friend listSelectionListener As ListSelectionListener
		Protected Friend listDataListener As ListDataListener
		Protected Friend propertyChangeListener As java.beans.PropertyChangeListener
		Private handler As Handler

		Protected Friend cellHeights As Integer() = Nothing
		Protected Friend cellHeight As Integer = -1
		Protected Friend cellWidth As Integer = -1
		Protected Friend updateLayoutStateNeeded As Integer = modelChanged
		''' <summary>
		''' Height of the list. When asked to paint, if the current size of
		''' the list differs, this will update the layout state.
		''' </summary>
		Private listHeight As Integer

		''' <summary>
		''' Width of the list. When asked to paint, if the current size of
		''' the list differs, this will update the layout state.
		''' </summary>
		Private listWidth As Integer

		''' <summary>
		''' The layout orientation of the list.
		''' </summary>
		Private layoutOrientation As Integer

		' Following ivars are used if the list is laying out horizontally

		''' <summary>
		''' Number of columns to create.
		''' </summary>
		Private columnCount As Integer
		''' <summary>
		''' Preferred height to make the list, this is only used if the
		''' the list is layed out horizontally.
		''' </summary>
		Private preferredHeight As Integer
		''' <summary>
		''' Number of rows per column. This is only used if the row height is
		''' fixed.
		''' </summary>
		Private rowsPerColumn As Integer

		''' <summary>
		''' The time factor to treate the series of typed alphanumeric key
		''' as prefix for first letter navigation.
		''' </summary>
		Private timeFactor As Long = 1000L

		''' <summary>
		''' Local cache of JList's client property "List.isFileList"
		''' </summary>
		Private isFileList As Boolean = False

		''' <summary>
		''' Local cache of JList's component orientation property
		''' </summary>
		Private isLeftToRight As Boolean = True

	'     The bits below define JList property changes that affect layout.
	'     * When one of these properties changes we set a bit in
	'     * updateLayoutStateNeeded.  The change is dealt with lazily, see
	'     * maybeUpdateLayoutState.  Changes to the JLists model, e.g. the
	'     * models length changed, are handled similarly, see DataListener.
	'     

		Protected Friend Shared ReadOnly modelChanged As Integer = 1 << 0
		Protected Friend Shared ReadOnly selectionModelChanged As Integer = 1 << 1
		Protected Friend Shared ReadOnly fontChanged As Integer = 1 << 2
		Protected Friend Shared ReadOnly fixedCellWidthChanged As Integer = 1 << 3
		Protected Friend Shared ReadOnly fixedCellHeightChanged As Integer = 1 << 4
		Protected Friend Shared ReadOnly prototypeCellValueChanged As Integer = 1 << 5
		Protected Friend Shared ReadOnly cellRendererChanged As Integer = 1 << 6
		Private Shared ReadOnly layoutOrientationChanged As Integer = 1 << 7
		Private Shared ReadOnly heightChanged As Integer = 1 << 8
		Private Shared ReadOnly widthChanged As Integer = 1 << 9
		Private Shared ReadOnly componentOrientationChanged As Integer = 1 << 10

		Private Const DROP_LINE_THICKNESS As Integer = 2

		Friend Shared Sub loadActionMap(ByVal map As LazyActionMap)
			map.put(New Actions(Actions.SELECT_PREVIOUS_COLUMN))
			map.put(New Actions(Actions.SELECT_PREVIOUS_COLUMN_EXTEND))
			map.put(New Actions(Actions.SELECT_PREVIOUS_COLUMN_CHANGE_LEAD))
			map.put(New Actions(Actions.SELECT_NEXT_COLUMN))
			map.put(New Actions(Actions.SELECT_NEXT_COLUMN_EXTEND))
			map.put(New Actions(Actions.SELECT_NEXT_COLUMN_CHANGE_LEAD))
			map.put(New Actions(Actions.SELECT_PREVIOUS_ROW))
			map.put(New Actions(Actions.SELECT_PREVIOUS_ROW_EXTEND))
			map.put(New Actions(Actions.SELECT_PREVIOUS_ROW_CHANGE_LEAD))
			map.put(New Actions(Actions.SELECT_NEXT_ROW))
			map.put(New Actions(Actions.SELECT_NEXT_ROW_EXTEND))
			map.put(New Actions(Actions.SELECT_NEXT_ROW_CHANGE_LEAD))
			map.put(New Actions(Actions.SELECT_FIRST_ROW))
			map.put(New Actions(Actions.SELECT_FIRST_ROW_EXTEND))
			map.put(New Actions(Actions.SELECT_FIRST_ROW_CHANGE_LEAD))
			map.put(New Actions(Actions.SELECT_LAST_ROW))
			map.put(New Actions(Actions.SELECT_LAST_ROW_EXTEND))
			map.put(New Actions(Actions.SELECT_LAST_ROW_CHANGE_LEAD))
			map.put(New Actions(Actions.SCROLL_UP))
			map.put(New Actions(Actions.SCROLL_UP_EXTEND))
			map.put(New Actions(Actions.SCROLL_UP_CHANGE_LEAD))
			map.put(New Actions(Actions.SCROLL_DOWN))
			map.put(New Actions(Actions.SCROLL_DOWN_EXTEND))
			map.put(New Actions(Actions.SCROLL_DOWN_CHANGE_LEAD))
			map.put(New Actions(Actions.SELECT_ALL))
			map.put(New Actions(Actions.CLEAR_SELECTION))
			map.put(New Actions(Actions.ADD_TO_SELECTION))
			map.put(New Actions(Actions.TOGGLE_AND_ANCHOR))
			map.put(New Actions(Actions.EXTEND_TO))
			map.put(New Actions(Actions.MOVE_SELECTION_TO))

			map.put(TransferHandler.cutAction.getValue(Action.NAME), TransferHandler.cutAction)
			map.put(TransferHandler.copyAction.getValue(Action.NAME), TransferHandler.copyAction)
			map.put(TransferHandler.pasteAction.getValue(Action.NAME), TransferHandler.pasteAction)
		End Sub

		''' <summary>
		''' Paint one List cell: compute the relevant state, get the "rubber stamp"
		''' cell renderer component, and then use the CellRendererPane to paint it.
		''' Subclasses may want to override this method rather than paint().
		''' </summary>
		''' <seealso cref= #paint </seealso>
		Protected Friend Overridable Sub paintCell(ByVal g As Graphics, ByVal row As Integer, ByVal rowBounds As Rectangle, ByVal cellRenderer As ListCellRenderer, ByVal dataModel As ListModel, ByVal selModel As ListSelectionModel, ByVal leadIndex As Integer)
			Dim value As Object = dataModel.getElementAt(row)
			Dim cellHasFocus As Boolean = list.hasFocus() AndAlso (row = leadIndex)
			Dim isSelected As Boolean = selModel.isSelectedIndex(row)

			Dim rendererComponent As Component = cellRenderer.getListCellRendererComponent(list, value, row, isSelected, cellHasFocus)

			Dim cx As Integer = rowBounds.x
			Dim cy As Integer = rowBounds.y
			Dim cw As Integer = rowBounds.width
			Dim ch As Integer = rowBounds.height

			If isFileList Then
				' Shrink renderer to preferred size. This is mostly used on Windows
				' where selection is only shown around the file name, instead of
				' across the whole list cell.
				Dim w As Integer = Math.Min(cw, rendererComponent.preferredSize.width + 4)
				If Not isLeftToRight Then cx += (cw - w)
				cw = w
			End If

			rendererPane.paintComponent(g, rendererComponent, list, cx, cy, cw, ch, True)
		End Sub


		''' <summary>
		''' Paint the rows that intersect the Graphics objects clipRect.  This
		''' method calls paintCell as necessary.  Subclasses
		''' may want to override these methods.
		''' </summary>
		''' <seealso cref= #paintCell </seealso>
		Public Overridable Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			Dim clip As Shape = g.clip
			paintImpl(g, c)
			g.clip = clip

			paintDropLine(g)
		End Sub

		Private Sub paintImpl(ByVal g As Graphics, ByVal c As JComponent)
			Select Case layoutOrientation
			Case JList.VERTICAL_WRAP
				If list.height <> listHeight Then
					updateLayoutStateNeeded = updateLayoutStateNeeded Or heightChanged
					redrawList()
				End If
			Case JList.HORIZONTAL_WRAP
				If list.width <> listWidth Then
					updateLayoutStateNeeded = updateLayoutStateNeeded Or widthChanged
					redrawList()
				End If
			Case Else
			End Select
			maybeUpdateLayoutState()

			Dim renderer As ListCellRenderer = list.cellRenderer
			Dim dataModel As ListModel = list.model
			Dim selModel As ListSelectionModel = list.selectionModel
			Dim size As Integer

			size = dataModel.size
			If (renderer Is Nothing) OrElse size = 0 Then Return

			' Determine how many columns we need to paint
			Dim paintBounds As Rectangle = g.clipBounds

			Dim startColumn, endColumn As Integer
			If c.componentOrientation.leftToRight Then
				startColumn = convertLocationToColumn(paintBounds.x, paintBounds.y)
				endColumn = convertLocationToColumn(paintBounds.x + paintBounds.width, paintBounds.y)
			Else
				startColumn = convertLocationToColumn(paintBounds.x + paintBounds.width, paintBounds.y)
				endColumn = convertLocationToColumn(paintBounds.x, paintBounds.y)
			End If
			Dim maxY As Integer = paintBounds.y + paintBounds.height
			Dim leadIndex As Integer = adjustIndex(list.leadSelectionIndex, list)
			Dim rowIncrement As Integer = If(layoutOrientation = JList.HORIZONTAL_WRAP, columnCount, 1)


			For colCounter As Integer = startColumn To endColumn
				' And then how many rows in this columnn
				Dim row As Integer = convertLocationToRowInColumn(paintBounds.y, colCounter)
				Dim ___rowCount As Integer = getRowCount(colCounter)
				Dim index As Integer = getModelIndex(colCounter, row)
				Dim rowBounds As Rectangle = getCellBounds(list, index, index)

				If rowBounds Is Nothing Then Return
				Do While row < ___rowCount AndAlso rowBounds.y < maxY AndAlso index < size
					rowBounds.height = getHeight(colCounter, row)
					g.cliplip(rowBounds.x, rowBounds.y, rowBounds.width, rowBounds.height)
					g.clipRect(paintBounds.x, paintBounds.y, paintBounds.width, paintBounds.height)
					paintCell(g, index, rowBounds, renderer, dataModel, selModel, leadIndex)
					rowBounds.y += rowBounds.height
					index += rowIncrement
					row += 1
				Loop
			Next colCounter
			' Empty out the renderer pane, allowing renderers to be gc'ed.
			rendererPane.removeAll()
		End Sub

		Private Sub paintDropLine(ByVal g As Graphics)
			Dim loc As JList.DropLocation = list.dropLocation
			If loc Is Nothing OrElse (Not loc.insert) Then Return

			Dim c As Color = sun.swing.DefaultLookup.getColor(list, Me, "List.dropLineColor", Nothing)
			If c IsNot Nothing Then
				g.color = c
				Dim rect As Rectangle = getDropLineRect(loc)
				g.fillRect(rect.x, rect.y, rect.width, rect.height)
			End If
		End Sub

		Private Function getDropLineRect(ByVal loc As JList.DropLocation) As Rectangle
			Dim size As Integer = list.model.size

			If size = 0 Then
				Dim insets As Insets = list.insets
				If layoutOrientation = JList.HORIZONTAL_WRAP Then
					If isLeftToRight Then
						Return New Rectangle(insets.left, insets.top, DROP_LINE_THICKNESS, 20)
					Else
						Return New Rectangle(list.width - DROP_LINE_THICKNESS - insets.right, insets.top, DROP_LINE_THICKNESS, 20)
					End If
				Else
					Return New Rectangle(insets.left, insets.top, list.width - insets.left - insets.right, DROP_LINE_THICKNESS)
				End If
			End If

			Dim rect As Rectangle = Nothing
			Dim index As Integer = loc.index
			Dim decr As Boolean = False

			If layoutOrientation = JList.HORIZONTAL_WRAP Then
				If index = size Then
					decr = True
				ElseIf index <> 0 AndAlso convertModelToRow(index) <> convertModelToRow(index - 1) Then

					Dim prev As Rectangle = getCellBounds(list, index - 1)
					Dim [me] As Rectangle = getCellBounds(list, index)
					Dim p As Point = loc.dropPoint

					If isLeftToRight Then
						decr = java.awt.geom.Point2D.distance(prev.x + prev.width, prev.y + CInt(Fix(prev.height / 2.0)), p.x, p.y) < java.awt.geom.Point2D.distance([me].x, [me].y + CInt(Fix([me].height / 2.0)), p.x, p.y)
					Else
						decr = java.awt.geom.Point2D.distance(prev.x, prev.y + CInt(Fix(prev.height / 2.0)), p.x, p.y) < java.awt.geom.Point2D.distance([me].x + [me].width, [me].y + CInt(Fix(prev.height / 2.0)), p.x, p.y)
					End If
				End If

				If decr Then
					index -= 1
					rect = getCellBounds(list, index)
					If isLeftToRight Then
						rect.x += rect.width
					Else
						rect.x -= DROP_LINE_THICKNESS
					End If
				Else
					rect = getCellBounds(list, index)
					If Not isLeftToRight Then rect.x += rect.width - DROP_LINE_THICKNESS
				End If

				If rect.x >= list.width Then
					rect.x = list.width - DROP_LINE_THICKNESS
				ElseIf rect.x < 0 Then
					rect.x = 0
				End If

				rect.width = DROP_LINE_THICKNESS
			ElseIf layoutOrientation = JList.VERTICAL_WRAP Then
				If index = size Then
					index -= 1
					rect = getCellBounds(list, index)
					rect.y += rect.height
				ElseIf index <> 0 AndAlso convertModelToColumn(index) <> convertModelToColumn(index - 1) Then

					Dim prev As Rectangle = getCellBounds(list, index - 1)
					Dim [me] As Rectangle = getCellBounds(list, index)
					Dim p As Point = loc.dropPoint
					If java.awt.geom.Point2D.distance(prev.x + CInt(Fix(prev.width / 2.0)), prev.y + prev.height, p.x, p.y) < java.awt.geom.Point2D.distance([me].x + CInt(Fix([me].width / 2.0)), [me].y, p.x, p.y) Then

						index -= 1
						rect = getCellBounds(list, index)
						rect.y += rect.height
					Else
						rect = getCellBounds(list, index)
					End If
				Else
					rect = getCellBounds(list, index)
				End If

				If rect.y >= list.height Then rect.y = list.height - DROP_LINE_THICKNESS

				rect.height = DROP_LINE_THICKNESS
			Else
				If index = size Then
					index -= 1
					rect = getCellBounds(list, index)
					rect.y += rect.height
				Else
					rect = getCellBounds(list, index)
				End If

				If rect.y >= list.height Then rect.y = list.height - DROP_LINE_THICKNESS

				rect.height = DROP_LINE_THICKNESS
			End If

			Return rect
		End Function

		''' <summary>
		''' Returns the baseline.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaseline(ByVal c As JComponent, ByVal width As Integer, ByVal height As Integer) As Integer
			MyBase.getBaseline(c, width, height)
			Dim ___rowHeight As Integer = list.fixedCellHeight
			Dim lafDefaults As UIDefaults = UIManager.lookAndFeelDefaults
			Dim renderer As Component = CType(lafDefaults(BASELINE_COMPONENT_KEY), Component)
			If renderer Is Nothing Then
				Dim lcr As ListCellRenderer = CType(UIManager.get("List.cellRenderer"), ListCellRenderer)

				' fix for 6711072 some LAFs like Nimbus do not provide this
				' UIManager key and we should not through a NPE here because of it
				If lcr Is Nothing Then lcr = New DefaultListCellRenderer
				renderer = lcr.getListCellRendererComponent(list, "a", -1, False, False)
				lafDefaults(BASELINE_COMPONENT_KEY) = renderer
			End If
			renderer.font = list.font
			' JList actually has much more complex behavior here.
			' If rowHeight != -1 the rowHeight is either the max of all cell
			' heights (layout orientation != VERTICAL), or is variable depending
			' upon the cell.  We assume a default size.
			' We could theoretically query the real renderer, but that would
			' not work for an empty model and the results may vary with
			' the content.
			If ___rowHeight = -1 Then ___rowHeight = renderer.preferredSize.height
			Return renderer.getBaseline(Integer.MaxValue, ___rowHeight) + list.insets.top
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

		''' <summary>
		''' The preferredSize of the list depends upon the layout orientation.
		''' <table summary="Describes the preferred size for each layout orientation">
		''' <tr><th>Layout Orientation</th><th>Preferred Size</th></tr>
		''' <tr>
		'''   <td>JList.VERTICAL
		'''   <td>The preferredSize of the list is total height of the rows
		'''       and the maximum width of the cells.  If JList.fixedCellHeight
		'''       is specified then the total height of the rows is just
		'''       (cellVerticalMargins + fixedCellHeight) * model.getSize() where
		'''       rowVerticalMargins is the space we allocate for drawing
		'''       the yellow focus outline.  Similarly if fixedCellWidth is
		'''       specified then we just use that.
		'''   </td>
		''' <tr>
		'''   <td>JList.VERTICAL_WRAP
		'''   <td>If the visible row count is greater than zero, the preferredHeight
		'''       is the maximum cell height * visibleRowCount. If the visible row
		'''       count is &lt;= 0, the preferred height is either the current height
		'''       of the list, or the maximum cell height, whichever is
		'''       bigger. The preferred width is than the maximum cell width *
		'''       number of columns needed. Where the number of columns needs is
		'''       list.height / max cell height. Max cell height is either the fixed
		'''       cell height, or is determined by iterating through all the cells
		'''       to find the maximum height from the ListCellRenderer.
		''' <tr>
		'''   <td>JList.HORIZONTAL_WRAP
		'''   <td>If the visible row count is greater than zero, the preferredHeight
		'''       is the maximum cell height * adjustedRowCount.  Where
		'''       visibleRowCount is used to determine the number of columns.
		'''       Because this lays out horizontally the number of rows is
		'''       then determined from the column count.  For example, lets say
		'''       you have a model with 10 items and the visible row count is 8.
		'''       The number of columns needed to display this is 2, but you no
		'''       longer need 8 rows to display this, you only need 5, thus
		'''       the adjustedRowCount is 5.
		'''       <p>If the visible row
		'''       count is &lt;= 0, the preferred height is dictated by the
		'''       number of columns, which will be as many as can fit in the width
		'''       of the <code>JList</code> (width / max cell width), with at
		'''       least one column.  The preferred height then becomes the
		'''       model size / number of columns * maximum cell height.
		'''       Max cell height is either the fixed
		'''       cell height, or is determined by iterating through all the cells
		'''       to find the maximum height from the ListCellRenderer.
		''' </table>
		''' The above specifies the raw preferred width and height. The resulting
		''' preferred width is the above width + insets.left + insets.right and
		''' the resulting preferred height is the above height + insets.top +
		''' insets.bottom. Where the <code>Insets</code> are determined from
		''' <code>list.getInsets()</code>.
		''' </summary>
		''' <param name="c"> The JList component. </param>
		''' <returns> The total size of the list. </returns>
		Public Overridable Function getPreferredSize(ByVal c As JComponent) As Dimension
			maybeUpdateLayoutState()

			Dim lastRow As Integer = list.model.size - 1
			If lastRow < 0 Then Return New Dimension(0, 0)

			Dim insets As Insets = list.insets
			Dim width As Integer = cellWidth * columnCount + insets.left + insets.right
			Dim ___height As Integer

			If layoutOrientation <> JList.VERTICAL Then
				___height = preferredHeight
			Else
				Dim bounds As Rectangle = getCellBounds(list, lastRow)

				If bounds IsNot Nothing Then
					___height = bounds.y + bounds.height + insets.bottom
				Else
					___height = 0
				End If
			End If
			Return New Dimension(width, ___height)
		End Function


		''' <summary>
		''' Selected the previous row and force it to be visible.
		''' </summary>
		''' <seealso cref= JList#ensureIndexIsVisible </seealso>
		Protected Friend Overridable Sub selectPreviousIndex()
			Dim s As Integer = list.selectedIndex
			If s > 0 Then
				s -= 1
				list.selectedIndex = s
				list.ensureIndexIsVisible(s)
			End If
		End Sub


		''' <summary>
		''' Selected the previous row and force it to be visible.
		''' </summary>
		''' <seealso cref= JList#ensureIndexIsVisible </seealso>
		Protected Friend Overridable Sub selectNextIndex()
			Dim s As Integer = list.selectedIndex
			If (s + 1) < list.model.size Then
				s += 1
				list.selectedIndex = s
				list.ensureIndexIsVisible(s)
			End If
		End Sub


		''' <summary>
		''' Registers the keyboard bindings on the <code>JList</code> that the
		''' <code>BasicListUI</code> is associated with. This method is called at
		''' installUI() time.
		''' </summary>
		''' <seealso cref= #installUI </seealso>
		Protected Friend Overridable Sub installKeyboardActions()
			Dim ___inputMap As InputMap = getInputMap(JComponent.WHEN_FOCUSED)

			SwingUtilities.replaceUIInputMap(list, JComponent.WHEN_FOCUSED, ___inputMap)

			LazyActionMap.installLazyActionMap(list, GetType(BasicListUI), "List.actionMap")
		End Sub

		Friend Overridable Function getInputMap(ByVal condition As Integer) As InputMap
			If condition = JComponent.WHEN_FOCUSED Then
				Dim keyMap As InputMap = CType(sun.swing.DefaultLookup.get(list, Me, "List.focusInputMap"), InputMap)
				Dim rtlKeyMap As InputMap

				rtlKeyMap = CType(sun.swing.DefaultLookup.get(list, Me, "List.focusInputMap.RightToLeft"), InputMap)
				If isLeftToRight OrElse (rtlKeyMap Is Nothing) Then
						Return keyMap
				Else
					rtlKeyMap.parent = keyMap
					Return rtlKeyMap
				End If
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Unregisters keyboard actions installed from
		''' <code>installKeyboardActions</code>.
		''' This method is called at uninstallUI() time - subclassess should
		''' ensure that all of the keyboard actions registered at installUI
		''' time are removed here.
		''' </summary>
		''' <seealso cref= #installUI </seealso>
		Protected Friend Overridable Sub uninstallKeyboardActions()
			SwingUtilities.replaceUIActionMap(list, Nothing)
			SwingUtilities.replaceUIInputMap(list, JComponent.WHEN_FOCUSED, Nothing)
		End Sub


		''' <summary>
		''' Creates and installs the listeners for the JList, its model, and its
		''' selectionModel.  This method is called at installUI() time.
		''' </summary>
		''' <seealso cref= #installUI </seealso>
		''' <seealso cref= #uninstallListeners </seealso>
		Protected Friend Overridable Sub installListeners()
			Dim th As TransferHandler = list.transferHandler
			If th Is Nothing OrElse TypeOf th Is UIResource Then
				list.transferHandler = defaultTransferHandler
				' default TransferHandler doesn't support drop
				' so we don't want drop handling
				If TypeOf list.dropTarget Is UIResource Then list.dropTarget = Nothing
			End If

			focusListener = createFocusListener()
			mouseInputListener = createMouseInputListener()
			propertyChangeListener = createPropertyChangeListener()
			listSelectionListener = createListSelectionListener()
			listDataListener = createListDataListener()

			list.addFocusListener(focusListener)
			list.addMouseListener(mouseInputListener)
			list.addMouseMotionListener(mouseInputListener)
			list.addPropertyChangeListener(propertyChangeListener)
			list.addKeyListener(handler)

			Dim model As ListModel = list.model
			If model IsNot Nothing Then model.addListDataListener(listDataListener)

			Dim selectionModel As ListSelectionModel = list.selectionModel
			If selectionModel IsNot Nothing Then selectionModel.addListSelectionListener(listSelectionListener)
		End Sub


		''' <summary>
		''' Removes the listeners from the JList, its model, and its
		''' selectionModel.  All of the listener fields, are reset to
		''' null here.  This method is called at uninstallUI() time,
		''' it should be kept in sync with installListeners.
		''' </summary>
		''' <seealso cref= #uninstallUI </seealso>
		''' <seealso cref= #installListeners </seealso>
		Protected Friend Overridable Sub uninstallListeners()
			list.removeFocusListener(focusListener)
			list.removeMouseListener(mouseInputListener)
			list.removeMouseMotionListener(mouseInputListener)
			list.removePropertyChangeListener(propertyChangeListener)
			list.removeKeyListener(handler)

			Dim model As ListModel = list.model
			If model IsNot Nothing Then model.removeListDataListener(listDataListener)

			Dim selectionModel As ListSelectionModel = list.selectionModel
			If selectionModel IsNot Nothing Then selectionModel.removeListSelectionListener(listSelectionListener)

			focusListener = Nothing
			mouseInputListener = Nothing
			listSelectionListener = Nothing
			listDataListener = Nothing
			propertyChangeListener = Nothing
			handler = Nothing
		End Sub


		''' <summary>
		''' Initializes list properties such as font, foreground, and background,
		''' and adds the CellRendererPane. The font, foreground, and background
		''' properties are only set if their current value is either null
		''' or a UIResource, other properties are set if the current
		''' value is null.
		''' </summary>
		''' <seealso cref= #uninstallDefaults </seealso>
		''' <seealso cref= #installUI </seealso>
		''' <seealso cref= CellRendererPane </seealso>
		Protected Friend Overridable Sub installDefaults()
			list.layout = Nothing

			LookAndFeel.installBorder(list, "List.border")

			LookAndFeel.installColorsAndFont(list, "List.background", "List.foreground", "List.font")

			LookAndFeel.installProperty(list, "opaque", Boolean.TRUE)

			If list.cellRenderer Is Nothing Then list.cellRenderer = CType(UIManager.get("List.cellRenderer"), ListCellRenderer)

			Dim sbg As Color = list.selectionBackground
			If sbg Is Nothing OrElse TypeOf sbg Is UIResource Then list.selectionBackground = UIManager.getColor("List.selectionBackground")

			Dim sfg As Color = list.selectionForeground
			If sfg Is Nothing OrElse TypeOf sfg Is UIResource Then list.selectionForeground = UIManager.getColor("List.selectionForeground")

			Dim l As Long? = CLng(Fix(UIManager.get("List.timeFactor")))
			timeFactor = If(l IsNot Nothing, l, 1000L)

			updateIsFileList()
		End Sub

		Private Sub updateIsFileList()
			Dim b As Boolean = Boolean.TRUE.Equals(list.getClientProperty("List.isFileList"))
			If b <> isFileList Then
				isFileList = b
				Dim oldFont As Font = list.font
				If oldFont Is Nothing OrElse TypeOf oldFont Is UIResource Then
					Dim newFont As Font = UIManager.getFont(If(b, "FileChooser.listFont", "List.font"))
					If newFont IsNot Nothing AndAlso newFont IsNot oldFont Then list.font = newFont
				End If
			End If
		End Sub


		''' <summary>
		''' Sets the list properties that have not been explicitly overridden to
		''' {@code null}. A property is considered overridden if its current value
		''' is not a {@code UIResource}.
		''' </summary>
		''' <seealso cref= #installDefaults </seealso>
		''' <seealso cref= #uninstallUI </seealso>
		''' <seealso cref= CellRendererPane </seealso>
		Protected Friend Overridable Sub uninstallDefaults()
			LookAndFeel.uninstallBorder(list)
			If TypeOf list.font Is UIResource Then list.font = Nothing
			If TypeOf list.foreground Is UIResource Then list.foreground = Nothing
			If TypeOf list.background Is UIResource Then list.background = Nothing
			If TypeOf list.selectionBackground Is UIResource Then list.selectionBackground = Nothing
			If TypeOf list.selectionForeground Is UIResource Then list.selectionForeground = Nothing
			If TypeOf list.cellRenderer Is UIResource Then list.cellRenderer = Nothing
			If TypeOf list.transferHandler Is UIResource Then list.transferHandler = Nothing
		End Sub


		''' <summary>
		''' Initializes <code>this.list</code> by calling <code>installDefaults()</code>,
		''' <code>installListeners()</code>, and <code>installKeyboardActions()</code>
		''' in order.
		''' </summary>
		''' <seealso cref= #installDefaults </seealso>
		''' <seealso cref= #installListeners </seealso>
		''' <seealso cref= #installKeyboardActions </seealso>
		Public Overridable Sub installUI(ByVal c As JComponent)
			list = CType(c, JList)

			layoutOrientation = list.layoutOrientation

			rendererPane = New CellRendererPane
			list.add(rendererPane)

			columnCount = 1

			updateLayoutStateNeeded = modelChanged
			isLeftToRight = list.componentOrientation.leftToRight

			installDefaults()
			installListeners()
			installKeyboardActions()
		End Sub


		''' <summary>
		''' Uninitializes <code>this.list</code> by calling <code>uninstallListeners()</code>,
		''' <code>uninstallKeyboardActions()</code>, and <code>uninstallDefaults()</code>
		''' in order.  Sets this.list to null.
		''' </summary>
		''' <seealso cref= #uninstallListeners </seealso>
		''' <seealso cref= #uninstallKeyboardActions </seealso>
		''' <seealso cref= #uninstallDefaults </seealso>
		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			uninstallListeners()
			uninstallDefaults()
			uninstallKeyboardActions()

				cellHeight = -1
				cellWidth = cellHeight
			cellHeights = Nothing

				listHeight = -1
				listWidth = listHeight

			list.remove(rendererPane)
			rendererPane = Nothing
			list = Nothing
		End Sub


		''' <summary>
		''' Returns a new instance of BasicListUI.  BasicListUI delegates are
		''' allocated one per JList.
		''' </summary>
		''' <returns> A new ListUI implementation for the Windows look and feel. </returns>
		Public Shared Function createUI(ByVal list As JComponent) As ComponentUI
			Return New BasicListUI
		End Function


		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Function locationToIndex(ByVal list As JList, ByVal location As Point) As Integer
			maybeUpdateLayoutState()
			Return convertLocationToModel(location.x, location.y)
		End Function


		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Function indexToLocation(ByVal list As JList, ByVal index As Integer) As Point
			maybeUpdateLayoutState()
			Dim rect As Rectangle = getCellBounds(list, index, index)

			If rect IsNot Nothing Then Return New Point(rect.x, rect.y)
			Return Nothing
		End Function


		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Function getCellBounds(ByVal list As JList, ByVal index1 As Integer, ByVal index2 As Integer) As Rectangle
			maybeUpdateLayoutState()

			Dim minIndex As Integer = Math.Min(index1, index2)
			Dim maxIndex As Integer = Math.Max(index1, index2)

			If minIndex >= list.model.size Then Return Nothing

			Dim minBounds As Rectangle = getCellBounds(list, minIndex)

			If minBounds Is Nothing Then Return Nothing
			If minIndex = maxIndex Then Return minBounds
			Dim maxBounds As Rectangle = getCellBounds(list, maxIndex)

			If maxBounds IsNot Nothing Then
				If layoutOrientation = JList.HORIZONTAL_WRAP Then
					Dim minRow As Integer = convertModelToRow(minIndex)
					Dim maxRow As Integer = convertModelToRow(maxIndex)

					If minRow <> maxRow Then
						minBounds.x = 0
						minBounds.width = list.width
					End If
				ElseIf minBounds.x <> maxBounds.x Then
					' Different columns
					minBounds.y = 0
					minBounds.height = list.height
				End If
				minBounds.add(maxBounds)
			End If
			Return minBounds
		End Function

		''' <summary>
		''' Gets the bounds of the specified model index, returning the resulting
		''' bounds, or null if <code>index</code> is not valid.
		''' </summary>
		Private Function getCellBounds(ByVal list As JList, ByVal index As Integer) As Rectangle
			maybeUpdateLayoutState()

			Dim row As Integer = convertModelToRow(index)
			Dim column As Integer = convertModelToColumn(index)

			If row = -1 OrElse column = -1 Then Return Nothing

			Dim insets As Insets = list.insets
			Dim x As Integer
			Dim w As Integer = cellWidth
			Dim y As Integer = insets.top
			Dim h As Integer
			Select Case layoutOrientation
			Case JList.VERTICAL_WRAP, JList.HORIZONTAL_WRAP
				If isLeftToRight Then
					x = insets.left + column * cellWidth
				Else
					x = list.width - insets.right - (column+1) * cellWidth
				End If
				y += cellHeight * row
				h = cellHeight
			Case Else
				x = insets.left
				If cellHeights Is Nothing Then
					y += (cellHeight * row)
				ElseIf row >= cellHeights.Length Then
					y = 0
				Else
					For i As Integer = 0 To row - 1
						y += cellHeights(i)
					Next i
				End If
				w = list.width - (insets.left + insets.right)
				h = getRowHeight(index)
			End Select
			Return New Rectangle(x, y, w, h)
		End Function

		''' <summary>
		''' Returns the height of the specified row based on the current layout.
		''' </summary>
		''' <returns> The specified row height or -1 if row isn't valid. </returns>
		''' <seealso cref= #convertYToRow </seealso>
		''' <seealso cref= #convertRowToY </seealso>
		''' <seealso cref= #updateLayoutState </seealso>
		Protected Friend Overridable Function getRowHeight(ByVal row As Integer) As Integer
			Return getHeight(0, row)
		End Function


		''' <summary>
		''' Convert the JList relative coordinate to the row that contains it,
		''' based on the current layout.  If y0 doesn't fall within any row,
		''' return -1.
		''' </summary>
		''' <returns> The row that contains y0, or -1. </returns>
		''' <seealso cref= #getRowHeight </seealso>
		''' <seealso cref= #updateLayoutState </seealso>
		Protected Friend Overridable Function convertYToRow(ByVal y0 As Integer) As Integer
			Return convertLocationToRow(0, y0, False)
		End Function


		''' <summary>
		''' Return the JList relative Y coordinate of the origin of the specified
		''' row or -1 if row isn't valid.
		''' </summary>
		''' <returns> The Y coordinate of the origin of row, or -1. </returns>
		''' <seealso cref= #getRowHeight </seealso>
		''' <seealso cref= #updateLayoutState </seealso>
		Protected Friend Overridable Function convertRowToY(ByVal row As Integer) As Integer
			If row >= getRowCount(0) OrElse row < 0 Then Return -1
			Dim bounds As Rectangle = getCellBounds(list, row, row)
			Return bounds.y
		End Function

		''' <summary>
		''' Returns the height of the cell at the passed in location.
		''' </summary>
		Private Function getHeight(ByVal column As Integer, ByVal row As Integer) As Integer
			If column < 0 OrElse column > columnCount OrElse row < 0 Then Return -1
			If layoutOrientation <> JList.VERTICAL Then Return cellHeight
			If row >= list.model.size Then Return -1
			Return If(cellHeights Is Nothing, cellHeight, (If(row < cellHeights.Length, cellHeights(row), -1)))
		End Function

		''' <summary>
		''' Returns the row at location x/y.
		''' </summary>
		''' <param name="closest"> If true and the location doesn't exactly match a
		'''                particular location, this will return the closest row. </param>
		Private Function convertLocationToRow(ByVal x As Integer, ByVal y0 As Integer, ByVal closest As Boolean) As Integer
			Dim size As Integer = list.model.size

			If size <= 0 Then Return -1
			Dim insets As Insets = list.insets
			If cellHeights Is Nothing Then
				Dim row As Integer = If(cellHeight = 0, 0, ((y0 - insets.top) / cellHeight))
				If closest Then
					If row < 0 Then
						row = 0
					ElseIf row >= size Then
						row = size - 1
					End If
				End If
				Return row
			ElseIf size > cellHeights.Length Then
				Return -1
			Else
				Dim y As Integer = insets.top
				Dim row As Integer = 0

				If closest AndAlso y0 < y Then Return 0
				Dim i As Integer
				For i = 0 To size - 1
					If (y0 >= y) AndAlso (y0 < y + cellHeights(i)) Then Return row
					y += cellHeights(i)
					row += 1
				Next i
				Return i - 1
			End If
		End Function

		''' <summary>
		''' Returns the closest row that starts at the specified y-location
		''' in the passed in column.
		''' </summary>
		Private Function convertLocationToRowInColumn(ByVal y As Integer, ByVal column As Integer) As Integer
			Dim x As Integer = 0

			If layoutOrientation <> JList.VERTICAL Then
				If isLeftToRight Then
					x = column * cellWidth
				Else
					x = list.width - (column+1)*cellWidth - list.insets.right
				End If
			End If
			Return convertLocationToRow(x, y, True)
		End Function

		''' <summary>
		''' Returns the closest location to the model index of the passed in
		''' location.
		''' </summary>
		Private Function convertLocationToModel(ByVal x As Integer, ByVal y As Integer) As Integer
			Dim row As Integer = convertLocationToRow(x, y, True)
			Dim column As Integer = convertLocationToColumn(x, y)

			If row >= 0 AndAlso column >= 0 Then Return getModelIndex(column, row)
			Return -1
		End Function

		''' <summary>
		''' Returns the number of rows in the given column.
		''' </summary>
		Private Function getRowCount(ByVal column As Integer) As Integer
			If column < 0 OrElse column >= columnCount Then Return -1
			If layoutOrientation = JList.VERTICAL OrElse (column = 0 AndAlso columnCount = 1) Then Return list.model.size
			If column >= columnCount Then Return -1
			If layoutOrientation = JList.VERTICAL_WRAP Then
				If column < (columnCount - 1) Then Return rowsPerColumn
				Return list.model.size - (columnCount - 1) * rowsPerColumn
			End If
			' JList.HORIZONTAL_WRAP
			Dim diff As Integer = columnCount - (columnCount * rowsPerColumn - list.model.size)

			If column >= diff Then Return Math.Max(0, rowsPerColumn - 1)
			Return rowsPerColumn
		End Function

		''' <summary>
		''' Returns the model index for the specified display location.
		''' If <code>column</code>x<code>row</code> is beyond the length of the
		''' model, this will return the model size - 1.
		''' </summary>
		Private Function getModelIndex(ByVal column As Integer, ByVal row As Integer) As Integer
			Select Case layoutOrientation
			Case JList.VERTICAL_WRAP
				Return Math.Min(list.model.size - 1, rowsPerColumn * column + Math.Min(row, rowsPerColumn-1))
			Case JList.HORIZONTAL_WRAP
				Return Math.Min(list.model.size - 1, row * columnCount + column)
			Case Else
				Return row
			End Select
		End Function

		''' <summary>
		''' Returns the closest column to the passed in location.
		''' </summary>
		Private Function convertLocationToColumn(ByVal x As Integer, ByVal y As Integer) As Integer
			If cellWidth > 0 Then
				If layoutOrientation = JList.VERTICAL Then Return 0
				Dim insets As Insets = list.insets
				Dim col As Integer
				If isLeftToRight Then
					col = (x - insets.left) / cellWidth
				Else
					col = (list.width - x - insets.right - 1) / cellWidth
				End If
				If col < 0 Then
					Return 0
				ElseIf col >= columnCount Then
					Return columnCount - 1
				End If
				Return col
			End If
			Return 0
		End Function

		''' <summary>
		''' Returns the row that the model index <code>index</code> will be
		''' displayed in..
		''' </summary>
		Private Function convertModelToRow(ByVal index As Integer) As Integer
			Dim size As Integer = list.model.size

			If (index < 0) OrElse (index >= size) Then Return -1

			If layoutOrientation <> JList.VERTICAL AndAlso columnCount > 1 AndAlso rowsPerColumn > 0 Then
				If layoutOrientation = JList.VERTICAL_WRAP Then Return index Mod rowsPerColumn
				Return index \ columnCount
			End If
			Return index
		End Function

		''' <summary>
		''' Returns the column that the model index <code>index</code> will be
		''' displayed in.
		''' </summary>
		Private Function convertModelToColumn(ByVal index As Integer) As Integer
			Dim size As Integer = list.model.size

			If (index < 0) OrElse (index >= size) Then Return -1

			If layoutOrientation <> JList.VERTICAL AndAlso rowsPerColumn > 0 AndAlso columnCount > 1 Then
				If layoutOrientation = JList.VERTICAL_WRAP Then Return index \ rowsPerColumn
				Return index Mod columnCount
			End If
			Return 0
		End Function

		''' <summary>
		''' If updateLayoutStateNeeded is non zero, call updateLayoutState() and reset
		''' updateLayoutStateNeeded.  This method should be called by methods
		''' before doing any computation based on the geometry of the list.
		''' For example it's the first call in paint() and getPreferredSize().
		''' </summary>
		''' <seealso cref= #updateLayoutState </seealso>
		Protected Friend Overridable Sub maybeUpdateLayoutState()
			If updateLayoutStateNeeded <> 0 Then
				updateLayoutState()
				updateLayoutStateNeeded = 0
			End If
		End Sub


		''' <summary>
		''' Recompute the value of cellHeight or cellHeights based
		''' and cellWidth, based on the current font and the current
		''' values of fixedCellWidth, fixedCellHeight, and prototypeCellValue.
		''' </summary>
		''' <seealso cref= #maybeUpdateLayoutState </seealso>
		Protected Friend Overridable Sub updateLayoutState()
	'         If both JList fixedCellWidth and fixedCellHeight have been
	'         * set, then initialize cellWidth and cellHeight, and set
	'         * cellHeights to null.
	'         

			Dim fixedCellHeight As Integer = list.fixedCellHeight
			Dim fixedCellWidth As Integer = list.fixedCellWidth

			cellWidth = If(fixedCellWidth <> -1, fixedCellWidth, -1)

			If fixedCellHeight <> -1 Then
				cellHeight = fixedCellHeight
				cellHeights = Nothing
			Else
				cellHeight = -1
				cellHeights = New Integer(list.model.size - 1){}
			End If

	'         If either of  JList fixedCellWidth and fixedCellHeight haven't
	'         * been set, then initialize cellWidth and cellHeights by
	'         * scanning through the entire model.  Note: if the renderer is
	'         * null, we just set cellWidth and cellHeights[*] to zero,
	'         * if they're not set already.
	'         

			If (fixedCellWidth = -1) OrElse (fixedCellHeight = -1) Then

				Dim dataModel As ListModel = list.model
				Dim dataModelSize As Integer = dataModel.size
				Dim renderer As ListCellRenderer = list.cellRenderer

				If renderer IsNot Nothing Then
					For index As Integer = 0 To dataModelSize - 1
						Dim value As Object = dataModel.getElementAt(index)
						Dim c As Component = renderer.getListCellRendererComponent(list, value, index, False, False)
						rendererPane.add(c)
						Dim cellSize As Dimension = c.preferredSize
						If fixedCellWidth = -1 Then cellWidth = Math.Max(cellSize.width, cellWidth)
						If fixedCellHeight = -1 Then cellHeights(index) = cellSize.height
					Next index
				Else
					If cellWidth = -1 Then cellWidth = 0
					If cellHeights Is Nothing Then cellHeights = New Integer(dataModelSize - 1){}
					For index As Integer = 0 To dataModelSize - 1
						cellHeights(index) = 0
					Next index
				End If
			End If

			columnCount = 1
			If layoutOrientation <> JList.VERTICAL Then updateHorizontalLayoutState(fixedCellWidth, fixedCellHeight)
		End Sub

		''' <summary>
		''' Invoked when the list is layed out horizontally to determine how
		''' many columns to create.
		''' <p>
		''' This updates the <code>rowsPerColumn, </code><code>columnCount</code>,
		''' <code>preferredHeight</code> and potentially <code>cellHeight</code>
		''' instance variables.
		''' </summary>
		Private Sub updateHorizontalLayoutState(ByVal fixedCellWidth As Integer, ByVal fixedCellHeight As Integer)
			Dim visRows As Integer = list.visibleRowCount
			Dim dataModelSize As Integer = list.model.size
			Dim insets As Insets = list.insets

			listHeight = list.height
			listWidth = list.width

			If dataModelSize = 0 Then
					columnCount = 0
					rowsPerColumn = columnCount
				preferredHeight = insets.top + insets.bottom
				Return
			End If

			Dim ___height As Integer

			If fixedCellHeight <> -1 Then
				___height = fixedCellHeight
			Else
				' Determine the max of the renderer heights.
				Dim maxHeight As Integer = 0
				If cellHeights.Length > 0 Then
					maxHeight = cellHeights(cellHeights.Length - 1)
					For counter As Integer = cellHeights.Length - 2 To 0 Step -1
						maxHeight = Math.Max(maxHeight, cellHeights(counter))
					Next counter
				End If
					cellHeight = maxHeight
					___height = cellHeight
				cellHeights = Nothing
			End If
			' The number of rows is either determined by the visible row
			' count, or by the height of the list.
			rowsPerColumn = dataModelSize
			If visRows > 0 Then
				rowsPerColumn = visRows
				columnCount = Math.Max(1, dataModelSize \ rowsPerColumn)
				If dataModelSize > 0 AndAlso dataModelSize > rowsPerColumn AndAlso dataModelSize Mod rowsPerColumn <> 0 Then columnCount += 1
				If layoutOrientation = JList.HORIZONTAL_WRAP Then
					' Because HORIZONTAL_WRAP flows differently, the
					' rowsPerColumn needs to be adjusted.
					rowsPerColumn = (dataModelSize \ columnCount)
					If dataModelSize Mod columnCount > 0 Then rowsPerColumn += 1
				End If
			ElseIf layoutOrientation = JList.VERTICAL_WRAP AndAlso ___height <> 0 Then
				rowsPerColumn = Math.Max(1, (listHeight - insets.top - insets.bottom) / ___height)
				columnCount = Math.Max(1, dataModelSize \ rowsPerColumn)
				If dataModelSize > 0 AndAlso dataModelSize > rowsPerColumn AndAlso dataModelSize Mod rowsPerColumn <> 0 Then columnCount += 1
			ElseIf layoutOrientation = JList.HORIZONTAL_WRAP AndAlso cellWidth > 0 AndAlso listWidth > 0 Then
				columnCount = Math.Max(1, (listWidth - insets.left - insets.right) / cellWidth)
				rowsPerColumn = dataModelSize \ columnCount
				If dataModelSize Mod columnCount > 0 Then rowsPerColumn += 1
			End If
			preferredHeight = rowsPerColumn * cellHeight + insets.top + insets.bottom
		End Sub

		Private Property handler As Handler
			Get
				If handler Is Nothing Then handler = New Handler(Me)
				Return handler
			End Get
		End Property

		''' <summary>
		''' Mouse input, and focus handling for JList.  An instance of this
		''' class is added to the appropriate java.awt.Component lists
		''' at installUI() time.  Note keyboard input is handled with JComponent
		''' KeyboardActions, see installKeyboardActions().
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
		''' <seealso cref= #createMouseInputListener </seealso>
		''' <seealso cref= #installKeyboardActions </seealso>
		''' <seealso cref= #installUI </seealso>
		Public Class MouseInputHandler
			Implements MouseInputListener

			Private ReadOnly outerInstance As BasicListUI

			Public Sub New(ByVal outerInstance As BasicListUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub mouseClicked(ByVal e As MouseEvent)
				outerInstance.handler.mouseClicked(e)
			End Sub

			Public Overridable Sub mouseEntered(ByVal e As MouseEvent)
				outerInstance.handler.mouseEntered(e)
			End Sub

			Public Overridable Sub mouseExited(ByVal e As MouseEvent)
				outerInstance.handler.mouseExited(e)
			End Sub

			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				outerInstance.handler.mousePressed(e)
			End Sub

			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
				outerInstance.handler.mouseDragged(e)
			End Sub

			Public Overridable Sub mouseMoved(ByVal e As MouseEvent)
				outerInstance.handler.mouseMoved(e)
			End Sub

			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
				outerInstance.handler.mouseReleased(e)
			End Sub
		End Class


		''' <summary>
		''' Creates a delegate that implements MouseInputListener.
		''' The delegate is added to the corresponding java.awt.Component listener
		''' lists at installUI() time. Subclasses can override this method to return
		''' a custom MouseInputListener, e.g.
		''' <pre>
		''' class MyListUI extends BasicListUI {
		'''    protected MouseInputListener <b>createMouseInputListener</b>() {
		'''        return new MyMouseInputHandler();
		'''    }
		'''    public class MyMouseInputHandler extends MouseInputHandler {
		'''        public void mouseMoved(MouseEvent e) {
		'''            // do some extra work when the mouse moves
		'''            super.mouseMoved(e);
		'''        }
		'''    }
		''' }
		''' </pre>
		''' </summary>
		''' <seealso cref= MouseInputHandler </seealso>
		''' <seealso cref= #installUI </seealso>
		Protected Friend Overridable Function createMouseInputListener() As MouseInputListener
			Return handler
		End Function

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of {@code BasicListUI}.
		''' </summary>
		Public Class FocusHandler
			Implements FocusListener

			Private ReadOnly outerInstance As BasicListUI

			Public Sub New(ByVal outerInstance As BasicListUI)
				Me.outerInstance = outerInstance
			End Sub

			Protected Friend Overridable Sub repaintCellFocus()
				outerInstance.handler.repaintCellFocus()
			End Sub

	'         The focusGained() focusLost() methods run when the JList
	'         * focus changes.
	'         

			Public Overridable Sub focusGained(ByVal e As FocusEvent)
				outerInstance.handler.focusGained(e)
			End Sub

			Public Overridable Sub focusLost(ByVal e As FocusEvent)
				outerInstance.handler.focusLost(e)
			End Sub
		End Class

		Protected Friend Overridable Function createFocusListener() As FocusListener
			Return handler
		End Function

		''' <summary>
		''' The ListSelectionListener that's added to the JLists selection
		''' model at installUI time, and whenever the JList.selectionModel property
		''' changes.  When the selection changes we repaint the affected rows.
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
		''' <seealso cref= #createListSelectionListener </seealso>
		''' <seealso cref= #getCellBounds </seealso>
		''' <seealso cref= #installUI </seealso>
		Public Class ListSelectionHandler
			Implements ListSelectionListener

			Private ReadOnly outerInstance As BasicListUI

			Public Sub New(ByVal outerInstance As BasicListUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub valueChanged(ByVal e As ListSelectionEvent) Implements ListSelectionListener.valueChanged
				outerInstance.handler.valueChanged(e)
			End Sub
		End Class


		''' <summary>
		''' Creates an instance of ListSelectionHandler that's added to
		''' the JLists by selectionModel as needed.  Subclasses can override
		''' this method to return a custom ListSelectionListener, e.g.
		''' <pre>
		''' class MyListUI extends BasicListUI {
		'''    protected ListSelectionListener <b>createListSelectionListener</b>() {
		'''        return new MySelectionListener();
		'''    }
		'''    public class MySelectionListener extends ListSelectionHandler {
		'''        public void valueChanged(ListSelectionEvent e) {
		'''            // do some extra work when the selection changes
		'''            super.valueChange(e);
		'''        }
		'''    }
		''' }
		''' </pre>
		''' </summary>
		''' <seealso cref= ListSelectionHandler </seealso>
		''' <seealso cref= #installUI </seealso>
		Protected Friend Overridable Function createListSelectionListener() As ListSelectionListener
			Return handler
		End Function


		Private Sub redrawList()
			list.revalidate()
			list.repaint()
		End Sub


		''' <summary>
		''' The ListDataListener that's added to the JLists model at
		''' installUI time, and whenever the JList.model property changes.
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
		''' <seealso cref= JList#getModel </seealso>
		''' <seealso cref= #maybeUpdateLayoutState </seealso>
		''' <seealso cref= #createListDataListener </seealso>
		''' <seealso cref= #installUI </seealso>
		Public Class ListDataHandler
			Implements ListDataListener

			Private ReadOnly outerInstance As BasicListUI

			Public Sub New(ByVal outerInstance As BasicListUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub intervalAdded(ByVal e As ListDataEvent) Implements ListDataListener.intervalAdded
				outerInstance.handler.intervalAdded(e)
			End Sub


			Public Overridable Sub intervalRemoved(ByVal e As ListDataEvent) Implements ListDataListener.intervalRemoved
				outerInstance.handler.intervalRemoved(e)
			End Sub


			Public Overridable Sub contentsChanged(ByVal e As ListDataEvent) Implements ListDataListener.contentsChanged
				outerInstance.handler.contentsChanged(e)
			End Sub
		End Class


		''' <summary>
		''' Creates an instance of ListDataListener that's added to
		''' the JLists by model as needed.  Subclasses can override
		''' this method to return a custom ListDataListener, e.g.
		''' <pre>
		''' class MyListUI extends BasicListUI {
		'''    protected ListDataListener <b>createListDataListener</b>() {
		'''        return new MyListDataListener();
		'''    }
		'''    public class MyListDataListener extends ListDataHandler {
		'''        public void contentsChanged(ListDataEvent e) {
		'''            // do some extra work when the models contents change
		'''            super.contentsChange(e);
		'''        }
		'''    }
		''' }
		''' </pre>
		''' </summary>
		''' <seealso cref= ListDataListener </seealso>
		''' <seealso cref= JList#getModel </seealso>
		''' <seealso cref= #installUI </seealso>
		Protected Friend Overridable Function createListDataListener() As ListDataListener
			Return handler
		End Function


		''' <summary>
		''' The PropertyChangeListener that's added to the JList at
		''' installUI time.  When the value of a JList property that
		''' affects layout changes, we set a bit in updateLayoutStateNeeded.
		''' If the JLists model changes we additionally remove our listeners
		''' from the old model.  Likewise for the JList selectionModel.
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
		''' <seealso cref= #maybeUpdateLayoutState </seealso>
		''' <seealso cref= #createPropertyChangeListener </seealso>
		''' <seealso cref= #installUI </seealso>
		Public Class PropertyChangeHandler
			Implements java.beans.PropertyChangeListener

			Private ReadOnly outerInstance As BasicListUI

			Public Sub New(ByVal outerInstance As BasicListUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
				outerInstance.handler.propertyChange(e)
			End Sub
		End Class


		''' <summary>
		''' Creates an instance of PropertyChangeHandler that's added to
		''' the JList by installUI().  Subclasses can override this method
		''' to return a custom PropertyChangeListener, e.g.
		''' <pre>
		''' class MyListUI extends BasicListUI {
		'''    protected PropertyChangeListener <b>createPropertyChangeListener</b>() {
		'''        return new MyPropertyChangeListener();
		'''    }
		'''    public class MyPropertyChangeListener extends PropertyChangeHandler {
		'''        public void propertyChange(PropertyChangeEvent e) {
		'''            if (e.getPropertyName().equals("model")) {
		'''                // do some extra work when the model changes
		'''            }
		'''            super.propertyChange(e);
		'''        }
		'''    }
		''' }
		''' </pre>
		''' </summary>
		''' <seealso cref= PropertyChangeListener </seealso>
		''' <seealso cref= #installUI </seealso>
		Protected Friend Overridable Function createPropertyChangeListener() As java.beans.PropertyChangeListener
			Return handler
		End Function

		''' <summary>
		''' Used by IncrementLeadSelectionAction. Indicates the action should
		''' change the lead, and not select it. 
		''' </summary>
		Private Const CHANGE_LEAD As Integer = 0
		''' <summary>
		''' Used by IncrementLeadSelectionAction. Indicates the action should
		''' change the selection and lead. 
		''' </summary>
		Private Const CHANGE_SELECTION As Integer = 1
		''' <summary>
		''' Used by IncrementLeadSelectionAction. Indicates the action should
		''' extend the selection from the anchor to the next index. 
		''' </summary>
		Private Const EXTEND_SELECTION As Integer = 2


		Private Class Actions
			Inherits sun.swing.UIAction

			Private Const SELECT_PREVIOUS_COLUMN As String = "selectPreviousColumn"
			Private Const SELECT_PREVIOUS_COLUMN_EXTEND As String = "selectPreviousColumnExtendSelection"
			Private Const SELECT_PREVIOUS_COLUMN_CHANGE_LEAD As String = "selectPreviousColumnChangeLead"
			Private Const SELECT_NEXT_COLUMN As String = "selectNextColumn"
			Private Const SELECT_NEXT_COLUMN_EXTEND As String = "selectNextColumnExtendSelection"
			Private Const SELECT_NEXT_COLUMN_CHANGE_LEAD As String = "selectNextColumnChangeLead"
			Private Const SELECT_PREVIOUS_ROW As String = "selectPreviousRow"
			Private Const SELECT_PREVIOUS_ROW_EXTEND As String = "selectPreviousRowExtendSelection"
			Private Const SELECT_PREVIOUS_ROW_CHANGE_LEAD As String = "selectPreviousRowChangeLead"
			Private Const SELECT_NEXT_ROW As String = "selectNextRow"
			Private Const SELECT_NEXT_ROW_EXTEND As String = "selectNextRowExtendSelection"
			Private Const SELECT_NEXT_ROW_CHANGE_LEAD As String = "selectNextRowChangeLead"
			Private Const SELECT_FIRST_ROW As String = "selectFirstRow"
			Private Const SELECT_FIRST_ROW_EXTEND As String = "selectFirstRowExtendSelection"
			Private Const SELECT_FIRST_ROW_CHANGE_LEAD As String = "selectFirstRowChangeLead"
			Private Const SELECT_LAST_ROW As String = "selectLastRow"
			Private Const SELECT_LAST_ROW_EXTEND As String = "selectLastRowExtendSelection"
			Private Const SELECT_LAST_ROW_CHANGE_LEAD As String = "selectLastRowChangeLead"
			Private Const SCROLL_UP As String = "scrollUp"
			Private Const SCROLL_UP_EXTEND As String = "scrollUpExtendSelection"
			Private Const SCROLL_UP_CHANGE_LEAD As String = "scrollUpChangeLead"
			Private Const SCROLL_DOWN As String = "scrollDown"
			Private Const SCROLL_DOWN_EXTEND As String = "scrollDownExtendSelection"
			Private Const SCROLL_DOWN_CHANGE_LEAD As String = "scrollDownChangeLead"
			Private Const SELECT_ALL As String = "selectAll"
			Private Const CLEAR_SELECTION As String = "clearSelection"

			' add the lead item to the selection without changing lead or anchor
			Private Const ADD_TO_SELECTION As String = "addToSelection"

			' toggle the selected state of the lead item and move the anchor to it
			Private Const TOGGLE_AND_ANCHOR As String = "toggleAndAnchor"

			' extend the selection to the lead item
			Private Const EXTEND_TO As String = "extendTo"

			' move the anchor to the lead and ensure only that item is selected
			Private Const MOVE_SELECTION_TO As String = "moveSelectionTo"

			Friend Sub New(ByVal name As String)
				MyBase.New(name)
			End Sub
			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				Dim name As String = name
				Dim list As JList = CType(e.source, JList)
				Dim ui As BasicListUI = CType(BasicLookAndFeel.getUIOfType(list.uI, GetType(BasicListUI)), BasicListUI)

				If name = SELECT_PREVIOUS_COLUMN Then
					changeSelection(list, CHANGE_SELECTION, getNextColumnIndex(list, ui, -1), -1)
				ElseIf name = SELECT_PREVIOUS_COLUMN_EXTEND Then
					changeSelection(list, EXTEND_SELECTION, getNextColumnIndex(list, ui, -1), -1)
				ElseIf name = SELECT_PREVIOUS_COLUMN_CHANGE_LEAD Then
					changeSelection(list, CHANGE_LEAD, getNextColumnIndex(list, ui, -1), -1)
				ElseIf name = SELECT_NEXT_COLUMN Then
					changeSelection(list, CHANGE_SELECTION, getNextColumnIndex(list, ui, 1), 1)
				ElseIf name = SELECT_NEXT_COLUMN_EXTEND Then
					changeSelection(list, EXTEND_SELECTION, getNextColumnIndex(list, ui, 1), 1)
				ElseIf name = SELECT_NEXT_COLUMN_CHANGE_LEAD Then
					changeSelection(list, CHANGE_LEAD, getNextColumnIndex(list, ui, 1), 1)
				ElseIf name = SELECT_PREVIOUS_ROW Then
					changeSelection(list, CHANGE_SELECTION, getNextIndex(list, ui, -1), -1)
				ElseIf name = SELECT_PREVIOUS_ROW_EXTEND Then
					changeSelection(list, EXTEND_SELECTION, getNextIndex(list, ui, -1), -1)
				ElseIf name = SELECT_PREVIOUS_ROW_CHANGE_LEAD Then
					changeSelection(list, CHANGE_LEAD, getNextIndex(list, ui, -1), -1)
				ElseIf name = SELECT_NEXT_ROW Then
					changeSelection(list, CHANGE_SELECTION, getNextIndex(list, ui, 1), 1)
				ElseIf name = SELECT_NEXT_ROW_EXTEND Then
					changeSelection(list, EXTEND_SELECTION, getNextIndex(list, ui, 1), 1)
				ElseIf name = SELECT_NEXT_ROW_CHANGE_LEAD Then
					changeSelection(list, CHANGE_LEAD, getNextIndex(list, ui, 1), 1)
				ElseIf name = SELECT_FIRST_ROW Then
					changeSelection(list, CHANGE_SELECTION, 0, -1)
				ElseIf name = SELECT_FIRST_ROW_EXTEND Then
					changeSelection(list, EXTEND_SELECTION, 0, -1)
				ElseIf name = SELECT_FIRST_ROW_CHANGE_LEAD Then
					changeSelection(list, CHANGE_LEAD, 0, -1)
				ElseIf name = SELECT_LAST_ROW Then
					changeSelection(list, CHANGE_SELECTION, list.model.size - 1, 1)
				ElseIf name = SELECT_LAST_ROW_EXTEND Then
					changeSelection(list, EXTEND_SELECTION, list.model.size - 1, 1)
				ElseIf name = SELECT_LAST_ROW_CHANGE_LEAD Then
					changeSelection(list, CHANGE_LEAD, list.model.size - 1, 1)
				ElseIf name = SCROLL_UP Then
					changeSelection(list, CHANGE_SELECTION, getNextPageIndex(list, -1), -1)
				ElseIf name = SCROLL_UP_EXTEND Then
					changeSelection(list, EXTEND_SELECTION, getNextPageIndex(list, -1), -1)
				ElseIf name = SCROLL_UP_CHANGE_LEAD Then
					changeSelection(list, CHANGE_LEAD, getNextPageIndex(list, -1), -1)
				ElseIf name = SCROLL_DOWN Then
					changeSelection(list, CHANGE_SELECTION, getNextPageIndex(list, 1), 1)
				ElseIf name = SCROLL_DOWN_EXTEND Then
					changeSelection(list, EXTEND_SELECTION, getNextPageIndex(list, 1), 1)
				ElseIf name = SCROLL_DOWN_CHANGE_LEAD Then
					changeSelection(list, CHANGE_LEAD, getNextPageIndex(list, 1), 1)
				ElseIf name = SELECT_ALL Then
					selectAll(list)
				ElseIf name = CLEAR_SELECTION Then
					clearSelection(list)
				ElseIf name = ADD_TO_SELECTION Then
					Dim index As Integer = adjustIndex(list.selectionModel.leadSelectionIndex, list)

					If Not list.isSelectedIndex(index) Then
						Dim oldAnchor As Integer = list.selectionModel.anchorSelectionIndex
						list.valueIsAdjusting = True
						list.addSelectionInterval(index, index)
						list.selectionModel.anchorSelectionIndex = oldAnchor
						list.valueIsAdjusting = False
					End If
				ElseIf name = TOGGLE_AND_ANCHOR Then
					Dim index As Integer = adjustIndex(list.selectionModel.leadSelectionIndex, list)

					If list.isSelectedIndex(index) Then
						list.removeSelectionInterval(index, index)
					Else
						list.addSelectionInterval(index, index)
					End If
				ElseIf name = EXTEND_TO Then
					changeSelection(list, EXTEND_SELECTION, adjustIndex(list.selectionModel.leadSelectionIndex, list), 0)
				ElseIf name = MOVE_SELECTION_TO Then
					changeSelection(list, CHANGE_SELECTION, adjustIndex(list.selectionModel.leadSelectionIndex, list), 0)
				End If
			End Sub

			Public Overridable Function isEnabled(ByVal c As Object) As Boolean
				Dim name As Object = name
				If name Is SELECT_PREVIOUS_COLUMN_CHANGE_LEAD OrElse name Is SELECT_NEXT_COLUMN_CHANGE_LEAD OrElse name Is SELECT_PREVIOUS_ROW_CHANGE_LEAD OrElse name Is SELECT_NEXT_ROW_CHANGE_LEAD OrElse name Is SELECT_FIRST_ROW_CHANGE_LEAD OrElse name Is SELECT_LAST_ROW_CHANGE_LEAD OrElse name Is SCROLL_UP_CHANGE_LEAD OrElse name Is SCROLL_DOWN_CHANGE_LEAD Then Return c IsNot Nothing AndAlso TypeOf CType(c, JList).selectionModel Is DefaultListSelectionModel

				Return True
			End Function

			Private Sub clearSelection(ByVal list As JList)
				list.clearSelection()
			End Sub

			Private Sub selectAll(ByVal list As JList)
				Dim size As Integer = list.model.size
				If size > 0 Then
					Dim lsm As ListSelectionModel = list.selectionModel
					Dim lead As Integer = adjustIndex(lsm.leadSelectionIndex, list)

					If lsm.selectionMode = ListSelectionModel.SINGLE_SELECTION Then
						If lead = -1 Then
							Dim min As Integer = adjustIndex(list.minSelectionIndex, list)
							lead = (If(min = -1, 0, min))
						End If

						list.selectionIntervalval(lead, lead)
						list.ensureIndexIsVisible(lead)
					Else
						list.valueIsAdjusting = True

						Dim anchor As Integer = adjustIndex(lsm.anchorSelectionIndex, list)

						list.selectionIntervalval(0, size - 1)

						' this is done to restore the anchor and lead
						sun.swing.SwingUtilities2.leadAnchorWithoutSelectionion(lsm, anchor, lead)

						list.valueIsAdjusting = False
					End If
				End If
			End Sub

			Private Function getNextPageIndex(ByVal list As JList, ByVal direction As Integer) As Integer
				If list.model.size = 0 Then Return -1

				Dim index As Integer = -1
				Dim visRect As Rectangle = list.visibleRect
				Dim lsm As ListSelectionModel = list.selectionModel
				Dim lead As Integer = adjustIndex(lsm.leadSelectionIndex, list)
				Dim leadRect As Rectangle = If(lead=-1, New Rectangle, list.getCellBounds(lead, lead))

				If list.layoutOrientation = JList.VERTICAL_WRAP AndAlso list.visibleRowCount <= 0 Then
					If Not list.componentOrientation.leftToRight Then direction = -direction
					' apply for horizontal scrolling: the step for next
					' page index is number of visible columns
					If direction < 0 Then
						' left
						visRect.x = leadRect.x + leadRect.width - visRect.width
						Dim p As New Point(visRect.x - 1, leadRect.y)
						index = list.locationToIndex(p)
						Dim cellBounds As Rectangle = list.getCellBounds(index, index)
						If visRect.intersects(cellBounds) Then
							p.x = cellBounds.x - 1
							index = list.locationToIndex(p)
							cellBounds = list.getCellBounds(index, index)
						End If
						' this is necessary for right-to-left orientation only
						If cellBounds.y <> leadRect.y Then
							p.x = cellBounds.x + cellBounds.width
							index = list.locationToIndex(p)
						End If
					Else
						' right
						visRect.x = leadRect.x
						Dim p As New Point(visRect.x + visRect.width, leadRect.y)
						index = list.locationToIndex(p)
						Dim cellBounds As Rectangle = list.getCellBounds(index, index)
						If visRect.intersects(cellBounds) Then
							p.x = cellBounds.x + cellBounds.width
							index = list.locationToIndex(p)
							cellBounds = list.getCellBounds(index, index)
						End If
						If cellBounds.y <> leadRect.y Then
							p.x = cellBounds.x - 1
							index = list.locationToIndex(p)
						End If
					End If
				Else
					If direction < 0 Then
						' up
						' go to the first visible cell
						Dim p As New Point(leadRect.x, visRect.y)
						index = list.locationToIndex(p)
						If lead <= index Then
							' if lead is the first visible cell (or above it)
							' adjust the visible rect up
							visRect.y = leadRect.y + leadRect.height - visRect.height
							p.y = visRect.y
							index = list.locationToIndex(p)
							Dim cellBounds As Rectangle = list.getCellBounds(index, index)
							' go one cell down if first visible cell doesn't fit
							' into adjasted visible rectangle
							If cellBounds.y < visRect.y Then
								p.y = cellBounds.y + cellBounds.height
								index = list.locationToIndex(p)
								cellBounds = list.getCellBounds(index, index)
							End If
							' if index isn't less then lead
							' try to go to cell previous to lead
							If cellBounds.y >= leadRect.y Then
								p.y = leadRect.y - 1
								index = list.locationToIndex(p)
							End If
						End If
					Else
						' down
						' go to the last completely visible cell
						Dim p As New Point(leadRect.x, visRect.y + visRect.height - 1)
						index = list.locationToIndex(p)
						Dim cellBounds As Rectangle = list.getCellBounds(index, index)
						' go up one cell if last visible cell doesn't fit
						' into visible rectangle
						If cellBounds.y + cellBounds.height > visRect.y + visRect.height Then
							p.y = cellBounds.y - 1
							index = list.locationToIndex(p)
							cellBounds = list.getCellBounds(index, index)
							index = Math.Max(index, lead)
						End If

						If lead >= index Then
							' if lead is the last completely visible index
							' (or below it) adjust the visible rect down
							visRect.y = leadRect.y
							p.y = visRect.y + visRect.height - 1
							index = list.locationToIndex(p)
							cellBounds = list.getCellBounds(index, index)
							' go one cell up if last visible cell doesn't fit
							' into adjasted visible rectangle
							If cellBounds.y + cellBounds.height > visRect.y + visRect.height Then
								p.y = cellBounds.y - 1
								index = list.locationToIndex(p)
								cellBounds = list.getCellBounds(index, index)
							End If
							' if index isn't greater then lead
							' try to go to cell next after lead
							If cellBounds.y <= leadRect.y Then
								p.y = leadRect.y + leadRect.height
								index = list.locationToIndex(p)
							End If
						End If
					End If
				End If
				Return index
			End Function

			Private Sub changeSelection(ByVal list As JList, ByVal type As Integer, ByVal index As Integer, ByVal direction As Integer)
				If index >= 0 AndAlso index < list.model.size Then
					Dim lsm As ListSelectionModel = list.selectionModel

					' CHANGE_LEAD is only valid with multiple interval selection
					If type = CHANGE_LEAD AndAlso list.selectionMode <> ListSelectionModel.MULTIPLE_INTERVAL_SELECTION Then type = CHANGE_SELECTION

					' IMPORTANT - This needs to happen before the index is changed.
					' This is because JFileChooser, which uses JList, also scrolls
					' the selected item into view. If that happens first, then
					' this method becomes a no-op.
					adjustScrollPositionIfNecessary(list, index, direction)

					If type = EXTEND_SELECTION Then
						Dim anchor As Integer = adjustIndex(lsm.anchorSelectionIndex, list)
						If anchor = -1 Then anchor = 0

						list.selectionIntervalval(anchor, index)
					ElseIf type = CHANGE_SELECTION Then
						list.selectedIndex = index
					Else
						' casting should be safe since the action is only enabled
						' for DefaultListSelectionModel
						CType(lsm, DefaultListSelectionModel).moveLeadSelectionIndex(index)
					End If
				End If
			End Sub

			''' <summary>
			''' When scroll down makes selected index the last completely visible
			''' index. When scroll up makes selected index the first visible index.
			''' Adjust visible rectangle respect to list's component orientation.
			''' </summary>
			Private Sub adjustScrollPositionIfNecessary(ByVal list As JList, ByVal index As Integer, ByVal direction As Integer)
				If direction = 0 Then Return
				Dim cellBounds As Rectangle = list.getCellBounds(index, index)
				Dim visRect As Rectangle = list.visibleRect
				If cellBounds IsNot Nothing AndAlso (Not visRect.contains(cellBounds)) Then
					If list.layoutOrientation = JList.VERTICAL_WRAP AndAlso list.visibleRowCount <= 0 Then
						' horizontal
						If list.componentOrientation.leftToRight Then
							If direction > 0 Then
								' right for left-to-right
								Dim x As Integer =Math.Max(0, cellBounds.x + cellBounds.width - visRect.width)
								Dim startIndex As Integer = list.locationToIndex(New Point(x, cellBounds.y))
								Dim startRect As Rectangle = list.getCellBounds(startIndex, startIndex)
								If startRect.x < x AndAlso startRect.x < cellBounds.x Then
									startRect.x += startRect.width
									startIndex = list.locationToIndex(startRect.location)
									startRect = list.getCellBounds(startIndex, startIndex)
								End If
								cellBounds = startRect
							End If
							cellBounds.width = visRect.width
						Else
							If direction > 0 Then
								' left for right-to-left
								Dim x As Integer = cellBounds.x + visRect.width
								Dim rightIndex As Integer = list.locationToIndex(New Point(x, cellBounds.y))
								Dim rightRect As Rectangle = list.getCellBounds(rightIndex, rightIndex)
								If rightRect.x + rightRect.width > x AndAlso rightRect.x > cellBounds.x Then rightRect.width = 0
								cellBounds.x = Math.Max(0, rightRect.x + rightRect.width - visRect.width)
								cellBounds.width = visRect.width
							Else
								cellBounds.x += Math.Max(0, cellBounds.width - visRect.width)
								' adjust width to fit into visible rectangle
								cellBounds.width = Math.Min(cellBounds.width, visRect.width)
							End If
						End If
					Else
						' vertical
						If direction > 0 AndAlso (cellBounds.y < visRect.y OrElse cellBounds.y + cellBounds.height > visRect.y + visRect.height) Then
							'down
							Dim y As Integer = Math.Max(0, cellBounds.y + cellBounds.height - visRect.height)
							Dim startIndex As Integer = list.locationToIndex(New Point(cellBounds.x, y))
							Dim startRect As Rectangle = list.getCellBounds(startIndex, startIndex)
							If startRect.y < y AndAlso startRect.y < cellBounds.y Then
								startRect.y += startRect.height
								startIndex = list.locationToIndex(startRect.location)
								startRect = list.getCellBounds(startIndex, startIndex)
							End If
							cellBounds = startRect
							cellBounds.height = visRect.height
						Else
							' adjust height to fit into visible rectangle
							cellBounds.height = Math.Min(cellBounds.height, visRect.height)
						End If
					End If
					list.scrollRectToVisible(cellBounds)
				End If
			End Sub

			Private Function getNextColumnIndex(ByVal list As JList, ByVal ui As BasicListUI, ByVal amount As Integer) As Integer
				If list.layoutOrientation <> JList.VERTICAL Then
					Dim index As Integer = adjustIndex(list.leadSelectionIndex, list)
					Dim size As Integer = list.model.size

					If index = -1 Then
						Return 0
					ElseIf size = 1 Then
						' there's only one item so we should select it
						Return 0
					ElseIf ui Is Nothing OrElse ui.columnCount <= 1 Then
						Return -1
					End If

					Dim column As Integer = ui.convertModelToColumn(index)
					Dim row As Integer = ui.convertModelToRow(index)

					column += amount
					If column >= ui.columnCount OrElse column < 0 Then Return -1
					Dim maxRowCount As Integer = ui.getRowCount(column)
					If row >= maxRowCount Then Return -1
					Return ui.getModelIndex(column, row)
				End If
				' Won't change the selection.
				Return -1
			End Function

			Private Function getNextIndex(ByVal list As JList, ByVal ui As BasicListUI, ByVal amount As Integer) As Integer
				Dim index As Integer = adjustIndex(list.leadSelectionIndex, list)
				Dim size As Integer = list.model.size

				If index = -1 Then
					If size > 0 Then
						If amount > 0 Then
							index = 0
						Else
							index = size - 1
						End If
					End If
				ElseIf size = 1 Then
					' there's only one item so we should select it
					index = 0
				ElseIf list.layoutOrientation = JList.HORIZONTAL_WRAP Then
					If ui IsNot Nothing Then index += ui.columnCount * amount
				Else
					index += amount
				End If

				Return index
			End Function
		End Class


		Private Class Handler
			Implements FocusListener, KeyListener, ListDataListener, ListSelectionListener, MouseInputListener, java.beans.PropertyChangeListener, javax.swing.plaf.basic.DragRecognitionSupport.BeforeDrag

			Private ReadOnly outerInstance As BasicListUI

			Public Sub New(ByVal outerInstance As BasicListUI)
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
				Dim src As JList = CType(e.source, JList)
				Dim model As ListModel = src.model

				If model.size = 0 OrElse e.altDown OrElse BasicGraphicsUtils.isMenuShortcutKeyDown(e) OrElse isNavigationKey(e) Then Return
				Dim startingFromSelection As Boolean = True

				Dim c As Char = e.keyChar

				Dim time As Long = e.when
				Dim startIndex As Integer = adjustIndex(src.leadSelectionIndex, outerInstance.list)
				If time - lastTime < outerInstance.timeFactor Then
					typedString += c
					If (prefix.Length = 1) AndAlso (c = prefix.Chars(0)) Then
						' Subsequent same key presses move the keyboard focus to the next
						' object that starts with the same letter.
						startIndex += 1
					Else
						prefix = typedString
					End If
				Else
					startIndex += 1
					typedString = "" & AscW(c)
					prefix = typedString
				End If
				lastTime = time

				If startIndex < 0 OrElse startIndex >= model.size Then
					startingFromSelection = False
					startIndex = 0
				End If
				Dim index As Integer = src.getNextMatch(prefix, startIndex, javax.swing.text.Position.Bias.Forward)
				If index >= 0 Then
					src.selectedIndex = index
					src.ensureIndexIsVisible(index) ' wrap
				ElseIf startingFromSelection Then
					index = src.getNextMatch(prefix, 0, javax.swing.text.Position.Bias.Forward)
					If index >= 0 Then
						src.selectedIndex = index
						src.ensureIndexIsVisible(index)
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
				If isNavigationKey(e) Then
					prefix = ""
					typedString = ""
					lastTime = 0L
				End If
			End Sub

			''' <summary>
			''' Invoked when a key has been released.
			''' See the class description for <seealso cref="KeyEvent"/> for a definition of
			''' a key released event.
			''' </summary>
			Public Overridable Sub keyReleased(ByVal e As KeyEvent)
			End Sub

			''' <summary>
			''' Returns whether or not the supplied key event maps to a key that is used for
			''' navigation.  This is used for optimizing key input by only passing non-
			''' navigation keys to the first letter navigation mechanism.
			''' </summary>
			Private Function isNavigationKey(ByVal [event] As KeyEvent) As Boolean
				Dim inputMap As InputMap = outerInstance.list.getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT)
				Dim key As KeyStroke = KeyStroke.getKeyStrokeForEvent([event])

				If inputMap IsNot Nothing AndAlso inputMap.get(key) IsNot Nothing Then Return True
				Return False
			End Function

			'
			' PropertyChangeListener
			'
			Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
				Dim propertyName As String = e.propertyName

	'             If the JList.model property changes, remove our listener,
	'             * listDataListener from the old model and add it to the new one.
	'             
				If propertyName = "model" Then
					Dim oldModel As ListModel = CType(e.oldValue, ListModel)
					Dim newModel As ListModel = CType(e.newValue, ListModel)
					If oldModel IsNot Nothing Then oldModel.removeListDataListener(outerInstance.listDataListener)
					If newModel IsNot Nothing Then newModel.addListDataListener(outerInstance.listDataListener)
					outerInstance.updateLayoutStateNeeded = outerInstance.updateLayoutStateNeeded Or modelChanged
					outerInstance.redrawList()

	'             If the JList.selectionModel property changes, remove our listener,
	'             * listSelectionListener from the old selectionModel and add it to the new one.
	'             
				ElseIf propertyName = "selectionModel" Then
					Dim oldModel As ListSelectionModel = CType(e.oldValue, ListSelectionModel)
					Dim newModel As ListSelectionModel = CType(e.newValue, ListSelectionModel)
					If oldModel IsNot Nothing Then oldModel.removeListSelectionListener(outerInstance.listSelectionListener)
					If newModel IsNot Nothing Then newModel.addListSelectionListener(outerInstance.listSelectionListener)
					outerInstance.updateLayoutStateNeeded = outerInstance.updateLayoutStateNeeded Or modelChanged
					outerInstance.redrawList()
				ElseIf propertyName = "cellRenderer" Then
					outerInstance.updateLayoutStateNeeded = outerInstance.updateLayoutStateNeeded Or cellRendererChanged
					outerInstance.redrawList()
				ElseIf propertyName = "font" Then
					outerInstance.updateLayoutStateNeeded = outerInstance.updateLayoutStateNeeded Or fontChanged
					outerInstance.redrawList()
				ElseIf propertyName = "prototypeCellValue" Then
					outerInstance.updateLayoutStateNeeded = outerInstance.updateLayoutStateNeeded Or prototypeCellValueChanged
					outerInstance.redrawList()
				ElseIf propertyName = "fixedCellHeight" Then
					outerInstance.updateLayoutStateNeeded = outerInstance.updateLayoutStateNeeded Or fixedCellHeightChanged
					outerInstance.redrawList()
				ElseIf propertyName = "fixedCellWidth" Then
					outerInstance.updateLayoutStateNeeded = outerInstance.updateLayoutStateNeeded Or fixedCellWidthChanged
					outerInstance.redrawList()
				ElseIf propertyName = "selectionForeground" Then
					outerInstance.list.repaint()
				ElseIf propertyName = "selectionBackground" Then
					outerInstance.list.repaint()
				ElseIf "layoutOrientation" = propertyName Then
					outerInstance.updateLayoutStateNeeded = outerInstance.updateLayoutStateNeeded Or layoutOrientationChanged
					outerInstance.layoutOrientation = outerInstance.list.layoutOrientation
					outerInstance.redrawList()
				ElseIf "visibleRowCount" = propertyName Then
					If outerInstance.layoutOrientation <> JList.VERTICAL Then
						outerInstance.updateLayoutStateNeeded = outerInstance.updateLayoutStateNeeded Or layoutOrientationChanged
						outerInstance.redrawList()
					End If
				ElseIf "componentOrientation" = propertyName Then
					outerInstance.isLeftToRight = outerInstance.list.componentOrientation.leftToRight
					outerInstance.updateLayoutStateNeeded = outerInstance.updateLayoutStateNeeded Or componentOrientationChanged
					outerInstance.redrawList()

					Dim inputMap As InputMap = outerInstance.getInputMap(JComponent.WHEN_FOCUSED)
					SwingUtilities.replaceUIInputMap(outerInstance.list, JComponent.WHEN_FOCUSED, inputMap)
				ElseIf "List.isFileList" = propertyName Then
					outerInstance.updateIsFileList()
					outerInstance.redrawList()
				ElseIf "dropLocation" = propertyName Then
					Dim oldValue As JList.DropLocation = CType(e.oldValue, JList.DropLocation)
					repaintDropLocation(oldValue)
					repaintDropLocation(outerInstance.list.dropLocation)
				End If
			End Sub

			Private Sub repaintDropLocation(ByVal loc As JList.DropLocation)
				If loc Is Nothing Then Return

				Dim r As Rectangle

				If loc.insert Then
					r = outerInstance.getDropLineRect(loc)
				Else
					r = outerInstance.getCellBounds(outerInstance.list, loc.index)
				End If

				If r IsNot Nothing Then outerInstance.list.repaint(r)
			End Sub

			'
			' ListDataListener
			'
			Public Overridable Sub intervalAdded(ByVal e As ListDataEvent) Implements ListDataListener.intervalAdded
				outerInstance.updateLayoutStateNeeded = modelChanged

				Dim minIndex As Integer = Math.Min(e.index0, e.index1)
				Dim maxIndex As Integer = Math.Max(e.index0, e.index1)

	'             Sync the SelectionModel with the DataModel.
	'             

				Dim sm As ListSelectionModel = outerInstance.list.selectionModel
				If sm IsNot Nothing Then sm.insertIndexInterval(minIndex, maxIndex - minIndex+1, True)

	'             Repaint the entire list, from the origin of
	'             * the first added cell, to the bottom of the
	'             * component.
	'             
				outerInstance.redrawList()
			End Sub


			Public Overridable Sub intervalRemoved(ByVal e As ListDataEvent) Implements ListDataListener.intervalRemoved
				outerInstance.updateLayoutStateNeeded = modelChanged

	'             Sync the SelectionModel with the DataModel.
	'             

				Dim sm As ListSelectionModel = outerInstance.list.selectionModel
				If sm IsNot Nothing Then sm.removeIndexInterval(e.index0, e.index1)

	'             Repaint the entire list, from the origin of
	'             * the first removed cell, to the bottom of the
	'             * component.
	'             

				outerInstance.redrawList()
			End Sub


			Public Overridable Sub contentsChanged(ByVal e As ListDataEvent) Implements ListDataListener.contentsChanged
				outerInstance.updateLayoutStateNeeded = modelChanged
				outerInstance.redrawList()
			End Sub


			'
			' ListSelectionListener
			'
			Public Overridable Sub valueChanged(ByVal e As ListSelectionEvent) Implements ListSelectionListener.valueChanged
				outerInstance.maybeUpdateLayoutState()

				Dim size As Integer = outerInstance.list.model.size
				Dim firstIndex As Integer = Math.Min(size - 1, Math.Max(e.firstIndex, 0))
				Dim lastIndex As Integer = Math.Min(size - 1, Math.Max(e.lastIndex, 0))

				Dim bounds As Rectangle = outerInstance.getCellBounds(outerInstance.list, firstIndex, lastIndex)

				If bounds IsNot Nothing Then outerInstance.list.repaint(bounds.x, bounds.y, bounds.width, bounds.height)
			End Sub

			'
			' MouseListener
			'
			Public Overridable Sub mouseClicked(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseEntered(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseExited(ByVal e As MouseEvent)
			End Sub

			' Whether or not the mouse press (which is being considered as part
			' of a drag sequence) also caused the selection change to be fully
			' processed.
			Private dragPressDidSelection As Boolean

			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				If sun.swing.SwingUtilities2.shouldIgnore(e, outerInstance.list) Then Return

				Dim dragEnabled As Boolean = outerInstance.list.dragEnabled
				Dim grabFocus As Boolean = True

				' different behavior if drag is enabled
				If dragEnabled Then
					Dim row As Integer = sun.swing.SwingUtilities2.loc2IndexFileList(outerInstance.list, e.point)
					' if we have a valid row and this is a drag initiating event
					If row <> -1 AndAlso DragRecognitionSupport.mousePressed(e) Then
						dragPressDidSelection = False

						If BasicGraphicsUtils.isMenuShortcutKeyDown(e) Then
							' do nothing for control - will be handled on release
							' or when drag starts
							Return
						ElseIf (Not e.shiftDown) AndAlso outerInstance.list.isSelectedIndex(row) Then
							' clicking on something that's already selected
							' and need to make it the lead now
							outerInstance.list.addSelectionInterval(row, row)
							Return
						End If

						' could be a drag initiating event - don't grab focus
						grabFocus = False

						dragPressDidSelection = True
					End If
				Else
					' When drag is enabled mouse drags won't change the selection
					' in the list, so we only set the isAdjusting flag when it's
					' not enabled
					outerInstance.list.valueIsAdjusting = True
				End If

				If grabFocus Then sun.swing.SwingUtilities2.adjustFocus(outerInstance.list)

				adjustSelection(e)
			End Sub

			Private Sub adjustSelection(ByVal e As MouseEvent)
				Dim row As Integer = sun.swing.SwingUtilities2.loc2IndexFileList(outerInstance.list, e.point)
				If row < 0 Then
					' If shift is down in multi-select, we should do nothing.
					' For single select or non-shift-click, clear the selection
					If outerInstance.isFileList AndAlso e.iD = MouseEvent.MOUSE_PRESSED AndAlso ((Not e.shiftDown) OrElse outerInstance.list.selectionMode = ListSelectionModel.SINGLE_SELECTION) Then outerInstance.list.clearSelection()
				Else
					Dim anchorIndex As Integer = adjustIndex(outerInstance.list.anchorSelectionIndex, outerInstance.list)
					Dim anchorSelected As Boolean
					If anchorIndex = -1 Then
						anchorIndex = 0
						anchorSelected = False
					Else
						anchorSelected = outerInstance.list.isSelectedIndex(anchorIndex)
					End If

					If BasicGraphicsUtils.isMenuShortcutKeyDown(e) Then
						If e.shiftDown Then
							If anchorSelected Then
								outerInstance.list.addSelectionInterval(anchorIndex, row)
							Else
								outerInstance.list.removeSelectionInterval(anchorIndex, row)
								If outerInstance.isFileList Then
									outerInstance.list.addSelectionInterval(row, row)
									outerInstance.list.selectionModel.anchorSelectionIndex = anchorIndex
								End If
							End If
						ElseIf outerInstance.list.isSelectedIndex(row) Then
							outerInstance.list.removeSelectionInterval(row, row)
						Else
							outerInstance.list.addSelectionInterval(row, row)
						End If
					ElseIf e.shiftDown Then
						outerInstance.list.selectionIntervalval(anchorIndex, row)
					Else
						outerInstance.list.selectionIntervalval(row, row)
					End If
				End If
			End Sub

			Public Overridable Sub dragStarting(ByVal [me] As MouseEvent)
				If BasicGraphicsUtils.isMenuShortcutKeyDown([me]) Then
					Dim row As Integer = sun.swing.SwingUtilities2.loc2IndexFileList(outerInstance.list, [me].point)
					outerInstance.list.addSelectionInterval(row, row)
				End If
			End Sub

			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
				If sun.swing.SwingUtilities2.shouldIgnore(e, outerInstance.list) Then Return

				If outerInstance.list.dragEnabled Then
					DragRecognitionSupport.mouseDragged(e, Me)
					Return
				End If

				If e.shiftDown OrElse BasicGraphicsUtils.isMenuShortcutKeyDown(e) Then Return

				Dim row As Integer = outerInstance.locationToIndex(outerInstance.list, e.point)
				If row <> -1 Then
					' 4835633.  Dragging onto a File should not select it.
					If outerInstance.isFileList Then Return
					Dim cellBounds As Rectangle = outerInstance.getCellBounds(outerInstance.list, row, row)
					If cellBounds IsNot Nothing Then
						outerInstance.list.scrollRectToVisible(cellBounds)
						outerInstance.list.selectionIntervalval(row, row)
					End If
				End If
			End Sub

			Public Overridable Sub mouseMoved(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
				If sun.swing.SwingUtilities2.shouldIgnore(e, outerInstance.list) Then Return

				If outerInstance.list.dragEnabled Then
					Dim [me] As MouseEvent = DragRecognitionSupport.mouseReleased(e)
					If [me] IsNot Nothing Then
						sun.swing.SwingUtilities2.adjustFocus(outerInstance.list)
						If Not dragPressDidSelection Then adjustSelection([me])
					End If
				Else
					outerInstance.list.valueIsAdjusting = False
				End If
			End Sub

			'
			' FocusListener
			'
			Protected Friend Overridable Sub repaintCellFocus()
				Dim leadIndex As Integer = adjustIndex(outerInstance.list.leadSelectionIndex, outerInstance.list)
				If leadIndex <> -1 Then
					Dim r As Rectangle = outerInstance.getCellBounds(outerInstance.list, leadIndex, leadIndex)
					If r IsNot Nothing Then outerInstance.list.repaint(r.x, r.y, r.width, r.height)
				End If
			End Sub

	'         The focusGained() focusLost() methods run when the JList
	'         * focus changes.
	'         

			Public Overridable Sub focusGained(ByVal e As FocusEvent)
				repaintCellFocus()
			End Sub

			Public Overridable Sub focusLost(ByVal e As FocusEvent)
				repaintCellFocus()
			End Sub
		End Class

		Private Shared Function adjustIndex(ByVal index As Integer, ByVal list As JList) As Integer
			Return If(index < list.model.size, index, -1)
		End Function

		Private Shared ReadOnly defaultTransferHandler As TransferHandler = New ListTransferHandler

		Friend Class ListTransferHandler
			Inherits TransferHandler
			Implements UIResource

			''' <summary>
			''' Create a Transferable to use as the source for a data transfer.
			''' </summary>
			''' <param name="c">  The component holding the data to be transfered.  This
			'''  argument is provided to enable sharing of TransferHandlers by
			'''  multiple components. </param>
			''' <returns>  The representation of the data to be transfered.
			'''  </returns>
			Protected Friend Overrides Function createTransferable(ByVal c As JComponent) As java.awt.datatransfer.Transferable
				If TypeOf c Is JList Then
					Dim list As JList = CType(c, JList)
					Dim values As Object() = list.selectedValues

					If values Is Nothing OrElse values.Length = 0 Then Return Nothing

					Dim plainBuf As New StringBuilder
					Dim htmlBuf As New StringBuilder

					htmlBuf.Append("<html>" & vbLf & "<body>" & vbLf & "<ul>" & vbLf)

					For i As Integer = 0 To values.Length - 1
						Dim obj As Object = values(i)
						Dim val As String = (If(obj Is Nothing, "", obj.ToString()))
						plainBuf.Append(val & vbLf)
						htmlBuf.Append("  <li>" & val & vbLf)
					Next i

					' remove the last newline
					plainBuf.Remove(plainBuf.Length - 1, 1)
					htmlBuf.Append("</ul>" & vbLf & "</body>" & vbLf & "</html>")

					Return New BasicTransferable(plainBuf.ToString(), htmlBuf.ToString())
				End If

				Return Nothing
			End Function

			Public Overrides Function getSourceActions(ByVal c As JComponent) As Integer
				Return COPY
			End Function

		End Class
	End Class

End Namespace
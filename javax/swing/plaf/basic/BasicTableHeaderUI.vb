Imports System
Imports System.Collections
Imports javax.swing
Imports javax.swing.event
Imports javax.swing.plaf
Imports javax.swing.table
Imports sun.swing

'
' * Copyright (c) 1997, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' BasicTableHeaderUI implementation
	''' 
	''' @author Alan Chung
	''' @author Philip Milne
	''' </summary>
	Public Class BasicTableHeaderUI
		Inherits TableHeaderUI

		Private Shared resizeCursor As Cursor = Cursor.getPredefinedCursor(Cursor.E_RESIZE_CURSOR)

	'
	' Instance Variables
	'

		''' <summary>
		''' The JTableHeader that is delegating the painting to this UI. </summary>
		Protected Friend header As JTableHeader
		Protected Friend rendererPane As CellRendererPane

		' Listeners that are attached to the JTable
		Protected Friend mouseInputListener As MouseInputListener

		' The column header over which the mouse currently is.
		Private rolloverColumn As Integer = -1

		' The column that should be highlighted when the table header has the focus.
		Private selectedColumnIndex As Integer = 0 ' Read ONLY via getSelectedColumnIndex!

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'		private static FocusListener focusListener = New FocusListener()
	'	{
	'		public void focusGained(FocusEvent e)
	'		{
	'			repaintHeader(e.getSource());
	'		}
	'
	'		public void focusLost(FocusEvent e)
	'		{
	'			repaintHeader(e.getSource());
	'		}
	'
	'		private void repaintHeader(Object source)
	'		{
	'			if (source instanceof JTableHeader)
	'			{
	'				JTableHeader th = (JTableHeader)source;
	'				BasicTableHeaderUI ui = (BasicTableHeaderUI)BasicLookAndFeel.getUIOfType(th.getUI(), BasicTableHeaderUI.class);
	'				if (ui == Nothing)
	'				{
	'					Return;
	'				}
	'
	'				th.repaint(th.getHeaderRect(ui.getSelectedColumnIndex()));
	'			}
	'		}
	'	};

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of {@code BasicTableHeaderUI}.
		''' </summary>
		Public Class MouseInputHandler
			Implements MouseInputListener

			Private ReadOnly outerInstance As BasicTableHeaderUI

			Public Sub New(ByVal outerInstance As BasicTableHeaderUI)
				Me.outerInstance = outerInstance
			End Sub


			Private mouseXOffset As Integer
			Private otherCursor As Cursor = resizeCursor

			Public Overridable Sub mouseClicked(ByVal e As MouseEvent)
				If Not outerInstance.header.enabled Then Return
				If e.clickCount Mod 2 = 1 AndAlso SwingUtilities.isLeftMouseButton(e) Then
					Dim table As JTable = outerInstance.header.table
					Dim sorter As RowSorter
					sorter = table.rowSorter
					If table IsNot Nothing AndAlso sorter IsNot Nothing Then
						Dim columnIndex As Integer = outerInstance.header.columnAtPoint(e.point)
						If columnIndex <> -1 Then
							columnIndex = table.convertColumnIndexToModel(columnIndex)
							sorter.toggleSortOrder(columnIndex)
						End If
					End If
				End If
			End Sub

			Private Function getResizingColumn(ByVal p As Point) As TableColumn
				Return getResizingColumn(p, outerInstance.header.columnAtPoint(p))
			End Function

			Private Function getResizingColumn(ByVal p As Point, ByVal column As Integer) As TableColumn
				If column = -1 Then Return Nothing
				Dim r As Rectangle = outerInstance.header.getHeaderRect(column)
				r.grow(-3, 0)
				If r.contains(p) Then Return Nothing
				Dim midPoint As Integer = r.x + r.width/2
				Dim columnIndex As Integer
				If outerInstance.header.componentOrientation.leftToRight Then
					columnIndex = If(p.x < midPoint, column - 1, column)
				Else
					columnIndex = If(p.x < midPoint, column, column - 1)
				End If
				If columnIndex = -1 Then Return Nothing
				Return outerInstance.header.columnModel.getColumn(columnIndex)
			End Function

			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				If Not outerInstance.header.enabled Then Return
				outerInstance.header.draggedColumn = Nothing
				outerInstance.header.resizingColumn = Nothing
				outerInstance.header.draggedDistance = 0

				Dim p As Point = e.point

				' First find which header cell was hit
				Dim columnModel As TableColumnModel = outerInstance.header.columnModel
				Dim index As Integer = outerInstance.header.columnAtPoint(p)

				If index <> -1 Then
					' The last 3 pixels + 3 pixels of next column are for resizing
					Dim ___resizingColumn As TableColumn = getResizingColumn(p, index)
					If canResize(___resizingColumn, outerInstance.header) Then
						outerInstance.header.resizingColumn = ___resizingColumn
						If outerInstance.header.componentOrientation.leftToRight Then
							mouseXOffset = p.x - ___resizingColumn.width
						Else
							mouseXOffset = p.x + ___resizingColumn.width
						End If
					ElseIf outerInstance.header.reorderingAllowed Then
						Dim hitColumn As TableColumn = columnModel.getColumn(index)
						outerInstance.header.draggedColumn = hitColumn
						mouseXOffset = p.x
					End If
				End If

				If outerInstance.header.reorderingAllowed Then
					Dim oldRolloverColumn As Integer = outerInstance.rolloverColumn
					outerInstance.rolloverColumn = -1
					outerInstance.rolloverColumnUpdated(oldRolloverColumn, outerInstance.rolloverColumn)
				End If
			End Sub

			Private Sub swapCursor()
				Dim tmp As Cursor = outerInstance.header.cursor
				outerInstance.header.cursor = otherCursor
				otherCursor = tmp
			End Sub

			Public Overridable Sub mouseMoved(ByVal e As MouseEvent)
				If Not outerInstance.header.enabled Then Return
				If canResize(getResizingColumn(e.point), outerInstance.header) <> (outerInstance.header.cursor Is resizeCursor) Then swapCursor()
				outerInstance.updateRolloverColumn(e)
			End Sub

			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
				If Not outerInstance.header.enabled Then Return
				Dim mouseX As Integer = e.x

				Dim ___resizingColumn As TableColumn = outerInstance.header.resizingColumn
				Dim draggedColumn As TableColumn = outerInstance.header.draggedColumn

				Dim headerLeftToRight As Boolean = outerInstance.header.componentOrientation.leftToRight

				If ___resizingColumn IsNot Nothing Then
					Dim oldWidth As Integer = ___resizingColumn.width
					Dim newWidth As Integer
					If headerLeftToRight Then
						newWidth = mouseX - mouseXOffset
					Else
						newWidth = mouseXOffset - mouseX
					End If
					mouseXOffset += outerInstance.changeColumnWidth(___resizingColumn, outerInstance.header, oldWidth, newWidth)
				ElseIf draggedColumn IsNot Nothing Then
					Dim cm As TableColumnModel = outerInstance.header.columnModel
					Dim ___draggedDistance As Integer = mouseX - mouseXOffset
					Dim direction As Integer = If(___draggedDistance < 0, -1, 1)
					Dim columnIndex As Integer = outerInstance.viewIndexForColumn(draggedColumn)
					Dim newColumnIndex As Integer = columnIndex + (If(headerLeftToRight, direction, -direction))
					If 0 <= newColumnIndex AndAlso newColumnIndex < cm.columnCount Then
						Dim width As Integer = cm.getColumn(newColumnIndex).width
						If Math.Abs(___draggedDistance) > (width \ 2) Then

							mouseXOffset = mouseXOffset + direction * width
							outerInstance.header.draggedDistance = ___draggedDistance - direction * width

							'Cache the selected column.
							Dim selectedIndex As Integer = SwingUtilities2.convertColumnIndexToModel(outerInstance.header.columnModel, outerInstance.selectedColumnIndex)

							'Now do the move.
							cm.moveColumn(columnIndex, newColumnIndex)

							'Update the selected index.
							outerInstance.selectColumn(SwingUtilities2.convertColumnIndexToView(outerInstance.header.columnModel, selectedIndex), False)

							Return
						End If
					End If
					draggedDistancence(___draggedDistance, columnIndex)
				End If

				outerInstance.updateRolloverColumn(e)
			End Sub

			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
				If Not outerInstance.header.enabled Then Return
				draggedDistancence(0, outerInstance.viewIndexForColumn(outerInstance.header.draggedColumn))

				outerInstance.header.resizingColumn = Nothing
				outerInstance.header.draggedColumn = Nothing

				outerInstance.updateRolloverColumn(e)
			End Sub

			Public Overridable Sub mouseEntered(ByVal e As MouseEvent)
				If Not outerInstance.header.enabled Then Return
				outerInstance.updateRolloverColumn(e)
			End Sub

			Public Overridable Sub mouseExited(ByVal e As MouseEvent)
				If Not outerInstance.header.enabled Then Return
				Dim oldRolloverColumn As Integer = outerInstance.rolloverColumn
				outerInstance.rolloverColumn = -1
				outerInstance.rolloverColumnUpdated(oldRolloverColumn, outerInstance.rolloverColumn)
			End Sub
	'
	' Protected & Private Methods
	'

			Private Sub setDraggedDistance(ByVal draggedDistance As Integer, ByVal column As Integer)
				outerInstance.header.draggedDistance = draggedDistance
				If column <> -1 Then outerInstance.header.columnModel.moveColumn(column, column)
			End Sub
		End Class

	'
	'  Factory methods for the Listeners
	'

		''' <summary>
		''' Creates the mouse listener for the JTableHeader.
		''' </summary>
		Protected Friend Overridable Function createMouseInputListener() As MouseInputListener
			Return New MouseInputHandler(Me)
		End Function

	'
	'  The installation/uninstall procedures and support
	'

		Public Shared Function createUI(ByVal h As JComponent) As ComponentUI
			Return New BasicTableHeaderUI
		End Function

	'  Installation

		Public Overridable Sub installUI(ByVal c As JComponent)
			header = CType(c, JTableHeader)

			rendererPane = New CellRendererPane
			header.add(rendererPane)

			installDefaults()
			installListeners()
			installKeyboardActions()
		End Sub

		''' <summary>
		''' Initializes JTableHeader properties such as font, foreground, and background.
		''' The font, foreground, and background properties are only set if their
		''' current value is either null or a UIResource, other properties are set
		''' if the current value is null.
		''' </summary>
		''' <seealso cref= #installUI </seealso>
		Protected Friend Overridable Sub installDefaults()
			LookAndFeel.installColorsAndFont(header, "TableHeader.background", "TableHeader.foreground", "TableHeader.font")
			LookAndFeel.installProperty(header, "opaque", Boolean.TRUE)
		End Sub

		''' <summary>
		''' Attaches listeners to the JTableHeader.
		''' </summary>
		Protected Friend Overridable Sub installListeners()
			mouseInputListener = createMouseInputListener()

			header.addMouseListener(mouseInputListener)
			header.addMouseMotionListener(mouseInputListener)
			header.addFocusListener(focusListener)
		End Sub

		''' <summary>
		''' Register all keyboard actions on the JTableHeader.
		''' </summary>
		Protected Friend Overridable Sub installKeyboardActions()
			Dim keyMap As InputMap = CType(DefaultLookup.get(header, Me, "TableHeader.ancestorInputMap"), InputMap)
			SwingUtilities.replaceUIInputMap(header, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, keyMap)
			LazyActionMap.installLazyActionMap(header, GetType(BasicTableHeaderUI), "TableHeader.actionMap")
		End Sub

	' Uninstall methods

		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			uninstallDefaults()
			uninstallListeners()
			uninstallKeyboardActions()

			header.remove(rendererPane)
			rendererPane = Nothing
			header = Nothing
		End Sub

		Protected Friend Overridable Sub uninstallDefaults()
		End Sub

		Protected Friend Overridable Sub uninstallListeners()
			header.removeMouseListener(mouseInputListener)
			header.removeMouseMotionListener(mouseInputListener)

			mouseInputListener = Nothing
		End Sub

		''' <summary>
		''' Unregisters default key actions.
		''' </summary>
		Protected Friend Overridable Sub uninstallKeyboardActions()
			SwingUtilities.replaceUIInputMap(header, JComponent.WHEN_FOCUSED, Nothing)
			SwingUtilities.replaceUIActionMap(header, Nothing)
		End Sub

		''' <summary>
		''' Populates TableHeader's actions.
		''' </summary>
		Friend Shared Sub loadActionMap(ByVal map As LazyActionMap)
			map.put(New Actions(Actions.TOGGLE_SORT_ORDER))
			map.put(New Actions(Actions.SELECT_COLUMN_TO_LEFT))
			map.put(New Actions(Actions.SELECT_COLUMN_TO_RIGHT))
			map.put(New Actions(Actions.MOVE_COLUMN_LEFT))
			map.put(New Actions(Actions.MOVE_COLUMN_RIGHT))
			map.put(New Actions(Actions.RESIZE_LEFT))
			map.put(New Actions(Actions.RESIZE_RIGHT))
			map.put(New Actions(Actions.FOCUS_TABLE))
		End Sub

	'
	' Support for mouse rollover
	'

		''' <summary>
		''' Returns the index of the column header over which the mouse
		''' currently is. When the mouse is not over the table header,
		''' -1 is returned.
		''' </summary>
		''' <seealso cref= #rolloverColumnUpdated(int, int) </seealso>
		''' <returns> the index of the current rollover column
		''' @since 1.6 </returns>
		Protected Friend Overridable Property rolloverColumn As Integer
			Get
				Return rolloverColumn
			End Get
		End Property

		''' <summary>
		''' This method gets called every time when a rollover column in the table
		''' header is updated. Every look and feel that supports a rollover effect
		''' in a table header should override this method and repaint the header.
		''' </summary>
		''' <param name="oldColumn"> the index of the previous rollover column or -1 if the
		''' mouse was not over a column </param>
		''' <param name="newColumn"> the index of the new rollover column or -1 if the mouse
		''' is not over a column </param>
		''' <seealso cref= #getRolloverColumn() </seealso>
		''' <seealso cref= JTableHeader#getHeaderRect(int)
		''' @since 1.6 </seealso>
		Protected Friend Overridable Sub rolloverColumnUpdated(ByVal oldColumn As Integer, ByVal newColumn As Integer)
		End Sub

		Private Sub updateRolloverColumn(ByVal e As MouseEvent)
			If header.draggedColumn Is Nothing AndAlso header.contains(e.point) Then

				Dim col As Integer = header.columnAtPoint(e.point)
				If col <> rolloverColumn Then
					Dim oldRolloverColumn As Integer = rolloverColumn
					rolloverColumn = col
					rolloverColumnUpdated(oldRolloverColumn, rolloverColumn)
				End If
			End If
		End Sub

	'
	' Support for keyboard and mouse access
	'
		Private Function selectNextColumn(ByVal doIt As Boolean) As Integer
			Dim newIndex As Integer = selectedColumnIndex
			If newIndex < header.columnModel.columnCount - 1 Then
				newIndex += 1
				If doIt Then selectColumn(newIndex)
			End If
			Return newIndex
		End Function

		Private Function selectPreviousColumn(ByVal doIt As Boolean) As Integer
			Dim newIndex As Integer = selectedColumnIndex
			If newIndex > 0 Then
				newIndex -= 1
				If doIt Then selectColumn(newIndex)
			End If
			Return newIndex
		End Function

		''' <summary>
		''' Selects the specified column in the table header. Repaints the
		''' affected header cells and makes sure the newly selected one is visible.
		''' </summary>
		Friend Overridable Sub selectColumn(ByVal newColIndex As Integer)
			selectColumn(newColIndex, True)
		End Sub

		Friend Overridable Sub selectColumn(ByVal newColIndex As Integer, ByVal doScroll As Boolean)
			Dim repaintRect As Rectangle = header.getHeaderRect(selectedColumnIndex)
			header.repaint(repaintRect)
			selectedColumnIndex = newColIndex
			repaintRect = header.getHeaderRect(newColIndex)
			header.repaint(repaintRect)
			If doScroll Then scrollToColumn(newColIndex)
			Return
		End Sub
		''' <summary>
		''' Used by selectColumn to scroll horizontally, if necessary,
		''' to ensure that the newly selected column is visible.
		''' </summary>
		Private Sub scrollToColumn(ByVal col As Integer)
			Dim container As Container
			Dim table As JTable

			'Test whether the header is in a scroll pane and has a table.
			container = header.parent.parent
			table = header.table
			If (header.parent Is Nothing) OrElse (container Is Nothing) OrElse Not(TypeOf container Is JScrollPane) OrElse (table Is Nothing) Then Return

			'Now scroll, if necessary.
			Dim vis As Rectangle = table.visibleRect
			Dim cellBounds As Rectangle = table.getCellRect(0, col, True)
			vis.x = cellBounds.x
			vis.width = cellBounds.width
			table.scrollRectToVisible(vis)
		End Sub

		Private Property selectedColumnIndex As Integer
			Get
				Dim numCols As Integer = header.columnModel.columnCount
				If selectedColumnIndex >= numCols AndAlso numCols > 0 Then selectedColumnIndex = numCols - 1
				Return selectedColumnIndex
			End Get
		End Property

		Private Shared Function canResize(ByVal column As TableColumn, ByVal header As JTableHeader) As Boolean
			Return (column IsNot Nothing) AndAlso header.resizingAllowed AndAlso column.resizable
		End Function

		Private Function changeColumnWidth(ByVal resizingColumn As TableColumn, ByVal th As JTableHeader, ByVal oldWidth As Integer, ByVal newWidth As Integer) As Integer
			resizingColumn.width = newWidth

			Dim container As Container
			Dim table As JTable

			container = th.parent.parent
			table = th.table
			If (th.parent Is Nothing) OrElse (container Is Nothing) OrElse Not(TypeOf container Is JScrollPane) OrElse (table Is Nothing) Then Return 0

			If (Not container.componentOrientation.leftToRight) AndAlso (Not th.componentOrientation.leftToRight) Then
					Dim viewport As JViewport = CType(container, JScrollPane).viewport
					Dim viewportWidth As Integer = viewport.width
					Dim diff As Integer = newWidth - oldWidth
					Dim newHeaderWidth As Integer = table.width + diff

					' Resize a table 
					Dim tableSize As Dimension = table.size
					tableSize.width += diff
					table.size = tableSize

	'                 If this table is in AUTO_RESIZE_OFF mode and
	'                 * has a horizontal scrollbar, we need to update
	'                 * a view's position.
	'                 
					If (newHeaderWidth >= viewportWidth) AndAlso (table.autoResizeMode = JTable.AUTO_RESIZE_OFF) Then
						Dim p As Point = viewport.viewPosition
						p.x = Math.Max(0, Math.Min(newHeaderWidth - viewportWidth, p.x + diff))
						viewport.viewPosition = p
						Return diff
					End If
			End If
			Return 0
		End Function

	'
	' Baseline
	'

		''' <summary>
		''' Returns the baseline.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaseline(ByVal c As JComponent, ByVal width As Integer, ByVal height As Integer) As Integer
			MyBase.getBaseline(c, width, height)
			Dim ___baseline As Integer = -1
			Dim columnModel As TableColumnModel = header.columnModel
			For column As Integer = 0 To columnModel.columnCount - 1
				Dim aColumn As TableColumn = columnModel.getColumn(column)
				Dim comp As Component = getHeaderRenderer(column)
				Dim pref As Dimension = comp.preferredSize
				Dim columnBaseline As Integer = comp.getBaseline(pref.width, height)
				If columnBaseline >= 0 Then
					If ___baseline = -1 Then
						___baseline = columnBaseline
					ElseIf ___baseline <> columnBaseline Then
						___baseline = -1
						Exit For
					End If
				End If
			Next column
			Return ___baseline
		End Function

	'
	' Paint Methods and support
	'

		Public Overridable Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			If header.columnModel.columnCount <= 0 Then Return
			Dim ltr As Boolean = header.componentOrientation.leftToRight

			Dim clip As Rectangle = g.clipBounds
			Dim left As Point = clip.location
			Dim right As New Point(clip.x + clip.width - 1, clip.y)
			Dim cm As TableColumnModel = header.columnModel
			Dim cMin As Integer = header.columnAtPoint(If(ltr, left, right))
			Dim cMax As Integer = header.columnAtPoint(If(ltr, right, left))
			' This should never happen.
			If cMin = -1 Then cMin = 0
			' If the table does not have enough columns to fill the view we'll get -1.
			' Replace this with the index of the last column.
			If cMax = -1 Then cMax = cm.columnCount-1

			Dim draggedColumn As TableColumn = header.draggedColumn
			Dim columnWidth As Integer
			Dim cellRect As Rectangle = header.getHeaderRect(If(ltr, cMin, cMax))
			Dim aColumn As TableColumn
			If ltr Then
				For column As Integer = cMin To cMax
					aColumn = cm.getColumn(column)
					columnWidth = aColumn.width
					cellRect.width = columnWidth
					If aColumn IsNot draggedColumn Then paintCell(g, cellRect, column)
					cellRect.x += columnWidth
				Next column
			Else
				For column As Integer = cMax To cMin Step -1
					aColumn = cm.getColumn(column)
					columnWidth = aColumn.width
					cellRect.width = columnWidth
					If aColumn IsNot draggedColumn Then paintCell(g, cellRect, column)
					cellRect.x += columnWidth
				Next column
			End If

			' Paint the dragged column if we are dragging.
			If draggedColumn IsNot Nothing Then
				Dim draggedColumnIndex As Integer = viewIndexForColumn(draggedColumn)
				Dim draggedCellRect As Rectangle = header.getHeaderRect(draggedColumnIndex)

				' Draw a gray well in place of the moving column.
				g.color = header.parent.background
				g.fillRect(draggedCellRect.x, draggedCellRect.y, draggedCellRect.width, draggedCellRect.height)

				draggedCellRect.x += header.draggedDistance

				' Fill the background.
				g.color = header.background
				g.fillRect(draggedCellRect.x, draggedCellRect.y, draggedCellRect.width, draggedCellRect.height)

				paintCell(g, draggedCellRect, draggedColumnIndex)
			End If

			' Remove all components in the rendererPane.
			rendererPane.removeAll()
		End Sub

		Private Function getHeaderRenderer(ByVal columnIndex As Integer) As Component
			Dim aColumn As TableColumn = header.columnModel.getColumn(columnIndex)
			Dim renderer As TableCellRenderer = aColumn.headerRenderer
			If renderer Is Nothing Then renderer = header.defaultRenderer

			Dim hasFocus As Boolean = (Not header.paintingForPrint) AndAlso (columnIndex = selectedColumnIndex) AndAlso header.hasFocus()
			Return renderer.getTableCellRendererComponent(header.table, aColumn.headerValue, False, hasFocus, -1, columnIndex)
		End Function

		Private Sub paintCell(ByVal g As Graphics, ByVal cellRect As Rectangle, ByVal columnIndex As Integer)
			Dim component As Component = getHeaderRenderer(columnIndex)
			rendererPane.paintComponent(g, component, header, cellRect.x, cellRect.y, cellRect.width, cellRect.height, True)
		End Sub

		Private Function viewIndexForColumn(ByVal aColumn As TableColumn) As Integer
			Dim cm As TableColumnModel = header.columnModel
			For column As Integer = 0 To cm.columnCount - 1
				If cm.getColumn(column) Is aColumn Then Return column
			Next column
			Return -1
		End Function

	'
	' Size Methods
	'

		Private Property headerHeight As Integer
			Get
				Dim height As Integer = 0
				Dim accomodatedDefault As Boolean = False
				Dim columnModel As TableColumnModel = header.columnModel
				For column As Integer = 0 To columnModel.columnCount - 1
					Dim aColumn As TableColumn = columnModel.getColumn(column)
					Dim isDefault As Boolean = (aColumn.headerRenderer Is Nothing)
    
					If (Not isDefault) OrElse (Not accomodatedDefault) Then
						Dim comp As Component = getHeaderRenderer(column)
						Dim rendererHeight As Integer = comp.preferredSize.height
						height = Math.Max(height, rendererHeight)
    
						' Configuring the header renderer to calculate its preferred size
						' is expensive. Optimise this by assuming the default renderer
						' always has the same height as the first non-zero height that
						' it returns for a non-null/non-empty value.
						If isDefault AndAlso rendererHeight > 0 Then
							Dim headerValue As Object = aColumn.headerValue
							If headerValue IsNot Nothing Then
								headerValue = headerValue.ToString()
    
								If headerValue IsNot Nothing AndAlso (Not headerValue.Equals("")) Then accomodatedDefault = True
							End If
						End If
					End If
				Next column
				Return height
			End Get
		End Property

		Private Function createHeaderSize(ByVal width As Long) As Dimension
			' None of the callers include the intercell spacing, do it here.
			If width > Integer.MaxValue Then width = Integer.MaxValue
			Return New Dimension(CInt(width), headerHeight)
		End Function


		''' <summary>
		''' Return the minimum size of the header. The minimum width is the sum
		''' of the minimum widths of each column (plus inter-cell spacing).
		''' </summary>
		Public Overridable Function getMinimumSize(ByVal c As JComponent) As Dimension
			Dim width As Long = 0
			Dim enumeration As System.Collections.IEnumerator = header.columnModel.columns
			Do While enumeration.hasMoreElements()
				Dim aColumn As TableColumn = CType(enumeration.nextElement(), TableColumn)
				width = width + aColumn.minWidth
			Loop
			Return createHeaderSize(width)
		End Function

		''' <summary>
		''' Return the preferred size of the header. The preferred height is the
		''' maximum of the preferred heights of all of the components provided
		''' by the header renderers. The preferred width is the sum of the
		''' preferred widths of each column (plus inter-cell spacing).
		''' </summary>
		Public Overridable Function getPreferredSize(ByVal c As JComponent) As Dimension
			Dim width As Long = 0
			Dim enumeration As System.Collections.IEnumerator = header.columnModel.columns
			Do While enumeration.hasMoreElements()
				Dim aColumn As TableColumn = CType(enumeration.nextElement(), TableColumn)
				width = width + aColumn.preferredWidth
			Loop
			Return createHeaderSize(width)
		End Function

		''' <summary>
		''' Return the maximum size of the header. The maximum width is the sum
		''' of the maximum widths of each column (plus inter-cell spacing).
		''' </summary>
		Public Overridable Function getMaximumSize(ByVal c As JComponent) As Dimension
			Dim width As Long = 0
			Dim enumeration As System.Collections.IEnumerator = header.columnModel.columns
			Do While enumeration.hasMoreElements()
				Dim aColumn As TableColumn = CType(enumeration.nextElement(), TableColumn)
				width = width + aColumn.maxWidth
			Loop
			Return createHeaderSize(width)
		End Function

		Private Class Actions
			Inherits UIAction

			Public Const TOGGLE_SORT_ORDER As String = "toggleSortOrder"
			Public Const SELECT_COLUMN_TO_LEFT As String = "selectColumnToLeft"
			Public Const SELECT_COLUMN_TO_RIGHT As String = "selectColumnToRight"
			Public Const MOVE_COLUMN_LEFT As String = "moveColumnLeft"
			Public Const MOVE_COLUMN_RIGHT As String = "moveColumnRight"
			Public Const RESIZE_LEFT As String = "resizeLeft"
			Public Const RESIZE_RIGHT As String = "resizeRight"
			Public Const FOCUS_TABLE As String = "focusTable"

			Public Sub New(ByVal name As String)
				MyBase.New(name)
			End Sub

			Public Overridable Function isEnabled(ByVal sender As Object) As Boolean
				If TypeOf sender Is JTableHeader Then
					Dim th As JTableHeader = CType(sender, JTableHeader)
					Dim cm As TableColumnModel = th.columnModel
					If cm.columnCount <= 0 Then Return False

					Dim key As String = name
					Dim ui As BasicTableHeaderUI = CType(BasicLookAndFeel.getUIOfType(th.uI, GetType(BasicTableHeaderUI)), BasicTableHeaderUI)
					If ui IsNot Nothing Then
						If key = MOVE_COLUMN_LEFT Then
							Return th.reorderingAllowed AndAlso maybeMoveColumn(True, th, ui, False)
						ElseIf key = MOVE_COLUMN_RIGHT Then
							Return th.reorderingAllowed AndAlso maybeMoveColumn(False, th, ui, False)
						ElseIf key = RESIZE_LEFT OrElse key = RESIZE_RIGHT Then
							Return canResize(cm.getColumn(ui.selectedColumnIndex), th)
						ElseIf key = FOCUS_TABLE Then
							Return (th.table IsNot Nothing)
						End If
					End If
				End If
				Return True
			End Function

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				Dim th As JTableHeader = CType(e.source, JTableHeader)
				Dim ui As BasicTableHeaderUI = CType(BasicLookAndFeel.getUIOfType(th.uI, GetType(BasicTableHeaderUI)), BasicTableHeaderUI)
				If ui Is Nothing Then Return

				Dim name As String = name
				If TOGGLE_SORT_ORDER = name Then
					Dim table As JTable = th.table
					Dim sorter As RowSorter = If(table Is Nothing, Nothing, table.rowSorter)
					If sorter IsNot Nothing Then
						Dim columnIndex As Integer = ui.selectedColumnIndex
						columnIndex = table.convertColumnIndexToModel(columnIndex)
						sorter.toggleSortOrder(columnIndex)
					End If
				ElseIf SELECT_COLUMN_TO_LEFT = name Then
					If th.componentOrientation.leftToRight Then
						ui.selectPreviousColumn(True)
					Else
						ui.selectNextColumn(True)
					End If
				ElseIf SELECT_COLUMN_TO_RIGHT = name Then
					If th.componentOrientation.leftToRight Then
						ui.selectNextColumn(True)
					Else
						ui.selectPreviousColumn(True)
					End If
				ElseIf MOVE_COLUMN_LEFT = name Then
					moveColumn(True, th, ui)
				ElseIf MOVE_COLUMN_RIGHT = name Then
					moveColumn(False, th, ui)
				ElseIf RESIZE_LEFT = name Then
					resize(True, th, ui)
				ElseIf RESIZE_RIGHT = name Then
					resize(False, th, ui)
				ElseIf FOCUS_TABLE = name Then
					Dim table As JTable = th.table
					If table IsNot Nothing Then table.requestFocusInWindow()
				End If
			End Sub

			Private Sub moveColumn(ByVal leftArrow As Boolean, ByVal th As JTableHeader, ByVal ui As BasicTableHeaderUI)
				maybeMoveColumn(leftArrow, th, ui, True)
			End Sub

			Private Function maybeMoveColumn(ByVal leftArrow As Boolean, ByVal th As JTableHeader, ByVal ui As BasicTableHeaderUI, ByVal doIt As Boolean) As Boolean
				Dim oldIndex As Integer = ui.selectedColumnIndex
				Dim newIndex As Integer

				If th.componentOrientation.leftToRight Then
					newIndex = If(leftArrow, ui.selectPreviousColumn(doIt), ui.selectNextColumn(doIt))
				Else
					newIndex = If(leftArrow, ui.selectNextColumn(doIt), ui.selectPreviousColumn(doIt))
				End If

				If newIndex <> oldIndex Then
					If doIt Then
						th.columnModel.moveColumn(oldIndex, newIndex)
					Else
						Return True ' we'd do the move if asked
					End If
				End If

				Return False
			End Function

			Private Sub resize(ByVal leftArrow As Boolean, ByVal th As JTableHeader, ByVal ui As BasicTableHeaderUI)
				Dim columnIndex As Integer = ui.selectedColumnIndex
				Dim resizingColumn As TableColumn = th.columnModel.getColumn(columnIndex)

				th.resizingColumn = resizingColumn
				Dim oldWidth As Integer = resizingColumn.width
				Dim newWidth As Integer = oldWidth

				If th.componentOrientation.leftToRight Then
					newWidth = newWidth + (If(leftArrow, -1, 1))
				Else
					newWidth = newWidth + (If(leftArrow, 1, -1))
				End If

				ui.changeColumnWidth(resizingColumn, th, oldWidth, newWidth)
			End Sub
		End Class
	End Class ' End of Class BasicTableHeaderUI

End Namespace
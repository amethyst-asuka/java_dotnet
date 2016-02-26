Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Text
Imports javax.swing
Imports javax.swing.event
Imports javax.swing.plaf
Imports javax.swing.text
Imports javax.swing.table

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
	''' BasicTableUI implementation
	''' 
	''' @author Philip Milne
	''' @author Shannon Hickey (drag and drop)
	''' </summary>
	Public Class BasicTableUI
		Inherits TableUI

		Private Shared ReadOnly BASELINE_COMPONENT_KEY As New StringBuilder("Table.baselineComponent")

	'
	' Instance Variables
	'

		' The JTable that is delegating the painting to this UI.
		Protected Friend table As JTable
		Protected Friend rendererPane As CellRendererPane

		' Listeners that are attached to the JTable
		Protected Friend keyListener As KeyListener
		Protected Friend focusListener As FocusListener
		Protected Friend mouseInputListener As MouseInputListener

		Private handler As Handler

		''' <summary>
		''' Local cache of Table's client property "Table.isFileList"
		''' </summary>
		Private isFileList As Boolean = False

	'
	'  Helper class for keyboard actions
	'

		Private Class Actions
			Inherits sun.swing.UIAction

			Private Const CANCEL_EDITING As String = "cancel"
			Private Const SELECT_ALL As String = "selectAll"
			Private Const CLEAR_SELECTION As String = "clearSelection"
			Private Const START_EDITING As String = "startEditing"

			Private Const NEXT_ROW As String = "selectNextRow"
			Private Const NEXT_ROW_CELL As String = "selectNextRowCell"
			Private Const NEXT_ROW_EXTEND_SELECTION As String = "selectNextRowExtendSelection"
			Private Const NEXT_ROW_CHANGE_LEAD As String = "selectNextRowChangeLead"
			Private Const PREVIOUS_ROW As String = "selectPreviousRow"
			Private Const PREVIOUS_ROW_CELL As String = "selectPreviousRowCell"
			Private Const PREVIOUS_ROW_EXTEND_SELECTION As String = "selectPreviousRowExtendSelection"
			Private Const PREVIOUS_ROW_CHANGE_LEAD As String = "selectPreviousRowChangeLead"

			Private Const NEXT_COLUMN As String = "selectNextColumn"
			Private Const NEXT_COLUMN_CELL As String = "selectNextColumnCell"
			Private Const NEXT_COLUMN_EXTEND_SELECTION As String = "selectNextColumnExtendSelection"
			Private Const NEXT_COLUMN_CHANGE_LEAD As String = "selectNextColumnChangeLead"
			Private Const PREVIOUS_COLUMN As String = "selectPreviousColumn"
			Private Const PREVIOUS_COLUMN_CELL As String = "selectPreviousColumnCell"
			Private Const PREVIOUS_COLUMN_EXTEND_SELECTION As String = "selectPreviousColumnExtendSelection"
			Private Const PREVIOUS_COLUMN_CHANGE_LEAD As String = "selectPreviousColumnChangeLead"

			Private Const SCROLL_LEFT_CHANGE_SELECTION As String = "scrollLeftChangeSelection"
			Private Const SCROLL_LEFT_EXTEND_SELECTION As String = "scrollLeftExtendSelection"
			Private Const SCROLL_RIGHT_CHANGE_SELECTION As String = "scrollRightChangeSelection"
			Private Const SCROLL_RIGHT_EXTEND_SELECTION As String = "scrollRightExtendSelection"

			Private Const SCROLL_UP_CHANGE_SELECTION As String = "scrollUpChangeSelection"
			Private Const SCROLL_UP_EXTEND_SELECTION As String = "scrollUpExtendSelection"
			Private Const SCROLL_DOWN_CHANGE_SELECTION As String = "scrollDownChangeSelection"
			Private Const SCROLL_DOWN_EXTEND_SELECTION As String = "scrollDownExtendSelection"

			Private Const FIRST_COLUMN As String = "selectFirstColumn"
			Private Const FIRST_COLUMN_EXTEND_SELECTION As String = "selectFirstColumnExtendSelection"
			Private Const LAST_COLUMN As String = "selectLastColumn"
			Private Const LAST_COLUMN_EXTEND_SELECTION As String = "selectLastColumnExtendSelection"

			Private Const FIRST_ROW As String = "selectFirstRow"
			Private Const FIRST_ROW_EXTEND_SELECTION As String = "selectFirstRowExtendSelection"
			Private Const LAST_ROW As String = "selectLastRow"
			Private Const LAST_ROW_EXTEND_SELECTION As String = "selectLastRowExtendSelection"

			' add the lead item to the selection without changing lead or anchor
			Private Const ADD_TO_SELECTION As String = "addToSelection"

			' toggle the selected state of the lead item and move the anchor to it
			Private Const TOGGLE_AND_ANCHOR As String = "toggleAndAnchor"

			' extend the selection to the lead item
			Private Const EXTEND_TO As String = "extendTo"

			' move the anchor to the lead and ensure only that item is selected
			Private Const MOVE_SELECTION_TO As String = "moveSelectionTo"

			' give focus to the JTableHeader, if one exists
			Private Const FOCUS_HEADER As String = "focusHeader"

			Protected Friend dx As Integer
			Protected Friend dy As Integer
			Protected Friend extend As Boolean
			Protected Friend inSelection As Boolean

			' horizontally, forwards always means right,
			' regardless of component orientation
			Protected Friend forwards As Boolean
			Protected Friend vertically As Boolean
			Protected Friend toLimit As Boolean

			Protected Friend leadRow As Integer
			Protected Friend leadColumn As Integer

			Friend Sub New(ByVal name As String)
				MyBase.New(name)
			End Sub

			Friend Sub New(ByVal name As String, ByVal dx As Integer, ByVal dy As Integer, ByVal extend As Boolean, ByVal inSelection As Boolean)
				MyBase.New(name)

				' Actions spcifying true for "inSelection" are
				' fairly sensitive to bad parameter values. They require
				' that one of dx and dy be 0 and the other be -1 or 1.
				' Bogus parameter values could cause an infinite loop.
				' To prevent any problems we massage the params here
				' and complain if we get something we can't deal with.
				If inSelection Then
					Me.inSelection = True

					' look at the sign of dx and dy only
					dx = sign(dx)
					dy = sign(dy)

					' make sure one is zero, but not both
					assert(dx = 0 OrElse dy = 0) AndAlso Not(dx = 0 AndAlso dy = 0)
				End If

				Me.dx = dx
				Me.dy = dy
				Me.extend = extend
			End Sub

			Friend Sub New(ByVal name As String, ByVal extend As Boolean, ByVal forwards As Boolean, ByVal vertically As Boolean, ByVal toLimit As Boolean)
				Me.New(name, 0, 0, extend, False)
				Me.forwards = forwards
				Me.vertically = vertically
				Me.toLimit = toLimit
			End Sub

			Private Shared Function clipToRange(ByVal i As Integer, ByVal a As Integer, ByVal b As Integer) As Integer
				Return Math.Min(Math.Max(i, a), b-1)
			End Function

			Private Sub moveWithinTableRange(ByVal table As JTable, ByVal dx As Integer, ByVal dy As Integer)
				leadRow = clipToRange(leadRow+dy, 0, table.rowCount)
				leadColumn = clipToRange(leadColumn+dx, 0, table.columnCount)
			End Sub

			Private Shared Function sign(ByVal num As Integer) As Integer
				Return If(num < 0, -1, (If(num = 0, 0, 1)))
			End Function

			''' <summary>
			''' Called to move within the selected range of the given JTable.
			''' This method uses the table's notion of selection, which is
			''' important to allow the user to navigate between items visually
			''' selected on screen. This notion may or may not be the same as
			''' what could be determined by directly querying the selection models.
			''' It depends on certain table properties (such as whether or not
			''' row or column selection is allowed). When performing modifications,
			''' it is recommended that caution be taken in order to preserve
			''' the intent of this method, especially when deciding whether to
			''' query the selection models or interact with JTable directly.
			''' </summary>
			Private Function moveWithinSelectedRange(ByVal table As JTable, ByVal dx As Integer, ByVal dy As Integer, ByVal rsm As ListSelectionModel, ByVal csm As ListSelectionModel) As Boolean

				' Note: The Actions constructor ensures that only one of
				' dx and dy is 0, and the other is either -1 or 1

				' find out how many items the table is showing as selected
				' and the range of items to navigate through
				Dim totalCount As Integer
				Dim minX, maxX, minY, maxY As Integer

				Dim rs As Boolean = table.rowSelectionAllowed
				Dim cs As Boolean = table.columnSelectionAllowed

				' both column and row selection
				If rs AndAlso cs Then
					totalCount = table.selectedRowCount * table.selectedColumnCount
					minX = csm.minSelectionIndex
					maxX = csm.maxSelectionIndex
					minY = rsm.minSelectionIndex
					maxY = rsm.maxSelectionIndex
				' row selection only
				ElseIf rs Then
					totalCount = table.selectedRowCount
					minX = 0
					maxX = table.columnCount - 1
					minY = rsm.minSelectionIndex
					maxY = rsm.maxSelectionIndex
				' column selection only
				ElseIf cs Then
					totalCount = table.selectedColumnCount
					minX = csm.minSelectionIndex
					maxX = csm.maxSelectionIndex
					minY = 0
					maxY = table.rowCount - 1
				' no selection allowed
				Else
					totalCount = 0
					' A bogus assignment to stop javac from complaining
					' about unitialized values. In this case, these
					' won't even be used.
						maxY = 0
							minY = maxY
								maxX = minY
								minX = maxX
				End If

				' For some cases, there is no point in trying to stay within the
				' selected area. Instead, move outside the selection, wrapping at
				' the table boundaries. The cases are:
				Dim stayInSelection As Boolean

				' - nothing selected
				If totalCount = 0 OrElse (totalCount = 1 AndAlso table.isCellSelected(leadRow, leadColumn)) Then
						' - one item selected, and the lead is already selected

					stayInSelection = False

					maxX = table.columnCount - 1
					maxY = table.rowCount - 1

					' the mins are calculated like this in case the max is -1
					minX = Math.Min(0, maxX)
					minY = Math.Min(0, maxY)
				Else
					stayInSelection = True
				End If

				' the algorithm below isn't prepared to deal with -1 lead/anchor
				' so massage appropriately here first
				If dy = 1 AndAlso leadColumn = -1 Then
					leadColumn = minX
					leadRow = -1
				ElseIf dx = 1 AndAlso leadRow = -1 Then
					leadRow = minY
					leadColumn = -1
				ElseIf dy = -1 AndAlso leadColumn = -1 Then
					leadColumn = maxX
					leadRow = maxY + 1
				ElseIf dx = -1 AndAlso leadRow = -1 Then
					leadRow = maxY
					leadColumn = maxX + 1
				End If

				' In cases where the lead is not within the search range,
				' we need to bring it within one cell for the the search
				' to work properly. Check these here.
				leadRow = Math.Min(Math.Max(leadRow, minY - 1), maxY + 1)
				leadColumn = Math.Min(Math.Max(leadColumn, minX - 1), maxX + 1)

				' find the next position, possibly looping until it is selected
				Do
					calcNextPos(dx, minX, maxX, dy, minY, maxY)
				Loop While stayInSelection AndAlso Not table.isCellSelected(leadRow, leadColumn)

				Return stayInSelection
			End Function

			''' <summary>
			''' Find the next lead row and column based on the given
			''' dx/dy and max/min values.
			''' </summary>
			Private Sub calcNextPos(ByVal dx As Integer, ByVal minX As Integer, ByVal maxX As Integer, ByVal dy As Integer, ByVal minY As Integer, ByVal maxY As Integer)

				If dx <> 0 Then
					leadColumn += dx
					If leadColumn > maxX Then
						leadColumn = minX
						leadRow += 1
						If leadRow > maxY Then leadRow = minY
					ElseIf leadColumn < minX Then
						leadColumn = maxX
						leadRow -= 1
						If leadRow < minY Then leadRow = maxY
					End If
				Else
					leadRow += dy
					If leadRow > maxY Then
						leadRow = minY
						leadColumn += 1
						If leadColumn > maxX Then leadColumn = minX
					ElseIf leadRow < minY Then
						leadRow = maxY
						leadColumn -= 1
						If leadColumn < minX Then leadColumn = maxX
					End If
				End If
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				Dim key As String = name
				Dim table As JTable = CType(e.source, JTable)

				Dim rsm As ListSelectionModel = table.selectionModel
				leadRow = getAdjustedLead(table, True, rsm)

				Dim csm As ListSelectionModel = table.columnModel.selectionModel
				leadColumn = getAdjustedLead(table, False, csm)

				If key = SCROLL_LEFT_CHANGE_SELECTION OrElse key = SCROLL_LEFT_EXTEND_SELECTION OrElse key = SCROLL_RIGHT_CHANGE_SELECTION OrElse key = SCROLL_RIGHT_EXTEND_SELECTION OrElse key = SCROLL_UP_CHANGE_SELECTION OrElse key = SCROLL_UP_EXTEND_SELECTION OrElse key = SCROLL_DOWN_CHANGE_SELECTION OrElse key = SCROLL_DOWN_EXTEND_SELECTION OrElse key = FIRST_COLUMN OrElse key = FIRST_COLUMN_EXTEND_SELECTION OrElse key = FIRST_ROW OrElse key = FIRST_ROW_EXTEND_SELECTION OrElse key = LAST_COLUMN OrElse key = LAST_COLUMN_EXTEND_SELECTION OrElse key = LAST_ROW OrElse key = LAST_ROW_EXTEND_SELECTION Then ' Paging Actions
					If toLimit Then
						If vertically Then
							Dim rowCount As Integer = table.rowCount
							Me.dx = 0
							Me.dy = If(forwards, rowCount, -rowCount)
						Else
							Dim colCount As Integer = table.columnCount
							Me.dx = If(forwards, colCount, -colCount)
							Me.dy = 0
						End If
					Else
						If Not(TypeOf SwingUtilities.getUnwrappedParent(table).parent Is JScrollPane) Then Return

						Dim delta As Dimension = table.parent.size

						If vertically Then
							Dim r As Rectangle = table.getCellRect(leadRow, 0, True)
							If forwards Then
								' scroll by at least one cell
								r.y += Math.Max(delta.height, r.height)
							Else
								r.y -= delta.height
							End If

							Me.dx = 0
							Dim newRow As Integer = table.rowAtPoint(r.location)
							If newRow = -1 AndAlso forwards Then newRow = table.rowCount
							Me.dy = newRow - leadRow
						Else
							Dim r As Rectangle = table.getCellRect(0, leadColumn, True)

							If forwards Then
								' scroll by at least one cell
								r.x += Math.Max(delta.width, r.width)
							Else
								r.x -= delta.width
							End If

							Dim newColumn As Integer = table.columnAtPoint(r.location)
							If newColumn = -1 Then
								Dim ltr As Boolean = table.componentOrientation.leftToRight

								newColumn = If(forwards, (If(ltr, table.columnCount, 0)), (If(ltr, 0, table.columnCount)))

							End If
							Me.dx = newColumn - leadColumn
							Me.dy = 0
						End If
					End If
				End If
				If key = NEXT_ROW OrElse key = NEXT_ROW_CELL OrElse key = NEXT_ROW_EXTEND_SELECTION OrElse key = NEXT_ROW_CHANGE_LEAD OrElse key = NEXT_COLUMN OrElse key = NEXT_COLUMN_CELL OrElse key = NEXT_COLUMN_EXTEND_SELECTION OrElse key = NEXT_COLUMN_CHANGE_LEAD OrElse key = PREVIOUS_ROW OrElse key = PREVIOUS_ROW_CELL OrElse key = PREVIOUS_ROW_EXTEND_SELECTION OrElse key = PREVIOUS_ROW_CHANGE_LEAD OrElse key = PREVIOUS_COLUMN OrElse key = PREVIOUS_COLUMN_CELL OrElse key = PREVIOUS_COLUMN_EXTEND_SELECTION OrElse key = PREVIOUS_COLUMN_CHANGE_LEAD OrElse key = SCROLL_LEFT_CHANGE_SELECTION OrElse key = SCROLL_LEFT_EXTEND_SELECTION OrElse key = SCROLL_RIGHT_CHANGE_SELECTION OrElse key = SCROLL_RIGHT_EXTEND_SELECTION OrElse key = SCROLL_UP_CHANGE_SELECTION OrElse key = SCROLL_UP_EXTEND_SELECTION OrElse key = SCROLL_DOWN_CHANGE_SELECTION OrElse key = SCROLL_DOWN_EXTEND_SELECTION OrElse key = FIRST_COLUMN OrElse key = FIRST_COLUMN_EXTEND_SELECTION OrElse key = FIRST_ROW OrElse key = FIRST_ROW_EXTEND_SELECTION OrElse key = LAST_COLUMN OrElse key = LAST_COLUMN_EXTEND_SELECTION OrElse key = LAST_ROW OrElse key = LAST_ROW_EXTEND_SELECTION Then ' Navigate Actions
						' Paging Actions.

					If table.editing AndAlso (Not table.cellEditor.stopCellEditing()) Then Return

					' Unfortunately, this strategy introduces bugs because
					' of the asynchronous nature of requestFocus() call below.
					' Introducing a delay with invokeLater() makes this work
					' in the typical case though race conditions then allow
					' focus to disappear altogether. The right solution appears
					' to be to fix requestFocus() so that it queues a request
					' for the focus regardless of who owns the focus at the
					' time the call to requestFocus() is made. The optimisation
					' to ignore the call to requestFocus() when the component
					' already has focus may ligitimately be made as the
					' request focus event is dequeued, not before.

					' boolean wasEditingWithFocus = table.isEditing() &&
					' table.getEditorComponent().isFocusOwner();

					Dim changeLead As Boolean = False
					If key = NEXT_ROW_CHANGE_LEAD OrElse key = PREVIOUS_ROW_CHANGE_LEAD Then
						changeLead = (rsm.selectionMode = ListSelectionModel.MULTIPLE_INTERVAL_SELECTION)
					ElseIf key = NEXT_COLUMN_CHANGE_LEAD OrElse key = PREVIOUS_COLUMN_CHANGE_LEAD Then
						changeLead = (csm.selectionMode = ListSelectionModel.MULTIPLE_INTERVAL_SELECTION)
					End If

					If changeLead Then
						moveWithinTableRange(table, dx, dy)
						If dy <> 0 Then
							' casting should be safe since the action is only enabled
							' for DefaultListSelectionModel
							CType(rsm, DefaultListSelectionModel).moveLeadSelectionIndex(leadRow)
							If getAdjustedLead(table, False, csm) = -1 AndAlso table.columnCount > 0 Then CType(csm, DefaultListSelectionModel).moveLeadSelectionIndex(0)
						Else
							' casting should be safe since the action is only enabled
							' for DefaultListSelectionModel
							CType(csm, DefaultListSelectionModel).moveLeadSelectionIndex(leadColumn)
							If getAdjustedLead(table, True, rsm) = -1 AndAlso table.rowCount > 0 Then CType(rsm, DefaultListSelectionModel).moveLeadSelectionIndex(0)
						End If

						Dim cellRect As Rectangle = table.getCellRect(leadRow, leadColumn, False)
						If cellRect IsNot Nothing Then table.scrollRectToVisible(cellRect)
					ElseIf Not inSelection Then
						moveWithinTableRange(table, dx, dy)
						table.changeSelection(leadRow, leadColumn, False, extend)
					Else
						If table.rowCount <= 0 OrElse table.columnCount <= 0 Then Return

						If moveWithinSelectedRange(table, dx, dy, rsm, csm) Then
							' this is the only way we have to set both the lead
							' and the anchor without changing the selection
							If rsm.isSelectedIndex(leadRow) Then
								rsm.addSelectionInterval(leadRow, leadRow)
							Else
								rsm.removeSelectionInterval(leadRow, leadRow)
							End If

							If csm.isSelectedIndex(leadColumn) Then
								csm.addSelectionInterval(leadColumn, leadColumn)
							Else
								csm.removeSelectionInterval(leadColumn, leadColumn)
							End If

							Dim cellRect As Rectangle = table.getCellRect(leadRow, leadColumn, False)
							If cellRect IsNot Nothing Then table.scrollRectToVisible(cellRect)
						Else
							table.changeSelection(leadRow, leadColumn, False, False)
						End If
					End If

	'                
	'                if (wasEditingWithFocus) {
	'                    table.editCellAt(leadRow, leadColumn);
	'                    final Component editorComp = table.getEditorComponent();
	'                    if (editorComp != null) {
	'                        SwingUtilities.invokeLater(new Runnable() {
	'                            public void run() {
	'                                editorComp.requestFocus();
	'                            }
	'                        });
	'                    }
	'                }
	'                
				ElseIf key = CANCEL_EDITING Then
					table.removeEditor()
				ElseIf key = SELECT_ALL Then
					table.selectAll()
				ElseIf key = CLEAR_SELECTION Then
					table.clearSelection()
				ElseIf key = START_EDITING Then
					If Not table.hasFocus() Then
						Dim cellEditor As CellEditor = table.cellEditor
						If cellEditor IsNot Nothing AndAlso (Not cellEditor.stopCellEditing()) Then Return
						table.requestFocus()
						Return
					End If
					table.editCellAt(leadRow, leadColumn, e)
					Dim editorComp As Component = table.editorComponent
					If editorComp IsNot Nothing Then editorComp.requestFocus()
				ElseIf key = ADD_TO_SELECTION Then
					If Not table.isCellSelected(leadRow, leadColumn) Then
						Dim oldAnchorRow As Integer = rsm.anchorSelectionIndex
						Dim oldAnchorColumn As Integer = csm.anchorSelectionIndex
						rsm.valueIsAdjusting = True
						csm.valueIsAdjusting = True
						table.changeSelection(leadRow, leadColumn, True, False)
						rsm.anchorSelectionIndex = oldAnchorRow
						csm.anchorSelectionIndex = oldAnchorColumn
						rsm.valueIsAdjusting = False
						csm.valueIsAdjusting = False
					End If
				ElseIf key = TOGGLE_AND_ANCHOR Then
					table.changeSelection(leadRow, leadColumn, True, False)
				ElseIf key = EXTEND_TO Then
					table.changeSelection(leadRow, leadColumn, False, True)
				ElseIf key = MOVE_SELECTION_TO Then
					table.changeSelection(leadRow, leadColumn, False, False)
				ElseIf key = FOCUS_HEADER Then
					Dim th As JTableHeader = table.tableHeader
					If th IsNot Nothing Then
						'Set the header's selected column to match the table.
						Dim col As Integer = table.selectedColumn
						If col >= 0 Then
							Dim thUI As TableHeaderUI = th.uI
							If TypeOf thUI Is BasicTableHeaderUI Then CType(thUI, BasicTableHeaderUI).selectColumn(col)
						End If

						'Then give the header the focus.
						th.requestFocusInWindow()
					End If
				End If
			End Sub

			Public Overridable Function isEnabled(ByVal sender As Object) As Boolean
				Dim key As String = name

				If TypeOf sender Is JTable AndAlso Boolean.TRUE.Equals(CType(sender, JTable).getClientProperty("Table.isFileList")) Then
					If key = NEXT_COLUMN OrElse key = NEXT_COLUMN_CELL OrElse key = NEXT_COLUMN_EXTEND_SELECTION OrElse key = NEXT_COLUMN_CHANGE_LEAD OrElse key = PREVIOUS_COLUMN OrElse key = PREVIOUS_COLUMN_CELL OrElse key = PREVIOUS_COLUMN_EXTEND_SELECTION OrElse key = PREVIOUS_COLUMN_CHANGE_LEAD OrElse key = SCROLL_LEFT_CHANGE_SELECTION OrElse key = SCROLL_LEFT_EXTEND_SELECTION OrElse key = SCROLL_RIGHT_CHANGE_SELECTION OrElse key = SCROLL_RIGHT_EXTEND_SELECTION OrElse key = FIRST_COLUMN OrElse key = FIRST_COLUMN_EXTEND_SELECTION OrElse key = LAST_COLUMN OrElse key = LAST_COLUMN_EXTEND_SELECTION OrElse key = NEXT_ROW_CELL OrElse key = PREVIOUS_ROW_CELL Then Return False
				End If

				If key = CANCEL_EDITING AndAlso TypeOf sender Is JTable Then
					Return CType(sender, JTable).editing
				ElseIf key = NEXT_ROW_CHANGE_LEAD OrElse key = PREVIOUS_ROW_CHANGE_LEAD Then
					' discontinuous selection actions are only enabled for
					' DefaultListSelectionModel
					Return sender IsNot Nothing AndAlso TypeOf CType(sender, JTable).selectionModel Is DefaultListSelectionModel
				ElseIf key = NEXT_COLUMN_CHANGE_LEAD OrElse key = PREVIOUS_COLUMN_CHANGE_LEAD Then
					' discontinuous selection actions are only enabled for
					' DefaultListSelectionModel
					Return sender IsNot Nothing AndAlso TypeOf CType(sender, JTable).columnModel.selectionModel Is DefaultListSelectionModel
				ElseIf key = ADD_TO_SELECTION AndAlso TypeOf sender Is JTable Then
					' This action is typically bound to SPACE.
					' If the table is already in an editing mode, SPACE should
					' simply enter a space character into the table, and not
					' select a cell. Likewise, if the lead cell is already selected
					' then hitting SPACE should just enter a space character
					' into the cell and begin editing. In both of these cases
					' this action will be disabled.
					Dim table As JTable = CType(sender, JTable)
					Dim leadRow As Integer = getAdjustedLead(table, True)
					Dim leadCol As Integer = getAdjustedLead(table, False)
					Return Not(table.editing OrElse table.isCellSelected(leadRow, leadCol))
				ElseIf key = FOCUS_HEADER AndAlso TypeOf sender Is JTable Then
					Dim table As JTable = CType(sender, JTable)
					Return table.tableHeader IsNot Nothing
				End If

				Return True
			End Function
		End Class


	'
	'  The Table's Key listener
	'

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of {@code BasicTableUI}.
		''' <p>As of Java 2 platform v1.3 this class is no longer used.
		''' Instead <code>JTable</code>
		''' overrides <code>processKeyBinding</code> to dispatch the event to
		''' the current <code>TableCellEditor</code>.
		''' </summary>
		 Public Class KeyHandler
			 Implements KeyListener

			 Private ReadOnly outerInstance As BasicTableUI

			 Public Sub New(ByVal outerInstance As BasicTableUI)
				 Me.outerInstance = outerInstance
			 End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.
			Public Overridable Sub keyPressed(ByVal e As KeyEvent)
				outerInstance.handler.keyPressed(e)
			End Sub

			Public Overridable Sub keyReleased(ByVal e As KeyEvent)
				outerInstance.handler.keyReleased(e)
			End Sub

			Public Overridable Sub keyTyped(ByVal e As KeyEvent)
				outerInstance.handler.keyTyped(e)
			End Sub
		 End Class

	'
	'  The Table's focus listener
	'

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of {@code BasicTableUI}.
		''' </summary>
		Public Class FocusHandler
			Implements FocusListener

			Private ReadOnly outerInstance As BasicTableUI

			Public Sub New(ByVal outerInstance As BasicTableUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.
			Public Overridable Sub focusGained(ByVal e As FocusEvent)
				outerInstance.handler.focusGained(e)
			End Sub

			Public Overridable Sub focusLost(ByVal e As FocusEvent)
				outerInstance.handler.focusLost(e)
			End Sub
		End Class

	'
	'  The Table's mouse and mouse motion listeners
	'

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of BasicTableUI.
		''' </summary>
		Public Class MouseInputHandler
			Implements MouseInputListener

			Private ReadOnly outerInstance As BasicTableUI

			Public Sub New(ByVal outerInstance As BasicTableUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.
			Public Overridable Sub mouseClicked(ByVal e As MouseEvent)
				outerInstance.handler.mouseClicked(e)
			End Sub

			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				outerInstance.handler.mousePressed(e)
			End Sub

			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
				outerInstance.handler.mouseReleased(e)
			End Sub

			Public Overridable Sub mouseEntered(ByVal e As MouseEvent)
				outerInstance.handler.mouseEntered(e)
			End Sub

			Public Overridable Sub mouseExited(ByVal e As MouseEvent)
				outerInstance.handler.mouseExited(e)
			End Sub

			Public Overridable Sub mouseMoved(ByVal e As MouseEvent)
				outerInstance.handler.mouseMoved(e)
			End Sub

			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
				outerInstance.handler.mouseDragged(e)
			End Sub
		End Class

		Private Class Handler
			Implements FocusListener, MouseInputListener, java.beans.PropertyChangeListener, ListSelectionListener, ActionListener, javax.swing.plaf.basic.DragRecognitionSupport.BeforeDrag

			Private ReadOnly outerInstance As BasicTableUI

			Public Sub New(ByVal outerInstance As BasicTableUI)
				Me.outerInstance = outerInstance
			End Sub


			' FocusListener
			Private Sub repaintLeadCell()
				Dim lr As Integer = getAdjustedLead(outerInstance.table, True)
				Dim lc As Integer = getAdjustedLead(outerInstance.table, False)

				If lr < 0 OrElse lc < 0 Then Return

				Dim dirtyRect As Rectangle = outerInstance.table.getCellRect(lr, lc, False)
				outerInstance.table.repaint(dirtyRect)
			End Sub

			Public Overridable Sub focusGained(ByVal e As FocusEvent)
				repaintLeadCell()
			End Sub

			Public Overridable Sub focusLost(ByVal e As FocusEvent)
				repaintLeadCell()
			End Sub


			' KeyListener
			Public Overridable Sub keyPressed(ByVal e As KeyEvent)
			End Sub

			Public Overridable Sub keyReleased(ByVal e As KeyEvent)
			End Sub

			Public Overridable Sub keyTyped(ByVal e As KeyEvent)
				Dim ___keyStroke As KeyStroke = KeyStroke.getKeyStroke(e.keyChar, e.modifiers)

				' We register all actions using ANCESTOR_OF_FOCUSED_COMPONENT
				' which means that we might perform the appropriate action
				' in the table and then forward it to the editor if the editor
				' had focus. Make sure this doesn't happen by checking our
				' InputMaps.
				Dim map As InputMap = outerInstance.table.getInputMap(JComponent.WHEN_FOCUSED)
				If map IsNot Nothing AndAlso map.get(___keyStroke) IsNot Nothing Then Return
				map = outerInstance.table.getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT)
				If map IsNot Nothing AndAlso map.get(___keyStroke) IsNot Nothing Then Return

				___keyStroke = KeyStroke.getKeyStrokeForEvent(e)

				' The AWT seems to generate an unconsumed \r event when
				' ENTER (\n) is pressed.
				If e.keyChar = ControlChars.Cr Then Return

				Dim leadRow As Integer = getAdjustedLead(outerInstance.table, True)
				Dim leadColumn As Integer = getAdjustedLead(outerInstance.table, False)
				If leadRow <> -1 AndAlso leadColumn <> -1 AndAlso (Not outerInstance.table.editing) Then
					If Not outerInstance.table.editCellAt(leadRow, leadColumn) Then Return
				End If

				' Forwarding events this way seems to put the component
				' in a state where it believes it has focus. In reality
				' the table retains focus - though it is difficult for
				' a user to tell, since the caret is visible and flashing.

				' Calling table.requestFocus() here, to get the focus back to
				' the table, seems to have no effect.

				Dim editorComp As Component = outerInstance.table.editorComponent
				If outerInstance.table.editing AndAlso editorComp IsNot Nothing Then
					If TypeOf editorComp Is JComponent Then
						Dim component As JComponent = CType(editorComp, JComponent)
						map = component.getInputMap(JComponent.WHEN_FOCUSED)
						Dim binding As Object = If(map IsNot Nothing, map.get(___keyStroke), Nothing)
						If binding Is Nothing Then
							map = component.getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT)
							binding = If(map IsNot Nothing, map.get(___keyStroke), Nothing)
						End If
						If binding IsNot Nothing Then
							Dim am As ActionMap = component.actionMap
							Dim action As Action = If(am IsNot Nothing, am.get(binding), Nothing)
							If action IsNot Nothing AndAlso SwingUtilities.notifyAction(action, ___keyStroke, e, component, e.modifiers) Then e.consume()
						End If
					End If
				End If
			End Sub


			' MouseInputListener

			' Component receiving mouse events during editing.
			' May not be editorComponent.
			Private dispatchComponent As Component

			Public Overridable Sub mouseClicked(ByVal e As MouseEvent)
			End Sub

			Private Property dispatchComponent As MouseEvent
				Set(ByVal e As MouseEvent)
					Dim editorComponent As Component = outerInstance.table.editorComponent
					Dim p As Point = e.point
					Dim p2 As Point = SwingUtilities.convertPoint(outerInstance.table, p, editorComponent)
					dispatchComponent = SwingUtilities.getDeepestComponentAt(editorComponent, p2.x, p2.y)
					sun.swing.SwingUtilities2.skipClickCountunt(dispatchComponent, e.clickCount - 1)
				End Set
			End Property

			Private Function repostEvent(ByVal e As MouseEvent) As Boolean
				' Check for isEditing() in case another event has
				' caused the editor to be removed. See bug #4306499.
				If dispatchComponent Is Nothing OrElse (Not outerInstance.table.editing) Then Return False
				Dim e2 As MouseEvent = SwingUtilities.convertMouseEvent(outerInstance.table, e, dispatchComponent)
				dispatchComponent.dispatchEvent(e2)
				Return True
			End Function

			Private Property valueIsAdjusting As Boolean
				Set(ByVal flag As Boolean)
					outerInstance.table.selectionModel.valueIsAdjusting = flag
					outerInstance.table.columnModel.selectionModel.valueIsAdjusting = flag
				End Set
			End Property

			' The row and column where the press occurred and the
			' press event itself
			Private pressedRow As Integer
			Private pressedCol As Integer
			Private pressedEvent As MouseEvent

			' Whether or not the mouse press (which is being considered as part
			' of a drag sequence) also caused the selection change to be fully
			' processed.
			Private dragPressDidSelection As Boolean

			' Set to true when a drag gesture has been fully recognized and DnD
			' begins. Use this to ignore further mouse events which could be
			' delivered if DnD is cancelled (via ESCAPE for example)
			Private dragStarted As Boolean

			' Whether or not we should start the editing timer on release
			Private shouldStartTimer As Boolean

			' To cache the return value of pointOutsidePrefSize since we use
			' it multiple times.
			Private outsidePrefSize As Boolean

			' Used to delay the start of editing.
			Private ___timer As Timer = Nothing

			Private Function canStartDrag() As Boolean
				If pressedRow = -1 OrElse pressedCol = -1 Then Return False

				If outerInstance.isFileList Then Return Not outsidePrefSize

				' if this is a single selection table
				If (outerInstance.table.selectionModel.selectionMode = ListSelectionModel.SINGLE_SELECTION) AndAlso (outerInstance.table.columnModel.selectionModel.selectionMode = ListSelectionModel.SINGLE_SELECTION) Then Return True

				Return outerInstance.table.isCellSelected(pressedRow, pressedCol)
			End Function

			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				If sun.swing.SwingUtilities2.shouldIgnore(e, outerInstance.table) Then Return

				If outerInstance.table.editing AndAlso (Not outerInstance.table.cellEditor.stopCellEditing()) Then
					Dim editorComponent As Component = outerInstance.table.editorComponent
					If editorComponent IsNot Nothing AndAlso (Not editorComponent.hasFocus()) Then sun.swing.SwingUtilities2.compositeRequestFocus(editorComponent)
					Return
				End If

				Dim p As Point = e.point
				pressedRow = outerInstance.table.rowAtPoint(p)
				pressedCol = outerInstance.table.columnAtPoint(p)
				outsidePrefSize = outerInstance.pointOutsidePrefSize(pressedRow, pressedCol, p)

				If outerInstance.isFileList Then shouldStartTimer = outerInstance.table.isCellSelected(pressedRow, pressedCol) AndAlso (Not e.shiftDown) AndAlso (Not BasicGraphicsUtils.isMenuShortcutKeyDown(e)) AndAlso Not outsidePrefSize

				If outerInstance.table.dragEnabled Then
					mousePressedDND(e)
				Else
					sun.swing.SwingUtilities2.adjustFocus(outerInstance.table)
					If Not outerInstance.isFileList Then valueIsAdjusting = True
					adjustSelection(e)
				End If
			End Sub

			Private Sub mousePressedDND(ByVal e As MouseEvent)
				pressedEvent = e
				Dim grabFocus As Boolean = True
				dragStarted = False

				If canStartDrag() AndAlso DragRecognitionSupport.mousePressed(e) Then

					dragPressDidSelection = False

					If BasicGraphicsUtils.isMenuShortcutKeyDown(e) AndAlso outerInstance.isFileList Then
						' do nothing for control - will be handled on release
						' or when drag starts
						Return
					ElseIf (Not e.shiftDown) AndAlso outerInstance.table.isCellSelected(pressedRow, pressedCol) Then
						' clicking on something that's already selected
						' and need to make it the lead now
						outerInstance.table.selectionModel.addSelectionInterval(pressedRow, pressedRow)
						outerInstance.table.columnModel.selectionModel.addSelectionInterval(pressedCol, pressedCol)

						Return
					End If

					dragPressDidSelection = True

					' could be a drag initiating event - don't grab focus
					grabFocus = False
				ElseIf Not outerInstance.isFileList Then
					' When drag can't happen, mouse drags might change the selection in the table
					' so we want the isAdjusting flag to be set
					valueIsAdjusting = True
				End If

				If grabFocus Then sun.swing.SwingUtilities2.adjustFocus(outerInstance.table)

				adjustSelection(e)
			End Sub

			Private Sub adjustSelection(ByVal e As MouseEvent)
				' Fix for 4835633
				If outsidePrefSize Then
					' If shift is down in multi-select, we should just return.
					' For single select or non-shift-click, clear the selection
					If e.iD = MouseEvent.MOUSE_PRESSED AndAlso ((Not e.shiftDown) OrElse outerInstance.table.selectionModel.selectionMode = ListSelectionModel.SINGLE_SELECTION) Then
						outerInstance.table.clearSelection()
						Dim tce As TableCellEditor = outerInstance.table.cellEditor
						If tce IsNot Nothing Then tce.stopCellEditing()
					End If
					Return
				End If
				' The autoscroller can generate drag events outside the
				' table's range.
				If (pressedCol = -1) OrElse (pressedRow = -1) Then Return

				Dim dragEnabled As Boolean = outerInstance.table.dragEnabled

				If (Not dragEnabled) AndAlso (Not outerInstance.isFileList) AndAlso outerInstance.table.editCellAt(pressedRow, pressedCol, e) Then
					dispatchComponent = e
					repostEvent(e)
				End If

				Dim editor As CellEditor = outerInstance.table.cellEditor
				If dragEnabled OrElse editor Is Nothing OrElse editor.shouldSelectCell(e) Then outerInstance.table.changeSelection(pressedRow, pressedCol, BasicGraphicsUtils.isMenuShortcutKeyDown(e), e.shiftDown)
			End Sub

			Public Overridable Sub valueChanged(ByVal e As ListSelectionEvent) Implements ListSelectionListener.valueChanged
				If ___timer IsNot Nothing Then
					___timer.stop()
					___timer = Nothing
				End If
			End Sub

			Public Overridable Sub actionPerformed(ByVal ae As ActionEvent)
				outerInstance.table.editCellAt(pressedRow, pressedCol, Nothing)
				Dim editorComponent As Component = outerInstance.table.editorComponent
				If editorComponent IsNot Nothing AndAlso (Not editorComponent.hasFocus()) Then sun.swing.SwingUtilities2.compositeRequestFocus(editorComponent)
				Return
			End Sub

			Private Sub maybeStartTimer()
				If Not shouldStartTimer Then Return

				If ___timer Is Nothing Then
					___timer = New Timer(1200, Me)
					___timer.repeats = False
				End If

				___timer.start()
			End Sub

			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
				If sun.swing.SwingUtilities2.shouldIgnore(e, outerInstance.table) Then Return

				If outerInstance.table.dragEnabled Then
					mouseReleasedDND(e)
				Else
					If outerInstance.isFileList Then maybeStartTimer()
				End If

				pressedEvent = Nothing
				repostEvent(e)
				dispatchComponent = Nothing
				valueIsAdjusting = False
			End Sub

			Private Sub mouseReleasedDND(ByVal e As MouseEvent)
				Dim [me] As MouseEvent = DragRecognitionSupport.mouseReleased(e)
				If [me] IsNot Nothing Then
					sun.swing.SwingUtilities2.adjustFocus(outerInstance.table)
					If Not dragPressDidSelection Then adjustSelection([me])
				End If

				If Not dragStarted Then
					If outerInstance.isFileList Then
						maybeStartTimer()
						Return
					End If

					Dim p As Point = e.point

					If pressedEvent IsNot Nothing AndAlso outerInstance.table.rowAtPoint(p) = pressedRow AndAlso outerInstance.table.columnAtPoint(p) = pressedCol AndAlso outerInstance.table.editCellAt(pressedRow, pressedCol, pressedEvent) Then

						dispatchComponent = pressedEvent
						repostEvent(pressedEvent)

						' This may appear completely odd, but must be done for backward
						' compatibility reasons. Developers have been known to rely on
						' a call to shouldSelectCell after editing has begun.
						Dim ce As CellEditor = outerInstance.table.cellEditor
						If ce IsNot Nothing Then ce.shouldSelectCell(pressedEvent)
					End If
				End If
			End Sub

			Public Overridable Sub mouseEntered(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseExited(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseMoved(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub dragStarting(ByVal [me] As MouseEvent)
				dragStarted = True

				If BasicGraphicsUtils.isMenuShortcutKeyDown([me]) AndAlso outerInstance.isFileList Then
					outerInstance.table.selectionModel.addSelectionInterval(pressedRow, pressedRow)
					outerInstance.table.columnModel.selectionModel.addSelectionInterval(pressedCol, pressedCol)
				End If

				pressedEvent = Nothing
			End Sub

			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
				If sun.swing.SwingUtilities2.shouldIgnore(e, outerInstance.table) Then Return

				If outerInstance.table.dragEnabled AndAlso (DragRecognitionSupport.mouseDragged(e, Me) OrElse dragStarted) Then Return

				repostEvent(e)

				' Check isFileList:
				' Until we support drag-selection, dragging should not change
				' the selection (act like single-select).
				If outerInstance.isFileList OrElse outerInstance.table.editing Then Return

				Dim p As Point = e.point
				Dim row As Integer = outerInstance.table.rowAtPoint(p)
				Dim column As Integer = outerInstance.table.columnAtPoint(p)
				' The autoscroller can generate drag events outside the
				' table's range.
				If (column = -1) OrElse (row = -1) Then Return

				outerInstance.table.changeSelection(row, column, BasicGraphicsUtils.isMenuShortcutKeyDown(e), True)
			End Sub


			' PropertyChangeListener
			Public Overridable Sub propertyChange(ByVal [event] As java.beans.PropertyChangeEvent)
				Dim changeName As String = [event].propertyName

				If "componentOrientation" = changeName Then
					Dim inputMap As InputMap = outerInstance.getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT)

					SwingUtilities.replaceUIInputMap(outerInstance.table, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, inputMap)

					Dim header As JTableHeader = outerInstance.table.tableHeader
					If header IsNot Nothing Then header.componentOrientation = CType([event].newValue, ComponentOrientation)
				ElseIf "dropLocation" = changeName Then
					Dim oldValue As JTable.DropLocation = CType([event].oldValue, JTable.DropLocation)
					repaintDropLocation(oldValue)
					repaintDropLocation(outerInstance.table.dropLocation)
				ElseIf "Table.isFileList" = changeName Then
					outerInstance.isFileList = Boolean.TRUE.Equals(outerInstance.table.getClientProperty("Table.isFileList"))
					outerInstance.table.revalidate()
					outerInstance.table.repaint()
					If outerInstance.isFileList Then
						outerInstance.table.selectionModel.addListSelectionListener(outerInstance.handler)
					Else
						outerInstance.table.selectionModel.removeListSelectionListener(outerInstance.handler)
						___timer = Nothing
					End If
				ElseIf "selectionModel" = changeName Then
					If outerInstance.isFileList Then
						Dim old As ListSelectionModel = CType([event].oldValue, ListSelectionModel)
						old.removeListSelectionListener(outerInstance.handler)
						outerInstance.table.selectionModel.addListSelectionListener(outerInstance.handler)
					End If
				End If
			End Sub

			Private Sub repaintDropLocation(ByVal loc As JTable.DropLocation)
				If loc Is Nothing Then Return

				If (Not loc.insertRow) AndAlso (Not loc.insertColumn) Then
					Dim rect As Rectangle = outerInstance.table.getCellRect(loc.row, loc.column, False)
					If rect IsNot Nothing Then outerInstance.table.repaint(rect)
					Return
				End If

				If loc.insertRow Then
					Dim rect As Rectangle = outerInstance.extendRect(outerInstance.getHDropLineRect(loc), True)
					If rect IsNot Nothing Then outerInstance.table.repaint(rect)
				End If

				If loc.insertColumn Then
					Dim rect As Rectangle = outerInstance.extendRect(outerInstance.getVDropLineRect(loc), False)
					If rect IsNot Nothing Then outerInstance.table.repaint(rect)
				End If
			End Sub
		End Class


	'    
	'     * Returns true if the given point is outside the preferredSize of the
	'     * item at the given row of the table.  (Column must be 0).
	'     * Returns false if the "Table.isFileList" client property is not set.
	'     
		Private Function pointOutsidePrefSize(ByVal row As Integer, ByVal column As Integer, ByVal p As Point) As Boolean
			If Not isFileList Then Return False

			Return sun.swing.SwingUtilities2.pointOutsidePrefSize(table, row, column, p)
		End Function

	'
	'  Factory methods for the Listeners
	'

		Private Property handler As Handler
			Get
				If handler Is Nothing Then handler = New Handler(Me)
				Return handler
			End Get
		End Property

		''' <summary>
		''' Creates the key listener for handling keyboard navigation in the JTable.
		''' </summary>
		Protected Friend Overridable Function createKeyListener() As KeyListener
			Return Nothing
		End Function

		''' <summary>
		''' Creates the focus listener for handling keyboard navigation in the JTable.
		''' </summary>
		Protected Friend Overridable Function createFocusListener() As FocusListener
			Return handler
		End Function

		''' <summary>
		''' Creates the mouse listener for the JTable.
		''' </summary>
		Protected Friend Overridable Function createMouseInputListener() As MouseInputListener
			Return handler
		End Function

	'
	'  The installation/uninstall procedures and support
	'

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New BasicTableUI
		End Function

	'  Installation

		Public Overridable Sub installUI(ByVal c As JComponent)
			table = CType(c, JTable)

			rendererPane = New CellRendererPane
			table.add(rendererPane)
			installDefaults()
			installDefaults2()
			installListeners()
			installKeyboardActions()
		End Sub

		''' <summary>
		''' Initialize JTable properties, e.g. font, foreground, and background.
		''' The font, foreground, and background properties are only set if their
		''' current value is either null or a UIResource, other properties are set
		''' if the current value is null.
		''' </summary>
		''' <seealso cref= #installUI </seealso>
		Protected Friend Overridable Sub installDefaults()
			LookAndFeel.installColorsAndFont(table, "Table.background", "Table.foreground", "Table.font")
			' JTable's original row height is 16.  To correctly display the
			' contents on Linux we should have set it to 18, Windows 19 and
			' Solaris 20.  As these values vary so much it's too hard to
			' be backward compatable and try to update the row height, we're
			' therefor NOT going to adjust the row height based on font.  If the
			' developer changes the font, it's there responsability to update
			' the row height.

			LookAndFeel.installProperty(table, "opaque", Boolean.TRUE)

			Dim sbg As Color = table.selectionBackground
			If sbg Is Nothing OrElse TypeOf sbg Is UIResource Then
				sbg = UIManager.getColor("Table.selectionBackground")
				table.selectionBackground = If(sbg IsNot Nothing, sbg, UIManager.getColor("textHighlight"))
			End If

			Dim sfg As Color = table.selectionForeground
			If sfg Is Nothing OrElse TypeOf sfg Is UIResource Then
				sfg = UIManager.getColor("Table.selectionForeground")
				table.selectionForeground = If(sfg IsNot Nothing, sfg, UIManager.getColor("textHighlightText"))
			End If

			Dim gridColor As Color = table.gridColor
			If gridColor Is Nothing OrElse TypeOf gridColor Is UIResource Then
				gridColor = UIManager.getColor("Table.gridColor")
				table.gridColor = If(gridColor IsNot Nothing, gridColor, Color.GRAY)
			End If

			' install the scrollpane border
			Dim parent As Container = SwingUtilities.getUnwrappedParent(table) ' should be viewport
			If parent IsNot Nothing Then
				parent = parent.parent ' should be the scrollpane
				If parent IsNot Nothing AndAlso TypeOf parent Is JScrollPane Then LookAndFeel.installBorder(CType(parent, JScrollPane), "Table.scrollPaneBorder")
			End If

			isFileList = Boolean.TRUE.Equals(table.getClientProperty("Table.isFileList"))
		End Sub

		Private Sub installDefaults2()
			Dim th As TransferHandler = table.transferHandler
			If th Is Nothing OrElse TypeOf th Is UIResource Then
				table.transferHandler = defaultTransferHandler
				' default TransferHandler doesn't support drop
				' so we don't want drop handling
				If TypeOf table.dropTarget Is UIResource Then table.dropTarget = Nothing
			End If
		End Sub

		''' <summary>
		''' Attaches listeners to the JTable.
		''' </summary>
		Protected Friend Overridable Sub installListeners()
			focusListener = createFocusListener()
			keyListener = createKeyListener()
			mouseInputListener = createMouseInputListener()

			table.addFocusListener(focusListener)
			table.addKeyListener(keyListener)
			table.addMouseListener(mouseInputListener)
			table.addMouseMotionListener(mouseInputListener)
			table.addPropertyChangeListener(handler)
			If isFileList Then table.selectionModel.addListSelectionListener(handler)
		End Sub

		''' <summary>
		''' Register all keyboard actions on the JTable.
		''' </summary>
		Protected Friend Overridable Sub installKeyboardActions()
			LazyActionMap.installLazyActionMap(table, GetType(BasicTableUI), "Table.actionMap")

			Dim ___inputMap As InputMap = getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT)
			SwingUtilities.replaceUIInputMap(table, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, ___inputMap)
		End Sub

		Friend Overridable Function getInputMap(ByVal condition As Integer) As InputMap
			If condition = JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT Then
				Dim keyMap As InputMap = CType(sun.swing.DefaultLookup.get(table, Me, "Table.ancestorInputMap"), InputMap)
				Dim rtlKeyMap As InputMap

				rtlKeyMap = CType(sun.swing.DefaultLookup.get(table, Me, "Table.ancestorInputMap.RightToLeft"), InputMap)
				If table.componentOrientation.leftToRight OrElse (rtlKeyMap Is Nothing) Then
					Return keyMap
				Else
					rtlKeyMap.parent = keyMap
					Return rtlKeyMap
				End If
			End If
			Return Nothing
		End Function

		Friend Shared Sub loadActionMap(ByVal map As LazyActionMap)
			' IMPORTANT: There is a very close coupling between the parameters
			' passed to the Actions constructor. Only certain parameter
			' combinations are supported. For example, the following Action would
			' not work as expected:
			'     new Actions(Actions.NEXT_ROW_CELL, 1, 4, false, true)
			' Actions which move within the selection only (having a true
			' inSelection parameter) require that one of dx or dy be
			' zero and the other be -1 or 1. The point of this warning is
			' that you should be very careful about making sure a particular
			' combination of parameters is supported before changing or
			' adding anything here.

			map.put(New Actions(Actions.NEXT_COLUMN, 1, 0, False, False))
			map.put(New Actions(Actions.NEXT_COLUMN_CHANGE_LEAD, 1, 0, False, False))
			map.put(New Actions(Actions.PREVIOUS_COLUMN, -1, 0, False, False))
			map.put(New Actions(Actions.PREVIOUS_COLUMN_CHANGE_LEAD, -1, 0, False, False))
			map.put(New Actions(Actions.NEXT_ROW, 0, 1, False, False))
			map.put(New Actions(Actions.NEXT_ROW_CHANGE_LEAD, 0, 1, False, False))
			map.put(New Actions(Actions.PREVIOUS_ROW, 0, -1, False, False))
			map.put(New Actions(Actions.PREVIOUS_ROW_CHANGE_LEAD, 0, -1, False, False))
			map.put(New Actions(Actions.NEXT_COLUMN_EXTEND_SELECTION, 1, 0, True, False))
			map.put(New Actions(Actions.PREVIOUS_COLUMN_EXTEND_SELECTION, -1, 0, True, False))
			map.put(New Actions(Actions.NEXT_ROW_EXTEND_SELECTION, 0, 1, True, False))
			map.put(New Actions(Actions.PREVIOUS_ROW_EXTEND_SELECTION, 0, -1, True, False))
			map.put(New Actions(Actions.SCROLL_UP_CHANGE_SELECTION, False, False, True, False))
			map.put(New Actions(Actions.SCROLL_DOWN_CHANGE_SELECTION, False, True, True, False))
			map.put(New Actions(Actions.FIRST_COLUMN, False, False, False, True))
			map.put(New Actions(Actions.LAST_COLUMN, False, True, False, True))

			map.put(New Actions(Actions.SCROLL_UP_EXTEND_SELECTION, True, False, True, False))
			map.put(New Actions(Actions.SCROLL_DOWN_EXTEND_SELECTION, True, True, True, False))
			map.put(New Actions(Actions.FIRST_COLUMN_EXTEND_SELECTION, True, False, False, True))
			map.put(New Actions(Actions.LAST_COLUMN_EXTEND_SELECTION, True, True, False, True))

			map.put(New Actions(Actions.FIRST_ROW, False, False, True, True))
			map.put(New Actions(Actions.LAST_ROW, False, True, True, True))

			map.put(New Actions(Actions.FIRST_ROW_EXTEND_SELECTION, True, False, True, True))
			map.put(New Actions(Actions.LAST_ROW_EXTEND_SELECTION, True, True, True, True))

			map.put(New Actions(Actions.NEXT_COLUMN_CELL, 1, 0, False, True))
			map.put(New Actions(Actions.PREVIOUS_COLUMN_CELL, -1, 0, False, True))
			map.put(New Actions(Actions.NEXT_ROW_CELL, 0, 1, False, True))
			map.put(New Actions(Actions.PREVIOUS_ROW_CELL, 0, -1, False, True))

			map.put(New Actions(Actions.SELECT_ALL))
			map.put(New Actions(Actions.CLEAR_SELECTION))
			map.put(New Actions(Actions.CANCEL_EDITING))
			map.put(New Actions(Actions.START_EDITING))

			map.put(TransferHandler.cutAction.getValue(Action.NAME), TransferHandler.cutAction)
			map.put(TransferHandler.copyAction.getValue(Action.NAME), TransferHandler.copyAction)
			map.put(TransferHandler.pasteAction.getValue(Action.NAME), TransferHandler.pasteAction)

			map.put(New Actions(Actions.SCROLL_LEFT_CHANGE_SELECTION, False, False, False, False))
			map.put(New Actions(Actions.SCROLL_RIGHT_CHANGE_SELECTION, False, True, False, False))
			map.put(New Actions(Actions.SCROLL_LEFT_EXTEND_SELECTION, True, False, False, False))
			map.put(New Actions(Actions.SCROLL_RIGHT_EXTEND_SELECTION, True, True, False, False))

			map.put(New Actions(Actions.ADD_TO_SELECTION))
			map.put(New Actions(Actions.TOGGLE_AND_ANCHOR))
			map.put(New Actions(Actions.EXTEND_TO))
			map.put(New Actions(Actions.MOVE_SELECTION_TO))
			map.put(New Actions(Actions.FOCUS_HEADER))
		End Sub

	'  Uninstallation

		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			uninstallDefaults()
			uninstallListeners()
			uninstallKeyboardActions()

			table.remove(rendererPane)
			rendererPane = Nothing
			table = Nothing
		End Sub

		Protected Friend Overridable Sub uninstallDefaults()
			If TypeOf table.transferHandler Is UIResource Then table.transferHandler = Nothing
		End Sub

		Protected Friend Overridable Sub uninstallListeners()
			table.removeFocusListener(focusListener)
			table.removeKeyListener(keyListener)
			table.removeMouseListener(mouseInputListener)
			table.removeMouseMotionListener(mouseInputListener)
			table.removePropertyChangeListener(handler)
			If isFileList Then table.selectionModel.removeListSelectionListener(handler)

			focusListener = Nothing
			keyListener = Nothing
			mouseInputListener = Nothing
			handler = Nothing
		End Sub

		Protected Friend Overridable Sub uninstallKeyboardActions()
			SwingUtilities.replaceUIInputMap(table, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, Nothing)
			SwingUtilities.replaceUIActionMap(table, Nothing)
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
				Dim tcr As New DefaultTableCellRenderer
				renderer = tcr.getTableCellRendererComponent(table, "a", False, False, -1, -1)
				lafDefaults(BASELINE_COMPONENT_KEY) = renderer
			End If
			renderer.font = table.font
			Dim rowMargin As Integer = table.rowMargin
			Return renderer.getBaseline(Integer.MaxValue, table.rowHeight - rowMargin) + rowMargin \ 2
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
	' Size Methods
	'

		Private Function createTableSize(ByVal width As Long) As Dimension
			Dim height As Integer = 0
			Dim rowCount As Integer = table.rowCount
			If rowCount > 0 AndAlso table.columnCount > 0 Then
				Dim r As Rectangle = table.getCellRect(rowCount-1, 0, True)
				height = r.y + r.height
			End If
			' Width is always positive. The call to abs() is a workaround for
			' a bug in the 1.1.6 JIT on Windows.
			Dim tmp As Long = Math.Abs(width)
			If tmp > Integer.MaxValue Then tmp = Integer.MaxValue
			Return New Dimension(CInt(tmp), height)
		End Function

		''' <summary>
		''' Return the minimum size of the table. The minimum height is the
		''' row height times the number of rows.
		''' The minimum width is the sum of the minimum widths of each column.
		''' </summary>
		Public Overridable Function getMinimumSize(ByVal c As JComponent) As Dimension
			Dim width As Long = 0
			Dim enumeration As System.Collections.IEnumerator = table.columnModel.columns
			Do While enumeration.hasMoreElements()
				Dim aColumn As TableColumn = CType(enumeration.nextElement(), TableColumn)
				width = width + aColumn.minWidth
			Loop
			Return createTableSize(width)
		End Function

		''' <summary>
		''' Return the preferred size of the table. The preferred height is the
		''' row height times the number of rows.
		''' The preferred width is the sum of the preferred widths of each column.
		''' </summary>
		Public Overridable Function getPreferredSize(ByVal c As JComponent) As Dimension
			Dim width As Long = 0
			Dim enumeration As System.Collections.IEnumerator = table.columnModel.columns
			Do While enumeration.hasMoreElements()
				Dim aColumn As TableColumn = CType(enumeration.nextElement(), TableColumn)
				width = width + aColumn.preferredWidth
			Loop
			Return createTableSize(width)
		End Function

		''' <summary>
		''' Return the maximum size of the table. The maximum height is the
		''' row heighttimes the number of rows.
		''' The maximum width is the sum of the maximum widths of each column.
		''' </summary>
		Public Overridable Function getMaximumSize(ByVal c As JComponent) As Dimension
			Dim width As Long = 0
			Dim enumeration As System.Collections.IEnumerator = table.columnModel.columns
			Do While enumeration.hasMoreElements()
				Dim aColumn As TableColumn = CType(enumeration.nextElement(), TableColumn)
				width = width + aColumn.maxWidth
			Loop
			Return createTableSize(width)
		End Function

	'
	'  Paint methods and support
	'

		''' <summary>
		''' Paint a representation of the <code>table</code> instance
		''' that was set in installUI().
		''' </summary>
		Public Overridable Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			Dim clip As Rectangle = g.clipBounds

			Dim bounds As Rectangle = table.bounds
			' account for the fact that the graphics has already been translated
			' into the table's bounds
				bounds.y = 0
				bounds.x = bounds.y

			If table.rowCount <= 0 OrElse table.columnCount <= 0 OrElse (Not bounds.intersects(clip)) Then
					' this check prevents us from painting the entire table
					' when the clip doesn't intersect our bounds at all

				paintDropLines(g)
				Return
			End If

			Dim ltr As Boolean = table.componentOrientation.leftToRight

			Dim upperLeft As Point = clip.location
			Dim lowerRight As New Point(clip.x + clip.width - 1, clip.y + clip.height - 1)

			Dim rMin As Integer = table.rowAtPoint(upperLeft)
			Dim rMax As Integer = table.rowAtPoint(lowerRight)
			' This should never happen (as long as our bounds intersect the clip,
			' which is why we bail above if that is the case).
			If rMin = -1 Then rMin = 0
			' If the table does not have enough rows to fill the view we'll get -1.
			' (We could also get -1 if our bounds don't intersect the clip,
			' which is why we bail above if that is the case).
			' Replace this with the index of the last row.
			If rMax = -1 Then rMax = table.rowCount-1

			Dim cMin As Integer = table.columnAtPoint(If(ltr, upperLeft, lowerRight))
			Dim cMax As Integer = table.columnAtPoint(If(ltr, lowerRight, upperLeft))
			' This should never happen.
			If cMin = -1 Then cMin = 0
			' If the table does not have enough columns to fill the view we'll get -1.
			' Replace this with the index of the last column.
			If cMax = -1 Then cMax = table.columnCount-1

			' Paint the grid.
			paintGrid(g, rMin, rMax, cMin, cMax)

			' Paint the cells.
			paintCells(g, rMin, rMax, cMin, cMax)

			paintDropLines(g)
		End Sub

		Private Sub paintDropLines(ByVal g As Graphics)
			Dim loc As JTable.DropLocation = table.dropLocation
			If loc Is Nothing Then Return

			Dim color As Color = UIManager.getColor("Table.dropLineColor")
			Dim shortColor As Color = UIManager.getColor("Table.dropLineShortColor")
			If color Is Nothing AndAlso shortColor Is Nothing Then Return

			Dim rect As Rectangle

			rect = getHDropLineRect(loc)
			If rect IsNot Nothing Then
				Dim x As Integer = rect.x
				Dim w As Integer = rect.width
				If color IsNot Nothing Then
					extendRect(rect, True)
					g.color = color
					g.fillRect(rect.x, rect.y, rect.width, rect.height)
				End If
				If (Not loc.insertColumn) AndAlso shortColor IsNot Nothing Then
					g.color = shortColor
					g.fillRect(x, rect.y, w, rect.height)
				End If
			End If

			rect = getVDropLineRect(loc)
			If rect IsNot Nothing Then
				Dim y As Integer = rect.y
				Dim h As Integer = rect.height
				If color IsNot Nothing Then
					extendRect(rect, False)
					g.color = color
					g.fillRect(rect.x, rect.y, rect.width, rect.height)
				End If
				If (Not loc.insertRow) AndAlso shortColor IsNot Nothing Then
					g.color = shortColor
					g.fillRect(rect.x, y, rect.width, h)
				End If
			End If
		End Sub

		Private Function getHDropLineRect(ByVal loc As JTable.DropLocation) As Rectangle
			If Not loc.insertRow Then Return Nothing

			Dim row As Integer = loc.row
			Dim col As Integer = loc.column
			If col >= table.columnCount Then col -= 1

			Dim rect As Rectangle = table.getCellRect(row, col, True)

			If row >= table.rowCount Then
				row -= 1
				Dim prevRect As Rectangle = table.getCellRect(row, col, True)
				rect.y = prevRect.y + prevRect.height
			End If

			If rect.y = 0 Then
				rect.y = -1
			Else
				rect.y -= 2
			End If

			rect.height = 3

			Return rect
		End Function

		Private Function getVDropLineRect(ByVal loc As JTable.DropLocation) As Rectangle
			If Not loc.insertColumn Then Return Nothing

			Dim ltr As Boolean = table.componentOrientation.leftToRight
			Dim col As Integer = loc.column
			Dim rect As Rectangle = table.getCellRect(loc.row, col, True)

			If col >= table.columnCount Then
				col -= 1
				rect = table.getCellRect(loc.row, col, True)
				If ltr Then rect.x = rect.x + rect.width
			ElseIf Not ltr Then
				rect.x = rect.x + rect.width
			End If

			If rect.x = 0 Then
				rect.x = -1
			Else
				rect.x -= 2
			End If

			rect.width = 3

			Return rect
		End Function

		Private Function extendRect(ByVal rect As Rectangle, ByVal horizontal As Boolean) As Rectangle
			If rect Is Nothing Then Return rect

			If horizontal Then
				rect.x = 0
				rect.width = table.width
			Else
				rect.y = 0

				If table.rowCount <> 0 Then
					Dim lastRect As Rectangle = table.getCellRect(table.rowCount - 1, 0, True)
					rect.height = lastRect.y + lastRect.height
				Else
					rect.height = table.height
				End If
			End If

			Return rect
		End Function

	'    
	'     * Paints the grid lines within <I>aRect</I>, using the grid
	'     * color set with <I>setGridColor</I>. Paints vertical lines
	'     * if <code>getShowVerticalLines()</code> returns true and paints
	'     * horizontal lines if <code>getShowHorizontalLines()</code>
	'     * returns true.
	'     
		Private Sub paintGrid(ByVal g As Graphics, ByVal rMin As Integer, ByVal rMax As Integer, ByVal cMin As Integer, ByVal cMax As Integer)
			g.color = table.gridColor

			Dim minCell As Rectangle = table.getCellRect(rMin, cMin, True)
			Dim maxCell As Rectangle = table.getCellRect(rMax, cMax, True)
			Dim damagedArea As Rectangle = minCell.union(maxCell)

			If table.showHorizontalLines Then
				Dim tableWidth As Integer = damagedArea.x + damagedArea.width
				Dim y As Integer = damagedArea.y
				For row As Integer = rMin To rMax
					y += table.getRowHeight(row)
					g.drawLine(damagedArea.x, y - 1, tableWidth - 1, y - 1)
				Next row
			End If
			If table.showVerticalLines Then
				Dim cm As TableColumnModel = table.columnModel
				Dim tableHeight As Integer = damagedArea.y + damagedArea.height
				Dim x As Integer
				If table.componentOrientation.leftToRight Then
					x = damagedArea.x
					For column As Integer = cMin To cMax
						Dim w As Integer = cm.getColumn(column).width
						x += w
						g.drawLine(x - 1, 0, x - 1, tableHeight - 1)
					Next column
				Else
					x = damagedArea.x
					For column As Integer = cMax To cMin Step -1
						Dim w As Integer = cm.getColumn(column).width
						x += w
						g.drawLine(x - 1, 0, x - 1, tableHeight - 1)
					Next column
				End If
			End If
		End Sub

		Private Function viewIndexForColumn(ByVal aColumn As TableColumn) As Integer
			Dim cm As TableColumnModel = table.columnModel
			For column As Integer = 0 To cm.columnCount - 1
				If cm.getColumn(column) Is aColumn Then Return column
			Next column
			Return -1
		End Function

		Private Sub paintCells(ByVal g As Graphics, ByVal rMin As Integer, ByVal rMax As Integer, ByVal cMin As Integer, ByVal cMax As Integer)
			Dim header As JTableHeader = table.tableHeader
			Dim draggedColumn As TableColumn = If(header Is Nothing, Nothing, header.draggedColumn)

			Dim cm As TableColumnModel = table.columnModel
			Dim columnMargin As Integer = cm.columnMargin

			Dim cellRect As Rectangle
			Dim aColumn As TableColumn
			Dim columnWidth As Integer
			If table.componentOrientation.leftToRight Then
				For row As Integer = rMin To rMax
					cellRect = table.getCellRect(row, cMin, False)
					For column As Integer = cMin To cMax
						aColumn = cm.getColumn(column)
						columnWidth = aColumn.width
						cellRect.width = columnWidth - columnMargin
						If aColumn IsNot draggedColumn Then paintCell(g, cellRect, row, column)
						cellRect.x += columnWidth
					Next column
				Next row
			Else
				For row As Integer = rMin To rMax
					cellRect = table.getCellRect(row, cMin, False)
					aColumn = cm.getColumn(cMin)
					If aColumn IsNot draggedColumn Then
						columnWidth = aColumn.width
						cellRect.width = columnWidth - columnMargin
						paintCell(g, cellRect, row, cMin)
					End If
					For column As Integer = cMin+1 To cMax
						aColumn = cm.getColumn(column)
						columnWidth = aColumn.width
						cellRect.width = columnWidth - columnMargin
						cellRect.x -= columnWidth
						If aColumn IsNot draggedColumn Then paintCell(g, cellRect, row, column)
					Next column
				Next row
			End If

			' Paint the dragged column if we are dragging.
			If draggedColumn IsNot Nothing Then paintDraggedArea(g, rMin, rMax, draggedColumn, header.draggedDistance)

			' Remove any renderers that may be left in the rendererPane.
			rendererPane.removeAll()
		End Sub

		Private Sub paintDraggedArea(ByVal g As Graphics, ByVal rMin As Integer, ByVal rMax As Integer, ByVal draggedColumn As TableColumn, ByVal distance As Integer)
			Dim draggedColumnIndex As Integer = viewIndexForColumn(draggedColumn)

			Dim minCell As Rectangle = table.getCellRect(rMin, draggedColumnIndex, True)
			Dim maxCell As Rectangle = table.getCellRect(rMax, draggedColumnIndex, True)

			Dim vacatedColumnRect As Rectangle = minCell.union(maxCell)

			' Paint a gray well in place of the moving column.
			g.color = table.parent.background
			g.fillRect(vacatedColumnRect.x, vacatedColumnRect.y, vacatedColumnRect.width, vacatedColumnRect.height)

			' Move to the where the cell has been dragged.
			vacatedColumnRect.x += distance

			' Fill the background.
			g.color = table.background
			g.fillRect(vacatedColumnRect.x, vacatedColumnRect.y, vacatedColumnRect.width, vacatedColumnRect.height)

			' Paint the vertical grid lines if necessary.
			If table.showVerticalLines Then
				g.color = table.gridColor
				Dim x1 As Integer = vacatedColumnRect.x
				Dim y1 As Integer = vacatedColumnRect.y
				Dim x2 As Integer = x1 + vacatedColumnRect.width - 1
				Dim y2 As Integer = y1 + vacatedColumnRect.height - 1
				' Left
				g.drawLine(x1-1, y1, x1-1, y2)
				' Right
				g.drawLine(x2, y1, x2, y2)
			End If

			For row As Integer = rMin To rMax
				' Render the cell value
				Dim r As Rectangle = table.getCellRect(row, draggedColumnIndex, False)
				r.x += distance
				paintCell(g, r, row, draggedColumnIndex)

				' Paint the (lower) horizontal grid line if necessary.
				If table.showHorizontalLines Then
					g.color = table.gridColor
					Dim rcr As Rectangle = table.getCellRect(row, draggedColumnIndex, True)
					rcr.x += distance
					Dim x1 As Integer = rcr.x
					Dim y1 As Integer = rcr.y
					Dim x2 As Integer = x1 + rcr.width - 1
					Dim y2 As Integer = y1 + rcr.height - 1
					g.drawLine(x1, y2, x2, y2)
				End If
			Next row
		End Sub

		Private Sub paintCell(ByVal g As Graphics, ByVal cellRect As Rectangle, ByVal row As Integer, ByVal column As Integer)
			If table.editing AndAlso table.editingRow=row AndAlso table.editingColumn=column Then
				Dim component As Component = table.editorComponent
				component.bounds = cellRect
				component.validate()
			Else
				Dim renderer As TableCellRenderer = table.getCellRenderer(row, column)
				Dim component As Component = table.prepareRenderer(renderer, row, column)
				rendererPane.paintComponent(g, component, table, cellRect.x, cellRect.y, cellRect.width, cellRect.height, True)
			End If
		End Sub

		Private Shared Function getAdjustedLead(ByVal table As JTable, ByVal row As Boolean, ByVal model As ListSelectionModel) As Integer

			Dim index As Integer = model.leadSelectionIndex
			Dim compare As Integer = If(row, table.rowCount, table.columnCount)
			Return If(index < compare, index, -1)
		End Function

		Private Shared Function getAdjustedLead(ByVal table As JTable, ByVal row As Boolean) As Integer
			Return If(row, getAdjustedLead(table, row, table.selectionModel), getAdjustedLead(table, row, table.columnModel.selectionModel))
		End Function


		Private Shared ReadOnly defaultTransferHandler As TransferHandler = New TableTransferHandler

		Friend Class TableTransferHandler
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
			Protected Friend Overrides Function createTransferable(ByVal c As JComponent) As Transferable
				If TypeOf c Is JTable Then
					Dim table As JTable = CType(c, JTable)
					Dim rows As Integer()
					Dim cols As Integer()

					If (Not table.rowSelectionAllowed) AndAlso (Not table.columnSelectionAllowed) Then Return Nothing

					If Not table.rowSelectionAllowed Then
						Dim rowCount As Integer = table.rowCount

						rows = New Integer(rowCount - 1){}
						For counter As Integer = 0 To rowCount - 1
							rows(counter) = counter
						Next counter
					Else
						rows = table.selectedRows
					End If

					If Not table.columnSelectionAllowed Then
						Dim colCount As Integer = table.columnCount

						cols = New Integer(colCount - 1){}
						For counter As Integer = 0 To colCount - 1
							cols(counter) = counter
						Next counter
					Else
						cols = table.selectedColumns
					End If

					If rows Is Nothing OrElse cols Is Nothing OrElse rows.Length = 0 OrElse cols.Length = 0 Then Return Nothing

					Dim plainBuf As New StringBuilder
					Dim htmlBuf As New StringBuilder

					htmlBuf.Append("<html>" & vbLf & "<body>" & vbLf & "<table>" & vbLf)

					For row As Integer = 0 To rows.Length - 1
						htmlBuf.Append("<tr>" & vbLf)
						For col As Integer = 0 To cols.Length - 1
							Dim obj As Object = table.getValueAt(rows(row), cols(col))
							Dim val As String = (If(obj Is Nothing, "", obj.ToString()))
							plainBuf.Append(val & vbTab)
							htmlBuf.Append("  <td>" & val & "</td>" & vbLf)
						Next col
						' we want a newline at the end of each line and not a tab
						plainBuf.Remove(plainBuf.Length - 1, 1).append(vbLf)
						htmlBuf.Append("</tr>" & vbLf)
					Next row

					' remove the last newline
					plainBuf.Remove(plainBuf.Length - 1, 1)
					htmlBuf.Append("</table>" & vbLf & "</body>" & vbLf & "</html>")

					Return New BasicTransferable(plainBuf.ToString(), htmlBuf.ToString())
				End If

				Return Nothing
			End Function

			Public Overrides Function getSourceActions(ByVal c As JComponent) As Integer
				Return COPY
			End Function

		End Class
	End Class ' End of Class BasicTableUI

End Namespace
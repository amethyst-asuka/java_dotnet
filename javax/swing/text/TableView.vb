Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic

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
Namespace javax.swing.text



	''' <summary>
	''' <p>
	''' Implements View interface for a table, that is composed of an
	''' element structure where the child elements of the element
	''' this view is responsible for represent rows and the child
	''' elements of the row elements are cells.  The cell elements can
	''' have an arbitrary element structure under them, which will
	''' be built with the ViewFactory returned by the getViewFactory
	''' method.
	''' <pre>
	''' 
	''' &nbsp;  TABLE
	''' &nbsp;    ROW
	''' &nbsp;      CELL
	''' &nbsp;      CELL
	''' &nbsp;    ROW
	''' &nbsp;      CELL
	''' &nbsp;      CELL
	''' 
	''' </pre>
	''' <p>
	''' This is implemented as a hierarchy of boxes, the table itself
	''' is a vertical box, the rows are horizontal boxes, and the cells
	''' are vertical boxes.  The cells are allowed to span multiple
	''' columns and rows.  By default, the table can be thought of as
	''' being formed over a grid (i.e. somewhat like one would find in
	''' gridbag layout), where table cells can request to span more
	''' than one grid cell.  The default horizontal span of table cells
	''' will be based upon this grid, but can be changed by reimplementing
	''' the requested span of the cell (i.e. table cells can have independant
	''' spans if desired).
	''' 
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref=     View </seealso>
	Public MustInherit Class TableView
		Inherits BoxView

		''' <summary>
		''' Constructs a TableView for the given element.
		''' </summary>
		''' <param name="elem"> the element that this view is responsible for </param>
		Public Sub New(ByVal elem As Element)
			MyBase.New(elem, View.Y_AXIS)
			rows = New List(Of TableRow)
			gridValid = False
		End Sub

		''' <summary>
		''' Creates a new table row.
		''' </summary>
		''' <param name="elem"> an element </param>
		''' <returns> the row </returns>
		Protected Friend Overridable Function createTableRow(ByVal elem As Element) As TableRow
			Return New TableRow(Me, elem)
		End Function

		''' @deprecated Table cells can now be any arbitrary
		''' View implementation and should be produced by the
		''' ViewFactory rather than the table.
		''' 
		''' <param name="elem"> an element </param>
		''' <returns> the cell </returns>
		<Obsolete("Table cells can now be any arbitrary")> _
		Protected Friend Overridable Function createTableCell(ByVal elem As Element) As TableCell
			Return New TableCell(Me, elem)
		End Function

		''' <summary>
		''' The number of columns in the table.
		''' </summary>
		Friend Overridable Property columnCount As Integer
			Get
				Return columnSpans.Length
			End Get
		End Property

		''' <summary>
		''' Fetches the span (width) of the given column.
		''' This is used by the nested cells to query the
		''' sizes of grid locations outside of themselves.
		''' </summary>
		Friend Overridable Function getColumnSpan(ByVal col As Integer) As Integer
			Return columnSpans(col)
		End Function

		''' <summary>
		''' The number of rows in the table.
		''' </summary>
		Friend Overridable Property rowCount As Integer
			Get
				Return rows.Count
			End Get
		End Property

		''' <summary>
		''' Fetches the span (height) of the given row.
		''' </summary>
		Friend Overridable Function getRowSpan(ByVal row As Integer) As Integer
			Dim rv As View = getRow(row)
			If rv IsNot Nothing Then Return CInt(Fix(rv.getPreferredSpan(Y_AXIS)))
			Return 0
		End Function

		Friend Overridable Function getRow(ByVal row As Integer) As TableRow
			If row < rows.Count Then Return rows(row)
			Return Nothing
		End Function

		''' <summary>
		''' Determines the number of columns occupied by
		''' the table cell represented by given element.
		''' </summary>
		'protected
	 Friend Overridable Function getColumnsOccupied(ByVal v As View) As Integer
			' PENDING(prinz) this code should be in the html
			' paragraph, but we can't add api to enable it.
			Dim a As AttributeSet = v.element.attributes
			Dim s As String = CStr(a.getAttribute(javax.swing.text.html.HTML.Attribute.COLSPAN))
			If s IsNot Nothing Then
				Try
					Return Convert.ToInt32(s)
				Catch nfe As NumberFormatException
					' fall through to one column
				End Try
			End If

			Return 1
	 End Function

		''' <summary>
		''' Determines the number of rows occupied by
		''' the table cell represented by given element.
		''' </summary>
		'protected
	 Friend Overridable Function getRowsOccupied(ByVal v As View) As Integer
			' PENDING(prinz) this code should be in the html
			' paragraph, but we can't add api to enable it.
			Dim a As AttributeSet = v.element.attributes
			Dim s As String = CStr(a.getAttribute(javax.swing.text.html.HTML.Attribute.ROWSPAN))
			If s IsNot Nothing Then
				Try
					Return Convert.ToInt32(s)
				Catch nfe As NumberFormatException
					' fall through to one row
				End Try
			End If

			Return 1
	 End Function

		'protected
	 Friend Overridable Sub invalidateGrid()
			gridValid = False
	 End Sub

		Protected Friend Overrides Sub forwardUpdate(ByVal ec As javax.swing.event.DocumentEvent.ElementChange, ByVal e As javax.swing.event.DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			MyBase.forwardUpdate(ec, e, a, f)
			' A change in any of the table cells usually effects the whole table,
			' so redraw it all!
			If a IsNot Nothing Then
				Dim c As Component = container
				If c IsNot Nothing Then
					Dim alloc As Rectangle = If(TypeOf a Is Rectangle, CType(a, Rectangle), a.bounds)
					c.repaint(alloc.x, alloc.y, alloc.width, alloc.height)
				End If
			End If
		End Sub

		''' <summary>
		''' Change the child views.  This is implemented to
		''' provide the superclass behavior and invalidate the
		''' grid so that rows and columns will be recalculated.
		''' </summary>
		Public Overrides Sub replace(ByVal offset As Integer, ByVal length As Integer, ByVal views As View())
			MyBase.replace(offset, length, views)
			invalidateGrid()
		End Sub

		''' <summary>
		''' Fill in the grid locations that are placeholders
		''' for multi-column, multi-row, and missing grid
		''' locations.
		''' </summary>
		Friend Overridable Sub updateGrid()
			If Not gridValid Then
				' determine which views are table rows and clear out
				' grid points marked filled.
				rows.Clear()
				Dim n As Integer = viewCount
				For i As Integer = 0 To n - 1
					Dim v As View = getView(i)
					If TypeOf v Is TableRow Then
						rows.Add(CType(v, TableRow))
						Dim rv As TableRow = CType(v, TableRow)
						rv.clearFilledColumns()
						rv.row = i
					End If
				Next i

				Dim maxColumns As Integer = 0
				Dim nrows As Integer = rows.Count
				For ___row As Integer = 0 To nrows - 1
					Dim rv As TableRow = getRow(___row)
					Dim col As Integer = 0
					Dim cell As Integer = 0
					Do While cell < rv.viewCount
						Dim cv As View = rv.getView(cell)
						' advance to a free column
						Do While rv.isFilled(col)

							col += 1
						Loop
						Dim ___rowSpan As Integer = getRowsOccupied(cv)
						Dim colSpan As Integer = getColumnsOccupied(cv)
						If (colSpan > 1) OrElse (___rowSpan > 1) Then
							' fill in the overflow entries for this cell
							Dim rowLimit As Integer = ___row + ___rowSpan
							Dim colLimit As Integer = col + colSpan
							For i As Integer = row To rowLimit - 1
								For j As Integer = col To colLimit - 1
									If i <> ___row OrElse j <> col Then addFill(i, j)
								Next j
							Next i
							If colSpan > 1 Then col += colSpan - 1
						End If
						cell += 1
						col += 1
					Loop
					maxColumns = Math.Max(maxColumns, col)
				Next ___row

				' setup the column layout/requirements
				columnSpans = New Integer(maxColumns - 1){}
				columnOffsets = New Integer(maxColumns - 1){}
				columnRequirements = New javax.swing.SizeRequirements(maxColumns - 1){}
				For i As Integer = 0 To maxColumns - 1
					columnRequirements(i) = New javax.swing.SizeRequirements
				Next i
				gridValid = True
			End If
		End Sub

		''' <summary>
		''' Mark a grid location as filled in for a cells overflow.
		''' </summary>
		Friend Overridable Sub addFill(ByVal row As Integer, ByVal col As Integer)
			Dim rv As TableRow = getRow(row)
			If rv IsNot Nothing Then rv.fillColumn(col)
		End Sub

		''' <summary>
		''' Lays out the columns to fit within the given target span.
		''' Returns the results through {@code offsets} and {@code spans}.
		''' </summary>
		''' <param name="targetSpan"> the given span for total of all the table
		'''  columns </param>
		''' <param name="reqs"> the requirements desired for each column.  This
		'''  is the column maximum of the cells minimum, preferred, and
		'''  maximum requested span </param>
		''' <param name="spans"> the return value of how much to allocated to
		'''  each column </param>
		''' <param name="offsets"> the return value of the offset from the
		'''  origin for each column </param>
		Protected Friend Overridable Sub layoutColumns(ByVal targetSpan As Integer, ByVal offsets As Integer(), ByVal spans As Integer(), ByVal reqs As javax.swing.SizeRequirements())
			' allocate using the convenience method on SizeRequirements
			javax.swing.SizeRequirements.calculateTiledPositions(targetSpan, Nothing, reqs, offsets, spans)
		End Sub

		''' <summary>
		''' Perform layout for the minor axis of the box (i.e. the
		''' axis orthogonal to the axis that it represents).  The results
		''' of the layout should be placed in the given arrays which represent
		''' the allocations to the children along the minor axis.  This
		''' is called by the superclass whenever the layout needs to be
		''' updated along the minor axis.
		''' <p>
		''' This is implemented to call the
		''' <seealso cref="#layoutColumns layoutColumns"/> method, and then
		''' forward to the superclass to actually carry out the layout
		''' of the tables rows.
		''' </summary>
		''' <param name="targetSpan"> the total span given to the view, which
		'''  would be used to layout the children. </param>
		''' <param name="axis"> the axis being layed out. </param>
		''' <param name="offsets"> the offsets from the origin of the view for
		'''  each of the child views.  This is a return value and is
		'''  filled in by the implementation of this method. </param>
		''' <param name="spans"> the span of each child view.  This is a return
		'''  value and is filled in by the implementation of this method. </param>
		Protected Friend Overrides Sub layoutMinorAxis(ByVal targetSpan As Integer, ByVal axis As Integer, ByVal offsets As Integer(), ByVal spans As Integer())
			' make grid is properly represented
			updateGrid()

			' all of the row layouts are invalid, so mark them that way
			Dim n As Integer = rowCount
			For i As Integer = 0 To n - 1
				Dim ___row As TableRow = getRow(i)
				___row.layoutChanged(axis)
			Next i

			' calculate column spans
			layoutColumns(targetSpan, columnOffsets, columnSpans, columnRequirements)

			' continue normal layout
			MyBase.layoutMinorAxis(targetSpan, axis, offsets, spans)
		End Sub

		''' <summary>
		''' Calculate the requirements for the minor axis.  This is called by
		''' the superclass whenever the requirements need to be updated (i.e.
		''' a preferenceChanged was messaged through this view).
		''' <p>
		''' This is implemented to calculate the requirements as the sum of the
		''' requirements of the columns.
		''' </summary>
		Protected Friend Overrides Function calculateMinorAxisRequirements(ByVal axis As Integer, ByVal r As javax.swing.SizeRequirements) As javax.swing.SizeRequirements
			updateGrid()

			' calculate column requirements for each column
			calculateColumnRequirements(axis)


			' the requirements are the sum of the columns.
			If r Is Nothing Then r = New javax.swing.SizeRequirements
			Dim min As Long = 0
			Dim pref As Long = 0
			Dim max As Long = 0
			For Each req As javax.swing.SizeRequirements In columnRequirements
				min += req.minimum
				pref += req.preferred
				max += req.maximum
			Next req
			r.minimum = CInt(min)
			r.preferred = CInt(pref)
			r.maximum = CInt(max)
			r.alignment = 0
			Return r
		End Function

	'    
	'    boolean shouldTrace() {
	'        AttributeSet a = getElement().getAttributes();
	'        Object o = a.getAttribute(HTML.Attribute.ID);
	'        if ((o != null) && o.equals("debug")) {
	'            return true;
	'        }
	'        return false;
	'    }
	'    

		''' <summary>
		''' Calculate the requirements for each column.  The calculation
		''' is done as two passes over the table.  The table cells that
		''' occupy a single column are scanned first to determine the
		''' maximum of minimum, preferred, and maximum spans along the
		''' give axis.  Table cells that span multiple columns are excluded
		''' from the first pass.  A second pass is made to determine if
		''' the cells that span multiple columns are satisfied.  If the
		''' column requirements are not satisified, the needs of the
		''' multi-column cell is mixed into the existing column requirements.
		''' The calculation of the multi-column distribution is based upon
		''' the proportions of the existing column requirements and taking
		''' into consideration any constraining maximums.
		''' </summary>
		Friend Overridable Sub calculateColumnRequirements(ByVal axis As Integer)
			' pass 1 - single column cells
			Dim hasMultiColumn As Boolean = False
			Dim nrows As Integer = rowCount
			For i As Integer = 0 To nrows - 1
				Dim ___row As TableRow = getRow(i)
				Dim col As Integer = 0
				Dim ncells As Integer = ___row.viewCount
				Dim cell As Integer = 0
				Do While cell < ncells
					Dim cv As View = ___row.getView(cell)
					Do While ___row.isFilled(col) ' advance to a free column

						col += 1
					Loop
					Dim ___rowSpan As Integer = getRowsOccupied(cv)
					Dim colSpan As Integer = getColumnsOccupied(cv)
					If colSpan = 1 Then
						checkSingleColumnCell(axis, col, cv)
					Else
						hasMultiColumn = True
						col += colSpan - 1
					End If
					cell += 1
					col += 1
				Loop
			Next i

			' pass 2 - multi-column cells
			If hasMultiColumn Then
				For i As Integer = 0 To nrows - 1
					Dim ___row As TableRow = getRow(i)
					Dim col As Integer = 0
					Dim ncells As Integer = ___row.viewCount
					Dim cell As Integer = 0
					Do While cell < ncells
						Dim cv As View = ___row.getView(cell)
						Do While ___row.isFilled(col) ' advance to a free column

							col += 1
						Loop
						Dim colSpan As Integer = getColumnsOccupied(cv)
						If colSpan > 1 Then
							checkMultiColumnCell(axis, col, colSpan, cv)
							col += colSpan - 1
						End If
						cell += 1
						col += 1
					Loop
				Next i
			End If

	'        
	'        if (shouldTrace()) {
	'            System.err.println("calc:");
	'            for (int i = 0; i < columnRequirements.length; i++) {
	'                System.err.println(" " + i + ": " + columnRequirements[i]);
	'            }
	'        }
	'        
		End Sub

		''' <summary>
		''' check the requirements of a table cell that spans a single column.
		''' </summary>
		Friend Overridable Sub checkSingleColumnCell(ByVal axis As Integer, ByVal col As Integer, ByVal v As View)
			Dim req As javax.swing.SizeRequirements = columnRequirements(col)
			req.minimum = Math.Max(CInt(Fix(v.getMinimumSpan(axis))), req.minimum)
			req.preferred = Math.Max(CInt(Fix(v.getPreferredSpan(axis))), req.preferred)
			req.maximum = Math.Max(CInt(Fix(v.getMaximumSpan(axis))), req.maximum)
		End Sub

		''' <summary>
		''' check the requirements of a table cell that spans multiple
		''' columns.
		''' </summary>
		Friend Overridable Sub checkMultiColumnCell(ByVal axis As Integer, ByVal col As Integer, ByVal ncols As Integer, ByVal v As View)
			' calculate the totals
			Dim min As Long = 0
			Dim pref As Long = 0
			Dim max As Long = 0
			For i As Integer = 0 To ncols - 1
				Dim req As javax.swing.SizeRequirements = columnRequirements(col + i)
				min += req.minimum
				pref += req.preferred
				max += req.maximum
			Next i

			' check if the minimum size needs adjustment.
			Dim cmin As Integer = CInt(Fix(v.getMinimumSpan(axis)))
			If cmin > min Then
	'            
	'             * the columns that this cell spans need adjustment to fit
	'             * this table cell.... calculate the adjustments.  The
	'             * maximum for each cell is the maximum of the existing
	'             * maximum or the amount needed by the cell.
	'             
				Dim reqs As javax.swing.SizeRequirements() = New javax.swing.SizeRequirements(ncols - 1){}
				For i As Integer = 0 To ncols - 1
						reqs(i) = columnRequirements(col + i)
						Dim r As javax.swing.SizeRequirements = reqs(i)
					r.maximum = Math.Max(r.maximum, CInt(Fix(v.getMaximumSpan(axis))))
				Next i
				Dim spans As Integer() = New Integer(ncols - 1){}
				Dim offsets As Integer() = New Integer(ncols - 1){}
				javax.swing.SizeRequirements.calculateTiledPositions(cmin, Nothing, reqs, offsets, spans)
				' apply the adjustments
				For i As Integer = 0 To ncols - 1
					Dim req As javax.swing.SizeRequirements = reqs(i)
					req.minimum = Math.Max(spans(i), req.minimum)
					req.preferred = Math.Max(req.minimum, req.preferred)
					req.maximum = Math.Max(req.preferred, req.maximum)
				Next i
			End If

			' check if the preferred size needs adjustment.
			Dim cpref As Integer = CInt(Fix(v.getPreferredSpan(axis)))
			If cpref > pref Then
	'            
	'             * the columns that this cell spans need adjustment to fit
	'             * this table cell.... calculate the adjustments.  The
	'             * maximum for each cell is the maximum of the existing
	'             * maximum or the amount needed by the cell.
	'             
				Dim reqs As javax.swing.SizeRequirements() = New javax.swing.SizeRequirements(ncols - 1){}
				For i As Integer = 0 To ncols - 1
						reqs(i) = columnRequirements(col + i)
						Dim r As javax.swing.SizeRequirements = reqs(i)
				Next i
				Dim spans As Integer() = New Integer(ncols - 1){}
				Dim offsets As Integer() = New Integer(ncols - 1){}
				javax.swing.SizeRequirements.calculateTiledPositions(cpref, Nothing, reqs, offsets, spans)
				' apply the adjustments
				For i As Integer = 0 To ncols - 1
					Dim req As javax.swing.SizeRequirements = reqs(i)
					req.preferred = Math.Max(spans(i), req.preferred)
					req.maximum = Math.Max(req.preferred, req.maximum)
				Next i
			End If

		End Sub

		''' <summary>
		''' Fetches the child view that represents the given position in
		''' the model.  This is implemented to walk through the children
		''' looking for a range that contains the given position.  In this
		''' view the children do not necessarily have a one to one mapping
		''' with the child elements.
		''' </summary>
		''' <param name="pos">  the search position &gt;= 0 </param>
		''' <param name="a">  the allocation to the table on entry, and the
		'''   allocation of the view containing the position on exit </param>
		''' <returns>  the view representing the given position, or
		'''   <code>null</code> if there isn't one </returns>
		Protected Friend Overrides Function getViewAtPosition(ByVal pos As Integer, ByVal a As Rectangle) As View
			Dim n As Integer = viewCount
			For i As Integer = 0 To n - 1
				Dim v As View = getView(i)
				Dim p0 As Integer = v.startOffset
				Dim p1 As Integer = v.endOffset
				If (pos >= p0) AndAlso (pos < p1) Then
					' it's in this view.
					If a IsNot Nothing Then childAllocation(i, a)
					Return v
				End If
			Next i
			If pos = endOffset Then
				Dim v As View = getView(n - 1)
				If a IsNot Nothing Then Me.childAllocation(n - 1, a)
				Return v
			End If
			Return Nothing
		End Function

		' ---- variables ----------------------------------------------------

		Friend columnSpans As Integer()
		Friend columnOffsets As Integer()
		Friend columnRequirements As javax.swing.SizeRequirements()
		Friend rows As List(Of TableRow)
		Friend gridValid As Boolean
		Private Shared ReadOnly EMPTY As New BitArray

		''' <summary>
		''' View of a row in a row-centric table.
		''' </summary>
		Public Class TableRow
			Inherits BoxView

			Private ReadOnly outerInstance As TableView


			''' <summary>
			''' Constructs a TableView for the given element.
			''' </summary>
			''' <param name="elem"> the element that this view is responsible for
			''' @since 1.4 </param>
			Public Sub New(ByVal outerInstance As TableView, ByVal elem As Element)
					Me.outerInstance = outerInstance
				MyBase.New(elem, View.X_AXIS)
				fillColumns = New BitArray
			End Sub

			Friend Overridable Sub clearFilledColumns()
				fillColumns = fillColumns.And(EMPTY)
			End Sub

			Friend Overridable Sub fillColumn(ByVal col As Integer)
				fillColumns.Set(col, True)
			End Sub

			Friend Overridable Function isFilled(ByVal col As Integer) As Boolean
				Return fillColumns.Get(col)
			End Function

			''' <summary>
			''' get location in the overall set of rows </summary>
			Friend Overridable Property row As Integer
				Get
					Return row
				End Get
				Set(ByVal row As Integer)
					Me.row = row
				End Set
			End Property


			''' <summary>
			''' The number of columns present in this row.
			''' </summary>
			Friend Overridable Property columnCount As Integer
				Get
					Dim nfill As Integer = 0
					Dim n As Integer = fillColumns.Count
					For i As Integer = 0 To n - 1
						If fillColumns.Get(i) Then nfill += 1
					Next i
					Return viewCount + nfill
				End Get
			End Property

			''' <summary>
			''' Change the child views.  This is implemented to
			''' provide the superclass behavior and invalidate the
			''' grid so that rows and columns will be recalculated.
			''' </summary>
			Public Overrides Sub replace(ByVal offset As Integer, ByVal length As Integer, ByVal views As View())
				MyBase.replace(offset, length, views)
				outerInstance.invalidateGrid()
			End Sub

			''' <summary>
			''' Perform layout for the major axis of the box (i.e. the
			''' axis that it represents).  The results of the layout should
			''' be placed in the given arrays which represent the allocations
			''' to the children along the major axis.
			''' <p>
			''' This is re-implemented to give each child the span of the column
			''' width for the table, and to give cells that span multiple columns
			''' the multi-column span.
			''' </summary>
			''' <param name="targetSpan"> the total span given to the view, which
			'''  would be used to layout the children. </param>
			''' <param name="axis"> the axis being layed out. </param>
			''' <param name="offsets"> the offsets from the origin of the view for
			'''  each of the child views.  This is a return value and is
			'''  filled in by the implementation of this method. </param>
			''' <param name="spans"> the span of each child view.  This is a return
			'''  value and is filled in by the implementation of this method. </param>
			Protected Friend Overrides Sub layoutMajorAxis(ByVal targetSpan As Integer, ByVal axis As Integer, ByVal offsets As Integer(), ByVal spans As Integer())
				Dim col As Integer = 0
				Dim ncells As Integer = viewCount
				Dim cell As Integer = 0
				Do While cell < ncells
					Dim cv As View = getView(cell)
					Do While isFilled(col) ' advance to a free column

						col += 1
					Loop
					Dim colSpan As Integer = outerInstance.getColumnsOccupied(cv)
					spans(cell) = outerInstance.columnSpans(col)
					offsets(cell) = outerInstance.columnOffsets(col)
					If colSpan > 1 Then
						Dim n As Integer = outerInstance.columnSpans.Length
						For j As Integer = 1 To colSpan - 1
							' Because the table may be only partially formed, some
							' of the columns may not yet exist.  Therefore we check
							' the bounds.
							If (col+j) < n Then spans(cell) += outerInstance.columnSpans(col+j)
						Next j
						col += colSpan - 1
					End If
					cell += 1
					col += 1
				Loop
			End Sub

			''' <summary>
			''' Perform layout for the minor axis of the box (i.e. the
			''' axis orthogonal to the axis that it represents).  The results
			''' of the layout should be placed in the given arrays which represent
			''' the allocations to the children along the minor axis.  This
			''' is called by the superclass whenever the layout needs to be
			''' updated along the minor axis.
			''' <p>
			''' This is implemented to delegate to the superclass, then adjust
			''' the span for any cell that spans multiple rows.
			''' </summary>
			''' <param name="targetSpan"> the total span given to the view, which
			'''  would be used to layout the children. </param>
			''' <param name="axis"> the axis being layed out. </param>
			''' <param name="offsets"> the offsets from the origin of the view for
			'''  each of the child views.  This is a return value and is
			'''  filled in by the implementation of this method. </param>
			''' <param name="spans"> the span of each child view.  This is a return
			'''  value and is filled in by the implementation of this method. </param>
			Protected Friend Overrides Sub layoutMinorAxis(ByVal targetSpan As Integer, ByVal axis As Integer, ByVal offsets As Integer(), ByVal spans As Integer())
				MyBase.layoutMinorAxis(targetSpan, axis, offsets, spans)
				Dim col As Integer = 0
				Dim ncells As Integer = viewCount
				Dim cell As Integer = 0
				Do While cell < ncells
					Dim cv As View = getView(cell)
					Do While isFilled(col) ' advance to a free column

						col += 1
					Loop
					Dim colSpan As Integer = outerInstance.getColumnsOccupied(cv)
					Dim rowSpan As Integer = outerInstance.getRowsOccupied(cv)
					If rowSpan > 1 Then
						For j As Integer = 1 To rowSpan - 1
							' test bounds of each row because it may not exist
							' either because of error or because the table isn't
							' fully loaded yet.
							Dim ___row As Integer = row + j
							If ___row < outerInstance.viewCount Then
								Dim ___span As Integer = outerInstance.getSpan(Y_AXIS, row+j)
								spans(cell) += ___span
							End If
						Next j
					End If
					If colSpan > 1 Then col += colSpan - 1
					cell += 1
					col += 1
				Loop
			End Sub

			''' <summary>
			''' Determines the resizability of the view along the
			''' given axis.  A value of 0 or less is not resizable.
			''' </summary>
			''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
			''' <returns> the resize weight </returns>
			''' <exception cref="IllegalArgumentException"> for an invalid axis </exception>
			Public Overrides Function getResizeWeight(ByVal axis As Integer) As Integer
				Return 1
			End Function

			''' <summary>
			''' Fetches the child view that represents the given position in
			''' the model.  This is implemented to walk through the children
			''' looking for a range that contains the given position.  In this
			''' view the children do not necessarily have a one to one mapping
			''' with the child elements.
			''' </summary>
			''' <param name="pos">  the search position &gt;= 0 </param>
			''' <param name="a">  the allocation to the table on entry, and the
			'''   allocation of the view containing the position on exit </param>
			''' <returns>  the view representing the given position, or
			'''   <code>null</code> if there isn't one </returns>
			Protected Friend Overrides Function getViewAtPosition(ByVal pos As Integer, ByVal a As Rectangle) As View
				Dim n As Integer = viewCount
				For i As Integer = 0 To n - 1
					Dim v As View = getView(i)
					Dim p0 As Integer = v.startOffset
					Dim p1 As Integer = v.endOffset
					If (pos >= p0) AndAlso (pos < p1) Then
						' it's in this view.
						If a IsNot Nothing Then childAllocation(i, a)
						Return v
					End If
				Next i
				If pos = endOffset Then
					Dim v As View = getView(n - 1)
					If a IsNot Nothing Then Me.childAllocation(n - 1, a)
					Return v
				End If
				Return Nothing
			End Function

			''' <summary>
			''' columns filled by multi-column or multi-row cells </summary>
			Friend fillColumns As BitArray
			''' <summary>
			''' the row within the overall grid </summary>
			Friend row As Integer
		End Class

		''' @deprecated  A table cell can now be any View implementation. 
		<Obsolete(" A table cell can now be any View implementation.")> _
		Public Class TableCell
			Inherits BoxView
			Implements GridCell

			Private ReadOnly outerInstance As TableView


			''' <summary>
			''' Constructs a TableCell for the given element.
			''' </summary>
			''' <param name="elem"> the element that this view is responsible for
			''' @since 1.4 </param>
			Public Sub New(ByVal outerInstance As TableView, ByVal elem As Element)
					Me.outerInstance = outerInstance
				MyBase.New(elem, View.Y_AXIS)
			End Sub

			' --- GridCell methods -------------------------------------

			''' <summary>
			''' Gets the number of columns this cell spans (e.g. the
			''' grid width).
			''' </summary>
			''' <returns> the number of columns </returns>
			Public Overridable Property columnCount As Integer
				Get
					Return 1
				End Get
			End Property

			''' <summary>
			''' Gets the number of rows this cell spans (that is, the
			''' grid height).
			''' </summary>
			''' <returns> the number of rows </returns>
			Public Overridable Property rowCount As Integer
				Get
					Return 1
				End Get
			End Property


			''' <summary>
			''' Sets the grid location.
			''' </summary>
			''' <param name="row"> the row &gt;= 0 </param>
			''' <param name="col"> the column &gt;= 0 </param>
			Public Overridable Sub setGridLocation(ByVal row As Integer, ByVal col As Integer)
				Me.row = row
				Me.col = col
			End Sub

			''' <summary>
			''' Gets the row of the grid location
			''' </summary>
			Public Overridable Property gridRow As Integer
				Get
					Return row
				End Get
			End Property

			''' <summary>
			''' Gets the column of the grid location
			''' </summary>
			Public Overridable Property gridColumn As Integer
				Get
					Return col
				End Get
			End Property

			Friend row As Integer
			Friend col As Integer
		End Class

		''' <summary>
		''' <em>
		''' THIS IS NO LONGER USED, AND WILL BE REMOVED IN THE
		''' NEXT RELEASE.  THE JCK SIGNATURE TEST THINKS THIS INTERFACE
		''' SHOULD EXIST
		''' </em>
		''' </summary>
		Friend Interface GridCell

			''' <summary>
			''' Sets the grid location.
			''' </summary>
			''' <param name="row"> the row &gt;= 0 </param>
			''' <param name="col"> the column &gt;= 0 </param>
			Sub setGridLocation(ByVal row As Integer, ByVal col As Integer)

			''' <summary>
			''' Gets the row of the grid location
			''' </summary>
			ReadOnly Property gridRow As Integer

			''' <summary>
			''' Gets the column of the grid location
			''' </summary>
			ReadOnly Property gridColumn As Integer

			''' <summary>
			''' Gets the number of columns this cell spans (e.g. the
			''' grid width).
			''' </summary>
			''' <returns> the number of columns </returns>
			ReadOnly Property columnCount As Integer

			''' <summary>
			''' Gets the number of rows this cell spans (that is, the
			''' grid height).
			''' </summary>
			''' <returns> the number of rows </returns>
			ReadOnly Property rowCount As Integer

		End Interface

	End Class

End Namespace
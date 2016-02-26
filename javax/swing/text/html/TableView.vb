Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports javax.swing.text

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
Namespace javax.swing.text.html


	''' <summary>
	''' HTML table view.
	''' 
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref=     View </seealso>
	'public
	 Friend Class TableView
		 Inherits BoxView
		 Implements ViewFactory

		''' <summary>
		''' Constructs a TableView for the given element.
		''' </summary>
		''' <param name="elem"> the element that this view is responsible for </param>
		Public Sub New(ByVal elem As Element)
			MyBase.New(elem, View.Y_AXIS)
			rows = New List(Of RowView)
			gridValid = False
			captionIndex = -1
			totalColumnRequirements = New javax.swing.SizeRequirements
		End Sub

		''' <summary>
		''' Creates a new table row.
		''' </summary>
		''' <param name="elem"> an element </param>
		''' <returns> the row </returns>
		Protected Friend Overridable Function createTableRow(ByVal elem As Element) As RowView
			' PENDING(prinz) need to add support for some of the other
			' elements, but for now just ignore anything that is not
			' a TR.
			Dim o As Object = elem.attributes.getAttribute(StyleConstants.NameAttribute)
			If o Is HTML.Tag.TR Then Return New RowView(Me, elem)
			Return Nothing
		End Function

		''' <summary>
		''' The number of columns in the table.
		''' </summary>
		Public Overridable Property columnCount As Integer
			Get
				Return columnSpans.Length
			End Get
		End Property

		''' <summary>
		''' Fetches the span (width) of the given column.
		''' This is used by the nested cells to query the
		''' sizes of grid locations outside of themselves.
		''' </summary>
		Public Overridable Function getColumnSpan(ByVal col As Integer) As Integer
			If col < columnSpans.Length Then Return columnSpans(col)
			Return 0
		End Function

		''' <summary>
		''' The number of rows in the table.
		''' </summary>
		Public Overridable Property rowCount As Integer
			Get
				Return rows.Count
			End Get
		End Property

		''' <summary>
		''' Fetch the span of multiple rows.  This includes
		''' the border area.
		''' </summary>
		Public Overridable Function getMultiRowSpan(ByVal row0 As Integer, ByVal row1 As Integer) As Integer
			Dim rv0 As RowView = getRow(row0)
			Dim rv1 As RowView = getRow(row1)
			If (rv0 IsNot Nothing) AndAlso (rv1 IsNot Nothing) Then
				Dim index0 As Integer = rv0.viewIndex
				Dim index1 As Integer = rv1.viewIndex
				Dim ___span As Integer = getOffset(Y_AXIS, index1) - getOffset(Y_AXIS, index0) + getSpan(Y_AXIS, index1)
				Return ___span
			End If
			Return 0
		End Function

		''' <summary>
		''' Fetches the span (height) of the given row.
		''' </summary>
		Public Overridable Function getRowSpan(ByVal row As Integer) As Integer
			Dim rv As RowView = getRow(row)
			If rv IsNot Nothing Then Return getSpan(Y_AXIS, rv.viewIndex)
			Return 0
		End Function

		Friend Overridable Function getRow(ByVal row As Integer) As RowView
			If row < rows.Count Then Return rows(row)
			Return Nothing
		End Function

		Protected Friend Overrides Function getViewAtPoint(ByVal x As Integer, ByVal y As Integer, ByVal alloc As Rectangle) As View
			Dim n As Integer = viewCount
			Dim v As View
			Dim allocation As New Rectangle
			For i As Integer = 0 To n - 1
				allocation.bounds = alloc
				childAllocation(i, allocation)
				v = getView(i)
				If TypeOf v Is RowView Then
					v = CType(v, RowView).findViewAtPoint(x, y, allocation)
					If v IsNot Nothing Then
						alloc.bounds = allocation
						Return v
					End If
				End If
			Next i
			Return MyBase.getViewAtPoint(x, y, alloc)
		End Function

		''' <summary>
		''' Determines the number of columns occupied by
		''' the table cell represented by given element.
		''' </summary>
		Protected Friend Overridable Function getColumnsOccupied(ByVal v As View) As Integer
			Dim a As AttributeSet = v.element.attributes

			If a.isDefined(HTML.Attribute.COLSPAN) Then
				Dim s As String = CStr(a.getAttribute(HTML.Attribute.COLSPAN))
				If s IsNot Nothing Then
					Try
						Return Convert.ToInt32(s)
					Catch nfe As NumberFormatException
						' fall through to one column
					End Try
				End If
			End If

			Return 1
		End Function

		''' <summary>
		''' Determines the number of rows occupied by
		''' the table cell represented by given element.
		''' </summary>
		Protected Friend Overridable Function getRowsOccupied(ByVal v As View) As Integer
			Dim a As AttributeSet = v.element.attributes

			If a.isDefined(HTML.Attribute.ROWSPAN) Then
				Dim s As String = CStr(a.getAttribute(HTML.Attribute.ROWSPAN))
				If s IsNot Nothing Then
					Try
						Return Convert.ToInt32(s)
					Catch nfe As NumberFormatException
						' fall through to one row
					End Try
				End If
			End If

			Return 1
		End Function

		Protected Friend Overridable Sub invalidateGrid()
			gridValid = False
		End Sub

		Protected Friend Overridable Property styleSheet As StyleSheet
			Get
				Dim doc As HTMLDocument = CType(document, HTMLDocument)
				Return doc.styleSheet
			End Get
		End Property

		''' <summary>
		''' Update the insets, which contain the caption if there
		''' is a caption.
		''' </summary>
		Friend Overridable Sub updateInsets()
			Dim top As Short = CShort(Fix(painter.getInset(TOP, Me)))
			Dim bottom As Short = CShort(Fix(painter.getInset(BOTTOM, Me)))
			If captionIndex <> -1 Then
				Dim caption As View = getView(captionIndex)
				Dim h As Short = CShort(Fix(caption.getPreferredSpan(Y_AXIS)))
				Dim a As AttributeSet = caption.attributes
				Dim align As Object = a.getAttribute(CSS.Attribute.CAPTION_SIDE)
				If (align IsNot Nothing) AndAlso (align.Equals("bottom")) Then
					bottom += h
				Else
					top += h
				End If
			End If
			insetsets(top, CShort(Fix(painter.getInset(LEFT, Me))), bottom, CShort(Fix(painter.getInset(RIGHT, Me))))
		End Sub

		''' <summary>
		''' Update any cached values that come from attributes.
		''' </summary>
		Protected Friend Overridable Sub setPropertiesFromAttributes()
			Dim sheet As StyleSheet = styleSheet
			attr = sheet.getViewAttributes(Me)
			painter = sheet.getBoxPainter(attr)
			If attr IsNot Nothing Then
				insetsets(CShort(Fix(painter.getInset(TOP, Me))), CShort(Fix(painter.getInset(LEFT, Me))), CShort(Fix(painter.getInset(BOTTOM, Me))), CShort(Fix(painter.getInset(RIGHT, Me))))

				Dim lv As CSS.LengthValue = CType(attr.getAttribute(CSS.Attribute.BORDER_SPACING), CSS.LengthValue)
				If lv IsNot Nothing Then
					cellSpacing = CInt(Fix(lv.value))
				Else
					' Default cell spacing equals 2
					cellSpacing = 2
				End If
				lv = CType(attr.getAttribute(CSS.Attribute.BORDER_TOP_WIDTH), CSS.LengthValue)
				If lv IsNot Nothing Then
						borderWidth = CInt(Fix(lv.value))
				Else
						borderWidth = 0
				End If
			End If
		End Sub

		''' <summary>
		''' Fill in the grid locations that are placeholders
		''' for multi-column, multi-row, and missing grid
		''' locations.
		''' </summary>
		Friend Overridable Sub updateGrid()
			If Not gridValid Then
				relativeCells = False
				multiRowCells = False

				' determine which views are table rows and clear out
				' grid points marked filled.
				captionIndex = -1
				rows.Clear()
				Dim n As Integer = viewCount
				For i As Integer = 0 To n - 1
					Dim v As View = getView(i)
					If TypeOf v Is RowView Then
						rows.Add(CType(v, RowView))
						Dim rv As RowView = CType(v, RowView)
						rv.clearFilledColumns()
						rv.rowIndex = rows.Count - 1
						rv.viewIndex = i
					Else
						Dim o As Object = v.element.attributes.getAttribute(StyleConstants.NameAttribute)
						If TypeOf o Is HTML.Tag Then
							Dim kind As HTML.Tag = CType(o, HTML.Tag)
							If kind Is HTML.Tag.CAPTION Then captionIndex = i
						End If
					End If
				Next i

				Dim maxColumns As Integer = 0
				Dim nrows As Integer = rows.Count
				For ___row As Integer = 0 To nrows - 1
					Dim rv As RowView = getRow(___row)
					Dim col As Integer = 0
					Dim cell As Integer = 0
					Do While cell < rv.viewCount
						Dim cv As View = rv.getView(cell)
						If Not relativeCells Then
							Dim a As AttributeSet = cv.attributes
							Dim lv As CSS.LengthValue = CType(a.getAttribute(CSS.Attribute.WIDTH), CSS.LengthValue)
							If (lv IsNot Nothing) AndAlso (lv.percentage) Then relativeCells = True
						End If
						' advance to a free column
						Do While rv.isFilled(col)

							col += 1
						Loop
						Dim ___rowSpan As Integer = getRowsOccupied(cv)
						If ___rowSpan > 1 Then multiRowCells = True
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
					columnRequirements(i).maximum = Integer.MaxValue
				Next i
				gridValid = True
			End If
		End Sub

		''' <summary>
		''' Mark a grid location as filled in for a cells overflow.
		''' </summary>
		Friend Overridable Sub addFill(ByVal row As Integer, ByVal col As Integer)
			Dim rv As RowView = getRow(row)
			If rv IsNot Nothing Then rv.fillColumn(col)
		End Sub

		''' <summary>
		''' Layout the columns to fit within the given target span.
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
		''' <returns> the offset from the origin and the span for each column
		'''  in the offsets and spans parameters </returns>
		Protected Friend Overridable Sub layoutColumns(ByVal targetSpan As Integer, ByVal offsets As Integer(), ByVal spans As Integer(), ByVal reqs As javax.swing.SizeRequirements())
			'clean offsets and spans
			java.util.Arrays.fill(offsets, 0)
			java.util.Arrays.fill(spans, 0)
			colIterator.layoutArraysays(offsets, spans, targetSpan)
			CSS.calculateTiledLayout(colIterator, targetSpan)
		End Sub

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
			' clean columnRequirements
			For Each req As javax.swing.SizeRequirements In columnRequirements
				req.minimum = 0
				req.preferred = 0
				req.maximum = Integer.MaxValue
			Next req
			Dim host As Container = container
			If host IsNot Nothing Then
				If TypeOf host Is JTextComponent Then
					skipComments = Not CType(host, JTextComponent).editable
				Else
					skipComments = True
				End If
			End If
			' pass 1 - single column cells
			Dim hasMultiColumn As Boolean = False
			Dim nrows As Integer = rowCount
			For i As Integer = 0 To nrows - 1
				Dim ___row As RowView = getRow(i)
				Dim col As Integer = 0
				Dim ncells As Integer = ___row.viewCount
				For cell As Integer = 0 To ncells - 1
					Dim cv As View = ___row.getView(cell)
					If skipComments AndAlso Not(TypeOf cv Is CellView) Then Continue For
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
					col += 1
				Next cell
			Next i

			' pass 2 - multi-column cells
			If hasMultiColumn Then
				For i As Integer = 0 To nrows - 1
					Dim ___row As RowView = getRow(i)
					Dim col As Integer = 0
					Dim ncells As Integer = ___row.viewCount
					For cell As Integer = 0 To ncells - 1
						Dim cv As View = ___row.getView(cell)
						If skipComments AndAlso Not(TypeOf cv Is CellView) Then Continue For
						Do While ___row.isFilled(col) ' advance to a free column

							col += 1
						Loop
						Dim colSpan As Integer = getColumnsOccupied(cv)
						If colSpan > 1 Then
							checkMultiColumnCell(axis, col, colSpan, cv)
							col += colSpan - 1
						End If
						col += 1
					Next cell
				Next i
			End If
		End Sub

		''' <summary>
		''' check the requirements of a table cell that spans a single column.
		''' </summary>
		Friend Overridable Sub checkSingleColumnCell(ByVal axis As Integer, ByVal col As Integer, ByVal v As View)
			Dim req As javax.swing.SizeRequirements = columnRequirements(col)
			req.minimum = Math.Max(CInt(Fix(v.getMinimumSpan(axis))), req.minimum)
			req.preferred = Math.Max(CInt(Fix(v.getPreferredSpan(axis))), req.preferred)
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
	'             * this table cell.... calculate the adjustments.
	'             
				Dim reqs As javax.swing.SizeRequirements() = New javax.swing.SizeRequirements(ncols - 1){}
				For i As Integer = 0 To ncols - 1
					reqs(i) = columnRequirements(col + i)
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
	'             * this table cell.... calculate the adjustments.
	'             
				Dim reqs As javax.swing.SizeRequirements() = New javax.swing.SizeRequirements(ncols - 1){}
				For i As Integer = 0 To ncols - 1
					reqs(i) = columnRequirements(col + i)
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

		' --- BoxView methods -----------------------------------------

		''' <summary>
		''' Calculate the requirements for the minor axis.  This is called by
		''' the superclass whenever the requirements need to be updated (i.e.
		''' a preferenceChanged was messaged through this view).
		''' <p>
		''' This is implemented to calculate the requirements as the sum of the
		''' requirements of the columns and then adjust it if the
		''' CSS width or height attribute is specified and applicable to
		''' the axis.
		''' </summary>
		Protected Friend Overrides Function calculateMinorAxisRequirements(ByVal axis As Integer, ByVal r As javax.swing.SizeRequirements) As javax.swing.SizeRequirements
			updateGrid()

			' calculate column requirements for each column
			calculateColumnRequirements(axis)


			' the requirements are the sum of the columns.
			If r Is Nothing Then r = New javax.swing.SizeRequirements
			Dim min As Long = 0
			Dim pref As Long = 0
			Dim n As Integer = columnRequirements.Length
			For i As Integer = 0 To n - 1
				Dim req As javax.swing.SizeRequirements = columnRequirements(i)
				min += req.minimum
				pref += req.preferred
			Next i
			Dim adjust As Integer = (n + 1) * cellSpacing + 2 * borderWidth
			min += adjust
			pref += adjust
			r.minimum = CInt(min)
			r.preferred = CInt(pref)
			r.maximum = CInt(pref)


			Dim attr As AttributeSet = attributes
			Dim cssWidth As CSS.LengthValue = CType(attr.getAttribute(CSS.Attribute.WIDTH), CSS.LengthValue)

			If BlockView.spanSetFromAttributes(axis, r, cssWidth, Nothing) Then
				If r.minimum < CInt(min) Then
					' The user has requested a smaller size than is needed to
					' show the table, override it.
						r.preferred = CInt(min)
							r.minimum = r.preferred
							r.maximum = r.minimum
				End If
			End If
			totalColumnRequirements.minimum = r.minimum
			totalColumnRequirements.preferred = r.preferred
			totalColumnRequirements.maximum = r.maximum

			' set the alignment
			Dim o As Object = attr.getAttribute(CSS.Attribute.TEXT_ALIGN)
			If o IsNot Nothing Then
				' set horizontal alignment
				Dim ta As String = o.ToString()
				If ta.Equals("left") Then
					r.alignment = 0
				ElseIf ta.Equals("center") Then
					r.alignment = 0.5f
				ElseIf ta.Equals("right") Then
					r.alignment = 1
				Else
					r.alignment = 0
				End If
			Else
				r.alignment = 0
			End If

			Return r
		End Function

		''' <summary>
		''' Calculate the requirements for the major axis.  This is called by
		''' the superclass whenever the requirements need to be updated (i.e.
		''' a preferenceChanged was messaged through this view).
		''' <p>
		''' This is implemented to provide the superclass behavior adjusted for
		''' multi-row table cells.
		''' </summary>
		Protected Friend Overrides Function calculateMajorAxisRequirements(ByVal axis As Integer, ByVal r As javax.swing.SizeRequirements) As javax.swing.SizeRequirements
			updateInsets()
			rowIterator.updateAdjustments()
			r = CSS.calculateTiledRequirements(rowIterator, r)
			r.maximum = r.preferred
			Return r
		End Function

		''' <summary>
		''' Perform layout for the minor axis of the box (i.e. the
		''' axis orthogonal to the axis that it represents).  The results
		''' of the layout should be placed in the given arrays which represent
		''' the allocations to the children along the minor axis.  This
		''' is called by the superclass whenever the layout needs to be
		''' updated along the minor axis.
		''' <p>
		''' This is implemented to call the
		''' <a href="#layoutColumns">layoutColumns</a> method, and then
		''' forward to the superclass to actually carry out the layout
		''' of the tables rows.
		''' </summary>
		''' <param name="targetSpan"> the total span given to the view, which
		'''  would be used to layout the children </param>
		''' <param name="axis"> the axis being layed out </param>
		''' <param name="offsets"> the offsets from the origin of the view for
		'''  each of the child views.  This is a return value and is
		'''  filled in by the implementation of this method </param>
		''' <param name="spans"> the span of each child view;  this is a return
		'''  value and is filled in by the implementation of this method </param>
		''' <returns> the offset and span for each child view in the
		'''  offsets and spans parameters </returns>
		Protected Friend Overrides Sub layoutMinorAxis(ByVal targetSpan As Integer, ByVal axis As Integer, ByVal offsets As Integer(), ByVal spans As Integer())
			' make grid is properly represented
			updateGrid()

			' all of the row layouts are invalid, so mark them that way
			Dim n As Integer = rowCount
			For i As Integer = 0 To n - 1
				Dim ___row As RowView = getRow(i)
				___row.layoutChanged(axis)
			Next i

			' calculate column spans
			layoutColumns(targetSpan, columnOffsets, columnSpans, columnRequirements)

			' continue normal layout
			MyBase.layoutMinorAxis(targetSpan, axis, offsets, spans)
		End Sub


		''' <summary>
		''' Perform layout for the major axis of the box (i.e. the
		''' axis that it represents).  The results
		''' of the layout should be placed in the given arrays which represent
		''' the allocations to the children along the minor axis.  This
		''' is called by the superclass whenever the layout needs to be
		''' updated along the minor axis.
		''' <p>
		''' This method is where the layout of the table rows within the
		''' table takes place.  This method is implemented to call the use
		''' the RowIterator and the CSS collapsing tile to layout
		''' with border spacing and border collapsing capabilities.
		''' </summary>
		''' <param name="targetSpan"> the total span given to the view, which
		'''  would be used to layout the children </param>
		''' <param name="axis"> the axis being layed out </param>
		''' <param name="offsets"> the offsets from the origin of the view for
		'''  each of the child views; this is a return value and is
		'''  filled in by the implementation of this method </param>
		''' <param name="spans"> the span of each child view; this is a return
		'''  value and is filled in by the implementation of this method </param>
		''' <returns> the offset and span for each child view in the
		'''  offsets and spans parameters </returns>
		Protected Friend Overrides Sub layoutMajorAxis(ByVal targetSpan As Integer, ByVal axis As Integer, ByVal offsets As Integer(), ByVal spans As Integer())
			rowIterator.layoutArraysays(offsets, spans)
			CSS.calculateTiledLayout(rowIterator, targetSpan)

			If captionIndex <> -1 Then
				' place the caption
				Dim caption As View = getView(captionIndex)
				Dim h As Integer = CInt(Fix(caption.getPreferredSpan(Y_AXIS)))
				spans(captionIndex) = h
				Dim boxBottom As Short = CShort(Fix(painter.getInset(BOTTOM, Me)))
				If boxBottom <> bottomInset Then
					offsets(captionIndex) = targetSpan + boxBottom
				Else
					offsets(captionIndex) = - topInset
				End If
			End If
		End Sub

		''' <summary>
		''' Fetches the child view that represents the given position in
		''' the model.  This is implemented to walk through the children
		''' looking for a range that contains the given position.  In this
		''' view the children do not necessarily have a one to one mapping
		''' with the child elements.
		''' </summary>
		''' <param name="pos">  the search position >= 0 </param>
		''' <param name="a">  the allocation to the table on entry, and the
		'''   allocation of the view containing the position on exit </param>
		''' <returns>  the view representing the given position, or
		'''   null if there isn't one </returns>
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

		' --- View methods ---------------------------------------------

		''' <summary>
		''' Fetches the attributes to use when rendering.  This is
		''' implemented to multiplex the attributes specified in the
		''' model with a StyleSheet.
		''' </summary>
		Public Property Overrides attributes As AttributeSet
			Get
				If attr Is Nothing Then
					Dim sheet As StyleSheet = styleSheet
					attr = sheet.getViewAttributes(Me)
				End If
				Return attr
			End Get
		End Property

		''' <summary>
		''' Renders using the given rendering surface and area on that
		''' surface.  This is implemented to delegate to the css box
		''' painter to paint the border and background prior to the
		''' interior.  The superclass culls rendering the children
		''' that don't directly intersect the clip and the row may
		''' have cells hanging from a row above in it.  The table
		''' does not use the superclass rendering behavior and instead
		''' paints all of the rows and lets the rows cull those
		''' cells not intersecting the clip region.
		''' </summary>
		''' <param name="g"> the rendering surface to use </param>
		''' <param name="allocation"> the allocated region to render into </param>
		''' <seealso cref= View#paint </seealso>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal allocation As Shape)
			' paint the border
			Dim a As Rectangle = allocation.bounds
			sizeize(a.width, a.height)
			If captionIndex <> -1 Then
				' adjust the border for the caption
				Dim top As Short = CShort(Fix(painter.getInset(TOP, Me)))
				Dim bottom As Short = CShort(Fix(painter.getInset(BOTTOM, Me)))
				If top <> topInset Then
					Dim h As Integer = topInset - top
					a.y += h
					a.height -= h
				Else
					a.height -= bottomInset - bottom
				End If
			End If
			painter.paint(g, a.x, a.y, a.width, a.height, Me)
			' paint interior
			Dim n As Integer = viewCount
			For i As Integer = 0 To n - 1
				Dim v As View = getView(i)
				v.paint(g, getChildAllocation(i, allocation))
			Next i
			'super.paint(g, a);
		End Sub

		''' <summary>
		''' Establishes the parent view for this view.  This is
		''' guaranteed to be called before any other methods if the
		''' parent view is functioning properly.
		''' <p>
		''' This is implemented
		''' to forward to the superclass as well as call the
		''' <a href="#setPropertiesFromAttributes">setPropertiesFromAttributes</a>
		''' method to set the paragraph properties from the css
		''' attributes.  The call is made at this time to ensure
		''' the ability to resolve upward through the parents
		''' view attributes.
		''' </summary>
		''' <param name="parent"> the new parent, or null if the view is
		'''  being removed from a parent it was previously added
		'''  to </param>
		Public Overrides Property parent As View
			Set(ByVal parent As View)
				MyBase.parent = parent
				If parent IsNot Nothing Then propertiesFromAttributestes()
			End Set
		End Property

		''' <summary>
		''' Fetches the ViewFactory implementation that is feeding
		''' the view hierarchy.
		''' This replaces the ViewFactory with an implementation that
		''' calls through to the createTableRow and createTableCell
		''' methods.   If the element given to the factory isn't a
		''' table row or cell, the request is delegated to the factory
		''' produced by the superclass behavior.
		''' </summary>
		''' <returns> the factory, null if none </returns>
		Public Property Overrides viewFactory As ViewFactory
			Get
				Return Me
			End Get
		End Property

		''' <summary>
		''' Gives notification that something was inserted into
		''' the document in a location that this view is responsible for.
		''' This replaces the ViewFactory with an implementation that
		''' calls through to the createTableRow and createTableCell
		''' methods.   If the element given to the factory isn't a
		''' table row or cell, the request is delegated to the factory
		''' passed as an argument.
		''' </summary>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#insertUpdate </seealso>
		Public Overridable Sub insertUpdate(ByVal e As javax.swing.event.DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			MyBase.insertUpdate(e, a, Me)
		End Sub

		''' <summary>
		''' Gives notification that something was removed from the document
		''' in a location that this view is responsible for.
		''' This replaces the ViewFactory with an implementation that
		''' calls through to the createTableRow and createTableCell
		''' methods.   If the element given to the factory isn't a
		''' table row or cell, the request is delegated to the factory
		''' passed as an argument.
		''' </summary>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#removeUpdate </seealso>
		Public Overridable Sub removeUpdate(ByVal e As javax.swing.event.DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			MyBase.removeUpdate(e, a, Me)
		End Sub

		''' <summary>
		''' Gives notification from the document that attributes were changed
		''' in a location that this view is responsible for.
		''' This replaces the ViewFactory with an implementation that
		''' calls through to the createTableRow and createTableCell
		''' methods.   If the element given to the factory isn't a
		''' table row or cell, the request is delegated to the factory
		''' passed as an argument.
		''' </summary>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#changedUpdate </seealso>
		Public Overridable Sub changedUpdate(ByVal e As javax.swing.event.DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			MyBase.changedUpdate(e, a, Me)
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

		' --- ViewFactory methods ------------------------------------------

		''' <summary>
		''' The table itself acts as a factory for the various
		''' views that actually represent pieces of the table.
		''' All other factory activity is delegated to the factory
		''' returned by the parent of the table.
		''' </summary>
		Public Overridable Function create(ByVal elem As Element) As View Implements ViewFactory.create
			Dim o As Object = elem.attributes.getAttribute(StyleConstants.NameAttribute)
			If TypeOf o Is HTML.Tag Then
				Dim kind As HTML.Tag = CType(o, HTML.Tag)
				If kind Is HTML.Tag.TR Then
					Return createTableRow(elem)
				ElseIf (kind Is HTML.Tag.TD) OrElse (kind Is HTML.Tag.TH) Then
					Return New CellView(Me, elem)
				ElseIf kind Is HTML.Tag.CAPTION Then
					Return New javax.swing.text.html.ParagraphView(elem)
				End If
			End If
			' default is to delegate to the normal factory
			Dim p As View = parent
			If p IsNot Nothing Then
				Dim f As ViewFactory = p.viewFactory
				If f IsNot Nothing Then Return f.create(elem)
			End If
			Return Nothing
		End Function

		' ---- variables ----------------------------------------------------

		Private attr As AttributeSet
		Private painter As StyleSheet.BoxPainter

		Private cellSpacing As Integer
		Private borderWidth As Integer

		''' <summary>
		''' The index of the caption view if there is a caption.
		''' This has a value of -1 if there is no caption.  The
		''' caption lives in the inset area of the table, and is
		''' updated with each time the grid is recalculated.
		''' </summary>
		Private captionIndex As Integer

		''' <summary>
		''' Do any of the table cells contain a relative size
		''' specification?  This is updated with each call to
		''' updateGrid().  If this is true, the ColumnIterator
		''' will do extra work to calculate relative cell
		''' specifications.
		''' </summary>
		Private relativeCells As Boolean

		''' <summary>
		''' Do any of the table cells span multiple rows?  If
		''' true, the RowRequirementIterator will do additional
		''' work to adjust the requirements of rows spanned by
		''' a single table cell.  This is updated with each call to
		''' updateGrid().
		''' </summary>
		Private multiRowCells As Boolean

		Friend columnSpans As Integer()
		Friend columnOffsets As Integer()
		''' <summary>
		''' SizeRequirements for all the columns.
		''' </summary>
		Friend totalColumnRequirements As javax.swing.SizeRequirements
		Friend columnRequirements As javax.swing.SizeRequirements()

		Friend rowIterator As New RowIterator(Me)
		Friend colIterator As New ColumnIterator(Me)

		Friend rows As List(Of RowView)

		' whether to display comments inside table or not.
		Friend skipComments As Boolean = False

		Friend gridValid As Boolean
		Private Shared ReadOnly EMPTY As New BitArray

		Friend Class ColumnIterator
			Implements CSS.LayoutIterator

			Private ReadOnly outerInstance As TableView

			Public Sub New(ByVal outerInstance As TableView)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Disable percentage adjustments which should only apply
			''' when calculating layout, not requirements.
			''' </summary>
			Friend Overridable Sub disablePercentages()
				percentages = Nothing
			End Sub

			''' <summary>
			''' Update percentage adjustments if they are needed.
			''' </summary>
			Private Sub updatePercentagesAndAdjustmentWeights(ByVal span As Integer)
				adjustmentWeights = New Integer(outerInstance.columnRequirements.Length - 1){}
				For i As Integer = 0 To outerInstance.columnRequirements.Length - 1
					adjustmentWeights(i) = 0
				Next i
				If outerInstance.relativeCells Then
					percentages = New Integer(outerInstance.columnRequirements.Length - 1){}
				Else
					percentages = Nothing
				End If
				Dim nrows As Integer = outerInstance.rowCount
				For rowIndex As Integer = 0 To nrows - 1
					Dim row As RowView = outerInstance.getRow(rowIndex)
					Dim col As Integer = 0
					Dim ncells As Integer = row.viewCount
					Dim cell As Integer = 0
					Do While cell < ncells
						Dim cv As View = row.getView(cell)
						Do While row.isFilled(col) ' advance to a free column

							col += 1
						Loop
						Dim rowSpan As Integer = outerInstance.getRowsOccupied(cv)
						Dim colSpan As Integer = outerInstance.getColumnsOccupied(cv)
						Dim a As AttributeSet = cv.attributes
						Dim lv As CSS.LengthValue = CType(a.getAttribute(CSS.Attribute.WIDTH), CSS.LengthValue)
						If lv IsNot Nothing Then
							Dim len As Integer = CInt(Fix(lv.getValue(span) / colSpan + 0.5f))
							For i As Integer = 0 To colSpan - 1
								If lv.percentage Then
									' add a percentage requirement
									percentages(col+i) = Math.Max(percentages(col+i), len)
									adjustmentWeights(col + i) = Math.Max(adjustmentWeights(col + i), WorstAdjustmentWeight)
								Else
									adjustmentWeights(col + i) = Math.Max(adjustmentWeights(col + i), WorstAdjustmentWeight - 1)
								End If
							Next i
						End If
						col += colSpan - 1
						cell += 1
						col += 1
					Loop
				Next rowIndex
			End Sub

			''' <summary>
			''' Set the layout arrays to use for holding layout results
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			public void setLayoutArrays(int offsets() , int spans(), int targetSpan)
				Me.offsets = offsets
				Me.spans = spans
				updatePercentagesAndAdjustmentWeights(targetSpan)

			' --- RequirementIterator methods -------------------

			public Integer count
				Return outerInstance.columnRequirements.Length

			public void indexdex(Integer i)
				col = i

			public void offsetset(Integer offs)
				offsets(col) = offs

			public Integer offset
				Return offsets(col)

			public void spanpan(Integer span)
				spans(col) = span

			public Integer span
				Return spans(col)

			public Single getMinimumSpan(Single parentSpan)
				' do not care for percentages, since min span can't
				' be less than columnRequirements[col].minimum,
				' but can be less than percentage value.
				Return outerInstance.columnRequirements(col).minimum

			public Single getPreferredSpan(Single parentSpan)
				If (percentages IsNot Nothing) AndAlso (percentages(col) <> 0) Then Return Math.Max(percentages(col), outerInstance.columnRequirements(col).minimum)
				Return outerInstance.columnRequirements(col).preferred

			public Single getMaximumSpan(Single parentSpan)
				Return outerInstance.columnRequirements(col).maximum

			public Single borderWidth
				Return outerInstance.borderWidth


			public Single leadingCollapseSpan
				Return outerInstance.cellSpacing

			public Single trailingCollapseSpan
				Return outerInstance.cellSpacing

			public Integer adjustmentWeight
				Return adjustmentWeights(col)

			''' <summary>
			''' Current column index
			''' </summary>
			private Integer col

			''' <summary>
			''' percentage values (may be null since there
			''' might not be any).
			''' </summary>
			private Integer() percentages

			private Integer() adjustmentWeights

			private Integer() offsets
			private Integer() spans
		End Class

'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'		class RowIterator implements CSS.LayoutIterator
	'	{
	'
	'		RowIterator()
	'		{
	'		}
	'
	'		void updateAdjustments()
	'		{
	'			int axis = Y_AXIS;
	'			if (multiRowCells)
	'			{
	'				' adjust requirements of multi-row cells
	'				int n = getRowCount();
	'				adjustments = New int[n];
	'				for (int i = 0; i < n; i += 1)
	'				{
	'					RowView rv = getRow(i);
	'					if (rv.multiRowCells == True)
	'					{
	'						int ncells = rv.getViewCount();
	'						for (int j = 0; j < ncells; j += 1)
	'						{
	'							View v = rv.getView(j);
	'							int nrows = getRowsOccupied(v);
	'							if (nrows > 1)
	'							{
	'								int spanNeeded = (int) v.getPreferredSpan(axis);
	'								adjustMultiRowSpan(spanNeeded, nrows, i);
	'							}
	'						}
	'					}
	'				}
	'			}
	'			else
	'			{
	'				adjustments = Nothing;
	'			}
	'		}
	'
	'		''' <summary>
	'		''' Fixup preferences to accommodate a multi-row table cell
	'		''' if not already covered by existing preferences.  This is
	'		''' a no-op if not all of the rows needed (to do this check/fixup)
	'		''' have arrived yet.
	'		''' </summary>
	'		void adjustMultiRowSpan(int spanNeeded, int nrows, int rowIndex)
	'		{
	'			if ((rowIndex + nrows) > getCount())
	'			{
	'				' rows are missing (could be a bad rowspan specification)
	'				' or not all the rows have arrived.  Do the best we can with
	'				' the current set of rows.
	'				nrows = getCount() - rowIndex;
	'				if (nrows < 1)
	'				{
	'					Return;
	'				}
	'			}
	'			int span = 0;
	'			for (int i = 0; i < nrows; i += 1)
	'			{
	'				RowView rv = getRow(rowIndex + i);
	'				span += rv.getPreferredSpan(Y_AXIS);
	'			}
	'			if (spanNeeded > span)
	'			{
	'				int adjust = (spanNeeded - span);
	'				int rowAdjust = adjust / nrows;
	'				int firstAdjust = rowAdjust + (adjust - (rowAdjust * nrows));
	'				RowView rv = getRow(rowIndex);
	'				adjustments[rowIndex] = Math.max(adjustments[rowIndex], firstAdjust);
	'				for (int i = 1; i < nrows; i += 1)
	'				{
	'					adjustments[rowIndex + i] = Math.max(adjustments[rowIndex + i], rowAdjust);
	'				}
	'			}
	'		}
	'
	'		void setLayoutArrays(int[] offsets, int[] spans)
	'		{
	'			Me.offsets = offsets;
	'			Me.spans = spans;
	'		}
	'
	'		' --- RequirementIterator methods -------------------
	'
	'		public void setOffset(int offs)
	'		{
	'			RowView rv = getRow(row);
	'			if (rv != Nothing)
	'			{
	'				offsets[rv.viewIndex] = offs;
	'			}
	'		}
	'
	'		public int getOffset()
	'		{
	'			RowView rv = getRow(row);
	'			if (rv != Nothing)
	'			{
	'				Return offsets[rv.viewIndex];
	'			}
	'			Return 0;
	'		}
	'
	'		public void setSpan(int span)
	'		{
	'			RowView rv = getRow(row);
	'			if (rv != Nothing)
	'			{
	'				spans[rv.viewIndex] = span;
	'			}
	'		}
	'
	'		public int getSpan()
	'		{
	'			RowView rv = getRow(row);
	'			if (rv != Nothing)
	'			{
	'				Return spans[rv.viewIndex];
	'			}
	'			Return 0;
	'		}
	'
	'		public int getCount()
	'		{
	'			Return rows.size();
	'		}
	'
	'		public void setIndex(int i)
	'		{
	'			row = i;
	'		}
	'
	'		public float getMinimumSpan(float parentSpan)
	'		{
	'			Return getPreferredSpan(parentSpan);
	'		}
	'
	'		public float getPreferredSpan(float parentSpan)
	'		{
	'			RowView rv = getRow(row);
	'			if (rv != Nothing)
	'			{
	'				int adjust = (adjustments != Nothing) ? adjustments[row] : 0;
	'				Return rv.getPreferredSpan(outerInstance.getAxis()) + adjust;
	'			}
	'			Return 0;
	'		}
	'
	'		public float getMaximumSpan(float parentSpan)
	'		{
	'			Return getPreferredSpan(parentSpan);
	'		}
	'
	'		public float getBorderWidth()
	'		{
	'			Return borderWidth;
	'		}
	'
	'		public float getLeadingCollapseSpan()
	'		{
	'			Return cellSpacing;
	'		}
	'
	'		public float getTrailingCollapseSpan()
	'		{
	'			Return cellSpacing;
	'		}
	'
	'		public int getAdjustmentWeight()
	'		{
	'			Return 0;
	'		}
	'
	'		''' <summary>
	'		''' Current row index
	'		''' </summary>
	'		private int row;
	'
	'		''' <summary>
	'		''' Adjustments to the row requirements to handle multi-row
	'		''' table cells.
	'		''' </summary>
	'		private int[] adjustments;
	'
	'		private int[] offsets;
	'		private int[] spans;
	'	}

		''' <summary>
		''' View of a row in a row-centric table.
		''' </summary>
		public class RowView extends BoxView

			''' <summary>
			''' Constructs a TableView for the given element.
			''' </summary>
			''' <param name="elem"> the element that this view is responsible for </param>
			public RowView(Element elem)
				MyBase(elem, View.X_AXIS)
				fillColumns = New BitArray
				outerInstance.propertiesFromAttributestes()

			void clearFilledColumns()
				fillColumns.and(EMPTY)

			void fillColumn(Integer col)
				fillColumns.set(col)

			Boolean isFilled(Integer col)
				Return fillColumns.get(col)

			''' <summary>
			''' The number of columns present in this row.
			''' </summary>
			Integer columnCount
				Dim nfill As Integer = 0
				Dim n As Integer = fillColumns.size()
				For i As Integer = 0 To n - 1
					If fillColumns.get(i) Then nfill += 1
				Next i
				Return viewCount + nfill

			''' <summary>
			''' Fetches the attributes to use when rendering.  This is
			''' implemented to multiplex the attributes specified in the
			''' model with a StyleSheet.
			''' </summary>
			public AttributeSet attributes
				Return attr

			View findViewAtPoint(Integer x, Integer y, Rectangle alloc)
				Dim n As Integer = viewCount
				For i As Integer = 0 To n - 1
					If getChildAllocation(i, alloc).contains(x, y) Then
						childAllocation(i, alloc)
						Return getView(i)
					End If
				Next i
				Return Nothing

			protected StyleSheet styleSheet
				Dim doc As HTMLDocument = CType(document, HTMLDocument)
				Return doc.styleSheet

			''' <summary>
			''' This is called by a child to indicate its
			''' preferred span has changed.  This is implemented to
			''' execute the superclass behavior and well as try to
			''' determine if a row with a multi-row cell hangs across
			''' this row.  If a multi-row cell covers this row it also
			''' needs to propagate a preferenceChanged so that it will
			''' recalculate the multi-row cell.
			''' </summary>
			''' <param name="child"> the child view </param>
			''' <param name="width"> true if the width preference should change </param>
			''' <param name="height"> true if the height preference should change </param>
			public void preferenceChanged(View child, Boolean width, Boolean height)
				MyBase.preferenceChanged(child, width, height)
				If outerInstance.multiRowCells AndAlso height Then
					For i As Integer = rowIndex - 1 To 0 Step -1
						Dim rv As RowView = outerInstance.getRow(i)
						If rv.multiRowCells Then
							rv.preferenceChanged(Nothing, False, True)
							Exit For
						End If
					Next i
				End If

			' The major axis requirements for a row are dictated by the column
			' requirements. These methods use the value calculated by
			' TableView.
			protected javax.swing.SizeRequirements calculateMajorAxisRequirements(Integer axis, javax.swing.SizeRequirements r)
				Dim req As New javax.swing.SizeRequirements
				req.minimum = totalColumnRequirements.minimum
				req.maximum = totalColumnRequirements.maximum
				req.preferred = totalColumnRequirements.preferred
				req.alignment = 0f
				Return req

			public Single getMinimumSpan(Integer axis)
				Dim value As Single

				If axis = View.X_AXIS Then
					value = totalColumnRequirements.minimum + leftInset + rightInset
				Else
					value = MyBase.getMinimumSpan(axis)
				End If
				Return value

			public Single getMaximumSpan(Integer axis)
				Dim value As Single

				If axis = View.X_AXIS Then
					' We're flexible.
					value = CSng(Integer.MaxValue)
				Else
					value = MyBase.getMaximumSpan(axis)
				End If
				Return value

			public Single getPreferredSpan(Integer axis)
				Dim value As Single

				If axis = View.X_AXIS Then
					value = totalColumnRequirements.preferred + leftInset + rightInset
				Else
					value = MyBase.getPreferredSpan(axis)
				End If
				Return value

			public void changedUpdate(javax.swing.event.DocumentEvent e, Shape a, ViewFactory f)
				MyBase.changedUpdate(e, a, f)
				Dim pos As Integer = e.offset
				If pos <= startOffset AndAlso (pos + e.length) >= endOffset Then outerInstance.propertiesFromAttributestes()

			''' <summary>
			''' Renders using the given rendering surface and area on that
			''' surface.  This is implemented to delegate to the css box
			''' painter to paint the border and background prior to the
			''' interior.
			''' </summary>
			''' <param name="g"> the rendering surface to use </param>
			''' <param name="allocation"> the allocated region to render into </param>
			''' <seealso cref= View#paint </seealso>
			public void paint(Graphics g, Shape allocation)
				Dim a As Rectangle = CType(allocation, Rectangle)
				painter.paint(g, a.x, a.y, a.width, a.height, Me)
				MyBase.paint(g, a)

			''' <summary>
			''' Change the child views.  This is implemented to
			''' provide the superclass behavior and invalidate the
			''' grid so that rows and columns will be recalculated.
			''' </summary>
			public void replace(Integer offset, Integer length, View() views)
				MyBase.replace(offset, length, views)
				invalidateGrid()

			''' <summary>
			''' Calculate the height requirements of the table row.  The
			''' requirements of multi-row cells are not considered for this
			''' calculation.  The table itself will check and adjust the row
			''' requirements for all the rows that have multi-row cells spanning
			''' them.  This method updates the multi-row flag that indicates that
			''' this row and rows below need additional consideration.
			''' </summary>
			protected javax.swing.SizeRequirements calculateMinorAxisRequirements(Integer axis, javax.swing.SizeRequirements r)
	'          return super.calculateMinorAxisRequirements(axis, r);
				Dim min As Long = 0
				Dim pref As Long = 0
				Dim max As Long = 0
				multiRowCells = False
				Dim n As Integer = viewCount
				For i As Integer = 0 To n - 1
					Dim v As View = getView(i)
					If getRowsOccupied(v) > 1 Then
						multiRowCells = True
						max = Math.Max(CInt(Fix(v.getMaximumSpan(axis))), max)
					Else
						min = Math.Max(CInt(Fix(v.getMinimumSpan(axis))), min)
						pref = Math.Max(CInt(Fix(v.getPreferredSpan(axis))), pref)
						max = Math.Max(CInt(Fix(v.getMaximumSpan(axis))), max)
					End If
				Next i

				If r Is Nothing Then
					r = New javax.swing.SizeRequirements
					r.alignment = 0.5f
				End If
				r.preferred = CInt(pref)
				r.minimum = CInt(min)
				r.maximum = CInt(max)
				Return r

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
			'''  would be used to layout the children </param>
			''' <param name="axis"> the axis being layed out </param>
			''' <param name="offsets"> the offsets from the origin of the view for
			'''  each of the child views; this is a return value and is
			'''  filled in by the implementation of this method </param>
			''' <param name="spans"> the span of each child view; this is a return
			'''  value and is filled in by the implementation of this method </param>
			''' <returns> the offset and span for each child view in the
			'''  offsets and spans parameters </returns>
			protected void layoutMajorAxis(Integer targetSpan, Integer axis, Integer() offsets, Integer() spans)
				Dim col As Integer = 0
				Dim ncells As Integer = viewCount
				For cell As Integer = 0 To ncells - 1
					Dim cv As View = getView(cell)
					If skipComments AndAlso Not(TypeOf cv Is CellView) Then Continue For
					Do While isFilled(col) ' advance to a free column

						col += 1
					Loop
					Dim colSpan As Integer = getColumnsOccupied(cv)
					spans(cell) = columnSpans(col)
					offsets(cell) = columnOffsets(col)
					If colSpan > 1 Then
						Dim n As Integer = columnSpans.Length
						For j As Integer = 1 To colSpan - 1
							' Because the table may be only partially formed, some
							' of the columns may not yet exist.  Therefore we check
							' the bounds.
							If (col+j) < n Then
								spans(cell) += columnSpans(col+j)
								spans(cell) += cellSpacing
							End If
						Next j
						col += colSpan - 1
					End If
					col += 1
				Next cell

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
			'''  would be used to layout the children </param>
			''' <param name="axis"> the axis being layed out </param>
			''' <param name="offsets"> the offsets from the origin of the view for
			'''  each of the child views; this is a return value and is
			'''  filled in by the implementation of this method </param>
			''' <param name="spans"> the span of each child view; this is a return
			'''  value and is filled in by the implementation of this method </param>
			''' <returns> the offset and span for each child view in the
			'''  offsets and spans parameters </returns>
			protected void layoutMinorAxis(Integer targetSpan, Integer axis, Integer() offsets, Integer() spans)
				MyBase.layoutMinorAxis(targetSpan, axis, offsets, spans)
				Dim col As Integer = 0
				Dim ncells As Integer = viewCount
				Dim cell As Integer = 0
				Do While cell < ncells
					Dim cv As View = getView(cell)
					Do While isFilled(col) ' advance to a free column

						col += 1
					Loop
					Dim colSpan As Integer = getColumnsOccupied(cv)
					Dim ___rowSpan As Integer = getRowsOccupied(cv)
					If ___rowSpan > 1 Then

						Dim row0 As Integer = rowIndex
						Dim row1 As Integer = Math.Min(rowIndex + ___rowSpan - 1, rowCount-1)
						spans(cell) = getMultiRowSpan(row0, row1)
					End If
					If colSpan > 1 Then col += colSpan - 1
					cell += 1
					col += 1
				Loop

			''' <summary>
			''' Determines the resizability of the view along the
			''' given axis.  A value of 0 or less is not resizable.
			''' </summary>
			''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
			''' <returns> the resize weight </returns>
			''' <exception cref="IllegalArgumentException"> for an invalid axis </exception>
			public Integer getResizeWeight(Integer axis)
				Return 1

			''' <summary>
			''' Fetches the child view that represents the given position in
			''' the model.  This is implemented to walk through the children
			''' looking for a range that contains the given position.  In this
			''' view the children do not necessarily have a one to one mapping
			''' with the child elements.
			''' </summary>
			''' <param name="pos">  the search position >= 0 </param>
			''' <param name="a">  the allocation to the table on entry, and the
			'''   allocation of the view containing the position on exit </param>
			''' <returns>  the view representing the given position, or
			'''   null if there isn't one </returns>
			protected View getViewAtPosition(Integer pos, Rectangle a)
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

			''' <summary>
			''' Update any cached values that come from attributes.
			''' </summary>
			void propertiesFromAttributestes()
				Dim sheet As StyleSheet = styleSheet
				attr = sheet.getViewAttributes(Me)
				painter = sheet.getBoxPainter(attr)

			private StyleSheet.BoxPainter painter
			private AttributeSet attr

			''' <summary>
			''' columns filled by multi-column or multi-row cells </summary>
			Dim fillColumns As BitArray

			''' <summary>
			''' The row index within the overall grid
			''' </summary>
			Dim rowIndex As Integer

			''' <summary>
			''' The view index (for row index to view index conversion).
			''' This is set by the updateGrid method.
			''' </summary>
			Dim ___viewIndex As Integer

			''' <summary>
			''' Does this table row have cells that span multiple rows?
			''' </summary>
			Dim multiRowCells As Boolean


		''' <summary>
		''' Default view of an html table cell.  This needs to be moved
		''' somewhere else.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'		class CellView extends BlockView
	'	{
	'
	'		''' <summary>
	'		''' Constructs a TableCell for the given element.
	'		''' </summary>
	'		''' <param name="elem"> the element that this view is responsible for </param>
	'		public CellView(Element elem)
	'		{
	'			MyBase(elem, Y_AXIS);
	'		}
	'
	'		''' <summary>
	'		''' Perform layout for the major axis of the box (i.e. the
	'		''' axis that it represents).  The results of the layout should
	'		''' be placed in the given arrays which represent the allocations
	'		''' to the children along the major axis.  This is called by the
	'		''' superclass to recalculate the positions of the child views
	'		''' when the layout might have changed.
	'		''' <p>
	'		''' This is implemented to delegate to the superclass to
	'		''' tile the children.  If the target span is greater than
	'		''' was needed, the offsets are adjusted to align the children
	'		''' (i.e. position according to the html valign attribute).
	'		''' </summary>
	'		''' <param name="targetSpan"> the total span given to the view, which
	'		'''  would be used to layout the children </param>
	'		''' <param name="axis"> the axis being layed out </param>
	'		''' <param name="offsets"> the offsets from the origin of the view for
	'		'''  each of the child views; this is a return value and is
	'		'''  filled in by the implementation of this method </param>
	'		''' <param name="spans"> the span of each child view; this is a return
	'		'''  value and is filled in by the implementation of this method </param>
	'		''' <returns> the offset and span for each child view in the
	'		'''  offsets and spans parameters </returns>
	'		protected void layoutMajorAxis(int targetSpan, int axis, int[] offsets, int[] spans)
	'		{
	'			MyBase.layoutMajorAxis(targetSpan, axis, offsets, spans);
	'			' calculate usage
	'			int used = 0;
	'			int n = spans.length;
	'			for (int i = 0; i < n; i += 1)
	'			{
	'				used += spans[i];
	'			}
	'
	'			' calculate adjustments
	'			int adjust = 0;
	'			if (used < targetSpan)
	'			{
	'				' PENDING(prinz) change to use the css alignment.
	'				String valign = (String) getElement().getAttributes().getAttribute(HTML.Attribute.VALIGN);
	'				if (valign == Nothing)
	'				{
	'					AttributeSet rowAttr = getElement().getParentElement().getAttributes();
	'					valign = (String) rowAttr.getAttribute(HTML.Attribute.VALIGN);
	'				}
	'				if ((valign == Nothing) || valign.equals("middle"))
	'				{
	'					adjust = (targetSpan - used) / 2;
	'				}
	'				else if (valign.equals("bottom"))
	'				{
	'					adjust = targetSpan - used;
	'				}
	'			}
	'
	'			' make adjustments.
	'			if (adjust != 0)
	'			{
	'				for (int i = 0; i < n; i += 1)
	'				{
	'					offsets[i] += adjust;
	'				}
	'			}
	'		}
	'
	'		''' <summary>
	'		''' Calculate the requirements needed along the major axis.
	'		''' This is called by the superclass whenever the requirements
	'		''' need to be updated (i.e. a preferenceChanged was messaged
	'		''' through this view).
	'		''' <p>
	'		''' This is implemented to delegate to the superclass, but
	'		''' indicate the maximum size is very large (i.e. the cell
	'		''' is willing to expend to occupy the full height of the row).
	'		''' </summary>
	'		''' <param name="axis"> the axis being layed out. </param>
	'		''' <param name="r"> the requirements to fill in.  If null, a new one
	'		'''  should be allocated. </param>
	'		protected SizeRequirements calculateMajorAxisRequirements(int axis, SizeRequirements r)
	'		{
	'			SizeRequirements req = MyBase.calculateMajorAxisRequirements(axis, r);
	'			req.maximum = Integer.MAX_VALUE;
	'			Return req;
	'		}
	'
	'		@Override protected SizeRequirements calculateMinorAxisRequirements(int axis, SizeRequirements r)
	'		{
	'			SizeRequirements rv = MyBase.calculateMinorAxisRequirements(axis, r);
	'			'for the cell the minimum should be derived from the child views
	'			'the parent behaviour is to use CSS for that
	'			int n = getViewCount();
	'			int min = 0;
	'			for (int i = 0; i < n; i += 1)
	'			{
	'				View v = getView(i);
	'				min = Math.max((int) v.getMinimumSpan(axis), min);
	'			}
	'			rv.minimum = Math.min(rv.minimum, min);
	'			Return rv;
	'		}
	'	}


	 End Class

End Namespace
Imports System
Imports javax.swing.plaf

'
' * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf.synth


	''' <summary>
	''' Provides the Synth L&amp;F UI delegate for
	''' <seealso cref="javax.swing.JTable"/>.
	''' 
	''' @author Philip Milne
	''' @since 1.7
	''' </summary>
	Public Class SynthTableUI
		Inherits javax.swing.plaf.basic.BasicTableUI
		Implements SynthUI, java.beans.PropertyChangeListener

	'
	' Instance Variables
	'

		Private style As SynthStyle

		Private useTableColors As Boolean
		Private useUIBorder As Boolean
		Private alternateColor As java.awt.Color 'the background color to use for cells for alternate cells

		' TableCellRenderer installed on the JTable at the time we're installed,
		' cached so that we can reinstall them at uninstallUI time.
		Private dateRenderer As javax.swing.table.TableCellRenderer
		Private numberRenderer As javax.swing.table.TableCellRenderer
		Private doubleRender As javax.swing.table.TableCellRenderer
		Private floatRenderer As javax.swing.table.TableCellRenderer
		Private iconRenderer As javax.swing.table.TableCellRenderer
		Private imageIconRenderer As javax.swing.table.TableCellRenderer
		Private booleanRenderer As javax.swing.table.TableCellRenderer
		Private objectRenderer As javax.swing.table.TableCellRenderer

	'
	'  The installation/uninstall procedures and support
	'

		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="c"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal c As javax.swing.JComponent) As ComponentUI
			Return New SynthTableUI
		End Function

		''' <summary>
		''' Initializes JTable properties, such as font, foreground, and background.
		''' The font, foreground, and background properties are only set if their
		''' current value is either null or a UIResource, other properties are set
		''' if the current value is null.
		''' </summary>
		''' <seealso cref= #installUI </seealso>
		Protected Friend Overrides Sub installDefaults()
			dateRenderer = installRendererIfPossible(GetType(DateTime), Nothing)
			numberRenderer = installRendererIfPossible(GetType(Number), Nothing)
			doubleRender = installRendererIfPossible(GetType(Double), Nothing)
			floatRenderer = installRendererIfPossible(GetType(Single?), Nothing)
			iconRenderer = installRendererIfPossible(GetType(javax.swing.Icon), Nothing)
			imageIconRenderer = installRendererIfPossible(GetType(javax.swing.ImageIcon), Nothing)
			booleanRenderer = installRendererIfPossible(GetType(Boolean), New SynthBooleanTableCellRenderer(Me))
			objectRenderer = installRendererIfPossible(GetType(Object), New SynthTableCellRenderer(Me))
			updateStyle(table)
		End Sub

		Private Function installRendererIfPossible(ByVal objectClass As Type, ByVal renderer As javax.swing.table.TableCellRenderer) As javax.swing.table.TableCellRenderer
			Dim currentRenderer As javax.swing.table.TableCellRenderer = table.getDefaultRenderer(objectClass)
			If TypeOf currentRenderer Is UIResource Then table.defaultRendererrer(objectClass, renderer)
			Return currentRenderer
		End Function

		Private Sub updateStyle(ByVal c As javax.swing.JTable)
			Dim ___context As SynthContext = getContext(c, ENABLED)
			Dim oldStyle As SynthStyle = style
			style = SynthLookAndFeel.updateStyle(___context, Me)
			If style IsNot oldStyle Then
				___context.componentState = ENABLED Or SELECTED

				Dim sbg As java.awt.Color = table.selectionBackground
				If sbg Is Nothing OrElse TypeOf sbg Is UIResource Then table.selectionBackground = style.getColor(___context, ColorType.TEXT_BACKGROUND)

				Dim sfg As java.awt.Color = table.selectionForeground
				If sfg Is Nothing OrElse TypeOf sfg Is UIResource Then table.selectionForeground = style.getColor(___context, ColorType.TEXT_FOREGROUND)

				___context.componentState = ENABLED

				Dim gridColor As java.awt.Color = table.gridColor
				If gridColor Is Nothing OrElse TypeOf gridColor Is UIResource Then
					gridColor = CType(style.get(___context, "Table.gridColor"), java.awt.Color)
					If gridColor Is Nothing Then gridColor = style.getColor(___context, ColorType.FOREGROUND)
					table.gridColor = If(gridColor Is Nothing, New ColorUIResource(java.awt.Color.GRAY), gridColor)
				End If

				useTableColors = style.getBoolean(___context, "Table.rendererUseTableColors", True)
				useUIBorder = style.getBoolean(___context, "Table.rendererUseUIBorder", True)

				Dim rowHeight As Object = style.get(___context, "Table.rowHeight")
				If rowHeight IsNot Nothing Then javax.swing.LookAndFeel.installProperty(table, "rowHeight", rowHeight)
				Dim showGrid As Boolean = style.getBoolean(___context, "Table.showGrid", True)
				If Not showGrid Then table.showGrid = False
				Dim d As java.awt.Dimension = table.intercellSpacing
	'            if (d == null || d instanceof UIResource) {
				If d IsNot Nothing Then d = CType(style.get(___context, "Table.intercellSpacing"), java.awt.Dimension)
				alternateColor = CType(style.get(___context, "Table.alternateRowColor"), java.awt.Color)
				If d IsNot Nothing Then table.intercellSpacing = d


				If oldStyle IsNot Nothing Then
					uninstallKeyboardActions()
					installKeyboardActions()
				End If
			End If
			___context.Dispose()
		End Sub

		''' <summary>
		''' Attaches listeners to the JTable.
		''' </summary>
		Protected Friend Overrides Sub installListeners()
			MyBase.installListeners()
			table.addPropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallDefaults()
			table.defaultRendererrer(GetType(DateTime), dateRenderer)
			table.defaultRendererrer(GetType(Number), numberRenderer)
			table.defaultRendererrer(GetType(Double), doubleRender)
			table.defaultRendererrer(GetType(Single?), floatRenderer)
			table.defaultRendererrer(GetType(javax.swing.Icon), iconRenderer)
			table.defaultRendererrer(GetType(javax.swing.ImageIcon), imageIconRenderer)
			table.defaultRendererrer(GetType(Boolean), booleanRenderer)
			table.defaultRendererrer(GetType(Object), objectRenderer)

			If TypeOf table.transferHandler Is UIResource Then table.transferHandler = Nothing
			Dim ___context As SynthContext = getContext(table, ENABLED)
			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallListeners()
			table.removePropertyChangeListener(Me)
			MyBase.uninstallListeners()
		End Sub

		'
		' SynthUI
		'

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getContext(ByVal c As javax.swing.JComponent) As SynthContext Implements SynthUI.getContext
			Return getContext(c, SynthLookAndFeel.getComponentState(c))
		End Function

		Private Function getContext(ByVal c As javax.swing.JComponent, ByVal state As Integer) As SynthContext
			Return SynthContext.getContext(c, style, state)
		End Function

	'
	'  Paint methods and support
	'

		''' <summary>
		''' Notifies this UI delegate to repaint the specified component.
		''' This method paints the component background, then calls
		''' the <seealso cref="#paint(SynthContext,Graphics)"/> method.
		''' 
		''' <p>In general, this method does not need to be overridden by subclasses.
		''' All Look and Feel rendering code should reside in the {@code paint} method.
		''' </summary>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <param name="c"> the component being painted </param>
		''' <seealso cref= #paint(SynthContext,Graphics) </seealso>
		Public Overrides Sub update(ByVal g As java.awt.Graphics, ByVal c As javax.swing.JComponent)
			Dim ___context As SynthContext = getContext(c)

			SynthLookAndFeel.update(___context, g)
			___context.painter.paintTableBackground(___context, g, 0, 0, c.width, c.height)
			paint(___context, g)
			___context.Dispose()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) Implements SynthUI.paintBorder
			context.painter.paintTableBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the specified component according to the Look and Feel.
		''' <p>This method is not used by Synth Look and Feel.
		''' Painting is handled by the <seealso cref="#paint(SynthContext,Graphics)"/> method.
		''' </summary>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <param name="c"> the component being painted </param>
		''' <seealso cref= #paint(SynthContext,Graphics) </seealso>
		Public Overrides Sub paint(ByVal g As java.awt.Graphics, ByVal c As javax.swing.JComponent)
			Dim ___context As SynthContext = getContext(c)

			paint(___context, g)
			___context.Dispose()
		End Sub

		''' <summary>
		''' Paints the specified component.
		''' </summary>
		''' <param name="context"> context for the component being painted </param>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <seealso cref= #update(Graphics,JComponent) </seealso>
		Protected Friend Overridable Sub paint(ByVal context As SynthContext, ByVal g As java.awt.Graphics)
			Dim clip As java.awt.Rectangle = g.clipBounds

			Dim bounds As java.awt.Rectangle = table.bounds
			' account for the fact that the graphics has already been translated
			' into the table's bounds
				bounds.y = 0
				bounds.x = bounds.y

			If table.rowCount <= 0 OrElse table.columnCount <= 0 OrElse (Not bounds.intersects(clip)) Then
					' this check prevents us from painting the entire table
					' when the clip doesn't intersect our bounds at all

				paintDropLines(context, g)
				Return
			End If

			Dim ltr As Boolean = table.componentOrientation.leftToRight

			Dim upperLeft As java.awt.Point = clip.location

			Dim lowerRight As New java.awt.Point(clip.x + clip.width - 1, clip.y + clip.height - 1)

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

			' Paint the cells.
			paintCells(context, g, rMin, rMax, cMin, cMax)

			' Paint the grid.
			' it is important to paint the grid after the cells, otherwise the grid will be overpainted
			' because in Synth cell renderers are likely to be opaque
			paintGrid(context, g, rMin, rMax, cMin, cMax)

			paintDropLines(context, g)
		End Sub

		Private Sub paintDropLines(ByVal context As SynthContext, ByVal g As java.awt.Graphics)
			Dim loc As javax.swing.JTable.DropLocation = table.dropLocation
			If loc Is Nothing Then Return

			Dim color As java.awt.Color = CType(style.get(context, "Table.dropLineColor"), java.awt.Color)
			Dim shortColor As java.awt.Color = CType(style.get(context, "Table.dropLineShortColor"), java.awt.Color)
			If color Is Nothing AndAlso shortColor Is Nothing Then Return

			Dim rect As java.awt.Rectangle

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

		Private Function getHDropLineRect(ByVal loc As javax.swing.JTable.DropLocation) As java.awt.Rectangle
			If Not loc.insertRow Then Return Nothing

			Dim row As Integer = loc.row
			Dim col As Integer = loc.column
			If col >= table.columnCount Then col -= 1

			Dim rect As java.awt.Rectangle = table.getCellRect(row, col, True)

			If row >= table.rowCount Then
				row -= 1
				Dim prevRect As java.awt.Rectangle = table.getCellRect(row, col, True)
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

		Private Function getVDropLineRect(ByVal loc As javax.swing.JTable.DropLocation) As java.awt.Rectangle
			If Not loc.insertColumn Then Return Nothing

			Dim ltr As Boolean = table.componentOrientation.leftToRight
			Dim col As Integer = loc.column
			Dim rect As java.awt.Rectangle = table.getCellRect(loc.row, col, True)

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

		Private Function extendRect(ByVal rect As java.awt.Rectangle, ByVal horizontal As Boolean) As java.awt.Rectangle
			If rect Is Nothing Then Return rect

			If horizontal Then
				rect.x = 0
				rect.width = table.width
			Else
				rect.y = 0

				If table.rowCount <> 0 Then
					Dim lastRect As java.awt.Rectangle = table.getCellRect(table.rowCount - 1, 0, True)
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
		Private Sub paintGrid(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal rMin As Integer, ByVal rMax As Integer, ByVal cMin As Integer, ByVal cMax As Integer)
			g.color = table.gridColor

			Dim minCell As java.awt.Rectangle = table.getCellRect(rMin, cMin, True)
			Dim maxCell As java.awt.Rectangle = table.getCellRect(rMax, cMax, True)
			Dim damagedArea As java.awt.Rectangle = minCell.union(maxCell)
			Dim synthG As SynthGraphicsUtils = context.style.getGraphicsUtils(context)

			If table.showHorizontalLines Then
				Dim tableWidth As Integer = damagedArea.x + damagedArea.width
				Dim y As Integer = damagedArea.y
				For row As Integer = rMin To rMax
					y += table.getRowHeight(row)
					synthG.drawLine(context, "Table.grid", g, damagedArea.x, y - 1, tableWidth - 1,y - 1)
				Next row
			End If
			If table.showVerticalLines Then
				Dim cm As javax.swing.table.TableColumnModel = table.columnModel
				Dim tableHeight As Integer = damagedArea.y + damagedArea.height
				Dim x As Integer
				If table.componentOrientation.leftToRight Then
					x = damagedArea.x
					For column As Integer = cMin To cMax
						Dim w As Integer = cm.getColumn(column).width
						x += w
						synthG.drawLine(context, "Table.grid", g, x - 1, 0, x - 1, tableHeight - 1)
					Next column
				Else
					x = damagedArea.x
					For column As Integer = cMax To cMin Step -1
						Dim w As Integer = cm.getColumn(column).width
						x += w
						synthG.drawLine(context, "Table.grid", g, x - 1, 0, x - 1, tableHeight - 1)
					Next column
				End If
			End If
		End Sub

		Private Function viewIndexForColumn(ByVal aColumn As javax.swing.table.TableColumn) As Integer
			Dim cm As javax.swing.table.TableColumnModel = table.columnModel
			For column As Integer = 0 To cm.columnCount - 1
				If cm.getColumn(column) Is aColumn Then Return column
			Next column
			Return -1
		End Function

		Private Sub paintCells(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal rMin As Integer, ByVal rMax As Integer, ByVal cMin As Integer, ByVal cMax As Integer)
			Dim header As javax.swing.table.JTableHeader = table.tableHeader
			Dim draggedColumn As javax.swing.table.TableColumn = If(header Is Nothing, Nothing, header.draggedColumn)

			Dim cm As javax.swing.table.TableColumnModel = table.columnModel
			Dim columnMargin As Integer = cm.columnMargin

			Dim cellRect As java.awt.Rectangle
			Dim aColumn As javax.swing.table.TableColumn
			Dim columnWidth As Integer
			If table.componentOrientation.leftToRight Then
				For row As Integer = rMin To rMax
					cellRect = table.getCellRect(row, cMin, False)
					For column As Integer = cMin To cMax
						aColumn = cm.getColumn(column)
						columnWidth = aColumn.width
						cellRect.width = columnWidth - columnMargin
						If aColumn IsNot draggedColumn Then paintCell(context, g, cellRect, row, column)
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
						paintCell(context, g, cellRect, row, cMin)
					End If
					For column As Integer = cMin+1 To cMax
						aColumn = cm.getColumn(column)
						columnWidth = aColumn.width
						cellRect.width = columnWidth - columnMargin
						cellRect.x -= columnWidth
						If aColumn IsNot draggedColumn Then paintCell(context, g, cellRect, row, column)
					Next column
				Next row
			End If

			' Paint the dragged column if we are dragging.
			If draggedColumn IsNot Nothing Then paintDraggedArea(context, g, rMin, rMax, draggedColumn, header.draggedDistance)

			' Remove any renderers that may be left in the rendererPane.
			rendererPane.removeAll()
		End Sub

		Private Sub paintDraggedArea(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal rMin As Integer, ByVal rMax As Integer, ByVal draggedColumn As javax.swing.table.TableColumn, ByVal distance As Integer)
			Dim draggedColumnIndex As Integer = viewIndexForColumn(draggedColumn)

			Dim minCell As java.awt.Rectangle = table.getCellRect(rMin, draggedColumnIndex, True)
			Dim maxCell As java.awt.Rectangle = table.getCellRect(rMax, draggedColumnIndex, True)

			Dim vacatedColumnRect As java.awt.Rectangle = minCell.union(maxCell)

			' Paint a gray well in place of the moving column.
			g.color = table.parent.background
			g.fillRect(vacatedColumnRect.x, vacatedColumnRect.y, vacatedColumnRect.width, vacatedColumnRect.height)

			' Move to the where the cell has been dragged.
			vacatedColumnRect.x += distance

			' Fill the background.
			g.color = context.style.getColor(context, ColorType.BACKGROUND)
			g.fillRect(vacatedColumnRect.x, vacatedColumnRect.y, vacatedColumnRect.width, vacatedColumnRect.height)

			Dim synthG As SynthGraphicsUtils = context.style.getGraphicsUtils(context)


			' Paint the vertical grid lines if necessary.
			If table.showVerticalLines Then
				g.color = table.gridColor
				Dim x1 As Integer = vacatedColumnRect.x
				Dim y1 As Integer = vacatedColumnRect.y
				Dim x2 As Integer = x1 + vacatedColumnRect.width - 1
				Dim y2 As Integer = y1 + vacatedColumnRect.height - 1
				' Left
				synthG.drawLine(context, "Table.grid", g, x1-1, y1, x1-1, y2)
				' Right
				synthG.drawLine(context, "Table.grid", g, x2, y1, x2, y2)
			End If

			For row As Integer = rMin To rMax
				' Render the cell value
				Dim r As java.awt.Rectangle = table.getCellRect(row, draggedColumnIndex, False)
				r.x += distance
				paintCell(context, g, r, row, draggedColumnIndex)

				' Paint the (lower) horizontal grid line if necessary.
				If table.showHorizontalLines Then
					g.color = table.gridColor
					Dim rcr As java.awt.Rectangle = table.getCellRect(row, draggedColumnIndex, True)
					rcr.x += distance
					Dim x1 As Integer = rcr.x
					Dim y1 As Integer = rcr.y
					Dim x2 As Integer = x1 + rcr.width - 1
					Dim y2 As Integer = y1 + rcr.height - 1
					synthG.drawLine(context, "Table.grid", g, x1, y2, x2, y2)
				End If
			Next row
		End Sub

		Private Sub paintCell(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal cellRect As java.awt.Rectangle, ByVal row As Integer, ByVal column As Integer)
			If table.editing AndAlso table.editingRow=row AndAlso table.editingColumn=column Then
				Dim component As java.awt.Component = table.editorComponent
				component.bounds = cellRect
				component.validate()
			Else
				Dim renderer As javax.swing.table.TableCellRenderer = table.getCellRenderer(row, column)
				Dim component As java.awt.Component = table.prepareRenderer(renderer, row, column)
				Dim b As java.awt.Color = component.background
				If (b Is Nothing OrElse TypeOf b Is UIResource OrElse TypeOf component Is SynthBooleanTableCellRenderer) AndAlso (Not table.isCellSelected(row, column)) Then
					If alternateColor IsNot Nothing AndAlso row Mod 2 <> 0 Then component.background = alternateColor
				End If
				rendererPane.paintComponent(g, component, table, cellRect.x, cellRect.y, cellRect.width, cellRect.height, True)
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub propertyChange(ByVal [event] As java.beans.PropertyChangeEvent)
			If SynthLookAndFeel.shouldUpdateStyle([event]) Then updateStyle(CType([event].source, javax.swing.JTable))
		End Sub


		Private Class SynthBooleanTableCellRenderer
			Inherits javax.swing.JCheckBox
			Implements javax.swing.table.TableCellRenderer

			Private ReadOnly outerInstance As SynthTableUI

			Private isRowSelected As Boolean

			Public Sub New(ByVal outerInstance As SynthTableUI)
					Me.outerInstance = outerInstance
				horizontalAlignment = javax.swing.JLabel.CENTER
				name = "Table.cellRenderer"
			End Sub

			Public Overridable Function getTableCellRendererComponent(ByVal table As javax.swing.JTable, ByVal value As Object, ByVal isSelected As Boolean, ByVal hasFocus As Boolean, ByVal row As Integer, ByVal column As Integer) As java.awt.Component
				isRowSelected = isSelected

				If isSelected Then
					foreground = unwrap(table.selectionForeground)
					background = unwrap(table.selectionBackground)
				Else
					foreground = unwrap(table.foreground)
					background = unwrap(table.background)
				End If

				selected = (value IsNot Nothing AndAlso CBool(value))
				Return Me
			End Function

			Private Function unwrap(ByVal c As java.awt.Color) As java.awt.Color
				If TypeOf c Is UIResource Then Return New java.awt.Color(c.rGB)
				Return c
			End Function

			Public Property Overrides opaque As Boolean
				Get
					Return If(isRowSelected, True, MyBase.opaque)
				End Get
			End Property
		End Class

		Private Class SynthTableCellRenderer
			Inherits javax.swing.table.DefaultTableCellRenderer

			Private ReadOnly outerInstance As SynthTableUI

			Public Sub New(ByVal outerInstance As SynthTableUI)
				Me.outerInstance = outerInstance
			End Sub

			Private numberFormat As Object
			Private dateFormat As Object
			Private opaque As Boolean

			Public Overrides Property opaque As Boolean
				Set(ByVal isOpaque As Boolean)
					opaque = isOpaque
				End Set
				Get
					Return opaque
				End Get
			End Property


			Public Overridable Property name As String
				Get
					Dim ___name As String = MyBase.name
					If ___name Is Nothing Then Return "Table.cellRenderer"
					Return ___name
				End Get
			End Property

			Public Overridable Property border As javax.swing.border.Border
				Set(ByVal b As javax.swing.border.Border)
					If outerInstance.useUIBorder OrElse TypeOf b Is SynthBorder Then MyBase.border = b
				End Set
			End Property

			Public Overridable Function getTableCellRendererComponent(ByVal table As javax.swing.JTable, ByVal value As Object, ByVal isSelected As Boolean, ByVal hasFocus As Boolean, ByVal row As Integer, ByVal column As Integer) As java.awt.Component
				If (Not outerInstance.useTableColors) AndAlso (isSelected OrElse hasFocus) Then
					SynthLookAndFeel.selectedUIdUI(CType(SynthLookAndFeel.getUIOfType(uI, GetType(SynthLabelUI)), SynthLabelUI), isSelected, hasFocus, table.enabled, False)
				Else
					SynthLookAndFeel.resetSelectedUI()
				End If
				MyBase.getTableCellRendererComponent(table, value, isSelected, hasFocus, row, column)

				icon = Nothing
				If table IsNot Nothing Then configureValue(value, table.getColumnClass(column))
				Return Me
			End Function

			Private Sub configureValue(ByVal value As Object, ByVal columnClass As Type)
				If columnClass Is GetType(Object) OrElse columnClass Is Nothing Then
					horizontalAlignment = javax.swing.JLabel.LEADING
				ElseIf columnClass Is GetType(Single?) OrElse columnClass Is GetType(Double) Then
					If numberFormat Is Nothing Then numberFormat = java.text.NumberFormat.instance
					horizontalAlignment = javax.swing.JLabel.TRAILING
					text = If(value Is Nothing, "", CType(numberFormat, java.text.NumberFormat).format(value))
				ElseIf columnClass Is GetType(Number) Then
					horizontalAlignment = javax.swing.JLabel.TRAILING
					' Super will have set value.
				ElseIf columnClass Is GetType(javax.swing.Icon) OrElse columnClass Is GetType(javax.swing.ImageIcon) Then
					horizontalAlignment = javax.swing.JLabel.CENTER
					icon = If(TypeOf value Is javax.swing.Icon, CType(value, javax.swing.Icon), Nothing)
					text = ""
				ElseIf columnClass Is GetType(DateTime) Then
					If dateFormat Is Nothing Then dateFormat = java.text.DateFormat.dateInstance
					horizontalAlignment = javax.swing.JLabel.LEADING
					text = If(value Is Nothing, "", CType(dateFormat, java.text.Format).format(value))
				Else
					configureValue(value, columnClass.BaseType)
				End If
			End Sub

			Public Overridable Sub paint(ByVal g As java.awt.Graphics)
				MyBase.paint(g)
				SynthLookAndFeel.resetSelectedUI()
			End Sub
		End Class
	End Class

End Namespace
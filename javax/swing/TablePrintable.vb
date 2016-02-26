Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports javax.swing.table

'
' * Copyright (c) 2003, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' An implementation of <code>Printable</code> for printing
	''' <code>JTable</code>s.
	''' <p>
	''' This implementation spreads table rows naturally in sequence
	''' across multiple pages, fitting as many rows as possible per page.
	''' The distribution of columns, on the other hand, is controlled by a
	''' printing mode parameter passed to the constructor. When
	''' <code>JTable.PrintMode.NORMAL</code> is used, the implementation
	''' handles columns in a similar manner to how it handles rows, spreading them
	''' across multiple pages (in an order consistent with the table's
	''' <code>ComponentOrientation</code>).
	''' When <code>JTable.PrintMode.FIT_WIDTH</code> is given, the implementation
	''' scales the output smaller if necessary, to ensure that all columns fit on
	''' the page. (Note that width and height are scaled equally, ensuring that the
	''' aspect ratio remains the same).
	''' <p>
	''' The portion of table printed on each page is headed by the
	''' appropriate section of the table's <code>JTableHeader</code>.
	''' <p>
	''' Header and footer text can be added to the output by providing
	''' <code>MessageFormat</code> instances to the constructor. The
	''' printing code requests Strings from the formats by calling
	''' their <code>format</code> method with a single parameter:
	''' an <code>Object</code> array containing a single element of type
	''' <code>Integer</code>, representing the current page number.
	''' <p>
	''' There are certain circumstances where this <code>Printable</code>
	''' cannot fit items appropriately, resulting in clipped output.
	''' These are:
	''' <ul>
	'''   <li>In any mode, when the header or footer text is too wide to
	'''       fit completely in the printable area. The implementation
	'''       prints as much of the text as possible starting from the beginning,
	'''       as determined by the table's <code>ComponentOrientation</code>.
	'''   <li>In any mode, when a row is too tall to fit in the
	'''       printable area. The upper most portion of the row
	'''       is printed and no lower border is shown.
	'''   <li>In <code>JTable.PrintMode.NORMAL</code> when a column
	'''       is too wide to fit in the printable area. The center of the
	'''       column is printed and no left and right borders are shown.
	''' </ul>
	''' <p>
	''' It is entirely valid for a developer to wrap this <code>Printable</code>
	''' inside another in order to create complex reports and documents. They may
	''' even request that different pages be rendered into different sized
	''' printable areas. The implementation was designed to handle this by
	''' performing most of its calculations on the fly. However, providing different
	''' sizes works best when <code>JTable.PrintMode.FIT_WIDTH</code> is used, or
	''' when only the printable width is changed between pages. This is because when
	''' it is printing a set of rows in <code>JTable.PrintMode.NORMAL</code> and the
	''' implementation determines a need to distribute columns across pages,
	''' it assumes that all of those rows will fit on each subsequent page needed
	''' to fit the columns.
	''' <p>
	''' It is the responsibility of the developer to ensure that the table is not
	''' modified in any way after this <code>Printable</code> is created (invalid
	''' modifications include changes in: size, renderers, or underlying data).
	''' The behavior of this <code>Printable</code> is undefined if the table is
	''' changed at any time after creation.
	''' 
	''' @author  Shannon Hickey
	''' </summary>
	Friend Class TablePrintable
		Implements Printable

		''' <summary>
		''' The table to print. </summary>
		Private table As JTable

		''' <summary>
		''' For quick reference to the table's header. </summary>
		Private header As JTableHeader

		''' <summary>
		''' For quick reference to the table's column model. </summary>
		Private colModel As TableColumnModel

		''' <summary>
		''' To save multiple calculations of total column width. </summary>
		Private totalColWidth As Integer

		''' <summary>
		''' The printing mode of this printable. </summary>
		Private printMode As JTable.PrintMode

		''' <summary>
		''' Provides the header text for the table. </summary>
		Private headerFormat As java.text.MessageFormat

		''' <summary>
		''' Provides the footer text for the table. </summary>
		Private footerFormat As java.text.MessageFormat

		''' <summary>
		''' The most recent page index asked to print. </summary>
		Private last As Integer = -1

		''' <summary>
		''' The next row to print. </summary>
		Private row As Integer = 0

		''' <summary>
		''' The next column to print. </summary>
		Private col As Integer = 0

		''' <summary>
		''' Used to store an area of the table to be printed. </summary>
		Private ReadOnly clip As New Rectangle(0, 0, 0, 0)

		''' <summary>
		''' Used to store an area of the table's header to be printed. </summary>
		Private ReadOnly hclip As New Rectangle(0, 0, 0, 0)

		''' <summary>
		''' Saves the creation of multiple rectangles. </summary>
		Private ReadOnly tempRect As New Rectangle(0, 0, 0, 0)

		''' <summary>
		''' Vertical space to leave between table and header/footer text. </summary>
		Private Const H_F_SPACE As Integer = 8

		''' <summary>
		''' Font size for the header text. </summary>
		Private Const HEADER_FONT_SIZE As Single = 18.0f

		''' <summary>
		''' Font size for the footer text. </summary>
		Private Const FOOTER_FONT_SIZE As Single = 12.0f

		''' <summary>
		''' The font to use in rendering header text. </summary>
		Private headerFont As Font

		''' <summary>
		''' The font to use in rendering footer text. </summary>
		Private footerFont As Font

		''' <summary>
		''' Create a new <code>TablePrintable</code> for the given
		''' <code>JTable</code>. Header and footer text can be specified using the
		''' two <code>MessageFormat</code> parameters. When called upon to provide
		''' a String, each format is given the current page number.
		''' </summary>
		''' <param name="table">         the table to print </param>
		''' <param name="printMode">     the printing mode for this printable </param>
		''' <param name="headerFormat">  a <code>MessageFormat</code> specifying the text to
		'''                       be used in printing a header, or null for none </param>
		''' <param name="footerFormat">  a <code>MessageFormat</code> specifying the text to
		'''                       be used in printing a footer, or null for none </param>
		''' <exception cref="IllegalArgumentException"> if passed an invalid print mode </exception>
		Public Sub New(ByVal table As JTable, ByVal printMode As JTable.PrintMode, ByVal headerFormat As java.text.MessageFormat, ByVal footerFormat As java.text.MessageFormat)

			Me.table = table

			header = table.tableHeader
			colModel = table.columnModel
			totalColWidth = colModel.totalColumnWidth

			If header IsNot Nothing Then hclip.height = header.height

			Me.printMode = printMode

			Me.headerFormat = headerFormat
			Me.footerFormat = footerFormat

			' derive the header and footer font from the table's font
			headerFont = table.font.deriveFont(Font.BOLD, HEADER_FONT_SIZE)
			footerFont = table.font.deriveFont(Font.PLAIN, FOOTER_FONT_SIZE)
		End Sub

		''' <summary>
		''' Prints the specified page of the table into the given <seealso cref="Graphics"/>
		''' context, in the specified format.
		''' </summary>
		''' <param name="graphics">    the context into which the page is drawn </param>
		''' <param name="pageFormat">  the size and orientation of the page being drawn </param>
		''' <param name="pageIndex">   the zero based index of the page to be drawn </param>
		''' <returns>  PAGE_EXISTS if the page is rendered successfully, or
		'''          NO_SUCH_PAGE if a non-existent page index is specified </returns>
		''' <exception cref="PrinterException"> if an error causes printing to be aborted </exception>
		Public Overridable Function print(ByVal graphics As Graphics, ByVal pageFormat As PageFormat, ByVal pageIndex As Integer) As Integer

			' for easy access to these values
			Dim imgWidth As Integer = CInt(Fix(pageFormat.imageableWidth))
			Dim imgHeight As Integer = CInt(Fix(pageFormat.imageableHeight))

			If imgWidth <= 0 Then Throw New PrinterException("Width of printable area is too small.")

			' to pass the page number when formatting the header and footer text
			Dim pageNumber As Object() = {Convert.ToInt32(pageIndex + 1)}

			' fetch the formatted header text, if any
			Dim headerText As String = Nothing
			If headerFormat IsNot Nothing Then headerText = headerFormat.format(pageNumber)

			' fetch the formatted footer text, if any
			Dim footerText As String = Nothing
			If footerFormat IsNot Nothing Then footerText = footerFormat.format(pageNumber)

			' to store the bounds of the header and footer text
			Dim hRect As Rectangle2D = Nothing
			Dim fRect As Rectangle2D = Nothing

			' the amount of vertical space needed for the header and footer text
			Dim headerTextSpace As Integer = 0
			Dim footerTextSpace As Integer = 0

			' the amount of vertical space available for printing the table
			Dim availableSpace As Integer = imgHeight

			' if there's header text, find out how much space is needed for it
			' and subtract that from the available space
			If headerText IsNot Nothing Then
				graphics.font = headerFont
				hRect = graphics.fontMetrics.getStringBounds(headerText, graphics)

				headerTextSpace = CInt(Fix(Math.Ceiling(hRect.height)))
				availableSpace -= headerTextSpace + H_F_SPACE
			End If

			' if there's footer text, find out how much space is needed for it
			' and subtract that from the available space
			If footerText IsNot Nothing Then
				graphics.font = footerFont
				fRect = graphics.fontMetrics.getStringBounds(footerText, graphics)

				footerTextSpace = CInt(Fix(Math.Ceiling(fRect.height)))
				availableSpace -= footerTextSpace + H_F_SPACE
			End If

			If availableSpace <= 0 Then Throw New PrinterException("Height of printable area is too small.")

			' depending on the print mode, we may need a scale factor to
			' fit the table's entire width on the page
			Dim sf As Double = 1.0R
			If printMode = JTable.PrintMode.FIT_WIDTH AndAlso totalColWidth > imgWidth Then

				' if not, we would have thrown an acception previously
				Debug.Assert(imgWidth > 0)

				' it must be, according to the if-condition, since imgWidth > 0
				Debug.Assert(totalColWidth > 1)

				sf = CDbl(imgWidth) / CDbl(totalColWidth)
			End If

			' dictated by the previous two assertions
			Debug.Assert(sf > 0)

			' This is in a loop for two reasons:
			' First, it allows us to catch up in case we're called starting
			' with a non-zero pageIndex. Second, we know that we can be called
			' for the same page multiple times. The condition of this while
			' loop acts as a check, ensuring that we don't attempt to do the
			' calculations again when we are called subsequent times for the
			' same page.
			Do While last < pageIndex
				' if we are finished all columns in all rows
				If row >= table.rowCount AndAlso col = 0 Then Return NO_SUCH_PAGE

				' rather than multiplying every row and column by the scale factor
				' in findNextClip, just pass a width and height that have already
				' been divided by it
				Dim scaledWidth As Integer = CInt(Fix(imgWidth / sf))
				Dim scaledHeight As Integer = CInt(Fix((availableSpace - hclip.height) / sf))

				' calculate the area of the table to be printed for this page
				findNextClip(scaledWidth, scaledHeight)

				last += 1
			Loop

			' create a copy of the graphics so we don't affect the one given to us
			Dim g2d As Graphics2D = CType(graphics.create(), Graphics2D)

			' translate into the co-ordinate system of the pageFormat
			g2d.translate(pageFormat.imageableX, pageFormat.imageableY)

			' to save and store the transform
			Dim oldTrans As AffineTransform

			' if there's footer text, print it at the bottom of the imageable area
			If footerText IsNot Nothing Then
				oldTrans = g2d.transform

				g2d.translate(0, imgHeight - footerTextSpace)

				printText(g2d, footerText, fRect, footerFont, imgWidth)

				g2d.transform = oldTrans
			End If

			' if there's header text, print it at the top of the imageable area
			' and then translate downwards
			If headerText IsNot Nothing Then
				printText(g2d, headerText, hRect, headerFont, imgWidth)

				g2d.translate(0, headerTextSpace + H_F_SPACE)
			End If

			' constrain the table output to the available space
			tempRect.x = 0
			tempRect.y = 0
			tempRect.width = imgWidth
			tempRect.height = availableSpace
			g2d.clip(tempRect)

			' if we have a scale factor, scale the graphics object to fit
			' the entire width
			If sf <> 1.0R Then
				g2d.scale(sf, sf)

			' otherwise, ensure that the current portion of the table is
			' centered horizontally
			Else
				Dim diff As Integer = (imgWidth - clip.width) / 2
				g2d.translate(diff, 0)
			End If

			' store the old transform and clip for later restoration
			oldTrans = g2d.transform
			Dim oldClip As Shape = g2d.clip

			' if there's a table header, print the current section and
			' then translate downwards
			If header IsNot Nothing Then
				hclip.x = clip.x
				hclip.width = clip.width

				g2d.translate(-hclip.x, 0)
				g2d.clip(hclip)
				header.print(g2d)

				' restore the original transform and clip
				g2d.transform = oldTrans
				g2d.clip = oldClip

				' translate downwards
				g2d.translate(0, hclip.height)
			End If

			' print the current section of the table
			g2d.translate(-clip.x, -clip.y)
			g2d.clip(clip)
			table.print(g2d)

			' restore the original transform and clip
			g2d.transform = oldTrans
			g2d.clip = oldClip

			' draw a box around the table
			g2d.color = Color.BLACK
			g2d.drawRect(0, 0, clip.width, hclip.height + clip.height)

			' dispose the graphics copy
			g2d.Dispose()

			Return PAGE_EXISTS
		End Function

		''' <summary>
		''' A helper method that encapsulates common code for rendering the
		''' header and footer text.
		''' </summary>
		''' <param name="g2d">       the graphics to draw into </param>
		''' <param name="text">      the text to draw, non null </param>
		''' <param name="rect">      the bounding rectangle for this text,
		'''                   as calculated at the given font, non null </param>
		''' <param name="font">      the font to draw the text in, non null </param>
		''' <param name="imgWidth">  the width of the area to draw into </param>
		Private Sub printText(ByVal g2d As Graphics2D, ByVal text As String, ByVal rect As Rectangle2D, ByVal font As Font, ByVal imgWidth As Integer)

				Dim tx As Integer

				' if the text is small enough to fit, center it
				If rect.width < imgWidth Then
					tx = CInt(Fix((imgWidth - rect.width) / 2))

				' otherwise, if the table is LTR, ensure the left side of
				' the text shows; the right can be clipped
				ElseIf table.componentOrientation.leftToRight Then
					tx = 0

				' otherwise, ensure the right side of the text shows
				Else
					tx = -CInt(Fix(Math.Ceiling(rect.width) - imgWidth))
				End If

				Dim ty As Integer = CInt(Fix(Math.Ceiling(Math.Abs(rect.y))))
				g2d.color = Color.BLACK
				g2d.font = font
				g2d.drawString(text, tx, ty)
		End Sub

		''' <summary>
		''' Calculate the area of the table to be printed for
		''' the next page. This should only be called if there
		''' are rows and columns left to print.
		''' 
		''' To avoid an infinite loop in printing, this will
		''' always put at least one cell on each page.
		''' </summary>
		''' <param name="pw">  the width of the area to print in </param>
		''' <param name="ph">  the height of the area to print in </param>
		Private Sub findNextClip(ByVal pw As Integer, ByVal ph As Integer)
			Dim ltr As Boolean = table.componentOrientation.leftToRight

			' if we're ready to start a new set of rows
			If col = 0 Then
				If ltr Then
					' adjust clip to the left of the first column
					clip.x = 0
				Else
					' adjust clip to the right of the first column
					clip.x = totalColWidth
				End If

				' adjust clip to the top of the next set of rows
				clip.y += clip.height

				' adjust clip width and height to be zero
				clip.width = 0
				clip.height = 0

				' fit as many rows as possible, and at least one
				Dim rowCount As Integer = table.rowCount
				Dim rowHeight As Integer = table.getRowHeight(row)
				Do
					clip.height += rowHeight

					row += 1
					If row >= rowCount Then Exit Do

					rowHeight = table.getRowHeight(row)
				Loop While clip.height + rowHeight <= ph
			End If

			' we can short-circuit for JTable.PrintMode.FIT_WIDTH since
			' we'll always fit all columns on the page
			If printMode = JTable.PrintMode.FIT_WIDTH Then
				clip.x = 0
				clip.width = totalColWidth
				Return
			End If

			If ltr Then clip.x += clip.width

			' adjust clip width to be zero
			clip.width = 0

			' fit as many columns as possible, and at least one
			Dim colCount As Integer = table.columnCount
			Dim colWidth As Integer = colModel.getColumn(col).width
			Do
				clip.width += colWidth
				If Not ltr Then clip.x -= colWidth

				col += 1
				If col >= colCount Then
					' reset col to 0 to indicate we're finished all columns
					col = 0

					Exit Do
				End If

				colWidth = colModel.getColumn(col).width
			Loop While clip.width + colWidth <= pw

		End Sub

	End Class

End Namespace
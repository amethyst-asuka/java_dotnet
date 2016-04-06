Imports System.Collections

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

Namespace java.awt.print


	''' <summary>
	''' The <code>Book</code> class provides a representation of a document in
	''' which pages may have different page formats and page painters. This
	''' class uses the <seealso cref="Pageable"/> interface to interact with a
	''' <seealso cref="PrinterJob"/>. </summary>
	''' <seealso cref= Pageable </seealso>
	''' <seealso cref= PrinterJob </seealso>

	Public Class Book
		Implements Pageable

	 ' Class Constants 

	 ' Class Variables 

	 ' Instance Variables 

		''' <summary>
		''' The set of pages that make up the Book.
		''' </summary>
		Private mPages As ArrayList

	 ' Instance Methods 

		''' <summary>
		'''  Creates a new, empty <code>Book</code>.
		''' </summary>
		Public Sub New()
			mPages = New ArrayList
		End Sub

		''' <summary>
		''' Returns the number of pages in this <code>Book</code>. </summary>
		''' <returns> the number of pages this <code>Book</code> contains. </returns>
		Public Overridable Property numberOfPages As Integer Implements Pageable.getNumberOfPages
			Get
				Return mPages.Count
			End Get
		End Property

		''' <summary>
		''' Returns the <seealso cref="PageFormat"/> of the page specified by
		''' <code>pageIndex</code>. </summary>
		''' <param name="pageIndex"> the zero based index of the page whose
		'''            <code>PageFormat</code> is being requested </param>
		''' <returns> the <code>PageFormat</code> describing the size and
		'''          orientation of the page. </returns>
		''' <exception cref="IndexOutOfBoundsException"> if the <code>Pageable</code>
		'''          does not contain the requested page </exception>
		Public Overridable Function getPageFormat(  pageIndex As Integer) As PageFormat Implements Pageable.getPageFormat
			Return getPage(pageIndex).pageFormat
		End Function

		''' <summary>
		''' Returns the <seealso cref="Printable"/> instance responsible for rendering
		''' the page specified by <code>pageIndex</code>. </summary>
		''' <param name="pageIndex"> the zero based index of the page whose
		'''                  <code>Printable</code> is being requested </param>
		''' <returns> the <code>Printable</code> that renders the page. </returns>
		''' <exception cref="IndexOutOfBoundsException"> if the <code>Pageable</code>
		'''            does not contain the requested page </exception>
		Public Overridable Function getPrintable(  pageIndex As Integer) As Printable Implements Pageable.getPrintable
			Return getPage(pageIndex).printable
		End Function

		''' <summary>
		''' Sets the <code>PageFormat</code> and the <code>Painter</code> for a
		''' specified page number. </summary>
		''' <param name="pageIndex"> the zero based index of the page whose
		'''                  painter and format is altered </param>
		''' <param name="painter">   the <code>Printable</code> instance that
		'''                  renders the page </param>
		''' <param name="page">      the size and orientation of the page </param>
		''' <exception cref="IndexOutOfBoundsException"> if the specified
		'''          page is not already in this <code>Book</code> </exception>
		''' <exception cref="NullPointerException"> if the <code>painter</code> or
		'''          <code>page</code> argument is <code>null</code> </exception>
		Public Overridable Sub setPage(  pageIndex As Integer,   painter As Printable,   page As PageFormat)
			If painter Is Nothing Then Throw New NullPointerException("painter is null")

			If page Is Nothing Then Throw New NullPointerException("page is null")

			mPages(pageIndex) = New BookPage(Me, painter, page)
		End Sub

		''' <summary>
		''' Appends a single page to the end of this <code>Book</code>. </summary>
		''' <param name="painter">   the <code>Printable</code> instance that
		'''                  renders the page </param>
		''' <param name="page">      the size and orientation of the page </param>
		''' <exception cref="NullPointerException">
		'''          If the <code>painter</code> or <code>page</code>
		'''          argument is <code>null</code> </exception>
		Public Overridable Sub append(  painter As Printable,   page As PageFormat)
			mPages.Add(New BookPage(Me, painter, page))
		End Sub

		''' <summary>
		''' Appends <code>numPages</code> pages to the end of this
		''' <code>Book</code>.  Each of the pages is associated with
		''' <code>page</code>. </summary>
		''' <param name="painter">   the <code>Printable</code> instance that renders
		'''                  the page </param>
		''' <param name="page">      the size and orientation of the page </param>
		''' <param name="numPages">  the number of pages to be added to the
		'''                  this <code>Book</code>. </param>
		''' <exception cref="NullPointerException">
		'''          If the <code>painter</code> or <code>page</code>
		'''          argument is <code>null</code> </exception>
		Public Overridable Sub append(  painter As Printable,   page As PageFormat,   numPages As Integer)
			Dim bookPage As New BookPage(Me, painter, page)
			Dim pageIndex As Integer = mPages.Count
			Dim newSize As Integer = pageIndex + numPages

			mPages.Capacity = newSize
			For i As Integer = pageIndex To newSize - 1
				mPages(i) = bookPage
			Next i
		End Sub

		''' <summary>
		''' Return the BookPage for the page specified by 'pageIndex'.
		''' </summary>
		Private Function getPage(  pageIndex As Integer) As BookPage
			Return CType(mPages(pageIndex), BookPage)
		End Function

		''' <summary>
		''' The BookPage inner class describes an individual
		''' page in a Book through a PageFormat-Printable pair.
		''' </summary>
		Private Class BookPage
			Private ReadOnly outerInstance As Book

			''' <summary>
			'''  The size and orientation of the page.
			''' </summary>
			Private mFormat As PageFormat

			''' <summary>
			''' The instance that will draw the page.
			''' </summary>
			Private mPainter As Printable

			''' <summary>
			''' A new instance where 'format' describes the page's
			''' size and orientation and 'painter' is the instance
			''' that will draw the page's graphics. </summary>
			''' <exception cref="NullPointerException">
			'''          If the <code>painter</code> or <code>format</code>
			'''          argument is <code>null</code> </exception>
			Friend Sub New(  outerInstance As Book,   painter As Printable,   format As PageFormat)
					Me.outerInstance = outerInstance

				If painter Is Nothing OrElse format Is Nothing Then Throw New NullPointerException

				mFormat = format
				mPainter = painter
			End Sub

			''' <summary>
			''' Return the instance that paints the
			''' page.
			''' </summary>
			Friend Overridable Property printable As Printable
				Get
					Return mPainter
				End Get
			End Property

			''' <summary>
			''' Return the format of the page.
			''' </summary>
			Friend Overridable Property pageFormat As PageFormat
				Get
					Return mFormat
				End Get
			End Property
		End Class
	End Class

End Namespace
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
	''' The <code>PageFormat</code> class describes the size and
	''' orientation of a page to be printed.
	''' </summary>
	Public Class PageFormat
		Implements Cloneable

	 ' Class Constants 

		''' <summary>
		'''  The origin is at the bottom left of the paper with
		'''  x running bottom to top and y running left to right.
		'''  Note that this is not the Macintosh landscape but
		'''  is the Window's and PostScript landscape.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const LANDSCAPE As Integer = 0

		''' <summary>
		'''  The origin is at the top left of the paper with
		'''  x running to the right and y running down the
		'''  paper.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const PORTRAIT As Integer = 1

		''' <summary>
		'''  The origin is at the top right of the paper with x
		'''  running top to bottom and y running right to left.
		'''  Note that this is the Macintosh landscape.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const REVERSE_LANDSCAPE As Integer = 2

	 ' Instance Variables 

		''' <summary>
		''' A description of the physical piece of paper.
		''' </summary>
		Private mPaper As Paper

		''' <summary>
		''' The orientation of the current page. This will be
		''' one of the constants: PORTRIAT, LANDSCAPE, or
		''' REVERSE_LANDSCAPE,
		''' </summary>
		Private mOrientation As Integer = PORTRAIT

	 ' Constructors 

		''' <summary>
		''' Creates a default, portrait-oriented
		''' <code>PageFormat</code>.
		''' </summary>
		Public Sub New()
			mPaper = New Paper
		End Sub

	 ' Instance Methods 

		''' <summary>
		''' Makes a copy of this <code>PageFormat</code> with the same
		''' contents as this <code>PageFormat</code>. </summary>
		''' <returns> a copy of this <code>PageFormat</code>. </returns>
		Public Overridable Function clone() As Object
			Dim newPage As PageFormat

			Try
				newPage = CType(MyBase.clone(), PageFormat)
				newPage.mPaper = CType(mPaper.clone(), Paper)

			Catch e As CloneNotSupportedException
				Console.WriteLine(e.ToString())
				Console.Write(e.StackTrace)
				newPage = Nothing ' should never happen.
			End Try

			Return newPage
		End Function


		''' <summary>
		''' Returns the width, in 1/72nds of an inch, of the page.
		''' This method takes into account the orientation of the
		''' page when determining the width. </summary>
		''' <returns> the width of the page. </returns>
		Public Overridable Property width As Double
			Get
				Dim width_Renamed As Double
				Dim orientation_Renamed As Integer = orientation
    
				If orientation_Renamed = PORTRAIT Then
					width_Renamed = mPaper.width
				Else
					width_Renamed = mPaper.height
				End If
    
				Return width_Renamed
			End Get
		End Property

		''' <summary>
		''' Returns the height, in 1/72nds of an inch, of the page.
		''' This method takes into account the orientation of the
		''' page when determining the height. </summary>
		''' <returns> the height of the page. </returns>
		Public Overridable Property height As Double
			Get
				Dim height_Renamed As Double
				Dim orientation_Renamed As Integer = orientation
    
				If orientation_Renamed = PORTRAIT Then
					height_Renamed = mPaper.height
				Else
					height_Renamed = mPaper.width
				End If
    
				Return height_Renamed
			End Get
		End Property

		''' <summary>
		''' Returns the x coordinate of the upper left point of the
		''' imageable area of the <code>Paper</code> object
		''' associated with this <code>PageFormat</code>.
		''' This method takes into account the
		''' orientation of the page. </summary>
		''' <returns> the x coordinate of the upper left point of the
		''' imageable area of the <code>Paper</code> object
		''' associated with this <code>PageFormat</code>. </returns>
		Public Overridable Property imageableX As Double
			Get
				Dim x As Double
    
				Select Case orientation
    
				Case LANDSCAPE
					x = mPaper.height - (mPaper.imageableY + mPaper.imageableHeight)
    
				Case PORTRAIT
					x = mPaper.imageableX
    
				Case REVERSE_LANDSCAPE
					x = mPaper.imageableY
    
				Case Else
		'             This should never happen since it signifies that the
		'             * PageFormat is in an invalid orientation.
		'             
					Throw New InternalError("unrecognized orientation")
    
				End Select
    
				Return x
			End Get
		End Property

		''' <summary>
		''' Returns the y coordinate of the upper left point of the
		''' imageable area of the <code>Paper</code> object
		''' associated with this <code>PageFormat</code>.
		''' This method takes into account the
		''' orientation of the page. </summary>
		''' <returns> the y coordinate of the upper left point of the
		''' imageable area of the <code>Paper</code> object
		''' associated with this <code>PageFormat</code>. </returns>
		Public Overridable Property imageableY As Double
			Get
				Dim y As Double
    
				Select Case orientation
    
				Case LANDSCAPE
					y = mPaper.imageableX
    
				Case PORTRAIT
					y = mPaper.imageableY
    
				Case REVERSE_LANDSCAPE
					y = mPaper.width - (mPaper.imageableX + mPaper.imageableWidth)
    
				Case Else
		'             This should never happen since it signifies that the
		'             * PageFormat is in an invalid orientation.
		'             
					Throw New InternalError("unrecognized orientation")
    
				End Select
    
				Return y
			End Get
		End Property

		''' <summary>
		''' Returns the width, in 1/72nds of an inch, of the imageable
		''' area of the page. This method takes into account the orientation
		''' of the page. </summary>
		''' <returns> the width of the page. </returns>
		Public Overridable Property imageableWidth As Double
			Get
				Dim width_Renamed As Double
    
				If orientation = PORTRAIT Then
					width_Renamed = mPaper.imageableWidth
				Else
					width_Renamed = mPaper.imageableHeight
				End If
    
				Return width_Renamed
			End Get
		End Property

		''' <summary>
		''' Return the height, in 1/72nds of an inch, of the imageable
		''' area of the page. This method takes into account the orientation
		''' of the page. </summary>
		''' <returns> the height of the page. </returns>
		Public Overridable Property imageableHeight As Double
			Get
				Dim height_Renamed As Double
    
				If orientation = PORTRAIT Then
					height_Renamed = mPaper.imageableHeight
				Else
					height_Renamed = mPaper.imageableWidth
				End If
    
				Return height_Renamed
			End Get
		End Property


		''' <summary>
		''' Returns a copy of the <seealso cref="Paper"/> object associated
		''' with this <code>PageFormat</code>.  Changes made to the
		''' <code>Paper</code> object returned from this method do not
		''' affect the <code>Paper</code> object of this
		''' <code>PageFormat</code>.  To update the <code>Paper</code>
		''' object of this <code>PageFormat</code>, create a new
		''' <code>Paper</code> object and set it into this
		''' <code>PageFormat</code> by using the <seealso cref="#setPaper(Paper)"/>
		''' method. </summary>
		''' <returns> a copy of the <code>Paper</code> object associated
		'''          with this <code>PageFormat</code>. </returns>
		''' <seealso cref= #setPaper </seealso>
		Public Overridable Property paper As Paper
			Get
				Return CType(mPaper.clone(), Paper)
			End Get
			Set(  paper As Paper)
				 mPaper = CType(paper.clone(), Paper)
			 End Set
		End Property


		''' <summary>
		''' Sets the page orientation. <code>orientation</code> must be
		''' one of the constants: PORTRAIT, LANDSCAPE,
		''' or REVERSE_LANDSCAPE. </summary>
		''' <param name="orientation"> the new orientation for the page </param>
		''' <exception cref="IllegalArgumentException"> if
		'''          an unknown orientation was requested </exception>
		''' <seealso cref= #getOrientation </seealso>
		Public Overridable Property orientation As Integer
			Set(  orientation As Integer)
				If 0 <= orientation AndAlso orientation <= REVERSE_LANDSCAPE Then
					mOrientation = orientation
				Else
					Throw New IllegalArgumentException
				End If
			End Set
			Get
				Return mOrientation
			End Get
		End Property


		''' <summary>
		''' Returns a transformation matrix that translates user
		''' space rendering to the requested orientation
		''' of the page.  The values are placed into the
		''' array as
		''' {&nbsp;m00,&nbsp;m10,&nbsp;m01,&nbsp;m11,&nbsp;m02,&nbsp;m12} in
		''' the form required by the <seealso cref="AffineTransform"/>
		''' constructor. </summary>
		''' <returns> the matrix used to translate user space rendering
		''' to the orientation of the page. </returns>
		''' <seealso cref= java.awt.geom.AffineTransform </seealso>
		Public Overridable Property matrix As Double()
			Get
				Dim matrix_Renamed As Double() = New Double(5){}
    
				Select Case mOrientation
    
				Case LANDSCAPE
					matrix_Renamed(0) = 0
					matrix_Renamed(1) = -1
					matrix_Renamed(2) = 1
					matrix_Renamed(3) = 0
					matrix_Renamed(4) = 0
					matrix_Renamed(5) = mPaper.height
    
				Case PORTRAIT
					matrix_Renamed(0) = 1
					matrix_Renamed(1) = 0
					matrix_Renamed(2) = 0
					matrix_Renamed(3) = 1
					matrix_Renamed(4) = 0
					matrix_Renamed(5) = 0
    
				Case REVERSE_LANDSCAPE
					matrix_Renamed(0) = 0
					matrix_Renamed(1) = 1
					matrix_Renamed(2) = -1
					matrix_Renamed(3) = 0
					matrix_Renamed(4) = mPaper.width
					matrix_Renamed(5) = 0
    
				Case Else
					Throw New IllegalArgumentException
				End Select
    
				Return matrix_Renamed
			End Get
		End Property
	End Class

End Namespace
'
' * Copyright (c) 1997, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>Paper</code> class describes the physical characteristics of
	''' a piece of paper.
	''' <p>
	''' When creating a <code>Paper</code> object, it is the application's
	''' responsibility to ensure that the paper size and the imageable area
	''' are compatible.  For example, if the paper size is changed from
	''' 11 x 17 to 8.5 x 11, the application might need to reduce the
	''' imageable area so that whatever is printed fits on the page.
	''' <p> </summary>
	''' <seealso cref= #setSize(double, double) </seealso>
	''' <seealso cref= #setImageableArea(double, double, double, double) </seealso>
	Public Class Paper
		Implements Cloneable

	 ' Private Class Variables 

		Private Const INCH As Integer = 72
		Private Shared ReadOnly LETTER_WIDTH As Double = 8.5 * INCH
		Private Shared ReadOnly LETTER_HEIGHT As Double = 11 * INCH

	 ' Instance Variables 

		''' <summary>
		''' The height of the physical page in 1/72nds
		''' of an inch. The number is stored as a floating
		''' point value rather than as an integer
		''' to facilitate the conversion from metric
		''' units to 1/72nds of an inch and then back.
		''' (This may or may not be a good enough reason
		''' for a float).
		''' </summary>
		Private mHeight As Double

		''' <summary>
		''' The width of the physical page in 1/72nds
		''' of an inch.
		''' </summary>
		Private mWidth As Double

		''' <summary>
		''' The area of the page on which drawing will
		''' be visable. The area outside of this
		''' rectangle but on the Page generally
		''' reflects the printer's hardware margins.
		''' The origin of the physical page is
		''' at (0, 0) with this rectangle provided
		''' in that coordinate system.
		''' </summary>
		Private mImageableArea As java.awt.geom.Rectangle2D

	 ' Constructors 

		''' <summary>
		''' Creates a letter sized piece of paper
		''' with one inch margins.
		''' </summary>
		Public Sub New()
			mHeight = LETTER_HEIGHT
			mWidth = LETTER_WIDTH
			mImageableArea = New java.awt.geom.Rectangle2D.Double(INCH, INCH, mWidth - 2 * INCH, mHeight - 2 * INCH)
		End Sub

	 ' Instance Methods 

		''' <summary>
		''' Creates a copy of this <code>Paper</code> with the same contents
		''' as this <code>Paper</code>. </summary>
		''' <returns> a copy of this <code>Paper</code>. </returns>
		Public Overridable Function clone() As Object

			Dim newPaper As Paper

			Try
	'             It's okay to copy the reference to the imageable
	'             * area into the clone since we always return a copy
	'             * of the imageable area when asked for it.
	'             
				newPaper = CType(MyBase.clone(), Paper)

			Catch e As CloneNotSupportedException
				Console.WriteLine(e.ToString())
				Console.Write(e.StackTrace)
				newPaper = Nothing ' should never happen.
			End Try

			Return newPaper
		End Function

		''' <summary>
		''' Returns the height of the page in 1/72nds of an inch. </summary>
		''' <returns> the height of the page described by this
		'''          <code>Paper</code>. </returns>
		Public Overridable Property height As Double
			Get
				Return mHeight
			End Get
		End Property

		''' <summary>
		''' Sets the width and height of this <code>Paper</code>
		''' object, which represents the properties of the page onto
		''' which printing occurs.
		''' The dimensions are supplied in 1/72nds of
		''' an inch. </summary>
		''' <param name="width"> the value to which to set this <code>Paper</code>
		''' object's width </param>
		''' <param name="height"> the value to which to set this <code>Paper</code>
		''' object's height </param>
		Public Overridable Sub setSize(ByVal width As Double, ByVal height As Double)
			mWidth = width
			mHeight = height
		End Sub

		''' <summary>
		''' Returns the width of the page in 1/72nds
		''' of an inch. </summary>
		''' <returns> the width of the page described by this
		''' <code>Paper</code>. </returns>
		Public Overridable Property width As Double
			Get
				Return mWidth
			End Get
		End Property

		''' <summary>
		''' Sets the imageable area of this <code>Paper</code>.  The
		''' imageable area is the area on the page in which printing
		''' occurs. </summary>
		''' <param name="x"> the X coordinate to which to set the
		''' upper-left corner of the imageable area of this <code>Paper</code> </param>
		''' <param name="y"> the Y coordinate to which to set the
		''' upper-left corner of the imageable area of this <code>Paper</code> </param>
		''' <param name="width"> the value to which to set the width of the
		''' imageable area of this <code>Paper</code> </param>
		''' <param name="height"> the value to which to set the height of the
		''' imageable area of this <code>Paper</code> </param>
		Public Overridable Sub setImageableArea(ByVal x As Double, ByVal y As Double, ByVal width As Double, ByVal height As Double)
			mImageableArea = New java.awt.geom.Rectangle2D.Double(x, y, width,height)
		End Sub

		''' <summary>
		''' Returns the x coordinate of the upper-left corner of this
		''' <code>Paper</code> object's imageable area. </summary>
		''' <returns> the x coordinate of the imageable area. </returns>
		Public Overridable Property imageableX As Double
			Get
				Return mImageableArea.x
			End Get
		End Property

		''' <summary>
		''' Returns the y coordinate of the upper-left corner of this
		''' <code>Paper</code> object's imageable area. </summary>
		''' <returns> the y coordinate of the imageable area. </returns>
		Public Overridable Property imageableY As Double
			Get
				Return mImageableArea.y
			End Get
		End Property

		''' <summary>
		''' Returns the width of this <code>Paper</code> object's imageable
		''' area. </summary>
		''' <returns> the width of the imageable area. </returns>
		Public Overridable Property imageableWidth As Double
			Get
				Return mImageableArea.width
			End Get
		End Property

		''' <summary>
		''' Returns the height of this <code>Paper</code> object's imageable
		''' area. </summary>
		''' <returns> the height of the imageable area. </returns>
		Public Overridable Property imageableHeight As Double
			Get
				Return mImageableArea.height
			End Get
		End Property
	End Class

End Namespace
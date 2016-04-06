Imports Microsoft.VisualBasic

'
' * Copyright (c) 1997, 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.image


	''' <summary>
	''' This class performs an arbitrary linear combination of the bands
	''' in a <CODE>Raster</CODE>, using a specified matrix.
	''' <p>
	''' The width of the matrix must be equal to the number of bands in the
	''' source <CODE>Raster</CODE>, optionally plus one.  If there is one more
	''' column in the matrix than the number of bands, there is an implied 1 at the
	''' end of the vector of band samples representing a pixel.  The height
	''' of the matrix must be equal to the number of bands in the destination.
	''' <p>
	''' For example, a 3-banded <CODE>Raster</CODE> might have the following
	''' transformation applied to each pixel in order to invert the second band of
	''' the <CODE>Raster</CODE>.
	''' <pre>
	'''   [ 1.0   0.0   0.0    0.0  ]     [ b1 ]
	'''   [ 0.0  -1.0   0.0  255.0  ]  x  [ b2 ]
	'''   [ 0.0   0.0   1.0    0.0  ]     [ b3 ]
	'''                                   [ 1 ]
	''' </pre>
	''' 
	''' <p>
	''' Note that the source and destination can be the same object.
	''' </summary>
	Public Class BandCombineOp
		Implements RasterOp

		Friend matrix As Single()()
		Friend nrows As Integer = 0
		Friend ncols As Integer = 0
		Friend hints As java.awt.RenderingHints

		''' <summary>
		''' Constructs a <CODE>BandCombineOp</CODE> with the specified matrix.
		''' The width of the matrix must be equal to the number of bands in
		''' the source <CODE>Raster</CODE>, optionally plus one.  If there is one
		''' more column in the matrix than the number of bands, there is an implied
		''' 1 at the end of the vector of band samples representing a pixel.  The
		''' height of the matrix must be equal to the number of bands in the
		''' destination.
		''' <p>
		''' The first subscript is the row index and the second
		''' is the column index.  This operation uses none of the currently
		''' defined rendering hints; the <CODE>RenderingHints</CODE> argument can be
		''' null.
		''' </summary>
		''' <param name="matrix"> The matrix to use for the band combine operation. </param>
		''' <param name="hints"> The <CODE>RenderingHints</CODE> object for this operation.
		''' Not currently used so it can be null. </param>
		Public Sub New(  matrix As Single()(),   hints As java.awt.RenderingHints)
			nrows = matrix.Length
			ncols = matrix(0).Length
			Me.matrix = New Single(nrows - 1)(){}
			For i As Integer = 0 To nrows - 1
	'             Arrays.copyOf is forgiving of the source array being
	'             * too short, but it is also faster than other cloning
	'             * methods, so we provide our own protection for short
	'             * matrix rows.
	'             
				If ncols > matrix(i).Length Then Throw New IndexOutOfBoundsException("row " & i & " too short")
				Me.matrix(i) = java.util.Arrays.copyOf(matrix(i), ncols)
			Next i
			Me.hints = hints
		End Sub

		''' <summary>
		''' Returns a copy of the linear combination matrix.
		''' </summary>
		''' <returns> The matrix associated with this band combine operation. </returns>
		Public Property matrix As Single()()
			Get
				Dim ret As Single()() = New Single(nrows - 1)(){}
				For i As Integer = 0 To nrows - 1
					ret(i) = java.util.Arrays.copyOf(matrix(i), ncols)
				Next i
				Return ret
			End Get
		End Property

		''' <summary>
		''' Transforms the <CODE>Raster</CODE> using the matrix specified in the
		''' constructor. An <CODE>IllegalArgumentException</CODE> may be thrown if
		''' the number of bands in the source or destination is incompatible with
		''' the matrix.  See the class comments for more details.
		''' <p>
		''' If the destination is null, it will be created with a number of bands
		''' equalling the number of rows in the matrix. No exception is thrown
		''' if the operation causes a data overflow.
		''' </summary>
		''' <param name="src"> The <CODE>Raster</CODE> to be filtered. </param>
		''' <param name="dst"> The <CODE>Raster</CODE> in which to store the results
		''' of the filter operation.
		''' </param>
		''' <returns> The filtered <CODE>Raster</CODE>.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If the number of bands in the
		''' source or destination is incompatible with the matrix. </exception>
		Public Overridable Function filter(  src As Raster,   dst As WritableRaster) As WritableRaster Implements RasterOp.filter
			Dim nBands As Integer = src.numBands
			If ncols <> nBands AndAlso ncols <> (nBands+1) Then Throw New IllegalArgumentException("Number of columns in the " & "matrix (" & ncols & ") must be equal to the number" & " of bands ([+1]) in src (" & nBands & ").")
			If dst Is Nothing Then
				dst = createCompatibleDestRaster(src)
			ElseIf nrows <> dst.numBands Then
				Throw New IllegalArgumentException("Number of rows in the " & "matrix (" & nrows & ") must be equal to the number" & " of bands ([+1]) in dst (" & nBands & ").")
			End If

			If sun.awt.image.ImagingLib.filter(Me, src, dst) IsNot Nothing Then Return dst

			Dim pixel As Integer() = Nothing
			Dim dstPixel As Integer() = New Integer(dst.numBands - 1){}
			Dim accum As Single
			Dim sminX As Integer = src.minX
			Dim sY As Integer = src.minY
			Dim dminX As Integer = dst.minX
			Dim dY As Integer = dst.minY
			Dim sX As Integer
			Dim dX As Integer
			If ncols = nBands Then
				Dim y As Integer=0
				Do While y < src.height
					dX = dminX
					sX = sminX
					Dim x As Integer=0
					Do While x < src.width
						pixel = src.getPixel(sX, sY, pixel)
						For r As Integer = 0 To nrows - 1
							accum = 0.0f
							For c As Integer = 0 To ncols - 1
								accum += matrix(r)(c)*pixel(c)
							Next c
							dstPixel(r) = CInt(Fix(accum))
						Next r
						dst.pixelxel(dX, dY, dstPixel)
						x += 1
						sX += 1
						dX += 1
					Loop
					y += 1
					sY += 1
					dY += 1
				Loop
			Else
				' Need to add constant
				Dim y As Integer=0
				Do While y < src.height
					dX = dminX
					sX = sminX
					Dim x As Integer=0
					Do While x < src.width
						pixel = src.getPixel(sX, sY, pixel)
						For r As Integer = 0 To nrows - 1
							accum = 0.0f
							For c As Integer = 0 To nBands - 1
								accum += matrix(r)(c)*pixel(c)
							Next c
							dstPixel(r) = CInt(Fix(accum+matrix(r)(nBands)))
						Next r
						dst.pixelxel(dX, dY, dstPixel)
						x += 1
						sX += 1
						dX += 1
					Loop
					y += 1
					sY += 1
					dY += 1
				Loop
			End If

			Return dst
		End Function

		''' <summary>
		''' Returns the bounding box of the transformed destination.  Since
		''' this is not a geometric operation, the bounding box is the same for
		''' the source and destination.
		''' An <CODE>IllegalArgumentException</CODE> may be thrown if the number of
		''' bands in the source is incompatible with the matrix.  See
		''' the class comments for more details.
		''' </summary>
		''' <param name="src"> The <CODE>Raster</CODE> to be filtered.
		''' </param>
		''' <returns> The <CODE>Rectangle2D</CODE> representing the destination
		''' image's bounding box.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If the number of bands in the source
		''' is incompatible with the matrix. </exception>
		Public Function getBounds2D(  src As Raster) As java.awt.geom.Rectangle2D Implements RasterOp.getBounds2D
			Return src.bounds
		End Function


		''' <summary>
		''' Creates a zeroed destination <CODE>Raster</CODE> with the correct size
		''' and number of bands.
		''' An <CODE>IllegalArgumentException</CODE> may be thrown if the number of
		''' bands in the source is incompatible with the matrix.  See
		''' the class comments for more details.
		''' </summary>
		''' <param name="src"> The <CODE>Raster</CODE> to be filtered.
		''' </param>
		''' <returns> The zeroed destination <CODE>Raster</CODE>. </returns>
		Public Overridable Function createCompatibleDestRaster(  src As Raster) As WritableRaster Implements RasterOp.createCompatibleDestRaster
			Dim nBands As Integer = src.numBands
			If (ncols <> nBands) AndAlso (ncols <> (nBands+1)) Then Throw New IllegalArgumentException("Number of columns in the " & "matrix (" & ncols & ") must be equal to the number" & " of bands ([+1]) in src (" & nBands & ").")
			If src.numBands = nrows Then
				Return src.createCompatibleWritableRaster()
			Else
				Throw New IllegalArgumentException("Don't know how to create a " & " compatible Raster with " & nrows & " bands.")
			End If
		End Function

		''' <summary>
		''' Returns the location of the corresponding destination point given a
		''' point in the source <CODE>Raster</CODE>.  If <CODE>dstPt</CODE> is
		''' specified, it is used to hold the return value.
		''' Since this is not a geometric operation, the point returned
		''' is the same as the specified <CODE>srcPt</CODE>.
		''' </summary>
		''' <param name="srcPt"> The <code>Point2D</code> that represents the point in
		'''              the source <code>Raster</code> </param>
		''' <param name="dstPt"> The <CODE>Point2D</CODE> in which to store the result.
		''' </param>
		''' <returns> The <CODE>Point2D</CODE> in the destination image that
		''' corresponds to the specified point in the source image. </returns>
		Public Function getPoint2D(  srcPt As java.awt.geom.Point2D,   dstPt As java.awt.geom.Point2D) As java.awt.geom.Point2D Implements RasterOp.getPoint2D
			If dstPt Is Nothing Then dstPt = New java.awt.geom.Point2D.Float
			dstPt.locationion(srcPt.x, srcPt.y)

			Return dstPt
		End Function

		''' <summary>
		''' Returns the rendering hints for this operation.
		''' </summary>
		''' <returns> The <CODE>RenderingHints</CODE> object associated with this
		''' operation.  Returns null if no hints have been set. </returns>
		Public Property renderingHints As java.awt.RenderingHints Implements RasterOp.getRenderingHints
			Get
				Return hints
			End Get
		End Property
	End Class

End Namespace
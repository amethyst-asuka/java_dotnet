Imports System.Collections.Generic

'
' * Copyright (c) 1995, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' An ImageFilter class for cropping images.
	''' This class extends the basic ImageFilter Class to extract a given
	''' rectangular region of an existing Image and provide a source for a
	''' new image containing just the extracted region.  It is meant to
	''' be used in conjunction with a FilteredImageSource object to produce
	''' cropped versions of existing images.
	''' </summary>
	''' <seealso cref= FilteredImageSource </seealso>
	''' <seealso cref= ImageFilter
	''' 
	''' @author      Jim Graham </seealso>
	Public Class CropImageFilter
		Inherits ImageFilter

		Friend cropX As Integer
		Friend cropY As Integer
		Friend cropW As Integer
		Friend cropH As Integer

		''' <summary>
		''' Constructs a CropImageFilter that extracts the absolute rectangular
		''' region of pixels from its source Image as specified by the x, y,
		''' w, and h parameters. </summary>
		''' <param name="x"> the x location of the top of the rectangle to be extracted </param>
		''' <param name="y"> the y location of the top of the rectangle to be extracted </param>
		''' <param name="w"> the width of the rectangle to be extracted </param>
		''' <param name="h"> the height of the rectangle to be extracted </param>
		Public Sub New(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			cropX = x
			cropY = y
			cropW = w
			cropH = h
		End Sub

		''' <summary>
		''' Passes along  the properties from the source object after adding a
		''' property indicating the cropped region.
		''' This method invokes <code>super.setProperties</code>,
		''' which might result in additional properties being added.
		''' <p>
		''' Note: This method is intended to be called by the
		''' <code>ImageProducer</code> of the <code>Image</code> whose pixels
		''' are being filtered. Developers using
		''' this class to filter pixels from an image should avoid calling
		''' this method directly since that operation could interfere
		''' with the filtering operation.
		''' </summary>
		Public Overrides Property properties(Of T1) As Dictionary(Of T1)
			Set(ByVal props As Dictionary(Of T1))
				Dim p As Dictionary(Of Object, Object) = CType(props.clone(), Dictionary(Of Object, Object))
				p("croprect") = New java.awt.Rectangle(cropX, cropY, cropW, cropH)
				MyBase.properties = p
			End Set
		End Property

		''' <summary>
		''' Override the source image's dimensions and pass the dimensions
		''' of the rectangular cropped region to the ImageConsumer.
		''' <p>
		''' Note: This method is intended to be called by the
		''' <code>ImageProducer</code> of the <code>Image</code> whose
		''' pixels are being filtered. Developers using
		''' this class to filter pixels from an image should avoid calling
		''' this method directly since that operation could interfere
		''' with the filtering operation. </summary>
		''' <seealso cref= ImageConsumer </seealso>
		Public Overrides Sub setDimensions(ByVal w As Integer, ByVal h As Integer)
			consumer.dimensionsons(cropW, cropH)
		End Sub

		''' <summary>
		''' Determine whether the delivered byte pixels intersect the region to
		''' be extracted and passes through only that subset of pixels that
		''' appear in the output region.
		''' <p>
		''' Note: This method is intended to be called by the
		''' <code>ImageProducer</code> of the <code>Image</code> whose
		''' pixels are being filtered. Developers using
		''' this class to filter pixels from an image should avoid calling
		''' this method directly since that operation could interfere
		''' with the filtering operation.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public void setPixels(int x, int y, int w, int h, java.awt.image.ColorModel model, byte pixels() , int off, int scansize)
			Dim x1 As Integer = x
			If x1 < cropX Then x1 = cropX
		Dim x2 As Integer = addWithoutOverflow(x, w)
			If x2 > cropX + cropW Then x2 = cropX + cropW
			Dim y1 As Integer = y
			If y1 < cropY Then y1 = cropY

		Dim y2 As Integer = addWithoutOverflow(y, h)
			If y2 > cropY + cropH Then y2 = cropY + cropH
			If x1 >= x2 OrElse y1 >= y2 Then Return
			consumer.pixelsels(x1 - cropX, y1 - cropY, (x2 - x1), (y2 - y1), model, pixels, off + (y1 - y) * scansize + (x1 - x), scansize)

		''' <summary>
		''' Determine if the delivered int pixels intersect the region to
		''' be extracted and pass through only that subset of pixels that
		''' appear in the output region.
		''' <p>
		''' Note: This method is intended to be called by the
		''' <code>ImageProducer</code> of the <code>Image</code> whose
		''' pixels are being filtered. Developers using
		''' this class to filter pixels from an image should avoid calling
		''' this method directly since that operation could interfere
		''' with the filtering operation.
		''' </summary>
		public void pixelsels(Integer x, Integer y, Integer w, Integer h, java.awt.image.ColorModel model, Integer pixels() , Integer off, Integer scansize)
			Dim x1 As Integer = x
			If x1 < cropX Then x1 = cropX
		Dim x2 As Integer = addWithoutOverflow(x, w)
			If x2 > cropX + cropW Then x2 = cropX + cropW
			Dim y1 As Integer = y
			If y1 < cropY Then y1 = cropY

		Dim y2 As Integer = addWithoutOverflow(y, h)
			If y2 > cropY + cropH Then y2 = cropY + cropH
			If x1 >= x2 OrElse y1 >= y2 Then Return
			consumer.pixelsels(x1 - cropX, y1 - cropY, (x2 - x1), (y2 - y1), model, pixels, off + (y1 - y) * scansize + (x1 - x), scansize)

		'check for potential overflow (see bug 4801285)
		private Integer addWithoutOverflow(Integer x, Integer w)
			Dim x2 As Integer = x + w
			If x > 0 AndAlso w > 0 AndAlso x2 < 0 Then
				x2 = Integer.MaxValue
			ElseIf x < 0 AndAlso w < 0 AndAlso x2 > 0 Then
				x2 = Integer.MinValue
			End If
			Return x2
	End Class

End Namespace
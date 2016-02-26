Imports Microsoft.VisualBasic

'
' * Copyright (c) 1997, 1998, Oracle and/or its affiliates. All rights reserved.
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
	''' An image filter that "disables" an image by turning
	''' it into a grayscale image, and brightening the pixels
	''' in the image. Used by buttons to create an image for
	''' a disabled button.
	''' 
	''' @author      Jeff Dinkins
	''' @author      Tom Ball
	''' @author      Jim Graham
	''' </summary>
	Public Class GrayFilter
		Inherits RGBImageFilter

		Private brighter As Boolean
		Private percent As Integer

		''' <summary>
		''' Creates a disabled image
		''' </summary>
		Public Shared Function createDisabledImage(ByVal i As Image) As Image
			Dim filter As New GrayFilter(True, 50)
			Dim prod As ImageProducer = New FilteredImageSource(i.source, filter)
			Dim grayImage As Image = Toolkit.defaultToolkit.createImage(prod)
			Return grayImage
		End Function

		''' <summary>
		''' Constructs a GrayFilter object that filters a color image to a
		''' grayscale image. Used by buttons to create disabled ("grayed out")
		''' button images.
		''' </summary>
		''' <param name="b">  a boolean -- true if the pixels should be brightened </param>
		''' <param name="p">  an int in the range 0..100 that determines the percentage
		'''           of gray, where 100 is the darkest gray, and 0 is the lightest </param>
		Public Sub New(ByVal b As Boolean, ByVal p As Integer)
			brighter = b
			percent = p

			' canFilterIndexColorModel indicates whether or not it is acceptable
			' to apply the color filtering of the filterRGB method to the color
			' table entries of an IndexColorModel object in lieu of pixel by pixel
			' filtering.
			canFilterIndexColorModel = True
		End Sub

		''' <summary>
		''' Overrides <code>RGBImageFilter.filterRGB</code>.
		''' </summary>
		Public Overridable Function filterRGB(ByVal x As Integer, ByVal y As Integer, ByVal rgb As Integer) As Integer
			' Use NTSC conversion formula.
			Dim gray As Integer = CInt(Fix((0.30 * ((rgb >> 16) And &Hff) + 0.59 * ((rgb >> 8) And &Hff) + 0.11 * (rgb And &Hff)) / 3))

			If brighter Then
				gray = (255 - ((255 - gray) * (100 - percent) \ 100))
			Else
				gray = (gray * (100 - percent) \ 100)
			End If

			If gray < 0 Then gray = 0
			If gray > 255 Then gray = 255
			Return (rgb And &Hff000000L) Or (gray << 16) Or (gray << 8) Or (gray << 0)
		End Function
	End Class

End Namespace
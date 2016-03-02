Imports System.Collections.Generic

'
' * Copyright (c) 1996, 2004, Oracle and/or its affiliates. All rights reserved.
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
    ''' An ImageFilter class for scaling images using the simplest algorithm.
    ''' This class extends the basic ImageFilter Class to scale an existing
    ''' image and provide a source for a new image containing the resampled
    ''' image.  The pixels in the source image are sampled to produce pixels
    ''' for an image of the specified size by replicating rows and columns of
    ''' pixels to scale up or omitting rows and columns of pixels to scale
    ''' down.
    ''' <p>It is meant to be used in conjunction with a FilteredImageSource
    ''' object to produce scaled versions of existing images.  Due to
    ''' implementation dependencies, there may be differences in pixel values
    ''' of an image filtered on different platforms.
    ''' </summary>
    ''' <seealso cref= FilteredImageSource </seealso>
    ''' <seealso cref= ImageFilter
    ''' 
    ''' @author      Jim Graham </seealso>
    Public Class ReplicateScaleFilter
        Inherits ImageFilter

        ''' <summary>
        ''' The width of the source image.
        ''' </summary>
        Protected Friend srcWidth As Integer

        ''' <summary>
        ''' The height of the source image.
        ''' </summary>
        Protected Friend srcHeight As Integer

        ''' <summary>
        ''' The target width to scale the image.
        ''' </summary>
        Protected Friend destWidth As Integer

        ''' <summary>
        ''' The target height to scale the image.
        ''' </summary>
        Protected Friend destHeight As Integer

        ''' <summary>
        ''' An <code>int</code> array containing information about a
        ''' row of pixels.
        ''' </summary>
        Protected Friend srcrows As Integer()

        ''' <summary>
        ''' An <code>int</code> array containing information about a
        ''' column of pixels.
        ''' </summary>
        Protected Friend srccols As Integer()

        ''' <summary>
        ''' A <code>byte</code> array initialized with a size of
        ''' <seealso cref="#destWidth"/> and used to deliver a row of pixel
        ''' data to the <seealso cref="ImageConsumer"/>.
        ''' </summary>
        Protected Friend outpixbuf As Object

        ''' <summary>
        ''' Constructs a ReplicateScaleFilter that scales the pixels from
        ''' its source Image as specified by the width and height parameters. </summary>
        ''' <param name="width"> the target width to scale the image </param>
        ''' <param name="height"> the target height to scale the image </param>
        ''' <exception cref="IllegalArgumentException"> if <code>width</code> equals
        '''         zero or <code>height</code> equals zero </exception>
        Public Sub New(ByVal width As Integer, ByVal height As Integer)
            If width = 0 OrElse height = 0 Then Throw New IllegalArgumentException("Width (" & width & ") and height (" & height & ") must be non-zero")
            destWidth = width
            destHeight = height
        End Sub

        ''' <summary>
        ''' Passes along the properties from the source object after adding a
        ''' property indicating the scale applied.
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
                Dim key As String = "rescale"
                Dim val As String = destWidth & "x" & destHeight
                Dim o As Object = p(key)
                If o IsNot Nothing AndAlso TypeOf o Is String Then val = (CStr(o)) & ", " & val
                p(key) = val
                MyBase.properties = p
            End Set
        End Property

        ''' <summary>
        ''' Override the dimensions of the source image and pass the dimensions
        ''' of the new scaled size to the ImageConsumer.
        ''' <p>
        ''' Note: This method is intended to be called by the
        ''' <code>ImageProducer</code> of the <code>Image</code> whose pixels
        ''' are being filtered. Developers using
        ''' this class to filter pixels from an image should avoid calling
        ''' this method directly since that operation could interfere
        ''' with the filtering operation. </summary>
        ''' <seealso cref= ImageConsumer </seealso>
        Public Overrides Sub setDimensions(ByVal w As Integer, ByVal h As Integer)
            srcWidth = w
            srcHeight = h
            If destWidth < 0 Then
                If destHeight < 0 Then
                    destWidth = srcWidth
                    destHeight = srcHeight
                Else
                    destWidth = srcWidth * destHeight \ srcHeight
                End If
            ElseIf destHeight < 0 Then
                destHeight = srcHeight * destWidth \ srcWidth
            End If
            consumer.dimensionsons(destWidth, destHeight)
        End Sub

        Private Sub calculateMaps()
            srcrows = New Integer(destHeight) {}
            For y As Integer = 0 To destHeight
                srcrows(y) = (2 * y * srcHeight + srcHeight) \ (2 * destHeight)
            Next y
            srccols = New Integer(destWidth) {}
            For x As Integer = 0 To destWidth
                srccols(x) = (2 * x * srcWidth + srcWidth) \ (2 * destWidth)
            Next x
        End Sub

        ''' <summary>
        ''' Choose which rows and columns of the delivered byte pixels are
        ''' needed for the destination scaled image and pass through just
        ''' those rows and columns that are needed, replicated as necessary.
        ''' <p>
        ''' Note: This method is intended to be called by the
        ''' <code>ImageProducer</code> of the <code>Image</code> whose pixels
        ''' are being filtered. Developers using
        ''' this class to filter pixels from an image should avoid calling
        ''' this method directly since that operation could interfere
        ''' with the filtering operation.
        ''' </summary>
        Public Sub setPixels(x As Integer, y As Integer, w As Integer, h As Integer, model As ColorModel, pixels() As Byte, off As Integer, scansize As Integer)
            If srcrows Is Nothing OrElse srccols Is Nothing Then calculateMaps()
            Dim sx, sy As Integer
            Dim dx1 As Integer = (2 * x * destWidth + srcWidth - 1) \ (2 * srcWidth)
            Dim dy1 As Integer = (2 * y * destHeight + srcHeight - 1) \ (2 * srcHeight)
            Dim outpix As SByte()
            If outpixbuf IsNot Nothing AndAlso TypeOf outpixbuf Is SByte() Then
                outpix = CType(outpixbuf, SByte())
            Else
                outpix = New SByte(destWidth - 1) {}
                outpixbuf = outpix
            End If
            Dim dy As Integer = dy1
            'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
            Do While (sy = srcrows(dy)) < y + h
                Dim srcoff As Integer = off + scansize * (sy - y)
                Dim dx As Integer
                dx = dx1
                'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
                Do While (sx = srccols(dx)) < x + w
                    outpix(dx) = pixels(srcoff + sx - x)
                    dx += 1
                Loop
                If dx > dx1 Then consumer.pixelsels(dx1, dy, dx - dx1, 1, model, outpix, dx1, destWidth)
                dy += 1
            Loop
        End Sub

        ''' <summary>
        ''' Choose which rows and columns of the delivered int pixels are
        ''' needed for the destination scaled image and pass through just
        ''' those rows and columns that are needed, replicated as necessary.
        ''' <p>
        ''' Note: This method is intended to be called by the
        ''' <code>ImageProducer</code> of the <code>Image</code> whose pixels
        ''' are being filtered. Developers using
        ''' this class to filter pixels from an image should avoid calling
        ''' this method directly since that operation could interfere
        ''' with the filtering operation.
        ''' </summary>
        Public Sub pixelsels(x As Integer, y As Integer, w As Integer, h As Integer, model As ColorModel, pixels() As Integer, off As Integer, scansize As Integer)
            If srcrows Is Nothing OrElse srccols Is Nothing Then calculateMaps()
            Dim sx, sy As Integer
            Dim dx1 As Integer = (2 * x * destWidth + srcWidth - 1) \ (2 * srcWidth)
            Dim dy1 As Integer = (2 * y * destHeight + srcHeight - 1) \ (2 * srcHeight)
            Dim outpix As Integer()
            If outpixbuf IsNot Nothing AndAlso TypeOf outpixbuf Is Integer() Then
                outpix = CType(outpixbuf, Integer())
            Else
                outpix = New Integer(destWidth - 1) {}
                outpixbuf = outpix
            End If
            Dim dy As Integer = dy1
            'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
            Do While (sy = srcrows(dy)) < y + h
                Dim srcoff As Integer = off + scansize * (sy - y)
                Dim dx As Integer
                dx = dx1
                'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
                Do While (sx = srccols(dx)) < x + w
                    outpix(dx) = pixels(srcoff + sx - x)
                    dx += 1
                Loop
                If dx > dx1 Then consumer.pixelsels(dx1, dy, dx - dx1, 1, model, outpix, dx1, destWidth)
                dy += 1
            Loop
        End Sub
    End Class
End Namespace
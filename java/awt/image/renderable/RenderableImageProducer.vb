Imports System.Runtime.CompilerServices
Imports System.Collections

'
' * Copyright (c) 1998, 2008, Oracle and/or its affiliates. All rights reserved.
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

' ********************************************************************
' **********************************************************************
' **********************************************************************
' *** COPYRIGHT (c) Eastman Kodak Company, 1997                      ***
' *** As  an unpublished  work pursuant to Title 17 of the United    ***
' *** States Code.  All rights reserved.                             ***
' **********************************************************************
' **********************************************************************
' *********************************************************************

Namespace java.awt.image.renderable

	''' <summary>
	''' An adapter class that implements ImageProducer to allow the
	''' asynchronous production of a RenderableImage.  The size of the
	''' ImageConsumer is determined by the scale factor of the usr2dev
	''' transform in the RenderContext.  If the RenderContext is null, the
	''' default rendering of the RenderableImage is used.  This class
	''' implements an asynchronous production that produces the image in
	''' one thread at one resolution.  This class may be subclassed to
	''' implement versions that will render the image using several
	''' threads.  These threads could render either the same image at
	''' progressively better quality, or different sections of the image at
	''' a single resolution.
	''' </summary>
	Public Class RenderableImageProducer
		Implements java.awt.image.ImageProducer, Runnable

		''' <summary>
		''' The RenderableImage source for the producer. </summary>
		Friend rdblImage As RenderableImage

		''' <summary>
		''' The RenderContext to use for producing the image. </summary>
		Friend rc As RenderContext

		''' <summary>
		''' A Vector of image consumers. </summary>
		Friend ics As New ArrayList

		''' <summary>
		''' Constructs a new RenderableImageProducer from a RenderableImage
		''' and a RenderContext.
		''' </summary>
		''' <param name="rdblImage"> the RenderableImage to be rendered. </param>
		''' <param name="rc"> the RenderContext to use for producing the pixels. </param>
		Public Sub New(  rdblImage As RenderableImage,   rc As RenderContext)
			Me.rdblImage = rdblImage
			Me.rc = rc
		End Sub

		''' <summary>
		''' Sets a new RenderContext to use for the next startProduction() call.
		''' </summary>
		''' <param name="rc"> the new RenderContext. </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property renderContext As RenderContext
			Set(  rc As RenderContext)
				Me.rc = rc
			End Set
		End Property

	   ''' <summary>
	   ''' Adds an ImageConsumer to the list of consumers interested in
	   ''' data for this image.
	   ''' </summary>
	   ''' <param name="ic"> an ImageConsumer to be added to the interest list. </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addConsumer(  ic As java.awt.image.ImageConsumer)
			If Not ics.Contains(ic) Then ics.Add(ic)
		End Sub

		''' <summary>
		''' Determine if an ImageConsumer is on the list of consumers
		''' currently interested in data for this image.
		''' </summary>
		''' <param name="ic"> the ImageConsumer to be checked. </param>
		''' <returns> true if the ImageConsumer is on the list; false otherwise. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function isConsumer(  ic As java.awt.image.ImageConsumer) As Boolean
			Return ics.Contains(ic)
		End Function

		''' <summary>
		''' Remove an ImageConsumer from the list of consumers interested in
		''' data for this image.
		''' </summary>
		''' <param name="ic"> the ImageConsumer to be removed. </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeConsumer(  ic As java.awt.image.ImageConsumer)
			ics.Remove(ic)
		End Sub

		''' <summary>
		''' Adds an ImageConsumer to the list of consumers interested in
		''' data for this image, and immediately starts delivery of the
		''' image data through the ImageConsumer interface.
		''' </summary>
		''' <param name="ic"> the ImageConsumer to be added to the list of consumers. </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub startProduction(  ic As java.awt.image.ImageConsumer)
			addConsumer(ic)
			' Need to build a runnable object for the Thread.
			Dim thread_Renamed As New Thread(Me, "RenderableImageProducer Thread")
			thread_Renamed.start()
		End Sub

		''' <summary>
		''' Requests that a given ImageConsumer have the image data delivered
		''' one more time in top-down, left-right order.
		''' </summary>
		''' <param name="ic"> the ImageConsumer requesting the resend. </param>
		Public Overridable Sub requestTopDownLeftRightResend(  ic As java.awt.image.ImageConsumer)
			' So far, all pixels are already sent in TDLR order
		End Sub

		''' <summary>
		''' The runnable method for this class. This will produce an image using
		''' the current RenderableImage and RenderContext and send it to all the
		''' ImageConsumer currently registered with this class.
		''' </summary>
		Public Overridable Sub run() Implements Runnable.run
			' First get the rendered image
			Dim rdrdImage As java.awt.image.RenderedImage
			If rc IsNot Nothing Then
				rdrdImage = rdblImage.createRendering(rc)
			Else
				rdrdImage = rdblImage.createDefaultRendering()
			End If

			' And its ColorModel
			Dim colorModel_Renamed As java.awt.image.ColorModel = rdrdImage.colorModel
			Dim raster_Renamed As java.awt.image.Raster = rdrdImage.data
			Dim sampleModel_Renamed As java.awt.image.SampleModel = raster_Renamed.sampleModel
			Dim dataBuffer_Renamed As java.awt.image.DataBuffer = raster_Renamed.dataBuffer

			If colorModel_Renamed Is Nothing Then colorModel_Renamed = java.awt.image.ColorModel.rGBdefault
			Dim minX As Integer = raster_Renamed.minX
			Dim minY As Integer = raster_Renamed.minY
			Dim width As Integer = raster_Renamed.width
			Dim height As Integer = raster_Renamed.height

			Dim icList As System.Collections.IEnumerator
			Dim ic As java.awt.image.ImageConsumer
			' Set up the ImageConsumers
			icList = ics.elements()
			Do While icList.hasMoreElements()
				ic = CType(icList.nextElement(), java.awt.image.ImageConsumer)
				ic.dimensionsons(width,height)
				ic.hints = java.awt.image.ImageConsumer.TOPDOWNLEFTRIGHT Or java.awt.image.ImageConsumer.COMPLETESCANLINES Or java.awt.image.ImageConsumer.SINGLEPASS Or java.awt.image.ImageConsumer.SINGLEFRAME
			Loop

			' Get RGB pixels from the raster scanline by scanline and
			' send to consumers.
			Dim pix As Integer() = New Integer(width - 1){}
			Dim i, j As Integer
			Dim numBands As Integer = sampleModel_Renamed.numBands
			Dim tmpPixel As Integer() = New Integer(numBands - 1){}
			For j = 0 To height - 1
				For i = 0 To width - 1
					sampleModel_Renamed.getPixel(i, j, tmpPixel, dataBuffer_Renamed)
					pix(i) = colorModel_Renamed.getDataElement(tmpPixel, 0)
				Next i
				' Now send the scanline to the Consumers
				icList = ics.elements()
				Do While icList.hasMoreElements()
					ic = CType(icList.nextElement(), java.awt.image.ImageConsumer)
					ic.pixelsels(0, j, width, 1, colorModel_Renamed, pix, 0, width)
				Loop
			Next j

			' Now tell the consumers we're done.
			icList = ics.elements()
			Do While icList.hasMoreElements()
				ic = CType(icList.nextElement(), java.awt.image.ImageConsumer)
				ic.imageComplete(java.awt.image.ImageConsumer.STATICIMAGEDONE)
			Loop
		End Sub
	End Class

End Namespace
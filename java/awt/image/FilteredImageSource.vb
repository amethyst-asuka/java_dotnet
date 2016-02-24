Imports System.Runtime.CompilerServices
Imports System.Collections

'
' * Copyright (c) 1995, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' This class is an implementation of the ImageProducer interface which
	''' takes an existing image and a filter object and uses them to produce
	''' image data for a new filtered version of the original image.
	''' Here is an example which filters an image by swapping the red and
	''' blue compents:
	''' <pre>
	''' 
	'''      Image src = getImage("doc:///demo/images/duke/T1.gif");
	'''      ImageFilter colorfilter = new RedBlueSwapFilter();
	'''      Image img = createImage(new FilteredImageSource(src.getSource(),
	'''                                                      colorfilter));
	''' 
	''' </pre>
	''' </summary>
	''' <seealso cref= ImageProducer
	''' 
	''' @author      Jim Graham </seealso>
	Public Class FilteredImageSource
		Implements java.awt.image.ImageProducer

		Friend src As java.awt.image.ImageProducer
		Friend filter As java.awt.image.ImageFilter

		''' <summary>
		''' Constructs an ImageProducer object from an existing ImageProducer
		''' and a filter object. </summary>
		''' <param name="orig"> the specified <code>ImageProducer</code> </param>
		''' <param name="imgf"> the specified <code>ImageFilter</code> </param>
		''' <seealso cref= ImageFilter </seealso>
		''' <seealso cref= java.awt.Component#createImage </seealso>
		Public Sub New(ByVal orig As java.awt.image.ImageProducer, ByVal imgf As java.awt.image.ImageFilter)
			src = orig
			filter = imgf
		End Sub

		Private proxies As Hashtable

		''' <summary>
		''' Adds the specified <code>ImageConsumer</code>
		''' to the list of consumers interested in data for the filtered image.
		''' An instance of the original <code>ImageFilter</code>
		''' is created
		''' (using the filter's <code>getFilterInstance</code> method)
		''' to manipulate the image data
		''' for the specified <code>ImageConsumer</code>.
		''' The newly created filter instance
		''' is then passed to the <code>addConsumer</code> method
		''' of the original <code>ImageProducer</code>.
		''' 
		''' <p>
		''' This method is public as a side effect
		''' of this class implementing
		''' the <code>ImageProducer</code> interface.
		''' It should not be called from user code,
		''' and its behavior if called from user code is unspecified.
		''' </summary>
		''' <param name="ic">  the consumer for the filtered image </param>
		''' <seealso cref= ImageConsumer </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addConsumer(ByVal ic As java.awt.image.ImageConsumer)
			If proxies Is Nothing Then proxies = New Hashtable
			If Not proxies.ContainsKey(ic) Then
				Dim imgf As java.awt.image.ImageFilter = filter.getFilterInstance(ic)
				proxies(ic) = imgf
				src.addConsumer(imgf)
			End If
		End Sub

		''' <summary>
		''' Determines whether an ImageConsumer is on the list of consumers
		''' currently interested in data for this image.
		''' 
		''' <p>
		''' This method is public as a side effect
		''' of this class implementing
		''' the <code>ImageProducer</code> interface.
		''' It should not be called from user code,
		''' and its behavior if called from user code is unspecified.
		''' </summary>
		''' <param name="ic"> the specified <code>ImageConsumer</code> </param>
		''' <returns> true if the ImageConsumer is on the list; false otherwise </returns>
		''' <seealso cref= ImageConsumer </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function isConsumer(ByVal ic As java.awt.image.ImageConsumer) As Boolean
			Return (proxies IsNot Nothing AndAlso proxies.ContainsKey(ic))
		End Function

		''' <summary>
		''' Removes an ImageConsumer from the list of consumers interested in
		''' data for this image.
		''' 
		''' <p>
		''' This method is public as a side effect
		''' of this class implementing
		''' the <code>ImageProducer</code> interface.
		''' It should not be called from user code,
		''' and its behavior if called from user code is unspecified.
		''' </summary>
		''' <seealso cref= ImageConsumer </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeConsumer(ByVal ic As java.awt.image.ImageConsumer)
			If proxies IsNot Nothing Then
				Dim imgf As java.awt.image.ImageFilter = CType(proxies(ic), java.awt.image.ImageFilter)
				If imgf IsNot Nothing Then
					src.removeConsumer(imgf)
					proxies.Remove(ic)
					If proxies.Count = 0 Then proxies = Nothing
				End If
			End If
		End Sub

		''' <summary>
		''' Starts production of the filtered image.
		''' If the specified <code>ImageConsumer</code>
		''' isn't already a consumer of the filtered image,
		''' an instance of the original <code>ImageFilter</code>
		''' is created
		''' (using the filter's <code>getFilterInstance</code> method)
		''' to manipulate the image data
		''' for the <code>ImageConsumer</code>.
		''' The filter instance for the <code>ImageConsumer</code>
		''' is then passed to the <code>startProduction</code> method
		''' of the original <code>ImageProducer</code>.
		''' 
		''' <p>
		''' This method is public as a side effect
		''' of this class implementing
		''' the <code>ImageProducer</code> interface.
		''' It should not be called from user code,
		''' and its behavior if called from user code is unspecified.
		''' </summary>
		''' <param name="ic">  the consumer for the filtered image </param>
		''' <seealso cref= ImageConsumer </seealso>
		Public Overridable Sub startProduction(ByVal ic As java.awt.image.ImageConsumer)
			If proxies Is Nothing Then proxies = New Hashtable
			Dim imgf As java.awt.image.ImageFilter = CType(proxies(ic), java.awt.image.ImageFilter)
			If imgf Is Nothing Then
				imgf = filter.getFilterInstance(ic)
				proxies(ic) = imgf
			End If
			src.startProduction(imgf)
		End Sub

		''' <summary>
		''' Requests that a given ImageConsumer have the image data delivered
		''' one more time in top-down, left-right order.  The request is
		''' handed to the ImageFilter for further processing, since the
		''' ability to preserve the pixel ordering depends on the filter.
		''' 
		''' <p>
		''' This method is public as a side effect
		''' of this class implementing
		''' the <code>ImageProducer</code> interface.
		''' It should not be called from user code,
		''' and its behavior if called from user code is unspecified.
		''' </summary>
		''' <seealso cref= ImageConsumer </seealso>
		Public Overridable Sub requestTopDownLeftRightResend(ByVal ic As java.awt.image.ImageConsumer)
			If proxies IsNot Nothing Then
				Dim imgf As java.awt.image.ImageFilter = CType(proxies(ic), java.awt.image.ImageFilter)
				If imgf IsNot Nothing Then imgf.resendTopDownLeftRight(src)
			End If
		End Sub
	End Class

End Namespace
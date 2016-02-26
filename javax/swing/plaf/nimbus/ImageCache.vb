Imports System.Collections.Generic

'
' * Copyright (c) 2005, 2006, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.plaf.nimbus


	''' <summary>
	''' ImageCache - A fixed pixel count sized cache of Images keyed by arbitrary set of arguments. All images are held with
	''' SoftReferences so they will be dropped by the GC if heap memory gets tight. When our size hits max pixel count least
	''' recently requested images are removed first.
	''' 
	''' @author Created by Jasper Potts (Aug 7, 2007)
	''' </summary>
	Friend Class ImageCache
		' Ordered Map keyed by args hash, ordered by most recent accessed entry.
		Private ReadOnly map As New java.util.LinkedHashMap(Of Integer?, PixelCountSoftReference)(16, 0.75f, True)
		' Maximum number of pixels to cache, this is used if maxCount
		Private ReadOnly maxPixelCount As Integer
		' Maximum cached image size in pxiels
		Private ReadOnly maxSingleImagePixelSize As Integer
		' The current number of pixels stored in the cache
		Private currentPixelCount As Integer = 0
		' Lock for concurrent access to map
		Private lock As java.util.concurrent.locks.ReadWriteLock = New java.util.concurrent.locks.ReentrantReadWriteLock
		' Reference queue for tracking lost softreferences to images in the cache
		Private referenceQueue As New ReferenceQueue(Of java.awt.Image)
		' Singleton Instance
		Private Shared ReadOnly instance As New ImageCache


		''' <summary>
		''' Get static singleton instance </summary>
		Shared instance As ImageCache
			Get
				Return instance
			End Get
		End Property

		Public Sub New()
			Me.maxPixelCount = (8 * 1024 * 1024) \ 4 ' 8Mb of pixels
			Me.maxSingleImagePixelSize = 300 * 300
		End Sub

		Public Sub New(ByVal maxPixelCount As Integer, ByVal maxSingleImagePixelSize As Integer)
			Me.maxPixelCount = maxPixelCount
			Me.maxSingleImagePixelSize = maxSingleImagePixelSize
		End Sub

		''' <summary>
		''' Clear the cache </summary>
		Public Overridable Sub flush()
			lock.readLock().lock()
			Try
				map.clear()
			Finally
				lock.readLock().unlock()
			End Try
		End Sub

		''' <summary>
		''' Check if the image size is to big to be stored in the cache
		''' </summary>
		''' <param name="w"> The image width </param>
		''' <param name="h"> The image height </param>
		''' <returns> True if the image size is less than max </returns>
		Public Overridable Function isImageCachable(ByVal w As Integer, ByVal h As Integer) As Boolean
			Return (w * h) < maxSingleImagePixelSize
		End Function

		''' <summary>
		''' Get the cached image for given keys
		''' </summary>
		''' <param name="config"> The graphics configuration, needed if cached image is a Volatile Image. Used as part of cache key </param>
		''' <param name="w">      The image width, used as part of cache key </param>
		''' <param name="h">      The image height, used as part of cache key </param>
		''' <param name="args">   Other arguments to use as part of the cache key </param>
		''' <returns> Returns the cached Image, or null there is no cached image for key </returns>
		Public Overridable Function getImage(ByVal config As java.awt.GraphicsConfiguration, ByVal w As Integer, ByVal h As Integer, ParamArray ByVal args As Object()) As java.awt.Image
			lock.readLock().lock()
			Try
				Dim ref As PixelCountSoftReference = map.get(hash(config, w, h, args))
				' check reference has not been lost and the key truly matches, in case of false positive hash match
				If ref IsNot Nothing AndAlso ref.Equals(config,w, h, args) Then
					Return ref.get()
				Else
					Return Nothing
				End If
			Finally
				lock.readLock().unlock()
			End Try
		End Function

		''' <summary>
		''' Sets the cached image for the specified constraints.
		''' </summary>
		''' <param name="image">  The image to store in cache </param>
		''' <param name="config"> The graphics configuration, needed if cached image is a Volatile Image. Used as part of cache key </param>
		''' <param name="w">      The image width, used as part of cache key </param>
		''' <param name="h">      The image height, used as part of cache key </param>
		''' <param name="args">   Other arguments to use as part of the cache key </param>
		''' <returns> true if the image could be cached or false if the image is too big </returns>
		Public Overridable Function setImage(ByVal image As java.awt.Image, ByVal config As java.awt.GraphicsConfiguration, ByVal w As Integer, ByVal h As Integer, ParamArray ByVal args As Object()) As Boolean
			If Not isImageCachable(w, h) Then Return False
			Dim hash As Integer = hash(config, w, h, args)
			lock.writeLock().lock()
			Try
				Dim ref As PixelCountSoftReference = map.get(hash)
				' check if currently in map
				If ref IsNot Nothing AndAlso ref.get() Is image Then Return True
				' clear out old
				If ref IsNot Nothing Then
					currentPixelCount -= ref.pixelCount
					map.remove(hash)
				End If
				' add new image to pixel count
				Dim newPixelCount As Integer = image.getWidth(Nothing) * image.getHeight(Nothing)
				currentPixelCount += newPixelCount
				' clean out lost references if not enough space
				If currentPixelCount > maxPixelCount Then
					ref = CType(referenceQueue.poll(), PixelCountSoftReference)
					Do While ref IsNot Nothing
						'reference lost
						map.remove(ref.hash)
						currentPixelCount -= ref.pixelCount
						ref = CType(referenceQueue.poll(), PixelCountSoftReference)
					Loop
				End If
				' remove old items till there is enough free space
				If currentPixelCount > maxPixelCount Then
					Dim mapIter As IEnumerator(Of KeyValuePair(Of Integer?, PixelCountSoftReference)) = map.entrySet().GetEnumerator()
					Do While (currentPixelCount > maxPixelCount) AndAlso mapIter.MoveNext()
						Dim entry As KeyValuePair(Of Integer?, PixelCountSoftReference) = mapIter.Current
						mapIter.remove()
						Dim img As java.awt.Image = entry.Value.get()
						If img IsNot Nothing Then img.flush()
						currentPixelCount -= entry.Value.pixelCount
					Loop
				End If
				' finaly put new in map
				map.put(hash, New PixelCountSoftReference(image, referenceQueue, newPixelCount,hash, config, w, h, args))
				Return True
			Finally
				lock.writeLock().unlock()
			End Try
		End Function

		''' <summary>
		''' Create a unique hash from all the input </summary>
		Private Function hash(ByVal config As java.awt.GraphicsConfiguration, ByVal w As Integer, ByVal h As Integer, ParamArray ByVal args As Object()) As Integer
			Dim ___hash As Integer
			___hash = (If(config IsNot Nothing, config.GetHashCode(), 0))
			___hash = 31 * ___hash + w
			___hash = 31 * ___hash + h
			___hash = 31 * ___hash + java.util.Arrays.deepHashCode(args)
			Return ___hash
		End Function


		''' <summary>
		''' Extended SoftReference that stores the pixel count even after the image is lost </summary>
		Private Class PixelCountSoftReference
			Inherits SoftReference(Of java.awt.Image)

			Private ReadOnly pixelCount As Integer
			Private ReadOnly hash As Integer
			' key parts
			Private ReadOnly config As java.awt.GraphicsConfiguration
			Private ReadOnly w As Integer
			Private ReadOnly h As Integer
			Private ReadOnly args As Object()

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub New(Of T1)(ByVal referent As java.awt.Image, ByVal q As ReferenceQueue(Of T1), ByVal pixelCount As Integer, ByVal hash As Integer, ByVal config As java.awt.GraphicsConfiguration, ByVal w As Integer, ByVal h As Integer, ByVal args As Object())
				MyBase.New(referent, q)
				Me.pixelCount = pixelCount
				Me.hash = hash
				Me.config = config
				Me.w = w
				Me.h = h
				Me.args = args
			End Sub

			Public Overrides Function Equals(ByVal config As java.awt.GraphicsConfiguration, ByVal w As Integer, ByVal h As Integer, ByVal args As Object()) As Boolean
				Return config Is Me.config AndAlso w = Me.w AndAlso h = Me.h AndAlso java.util.Arrays.Equals(args, Me.args)
			End Function
		End Class
	End Class

End Namespace
Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2004, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.imageio


	''' <summary>
	''' A simple container class to aggregate an image, a set of
	''' thumbnail (preview) images, and an object representing metadata
	''' associated with the image.
	''' 
	''' <p> The image data may take the form of either a
	''' <code>RenderedImage</code>, or a <code>Raster</code>.  Reader
	''' methods that return an <code>IIOImage</code> will always return a
	''' <code>BufferedImage</code> using the <code>RenderedImage</code>
	''' reference.  Writer methods that accept an <code>IIOImage</code>
	''' will always accept a <code>RenderedImage</code>, and may optionally
	''' accept a <code>Raster</code>.
	''' 
	''' <p> Exactly one of <code>getRenderedImage</code> and
	''' <code>getRaster</code> will return a non-<code>null</code> value.
	''' Subclasses are responsible for ensuring this behavior.
	''' </summary>
	''' <seealso cref= ImageReader#readAll(int, ImageReadParam) </seealso>
	''' <seealso cref= ImageReader#readAll(java.util.Iterator) </seealso>
	''' <seealso cref= ImageWriter#write(javax.imageio.metadata.IIOMetadata,
	'''                        IIOImage, ImageWriteParam) </seealso>
	''' <seealso cref= ImageWriter#write(IIOImage) </seealso>
	''' <seealso cref= ImageWriter#writeToSequence(IIOImage, ImageWriteParam) </seealso>
	''' <seealso cref= ImageWriter#writeInsert(int, IIOImage, ImageWriteParam)
	'''  </seealso>
	Public Class IIOImage

		''' <summary>
		''' The <code>RenderedImage</code> being referenced.
		''' </summary>
		Protected Friend image As java.awt.image.RenderedImage

		''' <summary>
		''' The <code>Raster</code> being referenced.
		''' </summary>
		Protected Friend raster As java.awt.image.Raster

		''' <summary>
		''' A <code>List</code> of <code>BufferedImage</code> thumbnails,
		''' or <code>null</code>.  Non-<code>BufferedImage</code> objects
		''' must not be stored in this <code>List</code>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Protected Friend thumbnails As IList(Of ? As java.awt.image.BufferedImage) = Nothing

		''' <summary>
		''' An <code>IIOMetadata</code> object containing metadata
		''' associated with the image.
		''' </summary>
		Protected Friend metadata As javax.imageio.metadata.IIOMetadata

		''' <summary>
		''' Constructs an <code>IIOImage</code> containing a
		''' <code>RenderedImage</code>, and thumbnails and metadata
		''' associated with it.
		''' 
		''' <p> All parameters are stored by reference.
		''' 
		''' <p> The <code>thumbnails</code> argument must either be
		''' <code>null</code> or contain only <code>BufferedImage</code>
		''' objects.
		''' </summary>
		''' <param name="image"> a <code>RenderedImage</code>. </param>
		''' <param name="thumbnails"> a <code>List</code> of <code>BufferedImage</code>s,
		''' or <code>null</code>. </param>
		''' <param name="metadata"> an <code>IIOMetadata</code> object, or
		''' <code>null</code>.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>image</code> is
		''' <code>null</code>. </exception>
		Public Sub New(Of T1 As java.awt.image.BufferedImage)(ByVal image As java.awt.image.RenderedImage, ByVal thumbnails As IList(Of T1), ByVal metadata As javax.imageio.metadata.IIOMetadata)
			If image Is Nothing Then Throw New System.ArgumentException("image == null!")
			Me.image = image
			Me.raster = Nothing
			Me.thumbnails = thumbnails
			Me.metadata = metadata
		End Sub

		''' <summary>
		''' Constructs an <code>IIOImage</code> containing a
		''' <code>Raster</code>, and thumbnails and metadata
		''' associated with it.
		''' 
		''' <p> All parameters are stored by reference.
		''' </summary>
		''' <param name="raster"> a <code>Raster</code>. </param>
		''' <param name="thumbnails"> a <code>List</code> of <code>BufferedImage</code>s,
		''' or <code>null</code>. </param>
		''' <param name="metadata"> an <code>IIOMetadata</code> object, or
		''' <code>null</code>.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>raster</code> is
		''' <code>null</code>. </exception>
		Public Sub New(Of T1 As java.awt.image.BufferedImage)(ByVal raster As java.awt.image.Raster, ByVal thumbnails As IList(Of T1), ByVal metadata As javax.imageio.metadata.IIOMetadata)
			If raster Is Nothing Then Throw New System.ArgumentException("raster == null!")
			Me.raster = raster
			Me.image = Nothing
			Me.thumbnails = thumbnails
			Me.metadata = metadata
		End Sub

		''' <summary>
		''' Returns the currently set <code>RenderedImage</code>, or
		''' <code>null</code> if only a <code>Raster</code> is available.
		''' </summary>
		''' <returns> a <code>RenderedImage</code>, or <code>null</code>.
		''' </returns>
		''' <seealso cref= #setRenderedImage </seealso>
		Public Overridable Property renderedImage As java.awt.image.RenderedImage
			Get
				SyncLock Me
					Return image
				End SyncLock
			End Get
			Set(ByVal image As java.awt.image.RenderedImage)
				SyncLock Me
					If image Is Nothing Then Throw New System.ArgumentException("image == null!")
					Me.image = image
					Me.raster = Nothing
				End SyncLock
			End Set
		End Property


		''' <summary>
		''' Returns <code>true</code> if this <code>IIOImage</code> stores
		''' a <code>Raster</code> rather than a <code>RenderedImage</code>.
		''' </summary>
		''' <returns> <code>true</code> if a <code>Raster</code> is
		''' available. </returns>
		Public Overridable Function hasRaster() As Boolean
			SyncLock Me
				Return (raster IsNot Nothing)
			End SyncLock
		End Function

		''' <summary>
		''' Returns the currently set <code>Raster</code>, or
		''' <code>null</code> if only a <code>RenderedImage</code> is
		''' available.
		''' </summary>
		''' <returns> a <code>Raster</code>, or <code>null</code>.
		''' </returns>
		''' <seealso cref= #setRaster </seealso>
		Public Overridable Property raster As java.awt.image.Raster
			Get
				SyncLock Me
					Return raster
				End SyncLock
			End Get
			Set(ByVal raster As java.awt.image.Raster)
				SyncLock Me
					If raster Is Nothing Then Throw New System.ArgumentException("raster == null!")
					Me.raster = raster
					Me.image = Nothing
				End SyncLock
			End Set
		End Property


		''' <summary>
		''' Returns the number of thumbnails stored in this
		''' <code>IIOImage</code>.
		''' </summary>
		''' <returns> the number of thumbnails, as an <code>int</code>. </returns>
		Public Overridable Property numThumbnails As Integer
			Get
				Return If(thumbnails Is Nothing, 0, thumbnails.Count)
			End Get
		End Property

		''' <summary>
		''' Returns a thumbnail associated with the main image.
		''' </summary>
		''' <param name="index"> the index of the desired thumbnail image.
		''' </param>
		''' <returns> a thumbnail image, as a <code>BufferedImage</code>.
		''' </returns>
		''' <exception cref="IndexOutOfBoundsException"> if the supplied index is
		''' negative or larger than the largest valid index. </exception>
		''' <exception cref="ClassCastException"> if a
		''' non-<code>BufferedImage</code> object is encountered in the
		''' list of thumbnails at the given index.
		''' </exception>
		''' <seealso cref= #getThumbnails </seealso>
		''' <seealso cref= #setThumbnails </seealso>
		Public Overridable Function getThumbnail(ByVal index As Integer) As java.awt.image.BufferedImage
			If thumbnails Is Nothing Then Throw New System.IndexOutOfRangeException("No thumbnails available!")
			Return CType(thumbnails(index), java.awt.image.BufferedImage)
		End Function

		''' <summary>
		''' Returns the current <code>List</code> of thumbnail
		''' <code>BufferedImage</code>s, or <code>null</code> if none is
		''' set.  A live reference is returned.
		''' </summary>
		''' <returns> the current <code>List</code> of
		''' <code>BufferedImage</code> thumbnails, or <code>null</code>.
		''' </returns>
		''' <seealso cref= #getThumbnail(int) </seealso>
		''' <seealso cref= #setThumbnails </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property thumbnails As IList(Of ? As java.awt.image.BufferedImage)
			Get
				Return thumbnails
			End Get
			Set(ByVal thumbnails As IList(Of T1))
				Me.thumbnails = thumbnails
			End Set
		End Property


		''' <summary>
		''' Returns a reference to the current <code>IIOMetadata</code>
		''' object, or <code>null</code> is none is set.
		''' </summary>
		''' <returns> an <code>IIOMetadata</code> object, or <code>null</code>.
		''' </returns>
		''' <seealso cref= #setMetadata </seealso>
		Public Overridable Property metadata As javax.imageio.metadata.IIOMetadata
			Get
				Return metadata
			End Get
			Set(ByVal metadata As javax.imageio.metadata.IIOMetadata)
				Me.metadata = metadata
			End Set
		End Property

	End Class

End Namespace
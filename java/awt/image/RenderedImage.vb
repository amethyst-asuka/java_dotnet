Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2008, Oracle and/or its affiliates. All rights reserved.
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

' ****************************************************************
' ******************************************************************
' ******************************************************************
' *** COPYRIGHT (c) Eastman Kodak Company, 1997
' *** As  an unpublished  work pursuant to Title 17 of the United
' *** States Code.  All rights reserved.
' ******************************************************************
' ******************************************************************
' *****************************************************************

Namespace java.awt.image

	''' <summary>
	''' RenderedImage is a common interface for objects which contain
	''' or can produce image data in the form of Rasters.  The image
	''' data may be stored/produced as a single tile or a regular array
	''' of tiles.
	''' </summary>

	Public Interface RenderedImage

		''' <summary>
		''' Returns a vector of RenderedImages that are the immediate sources of
		''' image data for this RenderedImage.  This method returns null if
		''' the RenderedImage object has no information about its immediate
		''' sources.  It returns an empty Vector if the RenderedImage object has
		''' no immediate sources. </summary>
		''' <returns> a Vector of <code>RenderedImage</code> objects. </returns>
		ReadOnly Property sources As List(Of RenderedImage)

		''' <summary>
		''' Gets a property from the property set of this image.  The set of
		''' properties and whether it is immutable is determined by the
		''' implementing class.  This method returns
		''' java.awt.Image.UndefinedProperty if the specified property is
		''' not defined for this RenderedImage. </summary>
		''' <param name="name"> the name of the property </param>
		''' <returns> the property indicated by the specified name. </returns>
		''' <seealso cref= java.awt.Image#UndefinedProperty </seealso>
		Function getProperty(ByVal name As String) As Object

		''' <summary>
		''' Returns an array of names recognized by
		''' <seealso cref="#getProperty(String) getProperty(String)"/>
		''' or <code>null</code>, if no property names are recognized. </summary>
		''' <returns> a <code>String</code> array containing all of the
		''' property names that <code>getProperty(String)</code> recognizes;
		''' or <code>null</code> if no property names are recognized. </returns>
		ReadOnly Property propertyNames As String()

		''' <summary>
		''' Returns the ColorModel associated with this image.  All Rasters
		''' returned from this image will have this as their ColorModel.  This
		''' can return null. </summary>
		''' <returns> the <code>ColorModel</code> of this image. </returns>
		ReadOnly Property colorModel As ColorModel

		''' <summary>
		''' Returns the SampleModel associated with this image.  All Rasters
		''' returned from this image will have this as their SampleModel. </summary>
		''' <returns> the <code>SampleModel</code> of this image. </returns>
		ReadOnly Property sampleModel As SampleModel

		''' <summary>
		''' Returns the width of the RenderedImage. </summary>
		''' <returns> the width of this <code>RenderedImage</code>. </returns>
		ReadOnly Property width As Integer

		''' <summary>
		''' Returns the height of the RenderedImage. </summary>
		''' <returns> the height of this <code>RenderedImage</code>. </returns>
		ReadOnly Property height As Integer

		''' <summary>
		''' Returns the minimum X coordinate (inclusive) of the RenderedImage. </summary>
		''' <returns> the X coordinate of this <code>RenderedImage</code>. </returns>
		ReadOnly Property minX As Integer

		''' <summary>
		''' Returns the minimum Y coordinate (inclusive) of the RenderedImage. </summary>
		''' <returns> the Y coordinate of this <code>RenderedImage</code>. </returns>
		ReadOnly Property minY As Integer

		''' <summary>
		''' Returns the number of tiles in the X direction. </summary>
		''' <returns> the number of tiles in the X direction. </returns>
		ReadOnly Property numXTiles As Integer

		''' <summary>
		''' Returns the number of tiles in the Y direction. </summary>
		''' <returns> the number of tiles in the Y direction. </returns>
		ReadOnly Property numYTiles As Integer

		''' <summary>
		'''  Returns the minimum tile index in the X direction. </summary>
		'''  <returns> the minimum tile index in the X direction. </returns>
		ReadOnly Property minTileX As Integer

		''' <summary>
		'''  Returns the minimum tile index in the Y direction. </summary>
		'''  <returns> the minimum tile index in the X direction. </returns>
		ReadOnly Property minTileY As Integer

		''' <summary>
		'''  Returns the tile width in pixels.  All tiles must have the same
		'''  width. </summary>
		'''  <returns> the tile width in pixels. </returns>
		ReadOnly Property tileWidth As Integer

		''' <summary>
		'''  Returns the tile height in pixels.  All tiles must have the same
		'''  height. </summary>
		'''  <returns> the tile height in pixels. </returns>
		ReadOnly Property tileHeight As Integer

		''' <summary>
		''' Returns the X offset of the tile grid relative to the origin,
		''' i.e., the X coordinate of the upper-left pixel of tile (0, 0).
		''' (Note that tile (0, 0) may not actually exist.) </summary>
		''' <returns> the X offset of the tile grid relative to the origin. </returns>
		ReadOnly Property tileGridXOffset As Integer

		''' <summary>
		''' Returns the Y offset of the tile grid relative to the origin,
		''' i.e., the Y coordinate of the upper-left pixel of tile (0, 0).
		''' (Note that tile (0, 0) may not actually exist.) </summary>
		''' <returns> the Y offset of the tile grid relative to the origin. </returns>
		ReadOnly Property tileGridYOffset As Integer

		''' <summary>
		''' Returns tile (tileX, tileY).  Note that tileX and tileY are indices
		''' into the tile array, not pixel locations.  The Raster that is returned
		''' is live and will be updated if the image is changed. </summary>
		''' <param name="tileX"> the X index of the requested tile in the tile array </param>
		''' <param name="tileY"> the Y index of the requested tile in the tile array </param>
		''' <returns> the tile given the specified indices. </returns>
	   Function getTile(ByVal tileX As Integer, ByVal tileY As Integer) As Raster

		''' <summary>
		''' Returns the image as one large tile (for tile based
		''' images this will require fetching the whole image
		''' and copying the image data over).  The Raster returned is
		''' a copy of the image data and will not be updated if the image
		''' is changed. </summary>
		''' <returns> the image as one large tile. </returns>
		ReadOnly Property data As Raster

		''' <summary>
		''' Computes and returns an arbitrary region of the RenderedImage.
		''' The Raster returned is a copy of the image data and will not
		''' be updated if the image is changed. </summary>
		''' <param name="rect"> the region of the RenderedImage to be returned. </param>
		''' <returns> the region of the <code>RenderedImage</code>
		''' indicated by the specified <code>Rectangle</code>. </returns>
		Function getData(ByVal rect As java.awt.Rectangle) As Raster

		''' <summary>
		''' Computes an arbitrary rectangular region of the RenderedImage
		''' and copies it into a caller-supplied WritableRaster.  The region
		''' to be computed is determined from the bounds of the supplied
		''' WritableRaster.  The supplied WritableRaster must have a
		''' SampleModel that is compatible with this image.  If raster is null,
		''' an appropriate WritableRaster is created. </summary>
		''' <param name="raster"> a WritableRaster to hold the returned portion of the
		'''               image, or null. </param>
		''' <returns> a reference to the supplied or created WritableRaster. </returns>
		Function copyData(ByVal raster As WritableRaster) As WritableRaster
	End Interface

End Namespace
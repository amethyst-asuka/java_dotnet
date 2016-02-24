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
	''' WriteableRenderedImage is a common interface for objects which
	''' contain or can produce image data in the form of Rasters and
	''' which can be modified and/or written over.  The image
	''' data may be stored/produced as a single tile or a regular array
	''' of tiles.
	''' <p>
	''' WritableRenderedImage provides notification to other interested
	''' objects when a tile is checked out for writing (via the
	''' getWritableTile method) and when the last writer of a particular
	''' tile relinquishes its access (via a call to releaseWritableTile).
	''' Additionally, it allows any caller to determine whether any tiles
	''' are currently checked out (via hasTileWriters), and to obtain a
	''' list of such tiles (via getWritableTileIndices, in the form of a Vector
	''' of Point objects).
	''' <p>
	''' Objects wishing to be notified of changes in tile writability must
	''' implement the TileObserver interface, and are added by a
	''' call to addTileObserver.  Multiple calls to
	''' addTileObserver for the same object will result in multiple
	''' notifications.  An existing observer may reduce its notifications
	''' by calling removeTileObserver; if the observer had no
	''' notifications the operation is a no-op.
	''' <p>
	''' It is necessary for a WritableRenderedImage to ensure that
	''' notifications occur only when the first writer acquires a tile and
	''' the last writer releases it.
	''' 
	''' </summary>

	Public Interface WritableRenderedImage
		Inherits RenderedImage

	  ''' <summary>
	  ''' Adds an observer.  If the observer is already present,
	  ''' it will receive multiple notifications. </summary>
	  ''' <param name="to"> the specified <code>TileObserver</code> </param>
	  Sub addTileObserver(ByVal [to] As TileObserver)

	  ''' <summary>
	  ''' Removes an observer.  If the observer was not registered,
	  ''' nothing happens.  If the observer was registered for multiple
	  ''' notifications, it will now be registered for one fewer. </summary>
	  ''' <param name="to"> the specified <code>TileObserver</code> </param>
	  Sub removeTileObserver(ByVal [to] As TileObserver)

	  ''' <summary>
	  ''' Checks out a tile for writing.
	  ''' 
	  ''' The WritableRenderedImage is responsible for notifying all
	  ''' of its TileObservers when a tile goes from having
	  ''' no writers to having one writer.
	  ''' </summary>
	  ''' <param name="tileX"> the X index of the tile. </param>
	  ''' <param name="tileY"> the Y index of the tile. </param>
	  ''' <returns> a writable tile. </returns>
	  Function getWritableTile(ByVal tileX As Integer, ByVal tileY As Integer) As WritableRaster

	  ''' <summary>
	  ''' Relinquishes the right to write to a tile.  If the caller
	  ''' continues to write to the tile, the results are undefined.
	  ''' Calls to this method should only appear in matching pairs
	  ''' with calls to getWritableTile; any other use will lead
	  ''' to undefined results.
	  ''' 
	  ''' The WritableRenderedImage is responsible for notifying all of
	  ''' its TileObservers when a tile goes from having one writer
	  ''' to having no writers.
	  ''' </summary>
	  ''' <param name="tileX"> the X index of the tile. </param>
	  ''' <param name="tileY"> the Y index of the tile. </param>
	  Sub releaseWritableTile(ByVal tileX As Integer, ByVal tileY As Integer)

	  ''' <summary>
	  ''' Returns whether a tile is currently checked out for writing.
	  ''' </summary>
	  ''' <param name="tileX"> the X index of the tile. </param>
	  ''' <param name="tileY"> the Y index of the tile. </param>
	  ''' <returns> <code>true</code> if specified tile is checked out
	  '''         for writing; <code>false</code> otherwise. </returns>
	  Function isTileWritable(ByVal tileX As Integer, ByVal tileY As Integer) As Boolean

	  ''' <summary>
	  ''' Returns an array of Point objects indicating which tiles
	  ''' are checked out for writing.  Returns null if none are
	  ''' checked out. </summary>
	  ''' <returns> an array containing the locations of tiles that are
	  '''         checked out for writing. </returns>
	  ReadOnly Property writableTileIndices As java.awt.Point()

	  ''' <summary>
	  ''' Returns whether any tile is checked out for writing.
	  ''' Semantically equivalent to (getWritableTileIndices() != null). </summary>
	  ''' <returns> <code>true</code> if any tiles are checked out for
	  '''         writing; <code>false</code> otherwise. </returns>
	  Function hasTileWriters() As Boolean

	  ''' <summary>
	  ''' Sets a rect of the image to the contents of the Raster r, which is
	  ''' assumed to be in the same coordinate space as the WritableRenderedImage.
	  ''' The operation is clipped to the bounds of the WritableRenderedImage. </summary>
	  ''' <param name="r"> the specified <code>Raster</code> </param>
	  WriteOnly Property data As Raster

	End Interface

End Namespace
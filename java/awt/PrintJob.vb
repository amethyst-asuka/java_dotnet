'
' * Copyright (c) 1996, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt

	''' <summary>
	''' An abstract class which initiates and executes a print job.
	''' It provides access to a print graphics object which renders
	''' to an appropriate print device.
	''' </summary>
	''' <seealso cref= Toolkit#getPrintJob
	''' 
	''' @author      Amy Fowler </seealso>
	Public MustInherit Class PrintJob

		''' <summary>
		''' Gets a Graphics object that will draw to the next page.
		''' The page is sent to the printer when the graphics
		''' object is disposed.  This graphics object will also implement
		''' the PrintGraphics interface. </summary>
		''' <seealso cref= PrintGraphics </seealso>
		Public MustOverride ReadOnly Property graphics As Graphics

		''' <summary>
		''' Returns the dimensions of the page in pixels.
		''' The resolution of the page is chosen so that it
		''' is similar to the screen resolution.
		''' </summary>
		Public MustOverride ReadOnly Property pageDimension As Dimension

		''' <summary>
		''' Returns the resolution of the page in pixels per inch.
		''' Note that this doesn't have to correspond to the physical
		''' resolution of the printer.
		''' </summary>
		Public MustOverride ReadOnly Property pageResolution As Integer

		''' <summary>
		''' Returns true if the last page will be printed first.
		''' </summary>
		Public MustOverride Function lastPageFirst() As Boolean

		''' <summary>
		''' Ends the print job and does any necessary cleanup.
		''' </summary>
		Public MustOverride Sub [end]()

		''' <summary>
		''' Ends this print job once it is no longer referenced. </summary>
		''' <seealso cref= #end </seealso>
		Protected Overrides Sub Finalize()
			[end]()
		End Sub

	End Class

End Namespace
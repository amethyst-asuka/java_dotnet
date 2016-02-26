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
	''' A special painter implementation for tool bar separators in Nimbus.
	''' The designer tool doesn't have support for painters which render
	''' repeated patterns, but that's exactly what the toolbar separator design
	''' is for Nimbus. This custom painter is designed to handle this situation.
	''' When support is added to the design tool / code generator to deal with
	''' repeated patterns, then we can remove this class.
	''' <p>
	''' </summary>
	Friend NotInheritable Class ToolBarSeparatorPainter
		Inherits AbstractRegionPainter

		Private Const SPACE As Integer = 3
		Private Const INSET As Integer = 2

		Protected Friend Property Overrides paintContext As PaintContext
			Get
				'the paint context returned will have a few dummy values. The
				'implementation of doPaint doesn't bother with the "decode" methods
				'but calculates where to paint the circles manually. As such, we
				'only need to indicate in our PaintContext that we don't want this
				'to ever be cached
				Return New PaintContext(New java.awt.Insets(1, 0, 1, 0), New java.awt.Dimension(38, 7), False, javax.swing.plaf.nimbus.AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1, 1)
			End Get
		End Property

		Protected Friend Overrides Sub doPaint(ByVal g As java.awt.Graphics2D, ByVal c As javax.swing.JComponent, ByVal width As Integer, ByVal height As Integer, ByVal extendedCacheKeys As Object())
			'it is assumed that in the normal orientation the separator renders
			'horizontally. Other code rotates it as necessary for a vertical
			'separator.
			g.color = c.foreground
			Dim y As Integer = height \ 2
			For i As Integer = INSET To width-INSET Step SPACE
				g.fillRect(i, y, 1, 1)
			Next i
		End Sub
	End Class

End Namespace
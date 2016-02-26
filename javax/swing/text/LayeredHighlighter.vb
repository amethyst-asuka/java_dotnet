'
' * Copyright (c) 1998, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text


	''' 
	''' <summary>
	''' @author  Scott Violet
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref=     Highlighter </seealso>
	Public MustInherit Class LayeredHighlighter
		Implements Highlighter

		''' <summary>
		''' When leaf Views (such as LabelView) are rendering they should
		''' call into this method. If a highlight is in the given region it will
		''' be drawn immediately.
		''' </summary>
		''' <param name="g"> Graphics used to draw </param>
		''' <param name="p0"> starting offset of view </param>
		''' <param name="p1"> ending offset of view </param>
		''' <param name="viewBounds"> Bounds of View </param>
		''' <param name="editor"> JTextComponent </param>
		''' <param name="view"> View instance being rendered </param>
		Public MustOverride Sub paintLayeredHighlights(ByVal g As java.awt.Graphics, ByVal p0 As Integer, ByVal p1 As Integer, ByVal viewBounds As java.awt.Shape, ByVal editor As JTextComponent, ByVal view As View)


		''' <summary>
		''' Layered highlight renderer.
		''' </summary>
		Public MustInherit Class LayerPainter
			Implements Highlighter.HighlightPainter

				Public MustOverride ReadOnly Property painter As HighlightPainter
				Public MustOverride ReadOnly Property endOffset As Integer
				Public MustOverride ReadOnly Property startOffset As Integer
				Public MustOverride Sub paint(ByVal g As java.awt.Graphics, ByVal p0 As Integer, ByVal p1 As Integer, ByVal bounds As java.awt.Shape, ByVal c As JTextComponent)
				Public MustOverride ReadOnly Property highlights As Highlight()
				Public MustOverride Sub changeHighlight(ByVal tag As Object, ByVal p0 As Integer, ByVal p1 As Integer)
				Public MustOverride Sub removeAllHighlights()
				Public MustOverride Sub removeHighlight(ByVal tag As Object)
				Public MustOverride Function addHighlight(ByVal p0 As Integer, ByVal p1 As Integer, ByVal p As HighlightPainter) As Object
				Public MustOverride Sub paint(ByVal g As java.awt.Graphics)
				Public MustOverride Sub deinstall(ByVal c As JTextComponent)
				Public MustOverride Sub install(ByVal c As JTextComponent)
			Public MustOverride Function paintLayer(ByVal g As java.awt.Graphics, ByVal p0 As Integer, ByVal p1 As Integer, ByVal viewBounds As java.awt.Shape, ByVal editor As JTextComponent, ByVal view As View) As java.awt.Shape
		End Class
	End Class

End Namespace
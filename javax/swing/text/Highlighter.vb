'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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


	''' <summary>
	''' An interface for an object that allows one to mark up the background
	''' with colored areas.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Interface Highlighter

		''' <summary>
		''' Called when the UI is being installed into the
		''' interface of a JTextComponent.  This can be used
		''' to gain access to the model that is being navigated
		''' by the implementation of this interface.
		''' </summary>
		''' <param name="c"> the JTextComponent editor </param>
		Sub install(ByVal c As JTextComponent)

		''' <summary>
		''' Called when the UI is being removed from the
		''' interface of a JTextComponent.  This is used to
		''' unregister any listeners that were attached.
		''' </summary>
		''' <param name="c"> the JTextComponent editor </param>
		Sub deinstall(ByVal c As JTextComponent)

		''' <summary>
		''' Renders the highlights.
		''' </summary>
		''' <param name="g"> the graphics context. </param>
		Sub paint(ByVal g As java.awt.Graphics)

		''' <summary>
		''' Adds a highlight to the view.  Returns a tag that can be used
		''' to refer to the highlight.
		''' </summary>
		''' <param name="p0"> the beginning of the range &gt;= 0 </param>
		''' <param name="p1"> the end of the range &gt;= p0 </param>
		''' <param name="p"> the painter to use for the actual highlighting </param>
		''' <returns> an object that refers to the highlight </returns>
		''' <exception cref="BadLocationException"> for an invalid range specification </exception>
		Function addHighlight(ByVal p0 As Integer, ByVal p1 As Integer, ByVal p As HighlightPainter) As Object

		''' <summary>
		''' Removes a highlight from the view.
		''' </summary>
		''' <param name="tag">  which highlight to remove </param>
		Sub removeHighlight(ByVal tag As Object)

		''' <summary>
		''' Removes all highlights this highlighter is responsible for.
		''' </summary>
		Sub removeAllHighlights()

		''' <summary>
		''' Changes the given highlight to span a different portion of
		''' the document.  This may be more efficient than a remove/add
		''' when a selection is expanding/shrinking (such as a sweep
		''' with a mouse) by damaging only what changed.
		''' </summary>
		''' <param name="tag">  which highlight to change </param>
		''' <param name="p0"> the beginning of the range &gt;= 0 </param>
		''' <param name="p1"> the end of the range &gt;= p0 </param>
		''' <exception cref="BadLocationException"> for an invalid range specification </exception>
		Sub changeHighlight(ByVal tag As Object, ByVal p0 As Integer, ByVal p1 As Integer)

		''' <summary>
		''' Fetches the current list of highlights.
		''' </summary>
		''' <returns> the highlight list </returns>
		ReadOnly Property highlights As Highlight()

		''' <summary>
		''' Highlight renderer.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public interface HighlightPainter
	'	{
	'
	'		''' <summary>
	'		''' Renders the highlight.
	'		''' </summary>
	'		''' <param name="g"> the graphics context </param>
	'		''' <param name="p0"> the starting offset in the model &gt;= 0 </param>
	'		''' <param name="p1"> the ending offset in the model &gt;= p0 </param>
	'		''' <param name="bounds"> the bounding box for the highlight </param>
	'		''' <param name="c"> the editor </param>
	'		public void paint(Graphics g, int p0, int p1, Shape bounds, JTextComponent c);
	'
	'	}

'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public interface Highlight
	'	{
	'
	'		''' <summary>
	'		''' Gets the starting model offset for the highlight.
	'		''' </summary>
	'		''' <returns> the starting offset &gt;= 0 </returns>
	'		public int getStartOffset();
	'
	'		''' <summary>
	'		''' Gets the ending model offset for the highlight.
	'		''' </summary>
	'		''' <returns> the ending offset &gt;= 0 </returns>
	'		public int getEndOffset();
	'
	'		''' <summary>
	'		''' Gets the painter for the highlighter.
	'		''' </summary>
	'		''' <returns> the painter </returns>
	'		public HighlightPainter getPainter();
	'
	'	}

	End Interface

End Namespace
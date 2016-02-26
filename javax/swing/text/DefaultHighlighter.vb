Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports javax.swing.plaf
Imports javax.swing

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
	''' Implements the Highlighter interfaces.  Implements a simple highlight
	''' painter that renders in a solid color.
	''' 
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref=     Highlighter </seealso>
	Public Class DefaultHighlighter
		Inherits LayeredHighlighter

		''' <summary>
		''' Creates a new DefaultHighlighther object.
		''' </summary>
		Public Sub New()
			drawsLayeredHighlights = True
		End Sub

		' ---- Highlighter methods ----------------------------------------------

		''' <summary>
		''' Renders the highlights.
		''' </summary>
		''' <param name="g"> the graphics context </param>
		Public Overridable Sub paint(ByVal g As Graphics)
			' PENDING(prinz) - should cull ranges not visible
			Dim len As Integer = highlights.Count
			For i As Integer = 0 To len - 1
				Dim info As HighlightInfo = highlights(i)
				If Not(TypeOf info Is LayeredHighlightInfo) Then
					' Avoid allocing unless we need it.
					Dim a As Rectangle = component.bounds
					Dim insets As Insets = component.insets
					a.x = insets.left
					a.y = insets.top
					a.width -= insets.left + insets.right
					a.height -= insets.top + insets.bottom
					Do While i < len
						info = highlights(i)
						If Not(TypeOf info Is LayeredHighlightInfo) Then
							Dim p As Highlighter.HighlightPainter = info.painter
							p.paint(g, info.startOffset, info.endOffset, a, component)
						End If
						i += 1
					Loop
				End If
			Next i
		End Sub

		''' <summary>
		''' Called when the UI is being installed into the
		''' interface of a JTextComponent.  Installs the editor, and
		''' removes any existing highlights.
		''' </summary>
		''' <param name="c"> the editor component </param>
		''' <seealso cref= Highlighter#install </seealso>
		Public Overridable Sub install(ByVal c As JTextComponent)
			component = c
			removeAllHighlights()
		End Sub

		''' <summary>
		''' Called when the UI is being removed from the interface of
		''' a JTextComponent.
		''' </summary>
		''' <param name="c"> the component </param>
		''' <seealso cref= Highlighter#deinstall </seealso>
		Public Overridable Sub deinstall(ByVal c As JTextComponent)
			component = Nothing
		End Sub

		''' <summary>
		''' Adds a highlight to the view.  Returns a tag that can be used
		''' to refer to the highlight.
		''' </summary>
		''' <param name="p0">   the start offset of the range to highlight &gt;= 0 </param>
		''' <param name="p1">   the end offset of the range to highlight &gt;= p0 </param>
		''' <param name="p">    the painter to use to actually render the highlight </param>
		''' <returns>     an object that can be used as a tag
		'''   to refer to the highlight </returns>
		''' <exception cref="BadLocationException"> if the specified location is invalid </exception>
		Public Overridable Function addHighlight(ByVal p0 As Integer, ByVal p1 As Integer, ByVal p As Highlighter.HighlightPainter) As Object
			If p0 < 0 Then Throw New BadLocationException("Invalid start offset", p0)

			If p1 < p0 Then Throw New BadLocationException("Invalid end offset", p1)

			Dim doc As Document = component.document
			Dim i As HighlightInfo = If(drawsLayeredHighlights AndAlso (TypeOf p Is LayeredHighlighter.LayerPainter), New LayeredHighlightInfo(Me), New HighlightInfo(Me))
			i.painter = p
			i.p0 = doc.createPosition(p0)
			i.p1 = doc.createPosition(p1)
			highlights.Add(i)
			safeDamageRange(p0, p1)
			Return i
		End Function

		''' <summary>
		''' Removes a highlight from the view.
		''' </summary>
		''' <param name="tag"> the reference to the highlight </param>
		Public Overridable Sub removeHighlight(ByVal tag As Object)
			If TypeOf tag Is LayeredHighlightInfo Then
				Dim lhi As LayeredHighlightInfo = CType(tag, LayeredHighlightInfo)
				If lhi.width > 0 AndAlso lhi.height > 0 Then component.repaint(lhi.x, lhi.y, lhi.width, lhi.height)
			Else
				Dim info As HighlightInfo = CType(tag, HighlightInfo)
				safeDamageRange(info.p0, info.p1)
			End If
			highlights.Remove(tag)
		End Sub

		''' <summary>
		''' Removes all highlights.
		''' </summary>
		Public Overridable Sub removeAllHighlights()
			Dim mapper As TextUI = component.uI
			If drawsLayeredHighlights Then
				Dim len As Integer = highlights.Count
				If len <> 0 Then
					Dim minX As Integer = 0
					Dim minY As Integer = 0
					Dim maxX As Integer = 0
					Dim maxY As Integer = 0
					Dim p0 As Integer = -1
					Dim p1 As Integer = -1
					For i As Integer = 0 To len - 1
						Dim hi As HighlightInfo = highlights(i)
						If TypeOf hi Is LayeredHighlightInfo Then
							Dim info As LayeredHighlightInfo = CType(hi, LayeredHighlightInfo)
							minX = Math.Min(minX, info.x)
							minY = Math.Min(minY, info.y)
							maxX = Math.Max(maxX, info.x + info.width)
							maxY = Math.Max(maxY, info.y + info.height)
						Else
							If p0 = -1 Then
								p0 = hi.p0.offset
								p1 = hi.p1.offset
							Else
								p0 = Math.Min(p0, hi.p0.offset)
								p1 = Math.Max(p1, hi.p1.offset)
							End If
						End If
					Next i
					If minX <> maxX AndAlso minY <> maxY Then component.repaint(minX, minY, maxX - minX, maxY - minY)
					If p0 <> -1 Then
						Try
							safeDamageRange(p0, p1)
						Catch e As BadLocationException
						End Try
					End If
					highlights.Clear()
				End If
			ElseIf mapper IsNot Nothing Then
				Dim len As Integer = highlights.Count
				If len <> 0 Then
					Dim p0 As Integer = Integer.MaxValue
					Dim p1 As Integer = 0
					For i As Integer = 0 To len - 1
						Dim info As HighlightInfo = highlights(i)
						p0 = Math.Min(p0, info.p0.offset)
						p1 = Math.Max(p1, info.p1.offset)
					Next i
					Try
						safeDamageRange(p0, p1)
					Catch e As BadLocationException
					End Try

					highlights.Clear()
				End If
			End If
		End Sub

		''' <summary>
		''' Changes a highlight.
		''' </summary>
		''' <param name="tag"> the highlight tag </param>
		''' <param name="p0"> the beginning of the range &gt;= 0 </param>
		''' <param name="p1"> the end of the range &gt;= p0 </param>
		''' <exception cref="BadLocationException"> if the specified location is invalid </exception>
		Public Overridable Sub changeHighlight(ByVal tag As Object, ByVal p0 As Integer, ByVal p1 As Integer)
			If p0 < 0 Then Throw New BadLocationException("Invalid beginning of the range", p0)

			If p1 < p0 Then Throw New BadLocationException("Invalid end of the range", p1)

			Dim doc As Document = component.document
			If TypeOf tag Is LayeredHighlightInfo Then
				Dim lhi As LayeredHighlightInfo = CType(tag, LayeredHighlightInfo)
				If lhi.width > 0 AndAlso lhi.height > 0 Then component.repaint(lhi.x, lhi.y, lhi.width, lhi.height)
				' Mark the highlights region as invalid, it will reset itself
				' next time asked to paint.
					lhi.height = 0
					lhi.width = lhi.height
				lhi.p0 = doc.createPosition(p0)
				lhi.p1 = doc.createPosition(p1)
				safeDamageRange(Math.Min(p0, p1), Math.Max(p0, p1))
			Else
				Dim info As HighlightInfo = CType(tag, HighlightInfo)
				Dim oldP0 As Integer = info.p0.offset
				Dim oldP1 As Integer = info.p1.offset
				If p0 = oldP0 Then
					safeDamageRange(Math.Min(oldP1, p1), Math.Max(oldP1, p1))
				ElseIf p1 = oldP1 Then
					safeDamageRange(Math.Min(p0, oldP0), Math.Max(p0, oldP0))
				Else
					safeDamageRange(oldP0, oldP1)
					safeDamageRange(p0, p1)
				End If
				info.p0 = doc.createPosition(p0)
				info.p1 = doc.createPosition(p1)
			End If
		End Sub

		''' <summary>
		''' Makes a copy of the highlights.  Does not actually clone each highlight,
		''' but only makes references to them.
		''' </summary>
		''' <returns> the copy </returns>
		''' <seealso cref= Highlighter#getHighlights </seealso>
		Public Overridable Property highlights As Highlighter.Highlight()
			Get
				Dim size As Integer = highlights.Count
				If size = 0 Then Return noHighlights
				Dim h As Highlighter.Highlight() = New Highlighter.Highlight(size - 1){}
				highlights.CopyTo(h)
				Return h
			End Get
		End Property

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
		Public Overridable Sub paintLayeredHighlights(ByVal g As Graphics, ByVal p0 As Integer, ByVal p1 As Integer, ByVal viewBounds As Shape, ByVal editor As JTextComponent, ByVal ___view As View)
			For counter As Integer = highlights.Count - 1 To 0 Step -1
				Dim tag As HighlightInfo = highlights(counter)
				If TypeOf tag Is LayeredHighlightInfo Then
					Dim lhi As LayeredHighlightInfo = CType(tag, LayeredHighlightInfo)
					Dim start As Integer = lhi.startOffset
					Dim [end] As Integer = lhi.endOffset
					If (p0 < start AndAlso p1 > start) OrElse (p0 >= start AndAlso p0 < [end]) Then lhi.paintLayeredHighlights(g, p0, p1, viewBounds, editor, ___view)
				End If
			Next counter
		End Sub

		''' <summary>
		''' Queues damageRange() call into event dispatch thread
		''' to be sure that views are in consistent state.
		''' </summary>
		Private Sub safeDamageRange(ByVal p0 As Position, ByVal p1 As Position)
			safeDamager.damageRange(p0, p1)
		End Sub

		''' <summary>
		''' Queues damageRange() call into event dispatch thread
		''' to be sure that views are in consistent state.
		''' </summary>
		Private Sub safeDamageRange(ByVal a0 As Integer, ByVal a1 As Integer)
			Dim doc As Document = component.document
			safeDamageRange(doc.createPosition(a0), doc.createPosition(a1))
		End Sub

		''' <summary>
		''' If true, highlights are drawn as the Views draw the text. That is
		''' the Views will call into <code>paintLayeredHighlight</code> which
		''' will result in a rectangle being drawn before the text is drawn
		''' (if the offsets are in a highlighted region that is). For this to
		''' work the painter supplied must be an instance of
		''' LayeredHighlightPainter.
		''' </summary>
		Public Overridable Property drawsLayeredHighlights As Boolean
			Set(ByVal newValue As Boolean)
				drawsLayeredHighlights = newValue
			End Set
			Get
				Return drawsLayeredHighlights
			End Get
		End Property


		' ---- member variables --------------------------------------------

		Private Shared ReadOnly noHighlights As Highlighter.Highlight() = New Highlighter.Highlight(){}
		Private highlights As New List(Of HighlightInfo)
		Private component As JTextComponent
		Private drawsLayeredHighlights As Boolean
		Private safeDamager As New SafeDamager(Me)


		''' <summary>
		''' Default implementation of LayeredHighlighter.LayerPainter that can
		''' be used for painting highlights.
		''' <p>
		''' As of 1.4 this field is final.
		''' </summary>
		Public Shared ReadOnly DefaultPainter As LayeredHighlighter.LayerPainter = New DefaultHighlightPainter(Nothing)


		''' <summary>
		''' Simple highlight painter that fills a highlighted area with
		''' a solid color.
		''' </summary>
		Public Class DefaultHighlightPainter
			Inherits LayeredHighlighter.LayerPainter

			''' <summary>
			''' Constructs a new highlight painter. If <code>c</code> is null,
			''' the JTextComponent will be queried for its selection color.
			''' </summary>
			''' <param name="c"> the color for the highlight </param>
			Public Sub New(ByVal c As Color)
				color = c
			End Sub

			''' <summary>
			''' Returns the color of the highlight.
			''' </summary>
			''' <returns> the color </returns>
			Public Overridable Property color As Color
				Get
					Return color
				End Get
			End Property

			' --- HighlightPainter methods ---------------------------------------

			''' <summary>
			''' Paints a highlight.
			''' </summary>
			''' <param name="g"> the graphics context </param>
			''' <param name="offs0"> the starting model offset &gt;= 0 </param>
			''' <param name="offs1"> the ending model offset &gt;= offs1 </param>
			''' <param name="bounds"> the bounding box for the highlight </param>
			''' <param name="c"> the editor </param>
			Public Overridable Sub paint(ByVal g As Graphics, ByVal offs0 As Integer, ByVal offs1 As Integer, ByVal bounds As Shape, ByVal c As JTextComponent)
				Dim alloc As Rectangle = bounds.bounds
				Try
					' --- determine locations ---
					Dim mapper As TextUI = c.uI
					Dim p0 As Rectangle = mapper.modelToView(c, offs0)
					Dim p1 As Rectangle = mapper.modelToView(c, offs1)

					' --- render ---
					Dim ___color As Color = color

					If ___color Is Nothing Then
						g.color = c.selectionColor
					Else
						g.color = ___color
					End If
					If p0.y = p1.y Then
						' same line, render a rectangle
						Dim r As Rectangle = p0.union(p1)
						g.fillRect(r.x, r.y, r.width, r.height)
					Else
						' different lines
						Dim p0ToMarginWidth As Integer = alloc.x + alloc.width - p0.x
						g.fillRect(p0.x, p0.y, p0ToMarginWidth, p0.height)
						If (p0.y + p0.height) <> p1.y Then g.fillRect(alloc.x, p0.y + p0.height, alloc.width, p1.y - (p0.y + p0.height))
						g.fillRect(alloc.x, p1.y, (p1.x - alloc.x), p1.height)
					End If
				Catch e As BadLocationException
					' can't render
				End Try
			End Sub

			' --- LayerPainter methods ----------------------------
			''' <summary>
			''' Paints a portion of a highlight.
			''' </summary>
			''' <param name="g"> the graphics context </param>
			''' <param name="offs0"> the starting model offset &gt;= 0 </param>
			''' <param name="offs1"> the ending model offset &gt;= offs1 </param>
			''' <param name="bounds"> the bounding box of the view, which is not
			'''        necessarily the region to paint. </param>
			''' <param name="c"> the editor </param>
			''' <param name="view"> View painting for </param>
			''' <returns> region drawing occurred in </returns>
			Public Overridable Function paintLayer(ByVal g As Graphics, ByVal offs0 As Integer, ByVal offs1 As Integer, ByVal bounds As Shape, ByVal c As JTextComponent, ByVal ___view As View) As Shape
				Dim ___color As Color = color

				If ___color Is Nothing Then
					g.color = c.selectionColor
				Else
					g.color = ___color
				End If

				Dim r As Rectangle

				If offs0 = ___view.startOffset AndAlso offs1 = ___view.endOffset Then
					' Contained in view, can just use bounds.
					If TypeOf bounds Is Rectangle Then
						r = CType(bounds, Rectangle)
					Else
						r = bounds.bounds
					End If
				Else
					' Should only render part of View.
					Try
						' --- determine locations ---
						Dim shape As Shape = ___view.modelToView(offs0, Position.Bias.Forward, offs1,Position.Bias.Backward, bounds)
						r = If(TypeOf shape Is Rectangle, CType(shape, Rectangle), shape.bounds)
					Catch e As BadLocationException
						' can't render
						r = Nothing
					End Try
				End If

				If r IsNot Nothing Then
					' If we are asked to highlight, we should draw something even
					' if the model-to-view projection is of zero width (6340106).
					r.width = Math.Max(r.width, 1)

					g.fillRect(r.x, r.y, r.width, r.height)
				End If

				Return r
			End Function

			Private color As Color

		End Class


		Friend Class HighlightInfo
			Implements Highlighter.Highlight

			Private ReadOnly outerInstance As DefaultHighlighter

			Public Sub New(ByVal outerInstance As DefaultHighlighter)
				Me.outerInstance = outerInstance
			End Sub


			Public Overridable Property startOffset As Integer
				Get
					Return p0.offset
				End Get
			End Property

			Public Overridable Property endOffset As Integer
				Get
					Return p1.offset
				End Get
			End Property

			Public Overridable Property painter As Highlighter.HighlightPainter
				Get
					Return painter
				End Get
			End Property

			Friend p0 As Position
			Friend p1 As Position
			Friend painter As Highlighter.HighlightPainter
		End Class


		''' <summary>
		''' LayeredHighlightPainter is used when a drawsLayeredHighlights is
		''' true. It maintains a rectangle of the region to paint.
		''' </summary>
		Friend Class LayeredHighlightInfo
			Inherits HighlightInfo

			Private ReadOnly outerInstance As DefaultHighlighter

			Public Sub New(ByVal outerInstance As DefaultHighlighter)
				Me.outerInstance = outerInstance
			End Sub


			Friend Overridable Sub union(ByVal bounds As Shape)
				If bounds Is Nothing Then Return

				Dim alloc As Rectangle
				If TypeOf bounds Is Rectangle Then
					alloc = CType(bounds, Rectangle)
				Else
					alloc = bounds.bounds
				End If
				If width = 0 OrElse height = 0 Then
					x = alloc.x
					y = alloc.y
					width = alloc.width
					height = alloc.height
				Else
					width = Math.Max(x + width, alloc.x + alloc.width)
					height = Math.Max(y + height, alloc.y + alloc.height)
					x = Math.Min(x, alloc.x)
					width -= x
					y = Math.Min(y, alloc.y)
					height -= y
				End If
			End Sub

			''' <summary>
			''' Restricts the region based on the receivers offsets and messages
			''' the painter to paint the region.
			''' </summary>
			Friend Overridable Sub paintLayeredHighlights(ByVal g As Graphics, ByVal p0 As Integer, ByVal p1 As Integer, ByVal viewBounds As Shape, ByVal editor As JTextComponent, ByVal ___view As View)
				Dim start As Integer = startOffset
				Dim [end] As Integer = endOffset
				' Restrict the region to what we represent
				p0 = Math.Max(start, p0)
				p1 = Math.Min([end], p1)
				' Paint the appropriate region using the painter and union
				' the effected region with our bounds.
				union(CType(painter, LayeredHighlighter.LayerPainter).paintLayer(g, p0, p1, viewBounds, editor, ___view))
			End Sub

			Friend x As Integer
			Friend y As Integer
			Friend width As Integer
			Friend height As Integer
		End Class

		''' <summary>
		''' This class invokes <code>mapper.damageRange</code> in
		''' EventDispatchThread. The only one instance per Highlighter
		''' is cretaed. When a number of ranges should be damaged
		''' it collects them into queue and damages
		''' them in consecutive order in <code>run</code>
		''' call.
		''' </summary>
		Friend Class SafeDamager
			Implements Runnable

			Private ReadOnly outerInstance As DefaultHighlighter

			Public Sub New(ByVal outerInstance As DefaultHighlighter)
				Me.outerInstance = outerInstance
			End Sub

			Private p0 As New List(Of Position)(10)
			Private p1 As New List(Of Position)(10)
			Private lastDoc As Document = Nothing

			''' <summary>
			''' Executes range(s) damage and cleans range queue.
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Sub run()
				If outerInstance.component IsNot Nothing Then
					Dim mapper As TextUI = outerInstance.component.uI
					If mapper IsNot Nothing AndAlso lastDoc Is outerInstance.component.document Then
						' the Document should be the same to properly
						' display highlights
						Dim len As Integer = p0.Count
						For i As Integer = 0 To len - 1
							mapper.damageRange(outerInstance.component, p0(i).offset, p1(i).offset)
						Next i
					End If
				End If
				p0.Clear()
				p1.Clear()

				' release reference
				lastDoc = Nothing
			End Sub

			''' <summary>
			''' Adds the range to be damaged into the range queue. If the
			''' range queue is empty (the first call or run() was already
			''' invoked) then adds this class instance into EventDispatch
			''' queue.
			''' 
			''' The method also tracks if the current document changed or
			''' component is null. In this case it removes all ranges added
			''' before from range queue.
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Sub damageRange(ByVal pos0 As Position, ByVal pos1 As Position)
				If outerInstance.component Is Nothing Then
					p0.Clear()
					lastDoc = Nothing
					Return
				End If

				Dim addToQueue As Boolean = p0.Count = 0
				Dim curDoc As Document = outerInstance.component.document
				If curDoc IsNot lastDoc Then
					If p0.Count > 0 Then
						p0.Clear()
						p1.Clear()
					End If
					lastDoc = curDoc
				End If
				p0.Add(pos0)
				p1.Add(pos1)

				If addToQueue Then SwingUtilities.invokeLater(Me)
			End Sub
		End Class
	End Class

End Namespace
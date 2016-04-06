Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1998, 2011, Oracle and/or its affiliates. All rights reserved.
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

'
' * (C) Copyright IBM Corp. 1998-2003, All Rights Reserved
' *
' 

Namespace java.awt.font



	Friend NotInheritable Class TextLine

		Friend NotInheritable Class TextLineMetrics
			Public ReadOnly ascent As Single
			Public ReadOnly descent As Single
			Public ReadOnly leading As Single
			Public ReadOnly advance As Single

			Public Sub New(  ascent As Single,   descent As Single,   leading As Single,   advance As Single)
				Me.ascent = ascent
				Me.descent = descent
				Me.leading = leading
				Me.advance = advance
			End Sub
		End Class

		Private fComponents As sun.font.TextLineComponent()
		Private fBaselineOffsets As Single()
		Private fComponentVisualOrder As Integer() ' if null, ltr
		Private locs As Single() ' x,y pairs for components in visual order
		Private fChars As Char()
		Private fCharsStart As Integer
		Private fCharsLimit As Integer
		Private fCharVisualOrder As Integer() ' if null, ltr
		Private fCharLogicalOrder As Integer() ' if null, ltr
		Private fCharLevels As SByte() ' if null, 0
		Private fIsDirectionLTR As Boolean
		Private lp As sun.font.LayoutPathImpl
		Private isSimple As Boolean
		Private pixelBounds As java.awt.Rectangle
		Private frc As FontRenderContext

		Private fMetrics As TextLineMetrics = Nothing ' built on demand in getMetrics

		Public Sub New(  frc As FontRenderContext,   components As sun.font.TextLineComponent(),   baselineOffsets As Single(),   chars As Char(),   charsStart As Integer,   charsLimit As Integer,   charLogicalOrder As Integer(),   charLevels As SByte(),   isDirectionLTR As Boolean)

			Dim componentVisualOrder As Integer() = computeComponentOrder(components, charLogicalOrder)

			Me.frc = frc
			fComponents = components
			fBaselineOffsets = baselineOffsets
			fComponentVisualOrder = componentVisualOrder
			fChars = chars
			fCharsStart = charsStart
			fCharsLimit = charsLimit
			fCharLogicalOrder = charLogicalOrder
			fCharLevels = charLevels
			fIsDirectionLTR = isDirectionLTR
			checkCtorArgs()

			init()
		End Sub

		Private Sub checkCtorArgs()

			Dim checkCharCount As Integer = 0
			For i As Integer = 0 To fComponents.Length - 1
				checkCharCount += fComponents(i).numCharacters
			Next i

			If checkCharCount <> Me.characterCount() Then Throw New IllegalArgumentException("Invalid TextLine!  " & "char count is different from " & "sum of char counts of components.")
		End Sub

		Private Sub init()

			' first, we need to check for graphic components on the TOP or BOTTOM baselines.  So
			' we perform the work that used to be in getMetrics here.

			Dim ascent As Single = 0
			Dim descent As Single = 0
			Dim leading As Single = 0
			Dim advance As Single = 0

			' ascent + descent must not be less than this value
			Dim maxGraphicHeight As Single = 0
			Dim maxGraphicHeightWithLeading As Single = 0

			' walk through EGA's
			Dim tlc As sun.font.TextLineComponent
			Dim fitTopAndBottomGraphics As Boolean = False

			isSimple = True

			For i As Integer = 0 To fComponents.Length - 1
				tlc = fComponents(i)

				isSimple = isSimple And tlc.simple

				Dim cm As sun.font.CoreMetrics = tlc.coreMetrics

				Dim baseline As SByte = CByte(cm.baselineIndex)

				If baseline >= 0 Then
					Dim baselineOffset As Single = fBaselineOffsets(baseline)

					ascent = System.Math.Max(ascent, -baselineOffset + cm.ascent)

					Dim gd As Single = baselineOffset + cm.descent
					descent = System.Math.Max(descent, gd)

					leading = System.Math.Max(leading, gd + cm.leading)
				Else
					fitTopAndBottomGraphics = True
					Dim graphicHeight As Single = cm.ascent + cm.descent
					Dim graphicHeightWithLeading As Single = graphicHeight + cm.leading
					maxGraphicHeight = System.Math.Max(maxGraphicHeight, graphicHeight)
					maxGraphicHeightWithLeading = System.Math.Max(maxGraphicHeightWithLeading, graphicHeightWithLeading)
				End If
			Next i

			If fitTopAndBottomGraphics Then
				If maxGraphicHeight > ascent + descent Then descent = maxGraphicHeight - ascent
				If maxGraphicHeightWithLeading > ascent + leading Then leading = maxGraphicHeightWithLeading - ascent
			End If

			leading -= descent

			' we now know enough to compute the locs, but we need the final loc
			' for the advance before we can create the metrics object

			If fitTopAndBottomGraphics Then fBaselineOffsets = New Single() { fBaselineOffsets(0), fBaselineOffsets(1), fBaselineOffsets(2), descent, -ascent }

			Dim x As Single = 0
			Dim y As Single = 0
			Dim pcm As sun.font.CoreMetrics = Nothing

			Dim needPath As Boolean = False
			locs = New Single(fComponents.Length * 2 + 2 - 1){}

			Dim i As Integer = 0
			Dim n As Integer = 0
			Do While i < fComponents.Length
				tlc = fComponents(getComponentLogicalIndex(i))
				Dim cm As sun.font.CoreMetrics = tlc.coreMetrics

				If (pcm IsNot Nothing) AndAlso (pcm.italicAngle <> 0 OrElse cm.italicAngle <> 0) AndAlso (pcm.italicAngle <> cm.italicAngle OrElse pcm.baselineIndex <> cm.baselineIndex OrElse pcm.ssOffset <> cm.ssOffset) Then ' adjust because of italics

					' 1) compute the area of overlap - min effective ascent and min effective descent
					' 2) compute the x positions along italic angle of ascent and descent for left and right
					' 3) compute maximum left - right, adjust right position by this value
					' this is a crude form of kerning between textcomponents

					' note glyphvectors preposition glyphs based on offset,
					' so tl doesn't need to adjust glyphvector position
					' 1)
					Dim pb As Single = pcm.effectiveBaselineOffset(fBaselineOffsets)
					Dim pa As Single = pb - pcm.ascent
					Dim pd As Single = pb + pcm.descent
					' pb += pcm.ssOffset;

					Dim cb As Single = cm.effectiveBaselineOffset(fBaselineOffsets)
					Dim ca As Single = cb - cm.ascent
					Dim cd As Single = cb + cm.descent
					' cb += cm.ssOffset;

					Dim a As Single = System.Math.Max(pa, ca)
					Dim d As Single = System.Math.Min(pd, cd)

					' 2)
					Dim pax As Single = pcm.italicAngle * (pb - a)
					Dim pdx As Single = pcm.italicAngle * (pb - d)

					Dim cax As Single = cm.italicAngle * (cb - a)
					Dim cdx As Single = cm.italicAngle * (cb - d)

					' 3)
					Dim dax As Single = pax - cax
					Dim ddx As Single = pdx - cdx
					Dim dx As Single = System.Math.Max(dax, ddx)

					x += dx
					y = cb
				Else
					' no italic adjustment for x, but still need to compute y
					y = cm.effectiveBaselineOffset(fBaselineOffsets) ' + cm.ssOffset;
				End If

				locs(n) = x
				locs(n+1) = y

				x += tlc.advance
				pcm = cm

				needPath = needPath Or tlc.baselineTransform IsNot Nothing
				i += 1
				n += 2
			Loop

			' do we want italic padding at the right of the line?
			If pcm.italicAngle <> 0 Then
				Dim pb As Single = pcm.effectiveBaselineOffset(fBaselineOffsets)
				Dim pa As Single = pb - pcm.ascent
				Dim pd As Single = pb + pcm.descent
				pb += pcm.ssOffset

				Dim d As Single
				If pcm.italicAngle > 0 Then
					d = pb + pcm.ascent
				Else
					d = pb - pcm.descent
				End If
				d *= pcm.italicAngle

				x += d
			End If
			locs(locs.Length - 2) = x
			' locs[locs.length - 1] = 0; // final offset is always back on baseline

			' ok, build fMetrics since we have the final advance
			advance = x
			fMetrics = New TextLineMetrics(ascent, descent, leading, advance)

			' build path if we need it
			If needPath Then
				isSimple = False

				Dim pt As New java.awt.geom.Point2D.Double
				Dim tx As Double = 0, ty As Double = 0
				Dim builder As New sun.font.LayoutPathImpl.SegmentPathBuilder
				builder.moveTo(locs(0), 0)
				i = 0
				n = 0
				Do While i < fComponents.Length
					tlc = fComponents(getComponentLogicalIndex(i))
					Dim at As java.awt.geom.AffineTransform = tlc.baselineTransform
					If at IsNot Nothing AndAlso ((at.type And java.awt.geom.AffineTransform.TYPE_TRANSLATION) <> 0) Then
						Dim dx As Double = at.translateX
						Dim dy As Double = at.translateY
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						builder.moveTo(tx += dx, ty += dy)
					End If
					pt.x = locs(n+2) - locs(n)
					pt.y = 0
					If at IsNot Nothing Then at.deltaTransform(pt, pt)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					builder.lineTo(tx += pt.x, ty += pt.y)
					i += 1
					n += 2
				Loop
				lp = builder.complete()

				If lp Is Nothing Then ' empty path
					tlc = fComponents(getComponentLogicalIndex(0))
					Dim at As java.awt.geom.AffineTransform = tlc.baselineTransform
					If at IsNot Nothing Then lp = New sun.font.LayoutPathImpl.EmptyPath(at)
				End If
			End If
		End Sub

		Public Function getPixelBounds(  frc As FontRenderContext,   x As Single,   y As Single) As java.awt.Rectangle
			Dim result As java.awt.Rectangle = Nothing

			' if we have a matching frc, set it to null so we don't have to test it
			' for each component
			If frc IsNot Nothing AndAlso frc.Equals(Me.frc) Then frc = Nothing

			' only cache integral locations with the default frc, this is a bit strict
			Dim ix As Integer = CInt(Fix (System.Math.Floor(x)))
			Dim iy As Integer = CInt(Fix (System.Math.Floor(y)))
			Dim rx As Single = x - ix
			Dim ry As Single = y - iy
			Dim canCache As Boolean = frc Is Nothing AndAlso rx = 0 AndAlso ry = 0

			If canCache AndAlso pixelBounds IsNot Nothing Then
				result = New java.awt.Rectangle(pixelBounds)
				result.x += ix
				result.y += iy
				Return result
			End If

			' couldn't use cache, or didn't have it, so compute

			If isSimple Then ' all glyphvectors with no decorations, no layout path
				Dim i As Integer = 0
				Dim n As Integer = 0
				Do While i < fComponents.Length
					Dim tlc As sun.font.TextLineComponent = fComponents(getComponentLogicalIndex(i))
					Dim pb As java.awt.Rectangle = tlc.getPixelBounds(frc, locs(n) + rx, locs(n+1) + ry)
					If Not pb.empty Then
						If result Is Nothing Then
							result = pb
						Else
							result.add(pb)
						End If
					End If
					i += 1
					n += 2
				Loop
				If result Is Nothing Then
					result = New java.awt.Rectangle(0, 0, 0, 0)
				End If ' draw and test
			Else
				Const MARGIN As Integer = 3
				Dim r2d As java.awt.geom.Rectangle2D = visualBounds
				If lp IsNot Nothing Then r2d = lp.mapShape(r2d).bounds
				Dim bounds As java.awt.Rectangle = r2d.bounds
				Dim im As New java.awt.image.BufferedImage(bounds.width + MARGIN * 2, bounds.height + MARGIN * 2, java.awt.image.BufferedImage.TYPE_INT_ARGB)

				Dim g2d As java.awt.Graphics2D = im.createGraphics()
				g2d.color = java.awt.Color.WHITE
				g2d.fillRect(0, 0, im.width, im.height)

				g2d.color = java.awt.Color.BLACK
				draw(g2d, rx + MARGIN - bounds.x, ry + MARGIN - bounds.y)

				result = computePixelBounds(im)
				result.x -= MARGIN - bounds.x
				result.y -= MARGIN - bounds.y
			End If

			If canCache Then pixelBounds = New java.awt.Rectangle(result)

			result.x += ix
			result.y += iy
			Return result
		End Function

		Friend Shared Function computePixelBounds(  im As java.awt.image.BufferedImage) As java.awt.Rectangle
			Dim w As Integer = im.width
			Dim h As Integer = im.height

			Dim l As Integer = -1, t As Integer = -1, r As Integer = w, b As Integer = h

				' get top
				Dim buf As Integer() = New Integer(w - 1){}
				loop:
				t += 1
				Do While t < h
					im.getRGB(0, t, buf.Length, 1, buf, 0, w) ' w ignored
					For i As Integer = 0 To buf.Length - 1
						If buf(i) <> -1 Then GoTo loop
					Next i
					t += 1
				Loop

			' get bottom
				Dim buf As Integer() = New Integer(w - 1){}
				loop:
				b -= 1
				Do While b > t
					im.getRGB(0, b, buf.Length, 1, buf, 0, w) ' w ignored
					For i As Integer = 0 To buf.Length - 1
						If buf(i) <> -1 Then GoTo loop
					Next i
					b -= 1
				Loop
				b += 1

			' get left
				loop:
				l += 1
				Do While l < r
					For i As Integer = t To b - 1
						Dim v As Integer = im.getRGB(l, i)
						If v <> -1 Then GoTo loop
					Next i
					l += 1
				Loop

			' get right
				loop:
				r -= 1
				Do While r > l
					For i As Integer = t To b - 1
						Dim v As Integer = im.getRGB(r, i)
						If v <> -1 Then GoTo loop
					Next i
					r -= 1
				Loop
				r += 1

			Return New java.awt.Rectangle(l, t, r-l, b-t)
		End Function

		Private MustInherit Class [Function]

			Friend MustOverride Function computeFunction(  line As TextLine,   componentIndex As Integer,   indexInArray As Integer) As Single
		End Class

		Private Shared fgPosAdvF As [Function] = New FunctionAnonymousInnerClassHelper

		Private Class FunctionAnonymousInnerClassHelper
			Inherits [Function]

			Friend Overrides Function computeFunction(  line As TextLine,   componentIndex As Integer,   indexInArray As Integer) As Single

				Dim tlc As sun.font.TextLineComponent = line.fComponents(componentIndex)
					Dim vi As Integer = line.getComponentVisualIndex(componentIndex)
				Return line.locs(vi * 2) + tlc.getCharX(indexInArray) + tlc.getCharAdvance(indexInArray)
			End Function
		End Class

		Private Shared fgAdvanceF As [Function] = New FunctionAnonymousInnerClassHelper2

		Private Class FunctionAnonymousInnerClassHelper2
			Inherits [Function]

			Friend Overrides Function computeFunction(  line As TextLine,   componentIndex As Integer,   indexInArray As Integer) As Single

				Dim tlc As sun.font.TextLineComponent = line.fComponents(componentIndex)
				Return tlc.getCharAdvance(indexInArray)
			End Function
		End Class

		Private Shared fgXPositionF As [Function] = New FunctionAnonymousInnerClassHelper3

		Private Class FunctionAnonymousInnerClassHelper3
			Inherits [Function]

			Friend Overrides Function computeFunction(  line As TextLine,   componentIndex As Integer,   indexInArray As Integer) As Single

					Dim vi As Integer = line.getComponentVisualIndex(componentIndex)
				Dim tlc As sun.font.TextLineComponent = line.fComponents(componentIndex)
				Return line.locs(vi * 2) + tlc.getCharX(indexInArray)
			End Function
		End Class

		Private Shared fgYPositionF As [Function] = New FunctionAnonymousInnerClassHelper4

		Private Class FunctionAnonymousInnerClassHelper4
			Inherits [Function]

			Friend Overrides Function computeFunction(  line As TextLine,   componentIndex As Integer,   indexInArray As Integer) As Single

				Dim tlc As sun.font.TextLineComponent = line.fComponents(componentIndex)
				Dim charPos As Single = tlc.getCharY(indexInArray)

				' charPos is relative to the component - adjust for
				' baseline

				Return charPos + line.getComponentShift(componentIndex)
			End Function
		End Class

		Public Function characterCount() As Integer

			Return fCharsLimit - fCharsStart
		End Function

		Public Property directionLTR As Boolean
			Get
    
				Return fIsDirectionLTR
			End Get
		End Property

		Public Property metrics As TextLineMetrics
			Get
				Return fMetrics
			End Get
		End Property

		Public Function visualToLogical(  visualIndex As Integer) As Integer

			If fCharLogicalOrder Is Nothing Then Return visualIndex

			If fCharVisualOrder Is Nothing Then fCharVisualOrder = sun.font.BidiUtils.createInverseMap(fCharLogicalOrder)

			Return fCharVisualOrder(visualIndex)
		End Function

		Public Function logicalToVisual(  logicalIndex As Integer) As Integer

			Return If(fCharLogicalOrder Is Nothing, logicalIndex, fCharLogicalOrder(logicalIndex))
		End Function

		Public Function getCharLevel(  logicalIndex As Integer) As SByte

			Return If(fCharLevels Is Nothing, 0, fCharLevels(logicalIndex))
		End Function

		Public Function isCharLTR(  logicalIndex As Integer) As Boolean

			Return (getCharLevel(logicalIndex) And &H1) = 0
		End Function

		Public Function getCharType(  logicalIndex As Integer) As Integer

			Return Character.getType(fChars(logicalIndex + fCharsStart))
		End Function

		Public Function isCharSpace(  logicalIndex As Integer) As Boolean

			Return Character.isSpaceChar(fChars(logicalIndex + fCharsStart))
		End Function

		Public Function isCharWhitespace(  logicalIndex As Integer) As Boolean

			Return Char.IsWhiteSpace(fChars(logicalIndex + fCharsStart))
		End Function

		Public Function getCharAngle(  logicalIndex As Integer) As Single

			Return getCoreMetricsAt(logicalIndex).italicAngle
		End Function

		Public Function getCoreMetricsAt(  logicalIndex As Integer) As sun.font.CoreMetrics

			If logicalIndex < 0 Then Throw New IllegalArgumentException("Negative logicalIndex.")

			If logicalIndex > fCharsLimit - fCharsStart Then Throw New IllegalArgumentException("logicalIndex too large.")

			Dim currentTlc As Integer = 0
			Dim tlcStart As Integer = 0
			Dim tlcLimit As Integer = 0

			Do
				tlcLimit += fComponents(currentTlc).numCharacters
				If tlcLimit > logicalIndex Then Exit Do
				currentTlc += 1
				tlcStart = tlcLimit
			Loop While currentTlc < fComponents.Length

			Return fComponents(currentTlc).coreMetrics
		End Function

		Public Function getCharAscent(  logicalIndex As Integer) As Single

			Return getCoreMetricsAt(logicalIndex).ascent
		End Function

		Public Function getCharDescent(  logicalIndex As Integer) As Single

			Return getCoreMetricsAt(logicalIndex).descent
		End Function

		Public Function getCharShift(  logicalIndex As Integer) As Single

			Return getCoreMetricsAt(logicalIndex).ssOffset
		End Function

		Private Function applyFunctionAtIndex(  logicalIndex As Integer,   f As [Function]) As Single

			If logicalIndex < 0 Then Throw New IllegalArgumentException("Negative logicalIndex.")

			Dim tlcStart As Integer = 0

			For i As Integer = 0 To fComponents.Length - 1

				Dim tlcLimit As Integer = tlcStart + fComponents(i).numCharacters
				If tlcLimit > logicalIndex Then
					Return f.computeFunction(Me, i, logicalIndex - tlcStart)
				Else
					tlcStart = tlcLimit
				End If
			Next i

			Throw New IllegalArgumentException("logicalIndex too large.")
		End Function

		Public Function getCharAdvance(  logicalIndex As Integer) As Single

			Return applyFunctionAtIndex(logicalIndex, fgAdvanceF)
		End Function

		Public Function getCharXPosition(  logicalIndex As Integer) As Single

			Return applyFunctionAtIndex(logicalIndex, fgXPositionF)
		End Function

		Public Function getCharYPosition(  logicalIndex As Integer) As Single

			Return applyFunctionAtIndex(logicalIndex, fgYPositionF)
		End Function

		Public Function getCharLinePosition(  logicalIndex As Integer) As Single

			Return getCharXPosition(logicalIndex)
		End Function

		Public Function getCharLinePosition(  logicalIndex As Integer,   leading As Boolean) As Single
			Dim f As [Function] = If(isCharLTR(logicalIndex) = leading, fgXPositionF, fgPosAdvF)
			Return applyFunctionAtIndex(logicalIndex, f)
		End Function

		Public Function caretAtOffsetIsValid(  offset As Integer) As Boolean

			If offset < 0 Then Throw New IllegalArgumentException("Negative offset.")

			Dim tlcStart As Integer = 0

			For i As Integer = 0 To fComponents.Length - 1

				Dim tlcLimit As Integer = tlcStart + fComponents(i).numCharacters
				If tlcLimit > offset Then
					Return fComponents(i).caretAtOffsetIsValid(offset-tlcStart)
				Else
					tlcStart = tlcLimit
				End If
			Next i

			Throw New IllegalArgumentException("logicalIndex too large.")
		End Function

		''' <summary>
		''' map a component visual index to the logical index.
		''' </summary>
		Private Function getComponentLogicalIndex(  vi As Integer) As Integer
			If fComponentVisualOrder Is Nothing Then Return vi
			Return fComponentVisualOrder(vi)
		End Function

		''' <summary>
		''' map a component logical index to the visual index.
		''' </summary>
		Private Function getComponentVisualIndex(  li As Integer) As Integer
			If fComponentVisualOrder Is Nothing Then Return li
			For i As Integer = 0 To fComponentVisualOrder.Length - 1
					If fComponentVisualOrder(i) = li Then Return i
			Next i
			Throw New IndexOutOfBoundsException("bad component index: " & li)
		End Function

		Public Function getCharBounds(  logicalIndex As Integer) As java.awt.geom.Rectangle2D

			If logicalIndex < 0 Then Throw New IllegalArgumentException("Negative logicalIndex.")

			Dim tlcStart As Integer = 0

			For i As Integer = 0 To fComponents.Length - 1

				Dim tlcLimit As Integer = tlcStart + fComponents(i).numCharacters
				If tlcLimit > logicalIndex Then

					Dim tlc As sun.font.TextLineComponent = fComponents(i)
					Dim indexInTlc As Integer = logicalIndex - tlcStart
					Dim chBounds As java.awt.geom.Rectangle2D = tlc.getCharVisualBounds(indexInTlc)

							Dim vi As Integer = getComponentVisualIndex(i)
					chBounds.rectect(chBounds.x + locs(vi * 2), chBounds.y + locs(vi * 2 + 1), chBounds.width, chBounds.height)
					Return chBounds
				Else
					tlcStart = tlcLimit
				End If
			Next i

			Throw New IllegalArgumentException("logicalIndex too large.")
		End Function

		Private Function getComponentShift(  index As Integer) As Single
			Dim cm As sun.font.CoreMetrics = fComponents(index).coreMetrics
			Return cm.effectiveBaselineOffset(fBaselineOffsets)
		End Function

		Public Sub draw(  g2 As java.awt.Graphics2D,   x As Single,   y As Single)
			If lp Is Nothing Then
				Dim i As Integer = 0
				Dim n As Integer = 0
				Do While i < fComponents.Length
					Dim tlc As sun.font.TextLineComponent = fComponents(getComponentLogicalIndex(i))
					tlc.draw(g2, locs(n) + x, locs(n+1) + y)
					i += 1
					n += 2
				Loop
			Else
				Dim oldTx As java.awt.geom.AffineTransform = g2.transform
				Dim pt As New java.awt.geom.Point2D.Float
				Dim i As Integer = 0
				Dim n As Integer = 0
				Do While i < fComponents.Length
					Dim tlc As sun.font.TextLineComponent = fComponents(getComponentLogicalIndex(i))
					lp.pathToPoint(locs(n), locs(n+1), False, pt)
					pt.x += x
					pt.y += y
					Dim at As java.awt.geom.AffineTransform = tlc.baselineTransform

					If at IsNot Nothing Then
						g2.translate(pt.x - at.translateX, pt.y - at.translateY)
						g2.transform(at)
						tlc.draw(g2, 0, 0)
						g2.transform = oldTx
					Else
						tlc.draw(g2, pt.x, pt.y)
					End If
					i += 1
					n += 2
				Loop
			End If
		End Sub

		''' <summary>
		''' Return the union of the visual bounds of all the components.
		''' This incorporates the path.  It does not include logical
		''' bounds (used by carets).
		''' </summary>
		Public Property visualBounds As java.awt.geom.Rectangle2D
			Get
				Dim result As java.awt.geom.Rectangle2D = Nothing
    
				Dim i As Integer = 0
				Dim n As Integer = 0
				Do While i < fComponents.Length
					Dim tlc As sun.font.TextLineComponent = fComponents(getComponentLogicalIndex(i))
					Dim r As java.awt.geom.Rectangle2D = tlc.visualBounds
    
					Dim pt As New java.awt.geom.Point2D.Float(locs(n), locs(n+1))
					If lp Is Nothing Then
						r.rectect(r.minX + pt.x, r.minY + pt.y, r.width, r.height)
					Else
						lp.pathToPoint(pt, False, pt)
    
						Dim at As java.awt.geom.AffineTransform = tlc.baselineTransform
						If at IsNot Nothing Then
							Dim tx As java.awt.geom.AffineTransform = java.awt.geom.AffineTransform.getTranslateInstance(pt.x - at.translateX, pt.y - at.translateY)
							tx.concatenate(at)
							r = tx.createTransformedShape(r).bounds2D
						Else
							r.rectect(r.minX + pt.x, r.minY + pt.y, r.width, r.height)
						End If
					End If
    
					If result Is Nothing Then
						result = r
					Else
						result.add(r)
					End If
					i += 1
					n += 2
				Loop
    
				If result Is Nothing Then result = New java.awt.geom.Rectangle2D.Float(Float.Max_Value, Float.Max_Value, Float.MIN_VALUE, Float.MIN_VALUE)
    
				Return result
			End Get
		End Property

		Public Property italicBounds As java.awt.geom.Rectangle2D
			Get
    
				Dim left As Single = Float.Max_Value, right As Single = -Float.Max_Value
				Dim top As Single = Float.Max_Value, bottom As Single = -Float.Max_Value
    
				Dim i As Integer=0
				Dim n As Integer = 0
				Do While i < fComponents.Length
					Dim tlc As sun.font.TextLineComponent = fComponents(getComponentLogicalIndex(i))
    
					Dim tlcBounds As java.awt.geom.Rectangle2D = tlc.italicBounds
					Dim x As Single = locs(n)
					Dim y As Single = locs(n+1)
    
					left = System.Math.Min(left, x + CSng(tlcBounds.x))
					right = System.Math.Max(right, x + CSng(tlcBounds.maxX))
    
					top = System.Math.Min(top, y + CSng(tlcBounds.y))
					bottom = System.Math.Max(bottom, y + CSng(tlcBounds.maxY))
					i += 1
					n += 2
				Loop
    
				Return New java.awt.geom.Rectangle2D.Float(left, top, right-left, bottom-top)
			End Get
		End Property

		Public Function getOutline(  tx As java.awt.geom.AffineTransform) As java.awt.Shape

			Dim dstShape As New java.awt.geom.GeneralPath(java.awt.geom.GeneralPath.WIND_NON_ZERO)

			Dim i As Integer=0
			Dim n As Integer = 0
			Do While i < fComponents.Length
				Dim tlc As sun.font.TextLineComponent = fComponents(getComponentLogicalIndex(i))

				dstShape.append(tlc.getOutline(locs(n), locs(n+1)), False)
				i += 1
				n += 2
			Loop

			If tx IsNot Nothing Then dstShape.transform(tx)
			Return dstShape
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return (fComponents.Length << 16) Xor (fComponents(0).GetHashCode() << 3) Xor (fCharsLimit-fCharsStart)
		End Function

		Public Overrides Function ToString() As String
			Dim buf As New StringBuilder

			For i As Integer = 0 To fComponents.Length - 1
				buf.append(fComponents(i))
			Next i

			Return buf.ToString()
		End Function

		''' <summary>
		''' Create a TextLine from the text.  The Font must be able to
		''' display all of the text.
		''' attributes==null is equivalent to using an empty Map for
		''' attributes
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function fastCreateTextLine(Of T1 As java.text.AttributedCharacterIterator.Attribute, ?)(  frc As FontRenderContext,   chars As Char(),   font_Renamed As java.awt.Font,   lm As sun.font.CoreMetrics,   attributes As IDictionary(Of T1)) As TextLine

			Dim isDirectionLTR As Boolean = True
			Dim levels As SByte() = Nothing
			Dim charsLtoV As Integer() = Nothing
			Dim bidi As java.text.Bidi = Nothing
			Dim characterCount As Integer = chars.Length

			Dim requiresBidi As Boolean = False
			Dim embs As SByte() = Nothing

			Dim values As sun.font.AttributeValues = Nothing
			If attributes IsNot Nothing Then
				values = sun.font.AttributeValues.fromMap(attributes)
				If values.runDirection >= 0 Then
					isDirectionLTR = values.runDirection = 0
					requiresBidi = Not isDirectionLTR
				End If
				If values.bidiEmbedding <> 0 Then
					requiresBidi = True
					Dim level As SByte = CByte(values.bidiEmbedding)
					embs = New SByte(characterCount - 1){}
					For i As Integer = 0 To embs.Length - 1
						embs(i) = level
					Next i
				End If
			End If

			' dlf: get baseRot from font for now???

			If Not requiresBidi Then requiresBidi = java.text.Bidi.requiresBidi(chars, 0, chars.Length)

			If requiresBidi Then
			  Dim bidiflags As Integer = If(values Is Nothing, java.text.Bidi.DIRECTION_DEFAULT_LEFT_TO_RIGHT, values.runDirection)

			  bidi = New java.text.Bidi(chars, 0, embs, 0, chars.Length, bidiflags)
			  If Not bidi.leftToRight Then
				  levels = sun.font.BidiUtils.getLevels(bidi)
				  Dim charsVtoL As Integer() = sun.font.BidiUtils.createVisualToLogicalMap(levels)
				  charsLtoV = sun.font.BidiUtils.createInverseMap(charsVtoL)
				  isDirectionLTR = bidi.baseIsLeftToRight()
			  End If
			End If

			Dim decorator As sun.font.Decoration = sun.font.Decoration.getDecoration(values)

			Dim layoutFlags As Integer = 0 ' no extra info yet, bidi determines run and line direction
			Dim factory As New sun.font.TextLabelFactory(frc, chars, bidi, layoutFlags)

			Dim components_Renamed As sun.font.TextLineComponent() = New sun.font.TextLineComponent(0){}

			components_Renamed = createComponentsOnRun(0, chars.Length, chars, charsLtoV, levels, factory, font_Renamed, lm, frc, decorator, components_Renamed, 0)

			Dim numComponents As Integer = components_Renamed.Length
			Do While components_Renamed(numComponents-1) Is Nothing
				numComponents -= 1
			Loop

			If numComponents <> components_Renamed.Length Then
				Dim temp As sun.font.TextLineComponent() = New sun.font.TextLineComponent(numComponents - 1){}
				Array.Copy(components_Renamed, 0, temp, 0, numComponents)
				components_Renamed = temp
			End If

			Return New TextLine(frc, components_Renamed, lm.baselineOffsets, chars, 0, chars.Length, charsLtoV, levels, isDirectionLTR)
		End Function

		Private Shared Function expandArray(  orig As sun.font.TextLineComponent()) As sun.font.TextLineComponent()

			Dim newComponents As sun.font.TextLineComponent() = New sun.font.TextLineComponent(orig.Length + 8 - 1){}
			Array.Copy(orig, 0, newComponents, 0, orig.Length)

			Return newComponents
		End Function

		''' <summary>
		''' Returns an array in logical order of the TextLineComponents on
		''' the text in the given range, with the given attributes.
		''' </summary>
		Public Shared Function createComponentsOnRun(  runStart As Integer,   runLimit As Integer,   chars As Char(),   charsLtoV As Integer(),   levels As SByte(),   factory As sun.font.TextLabelFactory,   font_Renamed As java.awt.Font,   cm As sun.font.CoreMetrics,   frc As FontRenderContext,   decorator As sun.font.Decoration,   components As sun.font.TextLineComponent(),   numComponents As Integer) As sun.font.TextLineComponent()

			Dim pos As Integer = runStart
			Do
				Dim chunkLimit As Integer = firstVisualChunk(charsLtoV, levels, pos, runLimit) ' <= displayLimit

				Do
					Dim startPos As Integer = pos
					Dim lmCount As Integer

					If cm Is Nothing Then
						Dim lineMetrics As LineMetrics = font_Renamed.getLineMetrics(chars, startPos, chunkLimit, frc)
						cm = sun.font.CoreMetrics.get(lineMetrics)
						lmCount = lineMetrics.numChars
					Else
						lmCount = (chunkLimit-startPos)
					End If

					Dim nextComponent As sun.font.TextLineComponent = factory.createExtended(font_Renamed, cm, decorator, startPos, startPos + lmCount)

					numComponents += 1
					If numComponents >= components.Length Then components = expandArray(components)

					components(numComponents-1) = nextComponent

					pos += lmCount
				Loop While pos < chunkLimit

			Loop While pos < runLimit

			Return components
		End Function

		''' <summary>
		''' Returns an array (in logical order) of the TextLineComponents representing
		''' the text.  The components are both logically and visually contiguous.
		''' </summary>
		Public Shared Function getComponents(  styledParagraph_Renamed As StyledParagraph,   chars As Char(),   textStart As Integer,   textLimit As Integer,   charsLtoV As Integer(),   levels As SByte(),   factory As sun.font.TextLabelFactory) As sun.font.TextLineComponent()

			Dim frc As FontRenderContext = factory.fontRenderContext

			Dim numComponents As Integer = 0
			Dim tempComponents As sun.font.TextLineComponent() = New sun.font.TextLineComponent(0){}

			Dim pos As Integer = textStart
			Do
				Dim runLimit As Integer = System.Math.Min(styledParagraph_Renamed.getRunLimit(pos), textLimit)

				Dim decorator As sun.font.Decoration = styledParagraph_Renamed.getDecorationAt(pos)

				Dim graphicOrFont As Object = styledParagraph_Renamed.getFontOrGraphicAt(pos)

				If TypeOf graphicOrFont Is GraphicAttribute Then
					' AffineTransform baseRot = styledParagraph.getBaselineRotationAt(pos);
					' !!! For now, let's assign runs of text with both fonts and graphic attributes
					' a null rotation (e.g. the baseline rotation goes away when a graphic
					' is applied.
					Dim baseRot As java.awt.geom.AffineTransform = Nothing
					Dim graphicAttribute_Renamed As GraphicAttribute = CType(graphicOrFont, GraphicAttribute)
					Do
						Dim chunkLimit As Integer = firstVisualChunk(charsLtoV, levels, pos, runLimit)

						Dim nextGraphic As New sun.font.GraphicComponent(graphicAttribute_Renamed, decorator, charsLtoV, levels, pos, chunkLimit, baseRot)
						pos = chunkLimit

						numComponents += 1
						If numComponents >= tempComponents.Length Then tempComponents = expandArray(tempComponents)

						tempComponents(numComponents-1) = nextGraphic

					Loop While pos < runLimit
				Else
					Dim font_Renamed As java.awt.Font = CType(graphicOrFont, java.awt.Font)

					tempComponents = createComponentsOnRun(pos, runLimit, chars, charsLtoV, levels, factory, font_Renamed, Nothing, frc, decorator, tempComponents, numComponents)
					pos = runLimit
					numComponents = tempComponents.Length
					Do While tempComponents(numComponents-1) Is Nothing
						numComponents -= 1
					Loop
				End If

			Loop While pos < textLimit

			Dim components_Renamed As sun.font.TextLineComponent()
			If tempComponents.Length = numComponents Then
				components_Renamed = tempComponents
			Else
				components_Renamed = New sun.font.TextLineComponent(numComponents - 1){}
				Array.Copy(tempComponents, 0, components_Renamed, 0, numComponents)
			End If

			Return components_Renamed
		End Function

		''' <summary>
		''' Create a TextLine from the Font and character data over the
		''' range.  The range is relative to both the StyledParagraph and the
		''' character array.
		''' </summary>
		Public Shared Function createLineFromText(  chars As Char(),   styledParagraph_Renamed As StyledParagraph,   factory As sun.font.TextLabelFactory,   isDirectionLTR As Boolean,   baselineOffsets As Single()) As TextLine

			factory.lineContextext(0, chars.Length)

			Dim lineBidi As java.text.Bidi = factory.lineBidi
			Dim charsLtoV As Integer() = Nothing
			Dim levels As SByte() = Nothing

			If lineBidi IsNot Nothing Then
				levels = sun.font.BidiUtils.getLevels(lineBidi)
				Dim charsVtoL As Integer() = sun.font.BidiUtils.createVisualToLogicalMap(levels)
				charsLtoV = sun.font.BidiUtils.createInverseMap(charsVtoL)
			End If

			Dim components_Renamed As sun.font.TextLineComponent() = getComponents(styledParagraph_Renamed, chars, 0, chars.Length, charsLtoV, levels, factory)

			Return New TextLine(factory.fontRenderContext, components_Renamed, baselineOffsets, chars, 0, chars.Length, charsLtoV, levels, isDirectionLTR)
		End Function

		''' <summary>
		''' Compute the components order from the given components array and
		''' logical-to-visual character mapping.  May return null if canonical.
		''' </summary>
		Private Shared Function computeComponentOrder(  components As sun.font.TextLineComponent(),   charsLtoV As Integer()) As Integer()

	'        
	'         * Create a visual ordering for the glyph sets.  The important thing
	'         * here is that the values have the proper rank with respect to
	'         * each other, not the exact values.  For example, the first glyph
	'         * set that appears visually should have the lowest value.  The last
	'         * should have the highest value.  The values are then normalized
	'         * to map 1-1 with positions in glyphs.
	'         *
	'         
			Dim componentOrder As Integer() = Nothing
			If charsLtoV IsNot Nothing AndAlso components.Length > 1 Then
				componentOrder = New Integer(components.Length - 1){}
				Dim gStart As Integer = 0
				For i As Integer = 0 To components.Length - 1
					componentOrder(i) = charsLtoV(gStart)
					gStart += components(i).numCharacters
				Next i

				componentOrder = sun.font.BidiUtils.createContiguousOrder(componentOrder)
				componentOrder = sun.font.BidiUtils.createInverseMap(componentOrder)
			End If
			Return componentOrder
		End Function


		''' <summary>
		''' Create a TextLine from the text.  chars is just the text in the iterator.
		''' </summary>
		Public Shared Function standardCreateTextLine(  frc As FontRenderContext,   text As java.text.AttributedCharacterIterator,   chars As Char(),   baselineOffsets As Single()) As TextLine

			Dim styledParagraph_Renamed As New StyledParagraph(text, chars)
			Dim bidi As New java.text.Bidi(text)
			If bidi.leftToRight Then bidi = Nothing
			Dim layoutFlags As Integer = 0 ' no extra info yet, bidi determines run and line direction
			Dim factory As New sun.font.TextLabelFactory(frc, chars, bidi, layoutFlags)

			Dim isDirectionLTR As Boolean = True
			If bidi IsNot Nothing Then isDirectionLTR = bidi.baseIsLeftToRight()
			Return createLineFromText(chars, styledParagraph_Renamed, factory, isDirectionLTR, baselineOffsets)
		End Function



	'    
	'     * A utility to get a range of text that is both logically and visually
	'     * contiguous.
	'     * If the entire range is ok, return limit, otherwise return the first
	'     * directional change after start.  We could do better than this, but
	'     * it doesn't seem worth it at the moment.
	'    private static int firstVisualChunk(int order[], byte direction[],
	'                                        int start, int limit)
	'    {
	'        if (order != null) {
	'            int min = order[start];
	'            int max = order[start];
	'            int count = limit - start;
	'            for (int i = start + 1; i < limit; i++) {
	'                min = System.Math.min(min, order[i]);
	'                max = System.Math.max(max, order[i]);
	'                if (max - min >= count) {
	'                    if (direction != null) {
	'                        byte baseLevel = direction[start];
	'                        for (int j = start + 1; j < i; j++) {
	'                            if (direction[j] != baseLevel) {
	'                                return j;
	'                            }
	'                        }
	'                    }
	'                    return i;
	'                }
	'            }
	'        }
	'        return limit;
	'    }
	'     

		''' <summary>
		''' When this returns, the ACI's current position will be at the start of the
		''' first run which does NOT contain a GraphicAttribute.  If no such run exists
		''' the ACI's position will be at the end, and this method will return false.
		''' </summary>
		Friend Shared Function advanceToFirstFont(  aci As java.text.AttributedCharacterIterator) As Boolean

			Dim ch As Char = aci.first()
			Do While ch <> java.text.CharacterIterator.DONE

				If aci.getAttribute(TextAttribute.CHAR_REPLACEMENT) Is Nothing Then Return True
				ch = aci.indexdex(aci.runLimit)
			Loop

			Return False
		End Function

		Friend Shared Function getNormalizedOffsets(  baselineOffsets As Single(),   baseline As SByte) As Single()

			If baselineOffsets(baseline) <> 0 Then
				Dim base As Single = baselineOffsets(baseline)
				Dim temp As Single() = New Single(baselineOffsets.Length - 1){}
				For i As Integer = 0 To temp.Length - 1
					temp(i) = baselineOffsets(i) - base
				Next i
				baselineOffsets = temp
			End If
			Return baselineOffsets
		End Function

		Friend Shared Function getFontAtCurrentPos(  aci As java.text.AttributedCharacterIterator) As java.awt.Font

			Dim value As Object = aci.getAttribute(TextAttribute.FONT)
			If value IsNot Nothing Then Return CType(value, java.awt.Font)
			If aci.getAttribute(TextAttribute.FAMILY) IsNot Nothing Then Return java.awt.Font.getFont(aci.attributes)

			Dim ch As Integer = sun.text.CodePointIterator.create(aci).next()
			If ch <> sun.text.CodePointIterator.DONE Then
				Dim resolver As sun.font.FontResolver = sun.font.FontResolver.instance
				Return resolver.getFont(resolver.getFontIndex(ch), aci.attributes)
			End If
			Return Nothing
		End Function

	'  
	'   * The new version requires that chunks be at the same level.
	'   
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		private static int firstVisualChunk(int order() , byte direction(), int start, int limit)
			If order IsNot Nothing AndAlso direction IsNot Nothing Then
			  Dim dir As SByte = direction(start)
			  start += 1
			  Do While start < limit AndAlso direction(start) = dir
				  start += 1
			  Loop
			  Return start
			End If
			Return limit

	'  
	'   * create a new line with characters between charStart and charLimit
	'   * justified using the provided width and ratio.
	'   
		public TextLine getJustifiedLine(Single justificationWidth, Single justifyRatio, Integer justStart, Integer justLimit)

			Dim newComponents As sun.font.TextLineComponent() = New sun.font.TextLineComponent(fComponents.Length - 1){}
			Array.Copy(fComponents, 0, newComponents, 0, fComponents.Length)

			Dim leftHang As Single = 0
			Dim adv As Single = 0
			Dim justifyDelta As Single = 0
			Dim rejustify As Boolean = False
			Do
				adv = getAdvanceBetween(newComponents, 0, characterCount())

				' all characters outside the justification range must be in the base direction
				' of the layout, otherwise justification makes no sense.

				Dim justifyAdvance As Single = getAdvanceBetween(newComponents, justStart, justLimit)

				' get the actual justification delta
				justifyDelta = (justificationWidth - justifyAdvance) * justifyRatio

				' generate an array of GlyphJustificationInfo records to pass to
				' the justifier.  Array is visually ordered.

				' get positions that each component will be using
				Dim infoPositions As Integer() = New Integer(newComponents.Length - 1){}
				Dim infoCount As Integer = 0
				For visIndex As Integer = 0 To newComponents.Length - 1
						Dim logIndex As Integer = getComponentLogicalIndex(visIndex)
					infoPositions(logIndex) = infoCount
					infoCount += newComponents(logIndex).numJustificationInfos
				Next visIndex
				Dim infos As GlyphJustificationInfo() = New GlyphJustificationInfo(infoCount - 1){}

				' get justification infos
				Dim compStart As Integer = 0
				For i As Integer = 0 To newComponents.Length - 1
					Dim comp As sun.font.TextLineComponent = newComponents(i)
					Dim compLength As Integer = comp.numCharacters
					Dim compLimit As Integer = compStart + compLength
					If compLimit > justStart Then
						Dim rangeMin As Integer = System.Math.Max(0, justStart - compStart)
						Dim rangeMax As Integer = System.Math.Min(compLength, justLimit - compStart)
						comp.getJustificationInfos(infos, infoPositions(i), rangeMin, rangeMax)

						If compLimit >= justLimit Then Exit For
					End If
				Next i

				' records are visually ordered, and contiguous, so start and end are
				' simply the places where we didn't fetch records
				Dim infoStart As Integer = 0
				Dim infoLimit As Integer = infoCount
				Do While infoStart < infoLimit AndAlso infos(infoStart) Is Nothing
					infoStart += 1
				Loop

				Do While infoLimit > infoStart AndAlso infos(infoLimit - 1) Is Nothing
					infoLimit -= 1
				Loop

				' invoke justifier on the records
				Dim justifier As New TextJustifier(infos, infoStart, infoLimit)

				Dim deltas As Single() = justifier.justify(justifyDelta)

				Dim canRejustify As Boolean = rejustify = False
				Dim wantRejustify As Boolean = False
				Dim flags As Boolean() = New Boolean(0){}

				' apply justification deltas
				compStart = 0
				For i As Integer = 0 To newComponents.Length - 1
					Dim comp As sun.font.TextLineComponent = newComponents(i)
					Dim compLength As Integer = comp.numCharacters
					Dim compLimit As Integer = compStart + compLength
					If compLimit > justStart Then
						Dim rangeMin As Integer = System.Math.Max(0, justStart - compStart)
						Dim rangeMax As Integer = System.Math.Min(compLength, justLimit - compStart)
						newComponents(i) = comp.applyJustificationDeltas(deltas, infoPositions(i) * 2, flags)

						wantRejustify = wantRejustify Or flags(0)

						If compLimit >= justLimit Then Exit For
					End If
				Next i

				rejustify = wantRejustify AndAlso Not rejustify ' only make two passes
			Loop While rejustify

			Return New TextLine(frc, newComponents, fBaselineOffsets, fChars, fCharsStart, fCharsLimit, fCharLogicalOrder, fCharLevels, fIsDirectionLTR)

		' return the sum of the advances of text between the logical start and limit
		Public Shared Single getAdvanceBetween(sun.font.TextLineComponent() components, Integer start, Integer limit)
			Dim advance As Single = 0

			Dim tlcStart As Integer = 0
			For i As Integer = 0 To components.length - 1
				Dim comp As sun.font.TextLineComponent = components(i)

				Dim tlcLength As Integer = comp.numCharacters
				Dim tlcLimit As Integer = tlcStart + tlcLength
				If tlcLimit > start Then
					Dim measureStart As Integer = System.Math.Max(0, start - tlcStart)
					Dim measureLimit As Integer = System.Math.Min(tlcLength, limit - tlcStart)
					advance += comp.getAdvanceBetween(measureStart, measureLimit)
					If tlcLimit >= limit Then Exit For
				End If

				tlcStart = tlcLimit
			Next i

			Return advance

		sun.font.LayoutPathImpl layoutPath
			Return lp
	End Class

End Namespace
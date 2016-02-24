Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 1997, 1999, Oracle and/or its affiliates. All rights reserved.
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
' * (C) Copyright Taligent, Inc. 1996 - 1997, All Rights Reserved
' * (C) Copyright IBM Corp. 1996 - 1998, All Rights Reserved
' *
' * The original version of this source code and documentation is
' * copyrighted and owned by Taligent, Inc., a wholly-owned subsidiary
' * of IBM. These materials are provided under terms of a License
' * Agreement between Taligent and Sun. This technology is protected
' * by multiple US and International patents.
' *
' * This notice and attribution to Taligent may not be removed.
' * Taligent is a registered trademark of Taligent, Inc.
' *
' 

Namespace java.awt.font

	'
	' * one info for each side of each glyph
	' * separate infos for grow and shrink case
	' * !!! this doesn't really need to be a separate class.  If we keep it
	' * separate, probably the newJustify code from TextLayout belongs here as well.
	' 

	Friend Class TextJustifier
		Private info As GlyphJustificationInfo()
		Private start As Integer
		Private limit As Integer

		Friend Shared DEBUG As Boolean = False

		''' <summary>
		''' Initialize the justifier with an array of infos corresponding to each
		''' glyph. Start and limit indicate the range of the array to examine.
		''' </summary>
		Friend Sub New(ByVal info As GlyphJustificationInfo(), ByVal start As Integer, ByVal limit As Integer)
			Me.info = info
			Me.start = start
			Me.limit = limit

			If DEBUG Then
				Console.WriteLine("start: " & start & ", limit: " & limit)
				For i As Integer = start To limit - 1
					Dim gji As GlyphJustificationInfo = info(i)
					Console.WriteLine("w: " & gji.weight & ", gp: " & gji.growPriority & ", gll: " & gji.growLeftLimit & ", grl: " & gji.growRightLimit)
				Next i
			End If
		End Sub

		Public Const MAX_PRIORITY As Integer = 3

		''' <summary>
		''' Return an array of deltas twice as long as the original info array,
		''' indicating the amount by which each side of each glyph should grow
		''' or shrink.
		''' 
		''' Delta should be positive to expand the line, and negative to compress it.
		''' </summary>
		Public Overridable Function justify(ByVal delta As Single) As Single()
			Dim deltas As Single() = New Single(info.Length * 2 - 1){}

			Dim grow As Boolean = delta > 0

			If DEBUG Then Console.WriteLine("delta: " & delta)

			' make separate passes through glyphs in order of decreasing priority
			' until justifyDelta is zero or we run out of priorities.
			Dim fallbackPriority As Integer = -1
			Dim p As Integer = 0
			Do While delta <> 0
	'            
	'             * special case 'fallback' iteration, set flag and recheck
	'             * highest priority
	'             
				Dim lastPass As Boolean = p > MAX_PRIORITY
				If lastPass Then p = fallbackPriority

				' pass through glyphs, first collecting weights and limits
				Dim weight As Single = 0
				Dim gslimit As Single = 0
				Dim absorbweight As Single = 0
				For i As Integer = start To limit - 1
					Dim gi As GlyphJustificationInfo = info(i)
					If (If(grow, gi.growPriority, gi.shrinkPriority)) = p Then
						If fallbackPriority = -1 Then fallbackPriority = p

						If i <> start Then ' ignore left of first character
							weight += gi.weight
							If grow Then
								gslimit += gi.growLeftLimit
								If gi.growAbsorb Then absorbweight += gi.weight
							Else
								gslimit += gi.shrinkLeftLimit
								If gi.shrinkAbsorb Then absorbweight += gi.weight
							End If
						End If

						If i + 1 <> limit Then ' ignore right of last character
							weight += gi.weight
							If grow Then
								gslimit += gi.growRightLimit
								If gi.growAbsorb Then absorbweight += gi.weight
							Else
								gslimit += gi.shrinkRightLimit
								If gi.shrinkAbsorb Then absorbweight += gi.weight
							End If
						End If
					End If
				Next i

				' did we hit the limit?
				If Not grow Then gslimit = -gslimit ' negative for negative deltas
				Dim hitLimit As Boolean = (weight = 0) OrElse ((Not lastPass) AndAlso ((delta < 0) = (delta < gslimit)))
				Dim absorbing As Boolean = hitLimit AndAlso absorbweight > 0

				' predivide delta by weight
				Dim weightedDelta As Single = delta / weight ' not used if weight == 0

				Dim weightedAbsorb As Single = 0
				If hitLimit AndAlso absorbweight > 0 Then weightedAbsorb = (delta - gslimit) / absorbweight

				If DEBUG Then Console.WriteLine("pass: " & p & ", d: " & delta & ", l: " & gslimit & ", w: " & weight & ", aw: " & absorbweight & ", wd: " & weightedDelta & ", wa: " & weightedAbsorb & ", hit: " & (If(hitLimit, "y", "n")))

				' now allocate this based on ratio of weight to total weight
				Dim n As Integer = start * 2
				For i As Integer = start To limit - 1
					Dim gi As GlyphJustificationInfo = info(i)
					If (If(grow, gi.growPriority, gi.shrinkPriority)) = p Then
						If i <> start Then ' ignore left
							Dim d As Single
							If hitLimit Then
								' factor in sign
								d = If(grow, gi.growLeftLimit, -gi.shrinkLeftLimit)
								If absorbing Then d += gi.weight * weightedAbsorb
							Else
								' sign factored in already
								d = gi.weight * weightedDelta
							End If

							deltas(n) += d
						End If
						n += 1

						If i + 1 <> limit Then ' ignore right
							Dim d As Single
							If hitLimit Then
								d = If(grow, gi.growRightLimit, -gi.shrinkRightLimit)
								If absorbing Then d += gi.weight * weightedAbsorb
							Else
								d = gi.weight * weightedDelta
							End If

							deltas(n) += d
						End If
						n += 1
					Else
						n += 2
					End If
				Next i

				If (Not lastPass) AndAlso hitLimit AndAlso (Not absorbing) Then
					delta -= gslimit
				Else
					delta = 0 ' stop iteration
				End If
				p += 1
			Loop

			If DEBUG Then
				Dim total As Single = 0
				For i As Integer = 0 To deltas.Length - 1
					total += deltas(i)
					Console.Write(deltas(i) & ", ")
					If i Mod 20 = 9 Then Console.WriteLine()
				Next i
				Console.WriteLine(vbLf & "total: " & total)
				Console.WriteLine()
			End If

			Return deltas
		End Function
	End Class

End Namespace
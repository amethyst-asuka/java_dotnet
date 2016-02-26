Imports Microsoft.VisualBasic
Imports System
Imports javax.swing
Imports javax.swing.text
Imports javax.swing.event

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text.html

	''' <summary>
	''' Implements a FrameSetView, intended to support the HTML
	''' &lt;FRAMESET&gt; tag.  Supports the ROWS and COLS attributes.
	''' 
	''' @author  Sunita Mani
	''' 
	'''          Credit also to the hotjava browser engineers that
	'''          worked on making the allocation of space algorithms
	'''          conform to the HTML 4.0 standard and also be netscape
	'''          compatible.
	''' 
	''' </summary>

	Friend Class FrameSetView
		Inherits javax.swing.text.BoxView

		Friend children As String()
		Friend percentChildren As Integer()
		Friend absoluteChildren As Integer()
		Friend relativeChildren As Integer()
		Friend percentTotals As Integer
		Friend absoluteTotals As Integer
		Friend relativeTotals As Integer

		''' <summary>
		''' Constructs a FrameSetView for the given element.
		''' </summary>
		''' <param name="elem"> the element that this view is responsible for </param>
		Public Sub New(ByVal elem As Element, ByVal axis As Integer)
			MyBase.New(elem, axis)
			children = Nothing
		End Sub

		''' <summary>
		''' Parses the ROW or COL attributes and returns
		''' an array of strings that represent the space
		''' distribution.
		''' 
		''' </summary>
		Private Function parseRowColSpec(ByVal key As HTML.Attribute) As String()

			Dim ___attributes As AttributeSet = element.attributes
			Dim spec As String = "*"
			If ___attributes IsNot Nothing Then
				If ___attributes.getAttribute(key) IsNot Nothing Then spec = CStr(___attributes.getAttribute(key))
			End If

			Dim tokenizer As New StringTokenizer(spec, ",")
			Dim nTokens As Integer = tokenizer.countTokens()
			Dim n As Integer = viewCount
			Dim items As String() = New String(Math.Max(nTokens, n) - 1){}
			Dim i As Integer = 0
			Do While i < nTokens
				items(i) = tokenizer.nextToken().Trim()
				' As per the spec, 100% is the same as *
				' hence the mapping.
				'
				If items(i).Equals("100%") Then items(i) = "*"
				i += 1
			Loop
			' extend spec if we have more children than specified
			' in ROWS or COLS attribute
			Do While i < items.Length
				items(i) = "*"
				i += 1
			Loop
			Return items
		End Function


		''' <summary>
		''' Initializes a number of internal state variables
		''' that store information about space allocation
		''' for the frames contained within the frameset.
		''' </summary>
		Private Sub init()
			If axis = View.Y_AXIS Then
				children = parseRowColSpec(HTML.Attribute.ROWS)
			Else
				children = parseRowColSpec(HTML.Attribute.COLS)
			End If
			percentChildren = New Integer(children.Length - 1){}
			relativeChildren = New Integer(children.Length - 1){}
			absoluteChildren = New Integer(children.Length - 1){}

			For i As Integer = 0 To children.Length - 1
				percentChildren(i) = -1
				relativeChildren(i) = -1
				absoluteChildren(i) = -1

				If children(i).EndsWith("*") Then
					If children(i).Length > 1 Then
						relativeChildren(i) = Convert.ToInt32(children(i).Substring(0, children(i).Length-1))
						relativeTotals += relativeChildren(i)
					Else
						relativeChildren(i) = 1
						relativeTotals += 1
					End If
				ElseIf children(i).IndexOf("%"c) <> -1 Then
					percentChildren(i) = parseDigits(children(i))
					percentTotals += percentChildren(i)
				Else
					absoluteChildren(i) = Convert.ToInt32(children(i))
				End If
			Next i
			If percentTotals > 100 Then
				For i As Integer = 0 To percentChildren.Length - 1
					If percentChildren(i) > 0 Then percentChildren(i) = (percentChildren(i) * 100) \ percentTotals
				Next i
				percentTotals = 100
			End If
		End Sub

		''' <summary>
		''' Perform layout for the major axis of the box (i.e. the
		''' axis that it represents).  The results of the layout should
		''' be placed in the given arrays which represent the allocations
		''' to the children along the major axis.
		''' </summary>
		''' <param name="targetSpan"> the total span given to the view, which
		'''  would be used to layout the children </param>
		''' <param name="axis"> the axis being layed out </param>
		''' <param name="offsets"> the offsets from the origin of the view for
		'''  each of the child views; this is a return value and is
		'''  filled in by the implementation of this method </param>
		''' <param name="spans"> the span of each child view; this is a return
		'''  value and is filled in by the implementation of this method </param>
		''' <returns> the offset and span for each child view in the
		'''  offsets and spans parameters </returns>
		Protected Friend Overrides Sub layoutMajorAxis(ByVal targetSpan As Integer, ByVal axis As Integer, ByVal offsets As Integer(), ByVal spans As Integer())
			If children Is Nothing Then init()
			SizeRequirements.calculateTiledPositions(targetSpan, Nothing, getChildRequests(targetSpan, axis), offsets, spans)
		End Sub

		Protected Friend Overridable Function getChildRequests(ByVal targetSpan As Integer, ByVal axis As Integer) As SizeRequirements()

			Dim ___span As Integer() = New Integer(children.Length - 1){}

			spread(targetSpan, ___span)
			Dim n As Integer = viewCount
			Dim reqs As SizeRequirements() = New SizeRequirements(n - 1){}
			Dim i As Integer = 0
			Dim sIndex As Integer = 0
			Do While i < n
				Dim v As View = getView(i)
				If (TypeOf v Is FrameView) OrElse (TypeOf v Is FrameSetView) Then
					reqs(i) = New SizeRequirements(CInt(Fix(v.getMinimumSpan(axis))), ___span(sIndex), CInt(Fix(v.getMaximumSpan(axis))), 0.5f)
					sIndex += 1
				Else
					Dim min As Integer = CInt(Fix(v.getMinimumSpan(axis)))
					Dim pref As Integer = CInt(Fix(v.getPreferredSpan(axis)))
					Dim max As Integer = CInt(Fix(v.getMaximumSpan(axis)))
					Dim a As Single = v.getAlignment(axis)
					reqs(i) = New SizeRequirements(min, pref, max, a)
				End If
				i += 1
			Loop
			Return reqs
		End Function


		''' <summary>
		''' This method is responsible for returning in span[] the
		''' span for each child view along the major axis.  it
		''' computes this based on the information that extracted
		''' from the value of the ROW/COL attribute.
		''' </summary>
		Private Sub spread(ByVal targetSpan As Integer, ByVal span As Integer())

			If targetSpan = 0 Then Return

			Dim tempSpace As Integer = 0
			Dim remainingSpace As Integer = targetSpan

			' allocate the absolute's first, they have
			' precedence
			'
			For i As Integer = 0 To span.Length - 1
				If absoluteChildren(i) > 0 Then
					span(i) = absoluteChildren(i)
					remainingSpace -= span(i)
				End If
			Next i

			' then deal with percents.
			'
			tempSpace = remainingSpace
			For i As Integer = 0 To span.Length - 1
				If percentChildren(i) > 0 AndAlso tempSpace > 0 Then
					span(i) = (percentChildren(i) * tempSpace) \ 100
					remainingSpace -= span(i)
				ElseIf percentChildren(i) > 0 AndAlso tempSpace <= 0 Then
					span(i) = targetSpan \ span.Length
					remainingSpace -= span(i)
				End If
			Next i

			' allocate remainingSpace to relative
			If remainingSpace > 0 AndAlso relativeTotals > 0 Then
				For i As Integer = 0 To span.Length - 1
					If relativeChildren(i) > 0 Then span(i) = (remainingSpace * relativeChildren(i)) \ relativeTotals
				Next i
			ElseIf remainingSpace > 0 Then
				' There are no relative columns and the space has been
				' under- or overallocated.  In this case, turn all the
				' percentage and pixel specified columns to percentage
				' columns based on the ratio of their pixel count to the
				' total "virtual" size. (In the case of percentage columns,
				' the pixel count would equal the specified percentage
				' of the screen size.

				' This action is in accordance with the HTML
				' 4.0 spec (see section 8.3, the end of the discussion of
				' the FRAMESET tag).  The precedence of percentage and pixel
				' specified columns is unclear (spec seems to indicate that
				' they share priority, however, unspecified what happens when
				' overallocation occurs.)

				' addendum is that we behave similar to netscape in that specified
				' widths have precedance over percentage widths...

				Dim vTotal As Single = CSng(targetSpan - remainingSpace)
				Dim tempPercents As Single() = New Single(span.Length - 1){}
				remainingSpace = targetSpan
				For i As Integer = 0 To span.Length - 1
					' ok we know what our total space is, and we know how large each
					' column should be relative to each other... therefore we can use
					' that relative information to deduce their percentages of a whole
					' and then scale them appropriately for the correct size
					tempPercents(i) = (CSng(span(i)) / vTotal) * 100.00f
					span(i) = CInt(Fix((CSng(targetSpan) * tempPercents(i)) / 100.00f))
					remainingSpace -= span(i)
				Next i


				' this is for just in case there is something left over.. if there is we just
				' add it one pixel at a time to the frames in order.. We shouldn't really ever get
				' here and if we do it shouldn't be with more than 1 pixel, maybe two.
				Dim i As Integer = 0
				Do While remainingSpace <> 0
					If remainingSpace < 0 Then
						span(i) -= 1
						i += 1
						remainingSpace += 1
					Else
						span(i) += 1
						i += 1
						remainingSpace -= 1
					End If

					' just in case there are more pixels than frames...should never happen..
					If i = span.Length Then i = 0
				Loop
			End If
		End Sub

	'    
	'     * Users have been known to type things like "%25" and "25 %".  Deal
	'     * with it.
	'     
		Private Function parseDigits(ByVal mixedStr As String) As Integer
			Dim result As Integer = 0
			For i As Integer = 0 To mixedStr.Length - 1
				Dim ch As Char = mixedStr.Chars(i)
				If Char.IsDigit(ch) Then result = (result * 10) + Char.digit(ch, 10)
			Next i
			Return result
		End Function

	End Class

End Namespace
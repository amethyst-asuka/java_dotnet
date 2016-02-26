Imports Microsoft.VisualBasic
Imports System

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
Namespace javax.swing



	''' <summary>
	''' For the convenience of layout managers,
	''' calculates information about the size and position of components.
	''' All size and position calculation methods are class methods
	''' that take arrays of SizeRequirements as arguments.
	''' The SizeRequirements class supports two types of layout:
	''' 
	''' <blockquote>
	''' <dl>
	''' <dt> tiled
	''' <dd> The components are placed end-to-end,
	'''      starting either at coordinate 0 (the leftmost or topmost position)
	'''      or at the coordinate representing the end of the allocated span
	'''      (the rightmost or bottommost position).
	''' 
	''' <dt> aligned
	''' <dd> The components are aligned as specified
	'''      by each component's X or Y alignment value.
	''' </dl>
	''' </blockquote>
	''' 
	''' <p>
	''' 
	''' Each SizeRequirements object contains information
	''' about either the width (and X alignment)
	''' or height (and Y alignment)
	''' of a single component or a group of components:
	''' 
	''' <blockquote>
	''' <dl>
	''' <dt> <code>minimum</code>
	''' <dd> The smallest reasonable width/height of the component
	'''      or component group, in pixels.
	''' 
	''' <dt> <code>preferred</code>
	''' <dd> The natural width/height of the component
	'''      or component group, in pixels.
	''' 
	''' <dt> <code>maximum</code>
	''' <dd> The largest reasonable width/height of the component
	'''      or component group, in pixels.
	''' 
	''' <dt> <code>alignment</code>
	''' <dd> The X/Y alignment of the component
	'''      or component group.
	''' </dl>
	''' </blockquote>
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' </summary>
	''' <seealso cref= Component#getMinimumSize </seealso>
	''' <seealso cref= Component#getPreferredSize </seealso>
	''' <seealso cref= Component#getMaximumSize </seealso>
	''' <seealso cref= Component#getAlignmentX </seealso>
	''' <seealso cref= Component#getAlignmentY
	''' 
	''' @author Timothy Prinzing </seealso>
	<Serializable> _
	Public Class SizeRequirements

		''' <summary>
		''' The minimum size required.
		''' For a component <code>comp</code>, this should be equal to either
		''' <code>comp.getMinimumSize().width</code> or
		''' <code>comp.getMinimumSize().height</code>.
		''' </summary>
		Public minimum As Integer

		''' <summary>
		''' The preferred (natural) size.
		''' For a component <code>comp</code>, this should be equal to either
		''' <code>comp.getPreferredSize().width</code> or
		''' <code>comp.getPreferredSize().height</code>.
		''' </summary>
		Public preferred As Integer

		''' <summary>
		''' The maximum size allowed.
		''' For a component <code>comp</code>, this should be equal to either
		''' <code>comp.getMaximumSize().width</code> or
		''' <code>comp.getMaximumSize().height</code>.
		''' </summary>
		Public maximum As Integer

		''' <summary>
		''' The alignment, specified as a value between 0.0 and 1.0,
		''' inclusive.
		''' To specify centering, the alignment should be 0.5.
		''' </summary>
		Public alignment As Single

		''' <summary>
		''' Creates a SizeRequirements object with the minimum, preferred,
		''' and maximum sizes set to zero and an alignment value of 0.5
		''' (centered).
		''' </summary>
		Public Sub New()
			minimum = 0
			preferred = 0
			maximum = 0
			alignment = 0.5f
		End Sub

		''' <summary>
		''' Creates a SizeRequirements object with the specified minimum, preferred,
		''' and maximum sizes and the specified alignment.
		''' </summary>
		''' <param name="min"> the minimum size &gt;= 0 </param>
		''' <param name="pref"> the preferred size &gt;= 0 </param>
		''' <param name="max"> the maximum size &gt;= 0 </param>
		''' <param name="a"> the alignment &gt;= 0.0f &amp;&amp; &lt;= 1.0f </param>
		Public Sub New(ByVal min As Integer, ByVal pref As Integer, ByVal max As Integer, ByVal a As Single)
			minimum = min
			preferred = pref
			maximum = max
			alignment = If(a > 1.0f, 1.0f, If(a < 0.0f, 0.0f, a))
		End Sub

		''' <summary>
		''' Returns a string describing the minimum, preferred, and maximum
		''' size requirements, along with the alignment.
		''' </summary>
		''' <returns> the string </returns>
		Public Overrides Function ToString() As String
			Return "[" & minimum & "," & preferred & "," & maximum & "]@" & alignment
		End Function

		''' <summary>
		''' Determines the total space necessary to
		''' place a set of components end-to-end.  The needs
		''' of each component in the set are represented by an entry in the
		''' passed-in SizeRequirements array.
		''' The returned SizeRequirements object has an alignment of 0.5
		''' (centered).  The space requirement is never more than
		''' Integer.MAX_VALUE.
		''' </summary>
		''' <param name="children">  the space requirements for a set of components.
		'''   The vector may be of zero length, which will result in a
		'''   default SizeRequirements object instance being passed back. </param>
		''' <returns>  the total space requirements. </returns>
		Public Shared Function getTiledSizeRequirements(ByVal children As SizeRequirements()) As SizeRequirements
			Dim total As New SizeRequirements
			For i As Integer = 0 To children.Length - 1
				Dim req As SizeRequirements = children(i)
				total.minimum = CInt(Fix(Math.Min(CLng(total.minimum) + CLng(req.minimum), Integer.MaxValue)))
				total.preferred = CInt(Fix(Math.Min(CLng(total.preferred) + CLng(req.preferred), Integer.MaxValue)))
				total.maximum = CInt(Fix(Math.Min(CLng(total.maximum) + CLng(req.maximum), Integer.MaxValue)))
			Next i
			Return total
		End Function

		''' <summary>
		''' Determines the total space necessary to
		''' align a set of components.  The needs
		''' of each component in the set are represented by an entry in the
		''' passed-in SizeRequirements array.  The total space required will
		''' never be more than Integer.MAX_VALUE.
		''' </summary>
		''' <param name="children">  the set of child requirements.  If of zero length,
		'''  the returns result will be a default instance of SizeRequirements. </param>
		''' <returns>  the total space requirements. </returns>
		Public Shared Function getAlignedSizeRequirements(ByVal children As SizeRequirements()) As SizeRequirements
			Dim totalAscent As New SizeRequirements
			Dim totalDescent As New SizeRequirements
			For i As Integer = 0 To children.Length - 1
				Dim req As SizeRequirements = children(i)

				Dim ascent As Integer = CInt(Fix(req.alignment * req.minimum))
				Dim descent As Integer = req.minimum - ascent
				totalAscent.minimum = Math.Max(ascent, totalAscent.minimum)
				totalDescent.minimum = Math.Max(descent, totalDescent.minimum)

				ascent = CInt(Fix(req.alignment * req.preferred))
				descent = req.preferred - ascent
				totalAscent.preferred = Math.Max(ascent, totalAscent.preferred)
				totalDescent.preferred = Math.Max(descent, totalDescent.preferred)

				ascent = CInt(Fix(req.alignment * req.maximum))
				descent = req.maximum - ascent
				totalAscent.maximum = Math.Max(ascent, totalAscent.maximum)
				totalDescent.maximum = Math.Max(descent, totalDescent.maximum)
			Next i
			Dim min As Integer = CInt(Fix(Math.Min(CLng(totalAscent.minimum) + CLng(totalDescent.minimum), Integer.MaxValue)))
			Dim pref As Integer = CInt(Fix(Math.Min(CLng(totalAscent.preferred) + CLng(totalDescent.preferred), Integer.MaxValue)))
			Dim max As Integer = CInt(Fix(Math.Min(CLng(totalAscent.maximum) + CLng(totalDescent.maximum), Integer.MaxValue)))
			Dim alignment As Single = 0.0f
			If min > 0 Then
				alignment = CSng(totalAscent.minimum) / min
				alignment = If(alignment > 1.0f, 1.0f, If(alignment < 0.0f, 0.0f, alignment))
			End If
			Return New SizeRequirements(min, pref, max, alignment)
		End Function

		''' <summary>
		''' Creates a set of offset/span pairs representing how to
		''' lay out a set of components end-to-end.
		''' This method requires that you specify
		''' the total amount of space to be allocated,
		''' the size requirements for each component to be placed
		''' (specified as an array of SizeRequirements), and
		''' the total size requirement of the set of components.
		''' You can get the total size requirement
		''' by invoking the getTiledSizeRequirements method.  The components
		''' will be tiled in the forward direction with offsets increasing from 0.
		''' </summary>
		''' <param name="allocated"> the total span to be allocated &gt;= 0. </param>
		''' <param name="total">     the total of the children requests.  This argument
		'''  is optional and may be null. </param>
		''' <param name="children">  the size requirements for each component. </param>
		''' <param name="offsets">   the offset from 0 for each child where
		'''   the spans were allocated (determines placement of the span). </param>
		''' <param name="spans">     the span allocated for each child to make the
		'''   total target span. </param>
		Public Shared Sub calculateTiledPositions(ByVal allocated As Integer, ByVal total As SizeRequirements, ByVal children As SizeRequirements(), ByVal offsets As Integer(), ByVal spans As Integer())
			calculateTiledPositions(allocated, total, children, offsets, spans, True)
		End Sub

		''' <summary>
		''' Creates a set of offset/span pairs representing how to
		''' lay out a set of components end-to-end.
		''' This method requires that you specify
		''' the total amount of space to be allocated,
		''' the size requirements for each component to be placed
		''' (specified as an array of SizeRequirements), and
		''' the total size requirement of the set of components.
		''' You can get the total size requirement
		''' by invoking the getTiledSizeRequirements method.
		''' 
		''' This method also requires a flag indicating whether components
		''' should be tiled in the forward direction (offsets increasing
		''' from 0) or reverse direction (offsets decreasing from the end
		''' of the allocated space).  The forward direction represents
		''' components tiled from left to right or top to bottom.  The
		''' reverse direction represents components tiled from right to left
		''' or bottom to top.
		''' </summary>
		''' <param name="allocated"> the total span to be allocated &gt;= 0. </param>
		''' <param name="total">     the total of the children requests.  This argument
		'''  is optional and may be null. </param>
		''' <param name="children">  the size requirements for each component. </param>
		''' <param name="offsets">   the offset from 0 for each child where
		'''   the spans were allocated (determines placement of the span). </param>
		''' <param name="spans">     the span allocated for each child to make the
		'''   total target span. </param>
		''' <param name="forward">   tile with offsets increasing from 0 if true
		'''   and with offsets decreasing from the end of the allocated space
		'''   if false.
		''' @since 1.4 </param>
		Public Shared Sub calculateTiledPositions(ByVal allocated As Integer, ByVal total As SizeRequirements, ByVal children As SizeRequirements(), ByVal offsets As Integer(), ByVal spans As Integer(), ByVal forward As Boolean)
			' The total argument turns out to be a bad idea since the
			' total of all the children can overflow the integer used to
			' hold the total.  The total must therefore be calculated and
			' stored in long variables.
			Dim min As Long = 0
			Dim pref As Long = 0
			Dim max As Long = 0
			For i As Integer = 0 To children.Length - 1
				min += children(i).minimum
				pref += children(i).preferred
				max += children(i).maximum
			Next i
			If allocated >= pref Then
				expandedTile(allocated, min, pref, max, children, offsets, spans, forward)
			Else
				compressedTile(allocated, min, pref, max, children, offsets, spans, forward)
			End If
		End Sub

		Private Shared Sub compressedTile(ByVal allocated As Integer, ByVal min As Long, ByVal pref As Long, ByVal max As Long, ByVal request As SizeRequirements(), ByVal offsets As Integer(), ByVal spans As Integer(), ByVal forward As Boolean)

			' ---- determine what we have to work with ----
			Dim totalPlay As Single = Math.Min(pref - allocated, pref - min)
			Dim factor As Single = If(pref - min = 0, 0.0f, totalPlay / (pref - min))

			' ---- make the adjustments ----
			Dim totalOffset As Integer
			If forward Then
				' lay out with offsets increasing from 0
				totalOffset = 0
				For i As Integer = 0 To spans.Length - 1
					offsets(i) = totalOffset
					Dim req As SizeRequirements = request(i)
					Dim play As Single = factor * (req.preferred - req.minimum)
					spans(i) = CInt(Fix(req.preferred - play))
					totalOffset = CInt(Fix(Math.Min(CLng(totalOffset) + CLng(spans(i)), Integer.MaxValue)))
				Next i
			Else
				' lay out with offsets decreasing from the end of the allocation
				totalOffset = allocated
				For i As Integer = 0 To spans.Length - 1
					Dim req As SizeRequirements = request(i)
					Dim play As Single = factor * (req.preferred - req.minimum)
					spans(i) = CInt(Fix(req.preferred - play))
					offsets(i) = totalOffset - spans(i)
					totalOffset = CInt(Fix(Math.Max(CLng(totalOffset) - CLng(spans(i)), 0)))
				Next i
			End If
		End Sub

		Private Shared Sub expandedTile(ByVal allocated As Integer, ByVal min As Long, ByVal pref As Long, ByVal max As Long, ByVal request As SizeRequirements(), ByVal offsets As Integer(), ByVal spans As Integer(), ByVal forward As Boolean)

			' ---- determine what we have to work with ----
			Dim totalPlay As Single = Math.Min(allocated - pref, max - pref)
			Dim factor As Single = If(max - pref = 0, 0.0f, totalPlay / (max - pref))

			' ---- make the adjustments ----
			Dim totalOffset As Integer
			If forward Then
				' lay out with offsets increasing from 0
				totalOffset = 0
				For i As Integer = 0 To spans.Length - 1
					offsets(i) = totalOffset
					Dim req As SizeRequirements = request(i)
					Dim play As Integer = CInt(Fix(factor * (req.maximum - req.preferred)))
					spans(i) = CInt(Fix(Math.Min(CLng(req.preferred) + CLng(play), Integer.MaxValue)))
					totalOffset = CInt(Fix(Math.Min(CLng(totalOffset) + CLng(spans(i)), Integer.MaxValue)))
				Next i
			Else
				' lay out with offsets decreasing from the end of the allocation
				totalOffset = allocated
				For i As Integer = 0 To spans.Length - 1
					Dim req As SizeRequirements = request(i)
					Dim play As Integer = CInt(Fix(factor * (req.maximum - req.preferred)))
					spans(i) = CInt(Fix(Math.Min(CLng(req.preferred) + CLng(play), Integer.MaxValue)))
					offsets(i) = totalOffset - spans(i)
					totalOffset = CInt(Fix(Math.Max(CLng(totalOffset) - CLng(spans(i)), 0)))
				Next i
			End If
		End Sub

		''' <summary>
		''' Creates a bunch of offset/span pairs specifying how to
		''' lay out a set of components with the specified alignments.
		''' The resulting span allocations will overlap, with each one
		''' fitting as well as possible into the given total allocation.
		''' This method requires that you specify
		''' the total amount of space to be allocated,
		''' the size requirements for each component to be placed
		''' (specified as an array of SizeRequirements), and
		''' the total size requirements of the set of components
		''' (only the alignment field of which is actually used).
		''' You can get the total size requirement by invoking
		''' getAlignedSizeRequirements.
		''' 
		''' Normal alignment will be done with an alignment value of 0.0f
		''' representing the left/top edge of a component.
		''' </summary>
		''' <param name="allocated"> the total span to be allocated &gt;= 0. </param>
		''' <param name="total">     the total of the children requests. </param>
		''' <param name="children">  the size requirements for each component. </param>
		''' <param name="offsets">   the offset from 0 for each child where
		'''   the spans were allocated (determines placement of the span). </param>
		''' <param name="spans">     the span allocated for each child to make the
		'''   total target span. </param>
		Public Shared Sub calculateAlignedPositions(ByVal allocated As Integer, ByVal total As SizeRequirements, ByVal children As SizeRequirements(), ByVal offsets As Integer(), ByVal spans As Integer())
			calculateAlignedPositions(allocated, total, children, offsets, spans, True)
		End Sub

		''' <summary>
		''' Creates a set of offset/span pairs specifying how to
		''' lay out a set of components with the specified alignments.
		''' The resulting span allocations will overlap, with each one
		''' fitting as well as possible into the given total allocation.
		''' This method requires that you specify
		''' the total amount of space to be allocated,
		''' the size requirements for each component to be placed
		''' (specified as an array of SizeRequirements), and
		''' the total size requirements of the set of components
		''' (only the alignment field of which is actually used)
		''' You can get the total size requirement by invoking
		''' getAlignedSizeRequirements.
		''' 
		''' This method also requires a flag indicating whether normal or
		''' reverse alignment should be performed.  With normal alignment
		''' the value 0.0f represents the left/top edge of the component
		''' to be aligned.  With reverse alignment, 0.0f represents the
		''' right/bottom edge.
		''' </summary>
		''' <param name="allocated"> the total span to be allocated &gt;= 0. </param>
		''' <param name="total">     the total of the children requests. </param>
		''' <param name="children">  the size requirements for each component. </param>
		''' <param name="offsets">   the offset from 0 for each child where
		'''   the spans were allocated (determines placement of the span). </param>
		''' <param name="spans">     the span allocated for each child to make the
		'''   total target span. </param>
		''' <param name="normal">    when true, the alignment value 0.0f means
		'''   left/top; when false, it means right/bottom.
		''' @since 1.4 </param>
		Public Shared Sub calculateAlignedPositions(ByVal allocated As Integer, ByVal total As SizeRequirements, ByVal children As SizeRequirements(), ByVal offsets As Integer(), ByVal spans As Integer(), ByVal normal As Boolean)
			Dim totalAlignment As Single = If(normal, total.alignment, 1.0f - total.alignment)
			Dim totalAscent As Integer = CInt(Fix(allocated * totalAlignment))
			Dim totalDescent As Integer = allocated - totalAscent
			For i As Integer = 0 To children.Length - 1
				Dim req As SizeRequirements = children(i)
				Dim alignment As Single = If(normal, req.alignment, 1.0f - req.alignment)
				Dim maxAscent As Integer = CInt(Fix(req.maximum * alignment))
				Dim maxDescent As Integer = req.maximum - maxAscent
				Dim ascent As Integer = Math.Min(totalAscent, maxAscent)
				Dim descent As Integer = Math.Min(totalDescent, maxDescent)

				offsets(i) = totalAscent - ascent
				spans(i) = CInt(Fix(Math.Min(CLng(ascent) + CLng(descent), Integer.MaxValue)))
			Next i
		End Sub

		' This method was used by the JTable - which now uses a different technique.
		''' <summary>
		''' Adjust a specified array of sizes by a given amount.
		''' </summary>
		''' <param name="delta">     an int specifying the size difference </param>
		''' <param name="children">  an array of SizeRequirements objects </param>
		''' <returns> an array of ints containing the final size for each item </returns>
		Public Shared Function adjustSizes(ByVal delta As Integer, ByVal children As SizeRequirements()) As Integer()
		  Return New Integer(){}
		End Function
	End Class

End Namespace
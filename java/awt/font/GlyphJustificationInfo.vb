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
' 

Namespace java.awt.font

	''' <summary>
	''' The <code>GlyphJustificationInfo</code> class represents information
	''' about the justification properties of a glyph.  A glyph is the visual
	''' representation of one or more characters.  Many different glyphs can
	''' be used to represent a single character or combination of characters.
	''' The four justification properties represented by
	''' <code>GlyphJustificationInfo</code> are weight, priority, absorb and
	''' limit.
	''' <p>
	''' Weight is the overall 'weight' of the glyph in the line.  Generally it is
	''' proportional to the size of the font.  Glyphs with larger weight are
	''' allocated a correspondingly larger amount of the change in space.
	''' <p>
	''' Priority determines the justification phase in which this glyph is used.
	''' All glyphs of the same priority are examined before glyphs of the next
	''' priority.  If all the change in space can be allocated to these glyphs
	''' without exceeding their limits, then glyphs of the next priority are not
	''' examined. There are four priorities, kashida, whitespace, interchar,
	''' and none.  KASHIDA is the first priority examined. NONE is the last
	''' priority examined.
	''' <p>
	''' Absorb determines whether a glyph absorbs all change in space.  Within a
	''' given priority, some glyphs may absorb all the change in space.  If any of
	''' these glyphs are present, no glyphs of later priority are examined.
	''' <p>
	''' Limit determines the maximum or minimum amount by which the glyph can
	''' change. Left and right sides of the glyph can have different limits.
	''' <p>
	''' Each <code>GlyphJustificationInfo</code> represents two sets of
	''' metrics, which are <i>growing</i> and <i>shrinking</i>.  Growing
	''' metrics are used when the glyphs on a line are to be
	''' spread apart to fit a larger width.  Shrinking metrics are used when
	''' the glyphs are to be moved together to fit a smaller width.
	''' </summary>

	Public NotInheritable Class GlyphJustificationInfo

		''' <summary>
		''' Constructs information about the justification properties of a
		''' glyph. </summary>
		''' <param name="weight"> the weight of this glyph when allocating space.  Must be non-negative. </param>
		''' <param name="growAbsorb"> if <code>true</code> this glyph absorbs
		''' all extra space at this priority and lower priority levels when it
		''' grows </param>
		''' <param name="growPriority"> the priority level of this glyph when it
		''' grows </param>
		''' <param name="growLeftLimit"> the maximum amount by which the left side of this
		''' glyph can grow.  Must be non-negative. </param>
		''' <param name="growRightLimit"> the maximum amount by which the right side of this
		''' glyph can grow.  Must be non-negative. </param>
		''' <param name="shrinkAbsorb"> if <code>true</code>, this glyph absorbs all
		''' remaining shrinkage at this and lower priority levels when it
		''' shrinks </param>
		''' <param name="shrinkPriority"> the priority level of this glyph when
		''' it shrinks </param>
		''' <param name="shrinkLeftLimit"> the maximum amount by which the left side of this
		''' glyph can shrink.  Must be non-negative. </param>
		''' <param name="shrinkRightLimit"> the maximum amount by which the right side
		''' of this glyph can shrink.  Must be non-negative. </param>
		 Public Sub New(  weight As Single,   growAbsorb As Boolean,   growPriority As Integer,   growLeftLimit As Single,   growRightLimit As Single,   shrinkAbsorb As Boolean,   shrinkPriority As Integer,   shrinkLeftLimit As Single,   shrinkRightLimit As Single)
			If weight < 0 Then Throw New IllegalArgumentException("weight is negative")

			If Not priorityIsValid(growPriority) Then Throw New IllegalArgumentException("Invalid grow priority")
			If growLeftLimit < 0 Then Throw New IllegalArgumentException("growLeftLimit is negative")
			If growRightLimit < 0 Then Throw New IllegalArgumentException("growRightLimit is negative")

			If Not priorityIsValid(shrinkPriority) Then Throw New IllegalArgumentException("Invalid shrink priority")
			If shrinkLeftLimit < 0 Then Throw New IllegalArgumentException("shrinkLeftLimit is negative")
			If shrinkRightLimit < 0 Then Throw New IllegalArgumentException("shrinkRightLimit is negative")

			Me.weight = weight
			Me.growAbsorb = growAbsorb
			Me.growPriority = growPriority
			Me.growLeftLimit = growLeftLimit
			Me.growRightLimit = growRightLimit
			Me.shrinkAbsorb = shrinkAbsorb
			Me.shrinkPriority = shrinkPriority
			Me.shrinkLeftLimit = shrinkLeftLimit
			Me.shrinkRightLimit = shrinkRightLimit
		 End Sub

		Private Shared Function priorityIsValid(  priority As Integer) As Boolean

			Return priority >= PRIORITY_KASHIDA AndAlso priority <= PRIORITY_NONE
		End Function

		''' <summary>
		''' The highest justification priority. </summary>
		Public Const PRIORITY_KASHIDA As Integer = 0

		''' <summary>
		''' The second highest justification priority. </summary>
		Public Const PRIORITY_WHITESPACE As Integer = 1

		''' <summary>
		''' The second lowest justification priority. </summary>
		Public Const PRIORITY_INTERCHAR As Integer = 2

		''' <summary>
		''' The lowest justification priority. </summary>
		Public Const PRIORITY_NONE As Integer = 3

		''' <summary>
		''' The weight of this glyph.
		''' </summary>
		Public ReadOnly weight As Single

		''' <summary>
		''' The priority level of this glyph as it is growing.
		''' </summary>
		Public ReadOnly growPriority As Integer

		''' <summary>
		''' If <code>true</code>, this glyph absorbs all extra
		''' space at this and lower priority levels when it grows.
		''' </summary>
		Public ReadOnly growAbsorb As Boolean

		''' <summary>
		''' The maximum amount by which the left side of this glyph can grow.
		''' </summary>
		Public ReadOnly growLeftLimit As Single

		''' <summary>
		''' The maximum amount by which the right side of this glyph can grow.
		''' </summary>
		Public ReadOnly growRightLimit As Single

		''' <summary>
		''' The priority level of this glyph as it is shrinking.
		''' </summary>
		Public ReadOnly shrinkPriority As Integer

		''' <summary>
		''' If <code>true</code>,this glyph absorbs all remaining shrinkage at
		''' this and lower priority levels as it shrinks.
		''' </summary>
		Public ReadOnly shrinkAbsorb As Boolean

		''' <summary>
		''' The maximum amount by which the left side of this glyph can shrink
		''' (a positive number).
		''' </summary>
		Public ReadOnly shrinkLeftLimit As Single

		''' <summary>
		''' The maximum amount by which the right side of this glyph can shrink
		''' (a positive number).
		''' </summary>
		Public ReadOnly shrinkRightLimit As Single
	End Class

End Namespace
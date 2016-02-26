Imports System

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
Namespace javax.swing.text


	''' <summary>
	''' An implementation of a gapped buffer similar to that used by
	''' emacs.  The underlying storage is a java array of some type,
	''' which is known only by the subclass of this class.  The array
	''' has a gap somewhere.  The gap is moved to the location of changes
	''' to take advantage of common behavior where most changes occur
	''' in the same location.  Changes that occur at a gap boundary are
	''' generally cheap and moving the gap is generally cheaper than
	''' moving the array contents directly to accommodate the change.
	''' 
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref= GapContent </seealso>
	<Serializable> _
	Friend MustInherit Class GapVector


		''' <summary>
		''' Creates a new GapVector object.  Initial size defaults to 10.
		''' </summary>
		Public Sub New()
			Me.New(10)
		End Sub

		''' <summary>
		''' Creates a new GapVector object, with the initial
		''' size specified.
		''' </summary>
		''' <param name="initialLength"> the initial size </param>
		Public Sub New(ByVal initialLength As Integer)
			array = allocateArray(initialLength)
			g0 = 0
			g1 = initialLength
		End Sub

		''' <summary>
		''' Allocate an array to store items of the type
		''' appropriate (which is determined by the subclass).
		''' </summary>
		Protected Friend MustOverride Function allocateArray(ByVal len As Integer) As Object

		''' <summary>
		''' Get the length of the allocated array
		''' </summary>
		Protected Friend MustOverride ReadOnly Property arrayLength As Integer

		''' <summary>
		''' Access to the array.  The actual type
		''' of the array is known only by the subclass.
		''' </summary>
		Protected Friend Property array As Object
			Get
				Return array
			End Get
		End Property

		''' <summary>
		''' Access to the start of the gap.
		''' </summary>
		Protected Friend Property gapStart As Integer
			Get
				Return g0
			End Get
		End Property

		''' <summary>
		''' Access to the end of the gap.
		''' </summary>
		Protected Friend Property gapEnd As Integer
			Get
				Return g1
			End Get
		End Property

		' ---- variables -----------------------------------

		''' <summary>
		''' The array of items.  The type is determined by the subclass.
		''' </summary>
		Private array As Object

		''' <summary>
		''' start of gap in the array
		''' </summary>
		Private g0 As Integer

		''' <summary>
		''' end of gap in the array
		''' </summary>
		Private g1 As Integer


		' --- gap management -------------------------------

		''' <summary>
		''' Replace the given logical position in the storage with
		''' the given new items.  This will move the gap to the area
		''' being changed if the gap is not currently located at the
		''' change location.
		''' </summary>
		''' <param name="position"> the location to make the replacement.  This
		'''  is not the location in the underlying storage array, but
		'''  the location in the contiguous space being modeled. </param>
		''' <param name="rmSize"> the number of items to remove </param>
		''' <param name="addItems"> the new items to place in storage. </param>
		Protected Friend Overridable Sub replace(ByVal position As Integer, ByVal rmSize As Integer, ByVal addItems As Object, ByVal addSize As Integer)
			Dim addOffset As Integer = 0
			If addSize = 0 Then
				close(position, rmSize)
				Return
			ElseIf rmSize > addSize Then
				' Shrink the end. 
				close(position+addSize, rmSize-addSize)
			Else
				' Grow the end, do two chunks. 
				Dim endSize As Integer = addSize - rmSize
				Dim [end] As Integer = open(position + rmSize, endSize)
				Array.Copy(addItems, rmSize, array, [end], endSize)
				addSize = rmSize
			End If
			Array.Copy(addItems, addOffset, array, position, addSize)
		End Sub

		''' <summary>
		''' Delete nItems at position.  Squeezes any marks
		''' within the deleted area to position.  This moves
		''' the gap to the best place by minimizing it's
		''' overall movement.  The gap must intersect the
		''' target block.
		''' </summary>
		Friend Overridable Sub close(ByVal position As Integer, ByVal nItems As Integer)
			If nItems = 0 Then Return

			Dim [end] As Integer = position + nItems
			Dim new_gs As Integer = (g1 - g0) + nItems
			If [end] <= g0 Then
				' Move gap to end of block.
				If g0 <> [end] Then shiftGap([end])
				' Adjust g0.
				shiftGapStartDown(g0 - nItems)
			ElseIf position >= g0 Then
				' Move gap to beginning of block.
				If g0 <> position Then shiftGap(position)
				' Adjust g1.
				shiftGapEndUp(g0 + new_gs)
			Else
				' The gap is properly inside the target block.
				' No data movement necessary, simply move both gap pointers.
				shiftGapStartDown(position)
				shiftGapEndUp(g0 + new_gs)
			End If
		End Sub

		''' <summary>
		''' Make space for the given number of items at the given
		''' location.
		''' </summary>
		''' <returns> the location that the caller should fill in </returns>
		Friend Overridable Function open(ByVal position As Integer, ByVal nItems As Integer) As Integer
			Dim gapSize As Integer = g1 - g0
			If nItems = 0 Then
				If position > g0 Then position += gapSize
				Return position
			End If

			' Expand the array if the gap is too small.
			shiftGap(position)
			If nItems >= gapSize Then
				' Pre-shift the gap, to reduce total movement.
				shiftEnd(arrayLength - gapSize + nItems)
				gapSize = g1 - g0
			End If

			g0 = g0 + nItems
			Return position
		End Function

		''' <summary>
		''' resize the underlying storage array to the
		''' given new size
		''' </summary>
		Friend Overridable Sub resize(ByVal nsize As Integer)
			Dim narray As Object = allocateArray(nsize)
			Array.Copy(array, 0, narray, 0, Math.Min(nsize, arrayLength))
			array = narray
		End Sub

		''' <summary>
		''' Make the gap bigger, moving any necessary data and updating
		''' the appropriate marks
		''' </summary>
		Protected Friend Overridable Sub shiftEnd(ByVal newSize As Integer)
			Dim oldSize As Integer = arrayLength
			Dim oldGapEnd As Integer = g1
			Dim upperSize As Integer = oldSize - oldGapEnd
			Dim ___arrayLength As Integer = getNewArraySize(newSize)
			Dim newGapEnd As Integer = ___arrayLength - upperSize
			resize(___arrayLength)
			g1 = newGapEnd

			If upperSize <> 0 Then Array.Copy(array, oldGapEnd, array, newGapEnd, upperSize)
		End Sub

		''' <summary>
		''' Calculates a new size of the storage array depending on required
		''' capacity. </summary>
		''' <param name="reqSize"> the size which is necessary for new content </param>
		''' <returns> the new size of the storage array </returns>
		Friend Overridable Function getNewArraySize(ByVal reqSize As Integer) As Integer
			Return (reqSize + 1) * 2
		End Function

		''' <summary>
		''' Move the start of the gap to a new location,
		''' without changing the size of the gap.  This
		''' moves the data in the array and updates the
		''' marks accordingly.
		''' </summary>
		Protected Friend Overridable Sub shiftGap(ByVal newGapStart As Integer)
			If newGapStart = g0 Then Return
			Dim oldGapStart As Integer = g0
			Dim dg As Integer = newGapStart - oldGapStart
			Dim oldGapEnd As Integer = g1
			Dim newGapEnd As Integer = oldGapEnd + dg
			Dim gapSize As Integer = oldGapEnd - oldGapStart

			g0 = newGapStart
			g1 = newGapEnd
			If dg > 0 Then
				' Move gap up, move data down.
				Array.Copy(array, oldGapEnd, array, oldGapStart, dg)
			ElseIf dg < 0 Then
				' Move gap down, move data up.
				Array.Copy(array, newGapStart, array, newGapEnd, -dg)
			End If
		End Sub

		''' <summary>
		''' Adjust the gap end downward.  This doesn't move
		''' any data, but it does update any marks affected
		''' by the boundary change.  All marks from the old
		''' gap start down to the new gap start are squeezed
		''' to the end of the gap (their location has been
		''' removed).
		''' </summary>
		Protected Friend Overridable Sub shiftGapStartDown(ByVal newGapStart As Integer)
			g0 = newGapStart
		End Sub

		''' <summary>
		''' Adjust the gap end upward.  This doesn't move
		''' any data, but it does update any marks affected
		''' by the boundary change. All marks from the old
		''' gap end up to the new gap end are squeezed
		''' to the end of the gap (their location has been
		''' removed).
		''' </summary>
		Protected Friend Overridable Sub shiftGapEndUp(ByVal newGapEnd As Integer)
			g1 = newGapEnd
		End Sub

	End Class

End Namespace
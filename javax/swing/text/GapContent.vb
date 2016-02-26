Imports Microsoft.VisualBasic
Imports System
Imports System.Collections

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
	''' An implementation of the AbstractDocument.Content interface
	''' implemented using a gapped buffer similar to that used by emacs.
	''' The underlying storage is a array of unicode characters with
	''' a gap somewhere.  The gap is moved to the location of changes
	''' to take advantage of common behavior where most changes are
	''' in the same location.  Changes that occur at a gap boundary are
	''' generally cheap and moving the gap is generally cheaper than
	''' moving the array contents directly to accommodate the change.
	''' <p>
	''' The positions tracking change are also generally cheap to
	''' maintain.  The Position implementations (marks) store the array
	''' index and can easily calculate the sequential position from
	''' the current gap location.  Changes only require update to the
	''' the marks between the old and new gap boundaries when the gap
	''' is moved, so generally updating the marks is pretty cheap.
	''' The marks are stored sorted so they can be located quickly
	''' with a binary search.  This increases the cost of adding a
	''' mark, and decreases the cost of keeping the mark updated.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	<Serializable> _
	Public Class GapContent
		Inherits GapVector
		Implements AbstractDocument.Content

		''' <summary>
		''' Creates a new GapContent object.  Initial size defaults to 10.
		''' </summary>
		Public Sub New()
			Me.New(10)
		End Sub

		''' <summary>
		''' Creates a new GapContent object, with the initial
		''' size specified.  The initial size will not be allowed
		''' to go below 2, to give room for the implied break and
		''' the gap.
		''' </summary>
		''' <param name="initialLength"> the initial size </param>
		Public Sub New(ByVal initialLength As Integer)
			MyBase.New(Math.Max(initialLength,2))
			Dim implied As Char() = New Char(0){}
			implied(0) = ControlChars.Lf
			replace(0, 0, implied, implied.Length)

			marks = New MarkVector
			search = New MarkData(Me, 0)
			queue = New ReferenceQueue(Of StickyPosition)
		End Sub

		''' <summary>
		''' Allocate an array to store items of the type
		''' appropriate (which is determined by the subclass).
		''' </summary>
		Protected Friend Overrides Function allocateArray(ByVal len As Integer) As Object
			Return New Char(len - 1){}
		End Function

		''' <summary>
		''' Get the length of the allocated array.
		''' </summary>
		Protected Friend Property Overrides arrayLength As Integer
			Get
				Dim carray As Char() = CType(array, Char())
				Return carray.Length
			End Get
		End Property

		' --- AbstractDocument.Content methods -------------------------

		''' <summary>
		''' Returns the length of the content.
		''' </summary>
		''' <returns> the length &gt;= 1 </returns>
		''' <seealso cref= AbstractDocument.Content#length </seealso>
		Public Overridable Function length() As Integer Implements AbstractDocument.Content.length
			Dim len As Integer = arrayLength - (gapEnd - gapStart)
			Return len
		End Function

		''' <summary>
		''' Inserts a string into the content.
		''' </summary>
		''' <param name="where"> the starting position &gt;= 0, &lt; length() </param>
		''' <param name="str"> the non-null string to insert </param>
		''' <returns> an UndoableEdit object for undoing </returns>
		''' <exception cref="BadLocationException"> if the specified position is invalid </exception>
		''' <seealso cref= AbstractDocument.Content#insertString </seealso>
		Public Overridable Function insertString(ByVal where As Integer, ByVal str As String) As javax.swing.undo.UndoableEdit
			If where > length() OrElse where < 0 Then Throw New BadLocationException("Invalid insert", length())
			Dim ___chars As Char() = str.ToCharArray()
			replace(where, 0, ___chars, ___chars.Length)
			Return New InsertUndo(Me, where, str.Length)
		End Function

		''' <summary>
		''' Removes part of the content.
		''' </summary>
		''' <param name="where"> the starting position &gt;= 0, where + nitems &lt; length() </param>
		''' <param name="nitems"> the number of characters to remove &gt;= 0 </param>
		''' <returns> an UndoableEdit object for undoing </returns>
		''' <exception cref="BadLocationException"> if the specified position is invalid </exception>
		''' <seealso cref= AbstractDocument.Content#remove </seealso>
		Public Overridable Function remove(ByVal where As Integer, ByVal nitems As Integer) As javax.swing.undo.UndoableEdit
			If where + nitems >= length() Then Throw New BadLocationException("Invalid remove", length() + 1)
			Dim removedString As String = getString(where, nitems)
			Dim edit As javax.swing.undo.UndoableEdit = New RemoveUndo(Me, where, removedString)
			replace(where, nitems, empty, 0)
			Return edit

		End Function

		''' <summary>
		''' Retrieves a portion of the content.
		''' </summary>
		''' <param name="where"> the starting position &gt;= 0 </param>
		''' <param name="len"> the length to retrieve &gt;= 0 </param>
		''' <returns> a string representing the content </returns>
		''' <exception cref="BadLocationException"> if the specified position is invalid </exception>
		''' <seealso cref= AbstractDocument.Content#getString </seealso>
		Public Overridable Function getString(ByVal where As Integer, ByVal len As Integer) As String Implements AbstractDocument.Content.getString
			Dim s As New Segment
			getChars(where, len, s)
			Return New String(s.array, s.offset, s.count)
		End Function

		''' <summary>
		''' Retrieves a portion of the content.  If the desired content spans
		''' the gap, we copy the content.  If the desired content does not
		''' span the gap, the actual store is returned to avoid the copy since
		''' it is contiguous.
		''' </summary>
		''' <param name="where"> the starting position &gt;= 0, where + len &lt;= length() </param>
		''' <param name="len"> the number of characters to retrieve &gt;= 0 </param>
		''' <param name="chars"> the Segment object to return the characters in </param>
		''' <exception cref="BadLocationException"> if the specified position is invalid </exception>
		''' <seealso cref= AbstractDocument.Content#getChars </seealso>
		Public Overridable Sub getChars(ByVal where As Integer, ByVal len As Integer, ByVal chars As Segment) Implements AbstractDocument.Content.getChars
			Dim [end] As Integer = where + len
			If where < 0 OrElse [end] < 0 Then Throw New BadLocationException("Invalid location", -1)
			If [end] > length() OrElse where > length() Then Throw New BadLocationException("Invalid location", length() + 1)
			Dim g0 As Integer = gapStart
			Dim g1 As Integer = gapEnd
			Dim ___array As Char() = CType(array, Char())
			If (where + len) <= g0 Then
				' below gap
				chars.array = ___array
				chars.offset = where
			ElseIf where >= g0 Then
				' above gap
				chars.array = ___array
				chars.offset = g1 + where - g0
			Else
				' spans the gap
				Dim before As Integer = g0 - where
				If chars.partialReturn Then
					' partial return allowed, return amount before the gap
					chars.array = ___array
					chars.offset = where
					chars.count = before
					Return
				End If
				' partial return not allowed, must copy
				chars.array = New Char(len - 1){}
				chars.offset = 0
				Array.Copy(___array, where, chars.array, 0, before)
				Array.Copy(___array, g1, chars.array, before, len - before)
			End If
			chars.count = len
		End Sub

		''' <summary>
		''' Creates a position within the content that will
		''' track change as the content is mutated.
		''' </summary>
		''' <param name="offset"> the offset to track &gt;= 0 </param>
		''' <returns> the position </returns>
		''' <exception cref="BadLocationException"> if the specified position is invalid </exception>
		Public Overridable Function createPosition(ByVal offset As Integer) As Position Implements AbstractDocument.Content.createPosition
			Do While queue.poll() IsNot Nothing
				unusedMarks += 1
			Loop
			If unusedMarks > Math.Max(5, (marks.size() \ 10)) Then removeUnusedMarks()
			Dim g0 As Integer = gapStart
			Dim g1 As Integer = gapEnd
			Dim index As Integer = If(offset < g0, offset, offset + (g1 - g0))
			search.index = index
			Dim sortIndex As Integer = findSortIndex(search)
			Dim m As MarkData
			Dim position As StickyPosition
			m = marks.elementAt(sortIndex)
			position = m.position
			If sortIndex < marks.size() AndAlso m .index = index AndAlso position IsNot Nothing Then
				'position references the correct StickyPostition
			Else
				position = New StickyPosition(Me)
				m = New MarkData(Me, index,position,queue)
				position.mark = m
				marks.insertElementAt(m, sortIndex)
			End If

			Return position
		End Function

		''' <summary>
		''' Holds the data for a mark... separately from
		''' the real mark so that the real mark (Position
		''' that the caller of createPosition holds) can be
		''' collected if there are no more references to
		''' it.  The update table holds only a reference
		''' to this data.
		''' </summary>
		Friend NotInheritable Class MarkData
			Inherits WeakReference(Of StickyPosition)

			Private ReadOnly outerInstance As GapContent


			Friend Sub New(ByVal outerInstance As GapContent, ByVal index As Integer)
					Me.outerInstance = outerInstance
				MyBase.New(Nothing)
				Me.index = index
			End Sub
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(ByVal outerInstance As GapContent, Of T1)(ByVal index As Integer, ByVal position As StickyPosition, ByVal queue As ReferenceQueue(Of T1))
					Me.outerInstance = outerInstance
				MyBase.New(position, queue)
				Me.index = index
			End Sub

			''' <summary>
			''' Fetch the location in the contiguous sequence
			''' being modeled.  The index in the gap array
			''' is held by the mark, so it is adjusted according
			''' to it's relationship to the gap.
			''' </summary>
			Public Property offset As Integer
				Get
					Dim g0 As Integer = outerInstance.gapStart
					Dim g1 As Integer = outerInstance.gapEnd
					Dim offs As Integer = If(index < g0, index, index - (g1 - g0))
					Return Math.Max(offs, 0)
				End Get
			End Property

			Friend Property position As StickyPosition
				Get
					Return get()
				End Get
			End Property
			Friend index As Integer
		End Class

		Friend NotInheritable Class StickyPosition
			Implements Position

			Private ReadOnly outerInstance As GapContent


			Friend Sub New(ByVal outerInstance As GapContent)
					Me.outerInstance = outerInstance
			End Sub

			Friend Property mark As MarkData
				Set(ByVal mark As MarkData)
					Me.mark = mark
				End Set
			End Property

			Public Property offset As Integer Implements Position.getOffset
				Get
					Return mark.offset
				End Get
			End Property

			Public Overrides Function ToString() As String
				Return Convert.ToString(offset)
			End Function

			Friend mark As MarkData
		End Class

		' --- variables --------------------------------------

		Private Shared ReadOnly empty As Char() = New Char(){}
		<NonSerialized> _
		Private marks As MarkVector

		''' <summary>
		''' Record used for searching for the place to
		''' start updating mark indexs when the gap
		''' boundaries are moved.
		''' </summary>
		<NonSerialized> _
		Private search As MarkData

		''' <summary>
		''' The number of unused mark entries
		''' </summary>
		<NonSerialized> _
		Private unusedMarks As Integer = 0

		<NonSerialized> _
		Private queue As ReferenceQueue(Of StickyPosition)

		Friend Const GROWTH_SIZE As Integer = 1024 * 512

		' --- gap management -------------------------------

		''' <summary>
		''' Make the gap bigger, moving any necessary data and updating
		''' the appropriate marks
		''' </summary>
		Protected Friend Overrides Sub shiftEnd(ByVal newSize As Integer)
			Dim oldGapEnd As Integer = gapEnd

			MyBase.shiftEnd(newSize)

			' Adjust marks.
			Dim dg As Integer = gapEnd - oldGapEnd
			Dim adjustIndex As Integer = findMarkAdjustIndex(oldGapEnd)
			Dim n As Integer = marks.size()
			For i As Integer = adjustIndex To n - 1
				Dim mark As MarkData = marks.elementAt(i)
				mark.index += dg
			Next i
		End Sub

		''' <summary>
		''' Overridden to make growth policy less agressive for large
		''' text amount.
		''' </summary>
		Friend Overrides Function getNewArraySize(ByVal reqSize As Integer) As Integer
			If reqSize < GROWTH_SIZE Then
				Return MyBase.getNewArraySize(reqSize)
			Else
				Return reqSize + GROWTH_SIZE
			End If
		End Function

		''' <summary>
		''' Move the start of the gap to a new location,
		''' without changing the size of the gap.  This
		''' moves the data in the array and updates the
		''' marks accordingly.
		''' </summary>
		Protected Friend Overrides Sub shiftGap(ByVal newGapStart As Integer)
			Dim oldGapStart As Integer = gapStart
			Dim dg As Integer = newGapStart - oldGapStart
			Dim oldGapEnd As Integer = gapEnd
			Dim newGapEnd As Integer = oldGapEnd + dg
			Dim gapSize As Integer = oldGapEnd - oldGapStart

			' shift gap in the character array
			MyBase.shiftGap(newGapStart)

			' update the marks
			If dg > 0 Then
				' Move gap up, move data and marks down.
				Dim adjustIndex As Integer = findMarkAdjustIndex(oldGapStart)
				Dim n As Integer = marks.size()
				For i As Integer = adjustIndex To n - 1
					Dim mark As MarkData = marks.elementAt(i)
					If mark.index >= newGapEnd Then Exit For
					mark.index -= gapSize
				Next i
			ElseIf dg < 0 Then
				' Move gap down, move data and marks up.
				Dim adjustIndex As Integer = findMarkAdjustIndex(newGapStart)
				Dim n As Integer = marks.size()
				For i As Integer = adjustIndex To n - 1
					Dim mark As MarkData = marks.elementAt(i)
					If mark.index >= oldGapEnd Then Exit For
					mark.index += gapSize
				Next i
			End If
			resetMarksAtZero()
		End Sub

		''' <summary>
		''' Resets all the marks that have an offset of 0 to have an index of
		''' zero as well.
		''' </summary>
		Protected Friend Overridable Sub resetMarksAtZero()
			If marks IsNot Nothing AndAlso gapStart = 0 Then
				Dim g1 As Integer = gapEnd
				Dim counter As Integer = 0
				Dim maxCounter As Integer = marks.size()
				Do While counter < maxCounter
					Dim mark As MarkData = marks.elementAt(counter)
					If mark.index <= g1 Then
						mark.index = 0
					Else
						Exit Do
					End If
					counter += 1
				Loop
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
		Protected Friend Overrides Sub shiftGapStartDown(ByVal newGapStart As Integer)
			' Push aside all marks from oldGapStart down to newGapStart.
			Dim adjustIndex As Integer = findMarkAdjustIndex(newGapStart)
			Dim n As Integer = marks.size()
			Dim g0 As Integer = gapStart
			Dim g1 As Integer = gapEnd
			For i As Integer = adjustIndex To n - 1
				Dim mark As MarkData = marks.elementAt(i)
				If mark.index > g0 Then Exit For
				mark.index = g1
			Next i

			' shift the gap in the character array
			MyBase.shiftGapStartDown(newGapStart)

			resetMarksAtZero()
		End Sub

		''' <summary>
		''' Adjust the gap end upward.  This doesn't move
		''' any data, but it does update any marks affected
		''' by the boundary change. All marks from the old
		''' gap end up to the new gap end are squeezed
		''' to the end of the gap (their location has been
		''' removed).
		''' </summary>
		Protected Friend Overrides Sub shiftGapEndUp(ByVal newGapEnd As Integer)
			Dim adjustIndex As Integer = findMarkAdjustIndex(gapEnd)
			Dim n As Integer = marks.size()
			For i As Integer = adjustIndex To n - 1
				Dim mark As MarkData = marks.elementAt(i)
				If mark.index >= newGapEnd Then Exit For
				mark.index = newGapEnd
			Next i

			' shift the gap in the character array
			MyBase.shiftGapEndUp(newGapEnd)

			resetMarksAtZero()
		End Sub

		''' <summary>
		''' Compares two marks.
		''' </summary>
		''' <param name="o1"> the first object </param>
		''' <param name="o2"> the second object </param>
		''' <returns> < 0 if o1 < o2, 0 if the same, > 0 if o1 > o2 </returns>
		Friend Function compare(ByVal o1 As MarkData, ByVal o2 As MarkData) As Integer
			If o1.index < o2.index Then
				Return -1
			ElseIf o1.index > o2.index Then
				Return 1
			Else
				Return 0
			End If
		End Function

		''' <summary>
		''' Finds the index to start mark adjustments given
		''' some search index.
		''' </summary>
		Friend Function findMarkAdjustIndex(ByVal searchIndex As Integer) As Integer
			search.index = Math.Max(searchIndex, 1)
			Dim index As Integer = findSortIndex(search)

			' return the first in the series
			' (ie. there may be duplicates).
			For i As Integer = index - 1 To 0 Step -1
				Dim d As MarkData = marks.elementAt(i)
				If d.index <> search.index Then Exit For
				index -= 1
			Next i
			Return index
		End Function

		''' <summary>
		''' Finds the index of where to insert a new mark.
		''' </summary>
		''' <param name="o"> the mark to insert </param>
		''' <returns> the index </returns>
		Friend Function findSortIndex(ByVal o As MarkData) As Integer
			Dim lower As Integer = 0
			Dim upper As Integer = marks.size() - 1
			Dim mid As Integer = 0

			If upper = -1 Then Return 0

			Dim cmp As Integer
			Dim last As MarkData = marks.elementAt(upper)
			cmp = compare(o, last)
			If cmp > 0 Then Return upper + 1

			Do While lower <= upper
				mid = lower + ((upper - lower) \ 2)
				Dim entry As MarkData = marks.elementAt(mid)
				cmp = compare(o, entry)

				If cmp = 0 Then
					' found a match
					Return mid
				ElseIf cmp < 0 Then
					upper = mid - 1
				Else
					lower = mid + 1
				End If
			Loop

			' didn't find it, but we indicate the index of where it would belong.
			Return If(cmp < 0, mid, mid + 1)
		End Function

		''' <summary>
		''' Remove all unused marks out of the sorted collection
		''' of marks.
		''' </summary>
		Friend Sub removeUnusedMarks()
			Dim n As Integer = marks.size()
			Dim cleaned As New MarkVector(n)
			For i As Integer = 0 To n - 1
				Dim mark As MarkData = marks.elementAt(i)
				If mark.get() IsNot Nothing Then cleaned.addElement(mark)
			Next i
			marks = cleaned
			unusedMarks = 0
		End Sub


		Friend Class MarkVector
			Inherits GapVector

			Friend Sub New()
				MyBase.New()
			End Sub

			Friend Sub New(ByVal size As Integer)
				MyBase.New(size)
			End Sub

			''' <summary>
			''' Allocate an array to store items of the type
			''' appropriate (which is determined by the subclass).
			''' </summary>
			Protected Friend Overrides Function allocateArray(ByVal len As Integer) As Object
				Return New MarkData(len - 1){}
			End Function

			''' <summary>
			''' Get the length of the allocated array
			''' </summary>
			Protected Friend Property Overrides arrayLength As Integer
				Get
					Dim marks As MarkData() = CType(array, MarkData())
					Return marks.Length
				End Get
			End Property

			''' <summary>
			''' Returns the number of marks currently held
			''' </summary>
			Public Overridable Function size() As Integer
				Dim len As Integer = arrayLength - (gapEnd - gapStart)
				Return len
			End Function

			''' <summary>
			''' Inserts a mark into the vector
			''' </summary>
			Public Overridable Sub insertElementAt(ByVal m As MarkData, ByVal index As Integer)
				oneMark(0) = m
				replace(index, 0, oneMark, 1)
			End Sub

			''' <summary>
			''' Add a mark to the end
			''' </summary>
			Public Overridable Sub addElement(ByVal m As MarkData)
				insertElementAt(m, size())
			End Sub

			''' <summary>
			''' Fetches the mark at the given index
			''' </summary>
			Public Overridable Function elementAt(ByVal index As Integer) As MarkData
				Dim g0 As Integer = gapStart
				Dim g1 As Integer = gapEnd
				Dim ___array As MarkData() = CType(array, MarkData())
				If index < g0 Then
					' below gap
					Return ___array(index)
				Else
					' above gap
					index += g1 - g0
					Return ___array(index)
				End If
			End Function

			''' <summary>
			''' Replaces the elements in the specified range with the passed
			''' in objects. This will NOT adjust the gap. The passed in indices
			''' do not account for the gap, they are the same as would be used
			''' int <code>elementAt</code>.
			''' </summary>
			Protected Friend Overridable Sub replaceRange(ByVal start As Integer, ByVal [end] As Integer, ByVal marks As Object())
				Dim g0 As Integer = gapStart
				Dim g1 As Integer = gapEnd
				Dim index As Integer = start
				Dim newIndex As Integer = 0
				Dim ___array As Object() = CType(array, Object())
				If start >= g0 Then
					' Completely passed gap
					index += (g1 - g0)
					[end] += (g1 - g0)
				ElseIf [end] >= g0 Then
					' straddles gap
					[end] += (g1 - g0)
					Do While index < g0
						___array(index) = marks(newIndex)
						newIndex += 1
						index += 1
					Loop
					index = g1
				Else
					' below gap
					Do While index < [end]
						___array(index) = marks(newIndex)
						newIndex += 1
						index += 1
					Loop
				End If
				Do While index < [end]
					___array(index) = marks(newIndex)
					newIndex += 1
					index += 1
				Loop
			End Sub

			Friend oneMark As MarkData() = New MarkData(0){}

		End Class

		' --- serialization -------------------------------------

		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			marks = New MarkVector
			search = New MarkData(Me, 0)
			queue = New ReferenceQueue(Of StickyPosition)
		End Sub


		' --- undo support --------------------------------------

		''' <summary>
		''' Returns a Vector containing instances of UndoPosRef for the
		''' Positions in the range
		''' <code>offset</code> to <code>offset</code> + <code>length</code>.
		''' If <code>v</code> is not null the matching Positions are placed in
		''' there. The vector with the resulting Positions are returned.
		''' </summary>
		''' <param name="v"> the Vector to use, with a new one created on null </param>
		''' <param name="offset"> the starting offset &gt;= 0 </param>
		''' <param name="length"> the length &gt;= 0 </param>
		''' <returns> the set of instances </returns>
		Protected Friend Overridable Function getPositionsInRange(ByVal v As ArrayList, ByVal offset As Integer, ByVal length As Integer) As ArrayList
			Dim endOffset As Integer = offset + length
			Dim startIndex As Integer
			Dim endIndex As Integer
			Dim g0 As Integer = gapStart
			Dim g1 As Integer = gapEnd

			' Find the index of the marks.
			If offset < g0 Then
				If offset = 0 Then
					' findMarkAdjustIndex start at 1!
					startIndex = 0
				Else
					startIndex = findMarkAdjustIndex(offset)
				End If
				If endOffset >= g0 Then
					endIndex = findMarkAdjustIndex(endOffset + (g1 - g0) + 1)
				Else
					endIndex = findMarkAdjustIndex(endOffset + 1)
				End If
			Else
				startIndex = findMarkAdjustIndex(offset + (g1 - g0))
				endIndex = findMarkAdjustIndex(endOffset + (g1 - g0) + 1)
			End If

			Dim placeIn As ArrayList = If(v Is Nothing, New ArrayList(Math.Max(1, endIndex - startIndex)), v)

			For counter As Integer = startIndex To endIndex - 1
				placeIn.Add(New UndoPosRef(Me, marks.elementAt(counter)))
			Next counter
			Return placeIn
		End Function

		''' <summary>
		''' Resets the location for all the UndoPosRef instances
		''' in <code>positions</code>.
		''' <p>
		''' This is meant for internal usage, and is generally not of interest
		''' to subclasses.
		''' </summary>
		''' <param name="positions"> the UndoPosRef instances to reset </param>
		Protected Friend Overridable Sub updateUndoPositions(ByVal positions As ArrayList, ByVal offset As Integer, ByVal length As Integer)
			' Find the indexs of the end points.
			Dim endOffset As Integer = offset + length
			Dim g1 As Integer = gapEnd
			Dim startIndex As Integer
			Dim endIndex As Integer = findMarkAdjustIndex(g1 + 1)

			If offset <> 0 Then
				startIndex = findMarkAdjustIndex(g1)
			Else
				startIndex = 0
			End If

			' Reset the location of the refenences.
			For counter As Integer = positions.Count - 1 To 0 Step -1
				Dim ref As UndoPosRef = CType(positions(counter), UndoPosRef)
				ref.resetLocation(endOffset, g1)
			Next counter
			' We have to resort the marks in the range startIndex to endIndex.
			' We can take advantage of the fact that it will be in
			' increasing order, accept there will be a bunch of MarkData's with
			' the index g1 (or 0 if offset == 0) interspersed throughout.
			If startIndex < endIndex Then
				Dim sorted As Object() = New Object(endIndex - startIndex - 1){}
				Dim addIndex As Integer = 0
				Dim counter As Integer
				If offset = 0 Then
					' If the offset is 0, the positions won't have incremented,
					' have to do the reverse thing.
					' Find the elements in startIndex whose index is 0
					For counter = startIndex To endIndex - 1
						Dim mark As MarkData = marks.elementAt(counter)
						If mark.index = 0 Then
							sorted(addIndex) = mark
							addIndex += 1
						End If
					Next counter
					For counter = startIndex To endIndex - 1
						Dim mark As MarkData = marks.elementAt(counter)
						If mark.index <> 0 Then
							sorted(addIndex) = mark
							addIndex += 1
						End If
					Next counter
				Else
					For counter = startIndex To endIndex - 1
						Dim mark As MarkData = marks.elementAt(counter)
						If mark.index <> g1 Then
							sorted(addIndex) = mark
							addIndex += 1
						End If
					Next counter
					For counter = startIndex To endIndex - 1
						Dim mark As MarkData = marks.elementAt(counter)
						If mark.index = g1 Then
							sorted(addIndex) = mark
							addIndex += 1
						End If
					Next counter
				End If
				' And replace
				marks.replaceRange(startIndex, endIndex, sorted)
			End If
		End Sub

		''' <summary>
		''' Used to hold a reference to a Mark that is being reset as the
		''' result of removing from the content.
		''' </summary>
		Friend NotInheritable Class UndoPosRef
			Private ReadOnly outerInstance As GapContent

			Friend Sub New(ByVal outerInstance As GapContent, ByVal rec As MarkData)
					Me.outerInstance = outerInstance
				Me.rec = rec
				Me.undoLocation = rec.offset
			End Sub

			''' <summary>
			''' Resets the location of the Position to the offset when the
			''' receiver was instantiated.
			''' </summary>
			''' <param name="endOffset"> end location of inserted string. </param>
			''' <param name="g1"> resulting end of gap. </param>
			Protected Friend Sub resetLocation(ByVal endOffset As Integer, ByVal g1 As Integer)
				If undoLocation <> endOffset Then
					Me.rec.index = undoLocation
				Else
					Me.rec.index = g1
				End If
			End Sub

			''' <summary>
			''' Previous Offset of rec. </summary>
			Protected Friend undoLocation As Integer
			''' <summary>
			''' Mark to reset offset. </summary>
			Protected Friend rec As MarkData
		End Class ' End of GapContent.UndoPosRef


		''' <summary>
		''' UnoableEdit created for inserts.
		''' </summary>
		Friend Class InsertUndo
			Inherits javax.swing.undo.AbstractUndoableEdit

			Private ReadOnly outerInstance As GapContent

			Protected Friend Sub New(ByVal outerInstance As GapContent, ByVal offset As Integer, ByVal length As Integer)
					Me.outerInstance = outerInstance
				MyBase.New()
				Me.offset = offset
				Me.length = length
			End Sub

			Public Overrides Sub undo()
				MyBase.undo()
				Try
					' Get the Positions in the range being removed.
					posRefs = outerInstance.getPositionsInRange(Nothing, offset, length)
					[string] = outerInstance.getString(offset, length)
					outerInstance.remove(offset, length)
				Catch bl As BadLocationException
				  Throw New javax.swing.undo.CannotUndoException
				End Try
			End Sub

			Public Overrides Sub redo()
				MyBase.redo()
				Try
					outerInstance.insertString(offset, [string])
					[string] = Nothing
					' Update the Positions that were in the range removed.
					If posRefs IsNot Nothing Then
						outerInstance.updateUndoPositions(posRefs, offset, length)
						posRefs = Nothing
					End If
				Catch bl As BadLocationException
					Throw New javax.swing.undo.CannotRedoException
				End Try
			End Sub

			''' <summary>
			''' Where string was inserted. </summary>
			Protected Friend offset As Integer
			''' <summary>
			''' Length of string inserted. </summary>
			Protected Friend length As Integer
			''' <summary>
			''' The string that was inserted. This will only be valid after an
			''' undo. 
			''' </summary>
			Protected Friend [string] As String
			''' <summary>
			''' An array of instances of UndoPosRef for the Positions in the
			''' range that was removed, valid after undo. 
			''' </summary>
			Protected Friend posRefs As ArrayList
		End Class ' GapContent.InsertUndo


		''' <summary>
		''' UndoableEdit created for removes.
		''' </summary>
		Friend Class RemoveUndo
			Inherits javax.swing.undo.AbstractUndoableEdit

			Private ReadOnly outerInstance As GapContent

			Protected Friend Sub New(ByVal outerInstance As GapContent, ByVal offset As Integer, ByVal [string] As String)
					Me.outerInstance = outerInstance
				MyBase.New()
				Me.offset = offset
				Me.string = [string]
				Me.length = [string].Length
				posRefs = outerInstance.getPositionsInRange(Nothing, offset, length)
			End Sub

			Public Overrides Sub undo()
				MyBase.undo()
				Try
					outerInstance.insertString(offset, [string])
					' Update the Positions that were in the range removed.
					If posRefs IsNot Nothing Then
						outerInstance.updateUndoPositions(posRefs, offset, length)
						posRefs = Nothing
					End If
					[string] = Nothing
				Catch bl As BadLocationException
				  Throw New javax.swing.undo.CannotUndoException
				End Try
			End Sub

			Public Overrides Sub redo()
				MyBase.redo()
				Try
					[string] = outerInstance.getString(offset, length)
					' Get the Positions in the range being removed.
					posRefs = outerInstance.getPositionsInRange(Nothing, offset, length)
					outerInstance.remove(offset, length)
				Catch bl As BadLocationException
				  Throw New javax.swing.undo.CannotRedoException
				End Try
			End Sub

			''' <summary>
			''' Where the string was removed from. </summary>
			Protected Friend offset As Integer
			''' <summary>
			''' Length of string removed. </summary>
			Protected Friend length As Integer
			''' <summary>
			''' The string that was removed. This is valid when redo is valid. </summary>
			Protected Friend [string] As String
			''' <summary>
			''' An array of instances of UndoPosRef for the Positions in the
			''' range that was removed, valid before undo. 
			''' </summary>
			Protected Friend posRefs As ArrayList
		End Class ' GapContent.RemoveUndo
	End Class

End Namespace
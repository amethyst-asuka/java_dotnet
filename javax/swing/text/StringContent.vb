Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Collections.Generic
Imports javax.swing.undo

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
	''' An implementation of the AbstractDocument.Content interface that is
	''' a brute force implementation that is useful for relatively small
	''' documents and/or debugging.  It manages the character content
	''' as a simple character array.  It is also quite inefficient.
	''' <p>
	''' It is generally recommended that the gap buffer or piece table
	''' implementations be used instead.  This buffer does not scale up
	''' to large sizes.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	<Serializable> _
	Public NotInheritable Class StringContent
		Implements AbstractDocument.Content

		''' <summary>
		''' Creates a new StringContent object.  Initial size defaults to 10.
		''' </summary>
		Public Sub New()
			Me.New(10)
		End Sub

		''' <summary>
		''' Creates a new StringContent object, with the initial
		''' size specified.  If the length is &lt; 1, a size of 1 is used.
		''' </summary>
		''' <param name="initialLength"> the initial size </param>
		Public Sub New(ByVal initialLength As Integer)
			If initialLength < 1 Then initialLength = 1
			data = New Char(initialLength - 1){}
			data(0) = ControlChars.Lf
			count = 1
		End Sub

		''' <summary>
		''' Returns the length of the content.
		''' </summary>
		''' <returns> the length &gt;= 1 </returns>
		''' <seealso cref= AbstractDocument.Content#length </seealso>
		Public Function length() As Integer Implements AbstractDocument.Content.length
			Return count
		End Function

		''' <summary>
		''' Inserts a string into the content.
		''' </summary>
		''' <param name="where"> the starting position &gt;= 0 &amp;&amp; &lt; length() </param>
		''' <param name="str"> the non-null string to insert </param>
		''' <returns> an UndoableEdit object for undoing </returns>
		''' <exception cref="BadLocationException"> if the specified position is invalid </exception>
		''' <seealso cref= AbstractDocument.Content#insertString </seealso>
		Public Function insertString(ByVal where As Integer, ByVal str As String) As UndoableEdit Implements AbstractDocument.Content.insertString
			If where >= count OrElse where < 0 Then Throw New BadLocationException("Invalid location", count)
			Dim ___chars As Char() = str.ToCharArray()
			replace(where, 0, ___chars, 0, ___chars.Length)
			If marks IsNot Nothing Then updateMarksForInsert(where, str.Length)
			Return New InsertUndo(Me, where, str.Length)
		End Function

		''' <summary>
		''' Removes part of the content.  where + nitems must be &lt; length().
		''' </summary>
		''' <param name="where"> the starting position &gt;= 0 </param>
		''' <param name="nitems"> the number of characters to remove &gt;= 0 </param>
		''' <returns> an UndoableEdit object for undoing </returns>
		''' <exception cref="BadLocationException"> if the specified position is invalid </exception>
		''' <seealso cref= AbstractDocument.Content#remove </seealso>
		Public Function remove(ByVal where As Integer, ByVal nitems As Integer) As UndoableEdit Implements AbstractDocument.Content.remove
			If where + nitems >= count Then Throw New BadLocationException("Invalid range", count)
			Dim removedString As String = getString(where, nitems)
			Dim edit As UndoableEdit = New RemoveUndo(Me, where, removedString)
			replace(where, nitems, empty, 0, 0)
			If marks IsNot Nothing Then updateMarksForRemove(where, nitems)
			Return edit

		End Function

		''' <summary>
		''' Retrieves a portion of the content.  where + len must be &lt;= length().
		''' </summary>
		''' <param name="where"> the starting position &gt;= 0 </param>
		''' <param name="len"> the length to retrieve &gt;= 0 </param>
		''' <returns> a string representing the content; may be empty </returns>
		''' <exception cref="BadLocationException"> if the specified position is invalid </exception>
		''' <seealso cref= AbstractDocument.Content#getString </seealso>
		Public Function getString(ByVal where As Integer, ByVal len As Integer) As String Implements AbstractDocument.Content.getString
			If where + len > count Then Throw New BadLocationException("Invalid range", count)
			Return New String(data, where, len)
		End Function

		''' <summary>
		''' Retrieves a portion of the content.  where + len must be &lt;= length()
		''' </summary>
		''' <param name="where"> the starting position &gt;= 0 </param>
		''' <param name="len"> the number of characters to retrieve &gt;= 0 </param>
		''' <param name="chars"> the Segment object to return the characters in </param>
		''' <exception cref="BadLocationException"> if the specified position is invalid </exception>
		''' <seealso cref= AbstractDocument.Content#getChars </seealso>
		Public Sub getChars(ByVal where As Integer, ByVal len As Integer, ByVal chars As Segment) Implements AbstractDocument.Content.getChars
			If where + len > count Then Throw New BadLocationException("Invalid location", count)
			chars.array = data
			chars.offset = where
			chars.count = len
		End Sub

		''' <summary>
		''' Creates a position within the content that will
		''' track change as the content is mutated.
		''' </summary>
		''' <param name="offset"> the offset to create a position for &gt;= 0 </param>
		''' <returns> the position </returns>
		''' <exception cref="BadLocationException"> if the specified position is invalid </exception>
		Public Function createPosition(ByVal offset As Integer) As Position Implements AbstractDocument.Content.createPosition
			' some small documents won't have any sticky positions
			' at all, so the buffer is created lazily.
			If marks Is Nothing Then marks = New List(Of PosRec)
			Return New StickyPosition(Me, offset)
		End Function

		' --- local methods ---------------------------------------

		''' <summary>
		''' Replaces some of the characters in the array </summary>
		''' <param name="offset">  offset into the array to start the replace </param>
		''' <param name="length">  number of characters to remove </param>
		''' <param name="replArray"> replacement array </param>
		''' <param name="replOffset"> offset into the replacement array </param>
		''' <param name="replLength"> number of character to use from the
		'''   replacement array. </param>
		Friend Sub replace(ByVal offset As Integer, ByVal length As Integer, ByVal replArray As Char(), ByVal replOffset As Integer, ByVal replLength As Integer)
			Dim delta As Integer = replLength - length
			Dim src As Integer = offset + length
			Dim nmove As Integer = count - src
			Dim dest As Integer = src + delta
			If (count + delta) >= data.Length Then
				' need to grow the array
				Dim newLength As Integer = Math.Max(2*data.Length, count + delta)
				Dim newData As Char() = New Char(newLength - 1){}
				Array.Copy(data, 0, newData, 0, offset)
				Array.Copy(replArray, replOffset, newData, offset, replLength)
				Array.Copy(data, src, newData, dest, nmove)
				data = newData
			Else
				' patch the existing array
				Array.Copy(data, src, data, dest, nmove)
				Array.Copy(replArray, replOffset, data, offset, replLength)
			End If
			count = count + delta
		End Sub

		Friend Sub resize(ByVal ncount As Integer)
			Dim ndata As Char() = New Char(ncount - 1){}
			Array.Copy(data, 0, ndata, 0, Math.Min(ncount, count))
			data = ndata
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Sub updateMarksForInsert(ByVal offset As Integer, ByVal length As Integer)
			If offset = 0 Then offset = 1
			Dim n As Integer = marks.Count
			Dim i As Integer = 0
			Do While i < n
				Dim mark As PosRec = marks(i)
				If mark.unused Then
					' this record is no longer used, get rid of it
					marks.RemoveAt(i)
					i -= 1
					n -= 1
				ElseIf mark.offset >= offset Then
					mark.offset += length
				End If
				i += 1
			Loop
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Sub updateMarksForRemove(ByVal offset As Integer, ByVal length As Integer)
			Dim n As Integer = marks.Count
			Dim i As Integer = 0
			Do While i < n
				Dim mark As PosRec = marks(i)
				If mark.unused Then
					' this record is no longer used, get rid of it
					marks.RemoveAt(i)
					i -= 1
					n -= 1
				ElseIf mark.offset >= (offset + length) Then
					mark.offset -= length
				ElseIf mark.offset >= offset Then
					mark.offset = offset
				End If
				i += 1
			Loop
		End Sub

		''' <summary>
		''' Returns a Vector containing instances of UndoPosRef for the
		''' Positions in the range
		''' <code>offset</code> to <code>offset</code> + <code>length</code>.
		''' If <code>v</code> is not null the matching Positions are placed in
		''' there. The vector with the resulting Positions are returned.
		''' <p>
		''' This is meant for internal usage, and is generally not of interest
		''' to subclasses.
		''' </summary>
		''' <param name="v"> the Vector to use, with a new one created on null </param>
		''' <param name="offset"> the starting offset &gt;= 0 </param>
		''' <param name="length"> the length &gt;= 0 </param>
		''' <returns> the set of instances </returns>
		Protected Friend Function getPositionsInRange(ByVal v As ArrayList, ByVal offset As Integer, ByVal length As Integer) As ArrayList
			Dim n As Integer = marks.Count
			Dim [end] As Integer = offset + length
			Dim placeIn As ArrayList = If(v Is Nothing, New ArrayList, v)
			Dim i As Integer = 0
			Do While i < n
				Dim mark As PosRec = marks(i)
				If mark.unused Then
					' this record is no longer used, get rid of it
					marks.RemoveAt(i)
					i -= 1
					n -= 1
				ElseIf mark.offset >= offset AndAlso mark.offset <= [end] Then
					placeIn.Add(New UndoPosRef(Me, mark))
				End If
				i += 1
			Loop
			Return placeIn
		End Function

		''' <summary>
		''' Resets the location for all the UndoPosRef instances
		''' in <code>positions</code>.
		''' <p>
		''' This is meant for internal usage, and is generally not of interest
		''' to subclasses.
		''' </summary>
		''' <param name="positions"> the positions of the instances </param>
		Protected Friend Sub updateUndoPositions(ByVal positions As ArrayList)
			For counter As Integer = positions.Count - 1 To 0 Step -1
				Dim ref As UndoPosRef = CType(positions(counter), UndoPosRef)
				' Check if the Position is still valid.
				If ref.rec.unused Then
					positions.RemoveAt(counter)
				Else
					ref.resetLocation()
				End If
			Next counter
		End Sub

		Private Shared ReadOnly empty As Char() = New Char(){}
		Private data As Char()
		Private count As Integer
		<NonSerialized> _
		Friend marks As List(Of PosRec)

		''' <summary>
		''' holds the data for a mark... separately from
		''' the real mark so that the real mark can be
		''' collected if there are no more references to
		''' it.... the update table holds only a reference
		''' to this grungy thing.
		''' </summary>
		Friend NotInheritable Class PosRec
			Private ReadOnly outerInstance As StringContent


			Friend Sub New(ByVal outerInstance As StringContent, ByVal offset As Integer)
					Me.outerInstance = outerInstance
				Me.offset = offset
			End Sub

			Friend offset As Integer
			Friend unused As Boolean
		End Class

		''' <summary>
		''' This really wants to be a weak reference but
		''' in 1.1 we don't have a 100% pure solution for
		''' this... so this class trys to hack a solution
		''' to causing the marks to be collected.
		''' </summary>
		Friend NotInheritable Class StickyPosition
			Implements Position

			Private ReadOnly outerInstance As StringContent


			Friend Sub New(ByVal outerInstance As StringContent, ByVal offset As Integer)
					Me.outerInstance = outerInstance
				rec = New PosRec(offset)
				outerInstance.marks.Add(rec)
			End Sub

			Public Property offset As Integer Implements Position.getOffset
				Get
					Return rec.offset
				End Get
			End Property

			Protected Overrides Sub Finalize()
				' schedule the record to be removed later
				' on another thread.
				rec.unused = True
			End Sub

			Public Overrides Function ToString() As String
				Return Convert.ToString(offset)
			End Function

			Friend rec As PosRec
		End Class

		''' <summary>
		''' Used to hold a reference to a Position that is being reset as the
		''' result of removing from the content.
		''' </summary>
		Friend NotInheritable Class UndoPosRef
			Private ReadOnly outerInstance As StringContent

			Friend Sub New(ByVal outerInstance As StringContent, ByVal rec As PosRec)
					Me.outerInstance = outerInstance
				Me.rec = rec
				Me.undoLocation = rec.offset
			End Sub

			''' <summary>
			''' Resets the location of the Position to the offset when the
			''' receiver was instantiated.
			''' </summary>
			Protected Friend Sub resetLocation()
				rec.offset = undoLocation
			End Sub

			''' <summary>
			''' Location to reset to when resetLocatino is invoked. </summary>
			Protected Friend undoLocation As Integer
			''' <summary>
			''' Position to reset offset. </summary>
			Protected Friend rec As PosRec
		End Class

		''' <summary>
		''' UnoableEdit created for inserts.
		''' </summary>
		Friend Class InsertUndo
			Inherits AbstractUndoableEdit

			Private ReadOnly outerInstance As StringContent

			Protected Friend Sub New(ByVal outerInstance As StringContent, ByVal offset As Integer, ByVal length As Integer)
					Me.outerInstance = outerInstance
				MyBase.New()
				Me.offset = offset
				Me.length = length
			End Sub

			Public Overrides Sub undo()
				MyBase.undo()
				Try
					SyncLock StringContent.this
						' Get the Positions in the range being removed.
						If outerInstance.marks IsNot Nothing Then posRefs = outerInstance.getPositionsInRange(Nothing, offset, length)
						[string] = outerInstance.getString(offset, length)
						outerInstance.remove(offset, length)
					End SyncLock
				Catch bl As BadLocationException
				  Throw New CannotUndoException
				End Try
			End Sub

			Public Overrides Sub redo()
				MyBase.redo()
				Try
					SyncLock StringContent.this
						outerInstance.insertString(offset, [string])
						[string] = Nothing
						' Update the Positions that were in the range removed.
						If posRefs IsNot Nothing Then
							outerInstance.updateUndoPositions(posRefs)
							posRefs = Nothing
						End If
					End SyncLock
				Catch bl As BadLocationException
				  Throw New CannotRedoException
				End Try
			End Sub

			' Where the string goes.
			Protected Friend offset As Integer
			' Length of the string.
			Protected Friend length As Integer
			' The string that was inserted. To cut down on space needed this
			' will only be valid after an undo.
			Protected Friend [string] As String
			' An array of instances of UndoPosRef for the Positions in the
			' range that was removed, valid after undo.
			Protected Friend posRefs As ArrayList
		End Class


		''' <summary>
		''' UndoableEdit created for removes.
		''' </summary>
		Friend Class RemoveUndo
			Inherits AbstractUndoableEdit

			Private ReadOnly outerInstance As StringContent

			Protected Friend Sub New(ByVal outerInstance As StringContent, ByVal offset As Integer, ByVal [string] As String)
					Me.outerInstance = outerInstance
				MyBase.New()
				Me.offset = offset
				Me.string = [string]
				Me.length = [string].Length
				If outerInstance.marks IsNot Nothing Then posRefs = outerInstance.getPositionsInRange(Nothing, offset, length)
			End Sub

			Public Overrides Sub undo()
				MyBase.undo()
				Try
					SyncLock StringContent.this
						outerInstance.insertString(offset, [string])
						' Update the Positions that were in the range removed.
						If posRefs IsNot Nothing Then
							outerInstance.updateUndoPositions(posRefs)
							posRefs = Nothing
						End If
						[string] = Nothing
					End SyncLock
				Catch bl As BadLocationException
				  Throw New CannotUndoException
				End Try
			End Sub

			Public Overrides Sub redo()
				MyBase.redo()
				Try
					SyncLock StringContent.this
						[string] = outerInstance.getString(offset, length)
						' Get the Positions in the range being removed.
						If outerInstance.marks IsNot Nothing Then posRefs = outerInstance.getPositionsInRange(Nothing, offset, length)
						outerInstance.remove(offset, length)
					End SyncLock
				Catch bl As BadLocationException
				  Throw New CannotRedoException
				End Try
			End Sub

			' Where the string goes.
			Protected Friend offset As Integer
			' Length of the string.
			Protected Friend length As Integer
			' The string that was inserted. This will be null after an undo.
			Protected Friend [string] As String
			' An array of instances of UndoPosRef for the Positions in the
			' range that was removed, valid before undo.
			Protected Friend posRefs As ArrayList
		End Class
	End Class

End Namespace
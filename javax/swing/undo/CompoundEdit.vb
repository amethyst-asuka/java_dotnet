Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2004, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.undo


	''' <summary>
	''' A concrete subclass of AbstractUndoableEdit, used to assemble little
	''' UndoableEdits into great big ones.
	''' 
	''' @author Ray Ryan
	''' </summary>
	Public Class CompoundEdit
		Inherits AbstractUndoableEdit

		''' <summary>
		''' True if this edit has never received <code>end</code>.
		''' </summary>
		Friend inProgress As Boolean

		''' <summary>
		''' The collection of <code>UndoableEdit</code>s
		''' undone/redone en masse by this <code>CompoundEdit</code>.
		''' </summary>
		Protected Friend edits As List(Of UndoableEdit)

		Public Sub New()
			MyBase.New()
			inProgress = True
			edits = New List(Of UndoableEdit)
		End Sub

		''' <summary>
		''' Sends <code>undo</code> to all contained
		''' <code>UndoableEdits</code> in the reverse of
		''' the order in which they were added.
		''' </summary>
		Public Overrides Sub undo()
			MyBase.undo()
			Dim i As Integer = edits.Count
			Dim tempVar As Boolean = i > 0
			i -= 1
			Do While tempVar
				Dim e As UndoableEdit = edits(i)
				e.undo()
				tempVar = i > 0
				i -= 1
			Loop
		End Sub

		''' <summary>
		''' Sends <code>redo</code> to all contained
		''' <code>UndoableEdit</code>s in the order in
		''' which they were added.
		''' </summary>
		Public Overrides Sub redo()
			MyBase.redo()
			Dim cursor As System.Collections.IEnumerator = edits.elements()
			Do While cursor.hasMoreElements()
				CType(cursor.nextElement(), UndoableEdit).redo()
			Loop
		End Sub

		''' <summary>
		''' Returns the last <code>UndoableEdit</code> in
		''' <code>edits</code>, or <code>null</code>
		''' if <code>edits</code> is empty.
		''' </summary>
		Protected Friend Overridable Function lastEdit() As UndoableEdit
			Dim count As Integer = edits.Count
			If count > 0 Then
				Return edits(count-1)
			Else
				Return Nothing
			End If
		End Function

		''' <summary>
		''' Sends <code>die</code> to each subedit,
		''' in the reverse of the order that they were added.
		''' </summary>
		Public Overrides Sub die()
			Dim size As Integer = edits.Count
			For i As Integer = size-1 To 0 Step -1
				Dim e As UndoableEdit = edits(i)
	'          System.out.println("CompoundEdit(" + i + "): Discarding " +
	'                             e.getUndoPresentationName());
				e.die()
			Next i
			MyBase.die()
		End Sub

		''' <summary>
		''' If this edit is <code>inProgress</code>,
		''' accepts <code>anEdit</code> and returns true.
		''' 
		''' <p>The last edit added to this <code>CompoundEdit</code>
		''' is given a chance to <code>addEdit(anEdit)</code>.
		''' If it refuses (returns false), <code>anEdit</code> is
		''' given a chance to <code>replaceEdit</code> the last edit.
		''' If <code>anEdit</code> returns false here,
		''' it is added to <code>edits</code>.
		''' </summary>
		''' <param name="anEdit"> the edit to be added </param>
		''' <returns> true if the edit is <code>inProgress</code>;
		'''  otherwise returns false </returns>
		Public Overrides Function addEdit(ByVal anEdit As UndoableEdit) As Boolean
			If Not inProgress Then
				Return False
			Else
				Dim last As UndoableEdit = lastEdit()

				' If this is the first subedit received, just add it.
				' Otherwise, give the last one a chance to absorb the new
				' one.  If it won't, give the new one a chance to absorb
				' the last one.

				If last Is Nothing Then
					edits.Add(anEdit)
				ElseIf Not last.addEdit(anEdit) Then
					If anEdit.replaceEdit(last) Then edits.RemoveAt(edits.Count-1)
					edits.Add(anEdit)
				End If

				Return True
			End If
		End Function

		''' <summary>
		''' Sets <code>inProgress</code> to false.
		''' </summary>
		''' <seealso cref= #canUndo </seealso>
		''' <seealso cref= #canRedo </seealso>
		Public Overridable Sub [end]()
			inProgress = False
		End Sub

		''' <summary>
		''' Returns false if <code>isInProgress</code> or if super
		''' returns false.
		''' </summary>
		''' <seealso cref=     #isInProgress </seealso>
		Public Overrides Function canUndo() As Boolean
			Return (Not inProgress) AndAlso MyBase.canUndo()
		End Function

		''' <summary>
		''' Returns false if <code>isInProgress</code> or if super
		''' returns false.
		''' </summary>
		''' <seealso cref=     #isInProgress </seealso>
		Public Overrides Function canRedo() As Boolean
			Return (Not inProgress) AndAlso MyBase.canRedo()
		End Function

		''' <summary>
		''' Returns true if this edit is in progress--that is, it has not
		''' received end. This generally means that edits are still being
		''' added to it.
		''' </summary>
		''' <seealso cref=     #end </seealso>
		Public Overridable Property inProgress As Boolean
			Get
				Return inProgress
			End Get
		End Property

		''' <summary>
		''' Returns true if any of the <code>UndoableEdit</code>s
		''' in <code>edits</code> do.
		''' Returns false if they all return false.
		''' </summary>
		Public Property Overrides significant As Boolean
			Get
				Dim cursor As System.Collections.IEnumerator = edits.elements()
				Do While cursor.hasMoreElements()
					If CType(cursor.nextElement(), UndoableEdit).significant Then Return True
				Loop
				Return False
			End Get
		End Property

		''' <summary>
		''' Returns <code>getPresentationName</code> from the
		''' last <code>UndoableEdit</code> added to
		''' <code>edits</code>. If <code>edits</code> is empty,
		''' calls super.
		''' </summary>
		Public Property Overrides presentationName As String
			Get
				Dim last As UndoableEdit = lastEdit()
				If last IsNot Nothing Then
					Return last.presentationName
				Else
					Return MyBase.presentationName
				End If
			End Get
		End Property

		''' <summary>
		''' Returns <code>getUndoPresentationName</code>
		''' from the last <code>UndoableEdit</code>
		''' added to <code>edits</code>.
		''' If <code>edits</code> is empty, calls super.
		''' </summary>
		Public Property Overrides undoPresentationName As String
			Get
				Dim last As UndoableEdit = lastEdit()
				If last IsNot Nothing Then
					Return last.undoPresentationName
				Else
					Return MyBase.undoPresentationName
				End If
			End Get
		End Property

		''' <summary>
		''' Returns <code>getRedoPresentationName</code>
		''' from the last <code>UndoableEdit</code>
		''' added to <code>edits</code>.
		''' If <code>edits</code> is empty, calls super.
		''' </summary>
		Public Property Overrides redoPresentationName As String
			Get
				Dim last As UndoableEdit = lastEdit()
				If last IsNot Nothing Then
					Return last.redoPresentationName
				Else
					Return MyBase.redoPresentationName
				End If
			End Get
		End Property

		''' <summary>
		''' Returns a string that displays and identifies this
		''' object's properties.
		''' </summary>
		''' <returns> a String representation of this object </returns>
		Public Overrides Function ToString() As String
			Return MyBase.ToString() & " inProgress: " & inProgress & " edits: " & edits
		End Function
	End Class

End Namespace
Imports System

'
' * Copyright (c) 1997, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' An abstract implementation of <code>UndoableEdit</code>,
	''' implementing simple responses to all boolean methods in
	''' that interface.
	''' 
	''' @author Ray Ryan
	''' </summary>
	<Serializable> _
	Public Class AbstractUndoableEdit
		Implements UndoableEdit

		''' <summary>
		''' String returned by <code>getUndoPresentationName</code>;
		''' as of Java 2 platform v1.3.1 this field is no longer used. This value
		''' is now localized and comes from the defaults table with key
		''' <code>AbstractUndoableEdit.undoText</code>.
		''' </summary>
		''' <seealso cref= javax.swing.UIDefaults </seealso>
		Protected Friend Const UndoName As String = "Undo"

		''' <summary>
		''' String returned by <code>getRedoPresentationName</code>;
		''' as of Java 2 platform v1.3.1 this field is no longer used. This value
		''' is now localized and comes from the defaults table with key
		''' <code>AbstractUndoableEdit.redoText</code>.
		''' </summary>
		''' <seealso cref= javax.swing.UIDefaults </seealso>
		Protected Friend Const RedoName As String = "Redo"

		''' <summary>
		''' Defaults to true; becomes false if this edit is undone, true
		''' again if it is redone.
		''' </summary>
		Friend hasBeenDone As Boolean

		''' <summary>
		''' True if this edit has not received <code>die</code>; defaults
		''' to <code>true</code>.
		''' </summary>
		Friend alive As Boolean

		''' <summary>
		''' Creates an <code>AbstractUndoableEdit</code> which defaults
		''' <code>hasBeenDone</code> and <code>alive</code> to <code>true</code>.
		''' </summary>
		Public Sub New()
			MyBase.New()

			hasBeenDone = True
			alive = True
		End Sub

		''' <summary>
		''' Sets <code>alive</code> to false. Note that this
		''' is a one way operation; dead edits cannot be resurrected.
		''' Sending <code>undo</code> or <code>redo</code> to
		''' a dead edit results in an exception being thrown.
		''' 
		''' <p>Typically an edit is killed when it is consolidated by
		''' another edit's <code>addEdit</code> or <code>replaceEdit</code>
		''' method, or when it is dequeued from an <code>UndoManager</code>.
		''' </summary>
		Public Overridable Sub die() Implements UndoableEdit.die
			alive = False
		End Sub

		''' <summary>
		''' Throws <code>CannotUndoException</code> if <code>canUndo</code>
		''' returns <code>false</code>. Sets <code>hasBeenDone</code>
		''' to <code>false</code>. Subclasses should override to undo the
		''' operation represented by this edit. Override should begin with
		''' a call to super.
		''' </summary>
		''' <exception cref="CannotUndoException"> if <code>canUndo</code>
		'''    returns <code>false</code> </exception>
		''' <seealso cref=     #canUndo </seealso>
		Public Overridable Sub undo() Implements UndoableEdit.undo
			If Not canUndo() Then Throw New CannotUndoException
			hasBeenDone = False
		End Sub

		''' <summary>
		''' Returns true if this edit is <code>alive</code>
		''' and <code>hasBeenDone</code> is <code>true</code>.
		''' </summary>
		''' <returns> true if this edit is <code>alive</code>
		'''    and <code>hasBeenDone</code> is <code>true</code>
		''' </returns>
		''' <seealso cref=     #die </seealso>
		''' <seealso cref=     #undo </seealso>
		''' <seealso cref=     #redo </seealso>
		Public Overridable Function canUndo() As Boolean Implements UndoableEdit.canUndo
			Return alive AndAlso hasBeenDone
		End Function

		''' <summary>
		''' Throws <code>CannotRedoException</code> if <code>canRedo</code>
		''' returns false. Sets <code>hasBeenDone</code> to <code>true</code>.
		''' Subclasses should override to redo the operation represented by
		''' this edit. Override should begin with a call to super.
		''' </summary>
		''' <exception cref="CannotRedoException"> if <code>canRedo</code>
		'''     returns <code>false</code> </exception>
		''' <seealso cref=     #canRedo </seealso>
		Public Overridable Sub redo() Implements UndoableEdit.redo
			If Not canRedo() Then Throw New CannotRedoException
			hasBeenDone = True
		End Sub

		''' <summary>
		''' Returns <code>true</code> if this edit is <code>alive</code>
		''' and <code>hasBeenDone</code> is <code>false</code>.
		''' </summary>
		''' <returns> <code>true</code> if this edit is <code>alive</code>
		'''   and <code>hasBeenDone</code> is <code>false</code> </returns>
		''' <seealso cref=     #die </seealso>
		''' <seealso cref=     #undo </seealso>
		''' <seealso cref=     #redo </seealso>
		Public Overridable Function canRedo() As Boolean Implements UndoableEdit.canRedo
			Return alive AndAlso Not hasBeenDone
		End Function

		''' <summary>
		''' This default implementation returns false.
		''' </summary>
		''' <param name="anEdit"> the edit to be added </param>
		''' <returns> false
		''' </returns>
		''' <seealso cref= UndoableEdit#addEdit </seealso>
		Public Overridable Function addEdit(ByVal anEdit As UndoableEdit) As Boolean Implements UndoableEdit.addEdit
			Return False
		End Function

		''' <summary>
		''' This default implementation returns false.
		''' </summary>
		''' <param name="anEdit"> the edit to replace </param>
		''' <returns> false
		''' </returns>
		''' <seealso cref= UndoableEdit#replaceEdit </seealso>
		Public Overridable Function replaceEdit(ByVal anEdit As UndoableEdit) As Boolean Implements UndoableEdit.replaceEdit
			Return False
		End Function

		''' <summary>
		''' This default implementation returns true.
		''' </summary>
		''' <returns> true </returns>
		''' <seealso cref= UndoableEdit#isSignificant </seealso>
		Public Overridable Property significant As Boolean Implements UndoableEdit.isSignificant
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' This default implementation returns "". Used by
		''' <code>getUndoPresentationName</code> and
		''' <code>getRedoPresentationName</code> to
		''' construct the strings they return. Subclasses should override to
		''' return an appropriate description of the operation this edit
		''' represents.
		''' </summary>
		''' <returns> the empty string ""
		''' </returns>
		''' <seealso cref=     #getUndoPresentationName </seealso>
		''' <seealso cref=     #getRedoPresentationName </seealso>
		Public Overridable Property presentationName As String Implements UndoableEdit.getPresentationName
			Get
				Return ""
			End Get
		End Property

		''' <summary>
		''' Retreives the value from the defaults table with key
		''' <code>AbstractUndoableEdit.undoText</code> and returns
		''' that value followed by a space, followed by
		''' <code>getPresentationName</code>.
		''' If <code>getPresentationName</code> returns "",
		''' then the defaults value is returned alone.
		''' </summary>
		''' <returns> the value from the defaults table with key
		'''    <code>AbstractUndoableEdit.undoText</code>, followed
		'''    by a space, followed by <code>getPresentationName</code>
		'''    unless <code>getPresentationName</code> is "" in which
		'''    case, the defaults value is returned alone. </returns>
		''' <seealso cref= #getPresentationName </seealso>
		Public Overridable Property undoPresentationName As String Implements UndoableEdit.getUndoPresentationName
			Get
				Dim name As String = presentationName
				If Not "".Equals(name) Then
					name = javax.swing.UIManager.getString("AbstractUndoableEdit.undoText") & " " & name
				Else
					name = javax.swing.UIManager.getString("AbstractUndoableEdit.undoText")
				End If
    
				Return name
			End Get
		End Property

		''' <summary>
		''' Retreives the value from the defaults table with key
		''' <code>AbstractUndoableEdit.redoText</code> and returns
		''' that value followed by a space, followed by
		''' <code>getPresentationName</code>.
		''' If <code>getPresentationName</code> returns "",
		''' then the defaults value is returned alone.
		''' </summary>
		''' <returns> the value from the defaults table with key
		'''    <code>AbstractUndoableEdit.redoText</code>, followed
		'''    by a space, followed by <code>getPresentationName</code>
		'''    unless <code>getPresentationName</code> is "" in which
		'''    case, the defaults value is returned alone. </returns>
		''' <seealso cref= #getPresentationName </seealso>
		Public Overridable Property redoPresentationName As String Implements UndoableEdit.getRedoPresentationName
			Get
				Dim name As String = presentationName
				If Not "".Equals(name) Then
					name = javax.swing.UIManager.getString("AbstractUndoableEdit.redoText") & " " & name
				Else
					name = javax.swing.UIManager.getString("AbstractUndoableEdit.redoText")
				End If
    
				Return name
			End Get
		End Property

		''' <summary>
		''' Returns a string that displays and identifies this
		''' object's properties.
		''' </summary>
		''' <returns> a String representation of this object </returns>
		Public Overrides Function ToString() As String
			Return MyBase.ToString() & " hasBeenDone: " & hasBeenDone & " alive: " & alive
		End Function
	End Class

End Namespace
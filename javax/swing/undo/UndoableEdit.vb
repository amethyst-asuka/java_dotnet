Imports javax.swing.event

'
' * Copyright (c) 1997, 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' An <code>UndoableEdit</code> represents an edit.  The edit may
	''' be undone, or if already undone the edit may be redone.
	''' <p>
	''' <code>UndoableEdit</code> is designed to be used with the
	''' <code>UndoManager</code>.  As <code>UndoableEdit</code>s are generated
	''' by an <code>UndoableEditListener</code> they are typically added to
	''' the <code>UndoManager</code>.  When an <code>UndoableEdit</code>
	''' is added to an <code>UndoManager</code> the following occurs (assuming
	''' <code>end</code> has not been called on the <code>UndoManager</code>):
	''' <ol>
	''' <li>If the <code>UndoManager</code> contains edits it will call
	'''     <code>addEdit</code> on the current edit passing in the new edit
	'''     as the argument.  If <code>addEdit</code> returns true the
	'''     new edit is assumed to have been incorporated into the current edit and
	'''     the new edit will not be added to the list of current edits.
	'''     Edits can use <code>addEdit</code> as a way for smaller edits to
	'''     be incorporated into a larger edit and treated as a single edit.
	''' <li>If <code>addEdit</code> returns false <code>replaceEdit</code>
	'''     is called on the new edit with the current edit passed in as the
	'''     argument. This is the inverse of <code>addEdit</code> &#151;
	'''     if the new edit returns true from <code>replaceEdit</code>, the new
	'''     edit replaces the current edit.
	''' </ol>
	''' The <code>UndoManager</code> makes use of
	''' <code>isSignificant</code> to determine how many edits should be
	''' undone or redone.  The <code>UndoManager</code> will undo or redo
	''' all insignificant edits (<code>isSignificant</code> returns false)
	''' between the current edit and the last or
	''' next significant edit.   <code>addEdit</code> and
	''' <code>replaceEdit</code> can be used to treat multiple edits as
	''' a single edit, returning false from <code>isSignificant</code>
	''' allows for treating can be used to
	''' have many smaller edits undone or redone at once.  Similar functionality
	''' can also be done using the <code>addEdit</code> method.
	''' 
	''' @author Ray Ryan
	''' </summary>
	Public Interface UndoableEdit
		''' <summary>
		''' Undo the edit.
		''' </summary>
		''' <exception cref="CannotUndoException"> if this edit can not be undone </exception>
		Sub undo()

		''' <summary>
		''' Returns true if this edit may be undone.
		''' </summary>
		''' <returns> true if this edit may be undone </returns>
		Function canUndo() As Boolean

		''' <summary>
		''' Re-applies the edit.
		''' </summary>
		''' <exception cref="CannotRedoException"> if this edit can not be redone </exception>
		Sub redo()

		''' <summary>
		''' Returns true if this edit may be redone.
		''' </summary>
		''' <returns> true if this edit may be redone </returns>
		Function canRedo() As Boolean

		''' <summary>
		''' Informs the edit that it should no longer be used. Once an
		''' <code>UndoableEdit</code> has been marked as dead it can no longer
		''' be undone or redone.
		''' <p>
		''' This is a useful hook for cleaning up state no longer
		''' needed once undoing or redoing is impossible--for example,
		''' deleting file resources used by objects that can no longer be
		''' undeleted. <code>UndoManager</code> calls this before it dequeues edits.
		''' <p>
		''' Note that this is a one-way operation. There is no "un-die"
		''' method.
		''' </summary>
		''' <seealso cref= CompoundEdit#die </seealso>
		Sub die()

		''' <summary>
		''' Adds an <code>UndoableEdit</code> to this <code>UndoableEdit</code>.
		''' This method can be used to coalesce smaller edits into a larger
		''' compound edit.  For example, text editors typically allow
		''' undo operations to apply to words or sentences.  The text
		''' editor may choose to generate edits on each key event, but allow
		''' those edits to be coalesced into a more user-friendly unit, such as
		''' a word. In this case, the <code>UndoableEdit</code> would
		''' override <code>addEdit</code> to return true when the edits may
		''' be coalesced.
		''' <p>
		''' A return value of true indicates <code>anEdit</code> was incorporated
		''' into this edit.  A return value of false indicates <code>anEdit</code>
		''' may not be incorporated into this edit.
		''' <p>Typically the receiver is already in the queue of a
		''' <code>UndoManager</code> (or other <code>UndoableEditListener</code>),
		''' and is being given a chance to incorporate <code>anEdit</code>
		''' rather than letting it be added to the queue in turn.</p>
		''' 
		''' <p>If true is returned, from now on <code>anEdit</code> must return
		''' false from <code>canUndo</code> and <code>canRedo</code>,
		''' and must throw the appropriate exception on <code>undo</code> or
		''' <code>redo</code>.</p>
		''' </summary>
		''' <param name="anEdit"> the edit to be added </param>
		''' <returns> true if <code>anEdit</code> may be incorporated into this
		'''              edit </returns>
		Function addEdit(ByVal anEdit As UndoableEdit) As Boolean

		''' <summary>
		''' Returns true if this <code>UndoableEdit</code> should replace
		''' <code>anEdit</code>. This method is used by <code>CompoundEdit</code>
		''' and the <code>UndoManager</code>; it is called if
		''' <code>anEdit</code> could not be added to the current edit
		''' (<code>addEdit</code> returns false).
		''' <p>
		''' This method provides a way for an edit to replace an existing edit.
		''' <p>This message is the opposite of addEdit--anEdit has typically
		''' already been queued in an <code>UndoManager</code> (or other
		''' UndoableEditListener), and the receiver is being given a chance
		''' to take its place.</p>
		''' 
		''' <p>If true is returned, from now on anEdit must return false from
		''' canUndo() and canRedo(), and must throw the appropriate
		''' exception on undo() or redo().</p>
		''' </summary>
		''' <param name="anEdit"> the edit that replaces the current edit </param>
		''' <returns> true if this edit should replace <code>anEdit</code> </returns>
		Function replaceEdit(ByVal anEdit As UndoableEdit) As Boolean

		''' <summary>
		''' Returns true if this edit is considered significant.  A significant
		''' edit is typically an edit that should be presented to the user, perhaps
		''' on a menu item or tooltip.  The <code>UndoManager</code> will undo,
		''' or redo, all insignificant edits to the next significant edit.
		''' </summary>
		''' <returns> true if this edit is significant </returns>
		ReadOnly Property significant As Boolean

		''' <summary>
		''' Returns a localized, human-readable description of this edit, suitable
		''' for use in a change log, for example.
		''' </summary>
		''' <returns> description of this edit </returns>
		ReadOnly Property presentationName As String

		''' <summary>
		''' Returns a localized, human-readable description of the undoable form of
		''' this edit, suitable for use as an Undo menu item, for example.
		''' This is typically derived from <code>getPresentationName</code>.
		''' </summary>
		''' <returns> a description of the undoable form of this edit </returns>
		ReadOnly Property undoPresentationName As String

		''' <summary>
		''' Returns a localized, human-readable description of the redoable form of
		''' this edit, suitable for use as a Redo menu item, for example. This is
		''' typically derived from <code>getPresentationName</code>.
		''' </summary>
		''' <returns> a description of the redoable form of this edit </returns>
		ReadOnly Property redoPresentationName As String
	End Interface

End Namespace
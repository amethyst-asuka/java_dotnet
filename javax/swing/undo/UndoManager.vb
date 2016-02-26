Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports javax.swing.event

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

Namespace javax.swing.undo


	''' <summary>
	''' {@code UndoManager} manages a list of {@code UndoableEdits},
	''' providing a way to undo or redo the appropriate edits.  There are
	''' two ways to add edits to an <code>UndoManager</code>.  Add the edit
	''' directly using the <code>addEdit</code> method, or add the
	''' <code>UndoManager</code> to a bean that supports
	''' <code>UndoableEditListener</code>.  The following examples creates
	''' an <code>UndoManager</code> and adds it as an
	''' <code>UndoableEditListener</code> to a <code>JTextField</code>:
	''' <pre>
	'''   UndoManager undoManager = new UndoManager();
	'''   JTextField tf = ...;
	'''   tf.getDocument().addUndoableEditListener(undoManager);
	''' </pre>
	''' <p>
	''' <code>UndoManager</code> maintains an ordered list of edits and the
	''' index of the next edit in that list. The index of the next edit is
	''' either the size of the current list of edits, or if
	''' <code>undo</code> has been invoked it corresponds to the index
	''' of the last significant edit that was undone. When
	''' <code>undo</code> is invoked all edits from the index of the next
	''' edit to the last significant edit are undone, in reverse order.
	''' For example, consider an <code>UndoManager</code> consisting of the
	''' following edits: <b>A</b> <i>b</i> <i>c</i> <b>D</b>.  Edits with a
	''' upper-case letter in bold are significant, those in lower-case
	''' and italicized are insignificant.
	''' <p>
	''' <a name="figure1"></a>
	''' <table border=0 summary="">
	''' <tr><td>
	'''     <img src="doc-files/UndoManager-1.gif" alt="">
	''' <tr><td align=center>Figure 1
	''' </table>
	''' <p>
	''' As shown in <a href="#figure1">figure 1</a>, if <b>D</b> was just added, the
	''' index of the next edit will be 4. Invoking <code>undo</code>
	''' results in invoking <code>undo</code> on <b>D</b> and setting the
	''' index of the next edit to 3 (edit <i>c</i>), as shown in the following
	''' figure.
	''' <p>
	''' <a name="figure2"></a>
	''' <table border=0 summary="">
	''' <tr><td>
	'''     <img src="doc-files/UndoManager-2.gif" alt="">
	''' <tr><td align=center>Figure 2
	''' </table>
	''' <p>
	''' The last significant edit is <b>A</b>, so that invoking
	''' <code>undo</code> again invokes <code>undo</code> on <i>c</i>,
	''' <i>b</i>, and <b>A</b>, in that order, setting the index of the
	''' next edit to 0, as shown in the following figure.
	''' <p>
	''' <a name="figure3"></a>
	''' <table border=0 summary="">
	''' <tr><td>
	'''     <img src="doc-files/UndoManager-3.gif" alt="">
	''' <tr><td align=center>Figure 3
	''' </table>
	''' <p>
	''' Invoking <code>redo</code> results in invoking <code>redo</code> on
	''' all edits between the index of the next edit and the next
	''' significant edit (or the end of the list).  Continuing with the previous
	''' example if <code>redo</code> were invoked, <code>redo</code> would in
	''' turn be invoked on <b>A</b>, <i>b</i> and <i>c</i>.  In addition
	''' the index of the next edit is set to 3 (as shown in <a
	''' href="#figure2">figure 2</a>).
	''' <p>
	''' Adding an edit to an <code>UndoManager</code> results in
	''' removing all edits from the index of the next edit to the end of
	''' the list.  Continuing with the previous example, if a new edit,
	''' <i>e</i>, is added the edit <b>D</b> is removed from the list
	''' (after having <code>die</code> invoked on it).  If <i>c</i> is not
	''' incorporated by the next edit
	''' (<code><i>c</i>.addEdit(<i>e</i>)</code> returns true), or replaced
	''' by it (<code><i>e</i>.replaceEdit(<i>c</i>)</code> returns true),
	''' the new edit is added after <i>c</i>, as shown in the following
	''' figure.
	''' <p>
	''' <a name="figure4"></a>
	''' <table border=0 summary="">
	''' <tr><td>
	'''     <img src="doc-files/UndoManager-4.gif" alt="">
	''' <tr><td align=center>Figure 4
	''' </table>
	''' <p>
	''' Once <code>end</code> has been invoked on an <code>UndoManager</code>
	''' the superclass behavior is used for all <code>UndoableEdit</code>
	''' methods.  Refer to <code>CompoundEdit</code> for more details on its
	''' behavior.
	''' <p>
	''' Unlike the rest of Swing, this class is thread safe.
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
	''' @author Ray Ryan
	''' </summary>
	Public Class UndoManager
		Inherits CompoundEdit
		Implements UndoableEditListener

		Friend indexOfNextAdd As Integer
		Friend limit As Integer

		''' <summary>
		''' Creates a new <code>UndoManager</code>.
		''' </summary>
		Public Sub New()
			MyBase.New()
			indexOfNextAdd = 0
			limit = 100
			edits.Capacity = limit
		End Sub

		''' <summary>
		''' Returns the maximum number of edits this {@code UndoManager}
		''' holds. A value less than 0 indicates the number of edits is not
		''' limited.
		''' </summary>
		''' <returns> the maximum number of edits this {@code UndoManager} holds </returns>
		''' <seealso cref= #addEdit </seealso>
		''' <seealso cref= #setLimit </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function getLimit() As Integer 'JavaToDotNetTempPropertyGetlimit
		Public Overridable Property limit As Integer
			Get
				Return limit
			End Get
			Set(ByVal l As Integer)
		End Property

		''' <summary>
		''' Empties the undo manager sending each edit a <code>die</code> message
		''' in the process.
		''' </summary>
		''' <seealso cref= AbstractUndoableEdit#die </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub discardAllEdits()
			For Each e As UndoableEdit In edits
				e.die()
			Next e
			edits = New List(Of UndoableEdit)
			indexOfNextAdd = 0
			' PENDING(rjrjr) when vector grows a removeRange() method
			' (expected in JDK 1.2), trimEdits() will be nice and
			' efficient, and this method can call that instead.
		End Sub

		''' <summary>
		''' Reduces the number of queued edits to a range of size limit,
		''' centered on the index of the next edit.
		''' </summary>
		Protected Friend Overridable Sub trimForLimit()
			If limit >= 0 Then
				Dim size As Integer = edits.Count
	'          System.out.print("limit: " + limit +
	'                           " size: " + size +
	'                           " indexOfNextAdd: " + indexOfNextAdd +
	'                           "\n");

				If size > limit Then
					Dim halfLimit As Integer = limit\2
					Dim keepFrom As Integer = indexOfNextAdd - 1 - halfLimit
					Dim keepTo As Integer = indexOfNextAdd - 1 + halfLimit

					' These are ints we're playing with, so dividing by two
					' rounds down for odd numbers, so make sure the limit was
					' honored properly. Note that the keep range is
					' inclusive.

					If keepTo - keepFrom + 1 > limit Then keepFrom += 1

					' The keep range is centered on indexOfNextAdd,
					' but odds are good that the actual edits Vector
					' isn't. Move the keep range to keep it legal.

					If keepFrom < 0 Then
						keepTo -= keepFrom
						keepFrom = 0
					End If
					If keepTo >= size Then
						Dim delta As Integer = size - keepTo - 1
						keepTo += delta
						keepFrom += delta
					End If

	'              System.out.println("Keeping " + keepFrom + " " + keepTo);
					trimEdits(keepTo+1, size-1)
					trimEdits(0, keepFrom-1)
				End If
			End If
		End Sub

		''' <summary>
		''' Removes edits in the specified range.
		''' All edits in the given range (inclusive, and in reverse order)
		''' will have <code>die</code> invoked on them and are removed from
		''' the list of edits. This has no effect if
		''' <code>from</code> &gt; <code>to</code>.
		''' </summary>
		''' <param name="from"> the minimum index to remove </param>
		''' <param name="to"> the maximum index to remove </param>
		Protected Friend Overridable Sub trimEdits(ByVal [from] As Integer, ByVal [to] As Integer)
			If [from] <= [to] Then
	'          System.out.println("Trimming " + from + " " + to + " with index " +
	'                           indexOfNextAdd);
				Dim i As Integer = [to]
				Do While [from] <= i
					Dim e As UndoableEdit = edits(i)
	'              System.out.println("JUM: Discarding " +
	'                                 e.getUndoPresentationName());
					e.die()
					' PENDING(rjrjr) when Vector supports range deletion (JDK
					' 1.2) , we can optimize the next line considerably.
					edits.RemoveAt(i)
					i -= 1
				Loop

				If indexOfNextAdd > [to] Then
	'              System.out.print("...right...");
					indexOfNextAdd -= [to]-[from]+1
				ElseIf indexOfNextAdd >= [from] Then
	'              System.out.println("...mid...");
					indexOfNextAdd = [from]
				End If

	'          System.out.println("new index " + indexOfNextAdd);
			End If
		End Sub

			If Not inProgress Then Throw New Exception("Attempt to call UndoManager.setLimit() after UndoManager.end() has been called")
			limit = l
			trimForLimit()
		End Sub


		''' <summary>
		''' Returns the the next significant edit to be undone if <code>undo</code>
		''' is invoked. This returns <code>null</code> if there are no edits
		''' to be undone.
		''' </summary>
		''' <returns> the next significant edit to be undone </returns>
		Protected Friend Overridable Function editToBeUndone() As UndoableEdit
			Dim i As Integer = indexOfNextAdd
			Do While i > 0
				i -= 1
				Dim edit As UndoableEdit = edits(i)
				If edit.significant Then Return edit
			Loop

			Return Nothing
		End Function

		''' <summary>
		''' Returns the the next significant edit to be redone if <code>redo</code>
		''' is invoked. This returns <code>null</code> if there are no edits
		''' to be redone.
		''' </summary>
		''' <returns> the next significant edit to be redone </returns>
		Protected Friend Overridable Function editToBeRedone() As UndoableEdit
			Dim count As Integer = edits.Count
			Dim i As Integer = indexOfNextAdd

			Do While i < count
				Dim edit As UndoableEdit = edits(i)
				i += 1
				If edit.significant Then Return edit
			Loop

			Return Nothing
		End Function

		''' <summary>
		''' Undoes all changes from the index of the next edit to
		''' <code>edit</code>, updating the index of the next edit appropriately.
		''' </summary>
		''' <exception cref="CannotUndoException"> if one of the edits throws
		'''         <code>CannotUndoException</code> </exception>
		Protected Friend Overridable Sub undoTo(ByVal edit As UndoableEdit)
			Dim done As Boolean = False
			Do While Not done
				indexOfNextAdd -= 1
				Dim [next] As UndoableEdit = edits(indexOfNextAdd)
				[next].undo()
				done = [next] Is edit
			Loop
		End Sub

		''' <summary>
		''' Redoes all changes from the index of the next edit to
		''' <code>edit</code>, updating the index of the next edit appropriately.
		''' </summary>
		''' <exception cref="CannotRedoException"> if one of the edits throws
		'''         <code>CannotRedoException</code> </exception>
		Protected Friend Overridable Sub redoTo(ByVal edit As UndoableEdit)
			Dim done As Boolean = False
			Do While Not done
				Dim [next] As UndoableEdit = edits(indexOfNextAdd)
				indexOfNextAdd += 1
				[next].redo()
				done = [next] Is edit
			Loop
		End Sub

		''' <summary>
		''' Convenience method that invokes one of <code>undo</code> or
		''' <code>redo</code>. If any edits have been undone (the index of
		''' the next edit is less than the length of the edits list) this
		''' invokes <code>redo</code>, otherwise it invokes <code>undo</code>.
		''' </summary>
		''' <seealso cref= #canUndoOrRedo </seealso>
		''' <seealso cref= #getUndoOrRedoPresentationName </seealso>
		''' <exception cref="CannotUndoException"> if one of the edits throws
		'''         <code>CannotUndoException</code> </exception>
		''' <exception cref="CannotRedoException"> if one of the edits throws
		'''         <code>CannotRedoException</code> </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub undoOrRedo()
			If indexOfNextAdd = edits.Count Then
				undo()
			Else
				redo()
			End If
		End Sub

		''' <summary>
		''' Returns true if it is possible to invoke <code>undo</code> or
		''' <code>redo</code>.
		''' </summary>
		''' <returns> true if invoking <code>canUndoOrRedo</code> is valid </returns>
		''' <seealso cref= #undoOrRedo </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function canUndoOrRedo() As Boolean
			If indexOfNextAdd = edits.Count Then
				Return canUndo()
			Else
				Return canRedo()
			End If
		End Function

		''' <summary>
		''' Undoes the appropriate edits.  If <code>end</code> has been
		''' invoked this calls through to the superclass, otherwise
		''' this invokes <code>undo</code> on all edits between the
		''' index of the next edit and the last significant edit, updating
		''' the index of the next edit appropriately.
		''' </summary>
		''' <exception cref="CannotUndoException"> if one of the edits throws
		'''         <code>CannotUndoException</code> or there are no edits
		'''         to be undone </exception>
		''' <seealso cref= CompoundEdit#end </seealso>
		''' <seealso cref= #canUndo </seealso>
		''' <seealso cref= #editToBeUndone </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub undo()
			If inProgress Then
				Dim edit As UndoableEdit = editToBeUndone()
				If edit Is Nothing Then Throw New CannotUndoException
				undoTo(edit)
			Else
				MyBase.undo()
			End If
		End Sub

		''' <summary>
		''' Returns true if edits may be undone.  If <code>end</code> has
		''' been invoked, this returns the value from super.  Otherwise
		''' this returns true if there are any edits to be undone
		''' (<code>editToBeUndone</code> returns non-<code>null</code>).
		''' </summary>
		''' <returns> true if there are edits to be undone </returns>
		''' <seealso cref= CompoundEdit#canUndo </seealso>
		''' <seealso cref= #editToBeUndone </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function canUndo() As Boolean
			If inProgress Then
				Dim edit As UndoableEdit = editToBeUndone()
				Return edit IsNot Nothing AndAlso edit.canUndo()
			Else
				Return MyBase.canUndo()
			End If
		End Function

		''' <summary>
		''' Redoes the appropriate edits.  If <code>end</code> has been
		''' invoked this calls through to the superclass.  Otherwise
		''' this invokes <code>redo</code> on all edits between the
		''' index of the next edit and the next significant edit, updating
		''' the index of the next edit appropriately.
		''' </summary>
		''' <exception cref="CannotRedoException"> if one of the edits throws
		'''         <code>CannotRedoException</code> or there are no edits
		'''         to be redone </exception>
		''' <seealso cref= CompoundEdit#end </seealso>
		''' <seealso cref= #canRedo </seealso>
		''' <seealso cref= #editToBeRedone </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub redo()
			If inProgress Then
				Dim edit As UndoableEdit = editToBeRedone()
				If edit Is Nothing Then Throw New CannotRedoException
				redoTo(edit)
			Else
				MyBase.redo()
			End If
		End Sub

		''' <summary>
		''' Returns true if edits may be redone.  If <code>end</code> has
		''' been invoked, this returns the value from super.  Otherwise,
		''' this returns true if there are any edits to be redone
		''' (<code>editToBeRedone</code> returns non-<code>null</code>).
		''' </summary>
		''' <returns> true if there are edits to be redone </returns>
		''' <seealso cref= CompoundEdit#canRedo </seealso>
		''' <seealso cref= #editToBeRedone </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function canRedo() As Boolean
			If inProgress Then
				Dim edit As UndoableEdit = editToBeRedone()
				Return edit IsNot Nothing AndAlso edit.canRedo()
			Else
				Return MyBase.canRedo()
			End If
		End Function

		''' <summary>
		''' Adds an <code>UndoableEdit</code> to this
		''' <code>UndoManager</code>, if it's possible.  This removes all
		''' edits from the index of the next edit to the end of the edits
		''' list.  If <code>end</code> has been invoked the edit is not added
		''' and <code>false</code> is returned.  If <code>end</code> hasn't
		''' been invoked this returns <code>true</code>.
		''' </summary>
		''' <param name="anEdit"> the edit to be added </param>
		''' <returns> true if <code>anEdit</code> can be incorporated into this
		'''              edit </returns>
		''' <seealso cref= CompoundEdit#end </seealso>
		''' <seealso cref= CompoundEdit#addEdit </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function addEdit(ByVal anEdit As UndoableEdit) As Boolean
			Dim retVal As Boolean

			' Trim from the indexOfNextAdd to the end, as we'll
			' never reach these edits once the new one is added.
			trimEdits(indexOfNextAdd, edits.Count-1)

			retVal = MyBase.addEdit(anEdit)
			If inProgress Then retVal = True

			' Maybe super added this edit, maybe it didn't (perhaps
			' an in progress compound edit took it instead. Or perhaps
			' this UndoManager is no longer in progress). So make sure
			' the indexOfNextAdd is pointed at the right place.
			indexOfNextAdd = edits.Count

			' Enforce the limit
			trimForLimit()

			Return retVal
		End Function


		''' <summary>
		''' Turns this <code>UndoManager</code> into a normal
		''' <code>CompoundEdit</code>.  This removes all edits that have
		''' been undone.
		''' </summary>
		''' <seealso cref= CompoundEdit#end </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub [end]()
			MyBase.end()
			Me.trimEdits(indexOfNextAdd, edits.Count-1)
		End Sub

		''' <summary>
		''' Convenience method that returns either
		''' <code>getUndoPresentationName</code> or
		''' <code>getRedoPresentationName</code>.  If the index of the next
		''' edit equals the size of the edits list,
		''' <code>getUndoPresentationName</code> is returned, otherwise
		''' <code>getRedoPresentationName</code> is returned.
		''' </summary>
		''' <returns> undo or redo name </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property undoOrRedoPresentationName As String
			Get
				If indexOfNextAdd = edits.Count Then
					Return undoPresentationName
				Else
					Return redoPresentationName
				End If
			End Get
		End Property

		''' <summary>
		''' Returns a description of the undoable form of this edit.
		''' If <code>end</code> has been invoked this calls into super.
		''' Otherwise if there are edits to be undone, this returns
		''' the value from the next significant edit that will be undone.
		''' If there are no edits to be undone and <code>end</code> has not
		''' been invoked this returns the value from the <code>UIManager</code>
		''' property "AbstractUndoableEdit.undoText".
		''' </summary>
		''' <returns> a description of the undoable form of this edit </returns>
		''' <seealso cref=     #undo </seealso>
		''' <seealso cref=     CompoundEdit#getUndoPresentationName </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Property Overrides undoPresentationName As String
			Get
				If inProgress Then
					If canUndo() Then
						Return editToBeUndone().undoPresentationName
					Else
						Return javax.swing.UIManager.getString("AbstractUndoableEdit.undoText")
					End If
				Else
					Return MyBase.undoPresentationName
				End If
			End Get
		End Property

		''' <summary>
		''' Returns a description of the redoable form of this edit.
		''' If <code>end</code> has been invoked this calls into super.
		''' Otherwise if there are edits to be redone, this returns
		''' the value from the next significant edit that will be redone.
		''' If there are no edits to be redone and <code>end</code> has not
		''' been invoked this returns the value from the <code>UIManager</code>
		''' property "AbstractUndoableEdit.redoText".
		''' </summary>
		''' <returns> a description of the redoable form of this edit </returns>
		''' <seealso cref=     #redo </seealso>
		''' <seealso cref=     CompoundEdit#getRedoPresentationName </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Property Overrides redoPresentationName As String
			Get
				If inProgress Then
					If canRedo() Then
						Return editToBeRedone().redoPresentationName
					Else
						Return javax.swing.UIManager.getString("AbstractUndoableEdit.redoText")
					End If
				Else
					Return MyBase.redoPresentationName
				End If
			End Get
		End Property

		''' <summary>
		''' An <code>UndoableEditListener</code> method. This invokes
		''' <code>addEdit</code> with <code>e.getEdit()</code>.
		''' </summary>
		''' <param name="e"> the <code>UndoableEditEvent</code> the
		'''        <code>UndoableEditEvent</code> will be added from </param>
		''' <seealso cref= #addEdit </seealso>
		Public Overridable Sub undoableEditHappened(ByVal e As UndoableEditEvent) Implements UndoableEditListener.undoableEditHappened
			addEdit(e.edit)
		End Sub

		''' <summary>
		''' Returns a string that displays and identifies this
		''' object's properties.
		''' </summary>
		''' <returns> a String representation of this object </returns>
		Public Overrides Function ToString() As String
			Return MyBase.ToString() & " limit: " & limit & " indexOfNextAdd: " & indexOfNextAdd
		End Function
	End Class

End Namespace
Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Collections.Generic
Imports javax.swing.event

'
' * Copyright (c) 1997, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' A support class used for managing <code>UndoableEdit</code> listeners.
	''' 
	''' @author Ray Ryan
	''' </summary>
	Public Class UndoableEditSupport
		Protected Friend updateLevel As Integer
		Protected Friend compoundEdit As CompoundEdit
		Protected Friend listeners As List(Of UndoableEditListener)
		Protected Friend realSource As Object

		''' <summary>
		''' Constructs an <code>UndoableEditSupport</code> object.
		''' </summary>
		Public Sub New()
			Me.New(Nothing)
		End Sub

		''' <summary>
		''' Constructs an <code>UndoableEditSupport</code> object.
		''' </summary>
		''' <param name="r">  an <code>Object</code> </param>
		Public Sub New(ByVal r As Object)
			realSource = If(r Is Nothing, Me, r)
			updateLevel = 0
			compoundEdit = Nothing
			listeners = New List(Of UndoableEditListener)
		End Sub

		''' <summary>
		''' Registers an <code>UndoableEditListener</code>.
		''' The listener is notified whenever an edit occurs which can be undone.
		''' </summary>
		''' <param name="l">  an <code>UndoableEditListener</code> object </param>
		''' <seealso cref= #removeUndoableEditListener </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addUndoableEditListener(ByVal l As UndoableEditListener)
			listeners.Add(l)
		End Sub

		''' <summary>
		''' Removes an <code>UndoableEditListener</code>.
		''' </summary>
		''' <param name="l">  the <code>UndoableEditListener</code> object to be removed </param>
		''' <seealso cref= #addUndoableEditListener </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeUndoableEditListener(ByVal l As UndoableEditListener)
			listeners.Remove(l)
		End Sub

		''' <summary>
		''' Returns an array of all the <code>UndoableEditListener</code>s added
		''' to this UndoableEditSupport with addUndoableEditListener().
		''' </summary>
		''' <returns> all of the <code>UndoableEditListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property undoableEditListeners As UndoableEditListener()
			Get
				Return listeners.ToArray()
			End Get
		End Property

		''' <summary>
		''' Called only from <code>postEdit</code> and <code>endUpdate</code>. Calls
		''' <code>undoableEditHappened</code> in all listeners. No synchronization
		''' is performed here, since the two calling methods are synchronized.
		''' </summary>
		Protected Friend Overridable Sub _postEdit(ByVal e As UndoableEdit)
			Dim ev As New UndoableEditEvent(realSource, e)
			Dim cursor As System.Collections.IEnumerator = CType(listeners.clone(), ArrayList).elements()
			Do While cursor.hasMoreElements()
				CType(cursor.nextElement(), UndoableEditListener).undoableEditHappened(ev)
			Loop
		End Sub

		''' <summary>
		''' DEADLOCK WARNING: Calling this method may call
		''' <code>undoableEditHappened</code> in all listeners.
		''' It is unwise to call this method from one of its listeners.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub postEdit(ByVal e As UndoableEdit)
			If updateLevel = 0 Then
				_postEdit(e)
			Else
				' PENDING(rjrjr) Throw an exception if this fails?
				compoundEdit.addEdit(e)
			End If
		End Sub

		''' <summary>
		''' Returns the update level value.
		''' </summary>
		''' <returns> an integer representing the update level </returns>
		Public Overridable Property updateLevel As Integer
			Get
				Return updateLevel
			End Get
		End Property

		''' 
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub beginUpdate()
			If updateLevel = 0 Then compoundEdit = createCompoundEdit()
			updateLevel += 1
		End Sub

		''' <summary>
		''' Called only from <code>beginUpdate</code>.
		''' Exposed here for subclasses' use.
		''' </summary>
		Protected Friend Overridable Function createCompoundEdit() As CompoundEdit
			Return New CompoundEdit
		End Function

		''' <summary>
		''' DEADLOCK WARNING: Calling this method may call
		''' <code>undoableEditHappened</code> in all listeners.
		''' It is unwise to call this method from one of its listeners.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub endUpdate()
			updateLevel -= 1
			If updateLevel = 0 Then
				compoundEdit.end()
				_postEdit(compoundEdit)
				compoundEdit = Nothing
			End If
		End Sub

		''' <summary>
		''' Returns a string that displays and identifies this
		''' object's properties.
		''' </summary>
		''' <returns> a <code>String</code> representation of this object </returns>
		Public Overrides Function ToString() As String
			Return MyBase.ToString() & " updateLevel: " & updateLevel & " listeners: " & listeners & " compoundEdit: " & compoundEdit
		End Function
	End Class

End Namespace
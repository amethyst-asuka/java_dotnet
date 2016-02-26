Imports System.Collections
Imports System.Collections.Generic

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
	''' <P>StateEdit is a general edit for objects that change state.
	''' Objects being edited must conform to the StateEditable interface.</P>
	''' 
	''' <P>This edit class works by asking an object to store it's state in
	''' Hashtables before and after editing occurs.  Upon undo or redo the
	''' object is told to restore it's state from these Hashtables.</P>
	''' 
	''' A state edit is used as follows:
	''' <PRE>
	'''      // Create the edit during the "before" state of the object
	'''      StateEdit newEdit = new StateEdit(myObject);
	'''      // Modify the object
	'''      myObject.someStateModifyingMethod();
	'''      // "end" the edit when you are done modifying the object
	'''      newEdit.end();
	''' </PRE>
	''' 
	''' <P><EM>Note that when a StateEdit ends, it removes redundant state from
	''' the Hashtables - A state Hashtable is not guaranteed to contain all
	''' keys/values placed into it when the state is stored!</EM></P>
	''' </summary>
	''' <seealso cref= StateEditable
	''' 
	''' @author Ray Ryan </seealso>

	Public Class StateEdit
		Inherits AbstractUndoableEdit

		Protected Friend Const RCSID As String = "$Id: StateEdit.java,v 1.6 1997/10/01 20:05:51 sandipc Exp $"

		'
		' Attributes
		'

		''' <summary>
		''' The object being edited
		''' </summary>
		Protected Friend [object] As StateEditable

		''' <summary>
		''' The state information prior to the edit
		''' </summary>
		Protected Friend preState As Dictionary(Of Object, Object)

		''' <summary>
		''' The state information after the edit
		''' </summary>
		Protected Friend postState As Dictionary(Of Object, Object)

		''' <summary>
		''' The undo/redo presentation name
		''' </summary>
		Protected Friend undoRedoName As String

		'
		' Constructors
		'

		''' <summary>
		''' Create and return a new StateEdit.
		''' </summary>
		''' <param name="anObject"> The object to watch for changing state
		''' </param>
		''' <seealso cref= StateEdit </seealso>
		Public Sub New(ByVal anObject As StateEditable)
			MyBase.New()
			init(anObject,Nothing)
		End Sub

		''' <summary>
		''' Create and return a new StateEdit with a presentation name.
		''' </summary>
		''' <param name="anObject"> The object to watch for changing state </param>
		''' <param name="name"> The presentation name to be used for this edit
		''' </param>
		''' <seealso cref= StateEdit </seealso>
		Public Sub New(ByVal anObject As StateEditable, ByVal name As String)
			MyBase.New()
			init(anObject,name)
		End Sub

		Protected Friend Overridable Sub init(ByVal anObject As StateEditable, ByVal name As String)
			Me.object = anObject
			Me.preState = New Dictionary(Of Object, Object)(11)
			Me.object.storeState(Me.preState)
			Me.postState = Nothing
			Me.undoRedoName = name
		End Sub


		'
		' Operation
		'


		''' <summary>
		''' Gets the post-edit state of the StateEditable object and
		''' ends the edit.
		''' </summary>
		Public Overridable Sub [end]()
			Me.postState = New Dictionary(Of Object, Object)(11)
			Me.object.storeState(Me.postState)
			Me.removeRedundantState()
		End Sub

		''' <summary>
		''' Tells the edited object to apply the state prior to the edit
		''' </summary>
		Public Overrides Sub undo()
			MyBase.undo()
			Me.object.restoreState(preState)
		End Sub

		''' <summary>
		''' Tells the edited object to apply the state after the edit
		''' </summary>
		Public Overrides Sub redo()
			MyBase.redo()
			Me.object.restoreState(postState)
		End Sub

		''' <summary>
		''' Gets the presentation name for this edit
		''' </summary>
		Public Property Overrides presentationName As String
			Get
				Return Me.undoRedoName
			End Get
		End Property


		'
		' Internal support
		'

		''' <summary>
		''' Remove redundant key/values in state hashtables.
		''' </summary>
		Protected Friend Overridable Sub removeRedundantState()
			Dim uselessKeys As New List(Of Object)
			Dim myKeys As System.Collections.IEnumerator = preState.Keys.GetEnumerator()

			' Locate redundant state
			Do While myKeys.hasMoreElements()
				Dim myKey As Object = myKeys.nextElement()
				If postState.ContainsKey(myKey) AndAlso postState(myKey).Equals(preState(myKey)) Then uselessKeys.Add(myKey)
			Loop

			' Remove redundant state
			For i As Integer = uselessKeys.Count-1 To 0 Step -1
				Dim myKey As Object = uselessKeys(i)
				preState.Remove(myKey)
				postState.Remove(myKey)
			Next i
		End Sub

	End Class ' End of class StateEdit

End Namespace
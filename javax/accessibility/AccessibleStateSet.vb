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

Namespace javax.accessibility


	''' <summary>
	''' Class AccessibleStateSet determines a component's state set.  The state set
	''' of a component is a set of AccessibleState objects and descriptions. E.G., The
	''' current overall state of the object, such as whether it is enabled,
	''' has focus, etc.
	''' </summary>
	''' <seealso cref= AccessibleState
	''' 
	''' @author      Willie Walker </seealso>
	Public Class AccessibleStateSet

		''' <summary>
		''' Each entry in the Vector represents an AccessibleState. </summary>
		''' <seealso cref= #add </seealso>
		''' <seealso cref= #addAll </seealso>
		''' <seealso cref= #remove </seealso>
		''' <seealso cref= #contains </seealso>
		''' <seealso cref= #toArray </seealso>
		''' <seealso cref= #clear </seealso>
		Protected Friend states As List(Of AccessibleState) = Nothing

		''' <summary>
		''' Creates a new empty state set.
		''' </summary>
		Public Sub New()
			states = Nothing
		End Sub

		''' <summary>
		''' Creates a new state with the initial set of states contained in
		''' the array of states passed in.  Duplicate entries are ignored.
		''' </summary>
		''' <param name="states"> an array of AccessibleState describing the state set. </param>
		Public Sub New(ByVal states As AccessibleState())
			If states.Length <> 0 Then
				Me.states = New ArrayList(states.Length)
				For i As Integer = 0 To states.Length - 1
					If Not Me.states.Contains(states(i)) Then Me.states.Add(states(i))
				Next i
			End If
		End Sub

		''' <summary>
		''' Adds a new state to the current state set if it is not already
		''' present.  If the state is already in the state set, the state
		''' set is unchanged and the return value is false.  Otherwise,
		''' the state is added to the state set and the return value is
		''' true. </summary>
		''' <param name="state"> the state to add to the state set </param>
		''' <returns> true if state is added to the state set; false if the state set
		''' is unchanged </returns>
		Public Overridable Function add(ByVal state As AccessibleState) As Boolean
			' [[[ PENDING:  WDW - the implementation of this does not need
			' to always use a vector of states.  It could be improved by
			' caching the states as a bit set.]]]
			If states Is Nothing Then states = New ArrayList

			If Not states.Contains(state) Then
				states.Add(state)
				Return True
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Adds all of the states to the existing state set.  Duplicate entries
		''' are ignored. </summary>
		''' <param name="states">  AccessibleState array describing the state set. </param>
		Public Overridable Sub addAll(ByVal states As AccessibleState())
			If states.Length <> 0 Then
				If Me.states Is Nothing Then Me.states = New ArrayList(states.Length)
				For i As Integer = 0 To states.Length - 1
					If Not Me.states.Contains(states(i)) Then Me.states.Add(states(i))
				Next i
			End If
		End Sub

		''' <summary>
		''' Removes a state from the current state set.  If the state is not
		''' in the set, the state set will be unchanged and the return value
		''' will be false.  If the state is in the state set, it will be removed
		''' from the set and the return value will be true.
		''' </summary>
		''' <param name="state"> the state to remove from the state set </param>
		''' <returns> true if the state is in the state set; false if the state set
		''' will be unchanged </returns>
		Public Overridable Function remove(ByVal state As AccessibleState) As Boolean
			If states Is Nothing Then
				Return False
			Else
				Return states.Remove(state)
			End If
		End Function

		''' <summary>
		''' Removes all the states from the current state set.
		''' </summary>
		Public Overridable Sub clear()
			If states IsNot Nothing Then states.Clear()
		End Sub

		''' <summary>
		''' Checks if the current state is in the state set. </summary>
		''' <param name="state"> the state </param>
		''' <returns> true if the state is in the state set; otherwise false </returns>
		Public Overridable Function contains(ByVal state As AccessibleState) As Boolean
			If states Is Nothing Then
				Return False
			Else
				Return states.Contains(state)
			End If
		End Function

		''' <summary>
		''' Returns the current state set as an array of AccessibleState </summary>
		''' <returns> AccessibleState array containing the current state. </returns>
		Public Overridable Function toArray() As AccessibleState()
			If states Is Nothing Then
				Return New AccessibleState(){}
			Else
				Dim stateArray As AccessibleState() = New AccessibleState(states.Count - 1){}
				For i As Integer = 0 To stateArray.Length - 1
					stateArray(i) = CType(states(i), AccessibleState)
				Next i
				Return stateArray
			End If
		End Function

		''' <summary>
		''' Creates a localized String representing all the states in the set
		''' using the default locale.
		''' </summary>
		''' <returns> comma separated localized String </returns>
		''' <seealso cref= AccessibleBundle#toDisplayString </seealso>
		Public Overrides Function ToString() As String
			Dim ret As String = Nothing
			If (states IsNot Nothing) AndAlso (states.Count > 0) Then
				ret = CType(states(0), AccessibleState).toDisplayString()
				For i As Integer = 1 To states.Count - 1
					ret = ret & "," & CType(states(i), AccessibleState).toDisplayString()
				Next i
			End If
			Return ret
		End Function
	End Class

End Namespace
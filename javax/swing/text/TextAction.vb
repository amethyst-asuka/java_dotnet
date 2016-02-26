Imports System.Collections.Generic

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
	''' An Action implementation useful for key bindings that are
	''' shared across a number of different text components.  Because
	''' the action is shared, it must have a way of getting it's
	''' target to act upon.  This class provides support to try and
	''' find a text component to operate on.  The preferred way of
	''' getting the component to act upon is through the ActionEvent
	''' that is received.  If the Object returned by getSource can
	''' be narrowed to a text component, it will be used.  If the
	''' action event is null or can't be narrowed, the last focused
	''' text component is tried.  This is determined by being
	''' used in conjunction with a JTextController which
	''' arranges to share that information with a TextAction.
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
	Public MustInherit Class TextAction
		Inherits javax.swing.AbstractAction

		''' <summary>
		''' Creates a new JTextAction object.
		''' </summary>
		''' <param name="name"> the name of the action </param>
		Public Sub New(ByVal name As String)
			MyBase.New(name)
		End Sub

		''' <summary>
		''' Determines the component to use for the action.
		''' This if fetched from the source of the ActionEvent
		''' if it's not null and can be narrowed.  Otherwise,
		''' the last focused component is used.
		''' </summary>
		''' <param name="e"> the ActionEvent </param>
		''' <returns> the component </returns>
		Protected Friend Function getTextComponent(ByVal e As java.awt.event.ActionEvent) As JTextComponent
			If e IsNot Nothing Then
				Dim o As Object = e.source
				If TypeOf o Is JTextComponent Then Return CType(o, JTextComponent)
			End If
			Return focusedComponent
		End Function

		''' <summary>
		''' Takes one list of
		''' commands and augments it with another list
		''' of commands.  The second list takes precedence
		''' over the first list; that is, when both lists
		''' contain a command with the same name, the command
		''' from the second list is used.
		''' </summary>
		''' <param name="list1"> the first list, may be empty but not
		'''              <code>null</code> </param>
		''' <param name="list2"> the second list, may be empty but not
		'''              <code>null</code> </param>
		''' <returns> the augmented list </returns>
		Public Shared Function augmentList(ByVal list1 As javax.swing.Action(), ByVal list2 As javax.swing.Action()) As javax.swing.Action()
			Dim h As New Dictionary(Of String, javax.swing.Action)
			For Each a As javax.swing.Action In list1
				Dim ___value As String = CStr(a.getValue(javax.swing.Action.NAME))
				h((If(___value IsNot Nothing, ___value, ""))) = a
			Next a
			For Each a As javax.swing.Action In list2
				Dim ___value As String = CStr(a.getValue(javax.swing.Action.NAME))
				h((If(___value IsNot Nothing, ___value, ""))) = a
			Next a
			Dim actions As javax.swing.Action() = New javax.swing.Action(h.Count - 1){}
			Dim index As Integer = 0
			Dim e As System.Collections.IEnumerator = h.Values.GetEnumerator()
			Do While e.hasMoreElements()
				actions(index) = CType(e.nextElement(), javax.swing.Action)
				index += 1
			Loop
			Return actions
		End Function

		''' <summary>
		''' Fetches the text component that currently has focus.
		''' This allows actions to be shared across text components
		''' which is useful for key-bindings where a large set of
		''' actions are defined, but generally used the same way
		''' across many different components.
		''' </summary>
		''' <returns> the component </returns>
		Protected Friend Property focusedComponent As JTextComponent
			Get
				Return JTextComponent.focusedComponent
			End Get
		End Property
	End Class

End Namespace
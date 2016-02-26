Imports System

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

Namespace javax.swing.tree


	''' <summary>
	''' Exception used to stop and expand/collapse from happening.
	''' See <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/events/treewillexpandlistener.html">How to Write a Tree-Will-Expand Listener</a>
	''' in <em>The Java Tutorial</em>
	''' for further information and examples.
	''' 
	''' @author Scott Violet
	''' </summary>
	Public Class ExpandVetoException
		Inherits Exception

		''' <summary>
		''' The event that the exception was created for. </summary>
		Protected Friend [event] As javax.swing.event.TreeExpansionEvent

		''' <summary>
		''' Constructs an ExpandVetoException object with no message.
		''' </summary>
		''' <param name="event">  a TreeExpansionEvent object </param>

		Public Sub New(ByVal [event] As javax.swing.event.TreeExpansionEvent)
			Me.New([event], Nothing)
		End Sub

		''' <summary>
		''' Constructs an ExpandVetoException object with the specified message.
		''' </summary>
		''' <param name="event">    a TreeExpansionEvent object </param>
		''' <param name="message">  a String containing the message </param>
		Public Sub New(ByVal [event] As javax.swing.event.TreeExpansionEvent, ByVal message As String)
			MyBase.New(message)
			Me.event = [event]
		End Sub
	End Class

End Namespace
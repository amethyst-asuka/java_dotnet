Imports System

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.security.auth.callback

	''' <summary>
	''' Signals that a {@code CallbackHandler} does not
	''' recognize a particular {@code Callback}.
	''' 
	''' </summary>
	Public Class UnsupportedCallbackException
		Inherits Exception

		Private Const serialVersionUID As Long = -6873556327655666839L

		''' <summary>
		''' @serial
		''' </summary>
		Private callback As Callback

		''' <summary>
		''' Constructs a {@code UnsupportedCallbackException}
		''' with no detail message.
		''' 
		''' <p>
		''' </summary>
		''' <param name="callback"> the unrecognized {@code Callback}. </param>
		Public Sub New(ByVal callback As Callback)
			MyBase.New()
			Me.callback = callback
		End Sub

		''' <summary>
		''' Constructs a UnsupportedCallbackException with the specified detail
		''' message.  A detail message is a String that describes this particular
		''' exception.
		''' 
		''' <p>
		''' </summary>
		''' <param name="callback"> the unrecognized {@code Callback}. <p>
		''' </param>
		''' <param name="msg"> the detail message. </param>
		Public Sub New(ByVal callback As Callback, ByVal msg As String)
			MyBase.New(msg)
			Me.callback = callback
		End Sub

		''' <summary>
		''' Get the unrecognized {@code Callback}.
		''' 
		''' <p>
		''' </summary>
		''' <returns> the unrecognized {@code Callback}. </returns>
		Public Overridable Property callback As Callback
			Get
				Return callback
			End Get
		End Property
	End Class

End Namespace
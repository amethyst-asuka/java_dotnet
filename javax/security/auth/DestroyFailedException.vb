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

Namespace javax.security.auth

	''' <summary>
	''' Signals that a {@code destroy} operation failed.
	''' 
	''' <p> This exception is thrown by credentials implementing
	''' the {@code Destroyable} interface when the {@code destroy}
	''' method fails.
	''' 
	''' </summary>
	Public Class DestroyFailedException
		Inherits Exception

		Private Const serialVersionUID As Long = -7790152857282749162L

		''' <summary>
		''' Constructs a DestroyFailedException with no detail message. A detail
		''' message is a String that describes this particular exception.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a DestroyFailedException with the specified detail
		''' message.  A detail message is a String that describes this particular
		''' exception.
		''' 
		''' <p>
		''' </summary>
		''' <param name="msg"> the detail message. </param>
		Public Sub New(ByVal msg As String)
			MyBase.New(msg)
		End Sub
	End Class

End Namespace
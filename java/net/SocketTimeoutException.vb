'
' * Copyright (c) 2000, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.net

	''' <summary>
	''' Signals that a timeout has occurred on a socket read or accept.
	''' 
	''' @since   1.4
	''' </summary>

	Public Class SocketTimeoutException
		Inherits java.io.InterruptedIOException

		Private Shadows Const serialVersionUID As Long = -8846654841826352300L

		''' <summary>
		''' Constructs a new SocketTimeoutException with a detail
		''' message. </summary>
		''' <param name="msg"> the detail message </param>
		Public Sub New(  msg As String)
			MyBase.New(msg)
		End Sub

		''' <summary>
		''' Construct a new SocketTimeoutException with no detailed message.
		''' </summary>
		Public Sub New()
		End Sub
	End Class

End Namespace
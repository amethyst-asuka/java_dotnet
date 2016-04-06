'
' * Copyright (c) 1996, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' Signals that an error occurred while attempting to connect a
	''' socket to a remote address and port.  Typically, the connection
	''' was refused remotely (e.g., no process is listening on the
	''' remote address/port).
	''' 
	''' @since   JDK1.1
	''' </summary>
	Public Class ConnectException
		Inherits SocketException

		Private Shadows Const serialVersionUID As Long = 3831404271622369215L

		''' <summary>
		''' Constructs a new ConnectException with the specified detail
		''' message as to why the connect error occurred.
		''' A detail message is a String that gives a specific
		''' description of this error. </summary>
		''' <param name="msg"> the detail message </param>
		Public Sub New(  msg As String)
			MyBase.New(msg)
		End Sub

		''' <summary>
		''' Construct a new ConnectException with no detailed message.
		''' </summary>
		Public Sub New()
		End Sub
	End Class

End Namespace
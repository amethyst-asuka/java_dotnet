'
' * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Signals that an ICMP Port Unreachable message has been
	''' received on a connected datagram.
	''' 
	''' @since   1.4
	''' </summary>

	Public Class PortUnreachableException
		Inherits SocketException

		Private Shadows Const serialVersionUID As Long = 8462541992376507323L

		''' <summary>
		''' Constructs a new {@code PortUnreachableException} with a
		''' detail message. </summary>
		''' <param name="msg"> the detail message </param>
		Public Sub New(ByVal msg As String)
			MyBase.New(msg)
		End Sub

		''' <summary>
		''' Construct a new {@code PortUnreachableException} with no
		''' detailed message.
		''' </summary>
		Public Sub New()
		End Sub
	End Class

End Namespace
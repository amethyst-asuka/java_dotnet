Imports System

'
' * Copyright (c) 1996, 1998, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.rmi.server

	''' <summary>
	''' An <code>ServerNotActiveException</code> is an <code>Exception</code>
	''' thrown during a call to <code>RemoteServer.getClientHost</code> if
	''' the getClientHost method is called outside of servicing a remote
	''' method call.
	''' 
	''' @author  Roger Riggs
	''' @since   JDK1.1 </summary>
	''' <seealso cref= java.rmi.server.RemoteServer#getClientHost() </seealso>
	Public Class ServerNotActiveException
		Inherits Exception

		' indicate compatibility with JDK 1.1.x version of class 
		Private Shadows Const serialVersionUID As Long = 4687940720827538231L

		''' <summary>
		''' Constructs an <code>ServerNotActiveException</code> with no specified
		''' detail message.
		''' @since JDK1.1
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs an <code>ServerNotActiveException</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="s"> the detail message.
		''' @since JDK1.1 </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace
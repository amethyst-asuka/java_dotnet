Imports System

'
' * Copyright (c) 1996, 2003, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.rmi

	''' <summary>
	''' A <code>ServerException</code> is thrown as a result of a remote method
	''' invocation when a <code>RemoteException</code> is thrown while processing
	''' the invocation on the server, either while unmarshalling the arguments or
	''' executing the remote method itself.
	''' 
	''' A <code>ServerException</code> instance contains the original
	''' <code>RemoteException</code> that occurred as its cause.
	''' 
	''' @author  Ann Wollrath
	''' @since   JDK1.1
	''' </summary>
	Public Class ServerException
		Inherits RemoteException

		' indicate compatibility with JDK 1.1.x version of class 
		Private Shadows Const serialVersionUID As Long = -4775845313121906682L

		''' <summary>
		''' Constructs a <code>ServerException</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="s"> the detail message
		''' @since JDK1.1 </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub

		''' <summary>
		''' Constructs a <code>ServerException</code> with the specified
		''' detail message and nested exception.
		''' </summary>
		''' <param name="s"> the detail message </param>
		''' <param name="ex"> the nested exception
		''' @since JDK1.1 </param>
		Public Sub New(ByVal s As String, ByVal ex As Exception)
			MyBase.New(s, ex)
		End Sub
	End Class

End Namespace
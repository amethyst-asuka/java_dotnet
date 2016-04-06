'
' * Copyright (c) 1996, 2001, Oracle and/or its affiliates. All rights reserved.
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
	''' A <code>ServerError</code> is thrown as a result of a remote method
	''' invocation when an <code>Error</code> is thrown while processing
	''' the invocation on the server, either while unmarshalling the arguments,
	''' executing the remote method itself, or marshalling the return value.
	''' 
	''' A <code>ServerError</code> instance contains the original
	''' <code>Error</code> that occurred as its cause.
	''' 
	''' @author  Ann Wollrath
	''' @since   JDK1.1
	''' </summary>
	Public Class ServerError
		Inherits RemoteException

		' indicate compatibility with JDK 1.1.x version of class 
		Private Shadows Const serialVersionUID As Long = 8455284893909696482L

		''' <summary>
		''' Constructs a <code>ServerError</code> with the specified
		''' detail message and nested error.
		''' </summary>
		''' <param name="s"> the detail message </param>
		''' <param name="err"> the nested error
		''' @since JDK1.1 </param>
		Public Sub New(  s As String,   err As [Error])
			MyBase.New(s, err)
		End Sub
	End Class

End Namespace
Imports System

'
' * Copyright (c) 1996, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' From a server executing on JDK&nbsp;1.1, a
	''' <code>ServerRuntimeException</code> is thrown as a result of a
	''' remote method invocation when a <code>RuntimeException</code> is
	''' thrown while processing the invocation on the server, either while
	''' unmarshalling the arguments, executing the remote method itself, or
	''' marshalling the return value.
	''' 
	''' A <code>ServerRuntimeException</code> instance contains the original
	''' <code>RuntimeException</code> that occurred as its cause.
	''' 
	''' <p>A <code>ServerRuntimeException</code> is not thrown from servers
	''' executing on the Java 2 platform v1.2 or later versions.
	''' 
	''' @author  Ann Wollrath
	''' @since   JDK1.1 </summary>
	''' @deprecated no replacement 
	<Obsolete("no replacement")> _
	Public Class ServerRuntimeException
		Inherits RemoteException

		' indicate compatibility with JDK 1.1.x version of class 
		Private Shadows Const serialVersionUID As Long = 7054464920481467219L

		''' <summary>
		''' Constructs a <code>ServerRuntimeException</code> with the specified
		''' detail message and nested exception.
		''' </summary>
		''' <param name="s"> the detail message </param>
		''' <param name="ex"> the nested exception </param>
		''' @deprecated no replacement
		''' @since JDK1.1 
		<Obsolete("no replacement")> _
		Public Sub New(  s As String,   ex As Exception)
			MyBase.New(s, ex)
		End Sub
	End Class

End Namespace
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

Namespace java.rmi

	''' <summary>
	''' An <code>UnmarshalException</code> can be thrown while unmarshalling the
	''' parameters or results of a remote method call if any of the following
	''' conditions occur:
	''' <ul>
	''' <li> if an exception occurs while unmarshalling the call header
	''' <li> if the protocol for the return value is invalid
	''' <li> if a <code>java.io.IOException</code> occurs unmarshalling
	''' parameters (on the server side) or the return value (on the client side).
	''' <li> if a <code>java.lang.ClassNotFoundException</code> occurs during
	''' unmarshalling parameters or return values
	''' <li> if no skeleton can be loaded on the server-side; note that skeletons
	''' are required in the 1.1 stub protocol, but not in the 1.2 stub protocol.
	''' <li> if the method hash is invalid (i.e., missing method).
	''' <li> if there is a failure to create a remote reference object for
	''' a remote object's stub when it is unmarshalled.
	''' </ul>
	''' 
	''' @author  Ann Wollrath
	''' @since   JDK1.1
	''' </summary>
	Public Class UnmarshalException
		Inherits RemoteException

		' indicate compatibility with JDK 1.1.x version of class 
		Private Shadows Const serialVersionUID As Long = 594380845140740218L

		''' <summary>
		''' Constructs an <code>UnmarshalException</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="s"> the detail message
		''' @since JDK1.1 </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub

		''' <summary>
		''' Constructs an <code>UnmarshalException</code> with the specified
		''' detail message and nested exception.
		''' </summary>
		''' <param name="s"> the detail message </param>
		''' <param name="ex"> the nested exception
		''' @since JDK1.1 </param>
		Public Sub New(  s As String,   ex As Exception)
			MyBase.New(s, ex)
		End Sub
	End Class

End Namespace
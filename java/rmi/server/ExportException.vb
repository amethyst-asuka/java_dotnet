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
	''' An <code>ExportException</code> is a <code>RemoteException</code>
	''' thrown if an attempt to export a remote object fails.  A remote object is
	''' exported via the constructors and <code>exportObject</code> methods of
	''' <code>java.rmi.server.UnicastRemoteObject</code> and
	''' <code>java.rmi.activation.Activatable</code>.
	''' 
	''' @author  Ann Wollrath
	''' @since   JDK1.1 </summary>
	''' <seealso cref= java.rmi.server.UnicastRemoteObject </seealso>
	''' <seealso cref= java.rmi.activation.Activatable </seealso>
	Public Class ExportException
		Inherits java.rmi.RemoteException

		' indicate compatibility with JDK 1.1.x version of class 
		Private Shadows Const serialVersionUID As Long = -9155485338494060170L

		''' <summary>
		''' Constructs an <code>ExportException</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="s"> the detail message
		''' @since JDK1.1 </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub

		''' <summary>
		''' Constructs an <code>ExportException</code> with the specified
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
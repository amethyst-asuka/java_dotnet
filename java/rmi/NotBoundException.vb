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
	''' A <code>NotBoundException</code> is thrown if an attempt
	''' is made to lookup or unbind in the registry a name that has
	''' no associated binding.
	''' 
	''' @since   JDK1.1
	''' @author  Ann Wollrath
	''' @author  Roger Riggs </summary>
	''' <seealso cref=     java.rmi.Naming#lookup(String) </seealso>
	''' <seealso cref=     java.rmi.Naming#unbind(String) </seealso>
	''' <seealso cref=     java.rmi.registry.Registry#lookup(String) </seealso>
	''' <seealso cref=     java.rmi.registry.Registry#unbind(String) </seealso>
	Public Class NotBoundException
		Inherits Exception

		' indicate compatibility with JDK 1.1.x version of class 
		Private Shadows Const serialVersionUID As Long = -1857741824849069317L

		''' <summary>
		''' Constructs a <code>NotBoundException</code> with no
		''' specified detail message.
		''' @since JDK1.1
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a <code>NotBoundException</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="s"> the detail message
		''' @since JDK1.1 </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace
Imports System

'
' * Copyright (c) 1999, 2003, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management


	''' <summary>
	''' Wraps exceptions thrown by the preRegister(), preDeregister() methods
	''' of the <CODE>MBeanRegistration</CODE> interface.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class MBeanRegistrationException
		Inherits MBeanException

		' Serial version 
		Private Const serialVersionUID As Long = 4482382455277067805L

		''' <summary>
		''' Creates an <CODE>MBeanRegistrationException</CODE> that wraps
		''' the actual <CODE>java.lang.Exception</CODE>.
		''' </summary>
		''' <param name="e"> the wrapped exception. </param>
		Public Sub New(ByVal e As Exception)
			MyBase.New(e)
		End Sub

		''' <summary>
		''' Creates an <CODE>MBeanRegistrationException</CODE> that wraps
		''' the actual <CODE>java.lang.Exception</CODE> with a detailed
		''' message.
		''' </summary>
		''' <param name="e"> the wrapped exception. </param>
		''' <param name="message"> the detail message. </param>
		Public Sub New(ByVal e As Exception, ByVal message As String)
			MyBase.New(e, message)
		End Sub
	End Class

End Namespace
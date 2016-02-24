Imports System

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.security

	''' <summary>
	''' The {@code GeneralSecurityException} class is a generic
	''' security exception class that provides type safety for all the
	''' security-related exception classes that extend from it.
	''' 
	''' @author Jan Luehe
	''' </summary>

	Public Class GeneralSecurityException
		Inherits Exception

		Private Shadows Const serialVersionUID As Long = 894798122053539237L

		''' <summary>
		''' Constructs a GeneralSecurityException with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a GeneralSecurityException with the specified detail
		''' message.
		''' A detail message is a String that describes this particular
		''' exception.
		''' </summary>
		''' <param name="msg"> the detail message. </param>
		Public Sub New(ByVal msg As String)
			MyBase.New(msg)
		End Sub

		''' <summary>
		''' Creates a {@code GeneralSecurityException} with the specified
		''' detail message and cause.
		''' </summary>
		''' <param name="message"> the detail message (which is saved for later retrieval
		'''        by the <seealso cref="#getMessage()"/> method). </param>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		'''        <seealso cref="#getCause()"/> method).  (A {@code null} value is permitted,
		'''        and indicates that the cause is nonexistent or unknown.)
		''' @since 1.5 </param>
		Public Sub New(ByVal message As String, ByVal cause As Throwable)
			MyBase.New(message, cause)
		End Sub

		''' <summary>
		''' Creates a {@code GeneralSecurityException} with the specified cause
		''' and a detail message of {@code (cause==null ? null : cause.toString())}
		''' (which typically contains the class and detail message of
		''' {@code cause}).
		''' </summary>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		'''        <seealso cref="#getCause()"/> method).  (A {@code null} value is permitted,
		'''        and indicates that the cause is nonexistent or unknown.)
		''' @since 1.5 </param>
		Public Sub New(ByVal cause As Throwable)
			MyBase.New(cause)
		End Sub
	End Class

End Namespace
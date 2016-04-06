'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This is the basic key exception.
	''' </summary>
	''' <seealso cref= Key </seealso>
	''' <seealso cref= InvalidKeyException </seealso>
	''' <seealso cref= KeyManagementException
	''' 
	''' @author Benjamin Renaud </seealso>

	Public Class KeyException
		Inherits GeneralSecurityException

		Private Shadows Const serialVersionUID As Long = -7483676942812432108L

		''' <summary>
		''' Constructs a KeyException with no detail message. A detail
		''' message is a String that describes this particular exception.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a KeyException with the specified detail message.
		''' A detail message is a String that describes this particular
		''' exception.
		''' </summary>
		''' <param name="msg"> the detail message. </param>
		Public Sub New(  msg As String)
			MyBase.New(msg)
		End Sub

		''' <summary>
		''' Creates a {@code KeyException} with the specified
		''' detail message and cause.
		''' </summary>
		''' <param name="message"> the detail message (which is saved for later retrieval
		'''        by the <seealso cref="#getMessage()"/> method). </param>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		'''        <seealso cref="#getCause()"/> method).  (A {@code null} value is permitted,
		'''        and indicates that the cause is nonexistent or unknown.)
		''' @since 1.5 </param>
		Public Sub New(  message As String,   cause As Throwable)
			MyBase.New(message, cause)
		End Sub

		''' <summary>
		''' Creates a {@code KeyException} with the specified cause
		''' and a detail message of {@code (cause==null ? null : cause.toString())}
		''' (which typically contains the class and detail message of
		''' {@code cause}).
		''' </summary>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		'''        <seealso cref="#getCause()"/> method).  (A {@code null} value is permitted,
		'''        and indicates that the cause is nonexistent or unknown.)
		''' @since 1.5 </param>
		Public Sub New(  cause As Throwable)
			MyBase.New(cause)
		End Sub
	End Class

End Namespace
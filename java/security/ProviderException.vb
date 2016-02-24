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
	''' A runtime exception for Provider exceptions (such as
	''' misconfiguration errors or unrecoverable internal errors),
	''' which may be subclassed by Providers to
	''' throw specialized, provider-specific runtime errors.
	''' 
	''' @author Benjamin Renaud
	''' </summary>
	Public Class ProviderException
		Inherits RuntimeException

		Private Shadows Const serialVersionUID As Long = 5256023526693665674L

		''' <summary>
		''' Constructs a ProviderException with no detail message. A
		''' detail message is a String that describes this particular
		''' exception.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a ProviderException with the specified detail
		''' message. A detail message is a String that describes this
		''' particular exception.
		''' </summary>
		''' <param name="s"> the detail message. </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub

		''' <summary>
		''' Creates a {@code ProviderException} with the specified
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
		''' Creates a {@code ProviderException} with the specified cause
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
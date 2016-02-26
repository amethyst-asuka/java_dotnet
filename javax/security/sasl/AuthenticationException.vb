Imports System

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.security.sasl

	''' <summary>
	''' This exception is thrown by a SASL mechanism implementation
	''' to indicate that the SASL
	''' exchange has failed due to reasons related to authentication, such as
	''' an invalid identity, passphrase, or key.
	''' <p>
	''' Note that the lack of an AuthenticationException does not mean that
	''' the failure was not due to an authentication error.  A SASL mechanism
	''' implementation might throw the more general SaslException instead of
	''' AuthenticationException if it is unable to determine the nature
	''' of the failure, or if does not want to disclose the nature of
	''' the failure, for example, due to security reasons.
	''' 
	''' @since 1.5
	''' 
	''' @author Rosanna Lee
	''' @author Rob Weltman
	''' </summary>
	Public Class AuthenticationException
		Inherits SaslException

		''' <summary>
		''' Constructs a new instance of {@code AuthenticationException}.
		''' The root exception and the detailed message are null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a new instance of {@code AuthenticationException}
		''' with a detailed message.
		''' The root exception is null. </summary>
		''' <param name="detail"> A possibly null string containing details of the exception.
		''' </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal detail As String)
			MyBase.New(detail)
		End Sub

		''' <summary>
		''' Constructs a new instance of {@code AuthenticationException} with a detailed message
		''' and a root exception.
		''' </summary>
		''' <param name="detail"> A possibly null string containing details of the exception. </param>
		''' <param name="ex"> A possibly null root exception that caused this exception.
		''' </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		''' <seealso cref= #getCause </seealso>
		Public Sub New(ByVal detail As String, ByVal ex As Exception)
			MyBase.New(detail, ex)
		End Sub

		''' <summary>
		''' Use serialVersionUID from JSR 28 RI for interoperability </summary>
		Private Const serialVersionUID As Long = -3579708765071815007L
	End Class

End Namespace
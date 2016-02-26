Imports System

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This class represents an error that has occurred when using SASL.
	''' 
	''' @since 1.5
	''' 
	''' @author Rosanna Lee
	''' @author Rob Weltman
	''' </summary>

	Public Class SaslException
		Inherits java.io.IOException

		''' <summary>
		''' The possibly null root cause exception.
		''' @serial
		''' </summary>
		' Required for serialization interoperability with JSR 28
		Private _exception As Exception

		''' <summary>
		''' Constructs a new instance of {@code SaslException}.
		''' The root exception and the detailed message are null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a new instance of {@code SaslException} with a detailed message.
		''' The root exception is null. </summary>
		''' <param name="detail"> A possibly null string containing details of the exception.
		''' </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal detail As String)
			MyBase.New(detail)
		End Sub

		''' <summary>
		''' Constructs a new instance of {@code SaslException} with a detailed message
		''' and a root exception.
		''' For example, a SaslException might result from a problem with
		''' the callback handler, which might throw a NoSuchCallbackException if
		''' it does not support the requested callback, or throw an IOException
		''' if it had problems obtaining data for the callback. The
		''' SaslException's root exception would be then be the exception thrown
		''' by the callback handler.
		''' </summary>
		''' <param name="detail"> A possibly null string containing details of the exception. </param>
		''' <param name="ex"> A possibly null root exception that caused this exception.
		''' </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		''' <seealso cref= #getCause </seealso>
		Public Sub New(ByVal detail As String, ByVal ex As Exception)
			MyBase.New(detail)
			If ex IsNot Nothing Then initCause(ex)
		End Sub

	'    
	'     * Override Throwable.getCause() to ensure deserialized object from
	'     * JSR 28 would return same value for getCause() (i.e., _exception).
	'     
		Public Overridable Property cause As Exception
			Get
				Return _exception
			End Get
		End Property

	'    
	'     * Override Throwable.initCause() to match getCause() by updating
	'     * _exception as well.
	'     
		Public Overridable Function initCause(ByVal cause As Exception) As Exception
			MyBase.initCause(cause)
			_exception = cause
			Return Me
		End Function

		''' <summary>
		''' Returns the string representation of this exception.
		''' The string representation contains
		''' this exception's class name, its detailed message, and if
		''' it has a root exception, the string representation of the root
		''' exception. This string representation
		''' is meant for debugging and not meant to be interpreted
		''' programmatically. </summary>
		''' <returns> The non-null string representation of this exception. </returns>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		' Override Throwable.toString() to conform to JSR 28
		Public Overrides Function ToString() As String
			Dim answer As String = MyBase.ToString()
			If _exception IsNot Nothing AndAlso _exception IsNot Me Then answer &= " [Caused by " & _exception.ToString() & "]"
			Return answer
		End Function

		''' <summary>
		''' Use serialVersionUID from JSR 28 RI for interoperability </summary>
		Private Const serialVersionUID As Long = 4579784287983423626L
	End Class

End Namespace
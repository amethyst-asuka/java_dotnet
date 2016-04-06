Imports System

'
' * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.invoke

	''' <summary>
	''' LambdaConversionException
	''' </summary>
	Public Class LambdaConversionException
		Inherits Exception

		Private Shadows Shared ReadOnly serialVersionUID As Long = 292L + 8L

		''' <summary>
		''' Constructs a {@code LambdaConversionException}.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a {@code LambdaConversionException} with a message. </summary>
		''' <param name="message"> the detail message </param>
		Public Sub New(  message As String)
			MyBase.New(message)
		End Sub

		''' <summary>
		''' Constructs a {@code LambdaConversionException} with a message and cause. </summary>
		''' <param name="message"> the detail message </param>
		''' <param name="cause"> the cause </param>
		Public Sub New(  message As String,   cause As Throwable)
			MyBase.New(message, cause)
		End Sub

		''' <summary>
		''' Constructs a {@code LambdaConversionException} with a cause. </summary>
		''' <param name="cause"> the cause </param>
		Public Sub New(  cause As Throwable)
			MyBase.New(cause)
		End Sub

		''' <summary>
		''' Constructs a {@code LambdaConversionException} with a message,
		''' cause, and other settings. </summary>
		''' <param name="message"> the detail message </param>
		''' <param name="cause"> the cause </param>
		''' <param name="enableSuppression"> whether or not suppressed exceptions are enabled </param>
		''' <param name="writableStackTrace"> whether or not the stack trace is writable </param>
		Public Sub New(  message As String,   cause As Throwable,   enableSuppression As Boolean,   writableStackTrace As Boolean)
			MyBase.New(message, cause, enableSuppression, writableStackTrace)
		End Sub
	End Class

End Namespace
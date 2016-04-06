'
' * Copyright (c) 2008, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang

	''' <summary>
	''' Thrown to indicate that an {@code invokedynamic} instruction has
	''' failed to find its bootstrap method,
	''' or the bootstrap method has failed to provide a
	''' <seealso cref="java.lang.invoke.CallSite call site"/> with a <seealso cref="java.lang.invoke.CallSite#getTarget target"/>
	''' of the correct <seealso cref="java.lang.invoke.MethodHandle#type method type"/>.
	''' 
	''' @author John Rose, JSR 292 EG
	''' @since 1.7
	''' </summary>
	Public Class BootstrapMethodError
		Inherits LinkageError

		Private Shadows Const serialVersionUID As Long = 292L

		''' <summary>
		''' Constructs a {@code BootstrapMethodError} with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a {@code BootstrapMethodError} with the specified
		''' detail message.
		''' </summary>
		''' <param name="s"> the detail message. </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub

		''' <summary>
		''' Constructs a {@code BootstrapMethodError} with the specified
		''' detail message and cause.
		''' </summary>
		''' <param name="s"> the detail message. </param>
		''' <param name="cause"> the cause, may be {@code null}. </param>
		Public Sub New(  s As String,   cause As Throwable)
			MyBase.New(s, cause)
		End Sub

		''' <summary>
		''' Constructs a {@code BootstrapMethodError} with the specified
		''' cause.
		''' </summary>
		''' <param name="cause"> the cause, may be {@code null}. </param>
		Public Sub New(  cause As Throwable)
			' cf. Throwable(Throwable cause) constructor.
			MyBase.New(If(cause Is Nothing, Nothing, cause.ToString()))
			initCause(cause)
		End Sub
	End Class

End Namespace
Imports System

'
' * Copyright (c) 2009, Oracle and/or its affiliates. All rights reserved.
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
	''' Common superclass of exceptions thrown by reflective operations in
	''' core reflection.
	''' </summary>
	''' <seealso cref= LinkageError
	''' @since 1.7 </seealso>
	Public Class ReflectiveOperationException
		Inherits Exception

		Friend Shadows Const serialVersionUID As Long = 123456789L

		''' <summary>
		''' Constructs a new exception with {@code null} as its detail
		''' message.  The cause is not initialized, and may subsequently be
		''' initialized by a call to <seealso cref="#initCause"/>.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a new exception with the specified detail message.
		''' The cause is not initialized, and may subsequently be
		''' initialized by a call to <seealso cref="#initCause"/>.
		''' </summary>
		''' <param name="message">   the detail message. The detail message is saved for
		'''          later retrieval by the <seealso cref="#getMessage()"/> method. </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		''' <summary>
		''' Constructs a new exception with the specified detail message
		''' and cause.
		''' 
		''' <p>Note that the detail message associated with
		''' {@code cause} is <em>not</em> automatically incorporated in
		''' this exception's detail message.
		''' </summary>
		''' <param name="message"> the detail message (which is saved for later retrieval
		'''         by the <seealso cref="#getMessage()"/> method). </param>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		'''         <seealso cref="#getCause()"/> method).  (A {@code null} value is
		'''         permitted, and indicates that the cause is nonexistent or
		'''         unknown.) </param>
		Public Sub New(ByVal message As String, ByVal cause As Throwable)
			MyBase.New(message, cause)
		End Sub

		''' <summary>
		''' Constructs a new exception with the specified cause and a detail
		''' message of {@code (cause==null ? null : cause.toString())} (which
		''' typically contains the class and detail message of {@code cause}).
		''' </summary>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		'''         <seealso cref="#getCause()"/> method).  (A {@code null} value is
		'''         permitted, and indicates that the cause is nonexistent or
		'''         unknown.) </param>
		Public Sub New(ByVal cause As Throwable)
			MyBase.New(cause)
		End Sub
	End Class

End Namespace
Imports System

'
' * Copyright (c) 1995, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' {@code RuntimeException} is the superclass of those
	''' exceptions that can be thrown during the normal operation of the
	''' Java Virtual Machine.
	''' 
	''' <p>{@code RuntimeException} and its subclasses are <em>unchecked
	''' exceptions</em>.  Unchecked exceptions do <em>not</em> need to be
	''' declared in a method or constructor's {@code throws} clause if they
	''' can be thrown by the execution of the method or constructor and
	''' propagate outside the method or constructor boundary.
	''' 
	''' @author  Frank Yellin
	''' @jls 11.2 Compile-Time Checking of Exceptions
	''' @since   JDK1.0
	''' </summary>
	Public Class RuntimeException
		Inherits Exception

		Friend Shadows Const serialVersionUID As Long = -7034897190745766939L

		''' <summary>
		''' Constructs a new runtime exception with {@code null} as its
		''' detail message.  The cause is not initialized, and may subsequently be
		''' initialized by a call to <seealso cref="#initCause"/>.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a new runtime exception with the specified detail message.
		''' The cause is not initialized, and may subsequently be initialized by a
		''' call to <seealso cref="#initCause"/>.
		''' </summary>
		''' <param name="message">   the detail message. The detail message is saved for
		'''          later retrieval by the <seealso cref="#getMessage()"/> method. </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		''' <summary>
		''' Constructs a new runtime exception with the specified detail message and
		''' cause.  <p>Note that the detail message associated with
		''' {@code cause} is <i>not</i> automatically incorporated in
		''' this runtime exception's detail message.
		''' </summary>
		''' <param name="message"> the detail message (which is saved for later retrieval
		'''         by the <seealso cref="#getMessage()"/> method). </param>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		'''         <seealso cref="#getCause()"/> method).  (A <tt>null</tt> value is
		'''         permitted, and indicates that the cause is nonexistent or
		'''         unknown.)
		''' @since  1.4 </param>
		Public Sub New(ByVal message As String, ByVal cause As Throwable)
			MyBase.New(message, cause)
		End Sub

		''' <summary>
		''' Constructs a new runtime exception with the specified cause and a
		''' detail message of <tt>(cause==null ? null : cause.toString())</tt>
		''' (which typically contains the class and detail message of
		''' <tt>cause</tt>).  This constructor is useful for runtime exceptions
		''' that are little more than wrappers for other throwables.
		''' </summary>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		'''         <seealso cref="#getCause()"/> method).  (A <tt>null</tt> value is
		'''         permitted, and indicates that the cause is nonexistent or
		'''         unknown.)
		''' @since  1.4 </param>
		Public Sub New(ByVal cause As Throwable)
			MyBase.New(cause)
		End Sub

		''' <summary>
		''' Constructs a new runtime exception with the specified detail
		''' message, cause, suppression enabled or disabled, and writable
		''' stack trace enabled or disabled.
		''' </summary>
		''' <param name="message"> the detail message. </param>
		''' <param name="cause"> the cause.  (A {@code null} value is permitted,
		''' and indicates that the cause is nonexistent or unknown.) </param>
		''' <param name="enableSuppression"> whether or not suppression is enabled
		'''                          or disabled </param>
		''' <param name="writableStackTrace"> whether or not the stack trace should
		'''                           be writable
		''' 
		''' @since 1.7 </param>
		Protected Friend Sub New(ByVal message As String, ByVal cause As Throwable, ByVal enableSuppression As Boolean, ByVal writableStackTrace As Boolean)
			MyBase.New(message, cause, enableSuppression, writableStackTrace)
		End Sub
	End Class

End Namespace
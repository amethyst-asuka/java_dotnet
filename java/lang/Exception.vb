Imports System

'
' * Copyright (c) 1994, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' The class {@code Exception} and its subclasses are a form of
	''' {@code Throwable} that indicates conditions that a reasonable
	''' application might want to catch.
	''' 
	''' <p>The class {@code Exception} and any subclasses that are not also
	''' subclasses of <seealso cref="RuntimeException"/> are <em>checked
	''' exceptions</em>.  Checked exceptions need to be declared in a
	''' method or constructor's {@code throws} clause if they can be thrown
	''' by the execution of the method or constructor and propagate outside
	''' the method or constructor boundary.
	''' 
	''' @author  Frank Yellin </summary>
	''' <seealso cref=     java.lang.Error
	''' @jls 11.2 Compile-Time Checking of Exceptions
	''' @since   JDK1.0 </seealso>
	Public Class Exception
		Inherits Throwable

		Friend Const serialVersionUID As Long = -3387516993124229948L

		''' <summary>
		''' Constructs a new exception with {@code null} as its detail message.
		''' The cause is not initialized, and may subsequently be initialized by a
		''' call to <seealso cref="#initCause"/>.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a new exception with the specified detail message.  The
		''' cause is not initialized, and may subsequently be initialized by
		''' a call to <seealso cref="#initCause"/>.
		''' </summary>
		''' <param name="message">   the detail message. The detail message is saved for
		'''          later retrieval by the <seealso cref="#getMessage()"/> method. </param>
		Public Sub New(  message As String)
			MyBase.New(message)
		End Sub

		''' <summary>
		''' Constructs a new exception with the specified detail message and
		''' cause.  <p>Note that the detail message associated with
		''' {@code cause} is <i>not</i> automatically incorporated in
		''' this exception's detail message.
		''' </summary>
		''' <param name="message"> the detail message (which is saved for later retrieval
		'''         by the <seealso cref="#getMessage()"/> method). </param>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		'''         <seealso cref="#getCause()"/> method).  (A <tt>null</tt> value is
		'''         permitted, and indicates that the cause is nonexistent or
		'''         unknown.)
		''' @since  1.4 </param>
		Public Sub New(  message As String,   cause As Throwable)
			MyBase.New(message, cause)
		End Sub

		''' <summary>
		''' Constructs a new exception with the specified cause and a detail
		''' message of <tt>(cause==null ? null : cause.toString())</tt> (which
		''' typically contains the class and detail message of <tt>cause</tt>).
		''' This constructor is useful for exceptions that are little more than
		''' wrappers for other throwables (for example, {@link
		''' java.security.PrivilegedActionException}).
		''' </summary>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		'''         <seealso cref="#getCause()"/> method).  (A <tt>null</tt> value is
		'''         permitted, and indicates that the cause is nonexistent or
		'''         unknown.)
		''' @since  1.4 </param>
		Public Sub New(  cause As Throwable)
			MyBase.New(cause)
		End Sub

		''' <summary>
		''' Constructs a new exception with the specified detail message,
		''' cause, suppression enabled or disabled, and writable stack
		''' trace enabled or disabled.
		''' </summary>
		''' <param name="message"> the detail message. </param>
		''' <param name="cause"> the cause.  (A {@code null} value is permitted,
		''' and indicates that the cause is nonexistent or unknown.) </param>
		''' <param name="enableSuppression"> whether or not suppression is enabled
		'''                          or disabled </param>
		''' <param name="writableStackTrace"> whether or not the stack trace should
		'''                           be writable
		''' @since 1.7 </param>
		Protected Friend Sub New(  message As String,   cause As Throwable,   enableSuppression As Boolean,   writableStackTrace As Boolean)
			MyBase.New(message, cause, enableSuppression, writableStackTrace)
		End Sub
	End Class

End Namespace
'
' * Copyright (c) 1996, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' Signals that a method has been invoked at an illegal or
	''' inappropriate time.  In other words, the Java environment or
	''' Java application is not in an appropriate state for the requested
	''' operation.
	''' 
	''' @author  Jonni Kanerva
	''' @since   JDK1.1
	''' </summary>
	Public Class IllegalStateException
		Inherits RuntimeException

		''' <summary>
		''' Constructs an IllegalStateException with no detail message.
		''' A detail message is a String that describes this particular exception.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an IllegalStateException with the specified detail
		''' message.  A detail message is a String that describes this particular
		''' exception.
		''' </summary>
		''' <param name="s"> the String that contains a detailed message </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub

		''' <summary>
		''' Constructs a new exception with the specified detail message and
		''' cause.
		''' 
		''' <p>Note that the detail message associated with <code>cause</code> is
		''' <i>not</i> automatically incorporated in this exception's detail
		''' message.
		''' </summary>
		''' <param name="message"> the detail message (which is saved for later retrieval
		'''         by the <seealso cref="Throwable#getMessage()"/> method). </param>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		'''         <seealso cref="Throwable#getCause()"/> method).  (A <tt>null</tt> value
		'''         is permitted, and indicates that the cause is nonexistent or
		'''         unknown.)
		''' @since 1.5 </param>
		Public Sub New(ByVal message As String, ByVal cause As Throwable)
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
		'''         <seealso cref="Throwable#getCause()"/> method).  (A <tt>null</tt> value is
		'''         permitted, and indicates that the cause is nonexistent or
		'''         unknown.)
		''' @since  1.5 </param>
		Public Sub New(ByVal cause As Throwable)
			MyBase.New(cause)
		End Sub

		Friend Shadows Const serialVersionUID As Long = -1848914673093119416L
	End Class

End Namespace
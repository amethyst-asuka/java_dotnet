Imports System

'
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

'
' *
' *
' *
' *
' *
' * Written by Doug Lea with assistance from members of JCP JSR-166
' * Expert Group and released to the public domain, as explained at
' * http://creativecommons.org/publicdomain/zero/1.0/
' 

Namespace java.util.concurrent

	''' <summary>
	''' Exception thrown when attempting to retrieve the result of a task
	''' that aborted by throwing an exception. This exception can be
	''' inspected using the <seealso cref="#getCause()"/> method.
	''' </summary>
	''' <seealso cref= Future
	''' @since 1.5
	''' @author Doug Lea </seealso>
	Public Class ExecutionException
		Inherits Exception

		Private Shadows Const serialVersionUID As Long = 7830266012832686185L

		''' <summary>
		''' Constructs an {@code ExecutionException} with no detail message.
		''' The cause is not initialized, and may subsequently be
		''' initialized by a call to <seealso cref="#initCause(Throwable) initCause"/>.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Constructs an {@code ExecutionException} with the specified detail
		''' message. The cause is not initialized, and may subsequently be
		''' initialized by a call to <seealso cref="#initCause(Throwable) initCause"/>.
		''' </summary>
		''' <param name="message"> the detail message </param>
		Protected Friend Sub New(  message As String)
			MyBase.New(message)
		End Sub

		''' <summary>
		''' Constructs an {@code ExecutionException} with the specified detail
		''' message and cause.
		''' </summary>
		''' <param name="message"> the detail message </param>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		'''         <seealso cref="#getCause()"/> method) </param>
		Public Sub New(  message As String,   cause As Throwable)
			MyBase.New(message, cause)
		End Sub

		''' <summary>
		''' Constructs an {@code ExecutionException} with the specified cause.
		''' The detail message is set to {@code (cause == null ? null :
		''' cause.toString())} (which typically contains the class and
		''' detail message of {@code cause}).
		''' </summary>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		'''         <seealso cref="#getCause()"/> method) </param>
		Public Sub New(  cause As Throwable)
			MyBase.New(cause)
		End Sub
	End Class

End Namespace
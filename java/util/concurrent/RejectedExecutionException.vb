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
	''' Exception thrown by an <seealso cref="Executor"/> when a task cannot be
	''' accepted for execution.
	''' 
	''' @since 1.5
	''' @author Doug Lea
	''' </summary>
	Public Class RejectedExecutionException
		Inherits RuntimeException

		Private Shadows Const serialVersionUID As Long = -375805702767069545L

		''' <summary>
		''' Constructs a {@code RejectedExecutionException} with no detail message.
		''' The cause is not initialized, and may subsequently be
		''' initialized by a call to <seealso cref="#initCause(Throwable) initCause"/>.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a {@code RejectedExecutionException} with the
		''' specified detail message. The cause is not initialized, and may
		''' subsequently be initialized by a call to {@link
		''' #initCause(Throwable) initCause}.
		''' </summary>
		''' <param name="message"> the detail message </param>
		Public Sub New(  message As String)
			MyBase.New(message)
		End Sub

		''' <summary>
		''' Constructs a {@code RejectedExecutionException} with the
		''' specified detail message and cause.
		''' </summary>
		''' <param name="message"> the detail message </param>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		'''         <seealso cref="#getCause()"/> method) </param>
		Public Sub New(  message As String,   cause As Throwable)
			MyBase.New(message, cause)
		End Sub

		''' <summary>
		''' Constructs a {@code RejectedExecutionException} with the
		''' specified cause.  The detail message is set to {@code (cause ==
		''' null ? null : cause.toString())} (which typically contains
		''' the class and detail message of {@code cause}).
		''' </summary>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		'''         <seealso cref="#getCause()"/> method) </param>
		Public Sub New(  cause As Throwable)
			MyBase.New(cause)
		End Sub
	End Class

End Namespace
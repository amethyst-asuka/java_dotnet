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
	''' Exception indicating that the result of a value-producing task,
	''' such as a <seealso cref="FutureTask"/>, cannot be retrieved because the task
	''' was cancelled.
	''' 
	''' @since 1.5
	''' @author Doug Lea
	''' </summary>
	Public Class CancellationException
		Inherits IllegalStateException

		Private Shadows Const serialVersionUID As Long = -9202173006928992231L

		''' <summary>
		''' Constructs a {@code CancellationException} with no detail message.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a {@code CancellationException} with the specified detail
		''' message.
		''' </summary>
		''' <param name="message"> the detail message </param>
		Public Sub New(  message As String)
			MyBase.New(message)
		End Sub
	End Class

End Namespace
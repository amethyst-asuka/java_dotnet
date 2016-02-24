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
	''' Exception thrown when a thread tries to wait upon a barrier that is
	''' in a broken state, or which enters the broken state while the thread
	''' is waiting.
	''' </summary>
	''' <seealso cref= CyclicBarrier
	''' 
	''' @since 1.5
	''' @author Doug Lea </seealso>
	Public Class BrokenBarrierException
		Inherits Exception

		Private Shadows Const serialVersionUID As Long = 7117394618823254244L

		''' <summary>
		''' Constructs a {@code BrokenBarrierException} with no specified detail
		''' message.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a {@code BrokenBarrierException} with the specified
		''' detail message.
		''' </summary>
		''' <param name="message"> the detail message </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub
	End Class

End Namespace
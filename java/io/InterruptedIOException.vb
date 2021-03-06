'
' * Copyright (c) 1995, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.io

	''' <summary>
	''' Signals that an I/O operation has been interrupted. An
	''' <code>InterruptedIOException</code> is thrown to indicate that an
	''' input or output transfer has been terminated because the thread
	''' performing it was interrupted. The field <seealso cref="#bytesTransferred"/>
	''' indicates how many bytes were successfully transferred before
	''' the interruption occurred.
	''' 
	''' @author  unascribed </summary>
	''' <seealso cref=     java.io.InputStream </seealso>
	''' <seealso cref=     java.io.OutputStream </seealso>
	''' <seealso cref=     java.lang.Thread#interrupt()
	''' @since   JDK1.0 </seealso>
	Public Class InterruptedIOException
		Inherits IOException

		Private Shadows Const serialVersionUID As Long = 4020568460727500567L

		''' <summary>
		''' Constructs an <code>InterruptedIOException</code> with
		''' <code>null</code> as its error detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an <code>InterruptedIOException</code> with the
		''' specified detail message. The string <code>s</code> can be
		''' retrieved later by the
		''' <code><seealso cref="java.lang.Throwable#getMessage"/></code>
		''' method of class <code>java.lang.Throwable</code>.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub

		''' <summary>
		''' Reports how many bytes had been transferred as part of the I/O
		''' operation before it was interrupted.
		''' 
		''' @serial
		''' </summary>
		Public bytesTransferred As Integer = 0
	End Class

End Namespace
Imports System

'
' * Copyright (c) 2000, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.imageio


	''' <summary>
	''' An exception class used for signaling run-time failure of reading
	''' and writing operations.
	''' 
	''' <p> In addition to a message string, a reference to another
	''' <code>Throwable</code> (<code>Error</code> or
	''' <code>Exception</code>) is maintained.  This reference, if
	''' non-<code>null</code>, refers to the event that caused this
	''' exception to occur.  For example, an <code>IOException</code> while
	''' reading from a <code>File</code> would be stored there.
	''' 
	''' </summary>
	Public Class IIOException
		Inherits java.io.IOException

		''' <summary>
		''' Constructs an <code>IIOException</code> with a given message
		''' <code>String</code>.  No underlying cause is set;
		''' <code>getCause</code> will return <code>null</code>.
		''' </summary>
		''' <param name="message"> the error message.
		''' </param>
		''' <seealso cref= #getMessage </seealso>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		''' <summary>
		''' Constructs an <code>IIOException</code> with a given message
		''' <code>String</code> and a <code>Throwable</code> that was its
		''' underlying cause.
		''' </summary>
		''' <param name="message"> the error message. </param>
		''' <param name="cause"> the <code>Throwable</code> (<code>Error</code> or
		''' <code>Exception</code>) that caused this exception to occur.
		''' </param>
		''' <seealso cref= #getCause </seealso>
		''' <seealso cref= #getMessage </seealso>
		Public Sub New(ByVal message As String, ByVal cause As Exception)
			MyBase.New(message)
			initCause(cause)
		End Sub
	End Class

End Namespace
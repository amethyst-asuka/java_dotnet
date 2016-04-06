'
' * Copyright (c) 1998, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.print

	''' <summary>
	''' The <code>PrinterAbortException</code> class is a subclass of
	''' <seealso cref="PrinterException"/> and is used to indicate that a user
	''' or application has terminated the print job while it was in
	''' the process of printing.
	''' </summary>

	Public Class PrinterAbortException
		Inherits PrinterException

		''' <summary>
		''' Constructs a new <code>PrinterAbortException</code> with no
		''' detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a new <code>PrinterAbortException</code> with
		''' the specified detail message. </summary>
		''' <param name="msg"> the message to be generated when a
		''' <code>PrinterAbortException</code> is thrown </param>
		Public Sub New(  msg As String)
			MyBase.New(msg)
		End Sub

	End Class

End Namespace
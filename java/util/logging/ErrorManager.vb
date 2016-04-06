Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 2001, 2004, Oracle and/or its affiliates. All rights reserved.
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


Namespace java.util.logging

	''' <summary>
	''' ErrorManager objects can be attached to Handlers to process
	''' any error that occurs on a Handler during Logging.
	''' <p>
	''' When processing logging output, if a Handler encounters problems
	''' then rather than throwing an Exception back to the issuer of
	''' the logging call (who is unlikely to be interested) the Handler
	''' should call its associated ErrorManager.
	''' </summary>

	Public Class ErrorManager
	   Private reported As Boolean = False

	'    
	'     * We declare standard error codes for important categories of errors.
	'     

		''' <summary>
		''' GENERIC_FAILURE is used for failure that don't fit
		''' into one of the other categories.
		''' </summary>
		Public Const GENERIC_FAILURE As Integer = 0
		''' <summary>
		''' WRITE_FAILURE is used when a write to an output stream fails.
		''' </summary>
		Public Const WRITE_FAILURE As Integer = 1
		''' <summary>
		''' FLUSH_FAILURE is used when a flush to an output stream fails.
		''' </summary>
		Public Const FLUSH_FAILURE As Integer = 2
		''' <summary>
		''' CLOSE_FAILURE is used when a close of an output stream fails.
		''' </summary>
		Public Const CLOSE_FAILURE As Integer = 3
		''' <summary>
		''' OPEN_FAILURE is used when an open of an output stream fails.
		''' </summary>
		Public Const OPEN_FAILURE As Integer = 4
		''' <summary>
		''' FORMAT_FAILURE is used when formatting fails for any reason.
		''' </summary>
		Public Const FORMAT_FAILURE As Integer = 5

		''' <summary>
		''' The error method is called when a Handler failure occurs.
		''' <p>
		''' This method may be overridden in subclasses.  The default
		''' behavior in this base class is that the first call is
		''' reported to System.err, and subsequent calls are ignored.
		''' </summary>
		''' <param name="msg">    a descriptive string (may be null) </param>
		''' <param name="ex">     an exception (may be null) </param>
		''' <param name="code">   an error code defined in ErrorManager </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub [error](  msg As String,   ex As Exception,   code As Integer)
			If reported Then Return
			reported = True
			Dim text As String = "java.util.logging.ErrorManager: " & code
			If msg IsNot Nothing Then text = text & ": " & msg
			Console.Error.WriteLine(text)
			If ex IsNot Nothing Then ex.printStackTrace()
		End Sub
	End Class

End Namespace
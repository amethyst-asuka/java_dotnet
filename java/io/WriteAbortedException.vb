Imports System

'
' * Copyright (c) 1996, 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' Signals that one of the ObjectStreamExceptions was thrown during a
	''' write operation.  Thrown during a read operation when one of the
	''' ObjectStreamExceptions was thrown during a write operation.  The
	''' exception that terminated the write can be found in the detail
	''' field. The stream is reset to it's initial state and all references
	''' to objects already deserialized are discarded.
	''' 
	''' <p>As of release 1.4, this exception has been retrofitted to conform to
	''' the general purpose exception-chaining mechanism.  The "exception causing
	''' the abort" that is provided at construction time and
	''' accessed via the public <seealso cref="#detail"/> field is now known as the
	''' <i>cause</i>, and may be accessed via the <seealso cref="Throwable#getCause()"/>
	''' method, as well as the aforementioned "legacy field."
	''' 
	''' @author  unascribed
	''' @since   JDK1.1
	''' </summary>
	Public Class WriteAbortedException
		Inherits ObjectStreamException

		Private Shadows Const serialVersionUID As Long = -3326426625597282442L

		''' <summary>
		''' Exception that was caught while writing the ObjectStream.
		''' 
		''' <p>This field predates the general-purpose exception chaining facility.
		''' The <seealso cref="Throwable#getCause()"/> method is now the preferred means of
		''' obtaining this information.
		''' 
		''' @serial
		''' </summary>
		Public detail As Exception

		''' <summary>
		''' Constructs a WriteAbortedException with a string describing
		''' the exception and the exception causing the abort. </summary>
		''' <param name="s">   String describing the exception. </param>
		''' <param name="ex">  Exception causing the abort. </param>
		Public Sub New(ByVal s As String, ByVal ex As Exception)
			MyBase.New(s)
			initCause(Nothing) ' Disallow subsequent initCause
			detail = ex
		End Sub

		''' <summary>
		''' Produce the message and include the message from the nested
		''' exception, if there is one.
		''' </summary>
		Public  Overrides ReadOnly Property  message As String
			Get
				If detail Is Nothing Then
					Return MyBase.message
				Else
					Return MyBase.message & "; " & detail.ToString()
				End If
			End Get
		End Property

		''' <summary>
		''' Returns the exception that terminated the operation (the <i>cause</i>).
		''' </summary>
		''' <returns>  the exception that terminated the operation (the <i>cause</i>),
		'''          which may be null.
		''' @since   1.4 </returns>
		Public  Overrides ReadOnly Property  cause As Throwable
			Get
				Return detail
			End Get
		End Property
	End Class

End Namespace
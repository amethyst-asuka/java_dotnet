Imports Microsoft.VisualBasic
Imports System

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

Namespace java.rmi.server

	''' <summary>
	''' A <code>ServerCloneException</code> is thrown if a remote exception occurs
	''' during the cloning of a <code>UnicastRemoteObject</code>.
	''' 
	''' <p>As of release 1.4, this exception has been retrofitted to conform to
	''' the general purpose exception-chaining mechanism.  The "nested exception"
	''' that may be provided at construction time and accessed via the public
	''' <seealso cref="#detail"/> field is now known as the <i>cause</i>, and may be
	''' accessed via the <seealso cref="Throwable#getCause()"/> method, as well as
	''' the aforementioned "legacy field."
	''' 
	''' <p>Invoking the method <seealso cref="Throwable#initCause(Throwable)"/> on an
	''' instance of <code>ServerCloneException</code> always throws {@link
	''' IllegalStateException}.
	''' 
	''' @author  Ann Wollrath
	''' @since   JDK1.1 </summary>
	''' <seealso cref=     java.rmi.server.UnicastRemoteObject#clone() </seealso>
	Public Class ServerCloneException
		Inherits CloneNotSupportedException

		''' <summary>
		''' The cause of the exception.
		''' 
		''' <p>This field predates the general-purpose exception chaining facility.
		''' The <seealso cref="Throwable#getCause()"/> method is now the preferred means of
		''' obtaining this information.
		''' 
		''' @serial
		''' </summary>
		Public detail As Exception

		' indicate compatibility with JDK 1.1.x version of class 
		Private Shadows Const serialVersionUID As Long = 6617456357664815945L

		''' <summary>
		''' Constructs a <code>ServerCloneException</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="s"> the detail message. </param>
		Public Sub New(  s As String)
			MyBase.New(s)
			initCause(Nothing) ' Disallow subsequent initCause
		End Sub

		''' <summary>
		''' Constructs a <code>ServerCloneException</code> with the specified
		''' detail message and cause.
		''' </summary>
		''' <param name="s"> the detail message. </param>
		''' <param name="cause"> the cause </param>
		Public Sub New(  s As String,   cause As Exception)
			MyBase.New(s)
			initCause(Nothing) ' Disallow subsequent initCause
			detail = cause
		End Sub

		''' <summary>
		''' Returns the detail message, including the message from the cause, if
		''' any, of this exception.
		''' </summary>
		''' <returns> the detail message </returns>
		Public  Overrides ReadOnly Property  message As String
			Get
				If detail Is Nothing Then
					Return MyBase.message
				Else
					Return MyBase.message & "; nested exception is: " & vbLf & vbTab & detail.ToString()
				End If
			End Get
		End Property

		''' <summary>
		''' Returns the cause of this exception.  This method returns the value
		''' of the <seealso cref="#detail"/> field.
		''' </summary>
		''' <returns>  the cause, which may be <tt>null</tt>.
		''' @since   1.4 </returns>
		Public  Overrides ReadOnly Property  cause As Throwable
			Get
				Return detail
			End Get
		End Property
	End Class

End Namespace
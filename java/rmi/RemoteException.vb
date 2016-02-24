Imports Microsoft.VisualBasic

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

Namespace java.rmi

	''' <summary>
	''' A <code>RemoteException</code> is the common superclass for a number of
	''' communication-related exceptions that may occur during the execution of a
	''' remote method call.  Each method of a remote interface, an interface that
	''' extends <code>java.rmi.Remote</code>, must list
	''' <code>RemoteException</code> in its throws clause.
	''' 
	''' <p>As of release 1.4, this exception has been retrofitted to conform to
	''' the general purpose exception-chaining mechanism.  The "wrapped remote
	''' exception" that may be provided at construction time and accessed via
	''' the public <seealso cref="#detail"/> field is now known as the <i>cause</i>, and
	''' may be accessed via the <seealso cref="Throwable#getCause()"/> method, as well as
	''' the aforementioned "legacy field."
	''' 
	''' <p>Invoking the method <seealso cref="Throwable#initCause(Throwable)"/> on an
	''' instance of <code>RemoteException</code> always throws {@link
	''' IllegalStateException}.
	''' 
	''' @author  Ann Wollrath
	''' @since   JDK1.1
	''' </summary>
	Public Class RemoteException
		Inherits java.io.IOException

		' indicate compatibility with JDK 1.1.x version of class 
		Private Shadows Const serialVersionUID As Long = -5148567311918794206L

		''' <summary>
		''' The cause of the remote exception.
		''' 
		''' <p>This field predates the general-purpose exception chaining facility.
		''' The <seealso cref="Throwable#getCause()"/> method is now the preferred means of
		''' obtaining this information.
		''' 
		''' @serial
		''' </summary>
		Public detail As Throwable

		''' <summary>
		''' Constructs a <code>RemoteException</code>.
		''' </summary>
		Public Sub New()
			initCause(Nothing) ' Disallow subsequent initCause
		End Sub

		''' <summary>
		''' Constructs a <code>RemoteException</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="s"> the detail message </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
			initCause(Nothing) ' Disallow subsequent initCause
		End Sub

		''' <summary>
		''' Constructs a <code>RemoteException</code> with the specified detail
		''' message and cause.  This constructor sets the <seealso cref="#detail"/>
		''' field to the specified <code>Throwable</code>.
		''' </summary>
		''' <param name="s"> the detail message </param>
		''' <param name="cause"> the cause </param>
		Public Sub New(ByVal s As String, ByVal cause As Throwable)
			MyBase.New(s)
			initCause(Nothing) ' Disallow subsequent initCause
			detail = cause
		End Sub

		''' <summary>
		''' Returns the detail message, including the message from the cause, if
		''' any, of this exception.
		''' </summary>
		''' <returns> the detail message </returns>
		Public Property Overrides message As String
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
		Public Property Overrides cause As Throwable
			Get
				Return detail
			End Get
		End Property
	End Class

End Namespace
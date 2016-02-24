Imports System

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.security

	''' <summary>
	''' This exception is thrown by
	''' {@code doPrivileged(PrivilegedExceptionAction)} and
	''' {@code doPrivileged(PrivilegedExceptionAction,
	''' AccessControlContext context)} to indicate
	''' that the action being performed threw a checked exception.  The exception
	''' thrown by the action can be obtained by calling the
	''' {@code getException} method.  In effect, an
	''' {@code PrivilegedActionException} is a "wrapper"
	''' for an exception thrown by a privileged action.
	''' 
	''' <p>As of release 1.4, this exception has been retrofitted to conform to
	''' the general purpose exception-chaining mechanism.  The "exception thrown
	''' by the privileged computation" that is provided at construction time and
	''' accessed via the <seealso cref="#getException()"/> method is now known as the
	''' <i>cause</i>, and may be accessed via the <seealso cref="Throwable#getCause()"/>
	''' method, as well as the aforementioned "legacy method."
	''' </summary>
	''' <seealso cref= PrivilegedExceptionAction </seealso>
	''' <seealso cref= AccessController#doPrivileged(PrivilegedExceptionAction) </seealso>
	''' <seealso cref= AccessController#doPrivileged(PrivilegedExceptionAction,AccessControlContext) </seealso>
	Public Class PrivilegedActionException
		Inherits Exception

		' use serialVersionUID from JDK 1.2.2 for interoperability
		Private Shadows Const serialVersionUID As Long = 4724086851538908602L

		''' <summary>
		''' @serial
		''' </summary>
		Private exception_Renamed As Exception

		''' <summary>
		''' Constructs a new PrivilegedActionException &quot;wrapping&quot;
		''' the specific Exception.
		''' </summary>
		''' <param name="exception"> The exception thrown </param>
		Public Sub New(ByVal exception_Renamed As Exception)
			MyBase.New(CType(Nothing, Throwable)) ' Disallow initCause
			Me.exception_Renamed = exception_Renamed
		End Sub

		''' <summary>
		''' Returns the exception thrown by the privileged computation that
		''' resulted in this {@code PrivilegedActionException}.
		''' 
		''' <p>This method predates the general-purpose exception chaining facility.
		''' The <seealso cref="Throwable#getCause()"/> method is now the preferred means of
		''' obtaining this information.
		''' </summary>
		''' <returns> the exception thrown by the privileged computation that
		'''         resulted in this {@code PrivilegedActionException}. </returns>
		''' <seealso cref= PrivilegedExceptionAction </seealso>
		''' <seealso cref= AccessController#doPrivileged(PrivilegedExceptionAction) </seealso>
		''' <seealso cref= AccessController#doPrivileged(PrivilegedExceptionAction,
		'''                                            AccessControlContext) </seealso>
		Public Overridable Property exception As Exception
			Get
				Return exception_Renamed
			End Get
		End Property

		''' <summary>
		''' Returns the cause of this exception (the exception thrown by
		''' the privileged computation that resulted in this
		''' {@code PrivilegedActionException}).
		''' </summary>
		''' <returns>  the cause of this exception.
		''' @since   1.4 </returns>
		Public Property Overrides cause As Throwable
			Get
				Return exception_Renamed
			End Get
		End Property

		Public Overrides Function ToString() As String
			Dim s As String = Me.GetType().name
			Return If(exception_Renamed IsNot Nothing, (s & ": " & exception_Renamed.ToString()), s)
		End Function
	End Class

End Namespace
'
' * Copyright (c) 1996, 2004, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.reflect

	''' <summary>
	''' InvocationTargetException is a checked exception that wraps
	''' an exception thrown by an invoked method or constructor.
	''' 
	''' <p>As of release 1.4, this exception has been retrofitted to conform to
	''' the general purpose exception-chaining mechanism.  The "target exception"
	''' that is provided at construction time and accessed via the
	''' <seealso cref="#getTargetException()"/> method is now known as the <i>cause</i>,
	''' and may be accessed via the <seealso cref="Throwable#getCause()"/> method,
	''' as well as the aforementioned "legacy method."
	''' </summary>
	''' <seealso cref= Method </seealso>
	''' <seealso cref= Constructor </seealso>
	Public Class InvocationTargetException
		Inherits ReflectiveOperationException

		''' <summary>
		''' Use serialVersionUID from JDK 1.1.X for interoperability
		''' </summary>
		Private Shadows Const serialVersionUID As Long = 4085088731926701167L

		 ''' <summary>
		 ''' This field holds the target if the
		 ''' InvocationTargetException(Throwable target) constructor was
		 ''' used to instantiate the object
		 ''' 
		 ''' @serial
		 ''' 
		 ''' </summary>
		Private target As Throwable

		''' <summary>
		''' Constructs an {@code InvocationTargetException} with
		''' {@code null} as the target exception.
		''' </summary>
		Protected Friend Sub New()
			MyBase.New(CType(Nothing, Throwable)) ' Disallow initCause
		End Sub

		''' <summary>
		''' Constructs a InvocationTargetException with a target exception.
		''' </summary>
		''' <param name="target"> the target exception </param>
		Public Sub New(  target As Throwable)
			MyBase.New(CType(Nothing, Throwable)) ' Disallow initCause
			Me.target = target
		End Sub

		''' <summary>
		''' Constructs a InvocationTargetException with a target exception
		''' and a detail message.
		''' </summary>
		''' <param name="target"> the target exception </param>
		''' <param name="s">      the detail message </param>
		Public Sub New(  target As Throwable,   s As String)
			MyBase.New(s, Nothing) ' Disallow initCause
			Me.target = target
		End Sub

		''' <summary>
		''' Get the thrown target exception.
		''' 
		''' <p>This method predates the general-purpose exception chaining facility.
		''' The <seealso cref="Throwable#getCause()"/> method is now the preferred means of
		''' obtaining this information.
		''' </summary>
		''' <returns> the thrown target exception (cause of this exception). </returns>
		Public Overridable Property targetException As Throwable
			Get
				Return target
			End Get
		End Property

		''' <summary>
		''' Returns the cause of this exception (the thrown target exception,
		''' which may be {@code null}).
		''' </summary>
		''' <returns>  the cause of this exception.
		''' @since   1.4 </returns>
		Public  Overrides ReadOnly Property  cause As Throwable
			Get
				Return target
			End Get
		End Property
	End Class

End Namespace
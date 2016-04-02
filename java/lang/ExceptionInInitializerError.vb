'
' * Copyright (c) 1996, 2000, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang

	''' <summary>
	''' Signals that an unexpected exception has occurred in a static initializer.
	''' An <code>ExceptionInInitializerError</code> is thrown to indicate that an
	''' exception occurred during evaluation of a static initializer or the
	''' initializer for a static variable.
	''' 
	''' <p>As of release 1.4, this exception has been retrofitted to conform to
	''' the general purpose exception-chaining mechanism.  The "saved throwable
	''' object" that may be provided at construction time and accessed via
	''' the <seealso cref="#getException()"/> method is now known as the <i>cause</i>,
	''' and may be accessed via the <seealso cref="Throwable#getCause()"/> method, as well
	''' as the aforementioned "legacy method."
	''' 
	''' @author  Frank Yellin
	''' @since   JDK1.1
	''' </summary>
	Public Class ExceptionInInitializerError
		Inherits LinkageError

		''' <summary>
		''' Use serialVersionUID from JDK 1.1.X for interoperability
		''' </summary>
		Private Shadows Const serialVersionUID As Long = 1521711792217232256L

		''' <summary>
		''' This field holds the exception if the
		''' ExceptionInInitializerError(Throwable thrown) constructor was
		''' used to instantiate the object
		''' 
		''' @serial
		''' 
		''' </summary>
		Private exception_Renamed As Throwable

		''' <summary>
		''' Constructs an <code>ExceptionInInitializerError</code> with
		''' <code>null</code> as its detail message string and with no saved
		''' throwable object.
		''' A detail message is a String that describes this particular exception.
		''' </summary>
		Public Sub New()
			initCause(Nothing) ' Disallow subsequent initCause
		End Sub

		''' <summary>
		''' Constructs a new <code>ExceptionInInitializerError</code> class by
		''' saving a reference to the <code>Throwable</code> object thrown for
		''' later retrieval by the <seealso cref="#getException()"/> method. The detail
		''' message string is set to <code>null</code>.
		''' </summary>
		''' <param name="thrown"> The exception thrown </param>
		Public Sub New(ByVal thrown As Throwable)
			initCause(Nothing) ' Disallow subsequent initCause
			Me.exception_Renamed = thrown
		End Sub

		''' <summary>
		''' Constructs an ExceptionInInitializerError with the specified detail
		''' message string.  A detail message is a String that describes this
		''' particular exception. The detail message string is saved for later
		''' retrieval by the <seealso cref="Throwable#getMessage()"/> method. There is no
		''' saved throwable object.
		''' 
		''' </summary>
		''' <param name="s"> the detail message </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
			initCause(Nothing) ' Disallow subsequent initCause
		End Sub

		''' <summary>
		''' Returns the exception that occurred during a static initialization that
		''' caused this error to be created.
		''' 
		''' <p>This method predates the general-purpose exception chaining facility.
		''' The <seealso cref="Throwable#getCause()"/> method is now the preferred means of
		''' obtaining this information.
		''' </summary>
		''' <returns> the saved throwable object of this
		'''         <code>ExceptionInInitializerError</code>, or <code>null</code>
		'''         if this <code>ExceptionInInitializerError</code> has no saved
		'''         throwable object. </returns>
		Public Overridable Property exception As Throwable
			Get
				Return exception_Renamed
			End Get
		End Property

		''' <summary>
		''' Returns the cause of this error (the exception that occurred
		''' during a static initialization that caused this error to be created).
		''' </summary>
		''' <returns>  the cause of this error or <code>null</code> if the
		'''          cause is nonexistent or unknown.
		''' @since   1.4 </returns>
		Public  Overrides ReadOnly Property  cause As Throwable
			Get
				Return exception_Renamed
			End Get
		End Property
	End Class

End Namespace
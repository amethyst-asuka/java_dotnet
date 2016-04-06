Imports System

'
' * Copyright (c) 2000, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' Thrown to indicate that an assertion has failed.
	''' 
	''' <p>The seven one-argument public constructors provided by this
	''' class ensure that the assertion error returned by the invocation:
	''' <pre>
	'''     new AssertionError(<i>expression</i>)
	''' </pre>
	''' has as its detail message the <i>string conversion</i> of
	''' <i>expression</i> (as defined in section 15.18.1.1 of
	''' <cite>The Java&trade; Language Specification</cite>),
	''' regardless of the type of <i>expression</i>.
	''' 
	''' @since   1.4
	''' </summary>
	Public Class AssertionError
		Inherits [Error]

		Private Shadows Const serialVersionUID As Long = -5013299493970297370L

		''' <summary>
		''' Constructs an AssertionError with no detail message.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' This internal constructor does no processing on its string argument,
		''' even if it is a null reference.  The public constructors will
		''' never call this constructor with a null argument.
		''' </summary>
		Private Sub New(  detailMessage As String)
			MyBase.New(detailMessage)
		End Sub

		''' <summary>
		''' Constructs an AssertionError with its detail message derived
		''' from the specified object, which is converted to a string as
		''' defined in section 15.18.1.1 of
		''' <cite>The Java&trade; Language Specification</cite>.
		''' <p>
		''' If the specified object is an instance of {@code Throwable}, it
		''' becomes the <i>cause</i> of the newly constructed assertion error.
		''' </summary>
		''' <param name="detailMessage"> value to be used in constructing detail message </param>
		''' <seealso cref=   Throwable#getCause() </seealso>
		Public Sub New(  detailMessage As Object)
			Me.New(Convert.ToString(detailMessage))
			If TypeOf detailMessage Is Throwable Then initCause(CType(detailMessage, Throwable))
		End Sub

		''' <summary>
		''' Constructs an AssertionError with its detail message derived
		''' from the specified <code>boolean</code>, which is converted to
		''' a string as defined in section 15.18.1.1 of
		''' <cite>The Java&trade; Language Specification</cite>.
		''' </summary>
		''' <param name="detailMessage"> value to be used in constructing detail message </param>
		Public Sub New(  detailMessage As Boolean)
			Me.New(Convert.ToString(detailMessage))
		End Sub

		''' <summary>
		''' Constructs an AssertionError with its detail message derived
		''' from the specified <code>char</code>, which is converted to a
		''' string as defined in section 15.18.1.1 of
		''' <cite>The Java&trade; Language Specification</cite>.
		''' </summary>
		''' <param name="detailMessage"> value to be used in constructing detail message </param>
		Public Sub New(  detailMessage As Char)
			Me.New(Convert.ToString(detailMessage))
		End Sub

		''' <summary>
		''' Constructs an AssertionError with its detail message derived
		''' from the specified <code>int</code>, which is converted to a
		''' string as defined in section 15.18.1.1 of
		''' <cite>The Java&trade; Language Specification</cite>.
		''' </summary>
		''' <param name="detailMessage"> value to be used in constructing detail message </param>
		Public Sub New(  detailMessage As Integer)
			Me.New(Convert.ToString(detailMessage))
		End Sub

		''' <summary>
		''' Constructs an AssertionError with its detail message derived
		''' from the specified <code>long</code>, which is converted to a
		''' string as defined in section 15.18.1.1 of
		''' <cite>The Java&trade; Language Specification</cite>.
		''' </summary>
		''' <param name="detailMessage"> value to be used in constructing detail message </param>
		Public Sub New(  detailMessage As Long)
			Me.New(Convert.ToString(detailMessage))
		End Sub

		''' <summary>
		''' Constructs an AssertionError with its detail message derived
		''' from the specified <code>float</code>, which is converted to a
		''' string as defined in section 15.18.1.1 of
		''' <cite>The Java&trade; Language Specification</cite>.
		''' </summary>
		''' <param name="detailMessage"> value to be used in constructing detail message </param>
		Public Sub New(  detailMessage As Single)
			Me.New(Convert.ToString(detailMessage))
		End Sub

		''' <summary>
		''' Constructs an AssertionError with its detail message derived
		''' from the specified <code>double</code>, which is converted to a
		''' string as defined in section 15.18.1.1 of
		''' <cite>The Java&trade; Language Specification</cite>.
		''' </summary>
		''' <param name="detailMessage"> value to be used in constructing detail message </param>
		Public Sub New(  detailMessage As Double)
			Me.New(Convert.ToString(detailMessage))
		End Sub

		''' <summary>
		''' Constructs a new {@code AssertionError} with the specified
		''' detail message and cause.
		''' 
		''' <p>Note that the detail message associated with
		''' {@code cause} is <i>not</i> automatically incorporated in
		''' this error's detail message.
		''' </summary>
		''' <param name="message"> the detail message, may be {@code null} </param>
		''' <param name="cause"> the cause, may be {@code null}
		''' 
		''' @since 1.7 </param>
		Public Sub New(  message As String,   cause As Throwable)
			MyBase.New(message, cause)
		End Sub
	End Class

End Namespace
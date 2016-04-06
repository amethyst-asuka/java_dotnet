'
' * Copyright (c) 1995, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' Thrown when an application tries to load in a class through its
	''' string name using:
	''' <ul>
	''' <li>The <code>forName</code> method in class <code>Class</code>.
	''' <li>The <code>findSystemClass</code> method in class
	'''     <code>ClassLoader</code> .
	''' <li>The <code>loadClass</code> method in class <code>ClassLoader</code>.
	''' </ul>
	''' <p>
	''' but no definition for the class with the specified name could be found.
	''' 
	''' <p>As of release 1.4, this exception has been retrofitted to conform to
	''' the general purpose exception-chaining mechanism.  The "optional exception
	''' that was raised while loading the class" that may be provided at
	''' construction time and accessed via the <seealso cref="#getException()"/> method is
	''' now known as the <i>cause</i>, and may be accessed via the {@link
	''' Throwable#getCause()} method, as well as the aforementioned "legacy method."
	''' 
	''' @author  unascribed </summary>
	''' <seealso cref=     java.lang.Class#forName(java.lang.String) </seealso>
	''' <seealso cref=     java.lang.ClassLoader#findSystemClass(java.lang.String) </seealso>
	''' <seealso cref=     java.lang.ClassLoader#loadClass(java.lang.String, boolean)
	''' @since   JDK1.0 </seealso>
	Public Class ClassNotFoundException
		Inherits ReflectiveOperationException

		''' <summary>
		''' use serialVersionUID from JDK 1.1.X for interoperability
		''' </summary>
		 Private Shadows Const serialVersionUID As Long = 9176873029745254542L

		''' <summary>
		''' This field holds the exception ex if the
		''' ClassNotFoundException(String s, Throwable ex) constructor was
		''' used to instantiate the object
		''' @serial
		''' @since 1.2
		''' </summary>
		Private ex As Throwable

		''' <summary>
		''' Constructs a <code>ClassNotFoundException</code> with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New(CType(Nothing, Throwable)) ' Disallow initCause
		End Sub

		''' <summary>
		''' Constructs a <code>ClassNotFoundException</code> with the
		''' specified detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(  s As String)
			MyBase.New(s, Nothing) '  Disallow initCause
		End Sub

		''' <summary>
		''' Constructs a <code>ClassNotFoundException</code> with the
		''' specified detail message and optional exception that was
		''' raised while loading the class.
		''' </summary>
		''' <param name="s"> the detail message </param>
		''' <param name="ex"> the exception that was raised while loading the class
		''' @since 1.2 </param>
		Public Sub New(  s As String,   ex As Throwable)
			MyBase.New(s, Nothing) '  Disallow initCause
			Me.ex = ex
		End Sub

		''' <summary>
		''' Returns the exception that was raised if an error occurred while
		''' attempting to load the class. Otherwise, returns <tt>null</tt>.
		''' 
		''' <p>This method predates the general-purpose exception chaining facility.
		''' The <seealso cref="Throwable#getCause()"/> method is now the preferred means of
		''' obtaining this information.
		''' </summary>
		''' <returns> the <code>Exception</code> that was raised while loading a class
		''' @since 1.2 </returns>
		Public Overridable Property exception As Throwable
			Get
				Return ex
			End Get
		End Property

		''' <summary>
		''' Returns the cause of this exception (the exception that was raised
		''' if an error occurred while attempting to load the class; otherwise
		''' <tt>null</tt>).
		''' </summary>
		''' <returns>  the cause of this exception.
		''' @since   1.4 </returns>
		Public  Overrides ReadOnly Property  cause As Throwable
			Get
				Return ex
			End Get
		End Property
	End Class

End Namespace
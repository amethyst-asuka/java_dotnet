'
' * Copyright (c) 1995, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' Thrown when an application tries to use the Java <code>new</code>
	''' construct to instantiate an abstract class or an interface.
	''' <p>
	''' Normally, this error is caught by the compiler; this error can
	''' only occur at run time if the definition of a class has
	''' incompatibly changed.
	''' 
	''' @author  unascribed
	''' @since   JDK1.0
	''' </summary>


	Public Class InstantiationError
		Inherits IncompatibleClassChangeError

		Private Shadows Const serialVersionUID As Long = -4885810657349421204L

		''' <summary>
		''' Constructs an <code>InstantiationError</code> with no detail  message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an <code>InstantiationError</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace
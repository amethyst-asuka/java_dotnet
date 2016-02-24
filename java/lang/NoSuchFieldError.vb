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
	''' Thrown if an application tries to access or modify a specified
	''' field of an object, and that object no longer has that field.
	''' <p>
	''' Normally, this error is caught by the compiler; this error can
	''' only occur at run time if the definition of a class has
	''' incompatibly changed.
	''' 
	''' @author  unascribed
	''' @since   JDK1.0
	''' </summary>
	Public Class NoSuchFieldError
		Inherits IncompatibleClassChangeError

		Private Shadows Const serialVersionUID As Long = -3456430195886129035L

		''' <summary>
		''' Constructs a <code>NoSuchFieldError</code> with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a <code>NoSuchFieldError</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace
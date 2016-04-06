'
' * Copyright (c) 1994, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' Thrown when an application tries to call an abstract method.
	''' Normally, this error is caught by the compiler; this error can
	''' only occur at run time if the definition of some class has
	''' incompatibly changed since the currently executing method was last
	''' compiled.
	''' 
	''' @author  unascribed
	''' @since   JDK1.0
	''' </summary>
	Public Class AbstractMethodError
		Inherits IncompatibleClassChangeError

		Private Shadows Const serialVersionUID As Long = -1654391082989018462L

		''' <summary>
		''' Constructs an <code>AbstractMethodError</code> with no detail  message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an <code>AbstractMethodError</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace
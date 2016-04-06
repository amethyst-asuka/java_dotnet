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
	''' Thrown if an application attempts to access or modify a field, or
	''' to call a method that it does not have access to.
	''' <p>
	''' Normally, this error is caught by the compiler; this error can
	''' only occur at run time if the definition of a class has
	''' incompatibly changed.
	''' 
	''' @author  unascribed
	''' @since   JDK1.0
	''' </summary>
	Public Class IllegalAccessError
		Inherits IncompatibleClassChangeError

		Private Shadows Const serialVersionUID As Long = -8988904074992417891L

		''' <summary>
		''' Constructs an <code>IllegalAccessError</code> with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an <code>IllegalAccessError</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace
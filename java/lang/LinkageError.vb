'
' * Copyright (c) 1995, 2010, Oracle and/or its affiliates. All rights reserved.
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
	''' Subclasses of {@code LinkageError} indicate that a class has
	''' some dependency on another class; however, the latter class has
	''' incompatibly changed after the compilation of the former class.
	''' 
	''' 
	''' @author  Frank Yellin
	''' @since   JDK1.0
	''' </summary>
	Public Class LinkageError
		Inherits [Error]

		Private Shadows Const serialVersionUID As Long = 3579600108157160122L

		''' <summary>
		''' Constructs a {@code LinkageError} with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a {@code LinkageError} with the specified detail
		''' message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub

		''' <summary>
		''' Constructs a {@code LinkageError} with the specified detail
		''' message and cause.
		''' </summary>
		''' <param name="s">     the detail message. </param>
		''' <param name="cause"> the cause, may be {@code null}
		''' @since 1.7 </param>
		Public Sub New(ByVal s As String, ByVal cause As Throwable)
			MyBase.New(s, cause)
		End Sub
	End Class

End Namespace
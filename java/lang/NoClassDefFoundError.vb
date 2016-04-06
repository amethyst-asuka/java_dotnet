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
	''' Thrown if the Java Virtual Machine or a <code>ClassLoader</code> instance
	''' tries to load in the definition of a class (as part of a normal method call
	''' or as part of creating a new instance using the <code>new</code> expression)
	''' and no definition of the class could be found.
	''' <p>
	''' The searched-for class definition existed when the currently
	''' executing class was compiled, but the definition can no longer be
	''' found.
	''' 
	''' @author  unascribed
	''' @since   JDK1.0
	''' </summary>
	Public Class NoClassDefFoundError
		Inherits LinkageError

		Private Shadows Const serialVersionUID As Long = 9095859863287012458L

		''' <summary>
		''' Constructs a <code>NoClassDefFoundError</code> with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a <code>NoClassDefFoundError</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace
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
	''' Thrown when an incompatible class change has occurred to some class
	''' definition. The definition of some [Class], on which the currently
	''' executing method depends, has since changed.
	''' 
	''' @author  unascribed
	''' @since   JDK1.0
	''' </summary>
	Public Class IncompatibleClassChangeError
		Inherits LinkageError

		Private Shadows Const serialVersionUID As Long = -4914975503642802119L

		''' <summary>
		''' Constructs an <code>IncompatibleClassChangeError</code> with no
		''' detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an <code>IncompatibleClassChangeError</code> with the
		''' specified detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace
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
	''' Thrown to indicate that an index of some sort (such as to an array, to a
	''' string, or to a vector) is out of range.
	''' <p>
	''' Applications can subclass this class to indicate similar exceptions.
	''' 
	''' @author  Frank Yellin
	''' @since   JDK1.0
	''' </summary>
	Public Class IndexOutOfBoundsException
		Inherits RuntimeException

		Private Shadows Const serialVersionUID As Long = 234122996006267687L

		''' <summary>
		''' Constructs an <code>IndexOutOfBoundsException</code> with no
		''' detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an <code>IndexOutOfBoundsException</code> with the
		''' specified detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace
'
' * Copyright (c) 2003, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util

	''' <summary>
	''' Thrown by a <code>Scanner</code> to indicate that the token
	''' retrieved does not match the pattern for the expected type, or
	''' that the token is out of range for the expected type.
	''' 
	''' @author  unascribed </summary>
	''' <seealso cref=     java.util.Scanner
	''' @since   1.5 </seealso>
	Public Class InputMismatchException
		Inherits NoSuchElementException

		Private Shadows Const serialVersionUID As Long = 8811230760997066428L

		''' <summary>
		''' Constructs an <code>InputMismatchException</code> with <tt>null</tt>
		''' as its error message string.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an <code>InputMismatchException</code>, saving a reference
		''' to the error message string <tt>s</tt> for later retrieval by the
		''' <tt>getMessage</tt> method.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace
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

Namespace java.util

	''' <summary>
	''' Thrown by methods in the <code>Stack</code> class to indicate
	''' that the stack is empty.
	''' 
	''' @author  Jonathan Payne </summary>
	''' <seealso cref=     java.util.Stack
	''' @since   JDK1.0 </seealso>
	Public Class EmptyStackException
		Inherits RuntimeException

		Private Shadows Const serialVersionUID As Long = 5084686378493302095L

		''' <summary>
		''' Constructs a new <code>EmptyStackException</code> with <tt>null</tt>
		''' as its error message string.
		''' </summary>
		Public Sub New()
		End Sub
	End Class

End Namespace
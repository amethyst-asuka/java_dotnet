'
' * Copyright (c) 1994, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' Thrown by various accessor methods to indicate that the element being requested
	''' does not exist.
	''' 
	''' @author  unascribed </summary>
	''' <seealso cref=     java.util.Enumeration#nextElement() </seealso>
	''' <seealso cref=     java.util.Iterator#next()
	''' @since   JDK1.0 </seealso>
	Public Class NoSuchElementException
		Inherits RuntimeException

		Private Shadows Const serialVersionUID As Long = 6769829250639411880L

		''' <summary>
		''' Constructs a <code>NoSuchElementException</code> with <tt>null</tt>
		''' as its error message string.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a <code>NoSuchElementException</code>, saving a reference
		''' to the error message string <tt>s</tt> for later retrieval by the
		''' <tt>getMessage</tt> method.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace
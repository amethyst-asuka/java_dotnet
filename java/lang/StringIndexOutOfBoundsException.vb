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

Namespace java.lang

	''' <summary>
	''' Thrown by {@code String} methods to indicate that an index
	''' is either negative or greater than the size of the string.  For
	''' some methods such as the charAt method, this exception also is
	''' thrown when the index is equal to the size of the string.
	''' 
	''' @author  unascribed </summary>
	''' <seealso cref=     java.lang.String#charAt(int)
	''' @since   JDK1.0 </seealso>
	Public Class StringIndexOutOfBoundsException
		Inherits IndexOutOfBoundsException

		Private Shadows Const serialVersionUID As Long = -6762910422159637258L

		''' <summary>
		''' Constructs a {@code StringIndexOutOfBoundsException} with no
		''' detail message.
		''' 
		''' @since   JDK1.0.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a {@code StringIndexOutOfBoundsException} with
		''' the specified detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub

		''' <summary>
		''' Constructs a new {@code StringIndexOutOfBoundsException}
		''' class with an argument indicating the illegal index.
		''' </summary>
		''' <param name="index">   the illegal index. </param>
		Public Sub New(  index As Integer)
			MyBase.New("String index out of range: " & index)
		End Sub
	End Class

End Namespace
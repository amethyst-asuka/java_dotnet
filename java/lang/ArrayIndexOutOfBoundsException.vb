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
	''' Thrown to indicate that an array has been accessed with an
	''' illegal index. The index is either negative or greater than or
	''' equal to the size of the array.
	''' 
	''' @author  unascribed
	''' @since   JDK1.0
	''' </summary>
	Public Class ArrayIndexOutOfBoundsException
		Inherits IndexOutOfBoundsException

		Private Shadows Const serialVersionUID As Long = -5116101128118950844L

		''' <summary>
		''' Constructs an <code>ArrayIndexOutOfBoundsException</code> with no
		''' detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a new <code>ArrayIndexOutOfBoundsException</code>
		''' class with an argument indicating the illegal index.
		''' </summary>
		''' <param name="index">   the illegal index. </param>
		Public Sub New(ByVal index As Integer)
			MyBase.New("Array index out of range: " & index)
		End Sub

		''' <summary>
		''' Constructs an <code>ArrayIndexOutOfBoundsException</code> class
		''' with the specified detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace
'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.dnd

	''' <summary>
	''' This exception is thrown by various methods in the java.awt.dnd package.
	''' It is usually thrown to indicate that the target in question is unable
	''' to undertake the requested operation that the present time, since the
	''' underlying DnD system is not in the appropriate state.
	''' 
	''' @since 1.2
	''' </summary>

	Public Class InvalidDnDOperationException
		Inherits IllegalStateException

		Private Shadows Const serialVersionUID As Long = -6062568741193956678L

		Private Shared dft_msg As String = "The operation requested cannot be performed by the DnD system since it is not in the appropriate state"

		''' <summary>
		''' Create a default Exception
		''' </summary>

		Public Sub New()
			MyBase.New(dft_msg)
		End Sub

		''' <summary>
		''' Create an Exception with its own descriptive message
		''' <P> </summary>
		''' <param name="msg"> the detail message </param>

		Public Sub New(ByVal msg As String)
			MyBase.New(msg)
		End Sub

	End Class

End Namespace
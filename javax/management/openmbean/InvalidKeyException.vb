'
' * Copyright (c) 2000, 2007, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management.openmbean

	''' <summary>
	''' This runtime exception is thrown to indicate that a method parameter which was expected to be
	''' an item name of a <i>composite data</i> or a row index of a <i>tabular data</i> is not valid.
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Class InvalidKeyException
		Inherits System.ArgumentException

		Private Const serialVersionUID As Long = 4224269443946322062L

		''' <summary>
		''' An InvalidKeyException with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' An InvalidKeyException with a detail message.
		''' </summary>
		''' <param name="msg"> the detail message. </param>
		Public Sub New(ByVal msg As String)
			MyBase.New(msg)
		End Sub

	End Class

End Namespace
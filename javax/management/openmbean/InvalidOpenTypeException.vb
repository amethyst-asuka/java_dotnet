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
	''' This runtime exception is thrown to indicate that the <i>open type</i> of an <i>open data</i> value
	''' is not the one expected.
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Class InvalidOpenTypeException
		Inherits System.ArgumentException

		Private Const serialVersionUID As Long = -2837312755412327534L

		''' <summary>
		''' An InvalidOpenTypeException with no detail message. </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' An InvalidOpenTypeException with a detail message.
		''' </summary>
		''' <param name="msg"> the detail message. </param>
		Public Sub New(ByVal msg As String)
			MyBase.New(msg)
		End Sub

	End Class

End Namespace
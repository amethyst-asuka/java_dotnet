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
	''' This runtime exception is thrown to indicate that the index of a row to be added to a <i>tabular data</i> instance
	''' is already used to refer to another row in this <i>tabular data</i> instance.
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Class KeyAlreadyExistsException
		Inherits System.ArgumentException

		Private Const serialVersionUID As Long = 1845183636745282866L

		''' <summary>
		''' A KeyAlreadyExistsException with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' A KeyAlreadyExistsException with a detail message.
		''' </summary>
		''' <param name="msg"> the detail message. </param>
		Public Sub New(ByVal msg As String)
			MyBase.New(msg)
		End Sub

	End Class

End Namespace
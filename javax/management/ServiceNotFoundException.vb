'
' * Copyright (c) 1999, 2003, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management

	'RI import


	''' <summary>
	''' Represents exceptions raised when a requested service is not supported.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class ServiceNotFoundException
		Inherits javax.management.OperationsException

		' Serial version 
		Private Const serialVersionUID As Long = -3990675661956646827L

		''' <summary>
		''' Default constructor.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructor that allows a specific error message to be specified.
		''' </summary>
		''' <param name="message"> the detail message. </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

	End Class

End Namespace
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


	''' <summary>
	''' The specified attribute does not exist or cannot be retrieved.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class AttributeNotFoundException
		Inherits OperationsException

		' Serial version 
		Private Const serialVersionUID As Long = 6511584241791106926L

		''' <summary>
		''' Default constructor.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructor that allows a specific error message to be specified.
		''' </summary>
		''' <param name="message"> detail message. </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

	End Class

End Namespace
Imports System

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
	''' Exceptions thrown by JMX implementations.
	''' It does not include the runtime exceptions.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class JMException
		Inherits Exception

		' Serial version 
		Private Const serialVersionUID As Long = 350520924977331825L

		''' <summary>
		''' Default constructor.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructor that allows a specific error message to be specified.
		''' </summary>
		''' <param name="msg"> the detail message. </param>
		Public Sub New(ByVal msg As String)
			MyBase.New(msg)
		End Sub

	End Class

End Namespace
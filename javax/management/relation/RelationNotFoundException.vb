'
' * Copyright (c) 2000, 2003, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management.relation

	''' <summary>
	''' This exception is raised when there is no relation for a given relation id
	''' in a Relation Service.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class RelationNotFoundException
		Inherits RelationException

		' Serial version 
		Private Const serialVersionUID As Long = -3793951411158559116L

		''' <summary>
		''' Default constructor, no message put in exception.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructor with given message put in exception.
		''' </summary>
		''' <param name="message"> the detail message. </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub
	End Class

End Namespace
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
	''' This exception is raised when there is no relation type with given name in
	''' Relation Service.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class RelationTypeNotFoundException
		Inherits RelationException

		' Serial version 
		Private Const serialVersionUID As Long = 1274155316284300752L

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
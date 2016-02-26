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
	''' Invalid relation type.
	''' This exception is raised when, in a relation type, there is already a
	''' relation type with that name, or the same name has been used for two
	''' different role infos, or no role info provided, or one null role info
	''' provided.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class InvalidRelationTypeException
		Inherits RelationException

		' Serial version 
		Private Const serialVersionUID As Long = 3007446608299169961L

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
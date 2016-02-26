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
	''' This exception is raised when an access is done to the Relation Service and
	''' that one is not registered.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class RelationServiceNotRegisteredException
		Inherits RelationException

		' Serial version 
		Private Const serialVersionUID As Long = 8454744887157122910L

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
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
	''' Role value is invalid.
	''' This exception is raised when, in a role, the number of referenced MBeans
	''' in given value is less than expected minimum degree, or the number of
	''' referenced MBeans in provided value exceeds expected maximum degree, or
	''' one referenced MBean in the value is not an Object of the MBean
	''' class expected for that role, or an MBean provided for that role does not
	''' exist.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class InvalidRoleValueException
		Inherits RelationException

		' Serial version 
		Private Const serialVersionUID As Long = -2066091747301983721L

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
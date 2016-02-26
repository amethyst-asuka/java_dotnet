'
' * Copyright (c) 2000, 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' This class describes the various problems which can be encountered when
	''' accessing a role.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class RoleStatus

		'
		' Possible problems
		'

		''' <summary>
		''' Problem type when trying to access an unknown role.
		''' </summary>
		Public Const NO_ROLE_WITH_NAME As Integer = 1
		''' <summary>
		''' Problem type when trying to read a non-readable attribute.
		''' </summary>
		Public Const ROLE_NOT_READABLE As Integer = 2
		''' <summary>
		''' Problem type when trying to update a non-writable attribute.
		''' </summary>
		Public Const ROLE_NOT_WRITABLE As Integer = 3
		''' <summary>
		''' Problem type when trying to set a role value with less ObjectNames than
		''' the minimum expected cardinality.
		''' </summary>
		Public Const LESS_THAN_MIN_ROLE_DEGREE As Integer = 4
		''' <summary>
		''' Problem type when trying to set a role value with more ObjectNames than
		''' the maximum expected cardinality.
		''' </summary>
		Public Const MORE_THAN_MAX_ROLE_DEGREE As Integer = 5
		''' <summary>
		''' Problem type when trying to set a role value including the ObjectName of
		''' a MBean not of the class expected for that role.
		''' </summary>
		Public Const REF_MBEAN_OF_INCORRECT_CLASS As Integer = 6
		''' <summary>
		''' Problem type when trying to set a role value including the ObjectName of
		''' a MBean not registered in the MBean Server.
		''' </summary>
		Public Const REF_MBEAN_NOT_REGISTERED As Integer = 7

		''' <summary>
		''' Returns true if given value corresponds to a known role status, false
		''' otherwise.
		''' </summary>
		''' <param name="status"> a status code.
		''' </param>
		''' <returns> true if this value is a known role status. </returns>
		Public Shared Function isRoleStatus(ByVal status As Integer) As Boolean
			If status <> NO_ROLE_WITH_NAME AndAlso status <> ROLE_NOT_READABLE AndAlso status <> ROLE_NOT_WRITABLE AndAlso status <> LESS_THAN_MIN_ROLE_DEGREE AndAlso status <> MORE_THAN_MAX_ROLE_DEGREE AndAlso status <> REF_MBEAN_OF_INCORRECT_CLASS AndAlso status <> REF_MBEAN_NOT_REGISTERED Then Return False
			Return True
		End Function
	End Class

End Namespace
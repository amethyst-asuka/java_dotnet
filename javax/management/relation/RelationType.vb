Imports System.Collections.Generic

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
	''' The RelationType interface has to be implemented by any class expected to
	''' represent a relation type.
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface RelationType
		Inherits java.io.Serializable

		'
		' Accessors
		'

		''' <summary>
		''' Returns the relation type name.
		''' </summary>
		''' <returns> the relation type name. </returns>
		ReadOnly Property relationTypeName As String

		''' <summary>
		''' Returns the list of role definitions (ArrayList of RoleInfo objects).
		''' </summary>
		''' <returns> an <seealso cref="ArrayList"/> of <seealso cref="RoleInfo"/>. </returns>
		ReadOnly Property roleInfos As IList(Of RoleInfo)

		''' <summary>
		''' Returns the role info (RoleInfo object) for the given role info name
		''' (null if not found).
		''' </summary>
		''' <param name="roleInfoName">  role info name
		''' </param>
		''' <returns> RoleInfo object providing role definition
		''' does not exist
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RoleInfoNotFoundException">  if no role info with that name in
		''' relation type. </exception>
		Function getRoleInfo(ByVal roleInfoName As String) As RoleInfo
	End Interface

End Namespace
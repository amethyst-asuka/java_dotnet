Imports System

'
' * Copyright (c) 2000, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' Represents the result of a multiple access to several roles of a relation
	''' (either for reading or writing).
	''' 
	''' <p>The <b>serialVersionUID</b> of this class is <code>-6304063118040985512L</code>.
	''' 
	''' @since 1.5
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<Serializable> _
	Public Class RoleResult

		' Serialization compatibility stuff:
		' Two serial forms are supported in this class. The selected form depends
		' on system property "jmx.serial.form":
		'  - "1.0" for JMX 1.0
		'  - any other value for JMX 1.1 and higher
		'
		' Serial version for old serial form
		Private Const oldSerialVersionUID As Long = 3786616013762091099L
		'
		' Serial version for new serial form
		Private Const newSerialVersionUID As Long = -6304063118040985512L
		'
		' Serializable fields in old serial form
		Private Shared ReadOnly oldSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("myRoleList", GetType(RoleList)), New java.io.ObjectStreamField("myRoleUnresList", GetType(RoleUnresolvedList)) }
		'
		' Serializable fields in new serial form
		Private Shared ReadOnly newSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("roleList", GetType(RoleList)), New java.io.ObjectStreamField("unresolvedRoleList", GetType(RoleUnresolvedList)) }
		'
		' Actual serial version and serial form
		Private Shared ReadOnly serialVersionUID As Long
		''' <summary>
		''' @serialField roleList RoleList List of roles successfully accessed
		''' @serialField unresolvedRoleList RoleUnresolvedList List of roles unsuccessfully accessed
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField()
		Private Shared compat As Boolean = False
		Shared Sub New()
			Try
				Dim act As New com.sun.jmx.mbeanserver.GetPropertyAction("jmx.serial.form")
				Dim form As String = java.security.AccessController.doPrivileged(act)
				compat = (form IsNot Nothing AndAlso form.Equals("1.0"))
			Catch e As Exception
				' OK : Too bad, no compat with 1.0
			End Try
			If compat Then
				serialPersistentFields = oldSerialPersistentFields
				serialVersionUID = oldSerialVersionUID
			Else
				serialPersistentFields = newSerialPersistentFields
				serialVersionUID = newSerialVersionUID
			End If
		End Sub
		'
		' END Serialization compatibility stuff

		'
		' Private members
		'

		''' <summary>
		''' @serial List of roles successfully accessed
		''' </summary>
		Private roleList As RoleList = Nothing

		''' <summary>
		''' @serial List of roles unsuccessfully accessed
		''' </summary>
		Private unresolvedRoleList As RoleUnresolvedList = Nothing

		'
		' Constructor
		'

		''' <summary>
		''' Constructor.
		''' </summary>
		''' <param name="list">  list of roles successfully accessed. </param>
		''' <param name="unresolvedList">  list of roles not accessed (with problem
		''' descriptions). </param>
		Public Sub New(ByVal list As RoleList, ByVal unresolvedList As RoleUnresolvedList)

			roles = list
			rolesUnresolved = unresolvedList
			Return
		End Sub

		'
		' Accessors
		'

		''' <summary>
		''' Retrieves list of roles successfully accessed.
		''' </summary>
		''' <returns> a RoleList
		''' </returns>
		''' <seealso cref= #setRoles </seealso>
		Public Overridable Property roles As RoleList
			Get
				Return roleList
			End Get
			Set(ByVal list As RoleList)
				If list IsNot Nothing Then
    
					roleList = New RoleList
    
					Dim roleIter As IEnumerator(Of ?) = list.GetEnumerator()
	'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Do While roleIter.MoveNext()
						Dim currRole As Role = CType(roleIter.Current, Role)
						roleList.Add(CType(currRole.clone(), Role))
					Loop
				Else
					roleList = Nothing
				End If
				Return
			End Set
		End Property

		''' <summary>
		''' Retrieves list of roles unsuccessfully accessed.
		''' </summary>
		''' <returns> a RoleUnresolvedList.
		''' </returns>
		''' <seealso cref= #setRolesUnresolved </seealso>
		Public Overridable Property rolesUnresolved As RoleUnresolvedList
			Get
				Return unresolvedRoleList
			End Get
			Set(ByVal unresolvedList As RoleUnresolvedList)
				If unresolvedList IsNot Nothing Then
    
					unresolvedRoleList = New RoleUnresolvedList
    
					Dim roleUnresIter As IEnumerator(Of ?) = unresolvedList.GetEnumerator()
	'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Do While roleUnresIter.MoveNext()
						Dim currRoleUnres As RoleUnresolved = CType(roleUnresIter.Current, RoleUnresolved)
						unresolvedRoleList.Add(CType(currRoleUnres.clone(), RoleUnresolved))
					Loop
				Else
					unresolvedRoleList = Nothing
				End If
				Return
			End Set
		End Property



		''' <summary>
		''' Deserializes a <seealso cref="RoleResult"/> from an <seealso cref="ObjectInputStream"/>.
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
		  If compat Then
			' Read an object serialized in the old serial form
			'
			Dim fields As java.io.ObjectInputStream.GetField = [in].readFields()
			roleList = CType(fields.get("myRoleList", Nothing), RoleList)
			If fields.defaulted("myRoleList") Then Throw New NullPointerException("myRoleList")
			unresolvedRoleList = CType(fields.get("myRoleUnresList", Nothing), RoleUnresolvedList)
			If fields.defaulted("myRoleUnresList") Then Throw New NullPointerException("myRoleUnresList")
		  Else
			' Read an object serialized in the new serial form
			'
			[in].defaultReadObject()
		  End If
		End Sub


		''' <summary>
		''' Serializes a <seealso cref="RoleResult"/> to an <seealso cref="ObjectOutputStream"/>.
		''' </summary>
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
		  If compat Then
			' Serializes this instance in the old serial form
			'
			Dim fields As java.io.ObjectOutputStream.PutField = out.putFields()
			fields.put("myRoleList", roleList)
			fields.put("myRoleUnresList", unresolvedRoleList)
			out.writeFields()
		  Else
			' Serializes this instance in the new serial form
			'
			out.defaultWriteObject()
		  End If
		End Sub
	End Class

End Namespace
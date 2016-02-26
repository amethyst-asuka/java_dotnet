Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Text
import static com.sun.jmx.defaults.JmxProperties.RELATION_LOGGER
import static com.sun.jmx.mbeanserver.Util.cast

'
' * Copyright (c) 2000, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' A RelationTypeSupport object implements the RelationType interface.
	''' <P>It represents a relation type, providing role information for each role
	''' expected to be supported in every relation of that type.
	''' 
	''' <P>A relation type includes a relation type name and a list of
	''' role infos (represented by RoleInfo objects).
	''' 
	''' <P>A relation type has to be declared in the Relation Service:
	''' <P>- either using the createRelationType() method, where a RelationTypeSupport
	''' object will be created and kept in the Relation Service
	''' <P>- either using the addRelationType() method where the user has to create
	''' an object implementing the RelationType interface, and this object will be
	''' used as representing a relation type in the Relation Service.
	''' 
	''' <p>The <b>serialVersionUID</b> of this class is <code>4611072955724144607L</code>.
	''' 
	''' @since 1.5
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class RelationTypeSupport
		Implements RelationType ' serialVersionUID not constant

		' Serialization compatibility stuff:
		' Two serial forms are supported in this class. The selected form depends
		' on system property "jmx.serial.form":
		'  - "1.0" for JMX 1.0
		'  - any other value for JMX 1.1 and higher
		'
		' Serial version for old serial form
		Private Const oldSerialVersionUID As Long = -8179019472410837190L
		'
		' Serial version for new serial form
		Private Const newSerialVersionUID As Long = 4611072955724144607L
		'
		' Serializable fields in old serial form
		Private Shared ReadOnly oldSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("myTypeName", GetType(String)), New java.io.ObjectStreamField("myRoleName2InfoMap", GetType(Hashtable)), New java.io.ObjectStreamField("myIsInRelServFlg", GetType(Boolean)) }
		'
		' Serializable fields in new serial form
		Private Shared ReadOnly newSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("typeName", GetType(String)), New java.io.ObjectStreamField("roleName2InfoMap", GetType(IDictionary)), New java.io.ObjectStreamField("isInRelationService", GetType(Boolean)) }
		'
		' Actual serial version and serial form
		Private Shared ReadOnly serialVersionUID As Long
		''' <summary>
		''' @serialField typeName String Relation type name
		''' @serialField roleName2InfoMap Map <seealso cref="Map"/> holding the mapping:
		'''              &lt;role name (<seealso cref="String"/>)&gt; -&gt; &lt;role info (<seealso cref="RoleInfo"/> object)&gt;
		''' @serialField isInRelationService boolean Flag specifying whether the relation type has been declared in the
		'''              Relation Service (so can no longer be updated)
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
		''' @serial Relation type name
		''' </summary>
		Private typeName As String = Nothing

		''' <summary>
		''' @serial <seealso cref="Map"/> holding the mapping:
		'''           &lt;role name (<seealso cref="String"/>)&gt; -&gt; &lt;role info (<seealso cref="RoleInfo"/> object)&gt;
		''' </summary>
		Private roleName2InfoMap As IDictionary(Of String, RoleInfo) = New Dictionary(Of String, RoleInfo)

		''' <summary>
		''' @serial Flag specifying whether the relation type has been declared in the
		'''         Relation Service (so can no longer be updated)
		''' </summary>
		Private isInRelationService As Boolean = False

		'
		' Constructors
		'

		''' <summary>
		''' Constructor where all role definitions are dynamically created and
		''' passed as parameter.
		''' </summary>
		''' <param name="relationTypeName">  Name of relation type </param>
		''' <param name="roleInfoArray">  List of role definitions (RoleInfo objects)
		''' </param>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="InvalidRelationTypeException">  if:
		''' <P>- the same name has been used for two different roles
		''' <P>- no role info provided
		''' <P>- one null role info provided </exception>
		Public Sub New(ByVal relationTypeName As String, ByVal roleInfoArray As RoleInfo())

			If relationTypeName Is Nothing OrElse roleInfoArray Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationTypeSupport).name, "RelationTypeSupport", relationTypeName)

			' Can throw InvalidRelationTypeException, ClassNotFoundException
			' and NotCompliantMBeanException
			initMembers(relationTypeName, roleInfoArray)

			RELATION_LOGGER.exiting(GetType(RelationTypeSupport).name, "RelationTypeSupport")
			Return
		End Sub

		''' <summary>
		''' Constructor to be used for subclasses.
		''' </summary>
		''' <param name="relationTypeName">  Name of relation type.
		''' </param>
		''' <exception cref="IllegalArgumentException">  if null parameter. </exception>
		Protected Friend Sub New(ByVal relationTypeName As String)
			If relationTypeName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationTypeSupport).name, "RelationTypeSupport", relationTypeName)

			typeName = relationTypeName

			RELATION_LOGGER.exiting(GetType(RelationTypeSupport).name, "RelationTypeSupport")
			Return
		End Sub

		'
		' Accessors
		'

		''' <summary>
		''' Returns the relation type name.
		''' </summary>
		''' <returns> the relation type name. </returns>
		Public Overridable Property relationTypeName As String Implements RelationType.getRelationTypeName
			Get
				Return typeName
			End Get
		End Property

		''' <summary>
		''' Returns the list of role definitions (ArrayList of RoleInfo objects).
		''' </summary>
		Public Overridable Property roleInfos As IList(Of RoleInfo) Implements RelationType.getRoleInfos
			Get
				Return New List(Of RoleInfo)(roleName2InfoMap.Values)
			End Get
		End Property

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
		Public Overridable Function getRoleInfo(ByVal roleInfoName As String) As RoleInfo Implements RelationType.getRoleInfo

			If roleInfoName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationTypeSupport).name, "getRoleInfo", roleInfoName)

			' No null RoleInfo allowed, so use get()
			Dim result As RoleInfo = roleName2InfoMap(roleInfoName)

			If result Is Nothing Then
				Dim excMsgStrB As New StringBuilder
				Dim excMsg As String = "No role info for role "
				excMsgStrB.Append(excMsg)
				excMsgStrB.Append(roleInfoName)
				Throw New RoleInfoNotFoundException(excMsgStrB.ToString())
			End If

			RELATION_LOGGER.exiting(GetType(RelationTypeSupport).name, "getRoleInfo")
			Return result
		End Function

		'
		' Misc
		'

		''' <summary>
		''' Add a role info.
		''' This method of course should not be used after the creation of the
		''' relation type, because updating it would invalidate that the relations
		''' created associated to that type still conform to it.
		''' Can throw a RuntimeException if trying to update a relation type
		''' declared in the Relation Service.
		''' </summary>
		''' <param name="roleInfo">  role info to be added.
		''' </param>
		''' <exception cref="IllegalArgumentException">  if null parameter. </exception>
		''' <exception cref="InvalidRelationTypeException">  if there is already a role
		'''  info in current relation type with the same name. </exception>
		Protected Friend Overridable Sub addRoleInfo(ByVal ___roleInfo As RoleInfo)

			If ___roleInfo Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationTypeSupport).name, "addRoleInfo", ___roleInfo)

			If isInRelationService Then
				' Trying to update a declared relation type
				Dim excMsg As String = "Relation type cannot be updated as it is declared in the Relation Service."
				Throw New Exception(excMsg)
			End If

			Dim roleName As String = ___roleInfo.name

			' Checks if the role info has already been described
			If roleName2InfoMap.ContainsKey(roleName) Then
				Dim excMsgStrB As New StringBuilder
				Dim excMsg As String = "Two role infos provided for role "
				excMsgStrB.Append(excMsg)
				excMsgStrB.Append(roleName)
				Throw New InvalidRelationTypeException(excMsgStrB.ToString())
			End If

			roleName2InfoMap(roleName) = New RoleInfo(___roleInfo)

			RELATION_LOGGER.exiting(GetType(RelationTypeSupport).name, "addRoleInfo")
			Return
		End Sub

		' Sets the internal flag to specify that the relation type has been
		' declared in the Relation Service
		Friend Overridable Property relationServiceFlag As Boolean
			Set(ByVal flag As Boolean)
				isInRelationService = flag
				Return
			End Set
		End Property

		' Initializes the members, i.e. type name and role info list.
		'
		' -param relationTypeName  Name of relation type
		' -param roleInfoArray  List of role definitions (RoleInfo objects)
		'
		' -exception IllegalArgumentException  if null parameter
		' -exception InvalidRelationTypeException  If:
		'  - the same name has been used for two different roles
		'  - no role info provided
		'  - one null role info provided
		Private Sub initMembers(ByVal relationTypeName As String, ByVal roleInfoArray As RoleInfo())

			If relationTypeName Is Nothing OrElse roleInfoArray Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationTypeSupport).name, "initMembers", relationTypeName)

			typeName = relationTypeName

			' Verifies role infos before setting them
			' Can throw InvalidRelationTypeException
			checkRoleInfos(roleInfoArray)

			For i As Integer = 0 To roleInfoArray.Length - 1
				Dim currRoleInfo As RoleInfo = roleInfoArray(i)
				roleName2InfoMap(currRoleInfo.name) = New RoleInfo(currRoleInfo)
			Next i

			RELATION_LOGGER.exiting(GetType(RelationTypeSupport).name, "initMembers")
			Return
		End Sub

		' Checks the given RoleInfo array to verify that:
		' - the array is not empty
		' - it does not contain a null element
		' - a given role name is used only for one RoleInfo
		'
		' -param roleInfoArray  array to be checked
		'
		' -exception IllegalArgumentException
		' -exception InvalidRelationTypeException  If:
		'  - the same name has been used for two different roles
		'  - no role info provided
		'  - one null role info provided
		Friend Shared Sub checkRoleInfos(ByVal roleInfoArray As RoleInfo())

			If roleInfoArray Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			If roleInfoArray.Length = 0 Then
				' No role info provided
				Dim excMsg As String = "No role info provided."
				Throw New InvalidRelationTypeException(excMsg)
			End If


			Dim roleNames As java.util.Set(Of String) = New HashSet(Of String)

			For i As Integer = 0 To roleInfoArray.Length - 1
				Dim currRoleInfo As RoleInfo = roleInfoArray(i)

				If currRoleInfo Is Nothing Then
					Dim excMsg As String = "Null role info provided."
					Throw New InvalidRelationTypeException(excMsg)
				End If

				Dim roleName As String = currRoleInfo.name

				' Checks if the role info has already been described
				If roleNames.contains(roleName) Then
					Dim excMsgStrB As New StringBuilder
					Dim excMsg As String = "Two role infos provided for role "
					excMsgStrB.Append(excMsg)
					excMsgStrB.Append(roleName)
					Throw New InvalidRelationTypeException(excMsgStrB.ToString())
				End If
				roleNames.add(roleName)
			Next i

			Return
		End Sub


		''' <summary>
		''' Deserializes a <seealso cref="RelationTypeSupport"/> from an <seealso cref="ObjectInputStream"/>.
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
		  If compat Then
			' Read an object serialized in the old serial form
			'
			Dim fields As java.io.ObjectInputStream.GetField = [in].readFields()
			typeName = CStr(fields.get("myTypeName", Nothing))
			If fields.defaulted("myTypeName") Then Throw New NullPointerException("myTypeName")
			roleName2InfoMap = cast(fields.get("myRoleName2InfoMap", Nothing))
			If fields.defaulted("myRoleName2InfoMap") Then Throw New NullPointerException("myRoleName2InfoMap")
			isInRelationService = fields.get("myIsInRelServFlg", False)
			If fields.defaulted("myIsInRelServFlg") Then Throw New NullPointerException("myIsInRelServFlg")
		  Else
			' Read an object serialized in the new serial form
			'
			[in].defaultReadObject()
		  End If
		End Sub


		''' <summary>
		''' Serializes a <seealso cref="RelationTypeSupport"/> to an <seealso cref="ObjectOutputStream"/>.
		''' </summary>
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
		  If compat Then
			' Serializes this instance in the old serial form
			'
			Dim fields As java.io.ObjectOutputStream.PutField = out.putFields()
			fields.put("myTypeName", typeName)
			fields.put("myRoleName2InfoMap", roleName2InfoMap)
			fields.put("myIsInRelServFlg", isInRelationService)
			out.writeFields()
		  Else
			' Serializes this instance in the new serial form
			'
			out.defaultWriteObject()
		  End If
		End Sub
	End Class

End Namespace
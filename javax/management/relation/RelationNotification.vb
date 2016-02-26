Imports System
Imports System.Collections
Imports System.Collections.Generic
import static com.sun.jmx.mbeanserver.Util.cast

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A notification of a change in the Relation Service.
	''' A RelationNotification notification is sent when a relation is created via
	''' the Relation Service, or an MBean is added as a relation in the Relation
	''' Service, or a role is updated in a relation, or a relation is removed from
	''' the Relation Service.
	''' 
	''' <p>The <b>serialVersionUID</b> of this class is <code>-6871117877523310399L</code>.
	''' 
	''' @since 1.5
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class RelationNotification
		Inherits javax.management.Notification ' serialVersionUID not constant

		' Serialization compatibility stuff:
		' Two serial forms are supported in this class. The selected form depends
		' on system property "jmx.serial.form":
		'  - "1.0" for JMX 1.0
		'  - any other value for JMX 1.1 and higher
		'
		' Serial version for old serial form
		Private Const oldSerialVersionUID As Long = -2126464566505527147L
		'
		' Serial version for new serial form
		Private Const newSerialVersionUID As Long = -6871117877523310399L
		'
		' Serializable fields in old serial form
		Private Shared ReadOnly oldSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("myNewRoleValue", GetType(ArrayList)), New java.io.ObjectStreamField("myOldRoleValue", GetType(ArrayList)), New java.io.ObjectStreamField("myRelId", GetType(String)), New java.io.ObjectStreamField("myRelObjName", GetType(javax.management.ObjectName)), New java.io.ObjectStreamField("myRelTypeName", GetType(String)), New java.io.ObjectStreamField("myRoleName", GetType(String)), New java.io.ObjectStreamField("myUnregMBeanList", GetType(ArrayList)) }
		'
		' Serializable fields in new serial form
		Private Shared ReadOnly newSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("newRoleValue", GetType(IList)), New java.io.ObjectStreamField("oldRoleValue", GetType(IList)), New java.io.ObjectStreamField("relationId", GetType(String)), New java.io.ObjectStreamField("relationObjName", GetType(javax.management.ObjectName)), New java.io.ObjectStreamField("relationTypeName", GetType(String)), New java.io.ObjectStreamField("roleName", GetType(String)), New java.io.ObjectStreamField("unregisterMBeanList", GetType(IList)) }
		'
		' Actual serial version and serial form
		Private Shared ReadOnly serialVersionUID As Long
		''' <summary>
		''' @serialField relationId String Relation identifier of
		''' created/removed/updated relation
		''' @serialField relationTypeName String Relation type name of
		''' created/removed/updated relation
		''' @serialField relationObjName ObjectName <seealso cref="ObjectName"/> of
		''' the relation MBean of created/removed/updated relation (only if
		''' the relation is represented by an MBean)
		''' @serialField unregisterMBeanList List List of {@link
		''' ObjectName}s of referenced MBeans to be unregistered due to
		''' relation removal
		''' @serialField roleName String Name of updated role (only for role update)
		''' @serialField oldRoleValue List Old role value ({@link
		''' ArrayList} of <seealso cref="ObjectName"/>s) (only for role update)
		''' @serialField newRoleValue List New role value ({@link
		''' ArrayList} of <seealso cref="ObjectName"/>s) (only for role update)
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
		' Notification types
		'

		''' <summary>
		''' Type for the creation of an internal relation.
		''' </summary>
		Public Const RELATION_BASIC_CREATION As String = "jmx.relation.creation.basic"
		''' <summary>
		''' Type for the relation MBean added into the Relation Service.
		''' </summary>
		Public Const RELATION_MBEAN_CREATION As String = "jmx.relation.creation.mbean"
		''' <summary>
		''' Type for an update of an internal relation.
		''' </summary>
		Public Const RELATION_BASIC_UPDATE As String = "jmx.relation.update.basic"
		''' <summary>
		''' Type for the update of a relation MBean.
		''' </summary>
		Public Const RELATION_MBEAN_UPDATE As String = "jmx.relation.update.mbean"
		''' <summary>
		''' Type for the removal from the Relation Service of an internal relation.
		''' </summary>
		Public Const RELATION_BASIC_REMOVAL As String = "jmx.relation.removal.basic"
		''' <summary>
		''' Type for the removal from the Relation Service of a relation MBean.
		''' </summary>
		Public Const RELATION_MBEAN_REMOVAL As String = "jmx.relation.removal.mbean"

		'
		' Private members
		'

		''' <summary>
		''' @serial Relation identifier of created/removed/updated relation
		''' </summary>
		Private relationId As String = Nothing

		''' <summary>
		''' @serial Relation type name of created/removed/updated relation
		''' </summary>
		Private relationTypeName As String = Nothing

		''' <summary>
		''' @serial <seealso cref="ObjectName"/> of the relation MBean of created/removed/updated relation
		'''         (only if the relation is represented by an MBean)
		''' </summary>
		Private relationObjName As javax.management.ObjectName = Nothing

		''' <summary>
		''' @serial List of <seealso cref="ObjectName"/>s of referenced MBeans to be unregistered due to
		'''         relation removal
		''' </summary>
		Private unregisterMBeanList As IList(Of javax.management.ObjectName) = Nothing

		''' <summary>
		''' @serial Name of updated role (only for role update)
		''' </summary>
		Private roleName As String = Nothing

		''' <summary>
		''' @serial Old role value (<seealso cref="ArrayList"/> of <seealso cref="ObjectName"/>s) (only for role update)
		''' </summary>
		Private oldRoleValue As IList(Of javax.management.ObjectName) = Nothing

		''' <summary>
		''' @serial New role value (<seealso cref="ArrayList"/> of <seealso cref="ObjectName"/>s) (only for role update)
		''' </summary>
		Private newRoleValue As IList(Of javax.management.ObjectName) = Nothing

		'
		' Constructors
		'

		''' <summary>
		''' Creates a notification for either a relation creation (RelationSupport
		''' object created internally in the Relation Service, or an MBean added as a
		''' relation) or for a relation removal from the Relation Service.
		''' </summary>
		''' <param name="notifType">  type of the notification; either:
		''' <P>- RELATION_BASIC_CREATION
		''' <P>- RELATION_MBEAN_CREATION
		''' <P>- RELATION_BASIC_REMOVAL
		''' <P>- RELATION_MBEAN_REMOVAL </param>
		''' <param name="sourceObj">  source object, sending the notification.  This is either
		''' an ObjectName or a RelationService object.  In the latter case it must be
		''' the MBean emitting the notification; the MBean Server will rewrite the
		''' source to be the ObjectName under which that MBean is registered. </param>
		''' <param name="sequence">  sequence number to identify the notification </param>
		''' <param name="timeStamp">  time stamp </param>
		''' <param name="message">  human-readable message describing the notification </param>
		''' <param name="id">  relation id identifying the relation in the Relation
		''' Service </param>
		''' <param name="typeName">  name of the relation type </param>
		''' <param name="objectName">  ObjectName of the relation object if it is an MBean
		''' (null for relations internally handled by the Relation Service) </param>
		''' <param name="unregMBeanList">  list of ObjectNames of referenced MBeans
		''' expected to be unregistered due to relation removal (only for removal,
		''' due to CIM qualifiers, can be null)
		''' </param>
		''' <exception cref="IllegalArgumentException">  if:
		''' <P>- no value for the notification type
		''' <P>- the notification type is not RELATION_BASIC_CREATION,
		''' RELATION_MBEAN_CREATION, RELATION_BASIC_REMOVAL or
		''' RELATION_MBEAN_REMOVAL
		''' <P>- no source object
		''' <P>- the source object is not a Relation Service
		''' <P>- no relation id
		''' <P>- no relation type name </exception>
		Public Sub New(ByVal notifType As String, ByVal sourceObj As Object, ByVal sequence As Long, ByVal timeStamp As Long, ByVal message As String, ByVal id As String, ByVal typeName As String, ByVal ___objectName As javax.management.ObjectName, ByVal unregMBeanList As IList(Of javax.management.ObjectName))

			MyBase.New(notifType, sourceObj, sequence, timeStamp, message)

			If (Not isValidBasicStrict(notifType,sourceObj,id,typeName)) OrElse (Not isValidCreate(notifType)) Then Throw New System.ArgumentException("Invalid parameter.")

			relationId = id
			relationTypeName = typeName
			relationObjName = safeGetObjectName(___objectName)
			unregisterMBeanList = safeGetObjectNameList(unregMBeanList)
		End Sub

		''' <summary>
		''' Creates a notification for a role update in a relation.
		''' </summary>
		''' <param name="notifType">  type of the notification; either:
		''' <P>- RELATION_BASIC_UPDATE
		''' <P>- RELATION_MBEAN_UPDATE </param>
		''' <param name="sourceObj">  source object, sending the notification. This is either
		''' an ObjectName or a RelationService object.  In the latter case it must be
		''' the MBean emitting the notification; the MBean Server will rewrite the
		''' source to be the ObjectName under which that MBean is registered. </param>
		''' <param name="sequence">  sequence number to identify the notification </param>
		''' <param name="timeStamp">  time stamp </param>
		''' <param name="message">  human-readable message describing the notification </param>
		''' <param name="id">  relation id identifying the relation in the Relation
		''' Service </param>
		''' <param name="typeName">  name of the relation type </param>
		''' <param name="objectName">  ObjectName of the relation object if it is an MBean
		''' (null for relations internally handled by the Relation Service) </param>
		''' <param name="name">  name of the updated role </param>
		''' <param name="newValue">  new role value (List of ObjectName objects) </param>
		''' <param name="oldValue">  old role value (List of ObjectName objects)
		''' </param>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		Public Sub New(ByVal notifType As String, ByVal sourceObj As Object, ByVal sequence As Long, ByVal timeStamp As Long, ByVal message As String, ByVal id As String, ByVal typeName As String, ByVal ___objectName As javax.management.ObjectName, ByVal name As String, ByVal newValue As IList(Of javax.management.ObjectName), ByVal oldValue As IList(Of javax.management.ObjectName))

			MyBase.New(notifType, sourceObj, sequence, timeStamp, message)

			If (Not isValidBasicStrict(notifType,sourceObj,id,typeName)) OrElse (Not isValidUpdate(notifType,name,newValue,oldValue)) Then Throw New System.ArgumentException("Invalid parameter.")

			relationId = id
			relationTypeName = typeName
			relationObjName = safeGetObjectName(___objectName)

			roleName = name
			oldRoleValue = safeGetObjectNameList(oldValue)
			newRoleValue = safeGetObjectNameList(newValue)
		End Sub

		'
		' Accessors
		'

		''' <summary>
		''' Returns the relation identifier of created/removed/updated relation.
		''' </summary>
		''' <returns> the relation id. </returns>
		Public Overridable Property relationId As String
			Get
				Return relationId
			End Get
		End Property

		''' <summary>
		''' Returns the relation type name of created/removed/updated relation.
		''' </summary>
		''' <returns> the relation type name. </returns>
		Public Overridable Property relationTypeName As String
			Get
				Return relationTypeName
			End Get
		End Property

		''' <summary>
		''' Returns the ObjectName of the
		''' created/removed/updated relation.
		''' </summary>
		''' <returns> the ObjectName if the relation is an MBean, otherwise null. </returns>
		Public Overridable Property objectName As javax.management.ObjectName
			Get
				Return relationObjName
			End Get
		End Property

		''' <summary>
		''' Returns the list of ObjectNames of MBeans expected to be unregistered
		''' due to a relation removal (only for relation removal).
		''' </summary>
		''' <returns> a <seealso cref="List"/> of <seealso cref="ObjectName"/>. </returns>
		Public Overridable Property mBeansToUnregister As IList(Of javax.management.ObjectName)
			Get
				Dim result As IList(Of javax.management.ObjectName)
				If unregisterMBeanList IsNot Nothing Then
					result = New List(Of javax.management.ObjectName)(unregisterMBeanList)
				Else
					result = java.util.Collections.emptyList()
				End If
				Return result
			End Get
		End Property

		''' <summary>
		''' Returns name of updated role of updated relation (only for role update).
		''' </summary>
		''' <returns> the name of the updated role. </returns>
		Public Overridable Property roleName As String
			Get
				Dim result As String = Nothing
				If roleName IsNot Nothing Then result = roleName
				Return result
			End Get
		End Property

		''' <summary>
		''' Returns old value of updated role (only for role update).
		''' </summary>
		''' <returns> the old value of the updated role. </returns>
		Public Overridable Property oldRoleValue As IList(Of javax.management.ObjectName)
			Get
				Dim result As IList(Of javax.management.ObjectName)
				If oldRoleValue IsNot Nothing Then
					result = New List(Of javax.management.ObjectName)(oldRoleValue)
				Else
					result = java.util.Collections.emptyList()
				End If
				Return result
			End Get
		End Property

		''' <summary>
		''' Returns new value of updated role (only for role update).
		''' </summary>
		''' <returns> the new value of the updated role. </returns>
		Public Overridable Property newRoleValue As IList(Of javax.management.ObjectName)
			Get
				Dim result As IList(Of javax.management.ObjectName)
				If newRoleValue IsNot Nothing Then
					result = New List(Of javax.management.ObjectName)(newRoleValue)
				Else
					result = java.util.Collections.emptyList()
				End If
				Return result
			End Get
		End Property

		'
		' Misc
		'

		' Initializes members
		'
		' -param notifKind  1 for creation/removal, 2 for update
		' -param notifType  type of the notification; either:
		'  - RELATION_BASIC_UPDATE
		'  - RELATION_MBEAN_UPDATE
		'  for an update, or:
		'  - RELATION_BASIC_CREATION
		'  - RELATION_MBEAN_CREATION
		'  - RELATION_BASIC_REMOVAL
		'  - RELATION_MBEAN_REMOVAL
		'  for a creation or removal
		' -param sourceObj  source object, sending the notification. Will always
		'  be a RelationService object.
		' -param sequence  sequence number to identify the notification
		' -param timeStamp  time stamp
		' -param message  human-readable message describing the notification
		' -param id  relation id identifying the relation in the Relation
		'  Service
		' -param typeName  name of the relation type
		' -param objectName  ObjectName of the relation object if it is an MBean
		'  (null for relations internally handled by the Relation Service)
		' -param unregMBeanList  list of ObjectNames of MBeans expected to be
		'  removed due to relation removal
		' -param name  name of the updated role
		' -param newValue  new value (List of ObjectName objects)
		' -param oldValue  old value (List of ObjectName objects)
		'
		' -exception IllegalArgumentException  if:
		'  - no value for the notification type
		'  - incorrect notification type
		'  - no source object
		'  - the source object is not a Relation Service
		'  - no relation id
		'  - no relation type name
		'  - no role name (for role update)
		'  - no role old value (for role update)
		'  - no role new value (for role update)

		' Despite the fact, that validation in constructor of RelationNotification prohibit
		' creation of the class instance with null sourceObj its possible to set it to null later
		' by public setSource() method.
		' So we should relax validation rules to preserve serialization behavior compatibility.

		Private Function isValidBasicStrict(ByVal notifType As String, ByVal sourceObj As Object, ByVal id As String, ByVal typeName As String) As Boolean
			If sourceObj Is Nothing Then Return False
			Return isValidBasic(notifType,sourceObj,id,typeName)
		End Function

		Private Function isValidBasic(ByVal notifType As String, ByVal sourceObj As Object, ByVal id As String, ByVal typeName As String) As Boolean
			If notifType Is Nothing OrElse id Is Nothing OrElse typeName Is Nothing Then Return False

			If sourceObj IsNot Nothing AndAlso (Not(TypeOf sourceObj Is RelationService) AndAlso Not(TypeOf sourceObj Is javax.management.ObjectName)) Then Return False

			Return True
		End Function

		Private Function isValidCreate(ByVal notifType As String) As Boolean
			Dim validTypes As String()= {RelationNotification.RELATION_BASIC_CREATION, RelationNotification.RELATION_MBEAN_CREATION, RelationNotification.RELATION_BASIC_REMOVAL, RelationNotification.RELATION_MBEAN_REMOVAL}

			Dim ctSet As java.util.Set(Of String) = New HashSet(Of String)(java.util.Arrays.asList(validTypes))
			Return ctSet.contains(notifType)
		End Function

		Private Function isValidUpdate(ByVal notifType As String, ByVal name As String, ByVal newValue As IList(Of javax.management.ObjectName), ByVal oldValue As IList(Of javax.management.ObjectName)) As Boolean

			If Not(notifType.Equals(RelationNotification.RELATION_BASIC_UPDATE)) AndAlso Not(notifType.Equals(RelationNotification.RELATION_MBEAN_UPDATE)) Then Return False

			If name Is Nothing OrElse oldValue Is Nothing OrElse newValue Is Nothing Then Return False

			Return True
		End Function

		Private Function safeGetObjectNameList(ByVal src As IList(Of javax.management.ObjectName)) As List(Of javax.management.ObjectName)
			Dim dest As List(Of javax.management.ObjectName) = Nothing
			If src IsNot Nothing Then
				dest = New List(Of javax.management.ObjectName)
				For Each item As javax.management.ObjectName In src
					' NPE thrown if we attempt to add null object
					dest.Add(javax.management.ObjectName.getInstance(item))
				Next item
			End If
			Return dest
		End Function

		Private Function safeGetObjectName(ByVal src As javax.management.ObjectName) As javax.management.ObjectName
			Dim dest As javax.management.ObjectName = Nothing
			If src IsNot Nothing Then dest = javax.management.ObjectName.getInstance(src)
			Return dest
		End Function

		''' <summary>
		''' Deserializes a <seealso cref="RelationNotification"/> from an <seealso cref="ObjectInputStream"/>.
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)

			Dim tmpRelationId, tmpRelationTypeName, tmpRoleName As String

			Dim tmpRelationObjName As javax.management.ObjectName
			Dim tmpNewRoleValue As IList(Of javax.management.ObjectName), tmpOldRoleValue As IList(Of javax.management.ObjectName), tmpUnregMBeanList As IList(Of javax.management.ObjectName)

			Dim fields As java.io.ObjectInputStream.GetField = [in].readFields()

			If compat Then
				tmpRelationId = CStr(fields.get("myRelId", Nothing))
				tmpRelationTypeName = CStr(fields.get("myRelTypeName", Nothing))
				tmpRoleName = CStr(fields.get("myRoleName", Nothing))

				tmpRelationObjName = CType(fields.get("myRelObjName", Nothing), javax.management.ObjectName)
				tmpNewRoleValue = cast(fields.get("myNewRoleValue", Nothing))
				tmpOldRoleValue = cast(fields.get("myOldRoleValue", Nothing))
				tmpUnregMBeanList = cast(fields.get("myUnregMBeanList", Nothing))
			Else
				tmpRelationId = CStr(fields.get("relationId", Nothing))
				tmpRelationTypeName = CStr(fields.get("relationTypeName", Nothing))
				tmpRoleName = CStr(fields.get("roleName", Nothing))

				tmpRelationObjName = CType(fields.get("relationObjName", Nothing), javax.management.ObjectName)
				tmpNewRoleValue = cast(fields.get("newRoleValue", Nothing))
				tmpOldRoleValue = cast(fields.get("oldRoleValue", Nothing))
				tmpUnregMBeanList = cast(fields.get("unregisterMBeanList", Nothing))
			End If

			' Validate fields we just read, throw InvalidObjectException
			' if something goes wrong

			Dim notifType As String = MyBase.type
			If (Not isValidBasic(notifType,MyBase.source,tmpRelationId,tmpRelationTypeName)) OrElse ((Not isValidCreate(notifType)) AndAlso (Not isValidUpdate(notifType,tmpRoleName,tmpNewRoleValue,tmpOldRoleValue))) Then

				MyBase.source = Nothing
				Throw New java.io.InvalidObjectException("Invalid object read")
			End If

			' assign deserialized vaules to object fields
			relationObjName = safeGetObjectName(tmpRelationObjName)
			newRoleValue = safeGetObjectNameList(tmpNewRoleValue)
			oldRoleValue = safeGetObjectNameList(tmpOldRoleValue)
			unregisterMBeanList = safeGetObjectNameList(tmpUnregMBeanList)

			relationId = tmpRelationId
			relationTypeName = tmpRelationTypeName
			roleName = tmpRoleName
		End Sub


		''' <summary>
		''' Serializes a <seealso cref="RelationNotification"/> to an <seealso cref="ObjectOutputStream"/>.
		''' </summary>
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
		  If compat Then
			' Serializes this instance in the old serial form
			'
			Dim fields As java.io.ObjectOutputStream.PutField = out.putFields()
			fields.put("myNewRoleValue", newRoleValue)
			fields.put("myOldRoleValue", oldRoleValue)
			fields.put("myRelId", relationId)
			fields.put("myRelObjName", relationObjName)
			fields.put("myRelTypeName", relationTypeName)
			fields.put("myRoleName",roleName)
			fields.put("myUnregMBeanList", unregisterMBeanList)
			out.writeFields()
		  Else
			' Serializes this instance in the new serial form
			'
			out.defaultWriteObject()
		  End If
		End Sub
	End Class

End Namespace
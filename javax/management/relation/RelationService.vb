Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
import static com.sun.jmx.defaults.JmxProperties.RELATION_LOGGER
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
	''' The Relation Service is in charge of creating and deleting relation types
	''' and relations, of handling the consistency and of providing query
	''' mechanisms.
	''' <P>It implements the NotificationBroadcaster by extending
	''' NotificationBroadcasterSupport to send notifications when a relation is
	''' removed from it.
	''' <P>It implements the NotificationListener interface to be able to receive
	''' notifications concerning unregistration of MBeans referenced in relation
	''' roles and of relation MBeans.
	''' <P>It implements the MBeanRegistration interface to be able to retrieve
	''' its ObjectName and MBean Server.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class RelationService
		Inherits javax.management.NotificationBroadcasterSupport
		Implements RelationServiceMBean, javax.management.MBeanRegistration, javax.management.NotificationListener

		'
		' Private members
		'

		' Map associating:
		'      <relation id> -> <RelationSupport object/ObjectName>
		' depending if the relation has been created using createRelation()
		' method (so internally handled) or is an MBean added as a relation by the
		' user
		Private myRelId2ObjMap As IDictionary(Of String, Object) = New Dictionary(Of String, Object)

		' Map associating:
		'      <relation id> -> <relation type name>
		Private myRelId2RelTypeMap As IDictionary(Of String, String) = New Dictionary(Of String, String)

		' Map associating:
		'      <relation MBean Object Name> -> <relation id>
		Private myRelMBeanObjName2RelIdMap As IDictionary(Of javax.management.ObjectName, String) = New Dictionary(Of javax.management.ObjectName, String)

		' Map associating:
		'       <relation type name> -> <RelationType object>
		Private myRelType2ObjMap As IDictionary(Of String, RelationType) = New Dictionary(Of String, RelationType)

		' Map associating:
		'       <relation type name> -> ArrayList of <relation id>
		' to list all the relations of a given type
		Private myRelType2RelIdsMap As IDictionary(Of String, IList(Of String)) = New Dictionary(Of String, IList(Of String))

		' Map associating:
		'       <ObjectName> -> HashMap
		' the value HashMap mapping:
		'       <relation id> -> ArrayList of <role name>
		' to track where a given MBean is referenced.
		Private ReadOnly myRefedMBeanObjName2RelIdsMap As IDictionary(Of javax.management.ObjectName, IDictionary(Of String, IList(Of String))) = New Dictionary(Of javax.management.ObjectName, IDictionary(Of String, IList(Of String)))

		' Flag to indicate if, when a notification is received for the
		' unregistration of an MBean referenced in a relation, if an immediate
		' "purge" of the relations (look for the relations no
		' longer valid) has to be performed , or if that will be performed only
		' when the purgeRelations method will be explicitly called.
		' true is immediate purge.
		Private myPurgeFlag As Boolean = True

		' Internal counter to provide sequence numbers for notifications sent by:
		' - the Relation Service
		' - a relation handled by the Relation Service
		Private ReadOnly atomicSeqNo As New java.util.concurrent.atomic.AtomicLong

		' ObjectName used to register the Relation Service in the MBean Server
		Private myObjName As javax.management.ObjectName = Nothing

		' MBean Server where the Relation Service is registered
		Private myMBeanServer As javax.management.MBeanServer = Nothing

		' Filter registered in the MBean Server with the Relation Service to be
		' informed of referenced MBean deregistrations
		Private myUnregNtfFilter As MBeanServerNotificationFilter = Nothing

		' List of unregistration notifications received (storage used if purge
		' of relations when unregistering a referenced MBean is not immediate but
		' on user request)
		Private myUnregNtfList As IList(Of javax.management.MBeanServerNotification) = New List(Of javax.management.MBeanServerNotification)

		'
		' Constructor
		'

		''' <summary>
		''' Constructor.
		''' </summary>
		''' <param name="immediatePurgeFlag">  flag to indicate when a notification is
		''' received for the unregistration of an MBean referenced in a relation, if
		''' an immediate "purge" of the relations (look for the relations no
		''' longer valid) has to be performed , or if that will be performed only
		''' when the purgeRelations method will be explicitly called.
		''' <P>true is immediate purge. </param>
		Public Sub New(ByVal immediatePurgeFlag As Boolean)

			RELATION_LOGGER.entering(GetType(RelationService).name, "RelationService")

			purgeFlag = immediatePurgeFlag

			RELATION_LOGGER.exiting(GetType(RelationService).name, "RelationService")
			Return
		End Sub

		''' <summary>
		''' Checks if the Relation Service is active.
		''' Current condition is that the Relation Service must be registered in the
		''' MBean Server
		''' </summary>
		''' <exception cref="RelationServiceNotRegisteredException">  if it is not
		''' registered </exception>
		Public Overridable Sub isActive() Implements RelationServiceMBean.isActive
			If myMBeanServer Is Nothing Then
				' MBean Server not set by preRegister(): relation service not
				' registered
				Dim excMsg As String = "Relation Service not registered in the MBean Server."
				Throw New RelationServiceNotRegisteredException(excMsg)
			End If
			Return
		End Sub

		'
		' MBeanRegistration interface
		'

		' Pre-registration: retrieves its ObjectName and MBean Server
		'
		' No exception thrown.
		Public Overridable Function preRegister(ByVal server As javax.management.MBeanServer, ByVal name As javax.management.ObjectName) As javax.management.ObjectName

			myMBeanServer = server
			myObjName = name
			Return name
		End Function

		' Post-registration: does nothing
		Public Overridable Sub postRegister(ByVal registrationDone As Boolean?)
			Return
		End Sub

		' Pre-unregistration: does nothing
		Public Overridable Sub preDeregister()
			Return
		End Sub

		' Post-unregistration: does nothing
		Public Overridable Sub postDeregister()
			Return
		End Sub

		'
		' Accessors
		'

		''' <summary>
		''' Returns the flag to indicate if when a notification is received for the
		''' unregistration of an MBean referenced in a relation, if an immediate
		''' "purge" of the relations (look for the relations no longer valid)
		''' has to be performed , or if that will be performed only when the
		''' purgeRelations method will be explicitly called.
		''' <P>true is immediate purge.
		''' </summary>
		''' <returns> true if purges are automatic.
		''' </returns>
		''' <seealso cref= #setPurgeFlag </seealso>
		Public Overridable Property purgeFlag As Boolean Implements RelationServiceMBean.getPurgeFlag
			Get
				Return myPurgeFlag
			End Get
			Set(ByVal purgeFlag As Boolean)
    
				myPurgeFlag = purgeFlag
				Return
			End Set
		End Property


		'
		' Relation type handling
		'

		''' <summary>
		''' Creates a relation type (a RelationTypeSupport object) with given
		''' role infos (provided by the RoleInfo objects), and adds it in the
		''' Relation Service.
		''' </summary>
		''' <param name="relationTypeName">  name of the relation type </param>
		''' <param name="roleInfoArray">  array of role infos
		''' </param>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="InvalidRelationTypeException">  If:
		''' <P>- there is already a relation type with that name
		''' <P>- the same name has been used for two different role infos
		''' <P>- no role info provided
		''' <P>- one null role info provided </exception>
		Public Overridable Sub createRelationType(ByVal relationTypeName As String, ByVal roleInfoArray As RoleInfo()) Implements RelationServiceMBean.createRelationType

			If relationTypeName Is Nothing OrElse roleInfoArray Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "createRelationType", relationTypeName)

			' Can throw an InvalidRelationTypeException
			Dim relType As RelationType = New RelationTypeSupport(relationTypeName, roleInfoArray)

			addRelationTypeInt(relType)

			RELATION_LOGGER.exiting(GetType(RelationService).name, "createRelationType")
			Return
		End Sub

		''' <summary>
		''' Adds given object as a relation type. The object is expected to
		''' implement the RelationType interface.
		''' </summary>
		''' <param name="relationTypeObj">  relation type object (implementing the
		''' RelationType interface)
		''' </param>
		''' <exception cref="IllegalArgumentException">  if null parameter or if
		''' {@link RelationType#getRelationTypeName
		''' relationTypeObj.getRelationTypeName()} returns null. </exception>
		''' <exception cref="InvalidRelationTypeException">  if:
		''' <P>- the same name has been used for two different roles
		''' <P>- no role info provided
		''' <P>- one null role info provided
		''' <P>- there is already a relation type with that name </exception>
		Public Overridable Sub addRelationType(ByVal relationTypeObj As RelationType) Implements RelationServiceMBean.addRelationType

			If relationTypeObj Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "addRelationType")

			' Checks the role infos
			Dim roleInfoList As IList(Of RoleInfo) = relationTypeObj.roleInfos
			If roleInfoList Is Nothing Then
				Dim excMsg As String = "No role info provided."
				Throw New InvalidRelationTypeException(excMsg)
			End If

			Dim roleInfoArray As RoleInfo() = New RoleInfo(roleInfoList.Count - 1){}
			Dim i As Integer = 0
			For Each currRoleInfo As RoleInfo In roleInfoList
				roleInfoArray(i) = currRoleInfo
				i += 1
			Next currRoleInfo
			' Can throw InvalidRelationTypeException
			RelationTypeSupport.checkRoleInfos(roleInfoArray)

			addRelationTypeInt(relationTypeObj)

			RELATION_LOGGER.exiting(GetType(RelationService).name, "addRelationType")
			Return
		End Sub

		''' <summary>
		''' Retrieves names of all known relation types.
		''' </summary>
		''' <returns> ArrayList of relation type names (Strings) </returns>
		Public Overridable Property allRelationTypeNames As IList(Of String) Implements RelationServiceMBean.getAllRelationTypeNames
			Get
				Dim result As List(Of String)
				SyncLock myRelType2ObjMap
					result = New List(Of String)(myRelType2ObjMap.Keys)
				End SyncLock
				Return result
			End Get
		End Property

		''' <summary>
		''' Retrieves list of role infos (RoleInfo objects) of a given relation
		''' type.
		''' </summary>
		''' <param name="relationTypeName">  name of relation type
		''' </param>
		''' <returns> ArrayList of RoleInfo.
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationTypeNotFoundException">  if there is no relation type
		''' with that name. </exception>
		Public Overridable Function getRoleInfos(ByVal relationTypeName As String) As IList(Of RoleInfo) Implements RelationServiceMBean.getRoleInfos

			If relationTypeName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "getRoleInfos", relationTypeName)

			' Can throw a RelationTypeNotFoundException
			Dim relType As RelationType = getRelationType(relationTypeName)

			RELATION_LOGGER.exiting(GetType(RelationService).name, "getRoleInfos")
			Return relType.roleInfos
		End Function

		''' <summary>
		''' Retrieves role info for given role name of a given relation type.
		''' </summary>
		''' <param name="relationTypeName">  name of relation type </param>
		''' <param name="roleInfoName">  name of role
		''' </param>
		''' <returns> RoleInfo object.
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationTypeNotFoundException">  if the relation type is not
		''' known in the Relation Service </exception>
		''' <exception cref="RoleInfoNotFoundException">  if the role is not part of the
		''' relation type. </exception>
		Public Overridable Function getRoleInfo(ByVal relationTypeName As String, ByVal roleInfoName As String) As RoleInfo Implements RelationServiceMBean.getRoleInfo

			If relationTypeName Is Nothing OrElse roleInfoName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "getRoleInfo", New Object() {relationTypeName, roleInfoName})

			' Can throw a RelationTypeNotFoundException
			Dim relType As RelationType = getRelationType(relationTypeName)

			' Can throw a RoleInfoNotFoundException
			Dim ___roleInfo As RoleInfo = relType.getRoleInfo(roleInfoName)

			RELATION_LOGGER.exiting(GetType(RelationService).name, "getRoleInfo")
			Return ___roleInfo
		End Function

		''' <summary>
		''' Removes given relation type from Relation Service.
		''' <P>The relation objects of that type will be removed from the
		''' Relation Service.
		''' </summary>
		''' <param name="relationTypeName">  name of the relation type to be removed
		''' </param>
		''' <exception cref="RelationServiceNotRegisteredException">  if the Relation
		''' Service is not registered in the MBean Server </exception>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationTypeNotFoundException">  If there is no relation type
		''' with that name </exception>
		Public Overridable Sub removeRelationType(ByVal relationTypeName As String) Implements RelationServiceMBean.removeRelationType

			' Can throw RelationServiceNotRegisteredException
			active

			If relationTypeName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "removeRelationType", relationTypeName)

			' Checks if the relation type to be removed exists
			' Can throw a RelationTypeNotFoundException
			Dim relType As RelationType = getRelationType(relationTypeName)

			' Retrieves the relation ids for relations of that type
			Dim relIdList As IList(Of String) = Nothing
			SyncLock myRelType2RelIdsMap
				' Note: take a copy of the list as it is a part of a map that
				'       will be updated by removeRelation() below.
				Dim relIdList1 As IList(Of String) = myRelType2RelIdsMap(relationTypeName)
				If relIdList1 IsNot Nothing Then relIdList = New List(Of String)(relIdList1)
			End SyncLock

			' Removes the relation type from all maps
			SyncLock myRelType2ObjMap
				myRelType2ObjMap.Remove(relationTypeName)
			End SyncLock
			SyncLock myRelType2RelIdsMap
				myRelType2RelIdsMap.Remove(relationTypeName)
			End SyncLock

			' Removes all relations of that type
			If relIdList IsNot Nothing Then
				For Each currRelId As String In relIdList
					' Note: will remove it from myRelId2RelTypeMap :)
					'
					' Can throw RelationServiceNotRegisteredException (detected
					' above)
					' Shall not throw a RelationNotFoundException
					Try
						removeRelation(currRelId)
					Catch exc1 As RelationNotFoundException
						Throw New Exception(exc1.Message)
					End Try
				Next currRelId
			End If

			RELATION_LOGGER.exiting(GetType(RelationService).name, "removeRelationType")
			Return
		End Sub

		'
		' Relation handling
		'

		''' <summary>
		''' Creates a simple relation (represented by a RelationSupport object) of
		''' given relation type, and adds it in the Relation Service.
		''' <P>Roles are initialized according to the role list provided in
		''' parameter. The ones not initialized in this way are set to an empty
		''' ArrayList of ObjectNames.
		''' <P>A RelationNotification, with type RELATION_BASIC_CREATION, is sent.
		''' </summary>
		''' <param name="relationId">  relation identifier, to identify uniquely the relation
		''' inside the Relation Service </param>
		''' <param name="relationTypeName">  name of the relation type (has to be created
		''' in the Relation Service) </param>
		''' <param name="roleList">  role list to initialize roles of the relation (can
		''' be null).
		''' </param>
		''' <exception cref="RelationServiceNotRegisteredException">  if the Relation
		''' Service is not registered in the MBean Server </exception>
		''' <exception cref="IllegalArgumentException">  if null parameter, except the role
		''' list which can be null if no role initialization </exception>
		''' <exception cref="RoleNotFoundException">  if a value is provided for a role
		''' that does not exist in the relation type </exception>
		''' <exception cref="InvalidRelationIdException">  if relation id already used </exception>
		''' <exception cref="RelationTypeNotFoundException">  if relation type not known in
		''' Relation Service </exception>
		''' <exception cref="InvalidRoleValueException"> if:
		''' <P>- the same role name is used for two different roles
		''' <P>- the number of referenced MBeans in given value is less than
		''' expected minimum degree
		''' <P>- the number of referenced MBeans in provided value exceeds expected
		''' maximum degree
		''' <P>- one referenced MBean in the value is not an Object of the MBean
		''' class expected for that role
		''' <P>- an MBean provided for that role does not exist </exception>
		Public Overridable Sub createRelation(ByVal relationId As String, ByVal relationTypeName As String, ByVal roleList As RoleList) Implements RelationServiceMBean.createRelation

			' Can throw RelationServiceNotRegisteredException
			active

			If relationId Is Nothing OrElse relationTypeName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "createRelation", New Object() {relationId, relationTypeName, roleList})

			' Creates RelationSupport object
			' Can throw InvalidRoleValueException
			Dim relObj As New RelationSupport(relationId, myObjName, relationTypeName, roleList)

			' Adds relation object as a relation into the Relation Service
			' Can throw RoleNotFoundException, InvalidRelationId,
			' RelationTypeNotFoundException, InvalidRoleValueException
			'
			' Cannot throw MBeanException
			addRelationInt(True, relObj, Nothing, relationId, relationTypeName, roleList)
			RELATION_LOGGER.exiting(GetType(RelationService).name, "createRelation")
			Return
		End Sub

		''' <summary>
		''' Adds an MBean created by the user (and registered by him in the MBean
		''' Server) as a relation in the Relation Service.
		''' <P>To be added as a relation, the MBean must conform to the
		''' following:
		''' <P>- implement the Relation interface
		''' <P>- have for RelationService ObjectName the ObjectName of current
		''' Relation Service
		''' <P>- have a relation id unique and unused in current Relation Service
		''' <P>- have for relation type a relation type created in the Relation
		''' Service
		''' <P>- have roles conforming to the role info provided in the relation
		''' type.
		''' </summary>
		''' <param name="relationObjectName">  ObjectName of the relation MBean to be added.
		''' </param>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationServiceNotRegisteredException">  if the Relation
		''' Service is not registered in the MBean Server </exception>
		''' <exception cref="NoSuchMethodException">  If the MBean does not implement the
		''' Relation interface </exception>
		''' <exception cref="InvalidRelationIdException">  if:
		''' <P>- no relation identifier in MBean
		''' <P>- the relation identifier is already used in the Relation Service </exception>
		''' <exception cref="InstanceNotFoundException">  if the MBean for given ObjectName
		''' has not been registered </exception>
		''' <exception cref="InvalidRelationServiceException">  if:
		''' <P>- no Relation Service name in MBean
		''' <P>- the Relation Service name in the MBean is not the one of the
		''' current Relation Service </exception>
		''' <exception cref="RelationTypeNotFoundException">  if:
		''' <P>- no relation type name in MBean
		''' <P>- the relation type name in MBean does not correspond to a relation
		''' type created in the Relation Service </exception>
		''' <exception cref="InvalidRoleValueException">  if:
		''' <P>- the number of referenced MBeans in a role is less than
		''' expected minimum degree
		''' <P>- the number of referenced MBeans in a role exceeds expected
		''' maximum degree
		''' <P>- one referenced MBean in the value is not an Object of the MBean
		''' class expected for that role
		''' <P>- an MBean provided for a role does not exist </exception>
		''' <exception cref="RoleNotFoundException">  if a value is provided for a role
		''' that does not exist in the relation type </exception>
		Public Overridable Sub addRelation(ByVal relationObjectName As javax.management.ObjectName) Implements RelationServiceMBean.addRelation

			If relationObjectName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "addRelation", relationObjectName)

			' Can throw RelationServiceNotRegisteredException
			active

			' Checks that the relation MBean implements the Relation interface.
			' It will also check that the provided ObjectName corresponds to a
			' registered MBean (else will throw an InstanceNotFoundException)
			If (Not(myMBeanServer.isInstanceOf(relationObjectName, "javax.management.relation.Relation"))) Then
				Dim excMsg As String = "This MBean does not implement the Relation interface."
				Throw New NoSuchMethodException(excMsg)
			End If
			' Checks there is a relation id in the relation MBean (its uniqueness
			' is checked in addRelationInt())
			' Can throw InstanceNotFoundException (but detected above)
			' No MBeanException as no exception raised by this method, and no
			' ReflectionException
			Dim relId As String
			Try
				relId = CStr(myMBeanServer.getAttribute(relationObjectName, "RelationId"))

			Catch exc1 As javax.management.MBeanException
				Throw New Exception((exc1.targetException).message)
			Catch exc2 As javax.management.ReflectionException
				Throw New Exception(exc2.Message)
			Catch exc3 As javax.management.AttributeNotFoundException
				Throw New Exception(exc3.Message)
			End Try

			If relId Is Nothing Then
				Dim excMsg As String = "This MBean does not provide a relation id."
				Throw New InvalidRelationIdException(excMsg)
			End If
			' Checks that the Relation Service where the relation MBean is
			' expected to be added is the current one
			' Can throw InstanceNotFoundException (but detected above)
			' No MBeanException as no exception raised by this method, no
			' ReflectionException
			Dim relServObjName As javax.management.ObjectName
			Try
				relServObjName = CType(myMBeanServer.getAttribute(relationObjectName, "RelationServiceName"), javax.management.ObjectName)

			Catch exc1 As javax.management.MBeanException
				Throw New Exception((exc1.targetException).message)
			Catch exc2 As javax.management.ReflectionException
				Throw New Exception(exc2.Message)
			Catch exc3 As javax.management.AttributeNotFoundException
				Throw New Exception(exc3.Message)
			End Try

			Dim badRelServFlag As Boolean = False
			If relServObjName Is Nothing Then
				badRelServFlag = True

			ElseIf Not(relServObjName.Equals(myObjName)) Then
				badRelServFlag = True
			End If
			If badRelServFlag Then
				Dim excMsg As String = "The Relation Service referenced in the MBean is not the current one."
				Throw New InvalidRelationServiceException(excMsg)
			End If
			' Checks that a relation type has been specified for the relation
			' Can throw InstanceNotFoundException (but detected above)
			' No MBeanException as no exception raised by this method, no
			' ReflectionException
			Dim relTypeName As String
			Try
				relTypeName = CStr(myMBeanServer.getAttribute(relationObjectName, "RelationTypeName"))

			Catch exc1 As javax.management.MBeanException
				Throw New Exception((exc1.targetException).message)
			Catch exc2 As javax.management.ReflectionException
				Throw New Exception(exc2.Message)
			Catch exc3 As javax.management.AttributeNotFoundException
				Throw New Exception(exc3.Message)
			End Try
			If relTypeName Is Nothing Then
				Dim excMsg As String = "No relation type provided."
				Throw New RelationTypeNotFoundException(excMsg)
			End If
			' Retrieves all roles without considering read mode
			' Can throw InstanceNotFoundException (but detected above)
			' No MBeanException as no exception raised by this method, no
			' ReflectionException
			Dim roleList As RoleList
			Try
				roleList = CType(myMBeanServer.invoke(relationObjectName, "retrieveAllRoles", Nothing, Nothing), RoleList)
			Catch exc1 As javax.management.MBeanException
				Throw New Exception((exc1.targetException).message)
			Catch exc2 As javax.management.ReflectionException
				Throw New Exception(exc2.Message)
			End Try

			' Can throw RoleNotFoundException, InvalidRelationIdException,
			' RelationTypeNotFoundException, InvalidRoleValueException
			addRelationInt(False, Nothing, relationObjectName, relId, relTypeName, roleList)
			' Adds relation MBean ObjectName in map
			SyncLock myRelMBeanObjName2RelIdMap
				myRelMBeanObjName2RelIdMap(relationObjectName) = relId
			End SyncLock

			' Updates flag to specify that the relation is managed by the Relation
			' Service
			' This flag and setter are inherited from RelationSupport and not parts
			' of the Relation interface, so may be not supported.
			Try
				myMBeanServer.attributeute(relationObjectName, New javax.management.Attribute("RelationServiceManagementFlag", Boolean.TRUE))
			Catch exc As Exception
				' OK : The flag is not supported.
			End Try

			' Updates listener information to received notification for
			' unregistration of this MBean
			Dim newRefList As IList(Of javax.management.ObjectName) = New List(Of javax.management.ObjectName)
			newRefList.Add(relationObjectName)
			updateUnregistrationListener(newRefList, Nothing)

			RELATION_LOGGER.exiting(GetType(RelationService).name, "addRelation")
			Return
		End Sub

		''' <summary>
		''' If the relation is represented by an MBean (created by the user and
		''' added as a relation in the Relation Service), returns the ObjectName of
		''' the MBean.
		''' </summary>
		''' <param name="relationId">  relation id identifying the relation
		''' </param>
		''' <returns> ObjectName of the corresponding relation MBean, or null if
		''' the relation is not an MBean.
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationNotFoundException"> there is no relation associated
		''' to that id </exception>
		Public Overridable Function isRelationMBean(ByVal relationId As String) As javax.management.ObjectName Implements RelationServiceMBean.isRelationMBean

			If relationId Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "isRelationMBean", relationId)

			' Can throw RelationNotFoundException
			Dim result As Object = getRelation(relationId)
			If TypeOf result Is javax.management.ObjectName Then
				Return (CType(result, javax.management.ObjectName))
			Else
				Return Nothing
			End If
		End Function

		''' <summary>
		''' Returns the relation id associated to the given ObjectName if the
		''' MBean has been added as a relation in the Relation Service.
		''' </summary>
		''' <param name="objectName">  ObjectName of supposed relation
		''' </param>
		''' <returns> relation id (String) or null (if the ObjectName is not a
		''' relation handled by the Relation Service)
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		Public Overridable Function isRelation(ByVal ___objectName As javax.management.ObjectName) As String Implements RelationServiceMBean.isRelation

			If ___objectName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "isRelation", ___objectName)

			Dim result As String = Nothing
			SyncLock myRelMBeanObjName2RelIdMap
				Dim relId As String = myRelMBeanObjName2RelIdMap(___objectName)
				If relId IsNot Nothing Then result = relId
			End SyncLock
			Return result
		End Function

		''' <summary>
		''' Checks if there is a relation identified in Relation Service with given
		''' relation id.
		''' </summary>
		''' <param name="relationId">  relation id identifying the relation
		''' </param>
		''' <returns> boolean: true if there is a relation, false else
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		Public Overridable Function hasRelation(ByVal relationId As String) As Boolean?

			If relationId Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "hasRelation", relationId)

			Try
				' Can throw RelationNotFoundException
				Dim result As Object = getRelation(relationId)
				Return True
			Catch exc As RelationNotFoundException
				Return False
			End Try
		End Function

		''' <summary>
		''' Returns all the relation ids for all the relations handled by the
		''' Relation Service.
		''' </summary>
		''' <returns> ArrayList of String </returns>
		Public Overridable Property allRelationIds As IList(Of String) Implements RelationServiceMBean.getAllRelationIds
			Get
				Dim result As IList(Of String)
				SyncLock myRelId2ObjMap
					result = New List(Of String)(myRelId2ObjMap.Keys)
				End SyncLock
				Return result
			End Get
		End Property

		''' <summary>
		''' Checks if given Role can be read in a relation of the given type.
		''' </summary>
		''' <param name="roleName">  name of role to be checked </param>
		''' <param name="relationTypeName">  name of the relation type
		''' </param>
		''' <returns> an Integer wrapping an integer corresponding to possible
		''' problems represented as constants in RoleUnresolved:
		''' <P>- 0 if role can be read
		''' <P>- integer corresponding to RoleStatus.NO_ROLE_WITH_NAME
		''' <P>- integer corresponding to RoleStatus.ROLE_NOT_READABLE
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationTypeNotFoundException">  if the relation type is not
		''' known in the Relation Service </exception>
		Public Overridable Function checkRoleReading(ByVal roleName As String, ByVal relationTypeName As String) As Integer?

			If roleName Is Nothing OrElse relationTypeName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "checkRoleReading", New Object() {roleName, relationTypeName})

			Dim result As Integer?

			' Can throw a RelationTypeNotFoundException
			Dim relType As RelationType = getRelationType(relationTypeName)

			Try
				' Can throw a RoleInfoNotFoundException to be transformed into
				' returned value RoleStatus.NO_ROLE_WITH_NAME
				Dim ___roleInfo As RoleInfo = relType.getRoleInfo(roleName)

				result = checkRoleInt(1, roleName, Nothing, ___roleInfo, False)

			Catch exc As RoleInfoNotFoundException
				result = Convert.ToInt32(RoleStatus.NO_ROLE_WITH_NAME)
			End Try

			RELATION_LOGGER.exiting(GetType(RelationService).name, "checkRoleReading")
			Return result
		End Function

		''' <summary>
		''' Checks if given Role can be set in a relation of given type.
		''' </summary>
		''' <param name="role">  role to be checked </param>
		''' <param name="relationTypeName">  name of relation type </param>
		''' <param name="initFlag">  flag to specify that the checking is done for the
		''' initialization of a role, write access shall not be verified.
		''' </param>
		''' <returns> an Integer wrapping an integer corresponding to possible
		''' problems represented as constants in RoleUnresolved:
		''' <P>- 0 if role can be set
		''' <P>- integer corresponding to RoleStatus.NO_ROLE_WITH_NAME
		''' <P>- integer for RoleStatus.ROLE_NOT_WRITABLE
		''' <P>- integer for RoleStatus.LESS_THAN_MIN_ROLE_DEGREE
		''' <P>- integer for RoleStatus.MORE_THAN_MAX_ROLE_DEGREE
		''' <P>- integer for RoleStatus.REF_MBEAN_OF_INCORRECT_CLASS
		''' <P>- integer for RoleStatus.REF_MBEAN_NOT_REGISTERED
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationTypeNotFoundException">  if unknown relation type </exception>
		Public Overridable Function checkRoleWriting(ByVal ___role As Role, ByVal relationTypeName As String, ByVal initFlag As Boolean?) As Integer?

			If ___role Is Nothing OrElse relationTypeName Is Nothing OrElse initFlag Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "checkRoleWriting", New Object() {___role, relationTypeName, initFlag})

			' Can throw a RelationTypeNotFoundException
			Dim relType As RelationType = getRelationType(relationTypeName)

			Dim roleName As String = ___role.roleName
			Dim roleValue As IList(Of javax.management.ObjectName) = ___role.roleValue
			Dim writeChkFlag As Boolean = True
			If initFlag Then writeChkFlag = False

			Dim ___roleInfo As RoleInfo
			Try
				___roleInfo = relType.getRoleInfo(roleName)
			Catch exc As RoleInfoNotFoundException
				RELATION_LOGGER.exiting(GetType(RelationService).name, "checkRoleWriting")
				Return Convert.ToInt32(RoleStatus.NO_ROLE_WITH_NAME)
			End Try

			Dim result As Integer? = checkRoleInt(2, roleName, roleValue, ___roleInfo, writeChkFlag)

			RELATION_LOGGER.exiting(GetType(RelationService).name, "checkRoleWriting")
			Return result
		End Function

		''' <summary>
		''' Sends a notification (RelationNotification) for a relation creation.
		''' The notification type is:
		''' <P>- RelationNotification.RELATION_BASIC_CREATION if the relation is an
		''' object internal to the Relation Service
		''' <P>- RelationNotification.RELATION_MBEAN_CREATION if the relation is a
		''' MBean added as a relation.
		''' <P>The source object is the Relation Service itself.
		''' <P>It is called in Relation Service createRelation() and
		''' addRelation() methods.
		''' </summary>
		''' <param name="relationId">  relation identifier of the updated relation
		''' </param>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationNotFoundException">  if there is no relation for given
		''' relation id </exception>
		Public Overridable Sub sendRelationCreationNotification(ByVal relationId As String) Implements RelationServiceMBean.sendRelationCreationNotification

			If relationId Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "sendRelationCreationNotification", relationId)

			' Message
			Dim ntfMsg As New StringBuilder("Creation of relation ")
			ntfMsg.Append(relationId)

			' Can throw RelationNotFoundException
			sendNotificationInt(1, ntfMsg.ToString(), relationId, Nothing, Nothing, Nothing, Nothing)

			RELATION_LOGGER.exiting(GetType(RelationService).name, "sendRelationCreationNotification")
			Return
		End Sub

		''' <summary>
		''' Sends a notification (RelationNotification) for a role update in the
		''' given relation. The notification type is:
		''' <P>- RelationNotification.RELATION_BASIC_UPDATE if the relation is an
		''' object internal to the Relation Service
		''' <P>- RelationNotification.RELATION_MBEAN_UPDATE if the relation is a
		''' MBean added as a relation.
		''' <P>The source object is the Relation Service itself.
		''' <P>It is called in relation MBean setRole() (for given role) and
		''' setRoles() (for each role) methods (implementation provided in
		''' RelationSupport class).
		''' <P>It is also called in Relation Service setRole() (for given role) and
		''' setRoles() (for each role) methods.
		''' </summary>
		''' <param name="relationId">  relation identifier of the updated relation </param>
		''' <param name="newRole">  new role (name and new value) </param>
		''' <param name="oldValue">  old role value (List of ObjectName objects)
		''' </param>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationNotFoundException">  if there is no relation for given
		''' relation id </exception>
		Public Overridable Sub sendRoleUpdateNotification(ByVal relationId As String, ByVal newRole As Role, ByVal oldValue As IList(Of javax.management.ObjectName)) Implements RelationServiceMBean.sendRoleUpdateNotification

			If relationId Is Nothing OrElse newRole Is Nothing OrElse oldValue Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			If Not(TypeOf oldValue Is List(Of ?)) Then oldValue = New List(Of javax.management.ObjectName)(oldValue)

			RELATION_LOGGER.entering(GetType(RelationService).name, "sendRoleUpdateNotification", New Object() {relationId, newRole, oldValue})

			Dim roleName As String = newRole.roleName
			Dim newRoleVal As IList(Of javax.management.ObjectName) = newRole.roleValue

			' Message
			Dim newRoleValString As String = Role.roleValueToString(newRoleVal)
			Dim oldRoleValString As String = Role.roleValueToString(oldValue)
			Dim ntfMsg As New StringBuilder("Value of role ")
			ntfMsg.Append(roleName)
			ntfMsg.Append(" has changed" & vbLf & "Old value:" & vbLf)
			ntfMsg.Append(oldRoleValString)
			ntfMsg.Append(vbLf & "New value:" & vbLf)
			ntfMsg.Append(newRoleValString)

			' Can throw a RelationNotFoundException
			sendNotificationInt(2, ntfMsg.ToString(), relationId, Nothing, roleName, newRoleVal, oldValue)

			RELATION_LOGGER.exiting(GetType(RelationService).name, "sendRoleUpdateNotification")
		End Sub

		''' <summary>
		''' Sends a notification (RelationNotification) for a relation removal.
		''' The notification type is:
		''' <P>- RelationNotification.RELATION_BASIC_REMOVAL if the relation is an
		''' object internal to the Relation Service
		''' <P>- RelationNotification.RELATION_MBEAN_REMOVAL if the relation is a
		''' MBean added as a relation.
		''' <P>The source object is the Relation Service itself.
		''' <P>It is called in Relation Service removeRelation() method.
		''' </summary>
		''' <param name="relationId">  relation identifier of the updated relation </param>
		''' <param name="unregMBeanList">  List of ObjectNames of MBeans expected
		''' to be unregistered due to relation removal (can be null)
		''' </param>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationNotFoundException">  if there is no relation for given
		''' relation id </exception>
		Public Overridable Sub sendRelationRemovalNotification(ByVal relationId As String, ByVal unregMBeanList As IList(Of javax.management.ObjectName)) Implements RelationServiceMBean.sendRelationRemovalNotification

			If relationId Is Nothing Then
				Dim excMsg As String = "Invalid parameter"
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "sendRelationRemovalNotification", New Object() {relationId, unregMBeanList})

			' Can throw RelationNotFoundException
			sendNotificationInt(3, "Removal of relation " & relationId, relationId, unregMBeanList, Nothing, Nothing, Nothing)


			RELATION_LOGGER.exiting(GetType(RelationService).name, "sendRelationRemovalNotification")
			Return
		End Sub

		''' <summary>
		''' Handles update of the Relation Service role map for the update of given
		''' role in given relation.
		''' <P>It is called in relation MBean setRole() (for given role) and
		''' setRoles() (for each role) methods (implementation provided in
		''' RelationSupport class).
		''' <P>It is also called in Relation Service setRole() (for given role) and
		''' setRoles() (for each role) methods.
		''' <P>To allow the Relation Service to maintain the consistency (in case
		''' of MBean unregistration) and to be able to perform queries, this method
		''' must be called when a role is updated.
		''' </summary>
		''' <param name="relationId">  relation identifier of the updated relation </param>
		''' <param name="newRole">  new role (name and new value) </param>
		''' <param name="oldValue">  old role value (List of ObjectName objects)
		''' </param>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationServiceNotRegisteredException">  if the Relation
		''' Service is not registered in the MBean Server </exception>
		''' <exception cref="RelationNotFoundException">  if no relation for given id. </exception>
		Public Overridable Sub updateRoleMap(ByVal relationId As String, ByVal newRole As Role, ByVal oldValue As IList(Of javax.management.ObjectName)) Implements RelationServiceMBean.updateRoleMap

			If relationId Is Nothing OrElse newRole Is Nothing OrElse oldValue Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "updateRoleMap", New Object() {relationId, newRole, oldValue})

			' Can throw RelationServiceNotRegisteredException
			active

			' Verifies the relation has been added in the Relation Service
			' Can throw a RelationNotFoundException
			Dim result As Object = getRelation(relationId)

			Dim roleName As String = newRole.roleName
			Dim newRoleValue As IList(Of javax.management.ObjectName) = newRole.roleValue
			' Note: no need to test if oldValue not null before cloning,
			'       tested above.
			Dim oldRoleValue As IList(Of javax.management.ObjectName) = New List(Of javax.management.ObjectName)(oldValue)

			' List of ObjectNames of new referenced MBeans
			Dim newRefList As IList(Of javax.management.ObjectName) = New List(Of javax.management.ObjectName)

			For Each currObjName As javax.management.ObjectName In newRoleValue

				' Checks if this ObjectName was already present in old value
				' Note: use copy (oldRoleValue) instead of original
				'       oldValue to speed up, as oldRoleValue is decreased
				'       by removing unchanged references :)
				Dim currObjNamePos As Integer = oldRoleValue.IndexOf(currObjName)

				If currObjNamePos = -1 Then
					' New reference to an ObjectName

					' Stores this reference into map
					' Returns true if new reference, false if MBean already
					' referenced
					Dim isNewFlag As Boolean = addNewMBeanReference(currObjName, relationId, roleName)

					If isNewFlag Then newRefList.Add(currObjName)

				Else
					' MBean was already referenced in old value

					' Removes it from old value (local list) to ignore it when
					' looking for remove MBean references
					oldRoleValue.RemoveAt(currObjNamePos)
				End If
			Next currObjName

			' List of ObjectNames of MBeans no longer referenced
			Dim obsRefList As IList(Of javax.management.ObjectName) = New List(Of javax.management.ObjectName)

			' Each ObjectName remaining in oldRoleValue is an ObjectName no longer
			' referenced in new value
			For Each currObjName As javax.management.ObjectName In oldRoleValue
				' Removes MBean reference from map
				' Returns true if the MBean is no longer referenced in any
				' relation
				Dim noLongerRefFlag As Boolean = removeMBeanReference(currObjName, relationId, roleName, False)

				If noLongerRefFlag Then obsRefList.Add(currObjName)
			Next currObjName

			' To avoid having one listener per ObjectName of referenced MBean,
			' and to increase performances, there is only one listener recording
			' all ObjectNames of interest
			updateUnregistrationListener(newRefList, obsRefList)

			RELATION_LOGGER.exiting(GetType(RelationService).name, "updateRoleMap")
			Return
		End Sub

		''' <summary>
		''' Removes given relation from the Relation Service.
		''' <P>A RelationNotification notification is sent, its type being:
		''' <P>- RelationNotification.RELATION_BASIC_REMOVAL if the relation was
		''' only internal to the Relation Service
		''' <P>- RelationNotification.RELATION_MBEAN_REMOVAL if the relation is
		''' registered as an MBean.
		''' <P>For MBeans referenced in such relation, nothing will be done,
		''' </summary>
		''' <param name="relationId">  relation id of the relation to be removed
		''' </param>
		''' <exception cref="RelationServiceNotRegisteredException">  if the Relation
		''' Service is not registered in the MBean Server </exception>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationNotFoundException">  if no relation corresponding to
		''' given relation id </exception>
		Public Overridable Sub removeRelation(ByVal relationId As String) Implements RelationServiceMBean.removeRelation

			' Can throw RelationServiceNotRegisteredException
			active

			If relationId Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "removeRelation", relationId)

			' Checks there is a relation with this id
			' Can throw RelationNotFoundException
			Dim result As Object = getRelation(relationId)

			' Removes it from listener filter
			If TypeOf result Is javax.management.ObjectName Then
				Dim obsRefList As IList(Of javax.management.ObjectName) = New List(Of javax.management.ObjectName)
				obsRefList.Add(CType(result, javax.management.ObjectName))
				' Can throw a RelationServiceNotRegisteredException
				updateUnregistrationListener(Nothing, obsRefList)
			End If

			' Sends a notification
			' Note: has to be done FIRST as needs the relation to be still in the
			'       Relation Service
			' No RelationNotFoundException as checked above

			' Revisit [cebro] Handle CIM "Delete" and "IfDeleted" qualifiers:
			'   deleting the relation can mean to delete referenced MBeans. In
			'   that case, MBeans to be unregistered are put in a list sent along
			'   with the notification below

			' Can throw a RelationNotFoundException (but detected above)
			sendRelationRemovalNotification(relationId, Nothing)

			' Removes the relation from various internal maps

			'  - MBean reference map
			' Retrieves the MBeans referenced in this relation
			' Note: here we cannot use removeMBeanReference() because it would
			'       require to know the MBeans referenced in the relation. For
			'       that it would be necessary to call 'getReferencedMBeans()'
			'       on the relation itself. Ok if it is an internal one, but if
			'       it is an MBean, it is possible it is already unregistered, so
			'       not available through the MBean Server.
			Dim refMBeanList As IList(Of javax.management.ObjectName) = New List(Of javax.management.ObjectName)
			' List of MBeans no longer referenced in any relation, to be
			' removed fom the map
			Dim nonRefObjNameList As IList(Of javax.management.ObjectName) = New List(Of javax.management.ObjectName)

			SyncLock myRefedMBeanObjName2RelIdsMap

				For Each currRefObjName As javax.management.ObjectName In myRefedMBeanObjName2RelIdsMap.Keys

					' Retrieves relations where the MBean is referenced
					Dim relIdMap As IDictionary(Of String, IList(Of String)) = myRefedMBeanObjName2RelIdsMap(currRefObjName)

					If relIdMap.ContainsKey(relationId) Then
						relIdMap.Remove(relationId)
						refMBeanList.Add(currRefObjName)
					End If

					If relIdMap.Count = 0 Then nonRefObjNameList.Add(currRefObjName)
				Next currRefObjName

				' Cleans MBean reference map by removing MBeans no longer
				' referenced
				For Each currRefObjName As javax.management.ObjectName In nonRefObjNameList
					myRefedMBeanObjName2RelIdsMap.Remove(currRefObjName)
				Next currRefObjName
			End SyncLock

			' - Relation id to object map
			SyncLock myRelId2ObjMap
				myRelId2ObjMap.Remove(relationId)
			End SyncLock

			If TypeOf result Is javax.management.ObjectName Then
				' - ObjectName to relation id map
				SyncLock myRelMBeanObjName2RelIdMap
					myRelMBeanObjName2RelIdMap.Remove(CType(result, javax.management.ObjectName))
				End SyncLock
			End If

			' Relation id to relation type name map
			' First retrieves the relation type name
			Dim relTypeName As String
			SyncLock myRelId2RelTypeMap
				relTypeName = myRelId2RelTypeMap(relationId)
				myRelId2RelTypeMap.Remove(relationId)
			End SyncLock
			' - Relation type name to relation id map
			SyncLock myRelType2RelIdsMap
				Dim relIdList As IList(Of String) = myRelType2RelIdsMap(relTypeName)
				If relIdList IsNot Nothing Then
					' Can be null if called from removeRelationType()
					relIdList.Remove(relationId)
					If relIdList.Count = 0 Then myRelType2RelIdsMap.Remove(relTypeName)
				End If
			End SyncLock

			RELATION_LOGGER.exiting(GetType(RelationService).name, "removeRelation")
			Return
		End Sub

		''' <summary>
		''' Purges the relations.
		''' 
		''' <P>Depending on the purgeFlag value, this method is either called
		''' automatically when a notification is received for the unregistration of
		''' an MBean referenced in a relation (if the flag is set to true), or not
		''' (if the flag is set to false).
		''' <P>In that case it is up to the user to call it to maintain the
		''' consistency of the relations. To be kept in mind that if an MBean is
		''' unregistered and the purge not done immediately, if the ObjectName is
		''' reused and assigned to another MBean referenced in a relation, calling
		''' manually this purgeRelations() method will cause trouble, as will
		''' consider the ObjectName as corresponding to the unregistered MBean, not
		''' seeing the new one.
		''' 
		''' <P>The behavior depends on the cardinality of the role where the
		''' unregistered MBean is referenced:
		''' <P>- if removing one MBean reference in the role makes its number of
		''' references less than the minimum degree, the relation has to be removed.
		''' <P>- if the remaining number of references after removing the MBean
		''' reference is still in the cardinality range, keep the relation and
		''' update it calling its handleMBeanUnregistration() callback.
		''' </summary>
		''' <exception cref="RelationServiceNotRegisteredException">  if the Relation
		''' Service is not registered in the MBean Server. </exception>
		Public Overridable Sub purgeRelations() Implements RelationServiceMBean.purgeRelations

			RELATION_LOGGER.entering(GetType(RelationService).name, "purgeRelations")

			' Can throw RelationServiceNotRegisteredException
			active

			' Revisit [cebro] Handle the CIM "Delete" and "IfDeleted" qualifier:
			'    if the unregistered MBean has the "IfDeleted" qualifier,
			'    possible that the relation itself or other referenced MBeans
			'    have to be removed (then a notification would have to be sent
			'    to inform that they should be unregistered.


			' Clones the list of notifications to be able to still receive new
			' notifications while proceeding those ones
			Dim localUnregNtfList As IList(Of javax.management.MBeanServerNotification)
			SyncLock myRefedMBeanObjName2RelIdsMap
				localUnregNtfList = New List(Of javax.management.MBeanServerNotification)(myUnregNtfList)
				' Resets list
				myUnregNtfList = New List(Of javax.management.MBeanServerNotification)
			End SyncLock


			' Updates the listener filter to avoid receiving notifications for
			' those MBeans again
			' Makes also a local "myRefedMBeanObjName2RelIdsMap" map, mapping
			' ObjectName -> relId -> roles, to remove the MBean from the global
			' map
			' List of references to be removed from the listener filter
			Dim obsRefList As IList(Of javax.management.ObjectName) = New List(Of javax.management.ObjectName)
			' Map including ObjectNames for unregistered MBeans, with
			' referencing relation ids and roles
			Dim localMBean2RelIdMap As IDictionary(Of javax.management.ObjectName, IDictionary(Of String, IList(Of String))) = New Dictionary(Of javax.management.ObjectName, IDictionary(Of String, IList(Of String)))

			SyncLock myRefedMBeanObjName2RelIdsMap
				For Each currNtf As javax.management.MBeanServerNotification In localUnregNtfList

					Dim unregMBeanName As javax.management.ObjectName = currNtf.mBeanName

					' Adds the unregsitered MBean in the list of references to
					' remove from the listener filter
					obsRefList.Add(unregMBeanName)

					' Retrieves the associated map of relation ids and roles
					Dim relIdMap As IDictionary(Of String, IList(Of String)) = myRefedMBeanObjName2RelIdsMap(unregMBeanName)
					localMBean2RelIdMap(unregMBeanName) = relIdMap

					myRefedMBeanObjName2RelIdsMap.Remove(unregMBeanName)
				Next currNtf
			End SyncLock

			' Updates the listener
			' Can throw RelationServiceNotRegisteredException
			updateUnregistrationListener(Nothing, obsRefList)

			For Each currNtf As javax.management.MBeanServerNotification In localUnregNtfList

				Dim unregMBeanName As javax.management.ObjectName = currNtf.mBeanName

				' Retrieves the relations where the MBean is referenced
				Dim localRelIdMap As IDictionary(Of String, IList(Of String)) = localMBean2RelIdMap(unregMBeanName)

				' List of relation ids where the unregistered MBean is
				' referenced
				For Each currRel As KeyValuePair(Of String, IList(Of String)) In localRelIdMap
					Dim currRelId As String = currRel.Key
					' List of roles of the relation where the MBean is
					' referenced
					Dim localRoleNameList As IList(Of String) = currRel.Value

					' Checks if the relation has to be removed or not,
					' regarding expected minimum role cardinality and current
					' number of references after removal of the current one
					' If the relation is kept, calls
					' handleMBeanUnregistration() callback of the relation to
					' update it
					'
					' Can throw RelationServiceNotRegisteredException
					'
					' Shall not throw RelationNotFoundException,
					' RoleNotFoundException, MBeanException
					Try
						handleReferenceUnregistration(currRelId, unregMBeanName, localRoleNameList)
					Catch exc1 As RelationNotFoundException
						Throw New Exception(exc1.Message)
					Catch exc2 As RoleNotFoundException
						Throw New Exception(exc2.Message)
					End Try
				Next currRel
			Next currNtf

			RELATION_LOGGER.exiting(GetType(RelationService).name, "purgeRelations")
			Return
		End Sub

		''' <summary>
		''' Retrieves the relations where a given MBean is referenced.
		''' <P>This corresponds to the CIM "References" and "ReferenceNames"
		''' operations.
		''' </summary>
		''' <param name="mbeanName">  ObjectName of MBean </param>
		''' <param name="relationTypeName">  can be null; if specified, only the relations
		''' of that type will be considered in the search. Else all relation types
		''' are considered. </param>
		''' <param name="roleName">  can be null; if specified, only the relations
		''' where the MBean is referenced in that role will be returned. Else all
		''' roles are considered.
		''' </param>
		''' <returns> an HashMap, where the keys are the relation ids of the relations
		''' where the MBean is referenced, and the value is, for each key,
		''' an ArrayList of role names (as an MBean can be referenced in several
		''' roles in the same relation).
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		Public Overridable Function findReferencingRelations(ByVal mbeanName As javax.management.ObjectName, ByVal relationTypeName As String, ByVal roleName As String) As IDictionary(Of String, IList(Of String)) Implements RelationServiceMBean.findReferencingRelations

			If mbeanName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "findReferencingRelations", New Object() {mbeanName, relationTypeName, roleName})

			Dim result As IDictionary(Of String, IList(Of String)) = New Dictionary(Of String, IList(Of String))

			SyncLock myRefedMBeanObjName2RelIdsMap

				' Retrieves the relations referencing the MBean
				Dim relId2RoleNamesMap As IDictionary(Of String, IList(Of String)) = myRefedMBeanObjName2RelIdsMap(mbeanName)

				If relId2RoleNamesMap IsNot Nothing Then

					' Relation Ids where the MBean is referenced
					Dim allRelIdSet As IDictionary(Of String, IList(Of String)).KeyCollection = relId2RoleNamesMap.Keys

					' List of relation ids of interest regarding the selected
					' relation type
					Dim relIdList As IList(Of String)
					If relationTypeName Is Nothing Then
						' Considers all relations
						relIdList = New List(Of String)(allRelIdSet)

					Else

						relIdList = New List(Of String)

						' Considers only the relation ids for relations of given
						' type
						For Each currRelId As String In allRelIdSet

							' Retrieves its relation type
							Dim currRelTypeName As String
							SyncLock myRelId2RelTypeMap
								currRelTypeName = myRelId2RelTypeMap(currRelId)
							End SyncLock

							If currRelTypeName.Equals(relationTypeName) Then relIdList.Add(currRelId)
						Next currRelId
					End If

					' Now looks at the roles where the MBean is expected to be
					' referenced

					For Each currRelId As String In relIdList
						' Retrieves list of role names where the MBean is
						' referenced
						Dim currRoleNameList As IList(Of String) = relId2RoleNamesMap(currRelId)

						If roleName Is Nothing Then
							' All roles to be considered
							' Note: no need to test if list not null before
							'       cloning, MUST be not null else bug :(
							result(currRelId) = New List(Of String)(currRoleNameList)

						ElseIf currRoleNameList.Contains(roleName) Then
							' Filters only the relations where the MBean is
							' referenced in // given role
							Dim dummyList As IList(Of String) = New List(Of String)
							dummyList.Add(roleName)
							result(currRelId) = dummyList
						End If
					Next currRelId
				End If
			End SyncLock

			RELATION_LOGGER.exiting(GetType(RelationService).name, "findReferencingRelations")
			Return result
		End Function

		''' <summary>
		''' Retrieves the MBeans associated to given one in a relation.
		''' <P>This corresponds to CIM Associators and AssociatorNames operations.
		''' </summary>
		''' <param name="mbeanName">  ObjectName of MBean </param>
		''' <param name="relationTypeName">  can be null; if specified, only the relations
		''' of that type will be considered in the search. Else all
		''' relation types are considered. </param>
		''' <param name="roleName">  can be null; if specified, only the relations
		''' where the MBean is referenced in that role will be considered. Else all
		''' roles are considered.
		''' </param>
		''' <returns> an HashMap, where the keys are the ObjectNames of the MBeans
		''' associated to given MBean, and the value is, for each key, an ArrayList
		''' of the relation ids of the relations where the key MBean is
		''' associated to given one (as they can be associated in several different
		''' relations).
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		Public Overridable Function findAssociatedMBeans(ByVal mbeanName As javax.management.ObjectName, ByVal relationTypeName As String, ByVal roleName As String) As IDictionary(Of javax.management.ObjectName, IList(Of String)) Implements RelationServiceMBean.findAssociatedMBeans

			If mbeanName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "findAssociatedMBeans", New Object() {mbeanName, relationTypeName, roleName})

			' Retrieves the map <relation id> -> <role names> for those
			' criterias
			Dim relId2RoleNamesMap As IDictionary(Of String, IList(Of String)) = findReferencingRelations(mbeanName, relationTypeName, roleName)

			Dim result As IDictionary(Of javax.management.ObjectName, IList(Of String)) = New Dictionary(Of javax.management.ObjectName, IList(Of String))

			For Each currRelId As String In relId2RoleNamesMap.Keys

				' Retrieves ObjectNames of MBeans referenced in this relation
				'
				' Shall not throw a RelationNotFoundException if incorrect status
				' of maps :(
				Dim objName2RoleNamesMap As IDictionary(Of javax.management.ObjectName, IList(Of String))
				Try
					objName2RoleNamesMap = getReferencedMBeans(currRelId)
				Catch exc As RelationNotFoundException
					Throw New Exception(exc.Message)
				End Try

				' For each MBean associated to given one in a relation, adds the
				' association <ObjectName> -> <relation id> into result map
				For Each currObjName As javax.management.ObjectName In objName2RoleNamesMap.Keys

					If Not(currObjName.Equals(mbeanName)) Then

						' Sees if this MBean is already associated to the given
						' one in another relation
						Dim currRelIdList As IList(Of String) = result(currObjName)
						If currRelIdList Is Nothing Then

							currRelIdList = New List(Of String)
							currRelIdList.Add(currRelId)
							result(currObjName) = currRelIdList

						Else
							currRelIdList.Add(currRelId)
						End If
					End If
				Next currObjName
			Next currRelId

			RELATION_LOGGER.exiting(GetType(RelationService).name, "findAssociatedMBeans")
			Return result
		End Function

		''' <summary>
		''' Returns the relation ids for relations of the given type.
		''' </summary>
		''' <param name="relationTypeName">  relation type name
		''' </param>
		''' <returns> an ArrayList of relation ids.
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationTypeNotFoundException">  if there is no relation type
		''' with that name. </exception>
		Public Overridable Function findRelationsOfType(ByVal relationTypeName As String) As IList(Of String) Implements RelationServiceMBean.findRelationsOfType

			If relationTypeName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "findRelationsOfType")

			' Can throw RelationTypeNotFoundException
			Dim relType As RelationType = getRelationType(relationTypeName)

			Dim result As IList(Of String)
			SyncLock myRelType2RelIdsMap
				Dim result1 As IList(Of String) = myRelType2RelIdsMap(relationTypeName)
				If result1 Is Nothing Then
					result = New List(Of String)
				Else
					result = New List(Of String)(result1)
				End If
			End SyncLock

			RELATION_LOGGER.exiting(GetType(RelationService).name, "findRelationsOfType")
			Return result
		End Function

		''' <summary>
		''' Retrieves role value for given role name in given relation.
		''' </summary>
		''' <param name="relationId">  relation id </param>
		''' <param name="roleName">  name of role
		''' </param>
		''' <returns> the ArrayList of ObjectName objects being the role value
		''' </returns>
		''' <exception cref="RelationServiceNotRegisteredException">  if the Relation
		''' Service is not registered </exception>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationNotFoundException">  if no relation with given id </exception>
		''' <exception cref="RoleNotFoundException">  if:
		''' <P>- there is no role with given name
		''' <P>or
		''' <P>- the role is not readable.
		''' </exception>
		''' <seealso cref= #setRole </seealso>
		Public Overridable Function getRole(ByVal relationId As String, ByVal roleName As String) As IList(Of javax.management.ObjectName) Implements RelationServiceMBean.getRole

			If relationId Is Nothing OrElse roleName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "getRole", New Object() {relationId, roleName})

			' Can throw RelationServiceNotRegisteredException
			active

			' Can throw a RelationNotFoundException
			Dim relObj As Object = getRelation(relationId)

			Dim result As IList(Of javax.management.ObjectName)

			If TypeOf relObj Is RelationSupport Then
				' Internal relation
				' Can throw RoleNotFoundException
				result = cast(CType(relObj, RelationSupport).getRoleInt(roleName, True, Me, False))

			Else
				' Relation MBean
				Dim params As Object() = New Object(0){}
				params(0) = roleName
				Dim signature As String() = New String(0){}
				signature(0) = "java.lang.String"
				' Can throw MBeanException wrapping a RoleNotFoundException:
				' throw wrapped exception
				'
				' Shall not throw InstanceNotFoundException or ReflectionException
				Try
					Dim invokeResult As IList(Of javax.management.ObjectName) = cast(myMBeanServer.invoke((CType(relObj, javax.management.ObjectName)), "getRole", params, signature))
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					If invokeResult Is Nothing OrElse TypeOf invokeResult Is List(Of ?) Then
						result = invokeResult
					Else
						result = New List(Of javax.management.ObjectName)(invokeResult)
					End If
				Catch exc1 As javax.management.InstanceNotFoundException
					Throw New Exception(exc1.Message)
				Catch exc2 As javax.management.ReflectionException
					Throw New Exception(exc2.Message)
				Catch exc3 As javax.management.MBeanException
					Dim wrappedExc As Exception = exc3.targetException
					If TypeOf wrappedExc Is RoleNotFoundException Then
						Throw (CType(wrappedExc, RoleNotFoundException))
					Else
						Throw New Exception(wrappedExc.Message)
					End If
				End Try
			End If

			RELATION_LOGGER.exiting(GetType(RelationService).name, "getRole")
			Return result
		End Function

		''' <summary>
		''' Retrieves values of roles with given names in given relation.
		''' </summary>
		''' <param name="relationId">  relation id </param>
		''' <param name="roleNameArray">  array of names of roles to be retrieved
		''' </param>
		''' <returns> a RoleResult object, including a RoleList (for roles
		''' successfully retrieved) and a RoleUnresolvedList (for roles not
		''' retrieved).
		''' </returns>
		''' <exception cref="RelationServiceNotRegisteredException">  if the Relation
		''' Service is not registered in the MBean Server </exception>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationNotFoundException">  if no relation with given id
		''' </exception>
		''' <seealso cref= #setRoles </seealso>
		Public Overridable Function getRoles(ByVal relationId As String, ByVal roleNameArray As String()) As RoleResult Implements RelationServiceMBean.getRoles

			If relationId Is Nothing OrElse roleNameArray Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "getRoles", relationId)

			' Can throw RelationServiceNotRegisteredException
			active

			' Can throw a RelationNotFoundException
			Dim relObj As Object = getRelation(relationId)

			Dim result As RoleResult

			If TypeOf relObj Is RelationSupport Then
				' Internal relation
				result = CType(relObj, RelationSupport).getRolesInt(roleNameArray, True, Me)
			Else
				' Relation MBean
				Dim params As Object() = New Object(0){}
				params(0) = roleNameArray
				Dim signature As String() = New String(0){}
				Try
					signature(0) = (roleNameArray.GetType()).name
				Catch exc As Exception
					' OK : This is an array of java.lang.String
					'      so this should never happen...
				End Try
				' Shall not throw InstanceNotFoundException, ReflectionException
				' or MBeanException
				Try
					result = CType(myMBeanServer.invoke((CType(relObj, javax.management.ObjectName)), "getRoles", params, signature), RoleResult)
				Catch exc1 As javax.management.InstanceNotFoundException
					Throw New Exception(exc1.Message)
				Catch exc2 As javax.management.ReflectionException
					Throw New Exception(exc2.Message)
				Catch exc3 As javax.management.MBeanException
					Throw New Exception((exc3.targetException).message)
				End Try
			End If

			RELATION_LOGGER.exiting(GetType(RelationService).name, "getRoles")
			Return result
		End Function

		''' <summary>
		''' Returns all roles present in the relation.
		''' </summary>
		''' <param name="relationId">  relation id
		''' </param>
		''' <returns> a RoleResult object, including a RoleList (for roles
		''' successfully retrieved) and a RoleUnresolvedList (for roles not
		''' readable).
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationNotFoundException">  if no relation for given id </exception>
		''' <exception cref="RelationServiceNotRegisteredException">  if the Relation
		''' Service is not registered in the MBean Server </exception>
		Public Overridable Function getAllRoles(ByVal relationId As String) As RoleResult Implements RelationServiceMBean.getAllRoles

			If relationId Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "getRoles", relationId)

			' Can throw a RelationNotFoundException
			Dim relObj As Object = getRelation(relationId)

			Dim result As RoleResult

			If TypeOf relObj Is RelationSupport Then
				' Internal relation
				result = CType(relObj, RelationSupport).getAllRolesInt(True, Me)

			Else
				' Relation MBean
				' Shall not throw any Exception
				Try
					result = CType(myMBeanServer.getAttribute((CType(relObj, javax.management.ObjectName)), "AllRoles"), RoleResult)
				Catch exc As Exception
					Throw New Exception(exc.Message)
				End Try
			End If

			RELATION_LOGGER.exiting(GetType(RelationService).name, "getRoles")
			Return result
		End Function

		''' <summary>
		''' Retrieves the number of MBeans currently referenced in the given role.
		''' </summary>
		''' <param name="relationId">  relation id </param>
		''' <param name="roleName">  name of role
		''' </param>
		''' <returns> the number of currently referenced MBeans in that role
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationNotFoundException">  if no relation with given id </exception>
		''' <exception cref="RoleNotFoundException">  if there is no role with given name </exception>
		Public Overridable Function getRoleCardinality(ByVal relationId As String, ByVal roleName As String) As Integer?

			If relationId Is Nothing OrElse roleName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "getRoleCardinality", New Object() {relationId, roleName})

			' Can throw a RelationNotFoundException
			Dim relObj As Object = getRelation(relationId)

			Dim result As Integer?

			If TypeOf relObj Is RelationSupport Then
				' Internal relation
				' Can throw RoleNotFoundException
				result = CType(relObj, RelationSupport).getRoleCardinality(roleName)

			Else
				' Relation MBean
				Dim params As Object() = New Object(0){}
				params(0) = roleName
				Dim signature As String() = New String(0){}
				signature(0) = "java.lang.String"
				' Can throw MBeanException wrapping RoleNotFoundException:
				' throw wrapped exception
				'
				' Shall not throw InstanceNotFoundException or ReflectionException
				Try
					result = CInt(Fix(myMBeanServer.invoke((CType(relObj, javax.management.ObjectName)), "getRoleCardinality", params, signature)))
				Catch exc1 As javax.management.InstanceNotFoundException
					Throw New Exception(exc1.Message)
				Catch exc2 As javax.management.ReflectionException
					Throw New Exception(exc2.Message)
				Catch exc3 As javax.management.MBeanException
					Dim wrappedExc As Exception = exc3.targetException
					If TypeOf wrappedExc Is RoleNotFoundException Then
						Throw (CType(wrappedExc, RoleNotFoundException))
					Else
						Throw New Exception(wrappedExc.Message)
					End If
				End Try
			End If

			RELATION_LOGGER.exiting(GetType(RelationService).name, "getRoleCardinality")
			Return result
		End Function

		''' <summary>
		''' Sets the given role in given relation.
		''' <P>Will check the role according to its corresponding role definition
		''' provided in relation's relation type
		''' <P>The Relation Service will keep track of the change to keep the
		''' consistency of relations by handling referenced MBean deregistrations.
		''' </summary>
		''' <param name="relationId">  relation id </param>
		''' <param name="role">  role to be set (name and new value)
		''' </param>
		''' <exception cref="RelationServiceNotRegisteredException">  if the Relation
		''' Service is not registered in the MBean Server </exception>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationNotFoundException">  if no relation with given id </exception>
		''' <exception cref="RoleNotFoundException">  if the role does not exist or is not
		''' writable </exception>
		''' <exception cref="InvalidRoleValueException">  if value provided for role is not
		''' valid:
		''' <P>- the number of referenced MBeans in given value is less than
		''' expected minimum degree
		''' <P>or
		''' <P>- the number of referenced MBeans in provided value exceeds expected
		''' maximum degree
		''' <P>or
		''' <P>- one referenced MBean in the value is not an Object of the MBean
		''' class expected for that role
		''' <P>or
		''' <P>- an MBean provided for that role does not exist
		''' </exception>
		''' <seealso cref= #getRole </seealso>
		Public Overridable Sub setRole(ByVal relationId As String, ByVal ___role As Role) Implements RelationServiceMBean.setRole

			If relationId Is Nothing OrElse ___role Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "setRole", New Object() {relationId, ___role})

			' Can throw RelationServiceNotRegisteredException
			active

			' Can throw a RelationNotFoundException
			Dim relObj As Object = getRelation(relationId)

			If TypeOf relObj Is RelationSupport Then
				' Internal relation
				' Can throw RoleNotFoundException,
				' InvalidRoleValueException and
				' RelationServiceNotRegisteredException
				'
				' Shall not throw RelationTypeNotFoundException
				' (as relation exists in the RS, its relation type is known)
				Try
					CType(relObj, RelationSupport).roleIntInt(___role, True, Me, False)

				Catch exc As RelationTypeNotFoundException
					Throw New Exception(exc.Message)
				End Try

			Else
				' Relation MBean
				Dim params As Object() = New Object(0){}
				params(0) = ___role
				Dim signature As String() = New String(0){}
				signature(0) = "javax.management.relation.Role"
				' Can throw MBeanException wrapping RoleNotFoundException,
				' InvalidRoleValueException
				'
				' Shall not MBeanException wrapping an MBeanException wrapping
				' RelationTypeNotFoundException, or ReflectionException, or
				' InstanceNotFoundException
				Try
					myMBeanServer.attributeute((CType(relObj, javax.management.ObjectName)), New javax.management.Attribute("Role", ___role))

				Catch exc1 As javax.management.InstanceNotFoundException
					Throw New Exception(exc1.Message)
				Catch exc3 As javax.management.ReflectionException
					Throw New Exception(exc3.Message)
				Catch exc2 As javax.management.MBeanException
					Dim wrappedExc As Exception = exc2.targetException
					If TypeOf wrappedExc Is RoleNotFoundException Then
						Throw (CType(wrappedExc, RoleNotFoundException))
					ElseIf TypeOf wrappedExc Is InvalidRoleValueException Then
						Throw (CType(wrappedExc, InvalidRoleValueException))
					Else
						Throw New Exception(wrappedExc.Message)

					End If
				Catch exc4 As javax.management.AttributeNotFoundException
				  Throw New Exception(exc4.Message)
				Catch exc5 As javax.management.InvalidAttributeValueException
				  Throw New Exception(exc5.Message)
				End Try
			End If

			RELATION_LOGGER.exiting(GetType(RelationService).name, "setRole")
			Return
		End Sub

		''' <summary>
		''' Sets the given roles in given relation.
		''' <P>Will check the role according to its corresponding role definition
		''' provided in relation's relation type
		''' <P>The Relation Service keeps track of the changes to keep the
		''' consistency of relations by handling referenced MBean deregistrations.
		''' </summary>
		''' <param name="relationId">  relation id </param>
		''' <param name="roleList">  list of roles to be set
		''' </param>
		''' <returns> a RoleResult object, including a RoleList (for roles
		''' successfully set) and a RoleUnresolvedList (for roles not
		''' set).
		''' </returns>
		''' <exception cref="RelationServiceNotRegisteredException">  if the Relation
		''' Service is not registered in the MBean Server </exception>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationNotFoundException">  if no relation with given id
		''' </exception>
		''' <seealso cref= #getRoles </seealso>
		Public Overridable Function setRoles(ByVal relationId As String, ByVal roleList As RoleList) As RoleResult Implements RelationServiceMBean.setRoles

			If relationId Is Nothing OrElse roleList Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "setRoles", New Object() {relationId, roleList})

			' Can throw RelationServiceNotRegisteredException
			active

			' Can throw a RelationNotFoundException
			Dim relObj As Object = getRelation(relationId)

			Dim result As RoleResult

			If TypeOf relObj Is RelationSupport Then
				' Internal relation
				' Can throw RelationServiceNotRegisteredException
				'
				' Shall not throw RelationTypeNotFoundException (as relation is
				' known, its relation type exists)
				Try
					result = CType(relObj, RelationSupport).rolesIntInt(roleList, True, Me)
				Catch exc As RelationTypeNotFoundException
					Throw New Exception(exc.Message)
				End Try

			Else
				' Relation MBean
				Dim params As Object() = New Object(0){}
				params(0) = roleList
				Dim signature As String() = New String(0){}
				signature(0) = "javax.management.relation.RoleList"
				' Shall not throw InstanceNotFoundException or an MBeanException
				' or ReflectionException
				Try
					result = CType(myMBeanServer.invoke((CType(relObj, javax.management.ObjectName)), "setRoles", params, signature), RoleResult)
				Catch exc1 As javax.management.InstanceNotFoundException
					Throw New Exception(exc1.Message)
				Catch exc3 As javax.management.ReflectionException
					Throw New Exception(exc3.Message)
				Catch exc2 As javax.management.MBeanException
					Throw New Exception((exc2.targetException).message)
				End Try
			End If

			RELATION_LOGGER.exiting(GetType(RelationService).name, "setRoles")
			Return result
		End Function

		''' <summary>
		''' Retrieves MBeans referenced in the various roles of the relation.
		''' </summary>
		''' <param name="relationId">  relation id
		''' </param>
		''' <returns> a HashMap mapping:
		''' <P> ObjectName {@literal ->} ArrayList of String (role names)
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationNotFoundException">  if no relation for given
		''' relation id </exception>
		Public Overridable Function getReferencedMBeans(ByVal relationId As String) As IDictionary(Of javax.management.ObjectName, IList(Of String)) Implements RelationServiceMBean.getReferencedMBeans

			If relationId Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "getReferencedMBeans", relationId)

			' Can throw a RelationNotFoundException
			Dim relObj As Object = getRelation(relationId)

			Dim result As IDictionary(Of javax.management.ObjectName, IList(Of String))

			If TypeOf relObj Is RelationSupport Then
				' Internal relation
				result = CType(relObj, RelationSupport).referencedMBeans

			Else
				' Relation MBean
				' No Exception
				Try
					result = cast(myMBeanServer.getAttribute((CType(relObj, javax.management.ObjectName)), "ReferencedMBeans"))
				Catch exc As Exception
					Throw New Exception(exc.Message)
				End Try
			End If

			RELATION_LOGGER.exiting(GetType(RelationService).name, "getReferencedMBeans")
			Return result
		End Function

		''' <summary>
		''' Returns name of associated relation type for given relation.
		''' </summary>
		''' <param name="relationId">  relation id
		''' </param>
		''' <returns> the name of the associated relation type.
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationNotFoundException">  if no relation for given
		''' relation id </exception>
		Public Overridable Function getRelationTypeName(ByVal relationId As String) As String Implements RelationServiceMBean.getRelationTypeName

			If relationId Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "getRelationTypeName", relationId)

			' Can throw a RelationNotFoundException
			Dim relObj As Object = getRelation(relationId)

			Dim result As String

			If TypeOf relObj Is RelationSupport Then
				' Internal relation
				result = CType(relObj, RelationSupport).relationTypeName

			Else
				' Relation MBean
				' No Exception
				Try
					result = CStr(myMBeanServer.getAttribute((CType(relObj, javax.management.ObjectName)), "RelationTypeName"))
				Catch exc As Exception
					Throw New Exception(exc.Message)
				End Try
			End If

			RELATION_LOGGER.exiting(GetType(RelationService).name, "getRelationTypeName")
			Return result
		End Function

		'
		' NotificationListener Interface
		'

		''' <summary>
		''' Invoked when a JMX notification occurs.
		''' Currently handles notifications for unregistration of MBeans, either
		''' referenced in a relation role or being a relation itself.
		''' </summary>
		''' <param name="notif">  The notification. </param>
		''' <param name="handback">  An opaque object which helps the listener to
		''' associate information regarding the MBean emitter (can be null). </param>
		Public Overridable Sub handleNotification(ByVal notif As javax.management.Notification, ByVal handback As Object)

			If notif Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "handleNotification", notif)

			If TypeOf notif Is javax.management.MBeanServerNotification Then

				Dim mbsNtf As javax.management.MBeanServerNotification = CType(notif, javax.management.MBeanServerNotification)
				Dim ntfType As String = notif.type

				If ntfType.Equals(javax.management.MBeanServerNotification.UNREGISTRATION_NOTIFICATION) Then
					Dim mbeanName As javax.management.ObjectName = CType(notif, javax.management.MBeanServerNotification).mBeanName

					' Note: use a flag to block access to
					' myRefedMBeanObjName2RelIdsMap only for a quick access
					Dim isRefedMBeanFlag As Boolean = False
					SyncLock myRefedMBeanObjName2RelIdsMap

						If myRefedMBeanObjName2RelIdsMap.ContainsKey(mbeanName) Then
							' Unregistration of a referenced MBean
							SyncLock myUnregNtfList
								myUnregNtfList.Add(mbsNtf)
							End SyncLock
							isRefedMBeanFlag = True
						End If
						If isRefedMBeanFlag AndAlso myPurgeFlag Then
							' Immediate purge
							' Can throw RelationServiceNotRegisteredException
							' but assume that will be fine :)
							Try
								purgeRelations()
							Catch exc As Exception
								Throw New Exception(exc.Message)
							End Try
						End If
					End SyncLock

					' Note: do both tests as a relation can be an MBean and be
					'       itself referenced in another relation :)
					Dim relId As String
					SyncLock myRelMBeanObjName2RelIdMap
						relId = myRelMBeanObjName2RelIdMap(mbeanName)
					End SyncLock
					If relId IsNot Nothing Then
						' Unregistration of a relation MBean
						' Can throw RelationTypeNotFoundException,
						' RelationServiceNotRegisteredException
						'
						' Shall not throw RelationTypeNotFoundException or
						' InstanceNotFoundException
						Try
							removeRelation(relId)
						Catch exc As Exception
							Throw New Exception(exc.Message)
						End Try
					End If
				End If
			End If

			RELATION_LOGGER.exiting(GetType(RelationService).name, "handleNotification")
			Return
		End Sub

		'
		' NotificationBroadcaster interface
		'

		''' <summary>
		''' Returns a NotificationInfo object containing the name of the Java class
		''' of the notification and the notification types sent.
		''' </summary>
		Public Property Overrides notificationInfo As javax.management.MBeanNotificationInfo()
			Get
    
				RELATION_LOGGER.entering(GetType(RelationService).name, "getNotificationInfo")
    
				Dim ntfClass As String = "javax.management.relation.RelationNotification"
    
				Dim ntfTypes As String() = { RelationNotification.RELATION_BASIC_CREATION, RelationNotification.RELATION_MBEAN_CREATION, RelationNotification.RELATION_BASIC_UPDATE, RelationNotification.RELATION_MBEAN_UPDATE, RelationNotification.RELATION_BASIC_REMOVAL, RelationNotification.RELATION_MBEAN_REMOVAL }
    
				Dim ntfDesc As String = "Sent when a relation is created, updated or deleted."
    
				Dim ntfInfo As New javax.management.MBeanNotificationInfo(ntfTypes, ntfClass, ntfDesc)
    
				RELATION_LOGGER.exiting(GetType(RelationService).name, "getNotificationInfo")
				Return New javax.management.MBeanNotificationInfo() {ntfInfo}
			End Get
		End Property

		'
		' Misc
		'

		' Adds given object as a relation type.
		'
		' -param relationTypeObj  relation type object
		'
		' -exception IllegalArgumentException  if null parameter
		' -exception InvalidRelationTypeException  if there is already a relation
		'  type with that name
		Private Sub addRelationTypeInt(ByVal relationTypeObj As RelationType)

			If relationTypeObj Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "addRelationTypeInt")

			Dim relTypeName As String = relationTypeObj.relationTypeName

			' Checks that there is not already a relation type with that name
			' existing in the Relation Service
			Try
				' Can throw a RelationTypeNotFoundException (in fact should ;)
				Dim relType As RelationType = getRelationType(relTypeName)

				If relType IsNot Nothing Then
					Dim excMsg As String = "There is already a relation type in the Relation Service with name "
					Dim excMsgStrB As New StringBuilder(excMsg)
					excMsgStrB.Append(relTypeName)
					Throw New InvalidRelationTypeException(excMsgStrB.ToString())
				End If

			Catch exc As RelationTypeNotFoundException
				' OK : The RelationType could not be found.
			End Try

			' Adds the relation type
			SyncLock myRelType2ObjMap
				myRelType2ObjMap(relTypeName) = relationTypeObj
			End SyncLock

			If TypeOf relationTypeObj Is RelationTypeSupport Then CType(relationTypeObj, RelationTypeSupport).relationServiceFlag = True

			RELATION_LOGGER.exiting(GetType(RelationService).name, "addRelationTypeInt")
			Return
		End Sub

		' Retrieves relation type with given name
		'
		' -param relationTypeName  expected name of a relation type created in the
		'  Relation Service
		'
		' -return RelationType object corresponding to given name
		'
		' -exception IllegalArgumentException  if null parameter
		' -exception RelationTypeNotFoundException  if no relation type for that
		'  name created in Relation Service
		'
		Friend Overridable Function getRelationType(ByVal relationTypeName As String) As RelationType

			If relationTypeName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "getRelationType", relationTypeName)

			' No null relation type accepted, so can use get()
			Dim relType As RelationType
			SyncLock myRelType2ObjMap
				relType = (myRelType2ObjMap(relationTypeName))
			End SyncLock

			If relType Is Nothing Then
				Dim excMsg As String = "No relation type created in the Relation Service with the name "
				Dim excMsgStrB As New StringBuilder(excMsg)
				excMsgStrB.Append(relationTypeName)
				Throw New RelationTypeNotFoundException(excMsgStrB.ToString())
			End If

			RELATION_LOGGER.exiting(GetType(RelationService).name, "getRelationType")
			Return relType
		End Function

		' Retrieves relation corresponding to given relation id.
		' Returns either:
		' - a RelationSupport object if the relation is internal
		' or
		' - the ObjectName of the corresponding MBean
		'
		' -param relationId  expected relation id
		'
		' -return RelationSupport object or ObjectName of relation with given id
		'
		' -exception IllegalArgumentException  if null parameter
		' -exception RelationNotFoundException  if no relation for that
		'  relation id created in Relation Service
		'
		Friend Overridable Function getRelation(ByVal relationId As String) As Object

			If relationId Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "getRelation", relationId)

			' No null relation  accepted, so can use get()
			Dim rel As Object
			SyncLock myRelId2ObjMap
				rel = myRelId2ObjMap(relationId)
			End SyncLock

			If rel Is Nothing Then
				Dim excMsg As String = "No relation associated to relation id " & relationId
				Throw New RelationNotFoundException(excMsg)
			End If

			RELATION_LOGGER.exiting(GetType(RelationService).name, "getRelation")
			Return rel
		End Function

		' Adds a new MBean reference (reference to an ObjectName) in the
		' referenced MBean map (myRefedMBeanObjName2RelIdsMap).
		'
		' -param objectName  ObjectName of new referenced MBean
		' -param relationId  relation id of the relation where the MBean is
		'  referenced
		' -param roleName  name of the role where the MBean is referenced
		'
		' -return boolean:
		'  - true  if the MBean was not referenced before, so really a new
		'    reference
		'  - false else
		'
		' -exception IllegalArgumentException  if null parameter
		Private Function addNewMBeanReference(ByVal ___objectName As javax.management.ObjectName, ByVal relationId As String, ByVal roleName As String) As Boolean

			If ___objectName Is Nothing OrElse relationId Is Nothing OrElse roleName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "addNewMBeanReference", New Object() {___objectName, relationId, roleName})

			Dim isNewFlag As Boolean = False

			SyncLock myRefedMBeanObjName2RelIdsMap

				' Checks if the MBean was already referenced
				' No null value allowed, use get() directly
				Dim mbeanRefMap As IDictionary(Of String, IList(Of String)) = myRefedMBeanObjName2RelIdsMap(___objectName)

				If mbeanRefMap Is Nothing Then
					' MBean not referenced in any relation yet

					isNewFlag = True

					' List of roles where the MBean is referenced in given
					' relation
					Dim roleNames As IList(Of String) = New List(Of String)
					roleNames.Add(roleName)

					' Map of relations where the MBean is referenced
					mbeanRefMap = New Dictionary(Of String, IList(Of String))
					mbeanRefMap(relationId) = roleNames

					myRefedMBeanObjName2RelIdsMap(___objectName) = mbeanRefMap

				Else
					' MBean already referenced in at least another relation
					' Checks if already referenced in another role in current
					' relation
					Dim roleNames As IList(Of String) = mbeanRefMap(relationId)

					If roleNames Is Nothing Then
						' MBean not referenced in current relation

						' List of roles where the MBean is referenced in given
						' relation
						roleNames = New List(Of String)
						roleNames.Add(roleName)

						' Adds new reference done in current relation
						mbeanRefMap(relationId) = roleNames

					Else
						' MBean already referenced in current relation in another
						' role
						' Adds new reference done
						roleNames.Add(roleName)
					End If
				End If
			End SyncLock

			RELATION_LOGGER.exiting(GetType(RelationService).name, "addNewMBeanReference")
			Return isNewFlag
		End Function

		' Removes an obsolete MBean reference (reference to an ObjectName) in
		' the referenced MBean map (myRefedMBeanObjName2RelIdsMap).
		'
		' -param objectName  ObjectName of MBean no longer referenced
		' -param relationId  relation id of the relation where the MBean was
		'  referenced
		' -param roleName  name of the role where the MBean was referenced
		' -param allRolesFlag  flag, if true removes reference to MBean for all
		'  roles in the relation, not only for the one above
		'
		' -return boolean:
		'  - true  if the MBean is no longer reference in any relation
		'  - false else
		'
		' -exception IllegalArgumentException  if null parameter
		Private Function removeMBeanReference(ByVal ___objectName As javax.management.ObjectName, ByVal relationId As String, ByVal roleName As String, ByVal allRolesFlag As Boolean) As Boolean

			If ___objectName Is Nothing OrElse relationId Is Nothing OrElse roleName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "removeMBeanReference", New Object() {___objectName, relationId, roleName, allRolesFlag})

			Dim noLongerRefFlag As Boolean = False

			SyncLock myRefedMBeanObjName2RelIdsMap

				' Retrieves the set of relations (designed via their relation ids)
				' where the MBean is referenced
				' Note that it is possible that the MBean has already been removed
				' from the internal map: this is the case when the MBean is
				' unregistered, the role is updated, then we arrive here.
				Dim mbeanRefMap As IDictionary(Of String, IList(Of String)) = (myRefedMBeanObjName2RelIdsMap(___objectName))

				If mbeanRefMap Is Nothing Then
					' The MBean is no longer referenced
					RELATION_LOGGER.exiting(GetType(RelationService).name, "removeMBeanReference")
					Return True
				End If

				Dim roleNames As IList(Of String) = Nothing
				If Not allRolesFlag Then
					' Now retrieves the roles of current relation where the MBean
					' was referenced
					roleNames = mbeanRefMap(relationId)

					' Removes obsolete reference to role
					Dim obsRefIdx As Integer = roleNames.IndexOf(roleName)
					If obsRefIdx <> -1 Then roleNames.RemoveAt(obsRefIdx)
				End If

				' Checks if there is still at least one role in current relation
				' where the MBean is referenced
				If roleNames.Count = 0 OrElse allRolesFlag Then mbeanRefMap.Remove(relationId)

				' Checks if the MBean is still referenced in at least on relation
				If mbeanRefMap.Count = 0 Then
					' MBean no longer referenced in any relation: removes entry
					myRefedMBeanObjName2RelIdsMap.Remove(___objectName)
					noLongerRefFlag = True
				End If
			End SyncLock

			RELATION_LOGGER.exiting(GetType(RelationService).name, "removeMBeanReference")
			Return noLongerRefFlag
		End Function

		' Updates the listener registered to the MBean Server to be informed of
		' referenced MBean deregistrations
		'
		' -param newRefList  ArrayList of ObjectNames for new references done
		'  to MBeans (can be null)
		' -param obsoleteRefList  ArrayList of ObjectNames for obsolete references
		'  to MBeans (can be null)
		'
		' -exception RelationServiceNotRegisteredException  if the Relation
		'  Service is not registered in the MBean Server.
		Private Sub updateUnregistrationListener(ByVal newRefList As IList(Of javax.management.ObjectName), ByVal obsoleteRefList As IList(Of javax.management.ObjectName))

			If newRefList IsNot Nothing AndAlso obsoleteRefList IsNot Nothing Then
				If newRefList.Count = 0 AndAlso obsoleteRefList.Count = 0 Then Return
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "updateUnregistrationListener", New Object() {newRefList, obsoleteRefList})

			' Can throw RelationServiceNotRegisteredException
			active

			If newRefList IsNot Nothing OrElse obsoleteRefList IsNot Nothing Then

				Dim newListenerFlag As Boolean = False
				If myUnregNtfFilter Is Nothing Then
					' Initialize it to be able to synchronise it :)
					myUnregNtfFilter = New MBeanServerNotificationFilter
					newListenerFlag = True
				End If

				SyncLock myUnregNtfFilter

					' Enables ObjectNames in newRefList
					If newRefList IsNot Nothing Then
						For Each newObjName As javax.management.ObjectName In newRefList
							myUnregNtfFilter.enableObjectName(newObjName)
						Next newObjName
					End If

					If obsoleteRefList IsNot Nothing Then
						' Disables ObjectNames in obsoleteRefList
						For Each obsObjName As javax.management.ObjectName In obsoleteRefList
							myUnregNtfFilter.disableObjectName(obsObjName)
						Next obsObjName
					End If

	' Under test
					If newListenerFlag Then
						Try
							myMBeanServer.addNotificationListener(javax.management.MBeanServerDelegate.DELEGATE_NAME, Me, myUnregNtfFilter, Nothing)
						Catch exc As javax.management.InstanceNotFoundException
							Throw New RelationServiceNotRegisteredException(exc.Message)
						End Try
					End If
	' End test


	'              if (!newListenerFlag) {
						' The Relation Service was already registered as a
						' listener:
						' removes it
						' Shall not throw InstanceNotFoundException (as the
						' MBean Server Delegate is expected to exist) or
						' ListenerNotFoundException (as it has been checked above
						' that the Relation Service is registered)
	'                  try {
	'                      myMBeanServer.removeNotificationListener(
	'                              MBeanServerDelegate.DELEGATE_NAME,
	'                              this);
	'                  } catch (InstanceNotFoundException exc1) {
	'                      throw new RuntimeException(exc1.getMessage());
	'                  } catch (ListenerNotFoundException exc2) {
	'                      throw new
	'                          RelationServiceNotRegisteredException(exc2.getMessage());
	'                  }
	'              }

					' Adds Relation Service with current filter
					' Can throw InstanceNotFoundException if the Relation
					' Service is not registered, to be transformed into
					' RelationServiceNotRegisteredException
					'
					' Assume that there will not be any InstanceNotFoundException
					' for the MBean Server Delegate :)
	'              try {
	'                  myMBeanServer.addNotificationListener(
	'                              MBeanServerDelegate.DELEGATE_NAME,
	'                              this,
	'                              myUnregNtfFilter,
	'                              null);
	'              } catch (InstanceNotFoundException exc) {
	'                  throw new
	'                     RelationServiceNotRegisteredException(exc.getMessage());
	'              }
				End SyncLock
			End If

			RELATION_LOGGER.exiting(GetType(RelationService).name, "updateUnregistrationListener")
			Return
		End Sub

		' Adds a relation (being either a RelationSupport object or an MBean
		' referenced using its ObjectName) in the Relation Service.
		' Will send a notification RelationNotification with type:
		' - RelationNotification.RELATION_BASIC_CREATION for internal relation
		'   creation
		' - RelationNotification.RELATION_MBEAN_CREATION for an MBean being added
		'   as a relation.
		'
		' -param relationBaseFlag  flag true if the relation is a RelationSupport
		'  object, false if it is an MBean
		' -param relationObj  RelationSupport object (if relation is internal)
		' -param relationObjName  ObjectName of the MBean to be added as a relation
		'  (only for the relation MBean)
		' -param relationId  relation identifier, to uniquely identify the relation
		'  inside the Relation Service
		' -param relationTypeName  name of the relation type (has to be created
		'  in the Relation Service)
		' -param roleList  role list to initialize roles of the relation
		'  (can be null)
		'
		' -exception IllegalArgumentException  if null paramater
		' -exception RelationServiceNotRegisteredException  if the Relation
		'  Service is not registered in the MBean Server
		' -exception RoleNotFoundException  if a value is provided for a role
		'  that does not exist in the relation type
		' -exception InvalidRelationIdException  if relation id already used
		' -exception RelationTypeNotFoundException  if relation type not known in
		'  Relation Service
		' -exception InvalidRoleValueException if:
		'  - the same role name is used for two different roles
		'  - the number of referenced MBeans in given value is less than
		'    expected minimum degree
		'  - the number of referenced MBeans in provided value exceeds expected
		'    maximum degree
		'  - one referenced MBean in the value is not an Object of the MBean
		'    class expected for that role
		'  - an MBean provided for that role does not exist
		Private Sub addRelationInt(ByVal relationBaseFlag As Boolean, ByVal relationObj As RelationSupport, ByVal relationObjName As javax.management.ObjectName, ByVal relationId As String, ByVal relationTypeName As String, ByVal roleList As RoleList)

			If relationId Is Nothing OrElse relationTypeName Is Nothing OrElse (relationBaseFlag AndAlso (relationObj Is Nothing OrElse relationObjName IsNot Nothing)) OrElse ((Not relationBaseFlag) AndAlso (relationObjName Is Nothing OrElse relationObj IsNot Nothing)) Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "addRelationInt", New Object() {relationBaseFlag, relationObj, relationObjName, relationId, relationTypeName, roleList})

			' Can throw RelationServiceNotRegisteredException
			active

			' Checks if there is already a relation with given id
			Try
				' Can throw a RelationNotFoundException (in fact should :)
				Dim rel As Object = getRelation(relationId)

				If rel IsNot Nothing Then
					' There is already a relation with that id
					Dim excMsg As String = "There is already a relation with id "
					Dim excMsgStrB As New StringBuilder(excMsg)
					excMsgStrB.Append(relationId)
					Throw New InvalidRelationIdException(excMsgStrB.ToString())
				End If
			Catch exc As RelationNotFoundException
				' OK : The Relation could not be found.
			End Try

			' Retrieves the relation type
			' Can throw RelationTypeNotFoundException
			Dim relType As RelationType = getRelationType(relationTypeName)

			' Checks that each provided role conforms to its role info provided in
			' the relation type
			' First retrieves a local list of the role infos of the relation type
			' to see which roles have not been initialized
			' Note: no need to test if list not null before cloning, not allowed
			'       to have an empty relation type.
			Dim roleInfoList As IList(Of RoleInfo) = New List(Of RoleInfo)(relType.roleInfos)

			If roleList IsNot Nothing Then

				For Each currRole As Role In roleList.asList()
					Dim currRoleName As String = currRole.roleName
					Dim currRoleValue As IList(Of javax.management.ObjectName) = currRole.roleValue
					' Retrieves corresponding role info
					' Can throw a RoleInfoNotFoundException to be converted into a
					' RoleNotFoundException
					Dim ___roleInfo As RoleInfo
					Try
						___roleInfo = relType.getRoleInfo(currRoleName)
					Catch exc As RoleInfoNotFoundException
						Throw New RoleNotFoundException(exc.Message)
					End Try

					' Checks that role conforms to role info,
					Dim status As Integer? = checkRoleInt(2, currRoleName, currRoleValue, ___roleInfo, False)
					Dim pbType As Integer = status
					If pbType <> 0 Then throwRoleProblemException(pbType, currRoleName)

					' Removes role info for that list from list of role infos for
					' roles to be defaulted
					Dim roleInfoIdx As Integer = roleInfoList.IndexOf(___roleInfo)
					' Note: no need to check if != -1, MUST be there :)
					roleInfoList.RemoveAt(roleInfoIdx)
				Next currRole
			End If

			' Initializes roles not initialized by roleList
			' Can throw InvalidRoleValueException
			initializeMissingRoles(relationBaseFlag, relationObj, relationObjName, relationId, relationTypeName, roleInfoList)

			' Creation of relation successfull!!!!

			' Updates internal maps
			' Relation id to object map
			SyncLock myRelId2ObjMap
				If relationBaseFlag Then
					' Note: do not clone relation object, created by us :)
					myRelId2ObjMap(relationId) = relationObj
				Else
					myRelId2ObjMap(relationId) = relationObjName
				End If
			End SyncLock

			' Relation id to relation type name map
			SyncLock myRelId2RelTypeMap
				myRelId2RelTypeMap(relationId) = relationTypeName
			End SyncLock

			' Relation type to relation id map
			SyncLock myRelType2RelIdsMap
				Dim relIdList As IList(Of String) = myRelType2RelIdsMap(relationTypeName)
				Dim firstRelFlag As Boolean = False
				If relIdList Is Nothing Then
					firstRelFlag = True
					relIdList = New List(Of String)
				End If
				relIdList.Add(relationId)
				If firstRelFlag Then myRelType2RelIdsMap(relationTypeName) = relIdList
			End SyncLock

			' Referenced MBean to relation id map
			' Only role list parameter used, as default initialization of roles
			' done automatically in initializeMissingRoles() sets each
			' uninitialized role to an empty value.
			For Each currRole As Role In roleList.asList()
				' Creates a dummy empty ArrayList of ObjectNames to be the old
				' role value :)
				Dim dummyList As IList(Of javax.management.ObjectName) = New List(Of javax.management.ObjectName)
				' Will not throw a RelationNotFoundException (as the RelId2Obj map
				' has been updated above) so catch it :)
				Try
					updateRoleMap(relationId, currRole, dummyList)

				Catch exc As RelationNotFoundException
					' OK : The Relation could not be found.
				End Try
			Next currRole

			' Sends a notification for relation creation
			' Will not throw RelationNotFoundException so catch it :)
			Try
				sendRelationCreationNotification(relationId)

			Catch exc As RelationNotFoundException
				' OK : The Relation could not be found.
			End Try

			RELATION_LOGGER.exiting(GetType(RelationService).name, "addRelationInt")
			Return
		End Sub

		' Checks that given role conforms to given role info.
		'
		' -param chkType  type of check:
		'  - 1: read, just check read access
		'  - 2: write, check value and write access if writeChkFlag
		' -param roleName  role name
		' -param roleValue  role value
		' -param roleInfo  corresponding role info
		' -param writeChkFlag  boolean to specify a current write access and
		'  to check it
		'
		' -return Integer with value:
		'  - 0: ok
		'  - RoleStatus.NO_ROLE_WITH_NAME
		'  - RoleStatus.ROLE_NOT_READABLE
		'  - RoleStatus.ROLE_NOT_WRITABLE
		'  - RoleStatus.LESS_THAN_MIN_ROLE_DEGREE
		'  - RoleStatus.MORE_THAN_MAX_ROLE_DEGREE
		'  - RoleStatus.REF_MBEAN_OF_INCORRECT_CLASS
		'  - RoleStatus.REF_MBEAN_NOT_REGISTERED
		'
		' -exception IllegalArgumentException  if null parameter
		Private Function checkRoleInt(ByVal chkType As Integer, ByVal roleName As String, ByVal roleValue As IList(Of javax.management.ObjectName), ByVal ___roleInfo As RoleInfo, ByVal writeChkFlag As Boolean) As Integer?

			If roleName Is Nothing OrElse ___roleInfo Is Nothing OrElse (chkType = 2 AndAlso roleValue Is Nothing) Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "checkRoleInt", New Object() {chkType, roleName, roleValue, ___roleInfo, writeChkFlag})

			' Compares names
			Dim expName As String = ___roleInfo.name
			If Not(roleName.Equals(expName)) Then
				RELATION_LOGGER.exiting(GetType(RelationService).name, "checkRoleInt")
				Return Convert.ToInt32(RoleStatus.NO_ROLE_WITH_NAME)
			End If

			' Checks read access if required
			If chkType = 1 Then
				Dim isReadable As Boolean = ___roleInfo.readable
				If Not isReadable Then
					RELATION_LOGGER.exiting(GetType(RelationService).name, "checkRoleInt")
					Return Convert.ToInt32(RoleStatus.ROLE_NOT_READABLE)
				Else
					' End of check :)
					RELATION_LOGGER.exiting(GetType(RelationService).name, "checkRoleInt")
					Return New Integer?(0)
				End If
			End If

			' Checks write access if required
			If writeChkFlag Then
				Dim isWritable As Boolean = ___roleInfo.writable
				If Not isWritable Then
					RELATION_LOGGER.exiting(GetType(RelationService).name, "checkRoleInt")
					Return New Integer?(RoleStatus.ROLE_NOT_WRITABLE)
				End If
			End If

			Dim refNbr As Integer = roleValue.Count

			' Checks minimum cardinality
			Dim chkMinFlag As Boolean = ___roleInfo.checkMinDegree(refNbr)
			If Not chkMinFlag Then
				RELATION_LOGGER.exiting(GetType(RelationService).name, "checkRoleInt")
				Return New Integer?(RoleStatus.LESS_THAN_MIN_ROLE_DEGREE)
			End If

			' Checks maximum cardinality
			Dim chkMaxFlag As Boolean = ___roleInfo.checkMaxDegree(refNbr)
			If Not chkMaxFlag Then
				RELATION_LOGGER.exiting(GetType(RelationService).name, "checkRoleInt")
				Return New Integer?(RoleStatus.MORE_THAN_MAX_ROLE_DEGREE)
			End If

			' Verifies that each referenced MBean is registered in the MBean
			' Server and that it is an instance of the class specified in the
			' role info, or of a subclass of it
			' Note that here again this is under the assumption that
			' referenced MBeans, relation MBeans and the Relation Service are
			' registered in the same MBean Server.
			Dim expClassName As String = ___roleInfo.refMBeanClassName

			For Each currObjName As javax.management.ObjectName In roleValue

				' Checks it is registered
				If currObjName Is Nothing Then
					RELATION_LOGGER.exiting(GetType(RelationService).name, "checkRoleInt")
					Return New Integer?(RoleStatus.REF_MBEAN_NOT_REGISTERED)
				End If

				' Checks if it is of the correct class
				' Can throw an InstanceNotFoundException, if MBean not registered
				Try
					Dim classSts As Boolean = myMBeanServer.isInstanceOf(currObjName, expClassName)
					If Not classSts Then
						RELATION_LOGGER.exiting(GetType(RelationService).name, "checkRoleInt")
						Return New Integer?(RoleStatus.REF_MBEAN_OF_INCORRECT_CLASS)
					End If

				Catch exc As javax.management.InstanceNotFoundException
					RELATION_LOGGER.exiting(GetType(RelationService).name, "checkRoleInt")
					Return New Integer?(RoleStatus.REF_MBEAN_NOT_REGISTERED)
				End Try
			Next currObjName

			RELATION_LOGGER.exiting(GetType(RelationService).name, "checkRoleInt")
			Return New Integer?(0)
		End Function


		' Initializes roles associated to given role infos to default value (empty
		' ArrayList of ObjectNames) in given relation.
		' It will succeed for every role except if the role info has a minimum
		' cardinality greater than 0. In that case, an InvalidRoleValueException
		' will be raised.
		'
		' -param relationBaseFlag  flag true if the relation is a RelationSupport
		'  object, false if it is an MBean
		' -param relationObj  RelationSupport object (if relation is internal)
		' -param relationObjName  ObjectName of the MBean to be added as a relation
		'  (only for the relation MBean)
		' -param relationId  relation id
		' -param relationTypeName  name of the relation type (has to be created
		'  in the Relation Service)
		' -param roleInfoList  list of role infos for roles to be defaulted
		'
		' -exception IllegalArgumentException  if null paramater
		' -exception RelationServiceNotRegisteredException  if the Relation
		'  Service is not registered in the MBean Server
		' -exception InvalidRoleValueException  if role must have a non-empty
		'  value

		' Revisit [cebro] Handle CIM qualifiers as REQUIRED to detect roles which
		'    should have been initialized by the user
		Private Sub initializeMissingRoles(ByVal relationBaseFlag As Boolean, ByVal relationObj As RelationSupport, ByVal relationObjName As javax.management.ObjectName, ByVal relationId As String, ByVal relationTypeName As String, ByVal roleInfoList As IList(Of RoleInfo))

			If (relationBaseFlag AndAlso (relationObj Is Nothing OrElse relationObjName IsNot Nothing)) OrElse ((Not relationBaseFlag) AndAlso (relationObjName Is Nothing OrElse relationObj IsNot Nothing)) OrElse relationId Is Nothing OrElse relationTypeName Is Nothing OrElse roleInfoList Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "initializeMissingRoles", New Object() {relationBaseFlag, relationObj, relationObjName, relationId, relationTypeName, roleInfoList})

			' Can throw RelationServiceNotRegisteredException
			active

			' For each role info (corresponding to a role not initialized by the
			' role list provided by the user), try to set in the relation a role
			' with an empty list of ObjectNames.
			' A check is performed to verify that the role can be set to an
			' empty value, according to its minimum cardinality
			For Each currRoleInfo As RoleInfo In roleInfoList

				Dim roleName As String = currRoleInfo.name

				' Creates an empty value
				Dim emptyValue As IList(Of javax.management.ObjectName) = New List(Of javax.management.ObjectName)
				' Creates a role
				Dim ___role As New Role(roleName, emptyValue)

				If relationBaseFlag Then

					' Internal relation
					' Can throw InvalidRoleValueException
					'
					' Will not throw RoleNotFoundException (role to be
					' initialized), or RelationNotFoundException, or
					' RelationTypeNotFoundException
					Try
						relationObj.roleIntInt(___role, True, Me, False)

					Catch exc1 As RoleNotFoundException
						Throw New Exception(exc1.Message)
					Catch exc2 As RelationNotFoundException
						Throw New Exception(exc2.Message)
					Catch exc3 As RelationTypeNotFoundException
						Throw New Exception(exc3.Message)
					End Try

				Else

					' Relation is an MBean
					' Use standard setRole()
					Dim params As Object() = New Object(0){}
					params(0) = ___role
					Dim signature As String() = New String(0){}
					signature(0) = "javax.management.relation.Role"
					' Can throw MBeanException wrapping
					' InvalidRoleValueException. Returns the target exception to
					' be homogeneous.
					'
					' Will not throw MBeanException (wrapping
					' RoleNotFoundException or MBeanException) or
					' InstanceNotFoundException, or ReflectionException
					'
					' Again here the assumption is that the Relation Service and
					' the relation MBeans are registered in the same MBean Server.
					Try
						myMBeanServer.attributeute(relationObjName, New javax.management.Attribute("Role", ___role))

					Catch exc1 As javax.management.InstanceNotFoundException
						Throw New Exception(exc1.Message)
					Catch exc3 As javax.management.ReflectionException
						Throw New Exception(exc3.Message)
					Catch exc2 As javax.management.MBeanException
						Dim wrappedExc As Exception = exc2.targetException
						If TypeOf wrappedExc Is InvalidRoleValueException Then
							Throw (CType(wrappedExc, InvalidRoleValueException))
						Else
							Throw New Exception(wrappedExc.Message)
						End If
					Catch exc4 As javax.management.AttributeNotFoundException
					  Throw New Exception(exc4.Message)
					Catch exc5 As javax.management.InvalidAttributeValueException
					  Throw New Exception(exc5.Message)
					End Try
				End If
			Next currRoleInfo

			RELATION_LOGGER.exiting(GetType(RelationService).name, "initializeMissingRoles")
			Return
		End Sub

		' Throws an exception corresponding to a given problem type
		'
		' -param pbType  possible problem, defined in RoleUnresolved
		' -param roleName  role name
		'
		' -exception IllegalArgumentException  if null parameter
		' -exception RoleNotFoundException  for problems:
		'  - NO_ROLE_WITH_NAME
		'  - ROLE_NOT_READABLE
		'  - ROLE_NOT_WRITABLE
		' -exception InvalidRoleValueException  for problems:
		'  - LESS_THAN_MIN_ROLE_DEGREE
		'  - MORE_THAN_MAX_ROLE_DEGREE
		'  - REF_MBEAN_OF_INCORRECT_CLASS
		'  - REF_MBEAN_NOT_REGISTERED
		Friend Shared Sub throwRoleProblemException(ByVal pbType As Integer, ByVal roleName As String)

			If roleName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			' Exception type: 1 = RoleNotFoundException
			'                 2 = InvalidRoleValueException
			Dim excType As Integer = 0

			Dim excMsgPart As String = Nothing

			Select Case pbType
			Case RoleStatus.NO_ROLE_WITH_NAME
				excMsgPart = " does not exist in relation."
				excType = 1
			Case RoleStatus.ROLE_NOT_READABLE
				excMsgPart = " is not readable."
				excType = 1
			Case RoleStatus.ROLE_NOT_WRITABLE
				excMsgPart = " is not writable."
				excType = 1
			Case RoleStatus.LESS_THAN_MIN_ROLE_DEGREE
				excMsgPart = " has a number of MBean references less than the expected minimum degree."
				excType = 2
			Case RoleStatus.MORE_THAN_MAX_ROLE_DEGREE
				excMsgPart = " has a number of MBean references greater than the expected maximum degree."
				excType = 2
			Case RoleStatus.REF_MBEAN_OF_INCORRECT_CLASS
				excMsgPart = " has an MBean reference to an MBean not of the expected class of references for that role."
				excType = 2
			Case RoleStatus.REF_MBEAN_NOT_REGISTERED
				excMsgPart = " has a reference to null or to an MBean not registered."
				excType = 2
			End Select
			' No default as we must have been in one of those cases

			Dim excMsgStrB As New StringBuilder(roleName)
			excMsgStrB.Append(excMsgPart)
			Dim excMsg As String = excMsgStrB.ToString()
			If excType = 1 Then
				Throw New RoleNotFoundException(excMsg)

			ElseIf excType = 2 Then
				Throw New InvalidRoleValueException(excMsg)
			End If
		End Sub

		' Sends a notification of given type, with given parameters
		'
		' -param intNtfType  integer to represent notification type:
		'  - 1 : create
		'  - 2 : update
		'  - 3 : delete
		' -param message  human-readable message
		' -param relationId  relation id of the created/updated/deleted relation
		' -param unregMBeanList  list of ObjectNames of referenced MBeans
		'  expected to be unregistered due to relation removal (only for removal,
		'  due to CIM qualifiers, can be null)
		' -param roleName  role name
		' -param roleNewValue  role new value (ArrayList of ObjectNames)
		' -param oldValue  old role value (ArrayList of ObjectNames)
		'
		' -exception IllegalArgument  if null parameter
		' -exception RelationNotFoundException  if no relation for given id
		Private Sub sendNotificationInt(ByVal intNtfType As Integer, ByVal message As String, ByVal relationId As String, ByVal unregMBeanList As IList(Of javax.management.ObjectName), ByVal roleName As String, ByVal roleNewValue As IList(Of javax.management.ObjectName), ByVal oldValue As IList(Of javax.management.ObjectName))

			If message Is Nothing OrElse relationId Is Nothing OrElse (intNtfType <> 3 AndAlso unregMBeanList IsNot Nothing) OrElse (intNtfType = 2 AndAlso (roleName Is Nothing OrElse roleNewValue Is Nothing OrElse oldValue Is Nothing)) Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "sendNotificationInt", New Object() {intNtfType, message, relationId, unregMBeanList, roleName, roleNewValue, oldValue})

			' Relation type name
			' Note: do not use getRelationTypeName() as if it is a relation MBean
			'       it is already unregistered.
			Dim relTypeName As String
			SyncLock myRelId2RelTypeMap
				relTypeName = (myRelId2RelTypeMap(relationId))
			End SyncLock

			' ObjectName (for a relation MBean)
			' Can also throw a RelationNotFoundException, but detected above
			Dim relObjName As javax.management.ObjectName = isRelationMBean(relationId)

			Dim ntfType As String = Nothing
			If relObjName IsNot Nothing Then
				Select Case intNtfType
				Case 1
					ntfType = RelationNotification.RELATION_MBEAN_CREATION
				Case 2
					ntfType = RelationNotification.RELATION_MBEAN_UPDATE
				Case 3
					ntfType = RelationNotification.RELATION_MBEAN_REMOVAL
				End Select
			Else
				Select Case intNtfType
				Case 1
					ntfType = RelationNotification.RELATION_BASIC_CREATION
				Case 2
					ntfType = RelationNotification.RELATION_BASIC_UPDATE
				Case 3
					ntfType = RelationNotification.RELATION_BASIC_REMOVAL
				End Select
			End If

			' Sequence number
			Dim seqNo As Long? = atomicSeqNo.incrementAndGet()

			' Timestamp
			Dim currDate As DateTime = DateTime.Now
			Dim timeStamp As Long = currDate

			Dim ntf As RelationNotification = Nothing

			If ntfType.Equals(RelationNotification.RELATION_BASIC_CREATION) OrElse ntfType.Equals(RelationNotification.RELATION_MBEAN_CREATION) OrElse ntfType.Equals(RelationNotification.RELATION_BASIC_REMOVAL) OrElse ntfType.Equals(RelationNotification.RELATION_MBEAN_REMOVAL) Then

				' Creation or removal
				ntf = New RelationNotification(ntfType, Me, seqNo, timeStamp, message, relationId, relTypeName, relObjName, unregMBeanList)

			ElseIf ntfType.Equals(RelationNotification.RELATION_BASIC_UPDATE) OrElse ntfType.Equals(RelationNotification.RELATION_MBEAN_UPDATE) Then
					' Update
					ntf = New RelationNotification(ntfType, Me, seqNo, timeStamp, message, relationId, relTypeName, relObjName, roleName, roleNewValue, oldValue)
			End If

			sendNotification(ntf)

			RELATION_LOGGER.exiting(GetType(RelationService).name, "sendNotificationInt")
			Return
		End Sub

		' Checks, for the unregistration of an MBean referenced in the roles given
		' in parameter, if the relation has to be removed or not, regarding
		' expected minimum role cardinality and current number of
		' references in each role after removal of the current one.
		' If the relation is kept, calls handleMBeanUnregistration() callback of
		' the relation to update it.
		'
		' -param relationId  relation id
		' -param objectName  ObjectName of the unregistered MBean
		' -param roleNameList  list of names of roles where the unregistered
		'  MBean is referenced.
		'
		' -exception IllegalArgumentException  if null parameter
		' -exception RelationServiceNotRegisteredException  if the Relation
		'  Service is not registered in the MBean Server
		' -exception RelationNotFoundException  if unknown relation id
		' -exception RoleNotFoundException  if one role given as parameter does
		'  not exist in the relation
		Private Sub handleReferenceUnregistration(ByVal relationId As String, ByVal ___objectName As javax.management.ObjectName, ByVal roleNameList As IList(Of String))

			If relationId Is Nothing OrElse roleNameList Is Nothing OrElse ___objectName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationService).name, "handleReferenceUnregistration", New Object() {relationId, ___objectName, roleNameList})

			' Can throw RelationServiceNotRegisteredException
			active

			' Retrieves the relation type name of the relation
			' Can throw RelationNotFoundException
			Dim currRelTypeName As String = getRelationTypeName(relationId)

			' Retrieves the relation
			' Can throw RelationNotFoundException, but already detected above
			Dim relObj As Object = getRelation(relationId)

			' Flag to specify if the relation has to be deleted
			Dim deleteRelFlag As Boolean = False

			For Each currRoleName As String In roleNameList

				If deleteRelFlag Then Exit For

				' Retrieves number of MBeans currently referenced in role
				' BEWARE! Do not use getRole() as role may be not readable
				'
				' Can throw RelationNotFoundException (but already checked),
				' RoleNotFoundException
				Dim currRoleRefNbr As Integer = (getRoleCardinality(relationId, currRoleName))

				' Retrieves new number of element in role
				Dim currRoleNewRefNbr As Integer = currRoleRefNbr - 1

				' Retrieves role info for that role
				'
				' Shall not throw RelationTypeNotFoundException or
				' RoleInfoNotFoundException
				Dim currRoleInfo As RoleInfo
				Try
					currRoleInfo = getRoleInfo(currRelTypeName, currRoleName)
				Catch exc1 As RelationTypeNotFoundException
					Throw New Exception(exc1.Message)
				Catch exc2 As RoleInfoNotFoundException
					Throw New Exception(exc2.Message)
				End Try

				' Checks with expected minimum number of elements
				Dim chkMinFlag As Boolean = currRoleInfo.checkMinDegree(currRoleNewRefNbr)

				If Not chkMinFlag Then deleteRelFlag = True
			Next currRoleName

			If deleteRelFlag Then
				' Removes the relation
				removeRelation(relationId)

			Else

				' Updates each role in the relation using
				' handleMBeanUnregistration() callback
				'
				' BEWARE: this roleNameList list MUST BE A COPY of a role name
				'         list for a referenced MBean in a relation, NOT a
				'         reference to an original one part of the
				'         myRefedMBeanObjName2RelIdsMap!!!! Because each role
				'         which name is in that list will be updated (potentially
				'         using setRole(). So the Relation Service will update the
				'         myRefedMBeanObjName2RelIdsMap to refelect the new role
				'         value!
				For Each currRoleName As String In roleNameList

					If TypeOf relObj Is RelationSupport Then
						' Internal relation
						' Can throw RoleNotFoundException (but already checked)
						'
						' Shall not throw
						' RelationTypeNotFoundException,
						' InvalidRoleValueException (value was correct, removing
						' one reference shall not invalidate it, else detected
						' above)
						Try
							CType(relObj, RelationSupport).handleMBeanUnregistrationInt(___objectName, currRoleName, True, Me)
						Catch exc3 As RelationTypeNotFoundException
							Throw New Exception(exc3.Message)
						Catch exc4 As InvalidRoleValueException
							Throw New Exception(exc4.Message)
						End Try

					Else
						' Relation MBean
						Dim params As Object() = New Object(1){}
						params(0) = ___objectName
						params(1) = currRoleName
						Dim signature As String() = New String(1){}
						signature(0) = "javax.management.ObjectName"
						signature(1) = "java.lang.String"
						' Shall not throw InstanceNotFoundException, or
						' MBeanException (wrapping RoleNotFoundException or
						' MBeanException or InvalidRoleValueException) or
						' ReflectionException
						Try
							myMBeanServer.invoke((CType(relObj, javax.management.ObjectName)), "handleMBeanUnregistration", params, signature)
						Catch exc1 As javax.management.InstanceNotFoundException
							Throw New Exception(exc1.Message)
						Catch exc3 As javax.management.ReflectionException
							Throw New Exception(exc3.Message)
						Catch exc2 As javax.management.MBeanException
							Dim wrappedExc As Exception = exc2.targetException
							Throw New Exception(wrappedExc.Message)
						End Try

					End If
				Next currRoleName
			End If

			RELATION_LOGGER.exiting(GetType(RelationService).name, "handleReferenceUnregistration")
			Return
		End Sub
	End Class

End Namespace
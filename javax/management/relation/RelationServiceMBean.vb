Imports System.Collections.Generic

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
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface RelationServiceMBean

		''' <summary>
		''' Checks if the Relation Service is active.
		''' Current condition is that the Relation Service must be registered in the
		''' MBean Server
		''' </summary>
		''' <exception cref="RelationServiceNotRegisteredException">  if it is not
		''' registered </exception>
		Sub isActive()

		'
		' Accessors
		'

		''' <summary>
		''' Returns the flag to indicate if when a notification is received for the
		''' unregistration of an MBean referenced in a relation, if an immediate
		''' "purge" of the relations (look for the relations no longer valid)
		''' has to be performed, or if that will be performed only when the
		''' purgeRelations method is explicitly called.
		''' <P>true is immediate purge.
		''' </summary>
		''' <returns> true if purges are immediate.
		''' </returns>
		''' <seealso cref= #setPurgeFlag </seealso>
		Property purgeFlag As Boolean


		'
		' Relation type handling
		'

		''' <summary>
		''' Creates a relation type (RelationTypeSupport object) with given
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
		Sub createRelationType(ByVal relationTypeName As String, ByVal roleInfoArray As RoleInfo())

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
		''' <exception cref="InvalidRelationTypeException">  if there is already a relation
		''' type with that name </exception>
		Sub addRelationType(ByVal relationTypeObj As RelationType)

		''' <summary>
		''' Retrieves names of all known relation types.
		''' </summary>
		''' <returns> ArrayList of relation type names (Strings) </returns>
		ReadOnly Property allRelationTypeNames As IList(Of String)

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
		Function getRoleInfos(ByVal relationTypeName As String) As IList(Of RoleInfo)

		''' <summary>
		''' Retrieves role info for given role of a given relation type.
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
		Function getRoleInfo(ByVal relationTypeName As String, ByVal roleInfoName As String) As RoleInfo

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
		Sub removeRelationType(ByVal relationTypeName As String)

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
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
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
		Sub createRelation(ByVal relationId As String, ByVal relationTypeName As String, ByVal roleList As RoleList)

		''' <summary>
		''' Adds an MBean created by the user (and registered by him in the MBean
		''' Server) as a relation in the Relation Service.
		''' <P>To be added as a relation, the MBean must conform to the
		''' following:
		''' <P>- implement the Relation interface
		''' <P>- have for RelationService ObjectName the ObjectName of current
		''' Relation Service
		''' <P>- have a relation id that is unique and unused in current Relation Service
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
		Sub addRelation(ByVal relationObjectName As javax.management.ObjectName)

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
		Function isRelationMBean(ByVal relationId As String) As javax.management.ObjectName

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
		Function isRelation(ByVal objectName As javax.management.ObjectName) As String

		''' <summary>
		''' Checks if there is a relation identified in Relation Service with given
		''' relation id.
		''' </summary>
		''' <param name="relationId">  relation id identifying the relation
		''' </param>
		''' <returns> boolean: true if there is a relation, false else
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		Function hasRelation(ByVal relationId As String) As Boolean?

		''' <summary>
		''' Returns all the relation ids for all the relations handled by the
		''' Relation Service.
		''' </summary>
		''' <returns> ArrayList of String </returns>
		ReadOnly Property allRelationIds As IList(Of String)

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
		Function checkRoleReading(ByVal roleName As String, ByVal relationTypeName As String) As Integer?

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
		Function checkRoleWriting(ByVal role As Role, ByVal relationTypeName As String, ByVal initFlag As Boolean?) As Integer?

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
		Sub sendRelationCreationNotification(ByVal relationId As String)

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
		''' <param name="oldRoleValue">  old role value (List of ObjectName objects)
		''' </param>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationNotFoundException">  if there is no relation for given
		''' relation id </exception>
		Sub sendRoleUpdateNotification(ByVal relationId As String, ByVal newRole As Role, ByVal oldRoleValue As IList(Of javax.management.ObjectName))

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
		Sub sendRelationRemovalNotification(ByVal relationId As String, ByVal unregMBeanList As IList(Of javax.management.ObjectName))

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
		''' <param name="oldRoleValue">  old role value (List of ObjectName objects)
		''' </param>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationServiceNotRegisteredException">  if the Relation
		''' Service is not registered in the MBean Server </exception>
		''' <exception cref="RelationNotFoundException">  if no relation for given id. </exception>
		Sub updateRoleMap(ByVal relationId As String, ByVal newRole As Role, ByVal oldRoleValue As IList(Of javax.management.ObjectName))

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
		Sub removeRelation(ByVal relationId As String)

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
		Sub purgeRelations()

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
		Function findReferencingRelations(ByVal mbeanName As javax.management.ObjectName, ByVal relationTypeName As String, ByVal roleName As String) As IDictionary(Of String, IList(Of String))

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
		Function findAssociatedMBeans(ByVal mbeanName As javax.management.ObjectName, ByVal relationTypeName As String, ByVal roleName As String) As IDictionary(Of javax.management.ObjectName, IList(Of String))

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
		Function findRelationsOfType(ByVal relationTypeName As String) As IList(Of String)

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
		Function getRole(ByVal relationId As String, ByVal roleName As String) As IList(Of javax.management.ObjectName)

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
		Function getRoles(ByVal relationId As String, ByVal roleNameArray As String()) As RoleResult

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
		Function getAllRoles(ByVal relationId As String) As RoleResult

		''' <summary>
		''' Retrieves the number of MBeans currently referenced in the
		''' given role.
		''' </summary>
		''' <param name="relationId">  relation id </param>
		''' <param name="roleName">  name of role
		''' </param>
		''' <returns> the number of currently referenced MBeans in that role
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationNotFoundException">  if no relation with given id </exception>
		''' <exception cref="RoleNotFoundException">  if there is no role with given name </exception>
		Function getRoleCardinality(ByVal relationId As String, ByVal roleName As String) As Integer?

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
		''' <exception cref="RoleNotFoundException">  if:
		''' <P>- internal relation
		''' <P>and
		''' <P>- the role does not exist or is not writable </exception>
		''' <exception cref="InvalidRoleValueException">  if internal relation and value
		''' provided for role is not valid:
		''' <P>- the number of referenced MBeans in given value is less than
		''' expected minimum degree
		''' <P>or
		''' <P>- the number of referenced MBeans in provided value exceeds expected
		''' maximum degree
		''' <P>or
		''' <P>- one referenced MBean in the value is not an Object of the MBean
		''' class expected for that role
		''' <P>or
		''' <P>- an MBean provided for that role does not exist </exception>
		''' <exception cref="RelationTypeNotFoundException">  if unknown relation type
		''' </exception>
		''' <seealso cref= #getRole </seealso>
		Sub setRole(ByVal relationId As String, ByVal role As Role)

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
		Function setRoles(ByVal relationId As String, ByVal roleList As RoleList) As RoleResult

		''' <summary>
		''' Retrieves MBeans referenced in the various roles of the relation.
		''' </summary>
		''' <param name="relationId">  relation id
		''' </param>
		''' <returns> a HashMap mapping:
		''' <P> ObjectName {@literal ->} ArrayList of String (role
		''' names)
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RelationNotFoundException">  if no relation for given
		''' relation id </exception>
		Function getReferencedMBeans(ByVal relationId As String) As IDictionary(Of javax.management.ObjectName, IList(Of String))

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
		Function getRelationTypeName(ByVal relationId As String) As String
	End Interface

End Namespace
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
	''' A RelationSupport object is used internally by the Relation Service to
	''' represent simple relations (only roles, no properties or methods), with an
	''' unlimited number of roles, of any relation type. As internal representation,
	''' it is not exposed to the user.
	''' <P>RelationSupport class conforms to the design patterns of standard MBean. So
	''' the user can decide to instantiate a RelationSupport object himself as
	''' a MBean (as it follows the MBean design patterns), to register it in the
	''' MBean Server, and then to add it in the Relation Service.
	''' <P>The user can also, when creating his own MBean relation class, have it
	''' extending RelationSupport, to retrieve the implementations of required
	''' interfaces (see below).
	''' <P>It is also possible to have in a user relation MBean class a member
	''' being a RelationSupport object, and to implement the required interfaces by
	''' delegating all to this member.
	''' <P> RelationSupport implements the Relation interface (to be handled by the
	''' Relation Service).
	''' <P>It implements also the MBeanRegistration interface to be able to retrieve
	''' the MBean Server where it is registered (if registered as a MBean) to access
	''' to its Relation Service.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class RelationSupport
		Implements RelationSupportMBean, javax.management.MBeanRegistration

		'
		' Private members
		'

		' Relation identifier (expected to be unique in the Relation Service where
		' the RelationSupport object will be added)
		Private myRelId As String = Nothing

		' ObjectName of the Relation Service where the relation will be added
		' REQUIRED if the RelationSupport is created by the user to be registered as
		' a MBean, as it will have to access the Relation Service via the MBean
		' Server to perform the check regarding the relation type.
		' Is null if current object is directly created by the Relation Service,
		' as the object will directly access it.
		Private myRelServiceName As javax.management.ObjectName = Nothing

		' Reference to the MBean Server where the Relation Service is
		' registered
		' REQUIRED if the RelationSupport is created by the user to be registered as
		' a MBean, as it will have to access the Relation Service via the MBean
		' Server to perform the check regarding the relation type.
		' If the Relationbase object is created by the Relation Service (use of
		' createRelation() method), this is null as not needed, direct access to
		' the Relation Service.
		' If the Relationbase object is created by the user and registered as a
		' MBean, this is set by the preRegister() method below.
		Private myRelServiceMBeanServer As javax.management.MBeanServer = Nothing

		' Relation type name (must be known in the Relation Service where the
		' relation will be added)
		Private myRelTypeName As String = Nothing

		' Role map, mapping <role-name> -> <Role>
		' Initialized by role list in the constructor, then updated:
		' - if the relation is a MBean, via setRole() and setRoles() methods, or
		'   via Relation Service setRole() and setRoles() methods
		' - if the relation is internal to the Relation Service, via
		'   setRoleInt() and setRolesInt() methods.
		Private ReadOnly myRoleName2ValueMap As IDictionary(Of String, Role) = New Dictionary(Of String, Role)

		' Flag to indicate if the object has been added in the Relation Service
		Private ReadOnly myInRelServFlg As New java.util.concurrent.atomic.AtomicBoolean

		'
		' Constructors
		'

		''' <summary>
		''' Creates a {@code RelationSupport} object.
		''' <P>This constructor has to be used when the RelationSupport object will
		''' be registered as a MBean by the user, or when creating a user relation
		''' MBean whose class extends RelationSupport.
		''' <P>Nothing is done at the Relation Service level, i.e.
		''' the {@code RelationSupport} object is not added to the
		''' {@code RelationService} and no checks are performed to
		''' see if the provided values are correct.
		''' The object is always created, EXCEPT if:
		''' <P>- any of the required parameters is {@code null}.
		''' <P>- the same name is used for two roles.
		''' <P>To be handled as a relation, the {@code RelationSupport} object has
		''' to be added to the Relation Service using the Relation Service method
		''' addRelation().
		''' </summary>
		''' <param name="relationId">  relation identifier, to identify the relation in the
		''' Relation Service.
		''' <P>Expected to be unique in the given Relation Service. </param>
		''' <param name="relationServiceName">  ObjectName of the Relation Service where
		''' the relation will be registered.
		''' <P>This parameter is required as it is the Relation Service that is
		''' aware of the definition of the relation type of the given relation,
		''' so that will be able to check update operations (set). </param>
		''' <param name="relationTypeName">  Name of relation type.
		''' <P>Expected to have been created in the given Relation Service. </param>
		''' <param name="list">  list of roles (Role objects) to initialize the
		''' relation. Can be {@code null}.
		''' <P>Expected to conform to relation info in associated relation type.
		''' </param>
		''' <exception cref="InvalidRoleValueException">  if the same name is used for two
		''' roles. </exception>
		''' <exception cref="IllegalArgumentException">  if any of the required parameters
		''' (relation id, relation service ObjectName, or relation type name) is
		''' {@code null}. </exception>
		Public Sub New(ByVal relationId As String, ByVal relationServiceName As javax.management.ObjectName, ByVal relationTypeName As String, ByVal list As RoleList)

			MyBase.New()

			RELATION_LOGGER.entering(GetType(RelationSupport).name, "RelationSupport")

			' Can throw InvalidRoleValueException and IllegalArgumentException
			initMembers(relationId, relationServiceName, Nothing, relationTypeName, list)

			RELATION_LOGGER.exiting(GetType(RelationSupport).name, "RelationSupport")
		End Sub

		''' <summary>
		''' Creates a {@code RelationSupport} object.
		''' <P>This constructor has to be used when the user relation MBean
		''' implements the interfaces expected to be supported by a relation by
		''' delegating to a RelationSupport object.
		''' <P>This object needs to know the Relation Service expected to handle the
		''' relation. So it has to know the MBean Server where the Relation Service
		''' is registered.
		''' <P>According to a limitation, a relation MBean must be registered in the
		''' same MBean Server as the Relation Service expected to handle it. So the
		''' user relation MBean has to be created and registered, and then the
		''' wrapped RelationSupport object can be created within the identified MBean
		''' Server.
		''' <P>Nothing is done at the Relation Service level, i.e.
		''' the {@code RelationSupport} object is not added to the
		''' {@code RelationService} and no checks are performed to
		''' see if the provided values are correct.
		''' The object is always created, EXCEPT if:
		''' <P>- any of the required parameters is {@code null}.
		''' <P>- the same name is used for two roles.
		''' <P>To be handled as a relation, the {@code RelationSupport} object has
		''' to be added to the Relation Service using the Relation Service method
		''' addRelation().
		''' </summary>
		''' <param name="relationId">  relation identifier, to identify the relation in the
		''' Relation Service.
		''' <P>Expected to be unique in the given Relation Service. </param>
		''' <param name="relationServiceName">  ObjectName of the Relation Service where
		''' the relation will be registered.
		''' <P>This parameter is required as it is the Relation Service that is
		''' aware of the definition of the relation type of the given relation,
		''' so that will be able to check update operations (set). </param>
		''' <param name="relationServiceMBeanServer">  MBean Server where the wrapping MBean
		''' is or will be registered.
		''' <P>Expected to be the MBean Server where the Relation Service is or will
		''' be registered. </param>
		''' <param name="relationTypeName">  Name of relation type.
		''' <P>Expected to have been created in the given Relation Service. </param>
		''' <param name="list">  list of roles (Role objects) to initialize the
		''' relation. Can be {@code null}.
		''' <P>Expected to conform to relation info in associated relation type.
		''' </param>
		''' <exception cref="InvalidRoleValueException">  if the same name is used for two
		''' roles. </exception>
		''' <exception cref="IllegalArgumentException">  if any of the required parameters
		''' (relation id, relation service ObjectName, relation service MBeanServer,
		''' or relation type name) is {@code null}. </exception>
		Public Sub New(ByVal relationId As String, ByVal relationServiceName As javax.management.ObjectName, ByVal relationServiceMBeanServer As javax.management.MBeanServer, ByVal relationTypeName As String, ByVal list As RoleList)

			MyBase.New()

			If relationServiceMBeanServer Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationSupport).name, "RelationSupport")

			' Can throw InvalidRoleValueException and
			' IllegalArgumentException
			initMembers(relationId, relationServiceName, relationServiceMBeanServer, relationTypeName, list)

			RELATION_LOGGER.exiting(GetType(RelationSupport).name, "RelationSupport")
		End Sub

		'
		' Relation Interface
		'

		''' <summary>
		''' Retrieves role value for given role name.
		''' <P>Checks if the role exists and is readable according to the relation
		''' type.
		''' </summary>
		''' <param name="roleName">  name of role
		''' </param>
		''' <returns> the ArrayList of ObjectName objects being the role value
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null role name </exception>
		''' <exception cref="RoleNotFoundException">  if:
		''' <P>- there is no role with given name
		''' <P>- the role is not readable. </exception>
		''' <exception cref="RelationServiceNotRegisteredException">  if the Relation
		''' Service is not registered in the MBean Server
		''' </exception>
		''' <seealso cref= #setRole </seealso>
		Public Overridable Function getRole(ByVal roleName As String) As IList(Of javax.management.ObjectName) Implements Relation.getRole

			If roleName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationSupport).name, "getRole", roleName)

			' Can throw RoleNotFoundException and
			' RelationServiceNotRegisteredException
			Dim result As IList(Of javax.management.ObjectName) = cast(getRoleInt(roleName, False, Nothing, False))

			RELATION_LOGGER.exiting(GetType(RelationSupport).name, "getRole")
			Return result
		End Function

		''' <summary>
		''' Retrieves values of roles with given names.
		''' <P>Checks for each role if it exists and is readable according to the
		''' relation type.
		''' </summary>
		''' <param name="roleNameArray">  array of names of roles to be retrieved
		''' </param>
		''' <returns> a RoleResult object, including a RoleList (for roles
		''' successfully retrieved) and a RoleUnresolvedList (for roles not
		''' retrieved).
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null role name </exception>
		''' <exception cref="RelationServiceNotRegisteredException">  if the Relation
		''' Service is not registered in the MBean Server
		''' </exception>
		''' <seealso cref= #setRoles </seealso>
		Public Overridable Function getRoles(ByVal roleNameArray As String()) As RoleResult Implements Relation.getRoles

			If roleNameArray Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationSupport).name, "getRoles")

			' Can throw RelationServiceNotRegisteredException
			Dim result As RoleResult = getRolesInt(roleNameArray, False, Nothing)

			RELATION_LOGGER.exiting(GetType(RelationSupport).name, "getRoles")
			Return result
		End Function

		''' <summary>
		''' Returns all roles present in the relation.
		''' </summary>
		''' <returns> a RoleResult object, including a RoleList (for roles
		''' successfully retrieved) and a RoleUnresolvedList (for roles not
		''' readable).
		''' </returns>
		''' <exception cref="RelationServiceNotRegisteredException">  if the Relation
		''' Service is not registered in the MBean Server </exception>
		Public Overridable Property allRoles As RoleResult Implements Relation.getAllRoles
			Get
    
				RELATION_LOGGER.entering(GetType(RelationSupport).name, "getAllRoles")
    
				Dim result As RoleResult = Nothing
				Try
					result = getAllRolesInt(False, Nothing)
				Catch exc As System.ArgumentException
					' OK : Invalid parameters, ignore...
				End Try
    
				RELATION_LOGGER.exiting(GetType(RelationSupport).name, "getAllRoles")
				Return result
			End Get
		End Property

		''' <summary>
		''' Returns all roles in the relation without checking read mode.
		''' </summary>
		''' <returns> a RoleList </returns>
		Public Overridable Function retrieveAllRoles() As RoleList Implements Relation.retrieveAllRoles

			RELATION_LOGGER.entering(GetType(RelationSupport).name, "retrieveAllRoles")

			Dim result As RoleList
			SyncLock myRoleName2ValueMap
				result = New RoleList(New List(Of Role)(myRoleName2ValueMap.Values))
			End SyncLock

			RELATION_LOGGER.exiting(GetType(RelationSupport).name, "retrieveAllRoles")
			Return result
		End Function

		''' <summary>
		''' Returns the number of MBeans currently referenced in the given role.
		''' </summary>
		''' <param name="roleName">  name of role
		''' </param>
		''' <returns> the number of currently referenced MBeans in that role
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null role name </exception>
		''' <exception cref="RoleNotFoundException">  if there is no role with given name </exception>
		Public Overridable Function getRoleCardinality(ByVal roleName As String) As Integer?

			If roleName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationSupport).name, "getRoleCardinality", roleName)

			' Try to retrieve the role
			Dim ___role As Role
			SyncLock myRoleName2ValueMap
				' No null Role is allowed, so direct use of get()
				___role = (myRoleName2ValueMap(roleName))
			End SyncLock
			If ___role Is Nothing Then
				Dim pbType As Integer = RoleStatus.NO_ROLE_WITH_NAME
				' Will throw a RoleNotFoundException
				'
				' Will not throw InvalidRoleValueException, so catch it for the
				' compiler
				Try
					RelationService.throwRoleProblemException(pbType, roleName)
				Catch exc As InvalidRoleValueException
					' OK : Do not throw InvalidRoleValueException as
					'      a RoleNotFoundException will be thrown.
				End Try
			End If

			Dim roleValue As IList(Of javax.management.ObjectName) = ___role.roleValue

			RELATION_LOGGER.exiting(GetType(RelationSupport).name, "getRoleCardinality")
			Return roleValue.Count
		End Function

		''' <summary>
		''' Sets the given role.
		''' <P>Will check the role according to its corresponding role definition
		''' provided in relation's relation type
		''' <P>Will send a notification (RelationNotification with type
		''' RELATION_BASIC_UPDATE or RELATION_MBEAN_UPDATE, depending if the
		''' relation is a MBean or not).
		''' </summary>
		''' <param name="role">  role to be set (name and new value)
		''' </param>
		''' <exception cref="IllegalArgumentException">  if null role </exception>
		''' <exception cref="RoleNotFoundException">  if there is no role with the supplied
		''' role's name or if the role is not writable (no test on the write access
		''' mode performed when initializing the role) </exception>
		''' <exception cref="InvalidRoleValueException">  if value provided for
		''' role is not valid, i.e.:
		''' <P>- the number of referenced MBeans in given value is less than
		''' expected minimum degree
		''' <P>- the number of referenced MBeans in provided value exceeds expected
		''' maximum degree
		''' <P>- one referenced MBean in the value is not an Object of the MBean
		''' class expected for that role
		''' <P>- a MBean provided for that role does not exist </exception>
		''' <exception cref="RelationServiceNotRegisteredException">  if the Relation
		''' Service is not registered in the MBean Server </exception>
		''' <exception cref="RelationTypeNotFoundException">  if the relation type has not
		''' been declared in the Relation Service </exception>
		''' <exception cref="RelationNotFoundException">  if the relation has not been
		''' added in the Relation Service.
		''' </exception>
		''' <seealso cref= #getRole </seealso>
		Public Overridable Property role Implements Relation.setRole As Role
			Set(ByVal ___role As Role)
    
				If ___role Is Nothing Then
					Dim excMsg As String = "Invalid parameter."
					Throw New System.ArgumentException(excMsg)
				End If
    
				RELATION_LOGGER.entering(GetType(RelationSupport).name, "setRole", ___role)
    
				' Will return null :)
				Dim result As Object = roleIntInt(___role, False, Nothing, False)
    
				RELATION_LOGGER.exiting(GetType(RelationSupport).name, "setRole")
				Return
			End Set
		End Property

		''' <summary>
		''' Sets the given roles.
		''' <P>Will check the role according to its corresponding role definition
		''' provided in relation's relation type
		''' <P>Will send one notification (RelationNotification with type
		''' RELATION_BASIC_UPDATE or RELATION_MBEAN_UPDATE, depending if the
		''' relation is a MBean or not) per updated role.
		''' </summary>
		''' <param name="list">  list of roles to be set
		''' </param>
		''' <returns> a RoleResult object, including a RoleList (for roles
		''' successfully set) and a RoleUnresolvedList (for roles not
		''' set).
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null role list </exception>
		''' <exception cref="RelationServiceNotRegisteredException">  if the Relation
		''' Service is not registered in the MBean Server </exception>
		''' <exception cref="RelationTypeNotFoundException">  if the relation type has not
		''' been declared in the Relation Service. </exception>
		''' <exception cref="RelationNotFoundException">  if the relation MBean has not been
		''' added in the Relation Service.
		''' </exception>
		''' <seealso cref= #getRoles </seealso>
		Public Overridable Function setRoles(ByVal list As RoleList) As RoleResult Implements Relation.setRoles

			If list Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationSupport).name, "setRoles", list)

			Dim result As RoleResult = rolesIntInt(list, False, Nothing)

			RELATION_LOGGER.exiting(GetType(RelationSupport).name, "setRoles")
			Return result
		End Function

		''' <summary>
		''' Callback used by the Relation Service when a MBean referenced in a role
		''' is unregistered.
		''' <P>The Relation Service will call this method to let the relation
		''' take action to reflect the impact of such unregistration.
		''' <P>BEWARE. the user is not expected to call this method.
		''' <P>Current implementation is to set the role with its current value
		''' (list of ObjectNames of referenced MBeans) without the unregistered
		''' one.
		''' </summary>
		''' <param name="objectName">  ObjectName of unregistered MBean </param>
		''' <param name="roleName">  name of role where the MBean is referenced
		''' </param>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="RoleNotFoundException">  if role does not exist in the
		''' relation or is not writable </exception>
		''' <exception cref="InvalidRoleValueException">  if role value does not conform to
		''' the associated role info (this will never happen when called from the
		''' Relation Service) </exception>
		''' <exception cref="RelationServiceNotRegisteredException">  if the Relation
		''' Service is not registered in the MBean Server </exception>
		''' <exception cref="RelationTypeNotFoundException">  if the relation type has not
		''' been declared in the Relation Service. </exception>
		''' <exception cref="RelationNotFoundException">  if this method is called for a
		''' relation MBean not added in the Relation Service. </exception>
		Public Overridable Sub handleMBeanUnregistration(ByVal ___objectName As javax.management.ObjectName, ByVal roleName As String) Implements Relation.handleMBeanUnregistration

			If ___objectName Is Nothing OrElse roleName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationSupport).name, "handleMBeanUnregistration", New Object(){___objectName, roleName})

			' Can throw RoleNotFoundException, InvalidRoleValueException,
			' or RelationTypeNotFoundException
			handleMBeanUnregistrationInt(___objectName, roleName, False, Nothing)

			RELATION_LOGGER.exiting(GetType(RelationSupport).name, "handleMBeanUnregistration")
			Return
		End Sub

		''' <summary>
		''' Retrieves MBeans referenced in the various roles of the relation.
		''' </summary>
		''' <returns> a HashMap mapping:
		''' <P> ObjectName {@literal ->} ArrayList of String (role names) </returns>
		Public Overridable Property referencedMBeans As IDictionary(Of javax.management.ObjectName, IList(Of String)) Implements Relation.getReferencedMBeans
			Get
    
				RELATION_LOGGER.entering(GetType(RelationSupport).name, "getReferencedMBeans")
    
				Dim refMBeanMap As IDictionary(Of javax.management.ObjectName, IList(Of String)) = New Dictionary(Of javax.management.ObjectName, IList(Of String))
    
				SyncLock myRoleName2ValueMap
    
					For Each currRole As Role In myRoleName2ValueMap.Values
    
						Dim currRoleName As String = currRole.roleName
						' Retrieves ObjectNames of MBeans referenced in current role
						Dim currRefMBeanList As IList(Of javax.management.ObjectName) = currRole.roleValue
    
						For Each currRoleObjName As javax.management.ObjectName In currRefMBeanList
    
							' Sees if current MBean has been already referenced in
							' roles already seen
							Dim mbeanRoleNameList As IList(Of String) = refMBeanMap(currRoleObjName)
    
							Dim newRefFlg As Boolean = False
							If mbeanRoleNameList Is Nothing Then
								newRefFlg = True
								mbeanRoleNameList = New List(Of String)
							End If
							mbeanRoleNameList.Add(currRoleName)
							If newRefFlg Then refMBeanMap(currRoleObjName) = mbeanRoleNameList
						Next currRoleObjName
					Next currRole
				End SyncLock
    
				RELATION_LOGGER.exiting(GetType(RelationSupport).name, "getReferencedMBeans")
				Return refMBeanMap
			End Get
		End Property

		''' <summary>
		''' Returns name of associated relation type.
		''' </summary>
		Public Overridable Property relationTypeName As String Implements Relation.getRelationTypeName
			Get
				Return myRelTypeName
			End Get
		End Property

		''' <summary>
		''' Returns ObjectName of the Relation Service handling the relation.
		''' </summary>
		''' <returns> the ObjectName of the Relation Service. </returns>
		Public Overridable Property relationServiceName As javax.management.ObjectName Implements Relation.getRelationServiceName
			Get
				Return myRelServiceName
			End Get
		End Property

		''' <summary>
		''' Returns relation identifier (used to uniquely identify the relation
		''' inside the Relation Service).
		''' </summary>
		''' <returns> the relation id. </returns>
		Public Overridable Property relationId As String Implements Relation.getRelationId
			Get
				Return myRelId
			End Get
		End Property

		'
		' MBeanRegistration interface
		'

		' Pre-registration: retrieves the MBean Server (useful to access to the
		' Relation Service)
		' This is the way to retrieve the MBean Server when the relation object is
		' a MBean created by the user outside of the Relation Service.
		'
		' No exception thrown.
		Public Overridable Function preRegister(ByVal server As javax.management.MBeanServer, ByVal name As javax.management.ObjectName) As javax.management.ObjectName

			myRelServiceMBeanServer = server
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
		' Others
		'

		''' <summary>
		''' Returns an internal flag specifying if the object is still handled by
		''' the Relation Service.
		''' </summary>
		Public Overridable Property inRelationService As Boolean?
			Get
				Return myInRelServFlg.get()
			End Get
		End Property

		Public Overridable Property relationServiceManagementFlag Implements RelationSupportMBean.setRelationServiceManagementFlag As Boolean?
			Set(ByVal flag As Boolean?)
    
				If flag Is Nothing Then
					Dim excMsg As String = "Invalid parameter."
					Throw New System.ArgumentException(excMsg)
				End If
				myInRelServFlg.set(flag)
			End Set
		End Property

		'
		' Misc
		'

		' Gets the role with given name
		' Checks if the role exists and is readable according to the relation
		' type.
		'
		' This method is called in getRole() above.
		' It is also called in the Relation Service getRole() method.
		' It is also called in getRolesInt() below (used for getRoles() above
		' and for Relation Service getRoles() method).
		'
		' Depending on parameters reflecting its use (either in the scope of
		' getting a single role or of getting several roles), will return:
		' - in case of success:
		'   - for single role retrieval, the ArrayList of ObjectNames being the
		'     role value
		'   - for multi-role retrieval, the Role object itself
		' - in case of failure (except critical exceptions):
		'   - for single role retrieval, if role does not exist or is not
		'     readable, an RoleNotFoundException exception is raised
		'   - for multi-role retrieval, a RoleUnresolved object
		'
		' -param roleName  name of role to be retrieved
		' -param relationServCallFlg  true if call from the Relation Service; this
		'  will happen if the current RelationSupport object has been created by
		'  the Relation Service (via createRelation()) method, so direct access is
		'  possible.
		' -param relationServ  reference to Relation Service object, if object
		'  created by Relation Service.
		' -param multiRoleFlg  true if getting the role in the scope of a
		'  multiple retrieval.
		'
		' -return:
		'  - for single role retrieval (multiRoleFlg false):
		'    - ArrayList of ObjectName objects, value of role with given name, if
		'      the role can be retrieved
		'    - raise a RoleNotFoundException exception else
		'  - for multi-role retrieval (multiRoleFlg true):
		'    - the Role object for given role name if role can be retrieved
		'    - a RoleUnresolved object with problem.
		'
		' -exception IllegalArgumentException  if null parameter
		' -exception RoleNotFoundException  if multiRoleFlg is false and:
		'  - there is no role with given name
		'  or
		'  - the role is not readable.
		' -exception RelationServiceNotRegisteredException  if the Relation
		'  Service is not registered in the MBean Server
		Friend Overridable Function getRoleInt(ByVal roleName As String, ByVal relationServCallFlg As Boolean, ByVal relationServ As RelationService, ByVal multiRoleFlg As Boolean) As Object

			If roleName Is Nothing OrElse (relationServCallFlg AndAlso relationServ Is Nothing) Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationSupport).name, "getRoleInt", roleName)

			Dim pbType As Integer = 0

			Dim ___role As Role
			SyncLock myRoleName2ValueMap
				' No null Role is allowed, so direct use of get()
				___role = (myRoleName2ValueMap(roleName))
			End SyncLock

			If ___role Is Nothing Then
					pbType = RoleStatus.NO_ROLE_WITH_NAME

			Else
				' Checks if the role is readable
				Dim status As Integer?

				If relationServCallFlg Then

					' Call from the Relation Service, so direct access to it,
					' avoiding MBean Server
					' Shall not throw a RelationTypeNotFoundException
					Try
						status = relationServ.checkRoleReading(roleName, myRelTypeName)
					Catch exc As RelationTypeNotFoundException
						Throw New Exception(exc.Message)
					End Try

				Else

					' Call from getRole() method above
					' So we have a MBean. We must access the Relation Service
					' via the MBean Server.
					Dim params As Object() = New Object(1){}
					params(0) = roleName
					params(1) = myRelTypeName
					Dim signature As String() = New String(1){}
					signature(0) = "java.lang.String"
					signature(1) = "java.lang.String"
					' Can throw InstanceNotFoundException if the Relation
					' Service is not registered (to be catched in any case and
					' transformed into RelationServiceNotRegisteredException).
					'
					' Shall not throw a MBeanException, or a ReflectionException
					' or an InstanceNotFoundException
					Try
						status = CInt(Fix(myRelServiceMBeanServer.invoke(myRelServiceName, "checkRoleReading", params, signature)))
					Catch exc1 As javax.management.MBeanException
						Throw New Exception("incorrect relation type")
					Catch exc2 As javax.management.ReflectionException
						Throw New Exception(exc2.Message)
					Catch exc3 As javax.management.InstanceNotFoundException
						Throw New RelationServiceNotRegisteredException(exc3.Message)
					End Try
				End If

				pbType = status
			End If

			Dim result As Object

			If pbType = 0 Then
				' Role can be retrieved

				If Not(multiRoleFlg) Then
					' Single role retrieved: returns its value
					' Note: no need to test if role value (list) not null before
					'       cloning, null value not allowed, empty list if
					'       nothing.
					result = New List(Of javax.management.ObjectName)(___role.roleValue)

				Else
					' Role retrieved during multi-role retrieval: returns the
					' role
					result = CType(___role.clone(), Role)
				End If

			Else
				' Role not retrieved

				If Not(multiRoleFlg) Then
					' Problem when retrieving a simple role: either role not
					' found or not readable, so raises a RoleNotFoundException.
					Try
						RelationService.throwRoleProblemException(pbType, roleName)
						' To keep compiler happy :)
						Return Nothing
					Catch exc As InvalidRoleValueException
						Throw New Exception(exc.Message)
					End Try

				Else
					' Problem when retrieving a role in a multi-role retrieval:
					' returns a RoleUnresolved object
					result = New RoleUnresolved(roleName, Nothing, pbType)
				End If
			End If

			RELATION_LOGGER.exiting(GetType(RelationSupport).name, "getRoleInt")
			Return result
		End Function

		' Gets the given roles
		' For each role, verifies if the role exists and is readable according to
		' the relation type.
		'
		' This method is called in getRoles() above and in Relation Service
		' getRoles() method.
		'
		' -param roleNameArray  array of names of roles to be retrieved
		' -param relationServCallFlg  true if call from the Relation Service; this
		'  will happen if the current RelationSupport object has been created by
		'  the Relation Service (via createRelation()) method, so direct access is
		'  possible.
		' -param relationServ  reference to Relation Service object, if object
		'  created by Relation Service.
		'
		' -return a RoleResult object
		'
		' -exception IllegalArgumentException  if null parameter
		' -exception RelationServiceNotRegisteredException  if the Relation
		'  Service is not registered in the MBean Server
		Friend Overridable Function getRolesInt(ByVal roleNameArray As String(), ByVal relationServCallFlg As Boolean, ByVal relationServ As RelationService) As RoleResult

			If roleNameArray Is Nothing OrElse (relationServCallFlg AndAlso relationServ Is Nothing) Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationSupport).name, "getRolesInt")

			Dim roleList As New RoleList
			Dim roleUnresList As New RoleUnresolvedList

			For i As Integer = 0 To roleNameArray.Length - 1
				Dim currRoleName As String = roleNameArray(i)

				Dim currResult As Object

				' Can throw RelationServiceNotRegisteredException
				'
				' RoleNotFoundException: not possible but catch it for compiler :)
				Try
					currResult = getRoleInt(currRoleName, relationServCallFlg, relationServ, True)

				Catch exc As RoleNotFoundException
					Return Nothing ' :)
				End Try

				If TypeOf currResult Is Role Then
					' Can throw IllegalArgumentException if role is null
					' (normally should not happen :(
					Try
						roleList.Add(CType(currResult, Role))
					Catch exc As System.ArgumentException
						Throw New Exception(exc.Message)
					End Try

				ElseIf TypeOf currResult Is RoleUnresolved Then
					' Can throw IllegalArgumentException if role is null
					' (normally should not happen :(
					Try
						roleUnresList.Add(CType(currResult, RoleUnresolved))
					Catch exc As System.ArgumentException
						Throw New Exception(exc.Message)
					End Try
				End If
			Next i

			Dim result As New RoleResult(roleList, roleUnresList)
			RELATION_LOGGER.exiting(GetType(RelationSupport).name, "getRolesInt")
			Return result
		End Function

		' Returns all roles present in the relation
		'
		' -return a RoleResult object, including a RoleList (for roles
		'  successfully retrieved) and a RoleUnresolvedList (for roles not
		'  readable).
		'
		' -exception IllegalArgumentException if null parameter
		' -exception RelationServiceNotRegisteredException  if the Relation
		'  Service is not registered in the MBean Server
		'
		Friend Overridable Function getAllRolesInt(ByVal relationServCallFlg As Boolean, ByVal relationServ As RelationService) As RoleResult

			If relationServCallFlg AndAlso relationServ Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationSupport).name, "getAllRolesInt")

			Dim roleNameList As IList(Of String)
			SyncLock myRoleName2ValueMap
				roleNameList = New List(Of String)(myRoleName2ValueMap.Keys)
			End SyncLock
			Dim roleNames As String() = New String(roleNameList.Count - 1){}
			roleNameList.ToArray(roleNames)

			Dim result As RoleResult = getRolesInt(roleNames, relationServCallFlg, relationServ)

			RELATION_LOGGER.exiting(GetType(RelationSupport).name, "getAllRolesInt")
			Return result
		End Function

		' Sets the role with given value
		'
		' This method is called in setRole() above.
		' It is also called by the Relation Service setRole() method.
		' It is also called in setRolesInt() method below (used in setRoles()
		' above and in RelationService setRoles() method).
		'
		' Will check the role according to its corresponding role definition
		' provided in relation's relation type
		' Will send a notification (RelationNotification with type
		' RELATION_BASIC_UPDATE or RELATION_MBEAN_UPDATE, depending if the
		' relation is a MBean or not) if not initialization of role.
		'
		' -param aRole  role to be set (name and new value)
		' -param relationServCallFlg  true if call from the Relation Service; this
		'  will happen if the current RelationSupport object has been created by
		'  the Relation Service (via createRelation()) method, so direct access is
		'  possible.
		' -param relationServ  reference to Relation Service object, if internal
		'  relation
		' -param multiRoleFlg  true if getting the role in the scope of a
		'  multiple retrieval.
		'
		' -return (except other "critical" exceptions):
		'  - for single role retrieval (multiRoleFlg false):
		'    - null if the role has been set
		'    - raise an InvalidRoleValueException
		' else
		'  - for multi-role retrieval (multiRoleFlg true):
		'    - the Role object for given role name if role has been set
		'    - a RoleUnresolved object with problem else.
		'
		' -exception IllegalArgumentException if null parameter
		' -exception RoleNotFoundException  if multiRoleFlg is false and:
		'  - internal relation and the role does not exist
		'  or
		'  - existing role (i.e. not initializing it) and the role is not
		'    writable.
		' -exception InvalidRoleValueException  ifmultiRoleFlg is false and
		'  value provided for:
		'   - the number of referenced MBeans in given value is less than
		'     expected minimum degree
		'   or
		'   - the number of referenced MBeans in provided value exceeds expected
		'     maximum degree
		'   or
		'   - one referenced MBean in the value is not an Object of the MBean
		'     class expected for that role
		'   or
		'   - a MBean provided for that role does not exist
		' -exception RelationServiceNotRegisteredException  if the Relation
		'  Service is not registered in the MBean Server
		' -exception RelationTypeNotFoundException  if relation type unknown
		' -exception RelationNotFoundException  if a relation MBean has not been
		'  added in the Relation Service
		Friend Overridable Function setRoleInt(ByVal aRole As Role, ByVal relationServCallFlg As Boolean, ByVal relationServ As RelationService, ByVal multiRoleFlg As Boolean) As Object

			If aRole Is Nothing OrElse (relationServCallFlg AndAlso relationServ Is Nothing) Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationSupport).name, "setRoleInt", New Object() {aRole, relationServCallFlg, relationServ, multiRoleFlg})

			Dim roleName As String = aRole.roleName
			Dim pbType As Integer = 0

			' Checks if role exists in the relation
			' No error if the role does not exist in the relation, to be able to
			' handle initialization of role when creating the relation
			' (roles provided in the RoleList parameter are directly set but
			' roles automatically initialized are set using setRole())
			Dim ___role As Role
			SyncLock myRoleName2ValueMap
				___role = (myRoleName2ValueMap(roleName))
			End SyncLock

			Dim oldRoleValue As IList(Of javax.management.ObjectName)
			Dim initFlg As Boolean?

			If ___role Is Nothing Then
				initFlg = True
				oldRoleValue = New List(Of javax.management.ObjectName)

			Else
				initFlg = False
				oldRoleValue = ___role.roleValue
			End If

			' Checks if the role can be set: is writable (except if
			' initialization) and correct value
			Try
				Dim status As Integer?

				If relationServCallFlg Then

					' Call from the Relation Service, so direct access to it,
					' avoiding MBean Server
					'
					' Shall not raise a RelationTypeNotFoundException
					status = relationServ.checkRoleWriting(aRole, myRelTypeName, initFlg)

				Else

					' Call from setRole() method above
					' So we have a MBean. We must access the Relation Service
					' via the MBean Server.
					Dim params As Object() = New Object(2){}
					params(0) = aRole
					params(1) = myRelTypeName
					params(2) = initFlg
					Dim signature As String() = New String(2){}
					signature(0) = "javax.management.relation.Role"
					signature(1) = "java.lang.String"
					signature(2) = "java.lang.Boolean"
					' Can throw InstanceNotFoundException if the Relation Service
					' is not registered (to be transformed into
					' RelationServiceNotRegisteredException in any case).
					'
					' Can throw a MBeanException wrapping a
					' RelationTypeNotFoundException:
					' throw wrapped exception.
					'
					' Shall not throw a ReflectionException
					status = CInt(Fix(myRelServiceMBeanServer.invoke(myRelServiceName, "checkRoleWriting", params, signature)))
				End If

				pbType = status

			Catch exc2 As javax.management.MBeanException

				' Retrieves underlying exception
				Dim wrappedExc As Exception = exc2.targetException
				If TypeOf wrappedExc Is RelationTypeNotFoundException Then
					Throw (CType(wrappedExc, RelationTypeNotFoundException))

				Else
					Throw New Exception(wrappedExc.Message)
				End If

			Catch exc3 As javax.management.ReflectionException
				Throw New Exception(exc3.Message)

			Catch exc4 As RelationTypeNotFoundException
				Throw New Exception(exc4.Message)

			Catch exc5 As javax.management.InstanceNotFoundException
				Throw New RelationServiceNotRegisteredException(exc5.Message)
			End Try

			Dim result As Object = Nothing

			If pbType = 0 Then
				' Role can be set
				If Not(initFlg) Then

					' Not initializing the role
					' If role being initialized:
					' - do not send an update notification
					' - do not try to update internal map of Relation Service
					'   listing referenced MBeans, as role is initialized to an
					'   empty list

					' Sends a notification (RelationNotification)
					' Can throw a RelationNotFoundException
					sendRoleUpdateNotification(aRole, oldRoleValue, relationServCallFlg, relationServ)

					' Updates the role map of the Relation Service
					' Can throw RelationNotFoundException
					updateRelationServiceMap(aRole, oldRoleValue, relationServCallFlg, relationServ)

				End If

				' Sets the role
				SyncLock myRoleName2ValueMap
					myRoleName2ValueMap(roleName) = CType(aRole.clone(), Role)
				End SyncLock

				' Single role set: returns null: nothing to set in result

				If multiRoleFlg Then result = aRole

			Else

				' Role not set

				If Not(multiRoleFlg) Then
					' Problem when setting a simple role: either role not
					' found, not writable, or incorrect value:
					' raises appropriate exception, RoleNotFoundException or
					' InvalidRoleValueException
					RelationService.throwRoleProblemException(pbType, roleName)
					' To keep compiler happy :)
					Return Nothing

				Else
					' Problem when retrieving a role in a multi-role retrieval:
					' returns a RoleUnresolved object
					result = New RoleUnresolved(roleName, aRole.roleValue, pbType)
				End If
			End If

			RELATION_LOGGER.exiting(GetType(RelationSupport).name, "setRoleInt")
			Return result
		End Function

		' Requires the Relation Service to send a notification
		' RelationNotification, with type being either:
		' - RelationNotification.RELATION_BASIC_UPDATE if the updated relation is
		'   a relation internal to the Relation Service
		' - RelationNotification.RELATION_MBEAN_UPDATE if the updated relation is
		'   a relation MBean.
		'
		' -param newRole  new role
		' -param oldRoleValue  old role value (ArrayList of ObjectNames)
		' -param relationServCallFlg  true if call from the Relation Service; this
		'  will happen if the current RelationSupport object has been created by
		'  the Relation Service (via createRelation()) method, so direct access is
		'  possible.
		' -param relationServ  reference to Relation Service object, if object
		'  created by Relation Service.
		'
		' -exception IllegalArgumentException  if null parameter provided
		' -exception RelationServiceNotRegisteredException  if the Relation
		'  Service is not registered in the MBean Server
		' -exception RelationNotFoundException if:
		'  - relation MBean
		'  and
		'  - it has not been added into the Relation Service
		Private Sub sendRoleUpdateNotification(ByVal newRole As Role, ByVal oldRoleValue As IList(Of javax.management.ObjectName), ByVal relationServCallFlg As Boolean, ByVal relationServ As RelationService)

			If newRole Is Nothing OrElse oldRoleValue Is Nothing OrElse (relationServCallFlg AndAlso relationServ Is Nothing) Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationSupport).name, "sendRoleUpdateNotification", New Object() {newRole, oldRoleValue, relationServCallFlg, relationServ})

			If relationServCallFlg Then
				' Direct call to the Relation Service
				' Shall not throw a RelationNotFoundException for an internal
				' relation
				Try
					relationServ.sendRoleUpdateNotification(myRelId, newRole, oldRoleValue)
				Catch exc As RelationNotFoundException
					Throw New Exception(exc.Message)
				End Try

			Else

				Dim params As Object() = New Object(2){}
				params(0) = myRelId
				params(1) = newRole
				params(2) = oldRoleValue
				Dim signature As String() = New String(2){}
				signature(0) = "java.lang.String"
				signature(1) = "javax.management.relation.Role"
				signature(2) = "java.util.List"

				' Can throw InstanceNotFoundException if the Relation Service
				' is not registered (to be transformed).
				'
				' Can throw a MBeanException wrapping a
				' RelationNotFoundException (to be raised in any case): wrapped
				' exception to be thrown
				'
				' Shall not throw a ReflectionException
				Try
					myRelServiceMBeanServer.invoke(myRelServiceName, "sendRoleUpdateNotification", params, signature)
				Catch exc1 As javax.management.ReflectionException
					Throw New Exception(exc1.Message)
				Catch exc2 As javax.management.InstanceNotFoundException
					Throw New RelationServiceNotRegisteredException(exc2.Message)
				Catch exc3 As javax.management.MBeanException
					Dim wrappedExc As Exception = exc3.targetException
					If TypeOf wrappedExc Is RelationNotFoundException Then
						Throw (CType(wrappedExc, RelationNotFoundException))
					Else
						Throw New Exception(wrappedExc.Message)
					End If
				End Try
			End If

			RELATION_LOGGER.exiting(GetType(RelationSupport).name, "sendRoleUpdateNotification")
			Return
		End Sub

		' Requires the Relation Service to update its internal map handling
		' MBeans referenced in relations.
		' The Relation Service will also update its recording as a listener to
		' be informed about unregistration of new referenced MBeans, and no longer
		' informed of MBeans no longer referenced.
		'
		' -param newRole  new role
		' -param oldRoleValue  old role value (ArrayList of ObjectNames)
		' -param relationServCallFlg  true if call from the Relation Service; this
		'  will happen if the current RelationSupport object has been created by
		'  the Relation Service (via createRelation()) method, so direct access is
		'  possible.
		' -param relationServ  reference to Relation Service object, if object
		'  created by Relation Service.
		'
		' -exception IllegalArgumentException  if null parameter
		' -exception RelationServiceNotRegisteredException  if the Relation
		'  Service is not registered in the MBean Server
		' -exception RelationNotFoundException if:
		'  - relation MBean
		'  and
		'  - the relation is not added in the Relation Service
		Private Sub updateRelationServiceMap(ByVal newRole As Role, ByVal oldRoleValue As IList(Of javax.management.ObjectName), ByVal relationServCallFlg As Boolean, ByVal relationServ As RelationService)

			If newRole Is Nothing OrElse oldRoleValue Is Nothing OrElse (relationServCallFlg AndAlso relationServ Is Nothing) Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationSupport).name, "updateRelationServiceMap", New Object() {newRole, oldRoleValue, relationServCallFlg, relationServ})

			If relationServCallFlg Then
				' Direct call to the Relation Service
				' Shall not throw a RelationNotFoundException
				Try
					relationServ.updateRoleMap(myRelId, newRole, oldRoleValue)
				Catch exc As RelationNotFoundException
					Throw New Exception(exc.Message)
				End Try

			Else
				Dim params As Object() = New Object(2){}
				params(0) = myRelId
				params(1) = newRole
				params(2) = oldRoleValue
				Dim signature As String() = New String(2){}
				signature(0) = "java.lang.String"
				signature(1) = "javax.management.relation.Role"
				signature(2) = "java.util.List"
				' Can throw InstanceNotFoundException if the Relation Service
				' is not registered (to be transformed).
				' Can throw a MBeanException wrapping a RelationNotFoundException:
				' wrapped exception to be thrown
				'
				' Shall not throw a ReflectionException
				Try
					myRelServiceMBeanServer.invoke(myRelServiceName, "updateRoleMap", params, signature)
				Catch exc1 As javax.management.ReflectionException
					Throw New Exception(exc1.Message)
				Catch exc2 As javax.management.InstanceNotFoundException
					Throw New RelationServiceNotRegisteredException(exc2.Message)
				Catch exc3 As javax.management.MBeanException
					Dim wrappedExc As Exception = exc3.targetException
					If TypeOf wrappedExc Is RelationNotFoundException Then
						Throw (CType(wrappedExc, RelationNotFoundException))
					Else
						Throw New Exception(wrappedExc.Message)
					End If
				End Try
			End If

			RELATION_LOGGER.exiting(GetType(RelationSupport).name, "updateRelationServiceMap")
			Return
		End Sub

		' Sets the given roles
		' For each role:
		' - will check the role according to its corresponding role definition
		'   provided in relation's relation type
		' - will send a notification (RelationNotification with type
		'   RELATION_BASIC_UPDATE or RELATION_MBEAN_UPDATE, depending if the
		'   relation is a MBean or not) for each updated role.
		'
		' This method is called in setRoles() above and in Relation Service
		' setRoles() method.
		'
		' -param list  list of roles to be set
		' -param relationServCallFlg  true if call from the Relation Service; this
		'  will happen if the current RelationSupport object has been created by
		'  the Relation Service (via createRelation()) method, so direct access is
		'  possible.
		' -param relationServ  reference to Relation Service object, if object
		'  created by Relation Service.
		'
		' -return a RoleResult object
		'
		' -exception IllegalArgumentException  if null parameter
		' -exception RelationServiceNotRegisteredException  if the Relation
		'  Service is not registered in the MBean Server
		' -exception RelationTypeNotFoundException if:
		'  - relation MBean
		'  and
		'  - unknown relation type
		' -exception RelationNotFoundException if:
		'  - relation MBean
		' and
		' - not added in the RS
		Friend Overridable Function setRolesInt(ByVal list As RoleList, ByVal relationServCallFlg As Boolean, ByVal relationServ As RelationService) As RoleResult

			If list Is Nothing OrElse (relationServCallFlg AndAlso relationServ Is Nothing) Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationSupport).name, "setRolesInt", New Object() {list, relationServCallFlg, relationServ})

			Dim roleList As New RoleList
			Dim roleUnresList As New RoleUnresolvedList

			For Each currRole As Role In list.asList()

				Dim currResult As Object = Nothing
				' Can throw:
				' RelationServiceNotRegisteredException,
				' RelationTypeNotFoundException
				'
				' Will not throw, due to parameters, RoleNotFoundException or
				' InvalidRoleValueException, but catch them to keep compiler
				' happy
				Try
					currResult = roleIntInt(currRole, relationServCallFlg, relationServ, True)
				Catch exc1 As RoleNotFoundException
					' OK : Do not throw a RoleNotFoundException.
				Catch exc2 As InvalidRoleValueException
					' OK : Do not throw an InvalidRoleValueException.
				End Try

				If TypeOf currResult Is Role Then
					' Can throw IllegalArgumentException if role is null
					' (normally should not happen :(
					Try
						roleList.Add(CType(currResult, Role))
					Catch exc As System.ArgumentException
						Throw New Exception(exc.Message)
					End Try

				ElseIf TypeOf currResult Is RoleUnresolved Then
					' Can throw IllegalArgumentException if role is null
					' (normally should not happen :(
					Try
						roleUnresList.Add(CType(currResult, RoleUnresolved))
					Catch exc As System.ArgumentException
						Throw New Exception(exc.Message)
					End Try
				End If
			Next currRole

			Dim result As New RoleResult(roleList, roleUnresList)

			RELATION_LOGGER.exiting(GetType(RelationSupport).name, "setRolesInt")
			Return result
		End Function

		' Initializes all members
		'
		' -param relationId  relation identifier, to identify the relation in the
		' Relation Service.
		' Expected to be unique in the given Relation Service.
		' -param relationServiceName  ObjectName of the Relation Service where
		' the relation will be registered.
		' It is required as this is the Relation Service that is aware of the
		' definition of the relation type of given relation, so that will be able
		' to check update operations (set). Direct access via the Relation
		' Service (RelationService.setRole()) do not need this information but
		' as any user relation is a MBean, setRole() is part of its management
		' interface and can be called directly on the user relation MBean. So the
		' user relation MBean must be aware of the Relation Service where it will
		' be added.
		' -param relationTypeName  Name of relation type.
		' Expected to have been created in given Relation Service.
		' -param list  list of roles (Role objects) to initialized the
		' relation. Can be null.
		' Expected to conform to relation info in associated relation type.
		'
		' -exception InvalidRoleValueException  if the same name is used for two
		'  roles.
		' -exception IllegalArgumentException  if a required value (Relation
		'  Service Object Name, etc.) is not provided as parameter.
		Private Sub initMembers(ByVal relationId As String, ByVal relationServiceName As javax.management.ObjectName, ByVal relationServiceMBeanServer As javax.management.MBeanServer, ByVal relationTypeName As String, ByVal list As RoleList)

			If relationId Is Nothing OrElse relationServiceName Is Nothing OrElse relationTypeName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationSupport).name, "initMembers", New Object() {relationId, relationServiceName, relationServiceMBeanServer, relationTypeName, list})

			myRelId = relationId
			myRelServiceName = relationServiceName
			myRelServiceMBeanServer = relationServiceMBeanServer
			myRelTypeName = relationTypeName
			' Can throw InvalidRoleValueException
			initRoleMap(list)

			RELATION_LOGGER.exiting(GetType(RelationSupport).name, "initMembers")
			Return
		End Sub

		' Initialize the internal role map from given RoleList parameter
		'
		' -param list  role list. Can be null.
		'  As it is a RoleList object, it cannot include null (rejected).
		'
		' -exception InvalidRoleValueException  if the same role name is used for
		'  several roles.
		'
		Private Sub initRoleMap(ByVal list As RoleList)

			If list Is Nothing Then Return

			RELATION_LOGGER.entering(GetType(RelationSupport).name, "initRoleMap", list)

			SyncLock myRoleName2ValueMap

				For Each currRole As Role In list.asList()

					' No need to check if role is null, it is not allowed to store
					' a null role in a RoleList :)
					Dim currRoleName As String = currRole.roleName

					If myRoleName2ValueMap.ContainsKey(currRoleName) Then
						' Role already provided in current list
						Dim excMsgStrB As New StringBuilder("Role name ")
						excMsgStrB.Append(currRoleName)
						excMsgStrB.Append(" used for two roles.")
						Throw New InvalidRoleValueException(excMsgStrB.ToString())
					End If

					myRoleName2ValueMap(currRoleName) = CType(currRole.clone(), Role)
				Next currRole
			End SyncLock

			RELATION_LOGGER.exiting(GetType(RelationSupport).name, "initRoleMap")
			Return
		End Sub

		' Callback used by the Relation Service when a MBean referenced in a role
		' is unregistered.
		' The Relation Service will call this method to let the relation
		' take action to reflect the impact of such unregistration.
		' Current implementation is to set the role with its current value
		' (list of ObjectNames of referenced MBeans) without the unregistered
		' one.
		'
		' -param objectName  ObjectName of unregistered MBean
		' -param roleName  name of role where the MBean is referenced
		' -param relationServCallFlg  true if call from the Relation Service; this
		'  will happen if the current RelationSupport object has been created by
		'  the Relation Service (via createRelation()) method, so direct access is
		'  possible.
		' -param relationServ  reference to Relation Service object, if internal
		'  relation
		'
		' -exception IllegalArgumentException if null parameter
		' -exception RoleNotFoundException  if:
		'  - the role does not exist
		'  or
		'  - role not writable.
		' -exception InvalidRoleValueException  if value provided for:
		'   - the number of referenced MBeans in given value is less than
		'     expected minimum degree
		'   or
		'   - the number of referenced MBeans in provided value exceeds expected
		'     maximum degree
		'   or
		'   - one referenced MBean in the value is not an Object of the MBean
		'     class expected for that role
		'   or
		'   - a MBean provided for that role does not exist
		' -exception RelationServiceNotRegisteredException  if the Relation
		'  Service is not registered in the MBean Server
		' -exception RelationTypeNotFoundException if unknown relation type
		' -exception RelationNotFoundException if current relation has not been
		'  added in the RS
		Friend Overridable Sub handleMBeanUnregistrationInt(ByVal ___objectName As javax.management.ObjectName, ByVal roleName As String, ByVal relationServCallFlg As Boolean, ByVal relationServ As RelationService)

			If ___objectName Is Nothing OrElse roleName Is Nothing OrElse (relationServCallFlg AndAlso relationServ Is Nothing) Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(RelationSupport).name, "handleMBeanUnregistrationInt", New Object() {___objectName, roleName, relationServCallFlg, relationServ})

			' Retrieves current role value
			Dim ___role As Role
			SyncLock myRoleName2ValueMap
				___role = (myRoleName2ValueMap(roleName))
			End SyncLock

			If ___role Is Nothing Then
				Dim excMsgStrB As New StringBuilder
				Dim excMsg As String = "No role with name "
				excMsgStrB.Append(excMsg)
				excMsgStrB.Append(roleName)
				Throw New RoleNotFoundException(excMsgStrB.ToString())
			End If
			Dim currRoleValue As IList(Of javax.management.ObjectName) = ___role.roleValue

			' Note: no need to test if list not null before cloning, null value
			'       not allowed for role value.
			Dim newRoleValue As IList(Of javax.management.ObjectName) = New List(Of javax.management.ObjectName)(currRoleValue)
			newRoleValue.Remove(___objectName)
			Dim newRole As New Role(roleName, newRoleValue)

			' Can throw InvalidRoleValueException,
			' RelationTypeNotFoundException
			' (RoleNotFoundException already detected)
			Dim result As Object = roleIntInt(newRole, relationServCallFlg, relationServ, False)

			RELATION_LOGGER.exiting(GetType(RelationSupport).name, "handleMBeanUnregistrationInt")
			Return
		End Sub

	End Class

End Namespace
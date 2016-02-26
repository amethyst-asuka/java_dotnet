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
	''' This interface has to be implemented by any MBean class expected to
	''' represent a relation managed using the Relation Service.
	''' <P>Simple relations, i.e. having only roles, no properties or methods, can
	''' be created directly by the Relation Service (represented as RelationSupport
	''' objects, internally handled by the Relation Service).
	''' <P>If the user wants to represent more complex relations, involving
	''' properties and/or methods, he has to provide his own class implementing the
	''' Relation interface. This can be achieved either by inheriting from
	''' RelationSupport class, or by implementing the interface (fully or delegation to
	''' a RelationSupport object member).
	''' <P>Specifying such user relation class is to introduce properties and/or
	''' methods. Those have to be exposed for remote management. So this means that
	''' any user relation class must be a MBean class.
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface Relation

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
		Function getRole(ByVal roleName As String) As IList(Of javax.management.ObjectName)

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
		Function getRoles(ByVal roleNameArray As String()) As RoleResult

		''' <summary>
		''' Returns the number of MBeans currently referenced in the given role.
		''' </summary>
		''' <param name="roleName">  name of role
		''' </param>
		''' <returns> the number of currently referenced MBeans in that role
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null role name </exception>
		''' <exception cref="RoleNotFoundException">  if there is no role with given name </exception>
		Function getRoleCardinality(ByVal roleName As String) As Integer?

		''' <summary>
		''' Returns all roles present in the relation.
		''' </summary>
		''' <returns> a RoleResult object, including a RoleList (for roles
		''' successfully retrieved) and a RoleUnresolvedList (for roles not
		''' readable).
		''' </returns>
		''' <exception cref="RelationServiceNotRegisteredException">  if the Relation
		''' Service is not registered in the MBean Server </exception>
		ReadOnly Property allRoles As RoleResult

		''' <summary>
		''' Returns all roles in the relation without checking read mode.
		''' </summary>
		''' <returns> a RoleList. </returns>
		Function retrieveAllRoles() As RoleList

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
		''' <P>- a MBean provided for that role does not exist. </exception>
		''' <exception cref="RelationServiceNotRegisteredException">  if the Relation
		''' Service is not registered in the MBean Server </exception>
		''' <exception cref="RelationTypeNotFoundException">  if the relation type has not
		''' been declared in the Relation Service. </exception>
		''' <exception cref="RelationNotFoundException">  if the relation has not been
		''' added in the Relation Service.
		''' </exception>
		''' <seealso cref= #getRole </seealso>
		WriteOnly Property role As Role

		''' <summary>
		''' Sets the given roles.
		''' <P>Will check the role according to its corresponding role definition
		''' provided in relation's relation type
		''' <P>Will send one notification (RelationNotification with type
		''' RELATION_BASIC_UPDATE or RELATION_MBEAN_UPDATE, depending if the
		''' relation is a MBean or not) per updated role.
		''' </summary>
		''' <param name="roleList">  list of roles to be set
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
		Function setRoles(ByVal roleList As RoleList) As RoleResult

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
		Sub handleMBeanUnregistration(ByVal objectName As javax.management.ObjectName, ByVal roleName As String)

		''' <summary>
		''' Retrieves MBeans referenced in the various roles of the relation.
		''' </summary>
		''' <returns> a HashMap mapping:
		''' <P> ObjectName {@literal ->} ArrayList of String (role names) </returns>
		ReadOnly Property referencedMBeans As IDictionary(Of javax.management.ObjectName, IList(Of String))

		''' <summary>
		''' Returns name of associated relation type.
		''' </summary>
		''' <returns> the name of the relation type. </returns>
		ReadOnly Property relationTypeName As String

		''' <summary>
		''' Returns ObjectName of the Relation Service handling the relation.
		''' </summary>
		''' <returns> the ObjectName of the Relation Service. </returns>
		ReadOnly Property relationServiceName As javax.management.ObjectName

		''' <summary>
		''' Returns relation identifier (used to uniquely identify the relation
		''' inside the Relation Service).
		''' </summary>
		''' <returns> the relation id. </returns>
		ReadOnly Property relationId As String
	End Interface

End Namespace
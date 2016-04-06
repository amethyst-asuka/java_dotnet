Imports System.Collections.Generic

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.security.acl


	''' <summary>
	''' This interface is used to represent a group of principals. (A principal
	''' represents an entity such as an individual user or a company). <p>
	''' 
	''' Note that Group extends Principal. Thus, either a Principal or a Group can
	''' be passed as an argument to methods containing a Principal parameter. For
	''' example, you can add either a Principal or a Group to a Group object by
	''' calling the object's {@code addMember} method, passing it the
	''' Principal or Group.
	''' 
	''' @author      Satish Dharmaraj
	''' </summary>
	Public Interface Group
		Inherits java.security.Principal

		''' <summary>
		''' Adds the specified member to the group.
		''' </summary>
		''' <param name="user"> the principal to add to this group.
		''' </param>
		''' <returns> true if the member was successfully added,
		''' false if the principal was already a member. </returns>
		Function addMember(  user As java.security.Principal) As Boolean

		''' <summary>
		''' Removes the specified member from the group.
		''' </summary>
		''' <param name="user"> the principal to remove from this group.
		''' </param>
		''' <returns> true if the principal was removed, or
		''' false if the principal was not a member. </returns>
		Function removeMember(  user As java.security.Principal) As Boolean

		''' <summary>
		''' Returns true if the passed principal is a member of the group.
		''' This method does a recursive search, so if a principal belongs to a
		''' group which is a member of this group, true is returned.
		''' </summary>
		''' <param name="member"> the principal whose membership is to be checked.
		''' </param>
		''' <returns> true if the principal is a member of this group,
		''' false otherwise. </returns>
		Function isMember(  member As java.security.Principal) As Boolean


		''' <summary>
		''' Returns an enumeration of the members in the group.
		''' The returned objects can be instances of either Principal
		''' or Group (which is a subclass of Principal).
		''' </summary>
		''' <returns> an enumeration of the group members. </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Function members() As System.Collections.IEnumerator(Of ? As java.security.Principal)

	End Interface

End Namespace
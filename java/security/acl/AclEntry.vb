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
	''' This is the interface used for representing one entry in an Access
	''' Control List (ACL).<p>
	''' 
	''' An ACL can be thought of as a data structure with multiple ACL entry
	''' objects. Each ACL entry object contains a set of permissions associated
	''' with a particular principal. (A principal represents an entity such as
	''' an individual user or a group). Additionally, each ACL entry is specified
	''' as being either positive or negative. If positive, the permissions are
	''' to be granted to the associated principal. If negative, the permissions
	''' are to be denied. Each principal can have at most one positive ACL entry
	''' and one negative entry; that is, multiple positive or negative ACL
	''' entries are not allowed for any principal.
	''' 
	''' Note: ACL entries are by default positive. An entry becomes a
	''' negative entry only if the
	''' <seealso cref="#setNegativePermissions() setNegativePermissions"/>
	''' method is called on it.
	''' </summary>
	''' <seealso cref= java.security.acl.Acl
	''' 
	''' @author      Satish Dharmaraj </seealso>
	Public Interface AclEntry
		Inherits Cloneable

		''' <summary>
		''' Specifies the principal for which permissions are granted or denied
		''' by this ACL entry. If a principal was already set for this ACL entry,
		''' false is returned, otherwise true is returned.
		''' </summary>
		''' <param name="user"> the principal to be set for this entry.
		''' </param>
		''' <returns> true if the principal is set, false if there was
		''' already a principal set for this entry.
		''' </returns>
		''' <seealso cref= #getPrincipal </seealso>
		Function setPrincipal(ByVal user As java.security.Principal) As Boolean

		''' <summary>
		''' Returns the principal for which permissions are granted or denied by
		''' this ACL entry. Returns null if there is no principal set for this
		''' entry yet.
		''' </summary>
		''' <returns> the principal associated with this entry.
		''' </returns>
		''' <seealso cref= #setPrincipal </seealso>
		ReadOnly Property principal As java.security.Principal

		''' <summary>
		''' Sets this ACL entry to be a negative one. That is, the associated
		''' principal (e.g., a user or a group) will be denied the permission set
		''' specified in the entry.
		''' 
		''' Note: ACL entries are by default positive. An entry becomes a
		''' negative entry only if this {@code setNegativePermissions}
		''' method is called on it.
		''' </summary>
		Sub setNegativePermissions()

		''' <summary>
		''' Returns true if this is a negative ACL entry (one denying the
		''' associated principal the set of permissions in the entry), false
		''' otherwise.
		''' </summary>
		''' <returns> true if this is a negative ACL entry, false if it's not. </returns>
		ReadOnly Property negative As Boolean

		''' <summary>
		''' Adds the specified permission to this ACL entry. Note: An entry can
		''' have multiple permissions.
		''' </summary>
		''' <param name="permission"> the permission to be associated with
		''' the principal in this entry.
		''' </param>
		''' <returns> true if the permission was added, false if the
		''' permission was already part of this entry's permission set. </returns>
		Function addPermission(ByVal permission As Permission) As Boolean

		''' <summary>
		''' Removes the specified permission from this ACL entry.
		''' </summary>
		''' <param name="permission"> the permission to be removed from this entry.
		''' </param>
		''' <returns> true if the permission is removed, false if the
		''' permission was not part of this entry's permission set. </returns>
		Function removePermission(ByVal permission As Permission) As Boolean

		''' <summary>
		''' Checks if the specified permission is part of the
		''' permission set in this entry.
		''' </summary>
		''' <param name="permission"> the permission to be checked for.
		''' </param>
		''' <returns> true if the permission is part of the
		''' permission set in this entry, false otherwise. </returns>
		Function checkPermission(ByVal permission As Permission) As Boolean

		''' <summary>
		''' Returns an enumeration of the permissions in this ACL entry.
		''' </summary>
		''' <returns> an enumeration of the permissions in this ACL entry. </returns>
		Function permissions() As System.Collections.IEnumerator(Of Permission)

		''' <summary>
		''' Returns a string representation of the contents of this ACL entry.
		''' </summary>
		''' <returns> a string representation of the contents. </returns>
		Function ToString() As String

		''' <summary>
		''' Clones this ACL entry.
		''' </summary>
		''' <returns> a clone of this ACL entry. </returns>
		Function clone() As Object
	End Interface

End Namespace
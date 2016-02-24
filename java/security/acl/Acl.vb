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
	''' Interface representing an Access Control List (ACL).  An Access
	''' Control List is a data structure used to guard access to
	''' resources.<p>
	''' 
	''' An ACL can be thought of as a data structure with multiple ACL
	''' entries.  Each ACL entry, of interface type AclEntry, contains a
	''' set of permissions associated with a particular principal. (A
	''' principal represents an entity such as an individual user or a
	''' group). Additionally, each ACL entry is specified as being either
	''' positive or negative. If positive, the permissions are to be
	''' granted to the associated principal. If negative, the permissions
	''' are to be denied.<p>
	''' 
	''' The ACL Entries in each ACL observe the following rules:
	''' 
	''' <ul> <li>Each principal can have at most one positive ACL entry and
	''' one negative entry; that is, multiple positive or negative ACL
	''' entries are not allowed for any principal.  Each entry specifies
	''' the set of permissions that are to be granted (if positive) or
	''' denied (if negative).
	''' 
	''' <li>If there is no entry for a particular principal, then the
	''' principal is considered to have a null (empty) permission set.
	''' 
	''' <li>If there is a positive entry that grants a principal a
	''' particular permission, and a negative entry that denies the
	''' principal the same permission, the result is as though the
	''' permission was never granted or denied.
	''' 
	''' <li>Individual permissions always override permissions of the
	''' group(s) to which the individual belongs. That is, individual
	''' negative permissions (specific denial of permissions) override the
	''' groups' positive permissions. And individual positive permissions
	''' override the groups' negative permissions.
	''' 
	''' </ul>
	''' 
	''' The {@code  java.security.acl } package provides the
	''' interfaces to the ACL and related data structures (ACL entries,
	''' groups, permissions, etc.), and the {@code  sun.security.acl }
	''' classes provide a default implementation of the interfaces. For
	''' example, {@code  java.security.acl.Acl } provides the
	''' interface to an ACL and the {@code  sun.security.acl.AclImpl }
	''' class provides the default implementation of the interface.<p>
	''' 
	''' The {@code  java.security.acl.Acl } interface extends the
	''' {@code  java.security.acl.Owner } interface. The Owner
	''' interface is used to maintain a list of owners for each ACL.  Only
	''' owners are allowed to modify an ACL. For example, only an owner can
	''' call the ACL's {@code addEntry} method to add a new ACL entry
	''' to the ACL.
	''' </summary>
	''' <seealso cref= java.security.acl.AclEntry </seealso>
	''' <seealso cref= java.security.acl.Owner </seealso>
	''' <seealso cref= java.security.acl.Acl#getPermissions
	''' 
	''' @author Satish Dharmaraj </seealso>

	Public Interface Acl
		Inherits Owner

		''' <summary>
		''' Sets the name of this ACL.
		''' </summary>
		''' <param name="caller"> the principal invoking this method. It must be an
		''' owner of this ACL.
		''' </param>
		''' <param name="name"> the name to be given to this ACL.
		''' </param>
		''' <exception cref="NotOwnerException"> if the caller principal
		''' is not an owner of this ACL.
		''' </exception>
		''' <seealso cref= #getName </seealso>
		Sub setName(ByVal caller As java.security.Principal, ByVal name As String)

		''' <summary>
		''' Returns the name of this ACL.
		''' </summary>
		''' <returns> the name of this ACL.
		''' </returns>
		''' <seealso cref= #setName </seealso>
		ReadOnly Property name As String

		''' <summary>
		''' Adds an ACL entry to this ACL. An entry associates a principal
		''' (e.g., an individual or a group) with a set of
		''' permissions. Each principal can have at most one positive ACL
		''' entry (specifying permissions to be granted to the principal)
		''' and one negative ACL entry (specifying permissions to be
		''' denied). If there is already an ACL entry of the same type
		''' (negative or positive) already in the ACL, false is returned.
		''' </summary>
		''' <param name="caller"> the principal invoking this method. It must be an
		''' owner of this ACL.
		''' </param>
		''' <param name="entry"> the ACL entry to be added to this ACL.
		''' </param>
		''' <returns> true on success, false if an entry of the same type
		''' (positive or negative) for the same principal is already
		''' present in this ACL.
		''' </returns>
		''' <exception cref="NotOwnerException"> if the caller principal
		'''  is not an owner of this ACL. </exception>
		Function addEntry(ByVal caller As java.security.Principal, ByVal entry As AclEntry) As Boolean

		''' <summary>
		''' Removes an ACL entry from this ACL.
		''' </summary>
		''' <param name="caller"> the principal invoking this method. It must be an
		''' owner of this ACL.
		''' </param>
		''' <param name="entry"> the ACL entry to be removed from this ACL.
		''' </param>
		''' <returns> true on success, false if the entry is not part of this ACL.
		''' </returns>
		''' <exception cref="NotOwnerException"> if the caller principal is not
		''' an owner of this Acl. </exception>
		Function removeEntry(ByVal caller As java.security.Principal, ByVal entry As AclEntry) As Boolean

		''' <summary>
		''' Returns an enumeration for the set of allowed permissions for the
		''' specified principal (representing an entity such as an individual or
		''' a group). This set of allowed permissions is calculated as
		''' follows:
		''' 
		''' <ul>
		''' 
		''' <li>If there is no entry in this Access Control List for the
		''' specified principal, an empty permission set is returned.
		''' 
		''' <li>Otherwise, the principal's group permission sets are determined.
		''' (A principal can belong to one or more groups, where a group is a
		''' group of principals, represented by the Group interface.)
		''' The group positive permission set is the union of all
		''' the positive permissions of each group that the principal belongs to.
		''' The group negative permission set is the union of all
		''' the negative permissions of each group that the principal belongs to.
		''' If there is a specific permission that occurs in both
		''' the positive permission set and the negative permission set,
		''' it is removed from both.<p>
		''' 
		''' The individual positive and negative permission sets are also
		''' determined. The positive permission set contains the permissions
		''' specified in the positive ACL entry (if any) for the principal.
		''' Similarly, the negative permission set contains the permissions
		''' specified in the negative ACL entry (if any) for the principal.
		''' The individual positive (or negative) permission set is considered
		''' to be null if there is not a positive (negative) ACL entry for the
		''' principal in this ACL.<p>
		''' 
		''' The set of permissions granted to the principal is then calculated
		''' using the simple rule that individual permissions always override
		''' the group permissions. That is, the principal's individual negative
		''' permission set (specific denial of permissions) overrides the group
		''' positive permission set, and the principal's individual positive
		''' permission set overrides the group negative permission set.
		''' 
		''' </ul>
		''' </summary>
		''' <param name="user"> the principal whose permission set is to be returned.
		''' </param>
		''' <returns> the permission set specifying the permissions the principal
		''' is allowed. </returns>
		Function getPermissions(ByVal user As java.security.Principal) As System.Collections.IEnumerator(Of Permission)

		''' <summary>
		''' Returns an enumeration of the entries in this ACL. Each element in
		''' the enumeration is of type AclEntry.
		''' </summary>
		''' <returns> an enumeration of the entries in this ACL. </returns>
		Function entries() As System.Collections.IEnumerator(Of AclEntry)

		''' <summary>
		''' Checks whether or not the specified principal has the specified
		''' permission. If it does, true is returned, otherwise false is returned.
		''' 
		''' More specifically, this method checks whether the passed permission
		''' is a member of the allowed permission set of the specified principal.
		''' The allowed permission set is determined by the same algorithm as is
		''' used by the {@code getPermissions} method.
		''' </summary>
		''' <param name="principal"> the principal, assumed to be a valid authenticated
		''' Principal.
		''' </param>
		''' <param name="permission"> the permission to be checked for.
		''' </param>
		''' <returns> true if the principal has the specified permission, false
		''' otherwise.
		''' </returns>
		''' <seealso cref= #getPermissions </seealso>
		Function checkPermission(ByVal principal As java.security.Principal, ByVal permission As Permission) As Boolean

		''' <summary>
		''' Returns a string representation of the
		''' ACL contents.
		''' </summary>
		''' <returns> a string representation of the ACL contents. </returns>
		Function ToString() As String
	End Interface

End Namespace
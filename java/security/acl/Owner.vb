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
	''' Interface for managing owners of Access Control Lists (ACLs) or ACL
	''' configurations. (Note that the Acl interface in the
	''' {@code  java.security.acl} package extends this Owner
	''' interface.) The initial owner Principal should be specified as an
	''' argument to the constructor of the class implementing this interface.
	''' </summary>
	''' <seealso cref= java.security.acl.Acl
	'''  </seealso>
	Public Interface Owner

		''' <summary>
		''' Adds an owner. Only owners can modify ACL contents. The caller
		''' principal must be an owner of the ACL in order to invoke this method.
		''' That is, only an owner can add another owner. The initial owner is
		''' configured at ACL construction time.
		''' </summary>
		''' <param name="caller"> the principal invoking this method. It must be an owner
		''' of the ACL.
		''' </param>
		''' <param name="owner"> the owner that should be added to the list of owners.
		''' </param>
		''' <returns> true if successful, false if owner is already an owner. </returns>
		''' <exception cref="NotOwnerException"> if the caller principal is not an owner
		''' of the ACL. </exception>
		Function addOwner(  caller As java.security.Principal,   owner As java.security.Principal) As Boolean

		''' <summary>
		''' Deletes an owner. If this is the last owner in the ACL, an exception is
		''' raised.<p>
		''' 
		''' The caller principal must be an owner of the ACL in order to invoke
		''' this method.
		''' </summary>
		''' <param name="caller"> the principal invoking this method. It must be an owner
		''' of the ACL.
		''' </param>
		''' <param name="owner"> the owner to be removed from the list of owners.
		''' </param>
		''' <returns> true if the owner is removed, false if the owner is not part
		''' of the list of owners.
		''' </returns>
		''' <exception cref="NotOwnerException"> if the caller principal is not an owner
		''' of the ACL.
		''' </exception>
		''' <exception cref="LastOwnerException"> if there is only one owner left, so that
		''' deleteOwner would leave the ACL owner-less. </exception>
		Function deleteOwner(  caller As java.security.Principal,   owner As java.security.Principal) As Boolean

		''' <summary>
		''' Returns true if the given principal is an owner of the ACL.
		''' </summary>
		''' <param name="owner"> the principal to be checked to determine whether or not
		''' it is an owner.
		''' </param>
		''' <returns> true if the passed principal is in the list of owners, false
		''' if not. </returns>
		Function isOwner(  owner As java.security.Principal) As Boolean

	End Interface

End Namespace
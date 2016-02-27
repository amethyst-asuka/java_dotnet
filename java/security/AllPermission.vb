Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.security


	''' <summary>
	''' The AllPermission is a permission that implies all other permissions.
	''' <p>
	''' <b>Note:</b> Granting AllPermission should be done with extreme care,
	''' as it implies all other permissions. Thus, it grants code the ability
	''' to run with security
	''' disabled.  Extreme caution should be taken before granting such
	''' a permission to code.  This permission should be used only during testing,
	''' or in extremely rare cases where an application or applet is
	''' completely trusted and adding the necessary permissions to the policy
	''' is prohibitively cumbersome.
	''' </summary>
	''' <seealso cref= java.security.Permission </seealso>
	''' <seealso cref= java.security.AccessController </seealso>
	''' <seealso cref= java.security.Permissions </seealso>
	''' <seealso cref= java.security.PermissionCollection </seealso>
	''' <seealso cref= java.lang.SecurityManager
	''' 
	''' 
	''' @author Roland Schemers
	''' 
	''' @serial exclude </seealso>

	Public NotInheritable Class AllPermission
		Inherits Permission

		Private Const serialVersionUID As Long = -2916474571451318075L

		''' <summary>
		''' Creates a new AllPermission object.
		''' </summary>
		Public Sub New()
			MyBase.New("<all permissions>")
		End Sub


		''' <summary>
		''' Creates a new AllPermission object. This
		''' constructor exists for use by the {@code Policy} object
		''' to instantiate new Permission objects.
		''' </summary>
		''' <param name="name"> ignored </param>
		''' <param name="actions"> ignored. </param>
		Public Sub New(ByVal name As String, ByVal actions As String)
			Me.New()
		End Sub

		''' <summary>
		''' Checks if the specified permission is "implied" by
		''' this object. This method always returns true.
		''' </summary>
		''' <param name="p"> the permission to check against.
		''' </param>
		''' <returns> return </returns>
		Public Overrides Function implies(ByVal p As Permission) As Boolean
			 Return True
		End Function

		''' <summary>
		''' Checks two AllPermission objects for equality. Two AllPermission
		''' objects are always equal.
		''' </summary>
		''' <param name="obj"> the object we are testing for equality with this object. </param>
		''' <returns> true if <i>obj</i> is an AllPermission, false otherwise. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return (TypeOf obj Is AllPermission)
		End Function

		''' <summary>
		''' Returns the hash code value for this object.
		''' </summary>
		''' <returns> a hash code value for this object. </returns>

		Public Overrides Function GetHashCode() As Integer
			Return 1
		End Function

        ''' <summary>
        ''' Returns the canonical string representation of the actions.
        ''' </summary>
        ''' <returns> the actions. </returns>
        Public Overrides ReadOnly Property actions As String
            Get
                Return "<all actions>"
            End Get
        End Property

        ''' <summary>
        ''' Returns a new PermissionCollection object for storing AllPermission
        ''' objects.
        ''' <p>
        ''' </summary>
        ''' <returns> a new PermissionCollection object suitable for
        ''' storing AllPermissions. </returns>
        Public Overrides Function newPermissionCollection() As PermissionCollection
			Return New AllPermissionCollection
		End Function

	End Class

	''' <summary>
	''' A AllPermissionCollection stores a collection
	''' of AllPermission permissions. AllPermission objects
	''' must be stored in a manner that allows them to be inserted in any
	''' order, but enable the implies function to evaluate the implies
	''' method in an efficient (and consistent) manner.
	''' </summary>
	''' <seealso cref= java.security.Permission </seealso>
	''' <seealso cref= java.security.Permissions
	''' 
	''' 
	''' @author Roland Schemers
	''' 
	''' @serial include </seealso>

	<Serializable> _
	Friend NotInheritable Class AllPermissionCollection
		Inherits PermissionCollection

		' use serialVersionUID from JDK 1.2.2 for interoperability
		Private Const serialVersionUID As Long = -4023755556366636806L

		Private all_allowed As Boolean ' true if any all permissions have been added

		''' <summary>
		''' Create an empty AllPermissions object.
		''' 
		''' </summary>

		Public Sub New()
			all_allowed = False
		End Sub

		''' <summary>
		''' Adds a permission to the AllPermissions. The key for the hash is
		''' permission.path.
		''' </summary>
		''' <param name="permission"> the Permission object to add.
		''' </param>
		''' <exception cref="IllegalArgumentException"> - if the permission is not a
		'''                                       AllPermission
		''' </exception>
		''' <exception cref="SecurityException"> - if this AllPermissionCollection object
		'''                                has been marked readonly </exception>

		Public Overrides Sub add(ByVal permission As Permission)
			If Not(TypeOf permission Is AllPermission) Then Throw New IllegalArgumentException("invalid permission: " & permission)
			If [readOnly] Then Throw New SecurityException("attempt to add a Permission to a readonly PermissionCollection")

			all_allowed = True ' No sync; staleness OK
		End Sub

		''' <summary>
		''' Check and see if this set of permissions implies the permissions
		''' expressed in "permission".
		''' </summary>
		''' <param name="permission"> the Permission object to compare
		''' </param>
		''' <returns> always returns true. </returns>

		Public Overrides Function implies(ByVal permission As Permission) As Boolean
			Return all_allowed ' No sync; staleness OK
		End Function

		''' <summary>
		''' Returns an enumeration of all the AllPermission objects in the
		''' container.
		''' </summary>
		''' <returns> an enumeration of all the AllPermission objects. </returns>
		Public Overrides Function elements() As System.Collections.IEnumerator(Of Permission)
			Return New EnumerationAnonymousInnerClassHelper(Of E)
		End Function

		Private Class EnumerationAnonymousInnerClassHelper(Of E)
			Implements System.Collections.IEnumerator(Of E)

			Private hasMore As Boolean = outerInstance.all_allowed

			Public Overridable Function hasMoreElements() As Boolean
				Return hasMore
			End Function

			Public Overridable Function nextElement() As Permission
				hasMore = False
				Return sun.security.util.SecurityConstants.ALL_PERMISSION
			End Function
		End Class
	End Class

End Namespace
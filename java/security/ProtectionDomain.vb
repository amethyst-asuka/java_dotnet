Imports Microsoft.VisualBasic
Imports System.Collections.Generic
import static sun.misc.JavaSecurityProtectionDomainAccess.ProtectionDomainCache

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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


	''' 
	''' <summary>
	''' <p>
	''' This ProtectionDomain class encapsulates the characteristics of a domain,
	''' which encloses a set of classes whose instances are granted a set
	''' of permissions when being executed on behalf of a given set of Principals.
	''' <p>
	''' A static set of permissions can be bound to a ProtectionDomain when it is
	''' constructed; such permissions are granted to the domain regardless of the
	''' Policy in force. However, to support dynamic security policies, a
	''' ProtectionDomain can also be constructed such that it is dynamically
	''' mapped to a set of permissions by the current Policy whenever a permission
	''' is checked.
	''' <p>
	''' 
	''' @author Li Gong
	''' @author Roland Schemers
	''' @author Gary Ellison
	''' </summary>

	Public Class ProtectionDomain
		Private Class JavaSecurityAccessImpl
			Implements sun.misc.JavaSecurityAccess

			Private Sub New()
			End Sub

			Public Overrides Function doIntersectionPrivilege(Of T)(ByVal action As PrivilegedAction(Of T), ByVal stack As AccessControlContext, ByVal context As AccessControlContext) As T
				If action Is Nothing Then Throw New NullPointerException

				Return AccessController.doPrivileged(action, getCombinedACC(context, stack))
			End Function

			Public Overrides Function doIntersectionPrivilege(Of T)(ByVal action As PrivilegedAction(Of T), ByVal context As AccessControlContext) As T
				Return doIntersectionPrivilege(action, AccessController.context, context)
			End Function

			Private Shared Function getCombinedACC(ByVal context As AccessControlContext, ByVal stack As AccessControlContext) As AccessControlContext
				Dim acc As New AccessControlContext(context, stack.combiner, True)

				Return (New AccessControlContext(stack.context, acc)).optimize()
			End Function
		End Class

		Shared Sub New()
			' Set up JavaSecurityAccess in SharedSecrets
			sun.misc.SharedSecrets.javaSecurityAccess = New JavaSecurityAccessImpl
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.misc.SharedSecrets.setJavaSecurityProtectionDomainAccess(New sun.misc.JavaSecurityProtectionDomainAccess()
	'		{
	'				public ProtectionDomainCache getProtectionDomainCache()
	'				{
	'					Return New ProtectionDomainCache()
	'					{
	'						private final Map<Key, PermissionCollection> map = Collections.synchronizedMap(New WeakHashMap<Key, PermissionCollection>());
	'						public void put(ProtectionDomain pd, PermissionCollection pc)
	'						{
	'							map.put((pd == Nothing ? Nothing : pd.key), pc);
	'						}
	'						public PermissionCollection get(ProtectionDomain pd)
	'						{
	'							Return pd == Nothing ? map.get(Nothing) : map.get(pd.key);
	'						}
	'					};
	'				}
	'			});
		End Sub

		' CodeSource 
		Private codesource As CodeSource

		' ClassLoader the protection domain was consed from 
		Private classloader As ClassLoader

		' Principals running-as within this protection domain 
		Private principals As Principal()

		' the rights this protection domain is granted 
		Private permissions As PermissionCollection

		' if the permissions object has AllPermission 
		Private hasAllPerm As Boolean = False

	'     the PermissionCollection is static (pre 1.4 constructor)
	'       or dynamic (via a policy refresh) 
		Private staticPermissions As Boolean

	'    
	'     * An object used as a key when the ProtectionDomain is stored in a Map.
	'     
		Friend ReadOnly key As New Key(Me)

		Private Shared ReadOnly debug As sun.security.util.Debug = sun.security.util.Debug.getInstance("domain")

		''' <summary>
		''' Creates a new ProtectionDomain with the given CodeSource and
		''' Permissions. If the permissions object is not null, then
		'''  {@code setReadOnly())} will be called on the passed in
		''' Permissions object. The only permissions granted to this domain
		''' are the ones specified; the current Policy will not be consulted.
		''' </summary>
		''' <param name="codesource"> the codesource associated with this domain </param>
		''' <param name="permissions"> the permissions granted to this domain </param>
		Public Sub New(ByVal codesource As CodeSource, ByVal permissions As PermissionCollection)
			Me.codesource = codesource
			If permissions IsNot Nothing Then
				Me.permissions = permissions
				Me.permissions.readOnlynly()
				If TypeOf permissions Is Permissions AndAlso CType(permissions, Permissions).allPermission IsNot Nothing Then hasAllPerm = True
			End If
			Me.classloader = Nothing
			Me.principals = New Principal(){}
			staticPermissions = True
		End Sub

		''' <summary>
		''' Creates a new ProtectionDomain qualified by the given CodeSource,
		''' Permissions, ClassLoader and array of Principals. If the
		''' permissions object is not null, then {@code setReadOnly()}
		''' will be called on the passed in Permissions object.
		''' The permissions granted to this domain are dynamic; they include
		''' both the static permissions passed to this constructor, and any
		''' permissions granted to this domain by the current Policy at the
		''' time a permission is checked.
		''' <p>
		''' This constructor is typically used by
		''' <seealso cref="SecureClassLoader ClassLoaders"/>
		''' and <seealso cref="DomainCombiner DomainCombiners"/> which delegate to
		''' {@code Policy} to actively associate the permissions granted to
		''' this domain. This constructor affords the
		''' Policy provider the opportunity to augment the supplied
		''' PermissionCollection to reflect policy changes.
		''' <p>
		''' </summary>
		''' <param name="codesource"> the CodeSource associated with this domain </param>
		''' <param name="permissions"> the permissions granted to this domain </param>
		''' <param name="classloader"> the ClassLoader associated with this domain </param>
		''' <param name="principals"> the array of Principals associated with this
		''' domain. The contents of the array are copied to protect against
		''' subsequent modification. </param>
		''' <seealso cref= Policy#refresh </seealso>
		''' <seealso cref= Policy#getPermissions(ProtectionDomain)
		''' @since 1.4 </seealso>
		Public Sub New(ByVal codesource As CodeSource, ByVal permissions As PermissionCollection, ByVal classloader As ClassLoader, ByVal principals As Principal())
			Me.codesource = codesource
			If permissions IsNot Nothing Then
				Me.permissions = permissions
				Me.permissions.readOnlynly()
				If TypeOf permissions Is Permissions AndAlso CType(permissions, Permissions).allPermission IsNot Nothing Then hasAllPerm = True
			End If
			Me.classloader = classloader
			Me.principals = (If(principals IsNot Nothing, principals.clone(), New Principal(){}))
			staticPermissions = False
		End Sub

		''' <summary>
		''' Returns the CodeSource of this domain. </summary>
		''' <returns> the CodeSource of this domain which may be null.
		''' @since 1.2 </returns>
		Public Property codeSource As CodeSource
			Get
				Return Me.codesource
			End Get
		End Property


		''' <summary>
		''' Returns the ClassLoader of this domain. </summary>
		''' <returns> the ClassLoader of this domain which may be null.
		''' 
		''' @since 1.4 </returns>
		Public Property classLoader As ClassLoader
			Get
				Return Me.classloader
			End Get
		End Property


		''' <summary>
		''' Returns an array of principals for this domain. </summary>
		''' <returns> a non-null array of principals for this domain.
		''' Returns a new array each time this method is called.
		''' 
		''' @since 1.4 </returns>
		Public Property principals As Principal()
			Get
				Return Me.principals.clone()
			End Get
		End Property

		''' <summary>
		''' Returns the static permissions granted to this domain.
		''' </summary>
		''' <returns> the static set of permissions for this domain which may be null. </returns>
		''' <seealso cref= Policy#refresh </seealso>
		''' <seealso cref= Policy#getPermissions(ProtectionDomain) </seealso>
		Public Property permissions As PermissionCollection
			Get
				Return permissions
			End Get
		End Property

		''' <summary>
		''' Check and see if this ProtectionDomain implies the permissions
		''' expressed in the Permission object.
		''' <p>
		''' The set of permissions evaluated is a function of whether the
		''' ProtectionDomain was constructed with a static set of permissions
		''' or it was bound to a dynamically mapped set of permissions.
		''' <p>
		''' If the ProtectionDomain was constructed to a
		''' {@link #ProtectionDomain(CodeSource, PermissionCollection)
		''' statically bound} PermissionCollection then the permission will
		''' only be checked against the PermissionCollection supplied at
		''' construction.
		''' <p>
		''' However, if the ProtectionDomain was constructed with
		''' the constructor variant which supports
		''' {@link #ProtectionDomain(CodeSource, PermissionCollection,
		''' ClassLoader, java.security.Principal[]) dynamically binding}
		''' permissions, then the permission will be checked against the
		''' combination of the PermissionCollection supplied at construction and
		''' the current Policy binding.
		''' <p>
		''' </summary>
		''' <param name="permission"> the Permission object to check.
		''' </param>
		''' <returns> true if "permission" is implicit to this ProtectionDomain. </returns>
		Public Overridable Function implies(ByVal permission As Permission) As Boolean

			If hasAllPerm Then Return True

			If (Not staticPermissions) AndAlso Policy.policyNoCheck.implies(Me, permission) Then Return True
			If permissions IsNot Nothing Then Return permissions.implies(permission)

			Return False
		End Function

		' called by the VM -- do not remove
		Friend Overridable Function impliesCreateAccessControlContext() As Boolean
			Return implies(sun.security.util.SecurityConstants.CREATE_ACC_PERMISSION)
		End Function

		''' <summary>
		''' Convert a ProtectionDomain to a String.
		''' </summary>
		Public Overrides Function ToString() As String
			Dim pals As String = "<no principals>"
			If principals IsNot Nothing AndAlso principals.Length > 0 Then
				Dim palBuf As New StringBuilder("(principals ")

				For i As Integer = 0 To principals.Length - 1
					palBuf.append(principals(i).GetType().name & " """ & principals(i).name & """")
					If i < principals.Length-1 Then
						palBuf.append("," & vbLf)
					Else
						palBuf.append(")" & vbLf)
					End If
				Next i
				pals = palBuf.ToString()
			End If

			' Check if policy is set; we don't want to load
			' the policy prematurely here
			Dim pc As PermissionCollection = If(Policy.set AndAlso seeAllp(), mergePermissions(), permissions)

			Return "ProtectionDomain " & " " & codesource & vbLf & " " & classloader & vbLf & " " & pals & vbLf & " " & pc & vbLf
		End Function

		''' <summary>
		''' Return true (merge policy permissions) in the following cases:
		''' 
		''' . SecurityManager is null
		''' 
		''' . SecurityManager is not null,
		'''          debug is not null,
		'''          SecurityManager impelmentation is in bootclasspath,
		'''          Policy implementation is in bootclasspath
		'''          (the bootclasspath restrictions avoid recursion)
		''' 
		''' . SecurityManager is not null,
		'''          debug is null,
		'''          caller has Policy.getPolicy permission
		''' </summary>
		Private Shared Function seeAllp() As Boolean
			Dim sm As SecurityManager = System.securityManager

			If sm Is Nothing Then
				Return True
			Else
				If debug IsNot Nothing Then
					If sm.GetType().classLoader Is Nothing AndAlso Policy.policyNoCheck.GetType().classLoader Is Nothing Then Return True
				Else
					Try
						sm.checkPermission(sun.security.util.SecurityConstants.GET_POLICY_PERMISSION)
						Return True
					Catch se As SecurityException
						' fall thru and return false
					End Try
				End If
			End If

			Return False
		End Function

		Private Function mergePermissions() As PermissionCollection
			If staticPermissions Then Return permissions

			Dim perms As PermissionCollection = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)

			Dim mergedPerms As New Permissions
			Dim swag As Integer = 32
			Dim vcap As Integer = 8
			Dim e As System.Collections.IEnumerator(Of Permission)
			Dim pdVector As IList(Of Permission) = New List(Of Permission)(vcap)
			Dim plVector As IList(Of Permission) = New List(Of Permission)(swag)

			'
			' Build a vector of domain permissions for subsequent merge
			If permissions IsNot Nothing Then
				SyncLock permissions
					e = permissions.elements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Do While e.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						pdVector.Add(e.nextElement())
					Loop
				End SyncLock
			End If

			'
			' Build a vector of Policy permissions for subsequent merge
			If perms IsNot Nothing Then
				SyncLock perms
					e = perms.elements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Do While e.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						plVector.Add(e.nextElement())
						vcap += 1
					Loop
				End SyncLock
			End If

			If perms IsNot Nothing AndAlso permissions IsNot Nothing Then
				'
				' Weed out the duplicates from the policy. Unless a refresh
				' has occurred since the pd was consed this should result in
				' an empty vector.
				SyncLock permissions
					e = permissions.elements() ' domain vs policy
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Do While e.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						Dim pdp As Permission = e.nextElement()
						Dim pdpClass As Class = pdp.GetType()
						Dim pdpActions As String = pdp.actions
						Dim pdpName As String = pdp.name
						For i As Integer = 0 To plVector.Count - 1
							Dim pp As Permission = plVector(i)
							If pdpClass.isInstance(pp) Then
								' The equals() method on some permissions
								' have some side effects so this manual
								' comparison is sufficient.
								If pdpName.Equals(pp.name) AndAlso pdpActions.Equals(pp.actions) Then
									plVector.RemoveAt(i)
									Exit For
								End If
							End If
						Next i
					Loop
				End SyncLock
			End If

			If perms IsNot Nothing Then
				' the order of adding to merged perms and permissions
				' needs to preserve the bugfix 4301064

				For i As Integer = plVector.Count-1 To 0 Step -1
					mergedPerms.add(plVector(i))
				Next i
			End If
			If permissions IsNot Nothing Then
				For i As Integer = pdVector.Count-1 To 0 Step -1
					mergedPerms.add(pdVector(i))
				Next i
			End If

			Return mergedPerms
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedAction(Of T)

			Public Overridable Function run() As PermissionCollection
				Dim p As Policy = Policy.policyNoCheck
				Return p.getPermissions(ProtectionDomain.this)
			End Function
		End Class

		''' <summary>
		''' Used for storing ProtectionDomains as keys in a Map.
		''' </summary>
		Friend NotInheritable Class Key
			Private ReadOnly outerInstance As ProtectionDomain

			Public Sub New(ByVal outerInstance As ProtectionDomain)
				Me.outerInstance = outerInstance
			End Sub

		End Class

	End Class

End Namespace
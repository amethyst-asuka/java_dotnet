Imports System
Imports System.Collections.Generic

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



	''' <summary>
	''' A Policy object is responsible for determining whether code executing
	''' in the Java runtime environment has permission to perform a
	''' security-sensitive operation.
	''' 
	''' <p> There is only one Policy object installed in the runtime at any
	''' given time.  A Policy object can be installed by calling the
	''' {@code setPolicy} method.  The installed Policy object can be
	''' obtained by calling the {@code getPolicy} method.
	''' 
	''' <p> If no Policy object has been installed in the runtime, a call to
	''' {@code getPolicy} installs an instance of the default Policy
	''' implementation (a default subclass implementation of this abstract [Class]).
	''' The default Policy implementation can be changed by setting the value
	''' of the {@code policy.provider} security property to the fully qualified
	''' name of the desired Policy subclass implementation.
	''' 
	''' <p> Application code can directly subclass Policy to provide a custom
	''' implementation.  In addition, an instance of a Policy object can be
	''' constructed by invoking one of the {@code getInstance} factory methods
	''' with a standard type.  The default policy type is "JavaPolicy".
	''' 
	''' <p> Once a Policy instance has been installed (either by default, or by
	''' calling {@code setPolicy}), the Java runtime invokes its
	''' {@code implies} method when it needs to
	''' determine whether executing code (encapsulated in a ProtectionDomain)
	''' can perform SecurityManager-protected operations.  How a Policy object
	''' retrieves its policy data is up to the Policy implementation itself.
	''' The policy data may be stored, for example, in a flat ASCII file,
	''' in a serialized binary file of the Policy [Class], or in a database.
	''' 
	''' <p> The {@code refresh} method causes the policy object to
	''' refresh/reload its data.  This operation is implementation-dependent.
	''' For example, if the policy object stores its data in configuration files,
	''' calling {@code refresh} will cause it to re-read the configuration
	''' policy files.  If a refresh operation is not supported, this method does
	''' nothing.  Note that refreshed policy may not have an effect on classes
	''' in a particular ProtectionDomain. This is dependent on the Policy
	''' provider's implementation of the {@code implies}
	''' method and its PermissionCollection caching strategy.
	''' 
	''' @author Roland Schemers
	''' @author Gary Ellison </summary>
	''' <seealso cref= java.security.Provider </seealso>
	''' <seealso cref= java.security.ProtectionDomain </seealso>
	''' <seealso cref= java.security.Permission </seealso>
	''' <seealso cref= java.security.Security security properties </seealso>

	Public MustInherit Class Policy

		''' <summary>
		''' A read-only empty PermissionCollection instance.
		''' @since 1.6
		''' </summary>
		Public Shared ReadOnly UNSUPPORTED_EMPTY_COLLECTION As PermissionCollection = New UnsupportedEmptyCollection

		' Information about the system-wide policy.
		Private Class PolicyInfo
			' the system-wide policy
			Friend ReadOnly policy_Renamed As Policy
			' a flag indicating if the system-wide policy has been initialized
			Friend ReadOnly initialized As Boolean

			Friend Sub New(ByVal policy_Renamed As Policy, ByVal initialized As Boolean)
				Me.policy_Renamed = policy_Renamed
				Me.initialized = initialized
			End Sub
		End Class

		' PolicyInfo is stored in an AtomicReference
		Private Shared policy_Renamed As New java.util.concurrent.atomic.AtomicReference(Of PolicyInfo)(New PolicyInfo(Nothing, False))

		Private Shared ReadOnly debug As sun.security.util.Debug = sun.security.util.Debug.getInstance("policy")

		' Cache mapping ProtectionDomain.Key to PermissionCollection
		Private pdMapping As java.util.WeakHashMap(Of ProtectionDomain.Key, PermissionCollection)

		''' <summary>
		''' package private for AccessControlContext and ProtectionDomain </summary>
		FriendShared ReadOnly Property[set] As Boolean
			Get
				Dim pi As PolicyInfo = policy_Renamed.get()
				Return pi.policy_Renamed IsNot Nothing AndAlso pi.initialized = True
			End Get
		End Property

		Private Shared Sub checkPermission(ByVal type As String)
			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then sm.checkPermission(New SecurityPermission("createPolicy." & type))
		End Sub

		''' <summary>
		''' Returns the installed Policy object. This value should not be cached,
		''' as it may be changed by a call to {@code setPolicy}.
		''' This method first calls
		''' {@code SecurityManager.checkPermission} with a
		''' {@code SecurityPermission("getPolicy")} permission
		''' to ensure it's ok to get the Policy object.
		''' </summary>
		''' <returns> the installed Policy.
		''' </returns>
		''' <exception cref="SecurityException">
		'''        if a security manager exists and its
		'''        {@code checkPermission} method doesn't allow
		'''        getting the Policy object.
		''' </exception>
		''' <seealso cref= SecurityManager#checkPermission(Permission) </seealso>
		''' <seealso cref= #setPolicy(java.security.Policy) </seealso>
		PublicShared ReadOnly Propertypolicy As Policy
			Get
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPermission(sun.security.util.SecurityConstants.GET_POLICY_PERMISSION)
				Return policyNoCheck
			End Get
		End Property

		''' <summary>
		''' Returns the installed Policy object, skipping the security check.
		''' Used by ProtectionDomain and getPolicy.
		''' </summary>
		''' <returns> the installed Policy. </returns>
		Shared policyNoCheck As Policy
			Get
				Dim pi As PolicyInfo = policy_Renamed.get()
				' Use double-check idiom to avoid locking if system-wide policy is
				' already initialized
				If pi.initialized = False OrElse pi.policy_Renamed Is Nothing Then
					SyncLock GetType(Policy)
						Dim pinfo As PolicyInfo = policy_Renamed.get()
						If pinfo.policy_Renamed Is Nothing Then
							Dim policy_class As String = AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
							If policy_class Is Nothing Then policy_class = "sun.security.provider.PolicyFile"
    
							Try
								pinfo = New PolicyInfo(CType(Type.GetType(policy_class).newInstance(), Policy), True)
							Catch e As Exception
		'                        
		'                         * The policy_class seems to be an extension
		'                         * so we have to bootstrap loading it via a policy
		'                         * provider that is on the bootclasspath.
		'                         * If it loads then shift gears to using the configured
		'                         * provider.
		'                         
    
								' install the bootstrap provider to avoid recursion
								Dim polFile As Policy = New sun.security.provider.PolicyFile
								pinfo = New PolicyInfo(polFile, False)
								policy_Renamed.set(pinfo)
    
								Dim pc As String = policy_class
								Dim pol As Policy = AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper2(Of T)
		'                        
		'                         * if it loaded install it as the policy provider. Otherwise
		'                         * continue to use the system default implementation
		'                         
								If pol IsNot Nothing Then
									pinfo = New PolicyInfo(pol, True)
								Else
									If debug IsNot Nothing Then debug.println("using sun.security.provider.PolicyFile")
									pinfo = New PolicyInfo(polFile, True)
								End If
							End Try
							policy_Renamed.set(pinfo)
						End If
						Return pinfo.policy_Renamed
					End SyncLock
				End If
				Return pi.policy_Renamed
			End Get
		End Property

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedAction(Of T)

			Public Overridable Function run() As String
				Return Security.getProperty("policy.provider")
			End Function
		End Class

		Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
			Implements PrivilegedAction(Of T)

			Public Overridable Function run() As Policy
				Try
					Dim cl As  ClassLoader = ClassLoader.systemClassLoader
					' we want the extension loader
					Dim extcl As  ClassLoader = Nothing
					Do While cl IsNot Nothing
						extcl = cl
						cl = cl.parent
					Loop
					Return (If(extcl IsNot Nothing, CType(Type.GetType(pc, True, extcl).newInstance(), Policy), Nothing))
				Catch e As Exception
					If debug IsNot Nothing Then
						debug.println("policy provider " & pc & " not available")
						e.printStackTrace()
					End If
					Return Nothing
				End Try
			End Function
		End Class

		''' <summary>
		''' Sets the system-wide Policy object. This method first calls
		''' {@code SecurityManager.checkPermission} with a
		''' {@code SecurityPermission("setPolicy")}
		''' permission to ensure it's ok to set the Policy.
		''' </summary>
		''' <param name="p"> the new system Policy object.
		''' </param>
		''' <exception cref="SecurityException">
		'''        if a security manager exists and its
		'''        {@code checkPermission} method doesn't allow
		'''        setting the Policy.
		''' </exception>
		''' <seealso cref= SecurityManager#checkPermission(Permission) </seealso>
		''' <seealso cref= #getPolicy()
		'''  </seealso>
		Public Shared Property policy As Policy
			Set(ByVal p As Policy)
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPermission(New SecurityPermission("setPolicy"))
				If p IsNot Nothing Then initPolicy(p)
				SyncLock GetType(Policy)
					policy_Renamed.set(New PolicyInfo(p, p IsNot Nothing))
				End SyncLock
			End Set
		End Property

		''' <summary>
		''' Initialize superclass state such that a legacy provider can
		''' handle queries for itself.
		''' 
		''' @since 1.4
		''' </summary>
		Private Shared Sub initPolicy(ByVal p As Policy)
	'        
	'         * A policy provider not on the bootclasspath could trigger
	'         * security checks fulfilling a call to either Policy.implies
	'         * or Policy.getPermissions. If this does occur the provider
	'         * must be able to answer for it's own ProtectionDomain
	'         * without triggering additional security checks, otherwise
	'         * the policy implementation will end up in an infinite
	'         * recursion.
	'         *
	'         * To mitigate this, the provider can collect it's own
	'         * ProtectionDomain and associate a PermissionCollection while
	'         * it is being installed. The currently installed policy
	'         * provider (if there is one) will handle calls to
	'         * Policy.implies or Policy.getPermissions during this
	'         * process.
	'         *
	'         * This Policy superclass caches away the ProtectionDomain and
	'         * statically binds permissions so that legacy Policy
	'         * implementations will continue to function.
	'         

			Dim policyDomain As ProtectionDomain = AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper3(Of T)

	'        
	'         * Collect the permissions granted to this protection domain
	'         * so that the provider can be security checked while processing
	'         * calls to Policy.implies or Policy.getPermissions.
	'         
			Dim policyPerms As PermissionCollection = Nothing
			SyncLock p
				If p.pdMapping Is Nothing Then p.pdMapping = New java.util.WeakHashMap(Of )
			End SyncLock

			If policyDomain.codeSource IsNot Nothing Then
				Dim pol As Policy = policy_Renamed.get().policy
				If pol IsNot Nothing Then policyPerms = pol.getPermissions(policyDomain)

				If policyPerms Is Nothing Then ' assume it has all
					policyPerms = New Permissions
					policyPerms.add(sun.security.util.SecurityConstants.ALL_PERMISSION)
				End If

				SyncLock p.pdMapping
					' cache of pd to permissions
					p.pdMapping.put(policyDomain.key, policyPerms)
				End SyncLock
			End If
			Return
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper3(Of T)
			Implements PrivilegedAction(Of T)

			Public Overridable Function run() As ProtectionDomain
				Return p.GetType().protectionDomain
			End Function
		End Class


		''' <summary>
		''' Returns a Policy object of the specified type.
		''' 
		''' <p> This method traverses the list of registered security providers,
		''' starting with the most preferred Provider.
		''' A new Policy object encapsulating the
		''' PolicySpi implementation from the first
		''' Provider that supports the specified type is returned.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="type"> the specified Policy type.  See the Policy section in the
		'''    <a href=
		'''    "{@docRoot}/../technotes/guides/security/StandardNames.html#Policy">
		'''    Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		'''    for a list of standard Policy types.
		''' </param>
		''' <param name="params"> parameters for the Policy, which may be null.
		''' </param>
		''' <returns> the new Policy object.
		''' </returns>
		''' <exception cref="SecurityException"> if the caller does not have permission
		'''          to get a Policy instance for the specified type.
		''' </exception>
		''' <exception cref="NullPointerException"> if the specified type is null.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the specified parameters
		'''          are not understood by the PolicySpi implementation
		'''          from the selected Provider.
		''' </exception>
		''' <exception cref="NoSuchAlgorithmException"> if no Provider supports a PolicySpi
		'''          implementation for the specified type.
		''' </exception>
		''' <seealso cref= Provider
		''' @since 1.6 </seealso>
		Public Shared Function getInstance(ByVal type As String, ByVal params As Policy.Parameters) As Policy

			checkPermission(type)
			Try
				Dim instance_Renamed As sun.security.jca.GetInstance.Instance = sun.security.jca.GetInstance.getInstance("Policy", GetType(PolicySpi), type, params)
				Return New PolicyDelegate(CType(instance_Renamed.impl, PolicySpi), instance_Renamed.provider, type, params)
			Catch nsae As NoSuchAlgorithmException
				Return handleException(nsae)
			End Try
		End Function

		''' <summary>
		''' Returns a Policy object of the specified type.
		''' 
		''' <p> A new Policy object encapsulating the
		''' PolicySpi implementation from the specified provider
		''' is returned.   The specified provider must be registered
		''' in the provider list.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="type"> the specified Policy type.  See the Policy section in the
		'''    <a href=
		'''    "{@docRoot}/../technotes/guides/security/StandardNames.html#Policy">
		'''    Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		'''    for a list of standard Policy types.
		''' </param>
		''' <param name="params"> parameters for the Policy, which may be null.
		''' </param>
		''' <param name="provider"> the provider.
		''' </param>
		''' <returns> the new Policy object.
		''' </returns>
		''' <exception cref="SecurityException"> if the caller does not have permission
		'''          to get a Policy instance for the specified type.
		''' </exception>
		''' <exception cref="NullPointerException"> if the specified type is null.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the specified provider
		'''          is null or empty,
		'''          or if the specified parameters are not understood by
		'''          the PolicySpi implementation from the specified provider.
		''' </exception>
		''' <exception cref="NoSuchProviderException"> if the specified provider is not
		'''          registered in the security provider list.
		''' </exception>
		''' <exception cref="NoSuchAlgorithmException"> if the specified provider does not
		'''          support a PolicySpi implementation for the specified type.
		''' </exception>
		''' <seealso cref= Provider
		''' @since 1.6 </seealso>
		Public Shared Function getInstance(ByVal type As String, ByVal params As Policy.Parameters, ByVal provider_Renamed As String) As Policy

			If provider_Renamed Is Nothing OrElse provider_Renamed.length() = 0 Then Throw New IllegalArgumentException("missing provider")

			checkPermission(type)
			Try
				Dim instance_Renamed As sun.security.jca.GetInstance.Instance = sun.security.jca.GetInstance.getInstance("Policy", GetType(PolicySpi), type, params, provider_Renamed)
				Return New PolicyDelegate(CType(instance_Renamed.impl, PolicySpi), instance_Renamed.provider, type, params)
			Catch nsae As NoSuchAlgorithmException
				Return handleException(nsae)
			End Try
		End Function

		''' <summary>
		''' Returns a Policy object of the specified type.
		''' 
		''' <p> A new Policy object encapsulating the
		''' PolicySpi implementation from the specified Provider
		''' object is returned.  Note that the specified Provider object
		''' does not have to be registered in the provider list.
		''' </summary>
		''' <param name="type"> the specified Policy type.  See the Policy section in the
		'''    <a href=
		'''    "{@docRoot}/../technotes/guides/security/StandardNames.html#Policy">
		'''    Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		'''    for a list of standard Policy types.
		''' </param>
		''' <param name="params"> parameters for the Policy, which may be null.
		''' </param>
		''' <param name="provider"> the Provider.
		''' </param>
		''' <returns> the new Policy object.
		''' </returns>
		''' <exception cref="SecurityException"> if the caller does not have permission
		'''          to get a Policy instance for the specified type.
		''' </exception>
		''' <exception cref="NullPointerException"> if the specified type is null.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the specified Provider is null,
		'''          or if the specified parameters are not understood by
		'''          the PolicySpi implementation from the specified Provider.
		''' </exception>
		''' <exception cref="NoSuchAlgorithmException"> if the specified Provider does not
		'''          support a PolicySpi implementation for the specified type.
		''' </exception>
		''' <seealso cref= Provider
		''' @since 1.6 </seealso>
		Public Shared Function getInstance(ByVal type As String, ByVal params As Policy.Parameters, ByVal provider_Renamed As Provider) As Policy

			If provider_Renamed Is Nothing Then Throw New IllegalArgumentException("missing provider")

			checkPermission(type)
			Try
				Dim instance_Renamed As sun.security.jca.GetInstance.Instance = sun.security.jca.GetInstance.getInstance("Policy", GetType(PolicySpi), type, params, provider_Renamed)
				Return New PolicyDelegate(CType(instance_Renamed.impl, PolicySpi), instance_Renamed.provider, type, params)
			Catch nsae As NoSuchAlgorithmException
				Return handleException(nsae)
			End Try
		End Function

		Private Shared Function handleException(ByVal nsae As NoSuchAlgorithmException) As Policy
			Dim cause As Throwable = nsae.InnerException
			If TypeOf cause Is IllegalArgumentException Then Throw CType(cause, IllegalArgumentException)
			Throw nsae
		End Function

		''' <summary>
		''' Return the Provider of this Policy.
		''' 
		''' <p> This Policy instance will only have a Provider if it
		''' was obtained via a call to {@code Policy.getInstance}.
		''' Otherwise this method returns null.
		''' </summary>
		''' <returns> the Provider of this Policy, or null.
		''' 
		''' @since 1.6 </returns>
		Public Overridable Property provider As Provider
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Return the type of this Policy.
		''' 
		''' <p> This Policy instance will only have a type if it
		''' was obtained via a call to {@code Policy.getInstance}.
		''' Otherwise this method returns null.
		''' </summary>
		''' <returns> the type of this Policy, or null.
		''' 
		''' @since 1.6 </returns>
		Public Overridable Property type As String
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Return Policy parameters.
		''' 
		''' <p> This Policy instance will only have parameters if it
		''' was obtained via a call to {@code Policy.getInstance}.
		''' Otherwise this method returns null.
		''' </summary>
		''' <returns> Policy parameters, or null.
		''' 
		''' @since 1.6 </returns>
		Public Overridable Property parameters As Policy.Parameters
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Return a PermissionCollection object containing the set of
		''' permissions granted to the specified CodeSource.
		''' 
		''' <p> Applications are discouraged from calling this method
		''' since this operation may not be supported by all policy implementations.
		''' Applications should solely rely on the {@code implies} method
		''' to perform policy checks.  If an application absolutely must call
		''' a getPermissions method, it should call
		''' {@code getPermissions(ProtectionDomain)}.
		''' 
		''' <p> The default implementation of this method returns
		''' Policy.UNSUPPORTED_EMPTY_COLLECTION.  This method can be
		''' overridden if the policy implementation can return a set of
		''' permissions granted to a CodeSource.
		''' </summary>
		''' <param name="codesource"> the CodeSource to which the returned
		'''          PermissionCollection has been granted.
		''' </param>
		''' <returns> a set of permissions granted to the specified CodeSource.
		'''          If this operation is supported, the returned
		'''          set of permissions must be a new mutable instance
		'''          and it must support heterogeneous Permission types.
		'''          If this operation is not supported,
		'''          Policy.UNSUPPORTED_EMPTY_COLLECTION is returned. </returns>
		Public Overridable Function getPermissions(ByVal codesource As CodeSource) As PermissionCollection
			Return Policy.UNSUPPORTED_EMPTY_COLLECTION
		End Function

		''' <summary>
		''' Return a PermissionCollection object containing the set of
		''' permissions granted to the specified ProtectionDomain.
		''' 
		''' <p> Applications are discouraged from calling this method
		''' since this operation may not be supported by all policy implementations.
		''' Applications should rely on the {@code implies} method
		''' to perform policy checks.
		''' 
		''' <p> The default implementation of this method first retrieves
		''' the permissions returned via {@code getPermissions(CodeSource)}
		''' (the CodeSource is taken from the specified ProtectionDomain),
		''' as well as the permissions located inside the specified ProtectionDomain.
		''' All of these permissions are then combined and returned in a new
		''' PermissionCollection object.  If {@code getPermissions(CodeSource)}
		''' returns Policy.UNSUPPORTED_EMPTY_COLLECTION, then this method
		''' returns the permissions contained inside the specified ProtectionDomain
		''' in a new PermissionCollection object.
		''' 
		''' <p> This method can be overridden if the policy implementation
		''' supports returning a set of permissions granted to a ProtectionDomain.
		''' </summary>
		''' <param name="domain"> the ProtectionDomain to which the returned
		'''          PermissionCollection has been granted.
		''' </param>
		''' <returns> a set of permissions granted to the specified ProtectionDomain.
		'''          If this operation is supported, the returned
		'''          set of permissions must be a new mutable instance
		'''          and it must support heterogeneous Permission types.
		'''          If this operation is not supported,
		'''          Policy.UNSUPPORTED_EMPTY_COLLECTION is returned.
		''' 
		''' @since 1.4 </returns>
		Public Overridable Function getPermissions(ByVal domain As ProtectionDomain) As PermissionCollection
			Dim pc As PermissionCollection = Nothing

			If domain Is Nothing Then Return New Permissions

			If pdMapping Is Nothing Then initPolicy(Me)

			SyncLock pdMapping
				pc = pdMapping.get(domain.key)
			End SyncLock

			If pc IsNot Nothing Then
				Dim perms As New Permissions
				SyncLock pc
					Dim e As System.Collections.IEnumerator(Of Permission) = pc.elements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Do While e.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						perms.add(e.nextElement())
					Loop
				End SyncLock
				Return perms
			End If

			pc = getPermissions(domain.codeSource)
			If pc Is Nothing OrElse pc Is UNSUPPORTED_EMPTY_COLLECTION Then pc = New Permissions

			addStaticPerms(pc, domain.permissions)
			Return pc
		End Function

		''' <summary>
		''' add static permissions to provided permission collection
		''' </summary>
		Private Sub addStaticPerms(ByVal perms As PermissionCollection, ByVal statics As PermissionCollection)
			If statics IsNot Nothing Then
				SyncLock statics
					Dim e As System.Collections.IEnumerator(Of Permission) = statics.elements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Do While e.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						perms.add(e.nextElement())
					Loop
				End SyncLock
			End If
		End Sub

		''' <summary>
		''' Evaluates the global policy for the permissions granted to
		''' the ProtectionDomain and tests whether the permission is
		''' granted.
		''' </summary>
		''' <param name="domain"> the ProtectionDomain to test </param>
		''' <param name="permission"> the Permission object to be tested for implication.
		''' </param>
		''' <returns> true if "permission" is a proper subset of a permission
		''' granted to this ProtectionDomain.
		''' </returns>
		''' <seealso cref= java.security.ProtectionDomain
		''' @since 1.4 </seealso>
		Public Overridable Function implies(ByVal domain As ProtectionDomain, ByVal permission As Permission) As Boolean
			Dim pc As PermissionCollection

			If pdMapping Is Nothing Then initPolicy(Me)

			SyncLock pdMapping
				pc = pdMapping.get(domain.key)
			End SyncLock

			If pc IsNot Nothing Then Return pc.implies(permission)

			pc = getPermissions(domain)
			If pc Is Nothing Then Return False

			SyncLock pdMapping
				' cache it
				pdMapping.put(domain.key, pc)
			End SyncLock

			Return pc.implies(permission)
		End Function

		''' <summary>
		''' Refreshes/reloads the policy configuration. The behavior of this method
		''' depends on the implementation. For example, calling {@code refresh}
		''' on a file-based policy will cause the file to be re-read.
		''' 
		''' <p> The default implementation of this method does nothing.
		''' This method should be overridden if a refresh operation is supported
		''' by the policy implementation.
		''' </summary>
		Public Overridable Sub refresh()
		End Sub

		''' <summary>
		''' This subclass is returned by the getInstance calls.  All Policy calls
		''' are delegated to the underlying PolicySpi.
		''' </summary>
		Private Class PolicyDelegate
			Inherits Policy

			Private spi As PolicySpi
			Private p As Provider
			Private type As String
			Private params As Policy.Parameters

			Private Sub New(ByVal spi As PolicySpi, ByVal p As Provider, ByVal type As String, ByVal params As Policy.Parameters)
				Me.spi = spi
				Me.p = p
				Me.type = type
				Me.params = params
			End Sub

			Public  Overrides ReadOnly Property  type As String
				Get
					Return type
				End Get
			End Property
			Public  Overrides ReadOnly Property  parameters As Policy.Parameters
				Get
					Return params
				End Get
			End Property
			Public  Overrides ReadOnly Property  provider As Provider
				Get
					Return p
				End Get
			End Property
			Public Overrides Function getPermissions(ByVal codesource As CodeSource) As PermissionCollection
				Return spi.engineGetPermissions(codesource)
			End Function
			Public Overrides Function getPermissions(ByVal domain As ProtectionDomain) As PermissionCollection
				Return spi.engineGetPermissions(domain)
			End Function
			Public Overrides Function implies(ByVal domain As ProtectionDomain, ByVal perm As Permission) As Boolean
				Return spi.engineImplies(domain, perm)
			End Function
			Public Overrides Sub refresh()
				spi.engineRefresh()
			End Sub
		End Class

		''' <summary>
		''' This represents a marker interface for Policy parameters.
		''' 
		''' @since 1.6
		''' </summary>
		Public Interface Parameters
		End Interface

		''' <summary>
		''' This class represents a read-only empty PermissionCollection object that
		''' is returned from the {@code getPermissions(CodeSource)} and
		''' {@code getPermissions(ProtectionDomain)}
		''' methods in the Policy class when those operations are not
		''' supported by the Policy implementation.
		''' </summary>
		Private Class UnsupportedEmptyCollection
			Inherits PermissionCollection

			Private Const serialVersionUID As Long = -8492269157353014774L

			Private perms As Permissions

			''' <summary>
			''' Create a read-only empty PermissionCollection object.
			''' </summary>
			Public Sub New()
				Me.perms = New Permissions
				perms.readOnlynly()
			End Sub

			''' <summary>
			''' Adds a permission object to the current collection of permission
			''' objects.
			''' </summary>
			''' <param name="permission"> the Permission object to add.
			''' </param>
			''' <exception cref="SecurityException"> - if this PermissionCollection object
			'''                                has been marked readonly </exception>
			Public Overrides Sub add(ByVal permission As Permission)
				perms.add(permission)
			End Sub

			''' <summary>
			''' Checks to see if the specified permission is implied by the
			''' collection of Permission objects held in this PermissionCollection.
			''' </summary>
			''' <param name="permission"> the Permission object to compare.
			''' </param>
			''' <returns> true if "permission" is implied by the permissions in
			''' the collection, false if not. </returns>
			Public Overrides Function implies(ByVal permission As Permission) As Boolean
				Return perms.implies(permission)
			End Function

			''' <summary>
			''' Returns an enumeration of all the Permission objects in the
			''' collection.
			''' </summary>
			''' <returns> an enumeration of all the Permissions. </returns>
			Public Overrides Function elements() As System.Collections.IEnumerator(Of Permission)
				Return perms.elements()
			End Function
		End Class
	End Class

End Namespace
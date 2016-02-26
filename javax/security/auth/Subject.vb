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

Namespace javax.security.auth


	''' <summary>
	''' <p> A {@code Subject} represents a grouping of related information
	''' for a single entity, such as a person.
	''' Such information includes the Subject's identities as well as
	''' its security-related attributes
	''' (passwords and cryptographic keys, for example).
	''' 
	''' <p> Subjects may potentially have multiple identities.
	''' Each identity is represented as a {@code Principal}
	''' within the {@code Subject}.  Principals simply bind names to a
	''' {@code Subject}.  For example, a {@code Subject} that happens
	''' to be a person, Alice, might have two Principals:
	''' one which binds "Alice Bar", the name on her driver license,
	''' to the {@code Subject}, and another which binds,
	''' "999-99-9999", the number on her student identification card,
	''' to the {@code Subject}.  Both Principals refer to the same
	''' {@code Subject} even though each has a different name.
	''' 
	''' <p> A {@code Subject} may also own security-related attributes,
	''' which are referred to as credentials.
	''' Sensitive credentials that require special protection, such as
	''' private cryptographic keys, are stored within a private credential
	''' {@code Set}.  Credentials intended to be shared, such as
	''' public key certificates or Kerberos server tickets are stored
	''' within a public credential {@code Set}.  Different permissions
	''' are required to access and modify the different credential Sets.
	''' 
	''' <p> To retrieve all the Principals associated with a {@code Subject},
	''' invoke the {@code getPrincipals} method.  To retrieve
	''' all the public or private credentials belonging to a {@code Subject},
	''' invoke the {@code getPublicCredentials} method or
	''' {@code getPrivateCredentials} method, respectively.
	''' To modify the returned {@code Set} of Principals and credentials,
	''' use the methods defined in the {@code Set} class.
	''' For example:
	''' <pre>
	'''      Subject subject;
	'''      Principal principal;
	'''      Object credential;
	''' 
	'''      // add a Principal and credential to the Subject
	'''      subject.getPrincipals().add(principal);
	'''      subject.getPublicCredentials().add(credential);
	''' </pre>
	''' 
	''' <p> This {@code Subject} class implements {@code Serializable}.
	''' While the Principals associated with the {@code Subject} are serialized,
	''' the credentials associated with the {@code Subject} are not.
	''' Note that the {@code java.security.Principal} class
	''' does not implement {@code Serializable}.  Therefore all concrete
	''' {@code Principal} implementations associated with Subjects
	''' must implement {@code Serializable}.
	''' </summary>
	''' <seealso cref= java.security.Principal </seealso>
	''' <seealso cref= java.security.DomainCombiner </seealso>
	<Serializable> _
	Public NotInheritable Class Subject

		Private Const serialVersionUID As Long = -8308522755600156056L

		''' <summary>
		''' A {@code Set} that provides a view of all of this
		''' Subject's Principals
		''' 
		''' <p>
		''' 
		''' @serial Each element in this set is a
		'''          {@code java.security.Principal}.
		'''          The set is a {@code Subject.SecureSet}.
		''' </summary>
		Friend principals As [Set](Of java.security.Principal)

		''' <summary>
		''' Sets that provide a view of all of this
		''' Subject's Credentials
		''' </summary>
		<NonSerialized> _
		Friend pubCredentials As [Set](Of Object)
		<NonSerialized> _
		Friend privCredentials As [Set](Of Object)

		''' <summary>
		''' Whether this Subject is read-only
		''' 
		''' @serial
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private [readOnly] As Boolean = False

		Private Const PRINCIPAL_SET As Integer = 1
		Private Const PUB_CREDENTIAL_SET As Integer = 2
		Private Const PRIV_CREDENTIAL_SET As Integer = 3

		Private Shared ReadOnly NULL_PD_ARRAY As java.security.ProtectionDomain() = New java.security.ProtectionDomain(){}

		''' <summary>
		''' Create an instance of a {@code Subject}
		''' with an empty {@code Set} of Principals and empty
		''' Sets of public and private credentials.
		''' 
		''' <p> The newly constructed Sets check whether this {@code Subject}
		''' has been set read-only before permitting subsequent modifications.
		''' The newly created Sets also prevent illegal modifications
		''' by ensuring that callers have sufficient permissions.
		''' 
		''' <p> To modify the Principals Set, the caller must have
		''' {@code AuthPermission("modifyPrincipals")}.
		''' To modify the public credential Set, the caller must have
		''' {@code AuthPermission("modifyPublicCredentials")}.
		''' To modify the private credential Set, the caller must have
		''' {@code AuthPermission("modifyPrivateCredentials")}.
		''' </summary>
		Public Sub New()

			Me.principals = Collections.synchronizedSet(New SecureSet(Of java.security.Principal)(Me, PRINCIPAL_SET))
			Me.pubCredentials = Collections.synchronizedSet(New SecureSet(Of Object)(Me, PUB_CREDENTIAL_SET))
			Me.privCredentials = Collections.synchronizedSet(New SecureSet(Of Object)(Me, PRIV_CREDENTIAL_SET))
		End Sub

		''' <summary>
		''' Create an instance of a {@code Subject} with
		''' Principals and credentials.
		''' 
		''' <p> The Principals and credentials from the specified Sets
		''' are copied into newly constructed Sets.
		''' These newly created Sets check whether this {@code Subject}
		''' has been set read-only before permitting subsequent modifications.
		''' The newly created Sets also prevent illegal modifications
		''' by ensuring that callers have sufficient permissions.
		''' 
		''' <p> To modify the Principals Set, the caller must have
		''' {@code AuthPermission("modifyPrincipals")}.
		''' To modify the public credential Set, the caller must have
		''' {@code AuthPermission("modifyPublicCredentials")}.
		''' To modify the private credential Set, the caller must have
		''' {@code AuthPermission("modifyPrivateCredentials")}.
		''' <p>
		''' </summary>
		''' <param name="readOnly"> true if the {@code Subject} is to be read-only,
		'''          and false otherwise. <p>
		''' </param>
		''' <param name="principals"> the {@code Set} of Principals
		'''          to be associated with this {@code Subject}. <p>
		''' </param>
		''' <param name="pubCredentials"> the {@code Set} of public credentials
		'''          to be associated with this {@code Subject}. <p>
		''' </param>
		''' <param name="privCredentials"> the {@code Set} of private credentials
		'''          to be associated with this {@code Subject}.
		''' </param>
		''' <exception cref="NullPointerException"> if the specified
		'''          {@code principals}, {@code pubCredentials},
		'''          or {@code privCredentials} are {@code null}. </exception>
		Public Sub New(Of T1 As java.security.Principal, T2, T3)(ByVal [readOnly] As Boolean, ByVal principals As [Set](Of T1), ByVal pubCredentials As [Set](Of T2), ByVal privCredentials As [Set](Of T3))

			If principals Is Nothing OrElse pubCredentials Is Nothing OrElse privCredentials Is Nothing Then Throw New NullPointerException(sun.security.util.ResourcesMgr.getString("invalid.null.input.s."))

			Me.principals = Collections.synchronizedSet(New SecureSet(Of java.security.Principal) (Me, PRINCIPAL_SET, principals))
			Me.pubCredentials = Collections.synchronizedSet(New SecureSet(Of Object) (Me, PUB_CREDENTIAL_SET, pubCredentials))
			Me.privCredentials = Collections.synchronizedSet(New SecureSet(Of Object) (Me, PRIV_CREDENTIAL_SET, privCredentials))
			Me.readOnly = [readOnly]
		End Sub

		''' <summary>
		''' Set this {@code Subject} to be read-only.
		''' 
		''' <p> Modifications (additions and removals) to this Subject's
		''' {@code Principal} {@code Set} and
		''' credential Sets will be disallowed.
		''' The {@code destroy} operation on this Subject's credentials will
		''' still be permitted.
		''' 
		''' <p> Subsequent attempts to modify the Subject's {@code Principal}
		''' and credential Sets will result in an
		''' {@code IllegalStateException} being thrown.
		''' Also, once a {@code Subject} is read-only,
		''' it can not be reset to being writable again.
		''' 
		''' <p>
		''' </summary>
		''' <exception cref="SecurityException"> if the caller does not have permission
		'''          to set this {@code Subject} to be read-only. </exception>
		Public Sub setReadOnly()
			Dim sm As java.lang.SecurityManager = System.securityManager
			If sm IsNot Nothing Then sm.checkPermission(AuthPermissionHolder.SET_READ_ONLY_PERMISSION)

			Me.readOnly = True
		End Sub

		''' <summary>
		''' Query whether this {@code Subject} is read-only.
		''' 
		''' <p>
		''' </summary>
		''' <returns> true if this {@code Subject} is read-only, false otherwise. </returns>
		Public Property [readOnly] As Boolean
			Get
				Return Me.readOnly
			End Get
		End Property

		''' <summary>
		''' Get the {@code Subject} associated with the provided
		''' {@code AccessControlContext}.
		''' 
		''' <p> The {@code AccessControlContext} may contain many
		''' Subjects (from nested {@code doAs} calls).
		''' In this situation, the most recent {@code Subject} associated
		''' with the {@code AccessControlContext} is returned.
		''' 
		''' <p>
		''' </summary>
		''' <param name="acc"> the {@code AccessControlContext} from which to retrieve
		'''          the {@code Subject}.
		''' </param>
		''' <returns>  the {@code Subject} associated with the provided
		'''          {@code AccessControlContext}, or {@code null}
		'''          if no {@code Subject} is associated
		'''          with the provided {@code AccessControlContext}.
		''' </returns>
		''' <exception cref="SecurityException"> if the caller does not have permission
		'''          to get the {@code Subject}. <p>
		''' </exception>
		''' <exception cref="NullPointerException"> if the provided
		'''          {@code AccessControlContext} is {@code null}. </exception>
		Public Shared Function getSubject(ByVal acc As java.security.AccessControlContext) As Subject

			Dim sm As java.lang.SecurityManager = System.securityManager
			If sm IsNot Nothing Then sm.checkPermission(AuthPermissionHolder.GET_SUBJECT_PERMISSION)

			If acc Is Nothing Then Throw New NullPointerException(sun.security.util.ResourcesMgr.getString("invalid.null.AccessControlContext.provided"))

			' return the Subject from the DomainCombiner of the provided context
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Return java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<Subject>()
	'		{
	'			public Subject run()
	'			{
	'				DomainCombiner dc = acc.getDomainCombiner();
	'				if (!(dc instanceof SubjectDomainCombiner))
	'					Return Nothing;
	'				SubjectDomainCombiner sdc = (SubjectDomainCombiner)dc;
	'				Return sdc.getSubject();
	'			}
	'		});
		End Function

		''' <summary>
		''' Perform work as a particular {@code Subject}.
		''' 
		''' <p> This method first retrieves the current Thread's
		''' {@code AccessControlContext} via
		''' {@code AccessController.getContext},
		''' and then instantiates a new {@code AccessControlContext}
		''' using the retrieved context along with a new
		''' {@code SubjectDomainCombiner} (constructed using
		''' the provided {@code Subject}).
		''' Finally, this method invokes {@code AccessController.doPrivileged},
		''' passing it the provided {@code PrivilegedAction},
		''' as well as the newly constructed {@code AccessControlContext}.
		''' 
		''' <p>
		''' </summary>
		''' <param name="subject"> the {@code Subject} that the specified
		'''                  {@code action} will run as.  This parameter
		'''                  may be {@code null}. <p>
		''' </param>
		''' @param <T> the type of the value returned by the PrivilegedAction's
		'''                  {@code run} method.
		''' </param>
		''' <param name="action"> the code to be run as the specified
		'''                  {@code Subject}. <p>
		''' </param>
		''' <returns> the value returned by the PrivilegedAction's
		'''                  {@code run} method.
		''' </returns>
		''' <exception cref="NullPointerException"> if the {@code PrivilegedAction}
		'''                  is {@code null}. <p>
		''' </exception>
		''' <exception cref="SecurityException"> if the caller does not have permission
		'''                  to invoke this method. </exception>
		Public Shared Function doAs(Of T)(ByVal ___subject As Subject, ByVal action As java.security.PrivilegedAction(Of T)) As T

			Dim sm As java.lang.SecurityManager = System.securityManager
			If sm IsNot Nothing Then sm.checkPermission(AuthPermissionHolder.DO_AS_PERMISSION)
			If action Is Nothing Then Throw New NullPointerException(sun.security.util.ResourcesMgr.getString("invalid.null.action.provided"))

			' set up the new Subject-based AccessControlContext
			' for doPrivileged
			Dim currentAcc As java.security.AccessControlContext = java.security.AccessController.context

			' call doPrivileged and push this new context on the stack
			Return java.security.AccessController.doPrivileged(action, createContext(___subject, currentAcc))
		End Function

		''' <summary>
		''' Perform work as a particular {@code Subject}.
		''' 
		''' <p> This method first retrieves the current Thread's
		''' {@code AccessControlContext} via
		''' {@code AccessController.getContext},
		''' and then instantiates a new {@code AccessControlContext}
		''' using the retrieved context along with a new
		''' {@code SubjectDomainCombiner} (constructed using
		''' the provided {@code Subject}).
		''' Finally, this method invokes {@code AccessController.doPrivileged},
		''' passing it the provided {@code PrivilegedExceptionAction},
		''' as well as the newly constructed {@code AccessControlContext}.
		''' 
		''' <p>
		''' </summary>
		''' <param name="subject"> the {@code Subject} that the specified
		'''                  {@code action} will run as.  This parameter
		'''                  may be {@code null}. <p>
		''' </param>
		''' @param <T> the type of the value returned by the
		'''                  PrivilegedExceptionAction's {@code run} method.
		''' </param>
		''' <param name="action"> the code to be run as the specified
		'''                  {@code Subject}. <p>
		''' </param>
		''' <returns> the value returned by the
		'''                  PrivilegedExceptionAction's {@code run} method.
		''' </returns>
		''' <exception cref="PrivilegedActionException"> if the
		'''                  {@code PrivilegedExceptionAction.run}
		'''                  method throws a checked exception. <p>
		''' </exception>
		''' <exception cref="NullPointerException"> if the specified
		'''                  {@code PrivilegedExceptionAction} is
		'''                  {@code null}. <p>
		''' </exception>
		''' <exception cref="SecurityException"> if the caller does not have permission
		'''                  to invoke this method. </exception>
		Public Shared Function doAs(Of T)(ByVal ___subject As Subject, ByVal action As java.security.PrivilegedExceptionAction(Of T)) As T

			Dim sm As java.lang.SecurityManager = System.securityManager
			If sm IsNot Nothing Then sm.checkPermission(AuthPermissionHolder.DO_AS_PERMISSION)

			If action Is Nothing Then Throw New NullPointerException(sun.security.util.ResourcesMgr.getString("invalid.null.action.provided"))

			' set up the new Subject-based AccessControlContext for doPrivileged
			Dim currentAcc As java.security.AccessControlContext = java.security.AccessController.context

			' call doPrivileged and push this new context on the stack
			Return java.security.AccessController.doPrivileged(action, createContext(___subject, currentAcc))
		End Function

		''' <summary>
		''' Perform privileged work as a particular {@code Subject}.
		''' 
		''' <p> This method behaves exactly as {@code Subject.doAs},
		''' except that instead of retrieving the current Thread's
		''' {@code AccessControlContext}, it uses the provided
		''' {@code AccessControlContext}.  If the provided
		''' {@code AccessControlContext} is {@code null},
		''' this method instantiates a new {@code AccessControlContext}
		''' with an empty collection of ProtectionDomains.
		''' 
		''' <p>
		''' </summary>
		''' <param name="subject"> the {@code Subject} that the specified
		'''                  {@code action} will run as.  This parameter
		'''                  may be {@code null}. <p>
		''' </param>
		''' @param <T> the type of the value returned by the PrivilegedAction's
		'''                  {@code run} method.
		''' </param>
		''' <param name="action"> the code to be run as the specified
		'''                  {@code Subject}. <p>
		''' </param>
		''' <param name="acc"> the {@code AccessControlContext} to be tied to the
		'''                  specified <i>subject</i> and <i>action</i>. <p>
		''' </param>
		''' <returns> the value returned by the PrivilegedAction's
		'''                  {@code run} method.
		''' </returns>
		''' <exception cref="NullPointerException"> if the {@code PrivilegedAction}
		'''                  is {@code null}. <p>
		''' </exception>
		''' <exception cref="SecurityException"> if the caller does not have permission
		'''                  to invoke this method. </exception>
		Public Shared Function doAsPrivileged(Of T)(ByVal ___subject As Subject, ByVal action As java.security.PrivilegedAction(Of T), ByVal acc As java.security.AccessControlContext) As T

			Dim sm As java.lang.SecurityManager = System.securityManager
			If sm IsNot Nothing Then sm.checkPermission(AuthPermissionHolder.DO_AS_PRIVILEGED_PERMISSION)

			If action Is Nothing Then Throw New NullPointerException(sun.security.util.ResourcesMgr.getString("invalid.null.action.provided"))

			' set up the new Subject-based AccessControlContext
			' for doPrivileged
			Dim callerAcc As java.security.AccessControlContext = (If(acc Is Nothing, New java.security.AccessControlContext(NULL_PD_ARRAY), acc))

			' call doPrivileged and push this new context on the stack
			Return java.security.AccessController.doPrivileged(action, createContext(___subject, callerAcc))
		End Function

		''' <summary>
		''' Perform privileged work as a particular {@code Subject}.
		''' 
		''' <p> This method behaves exactly as {@code Subject.doAs},
		''' except that instead of retrieving the current Thread's
		''' {@code AccessControlContext}, it uses the provided
		''' {@code AccessControlContext}.  If the provided
		''' {@code AccessControlContext} is {@code null},
		''' this method instantiates a new {@code AccessControlContext}
		''' with an empty collection of ProtectionDomains.
		''' 
		''' <p>
		''' </summary>
		''' <param name="subject"> the {@code Subject} that the specified
		'''                  {@code action} will run as.  This parameter
		'''                  may be {@code null}. <p>
		''' </param>
		''' @param <T> the type of the value returned by the
		'''                  PrivilegedExceptionAction's {@code run} method.
		''' </param>
		''' <param name="action"> the code to be run as the specified
		'''                  {@code Subject}. <p>
		''' </param>
		''' <param name="acc"> the {@code AccessControlContext} to be tied to the
		'''                  specified <i>subject</i> and <i>action</i>. <p>
		''' </param>
		''' <returns> the value returned by the
		'''                  PrivilegedExceptionAction's {@code run} method.
		''' </returns>
		''' <exception cref="PrivilegedActionException"> if the
		'''                  {@code PrivilegedExceptionAction.run}
		'''                  method throws a checked exception. <p>
		''' </exception>
		''' <exception cref="NullPointerException"> if the specified
		'''                  {@code PrivilegedExceptionAction} is
		'''                  {@code null}. <p>
		''' </exception>
		''' <exception cref="SecurityException"> if the caller does not have permission
		'''                  to invoke this method. </exception>
		Public Shared Function doAsPrivileged(Of T)(ByVal ___subject As Subject, ByVal action As java.security.PrivilegedExceptionAction(Of T), ByVal acc As java.security.AccessControlContext) As T

			Dim sm As java.lang.SecurityManager = System.securityManager
			If sm IsNot Nothing Then sm.checkPermission(AuthPermissionHolder.DO_AS_PRIVILEGED_PERMISSION)

			If action Is Nothing Then Throw New NullPointerException(sun.security.util.ResourcesMgr.getString("invalid.null.action.provided"))

			' set up the new Subject-based AccessControlContext for doPrivileged
			Dim callerAcc As java.security.AccessControlContext = (If(acc Is Nothing, New java.security.AccessControlContext(NULL_PD_ARRAY), acc))

			' call doPrivileged and push this new context on the stack
			Return java.security.AccessController.doPrivileged(action, createContext(___subject, callerAcc))
		End Function

		Private Shared Function createContext(ByVal ___subject As Subject, ByVal acc As java.security.AccessControlContext) As java.security.AccessControlContext


'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Return java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<java.security.AccessControlContext>()
	'		{
	'			public AccessControlContext run()
	'			{
	'				if (subject == Nothing)
	'					Return New AccessControlContext(acc, Nothing);
	'				else
	'					Return New AccessControlContext(acc, New SubjectDomainCombiner(subject));
	'			}
	'		});
		End Function

		''' <summary>
		''' Return the {@code Set} of Principals associated with this
		''' {@code Subject}.  Each {@code Principal} represents
		''' an identity for this {@code Subject}.
		''' 
		''' <p> The returned {@code Set} is backed by this Subject's
		''' internal {@code Principal} {@code Set}.  Any modification
		''' to the returned {@code Set} affects the internal
		''' {@code Principal} {@code Set} as well.
		''' 
		''' <p>
		''' </summary>
		''' <returns>  The {@code Set} of Principals associated with this
		'''          {@code Subject}. </returns>
		Public Property principals As [Set](Of java.security.Principal)
			Get
    
				' always return an empty Set instead of null
				' so LoginModules can add to the Set if necessary
				Return principals
			End Get
		End Property

		''' <summary>
		''' Return a {@code Set} of Principals associated with this
		''' {@code Subject} that are instances or subclasses of the specified
		''' {@code Class}.
		''' 
		''' <p> The returned {@code Set} is not backed by this Subject's
		''' internal {@code Principal} {@code Set}.  A new
		''' {@code Set} is created and returned for each method invocation.
		''' Modifications to the returned {@code Set}
		''' will not affect the internal {@code Principal} {@code Set}.
		''' 
		''' <p>
		''' </summary>
		''' @param <T> the type of the class modeled by {@code c}
		''' </param>
		''' <param name="c"> the returned {@code Set} of Principals will all be
		'''          instances of this class.
		''' </param>
		''' <returns> a {@code Set} of Principals that are instances of the
		'''          specified {@code Class}.
		''' </returns>
		''' <exception cref="NullPointerException"> if the specified {@code Class}
		'''                  is {@code null}. </exception>
		Public Function getPrincipals(Of T As java.security.Principal)(ByVal c As Type) As [Set](Of T)

			If c Is Nothing Then Throw New NullPointerException(sun.security.util.ResourcesMgr.getString("invalid.null.Class.provided"))

			' always return an empty Set instead of null
			' so LoginModules can add to the Set if necessary
			Return New ClassSet(Me, Of T)(PRINCIPAL_SET, c)
		End Function

		''' <summary>
		''' Return the {@code Set} of public credentials held by this
		''' {@code Subject}.
		''' 
		''' <p> The returned {@code Set} is backed by this Subject's
		''' internal public Credential {@code Set}.  Any modification
		''' to the returned {@code Set} affects the internal public
		''' Credential {@code Set} as well.
		''' 
		''' <p>
		''' </summary>
		''' <returns>  A {@code Set} of public credentials held by this
		'''          {@code Subject}. </returns>
		Public Property publicCredentials As [Set](Of Object)
			Get
    
				' always return an empty Set instead of null
				' so LoginModules can add to the Set if necessary
				Return pubCredentials
			End Get
		End Property

		''' <summary>
		''' Return the {@code Set} of private credentials held by this
		''' {@code Subject}.
		''' 
		''' <p> The returned {@code Set} is backed by this Subject's
		''' internal private Credential {@code Set}.  Any modification
		''' to the returned {@code Set} affects the internal private
		''' Credential {@code Set} as well.
		''' 
		''' <p> A caller requires permissions to access the Credentials
		''' in the returned {@code Set}, or to modify the
		''' {@code Set} itself.  A {@code SecurityException}
		''' is thrown if the caller does not have the proper permissions.
		''' 
		''' <p> While iterating through the {@code Set},
		''' a {@code SecurityException} is thrown
		''' if the caller does not have permission to access a
		''' particular Credential.  The {@code Iterator}
		''' is nevertheless advanced to next element in the {@code Set}.
		''' 
		''' <p>
		''' </summary>
		''' <returns>  A {@code Set} of private credentials held by this
		'''          {@code Subject}. </returns>
		Public Property privateCredentials As [Set](Of Object)
			Get
    
				' XXX
				' we do not need a security check for
				' AuthPermission(getPrivateCredentials)
				' because we already restrict access to private credentials
				' via the PrivateCredentialPermission.  all the extra AuthPermission
				' would do is protect the set operations themselves
				' (like size()), which don't seem security-sensitive.
    
				' always return an empty Set instead of null
				' so LoginModules can add to the Set if necessary
				Return privCredentials
			End Get
		End Property

		''' <summary>
		''' Return a {@code Set} of public credentials associated with this
		''' {@code Subject} that are instances or subclasses of the specified
		''' {@code Class}.
		''' 
		''' <p> The returned {@code Set} is not backed by this Subject's
		''' internal public Credential {@code Set}.  A new
		''' {@code Set} is created and returned for each method invocation.
		''' Modifications to the returned {@code Set}
		''' will not affect the internal public Credential {@code Set}.
		''' 
		''' <p>
		''' </summary>
		''' @param <T> the type of the class modeled by {@code c}
		''' </param>
		''' <param name="c"> the returned {@code Set} of public credentials will all be
		'''          instances of this class.
		''' </param>
		''' <returns> a {@code Set} of public credentials that are instances
		'''          of the  specified {@code Class}.
		''' </returns>
		''' <exception cref="NullPointerException"> if the specified {@code Class}
		'''          is {@code null}. </exception>
		Public Function getPublicCredentials(Of T)(ByVal c As Type) As [Set](Of T)

			If c Is Nothing Then Throw New NullPointerException(sun.security.util.ResourcesMgr.getString("invalid.null.Class.provided"))

			' always return an empty Set instead of null
			' so LoginModules can add to the Set if necessary
			Return New ClassSet(Me, Of T)(PUB_CREDENTIAL_SET, c)
		End Function

		''' <summary>
		''' Return a {@code Set} of private credentials associated with this
		''' {@code Subject} that are instances or subclasses of the specified
		''' {@code Class}.
		''' 
		''' <p> The caller must have permission to access all of the
		''' requested Credentials, or a {@code SecurityException}
		''' will be thrown.
		''' 
		''' <p> The returned {@code Set} is not backed by this Subject's
		''' internal private Credential {@code Set}.  A new
		''' {@code Set} is created and returned for each method invocation.
		''' Modifications to the returned {@code Set}
		''' will not affect the internal private Credential {@code Set}.
		''' 
		''' <p>
		''' </summary>
		''' @param <T> the type of the class modeled by {@code c}
		''' </param>
		''' <param name="c"> the returned {@code Set} of private credentials will all be
		'''          instances of this class.
		''' </param>
		''' <returns> a {@code Set} of private credentials that are instances
		'''          of the  specified {@code Class}.
		''' </returns>
		''' <exception cref="NullPointerException"> if the specified {@code Class}
		'''          is {@code null}. </exception>
		Public Function getPrivateCredentials(Of T)(ByVal c As Type) As [Set](Of T)

			' XXX
			' we do not need a security check for
			' AuthPermission(getPrivateCredentials)
			' because we already restrict access to private credentials
			' via the PrivateCredentialPermission.  all the extra AuthPermission
			' would do is protect the set operations themselves
			' (like size()), which don't seem security-sensitive.

			If c Is Nothing Then Throw New NullPointerException(sun.security.util.ResourcesMgr.getString("invalid.null.Class.provided"))

			' always return an empty Set instead of null
			' so LoginModules can add to the Set if necessary
			Return New ClassSet(Me, Of T)(PRIV_CREDENTIAL_SET, c)
		End Function

		''' <summary>
		''' Compares the specified Object with this {@code Subject}
		''' for equality.  Returns true if the given object is also a Subject
		''' and the two {@code Subject} instances are equivalent.
		''' More formally, two {@code Subject} instances are
		''' equal if their {@code Principal} and {@code Credential}
		''' Sets are equal.
		''' 
		''' <p>
		''' </summary>
		''' <param name="o"> Object to be compared for equality with this
		'''          {@code Subject}.
		''' </param>
		''' <returns> true if the specified Object is equal to this
		'''          {@code Subject}.
		''' </returns>
		''' <exception cref="SecurityException"> if the caller does not have permission
		'''          to access the private credentials for this {@code Subject},
		'''          or if the caller does not have permission to access the
		'''          private credentials for the provided {@code Subject}. </exception>
		Public Overrides Function Equals(ByVal o As Object) As Boolean

			If o Is Nothing Then Return False

			If Me Is o Then Return True

			If TypeOf o Is Subject Then

				Dim that As Subject = CType(o, Subject)

				' check the principal and credential sets
				Dim thatPrincipals As [Set](Of java.security.Principal)
				SyncLock that.principals
					' avoid deadlock from dual locks
					thatPrincipals = New HashSet(Of java.security.Principal)(that.principals)
				End SyncLock
				If Not principals.Equals(thatPrincipals) Then Return False

				Dim thatPubCredentials As [Set](Of Object)
				SyncLock that.pubCredentials
					' avoid deadlock from dual locks
					thatPubCredentials = New HashSet(Of Object)(that.pubCredentials)
				End SyncLock
				If Not pubCredentials.Equals(thatPubCredentials) Then Return False

				Dim thatPrivCredentials As [Set](Of Object)
				SyncLock that.privCredentials
					' avoid deadlock from dual locks
					thatPrivCredentials = New HashSet(Of Object)(that.privCredentials)
				End SyncLock
				If Not privCredentials.Equals(thatPrivCredentials) Then Return False
				Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Return the String representation of this {@code Subject}.
		''' 
		''' <p>
		''' </summary>
		''' <returns> the String representation of this {@code Subject}. </returns>
		Public Overrides Function ToString() As String
			Return ToString(True)
		End Function

		''' <summary>
		''' package private convenience method to print out the Subject
		''' without firing off a security check when trying to access
		''' the Private Credentials
		''' </summary>
		Friend Overrides Function ToString(ByVal includePrivateCredentials As Boolean) As String

			Dim s As String = sun.security.util.ResourcesMgr.getString("Subject.")
			Dim suffix As String = ""

			SyncLock principals
				Dim pI As IEnumerator(Of java.security.Principal) = principals.GetEnumerator()
				Do While pI.MoveNext()
					Dim p As java.security.Principal = pI.Current
					suffix = suffix + sun.security.util.ResourcesMgr.getString(".Principal.") + p.ToString() & sun.security.util.ResourcesMgr.getString("NEWLINE")
				Loop
			End SyncLock

			SyncLock pubCredentials
				Dim pI As IEnumerator(Of Object) = pubCredentials.GetEnumerator()
				Do While pI.MoveNext()
					Dim o As Object = pI.Current
					suffix = suffix + sun.security.util.ResourcesMgr.getString(".Public.Credential.") + o.ToString() & sun.security.util.ResourcesMgr.getString("NEWLINE")
				Loop
			End SyncLock

			If includePrivateCredentials Then
				SyncLock privCredentials
					Dim pI As IEnumerator(Of Object) = privCredentials.GetEnumerator()
					Do While pI.MoveNext()
						Try
							Dim o As Object = pI.Current
							suffix += sun.security.util.ResourcesMgr.getString(".Private.Credential.") + o.ToString() & sun.security.util.ResourcesMgr.getString("NEWLINE")
						Catch se As SecurityException
							suffix += sun.security.util.ResourcesMgr.getString(".Private.Credential.inaccessible.")
							Exit Do
						End Try
					Loop
				End SyncLock
			End If
			Return s + suffix
		End Function

		''' <summary>
		''' Returns a hashcode for this {@code Subject}.
		''' 
		''' <p>
		''' </summary>
		''' <returns> a hashcode for this {@code Subject}.
		''' </returns>
		''' <exception cref="SecurityException"> if the caller does not have permission
		'''          to access this Subject's private credentials. </exception>
		Public Overrides Function GetHashCode() As Integer

			''' <summary>
			''' The hashcode is derived exclusive or-ing the
			''' hashcodes of this Subject's Principals and credentials.
			''' 
			''' If a particular credential was destroyed
			''' ({@code credential.hashCode()} throws an
			''' {@code IllegalStateException}),
			''' the hashcode for that credential is derived via:
			''' {@code credential.getClass().toString().hashCode()}.
			''' </summary>

			Dim hashCode As Integer = 0

			SyncLock principals
				Dim pIterator As IEnumerator(Of java.security.Principal) = principals.GetEnumerator()
				Do While pIterator.MoveNext()
					Dim p As java.security.Principal = pIterator.Current
					hashCode = hashCode Xor p.GetHashCode()
				Loop
			End SyncLock

			SyncLock pubCredentials
				Dim pubCIterator As IEnumerator(Of Object) = pubCredentials.GetEnumerator()
				Do While pubCIterator.MoveNext()
					hashCode = hashCode Xor getCredHashCode(pubCIterator.Current)
				Loop
			End SyncLock
			Return hashCode
		End Function

		''' <summary>
		''' get a credential's hashcode
		''' </summary>
		Private Function getCredHashCode(ByVal o As Object) As Integer
			Try
				Return o.GetHashCode()
			Catch ise As IllegalStateException
				Return o.GetType().ToString().GetHashCode()
			End Try
		End Function

		''' <summary>
		''' Writes this object out to a stream (i.e., serializes it).
		''' </summary>
		Private Sub writeObject(ByVal oos As java.io.ObjectOutputStream)
			SyncLock principals
				oos.defaultWriteObject()
			End SyncLock
		End Sub

		''' <summary>
		''' Reads this object from a stream (i.e., deserializes it)
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)

			Dim gf As ObjectInputStream.GetField = s.readFields()

			[readOnly] = gf.get("readOnly", False)

			Dim inputPrincs As [Set](Of java.security.Principal) = CType(gf.get("principals", Nothing), Set(Of java.security.Principal))

			' Rewrap the principals into a SecureSet
			If inputPrincs Is Nothing Then Throw New NullPointerException(sun.security.util.ResourcesMgr.getString("invalid.null.input.s."))
			Try
				principals = Collections.synchronizedSet(New SecureSet(Of java.security.Principal) (Me, PRINCIPAL_SET, inputPrincs))
			Catch npe As NullPointerException
				' Sometimes people deserialize the principals set only.
				' Subject is not accessible, so just don't fail.
				principals = Collections.synchronizedSet(New SecureSet(Of java.security.Principal)(Me, PRINCIPAL_SET))
			End Try

			' The Credential {@code Set} is not serialized, but we do not
			' want the default deserialization routine to set it to null.
			Me.pubCredentials = Collections.synchronizedSet(New SecureSet(Of Object)(Me, PUB_CREDENTIAL_SET))
			Me.privCredentials = Collections.synchronizedSet(New SecureSet(Of Object)(Me, PRIV_CREDENTIAL_SET))
		End Sub

		''' <summary>
		''' Prevent modifications unless caller has permission.
		''' 
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class SecureSet(Of E)
			Inherits AbstractSet(Of E)

			Private Const serialVersionUID As Long = 7911754171111800359L

			''' <summary>
			''' @serialField this$0 Subject The outer Subject instance.
			''' @serialField elements LinkedList The elements in this set.
			''' </summary>
			Private Shared ReadOnly serialPersistentFields As ObjectStreamField() = { New ObjectStreamField("this$0", GetType(Subject)), New ObjectStreamField("elements", GetType(LinkedList)), New ObjectStreamField("which", GetType(Integer)) }

			Friend ___subject As Subject
			Friend elements As LinkedList(Of E)

			''' <summary>
			''' @serial An integer identifying the type of objects contained
			'''      in this set.  If {@code which == 1},
			'''      this is a Principal set and all the elements are
			'''      of type {@code java.security.Principal}.
			'''      If {@code which == 2}, this is a public credential
			'''      set and all the elements are of type {@code Object}.
			'''      If {@code which == 3}, this is a private credential
			'''      set and all the elements are of type {@code Object}.
			''' </summary>
			Private which As Integer

			Friend Sub New(ByVal ___subject As Subject, ByVal which As Integer)
				Me.___subject = ___subject
				Me.which = which
				Me.elements = New LinkedList(Of E)
			End Sub

			Friend Sub New(Of T1 As E)(ByVal ___subject As Subject, ByVal which As Integer, ByVal [set] As [Set](Of T1))
				Me.___subject = ___subject
				Me.which = which
				Me.elements = New LinkedList(Of E)([set])
			End Sub

			Public Overridable Function size() As Integer
				Return elements.Count
			End Function

			Public Overridable Function [iterator]() As IEnumerator(Of E)
				Dim list As LinkedList(Of E) = elements
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Iterator<E>()
	'			{
	'				ListIterator<E> i = list.listIterator(0);
	'
	'				public boolean hasNext()
	'				{
	'					Return i.hasNext();
	'				}
	'
	'				public E next()
	'				{
	'					if (which != Subject.PRIV_CREDENTIAL_SET)
	'					{
	'						Return i.next();
	'					}
	'
	'					SecurityManager sm = System.getSecurityManager();
	'					if (sm != Nothing)
	'					{
	'						try
	'						{
	'							sm.checkPermission(New PrivateCredentialPermission(list.get(i.nextIndex()).getClass().getName(), subject.getPrincipals()));
	'						}
	'						catch (SecurityException se)
	'						{
	'							i.next();
	'							throw (se);
	'						}
	'					}
	'					Return i.next();
	'				}
	'
	'				public void remove()
	'				{
	'
	'					if (subject.isReadOnly())
	'					{
	'						throw New IllegalStateException(ResourcesMgr.getString("Subject.is.read.only"));
	'					}
	'
	'					java.lang.SecurityManager sm = System.getSecurityManager();
	'					if (sm != Nothing)
	'					{
	'						switch (which)
	'						{
	'						case Subject.PRINCIPAL_SET:
	'							sm.checkPermission(AuthPermissionHolder.MODIFY_PRINCIPALS_PERMISSION);
	'							break;
	'						case Subject.PUB_CREDENTIAL_SET:
	'							sm.checkPermission(AuthPermissionHolder.MODIFY_PUBLIC_CREDENTIALS_PERMISSION);
	'							break;
	'						default:
	'							sm.checkPermission(AuthPermissionHolder.MODIFY_PRIVATE_CREDENTIALS_PERMISSION);
	'							break;
	'						}
	'					}
	'					i.remove();
	'				}
	'			};
			End Function

			Public Overridable Function add(ByVal o As E) As Boolean

				If ___subject.readOnly Then Throw New IllegalStateException(sun.security.util.ResourcesMgr.getString("Subject.is.read.only"))

				Dim sm As java.lang.SecurityManager = System.securityManager
				If sm IsNot Nothing Then
					Select Case which
					Case Subject.PRINCIPAL_SET
						sm.checkPermission(AuthPermissionHolder.MODIFY_PRINCIPALS_PERMISSION)
					Case Subject.PUB_CREDENTIAL_SET
						sm.checkPermission(AuthPermissionHolder.MODIFY_PUBLIC_CREDENTIALS_PERMISSION)
					Case Else
						sm.checkPermission(AuthPermissionHolder.MODIFY_PRIVATE_CREDENTIALS_PERMISSION)
					End Select
				End If

				Select Case which
				Case Subject.PRINCIPAL_SET
					If Not(TypeOf o Is java.security.Principal) Then Throw New SecurityException(sun.security.util.ResourcesMgr.getString("attempting.to.add.an.object.which.is.not.an.instance.of.java.security.Principal.to.a.Subject.s.Principal.Set"))
				Case Else
					' ok to add Objects of any kind to credential sets
				End Select

				' check for duplicates
				If Not elements.Contains(o) Then
					Return elements.AddLast(o)
				Else
					Return False
				End If
			End Function

			Public Overridable Function remove(ByVal o As Object) As Boolean

				Dim e As IEnumerator(Of E) = [iterator]()
				Do While e.MoveNext()
					Dim [next] As E
					If which <> Subject.PRIV_CREDENTIAL_SET Then
						[next] = e.Current
					Else
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'						next = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<E>()
	'					{
	'						public E run()
	'						{
	'							Return e.Current;
	'						}
	'					});
					End If

					If [next] Is Nothing Then
						If o Is Nothing Then
							e.remove()
							Return True
						End If
					ElseIf [next].Equals(o) Then
						e.remove()
						Return True
					End If
				Loop
				Return False
			End Function

			Public Overridable Function contains(ByVal o As Object) As Boolean
				Dim e As IEnumerator(Of E) = [iterator]()
				Do While e.MoveNext()
					Dim [next] As E
					If which <> Subject.PRIV_CREDENTIAL_SET Then
						[next] = e.Current
					Else

						' For private credentials:
						' If the caller does not have read permission for
						' for o.getClass(), we throw a SecurityException.
						' Otherwise we check the private cred set to see whether
						' it contains the Object

						Dim sm As SecurityManager = System.securityManager
						If sm IsNot Nothing Then sm.checkPermission(New PrivateCredentialPermission(o.GetType().name, ___subject.principals))
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'						next = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<E>()
	'					{
	'						public E run()
	'						{
	'							Return e.Current;
	'						}
	'					});
					End If

					If [next] Is Nothing Then
						If o Is Nothing Then Return True
					ElseIf [next].Equals(o) Then
						Return True
					End If
				Loop
				Return False
			End Function

			Public Overridable Function removeAll(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
				Objects.requireNonNull(c)
				Dim modified As Boolean = False
				Dim e As IEnumerator(Of E) = [iterator]()
				Do While e.MoveNext()
					Dim [next] As E
					If which <> Subject.PRIV_CREDENTIAL_SET Then
						[next] = e.Current
					Else
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'						next = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<E>()
	'					{
	'						public E run()
	'						{
	'							Return e.Current;
	'						}
	'					});
					End If

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim ce As IEnumerator(Of ?) = c.GetEnumerator()
					Do While ce.MoveNext()
						Dim o As Object = ce.Current
						If [next] Is Nothing Then
							If o Is Nothing Then
								e.remove()
								modified = True
								Exit Do
							End If
						ElseIf [next].Equals(o) Then
							e.remove()
							modified = True
							Exit Do
						End If
					Loop
				Loop
				Return modified
			End Function

			Public Overridable Function retainAll(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
				Objects.requireNonNull(c)
				Dim modified As Boolean = False
				Dim retain As Boolean = False
				Dim e As IEnumerator(Of E) = [iterator]()
				Do While e.MoveNext()
					retain = False
					Dim [next] As E
					If which <> Subject.PRIV_CREDENTIAL_SET Then
						[next] = e.Current
					Else
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'						next = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<E>()
	'					{
	'						public E run()
	'						{
	'							Return e.Current;
	'						}
	'					});
					End If

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim ce As IEnumerator(Of ?) = c.GetEnumerator()
					Do While ce.MoveNext()
						Dim o As Object = ce.Current
						If [next] Is Nothing Then
							If o Is Nothing Then
								retain = True
								Exit Do
							End If
						ElseIf [next].Equals(o) Then
							retain = True
							Exit Do
						End If
					Loop

					If Not retain Then
						e.remove()
						retain = False
						modified = True
					End If
				Loop
				Return modified
			End Function

			Public Overridable Sub clear()
				Dim e As IEnumerator(Of E) = [iterator]()
				Do While e.MoveNext()
					Dim [next] As E
					If which <> Subject.PRIV_CREDENTIAL_SET Then
						[next] = e.Current
					Else
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'						next = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<E>()
	'					{
	'						public E run()
	'						{
	'							Return e.Current;
	'						}
	'					});
					End If
					e.remove()
				Loop
			End Sub

			''' <summary>
			''' Writes this object out to a stream (i.e., serializes it).
			''' 
			''' <p>
			''' 
			''' @serialData If this is a private credential set,
			'''      a security check is performed to ensure that
			'''      the caller has permission to access each credential
			'''      in the set.  If the security check passes,
			'''      the set is serialized.
			''' </summary>
			Private Sub writeObject(ByVal oos As java.io.ObjectOutputStream)

				If which = Subject.PRIV_CREDENTIAL_SET Then
					' check permissions before serializing
					Dim i As IEnumerator(Of E) = [iterator]()
					Do While i.MoveNext()
						i.Current
					Loop
				End If
				Dim fields As ObjectOutputStream.PutField = oos.putFields()
				fields.put("this$0", ___subject)
				fields.put("elements", elements)
				fields.put("which", which)
				oos.writeFields()
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private Sub readObject(ByVal ois As ObjectInputStream)
				Dim fields As ObjectInputStream.GetField = ois.readFields()
				___subject = CType(fields.get("this$0", Nothing), Subject)
				which = fields.get("which", 0)

				Dim tmp As LinkedList(Of E) = CType(fields.get("elements", Nothing), LinkedList(Of E))
				If tmp.GetType() IsNot GetType(LinkedList) Then
					elements = New LinkedList(Of E)(tmp)
				Else
					elements = tmp
				End If
			End Sub
		End Class

		''' <summary>
		''' This class implements a {@code Set} which returns only
		''' members that are an instance of a specified Class.
		''' </summary>
		Private Class ClassSet(Of T)
			Inherits AbstractSet(Of T)

			Private ReadOnly outerInstance As Subject


			Private which As Integer
			Private c As Type
			Private [set] As [Set](Of T)

			Friend Sub New(ByVal outerInstance As Subject, ByVal which As Integer, ByVal c As Type)
					Me.outerInstance = outerInstance
				Me.which = which
				Me.c = c
				[set] = New HashSet(Of T)

				Select Case which
				Case Subject.PRINCIPAL_SET
					SyncLock outerInstance.principals
						populateSet()
					End SyncLock
				Case Subject.PUB_CREDENTIAL_SET
					SyncLock outerInstance.pubCredentials
						populateSet()
					End SyncLock
				Case Else
					SyncLock outerInstance.privCredentials
						populateSet()
					End SyncLock
				End Select
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private Sub populateSet() 'To suppress warning from line 1374
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim [iterator] As IEnumerator(Of ?)
				Select Case which
				Case Subject.PRINCIPAL_SET
					[iterator] = outerInstance.principals.GetEnumerator()
				Case Subject.PUB_CREDENTIAL_SET
					[iterator] = outerInstance.pubCredentials.GetEnumerator()
				Case Else
					[iterator] = outerInstance.privCredentials.GetEnumerator()
				End Select

				' Check whether the caller has permisson to get
				' credentials of Class c

				Do While [iterator].MoveNext()
					Dim [next] As Object
					If which = Subject.PRIV_CREDENTIAL_SET Then
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'						next = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<Object>()
	'					{
	'						public Object run()
	'						{
	'							Return iterator.Current;
	'						}
	'					});
					Else
						[next] = [iterator].Current
					End If
					If c.IsAssignableFrom([next].GetType()) Then
						If which <> Subject.PRIV_CREDENTIAL_SET Then
							[set].add(CType([next], T))
						Else
							' Check permission for private creds
							Dim sm As SecurityManager = System.securityManager
							If sm IsNot Nothing Then sm.checkPermission(New PrivateCredentialPermission([next].GetType().name, outerInstance.principals))
							[set].add(CType([next], T))
						End If
					End If
				Loop
			End Sub

			Public Overridable Function size() As Integer
				Return [set].size()
			End Function

			Public Overridable Function [iterator]() As IEnumerator(Of T)
				Return [set].GetEnumerator()
			End Function

			Public Overridable Function add(ByVal o As T) As Boolean

				If Not o.GetType().IsAssignableFrom(c) Then
					Dim form As New java.text.MessageFormat(sun.security.util.ResourcesMgr.getString("attempting.to.add.an.object.which.is.not.an.instance.of.class"))
					Dim source As Object() = {c.ToString()}
					Throw New SecurityException(form.format(source))
				End If

				Return [set].add(o)
			End Function
		End Class

		Friend Class AuthPermissionHolder
			Friend Shared ReadOnly DO_AS_PERMISSION As New AuthPermission("doAs")

			Friend Shared ReadOnly DO_AS_PRIVILEGED_PERMISSION As New AuthPermission("doAsPrivileged")

			Friend Shared ReadOnly SET_READ_ONLY_PERMISSION As New AuthPermission("setReadOnly")

			Friend Shared ReadOnly GET_SUBJECT_PERMISSION As New AuthPermission("getSubject")

			Friend Shared ReadOnly MODIFY_PRINCIPALS_PERMISSION As New AuthPermission("modifyPrincipals")

			Friend Shared ReadOnly MODIFY_PUBLIC_CREDENTIALS_PERMISSION As New AuthPermission("modifyPublicCredentials")

			Friend Shared ReadOnly MODIFY_PRIVATE_CREDENTIALS_PERMISSION As New AuthPermission("modifyPrivateCredentials")
		End Class
	End Class

End Namespace
Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports javax.security.auth.callback

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

Namespace javax.security.auth.login


	''' <summary>
	''' <p> The {@code LoginContext} class describes the basic methods used
	''' to authenticate Subjects and provides a way to develop an
	''' application independent of the underlying authentication technology.
	''' A {@code Configuration} specifies the authentication technology, or
	''' {@code LoginModule}, to be used with a particular application.
	''' Different LoginModules can be plugged in under an application
	''' without requiring any modifications to the application itself.
	''' 
	''' <p> In addition to supporting <i>pluggable</i> authentication, this class
	''' also supports the notion of <i>stacked</i> authentication.
	''' Applications may be configured to use more than one
	''' LoginModule.  For example, one could
	''' configure both a Kerberos LoginModule and a smart card
	''' LoginModule under an application.
	''' 
	''' <p> A typical caller instantiates a LoginContext with
	''' a <i>name</i> and a {@code CallbackHandler}.
	''' LoginContext uses the <i>name</i> as the index into a
	''' Configuration to determine which LoginModules should be used,
	''' and which ones must succeed in order for the overall authentication to
	''' succeed.  The {@code CallbackHandler} is passed to the underlying
	''' LoginModules so they may communicate and interact with users
	''' (prompting for a username and password via a graphical user interface,
	''' for example).
	''' 
	''' <p> Once the caller has instantiated a LoginContext,
	''' it invokes the {@code login} method to authenticate
	''' a {@code Subject}.  The {@code login} method invokes
	''' the configured modules to perform their respective types of authentication
	''' (username/password, smart card pin verification, etc.).
	''' Note that the LoginModules will not attempt authentication retries nor
	''' introduce delays if the authentication fails.
	''' Such tasks belong to the LoginContext caller.
	''' 
	''' <p> If the {@code login} method returns without
	''' throwing an exception, then the overall authentication succeeded.
	''' The caller can then retrieve
	''' the newly authenticated Subject by invoking the
	''' {@code getSubject} method.  Principals and Credentials associated
	''' with the Subject may be retrieved by invoking the Subject's
	''' respective {@code getPrincipals}, {@code getPublicCredentials},
	''' and {@code getPrivateCredentials} methods.
	''' 
	''' <p> To logout the Subject, the caller calls
	''' the {@code logout} method.  As with the {@code login}
	''' method, this {@code logout} method invokes the {@code logout}
	''' method for the configured modules.
	''' 
	''' <p> A LoginContext should not be used to authenticate
	''' more than one Subject.  A separate LoginContext
	''' should be used to authenticate each different Subject.
	''' 
	''' <p> The following documentation applies to all LoginContext constructors:
	''' <ol>
	''' 
	''' <li> {@code Subject}
	''' <ul>
	''' <li> If the constructor has a Subject
	''' input parameter, the LoginContext uses the caller-specified
	''' Subject object.
	''' 
	''' <li> If the caller specifies a {@code null} Subject
	''' and a {@code null} value is permitted,
	''' the LoginContext instantiates a new Subject.
	''' 
	''' <li> If the constructor does <b>not</b> have a Subject
	''' input parameter, the LoginContext instantiates a new Subject.
	''' <p>
	''' </ul>
	''' 
	''' <li> {@code Configuration}
	''' <ul>
	''' <li> If the constructor has a Configuration
	''' input parameter and the caller specifies a non-null Configuration,
	''' the LoginContext uses the caller-specified Configuration.
	''' <p>
	''' If the constructor does <b>not</b> have a Configuration
	''' input parameter, or if the caller specifies a {@code null}
	''' Configuration object, the constructor uses the following call to
	''' get the installed Configuration:
	''' <pre>
	'''      config = Configuration.getConfiguration();
	''' </pre>
	''' For both cases,
	''' the <i>name</i> argument given to the constructor is passed to the
	''' {@code Configuration.getAppConfigurationEntry} method.
	''' If the Configuration has no entries for the specified <i>name</i>,
	''' then the {@code LoginContext} calls
	''' {@code getAppConfigurationEntry} with the name, "<i>other</i>"
	''' (the default entry name).  If there is no entry for "<i>other</i>",
	''' then a {@code LoginException} is thrown.
	''' 
	''' <li> When LoginContext uses the installed Configuration, the caller
	''' requires the createLoginContext.<em>name</em> and possibly
	''' createLoginContext.other AuthPermissions. Furthermore, the
	''' LoginContext will invoke configured modules from within an
	''' {@code AccessController.doPrivileged} call so that modules that
	''' perform security-sensitive tasks (such as connecting to remote hosts,
	''' and updating the Subject) will require the respective permissions, but
	''' the callers of the LoginContext will not require those permissions.
	''' 
	''' <li> When LoginContext uses a caller-specified Configuration, the caller
	''' does not require any createLoginContext AuthPermission.  The LoginContext
	''' saves the {@code AccessControlContext} for the caller,
	''' and invokes the configured modules from within an
	''' {@code AccessController.doPrivileged} call constrained by that context.
	''' This means the caller context (stored when the LoginContext was created)
	''' must have sufficient permissions to perform any security-sensitive tasks
	''' that the modules may perform.
	''' <p>
	''' </ul>
	''' 
	''' <li> {@code CallbackHandler}
	''' <ul>
	''' <li> If the constructor has a CallbackHandler
	''' input parameter, the LoginContext uses the caller-specified
	''' CallbackHandler object.
	''' 
	''' <li> If the constructor does <b>not</b> have a CallbackHandler
	''' input parameter, or if the caller specifies a {@code null}
	''' CallbackHandler object (and a {@code null} value is permitted),
	''' the LoginContext queries the
	''' {@code auth.login.defaultCallbackHandler} security property for the
	''' fully qualified class name of a default handler
	''' implementation. If the security property is not set,
	''' then the underlying modules will not have a
	''' CallbackHandler for use in communicating
	''' with users.  The caller thus assumes that the configured
	''' modules have alternative means for authenticating the user.
	''' 
	''' 
	''' <li> When the LoginContext uses the installed Configuration (instead of
	''' a caller-specified Configuration, see above),
	''' then this LoginContext must wrap any
	''' caller-specified or default CallbackHandler implementation
	''' in a new CallbackHandler implementation
	''' whose {@code handle} method implementation invokes the
	''' specified CallbackHandler's {@code handle} method in a
	''' {@code java.security.AccessController.doPrivileged} call
	''' constrained by the caller's current {@code AccessControlContext}.
	''' </ul>
	''' </ol>
	''' </summary>
	''' <seealso cref= java.security.Security </seealso>
	''' <seealso cref= javax.security.auth.AuthPermission </seealso>
	''' <seealso cref= javax.security.auth.Subject </seealso>
	''' <seealso cref= javax.security.auth.callback.CallbackHandler </seealso>
	''' <seealso cref= javax.security.auth.login.Configuration </seealso>
	''' <seealso cref= javax.security.auth.spi.LoginModule </seealso>
	''' <seealso cref= java.security.Security security properties </seealso>
	Public Class LoginContext

		Private Const INIT_METHOD As String = "initialize"
		Private Const LOGIN_METHOD As String = "login"
		Private Const COMMIT_METHOD As String = "commit"
		Private Const ABORT_METHOD As String = "abort"
		Private Const LOGOUT_METHOD As String = "logout"
		Private Const OTHER As String = "other"
		Private Const DEFAULT_HANDLER As String = "auth.login.defaultCallbackHandler"
		Private ___subject As javax.security.auth.Subject = Nothing
		Private subjectProvided As Boolean = False
		Private loginSucceeded As Boolean = False
		Private callbackHandler As CallbackHandler
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private state As IDictionary(Of String, ?) = New Dictionary(Of String, Object)

		Private config As Configuration
		Private creatorAcc As java.security.AccessControlContext = Nothing ' customized config only
		Private moduleStack As ModuleInfo()
		Private contextClassLoader As ClassLoader = Nothing
		Private Shared ReadOnly PARAMS As Type() = { }

		' state saved in the event a user-specified asynchronous exception
		' was specified and thrown

		Private moduleIndex As Integer = 0
		Private firstError As LoginException = Nothing
		Private firstRequiredError As LoginException = Nothing
		Private success As Boolean = False

		Private Shared ReadOnly debug As sun.security.util.Debug = sun.security.util.Debug.getInstance("logincontext", vbTab & "[LoginContext]")

		Private Sub init(ByVal name As String)

			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing AndAlso creatorAcc Is Nothing Then sm.checkPermission(New javax.security.auth.AuthPermission("createLoginContext." & name))

			If name Is Nothing Then Throw New LoginException(sun.security.util.ResourcesMgr.getString("Invalid.null.input.name"))

			' get the Configuration
			If config Is Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				config = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<Configuration>()
	'			{
	'				public Configuration run()
	'				{
	'					Return Configuration.getConfiguration();
	'				}
	'			});
			End If

			' get the LoginModules configured for this application
			Dim entries As AppConfigurationEntry() = config.getAppConfigurationEntry(name)
			If entries Is Nothing Then

				If sm IsNot Nothing AndAlso creatorAcc Is Nothing Then sm.checkPermission(New javax.security.auth.AuthPermission("createLoginContext." & OTHER))

				entries = config.getAppConfigurationEntry(OTHER)
				If entries Is Nothing Then
					Dim form As New java.text.MessageFormat(sun.security.util.ResourcesMgr.getString("No.LoginModules.configured.for.name"))
					Dim source As Object() = {name}
					Throw New LoginException(form.format(source))
				End If
			End If
			moduleStack = New ModuleInfo(entries.Length - 1){}
			For i As Integer = 0 To entries.Length - 1
				' clone returned array
				moduleStack(i) = New ModuleInfo(New AppConfigurationEntry(entries(i).loginModuleName, entries(i).controlFlag, entries(i).options), Nothing)
			Next i

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			contextClassLoader = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<ClassLoader>()
	'		{
	'				public ClassLoader run()
	'				{
	'					ClassLoader loader = Thread.currentThread().getContextClassLoader();
	'					if (loader == Nothing)
	'					{
	'						' Don't use bootstrap class loader directly to ensure
	'						' proper package access control!
	'						loader = ClassLoader.getSystemClassLoader();
	'					}
	'
	'					Return loader;
	'				}
	'		});
		End Sub

		Private Sub loadDefaultCallbackHandler()

			' get the default handler class
			Try

				Dim finalLoader As ClassLoader = contextClassLoader

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Me.callbackHandler = java.security.AccessController.doPrivileged(New java.security.PrivilegedExceptionAction<CallbackHandler>()
	'			{
	'				public CallbackHandler run() throws Exception
	'				{
	'					String defaultHandler = java.security.Security.getProperty(DEFAULT_HANDLER);
	'					if (defaultHandler == Nothing || defaultHandler.length() == 0)
	'						Return Nothing;
	'					Class c = Class.forName(defaultHandler, True, finalLoader).asSubclass(CallbackHandler.class);
	'					Return c.newInstance();
	'				}
	'			});
			Catch pae As java.security.PrivilegedActionException
				Throw New LoginException(pae.exception.ToString())
			End Try

			' secure it with the caller's ACC
			If Me.callbackHandler IsNot Nothing AndAlso creatorAcc Is Nothing Then Me.callbackHandler = New SecureCallbackHandler(java.security.AccessController.context, Me.callbackHandler)
		End Sub

		''' <summary>
		''' Instantiate a new {@code LoginContext} object with a name.
		''' </summary>
		''' <param name="name"> the name used as the index into the
		'''          {@code Configuration}.
		''' </param>
		''' <exception cref="LoginException"> if the caller-specified {@code name}
		'''          does not appear in the {@code Configuration}
		'''          and there is no {@code Configuration} entry
		'''          for "<i>other</i>", or if the
		'''          <i>auth.login.defaultCallbackHandler</i>
		'''          security property was set, but the implementation
		'''          class could not be loaded.
		'''          <p> </exception>
		''' <exception cref="SecurityException"> if a SecurityManager is set and
		'''          the caller does not have
		'''          AuthPermission("createLoginContext.<i>name</i>"),
		'''          or if a configuration entry for <i>name</i> does not exist and
		'''          the caller does not additionally have
		'''          AuthPermission("createLoginContext.other") </exception>
		Public Sub New(ByVal name As String)
			init(name)
			loadDefaultCallbackHandler()
		End Sub

		''' <summary>
		''' Instantiate a new {@code LoginContext} object with a name
		''' and a {@code Subject} object.
		''' 
		''' <p>
		''' </summary>
		''' <param name="name"> the name used as the index into the
		'''          {@code Configuration}. <p>
		''' </param>
		''' <param name="subject"> the {@code Subject} to authenticate.
		''' </param>
		''' <exception cref="LoginException"> if the caller-specified {@code name}
		'''          does not appear in the {@code Configuration}
		'''          and there is no {@code Configuration} entry
		'''          for "<i>other</i>", if the caller-specified {@code subject}
		'''          is {@code null}, or if the
		'''          <i>auth.login.defaultCallbackHandler</i>
		'''          security property was set, but the implementation
		'''          class could not be loaded.
		'''          <p> </exception>
		''' <exception cref="SecurityException"> if a SecurityManager is set and
		'''          the caller does not have
		'''          AuthPermission("createLoginContext.<i>name</i>"),
		'''          or if a configuration entry for <i>name</i> does not exist and
		'''          the caller does not additionally have
		'''          AuthPermission("createLoginContext.other") </exception>
		Public Sub New(ByVal name As String, ByVal ___subject As javax.security.auth.Subject)
			init(name)
			If ___subject Is Nothing Then Throw New LoginException(sun.security.util.ResourcesMgr.getString("invalid.null.Subject.provided"))
			Me.___subject = ___subject
			subjectProvided = True
			loadDefaultCallbackHandler()
		End Sub

		''' <summary>
		''' Instantiate a new {@code LoginContext} object with a name
		''' and a {@code CallbackHandler} object.
		''' 
		''' <p>
		''' </summary>
		''' <param name="name"> the name used as the index into the
		'''          {@code Configuration}. <p>
		''' </param>
		''' <param name="callbackHandler"> the {@code CallbackHandler} object used by
		'''          LoginModules to communicate with the user.
		''' </param>
		''' <exception cref="LoginException"> if the caller-specified {@code name}
		'''          does not appear in the {@code Configuration}
		'''          and there is no {@code Configuration} entry
		'''          for "<i>other</i>", or if the caller-specified
		'''          {@code callbackHandler} is {@code null}.
		'''          <p> </exception>
		''' <exception cref="SecurityException"> if a SecurityManager is set and
		'''          the caller does not have
		'''          AuthPermission("createLoginContext.<i>name</i>"),
		'''          or if a configuration entry for <i>name</i> does not exist and
		'''          the caller does not additionally have
		'''          AuthPermission("createLoginContext.other") </exception>
		Public Sub New(ByVal name As String, ByVal callbackHandler As CallbackHandler)
			init(name)
			If callbackHandler Is Nothing Then Throw New LoginException(sun.security.util.ResourcesMgr.getString("invalid.null.CallbackHandler.provided"))
			Me.callbackHandler = New SecureCallbackHandler(java.security.AccessController.context, callbackHandler)
		End Sub

		''' <summary>
		''' Instantiate a new {@code LoginContext} object with a name,
		''' a {@code Subject} to be authenticated, and a
		''' {@code CallbackHandler} object.
		''' 
		''' <p>
		''' </summary>
		''' <param name="name"> the name used as the index into the
		'''          {@code Configuration}. <p>
		''' </param>
		''' <param name="subject"> the {@code Subject} to authenticate. <p>
		''' </param>
		''' <param name="callbackHandler"> the {@code CallbackHandler} object used by
		'''          LoginModules to communicate with the user.
		''' </param>
		''' <exception cref="LoginException"> if the caller-specified {@code name}
		'''          does not appear in the {@code Configuration}
		'''          and there is no {@code Configuration} entry
		'''          for "<i>other</i>", or if the caller-specified
		'''          {@code subject} is {@code null},
		'''          or if the caller-specified
		'''          {@code callbackHandler} is {@code null}.
		'''          <p> </exception>
		''' <exception cref="SecurityException"> if a SecurityManager is set and
		'''          the caller does not have
		'''          AuthPermission("createLoginContext.<i>name</i>"),
		'''          or if a configuration entry for <i>name</i> does not exist and
		'''          the caller does not additionally have
		'''          AuthPermission("createLoginContext.other") </exception>
		Public Sub New(ByVal name As String, ByVal ___subject As javax.security.auth.Subject, ByVal callbackHandler As CallbackHandler)
			Me.New(name, ___subject)
			If callbackHandler Is Nothing Then Throw New LoginException(sun.security.util.ResourcesMgr.getString("invalid.null.CallbackHandler.provided"))
			Me.callbackHandler = New SecureCallbackHandler(java.security.AccessController.context, callbackHandler)
		End Sub

		''' <summary>
		''' Instantiate a new {@code LoginContext} object with a name,
		''' a {@code Subject} to be authenticated,
		''' a {@code CallbackHandler} object, and a login
		''' {@code Configuration}.
		''' 
		''' <p>
		''' </summary>
		''' <param name="name"> the name used as the index into the caller-specified
		'''          {@code Configuration}. <p>
		''' </param>
		''' <param name="subject"> the {@code Subject} to authenticate,
		'''          or {@code null}. <p>
		''' </param>
		''' <param name="callbackHandler"> the {@code CallbackHandler} object used by
		'''          LoginModules to communicate with the user, or {@code null}.
		'''          <p>
		''' </param>
		''' <param name="config"> the {@code Configuration} that lists the
		'''          login modules to be called to perform the authentication,
		'''          or {@code null}.
		''' </param>
		''' <exception cref="LoginException"> if the caller-specified {@code name}
		'''          does not appear in the {@code Configuration}
		'''          and there is no {@code Configuration} entry
		'''          for "<i>other</i>".
		'''          <p> </exception>
		''' <exception cref="SecurityException"> if a SecurityManager is set,
		'''          <i>config</i> is {@code null},
		'''          and either the caller does not have
		'''          AuthPermission("createLoginContext.<i>name</i>"),
		'''          or if a configuration entry for <i>name</i> does not exist and
		'''          the caller does not additionally have
		'''          AuthPermission("createLoginContext.other")
		''' 
		''' @since 1.5 </exception>
		Public Sub New(ByVal name As String, ByVal ___subject As javax.security.auth.Subject, ByVal callbackHandler As CallbackHandler, ByVal config As Configuration)
			Me.config = config
			If config IsNot Nothing Then creatorAcc = java.security.AccessController.context

			init(name)
			If ___subject IsNot Nothing Then
				Me.___subject = ___subject
				subjectProvided = True
			End If
			If callbackHandler Is Nothing Then
				loadDefaultCallbackHandler()
			ElseIf creatorAcc Is Nothing Then
				Me.callbackHandler = New SecureCallbackHandler(java.security.AccessController.context, callbackHandler)
			Else
				Me.callbackHandler = callbackHandler
			End If
		End Sub

		''' <summary>
		''' Perform the authentication.
		''' 
		''' <p> This method invokes the {@code login} method for each
		''' LoginModule configured for the <i>name</i> specified to the
		''' {@code LoginContext} constructor, as determined by the login
		''' {@code Configuration}.  Each {@code LoginModule}
		''' then performs its respective type of authentication
		''' (username/password, smart card pin verification, etc.).
		''' 
		''' <p> This method completes a 2-phase authentication process by
		''' calling each configured LoginModule's {@code commit} method
		''' if the overall authentication succeeded (the relevant REQUIRED,
		''' REQUISITE, SUFFICIENT, and OPTIONAL LoginModules succeeded),
		''' or by calling each configured LoginModule's {@code abort} method
		''' if the overall authentication failed.  If authentication succeeded,
		''' each successful LoginModule's {@code commit} method associates
		''' the relevant Principals and Credentials with the {@code Subject}.
		''' If authentication failed, each LoginModule's {@code abort} method
		''' removes/destroys any previously stored state.
		''' 
		''' <p> If the {@code commit} phase of the authentication process
		''' fails, then the overall authentication fails and this method
		''' invokes the {@code abort} method for each configured
		''' {@code LoginModule}.
		''' 
		''' <p> If the {@code abort} phase
		''' fails for any reason, then this method propagates the
		''' original exception thrown either during the {@code login} phase
		''' or the {@code commit} phase.  In either case, the overall
		''' authentication fails.
		''' 
		''' <p> In the case where multiple LoginModules fail,
		''' this method propagates the exception raised by the first
		''' {@code LoginModule} which failed.
		''' 
		''' <p> Note that if this method enters the {@code abort} phase
		''' (either the {@code login} or {@code commit} phase failed),
		''' this method invokes all LoginModules configured for the
		''' application regardless of their respective {@code Configuration}
		''' flag parameters.  Essentially this means that {@code Requisite}
		''' and {@code Sufficient} semantics are ignored during the
		''' {@code abort} phase.  This guarantees that proper cleanup
		''' and state restoration can take place.
		''' 
		''' <p>
		''' </summary>
		''' <exception cref="LoginException"> if the authentication fails. </exception>
		Public Overridable Sub login()

			loginSucceeded = False

			If ___subject Is Nothing Then ___subject = New javax.security.auth.Subject

			Try
				' module invoked in doPrivileged
				invokePriv(LOGIN_METHOD)
				invokePriv(COMMIT_METHOD)
				loginSucceeded = True
			Catch le As LoginException
				Try
					invokePriv(ABORT_METHOD)
				Catch le2 As LoginException
					Throw le
				End Try
				Throw le
			End Try
		End Sub

		''' <summary>
		''' Logout the {@code Subject}.
		''' 
		''' <p> This method invokes the {@code logout} method for each
		''' {@code LoginModule} configured for this {@code LoginContext}.
		''' Each {@code LoginModule} performs its respective logout procedure
		''' which may include removing/destroying
		''' {@code Principal} and {@code Credential} information
		''' from the {@code Subject} and state cleanup.
		''' 
		''' <p> Note that this method invokes all LoginModules configured for the
		''' application regardless of their respective
		''' {@code Configuration} flag parameters.  Essentially this means
		''' that {@code Requisite} and {@code Sufficient} semantics are
		''' ignored for this method.  This guarantees that proper cleanup
		''' and state restoration can take place.
		''' 
		''' <p>
		''' </summary>
		''' <exception cref="LoginException"> if the logout fails. </exception>
		Public Overridable Sub logout()
			If ___subject Is Nothing Then Throw New LoginException(sun.security.util.ResourcesMgr.getString("null.subject.logout.called.before.login"))

			' module invoked in doPrivileged
			invokePriv(LOGOUT_METHOD)
		End Sub

		''' <summary>
		''' Return the authenticated Subject.
		''' 
		''' <p>
		''' </summary>
		''' <returns> the authenticated Subject.  If the caller specified a
		'''          Subject to this LoginContext's constructor,
		'''          this method returns the caller-specified Subject.
		'''          If a Subject was not specified and authentication succeeds,
		'''          this method returns the Subject instantiated and used for
		'''          authentication by this LoginContext.
		'''          If a Subject was not specified, and authentication fails or
		'''          has not been attempted, this method returns null. </returns>
		Public Overridable Property subject As javax.security.auth.Subject
			Get
				If (Not loginSucceeded) AndAlso (Not subjectProvided) Then Return Nothing
				Return ___subject
			End Get
		End Property

		Private Sub clearState()
			moduleIndex = 0
			firstError = Nothing
			firstRequiredError = Nothing
			success = False
		End Sub

		Private Sub throwException(ByVal originalError As LoginException, ByVal le As LoginException)

			' first clear state
			clearState()

			' throw the exception
			Dim [error] As LoginException = If(originalError IsNot Nothing, originalError, le)
			Throw [error]
		End Sub

		''' <summary>
		''' Invokes the login, commit, and logout methods
		''' from a LoginModule inside a doPrivileged block restricted
		''' by creatorAcc (may be null).
		''' 
		''' This version is called if the caller did not instantiate
		''' the LoginContext with a Configuration object.
		''' </summary>
		Private Sub invokePriv(ByVal methodName As String)
			Try
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				java.security.AccessController.doPrivileged(New java.security.PrivilegedExceptionAction<Void>()
	'			{
	'				public Void run() throws LoginException
	'				{
	'					invoke(methodName);
	'					Return Nothing;
	'				}
	'			}, creatorAcc);
			Catch pae As java.security.PrivilegedActionException
				Throw CType(pae.exception, LoginException)
			End Try
		End Sub

		Private Sub invoke(ByVal methodName As String)

			' start at moduleIndex
			' - this can only be non-zero if methodName is LOGIN_METHOD

			Dim i As Integer = moduleIndex
			Do While i < moduleStack.Length
				Try

					Dim mIndex As Integer = 0
					Dim methods As Method() = Nothing

					If moduleStack(i).module IsNot Nothing Then
						methods = moduleStack(i).module.GetType().GetMethods()
					Else

						' instantiate the LoginModule
						'
						' Allow any object to be a LoginModule as long as it
						' conforms to the interface.
						Dim c As Type = Type.GetType(moduleStack(i).entry.loginModuleName, True, contextClassLoader)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Dim constructor As Constructor(Of ?) = c.GetConstructor(PARAMS)
						Dim args As Object() = { }
						moduleStack(i).module = constructor.newInstance(args)

						' call the LoginModule's initialize method
						methods = moduleStack(i).module.GetType().GetMethods()
						For mIndex = 0 To methods.Length - 1
							If methods(mIndex).name.Equals(INIT_METHOD) Then Exit For
						Next mIndex

						Dim initArgs As Object() = {___subject, callbackHandler, state, moduleStack(i).entry.options }
						' invoke the LoginModule initialize method
						'
						' Throws ArrayIndexOutOfBoundsException if no such
						' method defined.  May improve to use LoginException in
						' the future.
						methods(mIndex).invoke(moduleStack(i).module, initArgs)
					End If

					' find the requested method in the LoginModule
					For mIndex = 0 To methods.Length - 1
						If methods(mIndex).name.Equals(methodName) Then Exit For
					Next mIndex

					' set up the arguments to be passed to the LoginModule method
					Dim args As Object() = { }

					' invoke the LoginModule method
					'
					' Throws ArrayIndexOutOfBoundsException if no such
					' method defined.  May improve to use LoginException in
					' the future.
					Dim status As Boolean = CBool(methods(mIndex).invoke(moduleStack(i).module, args))

					If status = True Then

						' if SUFFICIENT, return if no prior REQUIRED errors
						If (Not methodName.Equals(ABORT_METHOD)) AndAlso (Not methodName.Equals(LOGOUT_METHOD)) AndAlso moduleStack(i).entry.controlFlag Is AppConfigurationEntry.LoginModuleControlFlag.SUFFICIENT AndAlso firstRequiredError Is Nothing Then

							' clear state
							clearState()

							If debug IsNot Nothing Then debug.println(methodName & " SUFFICIENT success")
							Return
						End If

						If debug IsNot Nothing Then debug.println(methodName & " success")
						success = True
					Else
						If debug IsNot Nothing Then debug.println(methodName & " ignored")
					End If

				Catch nsme As NoSuchMethodException
					Dim form As New java.text.MessageFormat(sun.security.util.ResourcesMgr.getString("unable.to.instantiate.LoginModule.module.because.it.does.not.provide.a.no.argument.constructor"))
					Dim source As Object() = {moduleStack(i).entry.loginModuleName}
					throwException(Nothing, New LoginException(form.format(source)))
				Catch ie As InstantiationException
					throwException(Nothing, New LoginException(sun.security.util.ResourcesMgr.getString("unable.to.instantiate.LoginModule.") + ie.Message))
				Catch cnfe As ClassNotFoundException
					throwException(Nothing, New LoginException(sun.security.util.ResourcesMgr.getString("unable.to.find.LoginModule.class.") + cnfe.Message))
				Catch iae As IllegalAccessException
					throwException(Nothing, New LoginException(sun.security.util.ResourcesMgr.getString("unable.to.access.LoginModule.") + iae.Message))
				Catch ite As InvocationTargetException

					' failure cases

					Dim le As LoginException

					If TypeOf ite.InnerException Is sun.security.util.PendingException AndAlso methodName.Equals(LOGIN_METHOD) Then

						' XXX
						'
						' if a module's LOGIN_METHOD threw a PendingException
						' then immediately throw it.
						'
						' when LoginContext is called again,
						' the module that threw the exception is invoked first
						' (the module list is not invoked from the start).
						' previously thrown exception state is still present.
						'
						' it is assumed that the module which threw
						' the exception can have its
						' LOGIN_METHOD invoked twice in a row
						' without any commit/abort in between.
						'
						' in all cases when LoginContext returns
						' (either via natural return or by throwing an exception)
						' we need to call clearState before returning.
						' the only time that is not true is in this case -
						' do not call throwException here.

						Throw CType(ite.InnerException, sun.security.util.PendingException)

					ElseIf TypeOf ite.InnerException Is LoginException Then

						le = CType(ite.InnerException, LoginException)

					ElseIf TypeOf ite.InnerException Is SecurityException Then

						' do not want privacy leak
						' (e.g., sensitive file path in exception msg)

						le = New LoginException("Security Exception")
						le.initCause(New SecurityException)
						If debug IsNot Nothing Then
							debug.println("original security exception with detail msg " & "replaced by new exception with empty detail msg")
							debug.println("original security exception: " & ite.InnerException.ToString())
						End If
					Else

						' capture an unexpected LoginModule exception
						Dim sw As New java.io.StringWriter
						ite.InnerException.printStackTrace(New java.io.PrintWriter(sw))
						sw.flush()
						le = New LoginException(sw.ToString())
					End If

					If moduleStack(i).entry.controlFlag Is AppConfigurationEntry.LoginModuleControlFlag.REQUISITE Then

						If debug IsNot Nothing Then debug.println(methodName & " REQUISITE failure")

						' if REQUISITE, then immediately throw an exception
						If methodName.Equals(ABORT_METHOD) OrElse methodName.Equals(LOGOUT_METHOD) Then
							If firstRequiredError Is Nothing Then firstRequiredError = le
						Else
							throwException(firstRequiredError, le)
						End If

					ElseIf moduleStack(i).entry.controlFlag Is AppConfigurationEntry.LoginModuleControlFlag.REQUIRED Then

						If debug IsNot Nothing Then debug.println(methodName & " REQUIRED failure")

						' mark down that a REQUIRED module failed
						If firstRequiredError Is Nothing Then firstRequiredError = le

					Else

						If debug IsNot Nothing Then debug.println(methodName & " OPTIONAL failure")

						' mark down that an OPTIONAL module failed
						If firstError Is Nothing Then firstError = le
					End If
				End Try
				i += 1
				moduleIndex += 1
			Loop

			' we went thru all the LoginModules.
			If firstRequiredError IsNot Nothing Then
				' a REQUIRED module failed -- return the error
				throwException(firstRequiredError, Nothing)
			ElseIf success = False AndAlso firstError IsNot Nothing Then
				' no module succeeded -- return the first error
				throwException(firstError, Nothing)
			ElseIf success = False Then
				' no module succeeded -- all modules were IGNORED
				throwException(New LoginException(sun.security.util.ResourcesMgr.getString("Login.Failure.all.modules.ignored")), Nothing)
			Else
				' success

				clearState()
				Return
			End If
		End Sub

		''' <summary>
		''' Wrap the caller-specified CallbackHandler in our own
		''' and invoke it within a privileged block, constrained by
		''' the caller's AccessControlContext.
		''' </summary>
		Private Class SecureCallbackHandler
			Implements CallbackHandler

			Private ReadOnly acc As java.security.AccessControlContext
			Private ReadOnly ch As CallbackHandler

			Friend Sub New(ByVal acc As java.security.AccessControlContext, ByVal ch As CallbackHandler)
				Me.acc = acc
				Me.ch = ch
			End Sub

			Public Overridable Sub handle(ByVal callbacks As Callback()) Implements CallbackHandler.handle
				Try
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'					java.security.AccessController.doPrivileged(New java.security.PrivilegedExceptionAction<Void>()
	'				{
	'					public Void run() throws java.io.IOException, UnsupportedCallbackException
	'					{
	'						ch.handle(callbacks);
	'						Return Nothing;
	'					}
	'				}, acc);
				Catch pae As java.security.PrivilegedActionException
					If TypeOf pae.exception Is java.io.IOException Then
						Throw CType(pae.exception, java.io.IOException)
					Else
						Throw CType(pae.exception, UnsupportedCallbackException)
					End If
				End Try
			End Sub
		End Class

		''' <summary>
		''' LoginModule information -
		'''          incapsulates Configuration info and actual module instances
		''' </summary>
		Private Class ModuleInfo
			Friend entry As AppConfigurationEntry
			Friend [module] As Object

			Friend Sub New(ByVal newEntry As AppConfigurationEntry, ByVal newModule As Object)
				Me.entry = newEntry
				Me.module = newModule
			End Sub
		End Class
	End Class

End Namespace
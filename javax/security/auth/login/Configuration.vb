Imports Microsoft.VisualBasic
Imports System

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
	''' A Configuration object is responsible for specifying which LoginModules
	''' should be used for a particular application, and in what order the
	''' LoginModules should be invoked.
	''' 
	''' <p> A login configuration contains the following information.
	''' Note that this example only represents the default syntax for the
	''' {@code Configuration}.  Subclass implementations of this class
	''' may implement alternative syntaxes and may retrieve the
	''' {@code Configuration} from any source such as files, databases,
	''' or servers.
	''' 
	''' <pre>
	'''      Name {
	'''            ModuleClass  Flag    ModuleOptions;
	'''            ModuleClass  Flag    ModuleOptions;
	'''            ModuleClass  Flag    ModuleOptions;
	'''      };
	'''      Name {
	'''            ModuleClass  Flag    ModuleOptions;
	'''            ModuleClass  Flag    ModuleOptions;
	'''      };
	'''      other {
	'''            ModuleClass  Flag    ModuleOptions;
	'''            ModuleClass  Flag    ModuleOptions;
	'''      };
	''' </pre>
	''' 
	''' <p> Each entry in the {@code Configuration} is indexed via an
	''' application name, <i>Name</i>, and contains a list of
	''' LoginModules configured for that application.  Each {@code LoginModule}
	''' is specified via its fully qualified class name.
	''' Authentication proceeds down the module list in the exact order specified.
	''' If an application does not have a specific entry,
	''' it defaults to the specific entry for "<i>other</i>".
	''' 
	''' <p> The <i>Flag</i> value controls the overall behavior as authentication
	''' proceeds down the stack.  The following represents a description of the
	''' valid values for <i>Flag</i> and their respective semantics:
	''' 
	''' <pre>
	'''      1) Required     - The {@code LoginModule} is required to succeed.
	'''                      If it succeeds or fails, authentication still continues
	'''                      to proceed down the {@code LoginModule} list.
	''' 
	'''      2) Requisite    - The {@code LoginModule} is required to succeed.
	'''                      If it succeeds, authentication continues down the
	'''                      {@code LoginModule} list.  If it fails,
	'''                      control immediately returns to the application
	'''                      (authentication does not proceed down the
	'''                      {@code LoginModule} list).
	''' 
	'''      3) Sufficient   - The {@code LoginModule} is not required to
	'''                      succeed.  If it does succeed, control immediately
	'''                      returns to the application (authentication does not
	'''                      proceed down the {@code LoginModule} list).
	'''                      If it fails, authentication continues down the
	'''                      {@code LoginModule} list.
	''' 
	'''      4) Optional     - The {@code LoginModule} is not required to
	'''                      succeed.  If it succeeds or fails,
	'''                      authentication still continues to proceed down the
	'''                      {@code LoginModule} list.
	''' </pre>
	''' 
	''' <p> The overall authentication succeeds only if all <i>Required</i> and
	''' <i>Requisite</i> LoginModules succeed.  If a <i>Sufficient</i>
	''' {@code LoginModule} is configured and succeeds,
	''' then only the <i>Required</i> and <i>Requisite</i> LoginModules prior to
	''' that <i>Sufficient</i> {@code LoginModule} need to have succeeded for
	''' the overall authentication to succeed. If no <i>Required</i> or
	''' <i>Requisite</i> LoginModules are configured for an application,
	''' then at least one <i>Sufficient</i> or <i>Optional</i>
	''' {@code LoginModule} must succeed.
	''' 
	''' <p> <i>ModuleOptions</i> is a space separated list of
	''' {@code LoginModule}-specific values which are passed directly to
	''' the underlying LoginModules.  Options are defined by the
	''' {@code LoginModule} itself, and control the behavior within it.
	''' For example, a {@code LoginModule} may define options to support
	''' debugging/testing capabilities.  The correct way to specify options in the
	''' {@code Configuration} is by using the following key-value pairing:
	''' <i>debug="true"</i>.  The key and value should be separated by an
	''' 'equals' symbol, and the value should be surrounded by double quotes.
	''' If a String in the form, ${system.property}, occurs in the value,
	''' it will be expanded to the value of the system property.
	''' Note that there is no limit to the number of
	''' options a {@code LoginModule} may define.
	''' 
	''' <p> The following represents an example {@code Configuration} entry
	''' based on the syntax above:
	''' 
	''' <pre>
	''' Login {
	'''   com.sun.security.auth.module.UnixLoginModule required;
	'''   com.sun.security.auth.module.Krb5LoginModule optional
	'''                   useTicketCache="true"
	'''                   ticketCache="${user.home}${/}tickets";
	''' };
	''' </pre>
	''' 
	''' <p> This {@code Configuration} specifies that an application named,
	''' "Login", requires users to first authenticate to the
	''' <i>com.sun.security.auth.module.UnixLoginModule</i>, which is
	''' required to succeed.  Even if the <i>UnixLoginModule</i>
	''' authentication fails, the
	''' <i>com.sun.security.auth.module.Krb5LoginModule</i>
	''' still gets invoked.  This helps hide the source of failure.
	''' Since the <i>Krb5LoginModule</i> is <i>Optional</i>, the overall
	''' authentication succeeds only if the <i>UnixLoginModule</i>
	''' (<i>Required</i>) succeeds.
	''' 
	''' <p> Also note that the LoginModule-specific options,
	''' <i>useTicketCache="true"</i> and
	''' <i>ticketCache=${user.home}${/}tickets"</i>,
	''' are passed to the <i>Krb5LoginModule</i>.
	''' These options instruct the <i>Krb5LoginModule</i> to
	''' use the ticket cache at the specified location.
	''' The system properties, <i>user.home</i> and <i>/</i>
	''' (file.separator), are expanded to their respective values.
	''' 
	''' <p> There is only one Configuration object installed in the runtime at any
	''' given time.  A Configuration object can be installed by calling the
	''' {@code setConfiguration} method.  The installed Configuration object
	''' can be obtained by calling the {@code getConfiguration} method.
	''' 
	''' <p> If no Configuration object has been installed in the runtime, a call to
	''' {@code getConfiguration} installs an instance of the default
	''' Configuration implementation (a default subclass implementation of this
	''' abstract class).
	''' The default Configuration implementation can be changed by setting the value
	''' of the {@code login.configuration.provider} security property to the fully
	''' qualified name of the desired Configuration subclass implementation.
	''' 
	''' <p> Application code can directly subclass Configuration to provide a custom
	''' implementation.  In addition, an instance of a Configuration object can be
	''' constructed by invoking one of the {@code getInstance} factory methods
	''' with a standard type.  The default policy type is "JavaLoginConfig".
	''' See the Configuration section in the <a href=
	''' "{@docRoot}/../technotes/guides/security/StandardNames.html#Configuration">
	''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
	''' for a list of standard Configuration types.
	''' </summary>
	''' <seealso cref= javax.security.auth.login.LoginContext </seealso>
	''' <seealso cref= java.security.Security security properties </seealso>
	Public MustInherit Class Configuration

		Private Shared ___configuration As Configuration

		Private ReadOnly acc As java.security.AccessControlContext = java.security.AccessController.context

		Private Shared Sub checkPermission(ByVal type As String)
			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then sm.checkPermission(New javax.security.auth.AuthPermission("createLoginConfiguration." & type))
		End Sub

		''' <summary>
		''' Sole constructor.  (For invocation by subclass constructors, typically
		''' implicit.)
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Get the installed login Configuration.
		''' 
		''' <p>
		''' </summary>
		''' <returns> the login Configuration.  If a Configuration object was set
		'''          via the {@code Configuration.setConfiguration} method,
		'''          then that object is returned.  Otherwise, a default
		'''          Configuration object is returned.
		''' </returns>
		''' <exception cref="SecurityException"> if the caller does not have permission
		'''                          to retrieve the Configuration.
		''' </exception>
		''' <seealso cref= #setConfiguration </seealso>
		Public Property Shared configuration As Configuration
			Get
    
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPermission(New javax.security.auth.AuthPermission("getLoginConfiguration"))
    
				SyncLock GetType(Configuration)
					If ___configuration Is Nothing Then
						Dim config_class As String = Nothing
	'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
	'					config_class = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<String>()
		'				{
		'					public String run()
		'					{
		'						Return java.security.Security.getProperty("login.configuration.provider");
		'					}
		'				});
						If config_class Is Nothing Then config_class = "sun.security.provider.ConfigFile"
    
						Try
							Dim finalClass As String = config_class
	'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
	'						Configuration untrustedImpl = java.security.AccessController.doPrivileged(New java.security.PrivilegedExceptionAction<Configuration>()
		'					{
		'								public Configuration run() throws ClassNotFoundException, InstantiationException, IllegalAccessException
		'								{
		'									Class implClass = Class.forName(finalClass, False, Thread.currentThread().getContextClassLoader()).asSubclass(Configuration.class);
		'									Return implClass.newInstance();
		'								}
		'							});
	'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
	'						java.security.AccessController.doPrivileged(New java.security.PrivilegedExceptionAction<Void>()
		'					{
		'								public Void run()
		'								{
		'									setConfiguration(untrustedImpl);
		'									Return Nothing;
		'								}
		'							}, Objects.requireNonNull(untrustedImpl.acc)
						   )
						Catch e As java.security.PrivilegedActionException
							Dim ee As Exception = e.exception
							If TypeOf ee Is InstantiationException Then
								Throw CType((New SecurityException("Configuration error:" & ee.InnerException.Message & vbLf)).initCause(ee.InnerException), SecurityException)
							Else
								Throw CType((New SecurityException("Configuration error: " & ee.ToString() & vbLf)).initCause(ee), SecurityException)
							End If
						End Try
					End If
					Return ___configuration
				End SyncLock
			End Get
			Set(ByVal ___configuration As Configuration)
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPermission(New javax.security.auth.AuthPermission("setLoginConfiguration"))
				Configuration.___configuration = ___configuration
			End Set
		End Property


		''' <summary>
		''' Returns a Configuration object of the specified type.
		''' 
		''' <p> This method traverses the list of registered security providers,
		''' starting with the most preferred Provider.
		''' A new Configuration object encapsulating the
		''' ConfigurationSpi implementation from the first
		''' Provider that supports the specified type is returned.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="type"> the specified Configuration type.  See the Configuration
		'''    section in the <a href=
		'''    "{@docRoot}/../technotes/guides/security/StandardNames.html#Configuration">
		'''    Java Cryptography Architecture Standard Algorithm Name
		'''    Documentation</a> for a list of standard Configuration types.
		''' </param>
		''' <param name="params"> parameters for the Configuration, which may be null.
		''' </param>
		''' <returns> the new Configuration object.
		''' </returns>
		''' <exception cref="SecurityException"> if the caller does not have permission
		'''          to get a Configuration instance for the specified type.
		''' </exception>
		''' <exception cref="NullPointerException"> if the specified type is null.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the specified parameters
		'''          are not understood by the ConfigurationSpi implementation
		'''          from the selected Provider.
		''' </exception>
		''' <exception cref="NoSuchAlgorithmException"> if no Provider supports a
		'''          ConfigurationSpi implementation for the specified type.
		''' </exception>
		''' <seealso cref= Provider
		''' @since 1.6 </seealso>
		Public Shared Function getInstance(ByVal type As String, ByVal params As Configuration.Parameters) As Configuration

			checkPermission(type)
			Try
				Dim ___instance As sun.security.jca.GetInstance.Instance = sun.security.jca.GetInstance.getInstance("Configuration", GetType(ConfigurationSpi), type, params)
				Return New ConfigDelegate(CType(___instance.impl, ConfigurationSpi), ___instance.provider, type, params)
			Catch nsae As java.security.NoSuchAlgorithmException
				Return handleException(nsae)
			End Try
		End Function

		''' <summary>
		''' Returns a Configuration object of the specified type.
		''' 
		''' <p> A new Configuration object encapsulating the
		''' ConfigurationSpi implementation from the specified provider
		''' is returned.   The specified provider must be registered
		''' in the provider list.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="type"> the specified Configuration type.  See the Configuration
		'''    section in the <a href=
		'''    "{@docRoot}/../technotes/guides/security/StandardNames.html#Configuration">
		'''    Java Cryptography Architecture Standard Algorithm Name
		'''    Documentation</a> for a list of standard Configuration types.
		''' </param>
		''' <param name="params"> parameters for the Configuration, which may be null.
		''' </param>
		''' <param name="provider"> the provider.
		''' </param>
		''' <returns> the new Configuration object.
		''' </returns>
		''' <exception cref="SecurityException"> if the caller does not have permission
		'''          to get a Configuration instance for the specified type.
		''' </exception>
		''' <exception cref="NullPointerException"> if the specified type is null.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the specified provider
		'''          is null or empty,
		'''          or if the specified parameters are not understood by
		'''          the ConfigurationSpi implementation from the specified provider.
		''' </exception>
		''' <exception cref="NoSuchProviderException"> if the specified provider is not
		'''          registered in the security provider list.
		''' </exception>
		''' <exception cref="NoSuchAlgorithmException"> if the specified provider does not
		'''          support a ConfigurationSpi implementation for the specified
		'''          type.
		''' </exception>
		''' <seealso cref= Provider
		''' @since 1.6 </seealso>
		Public Shared Function getInstance(ByVal type As String, ByVal params As Configuration.Parameters, ByVal provider As String) As Configuration

			If provider Is Nothing OrElse provider.Length = 0 Then Throw New System.ArgumentException("missing provider")

			checkPermission(type)
			Try
				Dim ___instance As sun.security.jca.GetInstance.Instance = sun.security.jca.GetInstance.getInstance("Configuration", GetType(ConfigurationSpi), type, params, provider)
				Return New ConfigDelegate(CType(___instance.impl, ConfigurationSpi), ___instance.provider, type, params)
			Catch nsae As java.security.NoSuchAlgorithmException
				Return handleException(nsae)
			End Try
		End Function

		''' <summary>
		''' Returns a Configuration object of the specified type.
		''' 
		''' <p> A new Configuration object encapsulating the
		''' ConfigurationSpi implementation from the specified Provider
		''' object is returned.  Note that the specified Provider object
		''' does not have to be registered in the provider list.
		''' </summary>
		''' <param name="type"> the specified Configuration type.  See the Configuration
		'''    section in the <a href=
		'''    "{@docRoot}/../technotes/guides/security/StandardNames.html#Configuration">
		'''    Java Cryptography Architecture Standard Algorithm Name
		'''    Documentation</a> for a list of standard Configuration types.
		''' </param>
		''' <param name="params"> parameters for the Configuration, which may be null.
		''' </param>
		''' <param name="provider"> the Provider.
		''' </param>
		''' <returns> the new Configuration object.
		''' </returns>
		''' <exception cref="SecurityException"> if the caller does not have permission
		'''          to get a Configuration instance for the specified type.
		''' </exception>
		''' <exception cref="NullPointerException"> if the specified type is null.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the specified Provider is null,
		'''          or if the specified parameters are not understood by
		'''          the ConfigurationSpi implementation from the specified Provider.
		''' </exception>
		''' <exception cref="NoSuchAlgorithmException"> if the specified Provider does not
		'''          support a ConfigurationSpi implementation for the specified
		'''          type.
		''' </exception>
		''' <seealso cref= Provider
		''' @since 1.6 </seealso>
		Public Shared Function getInstance(ByVal type As String, ByVal params As Configuration.Parameters, ByVal provider As java.security.Provider) As Configuration

			If provider Is Nothing Then Throw New System.ArgumentException("missing provider")

			checkPermission(type)
			Try
				Dim ___instance As sun.security.jca.GetInstance.Instance = sun.security.jca.GetInstance.getInstance("Configuration", GetType(ConfigurationSpi), type, params, provider)
				Return New ConfigDelegate(CType(___instance.impl, ConfigurationSpi), ___instance.provider, type, params)
			Catch nsae As java.security.NoSuchAlgorithmException
				Return handleException(nsae)
			End Try
		End Function

		Private Shared Function handleException(ByVal nsae As java.security.NoSuchAlgorithmException) As Configuration
			Dim cause As Exception = nsae.InnerException
			If TypeOf cause Is System.ArgumentException Then Throw CType(cause, System.ArgumentException)
			Throw nsae
		End Function

		''' <summary>
		''' Return the Provider of this Configuration.
		''' 
		''' <p> This Configuration instance will only have a Provider if it
		''' was obtained via a call to {@code Configuration.getInstance}.
		''' Otherwise this method returns null.
		''' </summary>
		''' <returns> the Provider of this Configuration, or null.
		''' 
		''' @since 1.6 </returns>
		Public Overridable Property provider As java.security.Provider
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Return the type of this Configuration.
		''' 
		''' <p> This Configuration instance will only have a type if it
		''' was obtained via a call to {@code Configuration.getInstance}.
		''' Otherwise this method returns null.
		''' </summary>
		''' <returns> the type of this Configuration, or null.
		''' 
		''' @since 1.6 </returns>
		Public Overridable Property type As String
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Return Configuration parameters.
		''' 
		''' <p> This Configuration instance will only have parameters if it
		''' was obtained via a call to {@code Configuration.getInstance}.
		''' Otherwise this method returns null.
		''' </summary>
		''' <returns> Configuration parameters, or null.
		''' 
		''' @since 1.6 </returns>
		Public Overridable Property parameters As Configuration.Parameters
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Retrieve the AppConfigurationEntries for the specified <i>name</i>
		''' from this Configuration.
		''' 
		''' <p>
		''' </summary>
		''' <param name="name"> the name used to index the Configuration.
		''' </param>
		''' <returns> an array of AppConfigurationEntries for the specified <i>name</i>
		'''          from this Configuration, or null if there are no entries
		'''          for the specified <i>name</i> </returns>
		Public MustOverride Function getAppConfigurationEntry(ByVal name As String) As AppConfigurationEntry()

		''' <summary>
		''' Refresh and reload the Configuration.
		''' 
		''' <p> This method causes this Configuration object to refresh/reload its
		''' contents in an implementation-dependent manner.
		''' For example, if this Configuration object stores its entries in a file,
		''' calling {@code refresh} may cause the file to be re-read.
		''' 
		''' <p> The default implementation of this method does nothing.
		''' This method should be overridden if a refresh operation is supported
		''' by the implementation.
		''' </summary>
		''' <exception cref="SecurityException"> if the caller does not have permission
		'''                          to refresh its Configuration. </exception>
		Public Overridable Sub refresh()
		End Sub

		''' <summary>
		''' This subclass is returned by the getInstance calls.  All Configuration
		''' calls are delegated to the underlying ConfigurationSpi.
		''' </summary>
		Private Class ConfigDelegate
			Inherits Configuration

			Private spi As ConfigurationSpi
			Private p As java.security.Provider
			Private type As String
			Private params As Configuration.Parameters

			Private Sub New(ByVal spi As ConfigurationSpi, ByVal p As java.security.Provider, ByVal type As String, ByVal params As Configuration.Parameters)
				Me.spi = spi
				Me.p = p
				Me.type = type
				Me.params = params
			End Sub

			Public Property Overrides type As String
				Get
					Return type
				End Get
			End Property

			Public Property Overrides parameters As Configuration.Parameters
				Get
					Return params
				End Get
			End Property

			Public Property Overrides provider As java.security.Provider
				Get
					Return p
				End Get
			End Property

			Public Overrides Function getAppConfigurationEntry(ByVal name As String) As AppConfigurationEntry()
				Return spi.engineGetAppConfigurationEntry(name)
			End Function

			Public Overrides Sub refresh()
				spi.engineRefresh()
			End Sub
		End Class

		''' <summary>
		''' This represents a marker interface for Configuration parameters.
		''' 
		''' @since 1.6
		''' </summary>
		Public Interface Parameters
		End Interface
	End Class

End Namespace
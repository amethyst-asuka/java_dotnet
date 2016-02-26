Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports javax.naming

'
' * Copyright (c) 1999, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.naming.spi



	''' <summary>
	''' This class contains methods for creating context objects
	''' and objects referred to by location information in the naming
	''' or directory service.
	''' <p>
	''' This class cannot be instantiated.  It has only static methods.
	''' <p>
	''' The mention of URL in the documentation for this class refers to
	''' a URL string as defined by RFC 1738 and its related RFCs. It is
	''' any string that conforms to the syntax described therein, and
	''' may not always have corresponding support in the java.net.URL
	''' class or Web browsers.
	''' <p>
	''' NamingManager is safe for concurrent access by multiple threads.
	''' <p>
	''' Except as otherwise noted,
	''' a <tt>Name</tt> or environment parameter
	''' passed to any method is owned by the caller.
	''' The implementation will not modify the object or keep a reference
	''' to it, although it may keep a reference to a clone or copy.
	''' 
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>

	Public Class NamingManager

	'    
	'     * Disallow anyone from creating one of these.
	'     * Made package private so that DirectoryManager can subclass.
	'     

		Friend Sub New()
		End Sub

		' should be protected and package private
		Friend Shared ReadOnly helper As com.sun.naming.internal.VersionHelper = com.sun.naming.internal.VersionHelper.versionHelper

	' --------- object factory stuff

		''' <summary>
		''' Package-private; used by DirectoryManager and NamingManager.
		''' </summary>
		Private Shared object_factory_builder As ObjectFactoryBuilder = Nothing

		''' <summary>
		''' The ObjectFactoryBuilder determines the policy used when
		''' trying to load object factories.
		''' See getObjectInstance() and class ObjectFactory for a description
		''' of the default policy.
		''' setObjectFactoryBuilder() overrides this default policy by installing
		''' an ObjectFactoryBuilder. Subsequent object factories will
		''' be loaded and created using the installed builder.
		''' <p>
		''' The builder can only be installed if the executing thread is allowed
		''' (by the security manager's checkSetFactory() method) to do so.
		''' Once installed, the builder cannot be replaced.
		''' <p> </summary>
		''' <param name="builder"> The factory builder to install. If null, no builder
		'''                  is installed. </param>
		''' <exception cref="SecurityException"> builder cannot be installed
		'''          for security reasons. </exception>
		''' <exception cref="NamingException"> builder cannot be installed for
		'''         a non-security-related reason. </exception>
		''' <exception cref="IllegalStateException"> If a factory has already been installed. </exception>
		''' <seealso cref= #getObjectInstance </seealso>
		''' <seealso cref= ObjectFactory </seealso>
		''' <seealso cref= ObjectFactoryBuilder </seealso>
		''' <seealso cref= java.lang.SecurityManager#checkSetFactory </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Property objectFactoryBuilder As ObjectFactoryBuilder
			Set(ByVal builder As ObjectFactoryBuilder)
				If object_factory_builder IsNot Nothing Then Throw New IllegalStateException("ObjectFactoryBuilder already set")
    
				Dim security As SecurityManager = System.securityManager
				If security IsNot Nothing Then security.checkSetFactory()
				object_factory_builder = builder
			End Set
			Get
				Return object_factory_builder
			End Get
		End Property



		''' <summary>
		''' Retrieves the ObjectFactory for the object identified by a reference,
		''' using the reference's factory class name and factory codebase
		''' to load in the factory's class. </summary>
		''' <param name="ref"> The non-null reference to use. </param>
		''' <param name="factoryName"> The non-null class name of the factory. </param>
		''' <returns> The object factory for the object identified by ref; null
		''' if unable to load the factory. </returns>
		Friend Shared Function getObjectFactoryFromReference(ByVal ref As Reference, ByVal factoryName As String) As ObjectFactory
			Dim clas As Type = Nothing

			' Try to use current class loader
			Try
				 clas = helper.loadClass(factoryName)
			Catch e As ClassNotFoundException
				' ignore and continue
				' e.printStackTrace();
			End Try
			' All other exceptions are passed up.

			' Not in class path; try to use codebase
			Dim codebase As String
			codebase = ref.factoryClassLocation
			If clas Is Nothing AndAlso codebase IsNot Nothing Then
				Try
					clas = helper.loadClass(factoryName, codebase)
				Catch e As ClassNotFoundException
				End Try
			End If

			Return If(clas IsNot Nothing, CType(clas.newInstance(), ObjectFactory), Nothing)
		End Function


		''' <summary>
		''' Creates an object using the factories specified in the
		''' <tt>Context.OBJECT_FACTORIES</tt> property of the environment
		''' or of the provider resource file associated with <tt>nameCtx</tt>.
		''' </summary>
		''' <returns> factory created; null if cannot create </returns>
		Private Shared Function createObjectFromFactories(Of T1)(ByVal obj As Object, ByVal name As Name, ByVal nameCtx As Context, ByVal environment As Dictionary(Of T1)) As Object

			Dim factories As com.sun.naming.internal.FactoryEnumeration = com.sun.naming.internal.ResourceManager.getFactories(Context.OBJECT_FACTORIES, environment, nameCtx)

			If factories Is Nothing Then Return Nothing

			' Try each factory until one succeeds
			Dim factory As ObjectFactory
			Dim answer As Object = Nothing
			Do While answer Is Nothing AndAlso factories.hasMore()
				factory = CType(factories.next(), ObjectFactory)
				answer = factory.getObjectInstance(obj, name, nameCtx, environment)
			Loop
			Return answer
		End Function

		Private Shared Function getURLScheme(ByVal str As String) As String
			Dim colon_posn As Integer = str.IndexOf(":"c)
			Dim slash_posn As Integer = str.IndexOf("/"c)

			If colon_posn > 0 AndAlso (slash_posn = -1 OrElse colon_posn < slash_posn) Then Return str.Substring(0, colon_posn)
			Return Nothing
		End Function

		''' <summary>
		''' Creates an instance of an object for the specified object
		''' and environment.
		''' <p>
		''' If an object factory builder has been installed, it is used to
		''' create a factory for creating the object.
		''' Otherwise, the following rules are used to create the object:
		''' <ol>
		''' <li>If <code>refInfo</code> is a <code>Reference</code>
		'''    or <code>Referenceable</code> containing a factory class name,
		'''    use the named factory to create the object.
		'''    Return <code>refInfo</code> if the factory cannot be created.
		'''    Under JDK 1.1, if the factory class must be loaded from a location
		'''    specified in the reference, a <tt>SecurityManager</tt> must have
		'''    been installed or the factory creation will fail.
		'''    If an exception is encountered while creating the factory,
		'''    it is passed up to the caller.
		''' <li>If <tt>refInfo</tt> is a <tt>Reference</tt> or
		'''    <tt>Referenceable</tt> with no factory class name,
		'''    and the address or addresses are <tt>StringRefAddr</tt>s with
		'''    address type "URL",
		'''    try the URL context factory corresponding to each URL's scheme id
		'''    to create the object (see <tt>getURLContext()</tt>).
		'''    If that fails, continue to the next step.
		''' <li> Use the object factories specified in
		'''    the <tt>Context.OBJECT_FACTORIES</tt> property of the environment,
		'''    and of the provider resource file associated with
		'''    <tt>nameCtx</tt>, in that order.
		'''    The value of this property is a colon-separated list of factory
		'''    class names that are tried in order, and the first one that succeeds
		'''    in creating an object is the one used.
		'''    If none of the factories can be loaded,
		'''    return <code>refInfo</code>.
		'''    If an exception is encountered while creating the object, the
		'''    exception is passed up to the caller.
		''' </ol>
		''' <p>
		''' Service providers that implement the <tt>DirContext</tt>
		''' interface should use
		''' <tt>DirectoryManager.getObjectInstance()</tt>, not this method.
		''' Service providers that implement only the <tt>Context</tt>
		''' interface should use this method.
		''' <p>
		''' Note that an object factory (an object that implements the ObjectFactory
		''' interface) must be public and must have a public constructor that
		''' accepts no arguments.
		''' <p>
		''' The <code>name</code> and <code>nameCtx</code> parameters may
		''' optionally be used to specify the name of the object being created.
		''' <code>name</code> is the name of the object, relative to context
		''' <code>nameCtx</code>.  This information could be useful to the object
		''' factory or to the object implementation.
		'''  If there are several possible contexts from which the object
		'''  could be named -- as will often be the case -- it is up to
		'''  the caller to select one.  A good rule of thumb is to select the
		''' "deepest" context available.
		''' If <code>nameCtx</code> is null, <code>name</code> is relative
		''' to the default initial context.  If no name is being specified, the
		''' <code>name</code> parameter should be null.
		''' </summary>
		''' <param name="refInfo"> The possibly null object for which to create an object. </param>
		''' <param name="name"> The name of this object relative to <code>nameCtx</code>.
		'''          Specifying a name is optional; if it is
		'''          omitted, <code>name</code> should be null. </param>
		''' <param name="nameCtx"> The context relative to which the <code>name</code>
		'''          parameter is specified.  If null, <code>name</code> is
		'''          relative to the default initial context. </param>
		''' <param name="environment"> The possibly null environment to
		'''          be used in the creation of the object factory and the object. </param>
		''' <returns> An object created using <code>refInfo</code>; or
		'''          <code>refInfo</code> if an object cannot be created using
		'''          the algorithm described above. </returns>
		''' <exception cref="NamingException"> if a naming exception was encountered
		'''  while attempting to get a URL context, or if one of the
		'''          factories accessed throws a NamingException. </exception>
		''' <exception cref="Exception"> if one of the factories accessed throws an
		'''          exception, or if an error was encountered while loading
		'''          and instantiating the factory and object classes.
		'''          A factory should only throw an exception if it does not want
		'''          other factories to be used in an attempt to create an object.
		'''  See ObjectFactory.getObjectInstance(). </exception>
		''' <seealso cref= #getURLContext </seealso>
		''' <seealso cref= ObjectFactory </seealso>
		''' <seealso cref= ObjectFactory#getObjectInstance </seealso>
		Public Shared Function getObjectInstance(Of T1)(ByVal refInfo As Object, ByVal name As Name, ByVal nameCtx As Context, ByVal environment As Dictionary(Of T1)) As Object

			Dim factory As ObjectFactory

			' Use builder if installed
			Dim builder As ObjectFactoryBuilder = objectFactoryBuilder
			If builder IsNot Nothing Then
				' builder must return non-null factory
				factory = builder.createObjectFactory(refInfo, environment)
				Return factory.getObjectInstance(refInfo, name, nameCtx, environment)
			End If

			' Use reference if possible
			Dim ref As Reference = Nothing
			If TypeOf refInfo Is Reference Then
				ref = CType(refInfo, Reference)
			ElseIf TypeOf refInfo Is Referenceable Then
				ref = CType(refInfo, Referenceable).reference
			End If

			Dim answer As Object

			If ref IsNot Nothing Then
				Dim f As String = ref.factoryClassName
				If f IsNot Nothing Then
					' if reference identifies a factory, use exclusively

					factory = getObjectFactoryFromReference(ref, f)
					If factory IsNot Nothing Then Return factory.getObjectInstance(ref, name, nameCtx, environment)
					' No factory found, so return original refInfo.
					' Will reach this point if factory class is not in
					' class path and reference does not contain a URL for it
					Return refInfo

				Else
					' if reference has no factory, check for addresses
					' containing URLs

					answer = processURLAddrs(ref, name, nameCtx, environment)
					If answer IsNot Nothing Then Return answer
				End If
			End If

			' try using any specified factories
			answer = createObjectFromFactories(refInfo, name, nameCtx, environment)
			Return If(answer IsNot Nothing, answer, refInfo)
		End Function

	'    
	'     * Ref has no factory.  For each address of type "URL", try its URL
	'     * context factory.  Returns null if unsuccessful in creating and
	'     * invoking a factory.
	'     
		Friend Shared Function processURLAddrs(Of T1)(ByVal ref As Reference, ByVal name As Name, ByVal nameCtx As Context, ByVal environment As Dictionary(Of T1)) As Object

			For i As Integer = 0 To ref.size() - 1
				Dim addr As RefAddr = ref.get(i)
				If TypeOf addr Is StringRefAddr AndAlso addr.type.ToUpper() = "URL".ToUpper() Then

					Dim url As String = CStr(addr.content)
					Dim answer As Object = processURL(url, name, nameCtx, environment)
					If answer IsNot Nothing Then Return answer
				End If
			Next i
			Return Nothing
		End Function

		Private Shared Function processURL(Of T1)(ByVal refInfo As Object, ByVal name As Name, ByVal nameCtx As Context, ByVal environment As Dictionary(Of T1)) As Object
			Dim answer As Object

			' If refInfo is a URL string, try to use its URL context factory
			' If no context found, continue to try object factories.
			If TypeOf refInfo Is String Then
				Dim url As String = CStr(refInfo)
				Dim scheme As String = getURLScheme(url)
				If scheme IsNot Nothing Then
					answer = getURLObject(scheme, refInfo, name, nameCtx, environment)
					If answer IsNot Nothing Then Return answer
				End If
			End If

			' If refInfo is an array of URL strings,
			' try to find a context factory for any one of its URLs.
			' If no context found, continue to try object factories.
			If TypeOf refInfo Is String() Then
				Dim urls As String() = CType(refInfo, String())
				For i As Integer = 0 To urls.Length - 1
					Dim scheme As String = getURLScheme(urls(i))
					If scheme IsNot Nothing Then
						answer = getURLObject(scheme, refInfo, name, nameCtx, environment)
						If answer IsNot Nothing Then Return answer
					End If
				Next i
			End If
			Return Nothing
		End Function


		''' <summary>
		''' Retrieves a context identified by <code>obj</code>, using the specified
		''' environment.
		''' Used by ContinuationContext.
		''' </summary>
		''' <param name="obj">       The object identifying the context. </param>
		''' <param name="name">      The name of the context being returned, relative to
		'''                  <code>nameCtx</code>, or null if no name is being
		'''                  specified.
		'''                  See the <code>getObjectInstance</code> method for
		'''                  details. </param>
		''' <param name="nameCtx">   The context relative to which <code>name</code> is
		'''                  specified, or null for the default initial context.
		'''                  See the <code>getObjectInstance</code> method for
		'''                  details. </param>
		''' <param name="environment"> Environment specifying characteristics of the
		'''                  resulting context. </param>
		''' <returns> A context identified by <code>obj</code>.
		''' </returns>
		''' <seealso cref= #getObjectInstance </seealso>
		Friend Shared Function getContext(Of T1)(ByVal obj As Object, ByVal name As Name, ByVal nameCtx As Context, ByVal environment As Dictionary(Of T1)) As Context
			Dim answer As Object

			If TypeOf obj Is Context Then Return CType(obj, Context)

			Try
				answer = getObjectInstance(obj, name, nameCtx, environment)
			Catch e As NamingException
				Throw e
			Catch e As Exception
				Dim ne As New NamingException
				ne.rootCause = e
				Throw ne
			End Try

			Return If(TypeOf answer Is Context, CType(answer, Context), Nothing)
		End Function

		' Used by ContinuationContext
		Friend Shared Function getResolver(Of T1)(ByVal obj As Object, ByVal name As Name, ByVal nameCtx As Context, ByVal environment As Dictionary(Of T1)) As Resolver
			Dim answer As Object

			If TypeOf obj Is Resolver Then Return CType(obj, Resolver)

			Try
				answer = getObjectInstance(obj, name, nameCtx, environment)
			Catch e As NamingException
				Throw e
			Catch e As Exception
				Dim ne As New NamingException
				ne.rootCause = e
				Throw ne
			End Try

			Return If(TypeOf answer Is Resolver, CType(answer, Resolver), Nothing)
		End Function


		''' <summary>
		'''*************** URL Context implementations ************** </summary>

		''' <summary>
		''' Creates a context for the given URL scheme id.
		''' <p>
		''' The resulting context is for resolving URLs of the
		''' scheme <code>scheme</code>. The resulting context is not tied
		''' to a specific URL. It is able to handle arbitrary URLs with
		''' the specified scheme.
		''' <p>
		''' The class name of the factory that creates the resulting context
		''' has the naming convention <i>scheme-id</i>URLContextFactory
		''' (e.g. "ftpURLContextFactory" for the "ftp" scheme-id),
		''' in the package specified as follows.
		''' The <tt>Context.URL_PKG_PREFIXES</tt> environment property (which
		''' may contain values taken from applet parameters, system properties,
		''' or application resource files)
		''' contains a colon-separated list of package prefixes.
		''' Each package prefix in
		''' the property is tried in the order specified to load the factory class.
		''' The default package prefix is "com.sun.jndi.url" (if none of the
		''' specified packages work, this default is tried).
		''' The complete package name is constructed using the package prefix,
		''' concatenated with the scheme id.
		''' <p>
		''' For example, if the scheme id is "ldap", and the
		''' <tt>Context.URL_PKG_PREFIXES</tt> property
		''' contains "com.widget:com.wiz.jndi",
		''' the naming manager would attempt to load the following classes
		''' until one is successfully instantiated:
		''' <ul>
		''' <li>com.widget.ldap.ldapURLContextFactory
		'''  <li>com.wiz.jndi.ldap.ldapURLContextFactory
		'''  <li>com.sun.jndi.url.ldap.ldapURLContextFactory
		''' </ul>
		''' If none of the package prefixes work, null is returned.
		''' <p>
		''' If a factory is instantiated, it is invoked with the following
		''' parameters to produce the resulting context.
		''' <p>
		''' <code>factory.getObjectInstance(null, environment);</code>
		''' <p>
		''' For example, invoking getObjectInstance() as shown above
		''' on a LDAP URL context factory would return a
		''' context that can resolve LDAP urls
		''' (e.g. "ldap://ldap.wiz.com/o=wiz,c=us",
		''' "ldap://ldap.umich.edu/o=umich,c=us", ...).
		''' <p>
		''' Note that an object factory (an object that implements the ObjectFactory
		''' interface) must be public and must have a public constructor that
		''' accepts no arguments.
		''' </summary>
		''' <param name="scheme">    The non-null scheme-id of the URLs supported by the context. </param>
		''' <param name="environment"> The possibly null environment properties to be
		'''           used in the creation of the object factory and the context. </param>
		''' <returns> A context for resolving URLs with the
		'''         scheme id <code>scheme</code>;
		'''  <code>null</code> if the factory for creating the
		'''         context is not found. </returns>
		''' <exception cref="NamingException"> If a naming exception occurs while creating
		'''          the context. </exception>
		''' <seealso cref= #getObjectInstance </seealso>
		''' <seealso cref= ObjectFactory#getObjectInstance </seealso>
		Public Shared Function getURLContext(Of T1)(ByVal scheme As String, ByVal environment As Dictionary(Of T1)) As Context
			' pass in 'null' to indicate creation of generic context for scheme
			' (i.e. not specific to a URL).

				Dim answer As Object = getURLObject(scheme, Nothing, Nothing, Nothing, environment)
				If TypeOf answer Is Context Then
					Return CType(answer, Context)
				Else
					Return Nothing
				End If
		End Function

		Private Const defaultPkgPrefix As String = "com.sun.jndi.url"

		''' <summary>
		''' Creates an object for the given URL scheme id using
		''' the supplied urlInfo.
		''' <p>
		''' If urlInfo is null, the result is a context for resolving URLs
		''' with the scheme id 'scheme'.
		''' If urlInfo is a URL, the result is a context named by the URL.
		''' Names passed to this context is assumed to be relative to this
		''' context (i.e. not a URL). For example, if urlInfo is
		''' "ldap://ldap.wiz.com/o=Wiz,c=us", the resulting context will
		''' be that pointed to by "o=Wiz,c=us" on the server 'ldap.wiz.com'.
		''' Subsequent names that can be passed to this context will be
		''' LDAP names relative to this context (e.g. cn="Barbs Jensen").
		''' If urlInfo is an array of URLs, the URLs are assumed
		''' to be equivalent in terms of the context to which they refer.
		''' The resulting context is like that of the single URL case.
		''' If urlInfo is of any other type, that is handled by the
		''' context factory for the URL scheme. </summary>
		''' <param name="scheme"> the URL scheme id for the context </param>
		''' <param name="urlInfo"> information used to create the context </param>
		''' <param name="name"> name of this object relative to <code>nameCtx</code> </param>
		''' <param name="nameCtx"> Context whose provider resource file will be searched
		'''          for package prefix values (or null if none) </param>
		''' <param name="environment"> Environment properties for creating the context </param>
		''' <seealso cref= javax.naming.InitialContext </seealso>
		Private Shared Function getURLObject(Of T1)(ByVal scheme As String, ByVal urlInfo As Object, ByVal name As Name, ByVal nameCtx As Context, ByVal environment As Dictionary(Of T1)) As Object

			' e.g. "ftpURLContextFactory"
			Dim factory As ObjectFactory = CType(com.sun.naming.internal.ResourceManager.getFactory(Context.URL_PKG_PREFIXES, environment, nameCtx, "." & scheme & "." & scheme & "URLContextFactory", defaultPkgPrefix), ObjectFactory)

			If factory Is Nothing Then Return Nothing

			' Found object factory
			Try
				Return factory.getObjectInstance(urlInfo, name, nameCtx, environment)
			Catch e As NamingException
				Throw e
			Catch e As Exception
				Dim ne As New NamingException
				ne.rootCause = e
				Throw ne
			End Try

		End Function


	' ------------ Initial Context Factory Stuff
		Private Shared initctx_factory_builder As InitialContextFactoryBuilder = Nothing

		''' <summary>
		''' Use this method for accessing initctx_factory_builder while
		''' inside an unsynchronized method.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Property Shared initialContextFactoryBuilder As InitialContextFactoryBuilder
			Get
				Return initctx_factory_builder
			End Get
			Set(ByVal builder As InitialContextFactoryBuilder)
					If initctx_factory_builder IsNot Nothing Then Throw New IllegalStateException("InitialContextFactoryBuilder already set")
    
					Dim security As SecurityManager = System.securityManager
					If security IsNot Nothing Then security.checkSetFactory()
					initctx_factory_builder = builder
			End Set
		End Property

		''' <summary>
		''' Creates an initial context using the specified environment
		''' properties.
		''' <p>
		''' If an InitialContextFactoryBuilder has been installed,
		''' it is used to create the factory for creating the initial context.
		''' Otherwise, the class specified in the
		''' <tt>Context.INITIAL_CONTEXT_FACTORY</tt> environment property is used.
		''' Note that an initial context factory (an object that implements the
		''' InitialContextFactory interface) must be public and must have a
		''' public constructor that accepts no arguments.
		''' </summary>
		''' <param name="env"> The possibly null environment properties used when
		'''                  creating the context. </param>
		''' <returns> A non-null initial context. </returns>
		''' <exception cref="NoInitialContextException"> If the
		'''          <tt>Context.INITIAL_CONTEXT_FACTORY</tt> property
		'''         is not found or names a nonexistent
		'''         class or a class that cannot be instantiated,
		'''          or if the initial context could not be created for some other
		'''          reason. </exception>
		''' <exception cref="NamingException"> If some other naming exception was encountered. </exception>
		''' <seealso cref= javax.naming.InitialContext </seealso>
		''' <seealso cref= javax.naming.directory.InitialDirContext </seealso>
		Public Shared Function getInitialContext(Of T1)(ByVal env As Dictionary(Of T1)) As Context
			Dim factory As InitialContextFactory

			Dim builder As InitialContextFactoryBuilder = initialContextFactoryBuilder
			If builder Is Nothing Then
				' No factory installed, use property
				' Get initial context factory class name

				Dim className As String = If(env IsNot Nothing, CStr(env(Context.INITIAL_CONTEXT_FACTORY)), Nothing)
				If className Is Nothing Then
					Dim ne As New NoInitialContextException("Need to specify class name in environment or system " & "property, or as an applet parameter, or in an " & "application resource file:  " & Context.INITIAL_CONTEXT_FACTORY)
					Throw ne
				End If

				Try
					factory = CType(helper.loadClass(className).newInstance(), InitialContextFactory)
				Catch e As Exception
					Dim ne As New NoInitialContextException("Cannot instantiate class: " & className)
					ne.rootCause = e
					Throw ne
				End Try
			Else
				factory = builder.createInitialContextFactory(env)
			End If

			Return factory.getInitialContext(env)
		End Function



		''' <summary>
		''' Determines whether an initial context factory builder has
		''' been set. </summary>
		''' <returns> true if an initial context factory builder has
		'''           been set; false otherwise. </returns>
		''' <seealso cref= #setInitialContextFactoryBuilder </seealso>
		Public Shared Function hasInitialContextFactoryBuilder() As Boolean
			Return (initialContextFactoryBuilder IsNot Nothing)
		End Function

	' -----  Continuation Context Stuff

		''' <summary>
		''' Constant that holds the name of the environment property into
		''' which <tt>getContinuationContext()</tt> stores the value of its
		''' <tt>CannotProceedException</tt> parameter.
		''' This property is inherited by the continuation context, and may
		''' be used by that context's service provider to inspect the
		''' fields of the exception.
		''' <p>
		''' The value of this constant is "java.naming.spi.CannotProceedException".
		''' </summary>
		''' <seealso cref= #getContinuationContext
		''' @since 1.3 </seealso>
		Public Const CPE As String = "java.naming.spi.CannotProceedException"

		''' <summary>
		''' Creates a context in which to continue a context operation.
		''' <p>
		''' In performing an operation on a name that spans multiple
		''' namespaces, a context from one naming system may need to pass
		''' the operation on to the next naming system.  The context
		''' implementation does this by first constructing a
		''' <code>CannotProceedException</code> containing information
		''' pinpointing how far it has proceeded.  It then obtains a
		''' continuation context from JNDI by calling
		''' <code>getContinuationContext</code>.  The context
		''' implementation should then resume the context operation by
		''' invoking the same operation on the continuation context, using
		''' the remainder of the name that has not yet been resolved.
		''' <p>
		''' Before making use of the <tt>cpe</tt> parameter, this method
		''' updates the environment associated with that object by setting
		''' the value of the property <a href="#CPE"><tt>CPE</tt></a>
		''' to <tt>cpe</tt>.  This property will be inherited by the
		''' continuation context, and may be used by that context's
		''' service provider to inspect the fields of this exception.
		''' </summary>
		''' <param name="cpe">
		'''          The non-null exception that triggered this continuation. </param>
		''' <returns> A non-null Context object for continuing the operation. </returns>
		''' <exception cref="NamingException"> If a naming exception occurred. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function getContinuationContext(ByVal cpe As CannotProceedException) As Context

			Dim env As Dictionary(Of Object, Object) = CType(cpe.environment, Dictionary(Of Object, Object))
			If env Is Nothing Then
				env = New Dictionary(Of )(7)
			Else
				' Make a (shallow) copy of the environment.
				env = CType(env.clone(), Dictionary(Of Object, Object))
			End If
			env(NamingManager.CPE) = cpe

			Dim cctx As New ContinuationContext(cpe, env)
			Return cctx.targetContext
		End Function

	' ------------ State Factory Stuff

		''' <summary>
		''' Retrieves the state of an object for binding.
		''' <p>
		''' Service providers that implement the <tt>DirContext</tt> interface
		''' should use <tt>DirectoryManager.getStateToBind()</tt>, not this method.
		''' Service providers that implement only the <tt>Context</tt> interface
		''' should use this method.
		''' <p>
		''' This method uses the specified state factories in
		''' the <tt>Context.STATE_FACTORIES</tt> property from the environment
		''' properties, and from the provider resource file associated with
		''' <tt>nameCtx</tt>, in that order.
		'''    The value of this property is a colon-separated list of factory
		'''    class names that are tried in order, and the first one that succeeds
		'''    in returning the object's state is the one used.
		''' If no object's state can be retrieved in this way, return the
		''' object itself.
		'''    If an exception is encountered while retrieving the state, the
		'''    exception is passed up to the caller.
		''' <p>
		''' Note that a state factory
		''' (an object that implements the StateFactory
		''' interface) must be public and must have a public constructor that
		''' accepts no arguments.
		''' <p>
		''' The <code>name</code> and <code>nameCtx</code> parameters may
		''' optionally be used to specify the name of the object being created.
		''' See the description of "Name and Context Parameters" in
		''' {@link ObjectFactory#getObjectInstance
		'''          ObjectFactory.getObjectInstance()}
		''' for details.
		''' <p>
		''' This method may return a <tt>Referenceable</tt> object.  The
		''' service provider obtaining this object may choose to store it
		''' directly, or to extract its reference (using
		''' <tt>Referenceable.getReference()</tt>) and store that instead.
		''' </summary>
		''' <param name="obj"> The non-null object for which to get state to bind. </param>
		''' <param name="name"> The name of this object relative to <code>nameCtx</code>,
		'''          or null if no name is specified. </param>
		''' <param name="nameCtx"> The context relative to which the <code>name</code>
		'''          parameter is specified, or null if <code>name</code> is
		'''          relative to the default initial context. </param>
		'''  <param name="environment"> The possibly null environment to
		'''          be used in the creation of the state factory and
		'''  the object's state. </param>
		''' <returns> The non-null object representing <tt>obj</tt>'s state for
		'''  binding.  It could be the object (<tt>obj</tt>) itself. </returns>
		''' <exception cref="NamingException"> If one of the factories accessed throws an
		'''          exception, or if an error was encountered while loading
		'''          and instantiating the factory and object classes.
		'''          A factory should only throw an exception if it does not want
		'''          other factories to be used in an attempt to create an object.
		'''  See <tt>StateFactory.getStateToBind()</tt>. </exception>
		''' <seealso cref= StateFactory </seealso>
		''' <seealso cref= StateFactory#getStateToBind </seealso>
		''' <seealso cref= DirectoryManager#getStateToBind
		''' @since 1.3 </seealso>
		Public Shared Function getStateToBind(Of T1)(ByVal obj As Object, ByVal name As Name, ByVal nameCtx As Context, ByVal environment As Dictionary(Of T1)) As Object

			Dim factories As com.sun.naming.internal.FactoryEnumeration = com.sun.naming.internal.ResourceManager.getFactories(Context.STATE_FACTORIES, environment, nameCtx)

			If factories Is Nothing Then Return obj

			' Try each factory until one succeeds
			Dim factory As StateFactory
			Dim answer As Object = Nothing
			Do While answer Is Nothing AndAlso factories.hasMore()
				factory = CType(factories.next(), StateFactory)
				answer = factory.getStateToBind(obj, name, nameCtx, environment)
			Loop

			Return If(answer IsNot Nothing, answer, obj)
		End Function
	End Class

End Namespace
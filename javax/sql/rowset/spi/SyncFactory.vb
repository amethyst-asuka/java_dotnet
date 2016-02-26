Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Threading
Imports javax.sql
Imports javax.naming

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.sql.rowset.spi




	''' <summary>
	''' The Service Provider Interface (SPI) mechanism that generates <code>SyncProvider</code>
	''' instances to be used by disconnected <code>RowSet</code> objects.
	''' The <code>SyncProvider</code> instances in turn provide the
	''' <code>javax.sql.RowSetReader</code> object the <code>RowSet</code> object
	''' needs to populate itself with data and the
	''' <code>javax.sql.RowSetWriter</code> object it needs to
	''' propagate changes to its
	''' data back to the underlying data source.
	''' <P>
	''' Because the methods in the <code>SyncFactory</code> class are all static,
	''' there is only one <code>SyncFactory</code> object
	''' per Java VM at any one time. This ensures that there is a single source from which a
	''' <code>RowSet</code> implementation can obtain its <code>SyncProvider</code>
	''' implementation.
	''' 
	''' <h3>1.0 Overview</h3>
	''' The <code>SyncFactory</code> class provides an internal registry of available
	''' synchronization provider implementations (<code>SyncProvider</code> objects).
	''' This registry may be queried to determine which
	''' synchronization providers are available.
	''' The following line of code gets an enumeration of the providers currently registered.
	''' <PRE>
	'''     java.util.Enumeration e = SyncFactory.getRegisteredProviders();
	''' </PRE>
	''' All standard <code>RowSet</code> implementations must provide at least two providers:
	''' <UL>
	'''  <LI>an optimistic provider for use with a <code>CachedRowSet</code> implementation
	'''     or an implementation derived from it
	'''  <LI>an XML provider, which is used for reading and writing XML, such as with
	'''       <code>WebRowSet</code> objects
	''' </UL>
	''' Note that the JDBC RowSet Implementations include the <code>SyncProvider</code>
	''' implementations <code>RIOptimisticProvider</code> and <code>RIXmlProvider</code>,
	''' which satisfy this requirement.
	''' <P>
	''' The <code>SyncFactory</code> class provides accessor methods to assist
	''' applications in determining which synchronization providers are currently
	''' registered with the <code>SyncFactory</code>.
	''' <p>
	''' Other methods let <code>RowSet</code> persistence providers be
	''' registered or de-registered with the factory mechanism. This
	''' allows additional synchronization provider implementations to be made
	''' available to <code>RowSet</code> objects at run time.
	''' <p>
	''' Applications can apply a degree of filtering to determine the level of
	''' synchronization that a <code>SyncProvider</code> implementation offers.
	''' The following criteria determine whether a provider is
	''' made available to a <code>RowSet</code> object:
	''' <ol>
	''' <li>If a particular provider is specified by a <code>RowSet</code> object, and
	''' the <code>SyncFactory</code> does not contain a reference to this provider,
	''' a <code>SyncFactoryException</code> is thrown stating that the synchronization
	''' provider could not be found.
	''' 
	''' <li>If a <code>RowSet</code> implementation is instantiated with a specified
	''' provider and the specified provider has been properly registered, the
	''' requested provider is supplied. Otherwise a <code>SyncFactoryException</code>
	''' is thrown.
	''' 
	''' <li>If a <code>RowSet</code> object does not specify a
	''' <code>SyncProvider</code> implementation and no additional
	''' <code>SyncProvider</code> implementations are available, the reference
	''' implementation providers are supplied.
	''' </ol>
	''' <h3>2.0 Registering <code>SyncProvider</code> Implementations</h3>
	''' <p>
	''' Both vendors and developers can register <code>SyncProvider</code>
	''' implementations using one of the following mechanisms.
	''' <ul>
	''' <LI><B>Using the command line</B><BR>
	''' The name of the provider is supplied on the command line, which will add
	''' the provider to the system properties.
	''' For example:
	''' <PRE>
	'''    -Drowset.provider.classname=com.fred.providers.HighAvailabilityProvider
	''' </PRE>
	''' <li><b>Using the Standard Properties File</b><BR>
	''' The reference implementation is targeted
	''' to ship with J2SE 1.5, which will include an additional resource file
	''' that may be edited by hand. Here is an example of the properties file
	''' included in the reference implementation:
	''' <PRE>
	'''   #Default JDBC RowSet sync providers listing
	'''   #
	''' 
	'''   # Optimistic synchronization provider
	'''   rowset.provider.classname.0=com.sun.rowset.providers.RIOptimisticProvider
	'''   rowset.provider.vendor.0=Oracle Corporation
	'''   rowset.provider.version.0=1.0
	''' 
	'''   # XML Provider using standard XML schema
	'''   rowset.provider.classname.1=com.sun.rowset.providers.RIXMLProvider
	'''   rowset.provider.vendor.1=Oracle Corporation
	'''   rowset.provider.version.1=1.0
	''' </PRE>
	''' The <code>SyncFactory</code> checks this file and registers the
	''' <code>SyncProvider</code> implementations that it contains. A
	''' developer or vendor can add other implementations to this file.
	''' For example, here is a possible addition:
	''' <PRE>
	'''     rowset.provider.classname.2=com.fred.providers.HighAvailabilityProvider
	'''     rowset.provider.vendor.2=Fred, Inc.
	'''     rowset.provider.version.2=1.0
	''' </PRE>
	''' 
	''' <li><b>Using a JNDI Context</b><BR>
	''' Available providers can be registered on a JNDI
	''' context, and the <code>SyncFactory</code> will attempt to load
	''' <code>SyncProvider</code> implementations from that JNDI context.
	''' For example, the following code fragment registers a provider implementation
	''' on a JNDI context.  This is something a deployer would normally do. In this
	''' example, <code>MyProvider</code> is being registered on a CosNaming
	''' namespace, which is the namespace used by J2EE resources.
	''' <PRE>
	'''    import javax.naming.*;
	''' 
	'''    Hashtable svrEnv = new  Hashtable();
	'''    srvEnv.put(Context.INITIAL_CONTEXT_FACTORY, "CosNaming");
	''' 
	'''    Context ctx = new InitialContext(svrEnv);
	'''    com.fred.providers.MyProvider = new MyProvider();
	'''    ctx.rebind("providers/MyProvider", syncProvider);
	''' </PRE>
	''' </ul>
	''' Next, an application will register the JNDI context with the
	''' <code>SyncFactory</code> instance.  This allows the <code>SyncFactory</code>
	''' to browse within the JNDI context looking for <code>SyncProvider</code>
	''' implementations.
	''' <PRE>
	'''    Hashtable appEnv = new Hashtable();
	'''    appEnv.put(Context.INITIAL_CONTEXT_FACTORY, "CosNaming");
	'''    appEnv.put(Context.PROVIDER_URL, "iiop://hostname/providers");
	'''    Context ctx = new InitialContext(appEnv);
	''' 
	'''    SyncFactory.registerJNDIContext(ctx);
	''' </PRE>
	''' If a <code>RowSet</code> object attempts to obtain a <code>MyProvider</code>
	''' object, the <code>SyncFactory</code> will try to locate it. First it searches
	''' for it in the system properties, then it looks in the resource files, and
	''' finally it checks the JNDI context that has been set. The <code>SyncFactory</code>
	''' instance verifies that the requested provider is a valid extension of the
	''' <code>SyncProvider</code> abstract class and then gives it to the
	''' <code>RowSet</code> object. In the following code fragment, a new
	''' <code>CachedRowSet</code> object is created and initialized with
	''' <i>env</i>, which contains the binding to <code>MyProvider</code>.
	''' <PRE>
	'''    Hashtable env = new Hashtable();
	'''    env.put(SyncFactory.ROWSET_SYNC_PROVIDER, "com.fred.providers.MyProvider");
	'''    CachedRowSet crs = new com.sun.rowset.CachedRowSetImpl(env);
	''' </PRE>
	''' Further details on these mechanisms are available in the
	''' <code>javax.sql.rowset.spi</code> package specification.
	''' 
	''' @author  Jonathan Bruce </summary>
	''' <seealso cref= javax.sql.rowset.spi.SyncProvider </seealso>
	''' <seealso cref= javax.sql.rowset.spi.SyncFactoryException </seealso>
	Public Class SyncFactory

		''' <summary>
		''' Creates a new <code>SyncFactory</code> object, which is the singleton
		''' instance.
		''' Having a private constructor guarantees that no more than
		''' one <code>SyncProvider</code> object can exist at a time.
		''' </summary>
		Private Sub New()
		End Sub

		''' <summary>
		''' The standard property-id for a synchronization provider implementation
		''' name.
		''' </summary>
		Public Const ROWSET_SYNC_PROVIDER As String = "rowset.provider.classname"
		''' <summary>
		''' The standard property-id for a synchronization provider implementation
		''' vendor name.
		''' </summary>
		Public Const ROWSET_SYNC_VENDOR As String = "rowset.provider.vendor"
		''' <summary>
		''' The standard property-id for a synchronization provider implementation
		''' version tag.
		''' </summary>
		Public Const ROWSET_SYNC_PROVIDER_VERSION As String = "rowset.provider.version"
		''' <summary>
		''' The standard resource file name.
		''' </summary>
		Private Shared ROWSET_PROPERTIES As String = "rowset.properties"

		''' <summary>
		'''  Permission required to invoke setJNDIContext and setLogger
		''' </summary>
		Private Shared ReadOnly SET_SYNCFACTORY_PERMISSION As New SQLPermission("setSyncFactory")
		''' <summary>
		''' The initial JNDI context where <code>SyncProvider</code> implementations can
		''' be stored and from which they can be invoked.
		''' </summary>
		Private Shared ic As Context
		''' <summary>
		''' The <code>Logger</code> object to be used by the <code>SyncFactory</code>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared rsLogger As Logger

		''' <summary>
		''' The registry of available <code>SyncProvider</code> implementations.
		''' See section 2.0 of the class comment for <code>SyncFactory</code> for an
		''' explanation of how a provider can be added to this registry.
		''' </summary>
		Private Shared implementations As Dictionary(Of String, SyncProvider)

		''' <summary>
		''' Adds the the given synchronization provider to the factory register. Guidelines
		''' are provided in the <code>SyncProvider</code> specification for the
		''' required naming conventions for <code>SyncProvider</code>
		''' implementations.
		''' <p>
		''' Synchronization providers bound to a JNDI context can be
		''' registered by binding a SyncProvider instance to a JNDI namespace.
		''' 
		''' <pre>
		''' {@code
		''' SyncProvider p = new MySyncProvider();
		''' InitialContext ic = new InitialContext();
		''' ic.bind ("jdbc/rowset/MySyncProvider", p);
		''' } </pre>
		''' 
		''' Furthermore, an initial JNDI context should be set with the
		''' <code>SyncFactory</code> using the <code>setJNDIContext</code> method.
		''' The <code>SyncFactory</code> leverages this context to search for
		''' available <code>SyncProvider</code> objects bound to the JNDI
		''' context and its child nodes.
		''' </summary>
		''' <param name="providerID"> A <code>String</code> object with the unique ID of the
		'''             synchronization provider being registered </param>
		''' <exception cref="SyncFactoryException"> if an attempt is made to supply an empty
		'''         or null provider name </exception>
		''' <seealso cref= #setJNDIContext </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Sub registerProvider(ByVal providerID As String)

			Dim impl As New ProviderImpl
			impl.classname = providerID
			initMapIfNecessary()
			implementations(providerID) = impl

		End Sub

		''' <summary>
		''' Returns the <code>SyncFactory</code> singleton.
		''' </summary>
		''' <returns> the <code>SyncFactory</code> instance </returns>
		Public Property Shared syncFactory As SyncFactory
			Get
		'        
		'         * Using Initialization on Demand Holder idiom as
		'         * Effective Java 2nd Edition,ITEM 71, indicates it is more performant
		'         * than the Double-Check Locking idiom.
		'         
				Return SyncFactoryHolder.factory
			End Get
		End Property

		''' <summary>
		''' Removes the designated currently registered synchronization provider from the
		''' Factory SPI register.
		''' </summary>
		''' <param name="providerID"> The unique-id of the synchronization provider </param>
		''' <exception cref="SyncFactoryException"> If an attempt is made to
		''' unregister a SyncProvider implementation that was not registered. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Sub unregisterProvider(ByVal providerID As String)
			initMapIfNecessary()
			If implementations.ContainsKey(providerID) Then implementations.Remove(providerID)
		End Sub
		Private Shared colon As String = ":"
		Private Shared strFileSep As String = "/"

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Sub initMapIfNecessary()

			' Local implementation class names and keys from Properties
			' file, translate names into Class objects using Class.forName
			' and store mappings
			Dim properties As New Properties

			If implementations Is Nothing Then
				implementations = New Dictionary(Of )

				Try

					' check if user is supplying his Synchronisation Provider
					' Implementation if not using Oracle's implementation.
					' properties.load(new FileInputStream(ROWSET_PROPERTIES));

					' The rowset.properties needs to be in jdk/jre/lib when
					' integrated with jdk.
					' else it should be picked from -D option from command line.

					' -Drowset.properties will add to standard properties. Similar
					' keys will over-write

	'                
	'                 * Dependent on application
	'                 
					Dim strRowsetProperties As String
					Try
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'						strRowsetProperties = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<String>()
	'					{
	'						public String run()
	'						{
	'							Return System.getProperty("rowset.properties");
	'						}
	'					}, Nothing, New PropertyPermission("rowset.properties", "read"));
					Catch ex As Exception
						Console.WriteLine("errorget rowset.properties: " & ex)
						strRowsetProperties = Nothing
					End Try

					If strRowsetProperties IsNot Nothing Then
						' Load user's implementation of SyncProvider
						' here. -Drowset.properties=/abc/def/pqr.txt
						ROWSET_PROPERTIES = strRowsetProperties
						Using fis As New java.io.FileInputStream(ROWSET_PROPERTIES)
							properties.load(fis)
						End Using
						parseProperties(properties)
					End If

	'                
	'                 * Always available
	'                 
					ROWSET_PROPERTIES = "javax" & strFileSep & "sql" & strFileSep & "rowset" & strFileSep & "rowset.properties"

					Dim cl As ClassLoader = Thread.CurrentThread.contextClassLoader

					Try
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						java.security.AccessController.doPrivileged(CType(, java.security.PrivilegedExceptionAction(Of Void)) -> { try(java.io.InputStream stream = If(cl Is Nothing, ClassLoader.getSystemResourceAsStream(ROWSET_PROPERTIES), cl.getResourceAsStream(ROWSET_PROPERTIES))) { if(stream Is Nothing) { throw New SyncFactoryException("Resource " & ROWSET_PROPERTIES & " not found"); } properties.load(stream); } Return Nothing; })
					Catch ex As java.security.PrivilegedActionException
						Dim e As Exception = ex.exception
						If TypeOf e Is SyncFactoryException Then
						  Throw CType(e, SyncFactoryException)
						Else
							Dim sfe As New SyncFactoryException
							sfe.initCause(ex.exception)
							Throw sfe
						End If
					End Try

					parseProperties(properties)

				' removed else, has properties should sum together

				Catch e As java.io.FileNotFoundException
					Throw New SyncFactoryException("Cannot locate properties file: " & e)
				Catch e As java.io.IOException
					Throw New SyncFactoryException("IOException: " & e)
				End Try

	'            
	'             * Now deal with -Drowset.provider.classname
	'             * load additional properties from -D command line
	'             
				properties.clear()
				Dim providerImpls As String
				Try
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'					providerImpls = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<String>()
	'				{
	'					public String run()
	'					{
	'						Return System.getProperty(ROWSET_SYNC_PROVIDER);
	'					}
	'				}, Nothing, New PropertyPermission(ROWSET_SYNC_PROVIDER, "read"));
				Catch ex As Exception
					providerImpls = Nothing
				End Try

				If providerImpls IsNot Nothing Then
					Dim i As Integer = 0
					If providerImpls.IndexOf(colon) > 0 Then
						Dim tokenizer As New StringTokenizer(providerImpls, colon)
						Do While tokenizer.hasMoreElements()
							properties.put(ROWSET_SYNC_PROVIDER & "." & i, tokenizer.nextToken())
							i += 1
						Loop
					Else
						properties.put(ROWSET_SYNC_PROVIDER, providerImpls)
					End If
					parseProperties(properties)
				End If
			End If
		End Sub

		''' <summary>
		''' The internal debug switch.
		''' </summary>
		Private Shared debug As Boolean = False
		''' <summary>
		''' Internal registry count for the number of providers contained in the
		''' registry.
		''' </summary>
		Private Shared providerImplIndex As Integer = 0

		''' <summary>
		''' Internal handler for all standard property parsing. Parses standard
		''' ROWSET properties and stores lazy references into the the internal registry.
		''' </summary>
		Private Shared Sub parseProperties(ByVal p As Properties)

			Dim impl As ProviderImpl = Nothing
			Dim key As String = Nothing
			Dim ___propertyNames As String() = Nothing

			Dim e As System.Collections.IEnumerator(Of ?) = p.propertyNames()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While e.hasMoreElements()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim str As String = CStr(e.nextElement())

				Dim w As Integer = str.Length

				If str.StartsWith(SyncFactory.ROWSET_SYNC_PROVIDER) Then

					impl = New ProviderImpl
					impl.index = providerImplIndex
					providerImplIndex += 1

					If w = (SyncFactory.ROWSET_SYNC_PROVIDER).length() Then
						' no property index has been set.
						___propertyNames = getPropertyNames(False)
					Else
						' property index has been set.
						___propertyNames = getPropertyNames(True, str.Substring(w - 1))
					End If

					key = p.getProperty(___propertyNames(0))
					impl.classname = key
					impl.vendor = p.getProperty(___propertyNames(1))
					impl.version = p.getProperty(___propertyNames(2))
					implementations(key) = impl
				End If
			Loop
		End Sub

		''' <summary>
		''' Used by the parseProperties methods to disassemble each property tuple.
		''' </summary>
		Private Shared Function getPropertyNames(ByVal append As Boolean) As String()
			Return getPropertyNames(append, Nothing)
		End Function

		''' <summary>
		''' Disassembles each property and its associated value. Also handles
		''' overloaded property names that contain indexes.
		''' </summary>
		Private Shared Function getPropertyNames(ByVal append As Boolean, ByVal propertyIndex As String) As String()
			Dim dot As String = "."
			Dim ___propertyNames As String() = {SyncFactory.ROWSET_SYNC_PROVIDER, SyncFactory.ROWSET_SYNC_VENDOR, SyncFactory.ROWSET_SYNC_PROVIDER_VERSION}
			If append Then
				For i As Integer = 0 To ___propertyNames.Length - 1
					___propertyNames(i) = ___propertyNames(i) + dot + propertyIndex
				Next i
				Return ___propertyNames
			Else
				Return ___propertyNames
			End If
		End Function

		''' <summary>
		''' Internal debug method that outputs the registry contents.
		''' </summary>
		Private Shared Sub showImpl(ByVal impl As ProviderImpl)
			Console.WriteLine("Provider implementation:")
			Console.WriteLine("Classname: " & impl.classname)
			Console.WriteLine("Vendor: " & impl.vendor)
			Console.WriteLine("Version: " & impl.version)
			Console.WriteLine("Impl index: " & impl.index)
		End Sub

		''' <summary>
		''' Returns the <code>SyncProvider</code> instance identified by <i>providerID</i>.
		''' </summary>
		''' <param name="providerID"> the unique identifier of the provider </param>
		''' <returns> a <code>SyncProvider</code> implementation </returns>
		''' <exception cref="SyncFactoryException"> If the SyncProvider cannot be found,
		''' the providerID is {@code null}, or
		''' some error was encountered when trying to invoke this provider. </exception>
		Public Shared Function getInstance(ByVal providerID As String) As SyncProvider

			If providerID Is Nothing Then Throw New SyncFactoryException("The providerID cannot be null")

			initMapIfNecessary() ' populate HashTable
			initJNDIContext() ' check JNDI context for any additional bindings

			Dim impl As ProviderImpl = CType(implementations(providerID), ProviderImpl)

			If impl Is Nothing Then Return New com.sun.rowset.providers.RIOptimisticProvider

			Try
				sun.reflect.misc.ReflectUtil.checkPackageAccess(providerID)
			Catch e As java.security.AccessControlException
				Dim sfe As New SyncFactoryException
				sfe.initCause(e)
				Throw sfe
			End Try

			' Attempt to invoke classname from registered SyncProvider list
			Dim c As Type = Nothing
			Try
				Dim cl As ClassLoader = Thread.CurrentThread.contextClassLoader

				''' <summary>
				''' The SyncProvider implementation of the user will be in
				''' the classpath. We need to find the ClassLoader which loads
				''' this SyncFactory and try to load the SyncProvider class from
				''' there.
				''' 
				''' </summary>
				c = Type.GetType(providerID, True, cl)

				If c IsNot Nothing Then
					Return CType(c.newInstance(), SyncProvider)
				Else
					Return New com.sun.rowset.providers.RIOptimisticProvider
				End If

			Catch e As IllegalAccessException
				Throw New SyncFactoryException("IllegalAccessException: " & e.Message)
			Catch e As InstantiationException
				Throw New SyncFactoryException("InstantiationException: " & e.Message)
			Catch e As ClassNotFoundException
				Throw New SyncFactoryException("ClassNotFoundException: " & e.Message)
			End Try
		End Function

		''' <summary>
		''' Returns an Enumeration of currently registered synchronization
		''' providers.  A <code>RowSet</code> implementation may use any provider in
		''' the enumeration as its <code>SyncProvider</code> object.
		''' <p>
		''' At a minimum, the reference synchronization provider allowing
		''' RowSet content data to be stored using a JDBC driver should be
		''' possible.
		''' </summary>
		''' <returns> Enumeration  A enumeration of available synchronization
		''' providers that are registered with this Factory </returns>
		''' <exception cref="SyncFactoryException"> If an error occurs obtaining the registered
		''' providers </exception>
		Public Property Shared registeredProviders As System.Collections.IEnumerator(Of SyncProvider)
			Get
				initMapIfNecessary()
				' return a collection of classnames
				' of type SyncProvider
				Return implementations.Values.GetEnumerator()
			End Get
		End Property

		''' <summary>
		''' Sets the logging object to be used by the <code>SyncProvider</code>
		''' implementation provided by the <code>SyncFactory</code>. All
		''' <code>SyncProvider</code> implementations can log their events to
		''' this object and the application can retrieve a handle to this
		''' object using the <code>getLogger</code> method.
		''' <p>
		''' This method checks to see that there is an {@code SQLPermission}
		''' object  which grants the permission {@code setSyncFactory}
		''' before allowing the method to succeed.  If a
		''' {@code SecurityManager} exists and its
		''' {@code checkPermission} method denies calling {@code setLogger},
		''' this method throws a
		''' {@code java.lang.SecurityException}.
		''' </summary>
		''' <param name="logger"> A Logger object instance </param>
		''' <exception cref="java.lang.SecurityException"> if a security manager exists and its
		'''   {@code checkPermission} method denies calling {@code setLogger} </exception>
		''' <exception cref="NullPointerException"> if the logger is null </exception>
		''' <seealso cref= SecurityManager#checkPermission </seealso>
		Public Shared Property logger As Logger
			Set(ByVal logger As Logger)
    
				Dim sec As SecurityManager = System.securityManager
				If sec IsNot Nothing Then sec.checkPermission(SET_SYNCFACTORY_PERMISSION)
    
				If logger Is Nothing Then Throw New NullPointerException("You must provide a Logger")
				rsLogger = logger
			End Set
			Get
    
				Dim result As Logger = rsLogger
				' only one logger per session
				If result Is Nothing Then Throw New SyncFactoryException("(SyncFactory) : No logger has been set")
    
				Return result
			End Get
		End Property

		''' <summary>
		''' Sets the logging object that is used by <code>SyncProvider</code>
		''' implementations provided by the <code>SyncFactory</code> SPI. All
		''' <code>SyncProvider</code> implementations can log their events
		''' to this object and the application can retrieve a handle to this
		''' object using the <code>getLogger</code> method.
		''' <p>
		''' This method checks to see that there is an {@code SQLPermission}
		''' object  which grants the permission {@code setSyncFactory}
		''' before allowing the method to succeed.  If a
		''' {@code SecurityManager} exists and its
		''' {@code checkPermission} method denies calling {@code setLogger},
		''' this method throws a
		''' {@code java.lang.SecurityException}.
		''' </summary>
		''' <param name="logger"> a Logger object instance </param>
		''' <param name="level"> a Level object instance indicating the degree of logging
		''' required </param>
		''' <exception cref="java.lang.SecurityException"> if a security manager exists and its
		'''   {@code checkPermission} method denies calling {@code setLogger} </exception>
		''' <exception cref="NullPointerException"> if the logger is null </exception>
		''' <seealso cref= SecurityManager#checkPermission </seealso>
		''' <seealso cref= LoggingPermission </seealso>
		Public Shared Sub setLogger(ByVal logger As Logger, ByVal level As Level)
			' singleton
			Dim sec As SecurityManager = System.securityManager
			If sec IsNot Nothing Then sec.checkPermission(SET_SYNCFACTORY_PERMISSION)

			If logger Is Nothing Then Throw New NullPointerException("You must provide a Logger")
			logger.level = level
			rsLogger = logger
		End Sub


		''' <summary>
		''' Sets the initial JNDI context from which SyncProvider implementations
		''' can be retrieved from a JNDI namespace
		''' <p>
		'''  This method checks to see that there is an {@code SQLPermission}
		''' object  which grants the permission {@code setSyncFactory}
		''' before allowing the method to succeed.  If a
		''' {@code SecurityManager} exists and its
		''' {@code checkPermission} method denies calling {@code setJNDIContext},
		''' this method throws a
		''' {@code java.lang.SecurityException}.
		''' </summary>
		''' <param name="ctx"> a valid JNDI context </param>
		''' <exception cref="SyncFactoryException"> if the supplied JNDI context is null </exception>
		''' <exception cref="java.lang.SecurityException"> if a security manager exists and its
		'''  {@code checkPermission} method denies calling {@code setJNDIContext} </exception>
		''' <seealso cref= SecurityManager#checkPermission </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Property jNDIContext As javax.naming.Context
			Set(ByVal ctx As javax.naming.Context)
				Dim sec As SecurityManager = System.securityManager
				If sec IsNot Nothing Then sec.checkPermission(SET_SYNCFACTORY_PERMISSION)
				If ctx Is Nothing Then Throw New SyncFactoryException("Invalid JNDI context supplied")
				ic = ctx
			End Set
		End Property

		''' <summary>
		''' Controls JNDI context initialization.
		''' </summary>
		''' <exception cref="SyncFactoryException"> if an error occurs parsing the JNDI context </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Sub initJNDIContext()

			If (ic IsNot Nothing) AndAlso (lazyJNDICtxRefresh = False) Then
				Try
					parseProperties(parseJNDIContext())
					lazyJNDICtxRefresh = True ' touch JNDI namespace once.
				Catch e As NamingException
					Console.WriteLine(e.ToString())
					Console.Write(e.StackTrace)
					Throw New SyncFactoryException("SPI: NamingException: " & e.explanation)
				Catch e As Exception
					Console.WriteLine(e.ToString())
					Console.Write(e.StackTrace)
					Throw New SyncFactoryException("SPI: Exception: " & e.Message)
				End Try
			End If
		End Sub
		''' <summary>
		''' Internal switch indicating whether the JNDI namespace should be re-read.
		''' </summary>
		Private Shared lazyJNDICtxRefresh As Boolean = False

		''' <summary>
		''' Parses the set JNDI Context and passes bindings to the enumerateBindings
		''' method when complete.
		''' </summary>
		Private Shared Function parseJNDIContext() As Properties

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim bindings As NamingEnumeration(Of ?) = ic.listBindings("")
			Dim properties As New Properties

			' Hunt one level below context for available SyncProvider objects
			enumerateBindings(bindings, properties)

			Return properties
		End Function

		''' <summary>
		''' Scans each binding on JNDI context and determines if any binding is an
		''' instance of SyncProvider, if so, add this to the registry and continue to
		''' scan the current context using a re-entrant call to this method until all
		''' bindings have been enumerated.
		''' </summary>
		Private Shared Sub enumerateBindings(Of T1)(ByVal bindings As NamingEnumeration(Of T1), ByVal properties As Properties)

			Dim syncProviderObj As Boolean = False ' move to parameters ?

			Try
				Dim bd As Binding = Nothing
				Dim elementObj As Object = Nothing
				Dim element As String = Nothing
				Do While bindings.hasMore()
					bd = CType(bindings.next(), Binding)
					element = bd.name
					elementObj = bd.object

					If Not(TypeOf ic.lookup(element) Is Context) Then
						' skip directories/sub-contexts
						If TypeOf ic.lookup(element) Is SyncProvider Then syncProviderObj = True
					End If

					If syncProviderObj Then
						Dim sync As SyncProvider = CType(elementObj, SyncProvider)
						properties.put(SyncFactory.ROWSET_SYNC_PROVIDER, sync.providerID)
						syncProviderObj = False ' reset
					End If

				Loop
			Catch e As javax.naming.NotContextException
				bindings.next()
				' Re-entrant call into method
				enumerateBindings(bindings, properties)
			End Try
		End Sub

		''' <summary>
		''' Lazy initialization Holder class used by {@code getSyncFactory}
		''' </summary>
		Private Class SyncFactoryHolder
			Friend Shared ReadOnly factory As New SyncFactory
		End Class
	End Class

	''' <summary>
	''' Internal class that defines the lazy reference construct for each registered
	''' SyncProvider implementation.
	''' </summary>
	Friend Class ProviderImpl
		Inherits SyncProvider

		Private className As String = Nothing
		Private vendorName As String = Nothing
		Private ver As String = Nothing
		Private index As Integer

		Public Overridable Property classname As String
			Set(ByVal classname As String)
				Me.className = classname
			End Set
			Get
				Return className
			End Get
		End Property


		Public Overridable Property vendor As String
			Set(ByVal vendor As String)
				vendorName = vendor
			End Set
			Get
				Return vendorName
			End Get
		End Property


		Public Overridable Property version As String
			Set(ByVal providerVer As String)
				ver = providerVer
			End Set
			Get
				Return ver
			End Get
		End Property


		Public Overridable Property index As Integer
			Set(ByVal i As Integer)
				index = i
			End Set
			Get
				Return index
			End Get
		End Property


		Public Property Overrides dataSourceLock As Integer
			Get
    
				Dim dsLock As Integer = 0
				Try
					dsLock = SyncFactory.getInstance(className).dataSourceLock
				Catch sfEx As SyncFactoryException
    
					Throw New SyncProviderException(sfEx.Message)
				End Try
    
				Return dsLock
			End Get
			Set(ByVal param As Integer)
    
				Try
					SyncFactory.getInstance(className).dataSourceLock = param
				Catch sfEx As SyncFactoryException
    
					Throw New SyncProviderException(sfEx.Message)
				End Try
			End Set
		End Property

		Public Property Overrides providerGrade As Integer
			Get
    
				Dim grade As Integer = 0
    
				Try
					grade = SyncFactory.getInstance(className).providerGrade
				Catch sfEx As SyncFactoryException
					'
				End Try
    
				Return grade
			End Get
		End Property

		Public Property Overrides providerID As String
			Get
				Return className
			End Get
		End Property

	'    
	'    public javax.sql.RowSetInternal getRowSetInternal() {
	'    try
	'    {
	'    return SyncFactory.getInstance(className).getRowSetInternal();
	'    } catch(SyncFactoryException sfEx) {
	'    //
	'    }
	'    }
	'     
		Public Property Overrides rowSetReader As javax.sql.RowSetReader
			Get
    
				Dim rsReader As RowSetReader = Nothing
    
				Try
					rsReader = SyncFactory.getInstance(className).rowSetReader
				Catch sfEx As SyncFactoryException
					'
				End Try
    
				Return rsReader
    
			End Get
		End Property

		Public Property Overrides rowSetWriter As javax.sql.RowSetWriter
			Get
    
				Dim rsWriter As RowSetWriter = Nothing
				Try
					rsWriter = SyncFactory.getInstance(className).rowSetWriter
				Catch sfEx As SyncFactoryException
					'
				End Try
    
				Return rsWriter
			End Get
		End Property


		Public Overrides Function supportsUpdatableView() As Integer

			Dim view As Integer = 0

			Try
				view = SyncFactory.getInstance(className).supportsUpdatableView()
			Catch sfEx As SyncFactoryException
				'
			End Try

			Return view
		End Function
	End Class

End Namespace
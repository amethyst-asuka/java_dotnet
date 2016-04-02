Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Threading

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

Namespace java.sql



	''' <summary>
	''' <P>The basic service for managing a set of JDBC drivers.<br>
	''' <B>NOTE:</B> The <seealso cref="javax.sql.DataSource"/> interface, new in the
	''' JDBC 2.0 API, provides another way to connect to a data source.
	''' The use of a <code>DataSource</code> object is the preferred means of
	''' connecting to a data source.
	''' 
	''' <P>As part of its initialization, the <code>DriverManager</code> class will
	''' attempt to load the driver classes referenced in the "jdbc.drivers"
	''' system property. This allows a user to customize the JDBC Drivers
	''' used by their applications. For example in your
	''' ~/.hotjava/properties file you might specify:
	''' <pre>
	''' <CODE>jdbc.drivers=foo.bah.Driver:wombat.sql.Driver:bad.taste.ourDriver</CODE>
	''' </pre>
	''' <P> The <code>DriverManager</code> methods <code>getConnection</code> and
	''' <code>getDrivers</code> have been enhanced to support the Java Standard Edition
	''' <a href="../../../technotes/guides/jar/jar.html#Service%20Provider">Service Provider</a> mechanism. JDBC 4.0 Drivers must
	''' include the file <code>META-INF/services/java.sql.Driver</code>. This file contains the name of the JDBC drivers
	''' implementation of <code>java.sql.Driver</code>.  For example, to load the <code>my.sql.Driver</code> [Class],
	''' the <code>META-INF/services/java.sql.Driver</code> file would contain the entry:
	''' <pre>
	''' <code>my.sql.Driver</code>
	''' </pre>
	''' 
	''' <P>Applications no longer need to explicitly load JDBC drivers using <code>Class.forName()</code>. Existing programs
	''' which currently load JDBC drivers using <code>Class.forName()</code> will continue to work without
	''' modification.
	''' 
	''' <P>When the method <code>getConnection</code> is called,
	''' the <code>DriverManager</code> will attempt to
	''' locate a suitable driver from amongst those loaded at
	''' initialization and those loaded explicitly using the same classloader
	''' as the current applet or application.
	''' 
	''' <P>
	''' Starting with the Java 2 SDK, Standard Edition, version 1.3, a
	''' logging stream can be set only if the proper
	''' permission has been granted.  Normally this will be done with
	''' the tool PolicyTool, which can be used to grant <code>permission
	''' java.sql.SQLPermission "setLog"</code>. </summary>
	''' <seealso cref= Driver </seealso>
	''' <seealso cref= Connection </seealso>
	Public Class DriverManager


		' List of registered JDBC drivers
		Private Shared ReadOnly registeredDrivers As New java.util.concurrent.CopyOnWriteArrayList(Of DriverInfo)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared loginTimeout As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared logWriter As java.io.PrintWriter = Nothing
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared logStream As java.io.PrintStream = Nothing
		' Used in println() to synchronize logWriter
		Private Shared ReadOnly logSync As New Object

		' Prevent the DriverManager class from being instantiated. 
		Private Sub New()
		End Sub


		''' <summary>
		''' Load the initial JDBC drivers by checking the System property
		''' jdbc.properties and then use the {@code ServiceLoader} mechanism
		''' </summary>
		Shared Sub New()
			loadInitialDrivers()
			println("JDBC DriverManager initialized")
		End Sub

		''' <summary>
		''' The <code>SQLPermission</code> constant that allows the
		''' setting of the logging stream.
		''' @since 1.3
		''' </summary>
		Friend Shared ReadOnly SET_LOG_PERMISSION As New SQLPermission("setLog")

		''' <summary>
		''' The {@code SQLPermission} constant that allows the
		''' un-register a registered JDBC driver.
		''' @since 1.8
		''' </summary>
		Friend Shared ReadOnly DEREGISTER_DRIVER_PERMISSION As New SQLPermission("deregisterDriver")

		'--------------------------JDBC 2.0-----------------------------

		''' <summary>
		''' Retrieves the log writer.
		''' 
		''' The <code>getLogWriter</code> and <code>setLogWriter</code>
		''' methods should be used instead
		''' of the <code>get/setlogStream</code> methods, which are deprecated. </summary>
		''' <returns> a <code>java.io.PrintWriter</code> object </returns>
		''' <seealso cref= #setLogWriter
		''' @since 1.2 </seealso>
		PublicShared ReadOnly PropertylogWriter As java.io.PrintWriter
			Get
					Return logWriter
			End Get
			Set(ByVal out As java.io.PrintWriter)
    
				Dim sec As SecurityManager = System.securityManager
				If sec IsNot Nothing Then sec.checkPermission(SET_LOG_PERMISSION)
					logStream = Nothing
					logWriter = out
			End Set
		End Property



		'---------------------------------------------------------------

		''' <summary>
		''' Attempts to establish a connection to the given database URL.
		''' The <code>DriverManager</code> attempts to select an appropriate driver from
		''' the set of registered JDBC drivers.
		''' <p>
		''' <B>Note:</B> If a property is specified as part of the {@code url} and
		''' is also specified in the {@code Properties} object, it is
		''' implementation-defined as to which value will take precedence.
		''' For maximum portability, an application should only specify a
		''' property once.
		''' </summary>
		''' <param name="url"> a database url of the form
		''' <code> jdbc:<em>subprotocol</em>:<em>subname</em></code> </param>
		''' <param name="info"> a list of arbitrary string tag/value pairs as
		''' connection arguments; normally at least a "user" and
		''' "password" property should be included </param>
		''' <returns> a Connection to the URL </returns>
		''' <exception cref="SQLException"> if a database access error occurs or the url is
		''' {@code null} </exception>
		''' <exception cref="SQLTimeoutException">  when the driver has determined that the
		''' timeout value specified by the {@code setLoginTimeout} method
		''' has been exceeded and has at least tried to cancel the
		''' current database connection attempt </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function getConnection(ByVal url As String, ByVal info As java.util.Properties) As Connection

			Return (getConnection(url, info, sun.reflect.Reflection.callerClass))
		End Function

		''' <summary>
		''' Attempts to establish a connection to the given database URL.
		''' The <code>DriverManager</code> attempts to select an appropriate driver from
		''' the set of registered JDBC drivers.
		''' <p>
		''' <B>Note:</B> If the {@code user} or {@code password} property are
		''' also specified as part of the {@code url}, it is
		''' implementation-defined as to which value will take precedence.
		''' For maximum portability, an application should only specify a
		''' property once.
		''' </summary>
		''' <param name="url"> a database url of the form
		''' <code>jdbc:<em>subprotocol</em>:<em>subname</em></code> </param>
		''' <param name="user"> the database user on whose behalf the connection is being
		'''   made </param>
		''' <param name="password"> the user's password </param>
		''' <returns> a connection to the URL </returns>
		''' <exception cref="SQLException"> if a database access error occurs or the url is
		''' {@code null} </exception>
		''' <exception cref="SQLTimeoutException">  when the driver has determined that the
		''' timeout value specified by the {@code setLoginTimeout} method
		''' has been exceeded and has at least tried to cancel the
		''' current database connection attempt </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function getConnection(ByVal url As String, ByVal user As String, ByVal password As String) As Connection
			Dim info As New java.util.Properties

			If user IsNot Nothing Then info("user") = user
			If password IsNot Nothing Then info("password") = password

			Return (getConnection(url, info, sun.reflect.Reflection.callerClass))
		End Function

		''' <summary>
		''' Attempts to establish a connection to the given database URL.
		''' The <code>DriverManager</code> attempts to select an appropriate driver from
		''' the set of registered JDBC drivers.
		''' </summary>
		''' <param name="url"> a database url of the form
		'''  <code> jdbc:<em>subprotocol</em>:<em>subname</em></code> </param>
		''' <returns> a connection to the URL </returns>
		''' <exception cref="SQLException"> if a database access error occurs or the url is
		''' {@code null} </exception>
		''' <exception cref="SQLTimeoutException">  when the driver has determined that the
		''' timeout value specified by the {@code setLoginTimeout} method
		''' has been exceeded and has at least tried to cancel the
		''' current database connection attempt </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function getConnection(ByVal url As String) As Connection

			Dim info As New java.util.Properties
			Return (getConnection(url, info, sun.reflect.Reflection.callerClass))
		End Function

		''' <summary>
		''' Attempts to locate a driver that understands the given URL.
		''' The <code>DriverManager</code> attempts to select an appropriate driver from
		''' the set of registered JDBC drivers.
		''' </summary>
		''' <param name="url"> a database URL of the form
		'''     <code>jdbc:<em>subprotocol</em>:<em>subname</em></code> </param>
		''' <returns> a <code>Driver</code> object representing a driver
		''' that can connect to the given URL </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function getDriver(ByVal url As String) As Driver

			println("DriverManager.getDriver(""" & url & """)")

			Dim callerClass As  [Class] = sun.reflect.Reflection.callerClass

			' Walk through the loaded registeredDrivers attempting to locate someone
			' who understands the given URL.
			For Each aDriver As DriverInfo In registeredDrivers
				' If the caller does not have permission to load the driver then
				' skip it.
				If isDriverAllowed(aDriver.driver, callerClass) Then
					Try
						If aDriver.driver.acceptsURL(url) Then
							' Success!
							println("getDriver returning " & aDriver.driver.GetType().name)
						Return (aDriver.driver)
						End If

					Catch sqe As SQLException
						' Drop through and try the next driver.
					End Try
				Else
					println("    skipping: " & aDriver.driver.GetType().name)
				End If

			Next aDriver

			println("getDriver: no suitable driver")
			Throw New SQLException("No suitable driver", "08001")
		End Function


		''' <summary>
		''' Registers the given driver with the {@code DriverManager}.
		''' A newly-loaded driver class should call
		''' the method {@code registerDriver} to make itself
		''' known to the {@code DriverManager}. If the driver is currently
		''' registered, no action is taken.
		''' </summary>
		''' <param name="driver"> the new JDBC Driver that is to be registered with the
		'''               {@code DriverManager} </param>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="NullPointerException"> if {@code driver} is null </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Sub registerDriver(ByVal driver As java.sql.Driver)

			registerDriver(driver, Nothing)
		End Sub

		''' <summary>
		''' Registers the given driver with the {@code DriverManager}.
		''' A newly-loaded driver class should call
		''' the method {@code registerDriver} to make itself
		''' known to the {@code DriverManager}. If the driver is currently
		''' registered, no action is taken.
		''' </summary>
		''' <param name="driver"> the new JDBC Driver that is to be registered with the
		'''               {@code DriverManager} </param>
		''' <param name="da">     the {@code DriverAction} implementation to be used when
		'''               {@code DriverManager#deregisterDriver} is called </param>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="NullPointerException"> if {@code driver} is null
		''' @since 1.8 </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Sub registerDriver(ByVal driver As java.sql.Driver, ByVal da As DriverAction)

			' Register the driver if it has not already been added to our list 
			If driver IsNot Nothing Then
				registeredDrivers.addIfAbsent(New DriverInfo(driver, da))
			Else
				' This is for compatibility with the original DriverManager
				Throw New NullPointerException
			End If

			println("registerDriver: " & driver)

		End Sub

		''' <summary>
		''' Removes the specified driver from the {@code DriverManager}'s list of
		''' registered drivers.
		''' <p>
		''' If a {@code null} value is specified for the driver to be removed, then no
		''' action is taken.
		''' <p>
		''' If a security manager exists and its {@code checkPermission} denies
		''' permission, then a {@code SecurityException} will be thrown.
		''' <p>
		''' If the specified driver is not found in the list of registered drivers,
		''' then no action is taken.  If the driver was found, it will be removed
		''' from the list of registered drivers.
		''' <p>
		''' If a {@code DriverAction} instance was specified when the JDBC driver was
		''' registered, its deregister method will be called
		''' prior to the driver being removed from the list of registered drivers.
		''' </summary>
		''' <param name="driver"> the JDBC Driver to remove </param>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SecurityException"> if a security manager exists and its
		''' {@code checkPermission} method denies permission to deregister a driver.
		''' </exception>
		''' <seealso cref= SecurityManager#checkPermission </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Sub deregisterDriver(ByVal driver As Driver)
			If driver Is Nothing Then Return

			Dim sec As SecurityManager = System.securityManager
			If sec IsNot Nothing Then sec.checkPermission(DEREGISTER_DRIVER_PERMISSION)

			println("DriverManager.deregisterDriver: " & driver)

			Dim aDriver As New DriverInfo(driver, Nothing)
			If registeredDrivers.contains(aDriver) Then
				If isDriverAllowed(driver, sun.reflect.Reflection.callerClass) Then
					Dim di As DriverInfo = registeredDrivers.get(registeredDrivers.IndexOf(aDriver))
					 ' If a DriverAction was specified, Call it to notify the
					 ' driver that it has been deregistered
					 If di.action() IsNot Nothing Then di.action().deregister()
					 registeredDrivers.remove(aDriver)
				Else
					' If the caller does not have permission to load the driver then
					' throw a SecurityException.
					Throw New SecurityException
				End If
			Else
				println("    couldn't find driver to unload")
			End If
		End Sub

		''' <summary>
		''' Retrieves an Enumeration with all of the currently loaded JDBC drivers
		''' to which the current caller has access.
		''' 
		''' <P><B>Note:</B> The classname of a driver can be found using
		''' <CODE>d.getClass().getName()</CODE>
		''' </summary>
		''' <returns> the list of JDBC Drivers loaded by the caller's class loader </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		PublicShared ReadOnly Propertydrivers As System.Collections.IEnumerator(Of Driver)
			Get
				Dim result As New List(Of Driver)
    
				Dim callerClass As  [Class] = sun.reflect.Reflection.callerClass
    
				' Walk through the loaded registeredDrivers.
				For Each aDriver As DriverInfo In registeredDrivers
					' If the caller does not have permission to load the driver then
					' skip it.
					If isDriverAllowed(aDriver.driver, callerClass) Then
						result.Add(aDriver.driver)
					Else
						println("    skipping: " & aDriver.GetType().name)
					End If
				Next aDriver
				Return (result.elements())
			End Get
		End Property


		''' <summary>
		''' Sets the maximum time in seconds that a driver will wait
		''' while attempting to connect to a database once the driver has
		''' been identified.
		''' </summary>
		''' <param name="seconds"> the login time limit in seconds; zero means there is no limit </param>
		''' <seealso cref= #getLoginTimeout </seealso>
		Public Shared Property loginTimeout As Integer
			Set(ByVal seconds As Integer)
				loginTimeout = seconds
			End Set
			Get
				Return (loginTimeout)
			End Get
		End Property


		''' <summary>
		''' Sets the logging/tracing PrintStream that is used
		''' by the <code>DriverManager</code>
		''' and all drivers.
		''' <P>
		''' In the Java 2 SDK, Standard Edition, version 1.3 release, this method checks
		''' to see that there is an <code>SQLPermission</code> object before setting
		''' the logging stream.  If a <code>SecurityManager</code> exists and its
		''' <code>checkPermission</code> method denies setting the log writer, this
		''' method throws a <code>java.lang.SecurityException</code>.
		''' </summary>
		''' <param name="out"> the new logging/tracing PrintStream; to disable, set to <code>null</code> </param>
		''' @deprecated Use {@code setLogWriter} 
		''' <exception cref="SecurityException"> if a security manager exists and its
		'''    <code>checkPermission</code> method denies setting the log stream
		''' </exception>
		''' <seealso cref= SecurityManager#checkPermission </seealso>
		''' <seealso cref= #getLogStream </seealso>
		<Obsolete("Use {@code setLogWriter}")> _
		Public Shared Property logStream As java.io.PrintStream
			Set(ByVal out As java.io.PrintStream)
    
				Dim sec As SecurityManager = System.securityManager
				If sec IsNot Nothing Then sec.checkPermission(SET_LOG_PERMISSION)
    
				logStream = out
				If out IsNot Nothing Then
					logWriter = New java.io.PrintWriter(out)
				Else
					logWriter = Nothing
				End If
			End Set
			Get
				Return logStream
			End Get
		End Property


		''' <summary>
		''' Prints a message to the current JDBC log stream.
		''' </summary>
		''' <param name="message"> a log or tracing message </param>
		Public Shared Sub println(ByVal message As String)
			SyncLock logSync
				If logWriter IsNot Nothing Then
					logWriter.println(message)

					' automatic flushing is never enabled, so we must do it ourselves
					logWriter.flush()
				End If
			End SyncLock
		End Sub

		'------------------------------------------------------------------------

		' Indicates whether the class object that would be created if the code calling
		' DriverManager is accessible.
		Private Shared Function isDriverAllowed(ByVal driver As Driver, ByVal caller As [Class]) As Boolean
			Dim callerCL As  ClassLoader = If(caller IsNot Nothing, caller.classLoader, Nothing)
			Return isDriverAllowed(driver, callerCL)
		End Function

		Private Shared Function isDriverAllowed(ByVal driver As Driver, ByVal classLoader_Renamed As  ClassLoader) As Boolean
			Dim result As Boolean = False
			If driver IsNot Nothing Then
				Dim aClass As  [Class] = Nothing
				Try
					aClass = Type.GetType(driver.GetType().name, True, classLoader_Renamed)
				Catch ex As Exception
					result = False
				End Try

				 result = If(aClass Is driver.GetType(), True, False)
			End If

			Return result
		End Function

		Private Shared Sub loadInitialDrivers()
			Dim drivers_Renamed As String
			Try
				drivers_Renamed = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
			Catch ex As Exception
				drivers_Renamed = Nothing
			End Try
			' If the driver is packaged as a Service Provider, load it.
			' Get all the drivers through the classloader
			' exposed as a java.sql.Driver.class service.
			' ServiceLoader.load() replaces the sun.misc.Providers()

			java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper2(Of T)

			println("DriverManager.initialize: jdbc.drivers = " & drivers_Renamed)

			If drivers_Renamed Is Nothing OrElse drivers_Renamed.Equals("") Then Return
			Dim driversList As String() = drivers_Renamed.Split(":")
			println("number of Drivers:" & driversList.Length)
			For Each aDriver As String In driversList
				Try
					println("DriverManager.Initialize: loading " & aDriver)
					Type.GetType(aDriver, True, ClassLoader.systemClassLoader)
				Catch ex As Exception
					println("DriverManager.Initialize: load failed: " & ex)
				End Try
			Next aDriver
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As String
				Return System.getProperty("jdbc.drivers")
			End Function
		End Class

		Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Void

				Dim loadedDrivers As java.util.ServiceLoader(Of Driver) = java.util.ServiceLoader.load(GetType(Driver))
				Dim driversIterator As IEnumerator(Of Driver) = loadedDrivers.GetEnumerator()

		'                 Load these drivers, so that they can be instantiated.
		'                 * It may be the case that the driver class may not be there
		'                 * i.e. there may be a packaged driver with the service class
		'                 * as implementation of java.sql.Driver but the actual class
		'                 * may be missing. In that case a java.util.ServiceConfigurationError
		'                 * will be thrown at runtime by the VM trying to locate
		'                 * and load the service.
		'                 *
		'                 * Adding a try catch block to catch those runtime errors
		'                 * if driver not available in classpath but it's
		'                 * packaged as service and that service is there in classpath.
		'                 
				Try
					Do While driversIterator.MoveNext()
						driversIterator.Current
					Loop
				Catch t As Throwable
				' Do nothing
				End Try
				Return Nothing
			End Function
		End Class


		'  Worker method called by the public getConnection() methods.
		Private Shared Function getConnection(ByVal url As String, ByVal info As java.util.Properties, ByVal caller As [Class]) As Connection
	'        
	'         * When callerCl is null, we should check the application's
	'         * (which is invoking this class indirectly)
	'         * classloader, so that the JDBC driver class outside rt.jar
	'         * can be loaded from here.
	'         
			Dim callerCL As  ClassLoader = If(caller IsNot Nothing, caller.classLoader, Nothing)
			SyncLock GetType(DriverManager)
				' synchronize loading of the correct classloader.
				If callerCL Is Nothing Then callerCL = Thread.CurrentThread.contextClassLoader
			End SyncLock

			If url Is Nothing Then Throw New SQLException("The url cannot be null", "08001")

			println("DriverManager.getConnection(""" & url & """)")

			' Walk through the loaded registeredDrivers attempting to make a connection.
			' Remember the first exception that gets raised so we can reraise it.
			Dim reason As SQLException = Nothing

			For Each aDriver As DriverInfo In registeredDrivers
				' If the caller does not have permission to load the driver then
				' skip it.
				If isDriverAllowed(aDriver.driver, callerCL) Then
					Try
						println("    trying " & aDriver.driver.GetType().name)
						Dim con As Connection = aDriver.driver.connect(url, info)
						If con IsNot Nothing Then
							' Success!
							println("getConnection returning " & aDriver.driver.GetType().name)
							Return (con)
						End If
					Catch ex As SQLException
						If reason Is Nothing Then reason = ex
					End Try

				Else
					println("    skipping: " & aDriver.GetType().name)
				End If

			Next aDriver

			' if we got here nobody could connect.
			If reason IsNot Nothing Then
				println("getConnection failed: " & reason)
				Throw reason
			End If

			println("getConnection: no suitable driver found for " & url)
			Throw New SQLException("No suitable driver found for " & url, "08001")
		End Function


	End Class

	'
	' * Wrapper class for registered Drivers in order to not expose Driver.equals()
	' * to avoid the capture of the Driver it being compared to as it might not
	' * normally have access.
	' 
	Friend Class DriverInfo

		Friend ReadOnly driver As Driver
		Friend da As DriverAction
		Friend Sub New(ByVal driver As Driver, ByVal action As DriverAction)
			Me.driver = driver
			da = action
		End Sub

		Public Overrides Function Equals(ByVal other As Object) As Boolean
			Return (TypeOf other Is DriverInfo) AndAlso Me.driver Is CType(other, DriverInfo).driver
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return driver.GetHashCode()
		End Function

		Public Overrides Function ToString() As String
			Return ("driver[className=" & driver & "]")
		End Function

		Friend Overridable Function action() As DriverAction
			Return da
		End Function
	End Class

End Namespace
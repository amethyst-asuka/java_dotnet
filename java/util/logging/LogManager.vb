Imports System
Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Collections.Generic
Imports System.Threading

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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


Namespace java.util.logging


	''' <summary>
	''' There is a single global LogManager object that is used to
	''' maintain a set of shared state about Loggers and log services.
	''' <p>
	''' This LogManager object:
	''' <ul>
	''' <li> Manages a hierarchical namespace of Logger objects.  All
	'''      named Loggers are stored in this namespace.
	''' <li> Manages a set of logging control properties.  These are
	'''      simple key-value pairs that can be used by Handlers and
	'''      other logging objects to configure themselves.
	''' </ul>
	''' <p>
	''' The global LogManager object can be retrieved using LogManager.getLogManager().
	''' The LogManager object is created during class initialization and
	''' cannot subsequently be changed.
	''' <p>
	''' At startup the LogManager class is located using the
	''' java.util.logging.manager system property.
	''' <p>
	''' The LogManager defines two optional system properties that allow control over
	''' the initial configuration:
	''' <ul>
	''' <li>"java.util.logging.config.class"
	''' <li>"java.util.logging.config.file"
	''' </ul>
	''' These two properties may be specified on the command line to the "java"
	''' command, or as system property definitions passed to JNI_CreateJavaVM.
	''' <p>
	''' If the "java.util.logging.config.class" property is set, then the
	''' property value is treated as a class name.  The given class will be
	''' loaded, an object will be instantiated, and that object's constructor
	''' is responsible for reading in the initial configuration.  (That object
	''' may use other system properties to control its configuration.)  The
	''' alternate configuration class can use <tt>readConfiguration(InputStream)</tt>
	''' to define properties in the LogManager.
	''' <p>
	''' If "java.util.logging.config.class" property is <b>not</b> set,
	''' then the "java.util.logging.config.file" system property can be used
	''' to specify a properties file (in java.util.Properties format). The
	''' initial logging configuration will be read from this file.
	''' <p>
	''' If neither of these properties is defined then the LogManager uses its
	''' default configuration. The default configuration is typically loaded from the
	''' properties file "{@code lib/logging.properties}" in the Java installation
	''' directory.
	''' <p>
	''' The properties for loggers and Handlers will have names starting
	''' with the dot-separated name for the handler or logger.
	''' <p>
	''' The global logging properties may include:
	''' <ul>
	''' <li>A property "handlers".  This defines a whitespace or comma separated
	''' list of class names for handler classes to load and register as
	''' handlers on the root Logger (the Logger named "").  Each class
	''' name must be for a Handler class which has a default constructor.
	''' Note that these Handlers may be created lazily, when they are
	''' first used.
	''' 
	''' <li>A property "&lt;logger&gt;.handlers". This defines a whitespace or
	''' comma separated list of class names for handlers classes to
	''' load and register as handlers to the specified logger. Each class
	''' name must be for a Handler class which has a default constructor.
	''' Note that these Handlers may be created lazily, when they are
	''' first used.
	''' 
	''' <li>A property "&lt;logger&gt;.useParentHandlers". This defines a boolean
	''' value. By default every logger calls its parent in addition to
	''' handling the logging message itself, this often result in messages
	''' being handled by the root logger as well. When setting this property
	''' to false a Handler needs to be configured for this logger otherwise
	''' no logging messages are delivered.
	''' 
	''' <li>A property "config".  This property is intended to allow
	''' arbitrary configuration code to be run.  The property defines a
	''' whitespace or comma separated list of class names.  A new instance will be
	''' created for each named class.  The default constructor of each class
	''' may execute arbitrary code to update the logging configuration, such as
	''' setting logger levels, adding handlers, adding filters, etc.
	''' </ul>
	''' <p>
	''' Note that all classes loaded during LogManager configuration are
	''' first searched on the system class path before any user class path.
	''' That includes the LogManager [Class], any config classes, and any
	''' handler classes.
	''' <p>
	''' Loggers are organized into a naming hierarchy based on their
	''' dot separated names.  Thus "a.b.c" is a child of "a.b", but
	''' "a.b1" and a.b2" are peers.
	''' <p>
	''' All properties whose names end with ".level" are assumed to define
	''' log levels for Loggers.  Thus "foo.level" defines a log level for
	''' the logger called "foo" and (recursively) for any of its children
	''' in the naming hierarchy.  Log Levels are applied in the order they
	''' are defined in the properties file.  Thus level settings for child
	''' nodes in the tree should come after settings for their parents.
	''' The property name ".level" can be used to set the level for the
	''' root of the tree.
	''' <p>
	''' All methods on the LogManager object are multi-thread safe.
	''' 
	''' @since 1.4
	''' </summary>

	Public Class LogManager
		' The global LogManager object
		Private Shared ReadOnly manager As LogManager

		' 'props' is assigned within a lock but accessed without it.
		' Declaring it volatile makes sure that another thread will not
		' be able to see a partially constructed 'props' object.
		' (seeing a partially constructed 'props' object can result in
		' NPE being thrown in Hashtable.get(), because it leaves the door
		' open for props.getProperties() to be called before the construcor
		' of Hashtable is actually completed).
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private props As New Properties
		Private Shared ReadOnly defaultLevel As Level = Level.INFO

		' The map of the registered listeners. The map value is the registration
		' count to allow for cases where the same listener is registered many times.
		Private ReadOnly listenerMap As Map(Of Object, Integer?) = New HashMap(Of Object, Integer?)

		' LoggerContext for system loggers and user loggers
		Private ReadOnly systemContext As LoggerContext = New SystemLoggerContext(Me)
		Private ReadOnly userContext As New LoggerContext(Me)
		' non final field - make it volatile to make sure that other threads
		' will see the new value once ensureLogManagerInitialized() has finished
		' executing.
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private rootLogger As Logger
		' Have we done the primordial reading of the configuration file?
		' (Must be done after a suitable amount of java.lang.System
		' initialization has been done)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private readPrimordialConfiguration_Renamed As Boolean
		' Have we initialized global (root) handlers yet?
		' This gets set to false in readConfiguration
		Private initializedGlobalHandlers As Boolean = True
		' True if JVM death is imminent and the exit hook has been called.
		Private deathImminent As Boolean

		Shared Sub New()
			manager = AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedAction(Of T)

			Public Overrides Function run() As LogManager
				Dim mgr As LogManager = Nothing
				Dim cname As String = Nothing
				Try
					cname = System.getProperty("java.util.logging.manager")
					If cname IsNot Nothing Then
						Try
							Dim clz As  [Class] = ClassLoader.systemClassLoader.loadClass(cname)
							mgr = CType(clz.newInstance(), LogManager)
						Catch ex As  ClassNotFoundException
							Dim clz As  [Class] = Thread.CurrentThread.contextClassLoader.loadClass(cname)
							mgr = CType(clz.newInstance(), LogManager)
						End Try
					End If
				Catch ex As Exception
					Console.Error.WriteLine("Could not load Logmanager """ & cname & """")
					ex.printStackTrace()
				End Try
				If mgr Is Nothing Then mgr = New LogManager
				Return mgr

			End Function
		End Class


		' This private class is used as a shutdown hook.
		' It does a "reset" to close all open handlers.
		Private Class Cleaner
			Inherits Thread

			Private ReadOnly outerInstance As LogManager


			Private Sub New(ByVal outerInstance As LogManager)
					Me.outerInstance = outerInstance
	'             Set context class loader to null in order to avoid
	'             * keeping a strong reference to an application classloader.
	'             
				Me.contextClassLoader = Nothing
			End Sub

			Public Overrides Sub run()
				' This is to ensure the LogManager.<clinit> is completed
				' before synchronized block. Otherwise deadlocks are possible.
				Dim mgr As LogManager = manager

				' If the global handlers haven't been initialized yet, we
				' don't want to initialize them just so we can close them!
				SyncLock LogManager.this
					' Note that death is imminent.
					outerInstance.deathImminent = True
					outerInstance.initializedGlobalHandlers = True
				End SyncLock

				' Do a reset to close all active handlers.
				outerInstance.reset()
			End Sub
		End Class


		''' <summary>
		''' Protected constructor.  This is protected so that container applications
		''' (such as J2EE containers) can subclass the object.  It is non-public as
		''' it is intended that there only be one LogManager object, whose value is
		''' retrieved by calling LogManager.getLogManager.
		''' </summary>
		Protected Friend Sub New()
			Me.New(checkSubclassPermissions())
		End Sub

		Private Sub New(ByVal checked As Void)

			' Add a shutdown hook to close the global handlers.
			Try
				Runtime.runtime.addShutdownHook(New Cleaner(Me))
			Catch e As IllegalStateException
				' If the VM is already shutting down,
				' We do not need to register shutdownHook.
			End Try
		End Sub

		Private Shared Function checkSubclassPermissions() As Void
			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then
				' These permission will be checked in the LogManager constructor,
				' in order to register the Cleaner() thread as a shutdown hook.
				' Check them here to avoid the penalty of constructing the object
				' etc...
				sm.checkPermission(New RuntimePermission("shutdownHooks"))
				sm.checkPermission(New RuntimePermission("setContextClassLoader"))
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Lazy initialization: if this instance of manager is the global
		''' manager then this method will read the initial configuration and
		''' add the root logger and global logger by calling addLogger().
		''' 
		''' Note that it is subtly different from what we do in LoggerContext.
		''' In LoggerContext we're patching up the logger context tree in order to add
		''' the root and global logger *to the context tree*.
		''' 
		''' For this to work, addLogger() must have already have been called
		''' once on the LogManager instance for the default logger being
		''' added.
		''' 
		''' This is why ensureLogManagerInitialized() needs to be called before
		''' any logger is added to any logger context.
		''' 
		''' </summary>
		Private initializedCalled As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private initializationDone As Boolean = False
		Friend Sub ensureLogManagerInitialized()
			Dim owner As LogManager = Me
			If initializationDone OrElse owner IsNot manager Then Return

			' Maybe another thread has called ensureLogManagerInitialized()
			' before us and is still executing it. If so we will block until
			' the log manager has finished initialized, then acquire the monitor,
			' notice that initializationDone is now true and return.
			' Otherwise - we have come here first! We will acquire the monitor,
			' see that initializationDone is still false, and perform the
			' initialization.
			'
			SyncLock Me
				' If initializedCalled is true it means that we're already in
				' the process of initializing the LogManager in this thread.
				' There has been a recursive call to ensureLogManagerInitialized().
				Dim isRecursiveInitialization As Boolean = (initializedCalled = True)

				Debug.Assert(initializedCalled OrElse (Not initializationDone), "Initialization can't be done if initialized has not been called!")

				If isRecursiveInitialization OrElse initializationDone Then Return
				' Calling addLogger below will in turn call requiresDefaultLogger()
				' which will call ensureLogManagerInitialized().
				' We use initializedCalled to break the recursion.
				initializedCalled = True
				Try
					AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
				Finally
					initializationDone = True
				End Try
			End SyncLock
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedAction(Of T)

			Public Overrides Function run() As Object
				Debug.Assert(outerInstance.rootLogger Is Nothing)
				Debug.Assert(outerInstance.initializedCalled AndAlso (Not outerInstance.initializationDone))

				' Read configuration.
				owner.readPrimordialConfiguration()

				' Create and retain Logger for the root of the namespace.
				owner.rootLogger = owner.new RootLogger()
				owner.addLogger(owner.rootLogger)
				If Not owner.rootLogger.levelInitialized Then owner.rootLogger.level = defaultLevel

				' Adding the global Logger.
				' Do not call Logger.getGlobal() here as this might trigger
				' subtle inter-dependency issues.
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim [global] As Logger = Logger.global

				' Make sure the global logger will be registered in the
				' global manager
				owner.addLogger([global])
				Return Nothing
			End Function
		End Class

		''' <summary>
		''' Returns the global LogManager object. </summary>
		''' <returns> the global LogManager object </returns>
		Public Property Shared logManager As LogManager
			Get
				If manager IsNot Nothing Then manager.ensureLogManagerInitialized()
				Return manager
			End Get
		End Property

		Private Sub readPrimordialConfiguration()
			If Not readPrimordialConfiguration_Renamed Then
				SyncLock Me
					If Not readPrimordialConfiguration_Renamed Then
						' If System.in/out/err are null, it's a good
						' indication that we're still in the
						' bootstrapping phase
						If System.out Is Nothing Then Return
						readPrimordialConfiguration_Renamed = True

						Try
							AccessController.doPrivileged(New PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
						Catch ex As Exception
							Debug.Assert(False, "Exception raised while reading logging configuration: " & ex)
						End Try
					End If
				End SyncLock
			End If
		End Sub

		Private Class PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedExceptionAction(Of T)

			Public Overrides Function run() As Void
				outerInstance.readConfiguration()

				' Platform loggers begin to delegate to java.util.logging.Logger
				sun.util.logging.PlatformLogger.redirectPlatformLoggers()
				Return Nothing
			End Function
		End Class

		''' <summary>
		''' Adds an event listener to be invoked when the logging
		''' properties are re-read. Adding multiple instances of
		''' the same event Listener results in multiple entries
		''' in the property event listener table.
		''' 
		''' <p><b>WARNING:</b> This method is omitted from this class in all subset
		''' Profiles of Java SE that do not include the {@code java.beans} package.
		''' </p>
		''' </summary>
		''' <param name="l">  event listener </param>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have LoggingPermission("control"). </exception>
		''' <exception cref="NullPointerException"> if the PropertyChangeListener is null. </exception>
		''' @deprecated The dependency on {@code PropertyChangeListener} creates a
		'''             significant impediment to future modularization of the Java
		'''             platform. This method will be removed in a future release.
		'''             The global {@code LogManager} can detect changes to the
		'''             logging configuration by overridding the {@link
		'''             #readConfiguration readConfiguration} method. 
		<Obsolete("The dependency on {@code PropertyChangeListener} creates a")> _
		Public Overridable Sub addPropertyChangeListener(ByVal l As java.beans.PropertyChangeListener)
			Dim listener As java.beans.PropertyChangeListener = Objects.requireNonNull(l)
			checkPermission()
			SyncLock listenerMap
				' increment the registration count if already registered
				Dim value As Integer? = listenerMap.get(listener)
				value = If(value Is Nothing, 1, (value + 1))
				listenerMap.put(listener, value)
			End SyncLock
		End Sub

		''' <summary>
		''' Removes an event listener for property change events.
		''' If the same listener instance has been added to the listener table
		''' through multiple invocations of <CODE>addPropertyChangeListener</CODE>,
		''' then an equivalent number of
		''' <CODE>removePropertyChangeListener</CODE> invocations are required to remove
		''' all instances of that listener from the listener table.
		''' <P>
		''' Returns silently if the given listener is not found.
		''' 
		''' <p><b>WARNING:</b> This method is omitted from this class in all subset
		''' Profiles of Java SE that do not include the {@code java.beans} package.
		''' </p>
		''' </summary>
		''' <param name="l">  event listener (can be null) </param>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have LoggingPermission("control"). </exception>
		''' @deprecated The dependency on {@code PropertyChangeListener} creates a
		'''             significant impediment to future modularization of the Java
		'''             platform. This method will be removed in a future release.
		'''             The global {@code LogManager} can detect changes to the
		'''             logging configuration by overridding the {@link
		'''             #readConfiguration readConfiguration} method. 
		<Obsolete("The dependency on {@code PropertyChangeListener} creates a")> _
		Public Overridable Sub removePropertyChangeListener(ByVal l As java.beans.PropertyChangeListener)
			checkPermission()
			If l IsNot Nothing Then
				Dim listener As java.beans.PropertyChangeListener = l
				SyncLock listenerMap
					Dim value As Integer? = listenerMap.get(listener)
					If value IsNot Nothing Then
						' remove from map if registration count is 1, otherwise
						' just decrement its count
						Dim i As Integer = value
						If i = 1 Then
							listenerMap.remove(listener)
						Else
							Debug.Assert(i > 1)
							listenerMap.put(listener, i - 1)
						End If
					End If
				End SyncLock
			End If
		End Sub

		' LoggerContext maps from AppContext
		Private contextsMap As WeakHashMap(Of Object, LoggerContext) = Nothing

		' Returns the LoggerContext for the user code (i.e. application or AppContext).
		' Loggers are isolated from each AppContext.
		Private Property userContext As LoggerContext
			Get
				Dim context As LoggerContext = Nothing
    
				Dim sm As SecurityManager = System.securityManager
				Dim javaAwtAccess As sun.misc.JavaAWTAccess = sun.misc.SharedSecrets.javaAWTAccess
				If sm IsNot Nothing AndAlso javaAwtAccess IsNot Nothing Then
					' for each applet, it has its own LoggerContext isolated from others
					Dim ecx As Object = javaAwtAccess.appletContext
					If ecx IsNot Nothing Then
						SyncLock javaAwtAccess
							' find the AppContext of the applet code
							' will be null if we are in the main app context.
							If contextsMap Is Nothing Then contextsMap = New WeakHashMap(Of )
							context = contextsMap.get(ecx)
							If context Is Nothing Then
								' Create a new LoggerContext for the applet.
								context = New LoggerContext(Me)
								contextsMap.put(ecx, context)
							End If
						End SyncLock
					End If
				End If
				' for standalone app, return userContext
				Return If(context IsNot Nothing, context, userContext)
			End Get
		End Property

		' The system context.
		Friend Property systemContext As LoggerContext
			Get
				Return systemContext
			End Get
		End Property

		Private Function contexts() As List(Of LoggerContext)
			Dim cxs As List(Of LoggerContext) = New List(Of LoggerContext)
			cxs.add(systemContext)
			cxs.add(userContext)
			Return cxs
		End Function

		' Find or create a specified logger instance. If a logger has
		' already been created with the given name it is returned.
		' Otherwise a new logger instance is created and registered
		' in the LogManager global namespace.
		' This method will always return a non-null Logger object.
		' Synchronization is not required here. All synchronization for
		' adding a new Logger object is handled by addLogger().
		'
		' This method must delegate to the LogManager implementation to
		' add a new Logger or return the one that has been added previously
		' as a LogManager subclass may override the addLogger, getLogger,
		' readConfiguration, and other methods.
		Friend Overridable Function demandLogger(ByVal name As String, ByVal resourceBundleName As String, ByVal caller As [Class]) As Logger
			Dim result As Logger = getLogger(name)
			If result Is Nothing Then
				' only allocate the new logger once
				Dim newLogger As New Logger(name, resourceBundleName, caller, Me, False)
				Do
					If addLogger(newLogger) Then Return newLogger

					' We didn't add the new Logger that we created above
					' because another thread added a Logger with the same
					' name after our null check above and before our call
					' to addLogger(). We have to refetch the Logger because
					' addLogger() returns a boolean instead of the Logger
					' reference itself. However, if the thread that created
					' the other Logger is not holding a strong reference to
					' the other Logger, then it is possible for the other
					' Logger to be GC'ed after we saw it in addLogger() and
					' before we can refetch it. If it has been GC'ed then
					' we'll just loop around and try again.
					result = getLogger(name)
				Loop While result Is Nothing
			End If
			Return result
		End Function

		Friend Overridable Function demandSystemLogger(ByVal name As String, ByVal resourceBundleName As String) As Logger
			' Add a system logger in the system context's namespace
			Dim sysLogger As Logger = systemContext.demandLogger(name, resourceBundleName)

			' Add the system logger to the LogManager's namespace if not exist
			' so that there is only one single logger of the given name.
			' System loggers are visible to applications unless a logger of
			' the same name has been added.
			Dim logger_Renamed As Logger
			Do
				' First attempt to call addLogger instead of getLogger
				' This would avoid potential bug in custom LogManager.getLogger
				' implementation that adds a logger if does not exist
				If addLogger(sysLogger) Then
					' successfully added the new system logger
					logger_Renamed = sysLogger
				Else
					logger_Renamed = getLogger(name)
				End If
			Loop While logger_Renamed Is Nothing

			' LogManager will set the sysLogger's handlers via LogManager.addLogger method.
			If logger_Renamed IsNot sysLogger AndAlso sysLogger.accessCheckedHandlers().Length = 0 Then
				' if logger already exists but handlers not set
				Dim l As Logger = logger_Renamed
				AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper2(Of T)
			End If
			Return sysLogger
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
			Implements PrivilegedAction(Of T)

			Public Overrides Function run() As Void
				For Each hdl As Handler In l.accessCheckedHandlers()
					sysLogger.addHandler(hdl)
				Next hdl
				Return Nothing
			End Function
		End Class

		' LoggerContext maintains the logger namespace per context.
		' The default LogManager implementation has one system context and user
		' context.  The system context is used to maintain the namespace for
		' all system loggers and is queried by the system code.  If a system logger
		' doesn't exist in the user context, it'll also be added to the user context.
		' The user context is queried by the user code and all other loggers are
		' added in the user context.
		Friend Class LoggerContext
			Private ReadOnly outerInstance As LogManager

			' Table of named Loggers that maps names to Loggers.
			Private ReadOnly namedLoggers As New Dictionary(Of String, LoggerWeakRef)
			' Tree of named Loggers
			Private ReadOnly root As LogNode
			Private Sub New(ByVal outerInstance As LogManager)
					Me.outerInstance = outerInstance
				Me.root = New LogNode(Nothing, Me)
			End Sub


			' Tells whether default loggers are required in this context.
			' If true, the default loggers will be lazily added.
			Friend Function requiresDefaultLoggers() As Boolean
				Dim requiresDefaultLoggers_Renamed As Boolean = (owner Is manager)
				If requiresDefaultLoggers_Renamed Then owner.ensureLogManagerInitialized()
				Return requiresDefaultLoggers_Renamed
			End Function

			' This context's LogManager.
			Friend Property owner As LogManager
				Get
					Return LogManager.this
				End Get
			End Property

			' This context owner's root logger, which if not null, and if
			' the context requires default loggers, will be added to the context
			' logger's tree.
			Friend Property rootLogger As Logger
				Get
					Return owner.rootLogger
				End Get
			End Property

			' The global logger, which if not null, and if
			' the context requires default loggers, will be added to the context
			' logger's tree.
			Friend Property globalLogger As Logger
				Get
	'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim [global] As Logger = Logger.global ' avoids initialization cycles.
					Return [global]
				End Get
			End Property

			Friend Overridable Function demandLogger(ByVal name As String, ByVal resourceBundleName As String) As Logger
				' a LogManager subclass may have its own implementation to add and
				' get a Logger.  So delegate to the LogManager to do the work.
				Dim owner_Renamed As LogManager = owner
				Return owner_Renamed.demandLogger(name, resourceBundleName, Nothing)
			End Function


			' Due to subtle deadlock issues getUserContext() no longer
			' calls addLocalLogger(rootLogger);
			' Therefore - we need to add the default loggers later on.
			' Checks that the context is properly initialized
			' This is necessary before calling e.g. find(name)
			' or getLoggerNames()
			'
			Private Sub ensureInitialized()
				If requiresDefaultLoggers() Then
					' Ensure that the root and global loggers are set.
					ensureDefaultLogger(rootLogger)
					ensureDefaultLogger(globalLogger)
				End If
			End Sub


			<MethodImpl(MethodImplOptions.Synchronized)> _
			Friend Overridable Function findLogger(ByVal name As String) As Logger
				' ensure that this context is properly initialized before
				' looking for loggers.
				ensureInitialized()
				Dim ref As LoggerWeakRef = namedLoggers.get(name)
				If ref Is Nothing Then Return Nothing
				Dim logger_Renamed As Logger = ref.get()
				If logger_Renamed Is Nothing Then ref.Dispose()
				Return logger_Renamed
			End Function

			' This method is called before adding a logger to the
			' context.
			' 'logger' is the context that will be added.
			' This method will ensure that the defaults loggers are added
			' before adding 'logger'.
			'
			Private Sub ensureAllDefaultLoggers(ByVal logger_Renamed As Logger)
				If requiresDefaultLoggers() Then
					Dim name As String = logger_Renamed.name
					If Not name.empty Then
						ensureDefaultLogger(rootLogger)
						If Not Logger.GLOBAL_LOGGER_NAME.Equals(name) Then ensureDefaultLogger(globalLogger)
					End If
				End If
			End Sub

			Private Sub ensureDefaultLogger(ByVal logger_Renamed As Logger)
				' Used for lazy addition of root logger and global logger
				' to a LoggerContext.

				' This check is simple sanity: we do not want that this
				' method be called for anything else than Logger.global
				' or owner.rootLogger.
				If (Not requiresDefaultLoggers()) OrElse logger_Renamed Is Nothing OrElse logger_Renamed IsNot Logger.global AndAlso logger_Renamed IsNot outerInstance.rootLogger Then

					' the case where we have a non null logger which is neither
					' Logger.global nor manager.rootLogger indicates a serious
					' issue - as ensureDefaultLogger should never be called
					' with any other loggers than one of these two (or null - if
					' e.g manager.rootLogger is not yet initialized)...
					Debug.Assert(logger_Renamed Is Nothing)

					Return
				End If

				' Adds the logger if it's not already there.
				If Not namedLoggers.containsKey(logger_Renamed.name) Then addLocalLogger(logger_Renamed, False)
			End Sub

			Friend Overridable Function addLocalLogger(ByVal logger_Renamed As Logger) As Boolean
				' no need to add default loggers if it's not required
				Return addLocalLogger(logger_Renamed, requiresDefaultLoggers())
			End Function

			' Add a logger to this context.  This method will only set its level
			' and process parent loggers.  It doesn't set its handlers.
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Friend Overridable Function addLocalLogger(ByVal logger_Renamed As Logger, ByVal addDefaultLoggersIfNeeded As Boolean) As Boolean
				' addDefaultLoggersIfNeeded serves to break recursion when adding
				' default loggers. If we're adding one of the default loggers
				' (we're being called from ensureDefaultLogger()) then
				' addDefaultLoggersIfNeeded will be false: we don't want to
				' call ensureAllDefaultLoggers again.
				'
				' Note: addDefaultLoggersIfNeeded can also be false when
				'       requiresDefaultLoggers is false - since calling
				'       ensureAllDefaultLoggers would have no effect in this case.
				If addDefaultLoggersIfNeeded Then ensureAllDefaultLoggers(logger_Renamed)

				Dim name As String = logger_Renamed.name
				If name Is Nothing Then Throw New NullPointerException
				Dim ref As LoggerWeakRef = namedLoggers.get(name)
				If ref IsNot Nothing Then
					If ref.get() Is Nothing Then
						' It's possible that the Logger was GC'ed after a
						' drainLoggerRefQueueBounded() call above so allow
						' a new one to be registered.
						ref.Dispose()
					Else
						' We already have a registered logger with the given name.
						Return False
					End If
				End If

				' We're adding a new logger.
				' Note that we are creating a weak reference here.
				Dim owner_Renamed As LogManager = owner
				logger_Renamed.logManager = owner_Renamed
				ref = owner_Renamed.new LoggerWeakRef(logger_Renamed)
				namedLoggers.put(name, ref)

				' Apply any initial level defined for the new logger, unless
				' the logger's level is already initialized
				Dim level_Renamed As Level = owner_Renamed.getLevelProperty(name & ".level", Nothing)
				If level_Renamed IsNot Nothing AndAlso (Not logger_Renamed.levelInitialized) Then doSetLevel(logger_Renamed, level_Renamed)

				' instantiation of the handler is done in the LogManager.addLogger
				' implementation as a handler class may be only visible to LogManager
				' subclass for the custom log manager case
				processParentHandlers(logger_Renamed, name)

				' Find the new node and its parent.
				Dim node_Renamed As LogNode = getNode(name)
				node_Renamed.loggerRef = ref
				Dim parent As Logger = Nothing
				Dim nodep As LogNode = node_Renamed.parent
				Do While nodep IsNot Nothing
					Dim nodeRef As LoggerWeakRef = nodep.loggerRef
					If nodeRef IsNot Nothing Then
						parent = nodeRef.get()
						If parent IsNot Nothing Then Exit Do
					End If
					nodep = nodep.parent
				Loop

				If parent IsNot Nothing Then doSetParent(logger_Renamed, parent)
				' Walk over the children and tell them we are their new parent.
				node_Renamed.walkAndSetParent(logger_Renamed)
				' new LogNode is ready so tell the LoggerWeakRef about it
				ref.node = node_Renamed
				Return True
			End Function

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Friend Overridable Sub removeLoggerRef(ByVal name As String, ByVal ref As LoggerWeakRef)
				namedLoggers.remove(name, ref)
			End Sub

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Friend Overridable Property loggerNames As Enumeration(Of String)
				Get
					' ensure that this context is properly initialized before
					' returning logger names.
					ensureInitialized()
					Return namedLoggers.keys()
				End Get
			End Property

			' If logger.getUseParentHandlers() returns 'true' and any of the logger's
			' parents have levels or handlers defined, make sure they are instantiated.
			Private Sub processParentHandlers(ByVal logger_Renamed As Logger, ByVal name As String)
				Dim owner_Renamed As LogManager = owner
				AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper3(Of T)

				Dim ix As Integer = 1
				Do
					Dim ix2 As Integer = name.IndexOf(".", ix)
					If ix2 < 0 Then Exit Do
					Dim pname As String = name.Substring(0, ix2)
					If owner_Renamed.getProperty(pname & ".level") IsNot Nothing OrElse owner_Renamed.getProperty(pname & ".handlers") IsNot Nothing Then demandLogger(pname, Nothing)
					ix = ix2+1
				Loop
			End Sub

			Private Class PrivilegedActionAnonymousInnerClassHelper3(Of T)
				Implements PrivilegedAction(Of T)

				Public Overrides Function run() As Void
					If logger <> owner.rootLogger Then
						Dim useParent As Boolean = owner.getBooleanProperty(name & ".useParentHandlers", True)
						If Not useParent Then logger.useParentHandlers = False
					End If
					Return Nothing
				End Function
			End Class

			' Gets a node in our tree of logger nodes.
			' If necessary, create it.
			Friend Overridable Function getNode(ByVal name As String) As LogNode
				If name Is Nothing OrElse name.Equals("") Then Return root
				Dim node_Renamed As LogNode = root
				Do While name.length() > 0
					Dim ix As Integer = name.IndexOf(".")
					Dim head As String
					If ix > 0 Then
						head = name.Substring(0, ix)
						name = name.Substring(ix + 1)
					Else
						head = name
						name = ""
					End If
					If node_Renamed.children Is Nothing Then node_Renamed.children = New HashMap(Of )
					Dim child As LogNode = node_Renamed.children.get(head)
					If child Is Nothing Then
						child = New LogNode(node_Renamed, Me)
						node_Renamed.children.put(head, child)
					End If
					node_Renamed = child
				Loop
				Return node_Renamed
			End Function
		End Class

		Friend NotInheritable Class SystemLoggerContext
			Inherits LoggerContext

			Private ReadOnly outerInstance As LogManager

			Public Sub New(ByVal outerInstance As LogManager)
				Me.outerInstance = outerInstance
			End Sub

			' Add a system logger in the system context's namespace as well as
			' in the LogManager's namespace if not exist so that there is only
			' one single logger of the given name.  System loggers are visible
			' to applications unless a logger of the same name has been added.
			Friend Overrides Function demandLogger(ByVal name As String, ByVal resourceBundleName As String) As Logger
				Dim result As Logger = findLogger(name)
				If result Is Nothing Then
					' only allocate the new system logger once
					Dim newLogger As New Logger(name, resourceBundleName, Nothing, owner, True)
					Do
						If addLocalLogger(newLogger) Then
							' We successfully added the new Logger that we
							' created above so return it without refetching.
							result = newLogger
						Else
							' We didn't add the new Logger that we created above
							' because another thread added a Logger with the same
							' name after our null check above and before our call
							' to addLogger(). We have to refetch the Logger because
							' addLogger() returns a boolean instead of the Logger
							' reference itself. However, if the thread that created
							' the other Logger is not holding a strong reference to
							' the other Logger, then it is possible for the other
							' Logger to be GC'ed after we saw it in addLogger() and
							' before we can refetch it. If it has been GC'ed then
							' we'll just loop around and try again.
							result = findLogger(name)
						End If
					Loop While result Is Nothing
				End If
				Return result
			End Function
		End Class

		' Add new per logger handlers.
		' We need to raise privilege here. All our decisions will
		' be made based on the logging configuration, which can
		' only be modified by trusted code.
		Private Sub loadLoggerHandlers(ByVal logger_Renamed As Logger, ByVal name As String, ByVal handlersPropertyName As String)
			AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedAction(Of T)

			Public Overrides Function run() As Object
				Dim names As String() = outerInstance.parseClassNames(handlersPropertyName)
				For i As Integer = 0 To names.Length - 1
					Dim word As String = names(i)
					Try
						Dim clz As  [Class] = ClassLoader.systemClassLoader.loadClass(word)
						Dim hdl As Handler = CType(clz.newInstance(), Handler)
						' Check if there is a property defining the
						' this handler's level.
						Dim levs As String = outerInstance.getProperty(word & ".level")
						If levs IsNot Nothing Then
							Dim l As Level = Level.findLevel(levs)
							If l IsNot Nothing Then
								hdl.level = l
							Else
								' Probably a bad level. Drop through.
								Console.Error.WriteLine("Can't set level for " & word)
							End If
						End If
						' Add this Handler to the logger
						logger.addHandler(hdl)
					Catch ex As Exception
						Console.Error.WriteLine("Can't load log handler """ & word & """")
						Console.Error.WriteLine("" & ex)
						ex.printStackTrace()
					End Try
				Next i
				Return Nothing
			End Function
		End Class


		' loggerRefQueue holds LoggerWeakRef objects for Logger objects
		' that have been GC'ed.
		Private ReadOnly loggerRefQueue As New ReferenceQueue(Of Logger)

		' Package-level inner class.
		' Helper class for managing WeakReferences to Logger objects.
		'
		' LogManager.namedLoggers
		'     - has weak references to all named Loggers
		'     - namedLoggers keeps the LoggerWeakRef objects for the named
		'       Loggers around until we can deal with the book keeping for
		'       the named Logger that is being GC'ed.
		' LogManager.LogNode.loggerRef
		'     - has a weak reference to a named Logger
		'     - the LogNode will also keep the LoggerWeakRef objects for
		'       the named Loggers around; currently LogNodes never go away.
		' Logger.kids
		'     - has a weak reference to each direct child Logger; this
		'       includes anonymous and named Loggers
		'     - anonymous Loggers are always children of the rootLogger
		'       which is a strong reference; rootLogger.kids keeps the
		'       LoggerWeakRef objects for the anonymous Loggers around
		'       until we can deal with the book keeping.
		'
		Friend NotInheritable Class LoggerWeakRef
			Inherits WeakReference(Of Logger)

			Private ReadOnly outerInstance As LogManager

			Private name As String ' for namedLoggers cleanup
			Private node As LogNode ' for loggerRef cleanup
			Private parentRef As WeakReference(Of Logger) ' for kids cleanup
			Private disposed As Boolean = False ' avoid calling dispose twice

			Friend Sub New(ByVal outerInstance As LogManager, ByVal logger_Renamed As Logger)
					Me.outerInstance = outerInstance
				MyBase.New(logger_Renamed, outerInstance.loggerRefQueue)

				name = logger_Renamed.name ' save for namedLoggers cleanup
			End Sub

			' dispose of this LoggerWeakRef object
			Friend Sub dispose()
				' Avoid calling dispose twice. When a Logger is gc'ed, its
				' LoggerWeakRef will be enqueued.
				' However, a new logger of the same name may be added (or looked
				' up) before the queue is drained. When that happens, dispose()
				' will be called by addLocalLogger() or findLogger().
				' Later when the queue is drained, dispose() will be called again
				' for the same LoggerWeakRef. Marking LoggerWeakRef as disposed
				' avoids processing the data twice (even though the code should
				' now be reentrant).
				SyncLock Me
					' Note to maintainers:
					' Be careful not to call any method that tries to acquire
					' another lock from within this block - as this would surely
					' lead to deadlocks, given that dispose() can be called by
					' multiple threads, and from within different synchronized
					' methods/blocks.
					If disposed Then Return
					disposed = True
				End SyncLock

				Dim n As LogNode = node
				If n IsNot Nothing Then
					' n.loggerRef can only be safely modified from within
					' a lock on LoggerContext. removeLoggerRef is already
					' synchronized on LoggerContext so calling
					' n.context.removeLoggerRef from within this lock is safe.
					SyncLock n.context
						' if we have a LogNode, then we were a named Logger
						' so clear namedLoggers weak ref to us
						n.context.removeLoggerRef(name, Me)
						name = Nothing ' clear our ref to the Logger's name

						' LogNode may have been reused - so only clear
						' LogNode.loggerRef if LogNode.loggerRef == this
						If n.loggerRef Is Me Then n.loggerRef = Nothing ' clear LogNode's weak ref to us
						node = Nothing ' clear our ref to LogNode
					End SyncLock
				End If

				If parentRef IsNot Nothing Then
					' this LoggerWeakRef has or had a parent Logger
					Dim parent As Logger = parentRef.get()
					If parent IsNot Nothing Then parent.removeChildLogger(Me)
					parentRef = Nothing ' clear our weak ref to the parent Logger
				End If
			End Sub

			' set the node field to the specified value
			Friend Property node As LogNode
				Set(ByVal node As LogNode)
					Me.node = node
				End Set
			End Property

			' set the parentRef field to the specified value
			Friend Property parentRef As WeakReference(Of Logger)
				Set(ByVal parentRef As WeakReference(Of Logger))
					Me.parentRef = parentRef
				End Set
			End Property
		End Class

		' Package-level method.
		' Drain some Logger objects that have been GC'ed.
		'
		' drainLoggerRefQueueBounded() is called by addLogger() below
		' and by Logger.getAnonymousLogger(String) so we'll drain up to
		' MAX_ITERATIONS GC'ed Loggers for every Logger we add.
		'
		' On a WinXP VMware client, a MAX_ITERATIONS value of 400 gives
		' us about a 50/50 mix in increased weak ref counts versus
		' decreased weak ref counts in the AnonLoggerWeakRefLeak test.
		' Here are stats for cleaning up sets of 400 anonymous Loggers:
		'   - test duration 1 minute
		'   - sample size of 125 sets of 400
		'   - average: 1.99 ms
		'   - minimum: 0.57 ms
		'   - maximum: 25.3 ms
		'
		' The same config gives us a better decreased weak ref count
		' than increased weak ref count in the LoggerWeakRefLeak test.
		' Here are stats for cleaning up sets of 400 named Loggers:
		'   - test duration 2 minutes
		'   - sample size of 506 sets of 400
		'   - average: 0.57 ms
		'   - minimum: 0.02 ms
		'   - maximum: 10.9 ms
		'
		Private Const MAX_ITERATIONS As Integer = 400
		Friend Sub drainLoggerRefQueueBounded()
			For i As Integer = 0 To MAX_ITERATIONS - 1
				If loggerRefQueue Is Nothing Then Exit For

				Dim ref As LoggerWeakRef = CType(loggerRefQueue.poll(), LoggerWeakRef)
				If ref Is Nothing Then Exit For
				' a Logger object has been GC'ed so clean it up
				ref.Dispose()
			Next i
		End Sub

		''' <summary>
		''' Add a named logger.  This does nothing and returns false if a logger
		''' with the same name is already registered.
		''' <p>
		''' The Logger factory methods call this method to register each
		''' newly created Logger.
		''' <p>
		''' The application should retain its own reference to the Logger
		''' object to avoid it being garbage collected.  The LogManager
		''' may only retain a weak reference.
		''' </summary>
		''' <param name="logger"> the new logger. </param>
		''' <returns>  true if the argument logger was registered successfully,
		'''          false if a logger of that name already exists. </returns>
		''' <exception cref="NullPointerException"> if the logger name is null. </exception>
		Public Overridable Function addLogger(ByVal logger_Renamed As Logger) As Boolean
			Dim name As String = logger_Renamed.name
			If name Is Nothing Then Throw New NullPointerException
			drainLoggerRefQueueBounded()
			Dim cx As LoggerContext = userContext
			If cx.addLocalLogger(logger_Renamed) Then
				' Do we have a per logger handler too?
				' Note: this will add a 200ms penalty
				loadLoggerHandlers(logger_Renamed, name, name & ".handlers")
				Return True
			Else
				Return False
			End If
		End Function

		' Private method to set a level on a logger.
		' If necessary, we raise privilege before doing the call.
		Private Shared Sub doSetLevel(ByVal logger_Renamed As Logger, ByVal level_Renamed As Level)
			Dim sm As SecurityManager = System.securityManager
			If sm Is Nothing Then
				' There is no security manager, so things are easy.
				logger_Renamed.level = level_Renamed
				Return
			End If
			' There is a security manager.  Raise privilege before
			' calling setLevel.
			AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedAction(Of T)

			Public Overrides Function run() As Object
				logger.level = level
				Return Nothing
			End Function
		End Class

		' Private method to set a parent on a logger.
		' If necessary, we raise privilege before doing the setParent call.
		Private Shared Sub doSetParent(ByVal logger_Renamed As Logger, ByVal parent As Logger)
			Dim sm As SecurityManager = System.securityManager
			If sm Is Nothing Then
				' There is no security manager, so things are easy.
				logger_Renamed.parent = parent
				Return
			End If
			' There is a security manager.  Raise privilege before
			' calling setParent.
			AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper2(Of T)
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
			Implements PrivilegedAction(Of T)

			Public Overrides Function run() As Object
				logger.parent = parent
				Return Nothing
			End Function
		End Class

		''' <summary>
		''' Method to find a named logger.
		''' <p>
		''' Note that since untrusted code may create loggers with
		''' arbitrary names this method should not be relied on to
		''' find Loggers for security sensitive logging.
		''' It is also important to note that the Logger associated with the
		''' String {@code name} may be garbage collected at any time if there
		''' is no strong reference to the Logger. The caller of this method
		''' must check the return value for null in order to properly handle
		''' the case where the Logger has been garbage collected.
		''' <p> </summary>
		''' <param name="name"> name of the logger </param>
		''' <returns>  matching logger or null if none is found </returns>
		Public Overridable Function getLogger(ByVal name As String) As Logger
			Return userContext.findLogger(name)
		End Function

		''' <summary>
		''' Get an enumeration of known logger names.
		''' <p>
		''' Note:  Loggers may be added dynamically as new classes are loaded.
		''' This method only reports on the loggers that are currently registered.
		''' It is also important to note that this method only returns the name
		''' of a Logger, not a strong reference to the Logger itself.
		''' The returned String does nothing to prevent the Logger from being
		''' garbage collected. In particular, if the returned name is passed
		''' to {@code LogManager.getLogger()}, then the caller must check the
		''' return value from {@code LogManager.getLogger()} for null to properly
		''' handle the case where the Logger has been garbage collected in the
		''' time since its name was returned by this method.
		''' <p> </summary>
		''' <returns>  enumeration of logger name strings </returns>
		Public Overridable Property loggerNames As Enumeration(Of String)
			Get
				Return userContext.loggerNames
			End Get
		End Property

		''' <summary>
		''' Reinitialize the logging properties and reread the logging configuration.
		''' <p>
		''' The same rules are used for locating the configuration properties
		''' as are used at startup.  So normally the logging properties will
		''' be re-read from the same file that was used at startup.
		''' <P>
		''' Any log level definitions in the new configuration file will be
		''' applied using Logger.setLevel(), if the target Logger exists.
		''' <p>
		''' A PropertyChangeEvent will be fired after the properties are read.
		''' </summary>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have LoggingPermission("control"). </exception>
		''' <exception cref="IOException"> if there are IO problems reading the configuration. </exception>
		Public Overridable Sub readConfiguration()
			checkPermission()

			' if a configuration class is specified, load it and use it.
			Dim cname As String = System.getProperty("java.util.logging.config.class")
			If cname IsNot Nothing Then
				Try
					' Instantiate the named class.  It is its constructor's
					' responsibility to initialize the logging configuration, by
					' calling readConfiguration(InputStream) with a suitable stream.
					Try
						Dim clz As  [Class] = ClassLoader.systemClassLoader.loadClass(cname)
						clz.newInstance()
						Return
					Catch ex As  ClassNotFoundException
						Dim clz As  [Class] = Thread.CurrentThread.contextClassLoader.loadClass(cname)
						clz.newInstance()
						Return
					End Try
				Catch ex As Exception
					Console.Error.WriteLine("Logging configuration class """ & cname & """ failed")
					Console.Error.WriteLine("" & ex)
					' keep going and useful config file.
				End Try
			End If

			Dim fname As String = System.getProperty("java.util.logging.config.file")
			If fname Is Nothing Then
				fname = System.getProperty("java.home")
				If fname Is Nothing Then Throw New [Error]("Can't find java.home ??")
				Dim f As New File(fname, "lib")
				f = New File(f, "logging.properties")
				fname = f.canonicalPath
			End If
			Using [in] As InputStream = New FileInputStream(fname)
				Dim bin As New BufferedInputStream([in])
				readConfiguration(bin)
			End Using
		End Sub

		''' <summary>
		''' Reset the logging configuration.
		''' <p>
		''' For all named loggers, the reset operation removes and closes
		''' all Handlers and (except for the root logger) sets the level
		''' to null.  The root logger's level is set to Level.INFO.
		''' </summary>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have LoggingPermission("control"). </exception>

		Public Overridable Sub reset()
			checkPermission()
			SyncLock Me
				props = New Properties
				' Since we are doing a reset we no longer want to initialize
				' the global handlers, if they haven't been initialized yet.
				initializedGlobalHandlers = True
			End SyncLock
			For Each cx As LoggerContext In contexts()
				Dim enum_ As Enumeration(Of String) = cx.loggerNames
				Do While enum_.hasMoreElements()
					Dim name As String = enum_.nextElement()
					Dim logger_Renamed As Logger = cx.findLogger(name)
					If logger_Renamed IsNot Nothing Then resetLogger(logger_Renamed)
				Loop
			Next cx
		End Sub

		' Private method to reset an individual target logger.
		Private Sub resetLogger(ByVal logger_Renamed As Logger)
			' Close all the Logger's handlers.
			Dim targets As Handler() = logger_Renamed.handlers
			For i As Integer = 0 To targets.Length - 1
				Dim h As Handler = targets(i)
				logger_Renamed.removeHandler(h)
				Try
					h.close()
				Catch ex As Exception
					' Problems closing a handler?  Keep going...
				End Try
			Next i
			Dim name As String = logger_Renamed.name
			If name IsNot Nothing AndAlso name.Equals("") Then
				' This is the root logger.
				logger_Renamed.level = defaultLevel
			Else
				logger_Renamed.level = Nothing
			End If
		End Sub

		' get a list of whitespace separated classnames from a property.
		Private Function parseClassNames(ByVal propertyName As String) As String()
			Dim hands As String = getProperty(propertyName)
			If hands Is Nothing Then Return New String(){}
			hands = hands.Trim()
			Dim ix As Integer = 0
			Dim result As List(Of String) = New List(Of String)
			Do While ix < hands.length()
				Dim [end] As Integer = ix
				Do While [end] < hands.length()
					If Char.IsWhiteSpace(hands.Chars([end])) Then Exit Do
					If hands.Chars([end]) = ","c Then Exit Do
					[end] += 1
				Loop
				Dim word As String = hands.Substring(ix, [end] - ix)
				ix = [end]+1
				word = word.Trim()
				If word.length() = 0 Then Continue Do
				result.add(word)
			Loop
			Return result.ToArray(New String(result.size() - 1){})
		End Function

		''' <summary>
		''' Reinitialize the logging properties and reread the logging configuration
		''' from the given stream, which should be in java.util.Properties format.
		''' A PropertyChangeEvent will be fired after the properties are read.
		''' <p>
		''' Any log level definitions in the new configuration file will be
		''' applied using Logger.setLevel(), if the target Logger exists.
		''' </summary>
		''' <param name="ins">       stream to read properties from </param>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have LoggingPermission("control"). </exception>
		''' <exception cref="IOException"> if there are problems reading from the stream. </exception>
		Public Overridable Sub readConfiguration(ByVal ins As InputStream)
			checkPermission()
			reset()

			' Load the properties
			props.load(ins)
			' Instantiate new configuration objects.
			Dim names As String() = parseClassNames("config")

			For i As Integer = 0 To names.Length - 1
				Dim word As String = names(i)
				Try
					Dim clz As  [Class] = ClassLoader.systemClassLoader.loadClass(word)
					clz.newInstance()
				Catch ex As Exception
					Console.Error.WriteLine("Can't load config class """ & word & """")
					Console.Error.WriteLine("" & ex)
					' ex.printStackTrace();
				End Try
			Next i

			' Set levels on any pre-existing loggers, based on the new properties.
			levelsOnExistingLoggersers()

			' Notify any interested parties that our properties have changed.
			' We first take a copy of the listener map so that we aren't holding any
			' locks when calling the listeners.
			Dim listeners As Map(Of Object, Integer?) = Nothing
			SyncLock listenerMap
				If Not listenerMap.empty Then listeners = New HashMap(Of )(listenerMap)
			End SyncLock
			If listeners IsNot Nothing Then
				Debug.Assert(Beans.beansPresent)
				Dim ev As Object = Beans.newPropertyChangeEvent(GetType(LogManager), Nothing, Nothing, Nothing)
				For Each entry As KeyValuePair(Of Object, Integer?) In listeners.entrySet()
					Dim listener As Object = entry.Key
					Dim count As Integer = entry.Value
					For i As Integer = 0 To count - 1
						Beans.invokePropertyChange(listener, ev)
					Next i
				Next entry
			End If


			' Note that we need to reinitialize global handles when
			' they are first referenced.
			SyncLock Me
				initializedGlobalHandlers = False
			End SyncLock
		End Sub

		''' <summary>
		''' Get the value of a logging property.
		''' The method returns null if the property is not found. </summary>
		''' <param name="name">      property name </param>
		''' <returns>          property value </returns>
		Public Overridable Function getProperty(ByVal name As String) As String
			Return props.getProperty(name)
		End Function

		' Package private method to get a String property.
		' If the property is not defined we return the given
		' default value.
		Friend Overridable Function getStringProperty(ByVal name As String, ByVal defaultValue As String) As String
			Dim val As String = getProperty(name)
			If val Is Nothing Then Return defaultValue
			Return val.Trim()
		End Function

		' Package private method to get an integer property.
		' If the property is not defined or cannot be parsed
		' we return the given default value.
		Friend Overridable Function getIntProperty(ByVal name As String, ByVal defaultValue As Integer) As Integer
			Dim val As String = getProperty(name)
			If val Is Nothing Then Return defaultValue
			Try
				Return Convert.ToInt32(val.Trim())
			Catch ex As Exception
				Return defaultValue
			End Try
		End Function

		' Package private method to get a boolean property.
		' If the property is not defined or cannot be parsed
		' we return the given default value.
		Friend Overridable Function getBooleanProperty(ByVal name As String, ByVal defaultValue As Boolean) As Boolean
			Dim val As String = getProperty(name)
			If val Is Nothing Then Return defaultValue
			val = val.ToLower()
			If val.Equals("true") OrElse val.Equals("1") Then
				Return True
			ElseIf val.Equals("false") OrElse val.Equals("0") Then
				Return False
			End If
			Return defaultValue
		End Function

		' Package private method to get a Level property.
		' If the property is not defined or cannot be parsed
		' we return the given default value.
		Friend Overridable Function getLevelProperty(ByVal name As String, ByVal defaultValue As Level) As Level
			Dim val As String = getProperty(name)
			If val Is Nothing Then Return defaultValue
			Dim l As Level = Level.findLevel(val.Trim())
			Return If(l IsNot Nothing, l, defaultValue)
		End Function

		' Package private method to get a filter property.
		' We return an instance of the class named by the "name"
		' property. If the property is not defined or has problems
		' we return the defaultValue.
		Friend Overridable Function getFilterProperty(ByVal name As String, ByVal defaultValue As Filter) As Filter
			Dim val As String = getProperty(name)
			Try
				If val IsNot Nothing Then
					Dim clz As  [Class] = ClassLoader.systemClassLoader.loadClass(val)
					Return CType(clz.newInstance(), Filter)
				End If
			Catch ex As Exception
				' We got one of a variety of exceptions in creating the
				' class or creating an instance.
				' Drop through.
			End Try
			' We got an exception.  Return the defaultValue.
			Return defaultValue
		End Function


		' Package private method to get a formatter property.
		' We return an instance of the class named by the "name"
		' property. If the property is not defined or has problems
		' we return the defaultValue.
		Friend Overridable Function getFormatterProperty(ByVal name As String, ByVal defaultValue As Formatter) As Formatter
			Dim val As String = getProperty(name)
			Try
				If val IsNot Nothing Then
					Dim clz As  [Class] = ClassLoader.systemClassLoader.loadClass(val)
					Return CType(clz.newInstance(), Formatter)
				End If
			Catch ex As Exception
				' We got one of a variety of exceptions in creating the
				' class or creating an instance.
				' Drop through.
			End Try
			' We got an exception.  Return the defaultValue.
			Return defaultValue
		End Function

		' Private method to load the global handlers.
		' We do the real work lazily, when the global handlers
		' are first used.
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub initializeGlobalHandlers()
			If initializedGlobalHandlers Then Return

			initializedGlobalHandlers = True

			If deathImminent Then Return
			loadLoggerHandlers(rootLogger, Nothing, "handlers")
		End Sub

		Private ReadOnly controlPermission As Permission = New LoggingPermission("control", Nothing)

		Friend Overridable Sub checkPermission()
			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then sm.checkPermission(controlPermission)
		End Sub

		''' <summary>
		''' Check that the current context is trusted to modify the logging
		''' configuration.  This requires LoggingPermission("control").
		''' <p>
		''' If the check fails we throw a SecurityException, otherwise
		''' we return normally.
		''' </summary>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have LoggingPermission("control"). </exception>
		Public Overridable Sub checkAccess()
			checkPermission()
		End Sub

		' Nested class to represent a node in our tree of named loggers.
		Private Class LogNode
			Friend children As HashMap(Of String, LogNode)
			Friend loggerRef As LoggerWeakRef
			Friend parent As LogNode
			Friend ReadOnly context As LoggerContext

			Friend Sub New(ByVal parent As LogNode, ByVal context As LoggerContext)
				Me.parent = parent
				Me.context = context
			End Sub

			' Recursive method to walk the tree below a node and set
			' a new parent logger.
			Friend Overridable Sub walkAndSetParent(ByVal parent As Logger)
				If children Is Nothing Then Return
				Dim values As [Iterator](Of LogNode) = children.values().GetEnumerator()
				Do While values.MoveNext()
					Dim node As LogNode = values.Current
					Dim ref As LoggerWeakRef = node.loggerRef
					Dim logger_Renamed As Logger = If(ref Is Nothing, Nothing, ref.get())
					If logger_Renamed Is Nothing Then
						node.walkAndSetParent(parent)
					Else
						doSetParent(logger_Renamed, parent)
					End If
				Loop
			End Sub
		End Class

		' We use a subclass of Logger for the root logger, so
		' that we only instantiate the global handlers when they
		' are first needed.
		Private NotInheritable Class RootLogger
			Inherits Logger

			Private ReadOnly outerInstance As LogManager

			Private Sub New(ByVal outerInstance As LogManager)
					Me.outerInstance = outerInstance
				' We do not call the protected Logger two args constructor here,
				' to avoid calling LogManager.getLogManager() from within the
				' RootLogger constructor.
				MyBase.New("", Nothing, Nothing, LogManager.this, True)
			End Sub

			Public Overrides Sub log(ByVal record As LogRecord)
				' Make sure that the global handlers have been instantiated.
				outerInstance.initializeGlobalHandlers()
				MyBase.log(record)
			End Sub

			Public Overrides Sub [addHandler](ByVal h As Handler)
				outerInstance.initializeGlobalHandlers()
				MyBase.addHandler(h)
			End Sub

			Public Overrides Sub [removeHandler](ByVal h As Handler)
				outerInstance.initializeGlobalHandlers()
				MyBase.removeHandler(h)
			End Sub

			Friend Overrides Function accessCheckedHandlers() As Handler()
				outerInstance.initializeGlobalHandlers()
				Return MyBase.accessCheckedHandlers()
			End Function
		End Class


		' Private method to be called when the configuration has
		' changed to apply any level settings to any pre-existing loggers.
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub setLevelsOnExistingLoggers()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim enum_ As Enumeration(Of ?) = props.propertyNames()
			Do While enum_.hasMoreElements()
				Dim key As String = CStr(enum_.nextElement())
				If Not key.EndsWith(".level") Then Continue Do
				Dim ix As Integer = key.length() - 6
				Dim name As String = key.Substring(0, ix)
				Dim level_Renamed As Level = getLevelProperty(key, Nothing)
				If level_Renamed Is Nothing Then
					Console.Error.WriteLine("Bad level value for property: " & key)
					Continue Do
				End If
				For Each cx As LoggerContext In contexts()
					Dim l As Logger = cx.findLogger(name)
					If l Is Nothing Then Continue For
					l.level = level_Renamed
				Next cx
			Loop
		End Sub

		' Management Support
		Private Shared loggingMXBean As LoggingMXBean = Nothing
		''' <summary>
		''' String representation of the
		''' <seealso cref="javax.management.ObjectName"/> for the management interface
		''' for the logging facility.
		''' </summary>
		''' <seealso cref= java.lang.management.PlatformLoggingMXBean </seealso>
		''' <seealso cref= java.util.logging.LoggingMXBean
		''' 
		''' @since 1.5 </seealso>
		Public Const LOGGING_MXBEAN_NAME As String = "java.util.logging:type=Logging"

		''' <summary>
		''' Returns <tt>LoggingMXBean</tt> for managing loggers.
		''' An alternative way to manage loggers is through the
		''' <seealso cref="java.lang.management.PlatformLoggingMXBean"/> interface
		''' that can be obtained by calling:
		''' <pre>
		'''     PlatformLoggingMXBean logging = {@link java.lang.management.ManagementFactory#getPlatformMXBean(Class)
		'''         ManagementFactory.getPlatformMXBean}(PlatformLoggingMXBean.class);
		''' </pre>
		''' </summary>
		''' <returns> a <seealso cref="LoggingMXBean"/> object.
		''' </returns>
		''' <seealso cref= java.lang.management.PlatformLoggingMXBean
		''' @since 1.5 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Property Shared loggingMXBean As LoggingMXBean
			Get
				If loggingMXBean Is Nothing Then loggingMXBean = New Logging
				Return loggingMXBean
			End Get
		End Property

		''' <summary>
		''' A class that provides access to the java.beans.PropertyChangeListener
		''' and java.beans.PropertyChangeEvent without creating a static dependency
		''' on java.beans. This class can be removed once the addPropertyChangeListener
		''' and removePropertyChangeListener methods are removed.
		''' </summary>
		Private Class Beans
			Private Shared ReadOnly propertyChangeListenerClass As  [Class] = getClass("java.beans.PropertyChangeListener")

			Private Shared ReadOnly propertyChangeEventClass As  [Class] = getClass("java.beans.PropertyChangeEvent")

			Private Shared ReadOnly propertyChangeMethod As Method = getMethod(propertyChangeListenerClass, "propertyChange", propertyChangeEventClass)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private Shared ReadOnly propertyEventCtor As Constructor(Of ?) = getConstructor(propertyChangeEventClass, GetType(Object), GetType(String), GetType(Object), GetType(Object))

			Private Shared Function getClass(ByVal name As String) As  [Class]
				Try
					Return Type.GetType(name, True, GetType(Beans).classLoader)
				Catch e As  ClassNotFoundException
					Return Nothing
				End Try
			End Function
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private Shared Function getConstructor(ByVal c As [Class], ParamArray ByVal types As  [Class]()) As Constructor(Of ?)
				Try
					Return If(c Is Nothing, Nothing, c.getDeclaredConstructor(types))
				Catch x As NoSuchMethodException
					Throw New AssertionError(x)
				End Try
			End Function

			Private Shared Function getMethod(ByVal c As [Class], ByVal name As String, ParamArray ByVal types As  [Class]()) As Method
				Try
					Return If(c Is Nothing, Nothing, c.getMethod(name, types))
				Catch e As NoSuchMethodException
					Throw New AssertionError(e)
				End Try
			End Function

			''' <summary>
			''' Returns {@code true} if java.beans is present.
			''' </summary>
			Friend Property Shared beansPresent As Boolean
				Get
					Return propertyChangeListenerClass IsNot Nothing AndAlso propertyChangeEventClass IsNot Nothing
				End Get
			End Property

			''' <summary>
			''' Returns a new PropertyChangeEvent with the given source, property
			''' name, old and new values.
			''' </summary>
			Friend Shared Function newPropertyChangeEvent(ByVal source As Object, ByVal prop As String, ByVal oldValue As Object, ByVal newValue As Object) As Object
				Try
					Return propertyEventCtor.newInstance(source, prop, oldValue, newValue)
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
				Catch InstantiationException Or IllegalAccessException x
					Throw New AssertionError(x)
				Catch x As InvocationTargetException
					Dim cause As Throwable = x.InnerException
					If TypeOf cause Is Error Then Throw CType(cause, [Error])
					If TypeOf cause Is RuntimeException Then Throw CType(cause, RuntimeException)
					Throw New AssertionError(x)
				End Try
			End Function

			''' <summary>
			''' Invokes the given PropertyChangeListener's propertyChange method
			''' with the given event.
			''' </summary>
			Friend Shared Sub invokePropertyChange(ByVal listener As Object, ByVal ev As Object)
				Try
					propertyChangeMethod.invoke(listener, ev)
				Catch x As IllegalAccessException
					Throw New AssertionError(x)
				Catch x As InvocationTargetException
					Dim cause As Throwable = x.InnerException
					If TypeOf cause Is Error Then Throw CType(cause, [Error])
					If TypeOf cause Is RuntimeException Then Throw CType(cause, RuntimeException)
					Throw New AssertionError(x)
				End Try
			End Sub
		End Class
	End Class

End Namespace
Imports System
Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Collections.Generic
Imports System.Threading

'
' * Copyright (c) 2000, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' A Logger object is used to log messages for a specific
	''' system or application component.  Loggers are normally named,
	''' using a hierarchical dot-separated namespace.  Logger names
	''' can be arbitrary strings, but they should normally be based on
	''' the package name or class name of the logged component, such
	''' as java.net or javax.swing.  In addition it is possible to create
	''' "anonymous" Loggers that are not stored in the Logger namespace.
	''' <p>
	''' Logger objects may be obtained by calls on one of the getLogger
	''' factory methods.  These will either create a new Logger or
	''' return a suitable existing Logger. It is important to note that
	''' the Logger returned by one of the {@code getLogger} factory methods
	''' may be garbage collected at any time if a strong reference to the
	''' Logger is not kept.
	''' <p>
	''' Logging messages will be forwarded to registered Handler
	''' objects, which can forward the messages to a variety of
	''' destinations, including consoles, files, OS logs, etc.
	''' <p>
	''' Each Logger keeps track of a "parent" Logger, which is its
	''' nearest existing ancestor in the Logger namespace.
	''' <p>
	''' Each Logger has a "Level" associated with it.  This reflects
	''' a minimum Level that this logger cares about.  If a Logger's
	''' level is set to <tt>null</tt>, then its effective level is inherited
	''' from its parent, which may in turn obtain it recursively from its
	''' parent, and so on up the tree.
	''' <p>
	''' The log level can be configured based on the properties from the
	''' logging configuration file, as described in the description
	''' of the LogManager class.  However it may also be dynamically changed
	''' by calls on the Logger.setLevel method.  If a logger's level is
	''' changed the change may also affect child loggers, since any child
	''' logger that has <tt>null</tt> as its level will inherit its
	''' effective level from its parent.
	''' <p>
	''' On each logging call the Logger initially performs a cheap
	''' check of the request level (e.g., SEVERE or FINE) against the
	''' effective log level of the logger.  If the request level is
	''' lower than the log level, the logging call returns immediately.
	''' <p>
	''' After passing this initial (cheap) test, the Logger will allocate
	''' a LogRecord to describe the logging message.  It will then call a
	''' Filter (if present) to do a more detailed check on whether the
	''' record should be published.  If that passes it will then publish
	''' the LogRecord to its output Handlers.  By default, loggers also
	''' publish to their parent's Handlers, recursively up the tree.
	''' <p>
	''' Each Logger may have a {@code ResourceBundle} associated with it.
	''' The {@code ResourceBundle} may be specified by name, using the
	''' <seealso cref="#getLogger(java.lang.String, java.lang.String)"/> factory
	''' method, or by value - using the {@link
	''' #setResourceBundle(java.util.ResourceBundle) setResourceBundle} method.
	''' This bundle will be used for localizing logging messages.
	''' If a Logger does not have its own {@code ResourceBundle} or resource bundle
	''' name, then it will inherit the {@code ResourceBundle} or resource bundle name
	''' from its parent, recursively up the tree.
	''' <p>
	''' Most of the logger output methods take a "msg" argument.  This
	''' msg argument may be either a raw value or a localization key.
	''' During formatting, if the logger has (or inherits) a localization
	''' {@code ResourceBundle} and if the {@code ResourceBundle} has a mapping for
	''' the msg string, then the msg string is replaced by the localized value.
	''' Otherwise the original msg string is used.  Typically, formatters use
	''' java.text.MessageFormat style formatting to format parameters, so
	''' for example a format string "{0} {1}" would format two parameters
	''' as strings.
	''' <p>
	''' A set of methods alternatively take a "msgSupplier" instead of a "msg"
	''' argument.  These methods take a <seealso cref="Supplier"/>{@code <String>} function
	''' which is invoked to construct the desired log message only when the message
	''' actually is to be logged based on the effective log level thus eliminating
	''' unnecessary message construction. For example, if the developer wants to
	''' log system health status for diagnosis, with the String-accepting version,
	''' the code would look like:
	''' <pre><code>
	''' 
	'''   class DiagnosisMessages {
	'''     static String systemHealthStatus() {
	'''       // collect system health information
	'''       ...
	'''     }
	'''   }
	'''   ...
	'''   logger.log(Level.FINER, DiagnosisMessages.systemHealthStatus());
	''' </code></pre>
	''' With the above code, the health status is collected unnecessarily even when
	''' the log level FINER is disabled. With the Supplier-accepting version as
	''' below, the status will only be collected when the log level FINER is
	''' enabled.
	''' <pre><code>
	''' 
	'''   logger.log(Level.FINER, DiagnosisMessages::systemHealthStatus);
	''' </code></pre>
	''' <p>
	''' When looking for a {@code ResourceBundle}, the logger will first look at
	''' whether a bundle was specified using {@link
	''' #setResourceBundle(java.util.ResourceBundle) setResourceBundle}, and then
	''' only whether a resource bundle name was specified through the {@link
	''' #getLogger(java.lang.String, java.lang.String) getLogger} factory method.
	''' If no {@code ResourceBundle} or no resource bundle name is found,
	''' then it will use the nearest {@code ResourceBundle} or resource bundle
	''' name inherited from its parent tree.<br>
	''' When a {@code ResourceBundle} was inherited or specified through the
	''' {@link
	''' #setResourceBundle(java.util.ResourceBundle) setResourceBundle} method, then
	''' that {@code ResourceBundle} will be used. Otherwise if the logger only
	''' has or inherited a resource bundle name, then that resource bundle name
	''' will be mapped to a {@code ResourceBundle} object, using the default Locale
	''' at the time of logging.
	''' <br id="ResourceBundleMapping">When mapping resource bundle names to
	''' {@code ResourceBundle} objects, the logger will first try to use the
	''' Thread's {@link java.lang.Thread#getContextClassLoader() context class
	''' loader} to map the given resource bundle name to a {@code ResourceBundle}.
	''' If the thread context class loader is {@code null}, it will try the
	''' <seealso cref="java.lang.ClassLoader#getSystemClassLoader() system class loader"/>
	''' instead.  If the {@code ResourceBundle} is still not found, it will use the
	''' class loader of the first caller of the {@link
	''' #getLogger(java.lang.String, java.lang.String) getLogger} factory method.
	''' <p>
	''' Formatting (including localization) is the responsibility of
	''' the output Handler, which will typically call a Formatter.
	''' <p>
	''' Note that formatting need not occur synchronously.  It may be delayed
	''' until a LogRecord is actually written to an external sink.
	''' <p>
	''' The logging methods are grouped in five main categories:
	''' <ul>
	''' <li><p>
	'''     There are a set of "log" methods that take a log level, a message
	'''     string, and optionally some parameters to the message string.
	''' <li><p>
	'''     There are a set of "logp" methods (for "log precise") that are
	'''     like the "log" methods, but also take an explicit source class name
	'''     and method name.
	''' <li><p>
	'''     There are a set of "logrb" method (for "log with resource bundle")
	'''     that are like the "logp" method, but also take an explicit resource
	'''     bundle object for use in localizing the log message.
	''' <li><p>
	'''     There are convenience methods for tracing method entries (the
	'''     "entering" methods), method returns (the "exiting" methods) and
	'''     throwing exceptions (the "throwing" methods).
	''' <li><p>
	'''     Finally, there are a set of convenience methods for use in the
	'''     very simplest cases, when a developer simply wants to log a
	'''     simple string at a given log level.  These methods are named
	'''     after the standard Level names ("severe", "warning", "info", etc.)
	'''     and take a single argument, a message string.
	''' </ul>
	''' <p>
	''' For the methods that do not take an explicit source name and
	''' method name, the Logging framework will make a "best effort"
	''' to determine which class and method called into the logging method.
	''' However, it is important to realize that this automatically inferred
	''' information may only be approximate (or may even be quite wrong!).
	''' Virtual machines are allowed to do extensive optimizations when
	''' JITing and may entirely remove stack frames, making it impossible
	''' to reliably locate the calling class and method.
	''' <P>
	''' All methods on Logger are multi-thread safe.
	''' <p>
	''' <b>Subclassing Information:</b> Note that a LogManager class may
	''' provide its own implementation of named Loggers for any point in
	''' the namespace.  Therefore, any subclasses of Logger (unless they
	''' are implemented in conjunction with a new LogManager [Class]) should
	''' take care to obtain a Logger instance from the LogManager class and
	''' should delegate operations such as "isLoggable" and "log(LogRecord)"
	''' to that instance.  Note that in order to intercept all logging
	''' output, subclasses need only override the log(LogRecord) method.
	''' All the other logging methods are implemented as calls on this
	''' log(LogRecord) method.
	''' 
	''' @since 1.4
	''' </summary>
	Public Class Logger
		Private Shared ReadOnly emptyHandlers As Handler() = New Handler(){}
		Private Shared ReadOnly offValue As Integer = Level.OFF

		Friend Const SYSTEM_LOGGER_RB_NAME As String = "sun.util.logging.resources.logging"

		' This class is immutable and it is important that it remains so.
		Private NotInheritable Class LoggerBundle
			Friend ReadOnly resourceBundleName As String ' Base name of the bundle.
			Friend ReadOnly userBundle As java.util.ResourceBundle ' Bundle set through setResourceBundle.
			Private Sub New(ByVal resourceBundleName As String, ByVal bundle As java.util.ResourceBundle)
				Me.resourceBundleName = resourceBundleName
				Me.userBundle = bundle
			End Sub
			Friend Property systemBundle As Boolean
				Get
					Return SYSTEM_LOGGER_RB_NAME.Equals(resourceBundleName)
				End Get
			End Property
			Shared Function [get](ByVal name As String, ByVal bundle As java.util.ResourceBundle) As LoggerBundle
				If name Is Nothing AndAlso bundle Is Nothing Then
					Return NO_RESOURCE_BUNDLE
				ElseIf SYSTEM_LOGGER_RB_NAME.Equals(name) AndAlso bundle Is Nothing Then
					Return SYSTEM_BUNDLE
				Else
					Return New LoggerBundle(name, bundle)
				End If
			End Function
		End Class

		' This instance will be shared by all loggers created by the system
		' code
		Private Shared ReadOnly SYSTEM_BUNDLE As New LoggerBundle(SYSTEM_LOGGER_RB_NAME, Nothing)

		' This instance indicates that no resource bundle has been specified yet,
		' and it will be shared by all loggers which have no resource bundle.
		Private Shared ReadOnly NO_RESOURCE_BUNDLE As New LoggerBundle(Nothing, Nothing)

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private manager As LogManager
		Private name As String
		Private ReadOnly handlers As New java.util.concurrent.CopyOnWriteArrayList(Of Handler)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private loggerBundle_Renamed As LoggerBundle = NO_RESOURCE_BUNDLE
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private useParentHandlers As Boolean = True
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private filter As Filter
		Private anonymous As Boolean

		' Cache to speed up behavior of findResourceBundle:
		Private catalog As java.util.ResourceBundle ' Cached resource bundle
		Private catalogName As String ' name associated with catalog
		Private catalogLocale As java.util.Locale ' locale associated with catalog

		' The fields relating to parent-child relationships and levels
		' are managed under a separate lock, the treeLock.
		Private Shared ReadOnly treeLock As New Object
		' We keep weak references from parents to children, but strong
		' references from children to parents.
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private parent As Logger ' our nearest parent.
		Private kids As List(Of LogManager.LoggerWeakRef) ' WeakReferences to loggers that have us as parent
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private levelObject As Level
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private levelValue As Integer ' current effective level value
		Private callersClassLoaderRef As WeakReference(Of ClassLoader)
		Private ReadOnly isSystemLogger As Boolean

		''' <summary>
		''' GLOBAL_LOGGER_NAME is a name for the global logger.
		''' 
		''' @since 1.6
		''' </summary>
		Public Const GLOBAL_LOGGER_NAME As String = "global"

		''' <summary>
		''' Return global logger object with the name Logger.GLOBAL_LOGGER_NAME.
		''' </summary>
		''' <returns> global logger object
		''' @since 1.7 </returns>
		Public Property Shared [global] As Logger
			Get
				' In order to break a cyclic dependence between the LogManager
				' and Logger static initializers causing deadlocks, the global
				' logger is created with a special constructor that does not
				' initialize its log manager.
				'
				' If an application calls Logger.getGlobal() before any logger
				' has been initialized, it is therefore possible that the
				' LogManager class has not been initialized yet, and therefore
				' Logger.global.manager will be null.
				'
				' In order to finish the initialization of the global logger, we
				' will therefore call LogManager.getLogManager() here.
				'
				' To prevent race conditions we also need to call
				' LogManager.getLogManager() unconditionally here.
				' Indeed we cannot rely on the observed value of global.manager,
				' because global.manager will become not null somewhere during
				' the initialization of LogManager.
				' If two threads are calling getGlobal() concurrently, one thread
				' will see global.manager null and call LogManager.getLogManager(),
				' but the other thread could come in at a time when global.manager
				' is already set although ensureLogManagerInitialized is not finished
				' yet...
				' Calling LogManager.getLogManager() unconditionally will fix that.
    
				LogManager.logManager
    
				' Now the global LogManager should be initialized,
				' and the global logger should have been added to
				' it, unless we were called within the constructor of a LogManager
				' subclass installed as LogManager, in which case global.manager
				' would still be null, and global will be lazily initialized later on.
    
				Return [global]
			End Get
		End Property

		''' <summary>
		''' The "global" Logger object is provided as a convenience to developers
		''' who are making casual use of the Logging package.  Developers
		''' who are making serious use of the logging package (for example
		''' in products) should create and use their own Logger objects,
		''' with appropriate names, so that logging can be controlled on a
		''' suitable per-Logger granularity. Developers also need to keep a
		''' strong reference to their Logger objects to prevent them from
		''' being garbage collected.
		''' <p> </summary>
		''' @deprecated Initialization of this field is prone to deadlocks.
		''' The field must be initialized by the Logger class initialization
		''' which may cause deadlocks with the LogManager class initialization.
		''' In such cases two class initialization wait for each other to complete.
		''' The preferred way to get the global logger object is via the call
		''' <code>Logger.getGlobal()</code>.
		''' For compatibility with old JDK versions where the
		''' <code>Logger.getGlobal()</code> is not available use the call
		''' <code>Logger.getLogger(Logger.GLOBAL_LOGGER_NAME)</code>
		''' or <code>Logger.getLogger("global")</code>. 
		<Obsolete("Initialization of this field is prone to deadlocks.")> _
		Public Shared ReadOnly [global] As New Logger(GLOBAL_LOGGER_NAME)

		''' <summary>
		''' Protected method to construct a logger for a named subsystem.
		''' <p>
		''' The logger will be initially configured with a null Level
		''' and with useParentHandlers set to true.
		''' </summary>
		''' <param name="name">    A name for the logger.  This should
		'''                          be a dot-separated name and should normally
		'''                          be based on the package name or class name
		'''                          of the subsystem, such as java.net
		'''                          or javax.swing.  It may be null for anonymous Loggers. </param>
		''' <param name="resourceBundleName">  name of ResourceBundle to be used for localizing
		'''                          messages for this logger.  May be null if none
		'''                          of the messages require localization. </param>
		''' <exception cref="MissingResourceException"> if the resourceBundleName is non-null and
		'''             no corresponding resource can be found. </exception>
		Protected Friend Sub New(ByVal name As String, ByVal resourceBundleName As String)
			Me.New(name, resourceBundleName, Nothing, LogManager.logManager, False)
		End Sub

		Friend Sub New(ByVal name As String, ByVal resourceBundleName As String, ByVal caller As [Class], ByVal manager As LogManager, ByVal isSystemLogger As Boolean)
			Me.manager = manager
			Me.isSystemLogger = isSystemLogger
			setupResourceInfo(resourceBundleName, caller)
			Me.name = name
			levelValue = Level.INFO
		End Sub

		Private Property callersClassLoaderRef As  [Class]
			Set(ByVal caller As [Class])
				Dim callersClassLoader_Renamed As  ClassLoader = (If(caller IsNot Nothing, caller.classLoader, Nothing))
				If callersClassLoader_Renamed IsNot Nothing Then Me.callersClassLoaderRef = New WeakReference(Of )(callersClassLoader_Renamed)
			End Set
		End Property

		Private Property callersClassLoader As  ClassLoader
			Get
				Return If(callersClassLoaderRef IsNot Nothing, callersClassLoaderRef.get(), Nothing)
			End Get
		End Property

		' This constructor is used only to create the global Logger.
		' It is needed to break a cyclic dependence between the LogManager
		' and Logger static initializers causing deadlocks.
		Private Sub New(ByVal name As String)
			' The manager field is not initialized here.
			Me.name = name
			Me.isSystemLogger = True
			levelValue = Level.INFO
		End Sub

		' It is called from LoggerContext.addLocalLogger() when the logger
		' is actually added to a LogManager.
		Friend Overridable Property logManager As LogManager
			Set(ByVal manager As LogManager)
				Me.manager = manager
			End Set
		End Property

		Private Sub checkPermission()
			If Not anonymous Then
				If manager Is Nothing Then manager = LogManager.logManager
				manager.checkPermission()
			End If
		End Sub

		' Until all JDK code converted to call sun.util.logging.PlatformLogger
		' (see 7054233), we need to determine if Logger.getLogger is to add
		' a system logger or user logger.
		'
		' As an interim solution, if the immediate caller whose caller loader is
		' null, we assume it's a system logger and add it to the system context.
		' These system loggers only set the resource bundle to the given
		' resource bundle name (rather than the default system resource bundle).
		Private Class SystemLoggerHelper
			Friend Shared disableCallerCheck As Boolean = getBooleanProperty("sun.util.logging.disableCallerCheck")
			Private Shared Function getBooleanProperty(ByVal key As String) As Boolean
				Dim s As String = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
				Return Convert.ToBoolean(s)
			End Function

			Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
				Implements java.security.PrivilegedAction(Of T)

				Public Overrides Function run() As String
					Return System.getProperty(key)
				End Function
			End Class
		End Class

		Private Shared Function demandLogger(ByVal name As String, ByVal resourceBundleName As String, ByVal caller As [Class]) As Logger
			Dim manager As LogManager = LogManager.logManager
			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing AndAlso (Not SystemLoggerHelper.disableCallerCheck) Then
				If caller.classLoader Is Nothing Then Return manager.demandSystemLogger(name, resourceBundleName)
			End If
			Return manager.demandLogger(name, resourceBundleName, caller)
			' ends up calling new Logger(name, resourceBundleName, caller)
			' iff the logger doesn't exist already
		End Function

		''' <summary>
		''' Find or create a logger for a named subsystem.  If a logger has
		''' already been created with the given name it is returned.  Otherwise
		''' a new logger is created.
		''' <p>
		''' If a new logger is created its log level will be configured
		''' based on the LogManager configuration and it will configured
		''' to also send logging output to its parent's Handlers.  It will
		''' be registered in the LogManager global namespace.
		''' <p>
		''' Note: The LogManager may only retain a weak reference to the newly
		''' created Logger. It is important to understand that a previously
		''' created Logger with the given name may be garbage collected at any
		''' time if there is no strong reference to the Logger. In particular,
		''' this means that two back-to-back calls like
		''' {@code getLogger("MyLogger").log(...)} may use different Logger
		''' objects named "MyLogger" if there is no strong reference to the
		''' Logger named "MyLogger" elsewhere in the program.
		''' </summary>
		''' <param name="name">            A name for the logger.  This should
		'''                          be a dot-separated name and should normally
		'''                          be based on the package name or class name
		'''                          of the subsystem, such as java.net
		'''                          or javax.swing </param>
		''' <returns> a suitable Logger </returns>
		''' <exception cref="NullPointerException"> if the name is null. </exception>

		' Synchronization is not required here. All synchronization for
		' adding a new Logger object is handled by LogManager.addLogger().
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function getLogger(ByVal name As String) As Logger
			' This method is intentionally not a wrapper around a call
			' to getLogger(name, resourceBundleName). If it were then
			' this sequence:
			'
			'     getLogger("Foo", "resourceBundleForFoo");
			'     getLogger("Foo");
			'
			' would throw an IllegalArgumentException in the second call
			' because the wrapper would result in an attempt to replace
			' the existing "resourceBundleForFoo" with null.
			Return demandLogger(name, Nothing, sun.reflect.Reflection.callerClass)
		End Function

		''' <summary>
		''' Find or create a logger for a named subsystem.  If a logger has
		''' already been created with the given name it is returned.  Otherwise
		''' a new logger is created.
		''' <p>
		''' If a new logger is created its log level will be configured
		''' based on the LogManager and it will configured to also send logging
		''' output to its parent's Handlers.  It will be registered in
		''' the LogManager global namespace.
		''' <p>
		''' Note: The LogManager may only retain a weak reference to the newly
		''' created Logger. It is important to understand that a previously
		''' created Logger with the given name may be garbage collected at any
		''' time if there is no strong reference to the Logger. In particular,
		''' this means that two back-to-back calls like
		''' {@code getLogger("MyLogger", ...).log(...)} may use different Logger
		''' objects named "MyLogger" if there is no strong reference to the
		''' Logger named "MyLogger" elsewhere in the program.
		''' <p>
		''' If the named Logger already exists and does not yet have a
		''' localization resource bundle then the given resource bundle
		''' name is used.  If the named Logger already exists and has
		''' a different resource bundle name then an IllegalArgumentException
		''' is thrown.
		''' <p> </summary>
		''' <param name="name">    A name for the logger.  This should
		'''                          be a dot-separated name and should normally
		'''                          be based on the package name or class name
		'''                          of the subsystem, such as java.net
		'''                          or javax.swing </param>
		''' <param name="resourceBundleName">  name of ResourceBundle to be used for localizing
		'''                          messages for this logger. May be {@code null}
		'''                          if none of the messages require localization. </param>
		''' <returns> a suitable Logger </returns>
		''' <exception cref="MissingResourceException"> if the resourceBundleName is non-null and
		'''             no corresponding resource can be found. </exception>
		''' <exception cref="IllegalArgumentException"> if the Logger already exists and uses
		'''             a different resource bundle name; or if
		'''             {@code resourceBundleName} is {@code null} but the named
		'''             logger has a resource bundle set. </exception>
		''' <exception cref="NullPointerException"> if the name is null. </exception>

		' Synchronization is not required here. All synchronization for
		' adding a new Logger object is handled by LogManager.addLogger().
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function getLogger(ByVal name As String, ByVal resourceBundleName As String) As Logger
			Dim callerClass As  [Class] = sun.reflect.Reflection.callerClass
			Dim result As Logger = demandLogger(name, resourceBundleName, callerClass)

			' MissingResourceException or IllegalArgumentException can be
			' thrown by setupResourceInfo().
			' We have to set the callers ClassLoader here in case demandLogger
			' above found a previously created Logger.  This can happen, for
			' example, if Logger.getLogger(name) is called and subsequently
			' Logger.getLogger(name, resourceBundleName) is called.  In this case
			' we won't necessarily have the correct classloader saved away, so
			' we need to set it here, too.

			result.setupResourceInfo(resourceBundleName, callerClass)
			Return result
		End Function

		' package-private
		' Add a platform logger to the system context.
		' i.e. caller of sun.util.logging.PlatformLogger.getLogger
		Shared Function getPlatformLogger(ByVal name As String) As Logger
			Dim manager As LogManager = LogManager.logManager

			' all loggers in the system context will default to
			' the system logger's resource bundle
			Dim result As Logger = manager.demandSystemLogger(name, SYSTEM_LOGGER_RB_NAME)
			Return result
		End Function

		''' <summary>
		''' Create an anonymous Logger.  The newly created Logger is not
		''' registered in the LogManager namespace.  There will be no
		''' access checks on updates to the logger.
		''' <p>
		''' This factory method is primarily intended for use from applets.
		''' Because the resulting Logger is anonymous it can be kept private
		''' by the creating class.  This removes the need for normal security
		''' checks, which in turn allows untrusted applet code to update
		''' the control state of the Logger.  For example an applet can do
		''' a setLevel or an addHandler on an anonymous Logger.
		''' <p>
		''' Even although the new logger is anonymous, it is configured
		''' to have the root logger ("") as its parent.  This means that
		''' by default it inherits its effective level and handlers
		''' from the root logger. Changing its parent via the
		''' <seealso cref="#setParent(java.util.logging.Logger) setParent"/> method
		''' will still require the security permission specified by that method.
		''' <p>
		''' </summary>
		''' <returns> a newly created private Logger </returns>
		Public Property Shared anonymousLogger As Logger
			Get
				Return getAnonymousLogger(Nothing)
			End Get
		End Property

		''' <summary>
		''' Create an anonymous Logger.  The newly created Logger is not
		''' registered in the LogManager namespace.  There will be no
		''' access checks on updates to the logger.
		''' <p>
		''' This factory method is primarily intended for use from applets.
		''' Because the resulting Logger is anonymous it can be kept private
		''' by the creating class.  This removes the need for normal security
		''' checks, which in turn allows untrusted applet code to update
		''' the control state of the Logger.  For example an applet can do
		''' a setLevel or an addHandler on an anonymous Logger.
		''' <p>
		''' Even although the new logger is anonymous, it is configured
		''' to have the root logger ("") as its parent.  This means that
		''' by default it inherits its effective level and handlers
		''' from the root logger.  Changing its parent via the
		''' <seealso cref="#setParent(java.util.logging.Logger) setParent"/> method
		''' will still require the security permission specified by that method.
		''' <p> </summary>
		''' <param name="resourceBundleName">  name of ResourceBundle to be used for localizing
		'''                          messages for this logger.
		'''          May be null if none of the messages require localization. </param>
		''' <returns> a newly created private Logger </returns>
		''' <exception cref="MissingResourceException"> if the resourceBundleName is non-null and
		'''             no corresponding resource can be found. </exception>

		' Synchronization is not required here. All synchronization for
		' adding a new anonymous Logger object is handled by doSetParent().
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function getAnonymousLogger(ByVal resourceBundleName As String) As Logger
			Dim manager As LogManager = LogManager.logManager
			' cleanup some Loggers that have been GC'ed
			manager.drainLoggerRefQueueBounded()
			Dim result As New Logger(Nothing, resourceBundleName, sun.reflect.Reflection.callerClass, manager, False)
			result.anonymous = True
			Dim root As Logger = manager.getLogger("")
			result.doSetParent(root)
			Return result
		End Function

		''' <summary>
		''' Retrieve the localization resource bundle for this
		''' logger.
		''' This method will return a {@code ResourceBundle} that was either
		''' set by the {@link
		''' #setResourceBundle(java.util.ResourceBundle) setResourceBundle} method or
		''' <a href="#ResourceBundleMapping">mapped from the
		''' the resource bundle name</a> set via the {@link
		''' Logger#getLogger(java.lang.String, java.lang.String) getLogger} factory
		''' method for the current default locale.
		''' <br>Note that if the result is {@code null}, then the Logger will use a resource
		''' bundle or resource bundle name inherited from its parent.
		''' </summary>
		''' <returns> localization bundle (may be {@code null}) </returns>
		Public Overridable Property resourceBundle As java.util.ResourceBundle
			Get
				Return findResourceBundle(resourceBundleName, True)
			End Get
		End Property

		''' <summary>
		''' Retrieve the localization resource bundle name for this
		''' logger.
		''' This is either the name specified through the {@link
		''' #getLogger(java.lang.String, java.lang.String) getLogger} factory method,
		''' or the <seealso cref="ResourceBundle#getBaseBundleName() base name"/> of the
		''' ResourceBundle set through {@link
		''' #setResourceBundle(java.util.ResourceBundle) setResourceBundle} method.
		''' <br>Note that if the result is {@code null}, then the Logger will use a resource
		''' bundle or resource bundle name inherited from its parent.
		''' </summary>
		''' <returns> localization bundle name (may be {@code null}) </returns>
		Public Overridable Property resourceBundleName As String
			Get
				Return loggerBundle_Renamed.resourceBundleName
			End Get
		End Property

		''' <summary>
		''' Set a filter to control output on this Logger.
		''' <P>
		''' After passing the initial "level" check, the Logger will
		''' call this Filter to check if a log record should really
		''' be published.
		''' </summary>
		''' <param name="newFilter">  a filter object (may be null) </param>
		''' <exception cref="SecurityException"> if a security manager exists,
		'''          this logger is not anonymous, and the caller
		'''          does not have LoggingPermission("control"). </exception>
		Public Overridable Property filter As Filter
			Set(ByVal newFilter As Filter)
				checkPermission()
				filter = newFilter
			End Set
			Get
				Return filter
			End Get
		End Property


		''' <summary>
		''' Log a LogRecord.
		''' <p>
		''' All the other logging methods in this class call through
		''' this method to actually perform any logging.  Subclasses can
		''' override this single method to capture all log activity.
		''' </summary>
		''' <param name="record"> the LogRecord to be published </param>
		Public Overridable Sub log(ByVal record As LogRecord)
			If Not isLoggable(record.level) Then Return
			Dim theFilter As Filter = filter
			If theFilter IsNot Nothing AndAlso (Not theFilter.isLoggable(record)) Then Return

			' Post the LogRecord to all our Handlers, and then to
			' our parents' handlers, all the way up the tree.

			Dim logger_Renamed As Logger = Me
			Do While logger_Renamed IsNot Nothing
				Dim loggerHandlers As Handler() = If(isSystemLogger, logger_Renamed.accessCheckedHandlers(), logger_Renamed.handlers)

				For Each handler As Handler In loggerHandlers
					handler.publish(record)
				Next handler

				Dim useParentHdls As Boolean = If(isSystemLogger, logger_Renamed.useParentHandlers, logger_Renamed.useParentHandlers)

				If Not useParentHdls Then Exit Do

				logger_Renamed = If(isSystemLogger, logger_Renamed.parent, logger_Renamed.parent)
			Loop
		End Sub

		' private support method for logging.
		' We fill in the logger name, resource bundle name, and
		' resource bundle and then call "void log(LogRecord)".
		Private Sub doLog(ByVal lr As LogRecord)
			lr.loggerName = name
			Dim lb As LoggerBundle = effectiveLoggerBundle
			Dim bundle As java.util.ResourceBundle = lb.userBundle
			Dim ebname As String = lb.resourceBundleName
			If ebname IsNot Nothing AndAlso bundle IsNot Nothing Then
				lr.resourceBundleName = ebname
				lr.resourceBundle = bundle
			End If
			log(lr)
		End Sub


		'================================================================
		' Start of convenience methods WITHOUT className and methodName
		'================================================================

		''' <summary>
		''' Log a message, with no arguments.
		''' <p>
		''' If the logger is currently enabled for the given message
		''' level then the given message is forwarded to all the
		''' registered output Handler objects.
		''' <p> </summary>
		''' <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		''' <param name="msg">     The string message (or a key in the message catalog) </param>
		Public Overridable Sub log(ByVal level_Renamed As Level, ByVal msg As String)
			If Not isLoggable(level_Renamed) Then Return
			Dim lr As New LogRecord(level_Renamed, msg)
			doLog(lr)
		End Sub

		''' <summary>
		''' Log a message, which is only to be constructed if the logging level
		''' is such that the message will actually be logged.
		''' <p>
		''' If the logger is currently enabled for the given message
		''' level then the message is constructed by invoking the provided
		''' supplier function and forwarded to all the registered output
		''' Handler objects.
		''' <p> </summary>
		''' <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		''' <param name="msgSupplier">   A function, which when called, produces the
		'''                        desired log message </param>
		Public Overridable Sub log(ByVal level_Renamed As Level, ByVal msgSupplier As java.util.function.Supplier(Of String))
			If Not isLoggable(level_Renamed) Then Return
			Dim lr As New LogRecord(level_Renamed, msgSupplier.get())
			doLog(lr)
		End Sub

		''' <summary>
		''' Log a message, with one object parameter.
		''' <p>
		''' If the logger is currently enabled for the given message
		''' level then a corresponding LogRecord is created and forwarded
		''' to all the registered output Handler objects.
		''' <p> </summary>
		''' <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		''' <param name="msg">     The string message (or a key in the message catalog) </param>
		''' <param name="param1">  parameter to the message </param>
		Public Overridable Sub log(ByVal level_Renamed As Level, ByVal msg As String, ByVal param1 As Object)
			If Not isLoggable(level_Renamed) Then Return
			Dim lr As New LogRecord(level_Renamed, msg)
			Dim params As Object() = { param1 }
			lr.parameters = params
			doLog(lr)
		End Sub

		''' <summary>
		''' Log a message, with an array of object arguments.
		''' <p>
		''' If the logger is currently enabled for the given message
		''' level then a corresponding LogRecord is created and forwarded
		''' to all the registered output Handler objects.
		''' <p> </summary>
		''' <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		''' <param name="msg">     The string message (or a key in the message catalog) </param>
		''' <param name="params">  array of parameters to the message </param>
		Public Overridable Sub log(ByVal level_Renamed As Level, ByVal msg As String, ByVal params As Object())
			If Not isLoggable(level_Renamed) Then Return
			Dim lr As New LogRecord(level_Renamed, msg)
			lr.parameters = params
			doLog(lr)
		End Sub

		''' <summary>
		''' Log a message, with associated Throwable information.
		''' <p>
		''' If the logger is currently enabled for the given message
		''' level then the given arguments are stored in a LogRecord
		''' which is forwarded to all registered output handlers.
		''' <p>
		''' Note that the thrown argument is stored in the LogRecord thrown
		''' property, rather than the LogRecord parameters property.  Thus it is
		''' processed specially by output Formatters and is not treated
		''' as a formatting parameter to the LogRecord message property.
		''' <p> </summary>
		''' <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		''' <param name="msg">     The string message (or a key in the message catalog) </param>
		''' <param name="thrown">  Throwable associated with log message. </param>
		Public Overridable Sub log(ByVal level_Renamed As Level, ByVal msg As String, ByVal thrown As Throwable)
			If Not isLoggable(level_Renamed) Then Return
			Dim lr As New LogRecord(level_Renamed, msg)
			lr.thrown = thrown
			doLog(lr)
		End Sub

		''' <summary>
		''' Log a lazily constructed message, with associated Throwable information.
		''' <p>
		''' If the logger is currently enabled for the given message level then the
		''' message is constructed by invoking the provided supplier function. The
		''' message and the given <seealso cref="Throwable"/> are then stored in a {@link
		''' LogRecord} which is forwarded to all registered output handlers.
		''' <p>
		''' Note that the thrown argument is stored in the LogRecord thrown
		''' property, rather than the LogRecord parameters property.  Thus it is
		''' processed specially by output Formatters and is not treated
		''' as a formatting parameter to the LogRecord message property.
		''' <p> </summary>
		''' <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		''' <param name="thrown">  Throwable associated with log message. </param>
		''' <param name="msgSupplier">   A function, which when called, produces the
		'''                        desired log message
		''' @since   1.8 </param>
		Public Overridable Sub log(ByVal level_Renamed As Level, ByVal thrown As Throwable, ByVal msgSupplier As java.util.function.Supplier(Of String))
			If Not isLoggable(level_Renamed) Then Return
			Dim lr As New LogRecord(level_Renamed, msgSupplier.get())
			lr.thrown = thrown
			doLog(lr)
		End Sub

		'================================================================
		' Start of convenience methods WITH className and methodName
		'================================================================

		''' <summary>
		''' Log a message, specifying source class and method,
		''' with no arguments.
		''' <p>
		''' If the logger is currently enabled for the given message
		''' level then the given message is forwarded to all the
		''' registered output Handler objects.
		''' <p> </summary>
		''' <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		''' <param name="sourceClass">    name of class that issued the logging request </param>
		''' <param name="sourceMethod">   name of method that issued the logging request </param>
		''' <param name="msg">     The string message (or a key in the message catalog) </param>
		Public Overridable Sub logp(ByVal level_Renamed As Level, ByVal sourceClass As String, ByVal sourceMethod As String, ByVal msg As String)
			If Not isLoggable(level_Renamed) Then Return
			Dim lr As New LogRecord(level_Renamed, msg)
			lr.sourceClassName = sourceClass
			lr.sourceMethodName = sourceMethod
			doLog(lr)
		End Sub

		''' <summary>
		''' Log a lazily constructed message, specifying source class and method,
		''' with no arguments.
		''' <p>
		''' If the logger is currently enabled for the given message
		''' level then the message is constructed by invoking the provided
		''' supplier function and forwarded to all the registered output
		''' Handler objects.
		''' <p> </summary>
		''' <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		''' <param name="sourceClass">    name of class that issued the logging request </param>
		''' <param name="sourceMethod">   name of method that issued the logging request </param>
		''' <param name="msgSupplier">   A function, which when called, produces the
		'''                        desired log message
		''' @since   1.8 </param>
		Public Overridable Sub logp(ByVal level_Renamed As Level, ByVal sourceClass As String, ByVal sourceMethod As String, ByVal msgSupplier As java.util.function.Supplier(Of String))
			If Not isLoggable(level_Renamed) Then Return
			Dim lr As New LogRecord(level_Renamed, msgSupplier.get())
			lr.sourceClassName = sourceClass
			lr.sourceMethodName = sourceMethod
			doLog(lr)
		End Sub

		''' <summary>
		''' Log a message, specifying source class and method,
		''' with a single object parameter to the log message.
		''' <p>
		''' If the logger is currently enabled for the given message
		''' level then a corresponding LogRecord is created and forwarded
		''' to all the registered output Handler objects.
		''' <p> </summary>
		''' <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		''' <param name="sourceClass">    name of class that issued the logging request </param>
		''' <param name="sourceMethod">   name of method that issued the logging request </param>
		''' <param name="msg">      The string message (or a key in the message catalog) </param>
		''' <param name="param1">    Parameter to the log message. </param>
		Public Overridable Sub logp(ByVal level_Renamed As Level, ByVal sourceClass As String, ByVal sourceMethod As String, ByVal msg As String, ByVal param1 As Object)
			If Not isLoggable(level_Renamed) Then Return
			Dim lr As New LogRecord(level_Renamed, msg)
			lr.sourceClassName = sourceClass
			lr.sourceMethodName = sourceMethod
			Dim params As Object() = { param1 }
			lr.parameters = params
			doLog(lr)
		End Sub

		''' <summary>
		''' Log a message, specifying source class and method,
		''' with an array of object arguments.
		''' <p>
		''' If the logger is currently enabled for the given message
		''' level then a corresponding LogRecord is created and forwarded
		''' to all the registered output Handler objects.
		''' <p> </summary>
		''' <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		''' <param name="sourceClass">    name of class that issued the logging request </param>
		''' <param name="sourceMethod">   name of method that issued the logging request </param>
		''' <param name="msg">     The string message (or a key in the message catalog) </param>
		''' <param name="params">  Array of parameters to the message </param>
		Public Overridable Sub logp(ByVal level_Renamed As Level, ByVal sourceClass As String, ByVal sourceMethod As String, ByVal msg As String, ByVal params As Object())
			If Not isLoggable(level_Renamed) Then Return
			Dim lr As New LogRecord(level_Renamed, msg)
			lr.sourceClassName = sourceClass
			lr.sourceMethodName = sourceMethod
			lr.parameters = params
			doLog(lr)
		End Sub

		''' <summary>
		''' Log a message, specifying source class and method,
		''' with associated Throwable information.
		''' <p>
		''' If the logger is currently enabled for the given message
		''' level then the given arguments are stored in a LogRecord
		''' which is forwarded to all registered output handlers.
		''' <p>
		''' Note that the thrown argument is stored in the LogRecord thrown
		''' property, rather than the LogRecord parameters property.  Thus it is
		''' processed specially by output Formatters and is not treated
		''' as a formatting parameter to the LogRecord message property.
		''' <p> </summary>
		''' <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		''' <param name="sourceClass">    name of class that issued the logging request </param>
		''' <param name="sourceMethod">   name of method that issued the logging request </param>
		''' <param name="msg">     The string message (or a key in the message catalog) </param>
		''' <param name="thrown">  Throwable associated with log message. </param>
		Public Overridable Sub logp(ByVal level_Renamed As Level, ByVal sourceClass As String, ByVal sourceMethod As String, ByVal msg As String, ByVal thrown As Throwable)
			If Not isLoggable(level_Renamed) Then Return
			Dim lr As New LogRecord(level_Renamed, msg)
			lr.sourceClassName = sourceClass
			lr.sourceMethodName = sourceMethod
			lr.thrown = thrown
			doLog(lr)
		End Sub

		''' <summary>
		''' Log a lazily constructed message, specifying source class and method,
		''' with associated Throwable information.
		''' <p>
		''' If the logger is currently enabled for the given message level then the
		''' message is constructed by invoking the provided supplier function. The
		''' message and the given <seealso cref="Throwable"/> are then stored in a {@link
		''' LogRecord} which is forwarded to all registered output handlers.
		''' <p>
		''' Note that the thrown argument is stored in the LogRecord thrown
		''' property, rather than the LogRecord parameters property.  Thus it is
		''' processed specially by output Formatters and is not treated
		''' as a formatting parameter to the LogRecord message property.
		''' <p> </summary>
		''' <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		''' <param name="sourceClass">    name of class that issued the logging request </param>
		''' <param name="sourceMethod">   name of method that issued the logging request </param>
		''' <param name="thrown">  Throwable associated with log message. </param>
		''' <param name="msgSupplier">   A function, which when called, produces the
		'''                        desired log message
		''' @since   1.8 </param>
		Public Overridable Sub logp(ByVal level_Renamed As Level, ByVal sourceClass As String, ByVal sourceMethod As String, ByVal thrown As Throwable, ByVal msgSupplier As java.util.function.Supplier(Of String))
			If Not isLoggable(level_Renamed) Then Return
			Dim lr As New LogRecord(level_Renamed, msgSupplier.get())
			lr.sourceClassName = sourceClass
			lr.sourceMethodName = sourceMethod
			lr.thrown = thrown
			doLog(lr)
		End Sub


		'=========================================================================
		' Start of convenience methods WITH className, methodName and bundle name.
		'=========================================================================

		' Private support method for logging for "logrb" methods.
		' We fill in the logger name, resource bundle name, and
		' resource bundle and then call "void log(LogRecord)".
		Private Sub doLog(ByVal lr As LogRecord, ByVal rbname As String)
			lr.loggerName = name
			If rbname IsNot Nothing Then
				lr.resourceBundleName = rbname
				lr.resourceBundle = findResourceBundle(rbname, False)
			End If
			log(lr)
		End Sub

		' Private support method for logging for "logrb" methods.
		Private Sub doLog(ByVal lr As LogRecord, ByVal rb As java.util.ResourceBundle)
			lr.loggerName = name
			If rb IsNot Nothing Then
				lr.resourceBundleName = rb.baseBundleName
				lr.resourceBundle = rb
			End If
			log(lr)
		End Sub

		''' <summary>
		''' Log a message, specifying source [Class], method, and resource bundle name
		''' with no arguments.
		''' <p>
		''' If the logger is currently enabled for the given message
		''' level then the given message is forwarded to all the
		''' registered output Handler objects.
		''' <p>
		''' The msg string is localized using the named resource bundle.  If the
		''' resource bundle name is null, or an empty String or invalid
		''' then the msg string is not localized.
		''' <p> </summary>
		''' <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		''' <param name="sourceClass">    name of class that issued the logging request </param>
		''' <param name="sourceMethod">   name of method that issued the logging request </param>
		''' <param name="bundleName">     name of resource bundle to localize msg,
		'''                         can be null </param>
		''' <param name="msg">     The string message (or a key in the message catalog) </param>
		''' @deprecated Use {@link #logrb(java.util.logging.Level, java.lang.String,
		''' java.lang.String, java.util.ResourceBundle, java.lang.String,
		''' java.lang.Object...)} instead. 
		<Obsolete("Use {@link #logrb(java.util.logging.Level, java.lang.String,")> _
		Public Overridable Sub logrb(ByVal level_Renamed As Level, ByVal sourceClass As String, ByVal sourceMethod As String, ByVal bundleName As String, ByVal msg As String)
			If Not isLoggable(level_Renamed) Then Return
			Dim lr As New LogRecord(level_Renamed, msg)
			lr.sourceClassName = sourceClass
			lr.sourceMethodName = sourceMethod
			doLog(lr, bundleName)
		End Sub

		''' <summary>
		''' Log a message, specifying source [Class], method, and resource bundle name,
		''' with a single object parameter to the log message.
		''' <p>
		''' If the logger is currently enabled for the given message
		''' level then a corresponding LogRecord is created and forwarded
		''' to all the registered output Handler objects.
		''' <p>
		''' The msg string is localized using the named resource bundle.  If the
		''' resource bundle name is null, or an empty String or invalid
		''' then the msg string is not localized.
		''' <p> </summary>
		''' <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		''' <param name="sourceClass">    name of class that issued the logging request </param>
		''' <param name="sourceMethod">   name of method that issued the logging request </param>
		''' <param name="bundleName">     name of resource bundle to localize msg,
		'''                         can be null </param>
		''' <param name="msg">      The string message (or a key in the message catalog) </param>
		''' <param name="param1">    Parameter to the log message. </param>
		''' @deprecated Use {@link #logrb(java.util.logging.Level, java.lang.String,
		'''   java.lang.String, java.util.ResourceBundle, java.lang.String,
		'''   java.lang.Object...)} instead 
		<Obsolete("Use {@link #logrb(java.util.logging.Level, java.lang.String,")> _
		Public Overridable Sub logrb(ByVal level_Renamed As Level, ByVal sourceClass As String, ByVal sourceMethod As String, ByVal bundleName As String, ByVal msg As String, ByVal param1 As Object)
			If Not isLoggable(level_Renamed) Then Return
			Dim lr As New LogRecord(level_Renamed, msg)
			lr.sourceClassName = sourceClass
			lr.sourceMethodName = sourceMethod
			Dim params As Object() = { param1 }
			lr.parameters = params
			doLog(lr, bundleName)
		End Sub

		''' <summary>
		''' Log a message, specifying source [Class], method, and resource bundle name,
		''' with an array of object arguments.
		''' <p>
		''' If the logger is currently enabled for the given message
		''' level then a corresponding LogRecord is created and forwarded
		''' to all the registered output Handler objects.
		''' <p>
		''' The msg string is localized using the named resource bundle.  If the
		''' resource bundle name is null, or an empty String or invalid
		''' then the msg string is not localized.
		''' <p> </summary>
		''' <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		''' <param name="sourceClass">    name of class that issued the logging request </param>
		''' <param name="sourceMethod">   name of method that issued the logging request </param>
		''' <param name="bundleName">     name of resource bundle to localize msg,
		'''                         can be null. </param>
		''' <param name="msg">     The string message (or a key in the message catalog) </param>
		''' <param name="params">  Array of parameters to the message </param>
		''' @deprecated Use {@link #logrb(java.util.logging.Level, java.lang.String,
		'''      java.lang.String, java.util.ResourceBundle, java.lang.String,
		'''      java.lang.Object...)} instead. 
		<Obsolete("Use {@link #logrb(java.util.logging.Level, java.lang.String,")> _
		Public Overridable Sub logrb(ByVal level_Renamed As Level, ByVal sourceClass As String, ByVal sourceMethod As String, ByVal bundleName As String, ByVal msg As String, ByVal params As Object())
			If Not isLoggable(level_Renamed) Then Return
			Dim lr As New LogRecord(level_Renamed, msg)
			lr.sourceClassName = sourceClass
			lr.sourceMethodName = sourceMethod
			lr.parameters = params
			doLog(lr, bundleName)
		End Sub

		''' <summary>
		''' Log a message, specifying source [Class], method, and resource bundle,
		''' with an optional list of message parameters.
		''' <p>
		''' If the logger is currently enabled for the given message
		''' level then a corresponding LogRecord is created and forwarded
		''' to all the registered output Handler objects.
		''' <p>
		''' The {@code msg} string is localized using the given resource bundle.
		''' If the resource bundle is {@code null}, then the {@code msg} string is not
		''' localized.
		''' <p> </summary>
		''' <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		''' <param name="sourceClass">    Name of the class that issued the logging request </param>
		''' <param name="sourceMethod">   Name of the method that issued the logging request </param>
		''' <param name="bundle">         Resource bundle to localize {@code msg},
		'''                         can be {@code null}. </param>
		''' <param name="msg">     The string message (or a key in the message catalog) </param>
		''' <param name="params">  Parameters to the message (optional, may be none).
		''' @since 1.8 </param>
		Public Overridable Sub logrb(ByVal level_Renamed As Level, ByVal sourceClass As String, ByVal sourceMethod As String, ByVal bundle As java.util.ResourceBundle, ByVal msg As String, ParamArray ByVal params As Object())
			If Not isLoggable(level_Renamed) Then Return
			Dim lr As New LogRecord(level_Renamed, msg)
			lr.sourceClassName = sourceClass
			lr.sourceMethodName = sourceMethod
			If params IsNot Nothing AndAlso params.Length <> 0 Then lr.parameters = params
			doLog(lr, bundle)
		End Sub

		''' <summary>
		''' Log a message, specifying source [Class], method, and resource bundle name,
		''' with associated Throwable information.
		''' <p>
		''' If the logger is currently enabled for the given message
		''' level then the given arguments are stored in a LogRecord
		''' which is forwarded to all registered output handlers.
		''' <p>
		''' The msg string is localized using the named resource bundle.  If the
		''' resource bundle name is null, or an empty String or invalid
		''' then the msg string is not localized.
		''' <p>
		''' Note that the thrown argument is stored in the LogRecord thrown
		''' property, rather than the LogRecord parameters property.  Thus it is
		''' processed specially by output Formatters and is not treated
		''' as a formatting parameter to the LogRecord message property.
		''' <p> </summary>
		''' <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		''' <param name="sourceClass">    name of class that issued the logging request </param>
		''' <param name="sourceMethod">   name of method that issued the logging request </param>
		''' <param name="bundleName">     name of resource bundle to localize msg,
		'''                         can be null </param>
		''' <param name="msg">     The string message (or a key in the message catalog) </param>
		''' <param name="thrown">  Throwable associated with log message. </param>
		''' @deprecated Use {@link #logrb(java.util.logging.Level, java.lang.String,
		'''     java.lang.String, java.util.ResourceBundle, java.lang.String,
		'''     java.lang.Throwable)} instead. 
		<Obsolete("Use {@link #logrb(java.util.logging.Level, java.lang.String,")> _
		Public Overridable Sub logrb(ByVal level_Renamed As Level, ByVal sourceClass As String, ByVal sourceMethod As String, ByVal bundleName As String, ByVal msg As String, ByVal thrown As Throwable)
			If Not isLoggable(level_Renamed) Then Return
			Dim lr As New LogRecord(level_Renamed, msg)
			lr.sourceClassName = sourceClass
			lr.sourceMethodName = sourceMethod
			lr.thrown = thrown
			doLog(lr, bundleName)
		End Sub

		''' <summary>
		''' Log a message, specifying source [Class], method, and resource bundle,
		''' with associated Throwable information.
		''' <p>
		''' If the logger is currently enabled for the given message
		''' level then the given arguments are stored in a LogRecord
		''' which is forwarded to all registered output handlers.
		''' <p>
		''' The {@code msg} string is localized using the given resource bundle.
		''' If the resource bundle is {@code null}, then the {@code msg} string is not
		''' localized.
		''' <p>
		''' Note that the thrown argument is stored in the LogRecord thrown
		''' property, rather than the LogRecord parameters property.  Thus it is
		''' processed specially by output Formatters and is not treated
		''' as a formatting parameter to the LogRecord message property.
		''' <p> </summary>
		''' <param name="level">   One of the message level identifiers, e.g., SEVERE </param>
		''' <param name="sourceClass">    Name of the class that issued the logging request </param>
		''' <param name="sourceMethod">   Name of the method that issued the logging request </param>
		''' <param name="bundle">         Resource bundle to localize {@code msg},
		'''                         can be {@code null} </param>
		''' <param name="msg">     The string message (or a key in the message catalog) </param>
		''' <param name="thrown">  Throwable associated with the log message.
		''' @since 1.8 </param>
		Public Overridable Sub logrb(ByVal level_Renamed As Level, ByVal sourceClass As String, ByVal sourceMethod As String, ByVal bundle As java.util.ResourceBundle, ByVal msg As String, ByVal thrown As Throwable)
			If Not isLoggable(level_Renamed) Then Return
			Dim lr As New LogRecord(level_Renamed, msg)
			lr.sourceClassName = sourceClass
			lr.sourceMethodName = sourceMethod
			lr.thrown = thrown
			doLog(lr, bundle)
		End Sub

		'======================================================================
		' Start of convenience methods for logging method entries and returns.
		'======================================================================

		''' <summary>
		''' Log a method entry.
		''' <p>
		''' This is a convenience method that can be used to log entry
		''' to a method.  A LogRecord with message "ENTRY", log level
		''' FINER, and the given sourceMethod and sourceClass is logged.
		''' <p> </summary>
		''' <param name="sourceClass">    name of class that issued the logging request </param>
		''' <param name="sourceMethod">   name of method that is being entered </param>
		Public Overridable Sub entering(ByVal sourceClass As String, ByVal sourceMethod As String)
			logp(Level.FINER, sourceClass, sourceMethod, "ENTRY")
		End Sub

		''' <summary>
		''' Log a method entry, with one parameter.
		''' <p>
		''' This is a convenience method that can be used to log entry
		''' to a method.  A LogRecord with message "ENTRY {0}", log level
		''' FINER, and the given sourceMethod, sourceClass, and parameter
		''' is logged.
		''' <p> </summary>
		''' <param name="sourceClass">    name of class that issued the logging request </param>
		''' <param name="sourceMethod">   name of method that is being entered </param>
		''' <param name="param1">         parameter to the method being entered </param>
		Public Overridable Sub entering(ByVal sourceClass As String, ByVal sourceMethod As String, ByVal param1 As Object)
			logp(Level.FINER, sourceClass, sourceMethod, "ENTRY {0}", param1)
		End Sub

		''' <summary>
		''' Log a method entry, with an array of parameters.
		''' <p>
		''' This is a convenience method that can be used to log entry
		''' to a method.  A LogRecord with message "ENTRY" (followed by a
		''' format {N} indicator for each entry in the parameter array),
		''' log level FINER, and the given sourceMethod, sourceClass, and
		''' parameters is logged.
		''' <p> </summary>
		''' <param name="sourceClass">    name of class that issued the logging request </param>
		''' <param name="sourceMethod">   name of method that is being entered </param>
		''' <param name="params">         array of parameters to the method being entered </param>
		Public Overridable Sub entering(ByVal sourceClass As String, ByVal sourceMethod As String, ByVal params As Object())
			Dim msg As String = "ENTRY"
			If params Is Nothing Then
			   logp(Level.FINER, sourceClass, sourceMethod, msg)
			   Return
			End If
			If Not isLoggable(Level.FINER) Then Return
			For i As Integer = 0 To params.Length - 1
				msg = msg & " {" & i & "}"
			Next i
			logp(Level.FINER, sourceClass, sourceMethod, msg, params)
		End Sub

		''' <summary>
		''' Log a method return.
		''' <p>
		''' This is a convenience method that can be used to log returning
		''' from a method.  A LogRecord with message "RETURN", log level
		''' FINER, and the given sourceMethod and sourceClass is logged.
		''' <p> </summary>
		''' <param name="sourceClass">    name of class that issued the logging request </param>
		''' <param name="sourceMethod">   name of the method </param>
		Public Overridable Sub exiting(ByVal sourceClass As String, ByVal sourceMethod As String)
			logp(Level.FINER, sourceClass, sourceMethod, "RETURN")
		End Sub


		''' <summary>
		''' Log a method return, with result object.
		''' <p>
		''' This is a convenience method that can be used to log returning
		''' from a method.  A LogRecord with message "RETURN {0}", log level
		''' FINER, and the gives sourceMethod, sourceClass, and result
		''' object is logged.
		''' <p> </summary>
		''' <param name="sourceClass">    name of class that issued the logging request </param>
		''' <param name="sourceMethod">   name of the method </param>
		''' <param name="result">  Object that is being returned </param>
		Public Overridable Sub exiting(ByVal sourceClass As String, ByVal sourceMethod As String, ByVal result As Object)
			logp(Level.FINER, sourceClass, sourceMethod, "RETURN {0}", result)
		End Sub

		''' <summary>
		''' Log throwing an exception.
		''' <p>
		''' This is a convenience method to log that a method is
		''' terminating by throwing an exception.  The logging is done
		''' using the FINER level.
		''' <p>
		''' If the logger is currently enabled for the given message
		''' level then the given arguments are stored in a LogRecord
		''' which is forwarded to all registered output handlers.  The
		''' LogRecord's message is set to "THROW".
		''' <p>
		''' Note that the thrown argument is stored in the LogRecord thrown
		''' property, rather than the LogRecord parameters property.  Thus it is
		''' processed specially by output Formatters and is not treated
		''' as a formatting parameter to the LogRecord message property.
		''' <p> </summary>
		''' <param name="sourceClass">    name of class that issued the logging request </param>
		''' <param name="sourceMethod">  name of the method. </param>
		''' <param name="thrown">  The Throwable that is being thrown. </param>
		Public Overridable Sub throwing(ByVal sourceClass As String, ByVal sourceMethod As String, ByVal thrown As Throwable)
			If Not isLoggable(Level.FINER) Then Return
			Dim lr As New LogRecord(Level.FINER, "THROW")
			lr.sourceClassName = sourceClass
			lr.sourceMethodName = sourceMethod
			lr.thrown = thrown
			doLog(lr)
		End Sub

		'=======================================================================
		' Start of simple convenience methods using level names as method names
		'=======================================================================

		''' <summary>
		''' Log a SEVERE message.
		''' <p>
		''' If the logger is currently enabled for the SEVERE message
		''' level then the given message is forwarded to all the
		''' registered output Handler objects.
		''' <p> </summary>
		''' <param name="msg">     The string message (or a key in the message catalog) </param>
		Public Overridable Sub severe(ByVal msg As String)
			log(Level.SEVERE, msg)
		End Sub

		''' <summary>
		''' Log a WARNING message.
		''' <p>
		''' If the logger is currently enabled for the WARNING message
		''' level then the given message is forwarded to all the
		''' registered output Handler objects.
		''' <p> </summary>
		''' <param name="msg">     The string message (or a key in the message catalog) </param>
		Public Overridable Sub warning(ByVal msg As String)
			log(Level.WARNING, msg)
		End Sub

		''' <summary>
		''' Log an INFO message.
		''' <p>
		''' If the logger is currently enabled for the INFO message
		''' level then the given message is forwarded to all the
		''' registered output Handler objects.
		''' <p> </summary>
		''' <param name="msg">     The string message (or a key in the message catalog) </param>
		Public Overridable Sub info(ByVal msg As String)
			log(Level.INFO, msg)
		End Sub

		''' <summary>
		''' Log a CONFIG message.
		''' <p>
		''' If the logger is currently enabled for the CONFIG message
		''' level then the given message is forwarded to all the
		''' registered output Handler objects.
		''' <p> </summary>
		''' <param name="msg">     The string message (or a key in the message catalog) </param>
		Public Overridable Sub config(ByVal msg As String)
			log(Level.CONFIG, msg)
		End Sub

		''' <summary>
		''' Log a FINE message.
		''' <p>
		''' If the logger is currently enabled for the FINE message
		''' level then the given message is forwarded to all the
		''' registered output Handler objects.
		''' <p> </summary>
		''' <param name="msg">     The string message (or a key in the message catalog) </param>
		Public Overridable Sub fine(ByVal msg As String)
			log(Level.FINE, msg)
		End Sub

		''' <summary>
		''' Log a FINER message.
		''' <p>
		''' If the logger is currently enabled for the FINER message
		''' level then the given message is forwarded to all the
		''' registered output Handler objects.
		''' <p> </summary>
		''' <param name="msg">     The string message (or a key in the message catalog) </param>
		Public Overridable Sub finer(ByVal msg As String)
			log(Level.FINER, msg)
		End Sub

		''' <summary>
		''' Log a FINEST message.
		''' <p>
		''' If the logger is currently enabled for the FINEST message
		''' level then the given message is forwarded to all the
		''' registered output Handler objects.
		''' <p> </summary>
		''' <param name="msg">     The string message (or a key in the message catalog) </param>
		Public Overridable Sub finest(ByVal msg As String)
			log(Level.FINEST, msg)
		End Sub

		'=======================================================================
		' Start of simple convenience methods using level names as method names
		' and use Supplier<String>
		'=======================================================================

		''' <summary>
		''' Log a SEVERE message, which is only to be constructed if the logging
		''' level is such that the message will actually be logged.
		''' <p>
		''' If the logger is currently enabled for the SEVERE message
		''' level then the message is constructed by invoking the provided
		''' supplier function and forwarded to all the registered output
		''' Handler objects.
		''' <p> </summary>
		''' <param name="msgSupplier">   A function, which when called, produces the
		'''                        desired log message
		''' @since   1.8 </param>
		Public Overridable Sub severe(ByVal msgSupplier As java.util.function.Supplier(Of String))
			log(Level.SEVERE, msgSupplier)
		End Sub

		''' <summary>
		''' Log a WARNING message, which is only to be constructed if the logging
		''' level is such that the message will actually be logged.
		''' <p>
		''' If the logger is currently enabled for the WARNING message
		''' level then the message is constructed by invoking the provided
		''' supplier function and forwarded to all the registered output
		''' Handler objects.
		''' <p> </summary>
		''' <param name="msgSupplier">   A function, which when called, produces the
		'''                        desired log message
		''' @since   1.8 </param>
		Public Overridable Sub warning(ByVal msgSupplier As java.util.function.Supplier(Of String))
			log(Level.WARNING, msgSupplier)
		End Sub

		''' <summary>
		''' Log a INFO message, which is only to be constructed if the logging
		''' level is such that the message will actually be logged.
		''' <p>
		''' If the logger is currently enabled for the INFO message
		''' level then the message is constructed by invoking the provided
		''' supplier function and forwarded to all the registered output
		''' Handler objects.
		''' <p> </summary>
		''' <param name="msgSupplier">   A function, which when called, produces the
		'''                        desired log message
		''' @since   1.8 </param>
		Public Overridable Sub info(ByVal msgSupplier As java.util.function.Supplier(Of String))
			log(Level.INFO, msgSupplier)
		End Sub

		''' <summary>
		''' Log a CONFIG message, which is only to be constructed if the logging
		''' level is such that the message will actually be logged.
		''' <p>
		''' If the logger is currently enabled for the CONFIG message
		''' level then the message is constructed by invoking the provided
		''' supplier function and forwarded to all the registered output
		''' Handler objects.
		''' <p> </summary>
		''' <param name="msgSupplier">   A function, which when called, produces the
		'''                        desired log message
		''' @since   1.8 </param>
		Public Overridable Sub config(ByVal msgSupplier As java.util.function.Supplier(Of String))
			log(Level.CONFIG, msgSupplier)
		End Sub

		''' <summary>
		''' Log a FINE message, which is only to be constructed if the logging
		''' level is such that the message will actually be logged.
		''' <p>
		''' If the logger is currently enabled for the FINE message
		''' level then the message is constructed by invoking the provided
		''' supplier function and forwarded to all the registered output
		''' Handler objects.
		''' <p> </summary>
		''' <param name="msgSupplier">   A function, which when called, produces the
		'''                        desired log message
		''' @since   1.8 </param>
		Public Overridable Sub fine(ByVal msgSupplier As java.util.function.Supplier(Of String))
			log(Level.FINE, msgSupplier)
		End Sub

		''' <summary>
		''' Log a FINER message, which is only to be constructed if the logging
		''' level is such that the message will actually be logged.
		''' <p>
		''' If the logger is currently enabled for the FINER message
		''' level then the message is constructed by invoking the provided
		''' supplier function and forwarded to all the registered output
		''' Handler objects.
		''' <p> </summary>
		''' <param name="msgSupplier">   A function, which when called, produces the
		'''                        desired log message
		''' @since   1.8 </param>
		Public Overridable Sub finer(ByVal msgSupplier As java.util.function.Supplier(Of String))
			log(Level.FINER, msgSupplier)
		End Sub

		''' <summary>
		''' Log a FINEST message, which is only to be constructed if the logging
		''' level is such that the message will actually be logged.
		''' <p>
		''' If the logger is currently enabled for the FINEST message
		''' level then the message is constructed by invoking the provided
		''' supplier function and forwarded to all the registered output
		''' Handler objects.
		''' <p> </summary>
		''' <param name="msgSupplier">   A function, which when called, produces the
		'''                        desired log message
		''' @since   1.8 </param>
		Public Overridable Sub finest(ByVal msgSupplier As java.util.function.Supplier(Of String))
			log(Level.FINEST, msgSupplier)
		End Sub

		'================================================================
		' End of convenience methods
		'================================================================

		''' <summary>
		''' Set the log level specifying which message levels will be
		''' logged by this logger.  Message levels lower than this
		''' value will be discarded.  The level value Level.OFF
		''' can be used to turn off logging.
		''' <p>
		''' If the new level is null, it means that this node should
		''' inherit its level from its nearest ancestor with a specific
		''' (non-null) level value.
		''' </summary>
		''' <param name="newLevel">   the new value for the log level (may be null) </param>
		''' <exception cref="SecurityException"> if a security manager exists,
		'''          this logger is not anonymous, and the caller
		'''          does not have LoggingPermission("control"). </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setLevel(ByVal newLevel As Level) 'JavaToDotNetTempPropertySetlevel
		Public Overridable Property level As Level
			Set(ByVal newLevel As Level)
				checkPermission()
				SyncLock treeLock
					levelObject = newLevel
					updateEffectiveLevel()
				End SyncLock
			End Set
			Get
		End Property

		Friend Property levelInitialized As Boolean
			Get
				Return levelObject IsNot Nothing
			End Get
		End Property

			Return levelObject
		End Function

		''' <summary>
		''' Check if a message of the given level would actually be logged
		''' by this logger.  This check is based on the Loggers effective level,
		''' which may be inherited from its parent.
		''' </summary>
		''' <param name="level">   a message logging level </param>
		''' <returns>  true if the given message level is currently being logged. </returns>
		Public Overridable Function isLoggable(ByVal level_Renamed As Level) As Boolean
			If level_Renamed < levelValue OrElse levelValue = offValue Then Return False
			Return True
		End Function

		''' <summary>
		''' Get the name for this logger. </summary>
		''' <returns> logger name.  Will be null for anonymous Loggers. </returns>
		Public Overridable Property name As String
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' Add a log Handler to receive logging messages.
		''' <p>
		''' By default, Loggers also send their output to their parent logger.
		''' Typically the root Logger is configured with a set of Handlers
		''' that essentially act as default handlers for all loggers.
		''' </summary>
		''' <param name="handler"> a logging Handler </param>
		''' <exception cref="SecurityException"> if a security manager exists,
		'''          this logger is not anonymous, and the caller
		'''          does not have LoggingPermission("control"). </exception>
		Public Overridable Sub [addHandler](ByVal handler As Handler)
			' Check for null handler
			handler.GetType()
			checkPermission()
			handlers.add(handler)
		End Sub

		''' <summary>
		''' Remove a log Handler.
		''' <P>
		''' Returns silently if the given Handler is not found or is null
		''' </summary>
		''' <param name="handler"> a logging Handler </param>
		''' <exception cref="SecurityException"> if a security manager exists,
		'''          this logger is not anonymous, and the caller
		'''          does not have LoggingPermission("control"). </exception>
		Public Overridable Sub [removeHandler](ByVal handler As Handler)
			checkPermission()
			If handler Is Nothing Then Return
			handlers.remove(handler)
		End Sub

		''' <summary>
		''' Get the Handlers associated with this logger.
		''' <p> </summary>
		''' <returns>  an array of all registered Handlers </returns>
		Public Overridable Property handlers As Handler()
			Get
				Return accessCheckedHandlers()
			End Get
		End Property

		' This method should ideally be marked final - but unfortunately
		' it needs to be overridden by LogManager.RootLogger
		Friend Overridable Function accessCheckedHandlers() As Handler()
			Return handlers.ToArray(emptyHandlers)
		End Function

		''' <summary>
		''' Specify whether or not this logger should send its output
		''' to its parent Logger.  This means that any LogRecords will
		''' also be written to the parent's Handlers, and potentially
		''' to its parent, recursively up the namespace.
		''' </summary>
		''' <param name="useParentHandlers">   true if output is to be sent to the
		'''          logger's parent. </param>
		''' <exception cref="SecurityException"> if a security manager exists,
		'''          this logger is not anonymous, and the caller
		'''          does not have LoggingPermission("control"). </exception>
		Public Overridable Property useParentHandlers As Boolean
			Set(ByVal useParentHandlers As Boolean)
				checkPermission()
				Me.useParentHandlers = useParentHandlers
			End Set
			Get
				Return useParentHandlers
			End Get
		End Property


		Private Shared Function findSystemResourceBundle(ByVal locale_Renamed As java.util.Locale) As java.util.ResourceBundle
			' the resource bundle is in a restricted package
			Return java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overrides Function run() As java.util.ResourceBundle
				Try
					Return java.util.ResourceBundle.getBundle(SYSTEM_LOGGER_RB_NAME, locale, ClassLoader.systemClassLoader)
				Catch e As java.util.MissingResourceException
					Throw New InternalError(e.ToString())
				End Try
			End Function
		End Class

		''' <summary>
		''' Private utility method to map a resource bundle name to an
		''' actual resource bundle, using a simple one-entry cache.
		''' Returns null for a null name.
		''' May also return null if we can't find the resource bundle and
		''' there is no suitable previous cached value.
		''' </summary>
		''' <param name="name"> the ResourceBundle to locate </param>
		''' <param name="userCallersClassLoader"> if true search using the caller's ClassLoader </param>
		''' <returns> ResourceBundle specified by name or null if not found </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Function findResourceBundle(ByVal name As String, ByVal useCallersClassLoader As Boolean) As java.util.ResourceBundle
			' For all lookups, we first check the thread context class loader
			' if it is set.  If not, we use the system classloader.  If we
			' still haven't found it we use the callersClassLoaderRef if it
			' is set and useCallersClassLoader is true.  We set
			' callersClassLoaderRef initially upon creating the logger with a
			' non-null resource bundle name.

			' Return a null bundle for a null name.
			If name Is Nothing Then Return Nothing

			Dim currentLocale As java.util.Locale = java.util.Locale.default
			Dim lb As LoggerBundle = loggerBundle_Renamed

			' Normally we should hit on our simple one entry cache.
			If lb.userBundle IsNot Nothing AndAlso name.Equals(lb.resourceBundleName) Then
				Return lb.userBundle
			ElseIf catalog IsNot Nothing AndAlso currentLocale.Equals(catalogLocale) AndAlso name.Equals(catalogName) Then
				Return catalog
			End If

			If name.Equals(SYSTEM_LOGGER_RB_NAME) Then
				catalog = findSystemResourceBundle(currentLocale)
				catalogName = name
				catalogLocale = currentLocale
				Return catalog
			End If

			' Use the thread's context ClassLoader.  If there isn't one, use the
			' {@linkplain java.lang.ClassLoader#getSystemClassLoader() system ClassLoader}.
			Dim cl As  ClassLoader = Thread.CurrentThread.contextClassLoader
			If cl Is Nothing Then cl = ClassLoader.systemClassLoader
			Try
				catalog = java.util.ResourceBundle.getBundle(name, currentLocale, cl)
				catalogName = name
				catalogLocale = currentLocale
				Return catalog
			Catch ex As java.util.MissingResourceException
				' We can't find the ResourceBundle in the default
				' ClassLoader.  Drop through.
			End Try

			If useCallersClassLoader Then
				' Try with the caller's ClassLoader
				Dim callersClassLoader_Renamed As  ClassLoader = callersClassLoader

				If callersClassLoader_Renamed Is Nothing OrElse callersClassLoader_Renamed Is cl Then Return Nothing

				Try
					catalog = java.util.ResourceBundle.getBundle(name, currentLocale, callersClassLoader_Renamed)
					catalogName = name
					catalogLocale = currentLocale
					Return catalog
				Catch ex As java.util.MissingResourceException
					Return Nothing ' no luck
				End Try
			Else
				Return Nothing
			End If
		End Function

		' Private utility method to initialize our one entry
		' resource bundle name cache and the callers ClassLoader
		' Note: for consistency reasons, we are careful to check
		' that a suitable ResourceBundle exists before setting the
		' resourceBundleName field.
		' Synchronized to prevent races in setting the fields.
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub setupResourceInfo(ByVal name As String, ByVal callersClass As [Class])
			Dim lb As LoggerBundle = loggerBundle_Renamed
			If lb.resourceBundleName IsNot Nothing Then
				' this Logger already has a ResourceBundle

				If lb.resourceBundleName.Equals(name) Then Return

				' cannot change ResourceBundles once they are set
				Throw New IllegalArgumentException(lb.resourceBundleName & " != " & name)
			End If

			If name Is Nothing Then Return

			callersClassLoaderRef = callersClass
			If isSystemLogger AndAlso callersClassLoader IsNot Nothing Then checkPermission()
			If findResourceBundle(name, True) Is Nothing Then
				' We've failed to find an expected ResourceBundle.
				' unset the caller's ClassLoader since we were unable to find the
				' the bundle using it
				Me.callersClassLoaderRef = Nothing
				Throw New java.util.MissingResourceException("Can't find " & name & " bundle", name, "")
			End If

			' if lb.userBundle is not null we won't reach this line.
			Debug.Assert(lb.userBundle Is Nothing)
			loggerBundle_Renamed = LoggerBundle.get(name, Nothing)
		End Sub

		''' <summary>
		''' Sets a resource bundle on this logger.
		''' All messages will be logged using the given resource bundle for its
		''' specific <seealso cref="ResourceBundle#getLocale locale"/>. </summary>
		''' <param name="bundle"> The resource bundle that this logger shall use. </param>
		''' <exception cref="NullPointerException"> if the given bundle is {@code null}. </exception>
		''' <exception cref="IllegalArgumentException"> if the given bundle doesn't have a
		'''         <seealso cref="ResourceBundle#getBaseBundleName base name"/>,
		'''         or if this logger already has a resource bundle set but
		'''         the given bundle has a different base name. </exception>
		''' <exception cref="SecurityException"> if a security manager exists,
		'''         this logger is not anonymous, and the caller
		'''         does not have LoggingPermission("control").
		''' @since 1.8 </exception>
		Public Overridable Property resourceBundle As java.util.ResourceBundle
			Set(ByVal bundle As java.util.ResourceBundle)
				checkPermission()
    
				' Will throw NPE if bundle is null.
				Dim baseName As String = bundle.baseBundleName
    
				' bundle must have a name
				If baseName Is Nothing OrElse baseName.empty Then Throw New IllegalArgumentException("resource bundle must have a name")
    
				SyncLock Me
					Dim lb As LoggerBundle = loggerBundle_Renamed
					Dim canReplaceResourceBundle As Boolean = lb.resourceBundleName Is Nothing OrElse lb.resourceBundleName.Equals(baseName)
    
					If Not canReplaceResourceBundle Then Throw New IllegalArgumentException("can't replace resource bundle")
    
    
					loggerBundle_Renamed = LoggerBundle.get(baseName, bundle)
				End SyncLock
			End Set
		End Property

		''' <summary>
		''' Return the parent for this Logger.
		''' <p>
		''' This method returns the nearest extant parent in the namespace.
		''' Thus if a Logger is called "a.b.c.d", and a Logger called "a.b"
		''' has been created but no logger "a.b.c" exists, then a call of
		''' getParent on the Logger "a.b.c.d" will return the Logger "a.b".
		''' <p>
		''' The result will be null if it is called on the root Logger
		''' in the namespace.
		''' </summary>
		''' <returns> nearest existing parent Logger </returns>
		Public Overridable Property parent As Logger
			Get
				' Note: this used to be synchronized on treeLock.  However, this only
				' provided memory semantics, as there was no guarantee that the caller
				' would synchronize on treeLock (in fact, there is no way for external
				' callers to so synchronize).  Therefore, we have made parent volatile
				' instead.
				Return parent
			End Get
			Set(ByVal parent As Logger)
				If parent Is Nothing Then Throw New NullPointerException
    
				' check permission for all loggers, including anonymous loggers
				If manager Is Nothing Then manager = LogManager.logManager
				manager.checkPermission()
    
				doSetParent(parent)
			End Set
		End Property


		' Private method to do the work for parenting a child
		' Logger onto a parent logger.
		Private Sub doSetParent(ByVal newParent As Logger)

			' System.err.println("doSetParent \"" + getName() + "\" \""
			'                              + newParent.getName() + "\"");

			SyncLock treeLock

				' Remove ourself from any previous parent.
				Dim ref As LogManager.LoggerWeakRef = Nothing
				If parent IsNot Nothing Then
					' assert parent.kids != null;
					Dim iter As IEnumerator(Of LogManager.LoggerWeakRef) = parent.kids.GetEnumerator()
					Do While iter.MoveNext()
						ref = iter.Current
						Dim kid As Logger = ref.get()
						If kid Is Me Then
							' ref is used down below to complete the reparenting
							iter.remove()
							Exit Do
						Else
							ref = Nothing
						End If
					Loop
					' We have now removed ourself from our parents' kids.
				End If

				' Set our new parent.
				parent = newParent
				If parent.kids Is Nothing Then parent.kids = New List(Of )(2)
				If ref Is Nothing Then ref = manager.new LoggerWeakRef(Me)
				ref.parentRef = New WeakReference(Of )(parent)
				parent.kids.add(ref)

				' As a result of the reparenting, the effective level
				' may have changed for us and our children.
				updateEffectiveLevel()

			End SyncLock
		End Sub

		' Package-level method.
		' Remove the weak reference for the specified child Logger from the
		' kid list. We should only be called from LoggerWeakRef.dispose().
		Friend Sub removeChildLogger(ByVal child As LogManager.LoggerWeakRef)
			SyncLock treeLock
				Dim iter As IEnumerator(Of LogManager.LoggerWeakRef) = kids.GetEnumerator()
				Do While iter.MoveNext()
					Dim ref As LogManager.LoggerWeakRef = iter.Current
					If ref Is child Then
						iter.remove()
						Return
					End If
				Loop
			End SyncLock
		End Sub

		' Recalculate the effective level for this node and
		' recursively for our children.

		Private Sub updateEffectiveLevel()
			' assert Thread.holdsLock(treeLock);

			' Figure out our current effective level.
			Dim newLevelValue As Integer
			If levelObject IsNot Nothing Then
				newLevelValue = levelObject
			Else
				If parent IsNot Nothing Then
					newLevelValue = parent.levelValue
				Else
					' This may happen during initialization.
					newLevelValue = Level.INFO
				End If
			End If

			' If our effective value hasn't changed, we're done.
			If levelValue = newLevelValue Then Return

			levelValue = newLevelValue

			' System.err.println("effective level: \"" + getName() + "\" := " + level);

			' Recursively update the level on each of our kids.
			If kids IsNot Nothing Then
				For i As Integer = 0 To kids.size() - 1
					Dim ref As LogManager.LoggerWeakRef = kids.get(i)
					Dim kid As Logger = ref.get()
					If kid IsNot Nothing Then kid.updateEffectiveLevel()
				Next i
			End If
		End Sub


		' Private method to get the potentially inherited
		' resource bundle and resource bundle name for this Logger.
		' This method never returns null.
		Private Property effectiveLoggerBundle As LoggerBundle
			Get
				Dim lb As LoggerBundle = loggerBundle_Renamed
				If lb.systemBundle Then Return SYSTEM_BUNDLE
    
				' first take care of this logger
				Dim b As java.util.ResourceBundle = resourceBundle
				If b IsNot Nothing AndAlso b Is lb.userBundle Then
					Return lb
				ElseIf b IsNot Nothing Then
					' either lb.userBundle is null or getResourceBundle() is
					' overriden
					Dim rbName As String = resourceBundleName
					Return LoggerBundle.get(rbName, b)
				End If
    
				' no resource bundle was specified on this logger, look up the
				' parent stack.
				Dim target As Logger = Me.parent
				Do While target IsNot Nothing
					Dim trb As LoggerBundle = target.loggerBundle_Renamed
					If trb.systemBundle Then Return SYSTEM_BUNDLE
					If trb.userBundle IsNot Nothing Then Return trb
					Dim rbName As String = If(isSystemLogger, (If(target.isSystemLogger, trb.resourceBundleName, Nothing)), target.resourceBundleName)
						' ancestor of a system logger is expected to be a system logger.
						' ignore resource bundle name if it's not.
					If rbName IsNot Nothing Then Return LoggerBundle.get(rbName, findResourceBundle(rbName, True))
					target = If(isSystemLogger, target.parent, target.parent)
				Loop
				Return NO_RESOURCE_BUNDLE
			End Get
		End Property

	End Class

End Namespace
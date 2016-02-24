Imports System
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports org.omg.CORBA.portable

'
' * Copyright (c) 1995, 2014, Oracle and/or its affiliates. All rights reserved.
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

Namespace org.omg.CORBA





	''' <summary>
	''' A class providing APIs for the CORBA Object Request Broker
	''' features.  The <code>ORB</code> class also provides
	''' "pluggable ORB implementation" APIs that allow another vendor's ORB
	''' implementation to be used.
	''' <P>
	''' An ORB makes it possible for CORBA objects to communicate
	''' with each other by connecting objects making requests (clients) with
	''' objects servicing requests (servers).
	''' <P>
	''' 
	''' The <code>ORB</code> class, which
	''' encapsulates generic CORBA functionality, does the following:
	''' (Note that items 5 and 6, which include most of the methods in
	''' the class <code>ORB</code>, are typically used with the <code>Dynamic Invocation
	''' Interface</code> (DII) and the <code>Dynamic Skeleton Interface</code>
	''' (DSI).
	''' These interfaces may be used by a developer directly, but
	''' most commonly they are used by the ORB internally and are
	''' not seen by the general programmer.)
	''' <OL>
	''' <li> initializes the ORB implementation by supplying values for
	'''      predefined properties and environmental parameters
	''' <li> obtains initial object references to services such as
	''' the NameService using the method <code>resolve_initial_references</code>
	''' <li> converts object references to strings and back
	''' <li> connects the ORB to a servant (an instance of a CORBA object
	''' implementation) and disconnects the ORB from a servant
	''' <li> creates objects such as
	'''   <ul>
	'''   <li><code>TypeCode</code>
	'''   <li><code>Any</code>
	'''   <li><code>NamedValue</code>
	'''   <li><code>Context</code>
	'''   <li><code>Environment</code>
	'''   <li>lists (such as <code>NVList</code>) containing these objects
	'''   </ul>
	''' <li> sends multiple messages in the DII
	''' </OL>
	''' 
	''' <P>
	''' The <code>ORB</code> class can be used to obtain references to objects
	''' implemented anywhere on the network.
	''' <P>
	''' An application or applet gains access to the CORBA environment
	''' by initializing itself into an <code>ORB</code> using one of
	''' three <code>init</code> methods.  Two of the three methods use the properties
	''' (associations of a name with a value) shown in the
	''' table below.<BR>
	''' <TABLE BORDER=1 SUMMARY="Standard Java CORBA Properties">
	''' <TR><TH>Property Name</TH>   <TH>Property Value</TH></TR>
	''' <CAPTION>Standard Java CORBA Properties:</CAPTION>
	'''     <TR><TD>org.omg.CORBA.ORBClass</TD>
	'''     <TD>class name of an ORB implementation</TD></TR>
	'''     <TR><TD>org.omg.CORBA.ORBSingletonClass</TD>
	'''     <TD>class name of the ORB returned by <code>init()</code></TD></TR>
	''' </TABLE>
	''' <P>
	''' These properties allow a different vendor's <code>ORB</code>
	''' implementation to be "plugged in."
	''' <P>
	''' When an ORB instance is being created, the class name of the ORB
	''' implementation is located using
	''' the following standard search order:<P>
	''' 
	''' <OL>
	'''     <LI>check in Applet parameter or application string array, if any
	''' 
	'''     <LI>check in properties parameter, if any
	''' 
	'''     <LI>check in the System properties
	''' 
	'''     <LI>check in the orb.properties file located in the user.home
	'''         directory (if any)
	''' 
	'''     <LI>check in the orb.properties file located in the java.home/lib
	'''         directory (if any)
	''' 
	'''     <LI>fall back on a hardcoded default behavior (use the Java&nbsp;IDL
	'''         implementation)
	''' </OL>
	''' <P>
	''' Note that Java&nbsp;IDL provides a default implementation for the
	''' fully-functional ORB and for the Singleton ORB.  When the method
	''' <code>init</code> is given no parameters, the default Singleton
	''' ORB is returned.  When the method <code>init</code> is given parameters
	''' but no ORB class is specified, the Java&nbsp;IDL ORB implementation
	''' is returned.
	''' <P>
	''' The following code fragment creates an <code>ORB</code> object
	''' initialized with the default ORB Singleton.
	''' This ORB has a
	''' restricted implementation to prevent malicious applets from doing
	''' anything beyond creating typecodes.
	''' It is called a singleton
	''' because there is only one instance for an entire virtual machine.
	''' <PRE>
	'''    ORB orb = ORB.init();
	''' </PRE>
	''' <P>
	''' The following code fragment creates an <code>ORB</code> object
	''' for an application.  The parameter <code>args</code>
	''' represents the arguments supplied to the application's <code>main</code>
	''' method.  Since the property specifies the ORB class to be
	''' "SomeORBImplementation", the new ORB will be initialized with
	''' that ORB implementation.  If p had been null,
	''' and the arguments had not specified an ORB class,
	''' the new ORB would have been
	''' initialized with the default Java&nbsp;IDL implementation.
	''' <PRE>
	'''    Properties p = new Properties();
	'''    p.put("org.omg.CORBA.ORBClass", "SomeORBImplementation");
	'''    ORB orb = ORB.init(args, p);
	''' </PRE>
	''' <P>
	''' The following code fragment creates an <code>ORB</code> object
	''' for the applet supplied as the first parameter.  If the given
	''' applet does not specify an ORB class, the new ORB will be
	''' initialized with the default Java&nbsp;IDL implementation.
	''' <PRE>
	'''    ORB orb = ORB.init(myApplet, null);
	''' </PRE>
	''' <P>
	''' An application or applet can be initialized in one or more ORBs.
	''' ORB initialization is a bootstrap call into the CORBA world.
	''' @since   JDK1.2
	''' </summary>
	Public MustInherit Class ORB

		'
		' This is the ORB implementation used when nothing else is specified.
		' Whoever provides this class customizes this string to
		' point at their ORB implementation.
		'
		Private Const ORBClassKey As String = "org.omg.CORBA.ORBClass"
		Private Const ORBSingletonClassKey As String = "org.omg.CORBA.ORBSingletonClass"

		'
		' The global instance of the singleton ORB implementation which
		' acts as a factory for typecodes for generated Helper classes.
		' TypeCodes should be immutable since they may be shared across
		' different security contexts (applets). There should be no way to
		' use a TypeCode as a storage depot for illicitly passing
		' information or Java objects between different security contexts.
		'
		Private Shared singleton As ORB

		' Get System property
		Private Shared Function getSystemProperty(ByVal name As String) As String

			' This will not throw a SecurityException because this
			' class was loaded from rt.jar using the bootstrap classloader.
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			String propValue = (String) java.security.AccessController.doPrivileged(New java.security.PrivilegedAction()
	'		{
	'				public java.lang.Object run()
	'				{
	'					Return System.getProperty(name);
	'				}
	'			}
		   )

			Return propValue
		End Function

		' Get property from orb.properties in either <user.home> or <java-home>/lib
		' directories.
		Private Shared Function getPropertyFromFile(ByVal name As String) As String
			' This will not throw a SecurityException because this
			' class was loaded from rt.jar using the bootstrap classloader.

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			String propValue = (String) java.security.AccessController.doPrivileged(New java.security.PrivilegedAction()
	'		{
	'				private Properties getFileProperties(String fileName)
	'				{
	'					try
	'					{
	'						File propFile = New File(fileName);
	'						if (!propFile.exists())
	'							Return Nothing;
	'
	'						Properties props = New Properties();
	'						FileInputStream fis = New FileInputStream(propFile);
	'						try
	'						{
	'							props.load(fis);
	'						}
	'						finally
	'						{
	'							fis.close();
	'						}
	'
	'						Return props;
	'					}
	'					catch (Exception exc)
	'					{
	'						Return Nothing;
	'					}
	'				}
	'
	'				public java.lang.Object run()
	'				{
	'					String userHome = System.getProperty("user.home");
	'					String fileName = userHome + File.separator + "orb.properties";
	'					Properties props = getFileProperties(fileName);
	'
	'					if (props != Nothing)
	'					{
	'						String value = props.getProperty(name);
	'						if (value != Nothing)
	'							Return value;
	'					}
	'
	'					String javaHome = System.getProperty("java.home");
	'					fileName = javaHome + File.separator + "lib" + File.separator + "orb.properties";
	'					props = getFileProperties(fileName);
	'
	'					if (props == Nothing)
	'						Return Nothing;
	'					else
	'						Return props.getProperty(name);
	'				}
	'			}
		   )

			Return propValue
		End Function

		''' <summary>
		''' Returns the <code>ORB</code> singleton object. This method always returns the
		''' same ORB instance, which is an instance of the class described by the
		''' <code>org.omg.CORBA.ORBSingletonClass</code> system property.
		''' <P>
		''' This no-argument version of the method <code>init</code> is used primarily
		''' as a factory for <code>TypeCode</code> objects, which are used by
		''' <code>Helper</code> classes to implement the method <code>type</code>.
		''' It is also used to create <code>Any</code> objects that are used to
		''' describe <code>union</code> labels (as part of creating a <code>
		''' TypeCode</code> object for a <code>union</code>).
		''' <P>
		''' This method is not intended to be used by applets, and in the event
		''' that it is called in an applet environment, the ORB it returns
		''' is restricted so that it can be used only as a factory for
		''' <code>TypeCode</code> objects.  Any <code>TypeCode</code> objects
		''' it produces can be safely shared among untrusted applets.
		''' <P>
		''' If an ORB is created using this method from an applet,
		''' a system exception will be thrown if
		''' methods other than those for
		''' creating <code>TypeCode</code> objects are invoked.
		''' </summary>
		''' <returns> the singleton ORB </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Function init() As ORB
			If singleton Is Nothing Then
				Dim className As String = getSystemProperty(ORBSingletonClassKey)
				If className Is Nothing Then className = getPropertyFromFile(ORBSingletonClassKey)
				If (className Is Nothing) OrElse (className.Equals("com.sun.corba.se.impl.orb.ORBSingleton")) Then
					singleton = New com.sun.corba.se.impl.orb.ORBSingleton
				Else
					singleton = create_impl(className)
				End If
			End If
			Return singleton
		End Function

		Private Shared Function create_impl(ByVal className As String) As ORB
			Dim cl As ClassLoader = Thread.CurrentThread.contextClassLoader
			If cl Is Nothing Then cl = ClassLoader.systemClassLoader

			Try
				sun.reflect.misc.ReflectUtil.checkPackageAccess(className)
				Dim orbBaseClass As Type = GetType(org.omg.CORBA.ORB)
				Dim orbClass As Type = Type.GetType(className, True, cl).asSubclass(orbBaseClass)
				Return CType(orbClass.newInstance(), ORB)
			Catch ex As Exception
				Dim systemException As SystemException = New INITIALIZE("can't instantiate default ORB implementation " & className)
				systemException.initCause(ex)
				Throw systemException
			End Try
		End Function

		''' <summary>
		''' Creates a new <code>ORB</code> instance for a standalone
		''' application.  This method may be called from applications
		''' only and returns a new fully functional <code>ORB</code> object
		''' each time it is called. </summary>
		''' <param name="args"> command-line arguments for the application's <code>main</code>
		'''             method; may be <code>null</code> </param>
		''' <param name="props"> application-specific properties; may be <code>null</code> </param>
		''' <returns> the newly-created ORB instance </returns>
		Public Shared Function init(ByVal args As String(), ByVal props As java.util.Properties) As ORB
			'
			' Note that there is no standard command-line argument for
			' specifying the default ORB implementation. For an
			' application you can choose an implementation either by
			' setting the CLASSPATH to pick a different org.omg.CORBA
			' and it's baked-in ORB implementation default or by
			' setting an entry in the properties object or in the
			' system properties.
			'
			Dim className As String = Nothing
			Dim orb As ORB

			If props IsNot Nothing Then className = props.getProperty(ORBClassKey)
			If className Is Nothing Then className = getSystemProperty(ORBClassKey)
			If className Is Nothing Then className = getPropertyFromFile(ORBClassKey)
			If (className Is Nothing) OrElse (className.Equals("com.sun.corba.se.impl.orb.ORBImpl")) Then
				orb = New com.sun.corba.se.impl.orb.ORBImpl
			Else
				orb = create_impl(className)
			End If
			orb.set_parameters(args, props)
			Return orb
		End Function


		''' <summary>
		''' Creates a new <code>ORB</code> instance for an applet.  This
		''' method may be called from applets only and returns a new
		''' fully-functional <code>ORB</code> object each time it is called. </summary>
		''' <param name="app"> the applet; may be <code>null</code> </param>
		''' <param name="props"> applet-specific properties; may be <code>null</code> </param>
		''' <returns> the newly-created ORB instance </returns>
		Public Shared Function init(ByVal app As java.applet.Applet, ByVal props As java.util.Properties) As ORB
			Dim className As String
			Dim orb As ORB

			className = app.getParameter(ORBClassKey)
			If className Is Nothing AndAlso props IsNot Nothing Then className = props.getProperty(ORBClassKey)
			If className Is Nothing Then className = getSystemProperty(ORBClassKey)
			If className Is Nothing Then className = getPropertyFromFile(ORBClassKey)
			If (className Is Nothing) OrElse (className.Equals("com.sun.corba.se.impl.orb.ORBImpl")) Then
				orb = New com.sun.corba.se.impl.orb.ORBImpl
			Else
				orb = create_impl(className)
			End If
			orb.set_parameters(app, props)
			Return orb
		End Function

		''' <summary>
		''' Allows the ORB implementation to be initialized with the given
		''' parameters and properties. This method, used in applications only,
		''' is implemented by subclass ORB implementations and called
		''' by the appropriate <code>init</code> method to pass in its parameters.
		''' </summary>
		''' <param name="args"> command-line arguments for the application's <code>main</code>
		'''             method; may be <code>null</code> </param>
		''' <param name="props"> application-specific properties; may be <code>null</code> </param>
		Protected Friend MustOverride Sub set_parameters(ByVal args As String(), ByVal props As java.util.Properties)

		''' <summary>
		''' Allows the ORB implementation to be initialized with the given
		''' applet and parameters. This method, used in applets only,
		''' is implemented by subclass ORB implementations and called
		''' by the appropriate <code>init</code> method to pass in its parameters.
		''' </summary>
		''' <param name="app"> the applet; may be <code>null</code> </param>
		''' <param name="props"> applet-specific properties; may be <code>null</code> </param>
		Protected Friend MustOverride Sub set_parameters(ByVal app As java.applet.Applet, ByVal props As java.util.Properties)

		''' <summary>
		''' Connects the given servant object (a Java object that is
		''' an instance of the server implementation class)
		''' to the ORB. The servant class must
		''' extend the <code>ImplBase</code> class corresponding to the interface that is
		''' supported by the server. The servant must thus be a CORBA object
		''' reference, and inherit from <code>org.omg.CORBA.Object</code>.
		''' Servants created by the user can start receiving remote invocations
		''' after the method <code>connect</code> has been called. A servant may also be
		''' automatically and implicitly connected to the ORB if it is passed as
		''' an IDL parameter in an IDL method invocation on a non-local object,
		''' that is, if the servant object has to be marshalled and sent outside of the
		''' process address space.
		''' <P>
		''' Calling the method <code>connect</code> has no effect
		''' when the servant object is already connected to the ORB.
		''' <P>
		''' Deprecated by the OMG in favor of the Portable Object Adapter APIs.
		''' </summary>
		''' <param name="obj"> The servant object reference </param>
		Public Overridable Sub connect(ByVal obj As org.omg.CORBA.Object)
			Throw New NO_IMPLEMENT
		End Sub

		''' <summary>
		''' Destroys the ORB so that its resources can be reclaimed.
		''' Any operation invoked on a destroyed ORB reference will throw the
		''' <code>OBJECT_NOT_EXIST</code> exception.
		''' Once an ORB has been destroyed, another call to <code>init</code>
		''' with the same ORBid will return a reference to a newly constructed ORB.<p>
		''' If <code>destroy</code> is called on an ORB that has not been shut down,
		''' it will start the shut down process and block until the ORB has shut down
		''' before it destroys the ORB.<br>
		''' If an application calls <code>destroy</code> in a thread that is currently servicing
		''' an invocation, the <code>BAD_INV_ORDER</code> system exception will be thrown
		''' with the OMG minor code 3, since blocking would result in a deadlock.<p>
		''' For maximum portability and to avoid resource leaks, an application should
		''' always call <code>shutdown</code> and <code>destroy</code>
		''' on all ORB instances before exiting.
		''' </summary>
		''' <exception cref="org.omg.CORBA.BAD_INV_ORDER"> if the current thread is servicing an invocation </exception>
		Public Overridable Sub destroy()
			Throw New NO_IMPLEMENT
		End Sub

		''' <summary>
		''' Disconnects the given servant object from the ORB. After this method returns,
		''' the ORB will reject incoming remote requests for the disconnected
		''' servant and will send the exception
		''' <code>org.omg.CORBA.OBJECT_NOT_EXIST</code> back to the
		''' remote client. Thus the object appears to be destroyed from the
		''' point of view of remote clients. Note, however, that local requests issued
		''' using the servant  directly do not
		''' pass through the ORB; hence, they will continue to be processed by the
		''' servant.
		''' <P>
		''' Calling the method <code>disconnect</code> has no effect
		''' if the servant is not connected to the ORB.
		''' <P>
		''' Deprecated by the OMG in favor of the Portable Object Adapter APIs.
		''' </summary>
		''' <param name="obj"> The servant object to be disconnected from the ORB </param>
		Public Overridable Sub disconnect(ByVal obj As org.omg.CORBA.Object)
			Throw New NO_IMPLEMENT
		End Sub

		'
		' ORB method implementations.
		'
		' We are trying to accomplish 2 things at once in this class.
		' It can act as a default ORB implementation front-end,
		' creating an actual ORB implementation object which is a
		' subclass of this ORB class and then delegating the method
		' implementations.
		'
		' To accomplish the delegation model, the 'delegate' private instance
		' variable is set if an instance of this class is created directly.
		'

		''' <summary>
		''' Returns a list of the initially available CORBA object references,
		''' such as "NameService" and "InterfaceRepository".
		''' </summary>
		''' <returns> an array of <code>String</code> objects that represent
		'''         the object references for CORBA services
		'''         that are initially available with this ORB </returns>
		Public MustOverride Function list_initial_services() As String()

		''' <summary>
		''' Resolves a specific object reference from the set of available
		''' initial service names.
		''' </summary>
		''' <param name="object_name"> the name of the initial service as a string </param>
		''' <returns>  the object reference associated with the given name </returns>
		''' <exception cref="InvalidName"> if the given name is not associated with a
		'''                         known service </exception>
		Public MustOverride Function resolve_initial_references(ByVal object_name As String) As org.omg.CORBA.Object

		''' <summary>
		''' Converts the given CORBA object reference to a string.
		''' Note that the format of this string is predefined by IIOP, allowing
		''' strings generated by a different ORB to be converted back into an object
		''' reference.
		''' <P>
		''' The resulting <code>String</code> object may be stored or communicated
		''' in any way that a <code>String</code> object can be manipulated.
		''' </summary>
		''' <param name="obj"> the object reference to stringify </param>
		''' <returns> the string representing the object reference </returns>
		Public MustOverride Function object_to_string(ByVal obj As org.omg.CORBA.Object) As String

		''' <summary>
		''' Converts a string produced by the method <code>object_to_string</code>
		''' back to a CORBA object reference.
		''' </summary>
		''' <param name="str"> the string to be converted back to an object reference.  It must
		''' be the result of converting an object reference to a string using the
		''' method <code>object_to_string</code>. </param>
		''' <returns> the object reference </returns>
		Public MustOverride Function string_to_object(ByVal str As String) As org.omg.CORBA.Object

		''' <summary>
		''' Allocates an <code>NVList</code> with (probably) enough
		''' space for the specified number of <code>NamedValue</code> objects.
		''' Note that the specified size is only a hint to help with
		''' storage allocation and does not imply the maximum size of the list.
		''' </summary>
		''' <param name="count">  suggested number of <code>NamedValue</code> objects for
		'''               which to allocate space </param>
		''' <returns> the newly-created <code>NVList</code>
		''' </returns>
		''' <seealso cref= NVList </seealso>
		Public MustOverride Function create_list(ByVal count As Integer) As NVList

		''' <summary>
		''' Creates an <code>NVList</code> initialized with argument
		''' descriptions for the operation described in the given
		''' <code>OperationDef</code> object.  This <code>OperationDef</code> object
		''' is obtained from an Interface Repository. The arguments in the
		''' returned <code>NVList</code> object are in the same order as in the
		''' original IDL operation definition, which makes it possible for the list
		''' to be used in dynamic invocation requests.
		''' </summary>
		''' <param name="oper">      the <code>OperationDef</code> object to use to create the list </param>
		''' <returns>          a newly-created <code>NVList</code> object containing
		''' descriptions of the arguments to the method described in the given
		''' <code>OperationDef</code> object
		''' </returns>
		''' <seealso cref= NVList </seealso>
		Public Overridable Function create_operation_list(ByVal oper As org.omg.CORBA.Object) As NVList
			' If we came here, it means that the actual ORB implementation
			' did not have a create_operation_list(...CORBA.Object oper) method,
			' so lets check if it has a create_operation_list(OperationDef oper)
			' method.
			Try
				' First try to load the OperationDef class
				Dim opDefClassName As String = "org.omg.CORBA.OperationDef"
				Dim opDefClass As Type = Nothing

				Dim cl As ClassLoader = Thread.CurrentThread.contextClassLoader
				If cl Is Nothing Then cl = ClassLoader.systemClassLoader
				' if this throws a ClassNotFoundException, it will be caught below.
				opDefClass = Type.GetType(opDefClassName, True, cl)

				' OK, we loaded OperationDef. Now try to get the
				' create_operation_list(OperationDef oper) method.
				Dim argc As Type() = { opDefClass }
				Dim meth As System.Reflection.MethodInfo = Me.GetType().GetMethod("create_operation_list", argc)

				' OK, the method exists, so invoke it and be happy.
				Dim argx As Object() = { oper }
				Return CType(meth.invoke(Me, argx), org.omg.CORBA.NVList)
			Catch exs As java.lang.reflect.InvocationTargetException
				Dim t As Exception = exs.targetException
				If TypeOf t Is Exception Then
					Throw CType(t, [Error])
				ElseIf TypeOf t Is Exception Then
					Throw CType(t, Exception)
				Else
					Throw New org.omg.CORBA.NO_IMPLEMENT
				End If
			Catch ex As Exception
				Throw ex
			Catch exr As Exception
				Throw New org.omg.CORBA.NO_IMPLEMENT
			End Try
		End Function


		''' <summary>
		''' Creates a <code>NamedValue</code> object
		''' using the given name, value, and argument mode flags.
		''' <P>
		''' A <code>NamedValue</code> object serves as (1) a parameter or return
		''' value or (2) a context property.
		''' It may be used by itself or
		''' as an element in an <code>NVList</code> object.
		''' </summary>
		''' <param name="s">  the name of the <code>NamedValue</code> object </param>
		''' <param name="any">  the <code>Any</code> value to be inserted into the
		'''             <code>NamedValue</code> object </param>
		''' <param name="flags">  the argument mode flags for the <code>NamedValue</code>: one of
		''' <code>ARG_IN.value</code>, <code>ARG_OUT.value</code>,
		''' or <code>ARG_INOUT.value</code>.
		''' </param>
		''' <returns>  the newly-created <code>NamedValue</code> object </returns>
		''' <seealso cref= NamedValue </seealso>
		Public MustOverride Function create_named_value(ByVal s As String, ByVal any As Any, ByVal flags As Integer) As NamedValue

		''' <summary>
		''' Creates an empty <code>ExceptionList</code> object.
		''' </summary>
		''' <returns>  the newly-created <code>ExceptionList</code> object </returns>
		Public MustOverride Function create_exception_list() As ExceptionList

		''' <summary>
		''' Creates an empty <code>ContextList</code> object.
		''' </summary>
		''' <returns>  the newly-created <code>ContextList</code> object </returns>
		''' <seealso cref= ContextList </seealso>
		''' <seealso cref= Context </seealso>
		Public MustOverride Function create_context_list() As ContextList

		''' <summary>
		''' Gets the default <code>Context</code> object.
		''' </summary>
		''' <returns> the default <code>Context</code> object </returns>
		''' <seealso cref= Context </seealso>
		Public MustOverride Function get_default_context() As Context

		''' <summary>
		''' Creates an <code>Environment</code> object.
		''' </summary>
		''' <returns>  the newly-created <code>Environment</code> object </returns>
		''' <seealso cref= Environment </seealso>
		Public MustOverride Function create_environment() As Environment

		''' <summary>
		''' Creates a new <code>org.omg.CORBA.portable.OutputStream</code> into which
		''' IDL method parameters can be marshalled during method invocation. </summary>
		''' <returns>          the newly-created
		'''              <code>org.omg.CORBA.portable.OutputStream</code> object </returns>
		Public MustOverride Function create_output_stream() As org.omg.CORBA.portable.OutputStream

		''' <summary>
		''' Sends multiple dynamic (DII) requests asynchronously without expecting
		''' any responses. Note that oneway invocations are not guaranteed to
		''' reach the server.
		''' </summary>
		''' <param name="req">               an array of request objects </param>
		Public MustOverride Sub send_multiple_requests_oneway(ByVal req As Request())

		''' <summary>
		''' Sends multiple dynamic (DII) requests asynchronously.
		''' </summary>
		''' <param name="req">               an array of <code>Request</code> objects </param>
		Public MustOverride Sub send_multiple_requests_deferred(ByVal req As Request())

		''' <summary>
		''' Finds out if any of the deferred (asynchronous) invocations have
		''' a response yet. </summary>
		''' <returns> <code>true</code> if there is a response available;
		'''         <code> false</code> otherwise </returns>
		Public MustOverride Function poll_next_response() As Boolean

		''' <summary>
		''' Gets the next <code>Request</code> instance for which a response
		''' has been received.
		''' </summary>
		''' <returns>          the next <code>Request</code> object ready with a response </returns>
		''' <exception cref="WrongTransaction"> if the method <code>get_next_response</code>
		''' is called from a transaction scope different
		''' from the one from which the original request was sent. See the
		''' OMG Transaction Service specification for details. </exception>
		Public MustOverride Function get_next_response() As Request

		''' <summary>
		''' Retrieves the <code>TypeCode</code> object that represents
		''' the given primitive IDL type.
		''' </summary>
		''' <param name="tcKind">    the <code>TCKind</code> instance corresponding to the
		'''                  desired primitive type </param>
		''' <returns>          the requested <code>TypeCode</code> object </returns>
		Public MustOverride Function get_primitive_tc(ByVal tcKind As TCKind) As TypeCode

		''' <summary>
		''' Creates a <code>TypeCode</code> object representing an IDL <code>struct</code>.
		''' The <code>TypeCode</code> object is initialized with the given id,
		''' name, and members.
		''' </summary>
		''' <param name="id">        the repository id for the <code>struct</code> </param>
		''' <param name="name">      the name of the <code>struct</code> </param>
		''' <param name="members">   an array describing the members of the <code>struct</code> </param>
		''' <returns>          a newly-created <code>TypeCode</code> object describing
		'''              an IDL <code>struct</code> </returns>
		Public MustOverride Function create_struct_tc(ByVal id As String, ByVal name As String, ByVal members As StructMember()) As TypeCode

		''' <summary>
		''' Creates a <code>TypeCode</code> object representing an IDL <code>union</code>.
		''' The <code>TypeCode</code> object is initialized with the given id,
		''' name, discriminator type, and members.
		''' </summary>
		''' <param name="id">        the repository id of the <code>union</code> </param>
		''' <param name="name">      the name of the <code>union</code> </param>
		''' <param name="discriminator_type">        the type of the <code>union</code> discriminator </param>
		''' <param name="members">   an array describing the members of the <code>union</code> </param>
		''' <returns>          a newly-created <code>TypeCode</code> object describing
		'''              an IDL <code>union</code> </returns>
		Public MustOverride Function create_union_tc(ByVal id As String, ByVal name As String, ByVal discriminator_type As TypeCode, ByVal members As UnionMember()) As TypeCode

		''' <summary>
		''' Creates a <code>TypeCode</code> object representing an IDL <code>enum</code>.
		''' The <code>TypeCode</code> object is initialized with the given id,
		''' name, and members.
		''' </summary>
		''' <param name="id">        the repository id for the <code>enum</code> </param>
		''' <param name="name">      the name for the <code>enum</code> </param>
		''' <param name="members">   an array describing the members of the <code>enum</code> </param>
		''' <returns>          a newly-created <code>TypeCode</code> object describing
		'''              an IDL <code>enum</code> </returns>
		Public MustOverride Function create_enum_tc(ByVal id As String, ByVal name As String, ByVal members As String()) As TypeCode

		''' <summary>
		''' Creates a <code>TypeCode</code> object representing an IDL <code>alias</code>
		''' (<code>typedef</code>).
		''' The <code>TypeCode</code> object is initialized with the given id,
		''' name, and original type.
		''' </summary>
		''' <param name="id">        the repository id for the alias </param>
		''' <param name="name">      the name for the alias </param>
		''' <param name="original_type">
		'''                  the <code>TypeCode</code> object describing the original type
		'''          for which this is an alias </param>
		''' <returns>          a newly-created <code>TypeCode</code> object describing
		'''              an IDL <code>alias</code> </returns>
		Public MustOverride Function create_alias_tc(ByVal id As String, ByVal name As String, ByVal original_type As TypeCode) As TypeCode

		''' <summary>
		''' Creates a <code>TypeCode</code> object representing an IDL <code>exception</code>.
		''' The <code>TypeCode</code> object is initialized with the given id,
		''' name, and members.
		''' </summary>
		''' <param name="id">        the repository id for the <code>exception</code> </param>
		''' <param name="name">      the name for the <code>exception</code> </param>
		''' <param name="members">   an array describing the members of the <code>exception</code> </param>
		''' <returns>          a newly-created <code>TypeCode</code> object describing
		'''              an IDL <code>exception</code> </returns>
		Public MustOverride Function create_exception_tc(ByVal id As String, ByVal name As String, ByVal members As StructMember()) As TypeCode

		''' <summary>
		''' Creates a <code>TypeCode</code> object representing an IDL <code>interface</code>.
		''' The <code>TypeCode</code> object is initialized with the given id
		''' and name.
		''' </summary>
		''' <param name="id">        the repository id for the interface </param>
		''' <param name="name">      the name for the interface </param>
		''' <returns>          a newly-created <code>TypeCode</code> object describing
		'''              an IDL <code>interface</code> </returns>

		Public MustOverride Function create_interface_tc(ByVal id As String, ByVal name As String) As TypeCode

		''' <summary>
		''' Creates a <code>TypeCode</code> object representing a bounded IDL
		''' <code>string</code>.
		''' The <code>TypeCode</code> object is initialized with the given bound,
		''' which represents the maximum length of the string. Zero indicates
		''' that the string described by this type code is unbounded.
		''' </summary>
		''' <param name="bound">     the bound for the <code>string</code>; cannot be negative </param>
		''' <returns>          a newly-created <code>TypeCode</code> object describing
		'''              a bounded IDL <code>string</code> </returns>
		''' <exception cref="BAD_PARAM"> if bound is a negative value </exception>

		Public MustOverride Function create_string_tc(ByVal bound As Integer) As TypeCode

		''' <summary>
		''' Creates a <code>TypeCode</code> object representing a bounded IDL
		''' <code>wstring</code> (wide string).
		''' The <code>TypeCode</code> object is initialized with the given bound,
		''' which represents the maximum length of the wide string. Zero indicates
		''' that the string described by this type code is unbounded.
		''' </summary>
		''' <param name="bound">     the bound for the <code>wstring</code>; cannot be negative </param>
		''' <returns>          a newly-created <code>TypeCode</code> object describing
		'''              a bounded IDL <code>wstring</code> </returns>
		''' <exception cref="BAD_PARAM"> if bound is a negative value </exception>
		Public MustOverride Function create_wstring_tc(ByVal bound As Integer) As TypeCode

		''' <summary>
		''' Creates a <code>TypeCode</code> object representing an IDL <code>sequence</code>.
		''' The <code>TypeCode</code> object is initialized with the given bound and
		''' element type.
		''' </summary>
		''' <param name="bound">     the bound for the <code>sequence</code>, 0 if unbounded </param>
		''' <param name="element_type">
		'''                  the <code>TypeCode</code> object describing the elements
		'''          contained in the <code>sequence</code> </param>
		''' <returns>          a newly-created <code>TypeCode</code> object describing
		'''              an IDL <code>sequence</code> </returns>
		Public MustOverride Function create_sequence_tc(ByVal bound As Integer, ByVal element_type As TypeCode) As TypeCode

		''' <summary>
		''' Creates a <code>TypeCode</code> object representing a
		''' a recursive IDL <code>sequence</code>.
		''' <P>
		''' For the IDL <code>struct</code> Node in following code fragment,
		''' the offset parameter for creating its sequence would be 1:
		''' <PRE>
		'''    Struct Node {
		'''        long value;
		'''        Sequence &lt;Node&gt; subnodes;
		'''    };
		''' </PRE>
		''' </summary>
		''' <param name="bound">     the bound for the sequence, 0 if unbounded </param>
		''' <param name="offset">    the index to the enclosing <code>TypeCode</code> object
		'''                  that describes the elements of this sequence </param>
		''' <returns>          a newly-created <code>TypeCode</code> object describing
		'''                   a recursive sequence </returns>
		''' @deprecated Use a combination of create_recursive_tc and create_sequence_tc instead 
		''' <seealso cref= #create_recursive_tc(String) create_recursive_tc </seealso>
		''' <seealso cref= #create_sequence_tc(int, TypeCode) create_sequence_tc </seealso>
		<Obsolete("Use a combination of create_recursive_tc and create_sequence_tc instead")> _
		Public MustOverride Function create_recursive_sequence_tc(ByVal bound As Integer, ByVal offset As Integer) As TypeCode

		''' <summary>
		''' Creates a <code>TypeCode</code> object representing an IDL <code>array</code>.
		''' The <code>TypeCode</code> object is initialized with the given length and
		''' element type.
		''' </summary>
		''' <param name="length">    the length of the <code>array</code> </param>
		''' <param name="element_type">  a <code>TypeCode</code> object describing the type
		'''                      of element contained in the <code>array</code> </param>
		''' <returns>          a newly-created <code>TypeCode</code> object describing
		'''              an IDL <code>array</code> </returns>
		Public MustOverride Function create_array_tc(ByVal length As Integer, ByVal element_type As TypeCode) As TypeCode

		''' <summary>
		''' Create a <code>TypeCode</code> object for an IDL native type.
		''' </summary>
		''' <param name="id">        the logical id for the native type. </param>
		''' <param name="name">      the name of the native type. </param>
		''' <returns>          the requested TypeCode. </returns>
		Public Overridable Function create_native_tc(ByVal id As String, ByVal name As String) As org.omg.CORBA.TypeCode
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Create a <code>TypeCode</code> object for an IDL abstract interface.
		''' </summary>
		''' <param name="id">        the logical id for the abstract interface type. </param>
		''' <param name="name">      the name of the abstract interface type. </param>
		''' <returns>          the requested TypeCode. </returns>
		Public Overridable Function create_abstract_interface_tc(ByVal id As String, ByVal name As String) As org.omg.CORBA.TypeCode
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function


		''' <summary>
		''' Create a <code>TypeCode</code> object for an IDL fixed type.
		''' </summary>
		''' <param name="digits">    specifies the total number of decimal digits in the number
		'''                  and must be from 1 to 31 inclusive. </param>
		''' <param name="scale">     specifies the position of the decimal point. </param>
		''' <returns>          the requested TypeCode. </returns>
		Public Overridable Function create_fixed_tc(ByVal digits As Short, ByVal scale As Short) As org.omg.CORBA.TypeCode
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function


		' orbos 98-01-18: Objects By Value -- begin


		''' <summary>
		''' Create a <code>TypeCode</code> object for an IDL value type.
		''' The concrete_base parameter is the TypeCode for the immediate
		''' concrete valuetype base of the valuetype for which the TypeCode
		''' is being created.
		''' It may be null if the valuetype does not have a concrete base.
		''' </summary>
		''' <param name="id">                 the logical id for the value type. </param>
		''' <param name="name">               the name of the value type. </param>
		''' <param name="type_modifier">      one of the value type modifier constants:
		'''                           VM_NONE, VM_CUSTOM, VM_ABSTRACT or VM_TRUNCATABLE </param>
		''' <param name="concrete_base">      a <code>TypeCode</code> object
		'''                           describing the concrete valuetype base </param>
		''' <param name="members">            an array containing the members of the value type </param>
		''' <returns>                   the requested TypeCode </returns>
		Public Overridable Function create_value_tc(ByVal id As String, ByVal name As String, ByVal type_modifier As Short, ByVal concrete_base As TypeCode, ByVal members As ValueMember()) As org.omg.CORBA.TypeCode
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Create a recursive <code>TypeCode</code> object which
		''' serves as a placeholder for a concrete TypeCode during the process of creating
		''' TypeCodes which contain recursion. The id parameter specifies the repository id of
		''' the type for which the recursive TypeCode is serving as a placeholder. Once the
		''' recursive TypeCode has been properly embedded in the enclosing TypeCode which
		''' corresponds to the specified repository id, it will function as a normal TypeCode.
		''' Invoking operations on the recursive TypeCode before it has been embedded in the
		''' enclosing TypeCode will result in a <code>BAD_TYPECODE</code> exception.
		''' <P>
		''' For example, the following IDL type declaration contains recursion:
		''' <PRE>
		'''    Struct Node {
		'''        Sequence&lt;Node&gt; subnodes;
		'''    };
		''' </PRE>
		''' <P>
		''' To create a TypeCode for struct Node, you would invoke the TypeCode creation
		''' operations as shown below:
		''' <PRE>
		''' String nodeID = "IDL:Node:1.0";
		''' TypeCode recursiveSeqTC = orb.create_sequence_tc(0, orb.create_recursive_tc(nodeID));
		''' StructMember[] members = { new StructMember("subnodes", recursiveSeqTC, null) };
		''' TypeCode structNodeTC = orb.create_struct_tc(nodeID, "Node", members);
		''' </PRE>
		''' <P>
		''' Also note that the following is an illegal IDL type declaration:
		''' <PRE>
		'''    Struct Node {
		'''        Node next;
		'''    };
		''' </PRE>
		''' <P>
		''' Recursive types can only appear within sequences which can be empty.
		''' That way marshaling problems, when transmitting the struct in an Any, are avoided.
		''' <P> </summary>
		''' <param name="id">                 the logical id of the referenced type </param>
		''' <returns>                   the requested TypeCode </returns>
		Public Overridable Function create_recursive_tc(ByVal id As String) As org.omg.CORBA.TypeCode
			' implemented in subclass
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Creates a <code>TypeCode</code> object for an IDL value box.
		''' </summary>
		''' <param name="id">                 the logical id for the value type </param>
		''' <param name="name">               the name of the value type </param>
		''' <param name="boxed_type">         the TypeCode for the type </param>
		''' <returns>                   the requested TypeCode </returns>
		Public Overridable Function create_value_box_tc(ByVal id As String, ByVal name As String, ByVal boxed_type As TypeCode) As org.omg.CORBA.TypeCode
			' implemented in subclass
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		' orbos 98-01-18: Objects By Value -- end

		''' <summary>
		''' Creates an IDL <code>Any</code> object initialized to
		''' contain a <code>Typecode</code> object whose <code>kind</code> field
		''' is set to <code>TCKind.tc_null</code>.
		''' </summary>
		''' <returns>          a newly-created <code>Any</code> object </returns>
		Public MustOverride Function create_any() As Any




		''' <summary>
		''' Retrieves a <code>Current</code> object.
		''' The <code>Current</code> interface is used to manage thread-specific
		''' information for use by services such as transactions and security.
		''' </summary>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a>
		''' </seealso>
		''' <returns>          a newly-created <code>Current</code> object </returns>
		''' @deprecated      use <code>resolve_initial_references</code>. 
		<Obsolete("     use <code>resolve_initial_references</code>.")> _
		Public Overridable Function get_current() As org.omg.CORBA.Current
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' This operation blocks the current thread until the ORB has
		''' completed the shutdown process, initiated when some thread calls
		''' <code>shutdown</code>. It may be used by multiple threads which
		''' get all notified when the ORB shuts down.
		''' 
		''' </summary>
		Public Overridable Sub run()
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub

		''' <summary>
		''' Instructs the ORB to shut down, which causes all
		''' object adapters to shut down, in preparation for destruction.<br>
		''' If the <code>wait_for_completion</code> parameter
		''' is true, this operation blocks until all ORB processing (including
		''' processing of currently executing requests, object deactivation,
		''' and other object adapter operations) has completed.
		''' If an application does this in a thread that is currently servicing
		''' an invocation, the <code>BAD_INV_ORDER</code> system exception
		''' will be thrown with the OMG minor code 3,
		''' since blocking would result in a deadlock.<br>
		''' If the <code>wait_for_completion</code> parameter is <code>FALSE</code>,
		''' then shutdown may not have completed upon return.<p>
		''' While the ORB is in the process of shutting down, the ORB operates as normal,
		''' servicing incoming and outgoing requests until all requests have been completed.
		''' Once an ORB has shutdown, only object reference management operations
		''' may be invoked on the ORB or any object reference obtained from it.
		''' An application may also invoke the <code>destroy</code> operation on the ORB itself.
		''' Invoking any other operation will throw the <code>BAD_INV_ORDER</code>
		''' system exception with the OMG minor code 4.<p>
		''' The <code>ORB.run</code> method will return after
		''' <code>shutdown</code> has been called.
		''' </summary>
		''' <param name="wait_for_completion"> <code>true</code> if the call
		'''        should block until the shutdown is complete;
		'''        <code>false</code> if it should return immediately </param>
		''' <exception cref="org.omg.CORBA.BAD_INV_ORDER"> if the current thread is servicing
		'''         an invocation </exception>
		Public Overridable Sub shutdown(ByVal wait_for_completion As Boolean)
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub

		''' <summary>
		''' Returns <code>true</code> if the ORB needs the main thread to
		''' perform some work, and <code>false</code> if the ORB does not
		''' need the main thread.
		''' </summary>
		''' <returns> <code>true</code> if there is work pending, meaning that the ORB
		'''         needs the main thread to perform some work; <code>false</code>
		'''         if there is no work pending and thus the ORB does not need the
		'''         main thread
		'''  </returns>
		Public Overridable Function work_pending() As Boolean
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Performs an implementation-dependent unit of work if called
		''' by the main thread. Otherwise it does nothing.
		''' The methods <code>work_pending</code> and <code>perform_work</code>
		''' can be used in
		''' conjunction to implement a simple polling loop that multiplexes
		''' the main thread among the ORB and other activities.
		''' 
		''' </summary>
		Public Overridable Sub perform_work()
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub

		''' <summary>
		''' Used to obtain information about CORBA facilities and services
		''' that are supported by this ORB. The service type for which
		''' information is being requested is passed in as the in
		''' parameter <tt>service_type</tt>, the values defined by
		''' constants in the CORBA module. If service information is
		''' available for that type, that is returned in the out parameter
		''' <tt>service_info</tt>, and the operation returns the
		''' value <tt>true</tt>. If no information for the requested
		''' services type is available, the operation returns <tt>false</tt>
		'''  (i.e., the service is not supported by this ORB).
		''' <P> </summary>
		''' <param name="service_type"> a <code>short</code> indicating the
		'''        service type for which information is being requested </param>
		''' <param name="service_info"> a <code>ServiceInformationHolder</code> object
		'''        that will hold the <code>ServiceInformation</code> object
		'''        produced by this method </param>
		''' <returns> <code>true</code> if service information is available
		'''        for the <tt>service_type</tt>;
		'''         <tt>false</tt> if no information for the
		'''         requested services type is available </returns>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Function get_service_information(ByVal service_type As Short, ByVal service_info As ServiceInformationHolder) As Boolean
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		' orbos 98-01-18: Objects By Value -- begin

		''' <summary>
		''' Creates a new <code>DynAny</code> object from the given
		''' <code>Any</code> object.
		''' <P> </summary>
		''' <param name="value"> the <code>Any</code> object from which to create a new
		'''        <code>DynAny</code> object </param>
		''' <returns> the new <code>DynAny</code> object created from the given
		'''         <code>Any</code> object </returns>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		''' @deprecated Use the new <a href="../DynamicAny/DynAnyFactory.html">DynAnyFactory</a> API instead 
		<Obsolete("Use the new <a href="../DynamicAny/DynAnyFactory.html">DynAnyFactory</a> API instead")> _
		Public Overridable Function create_dyn_any(ByVal value As org.omg.CORBA.Any) As org.omg.CORBA.DynAny
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Creates a basic <code>DynAny</code> object from the given
		''' <code>TypeCode</code> object.
		''' <P> </summary>
		''' <param name="type"> the <code>TypeCode</code> object from which to create a new
		'''        <code>DynAny</code> object </param>
		''' <returns> the new <code>DynAny</code> object created from the given
		'''         <code>TypeCode</code> object </returns>
		''' <exception cref="org.omg.CORBA.ORBPackage.InconsistentTypeCode"> if the given
		'''         <code>TypeCode</code> object is not consistent with the operation. </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		''' @deprecated Use the new <a href="../DynamicAny/DynAnyFactory.html">DynAnyFactory</a> API instead 
		<Obsolete("Use the new <a href="../DynamicAny/DynAnyFactory.html">DynAnyFactory</a> API instead")> _
		Public Overridable Function create_basic_dyn_any(ByVal type As org.omg.CORBA.TypeCode) As org.omg.CORBA.DynAny
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Creates a new <code>DynStruct</code> object from the given
		''' <code>TypeCode</code> object.
		''' <P> </summary>
		''' <param name="type"> the <code>TypeCode</code> object from which to create a new
		'''        <code>DynStruct</code> object </param>
		''' <returns> the new <code>DynStruct</code> object created from the given
		'''         <code>TypeCode</code> object </returns>
		''' <exception cref="org.omg.CORBA.ORBPackage.InconsistentTypeCode"> if the given
		'''         <code>TypeCode</code> object is not consistent with the operation. </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		''' @deprecated Use the new <a href="../DynamicAny/DynAnyFactory.html">DynAnyFactory</a> API instead 
		<Obsolete("Use the new <a href="../DynamicAny/DynAnyFactory.html">DynAnyFactory</a> API instead")> _
		Public Overridable Function create_dyn_struct(ByVal type As org.omg.CORBA.TypeCode) As org.omg.CORBA.DynStruct
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Creates a new <code>DynSequence</code> object from the given
		''' <code>TypeCode</code> object.
		''' <P> </summary>
		''' <param name="type"> the <code>TypeCode</code> object from which to create a new
		'''        <code>DynSequence</code> object </param>
		''' <returns> the new <code>DynSequence</code> object created from the given
		'''         <code>TypeCode</code> object </returns>
		''' <exception cref="org.omg.CORBA.ORBPackage.InconsistentTypeCode"> if the given
		'''         <code>TypeCode</code> object is not consistent with the operation. </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		''' @deprecated Use the new <a href="../DynamicAny/DynAnyFactory.html">DynAnyFactory</a> API instead 
		<Obsolete("Use the new <a href="../DynamicAny/DynAnyFactory.html">DynAnyFactory</a> API instead")> _
		Public Overridable Function create_dyn_sequence(ByVal type As org.omg.CORBA.TypeCode) As org.omg.CORBA.DynSequence
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function


		''' <summary>
		''' Creates a new <code>DynArray</code> object from the given
		''' <code>TypeCode</code> object.
		''' <P> </summary>
		''' <param name="type"> the <code>TypeCode</code> object from which to create a new
		'''        <code>DynArray</code> object </param>
		''' <returns> the new <code>DynArray</code> object created from the given
		'''         <code>TypeCode</code> object </returns>
		''' <exception cref="org.omg.CORBA.ORBPackage.InconsistentTypeCode"> if the given
		'''         <code>TypeCode</code> object is not consistent with the operation. </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		''' @deprecated Use the new <a href="../DynamicAny/DynAnyFactory.html">DynAnyFactory</a> API instead 
		<Obsolete("Use the new <a href="../DynamicAny/DynAnyFactory.html">DynAnyFactory</a> API instead")> _
		Public Overridable Function create_dyn_array(ByVal type As org.omg.CORBA.TypeCode) As org.omg.CORBA.DynArray
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Creates a new <code>DynUnion</code> object from the given
		''' <code>TypeCode</code> object.
		''' <P> </summary>
		''' <param name="type"> the <code>TypeCode</code> object from which to create a new
		'''        <code>DynUnion</code> object </param>
		''' <returns> the new <code>DynUnion</code> object created from the given
		'''         <code>TypeCode</code> object </returns>
		''' <exception cref="org.omg.CORBA.ORBPackage.InconsistentTypeCode"> if the given
		'''         <code>TypeCode</code> object is not consistent with the operation. </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		''' @deprecated Use the new <a href="../DynamicAny/DynAnyFactory.html">DynAnyFactory</a> API instead 
		<Obsolete("Use the new <a href="../DynamicAny/DynAnyFactory.html">DynAnyFactory</a> API instead")> _
		Public Overridable Function create_dyn_union(ByVal type As org.omg.CORBA.TypeCode) As org.omg.CORBA.DynUnion
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Creates a new <code>DynEnum</code> object from the given
		''' <code>TypeCode</code> object.
		''' <P> </summary>
		''' <param name="type"> the <code>TypeCode</code> object from which to create a new
		'''        <code>DynEnum</code> object </param>
		''' <returns> the new <code>DynEnum</code> object created from the given
		'''         <code>TypeCode</code> object </returns>
		''' <exception cref="org.omg.CORBA.ORBPackage.InconsistentTypeCode"> if the given
		'''         <code>TypeCode</code> object is not consistent with the operation. </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		''' @deprecated Use the new <a href="../DynamicAny/DynAnyFactory.html">DynAnyFactory</a> API instead 
		<Obsolete("Use the new <a href="../DynamicAny/DynAnyFactory.html">DynAnyFactory</a> API instead")> _
		Public Overridable Function create_dyn_enum(ByVal type As org.omg.CORBA.TypeCode) As org.omg.CORBA.DynEnum
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Can be invoked to create new instances of policy objects
		''' of a specific type with specified initial state. If
		''' <tt>create_policy</tt> fails to instantiate a new Policy
		''' object due to its inability to interpret the requested type
		''' and content of the policy, it raises the <tt>PolicyError</tt>
		''' exception with the appropriate reason. </summary>
		''' <param name="type"> the <tt>PolicyType</tt> of the policy object to
		'''        be created </param>
		''' <param name="val"> the value that will be used to set the initial
		'''        state of the <tt>Policy</tt> object that is created </param>
		''' <returns> Reference to a newly created <tt>Policy</tt> object
		'''        of type specified by the <tt>type</tt> parameter and
		'''        initialized to a state specified by the <tt>val</tt>
		'''        parameter </returns>
		''' @throws <tt>org.omg.CORBA.PolicyError</tt> when the requested
		'''        policy is not supported or a requested initial state
		'''        for the policy is not supported. </exception>
		Public Overridable Function create_policy(ByVal type As Integer, ByVal val As org.omg.CORBA.Any) As org.omg.CORBA.Policy
			' Currently not implemented until PIORB.
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function
	End Class

End Namespace
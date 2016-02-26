Imports System
Imports System.Diagnostics

'
' * Copyright (c) 2004, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.xpath


	''' <summary>
	''' Implementation of <seealso cref="XPathFactory#newInstance(String)"/>.
	''' 
	''' @author <a href="Kohsuke.Kawaguchi@Sun.com">Kohsuke Kawaguchi</a>
	''' @version $Revision: 1.7 $, $Date: 2010-11-01 04:36:14 $
	''' @since 1.5
	''' </summary>
	Friend Class XPathFactoryFinder
		Private Const DEFAULT_PACKAGE As String = "com.sun.org.apache.xpath.internal"

		Private Shared ReadOnly ss As New SecuritySupport
		''' <summary>
		''' debug support code. </summary>
		Private Shared debug As Boolean = False
		Shared Sub New()
			' Use try/catch block to support applets
			Try
				debug = ss.getSystemProperty("jaxp.debug") IsNot Nothing
			Catch unused As Exception
				debug = False
			End Try
		End Sub

		''' <summary>
		''' <p>Cache properties for performance.</p>
		''' </summary>
		Private Shared ReadOnly cacheProps As New java.util.Properties

		''' <summary>
		''' <p>First time requires initialization overhead.</p>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared firstTime As Boolean = True

		''' <summary>
		''' <p>Conditional debug printing.</p>
		''' </summary>
		''' <param name="msg"> to print </param>
		Private Shared Sub debugPrintln(ByVal msg As String)
			If debug Then Console.Error.WriteLine("JAXP: " & msg)
		End Sub

		''' <summary>
		''' <p><code>ClassLoader</code> to use to find <code>XPathFactory</code>.</p>
		''' </summary>
		Private ReadOnly classLoader As ClassLoader

		''' <summary>
		''' <p>Constructor that specifies <code>ClassLoader</code> to use
		''' to find <code>XPathFactory</code>.</p>
		''' </summary>
		''' <param name="loader">
		'''      to be used to load resource and <seealso cref="XPathFactory"/>
		'''      implementations during the resolution process.
		'''      If this parameter is null, the default system class loader
		'''      will be used. </param>
		Public Sub New(ByVal loader As ClassLoader)
			Me.classLoader = loader
			If debug Then debugDisplayClassLoader()
		End Sub

		Private Sub debugDisplayClassLoader()
			Try
				If classLoader Is ss.contextClassLoader Then
					debugPrintln("using thread context class loader (" & classLoader & ") for search")
					Return
				End If
			Catch unused As Exception
				 ' getContextClassLoader() undefined in JDK1.1
			End Try

			If classLoader Is ClassLoader.systemClassLoader Then
				debugPrintln("using system class loader (" & classLoader & ") for search")
				Return
			End If

			debugPrintln("using class loader (" & classLoader & ") for search")
		End Sub

		''' <summary>
		''' <p>Creates a new <seealso cref="XPathFactory"/> object for the specified
		''' object model.</p>
		''' </summary>
		''' <param name="uri">
		'''       Identifies the underlying object model.
		''' </param>
		''' <returns> <code>null</code> if the callee fails to create one.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''      If the parameter is null. </exception>
		Public Overridable Function newFactory(ByVal uri As String) As XPathFactory
			If uri Is Nothing Then Throw New NullPointerException
			Dim f As XPathFactory = _newFactory(uri)
			If f IsNot Nothing Then
				debugPrintln("factory '" & f.GetType().name & "' was found for " & uri)
			Else
				debugPrintln("unable to find a factory for " & uri)
			End If
			Return f
		End Function

		''' <summary>
		''' <p>Lookup a <seealso cref="XPathFactory"/> for the given object model.</p>
		''' </summary>
		''' <param name="uri"> identifies the object model.
		''' </param>
		''' <returns> <seealso cref="XPathFactory"/> for the given object model. </returns>
		Private Function _newFactory(ByVal uri As String) As XPathFactory
			Dim xpathFactory As XPathFactory = Nothing

			Dim propertyName As String = SERVICE_CLASS.name & ":" & uri

			' system property look up
			Try
				debugPrintln("Looking up system property '" & propertyName & "'")
				Dim r As String = ss.getSystemProperty(propertyName)
				If r IsNot Nothing Then
					debugPrintln("The value is '" & r & "'")
					xpathFactory = createInstance(r, True)
					If xpathFactory IsNot Nothing Then Return xpathFactory
				Else
					debugPrintln("The property is undefined.")
				End If
			Catch t As Exception
				If debug Then
					debugPrintln("failed to look up system property '" & propertyName & "'")
					t.printStackTrace()
				End If
			End Try

			Dim javah As String = ss.getSystemProperty("java.home")
			Dim configFile As String = javah + File.separator & "lib" & File.separator & "jaxp.properties"

			' try to read from $java.home/lib/jaxp.properties
			Try
				If firstTime Then
					SyncLock cacheProps
						If firstTime Then
							Dim f As New File(configFile)
							firstTime = False
							If ss.doesFileExist(f) Then
								debugPrintln("Read properties file " & f)
								cacheProps.load(ss.getFileInputStream(f))
							End If
						End If
					End SyncLock
				End If
				Dim factoryClassName As String = cacheProps.getProperty(propertyName)
				debugPrintln("found " & factoryClassName & " in $java.home/jaxp.properties")

				If factoryClassName IsNot Nothing Then
					xpathFactory = createInstance(factoryClassName, True)
					If xpathFactory IsNot Nothing Then Return xpathFactory
				End If
			Catch ex As Exception
				If debug Then
					Console.WriteLine(ex.ToString())
					Console.Write(ex.StackTrace)
				End If
			End Try

			' Try with ServiceLoader
			Debug.Assert(xpathFactory Is Nothing)
			xpathFactory = findServiceProvider(uri)

			' The following assertion should always be true.
			' Uncomment it, recompile, and run with -ea in case of doubts:
			' assert xpathFactory == null || xpathFactory.isObjectModelSupported(uri);

			If xpathFactory IsNot Nothing Then Return xpathFactory

			' platform default
			If uri.Equals(XPathFactory.DEFAULT_OBJECT_MODEL_URI) Then
				debugPrintln("attempting to use the platform default W3C DOM XPath lib")
				Return createInstance("com.sun.org.apache.xpath.internal.jaxp.XPathFactoryImpl", True)
			End If

			debugPrintln("all things were tried, but none was found. bailing out.")
			Return Nothing
		End Function

		''' <summary>
		''' <p>Create class using appropriate ClassLoader.</p>
		''' </summary>
		''' <param name="className"> Name of class to create. </param>
		''' <returns> Created class or <code>null</code>. </returns>
		Private Function createClass(ByVal className As String) As Type
			Dim clazz As Type
			' make sure we have access to restricted packages
			Dim internal As Boolean = False
			If System.securityManager IsNot Nothing Then
				If className IsNot Nothing AndAlso className.StartsWith(DEFAULT_PACKAGE) Then internal = True
			End If

			' use approprite ClassLoader
			Try
				If classLoader IsNot Nothing AndAlso (Not internal) Then
						clazz = Type.GetType(className, False, classLoader)
				Else
						clazz = Type.GetType(className)
				End If
			Catch t As Exception
				If debug Then t.printStackTrace()
				Return Nothing
			End Try

			Return clazz
		End Function

		''' <summary>
		''' <p>Creates an instance of the specified and returns it.</p>
		''' </summary>
		''' <param name="className">
		'''      fully qualified class name to be instantiated.
		''' </param>
		''' <returns> null
		'''      if it fails. Error messages will be printed by this method. </returns>
		Friend Overridable Function createInstance(ByVal className As String) As XPathFactory
			Return createInstance(className, False)
		End Function

		Friend Overridable Function createInstance(ByVal className As String, ByVal useServicesMechanism As Boolean) As XPathFactory
			Dim ___xPathFactory As XPathFactory = Nothing

			debugPrintln("createInstance(" & className & ")")

			' get Class from className
			Dim clazz As Type = createClass(className)
			If clazz Is Nothing Then
				debugPrintln("failed to getClass(" & className & ")")
				Return Nothing
			End If
			debugPrintln("loaded " & className & " from " & which(clazz))

			' instantiate Class as a XPathFactory
			Try
				If Not useServicesMechanism Then ___xPathFactory = newInstanceNoServiceLoader(clazz)
				If ___xPathFactory Is Nothing Then ___xPathFactory = CType(clazz.newInstance(), XPathFactory)
			Catch classCastException As ClassCastException
					debugPrintln("could not instantiate " & clazz.name)
					If debug Then
							Console.WriteLine(classCastException.ToString())
							Console.Write(classCastException.StackTrace)
					End If
					Return Nothing
			Catch illegalAccessException As IllegalAccessException
					debugPrintln("could not instantiate " & clazz.name)
					If debug Then
							Console.WriteLine(illegalAccessException.ToString())
							Console.Write(illegalAccessException.StackTrace)
					End If
					Return Nothing
			Catch instantiationException As InstantiationException
					debugPrintln("could not instantiate " & clazz.name)
					If debug Then
							Console.WriteLine(instantiationException.ToString())
							Console.Write(instantiationException.StackTrace)
					End If
					Return Nothing
			End Try

			Return ___xPathFactory
		End Function
		''' <summary>
		''' Try to construct using newXPathFactoryNoServiceLoader
		'''   method if available.
		''' </summary>
		Private Shared Function newInstanceNoServiceLoader(ByVal providerClass As Type) As XPathFactory
			' Retain maximum compatibility if no security manager.
			If System.securityManager Is Nothing Then Return Nothing
			Try
				Dim creationMethod As Method = providerClass.getDeclaredMethod("newXPathFactoryNoServiceLoader")
				Dim modifiers As Integer = creationMethod.modifiers

				' Do not call "newXPathFactoryNoServiceLoader" if it's
				' not public static.
				If (Not Modifier.isStatic(modifiers)) OrElse (Not Modifier.isPublic(modifiers)) Then Return Nothing

				' Only calls "newXPathFactoryNoServiceLoader" if it's
				' declared to return an instance of XPathFactory.
				Dim returnType As Type = creationMethod.returnType
				If SERVICE_CLASS.IsAssignableFrom(returnType) Then
					Return SERVICE_CLASS.cast(creationMethod.invoke(Nothing, CType(Nothing, Object())))
				Else
					' Should not happen since
					' XPathFactoryImpl.newXPathFactoryNoServiceLoader is
					' declared to return XPathFactory.
					Throw New ClassCastException(returnType & " cannot be cast to " & SERVICE_CLASS)
				End If
			Catch e As ClassCastException
				Throw New XPathFactoryConfigurationException(e)
			Catch exc As NoSuchMethodException
				Return Nothing
			Catch exc As Exception
				Return Nothing
			End Try
		End Function

		' Call isObjectModelSupportedBy with initial context.
		Private Function isObjectModelSupportedBy(ByVal factory As XPathFactory, ByVal objectModel As String, ByVal acc As java.security.AccessControlContext) As Boolean
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Return java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<java.lang.Boolean>()
	'		{
	'					public java.lang.Boolean run()
	'					{
	'						Return factory.isObjectModelSupported(objectModel);
	'					}
	'				}, acc);
		End Function

		''' <summary>
		''' Finds a service provider subclass of XPathFactory that supports the
		''' given object model using the ServiceLoader.
		''' </summary>
		''' <param name="objectModel"> URI of object model to support. </param>
		''' <returns> An XPathFactory supporting the specified object model, or null
		'''         if none is found. </returns>
		''' <exception cref="XPathFactoryConfigurationException"> if a configuration error is found. </exception>
		Private Function findServiceProvider(ByVal objectModel As String) As XPathFactory

			Debug.Assert(objectModel IsNot Nothing)
			' store current context.
			Dim acc As java.security.AccessControlContext = java.security.AccessController.context
			Try
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<XPathFactory>()
	'			{
	'				public XPathFactory run()
	'				{
	'					final ServiceLoader<XPathFactory> loader = ServiceLoader.load(SERVICE_CLASS);
	'					for (XPathFactory factory : loader)
	'					{
	'						' restore initial context to call
	'						' factory.isObjectModelSupportedBy
	'						if (isObjectModelSupportedBy(factory, objectModel, acc))
	'						{
	'							Return factory;
	'						}
	'					}
	'					Return Nothing; ' no factory found.
	'				}
	'			});
			Catch [error] As java.util.ServiceConfigurationError
				Throw New XPathFactoryConfigurationException([error])
			End Try
		End Function

		Private Shared ReadOnly SERVICE_CLASS As Type = GetType(XPathFactory)

		Private Shared Function which(ByVal clazz As Type) As String
			Return which(clazz.name, clazz.classLoader)
		End Function

		''' <summary>
		''' <p>Search the specified classloader for the given classname.</p>
		''' </summary>
		''' <param name="classname"> the fully qualified name of the class to search for </param>
		''' <param name="loader"> the classloader to search
		''' </param>
		''' <returns> the source location of the resource, or null if it wasn't found </returns>
		Private Shared Function which(ByVal classname As String, ByVal loader As ClassLoader) As String

			Dim classnameAsResource As String = classname.Replace("."c, "/"c) & ".class"

			If loader Is Nothing Then loader = ClassLoader.systemClassLoader

			'URL it = loader.getResource(classnameAsResource);
			Dim it As java.net.URL = ss.getResourceAsURL(loader, classnameAsResource)
			If it IsNot Nothing Then
				Return it.ToString()
			Else
				Return Nothing
			End If
		End Function
	End Class

End Namespace
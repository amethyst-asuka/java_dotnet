Imports System
Imports System.Diagnostics

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

Namespace javax.xml.validation


	''' <summary>
	''' Implementation of <seealso cref="SchemaFactory#newInstance(String)"/>.
	''' 
	''' @author <a href="Kohsuke.Kawaguchi@Sun.com">Kohsuke Kawaguchi</a>
	''' @version $Revision: 1.8 $, $Date: 2010-11-01 04:36:13 $
	''' @since 1.5
	''' </summary>
	Friend Class SchemaFactoryFinder

		''' <summary>
		''' debug support code. </summary>
		Private Shared debug As Boolean = False
		''' <summary>
		''' <p> Take care of restrictions imposed by java security model </p>
		''' </summary>
		Private Shared ReadOnly ss As New SecuritySupport
		Private Const DEFAULT_PACKAGE As String = "com.sun.org.apache.xerces.internal"
		''' <summary>
		''' <p>Cache properties for performance.</p>
		''' </summary>
		Private Shared ReadOnly cacheProps As New java.util.Properties

		''' <summary>
		''' <p>First time requires initialization overhead.</p>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared firstTime As Boolean = True

		Shared Sub New()
			' Use try/catch block to support applets
			Try
				debug = ss.getSystemProperty("jaxp.debug") IsNot Nothing
			Catch unused As Exception
				debug = False
			End Try
		End Sub

		''' <summary>
		''' <p>Conditional debug printing.</p>
		''' </summary>
		''' <param name="msg"> to print </param>
		Private Shared Sub debugPrintln(ByVal msg As String)
			If debug Then Console.Error.WriteLine("JAXP: " & msg)
		End Sub

		''' <summary>
		''' <p><code>ClassLoader</code> to use to find <code>SchemaFactory</code>.</p>
		''' </summary>
		Private ReadOnly classLoader As ClassLoader

		''' <summary>
		''' <p>Constructor that specifies <code>ClassLoader</code> to use
		''' to find <code>SchemaFactory</code>.</p>
		''' </summary>
		''' <param name="loader">
		'''      to be used to load resource, <seealso cref="SchemaFactory"/>, and
		'''      <seealso cref="SchemaFactoryLoader"/> implementations during
		'''      the resolution process.
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
		''' <p>Creates a new <seealso cref="SchemaFactory"/> object for the specified
		''' schema language.</p>
		''' </summary>
		''' <param name="schemaLanguage">
		'''      See <seealso cref="SchemaFactory Schema Language"/> table in <code>SchemaFactory</code>
		'''      for the list of available schema languages.
		''' </param>
		''' <returns> <code>null</code> if the callee fails to create one.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''      If the <code>schemaLanguage</code> parameter is null. </exception>
		''' <exception cref="SchemaFactoryConfigurationError">
		'''      If a configuration error is encountered. </exception>
		Public Overridable Function newFactory(ByVal schemaLanguage As String) As SchemaFactory
			If schemaLanguage Is Nothing Then Throw New NullPointerException
			Dim f As SchemaFactory = _newFactory(schemaLanguage)
			If f IsNot Nothing Then
				debugPrintln("factory '" & f.GetType().name & "' was found for " & schemaLanguage)
			Else
				debugPrintln("unable to find a factory for " & schemaLanguage)
			End If
			Return f
		End Function

		''' <summary>
		''' <p>Lookup a <code>SchemaFactory</code> for the given <code>schemaLanguage</code>.</p>
		''' </summary>
		''' <param name="schemaLanguage"> Schema language to lookup <code>SchemaFactory</code> for.
		''' </param>
		''' <returns> <code>SchemaFactory</code> for the given <code>schemaLanguage</code>. </returns>
		Private Function _newFactory(ByVal schemaLanguage As String) As SchemaFactory
			Dim sf As SchemaFactory

			Dim propertyName As String = SERVICE_CLASS.name & ":" & schemaLanguage

			' system property look up
			Try
				debugPrintln("Looking up system property '" & propertyName & "'")
				Dim r As String = ss.getSystemProperty(propertyName)
				If r IsNot Nothing Then
					debugPrintln("The value is '" & r & "'")
					sf = createInstance(r, True)
					If sf IsNot Nothing Then Return sf
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
					sf = createInstance(factoryClassName, True)
					If sf IsNot Nothing Then Return sf
				End If
			Catch ex As Exception
				If debug Then
					Console.WriteLine(ex.ToString())
					Console.Write(ex.StackTrace)
				End If
			End Try

			' Try with ServiceLoader
			Dim factoryImpl As SchemaFactory = findServiceProvider(schemaLanguage)

			' The following assertion should always be true.
			' Uncomment it, recompile, and run with -ea in case of doubts:
			' assert factoryImpl == null || factoryImpl.isSchemaLanguageSupported(schemaLanguage);

			If factoryImpl IsNot Nothing Then Return factoryImpl

			' platform default
			If schemaLanguage.Equals("http://www.w3.org/2001/XMLSchema") Then
				debugPrintln("attempting to use the platform default XML Schema validator")
				Return createInstance("com.sun.org.apache.xerces.internal.jaxp.validation.XMLSchemaFactory", True)
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
		Friend Overridable Function createInstance(ByVal className As String) As SchemaFactory
			Return createInstance(className, False)
		End Function

		Friend Overridable Function createInstance(ByVal className As String, ByVal useServicesMechanism As Boolean) As SchemaFactory
			Dim ___schemaFactory As SchemaFactory = Nothing

			debugPrintln("createInstance(" & className & ")")

			' get Class from className
			Dim clazz As Type = createClass(className)
			If clazz Is Nothing Then
					debugPrintln("failed to getClass(" & className & ")")
					Return Nothing
			End If
			debugPrintln("loaded " & className & " from " & which(clazz))

			' instantiate Class as a SchemaFactory
			Try
					If Not clazz.IsSubclassOf(GetType(SchemaFactory)) Then Throw New ClassCastException(clazz.name & " cannot be cast to " & GetType(SchemaFactory))
					If Not useServicesMechanism Then ___schemaFactory = newInstanceNoServiceLoader(clazz)
					If ___schemaFactory Is Nothing Then ___schemaFactory = CType(clazz.newInstance(), SchemaFactory)
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

			Return ___schemaFactory
		End Function

		''' <summary>
		''' Try to construct using newXMLSchemaFactoryNoServiceLoader
		'''   method if available.
		''' </summary>
		Private Shared Function newInstanceNoServiceLoader(ByVal providerClass As Type) As SchemaFactory
			' Retain maximum compatibility if no security manager.
			If System.securityManager Is Nothing Then Return Nothing
			Try
				Dim creationMethod As Method = providerClass.getDeclaredMethod("newXMLSchemaFactoryNoServiceLoader")
				Dim modifiers As Integer = creationMethod.modifiers

				' Do not call the method if it's not public static.
				If (Not Modifier.isStatic(modifiers)) OrElse (Not Modifier.isPublic(modifiers)) Then Return Nothing

				' Only calls "newXMLSchemaFactoryNoServiceLoader" if it's
				' declared to return an instance of SchemaFactory.
				Dim returnType As Type = creationMethod.returnType
				If SERVICE_CLASS.IsAssignableFrom(returnType) Then
					Return SERVICE_CLASS.cast(creationMethod.invoke(Nothing, CType(Nothing, Object())))
				Else
					' Should not happen since
					' XMLSchemaFactory.newXMLSchemaFactoryNoServiceLoader is
					' declared to return XMLSchemaFactory.
					Throw New ClassCastException(returnType & " cannot be cast to " & SERVICE_CLASS)
				End If
			Catch e As ClassCastException
				Throw New SchemaFactoryConfigurationError(e.Message, e)
			Catch exc As NoSuchMethodException
				Return Nothing
			Catch exc As Exception
				Return Nothing
			End Try
		End Function

		' Call isSchemaLanguageSupported with initial context.
		Private Function isSchemaLanguageSupportedBy(ByVal factory As SchemaFactory, ByVal schemaLanguage As String, ByVal acc As java.security.AccessControlContext) As Boolean
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Return java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<java.lang.Boolean>()
	'		{
	'			public java.lang.Boolean run()
	'			{
	'				Return factory.isSchemaLanguageSupported(schemaLanguage);
	'			}
	'		}, acc);
		End Function

		''' <summary>
		''' Finds a service provider subclass of SchemaFactory that supports the
		''' given schema language using the ServiceLoader.
		''' </summary>
		''' <param name="schemaLanguage"> The schema language for which we seek a factory. </param>
		''' <returns> A SchemaFactory supporting the specified schema language, or null
		'''         if none is found. </returns>
		''' <exception cref="SchemaFactoryConfigurationError"> if a configuration error is found. </exception>
		Private Function findServiceProvider(ByVal schemaLanguage As String) As SchemaFactory
			Debug.Assert(schemaLanguage IsNot Nothing)
			' store current context.
			Dim acc As java.security.AccessControlContext = java.security.AccessController.context
			Try
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<SchemaFactory>()
	'			{
	'				public SchemaFactory run()
	'				{
	'					final ServiceLoader<SchemaFactory> loader = ServiceLoader.load(SERVICE_CLASS);
	'					for (SchemaFactory factory : loader)
	'					{
	'						' restore initial context to call
	'						' factory.isSchemaLanguageSupported
	'						if (isSchemaLanguageSupportedBy(factory, schemaLanguage, acc))
	'						{
	'							Return factory;
	'						}
	'					}
	'					Return Nothing; ' no factory found.
	'				}
	'			});
			Catch [error] As java.util.ServiceConfigurationError
				Throw New SchemaFactoryConfigurationError("Provider for " & SERVICE_CLASS & " cannot be created", [error])
			End Try
		End Function

		Private Shared ReadOnly SERVICE_CLASS As Type = GetType(SchemaFactory)


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
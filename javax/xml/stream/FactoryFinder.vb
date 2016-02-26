Imports System
Imports System.Diagnostics

'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.stream


	''' <summary>
	''' <p>Implements pluggable streams.</p>
	''' 
	''' <p>This class is duplicated for each JAXP subpackage so keep it in
	''' sync.  It is package private for secure class loading.</p>
	''' 
	''' @author Santiago.PericasGeertsen@sun.com
	''' </summary>
	Friend Class FactoryFinder
		' Check we have access to package.
		Private Const DEFAULT_PACKAGE As String = "com.sun.xml.internal."

		''' <summary>
		''' Internal debug flag.
		''' </summary>
		Private Shared debug As Boolean = False

		''' <summary>
		''' Cache for properties in java.home/lib/jaxp.properties
		''' </summary>
		Private Shared ReadOnly cacheProps As New java.util.Properties

		''' <summary>
		''' Flag indicating if properties from java.home/lib/jaxp.properties
		''' have been cached.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared firstTime As Boolean = True

		''' <summary>
		''' Security support class use to check access control before
		''' getting certain system resources.
		''' </summary>
		Private Shared ReadOnly ss As New SecuritySupport

		' Define system property "jaxp.debug" to get output
		Shared Sub New()
			' Use try/catch block to support applets, which throws
			' SecurityException out of this code.
			Try
				Dim val As String = ss.getSystemProperty("jaxp.debug")
				' Allow simply setting the prop to turn on debug
				debug = val IsNot Nothing AndAlso Not "false".Equals(val)
			Catch se As SecurityException
				debug = False
			End Try
		End Sub

		Private Shared Sub dPrint(ByVal msg As String)
			If debug Then Console.Error.WriteLine("JAXP: " & msg)
		End Sub

		''' <summary>
		''' Attempt to load a class using the class loader supplied. If that fails
		''' and fall back is enabled, the current (i.e. bootstrap) class loader is
		''' tried.
		''' 
		''' If the class loader supplied is <code>null</code>, first try using the
		''' context class loader followed by the current (i.e. bootstrap) class
		''' loader.
		''' 
		''' Use bootstrap classLoader if cl = null and useBSClsLoader is true
		''' </summary>
		Private Shared Function getProviderClass(ByVal className As String, ByVal cl As ClassLoader, ByVal doFallback As Boolean, ByVal useBSClsLoader As Boolean) As Type
			Try
				If cl Is Nothing Then
					If useBSClsLoader Then
						Return Type.GetType(className, False, GetType(FactoryFinder).classLoader)
					Else
						cl = ss.contextClassLoader
						If cl Is Nothing Then
							Throw New ClassNotFoundException
						Else
							Return Type.GetType(className, False, cl)
						End If
					End If
				Else
					Return Type.GetType(className, False, cl)
				End If
			Catch e1 As ClassNotFoundException
				If doFallback Then
					' Use current class loader - should always be bootstrap CL
					Return Type.GetType(className, False, GetType(FactoryFinder).classLoader)
				Else
					Throw e1
				End If
			End Try
		End Function

		''' <summary>
		''' Create an instance of a class. Delegates to method
		''' <code>getProviderClass()</code> in order to load the class.
		''' </summary>
		''' <param name="type"> Base class / Service interface  of the factory to
		'''             instantiate.
		''' </param>
		''' <param name="className"> Name of the concrete class corresponding to the
		''' service provider
		''' </param>
		''' <param name="cl"> <code>ClassLoader</code> used to load the factory class. If <code>null</code>
		''' current <code>Thread</code>'s context classLoader is used to load the factory class.
		''' </param>
		''' <param name="doFallback"> True if the current ClassLoader should be tried as
		''' a fallback if the class is not found using cl </param>
		Friend Shared Function newInstance(Of T)(ByVal type As Type, ByVal className As String, ByVal cl As ClassLoader, ByVal doFallback As Boolean) As T
			Return newInstance(type, className, cl, doFallback, False)
		End Function

		''' <summary>
		''' Create an instance of a class. Delegates to method
		''' <code>getProviderClass()</code> in order to load the class.
		''' </summary>
		''' <param name="type"> Base class / Service interface  of the factory to
		'''             instantiate.
		''' </param>
		''' <param name="className"> Name of the concrete class corresponding to the
		''' service provider
		''' </param>
		''' <param name="cl"> <code>ClassLoader</code> used to load the factory class. If <code>null</code>
		''' current <code>Thread</code>'s context classLoader is used to load the factory class.
		''' </param>
		''' <param name="doFallback"> True if the current ClassLoader should be tried as
		''' a fallback if the class is not found using cl
		''' </param>
		''' <param name="useBSClsLoader"> True if cl=null actually meant bootstrap classLoader. This parameter
		''' is needed since DocumentBuilderFactory/SAXParserFactory defined null as context classLoader. </param>
		Friend Shared Function newInstance(Of T)(ByVal type As Type, ByVal className As String, ByVal cl As ClassLoader, ByVal doFallback As Boolean, ByVal useBSClsLoader As Boolean) As T
			Debug.Assert(type IsNot Nothing)

			' make sure we have access to restricted packages
			If System.securityManager IsNot Nothing Then
				If className IsNot Nothing AndAlso className.StartsWith(DEFAULT_PACKAGE) Then
					cl = Nothing
					useBSClsLoader = True
				End If
			End If

			Try
				Dim ___providerClass As Type = getProviderClass(className, cl, doFallback, useBSClsLoader)
				If Not type.IsAssignableFrom(___providerClass) Then Throw New ClassCastException(className & " cannot be cast to " & type.name)
				Dim instance As Object = ___providerClass.newInstance()
				If debug Then ' Extra check to avoid computing cl strings dPrint("created new instance of " & ___providerClass & " using ClassLoader: " & cl)
				Return type.cast(instance)
			Catch x As ClassNotFoundException
				Throw New FactoryConfigurationError("Provider " & className & " not found", x)
			Catch x As Exception
				Throw New FactoryConfigurationError("Provider " & className & " could not be instantiated: " & x, x)
			End Try
		End Function

		''' <summary>
		''' Finds the implementation Class object in the specified order.
		''' </summary>
		''' <returns> Class object of factory, never null
		''' </returns>
		''' <param name="type">                  Base class / Service interface  of the
		'''                              factory to find.
		''' </param>
		''' <param name="fallbackClassName">     Implementation class name, if nothing else
		'''                              is found.  Use null to mean no fallback.
		''' 
		''' Package private so this code can be shared. </param>
		Friend Shared Function find(Of T)(ByVal type As Type, ByVal fallbackClassName As String) As T
			Return find(type, type.name, Nothing, fallbackClassName)
		End Function

		''' <summary>
		''' Finds the implementation Class object in the specified order.  Main
		''' entry point. </summary>
		''' <returns> Class object of factory, never null
		''' </returns>
		''' <param name="type">                  Base class / Service interface  of the
		'''                              factory to find.
		''' </param>
		''' <param name="factoryId">             Name of the factory to find, same as
		'''                              a property name
		''' </param>
		''' <param name="cl">                    ClassLoader to be used to load the class, null means to use
		''' the bootstrap ClassLoader
		''' </param>
		''' <param name="fallbackClassName">     Implementation class name, if nothing else
		'''                              is found.  Use null to mean no fallback.
		''' 
		''' Package private so this code can be shared. </param>
		Friend Shared Function find(Of T)(ByVal type As Type, ByVal factoryId As String, ByVal cl As ClassLoader, ByVal fallbackClassName As String) As T
			dPrint("find factoryId =" & factoryId)

			' Use the system property first
			Try

				Dim systemProp As String
				If type.name.Equals(factoryId) Then
					systemProp = ss.getSystemProperty(factoryId)
				Else
					systemProp = System.getProperty(factoryId)
				End If
				If systemProp IsNot Nothing Then
					dPrint("found system property, value=" & systemProp)
					Return newInstance(type, systemProp, cl, True)
				End If
			Catch se As SecurityException
				Throw New FactoryConfigurationError("Failed to read factoryId '" & factoryId & "'", se)
			End Try

			' Try read $java.home/lib/stax.properties followed by
			' $java.home/lib/jaxp.properties if former not present
			Dim configFile As String = Nothing
			Try
				If firstTime Then
					SyncLock cacheProps
						If firstTime Then
							configFile = ss.getSystemProperty("java.home") + File.separator & "lib" & File.separator & "stax.properties"
							Dim f As New File(configFile)
							firstTime = False
							If ss.doesFileExist(f) Then
								dPrint("Read properties file " & f)
								cacheProps.load(ss.getFileInputStream(f))
							Else
								configFile = ss.getSystemProperty("java.home") + File.separator & "lib" & File.separator & "jaxp.properties"
								f = New File(configFile)
								If ss.doesFileExist(f) Then
									dPrint("Read properties file " & f)
									cacheProps.load(ss.getFileInputStream(f))
								End If
							End If
						End If
					End SyncLock
				End If
				Dim factoryClassName As String = cacheProps.getProperty(factoryId)

				If factoryClassName IsNot Nothing Then
					dPrint("found in " & configFile & " value=" & factoryClassName)
					Return newInstance(type, factoryClassName, cl, True)
				End If
			Catch ex As Exception
				If debug Then
					Console.WriteLine(ex.ToString())
					Console.Write(ex.StackTrace)
				End If
			End Try

			If type.name.Equals(factoryId) Then
				' Try Jar Service Provider Mechanism
				Dim provider As T = findServiceProvider(type, cl)
				If provider IsNot Nothing Then Return provider
			Else
				' We're in the case where a 'custom' factoryId was provided,
				' and in every case where that happens, we expect that
				' fallbackClassName will be null.
				Debug.Assert(fallbackClassName Is Nothing)
			End If
			If fallbackClassName Is Nothing Then Throw New FactoryConfigurationError("Provider for " & factoryId & " cannot be found", Nothing)

			dPrint("loaded from fallback value: " & fallbackClassName)
			Return newInstance(type, fallbackClassName, cl, True)
		End Function

	'    
	'     * Try to find provider using the ServiceLoader API
	'     *
	'     * @param type Base class / Service interface  of the factory to find.
	'     *
	'     * @return instance of provider class if found or null
	'     
		Private Shared Function findServiceProvider(Of T)(ByVal type As Type, ByVal cl As ClassLoader) As T
			Try
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<T>()
	'			{
	'				@Override public T run()
	'				{
	'					final ServiceLoader<T> serviceLoader;
	'					if (cl == Nothing)
	'					{
	'						'the current thread's context class loader
	'						serviceLoader = ServiceLoader.load(type);
	'					}
	'					else
	'					{
	'						serviceLoader = ServiceLoader.load(type, cl);
	'					}
	'					final Iterator<T> iterator = serviceLoader.iterator();
	'					if (iterator.hasNext())
	'					{
	'						Return iterator.next();
	'					}
	'					else
	'					{
	'						Return Nothing;
	'					}
	'				}
	'			});
			Catch e As java.util.ServiceConfigurationError
				' It is not possible to wrap an error directly in
				' FactoryConfigurationError - so we need to wrap the
				' ServiceConfigurationError in a RuntimeException.
				' The alternative would be to modify the logic in
				' FactoryConfigurationError to allow setting a
				' Throwable as the cause, but that could cause
				' compatibility issues down the road.
				Dim x As New Exception("Provider for " & type & " cannot be created", e)
				Dim [error] As New FactoryConfigurationError(x, x.Message)
				Throw [error]
			End Try
		End Function

	End Class

End Namespace
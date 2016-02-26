Imports System
Imports System.Collections
Imports System.Threading

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

Namespace javax.xml.ws.spi



	Friend Class FactoryFinder

		''' <summary>
		''' Creates an instance of the specified class using the specified
		''' <code>ClassLoader</code> object.
		''' </summary>
		''' <exception cref="WebServiceException"> if the given class could not be found
		'''            or could not be instantiated </exception>
		Private Shared Function newInstance(ByVal className As String, ByVal classLoader As ClassLoader) As Object
			Try
				Dim spiClass As Type = safeLoadClass(className, classLoader)
				Return spiClass.newInstance()
			Catch x As ClassNotFoundException
				Throw New javax.xml.ws.WebServiceException("Provider " & className & " not found", x)
			Catch x As Exception
				Throw New javax.xml.ws.WebServiceException("Provider " & className & " could not be instantiated: " & x, x)
			End Try
		End Function

		''' <summary>
		''' Finds the implementation <code>Class</code> object for the given
		''' factory name, or if that fails, finds the <code>Class</code> object
		''' for the given fallback class name. The arguments supplied MUST be
		''' used in order. If using the first argument is successful, the second
		''' one will not be used.
		''' <P>
		''' This method is package private so that this code can be shared.
		''' </summary>
		''' <returns> the <code>Class</code> object of the specified message factory;
		'''         may not be <code>null</code>
		''' </returns>
		''' <param name="factoryId">             the name of the factory to find, which is
		'''                              a system property </param>
		''' <param name="fallbackClassName">     the implementation class name, which is
		'''                              to be used only if nothing else
		'''                              is found; <code>null</code> to indicate that
		'''                              there is no fallback class name </param>
		''' <exception cref="WebServiceException"> if there is an error </exception>
		Friend Shared Function find(ByVal factoryId As String, ByVal fallbackClassName As String) As Object
			If osgi Then Return lookupUsingOSGiServiceLoader(factoryId)
			Dim classLoader As ClassLoader
			Try
				classLoader = Thread.CurrentThread.contextClassLoader
			Catch x As Exception
				Throw New javax.xml.ws.WebServiceException(x.ToString(), x)
			End Try

			Dim serviceId As String = "META-INF/services/" & factoryId
			' try to find services in CLASSPATH
			Dim rd As BufferedReader = Nothing
			Try
				Dim [is] As InputStream
				If classLoader Is Nothing Then
					[is]=ClassLoader.getSystemResourceAsStream(serviceId)
				Else
					[is]=classLoader.getResourceAsStream(serviceId)
				End If

				If [is] IsNot Nothing Then
					rd = New BufferedReader(New InputStreamReader([is], "UTF-8"))

					Dim factoryClassName As String = rd.readLine()

					If factoryClassName IsNot Nothing AndAlso (Not "".Equals(factoryClassName)) Then Return newInstance(factoryClassName, classLoader)
				End If
			Catch ignored As Exception
			Finally
				close(rd)
			End Try


			' try to read from $java.home/lib/jaxws.properties
			Dim inStream As FileInputStream = Nothing
			Try
				Dim javah As String=System.getProperty("java.home")
				Dim configFile As String = javah + File.separator & "lib" & File.separator & "jaxws.properties"
				Dim f As New File(configFile)
				If f.exists() Then
					Dim props As New java.util.Properties
					inStream = New FileInputStream(f)
					props.load(inStream)
					Dim factoryClassName As String = props.getProperty(factoryId)
					Return newInstance(factoryClassName, classLoader)
				End If
			Catch ignored As Exception
			Finally
				close(inStream)
			End Try

			' Use the system property
			Try
				Dim systemProp As String = System.getProperty(factoryId)
				If systemProp IsNot Nothing Then Return newInstance(systemProp, classLoader)
			Catch ignored As SecurityException
			End Try

			If fallbackClassName Is Nothing Then Throw New javax.xml.ws.WebServiceException("Provider for " & factoryId & " cannot be found", Nothing)

			Return newInstance(fallbackClassName, classLoader)
		End Function

		Private Shared Sub close(ByVal closeable As Closeable)
			If closeable IsNot Nothing Then
				Try
					closeable.close()
				Catch ignored As IOException
				End Try
			End If
		End Sub


		''' <summary>
		''' Loads the class, provided that the calling thread has an access to the class being loaded.
		''' </summary>
		Private Shared Function safeLoadClass(ByVal className As String, ByVal classLoader As ClassLoader) As Type
			Try
				' make sure that the current thread has an access to the package of the given name.
				Dim s As SecurityManager = System.securityManager
				If s IsNot Nothing Then
					Dim i As Integer = className.LastIndexOf("."c)
					If i <> -1 Then s.checkPackageAccess(className.Substring(0, i))
				End If

				If classLoader Is Nothing Then
					Return Type.GetType(className)
				Else
					Return classLoader.loadClass(className)
				End If
			Catch se As SecurityException
				' anyone can access the platform default factory class without permission
				If Provider.DEFAULT_JAXWSPROVIDER.Equals(className) Then Return Type.GetType(className)
				Throw se
			End Try
		End Function

		Private Const OSGI_SERVICE_LOADER_CLASS_NAME As String = "com.sun.org.glassfish.hk2.osgiresourcelocator.ServiceLoader"

		Private Property Shared osgi As Boolean
			Get
				Try
					Type.GetType(OSGI_SERVICE_LOADER_CLASS_NAME)
					Return True
				Catch ignored As ClassNotFoundException
				End Try
				Return False
			End Get
		End Property

		Private Shared Function lookupUsingOSGiServiceLoader(ByVal factoryId As String) As Object
			Try
				' Use reflection to avoid having any dependendcy on ServiceLoader class
				Dim serviceClass As Type = Type.GetType(factoryId)
				Dim args As Type() = {serviceClass}
				Dim target As Type = Type.GetType(OSGI_SERVICE_LOADER_CLASS_NAME)
				Dim m As System.Reflection.MethodInfo = target.GetMethod("lookupProviderInstances", GetType(Type))
				Dim iter As IEnumerator = CType(m.invoke(Nothing, CType(args, Object())), IEnumerable).GetEnumerator()
				Return If(iter.hasNext(), iter.next(), Nothing)
			Catch ignored As Exception
				' log and continue
				Return Nothing
			End Try
		End Function

	End Class

End Namespace
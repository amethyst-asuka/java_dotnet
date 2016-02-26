Imports System
Imports System.Threading

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

Namespace javax.xml.soap



	Friend Class FactoryFinder

		''' <summary>
		''' Creates an instance of the specified class using the specified
		''' <code>ClassLoader</code> object.
		''' </summary>
		''' <exception cref="SOAPException"> if the given class could not be found
		'''            or could not be instantiated </exception>
		Private Shared Function newInstance(ByVal className As String, ByVal classLoader As ClassLoader) As Object
			Try
				Dim spiClass As Type = safeLoadClass(className, classLoader)
				Return spiClass.newInstance()

			Catch x As ClassNotFoundException
				Throw New SOAPException("Provider " & className & " not found", x)
			Catch x As Exception
				Throw New SOAPException("Provider " & className & " could not be instantiated: " & x, x)
			End Try
		End Function

		''' <summary>
		''' Finds the implementation <code>Class</code> object for the given
		''' factory name, or null if that fails.
		''' <P>
		''' This method is package private so that this code can be shared.
		''' </summary>
		''' <returns> the <code>Class</code> object of the specified message factory;
		'''         or <code>null</code>
		''' </returns>
		''' <param name="factoryId">             the name of the factory to find, which is
		'''                              a system property </param>
		''' <exception cref="SOAPException"> if there is a SOAP error </exception>
		Friend Shared Function find(ByVal factoryId As String) As Object
			Return find(factoryId, Nothing, False)
		End Function

		''' <summary>
		''' Finds the implementation <code>Class</code> object for the given
		''' factory name, or if that fails, finds the <code>Class</code> object
		''' for the given fallback class name. The arguments supplied must be
		''' used in order. If using the first argument is successful, the second
		''' one will not be used.
		''' <P>
		''' This method is package private so that this code can be shared.
		''' </summary>
		''' <returns> the <code>Class</code> object of the specified message factory;
		'''         may be <code>null</code>
		''' </returns>
		''' <param name="factoryId">             the name of the factory to find, which is
		'''                              a system property </param>
		''' <param name="fallbackClassName">     the implementation class name, which is
		'''                              to be used only if nothing else
		'''                              is found; <code>null</code> to indicate that
		'''                              there is no fallback class name </param>
		''' <exception cref="SOAPException"> if there is a SOAP error </exception>
		Friend Shared Function find(ByVal factoryId As String, ByVal fallbackClassName As String) As Object
			Return find(factoryId, fallbackClassName, True)
		End Function

		''' <summary>
		''' Finds the implementation <code>Class</code> object for the given
		''' factory name, or if that fails, finds the <code>Class</code> object
		''' for the given default class name, but only if <code>tryFallback</code>
		''' is <code>true</code>.  The arguments supplied must be used in order
		''' If using the first argument is successful, the second one will not
		''' be used.  Note the default class name may be needed even if fallback
		''' is not to be attempted, so certain error conditions can be handled.
		''' <P>
		''' This method is package private so that this code can be shared.
		''' </summary>
		''' <returns> the <code>Class</code> object of the specified message factory;
		'''         may not be <code>null</code>
		''' </returns>
		''' <param name="factoryId">             the name of the factory to find, which is
		'''                              a system property </param>
		''' <param name="defaultClassName">      the implementation class name, which is
		'''                              to be used only if nothing else
		'''                              is found; <code>null</code> to indicate
		'''                              that there is no default class name </param>
		''' <param name="tryFallback">           whether to try the default class as a
		'''                              fallback </param>
		''' <exception cref="SOAPException"> if there is a SOAP error </exception>
		Friend Shared Function find(ByVal factoryId As String, ByVal defaultClassName As String, ByVal tryFallback As Boolean) As Object
			Dim classLoader As ClassLoader
			Try
				classLoader = Thread.CurrentThread.contextClassLoader
			Catch x As Exception
				Throw New SOAPException(x.ToString(), x)
			End Try

			' Use the system property first
			Try
				Dim systemProp As String = System.getProperty(factoryId)
				If systemProp IsNot Nothing Then Return newInstance(systemProp, classLoader)
			Catch se As SecurityException
			End Try

			' try to read from $java.home/lib/jaxm.properties
			Try
				Dim javah As String=System.getProperty("java.home")
				Dim configFile As String = javah + File.separator & "lib" & File.separator & "jaxm.properties"
				Dim f As New File(configFile)
				If f.exists() Then
					Dim props As New java.util.Properties
					props.load(New FileInputStream(f))
					Dim factoryClassName As String = props.getProperty(factoryId)
					Return newInstance(factoryClassName, classLoader)
				End If
			Catch ex As Exception
			End Try

			Dim serviceId As String = "META-INF/services/" & factoryId
			' try to find services in CLASSPATH
			Try
				Dim [is] As InputStream=Nothing
				If classLoader Is Nothing Then
					[is]=ClassLoader.getSystemResourceAsStream(serviceId)
				Else
					[is]=classLoader.getResourceAsStream(serviceId)
				End If

				If [is] IsNot Nothing Then
					Dim rd As New BufferedReader(New InputStreamReader([is], "UTF-8"))

					Dim factoryClassName As String = rd.readLine()
					rd.close()

					If factoryClassName IsNot Nothing AndAlso (Not "".Equals(factoryClassName)) Then Return newInstance(factoryClassName, classLoader)
				End If
			Catch ex As Exception
			End Try

			' If not found and fallback should not be tried, return a null result.
			If Not tryFallback Then Return Nothing

			' We didn't find the class through the usual means so try the default
			' (built in) factory if specified.
			If defaultClassName Is Nothing Then Throw New SOAPException("Provider for " & factoryId & " cannot be found", Nothing)
			Return newInstance(defaultClassName, classLoader)
		End Function

		''' <summary>
		''' Loads the class, provided that the calling thread has an access to the
		''' class being loaded. If this is the specified default factory class and it
		''' is restricted by package.access we get a SecurityException and can do a
		''' Class.forName() on it so it will be loaded by the bootstrap class loader.
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
				' (only) default implementation can be loaded
				' using bootstrap class loader:
				If isDefaultImplementation(className) Then Return Type.GetType(className)

				Throw se
			End Try
		End Function

		Private Shared Function isDefaultImplementation(ByVal className As String) As Boolean
			Return MessageFactory.DEFAULT_MESSAGE_FACTORY.Equals(className) OrElse SOAPFactory.DEFAULT_SOAP_FACTORY.Equals(className) OrElse SOAPConnectionFactory.DEFAULT_SOAP_CONNECTION_FACTORY.Equals(className) OrElse SAAJMetaFactory.DEFAULT_META_FACTORY_CLASS.Equals(className)
		End Function
	End Class

End Namespace
Imports System
Imports System.Collections
Imports System.Text
Imports System.Threading
import static javax.xml.bind.JAXBContext.JAXB_CONTEXT_FACTORY

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

Namespace javax.xml.bind



	''' <summary>
	''' This class is package private and therefore is not exposed as part of the
	''' JAXB API.
	''' 
	''' This code is designed to implement the JAXB 1.0 spec pluggability feature
	''' 
	''' @author <ul><li>Ryan Shoemaker, Sun Microsystems, Inc.</li></ul> </summary>
	''' <seealso cref= JAXBContext </seealso>
	Friend Class ContextFinder
		Private Shared ReadOnly logger As java.util.logging.Logger
		Shared Sub New()
			logger = java.util.logging.Logger.getLogger("javax.xml.bind")
			Try
				If java.security.AccessController.doPrivileged(New GetPropertyAction("jaxb.debug")) IsNot Nothing Then
					' disconnect the logger from a bigger framework (if any)
					' and take the matters into our own hands
					logger.useParentHandlers = False
					logger.level = java.util.logging.Level.ALL
					Dim handler As New java.util.logging.ConsoleHandler
					handler.level = java.util.logging.Level.ALL
					logger.addHandler(handler)
				Else
					' don't change the setting of this logger
					' to honor what other frameworks
					' have done on configurations.
				End If
			Catch t As Exception
				' just to be extra safe. in particular System.getProperty may throw
				' SecurityException.
			End Try
		End Sub

		''' <summary>
		''' If the <seealso cref="InvocationTargetException"/> wraps an exception that shouldn't be wrapped,
		''' throw the wrapped exception.
		''' </summary>
		Private Shared Sub handleInvocationTargetException(ByVal x As InvocationTargetException)
			Dim t As Exception = x.targetException
			If t IsNot Nothing Then
				If TypeOf t Is JAXBException Then Throw CType(t, JAXBException)
				If TypeOf t Is Exception Then Throw CType(t, Exception)
				If TypeOf t Is Exception Then Throw CType(t, [Error])
			End If
		End Sub


		''' <summary>
		''' Determine if two types (JAXBContext in this case) will generate a ClassCastException.
		''' 
		''' For example, (targetType)originalType
		''' </summary>
		''' <param name="originalType">
		'''          The Class object of the type being cast </param>
		''' <param name="targetType">
		'''          The Class object of the type that is being cast to </param>
		''' <returns> JAXBException to be thrown. </returns>
		Private Shared Function handleClassCastException(ByVal originalType As Type, ByVal targetType As Type) As JAXBException
			Dim targetTypeURL As java.net.URL = which(targetType)

			Return New JAXBException(Messages.format(Messages.ILLEGAL_CAST, getClassClassLoader(originalType).getResource("javax/xml/bind/JAXBContext.class"), targetTypeURL))
					' we don't care where the impl class is, we want to know where JAXBContext lives in the impl
					' class' ClassLoader
		End Function

		''' <summary>
		''' Create an instance of a class using the specified ClassLoader
		''' </summary>
		Friend Shared Function newInstance(ByVal contextPath As String, ByVal className As String, ByVal classLoader As ClassLoader, ByVal properties As IDictionary) As JAXBContext
			Try
				Dim spFactory As Type = safeLoadClass(className,classLoader)
				Return newInstance(contextPath, spFactory, classLoader, properties)
			Catch x As ClassNotFoundException
				Throw New JAXBException(Messages.format(Messages.PROVIDER_NOT_FOUND, className), x)
			Catch x As Exception
				' avoid wrapping RuntimeException to JAXBException,
				' because it indicates a bug in this code.
				Throw x
			Catch x As Exception
				' can't catch JAXBException because the method is hidden behind
				' reflection.  Root element collisions detected in the call to
				' createContext() are reported as JAXBExceptions - just re-throw it
				' some other type of exception - just wrap it
				Throw New JAXBException(Messages.format(Messages.COULD_NOT_INSTANTIATE, className, x), x)
			End Try
		End Function

		Friend Shared Function newInstance(ByVal contextPath As String, ByVal spFactory As Type, ByVal classLoader As ClassLoader, ByVal properties As IDictionary) As JAXBContext
			Try
	'            
	'             * javax.xml.bind.context.factory points to a class which has a
	'             * static method called 'createContext' that
	'             * returns a javax.xml.JAXBContext.
	'             

				Dim context As Object = Nothing

				' first check the method that takes Map as the third parameter.
				' this is added in 2.0.
				Try
					Dim m As Method = spFactory.GetMethod("createContext",GetType(String),GetType(ClassLoader),GetType(IDictionary))
					' any failure in invoking this method would be considered fatal
					context = m.invoke(Nothing,contextPath,classLoader,properties)
				Catch e As NoSuchMethodException
					' it's not an error for the provider not to have this method.
				End Try

				If context Is Nothing Then
					' try the old method that doesn't take properties. compatible with 1.0.
					' it is an error for an implementation not to have both forms of the createContext method.
					Dim m As Method = spFactory.GetMethod("createContext",GetType(String),GetType(ClassLoader))
					' any failure in invoking this method would be considered fatal
					context = m.invoke(Nothing,contextPath,classLoader)
				End If

				If Not(TypeOf context Is JAXBContext) Then Throw handleClassCastException(context.GetType(), GetType(JAXBContext))
				Return CType(context, JAXBContext)
			Catch x As InvocationTargetException
				handleInvocationTargetException(x)
				' for other exceptions, wrap the internal target exception
				' with a JAXBException
				Dim e As Exception = x
				If x.targetException IsNot Nothing Then e = x.targetException

				Throw New JAXBException(Messages.format(Messages.COULD_NOT_INSTANTIATE, spFactory, e), e)
			Catch x As Exception
				' avoid wrapping RuntimeException to JAXBException,
				' because it indicates a bug in this code.
				Throw x
			Catch x As Exception
				' can't catch JAXBException because the method is hidden behind
				' reflection.  Root element collisions detected in the call to
				' createContext() are reported as JAXBExceptions - just re-throw it
				' some other type of exception - just wrap it
				Throw New JAXBException(Messages.format(Messages.COULD_NOT_INSTANTIATE, spFactory, x), x)
			End Try
		End Function


		''' <summary>
		''' Create an instance of a class using the thread context ClassLoader
		''' </summary>
		Friend Shared Function newInstance(ByVal classes As Type(), ByVal properties As IDictionary, ByVal className As String) As JAXBContext
			Dim cl As ClassLoader = contextClassLoader
			Dim spi As Type
			Try
				spi = safeLoadClass(className,cl)
			Catch e As ClassNotFoundException
				Throw New JAXBException(e)
			End Try

			If logger.isLoggable(java.util.logging.Level.FINE) Then logger.log(java.util.logging.Level.FINE, "loaded {0} from {1}", New Object(){className, which(spi)})

			Return newInstance(classes, properties, spi)
		End Function

		Friend Shared Function newInstance(ByVal classes As Type(), ByVal properties As IDictionary, ByVal spFactory As Type) As JAXBContext
			Dim m As Method
			Try
				m = spFactory.GetMethod("createContext", GetType(Type()), GetType(IDictionary))
			Catch e As NoSuchMethodException
				Throw New JAXBException(e)
			End Try
			Try
				Dim context As Object = m.invoke(Nothing, classes, properties)
				If Not(TypeOf context Is JAXBContext) Then Throw handleClassCastException(context.GetType(), GetType(JAXBContext))
				Return CType(context, JAXBContext)
			Catch e As IllegalAccessException
				Throw New JAXBException(e)
			Catch e As InvocationTargetException
				handleInvocationTargetException(e)

				Dim x As Exception = e
				If e.targetException IsNot Nothing Then x = e.targetException

				Throw New JAXBException(x)
			End Try
		End Function

		Friend Shared Function find(ByVal factoryId As String, ByVal contextPath As String, ByVal classLoader As ClassLoader, ByVal properties As IDictionary) As JAXBContext

			' TODO: do we want/need another layer of searching in $java.home/lib/jaxb.properties like JAXP?

			Dim jaxbContextFQCN As String = GetType(JAXBContext).name

			' search context path for jaxb.properties first
			Dim propFileName As StringBuilder
			Dim packages As New java.util.StringTokenizer(contextPath, ":")
			Dim factoryClassName As String

			If Not packages.hasMoreTokens() Then Throw New JAXBException(Messages.format(Messages.NO_PACKAGE_IN_CONTEXTPATH))


			logger.fine("Searching jaxb.properties")

			Do While packages.hasMoreTokens()
				Dim packageName As String = packages.nextToken(":").replace("."c,"/"c)
				' com.acme.foo - > com/acme/foo/jaxb.properties
				 propFileName = (New StringBuilder).Append(packageName).append("/jaxb.properties")

				Dim props As java.util.Properties = loadJAXBProperties(classLoader, propFileName.ToString())
				If props IsNot Nothing Then
					If props.containsKey(factoryId) Then
						factoryClassName = props.getProperty(factoryId)
						Return newInstance(contextPath, factoryClassName, classLoader, properties)
					Else
						Throw New JAXBException(Messages.format(Messages.MISSING_PROPERTY, packageName, factoryId))
					End If
				End If
			Loop

			logger.fine("Searching the system property")

			' search for a system property second (javax.xml.bind.JAXBContext)
			factoryClassName = java.security.AccessController.doPrivileged(New GetPropertyAction(JAXBContext.JAXB_CONTEXT_FACTORY))
			If factoryClassName IsNot Nothing Then
				Return newInstance(contextPath, factoryClassName, classLoader, properties) ' leave this here to assure compatibility
			Else
				factoryClassName = java.security.AccessController.doPrivileged(New GetPropertyAction(jaxbContextFQCN))
				If factoryClassName IsNot Nothing Then Return newInstance(contextPath, factoryClassName, classLoader, properties)
			End If

			' OSGi search
			Dim jaxbContext As Type = lookupJaxbContextUsingOsgiServiceLoader()
			If jaxbContext IsNot Nothing Then
				logger.fine("OSGi environment detected")
				Return newInstance(contextPath, jaxbContext, classLoader, properties)
			End If

			logger.fine("Searching META-INF/services")
			' search META-INF services next
			Dim r As java.io.BufferedReader = Nothing
			Try
				Dim resource As (New StringBuilder).Append("META-INF/services/").append(jaxbContextFQCN)
				Dim resourceStream As java.io.InputStream = classLoader.getResourceAsStream(resource.ToString())

				If resourceStream IsNot Nothing Then
					r = New java.io.BufferedReader(New java.io.InputStreamReader(resourceStream, "UTF-8"))
					factoryClassName = r.readLine()
					If factoryClassName IsNot Nothing Then factoryClassName = factoryClassName.Trim()
					r.close()
					Return newInstance(contextPath, factoryClassName, classLoader, properties)
				Else
					logger.log(java.util.logging.Level.FINE, "Unable to load:{0}", resource.ToString())
				End If
			Catch e As java.io.UnsupportedEncodingException
				' should never happen
				Throw New JAXBException(e)
			Catch e As java.io.IOException
				Throw New JAXBException(e)
			Finally
				Try
					If r IsNot Nothing Then r.close()
				Catch ex As java.io.IOException
					java.util.logging.Logger.getLogger(GetType(ContextFinder).name).log(java.util.logging.Level.SEVERE, Nothing, ex)
				End Try
			End Try

			' else no provider found
			logger.fine("Trying to create the platform default provider")
			Return newInstance(contextPath, PLATFORM_DEFAULT_FACTORY_CLASS, classLoader, properties)
		End Function

		Friend Shared Function find(ByVal classes As Type(), ByVal properties As IDictionary) As JAXBContext

			Dim jaxbContextFQCN As String = GetType(JAXBContext).name
			Dim factoryClassName As String

			' search for jaxb.properties in the class loader of each class first
			For Each c As Type In classes
				' this classloader is used only to load jaxb.properties, so doing this should be safe.
				Dim classLoader As ClassLoader = getClassClassLoader(c)
				Dim pkg As Package = c.Assembly
				If pkg Is Nothing Then Continue For ' this is possible for primitives, arrays, and classes that are loaded by poorly implemented ClassLoaders
				Dim packageName As String = pkg.name.Replace("."c, "/"c)

				' TODO: do we want to optimize away searching the same package?  org.Foo, org.Bar, com.Baz
				'       classes from the same package might come from different class loades, so it might be a bad idea

				' TODO: it's easier to look things up from the class
				' c.getResourceAsStream("jaxb.properties");

				' build the resource name and use the property loader code
				Dim resourceName As String = packageName & "/jaxb.properties"
				logger.log(java.util.logging.Level.FINE, "Trying to locate {0}", resourceName)
				Dim props As java.util.Properties = loadJAXBProperties(classLoader, resourceName)
				If props Is Nothing Then
					logger.fine("  not found")
				Else
					logger.fine("  found")
					If props.containsKey(JAXB_CONTEXT_FACTORY) Then
						' trim() seems redundant, but adding to satisfy customer complaint
						factoryClassName = props.getProperty(JAXB_CONTEXT_FACTORY).Trim()
						Return newInstance(classes, properties, factoryClassName)
					Else
						Throw New JAXBException(Messages.format(Messages.MISSING_PROPERTY, packageName, JAXB_CONTEXT_FACTORY))
					End If
				End If
			Next c

			' search for a system property second (javax.xml.bind.JAXBContext)
			logger.log(java.util.logging.Level.FINE, "Checking system property {0}", JAXBContext.JAXB_CONTEXT_FACTORY)
			factoryClassName = java.security.AccessController.doPrivileged(New GetPropertyAction(JAXBContext.JAXB_CONTEXT_FACTORY))
			If factoryClassName IsNot Nothing Then
				logger.log(java.util.logging.Level.FINE, "  found {0}", factoryClassName)
				Return newInstance(classes, properties, factoryClassName) ' leave it here for compatibility reasons
			Else
				logger.fine("  not found")
				logger.log(java.util.logging.Level.FINE, "Checking system property {0}", jaxbContextFQCN)
				factoryClassName = java.security.AccessController.doPrivileged(New GetPropertyAction(jaxbContextFQCN))
				If factoryClassName IsNot Nothing Then
					logger.log(java.util.logging.Level.FINE, "  found {0}", factoryClassName)
					Return newInstance(classes, properties, factoryClassName)
				Else
					logger.fine("  not found")
				End If
			End If

			' OSGi search
			Dim jaxbContext As Type = lookupJaxbContextUsingOsgiServiceLoader()
			If jaxbContext IsNot Nothing Then
				logger.fine("OSGi environment detected")
				Return newInstance(classes, properties, jaxbContext)
			End If

			' search META-INF services next
			logger.fine("Checking META-INF/services")
			Dim r As java.io.BufferedReader = Nothing
			Try
				Dim resource As String = (New StringBuilder("META-INF/services/")).Append(jaxbContextFQCN).ToString()
				Dim classLoader As ClassLoader = contextClassLoader
				Dim resourceURL As java.net.URL
				If classLoader Is Nothing Then
					resourceURL = ClassLoader.getSystemResource(resource)
				Else
					resourceURL = classLoader.getResource(resource)
				End If

				If resourceURL IsNot Nothing Then
					logger.log(java.util.logging.Level.FINE, "Reading {0}", resourceURL)
					r = New java.io.BufferedReader(New java.io.InputStreamReader(resourceURL.openStream(), "UTF-8"))
					factoryClassName = r.readLine()
					If factoryClassName IsNot Nothing Then factoryClassName = factoryClassName.Trim()
					Return newInstance(classes, properties, factoryClassName)
				Else
					logger.log(java.util.logging.Level.FINE, "Unable to find: {0}", resource)
				End If
			Catch e As java.io.UnsupportedEncodingException
				' should never happen
				Throw New JAXBException(e)
			Catch e As java.io.IOException
				Throw New JAXBException(e)
			Finally
				If r IsNot Nothing Then
					Try
						r.close()
					Catch ex As java.io.IOException
						logger.log(java.util.logging.Level.FINE, "Unable to close stream", ex)
					End Try
				End If
			End Try

			' else no provider found
			logger.fine("Trying to create the platform default provider")
			Return newInstance(classes, properties, PLATFORM_DEFAULT_FACTORY_CLASS)
		End Function

		Private Shared Function lookupJaxbContextUsingOsgiServiceLoader() As Type
			Try
				' Use reflection to avoid having any dependency on ServiceLoader class
				Dim target As Type = Type.GetType("com.sun.org.glassfish.hk2.osgiresourcelocator.ServiceLoader")
				Dim m As Method = target.GetMethod("lookupProviderClasses", GetType(Type))
				Dim iter As IEnumerator = CType(m.invoke(Nothing, GetType(JAXBContext)), IEnumerable).GetEnumerator()
				Return If(iter.hasNext(), CType(iter.next(), [Class]), Nothing)
			Catch e As Exception
				logger.log(java.util.logging.Level.FINE, "Unable to find from OSGi: javax.xml.bind.JAXBContext")
				Return Nothing
			End Try
		End Function

		Private Shared Function loadJAXBProperties(ByVal classLoader As ClassLoader, ByVal propFileName As String) As java.util.Properties

			Dim props As java.util.Properties = Nothing

			Try
				Dim url As java.net.URL
				If classLoader Is Nothing Then
					url = ClassLoader.getSystemResource(propFileName)
				Else
					url = classLoader.getResource(propFileName)
				End If

				If url IsNot Nothing Then
					logger.log(java.util.logging.Level.FINE, "loading props from {0}", url)
					props = New java.util.Properties
					Dim [is] As java.io.InputStream = url.openStream()
					props.load([is])
					[is].close()
				End If
			Catch ioe As java.io.IOException
				logger.log(java.util.logging.Level.FINE,"Unable to load " & propFileName,ioe)
				Throw New JAXBException(ioe.ToString(), ioe)
			End Try

			Return props
		End Function


		''' <summary>
		''' Search the given ClassLoader for an instance of the specified class and
		''' return a string representation of the URL that points to the resource.
		''' </summary>
		''' <param name="clazz">
		'''          The class to search for </param>
		''' <param name="loader">
		'''          The ClassLoader to search.  If this parameter is null, then the
		'''          system class loader will be searched
		''' @return
		'''          the URL for the class or null if it wasn't found </param>
		Friend Shared Function which(ByVal clazz As Type, ByVal loader As ClassLoader) As java.net.URL

			Dim classnameAsResource As String = clazz.name.Replace("."c, "/"c) & ".class"

			If loader Is Nothing Then loader = systemClassLoader

			Return loader.getResource(classnameAsResource)
		End Function

		''' <summary>
		''' Get the URL for the Class from it's ClassLoader.
		''' 
		''' Convenience method for <seealso cref="#which(Class, ClassLoader)"/>.
		''' 
		''' Equivalent to calling: which(clazz, clazz.getClassLoader())
		''' </summary>
		''' <param name="clazz">
		'''          The class to search for
		''' @return
		'''          the URL for the class or null if it wasn't found </param>
		Friend Shared Function which(ByVal clazz As Type) As java.net.URL
			Return which(clazz, getClassClassLoader(clazz))
		End Function

		''' <summary>
		''' When JAXB is in J2SE, rt.jar has to have a JAXB implementation.
		''' However, rt.jar cannot have META-INF/services/javax.xml.bind.JAXBContext
		''' because if it has, it will take precedence over any file that applications have
		''' in their jar files.
		''' 
		''' <p>
		''' When the user bundles his own JAXB implementation, we'd like to use it, and we
		''' want the platform default to be used only when there's no other JAXB provider.
		''' 
		''' <p>
		''' For this reason, we have to hard-code the class name into the API.
		''' </summary>
		Private Const PLATFORM_DEFAULT_FACTORY_CLASS As String = "com.sun.xml.internal.bind.v2.ContextFactory"

		''' <summary>
		''' Loads the class, provided that the calling thread has an access to the class being loaded.
		''' </summary>
		Private Shared Function safeLoadClass(ByVal className As String, ByVal classLoader As ClassLoader) As Type
		   logger.log(java.util.logging.Level.FINE, "Trying to load {0}", className)
		   Try
			  ' make sure that the current thread has an access to the package of the given name.
			  Dim s As SecurityManager = System.securityManager
			  If s IsNot Nothing Then
				  Dim i As Integer = className.LastIndexOf("."c)
				  If i <> -1 Then s.checkPackageAccess(className.Substring(0,i))
			  End If

			  If classLoader Is Nothing Then
				  Return Type.GetType(className)
			  Else
				  Return classLoader.loadClass(className)
			  End If
		   Catch se As SecurityException
			   ' anyone can access the platform default factory class without permission
			   If PLATFORM_DEFAULT_FACTORY_CLASS.Equals(className) Then Return Type.GetType(className)
			   Throw se
		   End Try
		End Function

		Private Property Shared contextClassLoader As ClassLoader
			Get
				If System.securityManager Is Nothing Then
					Return Thread.CurrentThread.contextClassLoader
				Else
	'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
	'				Return (ClassLoader) java.security.AccessController.doPrivileged(New java.security.PrivilegedAction()
		'			{
		'						public java.lang.Object run()
		'						{
		'							Return Thread.currentThread().getContextClassLoader();
		'						}
		'					});
				End If
			End Get
		End Property

		Private Shared Function getClassClassLoader(ByVal c As Type) As ClassLoader
			If System.securityManager Is Nothing Then
				Return c.classLoader
			Else
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return (ClassLoader) java.security.AccessController.doPrivileged(New java.security.PrivilegedAction()
	'			{
	'						public java.lang.Object run()
	'						{
	'							Return c.getClassLoader();
	'						}
	'					});
			End If
		End Function

		Private Property Shared systemClassLoader As ClassLoader
			Get
				If System.securityManager Is Nothing Then
					Return ClassLoader.systemClassLoader
				Else
	'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
	'				Return (ClassLoader) java.security.AccessController.doPrivileged(New java.security.PrivilegedAction()
		'			{
		'						public java.lang.Object run()
		'						{
		'							Return ClassLoader.getSystemClassLoader();
		'						}
		'					});
				End If
			End Get
		End Property

	End Class

End Namespace
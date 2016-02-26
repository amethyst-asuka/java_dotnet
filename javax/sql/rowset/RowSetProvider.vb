Imports System

'
' * Copyright (c) 2010, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.sql.rowset


	''' <summary>
	''' A factory API that enables applications to obtain a
	''' {@code RowSetFactory} implementation  that can be used to create different
	''' types of {@code RowSet} implementations.
	''' <p>
	''' Example:
	''' </p>
	''' <pre>
	''' RowSetFactory aFactory = RowSetProvider.newFactory();
	''' CachedRowSet crs = aFactory.createCachedRowSet();
	''' ...
	''' RowSetFactory rsf = RowSetProvider.newFactory("com.sun.rowset.RowSetFactoryImpl", null);
	''' WebRowSet wrs = rsf.createWebRowSet();
	''' </pre>
	''' <p>
	''' Tracing of this class may be enabled by setting the System property
	''' {@code javax.sql.rowset.RowSetFactory.debug} to any value but {@code false}.
	''' </p>
	''' 
	''' @author Lance Andersen
	''' @since 1.7
	''' </summary>
	Public Class RowSetProvider

		Private Const ROWSET_DEBUG_PROPERTY As String = "javax.sql.rowset.RowSetProvider.debug"
		Private Const ROWSET_FACTORY_IMPL As String = "com.sun.rowset.RowSetFactoryImpl"
		Private Const ROWSET_FACTORY_NAME As String = "javax.sql.rowset.RowSetFactory"
		''' <summary>
		''' Internal debug flag.
		''' </summary>
		Private Shared debug As Boolean = True


		Shared Sub New()
			' Check to see if the debug property is set
			Dim val As String = getSystemProperty(ROWSET_DEBUG_PROPERTY)
			' Allow simply setting the prop to turn on debug
			debug = val IsNot Nothing AndAlso Not "false".Equals(val)
		End Sub

		''' <summary>
		''' RowSetProvider constructor
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' <p>Creates a new instance of a <code>RowSetFactory</code>
		''' implementation.  This method uses the following
		''' look up order to determine
		''' the <code>RowSetFactory</code> implementation class to load:</p>
		''' <ul>
		''' <li>
		''' The System property {@code javax.sql.rowset.RowSetFactory}.  For example:
		''' <ul>
		''' <li>
		''' -Djavax.sql.rowset.RowSetFactory=com.sun.rowset.RowSetFactoryImpl
		''' </li>
		''' </ul>
		''' <li>
		''' The <seealso cref="ServiceLoader"/> API. The {@code ServiceLoader} API will look
		''' for a class name in the file
		''' {@code META-INF/services/javax.sql.rowset.RowSetFactory}
		''' in jars available to the runtime. For example, to have the the RowSetFactory
		''' implementation {@code com.sun.rowset.RowSetFactoryImpl } loaded, the
		''' entry in {@code META-INF/services/javax.sql.rowset.RowSetFactory} would be:
		'''  <ul>
		''' <li>
		''' {@code com.sun.rowset.RowSetFactoryImpl }
		''' </li>
		''' </ul>
		''' </li>
		''' <li>
		''' Platform default <code>RowSetFactory</code> instance.
		''' </li>
		''' </ul>
		''' 
		''' <p>Once an application has obtained a reference to a {@code RowSetFactory},
		''' it can use the factory to obtain RowSet instances.</p>
		''' </summary>
		''' <returns> New instance of a <code>RowSetFactory</code>
		''' </returns>
		''' <exception cref="SQLException"> if the default factory class cannot be loaded,
		''' instantiated. The cause will be set to actual Exception
		''' </exception>
		''' <seealso cref= ServiceLoader
		''' @since 1.7 </seealso>
		Public Shared Function newFactory() As RowSetFactory
			' Use the system property first
			Dim factory As RowSetFactory = Nothing
			Dim factoryClassName As String = Nothing
			Try
				trace("Checking for Rowset System Property...")
				factoryClassName = getSystemProperty(ROWSET_FACTORY_NAME)
				If factoryClassName IsNot Nothing Then
					trace("Found system property, value=" & factoryClassName)
					factory = CType(sun.reflect.misc.ReflectUtil.newInstance(getFactoryClass(factoryClassName, Nothing, True)), RowSetFactory)
				End If
			Catch e As Exception
				Throw New java.sql.SQLException("RowSetFactory: " & factoryClassName & " could not be instantiated: ", e)
			End Try

			' Check to see if we found the RowSetFactory via a System property
			If factory Is Nothing Then
				' If the RowSetFactory is not found via a System Property, now
				' look it up via the ServiceLoader API and if not found, use the
				' Java SE default.
				factory = loadViaServiceLoader()
				factory = If(factory Is Nothing, newFactory(ROWSET_FACTORY_IMPL, Nothing), factory)
			End If
			Return (factory)
		End Function

		''' <summary>
		''' <p>Creates  a new instance of a <code>RowSetFactory</code> from the
		''' specified factory class name.
		''' This function is useful when there are multiple providers in the classpath.
		''' It gives more control to the application as it can specify which provider
		''' should be loaded.</p>
		''' 
		''' <p>Once an application has obtained a reference to a <code>RowSetFactory</code>
		''' it can use the factory to obtain RowSet instances.</p>
		''' </summary>
		''' <param name="factoryClassName"> fully qualified factory class name that
		''' provides  an implementation of <code>javax.sql.rowset.RowSetFactory</code>.
		''' </param>
		''' <param name="cl"> <code>ClassLoader</code> used to load the factory
		''' class. If <code>null</code> current <code>Thread</code>'s context
		''' classLoader is used to load the factory class.
		''' </param>
		''' <returns> New instance of a <code>RowSetFactory</code>
		''' </returns>
		''' <exception cref="SQLException"> if <code>factoryClassName</code> is
		''' <code>null</code>, or the factory class cannot be loaded, instantiated.
		''' </exception>
		''' <seealso cref= #newFactory()
		''' 
		''' @since 1.7 </seealso>
		Public Shared Function newFactory(ByVal factoryClassName As String, ByVal cl As ClassLoader) As RowSetFactory

			trace("***In newInstance()")

			If factoryClassName Is Nothing Then Throw New java.sql.SQLException("Error: factoryClassName cannot be null")
			Try
				sun.reflect.misc.ReflectUtil.checkPackageAccess(factoryClassName)
			Catch e As java.security.AccessControlException
				Throw New java.sql.SQLException("Access Exception",e)
			End Try

			Try
				Dim providerClass As Type = getFactoryClass(factoryClassName, cl, False)
				Dim instance As RowSetFactory = CType(providerClass.newInstance(), RowSetFactory)
				If debug Then trace("Created new instance of " & providerClass & " using ClassLoader: " & cl)
				Return instance
			Catch x As ClassNotFoundException
				Throw New java.sql.SQLException("Provider " & factoryClassName & " not found", x)
			Catch x As Exception
				Throw New java.sql.SQLException("Provider " & factoryClassName & " could not be instantiated: " & x, x)
			End Try
		End Function

	'    
	'     * Returns the class loader to be used.
	'     * @return The ClassLoader to use.
	'     *
	'     
		Private Property Shared contextClassLoader As ClassLoader
			Get
	'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
	'			Return java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<ClassLoader>()
		'		{
		'
		'			public ClassLoader run()
		'			{
		'				ClassLoader cl = Nothing;
		'
		'				cl = Thread.currentThread().getContextClassLoader();
		'
		'				if (cl == Nothing)
		'				{
		'					cl = ClassLoader.getSystemClassLoader();
		'				}
		'
		'				Return cl;
		'			}
		'		});
			End Get
		End Property

		''' <summary>
		''' Attempt to load a class using the class loader supplied. If that fails
		''' and fall back is enabled, the current (i.e. bootstrap) class loader is
		''' tried.
		''' 
		''' If the class loader supplied is <code>null</code>, first try using the
		''' context class loader followed by the current class loader. </summary>
		'''  <returns> The class which was loaded </returns>
		Private Shared Function getFactoryClass(ByVal factoryClassName As String, ByVal cl As ClassLoader, ByVal doFallback As Boolean) As Type
			Try
				If cl Is Nothing Then
					cl = contextClassLoader
					If cl Is Nothing Then
						Throw New ClassNotFoundException
					Else
						Return cl.loadClass(factoryClassName)
					End If
				Else
					Return cl.loadClass(factoryClassName)
				End If
			Catch e As ClassNotFoundException
				If doFallback Then
					' Use current class loader
					Return Type.GetType(factoryClassName, True, GetType(RowSetFactory).classLoader)
				Else
					Throw e
				End If
			End Try
		End Function

		''' <summary>
		''' Use the ServiceLoader mechanism to load  the default RowSetFactory </summary>
		''' <returns> default RowSetFactory Implementation </returns>
		Private Shared Function loadViaServiceLoader() As RowSetFactory
			Dim theFactory As RowSetFactory = Nothing
			Try
				trace("***in loadViaServiceLoader():")
				For Each factory As RowSetFactory In java.util.ServiceLoader.load(GetType(javax.sql.rowset.RowSetFactory))
					trace(" Loading done by the java.util.ServiceLoader :" & factory.GetType().name)
					theFactory = factory
					Exit For
				Next factory
			Catch e As java.util.ServiceConfigurationError
				Throw New java.sql.SQLException("RowSetFactory: Error locating RowSetFactory using Service " & "Loader API: " & e, e)
			End Try
			Return theFactory

		End Function

		''' <summary>
		''' Returns the requested System Property.  If a {@code SecurityException}
		''' occurs, just return NULL </summary>
		''' <param name="propName"> - System property to retrieve </param>
		''' <returns> The System property value or NULL if the property does not exist
		''' or a {@code SecurityException} occurs. </returns>
		Private Shared Function getSystemProperty(ByVal propName As String) As String
			Dim [property] As String = Nothing
			Try
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				property = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<String>()
	'			{
	'
	'				public String run()
	'				{
	'					Return System.getProperty(propName);
	'				}
	'			}, Nothing, New PropertyPermission(propName, "read"));
			Catch se As SecurityException
				trace("error getting " & propName & ":  " & se)
				If debug Then
					Console.WriteLine(se.ToString())
					Console.Write(se.StackTrace)
				End If
			End Try
			Return [property]
		End Function

		''' <summary>
		''' Debug routine which will output tracing if the System Property
		''' -Djavax.sql.rowset.RowSetFactory.debug is set </summary>
		''' <param name="msg"> - The debug message to display </param>
		Private Shared Sub trace(ByVal msg As String)
			If debug Then Console.Error.WriteLine("###RowSets: " & msg)
		End Sub
	End Class

End Namespace
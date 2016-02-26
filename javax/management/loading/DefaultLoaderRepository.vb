Imports System
Imports System.Collections.Generic
import static com.sun.jmx.defaults.JmxProperties.MBEANSERVER_LOGGER

'
' * Copyright (c) 2000, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management.loading


	''' <summary>
	''' <p>Keeps the list of Class Loaders registered in the MBean Server.
	''' It provides the necessary methods to load classes using the registered
	''' Class Loaders.</p>
	''' 
	''' <p>This deprecated class is maintained for compatibility.  In
	''' previous versions of JMX, there was one
	''' <code>DefaultLoaderRepository</code> shared by all MBean servers.
	''' As of JMX 1.2, that functionality is approximated by using {@link
	''' MBeanServerFactory#findMBeanServer} to find all known MBean
	''' servers, and consulting the <seealso cref="ClassLoaderRepository"/> of each
	''' one.  It is strongly recommended that code referencing
	''' <code>DefaultLoaderRepository</code> be rewritten.</p>
	''' </summary>
	''' @deprecated Use
	''' <seealso cref="javax.management.MBeanServer#getClassLoaderRepository()"/>}
	''' instead.
	''' 
	''' @since 1.5 
	<Obsolete("Use")> _
	Public Class DefaultLoaderRepository

		''' <summary>
		''' Go through the list of class loaders and try to load the requested
		''' class.
		''' The method will stop as soon as the class is found. If the class
		''' is not found the method will throw a <CODE>ClassNotFoundException</CODE>
		''' exception.
		''' </summary>
		''' <param name="className"> The name of the class to be loaded.
		''' </param>
		''' <returns> the loaded class.
		''' </returns>
		''' <exception cref="ClassNotFoundException"> The specified class could not be
		'''            found. </exception>
		Public Shared Function loadClass(ByVal className As String) As Type
			MBEANSERVER_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DefaultLoaderRepository).name, "loadClass", className)
			Return load(Nothing, className)
		End Function

		''' <summary>
		''' Go through the list of class loaders but exclude the given
		''' class loader, then try to load
		''' the requested class.
		''' The method will stop as soon as the class is found. If the class
		''' is not found the method will throw a <CODE>ClassNotFoundException</CODE>
		''' exception.
		''' </summary>
		''' <param name="className"> The name of the class to be loaded. </param>
		''' <param name="loader"> The class loader to be excluded.
		''' </param>
		''' <returns> the loaded class.
		''' </returns>
		''' <exception cref="ClassNotFoundException"> The specified class could not be
		'''    found. </exception>
		Public Shared Function loadClassWithout(ByVal loader As ClassLoader, ByVal className As String) As Type
			MBEANSERVER_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DefaultLoaderRepository).name, "loadClassWithout", className)
			Return load(loader, className)
		End Function

		Private Shared Function load(ByVal without As ClassLoader, ByVal className As String) As Type
			Dim mbsList As IList(Of javax.management.MBeanServer) = javax.management.MBeanServerFactory.findMBeanServer(Nothing)

			For Each mbs As javax.management.MBeanServer In mbsList
				Dim clr As ClassLoaderRepository = mbs.classLoaderRepository
				Try
					Return clr.loadClassWithout(without, className)
				Catch e As ClassNotFoundException
					' OK : Try with next one...
				End Try
			Next mbs
			Throw New ClassNotFoundException(className)
		End Function

	End Class

End Namespace
Imports System

'
' * Copyright (c) 1999, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management


	''' <summary>
	''' <p>Keeps the list of Class Loaders registered in the MBean Server.
	''' It provides the necessary methods to load classes using the registered
	''' Class Loaders.</p>
	''' 
	''' <p>This deprecated class is maintained for compatibility.  In
	''' previous versions of the JMX API, there was one
	''' <code>DefaultLoaderRepository</code> shared by all MBean servers.
	''' As of version 1.2 of the JMX API, that functionality is
	''' approximated by using <seealso cref="MBeanServerFactory#findMBeanServer"/> to
	''' find all known MBean servers, and consulting the {@link
	''' ClassLoaderRepository} of each one.  It is strongly recommended
	''' that code referencing <code>DefaultLoaderRepository</code> be
	''' rewritten.</p>
	''' </summary>
	''' @deprecated Use
	''' <seealso cref="javax.management.MBeanServer#getClassLoaderRepository()"/>
	''' instead.
	''' 
	''' @since 1.5 
	<Obsolete("Use")> _
	Public Class DefaultLoaderRepository
		''' <summary>
		''' Go through the list of class loaders and try to load the requested class.
		''' The method will stop as soon as the class is found. If the class
		''' is not found the method will throw a <CODE>ClassNotFoundException</CODE>
		''' exception.
		''' </summary>
		''' <param name="className"> The name of the class to be loaded.
		''' </param>
		''' <returns> the loaded class.
		''' </returns>
		''' <exception cref="ClassNotFoundException"> The specified class could not be found. </exception>
		Public Shared Function loadClass(ByVal className As String) As Type
			Return javax.management.loading.DefaultLoaderRepository.loadClass(className)
		End Function


		''' <summary>
		''' Go through the list of class loaders but exclude the given class loader, then try to load
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
		''' <exception cref="ClassNotFoundException"> The specified class could not be found. </exception>
		Public Shared Function loadClassWithout(ByVal loader As ClassLoader, ByVal className As String) As Type
			Return javax.management.loading.DefaultLoaderRepository.loadClassWithout(loader, className)
		End Function

	End Class

End Namespace
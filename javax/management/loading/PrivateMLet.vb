'
' * Copyright (c) 2002, 2007, Oracle and/or its affiliates. All rights reserved.
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
	''' An MLet that is not added to the <seealso cref="ClassLoaderRepository"/>.
	''' This class acts exactly like its parent class, <seealso cref="MLet"/>, with
	''' one exception.  When a PrivateMLet is registered in an MBean
	''' server, it is not added to that MBean server's {@link
	''' ClassLoaderRepository}.  This is true because this class implements
	''' the interface <seealso cref="PrivateClassLoader"/>.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class PrivateMLet
		Inherits MLet
		Implements PrivateClassLoader

		Private Const serialVersionUID As Long = 2503458973393711979L

		''' <summary>
		''' Constructs a new PrivateMLet for the specified URLs using the
		''' default delegation parent ClassLoader.  The URLs will be
		''' searched in the order specified for classes and resources
		''' after first searching in the parent class loader.
		''' </summary>
		''' <param name="urls">  The URLs from which to load classes and resources. </param>
		''' <param name="delegateToCLR">  True if, when a class is not found in
		''' either the parent ClassLoader or the URLs, the MLet should delegate
		''' to its containing MBeanServer's <seealso cref="ClassLoaderRepository"/>.
		'''   </param>
		Public Sub New(ByVal urls As java.net.URL(), ByVal delegateToCLR As Boolean)
			MyBase.New(urls, delegateToCLR)
		End Sub

		''' <summary>
		''' Constructs a new PrivateMLet for the given URLs. The URLs will
		''' be searched in the order specified for classes and resources
		''' after first searching in the specified parent class loader.
		''' The parent argument will be used as the parent class loader
		''' for delegation.
		''' </summary>
		''' <param name="urls">  The URLs from which to load classes and resources. </param>
		''' <param name="parent"> The parent class loader for delegation. </param>
		''' <param name="delegateToCLR">  True if, when a class is not found in
		''' either the parent ClassLoader or the URLs, the MLet should delegate
		''' to its containing MBeanServer's <seealso cref="ClassLoaderRepository"/>.
		'''   </param>
		Public Sub New(ByVal urls As java.net.URL(), ByVal parent As ClassLoader, ByVal delegateToCLR As Boolean)
			MyBase.New(urls, parent, delegateToCLR)
		End Sub

		''' <summary>
		''' Constructs a new PrivateMLet for the specified URLs, parent
		''' class loader, and URLStreamHandlerFactory. The parent argument
		''' will be used as the parent class loader for delegation. The
		''' factory argument will be used as the stream handler factory to
		''' obtain protocol handlers when creating new URLs.
		''' </summary>
		''' <param name="urls">  The URLs from which to load classes and resources. </param>
		''' <param name="parent"> The parent class loader for delegation. </param>
		''' <param name="factory">  The URLStreamHandlerFactory to use when creating URLs. </param>
		''' <param name="delegateToCLR">  True if, when a class is not found in
		''' either the parent ClassLoader or the URLs, the MLet should delegate
		''' to its containing MBeanServer's <seealso cref="ClassLoaderRepository"/>.
		'''   </param>
		Public Sub New(ByVal urls As java.net.URL(), ByVal parent As ClassLoader, ByVal factory As java.net.URLStreamHandlerFactory, ByVal delegateToCLR As Boolean)
			MyBase.New(urls, parent, factory, delegateToCLR)
		End Sub
	End Class

End Namespace
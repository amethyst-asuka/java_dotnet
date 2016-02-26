Imports System

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
	''' <p>Instances of this interface are used to keep the list of ClassLoaders
	''' registered in an MBean Server.
	''' They provide the necessary methods to load classes using the registered
	''' ClassLoaders.</p>
	''' 
	''' <p>The first ClassLoader in a <code>ClassLoaderRepository</code> is
	''' always the MBean Server's own ClassLoader.</p>
	''' 
	''' <p>When an MBean is registered in an MBean Server, if it is of a
	''' subclass of <seealso cref="java.lang.ClassLoader"/> and if it does not
	''' implement the interface <seealso cref="PrivateClassLoader"/>, it is added to
	''' the end of the MBean Server's <code>ClassLoaderRepository</code>.
	''' If it is subsequently unregistered from the MBean Server, it is
	''' removed from the <code>ClassLoaderRepository</code>.</p>
	''' 
	''' <p>The order of MBeans in the <code>ClassLoaderRepository</code> is
	''' significant.  For any two MBeans <em>X</em> and <em>Y</em> in the
	''' <code>ClassLoaderRepository</code>, <em>X</em> must appear before
	''' <em>Y</em> if the registration of <em>X</em> was completed before
	''' the registration of <em>Y</em> started.  If <em>X</em> and
	''' <em>Y</em> were registered concurrently, their order is
	''' indeterminate.  The registration of an MBean corresponds to the
	''' call to <seealso cref="MBeanServer#registerMBean"/> or one of the {@link
	''' MBeanServer}<code>.createMBean</code> methods.</p>
	''' </summary>
	''' <seealso cref= javax.management.MBeanServerFactory
	''' 
	''' @since 1.5 </seealso>
	Public Interface ClassLoaderRepository

		''' <summary>
		''' <p>Load the given class name through the list of class loaders.
		''' Each ClassLoader in turn from the ClassLoaderRepository is
		''' asked to load the class via its {@link
		''' ClassLoader#loadClass(String)} method.  If it successfully
		''' returns a <seealso cref="Class"/> object, that is the result of this
		''' method.  If it throws a <seealso cref="ClassNotFoundException"/>, the
		''' search continues with the next ClassLoader.  If it throws
		''' another exception, the exception is propagated from this
		''' method.  If the end of the list is reached, a {@link
		''' ClassNotFoundException} is thrown.</p>
		''' </summary>
		''' <param name="className"> The name of the class to be loaded.
		''' </param>
		''' <returns> the loaded class.
		''' </returns>
		''' <exception cref="ClassNotFoundException"> The specified class could not be
		'''            found. </exception>
		Function loadClass(ByVal className As String) As Type

		''' <summary>
		''' <p>Load the given class name through the list of class loaders,
		''' excluding the given one.  Each ClassLoader in turn from the
		''' ClassLoaderRepository, except <code>exclude</code>, is asked to
		''' load the class via its <seealso cref="ClassLoader#loadClass(String)"/>
		''' method.  If it successfully returns a <seealso cref="Class"/> object,
		''' that is the result of this method.  If it throws a {@link
		''' ClassNotFoundException}, the search continues with the next
		''' ClassLoader.  If it throws another exception, the exception is
		''' propagated from this method.  If the end of the list is
		''' reached, a <seealso cref="ClassNotFoundException"/> is thrown.</p>
		''' 
		''' <p>Be aware that if a ClassLoader in the ClassLoaderRepository
		''' calls this method from its {@link ClassLoader#loadClass(String)
		''' loadClass} method, it exposes itself to a deadlock if another
		''' ClassLoader in the ClassLoaderRepository does the same thing at
		''' the same time.  The <seealso cref="#loadClassBefore"/> method is
		''' recommended to avoid the risk of deadlock.</p>
		''' </summary>
		''' <param name="className"> The name of the class to be loaded. </param>
		''' <param name="exclude"> The class loader to be excluded.  May be null,
		''' in which case this method is equivalent to {@link #loadClass
		''' loadClass(className)}.
		''' </param>
		''' <returns> the loaded class.
		''' </returns>
		''' <exception cref="ClassNotFoundException"> The specified class could not
		''' be found. </exception>
		Function loadClassWithout(ByVal exclude As ClassLoader, ByVal className As String) As Type

		''' <summary>
		''' <p>Load the given class name through the list of class loaders,
		''' stopping at the given one.  Each ClassLoader in turn from the
		''' ClassLoaderRepository is asked to load the class via its {@link
		''' ClassLoader#loadClass(String)} method.  If it successfully
		''' returns a <seealso cref="Class"/> object, that is the result of this
		''' method.  If it throws a <seealso cref="ClassNotFoundException"/>, the
		''' search continues with the next ClassLoader.  If it throws
		''' another exception, the exception is propagated from this
		''' method.  If the search reaches <code>stop</code> or the end of
		''' the list, a <seealso cref="ClassNotFoundException"/> is thrown.</p>
		''' 
		''' <p>Typically this method is called from the {@link
		''' ClassLoader#loadClass(String) loadClass} method of
		''' <code>stop</code>, to consult loaders that appear before it
		''' in the <code>ClassLoaderRepository</code>.  By stopping the
		''' search as soon as <code>stop</code> is reached, a potential
		''' deadlock with concurrent class loading is avoided.</p>
		''' </summary>
		''' <param name="className"> The name of the class to be loaded. </param>
		''' <param name="stop"> The class loader at which to stop.  May be null, in
		''' which case this method is equivalent to {@link #loadClass(String)
		''' loadClass(className)}.
		''' </param>
		''' <returns> the loaded class.
		''' </returns>
		''' <exception cref="ClassNotFoundException"> The specified class could not
		''' be found.
		'''  </exception>
		Function loadClassBefore(ByVal [stop] As ClassLoader, ByVal className As String) As Type

	End Interface

End Namespace
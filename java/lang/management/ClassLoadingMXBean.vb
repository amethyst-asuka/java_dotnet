'
' * Copyright (c) 2003, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.management

	''' <summary>
	''' The management interface for the class loading system of
	''' the Java virtual machine.
	''' 
	''' <p> A Java virtual machine has a single instance of the implementation
	''' class of this interface.  This instance implementing this interface is
	''' an <a href="ManagementFactory.html#MXBean">MXBean</a>
	''' that can be obtained by calling
	''' the <seealso cref="ManagementFactory#getClassLoadingMXBean"/> method or
	''' from the {@link ManagementFactory#getPlatformMBeanServer
	''' platform <tt>MBeanServer</tt>}.
	''' 
	''' <p>The <tt>ObjectName</tt> for uniquely identifying the MXBean for
	''' the class loading system within an <tt>MBeanServer</tt> is:
	''' <blockquote>
	''' {@link ManagementFactory#CLASS_LOADING_MXBEAN_NAME
	'''        <tt>java.lang:type=ClassLoading</tt>}
	''' </blockquote>
	''' 
	''' It can be obtained by calling the
	''' <seealso cref="PlatformManagedObject#getObjectName"/> method.
	''' </summary>
	''' <seealso cref= ManagementFactory#getPlatformMXBeans(Class) </seealso>
	''' <seealso cref= <a href="../../../javax/management/package-summary.html">
	'''      JMX Specification.</a> </seealso>
	''' <seealso cref= <a href="package-summary.html#examples">
	'''      Ways to Access MXBeans</a>
	''' 
	''' @author  Mandy Chung
	''' @since   1.5 </seealso>
	Public Interface ClassLoadingMXBean
		Inherits PlatformManagedObject

		''' <summary>
		''' Returns the total number of classes that have been loaded since
		''' the Java virtual machine has started execution.
		''' </summary>
		''' <returns> the total number of classes loaded.
		'''  </returns>
		ReadOnly Property totalLoadedClassCount As Long

		''' <summary>
		''' Returns the number of classes that are currently loaded in the
		''' Java virtual machine.
		''' </summary>
		''' <returns> the number of currently loaded classes. </returns>
		ReadOnly Property loadedClassCount As Integer

		''' <summary>
		''' Returns the total number of classes unloaded since the Java virtual machine
		''' has started execution.
		''' </summary>
		''' <returns> the total number of unloaded classes. </returns>
		ReadOnly Property unloadedClassCount As Long

		''' <summary>
		''' Tests if the verbose output for the class loading system is enabled.
		''' </summary>
		''' <returns> <tt>true</tt> if the verbose output for the class loading
		''' system is enabled; <tt>false</tt> otherwise. </returns>
		Property verbose As Boolean


	End Interface

End Namespace
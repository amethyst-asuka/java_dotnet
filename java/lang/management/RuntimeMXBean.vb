Imports System.Collections.Generic

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

Namespace java.lang.management

	''' <summary>
	''' The management interface for the runtime system of
	''' the Java virtual machine.
	''' 
	''' <p> A Java virtual machine has a single instance of the implementation
	''' class of this interface.  This instance implementing this interface is
	''' an <a href="ManagementFactory.html#MXBean">MXBean</a>
	''' that can be obtained by calling
	''' the <seealso cref="ManagementFactory#getRuntimeMXBean"/> method or
	''' from the {@link ManagementFactory#getPlatformMBeanServer
	''' platform <tt>MBeanServer</tt>} method.
	''' 
	''' <p>The <tt>ObjectName</tt> for uniquely identifying the MXBean for
	''' the runtime system within an MBeanServer is:
	''' <blockquote>
	'''    {@link ManagementFactory#RUNTIME_MXBEAN_NAME
	'''           <tt>java.lang:type=Runtime</tt>}
	''' </blockquote>
	''' 
	''' It can be obtained by calling the
	''' <seealso cref="PlatformManagedObject#getObjectName"/> method.
	''' 
	''' <p> This interface defines several convenient methods for accessing
	''' system properties about the Java virtual machine.
	''' </summary>
	''' <seealso cref= ManagementFactory#getPlatformMXBeans(Class) </seealso>
	''' <seealso cref= <a href="../../../javax/management/package-summary.html">
	'''      JMX Specification.</a> </seealso>
	''' <seealso cref= <a href="package-summary.html#examples">
	'''      Ways to Access MXBeans</a>
	''' 
	''' @author  Mandy Chung
	''' @since   1.5 </seealso>
	Public Interface RuntimeMXBean
		Inherits PlatformManagedObject

		''' <summary>
		''' Returns the name representing the running Java virtual machine.
		''' The returned name string can be any arbitrary string and
		''' a Java virtual machine implementation can choose
		''' to embed platform-specific useful information in the
		''' returned name string.  Each running virtual machine could have
		''' a different name.
		''' </summary>
		''' <returns> the name representing the running Java virtual machine. </returns>
		ReadOnly Property name As String

		''' <summary>
		''' Returns the Java virtual machine implementation name.
		''' This method is equivalent to {@link System#getProperty
		''' System.getProperty("java.vm.name")}.
		''' </summary>
		''' <returns> the Java virtual machine implementation name.
		''' </returns>
		''' <exception cref="java.lang.SecurityException">
		'''     if a security manager exists and its
		'''     <code>checkPropertiesAccess</code> method doesn't allow access
		'''     to this system property. </exception>
		''' <seealso cref= java.lang.SecurityManager#checkPropertyAccess(java.lang.String) </seealso>
		''' <seealso cref= java.lang.System#getProperty </seealso>
		ReadOnly Property vmName As String

		''' <summary>
		''' Returns the Java virtual machine implementation vendor.
		''' This method is equivalent to {@link System#getProperty
		''' System.getProperty("java.vm.vendor")}.
		''' </summary>
		''' <returns> the Java virtual machine implementation vendor.
		''' </returns>
		''' <exception cref="java.lang.SecurityException">
		'''     if a security manager exists and its
		'''     <code>checkPropertiesAccess</code> method doesn't allow access
		'''     to this system property. </exception>
		''' <seealso cref= java.lang.SecurityManager#checkPropertyAccess(java.lang.String) </seealso>
		''' <seealso cref= java.lang.System#getProperty </seealso>
		ReadOnly Property vmVendor As String

		''' <summary>
		''' Returns the Java virtual machine implementation version.
		''' This method is equivalent to {@link System#getProperty
		''' System.getProperty("java.vm.version")}.
		''' </summary>
		''' <returns> the Java virtual machine implementation version.
		''' </returns>
		''' <exception cref="java.lang.SecurityException">
		'''     if a security manager exists and its
		'''     <code>checkPropertiesAccess</code> method doesn't allow access
		'''     to this system property. </exception>
		''' <seealso cref= java.lang.SecurityManager#checkPropertyAccess(java.lang.String) </seealso>
		''' <seealso cref= java.lang.System#getProperty </seealso>
		ReadOnly Property vmVersion As String

		''' <summary>
		''' Returns the Java virtual machine specification name.
		''' This method is equivalent to {@link System#getProperty
		''' System.getProperty("java.vm.specification.name")}.
		''' </summary>
		''' <returns> the Java virtual machine specification name.
		''' </returns>
		''' <exception cref="java.lang.SecurityException">
		'''     if a security manager exists and its
		'''     <code>checkPropertiesAccess</code> method doesn't allow access
		'''     to this system property. </exception>
		''' <seealso cref= java.lang.SecurityManager#checkPropertyAccess(java.lang.String) </seealso>
		''' <seealso cref= java.lang.System#getProperty </seealso>
		ReadOnly Property specName As String

		''' <summary>
		''' Returns the Java virtual machine specification vendor.
		''' This method is equivalent to {@link System#getProperty
		''' System.getProperty("java.vm.specification.vendor")}.
		''' </summary>
		''' <returns> the Java virtual machine specification vendor.
		''' </returns>
		''' <exception cref="java.lang.SecurityException">
		'''     if a security manager exists and its
		'''     <code>checkPropertiesAccess</code> method doesn't allow access
		'''     to this system property. </exception>
		''' <seealso cref= java.lang.SecurityManager#checkPropertyAccess(java.lang.String) </seealso>
		''' <seealso cref= java.lang.System#getProperty </seealso>
		ReadOnly Property specVendor As String

		''' <summary>
		''' Returns the Java virtual machine specification version.
		''' This method is equivalent to {@link System#getProperty
		''' System.getProperty("java.vm.specification.version")}.
		''' </summary>
		''' <returns> the Java virtual machine specification version.
		''' </returns>
		''' <exception cref="java.lang.SecurityException">
		'''     if a security manager exists and its
		'''     <code>checkPropertiesAccess</code> method doesn't allow access
		'''     to this system property. </exception>
		''' <seealso cref= java.lang.SecurityManager#checkPropertyAccess(java.lang.String) </seealso>
		''' <seealso cref= java.lang.System#getProperty </seealso>
		ReadOnly Property specVersion As String


		''' <summary>
		''' Returns the version of the specification for the management interface
		''' implemented by the running Java virtual machine.
		''' </summary>
		''' <returns> the version of the specification for the management interface
		''' implemented by the running Java virtual machine. </returns>
		ReadOnly Property managementSpecVersion As String

		''' <summary>
		''' Returns the Java class path that is used by the system class loader
		''' to search for class files.
		''' This method is equivalent to {@link System#getProperty
		''' System.getProperty("java.class.path")}.
		''' 
		''' <p> Multiple paths in the Java class path are separated by the
		''' path separator character of the platform of the Java virtual machine
		''' being monitored.
		''' </summary>
		''' <returns> the Java class path.
		''' </returns>
		''' <exception cref="java.lang.SecurityException">
		'''     if a security manager exists and its
		'''     <code>checkPropertiesAccess</code> method doesn't allow access
		'''     to this system property. </exception>
		''' <seealso cref= java.lang.SecurityManager#checkPropertyAccess(java.lang.String) </seealso>
		''' <seealso cref= java.lang.System#getProperty </seealso>
		ReadOnly Property classPath As String

		''' <summary>
		''' Returns the Java library path.
		''' This method is equivalent to {@link System#getProperty
		''' System.getProperty("java.library.path")}.
		''' 
		''' <p> Multiple paths in the Java library path are separated by the
		''' path separator character of the platform of the Java virtual machine
		''' being monitored.
		''' </summary>
		''' <returns> the Java library path.
		''' </returns>
		''' <exception cref="java.lang.SecurityException">
		'''     if a security manager exists and its
		'''     <code>checkPropertiesAccess</code> method doesn't allow access
		'''     to this system property. </exception>
		''' <seealso cref= java.lang.SecurityManager#checkPropertyAccess(java.lang.String) </seealso>
		''' <seealso cref= java.lang.System#getProperty </seealso>
		ReadOnly Property libraryPath As String

		''' <summary>
		''' Tests if the Java virtual machine supports the boot class path
		''' mechanism used by the bootstrap class loader to search for class
		''' files.
		''' </summary>
		''' <returns> <tt>true</tt> if the Java virtual machine supports the
		''' class path mechanism; <tt>false</tt> otherwise. </returns>
		ReadOnly Property bootClassPathSupported As Boolean

		''' <summary>
		''' Returns the boot class path that is used by the bootstrap class loader
		''' to search for class files.
		''' 
		''' <p> Multiple paths in the boot class path are separated by the
		''' path separator character of the platform on which the Java
		''' virtual machine is running.
		''' 
		''' <p>A Java virtual machine implementation may not support
		''' the boot class path mechanism for the bootstrap class loader
		''' to search for class files.
		''' The <seealso cref="#isBootClassPathSupported"/> method can be used
		''' to determine if the Java virtual machine supports this method.
		''' </summary>
		''' <returns> the boot class path.
		''' </returns>
		''' <exception cref="java.lang.UnsupportedOperationException">
		'''     if the Java virtual machine does not support this operation.
		''' </exception>
		''' <exception cref="java.lang.SecurityException">
		'''     if a security manager exists and the caller does not have
		'''     ManagementPermission("monitor"). </exception>
		ReadOnly Property bootClassPath As String

		''' <summary>
		''' Returns the input arguments passed to the Java virtual machine
		''' which does not include the arguments to the <tt>main</tt> method.
		''' This method returns an empty list if there is no input argument
		''' to the Java virtual machine.
		''' <p>
		''' Some Java virtual machine implementations may take input arguments
		''' from multiple different sources: for examples, arguments passed from
		''' the application that launches the Java virtual machine such as
		''' the 'java' command, environment variables, configuration files, etc.
		''' <p>
		''' Typically, not all command-line options to the 'java' command
		''' are passed to the Java virtual machine.
		''' Thus, the returned input arguments may not
		''' include all command-line options.
		''' 
		''' <p>
		''' <b>MBeanServer access</b>:<br>
		''' The mapped type of {@code List<String>} is <tt>String[]</tt>.
		''' </summary>
		''' <returns> a list of <tt>String</tt> objects; each element
		''' is an argument passed to the Java virtual machine.
		''' </returns>
		''' <exception cref="java.lang.SecurityException">
		'''     if a security manager exists and the caller does not have
		'''     ManagementPermission("monitor"). </exception>
		ReadOnly Property inputArguments As IList(Of String)

		''' <summary>
		''' Returns the uptime of the Java virtual machine in milliseconds.
		''' </summary>
		''' <returns> uptime of the Java virtual machine in milliseconds. </returns>
		ReadOnly Property uptime As Long

		''' <summary>
		''' Returns the start time of the Java virtual machine in milliseconds.
		''' This method returns the approximate time when the Java virtual
		''' machine started.
		''' </summary>
		''' <returns> start time of the Java virtual machine in milliseconds.
		'''  </returns>
		ReadOnly Property startTime As Long

		''' <summary>
		''' Returns a map of names and values of all system properties.
		''' This method calls <seealso cref="System#getProperties"/> to get all
		''' system properties.  Properties whose name or value is not
		''' a <tt>String</tt> are omitted.
		''' 
		''' <p>
		''' <b>MBeanServer access</b>:<br>
		''' The mapped type of {@code Map<String,String>} is
		''' <seealso cref="javax.management.openmbean.TabularData TabularData"/>
		''' with two items in each row as follows:
		''' <blockquote>
		''' <table border summary="Name and Type for each item">
		''' <tr>
		'''   <th>Item Name</th>
		'''   <th>Item Type</th>
		'''   </tr>
		''' <tr>
		'''   <td><tt>key</tt></td>
		'''   <td><tt>String</tt></td>
		'''   </tr>
		''' <tr>
		'''   <td><tt>value</tt></td>
		'''   <td><tt>String</tt></td>
		'''   </tr>
		''' </table>
		''' </blockquote>
		''' </summary>
		''' <returns> a map of names and values of all system properties.
		''' </returns>
		''' <exception cref="java.lang.SecurityException">
		'''     if a security manager exists and its
		'''     <code>checkPropertiesAccess</code> method doesn't allow access
		'''     to the system properties. </exception>
		ReadOnly Property systemProperties As IDictionary(Of String, String)
	End Interface

End Namespace
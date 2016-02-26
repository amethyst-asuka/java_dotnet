'
' * Copyright (c) 2000, 2007, Oracle and/or its affiliates. All rights reserved.
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


Namespace javax.management.openmbean


	' java import
	'


	' jmx import
	'



	''' <summary>
	''' <p>Describes an Open MBean: an Open MBean is recognized as such if
	''' its {@link javax.management.DynamicMBean#getMBeanInfo()
	''' getMBeanInfo()} method returns an instance of a class which
	''' implements the <seealso cref="OpenMBeanInfo"/> interface, typically {@link
	''' OpenMBeanInfoSupport}.</p>
	''' 
	''' <p>This interface declares the same methods as the class {@link
	''' javax.management.MBeanInfo}.  A class implementing this interface
	''' (typically <seealso cref="OpenMBeanInfoSupport"/>) should extend {@link
	''' javax.management.MBeanInfo}.</p>
	''' 
	''' <p>The <seealso cref="#getAttributes()"/>, <seealso cref="#getOperations()"/> and
	''' <seealso cref="#getConstructors()"/> methods of the implementing class should
	''' return at runtime an array of instances of a subclass of {@link
	''' MBeanAttributeInfo}, <seealso cref="MBeanOperationInfo"/> or {@link
	''' MBeanConstructorInfo} respectively which implement the {@link
	''' OpenMBeanAttributeInfo}, <seealso cref="OpenMBeanOperationInfo"/> or {@link
	''' OpenMBeanConstructorInfo} interface respectively.
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface OpenMBeanInfo

		' Re-declares the methods that are in class MBeanInfo of JMX 1.0
		' (methods will be removed when MBeanInfo is made a parent interface of this interface)

		''' <summary>
		''' Returns the fully qualified Java class name of the open MBean
		''' instances this <tt>OpenMBeanInfo</tt> describes.
		''' </summary>
		''' <returns> the class name. </returns>
		ReadOnly Property className As String

		''' <summary>
		''' Returns a human readable description of the type of open MBean
		''' instances this <tt>OpenMBeanInfo</tt> describes.
		''' </summary>
		''' <returns> the description. </returns>
		ReadOnly Property description As String

		''' <summary>
		''' Returns an array of <tt>OpenMBeanAttributeInfo</tt> instances
		''' describing each attribute in the open MBean described by this
		''' <tt>OpenMBeanInfo</tt> instance.  Each instance in the returned
		''' array should actually be a subclass of
		''' <tt>MBeanAttributeInfo</tt> which implements the
		''' <tt>OpenMBeanAttributeInfo</tt> interface (typically {@link
		''' OpenMBeanAttributeInfoSupport}).
		''' </summary>
		''' <returns> the attribute array. </returns>
		ReadOnly Property attributes As javax.management.MBeanAttributeInfo()

		''' <summary>
		''' Returns an array of <tt>OpenMBeanOperationInfo</tt> instances
		''' describing each operation in the open MBean described by this
		''' <tt>OpenMBeanInfo</tt> instance.  Each instance in the returned
		''' array should actually be a subclass of
		''' <tt>MBeanOperationInfo</tt> which implements the
		''' <tt>OpenMBeanOperationInfo</tt> interface (typically {@link
		''' OpenMBeanOperationInfoSupport}).
		''' </summary>
		''' <returns> the operation array. </returns>
		ReadOnly Property operations As javax.management.MBeanOperationInfo()

		''' <summary>
		''' Returns an array of <tt>OpenMBeanConstructorInfo</tt> instances
		''' describing each constructor in the open MBean described by this
		''' <tt>OpenMBeanInfo</tt> instance.  Each instance in the returned
		''' array should actually be a subclass of
		''' <tt>MBeanConstructorInfo</tt> which implements the
		''' <tt>OpenMBeanConstructorInfo</tt> interface (typically {@link
		''' OpenMBeanConstructorInfoSupport}).
		''' </summary>
		''' <returns> the constructor array. </returns>
		ReadOnly Property constructors As javax.management.MBeanConstructorInfo()

		''' <summary>
		''' Returns an array of <tt>MBeanNotificationInfo</tt> instances
		''' describing each notification emitted by the open MBean
		''' described by this <tt>OpenMBeanInfo</tt> instance.
		''' </summary>
		''' <returns> the notification array. </returns>
		ReadOnly Property notifications As javax.management.MBeanNotificationInfo()


		' commodity methods
		'

		''' <summary>
		''' Compares the specified <var>obj</var> parameter with this <code>OpenMBeanInfo</code> instance for equality.
		''' <p>
		''' Returns <tt>true</tt> if and only if all of the following statements are true:
		''' <ul>
		''' <li><var>obj</var> is non null,</li>
		''' <li><var>obj</var> also implements the <code>OpenMBeanInfo</code> interface,</li>
		''' <li>their class names are equal</li>
		''' <li>their infos on attributes, constructors, operations and notifications are equal</li>
		''' </ul>
		''' This ensures that this <tt>equals</tt> method works properly for <var>obj</var> parameters which are
		''' different implementations of the <code>OpenMBeanInfo</code> interface.
		''' <br>&nbsp; </summary>
		''' <param name="obj">  the object to be compared for equality with this <code>OpenMBeanInfo</code> instance;
		''' </param>
		''' <returns>  <code>true</code> if the specified object is equal to this <code>OpenMBeanInfo</code> instance. </returns>
		Function Equals(ByVal obj As Object) As Boolean

		''' <summary>
		''' Returns the hash code value for this <code>OpenMBeanInfo</code> instance.
		''' <p>
		''' The hash code of an <code>OpenMBeanInfo</code> instance is the sum of the hash codes
		''' of all elements of information used in <code>equals</code> comparisons
		''' (ie: its class name, and its infos on attributes, constructors, operations and notifications,
		''' where the hashCode of each of these arrays is calculated by a call to
		'''  <tt>new java.util.HashSet(java.util.Arrays.asList(this.getSignature)).hashCode()</tt>).
		''' <p>
		''' This ensures that <code> t1.equals(t2) </code> implies that <code> t1.hashCode()==t2.hashCode() </code>
		''' for any two <code>OpenMBeanInfo</code> instances <code>t1</code> and <code>t2</code>,
		''' as required by the general contract of the method
		''' <seealso cref="Object#hashCode() Object.hashCode()"/>.
		''' <p>
		''' </summary>
		''' <returns>  the hash code value for this <code>OpenMBeanInfo</code> instance </returns>
		Function GetHashCode() As Integer

		''' <summary>
		''' Returns a string representation of this <code>OpenMBeanInfo</code> instance.
		''' <p>
		''' The string representation consists of the name of this class (ie <code>javax.management.openmbean.OpenMBeanInfo</code>),
		''' the MBean class name,
		''' and the string representation of infos on attributes, constructors, operations and notifications of the described MBean.
		''' </summary>
		''' <returns>  a string representation of this <code>OpenMBeanInfo</code> instance </returns>
		Function ToString() As String

	End Interface

End Namespace
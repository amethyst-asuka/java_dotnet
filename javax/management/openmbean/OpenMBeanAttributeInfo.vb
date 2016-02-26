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
	''' <p>Describes an attribute of an open MBean.</p>
	''' 
	''' <p>This interface declares the same methods as the class {@link
	''' javax.management.MBeanAttributeInfo}.  A class implementing this
	''' interface (typically <seealso cref="OpenMBeanAttributeInfoSupport"/>) should
	''' extend <seealso cref="javax.management.MBeanAttributeInfo"/>.</p>
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface OpenMBeanAttributeInfo
		Inherits OpenMBeanParameterInfo


		' Re-declares the methods that are in class MBeanAttributeInfo of JMX 1.0
		' (these will be removed when MBeanAttributeInfo is made a parent interface of this interface)

		''' <summary>
		''' Returns <tt>true</tt> if the attribute described by this <tt>OpenMBeanAttributeInfo</tt> instance is readable,
		''' <tt>false</tt> otherwise.
		''' </summary>
		''' <returns> true if the attribute is readable. </returns>
		ReadOnly Property readable As Boolean

		''' <summary>
		''' Returns <tt>true</tt> if the attribute described by this <tt>OpenMBeanAttributeInfo</tt> instance is writable,
		''' <tt>false</tt> otherwise.
		''' </summary>
		''' <returns> true if the attribute is writable. </returns>
		ReadOnly Property writable As Boolean

		''' <summary>
		''' Returns <tt>true</tt> if the attribute described by this <tt>OpenMBeanAttributeInfo</tt> instance
		''' is accessed through a <tt>is<i>XXX</i></tt> getter (applies only to <tt>boolean</tt> and <tt>Boolean</tt> values),
		''' <tt>false</tt> otherwise.
		''' </summary>
		''' <returns> true if the attribute is accessed through <tt>is<i>XXX</i></tt>. </returns>
		ReadOnly Property [is] As Boolean


		' commodity methods
		'

		''' <summary>
		''' Compares the specified <var>obj</var> parameter with this <code>OpenMBeanAttributeInfo</code> instance for equality.
		''' <p>
		''' Returns <tt>true</tt> if and only if all of the following statements are true:
		''' <ul>
		''' <li><var>obj</var> is non null,</li>
		''' <li><var>obj</var> also implements the <code>OpenMBeanAttributeInfo</code> interface,</li>
		''' <li>their names are equal</li>
		''' <li>their open types are equal</li>
		''' <li>their access properties (isReadable, isWritable and isIs) are equal</li>
		''' <li>their default, min, max and legal values are equal.</li>
		''' </ul>
		''' This ensures that this <tt>equals</tt> method works properly for <var>obj</var> parameters which are
		''' different implementations of the <code>OpenMBeanAttributeInfo</code> interface.
		''' <br>&nbsp; </summary>
		''' <param name="obj">  the object to be compared for equality with this <code>OpenMBeanAttributeInfo</code> instance;
		''' </param>
		''' <returns>  <code>true</code> if the specified object is equal to this <code>OpenMBeanAttributeInfo</code> instance. </returns>
		Function Equals(ByVal obj As Object) As Boolean

		''' <summary>
		''' Returns the hash code value for this <code>OpenMBeanAttributeInfo</code> instance.
		''' <p>
		''' The hash code of an <code>OpenMBeanAttributeInfo</code> instance is the sum of the hash codes
		''' of all elements of information used in <code>equals</code> comparisons
		''' (ie: its name, its <i>open type</i>, and its default, min, max and legal values).
		''' <p>
		''' This ensures that <code> t1.equals(t2) </code> implies that <code> t1.hashCode()==t2.hashCode() </code>
		''' for any two <code>OpenMBeanAttributeInfo</code> instances <code>t1</code> and <code>t2</code>,
		''' as required by the general contract of the method
		''' <seealso cref="Object#hashCode() Object.hashCode()"/>.
		''' <p>
		''' </summary>
		''' <returns>  the hash code value for this <code>OpenMBeanAttributeInfo</code> instance </returns>
		Function GetHashCode() As Integer

		''' <summary>
		''' Returns a string representation of this <code>OpenMBeanAttributeInfo</code> instance.
		''' <p>
		''' The string representation consists of the name of this class (ie <code>javax.management.openmbean.OpenMBeanAttributeInfo</code>),
		''' the string representation of the name and open type of the described attribute,
		''' and the string representation of its default, min, max and legal values.
		''' </summary>
		''' <returns>  a string representation of this <code>OpenMBeanAttributeInfo</code> instance </returns>
		Function ToString() As String


		' methods specific to open MBeans are inherited from
		' OpenMBeanParameterInfo
		'

	End Interface

End Namespace
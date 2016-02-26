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
	''' <p>Describes a constructor of an Open MBean.</p>
	''' 
	''' <p>This interface declares the same methods as the class {@link
	''' javax.management.MBeanConstructorInfo}.  A class implementing this
	''' interface (typically <seealso cref="OpenMBeanConstructorInfoSupport"/>)
	''' should extend <seealso cref="javax.management.MBeanConstructorInfo"/>.</p>
	''' 
	''' <p>The <seealso cref="#getSignature()"/> method should return at runtime an
	''' array of instances of a subclass of <seealso cref="MBeanParameterInfo"/>
	''' which implements the <seealso cref="OpenMBeanParameterInfo"/> interface
	''' (typically <seealso cref="OpenMBeanParameterInfoSupport"/>).</p>
	''' 
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface OpenMBeanConstructorInfo

		' Re-declares the methods that are in class MBeanConstructorInfo of JMX 1.0
		' (methods will be removed when MBeanConstructorInfo is made a parent interface of this interface)

		''' <summary>
		''' Returns a human readable description of the constructor
		''' described by this <tt>OpenMBeanConstructorInfo</tt> instance.
		''' </summary>
		''' <returns> the description. </returns>
		ReadOnly Property description As String

		''' <summary>
		''' Returns the name of the constructor
		''' described by this <tt>OpenMBeanConstructorInfo</tt> instance.
		''' </summary>
		''' <returns> the name. </returns>
		ReadOnly Property name As String

		''' <summary>
		''' Returns an array of <tt>OpenMBeanParameterInfo</tt> instances
		''' describing each parameter in the signature of the constructor
		''' described by this <tt>OpenMBeanConstructorInfo</tt> instance.
		''' </summary>
		''' <returns> the signature. </returns>
		ReadOnly Property signature As javax.management.MBeanParameterInfo()


		' commodity methods
		'

		''' <summary>
		''' Compares the specified <var>obj</var> parameter with this <code>OpenMBeanConstructorInfo</code> instance for equality.
		''' <p>
		''' Returns <tt>true</tt> if and only if all of the following statements are true:
		''' <ul>
		''' <li><var>obj</var> is non null,</li>
		''' <li><var>obj</var> also implements the <code>OpenMBeanConstructorInfo</code> interface,</li>
		''' <li>their names are equal</li>
		''' <li>their signatures are equal.</li>
		''' </ul>
		''' This ensures that this <tt>equals</tt> method works properly for <var>obj</var> parameters which are
		''' different implementations of the <code>OpenMBeanConstructorInfo</code> interface.
		''' <br>&nbsp; </summary>
		''' <param name="obj">  the object to be compared for equality with this <code>OpenMBeanConstructorInfo</code> instance;
		''' </param>
		''' <returns>  <code>true</code> if the specified object is equal to this <code>OpenMBeanConstructorInfo</code> instance. </returns>
		Function Equals(ByVal obj As Object) As Boolean

		''' <summary>
		''' Returns the hash code value for this <code>OpenMBeanConstructorInfo</code> instance.
		''' <p>
		''' The hash code of an <code>OpenMBeanConstructorInfo</code> instance is the sum of the hash codes
		''' of all elements of information used in <code>equals</code> comparisons
		''' (ie: its name and signature, where the signature hashCode is calculated by a call to
		'''  <tt>java.util.Arrays.asList(this.getSignature).hashCode()</tt>).
		''' <p>
		''' This ensures that <code> t1.equals(t2) </code> implies that <code> t1.hashCode()==t2.hashCode() </code>
		''' for any two <code>OpenMBeanConstructorInfo</code> instances <code>t1</code> and <code>t2</code>,
		''' as required by the general contract of the method
		''' <seealso cref="Object#hashCode() Object.hashCode()"/>.
		''' <p>
		''' </summary>
		''' <returns>  the hash code value for this <code>OpenMBeanConstructorInfo</code> instance </returns>
		Function GetHashCode() As Integer

		''' <summary>
		''' Returns a string representation of this <code>OpenMBeanConstructorInfo</code> instance.
		''' <p>
		''' The string representation consists of the name of this class (ie <code>javax.management.openmbean.OpenMBeanConstructorInfo</code>),
		''' and the name and signature of the described constructor.
		''' </summary>
		''' <returns>  a string representation of this <code>OpenMBeanConstructorInfo</code> instance </returns>
		Function ToString() As String

	End Interface

End Namespace
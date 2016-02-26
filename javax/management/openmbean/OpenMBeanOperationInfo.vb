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
	''' <p>Describes an operation of an Open MBean.</p>
	''' 
	''' <p>This interface declares the same methods as the class {@link
	''' javax.management.MBeanOperationInfo}.  A class implementing this
	''' interface (typically <seealso cref="OpenMBeanOperationInfoSupport"/>) should
	''' extend <seealso cref="javax.management.MBeanOperationInfo"/>.</p>
	''' 
	''' <p>The <seealso cref="#getSignature()"/> method should return at runtime an
	''' array of instances of a subclass of <seealso cref="MBeanParameterInfo"/>
	''' which implements the <seealso cref="OpenMBeanParameterInfo"/> interface
	''' (typically <seealso cref="OpenMBeanParameterInfoSupport"/>).</p>
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface OpenMBeanOperationInfo

		' Re-declares fields and methods that are in class MBeanOperationInfo of JMX 1.0
		' (fields and methods will be removed when MBeanOperationInfo is made a parent interface of this interface)

		''' <summary>
		''' Returns a human readable description of the operation
		''' described by this <tt>OpenMBeanOperationInfo</tt> instance.
		''' </summary>
		''' <returns> the description. </returns>
		ReadOnly Property description As String

		''' <summary>
		''' Returns the name of the operation
		''' described by this <tt>OpenMBeanOperationInfo</tt> instance.
		''' </summary>
		''' <returns> the name. </returns>
		ReadOnly Property name As String

		''' <summary>
		''' Returns an array of <tt>OpenMBeanParameterInfo</tt> instances
		''' describing each parameter in the signature of the operation
		''' described by this <tt>OpenMBeanOperationInfo</tt> instance.
		''' Each instance in the returned array should actually be a
		''' subclass of <tt>MBeanParameterInfo</tt> which implements the
		''' <tt>OpenMBeanParameterInfo</tt> interface (typically {@link
		''' OpenMBeanParameterInfoSupport}).
		''' </summary>
		''' <returns> the signature. </returns>
		ReadOnly Property signature As javax.management.MBeanParameterInfo()

		''' <summary>
		''' Returns an <tt>int</tt> constant qualifying the impact of the
		''' operation described by this <tt>OpenMBeanOperationInfo</tt>
		''' instance.
		''' 
		''' The returned constant is one of {@link
		''' javax.management.MBeanOperationInfo#INFO}, {@link
		''' javax.management.MBeanOperationInfo#ACTION}, {@link
		''' javax.management.MBeanOperationInfo#ACTION_INFO}, or {@link
		''' javax.management.MBeanOperationInfo#UNKNOWN}.
		''' </summary>
		''' <returns> the impact code. </returns>
		ReadOnly Property impact As Integer

		''' <summary>
		''' Returns the fully qualified Java class name of the values
		''' returned by the operation described by this
		''' <tt>OpenMBeanOperationInfo</tt> instance.  This method should
		''' return the same value as a call to
		''' <tt>getReturnOpenType().getClassName()</tt>.
		''' </summary>
		''' <returns> the return type. </returns>
		ReadOnly Property returnType As String


		' Now declares methods that are specific to open MBeans
		'

		''' <summary>
		''' Returns the <i>open type</i> of the values returned by the
		''' operation described by this <tt>OpenMBeanOperationInfo</tt>
		''' instance.
		''' </summary>
		''' <returns> the return type. </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		ReadOnly Property returnOpenType As OpenType(Of ?)


		' commodity methods
		'

		''' <summary>
		''' Compares the specified <var>obj</var> parameter with this <code>OpenMBeanOperationInfo</code> instance for equality.
		''' <p>
		''' Returns <tt>true</tt> if and only if all of the following statements are true:
		''' <ul>
		''' <li><var>obj</var> is non null,</li>
		''' <li><var>obj</var> also implements the <code>OpenMBeanOperationInfo</code> interface,</li>
		''' <li>their names are equal</li>
		''' <li>their signatures are equal</li>
		''' <li>their return open types are equal</li>
		''' <li>their impacts are equal</li>
		''' </ul>
		''' This ensures that this <tt>equals</tt> method works properly for <var>obj</var> parameters which are
		''' different implementations of the <code>OpenMBeanOperationInfo</code> interface.
		''' <br>&nbsp; </summary>
		''' <param name="obj">  the object to be compared for equality with this <code>OpenMBeanOperationInfo</code> instance;
		''' </param>
		''' <returns>  <code>true</code> if the specified object is equal to this <code>OpenMBeanOperationInfo</code> instance. </returns>
		Function Equals(ByVal obj As Object) As Boolean

		''' <summary>
		''' Returns the hash code value for this <code>OpenMBeanOperationInfo</code> instance.
		''' <p>
		''' The hash code of an <code>OpenMBeanOperationInfo</code> instance is the sum of the hash codes
		''' of all elements of information used in <code>equals</code> comparisons
		''' (ie: its name, return open type, impact and signature, where the signature hashCode is calculated by a call to
		'''  <tt>java.util.Arrays.asList(this.getSignature).hashCode()</tt>).
		''' <p>
		''' This ensures that <code> t1.equals(t2) </code> implies that <code> t1.hashCode()==t2.hashCode() </code>
		''' for any two <code>OpenMBeanOperationInfo</code> instances <code>t1</code> and <code>t2</code>,
		''' as required by the general contract of the method
		''' <seealso cref="Object#hashCode() Object.hashCode()"/>.
		''' <p>
		''' </summary>
		''' <returns>  the hash code value for this <code>OpenMBeanOperationInfo</code> instance </returns>
		Function GetHashCode() As Integer

		''' <summary>
		''' Returns a string representation of this <code>OpenMBeanOperationInfo</code> instance.
		''' <p>
		''' The string representation consists of the name of this class (ie <code>javax.management.openmbean.OpenMBeanOperationInfo</code>),
		''' and the name, signature, return open type and impact of the described operation.
		''' </summary>
		''' <returns>  a string representation of this <code>OpenMBeanOperationInfo</code> instance </returns>
		Function ToString() As String

	End Interface

End Namespace
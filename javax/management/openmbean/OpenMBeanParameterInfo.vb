Imports System

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
	''' <p>Describes a parameter used in one or more operations or
	''' constructors of an open MBean.</p>
	''' 
	''' <p>This interface declares the same methods as the class {@link
	''' javax.management.MBeanParameterInfo}.  A class implementing this
	''' interface (typically <seealso cref="OpenMBeanParameterInfoSupport"/>) should
	''' extend <seealso cref="javax.management.MBeanParameterInfo"/>.</p>
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface OpenMBeanParameterInfo


		' Re-declares methods that are in class MBeanParameterInfo of JMX 1.0
		' (these will be removed when MBeanParameterInfo is made a parent interface of this interface)

		''' <summary>
		''' Returns a human readable description of the parameter
		''' described by this <tt>OpenMBeanParameterInfo</tt> instance.
		''' </summary>
		''' <returns> the description. </returns>
		ReadOnly Property description As String

		''' <summary>
		''' Returns the name of the parameter
		''' described by this <tt>OpenMBeanParameterInfo</tt> instance.
		''' </summary>
		''' <returns> the name. </returns>
		ReadOnly Property name As String


		' Now declares methods that are specific to open MBeans
		'

		''' <summary>
		''' Returns the <i>open type</i> of the values of the parameter
		''' described by this <tt>OpenMBeanParameterInfo</tt> instance.
		''' </summary>
		''' <returns> the open type. </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		ReadOnly Property openType As OpenType(Of ?)

		''' <summary>
		''' Returns the default value for this parameter, if it has one, or
		''' <tt>null</tt> otherwise.
		''' </summary>
		''' <returns> the default value. </returns>
		ReadOnly Property defaultValue As Object

		''' <summary>
		''' Returns the set of legal values for this parameter, if it has
		''' one, or <tt>null</tt> otherwise.
		''' </summary>
		''' <returns> the set of legal values. </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		ReadOnly Property legalValues As java.util.Set(Of ?)

		''' <summary>
		''' Returns the minimal value for this parameter, if it has one, or
		''' <tt>null</tt> otherwise.
		''' </summary>
		''' <returns> the minimum value. </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		ReadOnly Property minValue As IComparable(Of ?)

		''' <summary>
		''' Returns the maximal value for this parameter, if it has one, or
		''' <tt>null</tt> otherwise.
		''' </summary>
		''' <returns> the maximum value. </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		ReadOnly Property maxValue As IComparable(Of ?)

		''' <summary>
		''' Returns <tt>true</tt> if this parameter has a specified default
		''' value, or <tt>false</tt> otherwise.
		''' </summary>
		''' <returns> true if there is a default value. </returns>
		Function hasDefaultValue() As Boolean

		''' <summary>
		''' Returns <tt>true</tt> if this parameter has a specified set of
		''' legal values, or <tt>false</tt> otherwise.
		''' </summary>
		''' <returns> true if there is a set of legal values. </returns>
		Function hasLegalValues() As Boolean

		''' <summary>
		''' Returns <tt>true</tt> if this parameter has a specified minimal
		''' value, or <tt>false</tt> otherwise.
		''' </summary>
		''' <returns> true if there is a minimum value. </returns>
		Function hasMinValue() As Boolean

		''' <summary>
		''' Returns <tt>true</tt> if this parameter has a specified maximal
		''' value, or <tt>false</tt> otherwise.
		''' </summary>
		''' <returns> true if there is a maximum value. </returns>
		Function hasMaxValue() As Boolean

		''' <summary>
		''' Tests whether <var>obj</var> is a valid value for the parameter
		''' described by this <code>OpenMBeanParameterInfo</code> instance.
		''' </summary>
		''' <param name="obj"> the object to be tested.
		''' </param>
		''' <returns> <code>true</code> if <var>obj</var> is a valid value
		''' for the parameter described by this
		''' <code>OpenMBeanParameterInfo</code> instance,
		''' <code>false</code> otherwise. </returns>
		Function isValue(ByVal obj As Object) As Boolean


		''' <summary>
		''' Compares the specified <var>obj</var> parameter with this <code>OpenMBeanParameterInfo</code> instance for equality.
		''' <p>
		''' Returns <tt>true</tt> if and only if all of the following statements are true:
		''' <ul>
		''' <li><var>obj</var> is non null,</li>
		''' <li><var>obj</var> also implements the <code>OpenMBeanParameterInfo</code> interface,</li>
		''' <li>their names are equal</li>
		''' <li>their open types are equal</li>
		''' <li>their default, min, max and legal values are equal.</li>
		''' </ul>
		''' This ensures that this <tt>equals</tt> method works properly for <var>obj</var> parameters which are
		''' different implementations of the <code>OpenMBeanParameterInfo</code> interface.
		''' <br>&nbsp; </summary>
		''' <param name="obj">  the object to be compared for equality with this <code>OpenMBeanParameterInfo</code> instance;
		''' </param>
		''' <returns>  <code>true</code> if the specified object is equal to this <code>OpenMBeanParameterInfo</code> instance. </returns>
		Function Equals(ByVal obj As Object) As Boolean

		''' <summary>
		''' Returns the hash code value for this <code>OpenMBeanParameterInfo</code> instance.
		''' <p>
		''' The hash code of an <code>OpenMBeanParameterInfo</code> instance is the sum of the hash codes
		''' of all elements of information used in <code>equals</code> comparisons
		''' (ie: its name, its <i>open type</i>, and its default, min, max and legal values).
		''' <p>
		''' This ensures that <code> t1.equals(t2) </code> implies that <code> t1.hashCode()==t2.hashCode() </code>
		''' for any two <code>OpenMBeanParameterInfo</code> instances <code>t1</code> and <code>t2</code>,
		''' as required by the general contract of the method
		''' <seealso cref="Object#hashCode() Object.hashCode()"/>.
		''' <p>
		''' </summary>
		''' <returns>  the hash code value for this <code>OpenMBeanParameterInfo</code> instance </returns>
		Function GetHashCode() As Integer

		''' <summary>
		''' Returns a string representation of this <code>OpenMBeanParameterInfo</code> instance.
		''' <p>
		''' The string representation consists of the name of this class (ie <code>javax.management.openmbean.OpenMBeanParameterInfo</code>),
		''' the string representation of the name and open type of the described parameter,
		''' and the string representation of its default, min, max and legal values.
		''' </summary>
		''' <returns>  a string representation of this <code>OpenMBeanParameterInfo</code> instance </returns>
		Function ToString() As String

	End Interface

End Namespace
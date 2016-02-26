Imports System.Collections.Generic

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
	''' The <tt>CompositeData</tt> interface specifies the behavior of a specific type of complex <i>open data</i> objects
	''' which represent <i>composite data</i> structures.
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface CompositeData


		''' <summary>
		''' Returns the <i>composite type </i> of this <i>composite data</i> instance.
		''' </summary>
		''' <returns> the type of this CompositeData. </returns>
		ReadOnly Property compositeType As CompositeType

		''' <summary>
		''' Returns the value of the item whose name is <tt>key</tt>.
		''' </summary>
		''' <param name="key"> the name of the item.
		''' </param>
		''' <returns> the value associated with this key.
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if <tt>key</tt> is a null or empty String.
		''' </exception>
		''' <exception cref="InvalidKeyException">  if <tt>key</tt> is not an existing item name for this <tt>CompositeData</tt> instance. </exception>
		Function [get](ByVal key As String) As Object

		''' <summary>
		''' Returns an array of the values of the items whose names are specified by <tt>keys</tt>, in the same order as <tt>keys</tt>.
		''' </summary>
		''' <param name="keys"> the names of the items.
		''' </param>
		''' <returns> the values corresponding to the keys.
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if an element in <tt>keys</tt> is a null or empty String.
		''' </exception>
		''' <exception cref="InvalidKeyException">  if an element in <tt>keys</tt> is not an existing item name for this <tt>CompositeData</tt> instance. </exception>
		Function getAll(ByVal keys As String()) As Object()

		''' <summary>
		''' Returns <tt>true</tt> if and only if this <tt>CompositeData</tt> instance contains
		''' an item whose name is <tt>key</tt>.
		''' If <tt>key</tt> is a null or empty String, this method simply returns false.
		''' </summary>
		''' <param name="key"> the key to be tested.
		''' </param>
		''' <returns> true if this <tt>CompositeData</tt> contains the key. </returns>
		Function containsKey(ByVal key As String) As Boolean

		''' <summary>
		''' Returns <tt>true</tt> if and only if this <tt>CompositeData</tt> instance contains an item
		''' whose value is <tt>value</tt>.
		''' </summary>
		''' <param name="value"> the value to be tested.
		''' </param>
		''' <returns> true if this <tt>CompositeData</tt> contains the value. </returns>
		Function containsValue(ByVal value As Object) As Boolean

		''' <summary>
		''' Returns an unmodifiable Collection view of the item values contained in this <tt>CompositeData</tt> instance.
		''' The returned collection's iterator will return the values in the ascending lexicographic order of the corresponding
		''' item names.
		''' </summary>
		''' <returns> the values. </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Function values() As ICollection(Of ?)

		''' <summary>
		''' Compares the specified <var>obj</var> parameter with this
		''' <code>CompositeData</code> instance for equality.
		''' <p>
		''' Returns <tt>true</tt> if and only if all of the following statements are true:
		''' <ul>
		''' <li><var>obj</var> is non null,</li>
		''' <li><var>obj</var> also implements the <code>CompositeData</code> interface,</li>
		''' <li>their composite types are equal</li>
		''' <li>their contents, i.e. (name, value) pairs are equal. If a value contained in
		''' the content is an array, the value comparison is done as if by calling
		''' the <seealso cref="java.util.Arrays#deepEquals(Object[], Object[]) deepEquals"/> method
		''' for arrays of object reference types or the appropriate overloading of
		''' {@code Arrays.equals(e1,e2)} for arrays of primitive types</li>
		''' </ul>
		''' <p>
		''' This ensures that this <tt>equals</tt> method works properly for
		''' <var>obj</var> parameters which are different implementations of the
		''' <code>CompositeData</code> interface, with the restrictions mentioned in the
		''' <seealso cref="java.util.Collection#equals(Object) equals"/>
		''' method of the <tt>java.util.Collection</tt> interface.
		''' </summary>
		''' <param name="obj">  the object to be compared for equality with this
		''' <code>CompositeData</code> instance. </param>
		''' <returns>  <code>true</code> if the specified object is equal to this
		''' <code>CompositeData</code> instance. </returns>
		Function Equals(ByVal obj As Object) As Boolean

		''' <summary>
		''' Returns the hash code value for this <code>CompositeData</code> instance.
		''' <p>
		''' The hash code of a <code>CompositeData</code> instance is the sum of the hash codes
		''' of all elements of information used in <code>equals</code> comparisons
		''' (ie: its <i>composite type</i> and all the item values).
		''' <p>
		''' This ensures that <code> t1.equals(t2) </code> implies that <code> t1.hashCode()==t2.hashCode() </code>
		''' for any two <code>CompositeData</code> instances <code>t1</code> and <code>t2</code>,
		''' as required by the general contract of the method
		''' <seealso cref="Object#hashCode() Object.hashCode()"/>.
		''' <p>
		''' Each item value's hash code is added to the returned hash code.
		''' If an item value is an array,
		''' its hash code is obtained as if by calling the
		''' <seealso cref="java.util.Arrays#deepHashCode(Object[]) deepHashCode"/> method
		''' for arrays of object reference types or the appropriate overloading
		''' of {@code Arrays.hashCode(e)} for arrays of primitive types.
		''' </summary>
		''' <returns> the hash code value for this <code>CompositeData</code> instance </returns>
		Function GetHashCode() As Integer

		''' <summary>
		''' Returns a string representation of this <code>CompositeData</code> instance.
		''' <p>
		''' The string representation consists of the name of the implementing class,
		''' the string representation of the composite type of this instance, and the string representation of the contents
		''' (ie list the itemName=itemValue mappings).
		''' </summary>
		''' <returns>  a string representation of this <code>CompositeData</code> instance </returns>
		Function ToString() As String

	End Interface

End Namespace
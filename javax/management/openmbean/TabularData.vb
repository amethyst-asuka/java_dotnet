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
	''' The <tt>TabularData</tt> interface specifies the behavior of a specific type of complex <i>open data</i> objects
	''' which represent <i>tabular data</i> structures.
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface TabularData 'extends Map


		' *** TabularData specific information methods *** 


		''' <summary>
		''' Returns the <i>tabular type</i> describing this
		''' <tt>TabularData</tt> instance.
		''' </summary>
		''' <returns> the tabular type. </returns>
		ReadOnly Property tabularType As TabularType


		''' <summary>
		''' Calculates the index that would be used in this <tt>TabularData</tt> instance to refer to the specified
		''' composite data <var>value</var> parameter if it were added to this instance.
		''' This method checks for the type validity of the specified <var>value</var>,
		''' but does not check if the calculated index is already used to refer to a value in this <tt>TabularData</tt> instance.
		''' </summary>
		''' <param name="value">                      the composite data value whose index in this
		'''                                    <tt>TabularData</tt> instance is to be calculated;
		'''                                    must be of the same composite type as this instance's row type;
		'''                                    must not be null.
		''' </param>
		''' <returns> the index that the specified <var>value</var> would have in this <tt>TabularData</tt> instance.
		''' </returns>
		''' <exception cref="NullPointerException">       if <var>value</var> is <tt>null</tt>
		''' </exception>
		''' <exception cref="InvalidOpenTypeException">   if <var>value</var> does not conform to this <tt>TabularData</tt> instance's
		'''                                    row type definition. </exception>
		Function calculateIndex(ByVal value As CompositeData) As Object()




		' *** Content information query methods *** 

		''' <summary>
		''' Returns the number of <tt>CompositeData</tt> values (ie the
		''' number of rows) contained in this <tt>TabularData</tt>
		''' instance.
		''' </summary>
		''' <returns> the number of values contained. </returns>
		Function size() As Integer

		''' <summary>
		''' Returns <tt>true</tt> if the number of <tt>CompositeData</tt>
		''' values (ie the number of rows) contained in this
		''' <tt>TabularData</tt> instance is zero.
		''' </summary>
		''' <returns> true if this <tt>TabularData</tt> is empty. </returns>
		ReadOnly Property empty As Boolean

		''' <summary>
		''' Returns <tt>true</tt> if and only if this <tt>TabularData</tt> instance contains a <tt>CompositeData</tt> value
		''' (ie a row) whose index is the specified <var>key</var>. If <var>key</var> is <tt>null</tt> or does not conform to
		''' this <tt>TabularData</tt> instance's <tt>TabularType</tt> definition, this method simply returns <tt>false</tt>.
		''' </summary>
		''' <param name="key">  the index value whose presence in this <tt>TabularData</tt> instance is to be tested.
		''' </param>
		''' <returns>  <tt>true</tt> if this <tt>TabularData</tt> indexes a row value with the specified key. </returns>
		Function containsKey(ByVal key As Object()) As Boolean

		''' <summary>
		''' Returns <tt>true</tt> if and only if this <tt>TabularData</tt> instance contains the specified
		''' <tt>CompositeData</tt> value. If <var>value</var> is <tt>null</tt> or does not conform to
		''' this <tt>TabularData</tt> instance's row type definition, this method simply returns <tt>false</tt>.
		''' </summary>
		''' <param name="value">  the row value whose presence in this <tt>TabularData</tt> instance is to be tested.
		''' </param>
		''' <returns>  <tt>true</tt> if this <tt>TabularData</tt> instance contains the specified row value. </returns>
		Function containsValue(ByVal value As CompositeData) As Boolean

		''' <summary>
		''' Returns the <tt>CompositeData</tt> value whose index is
		''' <var>key</var>, or <tt>null</tt> if there is no value mapping
		''' to <var>key</var>, in this <tt>TabularData</tt> instance.
		''' </summary>
		''' <param name="key"> the key of the row to return.
		''' </param>
		''' <returns> the value corresponding to <var>key</var>.
		''' </returns>
		''' <exception cref="NullPointerException"> if the <var>key</var> is
		''' <tt>null</tt> </exception>
		''' <exception cref="InvalidKeyException"> if the <var>key</var> does not
		''' conform to this <tt>TabularData</tt> instance's *
		''' <tt>TabularType</tt> definition </exception>
		Function [get](ByVal key As Object()) As CompositeData




		' *** Content modification operations (one element at a time) *** 


		''' <summary>
		''' Adds <var>value</var> to this <tt>TabularData</tt> instance.
		''' The composite type of <var>value</var> must be the same as this
		''' instance's row type (ie the composite type returned by
		''' <tt>this.getTabularType().{@link TabularType#getRowType
		''' getRowType()}</tt>), and there must not already be an existing
		''' value in this <tt>TabularData</tt> instance whose index is the
		''' same as the one calculated for the <var>value</var> to be
		''' added. The index for <var>value</var> is calculated according
		''' to this <tt>TabularData</tt> instance's <tt>TabularType</tt>
		''' definition (see <tt>TabularType.{@link
		''' TabularType#getIndexNames getIndexNames()}</tt>).
		''' </summary>
		''' <param name="value">                      the composite data value to be added as a new row to this <tt>TabularData</tt> instance;
		'''                                    must be of the same composite type as this instance's row type;
		'''                                    must not be null.
		''' </param>
		''' <exception cref="NullPointerException">       if <var>value</var> is <tt>null</tt> </exception>
		''' <exception cref="InvalidOpenTypeException">   if <var>value</var> does not conform to this <tt>TabularData</tt> instance's
		'''                                    row type definition. </exception>
		''' <exception cref="KeyAlreadyExistsException">  if the index for <var>value</var>, calculated according to
		'''                                    this <tt>TabularData</tt> instance's <tt>TabularType</tt> definition
		'''                                    already maps to an existing value in the underlying HashMap. </exception>
		Sub put(ByVal value As CompositeData)

		''' <summary>
		''' Removes the <tt>CompositeData</tt> value whose index is <var>key</var> from this <tt>TabularData</tt> instance,
		''' and returns the removed value, or returns <tt>null</tt> if there is no value whose index is <var>key</var>.
		''' </summary>
		''' <param name="key">  the index of the value to get in this <tt>TabularData</tt> instance;
		'''              must be valid with this <tt>TabularData</tt> instance's row type definition;
		'''              must not be null.
		''' </param>
		''' <returns> previous value associated with specified key, or <tt>null</tt>
		'''         if there was no mapping for key.
		''' </returns>
		''' <exception cref="NullPointerException">  if the <var>key</var> is <tt>null</tt> </exception>
		''' <exception cref="InvalidKeyException">   if the <var>key</var> does not conform to this <tt>TabularData</tt> instance's
		'''                               <tt>TabularType</tt> definition </exception>
		Function remove(ByVal key As Object()) As CompositeData




		' ***   Content modification bulk operations   *** 


		''' <summary>
		''' Add all the elements in <var>values</var> to this <tt>TabularData</tt> instance.
		''' If any  element in <var>values</var> does not satisfy the constraints defined in <seealso cref="#put(CompositeData) <tt>put</tt>"/>,
		''' or if any two elements in <var>values</var> have the same index calculated according to this <tt>TabularData</tt>
		''' instance's <tt>TabularType</tt> definition, then an exception describing the failure is thrown
		''' and no element of <var>values</var> is added,  thus leaving this <tt>TabularData</tt> instance unchanged.
		''' </summary>
		''' <param name="values">  the array of composite data values to be added as new rows to this <tt>TabularData</tt> instance;
		'''                 if <var>values</var> is <tt>null</tt> or empty, this method returns without doing anything.
		''' </param>
		''' <exception cref="NullPointerException">       if an element of <var>values</var> is <tt>null</tt> </exception>
		''' <exception cref="InvalidOpenTypeException">   if an element of <var>values</var> does not conform to
		'''                                    this <tt>TabularData</tt> instance's row type definition </exception>
		''' <exception cref="KeyAlreadyExistsException">  if the index for an element of <var>values</var>, calculated according to
		'''                                    this <tt>TabularData</tt> instance's <tt>TabularType</tt> definition
		'''                                    already maps to an existing value in this instance,
		'''                                    or two elements of <var>values</var> have the same index. </exception>
		Sub putAll(ByVal values As CompositeData())

		''' <summary>
		''' Removes all <tt>CompositeData</tt> values (ie rows) from this <tt>TabularData</tt> instance.
		''' </summary>
		Sub clear()




		' ***   Collection views of the keys and values   *** 


		''' <summary>
		''' Returns a set view of the keys (ie the index values) of the
		''' {@code CompositeData} values (ie the rows) contained in this
		''' {@code TabularData} instance. The returned {@code Set} is a
		''' {@code Set<List<?>>} but is declared as a {@code Set<?>} for
		''' compatibility reasons. The returned set can be used to iterate
		''' over the keys.
		''' </summary>
		''' <returns> a set view ({@code Set<List<?>>}) of the index values
		''' used in this {@code TabularData} instance. </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Function keySet() As java.util.Set(Of ?)

		''' <summary>
		''' Returns a collection view of the {@code CompositeData} values
		''' (ie the rows) contained in this {@code TabularData} instance.
		''' The returned {@code Collection} is a {@code Collection<CompositeData>}
		''' but is declared as a {@code Collection<?>} for compatibility reasons.
		''' The returned collection can be used to iterate over the values.
		''' </summary>
		''' <returns> a collection view ({@code Collection<CompositeData>})
		''' of the rows contained in this {@code TabularData} instance. </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Function values() As ICollection(Of ?)




		' ***  Commodity methods from java.lang.Object  *** 


		''' <summary>
		''' Compares the specified <var>obj</var> parameter with this <code>TabularData</code> instance for equality.
		''' <p>
		''' Returns <tt>true</tt> if and only if all of the following statements are true:
		''' <ul>
		''' <li><var>obj</var> is non null,</li>
		''' <li><var>obj</var> also implements the <code>TabularData</code> interface,</li>
		''' <li>their row types are equal</li>
		''' <li>their contents (ie index to value mappings) are equal</li>
		''' </ul>
		''' This ensures that this <tt>equals</tt> method works properly for <var>obj</var> parameters which are
		''' different implementations of the <code>TabularData</code> interface.
		''' <br>&nbsp; </summary>
		''' <param name="obj">  the object to be compared for equality with this <code>TabularData</code> instance;
		''' </param>
		''' <returns>  <code>true</code> if the specified object is equal to this <code>TabularData</code> instance. </returns>
		Function Equals(ByVal obj As Object) As Boolean

		''' <summary>
		''' Returns the hash code value for this <code>TabularData</code> instance.
		''' <p>
		''' The hash code of a <code>TabularData</code> instance is the sum of the hash codes
		''' of all elements of information used in <code>equals</code> comparisons
		''' (ie: its <i>tabular type</i> and its content, where the content is defined as all the index to value mappings).
		''' <p>
		''' This ensures that <code> t1.equals(t2) </code> implies that <code> t1.hashCode()==t2.hashCode() </code>
		''' for any two <code>TabularDataSupport</code> instances <code>t1</code> and <code>t2</code>,
		''' as required by the general contract of the method
		''' <seealso cref="Object#hashCode() Object.hashCode()"/>.
		''' </summary>
		''' <returns>  the hash code value for this <code>TabularDataSupport</code> instance </returns>
		Function GetHashCode() As Integer

		''' <summary>
		''' Returns a string representation of this <code>TabularData</code> instance.
		''' <p>
		''' The string representation consists of the name of the implementing class,
		''' and the tabular type of this instance.
		''' </summary>
		''' <returns>  a string representation of this <code>TabularData</code> instance </returns>
		Function ToString() As String

	End Interface

End Namespace
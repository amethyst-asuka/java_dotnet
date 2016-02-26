Imports System

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.print.attribute

	''' <summary>
	''' Interface AttributeSet specifies the interface for a set of printing
	''' attributes. A printing attribute is an object whose class implements
	''' interface <seealso cref="Attribute Attribute"/>.
	''' <P>
	''' An attribute set contains a group of <I>attribute values,</I>
	''' where duplicate values are not allowed in the set.
	''' Furthermore, each value in an attribute set is
	''' a member of some <I>category,</I> and at most one value in any particular
	''' category is allowed in the set. For an attribute set, the values are {@link
	''' Attribute Attribute} objects, and the categories are {@link java.lang.Class
	''' Class} objects. An attribute's category is the class (or interface) at the
	''' root of the class hierarchy for that kind of attribute. Note that an
	''' attribute object's category may be a superclass of the attribute object's
	''' class rather than the attribute object's class itself. An attribute
	''' object's
	''' category is determined by calling the {@link Attribute#getCategory()
	''' getCategory()} method defined in interface {@link Attribute
	''' Attribute}.
	''' <P>
	''' The interfaces of an AttributeSet resemble those of the Java Collections
	''' API's java.util.Map interface, but is more restrictive in the types
	''' it will accept, and combines keys and values into an Attribute.
	''' <P>
	''' Attribute sets are used in several places in the Print Service API. In
	''' each context, only certain kinds of attributes are allowed to appear in the
	''' attribute set, as determined by the tagging interfaces which the attribute
	''' class implements -- <seealso cref="DocAttribute DocAttribute"/>, {@link
	''' PrintRequestAttribute PrintRequestAttribute}, {@link PrintJobAttribute
	''' PrintJobAttribute}, and {@link PrintServiceAttribute
	''' PrintServiceAttribute}.
	''' There are four specializations of an attribute set that are restricted to
	''' contain just one of the four kinds of attribute -- {@link DocAttributeSet
	''' DocAttributeSet}, {@link PrintRequestAttributeSet
	''' PrintRequestAttributeSet},
	''' <seealso cref="PrintJobAttributeSet PrintJobAttributeSet"/>, and {@link
	''' PrintServiceAttributeSet PrintServiceAttributeSet}, respectively. Note that
	''' many attribute classes implement more than one tagging interface and so may
	''' appear in more than one context.
	''' <UL>
	''' <LI>
	''' A <seealso cref="DocAttributeSet DocAttributeSet"/>, containing {@link DocAttribute
	''' DocAttribute}s, specifies the characteristics of an individual doc and the
	''' print job settings to be applied to an individual doc.
	''' <P>
	''' <LI>
	''' A <seealso cref="PrintRequestAttributeSet PrintRequestAttributeSet"/>, containing
	''' <seealso cref="PrintRequestAttribute PrintRequestAttribute"/>s, specifies the
	''' settings
	''' to be applied to a whole print job and to all the docs in the print job.
	''' <P>
	''' <LI>
	''' A <seealso cref="PrintJobAttributeSet PrintJobAttributeSet"/>, containing {@link
	''' PrintJobAttribute PrintJobAttribute}s, reports the status of a print job.
	''' <P>
	''' <LI>
	''' A <seealso cref="PrintServiceAttributeSet PrintServiceAttributeSet"/>, containing
	''' <seealso cref="PrintServiceAttribute PrintServiceAttribute"/>s, reports the status of
	'''  a Print Service instance.
	''' </UL>
	''' <P>
	''' In some contexts, the client is only allowed to examine an attribute set's
	''' contents but not change them (the set is read-only). In other places, the
	''' client is allowed both to examine and to change an attribute set's contents
	''' (the set is read-write). For a read-only attribute set, calling a mutating
	''' operation throws an UnmodifiableSetException.
	''' <P>
	''' The Print Service API provides one implementation of interface
	''' AttributeSet, class <seealso cref="HashAttributeSet HashAttributeSet"/>.
	''' A client can use class {@link
	''' HashAttributeSet HashAttributeSet} or provide its own implementation of
	''' interface AttributeSet. The Print Service API also provides
	''' implementations of interface AttributeSet's subinterfaces -- classes {@link
	''' HashDocAttributeSet HashDocAttributeSet},
	''' {@link HashPrintRequestAttributeSet
	''' HashPrintRequestAttributeSet}, {@link HashPrintJobAttributeSet
	''' HashPrintJobAttributeSet}, and {@link HashPrintServiceAttributeSet
	''' HashPrintServiceAttributeSet}.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public Interface AttributeSet


		''' <summary>
		''' Returns the attribute value which this attribute set contains in the
		''' given attribute category. Returns <tt>null</tt> if this attribute set
		''' does not contain any attribute value in the given attribute category.
		''' </summary>
		''' <param name="category">  Attribute category whose associated attribute value
		'''                   is to be returned. It must be a
		'''                   <seealso cref="java.lang.Class Class"/>
		'''                   that implements interface {@link Attribute
		'''                   Attribute}.
		''' </param>
		''' <returns>  The attribute value in the given attribute category contained
		'''          in this attribute set, or <tt>null</tt> if this attribute set
		'''          does not contain any attribute value in the given attribute
		'''          category.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if the <CODE>category</CODE> is null. </exception>
		''' <exception cref="ClassCastException">
		'''     (unchecked exception) Thrown if the <CODE>category</CODE> is not a
		'''     <seealso cref="java.lang.Class Class"/> that implements interface {@link
		'''     Attribute Attribute}. </exception>
		Function [get](ByVal category As Type) As Attribute

		''' <summary>
		''' Adds the specified attribute to this attribute set if it is not
		''' already present, first removing any existing value in the same
		''' attribute category as the specified attribute value.
		''' </summary>
		''' <param name="attribute">  Attribute value to be added to this attribute set.
		''' </param>
		''' <returns>  <tt>true</tt> if this attribute set changed as a result of the
		'''          call, i.e., the given attribute value was not already a member
		'''          of this attribute set.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if the <CODE>attribute</CODE> is null. </exception>
		''' <exception cref="UnmodifiableSetException">
		'''     (unchecked exception) Thrown if this attribute set does not support
		'''     the <CODE>add()</CODE> operation. </exception>
		Function add(ByVal attribute As Attribute) As Boolean


		''' <summary>
		''' Removes any attribute for this category from this attribute set if
		''' present. If <CODE>category</CODE> is null, then
		''' <CODE>remove()</CODE> does nothing and returns <tt>false</tt>.
		''' </summary>
		''' <param name="category"> Attribute category to be removed from this
		'''                  attribute set.
		''' </param>
		''' <returns>  <tt>true</tt> if this attribute set changed as a result of the
		'''         call, i.e., the given attribute value had been a member of this
		'''          attribute set.
		''' </returns>
		''' <exception cref="UnmodifiableSetException">
		'''     (unchecked exception) Thrown if this attribute set does not support
		'''     the <CODE>remove()</CODE> operation. </exception>
		Function remove(ByVal category As Type) As Boolean

		''' <summary>
		''' Removes the specified attribute from this attribute set if
		''' present. If <CODE>attribute</CODE> is null, then
		''' <CODE>remove()</CODE> does nothing and returns <tt>false</tt>.
		''' </summary>
		''' <param name="attribute"> Attribute value to be removed from this attribute set.
		''' </param>
		''' <returns>  <tt>true</tt> if this attribute set changed as a result of the
		'''         call, i.e., the given attribute value had been a member of this
		'''          attribute set.
		''' </returns>
		''' <exception cref="UnmodifiableSetException">
		'''     (unchecked exception) Thrown if this attribute set does not support
		'''     the <CODE>remove()</CODE> operation. </exception>
		Function remove(ByVal attribute As Attribute) As Boolean

		''' <summary>
		''' Returns <tt>true</tt> if this attribute set contains an
		''' attribute for the specified category.
		''' </summary>
		''' <param name="category"> whose presence in this attribute set is
		'''            to be tested.
		''' </param>
		''' <returns>  <tt>true</tt> if this attribute set contains an attribute
		'''         value for the specified category. </returns>
		Function containsKey(ByVal category As Type) As Boolean

		''' <summary>
		''' Returns <tt>true</tt> if this attribute set contains the given
		''' attribute value.
		''' </summary>
		''' <param name="attribute">  Attribute value whose presence in this
		''' attribute set is to be tested.
		''' </param>
		''' <returns>  <tt>true</tt> if this attribute set contains the given
		'''      attribute  value. </returns>
		Function containsValue(ByVal attribute As Attribute) As Boolean

		''' <summary>
		''' Adds all of the elements in the specified set to this attribute.
		''' The outcome is the same as if the =
		''' <seealso cref="#add(Attribute) add(Attribute)"/>
		''' operation had been applied to this attribute set successively with each
		''' element from the specified set.
		''' The behavior of the <CODE>addAll(AttributeSet)</CODE>
		''' operation is unspecified if the specified set is modified while
		''' the operation is in progress.
		''' <P>
		''' If the <CODE>addAll(AttributeSet)</CODE> operation throws an exception,
		''' the effect on this attribute set's state is implementation dependent;
		''' elements from the specified set before the point of the exception may
		''' or may not have been added to this attribute set.
		''' </summary>
		''' <param name="attributes">  whose elements are to be added to this attribute
		'''            set.
		''' </param>
		''' <returns>  <tt>true</tt> if this attribute set changed as a result of the
		'''          call.
		''' </returns>
		''' <exception cref="UnmodifiableSetException">
		'''     (Unchecked exception) Thrown if this attribute set does not support
		'''     the <tt>addAll(AttributeSet)</tt> method. </exception>
		''' <exception cref="NullPointerException">
		'''     (Unchecked exception) Thrown if some element in the specified
		'''     set is null.
		''' </exception>
		''' <seealso cref= #add(Attribute) </seealso>
		Function addAll(ByVal attributes As AttributeSet) As Boolean

		''' <summary>
		''' Returns the number of attributes in this attribute set. If this
		''' attribute set contains more than <tt>Integer.MAX_VALUE</tt> elements,
		''' returns  <tt>Integer.MAX_VALUE</tt>.
		''' </summary>
		''' <returns>  The number of attributes in this attribute set. </returns>
		Function size() As Integer

		''' <summary>
		''' Returns an array of the attributes contained in this set. </summary>
		''' <returns> the Attributes contained in this set as an array, zero length
		''' if the AttributeSet is empty. </returns>
		Function toArray() As Attribute()


		''' <summary>
		''' Removes all attributes from this attribute set.
		''' </summary>
		''' <exception cref="UnmodifiableSetException">
		'''   (unchecked exception) Thrown if this attribute set does not support
		'''     the <CODE>clear()</CODE> operation. </exception>
		Sub clear()

		''' <summary>
		''' Returns true if this attribute set contains no attributes.
		''' </summary>
		''' <returns> true if this attribute set contains no attributes. </returns>
		ReadOnly Property empty As Boolean

		''' <summary>
		''' Compares the specified object with this attribute set for equality.
		''' Returns <tt>true</tt> if the given object is also an attribute set and
		''' the two attribute sets contain the same attribute category-attribute
		''' value mappings. This ensures that the
		''' <tt>equals()</tt> method works properly across different
		''' implementations of the AttributeSet interface.
		''' </summary>
		''' <param name="object"> to be compared for equality with this attribute set.
		''' </param>
		''' <returns>  <tt>true</tt> if the specified object is equal to this
		'''       attribute   set. </returns>
		Function Equals(ByVal [object] As Object) As Boolean

		''' <summary>
		''' Returns the hash code value for this attribute set. The hash code of an
		''' attribute set is defined to be the sum of the hash codes of each entry
		''' in the AttributeSet.
		''' This ensures that <tt>t1.equals(t2)</tt> implies that
		''' <tt>t1.hashCode()==t2.hashCode()</tt> for any two attribute sets
		''' <tt>t1</tt> and <tt>t2</tt>, as required by the general contract of
		''' <seealso cref="java.lang.Object#hashCode() Object.hashCode()"/>.
		''' </summary>
		''' <returns>  The hash code value for this attribute set. </returns>
		Function GetHashCode() As Integer

	End Interface

End Namespace
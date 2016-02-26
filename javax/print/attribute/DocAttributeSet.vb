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
	''' Interface DocAttributeSet specifies the interface for a set of doc
	''' attributes, i.e. printing attributes that implement interface {@link
	''' DocAttribute DocAttribute}. In the Print Service API, the client uses a
	''' DocAttributeSet to specify the characteristics of an individual doc and
	''' the print job settings to be applied to an individual doc.
	''' <P>
	''' A DocAttributeSet is just an <seealso cref="AttributeSet AttributeSet"/> whose
	''' constructors and mutating operations guarantee an additional invariant,
	''' namely that all attribute values in the DocAttributeSet must be instances
	''' of interface <seealso cref="DocAttribute DocAttribute"/>.
	''' The <seealso cref="#add(Attribute) add(Attribute)"/>, and
	''' <seealso cref="#addAll(AttributeSet) addAll(AttributeSet)"/> operations
	''' are respecified below to guarantee this additional invariant.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public Interface DocAttributeSet
		Inherits AttributeSet


		''' <summary>
		''' Adds the specified attribute value to this attribute set if it is not
		''' already present, first removing any existing value in the same
		''' attribute category as the specified attribute value (optional
		''' operation).
		''' </summary>
		''' <param name="attribute">  Attribute value to be added to this attribute set.
		''' </param>
		''' <returns>  <tt>true</tt> if this attribute set changed as a result of
		'''          the call, i.e., the given attribute value was not already a
		'''          member of this attribute set.
		''' </returns>
		''' <exception cref="UnmodifiableSetException">
		'''     (unchecked exception) Thrown if this attribute set does not
		'''     support the <CODE>add()</CODE> operation. </exception>
		''' <exception cref="ClassCastException">
		'''     (unchecked exception) Thrown if the <CODE>attribute</CODE> is
		'''     not an instance of interface
		'''     <seealso cref="DocAttribute DocAttribute"/>. </exception>
		''' <exception cref="NullPointerException">
		'''    (unchecked exception) Thrown if the <CODE>attribute</CODE> is null. </exception>
		Function add(ByVal attribute As Attribute) As Boolean

		''' <summary>
		''' Adds all of the elements in the specified set to this attribute.
		''' The outcome is  the same as if the
		''' <seealso cref="#add(Attribute) add(Attribute)"/>
		''' operation had been applied to this attribute set successively with
		''' each element from the specified set. If none of the categories in the
		''' specified set  are the same as any categories in this attribute set,
		''' the <tt>addAll()</tt> operation effectively modifies this attribute
		''' set so that its value is the <i>union</i> of the two sets.
		''' <P>
		''' The behavior of the <CODE>addAll()</CODE> operation is unspecified if
		''' the specified set is modified while the operation is in progress.
		''' <P>
		''' If the <CODE>addAll()</CODE> operation throws an exception, the effect
		''' on this attribute set's state is implementation dependent; elements
		''' from the specified set before the point of the exception may or
		''' may not have been added to this attribute set.
		''' </summary>
		''' <param name="attributes">  whose elements are to be added to this attribute
		'''            set.
		''' </param>
		''' <returns>  <tt>true</tt> if this attribute set changed as a result of
		'''          the call.
		''' </returns>
		''' <exception cref="UnmodifiableSetException">
		'''     (Unchecked exception) Thrown if this attribute set does not
		'''     support the <tt>addAll()</tt> method. </exception>
		''' <exception cref="ClassCastException">
		'''     (Unchecked exception) Thrown if some element in the specified
		'''     set is not an instance of interface {@link DocAttribute
		'''     DocAttribute}. </exception>
		''' <exception cref="NullPointerException">
		'''     (Unchecked exception) Thrown if the specified  set is null.
		''' </exception>
		''' <seealso cref= #add(Attribute) </seealso>
	   Function addAll(ByVal attributes As AttributeSet) As Boolean
	End Interface

End Namespace
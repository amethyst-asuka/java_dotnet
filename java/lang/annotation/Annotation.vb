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

Namespace java.lang.annotation

	''' <summary>
	''' The common interface extended by all annotation types.  Note that an
	''' interface that manually extends this one does <i>not</i> define
	''' an annotation type.  Also note that this interface does not itself
	''' define an annotation type.
	''' 
	''' More information about annotation types can be found in section 9.6 of
	''' <cite>The Java&trade; Language Specification</cite>.
	''' 
	''' The <seealso cref="java.lang.reflect.AnnotatedElement"/> interface discusses
	''' compatibility concerns when evolving an annotation type from being
	''' non-repeatable to being repeatable.
	''' 
	''' @author  Josh Bloch
	''' @since   1.5
	''' </summary>
	Public Interface Annotation
		''' <summary>
		''' Returns true if the specified object represents an annotation
		''' that is logically equivalent to this one.  In other words,
		''' returns true if the specified object is an instance of the same
		''' annotation type as this instance, all of whose members are equal
		''' to the corresponding member of this annotation, as defined below:
		''' <ul>
		'''    <li>Two corresponding primitive typed members whose values are
		'''    <tt>x</tt> and <tt>y</tt> are considered equal if <tt>x == y</tt>,
		'''    unless their type is <tt>float</tt> or <tt>double</tt>.
		''' 
		'''    <li>Two corresponding <tt>float</tt> members whose values
		'''    are <tt>x</tt> and <tt>y</tt> are considered equal if
		'''    <tt>Float.valueOf(x).equals(Float.valueOf(y))</tt>.
		'''    (Unlike the <tt>==</tt> operator, NaN is considered equal
		'''    to itself, and <tt>0.0f</tt> unequal to <tt>-0.0f</tt>.)
		''' 
		'''    <li>Two corresponding <tt>double</tt> members whose values
		'''    are <tt>x</tt> and <tt>y</tt> are considered equal if
		'''    <tt>Double.valueOf(x).equals(Double.valueOf(y))</tt>.
		'''    (Unlike the <tt>==</tt> operator, NaN is considered equal
		'''    to itself, and <tt>0.0</tt> unequal to <tt>-0.0</tt>.)
		''' 
		'''    <li>Two corresponding <tt>String</tt>, <tt>Class</tt>, enum, or
		'''    annotation typed members whose values are <tt>x</tt> and <tt>y</tt>
		'''    are considered equal if <tt>x.equals(y)</tt>.  (Note that this
		'''    definition is recursive for annotation typed members.)
		''' 
		'''    <li>Two corresponding array typed members <tt>x</tt> and <tt>y</tt>
		'''    are considered equal if <tt>Arrays.equals(x, y)</tt>, for the
		'''    appropriate overloading of <seealso cref="java.util.Arrays#equals"/>.
		''' </ul>
		''' </summary>
		''' <returns> true if the specified object represents an annotation
		'''     that is logically equivalent to this one, otherwise false </returns>
		Function Equals(  obj As Object) As Boolean

		''' <summary>
		''' Returns the hash code of this annotation, as defined below:
		''' 
		''' <p>The hash code of an annotation is the sum of the hash codes
		''' of its members (including those with default values), as defined
		''' below:
		''' 
		''' The hash code of an annotation member is (127 times the hash code
		''' of the member-name as computed by <seealso cref="String#hashCode()"/>) XOR
		''' the hash code of the member-value, as defined below:
		''' 
		''' <p>The hash code of a member-value depends on its type:
		''' <ul>
		''' <li>The hash code of a primitive value <tt><i>v</i></tt> is equal to
		'''     <tt><i>WrapperType</i>.valueOf(<i>v</i>).hashCode()</tt>, where
		'''     <tt><i>WrapperType</i></tt> is the wrapper type corresponding
		'''     to the primitive type of <tt><i>v</i></tt> (<seealso cref="Byte"/>,
		'''     <seealso cref="Character"/>, <seealso cref="Double"/>, <seealso cref="Float"/>, <seealso cref="Integer"/>,
		'''     <seealso cref="Long"/>, <seealso cref="Short"/>, or <seealso cref="Boolean"/>).
		''' 
		''' <li>The hash code of a string, enum, [Class], or annotation member-value
		''' I     <tt><i>v</i></tt> is computed as by calling
		'''     <tt><i>v</i>.hashCode()</tt>.  (In the case of annotation
		'''     member values, this is a recursive definition.)
		''' 
		''' <li>The hash code of an array member-value is computed by calling
		'''     the appropriate overloading of
		'''     <seealso cref="java.util.Arrays#hashCode(long[]) Arrays.hashCode"/>
		'''     on the value.  (There is one overloading for each primitive
		'''     type, and one for object reference types.)
		''' </ul>
		''' </summary>
		''' <returns> the hash code of this annotation </returns>
		Function GetHashCode() As Integer

		''' <summary>
		''' Returns a string representation of this annotation.  The details
		''' of the representation are implementation-dependent, but the following
		''' may be regarded as typical:
		''' <pre>
		'''   &#064;com.acme.util.Name(first=Alfred, middle=E., last=Neuman)
		''' </pre>
		''' </summary>
		''' <returns> a string representation of this annotation </returns>
		Function ToString() As String

        ''' <summary>
        ''' Returns the annotation type of this annotation. </summary>
        ''' <returns> the annotation type of this annotation </returns>
        Function annotationType() As [Class]
    End Interface

End Namespace
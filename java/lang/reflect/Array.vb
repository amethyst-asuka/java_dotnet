Imports System
Imports System.Runtime.InteropServices

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.reflect

	''' <summary>
	''' The {@code Array} class provides static methods to dynamically create and
	''' access Java arrays.
	''' 
	''' <p>{@code Array} permits widening conversions to occur during a get or set
	''' operation, but throws an {@code IllegalArgumentException} if a narrowing
	''' conversion would occur.
	''' 
	''' @author Nakul Saraiya
	''' </summary>
	Public NotInheritable Class Array

		''' <summary>
		''' Constructor.  Class Array is not instantiable.
		''' </summary>
		Private Sub New()
		End Sub

		''' <summary>
		''' Creates a new array with the specified component type and
		''' length.
		''' Invoking this method is equivalent to creating an array
		''' as follows:
		''' <blockquote>
		''' <pre>
		''' int[] x = {length};
		''' Array.newInstance(componentType, x);
		''' </pre>
		''' </blockquote>
		''' 
		''' <p>The number of dimensions of the new array must not
		''' exceed 255.
		''' </summary>
		''' <param name="componentType"> the {@code Class} object representing the
		''' component type of the new array </param>
		''' <param name="length"> the length of the new array </param>
		''' <returns> the new array </returns>
		''' <exception cref="NullPointerException"> if the specified
		''' {@code componentType} parameter is null </exception>
		''' <exception cref="IllegalArgumentException"> if componentType is {@link
		''' Void#TYPE} or if the number of dimensions of the requested array
		''' instance exceed 255. </exception>
		''' <exception cref="NegativeArraySizeException"> if the specified {@code length}
		''' is negative </exception>
		Public Shared Function newInstance(  componentType As [Class],   length As Integer) As Object
			Return newArray(componentType, length)
		End Function

		''' <summary>
		''' Creates a new array
		''' with the specified component type and dimensions.
		''' If {@code componentType}
		''' represents a non-array class or interface, the new array
		''' has {@code dimensions.length} dimensions and
		''' {@code componentType} as its component type. If
		''' {@code componentType} represents an array [Class], the
		''' number of dimensions of the new array is equal to the sum
		''' of {@code dimensions.length} and the number of
		''' dimensions of {@code componentType}. In this case, the
		''' component type of the new array is the component type of
		''' {@code componentType}.
		''' 
		''' <p>The number of dimensions of the new array must not
		''' exceed 255.
		''' </summary>
		''' <param name="componentType"> the {@code Class} object representing the component
		''' type of the new array </param>
		''' <param name="dimensions"> an array of {@code int} representing the dimensions of
		''' the new array </param>
		''' <returns> the new array </returns>
		''' <exception cref="NullPointerException"> if the specified
		''' {@code componentType} argument is null </exception>
		''' <exception cref="IllegalArgumentException"> if the specified {@code dimensions}
		''' argument is a zero-dimensional array, if componentType is {@link
		''' Void#TYPE}, or if the number of dimensions of the requested array
		''' instance exceed 255. </exception>
		''' <exception cref="NegativeArraySizeException"> if any of the components in
		''' the specified {@code dimensions} argument is negative. </exception>
		Public Shared Function newInstance(  componentType As [Class], ParamArray   dimensions As Integer()) As Object
			Return multiNewArray(componentType, dimensions)
		End Function

		''' <summary>
		''' Returns the length of the specified array object, as an {@code int}.
		''' </summary>
		''' <param name="array"> the array </param>
		''' <returns> the length of the array </returns>
		''' <exception cref="IllegalArgumentException"> if the object argument is not
		''' an array </exception>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function getLength(  array As Object) As Integer
		End Function

		''' <summary>
		''' Returns the value of the indexed component in the specified
		''' array object.  The value is automatically wrapped in an object
		''' if it has a primitive type.
		''' </summary>
		''' <param name="array"> the array </param>
		''' <param name="index"> the index </param>
		''' <returns> the (possibly wrapped) value of the indexed component in
		''' the specified array </returns>
		''' <exception cref="NullPointerException"> If the specified object is null </exception>
		''' <exception cref="IllegalArgumentException"> If the specified object is not
		''' an array </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If the specified {@code index}
		''' argument is negative, or if it is greater than or equal to the
		''' length of the specified array </exception>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function [get](  array As Object,   index As Integer) As Object
		End Function

		''' <summary>
		''' Returns the value of the indexed component in the specified
		''' array object, as a {@code boolean}.
		''' </summary>
		''' <param name="array"> the array </param>
		''' <param name="index"> the index </param>
		''' <returns> the value of the indexed component in the specified array </returns>
		''' <exception cref="NullPointerException"> If the specified object is null </exception>
		''' <exception cref="IllegalArgumentException"> If the specified object is not
		''' an array, or if the indexed element cannot be converted to the
		''' return type by an identity or widening conversion </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If the specified {@code index}
		''' argument is negative, or if it is greater than or equal to the
		''' length of the specified array </exception>
		''' <seealso cref= Array#get </seealso>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function getBoolean(  array As Object,   index As Integer) As Boolean
		End Function

		''' <summary>
		''' Returns the value of the indexed component in the specified
		''' array object, as a {@code byte}.
		''' </summary>
		''' <param name="array"> the array </param>
		''' <param name="index"> the index </param>
		''' <returns> the value of the indexed component in the specified array </returns>
		''' <exception cref="NullPointerException"> If the specified object is null </exception>
		''' <exception cref="IllegalArgumentException"> If the specified object is not
		''' an array, or if the indexed element cannot be converted to the
		''' return type by an identity or widening conversion </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If the specified {@code index}
		''' argument is negative, or if it is greater than or equal to the
		''' length of the specified array </exception>
		''' <seealso cref= Array#get </seealso>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function getByte(  array As Object,   index As Integer) As SByte
		End Function

		''' <summary>
		''' Returns the value of the indexed component in the specified
		''' array object, as a {@code char}.
		''' </summary>
		''' <param name="array"> the array </param>
		''' <param name="index"> the index </param>
		''' <returns> the value of the indexed component in the specified array </returns>
		''' <exception cref="NullPointerException"> If the specified object is null </exception>
		''' <exception cref="IllegalArgumentException"> If the specified object is not
		''' an array, or if the indexed element cannot be converted to the
		''' return type by an identity or widening conversion </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If the specified {@code index}
		''' argument is negative, or if it is greater than or equal to the
		''' length of the specified array </exception>
		''' <seealso cref= Array#get </seealso>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function getChar(  array As Object,   index As Integer) As Char
		End Function

		''' <summary>
		''' Returns the value of the indexed component in the specified
		''' array object, as a {@code short}.
		''' </summary>
		''' <param name="array"> the array </param>
		''' <param name="index"> the index </param>
		''' <returns> the value of the indexed component in the specified array </returns>
		''' <exception cref="NullPointerException"> If the specified object is null </exception>
		''' <exception cref="IllegalArgumentException"> If the specified object is not
		''' an array, or if the indexed element cannot be converted to the
		''' return type by an identity or widening conversion </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If the specified {@code index}
		''' argument is negative, or if it is greater than or equal to the
		''' length of the specified array </exception>
		''' <seealso cref= Array#get </seealso>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function getShort(  array As Object,   index As Integer) As Short
		End Function

		''' <summary>
		''' Returns the value of the indexed component in the specified
		''' array object, as an {@code int}.
		''' </summary>
		''' <param name="array"> the array </param>
		''' <param name="index"> the index </param>
		''' <returns> the value of the indexed component in the specified array </returns>
		''' <exception cref="NullPointerException"> If the specified object is null </exception>
		''' <exception cref="IllegalArgumentException"> If the specified object is not
		''' an array, or if the indexed element cannot be converted to the
		''' return type by an identity or widening conversion </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If the specified {@code index}
		''' argument is negative, or if it is greater than or equal to the
		''' length of the specified array </exception>
		''' <seealso cref= Array#get </seealso>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function getInt(  array As Object,   index As Integer) As Integer
		End Function

		''' <summary>
		''' Returns the value of the indexed component in the specified
		''' array object, as a {@code long}.
		''' </summary>
		''' <param name="array"> the array </param>
		''' <param name="index"> the index </param>
		''' <returns> the value of the indexed component in the specified array </returns>
		''' <exception cref="NullPointerException"> If the specified object is null </exception>
		''' <exception cref="IllegalArgumentException"> If the specified object is not
		''' an array, or if the indexed element cannot be converted to the
		''' return type by an identity or widening conversion </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If the specified {@code index}
		''' argument is negative, or if it is greater than or equal to the
		''' length of the specified array </exception>
		''' <seealso cref= Array#get </seealso>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function getLong(  array As Object,   index As Integer) As Long
		End Function

		''' <summary>
		''' Returns the value of the indexed component in the specified
		''' array object, as a {@code float}.
		''' </summary>
		''' <param name="array"> the array </param>
		''' <param name="index"> the index </param>
		''' <returns> the value of the indexed component in the specified array </returns>
		''' <exception cref="NullPointerException"> If the specified object is null </exception>
		''' <exception cref="IllegalArgumentException"> If the specified object is not
		''' an array, or if the indexed element cannot be converted to the
		''' return type by an identity or widening conversion </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If the specified {@code index}
		''' argument is negative, or if it is greater than or equal to the
		''' length of the specified array </exception>
		''' <seealso cref= Array#get </seealso>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function getFloat(  array As Object,   index As Integer) As Single
		End Function

		''' <summary>
		''' Returns the value of the indexed component in the specified
		''' array object, as a {@code double}.
		''' </summary>
		''' <param name="array"> the array </param>
		''' <param name="index"> the index </param>
		''' <returns> the value of the indexed component in the specified array </returns>
		''' <exception cref="NullPointerException"> If the specified object is null </exception>
		''' <exception cref="IllegalArgumentException"> If the specified object is not
		''' an array, or if the indexed element cannot be converted to the
		''' return type by an identity or widening conversion </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If the specified {@code index}
		''' argument is negative, or if it is greater than or equal to the
		''' length of the specified array </exception>
		''' <seealso cref= Array#get </seealso>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function getDouble(  array As Object,   index As Integer) As Double
		End Function

		''' <summary>
		''' Sets the value of the indexed component of the specified array
		''' object to the specified new value.  The new value is first
		''' automatically unwrapped if the array has a primitive component
		''' type. </summary>
		''' <param name="array"> the array </param>
		''' <param name="index"> the index into the array </param>
		''' <param name="value"> the new value of the indexed component </param>
		''' <exception cref="NullPointerException"> If the specified object argument
		''' is null </exception>
		''' <exception cref="IllegalArgumentException"> If the specified object argument
		''' is not an array, or if the array component type is primitive and
		''' an unwrapping conversion fails </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If the specified {@code index}
		''' argument is negative, or if it is greater than or equal to
		''' the length of the specified array </exception>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Sub [set](  array As Object,   index As Integer,   value As Object)
		End Sub

		''' <summary>
		''' Sets the value of the indexed component of the specified array
		''' object to the specified {@code boolean} value. </summary>
		''' <param name="array"> the array </param>
		''' <param name="index"> the index into the array </param>
		''' <param name="z"> the new value of the indexed component </param>
		''' <exception cref="NullPointerException"> If the specified object argument
		''' is null </exception>
		''' <exception cref="IllegalArgumentException"> If the specified object argument
		''' is not an array, or if the specified value cannot be converted
		''' to the underlying array's component type by an identity or a
		''' primitive widening conversion </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If the specified {@code index}
		''' argument is negative, or if it is greater than or equal to
		''' the length of the specified array </exception>
		''' <seealso cref= Array#set </seealso>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Sub setBoolean(  array As Object,   index As Integer,   z As Boolean)
		End Sub

		''' <summary>
		''' Sets the value of the indexed component of the specified array
		''' object to the specified {@code byte} value. </summary>
		''' <param name="array"> the array </param>
		''' <param name="index"> the index into the array </param>
		''' <param name="b"> the new value of the indexed component </param>
		''' <exception cref="NullPointerException"> If the specified object argument
		''' is null </exception>
		''' <exception cref="IllegalArgumentException"> If the specified object argument
		''' is not an array, or if the specified value cannot be converted
		''' to the underlying array's component type by an identity or a
		''' primitive widening conversion </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If the specified {@code index}
		''' argument is negative, or if it is greater than or equal to
		''' the length of the specified array </exception>
		''' <seealso cref= Array#set </seealso>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Sub setByte(  array As Object,   index As Integer,   b As SByte)
		End Sub

		''' <summary>
		''' Sets the value of the indexed component of the specified array
		''' object to the specified {@code char} value. </summary>
		''' <param name="array"> the array </param>
		''' <param name="index"> the index into the array </param>
		''' <param name="c"> the new value of the indexed component </param>
		''' <exception cref="NullPointerException"> If the specified object argument
		''' is null </exception>
		''' <exception cref="IllegalArgumentException"> If the specified object argument
		''' is not an array, or if the specified value cannot be converted
		''' to the underlying array's component type by an identity or a
		''' primitive widening conversion </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If the specified {@code index}
		''' argument is negative, or if it is greater than or equal to
		''' the length of the specified array </exception>
		''' <seealso cref= Array#set </seealso>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Sub setChar(  array As Object,   index As Integer,   c As Char)
		End Sub

		''' <summary>
		''' Sets the value of the indexed component of the specified array
		''' object to the specified {@code short} value. </summary>
		''' <param name="array"> the array </param>
		''' <param name="index"> the index into the array </param>
		''' <param name="s"> the new value of the indexed component </param>
		''' <exception cref="NullPointerException"> If the specified object argument
		''' is null </exception>
		''' <exception cref="IllegalArgumentException"> If the specified object argument
		''' is not an array, or if the specified value cannot be converted
		''' to the underlying array's component type by an identity or a
		''' primitive widening conversion </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If the specified {@code index}
		''' argument is negative, or if it is greater than or equal to
		''' the length of the specified array </exception>
		''' <seealso cref= Array#set </seealso>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Sub setShort(  array As Object,   index As Integer,   s As Short)
		End Sub

		''' <summary>
		''' Sets the value of the indexed component of the specified array
		''' object to the specified {@code int} value. </summary>
		''' <param name="array"> the array </param>
		''' <param name="index"> the index into the array </param>
		''' <param name="i"> the new value of the indexed component </param>
		''' <exception cref="NullPointerException"> If the specified object argument
		''' is null </exception>
		''' <exception cref="IllegalArgumentException"> If the specified object argument
		''' is not an array, or if the specified value cannot be converted
		''' to the underlying array's component type by an identity or a
		''' primitive widening conversion </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If the specified {@code index}
		''' argument is negative, or if it is greater than or equal to
		''' the length of the specified array </exception>
		''' <seealso cref= Array#set </seealso>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Sub setInt(  array As Object,   index As Integer,   i As Integer)
		End Sub

		''' <summary>
		''' Sets the value of the indexed component of the specified array
		''' object to the specified {@code long} value. </summary>
		''' <param name="array"> the array </param>
		''' <param name="index"> the index into the array </param>
		''' <param name="l"> the new value of the indexed component </param>
		''' <exception cref="NullPointerException"> If the specified object argument
		''' is null </exception>
		''' <exception cref="IllegalArgumentException"> If the specified object argument
		''' is not an array, or if the specified value cannot be converted
		''' to the underlying array's component type by an identity or a
		''' primitive widening conversion </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If the specified {@code index}
		''' argument is negative, or if it is greater than or equal to
		''' the length of the specified array </exception>
		''' <seealso cref= Array#set </seealso>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Sub setLong(  array As Object,   index As Integer,   l As Long)
		End Sub

		''' <summary>
		''' Sets the value of the indexed component of the specified array
		''' object to the specified {@code float} value. </summary>
		''' <param name="array"> the array </param>
		''' <param name="index"> the index into the array </param>
		''' <param name="f"> the new value of the indexed component </param>
		''' <exception cref="NullPointerException"> If the specified object argument
		''' is null </exception>
		''' <exception cref="IllegalArgumentException"> If the specified object argument
		''' is not an array, or if the specified value cannot be converted
		''' to the underlying array's component type by an identity or a
		''' primitive widening conversion </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If the specified {@code index}
		''' argument is negative, or if it is greater than or equal to
		''' the length of the specified array </exception>
		''' <seealso cref= Array#set </seealso>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Sub setFloat(  array As Object,   index As Integer,   f As Single)
		End Sub

		''' <summary>
		''' Sets the value of the indexed component of the specified array
		''' object to the specified {@code double} value. </summary>
		''' <param name="array"> the array </param>
		''' <param name="index"> the index into the array </param>
		''' <param name="d"> the new value of the indexed component </param>
		''' <exception cref="NullPointerException"> If the specified object argument
		''' is null </exception>
		''' <exception cref="IllegalArgumentException"> If the specified object argument
		''' is not an array, or if the specified value cannot be converted
		''' to the underlying array's component type by an identity or a
		''' primitive widening conversion </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If the specified {@code index}
		''' argument is negative, or if it is greater than or equal to
		''' the length of the specified array </exception>
		''' <seealso cref= Array#set </seealso>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Sub setDouble(  array As Object,   index As Integer,   d As Double)
		End Sub

	'    
	'     * Private
	'     

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function newArray(  componentType As [Class],   length As Integer) As Object
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function multiNewArray(  componentType As [Class],   dimensions As Integer()) As Object
		End Function


	End Class

End Namespace
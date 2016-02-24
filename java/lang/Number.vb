Imports System

'
' * Copyright (c) 1994, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang

	''' <summary>
	''' The abstract class {@code Number} is the superclass of platform
	''' classes representing numeric values that are convertible to the
	''' primitive types {@code byte}, {@code double}, {@code float}, {@code
	''' int}, {@code long}, and {@code short}.
	''' 
	''' The specific semantics of the conversion from the numeric value of
	''' a particular {@code Number} implementation to a given primitive
	''' type is defined by the {@code Number} implementation in question.
	''' 
	''' For platform classes, the conversion is often analogous to a
	''' narrowing primitive conversion or a widening primitive conversion
	''' as defining in <cite>The Java&trade; Language Specification</cite>
	''' for converting between primitive types.  Therefore, conversions may
	''' lose information about the overall magnitude of a numeric value, may
	''' lose precision, and may even return a result of a different sign
	''' than the input.
	''' 
	''' See the documentation of a given {@code Number} implementation for
	''' conversion details.
	''' 
	''' @author      Lee Boynton
	''' @author      Arthur van Hoff
	''' @jls 5.1.2 Widening Primitive Conversions
	''' @jls 5.1.3 Narrowing Primitive Conversions
	''' @since   JDK1.0
	''' </summary>
	<Serializable> _
	Public MustInherit Class Number
		''' <summary>
		''' Returns the value of the specified number as an {@code int},
		''' which may involve rounding or truncation.
		''' </summary>
		''' <returns>  the numeric value represented by this object after conversion
		'''          to type {@code int}. </returns>
		Public MustOverride Function intValue() As Integer

		''' <summary>
		''' Returns the value of the specified number as a {@code long},
		''' which may involve rounding or truncation.
		''' </summary>
		''' <returns>  the numeric value represented by this object after conversion
		'''          to type {@code long}. </returns>
		Public MustOverride Function longValue() As Long

		''' <summary>
		''' Returns the value of the specified number as a {@code float},
		''' which may involve rounding.
		''' </summary>
		''' <returns>  the numeric value represented by this object after conversion
		'''          to type {@code float}. </returns>
		Public MustOverride Function floatValue() As Single

		''' <summary>
		''' Returns the value of the specified number as a {@code double},
		''' which may involve rounding.
		''' </summary>
		''' <returns>  the numeric value represented by this object after conversion
		'''          to type {@code double}. </returns>
		Public MustOverride Function doubleValue() As Double

		''' <summary>
		''' Returns the value of the specified number as a {@code byte},
		''' which may involve rounding or truncation.
		''' 
		''' <p>This implementation returns the result of <seealso cref="#intValue"/> cast
		''' to a {@code byte}.
		''' </summary>
		''' <returns>  the numeric value represented by this object after conversion
		'''          to type {@code byte}.
		''' @since   JDK1.1 </returns>
		Public Overridable Function byteValue() As SByte
            Return CSByte(intValue())
        End Function

		''' <summary>
		''' Returns the value of the specified number as a {@code short},
		''' which may involve rounding or truncation.
		''' 
		''' <p>This implementation returns the result of <seealso cref="#intValue"/> cast
		''' to a {@code short}.
		''' </summary>
		''' <returns>  the numeric value represented by this object after conversion
		'''          to type {@code short}.
		''' @since   JDK1.1 </returns>
		Public Overridable Function shortValue() As Short
			Return CShort(intValue())
		End Function

		''' <summary>
		''' use serialVersionUID from JDK 1.0.2 for interoperability </summary>
		Private Const serialVersionUID As Long = -8742448824652078965L
	End Class

End Namespace
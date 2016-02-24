Imports System

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

Namespace java.lang

	''' 
	''' <summary>
	''' The {@code Byte} class wraps a value of primitive type {@code byte}
	''' in an object.  An object of type {@code Byte} contains a single
	''' field whose type is {@code byte}.
	''' 
	''' <p>In addition, this class provides several methods for converting
	''' a {@code byte} to a {@code String} and a {@code String} to a {@code
	''' byte}, as well as other constants and methods useful when dealing
	''' with a {@code byte}.
	''' 
	''' @author  Nakul Saraiya
	''' @author  Joseph D. Darcy </summary>
	''' <seealso cref=     java.lang.Number
	''' @since   JDK1.1 </seealso>
	Public NotInheritable Class [Byte]
		Inherits Number
		Implements Comparable(Of Byte)

		''' <summary>
		''' A constant holding the minimum value a {@code byte} can
		''' have, -2<sup>7</sup>.
		''' </summary>
		Public Const MIN_VALUE As SByte = -128

		''' <summary>
		''' A constant holding the maximum value a {@code byte} can
		''' have, 2<sup>7</sup>-1.
		''' </summary>
		Public Const MAX_VALUE As SByte = 127

		''' <summary>
		''' The {@code Class} instance representing the primitive type
		''' {@code byte}.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared ReadOnly TYPE As Class = CType(Class.getPrimitiveClass("byte"), [Class])

		''' <summary>
		''' Returns a new {@code String} object representing the
		''' specified {@code byte}. The radix is assumed to be 10.
		''' </summary>
		''' <param name="b"> the {@code byte} to be converted </param>
		''' <returns> the string representation of the specified {@code byte} </returns>
		''' <seealso cref= java.lang.Integer#toString(int) </seealso>
		Public Shared Function ToString(ByVal b As SByte) As String
			Return Convert.ToString(CInt(b), 10)
		End Function

		Private Class ByteCache
			Private Sub New()
			End Sub

			Friend Shared ReadOnly cache As Byte() = New Byte(-(-128) + 127){}

			Shared Sub New()
				For i As Integer = 0 To cache.Length - 1
					cache(i) = New Byte(CByte(i - 128))
				Next i
			End Sub
		End Class

		''' <summary>
		''' Returns a {@code Byte} instance representing the specified
		''' {@code byte} value.
		''' If a new {@code Byte} instance is not required, this method
		''' should generally be used in preference to the constructor
		''' <seealso cref="#Byte(byte)"/>, as this method is likely to yield
		''' significantly better space and time performance since
		''' all byte values are cached.
		''' </summary>
		''' <param name="b"> a byte value. </param>
		''' <returns> a {@code Byte} instance representing {@code b}.
		''' @since  1.5 </returns>
		Public Shared Function valueOf(ByVal b As SByte) As Byte
			Const offset As Integer = 128
			Return ByteCache.cache(CInt(b) + offset)
		End Function

		''' <summary>
		''' Parses the string argument as a signed {@code byte} in the
		''' radix specified by the second argument. The characters in the
		''' string must all be digits, of the specified radix (as
		''' determined by whether {@link java.lang.Character#digit(char,
		''' int)} returns a nonnegative value) except that the first
		''' character may be an ASCII minus sign {@code '-'}
		''' ({@code '\u005Cu002D'}) to indicate a negative value or an
		''' ASCII plus sign {@code '+'} ({@code '\u005Cu002B'}) to
		''' indicate a positive value.  The resulting {@code byte} value is
		''' returned.
		''' 
		''' <p>An exception of type {@code NumberFormatException} is
		''' thrown if any of the following situations occurs:
		''' <ul>
		''' <li> The first argument is {@code null} or is a string of
		''' length zero.
		''' 
		''' <li> The radix is either smaller than {@link
		''' java.lang.Character#MIN_RADIX} or larger than {@link
		''' java.lang.Character#MAX_RADIX}.
		''' 
		''' <li> Any character of the string is not a digit of the
		''' specified radix, except that the first character may be a minus
		''' sign {@code '-'} ({@code '\u005Cu002D'}) or plus sign
		''' {@code '+'} ({@code '\u005Cu002B'}) provided that the
		''' string is longer than length 1.
		''' 
		''' <li> The value represented by the string is not a value of type
		''' {@code byte}.
		''' </ul>
		''' </summary>
		''' <param name="s">         the {@code String} containing the
		'''                  {@code byte}
		'''                  representation to be parsed </param>
		''' <param name="radix">     the radix to be used while parsing {@code s} </param>
		''' <returns>          the {@code byte} value represented by the string
		'''                   argument in the specified radix </returns>
		''' <exception cref="NumberFormatException"> If the string does
		'''                  not contain a parsable {@code byte}. </exception>
		Public Shared Function parseByte(ByVal s As String, ByVal radix As Integer) As SByte
			Dim i As Integer = Convert.ToInt32(s, radix)
			If i < MIN_VALUE OrElse i > MAX_VALUE Then Throw New NumberFormatException("Value out of range. Value:""" & s & """ Radix:" & radix)
			Return CByte(i)
		End Function

		''' <summary>
		''' Parses the string argument as a signed decimal {@code
		''' byte}. The characters in the string must all be decimal digits,
		''' except that the first character may be an ASCII minus sign
		''' {@code '-'} ({@code '\u005Cu002D'}) to indicate a negative
		''' value or an ASCII plus sign {@code '+'}
		''' ({@code '\u005Cu002B'}) to indicate a positive value. The
		''' resulting {@code byte} value is returned, exactly as if the
		''' argument and the radix 10 were given as arguments to the {@link
		''' #parseByte(java.lang.String, int)} method.
		''' </summary>
		''' <param name="s">         a {@code String} containing the
		'''                  {@code byte} representation to be parsed </param>
		''' <returns>          the {@code byte} value represented by the
		'''                  argument in decimal </returns>
		''' <exception cref="NumberFormatException"> if the string does not
		'''                  contain a parsable {@code byte}. </exception>
		Public Shared Function parseByte(ByVal s As String) As SByte
			Return parseByte(s, 10)
		End Function

		''' <summary>
		''' Returns a {@code Byte} object holding the value
		''' extracted from the specified {@code String} when parsed
		''' with the radix given by the second argument. The first argument
		''' is interpreted as representing a signed {@code byte} in
		''' the radix specified by the second argument, exactly as if the
		''' argument were given to the {@link #parseByte(java.lang.String,
		''' int)} method. The result is a {@code Byte} object that
		''' represents the {@code byte} value specified by the string.
		''' 
		''' <p> In other words, this method returns a {@code Byte} object
		''' equal to the value of:
		''' 
		''' <blockquote>
		''' {@code new Byte(Byte.parseByte(s, radix))}
		''' </blockquote>
		''' </summary>
		''' <param name="s">         the string to be parsed </param>
		''' <param name="radix">     the radix to be used in interpreting {@code s} </param>
		''' <returns>          a {@code Byte} object holding the value
		'''                  represented by the string argument in the
		'''                  specified radix. </returns>
		''' <exception cref="NumberFormatException"> If the {@code String} does
		'''                  not contain a parsable {@code byte}. </exception>
		Public Shared Function valueOf(ByVal s As String, ByVal radix As Integer) As Byte
			Return valueOf(parseByte(s, radix))
		End Function

		''' <summary>
		''' Returns a {@code Byte} object holding the value
		''' given by the specified {@code String}. The argument is
		''' interpreted as representing a signed decimal {@code byte},
		''' exactly as if the argument were given to the {@link
		''' #parseByte(java.lang.String)} method. The result is a
		''' {@code Byte} object that represents the {@code byte}
		''' value specified by the string.
		''' 
		''' <p> In other words, this method returns a {@code Byte} object
		''' equal to the value of:
		''' 
		''' <blockquote>
		''' {@code new Byte(Byte.parseByte(s))}
		''' </blockquote>
		''' </summary>
		''' <param name="s">         the string to be parsed </param>
		''' <returns>          a {@code Byte} object holding the value
		'''                  represented by the string argument </returns>
		''' <exception cref="NumberFormatException"> If the {@code String} does
		'''                  not contain a parsable {@code byte}. </exception>
		Public Shared Function valueOf(ByVal s As String) As Byte
			Return valueOf(s, 10)
		End Function

		''' <summary>
		''' Decodes a {@code String} into a {@code Byte}.
		''' Accepts decimal, hexadecimal, and octal numbers given by
		''' the following grammar:
		''' 
		''' <blockquote>
		''' <dl>
		''' <dt><i>DecodableString:</i>
		''' <dd><i>Sign<sub>opt</sub> DecimalNumeral</i>
		''' <dd><i>Sign<sub>opt</sub></i> {@code 0x} <i>HexDigits</i>
		''' <dd><i>Sign<sub>opt</sub></i> {@code 0X} <i>HexDigits</i>
		''' <dd><i>Sign<sub>opt</sub></i> {@code #} <i>HexDigits</i>
		''' <dd><i>Sign<sub>opt</sub></i> {@code 0} <i>OctalDigits</i>
		''' 
		''' <dt><i>Sign:</i>
		''' <dd>{@code -}
		''' <dd>{@code +}
		''' </dl>
		''' </blockquote>
		''' 
		''' <i>DecimalNumeral</i>, <i>HexDigits</i>, and <i>OctalDigits</i>
		''' are as defined in section 3.10.1 of
		''' <cite>The Java&trade; Language Specification</cite>,
		''' except that underscores are not accepted between digits.
		''' 
		''' <p>The sequence of characters following an optional
		''' sign and/or radix specifier ("{@code 0x}", "{@code 0X}",
		''' "{@code #}", or leading zero) is parsed as by the {@code
		''' Byte.parseByte} method with the indicated radix (10, 16, or 8).
		''' This sequence of characters must represent a positive value or
		''' a <seealso cref="NumberFormatException"/> will be thrown.  The result is
		''' negated if first character of the specified {@code String} is
		''' the minus sign.  No whitespace characters are permitted in the
		''' {@code String}.
		''' </summary>
		''' <param name="nm"> the {@code String} to decode. </param>
		''' <returns>   a {@code Byte} object holding the {@code byte}
		'''          value represented by {@code nm} </returns>
		''' <exception cref="NumberFormatException">  if the {@code String} does not
		'''            contain a parsable {@code byte}. </exception>
		''' <seealso cref= java.lang.Byte#parseByte(java.lang.String, int) </seealso>
		Public Shared Function decode(ByVal nm As String) As Byte
			Dim i As Integer = Integer.decode(nm)
			If i < MIN_VALUE OrElse i > MAX_VALUE Then Throw New NumberFormatException("Value " & i & " out of range from input " & nm)
			Return valueOf(CByte(i))
		End Function

		''' <summary>
		''' The value of the {@code Byte}.
		''' 
		''' @serial
		''' </summary>
		Private ReadOnly value As SByte

		''' <summary>
		''' Constructs a newly allocated {@code Byte} object that
		''' represents the specified {@code byte} value.
		''' </summary>
		''' <param name="value">     the value to be represented by the
		'''                  {@code Byte}. </param>
		Public Sub New(ByVal value As SByte)
			Me.value = value
		End Sub

		''' <summary>
		''' Constructs a newly allocated {@code Byte} object that
		''' represents the {@code byte} value indicated by the
		''' {@code String} parameter. The string is converted to a
		''' {@code byte} value in exactly the manner used by the
		''' {@code parseByte} method for radix 10.
		''' </summary>
		''' <param name="s">         the {@code String} to be converted to a
		'''                  {@code Byte} </param>
		''' <exception cref="NumberFormatException"> If the {@code String}
		'''                  does not contain a parsable {@code byte}. </exception>
		''' <seealso cref=        java.lang.Byte#parseByte(java.lang.String, int) </seealso>
		Public Sub New(ByVal s As String)
			Me.value = parseByte(s, 10)
		End Sub

		''' <summary>
		''' Returns the value of this {@code Byte} as a
		''' {@code byte}.
		''' </summary>
		Public Overrides Function byteValue() As SByte
			Return value
		End Function

		''' <summary>
		''' Returns the value of this {@code Byte} as a {@code short} after
		''' a widening primitive conversion.
		''' @jls 5.1.2 Widening Primitive Conversions
		''' </summary>
		Public Overrides Function shortValue() As Short
			Return CShort(value)
		End Function

		''' <summary>
		''' Returns the value of this {@code Byte} as an {@code int} after
		''' a widening primitive conversion.
		''' @jls 5.1.2 Widening Primitive Conversions
		''' </summary>
		Public Overrides Function intValue() As Integer
			Return CInt(value)
		End Function

		''' <summary>
		''' Returns the value of this {@code Byte} as a {@code long} after
		''' a widening primitive conversion.
		''' @jls 5.1.2 Widening Primitive Conversions
		''' </summary>
		Public Overrides Function longValue() As Long
			Return CLng(value)
		End Function

		''' <summary>
		''' Returns the value of this {@code Byte} as a {@code float} after
		''' a widening primitive conversion.
		''' @jls 5.1.2 Widening Primitive Conversions
		''' </summary>
		Public Overrides Function floatValue() As Single
			Return CSng(value)
		End Function

		''' <summary>
		''' Returns the value of this {@code Byte} as a {@code double}
		''' after a widening primitive conversion.
		''' @jls 5.1.2 Widening Primitive Conversions
		''' </summary>
		Public Overrides Function doubleValue() As Double
			Return CDbl(value)
		End Function

		''' <summary>
		''' Returns a {@code String} object representing this
		''' {@code Byte}'s value.  The value is converted to signed
		''' decimal representation and returned as a string, exactly as if
		''' the {@code byte} value were given as an argument to the
		''' <seealso cref="java.lang.Byte#toString(byte)"/> method.
		''' </summary>
		''' <returns>  a string representation of the value of this object in
		'''          base&nbsp;10. </returns>
		Public Overrides Function ToString() As String
			Return Convert.ToString(CInt(value))
		End Function

		''' <summary>
		''' Returns a hash code for this {@code Byte}; equal to the result
		''' of invoking {@code intValue()}.
		''' </summary>
		''' <returns> a hash code value for this {@code Byte} </returns>
		Public Overrides Function GetHashCode() As Integer
			Return Byte.hashCode(value)
		End Function

		''' <summary>
		''' Returns a hash code for a {@code byte} value; compatible with
		''' {@code Byte.hashCode()}.
		''' </summary>
		''' <param name="value"> the value to hash </param>
		''' <returns> a hash code value for a {@code byte} value.
		''' @since 1.8 </returns>
		Public Shared Function GetHashCode(ByVal value As SByte) As Integer
			Return CInt(value)
		End Function

		''' <summary>
		''' Compares this object to the specified object.  The result is
		''' {@code true} if and only if the argument is not
		''' {@code null} and is a {@code Byte} object that
		''' contains the same {@code byte} value as this object.
		''' </summary>
		''' <param name="obj">       the object to compare with </param>
		''' <returns>          {@code true} if the objects are the same;
		'''                  {@code false} otherwise. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If TypeOf obj Is Byte Then Return value = CByte(obj)
			Return False
		End Function

		''' <summary>
		''' Compares two {@code Byte} objects numerically.
		''' </summary>
		''' <param name="anotherByte">   the {@code Byte} to be compared. </param>
		''' <returns>  the value {@code 0} if this {@code Byte} is
		'''          equal to the argument {@code Byte}; a value less than
		'''          {@code 0} if this {@code Byte} is numerically less
		'''          than the argument {@code Byte}; and a value greater than
		'''           {@code 0} if this {@code Byte} is numerically
		'''           greater than the argument {@code Byte} (signed
		'''           comparison).
		''' @since   1.2 </returns>
		Public Function compareTo(ByVal anotherByte As Byte) As Integer Implements Comparable(Of Byte).compareTo
			Return compare(Me.value, anotherByte.value)
		End Function

		''' <summary>
		''' Compares two {@code byte} values numerically.
		''' The value returned is identical to what would be returned by:
		''' <pre>
		'''    Byte.valueOf(x).compareTo(Byte.valueOf(y))
		''' </pre>
		''' </summary>
		''' <param name="x"> the first {@code byte} to compare </param>
		''' <param name="y"> the second {@code byte} to compare </param>
		''' <returns> the value {@code 0} if {@code x == y};
		'''         a value less than {@code 0} if {@code x < y}; and
		'''         a value greater than {@code 0} if {@code x > y}
		''' @since 1.7 </returns>
		Public Shared Function compare(ByVal x As SByte, ByVal y As SByte) As Integer
			Return x - y
		End Function

		''' <summary>
		''' Converts the argument to an {@code int} by an unsigned
		''' conversion.  In an unsigned conversion to an {@code int}, the
		''' high-order 24 bits of the {@code int} are zero and the
		''' low-order 8 bits are equal to the bits of the {@code byte} argument.
		''' 
		''' Consequently, zero and positive {@code byte} values are mapped
		''' to a numerically equal {@code int} value and negative {@code
		''' byte} values are mapped to an {@code int} value equal to the
		''' input plus 2<sup>8</sup>.
		''' </summary>
		''' <param name="x"> the value to convert to an unsigned {@code int} </param>
		''' <returns> the argument converted to {@code int} by an unsigned
		'''         conversion
		''' @since 1.8 </returns>
		Public Shared Function toUnsignedInt(ByVal x As SByte) As Integer
			Return (CInt(x)) And &Hff
		End Function

		''' <summary>
		''' Converts the argument to a {@code long} by an unsigned
		''' conversion.  In an unsigned conversion to a {@code long}, the
		''' high-order 56 bits of the {@code long} are zero and the
		''' low-order 8 bits are equal to the bits of the {@code byte} argument.
		''' 
		''' Consequently, zero and positive {@code byte} values are mapped
		''' to a numerically equal {@code long} value and negative {@code
		''' byte} values are mapped to a {@code long} value equal to the
		''' input plus 2<sup>8</sup>.
		''' </summary>
		''' <param name="x"> the value to convert to an unsigned {@code long} </param>
		''' <returns> the argument converted to {@code long} by an unsigned
		'''         conversion
		''' @since 1.8 </returns>
		Public Shared Function toUnsignedLong(ByVal x As SByte) As Long
			Return (CLng(x)) And &HffL
		End Function


		''' <summary>
		''' The number of bits used to represent a {@code byte} value in two's
		''' complement binary form.
		''' 
		''' @since 1.5
		''' </summary>
		Public Const SIZE As Integer = 8

		''' <summary>
		''' The number of bytes used to represent a {@code byte} value in two's
		''' complement binary form.
		''' 
		''' @since 1.8
		''' </summary>
		Public Const BYTES As Integer = SIZE \ Byte.SIZE

		''' <summary>
		''' use serialVersionUID from JDK 1.1. for interoperability </summary>
		Private Const serialVersionUID As Long = -7183698231559129828L
	End Class

End Namespace
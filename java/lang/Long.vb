Imports Microsoft.VisualBasic
Imports System
Imports java.math

'
' * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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
    ''' The {@code Long} class wraps a value of the primitive type {@code
    ''' long} in an object. An object of type {@code Long} contains a
    ''' single field whose type is {@code long}.
    ''' 
    ''' <p> In addition, this class provides several methods for converting
    ''' a {@code long} to a {@code String} and a {@code String} to a {@code
    ''' long}, as well as other constants and methods useful when dealing
    ''' with a {@code long}.
    ''' 
    ''' <p>Implementation note: The implementations of the "bit twiddling"
    ''' methods (such as <seealso cref="#highestOneBit(long) highestOneBit"/> and
    ''' <seealso cref="#numberOfTrailingZeros(long) numberOfTrailingZeros"/>) are
    ''' based on material from Henry S. Warren, Jr.'s <i>Hacker's
    ''' Delight</i>, (Addison Wesley, 2002).
    ''' 
    ''' @author  Lee Boynton
    ''' @author  Arthur van Hoff
    ''' @author  Josh Bloch
    ''' @author  Joseph D. Darcy
    ''' @since   JDK1.0
    ''' </summary>
    Public NotInheritable Class [Long]
        Inherits Number
        Implements Comparable(Of [Long])

        ''' <summary>
        ''' A constant holding the minimum value a {@code long} can
        ''' have, -2<sup>63</sup>.
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Const MIN_VALUE As Long = &H8000000000000000L

        ''' <summary>
        ''' A constant holding the maximum value a {@code long} can
        ''' have, 2<sup>63</sup>-1.
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Const MAX_VALUE As Long = &H7FFFFFFFFFFFFFFFL

        ''' <summary>
        ''' The {@code Class} instance representing the primitive type
        ''' {@code long}.
        ''' 
        ''' @since   JDK1.1
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Shared ReadOnly TYPE As [Class] = CType([Class].getPrimitiveClass("long"), [Class])

        ''' <summary>
        ''' Returns a string representation of the first argument in the
        ''' radix specified by the second argument.
        ''' 
        ''' <p>If the radix is smaller than {@code Character.MIN_RADIX}
        ''' or larger than {@code Character.MAX_RADIX}, then the radix
        ''' {@code 10} is used instead.
        ''' 
        ''' <p>If the first argument is negative, the first element of the
        ''' result is the ASCII minus sign {@code '-'}
        ''' ({@code '\u005Cu002d'}). If the first argument is not
        ''' negative, no sign character appears in the result.
        ''' 
        ''' <p>The remaining characters of the result represent the magnitude
        ''' of the first argument. If the magnitude is zero, it is
        ''' represented by a single zero character {@code '0'}
        ''' ({@code '\u005Cu0030'}); otherwise, the first character of
        ''' the representation of the magnitude will not be the zero
        ''' character.  The following ASCII characters are used as digits:
        ''' 
        ''' <blockquote>
        '''   {@code 0123456789abcdefghijklmnopqrstuvwxyz}
        ''' </blockquote>
        ''' 
        ''' These are {@code '\u005Cu0030'} through
        ''' {@code '\u005Cu0039'} and {@code '\u005Cu0061'} through
        ''' {@code '\u005Cu007a'}. If {@code radix} is
        ''' <var>N</var>, then the first <var>N</var> of these characters
        ''' are used as radix-<var>N</var> digits in the order shown. Thus,
        ''' the digits for hexadecimal (radix 16) are
        ''' {@code 0123456789abcdef}. If uppercase letters are
        ''' desired, the <seealso cref="java.lang.String#toUpperCase()"/> method may
        ''' be called on the result:
        ''' 
        ''' <blockquote>
        '''  {@code Long.toString(n, 16).toUpperCase()}
        ''' </blockquote>
        ''' </summary>
        ''' <param name="i">       a {@code long} to be converted to a string. </param>
        ''' <param name="radix">   the radix to use in the string representation. </param>
        ''' <returns>  a string representation of the argument in the specified radix. </returns>
        ''' <seealso cref=     java.lang.Character#MAX_RADIX </seealso>
        ''' <seealso cref=     java.lang.Character#MIN_RADIX </seealso>
        Public Overloads Shared Function ToString(ByVal i As Long, ByVal radix As Integer) As String
            If radix < Character.MIN_RADIX OrElse radix > Character.MAX_RADIX Then radix = 10
            If radix = 10 Then Return ToString(i)
            Dim buf As Char() = New Char(64) {}
            Dim charPos As Integer = 64
            Dim negative As Boolean = (i < 0)

            If Not negative Then i = -i

            Do While i <= -radix
                buf(charPos) = [Integer].digits(CInt(Fix(-(i Mod radix))))
                charPos -= 1
                i = i \ radix
            Loop
            buf(charPos) = [Integer].digits(CInt(Fix(-i)))

            If negative Then
                charPos -= 1
                buf(charPos) = "-"c
            End If

            Return New String(buf, charPos, (65 - charPos))
        End Function

        ''' <summary>
        ''' Returns a string representation of the first argument as an
        ''' unsigned integer value in the radix specified by the second
        ''' argument.
        ''' 
        ''' <p>If the radix is smaller than {@code Character.MIN_RADIX}
        ''' or larger than {@code Character.MAX_RADIX}, then the radix
        ''' {@code 10} is used instead.
        ''' 
        ''' <p>Note that since the first argument is treated as an unsigned
        ''' value, no leading sign character is printed.
        ''' 
        ''' <p>If the magnitude is zero, it is represented by a single zero
        ''' character {@code '0'} ({@code '\u005Cu0030'}); otherwise,
        ''' the first character of the representation of the magnitude will
        ''' not be the zero character.
        ''' 
        ''' <p>The behavior of radixes and the characters used as digits
        ''' are the same as <seealso cref="#toString(long, int) toString"/>.
        ''' </summary>
        ''' <param name="i">       an integer to be converted to an unsigned string. </param>
        ''' <param name="radix">   the radix to use in the string representation. </param>
        ''' <returns>  an unsigned string representation of the argument in the specified radix. </returns>
        ''' <seealso cref=     #toString(long, int)
        ''' @since 1.8 </seealso>
        Public Shared Function toUnsignedString(ByVal i As Long, ByVal radix As Integer) As String
            If i >= 0 Then
                Return ToString(i, radix)
            Else
                Select Case radix
                    Case 2
                        Return toBinaryString(i)

                    Case 4
                        Return toUnsignedString0(i, 2)

                    Case 8
                        Return toOctalString(i)

                    Case 10
                        '                
                        '                 * We can get the effect of an unsigned division by 10
                        '                 * on a long value by first shifting right, yielding a
                        '                 * positive value, and then dividing by 5.  This
                        '                 * allows the last digit and preceding digits to be
                        '                 * isolated more quickly than by an initial conversion
                        '                 * to BigInteger.
                        '                 
                        Dim quot As Long = (CLng(CULng(i) >> 1)) \ 5
                        Dim [rem] As Long = i - quot * 10
                        Return ToString(quot) + [rem]

                    Case 16
                        Return toHexString(i)

                    Case 32
                        Return toUnsignedString0(i, 5)

                    Case Else
                        Return toUnsignedBigInteger(i).ToString(radix)
                End Select
            End If
        End Function

        ''' <summary>
        ''' Return a BigInteger equal to the unsigned value of the
        ''' argument.
        ''' </summary>
        Private Shared Function toUnsignedBigInteger(ByVal i As Long) As BigInteger
            If i >= 0L Then
                Return BigInteger.valueOf(i)
            Else
                Dim upper As Integer = CInt(CLng(CULng(i) >> 32))
                Dim lower As Integer = CInt(i)

                ' return (upper << 32) + lower
                Return (BigInteger.valueOf(Integer.toUnsignedLong(upper))).shiftLeft(32).add(BigInteger.valueOf(Integer.toUnsignedLong(lower)))
            End If
        End Function

        ''' <summary>
        ''' Returns a string representation of the {@code long}
        ''' argument as an unsigned integer in base&nbsp;16.
        ''' 
        ''' <p>The unsigned {@code long} value is the argument plus
        ''' 2<sup>64</sup> if the argument is negative; otherwise, it is
        ''' equal to the argument.  This value is converted to a string of
        ''' ASCII digits in hexadecimal (base&nbsp;16) with no extra
        ''' leading {@code 0}s.
        ''' 
        ''' <p>The value of the argument can be recovered from the returned
        ''' string {@code s} by calling {@link
        ''' Long#parseUnsignedLong(String, int) Long.parseUnsignedLong(s,
        ''' 16)}.
        ''' 
        ''' <p>If the unsigned magnitude is zero, it is represented by a
        ''' single zero character {@code '0'} ({@code '\u005Cu0030'});
        ''' otherwise, the first character of the representation of the
        ''' unsigned magnitude will not be the zero character. The
        ''' following characters are used as hexadecimal digits:
        ''' 
        ''' <blockquote>
        '''  {@code 0123456789abcdef}
        ''' </blockquote>
        ''' 
        ''' These are the characters {@code '\u005Cu0030'} through
        ''' {@code '\u005Cu0039'} and  {@code '\u005Cu0061'} through
        ''' {@code '\u005Cu0066'}.  If uppercase letters are desired,
        ''' the <seealso cref="java.lang.String#toUpperCase()"/> method may be called
        ''' on the result:
        ''' 
        ''' <blockquote>
        '''  {@code Long.toHexString(n).toUpperCase()}
        ''' </blockquote>
        ''' </summary>
        ''' <param name="i">   a {@code long} to be converted to a string. </param>
        ''' <returns>  the string representation of the unsigned {@code long}
        '''          value represented by the argument in hexadecimal
        '''          (base&nbsp;16). </returns>
        ''' <seealso cref= #parseUnsignedLong(String, int) </seealso>
        ''' <seealso cref= #toUnsignedString(long, int)
        ''' @since   JDK 1.0.2 </seealso>
        Public Shared Function toHexString(ByVal i As Long) As String
            Return toUnsignedString0(i, 4)
        End Function

        ''' <summary>
        ''' Returns a string representation of the {@code long}
        ''' argument as an unsigned integer in base&nbsp;8.
        ''' 
        ''' <p>The unsigned {@code long} value is the argument plus
        ''' 2<sup>64</sup> if the argument is negative; otherwise, it is
        ''' equal to the argument.  This value is converted to a string of
        ''' ASCII digits in octal (base&nbsp;8) with no extra leading
        ''' {@code 0}s.
        ''' 
        ''' <p>The value of the argument can be recovered from the returned
        ''' string {@code s} by calling {@link
        ''' Long#parseUnsignedLong(String, int) Long.parseUnsignedLong(s,
        ''' 8)}.
        ''' 
        ''' <p>If the unsigned magnitude is zero, it is represented by a
        ''' single zero character {@code '0'} ({@code '\u005Cu0030'});
        ''' otherwise, the first character of the representation of the
        ''' unsigned magnitude will not be the zero character. The
        ''' following characters are used as octal digits:
        ''' 
        ''' <blockquote>
        '''  {@code 01234567}
        ''' </blockquote>
        ''' 
        ''' These are the characters {@code '\u005Cu0030'} through
        ''' {@code '\u005Cu0037'}.
        ''' </summary>
        ''' <param name="i">   a {@code long} to be converted to a string. </param>
        ''' <returns>  the string representation of the unsigned {@code long}
        '''          value represented by the argument in octal (base&nbsp;8). </returns>
        ''' <seealso cref= #parseUnsignedLong(String, int) </seealso>
        ''' <seealso cref= #toUnsignedString(long, int)
        ''' @since   JDK 1.0.2 </seealso>
        Public Shared Function toOctalString(ByVal i As Long) As String
            Return toUnsignedString0(i, 3)
        End Function

        ''' <summary>
        ''' Returns a string representation of the {@code long}
        ''' argument as an unsigned integer in base&nbsp;2.
        ''' 
        ''' <p>The unsigned {@code long} value is the argument plus
        ''' 2<sup>64</sup> if the argument is negative; otherwise, it is
        ''' equal to the argument.  This value is converted to a string of
        ''' ASCII digits in binary (base&nbsp;2) with no extra leading
        ''' {@code 0}s.
        ''' 
        ''' <p>The value of the argument can be recovered from the returned
        ''' string {@code s} by calling {@link
        ''' Long#parseUnsignedLong(String, int) Long.parseUnsignedLong(s,
        ''' 2)}.
        ''' 
        ''' <p>If the unsigned magnitude is zero, it is represented by a
        ''' single zero character {@code '0'} ({@code '\u005Cu0030'});
        ''' otherwise, the first character of the representation of the
        ''' unsigned magnitude will not be the zero character. The
        ''' characters {@code '0'} ({@code '\u005Cu0030'}) and {@code
        ''' '1'} ({@code '\u005Cu0031'}) are used as binary digits.
        ''' </summary>
        ''' <param name="i">   a {@code long} to be converted to a string. </param>
        ''' <returns>  the string representation of the unsigned {@code long}
        '''          value represented by the argument in binary (base&nbsp;2). </returns>
        ''' <seealso cref= #parseUnsignedLong(String, int) </seealso>
        ''' <seealso cref= #toUnsignedString(long, int)
        ''' @since   JDK 1.0.2 </seealso>
        Public Shared Function toBinaryString(ByVal i As Long) As String
            Return toUnsignedString0(i, 1)
        End Function

        ''' <summary>
        ''' Format a long (treated as unsigned) into a String. </summary>
        ''' <param name="val"> the value to format </param>
        ''' <param name="shift"> the log2 of the base to format in (4 for hex, 3 for octal, 1 for binary) </param>
        Friend Shared Function toUnsignedString0(ByVal val As Long, ByVal shift As Integer) As String
            ' assert shift > 0 && shift <=5 : "Illegal shift value";
            Dim mag As Integer = Long.SIZE - Long.numberOfLeadingZeros(val)
            Dim chars_Renamed As Integer = Math.max(((mag + (shift - 1)) \ shift), 1)
            Dim buf As Char() = New Char(chars_Renamed - 1) {}

            formatUnsignedLong(val, shift, buf, 0, chars_Renamed)
            Return New String(buf, True)
        End Function

        ''' <summary>
        ''' Format a long (treated as unsigned) into a character buffer. </summary>
        ''' <param name="val"> the unsigned long to format </param>
        ''' <param name="shift"> the log2 of the base to format in (4 for hex, 3 for octal, 1 for binary) </param>
        ''' <param name="buf"> the character buffer to write to </param>
        ''' <param name="offset"> the offset in the destination buffer to start at </param>
        ''' <param name="len"> the number of characters to write </param>
        ''' <returns> the lowest character location used </returns>
        Friend Shared Function formatUnsignedLong(ByVal val As Long, ByVal shift As Integer, ByVal buf As Char(), ByVal offset As Integer, ByVal len As Integer) As Integer
            Dim charPos As Integer = len
            Dim radix As Integer = 1 << shift
            Dim mask As Integer = radix - 1
            Do
                charPos -= 1
                buf(offset + charPos) = Integer.digits((CInt(val)) And mask)
                val >>>= shift
            Loop While val <> 0 AndAlso charPos > 0

            Return charPos
        End Function

        ''' <summary>
        ''' Returns a {@code String} object representing the specified
        ''' {@code long}.  The argument is converted to signed decimal
        ''' representation and returned as a string, exactly as if the
        ''' argument and the radix 10 were given as arguments to the {@link
        ''' #toString(long, int)} method.
        ''' </summary>
        ''' <param name="i">   a {@code long} to be converted. </param>
        ''' <returns>  a string representation of the argument in base&nbsp;10. </returns>
        Public Shared Function ToString(ByVal i As Long) As String
            If i = Long.MinValue Then Return "-9223372036854775808"
            Dim size_Renamed As Integer = If(i < 0, stringSize(-i) + 1, stringSize(i))
            Dim buf As Char() = New Char(size_Renamed - 1) {}
            getChars(i, size_Renamed, buf)
            Return New String(buf, True)
        End Function

        ''' <summary>
        ''' Returns a string representation of the argument as an unsigned
        ''' decimal value.
        ''' 
        ''' The argument is converted to unsigned decimal representation
        ''' and returned as a string exactly as if the argument and radix
        ''' 10 were given as arguments to the {@link #toUnsignedString(long,
        ''' int)} method.
        ''' </summary>
        ''' <param name="i">  an integer to be converted to an unsigned string. </param>
        ''' <returns>  an unsigned string representation of the argument. </returns>
        ''' <seealso cref=     #toUnsignedString(long, int)
        ''' @since 1.8 </seealso>
        Public Shared Function toUnsignedString(ByVal i As Long) As String
            Return toUnsignedString(i, 10)
        End Function

        ''' <summary>
        ''' Places characters representing the integer i into the
        ''' character array buf. The characters are placed into
        ''' the buffer backwards starting with the least significant
        ''' digit at the specified index (exclusive), and working
        ''' backwards from there.
        ''' 
        ''' Will fail if i == Long.MIN_VALUE
        ''' </summary>
        Friend Shared Sub getChars(ByVal i As Long, ByVal index As Integer, ByVal buf As Char())
            Dim q As Long
            Dim r As Integer
            Dim charPos As Integer = index
            Dim sign As Char = 0

            If i < 0 Then
                sign = "-"c
                i = -i
            End If

            ' Get 2 digits/iteration using longs until quotient fits into an int
            Do While i > Integer.MaxValue
                q = i \ 100
                ' really: r = i - (q * 100);
                r = CInt(Fix(i - ((q << 6) + (q << 5) + (q << 2))))
                i = q
                charPos -= 1
                buf(charPos) = Integer.DigitOnes(r)
                charPos -= 1
                buf(charPos) = Integer.DigitTens(r)
            Loop

            ' Get 2 digits/iteration using ints
            Dim q2 As Integer
            Dim i2 As Integer = CInt(i)
            Do While i2 >= 65536
                q2 = i2 \ 100
                ' really: r = i2 - (q * 100);
                r = i2 - ((q2 << 6) + (q2 << 5) + (q2 << 2))
                i2 = q2
                charPos -= 1
                buf(charPos) = Integer.DigitOnes(r)
                charPos -= 1
                buf(charPos) = Integer.DigitTens(r)
            Loop

            ' Fall thru to fast mode for smaller numbers
            ' assert(i2 <= 65536, i2);
            Do
                q2 = CInt(CUInt((i2 * 52429)) >> (16 + 3))
                r = i2 - ((q2 << 3) + (q2 << 1)) ' r = i2-(q2*10) ...
                charPos -= 1
                buf(charPos) = Integer.digits(r)
                i2 = q2
                If i2 = 0 Then Exit Do
            Loop
            If AscW(sign) <> 0 Then
                charPos -= 1
                buf(charPos) = sign
            End If
        End Sub

        ' Requires positive x
        Friend Shared Function stringSize(ByVal x As Long) As Integer
            Dim p As Long = 10
            For i As Integer = 1 To 18
                If x < p Then Return i
                p = 10 * p
            Next i
            Return 19
        End Function

        ''' <summary>
        ''' Parses the string argument as a signed {@code long} in the
        ''' radix specified by the second argument. The characters in the
        ''' string must all be digits of the specified radix (as determined
        ''' by whether <seealso cref="java.lang.Character#digit(char, int)"/> returns
        ''' a nonnegative value), except that the first character may be an
        ''' ASCII minus sign {@code '-'} ({@code '\u005Cu002D'}) to
        ''' indicate a negative value or an ASCII plus sign {@code '+'}
        ''' ({@code '\u005Cu002B'}) to indicate a positive value. The
        ''' resulting {@code long} value is returned.
        ''' 
        ''' <p>Note that neither the character {@code L}
        ''' ({@code '\u005Cu004C'}) nor {@code l}
        ''' ({@code '\u005Cu006C'}) is permitted to appear at the end
        ''' of the string as a type indicator, as would be permitted in
        ''' Java programming language source code - except that either
        ''' {@code L} or {@code l} may appear as a digit for a
        ''' radix greater than or equal to 22.
        ''' 
        ''' <p>An exception of type {@code NumberFormatException} is
        ''' thrown if any of the following situations occurs:
        ''' <ul>
        ''' 
        ''' <li>The first argument is {@code null} or is a string of
        ''' length zero.
        ''' 
        ''' <li>The {@code radix} is either smaller than {@link
        ''' java.lang.Character#MIN_RADIX} or larger than {@link
        ''' java.lang.Character#MAX_RADIX}.
        ''' 
        ''' <li>Any character of the string is not a digit of the specified
        ''' radix, except that the first character may be a minus sign
        ''' {@code '-'} ({@code '\u005Cu002d'}) or plus sign {@code
        ''' '+'} ({@code '\u005Cu002B'}) provided that the string is
        ''' longer than length 1.
        ''' 
        ''' <li>The value represented by the string is not a value of type
        '''      {@code long}.
        ''' </ul>
        ''' 
        ''' <p>Examples:
        ''' <blockquote><pre>
        ''' parseLong("0", 10) returns 0L
        ''' parseLong("473", 10) returns 473L
        ''' parseLong("+42", 10) returns 42L
        ''' parseLong("-0", 10) returns 0L
        ''' parseLong("-FF", 16) returns -255L
        ''' parseLong("1100110", 2) returns 102L
        ''' parseLong("99", 8) throws a NumberFormatException
        ''' parseLong("Hazelnut", 10) throws a NumberFormatException
        ''' parseLong("Hazelnut", 36) returns 1356099454469L
        ''' </pre></blockquote>
        ''' </summary>
        ''' <param name="s">       the {@code String} containing the
        '''                     {@code long} representation to be parsed. </param>
        ''' <param name="radix">   the radix to be used while parsing {@code s}. </param>
        ''' <returns>     the {@code long} represented by the string argument in
        '''             the specified radix. </returns>
        ''' <exception cref="NumberFormatException">  if the string does not contain a
        '''             parsable {@code long}. </exception>
        Public Shared Function parseLong(ByVal s As String, ByVal radix As Integer) As Long
            If s Is Nothing Then Throw New NumberFormatException("null")

            If radix < Character.MIN_RADIX Then Throw New NumberFormatException("radix " & radix & " less than Character.MIN_RADIX")
            If radix > Character.MAX_RADIX Then Throw New NumberFormatException("radix " & radix & " greater than Character.MAX_RADIX")

            Dim result As Long = 0
            Dim negative As Boolean = False
            Dim i As Integer = 0, len As Integer = s.Length()
            Dim limit As Long = -Long.MaxValue
            Dim multmin As Long
            Dim digit As Integer

            If len > 0 Then
                Dim firstChar As Char = s.Chars(0)
                If firstChar < "0"c Then ' Possible leading "+" or "-"
                    If firstChar = "-"c Then
                        negative = True
                        limit = Long.MinValue
                    ElseIf firstChar <> "+"c Then
                        Throw NumberFormatException.forInputString(s)
                    End If

                    If len = 1 Then ' Cannot have lone "+" or "-" Throw NumberFormatException.forInputString(s)
                        i += 1
                    End If
                    multmin = limit \ radix
                    Do While i < len
                        ' Accumulating negatively avoids surprises near MAX_VALUE
                        digit = Character.digit(s.Chars(i), radix)
                        i += 1
                        If digit < 0 Then Throw NumberFormatException.forInputString(s)
                        If result < multmin Then Throw NumberFormatException.forInputString(s)
                        result *= radix
                        If result < limit + digit Then Throw NumberFormatException.forInputString(s)
                        result -= digit
                    Loop
                Else
                    Throw NumberFormatException.forInputString(s)
                End If
                Return If(negative, result, -result)
        End Function

        ''' <summary>
        ''' Parses the string argument as a signed decimal {@code long}.
        ''' The characters in the string must all be decimal digits, except
        ''' that the first character may be an ASCII minus sign {@code '-'}
        ''' ({@code \u005Cu002D'}) to indicate a negative value or an
        ''' ASCII plus sign {@code '+'} ({@code '\u005Cu002B'}) to
        ''' indicate a positive value. The resulting {@code long} value is
        ''' returned, exactly as if the argument and the radix {@code 10}
        ''' were given as arguments to the {@link
        ''' #parseLong(java.lang.String, int)} method.
        ''' 
        ''' <p>Note that neither the character {@code L}
        ''' ({@code '\u005Cu004C'}) nor {@code l}
        ''' ({@code '\u005Cu006C'}) is permitted to appear at the end
        ''' of the string as a type indicator, as would be permitted in
        ''' Java programming language source code.
        ''' </summary>
        ''' <param name="s">   a {@code String} containing the {@code long}
        '''             representation to be parsed </param>
        ''' <returns>     the {@code long} represented by the argument in
        '''             decimal. </returns>
        ''' <exception cref="NumberFormatException">  if the string does not contain a
        '''             parsable {@code long}. </exception>
        Public Shared Function parseLong(ByVal s As String) As Long
            Return parseLong(s, 10)
        End Function

        ''' <summary>
        ''' Parses the string argument as an unsigned {@code long} in the
        ''' radix specified by the second argument.  An unsigned integer
        ''' maps the values usually associated with negative numbers to
        ''' positive numbers larger than {@code MAX_VALUE}.
        ''' 
        ''' The characters in the string must all be digits of the
        ''' specified radix (as determined by whether {@link
        ''' java.lang.Character#digit(char, int)} returns a nonnegative
        ''' value), except that the first character may be an ASCII plus
        ''' sign {@code '+'} ({@code '\u005Cu002B'}). The resulting
        ''' integer value is returned.
        ''' 
        ''' <p>An exception of type {@code NumberFormatException} is
        ''' thrown if any of the following situations occurs:
        ''' <ul>
        ''' <li>The first argument is {@code null} or is a string of
        ''' length zero.
        ''' 
        ''' <li>The radix is either smaller than
        ''' <seealso cref="java.lang.Character#MIN_RADIX"/> or
        ''' larger than <seealso cref="java.lang.Character#MAX_RADIX"/>.
        ''' 
        ''' <li>Any character of the string is not a digit of the specified
        ''' radix, except that the first character may be a plus sign
        ''' {@code '+'} ({@code '\u005Cu002B'}) provided that the
        ''' string is longer than length 1.
        ''' 
        ''' <li>The value represented by the string is larger than the
        ''' largest unsigned {@code long}, 2<sup>64</sup>-1.
        ''' 
        ''' </ul>
        ''' 
        ''' </summary>
        ''' <param name="s">   the {@code String} containing the unsigned integer
        '''                  representation to be parsed </param>
        ''' <param name="radix">   the radix to be used while parsing {@code s}. </param>
        ''' <returns>     the unsigned {@code long} represented by the string
        '''             argument in the specified radix. </returns>
        ''' <exception cref="NumberFormatException"> if the {@code String}
        '''             does not contain a parsable {@code long}.
        ''' @since 1.8 </exception>
        Public Shared Function parseUnsignedLong(ByVal s As String, ByVal radix As Integer) As Long
            If s Is Nothing Then Throw New NumberFormatException("null")

            Dim len As Integer = s.Length()
            If len > 0 Then
                Dim firstChar As Char = s.Chars(0)
                If firstChar = "-"c Then
                    Throw New NumberFormatException(String.Format("Illegal leading minus sign " & "on unsigned string {0}.", s))
                Else
                    If len <= 12 OrElse (radix = 10 AndAlso len <= 18) Then ' Long.MAX_VALUE in base 10 is 19 digits -  Long.MAX_VALUE in Character.MAX_RADIX is 13 digits Return parseLong(s, radix)

                        ' No need for range checks on len due to testing above.
                        Dim first As Long = parseLong(s.Substring(0, len - 1), radix)
                        Dim second As Integer = Character.digit(s.Chars(len - 1), radix)
                        If second < 0 Then Throw New NumberFormatException("Bad digit at end of " & s)
                        Dim result As Long = first * radix + second
                        If compareUnsigned(result, first) < 0 Then Throw New NumberFormatException(String.Format("String value {0} exceeds " & "range of unsigned long.", s))
                        Return result
                    End If
                    Else
                    Throw NumberFormatException.forInputString(s)
                End If
        End Function

        ''' <summary>
        ''' Parses the string argument as an unsigned decimal {@code long}. The
        ''' characters in the string must all be decimal digits, except
        ''' that the first character may be an an ASCII plus sign {@code
        ''' '+'} ({@code '\u005Cu002B'}). The resulting integer value
        ''' is returned, exactly as if the argument and the radix 10 were
        ''' given as arguments to the {@link
        ''' #parseUnsignedLong(java.lang.String, int)} method.
        ''' </summary>
        ''' <param name="s">   a {@code String} containing the unsigned {@code long}
        '''            representation to be parsed </param>
        ''' <returns>    the unsigned {@code long} value represented by the decimal string argument </returns>
        ''' <exception cref="NumberFormatException">  if the string does not contain a
        '''            parsable unsigned integer.
        ''' @since 1.8 </exception>
        Public Shared Function parseUnsignedLong(ByVal s As String) As Long
            Return parseUnsignedLong(s, 10)
        End Function

        ''' <summary>
        ''' Returns a {@code Long} object holding the value
        ''' extracted from the specified {@code String} when parsed
        ''' with the radix given by the second argument.  The first
        ''' argument is interpreted as representing a signed
        ''' {@code long} in the radix specified by the second
        ''' argument, exactly as if the arguments were given to the {@link
        ''' #parseLong(java.lang.String, int)} method. The result is a
        ''' {@code Long} object that represents the {@code long}
        ''' value specified by the string.
        ''' 
        ''' <p>In other words, this method returns a {@code Long} object equal
        ''' to the value of:
        ''' 
        ''' <blockquote>
        '''  {@code new Long(Long.parseLong(s, radix))}
        ''' </blockquote>
        ''' </summary>
        ''' <param name="s">       the string to be parsed </param>
        ''' <param name="radix">   the radix to be used in interpreting {@code s} </param>
        ''' <returns>     a {@code Long} object holding the value
        '''             represented by the string argument in the specified
        '''             radix. </returns>
        ''' <exception cref="NumberFormatException">  If the {@code String} does not
        '''             contain a parsable {@code long}. </exception>
        Public Shared Function valueOf(ByVal s As String, ByVal radix As Integer) As Long?
            Return Convert.ToInt64(parseLong(s, radix))
        End Function

        ''' <summary>
        ''' Returns a {@code Long} object holding the value
        ''' of the specified {@code String}. The argument is
        ''' interpreted as representing a signed decimal {@code long},
        ''' exactly as if the argument were given to the {@link
        ''' #parseLong(java.lang.String)} method. The result is a
        ''' {@code Long} object that represents the integer value
        ''' specified by the string.
        ''' 
        ''' <p>In other words, this method returns a {@code Long} object
        ''' equal to the value of:
        ''' 
        ''' <blockquote>
        '''  {@code new Long(Long.parseLong(s))}
        ''' </blockquote>
        ''' </summary>
        ''' <param name="s">   the string to be parsed. </param>
        ''' <returns>     a {@code Long} object holding the value
        '''             represented by the string argument. </returns>
        ''' <exception cref="NumberFormatException">  If the string cannot be parsed
        '''             as a {@code long}. </exception>
        Public Shared Function valueOf(ByVal s As String) As Long?
            Return Convert.ToInt64(parseLong(s, 10))
        End Function

        Private Class LongCache
            Private Sub New()
            End Sub

            Friend Shared ReadOnly cache As Long?() = New Long?(-(-128) + 127 + 1)

            Shared Sub New()
                For i As Integer = 0 To cache.Length - 1
                    cache(i) = New Long?(i - 128)
                Next i
            End Sub
        End Class

        ''' <summary>
        ''' Returns a {@code Long} instance representing the specified
        ''' {@code long} value.
        ''' If a new {@code Long} instance is not required, this method
        ''' should generally be used in preference to the constructor
        ''' <seealso cref="#Long(long)"/>, as this method is likely to yield
        ''' significantly better space and time performance by caching
        ''' frequently requested values.
        ''' 
        ''' Note that unlike the {@link Integer#valueOf(int)
        ''' corresponding method} in the {@code Integer} class, this method
        ''' is <em>not</em> required to cache values within a particular
        ''' range.
        ''' </summary>
        ''' <param name="l"> a long value. </param>
        ''' <returns> a {@code Long} instance representing {@code l}.
        ''' @since  1.5 </returns>
        Public Shared Function valueOf(ByVal l As Long) As Long?
            Const offset As Integer = 128
            If l >= -128 AndAlso l <= 127 Then ' will cache Return LongCache.cache(CInt(l) + offset)
                Return New Long?(l)
        End Function

        ''' <summary>
        ''' Decodes a {@code String} into a {@code Long}.
        ''' Accepts decimal, hexadecimal, and octal numbers given by the
        ''' following grammar:
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
        ''' Long.parseLong} method with the indicated radix (10, 16, or 8).
        ''' This sequence of characters must represent a positive value or
        ''' a <seealso cref="NumberFormatException"/> will be thrown.  The result is
        ''' negated if first character of the specified {@code String} is
        ''' the minus sign.  No whitespace characters are permitted in the
        ''' {@code String}.
        ''' </summary>
        ''' <param name="nm"> the {@code String} to decode. </param>
        ''' <returns>    a {@code Long} object holding the {@code long}
        '''            value represented by {@code nm} </returns>
        ''' <exception cref="NumberFormatException">  if the {@code String} does not
        '''            contain a parsable {@code long}. </exception>
        ''' <seealso cref= java.lang.Long#parseLong(String, int)
        ''' @since 1.2 </seealso>
        Public Shared Function decode(ByVal nm As String) As Long?
            Dim radix As Integer = 10
            Dim index As Integer = 0
            Dim negative As Boolean = False
            Dim result As Long?

            If nm.Length() = 0 Then Throw New NumberFormatException("Zero length string")
            Dim firstChar As Char = nm.Chars(0)
            ' Handle sign, if present
            If firstChar = "-"c Then
                negative = True
                index += 1
            ElseIf firstChar = "+"c Then
                index += 1
            End If

            ' Handle radix specifier, if present
            If nm.StartsWith("0x", index) OrElse nm.StartsWith("0X", index) Then
                index += 2
                radix = 16
            ElseIf nm.StartsWith("#", index) Then
                index += 1
                radix = 16
            ElseIf nm.StartsWith("0", index) AndAlso nm.Length() > 1 + index Then
                index += 1
                radix = 8
            End If

            If nm.StartsWith("-", index) OrElse nm.StartsWith("+", index) Then Throw New NumberFormatException("Sign character in wrong position")

            Try
                result = Convert.ToInt64(nm.Substring(index), radix)
                result = If(negative, Convert.ToInt64(-result), result)
            Catch e As NumberFormatException
                ' If number is Long.MIN_VALUE, we'll end up here. The next line
                ' handles this case, and causes any genuine format error to be
                ' rethrown.
                Dim constant As String = If(negative, ("-" & nm.Substring(index)), nm.Substring(index))
                result = Convert.ToInt64(constant, radix)
            End Try
            Return result
        End Function

        ''' <summary>
        ''' The value of the {@code Long}.
        ''' 
        ''' @serial
        ''' </summary>
        Private ReadOnly value As Long

        ''' <summary>
        ''' Constructs a newly allocated {@code Long} object that
        ''' represents the specified {@code long} argument.
        ''' </summary>
        ''' <param name="value">   the value to be represented by the
        '''          {@code Long} object. </param>
        Function java.lang.Long(ByVal value As Long) As [Public]
			Me.value = value
        End Function

        ''' <summary>
        ''' Constructs a newly allocated {@code Long} object that
        ''' represents the {@code long} value indicated by the
        ''' {@code String} parameter. The string is converted to a
        ''' {@code long} value in exactly the manner used by the
        ''' {@code parseLong} method for radix 10.
        ''' </summary>
        ''' <param name="s">   the {@code String} to be converted to a
        '''             {@code Long}. </param>
        ''' <exception cref="NumberFormatException">  if the {@code String} does not
        '''             contain a parsable {@code long}. </exception>
        ''' <seealso cref=        java.lang.Long#parseLong(java.lang.String, int) </seealso>
        Function java.lang.Long(ByVal s As String) As [Public]
			Me.value = parseLong(s, 10)
        End Function

        ''' <summary>
        ''' Returns the value of this {@code Long} as a {@code byte} after
        ''' a narrowing primitive conversion.
        ''' @jls 5.1.3 Narrowing Primitive Conversions
        ''' </summary>
        Public Overrides Function byteValue() As SByte
            Return CByte(value)
        End Function

        ''' <summary>
        ''' Returns the value of this {@code Long} as a {@code short} after
        ''' a narrowing primitive conversion.
        ''' @jls 5.1.3 Narrowing Primitive Conversions
        ''' </summary>
        Public Overrides Function shortValue() As Short
            Return CShort(Fix(value))
        End Function

        ''' <summary>
        ''' Returns the value of this {@code Long} as an {@code int} after
        ''' a narrowing primitive conversion.
        ''' @jls 5.1.3 Narrowing Primitive Conversions
        ''' </summary>
        Public Overrides Function intValue() As Integer
            Return CInt(Fix(value))
        End Function

        ''' <summary>
        ''' Returns the value of this {@code Long} as a
        ''' {@code long} value.
        ''' </summary>
        Public Overrides Function longValue() As Long
            Return value
        End Function

        ''' <summary>
        ''' Returns the value of this {@code Long} as a {@code float} after
        ''' a widening primitive conversion.
        ''' @jls 5.1.2 Widening Primitive Conversions
        ''' </summary>
        Public Overrides Function floatValue() As Single
            Return CSng(value)
        End Function

        ''' <summary>
        ''' Returns the value of this {@code Long} as a {@code double}
        ''' after a widening primitive conversion.
        ''' @jls 5.1.2 Widening Primitive Conversions
        ''' </summary>
        Public Overrides Function doubleValue() As Double
            Return CDbl(value)
        End Function

        ''' <summary>
        ''' Returns a {@code String} object representing this
        ''' {@code Long}'s value.  The value is converted to signed
        ''' decimal representation and returned as a string, exactly as if
        ''' the {@code long} value were given as an argument to the
        ''' <seealso cref="java.lang.Long#toString(long)"/> method.
        ''' </summary>
        ''' <returns>  a string representation of the value of this object in
        '''          base&nbsp;10. </returns>
        Public Overrides Function ToString() As String
            Return ToString(value)
        End Function

        ''' <summary>
        ''' Returns a hash code for this {@code Long}. The result is
        ''' the exclusive OR of the two halves of the primitive
        ''' {@code long} value held by this {@code Long}
        ''' object. That is, the hashcode is the value of the expression:
        ''' 
        ''' <blockquote>
        '''  {@code (int)(this.longValue()^(this.longValue()>>>32))}
        ''' </blockquote>
        ''' </summary>
        ''' <returns>  a hash code value for this object. </returns>
        Public Overrides Function GetHashCode() As Integer
            Return Long.hashCode(value)
        End Function

        ''' <summary>
        ''' Returns a hash code for a {@code long} value; compatible with
        ''' {@code Long.hashCode()}.
        ''' </summary>
        ''' <param name="value"> the value to hash </param>
        ''' <returns> a hash code value for a {@code long} value.
        ''' @since 1.8 </returns>
        Public Shared Function GetHashCode(ByVal value As Long) As Integer
            Return CInt(Fix(value Xor (CLng(CULng(value) >> 32))))
        End Function

        ''' <summary>
        ''' Compares this object to the specified object.  The result is
        ''' {@code true} if and only if the argument is not
        ''' {@code null} and is a {@code Long} object that
        ''' contains the same {@code long} value as this object.
        ''' </summary>
        ''' <param name="obj">   the object to compare with. </param>
        ''' <returns>  {@code true} if the objects are the same;
        '''          {@code false} otherwise. </returns>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If TypeOf obj Is Long? Then Return value = CLng(Fix(obj))
            Return False
        End Function

        ''' <summary>
        ''' Determines the {@code long} value of the system property
        ''' with the specified name.
        ''' 
        ''' <p>The first argument is treated as the name of a system
        ''' property.  System properties are accessible through the {@link
        ''' java.lang.System#getProperty(java.lang.String)} method. The
        ''' string value of this property is then interpreted as a {@code
        ''' long} value using the grammar supported by <seealso cref="Long#decode decode"/>
        ''' and a {@code Long} object representing this value is returned.
        ''' 
        ''' <p>If there is no property with the specified name, if the
        ''' specified name is empty or {@code null}, or if the property
        ''' does not have the correct numeric format, then {@code null} is
        ''' returned.
        ''' 
        ''' <p>In other words, this method returns a {@code Long} object
        ''' equal to the value of:
        ''' 
        ''' <blockquote>
        '''  {@code getLong(nm, null)}
        ''' </blockquote>
        ''' </summary>
        ''' <param name="nm">   property name. </param>
        ''' <returns>  the {@code Long} value of the property. </returns>
        ''' <exception cref="SecurityException"> for the same reasons as
        '''          <seealso cref="System#getProperty(String) System.getProperty"/> </exception>
        ''' <seealso cref=     java.lang.System#getProperty(java.lang.String) </seealso>
        ''' <seealso cref=     java.lang.System#getProperty(java.lang.String, java.lang.String) </seealso>
        Public Shared Function getLong(ByVal nm As String) As Long?
            Return getLong(nm, Nothing)
        End Function

        ''' <summary>
        ''' Determines the {@code long} value of the system property
        ''' with the specified name.
        ''' 
        ''' <p>The first argument is treated as the name of a system
        ''' property.  System properties are accessible through the {@link
        ''' java.lang.System#getProperty(java.lang.String)} method. The
        ''' string value of this property is then interpreted as a {@code
        ''' long} value using the grammar supported by <seealso cref="Long#decode decode"/>
        ''' and a {@code Long} object representing this value is returned.
        ''' 
        ''' <p>The second argument is the default value. A {@code Long} object
        ''' that represents the value of the second argument is returned if there
        ''' is no property of the specified name, if the property does not have
        ''' the correct numeric format, or if the specified name is empty or null.
        ''' 
        ''' <p>In other words, this method returns a {@code Long} object equal
        ''' to the value of:
        ''' 
        ''' <blockquote>
        '''  {@code getLong(nm, new Long(val))}
        ''' </blockquote>
        ''' 
        ''' but in practice it may be implemented in a manner such as:
        ''' 
        ''' <blockquote><pre>
        ''' Long result = getLong(nm, null);
        ''' return (result == null) ? new Long(val) : result;
        ''' </pre></blockquote>
        ''' 
        ''' to avoid the unnecessary allocation of a {@code Long} object when
        ''' the default value is not needed.
        ''' </summary>
        ''' <param name="nm">    property name. </param>
        ''' <param name="val">   default value. </param>
        ''' <returns>  the {@code Long} value of the property. </returns>
        ''' <exception cref="SecurityException"> for the same reasons as
        '''          <seealso cref="System#getProperty(String) System.getProperty"/> </exception>
        ''' <seealso cref=     java.lang.System#getProperty(java.lang.String) </seealso>
        ''' <seealso cref=     java.lang.System#getProperty(java.lang.String, java.lang.String) </seealso>
        Public Shared Function getLong(ByVal nm As String, ByVal val As Long) As Long?
            Dim result As Long? = Long.getLong(nm, Nothing)
            Return If(result Is Nothing, Convert.ToInt64(val), result)
        End Function

        ''' <summary>
        ''' Returns the {@code long} value of the system property with
        ''' the specified name.  The first argument is treated as the name
        ''' of a system property.  System properties are accessible through
        ''' the <seealso cref="java.lang.System#getProperty(java.lang.String)"/>
        ''' method. The string value of this property is then interpreted
        ''' as a {@code long} value, as per the
        ''' <seealso cref="Long#decode decode"/> method, and a {@code Long} object
        ''' representing this value is returned; in summary:
        ''' 
        ''' <ul>
        ''' <li>If the property value begins with the two ASCII characters
        ''' {@code 0x} or the ASCII character {@code #}, not followed by
        ''' a minus sign, then the rest of it is parsed as a hexadecimal integer
        ''' exactly as for the method <seealso cref="#valueOf(java.lang.String, int)"/>
        ''' with radix 16.
        ''' <li>If the property value begins with the ASCII character
        ''' {@code 0} followed by another character, it is parsed as
        ''' an octal integer exactly as by the method {@link
        ''' #valueOf(java.lang.String, int)} with radix 8.
        ''' <li>Otherwise the property value is parsed as a decimal
        ''' integer exactly as by the method
        ''' <seealso cref="#valueOf(java.lang.String, int)"/> with radix 10.
        ''' </ul>
        ''' 
        ''' <p>Note that, in every case, neither {@code L}
        ''' ({@code '\u005Cu004C'}) nor {@code l}
        ''' ({@code '\u005Cu006C'}) is permitted to appear at the end
        ''' of the property value as a type indicator, as would be
        ''' permitted in Java programming language source code.
        ''' 
        ''' <p>The second argument is the default value. The default value is
        ''' returned if there is no property of the specified name, if the
        ''' property does not have the correct numeric format, or if the
        ''' specified name is empty or {@code null}.
        ''' </summary>
        ''' <param name="nm">   property name. </param>
        ''' <param name="val">   default value. </param>
        ''' <returns>  the {@code Long} value of the property. </returns>
        ''' <exception cref="SecurityException"> for the same reasons as
        '''          <seealso cref="System#getProperty(String) System.getProperty"/> </exception>
        ''' <seealso cref=     System#getProperty(java.lang.String) </seealso>
        ''' <seealso cref=     System#getProperty(java.lang.String, java.lang.String) </seealso>
        Public Shared Function getLong(ByVal nm As String, ByVal val As Long?) As Long?
            Dim v As String = Nothing
            Try
                v = System.getProperty(nm)
                'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
            Catch IllegalArgumentException Or NullPointerException e
			End Try
            If v IsNot Nothing Then
                Try
                    Return Long.decode(v)
                Catch e As NumberFormatException
                End Try
            End If
            Return val
        End Function

        ''' <summary>
        ''' Compares two {@code Long} objects numerically.
        ''' </summary>
        ''' <param name="anotherLong">   the {@code Long} to be compared. </param>
        ''' <returns>  the value {@code 0} if this {@code Long} is
        '''          equal to the argument {@code Long}; a value less than
        '''          {@code 0} if this {@code Long} is numerically less
        '''          than the argument {@code Long}; and a value greater
        '''          than {@code 0} if this {@code Long} is numerically
        '''           greater than the argument {@code Long} (signed
        '''           comparison).
        ''' @since   1.2 </returns>
        Public Function compareTo(ByVal anotherLong As Long?) As Integer
            Return compare(Me.value, anotherLong.Value)
        End Function

        ''' <summary>
        ''' Compares two {@code long} values numerically.
        ''' The value returned is identical to what would be returned by:
        ''' <pre>
        '''    Long.valueOf(x).compareTo(Long.valueOf(y))
        ''' </pre>
        ''' </summary>
        ''' <param name="x"> the first {@code long} to compare </param>
        ''' <param name="y"> the second {@code long} to compare </param>
        ''' <returns> the value {@code 0} if {@code x == y};
        '''         a value less than {@code 0} if {@code x < y}; and
        '''         a value greater than {@code 0} if {@code x > y}
        ''' @since 1.7 </returns>
        Public Shared Function compare(ByVal x As Long, ByVal y As Long) As Integer
            Return If(x < y, -1, (If(x = y, 0, 1)))
        End Function

        ''' <summary>
        ''' Compares two {@code long} values numerically treating the values
        ''' as unsigned.
        ''' </summary>
        ''' <param name="x"> the first {@code long} to compare </param>
        ''' <param name="y"> the second {@code long} to compare </param>
        ''' <returns> the value {@code 0} if {@code x == y}; a value less
        '''         than {@code 0} if {@code x < y} as unsigned values; and
        '''         a value greater than {@code 0} if {@code x > y} as
        '''         unsigned values
        ''' @since 1.8 </returns>
        Public Shared Function compareUnsigned(ByVal x As Long, ByVal y As Long) As Integer
            Return compare(x + MIN_VALUE, y + MIN_VALUE)
        End Function


        ''' <summary>
        ''' Returns the unsigned quotient of dividing the first argument by
        ''' the second where each argument and the result is interpreted as
        ''' an unsigned value.
        ''' 
        ''' <p>Note that in two's complement arithmetic, the three other
        ''' basic arithmetic operations of add, subtract, and multiply are
        ''' bit-wise identical if the two operands are regarded as both
        ''' being signed or both being unsigned.  Therefore separate {@code
        ''' addUnsigned}, etc. methods are not provided.
        ''' </summary>
        ''' <param name="dividend"> the value to be divided </param>
        ''' <param name="divisor"> the value doing the dividing </param>
        ''' <returns> the unsigned quotient of the first argument divided by
        ''' the second argument </returns>
        ''' <seealso cref= #remainderUnsigned
        ''' @since 1.8 </seealso>
        Public Shared Function divideUnsigned(ByVal dividend As Long, ByVal divisor As Long) As Long
            If divisor < 0L Then ' signed comparison Return If((compareUnsigned(dividend, divisor)) < 0, 0L, 1L)

                If dividend > 0 Then '  Both inputs non-negative
                    Return dividend \ divisor
                Else
                    '            
                    '             * For simple code, leveraging BigInteger.  Longer and faster
                    '             * code written directly in terms of operations on longs is
                    '             * possible; see "Hacker's Delight" for divide and remainder
                    '             * algorithms.
                    '             
                    Return toUnsignedBigInteger(dividend).divide(toUnsignedBigInteger(divisor))
                End If
        End Function

        ''' <summary>
        ''' Returns the unsigned remainder from dividing the first argument
        ''' by the second where each argument and the result is interpreted
        ''' as an unsigned value.
        ''' </summary>
        ''' <param name="dividend"> the value to be divided </param>
        ''' <param name="divisor"> the value doing the dividing </param>
        ''' <returns> the unsigned remainder of the first argument divided by
        ''' the second argument </returns>
        ''' <seealso cref= #divideUnsigned
        ''' @since 1.8 </seealso>
        Public Shared Function remainderUnsigned(ByVal dividend As Long, ByVal divisor As Long) As Long
            If dividend > 0 AndAlso divisor > 0 Then ' signed comparisons
                Return dividend Mod divisor
            Else
                If compareUnsigned(dividend, divisor) < 0 Then ' Avoid explicit check for 0 divisor
                    Return dividend
                Else
                    Return toUnsignedBigInteger(dividend).remainder(toUnsignedBigInteger(divisor))
                End If
            End If
        End Function

        ' Bit Twiddling

        ''' <summary>
        ''' The number of bits used to represent a {@code long} value in two's
        ''' complement binary form.
        ''' 
        ''' @since 1.5
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Const SIZE As Integer = 64

        ''' <summary>
        ''' The number of bytes used to represent a {@code long} value in two's
        ''' complement binary form.
        ''' 
        ''' @since 1.8
        ''' </summary>
        Public Shared ReadOnly BYTES As Integer = SIZE \ Byte.SIZE

        ''' <summary>
        ''' Returns a {@code long} value with at most a single one-bit, in the
        ''' position of the highest-order ("leftmost") one-bit in the specified
        ''' {@code long} value.  Returns zero if the specified value has no
        ''' one-bits in its two's complement binary representation, that is, if it
        ''' is equal to zero.
        ''' </summary>
        ''' <param name="i"> the value whose highest one bit is to be computed </param>
        ''' <returns> a {@code long} value with a single one-bit, in the position
        '''     of the highest-order one-bit in the specified value, or zero if
        '''     the specified value is itself equal to zero.
        ''' @since 1.5 </returns>
        Public Shared Function highestOneBit(ByVal i As Long) As Long
            ' HD, Figure 3-1
            i = i Or (i >> 1)
            i = i Or (i >> 2)
            i = i Or (i >> 4)
            i = i Or (i >> 8)
            i = i Or (i >> 16)
            i = i Or (i >> 32)
            Return i - (CLng(CULng(i) >> 1))
        End Function

        ''' <summary>
        ''' Returns a {@code long} value with at most a single one-bit, in the
        ''' position of the lowest-order ("rightmost") one-bit in the specified
        ''' {@code long} value.  Returns zero if the specified value has no
        ''' one-bits in its two's complement binary representation, that is, if it
        ''' is equal to zero.
        ''' </summary>
        ''' <param name="i"> the value whose lowest one bit is to be computed </param>
        ''' <returns> a {@code long} value with a single one-bit, in the position
        '''     of the lowest-order one-bit in the specified value, or zero if
        '''     the specified value is itself equal to zero.
        ''' @since 1.5 </returns>
        Public Shared Function lowestOneBit(ByVal i As Long) As Long
            ' HD, Section 2-1
            Return i And -i
        End Function

        ''' <summary>
        ''' Returns the number of zero bits preceding the highest-order
        ''' ("leftmost") one-bit in the two's complement binary representation
        ''' of the specified {@code long} value.  Returns 64 if the
        ''' specified value has no one-bits in its two's complement representation,
        ''' in other words if it is equal to zero.
        ''' 
        ''' <p>Note that this method is closely related to the logarithm base 2.
        ''' For all positive {@code long} values x:
        ''' <ul>
        ''' <li>floor(log<sub>2</sub>(x)) = {@code 63 - numberOfLeadingZeros(x)}
        ''' <li>ceil(log<sub>2</sub>(x)) = {@code 64 - numberOfLeadingZeros(x - 1)}
        ''' </ul>
        ''' </summary>
        ''' <param name="i"> the value whose number of leading zeros is to be computed </param>
        ''' <returns> the number of zero bits preceding the highest-order
        '''     ("leftmost") one-bit in the two's complement binary representation
        '''     of the specified {@code long} value, or 64 if the value
        '''     is equal to zero.
        ''' @since 1.5 </returns>
        Public Shared Function numberOfLeadingZeros(ByVal i As Long) As Integer
            ' HD, Figure 5-6
            If i = 0 Then Return 64
            Dim n As Integer = 1
            Dim x As Integer = CInt(CLng(CULng(i) >> 32))
            If x = 0 Then
                n += 32
                x = CInt(i)
            End If
            If CInt(CUInt(x) >> 16 = 0) Then
                n += 16
                x <<= 16
            End If
            If CInt(CUInt(x) >> 24 = 0) Then
                n += 8
                x <<= 8
            End If
            If CInt(CUInt(x) >> 28 = 0) Then
                n += 4
                x <<= 4
            End If
            If CInt(CUInt(x) >> 30 = 0) Then
                n += 2
                x <<= 2
            End If
            n -= CInt(CUInt(x) >> 31)
            Return n
        End Function

        ''' <summary>
        ''' Returns the number of zero bits following the lowest-order ("rightmost")
        ''' one-bit in the two's complement binary representation of the specified
        ''' {@code long} value.  Returns 64 if the specified value has no
        ''' one-bits in its two's complement representation, in other words if it is
        ''' equal to zero.
        ''' </summary>
        ''' <param name="i"> the value whose number of trailing zeros is to be computed </param>
        ''' <returns> the number of zero bits following the lowest-order ("rightmost")
        '''     one-bit in the two's complement binary representation of the
        '''     specified {@code long} value, or 64 if the value is equal
        '''     to zero.
        ''' @since 1.5 </returns>
        Public Shared Function numberOfTrailingZeros(ByVal i As Long) As Integer
            ' HD, Figure 5-14
            Dim x, y As Integer
            If i = 0 Then Return 64
            Dim n As Integer = 63
            y = CInt(i)
            If y <> 0 Then
                n = n - 32
                x = y
            Else
                x = CInt(CLng(CULng(i) >> 32))
            End If
            y = x << 16
            If y <> 0 Then
                n = n - 16
                x = y
            End If
            y = x << 8
            If y <> 0 Then
                n = n - 8
                x = y
            End If
            y = x << 4
            If y <> 0 Then
                n = n - 4
                x = y
            End If
            y = x << 2
            If y <> 0 Then
                n = n - 2
                x = y
            End If
            Return n - (CInt(CUInt((x << 1)) >> 31))
        End Function

        ''' <summary>
        ''' Returns the number of one-bits in the two's complement binary
        ''' representation of the specified {@code long} value.  This function is
        ''' sometimes referred to as the <i>population count</i>.
        ''' </summary>
        ''' <param name="i"> the value whose bits are to be counted </param>
        ''' <returns> the number of one-bits in the two's complement binary
        '''     representation of the specified {@code long} value.
        ''' @since 1.5 </returns>
        Public Shared Function bitCount(ByVal i As Long) As Integer
            ' HD, Figure 5-14
            i = i - ((CLng(CULng(i) >> 1)) And &H5555555555555555L)
            i = (i And &H3333333333333333L) + ((CLng(CULng(i) >> 2)) And &H3333333333333333L)
            i = (i + (CLng(CULng(i) >> 4))) And &HF0F0F0F0F0F0F0FL
            i = i + (CLng(CULng(i) >> 8))
            i = i + (CLng(CULng(i) >> 16))
            i = i + (CLng(CULng(i) >> 32))
            Return CInt(i) And &H7F
        End Function

        ''' <summary>
        ''' Returns the value obtained by rotating the two's complement binary
        ''' representation of the specified {@code long} value left by the
        ''' specified number of bits.  (Bits shifted out of the left hand, or
        ''' high-order, side reenter on the right, or low-order.)
        ''' 
        ''' <p>Note that left rotation with a negative distance is equivalent to
        ''' right rotation: {@code rotateLeft(val, -distance) == rotateRight(val,
        ''' distance)}.  Note also that rotation by any multiple of 64 is a
        ''' no-op, so all but the last six bits of the rotation distance can be
        ''' ignored, even if the distance is negative: {@code rotateLeft(val,
        ''' distance) == rotateLeft(val, distance & 0x3F)}.
        ''' </summary>
        ''' <param name="i"> the value whose bits are to be rotated left </param>
        ''' <param name="distance"> the number of bit positions to rotate left </param>
        ''' <returns> the value obtained by rotating the two's complement binary
        '''     representation of the specified {@code long} value left by the
        '''     specified number of bits.
        ''' @since 1.5 </returns>
        Public Shared Function rotateLeft(ByVal i As Long, ByVal distance As Integer) As Long
            Return (i << distance) Or (CLng(CULng(i) >> -distance))
        End Function

        ''' <summary>
        ''' Returns the value obtained by rotating the two's complement binary
        ''' representation of the specified {@code long} value right by the
        ''' specified number of bits.  (Bits shifted out of the right hand, or
        ''' low-order, side reenter on the left, or high-order.)
        ''' 
        ''' <p>Note that right rotation with a negative distance is equivalent to
        ''' left rotation: {@code rotateRight(val, -distance) == rotateLeft(val,
        ''' distance)}.  Note also that rotation by any multiple of 64 is a
        ''' no-op, so all but the last six bits of the rotation distance can be
        ''' ignored, even if the distance is negative: {@code rotateRight(val,
        ''' distance) == rotateRight(val, distance & 0x3F)}.
        ''' </summary>
        ''' <param name="i"> the value whose bits are to be rotated right </param>
        ''' <param name="distance"> the number of bit positions to rotate right </param>
        ''' <returns> the value obtained by rotating the two's complement binary
        '''     representation of the specified {@code long} value right by the
        '''     specified number of bits.
        ''' @since 1.5 </returns>
        Public Shared Function rotateRight(ByVal i As Long, ByVal distance As Integer) As Long
            Return (CLng(CULng(i) >> distance)) Or (i << -distance)
        End Function

        ''' <summary>
        ''' Returns the value obtained by reversing the order of the bits in the
        ''' two's complement binary representation of the specified {@code long}
        ''' value.
        ''' </summary>
        ''' <param name="i"> the value to be reversed </param>
        ''' <returns> the value obtained by reversing order of the bits in the
        '''     specified {@code long} value.
        ''' @since 1.5 </returns>
        Public Shared Function reverse(ByVal i As Long) As Long
            ' HD, Figure 7-1
            i = (i And &H5555555555555555L) << 1 Or (CLng(CULng(i) >> 1)) And &H5555555555555555L
            i = (i And &H3333333333333333L) << 2 Or (CLng(CULng(i) >> 2)) And &H3333333333333333L
            i = (i And &HF0F0F0F0F0F0F0FL) << 4 Or (CLng(CULng(i) >> 4)) And &HF0F0F0F0F0F0F0FL
            i = (i And &HFF00FF00FF00FFL) << 8 Or (CLng(CULng(i) >> 8)) And &HFF00FF00FF00FFL
            i = (i << 48) Or ((i And &HFFFF0000L) << 16) Or ((CLng(CULng(i) >> 16)) And &HFFFF0000L) Or (CLng(CULng(i) >> 48))
            Return i
        End Function

        ''' <summary>
        ''' Returns the signum function of the specified {@code long} value.  (The
        ''' return value is -1 if the specified value is negative; 0 if the
        ''' specified value is zero; and 1 if the specified value is positive.)
        ''' </summary>
        ''' <param name="i"> the value whose signum is to be computed </param>
        ''' <returns> the signum function of the specified {@code long} value.
        ''' @since 1.5 </returns>
        Public Shared Function signum(ByVal i As Long) As Integer
            ' HD, Section 2-7
            Return CInt(Fix((i >> 63) Or (-CLng(CULng(i) >> 63))))
        End Function

        ''' <summary>
        ''' Returns the value obtained by reversing the order of the bytes in the
        ''' two's complement representation of the specified {@code long} value.
        ''' </summary>
        ''' <param name="i"> the value whose bytes are to be reversed </param>
        ''' <returns> the value obtained by reversing the bytes in the specified
        '''     {@code long} value.
        ''' @since 1.5 </returns>
        Public Shared Function reverseBytes(ByVal i As Long) As Long
            i = (i And &HFF00FF00FF00FFL) << 8 Or (CLng(CULng(i) >> 8)) And &HFF00FF00FF00FFL
            Return (i << 48) Or ((i And &HFFFF0000L) << 16) Or ((CLng(CULng(i) >> 16)) And &HFFFF0000L) Or (CLng(CULng(i) >> 48))
        End Function

        ''' <summary>
        ''' Adds two {@code long} values together as per the + operator.
        ''' </summary>
        ''' <param name="a"> the first operand </param>
        ''' <param name="b"> the second operand </param>
        ''' <returns> the sum of {@code a} and {@code b} </returns>
        ''' <seealso cref= java.util.function.BinaryOperator
        ''' @since 1.8 </seealso>
        Public Shared Function sum(ByVal a As Long, ByVal b As Long) As Long
            Return a + b
        End Function

        ''' <summary>
        ''' Returns the greater of two {@code long} values
        ''' as if by calling <seealso cref="Math#max(long, long) Math.max"/>.
        ''' </summary>
        ''' <param name="a"> the first operand </param>
        ''' <param name="b"> the second operand </param>
        ''' <returns> the greater of {@code a} and {@code b} </returns>
        ''' <seealso cref= java.util.function.BinaryOperator
        ''' @since 1.8 </seealso>
        Public Shared Function max(ByVal a As Long, ByVal b As Long) As Long
            Return Math.max(a, b)
        End Function

        ''' <summary>
        ''' Returns the smaller of two {@code long} values
        ''' as if by calling <seealso cref="Math#min(long, long) Math.min"/>.
        ''' </summary>
        ''' <param name="a"> the first operand </param>
        ''' <param name="b"> the second operand </param>
        ''' <returns> the smaller of {@code a} and {@code b} </returns>
        ''' <seealso cref= java.util.function.BinaryOperator
        ''' @since 1.8 </seealso>
        Public Shared Function min(ByVal a As Long, ByVal b As Long) As Long
            Return Math.min(a, b)
        End Function

        ''' <summary>
        ''' use serialVersionUID from JDK 1.0.2 for interoperability </summary>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Private Const serialVersionUID As Long = 4290774380558885855L
    End Class

End Namespace
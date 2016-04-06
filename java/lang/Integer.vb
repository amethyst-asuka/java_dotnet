Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics

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
    ''' The {@code Integer} class wraps a value of the primitive type
    ''' {@code int} in an object. An object of type {@code Integer}
    ''' contains a single field whose type is {@code int}.
    ''' 
    ''' <p>In addition, this class provides several methods for converting
    ''' an {@code int} to a {@code String} and a {@code String} to an
    ''' {@code int}, as well as other constants and methods useful when
    ''' dealing with an {@code int}.
    ''' 
    ''' <p>Implementation note: The implementations of the "bit twiddling"
    ''' methods (such as <seealso cref="#highestOneBit(int) highestOneBit"/> and
    ''' <seealso cref="#numberOfTrailingZeros(int) numberOfTrailingZeros"/>) are
    ''' based on material from Henry S. Warren, Jr.'s <i>Hacker's
    ''' Delight</i>, (Addison Wesley, 2002).
    ''' 
    ''' @author  Lee Boynton
    ''' @author  Arthur van Hoff
    ''' @author  Josh Bloch
    ''' @author  Joseph D. Darcy
    ''' @since JDK1.0
    ''' </summary>
    Public NotInheritable Class [Integer]
        Inherits Number
        Implements Comparable(Of [Integer])

        ''' <summary>
        ''' A constant holding the minimum value an {@code int} can
        ''' have, -2<sup>31</sup>.
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Const MIN_VALUE As Integer = &H80000000L

        ''' <summary>
        ''' A constant holding the maximum value an {@code int} can
        ''' have, 2<sup>31</sup>-1.
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Const MAX_VALUE As Integer = &H7FFFFFFF

        ''' <summary>
        ''' The {@code Class} instance representing the primitive type
        ''' {@code int}.
        ''' 
        ''' @since   JDK1.1
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Shared ReadOnly TYPE As [Class] = CType([Class].getPrimitiveClass("int"), [Class])

        ''' <summary>
        ''' All possible chars for representing a number as a String
        ''' </summary>
        Friend Shared ReadOnly digits As Char() = {"0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "a"c, "b"c, "c"c, "d"c, "e"c, "f"c, "g"c, "h"c, "i"c, "j"c, "k"c, "l"c, "m"c, "n"c, "o"c, "p"c, "q"c, "r"c, "s"c, "t"c, "u"c, "v"c, "w"c, "x"c, "y"c, "z"c}

        ''' <summary>
        ''' Returns a string representation of the first argument in the
        ''' radix specified by the second argument.
        ''' 
        ''' <p>If the radix is smaller than {@code Character.MIN_RADIX}
        ''' or larger than {@code Character.MAX_RADIX}, then the radix
        ''' {@code 10} is used instead.
        ''' 
        ''' <p>If the first argument is negative, the first element of the
        ''' result is the ASCII minus character {@code '-'}
        ''' ({@code '\u005Cu002D'}). If the first argument is not
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
        ''' {@code '\u005Cu007A'}. If {@code radix} is
        ''' <var>N</var>, then the first <var>N</var> of these characters
        ''' are used as radix-<var>N</var> digits in the order shown. Thus,
        ''' the digits for hexadecimal (radix 16) are
        ''' {@code 0123456789abcdef}. If uppercase letters are
        ''' desired, the <seealso cref="java.lang.String#toUpperCase()"/> method may
        ''' be called on the result:
        ''' 
        ''' <blockquote>
        '''  {@code  java.lang.[Integer].toString(n, 16).toUpperCase()}
        ''' </blockquote>
        ''' </summary>
        ''' <param name="i">       an integer to be converted to a string. </param>
        ''' <param name="radix">   the radix to use in the string representation. </param>
        ''' <returns>  a string representation of the argument in the specified radix. </returns>
        ''' <seealso cref=     java.lang.Character#MAX_RADIX </seealso>
        ''' <seealso cref=     java.lang.Character#MIN_RADIX </seealso>
        Public Overloads Shared Function ToString(  i As Integer,   radix As Integer) As String
            If radix < Character.MIN_RADIX OrElse radix > Character.MAX_RADIX Then radix = 10

            ' Use the faster version 
            If radix = 10 Then Return ToString(i)

            Dim buf As Char() = New Char(32) {}
            Dim negative As Boolean = (i < 0)
            Dim charPos As Integer = 32

            If Not negative Then i = -i

            Do While i <= -radix
                buf(charPos) = digits(-(i Mod radix))
                charPos -= 1
                i = i \ radix
            Loop
            buf(charPos) = digits(-i)

            If negative Then
                charPos -= 1
                buf(charPos) = "-"c
            End If

            Return New String(buf, charPos, (33 - charPos))
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
        ''' are the same as <seealso cref="#toString(int, int) toString"/>.
        ''' </summary>
        ''' <param name="i">       an integer to be converted to an unsigned string. </param>
        ''' <param name="radix">   the radix to use in the string representation. </param>
        ''' <returns>  an unsigned string representation of the argument in the specified radix. </returns>
        ''' <seealso cref=     #toString(int, int)
        ''' @since 1.8 </seealso>
        Public Shared Function toUnsignedString(  i As Integer,   radix As Integer) As String
            Return java.lang.[Long].toUnsignedString(toUnsignedLong(i), radix)
        End Function

        ''' <summary>
        ''' Returns a string representation of the integer argument as an
        ''' unsigned integer in base&nbsp;16.
        ''' 
        ''' <p>The unsigned integer value is the argument plus 2<sup>32</sup>
        ''' if the argument is negative; otherwise, it is equal to the
        ''' argument.  This value is converted to a string of ASCII digits
        ''' in hexadecimal (base&nbsp;16) with no extra leading
        ''' {@code 0}s.
        ''' 
        ''' <p>The value of the argument can be recovered from the returned
        ''' string {@code s} by calling {@link
        ''' Integer#parseUnsignedInt(String, int)
        '''  java.lang.[Integer].parseUnsignedInt(s, 16)}.
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
        ''' {@code '\u005Cu0039'} and {@code '\u005Cu0061'} through
        ''' {@code '\u005Cu0066'}. If uppercase letters are
        ''' desired, the <seealso cref="java.lang.String#toUpperCase()"/> method may
        ''' be called on the result:
        ''' 
        ''' <blockquote>
        '''  {@code  java.lang.[Integer].toHexString(n).toUpperCase()}
        ''' </blockquote>
        ''' </summary>
        ''' <param name="i">   an integer to be converted to a string. </param>
        ''' <returns>  the string representation of the unsigned integer value
        '''          represented by the argument in hexadecimal (base&nbsp;16). </returns>
        ''' <seealso cref= #parseUnsignedInt(String, int) </seealso>
        ''' <seealso cref= #toUnsignedString(int, int)
        ''' @since   JDK1.0.2 </seealso>
        Public Shared Function toHexString(  i As Integer) As String
            Return toUnsignedString0(i, 4)
        End Function

        ''' <summary>
        ''' Returns a string representation of the integer argument as an
        ''' unsigned integer in base&nbsp;8.
        ''' 
        ''' <p>The unsigned integer value is the argument plus 2<sup>32</sup>
        ''' if the argument is negative; otherwise, it is equal to the
        ''' argument.  This value is converted to a string of ASCII digits
        ''' in octal (base&nbsp;8) with no extra leading {@code 0}s.
        ''' 
        ''' <p>The value of the argument can be recovered from the returned
        ''' string {@code s} by calling {@link
        ''' Integer#parseUnsignedInt(String, int)
        '''  java.lang.[Integer].parseUnsignedInt(s, 8)}.
        ''' 
        ''' <p>If the unsigned magnitude is zero, it is represented by a
        ''' single zero character {@code '0'} ({@code '\u005Cu0030'});
        ''' otherwise, the first character of the representation of the
        ''' unsigned magnitude will not be the zero character. The
        ''' following characters are used as octal digits:
        ''' 
        ''' <blockquote>
        ''' {@code 01234567}
        ''' </blockquote>
        ''' 
        ''' These are the characters {@code '\u005Cu0030'} through
        ''' {@code '\u005Cu0037'}.
        ''' </summary>
        ''' <param name="i">   an integer to be converted to a string. </param>
        ''' <returns>  the string representation of the unsigned integer value
        '''          represented by the argument in octal (base&nbsp;8). </returns>
        ''' <seealso cref= #parseUnsignedInt(String, int) </seealso>
        ''' <seealso cref= #toUnsignedString(int, int)
        ''' @since   JDK1.0.2 </seealso>
        Public Shared Function toOctalString(  i As Integer) As String
            Return toUnsignedString0(i, 3)
        End Function

        ''' <summary>
        ''' Returns a string representation of the integer argument as an
        ''' unsigned integer in base&nbsp;2.
        ''' 
        ''' <p>The unsigned integer value is the argument plus 2<sup>32</sup>
        ''' if the argument is negative; otherwise it is equal to the
        ''' argument.  This value is converted to a string of ASCII digits
        ''' in binary (base&nbsp;2) with no extra leading {@code 0}s.
        ''' 
        ''' <p>The value of the argument can be recovered from the returned
        ''' string {@code s} by calling {@link
        ''' Integer#parseUnsignedInt(String, int)
        '''  java.lang.[Integer].parseUnsignedInt(s, 2)}.
        ''' 
        ''' <p>If the unsigned magnitude is zero, it is represented by a
        ''' single zero character {@code '0'} ({@code '\u005Cu0030'});
        ''' otherwise, the first character of the representation of the
        ''' unsigned magnitude will not be the zero character. The
        ''' characters {@code '0'} ({@code '\u005Cu0030'}) and {@code
        ''' '1'} ({@code '\u005Cu0031'}) are used as binary digits.
        ''' </summary>
        ''' <param name="i">   an integer to be converted to a string. </param>
        ''' <returns>  the string representation of the unsigned integer value
        '''          represented by the argument in binary (base&nbsp;2). </returns>
        ''' <seealso cref= #parseUnsignedInt(String, int) </seealso>
        ''' <seealso cref= #toUnsignedString(int, int)
        ''' @since   JDK1.0.2 </seealso>
        Public Shared Function toBinaryString(  i As Integer) As String
            Return toUnsignedString0(i, 1)
        End Function

        ''' <summary>
        ''' Convert the integer to an unsigned number.
        ''' </summary>
        Private Shared Function toUnsignedString0(  val As Integer,   shift As Integer) As String
            ' assert shift > 0 && shift <=5 : "Illegal shift value";
            Dim mag As Integer = java.lang.[Integer].SIZE - java.lang.[Integer].numberOfLeadingZeros(val)
            Dim chars_Renamed As Integer = Global.System.Math.Max(((mag + (shift - 1)) \ shift), 1)
            Dim buf As Char() = New Char(chars_Renamed - 1) {}

            formatUnsignedInt(val, shift, buf, 0, chars_Renamed)

            ' Use special constructor which takes over "buf".
            Return New String(buf, True)
        End Function

        ''' <summary>
        ''' Format a long (treated as unsigned) into a character buffer. </summary>
        ''' <param name="val"> the unsigned int to format </param>
        ''' <param name="shift"> the log2 of the base to format in (4 for hex, 3 for octal, 1 for binary) </param>
        ''' <param name="buf"> the character buffer to write to </param>
        ''' <param name="offset"> the offset in the destination buffer to start at </param>
        ''' <param name="len"> the number of characters to write </param>
        ''' <returns> the lowest character  location used </returns>
        Friend Shared Function formatUnsignedInt(  val As Integer,   shift As Integer,   buf As Char(),   offset As Integer,   len As Integer) As Integer
            Dim charPos As Integer = len
            Dim radix As Integer = 1 << shift
            Dim mask As Integer = radix - 1
            Do
                charPos -= 1
                buf(offset + charPos) = java.lang.[Integer].digits(val And mask)
                val >>>= shift
            Loop While val <> 0 AndAlso charPos > 0

            Return charPos
        End Function

        Friend Shared ReadOnly DigitTens As Char() = {"0"c, "0"c, "0"c, "0"c, "0"c, "0"c, "0"c, "0"c, "0"c, "0"c, "1"c, "1"c, "1"c, "1"c, "1"c, "1"c, "1"c, "1"c, "1"c, "1"c, "2"c, "2"c, "2"c, "2"c, "2"c, "2"c, "2"c, "2"c, "2"c, "2"c, "3"c, "3"c, "3"c, "3"c, "3"c, "3"c, "3"c, "3"c, "3"c, "3"c, "4"c, "4"c, "4"c, "4"c, "4"c, "4"c, "4"c, "4"c, "4"c, "4"c, "5"c, "5"c, "5"c, "5"c, "5"c, "5"c, "5"c, "5"c, "5"c, "5"c, "6"c, "6"c, "6"c, "6"c, "6"c, "6"c, "6"c, "6"c, "6"c, "6"c, "7"c, "7"c, "7"c, "7"c, "7"c, "7"c, "7"c, "7"c, "7"c, "7"c, "8"c, "8"c, "8"c, "8"c, "8"c, "8"c, "8"c, "8"c, "8"c, "8"c, "9"c, "9"c, "9"c, "9"c, "9"c, "9"c, "9"c, "9"c, "9"c, "9"c}

        Friend Shared ReadOnly DigitOnes As Char() = {"0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c}

        ' I use the "invariant division by multiplication" trick to
        ' accelerate  java.lang.[Integer].toString.  In particular we want to
        ' avoid division by 10.
        '
        ' The "trick" has roughly the same performance characteristics
        ' as the "classic"  java.lang.[Integer].toString code on a non-JIT VM.
        ' The trick avoids .rem and .div calls but has a longer code
        ' path and is thus dominated by dispatch overhead.  In the
        ' JIT case the dispatch overhead doesn't exist and the
        ' "trick" is considerably faster than the classic code.
        '
        ' TODO-FIXME: convert (x * 52429) into the equiv shift-add
        ' sequence.
        '
        ' RE:  Division by Invariant Integers using Multiplication
        '      T Gralund, P Montgomery
        '      ACM PLDI 1994
        '

        ''' <summary>
        ''' Returns a {@code String} object representing the
        ''' specified  java.lang.[Integer]. The argument is converted to signed decimal
        ''' representation and returned as a string, exactly as if the
        ''' argument and radix 10 were given as arguments to the {@link
        ''' #toString(int, int)} method.
        ''' </summary>
        ''' <param name="i">   an integer to be converted. </param>
        ''' <returns>  a string representation of the argument in base&nbsp;10. </returns>
        Public Overloads Shared Function ToString(  i As Integer) As String
            If i = java.lang.[Integer].MIN_VALUE Then Return "-2147483648"
            Dim size_Renamed As Integer = If(i < 0, stringSize(-i) + 1, stringSize(i))
            Dim buf As Char() = New Char(size_Renamed - 1) {}
            getChars(i, size_Renamed, buf)
            Return New [String](buf, True)
        End Function

        ''' <summary>
        ''' Returns a string representation of the argument as an unsigned
        ''' decimal value.
        ''' 
        ''' The argument is converted to unsigned decimal representation
        ''' and returned as a string exactly as if the argument and radix
        ''' 10 were given as arguments to the {@link #toUnsignedString(int,
        ''' int)} method.
        ''' </summary>
        ''' <param name="i">  an integer to be converted to an unsigned string. </param>
        ''' <returns>  an unsigned string representation of the argument. </returns>
        ''' <seealso cref=     #toUnsignedString(int, int)
        ''' @since 1.8 </seealso>
        Public Shared Function toUnsignedString(  i As Integer) As String
            Return Convert.ToString(toUnsignedLong(i))
        End Function

        ''' <summary>
        ''' Places characters representing the integer i into the
        ''' character array buf. The characters are placed into
        ''' the buffer backwards starting with the least significant
        ''' digit at the specified index (exclusive), and working
        ''' backwards from there.
        ''' 
        ''' Will fail if i ==  java.lang.[Integer].MIN_VALUE
        ''' </summary>
        Friend Shared Sub getChars(  i As Integer,   index As Integer,   buf As Char())
            Dim q, r As Integer
            Dim charPos As Integer = index
            Dim sign As Char = Chr(0)

            If i < 0 Then
                sign = "-"c
                i = -i
            End If

            ' Generate two digits per iteration
            Do While i >= 65536
                q = i \ 100
                ' really: r = i - (q * 100);
                r = i - ((q << 6) + (q << 5) + (q << 2))
                i = q
                charPos -= 1
                buf(charPos) = DigitOnes(r)
                charPos -= 1
                buf(charPos) = DigitTens(r)
            Loop

            ' Fall thru to fast mode for smaller numbers
            ' assert(i <= 65536, i);
            Do
                q = CInt(CUInt((i * 52429)) >> (16 + 3))
                r = i - ((q << 3) + (q << 1)) ' r = i-(q*10) ...
                charPos -= 1
                buf(charPos) = digits(r)
                i = q
                If i = 0 Then Exit Do
            Loop
            If AscW(sign) <> 0 Then
                charPos -= 1
                buf(charPos) = sign
            End If
        End Sub

        Friend Shared ReadOnly sizeTable As Integer() = {9, 99, 999, 9999, 99999, 999999, 9999999, 99999999, 999999999, java.lang.[Integer].MAX_VALUE}

        ' Requires positive x
        Friend Shared Function stringSize(  x As Integer) As Integer
            Dim i As Integer = 0
            Do
                If x <= sizeTable(i) Then Return i + 1
                i += 1
            Loop
        End Function

        ''' <summary>
        ''' Parses the string argument as a signed integer in the radix
        ''' specified by the second argument. The characters in the string
        ''' must all be digits of the specified radix (as determined by
        ''' whether <seealso cref="java.lang.Character#digit(char, int)"/> returns a
        ''' nonnegative value), except that the first character may be an
        ''' ASCII minus sign {@code '-'} ({@code '\u005Cu002D'}) to
        ''' indicate a negative value or an ASCII plus sign {@code '+'}
        ''' ({@code '\u005Cu002B'}) to indicate a positive value. The
        ''' resulting integer value is returned.
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
        ''' radix, except that the first character may be a minus sign
        ''' {@code '-'} ({@code '\u005Cu002D'}) or plus sign
        ''' {@code '+'} ({@code '\u005Cu002B'}) provided that the
        ''' string is longer than length 1.
        ''' 
        ''' <li>The value represented by the string is not a value of type
        ''' {@code int}.
        ''' </ul>
        ''' 
        ''' <p>Examples:
        ''' <blockquote><pre>
        ''' parseInt("0", 10) returns 0
        ''' parseInt("473", 10) returns 473
        ''' parseInt("+42", 10) returns 42
        ''' parseInt("-0", 10) returns 0
        ''' parseInt("-FF", 16) returns -255
        ''' parseInt("1100110", 2) returns 102
        ''' parseInt("2147483647", 10) returns 2147483647
        ''' parseInt("-2147483648", 10) returns -2147483648
        ''' parseInt("2147483648", 10) throws a NumberFormatException
        ''' parseInt("99", 8) throws a NumberFormatException
        ''' parseInt("Kona", 10) throws a NumberFormatException
        ''' parseInt("Kona", 27) returns 411787
        ''' </pre></blockquote>
        ''' </summary>
        ''' <param name="s">   the {@code String} containing the integer
        '''                  representation to be parsed </param>
        ''' <param name="radix">   the radix to be used while parsing {@code s}. </param>
        ''' <returns>     the integer represented by the string argument in the
        '''             specified radix. </returns>
        ''' <exception cref="NumberFormatException"> if the {@code String}
        '''             does not contain a parsable {@code int}. </exception>
        Public Shared Function parseInt(  s As String,   radix As Integer) As Integer
            '        
            '         * WARNING: This method may be invoked early during VM initialization
            '         * before IntegerCache is initialized. Care must be taken to not use
            '         * the valueOf method.
            '         

            If s Is Nothing Then Throw New NumberFormatException("null")

            If radix < Character.MIN_RADIX Then Throw New NumberFormatException("radix " & radix & " less than Character.MIN_RADIX")

            If radix > Character.MAX_RADIX Then Throw New NumberFormatException("radix " & radix & " greater than Character.MAX_RADIX")

            Dim result As Integer = 0
            Dim negative As Boolean = False
            Dim i As Integer = 0, len As Integer = s.Length()
            Dim limit As Integer = -java.lang.[Integer].Max_Value
            Dim multmin As Integer
            Dim digit As Integer

            If len > 0 Then
                Dim firstChar As Char = s.Chars(0)
                If firstChar < "0"c Then ' Possible leading "+" or "-"
                    If firstChar = "-"c Then
                        negative = True
                        limit = java.lang.[Integer].MIN_VALUE
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
        ''' Parses the string argument as a signed decimal  java.lang.[Integer]. The
        ''' characters in the string must all be decimal digits, except
        ''' that the first character may be an ASCII minus sign {@code '-'}
        ''' ({@code '\u005Cu002D'}) to indicate a negative value or an
        ''' ASCII plus sign {@code '+'} ({@code '\u005Cu002B'}) to
        ''' indicate a positive value. The resulting integer value is
        ''' returned, exactly as if the argument and the radix 10 were
        ''' given as arguments to the {@link #parseInt(java.lang.String,
        ''' int)} method.
        ''' </summary>
        ''' <param name="s">    a {@code String} containing the {@code int}
        '''             representation to be parsed </param>
        ''' <returns>     the integer value represented by the argument in decimal. </returns>
        ''' <exception cref="NumberFormatException">  if the string does not contain a
        '''               parsable  java.lang.[Integer]. </exception>
        Public Shared Function parseInt(  s As String) As Integer
            Return parseInt(s, 10)
        End Function

        ''' <summary>
        ''' Parses the string argument as an unsigned integer in the radix
        ''' specified by the second argument.  An unsigned integer maps the
        ''' values usually associated with negative numbers to positive
        ''' numbers larger than {@code MAX_VALUE}.
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
        ''' largest unsigned {@code int}, 2<sup>32</sup>-1.
        ''' 
        ''' </ul>
        ''' 
        ''' </summary>
        ''' <param name="s">   the {@code String} containing the unsigned integer
        '''                  representation to be parsed </param>
        ''' <param name="radix">   the radix to be used while parsing {@code s}. </param>
        ''' <returns>     the integer represented by the string argument in the
        '''             specified radix. </returns>
        ''' <exception cref="NumberFormatException"> if the {@code String}
        '''             does not contain a parsable {@code int}.
        ''' @since 1.8 </exception>
        Public Shared Function parseUnsignedInt(  s As String,   radix As Integer) As Integer
            If s Is Nothing Then Throw New NumberFormatException("null")

            Dim len As Integer = s.Length()
            If len > 0 Then
                Dim firstChar As Char = s.Chars(0)
                If firstChar = "-"c Then
                    Throw New NumberFormatException(String.Format("Illegal leading minus sign " & "on unsigned string {0}.", s))
                Else
                    If len <= 5 OrElse (radix = 10 AndAlso len <= 9) Then '  java.lang.[Integer].MAX_VALUE in base 10 is 10 digits -   java.lang.[Integer].MAX_VALUE in Character.MAX_RADIX is 6 digits
                        Return parseInt(s, radix)
                    Else
                        Dim ell As Long = Convert.ToInt64(s, radix)
                        If (ell And &HFFFF_ffff_0000_0000L) = 0 Then
                            Return CInt(ell)
                        Else
                            Throw New NumberFormatException(String.Format("String value {0} exceeds " & "range of unsigned int.", s))
                        End If
                    End If
                End If
            Else
                Throw NumberFormatException.forInputString(s)
            End If
        End Function

        ''' <summary>
        ''' Parses the string argument as an unsigned decimal  java.lang.[Integer]. The
        ''' characters in the string must all be decimal digits, except
        ''' that the first character may be an an ASCII plus sign {@code
        ''' '+'} ({@code '\u005Cu002B'}). The resulting integer value
        ''' is returned, exactly as if the argument and the radix 10 were
        ''' given as arguments to the {@link
        ''' #parseUnsignedInt(java.lang.String, int)} method.
        ''' </summary>
        ''' <param name="s">   a {@code String} containing the unsigned {@code int}
        '''            representation to be parsed </param>
        ''' <returns>    the unsigned integer value represented by the argument in decimal. </returns>
        ''' <exception cref="NumberFormatException">  if the string does not contain a
        '''            parsable unsigned  java.lang.[Integer].
        ''' @since 1.8 </exception>
        Public Shared Function parseUnsignedInt(  s As String) As Integer
            Return parseUnsignedInt(s, 10)
        End Function

        ''' <summary>
        ''' Returns an {@code Integer} object holding the value
        ''' extracted from the specified {@code String} when parsed
        ''' with the radix given by the second argument. The first argument
        ''' is interpreted as representing a signed integer in the radix
        ''' specified by the second argument, exactly as if the arguments
        ''' were given to the <seealso cref="#parseInt(java.lang.String, int)"/>
        ''' method. The result is an {@code Integer} object that
        ''' represents the integer value specified by the string.
        ''' 
        ''' <p>In other words, this method returns an {@code Integer}
        ''' object equal to the value of:
        ''' 
        ''' <blockquote>
        '''  {@code new Integer( java.lang.[Integer].parseInt(s, radix))}
        ''' </blockquote>
        ''' </summary>
        ''' <param name="s">   the string to be parsed. </param>
        ''' <param name="radix"> the radix to be used in interpreting {@code s} </param>
        ''' <returns>     an {@code Integer} object holding the value
        '''             represented by the string argument in the specified
        '''             radix. </returns>
        ''' <exception cref="NumberFormatException"> if the {@code String}
        '''            does not contain a parsable {@code int}. </exception>
        Public Shared Function valueOf(  s As String,   radix As Integer) As Integer?
            Return Convert.ToInt32(parseInt(s, radix))
        End Function

        ''' <summary>
        ''' Returns an {@code Integer} object holding the
        ''' value of the specified {@code String}. The argument is
        ''' interpreted as representing a signed decimal integer, exactly
        ''' as if the argument were given to the {@link
        ''' #parseInt(java.lang.String)} method. The result is an
        ''' {@code Integer} object that represents the integer value
        ''' specified by the string.
        ''' 
        ''' <p>In other words, this method returns an {@code Integer}
        ''' object equal to the value of:
        ''' 
        ''' <blockquote>
        '''  {@code new Integer( java.lang.[Integer].parseInt(s))}
        ''' </blockquote>
        ''' </summary>
        ''' <param name="s">   the string to be parsed. </param>
        ''' <returns>     an {@code Integer} object holding the value
        '''             represented by the string argument. </returns>
        ''' <exception cref="NumberFormatException">  if the string cannot be parsed
        '''             as an  java.lang.[Integer]. </exception>
        Public Shared Function valueOf(  s As String) As Integer?
            Return Convert.ToInt32(parseInt(s, 10))
        End Function

        ''' <summary>
        ''' Cache to support the object identity semantics of autoboxing for values between
        ''' -128 and 127 (inclusive) as required by JLS.
        ''' 
        ''' The cache is initialized on first usage.  The size of the cache
        ''' may be controlled by the {@code -XX:AutoBoxCacheMax=<size>} option.
        ''' During VM initialization, java.lang. java.lang.[Integer].IntegerCache.high property
        ''' may be set and saved in the private system properties in the
        ''' sun.misc.VM class.
        ''' </summary>

        Private Class IntegerCache
            Friend Const low As Integer = -128
            Friend Shared ReadOnly high As Integer
            Friend Shared ReadOnly cache As [Integer]()

            Shared Sub New()
                ' high value may be configured by property
                Dim h As Integer = 127
                Dim integerCacheHighPropValue As String = sun.misc.VM.getSavedProperty("java.lang. java.lang.[Integer].IntegerCache.high")
                If integerCacheHighPropValue IsNot Nothing Then
                    Try
                        Dim i As Integer = parseInt(integerCacheHighPropValue)
                        i = Global.System.Math.Max(i, 127)
                        ' Maximum array size is  java.lang.[Integer].MAX_VALUE
                        h = Global.System.Math.Min(i, java.lang.[Integer].MAX_VALUE - (-low) - 1)
                    Catch nfe As NumberFormatException
                        ' If the property cannot be parsed into an int, ignore it.
                    End Try
                End If
                high = h

                cache = New [Integer]((high - low) + 1) {}
                Dim j As Integer = low
                For k As Integer = 0 To cache.Length - 1
                    cache(k) = New [Integer](j)
                    j += 1
                Next k

                ' range [-128, 127] must be interned (JLS7 5.1.7)
                Debug.Assert(IntegerCache.high >= 127)
            End Sub

            Private Sub New()
            End Sub
        End Class

        ''' <summary>
        ''' Returns an {@code Integer} instance representing the specified
        ''' {@code int} value.  If a new {@code Integer} instance is not
        ''' required, this method should generally be used in preference to
        ''' the constructor <seealso cref="#Integer(int)"/>, as this method is likely
        ''' to yield significantly better space and time performance by
        ''' caching frequently requested values.
        ''' 
        ''' This method will always cache values in the range -128 to 127,
        ''' inclusive, and may cache other values outside of this range.
        ''' </summary>
        ''' <param name="i"> an {@code int} value. </param>
        ''' <returns> an {@code Integer} instance representing {@code i}.
        ''' @since  1.5 </returns>
        Public Shared Function valueOf(  i As Integer) As [Integer]
            If i >= IntegerCache.low AndAlso i <= IntegerCache.high Then Return IntegerCache.cache(i + (-IntegerCache.low))
            Return New Integer?(i)
        End Function

        ''' <summary>
        ''' The value of the {@code Integer}.
        ''' 
        ''' @serial
        ''' </summary>
        Private ReadOnly value As Integer

        ''' <summary>
        ''' Constructs a newly allocated {@code Integer} object that
        ''' represents the specified {@code int} value.
        ''' </summary>
        ''' <param name="value">   the value to be represented by the
        '''                  {@code Integer} object. </param>
        Sub New(  value As Integer)
            Me.value = value
        End Sub

        ''' <summary>
        ''' Constructs a newly allocated {@code Integer} object that
        ''' represents the {@code int} value indicated by the
        ''' {@code String} parameter. The string is converted to an
        ''' {@code int} value in exactly the manner used by the
        ''' {@code parseInt} method for radix 10.
        ''' </summary>
        ''' <param name="s">   the {@code String} to be converted to an
        '''                 {@code Integer}. </param>
        ''' <exception cref="NumberFormatException">  if the {@code String} does not
        '''               contain a parsable  java.lang.[Integer]. </exception>
        ''' <seealso cref=        java.lang.Integer#parseInt(java.lang.String, int) </seealso>
        Sub New(  s As String)
            Me.value = parseInt(s, 10)
        End Sub

        ''' <summary>
        ''' Returns the value of this {@code Integer} as a {@code byte}
        ''' after a narrowing primitive conversion.
        ''' @jls 5.1.3 Narrowing Primitive Conversions
        ''' </summary>
        Public Overrides Function byteValue() As SByte
            Return CByte(value)
        End Function

        ''' <summary>
        ''' Returns the value of this {@code Integer} as a {@code short}
        ''' after a narrowing primitive conversion.
        ''' @jls 5.1.3 Narrowing Primitive Conversions
        ''' </summary>
        Public Overrides Function shortValue() As Short
            Return CShort(Fix(value))
        End Function

        ''' <summary>
        ''' Returns the value of this {@code Integer} as an
        ''' {@code int}.
        ''' </summary>
        Public Overrides Function intValue() As Integer
            Return value
        End Function

        ''' <summary>
        ''' Returns the value of this {@code Integer} as a {@code long}
        ''' after a widening primitive conversion.
        ''' @jls 5.1.2 Widening Primitive Conversions </summary>
        ''' <seealso cref= Integer#toUnsignedLong(int) </seealso>
        Public Overrides Function longValue() As Long
            Return CLng(Fix(value))
        End Function

        ''' <summary>
        ''' Returns the value of this {@code Integer} as a {@code float}
        ''' after a widening primitive conversion.
        ''' @jls 5.1.2 Widening Primitive Conversions
        ''' </summary>
        Public Overrides Function floatValue() As Single
            Return CSng(value)
        End Function

        ''' <summary>
        ''' Returns the value of this {@code Integer} as a {@code double}
        ''' after a widening primitive conversion.
        ''' @jls 5.1.2 Widening Primitive Conversions
        ''' </summary>
        Public Overrides Function doubleValue() As Double
            Return CDbl(value)
        End Function

        ''' <summary>
        ''' Returns a {@code String} object representing this
        ''' {@code Integer}'s value. The value is converted to signed
        ''' decimal representation and returned as a string, exactly as if
        ''' the integer value were given as an argument to the {@link
        ''' java.lang.Integer#toString(int)} method.
        ''' </summary>
        ''' <returns>  a string representation of the value of this object in
        '''          base&nbsp;10. </returns>
        Public Overrides Function ToString() As String
            Return ToString(value)
        End Function

        ''' <summary>
        ''' Returns a hash code for this {@code Integer}.
        ''' </summary>
        ''' <returns>  a hash code value for this object, equal to the
        '''          primitive {@code int} value represented by this
        '''          {@code Integer} object. </returns>
        Public Overrides Function GetHashCode() As Integer
            Return java.lang.[Integer].GetHashCode(value)
        End Function

        ''' <summary>
        ''' Returns a hash code for a {@code int} value; compatible with
        ''' {@code  java.lang.[Integer].hashCode()}.
        ''' </summary>
        ''' <param name="value"> the value to hash
        ''' @since 1.8
        ''' </param>
        ''' <returns> a hash code value for a {@code int} value. </returns>
        Public Overloads Shared Function GetHashCode(  value As Integer) As Integer
            Return value
        End Function

        ''' <summary>
        ''' Compares this object to the specified object.  The result is
        ''' {@code true} if and only if the argument is not
        ''' {@code null} and is an {@code Integer} object that
        ''' contains the same {@code int} value as this object.
        ''' </summary>
        ''' <param name="obj">   the object to compare with. </param>
        ''' <returns>  {@code true} if the objects are the same;
        '''          {@code false} otherwise. </returns>
        Public Overrides Function Equals(  obj As Object) As Boolean
            If TypeOf obj Is Integer? Then Return value = CInt(Fix(obj))
            Return False
        End Function

        ''' <summary>
        ''' Determines the integer value of the system property with the
        ''' specified name.
        ''' 
        ''' <p>The first argument is treated as the name of a system
        ''' property.  System properties are accessible through the {@link
        ''' java.lang.System#getProperty(java.lang.String)} method. The
        ''' string value of this property is then interpreted as an integer
        ''' value using the grammar supported by <seealso cref="Integer#decode decode"/> and
        ''' an {@code Integer} object representing this value is returned.
        ''' 
        ''' <p>If there is no property with the specified name, if the
        ''' specified name is empty or {@code null}, or if the property
        ''' does not have the correct numeric format, then {@code null} is
        ''' returned.
        ''' 
        ''' <p>In other words, this method returns an {@code Integer}
        ''' object equal to the value of:
        ''' 
        ''' <blockquote>
        '''  {@code getInteger(nm, null)}
        ''' </blockquote>
        ''' </summary>
        ''' <param name="nm">   property name. </param>
        ''' <returns>  the {@code Integer} value of the property. </returns>
        ''' <exception cref="SecurityException"> for the same reasons as
        '''          <seealso cref="System#getProperty(String) System.getProperty"/> </exception>
        ''' <seealso cref=     java.lang.System#getProperty(java.lang.String) </seealso>
        ''' <seealso cref=     java.lang.System#getProperty(java.lang.String, java.lang.String) </seealso>
        Public Shared Function getInteger(  nm As String) As Integer?
            Return getInteger(nm, Nothing)
        End Function

        ''' <summary>
        ''' Determines the integer value of the system property with the
        ''' specified name.
        ''' 
        ''' <p>The first argument is treated as the name of a system
        ''' property.  System properties are accessible through the {@link
        ''' java.lang.System#getProperty(java.lang.String)} method. The
        ''' string value of this property is then interpreted as an integer
        ''' value using the grammar supported by <seealso cref="Integer#decode decode"/> and
        ''' an {@code Integer} object representing this value is returned.
        ''' 
        ''' <p>The second argument is the default value. An {@code Integer} object
        ''' that represents the value of the second argument is returned if there
        ''' is no property of the specified name, if the property does not have
        ''' the correct numeric format, or if the specified name is empty or
        ''' {@code null}.
        ''' 
        ''' <p>In other words, this method returns an {@code Integer} object
        ''' equal to the value of:
        ''' 
        ''' <blockquote>
        '''  {@code getInteger(nm, new Integer(val))}
        ''' </blockquote>
        ''' 
        ''' but in practice it may be implemented in a manner such as:
        ''' 
        ''' <blockquote><pre>
        ''' Integer result = getInteger(nm, null);
        ''' return (result == null) ? new Integer(val) : result;
        ''' </pre></blockquote>
        ''' 
        ''' to avoid the unnecessary allocation of an {@code Integer}
        ''' object when the default value is not needed.
        ''' </summary>
        ''' <param name="nm">   property name. </param>
        ''' <param name="val">   default value. </param>
        ''' <returns>  the {@code Integer} value of the property. </returns>
        ''' <exception cref="SecurityException"> for the same reasons as
        '''          <seealso cref="System#getProperty(String) System.getProperty"/> </exception>
        ''' <seealso cref=     java.lang.System#getProperty(java.lang.String) </seealso>
        ''' <seealso cref=     java.lang.System#getProperty(java.lang.String, java.lang.String) </seealso>
        Public Shared Function getInteger(  nm As String,   val As Integer) As Integer?
            Dim result As Integer? = getInteger(nm, Nothing)
            Return If(result Is Nothing, Convert.ToInt32(val), result)
        End Function

        ''' <summary>
        ''' Returns the integer value of the system property with the
        ''' specified name.  The first argument is treated as the name of a
        ''' system property.  System properties are accessible through the
        ''' <seealso cref="java.lang.System#getProperty(java.lang.String)"/> method.
        ''' The string value of this property is then interpreted as an
        ''' integer value, as per the <seealso cref="Integer#decode decode"/> method,
        ''' and an {@code Integer} object representing this value is
        ''' returned; in summary:
        ''' 
        ''' <ul><li>If the property value begins with the two ASCII characters
        '''         {@code 0x} or the ASCII character {@code #}, not
        '''      followed by a minus sign, then the rest of it is parsed as a
        '''      hexadecimal integer exactly as by the method
        '''      <seealso cref="#valueOf(java.lang.String, int)"/> with radix 16.
        ''' <li>If the property value begins with the ASCII character
        '''     {@code 0} followed by another character, it is parsed as an
        '''     octal integer exactly as by the method
        '''     <seealso cref="#valueOf(java.lang.String, int)"/> with radix 8.
        ''' <li>Otherwise, the property value is parsed as a decimal integer
        ''' exactly as by the method <seealso cref="#valueOf(java.lang.String, int)"/>
        ''' with radix 10.
        ''' </ul>
        ''' 
        ''' <p>The second argument is the default value. The default value is
        ''' returned if there is no property of the specified name, if the
        ''' property does not have the correct numeric format, or if the
        ''' specified name is empty or {@code null}.
        ''' </summary>
        ''' <param name="nm">   property name. </param>
        ''' <param name="val">   default value. </param>
        ''' <returns>  the {@code Integer} value of the property. </returns>
        ''' <exception cref="SecurityException"> for the same reasons as
        '''          <seealso cref="System#getProperty(String) System.getProperty"/> </exception>
        ''' <seealso cref=     System#getProperty(java.lang.String) </seealso>
        ''' <seealso cref=     System#getProperty(java.lang.String, java.lang.String) </seealso>
        Public Shared Function getInteger(  nm As String,   val As Integer?) As Integer?
            Dim v As String = Nothing
            Try
                v = System.getProperty(nm)
            Catch e As Exception
            End Try
            If v IsNot Nothing Then
                Try
                    Return java.lang.[Integer].decode(v)
                Catch e As NumberFormatException
                End Try
            End If
            Return val
        End Function

        ''' <summary>
        ''' Decodes a {@code String} into an {@code Integer}.
        ''' Accepts decimal, hexadecimal, and octal numbers given
        ''' by the following grammar:
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
        '''  java.lang.[Integer].parseInt} method with the indicated radix (10, 16, or
        ''' 8).  This sequence of characters must represent a positive
        ''' value or a <seealso cref="NumberFormatException"/> will be thrown.  The
        ''' result is negated if first character of the specified {@code
        ''' String} is the minus sign.  No whitespace characters are
        ''' permitted in the {@code String}.
        ''' </summary>
        ''' <param name="nm"> the {@code String} to decode. </param>
        ''' <returns>    an {@code Integer} object holding the {@code int}
        '''             value represented by {@code nm} </returns>
        ''' <exception cref="NumberFormatException">  if the {@code String} does not
        '''            contain a parsable  java.lang.[Integer]. </exception>
        ''' <seealso cref= java.lang.Integer#parseInt(java.lang.String, int) </seealso>
        Public Shared Function decode(  nm As String) As Integer?
            Dim radix As Integer = 10
            Dim index As Integer = 0
            Dim negative As Boolean = False
            Dim result As Integer?

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
                result = Convert.ToInt32(nm.Substring(index), radix)
                result = If(negative, Convert.ToInt32(-result), result)
            Catch e As NumberFormatException
                ' If number is  java.lang.[Integer].MIN_VALUE, we'll end up here. The next line
                ' handles this case, and causes any genuine format error to be
                ' rethrown.
                Dim constant As String = If(negative, ("-" & nm.Substring(index)), nm.Substring(index))
                result = Convert.ToInt32(constant, radix)
            End Try
            Return result
        End Function

        ''' <summary>
        ''' Compares two {@code Integer} objects numerically.
        ''' </summary>
        ''' <param name="anotherInteger">   the {@code Integer} to be compared. </param>
        ''' <returns>  the value {@code 0} if this {@code Integer} is
        '''          equal to the argument {@code Integer}; a value less than
        '''          {@code 0} if this {@code Integer} is numerically less
        '''          than the argument {@code Integer}; and a value greater
        '''          than {@code 0} if this {@code Integer} is numerically
        '''           greater than the argument {@code Integer} (signed
        '''           comparison).
        ''' @since   1.2 </returns>
        Public Function compareTo(  anotherInteger As [Integer]) As Integer Implements Comparable(Of [Integer])
            Return compare(Me.value, another java.lang.[Integer].Value)
        End Function

        ''' <summary>
        ''' Compares two {@code int} values numerically.
        ''' The value returned is identical to what would be returned by:
        ''' <pre>
        '''     java.lang.[Integer].valueOf(x).compareTo( java.lang.[Integer].valueOf(y))
        ''' </pre>
        ''' </summary>
        ''' <param name="x"> the first {@code int} to compare </param>
        ''' <param name="y"> the second {@code int} to compare </param>
        ''' <returns> the value {@code 0} if {@code x == y};
        '''         a value less than {@code 0} if {@code x < y}; and
        '''         a value greater than {@code 0} if {@code x > y}
        ''' @since 1.7 </returns>
        Public Shared Function compare(  x As [Integer],   y As [Integer]) As Integer
            Return If(x < y, -1, (If(x = y, 0, 1)))
        End Function

        ''' <summary>
        ''' Compares two {@code int} values numerically treating the values
        ''' as unsigned.
        ''' </summary>
        ''' <param name="x"> the first {@code int} to compare </param>
        ''' <param name="y"> the second {@code int} to compare </param>
        ''' <returns> the value {@code 0} if {@code x == y}; a value less
        '''         than {@code 0} if {@code x < y} as unsigned values; and
        '''         a value greater than {@code 0} if {@code x > y} as
        '''         unsigned values
        ''' @since 1.8 </returns>
        Public Shared Function compareUnsigned(  x As [Integer],   y As [Integer]) As Integer
            Return compare(x + MIN_VALUE, y + MIN_VALUE)
        End Function

        ''' <summary>
        ''' Converts the argument to a {@code long} by an unsigned
        ''' conversion.  In an unsigned conversion to a {@code long}, the
        ''' high-order 32 bits of the {@code long} are zero and the
        ''' low-order 32 bits are equal to the bits of the integer
        ''' argument.
        ''' 
        ''' Consequently, zero and positive {@code int} values are mapped
        ''' to a numerically equal {@code long} value and negative {@code
        ''' int} values are mapped to a {@code long} value equal to the
        ''' input plus 2<sup>32</sup>.
        ''' </summary>
        ''' <param name="x"> the value to convert to an unsigned {@code long} </param>
        ''' <returns> the argument converted to {@code long} by an unsigned
        '''         conversion
        ''' @since 1.8 </returns>
        Public Shared Function toUnsignedLong(  x As Integer) As Long
            Return (CLng(x)) And &HFFFFFFFFL
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
        Public Shared Function divideUnsigned(  dividend As Integer,   divisor As Integer) As Integer
            ' In lieu of tricky code, for now just use long arithmetic.
            Return CInt(toUnsignedLong(dividend) \ toUnsignedLong(divisor))
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
        Public Shared Function remainderUnsigned(  dividend As Integer,   divisor As Integer) As Integer
            ' In lieu of tricky code, for now just use long arithmetic.
            Return CInt(Fix(toUnsignedLong(dividend) Mod toUnsignedLong(divisor)))
        End Function


        ' Bit twiddling

        ''' <summary>
        ''' The number of bits used to represent an {@code int} value in two's
        ''' complement binary form.
        ''' 
        ''' @since 1.5
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Const SIZE As Integer = 32

        ''' <summary>
        ''' The number of bytes used to represent a {@code int} value in two's
        ''' complement binary form.
        ''' 
        ''' @since 1.8
        ''' </summary>
        Public Shared ReadOnly BYTES As Integer = SIZE \ java.lang.[Byte].SIZE

        ''' <summary>
        ''' Returns an {@code int} value with at most a single one-bit, in the
        ''' position of the highest-order ("leftmost") one-bit in the specified
        ''' {@code int} value.  Returns zero if the specified value has no
        ''' one-bits in its two's complement binary representation, that is, if it
        ''' is equal to zero.
        ''' </summary>
        ''' <param name="i"> the value whose highest one bit is to be computed </param>
        ''' <returns> an {@code int} value with a single one-bit, in the position
        '''     of the highest-order one-bit in the specified value, or zero if
        '''     the specified value is itself equal to zero.
        ''' @since 1.5 </returns>
        Public Shared Function highestOneBit(  i As Integer) As Integer
            ' HD, Figure 3-1
            i = i Or (i >> 1)
            i = i Or (i >> 2)
            i = i Or (i >> 4)
            i = i Or (i >> 8)
            i = i Or (i >> 16)
            Return i - (CInt(CUInt(i) >> 1))
        End Function

        ''' <summary>
        ''' Returns an {@code int} value with at most a single one-bit, in the
        ''' position of the lowest-order ("rightmost") one-bit in the specified
        ''' {@code int} value.  Returns zero if the specified value has no
        ''' one-bits in its two's complement binary representation, that is, if it
        ''' is equal to zero.
        ''' </summary>
        ''' <param name="i"> the value whose lowest one bit is to be computed </param>
        ''' <returns> an {@code int} value with a single one-bit, in the position
        '''     of the lowest-order one-bit in the specified value, or zero if
        '''     the specified value is itself equal to zero.
        ''' @since 1.5 </returns>
        Public Shared Function lowestOneBit(  i As Integer) As Integer
            ' HD, Section 2-1
            Return i And -i
        End Function

        ''' <summary>
        ''' Returns the number of zero bits preceding the highest-order
        ''' ("leftmost") one-bit in the two's complement binary representation
        ''' of the specified {@code int} value.  Returns 32 if the
        ''' specified value has no one-bits in its two's complement representation,
        ''' in other words if it is equal to zero.
        ''' 
        ''' <p>Note that this method is closely related to the logarithm base 2.
        ''' For all positive {@code int} values x:
        ''' <ul>
        ''' <li>floor(log<sub>2</sub>(x)) = {@code 31 - numberOfLeadingZeros(x)}
        ''' <li>ceil(log<sub>2</sub>(x)) = {@code 32 - numberOfLeadingZeros(x - 1)}
        ''' </ul>
        ''' </summary>
        ''' <param name="i"> the value whose number of leading zeros is to be computed </param>
        ''' <returns> the number of zero bits preceding the highest-order
        '''     ("leftmost") one-bit in the two's complement binary representation
        '''     of the specified {@code int} value, or 32 if the value
        '''     is equal to zero.
        ''' @since 1.5 </returns>
        Public Shared Function numberOfLeadingZeros(  i As Integer) As Integer
            ' HD, Figure 5-6
            If i = 0 Then Return 32
            Dim n As Integer = 1
            If CInt(CUInt(i) >> 16 = 0) Then
                n += 16
                i <<= 16
            End If
            If CInt(CUInt(i) >> 24 = 0) Then
                n += 8
                i <<= 8
            End If
            If CInt(CUInt(i) >> 28 = 0) Then
                n += 4
                i <<= 4
            End If
            If CInt(CUInt(i) >> 30 = 0) Then
                n += 2
                i <<= 2
            End If
            n -= CInt(CUInt(i) >> 31)
            Return n
        End Function

        ''' <summary>
        ''' Returns the number of zero bits following the lowest-order ("rightmost")
        ''' one-bit in the two's complement binary representation of the specified
        ''' {@code int} value.  Returns 32 if the specified value has no
        ''' one-bits in its two's complement representation, in other words if it is
        ''' equal to zero.
        ''' </summary>
        ''' <param name="i"> the value whose number of trailing zeros is to be computed </param>
        ''' <returns> the number of zero bits following the lowest-order ("rightmost")
        '''     one-bit in the two's complement binary representation of the
        '''     specified {@code int} value, or 32 if the value is equal
        '''     to zero.
        ''' @since 1.5 </returns>
        Public Shared Function numberOfTrailingZeros(  i As Integer) As Integer
            ' HD, Figure 5-14
            Dim y As Integer
            If i = 0 Then Return 32
            Dim n As Integer = 31
            y = i << 16
            If y <> 0 Then
                n = n - 16
                i = y
            End If
            y = i << 8
            If y <> 0 Then
                n = n - 8
                i = y
            End If
            y = i << 4
            If y <> 0 Then
                n = n - 4
                i = y
            End If
            y = i << 2
            If y <> 0 Then
                n = n - 2
                i = y
            End If
            Return n - (CInt(CUInt((i << 1)) >> 31))
        End Function

        ''' <summary>
        ''' Returns the number of one-bits in the two's complement binary
        ''' representation of the specified {@code int} value.  This function is
        ''' sometimes referred to as the <i>population count</i>.
        ''' </summary>
        ''' <param name="i"> the value whose bits are to be counted </param>
        ''' <returns> the number of one-bits in the two's complement binary
        '''     representation of the specified {@code int} value.
        ''' @since 1.5 </returns>
        Public Shared Function bitCount(  i As Integer) As Integer
            ' HD, Figure 5-2
            i = i - ((CInt(CUInt(i) >> 1)) And &H55555555)
            i = (i And &H33333333) + ((CInt(CUInt(i) >> 2)) And &H33333333)
            i = (i + (CInt(CUInt(i) >> 4))) And &HF0F0F0F
            i = i + (CInt(CUInt(i) >> 8))
            i = i + (CInt(CUInt(i) >> 16))
            Return i And &H3F
        End Function

        ''' <summary>
        ''' Returns the value obtained by rotating the two's complement binary
        ''' representation of the specified {@code int} value left by the
        ''' specified number of bits.  (Bits shifted out of the left hand, or
        ''' high-order, side reenter on the right, or low-order.)
        ''' 
        ''' <p>Note that left rotation with a negative distance is equivalent to
        ''' right rotation: {@code rotateLeft(val, -distance) == rotateRight(val,
        ''' distance)}.  Note also that rotation by any multiple of 32 is a
        ''' no-op, so all but the last five bits of the rotation distance can be
        ''' ignored, even if the distance is negative: {@code rotateLeft(val,
        ''' distance) == rotateLeft(val, distance & 0x1F)}.
        ''' </summary>
        ''' <param name="i"> the value whose bits are to be rotated left </param>
        ''' <param name="distance"> the number of bit positions to rotate left </param>
        ''' <returns> the value obtained by rotating the two's complement binary
        '''     representation of the specified {@code int} value left by the
        '''     specified number of bits.
        ''' @since 1.5 </returns>
        Public Shared Function rotateLeft(  i As Integer,   distance As Integer) As Integer
            Return (i << distance) Or (CInt(CUInt(i) >> -distance))
        End Function

        ''' <summary>
        ''' Returns the value obtained by rotating the two's complement binary
        ''' representation of the specified {@code int} value right by the
        ''' specified number of bits.  (Bits shifted out of the right hand, or
        ''' low-order, side reenter on the left, or high-order.)
        ''' 
        ''' <p>Note that right rotation with a negative distance is equivalent to
        ''' left rotation: {@code rotateRight(val, -distance) == rotateLeft(val,
        ''' distance)}.  Note also that rotation by any multiple of 32 is a
        ''' no-op, so all but the last five bits of the rotation distance can be
        ''' ignored, even if the distance is negative: {@code rotateRight(val,
        ''' distance) == rotateRight(val, distance & 0x1F)}.
        ''' </summary>
        ''' <param name="i"> the value whose bits are to be rotated right </param>
        ''' <param name="distance"> the number of bit positions to rotate right </param>
        ''' <returns> the value obtained by rotating the two's complement binary
        '''     representation of the specified {@code int} value right by the
        '''     specified number of bits.
        ''' @since 1.5 </returns>
        Public Shared Function rotateRight(  i As Integer,   distance As Integer) As Integer
            Return (CInt(CUInt(i) >> distance)) Or (i << -distance)
        End Function

        ''' <summary>
        ''' Returns the value obtained by reversing the order of the bits in the
        ''' two's complement binary representation of the specified {@code int}
        ''' value.
        ''' </summary>
        ''' <param name="i"> the value to be reversed </param>
        ''' <returns> the value obtained by reversing order of the bits in the
        '''     specified {@code int} value.
        ''' @since 1.5 </returns>
        Public Shared Function reverse(  i As Integer) As Integer
            ' HD, Figure 7-1
            i = (i And &H55555555) << 1 Or (CInt(CUInt(i) >> 1)) And &H55555555
            i = (i And &H33333333) << 2 Or (CInt(CUInt(i) >> 2)) And &H33333333
            i = (i And &HF0F0F0F) << 4 Or (CInt(CUInt(i) >> 4)) And &HF0F0F0F
            i = (i << 24) Or ((i And &HFF00) << 8) Or ((CInt(CUInt(i) >> 8)) And &HFF00) Or (CInt(CUInt(i) >> 24))
            Return i
        End Function

        ''' <summary>
        ''' Returns the signum function of the specified {@code int} value.  (The
        ''' return value is -1 if the specified value is negative; 0 if the
        ''' specified value is zero; and 1 if the specified value is positive.)
        ''' </summary>
        ''' <param name="i"> the value whose signum is to be computed </param>
        ''' <returns> the signum function of the specified {@code int} value.
        ''' @since 1.5 </returns>
        Public Shared Function signum(  i As Integer) As Integer
            ' HD, Section 2-7
            Return (i >> 31) Or (-CInt(CUInt(i) >> 31))
        End Function

        ''' <summary>
        ''' Returns the value obtained by reversing the order of the bytes in the
        ''' two's complement representation of the specified {@code int} value.
        ''' </summary>
        ''' <param name="i"> the value whose bytes are to be reversed </param>
        ''' <returns> the value obtained by reversing the bytes in the specified
        '''     {@code int} value.
        ''' @since 1.5 </returns>
        Public Shared Function reverseBytes(  i As Integer) As Integer
            Return ((CInt(CUInt(i) >> 24))) Or ((i >> 8) And &HFF00) Or ((i << 8) And &HFF0000) Or ((i << 24))
        End Function

        ''' <summary>
        ''' Adds two integers together as per the + operator.
        ''' </summary>
        ''' <param name="a"> the first operand </param>
        ''' <param name="b"> the second operand </param>
        ''' <returns> the sum of {@code a} and {@code b} </returns>
        ''' <seealso cref= java.util.function.BinaryOperator
        ''' @since 1.8 </seealso>
        Public Shared Function sum(  a As Integer,   b As Integer) As Integer
            Return a + b
        End Function

        ''' <summary>
        ''' Returns the greater of two {@code int} values
        ''' as if by calling <seealso cref="Math#max(int, int) System.Math.max"/>.
        ''' </summary>
        ''' <param name="a"> the first operand </param>
        ''' <param name="b"> the second operand </param>
        ''' <returns> the greater of {@code a} and {@code b} </returns>
        ''' <seealso cref= java.util.function.BinaryOperator
        ''' @since 1.8 </seealso>
        Public Shared Function max(  a As Integer,   b As Integer) As Integer
            Return Global.System.Math.Max(a, b)
        End Function

        ''' <summary>
        ''' Returns the smaller of two {@code int} values
        ''' as if by calling <seealso cref="Math#min(int, int) System.Math.min"/>.
        ''' </summary>
        ''' <param name="a"> the first operand </param>
        ''' <param name="b"> the second operand </param>
        ''' <returns> the smaller of {@code a} and {@code b} </returns>
        ''' <seealso cref= java.util.function.BinaryOperator
        ''' @since 1.8 </seealso>
        Public Shared Function min(  a As Integer,   b As Integer) As Integer
            Return Global.System.Math.Min(a, b)
        End Function

        ''' <summary>
        ''' use serialVersionUID from JDK 1.0.2 for interoperability </summary>
        Private Const serialVersionUID As Long = 1360826667806852920L
    End Class
End Namespace
Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports java.math.BigInteger
Imports java.lang

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

'
' * Portions Copyright IBM Corporation, 2001. All Rights Reserved.
' 

Namespace java.math


    ''' <summary>
    ''' Immutable, arbitrary-precision signed decimal numbers.  A
    ''' {@code BigDecimal} consists of an arbitrary precision integer
    ''' <i>unscaled value</i> and a 32-bit integer <i>scale</i>.  If zero
    ''' or positive, the scale is the number of digits to the right of the
    ''' decimal point.  If negative, the unscaled value of the number is
    ''' multiplied by ten to the power of the negation of the scale.  The
    ''' value of the number represented by the {@code BigDecimal} is
    ''' therefore <tt>(unscaledValue &times; 10<sup>-scale</sup>)</tt>.
    ''' 
    ''' <p>The {@code BigDecimal} class provides operations for
    ''' arithmetic, scale manipulation, rounding, comparison, hashing, and
    ''' format conversion.  The <seealso cref="#toString"/> method provides a
    ''' canonical representation of a {@code BigDecimal}.
    ''' 
    ''' <p>The {@code BigDecimal} class gives its user complete control
    ''' over rounding behavior.  If no rounding mode is specified and the
    ''' exact result cannot be represented, an exception is thrown;
    ''' otherwise, calculations can be carried out to a chosen precision
    ''' and rounding mode by supplying an appropriate <seealso cref="MathContext"/>
    ''' object to the operation.  In either case, eight <em>rounding
    ''' modes</em> are provided for the control of rounding.  Using the
    ''' integer fields in this class (such as <seealso cref="#ROUND_HALF_UP"/>) to
    ''' represent rounding mode is largely obsolete; the enumeration values
    ''' of the {@code RoundingMode} {@code enum}, (such as {@link
    ''' RoundingMode#HALF_UP}) should be used instead.
    ''' 
    ''' <p>When a {@code MathContext} object is supplied with a precision
    ''' setting of 0 (for example, <seealso cref="MathContext#UNLIMITED"/>),
    ''' arithmetic operations are exact, as are the arithmetic methods
    ''' which take no {@code MathContext} object.  (This is the only
    ''' behavior that was supported in releases prior to 5.)  As a
    ''' corollary of computing the exact result, the rounding mode setting
    ''' of a {@code MathContext} object with a precision setting of 0 is
    ''' not used and thus irrelevant.  In the case of divide, the exact
    ''' quotient could have an infinitely long decimal expansion; for
    ''' example, 1 divided by 3.  If the quotient has a nonterminating
    ''' decimal expansion and the operation is specified to return an exact
    ''' result, an {@code ArithmeticException} is thrown.  Otherwise, the
    ''' exact result of the division is returned, as done for other
    ''' operations.
    ''' 
    ''' <p>When the precision setting is not 0, the rules of
    ''' {@code BigDecimal} arithmetic are broadly compatible with selected
    ''' modes of operation of the arithmetic defined in ANSI X3.274-1996
    ''' and ANSI X3.274-1996/AM 1-2000 (section 7.4).  Unlike those
    ''' standards, {@code BigDecimal} includes many rounding modes, which
    ''' were mandatory for division in {@code BigDecimal} releases prior
    ''' to 5.  Any conflicts between these ANSI standards and the
    ''' {@code BigDecimal} specification are resolved in favor of
    ''' {@code BigDecimal}.
    ''' 
    ''' <p>Since the same numerical value can have different
    ''' representations (with different scales), the rules of arithmetic
    ''' and rounding must specify both the numerical result and the scale
    ''' used in the result's representation.
    ''' 
    ''' 
    ''' <p>In general the rounding modes and precision setting determine
    ''' how operations return results with a limited number of digits when
    ''' the exact result has more digits (perhaps infinitely many in the
    ''' case of division) than the number of digits returned.
    ''' 
    ''' First, the
    ''' total number of digits to return is specified by the
    ''' {@code MathContext}'s {@code precision} setting; this determines
    ''' the result's <i>precision</i>.  The digit count starts from the
    ''' leftmost nonzero digit of the exact result.  The rounding mode
    ''' determines how any discarded trailing digits affect the returned
    ''' result.
    ''' 
    ''' <p>For all arithmetic operators , the operation is carried out as
    ''' though an exact intermediate result were first calculated and then
    ''' rounded to the number of digits specified by the precision setting
    ''' (if necessary), using the selected rounding mode.  If the exact
    ''' result is not returned, some digit positions of the exact result
    ''' are discarded.  When rounding increases the magnitude of the
    ''' returned result, it is possible for a new digit position to be
    ''' created by a carry propagating to a leading {@literal "9"} digit.
    ''' For example, rounding the value 999.9 to three digits rounding up
    ''' would be numerically equal to one thousand, represented as
    ''' 100&times;10<sup>1</sup>.  In such cases, the new {@literal "1"} is
    ''' the leading digit position of the returned result.
    ''' 
    ''' <p>Besides a logical exact result, each arithmetic operation has a
    ''' preferred scale for representing a result.  The preferred
    ''' scale for each operation is listed in the table below.
    ''' 
    ''' <table border>
    ''' <caption><b>Preferred Scales for Results of Arithmetic Operations
    ''' </b></caption>
    ''' <tr><th>Operation</th><th>Preferred Scale of Result</th></tr>
    ''' <tr><td>Add</td><td>max(addend.scale(), augend.scale())</td>
    ''' <tr><td>Subtract</td><td>max(minuend.scale(), subtrahend.scale())</td>
    ''' <tr><td>Multiply</td><td>multiplier.scale() + multiplicand.scale()</td>
    ''' <tr><td>Divide</td><td>dividend.scale() - divisor.scale()</td>
    ''' </table>
    ''' 
    ''' These scales are the ones used by the methods which return exact
    ''' arithmetic results; except that an exact divide may have to use a
    ''' larger scale since the exact result may have more digits.  For
    ''' example, {@code 1/32} is {@code 0.03125}.
    ''' 
    ''' <p>Before rounding, the scale of the logical exact intermediate
    ''' result is the preferred scale for that operation.  If the exact
    ''' numerical result cannot be represented in {@code precision}
    ''' digits, rounding selects the set of digits to return and the scale
    ''' of the result is reduced from the scale of the intermediate result
    ''' to the least scale which can represent the {@code precision}
    ''' digits actually returned.  If the exact result can be represented
    ''' with at most {@code precision} digits, the representation
    ''' of the result with the scale closest to the preferred scale is
    ''' returned.  In particular, an exactly representable quotient may be
    ''' represented in fewer than {@code precision} digits by removing
    ''' trailing zeros and decreasing the scale.  For example, rounding to
    ''' three digits using the <seealso cref="RoundingMode#FLOOR floor"/>
    ''' rounding mode, <br>
    ''' 
    ''' {@code 19/100 = 0.19   // integer=19,  scale=2} <br>
    ''' 
    ''' but<br>
    ''' 
    ''' {@code 21/110 = 0.190  // integer=190, scale=3} <br>
    ''' 
    ''' <p>Note that for add, subtract, and multiply, the reduction in
    ''' scale will equal the number of digit positions of the exact result
    ''' which are discarded. If the rounding causes a carry propagation to
    ''' create a new high-order digit position, an additional digit of the
    ''' result is discarded than when no new digit position is created.
    ''' 
    ''' <p>Other methods may have slightly different rounding semantics.
    ''' For example, the result of the {@code pow} method using the
    ''' <seealso cref="#pow(int, MathContext) specified algorithm"/> can
    ''' occasionally differ from the rounded mathematical result by more
    ''' than one unit in the last place, one <i><seealso cref="#ulp() ulp"/></i>.
    ''' 
    ''' <p>Two types of operations are provided for manipulating the scale
    ''' of a {@code BigDecimal}: scaling/rounding operations and decimal
    ''' point motion operations.  Scaling/rounding operations ({@link
    ''' #setScale setScale} and <seealso cref="#round round"/>) return a
    ''' {@code BigDecimal} whose value is approximately (or exactly) equal
    ''' to that of the operand, but whose scale or precision is the
    ''' specified value; that is, they increase or decrease the precision
    ''' of the stored number with minimal effect on its value.  Decimal
    ''' point motion operations (<seealso cref="#movePointLeft movePointLeft"/> and
    ''' <seealso cref="#movePointRight movePointRight"/>) return a
    ''' {@code BigDecimal} created from the operand by moving the decimal
    ''' point a specified distance in the specified direction.
    ''' 
    ''' <p>For the sake of brevity and clarity, pseudo-code is used
    ''' throughout the descriptions of {@code BigDecimal} methods.  The
    ''' pseudo-code expression {@code (i + j)} is shorthand for "a
    ''' {@code BigDecimal} whose value is that of the {@code BigDecimal}
    ''' {@code i} added to that of the {@code BigDecimal}
    ''' {@code j}." The pseudo-code expression {@code (i == j)} is
    ''' shorthand for "{@code true} if and only if the
    ''' {@code BigDecimal} {@code i} represents the same value as the
    ''' {@code BigDecimal} {@code j}." Other pseudo-code expressions
    ''' are interpreted similarly.  Square brackets are used to represent
    ''' the particular {@code BigInteger} and scale pair defining a
    ''' {@code BigDecimal} value; for example [19, 2] is the
    ''' {@code BigDecimal} numerically equal to 0.19 having a scale of 2.
    ''' 
    ''' <p>Note: care should be exercised if {@code BigDecimal} objects
    ''' are used as keys in a <seealso cref="java.util.SortedMap SortedMap"/> or
    ''' elements in a <seealso cref="java.util.SortedSet SortedSet"/> since
    ''' {@code BigDecimal}'s <i>natural ordering</i> is <i>inconsistent
    ''' with equals</i>.  See <seealso cref="Comparable"/>, {@link
    ''' java.util.SortedMap} or <seealso cref="java.util.SortedSet"/> for more
    ''' information.
    ''' 
    ''' <p>All methods and constructors for this class throw
    ''' {@code NullPointerException} when passed a {@code null} object
    ''' reference for any input parameter.
    ''' </summary>
    ''' <seealso cref=     BigInteger </seealso>
    ''' <seealso cref=     MathContext </seealso>
    ''' <seealso cref=     RoundingMode </seealso>
    ''' <seealso cref=     java.util.SortedMap </seealso>
    ''' <seealso cref=     java.util.SortedSet
    ''' @author  Josh Bloch
    ''' @author  Mike Cowlishaw
    ''' @author  Joseph D. Darcy
    ''' @author  Sergey V. Kuksenko </seealso>
    Public Class BigDecimal
        Inherits Number
        Implements Comparable(Of BigDecimal)

        ''' <summary>
        ''' The unscaled value of this BigDecimal, as returned by {@link
        ''' #unscaledValue}.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #unscaledValue </seealso>
        Private ReadOnly intVal As BigInteger

        ''' <summary>
        ''' The scale of this BigDecimal, as returned by <seealso cref="#scale"/>.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #scale </seealso>
        Private ReadOnly scale_Renamed As Integer ' Note: this may have any value, so
        ' calculations must be done in longs

        ''' <summary>
        ''' The number of decimal digits in this BigDecimal, or 0 if the
        ''' number of digits are not known (lookaside information).  If
        ''' nonzero, the value is guaranteed correct.  Use the precision()
        ''' method to obtain and set the value if it might be 0.  This
        ''' field is mutable until set nonzero.
        ''' 
        ''' @since  1.5
        ''' </summary>
        <NonSerialized>
        Private precision_Renamed As Integer

        ''' <summary>
        ''' Used to store the canonical string representation, if computed.
        ''' </summary>
        <NonSerialized>
        Private stringCache As String

        ''' <summary>
        ''' Sentinel value for <seealso cref="#intCompact"/> indicating the
        ''' significand information is only available from {@code intVal}.
        ''' </summary>
        Friend Shared ReadOnly INFLATED_Renamed As Long = java.lang.[Long].MIN_VALUE

        Private Shared ReadOnly INFLATED_BIGINT As BigInteger = Big java.lang.[Integer].valueOf(INFLATED_Renamed)

        ''' <summary>
        ''' If the absolute value of the significand of this BigDecimal is
        ''' less than or equal to {@code java.lang.[Long].MAX_VALUE}, the value can be
        ''' compactly stored in this field and used in computations.
        ''' </summary>
        <NonSerialized>
        Private ReadOnly intCompact As Long

        ' All 18-digit base ten strings fit into a long; not all 19-digit
        ' strings will
        Private Const MAX_COMPACT_DIGITS As Integer = 18

        ' Appease the serialization gods 
        Private Const serialVersionUID As Long = 6108874887143696463L

        Private Shared ReadOnly threadLocalStringBuilderHelper As ThreadLocal(Of StringBuilderHelper) = New ThreadLocalAnonymousInnerClassHelper(Of T)

        Private Class ThreadLocalAnonymousInnerClassHelper(Of T)
            Inherits ThreadLocal(Of T)

            Protected Friend Overrides Function initialValue() As StringBuilderHelper
                Return New StringBuilderHelper
            End Function
        End Class

        ' Cache of common small BigDecimal values.
        Private Shared ReadOnly zeroThroughTen As BigDecimal() = {New BigDecimal(Big java.lang.[Integer].ZERO, 0, 0, 1), New BigDecimal(Big java.lang.[Integer].ONE, 1, 0, 1), New BigDecimal(Big java.lang.[Integer].valueOf(2), 2, 0, 1), New BigDecimal(Big java.lang.[Integer].valueOf(3), 3, 0, 1), New BigDecimal(Big java.lang.[Integer].valueOf(4), 4, 0, 1), New BigDecimal(Big java.lang.[Integer].valueOf(5), 5, 0, 1), New BigDecimal(Big java.lang.[Integer].valueOf(6), 6, 0, 1), New BigDecimal(Big java.lang.[Integer].valueOf(7), 7, 0, 1), New BigDecimal(Big java.lang.[Integer].valueOf(8), 8, 0, 1), New BigDecimal(Big java.lang.[Integer].valueOf(9), 9, 0, 1), New BigDecimal(Big java.lang.[Integer].TEN, 10, 0, 2)}

        ' Cache of zero scaled by 0 - 15
        Private Shared ReadOnly ZERO_SCALED_BY As BigDecimal() = {zeroThroughTen(0), New BigDecimal(Big java.lang.[Integer].ZERO, 0, 1, 1), New BigDecimal(Big java.lang.[Integer].ZERO, 0, 2, 1), New BigDecimal(Big java.lang.[Integer].ZERO, 0, 3, 1), New BigDecimal(Big java.lang.[Integer].ZERO, 0, 4, 1), New BigDecimal(Big java.lang.[Integer].ZERO, 0, 5, 1), New BigDecimal(Big java.lang.[Integer].ZERO, 0, 6, 1), New BigDecimal(Big java.lang.[Integer].ZERO, 0, 7, 1), New BigDecimal(Big java.lang.[Integer].ZERO, 0, 8, 1), New BigDecimal(Big java.lang.[Integer].ZERO, 0, 9, 1), New BigDecimal(Big java.lang.[Integer].ZERO, 0, 10, 1), New BigDecimal(Big java.lang.[Integer].ZERO, 0, 11, 1), New BigDecimal(Big java.lang.[Integer].ZERO, 0, 12, 1), New BigDecimal(Big java.lang.[Integer].ZERO, 0, 13, 1), New BigDecimal(Big java.lang.[Integer].ZERO, 0, 14, 1), New BigDecimal(Big java.lang.[Integer].ZERO, 0, 15, 1)}

        ' Half of java.lang.[Long].MIN_VALUE & java.lang.[Long].MAX_VALUE.
        Private Shared ReadOnly HALF_LONG_MAX_VALUE As Long = java.lang.[Long].MAX_VALUE / 2
        Private Shared ReadOnly HALF_LONG_MIN_VALUE As Long = java.lang.[Long].MIN_VALUE / 2

        ' Constants
        ''' <summary>
        ''' The value 0, with a scale of 0.
        ''' 
        ''' @since  1.5
        ''' </summary>
        Public Shared ReadOnly ZERO As BigDecimal = zeroThroughTen(0)

        ''' <summary>
        ''' The value 1, with a scale of 0.
        ''' 
        ''' @since  1.5
        ''' </summary>
        Public Shared ReadOnly ONE As BigDecimal = zeroThroughTen(1)

        ''' <summary>
        ''' The value 10, with a scale of 0.
        ''' 
        ''' @since  1.5
        ''' </summary>
        Public Shared ReadOnly TEN As BigDecimal = zeroThroughTen(10)

        ' Constructors

        ''' <summary>
        ''' Trusted package private constructor.
        ''' Trusted simply means if val is INFLATED, intVal could not be null and
        ''' if intVal is null, val could not be INFLATED.
        ''' </summary>
        Friend Sub New(ByVal intVal As BigInteger, ByVal val As Long, ByVal scale As Integer, ByVal prec As Integer)
            Me.scale_Renamed = scale
            Me.precision_Renamed = prec
            Me.intCompact = val
            Me.intVal = intVal
        End Sub

        ''' <summary>
        ''' Translates a character array representation of a
        ''' {@code BigDecimal} into a {@code BigDecimal}, accepting the
        ''' same sequence of characters as the <seealso cref="#BigDecimal(String)"/>
        ''' constructor, while allowing a sub-array to be specified.
        ''' 
        ''' <p>Note that if the sequence of characters is already available
        ''' within a character array, using this constructor is faster than
        ''' converting the {@code char} array to string and using the
        ''' {@code BigDecimal(String)} constructor .
        ''' </summary>
        ''' <param name="in"> {@code char} array that is the source of characters. </param>
        ''' <param name="offset"> first character in the array to inspect. </param>
        ''' <param name="len"> number of characters to consider. </param>
        ''' <exception cref="NumberFormatException"> if {@code in} is not a valid
        '''         representation of a {@code BigDecimal} or the defined subarray
        '''         is not wholly within {@code in}.
        ''' @since  1.5 </exception>
        Public Sub New(ByVal [in] As Char(), ByVal offset As Integer, ByVal len As Integer)
            Me.New([in], offset, len, MathContext.UNLIMITED)
        End Sub

        ''' <summary>
        ''' Translates a character array representation of a
        ''' {@code BigDecimal} into a {@code BigDecimal}, accepting the
        ''' same sequence of characters as the <seealso cref="#BigDecimal(String)"/>
        ''' constructor, while allowing a sub-array to be specified and
        ''' with rounding according to the context settings.
        ''' 
        ''' <p>Note that if the sequence of characters is already available
        ''' within a character array, using this constructor is faster than
        ''' converting the {@code char} array to string and using the
        ''' {@code BigDecimal(String)} constructor .
        ''' </summary>
        ''' <param name="in"> {@code char} array that is the source of characters. </param>
        ''' <param name="offset"> first character in the array to inspect. </param>
        ''' <param name="len"> number of characters to consider.. </param>
        ''' <param name="mc"> the context to use. </param>
        ''' <exception cref="ArithmeticException"> if the result is inexact but the
        '''         rounding mode is {@code UNNECESSARY}. </exception>
        ''' <exception cref="NumberFormatException"> if {@code in} is not a valid
        '''         representation of a {@code BigDecimal} or the defined subarray
        '''         is not wholly within {@code in}.
        ''' @since  1.5 </exception>
        Public Sub New(ByVal [in] As Char(), ByVal offset As Integer, ByVal len As Integer, ByVal mc As MathContext)
            ' protect against huge length.
            If offset + len > [in].Length OrElse offset < 0 Then Throw New NumberFormatException("Bad offset or len arguments for char[] input.")
            ' This is the primary string to BigDecimal constructor; all
            ' incoming strings end up here; it uses explicit (inline)
            ' parsing for speed and generates at most one intermediate
            ' (temporary) object (a char[] array) for non-compact case.

            ' Use locals for all fields values until completion
            Dim prec As Integer = 0 ' record precision value
            Dim scl As Integer = 0 ' record scale value
            Dim rs As Long = 0 ' the compact value in long
            Dim rb As BigInteger = Nothing ' the inflated value in BigInteger
            ' use array bounds checking to handle too-long, len == 0,
            ' bad offset, etc.
            Try
                ' handle the sign
                Dim isneg As Boolean = False ' assume positive
                If [in](offset) = "-"c Then
                    isneg = True ' leading minus means negative
                    offset += 1
                    len -= 1 ' leading + allowed
                ElseIf [in](offset) = "+"c Then
                    offset += 1
                    len -= 1
                End If

                ' should now be at numeric part of the significand
                Dim dot As Boolean = False ' true when there is a '.'
                Dim exp As Long = 0 ' exponent
                Dim c As Char ' current character
                Dim isCompact As Boolean = (len <= MAX_COMPACT_DIGITS)
                ' integer significand array & idx is the index to it. The array
                ' is ONLY used when we can't use a compact representation.
                Dim idx As Integer = 0
                If isCompact Then
                    ' First compact case, we need not to preserve the character
                    ' and we can just compute the value in place.
                    Do While len > 0
                        c = [in](offset)
                        If (c = "0"c) Then ' have zero
                            If prec = 0 Then
                                prec = 1
                            ElseIf rs <> 0 Then
                                rs *= 10
                                prec += 1
                            End If ' else digit is a redundant leading zero
                            If dot Then
                                scl += 1
                            End If ' have digit
                        ElseIf (c >= "1"c AndAlso c <= "9"c) Then
                            Dim digit As Integer = AscW(c) - AscW("0"c)
                            If prec <> 1 OrElse rs <> 0 Then prec += 1 ' prec unchanged if preceded by 0s
                            rs = rs * 10 + digit
                            If dot Then
                                scl += 1
                            End If ' have dot
                        ElseIf c = "."c Then
                            ' have dot
                            If dot Then ' two dots Throw New NumberFormatException
                                dot = True ' slow path
                            ElseIf Char.IsDigit(c) Then
                                Dim digit As Integer = Character.digit(c, 10)
                                If digit = 0 Then
                                    If prec = 0 Then
                                        prec = 1
                                    ElseIf rs <> 0 Then
                                        rs *= 10
                                        prec += 1
                                    End If ' else digit is a redundant leading zero
                                Else
                                    If prec <> 1 OrElse rs <> 0 Then prec += 1 ' prec unchanged if preceded by 0s
                                    rs = rs * 10 + digit
                                End If
                                If dot Then scl += 1
                            ElseIf (c = "e"c) OrElse (c = "E"c) Then
                                exp = parseExp([in], offset, len)
                                ' Next test is required for backwards compatibility
                                If CInt(exp) <> exp Then ' overflow Throw New NumberFormatException
                                    Exit Do ' [saves a test]
                                Else
                                    Throw New NumberFormatException
                                End If
                                offset += 1
                                len -= 1
                    Loop
                    If prec = 0 Then ' no digits found Throw New NumberFormatException
                        ' Adjust scale if exp is not zero.
                        If exp <> 0 Then ' had significant exponent scl = adjustScale(scl, exp)
                            rs = If(isneg, -rs, rs)
                            Dim mcp As Integer = mc.precision
                            Dim drop As Integer = prec - mcp ' prec has range [1, MAX_INT], mcp has range [0, MAX_INT];
                            ' therefore, this subtract cannot overflow
                            If mcp > 0 AndAlso drop > 0 Then ' do rounding
                                Do While drop > 0
                                    scl = checkScaleNonZero(CLng(scl) - drop)
                                    rs = divideAndRound(rs, LONG_TEN_POWERS_TABLE(drop), mc.roundingMode.oldMode)
                                    prec = longDigitLength(rs)
                                    drop = prec - mcp
                                Loop
                            End If
                        Else
                            Dim coeff As Char() = New Char(len - 1) {}
                            Do While len > 0
                                c = [in](offset)
                                ' have digit
                                If (c >= "0"c AndAlso c <= "9"c) OrElse Char.IsDigit(c) Then
                                    ' First compact case, we need not to preserve the character
                                    ' and we can just compute the value in place.
                                    If c = "0"c OrElse Character.digit(c, 10) = 0 Then
                                        If prec = 0 Then
                                            coeff(idx) = c
                                            prec = 1
                                        ElseIf idx <> 0 Then
                                            coeff(idx) = c
                                            idx += 1
                                            prec += 1
                                        End If ' else c must be a redundant leading zero
                                    Else
                                        If prec <> 1 OrElse idx <> 0 Then prec += 1 ' prec unchanged if preceded by 0s
                                        coeff(idx) = c
                                        idx += 1
                                    End If
                                    If dot Then scl += 1
                                    offset += 1
                                    len -= 1
                                    Continue Do
                                End If
                                ' have dot
                                If c = "."c Then
                                    ' have dot
                                    If dot Then ' two dots Throw New NumberFormatException
                                        dot = True
                                        offset += 1
                                        len -= 1
                                        Continue Do
                                    End If
                                    ' exponent expected
                                    If (c <> "e"c) AndAlso (c <> "E"c) Then Throw New NumberFormatException
                                    exp = parseExp([in], offset, len)
                                    ' Next test is required for backwards compatibility
                                    If CInt(exp) <> exp Then ' overflow Throw New NumberFormatException
                                        Exit Do ' [saves a test]
                                        offset += 1
                                        len -= 1
                    Loop
                            ' here when no characters left
                            If prec = 0 Then ' no digits found Throw New NumberFormatException
                                ' Adjust scale if exp is not zero.
                                If exp <> 0 Then ' had significant exponent scl = adjustScale(scl, exp)
                                    ' Remove leading zeros from precision (digits count)
                                    rb = New BigInteger(coeff, If(isneg, -1, 1), prec)
                                    rs = compactValFor(rb)
                                    Dim mcp As Integer = mc.precision
                                    If mcp > 0 AndAlso (prec > mcp) Then
                                        If rs = INFLATED_Renamed Then
                                            Dim drop As Integer = prec - mcp
                                            Do While drop > 0
                                                scl = checkScaleNonZero(CLng(scl) - drop)
                                                rb = divideAndRoundByTenPow(rb, drop, mc.roundingMode.oldMode)
                                                rs = compactValFor(rb)
                                                If rs <> INFLATED_Renamed Then
                                                    prec = longDigitLength(rs)
                                                    Exit Do
                                                End If
                                                prec = bigDigitLength(rb)
                                                drop = prec - mcp
                                            Loop
                                        End If
                                        If rs <> INFLATED_Renamed Then
                                            Dim drop As Integer = prec - mcp
                                            Do While drop > 0
                                                scl = checkScaleNonZero(CLng(scl) - drop)
                                                rs = divideAndRound(rs, LONG_TEN_POWERS_TABLE(drop), mc.roundingMode.oldMode)
                                                prec = longDigitLength(rs)
                                                drop = prec - mcp
                                            Loop
                                            rb = Nothing
                                        End If
                                    End If
                                End If
            Catch e As ArrayIndexOutOfBoundsException
                Throw New NumberFormatException
            Catch e As NegativeArraySizeException
                Throw New NumberFormatException
            End Try
            Me.scale_Renamed = scl
            Me.precision_Renamed = prec
            Me.intCompact = rs
            Me.intVal = rb
        End Sub

        Private Function adjustScale(ByVal scl As Integer, ByVal exp As Long) As Integer
            Dim adjustedScale As Long = scl - exp
            If adjustedScale >  java.lang.[Integer].Max_Value OrElse adjustedScale <  java.lang.[Integer].MIN_VALUE Then Throw New NumberFormatException("Scale out of range.")
            scl = CInt(adjustedScale)
            Return scl
        End Function

        '    
        '     * parse exponent
        '     
        Private Shared Function parseExp(ByVal [in] As Char(), ByVal offset As Integer, ByVal len As Integer) As Long
            Dim exp As Long = 0
            offset += 1
            Dim c As Char = [in](offset)
            len -= 1
            Dim negexp As Boolean = (c = "-"c)
            ' optional sign
            If negexp OrElse c = "+"c Then
                offset += 1
                c = [in](offset)
                len -= 1
            End If
            If len <= 0 Then ' no exponent digits Throw New NumberFormatException
                ' skip leading zeros in the exponent
                Do While len > 10 AndAlso (c = "0"c OrElse (Character.digit(c, 10) = 0))
                    offset += 1
                    c = [in](offset)
                    len -= 1
                Loop
                If len > 10 Then ' too many nonzero exponent digits Throw New NumberFormatException
                    ' c now holds first digit of exponent
                    Do
                        Dim v As Integer
                        If c >= "0"c AndAlso c <= "9"c Then
                            v = AscW(c) - AscW("0"c)
                        Else
                            v = Character.digit(c, 10)
                            If v < 0 Then ' not a digit Throw New NumberFormatException
                            End If
                            exp = exp * 10 + v
                            If len = 1 Then Exit Do ' that was final character
                            offset += 1
                            c = [in](offset)
                            len -= 1
            Loop
                    If negexp Then ' apply sign exp = -exp
                        Return exp
        End Function

        ''' <summary>
        ''' Translates a character array representation of a
        ''' {@code BigDecimal} into a {@code BigDecimal}, accepting the
        ''' same sequence of characters as the <seealso cref="#BigDecimal(String)"/>
        ''' constructor.
        ''' 
        ''' <p>Note that if the sequence of characters is already available
        ''' as a character array, using this constructor is faster than
        ''' converting the {@code char} array to string and using the
        ''' {@code BigDecimal(String)} constructor .
        ''' </summary>
        ''' <param name="in"> {@code char} array that is the source of characters. </param>
        ''' <exception cref="NumberFormatException"> if {@code in} is not a valid
        '''         representation of a {@code BigDecimal}.
        ''' @since  1.5 </exception>
        Public Sub New(ByVal [in] As Char())
            Me.New([in], 0, [in].Length)
        End Sub

        ''' <summary>
        ''' Translates a character array representation of a
        ''' {@code BigDecimal} into a {@code BigDecimal}, accepting the
        ''' same sequence of characters as the <seealso cref="#BigDecimal(String)"/>
        ''' constructor and with rounding according to the context
        ''' settings.
        ''' 
        ''' <p>Note that if the sequence of characters is already available
        ''' as a character array, using this constructor is faster than
        ''' converting the {@code char} array to string and using the
        ''' {@code BigDecimal(String)} constructor .
        ''' </summary>
        ''' <param name="in"> {@code char} array that is the source of characters. </param>
        ''' <param name="mc"> the context to use. </param>
        ''' <exception cref="ArithmeticException"> if the result is inexact but the
        '''         rounding mode is {@code UNNECESSARY}. </exception>
        ''' <exception cref="NumberFormatException"> if {@code in} is not a valid
        '''         representation of a {@code BigDecimal}.
        ''' @since  1.5 </exception>
        Public Sub New(ByVal [in] As Char(), ByVal mc As MathContext)
            Me.New([in], 0, [in].Length, mc)
        End Sub

        ''' <summary>
        ''' Translates the string representation of a {@code BigDecimal}
        ''' into a {@code BigDecimal}.  The string representation consists
        ''' of an optional sign, {@code '+'} (<tt> '&#92;u002B'</tt>) or
        ''' {@code '-'} (<tt>'&#92;u002D'</tt>), followed by a sequence of
        ''' zero or more decimal digits ("the integer"), optionally
        ''' followed by a fraction, optionally followed by an exponent.
        ''' 
        ''' <p>The fraction consists of a decimal point followed by zero
        ''' or more decimal digits.  The string must contain at least one
        ''' digit in either the integer or the fraction.  The number formed
        ''' by the sign, the integer and the fraction is referred to as the
        ''' <i>significand</i>.
        ''' 
        ''' <p>The exponent consists of the character {@code 'e'}
        ''' (<tt>'&#92;u0065'</tt>) or {@code 'E'} (<tt>'&#92;u0045'</tt>)
        ''' followed by one or more decimal digits.  The value of the
        ''' exponent must lie between -<seealso cref="Integer#MAX_VALUE"/> ({@link
        ''' Integer#MIN_VALUE}+1) and <seealso cref="Integer#MAX_VALUE"/>, inclusive.
        ''' 
        ''' <p>More formally, the strings this constructor accepts are
        ''' described by the following grammar:
        ''' <blockquote>
        ''' <dl>
        ''' <dt><i>BigDecimalString:</i>
        ''' <dd><i>Sign<sub>opt</sub> Significand Exponent<sub>opt</sub></i>
        ''' <dt><i>Sign:</i>
        ''' <dd>{@code +}
        ''' <dd>{@code -}
        ''' <dt><i>Significand:</i>
        ''' <dd><i>IntegerPart</i> {@code .} <i>FractionPart<sub>opt</sub></i>
        ''' <dd>{@code .} <i>FractionPart</i>
        ''' <dd><i>IntegerPart</i>
        ''' <dt><i>IntegerPart:</i>
        ''' <dd><i>Digits</i>
        ''' <dt><i>FractionPart:</i>
        ''' <dd><i>Digits</i>
        ''' <dt><i>Exponent:</i>
        ''' <dd><i>ExponentIndicator SignedInteger</i>
        ''' <dt><i>ExponentIndicator:</i>
        ''' <dd>{@code e}
        ''' <dd>{@code E}
        ''' <dt><i>SignedInteger:</i>
        ''' <dd><i>Sign<sub>opt</sub> Digits</i>
        ''' <dt><i>Digits:</i>
        ''' <dd><i>Digit</i>
        ''' <dd><i>Digits Digit</i>
        ''' <dt><i>Digit:</i>
        ''' <dd>any character for which <seealso cref="Character#isDigit"/>
        ''' returns {@code true}, including 0, 1, 2 ...
        ''' </dl>
        ''' </blockquote>
        ''' 
        ''' <p>The scale of the returned {@code BigDecimal} will be the
        ''' number of digits in the fraction, or zero if the string
        ''' contains no decimal point, subject to adjustment for any
        ''' exponent; if the string contains an exponent, the exponent is
        ''' subtracted from the scale.  The value of the resulting scale
        ''' must lie between {@code  java.lang.[Integer].MIN_VALUE} and
        ''' {@code  java.lang.[Integer].MAX_VALUE}, inclusive.
        ''' 
        ''' <p>The character-to-digit mapping is provided by {@link
        ''' java.lang.Character#digit} set to convert to radix 10.  The
        ''' String may not contain any extraneous characters (whitespace,
        ''' for example).
        ''' 
        ''' <p><b>Examples:</b><br>
        ''' The value of the returned {@code BigDecimal} is equal to
        ''' <i>significand</i> &times; 10<sup>&nbsp;<i>exponent</i></sup>.
        ''' For each string on the left, the resulting representation
        ''' [{@code BigInteger}, {@code scale}] is shown on the right.
        ''' <pre>
        ''' "0"            [0,0]
        ''' "0.00"         [0,2]
        ''' "123"          [123,0]
        ''' "-123"         [-123,0]
        ''' "1.23E3"       [123,-1]
        ''' "1.23E+3"      [123,-1]
        ''' "12.3E+7"      [123,-6]
        ''' "12.0"         [120,1]
        ''' "12.3"         [123,1]
        ''' "0.00123"      [123,5]
        ''' "-1.23E-12"    [-123,14]
        ''' "1234.5E-4"    [12345,5]
        ''' "0E+7"         [0,-7]
        ''' "-0"           [0,0]
        ''' </pre>
        ''' 
        ''' <p>Note: For values other than {@code float} and
        ''' {@code double} NaN and &plusmn;Infinity, this constructor is
        ''' compatible with the values returned by <seealso cref="Float#toString"/>
        ''' and <seealso cref="Double#toString"/>.  This is generally the preferred
        ''' way to convert a {@code float} or {@code double} into a
        ''' BigDecimal, as it doesn't suffer from the unpredictability of
        ''' the <seealso cref="#BigDecimal(double)"/> constructor.
        ''' </summary>
        ''' <param name="val"> String representation of {@code BigDecimal}.
        ''' </param>
        ''' <exception cref="NumberFormatException"> if {@code val} is not a valid
        '''         representation of a {@code BigDecimal}. </exception>
        Public Sub New(ByVal val As String)
            Me.New(val.ToCharArray(), 0, val.Length())
        End Sub

        ''' <summary>
        ''' Translates the string representation of a {@code BigDecimal}
        ''' into a {@code BigDecimal}, accepting the same strings as the
        ''' <seealso cref="#BigDecimal(String)"/> constructor, with rounding
        ''' according to the context settings.
        ''' </summary>
        ''' <param name="val"> string representation of a {@code BigDecimal}. </param>
        ''' <param name="mc"> the context to use. </param>
        ''' <exception cref="ArithmeticException"> if the result is inexact but the
        '''         rounding mode is {@code UNNECESSARY}. </exception>
        ''' <exception cref="NumberFormatException"> if {@code val} is not a valid
        '''         representation of a BigDecimal.
        ''' @since  1.5 </exception>
        Public Sub New(ByVal val As String, ByVal mc As MathContext)
            Me.New(val.ToCharArray(), 0, val.Length(), mc)
        End Sub

        ''' <summary>
        ''' Translates a {@code double} into a {@code BigDecimal} which
        ''' is the exact decimal representation of the {@code double}'s
        ''' binary floating-point value.  The scale of the returned
        ''' {@code BigDecimal} is the smallest value such that
        ''' <tt>(10<sup>scale</sup> &times; val)</tt> is an  java.lang.[Integer].
        ''' <p>
        ''' <b>Notes:</b>
        ''' <ol>
        ''' <li>
        ''' The results of this constructor can be somewhat unpredictable.
        ''' One might assume that writing {@code new BigDecimal(0.1)} in
        ''' Java creates a {@code BigDecimal} which is exactly equal to
        ''' 0.1 (an unscaled value of 1, with a scale of 1), but it is
        ''' actually equal to
        ''' 0.1000000000000000055511151231257827021181583404541015625.
        ''' This is because 0.1 cannot be represented exactly as a
        ''' {@code double} (or, for that matter, as a binary fraction of
        ''' any finite length).  Thus, the value that is being passed
        ''' <i>in</i> to the constructor is not exactly equal to 0.1,
        ''' appearances notwithstanding.
        ''' 
        ''' <li>
        ''' The {@code String} constructor, on the other hand, is
        ''' perfectly predictable: writing {@code new BigDecimal("0.1")}
        ''' creates a {@code BigDecimal} which is <i>exactly</i> equal to
        ''' 0.1, as one would expect.  Therefore, it is generally
        ''' recommended that the {@link #BigDecimal(String)
        ''' <tt>String</tt> constructor} be used in preference to this one.
        ''' 
        ''' <li>
        ''' When a {@code double} must be used as a source for a
        ''' {@code BigDecimal}, note that this constructor provides an
        ''' exact conversion; it does not give the same result as
        ''' converting the {@code double} to a {@code String} using the
        ''' <seealso cref="Double#toString(double)"/> method and then using the
        ''' <seealso cref="#BigDecimal(String)"/> constructor.  To get that result,
        ''' use the {@code static} <seealso cref="#valueOf(double)"/> method.
        ''' </ol>
        ''' </summary>
        ''' <param name="val"> {@code double} value to be converted to
        '''        {@code BigDecimal}. </param>
        ''' <exception cref="NumberFormatException"> if {@code val} is infinite or NaN. </exception>
        Public Sub New(ByVal val As Double)
            Me.New(val, MathContext.UNLIMITED)
        End Sub

        ''' <summary>
        ''' Translates a {@code double} into a {@code BigDecimal}, with
        ''' rounding according to the context settings.  The scale of the
        ''' {@code BigDecimal} is the smallest value such that
        ''' <tt>(10<sup>scale</sup> &times; val)</tt> is an  java.lang.[Integer].
        ''' 
        ''' <p>The results of this constructor can be somewhat unpredictable
        ''' and its use is generally not recommended; see the notes under
        ''' the <seealso cref="#BigDecimal(double)"/> constructor.
        ''' </summary>
        ''' <param name="val"> {@code double} value to be converted to
        '''         {@code BigDecimal}. </param>
        ''' <param name="mc"> the context to use. </param>
        ''' <exception cref="ArithmeticException"> if the result is inexact but the
        '''         RoundingMode is UNNECESSARY. </exception>
        ''' <exception cref="NumberFormatException"> if {@code val} is infinite or NaN.
        ''' @since  1.5 </exception>
        Public Sub New(ByVal val As Double, ByVal mc As MathContext)
            If java.lang.[Double].IsInfinity(val) OrElse java.lang.[Double].IsNaN(val) Then Throw New NumberFormatException("Infinite or NaN")
            ' Translate the double into sign, exponent and significand, according
            ' to the formulae in JLS, Section 20.10.22.
            Dim valBits As Long = java.lang.[Double].doubleToLongBits(val)
            Dim sign As Integer = (If((valBits >> 63) = 0, 1, -1))
            Dim exponent As Integer = CInt(Fix((valBits >> 52) And &H7FFL))
            Dim significand As Long = (If(exponent = 0, (valBits And ((1L << 52) - 1)) << 1, (valBits And ((1L << 52) - 1)) Or (1L << 52)))
            exponent -= 1075
            ' At this point, val == sign * significand * 2**exponent.

            '        
            '         * Special case zero to supress nonterminating normalization and bogus
            '         * scale calculation.
            '         
            If significand = 0 Then
                Me.intVal = Big java.lang.[Integer].ZERO
                Me.scale_Renamed = 0
                Me.intCompact = 0
                Me.precision_Renamed = 1
                Return
            End If
            ' Normalize
            Do While (significand And 1) = 0 ' i.e., significand is even
                significand >>= 1
                exponent += 1
            Loop
            Dim scale_Renamed As Integer = 0
            ' Calculate intVal and scale
            Dim intVal As BigInteger
            Dim compactVal As Long = sign * significand
            If exponent = 0 Then
                intVal = If(compactVal = INFLATED_Renamed, INFLATED_BIGINT, Nothing)
            Else
                If exponent < 0 Then
                    intVal = Big java.lang.[Integer].valueOf(5).pow(-exponent).multiply(compactVal)
                    scale_Renamed = -exponent '  (exponent > 0)
                Else
                    intVal = Big java.lang.[Integer].valueOf(2).pow(exponent).multiply(compactVal)
                End If
                compactVal = compactValFor(intVal)
            End If
            Dim prec As Integer = 0
            Dim mcp As Integer = mc.precision
            If mcp > 0 Then ' do rounding
                Dim mode As Integer = mc.roundingMode.oldMode
                Dim drop As Integer
                If compactVal = INFLATED_Renamed Then
                    prec = bigDigitLength(intVal)
                    drop = prec - mcp
                    Do While drop > 0
                        scale_Renamed = checkScaleNonZero(CLng(scale_Renamed) - drop)
                        intVal = divideAndRoundByTenPow(intVal, drop, mode)
                        compactVal = compactValFor(intVal)
                        If compactVal <> INFLATED_Renamed Then Exit Do
                        prec = bigDigitLength(intVal)
                        drop = prec - mcp
                    Loop
                End If
                If compactVal <> INFLATED_Renamed Then
                    prec = longDigitLength(compactVal)
                    drop = prec - mcp
                    Do While drop > 0
                        scale_Renamed = checkScaleNonZero(CLng(scale_Renamed) - drop)
                        compactVal = divideAndRound(compactVal, LONG_TEN_POWERS_TABLE(drop), mc.roundingMode.oldMode)
                        prec = longDigitLength(compactVal)
                        drop = prec - mcp
                    Loop
                    intVal = Nothing
                End If
            End If
            Me.intVal = intVal
            Me.intCompact = compactVal
            Me.scale_Renamed = scale_Renamed
            Me.precision_Renamed = prec
        End Sub

        ''' <summary>
        ''' Translates a {@code BigInteger} into a {@code BigDecimal}.
        ''' The scale of the {@code BigDecimal} is zero.
        ''' </summary>
        ''' <param name="val"> {@code BigInteger} value to be converted to
        '''            {@code BigDecimal}. </param>
        Public Sub New(ByVal val As BigInteger)
            scale_Renamed = 0
            intVal = val
            intCompact = compactValFor(val)
        End Sub

        ''' <summary>
        ''' Translates a {@code BigInteger} into a {@code BigDecimal}
        ''' rounding according to the context settings.  The scale of the
        ''' {@code BigDecimal} is zero.
        ''' </summary>
        ''' <param name="val"> {@code BigInteger} value to be converted to
        '''            {@code BigDecimal}. </param>
        ''' <param name="mc"> the context to use. </param>
        ''' <exception cref="ArithmeticException"> if the result is inexact but the
        '''         rounding mode is {@code UNNECESSARY}.
        ''' @since  1.5 </exception>
        Public Sub New(ByVal val As BigInteger, ByVal mc As MathContext)
            Me.New(val, 0, mc)
        End Sub

        ''' <summary>
        ''' Translates a {@code BigInteger} unscaled value and an
        ''' {@code int} scale into a {@code BigDecimal}.  The value of
        ''' the {@code BigDecimal} is
        ''' <tt>(unscaledVal &times; 10<sup>-scale</sup>)</tt>.
        ''' </summary>
        ''' <param name="unscaledVal"> unscaled value of the {@code BigDecimal}. </param>
        ''' <param name="scale"> scale of the {@code BigDecimal}. </param>
        Public Sub New(ByVal unscaledVal As BigInteger, ByVal scale As Integer)
            ' Negative scales are now allowed
            Me.intVal = unscaledVal
            Me.intCompact = compactValFor(unscaledVal)
            Me.scale_Renamed = scale
        End Sub

        ''' <summary>
        ''' Translates a {@code BigInteger} unscaled value and an
        ''' {@code int} scale into a {@code BigDecimal}, with rounding
        ''' according to the context settings.  The value of the
        ''' {@code BigDecimal} is <tt>(unscaledVal &times;
        ''' 10<sup>-scale</sup>)</tt>, rounded according to the
        ''' {@code precision} and rounding mode settings.
        ''' </summary>
        ''' <param name="unscaledVal"> unscaled value of the {@code BigDecimal}. </param>
        ''' <param name="scale"> scale of the {@code BigDecimal}. </param>
        ''' <param name="mc"> the context to use. </param>
        ''' <exception cref="ArithmeticException"> if the result is inexact but the
        '''         rounding mode is {@code UNNECESSARY}.
        ''' @since  1.5 </exception>
        Public Sub New(ByVal unscaledVal As BigInteger, ByVal scale As Integer, ByVal mc As MathContext)
            Dim compactVal As Long = compactValFor(unscaledVal)
            Dim mcp As Integer = mc.precision
            Dim prec As Integer = 0
            If mcp > 0 Then ' do rounding
                Dim mode As Integer = mc.roundingMode.oldMode
                If compactVal = INFLATED_Renamed Then
                    prec = bigDigitLength(unscaledVal)
                    Dim drop As Integer = prec - mcp
                    Do While drop > 0
                        scale = checkScaleNonZero(CLng(scale) - drop)
                        unscaledVal = divideAndRoundByTenPow(unscaledVal, drop, mode)
                        compactVal = compactValFor(unscaledVal)
                        If compactVal <> INFLATED_Renamed Then Exit Do
                        prec = bigDigitLength(unscaledVal)
                        drop = prec - mcp
                    Loop
                End If
                If compactVal <> INFLATED_Renamed Then
                    prec = longDigitLength(compactVal)
                    Dim drop As Integer = prec - mcp ' drop can't be more than 18
                    Do While drop > 0
                        scale = checkScaleNonZero(CLng(scale) - drop)
                        compactVal = divideAndRound(compactVal, LONG_TEN_POWERS_TABLE(drop), mode)
                        prec = longDigitLength(compactVal)
                        drop = prec - mcp
                    Loop
                    unscaledVal = Nothing
                End If
            End If
            Me.intVal = unscaledVal
            Me.intCompact = compactVal
            Me.scale_Renamed = scale
            Me.precision_Renamed = prec
        End Sub

        ''' <summary>
        ''' Translates an {@code int} into a {@code BigDecimal}.  The
        ''' scale of the {@code BigDecimal} is zero.
        ''' </summary>
        ''' <param name="val"> {@code int} value to be converted to
        '''            {@code BigDecimal}.
        ''' @since  1.5 </param>
        Public Sub New(ByVal val As Integer)
            Me.intCompact = val
            Me.scale_Renamed = 0
            Me.intVal = Nothing
        End Sub

        ''' <summary>
        ''' Translates an {@code int} into a {@code BigDecimal}, with
        ''' rounding according to the context settings.  The scale of the
        ''' {@code BigDecimal}, before any rounding, is zero.
        ''' </summary>
        ''' <param name="val"> {@code int} value to be converted to {@code BigDecimal}. </param>
        ''' <param name="mc"> the context to use. </param>
        ''' <exception cref="ArithmeticException"> if the result is inexact but the
        '''         rounding mode is {@code UNNECESSARY}.
        ''' @since  1.5 </exception>
        Public Sub New(ByVal val As Integer, ByVal mc As MathContext)
            Dim mcp As Integer = mc.precision
            Dim compactVal As Long = val
            Dim scale_Renamed As Integer = 0
            Dim prec As Integer = 0
            If mcp > 0 Then ' do rounding
                prec = longDigitLength(compactVal)
                Dim drop As Integer = prec - mcp ' drop can't be more than 18
                Do While drop > 0
                    scale_Renamed = checkScaleNonZero(CLng(scale_Renamed) - drop)
                    compactVal = divideAndRound(compactVal, LONG_TEN_POWERS_TABLE(drop), mc.roundingMode.oldMode)
                    prec = longDigitLength(compactVal)
                    drop = prec - mcp
                Loop
            End If
            Me.intVal = Nothing
            Me.intCompact = compactVal
            Me.scale_Renamed = scale_Renamed
            Me.precision_Renamed = prec
        End Sub

        ''' <summary>
        ''' Translates a {@code long} into a {@code BigDecimal}.  The
        ''' scale of the {@code BigDecimal} is zero.
        ''' </summary>
        ''' <param name="val"> {@code long} value to be converted to {@code BigDecimal}.
        ''' @since  1.5 </param>
        Public Sub New(ByVal val As Long)
            Me.intCompact = val
            Me.intVal = If(val = INFLATED_Renamed, INFLATED_BIGINT, Nothing)
            Me.scale_Renamed = 0
        End Sub

        ''' <summary>
        ''' Translates a {@code long} into a {@code BigDecimal}, with
        ''' rounding according to the context settings.  The scale of the
        ''' {@code BigDecimal}, before any rounding, is zero.
        ''' </summary>
        ''' <param name="val"> {@code long} value to be converted to {@code BigDecimal}. </param>
        ''' <param name="mc"> the context to use. </param>
        ''' <exception cref="ArithmeticException"> if the result is inexact but the
        '''         rounding mode is {@code UNNECESSARY}.
        ''' @since  1.5 </exception>
        Public Sub New(ByVal val As Long, ByVal mc As MathContext)
            Dim mcp As Integer = mc.precision
            Dim mode As Integer = mc.roundingMode.oldMode
            Dim prec As Integer = 0
            Dim scale_Renamed As Integer = 0
            Dim intVal As BigInteger = If(val = INFLATED_Renamed, INFLATED_BIGINT, Nothing)
            If mcp > 0 Then ' do rounding
                If val = INFLATED_Renamed Then
                    prec = 19
                    Dim drop As Integer = prec - mcp
                    Do While drop > 0
                        scale_Renamed = checkScaleNonZero(CLng(scale_Renamed) - drop)
                        intVal = divideAndRoundByTenPow(intVal, drop, mode)
                        val = compactValFor(intVal)
                        If val <> INFLATED_Renamed Then Exit Do
                        prec = bigDigitLength(intVal)
                        drop = prec - mcp
                    Loop
                End If
                If val <> INFLATED_Renamed Then
                    prec = longDigitLength(val)
                    Dim drop As Integer = prec - mcp
                    Do While drop > 0
                        scale_Renamed = checkScaleNonZero(CLng(scale_Renamed) - drop)
                        val = divideAndRound(val, LONG_TEN_POWERS_TABLE(drop), mc.roundingMode.oldMode)
                        prec = longDigitLength(val)
                        drop = prec - mcp
                    Loop
                    intVal = Nothing
                End If
            End If
            Me.intVal = intVal
            Me.intCompact = val
            Me.scale_Renamed = scale_Renamed
            Me.precision_Renamed = prec
        End Sub

        ' Static Factory Methods

        ''' <summary>
        ''' Translates a {@code long} unscaled value and an
        ''' {@code int} scale into a {@code BigDecimal}.  This
        ''' {@literal "static factory method"} is provided in preference to
        ''' a ({@code long}, {@code int}) constructor because it
        ''' allows for reuse of frequently used {@code BigDecimal} values..
        ''' </summary>
        ''' <param name="unscaledVal"> unscaled value of the {@code BigDecimal}. </param>
        ''' <param name="scale"> scale of the {@code BigDecimal}. </param>
        ''' <returns> a {@code BigDecimal} whose value is
        '''         <tt>(unscaledVal &times; 10<sup>-scale</sup>)</tt>. </returns>
        Public Shared Function valueOf(ByVal unscaledVal As Long, ByVal scale As Integer) As BigDecimal
            If scale = 0 Then
                Return valueOf(unscaledVal)
            ElseIf unscaledVal = 0 Then
                Return zeroValueOf(scale)
            End If
            Return New BigDecimal(If(unscaledVal = INFLATED_Renamed, INFLATED_BIGINT, Nothing), unscaledVal, scale, 0)
        End Function

        ''' <summary>
        ''' Translates a {@code long} value into a {@code BigDecimal}
        ''' with a scale of zero.  This {@literal "static factory method"}
        ''' is provided in preference to a ({@code long}) constructor
        ''' because it allows for reuse of frequently used
        ''' {@code BigDecimal} values.
        ''' </summary>
        ''' <param name="val"> value of the {@code BigDecimal}. </param>
        ''' <returns> a {@code BigDecimal} whose value is {@code val}. </returns>
        Public Shared Function valueOf(ByVal val As Long) As BigDecimal
            If val >= 0 AndAlso val < zeroThroughTen.Length Then
                Return zeroThroughTen(CInt(val))
            ElseIf val <> INFLATED_Renamed Then
                Return New BigDecimal(Nothing, val, 0, 0)
            End If
            Return New BigDecimal(INFLATED_BIGINT, val, 0, 0)
        End Function

        Shared Function valueOf(ByVal unscaledVal As Long, ByVal scale As Integer, ByVal prec As Integer) As BigDecimal
            If scale = 0 AndAlso unscaledVal >= 0 AndAlso unscaledVal < zeroThroughTen.Length Then
                Return zeroThroughTen(CInt(unscaledVal))
            ElseIf unscaledVal = 0 Then
                Return zeroValueOf(scale)
            End If
            Return New BigDecimal(If(unscaledVal = INFLATED_Renamed, INFLATED_BIGINT, Nothing), unscaledVal, scale, prec)
        End Function

        Shared Function valueOf(ByVal intVal As BigInteger, ByVal scale As Integer, ByVal prec As Integer) As BigDecimal
            Dim val As Long = compactValFor(intVal)
            If val = 0 Then
                Return zeroValueOf(scale)
            ElseIf scale = 0 AndAlso val >= 0 AndAlso val < zeroThroughTen.Length Then
                Return zeroThroughTen(CInt(val))
            End If
            Return New BigDecimal(intVal, val, scale, prec)
        End Function

        Shared Function zeroValueOf(ByVal scale As Integer) As BigDecimal
            If scale >= 0 AndAlso scale < ZERO_SCALED_BY.Length Then
                Return ZERO_SCALED_BY(scale)
            Else
                Return New BigDecimal(Big java.lang.[Integer].ZERO, 0, scale, 1)
            End If
        End Function

        ''' <summary>
        ''' Translates a {@code double} into a {@code BigDecimal}, using
        ''' the {@code double}'s canonical string representation provided
        ''' by the <seealso cref="Double#toString(double)"/> method.
        ''' 
        ''' <p><b>Note:</b> This is generally the preferred way to convert
        ''' a {@code double} (or {@code float}) into a
        ''' {@code BigDecimal}, as the value returned is equal to that
        ''' resulting from constructing a {@code BigDecimal} from the
        ''' result of using <seealso cref="Double#toString(double)"/>.
        ''' </summary>
        ''' <param name="val"> {@code double} to convert to a {@code BigDecimal}. </param>
        ''' <returns> a {@code BigDecimal} whose value is equal to or approximately
        '''         equal to the value of {@code val}. </returns>
        ''' <exception cref="NumberFormatException"> if {@code val} is infinite or NaN.
        ''' @since  1.5 </exception>
        Public Shared Function valueOf(ByVal val As Double) As BigDecimal
            ' Reminder: a zero double returns '0.0', so we cannot fastpath
            ' to use the constant ZERO.  This might be important enough to
            ' justify a factory approach, a cache, or a few private
            ' constants, later.
            Return New BigDecimal(Convert.ToString(val))
        End Function

        ' Arithmetic Operations
        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is {@code (this +
        ''' augend)}, and whose scale is {@code max(this.scale(),
        ''' augend.scale())}.
        ''' </summary>
        ''' <param name="augend"> value to be added to this {@code BigDecimal}. </param>
        ''' <returns> {@code this + augend} </returns>
        Public Overridable Function add(ByVal augend As BigDecimal) As BigDecimal
            If Me.intCompact <> INFLATED_Renamed Then
                If (augend.intCompact <> INFLATED_Renamed) Then
                    Return add(Me.intCompact, Me.scale_Renamed, augend.intCompact, augend.scale_Renamed)
                Else
                    Return add(Me.intCompact, Me.scale_Renamed, augend.intVal, augend.scale_Renamed)
                End If
            Else
                If (augend.intCompact <> INFLATED_Renamed) Then
                    Return add(augend.intCompact, augend.scale_Renamed, Me.intVal, Me.scale_Renamed)
                Else
                    Return add(Me.intVal, Me.scale_Renamed, augend.intVal, augend.scale_Renamed)
                End If
            End If
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is {@code (this + augend)},
        ''' with rounding according to the context settings.
        ''' 
        ''' If either number is zero and the precision setting is nonzero then
        ''' the other number, rounded if necessary, is used as the result.
        ''' </summary>
        ''' <param name="augend"> value to be added to this {@code BigDecimal}. </param>
        ''' <param name="mc"> the context to use. </param>
        ''' <returns> {@code this + augend}, rounded as necessary. </returns>
        ''' <exception cref="ArithmeticException"> if the result is inexact but the
        '''         rounding mode is {@code UNNECESSARY}.
        ''' @since  1.5 </exception>
        Public Overridable Function add(ByVal augend As BigDecimal, ByVal mc As MathContext) As BigDecimal
            If mc.precision = 0 Then Return add(augend)
            Dim lhs As BigDecimal = Me

            ' If either number is zero then the other number, rounded and
            ' scaled if necessary, is used as the result.
            Dim lhsIsZero As Boolean = lhs.signum() = 0
            Dim augendIsZero As Boolean = augend.signum() = 0

            If lhsIsZero OrElse augendIsZero Then
                Dim preferredScale As Integer = System.Math.Max(lhs.scale(), augend.scale())
                Dim result As BigDecimal

                If lhsIsZero AndAlso augendIsZero Then Return zeroValueOf(preferredScale)
                result = If(lhsIsZero, doRound(augend, mc), doRound(lhs, mc))

                If result.scale() = preferredScale Then
                    Return result
                ElseIf result.scale() > preferredScale Then
                    Return stripZerosToMatchScale(result.intVal, result.intCompact, result.scale_Renamed, preferredScale) ' result.scale < preferredScale
                Else
                    Dim precisionDiff As Integer = mc.precision - result.precision()
                    Dim scaleDiff As Integer = preferredScale - result.scale()

                    If precisionDiff >= scaleDiff Then
                        Return result.scaleale(preferredScale) ' can achieve target scale
                    Else
                        Return result.scaleale(result.scale() + precisionDiff)
                    End If
                End If
            End If

            Dim padding As Long = CLng(Fix(lhs.scale_Renamed)) - augend.scale_Renamed
            If padding <> 0 Then ' scales differ; alignment needed
                Dim arg As BigDecimal() = preAlign(lhs, augend, padding, mc)
                matchScale(arg)
                lhs = arg(0)
                augend = arg(1)
            End If
            Return doRound(lhs.inflated().add(augend.inflated()), lhs.scale_Renamed, mc)
        End Function

        ''' <summary>
        ''' Returns an array of length two, the sum of whose entries is
        ''' equal to the rounded sum of the {@code BigDecimal} arguments.
        ''' 
        ''' <p>If the digit positions of the arguments have a sufficient
        ''' gap between them, the value smaller in magnitude can be
        ''' condensed into a {@literal "sticky bit"} and the end result will
        ''' round the same way <em>if</em> the precision of the final
        ''' result does not include the high order digit of the small
        ''' magnitude operand.
        ''' 
        ''' <p>Note that while strictly speaking this is an optimization,
        ''' it makes a much wider range of additions practical.
        ''' 
        ''' <p>This corresponds to a pre-shift operation in a fixed
        ''' precision floating-point adder; this method is complicated by
        ''' variable precision of the result as determined by the
        ''' MathContext.  A more nuanced operation could implement a
        ''' {@literal "right shift"} on the smaller magnitude operand so
        ''' that the number of digits of the smaller operand could be
        ''' reduced even though the significands partially overlapped.
        ''' </summary>
        Private Function preAlign(ByVal lhs As BigDecimal, ByVal augend As BigDecimal, ByVal padding As Long, ByVal mc As MathContext) As BigDecimal()
            Debug.Assert(padding <> 0)
            Dim big As BigDecimal
            Dim small As BigDecimal

            If padding < 0 Then ' lhs is big; augend is small
                big = lhs
                small = augend ' lhs is small; augend is big
            Else
                big = augend
                small = lhs
            End If

            '        
            '         * This is the estimated scale of an ulp of the result; it assumes that
            '         * the result doesn't have a carry-out on a true add (e.g. 999 + 1 =>
            '         * 1000) or any subtractive cancellation on borrowing (e.g. 100 - 1.2 =>
            '         * 98.8)
            '         
            Dim estResultUlpScale As Long = CLng(Fix(big.scale_Renamed)) - big.precision() + mc.precision

            '        
            '         * The low-order digit position of big is big.scale().  This
            '         * is true regardless of whether big has a positive or
            '         * negative scale.  The high-order digit position of small is
            '         * small.scale - (small.precision() - 1).  To do the full
            '         * condensation, the digit positions of big and small must be
            '         * disjoint *and* the digit positions of small should not be
            '         * directly visible in the result.
            '         
            Dim smallHighDigitPos As Long = CLng(Fix(small.scale_Renamed)) - small.precision() + 1
            If smallHighDigitPos > big.scale_Renamed + 2 AndAlso smallHighDigitPos > estResultUlpScale + 2 Then ' small digits not visible -  big and small disjoint small = BigDecimal.valueOf(small.signum(), Me.checkScale (System.Math.Max(big.scale_Renamed, estResultUlpScale) + 3))

                ' Since addition is symmetric, preserving input order in
                ' returned operands doesn't matter
                Dim result As BigDecimal() = {big, small}
                Return result
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is {@code (this -
        ''' subtrahend)}, and whose scale is {@code max(this.scale(),
        ''' subtrahend.scale())}.
        ''' </summary>
        ''' <param name="subtrahend"> value to be subtracted from this {@code BigDecimal}. </param>
        ''' <returns> {@code this - subtrahend} </returns>
        Public Overridable Function subtract(ByVal subtrahend As BigDecimal) As BigDecimal
            If Me.intCompact <> INFLATED_Renamed Then
                If (subtrahend.intCompact <> INFLATED_Renamed) Then
                    Return add(Me.intCompact, Me.scale_Renamed, -subtrahend.intCompact, subtrahend.scale_Renamed)
                Else
                    Return add(Me.intCompact, Me.scale_Renamed, subtrahend.intVal.negate(), subtrahend.scale_Renamed)
                End If
            Else
                If (subtrahend.intCompact <> INFLATED_Renamed) Then
                    ' Pair of subtrahend values given before pair of
                    ' values from this BigDecimal to avoid need for
                    ' method overloading on the specialized add method
                    Return add(-subtrahend.intCompact, subtrahend.scale_Renamed, Me.intVal, Me.scale_Renamed)
                Else
                    Return add(Me.intVal, Me.scale_Renamed, subtrahend.intVal.negate(), subtrahend.scale_Renamed)
                End If
            End If
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is {@code (this - subtrahend)},
        ''' with rounding according to the context settings.
        ''' 
        ''' If {@code subtrahend} is zero then this, rounded if necessary, is used as the
        ''' result.  If this is zero then the result is {@code subtrahend.negate(mc)}.
        ''' </summary>
        ''' <param name="subtrahend"> value to be subtracted from this {@code BigDecimal}. </param>
        ''' <param name="mc"> the context to use. </param>
        ''' <returns> {@code this - subtrahend}, rounded as necessary. </returns>
        ''' <exception cref="ArithmeticException"> if the result is inexact but the
        '''         rounding mode is {@code UNNECESSARY}.
        ''' @since  1.5 </exception>
        Public Overridable Function subtract(ByVal subtrahend As BigDecimal, ByVal mc As MathContext) As BigDecimal
            If mc.precision = 0 Then Return subtract(subtrahend)
            ' share the special rounding code in add()
            Return add(subtrahend.negate(), mc)
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is <tt>(this &times;
        ''' multiplicand)</tt>, and whose scale is {@code (this.scale() +
        ''' multiplicand.scale())}.
        ''' </summary>
        ''' <param name="multiplicand"> value to be multiplied by this {@code BigDecimal}. </param>
        ''' <returns> {@code this * multiplicand} </returns>
        Public Overridable Function multiply(ByVal multiplicand As BigDecimal) As BigDecimal
            Dim productScale As Integer = checkScale(CLng(scale_Renamed) + multiplicand.scale_Renamed)
            If Me.intCompact <> INFLATED_Renamed Then
                If (multiplicand.intCompact <> INFLATED_Renamed) Then
                    Return multiply(Me.intCompact, multiplicand.intCompact, productScale)
                Else
                    Return multiply(Me.intCompact, multiplicand.intVal, productScale)
                End If
            Else
                If (multiplicand.intCompact <> INFLATED_Renamed) Then
                    Return multiply(multiplicand.intCompact, Me.intVal, productScale)
                Else
                    Return multiply(Me.intVal, multiplicand.intVal, productScale)
                End If
            End If
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is <tt>(this &times;
        ''' multiplicand)</tt>, with rounding according to the context settings.
        ''' </summary>
        ''' <param name="multiplicand"> value to be multiplied by this {@code BigDecimal}. </param>
        ''' <param name="mc"> the context to use. </param>
        ''' <returns> {@code this * multiplicand}, rounded as necessary. </returns>
        ''' <exception cref="ArithmeticException"> if the result is inexact but the
        '''         rounding mode is {@code UNNECESSARY}.
        ''' @since  1.5 </exception>
        Public Overridable Function multiply(ByVal multiplicand As BigDecimal, ByVal mc As MathContext) As BigDecimal
            If mc.precision = 0 Then Return multiply(multiplicand)
            Dim productScale As Integer = checkScale(CLng(scale_Renamed) + multiplicand.scale_Renamed)
            If Me.intCompact <> INFLATED_Renamed Then
                If (multiplicand.intCompact <> INFLATED_Renamed) Then
                    Return multiplyAndRound(Me.intCompact, multiplicand.intCompact, productScale, mc)
                Else
                    Return multiplyAndRound(Me.intCompact, multiplicand.intVal, productScale, mc)
                End If
            Else
                If (multiplicand.intCompact <> INFLATED_Renamed) Then
                    Return multiplyAndRound(multiplicand.intCompact, Me.intVal, productScale, mc)
                Else
                    Return multiplyAndRound(Me.intVal, multiplicand.intVal, productScale, mc)
                End If
            End If
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is {@code (this /
        ''' divisor)}, and whose scale is as specified.  If rounding must
        ''' be performed to generate a result with the specified scale, the
        ''' specified rounding mode is applied.
        ''' 
        ''' <p>The new <seealso cref="#divide(BigDecimal, int, RoundingMode)"/> method
        ''' should be used in preference to this legacy method.
        ''' </summary>
        ''' <param name="divisor"> value by which this {@code BigDecimal} is to be divided. </param>
        ''' <param name="scale"> scale of the {@code BigDecimal} quotient to be returned. </param>
        ''' <param name="roundingMode"> rounding mode to apply. </param>
        ''' <returns> {@code this / divisor} </returns>
        ''' <exception cref="ArithmeticException"> if {@code divisor} is zero,
        '''         {@code roundingMode==ROUND_UNNECESSARY} and
        '''         the specified scale is insufficient to represent the result
        '''         of the division exactly. </exception>
        ''' <exception cref="IllegalArgumentException"> if {@code roundingMode} does not
        '''         represent a valid rounding mode. </exception>
        ''' <seealso cref=    #ROUND_UP </seealso>
        ''' <seealso cref=    #ROUND_DOWN </seealso>
        ''' <seealso cref=    #ROUND_CEILING </seealso>
        ''' <seealso cref=    #ROUND_FLOOR </seealso>
        ''' <seealso cref=    #ROUND_HALF_UP </seealso>
        ''' <seealso cref=    #ROUND_HALF_DOWN </seealso>
        ''' <seealso cref=    #ROUND_HALF_EVEN </seealso>
        ''' <seealso cref=    #ROUND_UNNECESSARY </seealso>
        Public Overridable Function divide(ByVal divisor As BigDecimal, ByVal scale As Integer, ByVal roundingMode As Integer) As BigDecimal
            If roundingMode < ROUND_UP OrElse roundingMode > ROUND_UNNECESSARY Then Throw New IllegalArgumentException("Invalid rounding mode")
            If Me.intCompact <> INFLATED_Renamed Then
                If (divisor.intCompact <> INFLATED_Renamed) Then
                    Return divide(Me.intCompact, Me.scale_Renamed, divisor.intCompact, divisor.scale_Renamed, scale, roundingMode)
                Else
                    Return divide(Me.intCompact, Me.scale_Renamed, divisor.intVal, divisor.scale_Renamed, scale, roundingMode)
                End If
            Else
                If (divisor.intCompact <> INFLATED_Renamed) Then
                    Return divide(Me.intVal, Me.scale_Renamed, divisor.intCompact, divisor.scale_Renamed, scale, roundingMode)
                Else
                    Return divide(Me.intVal, Me.scale_Renamed, divisor.intVal, divisor.scale_Renamed, scale, roundingMode)
                End If
            End If
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is {@code (this /
        ''' divisor)}, and whose scale is as specified.  If rounding must
        ''' be performed to generate a result with the specified scale, the
        ''' specified rounding mode is applied.
        ''' </summary>
        ''' <param name="divisor"> value by which this {@code BigDecimal} is to be divided. </param>
        ''' <param name="scale"> scale of the {@code BigDecimal} quotient to be returned. </param>
        ''' <param name="roundingMode"> rounding mode to apply. </param>
        ''' <returns> {@code this / divisor} </returns>
        ''' <exception cref="ArithmeticException"> if {@code divisor} is zero,
        '''         {@code roundingMode==RoundingMode.UNNECESSARY} and
        '''         the specified scale is insufficient to represent the result
        '''         of the division exactly.
        ''' @since 1.5 </exception>
        Public Overridable Function divide(ByVal divisor As BigDecimal, ByVal scale As Integer, ByVal roundingMode As RoundingMode) As BigDecimal
            Return divide(divisor, scale, roundingMode.oldMode)
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is {@code (this /
        ''' divisor)}, and whose scale is {@code this.scale()}.  If
        ''' rounding must be performed to generate a result with the given
        ''' scale, the specified rounding mode is applied.
        ''' 
        ''' <p>The new <seealso cref="#divide(BigDecimal, RoundingMode)"/> method
        ''' should be used in preference to this legacy method.
        ''' </summary>
        ''' <param name="divisor"> value by which this {@code BigDecimal} is to be divided. </param>
        ''' <param name="roundingMode"> rounding mode to apply. </param>
        ''' <returns> {@code this / divisor} </returns>
        ''' <exception cref="ArithmeticException"> if {@code divisor==0}, or
        '''         {@code roundingMode==ROUND_UNNECESSARY} and
        '''         {@code this.scale()} is insufficient to represent the result
        '''         of the division exactly. </exception>
        ''' <exception cref="IllegalArgumentException"> if {@code roundingMode} does not
        '''         represent a valid rounding mode. </exception>
        ''' <seealso cref=    #ROUND_UP </seealso>
        ''' <seealso cref=    #ROUND_DOWN </seealso>
        ''' <seealso cref=    #ROUND_CEILING </seealso>
        ''' <seealso cref=    #ROUND_FLOOR </seealso>
        ''' <seealso cref=    #ROUND_HALF_UP </seealso>
        ''' <seealso cref=    #ROUND_HALF_DOWN </seealso>
        ''' <seealso cref=    #ROUND_HALF_EVEN </seealso>
        ''' <seealso cref=    #ROUND_UNNECESSARY </seealso>
        Public Overridable Function divide(ByVal divisor As BigDecimal, ByVal roundingMode As Integer) As BigDecimal
            Return Me.divide(divisor, scale_Renamed, roundingMode)
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is {@code (this /
        ''' divisor)}, and whose scale is {@code this.scale()}.  If
        ''' rounding must be performed to generate a result with the given
        ''' scale, the specified rounding mode is applied.
        ''' </summary>
        ''' <param name="divisor"> value by which this {@code BigDecimal} is to be divided. </param>
        ''' <param name="roundingMode"> rounding mode to apply. </param>
        ''' <returns> {@code this / divisor} </returns>
        ''' <exception cref="ArithmeticException"> if {@code divisor==0}, or
        '''         {@code roundingMode==RoundingMode.UNNECESSARY} and
        '''         {@code this.scale()} is insufficient to represent the result
        '''         of the division exactly.
        ''' @since 1.5 </exception>
        Public Overridable Function divide(ByVal divisor As BigDecimal, ByVal roundingMode As RoundingMode) As BigDecimal
            Return Me.divide(divisor, scale_Renamed, roundingMode.oldMode)
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is {@code (this /
        ''' divisor)}, and whose preferred scale is {@code (this.scale() -
        ''' divisor.scale())}; if the exact quotient cannot be
        ''' represented (because it has a non-terminating decimal
        ''' expansion) an {@code ArithmeticException} is thrown.
        ''' </summary>
        ''' <param name="divisor"> value by which this {@code BigDecimal} is to be divided. </param>
        ''' <exception cref="ArithmeticException"> if the exact quotient does not have a
        '''         terminating decimal expansion </exception>
        ''' <returns> {@code this / divisor}
        ''' @since 1.5
        ''' @author Joseph D. Darcy </returns>
        Public Overridable Function divide(ByVal divisor As BigDecimal) As BigDecimal
            '        
            '         * Handle zero cases first.
            '         
            If divisor.signum() = 0 Then ' x/0
                If Me.signum() = 0 Then ' 0/0 Throw New ArithmeticException("Division undefined") ' NaN
                    Throw New ArithmeticException("Division by zero")
                End If

                ' Calculate preferred scale
                Dim preferredScale As Integer = saturateLong(CLng(Me.scale_Renamed) - divisor.scale_Renamed)

                If Me.signum() = 0 Then ' 0/y
                    Return zeroValueOf(preferredScale)
                Else
                    '            
                    '             * If the quotient this/divisor has a terminating decimal
                    '             * expansion, the expansion can have no more than
                    '             * (a.precision() + ceil(10*b.precision)/3) digits.
                    '             * Therefore, create a MathContext object with this
                    '             * precision and do a divide with the UNNECESSARY rounding
                    '             * mode.
                    '             
                    Dim mc As New MathContext(CInt(Fix (System.Math.Min(Me.precision() + CLng(Fix (System.Math.Ceiling(10.0 * divisor.precision() / 3.0))),  java.lang.[Integer].Max_Value))), RoundingMode.UNNECESSARY)
                    Dim quotient As BigDecimal
                    Try
                        quotient = Me.divide(divisor, mc)
                    Catch e As ArithmeticException
                        Throw New ArithmeticException("Non-terminating decimal expansion; " & "no exact representable decimal result.")
                    End Try

                    Dim quotientScale As Integer = quotient.scale()

                    ' divide(BigDecimal, mc) tries to adjust the quotient to
                    ' the desired one by removing trailing zeros; since the
                    ' exact divide method does not have an explicit digit
                    ' limit, we can add zeros too.
                    If preferredScale > quotientScale Then Return quotient.scaleale(preferredScale, ROUND_UNNECESSARY)

                    Return quotient
                End If
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is {@code (this /
        ''' divisor)}, with rounding according to the context settings.
        ''' </summary>
        ''' <param name="divisor"> value by which this {@code BigDecimal} is to be divided. </param>
        ''' <param name="mc"> the context to use. </param>
        ''' <returns> {@code this / divisor}, rounded as necessary. </returns>
        ''' <exception cref="ArithmeticException"> if the result is inexact but the
        '''         rounding mode is {@code UNNECESSARY} or
        '''         {@code mc.precision == 0} and the quotient has a
        '''         non-terminating decimal expansion.
        ''' @since  1.5 </exception>
        Public Overridable Function divide(ByVal divisor As BigDecimal, ByVal mc As MathContext) As BigDecimal
            Dim mcp As Integer = mc.precision
            If mcp = 0 Then Return divide(divisor)

            Dim dividend As BigDecimal = Me
            Dim preferredScale As Long = CLng(Fix(dividend.scale_Renamed)) - divisor.scale_Renamed
            ' Now calculate the answer.  We use the existing
            ' divide-and-round method, but as this rounds to scale we have
            ' to normalize the values here to achieve the desired result.
            ' For x/y we first handle y=0 and x=0, and then normalize x and
            ' y to give x' and y' with the following constraints:
            '   (a) 0.1 <= x' < 1
            '   (b)  x' <= y' < 10*x'
            ' Dividing x'/y' with the required scale set to mc.precision then
            ' will give a result in the range 0.1 to 1 rounded to exactly
            ' the right number of digits (except in the case of a result of
            ' 1.000... which can arise when x=y, or when rounding overflows
            ' The 1.000... case will reduce properly to 1.
            If divisor.signum() = 0 Then ' x/0
                If dividend.signum() = 0 Then ' 0/0 Throw New ArithmeticException("Division undefined") ' NaN
                    Throw New ArithmeticException("Division by zero")
                End If
                If dividend.signum() = 0 Then ' 0/y Return zeroValueOf(saturateLong(preferredScale))
                    Dim xscale As Integer = dividend.precision()
                    Dim yscale As Integer = divisor.precision()
                    If dividend.intCompact <> INFLATED_Renamed Then
                        If divisor.intCompact <> INFLATED_Renamed Then
                            Return divide(dividend.intCompact, xscale, divisor.intCompact, yscale, preferredScale, mc)
                        Else
                            Return divide(dividend.intCompact, xscale, divisor.intVal, yscale, preferredScale, mc)
                        End If
                    Else
                        If divisor.intCompact <> INFLATED_Renamed Then
                            Return divide(dividend.intVal, xscale, divisor.intCompact, yscale, preferredScale, mc)
                        Else
                            Return divide(dividend.intVal, xscale, divisor.intVal, yscale, preferredScale, mc)
                        End If
                    End If
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is the integer part
        ''' of the quotient {@code (this / divisor)} rounded down.  The
        ''' preferred scale of the result is {@code (this.scale() -
        ''' divisor.scale())}.
        ''' </summary>
        ''' <param name="divisor"> value by which this {@code BigDecimal} is to be divided. </param>
        ''' <returns> The integer part of {@code this / divisor}. </returns>
        ''' <exception cref="ArithmeticException"> if {@code divisor==0}
        ''' @since  1.5 </exception>
        Public Overridable Function divideToIntegralValue(ByVal divisor As BigDecimal) As BigDecimal
            ' Calculate preferred scale
            Dim preferredScale As Integer = saturateLong(CLng(Me.scale_Renamed) - divisor.scale_Renamed)
            If Me.compareMagnitude(divisor) < 0 Then Return zeroValueOf(preferredScale)

            If Me.signum() = 0 AndAlso divisor.signum() <> 0 Then Return Me.scaleale(preferredScale, ROUND_UNNECESSARY)

            ' Perform a divide with enough digits to round to a correct
            ' integer value; then remove any fractional digits

            Dim maxDigits As Integer = CInt(Fix (System.Math.Min(Me.precision() + CLng(Fix (System.Math.Ceiling(10.0 * divisor.precision() / 3.0))) + System.Math.Abs(CLng(Me.scale()) - divisor.scale()) + 2,  java.lang.[Integer].Max_Value)))
            Dim quotient As BigDecimal = Me.divide(divisor, New MathContext(maxDigits, RoundingMode.DOWN))
            If quotient.scale_Renamed > 0 Then
                quotient = quotient.scaleale(0, RoundingMode.DOWN)
                quotient = stripZerosToMatchScale(quotient.intVal, quotient.intCompact, quotient.scale_Renamed, preferredScale)
            End If

            If quotient.scale_Renamed < preferredScale Then quotient = quotient.scaleale(preferredScale, ROUND_UNNECESSARY)

            Return quotient
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is the integer part
        ''' of {@code (this / divisor)}.  Since the integer part of the
        ''' exact quotient does not depend on the rounding mode, the
        ''' rounding mode does not affect the values returned by this
        ''' method.  The preferred scale of the result is
        ''' {@code (this.scale() - divisor.scale())}.  An
        ''' {@code ArithmeticException} is thrown if the integer part of
        ''' the exact quotient needs more than {@code mc.precision}
        ''' digits.
        ''' </summary>
        ''' <param name="divisor"> value by which this {@code BigDecimal} is to be divided. </param>
        ''' <param name="mc"> the context to use. </param>
        ''' <returns> The integer part of {@code this / divisor}. </returns>
        ''' <exception cref="ArithmeticException"> if {@code divisor==0} </exception>
        ''' <exception cref="ArithmeticException"> if {@code mc.precision} {@literal >} 0 and the result
        '''         requires a precision of more than {@code mc.precision} digits.
        ''' @since  1.5
        ''' @author Joseph D. Darcy </exception>
        Public Overridable Function divideToIntegralValue(ByVal divisor As BigDecimal, ByVal mc As MathContext) As BigDecimal
            If mc.precision = 0 OrElse (Me.compareMagnitude(divisor) < 0) Then ' zero result -  exact result Return divideToIntegralValue(divisor)

                ' Calculate preferred scale
                Dim preferredScale As Integer = saturateLong(CLng(Me.scale_Renamed) - divisor.scale_Renamed)

                '        
                '         * Perform a normal divide to mc.precision digits.  If the
                '         * remainder has absolute value less than the divisor, the
                '         * integer portion of the quotient fits into mc.precision
                '         * digits.  Next, remove any fractional digits from the
                '         * quotient and adjust the scale to the preferred value.
                '         
                Dim result As BigDecimal = Me.divide(divisor, New MathContext(mc.precision, RoundingMode.DOWN))

                If result.scale() < 0 Then
                    '            
                    '             * Result is an  java.lang.[Integer]. See if quotient represents the
                    '             * full integer portion of the exact quotient; if it does,
                    '             * the computed remainder will be less than the divisor.
                    '             
                    Dim product As BigDecimal = result.multiply(divisor)
                    ' If the quotient is the full integer value,
                    ' |dividend-product| < |divisor|.
                    If Me.subtract(product).compareMagnitude(divisor) >= 0 Then Throw New ArithmeticException("Division impossible")
                ElseIf result.scale() > 0 Then
                    '            
                    '             * Integer portion of quotient will fit into precision
                    '             * digits; recompute quotient to scale 0 to avoid double
                    '             * rounding and then try to adjust, if necessary.
                    '             
                    result = result.scaleale(0, RoundingMode.DOWN)
                End If
                ' else result.scale() == 0;

                Dim precisionDiff As Integer
                precisionDiff = mc.precision - result.precision()
                If (preferredScale > result.scale()) AndAlso precisionDiff > 0 Then
                    Return result.scaleale(result.scale() + System.Math.Min(precisionDiff, preferredScale - result.scale_Renamed))
                Else
                    Return stripZerosToMatchScale(result.intVal, result.intCompact, result.scale_Renamed, preferredScale)
                End If
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is {@code (this % divisor)}.
        ''' 
        ''' <p>The remainder is given by
        ''' {@code this.subtract(this.divideToIntegralValue(divisor).multiply(divisor))}.
        ''' Note that this is not the modulo operation (the result can be
        ''' negative).
        ''' </summary>
        ''' <param name="divisor"> value by which this {@code BigDecimal} is to be divided. </param>
        ''' <returns> {@code this % divisor}. </returns>
        ''' <exception cref="ArithmeticException"> if {@code divisor==0}
        ''' @since  1.5 </exception>
        Public Overridable Function remainder(ByVal divisor As BigDecimal) As BigDecimal
            Dim divrem As BigDecimal() = Me.divideAndRemainder(divisor)
            Return divrem(1)
        End Function


        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is {@code (this %
        ''' divisor)}, with rounding according to the context settings.
        ''' The {@code MathContext} settings affect the implicit divide
        ''' used to compute the remainder.  The remainder computation
        ''' itself is by definition exact.  Therefore, the remainder may
        ''' contain more than {@code mc.getPrecision()} digits.
        ''' 
        ''' <p>The remainder is given by
        ''' {@code this.subtract(this.divideToIntegralValue(divisor,
        ''' mc).multiply(divisor))}.  Note that this is not the modulo
        ''' operation (the result can be negative).
        ''' </summary>
        ''' <param name="divisor"> value by which this {@code BigDecimal} is to be divided. </param>
        ''' <param name="mc"> the context to use. </param>
        ''' <returns> {@code this % divisor}, rounded as necessary. </returns>
        ''' <exception cref="ArithmeticException"> if {@code divisor==0} </exception>
        ''' <exception cref="ArithmeticException"> if the result is inexact but the
        '''         rounding mode is {@code UNNECESSARY}, or {@code mc.precision}
        '''         {@literal >} 0 and the result of {@code this.divideToIntgralValue(divisor)} would
        '''         require a precision of more than {@code mc.precision} digits. </exception>
        ''' <seealso cref=    #divideToIntegralValue(java.math.BigDecimal, java.math.MathContext)
        ''' @since  1.5 </seealso>
        Public Overridable Function remainder(ByVal divisor As BigDecimal, ByVal mc As MathContext) As BigDecimal
            Dim divrem As BigDecimal() = Me.divideAndRemainder(divisor, mc)
            Return divrem(1)
        End Function

        ''' <summary>
        ''' Returns a two-element {@code BigDecimal} array containing the
        ''' result of {@code divideToIntegralValue} followed by the result of
        ''' {@code remainder} on the two operands.
        ''' 
        ''' <p>Note that if both the integer quotient and remainder are
        ''' needed, this method is faster than using the
        ''' {@code divideToIntegralValue} and {@code remainder} methods
        ''' separately because the division need only be carried out once.
        ''' </summary>
        ''' <param name="divisor"> value by which this {@code BigDecimal} is to be divided,
        '''         and the remainder computed. </param>
        ''' <returns> a two element {@code BigDecimal} array: the quotient
        '''         (the result of {@code divideToIntegralValue}) is the initial element
        '''         and the remainder is the final element. </returns>
        ''' <exception cref="ArithmeticException"> if {@code divisor==0} </exception>
        ''' <seealso cref=    #divideToIntegralValue(java.math.BigDecimal, java.math.MathContext) </seealso>
        ''' <seealso cref=    #remainder(java.math.BigDecimal, java.math.MathContext)
        ''' @since  1.5 </seealso>
        Public Overridable Function divideAndRemainder(ByVal divisor As BigDecimal) As BigDecimal()
            ' we use the identity  x = i * y + r to determine r
            Dim result As BigDecimal() = New BigDecimal(1) {}

            result(0) = Me.divideToIntegralValue(divisor)
            result(1) = Me.subtract(result(0).multiply(divisor))
            Return result
        End Function

        ''' <summary>
        ''' Returns a two-element {@code BigDecimal} array containing the
        ''' result of {@code divideToIntegralValue} followed by the result of
        ''' {@code remainder} on the two operands calculated with rounding
        ''' according to the context settings.
        ''' 
        ''' <p>Note that if both the integer quotient and remainder are
        ''' needed, this method is faster than using the
        ''' {@code divideToIntegralValue} and {@code remainder} methods
        ''' separately because the division need only be carried out once.
        ''' </summary>
        ''' <param name="divisor"> value by which this {@code BigDecimal} is to be divided,
        '''         and the remainder computed. </param>
        ''' <param name="mc"> the context to use. </param>
        ''' <returns> a two element {@code BigDecimal} array: the quotient
        '''         (the result of {@code divideToIntegralValue}) is the
        '''         initial element and the remainder is the final element. </returns>
        ''' <exception cref="ArithmeticException"> if {@code divisor==0} </exception>
        ''' <exception cref="ArithmeticException"> if the result is inexact but the
        '''         rounding mode is {@code UNNECESSARY}, or {@code mc.precision}
        '''         {@literal >} 0 and the result of {@code this.divideToIntgralValue(divisor)} would
        '''         require a precision of more than {@code mc.precision} digits. </exception>
        ''' <seealso cref=    #divideToIntegralValue(java.math.BigDecimal, java.math.MathContext) </seealso>
        ''' <seealso cref=    #remainder(java.math.BigDecimal, java.math.MathContext)
        ''' @since  1.5 </seealso>
        Public Overridable Function divideAndRemainder(ByVal divisor As BigDecimal, ByVal mc As MathContext) As BigDecimal()
            If mc.precision = 0 Then Return divideAndRemainder(divisor)

            Dim result As BigDecimal() = New BigDecimal(1) {}
            Dim lhs As BigDecimal = Me

            result(0) = lhs.divideToIntegralValue(divisor, mc)
            result(1) = lhs.subtract(result(0).multiply(divisor))
            Return result
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is
        ''' <tt>(this<sup>n</sup>)</tt>, The power is computed exactly, to
        ''' unlimited precision.
        ''' 
        ''' <p>The parameter {@code n} must be in the range 0 through
        ''' 999999999, inclusive.  {@code ZERO.pow(0)} returns {@link
        ''' #ONE}.
        ''' 
        ''' Note that future releases may expand the allowable exponent
        ''' range of this method.
        ''' </summary>
        ''' <param name="n"> power to raise this {@code BigDecimal} to. </param>
        ''' <returns> <tt>this<sup>n</sup></tt> </returns>
        ''' <exception cref="ArithmeticException"> if {@code n} is out of range.
        ''' @since  1.5 </exception>
        Public Overridable Function pow(ByVal n As Integer) As BigDecimal
            If n < 0 OrElse n > 999999999 Then Throw New ArithmeticException("Invalid operation")
            ' No need to calculate pow(n) if result will over/underflow.
            ' Don't attempt to support "supernormal" numbers.
            Dim newScale As Integer = checkScale(CLng(scale_Renamed) * n)
            Return New BigDecimal(Me.inflated().pow(n), newScale)
        End Function


        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is
        ''' <tt>(this<sup>n</sup>)</tt>.  The current implementation uses
        ''' the core algorithm defined in ANSI standard X3.274-1996 with
        ''' rounding according to the context settings.  In general, the
        ''' returned numerical value is within two ulps of the exact
        ''' numerical value for the chosen precision.  Note that future
        ''' releases may use a different algorithm with a decreased
        ''' allowable error bound and increased allowable exponent range.
        ''' 
        ''' <p>The X3.274-1996 algorithm is:
        ''' 
        ''' <ul>
        ''' <li> An {@code ArithmeticException} exception is thrown if
        '''  <ul>
        '''    <li>{@code abs(n) > 999999999}
        '''    <li>{@code mc.precision == 0} and {@code n < 0}
        '''    <li>{@code mc.precision > 0} and {@code n} has more than
        '''    {@code mc.precision} decimal digits
        '''  </ul>
        ''' 
        ''' <li> if {@code n} is zero, <seealso cref="#ONE"/> is returned even if
        ''' {@code this} is zero, otherwise
        ''' <ul>
        '''   <li> if {@code n} is positive, the result is calculated via
        '''   the repeated squaring technique into a single accumulator.
        '''   The individual multiplications with the accumulator use the
        '''   same math context settings as in {@code mc} except for a
        '''   precision increased to {@code mc.precision + elength + 1}
        '''   where {@code elength} is the number of decimal digits in
        '''   {@code n}.
        ''' 
        '''   <li> if {@code n} is negative, the result is calculated as if
        '''   {@code n} were positive; this value is then divided into one
        '''   using the working precision specified above.
        ''' 
        '''   <li> The final value from either the positive or negative case
        '''   is then rounded to the destination precision.
        '''   </ul>
        ''' </ul>
        ''' </summary>
        ''' <param name="n"> power to raise this {@code BigDecimal} to. </param>
        ''' <param name="mc"> the context to use. </param>
        ''' <returns> <tt>this<sup>n</sup></tt> using the ANSI standard X3.274-1996
        '''         algorithm </returns>
        ''' <exception cref="ArithmeticException"> if the result is inexact but the
        '''         rounding mode is {@code UNNECESSARY}, or {@code n} is out
        '''         of range.
        ''' @since  1.5 </exception>
        Public Overridable Function pow(ByVal n As Integer, ByVal mc As MathContext) As BigDecimal
            If mc.precision = 0 Then Return pow(n)
            If n < -999999999 OrElse n > 999999999 Then Throw New ArithmeticException("Invalid operation")
            If n = 0 Then Return ONE ' x**0 == 1 in X3.274
            Dim lhs As BigDecimal = Me
            Dim workmc As MathContext = mc ' working settings
            Dim mag As Integer = System.Math.Abs(n) ' magnitude of n
            If mc.precision > 0 Then
                Dim elength As Integer = longDigitLength(mag) ' length of n in digits
                If elength > mc.precision Then ' X3.274 rule Throw New ArithmeticException("Invalid operation")
                    workmc = New MathContext(mc.precision + elength + 1, mc.roundingMode)
                End If
                ' ready to carry out power calculation...
                Dim acc As BigDecimal = ONE ' accumulator
                Dim seenbit As Boolean = False ' set once we've seen a 1-bit
                Dim i As Integer = 1
                Do ' for each bit [top bit ignored]
                    mag += mag ' shift left 1 bit
                    If mag < 0 Then ' top bit is set
                        seenbit = True ' OK, we're off
                        acc = acc.multiply(lhs, workmc) ' acc=acc*x
                    End If
                    If i = 31 Then Exit Do ' that was the last bit
                    If seenbit Then acc = acc.multiply(acc, workmc) ' acc=acc*acc [square]
                    ' else (!seenbit) no point in squaring ONE
                    i += 1
                Loop
                ' if negative n, calculate the reciprocal using working precision
                If n < 0 Then ' [hence mc.precision>0] acc=ONE.divide(acc, workmc)
                    ' round to final precision and strip zeros
                    Return doRound(acc, mc)
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is the absolute value
        ''' of this {@code BigDecimal}, and whose scale is
        ''' {@code this.scale()}.
        ''' </summary>
        ''' <returns> {@code abs(this)} </returns>
        Public Overridable Function abs() As BigDecimal
            Return (If(signum() < 0, negate(), Me))
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is the absolute value
        ''' of this {@code BigDecimal}, with rounding according to the
        ''' context settings.
        ''' </summary>
        ''' <param name="mc"> the context to use. </param>
        ''' <returns> {@code abs(this)}, rounded as necessary. </returns>
        ''' <exception cref="ArithmeticException"> if the result is inexact but the
        '''         rounding mode is {@code UNNECESSARY}.
        ''' @since 1.5 </exception>
        Public Overridable Function abs(ByVal mc As MathContext) As BigDecimal
            Return (If(signum() < 0, negate(mc), plus(mc)))
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is {@code (-this)},
        ''' and whose scale is {@code this.scale()}.
        ''' </summary>
        ''' <returns> {@code -this}. </returns>
        Public Overridable Function negate() As BigDecimal
            If intCompact = INFLATED_Renamed Then
                Return New BigDecimal(intVal.negate(), INFLATED_Renamed, scale_Renamed, precision_Renamed)
            Else
                Return valueOf(-intCompact, scale_Renamed, precision_Renamed)
            End If
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is {@code (-this)},
        ''' with rounding according to the context settings.
        ''' </summary>
        ''' <param name="mc"> the context to use. </param>
        ''' <returns> {@code -this}, rounded as necessary. </returns>
        ''' <exception cref="ArithmeticException"> if the result is inexact but the
        '''         rounding mode is {@code UNNECESSARY}.
        ''' @since  1.5 </exception>
        Public Overridable Function negate(ByVal mc As MathContext) As BigDecimal
            Return negate().plus(mc)
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is {@code (+this)}, and whose
        ''' scale is {@code this.scale()}.
        ''' 
        ''' <p>This method, which simply returns this {@code BigDecimal}
        ''' is included for symmetry with the unary minus method {@link
        ''' #negate()}.
        ''' </summary>
        ''' <returns> {@code this}. </returns>
        ''' <seealso cref= #negate()
        ''' @since  1.5 </seealso>
        Public Overridable Function plus() As BigDecimal
            Return Me
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is {@code (+this)},
        ''' with rounding according to the context settings.
        ''' 
        ''' <p>The effect of this method is identical to that of the {@link
        ''' #round(MathContext)} method.
        ''' </summary>
        ''' <param name="mc"> the context to use. </param>
        ''' <returns> {@code this}, rounded as necessary.  A zero result will
        '''         have a scale of 0. </returns>
        ''' <exception cref="ArithmeticException"> if the result is inexact but the
        '''         rounding mode is {@code UNNECESSARY}. </exception>
        ''' <seealso cref=    #round(MathContext)
        ''' @since  1.5 </seealso>
        Public Overridable Function plus(ByVal mc As MathContext) As BigDecimal
            If mc.precision = 0 Then ' no rounding please Return Me
                Return doRound(Me, mc)
        End Function

        ''' <summary>
        ''' Returns the signum function of this {@code BigDecimal}.
        ''' </summary>
        ''' <returns> -1, 0, or 1 as the value of this {@code BigDecimal}
        '''         is negative, zero, or positive. </returns>
        Public Overridable Function signum() As Integer
            Return If(intCompact <> INFLATED_Renamed, java.lang.[Long].signum(intCompact), intVal.signum())
        End Function

        ''' <summary>
        ''' Returns the <i>scale</i> of this {@code BigDecimal}.  If zero
        ''' or positive, the scale is the number of digits to the right of
        ''' the decimal point.  If negative, the unscaled value of the
        ''' number is multiplied by ten to the power of the negation of the
        ''' scale.  For example, a scale of {@code -3} means the unscaled
        ''' value is multiplied by 1000.
        ''' </summary>
        ''' <returns> the scale of this {@code BigDecimal}. </returns>
        Public Overridable Function scale() As Integer
            Return scale_Renamed
        End Function

        ''' <summary>
        ''' Returns the <i>precision</i> of this {@code BigDecimal}.  (The
        ''' precision is the number of digits in the unscaled value.)
        ''' 
        ''' <p>The precision of a zero value is 1.
        ''' </summary>
        ''' <returns> the precision of this {@code BigDecimal}.
        ''' @since  1.5 </returns>
        Public Overridable Function precision() As Integer
            Dim result As Integer = precision_Renamed
            If result = 0 Then
                Dim s As Long = intCompact
                If s <> INFLATED_Renamed Then
                    result = longDigitLength(s)
                Else
                    result = bigDigitLength(intVal)
                End If
                precision_Renamed = result
            End If
            Return result
        End Function


        ''' <summary>
        ''' Returns a {@code BigInteger} whose value is the <i>unscaled
        ''' value</i> of this {@code BigDecimal}.  (Computes <tt>(this *
        ''' 10<sup>this.scale()</sup>)</tt>.)
        ''' </summary>
        ''' <returns> the unscaled value of this {@code BigDecimal}.
        ''' @since  1.2 </returns>
        Public Overridable Function unscaledValue() As BigInteger
            Return Me.inflated()
        End Function

        ' Rounding Modes

        ''' <summary>
        ''' Rounding mode to round away from zero.  Always increments the
        ''' digit prior to a nonzero discarded fraction.  Note that this rounding
        ''' mode never decreases the magnitude of the calculated value.
        ''' </summary>
        Public Const ROUND_UP As Integer = 0

        ''' <summary>
        ''' Rounding mode to round towards zero.  Never increments the digit
        ''' prior to a discarded fraction (i.e., truncates).  Note that this
        ''' rounding mode never increases the magnitude of the calculated value.
        ''' </summary>
        Public Const ROUND_DOWN As Integer = 1

        ''' <summary>
        ''' Rounding mode to round towards positive infinity.  If the
        ''' {@code BigDecimal} is positive, behaves as for
        ''' {@code ROUND_UP}; if negative, behaves as for
        ''' {@code ROUND_DOWN}.  Note that this rounding mode never
        ''' decreases the calculated value.
        ''' </summary>
        Public Const ROUND_CEILING As Integer = 2

        ''' <summary>
        ''' Rounding mode to round towards negative infinity.  If the
        ''' {@code BigDecimal} is positive, behave as for
        ''' {@code ROUND_DOWN}; if negative, behave as for
        ''' {@code ROUND_UP}.  Note that this rounding mode never
        ''' increases the calculated value.
        ''' </summary>
        Public Const ROUND_FLOOR As Integer = 3

        ''' <summary>
        ''' Rounding mode to round towards {@literal "nearest neighbor"}
        ''' unless both neighbors are equidistant, in which case round up.
        ''' Behaves as for {@code ROUND_UP} if the discarded fraction is
        ''' &ge; 0.5; otherwise, behaves as for {@code ROUND_DOWN}.  Note
        ''' that this is the rounding mode that most of us were taught in
        ''' grade school.
        ''' </summary>
        Public Const ROUND_HALF_UP As Integer = 4

        ''' <summary>
        ''' Rounding mode to round towards {@literal "nearest neighbor"}
        ''' unless both neighbors are equidistant, in which case round
        ''' down.  Behaves as for {@code ROUND_UP} if the discarded
        ''' fraction is {@literal >} 0.5; otherwise, behaves as for
        ''' {@code ROUND_DOWN}.
        ''' </summary>
        Public Const ROUND_HALF_DOWN As Integer = 5

        ''' <summary>
        ''' Rounding mode to round towards the {@literal "nearest neighbor"}
        ''' unless both neighbors are equidistant, in which case, round
        ''' towards the even neighbor.  Behaves as for
        ''' {@code ROUND_HALF_UP} if the digit to the left of the
        ''' discarded fraction is odd; behaves as for
        ''' {@code ROUND_HALF_DOWN} if it's even.  Note that this is the
        ''' rounding mode that minimizes cumulative error when applied
        ''' repeatedly over a sequence of calculations.
        ''' </summary>
        Public Const ROUND_HALF_EVEN As Integer = 6

        ''' <summary>
        ''' Rounding mode to assert that the requested operation has an exact
        ''' result, hence no rounding is necessary.  If this rounding mode is
        ''' specified on an operation that yields an inexact result, an
        ''' {@code ArithmeticException} is thrown.
        ''' </summary>
        Public Const ROUND_UNNECESSARY As Integer = 7


        ' Scaling/Rounding Operations

        ''' <summary>
        ''' Returns a {@code BigDecimal} rounded according to the
        ''' {@code MathContext} settings.  If the precision setting is 0 then
        ''' no rounding takes place.
        ''' 
        ''' <p>The effect of this method is identical to that of the
        ''' <seealso cref="#plus(MathContext)"/> method.
        ''' </summary>
        ''' <param name="mc"> the context to use. </param>
        ''' <returns> a {@code BigDecimal} rounded according to the
        '''         {@code MathContext} settings. </returns>
        ''' <exception cref="ArithmeticException"> if the rounding mode is
        '''         {@code UNNECESSARY} and the
        '''         {@code BigDecimal}  operation would require rounding. </exception>
        ''' <seealso cref=    #plus(MathContext)
        ''' @since  1.5 </seealso>
        Public Overridable Function round(ByVal mc As MathContext) As BigDecimal
            Return plus(mc)
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose scale is the specified
        ''' value, and whose unscaled value is determined by multiplying or
        ''' dividing this {@code BigDecimal}'s unscaled value by the
        ''' appropriate power of ten to maintain its overall value.  If the
        ''' scale is reduced by the operation, the unscaled value must be
        ''' divided (rather than multiplied), and the value may be changed;
        ''' in this case, the specified rounding mode is applied to the
        ''' division.
        ''' 
        ''' <p>Note that since BigDecimal objects are immutable, calls of
        ''' this method do <i>not</i> result in the original object being
        ''' modified, contrary to the usual convention of having methods
        ''' named <tt>set<i>X</i></tt> mutate field <i>{@code X}</i>.
        ''' Instead, {@code setScale} returns an object with the proper
        ''' scale; the returned object may or may not be newly allocated.
        ''' </summary>
        ''' <param name="newScale"> scale of the {@code BigDecimal} value to be returned. </param>
        ''' <param name="roundingMode"> The rounding mode to apply. </param>
        ''' <returns> a {@code BigDecimal} whose scale is the specified value,
        '''         and whose unscaled value is determined by multiplying or
        '''         dividing this {@code BigDecimal}'s unscaled value by the
        '''         appropriate power of ten to maintain its overall value. </returns>
        ''' <exception cref="ArithmeticException"> if {@code roundingMode==UNNECESSARY}
        '''         and the specified scaling operation would require
        '''         rounding. </exception>
        ''' <seealso cref=    RoundingMode
        ''' @since  1.5 </seealso>
        Public Overridable Function setScale(ByVal newScale As Integer, ByVal roundingMode As RoundingMode) As BigDecimal
            Return scaleale(newScale, roundingMode.oldMode)
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose scale is the specified
        ''' value, and whose unscaled value is determined by multiplying or
        ''' dividing this {@code BigDecimal}'s unscaled value by the
        ''' appropriate power of ten to maintain its overall value.  If the
        ''' scale is reduced by the operation, the unscaled value must be
        ''' divided (rather than multiplied), and the value may be changed;
        ''' in this case, the specified rounding mode is applied to the
        ''' division.
        ''' 
        ''' <p>Note that since BigDecimal objects are immutable, calls of
        ''' this method do <i>not</i> result in the original object being
        ''' modified, contrary to the usual convention of having methods
        ''' named <tt>set<i>X</i></tt> mutate field <i>{@code X}</i>.
        ''' Instead, {@code setScale} returns an object with the proper
        ''' scale; the returned object may or may not be newly allocated.
        ''' 
        ''' <p>The new <seealso cref="#setScale(int, RoundingMode)"/> method should
        ''' be used in preference to this legacy method.
        ''' </summary>
        ''' <param name="newScale"> scale of the {@code BigDecimal} value to be returned. </param>
        ''' <param name="roundingMode"> The rounding mode to apply. </param>
        ''' <returns> a {@code BigDecimal} whose scale is the specified value,
        '''         and whose unscaled value is determined by multiplying or
        '''         dividing this {@code BigDecimal}'s unscaled value by the
        '''         appropriate power of ten to maintain its overall value. </returns>
        ''' <exception cref="ArithmeticException"> if {@code roundingMode==ROUND_UNNECESSARY}
        '''         and the specified scaling operation would require
        '''         rounding. </exception>
        ''' <exception cref="IllegalArgumentException"> if {@code roundingMode} does not
        '''         represent a valid rounding mode. </exception>
        ''' <seealso cref=    #ROUND_UP </seealso>
        ''' <seealso cref=    #ROUND_DOWN </seealso>
        ''' <seealso cref=    #ROUND_CEILING </seealso>
        ''' <seealso cref=    #ROUND_FLOOR </seealso>
        ''' <seealso cref=    #ROUND_HALF_UP </seealso>
        ''' <seealso cref=    #ROUND_HALF_DOWN </seealso>
        ''' <seealso cref=    #ROUND_HALF_EVEN </seealso>
        ''' <seealso cref=    #ROUND_UNNECESSARY </seealso>
        Public Overridable Function setScale(ByVal newScale As Integer, ByVal roundingMode As Integer) As BigDecimal
            If roundingMode < ROUND_UP OrElse roundingMode > ROUND_UNNECESSARY Then Throw New IllegalArgumentException("Invalid rounding mode")

            Dim oldScale As Integer = Me.scale_Renamed
            If newScale = oldScale Then ' easy case Return Me
                If Me.signum() = 0 Then ' zero can have any scale Return zeroValueOf(newScale)
                    If Me.intCompact <> INFLATED_Renamed Then
                        Dim rs As Long = Me.intCompact
                        If newScale > oldScale Then
                            Dim raise As Integer = checkScale(CLng(newScale) - oldScale)
                            rs = longMultiplyPowerTen(rs, raise)
                            If rs <> INFLATED_Renamed Then Return valueOf(rs, newScale)
                            Dim rb As BigInteger = bigMultiplyPowerTen(raise)
                            Return New BigDecimal(rb, INFLATED_Renamed, newScale, If(precision_Renamed > 0, precision_Renamed + raise, 0))
                        Else
                            ' newScale < oldScale -- drop some digits
                            ' Can't predict the precision due to the effect of rounding.
                            Dim drop As Integer = checkScale(CLng(oldScale) - newScale)
                            If drop < LONG_TEN_POWERS_TABLE.Length Then
                                Return divideAndRound(rs, LONG_TEN_POWERS_TABLE(drop), newScale, roundingMode, newScale)
                            Else
                                Return divideAndRound(Me.inflated(), bigTenToThe(drop), newScale, roundingMode, newScale)
                            End If
                        End If
                    Else
                        If newScale > oldScale Then
                            Dim raise As Integer = checkScale(CLng(newScale) - oldScale)
                            Dim rb As BigInteger = bigMultiplyPowerTen(Me.intVal, raise)
                            Return New BigDecimal(rb, INFLATED_Renamed, newScale, If(precision_Renamed > 0, precision_Renamed + raise, 0))
                        Else
                            ' newScale < oldScale -- drop some digits
                            ' Can't predict the precision due to the effect of rounding.
                            Dim drop As Integer = checkScale(CLng(oldScale) - newScale)
                            If drop < LONG_TEN_POWERS_TABLE.Length Then
                                Return divideAndRound(Me.intVal, LONG_TEN_POWERS_TABLE(drop), newScale, roundingMode, newScale)
                            Else
                                Return divideAndRound(Me.intVal, bigTenToThe(drop), newScale, roundingMode, newScale)
                            End If
                        End If
                    End If
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose scale is the specified
        ''' value, and whose value is numerically equal to this
        ''' {@code BigDecimal}'s.  Throws an {@code ArithmeticException}
        ''' if this is not possible.
        ''' 
        ''' <p>This call is typically used to increase the scale, in which
        ''' case it is guaranteed that there exists a {@code BigDecimal}
        ''' of the specified scale and the correct value.  The call can
        ''' also be used to reduce the scale if the caller knows that the
        ''' {@code BigDecimal} has sufficiently many zeros at the end of
        ''' its fractional part (i.e., factors of ten in its integer value)
        ''' to allow for the rescaling without changing its value.
        ''' 
        ''' <p>This method returns the same result as the two-argument
        ''' versions of {@code setScale}, but saves the caller the trouble
        ''' of specifying a rounding mode in cases where it is irrelevant.
        ''' 
        ''' <p>Note that since {@code BigDecimal} objects are immutable,
        ''' calls of this method do <i>not</i> result in the original
        ''' object being modified, contrary to the usual convention of
        ''' having methods named <tt>set<i>X</i></tt> mutate field
        ''' <i>{@code X}</i>.  Instead, {@code setScale} returns an
        ''' object with the proper scale; the returned object may or may
        ''' not be newly allocated.
        ''' </summary>
        ''' <param name="newScale"> scale of the {@code BigDecimal} value to be returned. </param>
        ''' <returns> a {@code BigDecimal} whose scale is the specified value, and
        '''         whose unscaled value is determined by multiplying or dividing
        '''         this {@code BigDecimal}'s unscaled value by the appropriate
        '''         power of ten to maintain its overall value. </returns>
        ''' <exception cref="ArithmeticException"> if the specified scaling operation would
        '''         require rounding. </exception>
        ''' <seealso cref=    #setScale(int, int) </seealso>
        ''' <seealso cref=    #setScale(int, RoundingMode) </seealso>
        Public Overridable Function setScale(ByVal newScale As Integer) As BigDecimal
            Return scaleale(newScale, ROUND_UNNECESSARY)
        End Function

        ' Decimal Point Motion Operations

        ''' <summary>
        ''' Returns a {@code BigDecimal} which is equivalent to this one
        ''' with the decimal point moved {@code n} places to the left.  If
        ''' {@code n} is non-negative, the call merely adds {@code n} to
        ''' the scale.  If {@code n} is negative, the call is equivalent
        ''' to {@code movePointRight(-n)}.  The {@code BigDecimal}
        ''' returned by this call has value <tt>(this &times;
        ''' 10<sup>-n</sup>)</tt> and scale {@code max(this.scale()+n,
        ''' 0)}.
        ''' </summary>
        ''' <param name="n"> number of places to move the decimal point to the left. </param>
        ''' <returns> a {@code BigDecimal} which is equivalent to this one with the
        '''         decimal point moved {@code n} places to the left. </returns>
        ''' <exception cref="ArithmeticException"> if scale overflows. </exception>
        Public Overridable Function movePointLeft(ByVal n As Integer) As BigDecimal
            ' Cannot use movePointRight(-n) in case of n== java.lang.[Integer].MIN_VALUE
            Dim newScale As Integer = checkScale(CLng(scale_Renamed) + n)
            Dim num As New BigDecimal(intVal, intCompact, newScale, 0)
            Return If(num.scale_Renamed < 0, num.scaleale(0, ROUND_UNNECESSARY), num)
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} which is equivalent to this one
        ''' with the decimal point moved {@code n} places to the right.
        ''' If {@code n} is non-negative, the call merely subtracts
        ''' {@code n} from the scale.  If {@code n} is negative, the call
        ''' is equivalent to {@code movePointLeft(-n)}.  The
        ''' {@code BigDecimal} returned by this call has value <tt>(this
        ''' &times; 10<sup>n</sup>)</tt> and scale {@code max(this.scale()-n,
        ''' 0)}.
        ''' </summary>
        ''' <param name="n"> number of places to move the decimal point to the right. </param>
        ''' <returns> a {@code BigDecimal} which is equivalent to this one
        '''         with the decimal point moved {@code n} places to the right. </returns>
        ''' <exception cref="ArithmeticException"> if scale overflows. </exception>
        Public Overridable Function movePointRight(ByVal n As Integer) As BigDecimal
            ' Cannot use movePointLeft(-n) in case of n== java.lang.[Integer].MIN_VALUE
            Dim newScale As Integer = checkScale(CLng(scale_Renamed) - n)
            Dim num As New BigDecimal(intVal, intCompact, newScale, 0)
            Return If(num.scale_Renamed < 0, num.scaleale(0, ROUND_UNNECESSARY), num)
        End Function

        ''' <summary>
        ''' Returns a BigDecimal whose numerical value is equal to
        ''' ({@code this} * 10<sup>n</sup>).  The scale of
        ''' the result is {@code (this.scale() - n)}.
        ''' </summary>
        ''' <param name="n"> the exponent power of ten to scale by </param>
        ''' <returns> a BigDecimal whose numerical value is equal to
        ''' ({@code this} * 10<sup>n</sup>) </returns>
        ''' <exception cref="ArithmeticException"> if the scale would be
        '''         outside the range of a 32-bit  java.lang.[Integer].
        ''' 
        ''' @since 1.5 </exception>
        Public Overridable Function scaleByPowerOfTen(ByVal n As Integer) As BigDecimal
            Return New BigDecimal(intVal, intCompact, checkScale(CLng(scale_Renamed) - n), precision_Renamed)
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} which is numerically equal to
        ''' this one but with any trailing zeros removed from the
        ''' representation.  For example, stripping the trailing zeros from
        ''' the {@code BigDecimal} value {@code 600.0}, which has
        ''' [{@code BigInteger}, {@code scale}] components equals to
        ''' [6000, 1], yields {@code 6E2} with [{@code BigInteger},
        ''' {@code scale}] components equals to [6, -2].  If
        ''' this BigDecimal is numerically equal to zero, then
        ''' {@code BigDecimal.ZERO} is returned.
        ''' </summary>
        ''' <returns> a numerically equal {@code BigDecimal} with any
        ''' trailing zeros removed.
        ''' @since 1.5 </returns>
        Public Overridable Function stripTrailingZeros() As BigDecimal
            If intCompact = 0 OrElse (intVal IsNot Nothing AndAlso intVal.signum() = 0) Then
                Return Decimal.Zero
            ElseIf intCompact <> INFLATED_Renamed Then
                Return createAndStripZerosToMatchScale(intCompact, scale_Renamed, java.lang.[Long].MIN_VALUE)
            Else
                Return createAndStripZerosToMatchScale(intVal, scale_Renamed, java.lang.[Long].MIN_VALUE)
            End If
        End Function

        ' Comparison Operations

        ''' <summary>
        ''' Compares this {@code BigDecimal} with the specified
        ''' {@code BigDecimal}.  Two {@code BigDecimal} objects that are
        ''' equal in value but have a different scale (like 2.0 and 2.00)
        ''' are considered equal by this method.  This method is provided
        ''' in preference to individual methods for each of the six boolean
        ''' comparison operators ({@literal <}, ==,
        ''' {@literal >}, {@literal >=}, !=, {@literal <=}).  The
        ''' suggested idiom for performing these comparisons is:
        ''' {@code (x.compareTo(y)} &lt;<i>op</i>&gt; {@code 0)}, where
        ''' &lt;<i>op</i>&gt; is one of the six comparison operators.
        ''' </summary>
        ''' <param name="val"> {@code BigDecimal} to which this {@code BigDecimal} is
        '''         to be compared. </param>
        ''' <returns> -1, 0, or 1 as this {@code BigDecimal} is numerically
        '''          less than, equal to, or greater than {@code val}. </returns>
        Public Overridable Function compareTo(ByVal val As BigDecimal) As Integer Implements Comparable(Of BigDecimal).compareTo
            ' Quick path for equal scale and non-inflated case.
            If scale_Renamed = val.scale_Renamed Then
                Dim xs As Long = intCompact
                Dim ys As Long = val.intCompact
                If xs <> INFLATED_Renamed AndAlso ys <> INFLATED_Renamed Then Return If(xs <> ys, (If(xs > ys, 1, -1)), 0)
            End If
            Dim xsign As Integer = Me.signum()
            Dim ysign As Integer = val.signum()
            If xsign <> ysign Then Return If(xsign > ysign, 1, -1)
            If xsign = 0 Then Return 0
            Dim cmp As Integer = compareMagnitude(val)
            Return If(xsign > 0, cmp, -cmp)
        End Function

        ''' <summary>
        ''' Version of compareTo that ignores sign.
        ''' </summary>
        Private Function compareMagnitude(ByVal val As BigDecimal) As Integer
            ' Match scales, avoid unnecessary inflation
            Dim ys As Long = val.intCompact
            Dim xs As Long = Me.intCompact
            If xs = 0 Then Return If(ys = 0, 0, -1)
            If ys = 0 Then Return 1

            Dim sdiff As Long = CLng(Me.scale_Renamed) - val.scale_Renamed
            If sdiff <> 0 Then
                ' Avoid matching scales if the (adjusted) exponents differ
                Dim xae As Long = CLng(Me.precision()) - Me.scale_Renamed ' [-1]
                Dim yae As Long = CLng(val.precision()) - val.scale_Renamed ' [-1]
                If xae < yae Then Return -1
                If xae > yae Then Return 1
                Dim rb As BigInteger = Nothing
                If sdiff < 0 Then
                    ' The cases sdiff <=  java.lang.[Integer].MIN_VALUE intentionally fall through.
                    xs = longMultiplyPowerTen(xs, CInt(Fix(-sdiff)))
                    If sdiff >  java.lang.[Integer].MIN_VALUE AndAlso (xs = INFLATED_Renamed OrElse xs = INFLATED_Renamed) AndAlso ys = INFLATED_Renamed Then
                        rb = bigMultiplyPowerTen(CInt(Fix(-sdiff)))
                        Return rb.compareMagnitude(val.intVal)
                    End If ' sdiff > 0
                Else
                    ' The cases sdiff >  java.lang.[Integer].MAX_VALUE intentionally fall through.
                    ys = longMultiplyPowerTen(ys, CInt(sdiff))
                    If sdiff <=  java.lang.[Integer].Max_Value AndAlso (ys = INFLATED_Renamed OrElse ys = INFLATED_Renamed) AndAlso xs = INFLATED_Renamed Then
                        rb = val.bigMultiplyPowerTen(CInt(sdiff))
                        Return Me.intVal.compareMagnitude(rb)
                    End If
                End If
            End If
            If xs <> INFLATED_Renamed Then
                Return If(ys <> INFLATED_Renamed, longCompareMagnitude(xs, ys), -1)
            ElseIf ys <> INFLATED_Renamed Then
                Return 1
            Else
                Return Me.intVal.compareMagnitude(val.intVal)
            End If
        End Function

        ''' <summary>
        ''' Compares this {@code BigDecimal} with the specified
        ''' {@code Object} for equality.  Unlike {@link
        ''' #compareTo(BigDecimal) compareTo}, this method considers two
        ''' {@code BigDecimal} objects equal only if they are equal in
        ''' value and scale (thus 2.0 is not equal to 2.00 when compared by
        ''' this method).
        ''' </summary>
        ''' <param name="x"> {@code Object} to which this {@code BigDecimal} is
        '''         to be compared. </param>
        ''' <returns> {@code true} if and only if the specified {@code Object} is a
        '''         {@code BigDecimal} whose value and scale are equal to this
        '''         {@code BigDecimal}'s. </returns>
        ''' <seealso cref=    #compareTo(java.math.BigDecimal) </seealso>
        ''' <seealso cref=    #hashCode </seealso>
        Public Overrides Function Equals(ByVal x As Object) As Boolean
            If Not (TypeOf x Is BigDecimal) Then Return False
            Dim xDec As BigDecimal = CDec(x)
            If x Is Me Then Return True
            If scale_Renamed <> xDec.scale_Renamed Then Return False
            Dim s As Long = Me.intCompact
            Dim xs As Long = xDec.intCompact
            If s <> INFLATED_Renamed Then
                If xs = INFLATED_Renamed Then xs = compactValFor(xDec.intVal)
                Return xs = s
            ElseIf xs <> INFLATED_Renamed Then
                Return xs = compactValFor(Me.intVal)
            End If

            Return Me.inflated().Equals(xDec.inflated())
        End Function

        ''' <summary>
        ''' Returns the minimum of this {@code BigDecimal} and
        ''' {@code val}.
        ''' </summary>
        ''' <param name="val"> value with which the minimum is to be computed. </param>
        ''' <returns> the {@code BigDecimal} whose value is the lesser of this
        '''         {@code BigDecimal} and {@code val}.  If they are equal,
        '''         as defined by the <seealso cref="#compareTo(BigDecimal) compareTo"/>
        '''         method, {@code this} is returned. </returns>
        ''' <seealso cref=    #compareTo(java.math.BigDecimal) </seealso>
        Public Overridable Function min(ByVal val As BigDecimal) As BigDecimal
            Return (If(compareTo(val) <= 0, Me, val))
        End Function

        ''' <summary>
        ''' Returns the maximum of this {@code BigDecimal} and {@code val}.
        ''' </summary>
        ''' <param name="val"> value with which the maximum is to be computed. </param>
        ''' <returns> the {@code BigDecimal} whose value is the greater of this
        '''         {@code BigDecimal} and {@code val}.  If they are equal,
        '''         as defined by the <seealso cref="#compareTo(BigDecimal) compareTo"/>
        '''         method, {@code this} is returned. </returns>
        ''' <seealso cref=    #compareTo(java.math.BigDecimal) </seealso>
        Public Overridable Function max(ByVal val As BigDecimal) As BigDecimal
            Return (If(compareTo(val) >= 0, Me, val))
        End Function

        ' Hash Function

        ''' <summary>
        ''' Returns the hash code for this {@code BigDecimal}.  Note that
        ''' two {@code BigDecimal} objects that are numerically equal but
        ''' differ in scale (like 2.0 and 2.00) will generally <i>not</i>
        ''' have the same hash code.
        ''' </summary>
        ''' <returns> hash code for this {@code BigDecimal}. </returns>
        ''' <seealso cref= #equals(Object) </seealso>
        Public Overrides Function GetHashCode() As Integer
            If intCompact <> INFLATED_Renamed Then
                Dim val2 As Long = If(intCompact < 0, -intCompact, intCompact)
                Dim temp As Integer = CInt(Fix((CInt(CLng(CULng(val2) >> 32))) * 31 + (val2 And LONG_MASK)))
                Return 31 * (If(intCompact < 0, -temp, temp)) + scale_Renamed
            Else
                Return 31 * intVal.GetHashCode() + scale_Renamed
            End If
        End Function

        ' Format Converters

        ''' <summary>
        ''' Returns the string representation of this {@code BigDecimal},
        ''' using scientific notation if an exponent is needed.
        ''' 
        ''' <p>A standard canonical string form of the {@code BigDecimal}
        ''' is created as though by the following steps: first, the
        ''' absolute value of the unscaled value of the {@code BigDecimal}
        ''' is converted to a string in base ten using the characters
        ''' {@code '0'} through {@code '9'} with no leading zeros (except
        ''' if its value is zero, in which case a single {@code '0'}
        ''' character is used).
        ''' 
        ''' <p>Next, an <i>adjusted exponent</i> is calculated; this is the
        ''' negated scale, plus the number of characters in the converted
        ''' unscaled value, less one.  That is,
        ''' {@code -scale+(ulength-1)}, where {@code ulength} is the
        ''' length of the absolute value of the unscaled value in decimal
        ''' digits (its <i>precision</i>).
        ''' 
        ''' <p>If the scale is greater than or equal to zero and the
        ''' adjusted exponent is greater than or equal to {@code -6}, the
        ''' number will be converted to a character form without using
        ''' exponential notation.  In this case, if the scale is zero then
        ''' no decimal point is added and if the scale is positive a
        ''' decimal point will be inserted with the scale specifying the
        ''' number of characters to the right of the decimal point.
        ''' {@code '0'} characters are added to the left of the converted
        ''' unscaled value as necessary.  If no character precedes the
        ''' decimal point after this insertion then a conventional
        ''' {@code '0'} character is prefixed.
        ''' 
        ''' <p>Otherwise (that is, if the scale is negative, or the
        ''' adjusted exponent is less than {@code -6}), the number will be
        ''' converted to a character form using exponential notation.  In
        ''' this case, if the converted {@code BigInteger} has more than
        ''' one digit a decimal point is inserted after the first digit.
        ''' An exponent in character form is then suffixed to the converted
        ''' unscaled value (perhaps with inserted decimal point); this
        ''' comprises the letter {@code 'E'} followed immediately by the
        ''' adjusted exponent converted to a character form.  The latter is
        ''' in base ten, using the characters {@code '0'} through
        ''' {@code '9'} with no leading zeros, and is always prefixed by a
        ''' sign character {@code '-'} (<tt>'&#92;u002D'</tt>) if the
        ''' adjusted exponent is negative, {@code '+'}
        ''' (<tt>'&#92;u002B'</tt>) otherwise).
        ''' 
        ''' <p>Finally, the entire string is prefixed by a minus sign
        ''' character {@code '-'} (<tt>'&#92;u002D'</tt>) if the unscaled
        ''' value is less than zero.  No sign character is prefixed if the
        ''' unscaled value is zero or positive.
        ''' 
        ''' <p><b>Examples:</b>
        ''' <p>For each representation [<i>unscaled value</i>, <i>scale</i>]
        ''' on the left, the resulting string is shown on the right.
        ''' <pre>
        ''' [123,0]      "123"
        ''' [-123,0]     "-123"
        ''' [123,-1]     "1.23E+3"
        ''' [123,-3]     "1.23E+5"
        ''' [123,1]      "12.3"
        ''' [123,5]      "0.00123"
        ''' [123,10]     "1.23E-8"
        ''' [-123,12]    "-1.23E-10"
        ''' </pre>
        ''' 
        ''' <b>Notes:</b>
        ''' <ol>
        ''' 
        ''' <li>There is a one-to-one mapping between the distinguishable
        ''' {@code BigDecimal} values and the result of this conversion.
        ''' That is, every distinguishable {@code BigDecimal} value
        ''' (unscaled value and scale) has a unique string representation
        ''' as a result of using {@code toString}.  If that string
        ''' representation is converted back to a {@code BigDecimal} using
        ''' the <seealso cref="#BigDecimal(String)"/> constructor, then the original
        ''' value will be recovered.
        ''' 
        ''' <li>The string produced for a given number is always the same;
        ''' it is not affected by locale.  This means that it can be used
        ''' as a canonical string representation for exchanging decimal
        ''' data, or as a key for a Hashtable, etc.  Locale-sensitive
        ''' number formatting and parsing is handled by the {@link
        ''' java.text.NumberFormat} class and its subclasses.
        ''' 
        ''' <li>The <seealso cref="#toEngineeringString"/> method may be used for
        ''' presenting numbers with exponents in engineering notation, and the
        ''' <seealso cref="#setScale(int,RoundingMode) setScale"/> method may be used for
        ''' rounding a {@code BigDecimal} so it has a known number of digits after
        ''' the decimal point.
        ''' 
        ''' <li>The digit-to-character mapping provided by
        ''' {@code Character.forDigit} is used.
        ''' 
        ''' </ol>
        ''' </summary>
        ''' <returns> string representation of this {@code BigDecimal}. </returns>
        ''' <seealso cref=    Character#forDigit </seealso>
        ''' <seealso cref=    #BigDecimal(java.lang.String) </seealso>
        Public Overrides Function ToString() As String
            Dim sc As String = stringCache
            If sc Is Nothing Then
                sc = layoutChars(True)
                stringCache = sc
            End If
            Return sc
        End Function

        ''' <summary>
        ''' Returns a string representation of this {@code BigDecimal},
        ''' using engineering notation if an exponent is needed.
        ''' 
        ''' <p>Returns a string that represents the {@code BigDecimal} as
        ''' described in the <seealso cref="#toString()"/> method, except that if
        ''' exponential notation is used, the power of ten is adjusted to
        ''' be a multiple of three (engineering notation) such that the
        ''' integer part of nonzero values will be in the range 1 through
        ''' 999.  If exponential notation is used for zero values, a
        ''' decimal point and one or two fractional zero digits are used so
        ''' that the scale of the zero value is preserved.  Note that
        ''' unlike the output of <seealso cref="#toString()"/>, the output of this
        ''' method is <em>not</em> guaranteed to recover the same [integer,
        ''' scale] pair of this {@code BigDecimal} if the output string is
        ''' converting back to a {@code BigDecimal} using the {@linkplain
        ''' #BigDecimal(String) string constructor}.  The result of this method meets
        ''' the weaker constraint of always producing a numerically equal
        ''' result from applying the string constructor to the method's output.
        ''' </summary>
        ''' <returns> string representation of this {@code BigDecimal}, using
        '''         engineering notation if an exponent is needed.
        ''' @since  1.5 </returns>
        Public Overridable Function toEngineeringString() As String
            Return layoutChars(False)
        End Function

        ''' <summary>
        ''' Returns a string representation of this {@code BigDecimal}
        ''' without an exponent field.  For values with a positive scale,
        ''' the number of digits to the right of the decimal point is used
        ''' to indicate scale.  For values with a zero or negative scale,
        ''' the resulting string is generated as if the value were
        ''' converted to a numerically equal value with zero scale and as
        ''' if all the trailing zeros of the zero scale value were present
        ''' in the result.
        ''' 
        ''' The entire string is prefixed by a minus sign character '-'
        ''' (<tt>'&#92;u002D'</tt>) if the unscaled value is less than
        ''' zero. No sign character is prefixed if the unscaled value is
        ''' zero or positive.
        ''' 
        ''' Note that if the result of this method is passed to the
        ''' <seealso cref="#BigDecimal(String) string constructor"/>, only the
        ''' numerical value of this {@code BigDecimal} will necessarily be
        ''' recovered; the representation of the new {@code BigDecimal}
        ''' may have a different scale.  In particular, if this
        ''' {@code BigDecimal} has a negative scale, the string resulting
        ''' from this method will have a scale of zero when processed by
        ''' the string constructor.
        ''' 
        ''' (This method behaves analogously to the {@code toString}
        ''' method in 1.4 and earlier releases.)
        ''' </summary>
        ''' <returns> a string representation of this {@code BigDecimal}
        ''' without an exponent field.
        ''' @since 1.5 </returns>
        ''' <seealso cref= #toString() </seealso>
        ''' <seealso cref= #toEngineeringString() </seealso>
        Public Overridable Function toPlainString() As String
            If scale_Renamed = 0 Then
                If intCompact <> INFLATED_Renamed Then
                    Return Convert.ToString(intCompact)
                Else
                    Return intVal.ToString()
                End If
            End If
            If Me.scale_Renamed < 0 Then ' No decimal point
                If signum() = 0 Then Return "0"
                Dim tailingZeros As Integer = checkScaleNonZero((-CLng(scale_Renamed)))
                Dim buf As StringBuilder
                If intCompact <> INFLATED_Renamed Then
                    buf = New StringBuilder(20 + tailingZeros)
                    buf.append(intCompact)
                Else
                    Dim str As String = intVal.ToString()
                    buf = New StringBuilder(str.Length() + tailingZeros)
                    buf.append(str)
                End If
                For i As Integer = 0 To tailingZeros - 1
                    buf.append("0"c)
                Next i
                Return buf.ToString()
            End If
            Dim str As String
            If intCompact <> INFLATED_Renamed Then
                str = Convert.ToString (System.Math.Abs(intCompact))
            Else
                str = intVal.abs().ToString()
            End If
            Return getValueString(signum(), str, scale_Renamed)
        End Function

        ' Returns a digit.digit string 
        Private Function getValueString(ByVal signum As Integer, ByVal intString As String, ByVal scale As Integer) As String
            ' Insert decimal point 
            Dim buf As StringBuilder
            Dim insertionPoint As Integer = intString.Length() - scale
            If insertionPoint = 0 Then ' Point goes right before intVal
                Return (If(signum < 0, "-0.", "0.")) + intString ' Point goes inside intVal
            ElseIf insertionPoint > 0 Then
                buf = New StringBuilder(intString)
                buf.insert(insertionPoint, "."c)
                If signum < 0 Then
                    buf.insert(0, "-"c)
                End If ' We must insert zeros between point and intVal
            Else
                buf = New StringBuilder(3 - insertionPoint + intString.Length())
                buf.append(If(signum < 0, "-0.", "0."))
                For i As Integer = 0 To -insertionPoint - 1
                    buf.append("0"c)
                Next i
                buf.append(intString)
            End If
            Return buf.ToString()
        End Function

        ''' <summary>
        ''' Converts this {@code BigDecimal} to a {@code BigInteger}.
        ''' This conversion is analogous to the
        ''' <i>narrowing primitive conversion</i> from {@code double} to
        ''' {@code long} as defined in section 5.1.3 of
        ''' <cite>The Java&trade; Language Specification</cite>:
        ''' any fractional part of this
        ''' {@code BigDecimal} will be discarded.  Note that this
        ''' conversion can lose information about the precision of the
        ''' {@code BigDecimal} value.
        ''' <p>
        ''' To have an exception thrown if the conversion is inexact (in
        ''' other words if a nonzero fractional part is discarded), use the
        ''' <seealso cref="#toBigIntegerExact()"/> method.
        ''' </summary>
        ''' <returns> this {@code BigDecimal} converted to a {@code BigInteger}. </returns>
        Public Overridable Function toBigInteger() As BigInteger
            ' force to an integer, quietly
            Return Me.scaleale(0, ROUND_DOWN).inflated()
        End Function

        ''' <summary>
        ''' Converts this {@code BigDecimal} to a {@code BigInteger},
        ''' checking for lost information.  An exception is thrown if this
        ''' {@code BigDecimal} has a nonzero fractional part.
        ''' </summary>
        ''' <returns> this {@code BigDecimal} converted to a {@code BigInteger}. </returns>
        ''' <exception cref="ArithmeticException"> if {@code this} has a nonzero
        '''         fractional part.
        ''' @since  1.5 </exception>
        Public Overridable Function toBigIntegerExact() As BigInteger
            ' round to an integer, with Exception if decimal part non-0
            Return Me.scaleale(0, ROUND_UNNECESSARY).inflated()
        End Function

        ''' <summary>
        ''' Converts this {@code BigDecimal} to a {@code long}.
        ''' This conversion is analogous to the
        ''' <i>narrowing primitive conversion</i> from {@code double} to
        ''' {@code short} as defined in section 5.1.3 of
        ''' <cite>The Java&trade; Language Specification</cite>:
        ''' any fractional part of this
        ''' {@code BigDecimal} will be discarded, and if the resulting
        ''' "{@code BigInteger}" is too big to fit in a
        ''' {@code long}, only the low-order 64 bits are returned.
        ''' Note that this conversion can lose information about the
        ''' overall magnitude and precision of this {@code BigDecimal} value as well
        ''' as return a result with the opposite sign.
        ''' </summary>
        ''' <returns> this {@code BigDecimal} converted to a {@code long}. </returns>
        Public Overrides Function longValue() As Long
            Return If(intCompact <> INFLATED_Renamed AndAlso scale_Renamed = 0, intCompact, toBigInteger())
        End Function

        ''' <summary>
        ''' Converts this {@code BigDecimal} to a {@code long}, checking
        ''' for lost information.  If this {@code BigDecimal} has a
        ''' nonzero fractional part or is out of the possible range for a
        ''' {@code long} result then an {@code ArithmeticException} is
        ''' thrown.
        ''' </summary>
        ''' <returns> this {@code BigDecimal} converted to a {@code long}. </returns>
        ''' <exception cref="ArithmeticException"> if {@code this} has a nonzero
        '''         fractional part, or will not fit in a {@code long}.
        ''' @since  1.5 </exception>
        Public Overridable Function longValueExact() As Long
            If intCompact <> INFLATED_Renamed AndAlso scale_Renamed = 0 Then Return intCompact
            ' If more than 19 digits in integer part it cannot possibly fit
            If (precision() - scale_Renamed) > 19 Then ' [OK for negative scale too] Throw New java.lang.ArithmeticException("Overflow")
                ' Fastpath zero and < 1.0 numbers (the latter can be very slow
                ' to round if very small)
                If Me.signum() = 0 Then Return 0
                If (Me.precision() - Me.scale_Renamed) <= 0 Then Throw New ArithmeticException("Rounding necessary")
                ' round to an integer, with Exception if decimal part non-0
                Dim num As BigDecimal = Me.scaleale(0, ROUND_UNNECESSARY)
                If num.precision() >= 19 Then ' need to check carefully LongOverflow.check(num)
                    Return num.inflated()
        End Function

        Private Class LongOverflow
            ''' <summary>
            ''' BigInteger equal to java.lang.[Long].MIN_VALUE. </summary>
            Private Shared ReadOnly LONGMIN As BigInteger = Big java.lang.[Integer].valueOf(Long.MIN_VALUE)

            ''' <summary>
            ''' BigInteger equal to java.lang.[Long].MAX_VALUE. </summary>
            Private Shared ReadOnly LONGMAX As BigInteger = Big java.lang.[Integer].valueOf(Long.MAX_VALUE)

            Public Shared Sub check(ByVal num As BigDecimal)
                Dim intVal As BigInteger = num.inflated()
                If intVal.compareTo(LONGMIN) < 0 OrElse intVal.compareTo(LONGMAX) > 0 Then Throw New java.lang.ArithmeticException("Overflow")
            End Sub
        End Class

        ''' <summary>
        ''' Converts this {@code BigDecimal} to an {@code int}.
        ''' This conversion is analogous to the
        ''' <i>narrowing primitive conversion</i> from {@code double} to
        ''' {@code short} as defined in section 5.1.3 of
        ''' <cite>The Java&trade; Language Specification</cite>:
        ''' any fractional part of this
        ''' {@code BigDecimal} will be discarded, and if the resulting
        ''' "{@code BigInteger}" is too big to fit in an
        ''' {@code int}, only the low-order 32 bits are returned.
        ''' Note that this conversion can lose information about the
        ''' overall magnitude and precision of this {@code BigDecimal}
        ''' value as well as return a result with the opposite sign.
        ''' </summary>
        ''' <returns> this {@code BigDecimal} converted to an {@code int}. </returns>
        Public Overrides Function intValue() As Integer
            Return If(intCompact <> INFLATED_Renamed AndAlso scale_Renamed = 0, CInt(intCompact), toBigInteger())
        End Function

        ''' <summary>
        ''' Converts this {@code BigDecimal} to an {@code int}, checking
        ''' for lost information.  If this {@code BigDecimal} has a
        ''' nonzero fractional part or is out of the possible range for an
        ''' {@code int} result then an {@code ArithmeticException} is
        ''' thrown.
        ''' </summary>
        ''' <returns> this {@code BigDecimal} converted to an {@code int}. </returns>
        ''' <exception cref="ArithmeticException"> if {@code this} has a nonzero
        '''         fractional part, or will not fit in an {@code int}.
        ''' @since  1.5 </exception>
        Public Overridable Function intValueExact() As Integer
            Dim num As Long
            num = Me.longValueExact() ' will check decimal part
            If CInt(num) <> num Then Throw New java.lang.ArithmeticException("Overflow")
            Return CInt(num)
        End Function

        ''' <summary>
        ''' Converts this {@code BigDecimal} to a {@code short}, checking
        ''' for lost information.  If this {@code BigDecimal} has a
        ''' nonzero fractional part or is out of the possible range for a
        ''' {@code short} result then an {@code ArithmeticException} is
        ''' thrown.
        ''' </summary>
        ''' <returns> this {@code BigDecimal} converted to a {@code short}. </returns>
        ''' <exception cref="ArithmeticException"> if {@code this} has a nonzero
        '''         fractional part, or will not fit in a {@code short}.
        ''' @since  1.5 </exception>
        Public Overridable Function shortValueExact() As Short
            Dim num As Long
            num = Me.longValueExact() ' will check decimal part
            If CShort(num) <> num Then Throw New java.lang.ArithmeticException("Overflow")
            Return CShort(num)
        End Function

        ''' <summary>
        ''' Converts this {@code BigDecimal} to a {@code byte}, checking
        ''' for lost information.  If this {@code BigDecimal} has a
        ''' nonzero fractional part or is out of the possible range for a
        ''' {@code byte} result then an {@code ArithmeticException} is
        ''' thrown.
        ''' </summary>
        ''' <returns> this {@code BigDecimal} converted to a {@code byte}. </returns>
        ''' <exception cref="ArithmeticException"> if {@code this} has a nonzero
        '''         fractional part, or will not fit in a {@code byte}.
        ''' @since  1.5 </exception>
        Public Overridable Function byteValueExact() As SByte
            Dim num As Long
            num = Me.longValueExact() ' will check decimal part
            If CByte(num) <> num Then Throw New java.lang.ArithmeticException("Overflow")
            Return CByte(num)
        End Function

        ''' <summary>
        ''' Converts this {@code BigDecimal} to a {@code float}.
        ''' This conversion is similar to the
        ''' <i>narrowing primitive conversion</i> from {@code double} to
        ''' {@code float} as defined in section 5.1.3 of
        ''' <cite>The Java&trade; Language Specification</cite>:
        ''' if this {@code BigDecimal} has too great a
        ''' magnitude to represent as a {@code float}, it will be
        ''' converted to <seealso cref="Float#NEGATIVE_INFINITY"/> or {@link
        ''' Float#POSITIVE_INFINITY} as appropriate.  Note that even when
        ''' the return value is finite, this conversion can lose
        ''' information about the precision of the {@code BigDecimal}
        ''' value.
        ''' </summary>
        ''' <returns> this {@code BigDecimal} converted to a {@code float}. </returns>
        Public Overrides Function floatValue() As Single
            If intCompact <> INFLATED_Renamed Then
                If scale_Renamed = 0 Then
                    Return CSng(intCompact)
                Else
                    '                
                    '                 * If both intCompact and the scale can be exactly
                    '                 * represented as float values, perform a single float
                    '                 * multiply or divide to compute the (properly
                    '                 * rounded) result.
                    '                 
                    If System.Math.Abs(intCompact) < 1L << 22 Then
                        ' Don't have too guard against
                        ' System.Math.abs(MIN_VALUE) because of outer check
                        ' against INFLATED.
                        If scale_Renamed > 0 AndAlso scale_Renamed < float10pow.Length Then
                            Return CSng(intCompact) / float10pow(scale_Renamed)
                        ElseIf scale_Renamed < 0 AndAlso scale_Renamed > -float10pow.Length Then
                            Return CSng(intCompact) * float10pow(-scale_Renamed)
                        End If
                    End If
                End If
            End If
            ' Somewhat inefficient, but guaranteed to work.
            Return Convert.ToSingle(Me.ToString())
        End Function

        ''' <summary>
        ''' Converts this {@code BigDecimal} to a {@code double}.
        ''' This conversion is similar to the
        ''' <i>narrowing primitive conversion</i> from {@code double} to
        ''' {@code float} as defined in section 5.1.3 of
        ''' <cite>The Java&trade; Language Specification</cite>:
        ''' if this {@code BigDecimal} has too great a
        ''' magnitude represent as a {@code double}, it will be
        ''' converted to <seealso cref="Double#NEGATIVE_INFINITY"/> or {@link
        ''' Double#POSITIVE_INFINITY} as appropriate.  Note that even when
        ''' the return value is finite, this conversion can lose
        ''' information about the precision of the {@code BigDecimal}
        ''' value.
        ''' </summary>
        ''' <returns> this {@code BigDecimal} converted to a {@code double}. </returns>
        Public Overrides Function doubleValue() As Double
            If intCompact <> INFLATED_Renamed Then
                If scale_Renamed = 0 Then
                    Return CDbl(intCompact)
                Else
                    '                
                    '                 * If both intCompact and the scale can be exactly
                    '                 * represented as double values, perform a single
                    '                 * double multiply or divide to compute the (properly
                    '                 * rounded) result.
                    '                 
                    If System.Math.Abs(intCompact) < 1L << 52 Then
                        ' Don't have too guard against
                        ' System.Math.abs(MIN_VALUE) because of outer check
                        ' against INFLATED.
                        If scale_Renamed > 0 AndAlso scale_Renamed < double10pow.Length Then
                            Return CDbl(intCompact) / double10pow(scale_Renamed)
                        ElseIf scale_Renamed < 0 AndAlso scale_Renamed > -double10pow.Length Then
                            Return CDbl(intCompact) * double10pow(-scale_Renamed)
                        End If
                    End If
                End If
            End If
            ' Somewhat inefficient, but guaranteed to work.
            Return Convert.ToDouble(Me.ToString())
        End Function

        ''' <summary>
        ''' Powers of 10 which can be represented exactly in {@code
        ''' double}.
        ''' </summary>
        Private Shared ReadOnly double10pow As Double() = {1.0, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, 10000000.0, 100000000.0, 1000000000.0, 10000000000.0, 100000000000.0, 1000000000000.0, 10000000000000.0, 100000000000000.0, 1.0E+15, 1.0E+16, 1.0E+17, 1.0E+18, 1.0E+19, 1.0E+20, 1.0E+21, 1.0E+22}

        ''' <summary>
        ''' Powers of 10 which can be represented exactly in {@code
        ''' float}.
        ''' </summary>
        Private Shared ReadOnly float10pow As Single() = {1.0F, 10.0F, 100.0F, 1000.0F, 10000.0F, 100000.0F, 1000000.0F, 1.0E+7F, 1.0E+8F, 1.0E+9F, 1.0E+10F}

        ''' <summary>
        ''' Returns the size of an ulp, a unit in the last place, of this
        ''' {@code BigDecimal}.  An ulp of a nonzero {@code BigDecimal}
        ''' value is the positive distance between this value and the
        ''' {@code BigDecimal} value next larger in magnitude with the
        ''' same number of digits.  An ulp of a zero value is numerically
        ''' equal to 1 with the scale of {@code this}.  The result is
        ''' stored with the same scale as {@code this} so the result
        ''' for zero and nonzero values is equal to {@code [1,
        ''' this.scale()]}.
        ''' </summary>
        ''' <returns> the size of an ulp of {@code this}
        ''' @since 1.5 </returns>
        Public Overridable Function ulp() As BigDecimal
            Return BigDecimal.valueOf(1, Me.scale(), 1)
        End Function

        ' Private class to build a string representation for BigDecimal object.
        ' "StringBuilderHelper" is constructed as a thread local variable so it is
        ' thread safe. The StringBuilder field acts as a buffer to hold the temporary
        ' representation of BigDecimal. The cmpCharArray holds all the characters for
        ' the compact representation of BigDecimal (except for '-' sign' if it is
        ' negative) if its intCompact field is not INFLATED. It is shared by all
        ' calls to toString() and its variants in that particular thread.
        Friend Class StringBuilderHelper
            Friend ReadOnly sb As StringBuilder ' Placeholder for BigDecimal string
            Friend ReadOnly cmpCharArray As Char() ' character array to place the intCompact

            Friend Sub New()
                sb = New StringBuilder
                ' All non negative longs can be made to fit into 19 character array.
                cmpCharArray = New Char(18) {}
            End Sub

            ' Accessors.
            Friend Overridable Property stringBuilder As StringBuilder
                Get
                    sb.length = 0
                    Return sb
                End Get
            End Property

            Friend Overridable Property compactCharArray As Char()
                Get
                    Return cmpCharArray
                End Get
            End Property

            ''' <summary>
            ''' Places characters representing the intCompact in {@code long} into
            ''' cmpCharArray and returns the offset to the array where the
            ''' representation starts.
            ''' </summary>
            ''' <param name="intCompact"> the number to put into the cmpCharArray. </param>
            ''' <returns> offset to the array where the representation starts.
            ''' Note: intCompact must be greater or equal to zero. </returns>
            Friend Overridable Function putIntCompact(ByVal intCompact As Long) As Integer
                Debug.Assert(intCompact >= 0)

                Dim q As Long
                Dim r As Integer
                ' since we start from the least significant digit, charPos points to
                ' the last character in cmpCharArray.
                Dim charPos As Integer = cmpCharArray.Length

                ' Get 2 digits/iteration using longs until quotient fits into an int
                Do While intCompact >  java.lang.[Integer].Max_Value
                    q = intCompact \ 100
                    r = CInt(intCompact - q * 100)
                    intCompact = q
                    charPos -= 1
                    cmpCharArray(charPos) = DIGIT_ONES(r)
                    charPos -= 1
                    cmpCharArray(charPos) = DIGIT_TENS(r)
                Loop

                ' Get 2 digits/iteration using ints when i2 >= 100
                Dim q2 As Integer
                Dim i2 As Integer = CInt(intCompact)
                Do While i2 >= 100
                    q2 = i2 \ 100
                    r = i2 - q2 * 100
                    i2 = q2
                    charPos -= 1
                    cmpCharArray(charPos) = DIGIT_ONES(r)
                    charPos -= 1
                    cmpCharArray(charPos) = DIGIT_TENS(r)
                Loop

                charPos -= 1
                cmpCharArray(charPos) = DIGIT_ONES(i2)
                If i2 >= 10 Then
                    charPos -= 1
                    cmpCharArray(charPos) = DIGIT_TENS(i2)
                End If

                Return charPos
            End Function

            Friend Shared ReadOnly DIGIT_TENS As Char() = {"0"c, "0"c, "0"c, "0"c, "0"c, "0"c, "0"c, "0"c, "0"c, "0"c, "1"c, "1"c, "1"c, "1"c, "1"c, "1"c, "1"c, "1"c, "1"c, "1"c, "2"c, "2"c, "2"c, "2"c, "2"c, "2"c, "2"c, "2"c, "2"c, "2"c, "3"c, "3"c, "3"c, "3"c, "3"c, "3"c, "3"c, "3"c, "3"c, "3"c, "4"c, "4"c, "4"c, "4"c, "4"c, "4"c, "4"c, "4"c, "4"c, "4"c, "5"c, "5"c, "5"c, "5"c, "5"c, "5"c, "5"c, "5"c, "5"c, "5"c, "6"c, "6"c, "6"c, "6"c, "6"c, "6"c, "6"c, "6"c, "6"c, "6"c, "7"c, "7"c, "7"c, "7"c, "7"c, "7"c, "7"c, "7"c, "7"c, "7"c, "8"c, "8"c, "8"c, "8"c, "8"c, "8"c, "8"c, "8"c, "8"c, "8"c, "9"c, "9"c, "9"c, "9"c, "9"c, "9"c, "9"c, "9"c, "9"c, "9"c}

            Friend Shared ReadOnly DIGIT_ONES As Char() = {"0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c}
        End Class

        ''' <summary>
        ''' Lay out this {@code BigDecimal} into a {@code char[]} array.
        ''' The Java 1.2 equivalent to this was called {@code getValueString}.
        ''' </summary>
        ''' <param name="sci"> {@code true} for Scientific exponential notation;
        '''          {@code false} for Engineering </param>
        ''' <returns> string with canonical string representation of this
        '''         {@code BigDecimal} </returns>
        Private Function layoutChars(ByVal sci As Boolean) As String
            If scale_Renamed = 0 Then ' zero scale is trivial Return If(intCompact <> INFLATED_Renamed, Convert.ToString(intCompact), intVal.ToString())
                If scale_Renamed = 2 AndAlso intCompact >= 0 AndAlso intCompact <  java.lang.[Integer].Max_Value Then
                    ' currency fast path
                    Dim lowInt As Integer = CInt(intCompact) Mod 100
                    Dim highInt As Integer = CInt(intCompact) \ 100
                    Return (Convert.ToString(highInt) + AscW("."c) + AscW(StringBuilderHelper.DIGIT_TENS(lowInt)) + AscW(StringBuilderHelper.DIGIT_ONES(lowInt)))
                End If

                Dim sbHelper As StringBuilderHelper = threadLocalStringBuilderHelper.get()
                Dim coeff As Char()
                Dim offset As Integer ' offset is the starting index for coeff array
                ' Get the significand as an absolute value
                If intCompact <> INFLATED_Renamed Then
                    offset = sbHelper.putIntCompact (System.Math.Abs(intCompact))
                    coeff = sbHelper.compactCharArray
                Else
                    offset = 0
                    coeff = intVal.abs().ToString().ToCharArray()
                End If

                ' Construct a buffer, with sufficient capacity for all cases.
                ' If E-notation is needed, length will be: +1 if negative, +1
                ' if '.' needed, +2 for "E+", + up to 10 for adjusted exponent.
                ' Otherwise it could have +1 if negative, plus leading "0.00000"
                Dim buf As StringBuilder = sbHelper.stringBuilder
                If signum() < 0 Then ' prefix '-' if negative buf.append("-"c)
                    Dim coeffLen As Integer = coeff.Length - offset
                    Dim adjusted As Long = -CLng(scale_Renamed) + (coeffLen - 1)
                    If (scale_Renamed >= 0) AndAlso (adjusted >= -6) Then ' plain number
                        Dim pad As Integer = scale_Renamed - coeffLen ' count of padding zeros
                        If pad >= 0 Then ' 0.xxx form
                            buf.append("0"c)
                            buf.append("."c)
                            Do While pad > 0
                                buf.append("0"c)
                                pad -= 1
                            Loop
                            buf.append(coeff, offset, coeffLen) ' xx.xx form
                        Else
                            buf.append(coeff, offset, -pad)
                            buf.append("."c)
                            buf.append(coeff, -pad + offset, scale_Renamed)
                        End If ' E-notation is needed
                    Else
                        If sci Then ' Scientific notation
                            buf.append(coeff(offset)) ' first character
                            If coeffLen > 1 Then ' more to come
                                buf.append("."c)
                                buf.append(coeff, offset + 1, coeffLen - 1)
                            End If ' Engineering notation
                        Else
                            Dim sig As Integer = CInt(Fix(adjusted Mod 3))
                            If sig < 0 Then sig += 3 ' [adjusted was negative]
                            adjusted -= sig ' now a multiple of 3
                            sig += 1
                            If signum() = 0 Then
                                Select Case sig
                                    Case 1
                                        buf.append("0"c) ' exponent is a multiple of three
                                    Case 2
                                        buf.append("0.00")
                                        adjusted += 3
                                    Case 3
                                        buf.append("0.0")
                                        adjusted += 3
                                    Case Else
                                        Throw New AssertionError("Unexpected sig value " & sig)
                                End Select ' significand all in integer
                            ElseIf sig >= coeffLen Then
                                buf.append(coeff, offset, coeffLen)
                                ' may need some zeros, too
                                For i As Integer = sig - coeffLen To 1 Step -1
                                    buf.append("0"c)
                                Next i ' xx.xxE form
                            Else
                                buf.append(coeff, offset, sig)
                                buf.append("."c)
                                buf.append(coeff, offset + sig, coeffLen - sig)
                            End If
                        End If
                        If adjusted <> 0 Then ' [!sci could have made 0]
                            buf.append("E"c)
                            If adjusted > 0 Then ' force sign for positive buf.append("+"c)
                                buf.append(adjusted)
                            End If
                        End If
                        Return buf.ToString()
        End Function

        ''' <summary>
        ''' Return 10 to the power n, as a {@code BigInteger}.
        ''' </summary>
        ''' <param name="n"> the power of ten to be returned (>=0) </param>
        ''' <returns> a {@code BigInteger} with the value (10<sup>n</sup>) </returns>
        Private Shared Function bigTenToThe(ByVal n As Integer) As BigInteger
            If n < 0 Then Return Big java.lang.[Integer].ZERO

            If n < BIG_TEN_POWERS_TABLE_MAX Then
                Dim pows As BigInteger() = BIG_TEN_POWERS_TABLE
                If n < pows.Length Then
                    Return pows(n)
                Else
                    Return expandBigIntegerTenPowers(n)
                End If
            End If

            Return Big java.lang.[Integer].TEN.pow(n)
        End Function

        ''' <summary>
        ''' Expand the BIG_TEN_POWERS_TABLE array to contain at least 10**n.
        ''' </summary>
        ''' <param name="n"> the power of ten to be returned (>=0) </param>
        ''' <returns> a {@code BigDecimal} with the value (10<sup>n</sup>) and
        '''         in the meantime, the BIG_TEN_POWERS_TABLE array gets
        '''         expanded to the size greater than n. </returns>
        Private Shared Function expandBigIntegerTenPowers(ByVal n As Integer) As BigInteger
            SyncLock GetType(BigDecimal)
                Dim pows As BigInteger() = BIG_TEN_POWERS_TABLE
                Dim curLen As Integer = pows.Length
                ' The following comparison and the above synchronized statement is
                ' to prevent multiple threads from expanding the same array.
                If curLen <= n Then
                    Dim newLen As Integer = curLen << 1
                    Do While newLen <= n
                        newLen <<= 1
                    Loop
                    pows = java.util.Arrays.copyOf(pows, newLen)
                    For i As Integer = curLen To newLen - 1
                        pows(i) = pows(i - 1).multiply(Big java.lang.[Integer].TEN)
                    Next i
                    ' Based on the following facts:
                    ' 1. pows is a private local varible;
                    ' 2. the following store is a volatile store.
                    ' the newly created array elements can be safely published.
                    BIG_TEN_POWERS_TABLE = pows
                End If
                Return pows(n)
            End SyncLock
        End Function

        Private Shared ReadOnly LONG_TEN_POWERS_TABLE As Long() = {1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000, 10000000000L, 100000000000L, 1000000000000L, 10000000000000L, 100000000000000L, 1000000000000000L, 10000000000000000L, 100000000000000000L, 1000000000000000000L}

        'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
        Private Shared BIG_TEN_POWERS_TABLE As BigInteger() = {Big java.lang.[Integer].ONE, Big java.lang.[Integer].valueOf(10), Big java.lang.[Integer].valueOf(100), Big java.lang.[Integer].valueOf(1000), Big java.lang.[Integer].valueOf(10000), Big java.lang.[Integer].valueOf(100000), Big java.lang.[Integer].valueOf(1000000), Big java.lang.[Integer].valueOf(10000000), Big java.lang.[Integer].valueOf(100000000), Big java.lang.[Integer].valueOf(1000000000), Big java.lang.[Integer].valueOf(10000000000L), Big java.lang.[Integer].valueOf(100000000000L), Big java.lang.[Integer].valueOf(1000000000000L), Big java.lang.[Integer].valueOf(10000000000000L), Big java.lang.[Integer].valueOf(100000000000000L), Big java.lang.[Integer].valueOf(1000000000000000L), Big java.lang.[Integer].valueOf(10000000000000000L), Big java.lang.[Integer].valueOf(100000000000000000L), Big java.lang.[Integer].valueOf(1000000000000000000L)}

        Private Shared ReadOnly BIG_TEN_POWERS_TABLE_INITLEN As Integer = BIG_TEN_POWERS_TABLE.Length
        Private Shared ReadOnly BIG_TEN_POWERS_TABLE_MAX As Integer = 16 * BIG_TEN_POWERS_TABLE_INITLEN

        Private Shared ReadOnly THRESHOLDS_TABLE As Long() = {Long.MAX_VALUE, java.lang.[Long].MAX_VALUE / 10L, java.lang.[Long].MAX_VALUE / 100L, java.lang.[Long].MAX_VALUE / 1000L, java.lang.[Long].MAX_VALUE / 10000L, java.lang.[Long].MAX_VALUE / 100000L, java.lang.[Long].MAX_VALUE / 1000000L, java.lang.[Long].MAX_VALUE / 10000000L, java.lang.[Long].MAX_VALUE / 100000000L, java.lang.[Long].MAX_VALUE / 1000000000L, java.lang.[Long].MAX_VALUE / 10000000000L, java.lang.[Long].MAX_VALUE / 100000000000L, java.lang.[Long].MAX_VALUE / 1000000000000L, java.lang.[Long].MAX_VALUE / 10000000000000L, java.lang.[Long].MAX_VALUE / 100000000000000L, java.lang.[Long].MAX_VALUE / 1000000000000000L, java.lang.[Long].MAX_VALUE / 10000000000000000L, java.lang.[Long].MAX_VALUE / 100000000000000000L, java.lang.[Long].MAX_VALUE / 1000000000000000000L}

        ''' <summary>
        ''' Compute val * 10 ^ n; return this product if it is
        ''' representable as a long, INFLATED otherwise.
        ''' </summary>
        Private Shared Function longMultiplyPowerTen(ByVal val As Long, ByVal n As Integer) As Long
            If val = 0 OrElse n <= 0 Then Return val
            Dim tab As Long() = LONG_TEN_POWERS_TABLE
            Dim bounds As Long() = THRESHOLDS_TABLE
            If n < tab.Length AndAlso n < bounds.Length Then
                Dim tenpower As Long = tab(n)
                If val = 1 Then Return tenpower
                If System.Math.Abs(val) <= bounds(n) Then Return val * tenpower
            End If
            Return INFLATED_Renamed
        End Function

        ''' <summary>
        ''' Compute this * 10 ^ n.
        ''' Needed mainly to allow special casing to trap zero value
        ''' </summary>
        Private Function bigMultiplyPowerTen(ByVal n As Integer) As BigInteger
            If n <= 0 Then Return Me.inflated()

            If intCompact <> INFLATED_Renamed Then
                Return bigTenToThe(n).multiply(intCompact)
            Else
                Return intVal.multiply(bigTenToThe(n))
            End If
        End Function

        ''' <summary>
        ''' Returns appropriate BigInteger from intVal field if intVal is
        ''' null, i.e. the compact representation is in use.
        ''' </summary>
        Private Function inflated() As BigInteger
            If intVal Is Nothing Then Return Big java.lang.[Integer].valueOf(intCompact)
            Return intVal
        End Function

        ''' <summary>
        ''' Match the scales of two {@code BigDecimal}s to align their
        ''' least significant digits.
        ''' 
        ''' <p>If the scales of val[0] and val[1] differ, rescale
        ''' (non-destructively) the lower-scaled {@code BigDecimal} so
        ''' they match.  That is, the lower-scaled reference will be
        ''' replaced by a reference to a new object with the same scale as
        ''' the other {@code BigDecimal}.
        ''' </summary>
        ''' <param name="val"> array of two elements referring to the two
        '''         {@code BigDecimal}s to be aligned. </param>
        Private Shared Sub matchScale(ByVal val As BigDecimal())
            If val(0).scale_Renamed = val(1).scale_Renamed Then
                Return
            ElseIf val(0).scale_Renamed < val(1).scale_Renamed Then
                val(0) = val(0).scaleale(val(1).scale_Renamed, ROUND_UNNECESSARY)
            ElseIf val(1).scale_Renamed < val(0).scale_Renamed Then
                val(1) = val(1).scaleale(val(0).scale_Renamed, ROUND_UNNECESSARY)
            End If
        End Sub

        Private Class UnsafeHolder
            Private Shared ReadOnly unsafe As sun.misc.Unsafe
            Private Shared ReadOnly intCompactOffset As Long
            Private Shared ReadOnly intValOffset As Long
            Shared Sub New()
                Try
                    unsafe = sun.misc.Unsafe.unsafe
                    intCompactOffset = unsafe.objectFieldOffset(GetType(BigDecimal).getDeclaredField("intCompact"))
                    intValOffset = unsafe.objectFieldOffset(GetType(BigDecimal).getDeclaredField("intVal"))
                Catch ex As Exception
                    Throw New ExceptionInInitializerError(ex)
                End Try
            End Sub
            Friend Shared Sub setIntCompactVolatile(ByVal bd As BigDecimal, ByVal val As Long)
                unsafe.putLongVolatile(bd, intCompactOffset, val)
            End Sub

            Friend Shared Sub setIntValVolatile(ByVal bd As BigDecimal, ByVal val As BigInteger)
                unsafe.putObjectVolatile(bd, intValOffset, val)
            End Sub
        End Class

        ''' <summary>
        ''' Reconstitute the {@code BigDecimal} instance from a stream (that is,
        ''' deserialize it).
        ''' </summary>
        ''' <param name="s"> the stream being read. </param>
        Private Sub readObject(ByVal s As java.io.ObjectInputStream)
            ' Read in all fields
            s.defaultReadObject()
            ' validate possibly bad fields
            If intVal Is Nothing Then
                Dim message As String = "BigDecimal: null intVal in stream"
                Throw New java.io.StreamCorruptedException(message)
                ' [all values of scale are now allowed]
            End If
            UnsafeHolder.intCompactVolatileile(Me, compactValFor(intVal))
        End Sub

        ''' <summary>
        ''' Serialize this {@code BigDecimal} to the stream in question
        ''' </summary>
        ''' <param name="s"> the stream to serialize to. </param>
        Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
            ' Must inflate to maintain compatible serial form.
            If Me.intVal Is Nothing Then UnsafeHolder.intValVolatileile(Me, Big java.lang.[Integer].valueOf(Me.intCompact))
            ' Could reset intVal back to null if it has to be set.
            s.defaultWriteObject()
        End Sub

        ''' <summary>
        ''' Returns the length of the absolute value of a {@code long}, in decimal
        ''' digits.
        ''' </summary>
        ''' <param name="x"> the {@code long} </param>
        ''' <returns> the length of the unscaled value, in deciaml digits. </returns>
        Friend Shared Function longDigitLength(ByVal x As Long) As Integer
            '        
            '         * As described in "Bit Twiddling Hacks" by Sean Anderson,
            '         * (http://graphics.stanford.edu/~seander/bithacks.html)
            '         * integer log 10 of x is within 1 of (1233/4096)* (1 +
            '         * integer log 2 of x). The fraction 1233/4096 approximates
            '         * log10(2). So we first do a version of log2 (a variant of
            '         * Long class with pre-checks and opposite directionality) and
            '         * then scale and check against powers table. This is a little
            '         * simpler in present context than the version in Hacker's
            '         * Delight sec 11-4. Adding one to bit length allows comparing
            '         * downward from the LONG_TEN_POWERS_TABLE that we need
            '         * anyway.
            '         
            Debug.Assert(x <> BigDecimal.INFLATED_Renamed)
            If x < 0 Then x = -x
            If x < 10 Then ' must screen for 0, might as well 10 Return 1
                Dim r As Integer = CInt(CUInt(((64 - java.lang.[Long].numberOfLeadingZeros(x) + 1) * 1233)) >> 12)
                Dim tab As Long() = LONG_TEN_POWERS_TABLE
                ' if r >= length, must have max possible digits for long
                Return If(r >= tab.Length OrElse x < tab(r), r, r + 1)
        End Function

        ''' <summary>
        ''' Returns the length of the absolute value of a BigInteger, in
        ''' decimal digits.
        ''' </summary>
        ''' <param name="b"> the BigInteger </param>
        ''' <returns> the length of the unscaled value, in decimal digits </returns>
        Private Shared Function bigDigitLength(ByVal b As BigInteger) As Integer
            '        
            '         * Same idea as the long version, but we need a better
            '         * approximation of log10(2). Using 646456993/2^31
            '         * is accurate up to max possible reported bitLength.
            '         
            If b.signum_Renamed = 0 Then Return 1
            Dim r As Integer = CInt(CInt(CUInt(((CLng(b.bitLength()) + 1) * 646456993)) >> 31))
            Return If(b.compareMagnitude(bigTenToThe(r)) < 0, r, r + 1)
        End Function

        ''' <summary>
        ''' Check a scale for Underflow or Overflow.  If this BigDecimal is
        ''' nonzero, throw an exception if the scale is outof range. If this
        ''' is zero, saturate the scale to the extreme value of the right
        ''' sign if the scale is out of range.
        ''' </summary>
        ''' <param name="val"> The new scale. </param>
        ''' <exception cref="ArithmeticException"> (overflow or underflow) if the new
        '''         scale is out of range. </exception>
        ''' <returns> validated scale as an int. </returns>
        Private Function checkScale(ByVal val As Long) As Integer
            Dim asInt As Integer = CInt(val)
            If asInt <> val Then
                asInt = If(val >  java.lang.[Integer].Max_Value,  java.lang.[Integer].Max_Value,  java.lang.[Integer].MIN_VALUE)
                Dim b As BigInteger
                b = intVal
                If intCompact <> 0 AndAlso (b Is Nothing OrElse b.signum() <> 0) Then Throw New ArithmeticException(If(asInt > 0, "Underflow", "Overflow"))
            End If
            Return asInt
        End Function

        ''' <summary>
        ''' Returns the compact value for given {@code BigInteger}, or
        ''' INFLATED if too big. Relies on internal representation of
        ''' {@code BigInteger}.
        ''' </summary>
        Private Shared Function compactValFor(ByVal b As BigInteger) As Long
            Dim m As Integer() = b.mag
            Dim len As Integer = m.Length
            If len = 0 Then Return 0
            Dim d As Integer = m(0)
            If len > 2 OrElse (len = 2 AndAlso d < 0) Then Return INFLATED_Renamed

            Dim u As Long = If(len = 2, ((CLng(m(1)) And LONG_MASK) + ((CLng(d)) << 32)), ((CLng(d)) And LONG_MASK))
            Return If(b.signum_Renamed < 0, -u, u)
        End Function

        Private Shared Function longCompareMagnitude(ByVal x As Long, ByVal y As Long) As Integer
            If x < 0 Then x = -x
            If y < 0 Then y = -y
            Return If(x < y, -1, (If(x = y, 0, 1)))
        End Function

        Private Shared Function saturateLong(ByVal s As Long) As Integer
            Dim i As Integer = CInt(s)
            Return If(s = i, i, (If(s < 0,  java.lang.[Integer].MIN_VALUE,  java.lang.[Integer].Max_Value)))
        End Function

        '    
        '     * Internal printing routine
        '     
        Private Shared Sub print(ByVal name As String, ByVal bd As BigDecimal)
            System.err.format("%s:" & vbTab & "intCompact %d" & vbTab & "intVal %d" & vbTab & "scale %d" & vbTab & "precision %d%n", name, bd.intCompact, bd.intVal, bd.scale_Renamed, bd.precision_Renamed)
        End Sub

        ''' <summary>
        ''' Check internal invariants of this BigDecimal.  These invariants
        ''' include:
        ''' 
        ''' <ul>
        ''' 
        ''' <li>The object must be initialized; either intCompact must not be
        ''' INFLATED or intVal is non-null.  Both of these conditions may
        ''' be true.
        ''' 
        ''' <li>If both intCompact and intVal and set, their values must be
        ''' consistent.
        ''' 
        ''' <li>If precision is nonzero, it must have the right value.
        ''' </ul>
        ''' 
        ''' Note: Since this is an audit method, we are not supposed to change the
        ''' state of this BigDecimal object.
        ''' </summary>
        Private Function audit() As BigDecimal
            If intCompact = INFLATED_Renamed Then
                If intVal Is Nothing Then
                    print("audit", Me)
                    Throw New AssertionError("null intVal")
                End If
                ' Check precision
                If precision_Renamed > 0 AndAlso precision_Renamed <> bigDigitLength(intVal) Then
                    print("audit", Me)
                    Throw New AssertionError("precision mismatch")
                End If
            Else
                If intVal IsNot Nothing Then
                    Dim val As Long = intVal
                    If val <> intCompact Then
                        print("audit", Me)
                        Throw New AssertionError("Inconsistent state, intCompact=" & intCompact & vbTab & " intVal=" & val)
                    End If
                End If
                ' Check precision
                If precision_Renamed > 0 AndAlso precision_Renamed <> longDigitLength(intCompact) Then
                    print("audit", Me)
                    Throw New AssertionError("precision mismatch")
                End If
            End If
            Return Me
        End Function

        ' the same as checkScale where value!=0 
        Private Shared Function checkScaleNonZero(ByVal val As Long) As Integer
            Dim asInt As Integer = CInt(val)
            If asInt <> val Then Throw New ArithmeticException(If(asInt > 0, "Underflow", "Overflow"))
            Return asInt
        End Function

        Private Shared Function checkScale(ByVal intCompact As Long, ByVal val As Long) As Integer
            Dim asInt As Integer = CInt(val)
            If asInt <> val Then
                asInt = If(val >  java.lang.[Integer].Max_Value,  java.lang.[Integer].Max_Value,  java.lang.[Integer].MIN_VALUE)
                If intCompact <> 0 Then Throw New ArithmeticException(If(asInt > 0, "Underflow", "Overflow"))
            End If
            Return asInt
        End Function

        Private Shared Function checkScale(ByVal intVal As BigInteger, ByVal val As Long) As Integer
            Dim asInt As Integer = CInt(val)
            If asInt <> val Then
                asInt = If(val >  java.lang.[Integer].Max_Value,  java.lang.[Integer].Max_Value,  java.lang.[Integer].MIN_VALUE)
                If intVal.signum() <> 0 Then Throw New ArithmeticException(If(asInt > 0, "Underflow", "Overflow"))
            End If
            Return asInt
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} rounded according to the MathContext
        ''' settings;
        ''' If rounding is needed a new {@code BigDecimal} is created and returned.
        ''' </summary>
        ''' <param name="val"> the value to be rounded </param>
        ''' <param name="mc"> the context to use. </param>
        ''' <returns> a {@code BigDecimal} rounded according to the MathContext
        '''         settings.  May return {@code value}, if no rounding needed. </returns>
        ''' <exception cref="ArithmeticException"> if the rounding mode is
        '''         {@code RoundingMode.UNNECESSARY} and the
        '''         result is inexact. </exception>
        Private Shared Function doRound(ByVal val As BigDecimal, ByVal mc As MathContext) As BigDecimal
            Dim mcp As Integer = mc.precision
            Dim wasDivided As Boolean = False
            If mcp > 0 Then
                Dim intVal As BigInteger = val.intVal
                Dim compactVal As Long = val.intCompact
                Dim scale_Renamed As Integer = val.scale_Renamed
                Dim prec As Integer = val.precision()
                Dim mode As Integer = mc.roundingMode.oldMode
                Dim drop As Integer
                If compactVal = INFLATED_Renamed Then
                    drop = prec - mcp
                    Do While drop > 0
                        scale_Renamed = checkScaleNonZero(CLng(scale_Renamed) - drop)
                        intVal = divideAndRoundByTenPow(intVal, drop, mode)
                        wasDivided = True
                        compactVal = compactValFor(intVal)
                        If compactVal <> INFLATED_Renamed Then
                            prec = longDigitLength(compactVal)
                            Exit Do
                        End If
                        prec = bigDigitLength(intVal)
                        drop = prec - mcp
                    Loop
                End If
                If compactVal <> INFLATED_Renamed Then
                    drop = prec - mcp ' drop can't be more than 18
                    Do While drop > 0
                        scale_Renamed = checkScaleNonZero(CLng(scale_Renamed) - drop)
                        compactVal = divideAndRound(compactVal, LONG_TEN_POWERS_TABLE(drop), mc.roundingMode.oldMode)
                        wasDivided = True
                        prec = longDigitLength(compactVal)
                        drop = prec - mcp
                        intVal = Nothing
                    Loop
                End If
                Return If(wasDivided, New BigDecimal(intVal, compactVal, scale_Renamed, prec), val)
            End If
            Return val
        End Function

        '    
        '     * Returns a {@code BigDecimal} created from {@code long} value with
        '     * given scale rounded according to the MathContext settings
        '     
        Private Shared Function doRound(ByVal compactVal As Long, ByVal scale As Integer, ByVal mc As MathContext) As BigDecimal
            Dim mcp As Integer = mc.precision
            If mcp > 0 AndAlso mcp < 19 Then
                Dim prec As Integer = longDigitLength(compactVal)
                Dim drop As Integer = prec - mcp ' drop can't be more than 18
                Do While drop > 0
                    scale = checkScaleNonZero(CLng(scale) - drop)
                    compactVal = divideAndRound(compactVal, LONG_TEN_POWERS_TABLE(drop), mc.roundingMode.oldMode)
                    prec = longDigitLength(compactVal)
                    drop = prec - mcp
                Loop
                Return valueOf(compactVal, scale, prec)
            End If
            Return valueOf(compactVal, scale)
        End Function

        '    
        '     * Returns a {@code BigDecimal} created from {@code BigInteger} value with
        '     * given scale rounded according to the MathContext settings
        '     
        Private Shared Function doRound(ByVal intVal As BigInteger, ByVal scale As Integer, ByVal mc As MathContext) As BigDecimal
            Dim mcp As Integer = mc.precision
            Dim prec As Integer = 0
            If mcp > 0 Then
                Dim compactVal As Long = compactValFor(intVal)
                Dim mode As Integer = mc.roundingMode.oldMode
                Dim drop As Integer
                If compactVal = INFLATED_Renamed Then
                    prec = bigDigitLength(intVal)
                    drop = prec - mcp
                    Do While drop > 0
                        scale = checkScaleNonZero(CLng(scale) - drop)
                        intVal = divideAndRoundByTenPow(intVal, drop, mode)
                        compactVal = compactValFor(intVal)
                        If compactVal <> INFLATED_Renamed Then Exit Do
                        prec = bigDigitLength(intVal)
                        drop = prec - mcp
                    Loop
                End If
                If compactVal <> INFLATED_Renamed Then
                    prec = longDigitLength(compactVal)
                    drop = prec - mcp ' drop can't be more than 18
                    Do While drop > 0
                        scale = checkScaleNonZero(CLng(scale) - drop)
                        compactVal = divideAndRound(compactVal, LONG_TEN_POWERS_TABLE(drop), mc.roundingMode.oldMode)
                        prec = longDigitLength(compactVal)
                        drop = prec - mcp
                    Loop
                    Return valueOf(compactVal, scale, prec)
                End If
            End If
            Return New BigDecimal(intVal, INFLATED_Renamed, scale, prec)
        End Function

        '    
        '     * Divides {@code BigInteger} value by ten power.
        '     
        Private Shared Function divideAndRoundByTenPow(ByVal intVal As BigInteger, ByVal tenPow As Integer, ByVal roundingMode As Integer) As BigInteger
            If tenPow < LONG_TEN_POWERS_TABLE.Length Then
                intVal = divideAndRound(intVal, LONG_TEN_POWERS_TABLE(tenPow), roundingMode)
            Else
                intVal = divideAndRound(intVal, bigTenToThe(tenPow), roundingMode)
            End If
            Return intVal
        End Function

        ''' <summary>
        ''' Internally used for division operation for division {@code long} by
        ''' {@code long}.
        ''' The returned {@code BigDecimal} object is the quotient whose scale is set
        ''' to the passed in scale. If the remainder is not zero, it will be rounded
        ''' based on the passed in roundingMode. Also, if the remainder is zero and
        ''' the last parameter, i.e. preferredScale is NOT equal to scale, the
        ''' trailing zeros of the result is stripped to match the preferredScale.
        ''' </summary>
        Private Shared Function divideAndRound(ByVal ldividend As Long, ByVal ldivisor As Long, ByVal scale As Integer, ByVal roundingMode As Integer, ByVal preferredScale As Integer) As BigDecimal

            Dim qsign As Integer ' quotient sign
            Dim q As Long = ldividend \ ldivisor ' store quotient in long
            If roundingMode = ROUND_DOWN AndAlso scale = preferredScale Then Return valueOf(q, scale)
            Dim r As Long = ldividend Mod ldivisor ' store remainder in long
            qsign = If((ldividend < 0) = (ldivisor < 0), 1, -1)
            If r <> 0 Then
                Dim increment As Boolean = needIncrement(ldivisor, roundingMode, qsign, q, r)
                Return valueOf((If(increment, q + qsign, q)), scale)
            Else
                If preferredScale <> scale Then
                    Return createAndStripZerosToMatchScale(q, scale, preferredScale)
                Else
                    Return valueOf(q, scale)
                End If
            End If
        End Function

        ''' <summary>
        ''' Divides {@code long} by {@code long} and do rounding based on the
        ''' passed in roundingMode.
        ''' </summary>
        Private Shared Function divideAndRound(ByVal ldividend As Long, ByVal ldivisor As Long, ByVal roundingMode As Integer) As Long
            Dim qsign As Integer ' quotient sign
            Dim q As Long = ldividend \ ldivisor ' store quotient in long
            If roundingMode = ROUND_DOWN Then Return q
            Dim r As Long = ldividend Mod ldivisor ' store remainder in long
            qsign = If((ldividend < 0) = (ldivisor < 0), 1, -1)
            If r <> 0 Then
                Dim increment As Boolean = needIncrement(ldivisor, roundingMode, qsign, q, r)
                Return If(increment, q + qsign, q)
            Else
                Return q
            End If
        End Function

        ''' <summary>
        ''' Shared logic of need increment computation.
        ''' </summary>
        Private Shared Function commonNeedIncrement(ByVal roundingMode As Integer, ByVal qsign As Integer, ByVal cmpFracHalf As Integer, ByVal oddQuot As Boolean) As Boolean
            Select Case roundingMode
                Case ROUND_UNNECESSARY
                    Throw New ArithmeticException("Rounding necessary")

                Case ROUND_UP ' Away from zero
                    Return True

                Case ROUND_DOWN ' Towards zero
                    Return False

                Case ROUND_CEILING ' Towards +infinity
                    Return qsign > 0

                Case ROUND_FLOOR ' Towards -infinity
                    Return qsign < 0

                Case Else ' Some kind of half-way rounding
                    Debug.Assert(roundingMode >= ROUND_HALF_UP AndAlso roundingMode <= ROUND_HALF_EVEN, "Unexpected rounding mode" & System.Enum.Parse(GetType(RoundingMode), roundingMode))

                    If cmpFracHalf < 0 Then ' We're closer to higher digit
                        Return False
                    ElseIf cmpFracHalf > 0 Then ' We're closer to lower digit
                        Return True
                    Else ' half-way
                        Debug.Assert(cmpFracHalf = 0)

                        Select Case roundingMode
                            Case ROUND_HALF_DOWN
                                Return False

                            Case ROUND_HALF_UP
                                Return True

                            Case ROUND_HALF_EVEN
                                Return oddQuot

                            Case Else
                                Throw New AssertionError("Unexpected rounding mode" & roundingMode)
                        End Select
                    End If
            End Select
        End Function

        ''' <summary>
        ''' Tests if quotient has to be incremented according the roundingMode
        ''' </summary>
        Private Shared Function needIncrement(ByVal ldivisor As Long, ByVal roundingMode As Integer, ByVal qsign As Integer, ByVal q As Long, ByVal r As Long) As Boolean
            Debug.Assert(r <> 0L)

            Dim cmpFracHalf As Integer
            If r <= HALF_LONG_MIN_VALUE OrElse r > HALF_LONG_MAX_VALUE Then
                cmpFracHalf = 1 ' 2 * r can't fit into long
            Else
                cmpFracHalf = longCompareMagnitude(2 * r, ldivisor)
            End If

            Return commonNeedIncrement(roundingMode, qsign, cmpFracHalf, (q And 1L) <> 0L)
        End Function

        ''' <summary>
        ''' Divides {@code BigInteger} value by {@code long} value and
        ''' do rounding based on the passed in roundingMode.
        ''' </summary>
        Private Shared Function divideAndRound(ByVal bdividend As BigInteger, ByVal ldivisor As Long, ByVal roundingMode As Integer) As BigInteger
            Dim isRemainderZero As Boolean ' record remainder is zero or not
            Dim qsign As Integer ' quotient sign
            Dim r As Long = 0 ' store quotient & remainder in long
            Dim mq As MutableBigInteger = Nothing ' store quotient
            ' Descend into mutables for faster remainder checks
            Dim mdividend As New MutableBigInteger(bdividend.mag)
            mq = New MutableBigInteger
            r = mdividend.divide(ldivisor, mq)
            isRemainderZero = (r = 0)
            qsign = If(ldivisor < 0, -bdividend.signum_Renamed, bdividend.signum_Renamed)
            If Not isRemainderZero Then
                If needIncrement(ldivisor, roundingMode, qsign, mq, r) Then mq.add(MutableBig java.lang.[Integer].ONE)
            End If
            Return mq.toBigInteger(qsign)
        End Function

        ''' <summary>
        ''' Internally used for division operation for division {@code BigInteger}
        ''' by {@code long}.
        ''' The returned {@code BigDecimal} object is the quotient whose scale is set
        ''' to the passed in scale. If the remainder is not zero, it will be rounded
        ''' based on the passed in roundingMode. Also, if the remainder is zero and
        ''' the last parameter, i.e. preferredScale is NOT equal to scale, the
        ''' trailing zeros of the result is stripped to match the preferredScale.
        ''' </summary>
        Private Shared Function divideAndRound(ByVal bdividend As BigInteger, ByVal ldivisor As Long, ByVal scale As Integer, ByVal roundingMode As Integer, ByVal preferredScale As Integer) As BigDecimal
            Dim isRemainderZero As Boolean ' record remainder is zero or not
            Dim qsign As Integer ' quotient sign
            Dim r As Long = 0 ' store quotient & remainder in long
            Dim mq As MutableBigInteger = Nothing ' store quotient
            ' Descend into mutables for faster remainder checks
            Dim mdividend As New MutableBigInteger(bdividend.mag)
            mq = New MutableBigInteger
            r = mdividend.divide(ldivisor, mq)
            isRemainderZero = (r = 0)
            qsign = If(ldivisor < 0, -bdividend.signum_Renamed, bdividend.signum_Renamed)
            If Not isRemainderZero Then
                If needIncrement(ldivisor, roundingMode, qsign, mq, r) Then mq.add(MutableBig java.lang.[Integer].ONE)
                Return mq.toBigDecimal(qsign, scale)
            Else
                If preferredScale <> scale Then
                    Dim compactVal As Long = mq.toCompactValue(qsign)
                    If compactVal <> INFLATED_Renamed Then Return createAndStripZerosToMatchScale(compactVal, scale, preferredScale)
                    Dim intVal As BigInteger = mq.toBigInteger(qsign)
                    Return createAndStripZerosToMatchScale(intVal, scale, preferredScale)
                Else
                    Return mq.toBigDecimal(qsign, scale)
                End If
            End If
        End Function

        ''' <summary>
        ''' Tests if quotient has to be incremented according the roundingMode
        ''' </summary>
        Private Shared Function needIncrement(ByVal ldivisor As Long, ByVal roundingMode As Integer, ByVal qsign As Integer, ByVal mq As MutableBigInteger, ByVal r As Long) As Boolean
            Debug.Assert(r <> 0L)

            Dim cmpFracHalf As Integer
            If r <= HALF_LONG_MIN_VALUE OrElse r > HALF_LONG_MAX_VALUE Then
                cmpFracHalf = 1 ' 2 * r can't fit into long
            Else
                cmpFracHalf = longCompareMagnitude(2 * r, ldivisor)
            End If

            Return commonNeedIncrement(roundingMode, qsign, cmpFracHalf, mq.odd)
        End Function

        ''' <summary>
        ''' Divides {@code BigInteger} value by {@code BigInteger} value and
        ''' do rounding based on the passed in roundingMode.
        ''' </summary>
        Private Shared Function divideAndRound(ByVal bdividend As BigInteger, ByVal bdivisor As BigInteger, ByVal roundingMode As Integer) As BigInteger
            Dim isRemainderZero As Boolean ' record remainder is zero or not
            Dim qsign As Integer ' quotient sign
            ' Descend into mutables for faster remainder checks
            Dim mdividend As New MutableBigInteger(bdividend.mag)
            Dim mq As New MutableBigInteger
            Dim mdivisor As New MutableBigInteger(bdivisor.mag)
            Dim mr As MutableBigInteger = mdividend.divide(mdivisor, mq)
            isRemainderZero = mr.zero
            qsign = If(bdividend.signum_Renamed <> bdivisor.signum_Renamed, -1, 1)
            If Not isRemainderZero Then
                If needIncrement(mdivisor, roundingMode, qsign, mq, mr) Then mq.add(MutableBig java.lang.[Integer].ONE)
            End If
            Return mq.toBigInteger(qsign)
        End Function

        ''' <summary>
        ''' Internally used for division operation for division {@code BigInteger}
        ''' by {@code BigInteger}.
        ''' The returned {@code BigDecimal} object is the quotient whose scale is set
        ''' to the passed in scale. If the remainder is not zero, it will be rounded
        ''' based on the passed in roundingMode. Also, if the remainder is zero and
        ''' the last parameter, i.e. preferredScale is NOT equal to scale, the
        ''' trailing zeros of the result is stripped to match the preferredScale.
        ''' </summary>
        Private Shared Function divideAndRound(ByVal bdividend As BigInteger, ByVal bdivisor As BigInteger, ByVal scale As Integer, ByVal roundingMode As Integer, ByVal preferredScale As Integer) As BigDecimal
            Dim isRemainderZero As Boolean ' record remainder is zero or not
            Dim qsign As Integer ' quotient sign
            ' Descend into mutables for faster remainder checks
            Dim mdividend As New MutableBigInteger(bdividend.mag)
            Dim mq As New MutableBigInteger
            Dim mdivisor As New MutableBigInteger(bdivisor.mag)
            Dim mr As MutableBigInteger = mdividend.divide(mdivisor, mq)
            isRemainderZero = mr.zero
            qsign = If(bdividend.signum_Renamed <> bdivisor.signum_Renamed, -1, 1)
            If Not isRemainderZero Then
                If needIncrement(mdivisor, roundingMode, qsign, mq, mr) Then mq.add(MutableBig java.lang.[Integer].ONE)
                Return mq.toBigDecimal(qsign, scale)
            Else
                If preferredScale <> scale Then
                    Dim compactVal As Long = mq.toCompactValue(qsign)
                    If compactVal <> INFLATED_Renamed Then Return createAndStripZerosToMatchScale(compactVal, scale, preferredScale)
                    Dim intVal As BigInteger = mq.toBigInteger(qsign)
                    Return createAndStripZerosToMatchScale(intVal, scale, preferredScale)
                Else
                    Return mq.toBigDecimal(qsign, scale)
                End If
            End If
        End Function

        ''' <summary>
        ''' Tests if quotient has to be incremented according the roundingMode
        ''' </summary>
        Private Shared Function needIncrement(ByVal mdivisor As MutableBigInteger, ByVal roundingMode As Integer, ByVal qsign As Integer, ByVal mq As MutableBigInteger, ByVal mr As MutableBigInteger) As Boolean
            Debug.Assert((Not mr.zero))
            Dim cmpFracHalf As Integer = mr.compareHalf(mdivisor)
            Return commonNeedIncrement(roundingMode, qsign, cmpFracHalf, mq.odd)
        End Function

        ''' <summary>
        ''' Remove insignificant trailing zeros from this
        ''' {@code BigInteger} value until the preferred scale is reached or no
        ''' more zeros can be removed.  If the preferred scale is less than
        '''  java.lang.[Integer].MIN_VALUE, all the trailing zeros will be removed.
        ''' </summary>
        ''' <returns> new {@code BigDecimal} with a scale possibly reduced
        ''' to be closed to the preferred scale. </returns>
        Private Shared Function createAndStripZerosToMatchScale(ByVal intVal As BigInteger, ByVal scale As Integer, ByVal preferredScale As Long) As BigDecimal
            Dim qr As BigInteger() ' quotient-remainder pair
            Do While intVal.compareMagnitude(Big java.lang.[Integer].TEN) >= 0 AndAlso scale > preferredScale
                If intVal.testBit(0) Then Exit Do ' odd number cannot end in 0
                qr = intVal.divideAndRemainder(Big java.lang.[Integer].TEN)
                If qr(1).signum() <> 0 Then Exit Do ' non-0 remainder
                intVal = qr(0)
                scale = checkScale(intVal, CLng(scale) - 1) ' could Overflow
            Loop
            Return valueOf(intVal, scale, 0)
        End Function

        ''' <summary>
        ''' Remove insignificant trailing zeros from this
        ''' {@code long} value until the preferred scale is reached or no
        ''' more zeros can be removed.  If the preferred scale is less than
        '''  java.lang.[Integer].MIN_VALUE, all the trailing zeros will be removed.
        ''' </summary>
        ''' <returns> new {@code BigDecimal} with a scale possibly reduced
        ''' to be closed to the preferred scale. </returns>
        Private Shared Function createAndStripZerosToMatchScale(ByVal compactVal As Long, ByVal scale As Integer, ByVal preferredScale As Long) As BigDecimal
            Do While System.Math.Abs(compactVal) >= 10L AndAlso scale > preferredScale
                If (compactVal And 1L) <> 0L Then Exit Do ' odd number cannot end in 0
                Dim r As Long = compactVal Mod 10L
                If r <> 0L Then Exit Do ' non-0 remainder
                compactVal \= 10
                scale = checkScale(compactVal, CLng(scale) - 1) ' could Overflow
            Loop
            Return valueOf(compactVal, scale)
        End Function

        Private Shared Function stripZerosToMatchScale(ByVal intVal As BigInteger, ByVal intCompact As Long, ByVal scale As Integer, ByVal preferredScale As Integer) As BigDecimal
            If intCompact <> INFLATED_Renamed Then
                Return createAndStripZerosToMatchScale(intCompact, scale, preferredScale)
            Else
                Return createAndStripZerosToMatchScale(If(intVal Is Nothing, INFLATED_BIGINT, intVal), scale, preferredScale)
            End If
        End Function

        '    
        '     * returns INFLATED if oveflow
        '     
        Private Shared Function add(ByVal xs As Long, ByVal ys As Long) As Long
            Dim sum As Long = xs + ys
            ' See "Hacker's Delight" section 2-12 for explanation of
            ' the overflow test.
            If (((sum Xor xs) And (sum Xor ys))) >= 0L Then ' not overflowed Return sum
                Return INFLATED_Renamed
        End Function

        Private Shared Function add(ByVal xs As Long, ByVal ys As Long, ByVal scale As Integer) As BigDecimal
            Dim sum As Long = add(xs, ys)
            If sum <> INFLATED_Renamed Then Return BigDecimal.valueOf(sum, scale)
            Return New BigDecimal(Big java.lang.[Integer].valueOf(xs) + ys, scale)
        End Function

        Private Shared Function add(ByVal xs As Long, ByVal scale1 As Integer, ByVal ys As Long, ByVal scale2 As Integer) As BigDecimal
            Dim sdiff As Long = CLng(scale1) - scale2
            If sdiff = 0 Then
                Return add(xs, ys, scale1)
            ElseIf sdiff < 0 Then
                Dim raise As Integer = checkScale(xs, -sdiff)
                Dim scaledX As Long = longMultiplyPowerTen(xs, raise)
                If scaledX <> INFLATED_Renamed Then
                    Return add(scaledX, ys, scale2)
                Else
                    Dim bigsum As BigInteger = bigMultiplyPowerTen(xs, raise).add(ys)
                    Return If((xs Xor ys) >= 0, New BigDecimal(bigsum, INFLATED_Renamed, scale2, 0), valueOf(bigsum, scale2, 0)) ' same sign test
                End If
            Else
                Dim raise As Integer = checkScale(ys, sdiff)
                Dim scaledY As Long = longMultiplyPowerTen(ys, raise)
                If scaledY <> INFLATED_Renamed Then
                    Return add(xs, scaledY, scale1)
                Else
                    Dim bigsum As BigInteger = bigMultiplyPowerTen(ys, raise).add(xs)
                    Return If((xs Xor ys) >= 0, New BigDecimal(bigsum, INFLATED_Renamed, scale1, 0), valueOf(bigsum, scale1, 0))
                End If
            End If
        End Function

        Private Shared Function add(ByVal xs As Long, ByVal scale1 As Integer, ByVal snd As BigInteger, ByVal scale2 As Integer) As BigDecimal
            Dim rscale As Integer = scale1
            Dim sdiff As Long = CLng(rscale) - scale2
            Dim sameSigns As Boolean = (Long.signum(xs) = snd.signum_Renamed)
            Dim sum As BigInteger
            If sdiff < 0 Then
                Dim raise As Integer = checkScale(xs, -sdiff)
                rscale = scale2
                Dim scaledX As Long = longMultiplyPowerTen(xs, raise)
                If scaledX = INFLATED_Renamed Then
                    sum = snd.add(bigMultiplyPowerTen(xs, raise))
                Else
                    sum = snd.add(scaledX)
                End If 'if (sdiff > 0) {
            Else
                Dim raise As Integer = checkScale(snd, sdiff)
                snd = bigMultiplyPowerTen(snd, raise)
                sum = snd.add(xs)
            End If
            Return If(sameSigns, New BigDecimal(sum, INFLATED_Renamed, rscale, 0), valueOf(sum, rscale, 0))
        End Function

        Private Shared Function add(ByVal fst As BigInteger, ByVal scale1 As Integer, ByVal snd As BigInteger, ByVal scale2 As Integer) As BigDecimal
            Dim rscale As Integer = scale1
            Dim sdiff As Long = CLng(rscale) - scale2
            If sdiff <> 0 Then
                If sdiff < 0 Then
                    Dim raise As Integer = checkScale(fst, -sdiff)
                    rscale = scale2
                    fst = bigMultiplyPowerTen(fst, raise)
                Else
                    Dim raise As Integer = checkScale(snd, sdiff)
                    snd = bigMultiplyPowerTen(snd, raise)
                End If
            End If
            Dim sum As BigInteger = fst.add(snd)
            Return If(fst.signum_Renamed = snd.signum_Renamed, New BigDecimal(sum, INFLATED_Renamed, rscale, 0), valueOf(sum, rscale, 0))
        End Function

        Private Shared Function bigMultiplyPowerTen(ByVal value As Long, ByVal n As Integer) As BigInteger
            If n <= 0 Then Return Big java.lang.[Integer].valueOf(value)
            Return bigTenToThe(n).multiply(value)
        End Function

        Private Shared Function bigMultiplyPowerTen(ByVal value As BigInteger, ByVal n As Integer) As BigInteger
            If n <= 0 Then Return value
            If n < LONG_TEN_POWERS_TABLE.Length Then Return value.multiply(LONG_TEN_POWERS_TABLE(n))
            Return value.multiply(bigTenToThe(n))
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is {@code (xs /
        ''' ys)}, with rounding according to the context settings.
        ''' 
        ''' Fast path - used only when (xscale <= yscale && yscale < 18
        '''  && mc.presision<18) {
        ''' </summary>
        Private Shared Function divideSmallFastPath(ByVal xs As Long, ByVal xscale As Integer, ByVal ys As Long, ByVal yscale As Integer, ByVal preferredScale As Long, ByVal mc As MathContext) As BigDecimal
            Dim mcp As Integer = mc.precision
            Dim roundingMode As Integer = mc.roundingMode.oldMode

            Assert(xscale <= yscale) AndAlso (yscale < 18) AndAlso (mcp < 18)
			Dim xraise As Integer = yscale - xscale ' xraise >=0
            Dim scaledX As Long = If(xraise = 0, xs, longMultiplyPowerTen(xs, xraise)) ' can't overflow here!
            Dim quotient As BigDecimal

            Dim cmp As Integer = longCompareMagnitude(scaledX, ys)
            If cmp > 0 Then ' satisfy constraint (b)
                yscale -= 1 ' [that is, divisor *= 10]
                Dim scl As Integer = checkScaleNonZero(preferredScale + yscale - xscale + mcp)
                If checkScaleNonZero(CLng(mcp) + yscale - xscale) > 0 Then
                    ' assert newScale >= xscale
                    Dim raise As Integer = checkScaleNonZero(CLng(mcp) + yscale - xscale)
                    Dim scaledXs As Long
                    scaledXs = longMultiplyPowerTen(xs, raise)
                    If scaledXs = INFLATED_Renamed Then
                        quotient = Nothing
                        If (mcp - 1) >= 0 AndAlso (mcp - 1) < LONG_TEN_POWERS_TABLE.Length Then quotient = multiplyDivideAndRound(LONG_TEN_POWERS_TABLE(mcp - 1), scaledX, ys, scl, roundingMode, checkScaleNonZero(preferredScale))
                        If quotient Is Nothing Then
                            Dim rb As BigInteger = bigMultiplyPowerTen(scaledX, mcp - 1)
                            quotient = divideAndRound(rb, ys, scl, roundingMode, checkScaleNonZero(preferredScale))
                        End If
                    Else
                        quotient = divideAndRound(scaledXs, ys, scl, roundingMode, checkScaleNonZero(preferredScale))
                    End If
                Else
                    Dim newScale As Integer = checkScaleNonZero(CLng(xscale) - mcp)
                    ' assert newScale >= yscale
                    If newScale = yscale Then ' easy case
                        quotient = divideAndRound(xs, ys, scl, roundingMode, checkScaleNonZero(preferredScale))
                    Else
                        Dim raise As Integer = checkScaleNonZero(CLng(newScale) - yscale)
                        Dim scaledYs As Long
                        scaledYs = longMultiplyPowerTen(ys, raise)
                        If scaledYs = INFLATED_Renamed Then
                            Dim rb As BigInteger = bigMultiplyPowerTen(ys, raise)
                            quotient = divideAndRound(Big java.lang.[Integer].valueOf(xs), rb, scl, roundingMode, checkScaleNonZero(preferredScale))
                        Else
                            quotient = divideAndRound(xs, scaledYs, scl, roundingMode, checkScaleNonZero(preferredScale))
                        End If
                    End If
                End If
            Else
                ' abs(scaledX) <= abs(ys)
                ' result is "scaledX * 10^msp / ys"
                Dim scl As Integer = checkScaleNonZero(preferredScale + yscale - xscale + mcp)
                If cmp = 0 Then
                    ' abs(scaleX)== abs(ys) => result will be scaled 10^mcp + correct sign
                    quotient = roundedTenPower(If((scaledX < 0) = (ys < 0), 1, -1), mcp, scl, checkScaleNonZero(preferredScale))
                Else
                    ' abs(scaledX) < abs(ys)
                    Dim scaledXs As Long
                    scaledXs = longMultiplyPowerTen(scaledX, mcp)
                    If scaledXs = INFLATED_Renamed Then
                        quotient = Nothing
                        If mcp < LONG_TEN_POWERS_TABLE.Length Then quotient = multiplyDivideAndRound(LONG_TEN_POWERS_TABLE(mcp), scaledX, ys, scl, roundingMode, checkScaleNonZero(preferredScale))
                        If quotient Is Nothing Then
                            Dim rb As BigInteger = bigMultiplyPowerTen(scaledX, mcp)
                            quotient = divideAndRound(rb, ys, scl, roundingMode, checkScaleNonZero(preferredScale))
                        End If
                    Else
                        quotient = divideAndRound(scaledXs, ys, scl, roundingMode, checkScaleNonZero(preferredScale))
                    End If
                End If
            End If
            ' doRound, here, only affects 1000000000 case.
            Return doRound(quotient, mc)
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is {@code (xs /
        ''' ys)}, with rounding according to the context settings.
        ''' </summary>
        Private Shared Function divide(ByVal xs As Long, ByVal xscale As Integer, ByVal ys As Long, ByVal yscale As Integer, ByVal preferredScale As Long, ByVal mc As MathContext) As BigDecimal
            Dim mcp As Integer = mc.precision
            If xscale <= yscale AndAlso yscale < 18 AndAlso mcp < 18 Then Return divideSmallFastPath(xs, xscale, ys, yscale, preferredScale, mc)
            If compareMagnitudeNormalized(xs, xscale, ys, yscale) > 0 Then ' satisfy constraint (b) yscale -= 1 ' [that is, divisor *= 10]
                Dim roundingMode As Integer = mc.roundingMode.oldMode
                ' In order to find out whether the divide generates the exact result,
                ' we avoid calling the above divide method. 'quotient' holds the
                ' return BigDecimal object whose scale will be set to 'scl'.
                Dim scl As Integer = checkScaleNonZero(preferredScale + yscale - xscale + mcp)
                Dim quotient As BigDecimal
                If checkScaleNonZero(CLng(mcp) + yscale - xscale) > 0 Then
                    Dim raise As Integer = checkScaleNonZero(CLng(mcp) + yscale - xscale)
                    Dim scaledXs As Long
                    scaledXs = longMultiplyPowerTen(xs, raise)
                    If scaledXs = INFLATED_Renamed Then
                        Dim rb As BigInteger = bigMultiplyPowerTen(xs, raise)
                        quotient = divideAndRound(rb, ys, scl, roundingMode, checkScaleNonZero(preferredScale))
                    Else
                        quotient = divideAndRound(scaledXs, ys, scl, roundingMode, checkScaleNonZero(preferredScale))
                    End If
                Else
                    Dim newScale As Integer = checkScaleNonZero(CLng(xscale) - mcp)
                    ' assert newScale >= yscale
                    If newScale = yscale Then ' easy case
                        quotient = divideAndRound(xs, ys, scl, roundingMode, checkScaleNonZero(preferredScale))
                    Else
                        Dim raise As Integer = checkScaleNonZero(CLng(newScale) - yscale)
                        Dim scaledYs As Long
                        scaledYs = longMultiplyPowerTen(ys, raise)
                        If scaledYs = INFLATED_Renamed Then
                            Dim rb As BigInteger = bigMultiplyPowerTen(ys, raise)
                            quotient = divideAndRound(Big java.lang.[Integer].valueOf(xs), rb, scl, roundingMode, checkScaleNonZero(preferredScale))
                        Else
                            quotient = divideAndRound(xs, scaledYs, scl, roundingMode, checkScaleNonZero(preferredScale))
                        End If
                    End If
                End If
                ' doRound, here, only affects 1000000000 case.
                Return doRound(quotient, mc)
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is {@code (xs /
        ''' ys)}, with rounding according to the context settings.
        ''' </summary>
        Private Shared Function divide(ByVal xs As BigInteger, ByVal xscale As Integer, ByVal ys As Long, ByVal yscale As Integer, ByVal preferredScale As Long, ByVal mc As MathContext) As BigDecimal
            ' Normalize dividend & divisor so that both fall into [0.1, 0.999...]
            If (-compareMagnitudeNormalized(ys, yscale, xs, xscale)) > 0 Then ' satisfy constraint (b) yscale -= 1 ' [that is, divisor *= 10]
                Dim mcp As Integer = mc.precision
                Dim roundingMode As Integer = mc.roundingMode.oldMode

                ' In order to find out whether the divide generates the exact result,
                ' we avoid calling the above divide method. 'quotient' holds the
                ' return BigDecimal object whose scale will be set to 'scl'.
                Dim quotient As BigDecimal
                Dim scl As Integer = checkScaleNonZero(preferredScale + yscale - xscale + mcp)
                If checkScaleNonZero(CLng(mcp) + yscale - xscale) > 0 Then
                    Dim raise As Integer = checkScaleNonZero(CLng(mcp) + yscale - xscale)
                    Dim rb As BigInteger = bigMultiplyPowerTen(xs, raise)
                    quotient = divideAndRound(rb, ys, scl, roundingMode, checkScaleNonZero(preferredScale))
                Else
                    Dim newScale As Integer = checkScaleNonZero(CLng(xscale) - mcp)
                    ' assert newScale >= yscale
                    If newScale = yscale Then ' easy case
                        quotient = divideAndRound(xs, ys, scl, roundingMode, checkScaleNonZero(preferredScale))
                    Else
                        Dim raise As Integer = checkScaleNonZero(CLng(newScale) - yscale)
                        Dim scaledYs As Long
                        scaledYs = longMultiplyPowerTen(ys, raise)
                        If scaledYs = INFLATED_Renamed Then
                            Dim rb As BigInteger = bigMultiplyPowerTen(ys, raise)
                            quotient = divideAndRound(xs, rb, scl, roundingMode, checkScaleNonZero(preferredScale))
                        Else
                            quotient = divideAndRound(xs, scaledYs, scl, roundingMode, checkScaleNonZero(preferredScale))
                        End If
                    End If
                End If
                ' doRound, here, only affects 1000000000 case.
                Return doRound(quotient, mc)
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is {@code (xs /
        ''' ys)}, with rounding according to the context settings.
        ''' </summary>
        Private Shared Function divide(ByVal xs As Long, ByVal xscale As Integer, ByVal ys As BigInteger, ByVal yscale As Integer, ByVal preferredScale As Long, ByVal mc As MathContext) As BigDecimal
            ' Normalize dividend & divisor so that both fall into [0.1, 0.999...]
            If compareMagnitudeNormalized(xs, xscale, ys, yscale) > 0 Then ' satisfy constraint (b) yscale -= 1 ' [that is, divisor *= 10]
                Dim mcp As Integer = mc.precision
                Dim roundingMode As Integer = mc.roundingMode.oldMode

                ' In order to find out whether the divide generates the exact result,
                ' we avoid calling the above divide method. 'quotient' holds the
                ' return BigDecimal object whose scale will be set to 'scl'.
                Dim quotient As BigDecimal
                Dim scl As Integer = checkScaleNonZero(preferredScale + yscale - xscale + mcp)
                If checkScaleNonZero(CLng(mcp) + yscale - xscale) > 0 Then
                    Dim raise As Integer = checkScaleNonZero(CLng(mcp) + yscale - xscale)
                    Dim rb As BigInteger = bigMultiplyPowerTen(xs, raise)
                    quotient = divideAndRound(rb, ys, scl, roundingMode, checkScaleNonZero(preferredScale))
                Else
                    Dim newScale As Integer = checkScaleNonZero(CLng(xscale) - mcp)
                    Dim raise As Integer = checkScaleNonZero(CLng(newScale) - yscale)
                    Dim rb As BigInteger = bigMultiplyPowerTen(ys, raise)
                    quotient = divideAndRound(Big java.lang.[Integer].valueOf(xs), rb, scl, roundingMode, checkScaleNonZero(preferredScale))
                End If
                ' doRound, here, only affects 1000000000 case.
                Return doRound(quotient, mc)
        End Function

        ''' <summary>
        ''' Returns a {@code BigDecimal} whose value is {@code (xs /
        ''' ys)}, with rounding according to the context settings.
        ''' </summary>
        Private Shared Function divide(ByVal xs As BigInteger, ByVal xscale As Integer, ByVal ys As BigInteger, ByVal yscale As Integer, ByVal preferredScale As Long, ByVal mc As MathContext) As BigDecimal
            ' Normalize dividend & divisor so that both fall into [0.1, 0.999...]
            If compareMagnitudeNormalized(xs, xscale, ys, yscale) > 0 Then ' satisfy constraint (b) yscale -= 1 ' [that is, divisor *= 10]
                Dim mcp As Integer = mc.precision
                Dim roundingMode As Integer = mc.roundingMode.oldMode

                ' In order to find out whether the divide generates the exact result,
                ' we avoid calling the above divide method. 'quotient' holds the
                ' return BigDecimal object whose scale will be set to 'scl'.
                Dim quotient As BigDecimal
                Dim scl As Integer = checkScaleNonZero(preferredScale + yscale - xscale + mcp)
                If checkScaleNonZero(CLng(mcp) + yscale - xscale) > 0 Then
                    Dim raise As Integer = checkScaleNonZero(CLng(mcp) + yscale - xscale)
                    Dim rb As BigInteger = bigMultiplyPowerTen(xs, raise)
                    quotient = divideAndRound(rb, ys, scl, roundingMode, checkScaleNonZero(preferredScale))
                Else
                    Dim newScale As Integer = checkScaleNonZero(CLng(xscale) - mcp)
                    Dim raise As Integer = checkScaleNonZero(CLng(newScale) - yscale)
                    Dim rb As BigInteger = bigMultiplyPowerTen(ys, raise)
                    quotient = divideAndRound(xs, rb, scl, roundingMode, checkScaleNonZero(preferredScale))
                End If
                ' doRound, here, only affects 1000000000 case.
                Return doRound(quotient, mc)
        End Function

        '    
        '     * performs divideAndRound for (dividend0*dividend1, divisor)
        '     * returns null if quotient can't fit into long value;
        '     
        Private Shared Function multiplyDivideAndRound(ByVal dividend0 As Long, ByVal dividend1 As Long, ByVal divisor As Long, ByVal scale As Integer, ByVal roundingMode As Integer, ByVal preferredScale As Integer) As BigDecimal
            Dim qsign As Integer = java.lang.[Long].signum(dividend0) * java.lang.[Long].signum(dividend1) * java.lang.[Long].signum(divisor)
            dividend0 = System.Math.Abs(dividend0)
            dividend1 = System.Math.Abs(dividend1)
            divisor = System.Math.Abs(divisor)
            ' multiply dividend0 * dividend1
            Dim d0_hi As Long = CLng(CULng(dividend0) >> 32)
            Dim d0_lo As Long = dividend0 And LONG_MASK
            Dim d1_hi As Long = CLng(CULng(dividend1) >> 32)
            Dim d1_lo As Long = dividend1 And LONG_MASK
            Dim product As Long = d0_lo * d1_lo
            Dim d0 As Long = product And LONG_MASK
            Dim d1 As Long = CLng(CULng(product) >> 32)
            product = d0_hi * d1_lo + d1
            d1 = product And LONG_MASK
            Dim d2 As Long = CLng(CULng(product) >> 32)
            product = d0_lo * d1_hi + d1
            d1 = product And LONG_MASK
            d2 += CLng(CULng(product) >> 32)
            Dim d3 As Long = CLng(CULng(d2) >> 32)
            d2 = d2 And LONG_MASK
            product = d0_hi * d1_hi + d2
            d2 = product And LONG_MASK
            d3 = ((CLng(CULng(product) >> 32)) + d3) And LONG_MASK
            Dim dividendHi As Long = make64(d3, d2)
            Dim dividendLo As Long = make64(d1, d0)
            ' divide
            Return divideAndRound128(dividendHi, dividendLo, divisor, qsign, scale, roundingMode, preferredScale)
        End Function

        Private Shared ReadOnly DIV_NUM_BASE As Long = (1L << 32) ' Number base (32 bits).

        '    
        '     * divideAndRound 128-bit value by long divisor.
        '     * returns null if quotient can't fit into long value;
        '     * Specialized version of Knuth's division
        '     
        Private Shared Function divideAndRound128(ByVal dividendHi As Long, ByVal dividendLo As Long, ByVal divisor As Long, ByVal sign As Integer, ByVal scale As Integer, ByVal roundingMode As Integer, ByVal preferredScale As Integer) As BigDecimal
            If dividendHi >= divisor Then Return Nothing

            Dim shift As Integer = java.lang.[Long].numberOfLeadingZeros(divisor)
            divisor <<= shift

            Dim v1 As Long = CLng(CULng(divisor) >> 32)
            Dim v0 As Long = divisor And LONG_MASK

            Dim tmp As Long = dividendLo << shift
            Dim u1 As Long = CLng(CULng(tmp) >> 32)
            Dim u0 As Long = tmp And LONG_MASK

            tmp = (dividendHi << shift) Or (CLng(CULng(dividendLo) >> 64 - shift))
            Dim u2 As Long = tmp And LONG_MASK
            Dim q1, r_tmp As Long
            If v1 = 1 Then
                q1 = tmp
                r_tmp = 0
            ElseIf tmp >= 0 Then
                q1 = tmp \ v1
                r_tmp = tmp - q1 * v1
            Else
                Dim rq As Long() = divRemNegativeLong(tmp, v1)
                q1 = rq(1)
                r_tmp = rq(0)
            End If

            Do While q1 >= DIV_NUM_BASE OrElse unsignedLongCompare(q1 * v0, make64(r_tmp, u1))
                q1 -= 1
                r_tmp += v1
                If r_tmp >= DIV_NUM_BASE Then Exit Do
            Loop

            tmp = mulsub(u2, u1, v1, v0, q1)
            u1 = tmp And LONG_MASK
            Dim q0 As Long
            If v1 = 1 Then
                q0 = tmp
                r_tmp = 0
            ElseIf tmp >= 0 Then
                q0 = tmp \ v1
                r_tmp = tmp - q0 * v1
            Else
                Dim rq As Long() = divRemNegativeLong(tmp, v1)
                q0 = rq(1)
                r_tmp = rq(0)
            End If

            Do While q0 >= DIV_NUM_BASE OrElse unsignedLongCompare(q0 * v0, make64(r_tmp, u0))
                q0 -= 1
                r_tmp += v1
                If r_tmp >= DIV_NUM_BASE Then Exit Do
            Loop

            If CInt(q1) < 0 Then
                ' result (which is positive and unsigned here)
                ' can't fit into long due to sign bit is used for value
                Dim mq As New MutableBigInteger(New Integer() {CInt(q1), CInt(q0)})
                If roundingMode = ROUND_DOWN AndAlso scale = preferredScale Then Return mq.toBigDecimal(sign, scale)
                Dim r As Long = CInt(CUInt(mulsub(u1, u0, v1, v0, q0)) >> shift)
                If r <> 0 Then
                    If needIncrement(CLng(CULng(divisor) >> shift), roundingMode, sign, mq, r) Then mq.add(MutableBig java.lang.[Integer].ONE)
                    Return mq.toBigDecimal(sign, scale)
                Else
                    If preferredScale <> scale Then
                        Dim intVal As BigInteger = mq.toBigInteger(sign)
                        Return createAndStripZerosToMatchScale(intVal, scale, preferredScale)
                    Else
                        Return mq.toBigDecimal(sign, scale)
                    End If
                End If
            End If

            Dim q As Long = make64(q1, q0)
            q *= sign

            If roundingMode = ROUND_DOWN AndAlso scale = preferredScale Then Return valueOf(q, scale)

            Dim r As Long = CInt(CUInt(mulsub(u1, u0, v1, v0, q0)) >> shift)
            If r <> 0 Then
                Dim increment As Boolean = needIncrement(CLng(CULng(divisor) >> shift), roundingMode, sign, q, r)
                Return valueOf((If(increment, q + sign, q)), scale)
            Else
                If preferredScale <> scale Then
                    Return createAndStripZerosToMatchScale(q, scale, preferredScale)
                Else
                    Return valueOf(q, scale)
                End If
            End If
        End Function

        '    
        '     * calculate divideAndRound for ldividend*10^raise / divisor
        '     * when abs(dividend)==abs(divisor);
        '     
        Private Shared Function roundedTenPower(ByVal qsign As Integer, ByVal raise As Integer, ByVal scale As Integer, ByVal preferredScale As Integer) As BigDecimal
            If scale > preferredScale Then
                Dim diff As Integer = scale - preferredScale
                If diff < raise Then
                    Return scaledTenPow(raise - diff, qsign, preferredScale)
                Else
                    Return valueOf(qsign, scale - raise)
                End If
            Else
                Return scaledTenPow(raise, qsign, scale)
            End If
        End Function

        Shared Function scaledTenPow(ByVal n As Integer, ByVal sign As Integer, ByVal scale As Integer) As BigDecimal
            If n < LONG_TEN_POWERS_TABLE.Length Then
                Return valueOf(sign * LONG_TEN_POWERS_TABLE(n), scale)
            Else
                Dim unscaledVal As BigInteger = bigTenToThe(n)
                If sign = -1 Then unscaledVal = unscaledVal.negate()
                Return New BigDecimal(unscaledVal, INFLATED_Renamed, scale, n + 1)
            End If
        End Function

        ''' <summary>
        ''' Calculate the quotient and remainder of dividing a negative long by
        ''' another java.lang.[Long].
        ''' </summary>
        ''' <param name="n"> the numerator; must be negative </param>
        ''' <param name="d"> the denominator; must not be unity </param>
        ''' <returns> a two-element {@long} array with the remainder and quotient in
        '''         the initial and final elements, respectively </returns>
        Private Shared Function divRemNegativeLong(ByVal n As Long, ByVal d As Long) As Long()
            Debug.Assert(n < 0, "Non-negative numerator " & n)
            Debug.Assert(d <> 1, "Unity denominator")

            ' Approximate the quotient and remainder
            Dim q As Long = (CLng(CULng(n) >> 1)) \ (CLng(CULng(d) >> 1))
            Dim r As Long = n - q * d

            ' Correct the approximation
            Do While r < 0
                r += d
                q -= 1
            Loop
            Do While r >= d
                r -= d
                q += 1
            Loop

            ' n - q*d == r && 0 <= r < d, hence we're done.
            Return New Long() {r, q}
        End Function

        Private Shared Function make64(ByVal hi As Long, ByVal lo As Long) As Long
            Return hi << 32 Or lo
        End Function

        Private Shared Function mulsub(ByVal u1 As Long, ByVal u0 As Long, ByVal v1 As Long, ByVal v0 As Long, ByVal q0 As Long) As Long
            Dim tmp As Long = u0 - q0 * v0
            Return make64(u1 + (CLng(CULng(tmp) >> 32)) - q0 * v1, tmp And LONG_MASK)
        End Function

        Private Shared Function unsignedLongCompare(ByVal one As Long, ByVal two As Long) As Boolean
            Return (one + java.lang.[Long].MIN_VALUE) > (two + java.lang.[Long].MIN_VALUE)
        End Function

        Private Shared Function unsignedLongCompareEq(ByVal one As Long, ByVal two As Long) As Boolean
            Return (one + java.lang.[Long].MIN_VALUE) >= (two + java.lang.[Long].MIN_VALUE)
        End Function


        ' Compare Normalize dividend & divisor so that both fall into [0.1, 0.999...]
        Private Shared Function compareMagnitudeNormalized(ByVal xs As Long, ByVal xscale As Integer, ByVal ys As Long, ByVal yscale As Integer) As Integer
            ' assert xs!=0 && ys!=0
            Dim sdiff As Integer = xscale - yscale
            If sdiff <> 0 Then
                If sdiff < 0 Then
                    xs = longMultiplyPowerTen(xs, -sdiff) ' sdiff > 0
                Else
                    ys = longMultiplyPowerTen(ys, sdiff)
                End If
            End If
            If xs <> INFLATED_Renamed Then
                Return If(ys <> INFLATED_Renamed, longCompareMagnitude(xs, ys), -1)
            Else
                Return 1
            End If
        End Function

        ' Compare Normalize dividend & divisor so that both fall into [0.1, 0.999...]
        Private Shared Function compareMagnitudeNormalized(ByVal xs As Long, ByVal xscale As Integer, ByVal ys As BigInteger, ByVal yscale As Integer) As Integer
            ' assert "ys can't be represented as long"
            If xs = 0 Then Return -1
            Dim sdiff As Integer = xscale - yscale
            If sdiff < 0 Then
                If longMultiplyPowerTen(xs, -sdiff) = INFLATED_Renamed Then Return bigMultiplyPowerTen(xs, -sdiff).compareMagnitude(ys)
            End If
            Return -1
        End Function

        ' Compare Normalize dividend & divisor so that both fall into [0.1, 0.999...]
        Private Shared Function compareMagnitudeNormalized(ByVal xs As BigInteger, ByVal xscale As Integer, ByVal ys As BigInteger, ByVal yscale As Integer) As Integer
            Dim sdiff As Integer = xscale - yscale
            If sdiff < 0 Then
                Return bigMultiplyPowerTen(xs, -sdiff).compareMagnitude(ys) ' sdiff >= 0
            Else
                Return xs.compareMagnitude(bigMultiplyPowerTen(ys, sdiff))
            End If
        End Function

        Private Shared Function multiply(ByVal x As Long, ByVal y As Long) As Long
            Dim product As Long = x * y
            Dim ax As Long = System.Math.Abs(x)
            Dim ay As Long = System.Math.Abs(y)
            If (CInt(CUInt((ax Or ay)) >> 31 = 0)) OrElse (y = 0) OrElse (product \ y = x) Then Return product
            Return INFLATED_Renamed
        End Function

        Private Shared Function multiply(ByVal x As Long, ByVal y As Long, ByVal scale As Integer) As BigDecimal
            Dim product As Long = multiply(x, y)
            If product <> INFLATED_Renamed Then Return valueOf(product, scale)
            Return New BigDecimal(Big java.lang.[Integer].valueOf(x) * y, INFLATED_Renamed, scale, 0)
        End Function

        Private Shared Function multiply(ByVal x As Long, ByVal y As BigInteger, ByVal scale As Integer) As BigDecimal
            If x = 0 Then Return zeroValueOf(scale)
            Return New BigDecimal(y.multiply(x), INFLATED_Renamed, scale, 0)
        End Function

        Private Shared Function multiply(ByVal x As BigInteger, ByVal y As BigInteger, ByVal scale As Integer) As BigDecimal
            Return New BigDecimal(x.multiply(y), INFLATED_Renamed, scale, 0)
        End Function

        ''' <summary>
        ''' Multiplies two long values and rounds according {@code MathContext}
        ''' </summary>
        Private Shared Function multiplyAndRound(ByVal x As Long, ByVal y As Long, ByVal scale As Integer, ByVal mc As MathContext) As BigDecimal
            Dim product As Long = multiply(x, y)
            If product <> INFLATED_Renamed Then Return doRound(product, scale, mc)
            ' attempt to do it in 128 bits
            Dim rsign As Integer = 1
            If x < 0 Then
                x = -x
                rsign = -1
            End If
            If y < 0 Then
                y = -y
                rsign *= -1
            End If
            ' multiply dividend0 * dividend1
            Dim m0_hi As Long = CLng(CULng(x) >> 32)
            Dim m0_lo As Long = x And LONG_MASK
            Dim m1_hi As Long = CLng(CULng(y) >> 32)
            Dim m1_lo As Long = y And LONG_MASK
            product = m0_lo * m1_lo
            Dim m0 As Long = product And LONG_MASK
            Dim m1 As Long = CLng(CULng(product) >> 32)
            product = m0_hi * m1_lo + m1
            m1 = product And LONG_MASK
            Dim m2 As Long = CLng(CULng(product) >> 32)
            product = m0_lo * m1_hi + m1
            m1 = product And LONG_MASK
            m2 += CLng(CULng(product) >> 32)
            Dim m3 As Long = CLng(CULng(m2) >> 32)
            m2 = m2 And LONG_MASK
            product = m0_hi * m1_hi + m2
            m2 = product And LONG_MASK
            m3 = ((CLng(CULng(product) >> 32)) + m3) And LONG_MASK
            Dim mHi As Long = make64(m3, m2)
            Dim mLo As Long = make64(m1, m0)
            Dim res As BigDecimal = doRound128(mHi, mLo, rsign, scale, mc)
            If res IsNot Nothing Then Return res
            res = New BigDecimal(Big java.lang.[Integer].valueOf(x) * (y * rsign), INFLATED_Renamed, scale, 0)
            Return doRound(res, mc)
        End Function

        Private Shared Function multiplyAndRound(ByVal x As Long, ByVal y As BigInteger, ByVal scale As Integer, ByVal mc As MathContext) As BigDecimal
            If x = 0 Then Return zeroValueOf(scale)
            Return doRound(y.multiply(x), scale, mc)
        End Function

        Private Shared Function multiplyAndRound(ByVal x As BigInteger, ByVal y As BigInteger, ByVal scale As Integer, ByVal mc As MathContext) As BigDecimal
            Return doRound(x.multiply(y), scale, mc)
        End Function

        ''' <summary>
        ''' rounds 128-bit value according {@code MathContext}
        ''' returns null if result can't be repsented as compact BigDecimal.
        ''' </summary>
        Private Shared Function doRound128(ByVal hi As Long, ByVal lo As Long, ByVal sign As Integer, ByVal scale As Integer, ByVal mc As MathContext) As BigDecimal
            Dim mcp As Integer = mc.precision
            Dim drop As Integer
            Dim res As BigDecimal = Nothing
            drop = precision(hi, lo) - mcp
            If (drop > 0) AndAlso (drop < LONG_TEN_POWERS_TABLE.Length) Then
                scale = checkScaleNonZero(CLng(scale) - drop)
                res = divideAndRound128(hi, lo, LONG_TEN_POWERS_TABLE(drop), sign, scale, mc.roundingMode.oldMode, scale)
            End If
            If res IsNot Nothing Then Return doRound(res, mc)
            Return Nothing
        End Function

        Private Shared ReadOnly LONGLONG_TEN_POWERS_TABLE As Long()() = {New Long() {0L, &H8AC7230489E80000L}, New Long() {&H5L, &H6BC75E2R63100000L }, New Long() {&H36L, &H35C9ADC5Rea00000L }, New Long() {&H21EL, &H19E0C9BAB2400000L}, New Long() {&H152DL, &H2C7E14AF6800000L}, New Long() {&HD3C2L, &H1BCECCEDA1000000L}, New Long() {&H84595L, &H161401484A000000L}, New Long() {&H52B7R2L, &HDCC80CD2E4000000L }, New Long() {&H33B2E3CL, &H9FD0803CE8000000L}, New Long() {&H204FCE5EL, &H3E25026110000000L}, New Long() {&H1431E0FAEL, &H6D7217CAA0000000L}, New Long() {&HC9F2C9CD0L, &H4674EDEA40000000L}, New Long() {&H7E37BE2022L, &HC0914B2680000000L}, New Long() {&H4EE2R6d415bL, &H85ACEF8100000000L }, New Long() {&H314DC6448R93L, &H38C15B0A00000000L }, New Long() {&H1ED09BEAD87C0L, &H378D8E6400000000L}, New Long() {&H13426172C74R82L, &H2B878FE800000000L }, New Long() {&HC097CE7BC90715L, &HB34B9F1000000000L}, New Long() {&H785EE10R5da46d9L, &HF436A000000000L }, New Long() {&H4B3B4CA85A86C47AL, &H98A224000000000L}}

        '    
        '     * returns precision of 128-bit value
        '     
        Private Shared Function precision(ByVal hi As Long, ByVal lo As Long) As Integer
            If hi = 0 Then
                If lo >= 0 Then Return longDigitLength(lo)
                Return If(unsignedLongCompareEq(lo, LONGLONG_TEN_POWERS_TABLE(0)(1)), 20, 19)
                ' 0x8AC7230489E80000L  = unsigned 2^19
            End If
            Dim r As Integer = CInt(CUInt(((128 - java.lang.[Long].numberOfLeadingZeros(hi) + 1) * 1233)) >> 12)
            Dim idx As Integer = r - 19
            Return If(idx >= LONGLONG_TEN_POWERS_TABLE.Length OrElse longLongCompareMagnitude(hi, lo, LONGLONG_TEN_POWERS_TABLE(idx)(0), LONGLONG_TEN_POWERS_TABLE(idx)(1)), r, r + 1)
        End Function

        '    
        '     * returns true if 128 bit number <hi0,lo0> is less then <hi1,lo1>
        '     * hi0 & hi1 should be non-negative
        '     
        Private Shared Function longLongCompareMagnitude(ByVal hi0 As Long, ByVal lo0 As Long, ByVal hi1 As Long, ByVal lo1 As Long) As Boolean
            If hi0 <> hi1 Then Return hi0 < hi1
            Return (lo0 + java.lang.[Long].MIN_VALUE) < (lo1 + java.lang.[Long].MIN_VALUE)
        End Function

        Private Shared Function divide(ByVal dividend As Long, ByVal dividendScale As Integer, ByVal divisor As Long, ByVal divisorScale As Integer, ByVal scale As Integer, ByVal roundingMode As Integer) As BigDecimal
            If checkScale(dividend, CLng(scale) + divisorScale) > dividendScale Then
                Dim newScale As Integer = scale + divisorScale
                Dim raise As Integer = newScale - dividendScale
                If raise < LONG_TEN_POWERS_TABLE.Length Then
                    Dim xs As Long = dividend
                    xs = longMultiplyPowerTen(xs, raise)
                    If xs <> INFLATED_Renamed Then Return divideAndRound(xs, divisor, scale, roundingMode, scale)
                    Dim q As BigDecimal = multiplyDivideAndRound(LONG_TEN_POWERS_TABLE(raise), dividend, divisor, scale, roundingMode, scale)
                    If q IsNot Nothing Then Return q
                End If
                Dim scaledDividend As BigInteger = bigMultiplyPowerTen(dividend, raise)
                Return divideAndRound(scaledDividend, divisor, scale, roundingMode, scale)
            Else
                Dim newScale As Integer = checkScale(divisor, CLng(dividendScale) - scale)
                Dim raise As Integer = newScale - divisorScale
                If raise < LONG_TEN_POWERS_TABLE.Length Then
                    Dim ys As Long = divisor
                    ys = longMultiplyPowerTen(ys, raise)
                    If ys <> INFLATED_Renamed Then Return divideAndRound(dividend, ys, scale, roundingMode, scale)
                End If
                Dim scaledDivisor As BigInteger = bigMultiplyPowerTen(divisor, raise)
                Return divideAndRound(Big java.lang.[Integer].valueOf(dividend), scaledDivisor, scale, roundingMode, scale)
            End If
        End Function

        Private Shared Function divide(ByVal dividend As BigInteger, ByVal dividendScale As Integer, ByVal divisor As Long, ByVal divisorScale As Integer, ByVal scale As Integer, ByVal roundingMode As Integer) As BigDecimal
            If checkScale(dividend, CLng(scale) + divisorScale) > dividendScale Then
                Dim newScale As Integer = scale + divisorScale
                Dim raise As Integer = newScale - dividendScale
                Dim scaledDividend As BigInteger = bigMultiplyPowerTen(dividend, raise)
                Return divideAndRound(scaledDividend, divisor, scale, roundingMode, scale)
            Else
                Dim newScale As Integer = checkScale(divisor, CLng(dividendScale) - scale)
                Dim raise As Integer = newScale - divisorScale
                If raise < LONG_TEN_POWERS_TABLE.Length Then
                    Dim ys As Long = divisor
                    ys = longMultiplyPowerTen(ys, raise)
                    If ys <> INFLATED_Renamed Then Return divideAndRound(dividend, ys, scale, roundingMode, scale)
                End If
                Dim scaledDivisor As BigInteger = bigMultiplyPowerTen(divisor, raise)
                Return divideAndRound(dividend, scaledDivisor, scale, roundingMode, scale)
            End If
        End Function

        Private Shared Function divide(ByVal dividend As Long, ByVal dividendScale As Integer, ByVal divisor As BigInteger, ByVal divisorScale As Integer, ByVal scale As Integer, ByVal roundingMode As Integer) As BigDecimal
            If checkScale(dividend, CLng(scale) + divisorScale) > dividendScale Then
                Dim newScale As Integer = scale + divisorScale
                Dim raise As Integer = newScale - dividendScale
                Dim scaledDividend As BigInteger = bigMultiplyPowerTen(dividend, raise)
                Return divideAndRound(scaledDividend, divisor, scale, roundingMode, scale)
            Else
                Dim newScale As Integer = checkScale(divisor, CLng(dividendScale) - scale)
                Dim raise As Integer = newScale - divisorScale
                Dim scaledDivisor As BigInteger = bigMultiplyPowerTen(divisor, raise)
                Return divideAndRound(Big java.lang.[Integer].valueOf(dividend), scaledDivisor, scale, roundingMode, scale)
            End If
        End Function

        Private Shared Function divide(ByVal dividend As BigInteger, ByVal dividendScale As Integer, ByVal divisor As BigInteger, ByVal divisorScale As Integer, ByVal scale As Integer, ByVal roundingMode As Integer) As BigDecimal
            If checkScale(dividend, CLng(scale) + divisorScale) > dividendScale Then
                Dim newScale As Integer = scale + divisorScale
                Dim raise As Integer = newScale - dividendScale
                Dim scaledDividend As BigInteger = bigMultiplyPowerTen(dividend, raise)
                Return divideAndRound(scaledDividend, divisor, scale, roundingMode, scale)
            Else
                Dim newScale As Integer = checkScale(divisor, CLng(dividendScale) - scale)
                Dim raise As Integer = newScale - divisorScale
                Dim scaledDivisor As BigInteger = bigMultiplyPowerTen(divisor, raise)
                Return divideAndRound(dividend, scaledDivisor, scale, roundingMode, scale)
            End If
        End Function

    End Class

End Namespace
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

'
' * Portions Copyright IBM Corporation, 2001. All Rights Reserved.
' 
Imports java.lang

Namespace java.math

    ''' <summary>
    ''' Specifies a <i>rounding behavior</i> for numerical operations
    ''' capable of discarding precision. Each rounding mode indicates how
    ''' the least significant returned digit of a rounded result is to be
    ''' calculated.  If fewer digits are returned than the digits needed to
    ''' represent the exact numerical result, the discarded digits will be
    ''' referred to as the <i>discarded fraction</i> regardless the digits'
    ''' contribution to the value of the number.  In other words,
    ''' considered as a numerical value, the discarded fraction could have
    ''' an absolute value greater than one.
    ''' 
    ''' <p>Each rounding mode description includes a table listing how
    ''' different two-digit decimal values would round to a one digit
    ''' decimal value under the rounding mode in question.  The result
    ''' column in the tables could be gotten by creating a
    ''' {@code BigDecimal} number with the specified value, forming a
    ''' <seealso cref="MathContext"/> object with the proper settings
    ''' ({@code precision} set to {@code 1}, and the
    ''' {@code roundingMode} set to the rounding mode in question), and
    ''' calling <seealso cref="BigDecimal#round round"/> on this number with the
    ''' proper {@code MathContext}.  A summary table showing the results
    ''' of these rounding operations for all rounding modes appears below.
    ''' 
    ''' <table border>
    ''' <caption><b>Summary of Rounding Operations Under Different Rounding Modes</b></caption>
    ''' <tr><th></th><th colspan=8>Result of rounding input to one digit with the given
    '''                           rounding mode</th>
    ''' <tr valign=top>
    ''' <th>Input Number</th>         <th>{@code UP}</th>
    '''                                           <th>{@code DOWN}</th>
    '''                                                        <th>{@code CEILING}</th>
    '''                                                                       <th>{@code FLOOR}</th>
    '''                                                                                    <th>{@code HALF_UP}</th>
    '''                                                                                                   <th>{@code HALF_DOWN}</th>
    '''                                                                                                                    <th>{@code HALF_EVEN}</th>
    '''                                                                                                                                     <th>{@code UNNECESSARY}</th>
    ''' 
    ''' <tr align=right><td>5.5</td>  <td>6</td>  <td>5</td>    <td>6</td>    <td>5</td>  <td>6</td>      <td>5</td>       <td>6</td>       <td>throw {@code ArithmeticException}</td>
    ''' <tr align=right><td>2.5</td>  <td>3</td>  <td>2</td>    <td>3</td>    <td>2</td>  <td>3</td>      <td>2</td>       <td>2</td>       <td>throw {@code ArithmeticException}</td>
    ''' <tr align=right><td>1.6</td>  <td>2</td>  <td>1</td>    <td>2</td>    <td>1</td>  <td>2</td>      <td>2</td>       <td>2</td>       <td>throw {@code ArithmeticException}</td>
    ''' <tr align=right><td>1.1</td>  <td>2</td>  <td>1</td>    <td>2</td>    <td>1</td>  <td>1</td>      <td>1</td>       <td>1</td>       <td>throw {@code ArithmeticException}</td>
    ''' <tr align=right><td>1.0</td>  <td>1</td>  <td>1</td>    <td>1</td>    <td>1</td>  <td>1</td>      <td>1</td>       <td>1</td>       <td>1</td>
    ''' <tr align=right><td>-1.0</td> <td>-1</td> <td>-1</td>   <td>-1</td>   <td>-1</td> <td>-1</td>     <td>-1</td>      <td>-1</td>      <td>-1</td>
    ''' <tr align=right><td>-1.1</td> <td>-2</td> <td>-1</td>   <td>-1</td>   <td>-2</td> <td>-1</td>     <td>-1</td>      <td>-1</td>      <td>throw {@code ArithmeticException}</td>
    ''' <tr align=right><td>-1.6</td> <td>-2</td> <td>-1</td>   <td>-1</td>   <td>-2</td> <td>-2</td>     <td>-2</td>      <td>-2</td>      <td>throw {@code ArithmeticException}</td>
    ''' <tr align=right><td>-2.5</td> <td>-3</td> <td>-2</td>   <td>-2</td>   <td>-3</td> <td>-3</td>     <td>-2</td>      <td>-2</td>      <td>throw {@code ArithmeticException}</td>
    ''' <tr align=right><td>-5.5</td> <td>-6</td> <td>-5</td>   <td>-5</td>   <td>-6</td> <td>-6</td>     <td>-5</td>      <td>-6</td>      <td>throw {@code ArithmeticException}</td>
    ''' </table>
    ''' 
    ''' 
    ''' <p>This {@code enum} is intended to replace the integer-based
    ''' enumeration of rounding mode constants in <seealso cref="BigDecimal"/>
    ''' (<seealso cref="BigDecimal#ROUND_UP"/>, <seealso cref="BigDecimal#ROUND_DOWN"/>,
    ''' etc. ).
    ''' </summary>
    ''' <seealso cref=     BigDecimal </seealso>
    ''' <seealso cref=     MathContext
    ''' @author  Josh Bloch
    ''' @author  Mike Cowlishaw
    ''' @author  Joseph D. Darcy
    ''' @since 1.5 </seealso>
    Public Class RoundingMode

        ''' <summary>
        ''' Rounding mode to round away from zero.  Always increments the
        ''' digit prior to a non-zero discarded fraction.  Note that this
        ''' rounding mode never decreases the magnitude of the calculated
        ''' value.
        ''' 
        ''' <p>Example:
        ''' <table border>
        ''' <caption><b>Rounding mode UP Examples</b></caption>
        ''' <tr valign=top><th>Input Number</th>
        '''    <th>Input rounded to one digit<br> with {@code UP} rounding
        ''' <tr align=right><td>5.5</td>  <td>6</td>
        ''' <tr align=right><td>2.5</td>  <td>3</td>
        ''' <tr align=right><td>1.6</td>  <td>2</td>
        ''' <tr align=right><td>1.1</td>  <td>2</td>
        ''' <tr align=right><td>1.0</td>  <td>1</td>
        ''' <tr align=right><td>-1.0</td> <td>-1</td>
        ''' <tr align=right><td>-1.1</td> <td>-2</td>
        ''' <tr align=right><td>-1.6</td> <td>-2</td>
        ''' <tr align=right><td>-2.5</td> <td>-3</td>
        ''' <tr align=right><td>-5.5</td> <td>-6</td>
        ''' </table>
        ''' </summary>
        Public Shared ReadOnly Property UP As RoundingMode = BigDecimal.ROUND_UP

        ''' <summary>
        ''' Rounding mode to round towards zero.  Never increments the digit
        ''' prior to a discarded fraction (i.e., truncates).  Note that this
        ''' rounding mode never increases the magnitude of the calculated value.
        ''' 
        ''' <p>Example:
        ''' <table border>
        ''' <caption><b>Rounding mode DOWN Examples</b></caption>
        ''' <tr valign=top><th>Input Number</th>
        '''    <th>Input rounded to one digit<br> with {@code DOWN} rounding
        ''' <tr align=right><td>5.5</td>  <td>5</td>
        ''' <tr align=right><td>2.5</td>  <td>2</td>
        ''' <tr align=right><td>1.6</td>  <td>1</td>
        ''' <tr align=right><td>1.1</td>  <td>1</td>
        ''' <tr align=right><td>1.0</td>  <td>1</td>
        ''' <tr align=right><td>-1.0</td> <td>-1</td>
        ''' <tr align=right><td>-1.1</td> <td>-1</td>
        ''' <tr align=right><td>-1.6</td> <td>-1</td>
        ''' <tr align=right><td>-2.5</td> <td>-2</td>
        ''' <tr align=right><td>-5.5</td> <td>-5</td>
        ''' </table>
        ''' </summary>
        Public Shared ReadOnly Property DOWN As RoundingMode = BigDecimal.ROUND_DOWN

        ''' <summary>
        ''' Rounding mode to round towards positive infinity.  If the
        ''' result is positive, behaves as for {@code RoundingMode.UP};
        ''' if negative, behaves as for {@code RoundingMode.DOWN}.  Note
        ''' that this rounding mode never decreases the calculated value.
        ''' 
        ''' <p>Example:
        ''' <table border>
        ''' <caption><b>Rounding mode CEILING Examples</b></caption>
        ''' <tr valign=top><th>Input Number</th>
        '''    <th>Input rounded to one digit<br> with {@code CEILING} rounding
        ''' <tr align=right><td>5.5</td>  <td>6</td>
        ''' <tr align=right><td>2.5</td>  <td>3</td>
        ''' <tr align=right><td>1.6</td>  <td>2</td>
        ''' <tr align=right><td>1.1</td>  <td>2</td>
        ''' <tr align=right><td>1.0</td>  <td>1</td>
        ''' <tr align=right><td>-1.0</td> <td>-1</td>
        ''' <tr align=right><td>-1.1</td> <td>-1</td>
        ''' <tr align=right><td>-1.6</td> <td>-1</td>
        ''' <tr align=right><td>-2.5</td> <td>-2</td>
        ''' <tr align=right><td>-5.5</td> <td>-5</td>
        ''' </table>
        ''' </summary>
        Public Shared ReadOnly Property CEILING As RoundingMode = BigDecimal.ROUND_CEILING

        ''' <summary>
        ''' Rounding mode to round towards negative infinity.  If the
        ''' result is positive, behave as for {@code RoundingMode.DOWN};
        ''' if negative, behave as for {@code RoundingMode.UP}.  Note that
        ''' this rounding mode never increases the calculated value.
        ''' 
        ''' <p>Example:
        ''' <table border>
        ''' <caption><b>Rounding mode FLOOR Examples</b></caption>
        ''' <tr valign=top><th>Input Number</th>
        '''    <th>Input rounded to one digit<br> with {@code FLOOR} rounding
        ''' <tr align=right><td>5.5</td>  <td>5</td>
        ''' <tr align=right><td>2.5</td>  <td>2</td>
        ''' <tr align=right><td>1.6</td>  <td>1</td>
        ''' <tr align=right><td>1.1</td>  <td>1</td>
        ''' <tr align=right><td>1.0</td>  <td>1</td>
        ''' <tr align=right><td>-1.0</td> <td>-1</td>
        ''' <tr align=right><td>-1.1</td> <td>-2</td>
        ''' <tr align=right><td>-1.6</td> <td>-2</td>
        ''' <tr align=right><td>-2.5</td> <td>-3</td>
        ''' <tr align=right><td>-5.5</td> <td>-6</td>
        ''' </table>
        ''' </summary>
        Public Shared ReadOnly Property FLOOR As RoundingMode = BigDecimal.ROUND_FLOOR

        ''' <summary>
        ''' Rounding mode to round towards {@literal "nearest neighbor"}
        ''' unless both neighbors are equidistant, in which case round up.
        ''' Behaves as for {@code RoundingMode.UP} if the discarded
        ''' fraction is &ge; 0.5; otherwise, behaves as for
        ''' {@code RoundingMode.DOWN}.  Note that this is the rounding
        ''' mode commonly taught at school.
        ''' 
        ''' <p>Example:
        ''' <table border>
        ''' <caption><b>Rounding mode HALF_UP Examples</b></caption>
        ''' <tr valign=top><th>Input Number</th>
        '''    <th>Input rounded to one digit<br> with {@code HALF_UP} rounding
        ''' <tr align=right><td>5.5</td>  <td>6</td>
        ''' <tr align=right><td>2.5</td>  <td>3</td>
        ''' <tr align=right><td>1.6</td>  <td>2</td>
        ''' <tr align=right><td>1.1</td>  <td>1</td>
        ''' <tr align=right><td>1.0</td>  <td>1</td>
        ''' <tr align=right><td>-1.0</td> <td>-1</td>
        ''' <tr align=right><td>-1.1</td> <td>-1</td>
        ''' <tr align=right><td>-1.6</td> <td>-2</td>
        ''' <tr align=right><td>-2.5</td> <td>-3</td>
        ''' <tr align=right><td>-5.5</td> <td>-6</td>
        ''' </table>
        ''' </summary>
        Public Shared ReadOnly Property HALF_UP As RoundingMode = BigDecimal.ROUND_HALF_UP

        ''' <summary>
        ''' Rounding mode to round towards {@literal "nearest neighbor"}
        ''' unless both neighbors are equidistant, in which case round
        ''' down.  Behaves as for {@code RoundingMode.UP} if the discarded
        ''' fraction is &gt; 0.5; otherwise, behaves as for
        ''' {@code RoundingMode.DOWN}.
        ''' 
        ''' <p>Example:
        ''' <table border>
        ''' <caption><b>Rounding mode HALF_DOWN Examples</b></caption>
        ''' <tr valign=top><th>Input Number</th>
        '''    <th>Input rounded to one digit<br> with {@code HALF_DOWN} rounding
        ''' <tr align=right><td>5.5</td>  <td>5</td>
        ''' <tr align=right><td>2.5</td>  <td>2</td>
        ''' <tr align=right><td>1.6</td>  <td>2</td>
        ''' <tr align=right><td>1.1</td>  <td>1</td>
        ''' <tr align=right><td>1.0</td>  <td>1</td>
        ''' <tr align=right><td>-1.0</td> <td>-1</td>
        ''' <tr align=right><td>-1.1</td> <td>-1</td>
        ''' <tr align=right><td>-1.6</td> <td>-2</td>
        ''' <tr align=right><td>-2.5</td> <td>-2</td>
        ''' <tr align=right><td>-5.5</td> <td>-5</td>
        ''' </table>
        ''' </summary>
        Public Shared ReadOnly Property HALF_DOWN As RoundingMode = BigDecimal.ROUND_HALF_DOWN

        ''' <summary>
        ''' Rounding mode to round towards the {@literal "nearest neighbor"}
        ''' unless both neighbors are equidistant, in which case, round
        ''' towards the even neighbor.  Behaves as for
        ''' {@code RoundingMode.HALF_UP} if the digit to the left of the
        ''' discarded fraction is odd; behaves as for
        ''' {@code RoundingMode.HALF_DOWN} if it's even.  Note that this
        ''' is the rounding mode that statistically minimizes cumulative
        ''' error when applied repeatedly over a sequence of calculations.
        ''' It is sometimes known as {@literal "Banker's rounding,"} and is
        ''' chiefly used in the USA.  This rounding mode is analogous to
        ''' the rounding policy used for {@code float} and {@code double}
        ''' arithmetic in Java.
        ''' 
        ''' <p>Example:
        ''' <table border>
        ''' <caption><b>Rounding mode HALF_EVEN Examples</b></caption>
        ''' <tr valign=top><th>Input Number</th>
        '''    <th>Input rounded to one digit<br> with {@code HALF_EVEN} rounding
        ''' <tr align=right><td>5.5</td>  <td>6</td>
        ''' <tr align=right><td>2.5</td>  <td>2</td>
        ''' <tr align=right><td>1.6</td>  <td>2</td>
        ''' <tr align=right><td>1.1</td>  <td>1</td>
        ''' <tr align=right><td>1.0</td>  <td>1</td>
        ''' <tr align=right><td>-1.0</td> <td>-1</td>
        ''' <tr align=right><td>-1.1</td> <td>-1</td>
        ''' <tr align=right><td>-1.6</td> <td>-2</td>
        ''' <tr align=right><td>-2.5</td> <td>-2</td>
        ''' <tr align=right><td>-5.5</td> <td>-6</td>
        ''' </table>
        ''' </summary>
        Public Shared ReadOnly Property HALF_EVEN As RoundingMode = BigDecimal.ROUND_HALF_EVEN

        ''' <summary>
        ''' Rounding mode to assert that the requested operation has an exact
        ''' result, hence no rounding is necessary.  If this rounding mode is
        ''' specified on an operation that yields an inexact result, an
        ''' {@code ArithmeticException} is thrown.
        ''' <p>Example:
        ''' <table border>
        ''' <caption><b>Rounding mode UNNECESSARY Examples</b></caption>
        ''' <tr valign=top><th>Input Number</th>
        '''    <th>Input rounded to one digit<br> with {@code UNNECESSARY} rounding
        ''' <tr align=right><td>5.5</td>  <td>throw {@code ArithmeticException}</td>
        ''' <tr align=right><td>2.5</td>  <td>throw {@code ArithmeticException}</td>
        ''' <tr align=right><td>1.6</td>  <td>throw {@code ArithmeticException}</td>
        ''' <tr align=right><td>1.1</td>  <td>throw {@code ArithmeticException}</td>
        ''' <tr align=right><td>1.0</td>  <td>1</td>
        ''' <tr align=right><td>-1.0</td> <td>-1</td>
        ''' <tr align=right><td>-1.1</td> <td>throw {@code ArithmeticException}</td>
        ''' <tr align=right><td>-1.6</td> <td>throw {@code ArithmeticException}</td>
        ''' <tr align=right><td>-2.5</td> <td>throw {@code ArithmeticException}</td>
        ''' <tr align=right><td>-5.5</td> <td>throw {@code ArithmeticException}</td>
        ''' </table>
        ''' </summary>
        Public Shared ReadOnly Property UNNECESSARY As RoundingMode = BigDecimal.ROUND_UNNECESSARY

        ' Corresponding BigDecimal rounding constant
        Dim oldMode As Integer

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="oldMode"> The {@code BigDecimal} constant corresponding to
        '''        this mode </param>
        Sub New(oldMode As Integer)
            Me.oldMode = oldMode
        End Sub

        Public Shared Widening Operator CType(n As Integer) As RoundingMode
            Return New RoundingMode(n)
        End Operator

        ''' <summary>
        ''' Returns the {@code RoundingMode} object corresponding to a
        ''' legacy integer rounding mode constant in <seealso cref="BigDecimal"/>.
        ''' </summary>
        ''' <param name="rm"> legacy integer rounding mode to convert </param>
        ''' <returns> {@code RoundingMode} corresponding to the given integer. </returns>
        ''' <exception cref="IllegalArgumentException"> integer is out of range </exception>
        Public Function valueOf(rm As Integer) As RoundingMode

            Select Case rm
                Case BigDecimal.ROUND_UP
                    Return RoundingMode.UP

                Case BigDecimal.ROUND_DOWN
                    Return RoundingMode.DOWN

                Case BigDecimal.ROUND_CEILING
                    Return RoundingMode.CEILING

                Case BigDecimal.ROUND_FLOOR
                    Return RoundingMode.FLOOR

                Case BigDecimal.ROUND_HALF_UP
                    Return RoundingMode.HALF_UP

                Case BigDecimal.ROUND_HALF_DOWN
                    Return RoundingMode.HALF_DOWN

                Case BigDecimal.ROUND_HALF_EVEN
                    Return RoundingMode.HALF_EVEN

                Case BigDecimal.ROUND_UNNECESSARY
                    Return RoundingMode.UNNECESSARY

                Case Else
                    Throw New IllegalArgumentException("argument out of range")
            End Select
        End Function
    End Class
End Namespace
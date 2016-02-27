Imports System
Imports System.Diagnostics
Imports System.Runtime.InteropServices

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The class {@code StrictMath} contains methods for performing basic
	''' numeric operations such as the elementary exponential, logarithm,
	''' square root, and trigonometric functions.
	''' 
	''' <p>To help ensure portability of Java programs, the definitions of
	''' some of the numeric functions in this package require that they
	''' produce the same results as certain published algorithms. These
	''' algorithms are available from the well-known network library
	''' {@code netlib} as the package "Freely Distributable Math
	''' Library," <a
	''' href="ftp://ftp.netlib.org/fdlibm.tar">{@code fdlibm}</a>. These
	''' algorithms, which are written in the C programming language, are
	''' then to be understood as executed with all floating-point
	''' operations following the rules of Java floating-point arithmetic.
	''' 
	''' <p>The Java math library is defined with respect to
	''' {@code fdlibm} version 5.3. Where {@code fdlibm} provides
	''' more than one definition for a function (such as
	''' {@code acos}), use the "IEEE 754 core function" version
	''' (residing in a file whose name begins with the letter
	''' {@code e}).  The methods which require {@code fdlibm}
	''' semantics are {@code sin}, {@code cos}, {@code tan},
	''' {@code asin}, {@code acos}, {@code atan},
	''' {@code exp}, {@code log}, {@code log10},
	''' {@code cbrt}, {@code atan2}, {@code pow},
	''' {@code sinh}, {@code cosh}, {@code tanh},
	''' {@code hypot}, {@code expm1}, and {@code log1p}.
	''' 
	''' <p>
	''' The platform uses signed two's complement integer arithmetic with
	''' int and long primitive types.  The developer should choose
	''' the primitive type to ensure that arithmetic operations consistently
	''' produce correct results, which in some cases means the operations
	''' will not overflow the range of values of the computation.
	''' The best practice is to choose the primitive type and algorithm to avoid
	''' overflow. In cases where the size is {@code int} or {@code long} and
	''' overflow errors need to be detected, the methods {@code addExact},
	''' {@code subtractExact}, {@code multiplyExact}, and {@code toIntExact}
	''' throw an {@code ArithmeticException} when the results overflow.
	''' For other arithmetic operations such as divide, absolute value,
	''' increment, decrement, and negation overflow occurs only with
	''' a specific minimum or maximum value and should be checked against
	''' the minimum or maximum as appropriate.
	''' 
	''' @author  unascribed
	''' @author  Joseph D. Darcy
	''' @since   1.3
	''' </summary>

	Public NotInheritable Class StrictMath

		''' <summary>
		''' Don't let anyone instantiate this class.
		''' </summary>
		Private Sub New()
		End Sub

		''' <summary>
		''' The {@code double} value that is closer than any other to
		''' <i>e</i>, the base of the natural logarithms.
		''' </summary>
		Public Const E As Double = 2.7182818284590452354

		''' <summary>
		''' The {@code double} value that is closer than any other to
		''' <i>pi</i>, the ratio of the circumference of a circle to its
		''' diameter.
		''' </summary>
		Public Const PI As Double = 3.14159265358979323846

		''' <summary>
		''' Returns the trigonometric sine of an angle. Special cases:
		''' <ul><li>If the argument is NaN or an infinity, then the
		''' result is NaN.
		''' <li>If the argument is zero, then the result is a zero with the
		''' same sign as the argument.</ul>
		''' </summary>
		''' <param name="a">   an angle, in radians. </param>
		''' <returns>  the sine of the argument. </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function sin(ByVal a As Double) As Double
		End Function

		''' <summary>
		''' Returns the trigonometric cosine of an angle. Special cases:
		''' <ul><li>If the argument is NaN or an infinity, then the
		''' result is NaN.</ul>
		''' </summary>
		''' <param name="a">   an angle, in radians. </param>
		''' <returns>  the cosine of the argument. </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function cos(ByVal a As Double) As Double
		End Function

		''' <summary>
		''' Returns the trigonometric tangent of an angle. Special cases:
		''' <ul><li>If the argument is NaN or an infinity, then the result
		''' is NaN.
		''' <li>If the argument is zero, then the result is a zero with the
		''' same sign as the argument.</ul>
		''' </summary>
		''' <param name="a">   an angle, in radians. </param>
		''' <returns>  the tangent of the argument. </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function tan(ByVal a As Double) As Double
		End Function

		''' <summary>
		''' Returns the arc sine of a value; the returned angle is in the
		''' range -<i>pi</i>/2 through <i>pi</i>/2.  Special cases:
		''' <ul><li>If the argument is NaN or its absolute value is greater
		''' than 1, then the result is NaN.
		''' <li>If the argument is zero, then the result is a zero with the
		''' same sign as the argument.</ul>
		''' </summary>
		''' <param name="a">   the value whose arc sine is to be returned. </param>
		''' <returns>  the arc sine of the argument. </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function asin(ByVal a As Double) As Double
		End Function

		''' <summary>
		''' Returns the arc cosine of a value; the returned angle is in the
		''' range 0.0 through <i>pi</i>.  Special case:
		''' <ul><li>If the argument is NaN or its absolute value is greater
		''' than 1, then the result is NaN.</ul>
		''' </summary>
		''' <param name="a">   the value whose arc cosine is to be returned. </param>
		''' <returns>  the arc cosine of the argument. </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function acos(ByVal a As Double) As Double
		End Function

		''' <summary>
		''' Returns the arc tangent of a value; the returned angle is in the
		''' range -<i>pi</i>/2 through <i>pi</i>/2.  Special cases:
		''' <ul><li>If the argument is NaN, then the result is NaN.
		''' <li>If the argument is zero, then the result is a zero with the
		''' same sign as the argument.</ul>
		''' </summary>
		''' <param name="a">   the value whose arc tangent is to be returned. </param>
		''' <returns>  the arc tangent of the argument. </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function atan(ByVal a As Double) As Double
		End Function

		''' <summary>
		''' Converts an angle measured in degrees to an approximately
		''' equivalent angle measured in radians.  The conversion from
		''' degrees to radians is generally inexact.
		''' </summary>
		''' <param name="angdeg">   an angle, in degrees </param>
		''' <returns>  the measurement of the angle {@code angdeg}
		'''          in radians. </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent to 'strictfp' in .NET:
		Public Shared Function toRadians(ByVal angdeg As Double) As Double
			' Do not delegate to System.Math.toRadians(angdeg) because
			' this method has the strictfp modifier.
			Return angdeg / 180.0 * PI
		End Function

		''' <summary>
		''' Converts an angle measured in radians to an approximately
		''' equivalent angle measured in degrees.  The conversion from
		''' radians to degrees is generally inexact; users should
		''' <i>not</i> expect {@code cos(toRadians(90.0))} to exactly
		''' equal {@code 0.0}.
		''' </summary>
		''' <param name="angrad">   an angle, in radians </param>
		''' <returns>  the measurement of the angle {@code angrad}
		'''          in degrees. </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent to 'strictfp' in .NET:
		Public Shared Function toDegrees(ByVal angrad As Double) As Double
			' Do not delegate to System.Math.toDegrees(angrad) because
			' this method has the strictfp modifier.
			Return angrad * 180.0 / PI
		End Function

		''' <summary>
		''' Returns Euler's number <i>e</i> raised to the power of a
		''' {@code double} value. Special cases:
		''' <ul><li>If the argument is NaN, the result is NaN.
		''' <li>If the argument is positive infinity, then the result is
		''' positive infinity.
		''' <li>If the argument is negative infinity, then the result is
		''' positive zero.</ul>
		''' </summary>
		''' <param name="a">   the exponent to raise <i>e</i> to. </param>
		''' <returns>  the value <i>e</i><sup>{@code a}</sup>,
		'''          where <i>e</i> is the base of the natural logarithms. </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function exp(ByVal a As Double) As Double
		End Function

		''' <summary>
		''' Returns the natural logarithm (base <i>e</i>) of a {@code double}
		''' value. Special cases:
		''' <ul><li>If the argument is NaN or less than zero, then the result
		''' is NaN.
		''' <li>If the argument is positive infinity, then the result is
		''' positive infinity.
		''' <li>If the argument is positive zero or negative zero, then the
		''' result is negative infinity.</ul>
		''' </summary>
		''' <param name="a">   a value </param>
		''' <returns>  the value ln&nbsp;{@code a}, the natural logarithm of
		'''          {@code a}. </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function log(ByVal a As Double) As Double
		End Function


		''' <summary>
		''' Returns the base 10 logarithm of a {@code double} value.
		''' Special cases:
		''' 
		''' <ul><li>If the argument is NaN or less than zero, then the result
		''' is NaN.
		''' <li>If the argument is positive infinity, then the result is
		''' positive infinity.
		''' <li>If the argument is positive zero or negative zero, then the
		''' result is negative infinity.
		''' <li> If the argument is equal to 10<sup><i>n</i></sup> for
		''' integer <i>n</i>, then the result is <i>n</i>.
		''' </ul>
		''' </summary>
		''' <param name="a">   a value </param>
		''' <returns>  the base 10 logarithm of  {@code a}.
		''' @since 1.5 </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function log10(ByVal a As Double) As Double
		End Function

		''' <summary>
		''' Returns the correctly rounded positive square root of a
		''' {@code double} value.
		''' Special cases:
		''' <ul><li>If the argument is NaN or less than zero, then the result
		''' is NaN.
		''' <li>If the argument is positive infinity, then the result is positive
		''' infinity.
		''' <li>If the argument is positive zero or negative zero, then the
		''' result is the same as the argument.</ul>
		''' Otherwise, the result is the {@code double} value closest to
		''' the true mathematical square root of the argument value.
		''' </summary>
		''' <param name="a">   a value. </param>
		''' <returns>  the positive square root of {@code a}. </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function sqrt(ByVal a As Double) As Double
		End Function

		''' <summary>
		''' Returns the cube root of a {@code double} value.  For
		''' positive finite {@code x}, {@code cbrt(-x) ==
		''' -cbrt(x)}; that is, the cube root of a negative value is
		''' the negative of the cube root of that value's magnitude.
		''' Special cases:
		''' 
		''' <ul>
		''' 
		''' <li>If the argument is NaN, then the result is NaN.
		''' 
		''' <li>If the argument is infinite, then the result is an infinity
		''' with the same sign as the argument.
		''' 
		''' <li>If the argument is zero, then the result is a zero with the
		''' same sign as the argument.
		''' 
		''' </ul>
		''' </summary>
		''' <param name="a">   a value. </param>
		''' <returns>  the cube root of {@code a}.
		''' @since 1.5 </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function cbrt(ByVal a As Double) As Double
		End Function

		''' <summary>
		''' Computes the remainder operation on two arguments as prescribed
		''' by the IEEE 754 standard.
		''' The remainder value is mathematically equal to
		''' <code>f1&nbsp;-&nbsp;f2</code>&nbsp;&times;&nbsp;<i>n</i>,
		''' where <i>n</i> is the mathematical integer closest to the exact
		''' mathematical value of the quotient {@code f1/f2}, and if two
		''' mathematical integers are equally close to {@code f1/f2},
		''' then <i>n</i> is the integer that is even. If the remainder is
		''' zero, its sign is the same as the sign of the first argument.
		''' Special cases:
		''' <ul><li>If either argument is NaN, or the first argument is infinite,
		''' or the second argument is positive zero or negative zero, then the
		''' result is NaN.
		''' <li>If the first argument is finite and the second argument is
		''' infinite, then the result is the same as the first argument.</ul>
		''' </summary>
		''' <param name="f1">   the dividend. </param>
		''' <param name="f2">   the divisor. </param>
		''' <returns>  the remainder when {@code f1} is divided by
		'''          {@code f2}. </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function IEEEremainder(ByVal f1 As Double, ByVal f2 As Double) As Double
		End Function

		''' <summary>
		''' Returns the smallest (closest to negative infinity)
		''' {@code double} value that is greater than or equal to the
		''' argument and is equal to a mathematical  java.lang.[Integer]. Special cases:
		''' <ul><li>If the argument value is already equal to a
		''' mathematical integer, then the result is the same as the
		''' argument.  <li>If the argument is NaN or an infinity or
		''' positive zero or negative zero, then the result is the same as
		''' the argument.  <li>If the argument value is less than zero but
		''' greater than -1.0, then the result is negative zero.</ul> Note
		''' that the value of {@code StrictMath.ceil(x)} is exactly the
		''' value of {@code -StrictMath.floor(-x)}.
		''' </summary>
		''' <param name="a">   a value. </param>
		''' <returns>  the smallest (closest to negative infinity)
		'''          floating-point value that is greater than or equal to
		'''          the argument and is equal to a mathematical  java.lang.[Integer]. </returns>
		Public Shared Function ceil(ByVal a As Double) As Double
			Return floorOrCeil(a, -0.0, 1.0, 1.0)
		End Function

		''' <summary>
		''' Returns the largest (closest to positive infinity)
		''' {@code double} value that is less than or equal to the
		''' argument and is equal to a mathematical  java.lang.[Integer]. Special cases:
		''' <ul><li>If the argument value is already equal to a
		''' mathematical integer, then the result is the same as the
		''' argument.  <li>If the argument is NaN or an infinity or
		''' positive zero or negative zero, then the result is the same as
		''' the argument.</ul>
		''' </summary>
		''' <param name="a">   a value. </param>
		''' <returns>  the largest (closest to positive infinity)
		'''          floating-point value that less than or equal to the argument
		'''          and is equal to a mathematical  java.lang.[Integer]. </returns>
		Public Shared Function floor(ByVal a As Double) As Double
			Return floorOrCeil(a, -1.0, 0.0, -1.0)
		End Function

		''' <summary>
		''' Internal method to share logic between floor and ceil.
		''' </summary>
		''' <param name="a"> the value to be floored or ceiled </param>
		''' <param name="negativeBoundary"> result for values in (-1, 0) </param>
		''' <param name="positiveBoundary"> result for values in (0, 1) </param>
		''' <param name="increment"> value to add when the argument is non-integral </param>
		Private Shared Function floorOrCeil(ByVal a As Double, ByVal negativeBoundary As Double, ByVal positiveBoundary As Double, ByVal sign As Double) As Double
			Dim exponent_Renamed As Integer = System.Math.getExponent(a)

			If exponent_Renamed < 0 Then
	'            
	'             * Absolute value of argument is less than 1.
	'             * floorOrceil(-0.0) => -0.0
	'             * floorOrceil(+0.0) => +0.0
	'             
				Return (If(a = 0.0, a, (If(a < 0.0, negativeBoundary, positiveBoundary))))
			ElseIf exponent_Renamed >= 52 Then
	'            
	'             * Infinity, NaN, or a value so large it must be integral.
	'             
				Return a
			End If
			' Else the argument is either an integral value already XOR it
			' has to be rounded to one.
			Debug.Assert(exponent_Renamed >= 0 AndAlso exponent_Renamed <= 51)

			Dim doppel As Long = java.lang.[Double].doubleToRawLongBits(a)
			Dim mask As Long = sun.misc.DoubleConsts.SIGNIF_BIT_MASK >> exponent_Renamed

			If (mask And doppel) = 0L Then
				Return a ' integral value
			Else
				Dim result As Double = java.lang.[Double].longBitsToDouble(doppel And ((Not mask)))
				If sign*a > 0.0 Then result = result + sign
				Return result
			End If
		End Function

		''' <summary>
		''' Returns the {@code double} value that is closest in value
		''' to the argument and is equal to a mathematical  java.lang.[Integer]. If two
		''' {@code double} values that are mathematical integers are
		''' equally close to the value of the argument, the result is the
		''' integer value that is even. Special cases:
		''' <ul><li>If the argument value is already equal to a mathematical
		''' integer, then the result is the same as the argument.
		''' <li>If the argument is NaN or an infinity or positive zero or negative
		''' zero, then the result is the same as the argument.</ul>
		''' </summary>
		''' <param name="a">   a value. </param>
		''' <returns>  the closest floating-point value to {@code a} that is
		'''          equal to a mathematical  java.lang.[Integer].
		''' @author Joseph D. Darcy </returns>
		Public Shared Function rint(ByVal a As Double) As Double
	'        
	'         * If the absolute value of a is not less than 2^52, it
	'         * is either a finite integer (the double format does not have
	'         * enough significand bits for a number that large to have any
	'         * fractional portion), an infinity, or a NaN.  In any of
	'         * these cases, rint of the argument is the argument.
	'         *
	'         * Otherwise, the sum (twoToThe52 + a ) will properly round
	'         * away any fractional portion of a since ulp(twoToThe52) ==
	'         * 1.0; subtracting out twoToThe52 from this sum will then be
	'         * exact and leave the rounded integer portion of a.
	'         *
	'         * This method does *not* need to be declared strictfp to get
	'         * fully reproducible results.  Whether or not a method is
	'         * declared strictfp can only make a difference in the
	'         * returned result if some operation would overflow or
	'         * underflow with strictfp semantics.  The operation
	'         * (twoToThe52 + a ) cannot overflow since large values of a
	'         * are screened out; the add cannot underflow since twoToThe52
	'         * is too large.  The subtraction ((twoToThe52 + a ) -
	'         * twoToThe52) will be exact as discussed above and thus
	'         * cannot overflow or meaningfully underflow.  Finally, the
	'         * last multiply in the return statement is by plus or minus
	'         * 1.0, which is exact too.
	'         
			Dim twoToThe52 As Double = CDbl(1L << 52) ' 2^52
			Dim sign As Double = System.Math.copySign(1.0, a) ' preserve sign info
			a = System.Math.Abs(a)

			If a < twoToThe52 Then ' E_min <= ilogb(a) <= 51 a = ((twoToThe52 + a) - twoToThe52)

			Return sign * a ' restore original sign
		End Function

		''' <summary>
		''' Returns the angle <i>theta</i> from the conversion of rectangular
		''' coordinates ({@code x},&nbsp;{@code y}) to polar
		''' coordinates (r,&nbsp;<i>theta</i>).
		''' This method computes the phase <i>theta</i> by computing an arc tangent
		''' of {@code y/x} in the range of -<i>pi</i> to <i>pi</i>. Special
		''' cases:
		''' <ul><li>If either argument is NaN, then the result is NaN.
		''' <li>If the first argument is positive zero and the second argument
		''' is positive, or the first argument is positive and finite and the
		''' second argument is positive infinity, then the result is positive
		''' zero.
		''' <li>If the first argument is negative zero and the second argument
		''' is positive, or the first argument is negative and finite and the
		''' second argument is positive infinity, then the result is negative zero.
		''' <li>If the first argument is positive zero and the second argument
		''' is negative, or the first argument is positive and finite and the
		''' second argument is negative infinity, then the result is the
		''' {@code double} value closest to <i>pi</i>.
		''' <li>If the first argument is negative zero and the second argument
		''' is negative, or the first argument is negative and finite and the
		''' second argument is negative infinity, then the result is the
		''' {@code double} value closest to -<i>pi</i>.
		''' <li>If the first argument is positive and the second argument is
		''' positive zero or negative zero, or the first argument is positive
		''' infinity and the second argument is finite, then the result is the
		''' {@code double} value closest to <i>pi</i>/2.
		''' <li>If the first argument is negative and the second argument is
		''' positive zero or negative zero, or the first argument is negative
		''' infinity and the second argument is finite, then the result is the
		''' {@code double} value closest to -<i>pi</i>/2.
		''' <li>If both arguments are positive infinity, then the result is the
		''' {@code double} value closest to <i>pi</i>/4.
		''' <li>If the first argument is positive infinity and the second argument
		''' is negative infinity, then the result is the {@code double}
		''' value closest to 3*<i>pi</i>/4.
		''' <li>If the first argument is negative infinity and the second argument
		''' is positive infinity, then the result is the {@code double} value
		''' closest to -<i>pi</i>/4.
		''' <li>If both arguments are negative infinity, then the result is the
		''' {@code double} value closest to -3*<i>pi</i>/4.</ul>
		''' </summary>
		''' <param name="y">   the ordinate coordinate </param>
		''' <param name="x">   the abscissa coordinate </param>
		''' <returns>  the <i>theta</i> component of the point
		'''          (<i>r</i>,&nbsp;<i>theta</i>)
		'''          in polar coordinates that corresponds to the point
		'''          (<i>x</i>,&nbsp;<i>y</i>) in Cartesian coordinates. </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function atan2(ByVal y As Double, ByVal x As Double) As Double
		End Function


		''' <summary>
		''' Returns the value of the first argument raised to the power of the
		''' second argument. Special cases:
		''' 
		''' <ul><li>If the second argument is positive or negative zero, then the
		''' result is 1.0.
		''' <li>If the second argument is 1.0, then the result is the same as the
		''' first argument.
		''' <li>If the second argument is NaN, then the result is NaN.
		''' <li>If the first argument is NaN and the second argument is nonzero,
		''' then the result is NaN.
		''' 
		''' <li>If
		''' <ul>
		''' <li>the absolute value of the first argument is greater than 1
		''' and the second argument is positive infinity, or
		''' <li>the absolute value of the first argument is less than 1 and
		''' the second argument is negative infinity,
		''' </ul>
		''' then the result is positive infinity.
		''' 
		''' <li>If
		''' <ul>
		''' <li>the absolute value of the first argument is greater than 1 and
		''' the second argument is negative infinity, or
		''' <li>the absolute value of the
		''' first argument is less than 1 and the second argument is positive
		''' infinity,
		''' </ul>
		''' then the result is positive zero.
		''' 
		''' <li>If the absolute value of the first argument equals 1 and the
		''' second argument is infinite, then the result is NaN.
		''' 
		''' <li>If
		''' <ul>
		''' <li>the first argument is positive zero and the second argument
		''' is greater than zero, or
		''' <li>the first argument is positive infinity and the second
		''' argument is less than zero,
		''' </ul>
		''' then the result is positive zero.
		''' 
		''' <li>If
		''' <ul>
		''' <li>the first argument is positive zero and the second argument
		''' is less than zero, or
		''' <li>the first argument is positive infinity and the second
		''' argument is greater than zero,
		''' </ul>
		''' then the result is positive infinity.
		''' 
		''' <li>If
		''' <ul>
		''' <li>the first argument is negative zero and the second argument
		''' is greater than zero but not a finite odd integer, or
		''' <li>the first argument is negative infinity and the second
		''' argument is less than zero but not a finite odd integer,
		''' </ul>
		''' then the result is positive zero.
		''' 
		''' <li>If
		''' <ul>
		''' <li>the first argument is negative zero and the second argument
		''' is a positive finite odd integer, or
		''' <li>the first argument is negative infinity and the second
		''' argument is a negative finite odd integer,
		''' </ul>
		''' then the result is negative zero.
		''' 
		''' <li>If
		''' <ul>
		''' <li>the first argument is negative zero and the second argument
		''' is less than zero but not a finite odd integer, or
		''' <li>the first argument is negative infinity and the second
		''' argument is greater than zero but not a finite odd integer,
		''' </ul>
		''' then the result is positive infinity.
		''' 
		''' <li>If
		''' <ul>
		''' <li>the first argument is negative zero and the second argument
		''' is a negative finite odd integer, or
		''' <li>the first argument is negative infinity and the second
		''' argument is a positive finite odd integer,
		''' </ul>
		''' then the result is negative infinity.
		''' 
		''' <li>If the first argument is finite and less than zero
		''' <ul>
		''' <li> if the second argument is a finite even integer, the
		''' result is equal to the result of raising the absolute value of
		''' the first argument to the power of the second argument
		''' 
		''' <li>if the second argument is a finite odd integer, the result
		''' is equal to the negative of the result of raising the absolute
		''' value of the first argument to the power of the second
		''' argument
		''' 
		''' <li>if the second argument is finite and not an integer, then
		''' the result is NaN.
		''' </ul>
		''' 
		''' <li>If both arguments are integers, then the result is exactly equal
		''' to the mathematical result of raising the first argument to the power
		''' of the second argument if that result can in fact be represented
		''' exactly as a {@code double} value.</ul>
		''' 
		''' <p>(In the foregoing descriptions, a floating-point value is
		''' considered to be an integer if and only if it is finite and a
		''' fixed point of the method <seealso cref="#ceil ceil"/> or,
		''' equivalently, a fixed point of the method {@link #floor
		''' floor}. A value is a fixed point of a one-argument
		''' method if and only if the result of applying the method to the
		''' value is equal to the value.)
		''' </summary>
		''' <param name="a">   base. </param>
		''' <param name="b">   the exponent. </param>
		''' <returns>  the value {@code a}<sup>{@code b}</sup>. </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function pow(ByVal a As Double, ByVal b As Double) As Double
		End Function

		''' <summary>
		''' Returns the closest {@code int} to the argument, with ties
		''' rounding to positive infinity.
		''' 
		''' <p>Special cases:
		''' <ul><li>If the argument is NaN, the result is 0.
		''' <li>If the argument is negative infinity or any value less than or
		''' equal to the value of {@code  java.lang.[Integer].MIN_VALUE}, the result is
		''' equal to the value of {@code  java.lang.[Integer].MIN_VALUE}.
		''' <li>If the argument is positive infinity or any value greater than or
		''' equal to the value of {@code  java.lang.[Integer].MAX_VALUE}, the result is
		''' equal to the value of {@code  java.lang.[Integer].MAX_VALUE}.</ul>
		''' </summary>
		''' <param name="a">   a floating-point value to be rounded to an  java.lang.[Integer]. </param>
		''' <returns>  the value of the argument rounded to the nearest
		'''          {@code int} value. </returns>
		''' <seealso cref=     java.lang.Integer#MAX_VALUE </seealso>
		''' <seealso cref=     java.lang.Integer#MIN_VALUE </seealso>
		Public Shared Function round(ByVal a As Single) As Integer
			Return System.Math.Round(a)
		End Function

		''' <summary>
		''' Returns the closest {@code long} to the argument, with ties
		''' rounding to positive infinity.
		''' 
		''' <p>Special cases:
		''' <ul><li>If the argument is NaN, the result is 0.
		''' <li>If the argument is negative infinity or any value less than or
		''' equal to the value of {@code java.lang.[Long].MIN_VALUE}, the result is
		''' equal to the value of {@code java.lang.[Long].MIN_VALUE}.
		''' <li>If the argument is positive infinity or any value greater than or
		''' equal to the value of {@code java.lang.[Long].MAX_VALUE}, the result is
		''' equal to the value of {@code java.lang.[Long].MAX_VALUE}.</ul>
		''' </summary>
		''' <param name="a">  a floating-point value to be rounded to a
		'''          {@code long}. </param>
		''' <returns>  the value of the argument rounded to the nearest
		'''          {@code long} value. </returns>
		''' <seealso cref=     java.lang.Long#MAX_VALUE </seealso>
		''' <seealso cref=     java.lang.Long#MIN_VALUE </seealso>
		Public Shared Function round(ByVal a As Double) As Long
			Return System.Math.Round(a)
		End Function

		Private NotInheritable Class RandomNumberGeneratorHolder
			Friend Shared ReadOnly randomNumberGenerator As New Random
		End Class

		''' <summary>
		''' Returns a {@code double} value with a positive sign, greater
		''' than or equal to {@code 0.0} and less than {@code 1.0}.
		''' Returned values are chosen pseudorandomly with (approximately)
		''' uniform distribution from that range.
		''' 
		''' <p>When this method is first called, it creates a single new
		''' pseudorandom-number generator, exactly as if by the expression
		''' 
		''' <blockquote>{@code new java.util.Random()}</blockquote>
		''' 
		''' This new pseudorandom-number generator is used thereafter for
		''' all calls to this method and is used nowhere else.
		''' 
		''' <p>This method is properly synchronized to allow correct use by
		''' more than one thread. However, if many threads need to generate
		''' pseudorandom numbers at a great rate, it may reduce contention
		''' for each thread to have its own pseudorandom-number generator.
		''' </summary>
		''' <returns>  a pseudorandom {@code double} greater than or equal
		''' to {@code 0.0} and less than {@code 1.0}. </returns>
		''' <seealso cref= Random#nextDouble() </seealso>
		Public Shared Function random() As Double
			Return RandomNumberGeneratorHolder.randomNumberGenerator.NextDouble()
		End Function

		''' <summary>
		''' Returns the sum of its arguments,
		''' throwing an exception if the result overflows an {@code int}.
		''' </summary>
		''' <param name="x"> the first value </param>
		''' <param name="y"> the second value </param>
		''' <returns> the result </returns>
		''' <exception cref="ArithmeticException"> if the result overflows an int </exception>
		''' <seealso cref= Math#addExact(int,int)
		''' @since 1.8 </seealso>
		Public Shared Function addExact(ByVal x As Integer, ByVal y As Integer) As Integer
			Return System.Math.addExact(x, y)
		End Function

		''' <summary>
		''' Returns the sum of its arguments,
		''' throwing an exception if the result overflows a {@code long}.
		''' </summary>
		''' <param name="x"> the first value </param>
		''' <param name="y"> the second value </param>
		''' <returns> the result </returns>
		''' <exception cref="ArithmeticException"> if the result overflows a long </exception>
		''' <seealso cref= Math#addExact(long,long)
		''' @since 1.8 </seealso>
		Public Shared Function addExact(ByVal x As Long, ByVal y As Long) As Long
			Return System.Math.addExact(x, y)
		End Function

		''' <summary>
		''' Returns the difference of the arguments,
		''' throwing an exception if the result overflows an {@code int}.
		''' </summary>
		''' <param name="x"> the first value </param>
		''' <param name="y"> the second value to subtract from the first </param>
		''' <returns> the result </returns>
		''' <exception cref="ArithmeticException"> if the result overflows an int </exception>
		''' <seealso cref= Math#subtractExact(int,int)
		''' @since 1.8 </seealso>
		Public Shared Function subtractExact(ByVal x As Integer, ByVal y As Integer) As Integer
			Return System.Math.subtractExact(x, y)
		End Function

		''' <summary>
		''' Returns the difference of the arguments,
		''' throwing an exception if the result overflows a {@code long}.
		''' </summary>
		''' <param name="x"> the first value </param>
		''' <param name="y"> the second value to subtract from the first </param>
		''' <returns> the result </returns>
		''' <exception cref="ArithmeticException"> if the result overflows a long </exception>
		''' <seealso cref= Math#subtractExact(long,long)
		''' @since 1.8 </seealso>
		Public Shared Function subtractExact(ByVal x As Long, ByVal y As Long) As Long
			Return System.Math.subtractExact(x, y)
		End Function

		''' <summary>
		''' Returns the product of the arguments,
		''' throwing an exception if the result overflows an {@code int}.
		''' </summary>
		''' <param name="x"> the first value </param>
		''' <param name="y"> the second value </param>
		''' <returns> the result </returns>
		''' <exception cref="ArithmeticException"> if the result overflows an int </exception>
		''' <seealso cref= Math#multiplyExact(int,int)
		''' @since 1.8 </seealso>
		Public Shared Function multiplyExact(ByVal x As Integer, ByVal y As Integer) As Integer
			Return System.Math.multiplyExact(x, y)
		End Function

		''' <summary>
		''' Returns the product of the arguments,
		''' throwing an exception if the result overflows a {@code long}.
		''' </summary>
		''' <param name="x"> the first value </param>
		''' <param name="y"> the second value </param>
		''' <returns> the result </returns>
		''' <exception cref="ArithmeticException"> if the result overflows a long </exception>
		''' <seealso cref= Math#multiplyExact(long,long)
		''' @since 1.8 </seealso>
		Public Shared Function multiplyExact(ByVal x As Long, ByVal y As Long) As Long
			Return System.Math.multiplyExact(x, y)
		End Function

		''' <summary>
		''' Returns the value of the {@code long} argument;
		''' throwing an exception if the value overflows an {@code int}.
		''' </summary>
		''' <param name="value"> the long value </param>
		''' <returns> the argument as an int </returns>
		''' <exception cref="ArithmeticException"> if the {@code argument} overflows an int </exception>
		''' <seealso cref= Math#toIntExact(long)
		''' @since 1.8 </seealso>
		Public Shared Function toIntExact(ByVal value As Long) As Integer
			Return System.Math.toIntExact(value)
		End Function

		''' <summary>
		''' Returns the largest (closest to positive infinity)
		''' {@code int} value that is less than or equal to the algebraic quotient.
		''' There is one special case, if the dividend is the
		''' <seealso cref="Integer#MIN_VALUE  java.lang.[Integer].MIN_VALUE"/> and the divisor is {@code -1},
		''' then integer overflow occurs and
		''' the result is equal to the {@code  java.lang.[Integer].MIN_VALUE}.
		''' <p>
		''' See <seealso cref="Math#floorDiv(int, int) System.Math.floorDiv"/> for examples and
		''' a comparison to the integer division {@code /} operator.
		''' </summary>
		''' <param name="x"> the dividend </param>
		''' <param name="y"> the divisor </param>
		''' <returns> the largest (closest to positive infinity)
		''' {@code int} value that is less than or equal to the algebraic quotient. </returns>
		''' <exception cref="ArithmeticException"> if the divisor {@code y} is zero </exception>
		''' <seealso cref= Math#floorDiv(int, int) </seealso>
		''' <seealso cref= Math#floor(double)
		''' @since 1.8 </seealso>
		Public Shared Function floorDiv(ByVal x As Integer, ByVal y As Integer) As Integer
			Return System.Math.floorDiv(x, y)
		End Function

		''' <summary>
		''' Returns the largest (closest to positive infinity)
		''' {@code long} value that is less than or equal to the algebraic quotient.
		''' There is one special case, if the dividend is the
		''' <seealso cref="Long#MIN_VALUE java.lang.[Long].MIN_VALUE"/> and the divisor is {@code -1},
		''' then integer overflow occurs and
		''' the result is equal to the {@code java.lang.[Long].MIN_VALUE}.
		''' <p>
		''' See <seealso cref="Math#floorDiv(int, int) System.Math.floorDiv"/> for examples and
		''' a comparison to the integer division {@code /} operator.
		''' </summary>
		''' <param name="x"> the dividend </param>
		''' <param name="y"> the divisor </param>
		''' <returns> the largest (closest to positive infinity)
		''' {@code long} value that is less than or equal to the algebraic quotient. </returns>
		''' <exception cref="ArithmeticException"> if the divisor {@code y} is zero </exception>
		''' <seealso cref= Math#floorDiv(long, long) </seealso>
		''' <seealso cref= Math#floor(double)
		''' @since 1.8 </seealso>
		Public Shared Function floorDiv(ByVal x As Long, ByVal y As Long) As Long
			Return System.Math.floorDiv(x, y)
		End Function

		''' <summary>
		''' Returns the floor modulus of the {@code int} arguments.
		''' <p>
		''' The floor modulus is {@code x - (floorDiv(x, y) * y)},
		''' has the same sign as the divisor {@code y}, and
		''' is in the range of {@code -abs(y) < r < +abs(y)}.
		''' <p>
		''' The relationship between {@code floorDiv} and {@code floorMod} is such that:
		''' <ul>
		'''   <li>{@code floorDiv(x, y) * y + floorMod(x, y) == x}
		''' </ul>
		''' <p>
		''' See <seealso cref="Math#floorMod(int, int) System.Math.floorMod"/> for examples and
		''' a comparison to the {@code %} operator.
		''' </summary>
		''' <param name="x"> the dividend </param>
		''' <param name="y"> the divisor </param>
		''' <returns> the floor modulus {@code x - (floorDiv(x, y) * y)} </returns>
		''' <exception cref="ArithmeticException"> if the divisor {@code y} is zero </exception>
		''' <seealso cref= Math#floorMod(int, int) </seealso>
		''' <seealso cref= StrictMath#floorDiv(int, int)
		''' @since 1.8 </seealso>
		Public Shared Function floorMod(ByVal x As Integer, ByVal y As Integer) As Integer
			Return System.Math.floorMod(x, y)
		End Function
		''' <summary>
		''' Returns the floor modulus of the {@code long} arguments.
		''' <p>
		''' The floor modulus is {@code x - (floorDiv(x, y) * y)},
		''' has the same sign as the divisor {@code y}, and
		''' is in the range of {@code -abs(y) < r < +abs(y)}.
		''' <p>
		''' The relationship between {@code floorDiv} and {@code floorMod} is such that:
		''' <ul>
		'''   <li>{@code floorDiv(x, y) * y + floorMod(x, y) == x}
		''' </ul>
		''' <p>
		''' See <seealso cref="Math#floorMod(int, int) System.Math.floorMod"/> for examples and
		''' a comparison to the {@code %} operator.
		''' </summary>
		''' <param name="x"> the dividend </param>
		''' <param name="y"> the divisor </param>
		''' <returns> the floor modulus {@code x - (floorDiv(x, y) * y)} </returns>
		''' <exception cref="ArithmeticException"> if the divisor {@code y} is zero </exception>
		''' <seealso cref= Math#floorMod(long, long) </seealso>
		''' <seealso cref= StrictMath#floorDiv(long, long)
		''' @since 1.8 </seealso>
		Public Shared Function floorMod(ByVal x As Long, ByVal y As Long) As Long
			Return System.Math.floorMod(x, y)
		End Function

		''' <summary>
		''' Returns the absolute value of an {@code int} value.
		''' If the argument is not negative, the argument is returned.
		''' If the argument is negative, the negation of the argument is returned.
		''' 
		''' <p>Note that if the argument is equal to the value of
		''' <seealso cref="Integer#MIN_VALUE"/>, the most negative representable
		''' {@code int} value, the result is that same value, which is
		''' negative.
		''' </summary>
		''' <param name="a">   the  argument whose absolute value is to be determined. </param>
		''' <returns>  the absolute value of the argument. </returns>
		Public Shared Function abs(ByVal a As Integer) As Integer
			Return System.Math.Abs(a)
		End Function

		''' <summary>
		''' Returns the absolute value of a {@code long} value.
		''' If the argument is not negative, the argument is returned.
		''' If the argument is negative, the negation of the argument is returned.
		''' 
		''' <p>Note that if the argument is equal to the value of
		''' <seealso cref="Long#MIN_VALUE"/>, the most negative representable
		''' {@code long} value, the result is that same value, which
		''' is negative.
		''' </summary>
		''' <param name="a">   the  argument whose absolute value is to be determined. </param>
		''' <returns>  the absolute value of the argument. </returns>
		Public Shared Function abs(ByVal a As Long) As Long
			Return System.Math.Abs(a)
		End Function

		''' <summary>
		''' Returns the absolute value of a {@code float} value.
		''' If the argument is not negative, the argument is returned.
		''' If the argument is negative, the negation of the argument is returned.
		''' Special cases:
		''' <ul><li>If the argument is positive zero or negative zero, the
		''' result is positive zero.
		''' <li>If the argument is infinite, the result is positive infinity.
		''' <li>If the argument is NaN, the result is NaN.</ul>
		''' In other words, the result is the same as the value of the expression:
		''' <p>{@code Float.intBitsToFloat(0x7fffffff & Float.floatToIntBits(a))}
		''' </summary>
		''' <param name="a">   the argument whose absolute value is to be determined </param>
		''' <returns>  the absolute value of the argument. </returns>
		Public Shared Function abs(ByVal a As Single) As Single
			Return System.Math.Abs(a)
		End Function

		''' <summary>
		''' Returns the absolute value of a {@code double} value.
		''' If the argument is not negative, the argument is returned.
		''' If the argument is negative, the negation of the argument is returned.
		''' Special cases:
		''' <ul><li>If the argument is positive zero or negative zero, the result
		''' is positive zero.
		''' <li>If the argument is infinite, the result is positive infinity.
		''' <li>If the argument is NaN, the result is NaN.</ul>
		''' In other words, the result is the same as the value of the expression:
		''' <p>{@code java.lang.[Double].longBitsToDouble((Double.doubleToLongBits(a)<<1)>>>1)}
		''' </summary>
		''' <param name="a">   the argument whose absolute value is to be determined </param>
		''' <returns>  the absolute value of the argument. </returns>
		Public Shared Function abs(ByVal a As Double) As Double
			Return System.Math.Abs(a)
		End Function

		''' <summary>
		''' Returns the greater of two {@code int} values. That is, the
		''' result is the argument closer to the value of
		''' <seealso cref="Integer#MAX_VALUE"/>. If the arguments have the same value,
		''' the result is that same value.
		''' </summary>
		''' <param name="a">   an argument. </param>
		''' <param name="b">   another argument. </param>
		''' <returns>  the larger of {@code a} and {@code b}. </returns>
		Public Shared Function max(ByVal a As Integer, ByVal b As Integer) As Integer
			Return System.Math.Max(a, b)
		End Function

		''' <summary>
		''' Returns the greater of two {@code long} values. That is, the
		''' result is the argument closer to the value of
		''' <seealso cref="Long#MAX_VALUE"/>. If the arguments have the same value,
		''' the result is that same value.
		''' </summary>
		''' <param name="a">   an argument. </param>
		''' <param name="b">   another argument. </param>
		''' <returns>  the larger of {@code a} and {@code b}. </returns>
		Public Shared Function max(ByVal a As Long, ByVal b As Long) As Long
			Return System.Math.Max(a, b)
		End Function

		''' <summary>
		''' Returns the greater of two {@code float} values.  That is,
		''' the result is the argument closer to positive infinity. If the
		''' arguments have the same value, the result is that same
		''' value. If either value is NaN, then the result is NaN.  Unlike
		''' the numerical comparison operators, this method considers
		''' negative zero to be strictly smaller than positive zero. If one
		''' argument is positive zero and the other negative zero, the
		''' result is positive zero.
		''' </summary>
		''' <param name="a">   an argument. </param>
		''' <param name="b">   another argument. </param>
		''' <returns>  the larger of {@code a} and {@code b}. </returns>
		Public Shared Function max(ByVal a As Single, ByVal b As Single) As Single
			Return System.Math.Max(a, b)
		End Function

		''' <summary>
		''' Returns the greater of two {@code double} values.  That
		''' is, the result is the argument closer to positive infinity. If
		''' the arguments have the same value, the result is that same
		''' value. If either value is NaN, then the result is NaN.  Unlike
		''' the numerical comparison operators, this method considers
		''' negative zero to be strictly smaller than positive zero. If one
		''' argument is positive zero and the other negative zero, the
		''' result is positive zero.
		''' </summary>
		''' <param name="a">   an argument. </param>
		''' <param name="b">   another argument. </param>
		''' <returns>  the larger of {@code a} and {@code b}. </returns>
		Public Shared Function max(ByVal a As Double, ByVal b As Double) As Double
			Return System.Math.Max(a, b)
		End Function

		''' <summary>
		''' Returns the smaller of two {@code int} values. That is,
		''' the result the argument closer to the value of
		''' <seealso cref="Integer#MIN_VALUE"/>.  If the arguments have the same
		''' value, the result is that same value.
		''' </summary>
		''' <param name="a">   an argument. </param>
		''' <param name="b">   another argument. </param>
		''' <returns>  the smaller of {@code a} and {@code b}. </returns>
		Public Shared Function min(ByVal a As Integer, ByVal b As Integer) As Integer
			Return System.Math.Min(a, b)
		End Function

		''' <summary>
		''' Returns the smaller of two {@code long} values. That is,
		''' the result is the argument closer to the value of
		''' <seealso cref="Long#MIN_VALUE"/>. If the arguments have the same
		''' value, the result is that same value.
		''' </summary>
		''' <param name="a">   an argument. </param>
		''' <param name="b">   another argument. </param>
		''' <returns>  the smaller of {@code a} and {@code b}. </returns>
		Public Shared Function min(ByVal a As Long, ByVal b As Long) As Long
			Return System.Math.Min(a, b)
		End Function

		''' <summary>
		''' Returns the smaller of two {@code float} values.  That is,
		''' the result is the value closer to negative infinity. If the
		''' arguments have the same value, the result is that same
		''' value. If either value is NaN, then the result is NaN.  Unlike
		''' the numerical comparison operators, this method considers
		''' negative zero to be strictly smaller than positive zero.  If
		''' one argument is positive zero and the other is negative zero,
		''' the result is negative zero.
		''' </summary>
		''' <param name="a">   an argument. </param>
		''' <param name="b">   another argument. </param>
		''' <returns>  the smaller of {@code a} and {@code b.} </returns>
		Public Shared Function min(ByVal a As Single, ByVal b As Single) As Single
			Return System.Math.Min(a, b)
		End Function

		''' <summary>
		''' Returns the smaller of two {@code double} values.  That
		''' is, the result is the value closer to negative infinity. If the
		''' arguments have the same value, the result is that same
		''' value. If either value is NaN, then the result is NaN.  Unlike
		''' the numerical comparison operators, this method considers
		''' negative zero to be strictly smaller than positive zero. If one
		''' argument is positive zero and the other is negative zero, the
		''' result is negative zero.
		''' </summary>
		''' <param name="a">   an argument. </param>
		''' <param name="b">   another argument. </param>
		''' <returns>  the smaller of {@code a} and {@code b}. </returns>
		Public Shared Function min(ByVal a As Double, ByVal b As Double) As Double
			Return System.Math.Min(a, b)
		End Function

		''' <summary>
		''' Returns the size of an ulp of the argument.  An ulp, unit in
		''' the last place, of a {@code double} value is the positive
		''' distance between this floating-point value and the {@code
		''' double} value next larger in magnitude.  Note that for non-NaN
		''' <i>x</i>, <code>ulp(-<i>x</i>) == ulp(<i>x</i>)</code>.
		''' 
		''' <p>Special Cases:
		''' <ul>
		''' <li> If the argument is NaN, then the result is NaN.
		''' <li> If the argument is positive or negative infinity, then the
		''' result is positive infinity.
		''' <li> If the argument is positive or negative zero, then the result is
		''' {@code java.lang.[Double].MIN_VALUE}.
		''' <li> If the argument is &plusmn;{@code java.lang.[Double].MAX_VALUE}, then
		''' the result is equal to 2<sup>971</sup>.
		''' </ul>
		''' </summary>
		''' <param name="d"> the floating-point value whose ulp is to be returned </param>
		''' <returns> the size of an ulp of the argument
		''' @author Joseph D. Darcy
		''' @since 1.5 </returns>
		Public Shared Function ulp(ByVal d As Double) As Double
			Return System.Math.ulp(d)
		End Function

		''' <summary>
		''' Returns the size of an ulp of the argument.  An ulp, unit in
		''' the last place, of a {@code float} value is the positive
		''' distance between this floating-point value and the {@code
		''' float} value next larger in magnitude.  Note that for non-NaN
		''' <i>x</i>, <code>ulp(-<i>x</i>) == ulp(<i>x</i>)</code>.
		''' 
		''' <p>Special Cases:
		''' <ul>
		''' <li> If the argument is NaN, then the result is NaN.
		''' <li> If the argument is positive or negative infinity, then the
		''' result is positive infinity.
		''' <li> If the argument is positive or negative zero, then the result is
		''' {@code Float.MIN_VALUE}.
		''' <li> If the argument is &plusmn;{@code Float.MAX_VALUE}, then
		''' the result is equal to 2<sup>104</sup>.
		''' </ul>
		''' </summary>
		''' <param name="f"> the floating-point value whose ulp is to be returned </param>
		''' <returns> the size of an ulp of the argument
		''' @author Joseph D. Darcy
		''' @since 1.5 </returns>
		Public Shared Function ulp(ByVal f As Single) As Single
			Return System.Math.ulp(f)
		End Function

		''' <summary>
		''' Returns the signum function of the argument; zero if the argument
		''' is zero, 1.0 if the argument is greater than zero, -1.0 if the
		''' argument is less than zero.
		''' 
		''' <p>Special Cases:
		''' <ul>
		''' <li> If the argument is NaN, then the result is NaN.
		''' <li> If the argument is positive zero or negative zero, then the
		'''      result is the same as the argument.
		''' </ul>
		''' </summary>
		''' <param name="d"> the floating-point value whose signum is to be returned </param>
		''' <returns> the signum function of the argument
		''' @author Joseph D. Darcy
		''' @since 1.5 </returns>
		Public Shared Function signum(ByVal d As Double) As Double
			Return System.Math.Sign(d)
		End Function

		''' <summary>
		''' Returns the signum function of the argument; zero if the argument
		''' is zero, 1.0f if the argument is greater than zero, -1.0f if the
		''' argument is less than zero.
		''' 
		''' <p>Special Cases:
		''' <ul>
		''' <li> If the argument is NaN, then the result is NaN.
		''' <li> If the argument is positive zero or negative zero, then the
		'''      result is the same as the argument.
		''' </ul>
		''' </summary>
		''' <param name="f"> the floating-point value whose signum is to be returned </param>
		''' <returns> the signum function of the argument
		''' @author Joseph D. Darcy
		''' @since 1.5 </returns>
		Public Shared Function signum(ByVal f As Single) As Single
			Return System.Math.Sign(f)
		End Function

		''' <summary>
		''' Returns the hyperbolic sine of a {@code double} value.
		''' The hyperbolic sine of <i>x</i> is defined to be
		''' (<i>e<sup>x</sup>&nbsp;-&nbsp;e<sup>-x</sup></i>)/2
		''' where <i>e</i> is <seealso cref="Math#E Euler's number"/>.
		''' 
		''' <p>Special cases:
		''' <ul>
		''' 
		''' <li>If the argument is NaN, then the result is NaN.
		''' 
		''' <li>If the argument is infinite, then the result is an infinity
		''' with the same sign as the argument.
		''' 
		''' <li>If the argument is zero, then the result is a zero with the
		''' same sign as the argument.
		''' 
		''' </ul>
		''' </summary>
		''' <param name="x"> The number whose hyperbolic sine is to be returned. </param>
		''' <returns>  The hyperbolic sine of {@code x}.
		''' @since 1.5 </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function sinh(ByVal x As Double) As Double
		End Function

		''' <summary>
		''' Returns the hyperbolic cosine of a {@code double} value.
		''' The hyperbolic cosine of <i>x</i> is defined to be
		''' (<i>e<sup>x</sup>&nbsp;+&nbsp;e<sup>-x</sup></i>)/2
		''' where <i>e</i> is <seealso cref="Math#E Euler's number"/>.
		''' 
		''' <p>Special cases:
		''' <ul>
		''' 
		''' <li>If the argument is NaN, then the result is NaN.
		''' 
		''' <li>If the argument is infinite, then the result is positive
		''' infinity.
		''' 
		''' <li>If the argument is zero, then the result is {@code 1.0}.
		''' 
		''' </ul>
		''' </summary>
		''' <param name="x"> The number whose hyperbolic cosine is to be returned. </param>
		''' <returns>  The hyperbolic cosine of {@code x}.
		''' @since 1.5 </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function cosh(ByVal x As Double) As Double
		End Function

		''' <summary>
		''' Returns the hyperbolic tangent of a {@code double} value.
		''' The hyperbolic tangent of <i>x</i> is defined to be
		''' (<i>e<sup>x</sup>&nbsp;-&nbsp;e<sup>-x</sup></i>)/(<i>e<sup>x</sup>&nbsp;+&nbsp;e<sup>-x</sup></i>),
		''' in other words, {@link Math#sinh
		''' sinh(<i>x</i>)}/<seealso cref="Math#cosh cosh(<i>x</i>)"/>.  Note
		''' that the absolute value of the exact tanh is always less than
		''' 1.
		''' 
		''' <p>Special cases:
		''' <ul>
		''' 
		''' <li>If the argument is NaN, then the result is NaN.
		''' 
		''' <li>If the argument is zero, then the result is a zero with the
		''' same sign as the argument.
		''' 
		''' <li>If the argument is positive infinity, then the result is
		''' {@code +1.0}.
		''' 
		''' <li>If the argument is negative infinity, then the result is
		''' {@code -1.0}.
		''' 
		''' </ul>
		''' </summary>
		''' <param name="x"> The number whose hyperbolic tangent is to be returned. </param>
		''' <returns>  The hyperbolic tangent of {@code x}.
		''' @since 1.5 </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function tanh(ByVal x As Double) As Double
		End Function

		''' <summary>
		''' Returns sqrt(<i>x</i><sup>2</sup>&nbsp;+<i>y</i><sup>2</sup>)
		''' without intermediate overflow or underflow.
		''' 
		''' <p>Special cases:
		''' <ul>
		''' 
		''' <li> If either argument is infinite, then the result
		''' is positive infinity.
		''' 
		''' <li> If either argument is NaN and neither argument is infinite,
		''' then the result is NaN.
		''' 
		''' </ul>
		''' </summary>
		''' <param name="x"> a value </param>
		''' <param name="y"> a value </param>
		''' <returns> sqrt(<i>x</i><sup>2</sup>&nbsp;+<i>y</i><sup>2</sup>)
		''' without intermediate overflow or underflow
		''' @since 1.5 </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function hypot(ByVal x As Double, ByVal y As Double) As Double
		End Function

		''' <summary>
		''' Returns <i>e</i><sup>x</sup>&nbsp;-1.  Note that for values of
		''' <i>x</i> near 0, the exact sum of
		''' {@code expm1(x)}&nbsp;+&nbsp;1 is much closer to the true
		''' result of <i>e</i><sup>x</sup> than {@code exp(x)}.
		''' 
		''' <p>Special cases:
		''' <ul>
		''' <li>If the argument is NaN, the result is NaN.
		''' 
		''' <li>If the argument is positive infinity, then the result is
		''' positive infinity.
		''' 
		''' <li>If the argument is negative infinity, then the result is
		''' -1.0.
		''' 
		''' <li>If the argument is zero, then the result is a zero with the
		''' same sign as the argument.
		''' 
		''' </ul>
		''' </summary>
		''' <param name="x">   the exponent to raise <i>e</i> to in the computation of
		'''              <i>e</i><sup>{@code x}</sup>&nbsp;-1. </param>
		''' <returns>  the value <i>e</i><sup>{@code x}</sup>&nbsp;-&nbsp;1.
		''' @since 1.5 </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function expm1(ByVal x As Double) As Double
		End Function

		''' <summary>
		''' Returns the natural logarithm of the sum of the argument and 1.
		''' Note that for small values {@code x}, the result of
		''' {@code log1p(x)} is much closer to the true result of ln(1
		''' + {@code x}) than the floating-point evaluation of
		''' {@code log(1.0+x)}.
		''' 
		''' <p>Special cases:
		''' <ul>
		''' 
		''' <li>If the argument is NaN or less than -1, then the result is
		''' NaN.
		''' 
		''' <li>If the argument is positive infinity, then the result is
		''' positive infinity.
		''' 
		''' <li>If the argument is negative one, then the result is
		''' negative infinity.
		''' 
		''' <li>If the argument is zero, then the result is a zero with the
		''' same sign as the argument.
		''' 
		''' </ul>
		''' </summary>
		''' <param name="x">   a value </param>
		''' <returns> the value ln({@code x}&nbsp;+&nbsp;1), the natural
		''' log of {@code x}&nbsp;+&nbsp;1
		''' @since 1.5 </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function log1p(ByVal x As Double) As Double
		End Function

		''' <summary>
		''' Returns the first floating-point argument with the sign of the
		''' second floating-point argument.  For this method, a NaN
		''' {@code sign} argument is always treated as if it were
		''' positive.
		''' </summary>
		''' <param name="magnitude">  the parameter providing the magnitude of the result </param>
		''' <param name="sign">   the parameter providing the sign of the result </param>
		''' <returns> a value with the magnitude of {@code magnitude}
		''' and the sign of {@code sign}.
		''' @since 1.6 </returns>
		Public Shared Function copySign(ByVal magnitude As Double, ByVal sign As Double) As Double
			Return System.Math.copySign(magnitude, (If(Double.IsNaN(sign), 1.0R, sign)))
		End Function

		''' <summary>
		''' Returns the first floating-point argument with the sign of the
		''' second floating-point argument.  For this method, a NaN
		''' {@code sign} argument is always treated as if it were
		''' positive.
		''' </summary>
		''' <param name="magnitude">  the parameter providing the magnitude of the result </param>
		''' <param name="sign">   the parameter providing the sign of the result </param>
		''' <returns> a value with the magnitude of {@code magnitude}
		''' and the sign of {@code sign}.
		''' @since 1.6 </returns>
		Public Shared Function copySign(ByVal magnitude As Single, ByVal sign As Single) As Single
			Return System.Math.copySign(magnitude, (If(Float.IsNaN(sign), 1.0f, sign)))
		End Function
		''' <summary>
		''' Returns the unbiased exponent used in the representation of a
		''' {@code float}.  Special cases:
		''' 
		''' <ul>
		''' <li>If the argument is NaN or infinite, then the result is
		''' <seealso cref="Float#MAX_EXPONENT"/> + 1.
		''' <li>If the argument is zero or subnormal, then the result is
		''' <seealso cref="Float#MIN_EXPONENT"/> -1.
		''' </ul> </summary>
		''' <param name="f"> a {@code float} value </param>
		''' <returns> the unbiased exponent of the argument
		''' @since 1.6 </returns>
		Public Shared Function getExponent(ByVal f As Single) As Integer
			Return System.Math.getExponent(f)
		End Function

		''' <summary>
		''' Returns the unbiased exponent used in the representation of a
		''' {@code double}.  Special cases:
		''' 
		''' <ul>
		''' <li>If the argument is NaN or infinite, then the result is
		''' <seealso cref="Double#MAX_EXPONENT"/> + 1.
		''' <li>If the argument is zero or subnormal, then the result is
		''' <seealso cref="Double#MIN_EXPONENT"/> -1.
		''' </ul> </summary>
		''' <param name="d"> a {@code double} value </param>
		''' <returns> the unbiased exponent of the argument
		''' @since 1.6 </returns>
		Public Shared Function getExponent(ByVal d As Double) As Integer
			Return System.Math.getExponent(d)
		End Function

		''' <summary>
		''' Returns the floating-point number adjacent to the first
		''' argument in the direction of the second argument.  If both
		''' arguments compare as equal the second argument is returned.
		''' 
		''' <p>Special cases:
		''' <ul>
		''' <li> If either argument is a NaN, then NaN is returned.
		''' 
		''' <li> If both arguments are signed zeros, {@code direction}
		''' is returned unchanged (as implied by the requirement of
		''' returning the second argument if the arguments compare as
		''' equal).
		''' 
		''' <li> If {@code start} is
		''' &plusmn;<seealso cref="Double#MIN_VALUE"/> and {@code direction}
		''' has a value such that the result should have a smaller
		''' magnitude, then a zero with the same sign as {@code start}
		''' is returned.
		''' 
		''' <li> If {@code start} is infinite and
		''' {@code direction} has a value such that the result should
		''' have a smaller magnitude, <seealso cref="Double#MAX_VALUE"/> with the
		''' same sign as {@code start} is returned.
		''' 
		''' <li> If {@code start} is equal to &plusmn;
		''' <seealso cref="Double#MAX_VALUE"/> and {@code direction} has a
		''' value such that the result should have a larger magnitude, an
		''' infinity with same sign as {@code start} is returned.
		''' </ul>
		''' </summary>
		''' <param name="start">  starting floating-point value </param>
		''' <param name="direction"> value indicating which of
		''' {@code start}'s neighbors or {@code start} should
		''' be returned </param>
		''' <returns> The floating-point number adjacent to {@code start} in the
		''' direction of {@code direction}.
		''' @since 1.6 </returns>
		Public Shared Function nextAfter(ByVal start As Double, ByVal direction As Double) As Double
			Return System.Math.nextAfter(start, direction)
		End Function

		''' <summary>
		''' Returns the floating-point number adjacent to the first
		''' argument in the direction of the second argument.  If both
		''' arguments compare as equal a value equivalent to the second argument
		''' is returned.
		''' 
		''' <p>Special cases:
		''' <ul>
		''' <li> If either argument is a NaN, then NaN is returned.
		''' 
		''' <li> If both arguments are signed zeros, a value equivalent
		''' to {@code direction} is returned.
		''' 
		''' <li> If {@code start} is
		''' &plusmn;<seealso cref="Float#MIN_VALUE"/> and {@code direction}
		''' has a value such that the result should have a smaller
		''' magnitude, then a zero with the same sign as {@code start}
		''' is returned.
		''' 
		''' <li> If {@code start} is infinite and
		''' {@code direction} has a value such that the result should
		''' have a smaller magnitude, <seealso cref="Float#MAX_VALUE"/> with the
		''' same sign as {@code start} is returned.
		''' 
		''' <li> If {@code start} is equal to &plusmn;
		''' <seealso cref="Float#MAX_VALUE"/> and {@code direction} has a
		''' value such that the result should have a larger magnitude, an
		''' infinity with same sign as {@code start} is returned.
		''' </ul>
		''' </summary>
		''' <param name="start">  starting floating-point value </param>
		''' <param name="direction"> value indicating which of
		''' {@code start}'s neighbors or {@code start} should
		''' be returned </param>
		''' <returns> The floating-point number adjacent to {@code start} in the
		''' direction of {@code direction}.
		''' @since 1.6 </returns>
		Public Shared Function nextAfter(ByVal start As Single, ByVal direction As Double) As Single
			Return System.Math.nextAfter(start, direction)
		End Function

		''' <summary>
		''' Returns the floating-point value adjacent to {@code d} in
		''' the direction of positive infinity.  This method is
		''' semantically equivalent to {@code nextAfter(d,
		''' java.lang.[Double].POSITIVE_INFINITY)}; however, a {@code nextUp}
		''' implementation may run faster than its equivalent
		''' {@code nextAfter} call.
		''' 
		''' <p>Special Cases:
		''' <ul>
		''' <li> If the argument is NaN, the result is NaN.
		''' 
		''' <li> If the argument is positive infinity, the result is
		''' positive infinity.
		''' 
		''' <li> If the argument is zero, the result is
		''' <seealso cref="Double#MIN_VALUE"/>
		''' 
		''' </ul>
		''' </summary>
		''' <param name="d"> starting floating-point value </param>
		''' <returns> The adjacent floating-point value closer to positive
		''' infinity.
		''' @since 1.6 </returns>
		Public Shared Function nextUp(ByVal d As Double) As Double
			Return System.Math.nextUp(d)
		End Function

		''' <summary>
		''' Returns the floating-point value adjacent to {@code f} in
		''' the direction of positive infinity.  This method is
		''' semantically equivalent to {@code nextAfter(f,
		''' Float.POSITIVE_INFINITY)}; however, a {@code nextUp}
		''' implementation may run faster than its equivalent
		''' {@code nextAfter} call.
		''' 
		''' <p>Special Cases:
		''' <ul>
		''' <li> If the argument is NaN, the result is NaN.
		''' 
		''' <li> If the argument is positive infinity, the result is
		''' positive infinity.
		''' 
		''' <li> If the argument is zero, the result is
		''' <seealso cref="Float#MIN_VALUE"/>
		''' 
		''' </ul>
		''' </summary>
		''' <param name="f"> starting floating-point value </param>
		''' <returns> The adjacent floating-point value closer to positive
		''' infinity.
		''' @since 1.6 </returns>
		Public Shared Function nextUp(ByVal f As Single) As Single
			Return System.Math.nextUp(f)
		End Function

		''' <summary>
		''' Returns the floating-point value adjacent to {@code d} in
		''' the direction of negative infinity.  This method is
		''' semantically equivalent to {@code nextAfter(d,
		''' java.lang.[Double].NEGATIVE_INFINITY)}; however, a
		''' {@code nextDown} implementation may run faster than its
		''' equivalent {@code nextAfter} call.
		''' 
		''' <p>Special Cases:
		''' <ul>
		''' <li> If the argument is NaN, the result is NaN.
		''' 
		''' <li> If the argument is negative infinity, the result is
		''' negative infinity.
		''' 
		''' <li> If the argument is zero, the result is
		''' {@code -Double.MIN_VALUE}
		''' 
		''' </ul>
		''' </summary>
		''' <param name="d">  starting floating-point value </param>
		''' <returns> The adjacent floating-point value closer to negative
		''' infinity.
		''' @since 1.8 </returns>
		Public Shared Function nextDown(ByVal d As Double) As Double
			Return System.Math.nextDown(d)
		End Function

		''' <summary>
		''' Returns the floating-point value adjacent to {@code f} in
		''' the direction of negative infinity.  This method is
		''' semantically equivalent to {@code nextAfter(f,
		''' Float.NEGATIVE_INFINITY)}; however, a
		''' {@code nextDown} implementation may run faster than its
		''' equivalent {@code nextAfter} call.
		''' 
		''' <p>Special Cases:
		''' <ul>
		''' <li> If the argument is NaN, the result is NaN.
		''' 
		''' <li> If the argument is negative infinity, the result is
		''' negative infinity.
		''' 
		''' <li> If the argument is zero, the result is
		''' {@code -Float.MIN_VALUE}
		''' 
		''' </ul>
		''' </summary>
		''' <param name="f">  starting floating-point value </param>
		''' <returns> The adjacent floating-point value closer to negative
		''' infinity.
		''' @since 1.8 </returns>
		Public Shared Function nextDown(ByVal f As Single) As Single
			Return System.Math.nextDown(f)
		End Function

		''' <summary>
		''' Returns {@code d} &times;
		''' 2<sup>{@code scaleFactor}</sup> rounded as if performed
		''' by a single correctly rounded floating-point multiply to a
		''' member of the double value set.  See the Java
		''' Language Specification for a discussion of floating-point
		''' value sets.  If the exponent of the result is between {@link
		''' Double#MIN_EXPONENT} and <seealso cref="Double#MAX_EXPONENT"/>, the
		''' answer is calculated exactly.  If the exponent of the result
		''' would be larger than {@code java.lang.[Double].MAX_EXPONENT}, an
		''' infinity is returned.  Note that if the result is subnormal,
		''' precision may be lost; that is, when {@code scalb(x, n)}
		''' is subnormal, {@code scalb(scalb(x, n), -n)} may not equal
		''' <i>x</i>.  When the result is non-NaN, the result has the same
		''' sign as {@code d}.
		''' 
		''' <p>Special cases:
		''' <ul>
		''' <li> If the first argument is NaN, NaN is returned.
		''' <li> If the first argument is infinite, then an infinity of the
		''' same sign is returned.
		''' <li> If the first argument is zero, then a zero of the same
		''' sign is returned.
		''' </ul>
		''' </summary>
		''' <param name="d"> number to be scaled by a power of two. </param>
		''' <param name="scaleFactor"> power of 2 used to scale {@code d} </param>
		''' <returns> {@code d} &times; 2<sup>{@code scaleFactor}</sup>
		''' @since 1.6 </returns>
		Public Shared Function scalb(ByVal d As Double, ByVal scaleFactor As Integer) As Double
			Return System.Math.scalb(d, scaleFactor)
		End Function

		''' <summary>
		''' Returns {@code f} &times;
		''' 2<sup>{@code scaleFactor}</sup> rounded as if performed
		''' by a single correctly rounded floating-point multiply to a
		''' member of the float value set.  See the Java
		''' Language Specification for a discussion of floating-point
		''' value sets.  If the exponent of the result is between {@link
		''' Float#MIN_EXPONENT} and <seealso cref="Float#MAX_EXPONENT"/>, the
		''' answer is calculated exactly.  If the exponent of the result
		''' would be larger than {@code Float.MAX_EXPONENT}, an
		''' infinity is returned.  Note that if the result is subnormal,
		''' precision may be lost; that is, when {@code scalb(x, n)}
		''' is subnormal, {@code scalb(scalb(x, n), -n)} may not equal
		''' <i>x</i>.  When the result is non-NaN, the result has the same
		''' sign as {@code f}.
		''' 
		''' <p>Special cases:
		''' <ul>
		''' <li> If the first argument is NaN, NaN is returned.
		''' <li> If the first argument is infinite, then an infinity of the
		''' same sign is returned.
		''' <li> If the first argument is zero, then a zero of the same
		''' sign is returned.
		''' </ul>
		''' </summary>
		''' <param name="f"> number to be scaled by a power of two. </param>
		''' <param name="scaleFactor"> power of 2 used to scale {@code f} </param>
		''' <returns> {@code f} &times; 2<sup>{@code scaleFactor}</sup>
		''' @since 1.6 </returns>
		Public Shared Function scalb(ByVal f As Single, ByVal scaleFactor As Integer) As Single
			Return System.Math.scalb(f, scaleFactor)
		End Function
	End Class

End Namespace
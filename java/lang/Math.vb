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
	''' The class {@code Math} contains methods for performing basic
	''' numeric operations such as the elementary exponential, logarithm,
	''' square root, and trigonometric functions.
	''' 
	''' <p>Unlike some of the numeric methods of class
	''' {@code StrictMath}, all implementations of the equivalent
	''' functions of class {@code Math} are not defined to return the
	''' bit-for-bit same results.  This relaxation permits
	''' better-performing implementations where strict reproducibility is
	''' not required.
	''' 
	''' <p>By default many of the {@code Math} methods simply call
	''' the equivalent method in {@code StrictMath} for their
	''' implementation.  Code generators are encouraged to use
	''' platform-specific native libraries or microprocessor instructions,
	''' where available, to provide higher-performance implementations of
	''' {@code Math} methods.  Such higher-performance
	''' implementations still must conform to the specification for
	''' {@code Math}.
	''' 
	''' <p>The quality of implementation specifications concern two
	''' properties, accuracy of the returned result and monotonicity of the
	''' method.  Accuracy of the floating-point {@code Math} methods is
	''' measured in terms of <i>ulps</i>, units in the last place.  For a
	''' given floating-point format, an <seealso cref="#ulp(double) ulp"/> of a
	''' specific real number value is the distance between the two
	''' floating-point values bracketing that numerical value.  When
	''' discussing the accuracy of a method as a whole rather than at a
	''' specific argument, the number of ulps cited is for the worst-case
	''' error at any argument.  If a method always has an error less than
	''' 0.5 ulps, the method always returns the floating-point number
	''' nearest the exact result; such a method is <i>correctly
	''' rounded</i>.  A correctly rounded method is generally the best a
	''' floating-point approximation can be; however, it is impractical for
	''' many floating-point methods to be correctly rounded.  Instead, for
	''' the {@code Math} class, a larger error bound of 1 or 2 ulps is
	''' allowed for certain methods.  Informally, with a 1 ulp error bound,
	''' when the exact result is a representable number, the exact result
	''' should be returned as the computed result; otherwise, either of the
	''' two floating-point values which bracket the exact result may be
	''' returned.  For exact results large in magnitude, one of the
	''' endpoints of the bracket may be infinite.  Besides accuracy at
	''' individual arguments, maintaining proper relations between the
	''' method at different arguments is also important.  Therefore, most
	''' methods with more than 0.5 ulp errors are required to be
	''' <i>semi-monotonic</i>: whenever the mathematical function is
	''' non-decreasing, so is the floating-point approximation, likewise,
	''' whenever the mathematical function is non-increasing, so is the
	''' floating-point approximation.  Not all approximations that have 1
	''' ulp accuracy will automatically meet the monotonicity requirements.
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
	''' @since   JDK1.0
	''' </summary>

	Public NotInheritable Class Math

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
		''' Returns the trigonometric sine of an angle.  Special cases:
		''' <ul><li>If the argument is NaN or an infinity, then the
		''' result is NaN.
		''' <li>If the argument is zero, then the result is a zero with the
		''' same sign as the argument.</ul>
		''' 
		''' <p>The computed result must be within 1 ulp of the exact result.
		''' Results must be semi-monotonic.
		''' </summary>
		''' <param name="a">   an angle, in radians. </param>
		''' <returns>  the sine of the argument. </returns>
		Public Shared Function sin(ByVal a As Double) As Double
			Return Math.Sin(a) ' default impl. delegates to StrictMath
		End Function

		''' <summary>
		''' Returns the trigonometric cosine of an angle. Special cases:
		''' <ul><li>If the argument is NaN or an infinity, then the
		''' result is NaN.</ul>
		''' 
		''' <p>The computed result must be within 1 ulp of the exact result.
		''' Results must be semi-monotonic.
		''' </summary>
		''' <param name="a">   an angle, in radians. </param>
		''' <returns>  the cosine of the argument. </returns>
		Public Shared Function cos(ByVal a As Double) As Double
			Return Math.Cos(a) ' default impl. delegates to StrictMath
		End Function

		''' <summary>
		''' Returns the trigonometric tangent of an angle.  Special cases:
		''' <ul><li>If the argument is NaN or an infinity, then the result
		''' is NaN.
		''' <li>If the argument is zero, then the result is a zero with the
		''' same sign as the argument.</ul>
		''' 
		''' <p>The computed result must be within 1 ulp of the exact result.
		''' Results must be semi-monotonic.
		''' </summary>
		''' <param name="a">   an angle, in radians. </param>
		''' <returns>  the tangent of the argument. </returns>
		Public Shared Function tan(ByVal a As Double) As Double
			Return Math.Tan(a) ' default impl. delegates to StrictMath
		End Function

		''' <summary>
		''' Returns the arc sine of a value; the returned angle is in the
		''' range -<i>pi</i>/2 through <i>pi</i>/2.  Special cases:
		''' <ul><li>If the argument is NaN or its absolute value is greater
		''' than 1, then the result is NaN.
		''' <li>If the argument is zero, then the result is a zero with the
		''' same sign as the argument.</ul>
		''' 
		''' <p>The computed result must be within 1 ulp of the exact result.
		''' Results must be semi-monotonic.
		''' </summary>
		''' <param name="a">   the value whose arc sine is to be returned. </param>
		''' <returns>  the arc sine of the argument. </returns>
		Public Shared Function asin(ByVal a As Double) As Double
			Return Math.Asin(a) ' default impl. delegates to StrictMath
		End Function

		''' <summary>
		''' Returns the arc cosine of a value; the returned angle is in the
		''' range 0.0 through <i>pi</i>.  Special case:
		''' <ul><li>If the argument is NaN or its absolute value is greater
		''' than 1, then the result is NaN.</ul>
		''' 
		''' <p>The computed result must be within 1 ulp of the exact result.
		''' Results must be semi-monotonic.
		''' </summary>
		''' <param name="a">   the value whose arc cosine is to be returned. </param>
		''' <returns>  the arc cosine of the argument. </returns>
		Public Shared Function acos(ByVal a As Double) As Double
			Return Math.Acos(a) ' default impl. delegates to StrictMath
		End Function

		''' <summary>
		''' Returns the arc tangent of a value; the returned angle is in the
		''' range -<i>pi</i>/2 through <i>pi</i>/2.  Special cases:
		''' <ul><li>If the argument is NaN, then the result is NaN.
		''' <li>If the argument is zero, then the result is a zero with the
		''' same sign as the argument.</ul>
		''' 
		''' <p>The computed result must be within 1 ulp of the exact result.
		''' Results must be semi-monotonic.
		''' </summary>
		''' <param name="a">   the value whose arc tangent is to be returned. </param>
		''' <returns>  the arc tangent of the argument. </returns>
		Public Shared Function atan(ByVal a As Double) As Double
			Return Math.Atan(a) ' default impl. delegates to StrictMath
		End Function

		''' <summary>
		''' Converts an angle measured in degrees to an approximately
		''' equivalent angle measured in radians.  The conversion from
		''' degrees to radians is generally inexact.
		''' </summary>
		''' <param name="angdeg">   an angle, in degrees </param>
		''' <returns>  the measurement of the angle {@code angdeg}
		'''          in radians.
		''' @since   1.2 </returns>
		Public Shared Function toRadians(ByVal angdeg As Double) As Double
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
		'''          in degrees.
		''' @since   1.2 </returns>
		Public Shared Function toDegrees(ByVal angrad As Double) As Double
			Return angrad * 180.0 / PI
		End Function

		''' <summary>
		''' Returns Euler's number <i>e</i> raised to the power of a
		''' {@code double} value.  Special cases:
		''' <ul><li>If the argument is NaN, the result is NaN.
		''' <li>If the argument is positive infinity, then the result is
		''' positive infinity.
		''' <li>If the argument is negative infinity, then the result is
		''' positive zero.</ul>
		''' 
		''' <p>The computed result must be within 1 ulp of the exact result.
		''' Results must be semi-monotonic.
		''' </summary>
		''' <param name="a">   the exponent to raise <i>e</i> to. </param>
		''' <returns>  the value <i>e</i><sup>{@code a}</sup>,
		'''          where <i>e</i> is the base of the natural logarithms. </returns>
		Public Shared Function exp(ByVal a As Double) As Double
			Return Math.Exp(a) ' default impl. delegates to StrictMath
		End Function

		''' <summary>
		''' Returns the natural logarithm (base <i>e</i>) of a {@code double}
		''' value.  Special cases:
		''' <ul><li>If the argument is NaN or less than zero, then the result
		''' is NaN.
		''' <li>If the argument is positive infinity, then the result is
		''' positive infinity.
		''' <li>If the argument is positive zero or negative zero, then the
		''' result is negative infinity.</ul>
		''' 
		''' <p>The computed result must be within 1 ulp of the exact result.
		''' Results must be semi-monotonic.
		''' </summary>
		''' <param name="a">   a value </param>
		''' <returns>  the value ln&nbsp;{@code a}, the natural logarithm of
		'''          {@code a}. </returns>
		Public Shared Function log(ByVal a As Double) As Double
			Return Math.Log(a) ' default impl. delegates to StrictMath
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
		''' 
		''' <p>The computed result must be within 1 ulp of the exact result.
		''' Results must be semi-monotonic.
		''' </summary>
		''' <param name="a">   a value </param>
		''' <returns>  the base 10 logarithm of  {@code a}.
		''' @since 1.5 </returns>
		Public Shared Function log10(ByVal a As Double) As Double
			Return Math.Log10(a) ' default impl. delegates to StrictMath
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
		''' <returns>  the positive square root of {@code a}.
		'''          If the argument is NaN or less than zero, the result is NaN. </returns>
		Public Shared Function sqrt(ByVal a As Double) As Double
			Return Math.Sqrt(a) ' default impl. delegates to StrictMath
									   ' Note that hardware sqrt instructions
									   ' frequently can be directly used by JITs
									   ' and should be much faster than doing
									   ' Math.sqrt in software.
		End Function


		''' <summary>
		''' Returns the cube root of a {@code double} value.  For
		''' positive finite {@code x}, {@code cbrt(-x) ==
		''' -cbrt(x)}; that is, the cube root of a negative value is
		''' the negative of the cube root of that value's magnitude.
		''' 
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
		''' 
		''' <p>The computed result must be within 1 ulp of the exact result.
		''' </summary>
		''' <param name="a">   a value. </param>
		''' <returns>  the cube root of {@code a}.
		''' @since 1.5 </returns>
		Public Shared Function cbrt(ByVal a As Double) As Double
			Return Math.cbrt(a)
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
		Public Shared Function IEEEremainder(ByVal f1 As Double, ByVal f2 As Double) As Double
			Return Math.IEEERemainder(f1, f2) ' delegate to StrictMath
		End Function

		''' <summary>
		''' Returns the smallest (closest to negative infinity)
		''' {@code double} value that is greater than or equal to the
		''' argument and is equal to a mathematical integer. Special cases:
		''' <ul><li>If the argument value is already equal to a
		''' mathematical integer, then the result is the same as the
		''' argument.  <li>If the argument is NaN or an infinity or
		''' positive zero or negative zero, then the result is the same as
		''' the argument.  <li>If the argument value is less than zero but
		''' greater than -1.0, then the result is negative zero.</ul> Note
		''' that the value of {@code Math.ceil(x)} is exactly the
		''' value of {@code -Math.floor(-x)}.
		''' 
		''' </summary>
		''' <param name="a">   a value. </param>
		''' <returns>  the smallest (closest to negative infinity)
		'''          floating-point value that is greater than or equal to
		'''          the argument and is equal to a mathematical integer. </returns>
		Public Shared Function ceil(ByVal a As Double) As Double
			Return Math.Ceiling(a) ' default impl. delegates to StrictMath
		End Function

		''' <summary>
		''' Returns the largest (closest to positive infinity)
		''' {@code double} value that is less than or equal to the
		''' argument and is equal to a mathematical integer. Special cases:
		''' <ul><li>If the argument value is already equal to a
		''' mathematical integer, then the result is the same as the
		''' argument.  <li>If the argument is NaN or an infinity or
		''' positive zero or negative zero, then the result is the same as
		''' the argument.</ul>
		''' </summary>
		''' <param name="a">   a value. </param>
		''' <returns>  the largest (closest to positive infinity)
		'''          floating-point value that less than or equal to the argument
		'''          and is equal to a mathematical integer. </returns>
		Public Shared Function floor(ByVal a As Double) As Double
			Return Math.Floor(a) ' default impl. delegates to StrictMath
		End Function

		''' <summary>
		''' Returns the {@code double} value that is closest in value
		''' to the argument and is equal to a mathematical integer. If two
		''' {@code double} values that are mathematical integers are
		''' equally close, the result is the integer value that is
		''' even. Special cases:
		''' <ul><li>If the argument value is already equal to a mathematical
		''' integer, then the result is the same as the argument.
		''' <li>If the argument is NaN or an infinity or positive zero or negative
		''' zero, then the result is the same as the argument.</ul>
		''' </summary>
		''' <param name="a">   a {@code double} value. </param>
		''' <returns>  the closest floating-point value to {@code a} that is
		'''          equal to a mathematical integer. </returns>
		Public Shared Function rint(ByVal a As Double) As Double
			Return Math.rint(a) ' default impl. delegates to StrictMath
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
		''' 
		''' <p>The computed result must be within 2 ulps of the exact result.
		''' Results must be semi-monotonic.
		''' </summary>
		''' <param name="y">   the ordinate coordinate </param>
		''' <param name="x">   the abscissa coordinate </param>
		''' <returns>  the <i>theta</i> component of the point
		'''          (<i>r</i>,&nbsp;<i>theta</i>)
		'''          in polar coordinates that corresponds to the point
		'''          (<i>x</i>,&nbsp;<i>y</i>) in Cartesian coordinates. </returns>
		Public Shared Function atan2(ByVal y As Double, ByVal x As Double) As Double
			Return Math.Atan2(y, x) ' default impl. delegates to StrictMath
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
		''' 
		''' <p>The computed result must be within 1 ulp of the exact result.
		''' Results must be semi-monotonic.
		''' </summary>
		''' <param name="a">   the base. </param>
		''' <param name="b">   the exponent. </param>
		''' <returns>  the value {@code a}<sup>{@code b}</sup>. </returns>
		Public Shared Function pow(ByVal a As Double, ByVal b As Double) As Double
			Return Math.Pow(a, b) ' default impl. delegates to StrictMath
		End Function

		''' <summary>
		''' Returns the closest {@code int} to the argument, with ties
		''' rounding to positive infinity.
		''' 
		''' <p>
		''' Special cases:
		''' <ul><li>If the argument is NaN, the result is 0.
		''' <li>If the argument is negative infinity or any value less than or
		''' equal to the value of {@code Integer.MIN_VALUE}, the result is
		''' equal to the value of {@code Integer.MIN_VALUE}.
		''' <li>If the argument is positive infinity or any value greater than or
		''' equal to the value of {@code Integer.MAX_VALUE}, the result is
		''' equal to the value of {@code Integer.MAX_VALUE}.</ul>
		''' </summary>
		''' <param name="a">   a floating-point value to be rounded to an integer. </param>
		''' <returns>  the value of the argument rounded to the nearest
		'''          {@code int} value. </returns>
		''' <seealso cref=     java.lang.Integer#MAX_VALUE </seealso>
		''' <seealso cref=     java.lang.Integer#MIN_VALUE </seealso>
		Public Shared Function round(ByVal a As Single) As Integer
			Dim intBits As Integer = Float.floatToRawIntBits(a)
			Dim biasedExp As Integer = (intBits And sun.misc.FloatConsts.EXP_BIT_MASK) >> (sun.misc.FloatConsts.SIGNIFICAND_WIDTH - 1)
			Dim shift As Integer = (sun.misc.FloatConsts.SIGNIFICAND_WIDTH - 2 + sun.misc.FloatConsts.EXP_BIAS) - biasedExp
			If (shift And -32) = 0 Then ' shift >= 0 && shift < 32
				' a is a finite number such that pow(2,-32) <= ulp(a) < 1
				Dim r As Integer = ((intBits And sun.misc.FloatConsts.SIGNIF_BIT_MASK) Or (sun.misc.FloatConsts.SIGNIF_BIT_MASK + 1))
				If intBits < 0 Then r = -r
				' In the comments below each Java expression evaluates to the value
				' the corresponding mathematical expression:
				' (r) evaluates to a / ulp(a)
				' (r >> shift) evaluates to floor(a * 2)
				' ((r >> shift) + 1) evaluates to floor((a + 1/2) * 2)
				' (((r >> shift) + 1) >> 1) evaluates to floor(a + 1/2)
				Return ((r >> shift) + 1) >> 1
			Else
				' a is either
				' - a finite number with abs(a) < exp(2,FloatConsts.SIGNIFICAND_WIDTH-32) < 1/2
				' - a finite number with ulp(a) >= 1 and hence a is a mathematical integer
				' - an infinity or NaN
				Return CInt(Fix(a))
			End If
		End Function

		''' <summary>
		''' Returns the closest {@code long} to the argument, with ties
		''' rounding to positive infinity.
		''' 
		''' <p>Special cases:
		''' <ul><li>If the argument is NaN, the result is 0.
		''' <li>If the argument is negative infinity or any value less than or
		''' equal to the value of {@code Long.MIN_VALUE}, the result is
		''' equal to the value of {@code Long.MIN_VALUE}.
		''' <li>If the argument is positive infinity or any value greater than or
		''' equal to the value of {@code Long.MAX_VALUE}, the result is
		''' equal to the value of {@code Long.MAX_VALUE}.</ul>
		''' </summary>
		''' <param name="a">   a floating-point value to be rounded to a
		'''          {@code long}. </param>
		''' <returns>  the value of the argument rounded to the nearest
		'''          {@code long} value. </returns>
		''' <seealso cref=     java.lang.Long#MAX_VALUE </seealso>
		''' <seealso cref=     java.lang.Long#MIN_VALUE </seealso>
		Public Shared Function round(ByVal a As Double) As Long
			Dim longBits As Long = Double.doubleToRawLongBits(a)
			Dim biasedExp As Long = (longBits And sun.misc.DoubleConsts.EXP_BIT_MASK) >> (sun.misc.DoubleConsts.SIGNIFICAND_WIDTH - 1)
			Dim shift As Long = (sun.misc.DoubleConsts.SIGNIFICAND_WIDTH - 2 + sun.misc.DoubleConsts.EXP_BIAS) - biasedExp
			If (shift And -64) = 0 Then ' shift >= 0 && shift < 64
				' a is a finite number such that pow(2,-64) <= ulp(a) < 1
				Dim r As Long = ((longBits And sun.misc.DoubleConsts.SIGNIF_BIT_MASK) Or (sun.misc.DoubleConsts.SIGNIF_BIT_MASK + 1))
				If longBits < 0 Then r = -r
				' In the comments below each Java expression evaluates to the value
				' the corresponding mathematical expression:
				' (r) evaluates to a / ulp(a)
				' (r >> shift) evaluates to floor(a * 2)
				' ((r >> shift) + 1) evaluates to floor((a + 1/2) * 2)
				' (((r >> shift) + 1) >> 1) evaluates to floor(a + 1/2)
				Return ((r >> shift) + 1) >> 1
			Else
				' a is either
				' - a finite number with abs(a) < exp(2,DoubleConsts.SIGNIFICAND_WIDTH-64) < 1/2
				' - a finite number with ulp(a) >= 1 and hence a is a mathematical integer
				' - an infinity or NaN
				Return CLng(Fix(a))
			End If
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
		''' <exception cref="ArithmeticException"> if the result overflows an int
		''' @since 1.8 </exception>
		Public Shared Function addExact(ByVal x As Integer, ByVal y As Integer) As Integer
			Dim r As Integer = x + y
			' HD 2-12 Overflow iff both arguments have the opposite sign of the result
			If ((x Xor r) And (y Xor r)) < 0 Then Throw New ArithmeticException("integer overflow")
			Return r
		End Function

		''' <summary>
		''' Returns the sum of its arguments,
		''' throwing an exception if the result overflows a {@code long}.
		''' </summary>
		''' <param name="x"> the first value </param>
		''' <param name="y"> the second value </param>
		''' <returns> the result </returns>
		''' <exception cref="ArithmeticException"> if the result overflows a long
		''' @since 1.8 </exception>
		Public Shared Function addExact(ByVal x As Long, ByVal y As Long) As Long
			Dim r As Long = x + y
			' HD 2-12 Overflow iff both arguments have the opposite sign of the result
			If ((x Xor r) And (y Xor r)) < 0 Then Throw New ArithmeticException("long overflow")
			Return r
		End Function

		''' <summary>
		''' Returns the difference of the arguments,
		''' throwing an exception if the result overflows an {@code int}.
		''' </summary>
		''' <param name="x"> the first value </param>
		''' <param name="y"> the second value to subtract from the first </param>
		''' <returns> the result </returns>
		''' <exception cref="ArithmeticException"> if the result overflows an int
		''' @since 1.8 </exception>
		Public Shared Function subtractExact(ByVal x As Integer, ByVal y As Integer) As Integer
			Dim r As Integer = x - y
			' HD 2-12 Overflow iff the arguments have different signs and
			' the sign of the result is different than the sign of x
			If ((x Xor y) And (x Xor r)) < 0 Then Throw New ArithmeticException("integer overflow")
			Return r
		End Function

		''' <summary>
		''' Returns the difference of the arguments,
		''' throwing an exception if the result overflows a {@code long}.
		''' </summary>
		''' <param name="x"> the first value </param>
		''' <param name="y"> the second value to subtract from the first </param>
		''' <returns> the result </returns>
		''' <exception cref="ArithmeticException"> if the result overflows a long
		''' @since 1.8 </exception>
		Public Shared Function subtractExact(ByVal x As Long, ByVal y As Long) As Long
			Dim r As Long = x - y
			' HD 2-12 Overflow iff the arguments have different signs and
			' the sign of the result is different than the sign of x
			If ((x Xor y) And (x Xor r)) < 0 Then Throw New ArithmeticException("long overflow")
			Return r
		End Function

		''' <summary>
		''' Returns the product of the arguments,
		''' throwing an exception if the result overflows an {@code int}.
		''' </summary>
		''' <param name="x"> the first value </param>
		''' <param name="y"> the second value </param>
		''' <returns> the result </returns>
		''' <exception cref="ArithmeticException"> if the result overflows an int
		''' @since 1.8 </exception>
		Public Shared Function multiplyExact(ByVal x As Integer, ByVal y As Integer) As Integer
			Dim r As Long = CLng(x) * CLng(y)
			If CInt(r) <> r Then Throw New ArithmeticException("integer overflow")
			Return CInt(r)
		End Function

		''' <summary>
		''' Returns the product of the arguments,
		''' throwing an exception if the result overflows a {@code long}.
		''' </summary>
		''' <param name="x"> the first value </param>
		''' <param name="y"> the second value </param>
		''' <returns> the result </returns>
		''' <exception cref="ArithmeticException"> if the result overflows a long
		''' @since 1.8 </exception>
		Public Shared Function multiplyExact(ByVal x As Long, ByVal y As Long) As Long
			Dim r As Long = x * y
			Dim ax As Long = Math.Abs(x)
			Dim ay As Long = Math.Abs(y)
			If (CInt(CUInt((ax Or ay)) >> 31 <> 0)) Then
				' Some bits greater than 2^31 that might cause overflow
				' Check the result using the divide operator
				' and check for the special case of Long.MIN_VALUE * -1
			   If ((y <> 0) AndAlso (r \ y <> x)) OrElse (x = Long.MinValue AndAlso y = -1) Then Throw New ArithmeticException("long overflow")
			End If
			Return r
		End Function

		''' <summary>
		''' Returns the argument incremented by one, throwing an exception if the
		''' result overflows an {@code int}.
		''' </summary>
		''' <param name="a"> the value to increment </param>
		''' <returns> the result </returns>
		''' <exception cref="ArithmeticException"> if the result overflows an int
		''' @since 1.8 </exception>
		Public Shared Function incrementExact(ByVal a As Integer) As Integer
			If a = Integer.MaxValue Then Throw New ArithmeticException("integer overflow")

			Return a + 1
		End Function

		''' <summary>
		''' Returns the argument incremented by one, throwing an exception if the
		''' result overflows a {@code long}.
		''' </summary>
		''' <param name="a"> the value to increment </param>
		''' <returns> the result </returns>
		''' <exception cref="ArithmeticException"> if the result overflows a long
		''' @since 1.8 </exception>
		Public Shared Function incrementExact(ByVal a As Long) As Long
			If a = Long.MaxValue Then Throw New ArithmeticException("long overflow")

			Return a + 1L
		End Function

		''' <summary>
		''' Returns the argument decremented by one, throwing an exception if the
		''' result overflows an {@code int}.
		''' </summary>
		''' <param name="a"> the value to decrement </param>
		''' <returns> the result </returns>
		''' <exception cref="ArithmeticException"> if the result overflows an int
		''' @since 1.8 </exception>
		Public Shared Function decrementExact(ByVal a As Integer) As Integer
			If a = Integer.MinValue Then Throw New ArithmeticException("integer overflow")

			Return a - 1
		End Function

		''' <summary>
		''' Returns the argument decremented by one, throwing an exception if the
		''' result overflows a {@code long}.
		''' </summary>
		''' <param name="a"> the value to decrement </param>
		''' <returns> the result </returns>
		''' <exception cref="ArithmeticException"> if the result overflows a long
		''' @since 1.8 </exception>
		Public Shared Function decrementExact(ByVal a As Long) As Long
			If a = Long.MinValue Then Throw New ArithmeticException("long overflow")

			Return a - 1L
		End Function

		''' <summary>
		''' Returns the negation of the argument, throwing an exception if the
		''' result overflows an {@code int}.
		''' </summary>
		''' <param name="a"> the value to negate </param>
		''' <returns> the result </returns>
		''' <exception cref="ArithmeticException"> if the result overflows an int
		''' @since 1.8 </exception>
		Public Shared Function negateExact(ByVal a As Integer) As Integer
			If a = Integer.MinValue Then Throw New ArithmeticException("integer overflow")

			Return -a
		End Function

		''' <summary>
		''' Returns the negation of the argument, throwing an exception if the
		''' result overflows a {@code long}.
		''' </summary>
		''' <param name="a"> the value to negate </param>
		''' <returns> the result </returns>
		''' <exception cref="ArithmeticException"> if the result overflows a long
		''' @since 1.8 </exception>
		Public Shared Function negateExact(ByVal a As Long) As Long
			If a = Long.MinValue Then Throw New ArithmeticException("long overflow")

			Return -a
		End Function

		''' <summary>
		''' Returns the value of the {@code long} argument;
		''' throwing an exception if the value overflows an {@code int}.
		''' </summary>
		''' <param name="value"> the long value </param>
		''' <returns> the argument as an int </returns>
		''' <exception cref="ArithmeticException"> if the {@code argument} overflows an int
		''' @since 1.8 </exception>
		Public Shared Function toIntExact(ByVal value As Long) As Integer
			If CInt(value) <> value Then Throw New ArithmeticException("integer overflow")
			Return CInt(value)
		End Function

		''' <summary>
		''' Returns the largest (closest to positive infinity)
		''' {@code int} value that is less than or equal to the algebraic quotient.
		''' There is one special case, if the dividend is the
		''' <seealso cref="Integer#MIN_VALUE Integer.MIN_VALUE"/> and the divisor is {@code -1},
		''' then integer overflow occurs and
		''' the result is equal to the {@code Integer.MIN_VALUE}.
		''' <p>
		''' Normal integer division operates under the round to zero rounding mode
		''' (truncation).  This operation instead acts under the round toward
		''' negative infinity (floor) rounding mode.
		''' The floor rounding mode gives different results than truncation
		''' when the exact result is negative.
		''' <ul>
		'''   <li>If the signs of the arguments are the same, the results of
		'''       {@code floorDiv} and the {@code /} operator are the same.  <br>
		'''       For example, {@code floorDiv(4, 3) == 1} and {@code (4 / 3) == 1}.</li>
		'''   <li>If the signs of the arguments are different,  the quotient is negative and
		'''       {@code floorDiv} returns the integer less than or equal to the quotient
		'''       and the {@code /} operator returns the integer closest to zero.<br>
		'''       For example, {@code floorDiv(-4, 3) == -2},
		'''       whereas {@code (-4 / 3) == -1}.
		'''   </li>
		''' </ul>
		''' <p>
		''' </summary>
		''' <param name="x"> the dividend </param>
		''' <param name="y"> the divisor </param>
		''' <returns> the largest (closest to positive infinity)
		''' {@code int} value that is less than or equal to the algebraic quotient. </returns>
		''' <exception cref="ArithmeticException"> if the divisor {@code y} is zero </exception>
		''' <seealso cref= #floorMod(int, int) </seealso>
		''' <seealso cref= #floor(double)
		''' @since 1.8 </seealso>
		Public Shared Function floorDiv(ByVal x As Integer, ByVal y As Integer) As Integer
			Dim r As Integer = x \ y
			' if the signs are different and modulo not zero, round down
			If (x Xor y) < 0 AndAlso (r * y <> x) Then r -= 1
			Return r
		End Function

		''' <summary>
		''' Returns the largest (closest to positive infinity)
		''' {@code long} value that is less than or equal to the algebraic quotient.
		''' There is one special case, if the dividend is the
		''' <seealso cref="Long#MIN_VALUE Long.MIN_VALUE"/> and the divisor is {@code -1},
		''' then integer overflow occurs and
		''' the result is equal to the {@code Long.MIN_VALUE}.
		''' <p>
		''' Normal integer division operates under the round to zero rounding mode
		''' (truncation).  This operation instead acts under the round toward
		''' negative infinity (floor) rounding mode.
		''' The floor rounding mode gives different results than truncation
		''' when the exact result is negative.
		''' <p>
		''' For examples, see <seealso cref="#floorDiv(int, int)"/>.
		''' </summary>
		''' <param name="x"> the dividend </param>
		''' <param name="y"> the divisor </param>
		''' <returns> the largest (closest to positive infinity)
		''' {@code long} value that is less than or equal to the algebraic quotient. </returns>
		''' <exception cref="ArithmeticException"> if the divisor {@code y} is zero </exception>
		''' <seealso cref= #floorMod(long, long) </seealso>
		''' <seealso cref= #floor(double)
		''' @since 1.8 </seealso>
		Public Shared Function floorDiv(ByVal x As Long, ByVal y As Long) As Long
			Dim r As Long = x \ y
			' if the signs are different and modulo not zero, round down
			If (x Xor y) < 0 AndAlso (r * y <> x) Then r -= 1
			Return r
		End Function

		''' <summary>
		''' Returns the floor modulus of the {@code int} arguments.
		''' <p>
		''' The floor modulus is {@code x - (floorDiv(x, y) * y)},
		''' has the same sign as the divisor {@code y}, and
		''' is in the range of {@code -abs(y) < r < +abs(y)}.
		''' 
		''' <p>
		''' The relationship between {@code floorDiv} and {@code floorMod} is such that:
		''' <ul>
		'''   <li>{@code floorDiv(x, y) * y + floorMod(x, y) == x}
		''' </ul>
		''' <p>
		''' The difference in values between {@code floorMod} and
		''' the {@code %} operator is due to the difference between
		''' {@code floorDiv} that returns the integer less than or equal to the quotient
		''' and the {@code /} operator that returns the integer closest to zero.
		''' <p>
		''' Examples:
		''' <ul>
		'''   <li>If the signs of the arguments are the same, the results
		'''       of {@code floorMod} and the {@code %} operator are the same.  <br>
		'''       <ul>
		'''       <li>{@code floorMod(4, 3) == 1}; &nbsp; and {@code (4 % 3) == 1}</li>
		'''       </ul>
		'''   <li>If the signs of the arguments are different, the results differ from the {@code %} operator.<br>
		'''      <ul>
		'''      <li>{@code floorMod(+4, -3) == -2}; &nbsp; and {@code (+4 % -3) == +1} </li>
		'''      <li>{@code floorMod(-4, +3) == +2}; &nbsp; and {@code (-4 % +3) == -1} </li>
		'''      <li>{@code floorMod(-4, -3) == -1}; &nbsp; and {@code (-4 % -3) == -1 } </li>
		'''      </ul>
		'''   </li>
		''' </ul>
		''' <p>
		''' If the signs of arguments are unknown and a positive modulus
		''' is needed it can be computed as {@code (floorMod(x, y) + abs(y)) % abs(y)}.
		''' </summary>
		''' <param name="x"> the dividend </param>
		''' <param name="y"> the divisor </param>
		''' <returns> the floor modulus {@code x - (floorDiv(x, y) * y)} </returns>
		''' <exception cref="ArithmeticException"> if the divisor {@code y} is zero </exception>
		''' <seealso cref= #floorDiv(int, int)
		''' @since 1.8 </seealso>
		Public Shared Function floorMod(ByVal x As Integer, ByVal y As Integer) As Integer
			Dim r As Integer = x - floorDiv(x, y) * y
			Return r
		End Function

		''' <summary>
		''' Returns the floor modulus of the {@code long} arguments.
		''' <p>
		''' The floor modulus is {@code x - (floorDiv(x, y) * y)},
		''' has the same sign as the divisor {@code y}, and
		''' is in the range of {@code -abs(y) < r < +abs(y)}.
		''' 
		''' <p>
		''' The relationship between {@code floorDiv} and {@code floorMod} is such that:
		''' <ul>
		'''   <li>{@code floorDiv(x, y) * y + floorMod(x, y) == x}
		''' </ul>
		''' <p>
		''' For examples, see <seealso cref="#floorMod(int, int)"/>.
		''' </summary>
		''' <param name="x"> the dividend </param>
		''' <param name="y"> the divisor </param>
		''' <returns> the floor modulus {@code x - (floorDiv(x, y) * y)} </returns>
		''' <exception cref="ArithmeticException"> if the divisor {@code y} is zero </exception>
		''' <seealso cref= #floorDiv(long, long)
		''' @since 1.8 </seealso>
		Public Shared Function floorMod(ByVal x As Long, ByVal y As Long) As Long
			Return x - floorDiv(x, y) * y
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
		''' <param name="a">   the argument whose absolute value is to be determined </param>
		''' <returns>  the absolute value of the argument. </returns>
		Public Shared Function abs(ByVal a As Integer) As Integer
			Return If(a < 0, -a, a)
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
		''' <param name="a">   the argument whose absolute value is to be determined </param>
		''' <returns>  the absolute value of the argument. </returns>
		Public Shared Function abs(ByVal a As Long) As Long
			Return If(a < 0, -a, a)
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
			Return If(a <= 0.0F, 0.0F - a, a)
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
		''' <p>{@code Double.longBitsToDouble((Double.doubleToLongBits(a)<<1)>>>1)}
		''' </summary>
		''' <param name="a">   the argument whose absolute value is to be determined </param>
		''' <returns>  the absolute value of the argument. </returns>
		Public Shared Function abs(ByVal a As Double) As Double
			Return If(a <= 0.0R, 0.0R - a, a)
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
			Return If(a >= b, a, b)
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
			Return If(a >= b, a, b)
		End Function

		' Use raw bit-wise conversions on guaranteed non-NaN arguments.
		Private Shared negativeZeroFloatBits As Long = Float.floatToRawIntBits(-0.0f)
		Private Shared negativeZeroDoubleBits As Long = Double.doubleToRawLongBits(-0.0R)

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
			If a <> a Then Return a ' a is NaN
			If (a = 0.0f) AndAlso (b = 0.0f) AndAlso (Float.floatToRawIntBits(a) = negativeZeroFloatBits) Then Return b
			Return If(a >= b, a, b)
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
			If a <> a Then Return a ' a is NaN
			If (a = 0.0R) AndAlso (b = 0.0R) AndAlso (Double.doubleToRawLongBits(a) = negativeZeroDoubleBits) Then Return b
			Return If(a >= b, a, b)
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
			Return If(a <= b, a, b)
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
			Return If(a <= b, a, b)
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
		''' <returns>  the smaller of {@code a} and {@code b}. </returns>
		Public Shared Function min(ByVal a As Single, ByVal b As Single) As Single
			If a <> a Then Return a ' a is NaN
			If (a = 0.0f) AndAlso (b = 0.0f) AndAlso (Float.floatToRawIntBits(b) = negativeZeroFloatBits) Then Return b
			Return If(a <= b, a, b)
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
			If a <> a Then Return a ' a is NaN
			If (a = 0.0R) AndAlso (b = 0.0R) AndAlso (Double.doubleToRawLongBits(b) = negativeZeroDoubleBits) Then Return b
			Return If(a <= b, a, b)
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
		''' {@code Double.MIN_VALUE}.
		''' <li> If the argument is &plusmn;{@code Double.MAX_VALUE}, then
		''' the result is equal to 2<sup>971</sup>.
		''' </ul>
		''' </summary>
		''' <param name="d"> the floating-point value whose ulp is to be returned </param>
		''' <returns> the size of an ulp of the argument
		''' @author Joseph D. Darcy
		''' @since 1.5 </returns>
		Public Shared Function ulp(ByVal d As Double) As Double
			Dim exp As Integer = getExponent(d)

			Select Case exp
			Case sun.misc.DoubleConsts.MAX_EXPONENT+1 ' NaN or infinity
				Return Math.Abs(d)

			Case sun.misc.DoubleConsts.MIN_EXPONENT-1 ' zero or subnormal
				Return Double.MinValue

			Case Else
				Debug.Assert(exp <= sun.misc.DoubleConsts.MAX_EXPONENT AndAlso exp >= sun.misc.DoubleConsts.MIN_EXPONENT)

				' ulp(x) is usually 2^(SIGNIFICAND_WIDTH-1)*(2^ilogb(x))
				exp = exp - (sun.misc.DoubleConsts.SIGNIFICAND_WIDTH-1)
				If exp >= sun.misc.DoubleConsts.MIN_EXPONENT Then
					Return powerOfTwoD(exp)
				Else
					' return a subnormal result; left shift integer
					' representation of Double.MIN_VALUE appropriate
					' number of positions
					Return Double.longBitsToDouble(1L << (exp - (sun.misc.DoubleConsts.MIN_EXPONENT - (sun.misc.DoubleConsts.SIGNIFICAND_WIDTH-1))))
				End If
			End Select
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
			Dim exp As Integer = getExponent(f)

			Select Case exp
			Case sun.misc.FloatConsts.MAX_EXPONENT+1 ' NaN or infinity
				Return Math.Abs(f)

			Case sun.misc.FloatConsts.MIN_EXPONENT-1 ' zero or subnormal
				Return sun.misc.FloatConsts.MIN_VALUE

			Case Else
				Debug.Assert(exp <= sun.misc.FloatConsts.MAX_EXPONENT AndAlso exp >= sun.misc.FloatConsts.MIN_EXPONENT)

				' ulp(x) is usually 2^(SIGNIFICAND_WIDTH-1)*(2^ilogb(x))
				exp = exp - (sun.misc.FloatConsts.SIGNIFICAND_WIDTH-1)
				If exp >= sun.misc.FloatConsts.MIN_EXPONENT Then
					Return powerOfTwoF(exp)
				Else
					' return a subnormal result; left shift integer
					' representation of FloatConsts.MIN_VALUE appropriate
					' number of positions
					Return Float.intBitsToFloat(1 << (exp - (sun.misc.FloatConsts.MIN_EXPONENT - (sun.misc.FloatConsts.SIGNIFICAND_WIDTH-1))))
				End If
			End Select
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
			Return If(d = 0.0 OrElse Double.IsNaN(d), d, copySign(1.0, d))
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
			Return If(f = 0.0f OrElse Float.IsNaN(f), f, copySign(1.0f, f))
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
		''' 
		''' <p>The computed result must be within 2.5 ulps of the exact result.
		''' </summary>
		''' <param name="x"> The number whose hyperbolic sine is to be returned. </param>
		''' <returns>  The hyperbolic sine of {@code x}.
		''' @since 1.5 </returns>
		Public Shared Function sinh(ByVal x As Double) As Double
			Return Math.Sinh(x)
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
		''' 
		''' <p>The computed result must be within 2.5 ulps of the exact result.
		''' </summary>
		''' <param name="x"> The number whose hyperbolic cosine is to be returned. </param>
		''' <returns>  The hyperbolic cosine of {@code x}.
		''' @since 1.5 </returns>
		Public Shared Function cosh(ByVal x As Double) As Double
			Return Math.Cosh(x)
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
		''' 
		''' <p>The computed result must be within 2.5 ulps of the exact result.
		''' The result of {@code tanh} for any finite input must have
		''' an absolute value less than or equal to 1.  Note that once the
		''' exact result of tanh is within 1/2 of an ulp of the limit value
		''' of &plusmn;1, correctly signed &plusmn;{@code 1.0} should
		''' be returned.
		''' </summary>
		''' <param name="x"> The number whose hyperbolic tangent is to be returned. </param>
		''' <returns>  The hyperbolic tangent of {@code x}.
		''' @since 1.5 </returns>
		Public Shared Function tanh(ByVal x As Double) As Double
			Return Math.Tanh(x)
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
		''' 
		''' <p>The computed result must be within 1 ulp of the exact
		''' result.  If one parameter is held constant, the results must be
		''' semi-monotonic in the other parameter.
		''' </summary>
		''' <param name="x"> a value </param>
		''' <param name="y"> a value </param>
		''' <returns> sqrt(<i>x</i><sup>2</sup>&nbsp;+<i>y</i><sup>2</sup>)
		''' without intermediate overflow or underflow
		''' @since 1.5 </returns>
		Public Shared Function hypot(ByVal x As Double, ByVal y As Double) As Double
			Return Math.hypot(x, y)
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
		''' 
		''' <p>The computed result must be within 1 ulp of the exact result.
		''' Results must be semi-monotonic.  The result of
		''' {@code expm1} for any finite input must be greater than or
		''' equal to {@code -1.0}.  Note that once the exact result of
		''' <i>e</i><sup>{@code x}</sup>&nbsp;-&nbsp;1 is within 1/2
		''' ulp of the limit value -1, {@code -1.0} should be
		''' returned.
		''' </summary>
		''' <param name="x">   the exponent to raise <i>e</i> to in the computation of
		'''              <i>e</i><sup>{@code x}</sup>&nbsp;-1. </param>
		''' <returns>  the value <i>e</i><sup>{@code x}</sup>&nbsp;-&nbsp;1.
		''' @since 1.5 </returns>
		Public Shared Function expm1(ByVal x As Double) As Double
			Return Math.expm1(x)
		End Function

		''' <summary>
		''' Returns the natural logarithm of the sum of the argument and 1.
		''' Note that for small values {@code x}, the result of
		''' {@code log1p(x)} is much closer to the true result of ln(1
		''' + {@code x}) than the floating-point evaluation of
		''' {@code log(1.0+x)}.
		''' 
		''' <p>Special cases:
		''' 
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
		''' 
		''' <p>The computed result must be within 1 ulp of the exact result.
		''' Results must be semi-monotonic.
		''' </summary>
		''' <param name="x">   a value </param>
		''' <returns> the value ln({@code x}&nbsp;+&nbsp;1), the natural
		''' log of {@code x}&nbsp;+&nbsp;1
		''' @since 1.5 </returns>
		Public Shared Function log1p(ByVal x As Double) As Double
			Return Math.log1p(x)
		End Function

		''' <summary>
		''' Returns the first floating-point argument with the sign of the
		''' second floating-point argument.  Note that unlike the {@link
		''' StrictMath#copySign(double, double) StrictMath.copySign}
		''' method, this method does not require NaN {@code sign}
		''' arguments to be treated as positive values; implementations are
		''' permitted to treat some NaN arguments as positive and other NaN
		''' arguments as negative to allow greater performance.
		''' </summary>
		''' <param name="magnitude">  the parameter providing the magnitude of the result </param>
		''' <param name="sign">   the parameter providing the sign of the result </param>
		''' <returns> a value with the magnitude of {@code magnitude}
		''' and the sign of {@code sign}.
		''' @since 1.6 </returns>
		Public Shared Function copySign(ByVal magnitude As Double, ByVal sign As Double) As Double
			Return Double.longBitsToDouble((Double.doubleToRawLongBits(sign) And (sun.misc.DoubleConsts.SIGN_BIT_MASK)) Or (Double.doubleToRawLongBits(magnitude) And (sun.misc.DoubleConsts.EXP_BIT_MASK Or sun.misc.DoubleConsts.SIGNIF_BIT_MASK)))
		End Function

		''' <summary>
		''' Returns the first floating-point argument with the sign of the
		''' second floating-point argument.  Note that unlike the {@link
		''' StrictMath#copySign(float, float) StrictMath.copySign}
		''' method, this method does not require NaN {@code sign}
		''' arguments to be treated as positive values; implementations are
		''' permitted to treat some NaN arguments as positive and other NaN
		''' arguments as negative to allow greater performance.
		''' </summary>
		''' <param name="magnitude">  the parameter providing the magnitude of the result </param>
		''' <param name="sign">   the parameter providing the sign of the result </param>
		''' <returns> a value with the magnitude of {@code magnitude}
		''' and the sign of {@code sign}.
		''' @since 1.6 </returns>
		Public Shared Function copySign(ByVal magnitude As Single, ByVal sign As Single) As Single
			Return Float.intBitsToFloat((Float.floatToRawIntBits(sign) And (sun.misc.FloatConsts.SIGN_BIT_MASK)) Or (Float.floatToRawIntBits(magnitude) And (sun.misc.FloatConsts.EXP_BIT_MASK Or sun.misc.FloatConsts.SIGNIF_BIT_MASK)))
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
	'        
	'         * Bitwise convert f to integer, mask out exponent bits, shift
	'         * to the right and then subtract out float's bias adjust to
	'         * get true exponent value
	'         
			Return ((Float.floatToRawIntBits(f) And sun.misc.FloatConsts.EXP_BIT_MASK) >> (sun.misc.FloatConsts.SIGNIFICAND_WIDTH - 1)) - sun.misc.FloatConsts.EXP_BIAS
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
	'        
	'         * Bitwise convert d to long, mask out exponent bits, shift
	'         * to the right and then subtract out double's bias adjust to
	'         * get true exponent value.
	'         
			Return CInt(Fix(((Double.doubleToRawLongBits(d) And sun.misc.DoubleConsts.EXP_BIT_MASK) >> (sun.misc.DoubleConsts.SIGNIFICAND_WIDTH - 1)) - sun.misc.DoubleConsts.EXP_BIAS))
		End Function

		''' <summary>
		''' Returns the floating-point number adjacent to the first
		''' argument in the direction of the second argument.  If both
		''' arguments compare as equal the second argument is returned.
		''' 
		''' <p>
		''' Special cases:
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
	'        
	'         * The cases:
	'         *
	'         * nextAfter(+infinity, 0)  == MAX_VALUE
	'         * nextAfter(+infinity, +infinity)  == +infinity
	'         * nextAfter(-infinity, 0)  == -MAX_VALUE
	'         * nextAfter(-infinity, -infinity)  == -infinity
	'         *
	'         * are naturally handled without any additional testing
	'         

			' First check for NaN values
			If Double.IsNaN(start) OrElse Double.IsNaN(direction) Then
				' return a NaN derived from the input NaN(s)
				Return start + direction
			ElseIf start = direction Then
				Return direction ' start > direction or start < direction
			Else
				' Add +0.0 to get rid of a -0.0 (+0.0 + -0.0 => +0.0)
				' then bitwise convert start to integer.
				Dim transducer As Long = Double.doubleToRawLongBits(start + 0.0R)

	'            
	'             * IEEE 754 floating-point numbers are lexicographically
	'             * ordered if treated as signed- magnitude integers .
	'             * Since Java's integers are two's complement,
	'             * incrementing" the two's complement representation of a
	'             * logically negative floating-point value *decrements*
	'             * the signed-magnitude representation. Therefore, when
	'             * the integer representation of a floating-point values
	'             * is less than zero, the adjustment to the representation
	'             * is in the opposite direction than would be expected at
	'             * first .
	'             
				If direction > start Then ' Calculate next greater value
					transducer = transducer + (If(transducer >= 0L, 1L, -1L)) ' Calculate next lesser value
				Else
					Debug.Assert(direction < start)
					If transducer > 0L Then
						transducer -= 1
					Else
						If transducer < 0L Then
							transducer += 1
	'                    
	'                     * transducer==0, the result is -MIN_VALUE
	'                     *
	'                     * The transition from zero (implicitly
	'                     * positive) to the smallest negative
	'                     * signed magnitude value must be done
	'                     * explicitly.
	'                     
						Else
							transducer = sun.misc.DoubleConsts.SIGN_BIT_MASK Or 1L
						End If
					End If
				End If

				Return Double.longBitsToDouble(transducer)
			End If
		End Function

		''' <summary>
		''' Returns the floating-point number adjacent to the first
		''' argument in the direction of the second argument.  If both
		''' arguments compare as equal a value equivalent to the second argument
		''' is returned.
		''' 
		''' <p>
		''' Special cases:
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
	'        
	'         * The cases:
	'         *
	'         * nextAfter(+infinity, 0)  == MAX_VALUE
	'         * nextAfter(+infinity, +infinity)  == +infinity
	'         * nextAfter(-infinity, 0)  == -MAX_VALUE
	'         * nextAfter(-infinity, -infinity)  == -infinity
	'         *
	'         * are naturally handled without any additional testing
	'         

			' First check for NaN values
			If Float.IsNaN(start) OrElse Double.IsNaN(direction) Then
				' return a NaN derived from the input NaN(s)
				Return start + CSng(direction)
			ElseIf start = direction Then
				Return CSng(direction) ' start > direction or start < direction
			Else
				' Add +0.0 to get rid of a -0.0 (+0.0 + -0.0 => +0.0)
				' then bitwise convert start to integer.
				Dim transducer As Integer = Float.floatToRawIntBits(start + 0.0f)

	'            
	'             * IEEE 754 floating-point numbers are lexicographically
	'             * ordered if treated as signed- magnitude integers .
	'             * Since Java's integers are two's complement,
	'             * incrementing" the two's complement representation of a
	'             * logically negative floating-point value *decrements*
	'             * the signed-magnitude representation. Therefore, when
	'             * the integer representation of a floating-point values
	'             * is less than zero, the adjustment to the representation
	'             * is in the opposite direction than would be expected at
	'             * first.
	'             
				If direction > start Then ' Calculate next greater value
					transducer = transducer + (If(transducer >= 0, 1, -1)) ' Calculate next lesser value
				Else
					Debug.Assert(direction < start)
					If transducer > 0 Then
						transducer -= 1
					Else
						If transducer < 0 Then
							transducer += 1
	'                    
	'                     * transducer==0, the result is -MIN_VALUE
	'                     *
	'                     * The transition from zero (implicitly
	'                     * positive) to the smallest negative
	'                     * signed magnitude value must be done
	'                     * explicitly.
	'                     
						Else
							transducer = sun.misc.FloatConsts.SIGN_BIT_MASK Or 1
						End If
					End If
				End If

				Return Float.intBitsToFloat(transducer)
			End If
		End Function

		''' <summary>
		''' Returns the floating-point value adjacent to {@code d} in
		''' the direction of positive infinity.  This method is
		''' semantically equivalent to {@code nextAfter(d,
		''' Double.POSITIVE_INFINITY)}; however, a {@code nextUp}
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
			If Double.IsNaN(d) OrElse d = Double.PositiveInfinity Then
				Return d
			Else
				d += 0.0R
				Return Double.longBitsToDouble(Double.doubleToRawLongBits(d) + (If(d >= 0.0R, +1L, -1L)))
			End If
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
			If Float.IsNaN(f) OrElse f = sun.misc.FloatConsts.POSITIVE_INFINITY Then
				Return f
			Else
				f += 0.0f
				Return Float.intBitsToFloat(Float.floatToRawIntBits(f) + (If(f >= 0.0f, +1, -1)))
			End If
		End Function

		''' <summary>
		''' Returns the floating-point value adjacent to {@code d} in
		''' the direction of negative infinity.  This method is
		''' semantically equivalent to {@code nextAfter(d,
		''' Double.NEGATIVE_INFINITY)}; however, a
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
			If Double.IsNaN(d) OrElse d = Double.NegativeInfinity Then
				Return d
			Else
				If d = 0.0 Then
					Return -Double.MinValue
				Else
					Return Double.longBitsToDouble(Double.doubleToRawLongBits(d) + (If(d > 0.0R, -1L, +1L)))
				End If
			End If
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
			If Float.IsNaN(f) OrElse f = Float.NegativeInfinity Then
				Return f
			Else
				If f = 0.0f Then
					Return -Float.MinValue
				Else
					Return Float.intBitsToFloat(Float.floatToRawIntBits(f) + (If(f > 0.0f, -1, +1)))
				End If
			End If
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
		''' would be larger than {@code Double.MAX_EXPONENT}, an
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
	'        
	'         * This method does not need to be declared strictfp to
	'         * compute the same correct result on all platforms.  When
	'         * scaling up, it does not matter what order the
	'         * multiply-store operations are done; the result will be
	'         * finite or overflow regardless of the operation ordering.
	'         * However, to get the correct result when scaling down, a
	'         * particular ordering must be used.
	'         *
	'         * When scaling down, the multiply-store operations are
	'         * sequenced so that it is not possible for two consecutive
	'         * multiply-stores to return subnormal results.  If one
	'         * multiply-store result is subnormal, the next multiply will
	'         * round it away to zero.  This is done by first multiplying
	'         * by 2 ^ (scaleFactor % n) and then multiplying several
	'         * times by by 2^n as needed where n is the exponent of number
	'         * that is a covenient power of two.  In this way, at most one
	'         * real rounding error occurs.  If the double value set is
	'         * being used exclusively, the rounding will occur on a
	'         * multiply.  If the double-extended-exponent value set is
	'         * being used, the products will (perhaps) be exact but the
	'         * stores to d are guaranteed to round to the double value
	'         * set.
	'         *
	'         * It is _not_ a valid implementation to first multiply d by
	'         * 2^MIN_EXPONENT and then by 2 ^ (scaleFactor %
	'         * MIN_EXPONENT) since even in a strictfp program double
	'         * rounding on underflow could occur; e.g. if the scaleFactor
	'         * argument was (MIN_EXPONENT - n) and the exponent of d was a
	'         * little less than -(MIN_EXPONENT - n), meaning the final
	'         * result would be subnormal.
	'         *
	'         * Since exact reproducibility of this method can be achieved
	'         * without any undue performance burden, there is no
	'         * compelling reason to allow double rounding on underflow in
	'         * scalb.
	'         

			' magnitude of a power of two so large that scaling a finite
			' nonzero value by it would be guaranteed to over or
			' underflow; due to rounding, scaling down takes takes an
			' additional power of two which is reflected here
			Dim MAX_SCALE As Integer = sun.misc.DoubleConsts.MAX_EXPONENT + -sun.misc.DoubleConsts.MIN_EXPONENT + sun.misc.DoubleConsts.SIGNIFICAND_WIDTH + 1
			Dim exp_adjust As Integer = 0
			Dim scale_increment As Integer = 0
			Dim exp_delta As Double = Double.NaN

			' Make sure scaling factor is in a reasonable range

			If scaleFactor < 0 Then
				scaleFactor = Math.Max(scaleFactor, -MAX_SCALE)
				scale_increment = -512
				exp_delta = twoToTheDoubleScaleDown
			Else
				scaleFactor = Math.Min(scaleFactor, MAX_SCALE)
				scale_increment = 512
				exp_delta = twoToTheDoubleScaleUp
			End If

			' Calculate (scaleFactor % +/-512), 512 = 2^9, using
			' technique from "Hacker's Delight" section 10-2.
			Dim t As Integer = CInt(CUInt((scaleFactor >> 9-1)) >> 32 - 9)
			exp_adjust = ((scaleFactor + t) And (512 -1)) - t

			d *= powerOfTwoD(exp_adjust)
			scaleFactor -= exp_adjust

			Do While scaleFactor <> 0
				d *= exp_delta
				scaleFactor -= scale_increment
			Loop
			Return d
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
			' magnitude of a power of two so large that scaling a finite
			' nonzero value by it would be guaranteed to over or
			' underflow; due to rounding, scaling down takes takes an
			' additional power of two which is reflected here
			Dim MAX_SCALE As Integer = sun.misc.FloatConsts.MAX_EXPONENT + -sun.misc.FloatConsts.MIN_EXPONENT + sun.misc.FloatConsts.SIGNIFICAND_WIDTH + 1

			' Make sure scaling factor is in a reasonable range
			scaleFactor = Math.Max(Math.Min(scaleFactor, MAX_SCALE), -MAX_SCALE)

	'        
	'         * Since + MAX_SCALE for float fits well within the double
	'         * exponent range and + float -> double conversion is exact
	'         * the multiplication below will be exact. Therefore, the
	'         * rounding that occurs when the double product is cast to
	'         * float will be the correctly rounded float result.  Since
	'         * all operations other than the final multiply will be exact,
	'         * it is not necessary to declare this method strictfp.
	'         
			Return CSng(CDbl(f)*powerOfTwoD(scaleFactor))
		End Function

		' Constants used in scalb
		Friend Shared twoToTheDoubleScaleUp As Double = powerOfTwoD(512)
		Friend Shared twoToTheDoubleScaleDown As Double = powerOfTwoD(-512)

		''' <summary>
		''' Returns a floating-point power of two in the normal range.
		''' </summary>
		Friend Shared Function powerOfTwoD(ByVal n As Integer) As Double
			assert(n >= sun.misc.DoubleConsts.MIN_EXPONENT AndAlso n <= sun.misc.DoubleConsts.MAX_EXPONENT)
			Return Double.longBitsToDouble(((CLng(n) + CLng(Fix(sun.misc.DoubleConsts.EXP_BIAS))) << (sun.misc.DoubleConsts.SIGNIFICAND_WIDTH-1)) And sun.misc.DoubleConsts.EXP_BIT_MASK)
		End Function

		''' <summary>
		''' Returns a floating-point power of two in the normal range.
		''' </summary>
		Friend Shared Function powerOfTwoF(ByVal n As Integer) As Single
			assert(n >= sun.misc.FloatConsts.MIN_EXPONENT AndAlso n <= sun.misc.FloatConsts.MAX_EXPONENT)
			Return Float.intBitsToFloat(((n + sun.misc.FloatConsts.EXP_BIAS) << (sun.misc.FloatConsts.SIGNIFICAND_WIDTH-1)) And sun.misc.FloatConsts.EXP_BIT_MASK)
		End Function
	End Class

End Namespace
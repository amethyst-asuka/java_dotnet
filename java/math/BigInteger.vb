Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics

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
' * Portions Copyright (c) 1995  Colin Plumb.  All rights reserved.
' 

Namespace java.math


	''' <summary>
	''' Immutable arbitrary-precision integers.  All operations behave as if
	''' BigIntegers were represented in two's-complement notation (like Java's
	''' primitive integer types).  BigInteger provides analogues to all of Java's
	''' primitive integer operators, and all relevant methods from java.lang.Math.
	''' Additionally, BigInteger provides operations for modular arithmetic, GCD
	''' calculation, primality testing, prime generation, bit manipulation,
	''' and a few other miscellaneous operations.
	''' 
	''' <p>Semantics of arithmetic operations exactly mimic those of Java's integer
	''' arithmetic operators, as defined in <i>The Java Language Specification</i>.
	''' For example, division by zero throws an {@code ArithmeticException}, and
	''' division of a negative by a positive yields a negative (or zero) remainder.
	''' All of the details in the Spec concerning overflow are ignored, as
	''' BigIntegers are made as large as necessary to accommodate the results of an
	''' operation.
	''' 
	''' <p>Semantics of shift operations extend those of Java's shift operators
	''' to allow for negative shift distances.  A right-shift with a negative
	''' shift distance results in a left shift, and vice-versa.  The unsigned
	''' right shift operator ({@code >>>}) is omitted, as this operation makes
	''' little sense in combination with the "infinite word size" abstraction
	''' provided by this class.
	''' 
	''' <p>Semantics of bitwise logical operations exactly mimic those of Java's
	''' bitwise integer operators.  The binary operators ({@code and},
	''' {@code or}, {@code xor}) implicitly perform sign extension on the shorter
	''' of the two operands prior to performing the operation.
	''' 
	''' <p>Comparison operations perform signed integer comparisons, analogous to
	''' those performed by Java's relational and equality operators.
	''' 
	''' <p>Modular arithmetic operations are provided to compute residues, perform
	''' exponentiation, and compute multiplicative inverses.  These methods always
	''' return a non-negative result, between {@code 0} and {@code (modulus - 1)},
	''' inclusive.
	''' 
	''' <p>Bit operations operate on a single bit of the two's-complement
	''' representation of their operand.  If necessary, the operand is sign-
	''' extended so that it contains the designated bit.  None of the single-bit
	''' operations can produce a BigInteger with a different sign from the
	''' BigInteger being operated on, as they affect only a single bit, and the
	''' "infinite word size" abstraction provided by this class ensures that there
	''' are infinitely many "virtual sign bits" preceding each BigInteger.
	''' 
	''' <p>For the sake of brevity and clarity, pseudo-code is used throughout the
	''' descriptions of BigInteger methods.  The pseudo-code expression
	''' {@code (i + j)} is shorthand for "a BigInteger whose value is
	''' that of the BigInteger {@code i} plus that of the BigInteger {@code j}."
	''' The pseudo-code expression {@code (i == j)} is shorthand for
	''' "{@code true} if and only if the BigInteger {@code i} represents the same
	''' value as the BigInteger {@code j}."  Other pseudo-code expressions are
	''' interpreted similarly.
	''' 
	''' <p>All methods and constructors in this class throw
	''' {@code NullPointerException} when passed
	''' a null object reference for any input parameter.
	''' 
	''' BigInteger must support values in the range
	''' -2<sup>{@code Integer.MAX_VALUE}</sup> (exclusive) to
	''' +2<sup>{@code Integer.MAX_VALUE}</sup> (exclusive)
	''' and may support values outside of that range.
	''' 
	''' The range of probable prime values is limited and may be less than
	''' the full supported positive range of {@code BigInteger}.
	''' The range must be at least 1 to 2<sup>500000000</sup>.
	''' 
	''' @implNote
	''' BigInteger constructors and operations throw {@code ArithmeticException} when
	''' the result is out of the supported range of
	''' -2<sup>{@code Integer.MAX_VALUE}</sup> (exclusive) to
	''' +2<sup>{@code Integer.MAX_VALUE}</sup> (exclusive).
	''' </summary>
	''' <seealso cref=     BigDecimal
	''' @author  Josh Bloch
	''' @author  Michael McCloskey
	''' @author  Alan Eliasen
	''' @author  Timothy Buktu
	''' @since JDK1.1 </seealso>

	Public Class BigInteger
		Inherits Number
		Implements Comparable(Of BigInteger)

		''' <summary>
		''' The signum of this BigInteger: -1 for negative, 0 for zero, or
		''' 1 for positive.  Note that the BigInteger zero <i>must</i> have
		''' a signum of 0.  This is necessary to ensures that there is exactly one
		''' representation for each BigInteger value.
		''' 
		''' @serial
		''' </summary>
		Friend ReadOnly signum_Renamed As Integer

		''' <summary>
		''' The magnitude of this BigInteger, in <i>big-endian</i> order: the
		''' zeroth element of this array is the most-significant int of the
		''' magnitude.  The magnitude must be "minimal" in that the most-significant
		''' int ({@code mag[0]}) must be non-zero.  This is necessary to
		''' ensure that there is exactly one representation for each BigInteger
		''' value.  Note that this implies that the BigInteger zero has a
		''' zero-length mag array.
		''' </summary>
		Friend ReadOnly mag As Integer()

		' These "redundant fields" are initialized with recognizable nonsense
		' values, and cached the first time they are needed (or never, if they
		' aren't needed).

		 ''' <summary>
		 ''' One plus the bitCount of this BigInteger. Zeros means unitialized.
		 ''' 
		 ''' @serial </summary>
		 ''' <seealso cref= #bitCount </seealso>
		 ''' @deprecated Deprecated since logical value is offset from stored
		 ''' value and correction factor is applied in accessor method. 
		<Obsolete("Deprecated since logical value is offset from stored")> _
		Private bitCount_Renamed As Integer

		''' <summary>
		''' One plus the bitLength of this BigInteger. Zeros means unitialized.
		''' (either value is acceptable).
		''' 
		''' @serial </summary>
		''' <seealso cref= #bitLength() </seealso>
		''' @deprecated Deprecated since logical value is offset from stored
		''' value and correction factor is applied in accessor method. 
		<Obsolete("Deprecated since logical value is offset from stored")> _
		Private bitLength_Renamed As Integer

		''' <summary>
		''' Two plus the lowest set bit of this BigInteger, as returned by
		''' getLowestSetBit().
		''' 
		''' @serial </summary>
		''' <seealso cref= #getLowestSetBit </seealso>
		''' @deprecated Deprecated since logical value is offset from stored
		''' value and correction factor is applied in accessor method. 
		<Obsolete("Deprecated since logical value is offset from stored")> _
		Private lowestSetBit As Integer

		''' <summary>
		''' Two plus the index of the lowest-order int in the magnitude of this
		''' BigInteger that contains a nonzero int, or -2 (either value is acceptable).
		''' The least significant int has int-number 0, the next int in order of
		''' increasing significance has int-number 1, and so forth. </summary>
		''' @deprecated Deprecated since logical value is offset from stored
		''' value and correction factor is applied in accessor method. 
		<Obsolete("Deprecated since logical value is offset from stored")> _
		Private firstNonzeroIntNum_Renamed As Integer

		''' <summary>
		''' This mask is used to obtain the value of an int as if it were unsigned.
		''' </summary>
		Friend Const LONG_MASK As Long = &HffffffffL

		''' <summary>
		''' This constant limits {@code mag.length} of BigIntegers to the supported
		''' range.
		''' </summary>
		Private Shared ReadOnly MAX_MAG_LENGTH As Integer = Integer.MAX_VALUE / Integer.SIZE + 1 ' (1 << 26)

		''' <summary>
		''' Bit lengths larger than this constant can cause overflow in searchLen
		''' calculation and in BitSieve.singleSearch method.
		''' </summary>
		Private Const PRIME_SEARCH_BIT_LENGTH_LIMIT As Integer = 500000000

		''' <summary>
		''' The threshold value for using Karatsuba multiplication.  If the number
		''' of ints in both mag arrays are greater than this number, then
		''' Karatsuba multiplication will be used.   This value is found
		''' experimentally to work well.
		''' </summary>
		Private Const KARATSUBA_THRESHOLD As Integer = 80

		''' <summary>
		''' The threshold value for using 3-way Toom-Cook multiplication.
		''' If the number of ints in each mag array is greater than the
		''' Karatsuba threshold, and the number of ints in at least one of
		''' the mag arrays is greater than this threshold, then Toom-Cook
		''' multiplication will be used.
		''' </summary>
		Private Const TOOM_COOK_THRESHOLD As Integer = 240

		''' <summary>
		''' The threshold value for using Karatsuba squaring.  If the number
		''' of ints in the number are larger than this value,
		''' Karatsuba squaring will be used.   This value is found
		''' experimentally to work well.
		''' </summary>
		Private Const KARATSUBA_SQUARE_THRESHOLD As Integer = 128

		''' <summary>
		''' The threshold value for using Toom-Cook squaring.  If the number
		''' of ints in the number are larger than this value,
		''' Toom-Cook squaring will be used.   This value is found
		''' experimentally to work well.
		''' </summary>
		Private Const TOOM_COOK_SQUARE_THRESHOLD As Integer = 216

		''' <summary>
		''' The threshold value for using Burnikel-Ziegler division.  If the number
		''' of ints in the divisor are larger than this value, Burnikel-Ziegler
		''' division may be used.  This value is found experimentally to work well.
		''' </summary>
		Friend Const BURNIKEL_ZIEGLER_THRESHOLD As Integer = 80

		''' <summary>
		''' The offset value for using Burnikel-Ziegler division.  If the number
		''' of ints in the divisor exceeds the Burnikel-Ziegler threshold, and the
		''' number of ints in the dividend is greater than the number of ints in the
		''' divisor plus this value, Burnikel-Ziegler division will be used.  This
		''' value is found experimentally to work well.
		''' </summary>
		Friend Const BURNIKEL_ZIEGLER_OFFSET As Integer = 40

		''' <summary>
		''' The threshold value for using Schoenhage recursive base conversion. If
		''' the number of ints in the number are larger than this value,
		''' the Schoenhage algorithm will be used.  In practice, it appears that the
		''' Schoenhage routine is faster for any threshold down to 2, and is
		''' relatively flat for thresholds between 2-25, so this choice may be
		''' varied within this range for very small effect.
		''' </summary>
		Private Const SCHOENHAGE_BASE_CONVERSION_THRESHOLD As Integer = 20

		''' <summary>
		''' The threshold value for using squaring code to perform multiplication
		''' of a {@code BigInteger} instance by itself.  If the number of ints in
		''' the number are larger than this value, {@code multiply(this)} will
		''' return {@code square()}.
		''' </summary>
		Private Const MULTIPLY_SQUARE_THRESHOLD As Integer = 20

		' Constructors

		''' <summary>
		''' Translates a byte array containing the two's-complement binary
		''' representation of a BigInteger into a BigInteger.  The input array is
		''' assumed to be in <i>big-endian</i> byte-order: the most significant
		''' byte is in the zeroth element.
		''' </summary>
		''' <param name="val"> big-endian two's-complement binary representation of
		'''         BigInteger. </param>
		''' <exception cref="NumberFormatException"> {@code val} is zero bytes long. </exception>
		Public Sub New(ByVal val As SByte())
			If val.Length = 0 Then Throw New NumberFormatException("Zero length BigInteger")

			If val(0) < 0 Then
				mag = makePositive(val)
				signum_Renamed = -1
			Else
				mag = stripLeadingZeroBytes(val)
				signum_Renamed = (If(mag.Length = 0, 0, 1))
			End If
			If mag.Length >= MAX_MAG_LENGTH Then checkRange()
		End Sub

		''' <summary>
		''' This private constructor translates an int array containing the
		''' two's-complement binary representation of a BigInteger into a
		''' BigInteger. The input array is assumed to be in <i>big-endian</i>
		''' int-order: the most significant int is in the zeroth element.
		''' </summary>
		Private Sub New(ByVal val As Integer())
			If val.Length = 0 Then Throw New NumberFormatException("Zero length BigInteger")

			If val(0) < 0 Then
				mag = makePositive(val)
				signum_Renamed = -1
			Else
				mag = trustedStripLeadingZeroInts(val)
				signum_Renamed = (If(mag.Length = 0, 0, 1))
			End If
			If mag.Length >= MAX_MAG_LENGTH Then checkRange()
		End Sub

		''' <summary>
		''' Translates the sign-magnitude representation of a BigInteger into a
		''' BigInteger.  The sign is represented as an integer signum value: -1 for
		''' negative, 0 for zero, or 1 for positive.  The magnitude is a byte array
		''' in <i>big-endian</i> byte-order: the most significant byte is in the
		''' zeroth element.  A zero-length magnitude array is permissible, and will
		''' result in a BigInteger value of 0, whether signum is -1, 0 or 1.
		''' </summary>
		''' <param name="signum"> signum of the number (-1 for negative, 0 for zero, 1
		'''         for positive). </param>
		''' <param name="magnitude"> big-endian binary representation of the magnitude of
		'''         the number. </param>
		''' <exception cref="NumberFormatException"> {@code signum} is not one of the three
		'''         legal values (-1, 0, and 1), or {@code signum} is 0 and
		'''         {@code magnitude} contains one or more non-zero bytes. </exception>
		Public Sub New(ByVal signum As Integer, ByVal magnitude As SByte())
			Me.mag = stripLeadingZeroBytes(magnitude)

			If signum < -1 OrElse signum > 1 Then Throw (New NumberFormatException("Invalid signum value"))

			If Me.mag.Length = 0 Then
				Me.signum_Renamed = 0
			Else
				If signum = 0 Then Throw (New NumberFormatException("signum-magnitude mismatch"))
				Me.signum_Renamed = signum
			End If
			If mag.Length >= MAX_MAG_LENGTH Then checkRange()
		End Sub

		''' <summary>
		''' A constructor for internal use that translates the sign-magnitude
		''' representation of a BigInteger into a BigInteger. It checks the
		''' arguments and copies the magnitude so this constructor would be
		''' safe for external use.
		''' </summary>
		Private Sub New(ByVal signum As Integer, ByVal magnitude As Integer())
			Me.mag = stripLeadingZeroInts(magnitude)

			If signum < -1 OrElse signum > 1 Then Throw (New NumberFormatException("Invalid signum value"))

			If Me.mag.Length = 0 Then
				Me.signum_Renamed = 0
			Else
				If signum = 0 Then Throw (New NumberFormatException("signum-magnitude mismatch"))
				Me.signum_Renamed = signum
			End If
			If mag.Length >= MAX_MAG_LENGTH Then checkRange()
		End Sub

		''' <summary>
		''' Translates the String representation of a BigInteger in the
		''' specified radix into a BigInteger.  The String representation
		''' consists of an optional minus or plus sign followed by a
		''' sequence of one or more digits in the specified radix.  The
		''' character-to-digit mapping is provided by {@code
		''' Character.digit}.  The String may not contain any extraneous
		''' characters (whitespace, for example).
		''' </summary>
		''' <param name="val"> String representation of BigInteger. </param>
		''' <param name="radix"> radix to be used in interpreting {@code val}. </param>
		''' <exception cref="NumberFormatException"> {@code val} is not a valid representation
		'''         of a BigInteger in the specified radix, or {@code radix} is
		'''         outside the range from <seealso cref="Character#MIN_RADIX"/> to
		'''         <seealso cref="Character#MAX_RADIX"/>, inclusive. </exception>
		''' <seealso cref=    Character#digit </seealso>
		Public Sub New(ByVal val As String, ByVal radix As Integer)
			Dim cursor As Integer = 0, numDigits As Integer
			Dim len As Integer = val.length()

			If radix < Character.MIN_RADIX OrElse radix > Character.MAX_RADIX Then Throw New NumberFormatException("Radix out of range")
			If len = 0 Then Throw New NumberFormatException("Zero length BigInteger")

			' Check for at most one leading sign
			Dim sign As Integer = 1
			Dim index1 As Integer = val.LastIndexOf("-"c)
			Dim index2 As Integer = val.LastIndexOf("+"c)
			If index1 >= 0 Then
				If index1 <> 0 OrElse index2 >= 0 Then Throw New NumberFormatException("Illegal embedded sign character")
				sign = -1
				cursor = 1
			ElseIf index2 >= 0 Then
				If index2 <> 0 Then Throw New NumberFormatException("Illegal embedded sign character")
				cursor = 1
			End If
			If cursor = len Then Throw New NumberFormatException("Zero length BigInteger")

			' Skip leading zeros and compute number of digits in magnitude
			Do While cursor < len AndAlso Character.digit(val.Chars(cursor), radix) = 0
				cursor += 1
			Loop

			If cursor = len Then
				signum_Renamed = 0
				mag = ZERO.mag
				Return
			End If

			numDigits = len - cursor
			signum_Renamed = sign

			' Pre-allocate array of expected size. May be too large but can
			' never be too small. Typically exact.
			Dim numBits As Long = (CInt(CUInt((numDigits * bitsPerDigit(radix))) >> 10)) + 1
			If numBits + 31 >= (1L << 32) Then reportOverflow()
			Dim numWords As Integer = CInt(CUInt(CInt(numBits + 31)) >> 5)
			Dim magnitude As Integer() = New Integer(numWords - 1){}

			' Process first (potentially short) digit group
			Dim firstGroupLen As Integer = numDigits Mod digitsPerInt(radix)
			If firstGroupLen = 0 Then firstGroupLen = digitsPerInt(radix)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Dim group As String = val.Substring(cursor, cursor += firstGroupLen - cursor)
			magnitude(numWords - 1) = Convert.ToInt32(group, radix)
			If magnitude(numWords - 1) < 0 Then Throw New NumberFormatException("Illegal digit")

			' Process remaining digit groups
			Dim superRadix As Integer = intRadix(radix)
			Dim groupVal As Integer = 0
			Do While cursor < len
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				group = val.Substring(cursor, cursor += digitsPerInt(radix) - cursor)
				groupVal = Convert.ToInt32(group, radix)
				If groupVal < 0 Then Throw New NumberFormatException("Illegal digit")
				destructiveMulAdd(magnitude, superRadix, groupVal)
			Loop
			' Required for cases where the array was overallocated.
			mag = trustedStripLeadingZeroInts(magnitude)
			If mag.Length >= MAX_MAG_LENGTH Then checkRange()
		End Sub

	'    
	'     * Constructs a new BigInteger using a char array with radix=10.
	'     * Sign is precalculated outside and not allowed in the val.
	'     
		Friend Sub New(ByVal val As Char(), ByVal sign As Integer, ByVal len As Integer)
			Dim cursor As Integer = 0, numDigits As Integer

			' Skip leading zeros and compute number of digits in magnitude
			Do While cursor < len AndAlso Character.digit(val(cursor), 10) = 0
				cursor += 1
			Loop
			If cursor = len Then
				signum_Renamed = 0
				mag = ZERO.mag
				Return
			End If

			numDigits = len - cursor
			signum_Renamed = sign
			' Pre-allocate array of expected size
			Dim numWords As Integer
			If len < 10 Then
				numWords = 1
			Else
				Dim numBits As Long = (CInt(CUInt((numDigits * bitsPerDigit(10))) >> 10)) + 1
				If numBits + 31 >= (1L << 32) Then reportOverflow()
				numWords = CInt(CUInt(CInt(numBits + 31)) >> 5)
			End If
			Dim magnitude As Integer() = New Integer(numWords - 1){}

			' Process first (potentially short) digit group
			Dim firstGroupLen As Integer = numDigits Mod digitsPerInt(10)
			If firstGroupLen = 0 Then firstGroupLen = digitsPerInt(10)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			magnitude(numWords - 1) = parseInt(val, cursor, cursor += firstGroupLen)

			' Process remaining digit groups
			Do While cursor < len
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Dim groupVal As Integer = parseInt(val, cursor, cursor += digitsPerInt(10))
				destructiveMulAdd(magnitude, intRadix(10), groupVal)
			Loop
			mag = trustedStripLeadingZeroInts(magnitude)
			If mag.Length >= MAX_MAG_LENGTH Then checkRange()
		End Sub

		' Create an integer with the digits between the two indexes
		' Assumes start < end. The result may be negative, but it
		' is to be treated as an unsigned value.
		Private Function parseInt(ByVal source As Char(), ByVal start As Integer, ByVal [end] As Integer) As Integer
			Dim result As Integer = Character.digit(source(start), 10)
			start += 1
			If result = -1 Then Throw New NumberFormatException(New String(source))

			For index As Integer = start To [end] - 1
				Dim nextVal As Integer = Character.digit(source(index), 10)
				If nextVal = -1 Then Throw New NumberFormatException(New String(source))
				result = 10*result + nextVal
			Next index

			Return result
		End Function

		' bitsPerDigit in the given radix times 1024
		' Rounded up to avoid underallocation.
		Private Shared bitsPerDigit As Long() = { 0, 0, 1024, 1624, 2048, 2378, 2648, 2875, 3072, 3247, 3402, 3543, 3672, 3790, 3899, 4001, 4096, 4186, 4271, 4350, 4426, 4498, 4567, 4633, 4696, 4756, 4814, 4870, 4923, 4975, 5025, 5074, 5120, 5166, 5210, 5253, 5295}

		' Multiply x array times word y in place, and add word z
		Private Shared Sub destructiveMulAdd(ByVal x As Integer(), ByVal y As Integer, ByVal z As Integer)
			' Perform the multiplication word by word
			Dim ylong As Long = y And LONG_MASK
			Dim zlong As Long = z And LONG_MASK
			Dim len As Integer = x.Length

			Dim product As Long = 0
			Dim carry As Long = 0
			For i As Integer = len-1 To 0 Step -1
				product = ylong * (x(i) And LONG_MASK) + carry
				x(i) = CInt(product)
				carry = CLng(CULng(product) >> 32)
			Next i

			' Perform the addition
			Dim sum As Long = (x(len-1) And LONG_MASK) + zlong
			x(len-1) = CInt(sum)
			carry = CLng(CULng(sum) >> 32)
			For i As Integer = len-2 To 0 Step -1
				sum = (x(i) And LONG_MASK) + carry
				x(i) = CInt(sum)
				carry = CLng(CULng(sum) >> 32)
			Next i
		End Sub

		''' <summary>
		''' Translates the decimal String representation of a BigInteger into a
		''' BigInteger.  The String representation consists of an optional minus
		''' sign followed by a sequence of one or more decimal digits.  The
		''' character-to-digit mapping is provided by {@code Character.digit}.
		''' The String may not contain any extraneous characters (whitespace, for
		''' example).
		''' </summary>
		''' <param name="val"> decimal String representation of BigInteger. </param>
		''' <exception cref="NumberFormatException"> {@code val} is not a valid representation
		'''         of a BigInteger. </exception>
		''' <seealso cref=    Character#digit </seealso>
		Public Sub New(ByVal val As String)
			Me.New(val, 10)
		End Sub

		''' <summary>
		''' Constructs a randomly generated BigInteger, uniformly distributed over
		''' the range 0 to (2<sup>{@code numBits}</sup> - 1), inclusive.
		''' The uniformity of the distribution assumes that a fair source of random
		''' bits is provided in {@code rnd}.  Note that this constructor always
		''' constructs a non-negative BigInteger.
		''' </summary>
		''' <param name="numBits"> maximum bitLength of the new BigInteger. </param>
		''' <param name="rnd"> source of randomness to be used in computing the new
		'''         BigInteger. </param>
		''' <exception cref="IllegalArgumentException"> {@code numBits} is negative. </exception>
		''' <seealso cref= #bitLength() </seealso>
		Public Sub New(ByVal numBits As Integer, ByVal rnd As Random)
			Me.New(1, randomBits(numBits, rnd))
		End Sub

		Private Shared Function randomBits(ByVal numBits As Integer, ByVal rnd As Random) As SByte()
			If numBits < 0 Then Throw New IllegalArgumentException("numBits must be non-negative")
			Dim numBytes As Integer = CInt((CLng(numBits)+7)\8) ' avoid overflow
			Dim randomBits_Renamed As SByte() = New SByte(numBytes - 1){}

			' Generate random bytes and mask out any excess bits
			If numBytes > 0 Then
				rnd.nextBytes(randomBits_Renamed)
				Dim excessBits As Integer = 8*numBytes - numBits
				randomBits_Renamed(0) = randomBits_Renamed(0) And (1 << (8-excessBits)) - 1
			End If
			Return randomBits_Renamed
		End Function

		''' <summary>
		''' Constructs a randomly generated positive BigInteger that is probably
		''' prime, with the specified bitLength.
		''' 
		''' <p>It is recommended that the <seealso cref="#probablePrime probablePrime"/>
		''' method be used in preference to this constructor unless there
		''' is a compelling need to specify a certainty.
		''' </summary>
		''' <param name="bitLength"> bitLength of the returned BigInteger. </param>
		''' <param name="certainty"> a measure of the uncertainty that the caller is
		'''         willing to tolerate.  The probability that the new BigInteger
		'''         represents a prime number will exceed
		'''         (1 - 1/2<sup>{@code certainty}</sup>).  The execution time of
		'''         this constructor is proportional to the value of this parameter. </param>
		''' <param name="rnd"> source of random bits used to select candidates to be
		'''         tested for primality. </param>
		''' <exception cref="ArithmeticException"> {@code bitLength < 2} or {@code bitLength} is too large. </exception>
		''' <seealso cref=    #bitLength() </seealso>
		Public Sub New(ByVal bitLength As Integer, ByVal certainty As Integer, ByVal rnd As Random)
			Dim prime As BigInteger

			If bitLength < 2 Then Throw New ArithmeticException("bitLength < 2")
			prime = (If(bitLength < SMALL_PRIME_THRESHOLD, smallPrime(bitLength, certainty, rnd), largePrime(bitLength, certainty, rnd)))
			signum_Renamed = 1
			mag = prime.mag
		End Sub

		' Minimum size in bits that the requested prime number has
		' before we use the large prime number generating algorithms.
		' The cutoff of 95 was chosen empirically for best performance.
		Private Const SMALL_PRIME_THRESHOLD As Integer = 95

		' Certainty required to meet the spec of probablePrime
		Private Const DEFAULT_PRIME_CERTAINTY As Integer = 100

		''' <summary>
		''' Returns a positive BigInteger that is probably prime, with the
		''' specified bitLength. The probability that a BigInteger returned
		''' by this method is composite does not exceed 2<sup>-100</sup>.
		''' </summary>
		''' <param name="bitLength"> bitLength of the returned BigInteger. </param>
		''' <param name="rnd"> source of random bits used to select candidates to be
		'''         tested for primality. </param>
		''' <returns> a BigInteger of {@code bitLength} bits that is probably prime </returns>
		''' <exception cref="ArithmeticException"> {@code bitLength < 2} or {@code bitLength} is too large. </exception>
		''' <seealso cref=    #bitLength()
		''' @since 1.4 </seealso>
		Public Shared Function probablePrime(ByVal bitLength As Integer, ByVal rnd As Random) As BigInteger
			If bitLength < 2 Then Throw New ArithmeticException("bitLength < 2")

			Return (If(bitLength < SMALL_PRIME_THRESHOLD, smallPrime(bitLength, DEFAULT_PRIME_CERTAINTY, rnd), largePrime(bitLength, DEFAULT_PRIME_CERTAINTY, rnd)))
		End Function

		''' <summary>
		''' Find a random number of the specified bitLength that is probably prime.
		''' This method is used for smaller primes, its performance degrades on
		''' larger bitlengths.
		''' 
		''' This method assumes bitLength > 1.
		''' </summary>
		Private Shared Function smallPrime(ByVal bitLength As Integer, ByVal certainty As Integer, ByVal rnd As Random) As BigInteger
			Dim magLen As Integer = CInt(CUInt((bitLength + 31)) >> 5)
			Dim temp As Integer() = New Integer(magLen - 1){}
			Dim highBit As Integer = 1 << ((bitLength+31) And &H1f) ' High bit of high int
			Dim highMask As Integer = (highBit << 1) - 1 ' Bits to keep in high int

			Do
				' Construct a candidate
				For i As Integer = 0 To magLen - 1
					temp(i) = rnd.Next()
				Next i
				temp(0) = (temp(0) And highMask) Or highBit ' Ensure exact length
				If bitLength > 2 Then temp(magLen-1) = temp(magLen-1) Or 1 ' Make odd if bitlen > 2

				Dim p As New BigInteger(temp, 1)

				' Do cheap "pre-test" if applicable
				If bitLength > 6 Then
					Dim r As Long = p.remainder(SMALL_PRIME_PRODUCT)
					If (r Mod 3=0) OrElse (r Mod 5=0) OrElse (r Mod 7=0) OrElse (r Mod 11=0) OrElse (r Mod 13=0) OrElse (r Mod 17=0) OrElse (r Mod 19=0) OrElse (r Mod 23=0) OrElse (r Mod 29=0) OrElse (r Mod 31=0) OrElse (r Mod 37=0) OrElse (r Mod 41=0) Then Continue Do ' Candidate is composite; try another
				End If

				' All candidates of bitLength 2 and 3 are prime by this point
				If bitLength < 4 Then Return p

				' Do expensive test if we survive pre-test (or it's inapplicable)
				If p.primeToCertainty(certainty, rnd) Then Return p
			Loop
		End Function

		Private Shared ReadOnly SMALL_PRIME_PRODUCT As BigInteger = valueOf(3L*5*7*11*13*17*19*23*29*31*37*41)

		''' <summary>
		''' Find a random number of the specified bitLength that is probably prime.
		''' This method is more appropriate for larger bitlengths since it uses
		''' a sieve to eliminate most composites before using a more expensive
		''' test.
		''' </summary>
		Private Shared Function largePrime(ByVal bitLength As Integer, ByVal certainty As Integer, ByVal rnd As Random) As BigInteger
			Dim p As BigInteger
			p = (New BigInteger(bitLength, rnd)).bitBit(bitLength-1)
			p.mag(p.mag.Length-1) = p.mag(p.mag.Length-1) And &HfffffffeL

			' Use a sieve length likely to contain the next prime number
			Dim searchLen As Integer = getPrimeSearchLen(bitLength)
			Dim searchSieve As New BitSieve(p, searchLen)
			Dim candidate As BigInteger = searchSieve.retrieve(p, certainty, rnd)

			Do While (candidate Is Nothing) OrElse (candidate.bitLength() <> bitLength)
				p = p.add(BigInteger.valueOf(2*searchLen))
				If p.bitLength() <> bitLength Then p = (New BigInteger(bitLength, rnd)).bitBit(bitLength-1)
				p.mag(p.mag.Length-1) = p.mag(p.mag.Length-1) And &HfffffffeL
				searchSieve = New BitSieve(p, searchLen)
				candidate = searchSieve.retrieve(p, certainty, rnd)
			Loop
			Return candidate
		End Function

	   ''' <summary>
	   ''' Returns the first integer greater than this {@code BigInteger} that
	   ''' is probably prime.  The probability that the number returned by this
	   ''' method is composite does not exceed 2<sup>-100</sup>. This method will
	   ''' never skip over a prime when searching: if it returns {@code p}, there
	   ''' is no prime {@code q} such that {@code this < q < p}.
	   ''' </summary>
	   ''' <returns> the first integer greater than this {@code BigInteger} that
	   '''         is probably prime. </returns>
	   ''' <exception cref="ArithmeticException"> {@code this < 0} or {@code this} is too large.
	   ''' @since 1.5 </exception>
		Public Overridable Function nextProbablePrime() As BigInteger
			If Me.signum_Renamed < 0 Then Throw New ArithmeticException("start < 0: " & Me)

			' Handle trivial cases
			If (Me.signum_Renamed = 0) OrElse Me.Equals(ONE) Then Return TWO

			Dim result As BigInteger = Me.add(ONE)

			' Fastpath for small numbers
			If result.bitLength() < SMALL_PRIME_THRESHOLD Then

				' Ensure an odd number
				If Not result.testBit(0) Then result = result.add(ONE)

				Do
					' Do cheap "pre-test" if applicable
					If result.bitLength() > 6 Then
						Dim r As Long = result.remainder(SMALL_PRIME_PRODUCT)
						If (r Mod 3=0) OrElse (r Mod 5=0) OrElse (r Mod 7=0) OrElse (r Mod 11=0) OrElse (r Mod 13=0) OrElse (r Mod 17=0) OrElse (r Mod 19=0) OrElse (r Mod 23=0) OrElse (r Mod 29=0) OrElse (r Mod 31=0) OrElse (r Mod 37=0) OrElse (r Mod 41=0) Then
							result = result.add(TWO)
							Continue Do ' Candidate is composite; try another
						End If
					End If

					' All candidates of bitLength 2 and 3 are prime by this point
					If result.bitLength() < 4 Then Return result

					' The expensive test
					If result.primeToCertainty(DEFAULT_PRIME_CERTAINTY, Nothing) Then Return result

					result = result.add(TWO)
				Loop
			End If

			' Start at previous even number
			If result.testBit(0) Then result = result.subtract(ONE)

			' Looking for the next large prime
			Dim searchLen As Integer = getPrimeSearchLen(result.bitLength())

			Do
			   Dim searchSieve As New BitSieve(result, searchLen)
			   Dim candidate As BigInteger = searchSieve.retrieve(result, DEFAULT_PRIME_CERTAINTY, Nothing)
			   If candidate IsNot Nothing Then Return candidate
			   result = result.add(BigInteger.valueOf(2 * searchLen))
			Loop
		End Function

		Private Shared Function getPrimeSearchLen(ByVal bitLength As Integer) As Integer
			If bitLength > PRIME_SEARCH_BIT_LENGTH_LIMIT + 1 Then Throw New ArithmeticException("Prime search implementation restriction on bitLength")
			Return bitLength \ 20 * 64
		End Function

		''' <summary>
		''' Returns {@code true} if this BigInteger is probably prime,
		''' {@code false} if it's definitely composite.
		''' 
		''' This method assumes bitLength > 2.
		''' </summary>
		''' <param name="certainty"> a measure of the uncertainty that the caller is
		'''         willing to tolerate: if the call returns {@code true}
		'''         the probability that this BigInteger is prime exceeds
		'''         {@code (1 - 1/2<sup>certainty</sup>)}.  The execution time of
		'''         this method is proportional to the value of this parameter. </param>
		''' <returns> {@code true} if this BigInteger is probably prime,
		'''         {@code false} if it's definitely composite. </returns>
		Friend Overridable Function primeToCertainty(ByVal certainty As Integer, ByVal random As Random) As Boolean
			Dim rounds As Integer = 0
			Dim n As Integer = (Math.Min(certainty, Integer.MaxValue-1)+1)/2

			' The relationship between the certainty and the number of rounds
			' we perform is given in the draft standard ANSI X9.80, "PRIME
			' NUMBER GENERATION, PRIMALITY TESTING, AND PRIMALITY CERTIFICATES".
			Dim sizeInBits As Integer = Me.bitLength()
			If sizeInBits < 100 Then
				rounds = 50
				rounds = If(n < rounds, n, rounds)
				Return passesMillerRabin(rounds, random)
			End If

			If sizeInBits < 256 Then
				rounds = 27
			ElseIf sizeInBits < 512 Then
				rounds = 15
			ElseIf sizeInBits < 768 Then
				rounds = 8
			ElseIf sizeInBits < 1024 Then
				rounds = 4
			Else
				rounds = 2
			End If
			rounds = If(n < rounds, n, rounds)

			Return passesMillerRabin(rounds, random) AndAlso passesLucasLehmer()
		End Function

		''' <summary>
		''' Returns true iff this BigInteger is a Lucas-Lehmer probable prime.
		''' 
		''' The following assumptions are made:
		''' This BigInteger is a positive, odd number.
		''' </summary>
		Private Function passesLucasLehmer() As Boolean
			Dim thisPlusOne As BigInteger = Me.add(ONE)

			' Step 1
			Dim d As Integer = 5
			Do While jacobiSymbol(d, Me) <> -1
				' 5, -7, 9, -11, ...
				d = If(d < 0, Math.Abs(d)+2, -(d+2))
			Loop

			' Step 2
			Dim u As BigInteger = lucasLehmerSequence(d, thisPlusOne, Me)

			' Step 3
			Return u.mod(Me).Equals(ZERO)
		End Function

		''' <summary>
		''' Computes Jacobi(p,n).
		''' Assumes n positive, odd, n>=3.
		''' </summary>
		Private Shared Function jacobiSymbol(ByVal p As Integer, ByVal n As BigInteger) As Integer
			If p = 0 Then Return 0

			' Algorithm and comments adapted from Colin Plumb's C library.
			Dim j As Integer = 1
			Dim u As Integer = n.mag(n.mag.Length-1)

			' Make p positive
			If p < 0 Then
				p = -p
				Dim n8 As Integer = u And 7
				If (n8 = 3) OrElse (n8 = 7) Then j = -j ' 3 (011) or 7 (111) mod 8
			End If

			' Get rid of factors of 2 in p
			Do While (p And 3) = 0
				p >>= 2
			Loop
			If (p And 1) = 0 Then
				p >>= 1
				If ((u Xor (u>>1)) And 2) <> 0 Then j = -j ' 3 (011) or 5 (101) mod 8
			End If
			If p = 1 Then Return j
			' Then, apply quadratic reciprocity
			If (p And u And 2) <> 0 Then ' p = u = 3 (mod 4)? j = -j
			' And reduce u mod p
			u = n.mod(BigInteger.valueOf(p))

			' Now compute Jacobi(u,p), u < p
			Do While u <> 0
				Do While (u And 3) = 0
					u >>= 2
				Loop
				If (u And 1) = 0 Then
					u >>= 1
					If ((p Xor (p>>1)) And 2) <> 0 Then j = -j ' 3 (011) or 5 (101) mod 8
				End If
				If u = 1 Then Return j
				' Now both u and p are odd, so use quadratic reciprocity
				assert(u < p)
				Dim t As Integer = u
				u = p
				p = t
				If (u And p And 2) <> 0 Then ' u = p = 3 (mod 4)? j = -j
				' Now u >= p, so it can be reduced
				u = u Mod p
			Loop
			Return 0
		End Function

		Private Shared Function lucasLehmerSequence(ByVal z As Integer, ByVal k As BigInteger, ByVal n As BigInteger) As BigInteger
			Dim d As BigInteger = BigInteger.valueOf(z)
			Dim u As BigInteger = ONE
			Dim u2 As BigInteger
			Dim v As BigInteger = ONE
			Dim v2 As BigInteger

			For i As Integer = k.bitLength()-2 To 0 Step -1
				u2 = u.multiply(v).mod(n)

				v2 = v.square().add(d.multiply(u.square())).mod(n)
				If v2.testBit(0) Then v2 = v2.subtract(n)

				v2 = v2.shiftRight(1)

				u = u2
				v = v2
				If k.testBit(i) Then
					u2 = u.add(v).mod(n)
					If u2.testBit(0) Then u2 = u2.subtract(n)

					u2 = u2.shiftRight(1)
					v2 = v.add(d.multiply(u)).mod(n)
					If v2.testBit(0) Then v2 = v2.subtract(n)
					v2 = v2.shiftRight(1)

					u = u2
					v = v2
				End If
			Next i
			Return u
		End Function

		''' <summary>
		''' Returns true iff this BigInteger passes the specified number of
		''' Miller-Rabin tests. This test is taken from the DSA spec (NIST FIPS
		''' 186-2).
		''' 
		''' The following assumptions are made:
		''' This BigInteger is a positive, odd number greater than 2.
		''' iterations<=50.
		''' </summary>
		Private Function passesMillerRabin(ByVal iterations As Integer, ByVal rnd As Random) As Boolean
			' Find a and m such that m is odd and this == 1 + 2**a * m
			Dim thisMinusOne As BigInteger = Me.subtract(ONE)
			Dim m As BigInteger = thisMinusOne
			Dim a As Integer = m.lowestSetBit
			m = m.shiftRight(a)

			' Do the tests
			If rnd Is Nothing Then rnd = java.util.concurrent.ThreadLocalRandom.current()
			For i As Integer = 0 To iterations - 1
				' Generate a uniform random on (1, this)
				Dim b As BigInteger
				Do
					b = New BigInteger(Me.bitLength(), rnd)
				Loop While b.CompareTo(ONE) <= 0 OrElse b.CompareTo(Me) >= 0

				Dim j As Integer = 0
				Dim z As BigInteger = b.modPow(m, Me)
				Do While Not((j = 0 AndAlso z.Equals(ONE)) OrElse z.Equals(thisMinusOne))
					j += 1
					If j > 0 AndAlso z.Equals(ONE) OrElse j = a Then Return False
					z = z.modPow(TWO, Me)
				Loop
			Next i
			Return True
		End Function

		''' <summary>
		''' This internal constructor differs from its public cousin
		''' with the arguments reversed in two ways: it assumes that its
		''' arguments are correct, and it doesn't copy the magnitude array.
		''' </summary>
		Friend Sub New(ByVal magnitude As Integer(), ByVal signum As Integer)
			Me.signum_Renamed = (If(magnitude.Length = 0, 0, signum))
			Me.mag = magnitude
			If mag.Length >= MAX_MAG_LENGTH Then checkRange()
		End Sub

		''' <summary>
		''' This private constructor is for internal use and assumes that its
		''' arguments are correct.
		''' </summary>
		Private Sub New(ByVal magnitude As SByte(), ByVal signum As Integer)
			Me.signum_Renamed = (If(magnitude.Length = 0, 0, signum))
			Me.mag = stripLeadingZeroBytes(magnitude)
			If mag.Length >= MAX_MAG_LENGTH Then checkRange()
		End Sub

		''' <summary>
		''' Throws an {@code ArithmeticException} if the {@code BigInteger} would be
		''' out of the supported range.
		''' </summary>
		''' <exception cref="ArithmeticException"> if {@code this} exceeds the supported range. </exception>
		Private Sub checkRange()
			If mag.Length > MAX_MAG_LENGTH OrElse mag.Length = MAX_MAG_LENGTH AndAlso mag(0) < 0 Then reportOverflow()
		End Sub

		Private Shared Sub reportOverflow()
			Throw New ArithmeticException("BigInteger would overflow supported range")
		End Sub

		'Static Factory Methods

		''' <summary>
		''' Returns a BigInteger whose value is equal to that of the
		''' specified {@code long}.  This "static factory method" is
		''' provided in preference to a ({@code long}) constructor
		''' because it allows for reuse of frequently used BigIntegers.
		''' </summary>
		''' <param name="val"> value of the BigInteger to return. </param>
		''' <returns> a BigInteger with the specified value. </returns>
		Public Shared Function valueOf(ByVal val As Long) As BigInteger
			' If -MAX_CONSTANT < val < MAX_CONSTANT, return stashed constant
			If val = 0 Then Return ZERO
			If val > 0 AndAlso val <= MAX_CONSTANT Then
				Return posConst(CInt(val))
			ElseIf val < 0 AndAlso val >= -MAX_CONSTANT Then
				Return negConst((Integer) -val)
			End If

			Return New BigInteger(val)
		End Function

		''' <summary>
		''' Constructs a BigInteger with the specified value, which may not be zero.
		''' </summary>
		Private Sub New(ByVal val As Long)
			If val < 0 Then
				val = -val
				signum_Renamed = -1
			Else
				signum_Renamed = 1
			End If

			Dim highWord As Integer = CInt(CLng(CULng(val) >> 32))
			If highWord = 0 Then
				mag = New Integer(0){}
				mag(0) = CInt(val)
			Else
				mag = New Integer(1){}
				mag(0) = highWord
				mag(1) = CInt(val)
			End If
		End Sub

		''' <summary>
		''' Returns a BigInteger with the given two's complement representation.
		''' Assumes that the input array will not be modified (the returned
		''' BigInteger will reference the input array if feasible).
		''' </summary>
		Private Shared Function valueOf(ByVal val As Integer()) As BigInteger
			Return (If(val(0) > 0, New BigInteger(val, 1), New BigInteger(val)))
		End Function

		' Constants

		''' <summary>
		''' Initialize static constant array when class is loaded.
		''' </summary>
		Private Const MAX_CONSTANT As Integer = 16
		Private Shared posConst As BigInteger() = New BigInteger(MAX_CONSTANT){}
		Private Shared negConst As BigInteger() = New BigInteger(MAX_CONSTANT){}

		''' <summary>
		''' The cache of powers of each radix.  This allows us to not have to
		''' recalculate powers of radix^(2^n) more than once.  This speeds
		''' Schoenhage recursive base conversion significantly.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared powerCache As BigInteger()()

		''' <summary>
		''' The cache of logarithms of radices for base conversion. </summary>
		Private Shared ReadOnly logCache As Double()

		''' <summary>
		''' The natural log of 2.  This is used in computing cache indices. </summary>
		Private Shared ReadOnly LOG_TWO As Double = Math.log(2.0)

		Shared Sub New()
			For i As Integer = 1 To MAX_CONSTANT
				Dim magnitude As Integer() = New Integer(0){}
				magnitude(0) = i
				posConst(i) = New BigInteger(magnitude, 1)
				negConst(i) = New BigInteger(magnitude, -1)
			Next i

	'        
	'         * Initialize the cache of radix^(2^x) values used for base conversion
	'         * with just the very first value.  Additional values will be created
	'         * on demand.
	'         
			powerCache = New BigInteger(Character.MAX_RADIX)(){}
			logCache = New Double(Character.MAX_RADIX){}

			For i As Integer = Character.MIN_RADIX To Character.MAX_RADIX
				powerCache(i) = New BigInteger() { BigInteger.valueOf(i) }
				logCache(i) = Math.Log(i)
			Next i
			zeros(63) = "000000000000000000000000000000000000000000000000000000000000000"
			For i As Integer = 0 To 62
				zeros(i) = zeros(63).Substring(0, i)
			Next i
				Try
					unsafe = sun.misc.Unsafe.unsafe
					signumOffset = unsafe.objectFieldOffset(GetType(BigInteger).getDeclaredField("signum"))
					magOffset = unsafe.objectFieldOffset(GetType(BigInteger).getDeclaredField("mag"))
				Catch ex As Exception
					Throw New ExceptionInInitializerError(ex)
				End Try
		End Sub

		''' <summary>
		''' The BigInteger constant zero.
		''' 
		''' @since   1.2
		''' </summary>
		Public Shared ReadOnly ZERO As New BigInteger(New Integer(){}, 0)

		''' <summary>
		''' The BigInteger constant one.
		''' 
		''' @since   1.2
		''' </summary>
		Public Shared ReadOnly ONE As BigInteger = valueOf(1)

		''' <summary>
		''' The BigInteger constant two.  (Not exported.)
		''' </summary>
		Private Shared ReadOnly TWO As BigInteger = valueOf(2)

		''' <summary>
		''' The BigInteger constant -1.  (Not exported.)
		''' </summary>
		Private Shared ReadOnly NEGATIVE_ONE As BigInteger = valueOf(-1)

		''' <summary>
		''' The BigInteger constant ten.
		''' 
		''' @since   1.5
		''' </summary>
		Public Shared ReadOnly TEN As BigInteger = valueOf(10)

		' Arithmetic Operations

		''' <summary>
		''' Returns a BigInteger whose value is {@code (this + val)}.
		''' </summary>
		''' <param name="val"> value to be added to this BigInteger. </param>
		''' <returns> {@code this + val} </returns>
		Public Overridable Function add(ByVal val As BigInteger) As BigInteger
			If val.signum_Renamed = 0 Then Return Me
			If signum_Renamed = 0 Then Return val
			If val.signum_Renamed = signum_Renamed Then Return New BigInteger(add(mag, val.mag), signum_Renamed)

			Dim cmp As Integer = compareMagnitude(val)
			If cmp = 0 Then Return ZERO
			Dim resultMag As Integer() = (If(cmp > 0, subtract(mag, val.mag), subtract(val.mag, mag)))
			resultMag = trustedStripLeadingZeroInts(resultMag)

			Return New BigInteger(resultMag,If(cmp = signum_Renamed, 1, -1))
		End Function

		''' <summary>
		''' Package private methods used by BigDecimal code to add a BigInteger
		''' with a long. Assumes val is not equal to INFLATED.
		''' </summary>
		Friend Overridable Function add(ByVal val As Long) As BigInteger
			If val = 0 Then Return Me
			If signum_Renamed = 0 Then Return valueOf(val)
			If Long.signum(val) = signum_Renamed Then Return New BigInteger(add(mag, Math.Abs(val)), signum_Renamed)
			Dim cmp As Integer = compareMagnitude(val)
			If cmp = 0 Then Return ZERO
			Dim resultMag As Integer() = (If(cmp > 0, subtract(mag, Math.Abs(val)), subtract(Math.Abs(val), mag)))
			resultMag = trustedStripLeadingZeroInts(resultMag)
			Return New BigInteger(resultMag,If(cmp = signum_Renamed, 1, -1))
		End Function

		''' <summary>
		''' Adds the contents of the int array x and long value val. This
		''' method allocates a new int array to hold the answer and returns
		''' a reference to that array.  Assumes x.length &gt; 0 and val is
		''' non-negative
		''' </summary>
		Private Shared Function add(ByVal x As Integer(), ByVal val As Long) As Integer()
			Dim y As Integer()
			Dim sum As Long = 0
			Dim xIndex As Integer = x.Length
			Dim result As Integer()
			Dim highWord As Integer = CInt(CLng(CULng(val) >> 32))
			If highWord = 0 Then
				result = New Integer(xIndex - 1){}
				xIndex -= 1
				sum = (x(xIndex) And LONG_MASK) + val
				result(xIndex) = CInt(sum)
			Else
				If xIndex = 1 Then
					result = New Integer(1){}
					sum = val + (x(0) And LONG_MASK)
					result(1) = CInt(sum)
					result(0) = CInt(CLng(CULng(sum) >> 32))
					Return result
				Else
					result = New Integer(xIndex - 1){}
					xIndex -= 1
					sum = (x(xIndex) And LONG_MASK) + (val And LONG_MASK)
					result(xIndex) = CInt(sum)
					xIndex -= 1
					sum = (x(xIndex) And LONG_MASK) + (highWord And LONG_MASK) + (CLng(CULng(sum) >> 32))
					result(xIndex) = CInt(sum)
				End If
			End If
			' Copy remainder of longer number while carry propagation is required
			Dim carry As Boolean = (CLng(CULng(sum) >> 32 <> 0))
			Do While xIndex > 0 AndAlso carry
					xIndex -= 1
			Loop
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					carry = ((result(xIndex) = x(xIndex) + 1) = 0)
			' Copy remainder of longer number
			Do While xIndex > 0
					xIndex -= 1
			Loop
					result(xIndex) = x(xIndex)
			' Grow result if necessary
			If carry Then
				Dim bigger As Integer() = New Integer(result.Length){}
				Array.Copy(result, 0, bigger, 1, result.Length)
				bigger(0) = &H1
				Return bigger
			End If
			Return result
		End Function

		''' <summary>
		''' Adds the contents of the int arrays x and y. This method allocates
		''' a new int array to hold the answer and returns a reference to that
		''' array.
		''' </summary>
		Private Shared Function add(ByVal x As Integer(), ByVal y As Integer()) As Integer()
			' If x is shorter, swap the two arrays
			If x.Length < y.Length Then
				Dim tmp As Integer() = x
				x = y
				y = tmp
			End If

			Dim xIndex As Integer = x.Length
			Dim yIndex As Integer = y.Length
			Dim result As Integer() = New Integer(xIndex - 1){}
			Dim sum As Long = 0
			If yIndex = 1 Then
				xIndex -= 1
				sum = (x(xIndex) And LONG_MASK) + (y(0) And LONG_MASK)
				result(xIndex) = CInt(sum)
			Else
				' Add common parts of both numbers
				Do While yIndex > 0
					xIndex -= 1
					yIndex -= 1
					sum = (x(xIndex) And LONG_MASK) + (y(yIndex) And LONG_MASK) + (CLng(CULng(sum) >> 32))
					result(xIndex) = CInt(sum)
				Loop
			End If
			' Copy remainder of longer number while carry propagation is required
			Dim carry As Boolean = (CLng(CULng(sum) >> 32 <> 0))
			Do While xIndex > 0 AndAlso carry
					xIndex -= 1
			Loop
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					carry = ((result(xIndex) = x(xIndex) + 1) = 0)

			' Copy remainder of longer number
			Do While xIndex > 0
					xIndex -= 1
			Loop
					result(xIndex) = x(xIndex)

			' Grow result if necessary
			If carry Then
				Dim bigger As Integer() = New Integer(result.Length){}
				Array.Copy(result, 0, bigger, 1, result.Length)
				bigger(0) = &H1
				Return bigger
			End If
			Return result
		End Function

		Private Shared Function subtract(ByVal val As Long, ByVal little As Integer()) As Integer()
			Dim highWord As Integer = CInt(CLng(CULng(val) >> 32))
			If highWord = 0 Then
				Dim result As Integer() = New Integer(0){}
				result(0) = CInt(val - (little(0) And LONG_MASK))
				Return result
			Else
				Dim result As Integer() = New Integer(1){}
				If little.Length = 1 Then
					Dim difference As Long = (CInt(val) And LONG_MASK) - (little(0) And LONG_MASK)
					result(1) = CInt(difference)
					' Subtract remainder of longer number while borrow propagates
					Dim borrow As Boolean = (difference >> 32 <> 0)
					If borrow Then
						result(0) = highWord - 1 ' Copy remainder of longer number
					Else
						result(0) = highWord
					End If
					Return result ' little.length == 2
				Else
					Dim difference As Long = (CInt(val) And LONG_MASK) - (little(1) And LONG_MASK)
					result(1) = CInt(difference)
					difference = (highWord And LONG_MASK) - (little(0) And LONG_MASK) + (difference >> 32)
					result(0) = CInt(difference)
					Return result
				End If
			End If
		End Function

		''' <summary>
		''' Subtracts the contents of the second argument (val) from the
		''' first (big).  The first int array (big) must represent a larger number
		''' than the second.  This method allocates the space necessary to hold the
		''' answer.
		''' assumes val &gt;= 0
		''' </summary>
		Private Shared Function subtract(ByVal big As Integer(), ByVal val As Long) As Integer()
			Dim highWord As Integer = CInt(CLng(CULng(val) >> 32))
			Dim bigIndex As Integer = big.Length
			Dim result As Integer() = New Integer(bigIndex - 1){}
			Dim difference As Long = 0

			If highWord = 0 Then
				bigIndex -= 1
				difference = (big(bigIndex) And LONG_MASK) - val
				result(bigIndex) = CInt(difference)
			Else
				bigIndex -= 1
				difference = (big(bigIndex) And LONG_MASK) - (val And LONG_MASK)
				result(bigIndex) = CInt(difference)
				bigIndex -= 1
				difference = (big(bigIndex) And LONG_MASK) - (highWord And LONG_MASK) + (difference >> 32)
				result(bigIndex) = CInt(difference)
			End If

			' Subtract remainder of longer number while borrow propagates
			Dim borrow As Boolean = (difference >> 32 <> 0)
			Do While bigIndex > 0 AndAlso borrow
					bigIndex -= 1
			Loop
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					borrow = ((result(bigIndex) = big(bigIndex) - 1) = -1)

			' Copy remainder of longer number
			Do While bigIndex > 0
					bigIndex -= 1
			Loop
					result(bigIndex) = big(bigIndex)

			Return result
		End Function

		''' <summary>
		''' Returns a BigInteger whose value is {@code (this - val)}.
		''' </summary>
		''' <param name="val"> value to be subtracted from this BigInteger. </param>
		''' <returns> {@code this - val} </returns>
		Public Overridable Function subtract(ByVal val As BigInteger) As BigInteger
			If val.signum_Renamed = 0 Then Return Me
			If signum_Renamed = 0 Then Return val.negate()
			If val.signum_Renamed <> signum_Renamed Then Return New BigInteger(add(mag, val.mag), signum_Renamed)

			Dim cmp As Integer = compareMagnitude(val)
			If cmp = 0 Then Return ZERO
			Dim resultMag As Integer() = (If(cmp > 0, subtract(mag, val.mag), subtract(val.mag, mag)))
			resultMag = trustedStripLeadingZeroInts(resultMag)
			Return New BigInteger(resultMag,If(cmp = signum_Renamed, 1, -1))
		End Function

		''' <summary>
		''' Subtracts the contents of the second int arrays (little) from the
		''' first (big).  The first int array (big) must represent a larger number
		''' than the second.  This method allocates the space necessary to hold the
		''' answer.
		''' </summary>
		Private Shared Function subtract(ByVal big As Integer(), ByVal little As Integer()) As Integer()
			Dim bigIndex As Integer = big.Length
			Dim result As Integer() = New Integer(bigIndex - 1){}
			Dim littleIndex As Integer = little.Length
			Dim difference As Long = 0

			' Subtract common parts of both numbers
			Do While littleIndex > 0
				bigIndex -= 1
				littleIndex -= 1
				difference = (big(bigIndex) And LONG_MASK) - (little(littleIndex) And LONG_MASK) + (difference >> 32)
				result(bigIndex) = CInt(difference)
			Loop

			' Subtract remainder of longer number while borrow propagates
			Dim borrow As Boolean = (difference >> 32 <> 0)
			Do While bigIndex > 0 AndAlso borrow
					bigIndex -= 1
			Loop
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					borrow = ((result(bigIndex) = big(bigIndex) - 1) = -1)

			' Copy remainder of longer number
			Do While bigIndex > 0
					bigIndex -= 1
			Loop
					result(bigIndex) = big(bigIndex)

			Return result
		End Function

		''' <summary>
		''' Returns a BigInteger whose value is {@code (this * val)}.
		''' 
		''' @implNote An implementation may offer better algorithmic
		''' performance when {@code val == this}.
		''' </summary>
		''' <param name="val"> value to be multiplied by this BigInteger. </param>
		''' <returns> {@code this * val} </returns>
		Public Overridable Function multiply(ByVal val As BigInteger) As BigInteger
			If val.signum_Renamed = 0 OrElse signum_Renamed = 0 Then Return ZERO

			Dim xlen As Integer = mag.Length

			If val Is Me AndAlso xlen > MULTIPLY_SQUARE_THRESHOLD Then Return square()

			Dim ylen As Integer = val.mag.Length

			If (xlen < KARATSUBA_THRESHOLD) OrElse (ylen < KARATSUBA_THRESHOLD) Then
				Dim resultSign As Integer = If(signum_Renamed = val.signum_Renamed, 1, -1)
				If val.mag.Length = 1 Then Return multiplyByInt(mag,val.mag(0), resultSign)
				If mag.Length = 1 Then Return multiplyByInt(val.mag,mag(0), resultSign)
				Dim result As Integer() = multiplyToLen(mag, xlen, val.mag, ylen, Nothing)
				result = trustedStripLeadingZeroInts(result)
				Return New BigInteger(result, resultSign)
			Else
				If (xlen < TOOM_COOK_THRESHOLD) AndAlso (ylen < TOOM_COOK_THRESHOLD) Then
					Return multiplyKaratsuba(Me, val)
				Else
					Return multiplyToomCook3(Me, val)
				End If
			End If
		End Function

		Private Shared Function multiplyByInt(ByVal x As Integer(), ByVal y As Integer, ByVal sign As Integer) As BigInteger
			If Integer.bitCount(y) = 1 Then Return New BigInteger(shiftLeft(x,Integer.numberOfTrailingZeros(y)), sign)
			Dim xlen As Integer = x.Length
			Dim rmag As Integer() = New Integer(xlen){}
			Dim carry As Long = 0
			Dim yl As Long = y And LONG_MASK
			Dim rstart As Integer = rmag.Length - 1
			For i As Integer = xlen - 1 To 0 Step -1
				Dim product As Long = (x(i) And LONG_MASK) * yl + carry
				rmag(rstart) = CInt(product)
				rstart -= 1
				carry = CLng(CULng(product) >> 32)
			Next i
			If carry = 0L Then
				rmag = java.util.Arrays.copyOfRange(rmag, 1, rmag.Length)
			Else
				rmag(rstart) = CInt(carry)
			End If
			Return New BigInteger(rmag, sign)
		End Function

		''' <summary>
		''' Package private methods used by BigDecimal code to multiply a BigInteger
		''' with a long. Assumes v is not equal to INFLATED.
		''' </summary>
		Friend Overridable Function multiply(ByVal v As Long) As BigInteger
			If v = 0 OrElse signum_Renamed = 0 Then Return ZERO
			If v = BigDecimal.INFLATED_Renamed Then Return multiply(BigInteger.valueOf(v))
			Dim rsign As Integer = (If(v > 0, signum_Renamed, -signum_Renamed))
			If v < 0 Then v = -v
			Dim dh As Long = CLng(CULng(v) >> 32) ' higher order bits
			Dim dl As Long = v And LONG_MASK ' lower order bits

			Dim xlen As Integer = mag.Length
			Dim value As Integer() = mag
			Dim rmag As Integer() = If(dh = 0L, (New Integer(xlen){}), (New Integer(xlen + 2 - 1){}))
			Dim carry As Long = 0
			Dim rstart As Integer = rmag.Length - 1
			For i As Integer = xlen - 1 To 0 Step -1
				Dim product As Long = (value(i) And LONG_MASK) * dl + carry
				rmag(rstart) = CInt(product)
				rstart -= 1
				carry = CLng(CULng(product) >> 32)
			Next i
			rmag(rstart) = CInt(carry)
			If dh <> 0L Then
				carry = 0
				rstart = rmag.Length - 2
				For i As Integer = xlen - 1 To 0 Step -1
					Dim product As Long = (value(i) And LONG_MASK) * dh + (rmag(rstart) And LONG_MASK) + carry
					rmag(rstart) = CInt(product)
					rstart -= 1
					carry = CLng(CULng(product) >> 32)
				Next i
				rmag(0) = CInt(carry)
			End If
			If carry = 0L Then rmag = java.util.Arrays.copyOfRange(rmag, 1, rmag.Length)
			Return New BigInteger(rmag, rsign)
		End Function

		''' <summary>
		''' Multiplies int arrays x and y to the specified lengths and places
		''' the result into z. There will be no leading zeros in the resultant array.
		''' </summary>
		Private Function multiplyToLen(ByVal x As Integer(), ByVal xlen As Integer, ByVal y As Integer(), ByVal ylen As Integer, ByVal z As Integer()) As Integer()
			Dim xstart As Integer = xlen - 1
			Dim ystart As Integer = ylen - 1

			If z Is Nothing OrElse z.Length < (xlen+ ylen) Then z = New Integer(xlen+ylen - 1){}

			Dim carry As Long = 0
			Dim j As Integer=ystart
			Dim k As Integer=ystart+1+xstart
			Do While j >= 0
				Dim product As Long = (y(j) And LONG_MASK) * (x(xstart) And LONG_MASK) + carry
				z(k) = CInt(product)
				carry = CLng(CULng(product) >> 32)
				j -= 1
				k -= 1
			Loop
			z(xstart) = CInt(carry)

			For i As Integer = xstart-1 To 0 Step -1
				carry = 0
				j = ystart
				k = ystart+1+i
				Do While j >= 0
					Dim product As Long = (y(j) And LONG_MASK) * (x(i) And LONG_MASK) + (z(k) And LONG_MASK) + carry
					z(k) = CInt(product)
					carry = CLng(CULng(product) >> 32)
					j -= 1
					k -= 1
				Loop
				z(i) = CInt(carry)
			Next i
			Return z
		End Function

		''' <summary>
		''' Multiplies two BigIntegers using the Karatsuba multiplication
		''' algorithm.  This is a recursive divide-and-conquer algorithm which is
		''' more efficient for large numbers than what is commonly called the
		''' "grade-school" algorithm used in multiplyToLen.  If the numbers to be
		''' multiplied have length n, the "grade-school" algorithm has an
		''' asymptotic complexity of O(n^2).  In contrast, the Karatsuba algorithm
		''' has complexity of O(n^(log2(3))), or O(n^1.585).  It achieves this
		''' increased performance by doing 3 multiplies instead of 4 when
		''' evaluating the product.  As it has some overhead, should be used when
		''' both numbers are larger than a certain threshold (found
		''' experimentally).
		''' 
		''' See:  http://en.wikipedia.org/wiki/Karatsuba_algorithm
		''' </summary>
		Private Shared Function multiplyKaratsuba(ByVal x As BigInteger, ByVal y As BigInteger) As BigInteger
			Dim xlen As Integer = x.mag.Length
			Dim ylen As Integer = y.mag.Length

			' The number of ints in each half of the number.
			Dim half As Integer = (Math.Max(xlen, ylen)+1) / 2

			' xl and yl are the lower halves of x and y respectively,
			' xh and yh are the upper halves.
			Dim xl As BigInteger = x.getLower(half)
			Dim xh As BigInteger = x.getUpper(half)
			Dim yl As BigInteger = y.getLower(half)
			Dim yh As BigInteger = y.getUpper(half)

			Dim p1 As BigInteger = xh.multiply(yh) ' p1 = xh*yh
			Dim p2 As BigInteger = xl.multiply(yl) ' p2 = xl*yl

			' p3=(xh+xl)*(yh+yl)
			Dim p3 As BigInteger = xh.add(xl).multiply(yh.add(yl))

			' result = p1 * 2^(32*2*half) + (p3 - p1 - p2) * 2^(32*half) + p2
			Dim result As BigInteger = p1.shiftLeft(32*half).add(p3.subtract(p1).subtract(p2)).shiftLeft(32*half).add(p2)

			If x.signum_Renamed <> y.signum_Renamed Then
				Return result.negate()
			Else
				Return result
			End If
		End Function

		''' <summary>
		''' Multiplies two BigIntegers using a 3-way Toom-Cook multiplication
		''' algorithm.  This is a recursive divide-and-conquer algorithm which is
		''' more efficient for large numbers than what is commonly called the
		''' "grade-school" algorithm used in multiplyToLen.  If the numbers to be
		''' multiplied have length n, the "grade-school" algorithm has an
		''' asymptotic complexity of O(n^2).  In contrast, 3-way Toom-Cook has a
		''' complexity of about O(n^1.465).  It achieves this increased asymptotic
		''' performance by breaking each number into three parts and by doing 5
		''' multiplies instead of 9 when evaluating the product.  Due to overhead
		''' (additions, shifts, and one division) in the Toom-Cook algorithm, it
		''' should only be used when both numbers are larger than a certain
		''' threshold (found experimentally).  This threshold is generally larger
		''' than that for Karatsuba multiplication, so this algorithm is generally
		''' only used when numbers become significantly larger.
		''' 
		''' The algorithm used is the "optimal" 3-way Toom-Cook algorithm outlined
		''' by Marco Bodrato.
		''' 
		'''  See: http://bodrato.it/toom-cook/
		'''       http://bodrato.it/papers/#WAIFI2007
		''' 
		''' "Towards Optimal Toom-Cook Multiplication for Univariate and
		''' Multivariate Polynomials in Characteristic 2 and 0." by Marco BODRATO;
		''' In C.Carlet and B.Sunar, Eds., "WAIFI'07 proceedings", p. 116-133,
		''' LNCS #4547. Springer, Madrid, Spain, June 21-22, 2007.
		''' 
		''' </summary>
		Private Shared Function multiplyToomCook3(ByVal a As BigInteger, ByVal b As BigInteger) As BigInteger
			Dim alen As Integer = a.mag.Length
			Dim blen As Integer = b.mag.Length

			Dim largest As Integer = Math.Max(alen, blen)

			' k is the size (in ints) of the lower-order slices.
			Dim k As Integer = (largest+2)\3 ' Equal to ceil(largest/3)

			' r is the size (in ints) of the highest-order slice.
			Dim r As Integer = largest - 2*k

			' Obtain slices of the numbers. a2 and b2 are the most significant
			' bits of the numbers a and b, and a0 and b0 the least significant.
			Dim a0, a1, a2, b0, b1, b2 As BigInteger
			a2 = a.getToomSlice(k, r, 0, largest)
			a1 = a.getToomSlice(k, r, 1, largest)
			a0 = a.getToomSlice(k, r, 2, largest)
			b2 = b.getToomSlice(k, r, 0, largest)
			b1 = b.getToomSlice(k, r, 1, largest)
			b0 = b.getToomSlice(k, r, 2, largest)

			Dim v0, v1, v2, vm1, vinf, t1, t2, tm1, da1, db1 As BigInteger

			v0 = a0.multiply(b0)
			da1 = a2.add(a0)
			db1 = b2.add(b0)
			vm1 = da1.subtract(a1).multiply(db1.subtract(b1))
			da1 = da1.add(a1)
			db1 = db1.add(b1)
			v1 = da1.multiply(db1)
			v2 = da1.add(a2).shiftLeft(1).subtract(a0).multiply(db1.add(b2).shiftLeft(1).subtract(b0))
			vinf = a2.multiply(b2)

			' The algorithm requires two divisions by 2 and one by 3.
			' All divisions are known to be exact, that is, they do not produce
			' remainders, and all results are positive.  The divisions by 2 are
			' implemented as right shifts which are relatively efficient, leaving
			' only an exact division by 3, which is done by a specialized
			' linear-time algorithm.
			t2 = v2.subtract(vm1).exactDivideBy3()
			tm1 = v1.subtract(vm1).shiftRight(1)
			t1 = v1.subtract(v0)
			t2 = t2.subtract(t1).shiftRight(1)
			t1 = t1.subtract(tm1).subtract(vinf)
			t2 = t2.subtract(vinf.shiftLeft(1))
			tm1 = tm1.subtract(t2)

			' Number of bits to shift left.
			Dim ss As Integer = k*32

			Dim result As BigInteger = vinf.shiftLeft(ss).add(t2).shiftLeft(ss).add(t1).shiftLeft(ss).add(tm1).shiftLeft(ss).add(v0)

			If a.signum_Renamed <> b.signum_Renamed Then
				Return result.negate()
			Else
				Return result
			End If
		End Function


		''' <summary>
		''' Returns a slice of a BigInteger for use in Toom-Cook multiplication.
		''' </summary>
		''' <param name="lowerSize"> The size of the lower-order bit slices. </param>
		''' <param name="upperSize"> The size of the higher-order bit slices. </param>
		''' <param name="slice"> The index of which slice is requested, which must be a
		''' number from 0 to size-1. Slice 0 is the highest-order bits, and slice
		''' size-1 are the lowest-order bits. Slice 0 may be of different size than
		''' the other slices. </param>
		''' <param name="fullsize"> The size of the larger integer array, used to align
		''' slices to the appropriate position when multiplying different-sized
		''' numbers. </param>
		Private Function getToomSlice(ByVal lowerSize As Integer, ByVal upperSize As Integer, ByVal slice As Integer, ByVal fullsize As Integer) As BigInteger
			Dim start, [end], sliceSize, len, offset As Integer

			len = mag.Length
			offset = fullsize - len

			If slice = 0 Then
				start = 0 - offset
				[end] = upperSize - 1 - offset
			Else
				start = upperSize + (slice-1)*lowerSize - offset
				[end] = start + lowerSize - 1
			End If

			If start < 0 Then start = 0
			If [end] < 0 Then Return ZERO

			sliceSize = ([end]-start) + 1

			If sliceSize <= 0 Then Return ZERO

			' While performing Toom-Cook, all slices are positive and
			' the sign is adjusted when the final number is composed.
			If start = 0 AndAlso sliceSize >= len Then Return Me.abs()

			Dim intSlice As Integer() = New Integer(sliceSize - 1){}
			Array.Copy(mag, start, intSlice, 0, sliceSize)

			Return New BigInteger(trustedStripLeadingZeroInts(intSlice), 1)
		End Function

		''' <summary>
		''' Does an exact division (that is, the remainder is known to be zero)
		''' of the specified number by 3.  This is used in Toom-Cook
		''' multiplication.  This is an efficient algorithm that runs in linear
		''' time.  If the argument is not exactly divisible by 3, results are
		''' undefined.  Note that this is expected to be called with positive
		''' arguments only.
		''' </summary>
		Private Function exactDivideBy3() As BigInteger
			Dim len As Integer = mag.Length
			Dim result As Integer() = New Integer(len - 1){}
			Dim x, w, q, borrow As Long
			borrow = 0L
			For i As Integer = len-1 To 0 Step -1
				x = (mag(i) And LONG_MASK)
				w = x - borrow
				If borrow > x Then ' Did we make the number go negative?
					borrow = 1L
				Else
					borrow = 0L
				End If

				' 0xAAAAAAAB is the modular inverse of 3 (mod 2^32).  Thus,
				' the effect of this is to divide by 3 (mod 2^32).
				' This is much faster than division on most architectures.
				q = (w * &HAAAAAAABL) And LONG_MASK
				result(i) = CInt(q)

				' Now check the borrow. The second check can of course be
				' eliminated if the first fails.
				If q >= &H55555556L Then
					borrow += 1
					If q >= &HAAAAAAABL Then borrow += 1
				End If
			Next i
			result = trustedStripLeadingZeroInts(result)
			Return New BigInteger(result, signum_Renamed)
		End Function

		''' <summary>
		''' Returns a new BigInteger representing n lower ints of the number.
		''' This is used by Karatsuba multiplication and Karatsuba squaring.
		''' </summary>
		Private Function getLower(ByVal n As Integer) As BigInteger
			Dim len As Integer = mag.Length

			If len <= n Then Return abs()

			Dim lowerInts As Integer() = New Integer(n - 1){}
			Array.Copy(mag, len-n, lowerInts, 0, n)

			Return New BigInteger(trustedStripLeadingZeroInts(lowerInts), 1)
		End Function

		''' <summary>
		''' Returns a new BigInteger representing mag.length-n upper
		''' ints of the number.  This is used by Karatsuba multiplication and
		''' Karatsuba squaring.
		''' </summary>
		Private Function getUpper(ByVal n As Integer) As BigInteger
			Dim len As Integer = mag.Length

			If len <= n Then Return ZERO

			Dim upperLen As Integer = len - n
			Dim upperInts As Integer() = New Integer(upperLen - 1){}
			Array.Copy(mag, 0, upperInts, 0, upperLen)

			Return New BigInteger(trustedStripLeadingZeroInts(upperInts), 1)
		End Function

		' Squaring

		''' <summary>
		''' Returns a BigInteger whose value is {@code (this<sup>2</sup>)}.
		''' </summary>
		''' <returns> {@code this<sup>2</sup>} </returns>
		Private Function square() As BigInteger
			If signum_Renamed = 0 Then Return ZERO
			Dim len As Integer = mag.Length

			If len < KARATSUBA_SQUARE_THRESHOLD Then
				Dim z As Integer() = squareToLen(mag, len, Nothing)
				Return New BigInteger(trustedStripLeadingZeroInts(z), 1)
			Else
				If len < TOOM_COOK_SQUARE_THRESHOLD Then
					Return squareKaratsuba()
				Else
					Return squareToomCook3()
				End If
			End If
		End Function

		''' <summary>
		''' Squares the contents of the int array x. The result is placed into the
		''' int array z.  The contents of x are not changed.
		''' </summary>
		Private Shared Function squareToLen(ByVal x As Integer(), ByVal len As Integer, ByVal z As Integer()) As Integer()
	'        
	'         * The algorithm used here is adapted from Colin Plumb's C library.
	'         * Technique: Consider the partial products in the multiplication
	'         * of "abcde" by itself:
	'         *
	'         *               a  b  c  d  e
	'         *            *  a  b  c  d  e
	'         *          ==================
	'         *              ae be ce de ee
	'         *           ad bd cd dd de
	'         *        ac bc cc cd ce
	'         *     ab bb bc bd be
	'         *  aa ab ac ad ae
	'         *
	'         * Note that everything above the main diagonal:
	'         *              ae be ce de = (abcd) * e
	'         *           ad bd cd       = (abc) * d
	'         *        ac bc             = (ab) * c
	'         *     ab                   = (a) * b
	'         *
	'         * is a copy of everything below the main diagonal:
	'         *                       de
	'         *                 cd ce
	'         *           bc bd be
	'         *     ab ac ad ae
	'         *
	'         * Thus, the sum is 2 * (off the diagonal) + diagonal.
	'         *
	'         * This is accumulated beginning with the diagonal (which
	'         * consist of the squares of the digits of the input), which is then
	'         * divided by two, the off-diagonal added, and multiplied by two
	'         * again.  The low bit is simply a copy of the low bit of the
	'         * input, so it doesn't need special care.
	'         
			Dim zlen As Integer = len << 1
			If z Is Nothing OrElse z.Length < zlen Then z = New Integer(zlen - 1){}

			' Store the squares, right shifted one bit (i.e., divided by 2)
			Dim lastProductLowWord As Integer = 0
			Dim j As Integer=0
			Dim i As Integer=0
			Do While j < len
				Dim piece As Long = (x(j) And LONG_MASK)
				Dim product As Long = piece * piece
				z(i) = (lastProductLowWord << 31) Or CInt(CLng(CULng(product) >> 33))
				i += 1
				z(i) = CInt(CLng(CULng(product) >> 1))
				i += 1
				lastProductLowWord = CInt(product)
				j += 1
			Loop

			' Add in off-diagonal sums
			i = len
			Dim offset As Integer=1
			Do While i > 0
				Dim t As Integer = x(i-1)
				t = mulAdd(z, x, offset, i-1, t)
				addOne(z, offset-1, i, t)
				i -= 1
				offset+=2
			Loop

			' Shift back up and set low bit
			primitiveLeftShift(z, zlen, 1)
			z(zlen-1) = z(zlen-1) Or x(len-1) And 1

			Return z
		End Function

		''' <summary>
		''' Squares a BigInteger using the Karatsuba squaring algorithm.  It should
		''' be used when both numbers are larger than a certain threshold (found
		''' experimentally).  It is a recursive divide-and-conquer algorithm that
		''' has better asymptotic performance than the algorithm used in
		''' squareToLen.
		''' </summary>
		Private Function squareKaratsuba() As BigInteger
			Dim half As Integer = (mag.Length+1) \ 2

			Dim xl As BigInteger = getLower(half)
			Dim xh As BigInteger = getUpper(half)

			Dim xhs As BigInteger = xh.square() ' xhs = xh^2
			Dim xls As BigInteger = xl.square() ' xls = xl^2

			' xh^2 << 64  +  (((xl+xh)^2 - (xh^2 + xl^2)) << 32) + xl^2
			Return xhs.shiftLeft(half*32).add(xl.add(xh).square().subtract(xhs.add(xls))).shiftLeft(half*32).add(xls)
		End Function

		''' <summary>
		''' Squares a BigInteger using the 3-way Toom-Cook squaring algorithm.  It
		''' should be used when both numbers are larger than a certain threshold
		''' (found experimentally).  It is a recursive divide-and-conquer algorithm
		''' that has better asymptotic performance than the algorithm used in
		''' squareToLen or squareKaratsuba.
		''' </summary>
		Private Function squareToomCook3() As BigInteger
			Dim len As Integer = mag.Length

			' k is the size (in ints) of the lower-order slices.
			Dim k As Integer = (len+2)\3 ' Equal to ceil(largest/3)

			' r is the size (in ints) of the highest-order slice.
			Dim r As Integer = len - 2*k

			' Obtain slices of the numbers. a2 is the most significant
			' bits of the number, and a0 the least significant.
			Dim a0, a1, a2 As BigInteger
			a2 = getToomSlice(k, r, 0, len)
			a1 = getToomSlice(k, r, 1, len)
			a0 = getToomSlice(k, r, 2, len)
			Dim v0, v1, v2, vm1, vinf, t1, t2, tm1, da1 As BigInteger

			v0 = a0.square()
			da1 = a2.add(a0)
			vm1 = da1.subtract(a1).square()
			da1 = da1.add(a1)
			v1 = da1.square()
			vinf = a2.square()
			v2 = da1.add(a2).shiftLeft(1).subtract(a0).square()

			' The algorithm requires two divisions by 2 and one by 3.
			' All divisions are known to be exact, that is, they do not produce
			' remainders, and all results are positive.  The divisions by 2 are
			' implemented as right shifts which are relatively efficient, leaving
			' only a division by 3.
			' The division by 3 is done by an optimized algorithm for this case.
			t2 = v2.subtract(vm1).exactDivideBy3()
			tm1 = v1.subtract(vm1).shiftRight(1)
			t1 = v1.subtract(v0)
			t2 = t2.subtract(t1).shiftRight(1)
			t1 = t1.subtract(tm1).subtract(vinf)
			t2 = t2.subtract(vinf.shiftLeft(1))
			tm1 = tm1.subtract(t2)

			' Number of bits to shift left.
			Dim ss As Integer = k*32

			Return vinf.shiftLeft(ss).add(t2).shiftLeft(ss).add(t1).shiftLeft(ss).add(tm1).shiftLeft(ss).add(v0)
		End Function

		' Division

		''' <summary>
		''' Returns a BigInteger whose value is {@code (this / val)}.
		''' </summary>
		''' <param name="val"> value by which this BigInteger is to be divided. </param>
		''' <returns> {@code this / val} </returns>
		''' <exception cref="ArithmeticException"> if {@code val} is zero. </exception>
		Public Overridable Function divide(ByVal val As BigInteger) As BigInteger
			If val.mag.Length < BURNIKEL_ZIEGLER_THRESHOLD OrElse mag.Length - val.mag.Length < BURNIKEL_ZIEGLER_OFFSET Then
				Return divideKnuth(val)
			Else
				Return divideBurnikelZiegler(val)
			End If
		End Function

		''' <summary>
		''' Returns a BigInteger whose value is {@code (this / val)} using an O(n^2) algorithm from Knuth.
		''' </summary>
		''' <param name="val"> value by which this BigInteger is to be divided. </param>
		''' <returns> {@code this / val} </returns>
		''' <exception cref="ArithmeticException"> if {@code val} is zero. </exception>
		''' <seealso cref= MutableBigInteger#divideKnuth(MutableBigInteger, MutableBigInteger, boolean) </seealso>
		Private Function divideKnuth(ByVal val As BigInteger) As BigInteger
			Dim q As New MutableBigInteger, a As New MutableBigInteger(Me.mag), b As New MutableBigInteger(val.mag)

			a.divideKnuth(b, q, False)
			Return q.toBigInteger(Me.signum_Renamed * val.signum_Renamed)
		End Function

		''' <summary>
		''' Returns an array of two BigIntegers containing {@code (this / val)}
		''' followed by {@code (this % val)}.
		''' </summary>
		''' <param name="val"> value by which this BigInteger is to be divided, and the
		'''         remainder computed. </param>
		''' <returns> an array of two BigIntegers: the quotient {@code (this / val)}
		'''         is the initial element, and the remainder {@code (this % val)}
		'''         is the final element. </returns>
		''' <exception cref="ArithmeticException"> if {@code val} is zero. </exception>
		Public Overridable Function divideAndRemainder(ByVal val As BigInteger) As BigInteger()
			If val.mag.Length < BURNIKEL_ZIEGLER_THRESHOLD OrElse mag.Length - val.mag.Length < BURNIKEL_ZIEGLER_OFFSET Then
				Return divideAndRemainderKnuth(val)
			Else
				Return divideAndRemainderBurnikelZiegler(val)
			End If
		End Function

		''' <summary>
		''' Long division </summary>
		Private Function divideAndRemainderKnuth(ByVal val As BigInteger) As BigInteger()
			Dim result As BigInteger() = New BigInteger(1){}
			Dim q As New MutableBigInteger, a As New MutableBigInteger(Me.mag), b As New MutableBigInteger(val.mag)
			Dim r As MutableBigInteger = a.divideKnuth(b, q)
			result(0) = q.toBigInteger(If(Me.signum_Renamed = val.signum_Renamed, 1, -1))
			result(1) = r.toBigInteger(Me.signum_Renamed)
			Return result
		End Function

		''' <summary>
		''' Returns a BigInteger whose value is {@code (this % val)}.
		''' </summary>
		''' <param name="val"> value by which this BigInteger is to be divided, and the
		'''         remainder computed. </param>
		''' <returns> {@code this % val} </returns>
		''' <exception cref="ArithmeticException"> if {@code val} is zero. </exception>
		Public Overridable Function remainder(ByVal val As BigInteger) As BigInteger
			If val.mag.Length < BURNIKEL_ZIEGLER_THRESHOLD OrElse mag.Length - val.mag.Length < BURNIKEL_ZIEGLER_OFFSET Then
				Return remainderKnuth(val)
			Else
				Return remainderBurnikelZiegler(val)
			End If
		End Function

		''' <summary>
		''' Long division </summary>
		Private Function remainderKnuth(ByVal val As BigInteger) As BigInteger
			Dim q As New MutableBigInteger, a As New MutableBigInteger(Me.mag), b As New MutableBigInteger(val.mag)

			Return a.divideKnuth(b, q).toBigInteger(Me.signum_Renamed)
		End Function

		''' <summary>
		''' Calculates {@code this / val} using the Burnikel-Ziegler algorithm. </summary>
		''' <param name="val"> the divisor </param>
		''' <returns> {@code this / val} </returns>
		Private Function divideBurnikelZiegler(ByVal val As BigInteger) As BigInteger
			Return divideAndRemainderBurnikelZiegler(val)(0)
		End Function

		''' <summary>
		''' Calculates {@code this % val} using the Burnikel-Ziegler algorithm. </summary>
		''' <param name="val"> the divisor </param>
		''' <returns> {@code this % val} </returns>
		Private Function remainderBurnikelZiegler(ByVal val As BigInteger) As BigInteger
			Return divideAndRemainderBurnikelZiegler(val)(1)
		End Function

		''' <summary>
		''' Computes {@code this / val} and {@code this % val} using the
		''' Burnikel-Ziegler algorithm. </summary>
		''' <param name="val"> the divisor </param>
		''' <returns> an array containing the quotient and remainder </returns>
		Private Function divideAndRemainderBurnikelZiegler(ByVal val As BigInteger) As BigInteger()
			Dim q As New MutableBigInteger
			Dim r As (New MutableBigInteger(Me)).divideAndRemainderBurnikelZiegler(New MutableBigInteger(val), q)
			Dim qBigInt As BigInteger = If(q.zero, ZERO, q.toBigInteger(signum_Renamed*val.signum_Renamed))
			Dim rBigInt As BigInteger = If(r.zero, ZERO, r.toBigInteger(signum_Renamed))
			Return New BigInteger() {qBigInt, rBigInt}
		End Function

		''' <summary>
		''' Returns a BigInteger whose value is <tt>(this<sup>exponent</sup>)</tt>.
		''' Note that {@code exponent} is an integer rather than a BigInteger.
		''' </summary>
		''' <param name="exponent"> exponent to which this BigInteger is to be raised. </param>
		''' <returns> <tt>this<sup>exponent</sup></tt> </returns>
		''' <exception cref="ArithmeticException"> {@code exponent} is negative.  (This would
		'''         cause the operation to yield a non-integer value.) </exception>
		Public Overridable Function pow(ByVal exponent As Integer) As BigInteger
			If exponent < 0 Then Throw New ArithmeticException("Negative exponent")
			If signum_Renamed = 0 Then Return (If(exponent = 0, ONE, Me))

			Dim partToSquare As BigInteger = Me.abs()

			' Factor out powers of two from the base, as the exponentiation of
			' these can be done by left shifts only.
			' The remaining part can then be exponentiated faster.  The
			' powers of two will be multiplied back at the end.
			Dim powersOfTwo As Integer = partToSquare.lowestSetBit
			Dim bitsToShift As Long = CLng(powersOfTwo) * exponent
			If bitsToShift > Integer.MaxValue Then reportOverflow()

			Dim remainingBits As Integer

			' Factor the powers of two out quickly by shifting right, if needed.
			If powersOfTwo > 0 Then
				partToSquare = partToSquare.shiftRight(powersOfTwo)
				remainingBits = partToSquare.bitLength()
				If remainingBits = 1 Then ' Nothing left but +/- 1?
					If signum_Renamed < 0 AndAlso (exponent And 1) = 1 Then
						Return NEGATIVE_ONE.shiftLeft(powersOfTwo*exponent)
					Else
						Return ONE.shiftLeft(powersOfTwo*exponent)
					End If
				End If
			Else
				remainingBits = partToSquare.bitLength()
				If remainingBits = 1 Then ' Nothing left but +/- 1?
					If signum_Renamed < 0 AndAlso (exponent And 1) = 1 Then
						Return NEGATIVE_ONE
					Else
						Return ONE
					End If
				End If
			End If

			' This is a quick way to approximate the size of the result,
			' similar to doing log2[n] * exponent.  This will give an upper bound
			' of how big the result can be, and which algorithm to use.
			Dim scaleFactor As Long = CLng(remainingBits) * exponent

			' Use slightly different algorithms for small and large operands.
			' See if the result will safely fit into a long. (Largest 2^63-1)
			If partToSquare.mag.Length = 1 AndAlso scaleFactor <= 62 Then
				' Small number algorithm.  Everything fits into a long.
				Dim newSign As Integer = (If(signum_Renamed <0 AndAlso (exponent And 1) = 1, -1, 1))
				Dim result As Long = 1
				Dim baseToPow2 As Long = partToSquare.mag(0) And LONG_MASK

				Dim workingExponent As Integer = exponent

				' Perform exponentiation using repeated squaring trick
				Do While workingExponent <> 0
					If (workingExponent And 1) = 1 Then result = result * baseToPow2

					If (workingExponent >>>= 1) <> 0 Then baseToPow2 = baseToPow2 * baseToPow2
				Loop

				' Multiply back the powers of two (quickly, by shifting left)
				If powersOfTwo > 0 Then
					If bitsToShift + scaleFactor <= 62 Then ' Fits in long?
						Return valueOf((result << bitsToShift) * newSign)
					Else
						Return valueOf(result*newSign).shiftLeft(CInt(bitsToShift))
					End If
				Else
					Return valueOf(result*newSign)
				End If
			Else
				' Large number algorithm.  This is basically identical to
				' the algorithm above, but calls multiply() and square()
				' which may use more efficient algorithms for large numbers.
				Dim answer As BigInteger = ONE

				Dim workingExponent As Integer = exponent
				' Perform exponentiation using repeated squaring trick
				Do While workingExponent <> 0
					If (workingExponent And 1) = 1 Then answer = answer.multiply(partToSquare)

					If (workingExponent >>>= 1) <> 0 Then partToSquare = partToSquare.square()
				Loop
				' Multiply back the (exponentiated) powers of two (quickly,
				' by shifting left)
				If powersOfTwo > 0 Then answer = answer.shiftLeft(powersOfTwo*exponent)

				If signum_Renamed < 0 AndAlso (exponent And 1) = 1 Then
					Return answer.negate()
				Else
					Return answer
				End If
			End If
		End Function

		''' <summary>
		''' Returns a BigInteger whose value is the greatest common divisor of
		''' {@code abs(this)} and {@code abs(val)}.  Returns 0 if
		''' {@code this == 0 && val == 0}.
		''' </summary>
		''' <param name="val"> value with which the GCD is to be computed. </param>
		''' <returns> {@code GCD(abs(this), abs(val))} </returns>
		Public Overridable Function gcd(ByVal val As BigInteger) As BigInteger
			If val.signum_Renamed = 0 Then
				Return Me.abs()
			ElseIf Me.signum_Renamed = 0 Then
				Return val.abs()
			End If

			Dim a As New MutableBigInteger(Me)
			Dim b As New MutableBigInteger(val)

			Dim result As MutableBigInteger = a.hybridGCD(b)

			Return result.toBigInteger(1)
		End Function

		''' <summary>
		''' Package private method to return bit length for an integer.
		''' </summary>
		Friend Shared Function bitLengthForInt(ByVal n As Integer) As Integer
			Return 32 - Integer.numberOfLeadingZeros(n)
		End Function

		''' <summary>
		''' Left shift int array a up to len by n bits. Returns the array that
		''' results from the shift since space may have to be reallocated.
		''' </summary>
		Private Shared Function leftShift(ByVal a As Integer(), ByVal len As Integer, ByVal n As Integer) As Integer()
			Dim nInts As Integer = CInt(CUInt(n) >> 5)
			Dim nBits As Integer = n And &H1F
			Dim bitsInHighWord As Integer = bitLengthForInt(a(0))

			' If shift can be done without recopy, do so
			If n <= (32-bitsInHighWord) Then
				primitiveLeftShift(a, len, nBits)
				Return a ' Array must be resized
			Else
				If nBits <= (32-bitsInHighWord) Then
					Dim result As Integer() = New Integer(nInts+len - 1){}
					Array.Copy(a, 0, result, 0, len)
					primitiveLeftShift(result, result.Length, nBits)
					Return result
				Else
					Dim result As Integer() = New Integer(nInts+len){}
					Array.Copy(a, 0, result, 0, len)
					primitiveRightShift(result, result.Length, 32 - nBits)
					Return result
				End If
			End If
		End Function

		' shifts a up to len right n bits assumes no leading zeros, 0<n<32
		Friend Shared Sub primitiveRightShift(ByVal a As Integer(), ByVal len As Integer, ByVal n As Integer)
			Dim n2 As Integer = 32 - n
			Dim i As Integer=len-1
			Dim c As Integer=a(i)
			Do While i > 0
				Dim b As Integer = c
				c = a(i-1)
				a(i) = (c << n2) Or (CInt(CUInt(b) >> n))
				i -= 1
			Loop
			a(0) >>>= n
		End Sub

		' shifts a up to len left n bits assumes no leading zeros, 0<=n<32
		Friend Shared Sub primitiveLeftShift(ByVal a As Integer(), ByVal len As Integer, ByVal n As Integer)
			If len = 0 OrElse n = 0 Then Return

			Dim n2 As Integer = 32 - n
			Dim i As Integer=0
			Dim c As Integer=a(i)
			Dim m As Integer=i+len-1
			Do While i < m
				Dim b As Integer = c
				c = a(i+1)
				a(i) = (b << n) Or (CInt(CUInt(c) >> n2))
				i += 1
			Loop
			a(len-1) <<= n
		End Sub

		''' <summary>
		''' Calculate bitlength of contents of the first len elements an int array,
		''' assuming there are no leading zero ints.
		''' </summary>
		Private Shared Function bitLength(ByVal val As Integer(), ByVal len As Integer) As Integer
			If len = 0 Then Return 0
			Return ((len - 1) << 5) + bitLengthForInt(val(0))
		End Function

		''' <summary>
		''' Returns a BigInteger whose value is the absolute value of this
		''' BigInteger.
		''' </summary>
		''' <returns> {@code abs(this)} </returns>
		Public Overridable Function abs() As BigInteger
			Return (If(signum_Renamed >= 0, Me, Me.negate()))
		End Function

		''' <summary>
		''' Returns a BigInteger whose value is {@code (-this)}.
		''' </summary>
		''' <returns> {@code -this} </returns>
		Public Overridable Function negate() As BigInteger
			Return New BigInteger(Me.mag, -Me.signum_Renamed)
		End Function

		''' <summary>
		''' Returns the signum function of this BigInteger.
		''' </summary>
		''' <returns> -1, 0 or 1 as the value of this BigInteger is negative, zero or
		'''         positive. </returns>
		Public Overridable Function signum() As Integer
			Return Me.signum_Renamed
		End Function

		' Modular Arithmetic Operations

		''' <summary>
		''' Returns a BigInteger whose value is {@code (this mod m}).  This method
		''' differs from {@code remainder} in that it always returns a
		''' <i>non-negative</i> BigInteger.
		''' </summary>
		''' <param name="m"> the modulus. </param>
		''' <returns> {@code this mod m} </returns>
		''' <exception cref="ArithmeticException"> {@code m} &le; 0 </exception>
		''' <seealso cref=    #remainder </seealso>
		Public Overridable Function [mod](ByVal m As BigInteger) As BigInteger
			If m.signum_Renamed <= 0 Then Throw New ArithmeticException("BigInteger: modulus not positive")

			Dim result As BigInteger = Me.remainder(m)
			Return (If(result.signum_Renamed >= 0, result, result.add(m)))
		End Function

		''' <summary>
		''' Returns a BigInteger whose value is
		''' <tt>(this<sup>exponent</sup> mod m)</tt>.  (Unlike {@code pow}, this
		''' method permits negative exponents.)
		''' </summary>
		''' <param name="exponent"> the exponent. </param>
		''' <param name="m"> the modulus. </param>
		''' <returns> <tt>this<sup>exponent</sup> mod m</tt> </returns>
		''' <exception cref="ArithmeticException"> {@code m} &le; 0 or the exponent is
		'''         negative and this BigInteger is not <i>relatively
		'''         prime</i> to {@code m}. </exception>
		''' <seealso cref=    #modInverse </seealso>
		Public Overridable Function modPow(ByVal exponent As BigInteger, ByVal m As BigInteger) As BigInteger
			If m.signum_Renamed <= 0 Then Throw New ArithmeticException("BigInteger: modulus not positive")

			' Trivial cases
			If exponent.signum_Renamed = 0 Then Return (If(m.Equals(ONE), ZERO, ONE))

			If Me.Equals(ONE) Then Return (If(m.Equals(ONE), ZERO, ONE))

			If Me.Equals(ZERO) AndAlso exponent.signum_Renamed >= 0 Then Return ZERO

			If Me.Equals(negConst(1)) AndAlso ((Not exponent.testBit(0))) Then Return (If(m.Equals(ONE), ZERO, ONE))

			Dim invertResult As Boolean
			invertResult = (exponent.signum_Renamed < 0)
			If invertResult Then exponent = exponent.negate()

			Dim base As BigInteger = (If(Me.signum_Renamed < 0 OrElse Me.CompareTo(m) >= 0, Me.mod(m), Me))
			Dim result As BigInteger
			If m.testBit(0) Then ' odd modulus
				result = base.oddModPow(exponent, m)
			Else
	'            
	'             * Even modulus.  Tear it into an "odd part" (m1) and power of two
	'             * (m2), exponentiate mod m1, manually exponentiate mod m2, and
	'             * use Chinese Remainder Theorem to combine results.
	'             

				' Tear m apart into odd part (m1) and power of 2 (m2)
				Dim p As Integer = m.lowestSetBit ' Max pow of 2 that divides m

				Dim m1 As BigInteger = m.shiftRight(p) ' m/2**p
				Dim m2 As BigInteger = ONE.shiftLeft(p) ' 2**p

				' Calculate new base from m1
				Dim base2 As BigInteger = (If(Me.signum_Renamed < 0 OrElse Me.CompareTo(m1) >= 0, Me.mod(m1), Me))

				' Caculate (base ** exponent) mod m1.
				Dim a1 As BigInteger = (If(m1.Equals(ONE), ZERO, base2.oddModPow(exponent, m1)))

				' Calculate (this ** exponent) mod m2
				Dim a2 As BigInteger = base.modPow2(exponent, p)

				' Combine results using Chinese Remainder Theorem
				Dim y1 As BigInteger = m2.modInverse(m1)
				Dim y2 As BigInteger = m1.modInverse(m2)

				If m.mag.Length < MAX_MAG_LENGTH \ 2 Then
					result = a1.multiply(m2).multiply(y1).add(a2.multiply(m1).multiply(y2)).mod(m)
				Else
					Dim t1 As New MutableBigInteger
					CType(New MutableBigInteger(a1.multiply(m2)), MutableBigInteger).multiply(New MutableBigInteger(y1), t1)
					Dim t2 As New MutableBigInteger
					CType(New MutableBigInteger(a2.multiply(m1)), MutableBigInteger).multiply(New MutableBigInteger(y2), t2)
					t1.add(t2)
					Dim q As New MutableBigInteger
					result = t1.divide(New MutableBigInteger(m), q).toBigInteger()
				End If
			End If

			Return (If(invertResult, result.modInverse(m), result))
		End Function

		Friend Shared bnExpModThreshTable As Integer() = {7, 25, 81, 241, 673, 1793, Integer.MAX_VALUE}

		''' <summary>
		''' Returns a BigInteger whose value is x to the power of y mod z.
		''' Assumes: z is odd && x < z.
		''' </summary>
		Private Function oddModPow(ByVal y As BigInteger, ByVal z As BigInteger) As BigInteger
	'    
	'     * The algorithm is adapted from Colin Plumb's C library.
	'     *
	'     * The window algorithm:
	'     * The idea is to keep a running product of b1 = n^(high-order bits of exp)
	'     * and then keep appending exponent bits to it.  The following patterns
	'     * apply to a 3-bit window (k = 3):
	'     * To append   0: square
	'     * To append   1: square, multiply by n^1
	'     * To append  10: square, multiply by n^1, square
	'     * To append  11: square, square, multiply by n^3
	'     * To append 100: square, multiply by n^1, square, square
	'     * To append 101: square, square, square, multiply by n^5
	'     * To append 110: square, square, multiply by n^3, square
	'     * To append 111: square, square, square, multiply by n^7
	'     *
	'     * Since each pattern involves only one multiply, the longer the pattern
	'     * the better, except that a 0 (no multiplies) can be appended directly.
	'     * We precompute a table of odd powers of n, up to 2^k, and can then
	'     * multiply k bits of exponent at a time.  Actually, assuming random
	'     * exponents, there is on average one zero bit between needs to
	'     * multiply (1/2 of the time there's none, 1/4 of the time there's 1,
	'     * 1/8 of the time, there's 2, 1/32 of the time, there's 3, etc.), so
	'     * you have to do one multiply per k+1 bits of exponent.
	'     *
	'     * The loop walks down the exponent, squaring the result buffer as
	'     * it goes.  There is a wbits+1 bit lookahead buffer, buf, that is
	'     * filled with the upcoming exponent bits.  (What is read after the
	'     * end of the exponent is unimportant, but it is filled with zero here.)
	'     * When the most-significant bit of this buffer becomes set, i.e.
	'     * (buf & tblmask) != 0, we have to decide what pattern to multiply
	'     * by, and when to do it.  We decide, remember to do it in future
	'     * after a suitable number of squarings have passed (e.g. a pattern
	'     * of "100" in the buffer requires that we multiply by n^1 immediately;
	'     * a pattern of "110" calls for multiplying by n^3 after one more
	'     * squaring), clear the buffer, and continue.
	'     *
	'     * When we start, there is one more optimization: the result buffer
	'     * is implcitly one, so squaring it or multiplying by it can be
	'     * optimized away.  Further, if we start with a pattern like "100"
	'     * in the lookahead window, rather than placing n into the buffer
	'     * and then starting to square it, we have already computed n^2
	'     * to compute the odd-powers table, so we can place that into
	'     * the buffer and save a squaring.
	'     *
	'     * This means that if you have a k-bit window, to compute n^z,
	'     * where z is the high k bits of the exponent, 1/2 of the time
	'     * it requires no squarings.  1/4 of the time, it requires 1
	'     * squaring, ... 1/2^(k-1) of the time, it reqires k-2 squarings.
	'     * And the remaining 1/2^(k-1) of the time, the top k bits are a
	'     * 1 followed by k-1 0 bits, so it again only requires k-2
	'     * squarings, not k-1.  The average of these is 1.  Add that
	'     * to the one squaring we have to do to compute the table,
	'     * and you'll see that a k-bit window saves k-2 squarings
	'     * as well as reducing the multiplies.  (It actually doesn't
	'     * hurt in the case k = 1, either.)
	'     
			' Special case for exponent of one
			If y.Equals(ONE) Then Return Me

			' Special case for base of zero
			If signum_Renamed = 0 Then Return ZERO

			Dim base As Integer() = mag.clone()
			Dim exp As Integer() = y.mag
			Dim [mod] As Integer() = z.mag
			Dim modLen As Integer = [mod].Length

			' Select an appropriate window size
			Dim wbits As Integer = 0
			Dim ebits As Integer = bitLength(exp, exp.Length)
			' if exponent is 65537 (0x10001), use minimum window size
			If (ebits <> 17) OrElse (exp(0) <> 65537) Then
				Do While ebits > bnExpModThreshTable(wbits)
					wbits += 1
				Loop
			End If

			' Calculate appropriate table size
			Dim tblmask As Integer = 1 << wbits

			' Allocate table for precomputed odd powers of base in Montgomery form
			Dim table As Integer()() = New Integer(tblmask - 1)(){}
			For i As Integer = 0 To tblmask - 1
				table(i) = New Integer(modLen - 1){}
			Next i

			' Compute the modular inverse
			Dim inv As Integer = -MutableBigInteger.inverseMod32([mod](modLen-1))

			' Convert base to Montgomery form
			Dim a As Integer() = leftShift(base, base.Length, modLen << 5)

			Dim q As New MutableBigInteger, a2 As New MutableBigInteger(a), b2 As New MutableBigInteger([mod])

			Dim r As MutableBigInteger= a2.divide(b2, q)
			table(0) = r.toIntArray()

			' Pad table[0] with leading zeros so its length is at least modLen
			If table(0).Length < modLen Then
			   Dim offset As Integer = modLen - table(0).Length
			   Dim t2 As Integer() = New Integer(modLen - 1){}
			   For i As Integer = 0 To table(0).Length - 1
				   t2(i+offset) = table(0)(i)
			   Next i
			   table(0) = t2
			End If

			' Set b to the square of the base
			Dim b As Integer() = squareToLen(table(0), modLen, Nothing)
			b = montReduce(b, [mod], modLen, inv)

			' Set t to high half of b
			Dim t As Integer() = java.util.Arrays.copyOf(b, modLen)

			' Fill in the table with odd powers of the base
			For i As Integer = 1 To tblmask - 1
				Dim prod As Integer() = multiplyToLen(t, modLen, table(i-1), modLen, Nothing)
				table(i) = montReduce(prod, [mod], modLen, inv)
			Next i

			' Pre load the window that slides over the exponent
			Dim bitpos As Integer = 1 << ((ebits-1) And (32-1))

			Dim buf As Integer = 0
			Dim elen As Integer = exp.Length
			Dim eIndex As Integer = 0
			For i As Integer = 0 To wbits
				buf = (buf << 1) Or (If((exp(eIndex) And bitpos) <> 0, 1, 0))
				bitpos >>>= 1
				If bitpos = 0 Then
					eIndex += 1
					bitpos = 1 << (32-1)
					elen -= 1
				End If
			Next i

			Dim multpos As Integer = ebits

			' The first iteration, which is hoisted out of the main loop
			ebits -= 1
			Dim isone As Boolean = True

			multpos = ebits - wbits
			Do While (buf And 1) = 0
				buf >>>= 1
				multpos += 1
			Loop

			Dim mult As Integer() = table(CInt(CUInt(buf) >> 1))

			buf = 0
			If multpos = ebits Then isone = False

			' The main loop
			Do
				ebits -= 1
				' Advance the window
				buf <<= 1

				If elen <> 0 Then
					buf = buf Or If((exp(eIndex) And bitpos) <> 0, 1, 0)
					bitpos >>>= 1
					If bitpos = 0 Then
						eIndex += 1
						bitpos = 1 << (32-1)
						elen -= 1
					End If
				End If

				' Examine the window for pending multiplies
				If (buf And tblmask) <> 0 Then
					multpos = ebits - wbits
					Do While (buf And 1) = 0
						buf >>>= 1
						multpos += 1
					Loop
					mult = table(CInt(CUInt(buf) >> 1))
					buf = 0
				End If

				' Perform multiply
				If ebits = multpos Then
					If isone Then
						b = mult.clone()
						isone = False
					Else
						t = b
						a = multiplyToLen(t, modLen, mult, modLen, a)
						a = montReduce(a, [mod], modLen, inv)
						t = a
						a = b
						b = t
					End If
				End If

				' Check if done
				If ebits = 0 Then Exit Do

				' Square the input
				If Not isone Then
					t = b
					a = squareToLen(t, modLen, a)
					a = montReduce(a, [mod], modLen, inv)
					t = a
					a = b
					b = t
				End If
			Loop

			' Convert result out of Montgomery form and return
			Dim t2 As Integer() = New Integer(2*modLen - 1){}
			Array.Copy(b, 0, t2, modLen, modLen)

			b = montReduce(t2, [mod], modLen, inv)

			t2 = java.util.Arrays.copyOf(b, modLen)

			Return New BigInteger(1, t2)
		End Function

		''' <summary>
		''' Montgomery reduce n, modulo mod.  This reduces modulo mod and divides
		''' by 2^(32*mlen). Adapted from Colin Plumb's C library.
		''' </summary>
		Private Shared Function montReduce(ByVal n As Integer(), ByVal [mod] As Integer(), ByVal mlen As Integer, ByVal inv As Integer) As Integer()
			Dim c As Integer=0
			Dim len As Integer = mlen
			Dim offset As Integer=0

			Do
				Dim nEnd As Integer = n(n.Length-1-offset)
				Dim carry As Integer = mulAdd(n, [mod], offset, mlen, inv * nEnd)
				c += addOne(n, offset, mlen, carry)
				offset += 1
				len -= 1
			Loop While len > 0

			Do While c > 0
				c += subN(n, [mod], mlen)
			Loop

			Do While intArrayCmpToLen(n, [mod], mlen) >= 0
				subN(n, [mod], mlen)
			Loop

			Return n
		End Function


	'    
	'     * Returns -1, 0 or +1 as big-endian unsigned int array arg1 is less than,
	'     * equal to, or greater than arg2 up to length len.
	'     
		Private Shared Function intArrayCmpToLen(ByVal arg1 As Integer(), ByVal arg2 As Integer(), ByVal len As Integer) As Integer
			For i As Integer = 0 To len - 1
				Dim b1 As Long = arg1(i) And LONG_MASK
				Dim b2 As Long = arg2(i) And LONG_MASK
				If b1 < b2 Then Return -1
				If b1 > b2 Then Return 1
			Next i
			Return 0
		End Function

		''' <summary>
		''' Subtracts two numbers of same length, returning borrow.
		''' </summary>
		Private Shared Function subN(ByVal a As Integer(), ByVal b As Integer(), ByVal len As Integer) As Integer
			Dim sum As Long = 0

			len -= 1
			Do While len >= 0
				sum = (a(len) And LONG_MASK) - (b(len) And LONG_MASK) + (sum >> 32)
				a(len) = CInt(sum)
				len -= 1
			Loop

			Return CInt(Fix(sum >> 32))
		End Function

		''' <summary>
		''' Multiply an array by one word k and add to result, return the carry
		''' </summary>
		Friend Shared Function mulAdd(ByVal out As Integer(), ByVal [in] As Integer(), ByVal offset As Integer, ByVal len As Integer, ByVal k As Integer) As Integer
			Dim kLong As Long = k And LONG_MASK
			Dim carry As Long = 0

			offset = out.Length-offset - 1
			For j As Integer = len-1 To 0 Step -1
				Dim product As Long = ([in](j) And LONG_MASK) * kLong + (out(offset) And LONG_MASK) + carry
				out(offset) = CInt(product)
				offset -= 1
				carry = CLng(CULng(product) >> 32)
			Next j
			Return CInt(carry)
		End Function

		''' <summary>
		''' Add one word to the number a mlen words into a. Return the resulting
		''' carry.
		''' </summary>
		Friend Shared Function addOne(ByVal a As Integer(), ByVal offset As Integer, ByVal mlen As Integer, ByVal carry As Integer) As Integer
			offset = a.Length-1-mlen-offset
			Dim t As Long = (a(offset) And LONG_MASK) + (carry And LONG_MASK)

			a(offset) = CInt(t)
			If (CLng(CULng(t) >> 32)) = 0 Then Return 0
			mlen -= 1
			Do While mlen >= 0
				offset -= 1
				If offset < 0 Then ' Carry out of number
					Return 1
				Else
					a(offset) += 1
					If a(offset) <> 0 Then Return 0
				End If
				mlen -= 1
			Loop
			Return 1
		End Function

		''' <summary>
		''' Returns a BigInteger whose value is (this ** exponent) mod (2**p)
		''' </summary>
		Private Function modPow2(ByVal exponent As BigInteger, ByVal p As Integer) As BigInteger
	'        
	'         * Perform exponentiation using repeated squaring trick, chopping off
	'         * high order bits as indicated by modulus.
	'         
			Dim result As BigInteger = ONE
			Dim baseToPow2 As BigInteger = Me.mod2(p)
			Dim expOffset As Integer = 0

			Dim limit As Integer = exponent.bitLength()

			If Me.testBit(0) Then limit = If((p-1) < limit, (p-1), limit)

			Do While expOffset < limit
				If exponent.testBit(expOffset) Then result = result.multiply(baseToPow2).mod2(p)
				expOffset += 1
				If expOffset < limit Then baseToPow2 = baseToPow2.square().mod2(p)
			Loop

			Return result
		End Function

		''' <summary>
		''' Returns a BigInteger whose value is this mod(2**p).
		''' Assumes that this {@code BigInteger >= 0} and {@code p > 0}.
		''' </summary>
		Private Function mod2(ByVal p As Integer) As BigInteger
			If bitLength() <= p Then Return Me

			' Copy remaining ints of mag
			Dim numInts As Integer = CInt(CUInt((p + 31)) >> 5)
			Dim mag As Integer() = New Integer(numInts - 1){}
			Array.Copy(Me.mag, (Me.mag.Length - numInts), mag, 0, numInts)

			' Mask out any excess bits
			Dim excessBits As Integer = (numInts << 5) - p
			mag(0) = mag(0) And (1L << (32-excessBits)) - 1

			Return (If(mag(0) = 0, New BigInteger(1, mag), New BigInteger(mag, 1)))
		End Function

		''' <summary>
		''' Returns a BigInteger whose value is {@code (this}<sup>-1</sup> {@code mod m)}.
		''' </summary>
		''' <param name="m"> the modulus. </param>
		''' <returns> {@code this}<sup>-1</sup> {@code mod m}. </returns>
		''' <exception cref="ArithmeticException"> {@code  m} &le; 0, or this BigInteger
		'''         has no multiplicative inverse mod m (that is, this BigInteger
		'''         is not <i>relatively prime</i> to m). </exception>
		Public Overridable Function modInverse(ByVal m As BigInteger) As BigInteger
			If m.signum_Renamed <> 1 Then Throw New ArithmeticException("BigInteger: modulus not positive")

			If m.Equals(ONE) Then Return ZERO

			' Calculate (this mod m)
			Dim modVal As BigInteger = Me
			If signum_Renamed < 0 OrElse (Me.compareMagnitude(m) >= 0) Then modVal = Me.mod(m)

			If modVal.Equals(ONE) Then Return ONE

			Dim a As New MutableBigInteger(modVal)
			Dim b As New MutableBigInteger(m)

			Dim result As MutableBigInteger = a.mutableModInverse(b)
			Return result.toBigInteger(1)
		End Function

		' Shift Operations

		''' <summary>
		''' Returns a BigInteger whose value is {@code (this << n)}.
		''' The shift distance, {@code n}, may be negative, in which case
		''' this method performs a right shift.
		''' (Computes <tt>floor(this * 2<sup>n</sup>)</tt>.)
		''' </summary>
		''' <param name="n"> shift distance, in bits. </param>
		''' <returns> {@code this << n} </returns>
		''' <seealso cref= #shiftRight </seealso>
		Public Overridable Function shiftLeft(ByVal n As Integer) As BigInteger
			If signum_Renamed = 0 Then Return ZERO
			If n > 0 Then
				Return New BigInteger(shiftLeft(mag, n), signum_Renamed)
			ElseIf n = 0 Then
				Return Me
			Else
				' Possible int overflow in (-n) is not a trouble,
				' because shiftRightImpl considers its argument unsigned
				Return shiftRightImpl(-n)
			End If
		End Function

		''' <summary>
		''' Returns a magnitude array whose value is {@code (mag << n)}.
		''' The shift distance, {@code n}, is considered unnsigned.
		''' (Computes <tt>this * 2<sup>n</sup></tt>.)
		''' </summary>
		''' <param name="mag"> magnitude, the most-significant int ({@code mag[0]}) must be non-zero. </param>
		''' <param name="n"> unsigned shift distance, in bits. </param>
		''' <returns> {@code mag << n} </returns>
		Private Shared Function shiftLeft(ByVal mag As Integer(), ByVal n As Integer) As Integer()
			Dim nInts As Integer = CInt(CUInt(n) >> 5)
			Dim nBits As Integer = n And &H1f
			Dim magLen As Integer = mag.Length
			Dim newMag As Integer() = Nothing

			If nBits = 0 Then
				newMag = New Integer(magLen + nInts - 1){}
				Array.Copy(mag, 0, newMag, 0, magLen)
			Else
				Dim i As Integer = 0
				Dim nBits2 As Integer = 32 - nBits
				Dim highBits As Integer = CInt(CUInt(mag(0)) >> nBits2)
				If highBits <> 0 Then
					newMag = New Integer(magLen + nInts){}
					newMag(i) = highBits
					i += 1
				Else
					newMag = New Integer(magLen + nInts - 1){}
				End If
				Dim j As Integer=0
				Do While j < magLen-1
						newMag(i) = mag(j) << nBits Or CInt(CUInt(mag(j)) >> nBits2)
						j += 1
					i += 1
				Loop
				newMag(i) = mag(j) << nBits
			End If
			Return newMag
		End Function

		''' <summary>
		''' Returns a BigInteger whose value is {@code (this >> n)}.  Sign
		''' extension is performed.  The shift distance, {@code n}, may be
		''' negative, in which case this method performs a left shift.
		''' (Computes <tt>floor(this / 2<sup>n</sup>)</tt>.)
		''' </summary>
		''' <param name="n"> shift distance, in bits. </param>
		''' <returns> {@code this >> n} </returns>
		''' <seealso cref= #shiftLeft </seealso>
		Public Overridable Function shiftRight(ByVal n As Integer) As BigInteger
			If signum_Renamed = 0 Then Return ZERO
			If n > 0 Then
				Return shiftRightImpl(n)
			ElseIf n = 0 Then
				Return Me
			Else
				' Possible int overflow in {@code -n} is not a trouble,
				' because shiftLeft considers its argument unsigned
				Return New BigInteger(shiftLeft(mag, -n), signum_Renamed)
			End If
		End Function

		''' <summary>
		''' Returns a BigInteger whose value is {@code (this >> n)}. The shift
		''' distance, {@code n}, is considered unsigned.
		''' (Computes <tt>floor(this * 2<sup>-n</sup>)</tt>.)
		''' </summary>
		''' <param name="n"> unsigned shift distance, in bits. </param>
		''' <returns> {@code this >> n} </returns>
		Private Function shiftRightImpl(ByVal n As Integer) As BigInteger
			Dim nInts As Integer = CInt(CUInt(n) >> 5)
			Dim nBits As Integer = n And &H1f
			Dim magLen As Integer = mag.Length
			Dim newMag As Integer() = Nothing

			' Special case: entire contents shifted off the end
			If nInts >= magLen Then Return (If(signum_Renamed >= 0, ZERO, negConst(1)))

			If nBits = 0 Then
				Dim newMagLen As Integer = magLen - nInts
				newMag = java.util.Arrays.copyOf(mag, newMagLen)
			Else
				Dim i As Integer = 0
				Dim highBits As Integer = CInt(CUInt(mag(0)) >> nBits)
				If highBits <> 0 Then
					newMag = New Integer(magLen - nInts - 1){}
					newMag(i) = highBits
					i += 1
				Else
					newMag = New Integer(magLen - nInts -2){}
				End If

				Dim nBits2 As Integer = 32 - nBits
				Dim j As Integer=0
				Do While j < magLen - nInts - 1
						newMag(i) = (mag(j) << nBits2) Or (CInt(CUInt(mag(j)) >> nBits))
						j += 1
					i += 1
				Loop
			End If

			If signum_Renamed < 0 Then
				' Find out whether any one-bits were shifted off the end.
				Dim onesLost As Boolean = False
				Dim i As Integer=magLen-1
				Dim j As Integer=magLen-nInts
				Do While i >= j AndAlso Not onesLost
					onesLost = (mag(i) <> 0)
					i -= 1
				Loop
				If (Not onesLost) AndAlso nBits <> 0 Then onesLost = (mag(magLen - nInts - 1) << (32 - nBits) <> 0)

				If onesLost Then newMag = javaIncrement(newMag)
			End If

			Return New BigInteger(newMag, signum_Renamed)
		End Function

		Friend Overridable Function javaIncrement(ByVal val As Integer()) As Integer()
			Dim lastSum As Integer = 0
			Dim i As Integer=val.Length-1
			Do While i >= 0 AndAlso lastSum = 0
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				lastSum = (val(i) += 1)
				i -= 1
			Loop
			If lastSum = 0 Then
				val = New Integer(val.Length){}
				val(0) = 1
			End If
			Return val
		End Function

		' Bitwise Operations

		''' <summary>
		''' Returns a BigInteger whose value is {@code (this & val)}.  (This
		''' method returns a negative BigInteger if and only if this and val are
		''' both negative.)
		''' </summary>
		''' <param name="val"> value to be AND'ed with this BigInteger. </param>
		''' <returns> {@code this & val} </returns>
		Public Overridable Function [and](ByVal val As BigInteger) As BigInteger
			Dim result As Integer() = New Integer(Math.Max(intLength(), val.intLength()) - 1){}
			For i As Integer = 0 To result.Length - 1
				result(i) = (getInt(result.Length-i-1) And val.getInt(result.Length-i-1))
			Next i

			Return valueOf(result)
		End Function

		''' <summary>
		''' Returns a BigInteger whose value is {@code (this | val)}.  (This method
		''' returns a negative BigInteger if and only if either this or val is
		''' negative.)
		''' </summary>
		''' <param name="val"> value to be OR'ed with this BigInteger. </param>
		''' <returns> {@code this | val} </returns>
		Public Overridable Function [or](ByVal val As BigInteger) As BigInteger
			Dim result As Integer() = New Integer(Math.Max(intLength(), val.intLength()) - 1){}
			For i As Integer = 0 To result.Length - 1
				result(i) = (getInt(result.Length-i-1) Or val.getInt(result.Length-i-1))
			Next i

			Return valueOf(result)
		End Function

		''' <summary>
		''' Returns a BigInteger whose value is {@code (this ^ val)}.  (This method
		''' returns a negative BigInteger if and only if exactly one of this and
		''' val are negative.)
		''' </summary>
		''' <param name="val"> value to be XOR'ed with this BigInteger. </param>
		''' <returns> {@code this ^ val} </returns>
		Public Overridable Function [xor](ByVal val As BigInteger) As BigInteger
			Dim result As Integer() = New Integer(Math.Max(intLength(), val.intLength()) - 1){}
			For i As Integer = 0 To result.Length - 1
				result(i) = (getInt(result.Length-i-1) Xor val.getInt(result.Length-i-1))
			Next i

			Return valueOf(result)
		End Function

		''' <summary>
		''' Returns a BigInteger whose value is {@code (~this)}.  (This method
		''' returns a negative value if and only if this BigInteger is
		''' non-negative.)
		''' </summary>
		''' <returns> {@code ~this} </returns>
		Public Overridable Function [not]() As BigInteger
			Dim result As Integer() = New Integer(intLength() - 1){}
			For i As Integer = 0 To result.Length - 1
				result(i) = Not getInt(result.Length-i-1)
			Next i

			Return valueOf(result)
		End Function

		''' <summary>
		''' Returns a BigInteger whose value is {@code (this & ~val)}.  This
		''' method, which is equivalent to {@code and(val.not())}, is provided as
		''' a convenience for masking operations.  (This method returns a negative
		''' BigInteger if and only if {@code this} is negative and {@code val} is
		''' positive.)
		''' </summary>
		''' <param name="val"> value to be complemented and AND'ed with this BigInteger. </param>
		''' <returns> {@code this & ~val} </returns>
		Public Overridable Function andNot(ByVal val As BigInteger) As BigInteger
			Dim result As Integer() = New Integer(Math.Max(intLength(), val.intLength()) - 1){}
			For i As Integer = 0 To result.Length - 1
				result(i) = (getInt(result.Length-i-1) And (Not val.getInt(result.Length-i-1)))
			Next i

			Return valueOf(result)
		End Function


		' Single Bit Operations

		''' <summary>
		''' Returns {@code true} if and only if the designated bit is set.
		''' (Computes {@code ((this & (1<<n)) != 0)}.)
		''' </summary>
		''' <param name="n"> index of bit to test. </param>
		''' <returns> {@code true} if and only if the designated bit is set. </returns>
		''' <exception cref="ArithmeticException"> {@code n} is negative. </exception>
		Public Overridable Function testBit(ByVal n As Integer) As Boolean
			If n < 0 Then Throw New ArithmeticException("Negative bit address")

			Return (getInt(CInt(CUInt(n) >> 5)) And (1 << (n And 31))) <> 0
		End Function

		''' <summary>
		''' Returns a BigInteger whose value is equivalent to this BigInteger
		''' with the designated bit set.  (Computes {@code (this | (1<<n))}.)
		''' </summary>
		''' <param name="n"> index of bit to set. </param>
		''' <returns> {@code this | (1<<n)} </returns>
		''' <exception cref="ArithmeticException"> {@code n} is negative. </exception>
		Public Overridable Function setBit(ByVal n As Integer) As BigInteger
			If n < 0 Then Throw New ArithmeticException("Negative bit address")

			Dim intNum As Integer = CInt(CUInt(n) >> 5)
			Dim result As Integer() = New Integer(Math.Max(intLength(), intNum+2) - 1){}

			For i As Integer = 0 To result.Length - 1
				result(result.Length-i-1) = getInt(i)
			Next i

			result(result.Length-intNum-1) = result(result.Length-intNum-1) Or (1 << (n And 31))

			Return valueOf(result)
		End Function

		''' <summary>
		''' Returns a BigInteger whose value is equivalent to this BigInteger
		''' with the designated bit cleared.
		''' (Computes {@code (this & ~(1<<n))}.)
		''' </summary>
		''' <param name="n"> index of bit to clear. </param>
		''' <returns> {@code this & ~(1<<n)} </returns>
		''' <exception cref="ArithmeticException"> {@code n} is negative. </exception>
		Public Overridable Function clearBit(ByVal n As Integer) As BigInteger
			If n < 0 Then Throw New ArithmeticException("Negative bit address")

			Dim intNum As Integer = CInt(CUInt(n) >> 5)
			Dim result As Integer() = New Integer(Math.Max(intLength(), (CInt(CUInt((n + 1)) >> 5)) + 1) - 1){}

			For i As Integer = 0 To result.Length - 1
				result(result.Length-i-1) = getInt(i)
			Next i

			result(result.Length-intNum-1) = result(result.Length-intNum-1) And Not(1 << (n And 31))

			Return valueOf(result)
		End Function

		''' <summary>
		''' Returns a BigInteger whose value is equivalent to this BigInteger
		''' with the designated bit flipped.
		''' (Computes {@code (this ^ (1<<n))}.)
		''' </summary>
		''' <param name="n"> index of bit to flip. </param>
		''' <returns> {@code this ^ (1<<n)} </returns>
		''' <exception cref="ArithmeticException"> {@code n} is negative. </exception>
		Public Overridable Function flipBit(ByVal n As Integer) As BigInteger
			If n < 0 Then Throw New ArithmeticException("Negative bit address")

			Dim intNum As Integer = CInt(CUInt(n) >> 5)
			Dim result As Integer() = New Integer(Math.Max(intLength(), intNum+2) - 1){}

			For i As Integer = 0 To result.Length - 1
				result(result.Length-i-1) = getInt(i)
			Next i

			result(result.Length-intNum-1) = result(result.Length-intNum-1) Xor (1 << (n And 31))

			Return valueOf(result)
		End Function

		''' <summary>
		''' Returns the index of the rightmost (lowest-order) one bit in this
		''' BigInteger (the number of zero bits to the right of the rightmost
		''' one bit).  Returns -1 if this BigInteger contains no one bits.
		''' (Computes {@code (this == 0? -1 : log2(this & -this))}.)
		''' </summary>
		''' <returns> index of the rightmost one bit in this BigInteger. </returns>
		Public Overridable Property lowestSetBit As Integer
			Get
	'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim lsb As Integer = lowestSetBit - 2
				If lsb = -2 Then ' lowestSetBit not initialized yet
					lsb = 0
					If signum_Renamed = 0 Then
						lsb -= 1
					Else
						' Search for lowest order nonzero int
						Dim i, b As Integer
						i=0
	'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						Do While (b = getInt(i)) = 0
    
							i += 1
						Loop
						lsb += (i << 5) + Integer.numberOfTrailingZeros(b)
					End If
					lowestSetBit = lsb + 2
				End If
				Return lsb
			End Get
		End Property


		' Miscellaneous Bit Operations

		''' <summary>
		''' Returns the number of bits in the minimal two's-complement
		''' representation of this BigInteger, <i>excluding</i> a sign bit.
		''' For positive BigIntegers, this is equivalent to the number of bits in
		''' the ordinary binary representation.  (Computes
		''' {@code (ceil(log2(this < 0 ? -this : this+1)))}.)
		''' </summary>
		''' <returns> number of bits in the minimal two's-complement
		'''         representation of this BigInteger, <i>excluding</i> a sign bit. </returns>
		Public Overridable Function bitLength() As Integer
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim n As Integer = bitLength_Renamed - 1
			If n = -1 Then ' bitLength not initialized yet
				Dim m As Integer() = mag
				Dim len As Integer = m.Length
				If len = 0 Then
					n = 0 ' offset by one to initialize
				Else
					' Calculate the bit length of the magnitude
					Dim magBitLength As Integer = ((len - 1) << 5) + bitLengthForInt(mag(0))
					 If signum_Renamed < 0 Then
						 ' Check if magnitude is a power of two
						 Dim pow2 As Boolean = (Integer.bitCount(mag(0)) = 1)
						 Dim i As Integer=1
						 Do While i< len AndAlso pow2
							 pow2 = (mag(i) = 0)
							 i += 1
						 Loop

						 n = (If(pow2, magBitLength -1, magBitLength))
					 Else
						 n = magBitLength
					 End If
				End If
				bitLength_Renamed = n + 1
			End If
			Return n
		End Function

		''' <summary>
		''' Returns the number of bits in the two's complement representation
		''' of this BigInteger that differ from its sign bit.  This method is
		''' useful when implementing bit-vector style sets atop BigIntegers.
		''' </summary>
		''' <returns> number of bits in the two's complement representation
		'''         of this BigInteger that differ from its sign bit. </returns>
		Public Overridable Function bitCount() As Integer
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim bc As Integer = bitCount_Renamed - 1
			If bc = -1 Then ' bitCount not initialized yet
				bc = 0 ' offset by one to initialize
				' Count the bits in the magnitude
				For i As Integer = 0 To mag.Length - 1
					bc += Integer.bitCount(mag(i))
				Next i
				If signum_Renamed < 0 Then
					' Count the trailing zeros in the magnitude
					Dim magTrailingZeroCount As Integer = 0, j As Integer
					j=mag.Length-1
					Do While mag(j) = 0
						magTrailingZeroCount += 32
						j -= 1
					Loop
					magTrailingZeroCount += Integer.numberOfTrailingZeros(mag(j))
					bc += magTrailingZeroCount - 1
				End If
				bitCount_Renamed = bc + 1
			End If
			Return bc
		End Function

		' Primality Testing

		''' <summary>
		''' Returns {@code true} if this BigInteger is probably prime,
		''' {@code false} if it's definitely composite.  If
		''' {@code certainty} is &le; 0, {@code true} is
		''' returned.
		''' </summary>
		''' <param name="certainty"> a measure of the uncertainty that the caller is
		'''         willing to tolerate: if the call returns {@code true}
		'''         the probability that this BigInteger is prime exceeds
		'''         (1 - 1/2<sup>{@code certainty}</sup>).  The execution time of
		'''         this method is proportional to the value of this parameter. </param>
		''' <returns> {@code true} if this BigInteger is probably prime,
		'''         {@code false} if it's definitely composite. </returns>
		Public Overridable Function isProbablePrime(ByVal certainty As Integer) As Boolean
			If certainty <= 0 Then Return True
			Dim w As BigInteger = Me.abs()
			If w.Equals(TWO) Then Return True
			If (Not w.testBit(0)) OrElse w.Equals(ONE) Then Return False

			Return w.primeToCertainty(certainty, Nothing)
		End Function

		' Comparison Operations

		''' <summary>
		''' Compares this BigInteger with the specified BigInteger.  This
		''' method is provided in preference to individual methods for each
		''' of the six boolean comparison operators ({@literal <}, ==,
		''' {@literal >}, {@literal >=}, !=, {@literal <=}).  The suggested
		''' idiom for performing these comparisons is: {@code
		''' (x.compareTo(y)} &lt;<i>op</i>&gt; {@code 0)}, where
		''' &lt;<i>op</i>&gt; is one of the six comparison operators.
		''' </summary>
		''' <param name="val"> BigInteger to which this BigInteger is to be compared. </param>
		''' <returns> -1, 0 or 1 as this BigInteger is numerically less than, equal
		'''         to, or greater than {@code val}. </returns>
		Public Overridable Function compareTo(ByVal val As BigInteger) As Integer Implements Comparable(Of BigInteger).compareTo
			If signum_Renamed = val.signum_Renamed Then
				Select Case signum_Renamed
				Case 1
					Return compareMagnitude(val)
				Case -1
					Return val.compareMagnitude(Me)
				Case Else
					Return 0
				End Select
			End If
			Return If(signum_Renamed > val.signum_Renamed, 1, -1)
		End Function

		''' <summary>
		''' Compares the magnitude array of this BigInteger with the specified
		''' BigInteger's. This is the version of compareTo ignoring sign.
		''' </summary>
		''' <param name="val"> BigInteger whose magnitude array to be compared. </param>
		''' <returns> -1, 0 or 1 as this magnitude array is less than, equal to or
		'''         greater than the magnitude aray for the specified BigInteger's. </returns>
		Friend Function compareMagnitude(ByVal val As BigInteger) As Integer
			Dim m1 As Integer() = mag
			Dim len1 As Integer = m1.Length
			Dim m2 As Integer() = val.mag
			Dim len2 As Integer = m2.Length
			If len1 < len2 Then Return -1
			If len1 > len2 Then Return 1
			For i As Integer = 0 To len1 - 1
				Dim a As Integer = m1(i)
				Dim b As Integer = m2(i)
				If a <> b Then Return If((a And LONG_MASK) < (b And LONG_MASK), -1, 1)
			Next i
			Return 0
		End Function

		''' <summary>
		''' Version of compareMagnitude that compares magnitude with long value.
		''' val can't be Long.MIN_VALUE.
		''' </summary>
		Friend Function compareMagnitude(ByVal val As Long) As Integer
			Debug.Assert(val <> Long.MinValue)
			Dim m1 As Integer() = mag
			Dim len As Integer = m1.Length
			If len > 2 Then Return 1
			If val < 0 Then val = -val
			Dim highWord As Integer = CInt(CLng(CULng(val) >> 32))
			If highWord = 0 Then
				If len < 1 Then Return -1
				If len > 1 Then Return 1
				Dim a As Integer = m1(0)
				Dim b As Integer = CInt(val)
				If a <> b Then Return If((a And LONG_MASK) < (b And LONG_MASK), -1, 1)
				Return 0
			Else
				If len < 2 Then Return -1
				Dim a As Integer = m1(0)
				Dim b As Integer = highWord
				If a <> b Then Return If((a And LONG_MASK) < (b And LONG_MASK), -1, 1)
				a = m1(1)
				b = CInt(val)
				If a <> b Then Return If((a And LONG_MASK) < (b And LONG_MASK), -1, 1)
				Return 0
			End If
		End Function

		''' <summary>
		''' Compares this BigInteger with the specified Object for equality.
		''' </summary>
		''' <param name="x"> Object to which this BigInteger is to be compared. </param>
		''' <returns> {@code true} if and only if the specified Object is a
		'''         BigInteger whose value is numerically equal to this BigInteger. </returns>
		Public Overrides Function Equals(ByVal x As Object) As Boolean
			' This test is just an optimization, which may or may not help
			If x Is Me Then Return True

			If Not(TypeOf x Is BigInteger) Then Return False

			Dim xInt As BigInteger = CType(x, BigInteger)
			If xInt.signum_Renamed <> signum_Renamed Then Return False

			Dim m As Integer() = mag
			Dim len As Integer = m.Length
			Dim xm As Integer() = xInt.mag
			If len <> xm.Length Then Return False

			For i As Integer = 0 To len - 1
				If xm(i) <> m(i) Then Return False
			Next i

			Return True
		End Function

		''' <summary>
		''' Returns the minimum of this BigInteger and {@code val}.
		''' </summary>
		''' <param name="val"> value with which the minimum is to be computed. </param>
		''' <returns> the BigInteger whose value is the lesser of this BigInteger and
		'''         {@code val}.  If they are equal, either may be returned. </returns>
		Public Overridable Function min(ByVal val As BigInteger) As BigInteger
			Return (If(compareTo(val) < 0, Me, val))
		End Function

		''' <summary>
		''' Returns the maximum of this BigInteger and {@code val}.
		''' </summary>
		''' <param name="val"> value with which the maximum is to be computed. </param>
		''' <returns> the BigInteger whose value is the greater of this and
		'''         {@code val}.  If they are equal, either may be returned. </returns>
		Public Overridable Function max(ByVal val As BigInteger) As BigInteger
			Return (If(compareTo(val) > 0, Me, val))
		End Function


		' Hash Function

		''' <summary>
		''' Returns the hash code for this BigInteger.
		''' </summary>
		''' <returns> hash code for this BigInteger. </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim hashCode As Integer = 0

			For i As Integer = 0 To mag.Length - 1
				hashCode = CInt(31*hashCode + (mag(i) And LONG_MASK))
			Next i

			Return hashCode * signum_Renamed
		End Function

		''' <summary>
		''' Returns the String representation of this BigInteger in the
		''' given radix.  If the radix is outside the range from {@link
		''' Character#MIN_RADIX} to <seealso cref="Character#MAX_RADIX"/> inclusive,
		''' it will default to 10 (as is the case for
		''' {@code Integer.toString}).  The digit-to-character mapping
		''' provided by {@code Character.forDigit} is used, and a minus
		''' sign is prepended if appropriate.  (This representation is
		''' compatible with the {@link #BigInteger(String, int) (String,
		''' int)} constructor.)
		''' </summary>
		''' <param name="radix">  radix of the String representation. </param>
		''' <returns> String representation of this BigInteger in the given radix. </returns>
		''' <seealso cref=    Integer#toString </seealso>
		''' <seealso cref=    Character#forDigit </seealso>
		''' <seealso cref=    #BigInteger(java.lang.String, int) </seealso>
		Public Overrides Function ToString(ByVal radix As Integer) As String
			If signum_Renamed = 0 Then Return "0"
			If radix < Character.MIN_RADIX OrElse radix > Character.MAX_RADIX Then radix = 10

			' If it's small enough, use smallToString.
			If mag.Length <= SCHOENHAGE_BASE_CONVERSION_THRESHOLD Then Return smallToString(radix)

			' Otherwise use recursive toString, which requires positive arguments.
			' The results will be concatenated into this StringBuilder
			Dim sb As New StringBuilder
			If signum_Renamed < 0 Then
				ToString(Me.negate(), sb, radix, 0)
				sb.insert(0, "-"c)
			Else
				ToString(Me, sb, radix, 0)
			End If

			Return sb.ToString()
		End Function

		''' <summary>
		''' This method is used to perform toString when arguments are small. </summary>
		Private Function smallToString(ByVal radix As Integer) As String
			If signum_Renamed = 0 Then Return "0"

			' Compute upper bound on number of digit groups and allocate space
			Dim maxNumDigitGroups As Integer = (4*mag.Length + 6)\7
			Dim digitGroup As String() = New String(maxNumDigitGroups - 1){}

			' Translate number to string, a digit group at a time
			Dim tmp As BigInteger = Me.abs()
			Dim numGroups As Integer = 0
			Do While tmp.signum_Renamed <> 0
				Dim d As BigInteger = longRadix(radix)

				Dim q As New MutableBigInteger, a As New MutableBigInteger(tmp.mag), b As New MutableBigInteger(d.mag)
				Dim r As MutableBigInteger = a.divide(b, q)
				Dim q2 As BigInteger = q.toBigInteger(tmp.signum_Renamed * d.signum_Renamed)
				Dim r2 As BigInteger = r.toBigInteger(tmp.signum_Renamed * d.signum_Renamed)

				digitGroup(numGroups) = Convert.ToString(r2, radix)
				numGroups += 1
				tmp = q2
			Loop

			' Put sign (if any) and first digit group into result buffer
			Dim buf As New StringBuilder(numGroups*digitsPerLong(radix)+1)
			If signum_Renamed < 0 Then buf.append("-"c)
			buf.append(digitGroup(numGroups-1))

			' Append remaining digit groups padded with leading zeros
			For i As Integer = numGroups-2 To 0 Step -1
				' Prepend (any) leading zeros for this digit group
				Dim numLeadingZeros As Integer = digitsPerLong(radix)-digitGroup(i).length()
				If numLeadingZeros <> 0 Then buf.append(zeros(numLeadingZeros))
				buf.append(digitGroup(i))
			Next i
			Return buf.ToString()
		End Function

		''' <summary>
		''' Converts the specified BigInteger to a string and appends to
		''' {@code sb}.  This implements the recursive Schoenhage algorithm
		''' for base conversions.
		''' <p/>
		''' See Knuth, Donald,  _The Art of Computer Programming_, Vol. 2,
		''' Answers to Exercises (4.4) Question 14.
		''' </summary>
		''' <param name="u">      The number to convert to a string. </param>
		''' <param name="sb">     The StringBuilder that will be appended to in place. </param>
		''' <param name="radix">  The base to convert to. </param>
		''' <param name="digits"> The minimum number of digits to pad to. </param>
		Private Shared Sub toString(ByVal u As BigInteger, ByVal sb As StringBuilder, ByVal radix As Integer, ByVal digits As Integer)
	'         If we're smaller than a certain threshold, use the smallToString
	'           method, padding with leading zeroes when necessary. 
			If u.mag.Length <= SCHOENHAGE_BASE_CONVERSION_THRESHOLD Then
				Dim s As String = u.smallToString(radix)

				' Pad with internal zeros if necessary.
				' Don't pad if we're at the beginning of the string.
				If (s.length() < digits) AndAlso (sb.length() > 0) Then
					For i As Integer = s.length() To digits - 1 ' May be a faster way to
						sb.append("0"c) ' do this?
					Next i
				End If

				sb.append(s)
				Return
			End If

			Dim b, n As Integer
			b = u.bitLength()

			' Calculate a value for n in the equation radix^(2^n) = u
			' and subtract 1 from that value.  This is used to find the
			' cache index that contains the best value to divide u.
			n = CInt(Fix(Math.Round(Math.Log(b * LOG_TWO / logCache(radix)) / LOG_TWO - 1.0)))
			Dim v As BigInteger = getRadixConversionCache(radix, n)
			Dim results As BigInteger()
			results = u.divideAndRemainder(v)

			Dim expectedDigits As Integer = 1 << n

			' Now recursively build the two halves of each number.
			ToString(results(0), sb, radix, digits-expectedDigits)
			ToString(results(1), sb, radix, expectedDigits)
		End Sub

		''' <summary>
		''' Returns the value radix^(2^exponent) from the cache.
		''' If this value doesn't already exist in the cache, it is added.
		''' <p/>
		''' This could be changed to a more complicated caching method using
		''' {@code Future}.
		''' </summary>
		Private Shared Function getRadixConversionCache(ByVal radix As Integer, ByVal exponent As Integer) As BigInteger
			Dim cacheLine As BigInteger() = powerCache(radix) ' volatile read
			If exponent < cacheLine.Length Then Return cacheLine(exponent)

			Dim oldLength As Integer = cacheLine.Length
			cacheLine = java.util.Arrays.copyOf(cacheLine, exponent + 1)
			For i As Integer = oldLength To exponent
				cacheLine(i) = cacheLine(i - 1).pow(2)
			Next i

			Dim pc As BigInteger()() = powerCache ' volatile read again
			If exponent >= pc(radix).Length Then
				pc = pc.clone()
				pc(radix) = cacheLine
				powerCache = pc ' volatile write, publish
			End If
			Return cacheLine(exponent)
		End Function

		' zero[i] is a string of i consecutive zeros. 
		Private Shared zeros As String() = New String(63){}

		''' <summary>
		''' Returns the decimal String representation of this BigInteger.
		''' The digit-to-character mapping provided by
		''' {@code Character.forDigit} is used, and a minus sign is
		''' prepended if appropriate.  (This representation is compatible
		''' with the <seealso cref="#BigInteger(String) (String)"/> constructor, and
		''' allows for String concatenation with Java's + operator.)
		''' </summary>
		''' <returns> decimal String representation of this BigInteger. </returns>
		''' <seealso cref=    Character#forDigit </seealso>
		''' <seealso cref=    #BigInteger(java.lang.String) </seealso>
		Public Overrides Function ToString() As String
			Return ToString(10)
		End Function

		''' <summary>
		''' Returns a byte array containing the two's-complement
		''' representation of this BigInteger.  The byte array will be in
		''' <i>big-endian</i> byte-order: the most significant byte is in
		''' the zeroth element.  The array will contain the minimum number
		''' of bytes required to represent this BigInteger, including at
		''' least one sign bit, which is {@code (ceil((this.bitLength() +
		''' 1)/8))}.  (This representation is compatible with the
		''' <seealso cref="#BigInteger(byte[]) (byte[])"/> constructor.)
		''' </summary>
		''' <returns> a byte array containing the two's-complement representation of
		'''         this BigInteger. </returns>
		''' <seealso cref=    #BigInteger(byte[]) </seealso>
		Public Overridable Function toByteArray() As SByte()
			Dim byteLen As Integer = bitLength()\8 + 1
			Dim byteArray As SByte() = New SByte(byteLen - 1){}

			Dim i As Integer=byteLen-1
			Dim bytesCopied As Integer=4
			Dim nextInt As Integer=0
			Dim intIndex As Integer=0
			Do While i >= 0
				If bytesCopied = 4 Then
					nextInt = getInt(intIndex)
					intIndex += 1
					bytesCopied = 1
				Else
					nextInt >>>= 8
					bytesCopied += 1
				End If
				byteArray(i) = CByte(nextInt)
				i -= 1
			Loop
			Return byteArray
		End Function

		''' <summary>
		''' Converts this BigInteger to an {@code int}.  This
		''' conversion is analogous to a
		''' <i>narrowing primitive conversion</i> from {@code long} to
		''' {@code int} as defined in section 5.1.3 of
		''' <cite>The Java&trade; Language Specification</cite>:
		''' if this BigInteger is too big to fit in an
		''' {@code int}, only the low-order 32 bits are returned.
		''' Note that this conversion can lose information about the
		''' overall magnitude of the BigInteger value as well as return a
		''' result with the opposite sign.
		''' </summary>
		''' <returns> this BigInteger converted to an {@code int}. </returns>
		''' <seealso cref= #intValueExact() </seealso>
		Public Overrides Function intValue() As Integer
			Dim result As Integer = 0
			result = getInt(0)
			Return result
		End Function

		''' <summary>
		''' Converts this BigInteger to a {@code long}.  This
		''' conversion is analogous to a
		''' <i>narrowing primitive conversion</i> from {@code long} to
		''' {@code int} as defined in section 5.1.3 of
		''' <cite>The Java&trade; Language Specification</cite>:
		''' if this BigInteger is too big to fit in a
		''' {@code long}, only the low-order 64 bits are returned.
		''' Note that this conversion can lose information about the
		''' overall magnitude of the BigInteger value as well as return a
		''' result with the opposite sign.
		''' </summary>
		''' <returns> this BigInteger converted to a {@code long}. </returns>
		''' <seealso cref= #longValueExact() </seealso>
		Public Overrides Function longValue() As Long
			Dim result As Long = 0

			For i As Integer = 1 To 0 Step -1
				result = (result << 32) + (getInt(i) And LONG_MASK)
			Next i
			Return result
		End Function

		''' <summary>
		''' Converts this BigInteger to a {@code float}.  This
		''' conversion is similar to the
		''' <i>narrowing primitive conversion</i> from {@code double} to
		''' {@code float} as defined in section 5.1.3 of
		''' <cite>The Java&trade; Language Specification</cite>:
		''' if this BigInteger has too great a magnitude
		''' to represent as a {@code float}, it will be converted to
		''' <seealso cref="Float#NEGATIVE_INFINITY"/> or {@link
		''' Float#POSITIVE_INFINITY} as appropriate.  Note that even when
		''' the return value is finite, this conversion can lose
		''' information about the precision of the BigInteger value.
		''' </summary>
		''' <returns> this BigInteger converted to a {@code float}. </returns>
		Public Overrides Function floatValue() As Single
			If signum_Renamed = 0 Then Return 0.0f

			Dim exponent As Integer = ((mag.Length - 1) << 5) + bitLengthForInt(mag(0)) - 1

			' exponent == floor(log2(abs(this)))
			If exponent < Long.SIZE - 1 Then
				Return longValue()
			ElseIf exponent > Float.MAX_EXPONENT Then
				Return If(signum_Renamed > 0, Float.PositiveInfinity, Float.NegativeInfinity)
			End If

	'        
	'         * We need the top SIGNIFICAND_WIDTH bits, including the "implicit"
	'         * one bit. To make rounding easier, we pick out the top
	'         * SIGNIFICAND_WIDTH + 1 bits, so we have one to help us round up or
	'         * down. twiceSignifFloor will contain the top SIGNIFICAND_WIDTH + 1
	'         * bits, and signifFloor the top SIGNIFICAND_WIDTH.
	'         *
	'         * It helps to consider the real number signif = abs(this) *
	'         * 2^(SIGNIFICAND_WIDTH - 1 - exponent).
	'         
			Dim shift As Integer = exponent - sun.misc.FloatConsts.SIGNIFICAND_WIDTH

			Dim twiceSignifFloor As Integer
			' twiceSignifFloor will be == abs().shiftRight(shift).intValue()
			' We do the shift into an int directly to improve performance.

			Dim nBits As Integer = shift And &H1f
			Dim nBits2 As Integer = 32 - nBits

			If nBits = 0 Then
				twiceSignifFloor = mag(0)
			Else
				twiceSignifFloor = CInt(CUInt(mag(0)) >> nBits)
				If twiceSignifFloor = 0 Then twiceSignifFloor = (mag(0) << nBits2) Or (CInt(CUInt(mag(1)) >> nBits))
			End If

			Dim signifFloor As Integer = twiceSignifFloor >> 1
			signifFloor = signifFloor And sun.misc.FloatConsts.SIGNIF_BIT_MASK ' remove the implied bit

	'        
	'         * We round up if either the fractional part of signif is strictly
	'         * greater than 0.5 (which is true if the 0.5 bit is set and any lower
	'         * bit is set), or if the fractional part of signif is >= 0.5 and
	'         * signifFloor is odd (which is true if both the 0.5 bit and the 1 bit
	'         * are set). This is equivalent to the desired HALF_EVEN rounding.
	'         
			Dim increment As Boolean = (twiceSignifFloor And 1) <> 0 AndAlso ((signifFloor And 1) <> 0 OrElse abs().lowestSetBit < shift)
			Dim signifRounded As Integer = If(increment, signifFloor + 1, signifFloor)
			Dim bits As Integer = ((exponent + sun.misc.FloatConsts.EXP_BIAS)) << (sun.misc.FloatConsts.SIGNIFICAND_WIDTH - 1)
			bits += signifRounded
	'        
	'         * If signifRounded == 2^24, we'd need to set all of the significand
	'         * bits to zero and add 1 to the exponent. This is exactly the behavior
	'         * we get from just adding signifRounded to bits directly. If the
	'         * exponent is Float.MAX_EXPONENT, we round up (correctly) to
	'         * Float.POSITIVE_INFINITY.
	'         
			bits = bits Or signum_Renamed And sun.misc.FloatConsts.SIGN_BIT_MASK
			Return Float.intBitsToFloat(bits)
		End Function

		''' <summary>
		''' Converts this BigInteger to a {@code double}.  This
		''' conversion is similar to the
		''' <i>narrowing primitive conversion</i> from {@code double} to
		''' {@code float} as defined in section 5.1.3 of
		''' <cite>The Java&trade; Language Specification</cite>:
		''' if this BigInteger has too great a magnitude
		''' to represent as a {@code double}, it will be converted to
		''' <seealso cref="Double#NEGATIVE_INFINITY"/> or {@link
		''' Double#POSITIVE_INFINITY} as appropriate.  Note that even when
		''' the return value is finite, this conversion can lose
		''' information about the precision of the BigInteger value.
		''' </summary>
		''' <returns> this BigInteger converted to a {@code double}. </returns>
		Public Overrides Function doubleValue() As Double
			If signum_Renamed = 0 Then Return 0.0

			Dim exponent As Integer = ((mag.Length - 1) << 5) + bitLengthForInt(mag(0)) - 1

			' exponent == floor(log2(abs(this))Double)
			If exponent < Long.SIZE - 1 Then
				Return longValue()
			ElseIf exponent > Double.MAX_EXPONENT Then
				Return If(signum_Renamed > 0, Double.PositiveInfinity, Double.NegativeInfinity)
			End If

	'        
	'         * We need the top SIGNIFICAND_WIDTH bits, including the "implicit"
	'         * one bit. To make rounding easier, we pick out the top
	'         * SIGNIFICAND_WIDTH + 1 bits, so we have one to help us round up or
	'         * down. twiceSignifFloor will contain the top SIGNIFICAND_WIDTH + 1
	'         * bits, and signifFloor the top SIGNIFICAND_WIDTH.
	'         *
	'         * It helps to consider the real number signif = abs(this) *
	'         * 2^(SIGNIFICAND_WIDTH - 1 - exponent).
	'         
			Dim shift As Integer = exponent - sun.misc.DoubleConsts.SIGNIFICAND_WIDTH

			Dim twiceSignifFloor As Long
			' twiceSignifFloor will be == abs().shiftRight(shift).longValue()
			' We do the shift into a long directly to improve performance.

			Dim nBits As Integer = shift And &H1f
			Dim nBits2 As Integer = 32 - nBits

			Dim highBits As Integer
			Dim lowBits As Integer
			If nBits = 0 Then
				highBits = mag(0)
				lowBits = mag(1)
			Else
				highBits = CInt(CUInt(mag(0)) >> nBits)
				lowBits = (mag(0) << nBits2) Or (CInt(CUInt(mag(1)) >> nBits))
				If highBits = 0 Then
					highBits = lowBits
					lowBits = (mag(1) << nBits2) Or (CInt(CUInt(mag(2)) >> nBits))
				End If
			End If

			twiceSignifFloor = ((highBits And LONG_MASK) << 32) Or (lowBits And LONG_MASK)

			Dim signifFloor As Long = twiceSignifFloor >> 1
			signifFloor = signifFloor And sun.misc.DoubleConsts.SIGNIF_BIT_MASK ' remove the implied bit

	'        
	'         * We round up if either the fractional part of signif is strictly
	'         * greater than 0.5 (which is true if the 0.5 bit is set and any lower
	'         * bit is set), or if the fractional part of signif is >= 0.5 and
	'         * signifFloor is odd (which is true if both the 0.5 bit and the 1 bit
	'         * are set). This is equivalent to the desired HALF_EVEN rounding.
	'         
			Dim increment As Boolean = (twiceSignifFloor And 1) <> 0 AndAlso ((signifFloor And 1) <> 0 OrElse abs().lowestSetBit < shift)
			Dim signifRounded As Long = If(increment, signifFloor + 1, signifFloor)
			Dim bits As Long = CLng(Fix((exponent + sun.misc.DoubleConsts.EXP_BIAS))) << (sun.misc.DoubleConsts.SIGNIFICAND_WIDTH - 1)
			bits += signifRounded
	'        
	'         * If signifRounded == 2^53, we'd need to set all of the significand
	'         * bits to zero and add 1 to the exponent. This is exactly the behavior
	'         * we get from just adding signifRounded to bits directly. If the
	'         * exponent is Double.MAX_EXPONENT, we round up (correctly) to
	'         * Double.POSITIVE_INFINITY.
	'         
			bits = bits Or signum_Renamed And sun.misc.DoubleConsts.SIGN_BIT_MASK
			Return Double.longBitsToDouble(bits)
		End Function

		''' <summary>
		''' Returns a copy of the input array stripped of any leading zero bytes.
		''' </summary>
		Private Shared Function stripLeadingZeroInts(ByVal val As Integer()) As Integer()
			Dim vlen As Integer = val.Length
			Dim keep As Integer

			' Find first nonzero byte
			keep = 0
			Do While keep < vlen AndAlso val(keep) = 0

				keep += 1
			Loop
			Return java.util.Arrays.copyOfRange(val, keep, vlen)
		End Function

		''' <summary>
		''' Returns the input array stripped of any leading zero bytes.
		''' Since the source is trusted the copying may be skipped.
		''' </summary>
		Private Shared Function trustedStripLeadingZeroInts(ByVal val As Integer()) As Integer()
			Dim vlen As Integer = val.Length
			Dim keep As Integer

			' Find first nonzero byte
			keep = 0
			Do While keep < vlen AndAlso val(keep) = 0

				keep += 1
			Loop
			Return If(keep = 0, val, java.util.Arrays.copyOfRange(val, keep, vlen))
		End Function

		''' <summary>
		''' Returns a copy of the input array stripped of any leading zero bytes.
		''' </summary>
		Private Shared Function stripLeadingZeroBytes(ByVal a As SByte()) As Integer()
			Dim byteLength As Integer = a.Length
			Dim keep As Integer

			' Find first nonzero byte
			keep = 0
			Do While keep < byteLength AndAlso a(keep) = 0

				keep += 1
			Loop

			' Allocate new array and copy relevant part of input array
			Dim intLength As Integer = CInt(CUInt(((byteLength - keep) + 3)) >> 2)
			Dim result As Integer() = New Integer(intLength - 1){}
			Dim b As Integer = byteLength - 1
			For i As Integer = intLength-1 To 0 Step -1
				result(i) = a(b) And &Hff
				b -= 1
				Dim bytesRemaining As Integer = b - keep + 1
				Dim bytesToTransfer As Integer = Math.Min(3, bytesRemaining)
				For j As Integer = 8 To (bytesToTransfer << 3) Step 8
					result(i) = result(i) Or ((a(b) And &Hff) << j)
					b -= 1
				Next j
			Next i
			Return result
		End Function

		''' <summary>
		''' Takes an array a representing a negative 2's-complement number and
		''' returns the minimal (no leading zero bytes) unsigned whose value is -a.
		''' </summary>
		Private Shared Function makePositive(ByVal a As SByte()) As Integer()
			Dim keep, k As Integer
			Dim byteLength As Integer = a.Length

			' Find first non-sign (0xff) byte of input
			keep=0
			Do While keep < byteLength AndAlso a(keep) = -1

				keep += 1
			Loop


	'         Allocate output array.  If all non-sign bytes are 0x00, we must
	'         * allocate space for one extra output byte. 
			k=keep
			Do While k < byteLength AndAlso a(k) = 0

				k += 1
			Loop

			Dim extraByte As Integer = If(k = byteLength, 1, 0)
			Dim intLength As Integer = CInt(CUInt(((byteLength - keep + extraByte) + 3)) >> 2)
			Dim result As Integer() = New Integer(intLength - 1){}

	'         Copy one's complement of input into output, leaving extra
	'         * byte (if it exists) == 0x00 
			Dim b As Integer = byteLength - 1
			For i As Integer = intLength-1 To 0 Step -1
				result(i) = a(b) And &Hff
				b -= 1
				Dim numBytesToTransfer As Integer = Math.Min(3, b-keep+1)
				If numBytesToTransfer < 0 Then numBytesToTransfer = 0
				For j As Integer = 8 To 8*numBytesToTransfer Step 8
					result(i) = result(i) Or ((a(b) And &Hff) << j)
					b -= 1
				Next j

				' Mask indicates which bits must be complemented
				Dim mask As Integer = CInt(CUInt(-1) >> (8*(3-numBytesToTransfer)))
				result(i) = (Not result(i)) And mask
			Next i

			' Add one to one's complement to generate two's complement
			For i As Integer = result.Length-1 To 0 Step -1
				result(i) = CInt((result(i) And LONG_MASK) + 1)
				If result(i) <> 0 Then Exit For
			Next i

			Return result
		End Function

		''' <summary>
		''' Takes an array a representing a negative 2's-complement number and
		''' returns the minimal (no leading zero ints) unsigned whose value is -a.
		''' </summary>
		Private Shared Function makePositive(ByVal a As Integer()) As Integer()
			Dim keep, j As Integer

			' Find first non-sign (0xffffffff) int of input
			keep=0
			Do While keep < a.Length AndAlso a(keep) = -1

				keep += 1
			Loop

	'         Allocate output array.  If all non-sign ints are 0x00, we must
	'         * allocate space for one extra output int. 
			j=keep
			Do While j < a.Length AndAlso a(j) = 0

				j += 1
			Loop
			Dim extraInt As Integer = (If(j = a.Length, 1, 0))
			Dim result As Integer() = New Integer(a.Length - keep + extraInt - 1){}

	'         Copy one's complement of input into output, leaving extra
	'         * int (if it exists) == 0x00 
			For i As Integer = keep To a.Length - 1
				result(i - keep + extraInt) = Not a(i)
			Next i

			' Add one to one's complement to generate two's complement
			Dim i As Integer=result.Length-1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While result(i) += 1 = 0

				i -= 1
			Loop

			Return result
		End Function

	'    
	'     * The following two arrays are used for fast String conversions.  Both
	'     * are indexed by radix.  The first is the number of digits of the given
	'     * radix that can fit in a Java long without "going negative", i.e., the
	'     * highest integer n such that radix**n < 2**63.  The second is the
	'     * "long radix" that tears each number into "long digits", each of which
	'     * consists of the number of digits in the corresponding element in
	'     * digitsPerLong (longRadix[i] = i**digitPerLong[i]).  Both arrays have
	'     * nonsense values in their 0 and 1 elements, as radixes 0 and 1 are not
	'     * used.
	'     
		Private Shared digitsPerLong As Integer() = {0, 0, 62, 39, 31, 27, 24, 22, 20, 19, 18, 18, 17, 17, 16, 16, 15, 15, 15, 14, 14, 14, 14, 13, 13, 13, 13, 13, 13, 12, 12, 12, 12, 12, 12, 12, 12}

		Private Shared longRadix As BigInteger() = {Nothing, Nothing, valueOf(&H4000000000000000L), valueOf(&H383d9170b85ff80bL), valueOf(&H4000000000000000L), valueOf(&H6765c793fa10079RL), valueOf(&H41c21cb8e1000000L), valueOf(&H3642798750226111L), valueOf(&H1000000000000000L), valueOf(&H12bf307ae81ffd59L), valueOf(&Hde0b6b3a7640000L), valueOf(&H4d28cb56c33fa539L), valueOf(&H1eca170c00000000L), valueOf(&H780c7372621bd74RL), valueOf(&H1e39a5057R810000L), valueOf(&H5b27ac993Rf97701L), valueOf(&H1000000000000000L), valueOf(&H27b95e997e21R9f1L), valueOf(&H5da0e1e53c5c8000L), valueOf(&Hb16a458ef403f19L), valueOf(&H16bcc41e90000000L), valueOf(&H2d04b7fdd9c0ef49L), valueOf(&H5658597bcaa24000L), valueOf(&H6feb266931a75b7L), valueOf(&Hc29e98000000000L), valueOf(&H14adf4b7320334b9L), valueOf(&H226ed36478bfa000L), valueOf(&H383d9170b85ff80bL), valueOf(&H5a3c23e39c000000L), valueOf(&H4e900abb53e6b71L), valueOf(&H7600ec618141000L), valueOf(&Haee5720ee830681L), valueOf(&H1000000000000000L), valueOf(&H172588ad4f5f0981L), valueOf(&H211e44f7R02c1000L), valueOf(&H2ee56725f06e5c71L), valueOf(&H41c21cb8e1000000L)}

	'    
	'     * These two arrays are the integer analogue of above.
	'     
		Private Shared digitsPerInt As Integer() = {0, 0, 30, 19, 15, 13, 11, 11, 10, 9, 9, 8, 8, 8, 8, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 5}

		Private Shared intRadix As Integer() = {0, 0, &H40000000, &H4546b3Rb, &H40000000, &H48c27395, &H159fd800, &H75db9c97, &H40000000, &H17179149, &H3b9aca00, &Hcc6Rb61, &H19a10000, &H309f1021, &H57f6c100, &Ha2f1b6f, &H10000000, &H18754571, &H247dbc80, &H3547667b, &H4c4b4000, &H6b5a6e1R, &H6c20a40, &H8d2R931, &Hb640000, &He8d4a51, &H1269ae40, &H17179149, &H1cb91000, &H23744899, &H2b73a840, &H34e63b41, &H40000000, &H4cfa3cc1, &H5c13R840, &H6d91b519, &H39aa400 }

		''' <summary>
		''' These routines provide access to the two's complement representation
		''' of BigIntegers.
		''' </summary>

		''' <summary>
		''' Returns the length of the two's complement representation in ints,
		''' including space for at least one sign bit.
		''' </summary>
		Private Function intLength() As Integer
			Return (CInt(CUInt(bitLength()) >> 5)) + 1
		End Function

		' Returns sign bit 
		Private Function signBit() As Integer
			Return If(signum_Renamed < 0, 1, 0)
		End Function

		' Returns an int of sign bits 
		Private Function signInt() As Integer
			Return If(signum_Renamed < 0, -1, 0)
		End Function

		''' <summary>
		''' Returns the specified int of the little-endian two's complement
		''' representation (int 0 is the least significant).  The int number can
		''' be arbitrarily high (values are logically preceded by infinitely many
		''' sign ints).
		''' </summary>
		Private Function getInt(ByVal n As Integer) As Integer
			If n < 0 Then Return 0
			If n >= mag.Length Then Return signInt()

			Dim magInt As Integer = mag(mag.Length-n-1)

			Return (If(signum_Renamed >= 0, magInt, (If(n <= firstNonzeroIntNum(), -magInt, (Not magInt)))))
		End Function

		''' <summary>
		''' Returns the index of the int that contains the first nonzero int in the
		''' little-endian binary representation of the magnitude (int 0 is the
		''' least significant). If the magnitude is zero, return value is undefined.
		''' </summary>
		Private Function firstNonzeroIntNum() As Integer
			Dim fn As Integer = firstNonzeroIntNum_Renamed - 2
			If fn = -2 Then ' firstNonzeroIntNum not initialized yet
				fn = 0

				' Search for the first nonzero int
				Dim i As Integer
				Dim mlen As Integer = mag.Length
				i = mlen - 1
				Do While i >= 0 AndAlso mag(i) = 0

					i -= 1
				Loop
				fn = mlen - i - 1
				firstNonzeroIntNum_Renamed = fn + 2 ' offset by two to initialize
			End If
			Return fn
		End Function

		''' <summary>
		''' use serialVersionUID from JDK 1.1. for interoperability </summary>
		Private Const serialVersionUID As Long = -8287574255936472291L

		''' <summary>
		''' Serializable fields for BigInteger.
		''' 
		''' @serialField signum  int
		'''              signum of this BigInteger.
		''' @serialField magnitude int[]
		'''              magnitude array of this BigInteger.
		''' @serialField bitCount  int
		'''              number of bits in this BigInteger
		''' @serialField bitLength int
		'''              the number of bits in the minimal two's-complement
		'''              representation of this BigInteger
		''' @serialField lowestSetBit int
		'''              lowest set bit in the twos complement representation
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("signum", Integer.TYPE), New java.io.ObjectStreamField("magnitude", GetType(SByte())), New java.io.ObjectStreamField("bitCount", Integer.TYPE), New java.io.ObjectStreamField("bitLength", Integer.TYPE), New java.io.ObjectStreamField("firstNonzeroByteNum", Integer.TYPE), New java.io.ObjectStreamField("lowestSetBit", Integer.TYPE) }

		''' <summary>
		''' Reconstitute the {@code BigInteger} instance from a stream (that is,
		''' deserialize it). The magnitude is read in as an array of bytes
		''' for historical reasons, but it is converted to an array of ints
		''' and the byte array is discarded.
		''' Note:
		''' The current convention is to initialize the cache fields, bitCount,
		''' bitLength and lowestSetBit, to 0 rather than some other marker value.
		''' Therefore, no explicit action to set these fields needs to be taken in
		''' readObject because those fields already have a 0 value be default since
		''' defaultReadObject is not being used.
		''' </summary>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
	'        
	'         * In order to maintain compatibility with previous serialized forms,
	'         * the magnitude of a BigInteger is serialized as an array of bytes.
	'         * The magnitude field is used as a temporary store for the byte array
	'         * that is deserialized. The cached computation fields should be
	'         * transient but are serialized for compatibility reasons.
	'         

			' prepare to read the alternate persistent fields
			Dim fields As java.io.ObjectInputStream.GetField = s.readFields()

			' Read the alternate persistent fields that we care about
			Dim sign As Integer = fields.get("signum", -2)
			Dim magnitude As SByte() = CType(fields.get("magnitude", Nothing), SByte())

			' Validate signum
			If sign < -1 OrElse sign > 1 Then
				Dim message As String = "BigInteger: Invalid signum value"
				If fields.defaulted("signum") Then message = "BigInteger: Signum not present in stream"
				Throw New java.io.StreamCorruptedException(message)
			End If
			Dim mag As Integer() = stripLeadingZeroBytes(magnitude)
			If (mag.Length = 0) <> (sign = 0) Then
				Dim message As String = "BigInteger: signum-magnitude mismatch"
				If fields.defaulted("magnitude") Then message = "BigInteger: Magnitude not present in stream"
				Throw New java.io.StreamCorruptedException(message)
			End If

			' Commit final fields via Unsafe
			UnsafeHolder.putSign(Me, sign)

			' Calculate mag field from magnitude and discard magnitude
			UnsafeHolder.putMag(Me, mag)
			If mag.Length >= MAX_MAG_LENGTH Then
				Try
					checkRange()
				Catch e As ArithmeticException
					Throw New java.io.StreamCorruptedException("BigInteger: Out of the supported range")
				End Try
			End If
		End Sub

		' Support for resetting final fields while deserializing
		Private Class UnsafeHolder
			Private Shared ReadOnly unsafe As sun.misc.Unsafe
			Private Shared ReadOnly signumOffset As Long
			Private Shared ReadOnly magOffset As Long

			Friend Shared Sub putSign(ByVal bi As BigInteger, ByVal sign As Integer)
				unsafe.putIntVolatile(bi, signumOffset, sign)
			End Sub

			Friend Shared Sub putMag(ByVal bi As BigInteger, ByVal magnitude As Integer())
				unsafe.putObjectVolatile(bi, magOffset, magnitude)
			End Sub
		End Class

		''' <summary>
		''' Save the {@code BigInteger} instance to a stream.
		''' The magnitude of a BigInteger is serialized as a byte array for
		''' historical reasons.
		''' 
		''' @serialData two necessary fields are written as well as obsolete
		'''             fields for compatibility with older versions.
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			' set the values of the Serializable fields
			Dim fields As java.io.ObjectOutputStream.PutField = s.putFields()
			fields.put("signum", signum_Renamed)
			fields.put("magnitude", magSerializedForm())
			' The values written for cached fields are compatible with older
			' versions, but are ignored in readObject so don't otherwise matter.
			fields.put("bitCount", -1)
			fields.put("bitLength", -1)
			fields.put("lowestSetBit", -2)
			fields.put("firstNonzeroByteNum", -2)

			' save them
			s.writeFields()
		End Sub

		''' <summary>
		''' Returns the mag array as an array of bytes.
		''' </summary>
		Private Function magSerializedForm() As SByte()
			Dim len As Integer = mag.Length

			Dim bitLen As Integer = (If(len = 0, 0, ((len - 1) << 5) + bitLengthForInt(mag(0))))
			Dim byteLen As Integer = CInt(CUInt((bitLen + 7)) >> 3)
			Dim result As SByte() = New SByte(byteLen - 1){}

			Dim i As Integer = byteLen - 1
			Dim bytesCopied As Integer = 4
			Dim intIndex As Integer = len - 1
			Dim nextInt As Integer = 0
			Do While i >= 0
				If bytesCopied = 4 Then
					nextInt = mag(intIndex)
					intIndex -= 1
					bytesCopied = 1
				Else
					nextInt >>>= 8
					bytesCopied += 1
				End If
				result(i) = CByte(nextInt)
				i -= 1
			Loop
			Return result
		End Function

		''' <summary>
		''' Converts this {@code BigInteger} to a {@code long}, checking
		''' for lost information.  If the value of this {@code BigInteger}
		''' is out of the range of the {@code long} type, then an
		''' {@code ArithmeticException} is thrown.
		''' </summary>
		''' <returns> this {@code BigInteger} converted to a {@code long}. </returns>
		''' <exception cref="ArithmeticException"> if the value of {@code this} will
		''' not exactly fit in a {@code long}. </exception>
		''' <seealso cref= BigInteger#longValue
		''' @since  1.8 </seealso>
		Public Overridable Function longValueExact() As Long
			If mag.Length <= 2 AndAlso bitLength() <= 63 Then
				Return longValue()
			Else
				Throw New ArithmeticException("BigInteger out of long range")
			End If
		End Function

		''' <summary>
		''' Converts this {@code BigInteger} to an {@code int}, checking
		''' for lost information.  If the value of this {@code BigInteger}
		''' is out of the range of the {@code int} type, then an
		''' {@code ArithmeticException} is thrown.
		''' </summary>
		''' <returns> this {@code BigInteger} converted to an {@code int}. </returns>
		''' <exception cref="ArithmeticException"> if the value of {@code this} will
		''' not exactly fit in a {@code int}. </exception>
		''' <seealso cref= BigInteger#intValue
		''' @since  1.8 </seealso>
		Public Overridable Function intValueExact() As Integer
			If mag.Length <= 1 AndAlso bitLength() <= 31 Then
				Return intValue()
			Else
				Throw New ArithmeticException("BigInteger out of int range")
			End If
		End Function

		''' <summary>
		''' Converts this {@code BigInteger} to a {@code short}, checking
		''' for lost information.  If the value of this {@code BigInteger}
		''' is out of the range of the {@code short} type, then an
		''' {@code ArithmeticException} is thrown.
		''' </summary>
		''' <returns> this {@code BigInteger} converted to a {@code short}. </returns>
		''' <exception cref="ArithmeticException"> if the value of {@code this} will
		''' not exactly fit in a {@code short}. </exception>
		''' <seealso cref= BigInteger#shortValue
		''' @since  1.8 </seealso>
		Public Overridable Function shortValueExact() As Short
			If mag.Length <= 1 AndAlso bitLength() <= 31 Then
				Dim value As Integer = intValue()
				If value >= Short.MinValue AndAlso value <= Short.MaxValue Then Return shortValue()
			End If
			Throw New ArithmeticException("BigInteger out of short range")
		End Function

		''' <summary>
		''' Converts this {@code BigInteger} to a {@code byte}, checking
		''' for lost information.  If the value of this {@code BigInteger}
		''' is out of the range of the {@code byte} type, then an
		''' {@code ArithmeticException} is thrown.
		''' </summary>
		''' <returns> this {@code BigInteger} converted to a {@code byte}. </returns>
		''' <exception cref="ArithmeticException"> if the value of {@code this} will
		''' not exactly fit in a {@code byte}. </exception>
		''' <seealso cref= BigInteger#byteValue
		''' @since  1.8 </seealso>
		Public Overridable Function byteValueExact() As SByte
			If mag.Length <= 1 AndAlso bitLength() <= 31 Then
				Dim value As Integer = intValue()
				If value >= Byte.MinValue AndAlso value <= Byte.MaxValue Then Return byteValue()
			End If
			Throw New ArithmeticException("BigInteger out of byte range")
		End Function
	End Class

End Namespace
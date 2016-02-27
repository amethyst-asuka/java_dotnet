Imports System
Imports System.Diagnostics

'
' * Copyright (c) 1996, 2014, Oracle and/or its affiliates. All rights reserved.
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
' * (C) Copyright Taligent, Inc. 1996, 1997 - All Rights Reserved
' * (C) Copyright IBM Corp. 1996 - 1998 - All Rights Reserved
' *
' *   The original version of this source code and documentation is copyrighted
' * and owned by Taligent, Inc., a wholly-owned subsidiary of IBM. These
' * materials are provided under terms of a License Agreement between Taligent
' * and Sun. This technology is protected by multiple US and International
' * patents. This notice and attribution to Taligent may not be removed.
' *   Taligent is a registered trademark of Taligent, Inc.
' *
' 

Namespace java.text


	''' <summary>
	''' Digit List. Private to DecimalFormat.
	''' Handles the transcoding
	''' between numeric values and strings of characters.  Only handles
	''' non-negative numbers.  The division of labor between DigitList and
	''' DecimalFormat is that DigitList handles the radix 10 representation
	''' issues; DecimalFormat handles the locale-specific issues such as
	''' positive/negative, grouping, decimal point, currency, and so on.
	''' 
	''' A DigitList is really a representation of a floating point value.
	''' It may be an integer value; we assume that a double has sufficient
	''' precision to represent all digits of a java.lang.[Long].
	''' 
	''' The DigitList representation consists of a string of characters,
	''' which are the digits radix 10, from '0' to '9'.  It also has a radix
	''' 10 exponent associated with it.  The value represented by a DigitList
	''' object can be computed by mulitplying the fraction f, where 0 <= f < 1,
	''' derived by placing all the digits of the list to the right of the
	''' decimal point, by 10^exponent.
	''' </summary>
	''' <seealso cref=  Locale </seealso>
	''' <seealso cref=  Format </seealso>
	''' <seealso cref=  NumberFormat </seealso>
	''' <seealso cref=  DecimalFormat </seealso>
	''' <seealso cref=  ChoiceFormat </seealso>
	''' <seealso cref=  MessageFormat
	''' @author       Mark Davis, Alan Liu </seealso>
	Friend NotInheritable Class DigitList
		Implements Cloneable

		''' <summary>
		''' The maximum number of significant digits in an IEEE 754 double, that
		''' is, in a Java java.lang.[Double].  This must not be increased, or garbage digits
		''' will be generated, and should not be decreased, or accuracy will be lost.
		''' </summary>
		Public Const MAX_COUNT As Integer = 19 ' == java.lang.[Long].toString(Long.MAX_VALUE).length()

		''' <summary>
		''' These data members are intentionally public and can be set directly.
		''' 
		''' The value represented is given by placing the decimal point before
		''' digits[decimalAt].  If decimalAt is < 0, then leading zeros between
		''' the decimal point and the first nonzero digit are implied.  If decimalAt
		''' is > count, then trailing zeros between the digits[count-1] and the
		''' decimal point are implied.
		''' 
		''' Equivalently, the represented value is given by f * 10^decimalAt.  Here
		''' f is a value 0.1 <= f < 1 arrived at by placing the digits in Digits to
		''' the right of the decimal.
		''' 
		''' DigitList is normalized, so if it is non-zero, figits[0] is non-zero.  We
		''' don't allow denormalized numbers because our exponent is effectively of
		''' unlimited magnitude.  The count value contains the number of significant
		''' digits present in digits[].
		''' 
		''' Zero is represented by any DigitList with count == 0 or with each digits[i]
		''' for all i <= count == '0'.
		''' </summary>
		Public decimalAt As Integer = 0
		Public count As Integer = 0
		Public digits As Char() = New Char(MAX_COUNT - 1){}

		Private data As Char()
		Private roundingMode As java.math.RoundingMode = java.math.RoundingMode.HALF_EVEN
		Private isNegative As Boolean = False

		''' <summary>
		''' Return true if the represented number is zero.
		''' </summary>
		Friend Property zero As Boolean
			Get
				For i As Integer = 0 To count - 1
					If digits(i) <> "0"c Then Return False
				Next i
				Return True
			End Get
		End Property

		''' <summary>
		''' Set the rounding mode
		''' </summary>
		Friend Property roundingMode As java.math.RoundingMode
			Set(ByVal r As java.math.RoundingMode)
				roundingMode = r
			End Set
		End Property

		''' <summary>
		''' Clears out the digits.
		''' Use before appending them.
		''' Typically, you set a series of digits with append, then at the point
		''' you hit the decimal point, you set myDigitList.decimalAt = myDigitList.count;
		''' then go on appending digits.
		''' </summary>
		Public Sub clear()
			decimalAt = 0
			count = 0
		End Sub

		''' <summary>
		''' Appends a digit to the list, extending the list when necessary.
		''' </summary>
		Public Sub append(ByVal digit As Char)
			If count = digits.Length Then
				Dim data As Char() = New Char(count + 100 - 1){}
				Array.Copy(digits, 0, data, 0, count)
				digits = data
			End If
			digits(count) = digit
			count += 1
		End Sub

		''' <summary>
		''' Utility routine to get the value of the digit list
		''' If (count == 0) this throws a NumberFormatException, which
		''' mimics java.lang.[Long].parseLong().
		''' </summary>
		Public Property [double] As Double
			Get
				If count = 0 Then Return 0.0
    
				Dim temp As StringBuffer = stringBuffer
				temp.append("."c)
				temp.append(digits, 0, count)
				temp.append("E"c)
				temp.append(decimalAt)
				Return Convert.ToDouble(temp.ToString())
			End Get
		End Property

		''' <summary>
		''' Utility routine to get the value of the digit list.
		''' If (count == 0) this returns 0, unlike java.lang.[Long].parseLong().
		''' </summary>
		Public Property [long] As Long
			Get
				' for now, simple implementation; later, do proper IEEE native stuff
    
				If count = 0 Then Return 0
    
				' We have to check for this, because this is the one NEGATIVE value
				' we represent.  If we tried to just pass the digits off to parseLong,
				' we'd get a parse failure.
				If longMIN_VALUE Then Return java.lang.[Long].MIN_VALUE
    
				Dim temp As StringBuffer = stringBuffer
				temp.append(digits, 0, count)
				For i As Integer = count To decimalAt - 1
					temp.append("0"c)
				Next i
				Return Convert.ToInt64(temp.ToString())
			End Get
		End Property

		Public Property bigDecimal As Decimal
			Get
				If count = 0 Then
					If decimalAt = 0 Then
						Return Decimal.Zero
					Else
						Return New Decimal("0E" & decimalAt)
					End If
				End If
    
			   If decimalAt = count Then
				   Return New Decimal(digits, 0, count)
			   Else
				   Return (New Decimal(digits, 0, count)).scaleByPowerOfTen(decimalAt - count)
			   End If
			End Get
		End Property

		''' <summary>
		''' Return true if the number represented by this object can fit into
		''' a java.lang.[Long]. </summary>
		''' <param name="isPositive"> true if this number should be regarded as positive </param>
		''' <param name="ignoreNegativeZero"> true if -0 should be regarded as identical to
		''' +0; otherwise they are considered distinct </param>
		''' <returns> true if this number fits into a Java long </returns>
		Friend Function fitsIntoLong(ByVal isPositive As Boolean, ByVal ignoreNegativeZero As Boolean) As Boolean
			' Figure out if the result will fit in a java.lang.[Long].  We have to
			' first look for nonzero digits after the decimal point;
			' then check the size.  If the digit count is 18 or less, then
			' the value can definitely be represented as a java.lang.[Long].  If it is 19
			' then it may be too large.

			' Trim trailing zeros.  This does not change the represented value.
			Do While count > 0 AndAlso digits(count - 1) = "0"c
				count -= 1
			Loop

			If count = 0 Then Return isPositive OrElse ignoreNegativeZero

			If decimalAt < count OrElse decimalAt > MAX_COUNT Then Return False

			If decimalAt < MAX_COUNT Then Return True

			' At this point we have decimalAt == count, and count == MAX_COUNT.
			' The number will overflow if it is larger than 9223372036854775807
			' or smaller than -9223372036854775808.
			For i As Integer = 0 To count - 1
				Dim dig As Char = digits(i), max As Char = LONG_MIN_REP(i)
				If dig > max Then Return False
				If dig < max Then Return True
			Next i

			' At this point the first count digits match.  If decimalAt is less
			' than count, then the remaining digits are zero, and we return true.
			If count < decimalAt Then Return True

			' Now we have a representation of java.lang.[Long].MIN_VALUE, without the leading
			' negative sign.  If this represents a positive value, then it does
			' not fit; otherwise it fits.
			Return Not isPositive
		End Function

		''' <summary>
		''' Set the digit list to a representation of the given double value.
		''' This method supports fixed-point notation. </summary>
		''' <param name="isNegative"> Boolean value indicating whether the number is negative. </param>
		''' <param name="source"> Value to be converted; must not be Inf, -Inf, Nan,
		''' or a value <= 0. </param>
		''' <param name="maximumFractionDigits"> The most fractional digits which should
		''' be converted. </param>
		Friend Sub [set](ByVal isNegative As Boolean, ByVal source As Double, ByVal maximumFractionDigits As Integer)
			[set](isNegative, source, maximumFractionDigits, True)
		End Sub

		''' <summary>
		''' Set the digit list to a representation of the given double value.
		''' This method supports both fixed-point and exponential notation. </summary>
		''' <param name="isNegative"> Boolean value indicating whether the number is negative. </param>
		''' <param name="source"> Value to be converted; must not be Inf, -Inf, Nan,
		''' or a value <= 0. </param>
		''' <param name="maximumDigits"> The most fractional or total digits which should
		''' be converted. </param>
		''' <param name="fixedPoint"> If true, then maximumDigits is the maximum
		''' fractional digits to be converted.  If false, total digits. </param>
		Friend Sub [set](ByVal isNegative As Boolean, ByVal source As Double, ByVal maximumDigits As Integer, ByVal fixedPoint As Boolean)

			Dim fdConverter As sun.misc.FloatingDecimal.BinaryToASCIIConverter = sun.misc.FloatingDecimal.getBinaryToASCIIConverter(source)
			Dim hasBeenRoundedUp As Boolean = fdConverter.digitsRoundedUp()
			Dim valueExactAsDecimal As Boolean = fdConverter.decimalDigitsExact()
			Debug.Assert((Not fdConverter.exceptional))
			Dim digitsString As String = fdConverter.toJavaFormatString()

			[set](isNegative, digitsString, hasBeenRoundedUp, valueExactAsDecimal, maximumDigits, fixedPoint)
		End Sub

		''' <summary>
		''' Generate a representation of the form DDDDD, DDDDD.DDDDD, or
		''' DDDDDE+/-DDDDD. </summary>
		''' <param name="roundedUp"> whether or not rounding up has already happened. </param>
		''' <param name="valueExactAsDecimal"> whether or not collected digits provide
		''' an exact decimal representation of the value. </param>
		Private Sub [set](ByVal isNegative As Boolean, ByVal s As String, ByVal roundedUp As Boolean, ByVal valueExactAsDecimal As Boolean, ByVal maximumDigits As Integer, ByVal fixedPoint As Boolean)

			Me.isNegative = isNegative
			Dim len As Integer = s.length()
			Dim source As Char() = getDataChars(len)
			s.getChars(0, len, source, 0)

			decimalAt = -1
			count = 0
			Dim exponent As Integer = 0
			' Number of zeros between decimal point and first non-zero digit after
			' decimal point, for numbers < 1.
			Dim leadingZerosAfterDecimal As Integer = 0
			Dim nonZeroDigitSeen As Boolean = False

			Dim i As Integer = 0
			Do While i < len
				Dim c As Char = source(i)
				i += 1
				If c = "."c Then
					decimalAt = count
				ElseIf c = "e"c OrElse c = "E"c Then
					exponent = parseInt(source, i, len)
					Exit Do
				Else
					If Not nonZeroDigitSeen Then
						nonZeroDigitSeen = (c <> "0"c)
						If (Not nonZeroDigitSeen) AndAlso decimalAt <> -1 Then leadingZerosAfterDecimal += 1
					End If
					If nonZeroDigitSeen Then
						digits(count) = c
						count += 1
					End If
				End If
			Loop
			If decimalAt = -1 Then decimalAt = count
			If nonZeroDigitSeen Then decimalAt += exponent - leadingZerosAfterDecimal

			If fixedPoint Then
				' The negative of the exponent represents the number of leading
				' zeros between the decimal and the first non-zero digit, for
				' a value < 0.1 (e.g., for 0.00123, -decimalAt == 2).  If this
				' is more than the maximum fraction digits, then we have an underflow
				' for the printed representation.
				If -decimalAt > maximumDigits Then
					' Handle an underflow to zero when we round something like
					' 0.0009 to 2 fractional digits.
					count = 0
					Return
				ElseIf -decimalAt = maximumDigits Then
					' If we round 0.0009 to 3 fractional digits, then we have to
					' create a new one digit in the least significant location.
					If shouldRoundUp(0, roundedUp, valueExactAsDecimal) Then
						count = 1
						decimalAt += 1
						digits(0) = "1"c
					Else
						count = 0
					End If
					Return
				End If
				' else fall through
			End If

			' Eliminate trailing zeros.
			Do While count > 1 AndAlso digits(count - 1) = "0"c
				count -= 1
			Loop

			' Eliminate digits beyond maximum digits to be displayed.
			' Round up if appropriate.
			round(If(fixedPoint, (maximumDigits + decimalAt), maximumDigits), roundedUp, valueExactAsDecimal)

		End Sub

		''' <summary>
		''' Round the representation to the given number of digits. </summary>
		''' <param name="maximumDigits"> The maximum number of digits to be shown. </param>
		''' <param name="alreadyRounded"> whether or not rounding up has already happened. </param>
		''' <param name="valueExactAsDecimal"> whether or not collected digits provide
		''' an exact decimal representation of the value.
		''' 
		''' Upon return, count will be less than or equal to maximumDigits. </param>
		Private Sub round(ByVal maximumDigits As Integer, ByVal alreadyRounded As Boolean, ByVal valueExactAsDecimal As Boolean)
			' Eliminate digits beyond maximum digits to be displayed.
			' Round up if appropriate.
			If maximumDigits >= 0 AndAlso maximumDigits < count Then
				If shouldRoundUp(maximumDigits, alreadyRounded, valueExactAsDecimal) Then
					' Rounding up involved incrementing digits from LSD to MSD.
					' In most cases this is simple, but in a worst case situation
					' (9999..99) we have to adjust the decimalAt value.
					Do
						maximumDigits -= 1
						If maximumDigits < 0 Then
							' We have all 9's, so we increment to a single digit
							' of one and adjust the exponent.
							digits(0) = "1"c
							decimalAt += 1
							maximumDigits = 0 ' Adjust the count
							Exit Do
						End If

						digits(maximumDigits) = ChrW(AscW(digits(maximumDigits)) + 1)
						If digits(maximumDigits) <= "9"c Then Exit Do
						' digits[maximumDigits] = '0'; // Unnecessary since we'll truncate this
					Loop
					maximumDigits += 1 ' Increment for use as count
				End If
				count = maximumDigits

				' Eliminate trailing zeros.
				Do While count > 1 AndAlso digits(count-1) = "0"c
					count -= 1
				Loop
			End If
		End Sub


		''' <summary>
		''' Return true if truncating the representation to the given number
		''' of digits will result in an increment to the last digit.  This
		''' method implements the rounding modes defined in the
		''' java.math.RoundingMode class.
		''' [bnf] </summary>
		''' <param name="maximumDigits"> the number of digits to keep, from 0 to
		''' <code>count-1</code>.  If 0, then all digits are rounded away, and
		''' this method returns true if a one should be generated (e.g., formatting
		''' 0.09 with "#.#"). </param>
		''' <param name="alreadyRounded"> whether or not rounding up has already happened. </param>
		''' <param name="valueExactAsDecimal"> whether or not collected digits provide
		''' an exact decimal representation of the value. </param>
		''' <exception cref="ArithmeticException"> if rounding is needed with rounding
		'''            mode being set to RoundingMode.UNNECESSARY </exception>
		''' <returns> true if digit <code>maximumDigits-1</code> should be
		''' incremented </returns>
		Private Function shouldRoundUp(ByVal maximumDigits As Integer, ByVal alreadyRounded As Boolean, ByVal valueExactAsDecimal As Boolean) As Boolean
			If maximumDigits < count Then
	'            
	'             * To avoid erroneous double-rounding or truncation when converting
	'             * a binary double value to text, information about the exactness
	'             * of the conversion result in FloatingDecimal, as well as any
	'             * rounding done, is needed in this class.
	'             *
	'             * - For the  HALF_DOWN, HALF_EVEN, HALF_UP rounding rules below:
	'             *   In the case of formating float or double, We must take into
	'             *   account what FloatingDecimal has done in the binary to decimal
	'             *   conversion.
	'             *
	'             *   Considering the tie cases, FloatingDecimal may round up the
	'             *   value (returning decimal digits equal to tie when it is below),
	'             *   or "truncate" the value to the tie while value is above it,
	'             *   or provide the exact decimal digits when the binary value can be
	'             *   converted exactly to its decimal representation given formating
	'             *   rules of FloatingDecimal ( we have thus an exact decimal
	'             *   representation of the binary value).
	'             *
	'             *   - If the double binary value was converted exactly as a decimal
	'             *     value, then DigitList code must apply the expected rounding
	'             *     rule.
	'             *
	'             *   - If FloatingDecimal already rounded up the decimal value,
	'             *     DigitList should neither round up the value again in any of
	'             *     the three rounding modes above.
	'             *
	'             *   - If FloatingDecimal has truncated the decimal value to
	'             *     an ending '5' digit, DigitList should round up the value in
	'             *     all of the three rounding modes above.
	'             *
	'             *
	'             *   This has to be considered only if digit at maximumDigits index
	'             *   is exactly the last one in the set of digits, otherwise there are
	'             *   remaining digits after that position and we don't have to consider
	'             *   what FloatingDecimal did.
	'             *
	'             * - Other rounding modes are not impacted by these tie cases.
	'             *
	'             * - For other numbers that are always converted to exact digits
	'             *   (like BigInteger, Long, ...), the passed alreadyRounded boolean
	'             *   have to be  set to false, and valueExactAsDecimal has to be set to
	'             *   true in the upper DigitList call stack, providing the right state
	'             *   for those situations..
	'             

				Select Case roundingMode
				Case java.math.RoundingMode.UP
					For i As Integer = maximumDigits To count - 1
						If digits(i) <> "0"c Then Return True
					Next i
				Case java.math.RoundingMode.DOWN
				Case java.math.RoundingMode.CEILING
					For i As Integer = maximumDigits To count - 1
						If digits(i) <> "0"c Then Return Not isNegative
					Next i
				Case java.math.RoundingMode.FLOOR
					For i As Integer = maximumDigits To count - 1
						If digits(i) <> "0"c Then Return isNegative
					Next i
				Case java.math.RoundingMode.HALF_UP, HALF_DOWN
					If digits(maximumDigits) > "5"c Then
						' Value is above tie ==> must round up
						Return True
					ElseIf digits(maximumDigits) = "5"c Then
						' Digit at rounding position is a '5'. Tie cases.
						If maximumDigits <> (count - 1) Then
							' There are remaining digits. Above tie => must round up
							Return True
						Else
							' Digit at rounding position is the last one !
							If valueExactAsDecimal Then
								' Exact binary representation. On the tie.
								' Apply rounding given by roundingMode.
								Return roundingMode = java.math.RoundingMode.HALF_UP
							Else
								' Not an exact binary representation.
								' Digit sequence either rounded up or truncated.
								' Round up only if it was truncated.
								Return Not alreadyRounded
							End If
						End If
					End If
					' Digit at rounding position is < '5' ==> no round up.
					' Just let do the default, which is no round up (thus break).
				Case java.math.RoundingMode.HALF_EVEN
					' Implement IEEE half-even rounding
					If digits(maximumDigits) > "5"c Then
						Return True
					ElseIf digits(maximumDigits) = "5"c Then
						If maximumDigits = (count - 1) Then
							' the rounding position is exactly the last index :
							If alreadyRounded Then Return False

							If Not valueExactAsDecimal Then
								' Otherwise if the digits don't represent exact value,
								' value was above tie and FloatingDecimal truncated
								' digits to tie. We must round up.
								Return True
							Else
								' This is an exact tie value, and FloatingDecimal
								' provided all of the exact digits. We thus apply
								' HALF_EVEN rounding rule.
								Return ((maximumDigits > 0) AndAlso (digits(maximumDigits-1) Mod 2 <> 0))
							End If
						Else
							' Rounds up if it gives a non null digit after '5'
							For i As Integer = maximumDigits+1 To count - 1
								If digits(i) <> "0"c Then Return True
							Next i
						End If
					End If
				Case java.math.RoundingMode.UNNECESSARY
					For i As Integer = maximumDigits To count - 1
						If digits(i) <> "0"c Then Throw New ArithmeticException("Rounding needed with the rounding mode being set to RoundingMode.UNNECESSARY")
					Next i
				Case Else
					Debug.Assert(False)
				End Select
			End If
			Return False
		End Function

		''' <summary>
		''' Utility routine to set the value of the digit list from a long
		''' </summary>
		Friend Sub [set](ByVal isNegative As Boolean, ByVal source As Long)
			[set](isNegative, source, 0)
		End Sub

		''' <summary>
		''' Set the digit list to a representation of the given long value. </summary>
		''' <param name="isNegative"> Boolean value indicating whether the number is negative. </param>
		''' <param name="source"> Value to be converted; must be >= 0 or ==
		''' java.lang.[Long].MIN_VALUE. </param>
		''' <param name="maximumDigits"> The most digits which should be converted.
		''' If maximumDigits is lower than the number of significant digits
		''' in source, the representation will be rounded.  Ignored if <= 0. </param>
		Friend Sub [set](ByVal isNegative As Boolean, ByVal source As Long, ByVal maximumDigits As Integer)
			Me.isNegative = isNegative

			' This method does not expect a negative number. However,
			' "source" can be a java.lang.[Long].MIN_VALUE (-9223372036854775808),
			' if the number being formatted is a java.lang.[Long].MIN_VALUE.  In that
			' case, it will be formatted as -Long.MIN_VALUE, a number
			' which is outside the legal range of a long, but which can
			' be represented by DigitList.
			If source <= 0 Then
				If source = java.lang.[Long].MIN_VALUE Then
						count = MAX_COUNT
						decimalAt = count
					Array.Copy(LONG_MIN_REP, 0, digits, 0, count)
				Else
						count = 0
						decimalAt = count
				End If
			Else
				' Rewritten to improve performance.  I used to call
				' java.lang.[Long].toString(), which was about 4x slower than this code.
				Dim left As Integer = MAX_COUNT
				Dim right As Integer
				Do While source > 0
					left -= 1
					digits(left) = ChrW(AscW("0"c) + (source Mod 10))
					source \= 10
				Loop
				decimalAt = MAX_COUNT - left
				' Don't copy trailing zeros.  We are guaranteed that there is at
				' least one non-zero digit, so we don't have to check lower bounds.
				right = MAX_COUNT - 1
				Do While digits(right) = "0"c

					right -= 1
				Loop
				count = right - left + 1
				Array.Copy(digits, left, digits, 0, count)
			End If
			If maximumDigits > 0 Then round(maximumDigits, False, True)
		End Sub

		''' <summary>
		''' Set the digit list to a representation of the given BigDecimal value.
		''' This method supports both fixed-point and exponential notation. </summary>
		''' <param name="isNegative"> Boolean value indicating whether the number is negative. </param>
		''' <param name="source"> Value to be converted; must not be a value <= 0. </param>
		''' <param name="maximumDigits"> The most fractional or total digits which should
		''' be converted. </param>
		''' <param name="fixedPoint"> If true, then maximumDigits is the maximum
		''' fractional digits to be converted.  If false, total digits. </param>
		Friend Sub [set](ByVal isNegative As Boolean, ByVal source As Decimal, ByVal maximumDigits As Integer, ByVal fixedPoint As Boolean)
			Dim s As String = source.ToString()
			extendDigits(s.length())

			[set](isNegative, s, False, True, maximumDigits, fixedPoint)
		End Sub

		''' <summary>
		''' Set the digit list to a representation of the given BigInteger value. </summary>
		''' <param name="isNegative"> Boolean value indicating whether the number is negative. </param>
		''' <param name="source"> Value to be converted; must be >= 0. </param>
		''' <param name="maximumDigits"> The most digits which should be converted.
		''' If maximumDigits is lower than the number of significant digits
		''' in source, the representation will be rounded.  Ignored if <= 0. </param>
		Friend Sub [set](ByVal isNegative As Boolean, ByVal source As System.Numerics.BigInteger, ByVal maximumDigits As Integer)
			Me.isNegative = isNegative
			Dim s As String = source.ToString()
			Dim len As Integer = s.length()
			extendDigits(len)
			s.getChars(0, len, digits, 0)

			decimalAt = len
			Dim right As Integer
			right = len - 1
			Do While right >= 0 AndAlso digits(right) = "0"c

				right -= 1
			Loop
			count = right + 1

			If maximumDigits > 0 Then round(maximumDigits, False, True)
		End Sub

		''' <summary>
		''' equality test between two digit lists.
		''' </summary>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then ' quick check Return True
			If Not(TypeOf obj Is DigitList) Then ' (1) same object? Return False
			Dim other As DigitList = CType(obj, DigitList)
			If count <> other.count OrElse decimalAt <> other.decimalAt Then Return False
			For i As Integer = 0 To count - 1
				If digits(i) <> other.digits(i) Then Return False
			Next i
			Return True
		End Function

		''' <summary>
		''' Generates the hash code for the digit list.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Dim hashcode_Renamed As Integer = decimalAt

			For i As Integer = 0 To count - 1
				hashcode_Renamed = hashcode_Renamed * 37 + AscW(digits(i))
			Next i

			Return hashcode_Renamed
		End Function

		''' <summary>
		''' Creates a copy of this object. </summary>
		''' <returns> a clone of this instance. </returns>
		Public Function clone() As Object
			Try
				Dim other As DigitList = CType(MyBase.clone(), DigitList)
				Dim newDigits As Char() = New Char(digits.Length - 1){}
				Array.Copy(digits, 0, newDigits, 0, digits.Length)
				other.digits = newDigits
				other.tempBuffer = Nothing
				Return other
			Catch e As CloneNotSupportedException
				Throw New InternalError(e)
			End Try
		End Function

		''' <summary>
		''' Returns true if this DigitList represents java.lang.[Long].MIN_VALUE;
		''' false, otherwise.  This is required so that getLong() works.
		''' </summary>
		Private Property longMIN_VALUE As Boolean
			Get
				If decimalAt <> count OrElse count <> MAX_COUNT Then Return False
    
				For i As Integer = 0 To count - 1
					If digits(i) <> LONG_MIN_REP(i) Then Return False
				Next i
    
				Return True
			End Get
		End Property

		Private Shared Function parseInt(ByVal str As Char(), ByVal offset As Integer, ByVal strLen As Integer) As Integer
			Dim c As Char
			Dim positive As Boolean = True
			c = str(offset)
			If c = "-"c Then
				positive = False
				offset += 1
			ElseIf c = "+"c Then
				offset += 1
			End If

			Dim value As Integer = 0
			Do While offset < strLen
				c = str(offset)
				offset += 1
				If c >= "0"c AndAlso c <= "9"c Then
					value = value * 10 + (AscW(c) - AscW("0"c))
				Else
					Exit Do
				End If
			Loop
			Return If(positive, value, -value)
		End Function

		' The digit part of -9223372036854775808L
		Private Shared ReadOnly LONG_MIN_REP As Char() = "9223372036854775808".toCharArray()

		Public Overrides Function ToString() As String
			If zero Then Return "0"
			Dim buf As StringBuffer = stringBuffer
			buf.append("0.")
			buf.append(digits, 0, count)
			buf.append("x10^")
			buf.append(decimalAt)
			Return buf.ToString()
		End Function

		Private tempBuffer As StringBuffer

		Private Property stringBuffer As StringBuffer
			Get
				If tempBuffer Is Nothing Then
					tempBuffer = New StringBuffer(MAX_COUNT)
				Else
					tempBuffer.length = 0
				End If
				Return tempBuffer
			End Get
		End Property

		Private Sub extendDigits(ByVal len As Integer)
			If len > digits.Length Then digits = New Char(len - 1){}
		End Sub

		Private Function getDataChars(ByVal length As Integer) As Char()
			If data Is Nothing OrElse data.Length < length Then data = New Char(length - 1){}
			Return data
		End Function
	End Class

End Namespace
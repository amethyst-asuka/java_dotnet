Imports Microsoft.VisualBasic
Imports System

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

Namespace java.math

	''' <summary>
	''' A class used to represent multiprecision integers that makes efficient
	''' use of allocated space by allowing a number to occupy only part of
	''' an array so that the arrays do not have to be reallocated as often.
	''' When performing an operation with many iterations the array used to
	''' hold a number is only reallocated when necessary and does not have to
	''' be the same size as the number it represents. A mutable number allows
	''' calculations to occur on the same number without having to create
	''' a new number for every step of the calculation as occurs with
	''' BigIntegers.
	''' </summary>
	''' <seealso cref=     BigInteger
	''' @author  Michael McCloskey
	''' @author  Timothy Buktu
	''' @since   1.3 </seealso>


	Friend Class MutableBigInteger
		''' <summary>
		''' Holds the magnitude of this MutableBigInteger in big endian order.
		''' The magnitude may start at an offset into the value array, and it may
		''' end before the length of the value array.
		''' </summary>
		Friend value As Integer()

		''' <summary>
		''' The number of ints of the value array that are currently used
		''' to hold the magnitude of this MutableBigInteger. The magnitude starts
		''' at an offset and offset + intLen may be less than value.length.
		''' </summary>
		Friend intLen As Integer

		''' <summary>
		''' The offset into the value array where the magnitude of this
		''' MutableBigInteger begins.
		''' </summary>
		Friend offset As Integer = 0

		' Constants
		''' <summary>
		''' MutableBigInteger with one element value array with the value 1. Used by
		''' BigDecimal divideAndRound to increment the quotient. Use this constant
		''' only when the method is not going to modify this object.
		''' </summary>
		Friend Shared ReadOnly ONE As New MutableBigInteger(1)

		''' <summary>
		''' The minimum {@code intLen} for cancelling powers of two before
		''' dividing.
		''' If the number of ints is less than this threshold,
		''' {@code divideKnuth} does not eliminate common powers of two from
		''' the dividend and divisor.
		''' </summary>
		Friend Const KNUTH_POW2_THRESH_LEN As Integer = 6

		''' <summary>
		''' The minimum number of trailing zero ints for cancelling powers of two
		''' before dividing.
		''' If the dividend and divisor don't share at least this many zero ints
		''' at the end, {@code divideKnuth} does not eliminate common powers
		''' of two from the dividend and divisor.
		''' </summary>
		Friend Const KNUTH_POW2_THRESH_ZEROS As Integer = 3

		' Constructors

		''' <summary>
		''' The default constructor. An empty MutableBigInteger is created with
		''' a one word capacity.
		''' </summary>
		Friend Sub New()
			value = New Integer(0){}
			intLen = 0
		End Sub

		''' <summary>
		''' Construct a new MutableBigInteger with a magnitude specified by
		''' the int val.
		''' </summary>
		Friend Sub New(ByVal val As Integer)
			value = New Integer(0){}
			intLen = 1
			value(0) = val
		End Sub

		''' <summary>
		''' Construct a new MutableBigInteger with the specified value array
		''' up to the length of the array supplied.
		''' </summary>
		Friend Sub New(ByVal val As Integer())
			value = val
			intLen = val.Length
		End Sub

		''' <summary>
		''' Construct a new MutableBigInteger with a magnitude equal to the
		''' specified BigInteger.
		''' </summary>
		Friend Sub New(ByVal b As BigInteger)
			intLen = b.mag.Length
			value = java.util.Arrays.copyOf(b.mag, intLen)
		End Sub

		''' <summary>
		''' Construct a new MutableBigInteger with a magnitude equal to the
		''' specified MutableBigInteger.
		''' </summary>
		Friend Sub New(ByVal val As MutableBigInteger)
			intLen = val.intLen
			value = java.util.Arrays.copyOfRange(val.value, val.offset, val.offset + intLen)
		End Sub

		''' <summary>
		''' Makes this number an {@code n}-int number all of whose bits are ones.
		''' Used by Burnikel-Ziegler division. </summary>
		''' <param name="n"> number of ints in the {@code value} array </param>
		''' <returns> a number equal to {@code ((1<<(32*n)))-1} </returns>
		Private Sub ones(ByVal n As Integer)
			If n > value.Length Then value = New Integer(n - 1){}
			java.util.Arrays.fill(value, -1)
			offset = 0
			intLen = n
		End Sub

		''' <summary>
		''' Internal helper method to return the magnitude array. The caller is not
		''' supposed to modify the returned array.
		''' </summary>
		Private Property magnitudeArray As Integer()
			Get
				If offset > 0 OrElse value.Length <> intLen Then Return java.util.Arrays.copyOfRange(value, offset, offset + intLen)
				Return value
			End Get
		End Property

		''' <summary>
		''' Convert this MutableBigInteger to a long value. The caller has to make
		''' sure this MutableBigInteger can be fit into long.
		''' </summary>
		Private Function toLong() As Long
			assert(intLen <= 2) : "this MutableBigInteger exceeds the range of long"
			If intLen = 0 Then Return 0
			Dim d As Long = value(offset) And LONG_MASK
			Return If(intLen = 2, d << 32 Or (value(offset + 1) And LONG_MASK), d)
		End Function

		''' <summary>
		''' Convert this MutableBigInteger to a BigInteger object.
		''' </summary>
		Friend Overridable Function toBigInteger(ByVal sign As Integer) As BigInteger
			If intLen = 0 OrElse sign = 0 Then Return BigInteger.ZERO
			Return New BigInteger(magnitudeArray, sign)
		End Function

		''' <summary>
		''' Converts this number to a nonnegative {@code BigInteger}.
		''' </summary>
		Friend Overridable Function toBigInteger() As BigInteger
			normalize()
			Return toBigInteger(If(zero, 0, 1))
		End Function

		''' <summary>
		''' Convert this MutableBigInteger to BigDecimal object with the specified sign
		''' and scale.
		''' </summary>
		Friend Overridable Function toBigDecimal(ByVal sign As Integer, ByVal scale As Integer) As BigDecimal
			If intLen = 0 OrElse sign = 0 Then Return BigDecimal.zeroValueOf(scale)
			Dim mag As Integer() = magnitudeArray
			Dim len As Integer = mag.Length
			Dim d As Integer = mag(0)
			' If this MutableBigInteger can't be fit into long, we need to
			' make a BigInteger object for the resultant BigDecimal object.
			If len > 2 OrElse (d < 0 AndAlso len = 2) Then Return New BigDecimal(New BigInteger(mag, sign), INFLATED, scale, 0)
			Dim v As Long = If(len = 2, ((mag(1) And LONG_MASK) Or (d And LONG_MASK) << 32), d And LONG_MASK)
			Return BigDecimal.valueOf(If(sign = -1, -v, v), scale)
		End Function

		''' <summary>
		''' This is for internal use in converting from a MutableBigInteger
		''' object into a long value given a specified sign.
		''' returns INFLATED if value is not fit into long
		''' </summary>
		Friend Overridable Function toCompactValue(ByVal sign As Integer) As Long
			If intLen = 0 OrElse sign = 0 Then Return 0L
			Dim mag As Integer() = magnitudeArray
			Dim len As Integer = mag.Length
			Dim d As Integer = mag(0)
			' If this MutableBigInteger can not be fitted into long, we need to
			' make a BigInteger object for the resultant BigDecimal object.
			If len > 2 OrElse (d < 0 AndAlso len = 2) Then Return INFLATED
			Dim v As Long = If(len = 2, ((mag(1) And LONG_MASK) Or (d And LONG_MASK) << 32), d And LONG_MASK)
			Return If(sign = -1, -v, v)
		End Function

		''' <summary>
		''' Clear out a MutableBigInteger for reuse.
		''' </summary>
		Friend Overridable Sub clear()
				intLen = 0
				offset = intLen
			Dim index As Integer=0
			Dim n As Integer=value.Length
			Do While index < n
				value(index) = 0
				index += 1
			Loop
		End Sub

		''' <summary>
		''' Set a MutableBigInteger to zero, removing its offset.
		''' </summary>
		Friend Overridable Sub reset()
				intLen = 0
				offset = intLen
		End Sub

		''' <summary>
		''' Compare the magnitude of two MutableBigIntegers. Returns -1, 0 or 1
		''' as this MutableBigInteger is numerically less than, equal to, or
		''' greater than <tt>b</tt>.
		''' </summary>
		Friend Function compare(ByVal b As MutableBigInteger) As Integer
			Dim blen As Integer = b.intLen
			If intLen < blen Then Return -1
			If intLen > blen Then Return 1

			' Add Integer.MIN_VALUE to make the comparison act as unsigned integer
			' comparison.
			Dim bval As Integer() = b.value
			Dim i As Integer = offset
			Dim j As Integer = b.offset
			Do While i < intLen + offset
				Dim b1 As Integer = value(i) + &H80000000L
				Dim b2 As Integer = bval(j) + &H80000000L
				If b1 < b2 Then Return -1
				If b1 > b2 Then Return 1
				i += 1
				j += 1
			Loop
			Return 0
		End Function

		''' <summary>
		''' Returns a value equal to what {@code b.leftShift(32*ints); return compare(b);}
		''' would return, but doesn't change the value of {@code b}.
		''' </summary>
		Private Function compareShifted(ByVal b As MutableBigInteger, ByVal ints As Integer) As Integer
			Dim blen As Integer = b.intLen
			Dim alen As Integer = intLen - ints
			If alen < blen Then Return -1
			If alen > blen Then Return 1

			' Add Integer.MIN_VALUE to make the comparison act as unsigned integer
			' comparison.
			Dim bval As Integer() = b.value
			Dim i As Integer = offset
			Dim j As Integer = b.offset
			Do While i < alen + offset
				Dim b1 As Integer = value(i) + &H80000000L
				Dim b2 As Integer = bval(j) + &H80000000L
				If b1 < b2 Then Return -1
				If b1 > b2 Then Return 1
				i += 1
				j += 1
			Loop
			Return 0
		End Function

		''' <summary>
		''' Compare this against half of a MutableBigInteger object (Needed for
		''' remainder tests).
		''' Assumes no leading unnecessary zeros, which holds for results
		''' from divide().
		''' </summary>
		Friend Function compareHalf(ByVal b As MutableBigInteger) As Integer
			Dim blen As Integer = b.intLen
			Dim len As Integer = intLen
			If len <= 0 Then Return If(blen <= 0, 0, -1)
			If len > blen Then Return 1
			If len < blen - 1 Then Return -1
			Dim bval As Integer() = b.value
			Dim bstart As Integer = 0
			Dim carry As Integer = 0
			' Only 2 cases left:len == blen or len == blen - 1
			If len <> blen Then ' len == blen - 1
				If bval(bstart) = 1 Then
					bstart += 1
					carry = &H80000000L
				Else
					Return -1
				End If
			End If
			' compare values with right-shifted values of b,
			' carrying shifted-out bits across words
			Dim val As Integer() = value
			Dim i As Integer = offset
			Dim j As Integer = bstart
			Do While i < len + offset
				Dim bv As Integer = bval(j)
				j += 1
				Dim hb As Long = ((CInt(CUInt(bv) >> 1)) + carry) And LONG_MASK
				Dim v As Long = val(i) And LONG_MASK
				i += 1
				If v <> hb Then Return If(v < hb, -1, 1)
				carry = (bv And 1) << 31 ' carray will be either 0x80000000 or 0
			Loop
			Return If(carry = 0, 0, -1)
		End Function

		''' <summary>
		''' Return the index of the lowest set bit in this MutableBigInteger. If the
		''' magnitude of this MutableBigInteger is zero, -1 is returned.
		''' </summary>
		Private Property lowestSetBit As Integer
			Get
				If intLen = 0 Then Return -1
				Dim j, b As Integer
				j=intLen-1
				Do While (j > 0) AndAlso (value(j+offset) = 0)
    
					j -= 1
				Loop
				b = value(j+offset)
				If b = 0 Then Return -1
				Return ((intLen-1-j)<<5) + Integer.numberOfTrailingZeros(b)
			End Get
		End Property

		''' <summary>
		''' Return the int in use in this MutableBigInteger at the specified
		''' index. This method is not used because it is not inlined on all
		''' platforms.
		''' </summary>
		Private Function getInt(ByVal index As Integer) As Integer
			Return value(offset+index)
		End Function

		''' <summary>
		''' Return a long which is equal to the unsigned value of the int in
		''' use in this MutableBigInteger at the specified index. This method is
		''' not used because it is not inlined on all platforms.
		''' </summary>
		Private Function getLong(ByVal index As Integer) As Long
			Return value(offset+index) And LONG_MASK
		End Function

		''' <summary>
		''' Ensure that the MutableBigInteger is in normal form, specifically
		''' making sure that there are no leading zeros, and that if the
		''' magnitude is zero, then intLen is zero.
		''' </summary>
		Friend Sub normalize()
			If intLen = 0 Then
				offset = 0
				Return
			End If

			Dim index As Integer = offset
			If value(index) <> 0 Then Return

			Dim indexBound As Integer = index+intLen
			Do
				index += 1
			Loop While index < indexBound AndAlso value(index) = 0

			Dim numZeros As Integer = index - offset
			intLen -= numZeros
			offset = (If(intLen = 0, 0, offset+numZeros))
		End Sub

		''' <summary>
		''' If this MutableBigInteger cannot hold len words, increase the size
		''' of the value array to len words.
		''' </summary>
		Private Sub ensureCapacity(ByVal len As Integer)
			If value.Length < len Then
				value = New Integer(len - 1){}
				offset = 0
				intLen = len
			End If
		End Sub

		''' <summary>
		''' Convert this MutableBigInteger into an int array with no leading
		''' zeros, of a length that is equal to this MutableBigInteger's intLen.
		''' </summary>
		Friend Overridable Function toIntArray() As Integer()
			Dim result As Integer() = New Integer(intLen - 1){}
			Dim i As Integer=0
			Do While i < intLen
				result(i) = value(offset+i)
				i += 1
			Loop
			Return result
		End Function

		''' <summary>
		''' Sets the int at index+offset in this MutableBigInteger to val.
		''' This does not get inlined on all platforms so it is not used
		''' as often as originally intended.
		''' </summary>
		Friend Overridable Sub setInt(ByVal index As Integer, ByVal val As Integer)
			value(offset + index) = val
		End Sub

		''' <summary>
		''' Sets this MutableBigInteger's value array to the specified array.
		''' The intLen is set to the specified length.
		''' </summary>
		Friend Overridable Sub setValue(ByVal val As Integer(), ByVal length As Integer)
			value = val
			intLen = length
			offset = 0
		End Sub

		''' <summary>
		''' Sets this MutableBigInteger's value array to a copy of the specified
		''' array. The intLen is set to the length of the new array.
		''' </summary>
		Friend Overridable Sub copyValue(ByVal src As MutableBigInteger)
			Dim len As Integer = src.intLen
			If value.Length < len Then value = New Integer(len - 1){}
			Array.Copy(src.value, src.offset, value, 0, len)
			intLen = len
			offset = 0
		End Sub

		''' <summary>
		''' Sets this MutableBigInteger's value array to a copy of the specified
		''' array. The intLen is set to the length of the specified array.
		''' </summary>
		Friend Overridable Sub copyValue(ByVal val As Integer())
			Dim len As Integer = val.Length
			If value.Length < len Then value = New Integer(len - 1){}
			Array.Copy(val, 0, value, 0, len)
			intLen = len
			offset = 0
		End Sub

		''' <summary>
		''' Returns true iff this MutableBigInteger has a value of one.
		''' </summary>
		Friend Overridable Property one As Boolean
			Get
				Return (intLen = 1) AndAlso (value(offset) = 1)
			End Get
		End Property

		''' <summary>
		''' Returns true iff this MutableBigInteger has a value of zero.
		''' </summary>
		Friend Overridable Property zero As Boolean
			Get
				Return (intLen = 0)
			End Get
		End Property

		''' <summary>
		''' Returns true iff this MutableBigInteger is even.
		''' </summary>
		Friend Overridable Property even As Boolean
			Get
				Return (intLen = 0) OrElse ((value(offset + intLen - 1) And 1) = 0)
			End Get
		End Property

		''' <summary>
		''' Returns true iff this MutableBigInteger is odd.
		''' </summary>
		Friend Overridable Property odd As Boolean
			Get
				Return If(zero, False, ((value(offset + intLen - 1) And 1) = 1))
			End Get
		End Property

		''' <summary>
		''' Returns true iff this MutableBigInteger is in normal form. A
		''' MutableBigInteger is in normal form if it has no leading zeros
		''' after the offset, and intLen + offset <= value.length.
		''' </summary>
		Friend Overridable Property normal As Boolean
			Get
				If intLen + offset > value.Length Then Return False
				If intLen = 0 Then Return True
				Return (value(offset) <> 0)
			End Get
		End Property

		''' <summary>
		''' Returns a String representation of this MutableBigInteger in radix 10.
		''' </summary>
		Public Overrides Function ToString() As String
			Dim b As BigInteger = toBigInteger(1)
			Return b.ToString()
		End Function

		''' <summary>
		''' Like <seealso cref="#rightShift(int)"/> but {@code n} can be greater than the length of the number.
		''' </summary>
		Friend Overridable Sub safeRightShift(ByVal n As Integer)
			If n\32 >= intLen Then
				reset()
			Else
				rightShift(n)
			End If
		End Sub

		''' <summary>
		''' Right shift this MutableBigInteger n bits. The MutableBigInteger is left
		''' in normal form.
		''' </summary>
		Friend Overridable Sub rightShift(ByVal n As Integer)
			If intLen = 0 Then Return
			Dim nInts As Integer = CInt(CUInt(n) >> 5)
			Dim nBits As Integer = n And &H1F
			Me.intLen -= nInts
			If nBits = 0 Then Return
			Dim bitsInHighWord As Integer = BigInteger.bitLengthForInt(value(offset))
			If nBits >= bitsInHighWord Then
				Me.primitiveLeftShift(32 - nBits)
				Me.intLen -= 1
			Else
				primitiveRightShift(nBits)
			End If
		End Sub

		''' <summary>
		''' Like <seealso cref="#leftShift(int)"/> but {@code n} can be zero.
		''' </summary>
		Friend Overridable Sub safeLeftShift(ByVal n As Integer)
			If n > 0 Then leftShift(n)
		End Sub

		''' <summary>
		''' Left shift this MutableBigInteger n bits.
		''' </summary>
		Friend Overridable Sub leftShift(ByVal n As Integer)
	'        
	'         * If there is enough storage space in this MutableBigInteger already
	'         * the available space will be used. Space to the right of the used
	'         * ints in the value array is faster to utilize, so the extra space
	'         * will be taken from the right if possible.
	'         
			If intLen = 0 Then Return
			Dim nInts As Integer = CInt(CUInt(n) >> 5)
			Dim nBits As Integer = n And &H1F
			Dim bitsInHighWord As Integer = BigInteger.bitLengthForInt(value(offset))

			' If shift can be done without moving words, do so
			If n <= (32-bitsInHighWord) Then
				primitiveLeftShift(nBits)
				Return
			End If

			Dim newLen As Integer = intLen + nInts +1
			If nBits <= (32-bitsInHighWord) Then newLen -= 1
			If value.Length < newLen Then
				' The array must grow
				Dim result As Integer() = New Integer(newLen - 1){}
				Dim i As Integer=0
				Do While i < intLen
					result(i) = value(offset+i)
					i += 1
				Loop
				valuelue(result, newLen)
			ElseIf value.Length - offset >= newLen Then
				' Use space on right
				For i As Integer = 0 To newLen - intLen - 1
					value(offset+intLen+i) = 0
				Next i
			Else
				' Must use space on left
				Dim i As Integer=0
				Do While i < intLen
					value(i) = value(offset+i)
					i += 1
				Loop
				For i As Integer = intLen To newLen - 1
					value(i) = 0
				Next i
				offset = 0
			End If
			intLen = newLen
			If nBits = 0 Then Return
			If nBits <= (32-bitsInHighWord) Then
				primitiveLeftShift(nBits)
			Else
				primitiveRightShift(32 -nBits)
			End If
		End Sub

		''' <summary>
		''' A primitive used for division. This method adds in one multiple of the
		''' divisor a back to the dividend result at a specified offset. It is used
		''' when qhat was estimated too large, and must be adjusted.
		''' </summary>
		Private Function divadd(ByVal a As Integer(), ByVal result As Integer(), ByVal offset As Integer) As Integer
			Dim carry As Long = 0

			For j As Integer = a.Length-1 To 0 Step -1
				Dim sum As Long = (a(j) And LONG_MASK) + (result(j+offset) And LONG_MASK) + carry
				result(j+offset) = CInt(sum)
				carry = CLng(CULng(sum) >> 32)
			Next j
			Return CInt(carry)
		End Function

		''' <summary>
		''' This method is used for division. It multiplies an n word input a by one
		''' word input x, and subtracts the n word product from q. This is needed
		''' when subtracting qhat*divisor from dividend.
		''' </summary>
		Private Function mulsub(ByVal q As Integer(), ByVal a As Integer(), ByVal x As Integer, ByVal len As Integer, ByVal offset As Integer) As Integer
			Dim xLong As Long = x And LONG_MASK
			Dim carry As Long = 0
			offset += len

			For j As Integer = len-1 To 0 Step -1
				Dim product As Long = (a(j) And LONG_MASK) * xLong + carry
				Dim difference As Long = q(offset) - product
				q(offset) = CInt(difference)
				offset -= 1
				carry = (CLng(CULng(product) >> 32)) + (If((difference And LONG_MASK) > ((((Not CInt(product))) And LONG_MASK)), 1, 0))
			Next j
			Return CInt(carry)
		End Function

		''' <summary>
		''' The method is the same as mulsun, except the fact that q array is not
		''' updated, the only result of the method is borrow flag.
		''' </summary>
		Private Function mulsubBorrow(ByVal q As Integer(), ByVal a As Integer(), ByVal x As Integer, ByVal len As Integer, ByVal offset As Integer) As Integer
			Dim xLong As Long = x And LONG_MASK
			Dim carry As Long = 0
			offset += len
			For j As Integer = len-1 To 0 Step -1
				Dim product As Long = (a(j) And LONG_MASK) * xLong + carry
				Dim difference As Long = q(offset) - product
				offset -= 1
				carry = (CLng(CULng(product) >> 32)) + (If((difference And LONG_MASK) > ((((Not CInt(product))) And LONG_MASK)), 1, 0))
			Next j
			Return CInt(carry)
		End Function

		''' <summary>
		''' Right shift this MutableBigInteger n bits, where n is
		''' less than 32.
		''' Assumes that intLen > 0, n > 0 for speed
		''' </summary>
		Private Sub primitiveRightShift(ByVal n As Integer)
			Dim val As Integer() = value
			Dim n2 As Integer = 32 - n
			Dim i As Integer=offset+intLen-1
			Dim c As Integer=val(i)
			Do While i > offset
				Dim b As Integer = c
				c = val(i-1)
				val(i) = (c << n2) Or (CInt(CUInt(b) >> n))
				i -= 1
			Loop
			val(offset) >>>= n
		End Sub

		''' <summary>
		''' Left shift this MutableBigInteger n bits, where n is
		''' less than 32.
		''' Assumes that intLen > 0, n > 0 for speed
		''' </summary>
		Private Sub primitiveLeftShift(ByVal n As Integer)
			Dim val As Integer() = value
			Dim n2 As Integer = 32 - n
			Dim i As Integer=offset
			Dim c As Integer=val(i)
			Dim m As Integer=i+intLen-1
			Do While i < m
				Dim b As Integer = c
				c = val(i+1)
				val(i) = (b << n) Or (CInt(CUInt(c) >> n2))
				i += 1
			Loop
			val(offset+intLen-1) <<= n
		End Sub

		''' <summary>
		''' Returns a {@code BigInteger} equal to the {@code n}
		''' low ints of this number.
		''' </summary>
		Private Function getLower(ByVal n As Integer) As BigInteger
			If zero Then
				Return BigInteger.ZERO
			ElseIf intLen < n Then
				Return toBigInteger(1)
			Else
				' strip zeros
				Dim len As Integer = n
				Do While len > 0 AndAlso value(offset+intLen-len) = 0
					len -= 1
				Loop
				Dim sign As Integer = If(len > 0, 1, 0)
				Return New BigInteger(java.util.Arrays.copyOfRange(value, offset+intLen-len, offset+intLen), sign)
			End If
		End Function

		''' <summary>
		''' Discards all ints whose index is greater than {@code n}.
		''' </summary>
		Private Sub keepLower(ByVal n As Integer)
			If intLen >= n Then
				offset += intLen - n
				intLen = n
			End If
		End Sub

		''' <summary>
		''' Adds the contents of two MutableBigInteger objects.The result
		''' is placed within this MutableBigInteger.
		''' The contents of the addend are not changed.
		''' </summary>
		Friend Overridable Sub add(ByVal addend As MutableBigInteger)
			Dim x As Integer = intLen
			Dim y As Integer = addend.intLen
			Dim resultLen As Integer = (If(intLen > addend.intLen, intLen, addend.intLen))
			Dim result As Integer() = (If(value.Length < resultLen, New Integer(resultLen - 1){}, value))

			Dim rstart As Integer = result.Length-1
			Dim sum As Long
			Dim carry As Long = 0

			' Add common parts of both numbers
			Do While x > 0 AndAlso y > 0
				x -= 1
				y -= 1
				sum = (value(x+offset) And LONG_MASK) + (addend.value(y+addend.offset) And LONG_MASK) + carry
				result(rstart) = CInt(sum)
				rstart -= 1
				carry = CLng(CULng(sum) >> 32)
			Loop

			' Add remainder of the longer number
			Do While x > 0
				x -= 1
				If carry = 0 AndAlso result = value AndAlso rstart = (x + offset) Then Return
				sum = (value(x+offset) And LONG_MASK) + carry
				result(rstart) = CInt(sum)
				rstart -= 1
				carry = CLng(CULng(sum) >> 32)
			Loop
			Do While y > 0
				y -= 1
				sum = (addend.value(y+addend.offset) And LONG_MASK) + carry
				result(rstart) = CInt(sum)
				rstart -= 1
				carry = CLng(CULng(sum) >> 32)
			Loop

			If carry > 0 Then ' Result must grow in length
				resultLen += 1
				If result.Length < resultLen Then
					Dim temp As Integer() = New Integer(resultLen - 1){}
					' Result one word longer from carry-out; copy low-order
					' bits into new result.
					Array.Copy(result, 0, temp, 1, result.Length)
					temp(0) = 1
					result = temp
				Else
					result(rstart) = 1
					rstart -= 1
				End If
			End If

			value = result
			intLen = resultLen
			offset = result.Length - resultLen
		End Sub

		''' <summary>
		''' Adds the value of {@code addend} shifted {@code n} ints to the left.
		''' Has the same effect as {@code addend.leftShift(32*ints); add(addend);}
		''' but doesn't change the value of {@code addend}.
		''' </summary>
		Friend Overridable Sub addShifted(ByVal addend As MutableBigInteger, ByVal n As Integer)
			If addend.zero Then Return

			Dim x As Integer = intLen
			Dim y As Integer = addend.intLen + n
			Dim resultLen As Integer = (If(intLen > y, intLen, y))
			Dim result As Integer() = (If(value.Length < resultLen, New Integer(resultLen - 1){}, value))

			Dim rstart As Integer = result.Length-1
			Dim sum As Long
			Dim carry As Long = 0

			' Add common parts of both numbers
			Do While x > 0 AndAlso y > 0
				x -= 1
				y -= 1
				Dim bval As Integer = If(y+addend.offset < addend.value.Length, addend.value(y+addend.offset), 0)
				sum = (value(x+offset) And LONG_MASK) + (bval And LONG_MASK) + carry
				result(rstart) = CInt(sum)
				rstart -= 1
				carry = CLng(CULng(sum) >> 32)
			Loop

			' Add remainder of the longer number
			Do While x > 0
				x -= 1
				If carry = 0 AndAlso result = value AndAlso rstart = (x + offset) Then Return
				sum = (value(x+offset) And LONG_MASK) + carry
				result(rstart) = CInt(sum)
				rstart -= 1
				carry = CLng(CULng(sum) >> 32)
			Loop
			Do While y > 0
				y -= 1
				Dim bval As Integer = If(y+addend.offset < addend.value.Length, addend.value(y+addend.offset), 0)
				sum = (bval And LONG_MASK) + carry
				result(rstart) = CInt(sum)
				rstart -= 1
				carry = CLng(CULng(sum) >> 32)
			Loop

			If carry > 0 Then ' Result must grow in length
				resultLen += 1
				If result.Length < resultLen Then
					Dim temp As Integer() = New Integer(resultLen - 1){}
					' Result one word longer from carry-out; copy low-order
					' bits into new result.
					Array.Copy(result, 0, temp, 1, result.Length)
					temp(0) = 1
					result = temp
				Else
					result(rstart) = 1
					rstart -= 1
				End If
			End If

			value = result
			intLen = resultLen
			offset = result.Length - resultLen
		End Sub

		''' <summary>
		''' Like <seealso cref="#addShifted(MutableBigInteger, int)"/> but {@code this.intLen} must
		''' not be greater than {@code n}. In other words, concatenates {@code this}
		''' and {@code addend}.
		''' </summary>
		Friend Overridable Sub addDisjoint(ByVal addend As MutableBigInteger, ByVal n As Integer)
			If addend.zero Then Return

			Dim x As Integer = intLen
			Dim y As Integer = addend.intLen + n
			Dim resultLen As Integer = (If(intLen > y, intLen, y))
			Dim result As Integer()
			If value.Length < resultLen Then
				result = New Integer(resultLen - 1){}
			Else
				result = value
				java.util.Arrays.fill(value, offset+intLen, value.Length, 0)
			End If

			Dim rstart As Integer = result.Length-1

			' copy from this if needed
			Array.Copy(value, offset, result, rstart+1-x, x)
			y -= x
			rstart -= x

			Dim len As Integer = Math.Min(y, addend.value.Length-addend.offset)
			Array.Copy(addend.value, addend.offset, result, rstart+1-y, len)

			' zero the gap
			For i As Integer = rstart+1-y+len To rstart
				result(i) = 0
			Next i

			value = result
			intLen = resultLen
			offset = result.Length - resultLen
		End Sub

		''' <summary>
		''' Adds the low {@code n} ints of {@code addend}.
		''' </summary>
		Friend Overridable Sub addLower(ByVal addend As MutableBigInteger, ByVal n As Integer)
			Dim a As New MutableBigInteger(addend)
			If a.offset + a.intLen >= n Then
				a.offset = a.offset + a.intLen - n
				a.intLen = n
			End If
			a.normalize()
			add(a)
		End Sub

		''' <summary>
		''' Subtracts the smaller of this and b from the larger and places the
		''' result into this MutableBigInteger.
		''' </summary>
		Friend Overridable Function subtract(ByVal b As MutableBigInteger) As Integer
			Dim a As MutableBigInteger = Me

			Dim result As Integer() = value
			Dim sign As Integer = a.compare(b)

			If sign = 0 Then
				reset()
				Return 0
			End If
			If sign < 0 Then
				Dim tmp As MutableBigInteger = a
				a = b
				b = tmp
			End If

			Dim resultLen As Integer = a.intLen
			If result.Length < resultLen Then result = New Integer(resultLen - 1){}

			Dim diff As Long = 0
			Dim x As Integer = a.intLen
			Dim y As Integer = b.intLen
			Dim rstart As Integer = result.Length - 1

			' Subtract common parts of both numbers
			Do While y > 0
				x -= 1
				y -= 1

				diff = (a.value(x+a.offset) And LONG_MASK) - (b.value(y+b.offset) And LONG_MASK) - (CInt(Fix(-(diff>>32))))
				result(rstart) = CInt(diff)
				rstart -= 1
			Loop
			' Subtract remainder of longer number
			Do While x > 0
				x -= 1
				diff = (a.value(x+a.offset) And LONG_MASK) - (CInt(Fix(-(diff>>32))))
				result(rstart) = CInt(diff)
				rstart -= 1
			Loop

			value = result
			intLen = resultLen
			offset = value.Length - resultLen
			normalize()
			Return sign
		End Function

		''' <summary>
		''' Subtracts the smaller of a and b from the larger and places the result
		''' into the larger. Returns 1 if the answer is in a, -1 if in b, 0 if no
		''' operation was performed.
		''' </summary>
		Private Function difference(ByVal b As MutableBigInteger) As Integer
			Dim a As MutableBigInteger = Me
			Dim sign As Integer = a.compare(b)
			If sign = 0 Then Return 0
			If sign < 0 Then
				Dim tmp As MutableBigInteger = a
				a = b
				b = tmp
			End If

			Dim diff As Long = 0
			Dim x As Integer = a.intLen
			Dim y As Integer = b.intLen

			' Subtract common parts of both numbers
			Do While y > 0
				x -= 1
				y -= 1
				diff = (a.value(a.offset+ x) And LONG_MASK) - (b.value(b.offset+ y) And LONG_MASK) - (CInt(Fix(-(diff>>32))))
				a.value(a.offset+x) = CInt(diff)
			Loop
			' Subtract remainder of longer number
			Do While x > 0
				x -= 1
				diff = (a.value(a.offset+ x) And LONG_MASK) - (CInt(Fix(-(diff>>32))))
				a.value(a.offset+x) = CInt(diff)
			Loop

			a.normalize()
			Return sign
		End Function

		''' <summary>
		''' Multiply the contents of two MutableBigInteger objects. The result is
		''' placed into MutableBigInteger z. The contents of y are not changed.
		''' </summary>
		Friend Overridable Sub multiply(ByVal y As MutableBigInteger, ByVal z As MutableBigInteger)
			Dim xLen As Integer = intLen
			Dim yLen As Integer = y.intLen
			Dim newLen As Integer = xLen + yLen

			' Put z into an appropriate state to receive product
			If z.value.Length < newLen Then z.value = New Integer(newLen - 1){}
			z.offset = 0
			z.intLen = newLen

			' The first iteration is hoisted out of the loop to avoid extra add
			Dim carry As Long = 0
			Dim j As Integer=yLen-1
			Dim k As Integer=yLen+xLen-1
			Do While j >= 0
					Dim product As Long = (y.value(j+y.offset) And LONG_MASK) * (value(xLen-1+offset) And LONG_MASK) + carry
					z.value(k) = CInt(product)
					carry = CLng(CULng(product) >> 32)
				j -= 1
				k -= 1
			Loop
			z.value(xLen-1) = CInt(carry)

			' Perform the multiplication word by word
			For i As Integer = xLen-2 To 0 Step -1
				carry = 0
				j = yLen-1
				k = yLen+i
				Do While j >= 0
					Dim product As Long = (y.value(j+y.offset) And LONG_MASK) * (value(i+offset) And LONG_MASK) + (z.value(k) And LONG_MASK) + carry
					z.value(k) = CInt(product)
					carry = CLng(CULng(product) >> 32)
					j -= 1
					k -= 1
				Loop
				z.value(i) = CInt(carry)
			Next i

			' Remove leading zeros from product
			z.normalize()
		End Sub

		''' <summary>
		''' Multiply the contents of this MutableBigInteger by the word y. The
		''' result is placed into z.
		''' </summary>
		Friend Overridable Sub mul(ByVal y As Integer, ByVal z As MutableBigInteger)
			If y = 1 Then
				z.copyValue(Me)
				Return
			End If

			If y = 0 Then
				z.clear()
				Return
			End If

			' Perform the multiplication word by word
			Dim ylong As Long = y And LONG_MASK
			Dim zval As Integer() = (If(z.value.Length < intLen+1, New Integer(intLen){}, z.value))
			Dim carry As Long = 0
			For i As Integer = intLen-1 To 0 Step -1
				Dim product As Long = ylong * (value(i+offset) And LONG_MASK) + carry
				zval(i+1) = CInt(product)
				carry = CLng(CULng(product) >> 32)
			Next i

			If carry = 0 Then
				z.offset = 1
				z.intLen = intLen
			Else
				z.offset = 0
				z.intLen = intLen + 1
				zval(0) = CInt(carry)
			End If
			z.value = zval
		End Sub

		 ''' <summary>
		 ''' This method is used for division of an n word dividend by a one word
		 ''' divisor. The quotient is placed into quotient. The one word divisor is
		 ''' specified by divisor.
		 ''' </summary>
		 ''' <returns> the remainder of the division is returned.
		 '''  </returns>
		Friend Overridable Function divideOneWord(ByVal divisor As Integer, ByVal quotient As MutableBigInteger) As Integer
			Dim divisorLong As Long = divisor And LONG_MASK

			' Special case of one word dividend
			If intLen = 1 Then
				Dim dividendValue As Long = value(offset) And LONG_MASK
				Dim q As Integer = CInt(dividendValue \ divisorLong)
				Dim r As Integer = CInt(dividendValue - q * divisorLong)
				quotient.value(0) = q
				quotient.intLen = If(q = 0, 0, 1)
				quotient.offset = 0
				Return r
			End If

			If quotient.value.Length < intLen Then quotient.value = New Integer(intLen - 1){}
			quotient.offset = 0
			quotient.intLen = intLen

			' Normalize the divisor
			Dim shift As Integer = Integer.numberOfLeadingZeros(divisor)

			Dim [rem] As Integer = value(offset)
			Dim remLong As Long = [rem] And LONG_MASK
			If remLong < divisorLong Then
				quotient.value(0) = 0
			Else
				quotient.value(0) = CInt(remLong \ divisorLong)
				[rem] = CInt(remLong - (quotient.value(0) * divisorLong))
				remLong = [rem] And LONG_MASK
			End If
			Dim xlen As Integer = intLen
			xlen -= 1
			Do While xlen > 0
				Dim dividendEstimate As Long = (remLong << 32) Or (value(offset + intLen - xlen) And LONG_MASK)
				Dim q As Integer
				If dividendEstimate >= 0 Then
					q = CInt(dividendEstimate \ divisorLong)
					[rem] = CInt(dividendEstimate - q * divisorLong)
				Else
					Dim tmp As Long = divWord(dividendEstimate, divisor)
					q = CInt(Fix(tmp And LONG_MASK))
					[rem] = CInt(CLng(CULng(tmp) >> 32))
				End If
				quotient.value(intLen - xlen) = q
				remLong = [rem] And LONG_MASK
				xlen -= 1
			Loop

			quotient.normalize()
			' Unnormalize
			If shift > 0 Then
				Return [rem] Mod divisor
			Else
				Return [rem]
			End If
		End Function

		''' <summary>
		''' Calculates the quotient of this div b and places the quotient in the
		''' provided MutableBigInteger objects and the remainder object is returned.
		''' 
		''' </summary>
		Friend Overridable Function divide(ByVal b As MutableBigInteger, ByVal quotient As MutableBigInteger) As MutableBigInteger
			Return divide(b,quotient,True)
		End Function

		Friend Overridable Function divide(ByVal b As MutableBigInteger, ByVal quotient As MutableBigInteger, ByVal needRemainder As Boolean) As MutableBigInteger
			If b.intLen < BigInteger.BURNIKEL_ZIEGLER_THRESHOLD OrElse intLen - b.intLen < BigInteger.BURNIKEL_ZIEGLER_OFFSET Then
				Return divideKnuth(b, quotient, needRemainder)
			Else
				Return divideAndRemainderBurnikelZiegler(b, quotient)
			End If
		End Function

		''' <seealso cref= #divideKnuth(MutableBigInteger, MutableBigInteger, boolean) </seealso>
		Friend Overridable Function divideKnuth(ByVal b As MutableBigInteger, ByVal quotient As MutableBigInteger) As MutableBigInteger
			Return divideKnuth(b,quotient,True)
		End Function

		''' <summary>
		''' Calculates the quotient of this div b and places the quotient in the
		''' provided MutableBigInteger objects and the remainder object is returned.
		''' 
		''' Uses Algorithm D in Knuth section 4.3.1.
		''' Many optimizations to that algorithm have been adapted from the Colin
		''' Plumb C library.
		''' It special cases one word divisors for speed. The content of b is not
		''' changed.
		''' 
		''' </summary>
		Friend Overridable Function divideKnuth(ByVal b As MutableBigInteger, ByVal quotient As MutableBigInteger, ByVal needRemainder As Boolean) As MutableBigInteger
			If b.intLen = 0 Then Throw New ArithmeticException("BigInteger divide by zero")

			' Dividend is zero
			If intLen = 0 Then
					quotient.offset = 0
					quotient.intLen = quotient.offset
				Return If(needRemainder, New MutableBigInteger, Nothing)
			End If

			Dim cmp As Integer = compare(b)
			' Dividend less than divisor
			If cmp < 0 Then
					quotient.offset = 0
					quotient.intLen = quotient.offset
				Return If(needRemainder, New MutableBigInteger(Me), Nothing)
			End If
			' Dividend equal to divisor
			If cmp = 0 Then
					quotient.intLen = 1
					quotient.value(0) = quotient.intLen
				quotient.offset = 0
				Return If(needRemainder, New MutableBigInteger, Nothing)
			End If

			quotient.clear()
			' Special case one word divisor
			If b.intLen = 1 Then
				Dim r As Integer = divideOneWord(b.value(b.offset), quotient)
				If needRemainder Then
					If r = 0 Then Return New MutableBigInteger
					Return New MutableBigInteger(r)
				Else
					Return Nothing
				End If
			End If

			' Cancel common powers of two if we're above the KNUTH_POW2_* thresholds
			If intLen >= KNUTH_POW2_THRESH_LEN Then
				Dim trailingZeroBits As Integer = Math.Min(lowestSetBit, b.lowestSetBit)
				If trailingZeroBits >= KNUTH_POW2_THRESH_ZEROS*32 Then
					Dim a As New MutableBigInteger(Me)
					b = New MutableBigInteger(b)
					a.rightShift(trailingZeroBits)
					b.rightShift(trailingZeroBits)
					Dim r As MutableBigInteger = a.divideKnuth(b, quotient)
					r.leftShift(trailingZeroBits)
					Return r
				End If
			End If

			Return divideMagnitude(b, quotient, needRemainder)
		End Function

		''' <summary>
		''' Computes {@code this/b} and {@code this%b} using the
		''' <a href="http://cr.yp.to/bib/1998/burnikel.ps"> Burnikel-Ziegler algorithm</a>.
		''' This method implements algorithm 3 from pg. 9 of the Burnikel-Ziegler paper.
		''' The parameter beta was chosen to b 2<sup>32</sup> so almost all shifts are
		''' multiples of 32 bits.<br/>
		''' {@code this} and {@code b} must be nonnegative. </summary>
		''' <param name="b"> the divisor </param>
		''' <param name="quotient"> output parameter for {@code this/b} </param>
		''' <returns> the remainder </returns>
		Friend Overridable Function divideAndRemainderBurnikelZiegler(ByVal b As MutableBigInteger, ByVal quotient As MutableBigInteger) As MutableBigInteger
			Dim r As Integer = intLen
			Dim s As Integer = b.intLen

			' Clear the quotient
				quotient.intLen = 0
				quotient.offset = quotient.intLen

			If r < s Then
				Return Me
			Else
				' Unlike Knuth division, we don't check for common powers of two here because
				' BZ already runs faster if both numbers contain powers of two and cancelling them has no
				' additional benefit.

				' step 1: let m = min{2^k | (2^k)*BURNIKEL_ZIEGLER_THRESHOLD > s}
				Dim m As Integer = 1 << (32-Integer.numberOfLeadingZeros(s\BigInteger.BURNIKEL_ZIEGLER_THRESHOLD))

				Dim j As Integer = (s+m-1) \ m ' step 2a: j = ceil(s/m)
				Dim n As Integer = j * m ' step 2b: block length in 32-bit units
				Dim n32 As Long = 32L * n ' block length in bits
				Dim sigma As Integer = CInt(Fix(Math.Max(0, n32 - b.bitLength()))) ' step 3: sigma = max{T | (2^T)*B < beta^n}
				Dim bShifted As New MutableBigInteger(b)
				bShifted.safeLeftShift(sigma) ' step 4a: shift b so its length is a multiple of n
				Dim aShifted As New MutableBigInteger(Me)
				aShifted.safeLeftShift(sigma) ' step 4b: shift a by the same amount

				' step 5: t is the number of blocks needed to accommodate a plus one additional bit
				Dim t As Integer = CInt((aShifted.bitLength()+n32) \ n32)
				If t < 2 Then t = 2

				' step 6: conceptually split a into blocks a[t-1], ..., a[0]
				Dim a1 As MutableBigInteger = aShifted.getBlock(t-1, t, n) ' the most significant block of a

				' step 7: z[t-2] = [a[t-1], a[t-2]]
				Dim z As MutableBigInteger = aShifted.getBlock(t-2, t, n) ' the second to most significant block
				z.addDisjoint(a1, n) ' z[t-2]

				' do schoolbook division on blocks, dividing 2-block numbers by 1-block numbers
				Dim qi As New MutableBigInteger
				Dim ri As MutableBigInteger
				For i As Integer = t-2 To 1 Step -1
					' step 8a: compute (qi,ri) such that z=b*qi+ri
					ri = z.divide2n1n(bShifted, qi)

					' step 8b: z = [ri, a[i-1]]
					z = aShifted.getBlock(i-1, t, n) ' a[i-1]
					z.addDisjoint(ri, n)
					quotient.addShifted(qi, i*n) ' update q (part of step 9)
				Next i
				' final iteration of step 8: do the loop one more time for i=0 but leave z unchanged
				ri = z.divide2n1n(bShifted, qi)
				quotient.add(qi)

				ri.rightShift(sigma) ' step 9: a and b were shifted, so shift back
				Return ri
			End If
		End Function

		''' <summary>
		''' This method implements algorithm 1 from pg. 4 of the Burnikel-Ziegler paper.
		''' It divides a 2n-digit number by a n-digit number.<br/>
		''' The parameter beta is 2<sup>32</sup> so all shifts are multiples of 32 bits.
		''' <br/>
		''' {@code this} must be a nonnegative number such that {@code this.bitLength() <= 2*b.bitLength()} </summary>
		''' <param name="b"> a positive number such that {@code b.bitLength()} is even </param>
		''' <param name="quotient"> output parameter for {@code this/b} </param>
		''' <returns> {@code this%b} </returns>
		Private Function divide2n1n(ByVal b As MutableBigInteger, ByVal quotient As MutableBigInteger) As MutableBigInteger
			Dim n As Integer = b.intLen

			' step 1: base case
			If n Mod 2 <> 0 OrElse n < BigInteger.BURNIKEL_ZIEGLER_THRESHOLD Then Return divideKnuth(b, quotient)

			' step 2: view this as [a1,a2,a3,a4] where each ai is n/2 ints or less
			Dim aUpper As New MutableBigInteger(Me)
			aUpper.safeRightShift(32*(n\2)) ' aUpper = [a1,a2,a3]
			keepLower(n\2) ' this = a4

			' step 3: q1=aUpper/b, r1=aUpper%b
			Dim q1 As New MutableBigInteger
			Dim r1 As MutableBigInteger = aUpper.divide3n2n(b, q1)

			' step 4: quotient=[r1,this]/b, r2=[r1,this]%b
			addDisjoint(r1, n\2) ' this = [r1,this]
			Dim r2 As MutableBigInteger = divide3n2n(b, quotient)

			' step 5: let quotient=[q1,quotient] and return r2
			quotient.addDisjoint(q1, n\2)
			Return r2
		End Function

		''' <summary>
		''' This method implements algorithm 2 from pg. 5 of the Burnikel-Ziegler paper.
		''' It divides a 3n-digit number by a 2n-digit number.<br/>
		''' The parameter beta is 2<sup>32</sup> so all shifts are multiples of 32 bits.<br/>
		''' <br/>
		''' {@code this} must be a nonnegative number such that {@code 2*this.bitLength() <= 3*b.bitLength()} </summary>
		''' <param name="quotient"> output parameter for {@code this/b} </param>
		''' <returns> {@code this%b} </returns>
		Private Function divide3n2n(ByVal b As MutableBigInteger, ByVal quotient As MutableBigInteger) As MutableBigInteger
			Dim n As Integer = b.intLen \ 2 ' half the length of b in ints

			' step 1: view this as [a1,a2,a3] where each ai is n ints or less; let a12=[a1,a2]
			Dim a12 As New MutableBigInteger(Me)
			a12.safeRightShift(32*n)

			' step 2: view b as [b1,b2] where each bi is n ints or less
			Dim b1 As New MutableBigInteger(b)
			b1.safeRightShift(n * 32)
			Dim b2 As BigInteger = b.getLower(n)

			Dim r As MutableBigInteger
			Dim d As MutableBigInteger
			If compareShifted(b, n) < 0 Then
				' step 3a: if a1<b1, let quotient=a12/b1 and r=a12%b1
				r = a12.divide2n1n(b1, quotient)

				' step 4: d=quotient*b2
				d = New MutableBigInteger(quotient.toBigInteger().multiply(b2))
			Else
				' step 3b: if a1>=b1, let quotient=beta^n-1 and r=a12-b1*2^n+b1
				quotient.ones(n)
				a12.add(b1)
				b1.leftShift(32*n)
				a12.subtract(b1)
				r = a12

				' step 4: d=quotient*b2=(b2 << 32*n) - b2
				d = New MutableBigInteger(b2)
				d.leftShift(32 * n)
				d.subtract(New MutableBigInteger(b2))
			End If

			' step 5: r = r*beta^n + a3 - d (paper says a4)
			' However, don't subtract d until after the while loop so r doesn't become negative
			r.leftShift(32 * n)
			r.addLower(Me, n)

			' step 6: add b until r>=d
			Do While r.compare(d) < 0
				r.add(b)
				quotient.subtract(MutableBigInteger.ONE)
			Loop
			r.subtract(d)

			Return r
		End Function

		''' <summary>
		''' Returns a {@code MutableBigInteger} containing {@code blockLength} ints from
		''' {@code this} number, starting at {@code index*blockLength}.<br/>
		''' Used by Burnikel-Ziegler division. </summary>
		''' <param name="index"> the block index </param>
		''' <param name="numBlocks"> the total number of blocks in {@code this} number </param>
		''' <param name="blockLength"> length of one block in units of 32 bits
		''' @return </param>
		Private Function getBlock(ByVal index As Integer, ByVal numBlocks As Integer, ByVal blockLength As Integer) As MutableBigInteger
			Dim blockStart As Integer = index * blockLength
			If blockStart >= intLen Then Return New MutableBigInteger

			Dim blockEnd As Integer
			If index = numBlocks-1 Then
				blockEnd = intLen
			Else
				blockEnd = (index+1) * blockLength
			End If
			If blockEnd > intLen Then Return New MutableBigInteger

			Dim newVal As Integer() = java.util.Arrays.copyOfRange(value, offset+intLen-blockEnd, offset+intLen-blockStart)
			Return New MutableBigInteger(newVal)
		End Function

		''' <seealso cref= BigInteger#bitLength() </seealso>
		Friend Overridable Function bitLength() As Long
			If intLen = 0 Then Return 0
			Return intLen*32L - Integer.numberOfLeadingZeros(value(offset))
		End Function

		''' <summary>
		''' Internally used  to calculate the quotient of this div v and places the
		''' quotient in the provided MutableBigInteger object and the remainder is
		''' returned.
		''' </summary>
		''' <returns> the remainder of the division will be returned. </returns>
		Friend Overridable Function divide(ByVal v As Long, ByVal quotient As MutableBigInteger) As Long
			If v = 0 Then Throw New ArithmeticException("BigInteger divide by zero")

			' Dividend is zero
			If intLen = 0 Then
					quotient.offset = 0
					quotient.intLen = quotient.offset
				Return 0
			End If
			If v < 0 Then v = -v

			Dim d As Integer = CInt(CLng(CULng(v) >> 32))
			quotient.clear()
			' Special case on word divisor
			If d = 0 Then
				Return divideOneWord(CInt(v), quotient) And LONG_MASK
			Else
				Return divideLongMagnitude(v, quotient).toLong()
			End If
		End Function

		Private Shared Sub copyAndShift(ByVal src As Integer(), ByVal srcFrom As Integer, ByVal srcLen As Integer, ByVal dst As Integer(), ByVal dstFrom As Integer, ByVal shift As Integer)
			Dim n2 As Integer = 32 - shift
			Dim c As Integer=src(srcFrom)
			For i As Integer = 0 To srcLen-2
				Dim b As Integer = c
				srcFrom += 1
				c = src(srcFrom)
				dst(dstFrom+i) = (b << shift) Or (CInt(CUInt(c) >> n2))
			Next i
			dst(dstFrom+srcLen-1) = c << shift
		End Sub

		''' <summary>
		''' Divide this MutableBigInteger by the divisor.
		''' The quotient will be placed into the provided quotient object &
		''' the remainder object is returned.
		''' </summary>
		Private Function divideMagnitude(ByVal div As MutableBigInteger, ByVal quotient As MutableBigInteger, ByVal needRemainder As Boolean) As MutableBigInteger
			' assert div.intLen > 1
			' D1 normalize the divisor
			Dim shift As Integer = Integer.numberOfLeadingZeros(div.value(div.offset))
			' Copy divisor value to protect divisor
			Dim dlen As Integer = div.intLen
			Dim divisor As Integer()
			Dim [rem] As MutableBigInteger ' Remainder starts as dividend with space for a leading zero
			If shift > 0 Then
				divisor = New Integer(dlen - 1){}
				copyAndShift(div.value,div.offset,dlen,divisor,0,shift)
				If Integer.numberOfLeadingZeros(value(offset)) >= shift Then
					Dim remarr As Integer() = New Integer(intLen){}
					[rem] = New MutableBigInteger(remarr)
					[rem].intLen = intLen
					[rem].offset = 1
					copyAndShift(value,offset,intLen,remarr,1,shift)
				Else
					Dim remarr As Integer() = New Integer(intLen + 2 - 1){}
					[rem] = New MutableBigInteger(remarr)
					[rem].intLen = intLen+1
					[rem].offset = 1
					Dim rFrom As Integer = offset
					Dim c As Integer=0
					Dim n2 As Integer = 32 - shift
					Dim i As Integer=1
					Do While i < intLen+1
						Dim b As Integer = c
						c = value(rFrom)
						remarr(i) = (b << shift) Or (CInt(CUInt(c) >> n2))
						i += 1
						rFrom += 1
					Loop
					remarr(intLen+1) = c << shift
				End If
			Else
				divisor = java.util.Arrays.copyOfRange(div.value, div.offset, div.offset + div.intLen)
				[rem] = New MutableBigInteger(New Integer(intLen){})
				Array.Copy(value, offset, [rem].value, 1, intLen)
				[rem].intLen = intLen
				[rem].offset = 1
			End If

			Dim nlen As Integer = [rem].intLen

			' Set the quotient size
			Dim limit As Integer = nlen - dlen + 1
			If quotient.value.Length < limit Then
				quotient.value = New Integer(limit - 1){}
				quotient.offset = 0
			End If
			quotient.intLen = limit
			Dim q As Integer() = quotient.value


			' Must insert leading 0 in rem if its length did not change
			If [rem].intLen = nlen Then
				[rem].offset = 0
				[rem].value(0) = 0
				[rem].intLen += 1
			End If

			Dim dh As Integer = divisor(0)
			Dim dhLong As Long = dh And LONG_MASK
			Dim dl As Integer = divisor(1)

			' D2 Initialize j
			For j As Integer = 0 To limit-2
				' D3 Calculate qhat
				' estimate qhat
				Dim qhat As Integer = 0
				Dim qrem As Integer = 0
				Dim skipCorrection As Boolean = False
				Dim nh As Integer = [rem].value(j+[rem].offset)
				Dim nh2 As Integer = nh + &H80000000L
				Dim nm As Integer = [rem].value(j+1+[rem].offset)

				If nh = dh Then
					qhat = Not 0
					qrem = nh + nm
					skipCorrection = qrem + &H80000000L < nh2
				Else
					Dim nChunk As Long = ((CLng(nh)) << 32) Or (nm And LONG_MASK)
					If nChunk >= 0 Then
						qhat = CInt(nChunk \ dhLong)
						qrem = CInt(nChunk - (qhat * dhLong))
					Else
						Dim tmp As Long = divWord(nChunk, dh)
						qhat = CInt(Fix(tmp And LONG_MASK))
						qrem = CInt(CLng(CULng(tmp) >> 32))
					End If
				End If

				If qhat = 0 Then Continue For

				If Not skipCorrection Then ' Correct qhat
					Dim nl As Long = [rem].value(j+2+[rem].offset) And LONG_MASK
					Dim rs As Long = ((qrem And LONG_MASK) << 32) Or nl
					Dim estProduct As Long = (dl And LONG_MASK) * (qhat And LONG_MASK)

					If unsignedLongCompare(estProduct, rs) Then
						qhat -= 1
						qrem = CInt(Fix((qrem And LONG_MASK) + dhLong))
						If (qrem And LONG_MASK) >= dhLong Then
							estProduct -= (dl And LONG_MASK)
							rs = ((qrem And LONG_MASK) << 32) Or nl
							If unsignedLongCompare(estProduct, rs) Then qhat -= 1
						End If
					End If
				End If

				' D4 Multiply and subtract
				[rem].value(j+[rem].offset) = 0
				Dim borrow As Integer = mulsub([rem].value, divisor, qhat, dlen, j+[rem].offset)

				' D5 Test remainder
				If borrow + &H80000000L > nh2 Then
					' D6 Add back
					divadd(divisor, [rem].value, j+1+[rem].offset)
					qhat -= 1
				End If

				' Store the quotient digit
				q(j) = qhat
			Next j ' D7 loop on j
			' D3 Calculate qhat
			' estimate qhat
			Dim qhat As Integer = 0
			Dim qrem As Integer = 0
			Dim skipCorrection As Boolean = False
			Dim nh As Integer = [rem].value(limit - 1 + [rem].offset)
			Dim nh2 As Integer = nh + &H80000000L
			Dim nm As Integer = [rem].value(limit + [rem].offset)

			If nh = dh Then
				qhat = Not 0
				qrem = nh + nm
				skipCorrection = qrem + &H80000000L < nh2
			Else
				Dim nChunk As Long = ((CLng(nh)) << 32) Or (nm And LONG_MASK)
				If nChunk >= 0 Then
					qhat = CInt(nChunk \ dhLong)
					qrem = CInt(nChunk - (qhat * dhLong))
				Else
					Dim tmp As Long = divWord(nChunk, dh)
					qhat = CInt(Fix(tmp And LONG_MASK))
					qrem = CInt(CLng(CULng(tmp) >> 32))
				End If
			End If
			If qhat <> 0 Then
				If Not skipCorrection Then ' Correct qhat
					Dim nl As Long = [rem].value(limit + 1 + [rem].offset) And LONG_MASK
					Dim rs As Long = ((qrem And LONG_MASK) << 32) Or nl
					Dim estProduct As Long = (dl And LONG_MASK) * (qhat And LONG_MASK)

					If unsignedLongCompare(estProduct, rs) Then
						qhat -= 1
						qrem = CInt(Fix((qrem And LONG_MASK) + dhLong))
						If (qrem And LONG_MASK) >= dhLong Then
							estProduct -= (dl And LONG_MASK)
							rs = ((qrem And LONG_MASK) << 32) Or nl
							If unsignedLongCompare(estProduct, rs) Then qhat -= 1
						End If
					End If
				End If


				' D4 Multiply and subtract
				Dim borrow As Integer
				[rem].value(limit - 1 + [rem].offset) = 0
				If needRemainder Then
					borrow = mulsub([rem].value, divisor, qhat, dlen, limit - 1 + [rem].offset)
				Else
					borrow = mulsubBorrow([rem].value, divisor, qhat, dlen, limit - 1 + [rem].offset)
				End If

				' D5 Test remainder
				If borrow + &H80000000L > nh2 Then
					' D6 Add back
					If needRemainder Then divadd(divisor, [rem].value, limit - 1 + 1 + [rem].offset)
					qhat -= 1
				End If

				' Store the quotient digit
				q((limit - 1)) = qhat
			End If


			If needRemainder Then
				' D8 Unnormalize
				If shift > 0 Then [rem].rightShift(shift)
				[rem].normalize()
			End If
			quotient.normalize()
			Return If(needRemainder, [rem], Nothing)
		End Function

		''' <summary>
		''' Divide this MutableBigInteger by the divisor represented by positive long
		''' value. The quotient will be placed into the provided quotient object &
		''' the remainder object is returned.
		''' </summary>
		Private Function divideLongMagnitude(ByVal ldivisor As Long, ByVal quotient As MutableBigInteger) As MutableBigInteger
			' Remainder starts as dividend with space for a leading zero
			Dim [rem] As New MutableBigInteger(New Integer(intLen){})
			Array.Copy(value, offset, [rem].value, 1, intLen)
			[rem].intLen = intLen
			[rem].offset = 1

			Dim nlen As Integer = [rem].intLen

			Dim limit As Integer = nlen - 2 + 1
			If quotient.value.Length < limit Then
				quotient.value = New Integer(limit - 1){}
				quotient.offset = 0
			End If
			quotient.intLen = limit
			Dim q As Integer() = quotient.value

			' D1 normalize the divisor
			Dim shift As Integer = Long.numberOfLeadingZeros(ldivisor)
			If shift > 0 Then
				ldivisor<<=shift
				[rem].leftShift(shift)
			End If

			' Must insert leading 0 in rem if its length did not change
			If [rem].intLen = nlen Then
				[rem].offset = 0
				[rem].value(0) = 0
				[rem].intLen += 1
			End If

			Dim dh As Integer = CInt(CLng(CULng(ldivisor) >> 32))
			Dim dhLong As Long = dh And LONG_MASK
			Dim dl As Integer = CInt(Fix(ldivisor And LONG_MASK))

			' D2 Initialize j
			For j As Integer = 0 To limit - 1
				' D3 Calculate qhat
				' estimate qhat
				Dim qhat As Integer = 0
				Dim qrem As Integer = 0
				Dim skipCorrection As Boolean = False
				Dim nh As Integer = [rem].value(j + [rem].offset)
				Dim nh2 As Integer = nh + &H80000000L
				Dim nm As Integer = [rem].value(j + 1 + [rem].offset)

				If nh = dh Then
					qhat = Not 0
					qrem = nh + nm
					skipCorrection = qrem + &H80000000L < nh2
				Else
					Dim nChunk As Long = ((CLng(nh)) << 32) Or (nm And LONG_MASK)
					If nChunk >= 0 Then
						qhat = CInt(nChunk \ dhLong)
						qrem = CInt(nChunk - (qhat * dhLong))
					Else
						Dim tmp As Long = divWord(nChunk, dh)
						qhat =CInt(Fix(tmp And LONG_MASK))
						qrem = CInt(CLng(CULng(tmp)>>32))
					End If
				End If

				If qhat = 0 Then Continue For

				If Not skipCorrection Then ' Correct qhat
					Dim nl As Long = [rem].value(j + 2 + [rem].offset) And LONG_MASK
					Dim rs As Long = ((qrem And LONG_MASK) << 32) Or nl
					Dim estProduct As Long = (dl And LONG_MASK) * (qhat And LONG_MASK)

					If unsignedLongCompare(estProduct, rs) Then
						qhat -= 1
						qrem = CInt(Fix((qrem And LONG_MASK) + dhLong))
						If (qrem And LONG_MASK) >= dhLong Then
							estProduct -= (dl And LONG_MASK)
							rs = ((qrem And LONG_MASK) << 32) Or nl
							If unsignedLongCompare(estProduct, rs) Then qhat -= 1
						End If
					End If
				End If

				' D4 Multiply and subtract
				[rem].value(j + [rem].offset) = 0
				Dim borrow As Integer = mulsubLong([rem].value, dh, dl, qhat, j + [rem].offset)

				' D5 Test remainder
				If borrow + &H80000000L > nh2 Then
					' D6 Add back
					divaddLong(dh,dl, [rem].value, j + 1 + [rem].offset)
					qhat -= 1
				End If

				' Store the quotient digit
				q(j) = qhat
			Next j ' D7 loop on j

			' D8 Unnormalize
			If shift > 0 Then [rem].rightShift(shift)

			quotient.normalize()
			[rem].normalize()
			Return [rem]
		End Function

		''' <summary>
		''' A primitive used for division by long.
		''' Specialized version of the method divadd.
		''' dh is a high part of the divisor, dl is a low part
		''' </summary>
		Private Function divaddLong(ByVal dh As Integer, ByVal dl As Integer, ByVal result As Integer(), ByVal offset As Integer) As Integer
			Dim carry As Long = 0

			Dim sum As Long = (dl And LONG_MASK) + (result(1+offset) And LONG_MASK)
			result(1+offset) = CInt(sum)

			sum = (dh And LONG_MASK) + (result(offset) And LONG_MASK) + carry
			result(offset) = CInt(sum)
			carry = CLng(CULng(sum) >> 32)
			Return CInt(carry)
		End Function

		''' <summary>
		''' This method is used for division by long.
		''' Specialized version of the method sulsub.
		''' dh is a high part of the divisor, dl is a low part
		''' </summary>
		Private Function mulsubLong(ByVal q As Integer(), ByVal dh As Integer, ByVal dl As Integer, ByVal x As Integer, ByVal offset As Integer) As Integer
			Dim xLong As Long = x And LONG_MASK
			offset += 2
			Dim product As Long = (dl And LONG_MASK) * xLong
			Dim difference As Long = q(offset) - product
			q(offset) = CInt(difference)
			offset -= 1
			Dim carry As Long = (CLng(CULng(product) >> 32)) + (If((difference And LONG_MASK) > ((((Not CInt(product))) And LONG_MASK)), 1, 0))
			product = (dh And LONG_MASK) * xLong + carry
			difference = q(offset) - product
			q(offset) = CInt(difference)
			offset -= 1
			carry = (CLng(CULng(product) >> 32)) + (If((difference And LONG_MASK) > ((((Not CInt(product))) And LONG_MASK)), 1, 0))
			Return CInt(carry)
		End Function

		''' <summary>
		''' Compare two longs as if they were unsigned.
		''' Returns true iff one is bigger than two.
		''' </summary>
		Private Function unsignedLongCompare(ByVal one As Long, ByVal two As Long) As Boolean
			Return (one+Long.MinValue) > (two+Long.MinValue)
		End Function

		''' <summary>
		''' This method divides a long quantity by an int to estimate
		''' qhat for two multi precision numbers. It is used when
		''' the signed value of n is less than zero.
		''' Returns long value where high 32 bits contain remainder value and
		''' low 32 bits contain quotient value.
		''' </summary>
		Friend Shared Function divWord(ByVal n As Long, ByVal d As Integer) As Long
			Dim dLong As Long = d And LONG_MASK
			Dim r As Long
			Dim q As Long
			If dLong = 1 Then
				q = CInt(n)
				r = 0
				Return (r << 32) Or (q And LONG_MASK)
			End If

			' Approximate the quotient and remainder
			q = (CLng(CULng(n) >> 1)) \ (CLng(CULng(dLong) >> 1))
			r = n - q*dLong

			' Correct the approximation
			Do While r < 0
				r += dLong
				q -= 1
			Loop
			Do While r >= dLong
				r -= dLong
				q += 1
			Loop
			' n - q*dlong == r && 0 <= r <dLong, hence we're done.
			Return (r << 32) Or (q And LONG_MASK)
		End Function

		''' <summary>
		''' Calculate GCD of this and b. This and b are changed by the computation.
		''' </summary>
		Friend Overridable Function hybridGCD(ByVal b As MutableBigInteger) As MutableBigInteger
			' Use Euclid's algorithm until the numbers are approximately the
			' same length, then use the binary GCD algorithm to find the GCD.
			Dim a As MutableBigInteger = Me
			Dim q As New MutableBigInteger

			Do While b.intLen <> 0
				If Math.Abs(a.intLen - b.intLen) < 2 Then Return a.binaryGCD(b)

				Dim r As MutableBigInteger = a.divide(b, q)
				a = b
				b = r
			Loop
			Return a
		End Function

		''' <summary>
		''' Calculate GCD of this and v.
		''' Assumes that this and v are not zero.
		''' </summary>
		Private Function binaryGCD(ByVal v As MutableBigInteger) As MutableBigInteger
			' Algorithm B from Knuth section 4.5.2
			Dim u As MutableBigInteger = Me
			Dim r As New MutableBigInteger

			' step B1
			Dim s1 As Integer = u.lowestSetBit
			Dim s2 As Integer = v.lowestSetBit
			Dim k As Integer = If(s1 < s2, s1, s2)
			If k <> 0 Then
				u.rightShift(k)
				v.rightShift(k)
			End If

			' step B2
			Dim uOdd As Boolean = (k = s1)
			Dim t As MutableBigInteger = If(uOdd, v, u)
			Dim tsign As Integer = If(uOdd, -1, 1)

			Dim lb As Integer
			lb = t.lowestSetBit
			Do While lb >= 0
				' steps B3 and B4
				t.rightShift(lb)
				' step B5
				If tsign > 0 Then
					u = t
				Else
					v = t
				End If

				' Special case one word numbers
				If u.intLen < 2 AndAlso v.intLen < 2 Then
					Dim x As Integer = u.value(u.offset)
					Dim y As Integer = v.value(v.offset)
					x = binaryGcd(x, y)
					r.value(0) = x
					r.intLen = 1
					r.offset = 0
					If k > 0 Then r.leftShift(k)
					Return r
				End If

				' step B6
				tsign = u.difference(v)
				If tsign = 0 Then Exit Do
				t = If(tsign >= 0, u, v)
				lb = t.lowestSetBit
			Loop

			If k > 0 Then u.leftShift(k)
			Return u
		End Function

		''' <summary>
		''' Calculate GCD of a and b interpreted as unsigned integers.
		''' </summary>
		Friend Shared Function binaryGcd(ByVal a As Integer, ByVal b As Integer) As Integer
			If b = 0 Then Return a
			If a = 0 Then Return b

			' Right shift a & b till their last bits equal to 1.
			Dim aZeros As Integer = Integer.numberOfTrailingZeros(a)
			Dim bZeros As Integer = Integer.numberOfTrailingZeros(b)
			a >>>= aZeros
			b >>>= bZeros

			Dim t As Integer = (If(aZeros < bZeros, aZeros, bZeros))

			Do While a <> b
				If (a+&H80000000L) > (b+&H80000000L) Then ' a > b as unsigned
					a -= b
					a >>>= Integer.numberOfTrailingZeros(a)
				Else
					b -= a
					b >>>= Integer.numberOfTrailingZeros(b)
				End If
			Loop
			Return a<<t
		End Function

		''' <summary>
		''' Returns the modInverse of this mod p. This and p are not affected by
		''' the operation.
		''' </summary>
		Friend Overridable Function mutableModInverse(ByVal p As MutableBigInteger) As MutableBigInteger
			' Modulus is odd, use Schroeppel's algorithm
			If p.odd Then Return modInverse(p)

			' Base and modulus are even, throw exception
			If even Then Throw New ArithmeticException("BigInteger not invertible.")

			' Get even part of modulus expressed as a power of 2
			Dim powersOf2 As Integer = p.lowestSetBit

			' Construct odd part of modulus
			Dim oddMod As New MutableBigInteger(p)
			oddMod.rightShift(powersOf2)

			If oddMod.one Then Return modInverseMP2(powersOf2)

			' Calculate 1/a mod oddMod
			Dim oddPart As MutableBigInteger = modInverse(oddMod)

			' Calculate 1/a mod evenMod
			Dim evenPart As MutableBigInteger = modInverseMP2(powersOf2)

			' Combine the results using Chinese Remainder Theorem
			Dim y1 As MutableBigInteger = modInverseBP2(oddMod, powersOf2)
			Dim y2 As MutableBigInteger = oddMod.modInverseMP2(powersOf2)

			Dim temp1 As New MutableBigInteger
			Dim temp2 As New MutableBigInteger
			Dim result As New MutableBigInteger

			oddPart.leftShift(powersOf2)
			oddPart.multiply(y1, result)

			evenPart.multiply(oddMod, temp1)
			temp1.multiply(y2, temp2)

			result.add(temp2)
			Return result.divide(p, temp1)
		End Function

	'    
	'     * Calculate the multiplicative inverse of this mod 2^k.
	'     
		Friend Overridable Function modInverseMP2(ByVal k As Integer) As MutableBigInteger
			If even Then Throw New ArithmeticException("Non-invertible. (GCD != 1)")

			If k > 64 Then Return euclidModInverse(k)

			Dim t As Integer = inverseMod32(value(offset+intLen-1))

			If k < 33 Then
				t = (If(k = 32, t, t And ((1 << k) - 1)))
				Return New MutableBigInteger(t)
			End If

			Dim pLong As Long = (value(offset+intLen-1) And LONG_MASK)
			If intLen > 1 Then pLong = pLong Or (CLng(value(offset+intLen-2)) << 32)
			Dim tLong As Long = t And LONG_MASK
			tLong = tLong * (2 - pLong * tLong) ' 1 more Newton iter step
			tLong = (If(k = 64, tLong, tLong And ((1L << k) - 1)))

			Dim result As New MutableBigInteger(New Integer(1){})
			result.value(0) = CInt(CLng(CULng(tLong) >> 32))
			result.value(1) = CInt(tLong)
			result.intLen = 2
			result.normalize()
			Return result
		End Function

		''' <summary>
		''' Returns the multiplicative inverse of val mod 2^32.  Assumes val is odd.
		''' </summary>
		Friend Shared Function inverseMod32(ByVal val As Integer) As Integer
			' Newton's iteration!
			Dim t As Integer = val
			t *= 2 - val*t
			t *= 2 - val*t
			t *= 2 - val*t
			t *= 2 - val*t
			Return t
		End Function

		''' <summary>
		''' Calculate the multiplicative inverse of 2^k mod mod, where mod is odd.
		''' </summary>
		Shared Function modInverseBP2(ByVal [mod] As MutableBigInteger, ByVal k As Integer) As MutableBigInteger
			' Copy the mod to protect original
			Return fixup(New MutableBigInteger(1), New MutableBigInteger([mod]), k)
		End Function

		''' <summary>
		''' Calculate the multiplicative inverse of this mod mod, where mod is odd.
		''' This and mod are not changed by the calculation.
		''' 
		''' This method implements an algorithm due to Richard Schroeppel, that uses
		''' the same intermediate representation as Montgomery Reduction
		''' ("Montgomery Form").  The algorithm is described in an unpublished
		''' manuscript entitled "Fast Modular Reciprocals."
		''' </summary>
		Private Function modInverse(ByVal [mod] As MutableBigInteger) As MutableBigInteger
			Dim p As New MutableBigInteger([mod])
			Dim f As New MutableBigInteger(Me)
			Dim g As New MutableBigInteger(p)
			Dim c As New SignedMutableBigInteger(1)
			Dim d As New SignedMutableBigInteger
			Dim temp As MutableBigInteger = Nothing
			Dim sTemp As SignedMutableBigInteger = Nothing

			Dim k As Integer = 0
			' Right shift f k times until odd, left shift d k times
			If f.even Then
				Dim trailingZeros As Integer = f.lowestSetBit
				f.rightShift(trailingZeros)
				d.leftShift(trailingZeros)
				k = trailingZeros
			End If

			' The Almost Inverse Algorithm
			Do While Not f.one
				' If gcd(f, g) != 1, number is not invertible modulo mod
				If f.zero Then Throw New ArithmeticException("BigInteger not invertible.")

				' If f < g exchange f, g and c, d
				If f.compare(g) < 0 Then
					temp = f
					f = g
					g = temp
					sTemp = d
					d = c
					c = sTemp
				End If

				' If f == g (mod 4)
				If ((f.value(f.offset + f.intLen - 1) Xor g.value(g.offset + g.intLen - 1)) And 3) = 0 Then
					f.subtract(g)
					c.signedSubtract(d) ' If f != g (mod 4)
				Else
					f.add(g)
					c.signedAdd(d)
				End If

				' Right shift f k times until odd, left shift d k times
				Dim trailingZeros As Integer = f.lowestSetBit
				f.rightShift(trailingZeros)
				d.leftShift(trailingZeros)
				k += trailingZeros
			Loop

			Do While c.sign < 0
			   c.signedAdd(p)
			Loop

			Return fixup(c, p, k)
		End Function

		''' <summary>
		''' The Fixup Algorithm
		''' Calculates X such that X = C * 2^(-k) (mod P)
		''' Assumes C<P and P is odd.
		''' </summary>
		Shared Function fixup(ByVal c As MutableBigInteger, ByVal p As MutableBigInteger, ByVal k As Integer) As MutableBigInteger
			Dim temp As New MutableBigInteger
			' Set r to the multiplicative inverse of p mod 2^32
			Dim r As Integer = -inverseMod32(p.value(p.offset+p.intLen-1))

			Dim i As Integer=0
			Dim numWords As Integer = k >> 5
			Do While i < numWords
				' V = R * c (mod 2^j)
				Dim v As Integer = r * c.value(c.offset + c.intLen-1)
				' c = c + (v * p)
				p.mul(v, temp)
				c.add(temp)
				' c = c / 2^j
				c.intLen -= 1
				i += 1
			Loop
			Dim numBits As Integer = k And &H1f
			If numBits <> 0 Then
				' V = R * c (mod 2^j)
				Dim v As Integer = r * c.value(c.offset + c.intLen-1)
				v = v And ((1<<numBits) - 1)
				' c = c + (v * p)
				p.mul(v, temp)
				c.add(temp)
				' c = c / 2^j
				c.rightShift(numBits)
			End If

			' In theory, c may be greater than p at this point (Very rare!)
			Do While c.compare(p) >= 0
				c.subtract(p)
			Loop

			Return c
		End Function

		''' <summary>
		''' Uses the extended Euclidean algorithm to compute the modInverse of base
		''' mod a modulus that is a power of 2. The modulus is 2^k.
		''' </summary>
		Friend Overridable Function euclidModInverse(ByVal k As Integer) As MutableBigInteger
			Dim b As New MutableBigInteger(1)
			b.leftShift(k)
			Dim [mod] As New MutableBigInteger(b)

			Dim a As New MutableBigInteger(Me)
			Dim q As New MutableBigInteger
			Dim r As MutableBigInteger = b.divide(a, q)

			Dim swapper As MutableBigInteger = b
			' swap b & r
			b = r
			r = swapper

			Dim t1 As New MutableBigInteger(q)
			Dim t0 As New MutableBigInteger(1)
			Dim temp As New MutableBigInteger

			Do While Not b.one
				r = a.divide(b, q)

				If r.intLen = 0 Then Throw New ArithmeticException("BigInteger not invertible.")

				swapper = r
				a = swapper

				If q.intLen = 1 Then
					t1.mul(q.value(q.offset), temp)
				Else
					q.multiply(t1, temp)
				End If
				swapper = q
				q = temp
				temp = swapper
				t0.add(q)

				If a.one Then Return t0

				r = b.divide(a, q)

				If r.intLen = 0 Then Throw New ArithmeticException("BigInteger not invertible.")

				swapper = b
				b = r

				If q.intLen = 1 Then
					t0.mul(q.value(q.offset), temp)
				Else
					q.multiply(t0, temp)
				End If
				swapper = q
				q = temp
				temp = swapper

				t1.add(q)
			Loop
			[mod].subtract(t1)
			Return [mod]
		End Function
	End Class

End Namespace
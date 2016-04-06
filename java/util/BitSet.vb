Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 1995, 2014, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util


	''' <summary>
	''' This class implements a vector of bits that grows as needed. Each
	''' component of the bit set has a {@code boolean} value. The
	''' bits of a {@code BitSet} are indexed by nonnegative integers.
	''' Individual indexed bits can be examined, set, or cleared. One
	''' {@code BitSet} may be used to modify the contents of another
	''' {@code BitSet} through logical AND, logical inclusive OR, and
	''' logical exclusive OR operations.
	''' 
	''' <p>By default, all bits in the set initially have the value
	''' {@code false}.
	''' 
	''' <p>Every bit set has a current size, which is the number of bits
	''' of space currently in use by the bit set. Note that the size is
	''' related to the implementation of a bit set, so it may change with
	''' implementation. The length of a bit set relates to logical length
	''' of a bit set and is defined independently of implementation.
	''' 
	''' <p>Unless otherwise noted, passing a null parameter to any of the
	''' methods in a {@code BitSet} will result in a
	''' {@code NullPointerException}.
	''' 
	''' <p>A {@code BitSet} is not safe for multithreaded use without
	''' external synchronization.
	''' 
	''' @author  Arthur van Hoff
	''' @author  Michael McCloskey
	''' @author  Martin Buchholz
	''' @since   JDK1.0
	''' </summary>
	<Serializable> _
	Public Class BitSet
		Implements Cloneable

	'    
	'     * BitSets are packed into arrays of "words."  Currently a word is
	'     * a long, which consists of 64 bits, requiring 6 address bits.
	'     * The choice of word size is determined purely by performance concerns.
	'     
		Private Const ADDRESS_BITS_PER_WORD As Integer = 6
		Private Shared ReadOnly BITS_PER_WORD As Integer = 1 << ADDRESS_BITS_PER_WORD
		Private Shared ReadOnly BIT_INDEX_MASK As Integer = BITS_PER_WORD - 1

		' Used to shift left or right for a partial word mask 
		Private Const WORD_MASK As Long = &HffffffffffffffffL

		''' <summary>
		''' @serialField bits long[]
		''' 
		''' The bits in this BitSet.  The ith bit is stored in bits[i/64] at
		''' bit position i % 64 (where bit position 0 refers to the least
		''' significant bit and 63 refers to the most significant bit).
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As ObjectStreamField() = { New ObjectStreamField("bits", GetType(Long())) }

		''' <summary>
		''' The internal field corresponding to the serialField "bits".
		''' </summary>
		Private words As Long()

		''' <summary>
		''' The number of words in the logical size of this BitSet.
		''' </summary>
		<NonSerialized> _
		Private wordsInUse As Integer = 0

		''' <summary>
		''' Whether the size of "words" is user-specified.  If so, we assume
		''' the user knows what he's doing and try harder to preserve it.
		''' </summary>
		<NonSerialized> _
		Private sizeIsSticky As Boolean = False

		' use serialVersionUID from JDK 1.0.2 for interoperability 
		Private Const serialVersionUID As Long = 7997698588986878753L

		''' <summary>
		''' Given a bit index, return word index containing it.
		''' </summary>
		Private Shared Function wordIndex(  bitIndex As Integer) As Integer
			Return bitIndex >> ADDRESS_BITS_PER_WORD
		End Function

		''' <summary>
		''' Every public method must preserve these invariants.
		''' </summary>
		Private Sub checkInvariants()
			assert(wordsInUse = 0 OrElse words(wordsInUse - 1) <> 0)
			assert(wordsInUse >= 0 AndAlso wordsInUse <= words.Length)
			assert(wordsInUse = words.Length OrElse words(wordsInUse) = 0)
		End Sub

		''' <summary>
		''' Sets the field wordsInUse to the logical size in words of the bit set.
		''' WARNING:This method assumes that the number of words actually in use is
		''' less than or equal to the current value of wordsInUse!
		''' </summary>
		Private Sub recalculateWordsInUse()
			' Traverse the bitset until a used word is found
			Dim i As Integer
			For i = wordsInUse-1 To 0 Step -1
				If words(i) <> 0 Then Exit For
			Next i

			wordsInUse = i+1 ' The new logical size
		End Sub

		''' <summary>
		''' Creates a new bit set. All bits are initially {@code false}.
		''' </summary>
		Public Sub New()
			initWords(BITS_PER_WORD)
			sizeIsSticky = False
		End Sub

		''' <summary>
		''' Creates a bit set whose initial size is large enough to explicitly
		''' represent bits with indices in the range {@code 0} through
		''' {@code nbits-1}. All bits are initially {@code false}.
		''' </summary>
		''' <param name="nbits"> the initial size of the bit set </param>
		''' <exception cref="NegativeArraySizeException"> if the specified initial size
		'''         is negative </exception>
		Public Sub New(  nbits As Integer)
			' nbits can't be negative; size 0 is OK
			If nbits < 0 Then Throw New NegativeArraySizeException("nbits < 0: " & nbits)

			initWords(nbits)
			sizeIsSticky = True
		End Sub

		Private Sub initWords(  nbits As Integer)
			words = New Long(wordIndex(nbits-1)){}
		End Sub

		''' <summary>
		''' Creates a bit set using words as the internal representation.
		''' The last word (if there is one) must be non-zero.
		''' </summary>
		Private Sub New(  words As Long())
			Me.words = words
			Me.wordsInUse = words.Length
			checkInvariants()
		End Sub

		''' <summary>
		''' Returns a new bit set containing all the bits in the given long array.
		''' 
		''' <p>More precisely,
		''' <br>{@code BitSet.valueOf(longs).get(n) == ((longs[n/64] & (1L<<(n%64))) != 0)}
		''' <br>for all {@code n < 64 * longs.length}.
		''' 
		''' <p>This method is equivalent to
		''' {@code BitSet.valueOf(LongBuffer.wrap(longs))}.
		''' </summary>
		''' <param name="longs"> a long array containing a little-endian representation
		'''        of a sequence of bits to be used as the initial bits of the
		'''        new bit set </param>
		''' <returns> a {@code BitSet} containing all the bits in the long array
		''' @since 1.7 </returns>
		Public Shared Function valueOf(  longs As Long()) As BitSet
			Dim n As Integer
			n = longs.Length
			Do While n > 0 AndAlso longs(n - 1) = 0

				n -= 1
			Loop
			Return New BitSet(Arrays.copyOf(longs, n))
		End Function

		''' <summary>
		''' Returns a new bit set containing all the bits in the given long
		''' buffer between its position and limit.
		''' 
		''' <p>More precisely,
		''' <br>{@code BitSet.valueOf(lb).get(n) == ((lb.get(lb.position()+n/64) & (1L<<(n%64))) != 0)}
		''' <br>for all {@code n < 64 * lb.remaining()}.
		''' 
		''' <p>The long buffer is not modified by this method, and no
		''' reference to the buffer is retained by the bit set.
		''' </summary>
		''' <param name="lb"> a long buffer containing a little-endian representation
		'''        of a sequence of bits between its position and limit, to be
		'''        used as the initial bits of the new bit set </param>
		''' <returns> a {@code BitSet} containing all the bits in the buffer in the
		'''         specified range
		''' @since 1.7 </returns>
		Public Shared Function valueOf(  lb As java.nio.LongBuffer) As BitSet
			lb = lb.slice()
			Dim n As Integer
			n = lb.remaining()
			Do While n > 0 AndAlso lb.get(n - 1) = 0

				n -= 1
			Loop
			Dim words As Long() = New Long(n - 1){}
			lb.get(words)
			Return New BitSet(words)
		End Function

		''' <summary>
		''' Returns a new bit set containing all the bits in the given byte array.
		''' 
		''' <p>More precisely,
		''' <br>{@code BitSet.valueOf(bytes).get(n) == ((bytes[n/8] & (1<<(n%8))) != 0)}
		''' <br>for all {@code n <  8 * bytes.length}.
		''' 
		''' <p>This method is equivalent to
		''' {@code BitSet.valueOf(ByteBuffer.wrap(bytes))}.
		''' </summary>
		''' <param name="bytes"> a byte array containing a little-endian
		'''        representation of a sequence of bits to be used as the
		'''        initial bits of the new bit set </param>
		''' <returns> a {@code BitSet} containing all the bits in the byte array
		''' @since 1.7 </returns>
		Public Shared Function valueOf(  bytes As SByte()) As BitSet
			Return BitSet.valueOf(java.nio.ByteBuffer.wrap(bytes))
		End Function

		''' <summary>
		''' Returns a new bit set containing all the bits in the given byte
		''' buffer between its position and limit.
		''' 
		''' <p>More precisely,
		''' <br>{@code BitSet.valueOf(bb).get(n) == ((bb.get(bb.position()+n/8) & (1<<(n%8))) != 0)}
		''' <br>for all {@code n < 8 * bb.remaining()}.
		''' 
		''' <p>The byte buffer is not modified by this method, and no
		''' reference to the buffer is retained by the bit set.
		''' </summary>
		''' <param name="bb"> a byte buffer containing a little-endian representation
		'''        of a sequence of bits between its position and limit, to be
		'''        used as the initial bits of the new bit set </param>
		''' <returns> a {@code BitSet} containing all the bits in the buffer in the
		'''         specified range
		''' @since 1.7 </returns>
		Public Shared Function valueOf(  bb As java.nio.ByteBuffer) As BitSet
			bb = bb.slice().order(java.nio.ByteOrder.LITTLE_ENDIAN)
			Dim n As Integer
			n = bb.remaining()
			Do While n > 0 AndAlso bb.get(n - 1) = 0

				n -= 1
			Loop
			Dim words As Long() = New Long((n + 7) \ 8 - 1){}
			bb.limit(n)
			Dim i As Integer = 0
			Do While bb.remaining() >= 8
				words(i) = bb.long
				i += 1
			Loop
			Dim remaining As Integer = bb.remaining()
			Dim j As Integer = 0
			Do While j < remaining
				words(i) = words(i) Or (bb.get() And &HffL) << (8 * j)
				j += 1
			Loop
			Return New BitSet(words)
		End Function

		''' <summary>
		''' Returns a new byte array containing all the bits in this bit set.
		''' 
		''' <p>More precisely, if
		''' <br>{@code byte[] bytes = s.toByteArray();}
		''' <br>then {@code bytes.length == (s.length()+7)/8} and
		''' <br>{@code s.get(n) == ((bytes[n/8] & (1<<(n%8))) != 0)}
		''' <br>for all {@code n < 8 * bytes.length}.
		''' </summary>
		''' <returns> a byte array containing a little-endian representation
		'''         of all the bits in this bit set
		''' @since 1.7 </returns>
		Public Overridable Function toByteArray() As SByte()
			Dim n As Integer = wordsInUse
			If n = 0 Then Return New SByte(){}
			Dim len As Integer = 8 * (n-1)
			Dim x As Long = words(n - 1)
			Do While x <> 0
				len += 1
				x >>>= 8
			Loop
			Dim bytes As SByte() = New SByte(len - 1){}
			Dim bb As java.nio.ByteBuffer = java.nio.ByteBuffer.wrap(bytes).order(java.nio.ByteOrder.LITTLE_ENDIAN)
			For i As Integer = 0 To n - 2
				bb.putLong(words(i))
			Next i
			x = words(n - 1)
			Do While x <> 0
				bb.put(CByte(x And &Hff))
				x >>>= 8
			Loop
			Return bytes
		End Function

		''' <summary>
		''' Returns a new long array containing all the bits in this bit set.
		''' 
		''' <p>More precisely, if
		''' <br>{@code long[] longs = s.toLongArray();}
		''' <br>then {@code longs.length == (s.length()+63)/64} and
		''' <br>{@code s.get(n) == ((longs[n/64] & (1L<<(n%64))) != 0)}
		''' <br>for all {@code n < 64 * longs.length}.
		''' </summary>
		''' <returns> a long array containing a little-endian representation
		'''         of all the bits in this bit set
		''' @since 1.7 </returns>
		Public Overridable Function toLongArray() As Long()
			Return Arrays.copyOf(words, wordsInUse)
		End Function

		''' <summary>
		''' Ensures that the BitSet can hold enough words. </summary>
		''' <param name="wordsRequired"> the minimum acceptable number of words. </param>
		Private Sub ensureCapacity(  wordsRequired As Integer)
			If words.Length < wordsRequired Then
				' Allocate larger of doubled size or required size
				Dim request As Integer = System.Math.Max(2 * words.Length, wordsRequired)
				words = New Long(request - 1){}
				Array.Copy(words, words, request)
				sizeIsSticky = False
			End If
		End Sub

		''' <summary>
		''' Ensures that the BitSet can accommodate a given wordIndex,
		''' temporarily violating the invariants.  The caller must
		''' restore the invariants before returning to the user,
		''' possibly using recalculateWordsInUse(). </summary>
		''' <param name="wordIndex"> the index to be accommodated. </param>
		Private Sub expandTo(  wordIndex As Integer)
			Dim wordsRequired As Integer = wordIndex+1
			If wordsInUse < wordsRequired Then
				ensureCapacity(wordsRequired)
				wordsInUse = wordsRequired
			End If
		End Sub

		''' <summary>
		''' Checks that fromIndex ... toIndex is a valid range of bit indices.
		''' </summary>
		Private Shared Sub checkRange(  fromIndex As Integer,   toIndex As Integer)
			If fromIndex < 0 Then Throw New IndexOutOfBoundsException("fromIndex < 0: " & fromIndex)
			If toIndex < 0 Then Throw New IndexOutOfBoundsException("toIndex < 0: " & toIndex)
			If fromIndex > toIndex Then Throw New IndexOutOfBoundsException("fromIndex: " & fromIndex & " > toIndex: " & toIndex)
		End Sub

		''' <summary>
		''' Sets the bit at the specified index to the complement of its
		''' current value.
		''' </summary>
		''' <param name="bitIndex"> the index of the bit to flip </param>
		''' <exception cref="IndexOutOfBoundsException"> if the specified index is negative
		''' @since  1.4 </exception>
		Public Overridable Sub flip(  bitIndex As Integer)
			If bitIndex < 0 Then Throw New IndexOutOfBoundsException("bitIndex < 0: " & bitIndex)

			Dim wordIndex As Integer = wordIndex(bitIndex)
			expandTo(wordIndex)

			words(wordIndex) = words(wordIndex) Xor (1L << bitIndex)

			recalculateWordsInUse()
			checkInvariants()
		End Sub

		''' <summary>
		''' Sets each bit from the specified {@code fromIndex} (inclusive) to the
		''' specified {@code toIndex} (exclusive) to the complement of its current
		''' value.
		''' </summary>
		''' <param name="fromIndex"> index of the first bit to flip </param>
		''' <param name="toIndex"> index after the last bit to flip </param>
		''' <exception cref="IndexOutOfBoundsException"> if {@code fromIndex} is negative,
		'''         or {@code toIndex} is negative, or {@code fromIndex} is
		'''         larger than {@code toIndex}
		''' @since  1.4 </exception>
		Public Overridable Sub flip(  fromIndex As Integer,   toIndex As Integer)
			checkRange(fromIndex, toIndex)

			If fromIndex = toIndex Then Return

			Dim startWordIndex As Integer = wordIndex(fromIndex)
			Dim endWordIndex As Integer = wordIndex(toIndex - 1)
			expandTo(endWordIndex)

			Dim firstWordMask As Long = WORD_MASK << fromIndex
			Dim lastWordMask As Long = CLng(CULng(WORD_MASK) >> -toIndex)
			If startWordIndex = endWordIndex Then
				' Case 1: One word
				words(startWordIndex) = words(startWordIndex) Xor (firstWordMask And lastWordMask)
			Else
				' Case 2: Multiple words
				' Handle first word
				words(startWordIndex) = words(startWordIndex) Xor firstWordMask

				' Handle intermediate words, if any
				Dim i As Integer = startWordIndex+1
				Do While i < endWordIndex
					words(i) = words(i) Xor WORD_MASK
					i += 1
				Loop

				' Handle last word
				words(endWordIndex) = words(endWordIndex) Xor lastWordMask
			End If

			recalculateWordsInUse()
			checkInvariants()
		End Sub

		''' <summary>
		''' Sets the bit at the specified index to {@code true}.
		''' </summary>
		''' <param name="bitIndex"> a bit index </param>
		''' <exception cref="IndexOutOfBoundsException"> if the specified index is negative
		''' @since  JDK1.0 </exception>
		Public Overridable Sub [set](  bitIndex As Integer)
			If bitIndex < 0 Then Throw New IndexOutOfBoundsException("bitIndex < 0: " & bitIndex)

			Dim wordIndex As Integer = wordIndex(bitIndex)
			expandTo(wordIndex)

			words(wordIndex) = words(wordIndex) Or (1L << bitIndex) ' Restores invariants

			checkInvariants()
		End Sub

		''' <summary>
		''' Sets the bit at the specified index to the specified value.
		''' </summary>
		''' <param name="bitIndex"> a bit index </param>
		''' <param name="value"> a boolean value to set </param>
		''' <exception cref="IndexOutOfBoundsException"> if the specified index is negative
		''' @since  1.4 </exception>
		Public Overridable Sub [set](  bitIndex As Integer,   value As Boolean)
			If value Then
				[set](bitIndex)
			Else
				clear(bitIndex)
			End If
		End Sub

		''' <summary>
		''' Sets the bits from the specified {@code fromIndex} (inclusive) to the
		''' specified {@code toIndex} (exclusive) to {@code true}.
		''' </summary>
		''' <param name="fromIndex"> index of the first bit to be set </param>
		''' <param name="toIndex"> index after the last bit to be set </param>
		''' <exception cref="IndexOutOfBoundsException"> if {@code fromIndex} is negative,
		'''         or {@code toIndex} is negative, or {@code fromIndex} is
		'''         larger than {@code toIndex}
		''' @since  1.4 </exception>
		Public Overridable Sub [set](  fromIndex As Integer,   toIndex As Integer)
			checkRange(fromIndex, toIndex)

			If fromIndex = toIndex Then Return

			' Increase capacity if necessary
			Dim startWordIndex As Integer = wordIndex(fromIndex)
			Dim endWordIndex As Integer = wordIndex(toIndex - 1)
			expandTo(endWordIndex)

			Dim firstWordMask As Long = WORD_MASK << fromIndex
			Dim lastWordMask As Long = CLng(CULng(WORD_MASK) >> -toIndex)
			If startWordIndex = endWordIndex Then
				' Case 1: One word
				words(startWordIndex) = words(startWordIndex) Or (firstWordMask And lastWordMask)
			Else
				' Case 2: Multiple words
				' Handle first word
				words(startWordIndex) = words(startWordIndex) Or firstWordMask

				' Handle intermediate words, if any
				Dim i As Integer = startWordIndex+1
				Do While i < endWordIndex
					words(i) = WORD_MASK
					i += 1
				Loop

				' Handle last word (restores invariants)
				words(endWordIndex) = words(endWordIndex) Or lastWordMask
			End If

			checkInvariants()
		End Sub

		''' <summary>
		''' Sets the bits from the specified {@code fromIndex} (inclusive) to the
		''' specified {@code toIndex} (exclusive) to the specified value.
		''' </summary>
		''' <param name="fromIndex"> index of the first bit to be set </param>
		''' <param name="toIndex"> index after the last bit to be set </param>
		''' <param name="value"> value to set the selected bits to </param>
		''' <exception cref="IndexOutOfBoundsException"> if {@code fromIndex} is negative,
		'''         or {@code toIndex} is negative, or {@code fromIndex} is
		'''         larger than {@code toIndex}
		''' @since  1.4 </exception>
		Public Overridable Sub [set](  fromIndex As Integer,   toIndex As Integer,   value As Boolean)
			If value Then
				[set](fromIndex, toIndex)
			Else
				clear(fromIndex, toIndex)
			End If
		End Sub

		''' <summary>
		''' Sets the bit specified by the index to {@code false}.
		''' </summary>
		''' <param name="bitIndex"> the index of the bit to be cleared </param>
		''' <exception cref="IndexOutOfBoundsException"> if the specified index is negative
		''' @since  JDK1.0 </exception>
		Public Overridable Sub clear(  bitIndex As Integer)
			If bitIndex < 0 Then Throw New IndexOutOfBoundsException("bitIndex < 0: " & bitIndex)

			Dim wordIndex As Integer = wordIndex(bitIndex)
			If wordIndex >= wordsInUse Then Return

			words(wordIndex) = words(wordIndex) And Not(1L << bitIndex)

			recalculateWordsInUse()
			checkInvariants()
		End Sub

		''' <summary>
		''' Sets the bits from the specified {@code fromIndex} (inclusive) to the
		''' specified {@code toIndex} (exclusive) to {@code false}.
		''' </summary>
		''' <param name="fromIndex"> index of the first bit to be cleared </param>
		''' <param name="toIndex"> index after the last bit to be cleared </param>
		''' <exception cref="IndexOutOfBoundsException"> if {@code fromIndex} is negative,
		'''         or {@code toIndex} is negative, or {@code fromIndex} is
		'''         larger than {@code toIndex}
		''' @since  1.4 </exception>
		Public Overridable Sub clear(  fromIndex As Integer,   toIndex As Integer)
			checkRange(fromIndex, toIndex)

			If fromIndex = toIndex Then Return

			Dim startWordIndex As Integer = wordIndex(fromIndex)
			If startWordIndex >= wordsInUse Then Return

			Dim endWordIndex As Integer = wordIndex(toIndex - 1)
			If endWordIndex >= wordsInUse Then
				toIndex = length()
				endWordIndex = wordsInUse - 1
			End If

			Dim firstWordMask As Long = WORD_MASK << fromIndex
			Dim lastWordMask As Long = CLng(CULng(WORD_MASK) >> -toIndex)
			If startWordIndex = endWordIndex Then
				' Case 1: One word
				words(startWordIndex) = words(startWordIndex) And Not(firstWordMask And lastWordMask)
			Else
				' Case 2: Multiple words
				' Handle first word
				words(startWordIndex) = words(startWordIndex) And Not firstWordMask

				' Handle intermediate words, if any
				For i As Integer = startWordIndex+1 To endWordIndex - 1
					words(i) = 0
				Next i

				' Handle last word
				words(endWordIndex) = words(endWordIndex) And Not lastWordMask
			End If

			recalculateWordsInUse()
			checkInvariants()
		End Sub

		''' <summary>
		''' Sets all of the bits in this BitSet to {@code false}.
		''' 
		''' @since 1.4
		''' </summary>
		Public Overridable Sub clear()
			Do While wordsInUse > 0
				wordsInUse -= 1
				words(wordsInUse) = 0
			Loop
		End Sub

		''' <summary>
		''' Returns the value of the bit with the specified index. The value
		''' is {@code true} if the bit with the index {@code bitIndex}
		''' is currently set in this {@code BitSet}; otherwise, the result
		''' is {@code false}.
		''' </summary>
		''' <param name="bitIndex">   the bit index </param>
		''' <returns> the value of the bit with the specified index </returns>
		''' <exception cref="IndexOutOfBoundsException"> if the specified index is negative </exception>
		Public Overridable Function [get](  bitIndex As Integer) As Boolean
			If bitIndex < 0 Then Throw New IndexOutOfBoundsException("bitIndex < 0: " & bitIndex)

			checkInvariants()

			Dim wordIndex As Integer = wordIndex(bitIndex)
			Return (wordIndex < wordsInUse) AndAlso ((words(wordIndex) And (1L << bitIndex)) <> 0)
		End Function

		''' <summary>
		''' Returns a new {@code BitSet} composed of bits from this {@code BitSet}
		''' from {@code fromIndex} (inclusive) to {@code toIndex} (exclusive).
		''' </summary>
		''' <param name="fromIndex"> index of the first bit to include </param>
		''' <param name="toIndex"> index after the last bit to include </param>
		''' <returns> a new {@code BitSet} from a range of this {@code BitSet} </returns>
		''' <exception cref="IndexOutOfBoundsException"> if {@code fromIndex} is negative,
		'''         or {@code toIndex} is negative, or {@code fromIndex} is
		'''         larger than {@code toIndex}
		''' @since  1.4 </exception>
		Public Overridable Function [get](  fromIndex As Integer,   toIndex As Integer) As BitSet
			checkRange(fromIndex, toIndex)

			checkInvariants()

			Dim len As Integer = length()

			' If no set bits in range return empty bitset
			If len <= fromIndex OrElse fromIndex = toIndex Then Return New BitSet(0)

			' An optimization
			If toIndex > len Then toIndex = len

			Dim result As New BitSet(toIndex - fromIndex)
			Dim targetWords As Integer = wordIndex(toIndex - fromIndex - 1) + 1
			Dim sourceIndex As Integer = wordIndex(fromIndex)
			Dim wordAligned As Boolean = ((fromIndex And BIT_INDEX_MASK) = 0)

			' Process all words but the last word
			Dim i As Integer = 0
			Do While i < targetWords - 1
				result.words(i) = If(wordAligned, words(sourceIndex), (CLng(CULng(words(sourceIndex)) >> fromIndex)) Or (words(sourceIndex+1) << -fromIndex))
				i += 1
				sourceIndex += 1
			Loop

			' Process the last word
			Dim lastWordMask As Long = CLng(CULng(WORD_MASK) >> -toIndex)
			result.words(targetWords - 1) = If(((toIndex-1) And BIT_INDEX_MASK) < (fromIndex And BIT_INDEX_MASK), ((CLng(CULng(words(sourceIndex)) >> fromIndex)) Or (words(sourceIndex+1) And lastWordMask) << -fromIndex), (CInt(CUInt((words(sourceIndex) And lastWordMask)) >> fromIndex))) ' straddles source words

			' Set wordsInUse correctly
			result.wordsInUse = targetWords
			result.recalculateWordsInUse()
			result.checkInvariants()

			Return result
		End Function

		''' <summary>
		''' Returns the index of the first bit that is set to {@code true}
		''' that occurs on or after the specified starting index. If no such
		''' bit exists then {@code -1} is returned.
		''' 
		''' <p>To iterate over the {@code true} bits in a {@code BitSet},
		''' use the following loop:
		''' 
		'''  <pre> {@code
		''' for (int i = bs.nextSetBit(0); i >= 0; i = bs.nextSetBit(i+1)) {
		'''     // operate on index i here
		'''     if (i ==  java.lang.[Integer].MAX_VALUE) {
		'''         break; // or (i+1) would overflow
		'''     }
		''' }}</pre>
		''' </summary>
		''' <param name="fromIndex"> the index to start checking from (inclusive) </param>
		''' <returns> the index of the next set bit, or {@code -1} if there
		'''         is no such bit </returns>
		''' <exception cref="IndexOutOfBoundsException"> if the specified index is negative
		''' @since  1.4 </exception>
		Public Overridable Function nextSetBit(  fromIndex As Integer) As Integer
			If fromIndex < 0 Then Throw New IndexOutOfBoundsException("fromIndex < 0: " & fromIndex)

			checkInvariants()

			Dim u As Integer = wordIndex(fromIndex)
			If u >= wordsInUse Then Return -1

			Dim word As Long = words(u) And (WORD_MASK << fromIndex)

			Do
				If word <> 0 Then Return (u * BITS_PER_WORD) + java.lang.[Long].numberOfTrailingZeros(word)
				u += 1
				If u = wordsInUse Then Return -1
				word = words(u)
			Loop
		End Function

		''' <summary>
		''' Returns the index of the first bit that is set to {@code false}
		''' that occurs on or after the specified starting index.
		''' </summary>
		''' <param name="fromIndex"> the index to start checking from (inclusive) </param>
		''' <returns> the index of the next clear bit </returns>
		''' <exception cref="IndexOutOfBoundsException"> if the specified index is negative
		''' @since  1.4 </exception>
		Public Overridable Function nextClearBit(  fromIndex As Integer) As Integer
			' Neither spec nor implementation handle bitsets of maximal length.
			' See 4816253.
			If fromIndex < 0 Then Throw New IndexOutOfBoundsException("fromIndex < 0: " & fromIndex)

			checkInvariants()

			Dim u As Integer = wordIndex(fromIndex)
			If u >= wordsInUse Then Return fromIndex

			Dim word As Long = (Not words(u)) And (WORD_MASK << fromIndex)

			Do
				If word <> 0 Then Return (u * BITS_PER_WORD) + java.lang.[Long].numberOfTrailingZeros(word)
				u += 1
				If u = wordsInUse Then Return wordsInUse * BITS_PER_WORD
				word = Not words(u)
			Loop
		End Function

		''' <summary>
		''' Returns the index of the nearest bit that is set to {@code true}
		''' that occurs on or before the specified starting index.
		''' If no such bit exists, or if {@code -1} is given as the
		''' starting index, then {@code -1} is returned.
		''' 
		''' <p>To iterate over the {@code true} bits in a {@code BitSet},
		''' use the following loop:
		''' 
		'''  <pre> {@code
		''' for (int i = bs.length(); (i = bs.previousSetBit(i-1)) >= 0; ) {
		'''     // operate on index i here
		''' }}</pre>
		''' </summary>
		''' <param name="fromIndex"> the index to start checking from (inclusive) </param>
		''' <returns> the index of the previous set bit, or {@code -1} if there
		'''         is no such bit </returns>
		''' <exception cref="IndexOutOfBoundsException"> if the specified index is less
		'''         than {@code -1}
		''' @since  1.7 </exception>
		Public Overridable Function previousSetBit(  fromIndex As Integer) As Integer
			If fromIndex < 0 Then
				If fromIndex = -1 Then Return -1
				Throw New IndexOutOfBoundsException("fromIndex < -1: " & fromIndex)
			End If

			checkInvariants()

			Dim u As Integer = wordIndex(fromIndex)
			If u >= wordsInUse Then Return length() - 1

			Dim word As Long = words(u) And (CLng(CULng(WORD_MASK) >> -(fromIndex+1)))

			Do
				If word <> 0 Then Return (u+1) * BITS_PER_WORD - 1 - java.lang.[Long].numberOfLeadingZeros(word)
				Dim tempVar As Boolean = u = 0
				u -= 1
				If tempVar Then Return -1
				word = words(u)
			Loop
		End Function

		''' <summary>
		''' Returns the index of the nearest bit that is set to {@code false}
		''' that occurs on or before the specified starting index.
		''' If no such bit exists, or if {@code -1} is given as the
		''' starting index, then {@code -1} is returned.
		''' </summary>
		''' <param name="fromIndex"> the index to start checking from (inclusive) </param>
		''' <returns> the index of the previous clear bit, or {@code -1} if there
		'''         is no such bit </returns>
		''' <exception cref="IndexOutOfBoundsException"> if the specified index is less
		'''         than {@code -1}
		''' @since  1.7 </exception>
		Public Overridable Function previousClearBit(  fromIndex As Integer) As Integer
			If fromIndex < 0 Then
				If fromIndex = -1 Then Return -1
				Throw New IndexOutOfBoundsException("fromIndex < -1: " & fromIndex)
			End If

			checkInvariants()

			Dim u As Integer = wordIndex(fromIndex)
			If u >= wordsInUse Then Return fromIndex

			Dim word As Long = (Not words(u)) And (CLng(CULng(WORD_MASK) >> -(fromIndex+1)))

			Do
				If word <> 0 Then Return (u+1) * BITS_PER_WORD -1 - java.lang.[Long].numberOfLeadingZeros(word)
				Dim tempVar As Boolean = u = 0
				u -= 1
				If tempVar Then Return -1
				word = Not words(u)
			Loop
		End Function

		''' <summary>
		''' Returns the "logical size" of this {@code BitSet}: the index of
		''' the highest set bit in the {@code BitSet} plus one. Returns zero
		''' if the {@code BitSet} contains no set bits.
		''' </summary>
		''' <returns> the logical size of this {@code BitSet}
		''' @since  1.2 </returns>
		Public Overridable Function length() As Integer
			If wordsInUse = 0 Then Return 0

			Return BITS_PER_WORD * (wordsInUse - 1) + (BITS_PER_WORD - java.lang.[Long].numberOfLeadingZeros(words(wordsInUse - 1)))
		End Function

		''' <summary>
		''' Returns true if this {@code BitSet} contains no bits that are set
		''' to {@code true}.
		''' </summary>
		''' <returns> boolean indicating whether this {@code BitSet} is empty
		''' @since  1.4 </returns>
		Public Overridable Property empty As Boolean
			Get
				Return wordsInUse = 0
			End Get
		End Property

		''' <summary>
		''' Returns true if the specified {@code BitSet} has any bits set to
		''' {@code true} that are also set to {@code true} in this {@code BitSet}.
		''' </summary>
		''' <param name="set"> {@code BitSet} to intersect with </param>
		''' <returns> boolean indicating whether this {@code BitSet} intersects
		'''         the specified {@code BitSet}
		''' @since  1.4 </returns>
		Public Overridable Function intersects(  [set] As BitSet) As Boolean
			For i As Integer = System.Math.Min(wordsInUse, [set].wordsInUse) - 1 To 0 Step -1
				If (words(i) And [set].words(i)) <> 0 Then Return True
			Next i
			Return False
		End Function

		''' <summary>
		''' Returns the number of bits set to {@code true} in this {@code BitSet}.
		''' </summary>
		''' <returns> the number of bits set to {@code true} in this {@code BitSet}
		''' @since  1.4 </returns>
		Public Overridable Function cardinality() As Integer
			Dim sum As Integer = 0
			Dim i As Integer = 0
			Do While i < wordsInUse
				sum += java.lang.[Long].bitCount(words(i))
				i += 1
			Loop
			Return sum
		End Function

		''' <summary>
		''' Performs a logical <b>AND</b> of this target bit set with the
		''' argument bit set. This bit set is modified so that each bit in it
		''' has the value {@code true} if and only if it both initially
		''' had the value {@code true} and the corresponding bit in the
		''' bit set argument also had the value {@code true}.
		''' </summary>
		''' <param name="set"> a bit set </param>
		Public Overridable Sub [and](  [set] As BitSet)
			If Me Is [set] Then Return

			Do While wordsInUse > [set].wordsInUse
				wordsInUse -= 1
				words(wordsInUse) = 0
			Loop

			' Perform logical AND on words in common
			Dim i As Integer = 0
			Do While i < wordsInUse
				words(i) = words(i) And [set].words(i)
				i += 1
			Loop

			recalculateWordsInUse()
			checkInvariants()
		End Sub

		''' <summary>
		''' Performs a logical <b>OR</b> of this bit set with the bit set
		''' argument. This bit set is modified so that a bit in it has the
		''' value {@code true} if and only if it either already had the
		''' value {@code true} or the corresponding bit in the bit set
		''' argument has the value {@code true}.
		''' </summary>
		''' <param name="set"> a bit set </param>
		Public Overridable Sub [or](  [set] As BitSet)
			If Me Is [set] Then Return

			Dim wordsInCommon As Integer = System.Math.Min(wordsInUse, [set].wordsInUse)

			If wordsInUse < [set].wordsInUse Then
				ensureCapacity([set].wordsInUse)
				wordsInUse = [set].wordsInUse
			End If

			' Perform logical OR on words in common
			For i As Integer = 0 To wordsInCommon - 1
				words(i) = words(i) Or [set].words(i)
			Next i

			' Copy any remaining words
			If wordsInCommon < [set].wordsInUse Then Array.Copy([set].words, wordsInCommon, words, wordsInCommon, wordsInUse - wordsInCommon)

			' recalculateWordsInUse() is unnecessary
			checkInvariants()
		End Sub

		''' <summary>
		''' Performs a logical <b>XOR</b> of this bit set with the bit set
		''' argument. This bit set is modified so that a bit in it has the
		''' value {@code true} if and only if one of the following
		''' statements holds:
		''' <ul>
		''' <li>The bit initially has the value {@code true}, and the
		'''     corresponding bit in the argument has the value {@code false}.
		''' <li>The bit initially has the value {@code false}, and the
		'''     corresponding bit in the argument has the value {@code true}.
		''' </ul>
		''' </summary>
		''' <param name="set"> a bit set </param>
		Public Overridable Sub [xor](  [set] As BitSet)
			Dim wordsInCommon As Integer = System.Math.Min(wordsInUse, [set].wordsInUse)

			If wordsInUse < [set].wordsInUse Then
				ensureCapacity([set].wordsInUse)
				wordsInUse = [set].wordsInUse
			End If

			' Perform logical XOR on words in common
			For i As Integer = 0 To wordsInCommon - 1
				words(i) = words(i) Xor [set].words(i)
			Next i

			' Copy any remaining words
			If wordsInCommon < [set].wordsInUse Then Array.Copy([set].words, wordsInCommon, words, wordsInCommon, [set].wordsInUse - wordsInCommon)

			recalculateWordsInUse()
			checkInvariants()
		End Sub

		''' <summary>
		''' Clears all of the bits in this {@code BitSet} whose corresponding
		''' bit is set in the specified {@code BitSet}.
		''' </summary>
		''' <param name="set"> the {@code BitSet} with which to mask this
		'''         {@code BitSet}
		''' @since  1.2 </param>
		Public Overridable Sub andNot(  [set] As BitSet)
			' Perform logical (a & !b) on words in common
			For i As Integer = System.Math.Min(wordsInUse, [set].wordsInUse) - 1 To 0 Step -1
				words(i) = words(i) And Not [set].words(i)
			Next i

			recalculateWordsInUse()
			checkInvariants()
		End Sub

		''' <summary>
		''' Returns the hash code value for this bit set. The hash code depends
		''' only on which bits are set within this {@code BitSet}.
		''' 
		''' <p>The hash code is defined to be the result of the following
		''' calculation:
		'''  <pre> {@code
		''' public int hashCode() {
		'''     long h = 1234;
		'''     long[] words = toLongArray();
		'''     for (int i = words.length; --i >= 0; )
		'''         h ^= words[i] * (i + 1);
		'''     return (int)((h >> 32) ^ h);
		''' }}</pre>
		''' Note that the hash code changes if the set of bits is altered.
		''' </summary>
		''' <returns> the hash code value for this bit set </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim h As Long = 1234
			Dim i As Integer = wordsInUse
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While i -= 1 >= 0
				h = h Xor words(i) * (i + 1)
			Loop

			Return CInt(Fix((h >> 32) Xor h))
		End Function

		''' <summary>
		''' Returns the number of bits of space actually in use by this
		''' {@code BitSet} to represent bit values.
		''' The maximum element in the set is the size - 1st element.
		''' </summary>
		''' <returns> the number of bits currently in this bit set </returns>
		Public Overridable Function size() As Integer
			Return words.Length * BITS_PER_WORD
		End Function

		''' <summary>
		''' Compares this object against the specified object.
		''' The result is {@code true} if and only if the argument is
		''' not {@code null} and is a {@code Bitset} object that has
		''' exactly the same set of bits set to {@code true} as this bit
		''' set. That is, for every nonnegative {@code int} index {@code k},
		''' <pre>((BitSet)obj).get(k) == this.get(k)</pre>
		''' must be true. The current sizes of the two bit sets are not compared.
		''' </summary>
		''' <param name="obj"> the object to compare with </param>
		''' <returns> {@code true} if the objects are the same;
		'''         {@code false} otherwise </returns>
		''' <seealso cref=    #size() </seealso>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If Not(TypeOf obj Is BitSet) Then Return False
			If Me Is obj Then Return True

			Dim [set] As BitSet = CType(obj, BitSet)

			checkInvariants()
			[set].checkInvariants()

			If wordsInUse <> [set].wordsInUse Then Return False

			' Check words in use by both BitSets
			For i As Integer = 0 To wordsInUse - 1
				If words(i) <> [set].words(i) Then Return False
			Next i

			Return True
		End Function

		''' <summary>
		''' Cloning this {@code BitSet} produces a new {@code BitSet}
		''' that is equal to it.
		''' The clone of the bit set is another bit set that has exactly the
		''' same bits set to {@code true} as this bit set.
		''' </summary>
		''' <returns> a clone of this bit set </returns>
		''' <seealso cref=    #size() </seealso>
		Public Overridable Function clone() As Object
			If Not sizeIsSticky Then trimToSize()

			Try
				Dim result As BitSet = CType(MyBase.clone(), BitSet)
				result.words = words.clone()
				result.checkInvariants()
				Return result
			Catch e As CloneNotSupportedException
				Throw New InternalError(e)
			End Try
		End Function

		''' <summary>
		''' Attempts to reduce internal storage used for the bits in this bit set.
		''' Calling this method may, but is not required to, affect the value
		''' returned by a subsequent call to the <seealso cref="#size()"/> method.
		''' </summary>
		Private Sub trimToSize()
			If wordsInUse <> words.Length Then
				words = New Long(wordsInUse - 1){}
				Array.Copy(words, words, wordsInUse)
				checkInvariants()
			End If
		End Sub

		''' <summary>
		''' Save the state of the {@code BitSet} instance to a stream (i.e.,
		''' serialize it).
		''' </summary>
		Private Sub writeObject(  s As ObjectOutputStream)

			checkInvariants()

			If Not sizeIsSticky Then trimToSize()

			Dim fields As ObjectOutputStream.PutField = s.putFields()
			fields.put("bits", words)
			s.writeFields()
		End Sub

		''' <summary>
		''' Reconstitute the {@code BitSet} instance from a stream (i.e.,
		''' deserialize it).
		''' </summary>
		Private Sub readObject(  s As ObjectInputStream)

			Dim fields As ObjectInputStream.GetField = s.readFields()
			words = CType(fields.get("bits", Nothing), Long())

			' Assume maximum length then find real length
			' because recalculateWordsInUse assumes maintenance
			' or reduction in logical size
			wordsInUse = words.Length
			recalculateWordsInUse()
			sizeIsSticky = (words.Length > 0 AndAlso words(words.Length-1) = 0L) ' heuristic
			checkInvariants()
		End Sub

		''' <summary>
		''' Returns a string representation of this bit set. For every index
		''' for which this {@code BitSet} contains a bit in the set
		''' state, the decimal representation of that index is included in
		''' the result. Such indices are listed in order from lowest to
		''' highest, separated by ",&nbsp;" (a comma and a space) and
		''' surrounded by braces, resulting in the usual mathematical
		''' notation for a set of integers.
		''' 
		''' <p>Example:
		''' <pre>
		''' BitSet drPepper = new BitSet();</pre>
		''' Now {@code drPepper.toString()} returns "{@code {}}".
		''' <pre>
		''' drPepper.set(2);</pre>
		''' Now {@code drPepper.toString()} returns "{@code {2}}".
		''' <pre>
		''' drPepper.set(4);
		''' drPepper.set(10);</pre>
		''' Now {@code drPepper.toString()} returns "{@code {2, 4, 10}}".
		''' </summary>
		''' <returns> a string representation of this bit set </returns>
		Public Overrides Function ToString() As String
			checkInvariants()

			Dim numBits As Integer = If(wordsInUse > 128, cardinality(), wordsInUse * BITS_PER_WORD)
			Dim b As New StringBuilder(6*numBits + 2)
			b.append("{"c)

			Dim i As Integer = nextSetBit(0)
			If i <> -1 Then
				b.append(i)
				Do
					i += 1
					If i < 0 Then Exit Do
					i = nextSetBit(i)
					If i < 0 Then Exit Do
					Dim endOfRun As Integer = nextClearBit(i)
					Do
						b.append(", ").append(i)
						i += 1
					Loop While i <> endOfRun
				Loop
			End If

			b.append("}"c)
			Return b.ToString()
		End Function

		''' <summary>
		''' Returns a stream of indices for which this {@code BitSet}
		''' contains a bit in the set state. The indices are returned
		''' in order, from lowest to highest. The size of the stream
		''' is the number of bits in the set state, equal to the value
		''' returned by the <seealso cref="#cardinality()"/> method.
		''' 
		''' <p>The bit set must remain constant during the execution of the
		''' terminal stream operation.  Otherwise, the result of the terminal
		''' stream operation is undefined.
		''' </summary>
		''' <returns> a stream of integers representing set indices
		''' @since 1.8 </returns>
		Public Overridable Function stream() As java.util.stream.IntStream
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class BitSetIterator implements PrimitiveIterator.OfInt
	'		{
	'			int next = nextSetBit(0);
	'
	'			@Override public boolean hasNext()
	'			{
	'				Return next != -1;
	'			}
	'
	'			@Override public int nextInt()
	'			{
	'				if (next != -1)
	'				{
	'					int ret = next;
	'					next = nextSetBit(next+1);
	'					Return ret;
	'				}
	'				else
	'				{
	'					throw New NoSuchElementException();
	'				}
	'			}
	'		}

			Return java.util.stream.StreamSupport.intStream(() -> Spliterators.spliterator(New BitSetIterator, cardinality(), Spliterator.ORDERED Or Spliterator.DISTINCT Or Spliterator.SORTED), Spliterator.SIZED Or Spliterator.SUBSIZED Or Spliterator.ORDERED Or Spliterator.DISTINCT Or Spliterator.SORTED, False)
		End Function
	End Class

End Namespace